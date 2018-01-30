using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoyaltyViewerWpf {
	public class ControlsFactory {
		private static double cellHeight;

		public static SolidColorBrush BrushFromRGB(System.Drawing.Color color) {
			return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}

		public static Border CreateBorder(SolidColorBrush background, object tag, UIElement child = null) {
			Border border = new Border() {
				Background = background,
				Tag = tag,
				Child = child
			};

			return border;
		}

		public static TextBlock CreateTextBlock(
			string text,
			HorizontalAlignment horizontalAlignment,
			VerticalAlignment verticalAlignment,
			TextAlignment textAlignment,
			Thickness margin,
			FontWeight fontWeight,
			Brush foreground) {
			TextBlock textBlock = new TextBlock() {
				Text = text,
				HorizontalAlignment = horizontalAlignment,
				VerticalAlignment = verticalAlignment,
				TextAlignment = textAlignment,
				Margin = margin,
				FontWeight = fontWeight,
				Foreground = foreground
			};

			return textBlock;
		}

		public static Line CreateLine(double x1, double y1, double x2, double y2, int thickness) {
			Line line = new Line {
				X1 = x1,
				Y1 = y1,
				X2 = x2,
				Y2 = y2,
				StrokeThickness = thickness
			};

			return line;
		}

		public static Size MeasureString(string candidate, Typeface typeface, double fontSize) {
			var formattedText = new FormattedText(
				candidate,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				typeface,
				fontSize,
				Brushes.Black);

			return new Size(formattedText.Width, formattedText.Height);
		}

		public static Image CreateImage(string resourceName, double width, double height) {
			Image image = new Image {
				Source = GetResourceImage(resourceName),
				Width = width,
				Height = height
			};

			return image;
		}

		public static BitmapImage GetResourceImage(string name) {
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.UriSource = new Uri("pack://application:,,,/LoyaltyViewerWpf;component/Resources/" + name + ".png");
			image.EndInit();

			return image;
		}
	}
}
