using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderMapMainPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mBiddingInfTrans.localPosition = new Vector3(340f, 0f, 0f);
		this.mSelfServerInfTrans.localPosition = new Vector3(340f, -172f, 0f);
		this.InitHander();
		this.mBgImage.gameObject.SetActive(false);
		this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
		this.mBtnBiddingInfText.text = Global.GetLang("每轮竞拍");
		this.mBtnSelfServerInfText.text = Global.GetLang("本服情况");
		this.mBtnBiddingInfTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("竞拍结束时间")
		});
		this.mBtnInf.Label.text = Global.GetLang("详情");
		this.mBtnBiddingInfValueLabel1.text = string.Empty;
		this.mBtnBiddingInfValueLabel1.Margin = new Vector2(this.mBtnBiddingInfValueLabel1.Margin.x, 14f);
		UILabel uilabel = this.mBtnBiddingInfValueLabel1;
		uilabel.text += Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Concat(new string[]
			{
				Global.GetLang("第一轮竞拍结果："),
				"\n",
				Global.GetLang("第二轮竞拍结果："),
				"\n",
				Global.GetLang("第三轮竞拍结果："),
				"\n",
				Global.GetLang("第四轮竞拍结果：")
			})
		});
		this.mBtnSelfServerInfValueLabel1.text = string.Empty;
		this.mBtnSelfServerInfValueLabel1.Margin = new Vector2(this.mBtnSelfServerInfValueLabel1.Margin.x, 14f);
		this.mBtnSelfServerInfValueLabel1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Concat(new string[]
			{
				Global.GetLang("当前剩余资源:"),
				"\n",
				Global.GetLang("已征服服务器数："),
				"\n",
				Global.GetLang("世仇服务器数："),
				"\n",
				Global.GetLang("最强战盟名：")
			})
		});
		this.mBtnBiddingInfValueLabel2.Margin = new Vector2(this.mBtnBiddingInfValueLabel2.Margin.x, 14f);
		this.mBtnSelfServerInfValueLabel2.Margin = new Vector2(this.mBtnSelfServerInfValueLabel2.Margin.x, 14f);
		this.StartTimeTicks();
		this.BtnsList.Label.text = Global.GetLang("掠夺列表");
		this.mBtnSelfServerInfValueLabel2.pivot = 0;
		this.mBtnSelfServerInfValueLabel2.transform.localPosition = new Vector3(-90f, 52f, -1f);
	}

	protected override void OnDestroy()
	{
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
		base.OnDestroy();
		this.StopTimeTicks();
	}

	private void RefreshSelfServerInf()
	{
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			if (this.mKuaFuLueDuoStateData != null)
			{
				KuaFuLueDuoServerInfo kuaFuLueDuoServerInfoDataByID = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(this.mKuaFuLueDuoStateData.ServerID);
				if (kuaFuLueDuoServerInfoDataByID != null)
				{
					this.mBtnSelfServerInfValueLabel2.text = string.Concat(new string[]
					{
						Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							kuaFuLueDuoServerInfoDataByID.ZiYuan
						}),
						"\n",
						Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							(kuaFuLueDuoServerInfoDataByID.ZhengFuList != null) ? kuaFuLueDuoServerInfoDataByID.ZhengFuList.Count : 0
						}),
						"\n",
						Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							(kuaFuLueDuoServerInfoDataByID.ShiChouList != null) ? kuaFuLueDuoServerInfoDataByID.ShiChouList.Count : 0
						}),
						"\n"
					});
					if (!string.IsNullOrEmpty(kuaFuLueDuoServerInfoDataByID.MingXingZhanMengList))
					{
						List<KuaFuLueDuoRankInfo> list = KuaFuLueDuoServerInfo.MingXingStr2RankList(kuaFuLueDuoServerInfoDataByID.MingXingZhanMengList);
						if (0 < list.Count)
						{
							UILabel uilabel = this.mBtnSelfServerInfValueLabel2;
							uilabel.text += Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								(!string.IsNullOrEmpty(list[0].Param1)) ? list[0].Param1 : string.Empty
							});
						}
						else
						{
							UILabel uilabel2 = this.mBtnSelfServerInfValueLabel2;
							uilabel2.text += Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								Global.GetLang("暂无")
							});
						}
					}
					else
					{
						UILabel uilabel3 = this.mBtnSelfServerInfValueLabel2;
						uilabel3.text += Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							Global.GetLang("暂无")
						});
					}
				}
			}
			else
			{
				this.mBtnSelfServerInfValueLabel2.text = string.Empty;
			}
		}
		else
		{
			this.mBtnSelfServerInfValueLabel2.text = string.Empty;
		}
	}

	private void InitHander()
	{
		try
		{
			UIEventListener.Get(this.backBtn.gameObject).onClick = delegate(GameObject s)
			{
				KuaFuPlunderEventProcessor.ProcessEvent(EmKuaFuPlunderEvent.LeavePlunderMap);
			};
			this.BtnsList.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1505
				});
			};
			UIEventListener.Get(this.mBtnBiddingInf).onClick = delegate(GameObject g)
			{
				Vector3 zero = Vector3.zero;
				if (this.mBiddingInfTrans.localPosition == Vector3.zero)
				{
					zero..ctor(340f, 0f, 0f);
				}
				TweenPosition.Begin(this.mBiddingInfTrans.gameObject, 0.1f, zero);
			};
			UIEventListener.Get(this.mBtnSelfServerInf).onClick = delegate(GameObject g)
			{
				Vector3 vector;
				vector..ctor(0f, -174f, 0f);
				if (this.mSelfServerInfTrans.localPosition == vector)
				{
					vector..ctor(340f, -174f, 0f);
				}
				TweenPosition.Begin(this.mSelfServerInfTrans.gameObject, 0.1f, vector);
			};
			this.mBtnInf.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1510,
						MyID = KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID
					});
				}
				else
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1510,
						MyID = Global.Data.GameServerID
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
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlubderPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		this.mBtnBiddingInfValueLabel2.text = string.Empty;
		for (byte b = 0; b < 4; b += 1)
		{
			byte b2 = 0;
			DateTime nearTimeByRound = this.mCrusadeWarXml.GetNearTimeByRound((int)(b + 1), out b2);
			if (DateTime.MinValue < nearTimeByRound)
			{
				if (b2 == 0)
				{
					UILabel uilabel = this.mBtnBiddingInfValueLabel2;
					uilabel.text = uilabel.text + Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("尚未开始")
					}) + "\n";
				}
				else if (b2 == 1)
				{
					CrusadeWarVO crusadeWarVOByID = this.mCrusadeWarXml.GetCrusadeWarVOByID(1);
					if (crusadeWarVOByID != null)
					{
						CrusadeWarVO.TimePoint timePoint = crusadeWarVOByID.ApplyEnd[0];
						UILabel uilabel2 = this.mBtnBiddingInfValueLabel2;
						uilabel2.text = uilabel2.text + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							this.PrintfTime(timePoint.Time1[0]) + ":" + this.PrintfTime(timePoint.Time1[1])
						}) + "\n";
					}
				}
				else if (b2 == 2)
				{
					UILabel uilabel3 = this.mBtnBiddingInfValueLabel2;
					uilabel3.text = uilabel3.text + Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("尚未开始")
					}) + "\n";
				}
				else
				{
					UILabel uilabel4 = this.mBtnBiddingInfValueLabel2;
					uilabel4.text = uilabel4.text + Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("已公布")
					}) + "\n";
				}
			}
			else
			{
				UILabel uilabel5 = this.mBtnBiddingInfValueLabel2;
				uilabel5.text = uilabel5.text + Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("已公布")
				}) + "\n";
			}
		}
	}

	private string PrintfTime(int value)
	{
		if (10 > value)
		{
			return "0" + value.ToString();
		}
		return value.ToString();
	}

	public void RefreshInf(KuaFuLueDuoMainInfo data)
	{
		if (data != null)
		{
			if (this.mKuaFuLueDuoMainInfo != null)
			{
				if (this.mKuaFuLueDuoMainInfo.ServerListAge != data.ServerListAge)
				{
					this.mKuaFuLueDuoMainInfo.ServerList = data.ServerList;
					this.mKuaFuLueDuoMainInfo.ServerListAge = data.ServerListAge;
					this.RefreshSelfServerInf();
				}
				if (data.StateListAge != this.mKuaFuLueDuoMainInfo.StateListAge)
				{
					this.mKuaFuLueDuoMainInfo.StateListAge = data.StateListAge;
					this.mKuaFuLueDuoMainInfo.StateList = data.StateList;
					this.mKuaFuLueDuoMainInfo.JingJiaData = data.JingJiaData;
					this.RefreshSelfServerInf();
				}
			}
			else
			{
				this.mKuaFuLueDuoMainInfo = data;
				this.RefreshSelfServerInf();
			}
		}
	}

	internal void RefreshStateInf(KuaFuLueDuoStateData StateData)
	{
		bool flag = true;
		if (StateData != null)
		{
			if (this.mKuaFuLueDuoStateData != null)
			{
				if (this.mKuaFuLueDuoStateData.Age != StateData.Age)
				{
					this.mKuaFuLueDuoStateData = StateData;
					flag = true;
				}
			}
			else
			{
				this.mKuaFuLueDuoStateData = StateData;
				flag = true;
			}
		}
		if (flag)
		{
			this.RefreshSelfServerInf();
		}
	}

	[SerializeField]
	private UIButton backBtn;

	[SerializeField]
	private ShowNetImage mBgImage;

	[SerializeField]
	private GButton BtnsList;

	[SerializeField]
	private GameObject mBtnBiddingInf;

	[SerializeField]
	private GameObject mBtnSelfServerInf;

	[SerializeField]
	private Transform mBiddingInfTrans;

	[SerializeField]
	private Transform mSelfServerInfTrans;

	[SerializeField]
	private UILabel mBtnBiddingInfText;

	[SerializeField]
	private UILabel mBtnSelfServerInfText;

	[SerializeField]
	private UILabel mBtnBiddingInfTitleLabel;

	[SerializeField]
	private UILabel mBtnBiddingInfValueLabel1;

	[SerializeField]
	private UILabel mBtnBiddingInfValueLabel2;

	[SerializeField]
	private UILabel mBtnSelfServerInfValueLabel1;

	[SerializeField]
	private UILabel mBtnSelfServerInfValueLabel2;

	[SerializeField]
	private GButton mBtnInf;

	private DispatcherTimer mDispatcherTimer;

	private CrusadeWarXml mCrusadeWarXml;

	private KuaFuLueDuoMainInfo mKuaFuLueDuoMainInfo;

	private KuaFuLueDuoStateData mKuaFuLueDuoStateData;
}
