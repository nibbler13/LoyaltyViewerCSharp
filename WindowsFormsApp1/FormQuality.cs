using System;
using System.Collections.Generic;

namespace LoyaltyViewer {
	public partial class FormQuality : FormTemplate {
		public FormQuality(DataResult prolanResult) {
			InitializeComponent();

			LoggingSystem.LogMessageToFile("ФормаОценкаКачества.Отображение");
			LoggingSystem.LogMessageToFile("Оценок: " + prolanResult.total + ", отлично: " +
				prolanResult.percentLove + ", хорошо: " + prolanResult.percentHappy +
				", затрудняюсь ответить: " + prolanResult.percentNeutral + 
				", не очень: " + prolanResult.percentSad + 
				", плохо: " + prolanResult.percentAngry);
			LoggingSystem.LogMessageToFile("Заголовок: " + prolanResult.title);

			labelTitle.Text = Properties.Resources.FormDoctorsMarksTitle;

			SetupVotesLabels(
				prolanResult.title, labelSubTitle, labelVotes, pictureBoxVotes, prolanResult.total);

			List<IndicatorLabel> indicatorLabels = new List<IndicatorLabel>() {
				new IndicatorLabel(
					pictureBoxSmileLove, labelBackLove, labelFrontLove, labelPercentLove,
					prolanResult.percentLove, Properties.Settings.Default.formColorLove),
				new IndicatorLabel(
					pictureBoxSmileHappy, labelBackHappy, labelFrontHappy, labelPercentHappy,
					prolanResult.percentHappy, Properties.Settings.Default.formColorHappy),
				new IndicatorLabel(
					pictureBoxSmileNeutral, labelBackNeutral, labelFrontNeutral, labelPercentNeutral,
					prolanResult.percentNeutral, Properties.Settings.Default.formColorNeutral),
				new IndicatorLabel(
					pictureBoxSmileSad, labelBackSad, labelFrontSad, labelPercentSad,
					prolanResult.percentSad, Properties.Settings.Default.formColorSad),
				new IndicatorLabel(
					pictureBoxSmileAngry, labelBackAngry, labelFrontAngry, labelPercentAngry,
					prolanResult.percentAngry, Properties.Settings.Default.formColorAngry),
			};

			SetupMarkLabels(indicatorLabels);
			CreateClosingTimer();
		}
	}
}
