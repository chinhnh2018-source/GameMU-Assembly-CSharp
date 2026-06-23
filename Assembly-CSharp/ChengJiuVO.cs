using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ChengJiuVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.ChengJiuID = Global.GetXElementAttributeInt(xml, "ChengJiuID");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.ChengJiu = Global.GetXElementAttributeInt(xml, "ChengJiu");
		this.BindZuanShi = Global.GetXElementAttributeInt(xml, "BindZuanShi");
		this.BindMoney = Global.GetXElementAttributeInt(xml, "BindMoney");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.ZhuanShengLimit = Global.GetXElementAttributeInt(xml, "ZhuanShengLimit");
		this.LevelLimit = Global.GetXElementAttributeInt(xml, "LevelLimit");
		this.LoginDayOne = Global.GetXElementAttributeInt(xml, "LoginDayOne");
		this.LoginDayTwo = Global.GetXElementAttributeInt(xml, "LoginDayTwo");
		this.KillMonster = Global.GetXElementAttributeInt(xml, "KillMonster");
		this.KillBoss = Global.GetXElementAttributeInt(xml, "KillBoss");
		this.TongQianLimit = Global.GetXElementAttributeInt(xml, "TongQianLimit");
		this.QiangHuaLimit = Global.GetXElementAttributeStr(xml, "QiangHuaLimit");
		this.ZhuiJiaLimit = Global.GetXElementAttributeStr(xml, "ZhuiJiaLimit");
		this.HeChengLimit = Global.GetXElementAttributeStr(xml, "HeChengLimit");
		this.KillRaid = Global.GetXElementAttributeInt(xml, "KillRaid");
		this.GoodsLimit = Global.GetXElementAttributeStr(xml, "GoodsLimit");
		this.SkillLevel = Global.GetXElementAttributeInt(xml, "SkillLevel");
	}

	public int ID;

	public int ChengJiuID;

	public string Name;

	public int ChengJiu;

	public int BindZuanShi;

	public int BindMoney;

	public string Description;

	public int ZhuanShengLimit;

	public int LevelLimit;

	public int LoginDayOne;

	public int LoginDayTwo;

	public int KillMonster;

	public int KillBoss;

	public int TongQianLimit;

	public string QiangHuaLimit;

	public string ZhuiJiaLimit;

	public string HeChengLimit;

	public int KillRaid;

	public string GoodsLimit;

	public int SkillLevel;
}
