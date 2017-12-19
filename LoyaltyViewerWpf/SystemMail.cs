using System.Net.Mail;
using System;
using System.Reflection;

namespace LoyaltyViewerWpf {
	class SystemMail {
		public static void SendMail(string body) {
			SystemLogging.LogMessageToFile("Отправка e-mail с текстом: " + body);

			string toAddress = Properties.Settings.Default.IsDebug ?
							   Properties.Settings.Default.MailCopy : Properties.Settings.Default.MailTo;
			string copy = Properties.Settings.Default.MailCopy;
			string subject = "Уведомление от " + Assembly.GetExecutingAssembly().GetName().Name;
			string server = Properties.Settings.Default.MailServer;
			string user = Properties.Settings.Default.MailUser;
			string password = Properties.Settings.Default.MailPassword;
			string domain = Properties.Settings.Default.MailDomain;
			string opening = "На группу поддержки бизнес-приложений: " + Environment.NewLine;
			string ending = "\n\nЭто автоматически сгенерированное сообщение\n" +
							"Просьба не отвечать на него\n" +
							"Имя системы: " + Environment.MachineName;

			try {
				MailAddress from = new MailAddress(user + "@" + domain, Assembly.GetExecutingAssembly().GetName().Name);
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
				SystemLogging.LogMessageToFile("Не удалось отправить письмо: " + 
					e.StackTrace + " " + e.Message);
			}
		}
	}
}
