using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

namespace HSGameEngine.GameFramework.Logic
{
	public class AutoSystemChatMgr
	{
		public static void LoadAutoSystemChatItems()
		{
			XElement gameResXml = Global.GetGameResXml("Config/AutoSystemChat.xml");
			if (gameResXml == null)
			{
				return;
			}
			string text = string.Empty;
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Content");
					if (Global.Data.roleData.ChangeLifeCount >= xelementAttributeInt && Global.Data.roleData.ChangeLifeCount <= xelementAttributeInt2 && Global.Data.roleData.Level >= xelementAttributeInt3 && Global.Data.roleData.Level <= xelementAttributeInt4)
					{
						text = text + xelementAttributeStr + "|";
					}
				}
			}
			Super.AutoSystemChatItemsArray = text.Trim(new char[]
			{
				'|'
			}).Split(new char[]
			{
				'|'
			});
		}

		public static void BroadcastLocalSystemChatMessage(ChatBox chatBox)
		{
		}
	}
}
