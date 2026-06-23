using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanJianLiXML
{
	public TeQuanJianLiXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanJiangLi.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanJianLi");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanJianLiVO teQuanJianLiVO = new TeQuanJianLiVO();
				teQuanJianLiVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanJianLiVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanJiangLi.xml没有拿到"
			});
		}
	}

	public TeQuanJianLiVO GetItemById(int ID)
	{
		for (int i = 0; i < this.mDataList.size; i++)
		{
			if (this.mDataList[i] != null && ID == this.mDataList[i].ID)
			{
				return this.mDataList[i];
			}
		}
		return null;
	}

	private const string Path = "GameRes/Config/TeQuanJiangLi.xml";

	private BetterList<TeQuanJianLiVO> mDataList = new BetterList<TeQuanJianLiVO>();
}
