using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic.UnionPalace
{
	public class UnionPalaceManager : ICmdProcessorEx, ICmdProcessor, IManager
	{
		public static UnionPalaceManager getInstance()
		{
			return UnionPalaceManager.instance;
		}

		public bool initialize()
		{
			UnionPalaceManager.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1035, 1, 1, UnionPalaceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1036, 1, 1, UnionPalaceManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1035:
				result = this.ProcessCmdUnionPalaceData(client, nID, bytes, cmdParams);
				break;
			case 1036:
				result = this.ProcessCmdUnionPalaceUp(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		private bool ProcessCmdUnionPalaceData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				UnionPalaceData cmdData = UnionPalaceManager.UnionPalaceGetData(client, false);
				client.sendCmd<UnionPalaceData>(1035, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessCmdUnionPalaceUp(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!UnionPalaceManager.IsGongNengOpened(client))
				{
					return true;
				}
				UnionPalaceData cmdData = UnionPalaceManager.UnionPalaceUp(client);
				client.sendCmd<UnionPalaceData>(1036, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public static UnionPalaceData UnionPalaceGetData(GameClient client, bool isUpdataProps = false)
		{
			bool flag = false;
			UnionPalaceData myUPData2;
			try
			{
				object lockUnionPalace;
				Monitor.Enter(lockUnionPalace = client.ClientData.LockUnionPalace, ref flag);
				UnionPalaceData myUPData = client.ClientData.MyUnionPalaceData;
				UnionPalaceBasicInfo unionPalaceBasicInfo = null;
				if (!UnionPalaceManager.IsGongNengOpened(client))
				{
					myUPData = new UnionPalaceData();
					myUPData.ResultType = -1;
					myUPData2 = myUPData;
				}
				else if (Global.GetBangHuiLevel(client) <= 0)
				{
					if (myUPData == null)
					{
						myUPData = new UnionPalaceData();
					}
					myUPData.ResultType = -2;
					if (isUpdataProps)
					{
						UnionPalaceManager.SetUnionPalaceProps(client, myUPData);
					}
					myUPData2 = myUPData;
				}
				else
				{
					if (myUPData == null)
					{
						myUPData = new UnionPalaceData();
						List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "UnionPalace");
						if (roleParamsIntListFromDB == null || roleParamsIntListFromDB.Count <= 0)
						{
							myUPData.RoleID = client.ClientData.RoleID;
							myUPData.StatueID = 1;
							UnionPalaceManager.ModifyUnionPalaceData(client, myUPData);
						}
						else
						{
							myUPData.RoleID = client.ClientData.RoleID;
							myUPData.StatueID = roleParamsIntListFromDB[0];
							myUPData.LifeAdd = roleParamsIntListFromDB[1];
							myUPData.AttackAdd = roleParamsIntListFromDB[2];
							myUPData.DefenseAdd = roleParamsIntListFromDB[3];
							myUPData.AttackInjureAdd = roleParamsIntListFromDB[4];
						}
						unionPalaceBasicInfo = UnionPalaceManager.GetUPBasicInfoByID(myUPData.StatueID);
						myUPData.StatueType = unionPalaceBasicInfo.StatueType;
						myUPData.StatueLevel = unionPalaceBasicInfo.StatueLevel;
						client.ClientData.MyUnionPalaceData = myUPData;
						UnionPalaceManager.SetUnionPalaceProps(client, myUPData);
					}
					myUPData.ZhanGongNeed = UnionPalaceManager.GetUnionPalaceZG(client, 0);
					myUPData.UnionLevel = Global.GetBangHuiLevel(client);
					myUPData.BurstType = 0;
					if (unionPalaceBasicInfo == null)
					{
						unionPalaceBasicInfo = UnionPalaceManager.GetUPBasicInfoByID(myUPData.StatueID);
					}
					if (unionPalaceBasicInfo.UnionLevel < 0)
					{
						myUPData.ResultType = 3;
						if (myUPData.UnionLevel < 9)
						{
							myUPData.ResultType = 4;
						}
					}
					else
					{
						myUPData.ResultType = 0;
						if (myUPData.UnionLevel < unionPalaceBasicInfo.UnionLevel)
						{
							myUPData.ResultType = 5;
							if (myUPData.LifeAdd != 0 || myUPData.AttackInjureAdd != 0 || myUPData.DefenseAdd != 0 || myUPData.AttackAdd != 0 || myUPData.StatueType > 1)
							{
								myUPData.ResultType = 4;
							}
							else
							{
								int num = 0;
								IOrderedEnumerable<UnionPalaceBasicInfo> source = from info in UnionPalaceManager._unionPalaceBasicList.Values
								where info.StatueID <= myUPData.StatueID && info.StatueLevel <= myUPData.StatueLevel && info.UnionLevel <= myUPData.UnionLevel && info.UnionLevel > 0
								orderby info.StatueID descending
								select info;
								if (source.Any<UnionPalaceBasicInfo>())
								{
									num = source.First<UnionPalaceBasicInfo>().StatueLevel;
								}
								if (unionPalaceBasicInfo.StatueLevel > num + 1)
								{
									myUPData.ResultType = 4;
								}
							}
						}
					}
					if (isUpdataProps)
					{
						UnionPalaceManager.SetUnionPalaceProps(client, myUPData);
					}
					myUPData.ZhanGongLeft = client.ClientData.BangGong;
					myUPData2 = myUPData;
				}
			}
			finally
			{
				if (flag)
				{
					object lockUnionPalace;
					Monitor.Exit(lockUnionPalace);
				}
			}
			return myUPData2;
		}

		public static void ModifyUnionPalaceData(GameClient client, UnionPalaceData data)
		{
			List<int> list = new List<int>();
			list.AddRange(new int[]
			{
				data.StatueID,
				data.LifeAdd,
				data.AttackAdd,
				data.DefenseAdd,
				data.AttackInjureAdd
			});
			Global.SaveRoleParamsIntListToDB(client, list, "UnionPalace", true);
		}

		public static UnionPalaceData UnionPalaceUp(GameClient client)
		{
			UnionPalaceData result;
			lock (client.ClientData.LockUnionPalace)
			{
				UnionPalaceData unionPalaceData = UnionPalaceManager.UnionPalaceGetData(client, false);
				if (unionPalaceData.ResultType < 0)
				{
					result = unionPalaceData;
				}
				else if (unionPalaceData.ResultType == 3 || unionPalaceData.ResultType == -4)
				{
					unionPalaceData.ResultType = -4;
					result = unionPalaceData;
				}
				else if (!UnionPalaceManager.IsGongNengOpened(client))
				{
					unionPalaceData.ResultType = -1;
					result = unionPalaceData;
				}
				else
				{
					UnionPalaceBasicInfo upbasicInfoByID = UnionPalaceManager.GetUPBasicInfoByID(unionPalaceData.StatueID);
					if (upbasicInfoByID.UnionLevel < 0)
					{
						unionPalaceData.ResultType = -4;
						result = unionPalaceData;
					}
					else
					{
						int bangHuiLevel = Global.GetBangHuiLevel(client);
						if (upbasicInfoByID.UnionLevel > bangHuiLevel)
						{
							unionPalaceData.ResultType = -8;
							result = unionPalaceData;
						}
						else if (bangHuiLevel < unionPalaceData.StatueLevel)
						{
							unionPalaceData.ResultType = -7;
							result = unionPalaceData;
						}
						else
						{
							int unionPalaceZG = UnionPalaceManager.GetUnionPalaceZG(client, 0);
							if (!GameManager.ClientMgr.SubUserBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, unionPalaceZG))
							{
								unionPalaceData.ResultType = -3;
								result = unionPalaceData;
							}
							else
							{
								UnionPalaceRateInfo uprateInfoByID = UnionPalaceManager.GetUPRateInfoByID(upbasicInfoByID.StatueLevel);
								int[] array = null;
								int num = 0;
								int randomNumber = Global.GetRandomNumber(0, 100);
								for (int i = 0; i < uprateInfoByID.RateList.Count; i++)
								{
									num += uprateInfoByID.RateList[i];
									if (randomNumber <= num)
									{
										array = uprateInfoByID.AddNumList[uprateInfoByID.RateList[i]].ToArray();
										unionPalaceData.BurstType = i;
										break;
									}
								}
								List<int> list = new List<int>();
								list.Add(unionPalaceData.StatueID);
								list.Add(unionPalaceData.LifeAdd);
								list.Add(unionPalaceData.AttackAdd);
								list.Add(unionPalaceData.DefenseAdd);
								list.Add(unionPalaceData.AttackInjureAdd);
								unionPalaceData.LifeAdd += array[0] * UnionPalaceManager._gmRate;
								unionPalaceData.LifeAdd = ((unionPalaceData.LifeAdd > upbasicInfoByID.LifeMax) ? upbasicInfoByID.LifeMax : unionPalaceData.LifeAdd);
								unionPalaceData.AttackAdd += array[1] * UnionPalaceManager._gmRate;
								unionPalaceData.AttackAdd = ((unionPalaceData.AttackAdd > upbasicInfoByID.AttackMax) ? upbasicInfoByID.AttackMax : unionPalaceData.AttackAdd);
								unionPalaceData.DefenseAdd += array[2] * UnionPalaceManager._gmRate;
								unionPalaceData.DefenseAdd = ((unionPalaceData.DefenseAdd > upbasicInfoByID.DefenseMax) ? upbasicInfoByID.DefenseMax : unionPalaceData.DefenseAdd);
								unionPalaceData.AttackInjureAdd += array[3] * UnionPalaceManager._gmRate;
								unionPalaceData.AttackInjureAdd = ((unionPalaceData.AttackInjureAdd > upbasicInfoByID.AttackInjureMax) ? upbasicInfoByID.AttackInjureMax : unionPalaceData.AttackInjureAdd);
								if (unionPalaceData.LifeAdd < upbasicInfoByID.LifeMax || unionPalaceData.DefenseAdd < upbasicInfoByID.DefenseMax || unionPalaceData.AttackAdd < upbasicInfoByID.AttackMax || unionPalaceData.AttackInjureAdd < upbasicInfoByID.AttackInjureMax)
								{
									unionPalaceData.ResultType = 1;
								}
								else
								{
									unionPalaceData.StatueID++;
									upbasicInfoByID = UnionPalaceManager.GetUPBasicInfoByID(unionPalaceData.StatueID);
									unionPalaceData.StatueType = upbasicInfoByID.StatueType;
									unionPalaceData.StatueLevel = upbasicInfoByID.StatueLevel;
									unionPalaceData.LifeAdd = 0;
									unionPalaceData.AttackAdd = 0;
									unionPalaceData.DefenseAdd = 0;
									unionPalaceData.AttackInjureAdd = 0;
									unionPalaceData.ResultType = 2;
									if (unionPalaceData.StatueID > UnionPalaceManager._unionPalaceBasicList.Count || upbasicInfoByID.UnionLevel < 0)
									{
										unionPalaceData.ResultType = 3;
									}
									else if (bangHuiLevel < upbasicInfoByID.UnionLevel)
									{
										unionPalaceData.ResultType = 5;
									}
									else if (bangHuiLevel < unionPalaceData.StatueLevel)
									{
										unionPalaceData.ResultType = 4;
									}
								}
								int offsetDayNow = Global.GetOffsetDayNow();
								int upCount = UnionPalaceManager.GetUpCount(client, offsetDayNow);
								unionPalaceData.ZhanGongNeed = UnionPalaceManager.GetUnionPalaceZG(client, upCount + 1);
								UnionPalaceManager.ModifyUpCount(client, upCount + 1);
								UnionPalaceManager.ModifyUnionPalaceData(client, unionPalaceData);
								client.ClientData.MyUnionPalaceData = unionPalaceData;
								UnionPalaceManager.SetUnionPalaceProps(client, unionPalaceData);
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
								unionPalaceData.ZhanGongLeft = client.ClientData.BangGong;
								list.Add(unionPalaceData.StatueID);
								list.Add(unionPalaceData.LifeAdd);
								list.Add(unionPalaceData.AttackAdd);
								list.Add(unionPalaceData.DefenseAdd);
								list.Add(unionPalaceData.AttackInjureAdd);
								list.Add(upCount);
								EventLogManager.AddUnionPalaceEvent(client, LogRecordType.UnionPalace, new object[]
								{
									list.ToArray()
								});
								result = unionPalaceData;
							}
						}
					}
				}
			}
			return result;
		}

		public static void initSetUnionPalaceProps(GameClient client, bool isUpdataProps = false)
		{
			lock (client.ClientData.LockUnionPalace)
			{
				client.ClientData.MyUnionPalaceData = null;
				UnionPalaceData unionPalaceData = UnionPalaceManager.UnionPalaceGetData(client, isUpdataProps);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			}
		}

		public static void SetUnionPalaceProps(GameClient client, UnionPalaceData myData)
		{
			lock (client.ClientData.LockUnionPalace)
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				UnionPalaceBasicInfo upbasicInfoByID = UnionPalaceManager.GetUPBasicInfoByID(myData.StatueID);
				int bangHuiLevel = Global.GetBangHuiLevel(client);
				if (upbasicInfoByID != null && upbasicInfoByID.UnionLevel <= bangHuiLevel && myData.StatueLevel <= bangHuiLevel)
				{
					num = myData.LifeAdd;
					num2 = myData.AttackAdd;
					num3 = myData.DefenseAdd;
					num4 = myData.AttackInjureAdd;
				}
				foreach (UnionPalaceBasicInfo unionPalaceBasicInfo in UnionPalaceManager._unionPalaceBasicList.Values)
				{
					if (unionPalaceBasicInfo.StatueID < myData.StatueID && unionPalaceBasicInfo.UnionLevel <= bangHuiLevel && unionPalaceBasicInfo.StatueLevel <= bangHuiLevel)
					{
						num += unionPalaceBasicInfo.LifeMax;
						num2 += unionPalaceBasicInfo.AttackMax;
						num3 += unionPalaceBasicInfo.DefenseMax;
						num4 += unionPalaceBasicInfo.AttackInjureMax;
					}
				}
				double num5 = 0.0;
				int num6 = (myData.StatueID - 1) / 8;
				if (num6 > 0)
				{
					if (myData.ResultType == 4 || myData.ResultType == -7)
					{
						int num7 = 0;
						IOrderedEnumerable<UnionPalaceBasicInfo> source = from info in UnionPalaceManager._unionPalaceBasicList.Values
						where info.StatueID <= myData.StatueID && info.StatueLevel <= myData.StatueLevel && info.UnionLevel <= myData.UnionLevel && info.UnionLevel > 0
						orderby info.StatueID descending
						select info;
						if (source.Any<UnionPalaceBasicInfo>())
						{
							num7 = source.First<UnionPalaceBasicInfo>().StatueID;
						}
						num6 = num7 / 8;
					}
					UnionPalaceSpecialInfo upspecialInfoByID = UnionPalaceManager.GetUPSpecialInfoByID(num6);
					if (upspecialInfoByID != null && upspecialInfoByID.UnionLevel <= bangHuiLevel)
					{
						num5 = upspecialInfoByID.MaxLifePercent;
					}
				}
				EquipPropItem equipPropItem = new EquipPropItem();
				equipPropItem.ExtProps[13] = (double)num;
				equipPropItem.ExtProps[45] = (double)num2;
				equipPropItem.ExtProps[46] = (double)num3;
				equipPropItem.ExtProps[27] = (double)num4;
				equipPropItem.ExtProps[14] = num5;
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					19,
					equipPropItem.ExtProps
				});
			}
		}

		public static int GetUpCount(GameClient client, int day)
		{
			int num = 0;
			int num2 = 0;
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "UnionPalaceUpCount");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count > 0)
			{
				num2 = roleParamsIntListFromDB[0];
			}
			if (num2 == day)
			{
				num = roleParamsIntListFromDB[1];
			}
			else
			{
				UnionPalaceManager.ModifyUpCount(client, num);
			}
			return num;
		}

		public static void ModifyUpCount(GameClient client, int count)
		{
			List<int> list = new List<int>();
			list.AddRange(new int[]
			{
				Global.GetOffsetDayNow(),
				count
			});
			Global.SaveRoleParamsIntListToDB(client, list, "UnionPalaceUpCount", true);
		}

		public static bool IsGongNengOpened(GameClient client)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(9) && GlobalNew.IsGongNengOpened(client, 77, false) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("UnionPalace");
		}

		public static void InitConfig()
		{
			UnionPalaceManager.LoadUnionPalaceBasicInfo();
			UnionPalaceManager.LoadUnionPalaceSpecialInfo();
			UnionPalaceManager.LoadUnionPalaceRateInfo();
		}

		private static void LoadUnionPalaceBasicInfo()
		{
			string text = Global.GameResPath("Config/ShenDianLevelUp.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					UnionPalaceManager._unionPalaceBasicList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							UnionPalaceBasicInfo unionPalaceBasicInfo = new UnionPalaceBasicInfo();
							unionPalaceBasicInfo.StatueID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							unionPalaceBasicInfo.StatueType = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Type", "0"));
							unionPalaceBasicInfo.StatueLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Level", "0"));
							unionPalaceBasicInfo.UnionLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NeedZhanMengLevel", "0"));
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "NeedStatueLevel", "");
							if (!string.IsNullOrEmpty(defAttributeStr))
							{
								string[] array = defAttributeStr.Split(new char[]
								{
									','
								});
								if (array.Length == 2)
								{
									unionPalaceBasicInfo.PreStatueType = Convert.ToInt32(array[0]);
									unionPalaceBasicInfo.PreStatueLevel = Convert.ToInt32(array[1]);
								}
							}
							unionPalaceBasicInfo.LifeMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MaxLifeV", "0"));
							unionPalaceBasicInfo.AttackMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddAttack", "0"));
							unionPalaceBasicInfo.DefenseMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddDefense", "0"));
							unionPalaceBasicInfo.AttackInjureMax = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AddAttackInjure", "0"));
							UnionPalaceManager._unionPalaceBasicList.Add(unionPalaceBasicInfo.StatueID, unionPalaceBasicInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		public static void LoadUnionPalaceSpecialInfo()
		{
			string text = Global.GameResPath("Config/ShenDianExtra.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					UnionPalaceManager._unionPalaceSpecialList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							UnionPalaceSpecialInfo unionPalaceSpecialInfo = new UnionPalaceSpecialInfo();
							unionPalaceSpecialInfo.StatueLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "StatueLevel", "0"));
							unionPalaceSpecialInfo.UnionLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ZhanMengLevel", "0"));
							unionPalaceSpecialInfo.MaxLifePercent = Convert.ToDouble(Global.GetDefAttributeStr(xelement2, "MaxLifePercent", "0"));
							UnionPalaceManager._unionPalaceSpecialList.Add(unionPalaceSpecialInfo.StatueLevel, unionPalaceSpecialInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		public static void LoadUnionPalaceRateInfo()
		{
			string text = Global.GameResPath("Config/ShenDianScale.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					UnionPalaceManager._unionPalaceRateList.Clear();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							UnionPalaceRateInfo unionPalaceRateInfo = new UnionPalaceRateInfo();
							unionPalaceRateInfo.StatueLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Level", "0"));
							string text2 = Convert.ToString(Global.GetDefAttributeStr(xelement2, "Scale", ""));
							if (text2.Length > 0)
							{
								string[] array = text2.Split(new char[]
								{
									'|'
								});
								foreach (string text3 in array)
								{
									string[] array3 = text3.Split(new char[]
									{
										','
									});
									int num = (int)(float.Parse(array3[0]) * 100f);
									unionPalaceRateInfo.RateList.Add(num);
									List<int> list = new List<int>();
									for (int j = 1; j < array3.Length; j++)
									{
										list.Add(int.Parse(array3[j]));
									}
									unionPalaceRateInfo.AddNumList.Add(num, list);
								}
								UnionPalaceManager._unionPalaceRateList.Add(unionPalaceRateInfo.StatueLevel, unionPalaceRateInfo);
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
				}
			}
		}

		public static UnionPalaceBasicInfo GetUPBasicInfoByID(int id)
		{
			UnionPalaceBasicInfo result;
			if (UnionPalaceManager._unionPalaceBasicList.ContainsKey(id))
			{
				result = UnionPalaceManager._unionPalaceBasicList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static UnionPalaceSpecialInfo GetUPSpecialInfoByID(int id)
		{
			UnionPalaceSpecialInfo result;
			if (UnionPalaceManager._unionPalaceSpecialList.ContainsKey(id))
			{
				result = UnionPalaceManager._unionPalaceSpecialList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static UnionPalaceRateInfo GetUPRateInfoByID(int id)
		{
			UnionPalaceRateInfo result;
			if (UnionPalaceManager._unionPalaceRateList.ContainsKey(id))
			{
				result = UnionPalaceManager._unionPalaceRateList[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static int GetUnionPalaceZG(GameClient client, int upCount = 0)
		{
			if (upCount <= 0)
			{
				int offsetDayNow = Global.GetOffsetDayNow();
				upCount = UnionPalaceManager.GetUpCount(client, offsetDayNow);
			}
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengShenDian", ',');
			if (upCount >= paramValueIntArrayByName.Length)
			{
				upCount = paramValueIntArrayByName.Length - 1;
			}
			return paramValueIntArrayByName[upCount];
		}

		public static void SetUnionPalaceLevelByID(GameClient client, int id)
		{
			List<int> list = new List<int>();
			List<int> list2 = list;
			int[] array = new int[5];
			array[0] = id;
			list2.AddRange(array);
			Global.SaveRoleParamsIntListToDB(client, list, "UnionPalace", true);
			client.ClientData.MyUnionPalaceData = null;
			UnionPalaceManager.initSetUnionPalaceProps(client, true);
		}

		public static void SetUnionPalaceCount(GameClient client, int count)
		{
			count = ((count < 0) ? 0 : count);
			UnionPalaceManager.ModifyUpCount(client, count);
			UnionPalaceData myUnionPalaceData = client.ClientData.MyUnionPalaceData;
			myUnionPalaceData.ZhanGongNeed = UnionPalaceManager.GetUnionPalaceZG(client, 0);
		}

		public static void SetUnionPalaceRate(GameClient client, int rate)
		{
			UnionPalaceManager._gmRate = rate;
		}

		private const int DEFAULT_MIN_ID = 1;

		private const int UP_LEVEL_MAX = 5;

		private const int STATUE_COUNT = 8;

		private const int UNION_PALACE_MAX_LEVEL = 9;

		private const int STATUE_MAX_LEVEL = 5;

		public static int _gmRate = 1;

		private static UnionPalaceManager instance = new UnionPalaceManager();

		private static Dictionary<int, UnionPalaceBasicInfo> _unionPalaceBasicList = new Dictionary<int, UnionPalaceBasicInfo>();

		private static Dictionary<int, UnionPalaceSpecialInfo> _unionPalaceSpecialList = new Dictionary<int, UnionPalaceSpecialInfo>();

		private static Dictionary<int, UnionPalaceRateInfo> _unionPalaceRateList = new Dictionary<int, UnionPalaceRateInfo>();
	}
}
