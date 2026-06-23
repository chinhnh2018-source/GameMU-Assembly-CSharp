using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class LianluQianghuaChuanchenPart : UserControl
{
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

	public GCheckBox[] radioIconMode
	{
		get
		{
			return this._radioIconMode;
		}
		set
		{
			this._radioIconMode = value;
		}
	}

	public GButton[] clearIcon
	{
		get
		{
			return this._clearIcon;
		}
		set
		{
			this._clearIcon = value;
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

	public bool ChuanChengZhong
	{
		get
		{
			return this._ChuanChengZhong;
		}
		set
		{
			this._ChuanChengZhong = value;
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

	private void InitTextInPrefabs()
	{
		this.SubmitBtn.Text = Global.GetLang("传承");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public void InitPartSize(int width, int height)
	{
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney >= this.NeedMoney && this.NeedMoney > 0 && Global.GetZuanShi(ZuanShiPartClass.ZhuangBeiChuanCheng))
			{
				if (this.messageBoxWindow != null)
				{
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
				}
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "ZhuangBeiChuanCheng", this.NeedMoney);
				this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.NeedMoney, text)
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
						Global.SetZuanShi(ZuanShiPartClass.ZhuangBeiChuanCheng, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
					}
					Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						if (this.RadioModeIndex == 1)
						{
							this.StartChuancheng();
						}
						else if (this.RadioModeIndex == 2)
						{
							this.StartChuancheng2();
						}
						else if (this.RadioModeIndex == 3)
						{
							this.StartChuancheng3();
						}
						else if (this.RadioModeIndex == 4)
						{
							this.StartJuHunChuanCheng();
						}
						else if (this.RadioModeIndex == 5)
						{
							this.StartFuMoChuanCheng();
						}
					}
					return true;
				};
				return;
			}
			if (this.RadioModeIndex == 1)
			{
				this.StartChuancheng();
			}
			else if (this.RadioModeIndex == 2)
			{
				this.StartChuancheng2();
			}
			else if (this.RadioModeIndex == 3)
			{
				this.StartChuancheng3();
			}
			else if (this.RadioModeIndex == 4)
			{
				this.StartJuHunChuanCheng();
			}
			else if (this.RadioModeIndex == 5)
			{
				this.StartFuMoChuanCheng();
			}
			SystemHelpMgr.OnAction(UIObjIDs.LianLuChuanChengSubmit, HelpStateEvents.Clicked, -1);
		};
		this.ClearSubIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			if (this.equipIcon[1].Length() > 0)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.RadioModeIndex,
					FilterType = 1,
					ZhuZhuangBei = null,
					FuZhuangBei = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData)
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.RadioModeIndex,
					FilterType = 1,
					ZhuZhuangBei = null,
					FuZhuangBei = null
				});
			}
		};
		this.ClearAddIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(1);
			if (this.equipIcon[0].Length() > 0)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.RadioModeIndex,
					FilterType = 1,
					ZhuZhuangBei = (U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData),
					FuZhuangBei = null
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.RadioModeIndex,
					FilterType = 1,
					ZhuZhuangBei = null,
					FuZhuangBei = null
				});
			}
		};
		this.RadioTongqian.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectRadio(0);
			SystemHelpMgr.OnAction(UIObjIDs.LianLuChuanChengSubmit, HelpStateEvents.Clicked, -1);
		};
		this.RadioYuanbao.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectRadio(1);
		};
		this.RadioQianghuaChuancheng._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("强化")
		});
		this.RadioQianghuaChuancheng.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectModeRadio(0);
		};
		this.RadioZhuijiaChuancheng._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("追加")
		});
		this.RadioZhuijiaChuancheng.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectModeRadio(1);
		};
		this.RadioXilianChuancheng._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("培养")
		});
		this.RadioXilianChuancheng.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectModeRadio(2);
		};
		this.RadioJuHunChuancheng._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("聚魂")
		});
		this.RadioJuHunChuancheng.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectModeRadio(3);
		};
		this.RadioFuMoChuancheng._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("附魔")
		});
		this.RadioFuMoChuancheng.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectModeRadio(4);
		};
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[2];
		this.equipIcon[0] = this.EquipSub;
		this.equipIcon[1] = this.EquipAdd;
		this.clearIcon = new GButton[2];
		this.clearIcon[0] = this.ClearSubIcon;
		this.clearIcon[1] = this.ClearAddIcon;
		this.radioIcon = new GCheckBox[2];
		this.radioIcon[0] = this.RadioTongqian;
		this.radioIcon[1] = this.RadioYuanbao;
		this.radioIconMode = new GCheckBox[5];
		this.radioIconMode[0] = this.RadioQianghuaChuancheng;
		this.radioIconMode[1] = this.RadioZhuijiaChuancheng;
		this.radioIconMode[2] = this.RadioXilianChuancheng;
		this.radioIconMode[3] = this.RadioJuHunChuancheng;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (ConfigVersionSystemOpen.IsVersionSystemOpen(100104) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.FuMo, ref num, ref num2, ref num3))
		{
			this.radioIconMode[4] = this.RadioFuMoChuancheng;
		}
		else
		{
			this.RadioFuMoChuancheng.gameObject.SetActive(false);
		}
	}

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.TongqianText.Text = "0";
		this.YuanbaoText.Text = "0";
		this.ChenggonglvText.Text = "0%";
		for (int j = 0; j < this.clearIcon.Length; j++)
		{
			NGUITools.SetActive(this.clearIcon[j].gameObject, false);
		}
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 2)
		{
			this.equipIcon[index].Clear();
			NGUITools.SetActive(this.clearIcon[index].gameObject, false);
		}
		if (this.NeedMoney <= 0)
		{
			this.spZuanShi.spriteName = "diamond";
			this.spXiaoHao.spriteName = "XiaoHaoZuanShi";
		}
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
		if (this.equipIcon[0].Length() > 0)
		{
			GoodsData money = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
			this.SetMoney(money);
		}
	}

	private void SelectModeRadio(int index)
	{
		for (int i = 0; i < this.radioIconMode.Length; i++)
		{
			if (!(this.radioIconMode[i] == null))
			{
				if (i == index)
				{
					this.radioIconMode[i].Check = true;
					this.RadioModeIndex = i + 1;
				}
				else
				{
					this.radioIconMode[i].Check = false;
				}
			}
		}
		this.InitAllValue();
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = this.RadioModeIndex,
			FilterType = 1,
			ZhuZhuangBei = null,
			FuZhuangBei = null
		});
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			if (this.IsSucess)
			{
				this.InitAllValue();
			}
			if (this.equipIcon[0].Length() > 0 && this.equipIcon[1].Length() > 0)
			{
				return;
			}
			if (this.equipIcon[0].Length() <= 0 && this.equipIcon[1].Length() <= 0)
			{
				this.AddEquipGoodsIcon(gd, 0, false, 0);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.RadioModeIndex,
					FilterType = 1,
					ZhuZhuangBei = gd,
					FuZhuangBei = null
				});
				return;
			}
			if (this.equipIcon[0].Length() <= 0 && this.equipIcon[1].Length() > 0)
			{
				this.AddEquipGoodsIcon(gd, 0, false, 0);
				return;
			}
			if (this.equipIcon[1].Length() <= 0 && this.equipIcon[0].Length() > 0)
			{
				this.AddEquipGoodsIcon(gd, 1, false, 0);
				this.ClearEquip(2);
				return;
			}
		}
	}

	private void AddEquipGoodsIcon(GoodsData gd, int index, bool grayShow = false, int goodsOwnerType = 0)
	{
		this.IsSucess = false;
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
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			if (this.RadioModeIndex == 1 || this.RadioModeIndex == 3 || this.RadioModeIndex == 4 || this.RadioModeIndex == 5)
			{
				Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			}
			else if (this.RadioModeIndex == 2)
			{
				Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Zhuijia);
			}
			this.equipIcon[index].Add(icon);
			NGUITools.SetActive(this.clearIcon[index].gameObject, true);
			if (index == 0)
			{
				this.SetMoney(gd);
				if (this.RadioModeIndex == 1)
				{
					this.ChenggonglvText.Text = Global.GetChuanchengDiaojilv(gd.Forge_level).ToString() + "%";
				}
				else if (this.RadioModeIndex == 2)
				{
					this.ChenggonglvText.Text = Global.GetZhuijiaChuanchengDiaojilv(gd.AppendPropLev).ToString() + "%";
				}
				else if (this.RadioModeIndex == 3)
				{
					int washCount = (gd.WashProps != null) ? (gd.WashProps.Count / 2) : 0;
					this.ChenggonglvText.Text = Global.GetXilianChuanchengDiaojilv(washCount).ToString() + "%";
				}
				else if (this.RadioModeIndex == 4)
				{
					if (gd.JuHunID == 0)
					{
						this.ChenggonglvText.Text = "0%";
					}
					else
					{
						this.ChenggonglvText.Text = Global.GetJuHunChuanchengDiaojilv(ParseJuHunConfig.GetJuHunDataById(gd.JuHunID).Type).ToString() + "%";
					}
				}
				else if (this.RadioModeIndex == 5)
				{
					this.ChenggonglvText.Text = "100%";
				}
				IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "ZhuangBeiChuanCheng", this.NeedMoney, string.Empty);
				if (IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiChuanCheng", this.NeedMoney, false))
				{
					this.spXiaoHao.spriteName = "XiaoHaoHuanLeDaiBi";
				}
				else
				{
					this.spXiaoHao.spriteName = "XiaoHaoZuanShi";
				}
			}
		}
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		this.DiaojilvText.Text = "0%";
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = null;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.Text = iNeedNub.ToString();
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			if (Global.GetTotalGoodsCountByID(goodsID) > 0)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			this.equipIcon[index].Clear();
			this.equipIcon[index].Add(ggoodIcon);
		}
	}

	private void SetMoney(GoodsData gd)
	{
		int num = 0;
		this.TongqianText.textColor = 16777215U;
		this.YuanbaoText.textColor = 16777215U;
		this.TongqianText.Text = "0";
		this.YuanbaoText.Text = "0";
		if (this.RadioIndex == 0)
		{
			if (this.RadioModeIndex == 1)
			{
				num = Global.GetChuanchengMoney(gd.Forge_level, 0);
			}
			else if (this.RadioModeIndex == 2)
			{
				num = Global.GetZhuijiaChuanchengMoney(gd.AppendPropLev, 0);
			}
			else if (this.RadioModeIndex == 3)
			{
				if (gd.WashProps != null)
				{
					num = Global.GetXilianChuanchengMoney(gd.WashProps.Count / 2, 0);
				}
			}
			else if (this.RadioModeIndex == 4)
			{
				if (gd.JuHunID == 0)
				{
					num = 0;
				}
				else
				{
					num = Global.GetJuHunChuanChengMoney(ParseJuHunConfig.GetJuHunDataById(gd.JuHunID).Type, 0);
				}
			}
			else if (this.RadioModeIndex == 5)
			{
				num = ConfigSystemParam.GetSystemParamIntArrayByName("EnchantmentInheritCost", ',')[0];
			}
			this.TongqianText.Text = num.ToString();
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < num)
			{
				this.TongqianText.textColor = 16711680U;
			}
			else
			{
				this.TongqianText.textColor = 16777215U;
			}
		}
		else if (this.RadioIndex == 1)
		{
			if (this.RadioModeIndex == 1)
			{
				num = Global.GetChuanchengMoney(gd.Forge_level, 1);
			}
			else if (this.RadioModeIndex == 2)
			{
				num = Global.GetZhuijiaChuanchengMoney(gd.AppendPropLev, 1);
			}
			else if (this.RadioModeIndex == 3)
			{
				if (gd.WashProps != null)
				{
					num = Global.GetXilianChuanchengMoney(gd.WashProps.Count / 2, 1);
				}
			}
			else if (this.RadioModeIndex == 4)
			{
				if (gd.JuHunID == 0)
				{
					num = 0;
				}
				else
				{
					num = Global.GetJuHunChuanChengMoney(ParseJuHunConfig.GetJuHunDataById(gd.JuHunID).Type, 1);
				}
			}
			else if (this.RadioModeIndex == 5)
			{
				num = ConfigSystemParam.GetSystemParamIntArrayByName("EnchantmentInheritCost", ',')[1];
			}
			this.YuanbaoText.Text = num.ToString();
			if (Global.Data.roleData.UserMoney < num)
			{
				this.YuanbaoText.textColor = 16711680U;
			}
			else
			{
				this.YuanbaoText.textColor = 16777215U;
			}
		}
		this.NeedMoney = num;
	}

	private bool CheckEquip(GoodsData gd, int index)
	{
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(gd.GoodsID);
		if (categoriyByGoodsID == 7)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("护符不能传承，无法放入"), new object[0]), 0, -1, -1, 0);
			return false;
		}
		GoodsData goodsData;
		int categoriyByGoodsID2;
		if (index != 0)
		{
			if (index == 1)
			{
				goodsData = (U3DUtils.AS<GIcon>(this.equipIcon[0][0]).ItemObject as GoodsData);
				if (goodsData != null)
				{
					if (gd.Forge_level <= goodsData.Forge_level)
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("将要放入的的副装备没有主装备强化等级高，无法放入"), new object[0]), 0, -1, -1, 0);
						return false;
					}
					categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if (categoriyByGoodsID == categoriyByGoodsID2)
					{
						return true;
					}
					if ((categoriyByGoodsID == 0 || categoriyByGoodsID == 10) && (categoriyByGoodsID2 == 0 || categoriyByGoodsID2 == 10))
					{
						return true;
					}
					if ((categoriyByGoodsID == 1 || categoriyByGoodsID == 11) && (categoriyByGoodsID2 == 1 || categoriyByGoodsID2 == 11))
					{
						return true;
					}
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("将要放入的副装备与主装备不是同一部件，无法放入"), new object[0]), 0, -1, -1, 0);
					return false;
				}
			}
			return true;
		}
		goodsData = (U3DUtils.AS<GIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		if (goodsData == null)
		{
			return true;
		}
		if (gd.Forge_level >= goodsData.Forge_level)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("将要放入的主装备比副装备强化等级高，无法放入"), new object[0]), 0, -1, -1, 0);
			return false;
		}
		categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		if (categoriyByGoodsID == categoriyByGoodsID2)
		{
			return true;
		}
		if ((categoriyByGoodsID == 0 || categoriyByGoodsID == 10) && (categoriyByGoodsID2 == 0 || categoriyByGoodsID2 == 10))
		{
			return true;
		}
		if ((categoriyByGoodsID == 1 || categoriyByGoodsID == 11) && (categoriyByGoodsID2 == 1 || categoriyByGoodsID2 == 11))
		{
			return true;
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("将要放入的主装备与副装备不是同一部件，无法放入"), new object[0]), 0, -1, -1, 0);
		return false;
	}

	private bool CheckShengyoufu(int goodsID)
	{
		if (this.equipIcon[1].Length() <= 0)
		{
			return false;
		}
		int forge_level = (U3DUtils.AS<GIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Forge_level;
		return Global.CheckShengyoufuIsHefa(goodsID, forge_level);
	}

	private void StartChuancheng()
	{
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		int forge_level = goodsData.Forge_level;
		int binding = goodsData.Binding;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		int forge_level2 = goodsData.Forge_level;
		int binding2 = goodsData.Binding;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (forge_level < forge_level2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("旧强化等级较高，无法传承"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (forge_level == forge_level2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备强化等级相同，无法传承"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.RadioIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < this.NeedMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiChuanCheng", this.NeedMoney, false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			return;
		}
		if (binding == 1 && binding2 == 0 && forge_level2 == 0)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang = Global.GetLang("操作后新装备将被绑定，是否确认进行传承?");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.ShowModalDialog();
					GameInstance.Game.SpriteDoEquipInherit((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, this.RadioIndex + 1);
				}
			}, buttons);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteDoEquipInherit((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, this.RadioIndex + 1);
	}

	public void OnEquipInheritCompleted(int result, GoodsData leftGoodsData, GoodsData rightGoodsData)
	{
		this.CloseModalDialog();
		string goodsNameByID = Global.GetGoodsNameByID(leftGoodsData.GoodsID, false);
		string goodsNameByID2 = Global.GetGoodsNameByID(rightGoodsData.GoodsID, false);
		string text = Global.GetGoodsNameByID(this.NeedShengYouGoodsID, false);
		if (text.Length <= 0)
		{
			text = Global.GetLang("神佑符");
		}
		if (result < 1)
		{
			if (result == -21)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			}
			if (result == -22)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			}
			if (result == -23)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败"), new object[0]), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
		}
		else if (result == 1 || result == 2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，传承成功"), new object[0]), 0, -1, -1, 0);
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		}
		else if (result == 3)
		{
		}
		this.InitAllValue();
		this.AddEquipGoodsIcon(leftGoodsData, 0, false, 0);
		this.AddEquipGoodsIcon(rightGoodsData, 1, false, 0);
		if (result == 1 || result == 2)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
			if (this.DPEffectItem != null)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 1
				});
			}
			this.IsSucess = true;
		}
	}

	private void StartChuancheng2()
	{
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		int appendPropLev = goodsData.AppendPropLev;
		int binding = goodsData.Binding;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		int appendPropLev2 = goodsData.AppendPropLev;
		int binding2 = goodsData.Binding;
		int maxZhuijiaLevelByGoodsData = Global.GetMaxZhuijiaLevelByGoodsData(goodsData);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (appendPropLev < appendPropLev2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备追加等级较高，无法传承"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (appendPropLev == appendPropLev2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备追加等级相同，无法传承"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.RadioIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < this.NeedMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiChuanCheng", this.NeedMoney, false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			return;
		}
		List<int> list = new List<int>();
		if (binding == 1 && binding2 == 0)
		{
			list.Add(0);
		}
		if (appendPropLev > maxZhuijiaLevelByGoodsData)
		{
			list.Add(1);
		}
		this.StartChuancheng2Exe(list);
	}

	private void StartChuancheng2Exe(List<int> stateList)
	{
		string[] buttons = new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		};
		string[] array = new string[]
		{
			Global.GetLang("操作后新装备将被绑定，是否确认进行传承?"),
			Global.GetLang("{00FF00}剥离装备{-}的追加等级超过了{00FF00}继承装备{-}的追加上限值，将只能继承至该装备上限，是否确认进行传承?")
		};
		if (stateList.Count > 0)
		{
			Super.ShowMessageBoxEx(Global.GetLang("提示"), array[stateList[0]], delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					stateList.RemoveAt(0);
					this.StartChuancheng2Exe(stateList);
				}
			}, buttons);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteEquipAppendInheritCmd((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, this.RadioIndex + 1);
	}

	public void OnEquipInheritCompleted2(int result, GoodsData leftGoodsData, GoodsData rightGoodsData)
	{
		this.CloseModalDialog();
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.spZuanShi, "ZhuangBeiChuanCheng", this.NeedMoney, string.Empty);
		string goodsNameByID = Global.GetGoodsNameByID(leftGoodsData.GoodsID, false);
		string goodsNameByID2 = Global.GetGoodsNameByID(rightGoodsData.GoodsID, false);
		if (result < 1)
		{
			if (result == -21)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			}
			if (result == -22)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			}
			if (result == -23)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承失败"), new object[0]), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			if (this.DPEffectItem != null)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = -1
				});
			}
		}
		else if (result == 1 || result == 2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，传承成功"), new object[0]), 0, -1, -1, 0);
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		}
		else if (result == 3)
		{
		}
		this.InitAllValue();
		this.AddEquipGoodsIcon(leftGoodsData, 0, false, 0);
		this.AddEquipGoodsIcon(rightGoodsData, 1, false, 0);
		if (result == 1 || result == 2)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
			if (this.DPEffectItem != null)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 1
				});
			}
			this.IsSucess = true;
		}
	}

	private void StartChuancheng3()
	{
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (goodsData.WashProps == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备没有培养属性"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int num = (goodsData.WashProps != null) ? (goodsData.WashProps.Count / 2) : 0;
		int binding = goodsData.Binding;
		GoodsData gd = goodsData;
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (goodsData.WashProps == null)
		{
		}
		int maxWashPropsCountByGoodsData = Global.GetMaxWashPropsCountByGoodsData(goodsData);
		int binding2 = goodsData.Binding;
		GoodsData gd2 = goodsData;
		if (this.RadioIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < this.NeedMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiChuanCheng", this.NeedMoney, false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			return;
		}
		List<int> list = new List<int>();
		if (binding == 1 && binding2 == 0)
		{
			list.Add(0);
		}
		if (Global.IsXilianProps1MoreThan2UpLimit(gd, gd2))
		{
			list.Add(1);
		}
		this.StartChuancheng3Exe(list);
	}

	private void StartJuHunChuanCheng()
	{
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		bool flag = goodsData.Binding == 1;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (goodsData.JuHunID == 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备没有聚魂属性"), new object[0]), 0, -1, -1, 0);
			return;
		}
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		bool flag2 = goodsData.Binding == 1;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (flag && !flag2)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang = Global.GetLang("接收传承装备将会被绑定，确定要传承吗？");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					if (goodsData.JuHunID > 0)
					{
						string[] buttons3 = new string[]
						{
							Global.GetLang("确定"),
							Global.GetLang("取消")
						};
						string lang3 = Global.GetLang("接收传承装备上的聚魂将会被覆盖，确定要传承吗？");
						Super.ShowMessageBoxEx(Global.GetLang("提示"), lang3, delegate(object s2, DPSelectedItemEventArgs e2)
						{
							if (e2.ID == 0)
							{
								this.SendJuHunChuanChengToServer();
							}
						}, buttons3);
					}
					else
					{
						this.SendJuHunChuanChengToServer();
					}
				}
			}, buttons);
			return;
		}
		if (goodsData.JuHunID > 0)
		{
			string[] buttons2 = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang2 = Global.GetLang("接收传承装备上的聚魂将会被覆盖，确定要传承吗？");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang2, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.SendJuHunChuanChengToServer();
				}
			}, buttons2);
		}
		else
		{
			this.SendJuHunChuanChengToServer();
		}
	}

	private void SendJuHunChuanChengToServer()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendJuHunChuanChengToServer((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, this.RadioIndex + 1);
	}

	public void JuHunCompletedCallBack(int result, GoodsData leftGoodsData, GoodsData rightGoodsData)
	{
		bool flag = false;
		switch (result + 9)
		{
		case 0:
			Super.HintMainText(Global.GetLang("剥离装备聚魂等级必须大于传承装备的聚魂等级"), 10, 3);
			break;
		default:
			switch (result + 203)
			{
			case 0:
				Super.HintMainText(Global.GetLang("装备阶数不足"), 10, 3);
				break;
			case 1:
				Super.HintMainText(Global.GetLang("两件物品对应主职业匹配"), 10, 3);
				break;
			case 2:
				Super.HintMainText(Global.GetLang("装备类型不匹配"), 10, 3);
				break;
			case 3:
				Super.HintMainText(Global.GetLang("道具类别不能被聚魂"), 10, 3);
				break;
			default:
				switch (result + 22)
				{
				case 0:
					Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
					break;
				case 1:
					Super.HintMainText(Global.GetLang("金币不足"), 10, 3);
					break;
				case 2:
					Super.HintMainText(Global.GetLang("消耗类型不正确"), 10, 3);
					break;
				default:
					if (result != -501)
					{
						if (result != -500)
						{
							if (result != -110)
							{
								Super.HintMainText(Global.GetLang("有未处理的错误消息result：") + result, 10, 3);
							}
							else
							{
								Super.HintMainText(Global.GetLang("功能未开启"), 10, 3);
							}
						}
						else
						{
							Super.HintMainText(Global.GetLang("装备聚魂等级不在配置表中"), 10, 3);
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("概率配置出错"), 10, 3);
					}
					break;
				}
				break;
			}
			break;
		case 4:
			Super.HintMainText(Global.GetLang("装备不在背包中"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("没有找到传承接受者的装备"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("没有找到传承提供者的装备"), 10, 3);
			break;
		case 9:
			Super.HintMainText("传承失败", 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("传承成功"), 10, 3);
			flag = true;
			break;
		}
		this.InitAllValue();
		this.AddEquipGoodsIcon(leftGoodsData, 0, false, 0);
		this.AddEquipGoodsIcon(rightGoodsData, 1, false, 0);
		if (flag)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
			if (this.DPEffectItem != null)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 1
				});
			}
			this.IsSucess = true;
		}
	}

	private void StartChuancheng3Exe(List<int> stateList)
	{
		string[] buttons = new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		};
		string[] array = new string[]
		{
			Global.GetLang("操作后新装备将被绑定，是否确认进行传承?"),
			Global.GetLang("{00FF00}剥离装备{-}的培养属性超过了{00FF00}继承装备{-}培养属性的最大上限，只能继承至最大上限，是否确认进行传承?")
		};
		if (stateList.Count > 0)
		{
			Super.ShowMessageBoxEx(Global.GetLang("提示"), array[stateList[0]], delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					stateList.RemoveAt(0);
					this.StartChuancheng3Exe(stateList);
				}
			}, buttons);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteEquipWashInheritCmd((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, this.RadioIndex + 1);
	}

	public void OnEquipInheritCompleted3(int result, GoodsData leftGoodsData, GoodsData rightGoodsData)
	{
		this.CloseModalDialog();
		string goodsNameByID = Global.GetGoodsNameByID(leftGoodsData.GoodsID, false);
		string goodsNameByID2 = Global.GetGoodsNameByID(rightGoodsData.GoodsID, false);
		if (result < 0)
		{
			if (result == -9)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			}
			else if (result == -10)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			if (this.DPEffectItem != null)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = -1
				});
			}
		}
		else if (result == 1 || result == 2)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，传承成功"), new object[0]), 0, -1, -1, 0);
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
		}
		this.InitAllValue();
		this.AddEquipGoodsIcon(leftGoodsData, 0, false, 0);
		this.AddEquipGoodsIcon(rightGoodsData, 1, false, 0);
		if (result == 1)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
			if (this.DPEffectItem != null)
			{
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 1
				});
			}
			this.IsSucess = true;
		}
	}

	private void StartFuMoChuanCheng()
	{
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		bool flag = goodsData.Binding == 1;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		bool flag2 = goodsData.Binding == 1;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("传承装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.RadioIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < this.NeedMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ZhuangBeiChuanCheng", this.NeedMoney, false))
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			return;
		}
		if (flag && !flag2)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang = Global.GetLang("接收传承装备将会被绑定，确定要传承吗？");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					if (goodsData.JuHunID > 0)
					{
						string[] buttons3 = new string[]
						{
							Global.GetLang("确定"),
							Global.GetLang("取消")
						};
						string lang3 = Global.GetLang("接收传承装备上的附魔将会被覆盖，确定要传承吗？");
						Super.ShowMessageBoxEx(Global.GetLang("提示"), lang3, delegate(object s2, DPSelectedItemEventArgs e2)
						{
							if (e2.ID == 0)
							{
								this.SendFuMoChuanChengToServer();
							}
						}, buttons3);
					}
					else
					{
						this.SendFuMoChuanChengToServer();
					}
				}
			}, buttons);
			return;
		}
		if (goodsData.JuHunID > 0)
		{
			string[] buttons2 = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang2 = Global.GetLang("接收传承装备上的附魔将会被覆盖，确定要传承吗？");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang2, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.SendFuMoChuanChengToServer();
				}
			}, buttons2);
		}
		else
		{
			this.SendFuMoChuanChengToServer();
		}
	}

	private void SendFuMoChuanChengToServer()
	{
		Super.ShowNetWaiting(null);
		if (this.RadioTongqian.Check && !this.RadioYuanbao.Check)
		{
			GameInstance.Game.SenFuMoZhuangBeiChuanCheng((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, 1);
		}
		else if (!this.RadioTongqian.Check && this.RadioYuanbao.Check)
		{
			GameInstance.Game.SenFuMoZhuangBeiChuanCheng((U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData).Id, (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData).Id, 2);
		}
	}

	public void FuMoCompletedCallBack(int result)
	{
		if (result != 1)
		{
			Super.HintMainText(FuMoMailOptEnum.ErrorFuMo(result), 10, 3);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		GoodsData goodsData2 = U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData;
		List<int> elementhrtsProps = goodsData.ElementhrtsProps;
		if (Global.Data.roleData.GoodsDataList != null)
		{
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Id == goodsData.Id)
				{
					Global.Data.roleData.GoodsDataList[i].ElementhrtsProps = null;
				}
				else if (Global.Data.roleData.GoodsDataList[i].Id == goodsData2.Id)
				{
					Global.Data.roleData.GoodsDataList[i].ElementhrtsProps = elementhrtsProps;
					if (goodsData.Binding == 1)
					{
						Global.Data.roleData.GoodsDataList[i].Binding = 1;
					}
				}
			}
		}
		this.InitAllValue();
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		}
		if (this.DPEffectItem != null)
		{
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1
			});
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

	private GTextBlockOutLine DiaojilvText;

	private ListBox FuZhuangBei = new ListBox();

	private ListBox ZhuZhuangBei = new ListBox();

	private ListBox Rock = new ListBox();

	private int NeedShengYouGoodsID = -1;

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public GCheckBox RadioQianghuaChuancheng;

	public GCheckBox RadioZhuijiaChuancheng;

	public GCheckBox RadioXilianChuancheng;

	public GCheckBox RadioJuHunChuancheng;

	public GCheckBox RadioFuMoChuancheng;

	public GCheckBox RadioTongqian;

	public GCheckBox RadioYuanbao;

	public TextBlock TongqianText;

	public TextBlock YuanbaoText;

	public TextBlock ChenggonglvText;

	public SpriteSL EquipSub;

	public SpriteSL EquipAdd;

	public GButton ClearSubIcon;

	public GButton ClearAddIcon;

	public UISprite spXiaoHao;

	public UISprite spZuanShi;

	private int RadioModeIndex = 1;

	private int RadioIndex = 1;

	private int NeedMoney;

	private bool IsSucess;

	private GChildWindow messageBoxWindow;

	private GCheckBox[] _radioIcon;

	private GCheckBox[] _radioIconMode;

	private GButton[] _clearIcon;

	private SpriteSL[] _equipIcon;

	private bool _ChuanChengZhong;
}
