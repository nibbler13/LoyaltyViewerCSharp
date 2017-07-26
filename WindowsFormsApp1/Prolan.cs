using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.IO;
using System.Linq;
using System.Timers;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoyaltyViewer {
	class Prolan {
		private int lastUpdateDay = 0;
		private int errorsCountDownload = 0;
		private int errorsCountParse = 0;
		private ProlanResult qualityResultToday = new ProlanResult();
		private ProlanResult qualityResultYesterday = new ProlanResult();
		private ProlanResult recommendationLastWeek = new ProlanResult();
		private static readonly HttpClient httpClient = new HttpClient();

		public Prolan() {
			LoggingSystem.LogMessageToFile("Пролан.Инициализация");

			Timer timer = new Timer();
			timer.Interval = Properties.Settings.Default.prolanUpdatePeriodInSeconds * 1000;
			timer.Elapsed += Timer_Elapsed;
			timer.Start();
			UpdateData();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
			LoggingSystem.LogMessageToFile("Пролан.Таймер обновления");
			UpdateData();
		}

		private void UpdateData() {
			LoggingSystem.LogMessageToFile("Пролан.Запрос обновления данных");

			qualityResultToday = GetProlanResult(ProlanResult.Types.Quality, DateTime.Now, DateTime.Now);
			qualityResultToday.title = Properties.Resources.FormDoctorsMarksLabelToday;

			if (lastUpdateDay == DateTime.Now.Day)
				return;

			errorsCountDownload = 0;
			errorsCountParse = 0;

			qualityResultYesterday = GetProlanResult(ProlanResult.Types.Quality, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-1));
			qualityResultYesterday.title = Properties.Resources.FormDoctorsMarksLabelYesterday;

			recommendationLastWeek = GetProlanResult(ProlanResult.Types.Recommendation, DateTime.Now.AddDays(-7), DateTime.Now);
			recommendationLastWeek.title = Properties.Resources.FormRecommendationLabel;

			lastUpdateDay = DateTime.Now.Day;
		}

		public ProlanResult GetQualityResult() {
			if (qualityResultToday.total >= 5)
				return qualityResultToday;

			return qualityResultYesterday;
		}

		public ProlanResult GetRecommendationResult() {
			return recommendationLastWeek;
		}
		
		private async Task<string> GetPostResponseFromUrl(String url, Dictionary<string, string> values) {
			FormUrlEncodedContent content = new FormUrlEncodedContent(values);
			content.Headers.Add("RequestType", "GetXmlLoyaltyReport");
			ServicePointManager.Expect100Continue = false;
			HttpResponseMessage response = await httpClient.PostAsync(url, content);

			if (response.IsSuccessStatusCode) {
				IEnumerable<string> headerValues;
				if (response.Headers.TryGetValues("CompletionCode", out headerValues)) {
					string completionCode = headerValues.First();
					if (!completionCode.Equals("0") &&
						!completionCode.Equals("5")) {
						if (response.Headers.TryGetValues("ErrorDescription", out headerValues)) {
							errorsCountDownload++;
							string errorDescription = headerValues.First();
							LoggingSystem.LogMessageToFile("Пролан.Ошибка загрузки: " + completionCode + " " + errorDescription);
							return "";
						}
					}
				}

			} else {
				errorsCountDownload++;
				LoggingSystem.LogMessageToFile("Пролан.Ошибка загрузки: " + response.StatusCode + " " + response.ReasonPhrase);
				return "";
			}
			
			return await response.Content.ReadAsStringAsync(); ;
		}

		private List<String> GetDataFromProlanAsync(string questionId, DateTime beginDate, DateTime endDate)
		{
			LoggingSystem.LogMessageToFile("Пролан.Получение данных с сайта, questionId: " + questionId);

			Dictionary<string, string> values = new Dictionary<string, string>() {
				{ "LoginName", Properties.Settings.Default.prolanUserName },
				{ "Password", Properties.Settings.Default.prolanUserPassword },
				{ "QuestionID", questionId },
				{ "ReportID", Properties.Settings.Default.prolanReportId },
				{ "Begin", beginDate.ToString("dd.MM.yyyy") + " 00:00:00" },
				{ "End", endDate.ToString("dd.MM.yyyy") + " 23:59:59" },
				{ "UseUTC", Properties.Settings.Default.prolanUseUTC }
			};

			List<string> resultArray = new List<string>();
			string responseContent = "";
			try {
				responseContent = GetPostResponseFromUrl(Properties.Settings.Default.prolanUrl, values).Result;
			} catch (Exception e) {
				errorsCountDownload++;
				LoggingSystem.LogMessageToFile("Пролан.Ошибка загрузки данных: " + e.Message + " " + e.StackTrace);
				return resultArray;
			}

			if (string.IsNullOrEmpty(responseContent)) {
				LoggingSystem.LogMessageToFile("Пролан.Результат запроса пустая строка");
				return resultArray;
			}

			XmlReader xmlReader = null;
			try {
				xmlReader = XmlReader.Create(new StringReader(responseContent));
				while (xmlReader.Read()) {
					if (xmlReader.NodeType != XmlNodeType.Element || !xmlReader.Name.Equals("Rows"))
						continue;

					XmlReader xmlReaderInner = xmlReader.ReadSubtree();
					while (xmlReaderInner.Read()) {
							if (xmlReaderInner.NodeType != XmlNodeType.Text)
								continue;

							resultArray.Add(xmlReaderInner.Value);
					}
				}
			} catch (Exception e) {
				errorsCountDownload++;
				LoggingSystem.LogMessageToFile("Пролан.Ошибка разбора XML ответа: " + e.Message + " " + e.StackTrace);
				return resultArray;
			} finally {
				if (xmlReader != null)
					xmlReader.Close();
			}

			errorsCountDownload = 0;
			return resultArray;
		}

		private int[] GetMarksByCurrentDeptName(List<string> resultArray, int blockSize, int marksQuantity, string deptName) {
			LoggingSystem.LogMessageToFile("Пролан.Разбор XML ответа");
			int firstMarkPosition = 1;
			int[] marks = new int[marksQuantity];
			
			try {
				for (int blockPosition = 0; blockPosition < resultArray.Count; blockPosition += blockSize) {
					if (!resultArray[blockPosition].Contains(deptName))
						continue;

					for (int markPosition = firstMarkPosition; markPosition < firstMarkPosition + marksQuantity; markPosition++)
						marks[markPosition - firstMarkPosition] += int.Parse(resultArray[blockPosition + markPosition]);
				}

				errorsCountParse = 0;
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("Error while parsing values: " + e.StackTrace + " " + e.Message);

				errorsCountParse++;
				if (errorsCountParse == 3)
					MailSystem.SendMail("Не удалось выполнить разбор ответа XML с сайта пролан 3 или более раз");
			}

			return marks;
		}

		private ProlanResult GetProlanResult(ProlanResult.Types type, DateTime beginDate, DateTime endDate) {
			LoggingSystem.LogMessageToFile("Пролан.Расчет результатов данных типа: " + type.ToString());

			ProlanResult prolanResult = new ProlanResult();

			string questionId = "";
			string deptName = "";
			int blockSize = 0;
			int marksQuantity = 0;

			if (type == ProlanResult.Types.Quality) {
				questionId = Properties.Settings.Default.prolanQualityQuestionId;
				deptName = Properties.Settings.Default.prolanDeptNameQuality;
				blockSize = 11;
				marksQuantity = 5;
			} else if (type == ProlanResult.Types.Recommendation) {
				questionId = Properties.Settings.Default.prolanRecommendationQuestionId;
				deptName = Properties.Settings.Default.prolanDeptNameRecommendation;
				blockSize = 7;
				marksQuantity = 3;
			} else {
				LoggingSystem.LogMessageToFile("GetQualityMarksAndPercentage, wrong type");
				return prolanResult;
			}

			List<string> resultArray = GetDataFromProlanAsync(questionId, beginDate, endDate);

			if (resultArray.Count == 0) {
				if (errorsCountDownload == 30)
					MailSystem.SendMail("LoyaltyViewer не удалось загрузить данные с сайта пролан");
				return prolanResult;
			}
			
			int[] marks = GetMarksByCurrentDeptName(resultArray, blockSize, marksQuantity, deptName);

			try {
				prolanResult.total = marks.Sum();

				if (type == ProlanResult.Types.Quality) {
					prolanResult.percentHappy = (int)((double)marks[1] / (double)prolanResult.total * 100.0d);
					prolanResult.percentNeutral = (int)((double)marks[2] / (double)prolanResult.total * 100.0d);
					prolanResult.percentSad = (int)((double)marks[3] / (double)prolanResult.total * 100.0d);
					prolanResult.percentAngry = (int)((double)marks[4] / (double)prolanResult.total * 100.0d);

					prolanResult.percentLove = 100 - prolanResult.percentHappy - prolanResult.percentNeutral - 
						prolanResult.percentSad - prolanResult.percentAngry;
				} else if (type == ProlanResult.Types.Recommendation) {
					prolanResult.percentDontKnow = (int)((double)marks[1] / (double)prolanResult.total * 100.0d);
					prolanResult.percentDislike = (int)((double)marks[2] / (double)prolanResult.total * 100.0d);

					prolanResult.percentLike = 100 - prolanResult.percentDontKnow - prolanResult.percentDislike;
				}
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile(e.Message + " " + e.StackTrace);
			}

			return prolanResult;
		}
	}
}
