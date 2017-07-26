using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoyaltyViewer {
	static class Program {
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main() {
			LoggingSystem.LogMessageToFile("Запуск");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ApplicationExit += Application_ApplicationExit;
			Application.Run(new FormInfo());
		}

		private static void Application_ApplicationExit(object sender, EventArgs e) {
			LoggingSystem.LogMessageToFile("Завершение работы приложения");
		}
	}
}
