using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SettingMapVO
{
	public void CopyFrom(XElement xml)
	{
		this.Code = Global.GetXElementAttributeInt(xml, "Code");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.MapType = Global.GetXElementAttributeInt(xml, "MapType");
		this.PicCode = Global.GetXElementAttributeInt(xml, "PicCode");
		this.Music = Global.GetXElementAttributeStr(xml, "Music");
		this.AutoStart = Global.GetXElementAttributeInt(xml, "AutoStart");
		this.ResName = Global.GetXElementAttributeStr(xml, "ResName");
		this.SliceTerrain = Global.GetXElementAttributeInt(xml, "SliceTerrain");
		this.RealiveType = Global.GetXElementAttributeInt(xml, "RealiveType");
		this.loadingImage = Global.GetXElementAttributeInt(xml, "loadingImage");
		this.Goods = Global.GetXElementAttributeStr(xml, "Goods");
		this.MoveType = Global.GetXElementAttributeInt(xml, "MoveType");
		this.Logo = Global.GetXElementAttributeInt(xml, "Logo");
		this.BeiShu = Global.GetXElementAttributeFloat(xml, "BeiShu");
		this.Transfer = Global.GetXElementAttributeInt(xml, "Transfer");
		this.FenBao = Global.GetXElementAttributeInt(xml, "FenBao");
		this.FileSize = Global.GetXElementAttributeInt(xml, "FileSize");
		this.DownLoadZhuanSheng = Global.GetXElementAttributeInt(xml, "DownLoadZhuanSheng");
		this.DownLoadLevel = Global.GetXElementAttributeInt(xml, "DownLoadLevel");
		this.DownLoadReward = Global.GetXElementAttributeInt(xml, "DownLoadReward");
		this.Horse = Global.GetXElementAttributeInt(xml, "Horse");
		this.Transfiguration = Global.GetXElementAttributeInt(xml, "Transfiguration");
	}

	public int Code;

	public string Name;

	public int MapType;

	public int PicCode;

	public string Music;

	public int AutoStart;

	public string ResName;

	public int SliceTerrain;

	public int RealiveType;

	public int loadingImage;

	public string Goods;

	public int MoveType;

	public int Logo;

	public float BeiShu;

	public int Transfer;

	public int FenBao;

	public int FileSize;

	public int DownLoadZhuanSheng;

	public int DownLoadLevel;

	public int DownLoadReward;

	public int Horse;

	public int Transfiguration;
}
