using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RebornBossXML
{
	public RebornBossXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornBoss.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "RebornBoss");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornBossVO rebornBossVO = new RebornBossVO();
				rebornBossVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				rebornBossVO.MapID = Global.GetXElementAttributeInt(xelementList[i], "MapID");
				rebornBossVO.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
				rebornBossVO.RebornLevel = Global.GetXElementAttributeInt(xelementList[i], "RebornLevel");
				rebornBossVO.ZhanLi = Global.GetXElementAttributeInt(xelementList[i], "ZhanLi");
				rebornBossVO.Scale = Global.GetXElementAttributeDouble(xelementList[i], "Scale");
				rebornBossVO.Site = Global.GetXElementAttributeStr(xelementList[i], "Site");
				this.mDataList.Add(rebornBossVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornBoss.xml没有拿到"
			});
		}
	}

	public RebornBossVO GetItemByID(int ID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (ID == this.mDataList[i].ID)
			{
				return this.mDataList[i];
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"RebornBoss ID = " + ID + "没有找到"
		});
		return null;
	}

	public RebornBossVO GetItemByMonsterID(int MonsterID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (MonsterID == this.mDataList[i].MonstersID)
			{
				return this.mDataList[i];
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"RebornBoss ID = " + MonsterID + "没有找到"
		});
		return null;
	}

	public BetterList<RebornBossVO> GetAllItem()
	{
		return this.mDataList;
	}

	private const string Path = "GameRes/Config/RebornBoss.xml";

	private BetterList<RebornBossVO> mDataList = new BetterList<RebornBossVO>();
}
