using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using UnityEngine;

public class SendMailUtil
{
	public static void StartSendServer()
	{
		string server = "smtp.exmail.qq.com";
		string text = "yunweitools@tmsk.cn";
		string password = "tmsk201411";
		string to = "shenzx@tmsk.cn";
		string subject = "哈哈，这是程序发送";
		string body = "嘿嘿Unity,这是程序发送。最终结果，还是不要发送邮件了，会带来安装包的加大。程序启动变慢。";
		SendMailUtil.SendMailFromServer(text, "yunweitools", to, subject, body, text, password, server, string.Empty);
	}

	public static string SendMailFromServer(string from, string fromname, string to, string subject, string body, string username, string password, string server, string fujian)
	{
		string result;
		try
		{
			MailMessage mailMessage = new MailMessage();
			mailMessage.From = new MailAddress(from, fromname);
			mailMessage.To.Add(to);
			mailMessage.Subject = subject;
			mailMessage.Body = body;
			mailMessage.BodyEncoding = Encoding.UTF8;
			mailMessage.DeliveryNotificationOptions = 1;
			mailMessage.Priority = 2;
			mailMessage.IsBodyHtml = true;
			if (fujian.Length > 0)
			{
				mailMessage.Attachments.Add(new Attachment(fujian));
			}
			SmtpClient smtpClient = new SmtpClient();
			ICredentialsByHost credentialsByHost = new NetworkCredential(username, password) as ICredentialsByHost;
			if (credentialsByHost == null)
			{
				Debug.Log("tempHost == null");
			}
			smtpClient.Credentials = credentialsByHost;
			smtpClient.Host = server;
			smtpClient.DeliveryMethod = 0;
			smtpClient.Timeout = 10000;
			smtpClient.Send(mailMessage);
			result = "send ok";
		}
		catch (SmtpException ex)
		{
			result = ex.Message;
		}
		return result;
	}
}
