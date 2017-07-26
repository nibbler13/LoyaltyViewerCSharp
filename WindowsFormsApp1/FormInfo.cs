using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LoyaltyViewer {
	public partial class FormInfo : FormTemplate {
		Prolan prolan;

		public FormInfo() {
			InitializeComponent();

			//Do not change the order of functions call
			SetupLabels();
			SetupPictureBoxes();

			LoggingSystem.LogMessageToFile("Используются следующие параметры:");
			LoggingSystem.LogMessageToFile(Properties.Settings.Default.prolanDeptNameQuality.ToString());
			LoggingSystem.LogMessageToFile(Properties.Settings.Default.prolanDeptNameRecommendation.ToString());

			Thread thread = new Thread(BeginCycleChange);
			thread.Start();
		}

		private void SetupLabels() {
			labelTitle.Text = Properties.Resources.FormInfoTitle;

			labelInfoRecommendation.Text = Properties.Resources.FormInfoRecommendationLabel;
			labelLocationRecommendation.Text = Properties.Resources.FormInfoRecommendationLocation;
			labelInfoMarks.Text = Properties.Resources.FormInfoQualityLabel;
			labelLocationMarks.Text = Properties.Resources.FormInfoQualityLocation;

			List<Label> labels = new List<Label>() {
				labelLocationRecommendation,
				labelInfoRecommendation,
				labelLocationMarks,
				labelInfoMarks
			 };

			int labelWidth = (int)(windowWidth / 2 - gap * 2);

			foreach (Label label in labels) {
				label.Font = fontInfo;
				label.ForeColor = colorFont;
				label.MaximumSize = new Size(labelWidth, 0);

				if (Properties.Settings.Default.formDebug)
					label.BackColor = Color.AliceBlue;
			}

			labelLocationRecommendation.ForeColor = Color.DarkGray;
			labelLocationMarks.ForeColor = Color.DarkGray;

			int highestLocationLabelHeight = labelLocationRecommendation.Size.Height;
			if (highestLocationLabelHeight < labelLocationMarks.Size.Height)
				highestLocationLabelHeight = labelLocationMarks.Size.Height;

			int highestInfoLabelHeight = labelInfoRecommendation.Size.Height;
			if (highestInfoLabelHeight < labelInfoMarks.Size.Height)
				highestInfoLabelHeight = labelInfoMarks.Size.Height;

			int bottomLineTop = freeSpaceBottom - gap * 2;

			labelLocationRecommendation.Location =
				new Point(windowWidth / 4 - labelLocationRecommendation.Size.Width / 2,
						  bottomLineTop - highestLocationLabelHeight);
			labelInfoRecommendation.Location =
				new Point(windowWidth / 4 - labelInfoRecommendation.Size.Width / 2,
						  labelLocationRecommendation.Top - gap * 2 - highestInfoLabelHeight);
			labelLocationMarks.Location =
				new Point(windowWidth / 4 * 3 - labelLocationMarks.Size.Width / 2,
						  bottomLineTop - highestLocationLabelHeight);
			labelInfoMarks.Location =
				new Point(windowWidth / 4 * 3 - labelInfoMarks.Size.Width / 2,
						  labelLocationMarks.Top - gap * 2 - highestInfoLabelHeight);
		}

		private void SetupPictureBoxes() {
			freeSpaceTop += gap * 2;

			int picturesHeight = labelInfoMarks.Location.Y - freeSpaceTop - gap * 2;

			pictureBoxRecommendation.SetBounds(gap, freeSpaceTop, windowWidth / 2 - gap * 2, picturesHeight);
			pictureBoxMarks.SetBounds(windowWidth / 2 + gap, freeSpaceTop, windowWidth / 2 - gap * 2, picturesHeight);

			if (Properties.Settings.Default.formDebug) {
				pictureBoxRecommendation.BackColor = Color.AliceBlue;
				pictureBoxMarks.BackColor = Color.AliceBlue;
			}
		}

		private void BeginCycleChange() {
			while (true) {
				Thread.Sleep(Properties.Settings.Default.formChangePeriodInSeconds * 1000);

				if (prolan == null)
					prolan = new Prolan();

				this.Invoke((MethodInvoker)delegate () {
					ProlanResult prolanResult = prolan.GetRecommendationResult();
					if (prolanResult.total > 0) {
						Form formRecommendation = new FormRecommendation(prolanResult);
						formRecommendation.Show();
					} else {
						LoggingSystem.LogMessageToFile("ФормаИнфо.Пропуск блока рекоммендаций. Нет данных.");
					}
				});

				Thread.Sleep((Properties.Settings.Default.formChangePeriodInSeconds - 1) * 1000);

				this.Invoke((MethodInvoker)delegate () {
					ProlanResult prolanResult = prolan.GetQualityResult();
					if (prolanResult.total > 0) {
						Form formDoctorsMarks = new FormQuality(prolanResult);
						formDoctorsMarks.Show();
					} else {
						LoggingSystem.LogMessageToFile("ФормаИнфо.Пропуск блока оценок качества. Нет данных.");
					}
				});

				Thread.Sleep(Properties.Settings.Default.formChangePeriodInSeconds * 1000);
			}
		}
	}
}
