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

public class LianluZhuangbeiXilianPart : UserControl
{
	public bool isContinueTiLian
	{
		get
		{
			return this._isContinueTiLian;
		}
		set
		{
			if (value)
			{
			}
			this._isContinueTiLian = value;
		}
	}

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

	public SpriteSL[] equipIcon
	{
		get
		{
			return this._equipIcon;
		}
		set
		{
			this._equipIcon = value;
		}
	}

	public GCheckBox[] radioIcon
	{
		get
		{
			return this._radioIcon;
		}
		set
		{
			this._radioIcon = value;
		}
	}

	public GCheckBox[] radioIconMoney
	{
		get
		{
			return this._radioIconMoney;
		}
		set
		{
			this._radioIconMoney = value;
		}
	}

	protected override void OnDestroy()
	{
		if (this.messageBoxWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
		}
		base.OnDestroy();
	}

	public void InitPartSize(int width, int height)
	{
		this.ItemCollection = this.ProsList.ItemsSource;
		this.ProsList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ItemMouseLeftButtonUp);
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.RadioIndex == 0)
			{
				if (this.CheckBoxAutoSaveBind.Check)
				{
					if (this.SubmitBtn.Text == Global.GetLang("取消"))
					{
						this.isClick = false;
						this.StopAutoTiSheng_Tick();
					}
					else
					{
						if (this.RadioMoneyIndex == 1 && this.NeedZhuanshiNum > 0 && Global.GetZuanShi(ZuanShiPartClass.ZuangBeiPeiYang))
						{
							if (this.messageBoxWindow != null)
							{
								Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
							}
							string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ZhuangBeiPeiYang", this.NeedZhuanshiNum);
							this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								string.Format(Global.GetLang("选择后每次需要消耗{0}{1}，确定执行吗？"), this.NeedZhuanshiNum, text)
							}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
							if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
							{
								this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
							}
							this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
							{
								int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
								if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
								{
									Global.SetZuanShi(ZuanShiPartClass.ZuangBeiPeiYang, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
								}
								Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
								if (messageBoxReturn == 0)
								{
									this.SubmitBtn.Text = Global.GetLang("取消");
									base.InvokeRepeating("AutoTiSheng_Tick", 0.5f, 1f);
								}
								return true;
							};
							return;
						}
						this.SubmitBtn.Text = Global.GetLang("取消");
						base.InvokeRepeating("AutoTiSheng_Tick", 0.5f, 1f);
					}
				}
				else
				{
					if (this.RadioMoneyIndex == 1 && this.NeedZhuanshiNum > 0 && Global.GetZuanShi(ZuanShiPartClass.ZuangBeiPeiYang))
					{
						if (this.messageBoxWindow != null)
						{
							Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						}
						string text2 = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ZhuangBeiPeiYang", this.NeedZhuanshiNum);
						this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("选择后每次需要消耗{0}{1}，确定执行吗？"), this.NeedZhuanshiNum, text2)
						}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
						}
						this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
							if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
							{
								Global.SetZuanShi(ZuanShiPartClass.ZuangBeiPeiYang, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
							}
							Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								this.StartSubmit(-1);
							}
							return true;
						};
						return;
					}
					this.StartSubmit(-1);
				}
			}
			else if (this.RadioIndex == 1)
			{
				if (!this.isContinueTiLian)
				{
					return;
				}
				if (this.equipIcon[0].Length() > 0)
				{
					GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
					if (goodsData.WashProps == null)
					{
						this.SelectRadio(0);
						this.StartSubmit(-1);
						return;
					}
				}
				if (null == this.SelectedItem)
				{
					return;
				}
				NGUITools.SetActive(this.PropsPanel[0].gameObject, false);
				NGUITools.SetActive(this.PropsPanel[1].gameObject, true);
				this.StartReplace(this.SelectedItem.ID);
			}
		};
		this.ReplaceBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ReplaceBtn.isEnabled)
			{
				this.StartReplace(-3);
			}
		};
		this.RadioXianlianValue.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectRadio(0);
			NGUITools.SetActive(this.PropsPanel[0].gameObject, true);
			NGUITools.SetActive(this.PropsPanel[1].gameObject, false);
			this.SetBtnState(-2, true);
		};
		this.RadioXianlianProps.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectRadio(1);
			if (this.equipIcon[0].Length() > 0)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
				if (goodsData != null && goodsData.WashProps != null)
				{
					this.AddXilianPropsListNew(goodsData, null, 0);
				}
				this.SelectItem(0);
				this.SetBtnState(0, true);
			}
		};
		this.RadioTongqian.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.isContinueTiLian = false;
			this.SelectRadioMoney(0);
			if (this.LastOperation == -5 && !this.ReplaceBtn.gameObject.activeSelf)
			{
				NGUITools.SetActive(this.ReplaceBtn.gameObject, true);
			}
			this.CheckBoxAutoSaveBind.Check = false;
			if (!this.CheckBoxAutoSaveBind.Check)
			{
				this.StopAutoTiSheng_Tick();
			}
		};
		this.CheckBoxAutoSaveBind.Check = false;
		this.RadioYuanbao.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.isContinueTiLian = true;
			this.SelectRadioMoney(1);
			this.isClick = false;
			this.CheckBoxAutoSaveBind.Check = false;
			NGUITools.SetActive(this.CheckBoxAutoSaveBind.gameObject, true);
		};
		this.CheckBoxAutoSaveBind.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (!this.CheckBoxAutoSaveBind.Check)
			{
				this.StopAutoTiSheng_Tick();
			}
			if (this.ReplaceBtn.gameObject.activeSelf)
			{
				NGUITools.SetActive(this.ReplaceBtn.gameObject, false);
			}
			this.CheckBoxAutoSaveBind.Check = !this.isClick;
			this.isClick = !this.isClick;
		};
		this.CheckBoxBind.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.equipIcon[0].Length() > 0)
			{
				GoodsData gd = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
				this.AddCailiao(gd);
			}
		};
		if (this.CheckBoxAutoSaveBind.Check && this.RadioMoneyIndex == 1)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "ZhuangBeiPeiYang", this.NeedZhuanshiNum, string.Empty);
		}
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[2];
		this.equipIcon[0] = this.Equip;
		this.equipIcon[1] = this.Cailiao;
		this.radioIcon = new GCheckBox[2];
		this.radioIcon[0] = this.RadioXianlianValue;
		this.radioIcon[1] = this.RadioXianlianProps;
		this.radioIconMoney = new GCheckBox[2];
		this.radioIconMoney[0] = this.RadioTongqian;
		this.radioIconMoney[1] = this.RadioYuanbao;
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.ZhandouliText.Text = string.Empty;
		this.QianghuaHintText.Text = Global.GetLang("装备培养可为装备附加多种属性");
		this.CheckBoxAutoSaveBind._Lable.text = Global.GetLang("自动培养");
		this.CheckBoxAutoSaveBind._Lable.lineWidth = 90;
		NGUITools.SetActive(this.CheckBoxBind, false);
		NGUITools.SetActive(this.objCailiaoText, false);
		NGUITools.SetActive(this.QianghuaHintText.gameObject, true);
		NGUITools.SetActive(this.PropsPanel[0].gameObject, true);
		NGUITools.SetActive(this.PropsPanel[1].gameObject, false);
		NGUITools.SetActive(this.ReplaceBtn.gameObject, false);
		this.SetChangZhandouli(false, null, null, 0);
		this.SubmitBtn.Text = Global.GetLang("培养");
		this.CheckBoxBind.Text = Global.GetLang("优先使用绑定材料");
		this.ReplaceBtn.Text = Global.GetLang("保存");
		this.RadioTongqian.transform.localPosition = new Vector3(-190f, -28f, 0f);
		this.RadioYuanbao.transform.localPosition = new Vector3(-190f, -66f, 0f);
	}

	public void SetBtnState(int washIndex, bool isBefore)
	{
		if (washIndex == -1)
		{
			NGUITools.SetActive(this.ReplaceBtn.gameObject, false);
			if (this.CheckBoxAutoSaveBind.Check)
			{
				this.SubmitBtn.Text = Global.GetLang("取消");
			}
			else
			{
				this.SubmitBtn.Text = Global.GetLang("培养");
			}
			LianluZhuangbeiXilianPart.IsSave = true;
		}
		else if (washIndex > 0 || washIndex == 0 || washIndex == -2)
		{
			if (isBefore)
			{
				NGUITools.SetActive(this.ReplaceBtn.gameObject, false);
				LianluZhuangbeiXilianPart.IsSave = true;
			}
			else
			{
				NGUITools.SetActive(this.ReplaceBtn.gameObject, true);
				LianluZhuangbeiXilianPart.IsSave = false;
			}
			this.SubmitBtn.Text = Global.GetLang("培养");
		}
		else if (washIndex == -3)
		{
			if (isBefore)
			{
				NGUITools.SetActive(this.ReplaceBtn.gameObject, true);
				LianluZhuangbeiXilianPart.IsSave = false;
			}
			else
			{
				NGUITools.SetActive(this.ReplaceBtn.gameObject, false);
				LianluZhuangbeiXilianPart.IsSave = true;
			}
			if (this.CheckBoxAutoSaveBind.Check)
			{
				this.SubmitBtn.Text = Global.GetLang("取消");
			}
			else
			{
				this.SubmitBtn.Text = Global.GetLang("培养");
			}
		}
	}

	public void SetChangZhandouli(bool isShow, GoodsData gd, List<int> newPropsList, int washIndex)
	{
		this.ChangeZhandouli.Visibility = isShow;
		if (!isShow)
		{
			return;
		}
		if (washIndex == -3 || washIndex == -1)
		{
			this.ChangeZhandouli.Visibility = false;
			return;
		}
		string[] array = new string[]
		{
			"up",
			"down"
		};
		int num = (int)Global.GetGoodsDataZhanLi(gd);
		GoodsData goodsData = Global.CloneGoodsData(gd, false);
		if (this.RadioIndex == 1 && washIndex > 0)
		{
			washIndex = washIndex * 2 + 1;
			goodsData.WashProps[washIndex] = newPropsList[1];
		}
		else if (this.RadioIndex == 0 && washIndex != -3 && washIndex != -1)
		{
			goodsData.WashProps = newPropsList;
		}
		int num2 = (int)Global.GetGoodsDataZhanLi(goodsData);
		int num3 = num2 - num;
		if (num3 == 0)
		{
			this.ChangeFlag.gameObject.SetActive(false);
			this.ChangeValueText.Text = Global.GetLang("不变");
		}
		else
		{
			this.ChangeFlag.gameObject.SetActive(true);
			if (num3 > 0)
			{
				this.ChangeFlag.spriteName = array[0];
			}
			else if (num3 < 0)
			{
				this.ChangeFlag.spriteName = array[1];
			}
			this.ChangeValueText.Text = Mathf.Abs(num3).ToString();
		}
	}

	public void AddEquip(GoodsData gd, int result = 1)
	{
		if (gd != null && this.__flag_pre_equipid != gd.Id)
		{
			this.__flag_pre_equipid = gd.Id;
			this.LastOperation = 1;
		}
		if (gd != null)
		{
			this.InitAllValue();
			NGUITools.SetActive(this.QianghuaHintText.gameObject, false);
			NGUITools.SetActive(this.CheckBoxBind, true);
			if (Global.IsContainDBIDByXilian(gd.Id) && result > 0)
			{
				this.ZhandouliText.Text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(gd));
				this.StartSubmitExe(gd.Id, -5);
				return;
			}
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			this.AddCailiao(gd);
			this.ZhandouliText.Text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(gd));
			this.AddXilianPropsListNew(gd, null, 0);
			this.SelectItem(0);
			if (gd.WashProps == null)
			{
				this.SetBtnState(-1, true);
			}
			else
			{
				this.SetBtnState(-2, true);
			}
		}
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.equipIcon[index].Clear();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.TipType = 1;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsXmlNodeByID.ID,
				1,
				gd.Id,
				goodsOwnerType
			});
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = 12;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			if (index == 0)
			{
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(icon);
		}
	}

	private void GetEquipInfo(out int suitID)
	{
		suitID = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		suitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid3_bak";
			GoodsData gd = Global.GetIsBindGoodsDataByID(goodsID, this.CheckBoxBind.Check);
			if (gd == null)
			{
				gd = Global.GetDummyGoodsData(goodsID);
			}
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.isAutoSize = true;
			icon.BackSpriteName0 = backSpriteName;
			icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			icon.TipType = 1;
			icon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			icon.ItemCode = goodsID;
			icon.ItemNum = iNeedNub;
			icon.ItemObject = gd;
			icon.BoxTypes = 5;
			icon.STextVisibility = false;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				gd = (icon.ItemObject as GoodsData);
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			int num = 0;
			this.GetEquipInfo(out num);
			int num2 = 0;
			bool flag = false;
			num2 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "EquipSuit", ref flag, (long)num);
			int num3 = Global.GetTotalGoodsCountByID(goodsID);
			num3 += num2;
			icon.Text = string.Format("{0}/{1}", num3, iNeedNub);
			if (num3 >= iNeedNub)
			{
				icon.EnableIcon = true;
				icon.TextColor = 16777215U;
			}
			else
			{
				icon.EnableIcon = false;
				icon.TextColor = 16711680U;
			}
			icon.TextShadowColor = 4278190080U;
			icon.TextHorizontalAlignment = global::Layout.Right;
			icon.TextVerticalAlignment = global::Layout.Bottom;
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(icon);
		}
	}

	public void AddCailiao(GoodsData gd)
	{
		string text = string.Empty;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		XElement xilianXmlNode = Global.GetXilianXmlNode(goodsXmlNodeByID.XiLian.ToString());
		text = string.Empty;
		text = Global.GetXElementAttributeStr(xilianXmlNode, "NeedGoods");
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(new char[]
			{
				','
			});
			if (array != null && array.Length == 2)
			{
				NGUITools.SetActive(this.objCailiaoText, false);
				this.AddGoodsIcon(array[0].SafeToInt32(0), 1, array[1].SafeToInt32(0));
				this.RockGoodsID = array[0].SafeToInt32(0);
				this.NeedNum = array[1].SafeToInt32(0);
			}
		}
		this.NeedTongqianNum = Global.GetXElementAttributeInt(xilianXmlNode, "NeedJinBi");
		this.NeedZhuanshiNum = Global.GetXElementAttributeInt(xilianXmlNode, "NeedZuanShi");
		if (Global.isFanbei(106) && Global.JieriFanbeiInfo.ContainsKey(106))
		{
			JieRiFanBeiItemData jieRiFanBeiItemData = Global.JieriFanbeiInfo[106];
			this.NeedTongqianNum = (int)((double)this.NeedTongqianNum * Convert.ToDouble(jieRiFanBeiItemData.ExtArg1));
			if (this.NeedTongqianNum <= 0)
			{
				this.NeedTongqianNum = 1;
			}
			this.NeedZhuanshiNum = (int)((double)this.NeedZhuanshiNum * Convert.ToDouble(jieRiFanBeiItemData.ExtArg2));
			if (this.NeedZhuanshiNum <= 0)
			{
				this.NeedZhuanshiNum = 1;
			}
		}
		if (this.equipIcon[0].Length() > 0)
		{
			GoodsData money = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
			this.SetMoney(money);
		}
	}

	private void AddXilianPropsList(GoodsData gd, List<int> newPropsList = null, int id = 0)
	{
		uint colorByGoodsData = Global.GetColorByGoodsData(gd);
		if (null != this.SelectedItem && id == this.SelectedItem.ID && newPropsList != null)
		{
			if (newPropsList.Count == 2)
			{
				this.OldPropsText.textColor = colorByGoodsData;
				this.NewPropsText.textColor = colorByGoodsData;
				this.OldPropsText.Text = this.SelectedItem.TxtValue.Text;
				int num = newPropsList[0];
				if (ExtPropIndexes.ExtPropIndexPercents[num] == 1)
				{
					this.NewPropsText.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num]), newPropsList[1]);
				}
				else if (ExtPropIndexes.ExtPropIndexPercents[num] == 0)
				{
					this.NewPropsText.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num]), newPropsList[1]);
				}
			}
		}
		else
		{
			this.ItemCollection.Clear();
			List<int> washProps = gd.WashProps;
			if (gd.WashProps != null)
			{
				NGUITools.SetActive(this.QianghuaHintText.gameObject, false);
				for (int i = 0; i < washProps.Count; i += 2)
				{
					int num2 = washProps[i];
					int num3 = washProps[i + 1];
					LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.NEW<LianluZhuangbeiXilianPropsItem>();
					lianluZhuangbeiXilianPropsItem.RadioVisible = false;
					lianluZhuangbeiXilianPropsItem.PropsID = num2;
					lianluZhuangbeiXilianPropsItem.ID = this.ItemCollection.Count;
					lianluZhuangbeiXilianPropsItem.TxtValue.textColor = colorByGoodsData;
					if (newPropsList != null && id != -1)
					{
						if (id != -3)
						{
							lianluZhuangbeiXilianPropsItem.IsCompare = true;
						}
						else
						{
							lianluZhuangbeiXilianPropsItem.IsCompare = false;
						}
						if (newPropsList[i] == num2)
						{
							int num4 = newPropsList[i + 1];
							if (ExtPropIndexes.ExtPropIndexPercents[num2] == 1)
							{
								if (id != -3)
								{
									lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num2]), num3);
								}
								else
								{
									lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num2]), num4);
								}
							}
							else if (ExtPropIndexes.ExtPropIndexPercents[num2] == 0)
							{
								if (id != -3)
								{
									lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num2]), num3);
								}
								else
								{
									lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num2]), num4);
								}
							}
							int num5 = num4 - num3;
							lianluZhuangbeiXilianPropsItem.FlagState = num5;
							if (num5 != 0)
							{
								lianluZhuangbeiXilianPropsItem.TxtChangeValue.Text = Math.Abs(num5).ToString();
							}
						}
					}
					else
					{
						if (ExtPropIndexes.ExtPropIndexPercents[num2] == 1)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num2]), num3);
						}
						else if (ExtPropIndexes.ExtPropIndexPercents[num2] == 0)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num2]), num3);
						}
						lianluZhuangbeiXilianPropsItem.IsCompare = false;
					}
					this.ItemCollection.AddNoUpdate(lianluZhuangbeiXilianPropsItem);
				}
				this.ItemCollection.DelayUpdate();
			}
			else
			{
				this.QianghuaHintText.Text = Global.GetLang("暂无培养属性，需要材料激活");
				NGUITools.SetActive(this.QianghuaHintText.gameObject, true);
			}
		}
	}

	private void AddXilianPropsListNew(GoodsData gd, List<int> newPropsList = null, int id = 0)
	{
		uint colorByGoodsData = Global.GetColorByGoodsData(gd);
		this.ItemCollection.Clear();
		List<int> washProps = gd.WashProps;
		Dictionary<int, int> xilianPropsUpLimitDict = Global.GetXilianPropsUpLimitDict(gd);
		float xilianPropsUpFactor = Global.GetXilianPropsUpFactor(gd);
		if (newPropsList == null)
		{
			if (gd.WashProps != null)
			{
				for (int i = 0; i < gd.WashProps.Count; i += 2)
				{
					LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.NEW<LianluZhuangbeiXilianPropsItem>();
					lianluZhuangbeiXilianPropsItem.RadioVisible = false;
					lianluZhuangbeiXilianPropsItem.TxtValue.textColor = colorByGoodsData;
					lianluZhuangbeiXilianPropsItem.IsCompare = false;
					int num = gd.WashProps[i];
					int num2 = gd.WashProps[i + 1];
					if (ExtPropIndexes.ExtPropIndexPercents[num] == 1)
					{
						lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num]), num2);
					}
					else if (ExtPropIndexes.ExtPropIndexPercents[num] == 0)
					{
						lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num]), num2);
					}
					this.ShowUpLimitValue(lianluZhuangbeiXilianPropsItem, xilianPropsUpLimitDict, xilianPropsUpFactor, num, num2);
					this.ItemCollection.AddNoUpdate(lianluZhuangbeiXilianPropsItem);
				}
				this.ItemCollection.DelayUpdate();
			}
			else
			{
				foreach (int num3 in xilianPropsUpLimitDict.Keys)
				{
					if (xilianPropsUpLimitDict[num3] > 0)
					{
						LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.NEW<LianluZhuangbeiXilianPropsItem>();
						lianluZhuangbeiXilianPropsItem.RadioVisible = false;
						lianluZhuangbeiXilianPropsItem.TxtValue.textColor = colorByGoodsData;
						lianluZhuangbeiXilianPropsItem.IsCompare = false;
						if (ExtPropIndexes.ExtPropIndexPercents[num3] == 1)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num3]), 0);
						}
						else if (ExtPropIndexes.ExtPropIndexPercents[num3] == 0)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num3]), 0);
						}
						lianluZhuangbeiXilianPropsItem.IsCompare = false;
						this.ShowUpLimitValue(lianluZhuangbeiXilianPropsItem, xilianPropsUpLimitDict, xilianPropsUpFactor, num3, 0);
						this.ItemCollection.AddNoUpdate(lianluZhuangbeiXilianPropsItem);
					}
				}
				this.ItemCollection.DelayUpdate();
			}
		}
		else if (newPropsList != null)
		{
			for (int j = 0; j < newPropsList.Count; j += 2)
			{
				LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.NEW<LianluZhuangbeiXilianPropsItem>();
				lianluZhuangbeiXilianPropsItem.RadioVisible = false;
				lianluZhuangbeiXilianPropsItem.TxtValue.textColor = colorByGoodsData;
				lianluZhuangbeiXilianPropsItem.IsCompare = true;
				int num4 = newPropsList[j];
				int num5 = newPropsList[j + 1];
				int num6 = 0;
				if (gd.WashProps != null)
				{
					num6 = gd.WashProps[j + 1];
				}
				if (newPropsList != null)
				{
					if (id == -3)
					{
						lianluZhuangbeiXilianPropsItem.IsCompare = false;
						if (ExtPropIndexes.ExtPropIndexPercents[num4] == 1)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num5);
						}
						else if (ExtPropIndexes.ExtPropIndexPercents[num4] == 0)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num5);
						}
					}
					else
					{
						if (ExtPropIndexes.ExtPropIndexPercents[num4] == 1)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}%", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num6);
						}
						else if (ExtPropIndexes.ExtPropIndexPercents[num4] == 0)
						{
							lianluZhuangbeiXilianPropsItem.TxtValue.Text = string.Format("{0} +{1}", Global.GetLang(ExtPropIndexes.ChineseNames[num4]), num6);
						}
						int num7 = num5 - num6;
						lianluZhuangbeiXilianPropsItem.FlagState = num7;
						if (num7 != 0)
						{
							lianluZhuangbeiXilianPropsItem.TxtChangeValue.Text = Math.Abs(num7).ToString();
						}
					}
				}
				this.ShowUpLimitValue(lianluZhuangbeiXilianPropsItem, xilianPropsUpLimitDict, xilianPropsUpFactor, num4, num6);
				this.ItemCollection.AddNoUpdate(lianluZhuangbeiXilianPropsItem);
			}
			this.ItemCollection.DelayUpdate();
		}
	}

	private int ShowUpLimitValue(LianluZhuangbeiXilianPropsItem item, Dictionary<int, int> dict, float factor, int key, int currentValue)
	{
		int num = 0;
		if (dict != null && dict.TryGetValue(key, ref num))
		{
			num = (int)((float)num * factor);
			if (currentValue >= num)
			{
				item.TxtRangeValue.Text = Global.GetLang("(已达上限)");
				item.TxtRangeValue.textColor = 16565774U;
			}
			else
			{
				item.TxtRangeValue.textColor = 11692285U;
				item.TxtRangeValue.Text = string.Format(Global.GetLang("(上限{0})"), num);
			}
		}
		return num;
	}

	private void ItemMouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.RadioIndex == 0)
		{
			return;
		}
		LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.AS<LianluZhuangbeiXilianPropsItem>(this.ProsList.SelectedItem);
		if (null == lianluZhuangbeiXilianPropsItem)
		{
			return;
		}
		if (this.SelectedItem == lianluZhuangbeiXilianPropsItem)
		{
			return;
		}
		if (this.SelectedItem != null && this.SelectedItem != lianluZhuangbeiXilianPropsItem)
		{
			this.SelectedItem.RadioVisible = false;
		}
		this.SelectedItem = lianluZhuangbeiXilianPropsItem;
		this.SelectedItem.RadioVisible = true;
	}

	private void SelectItem(int index = 0)
	{
		if (this.RadioIndex == 0)
		{
			return;
		}
		this.ProsList.SelectedIndex = index;
		LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.AS<LianluZhuangbeiXilianPropsItem>(this.ProsList.SelectedItem);
		if (null == lianluZhuangbeiXilianPropsItem)
		{
			return;
		}
		lianluZhuangbeiXilianPropsItem.RadioVisible = true;
		this.SelectedItem = lianluZhuangbeiXilianPropsItem;
	}

	private void SelectRadio(int index)
	{
		for (int i = 0; i < this.radioIcon.Length; i++)
		{
			if (i == index)
			{
				this.radioIcon[i].Check = true;
				this.RadioIndex = i;
			}
			else
			{
				this.radioIcon[i].Check = false;
			}
		}
		for (int j = 0; j < this.ProsList.Length(); j++)
		{
			LianluZhuangbeiXilianPropsItem lianluZhuangbeiXilianPropsItem = U3DUtils.AS<LianluZhuangbeiXilianPropsItem>(this.ProsList[j]);
			lianluZhuangbeiXilianPropsItem.RadioVisible = false;
			if (this.RadioIndex == 1)
			{
				lianluZhuangbeiXilianPropsItem.IsCompare = false;
			}
		}
	}

	private void SelectRadioMoney(int index)
	{
		for (int i = 0; i < this.radioIconMoney.Length; i++)
		{
			if (i == index)
			{
				this.radioIconMoney[i].Check = true;
				this.RadioMoneyIndex = i;
			}
			else
			{
				this.radioIconMoney[i].Check = false;
			}
		}
		if (this.equipIcon[0].Length() > 0)
		{
			GoodsData money = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
			this.SetMoney(money);
		}
	}

	private void SetMoney(GoodsData gd)
	{
		this.TongqianText.textColor = 16777215U;
		this.YuanbaoText.textColor = 16777215U;
		this.TongqianText.Text = "0";
		this.YuanbaoText.Text = "0";
		if (this.RadioMoneyIndex == 0)
		{
			this.TongqianText.Text = this.NeedTongqianNum.ToString();
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedTongqianNum)
			{
				this.TongqianText.textColor = 16711680U;
			}
			else
			{
				this.TongqianText.textColor = 16777215U;
			}
		}
		else if (this.RadioMoneyIndex == 1)
		{
			this.YuanbaoText.Text = this.NeedZhuanshiNum.ToString();
			if (IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiPeiYang", this.NeedZhuanshiNum, false))
			{
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId);
				if (totalGoodsCountByID < this.NeedZhuanshiNum)
				{
					this.YuanbaoText.textColor = 16711680U;
				}
				else
				{
					this.YuanbaoText.textColor = 16777215U;
				}
				this.xiaoHaoDaiBi.spriteName = "XiaoHaoHuanLeDaiBi";
			}
			else
			{
				if (Global.Data.roleData.UserMoney < this.NeedZhuanshiNum)
				{
					this.YuanbaoText.textColor = 16711680U;
				}
				else
				{
					this.YuanbaoText.textColor = 16777215U;
				}
				this.xiaoHaoDaiBi.spriteName = "XiaoHaoZuanShi";
			}
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "ZhuangBeiPeiYang", this.NeedZhuanshiNum, string.Empty);
		}
	}

	private List<string[]> GetPropsStrArr(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		List<string[]> list = new List<string[]>();
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			list.Add(array2);
		}
		return list;
	}

	private string ProcessStr(string str)
	{
		if (str.Length > 0 && str.Substring(str.Length - 1) == "|")
		{
			str = str.Substring(0, str.Length - 1);
		}
		return str;
	}

	public void StartSubmit(int id = -1)
	{
		int num = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			if (this.CheckBoxAutoSaveBind.Check)
			{
				this.isClick = false;
				this.StopAutoTiSheng_Tick();
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入要培养的装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		this.EquipDbID = goodsData.Id;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int binding = goodsData.Binding;
		if (goodsData.WashProps == null)
		{
			id = -2;
		}
		else if (this.RadioIndex == 0)
		{
			id = -2;
		}
		else if (this.RadioIndex == 1 && null == this.SelectedItem)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请先选中要培养的属性"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (Global.IsToXilianPropsUpLimit(goodsData))
		{
			if (this.CheckBoxAutoSaveBind.Check || this.RadioMoneyIndex == 1)
			{
				this.StopAutoTiSheng_Tick();
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备的培养属性已达到上限"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.RadioMoneyIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedTongqianNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币数量不足"), new object[0]), 0, -1, -1, 0);
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioMoneyIndex == 1 && Global.Data.roleData.UserMoney < this.NeedZhuanshiNum && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiPeiYang", this.NeedZhuanshiNum, false))
		{
			if (this.CheckBoxAutoSaveBind.Check || this.RadioMoneyIndex == 1)
			{
				this.StopAutoTiSheng_Tick();
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("钻石数量不足"), new object[0]), 0, -1, -1, 0);
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			return;
		}
		GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]);
		int itemNum = ggoodIcon.ItemNum;
		int itemCode = ggoodIcon.ItemCode;
		int num2 = Global.GetTotalGoodsCountByID(itemCode);
		goodsData = Global.GetGoodsDataByID(itemCode);
		int num3 = 0;
		this.GetEquipInfo(out num3);
		int num4 = 0;
		bool flag = false;
		num4 += ConfigReplaceGoodVO.GetReplaceGoodCount(itemCode, "EquipSuit", ref flag, (long)num3);
		if (flag)
		{
			num = 1;
		}
		num2 += num4;
		if (num2 < itemNum)
		{
			if (Super.ShowGoodsGuide(itemCode, this.callback) == 1)
			{
				if (this.CheckBoxAutoSaveBind.Check || this.RadioMoneyIndex == 1)
				{
					this.isClick = false;
					this.StopAutoTiSheng_Tick();
				}
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料{0}，无法培养"), new object[]
				{
					Global.GetGoodsNameByID(itemCode, false)
				}), 19, -1, -1, itemCode);
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinhuajingshi, this.callback, string.Empty, string.Empty);
			}
			return;
		}
		goodsData = (ggoodIcon.ItemObject as GoodsData);
		if (goodsData != null && goodsData.Binding == 1)
		{
			num = goodsData.Binding;
		}
		if (this.isPart)
		{
			return;
		}
		if (binding == 0 && num == 1)
		{
			this.isPart = true;
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang = Global.GetLang("存在绑定的材料，操作后您的装备将变为绑定，确认要执行该操作吗?");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				this.isPart = false;
				if (e1.ID == 0)
				{
					this.StartSubmitExe(this.EquipDbID, id);
				}
				else
				{
					this.isClick = false;
					this.SubmitBtn.Text = Global.GetLang("培养");
					this.StopAutoTiSheng_Tick();
				}
			}, buttons);
			return;
		}
		this.StartSubmitExe(this.EquipDbID, id);
	}

	public void StartSubmitExe(int dbID, int id = -1)
	{
		if (this.RadioMoneyIndex == 1)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiPeiYang", this.NeedZhuanshiNum, false);
			if (id == -2)
			{
				if (this.isContinueTiLian)
				{
					this.isContinueTiLian = false;
					this.ShowModalDialog();
					this.LastOperation = id;
					GameInstance.Game.SpriteEquipWashPropCmd(dbID, id, (!this.CheckBoxBind.Check) ? 0 : 1, this.RadioMoneyIndex);
				}
			}
			else if (id == -3)
			{
				this.ShowModalDialog();
				this.LastOperation = id;
				GameInstance.Game.SpriteEquipWashPropCmd(dbID, id, (!this.CheckBoxBind.Check) ? 0 : 1, this.RadioMoneyIndex);
			}
			else if (id == -5)
			{
				this.ShowModalDialog();
				this.LastOperation = id;
				GameInstance.Game.SpriteEquipWashPropCmd(dbID, id, (!this.CheckBoxBind.Check) ? 0 : 1, this.RadioMoneyIndex);
			}
		}
		else
		{
			this.ShowModalDialog();
			this.LastOperation = id;
			GameInstance.Game.SpriteEquipWashPropCmd(dbID, id, (!this.CheckBoxBind.Check) ? 0 : 1, this.RadioMoneyIndex);
		}
	}

	private void AutoTiSheng_Tick()
	{
		this.StartSubmit(-1);
	}

	private void StopAutoTiSheng_Tick()
	{
		base.CancelInvoke("AutoTiSheng_Tick");
		this.SubmitBtn.Text = Global.GetLang("培养");
		this.CheckBoxAutoSaveBind.Check = false;
	}

	public void StopAutoTiShengWhenChangeItem()
	{
		if ((this.CheckBoxAutoSaveBind.Check || this.RadioMoneyIndex == 1) && base.IsInvoking("AutoTiSheng_Tick"))
		{
			this.isClick = false;
			this.StopAutoTiSheng_Tick();
			Super.HideNetWaiting();
		}
	}

	public void StartReplace(int id)
	{
		if (this.equipIcon[0].Length() <= 0)
		{
			if (this.CheckBoxAutoSaveBind.Check)
			{
				this.isClick = false;
				this.StopAutoTiSheng_Tick();
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入要培养的装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.EquipDbID = goodsData.Id;
		this.StartSubmitExe(this.EquipDbID, id);
	}

	public void NotifyXilianResult(int washIndex, int result, GoodsData goodsData, List<int> newPropsList)
	{
		this.CloseModalDialog();
		if (newPropsList == null || newPropsList.Count % 2 != 0)
		{
			return;
		}
		if (IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("ZhuangBeiPeiYang", this.NeedZhuanshiNum) && this.CheckBoxAutoSaveBind.Check && this.RadioMoneyIndex == 1)
		{
			this.StopAutoTiSheng_Tick();
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "ZhuangBeiPeiYang", this.NeedZhuanshiNum, string.Empty);
		if (result < 0)
		{
			if (this.CheckBoxAutoSaveBind.Check || this.RadioMoneyIndex == 1)
			{
				this.StopAutoTiSheng_Tick();
			}
			if (result == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料，无法培养"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("培养时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
			this.AddEquip(goodsData, result);
			return;
		}
		Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		if (washIndex > 0 || washIndex == 0 || washIndex == -2)
		{
			if (this.LastOperation == -5)
			{
				this.SetBtnState(washIndex, false);
			}
			else if (this.RadioMoneyIndex == 1)
			{
				this.StartReplace(-3);
				Global.SavePeiYangTimes();
				Global.ShowZhuangBeiPeiYangRedPoint();
			}
			else
			{
				if (this.CheckBoxAutoSaveBind.Check)
				{
					this.StartReplace(-3);
				}
				else
				{
					this.SetBtnState(washIndex, false);
				}
				Global.SavePeiYangTimes();
				Global.ShowZhuangBeiPeiYangRedPoint();
			}
		}
		else
		{
			this.SetBtnState(washIndex, false);
		}
		this.AddXilianPropsListNew(goodsData, newPropsList, washIndex);
		this.AddEquipGoodsIcon(goodsData, 0, false, 0);
		this.AddCailiao(goodsData);
		this.SetChangZhandouli(true, goodsData, newPropsList, washIndex);
		if (washIndex == -2)
		{
			Global.AddDBIdToDictByXilian(goodsData.Id);
		}
		if (washIndex == -3 || washIndex == -1)
		{
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1
			});
			this.ZhandouliText.Text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(goodsData));
			this.isContinueTiLian = true;
			Global.RemoveDBIdFromDictByXilian(goodsData.Id);
		}
		if (this.RadioIndex == 1 && washIndex == -3)
		{
			NGUITools.SetActive(this.PropsPanel[0].gameObject, true);
			NGUITools.SetActive(this.PropsPanel[1].gameObject, false);
			this.SelectItem(this.SelectedItem.ID);
		}
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public void ProcessUnSave(DPSelectedItemEventHandler handler)
	{
		if (LianluZhuangbeiXilianPart.IsSave)
		{
			return;
		}
		string[] buttons = new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		};
		string lang = Global.GetLang("培养属性没有保存，是否保存?");
		Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
		{
			if (e1.ID == 0 && this.ReplaceBtn.isEnabled)
			{
				this.StartReplace(-3);
			}
			LianluZhuangbeiXilianPart.IsSave = true;
			if (handler != null)
			{
				handler(s1, e1);
			}
		}, buttons);
	}

	public static bool IsSave = true;

	public bool isPart;

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public GButton SubmitBtn;

	public GButton ReplaceBtn;

	public SpriteSL Equip;

	public SpriteSL Cailiao;

	public TextBlock QianghuaHintText;

	public TextBlock ZhandouliText;

	public GCheckBox RadioXianlianValue;

	public GCheckBox RadioXianlianProps;

	public GCheckBox CheckBoxBind;

	public GCheckBox RadioTongqian;

	public GCheckBox RadioYuanbao;

	public GCheckBox CheckBoxAutoSaveBind;

	public GameObject[] PropsPanel;

	public ListBox ProsList;

	public TextBlock OldPropsText;

	public TextBlock NewPropsText;

	public GameObject objCailiaoText;

	public TextBlock TongqianText;

	public TextBlock YuanbaoText;

	public SpriteSL ChangeZhandouli;

	public UISprite ChangeFlag;

	public TextBlock ChangeValueText;

	public UISprite spZuanShi;

	public UISprite xiaoHaoDaiBi;

	private int RadioIndex;

	private int RadioMoneyIndex = 1;

	private int EquipDbID = -1;

	private int RockGoodsID = -1;

	private int NeedNum = -1;

	private int NeedTongqianNum;

	private int NeedZhuanshiNum;

	private GChildWindow messageBoxWindow;

	public bool isClick;

	public int LastOperation = -1;

	public bool _isContinueTiLian = true;

	private ObservableCollection _ItemCollection;

	private SpriteSL[] _equipIcon;

	private GCheckBox[] _radioIcon;

	private GCheckBox[] _radioIconMoney;

	private int __flag_pre_equipid = -1;

	private LianluZhuangbeiXilianPropsItem SelectedItem;
}
