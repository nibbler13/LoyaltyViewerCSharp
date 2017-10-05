using System;
using System.Collections.Generic;
using System.Timers;
using System.Data;
using System.Reflection;

namespace LoyaltyViewer {
	class DataService {
		private int lastUpdateDay = 0;
		private int errorsCountMisDb = 0;
		private bool needToSendToStp = true;

		private DataResult qualityResultToday = new DataResult();
		private DataResult qualityResultYesterday = new DataResult();
		private DataResult recommendationToday = new DataResult();
		private DataResult recommendationYesterday = new DataResult();

		private enum Types { QualityToday, QualityYesterday, RecommendationToday, RecommendationYesterday };
		
		private FBClient fBClient;

		public DataService() {
			LoggingSystem.LogMessageToFile("Инициализация сервиса данных");

			fBClient = new FBClient(
				Properties.Settings.Default.MisDbAddress,
				Properties.Settings.Default.MisDbName,
				Properties.Settings.Default.MisDbUserName,
				Properties.Settings.Default.MisDbPassword);

			Timer timer = new Timer();
			timer.Interval = Properties.Settings.Default.MisDbUpdatePeriodInSeconds * 1000;
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
			UpdateData();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
			UpdateData();
		}

		private void UpdateData() {
			LoggingSystem.LogMessageToFile("Запрос обновления данных");

			GetQualityData(Types.QualityToday);
			GetQualityData(Types.RecommendationToday);

			if (errorsCountMisDb > 30 && needToSendToStp) {
				MailSystem.SendMail("Сервису " + Assembly.GetExecutingAssembly().GetName().Name +
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
			DataResult dataResult;
			int votesCount;
			int voteShift;
			string columnName;

			if (type == Types.QualityToday) {
				query = Properties.Settings.Default.MisDbSelectQueryDocrateToday;
				dataResult = qualityResultToday;
				title = Properties.Resources.FormDoctorsMarksLabelToday;
			} else if (type == Types.QualityYesterday) {
				query = Properties.Settings.Default.MisDbSelectQueryDocrateYesterday;
				dataResult = qualityResultYesterday;
				title = Properties.Resources.FormDoctorsMarksLabelYesterday;
			} else if (type == Types.RecommendationToday) {
				query = Properties.Settings.Default.MisDbSelectQueryClRecommendToday;
				dataResult = recommendationToday;
				title = Properties.Resources.FormRecommendationLabelToday;
			} else if (type == Types.RecommendationYesterday) {
				query = Properties.Settings.Default.MisDbSelectQueryClRecommendYesterday;
				dataResult = recommendationYesterday;
				title = Properties.Resources.FormRecommendationLabelYesterday;
			} else {
				LoggingSystem.LogMessageToFile("GetQualityData неверный тип данных");
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

			int totalVotes = dataTable.Rows.Count;
			if (totalVotes == 0)
				return;

			int[] votes = new int[votesCount];
			foreach (DataRow row in dataTable.Rows) {
				try {
					int vote = int.Parse(row[columnName].ToString());
					votes[vote - voteShift]++;
				} catch (Exception) {
					continue;
				}
			}

			if (type == Types.QualityToday ||
				type == Types.QualityYesterday) {
				dataResult.percentAngry = (int)((double)votes[0] / (double)totalVotes * 100.0);
				dataResult.percentSad = (int)((double)votes[1] / (double)totalVotes * 100.0);
				dataResult.percentNeutral = (int)((double)votes[2] / (double)totalVotes * 100.0);
				dataResult.percentHappy = (int)((double)votes[3] / (double)totalVotes * 100.0);
				dataResult.percentLove = 100 - dataResult.percentAngry - dataResult.percentSad -
					dataResult.percentNeutral - dataResult.percentHappy;
			} else if (
				type == Types.RecommendationToday ||
				type == Types.RecommendationYesterday) {
				dataResult.percentDislike = (int)((double)(votes[0] + votes[1] + votes[2] + votes[3] + votes[4] + votes[5] + votes[6]) / 
					(double)totalVotes * 100.0);
				dataResult.percentDontKnow = (int)((double)(votes[7] + votes[8]) / totalVotes * 100.0);
				dataResult.percentLike = 100 - dataResult.percentDislike - dataResult.percentDontKnow;
			}
			
			dataResult.total = totalVotes;
			dataResult.title = title;
		}

		public DataResult GetQualityResult() {
			if (qualityResultToday.total >= 5)
				return qualityResultToday;

			return qualityResultYesterday;
		}

		public DataResult GetRecommendationResult() {
			if (recommendationToday.total >= 5)
				return recommendationToday;

			return recommendationYesterday;
		}
	}
}
