using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class JueXingManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static JueXingManager getInstance()
		{
			return JueXingManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			this.LoadAwakenActivationXml();
			this.LoadAwakenSuitXml();
			this.LoadAwakenLevelXml();
			this.LoadAwakenRecoveryXml();
			return true;
		}

		public void LoadConfig()
		{
			this.LoadDefaultXml();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1886, 1, 1, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1887, 3, 3, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1888, 3, 3, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1889, 1, 1, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1890, 2, 2, JueXingManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpen(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1886:
					result = this.ProcessGetJueXingDataCmd(client, nID, bytes, cmdParams);
					break;
				case 1887:
					result = this.ProcessJueXingJiHuoCmd(client, nID, bytes, cmdParams);
					break;
				case 1888:
					result = this.ProcessTaoZhuangChangeCmd(client, nID, bytes, cmdParams);
					break;
				case 1889:
					result = this.ProcessMoHuaCmd(client, nID, bytes, cmdParams);
					break;
				case 1890:
					result = this.ProcessHuiShouCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public bool ProcessGetJueXingDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<JueXingShiData>(nID, client.ClientData.JueXingData, false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JueXing :: 获取觉醒石数据错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessJueXingJiHuoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int suitID = Convert.ToInt32(cmdParams[1]);
				int num = Convert.ToInt32(cmdParams[2]);
				TaoZhuang taoZhuang;
				int num2;
				JueXingShiItem jueXingShiItem;
				if (!this.JueXingRunTimeData.TaoZhuangDict.TryGetValue(suitID, out taoZhuang))
				{
					num2 = -2;
				}
				else if (!this.JueXingRunTimeData.JueXingShiDict.TryGetValue(num, out jueXingShiItem))
				{
					num2 = -2;
				}
				else if (jueXingShiItem.SuitParent != suitID)
				{
					num2 = -5;
				}
				else
				{
					JueXingShiData jueXingData = client.ClientData.JueXingData;
					List<TaoZhuangData> taoZhuangList = jueXingData.TaoZhuangList;
					TaoZhuangData taoZhuangData = taoZhuangList.Find((TaoZhuangData _g) => _g.ID == suitID);
					if (null == taoZhuangData)
					{
						taoZhuangData = new TaoZhuangData
						{
							ID = suitID,
							ActiviteList = new List<int>()
						};
						taoZhuangList.Add(taoZhuangData);
					}
					if (taoZhuangData.ActiviteList.Contains(num))
					{
						num2 = -1;
					}
					else
					{
						int needGoodsID = jueXingShiItem.NeedGoodsID;
						int needGoodsNum = jueXingShiItem.NeedGoodsNum;
						bool flag;
						bool flag2;
						if (Global.UseGoodsBindOrNot(client, needGoodsID, needGoodsNum, true, out flag, out flag2) < 1)
						{
							num2 = -3;
						}
						else
						{
							taoZhuangData.ActiviteList.Add(num);
							string cmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, suitID, string.Join<int>(",", taoZhuangData.ActiviteList));
							num2 = Global.sendToDB<int, string>(20318, cmd, client.ServerId);
							Global.RefreshEquipProp(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num2, suitID, num), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JueXing :: 激活觉醒石错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessTaoZhuangChangeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 3))
				{
					return false;
				}
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[1]);
				int suitID = Convert.ToInt32(cmdParams[2]);
				if (suitID > 0)
				{
					TaoZhuang taoZhuang;
					if (!this.JueXingRunTimeData.TaoZhuangDict.TryGetValue(suitID, out taoZhuang))
					{
						num = -2;
						goto IL_1A0;
					}
					if (taoZhuang.Type != num2)
					{
						num = -6;
						goto IL_1A0;
					}
					if (client.ClientData.JueXingData.TaoZhuangList.Find((TaoZhuangData x) => x.ID == suitID) == null)
					{
						num = -7;
						goto IL_1A0;
					}
				}
				if (num2 == 1)
				{
					client.ClientData.JueXingData.AttackEquip = suitID;
					Global.SaveRoleParamsInt32ValueToDB(client, "10191", suitID, true);
				}
				else if (num2 == 2)
				{
					client.ClientData.JueXingData.DefenseEquip = suitID;
					Global.SaveRoleParamsInt32ValueToDB(client, "10192", suitID, true);
				}
				Global.RefreshEquipProp(client);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				IL_1A0:
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, num2, suitID), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JueXing :: 更换套装错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessMoHuaCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.JueXingRunTimeData.MoHuaOpen)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = 0;
				int num2 = Global.GetRoleParamsInt32FromDB(client, "10193");
				int jueXingJie = client.ClientData.JueXingData.JueXingJie;
				int jueXingJi = client.ClientData.JueXingData.JueXingJi;
				num2++;
				AwakenLevelItem awakenLevelItem;
				if (!this.JueXingRunTimeData.AwakenLevelDict.TryGetValue(num2, out awakenLevelItem))
				{
					num = -8;
				}
				else if (client.ClientData.JueXingZhiChen < (long)awakenLevelItem.Awakenment)
				{
					num = -9;
				}
				else
				{
					string[] array = awakenLevelItem.AwakenAdvancedment.Split(new char[]
					{
						','
					});
					if (array.Length > 1)
					{
						int needID = Convert.ToInt32(array[0]);
						int needCount = Convert.ToInt32(array[1]);
						bool flag;
						bool flag2;
						if (Global.UseGoodsBindOrNot(client, needID, needCount, true, out flag, out flag2) < 1)
						{
							num = -3;
							goto IL_1BB;
						}
					}
					GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, -awakenLevelItem.Awakenment, "觉醒魔化消耗", true, true, false);
					Global.SaveRoleParamsInt32ValueToDB(client, "10193", num2, true);
					client.ClientData.JueXingData.JueXingJie = awakenLevelItem.Order;
					client.ClientData.JueXingData.JueXingJi = awakenLevelItem.Star;
					this.UpdataPalyerJueXingAttr(client, true);
				}
				IL_1BB:
				client.sendCmd(nID, string.Format("{0}:{1}:{2}", num, client.ClientData.JueXingData.JueXingJie, client.ClientData.JueXingData.JueXingJi), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JueXing :: 觉醒魔化错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public bool ProcessHuiShouCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.JueXingRunTimeData.MoHuaOpen)
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = 0;
				foreach (string text in cmdParams[1].Split(new char[]
				{
					'|'
				}))
				{
					string[] array2 = text.Split(new char[]
					{
						','
					});
					if (array2.Length < 3)
					{
						break;
					}
					int num2 = Convert.ToInt32(array2[0]);
					int num3 = Convert.ToInt32(array2[1]);
					int num4 = Convert.ToInt32(array2[2]);
					int num5 = 0;
					if (!this.JueXingRunTimeData.AwakenRecoveryDict.TryGetValue(num2, out num5))
					{
						num = -2;
						break;
					}
					num5 *= num3;
					if (num4 > 0)
					{
						if (Global.GetTotalBindGoodsCountByID(client, num2) < num3)
						{
							num = -10;
							break;
						}
						bool flag;
						bool flag2;
						if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, num3, false, out flag, out flag2, false))
						{
							num = -10;
							break;
						}
					}
					else if (num4 < 1)
					{
						if (Global.GetTotalNotBindGoodsCountByID(client, num2) < num3)
						{
							num = -10;
							break;
						}
						bool flag;
						bool flag2;
						if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num2, num3, false, out flag, out flag2, false))
						{
							num = -10;
							break;
						}
					}
					GameManager.ClientMgr.ModifyJueXingZhiChenValue(client, num5, "碎片分解增加觉醒之尘", true, true, false);
				}
				client.sendCmd(nID, string.Format("{0}", num), false);
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("JueXing :: 觉醒魔化错误。rid:{0}, ex:{1}", client.ClientData.RoleID, ex.Message), null, true);
			}
			return false;
		}

		public void UpdataPalyerJueXingAttr(GameClient client, bool hint = true)
		{
			if (this.IsGongNengOpen(client, false))
			{
				double[] array = new double[177];
				if (null != client.ClientData.JueXingData)
				{
					foreach (TaoZhuangData taoZhuangData in client.ClientData.JueXingData.TaoZhuangList)
					{
						foreach (int num in taoZhuangData.ActiviteList)
						{
							if (num > 0)
							{
								JueXingShiItem jueXingShiItem;
								if (this.JueXingRunTimeData.JueXingShiDict.TryGetValue(num, out jueXingShiItem))
								{
									if (this.CanAddAttribute(client, jueXingShiItem.Position))
									{
										for (int i = 0; i < 177; i++)
										{
											array[i] += jueXingShiItem.ExtProps[i];
										}
									}
								}
							}
						}
					}
					AwakenLevelItem awakenLevelItem;
					if (this.JueXingRunTimeData.AwakenLevelDict.TryGetValue(Global.GetRoleParamsInt32FromDB(client, "10193"), out awakenLevelItem))
					{
						for (int j = 0; j < 177; j++)
						{
							array[j] *= 1.0 + awakenLevelItem.EnlargeRate / 100.0;
							array[j] += awakenLevelItem.ExtProps[j];
						}
					}
					client.PassiveEffectList.Clear();
					List<PassiveSkillData> skillList = new List<PassiveSkillData>();
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.WeaponMaster,
						new double[177]
					});
					this.UpdateTaoZhuangAttr(client, client.ClientData.JueXingData.GetAttackTaoZhuang(), ref skillList, ref array);
					this.UpdateTaoZhuangAttr(client, client.ClientData.JueXingData.GetDefenseTaoZhuang(), ref skillList, ref array);
					client.passiveSkillModule.UpdateOtherSkillList(skillList);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.JueXingShi,
						array
					});
					if (hint)
					{
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
			}
		}

		public void UpdateTaoZhuangAttr(GameClient client, TaoZhuangData taoZhuangEquip, ref List<PassiveSkillData> passiveSkillList, ref double[] _ExtProps)
		{
			if (null != taoZhuangEquip)
			{
				TaoZhuang taoZhuang;
				if (this.JueXingRunTimeData.TaoZhuangDict.TryGetValue(taoZhuangEquip.ID, out taoZhuang))
				{
					int count = taoZhuangEquip.ActiviteList.FindAll(delegate(int _x)
					{
						JueXingShiItem jueXingShiItem;
						return this.JueXingRunTimeData.JueXingShiDict.TryGetValue(_x, out jueXingShiItem) && this.CanAddAttribute(client, jueXingShiItem.Position);
					}).Count;
					if (count >= taoZhuang.TaoZhuangProps3Num && taoZhuang.TaoZhuangProps3Num > 0)
					{
						for (int i = 0; i < 177; i++)
						{
							_ExtProps[i] += taoZhuang.TaoZhuangProps3[i];
							_ExtProps[i] += taoZhuang.TaoZhuangProps2[i];
							_ExtProps[i] += taoZhuang.TaoZhuangProps1[i];
						}
					}
					else if (count >= taoZhuang.TaoZhuangProps2Num && taoZhuang.TaoZhuangProps2Num > 0)
					{
						for (int i = 0; i < 177; i++)
						{
							_ExtProps[i] += taoZhuang.TaoZhuangProps2[i];
							_ExtProps[i] += taoZhuang.TaoZhuangProps1[i];
						}
					}
					else if (count >= taoZhuang.TaoZhuangProps1Num && taoZhuang.TaoZhuangProps1Num > 0)
					{
						for (int i = 0; i < 177; i++)
						{
							_ExtProps[i] += taoZhuang.TaoZhuangProps1[i];
						}
					}
					if (taoZhuang.Type == 1)
					{
						client.ClientData.PropsCacheManager.SetExtProps(new object[]
						{
							PropsSystemTypes.WeaponMaster,
							new double[177]
						});
						if (count >= taoZhuang.WeaponMasterNum)
						{
							WeaponMaster.UpdateRoleAttr(client, taoZhuang.WeaponMasterType, false);
						}
					}
					foreach (List<int> list in taoZhuang.PassiveSkill)
					{
						if (list.Count > 1 && count >= list[0])
						{
							for (int i = 1; i < list.Count; i++)
							{
								SystemXmlItem systemXmlItem = null;
								if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(list[i], out systemXmlItem))
								{
									passiveSkillList.Add(new PassiveSkillData
									{
										skillId = list[i],
										triggerRate = (int)(systemXmlItem.GetDoubleValue("TriggerOdds") * 100.0),
										triggerType = systemXmlItem.GetIntValue("TriggerType", -1),
										coolDown = systemXmlItem.GetIntValue("CDTime", -1),
										triggerCD = systemXmlItem.GetIntValue("TriggerCD", -1)
									});
								}
							}
						}
					}
					foreach (List<int> list2 in taoZhuang.PassiveEffect)
					{
						if (list2.Count > 1 && count >= list2[0])
						{
							for (int i = 1; i < list2.Count; i++)
							{
								client.PassiveEffectList.Add(list2[i]);
							}
						}
					}
				}
			}
		}

		public void LoadDefaultXml()
		{
			try
			{
				lock (this.JueXingRunTimeData.Mutex)
				{
					this.JueXingRunTimeData.MoHuaOpen = (1L == GameManager.systemParamsList.GetParamValueIntByName("AwakenLevelUpOpen", -1));
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("AwakenCondition", ',');
					if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length == 2)
					{
						this.JueXingRunTimeData.SuitIDLimit = paramValueIntArrayByName[0];
						this.JueXingRunTimeData.ExcellencePropLimit = paramValueIntArrayByName[1];
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", "SystemParams.xml"), ex, true);
			}
		}

		public void LoadAwakenActivationXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(JueXingConsts.AwakenActivation);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, JueXingShiItem> dictionary = new Dictionary<int, JueXingShiItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							int[] array = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xelement2, "Material", "").Split(new char[]
							{
								','
							}), (string x) => Convert.ToInt32(x));
							if (array.Length < 2)
							{
								LogManager.WriteLog(2, string.Format("加载xml配置文件:{0}, 错误。", text), null, true);
							}
							else
							{
								string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "BaseProps");
								string[] array2 = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								double[] array3 = new double[177];
								foreach (string text2 in array2)
								{
									string[] array5 = text2.Split(new char[]
									{
										','
									});
									if (array5.Length == 2)
									{
										ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array5[0]);
										if (propIndexByPropName < ExtPropIndexes.Max)
										{
											array3[(int)propIndexByPropName] = Global.SafeConvertToDouble(array5[1]);
										}
									}
								}
								dictionary[num] = new JueXingShiItem
								{
									ID = num,
									Position = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Position", "0")),
									NeedGoodsID = array[0],
									NeedGoodsNum = array[1],
									ExtProps = array3
								};
								lock (this.JueXingRunTimeData.Mutex)
								{
									this.JueXingRunTimeData.JueXingShiDict = dictionary;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadAwakenSuitXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(JueXingConsts.AwakenSuit);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, TaoZhuang> dictionary = new Dictionary<int, TaoZhuang>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "TaoZhuangProps1");
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							double[] array2 = new double[177];
							foreach (string text2 in array)
							{
								string[] array4 = text2.Split(new char[]
								{
									','
								});
								if (array4.Length == 2)
								{
									ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
									if (propIndexByPropName < ExtPropIndexes.Max)
									{
										array2[(int)propIndexByPropName] = Global.SafeConvertToDouble(array4[1]);
									}
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "TaoZhuangProps2");
							array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							double[] array5 = new double[177];
							foreach (string text2 in array)
							{
								string[] array4 = text2.Split(new char[]
								{
									','
								});
								if (array4.Length == 2)
								{
									ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
									if (propIndexByPropName < ExtPropIndexes.Max)
									{
										array5[(int)propIndexByPropName] = Global.SafeConvertToDouble(array4[1]);
									}
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "TaoZhuangProps3");
							array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							double[] array6 = new double[177];
							foreach (string text2 in array)
							{
								string[] array4 = text2.Split(new char[]
								{
									','
								});
								if (array4.Length == 2)
								{
									ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
									if (propIndexByPropName < ExtPropIndexes.Max)
									{
										array6[(int)propIndexByPropName] = Global.SafeConvertToDouble(array4[1]);
									}
								}
							}
							string[] array7 = Global.GetDefAttributeStr(xelement2, "WeaponMaster", "").Split(new char[]
							{
								','
							});
							int weaponMasterNum = 0;
							int weaponMasterType = 0;
							if (array7.Length > 1)
							{
								weaponMasterNum = Convert.ToInt32(array7[0]);
								weaponMasterType = Convert.ToInt32(array7[1]);
							}
							List<List<int>> list = new List<List<int>>();
							string[] array8 = Global.GetDefAttributeStr(xelement2, "Magic", "").Split(new char[]
							{
								'|'
							});
							foreach (string text3 in array8)
							{
								if (!string.IsNullOrEmpty(text3))
								{
									string[] array9 = text3.Split(new char[]
									{
										','
									});
									if (array9.Length > 1)
									{
										list.Add(Array.ConvertAll<string, int>(array9, (string x) => Convert.ToInt32(x)).ToList<int>());
									}
								}
							}
							List<List<int>> list2 = new List<List<int>>();
							string[] array10 = Global.GetDefAttributeStr(xelement2, "PassiveEffect", "").Split(new char[]
							{
								'|'
							});
							foreach (string text3 in array10)
							{
								if (!string.IsNullOrEmpty(text3))
								{
									string[] array11 = text3.Split(new char[]
									{
										','
									});
									if (array11.Length > 1)
									{
										list2.Add(Array.ConvertAll<string, int>(array11, (string x) => Convert.ToInt32(x)).ToList<int>());
									}
								}
							}
							Dictionary<int, TaoZhuang> dictionary2 = dictionary;
							int key = num;
							TaoZhuang taoZhuang = new TaoZhuang();
							taoZhuang.ID = num;
							taoZhuang.Type = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Type", "0"));
							taoZhuang.AwakenList = Array.ConvertAll<string, int>(Global.GetDefAttributeStr(xelement2, "AwakenID", "").Split(new char[]
							{
								','
							}), (string x) => Convert.ToInt32(x)).ToList<int>();
							taoZhuang.TaoZhuangProps1Num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "TaoZhuangProps1Num", "0"));
							taoZhuang.TaoZhuangProps1 = array2;
							taoZhuang.TaoZhuangProps2Num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "TaoZhuangProps2Num", "0"));
							taoZhuang.TaoZhuangProps2 = array5;
							taoZhuang.TaoZhuangProps3Num = Global.SafeConvertToInt32(Global.GetDefAttributeStr(xelement2, "TaoZhuangProps3Num", "0"));
							taoZhuang.TaoZhuangProps3 = array6;
							taoZhuang.WeaponMasterNum = weaponMasterNum;
							taoZhuang.WeaponMasterType = weaponMasterType;
							taoZhuang.PassiveSkill = list;
							taoZhuang.PassiveEffect = list2;
							dictionary2[key] = taoZhuang;
						}
					}
					lock (this.JueXingRunTimeData.Mutex)
					{
						this.JueXingRunTimeData.TaoZhuangDict = dictionary;
						foreach (TaoZhuang taoZhuang2 in dictionary.Values)
						{
							foreach (int num in taoZhuang2.AwakenList)
							{
								if (this.JueXingRunTimeData.JueXingShiDict.ContainsKey(num))
								{
									this.JueXingRunTimeData.JueXingShiDict[num].SuitParent = taoZhuang2.ID;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadAwakenLevelXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(JueXingConsts.AwakenLevel);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, AwakenLevelItem> dictionary = new Dictionary<int, AwakenLevelItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					double[] array = new double[177];
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "AdvancedEffect");
							string[] array2 = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array2.Length > 0)
							{
								foreach (string text2 in array2)
								{
									string[] array4 = text2.Split(new char[]
									{
										','
									});
									if (array4.Length == 2)
									{
										ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array4[0]);
										if (propIndexByPropName < ExtPropIndexes.Max)
										{
											array[(int)propIndexByPropName] += Global.SafeConvertToDouble(array4[1]);
										}
									}
								}
							}
							dictionary[num] = new AwakenLevelItem
							{
								ID = num,
								Order = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Order", "0")),
								Star = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Star", "0")),
								Awakenment = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Awakenment", "0")),
								AwakenAdvancedment = Global.GetDefAttributeStr(xelement2, "AwakenAdvancedment", "0"),
								EnlargeRate = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "EnlargeRate", "0")),
								ExtProps = (double[])array.Clone()
							};
						}
					}
					lock (this.JueXingRunTimeData.Mutex)
					{
						this.JueXingRunTimeData.AwakenLevelDict = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void LoadAwakenRecoveryXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(JueXingConsts.AwakenRecovery);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							int key = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GoodsID", "0"));
							dictionary[key] = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AwakenNum", "0"));
						}
					}
					lock (this.JueXingRunTimeData.Mutex)
					{
						this.JueXingRunTimeData.AwakenRecoveryDict = dictionary;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public void InitRoleJueXingData(GameClient client)
		{
			if (this.IsGongNengOpen(client, false))
			{
				if (null == client.ClientData.JueXingData)
				{
					List<TaoZhuangData> list = Global.sendToDB<List<TaoZhuangData>, string>(20317, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
					if (null == list)
					{
						list = new List<TaoZhuangData>();
					}
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10193");
					int jueXingJie = 1;
					int jueXingJi = 0;
					AwakenLevelItem awakenLevelItem;
					if (this.JueXingRunTimeData.AwakenLevelDict.TryGetValue(roleParamsInt32FromDB, out awakenLevelItem))
					{
						jueXingJie = awakenLevelItem.Order;
						jueXingJi = awakenLevelItem.Star;
					}
					client.ClientData.JueXingData = new JueXingShiData
					{
						AttackEquip = Global.GetRoleParamsInt32FromDB(client, "10191"),
						DefenseEquip = Global.GetRoleParamsInt32FromDB(client, "10192"),
						TaoZhuangList = list,
						JueXingJie = jueXingJie,
						JueXingJi = jueXingJi
					};
				}
				this.UpdataPalyerJueXingAttr(client, true);
			}
		}

		public bool IsGongNengOpen(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(16) && GlobalNew.IsGongNengOpened(client, 93, hint);
		}

		public bool CanAddAttribute(GameClient client, int position)
		{
			int begin = 0;
			int end = 0;
			int bagIndex = 3;
			switch (position)
			{
			case 1:
				bagIndex = 2;
				begin = 11;
				end = 21;
				break;
			case 2:
				begin = (end = 5);
				break;
			case 3:
				bagIndex = 0;
				begin = (end = 6);
				break;
			case 4:
				bagIndex = 1;
				begin = (end = 6);
				break;
			case 5:
				begin = (end = 0);
				break;
			case 6:
				begin = (end = 1);
				break;
			case 7:
				begin = (end = 2);
				break;
			case 8:
				begin = (end = 3);
				break;
			case 9:
				begin = (end = 4);
				break;
			default:
				return false;
			}
			return client.ClientData.GoodsDataList.Find(delegate(GoodsData _g)
			{
				bool result;
				if (_g.Using != 1)
				{
					result = false;
				}
				else if (bagIndex < 2 && _g.BagIndex != bagIndex)
				{
					result = false;
				}
				else if (Global.GetEquipExcellencePropNum(_g) < this.JueXingRunTimeData.ExcellencePropLimit)
				{
					result = false;
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(_g.GoodsID, out systemXmlItem))
					{
						result = false;
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
						result = (intValue <= end && intValue >= begin && systemXmlItem.GetIntValue("SuitID", -1) >= this.JueXingRunTimeData.SuitIDLimit);
					}
				}
				return result;
			}) != null;
		}

		public JueXingRunData JueXingRunTimeData = new JueXingRunData();

		private static JueXingManager instance = new JueXingManager();
	}
}
