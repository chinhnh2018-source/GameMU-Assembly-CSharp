using System;
using Tmsk.Xml;

public class DaiBiShiYongVO
{
	public DaiBiShiYongVO()
	{
	}

	public DaiBiShiYongVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.ZhongWenMingCheng = xe.GetAttributeStr("ZhongWenMingCheng");
		this.XiTongMingCheng = xe.GetAttributeStr("XiTongMingCheng");
		this.DaiBiKaiGuan = xe.GetAttributeInt("DaiBiKaiGuan", -1);
	}

	public int ID { get; set; }

	public string ZhongWenMingCheng { get; set; }

	public string XiTongMingCheng { get; set; }

	public int DaiBiKaiGuan { get; set; }
}
