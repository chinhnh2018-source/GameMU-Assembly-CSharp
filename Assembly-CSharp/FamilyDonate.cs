using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class FamilyDonate : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnMoneyDonate1.Text = Global.GetLang("捐赠1次");
		this.btnMoneyDonate10.Text = Global.GetLang("捐赠10次");
		this.btnDiamondDonate1.Text = Global.GetLang("捐赠");
		this.btnDiamondDonate10.Text = Global.GetLang("捐赠10次");
		this.lblGoldZhangongLimit.transform.localPosition = new Vector3(-114f, -185f, -1f);
		this.lblPromptDiamond.transform.localPosition = new Vector3(-268f, -219f, -1f);
		this.lblPromptGold.transform.localPosition = new Vector3(-275f, -185f, -1f);
		this.lblDiamondZhangongLimit.transform.localPosition = new Vector3(-114f, -219f, -1f);
		this.lblLeagueMoney.transform.localPosition = new Vector3(-100f, 177f, -1f);
		this.lblSelfZhanGong.transform.localPosition = new Vector3(-345f, 177f, -1f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnMoneyDonate1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.SendEvent("705", Global.GetLang("金币捐赠1次次数"));
			GameInstance.Game.SpriteDonateBGMoney(1, 1);
			this.lastDonateType = 1;
		};
		this.btnMoneyDonate10.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.SendEvent("706", Global.GetLang("金币捐赠10次次数"));
			GameInstance.Game.SpriteDonateBGMoney(1, 10);
			this.lastDonateType = 1;
		};
		this.btnDiamondDonate1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (string.IsNullOrEmpty(this.selectedItemId))
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("请选择一个物品再进行捐赠!"), -1, -1, -1, -1, false);
			}
			else
			{
				string[] juanZengGoodsIds = Global.JuanZengGoodsIds;
				int goods1Num = (!(juanZengGoodsIds[0] == this.selectedItemId)) ? 0 : 1;
				int goods2Num = (!(juanZengGoodsIds[1] == this.selectedItemId)) ? 0 : 1;
				int goods3Num = (!(juanZengGoodsIds[2] == this.selectedItemId)) ? 0 : 1;
				int goods4Num = (!(juanZengGoodsIds[3] == this.selectedItemId)) ? 0 : 1;
				GameInstance.Game.SpriteDonateBGGoods(goods1Num, goods2Num, goods3Num, goods4Num, 0);
				this.lastDonateType = 2;
				Global.SendEvent("707", Global.GetLang("物品捐赠1次次数"));
			}
		};
		this.btnDiamondDonate10.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.SendEvent("708", Global.GetLang("钻石捐赠10次次数"));
			GameInstance.Game.SpriteDonateBGMoney(2, 10);
			this.lastDonateType = 2;
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				Super._ParcelPart.transform.parent = null;
				Object.Destroy(Super._ParcelPart.gameObject);
				Super._ParcelPart = null;
				this.selectedItemId = string.Empty;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = -10
				});
			}
		};
		if (Super._ParcelPart == null)
		{
			Super._ParcelPart = U3DUtils.NEW<ParcelPart>();
		}
		Super._ParcelPart.iBaoGuoMode = 0;
		Super._ParcelPart.InitPartData();
		Super._ParcelPart.transform.parent = base.transform;
		Super._ParcelPart.transform.localPosition = new Vector3(260f, -28f, -0.1f);
		Super._ParcelPart.transform.localScale = new Vector3(1f, 1f, 1f);
		Super._ParcelPart.Visibility = true;
		Super._ParcelPart.ChuShouBtn.isEnabled = false;
		this.lblSelfZhanGong.text = string.Empty + Global.Data.roleData.BangGong;
		this.lblTodayZhangongMLimit.text = "/" + Global.JuanZengJinBiHuoDeZhanGongShangXian;
		this.lblTodayZhangongDLimit.text = "/" + Global.JuanZengZuanShiHuoDeZhanGongShangXian;
		this.LoadSysParams();
		this.InitGoods();
	}

	private void LoadSysParams()
	{
		this.lblPromptGold.text = "{00ff00}" + Global.JuanZengJinBiHuoDeZhanGong + "{-}";
		this.lblPromptDiamond.text = "{00ff00}" + Global.JuanZengGoodsHuoDeZhanGong + "{-}";
		this.lblPromptGoldPerTime.text = "{00ff00}" + Global.JuanZengJinBiShuLiang + "{-}";
		this.lblPromptDiamodPerTime.text = "{00ff00}" + Global.JuanZengZuanShiShuLiang + "{-}";
		this.lblDiamondZhangongLimit.text = "{00ff00}" + Global.JuanZengGoodsHuoDeZhanGongShangXian + "{-}";
		this.lblGoldZhangongLimit.text = "{00ff00}" + Global.JuanZengJinBiHuoDeZhanGongShangXian + "{-}";
	}

	private void InitGoods()
	{
		string[] juanZengGoodsIds = Global.JuanZengGoodsIds;
		string text = string.Empty;
		if (juanZengGoodsIds != null)
		{
			for (int i = 0; i < juanZengGoodsIds.Length; i++)
			{
				GameObject gameObject = this.goodsGroup.transform.FindChild(string.Empty + i).gameObject;
				text = juanZengGoodsIds[i] + ",0,0,0,0,0,0";
				this.initGood(gameObject, text.Split(new char[]
				{
					','
				}), i);
			}
		}
	}

	private void initGood(GameObject parent, string[] goods, int idx)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		UISprite border = null;
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(num);
			ggoodIcon.Text = totalGoodsCountByID.ToString();
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
			if (totalGoodsCountByID > 0)
			{
				ggoodIcon.EnableIcon = true;
				ggoodIcon.TextColor = 16777215U;
			}
			else
			{
				ggoodIcon.EnableIcon = false;
				ggoodIcon.TextColor = 8421504U;
			}
			Vector3 localPosition = ggoodIcon.ContentText.transform.localPosition;
			localPosition.z = -0.2f;
			ggoodIcon.ContentText.transform.localPosition = localPosition;
			U3DUtils.AddChild(parent, ggoodIcon.gameObject, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.addEventListener("click", delegate(MouseEvent e)
			{
				border = parent.transform.FindChild("border").GetComponent<UISprite>();
				if (this.preSelected != border)
				{
					if (this.preSelected != null)
					{
						this.preSelected.gameObject.SetActive(false);
					}
					border.gameObject.SetActive(true);
					this.preSelected = border;
					GGoodIcon ggoodIcon2 = e.target.SafeGetComponent<GGoodIcon>();
					if (null == ggoodIcon2)
					{
						return;
					}
					GoodsData goodsData2 = ggoodIcon2.ItemObject as GoodsData;
					if (goodsData2 == null)
					{
						return;
					}
					this.SelectedGoodsIcon = ggoodIcon2;
					this.selectedItemId = string.Empty + goodsData2.GoodsID;
				}
			});
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			component.center = new Vector3(0f, 0f, -1f);
			Transform transform = parent.transform.FindChild("name");
			transform.transform.localScale = new Vector3(18f, 18f, 1f);
			UILabel component2 = transform.GetComponent<UILabel>();
			component2.text = goodsXmlNodeByID.Title;
		}
	}

	public void GetLeagueDonateData()
	{
	}

	private void RefreshSelectedGoodsIcon()
	{
		if (null == this.SelectedGoodsIcon)
		{
			return;
		}
		GoodsData goodsData = this.SelectedGoodsIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsData.GoodsID);
		this.SelectedGoodsIcon.Text = totalGoodsCountByID.ToString();
		if (totalGoodsCountByID > 0)
		{
			this.SelectedGoodsIcon.EnableIcon = true;
			this.SelectedGoodsIcon.TextColor = 16777215U;
		}
		else
		{
			this.SelectedGoodsIcon.EnableIcon = false;
			this.SelectedGoodsIcon.TextColor = 8421504U;
		}
	}

	public void RefreshDonateData(int retCode, int roleID, int bhid, int bhExtraMoney, int personZhangong)
	{
		if (retCode < 0)
		{
			if (retCode == -1010)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("战盟已经不存在"), new object[0]), 10, 3);
			}
			else if (retCode == -10)
			{
				Super.HintMainText(Global.GetLang("您的金币不足，无法进行战盟捐赠！"), 10, 3);
			}
			else if (retCode == -101)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
			else if (retCode == -30)
			{
				Super.HintMainText(Global.GetLang("您的道具不足，无法进行战盟捐赠！"), 10, 3);
			}
			else
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("贡献的绑定金币到战盟库存时发生错误: {0}"), new object[]
				{
					retCode
				}), 10, 3);
			}
			return;
		}
		this.RefreshSelectedGoodsIcon();
		this.lblLeagueMoney.text = string.Empty + (int.Parse(this.lblLeagueMoney.text) + bhExtraMoney);
		string text = Global.GetLang("战盟资金增加") + bhExtraMoney + "!";
		this.animForZijin.gameObject.SetActive(true);
		this.PlayStart(this.animForZijin, new ActiveAnimation.OnFinished(this.PlayFinished));
		if (this.lblSelfZhanGong.text != string.Empty + Global.Data.roleData.BangGong)
		{
			text = string.Concat(new object[]
			{
				Global.GetLang("个人战功增加"),
				Global.Data.roleData.BangGong - int.Parse(this.lblSelfZhanGong.text),
				", ",
				text
			});
			this.animForZhangong.gameObject.SetActive(true);
			this.PlayStart(this.animForZhangong, new ActiveAnimation.OnFinished(this.PlayFinished));
		}
		this.lblSelfZhanGong.text = string.Empty + Global.Data.roleData.BangGong;
		Super.HintMainText(text, 10, 3);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				Type = 1,
				ID = int.Parse(this.lblLeagueMoney.text)
			});
			if (this.lastDonateType == 1)
			{
				this.lblTodayZhangongM.text = string.Empty + (int.Parse(this.lblTodayZhangongM.text) + personZhangong);
				this.barForGold.Percent = (double)int.Parse(this.lblTodayZhangongM.text) / (double)Global.JuanZengJinBiHuoDeZhanGongShangXian;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = 2,
					ID = int.Parse(this.lblTodayZhangongM.text)
				});
			}
			else if (this.lastDonateType == 2)
			{
				this.lblTodayZhangongD.text = string.Empty + (int.Parse(this.lblTodayZhangongD.text) + personZhangong);
				this.barForDiamond.Percent = (double)int.Parse(this.lblTodayZhangongD.text) / (double)Global.JuanZengZuanShiHuoDeZhanGongShangXian;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = 3,
					ID = int.Parse(this.lblTodayZhangongD.text)
				});
			}
		}
	}

	public void SetDonateInfo(string money, int todayZhangongForGold, int todayZhangongForDiamond)
	{
		this.lblLeagueMoney.text = money;
		this.lblTodayZhangongM.text = string.Empty + todayZhangongForGold;
		this.barForGold.Percent = (double)((float)todayZhangongForGold / (float)Global.JuanZengJinBiHuoDeZhanGongShangXian);
		this.lblTodayZhangongD.text = string.Empty + todayZhangongForDiamond;
		this.barForDiamond.Percent = (double)((float)todayZhangongForDiamond / (float)Global.JuanZengZuanShiHuoDeZhanGongShangXian);
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

	public void NotifyDonateGoodsResult(int retCode, int roleID, int bhid)
	{
		if (retCode < 0)
		{
			if (retCode == -1010)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟已经不存在"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("贡献的道具到战盟库存时发生错误: {0}"), new object[]
				{
					retCode
				}), 0, -1, -1, 0);
			}
			return;
		}
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("贡献道具【{0}】战盟库存成功"), new object[]
		{
			Global.Data.roleData.BHName
		}), 0, -1, -1, 0);
	}

	public GButton btnMoneyDonate1;

	public GButton btnMoneyDonate10;

	public GButton btnDiamondDonate1;

	public GButton btnDiamondDonate10;

	public GButton btnClose;

	public UILabel lblSelfZhanGong;

	public UILabel lblLeagueMoney;

	public UILabel lblPromptGold;

	public UILabel lblPromptDiamond;

	public UILabel lblTodayZhangongM;

	public UILabel lblTodayZhangongD;

	public UILabel lblTodayZhangongMLimit;

	public UILabel lblTodayZhangongDLimit;

	public UILabel lblPromptGoldPerTime;

	public UILabel lblPromptDiamodPerTime;

	public UILabel lblDiamondZhangongLimit;

	public UILabel lblGoldZhangongLimit;

	public GImgProgressBar barForGold;

	public GImgProgressBar barForDiamond;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int lastDonateType = -1;

	public Animation animForZhangong;

	public Animation animForZijin;

	public GameObject goodsGroup;

	private UISprite preSelected;

	private string selectedItemId = string.Empty;

	private GGoodIcon SelectedGoodsIcon;
}
