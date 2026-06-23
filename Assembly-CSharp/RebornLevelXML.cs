using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RebornLevelXML
{
	public RebornLevelXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornLevel.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "RebornLevel");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornLevelVO rebornLevelVO = new RebornLevelVO(xelementList[i]);
				this.mDataList.Add(rebornLevelVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornLevel.xml没有拿到"
			});
		}
	}

	public RebornLevelVO GetItemByID(int ID)
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

	public void ClearData()
	{
		this.mDataList.Release();
	}

	public int GetMaxLevel()
	{
		int num = 0;
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i].ID > num)
			{
				num = this.mDataList[i].ID;
			}
		}
		return num;
	}

	private const string Path = "GameRes/Config/RebornLevel.xml";

	private BetterList<RebornLevelVO> mDataList = new BetterList<RebornLevelVO>();
}
