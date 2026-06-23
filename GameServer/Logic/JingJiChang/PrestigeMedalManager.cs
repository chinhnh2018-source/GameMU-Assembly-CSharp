using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic.JingJiChang
{
	public class PrestigeMedalManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx
	{
		public static PrestigeMedalManager getInstance()
		{
			return PrestigeMedalManager.instance;
		}

		public bool initialize()
		{
			return PrestigeMedalManager.initPrestigeMedal();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(782, 1, 1, PrestigeMedalManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(783, 2, 2, PrestigeMedalManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public void processEvent(EventObject eventObject)
		{
		}

		public void processEvent(EventObjectEx eventObject)
		{
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 782:
				result = this.ProcessCmdPrestigeMedalInfo(client, nID, bytes, cmdParams);
				break;
			case 783:
				result = this.ProcessCmdPrestigeMedalUp(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public bool ProcessCmdPrestigeMedalInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (cmdParams.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), cmdParams.Length), null, true);
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), num), null, true);
					return false;
				}
				PrestigeMedalData prestigeMedalData = PrestigeMedalManager.GetPrestigeMedalData(client);
				client.sendCmd<PrestigeMedalData>(782, prestigeMedalData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessCmdPrestigeMedalUp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (cmdParams.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), cmdParams.Length), null, true);
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int medalID = Convert.ToInt32(cmdParams[1]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), num), null, true);
					return false;
				}
				if (GameFuncControlManager.IsGameFuncDisabled(3))
				{
					LogManager.WriteLog(2, string.Format("ProcessCmdPrestigeMedalUp功能尚未开放, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(client.ClientSocket, false), num), null, true);
					return false;
				}
				PrestigeMedalData cmdData = PrestigeMedalManager.UpPrestigeMedal(client, medalID);
				client.sendCmd<PrestigeMedalData>(783, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public static bool initPrestigeMedal()
		{
			bool flag = PrestigeMedalManager.LoadPrestigeMedalBasicData();
			bool flag2 = PrestigeMedalManager.LoadPrestigeMedalSpecialData();
			return flag && flag2;
		}

		public static bool LoadPrestigeMedalBasicData()
		{
			string uri = "Config/ShengWangXunZhang.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			bool result;
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/ShengWangXunZhang.xml时出错!!!文件不存在", null, true);
				result = false;
			}
			else
			{
				try
				{
					PrestigeMedalManager._prestigeMedalBasicList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							PrestigeMedalBasicData prestigeMedalBasicData = new PrestigeMedalBasicData();
							prestigeMedalBasicData.MedalID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							prestigeMedalBasicData.MedalName = Convert.ToString(Global.GetDefAttributeStr(xelement2, "Name", ""));
							prestigeMedalBasicData.LifeMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "LifeV", "0"));
							prestigeMedalBasicData.AttackMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddAttack", "0"));
							prestigeMedalBasicData.DefenseMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddDefense", "0"));
							prestigeMedalBasicData.HitMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "HitV", "0"));
							prestigeMedalBasicData.PrestigeCost = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "CostShengWang", "0"));
							string text = Convert.ToString(Global.GetDefAttributeStr(xelement2, "QiangHua", ""));
							if (text.Length > 0)
							{
								prestigeMedalBasicData.RateList = new List<int>();
								prestigeMedalBasicData.AddNumList = new List<int[]>();
								string[] array = text.Split(new char[]
								{
									'|'
								});
								foreach (string text2 in array)
								{
									string[] array3 = text2.Split(new char[]
									{
										','
									});
									float num = float.Parse(array3[0]);
									prestigeMedalBasicData.RateList.Add((int)(num * 100f));
									List<int> list = new List<int>();
									for (int j = 1; j < array3.Length; j++)
									{
										list.Add(int.Parse(array3[j]));
									}
									prestigeMedalBasicData.AddNumList.Add(list.ToArray());
								}
							}
							PrestigeMedalManager._prestigeMedalBasicList.Add(prestigeMedalBasicData.MedalID, prestigeMedalBasicData);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/ShengWangXunZhang.xml时文件出现异常!!!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		public static bool LoadPrestigeMedalSpecialData()
		{
			string uri = "Config/ShengWangSpecialAttribute.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			bool result;
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/ShengWangSpecialAttribute.xml时出错!!!文件不存在", null, true);
				result = false;
			}
			else
			{
				try
				{
					PrestigeMedalManager._prestigeMedalSpecialList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							PrestigeMedalSpecialData prestigeMedalSpecialData = new PrestigeMedalSpecialData();
							prestigeMedalSpecialData.SpecialID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							prestigeMedalSpecialData.MedalID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedFuWen", "0"));
							prestigeMedalSpecialData.DoubleAttack = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "ZhiMingYiJi", "0"));
							prestigeMedalSpecialData.DiDouble = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "DiKangZhiMingYiJi", "0"));
							PrestigeMedalManager._prestigeMedalSpecialList.Add(prestigeMedalSpecialData.MedalID, prestigeMedalSpecialData);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/ShengWangSpecialAttribute.xml时出现异常!!!", ex, true);
					return false;
				}
				result = true;
			}
			return result;
		}

		public static PrestigeMedalBasicData GetPrestigeMedalBasicDataByID(int id)
		{
			PrestigeMedalBasicData result;
			if (PrestigeMedalManager._prestigeMedalBasicList.ContainsKey(id))
			{
				result = PrestigeMedalManager._prestigeMedalBasicList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static PrestigeMedalSpecialData GetPrestigeMedalSpecialDataByID(int id)
		{
			PrestigeMedalSpecialData result;
			if (PrestigeMedalManager._prestigeMedalSpecialList.ContainsKey(id))
			{
				result = PrestigeMedalManager._prestigeMedalSpecialList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static int GetPrestigeMedalUpCount(GameClient client)
		{
			int num = 0;
			int num2 = 0;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "PrestigeMedalUpCount");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count > 0)
			{
				num2 = roleParamsIntListFromDB[0];
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (num2 == dayOfYear)
			{
				num = roleParamsIntListFromDB[1];
			}
			else
			{
				PrestigeMedalManager.ModifyPrestigeMedalUpCount(client, num, true);
			}
			return num;
		}

		public static void ModifyPrestigeMedalUpCount(GameClient client, int count, bool writeToDB = false)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				List<int> list = new List<int>();
				list.AddRange(new int[]
				{
					TimeUtil.NowDateTime().DayOfYear,
					count
				});
				Global.SaveRoleParamsIntListToDB(client, list, "PrestigeMedalUpCount", writeToDB);
			}
		}

		public static int GetPrestigeMedalDiamond(GameClient client, int upCount)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ShengWangXunZhangZuanShi", ',');
			if (upCount >= paramValueIntArrayByName.Length)
			{
				upCount = paramValueIntArrayByName.Length - 1;
			}
			return paramValueIntArrayByName[upCount];
		}

		public static PrestigeMedalData GetPrestigeMedalData(GameClient client)
		{
			PrestigeMedalData result;
			if (GameFuncControlManager.IsGameFuncDisabled(3))
			{
				result = null;
			}
			else if (!GlobalNew.IsGongNengOpened(client, 55, false))
			{
				result = null;
			}
			else
			{
				PrestigeMedalData prestigeMedalData = client.ClientData.prestigeMedalData;
				if (prestigeMedalData == null)
				{
					prestigeMedalData = new PrestigeMedalData();
					List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "PrestigeMedal");
					PrestigeMedalBasicData prestigeMedalBasicDataByID;
					if (roleParamsIntListFromDB == null || roleParamsIntListFromDB.Count <= 0)
					{
						prestigeMedalBasicDataByID = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(PrestigeMedalManager._defaultMedalID);
						prestigeMedalData.RoleID = client.ClientData.RoleID;
						prestigeMedalData.MedalID = prestigeMedalBasicDataByID.MedalID;
						PrestigeMedalManager.ModifyPrestigeMedalData(client, prestigeMedalData, true);
					}
					else
					{
						prestigeMedalData.RoleID = client.ClientData.RoleID;
						prestigeMedalData.MedalID = roleParamsIntListFromDB[0];
						prestigeMedalData.LifeAdd = roleParamsIntListFromDB[1];
						prestigeMedalData.AttackAdd = roleParamsIntListFromDB[2];
						prestigeMedalData.DefenseAdd = roleParamsIntListFromDB[3];
						prestigeMedalData.HitAdd = roleParamsIntListFromDB[4];
						if (prestigeMedalData.MedalID > PrestigeMedalManager._prestigeMedalBasicList.Count)
						{
							prestigeMedalData.UpResultType = 3;
							prestigeMedalBasicDataByID = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(PrestigeMedalManager._prestigeMedalBasicList.Count);
						}
						else
						{
							prestigeMedalBasicDataByID = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(prestigeMedalData.MedalID);
						}
					}
					prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, PrestigeMedalManager.GetPrestigeMedalUpCount(client));
					prestigeMedalData.Prestige = prestigeMedalBasicDataByID.PrestigeCost;
					client.ClientData.prestigeMedalData = prestigeMedalData;
				}
				prestigeMedalData.PrestigeLeft = GameManager.ClientMgr.GetShengWangValue(client);
				result = prestigeMedalData;
			}
			return result;
		}

		public static void ModifyPrestigeMedalData(GameClient client, PrestigeMedalData data, bool writeToDB = false)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				List<int> list = new List<int>();
				list.AddRange(new int[]
				{
					data.MedalID,
					data.LifeAdd,
					data.AttackAdd,
					data.DefenseAdd,
					data.HitAdd
				});
				Global.SaveRoleParamsIntListToDB(client, list, "PrestigeMedal", writeToDB);
			}
		}

		public static PrestigeMedalData UpPrestigeMedal(GameClient client, int MedalID)
		{
			PrestigeMedalData prestigeMedalData = client.ClientData.prestigeMedalData;
			PrestigeMedalData result;
			if (prestigeMedalData != null && prestigeMedalData.UpResultType == 3)
			{
				prestigeMedalData.UpResultType = -4;
				result = prestigeMedalData;
			}
			else if (prestigeMedalData == null || prestigeMedalData.MedalID != MedalID)
			{
				prestigeMedalData.UpResultType = 0;
				result = prestigeMedalData;
			}
			else if (!GlobalNew.IsGongNengOpened(client, 55, false))
			{
				prestigeMedalData.UpResultType = -1;
				result = prestigeMedalData;
			}
			else
			{
				PrestigeMedalBasicData prestigeMedalBasicDataByID = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(MedalID);
				int shengWangValue = GameManager.ClientMgr.GetShengWangValue(client);
				if (prestigeMedalBasicDataByID.PrestigeCost > shengWangValue)
				{
					prestigeMedalData.UpResultType = -2;
					result = prestigeMedalData;
				}
				else
				{
					int prestigeMedalUpCount = PrestigeMedalManager.GetPrestigeMedalUpCount(client);
					int prestigeMedalDiamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, prestigeMedalUpCount);
					if (prestigeMedalDiamond > 0 && !GameManager.ClientMgr.SubUserMoney(client, prestigeMedalDiamond, "声望勋章提升", true, true, true, true, DaiBiSySType.ShengWangYinJi))
					{
						prestigeMedalData.UpResultType = -3;
						result = prestigeMedalData;
					}
					else
					{
						try
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, -prestigeMedalBasicDataByID.PrestigeCost, "声望勋章提升", false, true);
						}
						catch (Exception)
						{
							prestigeMedalData.UpResultType = -2;
							return prestigeMedalData;
						}
						int[] array = null;
						int num = 0;
						int randomNumber = Global.GetRandomNumber(0, 100);
						for (int i = 0; i < prestigeMedalBasicDataByID.RateList.Count; i++)
						{
							num += prestigeMedalBasicDataByID.RateList[i];
							if (randomNumber <= num)
							{
								array = prestigeMedalBasicDataByID.AddNumList[i];
								prestigeMedalData.BurstType = i;
								break;
							}
						}
						prestigeMedalData.LifeAdd += array[0] * PrestigeMedalManager._medalRate;
						prestigeMedalData.LifeAdd = ((prestigeMedalData.LifeAdd > prestigeMedalBasicDataByID.LifeMax) ? prestigeMedalBasicDataByID.LifeMax : prestigeMedalData.LifeAdd);
						prestigeMedalData.AttackAdd += array[1] * PrestigeMedalManager._medalRate;
						prestigeMedalData.AttackAdd = ((prestigeMedalData.AttackAdd > prestigeMedalBasicDataByID.AttackMax) ? prestigeMedalBasicDataByID.AttackMax : prestigeMedalData.AttackAdd);
						prestigeMedalData.DefenseAdd += array[2] * PrestigeMedalManager._medalRate;
						prestigeMedalData.DefenseAdd = ((prestigeMedalData.DefenseAdd > prestigeMedalBasicDataByID.DefenseMax) ? prestigeMedalBasicDataByID.DefenseMax : prestigeMedalData.DefenseAdd);
						prestigeMedalData.HitAdd += array[3] * PrestigeMedalManager._medalRate;
						prestigeMedalData.HitAdd = ((prestigeMedalData.HitAdd > prestigeMedalBasicDataByID.HitMax) ? prestigeMedalBasicDataByID.HitMax : prestigeMedalData.HitAdd);
						if (prestigeMedalData.LifeAdd < prestigeMedalBasicDataByID.LifeMax || prestigeMedalData.DefenseAdd < prestigeMedalBasicDataByID.DefenseMax || prestigeMedalData.AttackAdd < prestigeMedalBasicDataByID.AttackMax || prestigeMedalData.HitAdd < prestigeMedalBasicDataByID.HitMax)
						{
							prestigeMedalData.UpResultType = 1;
							prestigeMedalData.Prestige = prestigeMedalBasicDataByID.PrestigeCost;
							prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, prestigeMedalUpCount + 1);
						}
						else
						{
							prestigeMedalData.MedalID++;
							prestigeMedalData.LifeAdd = 0;
							prestigeMedalData.AttackAdd = 0;
							prestigeMedalData.DefenseAdd = 0;
							prestigeMedalData.HitAdd = 0;
							prestigeMedalData.UpResultType = 2;
							if (prestigeMedalData.MedalID > PrestigeMedalManager._prestigeMedalBasicList.Count)
							{
								prestigeMedalData.UpResultType = 3;
								prestigeMedalData.Prestige = 0;
								prestigeMedalData.Diamond = 0;
							}
							else
							{
								prestigeMedalBasicDataByID = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(prestigeMedalData.MedalID);
								prestigeMedalData.Prestige = prestigeMedalBasicDataByID.PrestigeCost;
								prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, prestigeMedalUpCount + 1);
							}
						}
						PrestigeMedalManager.ModifyPrestigeMedalUpCount(client, prestigeMedalUpCount + 1, true);
						PrestigeMedalManager.ModifyPrestigeMedalData(client, prestigeMedalData, true);
						client.ClientData.prestigeMedalData = prestigeMedalData;
						PrestigeMedalManager.SetPrestigeMedalProps(client, prestigeMedalData);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						prestigeMedalData.PrestigeLeft = GameManager.ClientMgr.GetShengWangValue(client);
						result = prestigeMedalData;
					}
				}
			}
			return result;
		}

		public static void initSetPrestigeMedalProps(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				if (GlobalNew.IsGongNengOpened(client, 55, false))
				{
					PrestigeMedalData prestigeMedalData = PrestigeMedalManager.GetPrestigeMedalData(client);
					PrestigeMedalManager.SetPrestigeMedalProps(client, prestigeMedalData);
				}
			}
		}

		public static void SetPrestigeMedalProps(GameClient client, PrestigeMedalData PrestigeMedalData)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(3))
			{
				int num = PrestigeMedalData.LifeAdd;
				int num2 = PrestigeMedalData.AttackAdd;
				int num3 = PrestigeMedalData.DefenseAdd;
				int num4 = PrestigeMedalData.HitAdd;
				foreach (PrestigeMedalBasicData prestigeMedalBasicData in PrestigeMedalManager._prestigeMedalBasicList.Values)
				{
					if (prestigeMedalBasicData.MedalID < PrestigeMedalData.MedalID)
					{
						num += prestigeMedalBasicData.LifeMax;
						num2 += prestigeMedalBasicData.AttackMax;
						num3 += prestigeMedalBasicData.DefenseMax;
						num4 += prestigeMedalBasicData.HitMax;
					}
				}
				double num5 = 0.0;
				double num6 = 0.0;
				if (PrestigeMedalData.MedalID > 1)
				{
					PrestigeMedalSpecialData prestigeMedalSpecialDataByID = PrestigeMedalManager.GetPrestigeMedalSpecialDataByID(PrestigeMedalData.MedalID - 1);
					num5 += prestigeMedalSpecialDataByID.DoubleAttack;
					num6 += prestigeMedalSpecialDataByID.DiDouble;
				}
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					13,
					num
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					45,
					num2
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					46,
					num3
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					18,
					num4
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					36,
					num5
				});
				client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
				{
					9,
					53,
					num6
				});
			}
		}

		public static void SetPrestigeLevel(GameClient client, int level)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShengWangLevel", level, true, "2020-12-12 12:12:12");
			GameManager.logDBCmdMgr.AddDBLogInfo(-1, "声望等级", "GM", "系统", client.ClientData.RoleName, "修改", level, client.ClientData.ZoneID, client.strUserID, level, client.ServerId, null);
			EventLogManager.AddRoleEvent(client, OpTypes.Trace, OpTags.GM, LogRecordType.IntValueWithType, new object[]
			{
				level,
				RoleAttributeType.ShengWangLevel
			});
			if (level > 0)
			{
				JingJiChangManager.getInstance().activeJunXianBuff(client, true);
			}
			Global.UpdateBufferData(client, BufferItemTypes.MU_JINGJICHANG_JUNXIAN, new double[]
			{
				(double)level - 1.0
			}, 0, true);
			ChengJiuManager.OnRoleJunXianChengJiu(client);
			Global.BroadcastClientMUShengWang(client, level);
			GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShengWangLevel, level);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			client._IconStateMgr.CheckJingJiChangJunXian(client);
			client._IconStateMgr.CheckSpecialActivity(client);
			client._IconStateMgr.CheckEverydayActivity(client);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		public static void SetPrestigeMedalLevel(GameClient client, int level)
		{
			level = ((level <= 0) ? 1 : level);
			PrestigeMedalData prestigeMedalData = new PrestigeMedalData();
			PrestigeMedalBasicData prestigeMedalBasicDataByID = PrestigeMedalManager.GetPrestigeMedalBasicDataByID(level);
			prestigeMedalData.RoleID = client.ClientData.RoleID;
			prestigeMedalData.MedalID = prestigeMedalBasicDataByID.MedalID;
			if (prestigeMedalData.MedalID > PrestigeMedalManager._prestigeMedalBasicList.Count)
			{
				prestigeMedalData.UpResultType = 3;
			}
			PrestigeMedalManager.ModifyPrestigeMedalData(client, prestigeMedalData, true);
			client.ClientData.prestigeMedalData = prestigeMedalData;
			PrestigeMedalManager.SetPrestigeMedalProps(client, prestigeMedalData);
		}

		public static void SetPrestigeMedalCount(GameClient client, int count)
		{
			count = ((count < 0) ? 0 : count);
			PrestigeMedalManager.ModifyPrestigeMedalUpCount(client, count, true);
			PrestigeMedalData prestigeMedalData = client.ClientData.prestigeMedalData;
			prestigeMedalData.Diamond = PrestigeMedalManager.GetPrestigeMedalDiamond(client, PrestigeMedalManager.GetPrestigeMedalUpCount(client));
			client.ClientData.prestigeMedalData = prestigeMedalData;
		}

		public static void SetPrestigeMedalRate(GameClient client, int rate)
		{
			PrestigeMedalManager._medalRate = rate;
		}

		private static int _medalRate = 1;

		private static int _defaultMedalID = 1;

		private static Dictionary<int, PrestigeMedalBasicData> _prestigeMedalBasicList = new Dictionary<int, PrestigeMedalBasicData>();

		private static Dictionary<int, PrestigeMedalSpecialData> _prestigeMedalSpecialList = new Dictionary<int, PrestigeMedalSpecialData>();

		private int _State = 0;

		private static PrestigeMedalManager instance = new PrestigeMedalManager();

		private enum PrestigeMedalResultType
		{
			End = 3,
			Next = 2,
			Success = 1,
			Fail = 0,
			EnoOpen = -1,
			EnoPrestige = -2,
			EnoDiamond = -3,
			EOver = -4
		}
	}
}
