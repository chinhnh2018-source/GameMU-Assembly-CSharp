using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class RebornPerBossXML
{
	public RebornPerBossXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornPerBoss.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "RebornPerBoss");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornPerBossVO rebornPerBossVO = new RebornPerBossVO();
				rebornPerBossVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				rebornPerBossVO.MapID = Global.GetXElementAttributeInt(xelementList[i], "MapID");
				rebornPerBossVO.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
				rebornPerBossVO.RebornLevel = Global.GetXElementAttributeInt(xelementList[i], "RebornLevel");
				rebornPerBossVO.ZhanLi = Global.GetXElementAttributeInt(xelementList[i], "ZhanLi");
				rebornPerBossVO.Scale = Global.GetXElementAttributeDouble(xelementList[i], "Scale");
				rebornPerBossVO.Site = Global.GetXElementAttributeStr(xelementList[i], "Site");
				rebornPerBossVO.FreeChallengeNum = Global.GetXElementAttributeInt(xelementList[i], "FreeChallengeNum");
				rebornPerBossVO.PayChallengeNum = Global.GetXElementAttributeStr(xelementList[i], "PayChallengeNum");
				rebornPerBossVO.PerfectGoodsOne = Global.GetXElementAttributeStr(xelementList[i], "PerfectGoodsOne");
				rebornPerBossVO.UnPerfectGoodsOne = Global.GetXElementAttributeStr(xelementList[i], "UnPerfectGoodsOne");
				rebornPerBossVO.FailGoodsOne = Global.GetXElementAttributeStr(xelementList[i], "FailGoodsOne");
				rebornPerBossVO.GoodsTwo = Global.GetXElementAttributeStr(xelementList[i], "GoodsTwo");
				rebornPerBossVO.MoppingConditions = Global.GetXElementAttributeInt(xelementList[i], "MoppingConditions");
				rebornPerBossVO.ChallengeReward = Global.GetXElementAttributeStr(xelementList[i], "ChallengeReward");
				this.mDataList.Add(rebornPerBossVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornPerBoss.xml没有拿到"
			});
		}
	}

	public RebornPerBossVO GetItemByID(int ID)
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

	public RebornPerBossVO GetItemByMonsterID(int MonsterID)
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

	public int GetPayChallegeNum(int MonsterID, int Num)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (MonsterID == this.mDataList[i].MonstersID)
			{
				return this.mDataList[i].PaychallgeNum(Num);
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"RebornBoss ID = " + MonsterID + "没有找到"
		});
		return 0;
	}

	public BetterList<RebornPerBossVO> GetAllItem()
	{
		return this.mDataList;
	}

	public BetterList<GoodsData> GetAwardGoodsDatas(int MonsterID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (MonsterID == this.mDataList[i].MonstersID)
			{
				return this.mDataList[i].GetAwardGoodsDatas();
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"RebornBoss ID = " + MonsterID + "没有找到"
		});
		return null;
	}

	public int GetMoppingConditions(int MonsterID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (MonsterID == this.mDataList[i].MonstersID)
			{
				return this.mDataList[i].MoppingConditions;
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"RebornBoss ID = " + MonsterID + "没有找到"
		});
		return 0;
	}

	internal int GetDayMaXEnterNum(int MonsterID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (MonsterID == this.mDataList[i].MonstersID)
			{
				return this.mDataList[i].DayMaxEnterNum;
			}
		}
		MUDebug.LogError<string>(new string[]
		{
			"RebornBoss ID = " + MonsterID + "没有找到"
		});
		return 0;
	}

	private const string Path = "GameRes/Config/RebornPerBoss.xml";

	private BetterList<RebornPerBossVO> mDataList = new BetterList<RebornPerBossVO>();
}
