using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoyaltyViewerWpf {
	/// <summary>
	/// Логика взаимодействия для PageDoctorsMarks.xaml
	/// </summary>
	public partial class PageMarks : Page {
		private double _percentMark5;
		private double _percentMark4;
		private double _percentMark3;
		private double _percentMark2;
		private double _percentMark1;

		public string PercentMark5 {
			get {
				return string.Format("{0:P0}", _percentMark5);
			}
		}

		public string PercentMark4 {
			get {
				return string.Format("{0:P0}", _percentMark4);
			}
		}

		public string PercentMark3 {
			get {
				return string.Format("{0:P0}", _percentMark3);
			}
		}

		public string PercentMark2 {
			get {
				return string.Format("{0:P0}", _percentMark2);
			}
		}

		public string PercentMark1 {
			get {
				return string.Format("{0:P0}", _percentMark1);
			}
		}

		public int TotalVotes { get; set; }
		public string Description { get; set; }

		public PageMarks(WindowMain.AvailablePages currentState, ItemDataResult dataResult) {
			InitializeComponent();

			if (currentState == WindowMain.AvailablePages.DoctorsMarks) {
				_percentMark5 = dataResult.PercentLove / 100.0d;
				_percentMark4 = dataResult.PercentHappy / 100.0d;
				_percentMark3 = dataResult.PercentNeutral / 100.0d;
				_percentMark2 = dataResult.PercentSad / 100.0d;
				_percentMark1 = dataResult.PercentAngry / 100.0d;
			} else if (currentState == WindowMain.AvailablePages.ClinicRecommendations) {
				_percentMark4 = dataResult.PercentLike / 100.0d;
				_percentMark3 = dataResult.PercentDontKnow / 100.0d;
				_percentMark2 = dataResult.PercentDislike / 100.0d;
				imageMark4.Source = ControlsFactory.GetResourceImage("icon_like");
				imageMark3.Source = ControlsFactory.GetResourceImage("icon_dont_know");
				imageMark2.Source = ControlsFactory.GetResourceImage("icon_dislike");
				BrushConverter converter = new BrushConverter();
				rectangleMark4.Fill = (Brush)converter.ConvertFromString("#FF4e9b44");
				rectangleMark3.Fill = (Brush)converter.ConvertFromString("#FF2d3d3f");
				rectangleMark2.Fill = (Brush)converter.ConvertFromString("#FF00a9dc");
				imageDescription.Source = ControlsFactory.GetResourceImage("man");

				foreach (UIElement element in gridMarks.Children) {
					int row = Grid.GetRow(element);
					if ((row >= 1 && row <= 3) ||
						(row >= 17 && row <= 19))
						element.Visibility = Visibility.Hidden;
				}

			} else {
				SystemLogging.LogMessageToFile("PageMarks: wrongType - " + currentState);
				return;
			}

			TotalVotes = dataResult.Total;
			Description = dataResult.Description;


			DataContext = this;
			Loaded += PageDoctorsMarks_Loaded;
		}

		private void PageDoctorsMarks_Loaded(object sender, RoutedEventArgs e) {
			DoubleAnimation gridAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
			gridAnimation.Completed += GridAnimation_Completed;
			gridMain.BeginAnimation(OpacityProperty, gridAnimation);
		}

		private void GridAnimation_Completed(object sender, EventArgs e) {
			double widthTemplate = rectangleTemplate.ActualWidth;
			Dictionary<Rectangle, double> rectangles = new Dictionary<Rectangle, double>() {
				{ rectangleMark5, widthTemplate * _percentMark5 },
				{ rectangleMark4, widthTemplate * _percentMark4 },
				{ rectangleMark3, widthTemplate * _percentMark3},
				{ rectangleMark2, widthTemplate * _percentMark2 },
				{ rectangleMark1, widthTemplate * _percentMark1}
			};

			foreach (KeyValuePair<Rectangle, double> rectangle in rectangles) {
				rectangle.Key.Width = 1;
				ScaleTransform trans = new ScaleTransform();
				rectangle.Key.RenderTransform = trans;
				DoubleAnimation anim = new DoubleAnimation(0, rectangle.Value, TimeSpan.FromSeconds(3));
				trans.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
			}
		}
	}
}