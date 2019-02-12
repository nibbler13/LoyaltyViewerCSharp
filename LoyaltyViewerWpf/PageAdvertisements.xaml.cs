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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoyaltyViewerWpf {
	/// <summary>
	/// Interaction logic for PageAdvertisements.xaml
	/// </summary>
	public partial class PageAdvertisements : Page {
		public PageAdvertisements(string advertisementPath) {
			InitializeComponent();

			try {
				ImageMain.Source = new BitmapImage(new Uri(advertisementPath));
			} catch (Exception e) {
				SystemLogging.LogMessageToFile(e.Message + Environment.NewLine + e.StackTrace);
			}
		}
	}
}
