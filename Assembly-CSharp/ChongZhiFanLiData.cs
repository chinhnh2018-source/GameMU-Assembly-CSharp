using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ChongZhiFanLiData
{
	public ChongZhiFanLiData(XElement element)
	{
		this.ID = Global.GetXElementAttributeInt(element, "ID");
		this.Data = DateTime.Parse(Global.GetXElementAttributeStr(element, "Data"));
		this.BeginTime = DateTime.Parse(Global.GetXElementAttributeStr(element, "BeginTime"));
		this.EndTime = DateTime.Parse(Global.GetXElementAttributeStr(element, "EndTime"));
		this.SinglePurchase = Global.GetXElementAttributeInt(element, "SinglePurchase");
		this.FullPurchase = Global.GetXElementAttributeInt(element, "FullPurchase");
		this.Num = Global.GetXElementAttributeInt(element, "Num");
		this.Description = Global.GetXElementAttributeStr(element, "Description");
	}

	public int ID;

	public DateTime Data;

	public DateTime BeginTime;

	public DateTime EndTime;

	public int SinglePurchase;

	public int FullPurchase;

	public int Num;

	public string Description;
}
