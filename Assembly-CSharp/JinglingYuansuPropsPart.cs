using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class JinglingYuansuPropsPart : UserControl
{
	public static int MaxEquipNum
	{
		get
		{
			CommonFlagX.CommonFlagData flagData = CommonFlagX.GetFlagData("jingling_yuansu");
			if (flagData != null && flagData.dic.ContainsKey("openindex"))
			{
				return (int)flagData.dic["openindex"];
			}
			return 8;
		}
	}

	private void InitTextInPrefabs()
	{
		this.txtName[0].text = Global.GetLang("生命上限");
		this.txtName[1].text = Global.GetLang("攻       击");
		this.txtName[2].text = Global.GetLang("命       中");
		this.txtName[3].text = Global.GetLang("闪       避");
		this.txtName[4].text = Global.GetLang("附加伤害");
		this.txtName[5].text = Global.GetLang("物理攻击");
		this.txtName[6].text = Global.GetLang("魔法攻击");
		this.txtName[7].text = Global.GetLang("物理防御");
		this.txtName[8].text = Global.GetLang("魔法防御");
		this.txtName[9].text = Global.GetLang("抵挡伤害");
		this.txtName[10].text = Global.GetLang("元素加成");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (JinglingYuansuPropsPart.UsedGoodsTypeDict == null)
		{
			JinglingYuansuPropsPart.UsedGoodsTypeDict = new Dictionary<int, UsedYuansuEqipData>();
		}
		if (this.EquipsPropsDict == null)
		{
			this.EquipsPropsDict = new Dictionary<int, double>();
		}
		for (int i = 0; i < this.EquipKongweiArr.Length; i++)
		{
			this.lockBoxs[i].Init(i);
			this.lockBoxs[i].isOpend = this.isLockedBoxOpened(i);
		}
		this.lockBoxs[0].UpdateBox89(this.lockBoxs[8], this.lockBoxs[9]);
		this.InitEquipsPropsByUsed();
		this.UpdateServerEquipKongweiArr();
	}

	protected override void OnDestroy()
	{
		if (JinglingYuansuPropsPart.UsedGoodsTypeDict != null)
		{
			JinglingYuansuPropsPart.UsedGoodsTypeDict.Clear();
			JinglingYuansuPropsPart.UsedGoodsTypeDict = null;
		}
	}

	public void InitEquipsByUsed(List<GoodsData> list)
	{
		if (list != null && list.Count <= JinglingYuansuPropsPart.MaxEquipNum)
		{
			if (list.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.Equips[i].Clear();
				this.Equips[i].Add(this.AddIcon(list[i]));
				int equipKongwei = this.GetEquipKongwei(0);
				this.SetEquipWei(equipKongwei, 1);
				int goodsCatetoriy = Global.GetGoodsCatetoriy(list[i].GoodsID);
				UsedYuansuEqipData usedYuansuEqipData = this.GetUsedYuansuEqipData(list[i], equipKongwei);
				if (!JinglingYuansuPropsPart.UsedGoodsTypeDict.ContainsKey(goodsCatetoriy))
				{
					JinglingYuansuPropsPart.UsedGoodsTypeDict.Add(goodsCatetoriy, usedYuansuEqipData);
				}
			}
			this.InitEquipsPropsByUsed();
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备超过了最大数量"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void ModEquip(GoodsData gd, ModYuansuEquipTypes modType)
	{
		if (gd == null)
		{
			return;
		}
		if (modType == ModYuansuEquipTypes.EquipLoad)
		{
			int equipKongwei = this.GetEquipKongwei(0);
			this.Equips[equipKongwei].Add(this.AddIcon(gd));
			this.SetEquipWei(equipKongwei, 1);
			int goodsCatetoriy = Global.GetGoodsCatetoriy(gd.GoodsID);
			if (!JinglingYuansuPropsPart.UsedGoodsTypeDict.ContainsKey(goodsCatetoriy))
			{
				UsedYuansuEqipData usedYuansuEqipData = this.GetUsedYuansuEqipData(gd, equipKongwei);
				JinglingYuansuPropsPart.UsedGoodsTypeDict.Add(goodsCatetoriy, usedYuansuEqipData);
			}
			this.InitEquipsPropsByUsed();
		}
		else if (modType == ModYuansuEquipTypes.EquipUnload)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(gd.GoodsID);
			UsedYuansuEqipData usedYuansuEqipData2 = null;
			if (JinglingYuansuPropsPart.UsedGoodsTypeDict.TryGetValue(categoriyByGoodsID, ref usedYuansuEqipData2))
			{
				int index = usedYuansuEqipData2.Index;
				this.Equips[index].Clear();
				this.SetEquipWei(index, 0);
				JinglingYuansuPropsPart.UsedGoodsTypeDict.Remove(categoriyByGoodsID);
				this.InitEquipsPropsByUsed();
			}
		}
		else if (modType == ModYuansuEquipTypes.EquipMod)
		{
			int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(gd.GoodsID);
			UsedYuansuEqipData usedYuansuEqipData3 = null;
			if (JinglingYuansuPropsPart.UsedGoodsTypeDict.TryGetValue(categoriyByGoodsID2, ref usedYuansuEqipData3))
			{
				int index2 = usedYuansuEqipData3.Index;
				this.Equips[index2].Clear();
				this.Equips[index2].Add(this.AddIcon(gd));
				JinglingYuansuPropsPart.UsedGoodsTypeDict[categoriyByGoodsID2].GData = gd;
				this.InitEquipsPropsByUsed();
			}
		}
	}

	private bool isLockedBoxOpened(int index)
	{
		return this.EquipKongweiArr[index] != 2;
	}

	private int GetEquipKongwei(int value = 0)
	{
		return this.EquipKongweiArr.IndexOf(value);
	}

	private void SetEquipWei(int index, int value)
	{
		if (index < 0 || index >= this.EquipKongweiArr.Length)
		{
			return;
		}
		this.EquipKongweiArr[index] = value;
	}

	private void InitEquipsPropsByUsed()
	{
		if (JinglingYuansuPropsPart.UsedGoodsTypeDict == null || JinglingYuansuPropsPart.UsedGoodsTypeDict.Count == 0)
		{
			for (int i = 0; i < this.Attrs.Length; i++)
			{
				this.Attrs[i].Text = "0";
			}
			return;
		}
		this.EquipsPropsDict.Clear();
		foreach (UsedYuansuEqipData usedYuansuEqipData in JinglingYuansuPropsPart.UsedGoodsTypeDict.Values)
		{
			GoodsData gdata = usedYuansuEqipData.GData;
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(gdata.GoodsID);
			for (int j = 0; j < this.PropsID.Length; j++)
			{
				int num = this.PropsID[j];
				if (!this.EquipsPropsDict.ContainsKey(num))
				{
					this.EquipsPropsDict.Add(num, 0.0);
				}
				int num2 = 1;
				if (gdata.ElementhrtsProps != null && gdata.ElementhrtsProps.Count >= 2)
				{
					num2 = gdata.ElementhrtsProps[0];
				}
				Dictionary<int, double> equipsPropsDict;
				Dictionary<int, double> dictionary = equipsPropsDict = this.EquipsPropsDict;
				int num4;
				int num3 = num4 = num;
				double num5 = equipsPropsDict[num4];
				dictionary[num3] = num5 + (double)num2 * goodsEquipPropsDoubleList[num];
			}
		}
		for (int k = 0; k < this.Attrs.Length; k++)
		{
			this.Attrs[k].Text = ((int)this.EquipsPropsDict[this.PropsID[k]]).ToString();
		}
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = goodsData.GoodsID;
			icon.ItemObject = goodsData;
			icon.BoxTypes = 15;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GoodsData goodData = icon.ItemObject as GoodsData;
				GTipServiceEx.ShowTip(icon, TipTypes.YuansuBagTip, GoodsOwnerTypes.YuansuBag, goodData);
			};
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = ev.IDType,
						ZhuZhuangBei = (icon.ItemObject as GoodsData)
					});
				}
			};
			Super.InitYuansuGoodsGIcon(icon, goodsData);
			return icon;
		}
		return null;
	}

	private UsedYuansuEqipData GetUsedYuansuEqipData(GoodsData gd, int index)
	{
		return new UsedYuansuEqipData
		{
			GData = gd,
			Index = index
		};
	}

	public void UpdateServerEquipKongweiArr()
	{
		int num = JinglingYuansuPropsPart.MaxEquipNum - 1;
		if (num >= 9)
		{
			if (this.EquipKongweiArr[8] == 2)
			{
				this.EquipKongweiArr[8] = 0;
			}
			if (this.EquipKongweiArr[9] == 2)
			{
				this.EquipKongweiArr[9] = 0;
			}
			this.lockBoxs[8].isOpend = this.isLockedBoxOpened(8);
			this.lockBoxs[9].isOpend = this.isLockedBoxOpened(9);
		}
		else if (num >= 8)
		{
			if (this.EquipKongweiArr[8] == 2)
			{
				this.EquipKongweiArr[8] = 0;
			}
			this.lockBoxs[8].isOpend = this.isLockedBoxOpened(8);
		}
		this.lockBoxs[0].UpdateBox89(this.lockBoxs[8], this.lockBoxs[9]);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL[] Equips;

	public JinglingYuansuLockedBox[] lockBoxs;

	public TextBlock[] Attrs;

	public static Dictionary<int, UsedYuansuEqipData> UsedGoodsTypeDict;

	private Dictionary<int, double> EquipsPropsDict;

	private int[] PropsID = new int[]
	{
		13,
		45,
		18,
		19,
		27,
		8,
		10,
		4,
		6,
		38
	};

	private int[] EquipKongweiArr = new int[]
	{
		default(int),
		default(int),
		default(int),
		default(int),
		default(int),
		default(int),
		default(int),
		default(int),
		2,
		2
	};

	public TextBlock[] txtName;
}
