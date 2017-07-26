using System;
using System.Collections.Generic;

namespace LoyaltyViewer {
	public partial class FormRecommendation : FormTemplate {
		public FormRecommendation(ProlanResult prolanResult) {
			InitializeComponent();

			LoggingSystem.LogMessageToFile("ФормаРекомендации.Отображение");
			LoggingSystem.LogMessageToFile("Пациентов: " + prolanResult.total + ", да: " +
				prolanResult.percentLike + ", не знаю: " + prolanResult.percentDontKnow +
				", нет: " + prolanResult.percentDislike);

			labelTitle.Text = Properties.Resources.FormRecommendationTitle;

			SetupVotesLabels(
				prolanResult.title, labelSubTitle, labelVotes, pictureBoxVotes, prolanResult.total);

			List<IndicatorLabel> indicatorLabels = new List<IndicatorLabel>() {
				new IndicatorLabel(
					pictureBoxLike, labelBackLike, labelFrontLike, labelPercentLike,
					prolanResult.percentLike, Properties.Settings.Default.formColorLike),
				new IndicatorLabel(
					pictureBoxDontKnow, labelBackDontKnow, labelFrontDontKnow, labelPercentDontKnow,
					prolanResult.percentDontKnow, Properties.Settings.Default.formColorDontKnow),
				new IndicatorLabel(
					pictureBoxDislike, labelBackDislike, labelFrontDislike, labelPercentDislike,
					prolanResult.percentDislike, Properties.Settings.Default.formColorDislike)
			};

			SetupMarkLabels(indicatorLabels);
			CreateClosingTimer();
		}
	}
}
