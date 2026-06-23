using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class RebornStageXML
{
	public RebornStageXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornStage.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "RebornStage");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornStageVO rebornStageVO = new RebornStageVO(xelementList[i]);
				this.mDataList.Add(rebornStageVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornStage.xml没有拿到"
			});
		}
	}

	public int GeiMaxStage()
	{
		int num = 0;
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (num < this.mDataList[i].ID)
			{
				num = this.mDataList[i].ID;
			}
		}
		return num;
	}

	public RebornStageVO GetItemByID(int ID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (ID == this.mDataList[i].ID)
			{
				return this.mDataList[i];
			}
		}
		return null;
	}

	public List<GoodsData> GetAwardGoodsByID(int ID)
	{
		RebornStageVO itemByID = this.GetItemByID(ID);
		if (itemByID != null)
		{
			return itemByID.AwardShowGoodsGoodsDataList;
		}
		return null;
	}

	public List<GoodsData> GetModalShowGoodsDataListByID(int ID)
	{
		RebornStageVO itemByID = this.GetItemByID(ID);
		if (itemByID != null)
		{
			return itemByID.ShowModalGoodsDataList;
		}
		return null;
	}

	public int[] GetRoleRebirthNeedZhuanShengByRoleRebirthLevel(int RebirthCount)
	{
		RebornStageVO itemByID = this.GetItemByID(RebirthCount);
		if (itemByID != null)
		{
			return itemByID.NeedZhuanShengInf;
		}
		return null;
	}

	public void ClearData()
	{
		this.mDataList.Release();
	}

	private const string Path = "GameRes/Config/RebornStage.xml";

	private BetterList<RebornStageVO> mDataList = new BetterList<RebornStageVO>();
}
