namespace LoyaltyViewer {
	partial class FormInfo {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.pictureBoxRecommendation = new System.Windows.Forms.PictureBox();
			this.labelInfoRecommendation = new System.Windows.Forms.Label();
			this.labelLocationRecommendation = new System.Windows.Forms.Label();
			this.pictureBoxMarks = new System.Windows.Forms.PictureBox();
			this.labelInfoMarks = new System.Windows.Forms.Label();
			this.labelLocationMarks = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomLine)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomTemplate)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRecommendation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMarks)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBoxRecommendation
			// 
			this.pictureBoxRecommendation.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxRecommendation.Image = global::LoyaltyViewer.Properties.Resources.picture_recommendation;
			this.pictureBoxRecommendation.Location = new System.Drawing.Point(174, 236);
			this.pictureBoxRecommendation.Name = "pictureBoxRecommendation";
			this.pictureBoxRecommendation.Size = new System.Drawing.Size(202, 343);
			this.pictureBoxRecommendation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxRecommendation.TabIndex = 4;
			this.pictureBoxRecommendation.TabStop = false;
			// 
			// labelInfoRecommendation
			// 
			this.labelInfoRecommendation.AutoSize = true;
			this.labelInfoRecommendation.BackColor = System.Drawing.Color.Transparent;
			this.labelInfoRecommendation.Location = new System.Drawing.Point(165, 661);
			this.labelInfoRecommendation.Name = "labelInfoRecommendation";
			this.labelInfoRecommendation.Size = new System.Drawing.Size(203, 26);
			this.labelInfoRecommendation.TabIndex = 5;
			this.labelInfoRecommendation.Text = "\"Порекомендуете ли Вы нашу клинику\r\nВашим друзьям и знакомым?\"";
			this.labelInfoRecommendation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelLocationRecommendation
			// 
			this.labelLocationRecommendation.AutoSize = true;
			this.labelLocationRecommendation.BackColor = System.Drawing.Color.Transparent;
			this.labelLocationRecommendation.Location = new System.Drawing.Point(194, 791);
			this.labelLocationRecommendation.Name = "labelLocationRecommendation";
			this.labelLocationRecommendation.Size = new System.Drawing.Size(130, 26);
			this.labelLocationRecommendation.TabIndex = 6;
			this.labelLocationRecommendation.Text = "Терминал установлен\r\nна стойке регистратуры";
			this.labelLocationRecommendation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBoxMarks
			// 
			this.pictureBoxMarks.BackColor = System.Drawing.Color.Transparent;
			this.pictureBoxMarks.Image = global::LoyaltyViewer.Properties.Resources.picture_prolan3000;
			this.pictureBoxMarks.Location = new System.Drawing.Point(1028, 236);
			this.pictureBoxMarks.Name = "pictureBoxMarks";
			this.pictureBoxMarks.Size = new System.Drawing.Size(491, 464);
			this.pictureBoxMarks.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxMarks.TabIndex = 7;
			this.pictureBoxMarks.TabStop = false;
			// 
			// labelInfoMarks
			// 
			this.labelInfoMarks.AutoSize = true;
			this.labelInfoMarks.BackColor = System.Drawing.Color.Transparent;
			this.labelInfoMarks.Location = new System.Drawing.Point(1219, 765);
			this.labelInfoMarks.Name = "labelInfoMarks";
			this.labelInfoMarks.Size = new System.Drawing.Size(94, 26);
			this.labelInfoMarks.TabIndex = 8;
			this.labelInfoMarks.Text = "Оценка качества\r\nприёма у врача";
			this.labelInfoMarks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelLocationMarks
			// 
			this.labelLocationMarks.AutoSize = true;
			this.labelLocationMarks.BackColor = System.Drawing.Color.Transparent;
			this.labelLocationMarks.Location = new System.Drawing.Point(1182, 868);
			this.labelLocationMarks.Name = "labelLocationMarks";
			this.labelLocationMarks.Size = new System.Drawing.Size(160, 13);
			this.labelLocationMarks.TabIndex = 9;
			this.labelLocationMarks.Text = "Терминал установлен в холле";
			this.labelLocationMarks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FormInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1920, 1080);
			this.Controls.Add(this.labelLocationMarks);
			this.Controls.Add(this.labelInfoMarks);
			this.Controls.Add(this.pictureBoxMarks);
			this.Controls.Add(this.labelLocationRecommendation);
			this.Controls.Add(this.labelInfoRecommendation);
			this.Controls.Add(this.pictureBoxRecommendation);
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "FormInfo";
			this.Text = "FormInfo";
			this.Controls.SetChildIndex(this.pictureBoxLogo, 0);
			this.Controls.SetChildIndex(this.pictureBoxBottomLine, 0);
			this.Controls.SetChildIndex(this.pictureBoxBottomTemplate, 0);
			this.Controls.SetChildIndex(this.labelTitle, 0);
			this.Controls.SetChildIndex(this.pictureBoxRecommendation, 0);
			this.Controls.SetChildIndex(this.labelInfoRecommendation, 0);
			this.Controls.SetChildIndex(this.labelLocationRecommendation, 0);
			this.Controls.SetChildIndex(this.pictureBoxMarks, 0);
			this.Controls.SetChildIndex(this.labelInfoMarks, 0);
			this.Controls.SetChildIndex(this.labelLocationMarks, 0);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomLine)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxBottomTemplate)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRecommendation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxMarks)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBoxRecommendation;
		private System.Windows.Forms.Label labelInfoRecommendation;
		private System.Windows.Forms.Label labelLocationRecommendation;
		private System.Windows.Forms.PictureBox pictureBoxMarks;
		private System.Windows.Forms.Label labelInfoMarks;
		private System.Windows.Forms.Label labelLocationMarks;
	}
}