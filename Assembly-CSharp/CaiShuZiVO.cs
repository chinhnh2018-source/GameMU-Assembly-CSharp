using System;
using Tmsk.Xml;

public class CaiShuZiVO
{
	public CaiShuZiVO()
	{
	}

	public CaiShuZiVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.KaiQiShiJian = xe.GetAttributeStr("KaiQiShiJian");
		this.JieShuShiJian = xe.GetAttributeStr("JieShuShiJian");
		this.KaiJiangShiJian = xe.GetAttributeStr("KaiJiangShiJian");
		this.XiaoHaoDaiBi = xe.GetAttributeInt("XiaoHaoDaiBi", -1);
		this.ZhongJiangFanLi = xe.GetAttributeInt("ZhongJiangFanLi", -1);
		this.ChuFaBiZhong = xe.GetAttributeInt("ChuFaBiZhong", -1);
		this.BuChongTiaoJian = xe.GetAttributeInt("BuChongTiaoJian", -1);
		this.XiTongChouCheng = xe.GetAttributeStr("XiTongChouCheng");
		this.ShangChengKaiGuan = xe.GetAttributeInt("ShangChengKaiGuan", -1);
	}

	public int ID { get; set; }

	public string KaiQiShiJian { get; set; }

	public string JieShuShiJian { get; set; }

	public string KaiJiangShiJian { get; set; }

	public int XiaoHaoDaiBi { get; set; }

	public int ZhongJiangFanLi { get; set; }

	public int ChuFaBiZhong { get; set; }

	public int BuChongTiaoJian { get; set; }

	public string XiTongChouCheng { get; set; }

	public int ShangChengKaiGuan { get; set; }
}
