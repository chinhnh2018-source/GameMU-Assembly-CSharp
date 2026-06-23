using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;

namespace HSGameEngine.GameFramework.Logic
{
	public class TengXunWordsFilter
	{
		public TengXunWordsFilter()
		{
			if (TengXunWordsFilter.BaseHttpURL.Length <= 0)
			{
				string text = Super.GetXapParamByName("serverip", string.Empty);
				text = Global.StringReplaceAll(text, "http://", string.Empty);
				TengXunWordsFilter.BaseHttpURL = StringUtil.substitute("http://{0}/{1}", new object[]
				{
					text,
					Super.GetXapParamByName("tx_wordfilterurl", "http://113.108.20.23/v3/csec/word_filter")
				});
			}
		}

		private static string GetUniqueMsgID(string openid)
		{
			TengXunWordsFilter.RequestCounter++;
			return openid + default(DateTime).Second.ToString() + TengXunWordsFilter.RequestCounter.ToString();
		}

		public void SendRequest(string content, string opuid = "", string touid = "")
		{
			this._TextToFilter = content;
			string xapParamByName = Super.GetXapParamByName("tx_openid", string.Empty);
			string xapParamByName2 = Super.GetXapParamByName("tx_openkey", string.Empty);
			string xapParamByName3 = Super.GetXapParamByName("tx_pf", string.Empty);
			string uniqueMsgID = TengXunWordsFilter.GetUniqueMsgID(xapParamByName);
			string text = StringUtil.substitute("{0}?pf={1}&content={2}&msgid={3}&openkey={4}&openid={5}", new object[]
			{
				TengXunWordsFilter.BaseHttpURL,
				xapParamByName3,
				content,
				uniqueMsgID,
				xapParamByName2,
				xapParamByName
			});
			this.downloader = new Downloader(null);
			this.downloader.Completed = new DownloaderEventHander(this.DownLoaderComplete);
		}

		public void DownLoaderComplete(object sender, DownloadEventArgs e)
		{
			this.downloader.Completed = null;
			this.downloader = null;
		}

		private static object StrToObject(string s)
		{
			try
			{
				return null;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return null;
		}

		public static string BaseHttpURL = string.Empty;

		public static int RequestCounter;

		private Downloader downloader;

		public CheckWordsResultEventHandler TengXunReturn;

		private string _TextToFilter = string.Empty;
	}
}
