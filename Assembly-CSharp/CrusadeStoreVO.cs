using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class CrusadeStoreVO
{
	public void CopyForm(XElement xle)
	{
		this.ID = Global.GetXElementAttributeInt(xle, "ID");
		this.Type = Global.GetXElementAttributeInt(xle, "Type");
		this.GoodsID = Global.GetXElementAttributeStr(xle, "GoodsID");
		this.JueXingNum = Global.GetXElementAttributeInt(xle, "JueXingNum");
		this.ZuanShiNum = Global.GetXElementAttributeInt(xle, "ZuanShiNum");
		this.SinglePurchase = Global.GetXElementAttributeInt(xle, "SinglePurchase");
		this.BeginNum = Global.GetXElementAttributeInt(xle, "BeginNum");
		this.EndNum = Global.GetXElementAttributeInt(xle, "EndNum");
	}

	public GoodsData Good
	{
		get
		{
			if (string.IsNullOrEmpty(this.GoodsID))
			{
				return null;
			}
			try
			{
				string[] array = this.GoodsID.Split(new char[]
				{
					','
				});
				return Global.GetEmptyGoodsData(array[0].SafeToInt32(0), 0, array[2].SafeToInt32(0), array[3].SafeToInt32(0), array[1].SafeToInt32(0), array[5].SafeToInt32(0), array[6].SafeToInt32(0), 0, 0);
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>" + ex.Message + "</color>"
				});
			}
			return null;
		}
	}

	public int ID;

	public int Type;

	private string GoodsID;

	public int JueXingNum;

	public int ZuanShiNum;

	public int SinglePurchase;

	public int BeginNum;

	public int EndNum;
}
