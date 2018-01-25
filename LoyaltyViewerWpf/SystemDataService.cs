using System;
using System.Collections.Generic;
using System.Timers;
using System.Data;
using System.Reflection;
using System.Windows.Threading;

namespace LoyaltyViewerWpf {
	class SystemDataService {
		private int lastUpdateDay = 0;
		private int errorsCountMisDb = 0;
		private bool needToSendToStp = true;

		private ItemDataResult qualityResultToday = new ItemDataResult();
		private ItemDataResult qualityResultYesterday = new ItemDataResult();
		private ItemDataResult recommendationToday = new ItemDataResult();
		private ItemDataResult recommendationYesterday = new ItemDataResult();

		private enum Types {
			QualityToday,
			QualityYesterday,
			RecommendationToday,
			RecommendationYesterday
		};

		private ItemPromoJustNow _promoJustNow = new ItemPromoJustNow();
		
		private SystemFirebirdClient fBClient;

		public SystemDataService() {
			SystemLogging.LogMessageToFile("Инициализация сервиса данных");

			fBClient = new SystemFirebirdClient(
				Properties.Settings.Default.MisDbAddress,
				Properties.Settings.Default.MisDbName,
				Properties.Settings.Default.MisDbUserName,
				Properties.Settings.Default.MisDbPassword);

			DispatcherTimer dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.MisDbUpdatePeriodInSeconds);
			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Start();
			UpdateData();
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e) {
			UpdateData();
		}




		private void UpdateData() {
			SystemLogging.LogMessageToFile("Обновление данных");

			if (Properties.Settings.Default.ShowLoyaltyInfo) {
				SystemLogging.LogMessageToFile("Результаты голосования на мониторе лояльности");
				GetQualityData(Types.QualityToday);
				GetQualityData(Types.RecommendationToday);

				if (lastUpdateDay != DateTime.Now.Day) {
					errorsCountMisDb = 0;
					needToSendToStp = true;

					GetQualityData(Types.QualityYesterday);
					GetQualityData(Types.RecommendationYesterday);

					lastUpdateDay = DateTime.Now.Day;
				}
			}

			if (Properties.Settings.Default.ShowPromoJustNow) {
				SystemLogging.LogMessageToFile("Данные по свободным ячейкам в расписании");
				GetPromoJustNowData();
			}

			if (errorsCountMisDb > 30 && needToSendToStp) {
				SystemMail.SendMail("Сервису " + Assembly.GetExecutingAssembly().GetName().Name +
					" не удалось получить данные с сервера " + Properties.Settings.Default.MisDbAddress + ":" +
					Properties.Settings.Default.MisDbName + Environment.NewLine + Environment.NewLine);
				needToSendToStp = false;
			}
		}

		private void GetPromoJustNowData() {
			string queryDoctors = Properties.Settings.Default.MisDbSelectQueryPromoJustNowDoctors;
			string queryBusy = Properties.Settings.Default.MisDbSelectQueryPromoJustNowBusy;
			string depnums = Properties.Settings.Default.PromoJustNowDepartments;

			queryDoctors = queryDoctors.Replace("@depnums", depnums);
			queryBusy = queryBusy.Replace("@depnums", depnums);

			DataTable dataTableDoctors = fBClient.GetDataTable(queryDoctors, new Dictionary<string, string>(), ref errorsCountMisDb);
			DataTable dataTableBusy = fBClient.GetDataTable(queryBusy, new Dictionary<string, string>(), ref errorsCountMisDb);

			if (dataTableDoctors.Rows.Count == 0)
				return;

			DateTime dateTimeNow = GetDateTime("12", "00"); //DateTime.Now;
			DateTime dateTimeMaxToShow = dateTimeNow.AddMinutes(Properties.Settings.Default.PromoJustNowShowIntervalMinutes);

			SystemLogging.LogMessageToFile("Total doctors: " + dataTableDoctors.Rows.Count);

			foreach (DataRow rowDoctor in dataTableDoctors.Rows) {
				try {
					string fullname = rowDoctor["FULLNAME"].ToString();
					string dcode = rowDoctor["DCODE"].ToString();
					string depname = rowDoctor["DEPNAME"].ToString();
					string depnum = rowDoctor["DEPNUM"].ToString();
					string beghour = rowDoctor["BEGHOUR"].ToString();
					string begmin = rowDoctor["BEGMIN"].ToString();
					string endhour = rowDoctor["ENDHOUR"].ToString();
					string endmin = rowDoctor["ENDMIN"].ToString();
					string shinterv = rowDoctor["SHINTERV"].ToString();

					Console.WriteLine("fullname: " + fullname);

					DateTime dateTimeBegin = GetDateTime(beghour, begmin);
					DateTime dateTimeEnd = GetDateTime(endhour, endmin);

					if (dateTimeBegin >= dateTimeNow ||
						dateTimeEnd <= dateTimeNow) {
						SystemLogging.LogMessageToFile("Doctor '" + fullname + "' working interval outside current time");
						continue;
					}

					ItemDoctor itemDoctor = new ItemDoctor(fullname);
					int.TryParse(shinterv, out int duration);
					DataRow[] dataTableBusyCurrentDoc = dataTableBusy.Select("DCODE = " + dcode);
					DateTime dateTimeIntervalStart = dateTimeNow.AddMinutes(Properties.Settings.Default.PromoJustNowTreatStartDelayMinutes);
					
					while (true) {
						if (dateTimeIntervalStart >= dateTimeEnd ||
							dateTimeIntervalStart >= dateTimeMaxToShow)
							break;

						DateTime dateTimeIntervalEnd = dateTimeIntervalStart.AddMinutes(duration);

						if (dateTimeIntervalEnd > dateTimeEnd ||
							dateTimeIntervalEnd > dateTimeMaxToShow)
							break;

						bool isBusy = false;
						foreach (DataRow dataRowBusyCell in dataTableBusyCurrentDoc) {
							try {
								string bhour = dataRowBusyCell["BHOUR"].ToString();
								string bmin = dataRowBusyCell["BMIN"].ToString();
								string fhour = dataRowBusyCell["FHOUR"].ToString();
								string fmin = dataRowBusyCell["FMIN"].ToString();

								DateTime busyStart = GetDateTime(bhour, bmin);
								DateTime busyEnd = GetDateTime(fhour, fmin);

								if (busyStart >= dateTimeIntervalEnd ||
									busyEnd <= dateTimeIntervalStart)
									continue;

								isBusy = true;
								dateTimeIntervalStart = busyEnd;

								break;
							} catch (Exception excBusy) {
								SystemLogging.LogMessageToFile(excBusy.Message + Environment.NewLine + excBusy.StackTrace);
							}
						}

						if (isBusy)
							continue;

						ItemFreeCell itemFreeCell = new ItemFreeCell(dateTimeIntervalStart, dateTimeIntervalEnd);
						itemDoctor.FreeCells.Add(itemFreeCell);
						dateTimeIntervalStart = dateTimeIntervalEnd;
					}

					if (itemDoctor.FreeCells.Count == 0) {
						SystemLogging.LogMessageToFile("Doctor '" + itemDoctor.Name + "' has no free cells");
						continue;
					}

					SystemLogging.LogMessageToFile("Doctor '" + itemDoctor.Name + "' free cells count: " + itemDoctor.FreeCells.Count);

					if (_promoJustNow.Departments.ContainsKey(depname)) {
						_promoJustNow.Departments[depname].Doctors.Add(itemDoctor);
					} else {
						ItemDepartment itemDepartment = new ItemDepartment(depname);
						itemDepartment.Doctors.Add(itemDoctor);
						_promoJustNow.Departments.Add(depname, itemDepartment);
					}
				} catch (Exception excDoc) {
					SystemLogging.LogMessageToFile(excDoc.Message + Environment.NewLine + excDoc.StackTrace);
				}
			}

			Console.WriteLine();
		}

		private DateTime GetDateTime(string hour, string minute) {
			DateTime today = DateTime.Now;
			int.TryParse(hour, out int intHour);
			int.TryParse(minute, out int intMinute);
			return new DateTime(today.Year, today.Month, today.Day, intHour, intMinute, 0);
		}

		public ItemPromoJustNow GetPromoJustNow() {
			return _promoJustNow;
		}

		private void GetQualityData(Types type) {
			string query;
			string title;
			ItemDataResult dataResult = new ItemDataResult();
			int votesCount;
			int voteShift;
			string columnName;

			switch (type) {
				case Types.QualityToday:
					query = Properties.Settings.Default.MisDbSelectQueryDocrateToday;
					title = Properties.Resources.DoctorsMarksLabelToday;
					break;
				case Types.QualityYesterday:
					query = Properties.Settings.Default.MisDbSelectQueryDocrateYesterday;
					title = Properties.Resources.DoctorsMarksLabelYesterday;
					break;
				case Types.RecommendationToday:
					query = Properties.Settings.Default.MisDbSelectQueryClRecommendToday;
					title = Properties.Resources.RecommendationLabelToday;
					break;
				case Types.RecommendationYesterday:
					query = Properties.Settings.Default.MisDbSelectQueryClRecommendYesterday;
					title = Properties.Resources.RecommendationLabelYesterday;
					break;
				default:
					SystemLogging.LogMessageToFile("GetQualityData неверный тип данных");
					return;
			}

			if (type == Types.RecommendationToday ||
				type == Types.RecommendationYesterday) {
				votesCount = 11;
				voteShift = 0;
				columnName = "CL_RECOMMEND";
			} else {
				votesCount = 5;
				voteShift = 1;
				columnName = "DOCRATE";
			}

			DataTable dataTable = fBClient.GetDataTable(query, new Dictionary<string, string>(), ref errorsCountMisDb);
			int totalVotes = 0;

			int[] votes = new int[votesCount];
			foreach (DataRow row in dataTable.Rows) {
				try {
					int vote;

					if (!int.TryParse(row[columnName].ToString(), out vote))
						continue;

					votes[vote - voteShift]++;
					totalVotes++;
				} catch (Exception) {
					continue;
				}
			}

			if (totalVotes > 0)
				if (type == Types.QualityToday ||
					type == Types.QualityYesterday) {
					dataResult.PercentAngry = ((double)votes[0] / (double)totalVotes);
					dataResult.PercentSad = ((double)votes[1] / (double)totalVotes);
					dataResult.PercentNeutral = ((double)votes[2] / (double)totalVotes);
					dataResult.PercentHappy = ((double)votes[3] / (double)totalVotes);
					dataResult.PercentLove = 1 - dataResult.PercentAngry - dataResult.PercentSad -
						dataResult.PercentNeutral - dataResult.PercentHappy;
				} else if (
					type == Types.RecommendationToday ||
					type == Types.RecommendationYesterday) {
					dataResult.PercentDislike = ((double)(votes[0] + votes[1] + votes[2] + votes[3] + votes[4] + votes[5] + votes[6]) / 
						(double)totalVotes);
					dataResult.PercentDontKnow = ((double)(votes[7] + votes[8]) / totalVotes);
					dataResult.PercentLike = 1 - dataResult.PercentDislike - dataResult.PercentDontKnow;
				}
			
			dataResult.Total = totalVotes;
			dataResult.Description = title;

			switch (type) {
				case Types.QualityToday:
					qualityResultToday = dataResult;
					break;
				case Types.QualityYesterday:
					qualityResultYesterday = dataResult;
					break;
				case Types.RecommendationToday:
					recommendationToday = dataResult;
					break;
				case Types.RecommendationYesterday:
					recommendationYesterday = dataResult;
					break;
				default:
					break;
			}
		}

		public ItemDataResult GetQualityResult() {
			if (qualityResultToday.Total >= 5)
				return qualityResultToday;

			return qualityResultYesterday;
		}

		public ItemDataResult GetRecommendationResult() {
			if (recommendationToday.Total >= 5)
				return recommendationToday;

			return recommendationYesterday;
		}
	}
}
