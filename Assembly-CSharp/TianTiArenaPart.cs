using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TianTiArenaPart : UserControl
{
	protected override void InitializeComponent()
	{
		TianTiArenaPart.Instance = this;
		base.InitializeComponent();
		try
		{
			this.m_LabelDuanWei.text = string.Empty;
			this.m_LabelShengLv.text = string.Empty;
			this.m_LabelLianSheng.text = string.Empty;
			this.m_LabelRongYaoChangCiShu.text = string.Empty;
			this.labRule.pivot = 3;
			this.labRule.transform.localPosition = new Vector3(-195f, this.labRule.transform.localPosition.y, this.labRule.transform.localPosition.z);
			this.m_LabelDuanWei.transform.localPosition = new Vector3(-230f, 232f, 0f);
			this.m_LabelTime.transform.localPosition = new Vector3(67f, 60f, -1f);
			this.m_LabelShengLv.text = string.Empty;
			this.m_LabelLianSheng.text = string.Empty;
			this.m_LabelRongYaoChangCiShu.text = string.Empty;
			this.labRule.text = string.Concat(new string[]
			{
				Global.GetLang("{fac60d}跨服天梯赛规则：{-}"),
				"\n",
				Global.GetLang("{dac7ae}1、积分规则 {-}"),
				"\n",
				Global.GetLang("{dac7ae}     胜利根据对方段位{17e43e}获得积分{-}，失败根{-}"),
				"\n",
				Global.GetLang("{dac7ae}     据自身段位{ff0000}扣除积分{-}，任何一方异常{-}"),
				"\n",
				Global.GetLang("{dac7ae}     未进入战场，双方不获得积分。{-}"),
				"\n",
				Global.GetLang("{dac7ae}2、新手保护{-}"),
				"\n",
				Global.GetLang("{dac7ae}     青铜1-5段的玩家失败{17e43e}不扣除积分{-}{-}"),
				"\n",
				Global.GetLang("{dac7ae}3、荣耀规则{-}"),
				"\n",
				Global.GetLang("{dac7ae}     每日前{17e43e}5次{-}战斗会获得荣耀点数，荣耀{-}"),
				"\n",
				Global.GetLang("{dac7ae}     点数可在荣耀商店中兑换珍贵道具 {-}"),
				"\n",
				Global.GetLang("{dac7ae}4、排行奖励{-}"),
				"\n",
				Global.GetLang("{dac7ae}     每月一号{17e43e}凌晨3点{-}刷新排行榜，根据排{-}"),
				"\n",
				Global.GetLang("{dac7ae}     行名次获得奖励{-}"),
				"\n",
				Global.GetLang("{dac7ae}5、天梯赛段位将于每月领取段位奖励时清零")
			});
			this.m_quxiao.text = Global.GetLang("取消");
			this.paimingLabs[0].transform.localPosition = new Vector3(338f, 60f, 0f);
			this.paimingLabs[1].transform.localPosition = new Vector3(17f, 60f, 0f);
			this.paimingLabs[2].transform.localPosition = new Vector3(-255f, 60f, 0f);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"越南用，可能预制报空"
			});
		}
		UIEventListener.Get(this.m_BtnClose.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, null);
		};
		UIEventListener.Get(this.btnZhanbao.gameObject).onClick = delegate(GameObject s)
		{
			this.GChildTiantiZhanbaoPart = U3DUtils.NEW<GChildWindow>();
			this.GChildTiantiZhanbaoPart.ModalType = ChildWindowModalType.Translucent;
			this.GChildTiantiZhanbaoPart.Modal = true;
			this.GChildTiantiZhanbaoPart.IsShowModal = true;
			Super.InitChildWindow(this.GChildTiantiZhanbaoPart, "GChildTiantiZhanbaoPart");
			Super.GData.GlobalPlayZone.Children.Add(this.GChildTiantiZhanbaoPart);
			this.m_TianTiZhanbaoPart = U3DUtils.NEW<TianTiZhanbaoPart>();
			this.GChildTiantiZhanbaoPart.Body.Add(this.m_TianTiZhanbaoPart);
			this.m_TianTiZhanbaoPart.transform.localPosition = new Vector3(0f, 0f, -100f);
			this.m_TianTiZhanbaoPart.transform.localScale = new Vector3(1f, 1f, 1f);
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(10000f, 10f, 0f);
		};
		UIEventListener.Get(this.BtnJoin.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData == null)
			{
				return;
			}
			if (KuafuJoinPart.IsBaoMing == Global.Data.roleData.RoleID)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("您已报名了幻影寺院活动，不能同时报名此活动"), 0, -1, -1, 0);
				return;
			}
			int mapCode = Global.Data.roleData.MapCode;
			if (ConfigSystemParam.GetSystemParamIntArrayByName("TianTiMap", ',').IndexOf(mapCode) < 0)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("此地图不能进入天梯系统"), 0, -1, -1, 0);
				return;
			}
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						string systemParamByName2 = ConfigSystemParam.GetSystemParamByName("MaxTianTiJiFen", true);
						int num2 = (!string.IsNullOrEmpty(systemParamByName2)) ? ConvertExt.SafeConvertToInt32(systemParamByName2) : 600000;
						if (this.TianTiDataAndDayPaiHangDataBag != null && this.TianTiDataAndDayPaiHangDataBag.TianTiData != null && num2 <= this.TianTiDataAndDayPaiHangDataBag.TianTiData.DayDuanWeiJiFen)
						{
							DPSelectedItemEventHandler handler2 = delegate(object sender, DPSelectedItemEventArgs args)
							{
								if (args.ID == 0)
								{
									this.AddInitInfo();
								}
							};
							string lang2 = Global.GetLang("您已达到本日获取积分上限，继续挑战不会获得积分奖励，是否继续挑战？");
							Super.ShowMessageBoxEx(Global.GetLang("提示"), lang2, handler2, new string[]
							{
								Global.GetLang("确定"),
								Global.GetLang("取消")
							});
						}
						else
						{
							this.AddInitInfo();
						}
					}
				}, -1);
			}
			else
			{
				string systemParamByName = ConfigSystemParam.GetSystemParamByName("MaxTianTiJiFen", true);
				int num = (!string.IsNullOrEmpty(systemParamByName)) ? ConvertExt.SafeConvertToInt32(systemParamByName) : 600000;
				if (this.TianTiDataAndDayPaiHangDataBag != null && this.TianTiDataAndDayPaiHangDataBag.TianTiData != null && num <= this.TianTiDataAndDayPaiHangDataBag.TianTiData.DayDuanWeiJiFen)
				{
					DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
					{
						if (args.ID == 0)
						{
							this.AddInitInfo();
						}
					};
					string lang = Global.GetLang("您已达到本日获取积分上限，继续挑战不会获得积分奖励，是否继续挑战？");
					Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, handler, new string[]
					{
						Global.GetLang("确定"),
						Global.GetLang("取消")
					});
				}
				else
				{
					this.AddInitInfo();
				}
			}
		};
		UIEventListener.Get(this.m_BtnQuXiao.gameObject).onClick = delegate(GameObject s)
		{
			this.m_XunZhaoDuiShou.gameObject.SetActive(false);
			TCPGameServerCmds.CMD_SPR_TIANTI_QUIT.SendDataUseRoleID();
			base.StopAllCoroutines();
			if (this.m_BoolPiPei)
			{
				string strcmd = StringUtil.substitute("{0}:{1}", new object[]
				{
					Global.Data.roleData.RoleID,
					0
				});
				TCPGameServerCmds.CMD_SPR_TIANTI_ENTER.SendData(strcmd);
			}
			this.m_Lab10Time.gameObject.SetActive(this.m_BoolPiPei);
		};
		UIEventListener.Get(this.btnPaihang.gameObject).onClick = delegate(GameObject s)
		{
			this.GChildTiantiPaihangPart = U3DUtils.NEW<GChildWindow>();
			this.GChildTiantiPaihangPart.ModalType = ChildWindowModalType.Translucent;
			this.GChildTiantiPaihangPart.Modal = true;
			this.GChildTiantiPaihangPart.IsShowModal = true;
			Super.InitChildWindow(this.GChildTiantiPaihangPart, "GChildTiantiPaihangPart");
			Super.GData.GlobalPlayZone.Children.Add(this.GChildTiantiPaihangPart);
			this.m_TianTiDuanWeiPart = U3DUtils.NEW<TianTiDuanWeiPart>();
			this.GChildTiantiPaihangPart.Body.Add(this.m_TianTiDuanWeiPart);
			this.m_TianTiDuanWeiPart.transform.localPosition = new Vector3(0f, 0f, -100f);
			this.m_TianTiDuanWeiPart.transform.localScale = new Vector3(1f, 1f, 1f);
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(10000f, 10f, 0f);
		};
		UIEventListener.Get(this.m_BtnShiPin.gameObject).onClick = delegate(GameObject s)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 4
				});
			}
		};
		UIEventListener.Get(this.btnJiangli.gameObject).onClick = delegate(GameObject s)
		{
			this.GChildTiantiDuiHuanPart = U3DUtils.NEW<GChildWindow>();
			this.GChildTiantiDuiHuanPart.ModalType = ChildWindowModalType.Translucent;
			this.GChildTiantiDuiHuanPart.Modal = true;
			this.GChildTiantiDuiHuanPart.IsShowModal = true;
			Super.InitChildWindow(this.GChildTiantiDuiHuanPart, "GChildTiantiDuiHuanPart");
			Super.GData.GlobalPlayZone.Children.Add(this.GChildTiantiDuiHuanPart);
			this.m_MUDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
			this.GChildTiantiDuiHuanPart.Body.Add(this.m_MUDuiHuanPart);
			this.m_MUDuiHuanPart.transform.localPosition = new Vector3(0f, 0f, -100f);
			this.m_MUDuiHuanPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.m_MUDuiHuanPart.InitPartData(4, 0);
			this.m_MUDuiHuanPart.valueText.text = Global.Data.roleData.TianTiRongYao + string.Empty;
			this.m_MUDuiHuanPart.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e)
			{
				TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(0f, 10f, 0f);
				Object.Destroy(this.GChildTiantiDuiHuanPart.gameObject);
				return false;
			};
			this.m_MUDuiHuanPart.SpriteType1.enabled = false;
			this.m_MUDuiHuanPart.SpriteType2.enabled = true;
			this.m_MUDuiHuanPart.Title1.enabled = false;
			this.m_MUDuiHuanPart.Title2.enabled = true;
			this.m_MUDuiHuanPart.hintText.text = string.Empty;
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(10000f, 10f, 0f);
		};
		UIEventListener.Get(this.m_BtnTianTiGuiZe.gameObject).onClick = delegate(GameObject s)
		{
			this.m_Rule.gameObject.SetActive(true);
			UIEventListener.Get(this.m_BtnCloseRule.gameObject).onClick = delegate(GameObject s1)
			{
				this.m_Rule.gameObject.SetActive(false);
			};
		};
		UIEventListener.Get(this.m_BtnTianTiJiangLiYuLan.gameObject).onClick = delegate(GameObject s)
		{
			this.CreateYuLanPart();
		};
		TCPGameServerCmds.CMD_SPR_TIANTI_DAY_DATA.SendDataUseRoleID();
		this.initXmlData();
		this.InitTextInPrefabs();
	}

	private void AddInitInfo()
	{
		TCPGameServerCmds.CMD_SPR_TIANTI_JOIN.SendDataUseRoleID();
		base.StopAllCoroutines();
		base.StartCoroutine(this.DaoJiShi(60, delegate
		{
			string text = StringUtil.substitute("{0}", new object[]
			{
				Global.Data.roleData.RoleID
			});
			TCPGameServerCmds.CMD_SPR_TIANTI_QUIT.SendDataUseRoleID();
			this.m_LabelTime.text = "{E2B36B}{-}";
			this.m_pipeibaoqian.SetActive(true);
			this.m_pipeichenggong.SetActive(false);
			this.m_pipeidaojishi.SetActive(false);
		}));
		this.m_XunZhaoDuiShou.gameObject.SetActive(true);
		this.m_LabelTime.text = "60";
		this.m_pipeibaoqian.SetActive(false);
		this.m_pipeichenggong.SetActive(false);
		this.m_pipeidaojishi.SetActive(true);
	}

	private void InitTextInPrefabs()
	{
		this.BtnJoin.GetComponentInChildren<UILabel>().text = Global.GetLang("参与战斗");
		this.m_Lab10Time.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang(string.Format(Global.GetLang("取消后{0}分钟内不可参加"), (int)ConfigSystemParam.GetSystemParamIntByName("TianTiCD") / 60))
		});
		this.m_Lab10Time.gameObject.SetActive(false);
	}

	private IEnumerator StarTime(long times)
	{
		this.timeCD = times;
		this.timeCD /= 1000L;
		while (this.timeCD > 0L)
		{
			int minute = (int)this.timeCD / 60;
			int second = (int)this.timeCD % 60;
			if (second >= 10)
			{
				this.m_LabEndTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					Global.GetLang(string.Format(Global.GetLang("冷却时间:{0}:{1}"), minute, second))
				});
			}
			else
			{
				this.m_LabEndTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					Global.GetLang(string.Format(Global.GetLang("冷却时间:{0}:0{1}"), minute, second))
				});
			}
			yield return new WaitForSeconds(1f);
			this.timeCD -= 1L;
		}
		if (this.timeCD <= 0L)
		{
			this.BtnJoin.isEnabled = true;
			if (this.BtnJoin.tweenTarget.gameObject.GetComponent<UISprite>() != null)
			{
				this.BtnJoin.tweenTarget.gameObject.GetComponent<UISprite>().spriteName = "tongyongBtn3_normal";
			}
			this.m_LabEndTime.text = string.Empty;
		}
		yield break;
	}

	public void aciton_CMD_SPR_TIANTI_ENTER(MUSocketConnectEventArgs e)
	{
		int gameID = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		this.m_pipeibaoqian.SetActive(false);
		this.m_pipeichenggong.SetActive(true);
		this.m_pipeidaojishi.SetActive(false);
		base.StopAllCoroutines();
		this.m_BoolPiPei = true;
		this.m_Lab10Time.gameObject.SetActive(this.m_BoolPiPei);
		base.StartCoroutine(this.DaoJiShi(3, delegate
		{
			string strcmd = string.Empty;
			strcmd = StringUtil.substitute("{0}:{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				gameID
			});
			TCPGameServerCmds.CMD_SPR_TIANTI_ENTER.SendData(strcmd);
		}));
	}

	public void aciton_CMD_SPR_TIANTI_JOIN(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StdErrorCode.GetErrMsg(num, false, false), 0, -1, -1, 0);
			this.m_XunZhaoDuiShou.gameObject.SetActive(false);
			TCPGameServerCmds.CMD_SPR_TIANTI_QUIT.SendDataUseRoleID();
		}
	}

	public void EndTimeCD(long endTime)
	{
		this.m_Lab10Time.gameObject.SetActive(false);
		this.m_BoolPiPei = false;
		long num = endTime - Global.GetCorrectLocalTime();
		if (num > 0L)
		{
			this.BtnJoin.isEnabled = false;
			if (this.BtnJoin.tweenTarget.gameObject.GetComponent<UISprite>() != null)
			{
				this.BtnJoin.tweenTarget.gameObject.GetComponent<UISprite>().spriteName = "tongyongBtn3_disable";
			}
			base.StartCoroutine<bool>(this.StarTime(num));
		}
		else
		{
			this.m_LabEndTime.text = string.Empty;
		}
	}

	public void aciton_CMD_SPR_TIANTI_DAY_DATA(MUSocketConnectEventArgs e)
	{
		this.TianTiDataAndDayPaiHangDataBag = DataHelper.BytesToObject<TianTiDataAndDayPaiHang>(e.bytesData, 0, e.bytesData.Length);
		if (this.TianTiDataAndDayPaiHangDataBag == null || this.TianTiDataAndDayPaiHangDataBag.TianTiData == null)
		{
			return;
		}
		XElement xelementByID = TianTiArenaPart.getXElementByID(this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
		if (this.TianTiDataAndDayPaiHangDataBag.TianTiData.FightCount != 0)
		{
			this.m_LabelShengLv.text = string.Concat(new object[]
			{
				"{C6B08B}",
				Global.GetLang("胜率: "),
				"{-}",
				Math.Round((double)((float)this.TianTiDataAndDayPaiHangDataBag.TianTiData.SuccessCount / (float)this.TianTiDataAndDayPaiHangDataBag.TianTiData.FightCount), 2) * 100.0,
				"%"
			});
		}
		else
		{
			this.m_LabelShengLv.text = string.Concat(new object[]
			{
				"{C6B08B}",
				Global.GetLang("胜率: "),
				"{-}",
				0,
				"%"
			});
		}
		this.m_LabelLianSheng.text = string.Concat(new object[]
		{
			"{C6B08B}",
			Global.GetLang("连胜: "),
			"{-}",
			this.TianTiDataAndDayPaiHangDataBag.TianTiData.LianSheng,
			string.Empty
		});
		if (this.TianTiDataAndDayPaiHangDataBag.TianTiData.TodayFightCount <= xelementByID.AttributeInt("RongYaoNum"))
		{
			this.m_LabelRongYaoChangCiShu.text = string.Concat(new object[]
			{
				"{C6B08B}",
				Global.GetLang("获取荣耀场次: "),
				"{-}{00ff00}",
				this.TianTiDataAndDayPaiHangDataBag.TianTiData.TodayFightCount,
				"/",
				xelementByID.AttributeStr("RongYaoNum"),
				"{-}"
			});
		}
		else
		{
			this.m_LabelRongYaoChangCiShu.text = string.Concat(new string[]
			{
				"{C6B08B}",
				Global.GetLang("获取荣耀场次: "),
				"{-}{00ff00}",
				xelementByID.AttributeStr("RongYaoNum"),
				"/",
				xelementByID.AttributeStr("RongYaoNum"),
				"{-}"
			});
		}
		this.m_LabelDuanWei.text = "{E3B26F}" + TianTiArenaPart.getDuanWeiByID(this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
		XElement xelementByID2 = TianTiArenaPart.getXElementByID((this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId + 1).ToString());
		if (xelementByID2 != null)
		{
			this.m_LabelDuanWeiSuoXu.text = this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiJiFen + "/" + xelementByID2.AttributeStr("NeedDuanWeiJiFen");
			this.m_SpriteDuanWei.fillAmount = (float)this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiJiFen / xelementByID2.AttributeFloat("NeedDuanWeiJiFen");
		}
		else
		{
			xelementByID2 = TianTiArenaPart.getXElementByID(this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
			if (xelementByID2 != null)
			{
				this.m_LabelDuanWeiSuoXu.text = this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiJiFen + "/" + xelementByID2.AttributeStr("NeedDuanWeiJiFen");
				this.m_SpriteDuanWei.fillAmount = 1f;
			}
		}
		DateTime dateTime;
		dateTime..ctor(Global.GetCorrectLocalTime() * 10000L);
		if ((this.TianTiDataAndDayPaiHangDataBag.TianTiData.FetchMonthDuanWeiRankAwardsTime.Month != dateTime.Month || this.TianTiDataAndDayPaiHangDataBag.TianTiData.FetchMonthDuanWeiRankAwardsTime.Year != dateTime.Year) && this.TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiRank > 0 && this.TianTiDataAndDayPaiHangDataBag.TianTiData.FetchMonthDuanWeiRankAwardsTime < this.TianTiDataAndDayPaiHangDataBag.TianTiData.RankUpdateTime)
		{
			this.m_TianTiLingQuJiangLiPart = U3DUtils.NEW<TianTiLingQuJiangLiPart>();
			this.m_TianTiLingQuJiangLiPart.transform.parent = base.transform;
			this.m_TianTiLingQuJiangLiPart.transform.localPosition = new Vector3(0f, 0f, -100f);
			this.m_TianTiLingQuJiangLiPart.transform.localScale = new Vector3(1f, 1f, 1f);
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(10000f, 10f, 0f);
		}
		List<TianTiPaiHangRoleData> paiHangRoleDataList = this.TianTiDataAndDayPaiHangDataBag.PaiHangRoleDataList;
		if (paiHangRoleDataList != null)
		{
			base.StartCoroutine(this.Load3DModual(paiHangRoleDataList));
			if (paiHangRoleDataList.Count > 0)
			{
				this.m_LabelDuanWeiXianShi1.text = "{E4B26D}" + TianTiArenaPart.getDuanWeiByID(paiHangRoleDataList[0].DuanWeiId.ToString());
			}
			if (paiHangRoleDataList.Count > 1)
			{
				this.m_LabelDuanWeiXianShi2.text = "{E4B26D}" + TianTiArenaPart.getDuanWeiByID(paiHangRoleDataList[1].DuanWeiId.ToString());
			}
			if (paiHangRoleDataList.Count > 2)
			{
				this.m_LabelDuanWeiXianShi3.text = "{E4B26D}" + TianTiArenaPart.getDuanWeiByID(paiHangRoleDataList[2].DuanWeiId.ToString());
			}
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (paiHangRoleDataList.Count > 0)
			{
				if (Global.GetNowServerIsZhuTiFu(paiHangRoleDataList[0].ZoneId, out ztBuffServerInfo))
				{
					this.m_LabelPaiMing1.text = paiHangRoleDataList[0].RoleName + Environment.NewLine + ztBuffServerInfo.strServerName;
				}
				else
				{
					this.m_LabelPaiMing1.text = string.Concat(new object[]
					{
						paiHangRoleDataList[0].RoleName,
						Environment.NewLine,
						Global.GetLang("【"),
						paiHangRoleDataList[0].ZoneId,
						Global.GetLang("区】")
					});
				}
			}
			if (paiHangRoleDataList.Count > 1)
			{
				if (Global.GetNowServerIsZhuTiFu(paiHangRoleDataList[1].ZoneId, out ztBuffServerInfo))
				{
					this.m_LabelPaiMing2.text = paiHangRoleDataList[1].RoleName + Environment.NewLine + ztBuffServerInfo.strServerName;
				}
				else
				{
					this.m_LabelPaiMing2.text = string.Concat(new object[]
					{
						paiHangRoleDataList[1].RoleName,
						Environment.NewLine,
						Global.GetLang("【"),
						paiHangRoleDataList[1].ZoneId,
						Global.GetLang("区】")
					});
				}
			}
			if (paiHangRoleDataList.Count > 2)
			{
				if (Global.GetNowServerIsZhuTiFu(paiHangRoleDataList[2].ZoneId, out ztBuffServerInfo))
				{
					this.m_LabelPaiMing3.text = paiHangRoleDataList[2].RoleName + Environment.NewLine + ztBuffServerInfo.strServerName;
				}
				else
				{
					this.m_LabelPaiMing3.text = string.Concat(new object[]
					{
						paiHangRoleDataList[2].RoleName,
						Environment.NewLine,
						Global.GetLang("【"),
						paiHangRoleDataList[2].ZoneId,
						Global.GetLang("区】")
					});
				}
			}
		}
	}

	public static string getDuanWeiByID(string ID)
	{
		if (ID == "0")
		{
			ID = "1";
		}
		string TypeID = string.Empty;
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/DuanWei.xml", new object[0]));
		XElement gameResXml2 = Global.GetGameResXml(string.Format("Config/DuanWeiType.xml", new object[0]));
		XElement xelement = gameResXml.XElementList("DuanWei").Find((XElement s) => s.AttributeStr("ID") == ID);
		if (xelement == null)
		{
			Debug.LogError(ID + Global.GetLang("段位不存在"));
			return string.Empty;
		}
		TypeID = xelement.AttributeStr("Type");
		List<XElement> list = gameResXml2.XElementList("DuanWeiType");
		return Global.GetLang(list.Find((XElement s) => s.AttributeStr("TypeID") == TypeID).AttributeStr("Name")) + xelement.AttributeStr("Level") + Global.GetLang("段");
	}

	public static XElement getXElementByID(string ID)
	{
		if (ID == "0")
		{
			ID = "1";
		}
		XElement gameResXml = Global.GetGameResXml(string.Format("Config/DuanWei.xml", new object[0]));
		return gameResXml.XElementList("DuanWei").Find((XElement s) => s.AttributeStr("ID") == ID);
	}

	private IEnumerator DaoJiShi(int time, Action act = null)
	{
		for (int i = time; i > 0; i--)
		{
			this.m_LabelTime.text = i + string.Empty;
			yield return new WaitForSeconds(1f);
		}
		if (act != null)
		{
			act.Invoke();
		}
		yield break;
	}

	private IEnumerator Load3DModual(List<TianTiPaiHangRoleData> TianTiPaiHangRoleDataList)
	{
		for (int i = 0; i < 3; i++)
		{
			if (TianTiPaiHangRoleDataList.Count < i + 1)
			{
				yield break;
			}
			TianTiPaiHangRoleData _TianTiPaiHangRoleData = TianTiPaiHangRoleDataList[i];
			Transform childTrans = this.m_RankingGroupCurrent.transform.FindChild(string.Empty + i);
			Modal3DShow modal = childTrans.GetComponent<Modal3DShow>();
			RoleData4Selector _RoleData4Selector = _TianTiPaiHangRoleData.RoleData4Selector;
			if (_RoleData4Selector != null)
			{
				int FashionWingGoodsId = Global.GetFashionGoodsID(_RoleData4Selector.FashionWingsID);
				RoleResLoader roleResLoader = UIHelper.LoadRoleRes(modal, _RoleData4Selector.SettingBitFlags, _RoleData4Selector.Occupation, _RoleData4Selector.SubOccupation, _RoleData4Selector.OtherName, _RoleData4Selector.GoodsDataList, null, _RoleData4Selector.MyWingData, 1f, FashionWingGoodsId, null, false);
				if (roleResLoader != null)
				{
					this.resLoader.Add(roleResLoader);
				}
			}
			UIHelper.SetModalPosZ(modal.transform);
			yield return new WaitForSeconds(0.8f);
		}
		yield break;
	}

	public static void aciton_CMD_SPR_TIANTI_AWARD(MUSocketConnectEventArgs e)
	{
		TianTiAwardsData tianTiAwardsData = DataHelper.BytesToObject<TianTiAwardsData>(e.bytesData, 0, e.bytesData.Length);
		if (tianTiAwardsData.Success == -1)
		{
			Super.ShowMessageBoxExt(Global.GetLang("提示"), Global.GetLang("对方进入异常战斗取消"), 10f, delegate(object s, DPSelectedItemEventArgs e1)
			{
				Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventFuBen", new ScriptEventArgs
				{
					NpcID = 1000000,
					ScriptID = 10,
					Hint = 0
				}));
			}, new string[]
			{
				Global.GetLang("确定")
			});
			return;
		}
		TianTiResultPart tianTiResultPart = U3DUtils.NEW<TianTiResultPart>();
		tianTiResultPart.transform.parent = PlayZone.GlobalPlayZone.transform;
		tianTiResultPart.transform.localPosition = new Vector3(0f, 0f, -1000f);
		tianTiResultPart.transform.localScale = new Vector3(1f, 1f, 1f);
		tianTiResultPart.InnitData(tianTiAwardsData);
	}

	public static void OpenWindow()
	{
		GChildWindow gchildWindow = U3DUtils.NEW<GChildWindow>();
		gchildWindow.IsShowModal = true;
		gchildWindow.Z = 100.0;
		TianTiArenaPart obj = U3DUtils.NEW<TianTiArenaPart>();
		Super.GData.GlobalPlayZone.Children.Add(gchildWindow);
		gchildWindow.ModalType = ChildWindowModalType.BlackBak;
		gchildWindow.SetContent(gchildWindow.BodyPresenter, obj, 0.0, 0.0, true);
	}

	public static void OpenWindowRongYao()
	{
		GChildWindow gchildWindow = U3DUtils.NEW<GChildWindow>();
		gchildWindow.IsShowModal = true;
		gchildWindow.Z = 100.0;
		MUDuiHuanPart m_MUDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
		Super.GData.GlobalPlayZone.Children.Add(gchildWindow);
		gchildWindow.ModalType = ChildWindowModalType.BlackBak;
		gchildWindow.SetContent(gchildWindow.BodyPresenter, m_MUDuiHuanPart, 0.0, 0.0, true);
		m_MUDuiHuanPart.InitPartData(4, 0);
		m_MUDuiHuanPart.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e)
		{
			Object.Destroy(m_MUDuiHuanPart.gameObject);
			if (m_MUDuiHuanPart.GetComponentInParent<GChildWindow>())
			{
				Object.Destroy(m_MUDuiHuanPart.GetComponentInParent<GChildWindow>().gameObject);
			}
			return false;
		};
	}

	protected override void OnDestroy()
	{
		int count = this.resLoader.Count;
		for (int i = 0; i < count; i++)
		{
			RoleResLoader roleResLoader = this.resLoader[i];
			roleResLoader.Stop();
		}
		TianTiArenaPart.Instance = null;
		base.StopAllCoroutines();
	}

	private void CreateYuLanPart()
	{
		this.GChildJTiantiYuLanPart = U3DUtils.NEW<GChildWindow>();
		this.GChildJTiantiYuLanPart.ModalType = ChildWindowModalType.Translucent;
		this.GChildJTiantiYuLanPart.Modal = true;
		this.GChildJTiantiYuLanPart.IsShowModal = true;
		Super.InitChildWindow(this.GChildJTiantiYuLanPart, "GChildJTiantiYuLanPart");
		Super.GData.GlobalPlayZone.Children.Add(this.GChildJTiantiYuLanPart);
		this.m_AwardPart = U3DUtils.NEW<JiangLiYuLanPart>();
		this.GChildJTiantiYuLanPart.Body.Add(this.m_AwardPart);
		this.m_AwardPart.transform.localPosition = new Vector3(0f, 0f, -800f);
		this.m_AwardPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_AwardPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == -10)
			{
				this.DestroyAwardPart();
			}
		};
		base.StartCoroutine<bool>(this.m_AwardPart.init(this.JingjiXmlList, 0, null));
	}

	private void DestroyAwardPart()
	{
		Object.Destroy(this.GChildJTiantiYuLanPart.gameObject);
	}

	private void initXmlData()
	{
		if (this.JingjiXmlList != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/DuanWeiRankAward.xml");
		if (gameResXml != null)
		{
			this.JingjiXmlList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		}
	}

	public static TianTiArenaPart Instance;

	public UILabel labRule;

	public UIButton btnZhanbao;

	public UIButton btnJiangli;

	public UIButton btnPaihang;

	public UIButton BtnJoin;

	public UIButton m_BtnCloseRule;

	public UIButton m_BtnTianTiGuiZe;

	public UIButton m_BtnClose;

	public UIButton m_BtnShiPin;

	public UIButton m_BtnQuXiao;

	public UIButton m_BtnTianTiJiangLiYuLan;

	public UILabel m_LabelShengLv;

	public UILabel m_LabelLianSheng;

	public UILabel m_LabelRongYaoChangCiShu;

	public UILabel m_LabelDuanWei;

	public UILabel m_LabelDuanWeiXianShi1;

	public UILabel m_LabelDuanWeiXianShi2;

	public UILabel m_LabelDuanWeiXianShi3;

	public UILabel m_LabelDuanWeiSuoXu;

	public UILabel m_LabelTime;

	public UILabel m_Lab10Time;

	public UILabel m_LabEndTime;

	public UILabel m_LabelPaiMing1;

	public UILabel m_LabelPaiMing2;

	public UILabel m_LabelPaiMing3;

	public GameObject m_pipeidaojishi;

	public GameObject m_pipeibaoqian;

	public GameObject m_pipeichenggong;

	public UISprite m_SpriteDuanWei;

	public Transform m_XunZhaoDuiShou;

	public Transform m_RankingGroupCurrent;

	public Transform m_RankingGroup;

	public Transform m_Rule;

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<XElement> JingjiXmlList;

	private JiangLiYuLanPart m_AwardPart;

	private Dictionary<int, TianTiArenaPart.ModalInfo> modalInfoDict = new Dictionary<int, TianTiArenaPart.ModalInfo>();

	private Dictionary<int, TianTiArenaPart.ModalInfo> modalInfoPrePageDict = new Dictionary<int, TianTiArenaPart.ModalInfo>();

	public TianTiDataAndDayPaiHang TianTiDataAndDayPaiHangDataBag;

	public TianTiZhanbaoPart m_TianTiZhanbaoPart;

	public TianTiDuanWeiPart m_TianTiDuanWeiPart;

	public TianTiLingQuJiangLiPart m_TianTiLingQuJiangLiPart;

	public UILabel m_quxiao;

	public UILabel[] paimingLabs;

	protected GChildWindow GChildTiantiDuiHuanPart;

	protected MUDuiHuanPart m_MUDuiHuanPart;

	protected GChildWindow GChildJTiantiYuLanPart;

	protected GChildWindow GChildTiantiZhanbaoPart;

	protected GChildWindow GChildTiantiPaihangPart;

	private bool m_BoolPiPei;

	private long timeCD = -1L;

	private List<RoleResLoader> resLoader = new List<RoleResLoader>();

	private class ModalInfo
	{
		public Modal3DShow modalShow;

		public RoleData4Selector roleData;

		public ArenaPartOtherPlayerItem infos;
	}
}
