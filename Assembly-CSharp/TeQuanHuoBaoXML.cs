using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanHuoBaoXML
{
	public TeQuanHuoBaoXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanHuoBao.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanHuoBao");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanHuoBaoVO teQuanHuoBaoVO = new TeQuanHuoBaoVO();
				teQuanHuoBaoVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanHuoBaoVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanHuoBao.xml没有拿到"
			});
		}
	}

	internal BetterList<TeQuanHuoBaoVO> GetShangChengVOItemsBuyTeQuanID(int TeQuanID)
	{
		BetterList<TeQuanHuoBaoVO> betterList = new BetterList<TeQuanHuoBaoVO>();
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i] != null && TeQuanID == this.mDataList[i].TeQuanID)
			{
				betterList.Add(this.mDataList[i]);
			}
		}
		return betterList;
	}

	private const string Path = "GameRes/Config/TeQuanHuoBao.xml";

	private BetterList<TeQuanHuoBaoVO> mDataList = new BetterList<TeQuanHuoBaoVO>();
}
