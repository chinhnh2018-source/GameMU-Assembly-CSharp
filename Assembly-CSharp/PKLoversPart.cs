using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class PKLoversPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.ShengLv.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("胜率：")
		});
		this.LianSheng.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("连胜：")
		});
		this.ChangCi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("每周获取荣耀场次：")
		});
		this.OnePeopleBtn.Label.text = Global.GetLang("单人匹配");
		this.TwoPeopleBtn.Label.text = Global.GetLang("双人准备");
		this.BackGround.URL = "NetImages/GameRes/Images/Plate/PKLovers.jpg.qj";
		this.ShengLvPer.transform.localPosition = new Vector3(20f, 161f, -1f);
		this.LianSheng.transform.localPosition = new Vector3(95f, 161f, -1f);
		this.LianShengPer.pivot = 3;
		this.LianShengPer.transform.localPosition = new Vector3(135f, 161f, -1f);
		this.ChangCi.transform.localPosition = new Vector3(275f, 161f, -1f);
		this.DuanWeiLev.text = string.Empty;
		this.LeftPlayerLab.text = string.Empty;
		this.rightPlayerlab.text = string.Empty;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
			GameInstance.Game.GetStateWatcherForPKLovers(0);
		};
		this.Help.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenRuleInterFace();
		};
		this.Shop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ItemHandler(this, new DPSelectedItemEventArgs());
		};
		this.ShiPinBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1507,
					MyID = 5
				});
			}
		};
		this.ZhanBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenZhanBaoInterFace();
		};
		this.DuanWei.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenPaiHangBangInterFace();
		};
		this.JiangLi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAwardInterFace();
		};
		this.OnePeopleBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						if (this.OnePeopleBtn.Label.text.Equals(Global.GetLang("单人匹配")))
						{
							GameInstance.Game.SendSingleJionInfoForPKLovers();
						}
						if (this.OnePeopleBtn.Label.text.Equals(Global.GetLang("取消准备")))
						{
							GameInstance.Game.SendQuitJionInfoForPKLovers();
						}
						Super.ShowNetWaiting(null);
					}
				}, -1);
			}
			else
			{
				if (this.OnePeopleBtn.Label.text.Equals(Global.GetLang("单人匹配")))
				{
					GameInstance.Game.SendSingleJionInfoForPKLovers();
				}
				if (this.OnePeopleBtn.Label.text.Equals(Global.GetLang("取消准备")))
				{
					GameInstance.Game.SendQuitJionInfoForPKLovers();
				}
				Super.ShowNetWaiting(null);
			}
		};
		this.TwoPeopleBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.Data.CurrentCopyTeamData != null)
			{
				Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						if (this.TwoPeopleBtn.Label.text.Equals(Global.GetLang("双人准备")))
						{
							GameInstance.Game.SetReadyStateForPKLovers(1);
						}
						if (this.TwoPeopleBtn.Label.text.Equals(Global.GetLang("取消准备")))
						{
							GameInstance.Game.SendQuitJionInfoForPKLovers();
						}
						Super.ShowNetWaiting(null);
					}
				}, -1);
			}
			else
			{
				if (this.TwoPeopleBtn.Label.text.Equals(Global.GetLang("双人准备")))
				{
					GameInstance.Game.SetReadyStateForPKLovers(1);
				}
				if (this.TwoPeopleBtn.Label.text.Equals(Global.GetLang("取消准备")))
				{
					GameInstance.Game.SendQuitJionInfoForPKLovers();
				}
				Super.ShowNetWaiting(null);
			}
		};
		this.SendInfo();
	}

	public void AnalysisMainDataBeFromServer(CoupleArenaMainData mainData)
	{
		if (mainData == null)
		{
			return;
		}
		this.LocalMainData = mainData;
		this.AnalysisAttributesData(mainData);
		this.LoadRoleRes(mainData);
		if (mainData.CanGetAwardId >= 1 && mainData.CanGetAwardId <= 17)
		{
			this.OpenGetAwardInterFace(mainData.JingJiData.Rank, mainData.CanGetAwardId);
		}
	}

	public void AnalysisRoleStateBeFromServer(List<CoupleArenaRoleStateData> stateData)
	{
		if (this.LocalMainData == null)
		{
			return;
		}
		if (stateData == null)
		{
			return;
		}
		string stateWife = null;
		string stateMan = null;
		int i = 0;
		int count = stateData.Count;
		while (i < count)
		{
			if (stateData[i].RoleId == this.LocalMainData.JingJiData.WifeRoleId)
			{
				this.rightPlayerlab.text = Global.GetColorStringForNGUIText(new object[]
				{
					this.SetScriptColors(stateData[i].MatchState),
					this.SetRoleState(stateData[i].MatchState)
				});
				stateWife = this.SetRoleState(stateData[i].MatchState);
			}
			else if (stateData[i].RoleId == this.LocalMainData.JingJiData.ManRoleId)
			{
				this.LeftPlayerLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					this.SetScriptColors(stateData[i].MatchState),
					this.SetRoleState(stateData[i].MatchState)
				});
				stateMan = this.SetRoleState(stateData[i].MatchState);
			}
			i++;
		}
		this.SetReadyState(stateWife, stateMan);
	}

	private void SetReadyState(string stateWife, string stateMan)
	{
		if (Global.Data.roleData.RoleSex == 0)
		{
			this.SetReadyStateToOnePeople(stateWife);
			if (this.canreadyonepeople)
			{
				this.ChangeMiaoShuToOnePeopleBtn(stateMan);
			}
			else
			{
				this.ChangeMiaoShuToDouplePeopleBtn(stateMan);
			}
		}
		else
		{
			this.SetReadyStateToOnePeople(stateMan);
			if (this.canreadyonepeople)
			{
				this.ChangeMiaoShuToOnePeopleBtn(stateWife);
			}
			else
			{
				this.ChangeMiaoShuToDouplePeopleBtn(stateWife);
			}
		}
	}

	private void ChangeMiaoShuToOnePeopleBtn(string state)
	{
		if (state.Equals(Global.GetLang("已准备")))
		{
			this.OnePeopleBtn.Label.text = Global.GetLang(Global.GetLang("取消准备"));
		}
		else
		{
			this.OnePeopleBtn.Label.text = Global.GetLang("单人匹配");
		}
	}

	private void SetReadyStateToOnePeople(string state)
	{
		if (state.Equals(Global.GetLang("功能未开启")) || state.Equals(Global.GetLang("离线")))
		{
			this.canreadyonepeople = true;
		}
		else
		{
			this.canreadyonepeople = false;
		}
		this.OnePeopleBtn.isEnabled = this.canreadyonepeople;
		this.TwoPeopleBtn.isEnabled = !this.canreadyonepeople;
		if (!this.canreadyonepeople)
		{
			this.OnePeopleBtn.Label.text = Global.GetLang("单人匹配");
		}
		else
		{
			this.TwoPeopleBtn.Label.text = Global.GetLang("双人准备");
		}
	}

	private void ChangeMiaoShuToDouplePeopleBtn(string state)
	{
		if (state.Equals("已准备"))
		{
			this.TwoPeopleBtn.Label.text = Global.GetLang("取消准备");
		}
		else
		{
			this.TwoPeopleBtn.Label.text = Global.GetLang("双人准备");
		}
	}

	private string SetScriptColors(int matchstate)
	{
		if (matchstate == 0)
		{
			return "ff0000";
		}
		if (matchstate == 1)
		{
			return "ffcc19";
		}
		if (matchstate == 2)
		{
			return "17e43e";
		}
		return "f0f0f0";
	}

	private string SetRoleState(int matchstate)
	{
		if (matchstate == 0)
		{
			return Global.GetLang("离线");
		}
		if (matchstate == 1)
		{
			return Global.GetLang("在线");
		}
		if (matchstate == 2)
		{
			return Global.GetLang("已准备");
		}
		if (matchstate == 3)
		{
			return Global.GetLang("功能未开启");
		}
		return null;
	}

	public override void Destroy()
	{
		if (this.leftResLoader != null)
		{
			this.leftResLoader.Stop();
		}
		if (this.rightResLoader != null)
		{
			this.rightResLoader.Stop();
		}
	}

	private void LoadRoleRes(CoupleArenaMainData mainData)
	{
		RoleData4Selector manSelector = mainData.JingJiData.ManSelector;
		RoleData4Selector wifeSelector = mainData.JingJiData.WifeSelector;
		if (this.leftResLoader != null)
		{
			this.leftResLoader.Stop();
		}
		if (this.rightResLoader != null)
		{
			this.rightResLoader.Stop();
		}
		this.leftResLoader = UIHelper.LoadRoleRes(this.LeftModel, manSelector.SettingBitFlags, manSelector.Occupation, manSelector.SubOccupation, manSelector.RoleName, manSelector.GoodsDataList, null, manSelector.MyWingData, 1f, 0, null, false);
		this.rightResLoader = UIHelper.LoadRoleRes(this.RightModel, wifeSelector.SettingBitFlags, wifeSelector.Occupation, wifeSelector.SubOccupation, wifeSelector.RoleName, wifeSelector.GoodsDataList, null, wifeSelector.MyWingData, 1f, 0, null, false);
	}

	private void AnalysisAttributesData(CoupleArenaMainData mainData)
	{
		string text = null;
		float num = 0f;
		if (PKLoversPart.GetCoupleDuanWeiTypeDic().ContainsKey(mainData.JingJiData.DuanWeiType))
		{
			text = string.Format("{0}", PKLoversPart.GetCoupleDuanWeiTypeDic()[mainData.JingJiData.DuanWeiType].Name);
		}
		this.DuanWeiLev.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			text
		});
		if (PKLoversPart.GetCoupleDuanWeiDic().ContainsKey(mainData.JingJiData.DuanWeiType) && PKLoversPart.GetCoupleDuanWeiDic()[mainData.JingJiData.DuanWeiType].ContainsKey(mainData.JingJiData.DuanWeiLevel))
		{
			this.DuanWeiPer.text = string.Format("{0}/{1}", mainData.JingJiData.JiFen, PKLoversPart.GetCoupleDuanWeiDic()[mainData.JingJiData.DuanWeiType][mainData.JingJiData.DuanWeiLevel].UpLevelNeedJiFen);
			num = (float)((double)mainData.JingJiData.JiFen * 1.0 / ((double)PKLoversPart.GetCoupleDuanWeiDic()[mainData.JingJiData.DuanWeiType][mainData.JingJiData.DuanWeiLevel].UpLevelNeedJiFen * 1.0));
			this.ChangCiPer.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Format("{0}/{1}", mainData.WeekGetRongYaoTimes, PKLoversPart.GetCoupleDuanWeiDic()[mainData.JingJiData.DuanWeiType][mainData.JingJiData.DuanWeiLevel].WeekRongYaoNum)
			});
		}
		this.JingYanTiao.transform.localScale = new Vector3(num * 400f, 12f, 1f);
		this.ShengLvPer.text = string.Format("{0}%", (mainData.JingJiData.TotalFightTimes != 0) ? ((int)((double)mainData.JingJiData.WinFightTimes * 1.0 / ((double)mainData.JingJiData.TotalFightTimes * 1.0) * 100.0)) : 0);
		this.LianShengPer.text = mainData.JingJiData.LianShengTimes.ToString();
	}

	private void SendInfo()
	{
		GameInstance.Game.GetMainDataForPKLovers();
		GameInstance.Game.GetStateWatcherForPKLovers(1);
		Super.ShowNetWaiting(null);
	}

	private void OpenRuleInterFace()
	{
		if (this.PKLRuleWindow == null)
		{
			this.PKLRuleWindow = U3DUtils.NEW<GChildWindow>();
			this.PKLRuleWindow.IsShowModal = true;
			this.PKLRuleWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.PKLRuleWindow, Global.GetLang("规则界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.PKLRuleWindow);
		}
		if (this.pkLoversPartRule == null)
		{
			this.pkLoversPartRule = U3DUtils.NEW<PKLoversPartRule>();
			this.pkLoversPartRule.CloseHandle = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseRuleInterFace();
			};
		}
		this.PKLRuleWindow.SetContent(this.PKLRuleWindow.BodyPresenter, this.pkLoversPartRule, 0.0, 0.0, true);
	}

	private void CloseRuleInterFace()
	{
		if (null != this.pkLoversPartRule)
		{
			this.pkLoversPartRule.transform.parent = null;
			Object.Destroy(this.pkLoversPartRule.gameObject);
			this.pkLoversPartRule = null;
		}
		if (null != this.PKLRuleWindow)
		{
			Super.CloseChildWindow(base.Children, this.PKLRuleWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.PKLRuleWindow, true);
			this.PKLRuleWindow = null;
		}
	}

	private void OpenZhanBaoInterFace()
	{
		if (this.PKLZhanBaoWindow == null)
		{
			this.PKLZhanBaoWindow = U3DUtils.NEW<GChildWindow>();
			this.PKLZhanBaoWindow.IsShowModal = true;
			this.PKLZhanBaoWindow.ModalType = ChildWindowModalType.Translucent;
			Super.GData.GlobalPlayZone.Children.Add(this.PKLZhanBaoWindow);
			Super.InitChildWindow(this.PKLZhanBaoWindow, Global.GetLang("战报界面"));
		}
		if (this.pkLoversPartZhanBao == null)
		{
			this.pkLoversPartZhanBao = U3DUtils.NEW<PKLoversPartZhanBao>();
			this.pkLoversPartZhanBao.InitAttr(this.LocalMainData);
			this.pkLoversPartZhanBao.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseZhanBaoInterFace();
			};
		}
		this.PKLZhanBaoWindow.SetContent(this.PKLZhanBaoWindow.BodyPresenter, this.pkLoversPartZhanBao, 0.0, 0.0, true);
	}

	private void CloseZhanBaoInterFace()
	{
		if (this.pkLoversPartZhanBao)
		{
			this.pkLoversPartZhanBao.transform.parent = null;
			Object.Destroy(this.pkLoversPartZhanBao.gameObject);
			this.pkLoversPartZhanBao = null;
		}
		if (this.PKLZhanBaoWindow)
		{
			Super.GData.GlobalPlayZone.Children.Remove(this.PKLZhanBaoWindow, true);
			Super.CloseChildWindow(base.Children, this.PKLZhanBaoWindow);
		}
	}

	private void OpenAwardInterFace()
	{
		if (this.PKLAwardWindow == null)
		{
			this.PKLAwardWindow = U3DUtils.NEW<GChildWindow>();
			this.PKLAwardWindow.IsShowModal = true;
			this.PKLAwardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.PKLAwardWindow, Global.GetLang("奖励界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.PKLAwardWindow);
		}
		if (this.pkLoversPartAward == null)
		{
			this.pkLoversPartAward = U3DUtils.NEW<PKLoversPartAward>();
			this.pkLoversPartAward.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAwardInterFace();
			};
		}
		this.PKLAwardWindow.SetContent(this.PKLAwardWindow.BodyPresenter, this.pkLoversPartAward, 0.0, 0.0, true);
	}

	private void CloseAwardInterFace()
	{
		if (this.pkLoversPartAward)
		{
			this.pkLoversPartAward.transform.parent = null;
			Object.Destroy(this.pkLoversPartAward.gameObject);
			this.pkLoversPartAward = null;
		}
		if (this.PKLAwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.PKLAwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.PKLAwardWindow, true);
			this.PKLAwardWindow = null;
		}
	}

	private void OpenPaiHangBangInterFace()
	{
		if (this.PKLPaiHangWindow == null)
		{
			this.PKLPaiHangWindow = U3DUtils.NEW<GChildWindow>();
			this.PKLPaiHangWindow.IsShowModal = true;
			this.PKLPaiHangWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.PKLPaiHangWindow, Global.GetLang("排行榜界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.PKLPaiHangWindow);
		}
		if (this.duanweiLoversPart == null)
		{
			this.duanweiLoversPart = U3DUtils.NEW<DuanWeiLoversPart>();
			if (this.LocalMainData != null && PKLoversPart.GetCoupleDuanWeiTypeDic().ContainsKey(this.LocalMainData.JingJiData.DuanWeiType))
			{
				this.duanweiLoversPart.PaiMing.text = this.LocalMainData.JingJiData.Rank.ToString();
				this.duanweiLoversPart.DuanWei.text = PKLoversPart.GetCoupleDuanWeiTypeDic()[this.LocalMainData.JingJiData.DuanWeiType].Name;
				this.duanweiLoversPart.DuanWeiJiFen.text = this.LocalMainData.JingJiData.JiFen.ToString();
			}
			this.duanweiLoversPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ClosePaiHangBangInterFace();
			};
		}
		this.PKLPaiHangWindow.SetContent(this.PKLPaiHangWindow.BodyPresenter, this.duanweiLoversPart, 0.0, 0.0, true);
	}

	private void ClosePaiHangBangInterFace()
	{
		if (this.duanweiLoversPart)
		{
			this.duanweiLoversPart.transform.parent = null;
			Object.Destroy(this.duanweiLoversPart.gameObject);
			this.duanweiLoversPart = null;
		}
		if (this.PKLPaiHangWindow)
		{
			Super.CloseChildWindow(base.Children, this.PKLPaiHangWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.PKLPaiHangWindow, true);
			this.PKLPaiHangWindow = null;
		}
	}

	private void OpenGetAwardInterFace(int paihang, int awardID)
	{
		if (this.PKLGetAwardWindow == null)
		{
			this.PKLGetAwardWindow = U3DUtils.NEW<GChildWindow>();
			this.PKLGetAwardWindow.IsShowModal = true;
			this.PKLGetAwardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.PKLGetAwardWindow, Global.GetLang("获奖励界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.PKLGetAwardWindow);
		}
		if (this.PKLoversPartGetAward == null)
		{
			this.PKLoversPartGetAward = U3DUtils.NEW<PKLoversPartGetAward>();
			this.PKLoversPartGetAward.InitListItem(paihang, awardID);
			this.PKLoversPartGetAward.DPSHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseGetAwardInterFace();
			};
		}
		this.PKLGetAwardWindow.SetContent(this.PKLGetAwardWindow.BodyPresenter, this.PKLoversPartGetAward, 0.0, 0.0, true);
	}

	private void CloseGetAwardInterFace()
	{
		if (this.PKLoversPartGetAward)
		{
			this.PKLoversPartGetAward.transform.parent = null;
			Object.Destroy(this.PKLoversPartGetAward.gameObject);
			this.PKLoversPartGetAward = null;
		}
		if (this.PKLGetAwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.PKLGetAwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.PKLGetAwardWindow, true);
			this.PKLGetAwardWindow = null;
		}
	}

	public static void ClearXMLData()
	{
		if (0 < PKLoversPart.CoupleDuanWeiDic.Count)
		{
			PKLoversPart.CoupleDuanWeiDic.Clear();
		}
		if (0 < PKLoversPart.CoupleWarAwardDic.Count)
		{
			PKLoversPart.CoupleWarAwardDic.Clear();
		}
		if (0 < PKLoversPart.CoupleDuanWeiTypeDic.Count)
		{
			PKLoversPart.CoupleDuanWeiTypeDic.Clear();
		}
	}

	public static Dictionary<int, Dictionary<int, CoupleDuanWei>> GetCoupleDuanWeiDic()
	{
		if (PKLoversPart.CoupleDuanWeiDic.Count > 0)
		{
			return PKLoversPart.CoupleDuanWeiDic;
		}
		XElement gameResXml = Global.GetGameResXml("Config/CoupleDuanWei.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "CoupleDuanWei");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			CoupleDuanWei coupleDuanWei = new CoupleDuanWei();
			coupleDuanWei.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			coupleDuanWei.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			coupleDuanWei.NeedCoupleDuanWeiJiFen = Global.GetXElementAttributeInt(xelementList[i], "NeedCoupleDuanWeiJiFen");
			coupleDuanWei.WinJiFen = Global.GetXElementAttributeInt(xelementList[i], "WinJiFen");
			coupleDuanWei.LoseJiFen = Global.GetXElementAttributeInt(xelementList[i], "LoseJiFen");
			coupleDuanWei.WeekRongYaoNum = Global.GetXElementAttributeInt(xelementList[i], "WeekRongYaoNum");
			coupleDuanWei.WinRongYu = Global.GetXElementAttributeInt(xelementList[i], "WinRongYu");
			coupleDuanWei.LoseRongYu = Global.GetXElementAttributeInt(xelementList[i], "LoseRongYu");
			coupleDuanWei.UpLevelNeedJiFen = Global.GetXElementAttributeInt(xelementList[i], "UpLevelNeedJiFen");
			if (!PKLoversPart.CoupleDuanWeiDic.ContainsKey(coupleDuanWei.Type))
			{
				Dictionary<int, CoupleDuanWei> dictionary = new Dictionary<int, CoupleDuanWei>();
				dictionary.Add(coupleDuanWei.Level, coupleDuanWei);
				PKLoversPart.CoupleDuanWeiDic.Add(coupleDuanWei.Type, dictionary);
			}
			else
			{
				PKLoversPart.CoupleDuanWeiDic[coupleDuanWei.Type].Add(coupleDuanWei.Level, coupleDuanWei);
			}
			i++;
		}
		return PKLoversPart.CoupleDuanWeiDic;
	}

	public static Dictionary<int, CoupleWarAward> GetCoupleWarAwardDic()
	{
		if (PKLoversPart.CoupleWarAwardDic.Count > 0)
		{
			return PKLoversPart.CoupleWarAwardDic;
		}
		XElement gameResXml = Global.GetGameResXml("Config/CoupleWarAward.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			CoupleWarAward coupleWarAward = new CoupleWarAward();
			coupleWarAward.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			coupleWarAward.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			coupleWarAward.StarRank = Global.GetXElementAttributeInt(xelementList[i], "StarRank");
			coupleWarAward.EndRank = Global.GetXElementAttributeInt(xelementList[i], "EndRank");
			coupleWarAward.Award = Global.GetXElementAttributeStr(xelementList[i], "Award");
			if (!PKLoversPart.CoupleWarAwardDic.ContainsKey(coupleWarAward.ID))
			{
				PKLoversPart.CoupleWarAwardDic.Add(coupleWarAward.ID, coupleWarAward);
			}
			i++;
		}
		return PKLoversPart.CoupleWarAwardDic;
	}

	public static Dictionary<int, CoupleDuanWeiType> GetCoupleDuanWeiTypeDic()
	{
		if (PKLoversPart.CoupleDuanWeiTypeDic.Count > 0)
		{
			return PKLoversPart.CoupleDuanWeiTypeDic;
		}
		XElement gameResXml = Global.GetGameResXml("Config/CoupleDuanWeiType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "CoupleDuanWeiType");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			CoupleDuanWeiType coupleDuanWeiType = new CoupleDuanWeiType();
			coupleDuanWeiType.TypeID = Global.GetXElementAttributeInt(xelementList[i], "TypeID");
			coupleDuanWeiType.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			if (!PKLoversPart.CoupleDuanWeiTypeDic.ContainsKey(coupleDuanWeiType.TypeID))
			{
				PKLoversPart.CoupleDuanWeiTypeDic.Add(coupleDuanWeiType.TypeID, coupleDuanWeiType);
			}
			i++;
		}
		return PKLoversPart.CoupleDuanWeiTypeDic;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ItemHandler;

	public GButton Close;

	public GButton Help;

	public GButton Shop;

	public GButton ZhanBao;

	public GButton DuanWei;

	public GButton JiangLi;

	public GButton OnePeopleBtn;

	public GButton TwoPeopleBtn;

	public GButton ShiPinBtn;

	public UILabel DuanWeiLev;

	public UILabel DuanWeiPer;

	public UILabel ShengLv;

	public UILabel ShengLvPer;

	public UILabel LianSheng;

	public UILabel LianShengPer;

	public UILabel ChangCi;

	public UILabel ChangCiPer;

	public UILabel LeftPlayerLab;

	public UILabel rightPlayerlab;

	public UISprite JingYanTiao;

	public ShowNetImage BackGround;

	public Modal3DShow LeftModel;

	public Modal3DShow RightModel;

	private GChildWindow PKLRuleWindow;

	private PKLoversPartRule pkLoversPartRule;

	private GChildWindow PKLAwardWindow;

	private PKLoversPartAward pkLoversPartAward;

	private GChildWindow PKLZhanBaoWindow;

	public PKLoversPartZhanBao pkLoversPartZhanBao;

	private GChildWindow PKLPaiHangWindow;

	public DuanWeiLoversPart duanweiLoversPart;

	private GChildWindow PKLGetAwardWindow;

	private PKLoversPartGetAward PKLoversPartGetAward;

	private CoupleArenaMainData LocalMainData;

	private bool canreadyonepeople;

	private RoleResLoader leftResLoader;

	private RoleResLoader rightResLoader;

	private static Dictionary<int, Dictionary<int, CoupleDuanWei>> CoupleDuanWeiDic = new Dictionary<int, Dictionary<int, CoupleDuanWei>>();

	private static Dictionary<int, CoupleWarAward> CoupleWarAwardDic = new Dictionary<int, CoupleWarAward>();

	private static Dictionary<int, CoupleDuanWeiType> CoupleDuanWeiTypeDic = new Dictionary<int, CoupleDuanWeiType>();
}
