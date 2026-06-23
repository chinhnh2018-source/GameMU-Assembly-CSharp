using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TaskZhangJieVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.ZhangJieName = Global.GetXElementAttributeStr(xml, "ZhangJieName");
		this.ZhangJieMiaoShu = Global.GetXElementAttributeStr(xml, "ZhangJieMiaoShu");
		this.EndTaskID = Global.GetXElementAttributeInt(xml, "EndTaskID");
		this.NeedTaskNum = Global.GetXElementAttributeInt(xml, "NeedTaskNum");
		this.GlGoodsID = Global.GetXElementAttributeInt(xml, "GlGoodsID");
	}

	public int ID;

	public string ZhangJieName;

	public string ZhangJieMiaoShu;

	public int EndTaskID;

	public int NeedTaskNum;

	public int GlGoodsID;

	public int TaskClass;

	public int TaskCount;
}
