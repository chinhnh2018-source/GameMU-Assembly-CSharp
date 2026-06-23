using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.TuJian
{
	public class GuardStatueManager : SingletonTemplate<GuardStatueManager>
	{
		private GuardStatueManager()
		{
		}

		public void LoadConfig()
		{
			if (!this.loadGuardSoul() || !this.loadGuardPoint() || !this.loadGuardLevelUp() || !this.loadGuardSuitUp())
			{
			}
		}

		private bool loadGuardSoul()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/TuJianShouHuType.xml"));
				if (xelement == null)
				{
					LogManager.WriteLog(2, string.Format("{0} 不存在!", Global.GameResPath("Config/TuJianShouHuType.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						GuardSoul guardSoul = new GuardSoul();
						guardSoul.TypeID = (int)Global.GetSafeAttributeLong(xelement2, "Type");
						guardSoul.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						guardSoul.GoodsID = (int)Global.GetSafeAttributeLong(xelement2, "GoodsID");
						this.guardSoulDict.Add(guardSoul.TypeID, guardSoul);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("{0} 读取出错!", Global.GameResPath("Config/TuJianShouHuType.xml")), ex, true);
				return false;
			}
			return true;
		}

		private bool loadGuardPoint()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/JingPoShouHu.xml"));
				if (xelement == null)
				{
					LogManager.WriteLog(2, string.Format("{0} 不存在!", Global.GameResPath("Config/JingPoShouHu.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						GuardPoint guardPoint = new GuardPoint();
						guardPoint.ItemID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						guardPoint.TypeID = (int)Global.GetSafeAttributeLong(xelement2, "Type");
						guardPoint.Name = Global.GetSafeAttributeStr(xelement2, "Name");
						guardPoint.GoodsID = (int)Global.GetSafeAttributeLong(xelement2, "GoodsID");
						guardPoint.Point = (int)Global.GetSafeAttributeLong(xelement2, "ShouHuAward");
						this.guardPointDict.Add(guardPoint.ItemID, guardPoint);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("{0} 读取出错!", Global.GameResPath("Config/JingPoShouHu.xml")), ex, true);
				return false;
			}
			return true;
		}

		private bool loadGuardLevelUp()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/ShouHuLevelUp.xml"));
				if (xelement == null)
				{
					LogManager.WriteLog(2, string.Format("{0} 不存在!", Global.GameResPath("Config/ShouHuLevelUp.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						GuardLevelUp guardLevelUp = new GuardLevelUp();
						guardLevelUp.Level = (int)Global.GetSafeAttributeLong(xelement2, "Level");
						guardLevelUp.NeedGuardPoint = (int)Global.GetSafeAttributeLong(xelement2, "NeedShouHu");
						this.guardLevelUpDict.Add(guardLevelUp.Level, guardLevelUp);
						this.GuardStatueMaxLevel = Math.Max(this.GuardStatueMaxLevel, guardLevelUp.Level);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("{0} 读取出错!", Global.GameResPath("Config/ShouHuLevelUp.xml")), ex, true);
				return false;
			}
			return true;
		}

		private bool loadGuardSuitUp()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/ShouHuSuitUp.xml"));
				if (xelement == null)
				{
					LogManager.WriteLog(2, string.Format("{0} 不存在!", Global.GameResPath("Config/ShouHuSuitUp.xml")), null, true);
					return false;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					if (xelement2 != null)
					{
						GuardSuitUp guardSuitUp = new GuardSuitUp();
						guardSuitUp.Suit = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "NeedGoods");
						guardSuitUp.GoodsCost = ConfigHelper.ParserIntArrayList(safeAttributeStr, true, '|', ',');
						this.guardSuitUpDict.Add(guardSuitUp.Suit, guardSuitUp);
						this.GuardStatueMaxSuit = Math.Max(this.GuardStatueMaxSuit, guardSuitUp.Suit);
					}
				}
				this.GuardStatueMaxSuit = (int)((sbyte)Global.GMin(this.GuardStatueMaxSuit, (int)GameManager.systemParamsList.GetParamValueIntByName("ShouHuShenMax", 0)));
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("{0} 读取出错!", Global.GameResPath("Config/ShouHuSuitUp.xml")), ex, true);
				return false;
			}
			return true;
		}

		public void OnTaskComplete(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(5))
			{
				if (client != null && !client.ClientData.MyGuardStatueDetail.IsActived)
				{
					this.CheckGuardStatueOpenInfo(client);
				}
			}
		}

		public void OnLogin(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(5))
			{
				if (client != null)
				{
					this.CheckGuardStatueOpenInfo(client);
					this.UpdateGuardStatueProps(client);
				}
			}
		}

		public void OnActiveTuJian(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(5))
			{
				if (client != null)
				{
					this.CheckGuardStatueOpenInfo(client);
				}
			}
		}

		private void CheckGuardStatueOpenInfo(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(5))
			{
				if (client != null)
				{
					if (!client.ClientData.MyGuardStatueDetail.IsActived)
					{
						if (GlobalNew.IsGongNengOpened(client, 60, false))
						{
							client.ClientData.MyGuardStatueDetail.IsActived = true;
							client.ClientData.MyGuardStatueDetail.GuardStatue.Level = 0;
							client.ClientData.MyGuardStatueDetail.GuardStatue.Suit = 1;
							client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint = 0;
							client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Clear();
							client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint = 0;
							client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset = Global.GetOffsetDay(TimeUtil.NowDateTime());
							client.ClientData.MyGuardStatueDetail.ActiveSoulSlot = 0;
							if (!this._UpdateGuardStatue2DB(client))
							{
								client.ClientData.MyGuardStatueDetail.IsActived = false;
							}
						}
					}
					if (client.ClientData.MyGuardStatueDetail.IsActived)
					{
						using (HashSet<int>.Enumerator enumerator = client.ClientData.ActivedTuJianType.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								int type = enumerator.Current;
								if (!client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Exists((GuardSoulData soul) => soul.Type == type))
								{
									GuardSoulData guardSoulData = new GuardSoulData
									{
										Type = type,
										EquipSlot = -1
									};
									if (this._UpdateGuardSoul2DB(client.ClientData.RoleID, guardSoulData.Type, guardSoulData.EquipSlot, client.ServerId))
									{
										client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(guardSoulData);
									}
								}
							}
						}
						int count = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count;
						int slotCntBySoulCnt = this.GetSlotCntBySoulCnt(count);
						if (client.ClientData.MyGuardStatueDetail.ActiveSoulSlot != slotCntBySoulCnt)
						{
							int activeSoulSlot = client.ClientData.MyGuardStatueDetail.ActiveSoulSlot;
							client.ClientData.MyGuardStatueDetail.ActiveSoulSlot = slotCntBySoulCnt;
							if (!this._UpdateGuardStatue2DB(client))
							{
								client.ClientData.MyGuardStatueDetail.ActiveSoulSlot = activeSoulSlot;
							}
						}
					}
				}
			}
		}

		private void UpdateGuardStatueProps(GameClient client)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(5))
			{
				if (client != null && client.ClientData.MyGuardStatueDetail.IsActived)
				{
					EquipPropItem equipPropItem = new EquipPropItem();
					foreach (GuardSoulData guardSoulData in client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList)
					{
						if (guardSoulData.EquipSlot != -1)
						{
							GuardSoul guardSoul = null;
							if (this.guardSoulDict.TryGetValue(guardSoulData.Type, out guardSoul))
							{
								EquipPropItem equipPropItem2 = GameManager.EquipPropsMgr.FindEquipPropItem(guardSoul.GoodsID);
								if (equipPropItem2 != null)
								{
									int level = client.ClientData.MyGuardStatueDetail.GuardStatue.Level;
									int suit = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
									for (int i = 0; i < equipPropItem2.ExtProps.Length; i++)
									{
										equipPropItem.ExtProps[i] += equipPropItem2.ExtProps[i] * (1.0 + (double)level * this.LevelFactor + (double)(suit - 1) * this.SuitFactor);
									}
								}
							}
						}
					}
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.GuardStatue,
						equipPropItem.ExtProps
					});
				}
			}
		}

		private bool _UpdateGuardStatue2DB(GameClient client)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = false;
			}
			else if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = false;
			}
			else
			{
				int roleID = client.ClientData.RoleID;
				int activeSoulSlot = client.ClientData.MyGuardStatueDetail.ActiveSoulSlot;
				int level = client.ClientData.MyGuardStatueDetail.GuardStatue.Level;
				int suit = client.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
				int hasGuardPoint = client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				int lastdayRecoverPoint = client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint;
				int lastdayRecoverOffset = client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset;
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					client.ClientData.RoleID,
					activeSoulSlot,
					level,
					suit,
					hasGuardPoint,
					lastdayRecoverPoint,
					lastdayRecoverOffset
				});
				string[] array = Global.ExecuteDBCmd(13210, strcmd, client.ServerId);
				if (array != null && array.Length != 1 && Convert.ToInt32(array[0]) < 0)
				{
					LogManager.WriteLog(2, string.Format("更新角色守护雕像失败，roleid={0}, slot={1}, level={2}, suit={3}, totalGuardPoint={4}, lastdayRecoverPoint={5}, lastdayRecoverOffset={6}", new object[]
					{
						roleID,
						activeSoulSlot,
						level,
						suit,
						hasGuardPoint,
						lastdayRecoverPoint,
						lastdayRecoverOffset
					}), null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		private bool _UpdateGuardSoul2DB(int roleid, int soulType, int equipSlot, int serverId)
		{
			bool result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = false;
			}
			else
			{
				string text = string.Format("{0}:{1}:{2}", roleid, soulType, equipSlot);
				string[] array = Global.ExecuteDBCmd(13211, text, serverId);
				if (array != null && array.Length != 1 && Convert.ToInt32(array[0]) < 0)
				{
					LogManager.WriteLog(2, "更新角色守护之灵信息失败，" + text, null, true);
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcRoleQueryGuardPointRecover(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = 0;
				int num3 = 0;
				GuardStatueErrorCode guardStatueErrorCode = this.QueryGuardPointRecoverInfo(client, out num2, out num3);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)guardStatueErrorCode, num2, num3), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private GuardStatueErrorCode QueryGuardPointRecoverInfo(GameClient client, out int todayHasRecover, out int todayMaxRecover)
		{
			todayHasRecover = 0;
			todayMaxRecover = 0;
			GuardStatueErrorCode result;
			if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				int count = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count;
				int offsetDay = Global.GetOffsetDay(TimeUtil.NowDateTime());
				if (client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset != offsetDay)
				{
					client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint = 0;
					client.ClientData.MyGuardStatueDetail.LastdayRecoverOffset = offsetDay;
					if (!this._UpdateGuardStatue2DB(client))
					{
					}
				}
				todayHasRecover = client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint;
				todayMaxRecover = this.GetDayMaxCanRecoverPointBySoulCnt(count);
				result = GuardStatueErrorCode.Success;
			}
			return result;
		}

		public TCPProcessCmdResults ProcRoleGuardPointRecover(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length <= 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int i = 1; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						int num2 = Convert.ToInt32(array2[0]);
						int num3 = Convert.ToInt32(array2[1]);
						if (dictionary.ContainsKey(num2))
						{
							Dictionary<int, int> dictionary2;
							int key;
							(dictionary2 = dictionary)[key = num2] = dictionary2[key] + num3;
						}
						else
						{
							dictionary.Add(num2, num3);
						}
					}
				}
				GuardStatueErrorCode guardStatueErrorCode = this.RecoverGuardPoint(client, dictionary);
				int num4 = 0;
				int num5 = 0;
				this.QueryGuardPointRecoverInfo(client, out num4, out num5);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)guardStatueErrorCode, num4, num5), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private GuardStatueErrorCode RecoverGuardPoint(GameClient client, Dictionary<int, int> itemDict)
		{
			GuardStatueErrorCode result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (client == null || itemDict == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				List<Tuple<int, int>> list = new List<Tuple<int, int>>();
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				this.QueryGuardPointRecoverInfo(client, out num2, out num3);
				foreach (KeyValuePair<int, int> keyValuePair in itemDict)
				{
					int key = keyValuePair.Key;
					int value = keyValuePair.Value;
					if (client.ClientData.ActivedTuJianItem.Contains(key))
					{
						GuardPoint guardPoint = null;
						if (this.guardPointDict.TryGetValue(key, out guardPoint))
						{
							if (client.ClientData.ActivedTuJianType.Contains(guardPoint.TypeID))
							{
								int num4 = num3 - num2 - num;
								if (num4 <= 0)
								{
									break;
								}
								int num5 = num4 / guardPoint.Point + ((num4 % guardPoint.Point != 0) ? 1 : 0);
								num5 = Math.Max(0, Math.Min(num5, value));
								if (Global.GetTotalGoodsCountByID(client, guardPoint.GoodsID) >= num5)
								{
									list.Add(new Tuple<int, int>(guardPoint.GoodsID, num5));
									num += guardPoint.Point * num5;
									if (num >= num3 - num2)
									{
										break;
									}
								}
							}
						}
					}
				}
				if (list.Count == 0 || num <= 0)
				{
					result = GuardStatueErrorCode.MaterialNotEnough;
				}
				else
				{
					if (num > num3 - num2)
					{
					}
					foreach (Tuple<int, int> tuple in list)
					{
						int item = tuple.Item1;
						int item2 = tuple.Item2;
						bool flag = false;
						bool flag2 = false;
						GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, item, item2, false, out flag, out flag2, false);
					}
					GuardStatueData guardStatue = client.ClientData.MyGuardStatueDetail.GuardStatue;
					int num6 = Math.Min(num, num3 - num2);
					guardStatue.HasGuardPoint += num6;
					client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint += num6;
					if (!this._UpdateGuardStatue2DB(client))
					{
						guardStatue.HasGuardPoint -= num6;
						client.ClientData.MyGuardStatueDetail.LastdayRecoverPoint -= num6;
						result = GuardStatueErrorCode.DBFailed;
					}
					else
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, guardStatue.HasGuardPoint);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.GuardPoint, (long)num6, (long)guardStatue.HasGuardPoint, "回收守护点");
						result = GuardStatueErrorCode.Success;
					}
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcRoleQueryGuardStatue(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GuardStatueData>(gameClient.ClientData.MyGuardStatueDetail.GuardStatue, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcRoleGuardStatueLevelUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GuardStatueErrorCode guardStatueErrorCode = this.HandleLevelUp(gameClient);
				int level = gameClient.ClientData.MyGuardStatueDetail.GuardStatue.Level;
				int hasGuardPoint = gameClient.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)guardStatueErrorCode, level, hasGuardPoint), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public GuardStatueErrorCode HandleLevelUp(GameClient client)
		{
			GuardStatueErrorCode result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				GuardStatueData guardStatue = client.ClientData.MyGuardStatueDetail.GuardStatue;
				if (guardStatue.Level >= this.GuardStatueMaxLevel)
				{
					result = GuardStatueErrorCode.LevelIsFull;
				}
				else if (guardStatue.Level > 0 && guardStatue.Level % 10 == 0 && (guardStatue.Level + 10) / 10 != guardStatue.Suit)
				{
					result = GuardStatueErrorCode.NeedSuitUp;
				}
				else
				{
					GuardLevelUp guardLevelUp = null;
					if (!this.guardLevelUpDict.TryGetValue(guardStatue.Level + 1, out guardLevelUp))
					{
						result = GuardStatueErrorCode.ConfigError;
					}
					else if (guardLevelUp.NeedGuardPoint > guardStatue.HasGuardPoint)
					{
						result = GuardStatueErrorCode.GuardPointNotEnough;
					}
					else
					{
						guardStatue.HasGuardPoint -= guardLevelUp.NeedGuardPoint;
						guardStatue.Level++;
						if (!this._UpdateGuardStatue2DB(client))
						{
							guardStatue.HasGuardPoint += guardLevelUp.NeedGuardPoint;
							guardStatue.Level--;
							result = GuardStatueErrorCode.DBFailed;
						}
						else
						{
							GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, guardStatue.HasGuardPoint);
							EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.GuardPoint, (long)(-(long)guardLevelUp.NeedGuardPoint), (long)guardStatue.HasGuardPoint, "升级守护雕像");
							this.UpdateGuardStatueProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							result = GuardStatueErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcRoleGuardStatueSuitUp(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				GuardStatueErrorCode guardStatueErrorCode = this.HandleSuitUp(gameClient);
				int suit = gameClient.ClientData.MyGuardStatueDetail.GuardStatue.Suit;
				int hasGuardPoint = gameClient.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint;
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, string.Format("{0}:{1}:{2}", (int)guardStatueErrorCode, suit, hasGuardPoint), nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private GuardStatueErrorCode HandleSuitUp(GameClient client)
		{
			GuardStatueErrorCode result;
			if (GameFuncControlManager.IsGameFuncDisabled(5))
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = GuardStatueErrorCode.NotOpen;
			}
			else
			{
				GuardStatueData guardStatue = client.ClientData.MyGuardStatueDetail.GuardStatue;
				string text = "";
				int suit = guardStatue.Suit;
				if (guardStatue.Suit >= this.GuardStatueMaxSuit)
				{
					result = GuardStatueErrorCode.SuitIsFull;
				}
				else if (guardStatue.Level == 0 || (guardStatue.Level + 10) / 10 == guardStatue.Suit)
				{
					result = GuardStatueErrorCode.NeedLevelUp;
				}
				else
				{
					GuardSuitUp guardSuitUp = null;
					if (!this.guardSuitUpDict.TryGetValue(guardStatue.Suit + 1, out guardSuitUp))
					{
						result = GuardStatueErrorCode.ConfigError;
					}
					else
					{
						for (int i = 0; i < guardSuitUp.GoodsCost.Count; i++)
						{
							int num = guardSuitUp.GoodsCost[i][0];
							int num2 = guardSuitUp.GoodsCost[i][1];
							int totalGoodsCountByID = Global.GetTotalGoodsCountByID(client, num);
							if (totalGoodsCountByID < num2)
							{
								return GuardStatueErrorCode.MaterialNotEnough;
							}
						}
						guardStatue.Suit++;
						if (!this._UpdateGuardStatue2DB(client))
						{
							guardStatue.Suit--;
							result = GuardStatueErrorCode.DBFailed;
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							for (int i = 0; i < guardSuitUp.GoodsCost.Count; i++)
							{
								int num = guardSuitUp.GoodsCost[i][0];
								int num2 = guardSuitUp.GoodsCost[i][1];
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num2, false, out flag, out flag2, false))
								{
									LogManager.WriteLog(2, string.Format("守护雕像进阶时，消耗{1}个GoodsID={0}的物品失败，但是已设置为升阶成功", num, num2), null, true);
								}
								GoodsData goodsData = new GoodsData
								{
									GoodsID = num,
									GCount = num2
								};
								text += EventLogManager.NewGoodsDataPropString(goodsData);
							}
							GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, guardStatue.HasGuardPoint);
							this.UpdateGuardStatueProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							if (client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
							{
								client._IconStateMgr.SendIconStateToClient(client);
							}
							EventLogManager.AddGuardStatueSuitEvent(client, suit, guardStatue.Suit, guardStatue.Level, guardStatue.Level, text);
							result = GuardStatueErrorCode.Success;
						}
					}
				}
			}
			return result;
		}

		public TCPProcessCmdResults ProcRoleModGuardSoulEquip(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, array.Length, text), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int slot = Convert.ToInt32(array[1]);
				int guardSoulType = Convert.ToInt32(array[2]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				this.HandleModGuardSoulEquip(gameClient, slot, guardSoulType);
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<GuardStatueData>(gameClient.ClientData.MyGuardStatueDetail.GuardStatue, pool, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		private void HandleModGuardSoulEquip(GameClient client, int slot, int guardSoulType)
		{
			if (client != null && client.ClientData.MyGuardStatueDetail.IsActived)
			{
				int activeSoulSlot = client.ClientData.MyGuardStatueDetail.ActiveSoulSlot;
				if (slot >= 0 && slot < activeSoulSlot)
				{
					GuardSoulData guardSoulData = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Find((GuardSoulData soul) => soul.EquipSlot == slot);
					GuardSoulData guardSoulData2 = null;
					if (guardSoulType != -1)
					{
						guardSoulData2 = client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Find((GuardSoulData soul) => soul.Type == guardSoulType);
					}
					if ((guardSoulData != null || guardSoulData2 != null) && guardSoulData != guardSoulData2)
					{
						if (guardSoulData2 == null || guardSoulData2.EquipSlot == -1)
						{
							if (guardSoulData != null && this._UpdateGuardSoul2DB(client.ClientData.RoleID, guardSoulData.Type, -1, client.ServerId))
							{
								guardSoulData.EquipSlot = -1;
							}
							if (guardSoulData == null || guardSoulData.EquipSlot == -1)
							{
								if (guardSoulData2 != null && this._UpdateGuardSoul2DB(client.ClientData.RoleID, guardSoulData2.Type, slot, client.ServerId))
								{
									guardSoulData2.EquipSlot = slot;
								}
							}
							this.UpdateGuardStatueProps(client);
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
							GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
						}
					}
				}
			}
		}

		private int GetSlotCntBySoulCnt(int cnt)
		{
			int num = 0;
			foreach (Tuple<int, int> tuple in this.maxActiveSlotCntList)
			{
				if (cnt >= tuple.Item1)
				{
					num = Math.Max(num, tuple.Item2);
				}
			}
			return num;
		}

		private int GetDayMaxCanRecoverPointBySoulCnt(int cnt)
		{
			int num = 0;
			foreach (Tuple<int, int> tuple in this.dayMaxCanRecoverPointList)
			{
				if (cnt >= tuple.Item1)
				{
					num = Math.Max(num, tuple.Item2);
				}
			}
			return num;
		}

		public void InitRecoverPoint_BySysParam(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				this.dayMaxCanRecoverPointList.Clear();
				string[] array = str.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						int item = Convert.ToInt32(array3[0]);
						int item2 = Convert.ToInt32(array3[1]);
						this.dayMaxCanRecoverPointList.Add(new Tuple<int, int>(item, item2));
					}
				}
			}
		}

		public void InitSoulSlot_BySysParam(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				this.maxActiveSlotCntList.Clear();
				string[] array = str.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						int item = Convert.ToInt32(array3[0]);
						int item2 = Convert.ToInt32(array3[1]);
						this.maxActiveSlotCntList.Add(new Tuple<int, int>(item2, item));
					}
				}
			}
		}

		public void GM_HandleModGuardSoulEquip(GameClient client, int slot, int guardSoulType)
		{
			this.HandleModGuardSoulEquip(client, slot, guardSoulType);
		}

		public string GM_QueryGuardPoint(GameClient client)
		{
			int num = 0;
			int num2 = 0;
			this.QueryGuardPointRecoverInfo(client, out num, out num2);
			int num3 = (client != null) ? client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint : 0;
			return string.Format("今日已回收[{0}], 今日最大可回收[{1}]，总共有守护点[{2}]", num, num2, num3);
		}

		public void GM_GuardPointRecover(GameClient client, Dictionary<int, int> itemDict)
		{
			this.RecoverGuardPoint(client, itemDict);
		}

		public void GM_HandleLevelUp(GameClient client)
		{
			this.HandleLevelUp(client);
		}

		public void GM_HandleSuitlUp(GameClient client)
		{
			this.HandleSuitUp(client);
		}

		public string GM_QueryGuardStatue(GameClient client)
		{
			string result;
			if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = "守护雕像未激活";
			}
			else
			{
				string text = string.Format("守护雕像已激活, 等级={0}, 品阶={1}, 激活的守护之灵装备栏={2}, 共有守护之灵={3}， ", new object[]
				{
					client.ClientData.MyGuardStatueDetail.GuardStatue.Level,
					client.ClientData.MyGuardStatueDetail.GuardStatue.Suit,
					this.GetSlotCntBySoulCnt(client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count),
					client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList.Count
				});
				text += this.GM_QueryGuardPoint(client);
				foreach (GuardSoulData guardSoulData in client.ClientData.MyGuardStatueDetail.GuardStatue.GuardSoulList)
				{
					text += string.Format(", 【type={0}, slot={1}】", guardSoulData.Type, guardSoulData.EquipSlot);
				}
				result = text;
			}
			return result;
		}

		public string GM_ModGuardPoint(GameClient client, int newVal)
		{
			string result;
			if (client == null || !client.ClientData.MyGuardStatueDetail.IsActived)
			{
				result = "守护雕像未激活";
			}
			else
			{
				client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint = newVal;
				if (!this._UpdateGuardStatue2DB(client))
				{
					result = "设置守护点失败";
				}
				else
				{
					GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint);
					EventLogManager.AddMoneyEvent(client, OpTypes.Set, OpTags.None, MoneyTypes.GuardPoint, 0L, (long)newVal, "GM设置");
					result = "设置守护点成功";
				}
			}
			return result;
		}

		public void AddGuardPoint(GameClient client, int point, string strFrom)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(5))
			{
				if (client != null && client.ClientData.MyGuardStatueDetail.IsActived)
				{
					client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint += point;
					if (!this._UpdateGuardStatue2DB(client))
					{
						LogManager.WriteLog(2, string.Format("AddGuardPoint failed, roleid={0}, rolename={1}, addpoint={2}", client.ClientData.RoleID, client.ClientData.RoleName, point), null, true);
					}
					else
					{
						GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TotalGuardPoint, client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint);
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "守护点", strFrom, "系统", client.ClientData.RoleName, "修改", point, client.ClientData.ZoneID, client.strUserID, client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.GuardPoint, (long)point, (long)client.ClientData.MyGuardStatueDetail.GuardStatue.HasGuardPoint, strFrom);
					}
				}
			}
		}

		private Dictionary<int, GuardSoul> guardSoulDict = new Dictionary<int, GuardSoul>();

		private Dictionary<int, GuardPoint> guardPointDict = new Dictionary<int, GuardPoint>();

		private Dictionary<int, GuardLevelUp> guardLevelUpDict = new Dictionary<int, GuardLevelUp>();

		private Dictionary<int, GuardSuitUp> guardSuitUpDict = new Dictionary<int, GuardSuitUp>();

		private List<Tuple<int, int>> dayMaxCanRecoverPointList = new List<Tuple<int, int>>();

		private List<Tuple<int, int>> maxActiveSlotCntList = new List<Tuple<int, int>>();

		private int GuardStatueMaxLevel = 0;

		private int GuardStatueMaxSuit = 0;

		public double LevelFactor = 1.0;

		public double SuitFactor = 1.0;
	}
}
