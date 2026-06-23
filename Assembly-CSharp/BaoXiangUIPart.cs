using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class BaoXiangUIPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mLblTitle.Text = Global.GetLang("宝箱");
		this.mLblDescribe.Text = Global.GetLang("开启后随机获得以下道具");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
	}

	public void RefreshTipsGoods(GoodsData gd)
	{
		int goodsID = gd.GoodsID;
		BaoXiangTipsVO baoXiangTipsVOByID = ConfigBaoXiangTips.GetBaoXiangTipsVOByID(goodsID);
		if (baoXiangTipsVOByID == null)
		{
			return;
		}
		this.mLblTitle.Text = Global.GetLang(baoXiangTipsVOByID.Name);
		List<int[]> list = null;
		if (baoXiangTipsVOByID.Type == 1)
		{
			list = this.InitGoodsList(int.Parse(baoXiangTipsVOByID.Award), "GoodsPack.xml", baoXiangTipsVOByID.Sex, baoXiangTipsVOByID.Ocuupation, baoXiangTipsVOByID.Type);
		}
		else if (baoXiangTipsVOByID.Type == 2)
		{
			list = this.InitGoodsList(int.Parse(baoXiangTipsVOByID.Award), "MonsterGoodsList.xml", baoXiangTipsVOByID.Sex, baoXiangTipsVOByID.Ocuupation, baoXiangTipsVOByID.Type);
		}
		else if (baoXiangTipsVOByID.Type == 3)
		{
			string[] array = baoXiangTipsVOByID.Award.Split(new char[]
			{
				'|'
			});
			list = new List<int[]>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length >= 7)
					{
						int[] array3 = new int[array2.Length];
						for (int j = 0; j < array2.Length; j++)
						{
							array3[j] = Global.SafeConvertToInt32(array2[j]);
						}
						if (this.AddIs(array3[0], baoXiangTipsVOByID.Sex, baoXiangTipsVOByID.Ocuupation))
						{
							list.Add(array3);
						}
					}
				}
			}
		}
		if (list == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		for (int k = 0; k < list.Count; k++)
		{
			GoodsData dummyGoodsDataMu;
			if (baoXiangTipsVOByID.Type == 1 || baoXiangTipsVOByID.Type == 3)
			{
				dummyGoodsDataMu = Global.GetDummyGoodsDataMu((list[k][0] >= 0) ? list[k][0] : 0, (list[k][3] >= 0) ? list[k][3] : 0, (list[k][4] >= 0) ? list[k][4] : 0, (list[k][6] >= 0) ? list[k][6] : 0, (list[k][5] >= 0) ? list[k][5] : 0, (list[k][2] >= 0) ? list[k][2] : 0, (list[k][1] >= 0) ? list[k][1] : 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			}
			else
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(list[k][0]);
				if (categoriyByGoodsID == 340)
				{
					dummyGoodsDataMu = Global.GetDummyGoodsDataMu((list[k][0] >= 0) ? list[k][0] : 0, (list[k][3] >= 0) ? list[k][3] : 0, (list[k][4] >= 0) ? list[k][4] : 0, (list[k][6] >= 0) ? list[k][6] : 0, (list[k][5] >= 0) ? list[k][5] : 0, (list[k][2] >= 0) ? list[k][2] : 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					dummyGoodsDataMu.WashProps = this.GetZuoQiZhuoYueValue((list[k][6] >= 0) ? list[k][6] : 0);
				}
				else
				{
					dummyGoodsDataMu = Global.GetDummyGoodsDataMu((list[k][0] >= 0) ? list[k][0] : 0, (list[k][4] >= 0) ? list[k][4] : 0, (list[k][5] >= 0) ? list[k][5] : 0, (list[k][6] >= 0) ? ((int)this.GetZhuoYueValue(list[k][6])) : 0, (list[k][3] >= 0) ? list[k][3] : 0, (list[k][2] >= 0) ? list[k][2] : 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				}
			}
			if (gd.Binding == 1)
			{
				dummyGoodsDataMu.Binding = 1;
			}
			else if (gd.Binding == 0)
			{
				dummyGoodsDataMu.Binding = 0;
			}
			this.AddGoodIcon(dummyGoodsDataMu);
		}
	}

	private double GetZhuoYueValue(int id)
	{
		XElement excellencePropertyRandomElement = Global.GetExcellencePropertyRandomElement(id);
		if (excellencePropertyRandomElement == null)
		{
			return 0.0;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(excellencePropertyRandomElement, "MAX");
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			return 0.0;
		}
		string[] array = xelementAttributeStr.Split(new char[]
		{
			','
		});
		double num = 0.0;
		for (int i = 0; i < array.Length; i++)
		{
			num += Math.Pow(2.0, Global.SafeConvertToDouble(array[i]));
		}
		return num;
	}

	private List<int> GetZuoQiZhuoYueValue(int id)
	{
		if (id == 0)
		{
			return null;
		}
		Dictionary<int, HorseSuperiorDrop> dicHorseSuperiorDrop = IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop;
		Dictionary<int, HorseSuperiorType> dicHorseSuperiorType = IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorType;
		HorseSuperiorDrop horseSuperiorDrop = dicHorseSuperiorDrop[id];
		if (horseSuperiorDrop == null)
		{
			return null;
		}
		List<int> list = new List<int>();
		this.ParseString(horseSuperiorDrop.CommonSuperiorBank, list);
		this.ParseString(horseSuperiorDrop.SeniorSuperiorBank, list);
		List<int> list2 = new List<int>();
		if (list.Count > 0)
		{
			list.Sort();
			for (int i = 0; i < list.Count; i++)
			{
				HorseSuperiorType horseSuperiorType = dicHorseSuperiorType[list[i]];
				if (horseSuperiorType != null)
				{
					int index = this.GetIndex(horseSuperiorType.Type);
					int value = this.GetValue(horseSuperiorType.Parameter);
					list2.Add(index);
					list2.Add(value);
				}
			}
		}
		return list2;
	}

	private int GetIndex(string name)
	{
		ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(name);
		if (extPropIndexesVOByWord != null)
		{
			return extPropIndexesVOByWord.ID;
		}
		return 0;
	}

	private int GetValue(string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			string[] array = name.Split(new char[]
			{
				'|'
			});
			string[] array2 = array[2].Split(new char[]
			{
				','
			});
			return Global.SafeConvertToInt32(array2[0]);
		}
		return 0;
	}

	private void ParseString(string value, List<int> valueList)
	{
		if (!string.IsNullOrEmpty(value))
		{
			this.SetListValue(value.Split(new char[]
			{
				','
			}), valueList);
		}
	}

	private void SetListValue(string[] valueStrs, List<int> valueList)
	{
		for (int i = 0; i < valueStrs.Length; i++)
		{
			valueList.Add(Global.SafeConvertToInt32(valueStrs[i]));
		}
	}

	private bool AddIs(int id, int sex, int occ)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
		return (sex != 0 || goodsXmlNodeByID.ToSex == Global.Data.roleData.RoleSex) && (occ == -1 || goodsXmlNodeByID.ToOccupation == -1 || Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == goodsXmlNodeByID.MainOccupation);
	}

	private List<int[]> InitGoodsList(int goodID, string XmlName, int sex, int occ, int Type)
	{
		XElement xelement = null;
		string attributeName = "GoodsID";
		if (Type == 2)
		{
			xelement = Global.GetIsolateResXml("Config/" + XmlName);
		}
		else if (Type == 1)
		{
			xelement = Global.GetGameResXml("Config/" + XmlName);
			attributeName = "Item";
		}
		if (xelement == null)
		{
			return null;
		}
		XElement xelement2 = Global.GetXElement(xelement, "Goods", "ID", goodID.ToString());
		if (xelement2 == null)
		{
			return null;
		}
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, attributeName);
		if (string.IsNullOrEmpty(xelementAttributeStr))
		{
			return null;
		}
		string[] array = xelementAttributeStr.Split(new char[]
		{
			'|'
		});
		List<int[]> list = new List<int[]>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length >= 7)
				{
					int[] array3 = new int[array2.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						array3[j] = Global.SafeConvertToInt32(array2[j]);
					}
					if (this.AddIs(array3[0], sex, occ))
					{
						if (list.Count <= 0 || !this.IsEqual(list, array3[0], array3[1]))
						{
							list.Add(array3);
						}
					}
				}
			}
		}
		return list;
	}

	private bool IsEqual(List<int[]> goodsList, int GoodID, int GoodsCount)
	{
		for (int i = 0; i < goodsList.Count; i++)
		{
			int[] array = goodsList[i];
			if (array[0] == GoodID && array[1] == GoodsCount)
			{
				return true;
			}
		}
		return false;
	}

	private void AddGoodIcon(GoodsData gd)
	{
		GGoodIcon icon = UIHelper.AddGoodsIcon(this.ItemCollection, gd, null, false, "bagGrid4_bak");
		if (gd.GoodsID <= 0)
		{
			icon.MouseLeftButtonUp = null;
		}
		UIDragPanelContents component = icon.transform.GetComponent<UIDragPanelContents>();
		if (component == null)
		{
			icon.gameObject.AddComponent<UIDragPanelContents>();
		}
		icon.MouseLeftButtonUp = null;
		icon.addEventListener("click", delegate(MouseEvent e)
		{
			GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
			if (null == ggoodIcon)
			{
				return;
			}
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
		});
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblTitle;

	public TextBlock mLblDescribe;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;
}
