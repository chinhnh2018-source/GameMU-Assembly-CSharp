using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class TeQuanTiaoJianVO
{
	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.KaiQiShiJian = Global.GetXElementAttributeStr(xml, "KaiQiShiJian");
		this.JieShuShiJian = Global.GetXElementAttributeStr(xml, "JieShuShiJian");
		this.TiaoJianLeiXing = Global.GetXElementAttributeInt(xml, "TiaoJianLeiXing");
		this._GuDingLeiXing = Global.GetXElementAttributeStr(xml, "GuDingLeiXing");
		this.AnNiuName = Global.GetXElementAttributeStr(xml, "AnNiuName");
		this.TiaoZhanLianJie = Global.GetXElementAttributeInt(xml, "TiaoZhanLianJie");
		this.JiHuoJieDuan = Global.GetXElementAttributeInt(xml, "JiHuoJieDuan");
		this.JiHuoID = Global.GetXElementAttributeStr(xml, "JiHuoID");
		this.HuoDongNiCheng = Global.GetXElementAttributeStr(xml, "HuoDongNiCheng");
		this.JieMianTips = Global.GetXElementAttributeStr(xml, "JieMianTips");
		this.JiangLiFanKui = Global.GetXElementAttributeStr(xml, "JiangLiFanKui");
		this.MeiRiShangXian = Global.GetXElementAttributeInt(xml, "MeiRiShangXian");
	}

	public int GuDingLeiXing
	{
		get
		{
			return this._GuDingLeiXing.SafeToInt32(0);
		}
	}

	public BetterList<int> JiHuoIDs
	{
		get
		{
			BetterList<int> betterList = new BetterList<int>();
			string[] array = this.JiHuoID.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				betterList.Add(array[i].SafeToInt32(0));
			}
			return betterList;
		}
	}

	public int ID;

	public string KaiQiShiJian;

	public string JieShuShiJian;

	public int TiaoJianLeiXing;

	public string _GuDingLeiXing;

	public string AnNiuName;

	public int TiaoZhanLianJie;

	public int JiHuoJieDuan;

	public string JiHuoID;

	public string HuoDongNiCheng;

	public string JieMianTips;

	public int MeiRiShangXian;

	public string JiangLiFanKui;
}
