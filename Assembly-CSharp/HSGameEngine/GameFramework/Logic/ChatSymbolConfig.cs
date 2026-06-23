using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

namespace HSGameEngine.GameFramework.Logic
{
	public class ChatSymbolConfig
	{
		public static ChatSymbolConfig Singleton
		{
			get
			{
				if (ChatSymbolConfig.m_singleton == null)
				{
					ChatSymbolConfig.m_singleton = new ChatSymbolConfig();
				}
				return ChatSymbolConfig.m_singleton;
			}
		}

		public void PrepareChatSymbolData()
		{
			string gameResTxt = Global.GetGameResTxt(string.Format("ChatSymbol", new object[0]));
			this.ParseChatSymbolList(gameResTxt);
		}

		private void ParseChatSymbolList(string content)
		{
			this.ChatSymbolDict.Clear();
			try
			{
				string[] array = content.Split(new char[]
				{
					'\n',
					'\r'
				}, 1);
				char[] array2 = new char[]
				{
					','
				};
				for (int i = 1; i < array.Length; i++)
				{
					string[] array3 = array[i].Split(array2);
					ChatSymbol chatSymbol = new ChatSymbol();
					chatSymbol.Id = uint.Parse(array3[0]);
					chatSymbol.ShowName = array3[1];
					chatSymbol.ImageName = array3[2];
					chatSymbol.IsAnimation = (int.Parse(array3[3]) > 0);
					if (chatSymbol.IsAnimation)
					{
						chatSymbol.AnimationFPS = int.Parse(array3[4]);
					}
					this.ChatSymbolDict.Add(chatSymbol.Id, chatSymbol);
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public ChatSymbol GetChatSymbol(string showName)
		{
			ChatSymbol result = null;
			foreach (KeyValuePair<uint, ChatSymbol> keyValuePair in this.ChatSymbolDict)
			{
				if (keyValuePair.Value.ShowName == showName)
				{
					result = keyValuePair.Value;
					break;
				}
			}
			return result;
		}

		public string FormatColorToHtml(string content)
		{
			string text = content;
			string text2 = string.Empty;
			if (content.StartsWith("{"))
			{
				int num = content.IndexOf("}");
				if (num > -1)
				{
					text2 = content.Substring(1, num - 1);
					if (text2.Length == 6)
					{
						string text3 = string.Format("<font color=#{0}>", text2);
						text = text3 + text.Substring(num + 1) + "</font>";
					}
				}
			}
			return text;
		}

		public string FormatToHtml(string content)
		{
			string text = content;
			List<string> symbolsList = this.GetSymbolsList(text);
			for (int i = 0; i < symbolsList.Count; i++)
			{
				string text2 = symbolsList[i];
				ChatSymbol chatSymbol = this.GetChatSymbol(text2);
				if (chatSymbol != null)
				{
					text = text.Replace(text2, chatSymbol.GetHtmlStr());
				}
			}
			return text;
		}

		public string FormatSourceForSize(string content, float imageWidth, float perCharWidth)
		{
			string text = content;
			string text2 = string.Empty;
			if (content.StartsWith("{"))
			{
				int num = content.IndexOf("}");
				if (num > -1)
				{
					text2 = content.Substring(0, num + 1);
					if (text2.Length == 8)
					{
						text = content.Substring(8);
					}
				}
			}
			List<string> symbolsList = this.GetSymbolsList(text);
			float num2 = 0f;
			if (perCharWidth != 0f)
			{
				num2 = imageWidth / perCharWidth;
			}
			string text3 = string.Empty;
			while (num2 > 0f)
			{
				text3 += " ";
				num2 -= 1f;
			}
			for (int i = 0; i < symbolsList.Count; i++)
			{
				string text4 = symbolsList[i];
				ChatSymbol chatSymbol = this.GetChatSymbol(text4);
				if (chatSymbol != null)
				{
					text = text.Replace(text4, text3);
				}
			}
			return text;
		}

		private List<string> GetSymbolsList(string content)
		{
			List<string> list = new List<string>();
			int num = content.IndexOf("［");
			int num2 = -1;
			if (num + 1 < content.Length)
			{
				num2 = content.IndexOf("］", num + 1);
			}
			while (num > -1 && num2 > -1 && !string.IsNullOrEmpty(content.Substring(num, num2 - num + 1)))
			{
				list.Add(content.Substring(num, num2 - num + 1));
				num = -1;
				if (num2 + 1 < content.Length)
				{
					num = content.IndexOf("［", num2 + 1);
					num2 = -1;
					if (num + 1 < content.Length)
					{
						num2 = content.IndexOf("］", num + 1);
					}
				}
			}
			return list;
		}

		private const string StartChar = "［";

		private const string EndChar = "］";

		private static ChatSymbolConfig m_singleton;

		public Dictionary<uint, ChatSymbol> ChatSymbolDict = new Dictionary<uint, ChatSymbol>();
	}
}
