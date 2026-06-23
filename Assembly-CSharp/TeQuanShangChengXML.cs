using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanShangChengXML
{
	public TeQuanShangChengXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanShangCheng.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanShangCheng");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanShangChengVO teQuanShangChengVO = new TeQuanShangChengVO();
				teQuanShangChengVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanShangChengVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanShangCheng.xml没有拿到"
			});
		}
	}

	internal BetterList<TeQuanShangChengVO> GetShangChengVOItemsBuyTeQuanID(int TeQuanID)
	{
		BetterList<TeQuanShangChengVO> betterList = new BetterList<TeQuanShangChengVO>();
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i] != null && TeQuanID == this.mDataList[i].TeQuanID)
			{
				betterList.Add(this.mDataList[i]);
			}
		}
		return betterList;
	}

	private const string Path = "GameRes/Config/TeQuanShangCheng.xml";

	private BetterList<TeQuanShangChengVO> mDataList = new BetterList<TeQuanShangChengVO>();
}
