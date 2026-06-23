using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class KuafuActivityItemConfig
{
	public void InitConfig(KuafuActivityTabConfigLine tabconfigLine)
	{
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/{0}", tabconfigLine.GLXml));
		XElement xelement = null;
		if (tabconfigLine.IsKuaFuBOSS || tabconfigLine.IsKuaFuWangZhe)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
			foreach (XElement xelement2 in xelementList)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "MinZhuanSheng");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "MinLevel");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "MaxZhuanSheng");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "MinLevel");
				if (Global.Data.roleData.ChangeLifeCount < xelementAttributeInt3 || (Global.Data.roleData.ChangeLifeCount == xelementAttributeInt3 && Global.Data.roleData.Level <= xelementAttributeInt4))
				{
					xelement = xelement2;
					break;
				}
			}
			if (xelement == null && xelementList.Count > 0)
			{
				xelement = xelementList[xelementList.Count - 1];
			}
		}
		if (tabconfigLine.IsKuFuPlunder)
		{
			this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
			this.id = tabconfigLine.ID;
		}
		else
		{
			if (xelement == null)
			{
				xelement = Global.GetXElement(gameResXml, "Item", "ID", tabconfigLine.ID.ToString());
			}
			if (xelement != null)
			{
				this.id = Global.GetXElementAttributeInt(xelement, "ID");
				this.group = Global.GetXElementAttributeInt(xelement, "Group");
				this.mapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
				this.ApplyTime = Global.GetXElementAttributeStr(xelement, "ApplyTime");
				this.TimePoints = Global.GetXElementAttributeStr(xelement, "TimePoints");
				this.waitingEnterSecs = Global.GetXElementAttributeInt(xelement, "WaitingEnterSecs");
				this.prepareSecs = Global.GetXElementAttributeInt(xelement, "PrepareSecs");
				this.fightingSecs = Global.GetXElementAttributeInt(xelement, "FightingSecs");
				this.clearRolesSecs = Global.GetXElementAttributeInt(xelement, "ClearRolesSecs");
				this.minZhuanSheng = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				this.minLevel = Global.GetXElementAttributeInt(xelement, "MinLevel");
				this.minRequestNum = Global.GetXElementAttributeInt(xelement, "MinRequestNum");
				this.maxEnterNum = Global.GetXElementAttributeInt(xelement, "MaxEnterNum");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Award");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Image");
				this.award = xelementAttributeStr.Split(new char[]
				{
					'|'
				});
				this.image = xelementAttributeStr2.Split(new char[]
				{
					'|'
				});
				List<string> list = new List<string>();
				string[] array = this.TimePoints.Split(new char[]
				{
					'|'
				});
				if (array.Length > 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							'-'
						});
						if (array2.Length > 0)
						{
							for (int j = 0; j < array2.Length; j++)
							{
								string[] array3 = array2[j].Split(new char[]
								{
									':'
								});
								list.Add(array3[0]);
								list.Add(array3[1]);
							}
						}
					}
				}
				this.timePoints = list;
				this.time = array;
			}
		}
	}

	public bool IsHaveApplyTime
	{
		get
		{
			return !string.IsNullOrEmpty(this.ApplyTime);
		}
	}

	public int id;

	public int group;

	public int mapCode;

	public string ApplyTime = string.Empty;

	public string TimePoints = string.Empty;

	public int waitingEnterSecs;

	public int prepareSecs;

	public int fightingSecs;

	public int clearRolesSecs;

	public int minZhuanSheng;

	public int minLevel;

	public int minRequestNum;

	public int maxEnterNum;

	public string[] award;

	public string[] image;

	public List<string> timePoints;

	public string[] time;

	public CrusadeWarXml mCrusadeWarXml;
}
