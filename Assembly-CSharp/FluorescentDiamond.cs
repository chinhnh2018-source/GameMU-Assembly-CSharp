using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FluorescentDiamond : UserControl
{
	private void InitTextInPrefabs()
	{
		this.diamondInlyBtn.Text = Global.GetLang("宝石镶嵌");
		this.diamondDigBtn.Text = Global.GetLang("宝石挖掘");
		this.soulCometStoneBtn.Text = Global.GetLang("魂石");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.diamondInlyBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TabbarSelectChanged(0);
		};
		this.diamondDigBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TabbarSelectChanged(1);
		};
		this.soulCometStoneBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.TabbarSelectChanged(2);
		};
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool active = GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.SoulCometStonePowder, ref num, ref num2, ref num3);
		this.soulCometStoneBtn.gameObject.SetActive(active);
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public void InitData(FluorescentOpenTypes type = FluorescentOpenTypes.DiamondInlay)
	{
		this.TabbarSelectChanged((int)type);
	}

	private new void OnDestroy()
	{
	}

	private void TabbarSelectChanged(int select_index)
	{
		if (this.selectIndex == select_index)
		{
			return;
		}
		if (Global.isSoulCometStoneGathering)
		{
			return;
		}
		this.selectIndex = select_index;
		switch (select_index)
		{
		case 0:
			this.SetTabbarState(this.diamondInlyBtn, true);
			this.SetTabbarState(this.diamondDigBtn, false);
			this.SetTabbarState(this.soulCometStoneBtn, false);
			this.iconType_left = 5;
			this.iconType_right = CurrentIconType.FluorescentPowder;
			if (null == this.diamondInlay)
			{
				this.diamondInlay = U3DUtils.NEW<DiamondInlay>();
				U3DUtils.AddChild(this.content.gameObject, this.diamondInlay.gameObject, true);
			}
			this.diamondInlay.gameObject.SetActive(true);
			if (null != this.diamondDig)
			{
				this.diamondDig.gameObject.SetActive(false);
			}
			if (null != this.soulCometStone)
			{
				this.soulCometStone.gameObject.SetActive(false);
			}
			this.RefreshCurrencyText_Left();
			this.RefreshCurrencyText_Right();
			break;
		case 1:
			this.SetTabbarState(this.diamondDigBtn, true);
			this.SetTabbarState(this.diamondInlyBtn, false);
			this.SetTabbarState(this.soulCometStoneBtn, false);
			this.iconType_left = ((!IConfigbase<ConfigXingYunXingShiYong>.Instance.XingYunXingKaiGuan("YingGuanShiChouQu")) ? 5 : 6);
			this.iconType_right = CurrentIconType.FluorescentPowder;
			if (null == this.diamondDig)
			{
				this.diamondDig = U3DUtils.NEW<DiamondDig>();
				this.diamondDig.callback = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = 0
					});
				};
				U3DUtils.AddChild(this.content.gameObject, this.diamondDig.gameObject, true);
				this.diamondDig.InitDiamondDig();
			}
			this.diamondDig.gameObject.SetActive(true);
			if (null != this.diamondInlay)
			{
				this.diamondInlay.gameObject.SetActive(false);
			}
			if (null != this.soulCometStone)
			{
				this.soulCometStone.gameObject.SetActive(false);
			}
			break;
		case 2:
			this.SetTabbarState(this.soulCometStoneBtn, true);
			this.SetTabbarState(this.diamondDigBtn, false);
			this.SetTabbarState(this.diamondInlyBtn, false);
			this.iconType_left = 5;
			this.iconType_right = CurrentIconType.SoulCometStonePowder;
			if (null == this.soulCometStone)
			{
				this.soulCometStone = U3DUtils.NEW<SoulCometStone>();
				this.soulCometStone.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 1500)
					{
						this.iconType_left = e.Type;
						this.SetCurrentIcon_Left(this.iconType_left);
						this.RefreshCurrencyText();
					}
				};
				U3DUtils.AddChild(this.content.gameObject, this.soulCometStone.gameObject, true);
				this.soulCometStone.InitSoulCometStone();
			}
			this.soulCometStone.gameObject.SetActive(true);
			if (null != this.diamondInlay)
			{
				this.diamondInlay.gameObject.SetActive(false);
			}
			if (null != this.diamondDig)
			{
				this.diamondDig.gameObject.SetActive(false);
			}
			break;
		}
		this.SetCurrencyIcon(this.iconType_left, this.iconType_right);
		this.RefreshCurrencyText();
	}

	public void RefreshCurrencyText()
	{
		this.RefreshCurrencyText_Left();
		this.RefreshCurrencyText_Right();
	}

	public void RefreshCurrencyText_Left()
	{
		if (null == this.currencyText_left)
		{
			return;
		}
		int num = 0;
		switch (this.iconType_left)
		{
		case 1:
			num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan);
			break;
		case 2:
			num = Global.Data.roleData.StarSoulValue;
			break;
		case 3:
			num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ChengJiu);
			break;
		case 4:
			num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang);
			break;
		case 5:
			num = Global.Data.roleData.UserMoney;
			break;
		case 6:
			num = Global.GetRoleOwnNumByMoneyType(163);
			break;
		}
		this.currencyText_left.Text = num.ToString();
	}

	private void RefreshCurrencyText_Right()
	{
		if (null == this.currencyText_right)
		{
			return;
		}
		int num = 0;
		CurrentIconType currentIconType = this.iconType_right;
		if (currentIconType != CurrentIconType.FluorescentPowder)
		{
			if (currentIconType == CurrentIconType.SoulCometStonePowder)
			{
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.LangHunFenMo);
			}
		}
		else
		{
			num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.FluorescentPoint);
		}
		this.currencyText_right.Text = num.ToString();
	}

	private void SetTabbarState(GButton btn, bool selected = true)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	private void SetCurrencyIcon(ESoulStoneExtCostType iconType_left = 5, CurrentIconType iconType_right = CurrentIconType.FluorescentPowder)
	{
		this.SetCurrentIcon_Left(iconType_left);
		this.SetCurrentIcon_Right(iconType_right);
	}

	private void SetCurrentIcon_Left(ESoulStoneExtCostType iconType = 5)
	{
		if (null == this.currencyIcon_left)
		{
			return;
		}
		string spriteName = string.Empty;
		switch (iconType)
		{
		case 1:
			spriteName = "mojing";
			break;
		case 2:
			spriteName = "xinghun";
			break;
		case 3:
			spriteName = "chengjiu";
			break;
		case 4:
			spriteName = "shengwang";
			break;
		case 5:
			spriteName = "diamond";
			break;
		case 6:
			spriteName = "xingyunzhixing";
			break;
		}
		this.currencyIcon_left.spriteName = spriteName;
	}

	private void SetCurrentIcon_Right(CurrentIconType iconType = CurrentIconType.FluorescentPowder)
	{
		if (null == this.currencyIcon_right)
		{
			return;
		}
		string spriteName = string.Empty;
		if (iconType != CurrentIconType.FluorescentPowder)
		{
			if (iconType == CurrentIconType.SoulCometStonePowder)
			{
				spriteName = "soulCometPowder";
			}
		}
		else
		{
			spriteName = "fluorescent_powder";
		}
		this.currencyIcon_right.spriteName = spriteName;
	}

	public GButton diamondInlyBtn;

	public GButton diamondDigBtn;

	public GButton soulCometStoneBtn;

	public GButton closeBtn;

	public GameObject content;

	public UISprite currencyIcon_left;

	public UISprite currencyIcon_right;

	public TextBlock currencyText_left;

	public TextBlock currencyText_right;

	public DPSelectedItemEventHandler DPSelectedItem;

	[HideInInspector]
	public DiamondInlay diamondInlay;

	[HideInInspector]
	public DiamondDig diamondDig;

	[HideInInspector]
	public SoulCometStone soulCometStone;

	private ESoulStoneExtCostType iconType_left = 5;

	private CurrentIconType iconType_right;

	private int selectIndex = -1;
}
