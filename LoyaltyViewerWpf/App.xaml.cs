using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LoyaltyViewerWpf {
	/// <summary>
	/// Логика взаимодействия для App.xaml
	/// </summary>
	public partial class App : Application {
		private void Application_Startup(object sender, StartupEventArgs e) {
			DispatcherUnhandledException += App_DispatcherUnhandledException;

			WindowMain windowMain = new WindowMain();
			windowMain.Show();
		}

		private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			SystemLogging.LogMessageToFile(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
			SystemMail.SendMail(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
			SystemLogging.LogMessageToFile("!!!App - Аварийное завершение работы");
			Process.GetCurrentProcess().Kill();
		}
	}
}
