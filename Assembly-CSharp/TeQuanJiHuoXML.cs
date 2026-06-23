using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanJiHuoXML
{
	public TeQuanJiHuoXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanJiHuo.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanJiHuo");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanJiHuoVO teQuanJiHuoVO = new TeQuanJiHuoVO();
				teQuanJiHuoVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanJiHuoVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanJiHuo.xml没有拿到"
			});
		}
	}

	public TeQuanJiHuoVO GetTeQuanJiHuoVOByID(int ID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i].ID == ID)
			{
				return this.mDataList[i];
			}
		}
		return null;
	}

	private const string Path = "GameRes/Config/TeQuanJiHuo.xml";

	private BetterList<TeQuanJiHuoVO> mDataList = new BetterList<TeQuanJiHuoVO>();
}
