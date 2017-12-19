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

		private enum Types { QualityToday, QualityYesterday, RecommendationToday, RecommendationYesterday };
		
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
			SystemLogging.LogMessageToFile("Запрос обновления данных");

			GetQualityData(Types.QualityToday);
			GetQualityData(Types.RecommendationToday);

			if (errorsCountMisDb > 30 && needToSendToStp) {
				SystemMail.SendMail("Сервису " + Assembly.GetExecutingAssembly().GetName().Name +
					" не удалось получить данные с сервера " + Properties.Settings.Default.MisDbAddress + ":" +
					Properties.Settings.Default.MisDbName + Environment.NewLine + Environment.NewLine +
					"Используемые запросы: " + Environment.NewLine +
					Properties.Settings.Default.MisDbSelectQueryDocrateToday + Environment.NewLine +
					Properties.Settings.Default.MisDbSelectQueryDocrateYesterday + Environment.NewLine +
					Properties.Settings.Default.MisDbSelectQueryClRecommendToday);
				needToSendToStp = false;
			}

			if (lastUpdateDay == DateTime.Now.Day)
				return;

			errorsCountMisDb = 0;
			needToSendToStp = true;

			GetQualityData(Types.QualityYesterday);
			GetQualityData(Types.RecommendationYesterday);

			lastUpdateDay = DateTime.Now.Day;
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
