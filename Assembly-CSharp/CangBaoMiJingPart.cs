using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class CangBaoMiJingPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitXML();
		this.InitPrefabText();
		this.InitHandler();
		this.InitRoleData();
		this.InitRoleHead();
		this.InitBGTexture();
		this.GetDataFromNet();
	}

	public override void Destroy()
	{
		base.Destroy();
		NGUITools.Destroy(this);
		NGUITools.Destroy(base.gameObject);
		this.m_xmlCangBao_MapData.Clear();
		this.m_xmlCangBao_EventData.Clear();
		this.m_xmlCangBao_BoxData.Clear();
		base.CancelInvoke("TickProc");
	}

	public override void Update()
	{
		base.Update();
		if (this.m_RollData != null)
		{
			this.m_RollData.Update(Time.deltaTime);
		}
	}

	private void InitRoleHead()
	{
		this.m_RoleHeadroot.localPosition = new Vector3(0f, 0f, -9f);
		this.m_Rolehead = U3DUtils.NEW<CangBaoMiJingRoleHeadPart>();
		this.m_Rolehead.transform.SetParent(this.m_RoleHeadroot, false);
		this.m_Rolehead.Handler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.ID == 0)
			{
				this.RoleMoveDoneOneFloor(false);
			}
		};
		if (this.m_Rolehead.gameObject.activeSelf)
		{
			NGUITools.SetActiveSelf(this.m_Rolehead.gameObject, false);
		}
	}

	private void InitPrefabText()
	{
		this.m_NormolBtn.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"edddbf",
			Global.GetLang("普通骰子")
		});
		this.m_MiracleBtn.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("奇迹骰子")
		});
		this.m_NormolHint_Label[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"edddbf",
			Global.GetLang("普通骰子：")
		});
		this.m_MiracleHint_Label[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("奇迹骰子：")
		});
		this.m_ChongzhiTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("重置倒计时："),
			"3eb431",
			Global.GetLang("小于一分钟")
		});
	}

	private void ShowShiJianPart(OnePieceTreasureEvent data, int floorID)
	{
		if (!this.EventWin(CangBaoMiJingPart.EventWithUIRelation.CanShowOnUI))
		{
			return;
		}
		int num = 0;
		Data_CangBao_Event data_CangBao_Event = null;
		if (this.m_xmlCangBao_EventData.TryGetValue(data.EventID, ref data_CangBao_Event))
		{
			num = data_CangBao_Event.Type;
			this.m_EventIsOnUI++;
		}
		if (num == 2 || num == 4)
		{
			GChildWindow m_MessageWin_ = this.CreateGCWin(-100);
			CangBaoMiJingShiJian m_ShiJian = U3DUtils.NEW<CangBaoMiJingShiJian>();
			m_ShiJian.transform.SetParent(m_MessageWin_.Body.transform, false);
			m_ShiJian.RefreshContent(data_CangBao_Event);
			m_ShiJian.Btnhandler = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (0 < this.m_EventIsOnUI)
				{
					this.m_EventIsOnUI--;
				}
				if (s.ID == 1)
				{
					if (m_ShiJian.EventType == 4)
					{
						Data_CangBao_Event data_CangBao_Event2 = null;
						if (this.m_xmlCangBao_EventData.TryGetValue(s.MyID, ref data_CangBao_Event2))
						{
							if (Global.Data.CurrentCopyTeamData != null)
							{
								Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
								{
									if (e2.ID == 0)
									{
										GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
										this.RemoveEventAtList(0);
										GameInstance.Game.SendOnePieceTiggerEvent();
										Super.ShowNetWaiting(null);
									}
								}, -1);
							}
							else
							{
								this.RemoveEventAtList(0);
								GameInstance.Game.SendOnePieceTiggerEvent();
								Super.ShowNetWaiting(null);
							}
						}
					}
					else if (m_ShiJian.EventType == 2)
					{
						GameInstance.Game.SendOnePieceTiggerEvent();
						Super.ShowNetWaiting(null);
					}
				}
				else if (s.ID == 1)
				{
					this.SwitchEventBtn();
				}
				if (null != m_ShiJian)
				{
					NGUITools.Destroy(m_ShiJian.gameObject);
				}
				if (null != m_MessageWin_)
				{
					NGUITools.Destroy(m_MessageWin_.gameObject);
				}
				m_ShiJian = null;
				m_MessageWin_ = null;
			};
		}
		else if (num == 5)
		{
			CangbaoMiJingBoxPart Boxpart = U3DUtils.NEW<CangbaoMiJingBoxPart>();
			List<Data_CangBao_Box> list = new List<Data_CangBao_Box>();
			for (int i = 0; i < data.BoxList.Count; i++)
			{
				CangBaoData_Box cangBaoData_Box = null;
				int num2 = data.BoxList[i] / 1000;
				int num3 = data.BoxList[i] % 1000;
				if (this.m_xmlCangBao_BoxData.TryGetValue(num2, ref cangBaoData_Box))
				{
					Data_CangBao_Box data_CangBao_Box = null;
					if (cangBaoData_Box.DataCangBao_Map.TryGetValue(num3, ref data_CangBao_Box))
					{
						list.Add(data_CangBao_Box);
					}
				}
			}
			Boxpart.AddGoods(list);
			GChildWindow m_MessageWin_ = this.CreateGCWin(-100);
			Boxpart.transform.SetParent(m_MessageWin_.Body.transform, false);
			Boxpart.handler = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.ID == 0)
				{
					this.SendOnePieceMove(1);
					if (null != Boxpart)
					{
						NGUITools.Destroy(Boxpart.gameObject);
					}
					if (null != m_MessageWin_)
					{
						NGUITools.Destroy(m_MessageWin_.gameObject);
					}
					Boxpart = null;
					m_MessageWin_ = null;
					this.RemoveEventAtList(0);
				}
				if (0 < this.m_EventIsOnUI)
				{
					this.m_EventIsOnUI--;
				}
			};
		}
		else if (num == 1)
		{
			this.RemoveEventAtList(0);
		}
	}

	private void ShowMUDuiHuan()
	{
		GChildWindow m_MessageWin_ = this.CreateGCWin(-120);
		m_MessageWin_.Modal = true;
		this.m_MUDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
		this.m_MUDuiHuanPart.transform.SetParent(m_MessageWin_.Body.transform, false);
		this.m_MUDuiHuanPart.transform.localScale = new Vector3(1f, 1f, 1f);
		this.m_MUDuiHuanPart.InitPartData(5, 0);
		this.m_MUDuiHuanPart.DPSelectedItem = delegate(object s1, DPSelectedItemEventArgs e)
		{
			NGUITools.Destroy(m_MessageWin_.gameObject);
			NGUITools.Destroy(this.m_MUDuiHuanPart.gameObject);
			this.m_MUDuiHuanPart = null;
			return false;
		};
	}

	private void ShowHelpPart()
	{
		string lang = Global.GetLang("游戏规则：");
		string lang2 = Global.GetLang("1. 投掷骰子移动，每天有");
		string text = string.Concat(new string[]
		{
			Global.GetLang("免费移动机会"),
			Environment.NewLine,
			Global.GetLang("2. 移动后，停留在事件格子上会触发"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("随机事件")
			}),
			Environment.NewLine,
			Global.GetLang("3. 移动或完成事件会获得"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("血钻")
			}),
			Global.GetLang("和"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("秘境点数")
			}),
			Global.GetLang("，可以在秘"),
			Environment.NewLine,
			Global.GetLang("境商店兑换道具"),
			Environment.NewLine,
			Global.GetLang("4. 移动到每层终点会自动传送下一层，并获得开"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("启秘")
			}),
			Environment.NewLine,
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("境宝箱")
			}),
			Global.GetLang("机会")
		});
		string lang3 = Global.GetLang("重置时间：");
		string str = string.Concat(new string[]
		{
			Global.GetLang("1. "),
			Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("每周一重置位置")
			}),
			Global.GetLang("，重置"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("不清空")
			}),
			Global.GetLang("血钻和秘境"),
			Global.GetLang("点数")
		});
		string lang4 = Global.GetLang("地图详情：");
		string str2 = string.Concat(new string[]
		{
			Global.GetLang("1. 共"),
			Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				"10"
			}),
			Global.GetLang("层地图，每层"),
			string.Format("{0}", CangBaoMiJingPart.MAXFLOORNUM),
			Global.GetLang("个格子"),
			Environment.NewLine,
			Global.GetLang("2. "),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("移动到最终层终点后，从第一层起点重新出发")
			})
		});
		GChildWindow m_MessageWin_ = this.CreateGCWin(-100);
		CangBaoMiJingHelpPart m_Help = U3DUtils.NEW<CangBaoMiJingHelpPart>();
		m_Help.transform.SetParent(m_MessageWin_.Body.transform, false);
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TreasureFreeNum", ',');
		int num = systemParamIntArrayByName[0] + systemParamIntArrayByName[1];
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"17e43e",
			StringUtil.substitute(Global.GetLang("{0}次"), new object[]
			{
				num
			})
		});
		m_Help.RefreshContent(lang, lang2 + colorStringForNGUIText + text, lang3, str, lang4, str2);
		m_Help.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != m_Help)
			{
				NGUITools.Destroy(m_Help.gameObject);
			}
			if (null != m_MessageWin_)
			{
				NGUITools.Destroy(m_MessageWin_.gameObject);
			}
			m_Help = null;
			m_MessageWin_ = null;
		};
	}

	private void ShowMessage(string Tips, string Inf)
	{
		GChildWindow m_MessageWin_ = this.CreateGCWin(-100);
		CangBaoMiJingMessage m_Message_ = U3DUtils.NEW<CangBaoMiJingMessage>();
		m_Message_.transform.SetParent(m_MessageWin_.Body.transform, false);
		m_Message_.RefreshMessage(Tips, Inf);
		m_Message_.Closehandler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != m_Message_)
			{
				NGUITools.Destroy(m_Message_.gameObject);
			}
			if (null != m_MessageWin_)
			{
				NGUITools.Destroy(m_MessageWin_.gameObject);
			}
			m_Message_ = null;
			m_MessageWin_ = null;
		};
	}

	private void InitHandler()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || this.m_RollAnimationIsPlaying)
			{
				Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
			}
			else
			{
				this.Closehandler(e, new DPSelectedItemEventArgs());
			}
		};
		this.m_ShopBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || this.m_RollAnimationIsPlaying)
			{
				Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
			}
			else if (!this.m_RoleHeadIsMoving)
			{
				this.ShowMUDuiHuan();
			}
		};
		this.m_Thingsbtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || this.m_RollAnimationIsPlaying)
			{
				Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
			}
			else if (!this.m_RoleHeadIsMoving && this.CanShowEvent())
			{
				this.ShowShiJianPart(this.m_NotDoneEventList[0], this.m_Rolehead.RoleOnFloorID);
			}
		};
		this.SwitchEventBtn();
		this.m_helpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || this.m_RollAnimationIsPlaying)
			{
				Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
			}
			else if (!this.m_RoleHeadIsMoving)
			{
				this.ShowHelpPart();
			}
		};
		this.m_MiracleBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || !this.m_MiracleBtnCCanClick || !this.EventWin(CangBaoMiJingPart.EventWithUIRelation.IsNotOnUI) || this.m_RollAnimationIsPlaying)
			{
				if (this.m_RoleHeadIsMoving)
				{
					Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
				}
				else if (!this.m_MiracleBtnCCanClick)
				{
					Super.HintMainText(Global.GetLang("请不要过快的点击"), 10, 3);
				}
				else
				{
					Super.HintMainText(Global.GetLang("尚有未处理事件"), 10, 3);
				}
			}
			else
			{
				if (this.CanShowEvent())
				{
					if (Super.MessageBoxIsHint[7] == 0)
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("还有事件没有处理，是否确认放弃？"), 2, null, MessBoxIsHintTypes.CangBaoMiJingEventHint);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								this.ShowHintPart(CangBaoMiJingHintPart.HintType.Miracle, 0);
							}
							return true;
						};
					}
					else
					{
						this.ShowHintPart(CangBaoMiJingHintPart.HintType.Miracle, 0);
					}
					this.m_m_MiracleCleck = true;
				}
				else
				{
					this.ShowHintPart(CangBaoMiJingHintPart.HintType.Miracle, 0);
				}
				this.m_MiracleBtnCCanClick = false;
				base.StartCoroutine<bool>(this.ChangeValue_Time(new Action<bool, int>(this.ChangeValue), true, 2));
			}
		};
		this.m_NormolBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || !this.m_NormolBtnCanClick || !this.EventWin(CangBaoMiJingPart.EventWithUIRelation.IsNotOnUI) || this.m_RollAnimationIsPlaying)
			{
				if (this.m_RoleHeadIsMoving)
				{
					Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
				}
				else if (!this.m_NormolBtnCanClick)
				{
					Super.HintMainText(Global.GetLang("请不要过快的点击"), 10, 3);
				}
				else if (!this.EventWin(CangBaoMiJingPart.EventWithUIRelation.IsNotOnUI))
				{
					Super.HintMainText(Global.GetLang("尚有未处理事件"), 10, 3);
				}
			}
			else
			{
				if (this.CanShowEvent())
				{
					if (Super.MessageBoxIsHint[7] == 0)
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("还有事件没有处理，是否确认放弃？"), 2, null, MessBoxIsHintTypes.CangBaoMiJingEventHint);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(this.Container, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								this.SendOnePieceROLL();
							}
							return true;
						};
					}
					else
					{
						this.SendOnePieceROLL();
					}
				}
				else
				{
					this.SendOnePieceROLL();
				}
				this.m_NormolBtnCanClick = false;
				base.StartCoroutine<bool>(this.ChangeValue_Time(new Action<bool, int>(this.ChangeValue), true, 1));
			}
		};
		this.m_Normolint_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || this.m_RollAnimationIsPlaying)
			{
				Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
			}
			else if (this.m_TreasureNormolNum >= 99)
			{
				Super.HintMainText(Global.GetLang("普通骰子的个数已达到最大"), 10, 3);
			}
			else
			{
				this.ShowHintPart(CangBaoMiJingHintPart.HintType.Buy_N_Dice, this.m_TreasureNormolNum);
			}
		};
		this.m_MiracleHint_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.m_RoleHeadIsMoving || this.m_RollAnimationIsPlaying)
			{
				Super.HintMainText(Global.GetLang("角色正在移动"), 10, 3);
			}
			else if (this.m_TreasureMiracleNum >= 99)
			{
				Super.HintMainText(Global.GetLang("奇迹骰子的个数已达到最大"), 10, 3);
			}
			else
			{
				this.ShowHintPart(CangBaoMiJingHintPart.HintType.Buy_M_Dice, this.m_TreasureMiracleNum);
			}
		};
	}

	private void SendOnePieceROLL()
	{
		if (this.m_TreasureNormolNum > 0)
		{
			GameInstance.Game.SendOnePieceROLL();
			Super.ShowNetWaiting(null);
		}
		else
		{
			this.ErrorCodeMessage(15, string.Empty);
		}
	}

	private void SendOnePieceROLL_MIRACLE(int num)
	{
		if (this.m_TreasureMiracleNum > 0)
		{
			GameInstance.Game.SendOnePieceROLL_MIRACLE(num);
			Super.ShowNetWaiting(null);
		}
		else
		{
			this.ErrorCodeMessage(15, string.Empty);
		}
	}

	private void SendOnePieceMove(int index)
	{
		GameInstance.Game.SendOnePieceMove();
		Super.ShowNetWaiting(null);
	}

	private void InitXML()
	{
		this.xml_TreasureBox = Global.GetGameResXml("GameRes/Config/Treasure/TreasureBox.xml");
		this.xml_TreasureEvent = Global.GetGameResXml("GameRes/Config/Treasure/TreasureEvent.xml");
		this.xml_TreasureMap = Global.GetGameResXml("GameRes/Config/Treasure/TreasureMap.xml");
		this.xml_TreasureRoute = Global.GetGameResXml("GameRes/Config/Treasure/TreasureRoute.xml");
		if (this.xml_TreasureMap != null)
		{
			CangBaoData_Map cangBaoData_Map = null;
			List<XElement> xelementList = Global.GetXElementList(this.xml_TreasureMap, "TreasureMap");
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Num");
				if (xelementAttributeInt == 0)
				{
					cangBaoData_Map = new CangBaoData_Map();
				}
				Data_CangBao_Map data_CangBao_Map = new Data_CangBao_Map();
				data_CangBao_Map.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				data_CangBao_Map.Num = xelementAttributeInt;
				data_CangBao_Map.Floor = Global.GetXElementAttributeInt(xelementList[i], "Floor");
				data_CangBao_Map.Pic = Global.GetXElementAttributeStr(xelementList[i], "Pic");
				data_CangBao_Map.Trigger = Global.GetXElementAttributeInt(xelementList[i], "Trigger");
				data_CangBao_Map.Score = Global.GetXElementAttributeInt(xelementList[i], "Score");
				data_CangBao_Map.Event = Global.GetXElementAttributeInt(xelementList[i], "Event");
				data_CangBao_Map.Tips = Global.GetXElementAttributeStr(xelementList[i], "Tips");
				data_CangBao_Map.Intro = Global.GetXElementAttributeStr(xelementList[i], "Intro");
				cangBaoData_Map.DataCangBao_Map.Add(data_CangBao_Map.Num, data_CangBao_Map);
				cangBaoData_Map.Floor = data_CangBao_Map.Floor;
				if (CangBaoMiJingPart.MAXFLOORNUM == xelementAttributeInt)
				{
					this.m_xmlCangBao_MapData.Add(cangBaoData_Map.Floor, cangBaoData_Map);
				}
			}
		}
		if (this.xml_TreasureEvent != null)
		{
			List<XElement> xelementList2 = Global.GetXElementList(this.xml_TreasureEvent, "TreasureEvent");
			for (int j = 0; j < xelementList2.Count; j++)
			{
				Data_CangBao_Event data_CangBao_Event = new Data_CangBao_Event();
				data_CangBao_Event.ID = Global.GetXElementAttributeInt(xelementList2[j], "ID");
				data_CangBao_Event.Type = Global.GetXElementAttributeInt(xelementList2[j], "Type");
				data_CangBao_Event.NewGoods = Global.GetXElementAttributeStr(xelementList2[j], "NewGoods");
				data_CangBao_Event.NewValue = Global.GetXElementAttributeStr(xelementList2[j], "NewValue");
				data_CangBao_Event.NeedGoods = Global.GetXElementAttributeStr(xelementList2[j], "NeedGoods");
				data_CangBao_Event.NeedValue = Global.GetXElementAttributeStr(xelementList2[j], "NeedValue");
				data_CangBao_Event.Move = Global.GetXElementAttributeStr(xelementList2[j], "Move");
				data_CangBao_Event.FuBenID = Global.GetXElementAttributeInt(xelementList2[j], "FuBenID");
				data_CangBao_Event.Box = Global.GetXElementAttributeStr(xelementList2[j], "Box");
				data_CangBao_Event.Descriptio = Global.GetXElementAttributeStr(xelementList2[j], "Descriptio");
				this.m_xmlCangBao_EventData.Add(data_CangBao_Event.ID, data_CangBao_Event);
			}
		}
		if (this.xml_TreasureBox != null)
		{
			List<XElement> xelementList3 = Global.GetXElementList(this.xml_TreasureBox, "Box");
			for (int k = 0; k < xelementList3.Count; k++)
			{
				CangBaoData_Box cangBaoData_Box = new CangBaoData_Box();
				cangBaoData_Box.ID = Global.GetXElementAttributeInt(xelementList3[k], "ID");
				List<XElement> xelementList4 = Global.GetXElementList(xelementList3[k], "Treasure");
				for (int l = 0; l < xelementList4.Count; l++)
				{
					Data_CangBao_Box data_CangBao_Box = new Data_CangBao_Box();
					data_CangBao_Box.ID = Global.GetXElementAttributeInt(xelementList4[l], "ID");
					data_CangBao_Box.Type = Global.GetXElementAttributeInt(xelementList4[l], "Type");
					data_CangBao_Box.Goods = Global.GetXElementAttributeStr(xelementList4[l], "Goods");
					data_CangBao_Box.BeginNum = Global.GetXElementAttributeInt(xelementList4[l], "BeginNum");
					data_CangBao_Box.EndNum = Global.GetXElementAttributeInt(xelementList4[l], "EndNum");
					cangBaoData_Box.DataCangBao_Map.Add(data_CangBao_Box.ID, data_CangBao_Box);
				}
				this.m_xmlCangBao_BoxData.Add(cangBaoData_Box.ID, cangBaoData_Box);
			}
		}
		if (this.xml_TreasureRoute != null)
		{
			List<XElement> xelementList5 = Global.GetXElementList(this.xml_TreasureRoute, "TreasureRoute");
			for (int m = 0; m < xelementList5.Count; m++)
			{
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList5[m], "ID");
				if (!this.m_MapData.ContainsKey(xelementAttributeInt2))
				{
					this.m_MapData.Add(xelementAttributeInt2, Global.GetXElementAttributeStr(xelementList5[m], "Route"));
				}
			}
		}
		this.m_TreasureNormolNum = (int)ConfigSystemParam.GetSystemParamIntByName("TreasureFreeNum");
		this.m_TreasurePrice = (int)ConfigSystemParam.GetSystemParamIntByName("TreasurePrice");
	}

	private void InitBGTexture()
	{
		if (null != this.m_BGTexture)
		{
			this.m_BGTexture.URL = "NetImages/GameRes/Images/CangBaoMiJing/CangBaoMiJingBg.jpg";
		}
	}

	private void InitRoleData()
	{
		this.UpdateReloData(-1, 0);
	}

	private Vector3 GetFloorPos(int x, int y, bool b_Z = false, bool refreshRolePos = false)
	{
		float x2 = -420f + 58.5f * (float)x;
		float num = -245f + 28.5f * (float)y;
		float num2 = this.m_LastPos.z;
		if (this.m_LastPos != Vector3.zero)
		{
			if (this.m_LastPos.y < num)
			{
				num2 += 1f;
			}
			else
			{
				num2 -= 1f;
			}
		}
		else
		{
			num2 = -19f;
		}
		this.m_LastPos.x = x2;
		this.m_LastPos.y = num;
		this.m_LastPos.z = num2;
		if (refreshRolePos)
		{
			Vector3 localPosition = this.m_Rolehead.transform.localPosition;
			localPosition.z = ((localPosition.z > num2) ? localPosition.z : (num2 - 10f));
			this.m_Rolehead.transform.localPosition = localPosition;
			for (int i = 0; i < this.m_GameHintF.Length; i++)
			{
				localPosition = this.m_GameHintF[i].transform.localPosition;
				localPosition.z = this.m_Rolehead.transform.localPosition.z - 29f;
				this.m_GameHintF[i].transform.localPosition = localPosition;
			}
		}
		return this.m_LastPos;
	}

	private GChildWindow CreateGCWin(int z = -100)
	{
		if (null != this.m_MessageWin)
		{
			for (int i = 0; i < this.m_MessageWin.Body.transform.childCount; i++)
			{
				GameObject gameObject = this.m_MessageWin.Body.transform.GetChild(i).gameObject;
				if (null != gameObject)
				{
					NGUITools.Destroy(gameObject);
				}
			}
			NGUITools.Destroy(this.m_MessageWin.gameObject);
		}
		this.m_MessageWin = U3DUtils.NEW<GChildWindow>();
		this.m_MessageWin.Modal = false;
		this.m_MessageWin.Visibility = true;
		this.m_MessageWin.transform.localPosition = new Vector3(0f, 0f, (float)z);
		this.m_MessageWin.transform.SetParent(base.transform, false);
		return this.m_MessageWin;
	}

	private void FloorHander(object sender, DPSelectedItemEventArgs args)
	{
		if (this.FloorArray.LongLength > (long)args.MyID)
		{
			this.ShowMessage(this.FloorArray[args.MyID]._CangbaoData.Tips, this.FloorArray[args.MyID]._CangbaoData.Intro);
		}
	}

	private void RefeshHintLanelContent(CangBaoMiJingPart.HintType type, int NewValue, int OldValue = 0, bool bFrist = false)
	{
		string text = string.Empty;
		if (type == CangBaoMiJingPart.HintType.Hint_MIRACLE)
		{
			int num = (OldValue != 0) ? OldValue : this.m_TreasureMiracleNum;
			this.m_TreasureMiracleNum = NewValue;
			if (!bFrist && 0 < NewValue - num)
			{
				text = string.Format(Global.GetLang("奇迹骰子数增加{0}个"), NewValue - num);
			}
			this.m_MiracleHint_Label[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format("{0}", NewValue)
			});
		}
		else if (type == CangBaoMiJingPart.HintType.Hint_Normal)
		{
			int num = (OldValue != 0) ? OldValue : this.m_TreasureNormolNum;
			this.m_TreasureNormolNum = NewValue;
			if (!bFrist && 0 < NewValue - num)
			{
				text = string.Format(Global.GetLang("普通骰子数增加{0}个"), NewValue - num);
			}
			this.m_NormolHint_Label[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"edddbf",
				string.Format("{0}", NewValue)
			});
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.HintMainText(text, 10, 3);
		}
	}

	private TimeSpan AutoConvert(long Time__)
	{
		DateTime correctDateTime = TimeManager.GetCorrectDateTime();
		DateTime dateTime = correctDateTime.AddSeconds((double)Time__);
		return dateTime - correctDateTime;
	}

	private void RoleMoveDoneOneFloor(bool bFrist = true)
	{
		if (0 < Math.Abs(this.m_MoveNum))
		{
			this.m_RoleHeadIsMoving = true;
			int num = (0 >= this.m_MoveNum) ? (this.m_Rolehead.RoleOnFloorID - 1) : (this.m_Rolehead.RoleOnFloorID + 1);
			if (num < this.FloorArray.Length && 0 <= num)
			{
				Vector3 floorPos = this.GetFloorPos((int)this.m_MapPos[num].x, (int)this.m_MapPos[num].y, false, false);
				floorPos.z = this.FloorArray[num].FloorPos.z - 9.5f;
				this.m_Rolehead.RefreshHeadPos(floorPos, false, num);
				this.m_LastPos = Vector2.zero;
			}
			else
			{
				if (this.CanShowEvent())
				{
					this.ShowShiJianPart(this.m_NotDoneEventList[0], this.m_Rolehead.RoleOnFloorID);
				}
				this.m_MoveNum = 0;
			}
		}
		else
		{
			if (this.CanShowEvent())
			{
				this.ShowShiJianPart(this.m_NotDoneEventList[0], this.m_Rolehead.RoleOnFloorID);
			}
			else
			{
				this.m_MoveNum = 0;
				this.SendOnePieceMove(6);
			}
			base.StartCoroutine<bool>(this.ChangeValue_Time(new Action<bool, int>(this.ChangeValue), false, 0));
		}
		if (!bFrist)
		{
			this.m_MoveNum = ((0 >= this.m_MoveNum) ? ((this.m_MoveNum != 0) ? (++this.m_MoveNum) : 0) : (--this.m_MoveNum));
		}
	}

	private void ChangeValue(bool Value, int itemindex)
	{
		switch (itemindex)
		{
		case 0:
			this.m_RoleHeadIsMoving = Value;
			break;
		case 1:
			this.m_NormolBtnCanClick = Value;
			break;
		case 2:
			this.m_MiracleBtnCCanClick = Value;
			break;
		}
	}

	private IEnumerator ChangeValue_Time(Action<bool, int> an, bool value, int itemindex)
	{
		yield return new WaitForSeconds(1.2f);
		an.Invoke(value, itemindex);
		yield break;
	}

	private void GetDataFromNet()
	{
		GameInstance.Game.GetOnePieceInfo();
		Super.ShowNetWaiting(null);
	}

	private void AddEventToArray(OnePieceTreasureEvent eventdata)
	{
		this.m_NotDoneEventList.Add(eventdata);
		this.SwitchEventBtn();
	}

	private bool CanShowEvent()
	{
		return 0 < this.m_NotDoneEventList.Count;
	}

	private bool EventWin(CangBaoMiJingPart.EventWithUIRelation relation)
	{
		if (relation == CangBaoMiJingPart.EventWithUIRelation.IsNotOnUI)
		{
			return 0 >= this.m_EventIsOnUI;
		}
		if (relation == CangBaoMiJingPart.EventWithUIRelation.CanShowOnUI)
		{
			return 1 > this.m_EventIsOnUI;
		}
		return relation == CangBaoMiJingPart.EventWithUIRelation.HaveEvent && 0 < this.m_NotDoneEventList.Count;
	}

	private void SwitchEventBtn()
	{
		if (this.EventWin(CangBaoMiJingPart.EventWithUIRelation.IsNotOnUI) && this.EventWin(CangBaoMiJingPart.EventWithUIRelation.HaveEvent))
		{
			NGUITools.SetActive(this.m_Thingsbtn, true);
		}
		else
		{
			NGUITools.SetActive(this.m_Thingsbtn, false);
		}
	}

	private Vector2[] GetMapPos(int index)
	{
		string empty = string.Empty;
		if (this.m_MapData.TryGetValue(index, ref empty))
		{
			string[] array = empty.Split(new char[]
			{
				'|'
			});
			int num = array.Length;
			if (string.IsNullOrEmpty(array[array.Length - 1]))
			{
				num--;
			}
			Vector2[] array2 = new Vector2[num];
			for (int i = 0; i < num; i++)
			{
				string[] array3 = array[i].Split(new char[]
				{
					','
				});
				array2[i].x = (float)Convert.ToInt32(array3[0]);
				array2[i].y = (float)Convert.ToInt32(array3[1]);
			}
			return array2;
		}
		return null;
	}

	private IEnumerator LoadMap_CB(Vector2[] MapPos, CangBaoMiJing_FloorItem[] floorArray, CangBaoData_Map map_data, int TeXiao_FloorId)
	{
		for (int i = 0; i < MapPos.Length; i++)
		{
			CangBaoMiJing_FloorItem item = U3DUtils.NEW<CangBaoMiJing_FloorItem>();
			item.SetParent(this.m_MapRoot, false);
			item.SetPos(this.GetFloorPos((int)MapPos[i].x, (int)MapPos[i].y, true, false));
			Data_CangBao_Map data = null;
			if (map_data.DataCangBao_Map.TryGetValue(i, ref data))
			{
				item._CangbaoData = data;
			}
			item.SelectState = (i == 0);
			item.EventFloor = (CangBaoMiJing_FloorItem.Event_CangBao)i;
			item.Btnhandler = new DPSelectedItemEventHandler(this.FloorHander);
			item.RefreshFlooar();
			item.MyTag = i;
			item.name = "Floor_" + i.ToString();
			this.FloorArray[i] = item;
			item.SelectState = (i == TeXiao_FloorId);
			if (i == 0)
			{
				this.RefreshRolePos(0);
			}
			if (i % 2 == 0)
			{
				yield return null;
			}
			if (i == MapPos.Length - 1)
			{
				if (!this.m_Rolehead.gameObject.activeSelf)
				{
					NGUITools.SetActiveSelf(this.m_Rolehead.gameObject, true);
				}
				this.m_Rolehead.RefreshHeadPos(this.GetFloorPos((int)this.m_MapPos[this.m_Rolehead.RoleOnFloorID].x, (int)this.m_MapPos[this.m_Rolehead.RoleOnFloorID].y, false, false), true, 0);
				if (this.m_Rolehead.RoleOnFloorID != TeXiao_FloorId)
				{
					this.m_MoveNum = TeXiao_FloorId - this.m_Rolehead.RoleOnFloorID;
					this.RoleMoveDoneOneFloor(false);
				}
				this.FloorArray[TeXiao_FloorId].SelectDepth = (int)(this.m_Rolehead.transform.localPosition.z - 10f);
				base.StopCoroutine("LoadMap_CB");
			}
		}
		yield break;
	}

	protected void TickProc()
	{
		string text = string.Empty;
		this.m_ChongZhiShengYuShiJian -= 1L;
		if (this.m_ChongZhiShengYuShiJian > 0L)
		{
			TimeSpan timeSpan = this.AutoConvert(this.m_ChongZhiShengYuShiJian);
			if (0 >= timeSpan.Days && 0 >= timeSpan.Hours && 0 >= timeSpan.Minutes)
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("重置倒计时：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"3eb431",
					Global.GetLang("小于一分钟")
				});
			}
			else
			{
				text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("重置倒计时：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("{0}天{1}小时{2}分"), timeSpan.Days, timeSpan.Hours, timeSpan.Minutes)
				});
			}
		}
		else
		{
			text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("重置倒计时：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"3eb431",
				Global.GetLang("小于一分钟")
			});
			base.CancelInvoke("TickProc");
		}
		this.m_ChongzhiTimeLabel.text = text;
	}

	private GameObject LoadRollTeXiaoObj(string Path)
	{
		Object @object = Resources.Load(Path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(base.transform, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			return gameObject;
		}
		return null;
	}

	private void ShowHintPart(CangBaoMiJingHintPart.HintType type, int roleHaveNum = 0)
	{
		if (type == CangBaoMiJingHintPart.HintType.Miracle && 0 >= this.m_TreasureMiracleNum)
		{
			Super.HintMainText(Global.GetLang("奇迹骰子数不足"), 10, 3);
			return;
		}
		GChildWindow m_MessageWin_ = this.CreateGCWin(-100);
		CangBaoMiJingHintPart m_Message_ = U3DUtils.NEW<CangBaoMiJingHintPart>();
		m_Message_.transform.SetParent(m_MessageWin_.Body.transform, false);
		m_Message_.RefreshContent(type, roleHaveNum);
		m_Message_.handler = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.ID != 0)
			{
				if (s.ID == 3)
				{
					this.SendOnePieceROLL_MIRACLE(s.MyID);
				}
			}
			if (null != m_Message_)
			{
				NGUITools.Destroy(m_Message_.gameObject);
			}
			if (null != m_MessageWin_)
			{
				NGUITools.Destroy(m_MessageWin_.gameObject);
			}
			m_Message_ = null;
			m_MessageWin_ = null;
		};
	}

	private void ResetPos(string data)
	{
		for (int i = 0; i < 3; i++)
		{
			base.StartCoroutine<bool>(this.ChangeValue_Time(new Action<bool, int>(this.ChangeValue), i != 0, i));
		}
	}

	public void ChangeDay()
	{
		GameInstance.Game.GetOnePieceInfo();
	}

	public void RefreshMap(int Custom = 1, int TeXiao_FloorId = -1)
	{
		if (null != this.m_CengShulabel)
		{
			this.m_CengShulabel.text = Custom.ToString();
		}
		this.ClearFloor();
		this.m_MapPos = this.GetMapPos(Custom);
		CangBaoData_Map cangBaoData_Map = null;
		if (!this.m_xmlCangBao_MapData.TryGetValue(Custom, ref cangBaoData_Map))
		{
			return;
		}
		if (cangBaoData_Map.DataCangBao_Map.Count != this.m_MapPos.Length)
		{
			return;
		}
		base.StopCoroutine("LoadMap_CB");
		if (this.m_Rolehead.gameObject.activeSelf)
		{
			NGUITools.SetActiveSelf(this.m_Rolehead.gameObject, false);
		}
		this.FloorArray = new CangBaoMiJing_FloorItem[this.m_MapPos.Length];
		this.m_LastPos = Vector3.zero;
		base.StartCoroutine<bool>(this.LoadMap_CB(this.m_MapPos, this.FloorArray, cangBaoData_Map, TeXiao_FloorId));
	}

	private void ClearFloor()
	{
		if (this.FloorArray != null)
		{
			for (int i = 0; i < this.FloorArray.Length; i++)
			{
				if (null != this.FloorArray[i])
				{
					NGUITools.Destroy(this.FloorArray[i].gameObject);
				}
			}
		}
	}

	public void UpdateReloData(int index = -1, int result = 0)
	{
		if (index == -1)
		{
			if (null != this.m_RoleDataLabel[0])
			{
				this.m_RoleDataLabel[0].text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TreasureJiFen).ToString();
			}
			if (null != this.m_RoleDataLabel[1])
			{
				this.m_RoleDataLabel[1].text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TreasureXueZuan).ToString();
			}
			if (null != this.m_RoleDataLabel[2])
			{
				this.m_RoleDataLabel[2].text = Global.Data.roleData.UserMoney.ToString();
			}
		}
		else
		{
			int num;
			if (index == 0)
			{
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TreasureJiFen);
			}
			else if (index == 1)
			{
				num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TreasureXueZuan);
			}
			else
			{
				num = Global.Data.roleData.UserMoney;
			}
			if (index < this.m_RoleDataLabel.Length && null != this.m_RoleDataLabel[index])
			{
				this.m_RoleDataLabel[index].text = num.ToString();
			}
		}
		if (null != this.m_MUDuiHuanPart)
		{
			this.m_MUDuiHuanPart.UpdateMojingZhi(result);
		}
	}

	public void RoleMove(int FloorLong)
	{
		if (this.EventWin(CangBaoMiJingPart.EventWithUIRelation.IsNotOnUI))
		{
			int comtusNum = this.m_ComtusNum;
			this.m_ComtusNum = FloorLong / 1000;
			this.m_RoleMoveNum = FloorLong % 1000;
			this.m_MoveNum = this.m_RoleMoveNum;
			this.m_MoveNum -= ((this.m_Rolehead.RoleOnFloorID != 30) ? this.m_Rolehead.RoleOnFloorID : 0);
			this.m_bRefreshmap = (comtusNum != this.m_ComtusNum);
			this.m_MoveNum = ((!this.m_bRefreshmap) ? this.m_MoveNum : this.m_RoleMoveNum);
			if (!this.m_RollAnimationIsPlaying)
			{
				if (this.m_bRefreshmap)
				{
					this.RefreshMap(this.m_ComtusNum, this.m_RoleMoveNum);
				}
				else if (0 < this.m_MoveNum)
				{
					this.RoleMoveDoneOneFloor(false);
				}
			}
		}
	}

	public void RemoveEventAtList(int index = 0)
	{
		if (index < this.m_NotDoneEventList.Count)
		{
			this.m_NotDoneEventList.RemoveAt(index);
		}
		if (0 < this.m_EventIsOnUI)
		{
			this.m_EventIsOnUI--;
		}
		this.SwitchEventBtn();
	}

	public void DuiHuanErrorCodeMessage(int index)
	{
		string text = string.Empty;
		if (index != -14)
		{
			if (index == -13)
			{
				text = Global.GetLang("藏宝积分不够");
			}
		}
		else
		{
			text = Global.GetLang("藏宝血钻不够");
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.HintMainText(text, 10, 3);
		}
	}

	public void ErrorCodeMessage(int index, string strData = "")
	{
		string text = string.Empty;
		switch (index)
		{
		case 1:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			break;
		case 2:
			text = Global.GetLang("背包空间不够");
			break;
		case 3:
			text = Global.GetLang("传来的参数错误");
			break;
		case 4:
			text = Global.GetLang("数据库出错");
			break;
		case 5:
			text = Global.GetLang("正在移动中，请稍后");
			break;
		case 6:
			text = Global.GetLang("没有待执行的事件");
			break;
		case 7:
			text = Global.GetLang("兑换物品配置错误");
			break;
		case 8:
			text = Global.GetLang("兑换物品配置错误");
			break;
		case 9:
			text = Global.GetLang("兑换背包物品不足");
			break;
		case 10:
			text = Global.GetLang("兑换所需货币不足");
			break;
		case 11:
			text = Global.GetLang("随机移动事件配置错误");
			break;
		case 12:
			if (this.m_m_MiracleCleck)
			{
				this.m_m_MiracleCleck = false;
			}
			break;
		case 13:
			this.ResetPos(strData);
			break;
		case 14:
			text = Global.GetLang("骰子数达到上限");
			break;
		case 15:
			text = Global.GetLang("骰子数不足");
			break;
		case 16:
			text = Global.GetLang("背包已满，奖励已发送邮件");
			break;
		}
		if (!string.IsNullOrEmpty(text))
		{
			Super.HintMainText(text, 10, 3);
		}
	}

	public void RefreshInF(OnePieceTreasureData data)
	{
		if (data != null)
		{
			this.m_ComtusNum = data.PosID / 1000;
			this.m_RoleMoveNum = data.PosID % 1000;
			this.RefeshHintLanelContent(CangBaoMiJingPart.HintType.Hint_Normal, data.RollNumNormal, 0, true);
			this.RefeshHintLanelContent(CangBaoMiJingPart.HintType.Hint_MIRACLE, data.RollNumMiracle, 0, true);
			if (data.EventID != 0)
			{
				this.AddEventToArray(new OnePieceTreasureEvent
				{
					EventID = data.EventID
				});
			}
			this.RefreshMap(this.m_ComtusNum, this.m_RoleMoveNum);
			this.RefreshRolePos(this.m_RoleMoveNum);
			this.m_ChongZhiShengYuShiJian = data.ResetPosTicks / 10000000L;
			this.TickProc();
			base.CancelInvoke("TickProc");
			base.InvokeRepeating("TickProc", 1f, 1f);
		}
	}

	public void RefreshRolePos(int FloorIndex)
	{
		if (null != this.m_Rolehead)
		{
			this.m_Rolehead.RoleOnFloorID = FloorIndex;
		}
	}

	public void RefreshROLL(int num, bool BNormol = true)
	{
		if (this.m_RollData != null)
		{
			this.m_RollData.Desrory();
			this.m_RollData = null;
		}
		this.m_RollData = new CangBaoMiJingPart.RollData();
		this.m_RollAnimationIsPlaying = true;
		this.m_RollData.SetInF(num, this.m_Rolehead.RoleOnFloorID, (!BNormol) ? null : this.LoadRollTeXiaoObj(this.Path_TeXiao + num.ToString()));
		this.m_RollData.Hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (this.m_bRefreshmap)
			{
				this.RefreshMap(this.m_ComtusNum, -1);
			}
			else
			{
				for (int i = 0; i < this.FloorArray.Length; i++)
				{
					if (i == this.m_RollData.TeXiaoOnFloorID)
					{
						this.FloorArray[i].SelectState = true;
						this.FloorArray[i].SelectDepth = (int)(this.m_Rolehead.transform.localPosition.z - 10f);
					}
					else
					{
						this.FloorArray[i].SelectState = false;
					}
				}
				this.RoleMoveDoneOneFloor(false);
			}
			this.m_RollAnimationIsPlaying = false;
			this.m_RollData = null;
		};
		this.RemoveEventAtList(0);
	}

	public void RefreshEvent_Net(OnePieceTreasureEvent data)
	{
		Data_CangBao_Event data_CangBao_Event = null;
		if (this.m_xmlCangBao_EventData.TryGetValue(data.EventID, ref data_CangBao_Event) && (data_CangBao_Event.Type == 2 || data_CangBao_Event.Type == 4 || data_CangBao_Event.Type == 5))
		{
			this.AddEventToArray(data);
			this.ShowShiJianPart(this.m_NotDoneEventList[0], this.m_Rolehead.RoleOnFloorID);
		}
		if (data.ErrCode != 0 && data.ErrCode == 16)
		{
			this.ErrorCodeMessage(data.ErrCode, string.Empty);
		}
	}

	public void RefreshRollNumber(DiceType type, int newnumber, int oldnumber)
	{
		if (type == DiceType.EDT_MIRACLE)
		{
			this.RefeshHintLanelContent(CangBaoMiJingPart.HintType.Hint_MIRACLE, newnumber, oldnumber, false);
		}
		else if (type == DiceType.EDT_Normal)
		{
			this.RefeshHintLanelContent(CangBaoMiJingPart.HintType.Hint_Normal, newnumber, oldnumber, false);
		}
	}

	public int TreasureNormolNum
	{
		get
		{
			return this.m_TreasureNormolNum;
		}
		set
		{
			this.m_TreasureNormolNum = value;
			this.RefeshHintLanelContent(CangBaoMiJingPart.HintType.Hint_Normal, this.m_TreasureNormolNum, 0, false);
		}
	}

	public int TreasureMiracleNum
	{
		get
		{
			return this.m_TreasureMiracleNum;
		}
		set
		{
			this.m_TreasureMiracleNum = value;
			this.RefeshHintLanelContent(CangBaoMiJingPart.HintType.Hint_MIRACLE, this.m_TreasureMiracleNum, 0, false);
		}
	}

	public ShowNetImage m_BGTexture;

	public ShowNetImage m_TopBgImage;

	public GButton m_CloseBtn;

	public GButton m_ShopBtn;

	public GButton m_helpBtn;

	public GButton m_Thingsbtn;

	public Transform m_MapRoot;

	public UILabel[] m_RoleDataLabel = new UILabel[3];

	public UILabel m_CengShulabel;

	public UILabel[] m_MiracleHint_Label = new UILabel[2];

	public GButton m_MiracleHint_Btn;

	public UILabel[] m_NormolHint_Label = new UILabel[2];

	public GButton m_Normolint_Btn;

	public GButton m_NormolBtn;

	public GButton m_MiracleBtn;

	public UILabel m_ChongzhiTimeLabel;

	public Transform m_RoleHeadroot;

	public GameObject[] m_GameHintF = new GameObject[3];

	private CangBaoMiJing_FloorItem[] FloorArray;

	private XElement xml_TreasureBox;

	private XElement xml_TreasureEvent;

	private XElement xml_TreasureMap;

	private XElement xml_TreasureRoute;

	private Dictionary<int, CangBaoData_Map> m_xmlCangBao_MapData = new Dictionary<int, CangBaoData_Map>();

	private Dictionary<int, Data_CangBao_Event> m_xmlCangBao_EventData = new Dictionary<int, Data_CangBao_Event>();

	private Dictionary<int, CangBaoData_Box> m_xmlCangBao_BoxData = new Dictionary<int, CangBaoData_Box>();

	private GChildWindow m_MessageWin;

	private CangBaoMiJingRoleHeadPart m_Rolehead;

	private long m_ChongZhiShengYuShiJian;

	private int m_MoveNum;

	private List<OnePieceTreasureEvent> m_NotDoneEventList = new List<OnePieceTreasureEvent>();

	private int m_ComtusNum;

	private bool m_bRefreshmap;

	private int m_RoleMoveNum;

	private int m_TreasureNormolNum;

	private int m_TreasureMiracleNum;

	private int m_TreasurePrice;

	private static int MAXFLOORNUM = 30;

	private bool m_RoleHeadIsMoving;

	private int m_EventIsOnUI;

	private bool m_RollAnimationIsPlaying;

	private bool m_MiracleBtnCCanClick = true;

	private bool m_NormolBtnCanClick = true;

	private MUDuiHuanPart m_MUDuiHuanPart;

	private Dictionary<int, string> m_MapData = new Dictionary<int, string>();

	private bool m_m_MiracleCleck;

	private Vector2[] m_MapPos;

	private CangBaoMiJingPart.RollData m_RollData;

	private string Path_TeXiao = "UITeXiao/Perfabs/shaizi/shaizi_0";

	private Vector3 m_LastPos = Vector2.zero;

	public DPSelectedItemEventHandler Closehandler;

	private enum RoleDataLabelIndex
	{
		TreasureJiFen,
		TreasureXueZuan,
		Diamond
	}

	private enum EventWithUIRelation
	{
		IsNotOnUI,
		CanShowOnUI,
		HaveEvent
	}

	private enum HintType
	{
		Hint_Normal,
		Hint_MIRACLE
	}

	private enum CangBaoConvertType
	{
		day,
		hour,
		minute,
		second
	}

	public enum OnePieceTreasureErrorCode
	{
		OnePiece_Success,
		OnePiece_ErrorZuanShiNotEnough,
		OnePiece_ErrorBagNotEnough,
		OnePiece_ErrorParams,
		OnePiece_DBFailed,
		OnePiece_ErrorMoving,
		OnePiece_ErrorNotHaveEvent,
		OnePiece_ErrorNeedGoodsID,
		OnePiece_ErrorNeedGoodsCount,
		OnePiece_ErrorGoodsNotEnough,
		OnePiece_ErrorNeedMoneyNotEnough,
		OnePiece_ErrorMoveRange,
		OnePiece_ErrorMoveNumNotEnough,
		OnePiece_ResetPos,
		OnePiece_ErrorRollNumMax,
		OnePiece_ErrorRollNumNotEnough,
		OnePiece_ErrorCheckMail
	}

	private class RollData
	{
		public int DiceNumber
		{
			get
			{
				return this.m_DiceNumber;
			}
		}

		public int RoleOnFloorID
		{
			get
			{
				return this.m_RoleOnFloorID;
			}
		}

		public void SetInF(int DicNum, int OnFloorID, GameObject TeXiaoObj)
		{
			this.m_RoleOnFloorID = OnFloorID;
			this.m_DiceNumber = DicNum;
			this.m_RollTeXiao = TeXiaoObj;
			this.m_TeXiaoLife = 1f;
			if (null != TeXiaoObj)
			{
				this.m_Animation = this.m_RollTeXiao.GetComponentInChildren<Animation>();
			}
			else
			{
				this.m_Animation = null;
			}
			this.m_TeXiaoOnFloorID = ((30 >= this.m_RoleOnFloorID + this.m_DiceNumber) ? (this.m_RoleOnFloorID + this.m_DiceNumber) : 30);
		}

		public void Update(float dTime)
		{
			if (null != this.m_Animation)
			{
				if (!this.m_Animation.isPlaying)
				{
					this.m_TeXiaoLife -= dTime;
					if (this.m_TeXiaoLife <= 0f)
					{
						this.Desrory();
						this.Hander(null, new DPSelectedItemEventArgs());
					}
				}
			}
			else
			{
				this.Hander(null, new DPSelectedItemEventArgs());
			}
		}

		public void Desrory()
		{
			if (null != this.m_Animation)
			{
				this.m_Animation.Stop();
				NGUITools.Destroy(this.m_RollTeXiao);
				this.m_DiceNumber = 0;
				this.m_Animation = null;
				this.m_RollTeXiao = null;
			}
		}

		public GameObject RollTeXiao
		{
			get
			{
				return this.m_RollTeXiao;
			}
			set
			{
				this.m_RollTeXiao = value;
			}
		}

		public int TeXiaoOnFloorID
		{
			get
			{
				return this.m_TeXiaoOnFloorID;
			}
		}

		private int m_DiceNumber;

		private int m_RoleOnFloorID;

		private GameObject m_RollTeXiao;

		private int m_TeXiaoOnFloorID;

		private float m_TeXiaoLife = 0.4f;

		private Animation m_Animation;

		public DPSelectedItemEventHandler Hander;
	}
}
