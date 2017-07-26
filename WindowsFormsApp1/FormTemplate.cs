using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace LoyaltyViewer {
	public partial class FormTemplate : Form {
		protected int gap = 0;
		private float fontSizeTitle = 0;
		private float fontSizeLabel = 0;
		private float fontSizeInfo = 0;
		protected int windowWidth = 0;
		protected int windowHeight = 0;
		protected int freeSpaceTop = 0;
		protected int freeSpaceBottom = 0;
		protected int availableHeight = 0;
		protected Color colorFont;
		protected int iconSize = 0;
		protected Font fontTitle;
		protected Font fontLabel;
		protected Font fontInfo;
		private System.Windows.Forms.Timer closingTimer;

		public FormTemplate() {
			InitializeComponent();

			windowWidth = Screen.FromControl(this).Bounds.Width;
			windowHeight = Screen.FromControl(this).Bounds.Height;

			if (!Properties.Settings.Default.formDebug) {
				Cursor.Hide();
				WindowState = FormWindowState.Maximized;
			} else {
				windowWidth = 800;
				windowHeight = 600;
				TopMost = false;
			}

			gap = (int)(windowWidth * 0.01);
			fontSizeTitle = windowHeight * 0.05f;
			fontSizeLabel = fontSizeTitle * 0.8f;
			fontSizeInfo = fontSizeLabel * 0.8f;

			iconSize = (int)(fontSizeTitle * 2.0);

			fontTitle = new Font(Properties.Settings.Default.formFontFamily.Name, fontSizeTitle);
			fontLabel = new Font(Properties.Settings.Default.formFontFamily.Name, fontSizeLabel);
			fontInfo = new Font(Properties.Settings.Default.formFontFamily.Name, fontSizeInfo);
			
			labelTitle.SizeChanged += LabelTitle_SizeChanged;

			//Do not change the order of functions call
			SetupLogo();
			SetupBottomLine();
			SetupTitle();

			if (Properties.Settings.Default.formDebug) {
				labelTitle.BackColor = Color.AliceBlue;
				pictureBoxLogo.BackColor = Color.AliceBlue;
			}

			KeyDown += FormTemplate_KeyDown;
		}

		private void FormTemplate_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode != Keys.Escape)
				return;

			LoggingSystem.LogMessageToFile("Завершение работы по нажатию клавиши ESC");
			Environment.Exit(0);
		}

		private void LabelTitle_SizeChanged(object sender, EventArgs e) {
			int titleBottom = labelTitle.Top + labelTitle.Size.Height;
			int logoBottom = pictureBoxLogo.Top + pictureBoxLogo.Size.Height;
			freeSpaceTop = titleBottom > logoBottom ? titleBottom : logoBottom;

			freeSpaceBottom = pictureBoxBottomLine.Top;
			availableHeight = freeSpaceBottom - freeSpaceTop;
		}

		private void SetupLogo() {
			int logoNewWidth = (int)(windowWidth * 0.07);
			float logoScale = (float)logoNewWidth / (float)Properties.Resources.butterfly_clear.Width;
			int logoNewHeight = (int)(Properties.Resources.butterfly_clear.Height * logoScale);
			pictureBoxLogo.SetBounds(windowWidth - logoNewWidth - gap, 0 + gap, logoNewWidth, logoNewHeight);
		}

		private void SetupBottomLine() {
			int lineNewHeight = (int)(windowHeight * 0.01);
			float lineScale = (float)lineNewHeight / (float)Properties.Resources.bottom_line_continues_clear.Height;
			int lineNewWidth = (int)(Properties.Resources.bottom_line_continues_clear.Width * lineScale);
			pictureBoxBottomLine.SetBounds(windowWidth - lineNewWidth, windowHeight - lineNewHeight, lineNewWidth, lineNewHeight);
			pictureBoxBottomTemplate.SetBounds(0, windowHeight - lineNewHeight, windowWidth - lineNewWidth, lineNewHeight);
		}

		private void SetupTitle() {
			labelTitle.Font = fontTitle;
			labelTitle.Left = gap;
			labelTitle.Top = gap;
			labelTitle.MaximumSize = new Size(windowWidth - pictureBoxLogo.Size.Width - gap * 3, 0);
			labelTitle.ForeColor = colorFont;
		}

		protected void SetupMarkLabels(List<IndicatorLabel> indicatorLabels) {
			int usedHeight = iconSize * indicatorLabels.Count + gap * 2 * (indicatorLabels.Count - 1);
			int startHeight = freeSpaceTop + availableHeight / 2 - usedHeight / 2;

			indicatorLabels[0].percent.Font = fontTitle;
			int maxValue = indicatorLabels.Max(x => x.value);
			indicatorLabels[0].percent.Text = maxValue + "%";
			int labelPercentWidth = indicatorLabels[0].percent.Size.Width;
			int labelPercentHeight = indicatorLabels[0].percent.Size.Height;

			int indicatorLabelWidth = windowWidth / 2 - gap * 4 - iconSize - labelPercentWidth;

			foreach (IndicatorLabel indicatorLabel in indicatorLabels) {
				indicatorLabel.icon.SetBounds(
					windowWidth / 2 + gap,
					startHeight,
					iconSize,
					iconSize);
				startHeight += iconSize + gap * 2;

				indicatorLabel.back.SetBounds(
					indicatorLabel.icon.Left + indicatorLabel.icon.Size.Width + gap,
					indicatorLabel.icon.Top + indicatorLabel.icon.Size.Height / 2 - labelPercentHeight / 2,
					indicatorLabelWidth,
					labelPercentHeight);
				indicatorLabel.back.BackColor = ColorTranslator.FromHtml("#F0F0F0");

				indicatorLabel.front.SetBounds(
					indicatorLabel.back.Left,
					indicatorLabel.back.Top,
					(int)(indicatorLabelWidth * ((float)indicatorLabel.value / 100.0f)),
					labelPercentHeight);
				indicatorLabel.front.BackColor = indicatorLabel.color;

				indicatorLabel.percent.Font = fontTitle;
				indicatorLabel.percent.ForeColor = colorFont;
				indicatorLabel.percent.Text = indicatorLabel.value + "%";
				indicatorLabel.percent.AutoSize = false;
				indicatorLabel.percent.SetBounds(
					windowWidth - gap - labelPercentWidth,
					indicatorLabel.icon.Top + indicatorLabel.icon.Size.Height / 2 - labelPercentHeight / 2,
					labelPercentWidth,
					labelPercentHeight);

				if (Properties.Settings.Default.formDebug) {
					indicatorLabel.percent.BackColor = Color.AliceBlue;
					indicatorLabel.icon.BackColor = Color.AliceBlue;
				}
			}

			HideControls();

			Thread thread = new Thread(ShowControls);
			thread.Start();
		}

		protected void SetupVotesLabels(string text, Label title, Label votes, PictureBox icon, int votesTotal) {
			title.Text = text;
			title.ForeColor = colorFont;
			title.Font = fontLabel;

			votes.Text = votesTotal.ToString();
			votes.ForeColor = colorFont;
			votes.Font = fontTitle;

			if (Properties.Settings.Default.formDebug) {
				title.BackColor = Color.AliceBlue;
				votes.BackColor = Color.AliceBlue;
				icon.BackColor = Color.AliceBlue;
			}

			title.MaximumSize = new Size(windowWidth / 2 - gap * 2, 0);

			icon.Size = new Size(iconSize, iconSize);

			int usedVotesBlockHeight = title.Size.Height + gap * 2;
			int maxVotesHeight = icon.Size.Height;
			if (icon.Size.Height < votes.Size.Height)
				maxVotesHeight = votes.Size.Height;

			usedVotesBlockHeight += maxVotesHeight;

			int centerFreeSpace = freeSpaceTop + (freeSpaceBottom - freeSpaceTop) / 2;

			title.Left = windowWidth / 4 - title.Size.Width / 2;
			title.Top = centerFreeSpace - usedVotesBlockHeight / 2;

			int usedWidth = icon.Size.Width + gap + votes.Size.Width;
			int subTitleLabelBottom = title.Top + title.Height + gap * 2;

			icon.Left = windowWidth / 4 - usedWidth / 2;
			icon.Top = subTitleLabelBottom + maxVotesHeight / 2 - icon.Size.Height / 2;

			votes.Left = icon.Left + icon.Size.Width + gap;
			votes.Top = subTitleLabelBottom + maxVotesHeight / 2 - votes.Size.Height / 2;
		}

		protected void CreateClosingTimer() {
			closingTimer = new System.Windows.Forms.Timer();
			closingTimer.Enabled = true;
			closingTimer.Interval = Properties.Settings.Default.formChangePeriodInSeconds * 1000;
			closingTimer.Tick += ClosingTimer_Tick;
		}

		private void ClosingTimer_Tick(object sender, EventArgs e) {
			LoggingSystem.LogMessageToFile("Форма.Закрытие, заголовок: " + labelTitle.Text);
			closingTimer.Stop();
			Close();
		}

		protected void HideControls() {
			foreach(Control control in this.Controls) {
				if (control.Handle == labelTitle.Handle ||
					control.Handle == pictureBoxLogo.Handle ||
					control.Handle == pictureBoxBottomLine.Handle ||
					control.Handle == pictureBoxBottomTemplate.Handle)
					continue;
				control.Hide();
			}
		}

		protected void ShowControls() {
			Thread.Sleep(500);

			for (int i = this.Controls.Count - 1; i >= 0 ; i--) {
				this.Controls[i].Invoke((MethodInvoker)delegate () {
					this.Controls[i].Show();
				});

				Thread.Sleep(200);
			}
		}
	}
}
