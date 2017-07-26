using System.Net.Mail;
using System;

namespace LoyaltyViewer {
	class MailSystem {
		public static void SendMail(string body) {
			LoggingSystem.LogMessageToFile("Отправка e-mail с текстом: " + body);

			string toAddress = Properties.Settings.Default.formDebug ?
							   "nn-admin@bzklinika.ru" : Properties.Settings.Default.mailTo;
			string copy = Properties.Settings.Default.mailCopy;
			string subject = "Уведомление";
			string server = Properties.Settings.Default.mailServer;
			string user = Properties.Settings.Default.mailUser;
			string password = Properties.Settings.Default.mailPassword;
			string domain = Properties.Settings.Default.mailDomain;
			string opening = "На группу поддержки бизнес-приложений: " + Environment.NewLine;
			string ending = "\n\nЭто автоматически сгенерированное сообщение\n" +
							"Просьба не отвечать на него\n" +
							"Имя системы: " + Environment.MachineName;

			try {
				MailAddress from = new MailAddress(user + "@" + domain, "LoyaltyViewer");
				MailAddress to = new MailAddress(toAddress);
				
				using (MailMessage message = new MailMessage(from, to)) {
					message.Subject = subject;
					message.Body = opening + body + ending;
					message.CC.Add(copy);

					SmtpClient client = new SmtpClient(server, 25);
					client.UseDefaultCredentials = false;
					client.Credentials = new System.Net.NetworkCredential(user, password, domain);
					client.Send(message);
				}
			} catch (Exception e) {
				LoggingSystem.LogMessageToFile("Не удалось отправить письмо: " + 
					e.StackTrace + " " + e.Message);
			}
		}
	}
}
