using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LianluZhuijiaChuanchenPart : UserControl
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

	public void InitPartSize(int width, int height)
	{
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartChuancheng();
		};
		this.ClearSubIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ClearEquip(0);
			if (this.equipIcon[1].Length() > 0)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					FilterType = 1,
					ZhuZhuangBei = null,
					FuZhuangBei = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData)
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
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
					ID = 1,
					FilterType = 1,
					ZhuZhuangBei = (U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData),
					FuZhuangBei = null
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					FilterType = 1,
					ZhuZhuangBei = null,
					FuZhuangBei = null
				});
			}
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 0,
				PlayID = 1
			});
		};
		this.RadioTongqian.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectRadio(0);
		};
		this.RadioYuanbao.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.SelectRadio(1);
		};
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

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			if (this.equipIcon[0].Length() > 0 && this.equipIcon[1].Length() > 0)
			{
				return;
			}
			if (this.equipIcon[0].Length() <= 0 && this.equipIcon[1].Length() <= 0)
			{
				this.AddEquipGoodsIcon(gd, 0, false, 0);
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					FilterType = 1,
					ZhuZhuangBei = gd,
					FuZhuangBei = null
				});
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 0,
					PlayID = 0
				});
				return;
			}
			if (this.equipIcon[0].Length() <= 0 && this.equipIcon[1].Length() > 0)
			{
				this.AddEquipGoodsIcon(gd, 0, false, 0);
				this.DPEffectItem(this, new NotifyLianluEffectEventArgs
				{
					EffectID = 0,
					PlayID = 0
				});
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
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			this.equipIcon[index].Add(icon);
			NGUITools.SetActive(this.clearIcon[index].gameObject, true);
			if (index == 0)
			{
				this.SetMoney(gd);
				this.ChenggonglvText.Text = Global.GetZhuijiaChuanchengDiaojilv(gd.Forge_level).ToString() + "%";
			}
		}
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
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
			num = Global.GetZhuijiaChuanchengMoney(gd.Forge_level, 0);
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
			num = Global.GetZhuijiaChuanchengMoney(gd.Forge_level, 1);
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("旧装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.equipIcon[1].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("新装备不存在"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		int forge_level = goodsData.Forge_level;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("旧装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[1][0]).ItemObject as GoodsData);
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		int forge_level2 = goodsData.Forge_level;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("新装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
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
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < this.NeedMoney)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, this.callback, string.Empty, string.Empty);
			return;
		}
		if (forge_level2 > 0)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang = Global.GetLang("继承装备的强化等级传承后将会被覆盖，是否确定要执行传承?");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.ShowModalDialog();
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

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public GCheckBox RadioTongqian;

	public GCheckBox RadioYuanbao;

	public TextBlock TongqianText;

	public TextBlock YuanbaoText;

	public TextBlock ChenggonglvText;

	public SpriteSL EquipSub;

	public SpriteSL EquipAdd;

	public GButton ClearSubIcon;

	public GButton ClearAddIcon;

	private int RadioIndex = 1;

	private int NeedMoney;

	private GCheckBox[] _radioIcon;

	private GButton[] _clearIcon;

	private SpriteSL[] _equipIcon;

	private bool _ChuanChengZhong;
}
