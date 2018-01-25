using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoyaltyViewerWpf {
    /// <summary>
    /// Логика взаимодействия для PagePromoJustNow.xaml
    /// </summary>
    public partial class PagePromoJustNow : Page {
		public string SubtitleText { get; set; }
		private TextBlock _textBlockTime;
		private ItemPromoJustNow _promoJustNow;

        public PagePromoJustNow(ItemPromoJustNow promoJustNow, TextBlock textBlockTime) {
			_promoJustNow = promoJustNow;
			_textBlockTime = textBlockTime;

			DataContext = this;
			SubtitleText = Properties.Resources.PromoJustNowSubtitle;
            InitializeComponent();

			DrawContent();
			UpdateTextBoxTime();

			Loaded += PagePromoJustNow_Loaded;
        }

		private void PagePromoJustNow_Loaded(object sender, RoutedEventArgs e) {
			DrawTimeLines();
		}

		private void UpdateTextBoxTime() {
			_textBlockTime.Text = _promoJustNow.DateTimeUpdated.ToShortTimeString();
		}

		private void DrawTimeLines() {
			double linesWidth = GridData.ColumnDefinitions[1].ActualWidth;
			double linesToDraw = Properties.Settings.Default.PromoJustNowShowIntervalMinutes;
			double step = linesWidth / linesToDraw;
			double currentX = 0;
			double gridRowHeight = GridData.RowDefinitions[0].ActualHeight;
			double cellHeight = gridRowHeight * 0.8;
			SolidColorBrush colorBrushMinuteRegular = new SolidColorBrush(Color.FromRgb(240, 240, 240));
			SolidColorBrush colorBrushMinuteSection = new SolidColorBrush(Color.FromRgb(220, 220, 220));

			//horizontal lines between doctors
			int rowsUsed = 1;
			bool previousIsDepartment = false;
			for (int gridRow = rowsUsed; gridRow < 10; gridRow++) {
				bool currentIsDoctor = false;
				bool hasChild = false;
				SortedDictionary<DateTime, ItemFreeCell> freeCells = null;

				foreach (UIElement element in GridData.Children) {
					if (Grid.GetRow(element) == gridRow &&
						Grid.GetColumn(element) == 0) {
						rowsUsed++;
						hasChild = true;

						if (element is Border) {
							object tag = (element as Border).Tag;
							if (tag is string) {
								previousIsDepartment = true;
								break;
							}

							freeCells = tag as SortedDictionary<DateTime, ItemFreeCell>;
						}
						
						currentIsDoctor = true;
						break;
					}
				}

				if (!currentIsDoctor && hasChild)
					continue;

				if (freeCells != null) {
					foreach (ItemFreeCell freeCell in freeCells.Values) {
						string cellText = freeCell.Begin.ToShortTimeString();
						int duration = freeCell.Duration.Minutes;

						double minutesBeforeTheStart = (freeCell.Begin - _promoJustNow.DateTimeUpdated).TotalMinutes;
						double cellWidth = step * duration;
						double cellLeft = linesWidth + (minutesBeforeTheStart * step);
						double cellTop = gridRow * gridRowHeight + (gridRowHeight - cellHeight) / 2;

						Border cellBorder = CreateBorderWithTextBlock(cellText);
						cellBorder.Background = WindowMain.BrushFromRGB(Properties.Settings.Default.ColorTitlePromoBackground);
						(cellBorder.Child as TextBlock).Foreground = WindowMain.BrushFromRGB(Properties.Settings.Default.ColorTitlePromoForeground);
						(cellBorder.Child as TextBlock).TextAlignment = TextAlignment.Center;
						//(cellBorder.Child as TextBlock).FontSize = _textBlockTime.FontSize / 1.5;
						(cellBorder.Child as TextBlock).FontWeight = FontWeights.UltraBold;

						cellBorder.Width = cellWidth;
						cellBorder.Height = cellHeight;
						cellBorder.BorderThickness = new Thickness(2);
						cellBorder.BorderBrush = Brushes.Transparent;
						Canvas.SetLeft(cellBorder, cellLeft);
						Canvas.SetTop(cellBorder, cellTop);

						AddDropShadowEffect(cellBorder);

						CanvasMain.Children.Add(cellBorder);

						Image image = new Image();
						image.Source = PageMarks.GetResourceImage("Sale-30");
						image.Width = cellHeight;
						image.Height = cellHeight;
						Canvas.SetLeft(image, cellLeft + cellWidth - cellHeight);
						Canvas.SetTop(image, cellTop);
						CanvasMain.Children.Add(image);

						DoubleAnimation da = new DoubleAnimation();
						da.From = 1;
						da.To = 0;
						da.Duration = new Duration(TimeSpan.FromSeconds(1));
						da.AutoReverse = true;
						da.RepeatBehavior = RepeatBehavior.Forever;
						image.BeginAnimation(OpacityProperty, da);
					}
				}

				if (previousIsDepartment) {
					previousIsDepartment = false;
					continue;
				}

				double y = gridRow * gridRowHeight;
				Line line = CreateLine(0, y, linesWidth * 2, y, 1);
				line.Stroke = colorBrushMinuteSection;

				Grid.SetColumnSpan(line, 2);
				Grid.SetRowSpan(line, 10);
				GridData.Children.Add(line);

				//last line reached
				if (!hasChild)
					break;
			}

			//vertical timelines
			double linesHeight = gridRowHeight * (rowsUsed - 1);
			DateTime dateTimeCurrent = _promoJustNow.DateTimeUpdated;

			for (int i = 0; i <= linesToDraw; i++) {
				Line line = CreateLine(currentX, 0, currentX, linesHeight, 1);

				if (dateTimeCurrent.Minute == 0 ||
					dateTimeCurrent.Minute == 30) {
					line.Stroke = colorBrushMinuteSection;

					string sectionTime = dateTimeCurrent.ToShortTimeString();
					Size sectionTimeSize = MeasureString(sectionTime);
					TextBlock textBlock = new TextBlock {
						Text = sectionTime,
						Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
						FontSize = _textBlockTime.FontSize / 1.5
					};

					double left = linesWidth + currentX - sectionTimeSize.Width / 2;
					double top = gridRowHeight - sectionTimeSize.Height;

					//textBox must be inside canvas
					if (left + sectionTimeSize.Width / 2 < linesWidth * 2) {
						Canvas.SetLeft(textBlock, left);
						Canvas.SetTop(textBlock, top);
						CanvasMain.Children.Add(textBlock);
					}
				} else
					line.Stroke = colorBrushMinuteRegular;

				Grid.SetColumn(line, 1);
				Grid.SetRow(line, 1);
				Grid.SetRowSpan(line, 9);
				Grid.SetZIndex(line, -1);
				GridData.Children.Add(line);
				currentX += step;

				dateTimeCurrent = dateTimeCurrent.AddMinutes(1);
			}


			TextBlockSubtitle.FontSize = _textBlockTime.FontSize / 1.5;
		}

		private Size MeasureString(string candidate) {
			var formattedText = new FormattedText(
				candidate,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface(_textBlockTime.FontFamily, _textBlockTime.FontStyle, _textBlockTime.FontWeight, _textBlockTime.FontStretch),
				_textBlockTime.FontSize / 1.5,
				Brushes.Black);

			return new Size(formattedText.Width, formattedText.Height);
		}

		private Line CreateLine(double x1, double y1, double x2, double y2, int thickness) {
			Line line = new Line();
			line.X1 = x1;
			line.Y1 = y1;
			line.X2 = x2;
			line.Y2 = y2;
			line.StrokeThickness = thickness;

			return line;
		}

		private void DrawContent() {
			if (_promoJustNow.Departments.Count == 0) {
				//nothing to show

				return;
			}

			int gridRow = 1;

			foreach (KeyValuePair<string, ItemDepartment> department in _promoJustNow.Departments) {
				if (gridRow >= 9)
					break;

				CreateBorderWithTextBlock(department.Key, gridRow, true);
				gridRow++;

				foreach (KeyValuePair<string, ItemDoctor> doctor in department.Value.Doctors) {
					if (gridRow > 9)
						break;

					CreateBorderWithTextBlock(doctor.Key, gridRow, false, doctor.Value.FreeCells);
					gridRow++;
				}
			}
		}

		private Border CreateBorderWithTextBlock(string value, int gridRow = 0, bool isDepartment = false, SortedDictionary<DateTime, ItemFreeCell> freeCells = null) {
			SolidColorBrush background = Brushes.Transparent;
			TextAlignment textAlignment = TextAlignment.Left;
			FontWeight fontWeight = FontWeights.Normal;
			int columnSpan = 1;

			if (isDepartment) {
				background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
				textAlignment = TextAlignment.Center;
				fontWeight = FontWeights.UltraBold;
				columnSpan = 2;
			}

			Border border = new Border {
				Background = background
			};

			TextBlock textBlock = new TextBlock {
				Text = value,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				TextAlignment = textAlignment,
				FontWeight = fontWeight
			};

			if (isDepartment) {
				AddDropShadowEffect(border);
				border.Tag = "department";
			} else {
				textBlock.Margin = new Thickness(20, 0, 20, 0);
				border.Tag = freeCells;
			}

			border.Child = textBlock;

			if (gridRow > 0) {
				Grid.SetRow(border, gridRow);
				Grid.SetColumnSpan(border, columnSpan);
				GridData.Children.Add(border);
			}

			return border;
		}

		private void AddDropShadowEffect(UIElement element) {
			DropShadowEffect effect = new DropShadowEffect {
				Color = Color.FromRgb(200, 200, 200),
				Opacity = 0.2,
				Direction = 320
			};

			element.Effect = effect;

		}
    }
}
