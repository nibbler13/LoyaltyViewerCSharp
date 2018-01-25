using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace LoyaltyViewerWpf {
	public partial class WindowMain : Window, INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		
		private bool _allowDirectNavigation = false;
		private NavigatingCancelEventArgs _navArgs = null;
		private Duration _duration = new Duration(TimeSpan.FromSeconds(1));
		private double _oldHeight = 0;

		public enum CurrentStates { About, ClinicRecommendations, DoctorsMarks };

		public double FontSizeMain { get; set; }
		public double FontSizeHeader { get; set; }
		public FontFamily FontFamilyMain { get; set; }
		public Brush ForegroundColorMain {get; set; }
		private CurrentStates currentState = CurrentStates.About;

		private SystemDataService dataService = new SystemDataService();

		private string _titleText;
		public string TitleText {
			get {
				return _titleText;
			} set {
				if (value != _titleText) {
					_titleText = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string AboutDeveloper { get; set; }

		public WindowMain() {
			InitializeComponent();

			SystemLogging.LogMessageToFile("==================================" +
				Environment.NewLine + "Создание основного окна");

			if (System.Windows.SystemParameters.PrimaryScreenWidth / 
				System.Windows.SystemParameters.PrimaryScreenHeight >= 1.5)
				FontSizeMain = Height * 0.05;
			else
				FontSizeMain = Height * 0.03;

			FontSizeHeader = FontSizeMain * 1.5;
			FontFamilyMain = new FontFamily(Properties.Resources.FontFamilyMain);

			ForegroundColorMain = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.ColorFontMain.A,
				Properties.Settings.Default.ColorFontMain.R,
				Properties.Settings.Default.ColorFontMain.G,
				Properties.Settings.Default.ColorFontMain.B));
			
			if (!Properties.Settings.Default.IsDebug &&
				!Environment.MachineName.Equals("MSSU-DEV")) {
				Topmost = true;
				Cursor = Cursors.None;
			}

			//New year theme
			int currentDay = DateTime.Now.Day;
			int currentMonth = DateTime.Now.Month;
			if ((currentMonth == 12 && currentDay >= 10) ||
				(currentMonth == 1 && currentDay <= 9)) {
				imageLogo.Source = PageMarks.GetResourceImage("PicChristmasTree");
				canvasSnowfall.Visibility = Visibility.Visible;
				List<string> snows = new List<string>();
				for (int i = 1; i <= 9; i++)
					snows.Add("pack://application:,,,/Graphics/snow@.png".Replace("@", i.ToString()));
				SnowEngine snow = new SnowEngine(canvasSnowfall, snows.ToArray());
				snow.Start();
			}

			AboutDeveloper = Properties.Resources.AboutDeveloper;
			TextBlockAboutDeveloper.FontSize = FontSizeMain / 2;

			TextBlockAboutDeveloper.Foreground = BrushFromRGB(Properties.Settings.Default.ColorFontAboutDeveloper);




			if (Properties.Settings.Default.ShowLoyaltyInfo) {
				TitleText = Properties.Resources.WindowMainTitleAbout;

				//Uri uri = new Uri("pack://application:,,,/PageAbout.xaml");
				//frame.Source = uri;
				//frame.Navigate(uri);

				frame.Navigated += Frame_Navigated;
				frame.Navigating += Frame_Navigating;

				DispatcherTimer dispatcherTimer = new DispatcherTimer();
				dispatcherTimer.Interval = TimeSpan.FromSeconds(Properties.Settings.Default.PageChangingPeriodInSeconds);
				dispatcherTimer.Tick += DispatcherTimer_Tick;
				dispatcherTimer.Start();
			} else if (Properties.Settings.Default.ShowPromoJustNow) {
				BackgroundWorker backgroundWorkerPromoJustNow = new BackgroundWorker();
				backgroundWorkerPromoJustNow.DoWork += BackgroundWorkerPromoJustNow_DoWork;
				backgroundWorkerPromoJustNow.RunWorkerCompleted += BackgroundWorkerPromoJustNow_RunWorkerCompleted;
				backgroundWorkerPromoJustNow.RunWorkerAsync();
			} else {

			}



			KeyDown += WindowMain_KeyDown;

			DataContext = this;
		}

		private void BackgroundWorkerPromoJustNow_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			TitleText = Properties.Resources.WindowMainTItlePromoJustNow;
			imageLogo.Visibility = Visibility.Hidden;
			LabelTitle.Margin = new Thickness(0);
			LabelTitle.Background = BrushFromRGB(Properties.Settings.Default.ColorTitlePromoBackground);
			LabelTitle.Foreground = BrushFromRGB(Properties.Settings.Default.ColorTitlePromoForeground);
			LabelTitle.VerticalContentAlignment = VerticalAlignment.Center;
			LabelTitle.HorizontalContentAlignment = HorizontalAlignment.Center;

			PagePromoJustNow pagePromoJustNow = new PagePromoJustNow(dataService.GetPromoJustNow());
			frame.Navigate(pagePromoJustNow);
		}

		private void BackgroundWorkerPromoJustNow_DoWork(object sender, DoWorkEventArgs e) {
			throw new NotImplementedException();
		}

		public static SolidColorBrush BrushFromRGB(System.Drawing.Color color) {
			return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}

		private void Frame_Navigating(object sender, NavigatingCancelEventArgs e) {
			if (Content != null && !_allowDirectNavigation) {
				e.Cancel = true;

				_navArgs = e;
				_oldHeight = frame.ActualHeight;

				DoubleAnimation animation0 = new DoubleAnimation();
				animation0.From = frame.ActualHeight;
				animation0.To = 0;
				animation0.Duration = _duration;
				animation0.Completed += SlideCompleted;
				frame.BeginAnimation(HeightProperty, animation0);
			}
			_allowDirectNavigation = false;
		}

		private void SlideCompleted(object sender, EventArgs e) {
			_allowDirectNavigation = true;
			switch (_navArgs.NavigationMode) {
				case NavigationMode.New:
					if (_navArgs.Uri == null)
						frame.Navigate(_navArgs.Content);
					else
						frame.Navigate(_navArgs.Uri);
					break;
				case NavigationMode.Back:
					frame.GoBack();
					break;
				case NavigationMode.Forward:
					frame.GoForward();
					break;
				case NavigationMode.Refresh:
					frame.Refresh();
					break;
			}

			Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
				(ThreadStart)delegate () {
					DoubleAnimation animation0 = new DoubleAnimation();
					animation0.From = 0;
					animation0.To = _oldHeight;
					animation0.Duration = _duration;
					frame.BeginAnimation(HeightProperty, animation0);
				});
		}

		private void Frame_Navigated(object sender, NavigationEventArgs e) {
			frame.NavigationService.RemoveBackEntry();
		}

		private void DispatcherTimer_Tick(object sender, EventArgs e) {
			Page navigateTo;

			if (currentState == CurrentStates.About) {
				currentState = CurrentStates.ClinicRecommendations;
				ItemDataResult dataResult = dataService.GetRecommendationResult();

				if (dataResult.Total == 0) {
					SystemLogging.LogMessageToFile("Skipping recommendations, total = 0");
					return;
				}

				navigateTo = new PageMarks(currentState, dataResult);
				TitleText = Properties.Resources.WindowMainTitleClinicRecommendations;
			} else if (currentState == CurrentStates.ClinicRecommendations) {
				currentState = CurrentStates.DoctorsMarks;
				ItemDataResult dataResult = dataService.GetQualityResult();

				if (dataResult.Total == 0) {
					SystemLogging.LogMessageToFile("Skipping quality, total = 0");
					return;
				}

				navigateTo = new PageMarks(currentState, dataResult);
				TitleText = Properties.Resources.WindowMainTitleDoctorsMarks;
			} else {
				navigateTo = new PageAbout();
				currentState = CurrentStates.About;
				TitleText = Properties.Resources.WindowMainTitleAbout;
			}

			navigateTo.FontFamily = FontFamily;
			navigateTo.FontSize = FontSize;
			navigateTo.Foreground = Foreground;

			frame.Navigate(navigateTo);
		}

		private void WindowMain_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key.Equals(Key.Escape)) {
				SystemLogging.LogMessageToFile("---------------------------------" +
					Environment.NewLine + "Закрытие по нажатию клавиши ESC");
				Application.Current.Shutdown();
			}
		}
	}
}
