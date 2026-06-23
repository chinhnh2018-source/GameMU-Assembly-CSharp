using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderServerInFPart : UserControl
{
	private KuaFuLueDuoStateData KuaFuLueDuoStateData
	{
		get
		{
			if (this.mKuaFuLueDuoStateData == null)
			{
				return KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData;
			}
			return this.mKuaFuLueDuoStateData;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		GameInstance.Game.SpriteQueryBangHuiDetail(Global.Data.roleData.Faction);
		this.InitTexture();
		this.InitHandler();
		this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
	}

	protected override void OnDestroy()
	{
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
		base.OnDestroy();
		this.StopTimeTicks();
	}

	private int GetNpcId(byte Index)
	{
		if (this.IsSelfServer())
		{
			return 86000;
		}
		if (Index == 0)
		{
			return 86004;
		}
		if (Index == 1)
		{
			return 86003;
		}
		if (Index == 2)
		{
			return 86002;
		}
		if (Index == 3)
		{
			return 86001;
		}
		return 86000;
	}

	private bool IsSelfServer()
	{
		if (this.mKuaFuLueDuoStateData != null)
		{
			return this.mServerID == this.mKuaFuLueDuoStateData.ServerID;
		}
		return KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && this.mServerID == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID;
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
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			if (this.mKuaFuLueDuoServerJingJiaState == null || this.mCrusadeWarXml == null)
			{
				return;
			}
			byte b = 0;
			DateTime nearTimeByRound = this.mCrusadeWarXml.GetNearTimeByRound(this.mKuaFuLueDuoServerJingJiaState.Round, out b);
			if (DateTime.MinValue < nearTimeByRound)
			{
				double totalSeconds = (nearTimeByRound - Global.GetCorrectDateTime()).TotalSeconds;
				if (0.0 < totalSeconds)
				{
					if (b == 0)
					{
						this.mInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("距离竞价开始："),
							"ff0000",
							Global.GetTimeStrBySecFilterZero((int)totalSeconds, true, 2)
						});
					}
					else if (b == 1)
					{
						this.mInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("距离竞价结束："),
							"17e43e",
							Global.GetTimeStrBySecFilterZero((int)totalSeconds, true, 2)
						});
					}
					else if (b == 2)
					{
						this.mInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							Global.GetLang("距离竞价开始："),
							"ff0000",
							Global.GetTimeStrBySecFilterZero((int)totalSeconds, true, 2)
						});
					}
					else
					{
						this.mInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							Global.GetLang("结果已公布")
						});
					}
				}
				else
				{
					this.mInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("结果已公布")
					});
				}
			}
			else
			{
				this.mInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("结果已公布")
				});
			}
		}
	}

	private string GetlabelStr(int idnex)
	{
		string text = string.Empty;
		if (idnex == 1)
		{
			text = Global.GetLang("距离公布结果：");
		}
		else if (idnex == 2)
		{
			text = Global.GetLang("当前资源：");
		}
		else if (idnex == 3)
		{
			text = Global.GetLang("暂时获得掠夺资格战盟个数：");
		}
		else if (idnex == 4)
		{
			text = Global.GetLang("本盟竞价：");
		}
		else if (idnex == 5)
		{
			text = Global.GetLang("竞价状态：");
		}
		return Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			text
		});
	}

	private void InitPrefabText()
	{
		try
		{
			this.mGoBiddingBtn.Text = Global.GetLang("竞价");
			this.mRightViewGuanXiLabel.Margin = new Vector2(1f, 14f);
			this.mRightViewRankLabel.Margin = new Vector2(1f, 14f);
			this.mRightViewMingXingLabel.Margin = new Vector2(1f, 14f);
			this.mRightViewLastZhanKuangLabel.Margin = new Vector2(1f, 14f);
			this.mRightViewGuanXiLabel.text = string.Empty;
			this.mRightViewRankLabel.text = string.Empty;
			this.mRightViewMingXingLabel.text = string.Empty;
			this.mRightViewLastZhanKuangLabel.text = string.Empty;
			this.mTipsInfLabel1.text = string.Empty;
			this.mTipsInfLabel2.text = string.Empty;
			this.mInfLabel.text = string.Empty;
			this.mInfLabel1.text = string.Empty;
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("服务器信息")
			});
			this.mRightViewRankLabel.lineWidth = 200;
			this.mInfLabel.pivot = 0;
			this.mInfLabel.transform.localPosition = new Vector3(0f, 40f, 0f);
			this.mInfLabel.lineWidth = 260;
			this.mInfLabel1.pivot = 0;
			this.mInfLabel1.transform.localPosition = new Vector3(0f, 60f, 0f);
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
			this.mBgImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/ChengChiDi.jpg";
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
			this.mTipsRoot.SetActive(false);
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
			this.mGoBiddingBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				int trigger = -1;
				int num = -1;
				int param = -1;
				if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuPlunderBindding, ref trigger, ref num, ref param) && Global.zhanmengLevel < num)
				{
					UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuPlunder, trigger, num, param, true);
					return;
				}
				int num2 = -1;
				if (this.mKuaFuLueDuoMainInfo != null && this.mKuaFuLueDuoMainInfo.JingJiaData != null && 0 < this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId && this.mServerID != this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId)
				{
					num2 = this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId;
				}
				if (num2 == -1)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1506,
						MyID = this.mServerID
					});
				}
				else
				{
					KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaStateDataByID = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(num2);
					if (kuaFuLueDuoServerJingJiaStateDataByID != null)
					{
						ZtBuffServerInfo ztBuffServerInfo = null;
						if (Global.GetNowServerIsZhuTiFu(kuaFuLueDuoServerJingJiaStateDataByID.ServerId, out ztBuffServerInfo))
						{
							Super.HintMainText(Global.GetLang("已竞价") + ztBuffServerInfo.strServerName + Global.GetLang("无法竞价其他服务器"), 10, 3);
						}
						else
						{
							Super.HintMainText(string.Concat(new object[]
							{
								Global.GetLang("已竞价"),
								"S.",
								kuaFuLueDuoServerJingJiaStateDataByID.ServerId,
								Global.GetLang("无法竞价其他服务器")
							}), 10, 3);
						}
					}
				}
			};
			UIEventListener.Get(this.mBgGameObject).onClick = delegate(GameObject g)
			{
				this.CloseTips();
			};
			this.RefreshBiddingBtn();
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private float GetLineHight()
	{
		float num = -36f;
		if (!string.IsNullOrEmpty(this.mRightViewGuanXiLabel.text))
		{
			num -= 18f;
			num -= this.mRightViewGuanXiLabel.relativeSize.y * 18f;
		}
		if (!string.IsNullOrEmpty(this.mRightViewRankLabel.text))
		{
			num -= 18f;
			num -= this.mRightViewRankLabel.relativeSize.y * 18f;
		}
		if (!string.IsNullOrEmpty(this.mRightViewMingXingLabel.text))
		{
			num -= 18f;
			num -= this.mRightViewMingXingLabel.relativeSize.y * 18f;
		}
		if (!string.IsNullOrEmpty(this.mRightViewLastZhanKuangLabel.text))
		{
			num -= 18f;
			num -= this.mRightViewLastZhanKuangLabel.relativeSize.y * 18f;
		}
		return num;
	}

	private void SetObjPos(Component obj)
	{
		if (null != obj)
		{
			Vector3 localPosition = obj.transform.localPosition;
			localPosition.y = this.GetLineHight();
			obj.transform.localPosition = localPosition;
		}
	}

	private void RefreshBoxCollider(UILabel label)
	{
		BoxCollider boxCollider = label.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = label.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.size = new Vector3(label.relativeSize.x, label.relativeSize.y, 0f);
		boxCollider.center = new Vector3(label.relativeSize.x / 2f, -label.relativeSize.y / 2f);
		UIEventListener.Get(label.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickLabel);
	}

	private bool ValueIsInList(List<int> list)
	{
		if (list != null)
		{
			foreach (int num in list)
			{
				if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && num == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public void RefreshLiftInf()
	{
		this.mListTitleServerNameLabel.text = string.Empty;
		this.mInfLabel.text = string.Empty;
		this.mInfLabel.Margin = new Vector2(0f, 8f);
		if (this.mKuaFuLueDuoMainInfo != null && this.mKuaFuLueDuoServerInfo != null)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(this.mServerID, out ztBuffServerInfo))
			{
				this.mListTitleServerNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("服务器：") + ztBuffServerInfo.strServerName
				});
			}
			else
			{
				this.mListTitleServerNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("服务器：S") + this.mServerID
				});
			}
			this.mInfLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("当前资源：") + this.mKuaFuLueDuoServerInfo.ZiYuan
			}) + "\n";
		}
		if (this.IsSelfServer())
		{
			UILabel uilabel = this.mInfLabel;
			uilabel.text += Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("暂时获得掠夺资格战盟个数：")
			});
			if (this.mKuaFuLueDuoServerJingJiaState != null)
			{
				if (this.mKuaFuLueDuoServerJingJiaState.State == 1 || this.mKuaFuLueDuoServerJingJiaState.State == 2 || this.mKuaFuLueDuoServerJingJiaState.State == 3)
				{
					if (this.mKuaFuLueDuoServerJingJiaState.JingJiaList != null)
					{
						int num = 0;
						for (int i = 0; i < this.mKuaFuLueDuoServerJingJiaState.JingJiaList.Count; i++)
						{
							if (0 < this.mKuaFuLueDuoServerJingJiaState.JingJiaList[i].BhId)
							{
								num++;
							}
							if (3 <= num)
							{
								break;
							}
						}
						UILabel uilabel2 = this.mInfLabel;
						uilabel2.text = uilabel2.text + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							num.ToString() + Global.GetLang("个")
						}) + "\n";
					}
					else
					{
						UILabel uilabel3 = this.mInfLabel;
						uilabel3.text = uilabel3.text + Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							"0" + Global.GetLang("个")
						}) + "\n";
					}
				}
				else
				{
					UILabel uilabel4 = this.mInfLabel;
					uilabel4.text = uilabel4.text + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						"0" + Global.GetLang("个")
					}) + "\n";
				}
			}
		}
		else if (this.mKuaFuLueDuoMainInfo != null && this.mKuaFuLueDuoServerJingJiaState != null)
		{
			if (this.mKuaFuLueDuoServerJingJiaState.State == 1)
			{
				UILabel uilabel5 = this.mInfLabel;
				uilabel5.text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("暂时获得征讨资格战盟：")
				});
				if (this.mKuaFuLueDuoServerJingJiaState.JingJiaList != null)
				{
					int num2 = 0;
					for (int j = 0; j < this.mKuaFuLueDuoServerJingJiaState.JingJiaList.Count; j++)
					{
						if (this.mKuaFuLueDuoServerJingJiaState.JingJiaList[j] != null)
						{
							if (0 < this.mKuaFuLueDuoServerJingJiaState.JingJiaList[j].BhId)
							{
								num2++;
							}
							if (3 <= num2)
							{
								break;
							}
						}
					}
					UILabel uilabel6 = this.mInfLabel;
					uilabel6.text = uilabel6.text + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						num2.ToString() + Global.GetLang("个")
					}) + "\n";
				}
				else
				{
					UILabel uilabel7 = this.mInfLabel;
					uilabel7.text = uilabel7.text + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						"0" + Global.GetLang("个")
					}) + "\n";
				}
				if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
				{
					this.mInputNum = PlayerPrefs.GetInt(string.Concat(new object[]
					{
						Global.Data.RoleID,
						"_",
						this.mServerID,
						"_LastPludderBiddingMoney"
					}));
					UILabel uilabel8 = this.mInfLabel;
					uilabel8.text = uilabel8.text + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("本战盟竞价：") + this.mInputNum.ToString()
					}) + "\n";
				}
				bool flag = false;
				if (this.mKuaFuLueDuoServerJingJiaState.JingJiaList != null)
				{
					for (int k = 0; k < this.mKuaFuLueDuoServerJingJiaState.JingJiaList.Count; k++)
					{
						if (this.mKuaFuLueDuoServerJingJiaState.JingJiaList[k].BhId == Global.Data.roleData.Faction)
						{
							UILabel uilabel9 = this.mInfLabel;
							uilabel9.text = uilabel9.text + Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								Global.GetLang("竞价状态："),
								"17e43e",
								Global.GetLang("已上榜")
							}) + "\n";
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					UILabel uilabel10 = this.mInfLabel;
					uilabel10.text = uilabel10.text + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("竞价状态："),
						"ff0000",
						Global.GetLang("未上榜")
					}) + "\n";
				}
			}
			else if (this.mKuaFuLueDuoServerJingJiaState.State == 2 || this.mKuaFuLueDuoServerJingJiaState.State == 3 || this.mKuaFuLueDuoServerJingJiaState.State == 4)
			{
				UILabel uilabel11 = this.mInfLabel;
				uilabel11.text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("获得征讨资格战盟：")
				});
				if (this.mKuaFuLueDuoServerJingJiaState.JingJiaList != null)
				{
					int num3 = 0;
					for (int l = 0; l < this.mKuaFuLueDuoServerJingJiaState.JingJiaList.Count; l++)
					{
						if (0 < this.mKuaFuLueDuoServerJingJiaState.JingJiaList[l].BhId)
						{
							ZtBuffServerInfo ztBuffServerInfo2 = null;
							if (Global.GetNowServerIsZhuTiFu(this.mKuaFuLueDuoServerJingJiaState.JingJiaList[l].ZoneId, out ztBuffServerInfo2))
							{
								UILabel uilabel12 = this.mInfLabel;
								uilabel12.text = uilabel12.text + "\n" + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									Global.FormatRoleNameZhuTiFu(ztBuffServerInfo2.strServerName, this.mKuaFuLueDuoServerJingJiaState.JingJiaList[l].BhName, 0)
								});
							}
							else
							{
								UILabel uilabel13 = this.mInfLabel;
								uilabel13.text = uilabel13.text + "\n" + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									"S." + this.mKuaFuLueDuoServerJingJiaState.JingJiaList[l].ZoneId + this.mKuaFuLueDuoServerJingJiaState.JingJiaList[l].BhName
								});
							}
						}
						if (3 <= num3)
						{
							break;
						}
					}
					UILabel uilabel14 = this.mInfLabel;
					uilabel14.text += "\n";
				}
				else
				{
					UILabel uilabel15 = this.mInfLabel;
					uilabel15.text = uilabel15.text + Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						Global.GetLang("暂无")
					}) + "\n";
				}
			}
			else
			{
				UILabel uilabel16 = this.mInfLabel;
				uilabel16.text += Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("获得征讨资格战盟：")
				});
				UILabel uilabel17 = this.mInfLabel;
				uilabel17.text = uilabel17.text + Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"0" + Global.GetLang("个")
				}) + "\n";
				UILabel uilabel18 = this.mInfLabel;
				uilabel18.text = uilabel18.text + Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("本盟竞价：0")
				}) + "\n";
				UILabel uilabel19 = this.mInfLabel;
				uilabel19.text = uilabel19.text + Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("竞价状态：暂无")
				}) + "\n";
			}
		}
		this.RefreshBiddingBtn();
	}

	public void RefreshRightInf()
	{
		this.mRightViewTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("服务器信息")
		});
		this.mRightViewGuanXiLabel.text = string.Empty;
		this.mRightViewRankLabel.text = string.Empty;
		this.mRightViewMingXingLabel.text = string.Empty;
		this.mRightViewLastZhanKuangLabel.text = string.Empty;
		BoxCollider boxCollider = this.mRightViewTitleLabel.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = this.mRightViewTitleLabel.gameObject.AddComponent<BoxCollider>();
		}
		UILabel component = this.mRightViewTitleLabel.GetComponent<UILabel>();
		boxCollider.size = new Vector3(component.relativeSize.x, component.relativeSize.y, 0f);
		boxCollider.center = new Vector3(0f, -component.relativeSize.y / 2f);
		UIEventListener.Get(this.mRightViewTitleLabel.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickLabel);
		this.SetObjPos(this.mRightViewGuanXiLabel);
		if (this.IsSelfServer())
		{
			if (this.mKuaFuLueDuoServerInfo != null)
			{
				if (this.mKuaFuLueDuoServerInfo.ShiChouList == null)
				{
					this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("世仇服务器："),
						"ff0000",
						Global.GetLang("0个")
					});
				}
				else
				{
					this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("世仇服务器："),
						"ff0000",
						this.mKuaFuLueDuoServerInfo.ShiChouList.Count + Global.GetLang("个")
					});
				}
			}
		}
		else
		{
			this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本服关系："),
				"17e43e",
				Global.GetLang("无")
			});
			if (this.mKuaFuLueDuoServerInfo != null)
			{
				if (this.KuaFuLueDuoStateData != null)
				{
					if (this.mKuaFuLueDuoServerInfo.ShiChouList != null && this.mKuaFuLueDuoServerInfo.ShiChouList.Contains(this.KuaFuLueDuoStateData.ServerID))
					{
						this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("本服关系："),
							"17e43e",
							Global.GetLang("已征服")
						});
					}
					if (this.mKuaFuLueDuoServerInfo.ZhengFuList != null && this.mKuaFuLueDuoServerInfo.ZhengFuList.Contains(this.KuaFuLueDuoStateData.ServerID))
					{
						this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("本服关系："),
							"ff0000",
							Global.GetLang("世仇")
						});
					}
				}
			}
			else if (this.KuaFuLueDuoStateData != null)
			{
				KuaFuLueDuoServerInfo kuaFuLueDuoServerInfoDataByID = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(this.KuaFuLueDuoStateData.ServerID);
				if (kuaFuLueDuoServerInfoDataByID != null)
				{
					if (kuaFuLueDuoServerInfoDataByID.ZhengFuList != null && kuaFuLueDuoServerInfoDataByID.ZhengFuList.Contains(this.mServerID))
					{
						this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("本服关系："),
							"17e43e",
							Global.GetLang("已征服")
						});
					}
					if (kuaFuLueDuoServerInfoDataByID.ShiChouList != null && kuaFuLueDuoServerInfoDataByID.ShiChouList.Contains(this.mServerID))
					{
						this.mRightViewGuanXiLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("本服关系："),
							"ff0000",
							Global.GetLang("世仇")
						});
					}
				}
			}
		}
		this.RefreshBoxCollider(this.mRightViewGuanXiLabel);
		this.SetObjPos(this.mRightViewRankLabel);
		if (this.mKuaFuLueDuoServerInfo != null)
		{
			this.mRightViewRankLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("已征服服务器个数："),
				"fdf7dd",
				(this.mKuaFuLueDuoServerInfo.ZhengFuList != null) ? (this.mKuaFuLueDuoServerInfo.ZhengFuList.Count.ToString() + Global.GetLang("个")) : "0"
			}) + "\n";
		}
		long age = -1L;
		if (this.mKuaFuLueDuoRankListCmdData != null)
		{
			age = this.mKuaFuLueDuoRankListCmdData.Age;
		}
		GameInstance.Game.SendGetKuFuPlubdeRankData(age, 0);
		if (this.mKuaFuLueDuoStateData != null)
		{
			age = this.mKuaFuLueDuoStateData.Age;
		}
		GameInstance.Game.SendGetKuFuPlubderGameStateData(age);
		Super.ShowNetWaiting(null);
	}

	private void RefreshRightInf1()
	{
		this.RefreshBoxCollider(this.mRightViewRankLabel);
		this.SetObjPos(this.mRightViewMingXingLabel);
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			KuaFuLueDuoServerInfo kuaFuLueDuoServerInfoDataByID = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(this.mServerID);
			if (kuaFuLueDuoServerInfoDataByID != null)
			{
				string text = string.Empty;
				if (!string.IsNullOrEmpty(kuaFuLueDuoServerInfoDataByID.MingXingZhanMengList))
				{
					List<KuaFuLueDuoRankInfo> list = KuaFuLueDuoServerInfo.MingXingStr2RankList(kuaFuLueDuoServerInfoDataByID.MingXingZhanMengList);
					if (0 < list.Count)
					{
						for (int i = 0; i < list.Count; i++)
						{
							text += Global.GetColorStringForNGUIText(new object[]
							{
								"fdf7dd",
								((!string.IsNullOrEmpty(list[i].Param1)) ? list[i].Param1 : string.Empty) + "\n"
							});
						}
					}
				}
				if (string.IsNullOrEmpty(text))
				{
					text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("暂无")
					});
				}
				this.mRightViewMingXingLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("明星战盟：") + "\n",
					"fdf7dd",
					text
				});
				this.RefreshBoxCollider(this.mRightViewMingXingLabel);
				this.SetObjPos(this.mRightViewLastZhanKuangLabel);
				string text2 = string.Empty;
				text2 = text2 + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("掠夺资源："),
					"fdf7dd",
					kuaFuLueDuoServerInfoDataByID.LastZiYuan
				}) + "\n";
				text2 = text2 + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("防守资源："),
					"fdf7dd",
					kuaFuLueDuoServerInfoDataByID.ZiYuan
				}) + "\n";
				this.mRightViewLastZhanKuangLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("上轮战况：") + "\n",
					"fdf7dd",
					text2
				});
				this.RefreshBoxCollider(this.mRightViewLastZhanKuangLabel);
			}
		}
		this.RrefreshGray();
	}

	private void ClickLabel(GameObject go)
	{
		if (null != go)
		{
			int index = -1;
			if (go.name == this.mRightViewGuanXiLabel.name)
			{
				index = 1;
			}
			else if (!(go.name == this.mRightViewLastZhanKuangLabel.name))
			{
				if (!(go.name == this.mRightViewMingXingLabel.name))
				{
					if (go.name == this.mRightViewRankLabel.name)
					{
						index = 2;
					}
					else if (go.name == this.mRightViewTitleLabel.name)
					{
						index = 0;
					}
				}
			}
			this.ShowTipsView(index);
		}
	}

	private void RefreshBiddingBtn()
	{
		if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction))
		{
			if (this.mKuaFuLueDuoServerJingJiaState != null)
			{
				if (this.mKuaFuLueDuoServerJingJiaState.State != 1)
				{
					this.mGoBiddingBtn.gameObject.SetActive(false);
				}
				else
				{
					this.mGoBiddingBtn.gameObject.SetActive(!this.IsSelfServer());
				}
			}
		}
		else
		{
			this.mGoBiddingBtn.gameObject.SetActive(false);
		}
	}

	private void ShowTipsView(int index)
	{
		if (this.mTipsRoot.activeSelf)
		{
			this.CloseTips();
		}
		this.mTipsRoot.SetActive(true);
		this.mTipsInfLabel2.text = string.Empty;
		this.mTipsInfLabel1.text = string.Empty;
		KuaFuLueDuoStateData kuaFuLueDuoStateData = KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData;
		if (index == 0)
		{
			this.mTipsInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("服务器信息")
			});
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			if (this.mKuaFuLueDuoServerInfo.ZoneIdRangeList != null && 0 < this.mKuaFuLueDuoServerInfo.ZoneIdRangeList.Count)
			{
				list2.AddRange(this.mKuaFuLueDuoServerInfo.ZoneIdRangeList);
			}
			if (list2 != null && 1 <= list2.Count)
			{
				for (int i = 0; i < list2.Count; i += 2)
				{
					if (i < list2.Count - 1 && list2[i] != list2[i + 1])
					{
						ZtBuffServerInfo ztBuffServerInfo = null;
						ZtBuffServerInfo ztBuffServerInfo2 = null;
						if (Global.GetNowServerIsZhuTiFu(list2[i], out ztBuffServerInfo) && Global.GetNowServerIsZhuTiFu(list2[i + 1], out ztBuffServerInfo2))
						{
							list.Add(ztBuffServerInfo.strServerName);
							list.Add(ztBuffServerInfo2.strServerName);
						}
						else
						{
							list.Add(list2[i].ToString() + Global.GetLang("-") + list2[i + 1].ToString() + Global.GetLang("区"));
						}
					}
					else
					{
						ZtBuffServerInfo ztBuffServerInfo3 = null;
						if (Global.GetNowServerIsZhuTiFu(list2[i], out ztBuffServerInfo3))
						{
							list.Add(ztBuffServerInfo3.strServerName);
						}
						else
						{
							list.Add(list2[i].ToString() + Global.GetLang("区"));
						}
					}
				}
			}
			else if (list2.Count == 1)
			{
				if (kuaFuLueDuoStateData != null)
				{
					if (kuaFuLueDuoStateData.ServerID == list2[0])
					{
						ZtBuffServerInfo ztBuffServerInfo4 = null;
						if (Global.GetNowServerIsZhuTiFu(list2[0], out ztBuffServerInfo4))
						{
							list.Add(ztBuffServerInfo4.strServerName);
						}
						else
						{
							list.Add(list2[0].ToString());
						}
					}
					else
					{
						ZtBuffServerInfo ztBuffServerInfo5 = null;
						if (Global.GetNowServerIsZhuTiFu(list2[0], out ztBuffServerInfo5))
						{
							list.Add(ztBuffServerInfo5.strServerName);
						}
						else
						{
							list.Add(list2[0].ToString() + Global.GetLang("区"));
						}
						if (Global.GetNowServerIsZhuTiFu(kuaFuLueDuoStateData.ServerID, out ztBuffServerInfo5))
						{
							list.Add(ztBuffServerInfo5.strServerName);
						}
						else
						{
							list.Add(kuaFuLueDuoStateData.ServerID.ToString() + Global.GetLang("区"));
						}
					}
				}
				else
				{
					ZtBuffServerInfo ztBuffServerInfo6 = null;
					if (Global.GetNowServerIsZhuTiFu(list2[0], out ztBuffServerInfo6))
					{
						list.Add(ztBuffServerInfo6.strServerName);
					}
					else
					{
						list.Add(list2[0].ToString() + Global.GetLang("区"));
					}
					if (Global.GetNowServerIsZhuTiFu(kuaFuLueDuoStateData.ServerID, out ztBuffServerInfo6))
					{
						list.Add(ztBuffServerInfo6.strServerName);
					}
					else
					{
						list.Add(kuaFuLueDuoStateData.ServerID.ToString() + Global.GetLang("区"));
					}
				}
			}
			else if (kuaFuLueDuoStateData != null)
			{
				ZtBuffServerInfo ztBuffServerInfo7 = null;
				if (Global.GetNowServerIsZhuTiFu(kuaFuLueDuoStateData.ServerID, out ztBuffServerInfo7))
				{
					list.Add(ztBuffServerInfo7.strServerName);
				}
				else
				{
					list.Add(kuaFuLueDuoStateData.ServerID + Global.GetLang("区"));
				}
			}
			this.mTipsInfLabel2.Margin = new Vector2(1f, 8f);
			for (int j = 0; j < list.Count; j++)
			{
				UILabel uilabel = this.mTipsInfLabel2;
				uilabel.text = uilabel.text + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					list[j]
				}) + "\n";
			}
		}
		else if (index == 1)
		{
			if (this.IsSelfServer())
			{
				this.mTipsInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("世仇服务器")
				});
				List<string> list3 = new List<string>();
				List<int> list4 = new List<int>();
				if (this.mKuaFuLueDuoServerInfo.ShiChouList != null && 0 < this.mKuaFuLueDuoServerInfo.ShiChouList.Count)
				{
					list4.AddRange(this.mKuaFuLueDuoServerInfo.ShiChouList);
				}
				if (list4 != null && 1 <= list4.Count)
				{
					for (int k = 0; k < list4.Count; k += 2)
					{
						if (k < list4.Count - 1 && list4[k] != list4[k + 1])
						{
							ZtBuffServerInfo ztBuffServerInfo8 = null;
							ZtBuffServerInfo ztBuffServerInfo9 = null;
							if (Global.GetNowServerIsZhuTiFu(list4[k], out ztBuffServerInfo8) && Global.GetNowServerIsZhuTiFu(list4[k + 1], out ztBuffServerInfo9))
							{
								list3.Add(ztBuffServerInfo8.strServerName);
								list3.Add(ztBuffServerInfo9.strServerName);
							}
							else
							{
								list3.Add(list4[k].ToString() + Global.GetLang("|") + list4[k + 1].ToString() + Global.GetLang("区"));
							}
						}
						else
						{
							ZtBuffServerInfo ztBuffServerInfo10 = null;
							if (Global.GetNowServerIsZhuTiFu(list4[k], out ztBuffServerInfo10))
							{
								list3.Add(ztBuffServerInfo10.strServerName);
							}
							else
							{
								list3.Add(list4[k].ToString() + Global.GetLang("区"));
							}
						}
					}
				}
				this.mTipsInfLabel2.Margin = new Vector2(1f, 8f);
				for (int l = 0; l < list3.Count; l++)
				{
					UILabel uilabel2 = this.mTipsInfLabel2;
					uilabel2.text = uilabel2.text + Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						list3[l]
					}) + "\n";
				}
			}
			else
			{
				this.CloseTips();
			}
		}
		else if (index == 2)
		{
			this.mTipsInfLabel1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("已征服服务器")
			});
			if (this.mKuaFuLueDuoServerInfo != null)
			{
				if (this.mKuaFuLueDuoServerInfo.ZhengFuList != null && 0 < this.mKuaFuLueDuoServerInfo.ZhengFuList.Count)
				{
					List<string> list5 = new List<string>();
					List<int> list6 = new List<int>();
					list6.AddRange(this.mKuaFuLueDuoServerInfo.ZhengFuList);
					if (list6 != null && 1 <= list6.Count)
					{
						for (int m = 0; m < list6.Count; m += 2)
						{
							if (m < list6.Count - 1 && list6[m] != list6[m + 1])
							{
								ZtBuffServerInfo ztBuffServerInfo11 = null;
								ZtBuffServerInfo ztBuffServerInfo12 = null;
								if (Global.GetNowServerIsZhuTiFu(list6[m], out ztBuffServerInfo11) && Global.GetNowServerIsZhuTiFu(list6[m + 1], out ztBuffServerInfo12))
								{
									list5.Add(ztBuffServerInfo11.strServerName);
									list5.Add(ztBuffServerInfo12.strServerName);
								}
								else
								{
									list5.Add(list6[m].ToString() + Global.GetLang("|") + list6[m + 1].ToString() + Global.GetLang("区"));
								}
							}
							else
							{
								ZtBuffServerInfo ztBuffServerInfo13 = null;
								if (Global.GetNowServerIsZhuTiFu(list6[m], out ztBuffServerInfo13))
								{
									list5.Add(ztBuffServerInfo13.strServerName);
								}
								else
								{
									list5.Add(list6[m].ToString() + Global.GetLang("区"));
								}
							}
						}
					}
					else if (list6.Count == 1)
					{
						if (kuaFuLueDuoStateData != null)
						{
							ZtBuffServerInfo ztBuffServerInfo14 = null;
							if (Global.GetNowServerIsZhuTiFu(list6[0], out ztBuffServerInfo14))
							{
								if (kuaFuLueDuoStateData.ServerID == list6[0])
								{
									list5.Add(ztBuffServerInfo14.strServerName);
								}
								else
								{
									list5.Add(ztBuffServerInfo14.strServerName);
									list5.Add(ztBuffServerInfo14.strServerName);
								}
							}
							else if (kuaFuLueDuoStateData.ServerID == list6[0])
							{
								list5.Add(list6[0].ToString());
							}
							else
							{
								list5.Add(list6[0].ToString() + Global.GetLang("区"));
								list5.Add(kuaFuLueDuoStateData.ServerID.ToString() + Global.GetLang("区"));
							}
						}
					}
					this.mTipsInfLabel2.Margin = new Vector2(1f, 8f);
					for (int n = 0; n < list5.Count; n++)
					{
						UILabel uilabel3 = this.mTipsInfLabel2;
						uilabel3.text = uilabel3.text + Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							list5[n]
						}) + "\n";
					}
				}
				else
				{
					UILabel uilabel4 = this.mTipsInfLabel2;
					uilabel4.text = uilabel4.text + Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("暂无")
					}) + "\n";
				}
			}
			else
			{
				UILabel uilabel5 = this.mTipsInfLabel2;
				uilabel5.text = uilabel5.text + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("暂无")
				}) + "\n";
			}
		}
		else
		{
			this.CloseTips();
		}
		Vector3 localScale = this.mBgSP.transform.localScale;
		localScale.y = 40f + this.mTipsInfLabel2.relativeSize.y * this.mTipsInfLabel2.transform.localScale.y;
		this.mBgSP.transform.localScale = localScale;
	}

	private void CloseTips()
	{
		this.mTipsInfLabel1.text = string.Empty;
		this.mTipsInfLabel2.text = string.Empty;
		this.mTipsRoot.SetActive(false);
	}

	private new void Update()
	{
		if (null != this.mModal._Target && this.mModal._Target.transform.localRotation != Global.GetQuaternionByDir(5))
		{
			this.mModal._Target.transform.localRotation = Global.GetQuaternionByDir(5);
		}
	}

	public void RefreshData()
	{
		if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderMap)
		{
			this.mKuaFuLueDuoGameStates = KuaFuPlunderMap.GetInstance().KuaFuLueDuoGameStates;
			this.mKuaFuLueDuoMainInfo = KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo;
			this.mKuaFuLueDuoServerInfo = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(this.mServerID);
			this.mKuaFuLueDuoServerJingJiaState = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(this.mServerID);
			this.RefreshRightInf();
			this.RefreshLiftInf();
		}
		else if (this.mKuaFuLueDuoMainInfo != null)
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(this.mKuaFuLueDuoMainInfo.ServerListAge, this.mKuaFuLueDuoMainInfo.StateListAge);
		}
		else
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
		}
	}

	public void NoticeGetSeverDataCallBack(KuaFuLueDuoMainInfo data)
	{
		bool flag = false;
		byte b = 0;
		byte b2 = 0;
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			if (this.mKuaFuLueDuoMainInfo.ServerListAge != data.ServerListAge)
			{
				this.mKuaFuLueDuoMainInfo.ServerList = data.ServerList;
				this.mKuaFuLueDuoMainInfo.ServerListAge = data.ServerListAge;
				b = 1;
				flag = true;
			}
			if (this.mKuaFuLueDuoMainInfo.StateListAge != data.StateListAge)
			{
				this.mKuaFuLueDuoMainInfo.StateList = data.StateList;
				this.mKuaFuLueDuoMainInfo.StateListAge = data.StateListAge;
				this.mKuaFuLueDuoMainInfo.JingJiaData = data.JingJiaData;
				b = 1;
				b2 = 1;
				flag = true;
			}
		}
		else
		{
			flag = true;
			b = 1;
			b2 = 1;
			this.mKuaFuLueDuoMainInfo = data;
		}
		if (flag)
		{
			if (b == 1)
			{
				this.mKuaFuLueDuoServerInfo = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(this.mServerID);
			}
			if (b2 == 1)
			{
				this.mKuaFuLueDuoServerJingJiaState = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(this.mServerID);
			}
			this.RefreshRightInf();
			this.RefreshLiftInf();
		}
	}

	public void NoticeGetGameStateCallBack(KuaFuLueDuoStateData State)
	{
		bool flag = false;
		if (State != null)
		{
			if (this.mKuaFuLueDuoStateData != null)
			{
				if (this.mKuaFuLueDuoStateData.Age != State.Age)
				{
					flag = true;
					this.mKuaFuLueDuoStateData = State;
				}
			}
			else
			{
				flag = true;
				this.mKuaFuLueDuoStateData = State;
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>KuaFuLueDuoStateData = null</color>"
			});
		}
		this.mKuaFuLueDuoGameStates = this.mKuaFuLueDuoStateData.GameState;
		if (flag)
		{
			this.RefreshLiftInf();
			this.RefreshBiddingBtn();
			if (this.mServerID == this.mKuaFuLueDuoStateData.ServerID)
			{
				this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("本服信息")
				});
			}
		}
	}

	public void NoticeGetRankDataCallBack(KuaFuLueDuoRankListCmdData data)
	{
		this.mKuaFuLueDuoRankListCmdData = data;
		if (data != null)
		{
			if (this.mKuaFuLueDuoRankListCmdData != null)
			{
				if (data.Age != this.mKuaFuLueDuoRankListCmdData.Age)
				{
					this.mKuaFuLueDuoRankListCmdData = data;
				}
			}
			else
			{
				this.mKuaFuLueDuoRankListCmdData = data;
			}
		}
		if (this.mKuaFuLueDuoRankListCmdData != null)
		{
			if (this.mKuaFuLueDuoRankListCmdData.ListRankList != null)
			{
				int num = -1;
				for (int i = 0; i < this.mKuaFuLueDuoRankListCmdData.ListRankList.Count; i++)
				{
					if (this.mKuaFuLueDuoRankListCmdData.ListRankList[i].Key == this.mServerID)
					{
						num = i + 1;
						break;
					}
				}
				if (num == -1)
				{
					UILabel uilabel = this.mRightViewRankLabel;
					uilabel.text += Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("征服排行榜："),
						"fdf7dd",
						Global.GetLang("未上榜")
					});
				}
				else
				{
					UILabel uilabel2 = this.mRightViewRankLabel;
					uilabel2.text += Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("征服排行榜："),
						"fdf7dd",
						num + Global.GetLang("名")
					});
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>mKuaFuLueDuoRankListCmdData.ListRankList = null</color>"
				});
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>KuaFuLueDuoRankListCmdData = null</color>"
			});
		}
		this.RefreshRightInf1();
	}

	private void RrefreshGray()
	{
		bool flag = false;
		if (this.mKuaFuLueDuoServerJingJiaState != null && (this.mKuaFuLueDuoServerJingJiaState.State == 2 || this.mKuaFuLueDuoServerJingJiaState.State == 3) && !this.IsSelfServer())
		{
			flag = true;
		}
		if (null != this.mModal._Target)
		{
			Renderer component = this.mModal._Target.GetComponent<Renderer>();
			if (null != component)
			{
				if (flag)
				{
					component.sharedMaterial.EnableKeyword("_GRAY");
				}
				else
				{
					component.sharedMaterial.DisableKeyword("_GRAY");
				}
			}
		}
		this.mModal._Target.transform.localRotation = Global.GetQuaternionByDir(5);
	}

	public byte Type
	{
		set
		{
			this.mType = value;
			this.mModal.CanRotate = false;
			UIHelper.LoadNPCRes(this.mModal, this.GetNpcId(this.mType), 9f, delegate(MonsterNPCLoaderData loader, GameObject go)
			{
				if (null == go)
				{
					return;
				}
				if (loader == null)
				{
					return;
				}
				int monsterID = loader.MonsterID;
				if (monsterID > 0)
				{
					go.AddComponent<LoadUIShaderAgain>();
				}
				Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(loader.parent);
				if (null != modal3DShow)
				{
					U3DUtils.AddChild(modal3DShow.gameObject, go, false);
					modal3DShow._Target = go;
					go.name = "UI_Boss_Monster_" + loader.MonsterID;
					go.transform.localScale = Vector3.one * loader.scale;
				}
				U3DUtils.ReplaceLayerInChildren(go, loader.layer, null);
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(monsterID);
				KuaFuPlunderServerInFPart.ReplaceMaterials(go, npcvobyID.ShaderID);
				Renderer componentInChildren = go.GetComponentInChildren<Renderer>();
				if (null != componentInChildren)
				{
					Shader shader = Shader.Find(componentInChildren.sharedMaterial.shader.name + "ForUI");
					componentInChildren.sharedMaterial.shader = shader;
					string name = componentInChildren.sharedMaterial.shader.name;
					if (name.EndsWith("ErrorShader"))
					{
						componentInChildren.sharedMaterial.shader = Shader.Find("Custom/Mobile/DiffuseForUI");
					}
				}
				this.RrefreshGray();
				go.transform.localRotation = Global.GetQuaternionByDir(5);
				this.mModal.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			});
		}
	}

	public static void ReplaceMaterials(GameObject go, int shaderID)
	{
		if (go == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				Global.GetLang("ReplaceMaterials对象为空,不应该为空")
			});
			return;
		}
		Renderer[] componentsInChildren;
		if (shaderID <= 0)
		{
			componentsInChildren = go.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
					for (int j = 0; j < sharedMaterials.Length; j++)
					{
						Texture texture = sharedMaterials[j].GetTexture("_MainTex");
						if (texture.name.IndexOf("_alpha") >= 0)
						{
							sharedMaterials[j].shader = Shader.Find("Custom/Mobile/Diffuse");
						}
						else
						{
							sharedMaterials[j].shader = Shader.Find(componentsInChildren[i].sharedMaterial.shader.name);
						}
					}
				}
			}
			return;
		}
		componentsInChildren = go.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				Renderer renderer = componentsInChildren[k];
				if (!(null == renderer))
				{
					if (!(renderer is ParticleRenderer) && !(renderer is ParticleSystemRenderer))
					{
						Material[] array = new Material[renderer.sharedMaterials.Length];
						for (int l = 0; l < renderer.sharedMaterials.Length; l++)
						{
							if (k != 0 && (renderer.sharedMaterials[l].shader.name.StartsWith("FXMaker/Mask") || renderer.sharedMaterials[l].shader.name.StartsWith("Custom/Mobile/Particles") || renderer.sharedMaterials[l].shader.name.StartsWith("Mobile/Particles/Additive Culled") || renderer.sharedMaterials[l].shader.name.StartsWith("Artist/Tint Particle") || renderer.sharedMaterials[l].shader.name.StartsWith("ZombieStyle/MobileRimDiffuseCutoutAlpha")))
							{
								array[l] = renderer.sharedMaterials[l];
							}
							else
							{
								Texture texture2 = renderer.sharedMaterials[l].GetTexture("_MainTex");
								Material materialReflByShaderID = U3DUtils.GetMaterialReflByShaderID(renderer.sharedMaterials[l], shaderID, texture2.name.IndexOf("_alpha") >= 0);
								if (null == materialReflByShaderID)
								{
									array[l] = materialReflByShaderID;
								}
								else
								{
									materialReflByShaderID.SetTexture("_MainTex", texture2);
									array[l] = materialReflByShaderID;
								}
							}
						}
						componentsInChildren[k].materials = array;
					}
				}
			}
		}
	}

	private int ModeDir
	{
		set
		{
			this.mModeDir = value;
			if (null != this.mModal && null != this.mModal._Target)
			{
				this.mModal._Target.transform.localRotation = Global.GetQuaternionByDir(5);
				this.mModal.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
	}

	public int ServerID
	{
		set
		{
			this.mServerID = value;
			this.RefreshData();
			if (this.mKuaFuLueDuoMainInfo != null && this.mKuaFuLueDuoMainInfo.JingJiaData != null)
			{
				this.mInputNum = PlayerPrefs.GetInt(string.Concat(new object[]
				{
					Global.Data.RoleID,
					"_",
					this.mServerID,
					"_LastPludderBiddingMoney"
				}));
			}
			this.StartTimeTicks();
			this.Type = KuaFuPlunderMap.GetInstance().GetModalTypeByServerId(value);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime minValue = DateTime.MinValue;
			string @string = PlayerPrefs.GetString("strPlunderBiddingTimeTicks" + Global.Data.RoleID);
			long num = -1L;
			if ("0" != @string && long.TryParse(@string, ref num))
			{
				minValue..ctor(num);
				if (3.0 <= (correctDateTime - minValue).TotalHours)
				{
					PlayerPrefs.SetInt(string.Concat(new object[]
					{
						Global.Data.RoleID,
						"_",
						this.mServerID,
						"_LastPludderBiddingMoney"
					}), 0);
				}
			}
			int modeDir = 5;
			this.ModeDir = modeDir;
		}
	}

	internal void NoticeBiddingCallBack(int ret)
	{
		if (0 > ret)
		{
			MUDebug.Log<string>(new string[]
			{
				"0000      " + PlayerPrefs.GetInt(string.Concat(new object[]
				{
					Global.Data.RoleID,
					"_",
					this.mServerID,
					"_LastPludderBiddingMoney"
				}))
			});
			if (ret == -1043 || ret == -1041)
			{
				PlayerPrefs.SetString("strPlunderBiddingTimeTicks" + Global.Data.RoleID, "0");
				PlayerPrefs.SetInt(string.Concat(new object[]
				{
					Global.Data.RoleID,
					"_",
					this.mServerID,
					"_LastPludderBiddingMoneyThis"
				}), 0);
				MUDebug.Log<string>(new string[]
				{
					"0000      "
				});
			}
			else
			{
				PlayerPrefs.SetInt(string.Concat(new object[]
				{
					Global.Data.RoleID,
					"_",
					this.mServerID,
					"_LastPludderBiddingMoney"
				}), PlayerPrefs.GetInt(string.Concat(new object[]
				{
					Global.Data.RoleID,
					"_",
					this.mServerID,
					"_LastPludderBiddingMoneyThis"
				})));
				MUDebug.Log<string>(new string[]
				{
					"2222      "
				});
			}
			Super.HintMainText(StdErrorCode.GetErrMsg(ret, false, false), 10, 3);
			MUDebug.Log<string>(new string[]
			{
				"111      " + PlayerPrefs.GetInt(string.Concat(new object[]
				{
					Global.Data.RoleID,
					"_",
					this.mServerID,
					"_LastPludderBiddingMoney"
				}))
			});
		}
		else
		{
			PlayerPrefs.SetInt(string.Concat(new object[]
			{
				Global.Data.RoleID,
				"_",
				this.mServerID,
				"_LastPludderBiddingMoney"
			}), PlayerPrefs.GetInt(string.Concat(new object[]
			{
				Global.Data.RoleID,
				"_",
				this.mServerID,
				"_LastPludderBiddingMoneyThis"
			})));
			Super.HintMainText(Global.GetLang("竞价成功临时获得入侵资格"), 10, 3);
			if (this.mKuaFuLueDuoMainInfo != null)
			{
				GameInstance.Game.SendGetKuFuPlubderServerDataList(this.mKuaFuLueDuoMainInfo.ServerListAge, this.mKuaFuLueDuoMainInfo.StateListAge);
			}
			else
			{
				GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
			}
		}
	}

	private const int ServerNameLabel = 0;

	private const int TimeLiftlabel = 1;

	private const int Reslabel = 2;

	private const int ServerStateValuelabel = 3;

	private const int SelfBiddingValuelabel = 4;

	private const int biddingstate = 5;

	private const int LineHight = 18;

	private const string mLastPludderBiddingMoneyThis = "LastPludderBiddingMoneyThis";

	private const string mLastPludderBiddingMoney = "LastPludderBiddingMoney";

	private const string mPlunderBiddingTimeTicksKey = "strPlunderBiddingTimeTicks";

	[SerializeField]
	private GameObject mBgGameObject;

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private UILabel mTitleLabel;

	[SerializeField]
	private ShowNetImage mBgImage;

	[SerializeField]
	private UILabel mListTitleServerNameLabel;

	[SerializeField]
	private GButton mGoBiddingBtn;

	[SerializeField]
	private UILabel mRightViewTitleLabel;

	[SerializeField]
	private UILabel mRightViewGuanXiLabel;

	[SerializeField]
	private UILabel mRightViewRankLabel;

	[SerializeField]
	private UILabel mRightViewMingXingLabel;

	[SerializeField]
	private UILabel mRightViewLastZhanKuangLabel;

	[SerializeField]
	private GameObject mTipsRoot;

	[SerializeField]
	private UISprite mBgSP;

	[SerializeField]
	private UILabel mInfLabel;

	[SerializeField]
	private UILabel mInfLabel1;

	[SerializeField]
	private UILabel mTipsInfLabel1;

	[SerializeField]
	private UILabel mTipsInfLabel2;

	[SerializeField]
	private Modal3DShow mModal;

	private int mInputNum;

	private DispatcherTimer mDispatcherTimer;

	private int mServerID;

	private KuaFuLueDuoMainInfo mKuaFuLueDuoMainInfo;

	private CrusadeWarXml mCrusadeWarXml;

	private KuaFuLueDuoServerInfo mKuaFuLueDuoServerInfo;

	private KuaFuLueDuoServerJingJiaState mKuaFuLueDuoServerJingJiaState;

	private KuaFuLueDuoRankListCmdData mKuaFuLueDuoRankListCmdData;

	private KuaFuLueDuoGameStates mKuaFuLueDuoGameStates = 4;

	private KuaFuLueDuoStateData mKuaFuLueDuoStateData;

	private byte mType;

	private int mModeDir;

	public DPSelectedItemEventHandler Hander;
}
