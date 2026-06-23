using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using HTMLEngine;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ChatBoxHTMLLink : MonoBehaviour
{
	private void Start()
	{
		HtEngine.LinkHoverColor = HtColor.Parse("#FF4444");
		HtEngine.LinkPressedFactor = 0.5f;
		HtEngine.LinkFunctionName = "onLinkClicked";
	}

	public void onLinkClicked(GameObject senderGo)
	{
		NGUILinkText component = senderGo.GetComponent<NGUILinkText>();
		if (component != null && component.linkText != null)
		{
			string linkText = component.linkText;
			if (linkText.IndexOf("RoleID") != -1)
			{
				string[] array = linkText.Split(new char[]
				{
					'_',
					'#'
				});
				int num = Convert.ToInt32(array[1]);
				if (num != Global.Data.roleData.RoleID && !Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) && Global.GetMapSceneUIClass() != SceneUIClasses.LangHunLingYu)
				{
					GameInstance.Game.SpriteGetOtherAttrib(num);
				}
				return;
			}
			if (linkText.IndexOf("NoticeID") != -1)
			{
				string[] array = linkText.Split(new char[]
				{
					'_',
					'#'
				});
				int id = Convert.ToInt32(array[1]);
				KFCompNotice systemNoticeByID = ShiLiData.GetSystemNoticeByID(id);
				if (systemNoticeByID == null)
				{
					return;
				}
				ShiLiData.GoToNoticePlace(systemNoticeByID);
				PlayZone playZone = Super.GData.PlayZoneRoot as PlayZone;
				playZone.CloseGameChatBoxWindow();
				return;
			}
			else
			{
				try
				{
					string[] array = linkText.Split(new char[]
					{
						'_'
					});
					string text = array[0];
					string text2 = array[1];
					string text3 = array[2];
					if (text.StartsWith("team"))
					{
						array = linkText.Split(new char[]
						{
							'_',
							'#'
						});
						int num2 = Global.SafeConvertToInt32(array[1]);
						int num3 = Global.SafeConvertToInt32(array[2]);
						int num4 = Global.SafeConvertToInt32(array[3]);
						long CopyTeamID = long.Parse(array[4]);
						int num5 = Global.SafeConvertToInt32(array[5]);
						int num6 = num2;
						XElement xelement = null;
						XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
						if (gameResXml != null)
						{
							List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
							foreach (XElement xelement2 in xelementList)
							{
								int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "ID");
								if (xelementAttributeInt == num6)
								{
									xelement = xelement2;
									break;
								}
							}
							if (xelement != null)
							{
								string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("AccessibleMap", ',');
								if (systemParamStringArrayByName.Length > 0)
								{
									int num7 = systemParamStringArrayByName.IndexOf(Global.Data.roleData.MapCode.ToString());
									if (num7 < 0)
									{
										string msg = string.Format(Global.GetLang("只有处于本服主线地图时才可以加入队伍"), new object[0]);
										Super.HintMainText(msg, 10, 3);
										return;
									}
								}
								int trigger = 0;
								int param = 0;
								int param2 = 0;
								if (num5 == 1)
								{
									if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZuDuiFuBen, ref trigger, ref param, ref param2))
									{
										UIHelper.HintGongNengOpenCondition(GongNengIDs.ZuDuiFuBen, trigger, param, param2, true);
										return;
									}
								}
								else if (num5 == 2)
								{
									if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.YuansuShiLian, ref trigger, ref param, ref param2))
									{
										UIHelper.HintGongNengOpenCondition(GongNengIDs.YuansuShiLian, trigger, param, param2, true);
										return;
									}
								}
								else if (num5 == 3)
								{
									if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuWanMoTa, ref trigger, ref param, ref param2))
									{
										UIHelper.HintGongNengOpenCondition(GongNengIDs.KuaFuWanMoTa, trigger, param, param2, true);
										return;
									}
								}
								else if (num5 == 4)
								{
									if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MoriShenPan, ref trigger, ref param, ref param2))
									{
										UIHelper.HintGongNengOpenCondition(GongNengIDs.MoriShenPan, trigger, param, param2, true);
										return;
									}
								}
								else if (num5 == 5 && !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LangHunYaoSai, ref trigger, ref param, ref param2))
								{
									UIHelper.HintGongNengOpenCondition(GongNengIDs.LangHunYaoSai, trigger, param, param2, true);
									return;
								}
								bool flag = false;
								bool flag2 = false;
								int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "TabID");
								int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
								int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MinLevel");
								int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "FinishNumber");
								int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "ZhanLi");
								if (Global.Data.roleData.ChangeLifeCount > xelementAttributeInt3)
								{
									flag = true;
								}
								else if (Global.Data.roleData.ChangeLifeCount == xelementAttributeInt3 && Global.Data.roleData.Level >= xelementAttributeInt4)
								{
									flag = true;
								}
								if (ActivityTipManager.GetFubenItemData(num6).FinishNum < xelementAttributeInt5)
								{
									flag2 = true;
								}
								if (!flag)
								{
									string msg2 = string.Format(Global.GetLang("等级不够"), new object[0]);
									Super.HintMainText(msg2, 10, 3);
								}
								else if (!flag2)
								{
									Super.HintMainText(Global.GetLang("该副本次数已用完"), 10, 3);
								}
								else if (Global.Data.roleData.CombatForce < num3)
								{
									Super.HintMainText(Global.GetLang("战力不符合条件"), 10, 3);
								}
								else
								{
									if (num4 == Global.Data.roleData.RoleID)
									{
									}
									CommonFlagX.CommonFlagData commonFlagData = new CommonFlagX.CommonFlagData();
									commonFlagData.name = "team";
									commonFlagData.strdata = linkText;
									commonFlagData.dic.Add("SceneIndex", num2);
									commonFlagData.dic.Add("MinZhanLi", num3);
									commonFlagData.dic.Add("LeaderRoleID", num4);
									commonFlagData.dic.Add("CopyTeamID", CopyTeamID);
									commonFlagData.dic.Add("IsKuaFuFuBen", num5);
									CommonFlagX.AddFlagData(commonFlagData);
									int zhanLi = 0;
									if (!Global.CanEnterFuBenByZhanLi(num6, out zhanLi))
									{
										PlayZone.GlobalPlayZone.OpenFuBenTiShiPartWindow(1, zhanLi);
										PlayZone.GlobalPlayZone.m_FuBenTiShiPart.dpsHandler = delegate(object s, DPSelectedItemEventArgs e)
										{
											if (e.ID == 1)
											{
												ZuduiFubenPart.SendSpriteCopyTeam(TeamCmds.Apply, CopyTeamID, 0, 0);
											}
										};
									}
									else
									{
										ZuduiFubenPart.SendSpriteCopyTeam(TeamCmds.Apply, CopyTeamID, 0, 0);
									}
								}
							}
						}
					}
					else
					{
						GTipServiceEx.ShowTip(null, TipTypes.GoodsText, GoodsOwnerTypes.ChatGoods, GoodsPriceUnitTypes.Jinbi, 0, Convert.ToInt32(text), Convert.ToInt32(text2), Convert.ToInt32(text3), null);
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogError<string>(new string[]
					{
						ex.ToString()
					});
				}
			}
		}
	}
}
