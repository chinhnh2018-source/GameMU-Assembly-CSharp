using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LianluZhuangbeiZhuijiaPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.QianghuaHintText.Text = Global.GetLang("装备追加后会附加额外的属性");
		this.CheckBoxBind.Text = Global.GetLang("优先使用绑定材料");
		this.SubmitBtn.Text = Global.GetLang("追加");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.CheckBox.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			if (!this.CheckBox.isChecked)
			{
				Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("不使用神佑晶石，追加失败等级降1级，确定不使用神佑晶石？"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
			}
		};
		this.CheckBoxBind.CheckChanged = delegate(object sender, BaseEventArgs e)
		{
			if (this.equipIcon[0].Length() > 0)
			{
				GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
				this.AddGoodsIcon(Global.GetZhuiJiaGoodsID(goodsData), 2, Global.GetZhuiJiaGoodsIDNums(goodsData));
				this.AddGoodsIcon(Global.GetZhuiJiaForgeLuckyGoodsIDs(), 3, 1);
			}
		};
		if (this.callback != null)
		{
			this.callback(this, new DPSelectedItemEventArgs
			{
				ID = -1
			});
		}
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			this.CheckBox.isChecked = false;
		}
		else if (args.ID == 1)
		{
			this.CheckBox.isChecked = true;
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

	public void InitAllValue()
	{
		for (int i = 0; i < this.equipIcon.Length; i++)
		{
			this.equipIcon[i].Clear();
		}
		this.ZhandouliText.Text = string.Empty;
		this.TongqianText.Text = "0";
		this.ChenggonglvText.Text = "0%";
		this.EquipText.Text = string.Empty;
		this.QianghuaProgressBar.Percent = 0.0;
		this.QianghuaProgressBar.uiLabel.text = string.Empty;
		NGUITools.SetActive(this.CheckBox.gameObject, false);
		NGUITools.SetActive(this.CheckBoxBind.gameObject, false);
		NGUITools.SetActive(this.QianghuaHintText.gameObject, true);
	}

	public void InitPartSize(int width, int height)
	{
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
		};
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[4];
		this.equipIcon[0] = this.EquipNow;
		this.equipIcon[1] = this.EquipMax;
		this.equipIcon[2] = this.Cailiao1;
		this.equipIcon[3] = this.Cailiao2;
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			this.EquipText.textColor = Global.GetColorByGoodsData(gd);
			if (gd.AppendPropLev >= 80)
			{
				return;
			}
			int maxZhuijiaLevelByGoodsData = Global.GetMaxZhuijiaLevelByGoodsData(gd);
			int zhuijiaLevel = Math.Min(gd.AppendPropLev + 1, maxZhuijiaLevelByGoodsData);
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(gd.GoodsID, gd.Forge_level, zhuijiaLevel, gd.ExcellenceInfo, gd.Lucky, gd.Binding, gd.GCount, 0, gd.WashProps, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			this.AddEquipGoodsIcon(dummyGoodsDataMu, 1, false, 0);
			this.QianghuaProgressBar.Percent = (double)((float)gd.AppendPropLev / (float)maxZhuijiaLevelByGoodsData);
			this.QianghuaProgressBar.uiLabel.text = string.Format("{0}/{1}", gd.AppendPropLev, maxZhuijiaLevelByGoodsData);
			NGUITools.SetActive(this.QianghuaHintText.gameObject, false);
			this.AddGoodsIcon(Global.GetZhuiJiaGoodsID(gd), 2, Global.GetZhuiJiaGoodsIDNums(gd));
			this.ZhandouliText.Text = string.Format(Global.GetLang("战斗力 +{0}"), Global.GetGoodsDataZhanLi(dummyGoodsDataMu) - Global.GetGoodsDataZhanLi(gd));
			int num = Global.GetZhuijiaForgeLevelNeedMoney(gd);
			num = Global.RecalcNeedYinLiang(num);
			this.TongqianText.Text = num.ToString();
			object roleData = Global.Data.roleData;
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < num)
			{
				this.TongqianText.textColor = 16711680U;
			}
			else
			{
				this.TongqianText.textColor = 16777215U;
			}
			this.ChenggonglvText.Text = string.Format("{0}%{{00ff00}} +{1}{{-}}", Global.GetZhuijiaChenggonglvs(gd), Global.GetSystemParamVipLeveValue("VIPZhuiJiaAdd"));
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
				NGUITools.SetActive(this.CheckBoxBind.gameObject, true);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Zhuijia);
			this.equipIcon[index].Add(icon);
		}
	}

	private void GetEquipInfo(out int suitID, out int zhuijiaLevel)
	{
		suitID = 0;
		zhuijiaLevel = 0;
		if (this.equipIcon[0].Length() <= 0)
		{
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		suitID = Global.GetEquipGoodsSuitID(goodsData.GoodsID);
		zhuijiaLevel = goodsData.AppendPropLev;
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
			int num2 = 0;
			this.GetEquipInfo(out num, out num2);
			int num3 = 0;
			bool flag = false;
			num3 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "EquipSuit", ref flag, (long)num);
			num3 += ConfigReplaceGoodVO.GetReplaceGoodCount(goodsID, "ZhuiJiaLevel", ref flag, (long)num2);
			int num4 = Global.GetTotalGoodsCountByID(goodsID);
			num4 += num3;
			icon.Text = string.Format("{0}/{1}", num4, iNeedNub);
			if (num4 >= iNeedNub)
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

	private void StartSubmit()
	{
		int num = 0;
		SystemHelpMgr.OnAction(UIObjIDs.LianLuZhuiJiaSubmit, HelpStateEvents.Clicked, -1);
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入要追加的装备"), new object[0]), 0, -1, -1, 0);
			return;
		}
		GoodsData goodsData = U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData;
		goodsData = Global.GetGoodsDataByDbID(goodsData.Id, null);
		this.ForgeDbID = goodsData.Id;
		if (goodsData == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int binding = goodsData.Binding;
		string goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
		int maxZhuijiaLevelByGoodsData = Global.GetMaxZhuijiaLevelByGoodsData(goodsData);
		if (goodsData.AppendPropLev >= maxZhuijiaLevelByGoodsData)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
			return;
		}
		this.NeedMoney = Global.GetZhuijiaForgeLevelNeedMoney(goodsData);
		this.NeedMoney = Global.RecalcNeedYinLiang(this.NeedMoney);
		if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, this.callback, string.Empty, string.Empty);
			return;
		}
		GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.equipIcon[2][0]);
		goodsData = (ggoodIcon.ItemObject as GoodsData);
		int itemNum = ggoodIcon.ItemNum;
		int itemCode = ggoodIcon.ItemCode;
		int num2 = Global.GetTotalGoodsCountByID(itemCode);
		int num3 = 0;
		int num4 = 0;
		this.GetEquipInfo(out num3, out num4);
		int num5 = 0;
		bool flag = false;
		num5 += ConfigReplaceGoodVO.GetReplaceGoodCount(itemCode, "EquipSuit", ref flag, (long)num3);
		if (flag)
		{
			num = 1;
		}
		num5 += ConfigReplaceGoodVO.GetReplaceGoodCount(itemCode, "ZhuiJiaLevel", ref flag, (long)num4);
		if (flag)
		{
			num = 1;
		}
		num2 += num5;
		if (num2 < itemNum)
		{
			if (Super.ShowGoodsGuide(itemCode, this.callback) == 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料{0}，无法追加"), new object[]
				{
					Global.GetGoodsNameByID(itemCode, false)
				}), 19, -1, -1, itemCode);
			}
			return;
		}
		if (goodsData != null && goodsData.Binding == 1)
		{
			num = goodsData.Binding;
		}
		this.FuGoodsID = -1;
		if (binding == 0 && num == 1)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			string lang = Global.GetLang("存在绑定的材料，操作后您的装备将变为绑定，确认要执行该操作吗?");
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.ShowModalDialog();
					GameInstance.Game.SpriteEquipAppendPropCmd(this.ForgeDbID, this.FuGoodsID, (!this.CheckBoxBind.Check) ? 0 : 1);
				}
			}, buttons);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteEquipAppendPropCmd(this.ForgeDbID, this.FuGoodsID, (!this.CheckBoxBind.Check) ? 0 : 1);
	}

	public void NotifyZhuijiaResult(int result, int dbID, int forgeLevel)
	{
		this.CloseModalDialog();
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(dbID, null);
		string goodsNameByID = Global.GetGoodsNameByID(goodsDataByDbID.GoodsID, false);
		if (result < 1)
		{
			if (result != 0)
			{
				if (result == -9998)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
				}
				else if (result == -7)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("材料不足"), new object[]
					{
						goodsNameByID
					}), 0, -1, -1, 0);
				}
				else if (result == -100)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("追加【{0}】时发生错误"), new object[]
					{
						goodsNameByID
					}), 0, -1, -1, 0);
				}
				else
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("追加【{0}】时发生错误:{1}"), new object[]
					{
						goodsNameByID,
						result
					}), 0, -1, -1, 0);
				}
			}
			Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = -1
			});
		}
		else
		{
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1,
				PlayID = goodsDataByDbID.AppendPropLev
			});
		}
		this.AddEquip(goodsDataByDbID);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 1
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

	public DPSelectedItemEventHandler DPSelectedItem;

	public LianluEffectEventHandler DPEffectItem;

	public DPSelectedItemEventHandler callback;

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public TextBlock EquipText;

	public TextBlock TongqianText;

	public TextBlock ChenggonglvText;

	public TextBlock QianghuaHintText;

	public TextBlock ZhandouliText;

	public GImgProgressBar QianghuaProgressBar;

	public GCheckBox CheckBox;

	public GCheckBox CheckBoxBind;

	public SpriteSL EquipNow;

	public SpriteSL EquipMax;

	public SpriteSL Cailiao1;

	public SpriteSL Cailiao2;

	private int ForgeDbID = -1;

	private int NeedMoney = -1;

	private int FuGoodsID = -1;

	private GButton[] _clearIcon;

	private SpriteSL[] _equipIcon;
}
