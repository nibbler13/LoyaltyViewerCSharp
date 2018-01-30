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

		public enum CurrentStates { About, ClinicRecommendations, DoctorsMarks, PromoJustNow };

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

			if (!Properties.Settings.Default.IsDebug && !Environment.MachineName.Equals("MSSU-DEV")) {
				Topmost = true;
				Cursor = Cursors.None;
			}

			FontSizeMain = SystemParameters.PrimaryScreenWidth / 40;
			FontSizeHeader = FontSizeMain * 1.5;
			FontFamilyMain = new FontFamily(Properties.Resources.FontFamilyMain);
			ForegroundColorMain = BrushFromRGB(Properties.Settings.Default.ColorFontMain);

			TitleText = Properties.Resources.WindowMainTitleAbout;
			SetupLabelTitle(false);

			AboutDeveloper = Properties.Resources.AboutDeveloper;
			TextBlockAboutDeveloper.FontSize = FontSizeMain / 2;
			TextBlockAboutDeveloper.Foreground = BrushFromRGB(Properties.Settings.Default.ColorFontAboutDeveloper);
			//if (Properties.Settings.Default.ShowPromoJustNow &&
			//	!Properties.Settings.Default.ShowLoyaltyInfo) {
			//	SetupLabelTitle(true);
			//	frame.Navigate(new PagePromoJustNow(dataService.GetPromoJustNow(), new Typeface(FontFamilyMain, FontStyle, FontWeight, FontStretch), FontSizeMain / 2));
			//} else {
				DispatcherTimer dispatcherTimer = new DispatcherTimer {
					Interval = TimeSpan.FromSeconds(Properties.Settings.Default.PageChangingPeriodInSeconds)
				};

				dispatcherTimer.Tick += DispatcherTimer_Tick;
				dispatcherTimer.Start();
			//}



			KeyDown += WindowMain_KeyDown;

			CreateNewYearTheme();
			DataContext = this;
		}


		private void SetupLabelTitle(bool isPromoJustNow) {
			TextBlockTime.Text = DateTime.Now.ToShortTimeString();
			ImageLogo.Visibility = isPromoJustNow ? Visibility.Hidden : Visibility.Visible;
			WrapPanelCurrentTime.Visibility = isPromoJustNow ? Visibility.Visible : Visibility.Hidden;
			Brush labelTitleForeground = isPromoJustNow ?
				BrushFromRGB(Properties.Settings.Default.ColorTitlePromoBackground) :
				BrushFromRGB(Properties.Settings.Default.ColorFontMain);
			LabelTitle.Margin = isPromoJustNow ? new Thickness(0) : new Thickness(20, 20, 0, 0);
			LabelTitle.HorizontalContentAlignment = isPromoJustNow ? HorizontalAlignment.Center : HorizontalAlignment.Left;
			LabelTitle.VerticalContentAlignment = isPromoJustNow ? VerticalAlignment.Center : VerticalAlignment.Top;
			
			SolidColorBrush solidColorBrushBackground = new SolidColorBrush(Colors.Transparent);
			ColorAnimation colorAnimation = null;
			DoubleAnimation doubleAnimation = null;
			if (isPromoJustNow) {
				colorAnimation = new ColorAnimation {
					From = Colors.Orange,
					To = Colors.OrangeRed,
					Duration = new Duration(TimeSpan.FromSeconds(3)),
					RepeatBehavior = RepeatBehavior.Forever,
					AutoReverse = true
				};

				doubleAnimation = new DoubleAnimation {
					From = FontSizeHeader,
					To = FontSizeHeader * 1.15,
					Duration = new Duration(TimeSpan.FromSeconds(3)),
					RepeatBehavior = RepeatBehavior.Forever,
					AutoReverse = true
				};

				TitleText = Properties.Resources.WindowMainTItlePromoJustNow;
			}

			LabelTitle.Foreground = isPromoJustNow ? 
				BrushFromRGB(Properties.Settings.Default.ColorTitlePromoForeground) : 
				ForegroundColorMain;
			LabelTitle.Background = solidColorBrushBackground;
			solidColorBrushBackground.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
			LabelTitle.BeginAnimation(FontSizeProperty, doubleAnimation);
		}

		private void CreateNewYearTheme() {
			int currentDay = DateTime.Now.Day;
			int currentMonth = DateTime.Now.Month;
			if ((currentMonth == 12 && currentDay >= 10) ||
				(currentMonth == 1 && currentDay <= 9)) {
				ImageLogo.Source = PageMarks.GetResourceImage("PicChristmasTree");
				canvasSnowfall.Visibility = Visibility.Visible;
				List<string> snows = new List<string>();

				for (int i = 1; i <= 9; i++)
					snows.Add("pack://application:,,,/Graphics/snow@.png".Replace("@", i.ToString()));

				SnowEngine snow = new SnowEngine(canvasSnowfall, snows.ToArray());
				snow.Start();
			}
		}

		public static SolidColorBrush BrushFromRGB(System.Drawing.Color color) {
			return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
		}



		private void DispatcherTimer_Tick(object sender, EventArgs e) {
			Console.WriteLine("DispatcherTimer_Tick: " + DateTime.Now.ToLongTimeString());
			Page navigateTo;
			ItemDataResult dataResult;

			switch (currentState) {
				case CurrentStates.About:
					currentState = CurrentStates.ClinicRecommendations;
					dataResult = dataService.GetRecommendationResult();

					if (!Properties.Settings.Default.ShowLoyaltyInfo || dataResult.Total == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

					navigateTo = new PageMarks(currentState, dataResult);
					TitleText = Properties.Resources.WindowMainTitleClinicRecommendations;

					break;
				case CurrentStates.ClinicRecommendations:
					currentState = CurrentStates.DoctorsMarks;

					dataResult = dataService.GetQualityResult();

					if (!Properties.Settings.Default.ShowLoyaltyInfo || dataResult.Total == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

					navigateTo = new PageMarks(currentState, dataResult);
					TitleText = Properties.Resources.WindowMainTitleDoctorsMarks;

					break;
				case CurrentStates.DoctorsMarks:
					currentState = CurrentStates.PromoJustNow;

					ItemPromoJustNow promoJustNowLastUpdated = dataService.GetPromoJustNow();

					if (!Properties.Settings.Default.ShowPromoJustNow ||
						promoJustNowLastUpdated.Departments.Count == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

					Typeface typeface = new Typeface(FontFamilyMain, FontStyle, FontWeight, FontStretch);
					SetupLabelTitle(true);

					//ItemPromoJustNow promoJustNowToShow = new ItemPromoJustNow {
					//	DateTimeUpdated = promoJustNowLastUpdated.DateTimeUpdated
					//};

					//(sender as DispatcherTimer).Stop();
					//Task.Run(() => {
					//	int gridRow = 1;

					//	foreach (KeyValuePair<string, ItemDepartment> department in promoJustNowLastUpdated.Departments) {
					//		if (gridRow >= 9) {
					//			Application.Current.Dispatcher.Invoke(new Action(() => {
					//				PagePromoJustNow pagePromoJustNow = new PagePromoJustNow(promoJustNowToShow, typeface, FontSizeMain) {
					//					FontFamily = FontFamily,
					//					FontSize = FontSize,
					//					Foreground = Foreground
					//				};

					//				frame.Navigate(pagePromoJustNow);
					//			}));
								
					//			promoJustNowToShow = new ItemPromoJustNow {
					//				DateTimeUpdated = promoJustNowLastUpdated.DateTimeUpdated
					//			};

					//			gridRow = 1;
					//			Thread.Sleep(TimeSpan.FromSeconds(Properties.Settings.Default.PageChangingPeriodInSeconds));
					//			Console.WriteLine("departments >= 9");
					//		}

					//		gridRow++;

					//		foreach (KeyValuePair<string, ItemDoctor> doctor in department.Value.Doctors) {
					//			if (gridRow > 9) {
					//				Application.Current.Dispatcher.Invoke(new Action(() => {
					//					PagePromoJustNow pagePromoJustNow = new PagePromoJustNow(promoJustNowToShow, typeface, FontSizeMain) {
					//						FontFamily = FontFamily,
					//						FontSize = FontSize,
					//						Foreground = Foreground
					//					};

					//					frame.Navigate(pagePromoJustNow);
					//				}));

					//				promoJustNowToShow = new ItemPromoJustNow {
					//					DateTimeUpdated = promoJustNowLastUpdated.DateTimeUpdated
					//				};

					//				gridRow = 1;
					//				Thread.Sleep(TimeSpan.FromSeconds(Properties.Settings.Default.PageChangingPeriodInSeconds));
					//				Console.WriteLine("doctor > 9");
					//			}


					//			if (!promoJustNowToShow.Departments.ContainsKey(department.Key))
					//				promoJustNowToShow.Departments.Add(department.Key, new ItemDepartment(department.Key));
					//			promoJustNowToShow.Departments[department.Key].Doctors.Add(doctor.Key, doctor.Value);

					//			gridRow++;
					//		}

					//		//(sender as DispatcherTimer).Start();
					//	}
					//});


					//if (promoJustNowToShow.Departments.Count == 0) {
					//	DispatcherTimer_Tick(sender, e);
					//	return;
					//}

					navigateTo = new PagePromoJustNow(promoJustNowLastUpdated, typeface, FontSizeMain);

					break;
				case CurrentStates.PromoJustNow:
					currentState = CurrentStates.About;

					navigateTo = new PageAbout();
					SetupLabelTitle(false);
					TitleText = Properties.Resources.WindowMainTitleAbout;

					break;
				default:
					return;
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
