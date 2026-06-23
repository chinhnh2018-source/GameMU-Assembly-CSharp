using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class SynthesizePart : UserControl
{
	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id >= 0 && id < 100)
			{
				GameObject at = this.TypeListBox.ItemsSource.GetAt(id);
				if (null != at)
				{
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(at.transform);
					SystemHelpPart.SetMask(at.transform, default(Vector4));
				}
				else
				{
					SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
				}
			}
			else if (id >= 100 && id < 200)
			{
				GameObject at2 = this.RightListBox.ItemsSource.GetAt(id - 100);
				if (null != at2)
				{
					SystemHelpPart.SetMask(at2.transform, default(Vector4));
				}
				else
				{
					SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
				}
			}
			else if (id == 200)
			{
				SystemHelpPart.SetMask(this.gGoodsIcon3, default(Vector4));
			}
			else if (id >= 300 && id < 400)
			{
				GameObject at3 = this.SelectListBox.ItemsSource.GetAt(id - 300);
				if (null != at3)
				{
					SystemHelpPart.SetMask(at3.transform, default(Vector4));
				}
				else
				{
					SystemHelpMgr.OnAction(UIObjIDs.Exception, HelpStateEvents.None, -1);
				}
			}
			else if (id == 1000)
			{
				SystemHelpPart.SetMask(this.HeChengBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
		SystemHelpMgr.OnAction(UIObjIDs.HeChengPart, HelpStateEvents.Inactived, -1);
	}

	private void InitTextInPrefabs()
	{
		this.HeChengBtn.Text = Global.GetLang("合成");
		this.Checkbox.transform.localPosition = new Vector3(-255f, -122f, 0f);
		this.Helppanel.transform.localPosition = new Vector3(0f, 0f, -5f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.TypeListBox.ItemsSource;
		this.ItemCollection2 = this.RightListBox.ItemsSource;
		this.ItemCollection3 = this.SelectListBox.ItemsSource;
		this.TypeListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.TypelistBox_SelectionChanged);
		this.RightListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.RightListBox_SelectionChanged);
		this.SelectListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectListBox_SelectionChanged);
		GameInstance.Game.GetJieriFanbeiInfo();
		this.HeChengBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CanHeChengBol)
			{
				if (this.sMergeType == "1000" && this.chiBangDBid == -1 && this.tempitem.IDText != "4")
				{
					Super.HintMainText(Global.GetLang("请先放入合成需要的翅膀!"), 10, 3);
				}
				else if (this.isTishi())
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提 示"), StringUtil.substitute(Global.GetLang("因为您使用了绑定材料进行操作，合成后的道具将会被绑定，确认继续？"), new object[0]), 1, null, MessBoxIsHintTypes.None);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							GameInstance.Game.SpriteGoodsMergeMsg(this.MergeItemID, 0, this.chiBangDBid, this.jingShiDBid, (!this.Checkbox.Check) ? 0 : 1);
							SystemHelpMgr.OnAction(UIObjIDs.HeChengSubmit, HelpStateEvents.Clicked, -1);
						}
						return true;
					};
				}
				else
				{
					GameInstance.Game.SpriteGoodsMergeMsg(this.MergeItemID, 0, this.chiBangDBid, this.jingShiDBid, (!this.Checkbox.Check) ? 0 : 1);
					SystemHelpMgr.OnAction(UIObjIDs.HeChengSubmit, HelpStateEvents.Clicked, -1);
				}
			}
			else
			{
				if (this.tishiType != -1)
				{
					if (this.tishiType == 1)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
					}
					else if (this.tishiType == 2)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedChuangzaojingshi, this.callback, string.Empty, string.Empty);
					}
					else if (this.tishiType == 3)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMayajingshi, this.callback, string.Empty, string.Empty);
					}
					return;
				}
				Super.HintMainText(Global.GetLang("合成条件不足，请检查！"), 10, 3);
			}
		};
		this.closChangedHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.helpObject.gameObject.SetActive(false);
		};
		this.CloseSelect.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.uipanel.gameObject.SetActive(false);
			this._ModalBak.gameObject.SetActive(false);
		};
		this.ReturnBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.ClearChiBangBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.gGoodsIcon2.Count() > 0)
			{
				this.chiBangDBid = -1;
				this.gGoodsIcon2.RemoveAt(0, true, false);
				this.gGoodsIcon2.Add(this.GetChiBangGGoodIcon());
				this.ClearChiBangBtn.gameObject.SetActive(false);
			}
		};
		this.ClearJingShiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.gGoodsIcon3.Count() > 0)
			{
				this.jingShiDBid = -1;
				this.gGoodsIcon3.RemoveAt(0, true, false);
				this.gGoodsIcon3.Add(this.GetJingShiGGoodIcon());
				this.ClearJingShiBtn.gameObject.SetActive(false);
			}
		};
		this.HelpBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.sMergeType == "1000")
			{
				this.HelpBak.URL = "NetImages/GameRes/Images/Plate/hechengStepHelp.png";
			}
			else
			{
				this.HelpBak.URL = "NetImages/GameRes/Images/Plate/hechengHelp.png";
			}
			this.Helppanel.gameObject.SetActive(true);
			this._ModalBak.gameObject.SetActive(true);
		};
		this.CloseHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Helppanel.gameObject.SetActive(false);
			this._ModalBak.gameObject.SetActive(false);
		};
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	public void InitFanbei()
	{
		if (Global.isFanbei(13))
		{
			FanbeiPrefab fanbeiPrefab = U3DUtils.NEW<FanbeiPrefab>();
			fanbeiPrefab.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/PercentDouble.png";
			this.obj.Add(fanbeiPrefab);
			this.obj.gameObject.SetActive(true);
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void InitPartData()
	{
		this.GoodsMergexml = Global.GetIsolateResXml("Config/GoodsMergeItems.Xml");
		this.InitData();
		if (this.TypeListBox.ItemsSource.Count > 0)
		{
			GButton gbutton = U3DUtils.AS<GButton>(this.TypeListBox.ItemsSource.GetAt(0));
			if (null != gbutton)
			{
				this.RefeshRightList(gbutton);
			}
		}
		SystemHelpMgr.OnAction(UIObjIDs.HeChengPart, HelpStateEvents.Actived, -1);
	}

	public void CleanUpChildWindows()
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		Super.CleanUpAllChildWindows(this.Container);
	}

	private void InitData()
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/GoodsMergeType.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "Types"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "PubStartTime");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "PubEndTime");
				if (Global.InLimitTimeRange(xelementAttributeStr, xelementAttributeStr2))
				{
					GButton gbutton = U3DUtils.NEW<GButton>("HeChengTabBtn");
					gbutton.Text = Global.GetXElementAttributeStr(xelement, "Title");
					gbutton.BtnTag = Global.GetXElementAttributeStr(xelement, "ID");
					gbutton.Label.color = NGUIMath.HexToColorEx(7697781U);
					gbutton.Label.transform.localScale = new Vector3(22f, 22f, 0f);
					gbutton.target.atlas = U3DUtils.LoadAtlas(Global.GetPrefabString("hechengWin", true));
					gbutton.normalSprite = "chatTab_normal";
					gbutton.hoverSprite = "chatTab_hover";
					gbutton.pressedSprite = "chatTab_hover";
					this.ItemCollection.Add(gbutton);
				}
			}
		}
	}

	private void RefeshRightList(GButton item)
	{
		if (item == null)
		{
			return;
		}
		if (this.GoodsMergexml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(this.GoodsMergexml, "Items"), "*");
		this.ItemCollection2.Clear();
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "PubStartTime");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "PubEndTime");
				if (Global.InLimitTimeRange(xelementAttributeStr, xelementAttributeStr2))
				{
					if (item.BtnTag == Global.GetXElementAttributeStr(xelement, "MergeType"))
					{
						GGoodIcon icon = this.GetIcon(Global.GetXElementAttributeInt(xelement, "NewGoodsCode"), "1", false);
						SynthesizeItem synthesizeItem = U3DUtils.NEW<SynthesizeItem>();
						synthesizeItem.gIcon.Add(icon);
						synthesizeItem.gIcon.GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 0f);
						synthesizeItem.NameText.Text = Global.GetXElementAttributeStr(xelement, "Title");
						synthesizeItem.IDText = Global.GetXElementAttributeStr(xelement, "ID");
						synthesizeItem.InfoText.Text = Global.GetXElementAttributeStr(xelement, "Description");
						this.ItemCollection2.AddNoUpdate(synthesizeItem);
					}
				}
			}
		}
		this.SetBtnStat(item);
		this.ItemCollection2.DelayUpdate();
		this.RightListBox.SelectedIndex = 0;
		this.RightListBox.Parent.GetComponent<UIDraggablePanel>().ResetPosition();
	}

	private void UnSelectItem()
	{
	}

	public void SelectType(int typeID)
	{
		if (this.TypeListBox.ItemsSource == null)
		{
			return;
		}
		if (this.TypeListBox.ItemsSource.Count > 0)
		{
			for (int i = 0; i < this.TypeListBox.ItemsSource.Count; i++)
			{
				GButton gbutton = U3DUtils.AS<GButton>(this.TypeListBox.ItemsSource.GetAt(i));
				if (null != gbutton && gbutton.BtnTag == typeID.ToString())
				{
					this.RefeshRightList(gbutton);
					break;
				}
			}
		}
	}

	private void TypelistBox_SelectionChanged(object sender, EventArgs e)
	{
		GButton gbutton = U3DUtils.AS<GButton>(this.TypeListBox.SelectedItem);
		if (null == gbutton)
		{
			return;
		}
		this.RefeshRightList(gbutton);
		if (gbutton.BtnTag == "101")
		{
			SystemHelpMgr.OnAction(UIObjIDs.HeChengTabBtn, HelpStateEvents.Clicked, 0);
		}
		else if (gbutton.BtnTag == "201")
		{
			SystemHelpMgr.OnAction(UIObjIDs.HeChengTabBtn, HelpStateEvents.Clicked, 1);
		}
		else if (gbutton.BtnTag == "1000")
		{
			SystemHelpMgr.OnAction(UIObjIDs.HeChengTabBtn, HelpStateEvents.Clicked, 2);
		}
		else if (gbutton.BtnTag == "401")
		{
			SystemHelpMgr.OnAction(UIObjIDs.HeChengTabBtn, HelpStateEvents.Clicked, 3);
		}
	}

	private void RightListBox_SelectionChanged(object sender, EventArgs e)
	{
		SynthesizeItem synthesizeItem = U3DUtils.AS<SynthesizeItem>(this.RightListBox.SelectedItem);
		if (null == synthesizeItem)
		{
			return;
		}
		if (this.tempitem != null && this.tempitem != synthesizeItem)
		{
			this.tempitem.Bak.spriteName = "lianluEquipItem_bak";
		}
		if (this.tempitem == synthesizeItem)
		{
			return;
		}
		this.tempitem = synthesizeItem;
		this.tempitem.Bak.spriteName = "lianluEquipItem_bak2";
		this.ShowHeChengInfo(synthesizeItem.IDText);
		SystemHelpMgr.OnAction(UIObjIDs.HeChengGoodsTarget, HelpStateEvents.Clicked, this.RightListBox.SelectedIndex);
	}

	private void SelectListBox_SelectionChanged(object sender, EventArgs e)
	{
		SynthesizeItem synthesizeItem = U3DUtils.AS<SynthesizeItem>(this.SelectListBox.SelectedItem);
		if (null == synthesizeItem)
		{
			return;
		}
		if (synthesizeItem.sTag == "CHIBANG")
		{
			if (this.gGoodsIcon2.Count() > 0)
			{
				this.gGoodsIcon2.RemoveAt(0, true, false);
			}
			this.gGoodsIcon2.Add(this.GetIcon(Convert.ToInt32(synthesizeItem.goodsData.GoodsID), "1", true));
			this.chiBangDBid = synthesizeItem.goodsData.Id;
			this.ClearChiBangBtn.gameObject.SetActive(true);
			this.uipanel.gameObject.SetActive(false);
			this._ModalBak.gameObject.SetActive(false);
		}
		else if (synthesizeItem.sTag == "JINGSHI")
		{
			if (this.gGoodsIcon3.Count() > 0)
			{
				this.gGoodsIcon3.RemoveAt(0, true, true);
			}
			this.gGoodsIcon3.Add(this.GetIcon(Convert.ToInt32(synthesizeItem.goodsData.GoodsID), "1", true));
			this.jingShiDBid = synthesizeItem.goodsData.Id;
			this.ClearJingShiBtn.gameObject.SetActive(true);
			this.uipanel.gameObject.SetActive(false);
			this._ModalBak.gameObject.SetActive(false);
			SystemHelpMgr.OnAction(UIObjIDs.HeChengJingShiItem, HelpStateEvents.Clicked, this.SelectListBox.SelectedIndex);
		}
	}

	private void ShowHeChengInfo(string index)
	{
		XElement isolateResXml = Global.GetIsolateResXml("Config/GoodsMergeItems.Xml");
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Item", "ID", index);
		if (xelement == null)
		{
			return;
		}
		this.CanHeChengBol = true;
		this.tishiType = -1;
		this.needGoods.Clear();
		this.MergeItemID = Convert.ToInt32(index);
		string text = string.Empty;
		string text2 = string.Empty;
		this.CaiLiaoText.Text = Global.GetXElementAttributeStr(xelement, "MustExplan");
		this.KeXuanCaiLiaoText.Text = Global.GetXElementAttributeStr(xelement, "OptionalExplan");
		this.sMergeType = Global.GetXElementAttributeStr(xelement, "MergeType");
		this.iMoney = Global.GetXElementAttributeInt(xelement, "Money");
		this.iYuanBao = Global.GetXElementAttributeInt(xelement, "DianJuan");
		this.iJingYuan = Global.GetXElementAttributeInt(xelement, "JingYuan");
		this.iUccessRate = Global.GetXElementAttributeInt(xelement, "SuccessRate");
		this.NeedJingBiText.Text = StringUtil.substitute(Global.GetLang("消耗{0} {1} {2}"), new object[]
		{
			(this.iMoney <= 0) ? string.Empty : Global.GetLang("金币："),
			(this.iJingYuan <= 0) ? string.Empty : Global.GetLang("积分："),
			(this.iYuanBao <= 0) ? string.Empty : Global.GetLang("钻石：")
		});
		this.NeedJingBiV.Text = ((this.iMoney <= 0) ? ((this.iJingYuan <= 0) ? ((this.iYuanBao <= 0) ? string.Empty : this.iYuanBao.ToString()) : this.iJingYuan.ToString()) : this.iMoney.ToString());
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.iMoney || Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < this.iJingYuan || Global.Data.roleData.UserMoney < this.iYuanBao)
		{
			this.NeedJingBiV.textColor = 4294901760U;
			this.CanHeChengBol = false;
			this.tishiType = 1;
		}
		else
		{
			this.NeedJingBiV.textColor = uint.MaxValue;
		}
		this.NeedChengGongLvText.Text = StringUtil.substitute("{0}%", new object[]
		{
			this.FanBei(this.iUccessRate)
		});
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "OrigGoodsIDs");
		if (!string.IsNullOrEmpty(xelementAttributeStr))
		{
			string[] array = Global.StringTrim(xelementAttributeStr).Split(new char[]
			{
				'|'
			});
			if (array.Length <= 0)
			{
				return;
			}
			this.ClearIcon();
			if (array.Length == 1 && this.sMergeType != "1000")
			{
				text = array[0];
				text2 = text.Substring(0, text.IndexOf(","));
				string sCout = text.Substring(text.IndexOf(",") + 1, text.Length - text.IndexOf(",") - 1);
				this.gGoodsIcon1.Add(this.GetIcon(Convert.ToInt32(text2), sCout, true));
				this.gGoodsIcon2.Add(this.GetDisableGGoodIcon());
				this.gGoodsIcon3.Add(this.GetDisableGGoodIcon());
			}
			else if (array.Length == 2 && this.sMergeType != "1000")
			{
				this.gGoodsIcon1.Add(this.GetIcon(Convert.ToInt32(array[0].Substring(0, array[0].IndexOf(","))), array[0].Substring(array[0].IndexOf(",") + 1, array[0].Length - array[0].IndexOf(",") - 1), true));
				this.gGoodsIcon2.Add(this.GetIcon(Convert.ToInt32(array[1].Substring(0, array[1].IndexOf(","))), array[1].Substring(array[1].IndexOf(",") + 1, array[1].Length - array[1].IndexOf(",") - 1), true));
				this.gGoodsIcon3.Add(this.GetDisableGGoodIcon());
			}
			else if (array.Length == 3 && this.sMergeType != "1000")
			{
				this.gGoodsIcon1.Add(this.GetIcon(Convert.ToInt32(array[0].Substring(0, array[0].IndexOf(","))), array[0].Substring(array[0].IndexOf(",") + 1, array[0].Length - array[0].IndexOf(",") - 1), true));
				this.gGoodsIcon2.Add(this.GetIcon(Convert.ToInt32(array[1].Substring(0, array[1].IndexOf(","))), array[1].Substring(array[1].IndexOf(",") + 1, array[1].Length - array[1].IndexOf(",") - 1), true));
				this.gGoodsIcon3.Add(this.GetIcon(Convert.ToInt32(array[2].Substring(0, array[2].IndexOf(","))), array[2].Substring(array[2].IndexOf(",") + 1, array[2].Length - array[2].IndexOf(",") - 1), true));
			}
			this.gGoodsIcon4.Add(this.GetIcon(Global.GetXElementAttributeInt(xelement, "NewGoodsCode"), "1", false));
		}
	}

	private int FanBei(int NowNum)
	{
		int num = 0;
		if (Global.isFanbei(13))
		{
			num = 2;
		}
		if (Global.isFanbei(212))
		{
			double num2 = 0.0;
			if (double.TryParse(Global.JieriFanbeiInfo[212].ExtArg1, ref num2))
			{
				num += (int)num2;
			}
		}
		if (num == 0)
		{
			num = 1;
		}
		int num3 = NowNum * num;
		if (100 < num3)
		{
			num3 = 100;
		}
		return num3;
	}

	private void ClearIcon()
	{
		if (this.gGoodsIcon1.Count() > 0)
		{
			this.gGoodsIcon1.RemoveAt(0, true, false);
		}
		if (this.gGoodsIcon2.Count() > 0)
		{
			this.gGoodsIcon2.RemoveAt(0, true, false);
		}
		if (this.gGoodsIcon3.Count() > 0)
		{
			this.gGoodsIcon3.RemoveAt(0, true, false);
		}
		if (this.gGoodsIcon4.Count() > 0)
		{
			this.gGoodsIcon4.RemoveAt(0, true, false);
		}
		if (this.gGoodsIcon5.Count() > 0)
		{
			this.gGoodsIcon5.RemoveAt(0, true, false);
		}
		this.chiBangDBid = -1;
		this.jingShiDBid = -1;
		this.ClearJingShiBtn.gameObject.SetActive(false);
		this.ClearChiBangBtn.gameObject.SetActive(false);
	}

	private void ShowSelectGoods(string type)
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ZhiYeHeChengJingShi", '|');
		string[] systemParamStringArrayByName2 = ConfigSystemParam.GetSystemParamStringArrayByName("HeChengChiBang", '|');
		string[] array = null;
		string[] array2 = null;
		if (this.MergeItemID == 4)
		{
			array2 = systemParamStringArrayByName[0].Split(new char[]
			{
				','
			});
		}
		else if (this.MergeItemID == 5)
		{
			array2 = systemParamStringArrayByName[1].Split(new char[]
			{
				','
			});
			array = systemParamStringArrayByName2[0].Split(new char[]
			{
				','
			});
		}
		else if (this.MergeItemID == 6)
		{
			array2 = systemParamStringArrayByName[2].Split(new char[]
			{
				','
			});
			array = systemParamStringArrayByName2[1].Split(new char[]
			{
				','
			});
		}
		if (Global.Data.roleData.GoodsDataList == null)
		{
			return;
		}
		this.ItemCollection3.Clear();
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
			if (goodsData.Site == 0 && goodsData.Using == 0)
			{
				if (type == "CHIBANG")
				{
					if (array.IndexOf(goodsData.GoodsID.ToString()) == -1)
					{
						goto IL_251;
					}
				}
				else if (type == "JINGSHI" && array2.IndexOf(goodsData.GoodsID.ToString()) == -1)
				{
					goto IL_251;
				}
				SynthesizeItem synthesizeItme = U3DUtils.NEW<SynthesizeItem>();
				synthesizeItme.NameText.Text = Global.GetGoodsNameByID(goodsData.GoodsID, false);
				synthesizeItme.NameText.transform.localPosition = new Vector3(33f, 0f, 0f);
				synthesizeItme.IDText = goodsData.GoodsID.ToString();
				synthesizeItme.goodsData = goodsData;
				synthesizeItme.sTag = type;
				GGoodIcon gicon = this.GetIcon_GoodsData(goodsData);
				synthesizeItme.gIcon.Add(gicon);
				synthesizeItme.gIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					if (SystemHelpPart.IsMaskShowing())
					{
						return;
					}
					GTipServiceEx.SelfBagOnly = false;
					GTipServiceEx.ShowTip(gicon, TipTypes.GoodsText, GoodsOwnerTypes.OtherRole, synthesizeItme.goodsData);
				};
				this.ItemCollection3.AddNoUpdate(synthesizeItme);
			}
			IL_251:;
		}
		if (this.ItemCollection3.Count <= 0)
		{
			if (type == "CHIBANG")
			{
				this.helpSprite.transform.localScale = new Vector3(286f, 112f);
				this.ChangedHelpPanel_Bak.transform.localScale = new Vector3(266f, 90f);
				this.ChangedHelpPanel_Bak.URL = "NetImages/GameRes/Images/Plate/hechengFangruchibangHelp.png";
			}
			else
			{
				this.ChangedHelpPanel_Bak.transform.localScale = new Vector3(333f, 160f);
				this.helpSprite.transform.localScale = new Vector3(352f, 180f);
				this.ChangedHelpPanel_Bak.URL = "NetImages/GameRes/Images/Plate/hechengFangruhuijiHelp.png";
			}
			this.helpObject.gameObject.SetActive(true);
		}
		else
		{
			this.helpObject.gameObject.SetActive(false);
			this.uipanel.gameObject.SetActive(true);
			this._ModalBak.gameObject.SetActive(true);
		}
		SystemHelpMgr.OnAction(UIObjIDs.HeChengCaiLiao, HelpStateEvents.Clicked, 0);
	}

	private GGoodIcon GetIcon(int goodsID, string sCout = "1", bool isToGrayBitmap = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, "NetImages/GameRes/");
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
		GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.TipType = 1;
		icon.ItemCode = goodsID;
		icon.BoxTypes = 5;
		icon.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			totalGoodsCountByID,
			sCout
		});
		icon.TextHorizontalAlignment = global::Layout.Right;
		icon.TextVerticalAlignment = global::Layout.Bottom;
		icon.Tag = sCout;
		icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, GoodsPriceUnitTypes.Jinbi, Convert.ToInt32(icon.Tag), icon.ItemCode, -1, -1, null);
		};
		if (totalGoodsCountByID < Convert.ToInt32(sCout))
		{
			if (isToGrayBitmap)
			{
				icon.GoodImg.ToGrayBitmap = true;
				this.CanHeChengBol = false;
				if (goodsID == 2004)
				{
					this.tishiType = 2;
				}
				else if (goodsID == 2000)
				{
					this.tishiType = 3;
				}
			}
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			icon.TextColor = 16711680U;
		}
		else
		{
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		if (!isToGrayBitmap)
		{
			icon.Text = string.Empty;
		}
		else
		{
			this.needGoods.Add(goodsID, Convert.ToInt32(sCout));
		}
		Super.InitGoodsGIcon(icon, Global.GetDummyGoodsData(goodsID), true, IconTextTypes.Qianghua);
		return icon;
	}

	private GGoodIcon GetIcon_GoodsData(GoodsData gd)
	{
		if (gd == null)
		{
			return null;
		}
		int goodsID = gd.GoodsID;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, "NetImages/GameRes/");
		if (goodsImageURLFromIconCode == null)
		{
			return null;
		}
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCode = goodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.BoxTypes = 5;
		ggoodIcon.Text = ((gd.Forge_level <= 0) ? string.Empty : StringUtil.substitute("+{0}", new object[]
		{
			gd.Forge_level.ToString()
		}));
		ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
		ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
		if (Global.GetTotalGoodsCountByID(goodsID) < 1)
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			ggoodIcon.TextColor = 16711680U;
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		return ggoodIcon;
	}

	public void RefreshData(int retCode)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.ShowHeChengInfo(this.tempitem.IDText);
		if (retCode >= 0)
		{
			this._LianluAnim[0].gameObject.SetActive(true);
			this.PlayStart(this._LianluAnim[0], new ActiveAnimation.OnFinished(this.PlayFinished));
			this._LianluAnim[1].gameObject.SetActive(false);
			this._LianluAnim[1].gameObject.SetActive(true);
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		}
		else
		{
			this._LianluAnim[2].gameObject.SetActive(true);
			this.PlayStart(this._LianluAnim[2], new ActiveAnimation.OnFinished(this.PlayFinished));
			this._LianluAnim[3].gameObject.SetActive(false);
			this._LianluAnim[3].gameObject.SetActive(true);
			if (retCode == -1)
			{
				Super.HintMainText(Global.GetLang("合成失败，您的背包否已满,请留出足空间再合成！"), 10, 3);
			}
			else if (retCode == -2)
			{
				Super.HintMainText(Global.GetLang("合成失败，合成所需材料不足！"), 10, 3);
			}
			else if (retCode == -3)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			}
			else if (retCode == -4)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			}
			else if (retCode == -5)
			{
				Super.HintMainText(Global.GetLang("合成失败，合成所需真气不足！"), 10, 3);
			}
			else if (retCode == -6)
			{
				Super.HintMainText(Global.GetLang("合成失败，合成所需神器之魂不足！"), 10, 3);
			}
			else if (retCode == -7)
			{
				Super.HintMainText(Global.GetLang("合成失败，合成所需积分不足！"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("合成失败，请继续努力！"), 10, 3);
				Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			}
		}
	}

	private string GetMergeItemTitle()
	{
		XElement xelement = Global.GetXElement(this.GoodsMergexml, "Item", "ID", this.MergeItemID.ToString());
		if (xelement == null)
		{
			return string.Empty;
		}
		return Global.GetXElementAttributeStr(xelement, "Title");
	}

	private GGoodIcon GetDisableGGoodIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = "bagGridLock_bak";
		return ggoodIcon;
	}

	private GGoodIcon GetChiBangGGoodIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL("NetImages/GameRes/Images/Plate/hechengFangruChibang.png", false, 0);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.ChiBangMouseLeftButtonUp));
		return ggoodIcon;
	}

	private GGoodIcon GetJingShiGGoodIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL("NetImages/GameRes/Images/Plate/hechengFangruJingshi.png", false, 0);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.JingShiMouseLeftButtonUp));
		return ggoodIcon;
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16777215U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(8350293U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16777215U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	private void ChiBangMouseLeftButtonUp(MouseEvent evt)
	{
		this.ShowSelectGoods("CHIBANG");
	}

	private void JingShiMouseLeftButtonUp(MouseEvent evt)
	{
		this.ShowSelectGoods("JINGSHI");
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	private bool isTishi()
	{
		if (this.needGoods.Count <= 0)
		{
			return false;
		}
		foreach (KeyValuePair<int, int> keyValuePair in this.needGoods)
		{
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(keyValuePair.Key);
			int totalBindingGoodsCountByID = Global.GetTotalBindingGoodsCountByID(keyValuePair.Key);
			int value = keyValuePair.Value;
			if (value > totalGoodsCountByID - totalBindingGoodsCountByID && totalGoodsCountByID != totalBindingGoodsCountByID)
			{
				return true;
			}
		}
		return false;
	}

	public ListBox TypeListBox;

	public ListBox RightListBox;

	public ListBox SelectListBox;

	public UISpriteAnimation animation;

	private XElement GoodsMergexml;

	public DPSelectedItemEventHandler callback;

	public SpriteSL obj;

	public UISprite _ModalBak;

	public UIPanel uipanel;

	public UIPanel Helppanel;

	public ObservableCollection ItemCollection;

	public ObservableCollection ItemCollection2;

	public ObservableCollection ItemCollection3;

	public ShowNetImage HelpBak;

	public GCheckBox Checkbox;

	public TextBlock TitleText;

	public TextBlock NeedJingBiText;

	public TextBlock NeedJingBiV;

	public TextBlock NeedChengGongLvText;

	public TextBlock CaiLiaoText;

	public TextBlock KeXuanCaiLiaoText;

	public GButton HelpBtn;

	public GButton CloseHelp;

	public GButton ReturnBtn;

	public GButton HeChengBtn;

	public GButton CloseSelect;

	public GButton ClearChiBangBtn;

	public GButton ClearJingShiBtn;

	public GGoodIcon gGoodsIcon1;

	public GGoodIcon gGoodsIcon2;

	public GGoodIcon gGoodsIcon3;

	public GGoodIcon gGoodsIcon4;

	public GGoodIcon gGoodsIcon5;

	public TextBlock Tishi1;

	public TextBlock TishiInfo;

	public GameObject EmObject;

	public ShowNetImage ChangedHelpPanel_Bak;

	public UISprite helpSprite;

	public GameObject helpObject;

	public GButton closChangedHelp;

	public Animation[] _LianluAnim;

	private LoadingWindow LoadingWin;

	private int MergeItemID = -1;

	private int iMoney;

	private int iYuanBao;

	private int iJingYuan;

	private int iUccessRate;

	private int chiBangDBid = -1;

	private int jingShiDBid = -1;

	private bool CanHeChengBol = true;

	private int tishiType = -1;

	private string sMergeType = string.Empty;

	public DPSelectedItemBoolEventHandler DPSelectedItem;

	private Dictionary<int, int> needGoods = new Dictionary<int, int>();

	private SynthesizeItem tempitem;

	private GButton tempBtn;
}
