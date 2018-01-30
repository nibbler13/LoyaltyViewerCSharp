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
		public string NothingToShow { get; set; }
		private ItemPromoJustNow promoJustNow;
		private SolidColorBrush colorBrushMinuteRegular;
		private SolidColorBrush colorBrushMinuteSection;
		private SolidColorBrush colorBrushDepartmentBackground;
		private SolidColorBrush colorBrushTreatmentBackground;
		private SolidColorBrush colorBrushTreatmentForeground;
		private SolidColorBrush colorTimeStampForeground;
		private int intervalToShow;

		public enum CellType { Department, Doctor, Treatment }

		public PagePromoJustNow(ItemPromoJustNow promoJustNow) {
			this.promoJustNow = promoJustNow;
			SubtitleText = Properties.Resources.PromoJustNowSubtitle;
			NothingToShow = Properties.Resources.PromoJustNowNothingToShow;
			colorBrushMinuteRegular = new SolidColorBrush(Color.FromRgb(240, 240, 240));
			colorBrushMinuteSection = new SolidColorBrush(Color.FromRgb(220, 220, 220));
			colorBrushDepartmentBackground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
			colorBrushTreatmentBackground = ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorTitlePromoBackground);
			colorBrushTreatmentForeground = ControlsFactory.BrushFromRGB(Properties.Settings.Default.ColorTitlePromoForeground);
			colorTimeStampForeground = new SolidColorBrush(Color.FromRgb(200, 200, 200));
			intervalToShow = Properties.Settings.Default.PromoJustNowFreeCellsShowIntervalMinutes;

			DataContext = this;
			InitializeComponent();

			DrawContent();
		}

		private void DrawContent() {
			if (promoJustNow.Departments.Count == 0) {
				foreach (UIElement element in GridMain.Children)
					element.Visibility = Visibility.Hidden;

				LabelNothingToShow.Visibility = Visibility.Visible;

				return;
			}

			int gridRow = 1;
			foreach (KeyValuePair<string, ItemDepartment> department in promoJustNow.Departments) {
				if (gridRow >= 9)
					break;

				CreateBorderWithTextBlock(CellType.Department, department.Key, gridRow, "department");
				gridRow++;

				foreach (KeyValuePair<string, ItemDoctor> doctor in department.Value.Doctors) {
					if (gridRow > 9)
						break;

					CreateBorderWithTextBlock(CellType.Doctor, doctor.Key, gridRow, doctor.Value);
					gridRow++;
				}
			}

			Loaded += PagePromoJustNow_Loaded;
		}

		private void PagePromoJustNow_Loaded(object sender, RoutedEventArgs e) {
			TextBlockSubtitle.FontSize = FontSize / 2;
			DrawTimeLinesAndCells();
		}

		private void DrawTimeLinesAndCells() {
			double linesWidthTotal = GridData.ColumnDefinitions[1].ActualWidth;
			double doctorsWidth = GridData.ColumnDefinitions[0].ActualWidth;
			double gridRowHeight = GridData.RowDefinitions[0].ActualHeight;
			double availableWidth = GridData.ActualWidth;
			double minuteStepWidth = linesWidthTotal / intervalToShow;
			double cellHeight = gridRowHeight * 0.8;
			double currentX = 0;
			Typeface typeface = new Typeface(FontFamily, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

			//horizontal lines between doctors
			int rowsUsed = 1;
			bool isPreviousLineADepartment = false;
			for (int currentRow = rowsUsed; currentRow < 11; currentRow++) {
				bool isCurrentADoctor = false;
				bool isCellHasChilds = false;
				ItemDoctor freeCells = null;

				foreach (UIElement element in GridData.Children) {
					if (Grid.GetRow(element) == currentRow && Grid.GetColumn(element) == 0) {
						rowsUsed++;
						isCellHasChilds = true;

						if (element is Border) {
							object tag = (element as Border).Tag;
							if (tag is string) {
								isPreviousLineADepartment = true;
								break;
							}

							freeCells = tag as ItemDoctor;
						}
						
						isCurrentADoctor = true;
						break;
					}
				}

				if (!isCurrentADoctor && isCellHasChilds)
					continue;

				if (freeCells != null) {
					foreach (ItemFreeCell freeCell in freeCells.FreeCells.Values) {
						string cellText = freeCell.Begin.ToShortTimeString();
						int duration = (int)freeCell.Duration.TotalMinutes;

						double minutesBeforeTheStart = (int)(freeCell.Begin - promoJustNow.DateTimeUpdated).TotalMinutes;
						double cellWidth = minuteStepWidth * duration;
						double cellLeft = doctorsWidth + (minutesBeforeTheStart * minuteStepWidth);
						double cellTop = currentRow * gridRowHeight + (gridRowHeight - cellHeight) / 2;

						Border cellBorder = CreateBorderWithTextBlock(CellType.Treatment, cellText);

						cellBorder.Width = cellWidth;
						cellBorder.Height = cellHeight;
						cellBorder.BorderThickness = new Thickness(2);
						cellBorder.BorderBrush = Brushes.Transparent;

						Canvas.SetLeft(cellBorder, cellLeft);
						Canvas.SetTop(cellBorder, cellTop);
						Canvas.SetZIndex(cellBorder, 1);
						CanvasMain.Children.Add(cellBorder);

						Image image = ControlsFactory.CreateImage("Sale-30", cellHeight, cellHeight);
						Canvas.SetLeft(image, cellLeft + cellWidth - cellHeight);
						Canvas.SetTop(image, cellTop);
						Canvas.SetZIndex(image, 2);
						CanvasMain.Children.Add(image);
					}
				}

				if (isPreviousLineADepartment) {
					isPreviousLineADepartment = false;
					continue;
				}

				double y = currentRow * gridRowHeight;
				Line line = ControlsFactory.CreateLine(0, y, availableWidth, y, 1);
				line.Stroke = colorBrushMinuteSection;

				Grid.SetColumnSpan(line, 2);
				Grid.SetRowSpan(line, 10);
				GridData.Children.Add(line);

				//last line reached
				if (!isCellHasChilds)
					break;
			}

			//vertical timelines
			double linesHeight = gridRowHeight * (rowsUsed - 1);
			DateTime dateTimeCurrent = promoJustNow.DateTimeUpdated;

			for (int i = 0; i < intervalToShow; i++) {
				Line line = ControlsFactory.CreateLine(currentX, 0, currentX, linesHeight, 1);

				if (dateTimeCurrent.Minute == 0 ||
					dateTimeCurrent.Minute == 30) {
					line.Stroke = colorBrushMinuteSection;

					string sectionTime = dateTimeCurrent.ToShortTimeString();
					Size sectionTimeSize = ControlsFactory.MeasureString(sectionTime, typeface, FontSize / 1.5);

					TextBlock textBlock = ControlsFactory.CreateTextBlock(
						sectionTime, 
						HorizontalAlignment.Center, 
						VerticalAlignment.Center, 
						TextAlignment.Center, 
						new Thickness(0), 
						FontWeights.Normal, 
						colorTimeStampForeground);
					textBlock.FontSize = FontSize / 1.5;

					double left = doctorsWidth + currentX - sectionTimeSize.Width / 2;
					double top = gridRowHeight - sectionTimeSize.Height;

					//textBox must be inside canvas
					if (left + sectionTimeSize.Width < availableWidth) {
						Canvas.SetLeft(textBlock, left);
						Canvas.SetTop(textBlock, top);
						CanvasMain.Children.Add(textBlock);
					}
				} else {
					if (dateTimeCurrent.Minute % 5 != 0)
						line.Stroke = new SolidColorBrush(Colors.White);
					else
						line.Stroke = colorBrushMinuteRegular;
				}

				Grid.SetColumn(line, 1);
				Grid.SetRow(line, 1);
				Grid.SetRowSpan(line, 9);
				Grid.SetZIndex(line, -1);
				GridData.Children.Add(line);

				currentX += minuteStepWidth;
				dateTimeCurrent = dateTimeCurrent.AddMinutes(1);
			}
		}

		private Border CreateBorderWithTextBlock(CellType cellType, string text, int gridRow = -1, object tag = null) {
			SolidColorBrush background;
			Brush foreground = Foreground;
			TextAlignment textAlignment = TextAlignment.Center;
			FontWeight fontWeight = FontWeights.UltraBold;
			Thickness margin = new Thickness(0);
			VerticalAlignment verticalAlignment = VerticalAlignment.Center;
			HorizontalAlignment horizontalAlignment = HorizontalAlignment.Stretch;
			int columnSpan = 1;


			switch (cellType) {
				case CellType.Department:
					background = colorBrushDepartmentBackground;
					columnSpan = 2;
					break;
				case CellType.Doctor:
					background = Brushes.Transparent;
					textAlignment = TextAlignment.Left;
					fontWeight = FontWeights.Normal;
					margin = new Thickness(20, 0, 20, 0);
					break;
				case CellType.Treatment:
					background = colorBrushTreatmentBackground;
					foreground = colorBrushTreatmentForeground;
					break;
				default:
					return null;
			}
			
			TextBlock textBlock = ControlsFactory.CreateTextBlock(
				text, 
				horizontalAlignment, 
				verticalAlignment, 
				textAlignment, 
				margin, 
				fontWeight, 
				foreground);

			Border border = ControlsFactory.CreateBorder(background, tag, textBlock);

			if (gridRow > -1) {
				Grid.SetRow(border, gridRow);
				Grid.SetColumnSpan(border, columnSpan);
				GridData.Children.Add(border);
			}

			return border;
		}
    }
}
