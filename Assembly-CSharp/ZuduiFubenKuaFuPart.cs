using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public class ZuduiFubenKuaFuPart : UserControl
{
	protected override void OnDestroy()
	{
		base.OnDestroy();
		ZuduiFubenKuaFuPart.Instance = null;
	}

	private void InitTextInPrefabs()
	{
		this.btnCreate.Text = Global.GetLang("随机组队");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		GameInstance.Game.GetJieriFanbeiInfo();
		ZuduiFubenKuaFuPart.Instance = this;
		ZuduiFubenKuaFuPart.IsShowTime = true;
		ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan.Clear();
	}

	public void InitPartData(ActivityCategorys category)
	{
		this.InitFubenTabConf(category);
		this.FubenItemCollection = this.fubenListBox.ItemsSource;
		if (this.fubenTabDict.Count > 0)
		{
			this.InitFubenConf();
			this.InitFubenMapConf();
			this.InitFubenItem();
			this.CheckAllItem();
			int trigger = 0;
			int param1 = 0;
			int param2 = 0;
			GButton gbutton = this.btnCreate;
			gbutton.MouseLeftButtonUp = (MouseLeftButtonUpEventHandler)Delegate.Combine(gbutton.MouseLeftButtonUp, delegate(object s, MouseEvent e)
			{
				if (this.fubenListBox.SelectedIndex == 1)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MoriShenPan, ref trigger, ref param1, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.MoriShenPan, trigger, param1, param2, true);
					}
					else
					{
						TCPGameServerCmds.CMD_SPR_MORI_JOIN.SendDataUseRoleID();
					}
				}
				else if (this.fubenListBox.SelectedIndex == 2)
				{
					if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuansuShiLian, ref trigger, ref param1, ref param2))
					{
						UIHelper.HintGongNengOpenCondition(GongNengIDs.YuansuShiLian, trigger, param1, param2, true);
					}
					else
					{
						TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_JOIN.SendDataUseRoleID();
					}
				}
				this.StopAllCoroutines();
				this.StartCoroutine(this.XunZhaoDuiShou_DaoJiShi());
			});
			UIEventListener uieventListener = UIEventListener.Get(this.m_BtnQuXiao.gameObject);
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, delegate(GameObject g)
			{
				this.m_XunZhaoDuiShou.gameObject.SetActive(false);
				if (this.fubenListBox.SelectedIndex == 1)
				{
					TCPGameServerCmds.CMD_SPR_MORI_QUIT.SendDataUseRoleID();
				}
				else if (this.fubenListBox.SelectedIndex == 2)
				{
					TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_QUIT.SendDataUseRoleID();
				}
			});
		}
		else
		{
			string lang = Global.GetLang("暂无多人副本！");
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s1, DPSelectedItemEventArgs e1)
			{
			}, buttons);
		}
		this.InitListbox();
		this.fubenListBox.SelectedIndex = 1;
	}

	private IEnumerator XunZhaoDuiShou_DaoJiShi()
	{
		for (int i = 60; i > 0; i--)
		{
			this.m_LabeTime.text = i + string.Empty;
			yield return new WaitForSeconds(1f);
		}
		if (this.fubenListBox.SelectedIndex == 1)
		{
			TCPGameServerCmds.CMD_SPR_MORI_QUIT.SendDataUseRoleID();
		}
		else if (this.fubenListBox.SelectedIndex == 2)
		{
			TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_QUIT.SendDataUseRoleID();
		}
		this.m_XunZhaoDuiShou.gameObject.SetActive(false);
		Super.HintMainText(Global.GetLang("未匹配成功"), 10, 3);
		yield break;
	}

	public void Action_CMD_SPR_MORI_JOIN(MUSocketConnectEventArgs e)
	{
		int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
		if (num < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StdErrorCode.GetErrMsg(num, true, true), 0, -1, -1, 0);
			this.m_XunZhaoDuiShou.gameObject.SetActive(false);
			return;
		}
		this.m_XunZhaoDuiShou.gameObject.SetActive(true);
	}

	public void Action_CMD_MORI_NTF_ENTER(MUSocketConnectEventArgs e)
	{
		int GameId = ConvertExt.SafeConvertToInt32(e.fields[0]);
		if (GameId < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StdErrorCode.GetErrMsg(GameId, true, true), 0, -1, -1, 0);
			this.m_XunZhaoDuiShou.gameObject.SetActive(false);
			this.m_XunZhaoDuiShouJieGuo.gameObject.SetActive(false);
			this.m_XunZhaoDuiShouDengDai.gameObject.SetActive(false);
			return;
		}
		if (e.fields.Length > 1)
		{
			int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
			this.m_LabelShiFouJinRu.text = Global.GetLang("{E2B36B}平均战力:  {FF0000}" + num + "{-}");
		}
		this.m_XunZhaoDuiShou.gameObject.SetActive(false);
		this.m_XunZhaoDuiShouJieGuo.gameObject.SetActive(true);
		base.StopAllCoroutines();
		base.StartCoroutine(this.XunZhaoDuiShouJieGuo_DaoJiShi());
		UIEventListener.Get(this.m_BtnQuXiao1.gameObject).onClick = delegate(GameObject g)
		{
			Super.ShowMessageBoxExt(Global.GetLang("提示"), Global.GetLang("强制退出受到惩罚，10分钟无法匹配，你确定要退出吗？"), 0f, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					this.m_XunZhaoDuiShouJieGuo.gameObject.SetActive(false);
					if (this.fubenListBox.SelectedIndex == 1)
					{
						TCPGameServerCmds.CMD_MORI_NTF_ENTER.SendData(Global.Data.RoleID + ":-1");
					}
					else if (this.fubenListBox.SelectedIndex == 2)
					{
						TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_ENTER.SendData(Global.Data.RoleID + ":-1");
					}
				}
			}, new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			});
		};
		UIEventListener.Get(this.m_BtnJinRu.gameObject).onClick = delegate(GameObject g)
		{
			this.m_XunZhaoDuiShou.gameObject.SetActive(false);
			this.StopAllCoroutines();
			PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
			if (playZone != null)
			{
				playZone.CloseActivityWindow();
			}
			if (this.fubenListBox.SelectedIndex == 1)
			{
				TCPGameServerCmds.CMD_MORI_NTF_ENTER.SendData(Global.Data.RoleID + ":" + GameId);
			}
			else if (this.fubenListBox.SelectedIndex == 2)
			{
				TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_ENTER.SendData(Global.Data.RoleID + ":" + GameId);
			}
			this.m_XunZhaoDuiShouJieGuo.gameObject.SetActive(false);
			this.m_XunZhaoDuiShouDengDai.gameObject.SetActive(true);
			this.m_LabelXunDengDai.text = Global.GetLang("{E2B36B}请耐心等待其他队友....{-}");
		};
	}

	public void Action_CMD_NTF_MORI_COPY_CANCEL(MUSocketConnectEventArgs e)
	{
		int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
		this.m_XunZhaoDuiShou.gameObject.SetActive(false);
		this.m_XunZhaoDuiShouJieGuo.gameObject.SetActive(false);
		this.m_XunZhaoDuiShouDengDai.gameObject.SetActive(false);
		if (num < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StdErrorCode.GetErrMsg(num, true, true), 0, -1, -1, 0);
			return;
		}
		this.m_LabelXunDengDai.text = Global.GetLang("{E2B36B}有队友放弃副本,请自动重新匹配{-}");
		base.StartCoroutine(this.Action_CMD_NTF_MORI_COPY_CANCELIEnumerator());
	}

	public static void Action_CMD_SPR_ELEMENT_WAR_SCORE_INFO(MUSocketConnectEventArgs e)
	{
		ElementWarScoreData elementWarScoreData = DataHelper.BytesToObject<ElementWarScoreData>(e.bytesData, 0, e.bytesData.Length);
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		ZuduiFubenKuaFuPart.IsLoadTime = true;
		if (playZone != null && elementWarScoreData != null)
		{
			if (ZuduiFubenKuaFuPart.thisWave != elementWarScoreData.Wave)
			{
				playZone.GameTaskBoxMini.ShowYuansuEffect(elementWarScoreData.Wave - ZuduiFubenKuaFuPart.thisWave);
			}
			string text = playZone.GameTaskBoxMini.SceneInfos[0].text;
			playZone.GameTaskBoxMini.SetSceneTaskInfos(0, string.Concat(new object[]
			{
				Global.GetLang("完成挑战"),
				" ",
				elementWarScoreData.Wave,
				Global.GetLang("波")
			}), new object[0]);
			playZone.GameTaskBoxMini.SetSceneTaskInfos(2, string.Concat(new object[]
			{
				Global.GetLang("剩余怪物"),
				" ",
				elementWarScoreData.MonsterCount,
				Global.GetLang("个")
			}), new object[0]);
			ZuduiFubenKuaFuPart.EndTime = elementWarScoreData.EndTime;
			if (ZuduiFubenKuaFuPart.IsLoadTime)
			{
				playZone.StartCoroutine(ZuduiFubenKuaFuPart.ShowShengyuShijian(playZone));
				ZuduiFubenKuaFuPart.IsLoadTime = false;
			}
			ZuduiFubenKuaFuPart.thisWave = elementWarScoreData.Wave;
		}
	}

	private static IEnumerator ShowShengyuShijian(MonoBehaviour mo)
	{
		DateTime date = new DateTime(ZuduiFubenKuaFuPart.EndTime * 10000L);
		DateTime now = new DateTime(Global.GetCorrectLocalTime() * 10000L);
		if (ZuduiFubenKuaFuPart.EndTime > 0L && date > now)
		{
			PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
			int second = (date - now).Seconds;
			string str_Seconds = second.ToString();
			if (second > 20)
			{
				str_Seconds = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					str_Seconds
				});
			}
			else if (second >= 10 && second <= 20)
			{
				str_Seconds = Global.GetColorStringForNGUIText(new object[]
				{
					"ffa500",
					str_Seconds
				});
			}
			else
			{
				str_Seconds = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					str_Seconds
				});
			}
			playZone.GameTaskBoxMini.SetSceneTaskInfos(1, Global.GetLang("剩余时间") + " " + str_Seconds + Global.GetLang("秒"), new object[0]);
		}
		yield return new WaitForSeconds(1f);
		mo.StartCoroutine(ZuduiFubenKuaFuPart.ShowShengyuShijian(mo));
		yield break;
	}

	private IEnumerator Action_CMD_NTF_MORI_COPY_CANCELIEnumerator()
	{
		yield return new WaitForSeconds(2f);
		this.m_XunZhaoDuiShouDengDai.gameObject.SetActive(false);
		this.m_XunZhaoDuiShou.gameObject.SetActive(true);
		if (this.fubenListBox.SelectedIndex == 1)
		{
			TCPGameServerCmds.CMD_SPR_MORI_JOIN.SendDataUseRoleID();
		}
		else if (this.fubenListBox.SelectedIndex == 2)
		{
			TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_JOIN.SendDataUseRoleID();
		}
		base.StopAllCoroutines();
		base.StartCoroutine(this.XunZhaoDuiShou_DaoJiShi());
		yield break;
	}

	private IEnumerator XunZhaoDuiShouJieGuo_DaoJiShi()
	{
		for (int i = 15; i > 0; i--)
		{
			this.m_LabelDaoJiShi1.text = Global.GetLang("{E2B36B}倒计时{DC463C} ") + i + Global.GetLang(" {-} 秒{-}");
			yield return new WaitForSeconds(1f);
		}
		if (this.fubenListBox.SelectedIndex == 1)
		{
			TCPGameServerCmds.CMD_MORI_NTF_ENTER.SendData(Global.Data.RoleID + ":-1");
		}
		else if (this.fubenListBox.SelectedIndex == 2)
		{
			TCPGameServerCmds.CMD_SPR_ELEMENT_WAR_ENTER.SendData(Global.Data.RoleID + ":-1");
		}
		this.m_XunZhaoDuiShouJieGuo.gameObject.SetActive(false);
		yield break;
	}

	public static void ReInitMoRiShenPan()
	{
		ZuduiFubenKuaFuPart.IsShowTime = true;
		ZuduiFubenKuaFuPart.SetContentOfLbl(null, string.Empty);
	}

	public static void Action_CMD_NTF_MORI_MONSTER_EVENT(MUSocketConnectEventArgs e)
	{
		ZuduiFubenKuaFuPart.e_temp = e;
		int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
		if (num < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StdErrorCode.GetErrMsg(num, true, true), 0, -1, -1, 0);
			return;
		}
		int num2 = ConvertExt.SafeConvertToInt32(e.fields[1]);
		long num3 = ConvertExt.SafeConvertToInt64(e.fields[2]);
		long num4 = ConvertExt.SafeConvertToInt64(e.fields[3]);
		if (num == 1)
		{
			if (num2 == 1)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("瘟疫之源已经降临，带来了腐化与堕落"), 3);
				if (Context.IsHaiwai)
				{
					for (int i = 1; i <= 4; i++)
					{
						ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[i] = Global.GetLang("未激活");
					}
				}
			}
			else if (num2 == 2)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("战争之祸已经降临，带来了杀戮与毁灭"), 3);
			}
			else if (num2 == 3)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("饥饿之舌已经降临，带来了饥荒与偏袒"), 3);
			}
			else if (num2 == 4)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("死亡之罚已经降临，带来了苦难与终结"), 3);
			}
			else if (num2 == 5)
			{
				SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("最终的末日审判已经降临"), 3);
			}
		}
		else if (num == 2 && num2 >= 1 && num2 <= 4)
		{
			int num5 = Convert.ToInt32(Global.GetValueByAnyKeyInMoRiByID(num2, "Time"));
			if ((float)(num4 - num3) / 1000f <= (float)num5)
			{
				Global.MoRiShenPanOnTimeKillCount++;
				if (Global.MoRiShenPanOnTimeKillCount > 4)
				{
					Global.MoRiShenPanOnTimeKillCount = 4;
				}
			}
		}
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		string text = playZone.GameTaskBoxMini.SceneInfos[0].text;
		playZone.GameTaskBoxMini.SceneInfos[0].transform.localPosition = new Vector3(20.01926f, -43.17551f, 0f);
		Vector3 localScale = playZone.GameTaskBoxMini.Bak.transform.localScale;
		localScale.x = 204f;
		playZone.GameTaskBoxMini.Bak.transform.localScale = localScale;
		playZone.GameTaskBoxMini.SetSceneTaskInfos(0, ZuduiFubenKuaFuPart.SetContentOfLbl(e, text), new object[0]);
		if (ZuduiFubenKuaFuPart.IsShowTime)
		{
			ZuduiFubenKuaFuPart.IsShowTime = false;
			playZone.StartCoroutine(ZuduiFubenKuaFuPart.ShowTime());
			Global.Data.GameScene.AddSpriteLeadDeco(Global.Data.GameScene.GetLeader());
		}
	}

	private static IEnumerator ShowTime()
	{
		PlayZone playZone = Super.GData.GlobalPlayZone as PlayZone;
		string contentLbl = playZone.GameTaskBoxMini.SceneInfos[0].text;
		playZone.GameTaskBoxMini.SetSceneTaskInfos(0, ZuduiFubenKuaFuPart.SetContentOfLbl(ZuduiFubenKuaFuPart.e_temp, contentLbl), new object[0]);
		yield return new WaitForSeconds(1f);
		playZone.StartCoroutine(ZuduiFubenKuaFuPart.ShowTime());
		yield break;
	}

	public static string SetContentOfLbl(MUSocketConnectEventArgs e, string Content)
	{
		if (ZuduiFubenKuaFuPart.XElementMoRiShenPan == null)
		{
			ZuduiFubenKuaFuPart.XElementMoRiShenPan = Global.GetGameResXml("Config/MoRiShenPan.xml");
		}
		if (e == null)
		{
			List<XElement> list = Enumerable.ToList<XElement>(ZuduiFubenKuaFuPart.XElementMoRiShenPan.Elements("MoRiShenPan")).FindAll((XElement s) => s.AttributeInt("ID") != 5);
			for (int j = 1; j <= list.Count; j++)
			{
				if (!ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan.ContainsKey(j))
				{
					ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan.Add(j, Global.GetLang("未激活"));
				}
				else
				{
					ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[j] = Global.GetLang("未激活");
				}
			}
		}
		else
		{
			int num = ConvertExt.SafeConvertToInt32(e.fields[0]);
			int Id = ConvertExt.SafeConvertToInt32(e.fields[1]);
			if (Id == 5)
			{
				return Content;
			}
			long num2 = long.Parse(e.fields[2]);
			long num3 = long.Parse(e.fields[3]);
			XElement xelement = Enumerable.ToList<XElement>(ZuduiFubenKuaFuPart.XElementMoRiShenPan.Elements("MoRiShenPan")).Find((XElement s) => s.AttributeInt("ID") == Id && s.AttributeInt("ID") != 5);
			string text = string.Empty;
			if (num == 1)
			{
				int num4 = xelement.AttributeInt("Time");
				DateTime dateTime;
				dateTime..ctor(num2 * 10000L);
				DateTime dateTime2;
				dateTime2..ctor(Global.GetCorrectLocalTime() * 10000L);
				int num5 = (int)(dateTime2 - dateTime).TotalSeconds;
				num5 = num4 - num5;
				if (num4 == -1)
				{
					text = Global.GetLang("出生");
				}
				else if (num5 > 0)
				{
					text = Global.GetLang("剩余") + num5 + Global.GetLang("秒");
					Global.SetShuijingState(Id, 2);
				}
				else
				{
					if (num4 != -1 && !ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[Id].Contains(Global.GetLang("失败")))
					{
						SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("限时击杀失败"), 3);
					}
					text = Global.GetLang("{F70101}失败{-}");
					Global.SetShuijingState(Id, 1);
				}
			}
			if (num == 2)
			{
				int num6 = xelement.AttributeInt("Time");
				DateTime dateTime3;
				dateTime3..ctor(num2 * 10000L);
				DateTime dateTime4;
				dateTime4..ctor(num3 * 10000L);
				int num7 = (int)(dateTime4 - dateTime3).TotalSeconds;
				num7 = num6 - num7;
				if (num7 > 0)
				{
					if (num6 != -1 && !ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[Id].Contains(Global.GetLang("成功")))
					{
						SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("完成限时击杀，末日审判被强化，获得奖励提升"), 3);
					}
					text = Global.GetLang("{02E902}成功{-}");
					Global.SetShuijingState(Id, 3);
				}
				else if (num6 != -1)
				{
					if (num6 != -1 && !ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[Id].Contains(Global.GetLang("失败")))
					{
						SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("限时击杀失败"), 3);
					}
					text = Global.GetLang("{F70101}失败{-}");
					Global.SetShuijingState(Id, 1);
				}
				else
				{
					text = Global.GetLang("{02E902}成功{-}");
					if (num6 != -1 && !ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[Id].Contains(Global.GetLang("成功")))
					{
						SystemHelpPart.ShowHintTextNoTarget(true, Global.GetLang("完成击杀，末日审判被强化，获得奖励提升"), 3);
					}
					Global.SetShuijingState(Id, 3);
				}
			}
			ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[Id] = text;
		}
		string text2 = string.Empty;
		float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("MoRiShenPanAward", ',');
		int num8 = Enumerable.Count<KeyValuePair<int, string>>(Enumerable.ToList<KeyValuePair<int, string>>(ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan).FindAll((KeyValuePair<int, string> s) => s.Value.Contains(Global.GetLang("成功")))) - 1;
		if (num8 >= 0 && num8 < systemParamFloatArrayByName.Length)
		{
			string text3 = text2;
			text2 = string.Concat(new object[]
			{
				text3,
				Global.GetLang("奖励加成"),
				" ",
				systemParamFloatArrayByName[num8] * 100f,
				"%",
				Environment.NewLine
			});
		}
		else
		{
			text2 = text2 + Global.GetLang("奖励加成") + " 100%" + Environment.NewLine;
		}
		int i;
		for (i = 1; i <= ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan.Count; i++)
		{
			string text3 = text2;
			text2 = string.Concat(new string[]
			{
				text3,
				Enumerable.ToList<XElement>(ZuduiFubenKuaFuPart.XElementMoRiShenPan.Elements("MoRiShenPan")).Find((XElement s) => s.AttributeInt("ID") == i).AttributeStr("Name"),
				" ",
				ZuduiFubenKuaFuPart.IDValueOfMoRiShenPan[i],
				Environment.NewLine
			});
		}
		return "{FFC90E}" + text2 + "{-}";
	}

	private void InitListbox()
	{
		this.fubenListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.FuBenSelectedChange);
	}

	private void InitBtns()
	{
	}

	private void InitFubenTabConf(ActivityCategorys category)
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBenTab.Xml");
		if (gameResXml == null)
		{
			return;
		}
		ZuduiFubenKuaFuPart.FubenTabConf fubenTabConf = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "FuBenType");
			if (xelementAttributeInt2 == (int)category)
			{
				fubenTabConf = null;
				if (!this.fubenTabDict.TryGetValue(xelementAttributeInt, ref fubenTabConf))
				{
					fubenTabConf = new ZuduiFubenKuaFuPart.FubenTabConf();
					this.fubenTabDict.Add(xelementAttributeInt, fubenTabConf);
				}
				fubenTabConf.name = Global.GetXElementAttributeStr(xelement, "Name");
				fubenTabConf.Preview = Global.GetXElementAttributeStr(xelement, "Preview");
			}
		}
	}

	private void InitFubenConf()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		ZuduiFubenKuaFuPart.FubenConf fubenConf = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			if (this.fubenTabDict.ContainsKey(xelementAttributeInt))
			{
				fubenConf = null;
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ID");
				if (!this.fubenDict.TryGetValue(xelementAttributeInt2, ref fubenConf))
				{
					fubenConf = new ZuduiFubenKuaFuPart.FubenConf();
					this.fubenDict.Add(xelementAttributeInt2, fubenConf);
				}
				fubenConf.TabID = Global.GetXElementAttributeInt(xelement, "TabID");
				fubenConf.zhuanshengLevelNeed = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
				fubenConf.levelNeed = Global.GetXElementAttributeInt(xelement, "MinLevel");
				fubenConf.finishNumber = Global.GetXElementAttributeInt(xelement, "FinishNumber");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MapCode");
				GameInstance.Game.SpriteQureyFuBenInfo(xelementAttributeInt3, xelementAttributeInt2);
			}
		}
	}

	private void InitFubenMapConf()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
		if (gameResXml == null)
		{
			return;
		}
		int CopyID = 0;
		ZuduiFubenKuaFuPart.FubenMapConf fubenMapConf = null;
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		XElement gameResXml2 = Global.GetGameResXml("Config/FuBen.Xml");
		List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "Copy");
		foreach (XElement xelement in xelementList)
		{
			CopyID = Global.GetXElementAttributeInt(xelement, "CopyID");
			if (this.fubenDict.ContainsKey(CopyID))
			{
				fubenMapConf = null;
				if (!this.fubenMapDict.TryGetValue(CopyID, ref fubenMapConf))
				{
					fubenMapConf = new ZuduiFubenKuaFuPart.FubenMapConf();
					fubenMapConf.Experienceaward = Global.GetXElementAttributeInt(xelement, "Experienceaward");
					fubenMapConf.Moneyaward = Global.GetXElementAttributeInt(xelement, "Moneyaward");
					fubenMapConf.GoodIDs = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
					fubenMapConf.Fenmoaward = Global.GetXElementAttributeInt(xelement, "YuanSuFenMoaward");
					fubenMapConf.YingGuangaward = Global.GetXElementAttributeInt(xelement, "YingGuangaward");
					XElement xelement2 = xelementList2.Find((XElement s) => s.AttributeInt("ID") == CopyID);
					if (xelement2 != null)
					{
						fubenMapConf.TuiJianZhanLi = Global.GetXElementAttributeInt(xelement2, "ZhanLi");
					}
					this.fubenMapDict.Add(CopyID, fubenMapConf);
				}
			}
		}
	}

	private void InitFubenItem()
	{
		this.FubenItemCollection.Clear();
		ZuduiFubtnItem zuduiFubtnItem = null;
		zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
		zuduiFubtnItem.SetBakUrl("-1");
		zuduiFubtnItem.ShowFanbei = false;
		zuduiFubtnItem.CanSelect = false;
		zuduiFubtnItem.bak.enabled = false;
		this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
		zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
		foreach (int num in this.fubenTabDict.Keys)
		{
			zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
			zuduiFubtnItem.SetBakUrl(this.fubenTabDict[num].Preview);
			foreach (int num2 in this.fubenDict.Keys)
			{
				if (this.fubenDict[num2].TabID == num)
				{
					zuduiFubtnItem.FubenID = num2;
					zuduiFubtnItem.NeedLevel = string.Concat(new object[]
					{
						"[",
						this.fubenDict[num2].zhuanshengLevelNeed,
						Global.GetLang("转]"),
						this.fubenDict[num2].levelNeed
					});
					zuduiFubtnItem.FinishNum = string.Empty + ActivityTipManager.GetFubenItemData(num2).FinishNum;
					zuduiFubtnItem.FinishNumLimit = "/" + this.fubenDict[num2].finishNumber;
					zuduiFubtnItem.RewardExp = string.Empty + this.fubenMapDict[num2].Experienceaward;
					zuduiFubtnItem.RewardMoney = string.Empty + this.fubenMapDict[num2].Moneyaward;
					zuduiFubtnItem.RewardFenMo = this.fubenMapDict[num2].Fenmoaward;
					zuduiFubtnItem.RewardGoods = this.fubenMapDict[num2].GoodIDs;
					zuduiFubtnItem.YingGuangaward = this.fubenMapDict[num2].YingGuangaward;
					zuduiFubtnItem.ShowFanbei = Global.isFanbei(5);
					zuduiFubtnItem.CanSelect = true;
					zuduiFubtnItem.m_lblZhanLiStatic.enabled = true;
					zuduiFubtnItem.m_lblZhanLi.text = string.Empty + this.fubenMapDict[num2].TuiJianZhanLi;
					if (Global.Data.roleData.CombatForce < this.fubenMapDict[num2].TuiJianZhanLi)
					{
						zuduiFubtnItem.m_lblZhanLi.text = "{F70101}" + zuduiFubtnItem.m_lblZhanLi.text + "{-}";
					}
				}
			}
			this.CouldEnterFuben(zuduiFubtnItem);
			this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
			zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
			zuduiFubtnItem.bak.enabled = false;
			UIPanel component = zuduiFubtnItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
		zuduiFubtnItem = U3DUtils.NEW<ZuduiFubtnItem>();
		zuduiFubtnItem.SetBakUrl("-1");
		zuduiFubtnItem.ShowFanbei = false;
		zuduiFubtnItem.CanSelect = false;
		this.FubenItemCollection.AddNoUpdate(zuduiFubtnItem);
		zuduiFubtnItem.bak.enabled = false;
		zuduiFubtnItem.GetComponent<BoxCollider>().center = new Vector3(0f, -0.5f, -1f);
		for (int i = 0; i < 4; i++)
		{
			ZuduiFubtnItem zuduiFubtnItem2 = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[i]);
			zuduiFubtnItem2.bak.enabled = false;
			UIPanel component2 = zuduiFubtnItem2.GetComponent<UIPanel>();
			if (component2)
			{
				Object.Destroy(component2);
			}
		}
	}

	public void NotifyCreateTeam(int teamId)
	{
	}

	public void NotifyQuitTeam()
	{
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[this.currentFubenIdx]);
	}

	public void NotifyJoinTeam()
	{
	}

	private bool CanStartFuben()
	{
		bool flag = false;
		foreach (CopyTeamMemberData copyTeamMemberData in Global.Data.CurrentCopyTeamData.TeamRoles)
		{
			flag = copyTeamMemberData.IsReady;
			if (!flag)
			{
				break;
			}
		}
		return flag;
	}

	private bool CouldEnterFuben(ZuduiFubtnItem fubenItem)
	{
		int fubenID = fubenItem.FubenID;
		bool flag = false;
		bool flag2 = false;
		if (this.fubenDict.ContainsKey(fubenID))
		{
			ZuduiFubenKuaFuPart.FubenConf fubenConf = this.fubenDict[fubenID];
			if (Global.Data.roleData.ChangeLifeCount > fubenConf.zhuanshengLevelNeed)
			{
				flag = true;
			}
			else if (Global.Data.roleData.ChangeLifeCount == fubenConf.zhuanshengLevelNeed && Global.Data.roleData.Level >= fubenConf.levelNeed)
			{
				flag = true;
			}
			else
			{
				fubenItem.NeedLevel = "{ff0000}" + fubenItem.NeedLevel + "{-}";
			}
			if (ActivityTipManager.GetFubenItemData(fubenID).FinishNum < this.fubenDict[fubenID].finishNumber)
			{
				flag2 = true;
			}
			else
			{
				fubenItem.FinishNum = "{ff0000}" + fubenItem.FinishNum + "{-}";
				fubenItem.FinishNumLimit = "{ff0000}" + fubenItem.FinishNumLimit + "{-}";
			}
		}
		return flag && flag2;
	}

	public void ProcessErrorCode(ZuduiFubenKuaFuPart.ZuduiErrCode err)
	{
		string message = string.Empty;
		switch (err + 12)
		{
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)0:
			message = Global.GetLang("被踢出房间");
			break;
		case ZuduiFubenKuaFuPart.ZuduiErrCode.ERR_SUCCESS:
			message = Global.GetLang("离开房间");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)5:
			message = Global.GetLang("没有合适的房间");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)6:
			message = Global.GetLang("未达到房间的最低战力要求");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)7:
			message = Global.GetLang("房间已满");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)8:
			message = Global.GetLang("你不是房主(房主才能执行)");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)9:
			message = Global.GetLang("已有房间");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)10:
			message = Global.GetLang("房间已解散");
			break;
		case (ZuduiFubenKuaFuPart.ZuduiErrCode)11:
			message = Global.GetLang("你已经不在房间中");
			break;
		}
		string[] buttons = new string[]
		{
			Global.GetLang("确定")
		};
		Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
		{
			if (e1.ID == 0)
			{
			}
		}, buttons);
	}

	public void QuitTeam()
	{
	}

	private void FuBenSelectedChange(object sender, MouseEvent e)
	{
		this.SelectetedFubenItem();
	}

	private void SelectetedFubenItem()
	{
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox.SelectedItem);
		if (null == zuduiFubtnItem)
		{
			return;
		}
		if (!zuduiFubtnItem.CanSelect)
		{
			return;
		}
		if (this.fubenItem != null && this.fubenItem != zuduiFubtnItem)
		{
			this.fubenItem.bak.enabled = false;
		}
		if (zuduiFubtnItem == this.fubenItem)
		{
			return;
		}
		this.fubenItem = zuduiFubtnItem;
		this.fubenItem.bak.enabled = true;
		this.CouldEnterFuben(zuduiFubtnItem);
		if (zuduiFubtnItem.FubenID == 70000)
		{
			this.m_labRule.text = Global.GetLang("\r\n{FFD35F}【规则介绍】{-}\r\n1、副本4个角落会依次出现{00FF00}天启四骑士{-}，四骑士全部被击杀后在中间会召唤最\r\n      终BOSS-{00FF00}末日审判{-}.击杀末日审判副本通关。\r\n2、每在{00FF00}规定时间{-}内击杀一个骑士，最终奖励{00FF00}提升50%{-}，但是最终BOSS末日审判\r\n      的攻击、防御、生命也会相应提高，{FF0000}需要挑战者们量力而行{-}。\r\n");
		}
		if (zuduiFubtnItem.FubenID == 70100)
		{
			this.m_labRule.text = Global.GetLang("\r\n{FFD35F}【规则介绍】{-}\r\n1、战斗开始后副本中会{00FF00}不断刷新怪物{-}，在规定时间内杀光一波刷新下一波，若\r\n     {00FF00}用时较少{-}波数可一次提升{00FF00}1至4波{-}，规定时间内{FF0000}未全部击杀{-}则副本结束。\r\n2、每新刷一波时间会被{00FF00}重置{-}，波数提高怪物的属性也随之提高，副本最大为{00FF00}30{-}   \r\n     波，{00FF00}波数越多最终的奖励越高{-}。\r\n{00FF00}提示：{-}元素恐惧者有{FF0000}极高的防御{-}，荧石的{00FF00}元素攻击{-}可对其造成大量伤害。\r\n");
		}
		if (zuduiFubtnItem.FubenID == 70200)
		{
			this.m_labRule.text = Global.GetLang("\r\n{FFD35F}【规则介绍】{-}\r\n1、战斗开始后副本中会{00FF00}不断刷新怪物{-}，在规定时间内杀光一波刷新下一波，若\r\n     {00FF00}用时较少{-}波数可一次提升{00FF00}1至4波{-}，规定时间内{FF0000}未全部击杀{-}则副本结束。\r\n2、每新刷一波时间会被{00FF00}重置{-}，波数提高怪物的属性也随之提高，副本最大为{00FF00}30{-}   \r\n     波，{00FF00}波数越多最终的奖励越高{-}。\r\n{00FF00}提示：{-}元素恐惧者有{FF0000}极高的防御{-}，荧石的{00FF00}元素攻击{-}可对其造成大量伤害。\r\n");
		}
	}

	private void checkFubenItem(int selected)
	{
		ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[selected]);
	}

	private void CheckAllItem()
	{
		for (int i = 0; i < this.fubenListBox.Count(); i++)
		{
			ZuduiFubtnItem zuduiFubtnItem = U3DUtils.AS<ZuduiFubtnItem>(this.fubenListBox[i]);
			this.CouldEnterFuben(zuduiFubtnItem);
		}
	}

	public static ZuduiFubenKuaFuPart Instance;

	public GButton btnCreate;

	public ListBox fubenListBox;

	public Transform m_XunZhaoDuiShou;

	public UIButton m_BtnQuXiao;

	private ObservableCollection FubenItemCollection;

	public UILabel ConstText;

	public Transform m_XunZhaoDuiShouDengDai;

	public Transform m_XunZhaoDuiShouJieGuo;

	public UIButton m_BtnJinRu;

	public UIButton m_BtnQuXiao1;

	public UILabel m_LabelDaoJiShi1;

	public UILabel m_LabelDaoJiShi;

	public UILabel m_LabelXunDengDai;

	public UILabel m_LabeTime;

	public UILabel m_LabelShiFouJinRu;

	public UILabel m_labRule;

	private int currentFubenIdx;

	private Dictionary<int, ZuduiFubenKuaFuPart.FubenTabConf> fubenTabDict = new Dictionary<int, ZuduiFubenKuaFuPart.FubenTabConf>();

	private Dictionary<int, ZuduiFubenKuaFuPart.FubenConf> fubenDict = new Dictionary<int, ZuduiFubenKuaFuPart.FubenConf>();

	private Dictionary<int, ZuduiFubenKuaFuPart.FubenMapConf> fubenMapDict = new Dictionary<int, ZuduiFubenKuaFuPart.FubenMapConf>();

	private static long EndTime = 0L;

	private static bool IsLoadTime = true;

	private static int thisWave = 0;

	public static MUSocketConnectEventArgs e_temp;

	public static bool IsShowTime = true;

	public static Dictionary<int, string> IDValueOfMoRiShenPan = new Dictionary<int, string>();

	public static XElement XElementMoRiShenPan = null;

	private ZuduiFubtnItem fubenItem;

	public enum ZuduiErrCode
	{
		ERR_SUCCESS = 1,
		ERR_NO_TEAM = -1,
		ERR_TEAM_IS_DESTORYED = -2,
		ERR_ALLREADY_HAS_TEAM = -3,
		ERR_NOT_TEAM_LEADER = -4,
		ERR_TEAM_IS_FULL = -5,
		ERR_FORCE_LOW = -6,
		ERR_NO_ACCEPTABLE_TEAM = -7,
		ERR_LEAVE_TEAM = -11,
		ERR_KICK_OUT = -12
	}

	private class FubenTabConf
	{
		public string name = string.Empty;

		public string Preview = string.Empty;
	}

	private class FubenConf
	{
		public int TabID;

		public int zhuanshengLevelNeed;

		public int levelNeed;

		public int finishNumber;
	}

	private class FubenMapConf
	{
		public int Moneyaward;

		public int Experienceaward;

		public string GoodIDs;

		public int Fenmoaward = -1;

		public int TuiJianZhanLi;

		public int YingGuangaward;
	}
}
