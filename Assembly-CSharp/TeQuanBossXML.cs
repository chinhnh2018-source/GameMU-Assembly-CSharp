using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanBossXML
{
	public TeQuanBossXML()
	{
		XElement teQuanXml = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanXml("GameRes/Config/TeQuanBoss.xml");
		if (teQuanXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(teQuanXml, "TeQuanBoss");
			for (int i = 0; i < xelementList.Count; i++)
			{
				TeQuanBossVO teQuanBossVO = new TeQuanBossVO();
				teQuanBossVO.CopyFrom(xelementList[i]);
				this.mDataList.Add(teQuanBossVO);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"配置GameRes/Config/TeQuanBoss.xml没有拿到"
			});
		}
	}

	private const string Path = "GameRes/Config/TeQuanBoss.xml";

	private BetterList<TeQuanBossVO> mDataList = new BetterList<TeQuanBossVO>();
}
