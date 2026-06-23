using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class ArenaPart : UserControl
{
	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				if (this.Items != null && this.Items.Length > 0)
				{
					SystemHelpPart.SetMask(this.Items[0].m_TiaozhanBtn, default(Vector4));
				}
				else
				{
					SystemHelpPart.HideMask();
				}
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void InitTextInPrefabs()
	{
		this.m_TiaozhanCDLabel.Text = Global.GetLang("无");
		this.lblZhanli.transform.localPosition = new Vector3(60f, 231f, -1f);
		this.lblJunxian.transform.localPosition = new Vector3(255f, 231f, -1f);
		this.m_TiaozhanCDLabel.Y = 163.0;
		this.m_TiaozhanLabel.Y = 163.0;
		this.m_VIPTiaozhanLabel.Y = 163.0;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		};
		UIEventListener.Get(this.btnJunxian.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowModals(false);
			this.CreateJunxianPart();
			this.m_btnPrePage.gameObject.SetActive(false);
			this.m_btnLastPage.gameObject.SetActive(false);
		};
		UIEventListener.Get(this.btnZhanbao.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowModals(false);
			this.CreateZhanbaoPart();
			this.m_btnPrePage.gameObject.SetActive(false);
			this.m_btnLastPage.gameObject.SetActive(false);
		};
		UIEventListener.Get(this.btnJiangli.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowModals(false);
			this.CreateAwardPart();
			this.m_btnPrePage.gameObject.SetActive(false);
			this.m_btnLastPage.gameObject.SetActive(false);
		};
		this.m_LingquBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SpriteJingJiRankingRewardCmd();
		};
		UIEventListener.Get(this.btnPaihang.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0,
				IDType = 0
			});
		};
		this.m_btnPrePage.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.modalInfoPrePageDict.Count == 0)
			{
				GameInstance.Game.SpriteJingJiDetailCmd(1);
			}
			this.isPrePage = true;
			this.m_btnPrePage.gameObject.SetActive(false);
			this.m_btnLastPage.gameObject.SetActive(true);
		};
		this.m_btnLastPage.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.isPrePage = false;
			this.m_btnPrePage.gameObject.SetActive(true);
			this.m_btnLastPage.gameObject.SetActive(false);
		};
		ActivityTipManager.RegActivityTipItem(4002, delegate(int s, ActivityTipItem e)
		{
			if (this.m_NewJunxian.gameObject.activeSelf != e.IsActive)
			{
				this.m_NewJunxian.gameObject.SetActive(e.IsActive);
			}
		});
	}

	public void initInfo(JingJiDetailData data)
	{
		if (data == null)
		{
			return;
		}
		this.m_DetailData = data;
		if (this.m_DetailData.beChallengerData != null)
		{
			this.m_DetailData.beChallengerData.Sort(new Comparison<PlayerJingJiMiniData>(this.CompareByRanking));
			if (this.isPrePage)
			{
				if (this.beChallengerDataPre != null)
				{
					this.beChallengerDataPre.Clear();
				}
				this.beChallengerDataPre = this.m_DetailData.beChallengerData;
			}
			else
			{
				if (this.beChallengerDataCurrent != null)
				{
					this.beChallengerDataCurrent.Clear();
				}
				this.beChallengerDataCurrent = this.m_DetailData.beChallengerData;
			}
			this.GetOtherAttr();
		}
		this.m_TiaozhanLabel.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			data.useFreeChallengeNum,
			data.freeChallengeNum
		});
		this.m_VIPTiaozhanLabel.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			data.useChallengeNum,
			data.vipChallengeNum
		});
		if (data.ranking < 1)
		{
			this.m_RankingLabel.Text = Global.GetLang("5000名后");
		}
		else
		{
			this.m_RankingLabel.Text = data.ranking + string.Empty;
		}
		if (data.ranking <= 4 && data.ranking >= 1)
		{
			this.m_btnPrePage.gameObject.SetActive(false);
			this.isTo3 = true;
		}
		this.initXmlData();
		this.initItems();
		if (ArenaPart.needToOpenJunxian)
		{
			this.ShowModals(false);
			this.CreateJunxianPart();
			ArenaPart.needToOpenJunxian = false;
		}
		this.RefrashJunxianNZhanli();
	}

	public void updeteTiaozhanTime()
	{
		this.m_DetailData.nextChallengeTime = 0L;
		this.m_TiaozhanCDLabel.Text = Global.GetLang("无");
		this.m_TiaozhanCD = false;
		if (this.isPrePage)
		{
			GameInstance.Game.SpriteRequestJingJiChallengeCmd(this.beChallengerDataPre[this.m_CurrPlayer].roleId, this.beChallengerDataPre[this.m_CurrPlayer].ranking, (this.m_DetailData.useFreeChallengeNum >= this.m_DetailData.freeChallengeNum) ? 1 : 0);
		}
		else
		{
			GameInstance.Game.SpriteRequestJingJiChallengeCmd(this.beChallengerDataCurrent[this.m_CurrPlayer].roleId, this.beChallengerDataCurrent[this.m_CurrPlayer].ranking, (this.m_DetailData.useFreeChallengeNum >= this.m_DetailData.freeChallengeNum) ? 1 : 0);
		}
	}

	public void updeteLingquTime(long newTime)
	{
		this.m_DetailData.nextRewardTime = newTime;
		long correctLocalTime = Global.GetCorrectLocalTime();
		long lingquInfo = (this.m_DetailData.nextRewardTime - correctLocalTime) / 1000L;
		if (this.m_AwardPart != null)
		{
			this.m_AwardPart.SetLingquInfo(lingquInfo);
		}
	}

	public void refJunxian(int type)
	{
		if (this.m_JunxianPart != null)
		{
			if (type == 1)
			{
				this.m_JunxianPart.refBuff();
			}
			else if (type == 2)
			{
				this.m_JunxianPart.refAttrPart();
			}
		}
		this.RefrashJunxianNZhanli();
	}

	public void refZhanbao(List<JingJiChallengeInfoData> list)
	{
		if (this.m_ZhanbaoPart != null)
		{
			this.m_ZhanbaoPart.refresh(list);
		}
	}

	private void initItems()
	{
		if (this.m_DetailData.beChallengerData != null)
		{
			if (this.isPrePage)
			{
				for (int i = 0; i < this.m_DetailData.beChallengerData.Count; i++)
				{
					if (this.modalInfoPrePageDict.ContainsKey(this.m_DetailData.beChallengerData[i].roleId))
					{
						this.modalInfoPrePageDict[this.m_DetailData.beChallengerData[i].roleId].infos.init(this.m_DetailData.beChallengerData[i], i);
					}
				}
			}
			else
			{
				for (int j = 0; j < this.m_DetailData.beChallengerData.Count; j++)
				{
					if (this.modalInfoDict.ContainsKey(this.m_DetailData.beChallengerData[j].roleId))
					{
						this.modalInfoDict[this.m_DetailData.beChallengerData[j].roleId].infos.init(this.m_DetailData.beChallengerData[j], j);
					}
				}
			}
		}
		SystemHelpMgr.OnAction(UIObjIDs.JingJiChangPart, HelpStateEvents.Actived, 1);
	}

	private int CompareByRanking(PlayerJingJiMiniData x, PlayerJingJiMiniData y)
	{
		return y.ranking.CompareTo(x.ranking);
	}

	private void initXmlData()
	{
		if (this.JingjiXmlList != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/JingJi.xml");
		if (gameResXml != null)
		{
			this.JingjiXmlList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		}
		gameResXml = Global.GetGameResXml("Config/JunXian.xml");
		if (gameResXml != null)
		{
			this.JunxianXmlList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		}
	}

	private void OnEnable()
	{
		base.StartCoroutine<bool>(this.TimeProc());
	}

	private new void OnDestroy()
	{
		int count = this.resLoader.Count;
		for (int i = 0; i < count; i++)
		{
			RoleResLoader roleResLoader = this.resLoader[i];
			roleResLoader.Stop();
		}
		this.resLoader.Clear();
		ActivityTipManager.RegActivityTipItem(4002, null);
	}

	private void clearItems()
	{
		if (this.Items == null)
		{
			return;
		}
		for (int i = 0; i < this.Items.Length; i++)
		{
			Object.Destroy(this.Items[i].gameObject);
			this.Items[i] = null;
		}
		this.Items = null;
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			if (!this.m_Top || this.m_DetailData == null)
			{
				yield return new WaitForSeconds(0.5f);
			}
			else
			{
				long currTime = Global.GetCorrectLocalTime();
				long cd = (this.m_DetailData.nextChallengeTime - currTime) / 1000L;
				if (cd >= 0L)
				{
					this.m_TiaozhanCDLabel.Text = UIHelper.FormatSecs(cd, "-");
					this.m_TiaozhanCD = true;
					if (this.msgBoxPart != null)
					{
						double moneyNum = ConfigSystemParam.GetSystemParamDoubleByName("CDXiaoHaoZhuanShi");
						moneyNum = Math.Ceiling((double)cd * moneyNum);
						string msg = Global.GetLang("消除等待时间需要花费:") + moneyNum + Global.GetLang("钻石");
						this.msgBoxPart.RefreshMessage(msg);
					}
				}
				else if (this.m_TiaozhanCD)
				{
					this.m_TiaozhanCDLabel.Text = Global.GetLang("无");
					this.m_TiaozhanCD = false;
				}
				cd = (this.m_DetailData.nextRewardTime - currTime) / 1000L;
				if (cd >= 0L)
				{
					if (this.m_NewJiangli.gameObject.activeInHierarchy)
					{
						this.m_NewJiangli.gameObject.SetActive(false);
						this.m_NewJiangli.Stop("TiaoJianDaCheng");
					}
				}
				else
				{
					this.m_NewJiangli.gameObject.SetActive(true);
					this.m_NewJiangli.Play("TiaoJianDaCheng");
				}
				if (this.m_AwardPart != null)
				{
					this.m_AwardPart.SetLingquInfo(cd);
				}
				yield return new WaitForSeconds(0.5f);
			}
		}
		yield break;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	private void TiaozhanClicked(int idx, int rankID = -1, int otherRankId = -1)
	{
		if (this.m_DetailData != null && this.m_DetailData.ranking > 100 && 0 < otherRankId && otherRankId <= 3)
		{
			Super.HintMainText(Global.GetLang("前100名才可挑战"), 10, 3);
			return;
		}
		SystemHelpMgr.OnAction(UIObjIDs.JingJiChangPartTianZhanBtn, HelpStateEvents.Clicked, 1);
		this.m_CurrPlayer = idx;
		if (this.m_DetailData.useFreeChallengeNum < this.m_DetailData.freeChallengeNum)
		{
			if (this.m_TiaozhanCD)
			{
				long num = (this.m_DetailData.nextChallengeTime - Global.GetCorrectLocalTime()) / 1000L;
				if (num >= 0L)
				{
					double num2 = ConfigSystemParam.GetSystemParamDoubleByName("CDXiaoHaoZhuanShi");
					num2 = Math.Ceiling((double)num * num2);
					string message = Global.GetLang("消除等待时间需要花费:") + num2 + Global.GetLang("钻石");
					string[] buttons = new string[]
					{
						Global.GetLang("挑战"),
						Global.GetLang("取消")
					};
					GChildWindow gchildWindow = Super.ShowMessageBoxGUI(Global.GetLang("提示"), message, delegate(object s, DPSelectedItemEventArgs e)
					{
						if (e.ID == 0)
						{
							GameInstance.Game.SpriteJingJiRemoveChallengeCDCmd();
						}
						this.msgBoxPart = null;
					}, buttons);
					this.msgBoxPart = gchildWindow.gameObject.GetComponentInChildren<MyMessageBoxExPart>();
					this.ResetLayer(gchildWindow.gameObject, "GUI");
				}
			}
			else if (this.isPrePage)
			{
				GameInstance.Game.SpriteRequestJingJiChallengeCmd(this.beChallengerDataPre[idx].roleId, this.beChallengerDataPre[idx].ranking, 0);
			}
			else
			{
				GameInstance.Game.SpriteRequestJingJiChallengeCmd(this.beChallengerDataCurrent[idx].roleId, this.beChallengerDataCurrent[idx].ranking, 0);
			}
		}
		else if (this.m_DetailData.useChallengeNum < this.m_DetailData.vipChallengeNum)
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("VIPGouMaiJingJi", true);
			string str = Global.GetLang("付费挑战需要花费:") + systemParamByName + Global.GetLang("钻石");
			string[] btns = new string[]
			{
				Global.GetLang("挑战"),
				Global.GetLang("取消")
			};
			GChildWindow gchildWindow2 = Super.ShowMessageBoxGUI(Global.GetLang("提示"), str, delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == 0)
				{
					if (this.m_TiaozhanCD)
					{
						long num3 = (this.m_DetailData.nextChallengeTime - Global.GetCorrectLocalTime()) / 1000L;
						if (num3 >= 0L)
						{
							double num4 = ConfigSystemParam.GetSystemParamDoubleByName("CDXiaoHaoZhuanShi");
							num4 = Math.Ceiling((double)num3 * num4);
							str = Global.GetLang("消除等待时间需要花费:") + num4 + Global.GetLang("钻石");
							GChildWindow gchildWindow4 = Super.ShowMessageBoxGUI(Global.GetLang("提示"), str, delegate(object s1, DPSelectedItemEventArgs e1)
							{
								if (e1.ID == 0)
								{
									GameInstance.Game.SpriteJingJiRemoveChallengeCDCmd();
								}
								this.msgBoxPart = null;
							}, btns);
							this.msgBoxPart = gchildWindow4.gameObject.GetComponentInChildren<MyMessageBoxExPart>();
							this.ResetLayer(gchildWindow4.gameObject, "GUI");
						}
					}
					else if (this.isPrePage)
					{
						GameInstance.Game.SpriteRequestJingJiChallengeCmd(this.beChallengerDataPre[idx].roleId, this.beChallengerDataPre[idx].ranking, 1);
					}
					else
					{
						GameInstance.Game.SpriteRequestJingJiChallengeCmd(this.beChallengerDataCurrent[idx].roleId, this.beChallengerDataCurrent[idx].ranking, 1);
					}
				}
			}, btns);
			this.ResetLayer(gchildWindow2.gameObject, "GUI");
		}
		else
		{
			string lang = Global.GetLang("今日已无挑战次数，提高VIP等级可购买额外的挑战次数！");
			string[] buttons2 = new string[]
			{
				Global.GetLang("确定")
			};
			GChildWindow gchildWindow3 = Super.ShowMessageBoxGUI(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
			{
			}, buttons2);
			this.ResetLayer(gchildWindow3.gameObject, "GUI");
		}
	}

	private bool isNewJunxian()
	{
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang);
		int roleCommonUseParamsValue2 = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
		int num = -1;
		for (int i = 0; i < this.JunxianXmlList.Count; i++)
		{
			XElement xelement = this.JunxianXmlList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
				if (xelementAttributeInt == roleCommonUseParamsValue2)
				{
					num = Global.GetXElementAttributeInt(xelement, "NeedShengWang");
					break;
				}
			}
		}
		return num != -1 && roleCommonUseParamsValue > num;
	}

	private void CreateJunxianPart()
	{
		this.GChildArenaJunxianPart = U3DUtils.NEW<GChildWindow>();
		this.GChildArenaJunxianPart = U3DUtils.NEW<GChildWindow>();
		this.GChildArenaJunxianPart.ModalType = ChildWindowModalType.Translucent;
		this.GChildArenaJunxianPart.Modal = true;
		this.GChildArenaJunxianPart.IsShowModal = true;
		Super.InitChildWindow(this.GChildArenaJunxianPart, "GChildJunxianPart");
		Super.GData.GlobalPlayZone.Children.Add(this.GChildArenaJunxianPart);
		this.m_JunxianPart = U3DUtils.NEW<JunxianPart>();
		this.GChildArenaJunxianPart.Body.Add(this.m_JunxianPart);
		this.m_JunxianPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.m_JunxianPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_JunxianPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == -10)
			{
				this.DestroyJunxianPart();
				this.ShowModals(true);
			}
		};
		this.m_JunxianPart.openAdendaViewEventHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == 1)
			{
				PlayZone.GlobalPlayZone.LoadAdendaView(s, e);
			}
		};
		this.m_Top = false;
		this.m_JunxianPart.init(this.m_DetailData.ranking, this.m_DetailData.winCount, this.JunxianXmlList);
	}

	private void DestroyJunxianPart()
	{
		Object.Destroy(this.GChildArenaJunxianPart.gameObject);
		this.m_JunxianPart = null;
		this.m_Top = true;
	}

	private void CreateZhanbaoPart()
	{
		this.GChildArenaZhanBaoPart = U3DUtils.NEW<GChildWindow>();
		this.GChildArenaZhanBaoPart = U3DUtils.NEW<GChildWindow>();
		this.GChildArenaZhanBaoPart.ModalType = ChildWindowModalType.Translucent;
		this.GChildArenaZhanBaoPart.Modal = true;
		this.GChildArenaZhanBaoPart.IsShowModal = true;
		Super.InitChildWindow(this.GChildArenaZhanBaoPart, "GChildArenaZhanBaoPart");
		Super.GData.GlobalPlayZone.Children.Add(this.GChildArenaZhanBaoPart);
		this.m_ZhanbaoPart = U3DUtils.NEW<ZhanbaoPart>();
		this.GChildArenaZhanBaoPart.Body.Add(this.m_ZhanbaoPart);
		this.m_ZhanbaoPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.m_ZhanbaoPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_ZhanbaoPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == -10)
			{
				this.DestroyZhanbaoPart();
				this.ShowModals(true);
			}
		};
		this.m_Top = false;
		this.m_ZhanbaoPart.init(this.m_DetailData.ranking, this.m_DetailData.maxwincount, this.JunxianXmlList);
	}

	private void DestroyZhanbaoPart()
	{
		Object.Destroy(this.GChildArenaZhanBaoPart.gameObject);
		this.m_ZhanbaoPart = null;
		this.m_Top = true;
	}

	private void CreateAwardPart()
	{
		this.GChildArenaJiangliPart = U3DUtils.NEW<GChildWindow>();
		this.GChildArenaJiangliPart.ModalType = ChildWindowModalType.Translucent;
		this.GChildArenaJiangliPart.Modal = true;
		this.GChildArenaJiangliPart.IsShowModal = true;
		Super.InitChildWindow(this.GChildArenaJiangliPart, "GChildArenaJiangliPart");
		Super.GData.GlobalPlayZone.Children.Add(this.GChildArenaJiangliPart);
		this.m_AwardPart = U3DUtils.NEW<JingjiAwardPart>();
		this.GChildArenaJiangliPart.Body.Add(this.m_AwardPart);
		this.m_AwardPart.transform.localPosition = new Vector3(0f, 0f, -2f);
		this.m_AwardPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_AwardPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.Type == -10)
			{
				this.DestroyAwardPart();
				this.ShowModals(true);
			}
		};
		this.m_AwardPart.init(this.JingjiXmlList);
		List<GGoodIcon> rewardGoodIcon = this.CalRewardGoods();
		this.m_AwardPart.SetRewardGoodIcon(rewardGoodIcon);
		long correctLocalTime = Global.GetCorrectLocalTime();
		long lingquInfo = (this.m_DetailData.nextRewardTime - correctLocalTime) / 1000L;
		this.m_AwardPart.SetLingquInfo(lingquInfo);
		this.m_AwardPart.SetRankingData(this.m_DetailData.ranking);
	}

	private void DestroyAwardPart()
	{
		Object.Destroy(this.GChildArenaJiangliPart.gameObject);
		this.m_AwardPart = null;
	}

	public void ShowJunXianPart()
	{
		ArenaPart.needToOpenJunxian = true;
	}

	public void NotifyRemoveCDResult(int errCode)
	{
		if (errCode == 1)
		{
			this.updeteTiaozhanTime();
		}
		else
		{
			this.ProcessErrorCode(errCode);
		}
	}

	public void NotifyChanllageResult(int errcode)
	{
		this.ProcessErrorCode(errcode);
		if (errcode == -11)
		{
			GameInstance.Game.SpriteJingJiDetailCmd(0);
		}
	}

	public void ProcessErrorCode(int errCode)
	{
		if (errCode == 1)
		{
			return;
		}
		string message = string.Empty;
		switch (errCode + 16)
		{
		case 0:
			message = Global.GetLang("玩家声望值不够！");
			break;
		case 1:
			message = Global.GetLang("当前有军衔BUFF！");
			break;
		case 2:
			message = Global.GetLang("玩家没有军衔！");
			break;
		case 3:
			message = Global.GetLang("排行榜领取奖励未冷却！");
			break;
		case 4:
			message = Global.GetLang("被挑战者正在被其他玩家挑战！");
			break;
		case 5:
			message = Global.GetLang("被挑战者排名已改变，请在刷新后重试！");
			break;
		case 6:
			message = Global.GetLang("被挑战者不存在！");
			break;
		case 7:
			message = Global.GetLang("冷却时间未到！");
			break;
		case 8:
			message = Global.GetLang("无效副本顺序！");
			break;
		case 9:
			message = Global.GetLang("无效地图！");
			break;
		case 10:
			message = Global.GetLang("钻石不足，无法进行操作！");
			break;
		case 11:
			message = Global.GetLang("付费失败！");
			break;
		case 12:
			message = Global.GetLang("vip次数不足，无法进行操作！");
			break;
		case 13:
			message = Global.GetLang("免费次数不足，无法进行操作！");
			break;
		case 14:
			message = Global.GetLang("死亡时不允许发起挑战！");
			break;
		case 15:
			message = Global.GetLang("战斗时不允许发起挑战！");
			break;
		case 16:
			message = Global.GetLang("非法参数！");
			break;
		default:
			message = Global.GetLang("其他错误，错误码[") + errCode + Global.GetLang("]！");
			break;
		}
		string[] buttons = new string[]
		{
			Global.GetLang("确定")
		};
		GChildWindow gchildWindow = Super.ShowMessageBoxGUI(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
		{
		}, buttons);
		this.ResetLayer(gchildWindow.gameObject, "GUI");
	}

	private XElement getElementByLevel(int value)
	{
		int count = this.JunxianXmlList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.JunxianXmlList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
				if (value == xelementAttributeInt)
				{
					return xelement;
				}
			}
		}
		return null;
	}

	private List<GGoodIcon> CalRewardGoods()
	{
		List<GGoodIcon> list = new List<GGoodIcon>();
		for (int i = this.JingjiXmlList.Count - 1; i > -1; i--)
		{
			XElement xelement = this.JingjiXmlList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinRank");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxRank");
				if ((this.m_DetailData.ranking >= xelementAttributeInt && this.m_DetailData.ranking <= xelementAttributeInt2) || this.m_DetailData.ranking == -1)
				{
					string[] array = Global.GetXElementAttributeStr(xelement, "GoodsID").Split(new char[]
					{
						','
					});
					int id = Convert.ToInt32(array[0]);
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
					GGoodIcon ggoodIcon = this.CreateGGoodsIcon(array);
					list.Add(ggoodIcon);
					BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
					component.center = new Vector3(0f, 0f, -1f);
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ExpCoefficient2");
					array = ("8002," + xelementAttributeStr + ",0,0,0,0,0").Split(new char[]
					{
						','
					});
					ggoodIcon = this.CreateGGoodsIcon(array);
					list.Add(ggoodIcon);
					component = ggoodIcon.transform.GetComponent<BoxCollider>();
					component.center = new Vector3(0f, 0f, -1f);
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ShengWang2");
					array = ("8016," + xelementAttributeStr2 + ",0,0,0,0,0").Split(new char[]
					{
						','
					});
					ggoodIcon = this.CreateGGoodsIcon(array);
					list.Add(ggoodIcon);
					component = ggoodIcon.transform.GetComponent<BoxCollider>();
					component.center = new Vector3(0f, 0f, -1f);
					break;
				}
			}
		}
		return list;
	}

	private GGoodIcon CreateGGoodsIcon(string[] goods)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
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
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.ShowGoodsTip(s);
			};
		}
		return ggoodIcon;
	}

	private void RefrashJunxianNZhanli()
	{
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
		if (roleCommonUseParamsValue <= 0)
		{
			this.lblJunxian.text = Global.GetLang("无");
		}
		else
		{
			this.lblJunxian.text = Global.GetXElementAttributeStr(this.getElementByLevel(roleCommonUseParamsValue), "Name");
		}
		this.lblZhanli.text = string.Empty + Global.Data.roleData.CombatForce;
	}

	public void NotifyOnGetOtherAttr(RoleData4Selector rd)
	{
		this.SentAndRecv--;
		if (this.isPrePage)
		{
			if (this.modalInfoPrePageDict.ContainsKey(rd.RoleID))
			{
				ArenaPart.ModalInfo modalInfo = this.modalInfoPrePageDict[rd.RoleID];
				modalInfo.roleData = rd;
			}
		}
		else if (this.modalInfoDict.ContainsKey(rd.RoleID))
		{
			ArenaPart.ModalInfo modalInfo2 = this.modalInfoDict[rd.RoleID];
			modalInfo2.roleData = rd;
		}
		if (this.SentAndRecv == 0)
		{
			this.Load3DModual();
		}
	}

	private void Load3DModual()
	{
		if (this.isPrePage)
		{
			foreach (int num in this.modalInfoPrePageDict.Keys)
			{
				RoleData4Selector roleData = this.modalInfoPrePageDict[num].roleData;
				if (roleData != null)
				{
					int fashionGoodsID = Global.GetFashionGoodsID(roleData.FashionWingsID);
					Modal3DShow modalShow = this.modalInfoPrePageDict[num].modalShow;
					RoleResLoader roleResLoader = UIHelper.LoadRoleRes(modalShow, roleData.SettingBitFlags, roleData.Occupation, roleData.SubOccupation, roleData.RoleName, roleData.GoodsDataList, null, roleData.MyWingData, 1f, fashionGoodsID, null, false);
					if (roleResLoader != null)
					{
						this.resLoader.Add(roleResLoader);
					}
					if (null != modalShow)
					{
						UIHelper.SetModalPosZ(modalShow.transform);
					}
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						num + Global.GetLang("的角色 的 RoleData为空")
					});
				}
			}
		}
		else
		{
			foreach (int num2 in this.modalInfoDict.Keys)
			{
				RoleData4Selector roleData = this.modalInfoDict[num2].roleData;
				Modal3DShow modalShow = this.modalInfoDict[num2].modalShow;
				if (roleData != null)
				{
					int fashionGoodsID2 = Global.GetFashionGoodsID(roleData.FashionWingsID);
					if (roleData != null && modalShow != null)
					{
						RoleResLoader roleResLoader = UIHelper.LoadRoleRes(modalShow, roleData.SettingBitFlags, roleData.Occupation, roleData.SubOccupation, roleData.RoleName, roleData.GoodsDataList, null, roleData.MyWingData, 1f, fashionGoodsID2, null, false);
						if (roleResLoader != null)
						{
							this.resLoader.Add(roleResLoader);
						}
						UIHelper.SetModalPosZ(modalShow.transform);
					}
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						num2 + Global.GetLang("的角色 的 RoleData为空  这里不是空指针  是我们自己打的log")
					});
				}
			}
		}
	}

	private void GetOtherAttr()
	{
		this.ClearModal();
		Modal3DShow modal3DShow = null;
		for (int i = 0; i < this.m_DetailData.beChallengerData.Count; i++)
		{
			GameInstance.Game.GetJingJiChangRoleLooks(this.m_DetailData.beChallengerData[i].roleId);
			this.SentAndRecv++;
			if (this.isPrePage)
			{
				ArenaPartOtherPlayerItem arenaPartOtherPlayerItem = U3DUtils.NEW<ArenaPartOtherPlayerItem>();
				Transform transform = this.groupPrePage.transform.FindChild(string.Empty + i);
				if (transform != null)
				{
					modal3DShow = transform.GetComponent<Modal3DShow>();
					arenaPartOtherPlayerItem.transform.parent = modal3DShow.gameObject.transform;
					arenaPartOtherPlayerItem.transform.localScale = new Vector3(0.667f, 0.667f, 0f);
					arenaPartOtherPlayerItem.transform.localPosition = new Vector3(0f, -22f, -300f);
				}
				arenaPartOtherPlayerItem.m_TiaozhanBtn.tag = string.Empty + i;
				arenaPartOtherPlayerItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.TiaozhanClicked(e.ID, this.m_DetailData.ranking, e.Flag);
				};
				ArenaPart.ModalInfo modalInfo = new ArenaPart.ModalInfo();
				modalInfo.infos = arenaPartOtherPlayerItem;
				modalInfo.modalShow = modal3DShow;
				this.modalInfoPrePageDict.Add(this.m_DetailData.beChallengerData[i].roleId, modalInfo);
			}
			else
			{
				ArenaPartOtherPlayerItem arenaPartOtherPlayerItem = U3DUtils.NEW<ArenaPartOtherPlayerItem>();
				Transform transform = this.groupCurrentPage.transform.FindChild(string.Empty + i);
				if (transform != null)
				{
					modal3DShow = transform.GetComponent<Modal3DShow>();
					arenaPartOtherPlayerItem.transform.parent = modal3DShow.gameObject.transform;
					arenaPartOtherPlayerItem.transform.localScale = new Vector3(0.667f, 0.667f, 0f);
					arenaPartOtherPlayerItem.transform.localPosition = new Vector3(0f, -22f, -300f);
				}
				arenaPartOtherPlayerItem.m_TiaozhanBtn.tag = string.Empty + i;
				arenaPartOtherPlayerItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.TiaozhanClicked(e.ID, this.m_DetailData.ranking, e.Flag);
				};
				ArenaPart.ModalInfo modalInfo2 = new ArenaPart.ModalInfo();
				modalInfo2.infos = arenaPartOtherPlayerItem;
				modalInfo2.modalShow = modal3DShow;
				this.modalInfoDict.Add(this.m_DetailData.beChallengerData[i].roleId, modalInfo2);
			}
		}
	}

	private void ClearModal()
	{
		if (this.isPrePage)
		{
			foreach (int num in this.modalInfoPrePageDict.Keys)
			{
				Modal3DShow modalShow = this.modalInfoPrePageDict[num].modalShow;
				int childCount = modalShow.transform.childCount;
				for (int i = 0; i < childCount; i++)
				{
					Object.Destroy(modalShow.transform.GetChild(i).gameObject);
				}
			}
			this.modalInfoPrePageDict.Clear();
		}
		else
		{
			foreach (int num2 in this.modalInfoDict.Keys)
			{
				Modal3DShow modalShow2 = this.modalInfoDict[num2].modalShow;
				int childCount2 = modalShow2.transform.childCount;
				for (int j = 0; j < childCount2; j++)
				{
					Object.Destroy(modalShow2.transform.GetChild(j).gameObject);
				}
			}
			this.modalInfoDict.Clear();
		}
	}

	private void ModalClicked(int x, int y)
	{
		if (y <= 341 && y >= 64)
		{
			if (x >= 90 && x <= 250)
			{
				this.TiaozhanClicked(0, -1, -1);
			}
			else if (x >= 400 && x <= 560)
			{
				this.TiaozhanClicked(1, -1, -1);
			}
			else if (x >= 700 && x <= 860)
			{
				this.TiaozhanClicked(2, -1, -1);
			}
		}
	}

	private void ShowModals(bool showSwitch)
	{
		this.groupPrePage.SetActive(showSwitch);
		this.groupCurrentPage.SetActive(showSwitch);
		this.m_btnPrePage.gameObject.SetActive(this.isPrePage || !this.isTo3);
		this.m_btnLastPage.gameObject.SetActive(this.isPrePage);
	}

	private void ResetLayer(GameObject obj, string layerName)
	{
	}

	public Transform GetShengWangLinkBtn()
	{
		if (null == this.m_JunxianPart)
		{
			return null;
		}
		return (!(null == this.m_JunxianPart.agendaBtn)) ? this.m_JunxianPart.agendaBtn.transform : null;
	}

	public UIButton btnJunxian;

	public UIButton btnZhanbao;

	public UIButton btnJiangli;

	public UIButton btnPaihang;

	public GButton m_LingquBtn;

	public GButton m_CloseBtn;

	public GButton m_btnPrePage;

	public GButton m_btnLastPage;

	public TextBlock m_TiaozhanLabel;

	public TextBlock m_VIPTiaozhanLabel;

	public TextBlock m_TiaozhanCDLabel;

	public TextBlock m_RankingLabel;

	public TextBlock m_LingquCDLabel;

	public UISprite m_GoodBg;

	public Animation m_NewJunxian;

	public Animation m_NewJiangli;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ArenaPartOtherPlayerItem[] Items;

	private JunxianPart m_JunxianPart;

	private ZhanbaoPart m_ZhanbaoPart;

	private JingjiAwardPart m_AwardPart;

	private JingJiDetailData m_DetailData;

	private bool m_Top = true;

	private List<XElement> JingjiXmlList;

	private List<XElement> JunxianXmlList;

	private bool m_TiaozhanCD;

	private int m_CurrPlayer;

	public static bool needToOpenJunxian;

	public UILabel lblZhanli;

	public UILabel lblJunxian;

	public MyMessageBoxExPart msgBoxPart;

	public GameObject groupCurrentPage;

	public GameObject groupPrePage;

	public Adenda adendaViewController;

	private Dictionary<int, ArenaPart.ModalInfo> modalInfoDict = new Dictionary<int, ArenaPart.ModalInfo>();

	private Dictionary<int, ArenaPart.ModalInfo> modalInfoPrePageDict = new Dictionary<int, ArenaPart.ModalInfo>();

	private List<PlayerJingJiMiniData> beChallengerDataCurrent;

	private List<PlayerJingJiMiniData> beChallengerDataPre;

	private int SentAndRecv;

	private bool isPrePage;

	private bool isTo3;

	protected GChildWindow GChildArenaJunxianPart;

	protected GChildWindow GChildArenaZhanBaoPart;

	protected GChildWindow GChildArenaJiangliPart;

	private List<RoleResLoader> resLoader = new List<RoleResLoader>();

	private class ModalInfo
	{
		public Modal3DShow modalShow;

		public RoleData4Selector roleData;

		public ArenaPartOtherPlayerItem infos;
	}
}
