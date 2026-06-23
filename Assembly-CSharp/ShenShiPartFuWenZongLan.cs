using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShenShiPartFuWenZongLan : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnAll.Label.text = Global.GetLang("所有");
		this.BtnHunLuan.Label.text = Global.GetLang("混乱");
		this.BtnZhiXu.Label.text = Global.GetLang("秩序");
		this.BtnPingHeng.Label.text = Global.GetLang("平衡");
		this.BtnFenJie.Label.text = Global.GetLang("一键分解");
		this.BtnAll.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnHunLuan.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnZhiXu.Label.color = NGUIMath.HexToColorEx(8350293U);
		this.BtnPingHeng.Label.color = NGUIMath.HexToColorEx(8350293U);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mOBC = this.FuWenItems.ItemsSource;
		this.FuWenItems.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.InitTextInPrefabs();
		this.serchType = 0;
		this.serchLevel = 1;
		Dictionary<int, GoodsItemData> dics = this.GetDicAllGoodsItemData();
		Dictionary<int, GoodsItemData> dicData = this.GetDicTypeLevelGoodsItemData(dics, 1, false);
		this.InitItems(dicData);
		this.BtnAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.serchType = 0;
			Dictionary<int, GoodsItemData> dics2 = this.GetDicAllGoodsItemData();
			Dictionary<int, GoodsItemData> dicData2 = this.GetDicTypeLevelGoodsItemData(dics2, this.serchLevel, false);
			this.InitItems(dicData2);
			this.SetBtnActieve(this.BtnAll);
			this.SetBtnActieve(this.BtnLevs[this.serchLevel - 1], this.serchLevel);
		};
		this.BtnHunLuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.serchType = 1;
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicTypeGoodsItemData("Red"), this.serchLevel, false));
			this.SetBtnActieve(this.BtnHunLuan);
			this.SetBtnActieve(this.BtnLevs[this.serchLevel - 1], this.serchLevel);
		};
		this.BtnZhiXu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.serchType = 2;
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicTypeGoodsItemData("Blue"), this.serchLevel, false));
			this.SetBtnActieve(this.BtnZhiXu);
			this.SetBtnActieve(this.BtnLevs[this.serchLevel - 1], this.serchLevel);
		};
		this.BtnPingHeng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.serchType = 3;
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicTypeGoodsItemData("Green"), this.serchLevel, false));
			this.SetBtnActieve(this.BtnPingHeng);
			this.SetBtnActieve(this.BtnLevs[this.serchLevel - 1], this.serchLevel);
		};
		this.BtnLevs[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(1);
			this.serchLevel = 1;
			this.SetBtnActieve(this.BtnLevs[0], 1);
		};
		this.BtnLevs[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(2);
			this.serchLevel = 2;
			this.SetBtnActieve(this.BtnLevs[1], 2);
		};
		this.BtnLevs[2].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(3);
			this.serchLevel = 3;
			this.SetBtnActieve(this.BtnLevs[2], 3);
		};
		this.BtnLevs[3].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(4);
			this.serchLevel = 4;
			this.SetBtnActieve(this.BtnLevs[3], 4);
		};
		this.BtnLevs[4].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(5);
			this.serchLevel = 5;
			this.SetBtnActieve(this.BtnLevs[4], 5);
		};
		this.BtnLevs[5].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(6);
			this.serchLevel = 6;
			this.SetBtnActieve(this.BtnLevs[5], 6);
		};
		this.BtnLevs[6].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SerchLevelItem(7);
			this.serchLevel = 7;
			this.SetBtnActieve(this.BtnLevs[6], 7);
		};
		int num = (int)ConfigSystemParam.GetSystemParamIntByName("FuWenSevenOpen");
		if (num == 1)
		{
			this.BtnLevs[6].gameObject.SetActive(true);
		}
		else
		{
			this.BtnLevs[6].gameObject.SetActive(false);
		}
		this.BtnFenJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenShenShiPartFuWenFenJiePartWindow();
		};
		this.SetBtnActieve(this.BtnAll);
		this.SetBtnActieve(this.BtnLevs[0], 1);
	}

	private void SerchLevelItem(int lev)
	{
		if (this.serchType == 0)
		{
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicAllGoodsItemData(), lev, lev >= this.showMaxLevel));
		}
		else if (this.serchType == 1)
		{
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicTypeGoodsItemData("Red"), lev, lev >= this.showMaxLevel));
		}
		else if (this.serchType == 2)
		{
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicTypeGoodsItemData("Blue"), lev, lev >= this.showMaxLevel));
		}
		else if (this.serchType == 3)
		{
			this.InitItems(this.GetDicTypeLevelGoodsItemData(this.GetDicTypeGoodsItemData("Green"), lev, lev >= this.showMaxLevel));
		}
	}

	public void ReInitItems()
	{
		this.SerchLevelItem(this.serchLevel);
	}

	public void SetBtnActieve(GButton btn)
	{
		if (btn == this.m_BtnLastSelect)
		{
			return;
		}
		if (null != btn)
		{
			btn.Label.color = NGUIMath.HexToColorEx(16309634U);
			btn.normalSprite = "tab2";
			btn.Refresh();
		}
		if (null != this.m_BtnLastSelect)
		{
			this.m_BtnLastSelect.Label.color = NGUIMath.HexToColorEx(8350293U);
			this.m_BtnLastSelect.normalSprite = "tab1";
			this.m_BtnLastSelect.Refresh();
		}
		this.m_BtnLastSelect = btn;
	}

	public void SetBtnActieve(GButton btn, int index)
	{
		if (btn == this.BtnLastSelect)
		{
			return;
		}
		if (null != btn)
		{
			btn.normalSprite = string.Format("btnl{0}", index);
			btn.Refresh();
		}
		if (null != this.BtnLastSelect)
		{
			this.BtnLastSelect.normalSprite = string.Format("btnl{0}{1}", this.lastIndex, this.lastIndex);
			this.BtnLastSelect.Refresh();
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
				j++;
			}
			i++;
		}
		return this.dicUsedFuWen;
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

	private Dictionary<int, GoodsItemData> GetDicAllGoodsItemData()
	{
		if (this.dicAllGoodsItemData != null && this.dicAllGoodsItemData.Count > 0)
		{
			this.dicAllGoodsItemData.Clear();
		}
		if (ShenShiPart.GetDicFuWen() == null)
		{
			return null;
		}
		Dictionary<int, FuWen>.Enumerator enumerator = ShenShiPart.GetDicFuWen().GetEnumerator();
		while (enumerator.MoveNext())
		{
			GoodsItemData goodsItemData = new GoodsItemData();
			GoodsItemData goodsItemData2 = goodsItemData;
			KeyValuePair<int, FuWen> keyValuePair = enumerator.Current;
			goodsItemData2.goodsID = keyValuePair.Key;
			GoodsItemData goodsItemData3 = goodsItemData;
			KeyValuePair<int, FuWen> keyValuePair2 = enumerator.Current;
			goodsItemData3.Name = keyValuePair2.Value.Name;
			GoodsItemData goodsItemData4 = goodsItemData;
			KeyValuePair<int, FuWen> keyValuePair3 = enumerator.Current;
			goodsItemData4.type = keyValuePair3.Value.Type;
			GoodsItemData goodsItemData5 = goodsItemData;
			KeyValuePair<int, FuWen> keyValuePair4 = enumerator.Current;
			goodsItemData5.level = keyValuePair4.Value.Level;
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			KeyValuePair<int, FuWen> keyValuePair5 = enumerator.Current;
			dictionary = ConfigGoods.GetDicEquipPropsByGoodsId(keyValuePair5.Key);
			Dictionary<int, double>.Enumerator enumerator2 = dictionary.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Dictionary<int, ExtPropIndexess> dicExtPropIndexes = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
				KeyValuePair<int, double> keyValuePair6 = enumerator2.Current;
				string text;
				if (dicExtPropIndexes[keyValuePair6.Key].Percent == 0)
				{
					KeyValuePair<int, double> keyValuePair7 = enumerator2.Current;
					text = keyValuePair7.Value.ToString();
				}
				else
				{
					string text2 = "{0}%";
					KeyValuePair<int, double> keyValuePair8 = enumerator2.Current;
					text = string.Format(text2, keyValuePair8.Value * 100.0);
				}
				GoodsItemData goodsItemData6 = goodsItemData;
				string attr = goodsItemData6.Attr;
				object[] array = new object[2];
				array[0] = "dac7ae";
				int num = 1;
				string text3 = "{0} + {1}";
				Dictionary<int, ExtPropIndexess> dicExtPropIndexes2 = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
				KeyValuePair<int, double> keyValuePair9 = enumerator2.Current;
				array[num] = string.Format(text3, dicExtPropIndexes2[keyValuePair9.Key].Description, text);
				goodsItemData6.Attr = attr + Global.GetColorStringForNGUIText(array) + "\r\n";
			}
			string text4 = null;
			KeyValuePair<int, FuWen> keyValuePair10 = enumerator.Current;
			if (keyValuePair10.Value.Blue > 0)
			{
				string lang = Global.GetLang("守序神识 + {0} \r\n");
				KeyValuePair<int, FuWen> keyValuePair11 = enumerator.Current;
				text4 = string.Format(lang, keyValuePair11.Value.Blue);
			}
			else
			{
				KeyValuePair<int, FuWen> keyValuePair12 = enumerator.Current;
				if (keyValuePair12.Value.Red > 0)
				{
					string lang2 = Global.GetLang("混乱神识 + {0} \r\n");
					KeyValuePair<int, FuWen> keyValuePair13 = enumerator.Current;
					text4 = string.Format(lang2, keyValuePair13.Value.Red);
				}
				else
				{
					KeyValuePair<int, FuWen> keyValuePair14 = enumerator.Current;
					if (keyValuePair14.Value.Green > 0)
					{
						string lang3 = Global.GetLang("平衡神识 + {0} \r\n");
						KeyValuePair<int, FuWen> keyValuePair15 = enumerator.Current;
						text4 = string.Format(lang3, keyValuePair15.Value.Green);
					}
				}
			}
			GoodsItemData goodsItemData7 = goodsItemData;
			goodsItemData7.Attr += text4;
			goodsItemData.Attr.TrimEnd(new char[]
			{
				'\n',
				'\r'
			});
			Dictionary<int, int> dictionary2 = this.ComputeAllFuWen();
			KeyValuePair<int, FuWen> keyValuePair16 = enumerator.Current;
			if (dictionary2.ContainsKey(keyValuePair16.Key))
			{
				goodsItemData.isGet = true;
				GoodsItemData goodsItemData8 = goodsItemData;
				Dictionary<int, int> dictionary3 = this.ComputeAllFuWen();
				KeyValuePair<int, FuWen> keyValuePair17 = enumerator.Current;
				goodsItemData8.Count = dictionary3[keyValuePair17.Key];
			}
			Dictionary<int, int> dictionary4 = this.ComputeUsedFuWen();
			KeyValuePair<int, FuWen> keyValuePair18 = enumerator.Current;
			if (dictionary4.ContainsKey(keyValuePair18.Key))
			{
				goodsItemData.isUsed = true;
			}
			Dictionary<int, GoodsItemData> dictionary5 = this.dicAllGoodsItemData;
			KeyValuePair<int, FuWen> keyValuePair19 = enumerator.Current;
			if (!dictionary5.ContainsKey(keyValuePair19.Key))
			{
				Dictionary<int, GoodsItemData> dictionary6 = this.dicAllGoodsItemData;
				KeyValuePair<int, FuWen> keyValuePair20 = enumerator.Current;
				dictionary6.Add(keyValuePair20.Key, goodsItemData);
			}
			else
			{
				Dictionary<int, GoodsItemData> dictionary7 = this.dicAllGoodsItemData;
				KeyValuePair<int, FuWen> keyValuePair21 = enumerator.Current;
				dictionary7[keyValuePair21.Key] = goodsItemData;
			}
		}
		return this.dicAllGoodsItemData;
	}

	private Dictionary<int, GoodsItemData> GetDicTypeGoodsItemData(string type)
	{
		if (this.dicTypeGoodsItemData != null && this.dicTypeGoodsItemData.Count > 0)
		{
			this.dicTypeGoodsItemData.Clear();
		}
		foreach (KeyValuePair<int, GoodsItemData> keyValuePair in this.GetDicAllGoodsItemData())
		{
			if (type.Equals(keyValuePair.Value.type))
			{
				Dictionary<int, GoodsItemData> dictionary = this.dicTypeGoodsItemData;
				Dictionary<int, GoodsItemData>.Enumerator enumerator;
				KeyValuePair<int, GoodsItemData> keyValuePair2 = enumerator.Current;
				int key = keyValuePair2.Key;
				KeyValuePair<int, GoodsItemData> keyValuePair3 = enumerator.Current;
				dictionary.Add(key, keyValuePair3.Value);
			}
		}
		return this.dicTypeGoodsItemData;
	}

	private Dictionary<int, GoodsItemData> GetDicTypeLevelGoodsItemData(Dictionary<int, GoodsItemData> dics, int lev, bool beContainBigger = false)
	{
		if (this.dicTypeLevelGoodsItemData != null && this.dicTypeLevelGoodsItemData.Count > 0)
		{
			this.dicTypeLevelGoodsItemData.Clear();
		}
		Dictionary<int, GoodsItemData>.Enumerator enumerator = dics.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (beContainBigger)
			{
				KeyValuePair<int, GoodsItemData> keyValuePair = enumerator.Current;
				if (keyValuePair.Value.level >= lev)
				{
					Dictionary<int, GoodsItemData> dictionary = this.dicTypeLevelGoodsItemData;
					KeyValuePair<int, GoodsItemData> keyValuePair2 = enumerator.Current;
					int key = keyValuePair2.Key;
					KeyValuePair<int, GoodsItemData> keyValuePair3 = enumerator.Current;
					dictionary.Add(key, keyValuePair3.Value);
				}
			}
			else
			{
				KeyValuePair<int, GoodsItemData> keyValuePair4 = enumerator.Current;
				if (lev == keyValuePair4.Value.level)
				{
					Dictionary<int, GoodsItemData> dictionary2 = this.dicTypeLevelGoodsItemData;
					KeyValuePair<int, GoodsItemData> keyValuePair5 = enumerator.Current;
					int key2 = keyValuePair5.Key;
					KeyValuePair<int, GoodsItemData> keyValuePair6 = enumerator.Current;
					dictionary2.Add(key2, keyValuePair6.Value);
				}
			}
		}
		return this.dicTypeLevelGoodsItemData;
	}

	private void InitItems(Dictionary<int, GoodsItemData> dicData)
	{
		if (this.SPanel)
		{
			this.SPanel.target = new Vector3(0f, 0f, 0f);
			this.SPanel.enabled = true;
		}
		if (this.mOBC != null)
		{
			this.mOBC.Clear();
		}
		Dictionary<int, GoodsItemData>.Enumerator enumerator = dicData.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem = U3DUtils.NEW<ShenShiPartFuWenZongLanItem>();
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem2 = shenShiPartFuWenZongLanItem;
			KeyValuePair<int, GoodsItemData> keyValuePair = enumerator.Current;
			shenShiPartFuWenZongLanItem2.goodID = keyValuePair.Key;
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem3 = shenShiPartFuWenZongLanItem;
			KeyValuePair<int, GoodsItemData> keyValuePair2 = enumerator.Current;
			shenShiPartFuWenZongLanItem3.isGet = keyValuePair2.Value.isGet;
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem4 = shenShiPartFuWenZongLanItem;
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
			shenShiPartFuWenZongLanItem4.isUsed = isUsed;
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem5 = shenShiPartFuWenZongLanItem;
			KeyValuePair<int, GoodsItemData> keyValuePair5 = enumerator.Current;
			shenShiPartFuWenZongLanItem5.Name = keyValuePair5.Value.Name;
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem6 = shenShiPartFuWenZongLanItem;
			KeyValuePair<int, GoodsItemData> keyValuePair6 = enumerator.Current;
			shenShiPartFuWenZongLanItem6.Count = keyValuePair6.Value.Count;
			ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem7 = shenShiPartFuWenZongLanItem;
			KeyValuePair<int, GoodsItemData> keyValuePair7 = enumerator.Current;
			shenShiPartFuWenZongLanItem7.Attr = keyValuePair7.Value.Attr;
			shenShiPartFuWenZongLanItem.XuanZhongFenJie.gameObject.SetActive(false);
			this.mOBC.AddNoUpdate(shenShiPartFuWenZongLanItem);
		}
	}

	private void OpenShenShiPartFuWenZongLanItemXiangQingWindow(int goodsId, string name, string attr, int count, bool ispeidai)
	{
		if (this.ShenShiPartFuWenZongLanItemXiangQingWindow == null)
		{
			this.ShenShiPartFuWenZongLanItemXiangQingWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenShiPartFuWenZongLanItemXiangQingWindow.IsShowModal = true;
			this.ShenShiPartFuWenZongLanItemXiangQingWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenShiPartFuWenZongLanItemXiangQingWindow, Global.GetLang("详情界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenShiPartFuWenZongLanItemXiangQingWindow);
		}
		if (this.ShenShiPartFuWenZongLanItemXiangQingPart == null)
		{
			this.ShenShiPartFuWenZongLanItemXiangQingPart = U3DUtils.NEW<ShenShiPartFuWenZongLanItemXiangQing>();
			this.ShenShiPartFuWenZongLanItemXiangQingPart.goodsID = goodsId;
			this.ShenShiPartFuWenZongLanItemXiangQingPart.GoodsName = name;
			this.ShenShiPartFuWenZongLanItemXiangQingPart.GoodsAttr = attr;
			this.ShenShiPartFuWenZongLanItemXiangQingPart.Count = count;
			this.ShenShiPartFuWenZongLanItemXiangQingPart.isPeiDai = ispeidai;
			this.ShenShiPartFuWenZongLanItemXiangQingPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShenShiPartFuWenZongLanItemXiangQingWindow();
			};
		}
		this.ShenShiPartFuWenZongLanItemXiangQingWindow.SetContent(this.ShenShiPartFuWenZongLanItemXiangQingWindow.BodyPresenter, this.ShenShiPartFuWenZongLanItemXiangQingPart, 0.0, 0.0, true);
	}

	private void CloseShenShiPartFuWenZongLanItemXiangQingWindow()
	{
		if (null != this.ShenShiPartFuWenZongLanItemXiangQingPart)
		{
			this.ShenShiPartFuWenZongLanItemXiangQingPart.transform.parent = null;
			Object.Destroy(this.ShenShiPartFuWenZongLanItemXiangQingPart.gameObject);
			this.ShenShiPartFuWenZongLanItemXiangQingPart = null;
		}
		if (null != this.ShenShiPartFuWenZongLanItemXiangQingWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenShiPartFuWenZongLanItemXiangQingWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenShiPartFuWenZongLanItemXiangQingWindow, true);
			this.ShenShiPartFuWenZongLanItemXiangQingWindow = null;
		}
	}

	private void OpenShenShiPartFuWenFenJiePartWindow()
	{
		if (this.ShenShiPartFuWenFenJieWindow == null)
		{
			this.ShenShiPartFuWenFenJieWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenShiPartFuWenFenJieWindow.IsShowModal = true;
			this.ShenShiPartFuWenFenJieWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenShiPartFuWenFenJieWindow, Global.GetLang("分解界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenShiPartFuWenFenJieWindow);
		}
		if (this.ShenShiPartFuWenFenJiePart == null)
		{
			this.ShenShiPartFuWenFenJiePart = U3DUtils.NEW<ShenShiPartFuWenFenJie>();
			this.ShenShiPartFuWenFenJiePart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShenShiPartFuWenFenJiePartWindow();
			};
		}
		this.ShenShiPartFuWenFenJieWindow.SetContent(this.ShenShiPartFuWenFenJieWindow.BodyPresenter, this.ShenShiPartFuWenFenJiePart, 0.0, 0.0, true);
	}

	private void CloseShenShiPartFuWenFenJiePartWindow()
	{
		if (null != this.ShenShiPartFuWenFenJiePart)
		{
			this.ShenShiPartFuWenFenJiePart.transform.parent = null;
			Object.Destroy(this.ShenShiPartFuWenFenJiePart.gameObject);
			this.ShenShiPartFuWenFenJiePart = null;
		}
		if (null != this.ShenShiPartFuWenFenJieWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenShiPartFuWenFenJieWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenShiPartFuWenFenJieWindow, true);
			this.ShenShiPartFuWenFenJieWindow = null;
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		ShenShiPartFuWenZongLanItem shenShiPartFuWenZongLanItem = U3DUtils.AS<ShenShiPartFuWenZongLanItem>(this.FuWenItems.SelectedItem);
		if (shenShiPartFuWenZongLanItem)
		{
			this.OpenShenShiPartFuWenZongLanItemXiangQingWindow(shenShiPartFuWenZongLanItem.goodID, shenShiPartFuWenZongLanItem.Name, shenShiPartFuWenZongLanItem.Attr, shenShiPartFuWenZongLanItem.Count, shenShiPartFuWenZongLanItem.isUsed);
		}
	}

	public GButton BtnAll;

	public GButton BtnHunLuan;

	public GButton BtnZhiXu;

	public GButton BtnPingHeng;

	public GButton BtnFenJie;

	public SpringPanel SPanel;

	public GButton[] BtnLevs;

	public ListBox FuWenItems;

	private int showMaxLevel = 7;

	private ObservableCollection mOBC;

	private int serchType;

	private int serchLevel;

	private GButton m_BtnLastSelect;

	private GButton BtnLastSelect;

	private int lastIndex;

	private Dictionary<int, int> dicUsedFuWen = new Dictionary<int, int>();

	private Dictionary<int, int> dicAllFuWen = new Dictionary<int, int>();

	private Dictionary<int, GoodsItemData> dicAllGoodsItemData = new Dictionary<int, GoodsItemData>();

	private Dictionary<int, GoodsItemData> dicTypeGoodsItemData = new Dictionary<int, GoodsItemData>();

	private Dictionary<int, GoodsItemData> dicTypeLevelGoodsItemData = new Dictionary<int, GoodsItemData>();

	protected GChildWindow ShenShiPartFuWenZongLanItemXiangQingWindow;

	public ShenShiPartFuWenZongLanItemXiangQing ShenShiPartFuWenZongLanItemXiangQingPart;

	protected GChildWindow ShenShiPartFuWenFenJieWindow;

	public ShenShiPartFuWenFenJie ShenShiPartFuWenFenJiePart;
}
