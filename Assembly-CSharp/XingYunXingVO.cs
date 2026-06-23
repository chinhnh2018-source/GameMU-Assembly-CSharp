using System;
using Tmsk.Xml;

public class XingYunXingVO
{
	public XingYunXingVO()
	{
	}

	public XingYunXingVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.ZhongWenMingCheng = xe.GetAttributeStr("ZhongWenMingCheng");
		this.XiTongMingCheng = xe.GetAttributeStr("XiTongMingCheng");
		this.XingYunXingKaiGuan = xe.GetAttributeInt("XingYunKaiGuan", -1);
	}

	public int ID { get; set; }

	public string ZhongWenMingCheng { get; set; }

	public string XiTongMingCheng { get; set; }

	public int XingYunXingKaiGuan { get; set; }
}
