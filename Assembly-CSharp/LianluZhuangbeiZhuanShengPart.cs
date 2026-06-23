using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class LianluZhuangbeiZhuanShengPart : UserControl
{
	protected override void InitializeComponent()
	{
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
		this.TongqianText.Text = "0";
		this.ChenggonglvText.Text = "0%";
		this.EquipText.Text = string.Empty;
		this.QianghuaProgressBar.Level = 0;
		NGUITools.SetActive(this.QianghuaHintText.gameObject, true);
	}

	public void InitPartSize(int width, int height)
	{
		this.QianghuaProgressBar.ItemWidth = 18f;
		this.QianghuaProgressBar.MaxLevel = 10;
		this.SubmitBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
		};
	}

	public void InitPartData()
	{
		this.equipIcon = new SpriteSL[3];
		this.equipIcon[0] = this.EquipNow;
		this.equipIcon[1] = this.EquipMax;
		this.equipIcon[2] = this.Cailiao1;
	}

	public void AddEquip(GoodsData gd)
	{
		if (gd != null)
		{
			this.InitAllValue();
			this.AddEquipGoodsIcon(gd, 0, false, 0);
			this.EquipText.textColor = Global.GetColorByGoodsData(gd);
			if (gd.ChangeLifeLevForEquip >= 10)
			{
				return;
			}
			int num = Math.Min(gd.ChangeLifeLevForEquip + 1, 10);
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(gd.GoodsID, gd.Forge_level, num, gd.ExcellenceInfo, gd.Lucky, gd.Binding, gd.GCount, num, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			this.AddEquipGoodsIcon(dummyGoodsDataMu, 1, false, 0);
			this.QianghuaProgressBar.Level = gd.ChangeLifeLevForEquip;
			NGUITools.SetActive(this.QianghuaHintText.gameObject, false);
			this.AddGoodsIcon(Global.GetZhuanshengGoodsID(gd), 2, 1);
			int zhuanshengLevelNeedMoney = Global.GetZhuanshengLevelNeedMoney(gd);
			this.TongqianText.Text = zhuanshengLevelNeedMoney.ToString();
			object roleData = Global.Data.roleData;
			if (Global.GetRoleOwnNumByMoneyType(13) < zhuanshengLevelNeedMoney)
			{
				this.TongqianText.textColor = 16711680U;
			}
			else
			{
				this.TongqianText.textColor = 16777215U;
			}
			this.ChenggonglvText.Text = StringUtil.substitute("{0}%", new object[]
			{
				Global.GetZhuanshengChenggonglvs(gd)
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
			if (index == 0)
			{
				this.EquipText.textColor = Global.GetColorByGoodsData(gd);
				string text = (gd.ExcellenceInfo <= 0) ? string.Empty : Global.GetLang("卓越的");
				this.EquipText.Text = string.Format("{0}{1}", text, goodsXmlNodeByID.Title);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Zhuansheng);
			this.equipIcon[index].Add(icon);
		}
	}

	public void AddGoodsIcon(int goodsID, int index, int iNeedNub)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid_bak";
			GoodsData gd = Global.GetGoodsDataByID(goodsID);
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
			icon.ItemObject = gd;
			icon.BoxTypes = 5;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				gd = (icon.ItemObject as GoodsData);
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
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

	private void StartSubmit()
	{
		SystemHelpMgr.OnAction(UIObjIDs.LianLuZhuiJiaSubmit, HelpStateEvents.Clicked, -1);
		if (this.equipIcon[0].Length() <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请放入要转生的装备"), new object[0]), 0, -1, -1, 0);
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
		if (goodsData.ChangeLifeLevForEquip >= 10)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】已经到了最高级别"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
			return;
		}
		this.NeedMoney = Global.GetZhuanshengLevelNeedMoney(goodsData);
		if (Global.GetRoleOwnNumByMoneyType(13) < this.NeedMoney)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("魔晶数量不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		int itemCode = U3DUtils.AS<GGoodIcon>(this.equipIcon[2][0]).ItemCode;
		if (Global.GetGoodsDataByID(itemCode) == null)
		{
			if (Super.ShowGoodsGuide(itemCode, this.callback) == 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("缺少材料{0}，无法强化"), new object[]
				{
					Global.GetGoodsNameByID(itemCode, false)
				}), 19, -1, -1, itemCode);
			}
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteQueryEquipChangeLifeCmd(this.ForgeDbID, itemCode, -1);
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("恭喜你，转生成功"), new object[0]), 0, -1, -1, 0);
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
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("转生失败"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -2 || result == -3)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("装备不在身上或背包中"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -100)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("转生【{0}】时发生错误"), new object[]
			{
				goodsNameByID
			}), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("转生【{0}】时发生错误:{1}"), new object[]
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

	public DPSelectedItemEventHandler callback;

	public SpriteSL Body;

	public ShowNetImage Bak;

	public GButton SubmitBtn;

	public TextBlock EquipText;

	public TextBlock TongqianText;

	public TextBlock ChenggonglvText;

	public TextBlock QianghuaHintText;

	public GImgProgressBar QianghuaProgressBar;

	public SpriteSL EquipNow;

	public SpriteSL EquipMax;

	public SpriteSL Cailiao1;

	private int ForgeDbID = -1;

	private int NeedMoney = -1;

	private SpriteSL[] _equipIcon;
}
