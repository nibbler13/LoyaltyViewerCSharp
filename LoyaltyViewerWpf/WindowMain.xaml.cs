using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace LoyaltyViewerWpf {
	public partial class WindowMain : Window, INotifyPropertyChanged {
		public static string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public enum AvailablePages { About, ClinicRecommendations, DoctorsMarks, PromoJustNow, Advertisements };

		public double FontSizeMain { get; set; }
		public double FontSizeHeader { get; set; }
		public FontFamily FontFamilyMain { get; set; }
		public Brush ForegroundColorMain {get; set; }
		private AvailablePages previousPage = AvailablePages.About;
		private SystemDataService dataService = new SystemDataService();

		private string titleText;
		public string TitleText {
			get {
				return titleText;
			} set {
				if (value != titleText) {
					titleText = value;
					NotifyPropertyChanged();
				}
			}
		}

		public string AboutDeveloper { get; set; }
		private DispatcherTimer dispatcherTimer;

        private int currentAdvertisement = 0;


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
			ForegroundColorMain = ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorFontMain);

			TitleText = Properties.Resources.WindowMainTitleAbout;
			SetupLabelTitle(false);

			AboutDeveloper = Properties.Resources.AboutDeveloper;
			TextBlockAboutDeveloper.FontSize = FontSizeMain / 2;
			TextBlockAboutDeveloper.Foreground = ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorFontAboutDeveloper);

			dispatcherTimer = new DispatcherTimer {
				Interval = TimeSpan.FromSeconds(Properties.Settings.Default.PageChangingPeriodInSeconds)
			};

			dispatcherTimer.Tick += DispatcherTimer_Tick;
			dispatcherTimer.Start();

			KeyDown += WindowMain_KeyDown;

			CreateNewYearTheme();
			DataContext = this;

			FrameMain.Navigating += Frame_Navigating;
		}

		private void Frame_Navigating(object sender, NavigatingCancelEventArgs e) {
			try {
				FrameMain.NavigationService.RemoveBackEntry();
			} catch (Exception exc) {
				SystemLogging.LogMessageToFile(exc.Message + Environment.NewLine + exc.StackTrace);
			}
		}

		private void SetupLabelTitle(bool isPromoJustNow) {
			TextBlockTime.Text = DateTime.Now.ToShortTimeString();
			ImageLogo.Visibility = isPromoJustNow ? Visibility.Hidden : Visibility.Visible;
			WrapPanelCurrentTime.Visibility = isPromoJustNow ? Visibility.Visible : Visibility.Hidden;
			Brush labelTitleForeground = isPromoJustNow ?
				ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorTitlePromoBackground) :
				ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorFontMain);
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
				ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorTitlePromoForeground) : 
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
				ImageLogo.Source = ControlsFactory.GetResourceImage("PicChristmasTree");
				canvasSnowfall.Visibility = Visibility.Visible;
				List<string> snows = new List<string>();

				for (int i = 1; i <= 9; i++)
					snows.Add("pack://application:,,,/Graphics/snow@.png".Replace("@", i.ToString()));

				SnowEngine snow = new SnowEngine(canvasSnowfall, snows.ToArray());
				snow.Start();
			}
		}

		private void SetRootElementsVisibility(Visibility visibility) {
			List<UIElement> elements = new List<UIElement> {
				LabelTitle,
				ImageLogo,
				TextBlockAboutDeveloper,
				GridBottom
			};

			foreach (UIElement element in elements)
				element.Visibility = visibility;

			int frameRow = 1;
			int frameRowSpan = 1;

			if (visibility == Visibility.Hidden) {
				frameRow = 0;
				frameRowSpan = 3;
			}

			Grid.SetRow(FrameMain, frameRow);
			Grid.SetRowSpan(FrameMain, frameRowSpan);

			//RowDefinitionTitle.Height = new GridLength(
			//	visibility == Visibility.Collapsed ?
			//	0 : 15, GridUnitType.Star);

			//GridMain.RowDefinitions[0].Height = new GridLength(
			//	visibility == Visibility.Collapsed ?
			//	0 : 15, GridUnitType.Star);
		}



		private async Task PutTaskDelay() {
			await Task.Delay(TimeSpan.FromSeconds(Properties.Settings.Default.PageChangingPeriodInSeconds));
		}



		private async void DispatcherTimer_Tick(object sender, EventArgs e) {
			Page pageNavigateTo;
			ItemDataResult dataResult;

			switch (previousPage) {
				case AvailablePages.About:
					previousPage = AvailablePages.ClinicRecommendations;
					dataResult = dataService.GetRecommendationResult();

					if (!Properties.Settings.Default.ShowLoyaltyInfo || dataResult.Total == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

                    SystemLogging.LogMessageToFile("Отображение страницы ClinicRecommendations");
					pageNavigateTo = new PageMarks(previousPage, dataResult);
					TitleText = Properties.Resources.WindowMainTitleClinicRecommendations;

					break;
				case AvailablePages.ClinicRecommendations:
					previousPage = AvailablePages.DoctorsMarks;

					dataResult = dataService.GetQualityResult();

					if (!Properties.Settings.Default.ShowLoyaltyInfo || dataResult.Total == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

                    SystemLogging.LogMessageToFile("Отображение страницы DoctorsMarks");
                    pageNavigateTo = new PageMarks(previousPage, dataResult);
					TitleText = Properties.Resources.WindowMainTitleDoctorsMarks;

					break;
				case AvailablePages.DoctorsMarks:
					previousPage = AvailablePages.PromoJustNow;

					ItemPromoJustNow promoJustNowLastUpdated = dataService.GetPromoJustNow();

					if (!Properties.Settings.Default.ShowPromoJustNow ||
						promoJustNowLastUpdated.Departments.Count == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

                    SystemLogging.LogMessageToFile("Отображение страницы PromoJustNow");
                    SetupLabelTitle(true);

                    ItemPromoJustNow promoJustNowToShow = new ItemPromoJustNow {
						DateTimeUpdated = promoJustNowLastUpdated.DateTimeUpdated
					};
					
					dispatcherTimer.Stop();
					int gridRow = 1;

					foreach (KeyValuePair<string, ItemDepartment> department in promoJustNowLastUpdated.Departments) {
						if (gridRow >= 9) {
							NavigateToPromoJustNow(ref promoJustNowToShow, promoJustNowLastUpdated.DateTimeUpdated, ref gridRow);
							await PutTaskDelay();
						}

						gridRow++;

						foreach (KeyValuePair<string, ItemDoctor> doctor in department.Value.Doctors) {
							if (gridRow > 9) {
								NavigateToPromoJustNow(ref promoJustNowToShow, promoJustNowLastUpdated.DateTimeUpdated, ref gridRow);
								await PutTaskDelay();
							}

							if (!promoJustNowToShow.Departments.ContainsKey(department.Key))
								promoJustNowToShow.Departments.Add(department.Key, new ItemDepartment(department.Key));

							promoJustNowToShow.Departments[department.Key].Doctors.Add(doctor.Key, doctor.Value);

							gridRow++;
						}
					}
					
					dispatcherTimer.Start();

					if (promoJustNowToShow.Departments.Count == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

                    SystemLogging.LogMessageToFile("Отображение страницы PromoJustNow");
                    pageNavigateTo = new PagePromoJustNow(promoJustNowToShow);

					break;
				case AvailablePages.PromoJustNow:
					previousPage = AvailablePages.Advertisements;

					List<string> advertisementsAvailable = new List<string>();

					try {
						string searchDir = Path.Combine(AssemblyDirectory, "Advertisements");

						List<string> advertisementsInFolder = Directory.GetFiles(searchDir, "*.*", SearchOption.AllDirectories).
							Where(f => new List<string> { ".jpg", ".png" }.IndexOf(Path.GetExtension(f)) >= 0).ToList();

						foreach (string item in advertisementsInFolder) {
							string itemName = Path.GetFileName(item);

							if (!itemName.StartsWith("[")) {
								advertisementsAvailable.Add(item);
								continue;
							}

							try {
								string dateToStop = itemName.Substring(1, 10);
								if (DateTime.TryParse(dateToStop, out DateTime dt)) {
									if (DateTime.Now.Date >= dt)
										continue;

									advertisementsAvailable.Add(item);
								}
							} catch (Exception) { }
						}
					} catch (Exception) { }

					if (!Properties.Settings.Default.ShowLastPageAdvertisements || 
						advertisementsAvailable.Count == 0) {
						DispatcherTimer_Tick(sender, e);
						return;
					}

                    SystemLogging.LogMessageToFile("Отображение страницы Advertisements");
                    SystemLogging.LogMessageToFile("Доступные изображения: " + Environment.NewLine +
                        string.Join(Environment.NewLine, advertisementsAvailable));

                    if (currentAdvertisement >= advertisementsAvailable.Count)
                        currentAdvertisement = 0;

                    string advertisement = advertisementsAvailable[currentAdvertisement++];
                    SystemLogging.LogMessageToFile("Выбранное изображение: " + advertisement);

					SetRootElementsVisibility(Visibility.Hidden);
					pageNavigateTo = new PageAdvertisements(advertisement);

					break;
				case AvailablePages.Advertisements:
					previousPage = AvailablePages.About;

                    SystemLogging.LogMessageToFile("Отображение страницы About");
                    pageNavigateTo = new PageAbout();
					SetupLabelTitle(false);
					TitleText = Properties.Resources.WindowMainTitleAbout;
					SetRootElementsVisibility(Visibility.Visible);

					break;
				default:
					return;
			}

			FrameNavigateTo(pageNavigateTo);
		}

		private void NavigateToPromoJustNow(ref ItemPromoJustNow promoJustNowToShow, DateTime dateTimeUpdated, ref int gridRow) {
			PagePromoJustNow pagePromoJustNow = new PagePromoJustNow(promoJustNowToShow);
			FrameNavigateTo(pagePromoJustNow);
			promoJustNowToShow = new ItemPromoJustNow { DateTimeUpdated = dateTimeUpdated };
			gridRow = 1;
		}

		private void FrameNavigateTo(Page page) {
			page.FontFamily = FontFamily;
			page.FontSize = FontSize;
			page.Foreground = Foreground;

			FrameMain.Navigate(page);
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
