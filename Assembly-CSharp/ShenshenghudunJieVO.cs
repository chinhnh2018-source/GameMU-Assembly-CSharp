using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ShenshenghudunJieVO
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

	public void CopyForm(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.ArmorClass = Global.GetXElementAttributeInt(xml, "ArmorClass");
			this.ModelID = Global.GetXElementAttributeInt(xml, "ModerID");
			this.LuckyOne = Global.GetXElementAttributeInt(xml, "LuckyOne");
			this.LuckyTwo = Global.GetXElementAttributeInt(xml, "LuckyTwo");
			this.LuckyTwoRate = Global.GetXElementAttributeFloat(xml, "LuckyTwoRate");
			this.Damageabsorption = Global.GetXElementAttributeFloat(xml, "Damageabsorption");
			this.HuDunHuiFuTips = Global.GetXElementAttributeStr(xml, "HuDunHuiFuTips");
			this.Armorrecovery = Global.GetXElementAttributeFloat(xml, "Armorrecovery");
			this.NeedGoods = Global.GetXElementAttributeStr(xml, "NeedGoods");
			this.NeedDiamond = Global.GetXElementAttributeInt(xml, "NeedDiamond");
		}
	}

	public int ID;

	public string Name;

	public int ArmorClass;

	public int ModelID;

	public int LuckyOne;

	public int LuckyTwo;

	public float LuckyTwoRate;

	public float Damageabsorption;

	public string HuDunHuiFuTips;

	public float Armorrecovery;

	private string NeedGoods;

	public int NeedDiamond;

	private GoodsData[] _GNeedGoods;
}
