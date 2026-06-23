using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ShenshenghudunXingVO
{
	public GoodsData[] GNeedGoods
	{
		get
		{
			if (this._GNeedGoods == null && !string.IsNullOrEmpty(this.NeedGoods))
			{
				string[] array = this.NeedGoods.Split(new char[]
				{
					'|'
				});
				if (0 < array.Length)
				{
					this._GNeedGoods = new GoodsData[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						this._GNeedGoods[i] = Global.GetEmptyGoodsData(array2[0].SafeToInt32(0), 0, 0, 0, array2[1].SafeToInt32(0), 0, 0, 0, 0);
					}
				}
			}
			return this._GNeedGoods;
		}
	}

	public Dictionary<string, double> Att
	{
		get
		{
			if (0 >= this._Att.Count)
			{
				this._Att.Add("ArmorMax", this.ArmorUp);
				this._Att.Add("AddAttack", this.AddAttack);
				this._Att.Add("AddDefense", this.AddDefense);
				this._Att.Add("MaxLifeV", this.MaxLifeV);
			}
			return this._Att;
		}
	}

	public void CopyForm(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.ArmorupStage = Global.GetXElementAttributeInt(xml, "ArmorupStage");
			this.StarLevel = Global.GetXElementAttributeInt(xml, "StarLevel");
			this.ModID = Global.GetXElementAttributeInt(xml, "ModID");
			this.ArmorUp = Global.GetXElementAttributeDouble(xml, "ArmorUp");
			this.AddAttack = Global.GetXElementAttributeDouble(xml, "AddAttack");
			this.AddDefense = Global.GetXElementAttributeDouble(xml, "AddDefense");
			this.MaxLifeV = Global.GetXElementAttributeDouble(xml, "ShenmingUP");
			this.StarExp = Global.GetXElementAttributeInt(xml, "StarExp");
			this.GoodsExp = Global.GetXElementAttributeInt(xml, "GoodsExp");
			this.ZuanShiExp = Global.GetXElementAttributeInt(xml, "ZuanShiExp");
			this.NeedGoods = Global.GetXElementAttributeStr(xml, "NeedGoods");
			this.NeedDiamond = Global.GetXElementAttributeInt(xml, "NeedDiamond");
		}
	}

	public int ID;

	public string Name;

	public int ArmorupStage;

	public int StarLevel;

	public int ModID;

	public double ArmorUp;

	public double AddAttack;

	public double AddDefense;

	public double MaxLifeV;

	public int StarExp;

	public int GoodsExp;

	public int ZuanShiExp;

	private string NeedGoods;

	public int NeedDiamond;

	private GoodsData[] _GNeedGoods;

	private Dictionary<string, double> _Att = new Dictionary<string, double>();
}
