using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderChoseFightPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mOneRoot.SetActive(false);
		this.mTwoRoot.SetActive(false);
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
		this.StartTimeTicks();
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(this.mKuaFuLueDuoMainInfo.ServerListAge, this.mKuaFuLueDuoMainInfo.StateListAge);
		}
		else
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
		}
		if (this.mKuaFuLueDuoStateData != null)
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(this.mKuaFuLueDuoStateData.Age);
		}
		else
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
		}
		this.NoticeRefreshEnterNumber();
	}

	protected override void OnDestroy()
	{
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
		base.OnDestroy();
		this.StopTimeTicks();
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		DateTime minValue = DateTime.MinValue;
		bool flag = false;
		this.mCrusadeWarXml.GetNextStateTimeData(out minValue, out flag, this.mKuaFuPlunderGameStateType);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (minValue > correctDateTime)
		{
			int sec = (int)(minValue - correctDateTime).TotalSeconds;
			string text = string.Empty;
			string text2 = "17e43e";
			if (this.mKuaFuPlunderGameStateType == null)
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("距竞价开始：")
				});
				text2 = "ff0000";
			}
			else if (this.mKuaFuPlunderGameStateType != 3)
			{
				if (this.mKuaFuPlunderGameStateType == 1)
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("距竞价结束：")
					});
				}
				else if (this.mKuaFuPlunderGameStateType == 2)
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("距活动开始：")
					});
					text2 = "ff0000";
				}
				else if (this.mKuaFuPlunderGameStateType == 4)
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("距竞价开始：")
					});
					text2 = "ff0000";
				}
			}
			if (this.mKuaFuPlunderGameStateType == 3)
			{
				this.mTimeLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format(Global.GetLang("参与次数为0无法进入"), new object[0])
				});
			}
			else
			{
				this.mTimeLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					string.Format(Global.GetLang("{0}{1}"), text, Global.GetTimeStrBySecFilterZero(sec, true, 2))
				});
			}
		}
		if (flag && this.mSynchronousData == null)
		{
			this.mSynchronousData = new KuaFuPlunderChoseFightPart.SynchronousData();
			this.mSynchronousData.SenderGetData();
		}
		if (this.mSynchronousData != null && this.mSynchronousData.UpDate())
		{
			this.mSynchronousData = null;
		}
		if (60 < ++this.RefreshCD)
		{
			this.RefreshCD = 0;
			if (this.mKuaFuLueDuoMainInfo != null)
			{
				GameInstance.Game.SendGetKuFuPlubderServerDataList(this.mKuaFuLueDuoMainInfo.ServerListAge, this.mKuaFuLueDuoMainInfo.StateListAge);
			}
			else
			{
				GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
			}
			if (this.mKuaFuLueDuoStateData == null)
			{
				GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
			}
			else
			{
				GameInstance.Game.SendGetKuFuPlubderGameStateData(this.mKuaFuLueDuoStateData.Age);
			}
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + Global.GetLang("向服务器同步信息") + "</color>"
			});
		}
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
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlunderChoseFightPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTwoPlunderValueLabel.Margin = new Vector2(0f, 16f);
			this.mTimeLiftLabel.text = string.Empty;
			this.mOneTargetLabel.text = string.Empty;
			this.mOneResValueLabel1.text = string.Empty;
			this.mOneResValueLabel2.text = string.Empty;
			this.mTwoPlunderLabel.text = string.Empty;
			this.mTwoPlunderValueLabel.text = string.Empty;
			this.mTwoResLiftLabel.text = string.Empty;
			this.mTargetInfBtn.Label.text = Global.GetLang("目标详情");
			this.mFightGoBtn.Label.text = Global.GetLang("前往入侵");
			this.mDefendBtn.Label.text = Global.GetLang("前往防守");
			this.mNumberLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("剩余参与次数："),
				"fdf7dd",
				Global.Data.roleData.MoneyData[134]
			});
			this.mBuyNumberBtn.Label.text = Global.GetLang("购买");
			this.mTwoPlunderValueLabel.pivot = 0;
			this.mTwoPlunderValueLabel.transform.localPosition = new Vector3(90f, -21f, -0.5f);
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
			this.mBGgImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_XuanZeZhanChangDi.jpg";
			this.mBGgImage.ImageDownloaded = delegate(object g)
			{
				this.mBGgImage.transform.localScale = new Vector3((float)this.mBGgImage.ItsSizeWidth, (float)this.mBGgImage.ItsSizeHeight, 0f);
			};
			this.mBGgImageone.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_RuQinLueDuo.jpg";
			this.mBGgImageone.ImageDownloaded = delegate(object g)
			{
				this.mBGgImageone.transform.localScale = new Vector3((float)this.mBGgImageone.ItsSizeWidth, (float)this.mBGgImageone.ItsSizeHeight, 0f);
			};
			this.mBGgImageTwo.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_FangShouBenFu.jpg";
			this.mBGgImageTwo.ImageDownloaded = delegate(object g)
			{
				this.mBGgImageTwo.transform.localScale = new Vector3((float)this.mBGgImageTwo.ItsSizeWidth, (float)this.mBGgImageTwo.ItsSizeHeight, 0f);
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

	private void InitHandler()
	{
		try
		{
			this.mTargetInfBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mKuaFuLueDuoMainInfo != null && this.mKuaFuLueDuoMainInfo.JingJiaData != null)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1510,
						MyID = this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId
					});
				}
			};
			this.mFightGoBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mKuaFuPlunderGameStateType == 3 && 0L >= Global.Data.roleData.MoneyData[134])
				{
					Super.HintMainText(Global.GetLang("参与次数已用光，无法进入"), 10, 3);
					return;
				}
				if (this.mKuaFuLueDuoMainInfo != null)
				{
					if (this.mKuaFuLueDuoMainInfo.JingJiaData != null)
					{
						if (0 < this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId)
						{
							GameInstance.Game.SendGetKuaFuPlundeEnterMap(this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId);
						}
						else
						{
							MUDebug.Log<string>(new string[]
							{
								"<color=yellow> mKuaFuLueDuoMainInfo.JingJiaData.ServerId <= 0</color>"
							});
						}
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>mKuaFuLueDuoMainInfo.JingJiaData == null</color>"
						});
					}
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"<color=yellow>mKuaFuLueDuoMainInfo == null</color>"
					});
				}
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this.mDefendBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mKuaFuPlunderGameStateType == 3 && 0L >= Global.Data.roleData.MoneyData[134])
				{
					Super.HintMainText(Global.GetLang("参与次数已用光，无法进入"), 10, 3);
					return;
				}
				if (this.mKuaFuLueDuoStateData != null)
				{
					if (this.mKuaFuLueDuoStateData.AttackerList == null || 0 >= this.mKuaFuLueDuoStateData.AttackerList.Count)
					{
						Super.HintMainText(Global.GetLang("无来犯战盟，无法进入"), 10, 3);
						return;
					}
				}
				GameInstance.Game.SendGetKuaFuPlundeEnterMap(0);
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
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
			this.mBuyNumberBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mKuaFuPlunderGameStateType != 3)
				{
					Super.HintMainText(Global.GetLang("战场尚未开放，不能购买参与次数"), 10, 3);
					return;
				}
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1513
				});
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

	private int GetSelfServerID()
	{
		if (this.mKuaFuLueDuoStateData != null)
		{
			return this.mKuaFuLueDuoStateData.ServerID;
		}
		return Global.Data.GameServerID;
	}

	public void NoticeGetMainInfCallBack(KuaFuLueDuoMainInfo data)
	{
		bool flag = false;
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			if (this.mKuaFuLueDuoMainInfo.ServerListAge != data.ServerListAge)
			{
				flag = true;
				this.mKuaFuLueDuoMainInfo.ServerList = data.ServerList;
				this.mKuaFuLueDuoMainInfo.ServerListAge = data.ServerListAge;
			}
			if (this.mKuaFuLueDuoMainInfo.StateListAge != data.StateListAge)
			{
				flag = true;
				this.mKuaFuLueDuoMainInfo.StateList = data.StateList;
				this.mKuaFuLueDuoMainInfo.JingJiaData = data.JingJiaData;
				this.mKuaFuLueDuoMainInfo.StateListAge = data.StateListAge;
			}
		}
		else
		{
			flag = true;
			this.mKuaFuLueDuoMainInfo = data;
		}
		if (flag)
		{
			int type = 0;
			if (this.mKuaFuLueDuoMainInfo.JingJiaData != null && 0 < this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId)
			{
				type = 1;
			}
			this.RefreshFightType(type);
		}
	}

	public void RefreshFightType(int type)
	{
		this.mTwoRoot.SetActive(true);
		if (type == 1)
		{
			this.mOneRoot.SetActive(true);
			this.mOneRoot.transform.localPosition = new Vector3(-283f, 169f, -1f);
			this.mTwoRoot.transform.localPosition = new Vector3(68f, 0f, -1f);
			this.mTwoRoot.SetActive(true);
			if (this.mKuaFuLueDuoStateData != null)
			{
				ZtBuffServerInfo ztBuffServerInfo = null;
				if (Global.GetNowServerIsZhuTiFu(this.mKuaFuLueDuoStateData.EnemyServerID, out ztBuffServerInfo))
				{
					this.mOneTargetLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("入侵目标："),
						"fdf7dd",
						ztBuffServerInfo.strServerName
					});
				}
				else
				{
					this.mOneTargetLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("入侵目标："),
						"fdf7dd",
						this.mKuaFuLueDuoStateData.EnemyServerID
					});
				}
				this.mOneResValueLabel1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("已掠夺资源："),
					"fdf7dd",
					this.mKuaFuLueDuoStateData.LueDuoZiYuan
				});
				this.mOneResValueLabel2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("剩余资源："),
					"fdf7dd",
					this.mKuaFuLueDuoStateData.EnemyZiYuan
				});
			}
			this.mTwoPlunderLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("来犯战盟：")
			});
			if (this.mKuaFuLueDuoStateData != null)
			{
				this.mTwoResLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("剩余资源："),
					"fdf7dd",
					this.mKuaFuLueDuoStateData.ZiYuan
				});
			}
		}
		else
		{
			this.mOneRoot.transform.localPosition = new Vector3(-80f, 0f, -1f);
			this.mOneRoot.SetActive(false);
			this.mTwoRoot.SetActive(true);
			this.mTwoRoot.transform.localPosition = new Vector3(-100f, 0f, -1f);
			KuaFuLueDuoServerInfo kuaFuLueDuoServerInfoDataByID = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(this.GetSelfServerID());
			if (kuaFuLueDuoServerInfoDataByID != null)
			{
				this.mTwoPlunderLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("来犯战盟：")
				});
				this.mTwoResLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("剩余资源："),
					"fdf7dd",
					kuaFuLueDuoServerInfoDataByID.ZiYuan
				});
			}
		}
		if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null)
		{
			this.NoticeGetGameStateCallBack(KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData);
		}
	}

	public void NoticeGetScoreDataCallBack(KuaFuLueDuoScoreData data)
	{
		this.NoticeRefreshEnterNumber();
	}

	public void NoticeGetGameStateCallBack(KuaFuLueDuoStateData StateData)
	{
		this.mKuaFuLueDuoStateData = StateData;
		this.mKuaFuPlunderGameStateType = StateData.GameState;
		if (this.mKuaFuLueDuoStateData != null)
		{
			string text = string.Empty;
			if (this.mKuaFuLueDuoStateData.AttackerList != null)
			{
				int num = 0;
				for (int i = 0; i < this.mKuaFuLueDuoStateData.AttackerList.Count; i++)
				{
					if (this.mKuaFuLueDuoStateData.AttackerList[i] != null)
					{
						ZtBuffServerInfo ztBuffServerInfo = null;
						if (Global.GetNowServerIsZhuTiFu(this.mKuaFuLueDuoStateData.AttackerList[i].ZoneID, out ztBuffServerInfo))
						{
							text += Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, StateData.AttackerList[i].BHName, 0);
							num++;
						}
						else
						{
							string text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								"S.",
								this.mKuaFuLueDuoStateData.AttackerList[i].ZoneID,
								" ",
								StateData.AttackerList[i].BHName,
								"\n"
							});
							num++;
						}
					}
					if (3 <= num)
					{
						break;
					}
				}
			}
			if (string.Empty.Equals(text))
			{
				text = Global.GetLang("暂无");
			}
			this.mTwoPlunderValueLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				text
			});
			ZtBuffServerInfo ztBuffServerInfo2 = null;
			if (Global.GetNowServerIsZhuTiFu(this.mKuaFuLueDuoStateData.EnemyServerID, out ztBuffServerInfo2))
			{
				this.mOneTargetLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("入侵目标："),
					"fdf7dd",
					ztBuffServerInfo2.strServerName
				});
			}
			else
			{
				this.mOneTargetLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("入侵目标："),
					"fdf7dd",
					this.mKuaFuLueDuoStateData.EnemyServerID
				});
			}
			this.mOneResValueLabel1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("已掠夺资源："),
				"fdf7dd",
				this.mKuaFuLueDuoStateData.LueDuoZiYuan
			});
			this.mOneResValueLabel2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("剩余资源："),
				"fdf7dd",
				this.mKuaFuLueDuoStateData.EnemyZiYuan
			});
			this.mTwoResLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("剩余资源："),
				"fdf7dd",
				this.mKuaFuLueDuoStateData.ZiYuan
			});
		}
	}

	public void NoticeRefreshEnterNumber()
	{
		if (null != this.mNumberLiftLabel)
		{
			this.mNumberLiftLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("剩余参与次数："),
				"fdf7dd",
				Global.Data.roleData.MoneyData[134]
			});
		}
	}

	public KuaFuLueDuoMainInfo KuaFuLueDuoMainInfo
	{
		set
		{
			this.mKuaFuLueDuoMainInfo = value;
			this.NoticeGetMainInfCallBack(this.mKuaFuLueDuoMainInfo);
		}
	}

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private GButton mTargetInfBtn;

	[SerializeField]
	private GButton mFightGoBtn;

	[SerializeField]
	private GButton mDefendBtn;

	[SerializeField]
	private UILabel mOneTargetLabel;

	[SerializeField]
	private UILabel mOneResValueLabel1;

	[SerializeField]
	private UILabel mOneResValueLabel2;

	[SerializeField]
	private GameObject mOneRoot;

	[SerializeField]
	private UILabel mTwoPlunderLabel;

	[SerializeField]
	private UILabel mTwoPlunderValueLabel;

	[SerializeField]
	private UILabel mTwoResLiftLabel;

	[SerializeField]
	private GameObject mTwoRoot;

	[SerializeField]
	private UILabel mTimeLiftLabel;

	[SerializeField]
	private UILabel mNumberLiftLabel;

	[SerializeField]
	private GButton mBuyNumberBtn;

	[SerializeField]
	private ShowNetImage mBGgImage;

	[SerializeField]
	private ShowNetImage mBGgImageone;

	[SerializeField]
	private ShowNetImage mBGgImageTwo;

	private DispatcherTimer mDispatcherTimer;

	private KuaFuPlunderChoseFightPart.SynchronousData mSynchronousData;

	private CrusadeWarXml mCrusadeWarXml;

	private KuaFuLueDuoGameStates mKuaFuPlunderGameStateType;

	private KuaFuLueDuoMainInfo mKuaFuLueDuoMainInfo;

	private KuaFuLueDuoStateData mKuaFuLueDuoStateData;

	private int RefreshCD;

	public DPSelectedItemEventHandler Hander;

	private class SynchronousData
	{
		public SynchronousData()
		{
			this.Synchronous = true;
		}

		public bool Synchronous
		{
			set
			{
				if (!this.mSynchronous)
				{
					this.mSynchronous = value;
					this.mSynchronousTime = 3;
				}
				if (!value)
				{
					this.mSynchronous = value;
				}
			}
		}

		public bool UpDate()
		{
			if (!this.mSynchronous)
			{
				return false;
			}
			this.mSynchronousTime--;
			if (0 > this.mSynchronousTime)
			{
				this.Synchronous = false;
				return true;
			}
			return false;
		}

		public void SenderGetData()
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
		}

		private int mSynchronousTime;

		private bool mSynchronous;
	}
}
