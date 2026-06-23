using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class GoodVO
{
	public double[] EquipProps
	{
		get
		{
			if (this._EquipProps == null)
			{
				this._EquipProps = VOBinOperator.UnPackArrayDouble(this._EquipPropsBinString);
			}
			return this._EquipProps;
		}
	}

	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Categoriy = Global.GetXElementAttributeInt(xml, "Categoriy");
		this.HandType = Global.GetXElementAttributeInt(xml, "HandType");
		this.ActionType = Global.GetXElementAttributeInt(xml, "ActionType");
		this.RebornEquip = Global.GetXElementAttributeInt(xml, "RebornEquip");
		this.Title = Global.GetXElementAttributeStr(xml, "Title");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.ResName = Global.GetXElementAttributeStr(xml, "ResName");
		this.GuaJieDian = Global.GetXElementAttributeStr(xml, "GuaJieDian");
		this.GuaJieTeXiao = Global.GetXElementAttributeStr(xml, "GuaJieTeXiao");
		this.ShaderID = Global.GetXElementAttributeStr(xml, "ShaderID");
		this.GridNum = Global.GetXElementAttributeInt(xml, "GridNum");
		this.PriceOne = Global.GetXElementAttributeInt(xml, "PriceOne");
		this.PriceTwo = Global.GetXElementAttributeInt(xml, "PriceTwo");
		this.CDTime = Global.GetXElementAttributeInt(xml, "CDTime");
		this.PubCDTime = Global.GetXElementAttributeInt(xml, "PubCDTime");
		this.ShareGroupID = Global.GetXElementAttributeInt(xml, "ShareGroupID");
		this.SuitID = Global.GetXElementAttributeInt(xml, "SuitID");
		this.ShouShiSuitID = Global.GetXElementAttributeInt(xml, "ShouShiSuitID");
		this.QualityID = Global.GetXElementAttributeInt(xml, "QualityID");
		this.IconCode = Global.GetXElementAttributeStr(xml, "IconCode");
		this.FallGoodsIcon = Global.GetXElementAttributeStr(xml, "FallGoodsIcon");
		this.GoodsColor = Global.GetXElementAttributeStr(xml, "GoodsColor");
		this.FallGoodsColor = Global.GetXElementAttributeStr(xml, "FallGoodsColor");
		this.ToZhuanSheng = Global.GetXElementAttributeInt(xml, "ToZhuanSheng");
		this.ToLevel = Global.GetXElementAttributeInt(xml, "ToLevel");
		this.ToOccupation = Global.GetXElementAttributeInt(xml, "ToOccupation");
		this.ToSex = Global.GetXElementAttributeInt(xml, "ToSex");
		this.ToType = Global.GetXElementAttributeStr(xml, "ToType");
		this.ToTypeProperty = Global.GetXElementAttributeStr(xml, "ToTypeProperty");
		this.UsingMode = Global.GetXElementAttributeInt(xml, "UsingMode");
		this.PiLiangUse = Global.GetXElementAttributeStr(xml, "PiLiangUse");
		this.UsingNum = Global.GetXElementAttributeInt(xml, "UsingNum");
		this.DayLimit = Global.GetXElementAttributeInt(xml, "DayLimit");
		this.ExecMagic = Global.GetXElementAttributeStr(xml, "ExecMagic");
		this.TuoZhan = Global.GetXElementAttributeInt(xml, "TuoZhan");
		this.XiLian = Global.GetXElementAttributeInt(xml, "XiLian");
		this.BaoguoID = Global.GetXElementAttributeInt(xml, "BaoguoID");
		this.GlUI = Global.GetXElementAttributeInt(xml, "GlUI");
		this.LieShaPrice = Global.GetXElementAttributeInt(xml, "LieShaPrice");
		this.JinYuanPrice = Global.GetXElementAttributeInt(xml, "JinYuanPrice");
		this.JunGongPrice = Global.GetXElementAttributeInt(xml, "JunGongPrice");
		this.JiFenPrice = Global.GetXElementAttributeInt(xml, "JiFenPrice");
		this.ZhanHunPrice = Global.GetXElementAttributeInt(xml, "ZhanHunPrice");
		this.ChangeJinYuan = Global.GetXElementAttributeInt(xml, "ChangeJinYuan");
		this.ChangeRebornExp = Global.GetXElementAttributeInt(xml, "ChangeRebornExp");
		this.Sound = Global.GetXElementAttributeStr(xml, "Sound");
		this.DropSound = Global.GetXElementAttributeStr(xml, "DropSound");
		this.GetSound = Global.GetXElementAttributeStr(xml, "GetSound");
		this.ToReborn = Global.GetXElementAttributeInt(xml, "ToReborn");
		this.ToRebornLevel = Global.GetXElementAttributeInt(xml, "ToRebornLevel");
		this._EquipProps = ConvertExt.StringArray2DoubleArray(Global.GetXElementAttributeStr(xml, "EquipProps").Split(new char[]
		{
			','
		}));
		this.Strength = Global.GetXElementAttributeInt(xml, "Strength");
		this.Dexterity = Global.GetXElementAttributeInt(xml, "Dexterity");
		this.Intelligence = Global.GetXElementAttributeInt(xml, "Intelligence");
		this.Constitution = Global.GetXElementAttributeInt(xml, "Constitution");
		this.JinJie = Global.GetXElementAttributeInt(xml, "JinJie");
		this.ChangeZaiZao = Global.GetXElementAttributeInt(xml, "ChangeZaiZao");
		this.MainOccupation = Global.GetXElementAttributeInt(xml, "MainOccupation");
		this.PulverizedFluorescentPowderNum = Global.GetXElementAttributeInt(xml, "ChangeYingGuang");
		this.ItemQuality = Global.GetXElementAttributeInt(xml, "ItemQuality");
		this.NoSaleOut = Global.GetXElementAttributeInt(xml, "NoSaleOut");
		this.Valuables = Global.GetXElementAttributeInt(xml, "Valuables");
		this.ChangeHunJing = Global.GetXElementAttributeInt(xml, "ChangeHunJing");
		this.ChangeFengYingJingShi = Global.GetXElementAttributeInt(xml, "ChangeFengYingJingShi");
		this.ChangeChongShengJingShi = Global.GetXElementAttributeInt(xml, "ChangeChongShengJingShi");
		this.ChangeXuanCaiJingShi = Global.GetXElementAttributeInt(xml, "ChangeXuanCaiJingShi");
	}

	public static Dictionary<string, int> PropertyIndexDict
	{
		get
		{
			if (GoodVO._PropertyIndexDict == null)
			{
				int num = -1;
				GoodVO._PropertyIndexDict = new Dictionary<string, int>();
				GoodVO._PropertyIndexDict.Add("ID", ++num);
				GoodVO._PropertyIndexDict.Add("Categoriy", ++num);
				GoodVO._PropertyIndexDict.Add("HandType", ++num);
				GoodVO._PropertyIndexDict.Add("ActionType", ++num);
				GoodVO._PropertyIndexDict.Add("RebornEquip", ++num);
				GoodVO._PropertyIndexDict.Add("Title", ++num);
				GoodVO._PropertyIndexDict.Add("Description", ++num);
				GoodVO._PropertyIndexDict.Add("ResName", ++num);
				GoodVO._PropertyIndexDict.Add("GuaJieDian", ++num);
				GoodVO._PropertyIndexDict.Add("GuaJieTeXiao", ++num);
				GoodVO._PropertyIndexDict.Add("ShaderID", ++num);
				GoodVO._PropertyIndexDict.Add("GridNum", ++num);
				GoodVO._PropertyIndexDict.Add("PriceOne", ++num);
				GoodVO._PropertyIndexDict.Add("PriceTwo", ++num);
				GoodVO._PropertyIndexDict.Add("CDTime", ++num);
				GoodVO._PropertyIndexDict.Add("PubCDTime", ++num);
				GoodVO._PropertyIndexDict.Add("ShareGroupID", ++num);
				GoodVO._PropertyIndexDict.Add("SuitID", ++num);
				GoodVO._PropertyIndexDict.Add("ShouShiSuitID", ++num);
				GoodVO._PropertyIndexDict.Add("QualityID", ++num);
				GoodVO._PropertyIndexDict.Add("IconCode", ++num);
				GoodVO._PropertyIndexDict.Add("FallGoodsIcon", ++num);
				GoodVO._PropertyIndexDict.Add("GoodsColor", ++num);
				GoodVO._PropertyIndexDict.Add("FallGoodsColor", ++num);
				GoodVO._PropertyIndexDict.Add("ToZhuanSheng", ++num);
				GoodVO._PropertyIndexDict.Add("ToLevel", ++num);
				GoodVO._PropertyIndexDict.Add("ToOccupation", ++num);
				GoodVO._PropertyIndexDict.Add("ToSex", ++num);
				GoodVO._PropertyIndexDict.Add("ToType", ++num);
				GoodVO._PropertyIndexDict.Add("ToTypeProperty", ++num);
				GoodVO._PropertyIndexDict.Add("UsingMode", ++num);
				GoodVO._PropertyIndexDict.Add("PiLiangUse", ++num);
				GoodVO._PropertyIndexDict.Add("UsingNum", ++num);
				GoodVO._PropertyIndexDict.Add("DayLimit", ++num);
				GoodVO._PropertyIndexDict.Add("ExecMagic", ++num);
				GoodVO._PropertyIndexDict.Add("TuoZhan", ++num);
				GoodVO._PropertyIndexDict.Add("XiLian", ++num);
				GoodVO._PropertyIndexDict.Add("BaoguoID", ++num);
				GoodVO._PropertyIndexDict.Add("GlUI", ++num);
				GoodVO._PropertyIndexDict.Add("LieShaPrice", ++num);
				GoodVO._PropertyIndexDict.Add("JinYuanPrice", ++num);
				GoodVO._PropertyIndexDict.Add("JunGongPrice", ++num);
				GoodVO._PropertyIndexDict.Add("JiFenPrice", ++num);
				GoodVO._PropertyIndexDict.Add("ZhanHunPrice", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeJinYuan", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeRebornExp", ++num);
				GoodVO._PropertyIndexDict.Add("Sound", ++num);
				GoodVO._PropertyIndexDict.Add("DropSound", ++num);
				GoodVO._PropertyIndexDict.Add("GetSound", ++num);
				GoodVO._PropertyIndexDict.Add("ToReborn", ++num);
				GoodVO._PropertyIndexDict.Add("ToRebornLevel", ++num);
				GoodVO._PropertyIndexDict.Add("EquipProps", ++num);
				GoodVO._PropertyIndexDict.Add("Strength", ++num);
				GoodVO._PropertyIndexDict.Add("Dexterity", ++num);
				GoodVO._PropertyIndexDict.Add("Intelligence", ++num);
				GoodVO._PropertyIndexDict.Add("Constitution", ++num);
				GoodVO._PropertyIndexDict.Add("JinJie", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeZaiZao", ++num);
				GoodVO._PropertyIndexDict.Add("MainOccupation", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeYingGuang", ++num);
				GoodVO._PropertyIndexDict.Add("ItemQuality", ++num);
				GoodVO._PropertyIndexDict.Add("NoSaleOut", ++num);
				GoodVO._PropertyIndexDict.Add("Valuables", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeHunJing", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeFengYingJingShi", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeChongShengJingShi", ++num);
				GoodVO._PropertyIndexDict.Add("ChangeXuanCaiJingShi", num + 1);
			}
			return GoodVO._PropertyIndexDict;
		}
	}

	public void CopyFrom(TrdGoodVOPairs pairs)
	{
		if (TrdGoodVOPairs.IsCopySpeedUp())
		{
			pairs.StartGoodVOCopySpeedUp();
		}
		this.ID = pairs.PropertyAtOfInt("ID");
		this.Categoriy = pairs.PropertyAtOfInt("Categoriy");
		this.HandType = pairs.PropertyAtOfInt("HandType");
		this.ActionType = pairs.PropertyAtOfInt("ActionType");
		this.RebornEquip = pairs.PropertyAtOfInt("RebornEquip");
		this.Title = pairs.PropertyAtOfStr("Title");
		this.Description = pairs.PropertyAtOfStr("Description");
		this.ResName = pairs.PropertyAtOfStr("ResName");
		this.GuaJieDian = pairs.PropertyAtOfStr("GuaJieDian");
		this.GuaJieTeXiao = pairs.PropertyAtOfStr("GuaJieTeXiao");
		this.ShaderID = pairs.PropertyAtOfStr("ShaderID");
		this.GridNum = pairs.PropertyAtOfInt("GridNum");
		this.PriceOne = pairs.PropertyAtOfInt("PriceOne");
		this.PriceTwo = pairs.PropertyAtOfInt("PriceTwo");
		this.CDTime = pairs.PropertyAtOfInt("CDTime");
		this.PubCDTime = pairs.PropertyAtOfInt("PubCDTime");
		this.ShareGroupID = pairs.PropertyAtOfInt("ShareGroupID");
		this.SuitID = pairs.PropertyAtOfInt("SuitID");
		this.ShouShiSuitID = pairs.PropertyAtOfInt("ShouShiSuitID");
		this.QualityID = pairs.PropertyAtOfInt("QualityID");
		this.IconCode = pairs.PropertyAtOfStr("IconCode");
		this.FallGoodsIcon = pairs.PropertyAtOfStr("FallGoodsIcon");
		this.GoodsColor = pairs.PropertyAtOfStr("GoodsColor");
		this.FallGoodsColor = pairs.PropertyAtOfStr("FallGoodsColor");
		this.ToZhuanSheng = pairs.PropertyAtOfInt("ToZhuanSheng");
		this.ToLevel = pairs.PropertyAtOfInt("ToLevel");
		this.ToOccupation = pairs.PropertyAtOfInt("ToOccupation");
		this.ToSex = pairs.PropertyAtOfInt("ToSex");
		this.ToType = pairs.PropertyAtOfStr("ToType");
		this.ToTypeProperty = pairs.PropertyAtOfStr("ToTypeProperty");
		this.UsingMode = pairs.PropertyAtOfInt("UsingMode");
		this.PiLiangUse = pairs.PropertyAtOfStr("PiLiangUse");
		this.UsingNum = pairs.PropertyAtOfInt("UsingNum");
		this.DayLimit = pairs.PropertyAtOfInt("DayLimit");
		this.ExecMagic = pairs.PropertyAtOfStr("ExecMagic");
		this.TuoZhan = pairs.PropertyAtOfInt("TuoZhan");
		this.XiLian = pairs.PropertyAtOfInt("XiLian");
		this.BaoguoID = pairs.PropertyAtOfInt("BaoguoID");
		this.GlUI = pairs.PropertyAtOfInt("GlUI");
		this.LieShaPrice = pairs.PropertyAtOfInt("LieShaPrice");
		this.JinYuanPrice = pairs.PropertyAtOfInt("JinYuanPrice");
		this.JunGongPrice = pairs.PropertyAtOfInt("JunGongPrice");
		this.JiFenPrice = pairs.PropertyAtOfInt("JiFenPrice");
		this.ZhanHunPrice = pairs.PropertyAtOfInt("ZhanHunPrice");
		this.ChangeJinYuan = pairs.PropertyAtOfInt("ChangeJinYuan");
		this.ChangeRebornExp = pairs.PropertyAtOfInt("ChangeRebornExp");
		this.Sound = pairs.PropertyAtOfStr("Sound");
		this.DropSound = pairs.PropertyAtOfStr("DropSound");
		this.GetSound = pairs.PropertyAtOfStr("GetSound");
		this.ToReborn = pairs.PropertyAtOfInt("ToReborn");
		this.ToRebornLevel = pairs.PropertyAtOfInt("ToRebornLevel");
		this._EquipPropsBinString = pairs.PropertyAtOfStr("EquipProps");
		this.Strength = pairs.PropertyAtOfInt("Strength");
		this.Dexterity = pairs.PropertyAtOfInt("Dexterity");
		this.Intelligence = pairs.PropertyAtOfInt("Intelligence");
		this.Constitution = pairs.PropertyAtOfInt("Constitution");
		this.JinJie = pairs.PropertyAtOfInt("JinJie");
		this.ChangeZaiZao = pairs.PropertyAtOfInt("ChangeZaiZao");
		this.MainOccupation = pairs.PropertyAtOfInt("MainOccupation");
		this.PulverizedFluorescentPowderNum = pairs.PropertyAtOfInt("ChangeYingGuang");
		this.ItemQuality = pairs.PropertyAtOfInt("ItemQuality");
		this.NoSaleOut = pairs.PropertyAtOfInt("NoSaleOut");
		this.Valuables = pairs.PropertyAtOfInt("Valuables");
		this.ChangeHunJing = pairs.PropertyAtOfInt("ChangeHunJing");
		this.ChangeFengYingJingShi = pairs.PropertyAtOfInt("ChangeFengYingJingShi");
		this.ChangeChongShengJingShi = pairs.PropertyAtOfInt("ChangeChongShengJingShi");
		this.ChangeXuanCaiJingShi = pairs.PropertyAtOfInt("ChangeXuanCaiJingShi");
	}

	public List<byte> ToByteLst(GoodVO preVO, GoodVO curVO)
	{
		return new List<byte>();
	}

	public bool Equals(GoodVO obj)
	{
		return obj != null;
	}

	public int ID;

	public int Categoriy;

	public int HandType;

	public int ActionType;

	public int RebornEquip;

	public string Title;

	public string Description;

	public string ResName;

	public string GuaJieDian;

	public string GuaJieTeXiao;

	public string ShaderID;

	public int GridNum;

	public int PriceOne;

	public int PriceTwo;

	public int CDTime;

	public int PubCDTime;

	public int ShareGroupID;

	public int SuitID;

	public int ShouShiSuitID;

	public int QualityID;

	public string IconCode;

	public string FallGoodsIcon;

	public string GoodsColor;

	public string FallGoodsColor;

	public int ToZhuanSheng;

	public int ToLevel;

	public int ToOccupation;

	public int ToSex;

	public string ToType;

	public string ToTypeProperty;

	public int UsingMode;

	public string PiLiangUse;

	public int UsingNum;

	public int DayLimit;

	public string ExecMagic;

	public int TuoZhan;

	public int XiLian;

	public int BaoguoID;

	public int GlUI;

	public int LieShaPrice;

	public int JinYuanPrice;

	public int JunGongPrice;

	public int JiFenPrice;

	public int ZhanHunPrice;

	public int ChangeJinYuan;

	public int ChangeRebornExp;

	public string Sound;

	public string DropSound;

	public string GetSound;

	public int ToReborn;

	public int ToRebornLevel;

	public int ChangeHunJing;

	public int Strength;

	public int Dexterity;

	public int Intelligence;

	public int Constitution;

	public int JinJie;

	public int ChangeZaiZao;

	public int MainOccupation;

	public int PulverizedFluorescentPowderNum;

	private double[] _EquipProps;

	private string _EquipPropsBinString;

	public int ItemQuality;

	public int NoSaleOut;

	public int Valuables;

	public int ChangeFengYingJingShi;

	public int ChangeChongShengJingShi;

	public int ChangeXuanCaiJingShi;

	private static Dictionary<string, int> _PropertyIndexDict;
}
