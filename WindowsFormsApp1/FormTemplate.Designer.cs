namespace LoyaltyViewer {
	partial class FormTemplate {
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTemplate));
			this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
			this.pictureBoxBottomLine = new System.Windows.Forms.PictureBox();
			this.pictureBoxBottomTemplate = new System.Windows.Forms.PictureBox();
			this.labelTitle = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomLine)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomTemplate)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxLogo
			// 
			this.pictureBoxLogo.Image = global::LoyaltyViewer.Properties.Resources.butterfly_clear;
			this.pictureBoxLogo.Location = new System.Drawing.Point(661, -1);
			this.pictureBoxLogo.Name = "pictureBoxLogo";
			this.pictureBoxLogo.Size = new System.Drawing.Size(136, 110);
			this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxLogo.TabIndex = 0;
			this.pictureBoxLogo.TabStop = false;
			// 
			// pictureBoxBottomLine
			// 
			this.pictureBoxBottomLine.Image = global::LoyaltyViewer.Properties.Resources.bottom_line_continues_clear;
			this.pictureBoxBottomLine.Location = new System.Drawing.Point(432, 582);
			this.pictureBoxBottomLine.Name = "pictureBoxBottomLine";
			this.pictureBoxBottomLine.Size = new System.Drawing.Size(375, 19);
			this.pictureBoxBottomLine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxBottomLine.TabIndex = 1;
			this.pictureBoxBottomLine.TabStop = false;
			// 
			// pictureBoxBottomTemplate
			// 
			this.pictureBoxBottomTemplate.Image = global::LoyaltyViewer.Properties.Resources.bottom_line_template;
			this.pictureBoxBottomTemplate.Location = new System.Drawing.Point(-1, 582);
			this.pictureBoxBottomTemplate.Name = "pictureBoxBottomTemplate";
			this.pictureBoxBottomTemplate.Size = new System.Drawing.Size(438, 19);
			this.pictureBoxBottomTemplate.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBoxBottomTemplate.TabIndex = 2;
			this.pictureBoxBottomTemplate.TabStop = false;
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Location = new System.Drawing.Point(-1, -1);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(173, 26);
			this.labelTitle.TabIndex = 3;
			this.labelTitle.Text = "Lorem ipsum dolor sit amet.\r\nNunc dictum tristique leo in lobortis.";
			// 
			// FormTemplate
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(800, 600);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.pictureBoxBottomTemplate);
			this.Controls.Add(this.pictureBoxBottomLine);
			this.Controls.Add(this.pictureBoxLogo);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTemplate";
			this.Text = "Form1";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomLine)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomTemplate)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.PictureBox pictureBoxLogo;
		protected System.Windows.Forms.PictureBox pictureBoxBottomLine;
		protected System.Windows.Forms.PictureBox pictureBoxBottomTemplate;
		protected System.Windows.Forms.Label labelTitle;
	}
}

