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

public class ShiPinPageOne : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.m_OrnamentSitexmlData = new ShiPinPageOne.OrnamentSitexmlData();
		if (null != this._PropListBox)
		{
			this.m_ObservableCollection = this._PropListBox.ItemsSource;
		}
		this._NoUpFull.SetActive(true);
		this._UpFull.SetActive(false);
	}

	public override void OnActive(bool active)
	{
		base.OnActive(active);
		base.gameObject.SetActive(active);
		this.m_Active = active;
		if (active)
		{
			Super.ShowNetWaiting(null);
			base.StartCoroutine<bool>(this.CarryHanderWaitForSeconds(new ShiPinPageOne.voidDelegate(Super.HideNetWaiting), 0.5f));
			if (this.m_OrnamentDataLst != null)
			{
				base.StartCoroutine(this.InitIcon(this.m_OrnamentDataLst));
			}
			else
			{
				base.StartCoroutine(this.InitIcon(null));
			}
		}
		else
		{
			Super.HideNetWaiting();
			for (int i = 0; i < this.m_IconLst.Count; i++)
			{
				if (null != this.m_IconLst[i])
				{
					this.m_IconLst[i].ShowTeXiao(false);
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Super.HideNetWaiting();
	}

	private IEnumerator CarryHanderWaitForSeconds(ShiPinPageOne.voidDelegate hander, float time)
	{
		yield return new WaitForSeconds(time);
		hander();
		yield break;
	}

	private IEnumerator InitIcon(List<OrnamentData> datalst)
	{
		Super.ShowNetWaiting(null);
		if (this.m_IconLst.Count == 0)
		{
			yield return null;
		}
		if (this.m_IconLst.Count == 0)
		{
			for (int i = 0; i < 5; i++)
			{
				ShiPinIconitem item = U3DUtils.NEW<ShiPinIconitem>();
				item.SetTrInf(this.m_IconPos[i], this._IconRoot);
				item.Index = i;
				item.Hander = new DPSelectedItemEventHandler(this.IconItemClick);
				item.IsLock = true;
				this.m_IconLst.Add(item);
				item.name = i.ToString();
			}
		}
		if (datalst != null)
		{
			int dataLstCount = datalst.Count;
			if (0 < dataLstCount)
			{
				for (int j = 0; j < dataLstCount; j++)
				{
					OrnamentData d = datalst[j];
					if (d != null && 6 > d.ID && 1 <= d.ID)
					{
						ShiPinIconitem item2 = null;
						int index = Mathf.Abs(d.ID) - 1;
						for (int k = 0; k < this.m_IconLst.Count; k++)
						{
							if (null != this.m_IconLst[j] && index == this.m_IconLst[k].Index)
							{
								item2 = this.m_IconLst[k];
								break;
							}
						}
						if (null != item2)
						{
							item2.HaveTips = false;
							item2.IsLock = false;
							item2.Level = d.Param1;
						}
					}
				}
			}
			List<GoodsData> lst = Global.GetRoleDecorationList();
			if (0 < lst.Count)
			{
				for (int l = 0; l < this.m_IconLst.Count; l++)
				{
					int index2 = this.m_IconLst[l].Index + 1;
					GoodsData d2 = lst.Find((GoodsData e) => e.BagIndex == index2 && 1 == e.Using);
					if (d2 != null)
					{
						this.m_IconLst[l].GoodsId = d2.GoodsID;
						this.m_IconLst[l].ShowLock = false;
						this.m_IconLst[l].IsGoods = true;
						this.m_IconLst[l].Tips = false;
						this.m_IconLst[l].bShowUnLoadBtn = true;
						this.m_IconLst[l].haveShiPin = true;
						this.m_IconLst[l].Level = this.m_IconLst[l].Level;
					}
				}
			}
		}
		List<ShiPinIconitem> its = this.m_IconLst.FindAll((ShiPinIconitem e) => e.haveShiPin && 0 < e.GoodsId);
		if (its != null && 0 < its.Count)
		{
			its.Sort((ShiPinIconitem a, ShiPinIconitem b) => a.Index - b.Index);
			its[0].Level = its[0].Level;
			this.RefreshProperty(its[0].GoodsId, its[0].Level);
			for (int m = 0; m < this.m_IconLst.Count; m++)
			{
				if (this.m_IconLst[m].haveShiPin && this.m_IconLst[m].GoodsId == its[0].GoodsId)
				{
					this.m_IconLst[m].BSelect = true;
				}
				else
				{
					this.m_IconLst[m].BSelect = false;
				}
			}
		}
		else
		{
			int GoodsId = 0;
			int level = 0;
			if (this.m_IconLst[0].haveShiPin)
			{
				GoodsId = this.m_IconLst[0].GoodsId;
				level = this.m_IconLst[0].Level;
			}
			this.RefreshProperty(GoodsId, level);
			this.m_IconLst[0].BSelect = true;
		}
		this.RefreshCaoWei();
		base.StartCoroutine<bool>(this.CarryHanderWaitForSeconds(new ShiPinPageOne.voidDelegate(Super.HideNetWaiting), 0.3f));
		yield break;
	}

	private void IconItemClick(object sender, DPSelectedItemEventArgs args)
	{
		for (int i = 0; i < this.m_IconLst.Count; i++)
		{
			ShiPinIconitem shiPinIconitem = this.m_IconLst[i];
			if (args != null)
			{
				if (0 > args.ID)
				{
					if (args.ID == -1 && args.Index == i)
					{
						if (this.Hander != null)
						{
							this.Hander(sender, new DPSelectedItemEventArgs
							{
								ID = args.ID,
								IDType = 1,
								Index = args.Index,
								Flag = (shiPinIconitem.IsLock ? 1 : 0)
							});
						}
						break;
					}
				}
				else if (args.Index == i)
				{
					shiPinIconitem.BSelect = true;
					if (shiPinIconitem.haveShiPin)
					{
						this.RefreshProperty(shiPinIconitem.GoodsId, shiPinIconitem.Level);
					}
					else
					{
						this.RefreshProperty(0, shiPinIconitem.Level);
					}
					if (!shiPinIconitem.haveShiPin)
					{
						if (this.Hander != null)
						{
							this.Hander(sender, new DPSelectedItemEventArgs
							{
								ID = args.ID,
								IDType = 2,
								Index = args.Index,
								Flag = (shiPinIconitem.IsLock ? 1 : 0),
								Level = shiPinIconitem.Level
							});
						}
					}
				}
				else
				{
					shiPinIconitem.BSelect = false;
				}
			}
		}
		for (int j = 0; j < this.m_IconLst.Count; j++)
		{
			if (j == args.Index)
			{
				this.m_IconLst[j].BSelect = true;
			}
			else
			{
				this.m_IconLst[j].BSelect = false;
			}
		}
	}

	private void InitPrefabText()
	{
		this._page1TitleLabels[0].text = string.Empty;
		this._page1TitleLabels[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("饰品槽位升级")
		});
		this._XiaoHaoLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("消耗：")
		});
		if (null != this._UpBtn.Label)
		{
			this._UpBtn.Label.text = Global.GetLang("升级");
		}
		this._UpBtn.Label.text = Global.GetLang("升级");
		this._EmptyLoadLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("目前未佩戴任何饰品")
		});
	}

	private void InitTexture()
	{
		if (null != this._Page1Bak)
		{
			this._Page1Bak.URL = "NetImages/GameRes/Images/ShipinPart/ShiPinPeiDaiBg.jpg";
		}
	}

	private void InitHandler()
	{
		this._UpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			ShiPinIconitem shiPinIconitem = this.m_IconLst.Find((ShiPinIconitem d) => d.BSelect);
			if (null != shiPinIconitem)
			{
				if (this.UpBtnCanClick)
				{
					if (shiPinIconitem.IsLock)
					{
						Super.HintMainText(Global.GetLang("请先解锁槽位"), 10, 3);
					}
					else if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.CharmPoint) >= this.m_OrnamentSitexmlData.GetCost(shiPinIconitem.Level))
					{
						GameInstance.Game.SendShiPinForge(shiPinIconitem.Index + 1);
						Super.ShowNetWaiting(null);
					}
					else
					{
						Super.HintMainText(Global.GetLang("魅力点不足"), 10, 3);
					}
					this.UpBtnCanClick = false;
					base.StartCoroutine(this.CarryHanderWaitForSeconds(delegate
					{
						this.UpBtnCanClick = true;
					}, 0.5f));
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("请选择一个槽位"), 10, 3);
			}
		};
		this._GroupBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.ShowGroupPart();
		};
		this._PropBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.ShowPropPart();
		};
	}

	private string[] GetPropStr()
	{
		List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
		string[] array = new string[]
		{
			Global.GetLang("属性总览"),
			string.Empty
		};
		List<ShiPinPageOne.PropData> list = new List<ShiPinPageOne.PropData>();
		if (this.m_OrnamentDataLst != null && 0 < this.m_OrnamentDataLst.Count && 0 < roleDecorationList.Count)
		{
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int i = 0; i < roleDecorationList.Count; i++)
			{
				GoodsData d = roleDecorationList[i];
				if (roleDecorationList[i] != null)
				{
					ShiPinIconitem shiPinIconitem = this.m_IconLst.Find((ShiPinIconitem e) => e.Index == d.BagIndex - 1);
					int num = 1;
					if (null != shiPinIconitem && roleDecorationList[i].Using == 1)
					{
						num = shiPinIconitem.Level;
					}
					if (1 > num)
					{
						num = 1;
					}
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(d.GoodsID);
					if (goodsXmlNodeByID != null)
					{
						double[] equipProps = goodsXmlNodeByID.EquipProps;
						if (equipProps != null && 0 < equipProps.Length)
						{
							for (int j2 = 1; j2 < equipProps.Length; j2++)
							{
								if (0.0 < equipProps[j2])
								{
									float num2 = (float)((double)num * 0.2 + 0.8) * (float)equipProps[j2];
									if (dictionary.ContainsKey(j2))
									{
										Dictionary<int, float> dictionary3;
										Dictionary<int, float> dictionary2 = dictionary3 = dictionary;
										int num4;
										int num3 = num4 = j2;
										float num5 = dictionary3[num4];
										dictionary2[num3] = num5 + num2;
									}
									else
									{
										dictionary.Add(j2, num2);
									}
								}
							}
						}
					}
				}
			}
			List<ShiPinPageOne.XmlData> list2 = new List<ShiPinPageOne.XmlData>();
			List<GoodsData> list3 = Global.GetRoleDecorationList().FindAll((GoodsData e) => 1 == e.Using);
			XElement gameResXml = Global.GetGameResXml("Config/OrnamentGroup.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "OrnamentGroup");
				if (xelementList != null && 0 < xelementList.Count)
				{
					for (int k = 0; k < xelementList.Count; k++)
					{
						ShiPinPageOne.XmlData xmlData = default(ShiPinPageOne.XmlData);
						xmlData.xml = xelementList[k];
						xmlData.Number = 0;
						xmlData.HaveActive = true;
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[k], "OrnamentGoods");
						if (!string.IsNullOrEmpty(xelementAttributeStr))
						{
							string[] OrnamentGoods_A = xelementAttributeStr.Split(new char[]
							{
								'|'
							});
							if (OrnamentGoods_A != null && 0 < OrnamentGoods_A.Length)
							{
								int j;
								for (j = 0; j < OrnamentGoods_A.Length; j++)
								{
									if (list3.Find((GoodsData e) => OrnamentGoods_A[j] == e.GoodsID.ToString()) == null)
									{
										xmlData.HaveActive = false;
									}
									else
									{
										xmlData.Number++;
									}
								}
							}
						}
						if (xmlData.HaveActive)
						{
							list2.Add(xmlData);
						}
					}
				}
			}
			if (0 < list2.Count)
			{
				for (int l = 0; l < list2.Count; l++)
				{
					string xelementAttrStr = list2[l].xml.GetXElementAttrStr("GroupProperty");
					if (!string.IsNullOrEmpty(xelementAttrStr))
					{
						string[] array2 = xelementAttrStr.Split(new char[]
						{
							'|'
						});
						if (array2 != null && 0 < array2.Length)
						{
							for (int m = 0; m < array2.Length; m++)
							{
								if (!string.IsNullOrEmpty(array2[m]))
								{
									string[] array3 = array2[m].Split(new char[]
									{
										','
									});
									if (array3.Length == 2)
									{
										int extPropIndexesIDByWord = ConfigExtPropIndexes.GetExtPropIndexesIDByWord(array3[0]);
										if (dictionary.ContainsKey(extPropIndexesIDByWord))
										{
											Dictionary<int, float> dictionary5;
											Dictionary<int, float> dictionary4 = dictionary5 = dictionary;
											int num4;
											int num6 = num4 = extPropIndexesIDByWord;
											float num5 = dictionary5[num4];
											dictionary4[num6] = num5 + float.Parse(array3[1]);
										}
										else
										{
											dictionary.Add(extPropIndexesIDByWord, float.Parse(array3[1]));
										}
									}
								}
							}
						}
					}
				}
			}
			if (0 < dictionary.Count)
			{
				foreach (KeyValuePair<int, float> keyValuePair in dictionary)
				{
					int extPropIndexesShowListByID = ConfigExtPropIndexes.GetExtPropIndexesShowListByID(keyValuePair.Key);
					ShiPinPageOne.PropData propData = default(ShiPinPageOne.PropData);
					propData.ShowList = extPropIndexesShowListByID;
					Dictionary<int, float>.Enumerator enumerator;
					KeyValuePair<int, float> keyValuePair2 = enumerator.Current;
					if (ConfigExtPropIndexes.GetPercentByID(keyValuePair2.Key))
					{
						string lang = Global.GetLang("{0}：{1}%");
						KeyValuePair<int, float> keyValuePair3 = enumerator.Current;
						object extPropIndexesDescriptionByID = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(keyValuePair3.Key, true);
						KeyValuePair<int, float> keyValuePair4 = enumerator.Current;
						propData.Str = string.Format(lang, extPropIndexesDescriptionByID, (keyValuePair4.Value * 100f).ToString("f1")) + Environment.NewLine;
					}
					else
					{
						string str = propData.Str;
						string lang2 = Global.GetLang("{0}：{1}");
						KeyValuePair<int, float> keyValuePair5 = enumerator.Current;
						object extPropIndexesDescriptionByID2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(keyValuePair5.Key, true);
						KeyValuePair<int, float> keyValuePair6 = enumerator.Current;
						propData.Str = str + string.Format(lang2, extPropIndexesDescriptionByID2, Mathf.CeilToInt(keyValuePair6.Value)) + Environment.NewLine;
					}
					list.Add(propData);
				}
			}
			if (0 < list.Count)
			{
				list.Sort((ShiPinPageOne.PropData x, ShiPinPageOne.PropData y) => x.ShowList - y.ShowList);
				for (int n = 0; n < list.Count; n++)
				{
					string[] array4 = array;
					int num7 = 1;
					array4[num7] += list[n].Str;
				}
			}
			return array;
		}
		return null;
	}

	private void ShowPropPart()
	{
		string[] propStr = this.GetPropStr();
		if (propStr != null && 0 < propStr[1].Length)
		{
			Global.ShowProPerty(1, propStr, null);
		}
		else
		{
			Super.HintMainText(Global.GetLang("当前无属性加成"), 10, 3);
		}
	}

	private void ClosePropPart()
	{
	}

	private void ShowGroupPart()
	{
		if (null != this.m_ShiPinYuLanPartWind)
		{
			this.m_ShiPinYuLanPartWind.Visibility = true;
		}
		else
		{
			this.m_ShiPinYuLanPartWind = U3DUtils.NEW<GChildWindow>();
			this.m_ShiPinYuLanPartWind.ModalType = ChildWindowModalType.Translucent;
			this.m_ShiPinYuLanPartWind.IsShowModal = true;
			Super.InitChildWindow(this.m_ShiPinYuLanPartWind, "ShiPinYuLanPart");
			this.Container.Children.Add(this.m_ShiPinYuLanPartWind);
		}
		UIEventListener.Get(this.m_ShiPinYuLanPartWind.ModalBak).onClick = delegate(GameObject g)
		{
			this.CloseGroupPart();
		};
		if (null != this.m_ShiPinYuLanPart)
		{
			Object.Destroy(this.m_ShiPinYuLanPart);
		}
		this.m_ShiPinYuLanPart = U3DUtils.NEW<ShiPinYuLanPart>();
		this.m_ShiPinYuLanPartWind.Body.Add(this.m_ShiPinYuLanPart);
		this.m_ShiPinYuLanPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			this.CloseGroupPart();
		};
	}

	private void CloseGroupPart()
	{
		Object.Destroy(this.m_ShiPinYuLanPart);
		this.m_ShiPinYuLanPart = null;
		Super.CloseChildWindow(this.Container, this.m_ShiPinYuLanPartWind);
	}

	private void PrfreshPropName(int GoodsID)
	{
		string goodsNameByID = Global.GetGoodsNameByID(GoodsID, false);
		this._page1TitleLabels[0].text = ((!string.IsNullOrEmpty(goodsNameByID)) ? goodsNameByID : Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("无")
		}));
	}

	private void RefreshProperty(int GoodsID, int Level)
	{
		this.RefreshUpCost(Level);
		this.PrfreshPropName(GoodsID);
		this.m_ObservableCollection.Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsID);
		if (goodsXmlNodeByID != null)
		{
			NGUITools.SetActive(this._EmptyLoadLabel, false);
			List<ShiPinPageOne.PropData> list = new List<ShiPinPageOne.PropData>();
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			double[] equipProps = goodsXmlNodeByID.EquipProps;
			if (equipProps != null && 0 < equipProps.Length)
			{
				for (int i = 1; i < equipProps.Length; i++)
				{
					if (0.0 < equipProps[i])
					{
						dictionary.Add(i, equipProps[i]);
					}
				}
			}
			double num = (double)Level * 0.2 + 0.8;
			double num2 = (double)(Level + 1) * 0.2 + 0.8;
			if (0 < dictionary.Count)
			{
				foreach (KeyValuePair<int, double> keyValuePair in dictionary)
				{
					int extPropIndexesShowListByID = ConfigExtPropIndexes.GetExtPropIndexesShowListByID(keyValuePair.Key);
					ShiPinPageOne.PropData propData = default(ShiPinPageOne.PropData);
					propData.ShowList = extPropIndexesShowListByID;
					object[] array = new object[2];
					array[0] = "e3b36c";
					int num3 = 1;
					Dictionary<int, double>.Enumerator enumerator;
					KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
					array[num3] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(keyValuePair2.Key, false) + Global.GetLang("：");
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
					object[] array2 = new object[2];
					array2[0] = "dac7ae";
					int num4 = 1;
					KeyValuePair<int, double> keyValuePair3 = enumerator.Current;
					object obj;
					if (ConfigExtPropIndexes.GetPercentByID(keyValuePair3.Key))
					{
						string text = "{0}%";
						KeyValuePair<int, double> keyValuePair4 = enumerator.Current;
						obj = string.Format(text, ShiPinPart.CutDoubleValue2(keyValuePair4.Value * num * 100.0));
					}
					else
					{
						KeyValuePair<int, double> keyValuePair5 = enumerator.Current;
						obj = ShiPinPart.ToInt((double)((float)((double)((float)keyValuePair5.Value) * num))).ToString();
					}
					array2[num4] = obj;
					propData.Str = colorStringForNGUIText + Global.GetColorStringForNGUIText(array2);
					propData.bMaxLevel = (Level == this.m_OrnamentSitexmlData.MaxLeve);
					ShiPinIconitem shiPinIconitem = this.m_IconLst.Find((ShiPinIconitem e) => e.GoodsId == GoodsID);
					if (null != shiPinIconitem)
					{
						int index = shiPinIconitem.Index;
					}
					if (!propData.bMaxLevel)
					{
						KeyValuePair<int, double> keyValuePair6 = enumerator.Current;
						double num5 = keyValuePair6.Value * num2;
						KeyValuePair<int, double> keyValuePair7 = enumerator.Current;
						double num6 = num5 - keyValuePair7.Value * num;
						object[] array3 = new object[2];
						array3[0] = "17e43e";
						int num7 = 1;
						KeyValuePair<int, double> keyValuePair8 = enumerator.Current;
						array3[num7] = ((!ConfigExtPropIndexes.GetPercentByID(keyValuePair8.Key)) ? ShiPinPart.ToInt((double)((float)num6)).ToString() : string.Format("{0}%", ShiPinPart.CutDoubleValue2(num6 * 100.0)));
						propData.str2 = Global.GetColorStringForNGUIText(array3);
					}
					list.Add(propData);
				}
			}
			if (0 < list.Count)
			{
				list.Sort((ShiPinPageOne.PropData x, ShiPinPageOne.PropData y) => x.ShowList.CompareTo(y.ShowList));
				for (int j = 0; j < list.Count; j++)
				{
					ShiPinPageOne.PropData propData2 = list[j];
					ShiPinPropertyItem shiPinPropertyItem = U3DUtils.NEW<ShiPinPropertyItem>();
					shiPinPropertyItem.Label1 = propData2.Str;
					shiPinPropertyItem.bShowUp = false;
					if (!propData2.bMaxLevel && !string.IsNullOrEmpty(propData2.str2))
					{
						shiPinPropertyItem.Label2 = propData2.str2;
						shiPinPropertyItem.bShowUp = true;
					}
					this.m_ObservableCollection.AddNoUpdate(shiPinPropertyItem);
					shiPinPropertyItem.DraggablePanel = this._DraggablePanel;
					shiPinPropertyItem.DelectPanel = true;
				}
				this._PropListBox.repositionNow = true;
			}
		}
		else
		{
			NGUITools.SetActive(this._EmptyLoadLabel, true);
		}
	}

	private int GetAllCaoWeiLevel()
	{
		int num = 0;
		if (this.m_OrnamentDataLst != null && 0 < this.m_OrnamentDataLst.Count)
		{
			byte b = 0;
			while ((int)b < this.m_IconLst.Count)
			{
				if (null != this.m_IconLst[(int)b] && !this.m_IconLst[(int)b].IsLock)
				{
					num += this.m_IconLst[(int)b].Level;
				}
				b += 1;
			}
		}
		if (0 >= num)
		{
			num = 1;
		}
		return num;
	}

	private void RefreshCaoWei()
	{
		if (this.m_IconLst != null && 0 < this.m_IconLst.Count)
		{
			int allCaoWeiLevel = this.GetAllCaoWeiLevel();
			byte b = 0;
			while ((int)b < this.m_IconLst.Count)
			{
				int clearLock = this.GetClearLock((int)b);
				if (allCaoWeiLevel >= clearLock)
				{
					this.m_IconLst[(int)b].IsLock = false;
					this.m_IconLst[(int)b].Level = this.m_IconLst[(int)b].Level;
				}
				b += 1;
			}
		}
	}

	private int GetClearLock(int index)
	{
		if (this.m_Rabbte.Count == 0)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("OrnamentSiteOpen", '|');
			if (systemParamStringArrayByName != null && 0 < systemParamStringArrayByName.Length)
			{
				for (int i = 0; i < systemParamStringArrayByName.Length; i++)
				{
					if (!string.IsNullOrEmpty(systemParamStringArrayByName[i]))
					{
						string[] array = systemParamStringArrayByName[i].Split(new char[]
						{
							','
						});
						if (array.Length == 2)
						{
							this.m_Rabbte.Add(int.Parse(array[0]), int.Parse(array[1]));
						}
					}
				}
			}
		}
		index++;
		if (this.m_Rabbte.ContainsKey(index))
		{
			return this.m_Rabbte[index];
		}
		return 0;
	}

	public void RefreshUpCost(int level)
	{
		if (0 <= level)
		{
			int num = this.m_OrnamentSitexmlData.GetCost(level);
			NGUITools.SetActive(this._UpFull, false);
			NGUITools.SetActive(this._UpBtn, true);
			NGUITools.SetActive(this._UpBtn.transform.parent, true);
			bool enabled = true;
			if (0 > num)
			{
				if (level >= this.m_OrnamentSitexmlData.MaxLeve)
				{
					NGUITools.SetActive(this._UpFull, true);
					NGUITools.SetActive(this._UpBtn, false);
					NGUITools.SetActive(this._UpBtn.transform.parent, false);
					enabled = false;
				}
				num = 0;
			}
			UIWidget[] componentsInChildren = this._UpBtn.transform.parent.GetComponentsInChildren<UIWidget>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = enabled;
			}
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.CharmPoint);
			this._CostLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				(num <= roleCommonUseParamsValue) ? "ffffff" : "ff0000",
				num.ToString()
			});
		}
	}

	public void RefreshShiPinIcons(List<OrnamentData> data)
	{
		this.m_OrnamentDataLst = data;
		if (this.m_Active)
		{
			base.StartCoroutine(this.InitIcon(data));
		}
		else
		{
			this.m_OrnamentDataLst = data;
		}
	}

	public void RefreshShiPinIcon(int index, int Level)
	{
		ShiPinIconitem shiPinIconitem = this.m_IconLst.Find((ShiPinIconitem e) => e.Index + 1 == index);
		if (null != shiPinIconitem)
		{
			shiPinIconitem.Level = Level;
			shiPinIconitem.ShowTeXiao(true);
			this.RefreshProperty((!shiPinIconitem.haveShiPin) ? 0 : shiPinIconitem.GoodsId, shiPinIconitem.Level);
		}
		OrnamentData ornamentData = this.m_OrnamentDataLst.Find((OrnamentData e) => e.ID == index);
		if (ornamentData != null)
		{
			ornamentData.Param1 = Level;
		}
		this.RefreshCaoWei();
		this.RefreshUpCost(Level);
	}

	public void RefreshShiPinIcon(GoodsData d)
	{
		ShiPinIconitem shiPinIconitem = this.m_IconLst.Find((ShiPinIconitem e) => e.Index + 1 == d.BagIndex);
		if (null != shiPinIconitem)
		{
			if (d.Using != 1)
			{
				shiPinIconitem.IsGoods = false;
				shiPinIconitem.bShowUnLoadBtn = false;
				shiPinIconitem.haveShiPin = false;
				shiPinIconitem.Icon = null;
				shiPinIconitem.ShowLock = true;
				shiPinIconitem.IsLock = false;
				shiPinIconitem.haveShiPin = false;
				shiPinIconitem.Tips = false;
				shiPinIconitem.Level = shiPinIconitem.Level;
				this.RefreshProperty(0, shiPinIconitem.Level);
			}
			else
			{
				shiPinIconitem.GoodsId = d.GoodsID;
				shiPinIconitem.IsGoods = true;
				shiPinIconitem.ShowLock = false;
				shiPinIconitem.Tips = false;
				shiPinIconitem.bShowUnLoadBtn = true;
				shiPinIconitem.haveShiPin = true;
				shiPinIconitem.Level = shiPinIconitem.Level;
				this.RefreshProperty(shiPinIconitem.GoodsId, shiPinIconitem.Level);
			}
		}
	}

	public ShowNetImage _Page1Bak;

	public UILabel[] _page1TitleLabels;

	public UILabel _EmptyLoadLabel;

	public Transform _IconRoot;

	public ListBox _PropListBox;

	public UIDraggablePanel _DraggablePanel;

	public UILabel _XiaoHaoLabel;

	public UILabel _CostLabel;

	public GameObject _UpFull;

	public GameObject _NoUpFull;

	public GButton _UpBtn;

	public GButton _PropBtn;

	public GButton _GroupBtn;

	private List<ShiPinIconitem> m_IconLst = new List<ShiPinIconitem>();

	private Vector3[] m_IconPos = new Vector3[]
	{
		new Vector3(-174f, 92f, -0.5f),
		new Vector3(-12f, -31f, -0.5f),
		new Vector3(-72f, -196f, -0.5f),
		new Vector3(-276f, -197f, -0.5f),
		new Vector3(-335f, -31f, -0.5f)
	};

	private ObservableCollection m_ObservableCollection;

	private List<OrnamentData> m_OrnamentDataLst;

	private bool m_Active = true;

	private ShiPinYuLanPart m_ShiPinYuLanPart;

	private GChildWindow m_ShiPinYuLanPartWind;

	private ShiPinPageOne.OrnamentSitexmlData m_OrnamentSitexmlData;

	private bool UpBtnCanClick = true;

	private Dictionary<int, int> m_Rabbte = new Dictionary<int, int>();

	public DPSelectedItemEventHandler Hander;

	private struct XmlData
	{
		public XElement xml;

		public int Number;

		public bool HaveActive;
	}

	private class OrnamentSitexmlData
	{
		public OrnamentSitexmlData()
		{
			XElement gameResXml = Global.GetGameResXml("Config/OrnamentSite.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "OrnamentSite");
				if (xelementList != null && 0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						if (xelementList[i] != null)
						{
							ShiPinPageOne.OrnamentSite ornamentSite = new ShiPinPageOne.OrnamentSite(xelementList[i]);
							if (this.dic.ContainsKey(ornamentSite.LevelID))
							{
								this.dic[ornamentSite.LevelID] = ornamentSite;
							}
							else
							{
								this.dic.Add(ornamentSite.LevelID, ornamentSite);
							}
						}
					}
				}
			}
		}

		public int GetCost(int levelId)
		{
			if (this.dic.ContainsKey(levelId))
			{
				return this.dic[levelId].Need;
			}
			return 0;
		}

		public int MaxLeve
		{
			get
			{
				if (this.m_MaxLevel == 0)
				{
					Dictionary<int, ShiPinPageOne.OrnamentSite>.Enumerator enumerator = this.dic.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int maxLevel = this.m_MaxLevel;
						KeyValuePair<int, ShiPinPageOne.OrnamentSite> keyValuePair = enumerator.Current;
						if (maxLevel < keyValuePair.Key)
						{
							KeyValuePair<int, ShiPinPageOne.OrnamentSite> keyValuePair2 = enumerator.Current;
							this.m_MaxLevel = keyValuePair2.Key;
						}
					}
				}
				return this.m_MaxLevel;
			}
		}

		private Dictionary<int, ShiPinPageOne.OrnamentSite> dic = new Dictionary<int, ShiPinPageOne.OrnamentSite>();

		private int m_MaxLevel;
	}

	private class OrnamentSite
	{
		public OrnamentSite(XElement ele)
		{
			this.LevelID = Global.GetXElementAttributeInt(ele, "LevelID");
			this.Need = Global.GetXElementAttributeInt(ele, "Need");
		}

		public int LevelID;

		public int Need;
	}

	private struct PropData
	{
		public bool bMaxLevel
		{
			get
			{
				return this.m_bMaxLevel;
			}
			set
			{
				this.m_bMaxLevel = value;
				if (!this.m_bMaxLevel)
				{
					this.str2 = string.Empty;
				}
			}
		}

		public int ShowList;

		private bool m_bMaxLevel;

		public string Str;

		public string str2;
	}

	public delegate void voidDelegate();
}
