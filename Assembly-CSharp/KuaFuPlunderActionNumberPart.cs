using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class KuaFuPlunderActionNumberPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.mBuyBtn.Text = Global.GetLang("购买");
			this.mTitlelabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("购买参与次数")
			});
			this.mInflabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("每轮自动补充至") + ConfigSystemParam.GetSystemParamByName("CrusadeEnterTime", true).Split(new char[]
				{
					','
				})[0].SafeToInt32(0) + Global.GetLang("次，超过不会获得")
			});
			this.mNamelabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("现有次数：")
			});
			this.mNamelabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("现有钻石：")
			});
			this.mNamelabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("购买消耗：")
			});
			this.mInflabel.pivot = 3;
			this.mInflabel.lineWidth = 360;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mBuyBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				int num = ConfigSystemParam.GetSystemParamByName("CrusadeEnterTime", true).Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
				if ((long)num <= Global.Data.roleData.MoneyData[134])
				{
					Super.HintMainText(Global.GetLang("参与次数已达到上限无需购买"), 10, 3);
				}
				else
				{
					int num2 = 0;
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CrusadeEnterPrice", ',');
					if (systemParamIntArrayByName != null && Global.Data.roleData.MoneyData[135] < (long)systemParamIntArrayByName.Length)
					{
						num2 = systemParamIntArrayByName[(int)(checked((IntPtr)Global.Data.roleData.MoneyData[135]))];
					}
					if (Global.Data.roleData.UserMoney >= num2)
					{
						GameInstance.Game.BuyKuaFuPlundeEnterNum(1);
					}
					else if (!this.IsInBattleMap)
					{
						Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
					}
					else
					{
						Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
					}
				}
			};
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						ID = 90,
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void NoticeRefreshEnterNumber()
	{
		this.RefreshInf();
	}

	public void InBattleMap(bool result)
	{
		this.IsInBattleMap = result;
	}

	public void RefreshInf()
	{
		int num = ConfigSystemParam.GetSystemParamByName("CrusadeEnterTime", true).Split(new char[]
		{
			','
		})[1].SafeToInt32(0);
		this.mValueLabel[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			((long)num > Global.Data.roleData.MoneyData[134]) ? "fdf7dd" : "ff0000",
			Global.Data.roleData.MoneyData[134] + "/" + num
		});
		int num2 = 0;
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CrusadeEnterPrice", ',');
		if (systemParamIntArrayByName != null)
		{
			if (Global.Data.roleData.MoneyData[135] < (long)systemParamIntArrayByName.Length)
			{
				num2 = systemParamIntArrayByName[(int)(checked((IntPtr)Global.Data.roleData.MoneyData[135]))];
			}
			else
			{
				num2 = systemParamIntArrayByName[systemParamIntArrayByName.Length - 1];
			}
		}
		this.mValueLabel[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			(Global.Data.roleData.UserMoney < num2) ? "ff0000" : "fdf7dd",
			Global.Data.roleData.UserMoney
		});
		this.mValueLabel[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			num2
		});
	}

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UILabel[] mNamelabel;

	[SerializeField]
	private UILabel[] mValueLabel;

	[SerializeField]
	private UILabel mTitlelabel;

	[SerializeField]
	private UILabel mInflabel;

	[SerializeField]
	private GButton mBuyBtn;

	private bool IsInBattleMap;

	public DPSelectedItemEventHandler Hander;
}
