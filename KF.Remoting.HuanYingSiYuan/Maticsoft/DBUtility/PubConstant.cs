using System;
using System.Configuration;
using Server.Tools;

namespace Maticsoft.DBUtility
{
	public class PubConstant
	{
		public static string ConnectionString
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ConnectionString"];
				if (PubConstant._CSE != text)
				{
					PubConstant._CSE = text;
					string a = ConfigurationManager.AppSettings["ConStringEncrypt"];
					if (a != "false")
					{
						text = StringEncrypt.Decrypt(text, "eabcix675u49,/", "3&3i4x4^+-0");
					}
					PubConstant._CS = text;
				}
				return PubConstant._CS;
			}
		}

		public static string ConnectionLogString
		{
			get
			{
				return ConfigurationManager.AppSettings["ConnectionLogString"];
			}
		}

		public static string GetDatabaseName(string dbKey)
		{
			return ConfigurationManager.AppSettings[dbKey];
		}

		public static string GetConnectionString(string configName)
		{
			string text = ConfigurationManager.AppSettings[configName];
			string a = ConfigurationManager.AppSettings["ConStringEncrypt"];
			if (a == "true")
			{
				text = DESEncrypt.Decrypt(text);
			}
			return text;
		}

		private static string _CSE;

		private static string _CS;
	}
}
