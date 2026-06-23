using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderBiddingPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mBiddingBtnCD = (int)ConfigSystemParam.GetSystemParamIntByName("CrusadeApplyCD");
		string @string = PlayerPrefs.GetString("strPlunderBiddingTimeTicks" + Global.Data.RoleID);
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		if (!long.TryParse(@string, ref this.mBiddingTimeTicks))
		{
			this.mBiddingTimeTicks = -1L;
		}
		if (0 >= Global.Data.roleData.Faction)
		{
			Super.HintMainText(Global.GetLang("请先加入或创建一个帮会"), 10, 3);
		}
		else
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SpriteQueryBangHuiDetail(Global.Data.roleData.Faction);
		}
		this.StartTimeTicks();
		if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderMap)
		{
			if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo != null)
			{
				this.RefreshBiddingData(KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo);
			}
			else
			{
				GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
			}
		}
	}

	protected override void OnDestroy()
	{
		this.StopTimeTicks();
		base.OnDestroy();
	}

	private void StopTimeTicks()
	{
		if (this.mDispatcherTimer != null)
		{
			this.mDispatcherTimer.Stop();
			this.mDispatcherTimer.Dispose();
			this.mDispatcherTimer = null;
		}
	}

	private void StartTimeTicks()
	{
		this.StopTimeTicks();
		this.mDispatcherTimer = null;
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlubderRankPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		if (0L < this.mBiddingTimeTicks)
		{
			this.RefreshBiddingBtnCD();
		}
	}

	private void RefreshBiddingBtnCD()
	{
		int num = (int)(Global.GetCorrectDateTime() - new DateTime(this.mBiddingTimeTicks)).TotalSeconds;
		if (0 < num)
		{
			if (this.mBiddingBtnCD > num)
			{
				this.ChangeBtnState(this.mSureBtn, false, (this.mBiddingBtnCD - num).ToString());
			}
			else
			{
				this.ChangeBtnState(this.mSureBtn, true, Global.GetLang("确定"));
			}
		}
		else
		{
			this.ChangeBtnState(this.mSureBtn, true, Global.GetLang("确定"));
		}
	}

	private void ChangeBtnState(GButton btn, bool isEnabled, string btnStr)
	{
		if (null != btn)
		{
			if (btnStr != null && null != btn.Label && !string.IsNullOrEmpty(btnStr))
			{
				btn.Label.text = btnStr;
			}
			btn.isEnabled = isEnabled;
			string text = string.Empty;
			string text2 = string.Empty;
			if (isEnabled)
			{
				text = "btn_green";
				text2 = "btn_green_selected";
			}
			else
			{
				text = "btn_green_disable";
				text2 = "btn_green_disable";
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				btn.normalSprite = text;
				btn.hoverSprite = text2;
				btn.pressedSprite = text2;
				btn.disabledSprite = text2;
			}
			btn.Refresh();
		}
	}

	private void InitPrefabText()
	{
		try
		{
			int num = 10;
			this.mUnit = (int)ConfigSystemParam.GetSystemParamIntByName("CrusadeMinApply");
			this.mTitleLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("竞价")
			});
			this.mInfLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("每次加价、竞价以"),
				"e3b36c",
				this.mUnit.ToString() + Global.GetLang("为单位")
			});
			this.mNamesLabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟资金：")
			});
			this.mNamesLabel[1].Margin = new Vector2((float)num, 0f);
			this.mNamesLabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("已竞价：")
			});
			this.mNamesLabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("当前状态：")
			});
			this.mNamesLabel[3].Margin = new Vector2((float)(num * 3), 0f);
			this.mNamesLabel[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("出价：")
			});
			this.mValuesLabel[0].text = string.Empty;
			this.mValuesLabel[1].text = string.Empty;
			this.mValuesLabel[2].text = string.Empty;
			this.mValuesLabel[3].text = string.Empty;
			this.mSureBtn.Label.text = Global.GetLang("确定");
			for (int i = 0; i < this.mNamesLabel.Length; i++)
			{
				this.mNamesLabel[i].pivot = 5;
				this.mNamesLabel[i].transform.localPosition = new Vector3(-10f, 0f, 0f);
			}
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

	private int GetBiddingMoney()
	{
		MUDebug.Log<string>(new string[]
		{
			this.mInputNum + " :: " + this.mUnit
		});
		return this.mInputNum * this.mUnit;
	}

	private void InitHandler()
	{
		try
		{
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this.mSureBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.CheckRoleBiddingMoney())
				{
					if ((long)Global.ZhanMengWeiHuXiaoHao <= this.mBHMoney - (long)this.GetBiddingMoney())
					{
						this.mBiddingTimeTicks = Global.GetCorrectDateTime().Ticks;
						Super.ShowNetWaiting(null);
						GameInstance.Game.SendGetKuaFuPlunderBiddingData(this.mSelectServerID, this.GetBiddingMoney());
						PlayerPrefs.SetString("strPlunderBiddingTimeTicks" + Global.Data.RoleID, Global.GetCorrectDateTime().Ticks.ToString());
						PlayerPrefs.SetInt(string.Concat(new object[]
						{
							Global.Data.RoleID,
							"_",
							this.mSelectServerID,
							"_LastPludderBiddingMoneyThis"
						}), this.GetBiddingMoney());
						if (this.Hander != null)
						{
							this.Hander(this, new DPSelectedItemEventArgs
							{
								ID = this.mInputNum * this.mUnit,
								Type = 0,
								MyID = this.mSelectServerID
							});
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("竞价后战盟资金将不能维持日常消耗"), 10, 3);
					}
				}
			};
			UIEventListener.Get(this.mAddSp.gameObject).onClick = delegate(GameObject g)
			{
				this.RefreshScollBar(1);
			};
			UIEventListener.Get(this.mSubSp.gameObject).onClick = delegate(GameObject g)
			{
				this.RefreshScollBar(-1);
			};
			this.mValueScrollBar.onChange = delegate(UIScrollBar sb)
			{
				this.RefreshNum(this.mValueScrollBar.scrollValue);
			};
			UIEventListener.Get(this.mInPutObj).onClick = delegate(GameObject g)
			{
				PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object e, DPSelectedItemEventArgs s)
				{
					int id = s.ID;
					this.mInputNum = id / this.mUnit;
					if (this.mInputNum > this.mMaxBiddingMoney)
					{
						Super.HintMainText(Global.GetLang("战盟资金不足"), 10, 3);
					}
					else
					{
						float scrollValue = (float)this.mInputNum / (float)this.mMaxBiddingMoney;
						this.mValueScrollBar.scrollValue = scrollValue;
					}
				}, null, 0, -100);
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

	private bool CheckRoleBiddingMoney()
	{
		int @int = PlayerPrefs.GetInt(string.Concat(new object[]
		{
			Global.Data.RoleID,
			"_",
			this.mSelectServerID,
			"_LastPludderBiddingMoney"
		}));
		if (0 >= @int)
		{
			return true;
		}
		if (this.GetBiddingMoney() <= @int || this.GetBiddingMoney() == 0)
		{
			Super.HintMainText(Global.GetLang("出这么少是无法竞价成功的"), 10, 3);
			return false;
		}
		return true;
	}

	private void RefreshNum(float value)
	{
		int num = Mathf.RoundToInt(value * (float)this.mMaxBiddingMoney);
		this.mInputNum = Mathf.Min(num, this.mMaxBiddingMoney);
		if (this.mInputNum <= 0)
		{
			this.mInputNum = 1;
			this.mValueScrollBar.scrollValue = (float)this.mInputNum / (float)this.mMaxBiddingMoney;
			return;
		}
		this.mValuesLabel[3].text = this.GetBiddingMoney().ToString();
	}

	private void RefreshScollBar(int direction)
	{
		if (0 < direction)
		{
			if (this.mMaxBiddingMoney <= this.mInputNum)
			{
				return;
			}
			this.mInputNum++;
		}
		else
		{
			if (1 >= this.mInputNum)
			{
				return;
			}
			this.mInputNum--;
		}
		float scrollValue = (float)this.mInputNum / (float)this.mMaxBiddingMoney;
		this.mValueScrollBar.scrollValue = scrollValue;
	}

	public void NoticeBhDataCallBack(BangHuiDetailData data)
	{
		if (data != null)
		{
			this.mBHMoney = (long)data.TotalMoney;
			this.mMaxBiddingMoney = (int)(this.mBHMoney - this.mBHMoney % (long)this.mUnit) / this.mUnit;
			this.mValuesLabel[0].text = data.TotalMoney.ToString();
			this.RefreshNum(0f);
		}
	}

	public void RefreshBiddingData(KuaFuLueDuoMainInfo data)
	{
		if (data != null)
		{
			bool flag = false;
			KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaStateDataByID = data.GetKuaFuLueDuoServerJingJiaStateDataByID(this.mSelectServerID);
			if (kuaFuLueDuoServerJingJiaStateDataByID == null && KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo != null)
			{
				kuaFuLueDuoServerJingJiaStateDataByID = KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(this.mSelectServerID);
			}
			if (kuaFuLueDuoServerJingJiaStateDataByID != null && kuaFuLueDuoServerJingJiaStateDataByID.JingJiaList != null)
			{
				for (int i = 0; i < kuaFuLueDuoServerJingJiaStateDataByID.JingJiaList.Count; i++)
				{
					if (kuaFuLueDuoServerJingJiaStateDataByID.JingJiaList[i].BhId == Global.Data.roleData.Faction)
					{
						this.mValuesLabel[2].text = Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("已上榜")
						});
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				this.mValuesLabel[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("未上榜")
				});
			}
		}
		else
		{
			this.mValuesLabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("未上榜")
			});
		}
		int num = PlayerPrefs.GetInt(string.Concat(new object[]
		{
			Global.Data.RoleID,
			"_",
			this.mSelectServerID,
			"_LastPludderBiddingMoney"
		}));
		if (0 > num)
		{
			num = 0;
		}
		this.mValuesLabel[1].text = num.ToString();
	}

	public void RefreshBiddingData(KuaFuLueDuoStateData StateData)
	{
	}

	public int SelectServerID
	{
		set
		{
			this.mSelectServerID = value;
			DateTime correctDateTime = Global.GetCorrectDateTime();
			string @string = PlayerPrefs.GetString("strPlunderBiddingTimeTicks" + Global.Data.RoleID);
			long num = -1L;
			if (long.TryParse(@string, ref num))
			{
			}
			if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo != null)
			{
				this.RefreshBiddingData(KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo);
			}
		}
	}

	private const string mPlunderBiddingTimeTicksKey = "strPlunderBiddingTimeTicks";

	private const string mLastPludderBiddingMoney = "LastPludderBiddingMoney";

	private const string mLastPludderBiddingMoneyThis = "LastPludderBiddingMoneyThis";

	private const string mLastBiddingServerIDStrList = "LastBiddingServerIDStrList";

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private UILabel mTitleLable;

	[SerializeField]
	private UILabel[] mNamesLabel;

	[SerializeField]
	private UILabel[] mValuesLabel;

	[SerializeField]
	private UISprite mAddSp;

	[SerializeField]
	private UISprite mSubSp;

	[SerializeField]
	private UIScrollBar mValueScrollBar;

	[SerializeField]
	private GButton mSureBtn;

	[SerializeField]
	private UILabel mInfLabel;

	[SerializeField]
	private GameObject mInPutObj;

	private int mMaxBiddingMoney;

	private long mBHMoney;

	private int mBiddingBtnCD = -1;

	private int mInputNum;

	private int mUnit = 1;

	private DispatcherTimer mDispatcherTimer;

	private long mBiddingTimeTicks = -1L;

	private int mSelectServerID;

	public DPSelectedItemEventHandler Hander;
}
