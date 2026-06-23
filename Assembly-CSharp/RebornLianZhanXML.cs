using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RebornLianZhanXML
{
	public RebornLianZhanXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornLianZhan.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "RebornLianZhan");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornLianZhanVO rebornLianZhanVO = new RebornLianZhanVO();
				rebornLianZhanVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				rebornLianZhanVO.Num = Global.GetXElementAttributeInt(xelementList[i], "Num");
				rebornLianZhanVO.RebornExp = Global.GetXElementAttributeInt(xelementList[i], "RebornExp");
				rebornLianZhanVO.Time = Global.GetXElementAttributeInt(xelementList[i], "Time");
				rebornLianZhanVO.GoodsID = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
				this.mDataList.Add(rebornLianZhanVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornLianZhan.xml没有拿到"
			});
		}
	}

	public RebornLianZhanVO GetItemByID(int ID)
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

	internal void ClearData()
	{
		this.mDataList.Release();
	}

	private const string Path = "GameRes/Config/RebornLianZhan.xml";

	private BetterList<RebornLianZhanVO> mDataList = new BetterList<RebornLianZhanVO>();
}
