using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RebornBossAwardXML
{
	public RebornBossAwardXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornBossAward.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "RebornBossAward");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornBossAwardVO rebornBossAwardVO = new RebornBossAwardVO();
				rebornBossAwardVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				rebornBossAwardVO.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
				rebornBossAwardVO.BeginNum = Global.GetXElementAttributeInt(xelementList[i], "BeginNum");
				rebornBossAwardVO.EndNum = Global.GetXElementAttributeInt(xelementList[i], "EndNum");
				rebornBossAwardVO.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
				rebornBossAwardVO.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
				rebornBossAwardVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
				rebornBossAwardVO.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
				this.mDataList.Add(rebornBossAwardVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornBossAward.xml没有拿到"
			});
		}
	}

	public RebornBossAwardVO GetItemByID(int ID)
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
			"RebornLianZhan ID = " + ID + "没有找到"
		});
		return null;
	}

	public List<RebornBossAwardVO> GetAwardsByMonsterID(int ID, bool CheckType = false)
	{
		List<RebornBossAwardVO> list = new List<RebornBossAwardVO>();
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (ID == this.mDataList[i].MonstersID)
			{
				if (CheckType)
				{
					if (this.mDataList[i].Type == 0)
					{
						list.Add(this.mDataList[i]);
					}
				}
				else
				{
					list.Add(this.mDataList[i]);
				}
			}
		}
		return list;
	}

	public RebornBossAwardVO GetRebornBossAwardVOLastAttAward(int BossID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (BossID == this.mDataList[i].MonstersID && this.mDataList[i].Type == 1)
			{
				return this.mDataList[i];
			}
		}
		return null;
	}

	private const string Path = "GameRes/Config/RebornBossAward.xml";

	private BetterList<RebornBossAwardVO> mDataList = new BetterList<RebornBossAwardVO>();
}
