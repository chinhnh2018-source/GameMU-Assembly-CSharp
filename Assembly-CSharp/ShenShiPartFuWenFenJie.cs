using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShenShiPartFuWenFenJie : UserControl
{
	private bool isAllSelect
	{
		set
		{
			this.XuanZhongAll.Check = value;
			TweenAlpha componentInChildren = this.XuanZhongAll.GetComponentInChildren<TweenAlpha>();
			if (componentInChildren)
			{
				if (value)
				{
					componentInChildren.from = 0f;
					componentInChildren.to = 1f;
				}
				else
				{
					componentInChildren.from = 1f;
					componentInChildren.to = 0f;
				}
				componentInChildren.enabled = true;
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this.title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("一键分解")
		});
		this.fenjieGet.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("分解获得:")
		});
		this.BtnFenJie.Label.text = Global.GetLang("分解");
		this.XuanZhongAll._Lable.text = Global.GetLang("全部");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.mOBC = this.ListItems.ItemsSource;
		this.ListItems.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectionChanged);
		this.isAllSelect = true;
		this.InitItemLevs();
		this.InitItems(this.GetDicLevGoodsItemData(1), true);
		this.inLev = 1;
		this.SetBtnActieve(this.ItemLevs[0], 1);
		UIEventListener.Get(this.ItemLevs[0]).onClick = delegate(GameObject s)
		{
			this.InitItems(this.GetDicLevGoodsItemData(1), true);
			this.inLev = 1;
			this.SetBtnActieve(this.ItemLevs[0], 1);
			this.isAllSelect = true;
		};
		UIEventListener.Get(this.ItemLevs[1]).onClick = delegate(GameObject s)
		{
			this.InitItems(this.GetDicLevGoodsItemData(2), true);
			this.inLev = 2;
			this.SetBtnActieve(this.ItemLevs[1], 2);
			this.isAllSelect = true;
		};
		UIEventListener.Get(this.ItemLevs[2]).onClick = delegate(GameObject s)
		{
			this.InitItems(this.GetDicLevGoodsItemData(3), true);
			this.inLev = 3;
			this.SetBtnActieve(this.ItemLevs[2], 3);
			this.isAllSelect = true;
		};
		UIEventListener.Get(this.ItemLevs[3]).onClick = delegate(GameObject s)
		{
			this.InitItems(this.GetDicLevGoodsItemData(4), true);
			this.inLev = 4;
			this.SetBtnActieve(this.ItemLevs[3], 4);
			this.isAllSelect = true;
		};
		this.XuanZhongAll.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			this.AllSelection();
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnFenJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			string text = null;
			Dictionary<int, string>.Enumerator enumerator = this.dicSendIDAndNum.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text2 = text;
				KeyValuePair<int, string> keyValuePair = enumerator.Current;
				text = text2 + keyValuePair.Value + ",";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
				GameInstance.Game.GetFuWenFenJie(text);
				Super.ShowNetWaiting(null);
			}
			else
			{
				Super.HintMainText(Global.GetLang("未选择符文"), 10, 3);
			}
		};
	}

	public void SetBtnActieve(GameObject btn, int index)
	{
		if (btn == this.BtnLastSelect)
		{
			return;
		}
		if (null != btn)
		{
			btn.GetComponentInChildren<UISprite>().spriteName = string.Format("btn0{0}", index);
		}
		if (null != this.BtnLastSelect)
		{
			this.BtnLastSelect.GetComponentInChildren<UISprite>().spriteName = string.Format("btn0{0}{1}", this.lastIndex, this.lastIndex);
		}
		this.BtnLastSelect = btn;
		this.lastIndex = index;
	}

	private Dictionary<int, int> ComputeUsedFuWen()
	{
		if (this.dicUsedFuWen != null && this.dicUsedFuWen.Count > 0)
		{
			this.dicUsedFuWen.Clear();
		}
		if (Global.Data.MyFuWenTabData == null || Global.Data.MyFuWenTabData.Count <= 0)
		{
			return this.dicUsedFuWen;
		}
		int i = 0;
		int count = Global.Data.MyFuWenTabData.Count;
		while (i < count)
		{
			int j = 0;
			int count2 = Global.Data.MyFuWenTabData[i].FuWenEquipList.Count;
			while (j < count2)
			{
				int num = Global.Data.MyFuWenTabData[i].FuWenEquipList[j];
				int sameIdCount = this.GetSameIdCount(i, num);
				if (this.dicUsedFuWen.ContainsKey(num))
				{
					if (sameIdCount > this.dicUsedFuWen[num])
					{
						this.dicUsedFuWen[num] = sameIdCount;
					}
				}
				else
				{
					this.dicUsedFuWen.Add(num, sameIdCount);
				}
				j++;
			}
			i++;
		}
		return this.dicUsedFuWen;
	}

	private int GetSameIdCount(int tabid, int id)
	{
		int num = 0;
		if (tabid < Global.Data.MyFuWenTabData.Count)
		{
			for (int i = 0; i < Global.Data.MyFuWenTabData[tabid].FuWenEquipList.Count; i++)
			{
				if (id == Global.Data.MyFuWenTabData[tabid].FuWenEquipList[i])
				{
					num++;
				}
			}
		}
		return num;
	}

	private Dictionary<int, int> ComputeAllFuWen()
	{
		if (this.dicAllFuWen != null && this.dicAllFuWen.Count > 0)
		{
			this.dicAllFuWen.Clear();
		}
		if (Global.Data.MyFuWenData == null || Global.Data.MyFuWenData.Count <= 0)
		{
			return this.dicAllFuWen;
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
		return this.dicAllFuWen;
	}

	private Dictionary<int, int> ComputeNoUseFuWen()
	{
		if (this.dicNoUseFuWen != null && this.dicNoUseFuWen.Count > 0)
		{
			this.dicNoUseFuWen.Clear();
		}
		this.dicNoUseFuWen = this.ComputeAllFuWen();
		Dictionary<int, int>.Enumerator enumerator = this.ComputeUsedFuWen().GetEnumerator();
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
				Dictionary<int, int> dictionary4 = this.dicNoUseFuWen;
				KeyValuePair<int, int> keyValuePair4 = enumerator.Current;
				if (dictionary4[keyValuePair4.Key] == 0)
				{
					Dictionary<int, int> dictionary5 = this.dicNoUseFuWen;
					KeyValuePair<int, int> keyValuePair5 = enumerator.Current;
					dictionary5.Remove(keyValuePair5.Key);
				}
			}
		}
		return this.dicNoUseFuWen;
	}

	private Dictionary<int, GoodsItemData> GetDicLevGoodsItemData(int lev)
	{
		if (this.dicLevGoodsItemData != null && this.dicLevGoodsItemData.Count > 0)
		{
			this.dicLevGoodsItemData.Clear();
		}
		Dictionary<int, int> dictionary = this.ComputeNoUseFuWen();
		if (dictionary == null || dictionary.Count <= 0)
		{
			return this.dicLevGoodsItemData;
		}
		Dictionary<int, int>.Enumerator enumerator = dictionary.GetEnumerator();
		Dictionary<int, FuWen> dicFuWen = ShenShiPart.GetDicFuWen();
		while (enumerator.MoveNext())
		{
			Dictionary<int, FuWen> dictionary2 = dicFuWen;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			if (dictionary2.ContainsKey(keyValuePair.Key))
			{
				Dictionary<int, FuWen> dictionary3 = dicFuWen;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				if (lev == dictionary3[keyValuePair2.Key].Level)
				{
					GoodsItemData goodsItemData = new GoodsItemData();
					GoodsItemData goodsItemData2 = goodsItemData;
					KeyValuePair<int, int> keyValuePair3 = enumerator.Current;
					goodsItemData2.goodsID = keyValuePair3.Key;
					GoodsItemData goodsItemData3 = goodsItemData;
					Dictionary<int, FuWen> dictionary4 = dicFuWen;
					KeyValuePair<int, int> keyValuePair4 = enumerator.Current;
					goodsItemData3.Name = dictionary4[keyValuePair4.Key].Name;
					GoodsItemData goodsItemData4 = goodsItemData;
					Dictionary<int, FuWen> dictionary5 = dicFuWen;
					KeyValuePair<int, int> keyValuePair5 = enumerator.Current;
					goodsItemData4.type = dictionary5[keyValuePair5.Key].Type;
					GoodsItemData goodsItemData5 = goodsItemData;
					Dictionary<int, FuWen> dictionary6 = dicFuWen;
					KeyValuePair<int, int> keyValuePair6 = enumerator.Current;
					goodsItemData5.level = dictionary6[keyValuePair6.Key].Level;
					goodsItemData.isUsed = false;
					goodsItemData.isGet = true;
					GoodsItemData goodsItemData6 = goodsItemData;
					KeyValuePair<int, int> keyValuePair7 = enumerator.Current;
					goodsItemData6.Count = keyValuePair7.Value;
					Dictionary<int, double> dictionary7 = new Dictionary<int, double>();
					KeyValuePair<int, int> keyValuePair8 = enumerator.Current;
					dictionary7 = ConfigGoods.GetDicEquipPropsByGoodsId(keyValuePair8.Key);
					Dictionary<int, double>.Enumerator enumerator2 = dictionary7.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Dictionary<int, ExtPropIndexess> dicExtPropIndexes = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
						KeyValuePair<int, double> keyValuePair9 = enumerator2.Current;
						string text;
						if (dicExtPropIndexes[keyValuePair9.Key].Percent == 0)
						{
							KeyValuePair<int, double> keyValuePair10 = enumerator2.Current;
							text = keyValuePair10.Value.ToString();
						}
						else
						{
							string text2 = "{0}%";
							KeyValuePair<int, double> keyValuePair11 = enumerator2.Current;
							text = string.Format(text2, keyValuePair11.Value * 100.0);
						}
						GoodsItemData goodsItemData7 = goodsItemData;
						string attr = goodsItemData7.Attr;
						object[] array = new object[2];
						array[0] = "dac7ae";
						int num = 1;
						string text3 = "{0} + {1}";
						Dictionary<int, ExtPropIndexess> dicExtPropIndexes2 = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
						KeyValuePair<int, double> keyValuePair12 = enumerator2.Current;
						array[num] = string.Format(text3, dicExtPropIndexes2[keyValuePair12.Key].Description, text);
						goodsItemData7.Attr = attr + Global.GetColorStringForNGUIText(array) + "\r\n";
					}
					Dictionary<int, GoodsItemData> dictionary8 = this.dicLevGoodsItemData;
					KeyValuePair<int, int> keyValuePair13 = enumerator.Current;
					dictionary8.Add(keyValuePair13.Key, goodsItemData);
				}
			}
		}
		return this.dicLevGoodsItemData;
	}

	private Dictionary<int, int> ComputeItemLevCount()
	{
		if (this.dicItemLevCount != null && this.dicItemLevCount.Count > 0)
		{
			this.dicItemLevCount.Clear();
		}
		int i = 1;
		int num = 4;
		while (i <= num)
		{
			Dictionary<int, GoodsItemData> dictionary = this.GetDicLevGoodsItemData(i);
			if (dictionary != null && dictionary.Count > 0)
			{
				Dictionary<int, GoodsItemData>.Enumerator enumerator = dictionary.GetEnumerator();
				int num2 = 0;
				while (enumerator.MoveNext())
				{
					int num3 = num2;
					KeyValuePair<int, GoodsItemData> keyValuePair = enumerator.Current;
					num2 = num3 + keyValuePair.Value.Count;
				}
				this.dicItemLevCount.Add(i, num2);
			}
			i++;
		}
		return this.dicItemLevCount;
	}

	private void InitItemLevs()
	{
		Dictionary<int, int>.Enumerator enumerator = this.ComputeItemLevCount().GetEnumerator();
		while (enumerator.MoveNext())
		{
			GameObject[] itemLevs = this.ItemLevs;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			UILabel componentInChildren = itemLevs[keyValuePair.Key - 1].GetComponentInChildren<UILabel>();
			KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
			componentInChildren.text = keyValuePair2.Value.ToString();
		}
	}

	public void ReInitItems()
	{
		this.InitItemLevs();
		this.InitItems(this.GetDicLevGoodsItemData(this.inLev), false);
	}

	private void InitItems(Dictionary<int, GoodsItemData> dicData, bool selec = true)
	{
		this.fenjieGetChen = 0;
		if (this.mOBC != null)
		{
			this.mOBC.Clear();
		}
		if (this.dicSendIDAndNum != null && this.dicSendIDAndNum.Count > 0)
		{
			this.dicSendIDAndNum.Clear();
		}
		Dictionary<int, GoodsItemData>.Enumerator enumerator = dicData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ShenShiPartFuWenZongLanItem item = U3DUtils.NEW<ShenShiPartFuWenZongLanItem>();
			ShenShiPartFuWenZongLanItem item7 = item;
			KeyValuePair<int, GoodsItemData> keyValuePair = enumerator.Current;
			item7.goodID = keyValuePair.Key;
			ShenShiPartFuWenZongLanItem item2 = item;
			KeyValuePair<int, GoodsItemData> keyValuePair2 = enumerator.Current;
			item2.isGet = keyValuePair2.Value.isGet;
			ShenShiPartFuWenZongLanItem item3 = item;
			KeyValuePair<int, GoodsItemData> keyValuePair3 = enumerator.Current;
			bool isUsed;
			if (keyValuePair3.Value.isGet)
			{
				KeyValuePair<int, GoodsItemData> keyValuePair4 = enumerator.Current;
				isUsed = keyValuePair4.Value.isUsed;
			}
			else
			{
				isUsed = false;
			}
			item3.isUsed = isUsed;
			ShenShiPartFuWenZongLanItem item4 = item;
			KeyValuePair<int, GoodsItemData> keyValuePair5 = enumerator.Current;
			item4.Name = keyValuePair5.Value.Name;
			ShenShiPartFuWenZongLanItem item5 = item;
			KeyValuePair<int, GoodsItemData> keyValuePair6 = enumerator.Current;
			item5.Count = keyValuePair6.Value.Count;
			ShenShiPartFuWenZongLanItem item6 = item;
			KeyValuePair<int, GoodsItemData> keyValuePair7 = enumerator.Current;
			item6.Attr = keyValuePair7.Value.Attr;
			item.XuanZhong = selec;
			item.Handler = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (!item.XuanZhong)
				{
					this.fenjieGetChen -= item.Count * ShenShiPart.GetDicFuWen()[item.goodID].SendNum;
				}
				else
				{
					this.fenjieGetChen += item.Count * ShenShiPart.GetDicFuWen()[item.goodID].SendNum;
				}
				this.fenjieGetNum.text = this.fenjieGetChen.ToString();
				if (this.dicSendIDAndNum.ContainsKey(item.goodID))
				{
					this.dicSendIDAndNum.Remove(item.goodID);
				}
				else
				{
					string text3 = string.Format("{0},{1}", item.goodID, item.Count);
					this.dicSendIDAndNum.Add(item.goodID, text3);
				}
				int i = 0;
				int count2 = this.mOBC.Count;
				while (i < count2)
				{
					ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem = U3DUtils.AS<ShenShiPartFuWenZongLanItem>(this.mOBC[i].gameObject);
					if (shenShiPartFuWenZongLanItem != null && !shenShiPartFuWenZongLanItem.XuanZhong)
					{
						this.isAllSelect = false;
						return;
					}
					i++;
				}
				this.isAllSelect = true;
			};
			this.mOBC.AddNoUpdate(item);
			if (selec)
			{
				int num = this.fenjieGetChen;
				KeyValuePair<int, GoodsItemData> keyValuePair8 = enumerator.Current;
				int count = keyValuePair8.Value.Count;
				Dictionary<int, FuWen> dicFuWen = ShenShiPart.GetDicFuWen();
				KeyValuePair<int, GoodsItemData> keyValuePair9 = enumerator.Current;
				this.fenjieGetChen = num + count * dicFuWen[keyValuePair9.Key].SendNum;
				string text = "{0},{1}";
				KeyValuePair<int, GoodsItemData> keyValuePair10 = enumerator.Current;
				object obj = keyValuePair10.Key;
				KeyValuePair<int, GoodsItemData> keyValuePair11 = enumerator.Current;
				string text2 = string.Format(text, obj, keyValuePair11.Value.Count);
				Dictionary<int, string> dictionary = this.dicSendIDAndNum;
				KeyValuePair<int, GoodsItemData> keyValuePair12 = enumerator.Current;
				dictionary.Add(keyValuePair12.Key, text2);
			}
			UIPanel component = item.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
		this.fenjieGetNum.text = this.fenjieGetChen.ToString();
	}

	private void ChackAllSelection()
	{
		int i = 0;
		int count = this.mOBC.Count;
		while (i < count)
		{
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem = U3DUtils.AS<ShenShiPartFuWenZongLanItem>(this.mOBC[i].gameObject);
			if (!shenShiPartFuWenZongLanItem.XuanZhong)
			{
				return;
			}
			i++;
		}
	}

	private void AllSelection()
	{
		this.fenjieGetChen = 0;
		bool check = this.XuanZhongAll.Check;
		if (!check)
		{
			this.fenjieGetChen = 0;
		}
		this.dicSendIDAndNum.Clear();
		int i = 0;
		int count = this.mOBC.Count;
		while (i < count)
		{
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem = U3DUtils.AS<ShenShiPartFuWenZongLanItem>(this.mOBC[i].gameObject);
			if (shenShiPartFuWenZongLanItem)
			{
				shenShiPartFuWenZongLanItem.XuanZhong = check;
				if (check)
				{
					if (!this.dicSendIDAndNum.ContainsKey(shenShiPartFuWenZongLanItem.goodID))
					{
						string text = string.Format("{0},{1}", shenShiPartFuWenZongLanItem.goodID, shenShiPartFuWenZongLanItem.Count);
						this.dicSendIDAndNum.Add(shenShiPartFuWenZongLanItem.goodID, text);
					}
					this.fenjieGetChen += shenShiPartFuWenZongLanItem.Count * ShenShiPart.GetDicFuWen()[shenShiPartFuWenZongLanItem.goodID].SendNum;
				}
			}
			i++;
		}
		this.fenjieGetNum.text = this.fenjieGetChen.ToString();
	}

	private void SelectionChanged(object sender, EventArgs e)
	{
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public GButton BtnFenJie;

	public GCheckBox XuanZhongAll;

	public GameObject[] ItemLevs;

	public UILabel title;

	public UILabel fenjieGet;

	public UILabel fenjieGetNum;

	public ListBox ListItems;

	private ObservableCollection mOBC;

	private int inLev;

	private int fenjieGetChen;

	private GameObject BtnLastSelect;

	private int lastIndex;

	private Dictionary<int, int> dicUsedFuWen = new Dictionary<int, int>();

	private Dictionary<int, int> dicAllFuWen = new Dictionary<int, int>();

	private Dictionary<int, int> dicNoUseFuWen = new Dictionary<int, int>();

	private Dictionary<int, GoodsItemData> dicLevGoodsItemData = new Dictionary<int, GoodsItemData>();

	private Dictionary<int, int> dicItemLevCount = new Dictionary<int, int>();

	private Dictionary<int, string> dicSendIDAndNum = new Dictionary<int, string>();
}
