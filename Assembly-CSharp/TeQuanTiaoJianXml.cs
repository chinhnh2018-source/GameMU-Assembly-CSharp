using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanTiaoJianXml
{
	public TeQuanTiaoJianXml()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanTiaoJian.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanTiaoJian");
			if (xelementList == null || 0 >= xelementList.Count)
			{
				xelementList = Global.GetXElementList(teQuanXml, "JingLing");
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanTiaoJianVO teQuanTiaoJianVO = new TeQuanTiaoJianVO();
				teQuanTiaoJianVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanTiaoJianVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanTiaoJian.xml没有拿到"
			});
		}
	}

	public BetterList<TeQuanTiaoJianVO> GetTeQuanTiaoJianVOList()
	{
		return this.mDataList;
	}

	public TeQuanTiaoJianVO GetTeQuanTiaoJianVOByID(int ID)
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

	private const string Path = "GameRes/Config/TeQuanTiaoJian.xml";

	private BetterList<TeQuanTiaoJianVO> mDataList = new BetterList<TeQuanTiaoJianVO>();
}
