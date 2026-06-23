using System;
using Tmsk.Xml;

public class DuiHuanShangChengVO
{
	public DuiHuanShangChengVO()
	{
	}

	public DuiHuanShangChengVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.WuPinID = xe.GetAttributeStr("WuPinID");
		this.DaiBiJiaGe = xe.GetAttributeInt("DaiBiJiaGe", -1);
		this.MeiRiShangXianDan = xe.GetAttributeInt("MeiRiShangXianDan", -1);
		this.QuanFuShangXian = xe.GetAttributeInt("QuanFuShangXian", -1);
		this.Tips = xe.GetAttributeStr("Tips");
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public string WuPinID { get; set; }

	public int DaiBiJiaGe { get; set; }

	public int MeiRiShangXianDan { get; set; }

	public int QuanFuShangXian { get; set; }

	public string Tips { get; set; }
}
