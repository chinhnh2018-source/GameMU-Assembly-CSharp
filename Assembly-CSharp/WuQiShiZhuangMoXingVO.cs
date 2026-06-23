using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class WuQiShiZhuangMoXingVO
{
	public void CopyForm(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.GoodsID = Global.GetXElementAttributeInt(xml, "GoodsID");
		this.Occupayion = Global.GetXElementAttributeInt(xml, "Occupayion");
		this.Left = Global.GetXElementAttributeInt(xml, "Left");
		this.Right = Global.GetXElementAttributeInt(xml, "Right");
	}

	public int ID;

	public string Name;

	public int GoodsID;

	public int Occupayion;

	public int Left;

	public int Right;
}
