using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LianluZhuanshengBoliPart : UserControl
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

	protected override void InitializeComponent()
	{
	}

	public void InitPartSize(int width, int height)
	{
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
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
		this.equipIcon[0] = this.EquipNow;
		this.equipIcon[1] = this.EquipMax;
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
	}

	private void ClearEquip(int index)
	{
		if (index >= 0 && index < 2)
		{
			this.equipIcon[index].Clear();
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
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			if (gd.AppendPropLev >= 80)
			{
				return;
			}
			int zhuijiaLevel = Math.Min(gd.AppendPropLev + 1, 80);
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(gd.GoodsID, gd.Forge_level, zhuijiaLevel, gd.ExcellenceInfo, gd.Lucky, gd.Binding, gd.GCount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			this.AddEquipGoodsIcon(dummyGoodsDataMu, 1, false, 0);
			this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Global.GetZhuanshengBoliChenggonglvs(gd.ChangeLifeLevForEquip)
			});
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
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Zhuansheng);
			this.equipIcon[index].Add(icon);
			if (index == 0)
			{
				this.SetMoney(gd);
			}
		}
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid_bak";
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
			icon.ItemObject = Global.GetGoodsDataByID(goodsID);
			icon.BoxTypes = 5;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GoodsData goodsData = icon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					goodsData = Global.GetDummyGoodsData(icon.ItemCode);
				}
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			};
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
			icon.Text = string.Format("{0}/{1}", totalGoodsCountByID, iNeedNub);
			if (totalGoodsCountByID >= iNeedNub)
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

	private void SetMoney(GoodsData gd)
	{
		int num = 0;
		this.TongqianText.textColor = 16777215U;
		this.YuanbaoText.textColor = 16777215U;
		this.TongqianText.Text = "0";
		this.YuanbaoText.Text = "0";
		if (this.RadioIndex == 0)
		{
			num = Global.GetZhuanshengBoliMoney(gd.ChangeLifeLevForEquip, 0);
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
			num = Global.GetZhuanshengBoliMoney(gd.ChangeLifeLevForEquip, 1);
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

	private void StartSubmit()
	{
		SystemHelpMgr.OnAction(UIObjIDs.LianLuZhuiJiaSubmit, HelpStateEvents.Clicked, -1);
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入要剥离转生的装备"), new object[0]), 0, -1, -1, 0);
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
		string goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
		if (goodsData.ChangeLifeLevForEquip <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】无法剥离"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
			return;
		}
		if (this.RadioIndex == 0)
		{
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.NeedMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				return;
			}
		}
		else if (this.RadioIndex == 1 && Global.Data.roleData.UserMoney < this.NeedMoney)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteQueryFlakeOffEquipChangeLifeCmd(this.ForgeDbID, this.RadioIndex + 1);
	}

	public void NotifyResult(int result, int dbID, int forgeLevel)
	{
		this.CloseModalDialog();
		GoodsData goodsData = null;
		string goodsNameByID;
		if (result >= 1)
		{
			Global.PlaySoundAudio("Audio/UI/hecheng_ok", false);
			this.DPEffectItem(this, new NotifyLianluEffectEventArgs
			{
				EffectID = 1,
				PlayID = 1
			});
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，剥离成功"), new object[0]), 0, -1, -1, 0);
			goodsData = Global.GetGoodsDataByDbID(dbID, null);
			goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
			this.AddEquip(goodsData);
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 1
				});
			}
			return;
		}
		if (this.equipIcon[0].Length() > 0)
		{
			goodsData = (U3DUtils.AS<GGoodIcon>(this.equipIcon[0][0]).ItemObject as GoodsData);
		}
		if (goodsData == null)
		{
			return;
		}
		goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
		if (result == -200)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离失败"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -2 || result == -3)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -100)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离【{0}】时发生错误"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剥离【{0}】时发生错误:{1}"), new object[]
			{
				goodsNameByID,
				result
			}), 0, -1, -1, 0);
		}
		Global.PlaySoundAudio("Audio/UI/hecheng_failed", false);
		this.AddEquip(goodsData);
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

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public GCheckBox RadioTongqian;

	public GCheckBox RadioYuanbao;

	public TextBlock TongqianText;

	public TextBlock YuanbaoText;

	public TextBlock ChenggonglvText;

	public SpriteSL EquipNow;

	public SpriteSL EquipMax;

	private int ForgeDbID = -1;

	private int RadioIndex = 1;

	private int NeedMoney;

	private GCheckBox[] _radioIcon;

	private SpriteSL[] _equipIcon;
}
