using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanBuffXML
{
	public TeQuanBuffXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanBuff.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanBuff");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanBuffVO teQuanBuffVO = new TeQuanBuffVO();
				teQuanBuffVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanBuffVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanBuff.xml没有拿到"
			});
		}
	}

	internal BetterList<TeQuanBuffVO> GetTeQuanBuffVOItemsBuyTeQuanID(int TeQuanID)
	{
		BetterList<TeQuanBuffVO> betterList = new BetterList<TeQuanBuffVO>();
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i] != null && TeQuanID == this.mDataList[i].TeQuanID)
			{
				betterList.Add(this.mDataList[i]);
			}
		}
		return betterList;
	}

	private const string Path = "GameRes/Config/TeQuanBuff.xml";

	private BetterList<TeQuanBuffVO> mDataList = new BetterList<TeQuanBuffVO>();
}
