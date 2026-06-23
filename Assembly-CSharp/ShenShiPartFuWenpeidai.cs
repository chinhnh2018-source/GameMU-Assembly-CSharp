using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShenShiPartFuWenpeidai : UserControl
{
	public int FuWenGoodsID
	{
		get
		{
			return this.fuwengoodsid;
		}
		set
		{
			this.fuwengoodsid = value;
		}
	}

	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
			if (value < 8)
			{
				this.FuWenType = "Blue";
			}
			else if (value > 15)
			{
				this.FuWenType = "Red";
			}
			else
			{
				this.FuWenType = "Green";
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mOBC = this.fuwenItems.ItemsSource;
		this.fuwenItems.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListBoxSelectChange);
		this.Btnclose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnSure.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			bool flag = true;
			string lang = Global.GetLang("镶嵌");
			string lang2 = Global.GetLang("卸下");
			if (!lang.Equals(this.BtnSure.Label.text.ToString()) && ShenShiPart.GetDicFuWen().ContainsKey(this.FuWenChangeGoodsID))
			{
				this.Mblue -= ShenShiPart.GetDicFuWen()[this.FuWenGoodsID].Blue;
				this.Mred -= ShenShiPart.GetDicFuWen()[this.FuWenGoodsID].Red;
				this.Mgreen -= ShenShiPart.GetDicFuWen()[this.FuWenGoodsID].Green;
				if (!lang2.Equals(this.BtnSure.Label.text.ToString()))
				{
					this.Mblue += ShenShiPart.GetDicFuWen()[this.FuWenChangeGoodsID].Blue;
					this.Mred += ShenShiPart.GetDicFuWen()[this.FuWenChangeGoodsID].Red;
					this.Mgreen += ShenShiPart.GetDicFuWen()[this.FuWenChangeGoodsID].Green;
				}
				if (Global.Data.MyFuWenTabData != null && this.FenWenTabId < Global.Data.MyFuWenTabData.Count && Global.Data.MyFuWenTabData[this.FenWenTabId].ShenShiActiveList != null)
				{
					int i = 0;
					int count = Global.Data.MyFuWenTabData[this.FenWenTabId].ShenShiActiveList.Count;
					while (i < count)
					{
						int num = Global.Data.MyFuWenTabData[this.FenWenTabId].ShenShiActiveList[i] / 100;
						int num2 = Global.Data.MyFuWenTabData[this.FenWenTabId].ShenShiActiveList[i] % 100;
						if (this.Mblue < ShenShiPart.GetDicFuWenGod()[num][num2].NeedBlue || this.Mred < ShenShiPart.GetDicFuWenGod()[num][num2].NeedRed || this.Mgreen < ShenShiPart.GetDicFuWenGod()[num][num2].NeedGreen)
						{
							string[] buttons = new string[]
							{
								Global.GetLang("确认"),
								Global.GetLang("取消")
							};
							Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("更换符文会影响已激活神识，确定要替换吗？"), delegate(object a, DPSelectedItemEventArgs b)
							{
								if (b.ID == 0)
								{
									string lang3 = Global.GetLang("卸下");
									if (lang3.Equals(this.BtnSure.Label.text.ToString()))
									{
										this.FuWenChangeGoodsID = 0;
									}
									GameInstance.Game.GetFuWenModEquip(this.FenWenTabId, this.Index, this.FuWenChangeGoodsID);
									Super.ShowNetWaiting(null);
									this.CloseHandler(this, new DPSelectedItemEventArgs());
								}
							}, buttons);
							flag = false;
							break;
						}
						i++;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			if (lang2.Equals(this.BtnSure.Label.text.ToString()))
			{
				this.FuWenChangeGoodsID = 0;
			}
			GameInstance.Game.GetFuWenModEquip(this.FenWenTabId, this.Index, this.FuWenChangeGoodsID);
			Super.ShowNetWaiting(null);
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	private void InitFuWenAttr(int GoodID)
	{
		this.fuwenTuBiao.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", GoodID);
		this.fuwenName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			ShenShiPart.GetDicFuWen()[GoodID].Name
		});
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		dictionary = ConfigGoods.GetDicEquipPropsByGoodsId(GoodID);
		Dictionary<int, double>.Enumerator enumerator = dictionary.GetEnumerator();
		this.fuwenAttr.text = null;
		while (enumerator.MoveNext())
		{
			Dictionary<int, ExtPropIndexess> dicExtPropIndexes = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
			KeyValuePair<int, double> keyValuePair = enumerator.Current;
			string text;
			if (dicExtPropIndexes[keyValuePair.Key].Percent == 0)
			{
				KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
				text = keyValuePair2.Value.ToString();
			}
			else
			{
				string text2 = "{0}%";
				KeyValuePair<int, double> keyValuePair3 = enumerator.Current;
				text = string.Format(text2, keyValuePair3.Value * 100.0);
			}
			UILabel uilabel = this.fuwenAttr;
			string text3 = uilabel.text;
			object[] array = new object[2];
			array[0] = "dac7ae";
			int num = 1;
			string text4 = "{0} + {1}";
			Dictionary<int, ExtPropIndexess> dicExtPropIndexes2 = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
			KeyValuePair<int, double> keyValuePair4 = enumerator.Current;
			array[num] = string.Format(text4, dicExtPropIndexes2[keyValuePair4.Key].Description, text);
			uilabel.text = text3 + Global.GetColorStringForNGUIText(array) + "\r\n";
		}
		string text5 = null;
		int num2 = 0;
		int blue = ShenShiPart.GetDicFuWen()[GoodID].Blue;
		int red = ShenShiPart.GetDicFuWen()[GoodID].Red;
		int green = ShenShiPart.GetDicFuWen()[GoodID].Green;
		string text6 = "FF0000";
		if (blue != 0)
		{
			text5 = Global.GetLang("守序");
			num2 = blue;
			text6 = "3681f3";
		}
		else if (red != 0)
		{
			text5 = Global.GetLang("混乱");
			num2 = red;
			text6 = "FF0000";
		}
		else if (green != 0)
		{
			text5 = Global.GetLang("平衡");
			num2 = green;
			text6 = "17e43e";
		}
		this.attrJiaCheng.text = Global.GetColorStringForNGUIText(new object[]
		{
			text6,
			string.Format(Global.GetLang("{0}神识 + {1}"), text5, num2)
		});
	}

	private void ComputeUsedFuWen()
	{
		if (Global.Data.MyFuWenTabData == null || Global.Data.MyFuWenTabData.Count <= 0)
		{
			return;
		}
		if (this.FenWenTabId >= Global.Data.MyFuWenTabData.Count)
		{
			return;
		}
		if (Global.Data.MyFuWenTabData[this.FenWenTabId].FuWenEquipList.Count <= 0)
		{
			return;
		}
		int i = 0;
		int count = Global.Data.MyFuWenTabData[this.FenWenTabId].FuWenEquipList.Count;
		while (i < count)
		{
			int num = Global.Data.MyFuWenTabData[this.FenWenTabId].FuWenEquipList[i];
			if (this.dicUsedFuWen.ContainsKey(num))
			{
				Dictionary<int, int> dictionary2;
				Dictionary<int, int> dictionary = dictionary2 = this.dicUsedFuWen;
				int num3;
				int num2 = num3 = num;
				num3 = dictionary2[num3];
				dictionary[num2] = num3 + 1;
			}
			else
			{
				this.dicUsedFuWen.Add(num, 1);
			}
			i++;
		}
	}

	private void ComputeAllFuWen()
	{
		if (Global.Data.MyFuWenData == null || Global.Data.MyFuWenData.Count <= 0)
		{
			return;
		}
		int i = 0;
		int count = Global.Data.MyFuWenData.Count;
		while (i < count)
		{
			GoodsData goodsData = Global.Data.MyFuWenData[i];
			if (this.dicAllFuWen.ContainsKey(goodsData.GoodsID))
			{
				Dictionary<int, int> dictionary2;
				Dictionary<int, int> dictionary = dictionary2 = this.dicAllFuWen;
				int num2;
				int num = num2 = goodsData.GoodsID;
				num2 = dictionary2[num2];
				dictionary[num] = num2 + goodsData.GCount;
			}
			else
			{
				this.dicAllFuWen.Add(goodsData.GoodsID, goodsData.GCount);
			}
			i++;
		}
	}

	private void ComputeNoUseFuWen()
	{
		this.dicNoUseFuWen = this.dicAllFuWen;
		if (this.dicUsedFuWen == null || this.dicUsedFuWen.Count <= 0)
		{
			return;
		}
		Dictionary<int, int>.Enumerator enumerator = this.dicUsedFuWen.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Dictionary<int, int> dictionary = this.dicNoUseFuWen;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			if (dictionary.ContainsKey(keyValuePair.Key))
			{
				Dictionary<int, int> dictionary3;
				Dictionary<int, int> dictionary2 = dictionary3 = this.dicNoUseFuWen;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				int num2;
				int num = num2 = keyValuePair2.Key;
				num2 = dictionary3[num2];
				int num3 = num2;
				KeyValuePair<int, int> keyValuePair3 = enumerator.Current;
				dictionary2[num] = num3 - keyValuePair3.Value;
			}
		}
	}

	private void ComputeNoUseSameTypeFuWen(Dictionary<int, int> dicNoUseFuWen)
	{
		Dictionary<int, int>.Enumerator enumerator = dicNoUseFuWen.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Dictionary<int, FuWen> dicFuWen = ShenShiPart.GetDicFuWen();
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			if (dicFuWen.ContainsKey(keyValuePair.Key))
			{
				Dictionary<int, FuWen> dicFuWen2 = ShenShiPart.GetDicFuWen();
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				string type = dicFuWen2[keyValuePair2.Key].Type;
				if (this.FuWenType != null && this.FuWenType.Equals(type))
				{
					Dictionary<int, int> dictionary = this.dicNoUseSameTypeFuWen;
					KeyValuePair<int, int> keyValuePair3 = enumerator.Current;
					if (!dictionary.ContainsKey(keyValuePair3.Key))
					{
						Dictionary<int, int> dictionary2 = this.dicNoUseSameTypeFuWen;
						KeyValuePair<int, int> keyValuePair4 = enumerator.Current;
						int key = keyValuePair4.Key;
						KeyValuePair<int, int> keyValuePair5 = enumerator.Current;
						dictionary2.Add(key, keyValuePair5.Value);
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"计算未使用的符文出错，存在相同项"
						});
					}
				}
			}
		}
	}

	private void SortNoUseSameTypeFuWen(Dictionary<int, int> dic)
	{
		this.listNoUseSameTypeFuWen = Enumerable.ToList<int>(dic.Keys).ConvertAll<FuWen>(delegate(int _g)
		{
			FuWen fuWen = new FuWen();
			return ShenShiPart.GetDicFuWen()[_g];
		});
		this.listNoUseSameTypeFuWen.Sort(delegate(FuWen X, FuWen y)
		{
			if (X.Level == y.Level)
			{
				return y.ID.CompareTo(X.ID);
			}
			return y.Level.CompareTo(X.Level);
		});
	}

	public void InitFuWenItems()
	{
		if (this.FenWenTabId >= Global.Data.MyFuWenTabData.Count)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
			Super.HintMainText(Global.GetLang("当前符文页不可佩戴"), 10, 3);
			return;
		}
		this.ComputeUsedFuWen();
		this.ComputeAllFuWen();
		this.ComputeNoUseFuWen();
		this.ComputeNoUseSameTypeFuWen(this.dicNoUseFuWen);
		this.SortNoUseSameTypeFuWen(this.dicNoUseSameTypeFuWen);
		if (this.dicNoUseSameTypeFuWen == null && this.dicNoUseSameTypeFuWen.Count <= 0)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedFuWen, null, string.Empty, string.Empty);
			this.CloseHandler(this, new DPSelectedItemEventArgs());
			Super.HintMainText(Global.GetLang("无可佩戴符文"), 10, 3);
			return;
		}
		if (this.FuWenGoodsID == 0)
		{
			bool flag = true;
			int i = 0;
			int count = this.listNoUseSameTypeFuWen.Count;
			while (i < count)
			{
				if (this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[i].GoodsID] > 0)
				{
					flag = false;
					break;
				}
				i++;
			}
			if (flag)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedFuWen, null, string.Empty, string.Empty);
				this.CloseHandler(this, new DPSelectedItemEventArgs());
				Super.HintMainText(Global.GetLang("无可佩戴符文"), 10, 3);
				return;
			}
		}
		if (this.FuWenGoodsID == 0)
		{
			this.BtnSure.Label.text = Global.GetLang("镶嵌");
			this.noUsed = true;
			int j = 0;
			int count2 = this.listNoUseSameTypeFuWen.Count;
			while (j < count2)
			{
				if (this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[j].GoodsID] > 0)
				{
					ShenShiPartFuWenpeidaiitem shenShiPartFuWenpeidaiitem = U3DUtils.NEW<ShenShiPartFuWenpeidaiitem>();
					shenShiPartFuWenpeidaiitem.goodsID = this.listNoUseSameTypeFuWen[j].GoodsID;
					shenShiPartFuWenpeidaiitem.Count = this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[j].GoodsID];
					shenShiPartFuWenpeidaiitem.setPeiDai = false;
					this.mOBC.AddNoUpdate(shenShiPartFuWenpeidaiitem);
				}
				j++;
			}
		}
		else
		{
			this.BtnSure.Label.text = Global.GetLang("卸下");
			int k = 0;
			int count3 = this.listNoUseSameTypeFuWen.Count;
			while (k < count3)
			{
				if (this.FuWenGoodsID == this.listNoUseSameTypeFuWen[k].GoodsID)
				{
					ShenShiPartFuWenpeidaiitem shenShiPartFuWenpeidaiitem2 = U3DUtils.NEW<ShenShiPartFuWenpeidaiitem>();
					shenShiPartFuWenpeidaiitem2.goodsID = this.listNoUseSameTypeFuWen[k].GoodsID;
					if (this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[k].GoodsID] > 0)
					{
						shenShiPartFuWenpeidaiitem2.Count = this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[k].GoodsID];
					}
					else
					{
						shenShiPartFuWenpeidaiitem2.Count = 1;
					}
					shenShiPartFuWenpeidaiitem2.setPeiDai = true;
					this.mOBC.AddNoUpdate(shenShiPartFuWenpeidaiitem2);
					this.listNoUseSameTypeFuWen.RemoveAt(k);
					break;
				}
				k++;
			}
			int l = 0;
			int count4 = this.listNoUseSameTypeFuWen.Count;
			while (l < count4)
			{
				if (this.FuWenGoodsID != this.listNoUseSameTypeFuWen[l].GoodsID)
				{
					if (this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[l].GoodsID] > 0)
					{
						ShenShiPartFuWenpeidaiitem shenShiPartFuWenpeidaiitem3 = U3DUtils.NEW<ShenShiPartFuWenpeidaiitem>();
						shenShiPartFuWenpeidaiitem3.goodsID = this.listNoUseSameTypeFuWen[l].GoodsID;
						shenShiPartFuWenpeidaiitem3.Count = this.dicNoUseSameTypeFuWen[this.listNoUseSameTypeFuWen[l].GoodsID];
						shenShiPartFuWenpeidaiitem3.setPeiDai = false;
						this.mOBC.AddNoUpdate(shenShiPartFuWenpeidaiitem3);
					}
				}
				l++;
			}
		}
		ShenShiPartFuWenpeidaiitem shenShiPartFuWenpeidaiitem4 = U3DUtils.AS<ShenShiPartFuWenpeidaiitem>(this.mOBC[0]);
		if (shenShiPartFuWenpeidaiitem4)
		{
			this.InitFuWenAttr(shenShiPartFuWenpeidaiitem4.goodsID);
			shenShiPartFuWenpeidaiitem4.XuanZhong = true;
			this.FuWenChangeGoodsID = shenShiPartFuWenpeidaiitem4.goodsID;
			this.fuwenItems.SelectedIndex = 0;
		}
	}

	private void ListBoxSelectChange(object sender, MouseEvent e)
	{
		ShenShiPartFuWenpeidaiitem shenShiPartFuWenpeidaiitem = null;
		GameObject selectedItem = this.fuwenItems.SelectedItem;
		if (null != selectedItem)
		{
			shenShiPartFuWenpeidaiitem = selectedItem.GetComponent<ShenShiPartFuWenpeidaiitem>();
			if (null != shenShiPartFuWenpeidaiitem)
			{
				shenShiPartFuWenpeidaiitem.XuanZhong = true;
			}
		}
		GameObject lastSelectedItem = this.fuwenItems.LastSelectedItem;
		if (null != lastSelectedItem && lastSelectedItem != selectedItem)
		{
			ShenShiPartFuWenpeidaiitem component = lastSelectedItem.GetComponent<ShenShiPartFuWenpeidaiitem>();
			if (null != component)
			{
				component.XuanZhong = false;
			}
		}
		if (shenShiPartFuWenpeidaiitem == null)
		{
			return;
		}
		if (this.FuWenGoodsID == 0 || this.noUsed)
		{
			this.BtnSure.Label.text = Global.GetLang("镶嵌");
		}
		else if (this.FuWenGoodsID == shenShiPartFuWenpeidaiitem.goodsID)
		{
			this.BtnSure.Label.text = Global.GetLang("卸下");
		}
		else
		{
			this.BtnSure.Label.text = Global.GetLang("替换");
		}
		this.InitFuWenAttr(shenShiPartFuWenpeidaiitem.goodsID);
		this.FuWenChangeGoodsID = shenShiPartFuWenpeidaiitem.goodsID;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton Btnclose;

	public GButton BtnSure;

	public ShowNetImage fuwenTuBiao;

	public UILabel fuwenName;

	public UILabel fuwenAttr;

	public UILabel fuwenJiaCheng;

	public UILabel attrJiaCheng;

	public ListBox fuwenItems;

	public int Mblue;

	public int Mred;

	public int Mgreen;

	public int FenWenTabId;

	private int fuwengoodsid;

	private int index;

	public string FuWenType;

	private int FuWenChangeGoodsID;

	private ObservableCollection mOBC;

	private Dictionary<int, int> dicUsedFuWen = new Dictionary<int, int>();

	private Dictionary<int, int> dicAllFuWen = new Dictionary<int, int>();

	private Dictionary<int, int> dicNoUseFuWen = new Dictionary<int, int>();

	private Dictionary<int, int> dicNoUseSameTypeFuWen = new Dictionary<int, int>();

	private List<FuWen> listNoUseSameTypeFuWen = new List<FuWen>();

	private bool noUsed;
}
