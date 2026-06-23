using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class TaLuoPaiPart : UserControl
{
	protected override void OnDestroy()
	{
		this.StopTimer();
		base.OnDestroy();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitXml();
		this.init();
		this.KingInit();
		this.GetData();
		this.m_BtnTeQuan.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.TeQuanHander);
		this.m_BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			PlayZone.GlobalPlayZone.CloseTaLuoPaiWindow();
		};
		this.m_Tab.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
		{
			List<GButton> tabBtns = this.m_Tab.TabBtns;
			for (int i = 0; i < tabBtns.Count; i++)
			{
				UISprite componentInChildren = tabBtns[i].transform.GetComponentInChildren<UISprite>();
				if (null != componentInChildren)
				{
					Vector3 localScale = componentInChildren.transform.localScale;
					localScale.y = (float)((i != s.ID) ? 50 : 52);
					componentInChildren.transform.localScale = localScale;
				}
				UILabel componentInChildren2 = tabBtns[0].transform.GetComponentInChildren<UILabel>();
				UILabel componentInChildren3 = tabBtns[1].transform.GetComponentInChildren<UILabel>();
				if (null != componentInChildren)
				{
					if (s.ID == 0)
					{
						componentInChildren2.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							string.Format("{0}", Global.GetLang("塔罗佩戴"))
						});
						componentInChildren3.text = Global.GetColorStringForNGUIText(new object[]
						{
							"9d8667",
							string.Format("{0}", Global.GetLang("塔罗图鉴"))
						});
					}
					else
					{
						componentInChildren2.text = Global.GetColorStringForNGUIText(new object[]
						{
							"9d8667",
							string.Format("{0}", Global.GetLang("塔罗佩戴"))
						});
						componentInChildren3.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							string.Format("{0}", Global.GetLang("塔罗图鉴"))
						});
					}
				}
			}
			return true;
		};
		this.m_ShuXingTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("属性总预览")
		});
		this.m_Shuxing.Text = string.Empty;
		this.SetFlag(true);
	}

	private void TeQuanHander(object sender, MouseEvent e)
	{
		this.SetFlag(false);
		if (this.m_KingFlag)
		{
			List<TaLuoPaiItem> list = new List<TaLuoPaiItem>();
			if (this.m_ListShengJi != null)
			{
				if (0 < this.m_ListShengJi.Count && this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null)
				{
					for (int i = 0; i < this.m_ListShengJi.Count; i++)
					{
						foreach (KeyValuePair<int, int> keyValuePair in this.m_TarotSystemData.KingData.AddtionDict)
						{
							int key = keyValuePair.Key;
							if (key >= 0)
							{
								if (this.m_ListShengJi[i].ItemGoodsId == key)
								{
									TaLuoPaiItem taLuoPaiItem = this.m_ListShengJi[i];
									Dictionary<int, int>.Enumerator enumerator;
									KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
									taLuoPaiItem.ExtraLevel = keyValuePair2.Value;
									list.Add(this.m_ListShengJi[i]);
									break;
								}
								this.m_ListShengJi[i].ExtraLevel = 0;
							}
						}
					}
					this.ShowTaLuoPaiKingPart(list);
				}
				else
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.SendUsingKingTeQuan();
					this.m_KingFlag = false;
				}
			}
		}
	}

	private void init()
	{
		this.m_BtnTeQuan.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			string.Format("{0}", Global.GetLang("国王特权"))
		});
		List<GButton> tabBtns = this.m_Tab.TabBtns;
		UILabel componentInChildren = tabBtns[0].transform.GetComponentInChildren<UILabel>();
		if (null != componentInChildren)
		{
			componentInChildren.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{0}", Global.GetLang("塔罗佩戴"))
			});
		}
		UILabel componentInChildren2 = tabBtns[1].transform.GetComponentInChildren<UILabel>();
		if (null != componentInChildren2)
		{
			componentInChildren2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{0}", Global.GetLang("塔罗图鉴"))
			});
		}
		this.m_TitleShuxing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("{0}", Global.GetLang("属性总预览"))
		});
		this.m_ObservableCollection_ListBox = this.m_ListBox.ItemsSource;
		this.m_ListGuanLi = new List<TaLuoPaiItem>();
		this.m_ListShengJi = new List<TaLuoPaiItem>();
	}

	private void GetData()
	{
		GameInstance.Game.GetGetTaLuopaiData();
		Super.ShowNetWaiting(null);
	}

	private void InitXml()
	{
		List<int> list = new List<int>();
		XElement gameResXml = Global.GetGameResXml("Config/Tarot.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Tarot");
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList[i], "Level");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[i], "Name");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
				TarotXmlData tarotXmlData = new TarotXmlData();
				tarotXmlData.ID = xelementAttributeInt;
				tarotXmlData.GoodsID = xelementAttributeInt2;
				tarotXmlData.Level = xelementAttributeInt3;
				tarotXmlData.Name = xelementAttributeStr;
				tarotXmlData.NeedGoods = xelementAttributeStr2;
				if (!list.Contains(tarotXmlData.GoodsID))
				{
					list.Add(tarotXmlData.GoodsID);
				}
				if (this.Dic_TarotData.ContainsKey(xelementAttributeInt2))
				{
					this.Dic_TarotData[xelementAttributeInt2].Add(xelementAttributeInt3, tarotXmlData);
				}
				else
				{
					TarotData tarotData = new TarotData();
					this.Dic_TarotData.Add(tarotXmlData.GoodsID, tarotData);
					this.Dic_TarotData[tarotXmlData.GoodsID].Add(xelementAttributeInt3, tarotXmlData);
				}
			}
		}
		if (this.Dic_TarotData.Count == 0 || this.m_DicKaPaiGuanLi.Count == 0)
		{
		}
		for (int j = 0; j < list.Count; j++)
		{
			this.m_TaLuoPailst.Add(ConfigGoods.GetGoodsXmlNodeByID(list[j]));
			if (this.Dic_TarotData.ContainsKey(list[j]))
			{
				TarotDataAndXmlData tarotDataAndXmlData = new TarotDataAndXmlData();
				tarotDataAndXmlData.xmlData = this.Dic_TarotData[list[j]].Get(0);
				this.m_DicKaPaiGuanLi.Add(tarotDataAndXmlData.xmlData.GoodsID, tarotDataAndXmlData);
			}
		}
	}

	private int PaiXu(TarotDataAndXmlData a, TarotDataAndXmlData b)
	{
		if (a.data == null || b.data == null)
		{
			return a.xmlData.ID - b.xmlData.ID;
		}
		if (a.data.Level == b.data.Level)
		{
			return a.xmlData.ID - b.xmlData.ID;
		}
		return b.data.Level - a.data.Level;
	}

	private IEnumerator AddTarotXmlData(List<TarotDataAndXmlData> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (i % 8 == 0 && i != 0)
			{
				yield return null;
			}
			TaLuoPaiItem m_Item = U3DUtils.NEW<TaLuoPaiItem>();
			this.m_ObservableCollection_ListBox.AddNoUpdate(m_Item);
			if (null != m_Item.gameObject.GetComponent<BoxCollider>())
			{
				m_Item.gameObject.GetComponent<BoxCollider>().size = new Vector3(320f, 325f, 1f);
			}
			m_Item.ItemId = list[i].xmlData.ID;
			if (list[i].data != null)
			{
				m_Item.IsActivate = true;
				m_Item.ItemGoodsId = list[i].data.GoodId;
				m_Item.Level = list[i].data.Level;
				m_Item.Name = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format("{0}", Global.GetLang(list[i].xmlData.Name))
				});
			}
			else
			{
				m_Item.IsActivate = false;
				m_Item.ItemGoodsId = list[i].xmlData.GoodsID;
				m_Item.Level = list[i].xmlData.Level;
				m_Item.Name = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format("{0}", Global.GetLang(list[i].xmlData.Name))
				});
			}
			int count = 0;
			if (this.Dic_TarotData[list[i].xmlData.GoodsID].Get(list[i].xmlData.Level + 1) != null)
			{
				int MaxLevel = 100;
				MaxLevel = this.Dic_TarotData[m_Item.ItemGoodsId].Maxlevel;
				m_Item.MaxLevel = MaxLevel;
				string[] NeedGoods = this.Dic_TarotData[list[i].xmlData.GoodsID].Get(list[i].xmlData.Level + 1).NeedGoods.Split(new char[]
				{
					','
				});
				if (NeedGoods.Length == 2)
				{
					int SuiPianGoodsId = int.Parse(NeedGoods[0]);
					count = int.Parse(NeedGoods[1]);
				}
				m_Item.UPEXP = count;
				if (list[i].data != null)
				{
					m_Item.NowEXP = list[i].data.TarotMoney;
				}
				else
				{
					TarotCardData tempData = null;
					if (this.m_TarotSystemData.TarotCardDatas != null)
					{
						tempData = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == m_Item.ItemGoodsId);
					}
					if (tempData != null)
					{
						m_Item.NowEXP = ((0 < tempData.TarotMoney) ? tempData.TarotMoney : 0);
					}
					else
					{
						m_Item.NowEXP = 0;
					}
				}
			}
			else
			{
				int MaxLevel2 = 100;
				MaxLevel2 = this.Dic_TarotData[m_Item.ItemGoodsId].Maxlevel;
				m_Item.MaxLevel = MaxLevel2;
				string[] NeedGoods2 = this.Dic_TarotData[list[i].xmlData.GoodsID].Get(list[i].xmlData.Level).NeedGoods.Split(new char[]
				{
					','
				});
				if (NeedGoods2.Length == 2)
				{
					int SuiPianGoodsId = int.Parse(NeedGoods2[0]);
					count = int.Parse(NeedGoods2[1]);
				}
				m_Item.UPEXP = count;
				if (list[i].data != null)
				{
					m_Item.NowEXP = list[i].data.TarotMoney;
				}
				else
				{
					TarotCardData tempData2 = null;
					if (this.m_TarotSystemData.TarotCardDatas != null)
					{
						tempData2 = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == m_Item.ItemGoodsId);
					}
					if (tempData2 != null)
					{
						m_Item.NowEXP = ((0 < tempData2.TarotMoney) ? tempData2.TarotMoney : 0);
					}
					else
					{
						m_Item.NowEXP = 0;
					}
				}
			}
			UIPanel p = m_Item.GetComponent<UIPanel>();
			m_Item.TarotDataAndXml = list[i];
			if (null != p)
			{
				Object.Destroy(p);
			}
			if (this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null)
			{
				if (this.m_TarotSystemData.KingData.AddtionDict.ContainsKey(m_Item.ItemGoodsId))
				{
					m_Item.ExtraLevel = this.m_TarotSystemData.KingData.AddtionDict[m_Item.ItemGoodsId];
				}
			}
			else
			{
				m_Item.ExtraLevel = 0;
			}
			m_Item.BScale = true;
			m_Item.gameObject.AddComponent<UIDragPanelContents>();
			m_Item.GetComponent<UIDragPanelContents>().draggablePanel = this.m_PanKaPaiShengJi.GetComponent<UIDraggablePanel>();
			UIEventListener.Get(m_Item.gameObject).onClick = new UIEventListener.VoidDelegate(this.TaLuoPaiItemClick);
			this.m_ListShengJi.Add(m_Item);
			if (i >= 14)
			{
				this.SetFlag(true);
			}
		}
		yield break;
	}

	private void SetFlag(bool flag)
	{
		if (this.m_ListGuanLi != null)
		{
			for (int i = 0; i < this.m_ListGuanLi.Count; i++)
			{
				this.m_ListGuanLi[i].SetFlag = flag;
			}
		}
		if (this.m_ListShengJi != null)
		{
			for (int j = 0; j < this.m_ListShengJi.Count; j++)
			{
				this.m_ListShengJi[j].SetFlag = flag;
			}
		}
	}

	private void TaLuoPaiItemClick(GameObject sender)
	{
		if (null != sender)
		{
			this.SetFlag(false);
			TaLuoPaiItem component = sender.GetComponent<TaLuoPaiItem>();
			if (component.Type == 0)
			{
				this.ShowTaLuoPaiJiHuoPart(component);
			}
			else if (component.Type == 1)
			{
				this.ShowQieHuanPart(component.ItemGoodsId, component.Pos);
			}
			else if (component.Type == 2)
			{
				this.ShowQieHuanPart(component.ItemGoodsId, component.Pos);
			}
		}
	}

	private void ShowQieHuanPart(int GoodsId, int index)
	{
		this.SetFlag(false);
		this.CheckIndex = index;
		this.m_PeiDaiGoodId = this.m_ListGuanLi[this.CheckIndex].ItemGoodsId;
		List<TarotDataAndXmlData> list = new List<TarotDataAndXmlData>();
		List<TarotDataAndXmlData> list2 = new List<TarotDataAndXmlData>();
		List<TarotDataAndXmlData> list3 = new List<TarotDataAndXmlData>();
		foreach (KeyValuePair<int, TarotDataAndXmlData> keyValuePair in this.m_DicKaPaiGuanLi)
		{
			TarotCardData data = keyValuePair.Value.data;
			if (data != null)
			{
				if (data.Postion != 0)
				{
					if (index + 1 == (int)data.Postion)
					{
						List<TarotDataAndXmlData> list4 = list;
						Dictionary<int, TarotDataAndXmlData>.Enumerator enumerator;
						KeyValuePair<int, TarotDataAndXmlData> keyValuePair2 = enumerator.Current;
						list4.Add(keyValuePair2.Value);
					}
				}
				else
				{
					List<TarotDataAndXmlData> list5 = list2;
					Dictionary<int, TarotDataAndXmlData>.Enumerator enumerator;
					KeyValuePair<int, TarotDataAndXmlData> keyValuePair3 = enumerator.Current;
					list5.Add(keyValuePair3.Value);
				}
			}
		}
		if (0 < list.Count)
		{
			list.Sort(new Comparison<TarotDataAndXmlData>(this.PaiXu));
		}
		if (0 < list2.Count)
		{
			list2.Sort(new Comparison<TarotDataAndXmlData>(this.PaiXu));
		}
		list3.AddRange(list);
		list3.AddRange(list2);
		if (0 < list3.Count)
		{
			if (null == this.m_TaLuoPaiQieHuanWind)
			{
				this.m_TaLuoPaiQieHuanWind = U3DUtils.NEW<GChildWindow>();
				this.m_TaLuoPaiQieHuanWind.ModalType = ChildWindowModalType.Translucent;
				Super.InitChildWindow(this.m_TaLuoPaiQieHuanWind, "TaLuoPaiQieHuanWindow");
				Super.GData.GlobalPlayZone.Children.Add(this.m_TaLuoPaiQieHuanWind);
			}
			this.m_TaLuoPaiQieHuanPart = U3DUtils.NEW<TaLuoPaiQieHuan>();
			if (this.m_TarotSystemData.KingData != null)
			{
				this.m_TaLuoPaiQieHuanPart.KingData = this.m_TarotSystemData.KingData.AddtionDict;
			}
			this.m_TaLuoPaiQieHuanPart.RefreshContent(list3);
			this.m_TaLuoPaiQieHuanPart.GoodsID = GoodsId;
			this.m_TaLuoPaiQieHuanWind.Body.Add(this.m_TaLuoPaiQieHuanPart);
			this.m_TaLuoPaiQieHuanPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (e == null)
				{
					if (s.ID == 0)
					{
						Object.Destroy(this.m_TaLuoPaiQieHuanPart.gameObject);
						this.m_TaLuoPaiQieHuanPart = null;
						Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.m_TaLuoPaiQieHuanWind);
						if (this.RefreshItemAndData() != null)
						{
							this.StartCoroutine<bool>(this.RefreshUI(this.RefreshItemAndData()));
						}
					}
					else if (s.ID == 11 || s.ID == 10)
					{
						if (this.m_QieHuanTime == 0L)
						{
							this.m_QieHuanTime = Global.GetCorrectLocalTime();
							Super.ShowNetWaiting(null);
							GameInstance.Game.SendTaLuopaiChangePOS(s.MyID, (s.ID != 10) ? 0 : (index + 1));
						}
						else if (Global.GetCorrectLocalTime() - this.m_QieHuanTime > this.m_QieHuankey)
						{
							Super.ShowNetWaiting(null);
							GameInstance.Game.SendTaLuopaiChangePOS(s.MyID, (s.ID != 10) ? 0 : (index + 1));
							this.m_QieHuanCount = 0;
							this.m_QieHuanTime = Global.GetCorrectLocalTime();
						}
						else
						{
							this.m_QieHuanCount++;
						}
						if (this.m_QieHuanCount > 3)
						{
							Super.HintMainText(Global.GetLang("请勿频繁切换卡牌"), 10, 3);
						}
					}
				}
			};
			UIEventListener.Get(this.m_TaLuoPaiQieHuanWind.ModalBak).onClick = delegate(GameObject e)
			{
				Object.Destroy(this.m_TaLuoPaiQieHuanPart.gameObject);
				this.m_TaLuoPaiQieHuanPart = null;
				Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.m_TaLuoPaiQieHuanWind);
				if (this.RefreshItemAndData() != null)
				{
					this.StartCoroutine<bool>(this.RefreshUI(this.RefreshItemAndData()));
				}
			};
		}
		else
		{
			this.SetFlag(true);
			Super.HintMainText(Global.GetLang("没有可用的卡片"), 10, 3);
		}
	}

	private void CloseQieHuanPart()
	{
	}

	private void ShowTaLuoPaiJiHuoPart(TaLuoPaiItem m_Item)
	{
		if (null == this.m_TalLuoPaiJiHuoWind && m_Item != null)
		{
			TarotCardData tarotCardData = null;
			int GoodsID = m_Item.ItemGoodsId;
			if (this.m_TarotSystemData != null && this.m_TarotSystemData.TarotCardDatas != null)
			{
				tarotCardData = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData s) => s.GoodId == GoodsID);
			}
			this.m_TalLuoPaiJiHuoWind = U3DUtils.NEW<GChildWindow>();
			this.m_TalLuoPaiJiHuoWind.ModalType = ChildWindowModalType.Translucent;
			this.m_TalLuoPaiJiHuoWind.transform.SetParent(Super.GData.PlayZoneRoot.Children.transform, false);
			this.m_TalLuoPaiJiHuoWind.transform.localPosition = new Vector3(0f, 0f, WindowManage.GetZ(base.transform.parent.parent) - 10f);
			UIEventListener.Get(this.m_TalLuoPaiJiHuoWind.ModalBak).onClick = delegate(GameObject e)
			{
				this.CloseTaLuoPaiJiHuoPart();
			};
			this.m_TalLuoPaiJiHuoWind.ChildWindowClose = delegate(object e, EventArgs s)
			{
				this.CloseTaLuoPaiJiHuoPart();
				return true;
			};
			this.m_TaLuoPaiJiHuoPart = U3DUtils.NEW<TaLuoPaiJiHuoPart>();
			this.m_TaLuoPaiJiHuoPart.KaPaiTaLuoPaiItem = m_Item;
			this.m_TaLuoPaiJiHuoPart.TarotCardData = tarotCardData;
			this.m_TalLuoPaiJiHuoWind.Body.Add(this.m_TaLuoPaiJiHuoPart);
			this.m_TaLuoPaiJiHuoPart.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.CloseTaLuoPaiJiHuoPart();
			};
		}
	}

	private void CloseTaLuoPaiJiHuoPart()
	{
		this.SetFlag(true);
		if (null != this.m_TalLuoPaiJiHuoWind)
		{
			Object.Destroy(this.m_TalLuoPaiJiHuoWind.gameObject);
		}
		if (null != this.m_TaLuoPaiJiHuoPart)
		{
			Object.Destroy(this.m_TaLuoPaiJiHuoPart.gameObject);
		}
		this.m_TalLuoPaiJiHuoWind = null;
		this.m_TaLuoPaiJiHuoPart = null;
	}

	private void RefreshUIPeiDai()
	{
		int i;
		for (i = 0; i < this.m_ListGuanLi.Count; i++)
		{
			TaLuoPaiItem item = this.m_ListGuanLi[i];
			if (this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null)
			{
				if (this.m_TarotSystemData.KingData.AddtionDict.ContainsKey(item.ItemGoodsId))
				{
					item.ExtraLevel = this.m_TarotSystemData.KingData.AddtionDict[item.ItemGoodsId];
				}
				else
				{
					item.ExtraLevel = 0;
				}
			}
			else
			{
				item.ExtraLevel = 0;
			}
			for (int j = 0; j < this.m_ListShengJi.Count; j++)
			{
				if (this.m_ListShengJi[j].ItemGoodsId == this.m_ListGuanLi[i].ItemGoodsId)
				{
					this.m_ListGuanLi[i].Level = this.m_ListShengJi[j].Level;
					this.m_ListGuanLi[i].ExtraLevel = this.m_ListShengJi[j].ExtraLevel;
					this.m_ListGuanLi[i].ItemGoodsId = this.m_ListShengJi[j].ItemGoodsId;
					TarotCardData tarotCardData = null;
					if (this.m_TarotSystemData.TarotCardDatas != null)
					{
						tarotCardData = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == this.m_ListGuanLi[i].ItemGoodsId);
					}
					if (tarotCardData != null)
					{
						item.NowEXP = ((0 < tarotCardData.TarotMoney) ? tarotCardData.TarotMoney : 0);
					}
					else
					{
						item.NowEXP = 0;
					}
				}
			}
			if (this.Dic_TarotData.ContainsKey(this.m_ListGuanLi[i].ItemGoodsId) && null != item)
			{
				TarotData tarotData = this.Dic_TarotData[this.m_ListGuanLi[i].ItemGoodsId];
				TarotXmlData tarotXmlData = tarotData.Get(this.m_ListGuanLi[i].Level);
				int needsGoodsNum;
				if (item.Level < item.MaxLevel)
				{
					int needsGoodsId = tarotData.GetNeedsGoodsId(this.m_ListGuanLi[i].Level + 1);
					needsGoodsNum = tarotData.GetNeedsGoodsNum(this.m_ListGuanLi[i].Level + 1);
				}
				else
				{
					int needsGoodsId = tarotData.GetNeedsGoodsId(this.m_ListGuanLi[i].Level);
					needsGoodsNum = tarotData.GetNeedsGoodsNum(this.m_ListGuanLi[i].Level);
				}
				item.UPEXP = needsGoodsNum;
				TarotCardData tarotCardData2 = null;
				if (this.m_TarotSystemData.TarotCardDatas != null)
				{
					tarotCardData2 = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == item.ItemGoodsId);
				}
				if (tarotCardData2 != null)
				{
					item.NowEXP = ((0 < tarotCardData2.TarotMoney) ? tarotCardData2.TarotMoney : 0);
				}
				else
				{
					item.NowEXP = 0;
				}
			}
		}
	}

	private IEnumerator RefreshUI(List<TarotDataAndXmlData> lst)
	{
		this.UpDateProoerty();
		this.KingInit();
		for (int i = 0; i < this.m_ListShengJi.Count; i++)
		{
			if (i % 8 == 0)
			{
				yield return null;
			}
			if (i < lst.Count)
			{
				TaLuoPaiItem item = this.m_ListShengJi[i];
				TarotDataAndXmlData data = lst[i];
				item.TarotDataAndXml = data;
				bool bActivate = false;
				if (data.data != null && data.data.Level != 0)
				{
					bActivate = true;
				}
				item.IsActivate = bActivate;
				item.ItemId = (bActivate ? data.data.GoodId : data.xmlData.ID);
				item.Level = (bActivate ? data.data.Level : 0);
				if (item.Level > 0)
				{
					item.IsActivate = true;
				}
				item.ItemGoodsId = data.xmlData.GoodsID;
				if (this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null)
				{
					if (this.m_TarotSystemData.KingData.AddtionDict.ContainsKey(item.ItemGoodsId))
					{
						item.ExtraLevel = this.m_TarotSystemData.KingData.AddtionDict[item.ItemGoodsId];
					}
					else
					{
						item.ExtraLevel = 0;
					}
				}
				else
				{
					item.ExtraLevel = 0;
				}
				item.Name = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format("{0}", Global.GetLang(data.xmlData.Name))
				});
				int count = 0;
				int MaxLevel = 100;
				MaxLevel = this.Dic_TarotData[data.xmlData.GoodsID].Maxlevel;
				if (this.Dic_TarotData.ContainsKey(data.xmlData.GoodsID))
				{
					TarotData data_tarot = this.Dic_TarotData[data.xmlData.GoodsID];
					TarotXmlData data_tarotxml = data_tarot.Get(data.xmlData.Level);
					if (data_tarotxml != null)
					{
						if (item.Level < MaxLevel)
						{
							int SuiPianGoodsId = data_tarot.GetNeedsGoodsId(data.xmlData.Level + 1);
							count = data_tarot.GetNeedsGoodsNum(data.xmlData.Level + 1);
						}
						else
						{
							int SuiPianGoodsId = data_tarot.GetNeedsGoodsId(data.xmlData.Level);
							count = data_tarot.GetNeedsGoodsNum(data.xmlData.Level);
						}
						item.UPEXP = count;
						TarotCardData tempData = null;
						if (this.m_TarotSystemData.TarotCardDatas != null)
						{
							tempData = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == item.ItemGoodsId);
						}
						if (tempData != null)
						{
							item.NowEXP = ((0 < tempData.TarotMoney) ? tempData.TarotMoney : 0);
						}
						else if (data.data != null)
						{
							item.NowEXP = ((0 < data.data.TarotMoney) ? data.data.TarotMoney : 0);
						}
						else
						{
							item.NowEXP = 0;
						}
					}
				}
				if (null != this.m_TaLuoPaiJiHuoPart && this.m_TaLuoPaiJiHuoPart.m_TaLuoPaiItem.ItemGoodsId == item.ItemGoodsId)
				{
					this.m_TaLuoPaiJiHuoPart.m_TaLuoPaiItem.ExtraLevel = item.ExtraLevel;
					this.m_TaLuoPaiJiHuoPart.RefreshProperty();
				}
			}
			if (this.m_ListShengJi.Count <= i + 1)
			{
				this.RefreshUIPeiDai();
				if (null != this.m_TaLuoKingPart || null != this.m_TaLuoPaiJiHuoPart || null != this.m_TaLuoPaiQieHuanPart)
				{
					this.SetFlag(false);
				}
				else
				{
					this.SetFlag(true);
				}
			}
		}
		yield break;
	}

	private void ShowTaLuoPaiKingPart(List<TaLuoPaiItem> m_ListItem)
	{
		if (null == this.m_TaLuoKingPart)
		{
			if (m_ListItem != null)
			{
				this.m_TaLuoKingWind = U3DUtils.NEW<GChildWindow>();
				this.m_TaLuoKingWind.ModalType = ChildWindowModalType.Translucent;
				this.m_TaLuoKingWind.transform.SetParent(Super.GData.PlayZoneRoot.Children.transform, false);
				this.m_TaLuoKingWind.transform.localPosition = new Vector3(0f, 0f, WindowManage.GetZ(base.transform.parent.parent) - 10f);
				UIEventListener.Get(this.m_TaLuoKingWind.ModalBak).onClick = delegate(GameObject e)
				{
					this.CloseKingPart();
				};
				this.m_TaLuoKingWind.ChildWindowClose = delegate(object e, EventArgs s)
				{
					this.CloseKingPart();
					return true;
				};
				this.m_TaLuoKingPart = U3DUtils.NEW<TaLuoPaiTeQuanPart>();
				this.m_TaLuoKingPart.StartTime = this.m_TarotSystemData.KingData.StartTime;
				this.m_TaLuoKingPart.BufferSecs = this.m_TarotSystemData.KingData.BufferSecs;
				this.m_TaLuoKingPart.ListTaLuoPaiItem = m_ListItem;
				this.m_TaLuoKingWind.Body.Add(this.m_TaLuoKingPart);
				this.m_TaLuoKingPart.StartTimer();
				this.m_TaLuoKingPart.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.CloseKingPart();
				};
			}
			this.m_KingFlag = true;
			return;
		}
		if (this.KingSuiPians <= 9999)
		{
			this.m_NeedsGoodsIcon.SText = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{1}/{0}", this.m_KingNumber, this.KingSuiPians)
			});
		}
		else
		{
			this.m_NeedsGoodsIcon.SText = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{1}+/{0}", this.m_KingNumber, 9999)
			});
		}
		this.m_TaLuoKingPart.StartTime = this.m_TarotSystemData.KingData.StartTime;
		this.m_TaLuoKingPart.BufferSecs = this.m_TarotSystemData.KingData.BufferSecs;
		this.m_TaLuoKingPart.setItem(this.sendKingList());
		this.m_KingFlag = true;
	}

	private void CloseKingPart()
	{
		this.SetFlag(true);
		if (null != this.m_TaLuoKingWind)
		{
			Object.Destroy(this.m_TaLuoKingWind.gameObject);
			Object.Destroy(this.m_TaLuoKingPart.gameObject);
			if (null != this.m_TaLuoKingPart.m_TaLuoPaiTeQuan2Part)
			{
				Object.Destroy(this.m_TaLuoKingPart.m_TaLuoPaiTeQuan2Part.gameObject);
			}
			if (null != this.m_TaLuoKingPart.m_TalLuoPaiTeQuan2Wind)
			{
				Object.Destroy(this.m_TaLuoKingPart.m_TalLuoPaiTeQuan2Wind.gameObject);
			}
			this.m_TaLuoKingPart.m_TaLuoPaiTeQuan2Part = null;
		}
		if (null != this.m_TaLuoKingPart)
		{
		}
		this.m_KingFlag = true;
		this.m_TaLuoKingWind = null;
		this.m_TaLuoKingPart = null;
	}

	private string GetAdornProperty()
	{
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		foreach (KeyValuePair<int, TarotDataAndXmlData> keyValuePair in this.m_DicKaPaiGuanLi)
		{
			TarotDataAndXmlData value = keyValuePair.Value;
			if (value.data != null && value.data.Postion != 0)
			{
				int goodId = value.data.GoodId;
				int num = value.data.Level;
				if (this.m_TarotSystemData.KingData != null && !this.m_TarotSystemData.KingData.KingIsOutTime() && this.m_TarotSystemData.KingData.AddtionDict != null && 0 < this.m_TarotSystemData.KingData.AddtionDict.Count)
				{
					foreach (KeyValuePair<int, int> keyValuePair2 in this.m_TarotSystemData.KingData.AddtionDict)
					{
						int key = keyValuePair2.Key;
						if (goodId == key)
						{
							int num2 = num;
							Dictionary<int, int>.Enumerator enumerator2;
							KeyValuePair<int, int> keyValuePair3 = enumerator2.Current;
							num = num2 + keyValuePair3.Value;
						}
					}
				}
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodId);
				if (goodsXmlNodeByID != null)
				{
					double[] equipProps = goodsXmlNodeByID.EquipProps;
					for (int i = 0; i < equipProps.Length; i++)
					{
						if (0.0 < equipProps[i])
						{
							if (dictionary.ContainsKey(i))
							{
								Dictionary<int, double> dictionary3;
								Dictionary<int, double> dictionary2 = dictionary3 = dictionary;
								int num4;
								int num3 = num4 = i;
								double num5 = dictionary3[num4];
								dictionary2[num3] = num5 + equipProps[i] * (double)num;
							}
							else
							{
								dictionary.Add(i, equipProps[i] * (double)num);
							}
						}
					}
				}
			}
		}
		string text = string.Empty;
		foreach (KeyValuePair<int, double> keyValuePair4 in dictionary)
		{
			int key2 = keyValuePair4.Key;
			Dictionary<int, double>.Enumerator enumerator3;
			KeyValuePair<int, double> keyValuePair5 = enumerator3.Current;
			double value2 = keyValuePair5.Value;
			if (key2 != 0)
			{
				if (ConfigExtPropIndexes.GetPercentByID(key2))
				{
					double num6 = value2 * 100.0;
					list2.Add(key2);
					list.Add(string.Format(Global.GetLang("{0}：{1}%"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key2, true)
					}), num6) + Environment.NewLine);
				}
				else
				{
					int num7 = (int)value2;
					list2.Add(key2);
					list.Add(string.Format(Global.GetLang("{0}：{1}"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key2, true)
					}), num7) + Environment.NewLine);
				}
			}
		}
		for (int j = 0; j < this.m_listID.Length; j++)
		{
			for (int k = 0; k < list2.Count; k++)
			{
				if (this.m_listID[j] == list2[k])
				{
					text += list[k];
				}
			}
		}
		return text;
	}

	private void UpDateProoerty()
	{
		this.m_Shuxing.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			string.Format("{0}", this.GetAdornProperty())
		});
		UIDraggablePanel componentInParent = this.m_Shuxing.GetComponentInParent<UIDraggablePanel>();
		if (null != componentInParent)
		{
			componentInParent.ResetPosition();
		}
	}

	public void StartTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer.Dispose();
			this.m_Timer = null;
		}
		this.m_Timer = new DispatcherTimer("TaLuoPaiPart_Timer");
		this.m_Timer.Interval = TimeSpan.FromSeconds(1.0);
		this.m_Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this.m_Timer.Start();
	}

	public void StopTimer()
	{
		if (this.m_Timer != null)
		{
			this.m_Timer.Tick = null;
			this.m_Timer.Stop();
			this.m_Timer = null;
		}
		this.Visibility = false;
	}

	private void Timer_Tick(object sender, object e)
	{
		if (this.m_Time > 0L)
		{
			this.m_Time -= 1L;
		}
		else if (this.m_KingTimeFlag)
		{
			this.m_KingTimeFlag = false;
			this.KingTimerEnd();
		}
	}

	public void KingTimerEnd()
	{
		if (this.m_TarotSystemData.KingData != null)
		{
			Super.HintMainText(Global.GetLang("国王特权效果消失"), 10, 3);
			this.m_TarotSystemData.KingData.AddtionDict = null;
			this.m_TarotSystemData.KingData = null;
			for (int i = 0; i < this.m_ListShengJi.Count; i++)
			{
				this.m_ListShengJi[i].ExtraLevel = 0;
			}
			for (int j = 0; j < this.m_ListGuanLi.Count; j++)
			{
				this.m_ListGuanLi[j].ExtraLevel = 0;
			}
			if (null != this.m_TaLuoKingPart)
			{
				this.CloseKingPart();
			}
			this.UpDateProoerty();
		}
	}

	public void RefreshUsingKingTeQuan(string[] data)
	{
		if (data != null && data.Length > 0)
		{
			int num = int.Parse(data[0]);
			if (num == 0)
			{
				if (data.Length < 7)
				{
					this.SetFlag(true);
					this.m_Time = 0L;
					this.KingTimerEnd();
					this.CloseKingPart();
					return;
				}
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int i = 1; i < data.Length; i += 2)
				{
					if (i + 1 < data.Length)
					{
						dictionary.Add(int.Parse(data[i]), int.Parse(data[i + 1]));
					}
				}
				foreach (KeyValuePair<int, int> keyValuePair in dictionary)
				{
					int key = keyValuePair.Key;
					Dictionary<int, int>.Enumerator enumerator;
					for (int j = 0; j < this.m_ListGuanLi.Count; j++)
					{
						TaLuoPaiItem taLuoPaiItem = this.m_ListGuanLi[j];
						if (key == taLuoPaiItem.ItemGoodsId)
						{
							KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
							if (keyValuePair2.Value > 0)
							{
								TaLuoPaiItem taLuoPaiItem2 = taLuoPaiItem;
								KeyValuePair<int, int> keyValuePair3 = enumerator.Current;
								taLuoPaiItem2.ExtraLevel = keyValuePair3.Value;
								break;
							}
						}
					}
					for (int k = 0; k < this.m_ListShengJi.Count; k++)
					{
						TaLuoPaiItem taLuoPaiItem3 = this.m_ListShengJi[k];
						if (key == taLuoPaiItem3.ItemGoodsId)
						{
							KeyValuePair<int, int> keyValuePair4 = enumerator.Current;
							if (keyValuePair4.Value > 0)
							{
								TaLuoPaiItem taLuoPaiItem4 = taLuoPaiItem3;
								KeyValuePair<int, int> keyValuePair5 = enumerator.Current;
								taLuoPaiItem4.ExtraLevel = keyValuePair5.Value;
								break;
							}
						}
					}
				}
				TarotKingData tarotKingData = new TarotKingData();
				tarotKingData.AddtionDict = dictionary;
				tarotKingData.StartTime = Global.GetCorrectLocalTime();
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TarotKingCost", ',');
				if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 3)
				{
					this.m_KingID = systemParamIntArrayByName[0];
					this.m_KingNumber = systemParamIntArrayByName[1];
					this.m_KingTimer = systemParamIntArrayByName[2] * 60;
				}
				if (this.m_KingNumber != this.KingSuiPians - Global.GetRoleGoodsNumberCountByGoodsID(this.m_KingID))
				{
					MUDebug.Log<string>(new string[]
					{
						"国王特权配表错误：服务器消耗数量和客户端配表数量不符"
					});
					this.m_KingNumber = this.KingSuiPians - Global.GetRoleGoodsNumberCountByGoodsID(this.m_KingID);
				}
				this.KingSuiPians = Global.GetRoleGoodsNumberCountByGoodsID(this.m_KingID);
				tarotKingData.BufferSecs = (long)this.m_KingTimer;
				if (this.m_ServiceBufferSecs != -1L)
				{
					if (this.m_ServiceBufferSecs == (long)this.m_KingTimer)
					{
						tarotKingData.BufferSecs = (long)this.m_KingTimer;
					}
					else
					{
						tarotKingData.BufferSecs = this.m_ServiceBufferSecs;
					}
				}
				else
				{
					tarotKingData.BufferSecs = (long)this.m_KingTimer;
				}
				if (this.m_TarotSystemData == null)
				{
					this.m_TarotSystemData = new TarotSystemData();
				}
				this.m_TarotSystemData.KingData = tarotKingData;
				this.m_StartTime = tarotKingData.StartTime;
				this.m_BufferSecs = tarotKingData.BufferSecs;
				this.m_Time = this.m_BufferSecs - (Global.GetCorrectLocalTime() - this.m_StartTime) / 1000L;
				if (this.KingSuiPians <= 9999)
				{
					this.m_NeedsGoodsIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{1}/{0}", this.m_KingNumber, this.KingSuiPians)
					});
				}
				else
				{
					this.m_NeedsGoodsIcon.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{1}+/{0}", this.m_KingNumber, 9999)
					});
				}
				this.ShowTaLuoPaiKingPart(this.sendKingList());
				this.m_KingTimeFlag = true;
				Super.HintMainText(Global.GetLang("国王特权获取成功"), 10, 3);
			}
			else
			{
				this.m_KingFlag = true;
				this.SetFlag(true);
				TaLuoPaiPart.ErrorLog((TaLuoPaiError)num);
			}
		}
		this.UpDateProoerty();
	}

	public void RefreshData(TarotSystemData data)
	{
		if (this.Dic_TarotData.Count == 0 || this.m_DicKaPaiGuanLi.Count == 0)
		{
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"重要事故 ，塔罗牌读表错误:Dic_TarotData.Count=",
					this.Dic_TarotData.Count,
					":m_DicKaPaiGuanLi.Count=",
					this.m_DicKaPaiGuanLi.Count
				})
			});
			Super.HintMainText(Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{0}", Global.GetLang("读表错误"))
			}), 10, 3);
			this.StopTimer();
			PlayZone.GlobalPlayZone.CloseTaLuoPaiWindow();
			return;
		}
		if (data.KingData != null && data.KingData.AddtionDict != null)
		{
			this.m_ServiceBufferSecs = data.KingData.BufferSecs;
			this.m_StartTime = data.KingData.StartTime;
			this.m_BufferSecs = data.KingData.BufferSecs;
			if (this.m_StartTime != -1L && this.m_BufferSecs != -1L)
			{
				this.m_Time = this.m_BufferSecs - (Global.GetCorrectLocalTime() - this.m_StartTime) / 1000L;
				this.StartTimer();
			}
		}
		bool flag = null != this.m_TarotSystemData;
		this.m_TarotSystemData = data;
		if (flag)
		{
			if (this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null && 0 < this.m_TarotSystemData.KingData.AddtionDict.Count)
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.m_TarotSystemData.KingData.AddtionDict)
				{
					int key = keyValuePair.Key;
					Dictionary<int, int>.Enumerator enumerator;
					for (int i = 0; i < this.m_ListGuanLi.Count; i++)
					{
						TaLuoPaiItem taLuoPaiItem = this.m_ListGuanLi[i];
						if (key == taLuoPaiItem.ItemGoodsId)
						{
							KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
							if (keyValuePair2.Value > 0)
							{
								TaLuoPaiItem taLuoPaiItem2 = taLuoPaiItem;
								KeyValuePair<int, int> keyValuePair3 = enumerator.Current;
								taLuoPaiItem2.ExtraLevel = keyValuePair3.Value;
								break;
							}
						}
					}
					for (int j = 0; j < this.m_ListShengJi.Count; j++)
					{
						TaLuoPaiItem taLuoPaiItem3 = this.m_ListShengJi[j];
						if (key == taLuoPaiItem3.ItemGoodsId)
						{
							KeyValuePair<int, int> keyValuePair4 = enumerator.Current;
							if (keyValuePair4.Value > 0)
							{
								TaLuoPaiItem taLuoPaiItem4 = taLuoPaiItem3;
								KeyValuePair<int, int> keyValuePair5 = enumerator.Current;
								taLuoPaiItem4.ExtraLevel = keyValuePair5.Value;
								break;
							}
						}
					}
				}
			}
			if (this.m_TarotSystemData.TarotCardDatas != null)
			{
				List<TarotCardData> tarotCardDatas = this.m_TarotSystemData.TarotCardDatas;
				for (int k = 0; k < tarotCardDatas.Count; k++)
				{
					if (tarotCardDatas[k] != null)
					{
						for (int l = 0; l < this.m_ListGuanLi.Count; l++)
						{
							TaLuoPaiItem taLuoPaiItem5 = this.m_ListGuanLi[l];
							if (tarotCardDatas[k].GoodId == taLuoPaiItem5.ItemGoodsId)
							{
								taLuoPaiItem5.NowEXP = tarotCardDatas[k].TarotMoney;
								break;
							}
						}
						for (int m = 0; m < this.m_ListShengJi.Count; m++)
						{
							TaLuoPaiItem taLuoPaiItem6 = this.m_ListShengJi[m];
							if (tarotCardDatas[k].GoodId == taLuoPaiItem6.ItemGoodsId)
							{
								taLuoPaiItem6.NowEXP = tarotCardDatas[k].TarotMoney;
								break;
							}
						}
					}
				}
			}
		}
		else
		{
			for (int n = 0; n < 6; n++)
			{
				TaLuoPaiItem taLuoPaiItem7 = U3DUtils.NEW<TaLuoPaiItem>();
				U3DUtils.AddChild(this.m_PanKaPaiGuanLi.gameObject, taLuoPaiItem7.gameObject, false);
				taLuoPaiItem7.transform.localScale = new Vector3(0.56f, 0.56f, 1f);
				taLuoPaiItem7.transform.localPosition = this.m_VecPosition[n];
				taLuoPaiItem7.transform.Rotate(this.m_VecRotate[n]);
				taLuoPaiItem7.IsActivate = false;
				taLuoPaiItem7.BScale = true;
				taLuoPaiItem7.BPeiDai = false;
				taLuoPaiItem7.Type = 2;
				taLuoPaiItem7.Pos = n;
				if (null != taLuoPaiItem7.gameObject.GetComponent<BoxCollider>())
				{
					taLuoPaiItem7.gameObject.GetComponent<BoxCollider>().size = new Vector3(240f, 319f, 1f);
				}
				UIEventListener.Get(taLuoPaiItem7.gameObject).onClick = new UIEventListener.VoidDelegate(this.TaLuoPaiItemClick);
				this.m_ListGuanLi.Add(taLuoPaiItem7);
			}
			if (this.m_TarotSystemData.TarotCardDatas != null)
			{
				for (int num = 0; num < this.m_TarotSystemData.TarotCardDatas.Count; num++)
				{
					TarotCardData tarotCardData = this.m_TarotSystemData.TarotCardDatas[num];
					if (tarotCardData.Postion != 0)
					{
						int num2 = (int)(tarotCardData.Postion - 1);
						if (0 <= num2 && num2 < this.m_ListGuanLi.Count)
						{
							TaLuoPaiItem m_Item = this.m_ListGuanLi[num2];
							m_Item.IsActivate = true;
							m_Item.BPeiDai = true;
							m_Item.ItemGoodsId = tarotCardData.GoodId;
							m_Item.Level = tarotCardData.Level;
							m_Item.Name = this.m_DicKaPaiGuanLi[tarotCardData.GoodId].xmlData.Name;
							int maxlevel = this.Dic_TarotData[m_Item.ItemGoodsId].Maxlevel;
							m_Item.MaxLevel = maxlevel;
							int num3 = -1;
							int num4 = -1;
							if (m_Item.Level < maxlevel)
							{
								string[] array = this.Dic_TarotData[tarotCardData.GoodId].Get(tarotCardData.Level + 1).NeedGoods.Split(new char[]
								{
									','
								});
								if (array.Length == 2)
								{
									num3 = int.Parse(array[0]);
									num4 = int.Parse(array[1]);
								}
							}
							else
							{
								string[] array2 = this.Dic_TarotData[tarotCardData.GoodId].Get(tarotCardData.Level).NeedGoods.Split(new char[]
								{
									','
								});
								if (array2.Length == 2)
								{
									num3 = int.Parse(array2[0]);
									num4 = int.Parse(array2[1]);
								}
							}
							if (num4 != -1 && num3 != -1)
							{
								m_Item.UPEXP = num4;
								TarotCardData tarotCardData2 = null;
								if (this.m_TarotSystemData.TarotCardDatas != null)
								{
									tarotCardData2 = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == m_Item.ItemGoodsId);
								}
								if (tarotCardData2 != null)
								{
									m_Item.NowEXP = ((0 < tarotCardData2.TarotMoney) ? tarotCardData2.TarotMoney : 0);
								}
								else
								{
									m_Item.NowEXP = 0;
								}
							}
							if (this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null)
							{
								if (this.m_TarotSystemData.KingData.AddtionDict.ContainsKey(m_Item.ItemGoodsId))
								{
									m_Item.ExtraLevel = this.m_TarotSystemData.KingData.AddtionDict[m_Item.ItemGoodsId];
								}
							}
							else
							{
								m_Item.ExtraLevel = 0;
							}
						}
					}
				}
			}
			if (data.TarotCardDatas != null)
			{
				for (int num5 = 0; num5 < data.TarotCardDatas.Count; num5++)
				{
					TarotCardData tarotCardData3 = data.TarotCardDatas[num5];
					if (tarotCardData3.Level != 0)
					{
						if (this.m_DicKaPaiGuanLi.ContainsKey(tarotCardData3.GoodId))
						{
							this.m_DicKaPaiGuanLi[tarotCardData3.GoodId].data = tarotCardData3;
						}
					}
				}
			}
			base.StartCoroutine<bool>(this.AddTarotXmlData(this.RefreshItemAndData()));
		}
		this.UpDateProoerty();
		this.SetFlag(true);
		NGUITools.SetActive(this.m_PanKaPaiShengJi.gameObject, false);
		if (null != this.m_TaLuoPaiJiHuoPart && NGUITools.GetActive(this.m_TaLuoPaiJiHuoPart.gameObject) && this.m_TaLuoPaiJiHuoPart.TarotCardData != null)
		{
			TarotCardData tarotCardData4 = data.TarotCardDatas.Find((TarotCardData e) => e.GoodId == this.m_TaLuoPaiJiHuoPart.TarotCardData.GoodId);
			if (tarotCardData4 != null)
			{
				this.m_TaLuoPaiJiHuoPart.TarotCardData = tarotCardData4;
			}
		}
		this.m_Tab.SetActivePage(this.m_Tab.SelectIndex);
	}

	public void RefreshPeiDaiTCP(string[] dataString)
	{
		TaLuoPaiError taLuoPaiError = (TaLuoPaiError)int.Parse(dataString[0]);
		if (taLuoPaiError == TaLuoPaiError.Success)
		{
			int num = Convert.ToInt32(dataString[1]);
			if (null != this.m_TaLuoPaiQieHuanPart)
			{
				int goodId = int.Parse(dataString[1]);
				this.m_TaLuoPaiQieHuanPart.RefreshItem(goodId);
			}
			if (this.m_DicKaPaiGuanLi.ContainsKey(this.m_PeiDaiGoodId))
			{
				this.m_DicKaPaiGuanLi[this.m_PeiDaiGoodId].data.Postion = 0;
			}
			TaLuoPaiItem item = this.m_ListGuanLi[this.CheckIndex];
			if (item.ItemGoodsId == num)
			{
				item.SetFlag = false;
				item.BPeiDai = false;
				item.ItemGoodsId = 0;
				if (this.m_DicKaPaiGuanLi.ContainsKey(num))
				{
					this.m_DicKaPaiGuanLi[num].data.Postion = 0;
				}
			}
			else
			{
				this.m_PeiDaiGoodId = num;
				if (this.m_ListGuanLi.Count > this.CheckIndex && null != item && this.m_DicKaPaiGuanLi.ContainsKey(num))
				{
					TarotDataAndXmlData tarotDataAndXmlData = this.m_DicKaPaiGuanLi[num];
					item.BPeiDai = true;
					tarotDataAndXmlData.data.Postion = (byte)((!item.BPeiDai) ? 0 : (this.CheckIndex + 1));
					item.ItemGoodsId = num;
					if (tarotDataAndXmlData.data != null)
					{
						TarotCardData tarotCardData = null;
						item.ItemGoodsId = tarotDataAndXmlData.data.GoodId;
						if (item.ItemGoodsId == 0)
						{
							item.ItemGoodsId = tarotDataAndXmlData.xmlData.GoodsID;
						}
						item.IsActivate = true;
						item.Level = tarotDataAndXmlData.data.Level;
						item.Name = tarotDataAndXmlData.xmlData.Name;
						item.MaxLevel = this.Dic_TarotData[item.ItemGoodsId].Maxlevel;
						if (item.Level < item.MaxLevel)
						{
							string[] array = this.Dic_TarotData[item.ItemGoodsId].Get(item.Level + 1).NeedGoods.Split(new char[]
							{
								','
							});
							int upexp = -1;
							if (array.Length == 2)
							{
								int num2 = int.Parse(array[0]);
								upexp = int.Parse(array[1]);
							}
							item.UPEXP = upexp;
							if (this.m_TarotSystemData.TarotCardDatas != null)
							{
								tarotCardData = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == item.ItemGoodsId);
							}
							if (tarotCardData != null)
							{
								item.NowEXP = ((0 < tarotCardData.TarotMoney) ? tarotCardData.TarotMoney : 0);
							}
							else
							{
								item.NowEXP = 0;
							}
							if (!item.BPeiDai)
							{
								item.NowEXP = 0;
							}
						}
						else
						{
							string[] array2 = this.Dic_TarotData[item.ItemGoodsId].Get(item.Level).NeedGoods.Split(new char[]
							{
								','
							});
							int upexp2 = -1;
							if (array2.Length == 2)
							{
								int num3 = int.Parse(array2[0]);
								upexp2 = int.Parse(array2[1]);
							}
							item.UPEXP = upexp2;
							if (this.m_TarotSystemData.TarotCardDatas != null)
							{
								tarotCardData = this.m_TarotSystemData.TarotCardDatas.Find((TarotCardData t) => t.GoodId == item.ItemGoodsId);
							}
							if (tarotCardData != null)
							{
								item.NowEXP = ((0 < tarotCardData.TarotMoney) ? tarotCardData.TarotMoney : 0);
							}
							else
							{
								item.NowEXP = 0;
							}
							if (!item.BPeiDai)
							{
								item.NowEXP = 0;
							}
						}
					}
				}
			}
			this.UpDateProoerty();
			this.RefreshUIPeiDai();
		}
		else
		{
			TaLuoPaiPart.ErrorLog(taLuoPaiError);
		}
	}

	public void SendData(string[] dataString)
	{
		if (null != this.m_TaLuoPaiJiHuoPart)
		{
			this.m_TaLuoPaiJiHuoPart.SendData(dataString);
		}
		if (dataString != null)
		{
			if (dataString.Length == 2)
			{
				if ("0" == dataString[0].ToString())
				{
					int num = 0;
					if (int.TryParse(dataString[1], ref num))
					{
						if (this.m_DicKaPaiGuanLi.ContainsKey(num))
						{
							TarotDataAndXmlData tarotDataAndXmlData = this.m_DicKaPaiGuanLi[num];
							if (tarotDataAndXmlData.data == null)
							{
								tarotDataAndXmlData.data = new TarotCardData
								{
									GoodId = num,
									Level = 1
								};
								tarotDataAndXmlData.xmlData.Level = 1;
							}
							else
							{
								tarotDataAndXmlData.xmlData.Level = tarotDataAndXmlData.data.Level;
							}
						}
						base.StartCoroutine<bool>(this.RefreshUI(this.RefreshItemAndData()));
					}
				}
				this.GetData();
			}
			else
			{
				TaLuoPaiError error = (TaLuoPaiError)int.Parse(dataString[0]);
				TaLuoPaiPart.ErrorLog(error);
			}
		}
		this.SetFlag(false);
	}

	private List<TarotDataAndXmlData> RefreshItemAndData()
	{
		List<TarotDataAndXmlData> list = new List<TarotDataAndXmlData>();
		List<TarotDataAndXmlData> list2 = new List<TarotDataAndXmlData>();
		List<TarotDataAndXmlData> list3 = new List<TarotDataAndXmlData>();
		foreach (KeyValuePair<int, TarotDataAndXmlData> keyValuePair in this.m_DicKaPaiGuanLi)
		{
			TarotCardData data = keyValuePair.Value.data;
			if (data != null)
			{
				if (data.Postion != 0)
				{
					List<TarotDataAndXmlData> list4 = list;
					Dictionary<int, TarotDataAndXmlData>.Enumerator enumerator;
					KeyValuePair<int, TarotDataAndXmlData> keyValuePair2 = enumerator.Current;
					list4.Add(keyValuePair2.Value);
				}
				else
				{
					List<TarotDataAndXmlData> list5 = list2;
					Dictionary<int, TarotDataAndXmlData>.Enumerator enumerator;
					KeyValuePair<int, TarotDataAndXmlData> keyValuePair3 = enumerator.Current;
					list5.Add(keyValuePair3.Value);
				}
			}
			else
			{
				List<TarotDataAndXmlData> list6 = list3;
				Dictionary<int, TarotDataAndXmlData>.Enumerator enumerator;
				KeyValuePair<int, TarotDataAndXmlData> keyValuePair4 = enumerator.Current;
				list6.Add(keyValuePair4.Value);
			}
		}
		if (0 < list.Count)
		{
			list.Sort(new Comparison<TarotDataAndXmlData>(this.PaiXu));
		}
		if (0 < list2.Count)
		{
			list2.Sort(new Comparison<TarotDataAndXmlData>(this.PaiXu));
		}
		if (0 < list3.Count)
		{
			list3.Sort(new Comparison<TarotDataAndXmlData>(this.PaiXu));
		}
		List<TarotDataAndXmlData> list7 = new List<TarotDataAndXmlData>();
		if (0 < list.Count)
		{
			list7.AddRange(list);
		}
		if (0 < list2.Count)
		{
			list7.AddRange(list2);
		}
		if (0 < list3.Count)
		{
			list7.AddRange(list3);
		}
		return list7;
	}

	public static void ErrorLog(TaLuoPaiError error)
	{
		string chineseText = string.Empty;
		switch (error + 1)
		{
		case TaLuoPaiError.Success:
			chineseText = Global.GetLang("非常规出错");
			break;
		case TaLuoPaiError.Fail:
			chineseText = Global.GetLang("成功");
			break;
		case TaLuoPaiError.MaxLevel:
			chineseText = Global.GetLang("失败");
			break;
		case TaLuoPaiError.NeedPart:
			chineseText = Global.GetLang("已达到最高等级");
			break;
		case TaLuoPaiError.PartSuitIsMax:
			chineseText = Global.GetLang("塔罗牌不足");
			break;
		case TaLuoPaiError.NotOpen:
			chineseText = Global.GetLang("部件已满级");
			break;
		case TaLuoPaiError.PartNumError:
			chineseText = Global.GetLang("功能未开启");
			break;
		case TaLuoPaiError.PosError:
			chineseText = Global.GetLang("碎片使用过多");
			break;
		case TaLuoPaiError.ItemNotEnough:
			chineseText = Global.GetLang("上阵位置有卡牌");
			break;
		case TaLuoPaiError.HasMaxNum:
			chineseText = Global.GetLang("道具不足");
			break;
		case TaLuoPaiError.MoneyNumError:
			chineseText = Global.GetLang("使用数量已达到达最大值");
			break;
		case TaLuoPaiError.GoodIdError:
			chineseText = string.Empty;
			break;
		case TaLuoPaiError.NotFindCard:
			chineseText = string.Empty;
			break;
		}
		Super.HintMainText(Global.GetLang(chineseText), 10, 3);
	}

	private List<TaLuoPaiItem> sendKingList()
	{
		List<TaLuoPaiItem> list = new List<TaLuoPaiItem>();
		if (this.m_ListShengJi != null && 0 < this.m_ListShengJi.Count && this.m_TarotSystemData.KingData != null && this.m_TarotSystemData.KingData.AddtionDict != null)
		{
			for (int i = 0; i < this.m_ListShengJi.Count; i++)
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.m_TarotSystemData.KingData.AddtionDict)
				{
					int key = keyValuePair.Key;
					if (key >= 0)
					{
						if (this.m_ListShengJi[i].ItemGoodsId == key)
						{
							TaLuoPaiItem taLuoPaiItem = this.m_ListShengJi[i];
							Dictionary<int, int>.Enumerator enumerator;
							KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
							taLuoPaiItem.ExtraLevel = keyValuePair2.Value;
							list.Add(this.m_ListShengJi[i]);
							break;
						}
						this.m_ListShengJi[i].ExtraLevel = 0;
					}
				}
			}
		}
		return list;
	}

	public void KingInit()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TarotKingCost", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 3)
		{
			this.m_KingID = systemParamIntArrayByName[0];
			this.m_KingNumber = systemParamIntArrayByName[1];
			this.m_KingTimer = systemParamIntArrayByName[2] * 60;
		}
		this.KingSuiPians = Global.GetRoleGoodsNumberCountByGoodsID(this.m_KingID);
		if (null != this.m_NeedsGoodsIcon && this.m_NeedsGoodsIcon.ItemCode != this.m_KingID)
		{
			Object.Destroy(this.m_NeedsGoodsIcon.gameObject);
			this.m_NeedsGoodsIcon = null;
		}
		if (this.m_NeedsGoodsIcon == null)
		{
			this.m_NeedsGoodsIcon = this.initGood(Global.GetEmptyGoodsData(this.m_KingID, 1, 1, 0, 1, 1, 1, 1, 1), true);
			this.m_NeedsGoodsIcon.transform.SetParent(this.m_KingDaoJu, false);
			this.m_NeedsGoodsIcon.transform.localPosition = new Vector3(0f, 0f, -0.8f);
			this.m_NeedsGoodsIcon.SecondText.Label.supportEncoding = true;
		}
		if (this.KingSuiPians <= 9999)
		{
			this.m_NeedsGoodsIcon.SText = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{1}/{0}", this.m_KingNumber, this.KingSuiPians)
			});
		}
		else
		{
			this.m_NeedsGoodsIcon.SText = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{1}+/{0}", this.m_KingNumber, 9999)
			});
		}
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			if (BHaveTips)
			{
				ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.ShowGoodsTip(e);
				};
			}
		}
		return ggoodIcon;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	public GButton m_BtnClose;

	public GTab m_Tab;

	public UIPanel m_PanKaPaiGuanLi;

	public GButton m_BtnTeQuan;

	public TextBlock m_Shuxing;

	public UILabel m_ShuXingTitle;

	public UIPanel m_PanKaPaiShengJi;

	public ListBox m_ListBox;

	public Transform m_KingDaoJu;

	public UILabel m_TitleShuxing;

	private List<TaLuoPaiItem> m_ListGuanLi;

	private List<TaLuoPaiItem> m_ListShengJi;

	private ObservableCollection m_ObservableCollection_ListBox;

	private Dictionary<int, TarotData> Dic_TarotData = new Dictionary<int, TarotData>();

	private Vector3[] m_VecPosition = new Vector3[]
	{
		new Vector3(-349f, 50f, -1f),
		new Vector3(-135f, 52.5f, -1f),
		new Vector3(80f, 50f, -1f),
		new Vector3(-314f, -140f, -1f),
		new Vector3(-135f, -141f, -1f),
		new Vector3(44f, -140f, -1f)
	};

	private Vector3[] m_VecRotate = new Vector3[]
	{
		new Vector3(0f, 0f, 7.4f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, -6.2f),
		new Vector3(0f, 0f, 6.6f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, -6.6f)
	};

	private TarotSystemData m_TarotSystemData;

	private List<GoodVO> m_TaLuoPailst = new List<GoodVO>();

	private GChildWindow m_TalLuoPaiJiHuoWind;

	private TaLuoPaiJiHuoPart m_TaLuoPaiJiHuoPart;

	private GChildWindow m_TaLuoKingWind;

	private TaLuoPaiTeQuanPart m_TaLuoKingPart;

	private GGoodIcon m_NeedsGoodsIcon;

	private List<GoodsData> m_ListKingDaoJu = new List<GoodsData>();

	private int m_KingID = -1;

	private int m_KingNumber = -1;

	private int m_KingTimer = -1;

	private long m_ServiceBufferSecs = -1L;

	private int KingSuiPians;

	private long m_QieHuanTime;

	private long m_QieHuankey = 1000L;

	private int m_QieHuanCount;

	private DispatcherTimer m_Timer;

	private long m_BufferSecs = -1L;

	private long m_StartTime = -1L;

	private long m_Time = -1L;

	private int m_PeiDaiGoodId;

	private bool m_KingFlag = true;

	private bool m_KingTimeFlag = true;

	private Dictionary<int, TarotDataAndXmlData> m_DicKaPaiGuanLi = new Dictionary<int, TarotDataAndXmlData>();

	private TaLuoPaiQieHuan m_TaLuoPaiQieHuanPart;

	private GChildWindow m_TaLuoPaiQieHuanWind;

	private int[] m_listID = new int[]
	{
		13,
		14,
		3,
		4,
		5,
		6,
		7,
		8,
		9,
		10,
		18,
		30,
		54,
		19,
		55,
		26,
		27,
		37,
		38,
		39,
		40,
		29,
		44,
		67,
		45,
		91,
		46,
		93
	};

	private int CheckIndex;
}
