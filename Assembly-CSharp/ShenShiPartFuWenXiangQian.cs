using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ShenShiPartFuWenXiangQian : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnChangeName.Label.text = Global.GetLang("改名");
		this.BtnBuy.Label.text = Global.GetLang("购买");
		this.BtnUse.Label.text = Global.GetLang("启用");
		this.BtnFuWenTuiJian.Label.text = Global.GetLang("符文推荐");
		this.BtnBuy.gameObject.SetActive(false);
		this.buyBG.gameObject.SetActive(false);
		this.buyZuanShi.gameObject.SetActive(false);
		this.buyNum.gameObject.SetActive(false);
		this.fuwenAttr.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("符文属性")
		});
		this.xiaohao = ConfigSystemParam.GetSystemParamIntArrayByName("FuWenList", ',');
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BG.URL = "NetImages/GameRes/Images/shenshiTexture/shenshiditu.jpg.qj";
		this.mOBCTabList = this.FuWenTabList.ItemsSource;
		this.FuWenTabList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.FuWenTabListSelectChange);
		this.FuWenItemOnClick();
		this.BtnChangeName.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenShenShiPartChangeNameWindow();
		};
		this.BtnUse.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.GetFuWenTabMod(this.fuwenyeID);
			Super.ShowNetWaiting(null);
		};
		this.BtnBuy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.fuwenyeID + 1 - this.xiaohao[0] > this.xiaohao.Length - 1)
			{
				Super.HintMainText(Global.GetLang("符文页已达到购买上限"), 10, 3);
				return;
			}
			string message = string.Format(Global.GetLang("购买符文页{0}需要{1}钻石"), this.fuwenyeID + 1, this.xiaohao[this.fuwenyeID + 1 - this.xiaohao[0]]);
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object ss, DPSelectedItemEventArgs ee)
			{
				if (ee.ID == 0)
				{
					GameInstance.Game.GetFuWenTabBuy();
				}
			}, new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			});
		};
		this.BtnFuWenTuiJian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenFuWenTuiJian();
		};
		UIEventListener.Get(this.BtnFuWenList).onClick = delegate(GameObject e)
		{
			this.TPAnimation();
		};
	}

	private void TPAnimation()
	{
		TweenPosition component = this.FUWenTabList.GetComponent<TweenPosition>();
		if (!this.fuwenTab)
		{
			if (component)
			{
				component.from = new Vector3(169f, this.fuwenTabHight, -0.5f);
				component.to = new Vector3(169f, 100f, -0.5f);
				component.Play(true);
			}
		}
		else if (component)
		{
			component.from = new Vector3(169f, 100f, -0.5f);
			component.to = new Vector3(169f, this.fuwenTabHight, -0.5f);
			component.Play(true);
		}
		this.fuwenTab = !this.fuwenTab;
	}

	private void SetBtnState(int index)
	{
		if (Global.Data.MyFuWenTabData.Count - 1 != index && index == this.mOBCTabList.Count - 1)
		{
			this.BtnChangeName.gameObject.SetActive(false);
			this.BtnUse.gameObject.SetActive(false);
			this.BtnBuy.gameObject.SetActive(true);
			this.buyBG.gameObject.SetActive(true);
			this.buyZuanShi.gameObject.SetActive(true);
			this.buyNum.gameObject.SetActive(true);
			this.buyNum.text = this.xiaohao[index + 1 - this.xiaohao[0]].ToString();
		}
		else
		{
			this.BtnChangeName.gameObject.SetActive(true);
			if (this.oldTabID == index)
			{
				this.BtnChangeName.gameObject.transform.localPosition = new Vector3(-110f, -212f, -1f);
				this.BtnUse.gameObject.SetActive(false);
			}
			else
			{
				this.BtnChangeName.gameObject.transform.localPosition = new Vector3(-270f, -212f, -1f);
				this.BtnUse.gameObject.SetActive(true);
			}
			this.BtnBuy.gameObject.SetActive(false);
			this.buyBG.gameObject.SetActive(false);
			this.buyZuanShi.gameObject.SetActive(false);
			this.buyNum.gameObject.SetActive(false);
		}
	}

	private void FuWenTabListSelectChange(object sender, MouseEvent e)
	{
		if (this.FuWenTabList.SelectedIndex == -1)
		{
			return;
		}
		this.SetBtnState(this.FuWenTabList.SelectedIndex);
		this.fuwenyeID = this.FuWenTabList.SelectedIndex;
		this.InitFuWen();
		this.InitFuWenTabName();
		this.ComputeFuWenAttr();
		this.ComputeFuWenBaseAttr();
		this.TPAnimation();
	}

	public void Init()
	{
		this.InitFuWen();
		this.InitFuWenTab();
		this.InitFuWenTabName();
		this.ComputeFuWenAttr();
		this.ComputeFuWenBaseAttr();
	}

	public void ResInit()
	{
		this.InitFuWen();
		this.ComputeFuWenAttr();
		this.ComputeFuWenBaseAttr();
	}

	private void InitFuWen()
	{
		int i = 0;
		int num = this.FuWen.Length;
		while (i < num)
		{
			UISprite componentInChildren = this.FuWen[i].GetComponentInChildren<UISprite>();
			string[] array = ShenShiPart.GetDicFuWenHole()[i + 1].OpenLevel.Split(new char[]
			{
				'|'
			});
			if (componentInChildren)
			{
				if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level < int.Parse(array[0]) * 100 + int.Parse(array[1]))
				{
					componentInChildren.enabled = true;
				}
				else
				{
					componentInChildren.enabled = false;
				}
			}
			ShowNetImage componentInChildren2 = this.FuWen[i].GetComponentInChildren<ShowNetImage>();
			if (componentInChildren2)
			{
				componentInChildren2.URL = string.Empty;
			}
			i++;
		}
		if (Global.Data.MyFuWenTabData == null || Global.Data.MyFuWenTabData.Count <= 0)
		{
			return;
		}
		if (this.fuwenyeID < Global.Data.MyFuWenTabData.Count)
		{
			int j = 0;
			int count = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList.Count;
			while (j < count)
			{
				int num2 = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList[j];
				if (num2 != 0)
				{
					UISprite componentInChildren3 = this.FuWen[j].GetComponentInChildren<UISprite>();
					if (componentInChildren3)
					{
						componentInChildren3.enabled = false;
					}
					ShowNetImage componentInChildren4 = this.FuWen[j].GetComponentInChildren<ShowNetImage>();
					if (componentInChildren4)
					{
						componentInChildren4.URL = string.Format("NetImages/GameRes/Images/Goods/{0}.png.qj", num2);
					}
				}
				j++;
			}
		}
	}

	public void InitFuWenTab()
	{
		this.SetBtnState(this.fuwenyeID);
		if (this.mOBCTabList.Count > 0)
		{
			this.mOBCTabList.Clear();
		}
		string text = null;
		int num;
		if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0 && Global.Data.MyFuWenTabData.Count - this.xiaohao[0] < this.xiaohao.Length - 1)
		{
			num = Global.Data.MyFuWenTabData.Count + 1;
			for (int i = 0; i < num; i++)
			{
				LabAttr labAttr = U3DUtils.NEW<LabAttr>();
				if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0 && i != num - 1)
				{
					text = Global.Data.MyFuWenTabData[i].Name;
				}
				else
				{
					text = string.Format(Global.GetLang("符文页{0}"), num);
				}
				labAttr.labText.text = Global.GetColorStringForNGUIText(new object[]
				{
					(this.oldTabID != i) ? "dac7ae" : "fac60d",
					text
				});
				this.mOBCTabList.AddNoUpdate(labAttr);
			}
		}
		else
		{
			num = Global.Data.MyFuWenTabData.Count;
			for (int j = 0; j < num; j++)
			{
				LabAttr labAttr2 = U3DUtils.NEW<LabAttr>();
				if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0)
				{
					text = Global.Data.MyFuWenTabData[j].Name;
				}
				labAttr2.labText.text = Global.GetColorStringForNGUIText(new object[]
				{
					(this.oldTabID != j) ? "dac7ae" : "fac60d",
					text
				});
				this.mOBCTabList.AddNoUpdate(labAttr2);
			}
		}
		SpringPosition component = this.FuWenTabList.GetComponent<SpringPosition>();
		if (component)
		{
			Object.Destroy(component);
		}
		this.fuwenTabHight = (float)(30 * num + 100);
		TweenPosition component2 = this.FUWenTabList.GetComponent<TweenPosition>();
		this.FUWenTabList.transform.localPosition = new Vector3(177f, this.fuwenTabHight, -0.5f);
	}

	private void InitFuWenTabName()
	{
		if (Global.Data.MyFuWenTabData == null || Global.Data.MyFuWenTabData.Count <= 0)
		{
			return;
		}
		if (this.fuwenyeID < Global.Data.MyFuWenTabData.Count)
		{
			this.fuwenyeLab.text = Global.GetLang(Global.Data.MyFuWenTabData[this.fuwenyeID].Name);
		}
		else
		{
			this.fuwenyeLab.text = string.Format(Global.GetLang("符文页{0}"), this.fuwenyeID + 1);
		}
	}

	public void SetFuWenTabName(int tabID, string name)
	{
		LabAttr labAttr = U3DUtils.AS<LabAttr>(this.mOBCTabList[tabID]);
		labAttr.labText.text = Global.GetColorStringForNGUIText(new object[]
		{
			(this.oldTabID != tabID) ? "dac7ae" : "fac60d",
			name
		});
		this.InitFuWenTabName();
	}

	private void ComputeFuWenAttr()
	{
		this.blue = 0;
		this.red = 0;
		this.green = 0;
		if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0 && this.fuwenyeID < Global.Data.MyFuWenTabData.Count)
		{
			int i = 0;
			int count = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList.Count;
			while (i < count)
			{
				int num = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList[i];
				if (ShenShiPart.GetDicFuWen().ContainsKey(num))
				{
					this.blue += ShenShiPart.GetDicFuWen()[num].Blue;
					this.red += ShenShiPart.GetDicFuWen()[num].Red;
					this.green += ShenShiPart.GetDicFuWen()[num].Green;
				}
				i++;
			}
		}
		this.shouxuLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"3681f3",
			string.Format(Global.GetLang("守序：{0}"), this.blue)
		});
		this.hunluanLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"FF0000",
			string.Format(Global.GetLang("混乱：{0}"), this.red)
		});
		this.pinghengLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			string.Format(Global.GetLang("平衡：{0}"), this.green)
		});
	}

	private void ComputeFuWenBaseAttr()
	{
		if (this.SPl)
		{
			this.SPl.target = new Vector3(0f, 0f, 0f);
			this.SPl.enabled = true;
		}
		if (Global.Data.MyFuWenTabData == null || Global.Data.MyFuWenTabData.Count <= 0)
		{
			return;
		}
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
		if (this.fuwenyeID < Global.Data.MyFuWenTabData.Count)
		{
			int i = 0;
			int count = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList.Count;
			while (i < count)
			{
				int num = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList[i];
				if (num > 0)
				{
					dictionary = ConfigGoods.GetDicEquipPropsByGoodsId(num);
					Dictionary<int, double>.Enumerator enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Dictionary<int, double> dictionary3 = dictionary2;
						KeyValuePair<int, double> keyValuePair = enumerator.Current;
						if (dictionary3.ContainsKey(keyValuePair.Key))
						{
							Dictionary<int, double> dictionary5;
							Dictionary<int, double> dictionary4 = dictionary5 = dictionary2;
							KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
							int key;
							int num2 = key = keyValuePair2.Key;
							double num3 = dictionary5[key];
							double num4 = num3;
							KeyValuePair<int, double> keyValuePair3 = enumerator.Current;
							dictionary4[num2] = num4 + keyValuePair3.Value;
						}
						else
						{
							Dictionary<int, double> dictionary6 = dictionary2;
							KeyValuePair<int, double> keyValuePair4 = enumerator.Current;
							int key2 = keyValuePair4.Key;
							KeyValuePair<int, double> keyValuePair5 = enumerator.Current;
							dictionary6.Add(key2, keyValuePair5.Value);
						}
					}
				}
				i++;
			}
		}
		Dictionary<int, double>.Enumerator enumerator2 = dictionary2.GetEnumerator();
		this.fuwenAttrLab.text = null;
		while (enumerator2.MoveNext())
		{
			Dictionary<int, ExtPropIndexess> dictionary7 = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
			KeyValuePair<int, double> keyValuePair6 = enumerator2.Current;
			string text;
			if (dictionary7[keyValuePair6.Key].Percent == 0)
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
			UILabel uilabel = this.fuwenAttrLab;
			string text3 = uilabel.text;
			object[] array = new object[2];
			array[0] = "e3b36c";
			int num5 = 1;
			string text4 = "{0}:";
			Dictionary<int, ExtPropIndexess> dictionary8 = ShenShiPartFuWenXiangQian.GetDicExtPropIndexes();
			KeyValuePair<int, double> keyValuePair9 = enumerator2.Current;
			array[num5] = string.Format(text4, dictionary8[keyValuePair9.Key].Description) + Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				text
			});
			uilabel.text = text3 + Global.GetColorStringForNGUIText(array) + "\r\n";
		}
	}

	public static void ClearXMLData()
	{
		if (0 < ShenShiPartFuWenXiangQian.dicExtPropIndexes.Count)
		{
			ShenShiPartFuWenXiangQian.dicExtPropIndexes.Clear();
		}
	}

	public static Dictionary<int, ExtPropIndexess> GetDicExtPropIndexes()
	{
		if (ShenShiPartFuWenXiangQian.dicExtPropIndexes.Count > 0)
		{
			return ShenShiPartFuWenXiangQian.dicExtPropIndexes;
		}
		XElement gameResXml = Global.GetGameResXml("Config/ExtPropIndexes.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ExtPropIndexes");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			ExtPropIndexess extPropIndexess = new ExtPropIndexess();
			extPropIndexess.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			extPropIndexess.World = Global.GetXElementAttributeStr(xelementList[i], "World");
			extPropIndexess.Description = Global.GetXElementAttributeStr(xelementList[i], "Description");
			extPropIndexess.ShowList = Global.GetXElementAttributeInt(xelementList[i], "ShowList");
			extPropIndexess.Percent = Global.GetXElementAttributeInt(xelementList[i], "Percent");
			if (!ShenShiPartFuWenXiangQian.dicExtPropIndexes.ContainsKey(extPropIndexess.ID))
			{
				ShenShiPartFuWenXiangQian.dicExtPropIndexes.Add(extPropIndexess.ID, extPropIndexess);
			}
			i++;
		}
		return ShenShiPartFuWenXiangQian.dicExtPropIndexes;
	}

	private void OpenShenShiPartChangeNameWindow()
	{
		if (this.ShenShiPartChangeNameWindow == null)
		{
			this.ShenShiPartChangeNameWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenShiPartChangeNameWindow.IsShowModal = true;
			this.ShenShiPartChangeNameWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenShiPartChangeNameWindow, Global.GetLang("改名界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenShiPartChangeNameWindow);
		}
		if (this.ShenShiPartChangeNamePart == null)
		{
			this.ShenShiPartChangeNamePart = U3DUtils.NEW<ShenShiPartChangeName>();
			this.ShenShiPartChangeNamePart.Name = this.fuwenyeLab.text;
			this.ShenShiPartChangeNamePart.TabID = this.fuwenyeID;
			this.ShenShiPartChangeNamePart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShenShiPartChangeNameWindow();
			};
		}
		this.ShenShiPartChangeNameWindow.SetContent(this.ShenShiPartChangeNameWindow.BodyPresenter, this.ShenShiPartChangeNamePart, 0.0, 0.0, true);
	}

	private void CloseShenShiPartChangeNameWindow()
	{
		if (null != this.ShenShiPartChangeNamePart)
		{
			this.ShenShiPartChangeNamePart.transform.parent = null;
			Object.Destroy(this.ShenShiPartChangeNamePart.gameObject);
			this.ShenShiPartChangeNamePart = null;
		}
		if (null != this.ShenShiPartChangeNameWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenShiPartChangeNameWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenShiPartChangeNameWindow, true);
			this.ShenShiPartChangeNameWindow = null;
		}
	}

	private void OpenShenShiPartFuWenpeidaiWindow(int fuwenTabId, int goodsId, int index)
	{
		if (this.ShenShiPartFuWenpeidaiWindow == null)
		{
			this.ShenShiPartFuWenpeidaiWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenShiPartFuWenpeidaiWindow.IsShowModal = true;
			this.ShenShiPartFuWenpeidaiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenShiPartFuWenpeidaiWindow, Global.GetLang("镶嵌界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenShiPartFuWenpeidaiWindow);
		}
		if (this.ShenShiPartFuWenpeidaiPart == null)
		{
			this.ShenShiPartFuWenpeidaiPart = U3DUtils.NEW<ShenShiPartFuWenpeidai>();
			if (ShenShiPart.GetDicFuWen().ContainsKey(goodsId))
			{
				string type = ShenShiPart.GetDicFuWen()[goodsId].Type;
			}
			this.ShenShiPartFuWenpeidaiPart.FenWenTabId = fuwenTabId;
			this.ShenShiPartFuWenpeidaiPart.FuWenGoodsID = goodsId;
			this.ShenShiPartFuWenpeidaiPart.Index = index;
			this.ShenShiPartFuWenpeidaiPart.Mblue = this.blue;
			this.ShenShiPartFuWenpeidaiPart.Mred = this.red;
			this.ShenShiPartFuWenpeidaiPart.Mgreen = this.green;
			this.ShenShiPartFuWenpeidaiPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShenShiPartFuWenpeidaiWindow();
			};
		}
		this.ShenShiPartFuWenpeidaiWindow.SetContent(this.ShenShiPartFuWenpeidaiWindow.BodyPresenter, this.ShenShiPartFuWenpeidaiPart, 0.0, 0.0, true);
		this.ShenShiPartFuWenpeidaiPart.InitFuWenItems();
	}

	private void CloseShenShiPartFuWenpeidaiWindow()
	{
		if (null != this.ShenShiPartFuWenpeidaiPart)
		{
			this.ShenShiPartFuWenpeidaiPart.transform.parent = null;
			Object.Destroy(this.ShenShiPartFuWenpeidaiPart.gameObject);
			this.ShenShiPartFuWenpeidaiPart = null;
		}
		if (null != this.ShenShiPartFuWenpeidaiWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenShiPartFuWenpeidaiWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenShiPartFuWenpeidaiWindow, true);
			this.ShenShiPartFuWenpeidaiWindow = null;
		}
	}

	private void FuWenItemOnClick()
	{
		int num = this.FuWen.Length;
		for (int i = 0; i < num; i++)
		{
			this.FuWen[i].name = i.ToString();
			UIEventListener.Get(this.FuWen[i]).onClick = delegate(GameObject e)
			{
				this.ItemOnClick(e);
			};
		}
	}

	private void ItemOnClick(GameObject e)
	{
		int num = int.Parse(e.name);
		UISprite componentInChildren = this.FuWen[num].GetComponentInChildren<UISprite>();
		if (componentInChildren != null && componentInChildren.enabled)
		{
			string[] array = ShenShiPart.GetDicFuWenHole()[num + 1].OpenLevel.Split(new char[]
			{
				'|'
			});
			Super.HintMainText(string.Format(Global.GetLang("符文槽未开启,需要{0}转{1}级"), array[0], array[1]), 10, 3);
			return;
		}
		if (this.fuwenyeID < Global.Data.MyFuWenTabData.Count)
		{
			string type;
			if (num < 8)
			{
				type = "Blue";
			}
			else if (num > 15)
			{
				type = "Red";
			}
			else
			{
				type = "Green";
			}
			int goodsId = 0;
			ShowNetImage componentInChildren2 = this.FuWen[num].GetComponentInChildren<ShowNetImage>();
			if (string.IsNullOrEmpty(componentInChildren2.ImageURL))
			{
				if (Global.Data.MyFuWenData != null && this.HadTheTypeFuWen(type))
				{
					this.OpenShenShiPartFuWenpeidaiWindow(this.fuwenyeID, goodsId, num);
				}
				else
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedFuWen, null, string.Empty, string.Empty);
				}
			}
			else
			{
				if (Global.Data.MyFuWenTabData != null && Global.Data.MyFuWenTabData.Count > 0 && this.fuwenyeID < Global.Data.MyFuWenTabData.Count)
				{
					goodsId = Global.Data.MyFuWenTabData[this.fuwenyeID].FuWenEquipList[num];
				}
				this.OpenShenShiPartFuWenpeidaiWindow(this.fuwenyeID, goodsId, num);
			}
			return;
		}
		if (this.fuwenyeID + 1 - this.xiaohao[0] > this.xiaohao.Length - 1)
		{
			Super.HintMainText(Global.GetLang("符文页已达到购买上限"), 10, 3);
			return;
		}
		string message = string.Format(Global.GetLang("购买符文页{0}需要{1}钻石"), this.fuwenyeID + 1, this.xiaohao[this.fuwenyeID + 1 - this.xiaohao[0]]);
		Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object ss, DPSelectedItemEventArgs ee)
		{
			if (ee.ID == 0)
			{
				GameInstance.Game.GetFuWenTabBuy();
			}
		}, new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		});
	}

	private bool HadTheTypeFuWen(string type)
	{
		if (Global.Data.MyFuWenData != null)
		{
			int i = 0;
			int count = Global.Data.MyFuWenData.Count;
			while (i < count)
			{
				if (ShenShiPart.GetDicFuWen().ContainsKey(Global.Data.MyFuWenData[i].GoodsID) && type == ShenShiPart.GetDicFuWen()[Global.Data.MyFuWenData[i].GoodsID].Type)
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	private void OpenFuWenTuiJian()
	{
		if (this.m_tuiJianWindow == null)
		{
			this.m_tuiJianWindow = U3DUtils.NEW<GChildWindow>();
			this.m_tuiJianWindow.IsShowModal = true;
			this.m_tuiJianWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_tuiJianWindow, Global.GetLang("推荐"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_tuiJianWindow);
		}
		if (this.m_tuiJianPart == null)
		{
			this.m_tuiJianPart = U3DUtils.NEW<ShenShiPartTuiJian>();
			this.m_tuiJianPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHuiShouWindow();
			};
		}
		this.m_tuiJianWindow.SetContent(this.m_tuiJianWindow.BodyPresenter, this.m_tuiJianPart, 0.0, 0.0, true);
	}

	private void CloseHuiShouWindow()
	{
		if (null != this.m_tuiJianPart)
		{
			this.m_tuiJianPart.transform.parent = null;
			Object.Destroy(this.m_tuiJianPart.gameObject);
			this.m_tuiJianPart = null;
		}
		if (null != this.m_tuiJianWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_tuiJianWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_tuiJianWindow, true);
			this.m_tuiJianWindow = null;
		}
	}

	public UILabel shouxuLab;

	public UILabel hunluanLab;

	public UILabel pinghengLab;

	public UILabel fuwenyeLab;

	public UILabel fuwenAttr;

	public UILabel fuwenAttrLab;

	public GButton BtnChangeName;

	public GButton BtnUse;

	public GButton BtnFuWenTuiJian;

	public GButton BtnBuy;

	public UISprite buyBG;

	public UISprite buyZuanShi;

	public UILabel buyNum;

	public GameObject BtnFuWenList;

	public GameObject FUWenTabList;

	public ListBox FuWenTabList;

	public GameObject[] FuWen;

	public ShowNetImage BG;

	public SpringPanel SPl;

	private ObservableCollection mOBCTabList;

	public int fuwenyeID;

	public int oldTabID;

	private bool fuwenTab;

	private float fuwenTabHight;

	private int blue;

	private int red;

	private int green;

	private int[] xiaohao;

	private static Dictionary<int, ExtPropIndexess> dicExtPropIndexes = new Dictionary<int, ExtPropIndexess>();

	protected GChildWindow ShenShiPartChangeNameWindow;

	protected ShenShiPartChangeName ShenShiPartChangeNamePart;

	protected GChildWindow ShenShiPartFuWenpeidaiWindow;

	protected ShenShiPartFuWenpeidai ShenShiPartFuWenpeidaiPart;

	protected GChildWindow m_tuiJianWindow;

	protected ShenShiPartTuiJian m_tuiJianPart;
}
