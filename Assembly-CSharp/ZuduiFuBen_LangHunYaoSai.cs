using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;

public static class ZuduiFuBen_LangHunYaoSai
{
	public static int WaveAmount
	{
		get
		{
			ZuduiFuBen_LangHunYaoSai.InitLangHunYaoSaiMonsterXmlDataDict();
			return ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.Count;
		}
	}

	public static ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData GetWaveData(int nWaveID)
	{
		ZuduiFuBen_LangHunYaoSai.InitLangHunYaoSaiMonsterXmlDataDict();
		if (ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.Count <= 0)
		{
			return null;
		}
		ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData result;
		if (ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.TryGetValue(nWaveID, ref result))
		{
			return result;
		}
		return null;
	}

	public static long EndTime
	{
		get
		{
			return ZuduiFuBen_LangHunYaoSai.endTime;
		}
	}

	public static int FortLifeNow
	{
		get
		{
			return ZuduiFuBen_LangHunYaoSai.fortLifeNow;
		}
	}

	public static void RecvCMD_LangHunScoreInfo(MUSocketConnectEventArgs e)
	{
		if (e == null)
		{
			return;
		}
		CopyWolfScoreData copyWolfScoreData = DataHelper.BytesToObject<CopyWolfScoreData>(e.bytesData, 0, e.bytesData.Length);
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (playZone != null && copyWolfScoreData != null)
		{
			if (ZuduiFuBen_LangHunYaoSai.thisWave != copyWolfScoreData.Wave)
			{
				ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData waveData = ZuduiFuBen_LangHunYaoSai.GetWaveData(copyWolfScoreData.Wave);
				if (waveData != null)
				{
					SystemHelpPart.ShowHintTextNoTarget(true, waveData.Intro, 3);
				}
			}
			ZuduiFuBen_LangHunYaoSai.endTime = copyWolfScoreData.EndTime;
			ZuduiFuBen_LangHunYaoSai.thisWave = copyWolfScoreData.Wave;
			ZuduiFuBen_LangHunYaoSai.fortLifeNow = copyWolfScoreData.FortLifeNow;
			if (playZone.GameTaskBoxMini != null)
			{
				int waveAmount = ZuduiFuBen_LangHunYaoSai.WaveAmount;
				int num = 0;
				if (Global.Data.roleData != null && copyWolfScoreData.RoleMonsterScore.ContainsKey(Global.Data.roleData.RoleID))
				{
					copyWolfScoreData.RoleMonsterScore.TryGetValue(Global.Data.roleData.RoleID, ref num);
				}
				string text = playZone.GameTaskBoxMini.SceneInfos[0].text;
				playZone.GameTaskBoxMini.SetSceneTaskInfos(0, string.Concat(new object[]
				{
					Global.GetLang("当前波数   "),
					copyWolfScoreData.Wave,
					"/",
					waveAmount
				}), new object[0]);
				playZone.GameTaskBoxMini.SetSceneTaskInfos(2, Global.GetLang("个人积分   ") + num, new object[0]);
				playZone.GameTaskBoxMini.SetProcessBarForGroundImg("bar_green");
				int num2 = 100;
				if (copyWolfScoreData.FortLifeMax > 0)
				{
					num2 = copyWolfScoreData.FortLifeNow * 100 / copyWolfScoreData.FortLifeMax;
				}
				string text2 = string.Concat(new object[]
				{
					Global.GetLang("狼魂要塞生命 "),
					copyWolfScoreData.FortLifeNow,
					"/",
					copyWolfScoreData.FortLifeMax
				});
				if (num2 > 80)
				{
					text2 = Global.GetColorStringForNGUIText(new object[]
					{
						"0ce82e",
						text2
					});
					playZone.GameTaskBoxMini.SetProcessBarForGroundImg("bar_green");
				}
				else if (num2 >= 50)
				{
					text2 = Global.GetColorStringForNGUIText(new object[]
					{
						"eefe12",
						text2
					});
					playZone.GameTaskBoxMini.SetProcessBarForGroundImg("bar_yellow");
				}
				else if (num2 >= 20)
				{
					text2 = Global.GetColorStringForNGUIText(new object[]
					{
						"ed8711",
						text2
					});
					playZone.GameTaskBoxMini.SetProcessBarForGroundImg("bar_orange");
				}
				else
				{
					text2 = Global.GetColorStringForNGUIText(new object[]
					{
						"e23a3a",
						text2
					});
					playZone.GameTaskBoxMini.SetProcessBarForGroundImg("bar_red");
				}
				playZone.GameTaskBoxMini.SetSceneTaskInfos(3, text2, new object[0]);
				playZone.GameTaskBoxMini.SetProcessBarPercent((float)num2);
				ZuduiFuBen_LangHunYaoSai.UpdateUI();
			}
		}
	}

	public static void UpdateUI()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		if (playZone.GameTaskBoxMini == null)
		{
			return;
		}
		DateTime dateTime;
		dateTime..ctor(ZuduiFuBen_LangHunYaoSai.EndTime * 10000L);
		DateTime dateTime2;
		dateTime2..ctor(Global.GetCorrectLocalTime() * 10000L);
		if (ZuduiFuBen_LangHunYaoSai.EndTime > 0L && dateTime > dateTime2)
		{
			int minutes = (dateTime - dateTime2).Minutes;
			int seconds = (dateTime - dateTime2).Seconds;
			if (minutes > 0)
			{
				playZone.GameTaskBoxMini.SetSceneTaskInfos(1, string.Concat(new string[]
				{
					Global.GetLang("下一波时间 "),
					minutes.ToString(),
					Global.GetLang(" 分 "),
					seconds.ToString(),
					Global.GetLang(" 秒")
				}), new object[0]);
			}
			else
			{
				playZone.GameTaskBoxMini.SetSceneTaskInfos(1, Global.GetLang("下一波时间 ") + seconds.ToString() + Global.GetLang(" 秒"), new object[0]);
			}
		}
	}

	public static void ClearXMLData()
	{
		if (ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict != null && 0 < ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.Count)
		{
			ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.Clear();
		}
	}

	private static void InitLangHunYaoSaiMonsterXmlDataDict()
	{
		if (ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict != null)
		{
			return;
		}
		ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict = new Dictionary<int, ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData>();
		XElement gameResXml = Global.GetGameResXml("Config/LangHunYaoSai.xml");
		if (gameResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(gameResXml, "Config");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "LangHunYaoSaiMonsters");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData langHunYaoSaiMonsterXmlData = new ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData();
			langHunYaoSaiMonsterXmlData.ID = Global.GetXElementAttributeInt(xelement2, "ID");
			langHunYaoSaiMonsterXmlData.MonstersID = Global.GetXElementAttributeStr(xelement2, "MonstersID");
			langHunYaoSaiMonsterXmlData.NextTime = Global.GetXElementAttributeInt(xelement2, "NextTime");
			langHunYaoSaiMonsterXmlData.Site = Global.GetXElementAttributeStr(xelement2, "Site");
			langHunYaoSaiMonsterXmlData.Intro = Global.GetXElementAttributeStr(xelement2, "Intro");
			int id = langHunYaoSaiMonsterXmlData.ID;
			if (!ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.ContainsKey(id))
			{
				ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlDataDict.Add(id, langHunYaoSaiMonsterXmlData);
			}
		}
	}

	private static long endTime;

	private static int thisWave;

	private static int fortLifeNow;

	private static Dictionary<int, ZuduiFuBen_LangHunYaoSai.LangHunYaoSaiMonsterXmlData> LangHunYaoSaiMonsterXmlDataDict;

	public class LangHunYaoSaiMonsterXmlData
	{
		public int ID;

		public string MonstersID;

		public int NextTime;

		public string Site;

		public string Intro;
	}
}
