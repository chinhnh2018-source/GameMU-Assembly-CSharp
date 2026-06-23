using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Logic
{
	public class GGameInfocs
	{
		public static event EventHandler NotifyGameInfoMessage;

		private static void CleanMessage()
		{
			if (GGameInfocs.GameInfoTextList.Count < 100)
			{
				return;
			}
			GGameInfocs.GameInfoTextList.RemoveAt(0);
		}

		public static void ClearEvents()
		{
			GGameInfocs.NotifyGameInfoMessage = null;
		}

		public static void AddGameInfoMessage(GameInfoTypeIndexes index, ShowGameInfoTypes ShowGameInfoType, string textMsg, int errCode = 0, int toX = -1, int toY = -1, int toBuyGoodsID = 0)
		{
			if (textMsg.StartsWith(Global.GetLang("当前时间段恶魔广场并未开启")))
			{
				textMsg += DateTime.Now.ToLongTimeString();
			}
			GGameInfocs.CleanMessage();
			long correctLocalTime = Global.GetCorrectLocalTime();
			GameInfoTextItem gameInfoTextItem = new GameInfoTextItem();
			gameInfoTextItem.TextMsg = textMsg;
			gameInfoTextItem.Ticks = correctLocalTime;
			gameInfoTextItem.GameInfoTypeIndex = index;
			gameInfoTextItem.ShowGameInfoType = ShowGameInfoType;
			gameInfoTextItem.ErrCode = errCode;
			gameInfoTextItem.ToX = toX;
			gameInfoTextItem.ToY = toY;
			gameInfoTextItem.ToBuyGoodsID = toBuyGoodsID;
			GGameInfocs.GameInfoTextList.Add(gameInfoTextItem);
			if (GGameInfocs.NotifyGameInfoMessage != null)
			{
				GGameInfocs.NotifyGameInfoMessage.Invoke(null, EventArgs.Empty);
			}
		}

		public static List<GameInfoTextItem> GameInfoTextList = new List<GameInfoTextItem>(100);
	}
}
