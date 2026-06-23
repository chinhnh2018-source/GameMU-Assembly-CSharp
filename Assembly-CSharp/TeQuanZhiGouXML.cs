using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanZhiGouXML
{
	public TeQuanZhiGouXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanZhiGou.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanZhiGou");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanZhiGouVO teQuanZhiGouVO = new TeQuanZhiGouVO();
				teQuanZhiGouVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanZhiGouVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanZhiGou.xml没有拿到"
			});
		}
	}

	public BetterList<TeQuanZhiGouVO> GetItems()
	{
		return this.mDataList;
	}

	public BetterList<TeQuanZhiGouVO> GetItemsByTeQuanID(int ID)
	{
		BetterList<TeQuanZhiGouVO> betterList = new BetterList<TeQuanZhiGouVO>();
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i].TeQuanID == ID)
			{
				betterList.Add(this.mDataList[i]);
			}
		}
		return betterList;
	}

	public TeQuanZhiGouVO GetItem(int id)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i].ID == id)
			{
				return this.mDataList[i];
			}
		}
		return null;
	}

	private const string Path = "GameRes/Config/TeQuanZhiGou.xml";

	private BetterList<TeQuanZhiGouVO> mDataList = new BetterList<TeQuanZhiGouVO>();
}
