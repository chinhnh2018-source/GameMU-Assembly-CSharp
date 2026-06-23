using System;
using System.Collections.Generic;
using System.IO;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class GongGaoDataManager
	{
		public static void LoadGongGaoData()
		{
			string path = Global.IsolateResPath("Config/Gonggao.xml");
			GongGaoDataManager.strGongGaoXML = File.ReadAllText(path);
			GongGaoDataManager.systemGongGaoMgr.LoadFromXMlFile("Config/Gonggao.xml", "", "ID", 1);
		}

		public static void CheckGongGaoInfo(GameClient client, int nID)
		{
			string strB = "";
			string strB2 = "";
			using (Dictionary<int, SystemXmlItem>.ValueCollection.Enumerator enumerator = GongGaoDataManager.systemGongGaoMgr.SystemXmlItemDict.Values.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					SystemXmlItem systemXmlItem = enumerator.Current;
					strB = systemXmlItem.GetStringValue("FromDate");
					strB2 = systemXmlItem.GetStringValue("ToDate");
				}
			}
			int num = 0;
			string strA = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
			if (string.Compare(strA, strB) >= 0 && string.Compare(strA, strB2) <= 0)
			{
				num = 1;
			}
			int nLianXuLoginReward = 0;
			int nLeiJiLoginReward = 0;
			if (client._IconStateMgr.CheckFuLiLianXuDengLuReward(client))
			{
				nLianXuLoginReward = 1;
			}
			if (client._IconStateMgr.CheckFuLiLeiJiDengLuReward(client))
			{
				nLeiJiLoginReward = 1;
			}
			GongGaoData gongGaoData = new GongGaoData();
			if (1 == num)
			{
				gongGaoData.strGongGaoInfo = GongGaoDataManager.strGongGaoXML;
			}
			gongGaoData.nHaveGongGao = num;
			gongGaoData.nLianXuLoginReward = nLianXuLoginReward;
			gongGaoData.nLeiJiLoginReward = nLeiJiLoginReward;
			client.sendCmd<GongGaoData>(nID, gongGaoData, false);
		}

		public static SystemXmlItems systemGongGaoMgr = new SystemXmlItems();

		public static string strGongGaoXML = "";
	}
}
