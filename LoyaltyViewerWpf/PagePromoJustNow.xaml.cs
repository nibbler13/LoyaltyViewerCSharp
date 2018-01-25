using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoyaltyViewerWpf {
    /// <summary>
    /// Логика взаимодействия для PagePromoJustNow.xaml
    /// </summary>
    public partial class PagePromoJustNow : Page {
		public string SubtitleText { get; set; }
		private ItemPromoJustNow _promoJustNow;

        public PagePromoJustNow(ItemPromoJustNow promoJustNow) {
			_promoJustNow = promoJustNow;

			DataContext = this;
			SubtitleText = Properties.Resources.PromoJustNowSubtitle;
            InitializeComponent();

			DrawContent();
        }

		private void DrawContent() {
			if (_promoJustNow.Departments.Count == 0) {
				//nothing to show

				return;
			}

			int gridRow = 1;

			foreach (KeyValuePair<string, ItemDepartment> department in _promoJustNow.Departments) {
				if (gridRow > 9)
					break;

				CreateTextBlock(department.Key, gridRow, new SolidColorBrush(Color.FromRgb(240, 240, 240)), TextAlignment.Center, 2);
				gridRow++;

				foreach (ItemDoctor doctor in department.Value.Doctors) {
					if (gridRow > 9)
						break;

					CreateTextBlock(doctor.Name, gridRow, Brushes.Transparent, TextAlignment.Center);
					gridRow++;
				}
			}


			Console.WriteLine(CanvasMain.ActualWidth + "x" + CanvasMain.ActualHeight);
		}

		private void CreateTextBlock(string value, int gridRow, SolidColorBrush background, TextAlignment textAlignment = TextAlignment.Left, int columnSpan = 1) {
			Border border = new Border();
			border.BorderThickness = new Thickness(0, 1, 2, 1);
			border.BorderBrush = new SolidColorBrush(Color.FromRgb(240, 240, 240));
			border.Background = background;

			TextBlock textBlock = new TextBlock();
			textBlock.Text = value;
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
			textBlock.TextAlignment = textAlignment;

			border.Child = textBlock;

			Grid.SetRow(border, gridRow);
			Grid.SetColumnSpan(border, columnSpan);
			GridData.Children.Add(border);
		}
    }
}
