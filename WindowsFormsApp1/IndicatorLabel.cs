using System;
using System.Drawing;
using System.Windows.Forms;

namespace LoyaltyViewer {
	public class IndicatorLabel {
		public PictureBox icon;
		public Label back;
		public Label front;
		public Label percent;
		public int value;
		public Color color;

		public IndicatorLabel(
			PictureBox icon,
			Label back,
			Label front,
			Label percent,
			int value,
			Color color) {
			this.icon = icon;
			this.back = back;
			this.front = front;
			this.percent = percent;
			this.value = value;
			this.color = color;
		}
	}
}
