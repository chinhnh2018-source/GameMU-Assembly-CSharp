using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class RebornIntroBossXML
{
	public RebornIntroBossXML()
	{
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornIntroBoss.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
			for (int i = 0; i < xelementList.Count; i++)
			{
				RebornIntroVO rebornIntroVO = new RebornIntroVO();
				rebornIntroVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				rebornIntroVO.Bold = Global.GetXElementAttributeInt(xelementList[i], "Bold");
				rebornIntroVO.Intro = Global.GetXElementAttributeStr(xelementList[i], "Intro");
				this.mDataList.Add(rebornIntroVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/RebornIntroBoss.xml没有拿到"
			});
		}
	}

	public RebornIntroVO GetItemByID(int ID)
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
			"RebornIntroVO ID = " + ID + "没有找到"
		});
		return null;
	}

	public bool GetItemBoldByID(int ID)
	{
		RebornIntroVO itemByID = this.GetItemByID(ID);
		return itemByID != null && 1 == itemByID.Bold;
	}

	public string GetTitle()
	{
		if (0 < this.mDataList.size)
		{
			return this.mDataList[0].Intro;
		}
		return string.Empty;
	}

	public string GetHelpContent()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 1; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i] != null && !string.IsNullOrEmpty(this.mDataList[i].Intro))
			{
				stringBuilder.AppendLine(this.mDataList[i].Intro);
			}
		}
		return stringBuilder.ToString();
	}

	public const string Path = "GameRes/Config/RebornIntroBoss.xml";

	private BetterList<RebornIntroVO> mDataList = new BetterList<RebornIntroVO>();
}
