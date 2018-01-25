using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoyaltyViewerWpf {
    /// <summary>
    /// Логика взаимодействия для PageAbout.xaml
    /// </summary>
    public partial class PageAbout : Page {
		public Brush ForegroundColorSub { get; set; }

		public PageAbout() {
            InitializeComponent();

			ForegroundColorSub = new SolidColorBrush(Color.FromArgb(Properties.Settings.Default.ColorFontSub.A,
				Properties.Settings.Default.ColorFontSub.R,
				Properties.Settings.Default.ColorFontSub.G,
				Properties.Settings.Default.ColorFontSub.B));

			DataContext = this;
			Loaded += PageAbout_Loaded;
		}

		private void PageAbout_Loaded(object sender, RoutedEventArgs e) {
			DoubleAnimation gridAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
			gridMain.BeginAnimation(OpacityProperty, gridAnimation);
		}
	}
}
