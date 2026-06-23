using System;
using Tmsk.Xml;

public class CaiDaXiaoVO
{
	public CaiDaXiaoVO()
	{
	}

	public CaiDaXiaoVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.HuoDongKaiQi = xe.GetAttributeStr("HuoDongKaiQi");
		this.HuoDongJieSu = xe.GetAttributeStr("HuoDongJieSu");
		this.MeiRiKaiQi = xe.GetAttributeStr("MeiRiKaiQi");
		this.MeiRiJieSu = xe.GetAttributeStr("MeiRiJieSu");
		this.XiaoHaoDaiBi = xe.GetAttributeInt("XiaoHaoDaiBi", -1);
		this.ZhuShuShangXian = xe.GetAttributeInt("ZhuShuShangXian", -1);
		this.ShangChengKaiGuan = xe.GetAttributeInt("ShangChengKaiGuan", -1);
	}

	public int ID { get; set; }

	public string HuoDongKaiQi { get; set; }

	public string HuoDongJieSu { get; set; }

	public string MeiRiKaiQi { get; set; }

	public string MeiRiJieSu { get; set; }

	public int XiaoHaoDaiBi { get; set; }

	public int ZhuShuShangXian { get; set; }

	public int ShangChengKaiGuan { get; set; }
}
