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

namespace GameServer.Logic
{
	public class MarryPartyLogic
	{
		public static MarryPartyLogic getInstance()
		{
			return MarryPartyLogic.Instance;
		}

		public void LoadMarryPartyConfig()
		{
			lock (this.MarryPartyConfigList)
			{
				this.MarryPartyConfigList.Clear();
				string uri = "Config/WeddingFeasttAward.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
				if (null == xelement)
				{
					return;
				}
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					MarryPartyConfigData marryPartyConfigData = new MarryPartyConfigData();
					marryPartyConfigData.PartyID = (int)Global.GetSafeAttributeLong(xml, "ID");
					marryPartyConfigData.PartyType = (int)Global.GetSafeAttributeLong(xml, "Type");
					marryPartyConfigData.PartyCost = (int)Global.GetSafeAttributeLong(xml, "ConductBindJinBi");
					marryPartyConfigData.PartyMaxJoinCount = (int)Global.GetSafeAttributeLong(xml, "SumNum");
					marryPartyConfigData.PlayerMaxJoinCount = (int)Global.GetSafeAttributeLong(xml, "UseNum");
					marryPartyConfigData.JoinCost = (int)Global.GetSafeAttributeLong(xml, "BindJinBi");
					marryPartyConfigData.RewardExp = (int)Global.GetSafeAttributeLong(xml, "EXPAward");
					marryPartyConfigData.RewardXingHun = (int)Global.GetSafeAttributeLong(xml, "XingHunAward");
					marryPartyConfigData.RewardShengWang = (int)Global.GetSafeAttributeLong(xml, "ShengWangAward");
					marryPartyConfigData.GoodWillRatio = (int)Global.GetSafeAttributeLong(xml, "GoodWillRatio");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsAward");
					string[] array = safeAttributeStr.Split(new char[]
					{
						','
					});
					if (array.Length == 7)
					{
						marryPartyConfigData.RewardItem = new AwardsItemData
						{
							Occupation = 0,
							RoleSex = 0,
							GoodsID = Convert.ToInt32(array[0]),
							GoodsNum = Convert.ToInt32(array[1]),
							Binding = Convert.ToInt32(array[2]),
							Level = Convert.ToInt32(array[3]),
							AppendLev = Convert.ToInt32(array[4]),
							IsHaveLuckyProp = Convert.ToInt32(array[5]),
							ExcellencePorpValue = Convert.ToInt32(array[6]),
							EndTime = "1900-01-01 12:00:00"
						};
					}
					this.MarryPartyConfigList.Add(marryPartyConfigData.PartyType, marryPartyConfigData);
				}
			}
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("HunYanNPC");
			string[] array2 = paramValueByName.Split(new char[]
			{
				','
			});
			if (array2.Length >= 5)
			{
				int.TryParse(array2[0], out this.MarryPartyNPCConfig.MapCode);
				int.TryParse(array2[1], out this.MarryPartyNPCConfig.NpcID);
				int.TryParse(array2[2], out this.MarryPartyNPCConfig.NpcX);
				int.TryParse(array2[3], out this.MarryPartyNPCConfig.NpcY);
				int.TryParse(array2[4], out this.MarryPartyNPCConfig.NpcDir);
			}
			this.MarryPartyPlayerMaxJoinCount = (int)GameManager.systemParamsList.GetParamValueIntByName("HunYanUseMaxNum", -1);
			this.MarryPartyJoinListResetTime = TimeUtil.NowDateTime().DayOfYear;
			this.MarryPartyQueryList();
		}

		private MarryPartyConfigData GetMarryPartyConfigData(int type)
		{
			MarryPartyConfigData result = null;
			lock (this.MarryPartyConfigList)
			{
				this.MarryPartyConfigList.TryGetValue(type, out result);
			}
			return result;
		}

		public bool MarryPartyQueryList()
		{
			byte[] array = null;
			bool result;
			if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10187, string.Format("{0}", GameManager.ServerLineID), out array, 0))
			{
				this.m_MarryPartyDataCache.MarryPartyList = new Dictionary<int, MarryPartyData>();
				result = false;
			}
			else if (array == null || array.Length <= 6)
			{
				this.m_MarryPartyDataCache.MarryPartyList = new Dictionary<int, MarryPartyData>();
				result = false;
			}
			else
			{
				int num = BitConverter.ToInt32(array, 0);
				this.m_MarryPartyDataCache.MarryPartyList = DataHelper.BytesToObject<Dictionary<int, MarryPartyData>>(array, 6, num - 2);
				result = true;
			}
			return result;
		}

		public bool MarryPartyJoinListClear(GameClient client, bool clearAll)
		{
			bool result;
			if (null == client.ClientData.MyMarryPartyJoinList)
			{
				result = false;
			}
			else
			{
				client.ClientData.MyMarryPartyJoinList.Clear();
				int num = clearAll ? 2 : 1;
				if (clearAll)
				{
					int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
					if (this.MarryPartyJoinListResetTime == dayOfYear)
					{
						num = 0;
					}
					else
					{
						this.MarryPartyJoinListResetTime = dayOfYear;
					}
				}
				byte[] array = null;
				if (TCPProcessCmdResults.RESULT_FAILED == Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10191, string.Format("{0}:{1}", client.ClientData.RoleID, num), out array, client.ServerId))
				{
					result = false;
				}
				else if (array == null || array.Length <= 6)
				{
					result = false;
				}
				else
				{
					this.SendMarryPartyJoinList(client);
					result = true;
				}
			}
			return result;
		}

		public MarryPartyResult MarryPartyCreate(GameClient client, int partyType, long startTime)
		{
			MarryPartyResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryPartyResult.NotOpen;
			}
			else
			{
				MarryPartyConfigData marryPartyConfigData = this.GetMarryPartyConfigData(partyType);
				if (null == marryPartyConfigData)
				{
					result = MarryPartyResult.InvalidParam;
				}
				else
				{
					MarriageData myMarriageData = client.ClientData.MyMarriageData;
					if (myMarriageData.nSpouseID < 0 || myMarriageData.byMarrytype < 0)
					{
						result = MarryPartyResult.NotMarry;
					}
					else
					{
						int num;
						string roleName;
						int num2;
						string roleName2;
						if (1 == myMarriageData.byMarrytype)
						{
							num = client.ClientData.RoleID;
							roleName = client.ClientData.RoleName;
							num2 = myMarriageData.nSpouseID;
							roleName2 = MarryLogic.GetRoleName(myMarriageData.nSpouseID);
						}
						else
						{
							num = myMarriageData.nSpouseID;
							roleName = MarryLogic.GetRoleName(myMarriageData.nSpouseID);
							num2 = client.ClientData.RoleID;
							roleName2 = client.ClientData.RoleName;
						}
						MarryPartyData marryPartyData = this.m_MarryPartyDataCache.AddParty(client.ClientData.RoleID, partyType, startTime, num, num2, roleName, roleName2);
						if (marryPartyData == null)
						{
							result = MarryPartyResult.AlreadyRequest;
						}
						else
						{
							MarryPartyResult marryPartyResult = MarryPartyResult.Success;
							byte[] array = null;
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10188, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
							{
								client.ClientData.RoleID,
								partyType,
								startTime,
								num,
								num2,
								roleName,
								roleName2
							}), out array, client.ServerId);
							if (TCPProcessCmdResults.RESULT_FAILED == tcpprocessCmdResults || array == null || array.Length <= 6)
							{
								marryPartyResult = MarryPartyResult.AlreadyRequest;
							}
							if (marryPartyResult == MarryPartyResult.Success)
							{
								if (marryPartyConfigData.PartyCost > Global.GetTotalBindTongQianAndTongQianVal(client))
								{
									marryPartyResult = MarryPartyResult.NotEnoughMoney;
								}
								if (marryPartyConfigData.PartyCost > 0)
								{
									if (!Global.SubBindTongQianAndTongQian(client, marryPartyConfigData.PartyCost, "举办婚宴"))
									{
										marryPartyResult = MarryPartyResult.NotEnoughMoney;
									}
								}
							}
							if (marryPartyResult != MarryPartyResult.Success)
							{
								if (tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED)
								{
									try
									{
										Global.SendAndRecvData<string>(10189, string.Format("{0}", client.ClientData.RoleID), client.ServerId, 0);
									}
									catch (Exception)
									{
									}
								}
								this.m_MarryPartyDataCache.RemoveParty(client.ClientData.RoleID);
								result = marryPartyResult;
							}
							else
							{
								int num3 = BitConverter.ToInt32(array, 0);
								MarryPartyData marryPartyData2 = DataHelper.BytesToObject<MarryPartyData>(array, 6, num3 - 2);
								this.m_MarryPartyDataCache.SetPartyTime(marryPartyData, marryPartyData2.StartTime);
								this.SendMarryPartyList(client, marryPartyData, -1);
								result = marryPartyResult;
							}
						}
					}
				}
			}
			return result;
		}

		public MarryPartyResult MarryPartyCancel(GameClient client)
		{
			MarryPartyResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryPartyResult.NotOpen;
			}
			else
			{
				MarryPartyData party = this.m_MarryPartyDataCache.GetParty(client.ClientData.MyMarriageData.nSpouseID);
				if (party != null)
				{
					result = MarryPartyResult.NotOriginator;
				}
				else
				{
					result = this.MarryPartyRemove(client.ClientData.RoleID, false, client);
				}
			}
			return result;
		}

		public MarryPartyResult MarryPartyRemove(int roleID, bool forceRemove, GameClient client)
		{
			MarryPartyData party = this.m_MarryPartyDataCache.GetParty(roleID);
			MarryPartyResult result;
			if (party == null)
			{
				result = MarryPartyResult.PartyNotFound;
			}
			else if (!forceRemove && party.StartTime <= TimeUtil.NOW())
			{
				result = MarryPartyResult.AlreadyStart;
			}
			else
			{
				MarryPartyConfigData marryPartyConfigData = this.GetMarryPartyConfigData(party.PartyType);
				if (null == marryPartyConfigData)
				{
					result = MarryPartyResult.InvalidParam;
				}
				else if (!this.MarryPartyRemoveInternal(roleID, forceRemove, client, party))
				{
					result = MarryPartyResult.PartyNotFound;
				}
				else
				{
					if (!forceRemove)
					{
						GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, marryPartyConfigData.PartyCost, "婚宴退款", false);
						this.SendMarryPartyList(client, new MarryPartyData(), -1);
					}
					else if (party.StartTime > TimeUtil.NOW())
					{
						GoodsData newGoodsData = Global.GetNewGoodsData(50014, 1);
						newGoodsData.GCount = marryPartyConfigData.PartyCost / 10000;
						Global.UseMailGivePlayerAward3(roleID, new List<GoodsData>
						{
							newGoodsData
						}, GLang.GetLang(491, new object[0]), GLang.GetLang(492, new object[0]), 0, 0, 0);
					}
					result = MarryPartyResult.Success;
				}
			}
			return result;
		}

		private bool MarryPartyRemoveInternal(int roleID, bool forceRemove, GameClient self, MarryPartyData partyData = null)
		{
			bool result;
			if (!this.m_MarryPartyDataCache.RemoveParty(roleID))
			{
				result = false;
			}
			else
			{
				byte[] array = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10189, string.Format("{0}", roleID), out array, self.ServerId);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					if (!forceRemove)
					{
						this.m_MarryPartyDataCache.RemovePartyCancel(partyData);
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public MarryPartyResult MarryPartyJoin(GameClient client, int roleID)
		{
			MarryPartyResult result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = MarryPartyResult.NotOpen;
			}
			else
			{
				MarryPartyData party = this.m_MarryPartyDataCache.GetParty(roleID);
				if (party == null)
				{
					result = MarryPartyResult.PartyNotFound;
				}
				else if (party.StartTime > TimeUtil.NOW())
				{
					result = MarryPartyResult.NotStart;
				}
				else
				{
					MarryPartyConfigData marryPartyConfigData = this.GetMarryPartyConfigData(party.PartyType);
					if (null == marryPartyConfigData)
					{
						result = MarryPartyResult.PartyNotFound;
					}
					else if (marryPartyConfigData.JoinCost > Global.GetTotalBindTongQianAndTongQianVal(client))
					{
						result = MarryPartyResult.NotEnoughMoney;
					}
					else
					{
						Dictionary<int, int> myMarryPartyJoinList = client.ClientData.MyMarryPartyJoinList;
						int num = 0;
						int num2 = 0;
						bool flag = false;
						lock (myMarryPartyJoinList)
						{
							myMarryPartyJoinList.TryGetValue(roleID, out num);
							foreach (KeyValuePair<int, int> keyValuePair in client.ClientData.MyMarryPartyJoinList)
							{
								num2 += keyValuePair.Value;
							}
							if (num2 >= this.MarryPartyPlayerMaxJoinCount)
							{
								return MarryPartyResult.ZeroPlayerJoinCount;
							}
							if (num >= marryPartyConfigData.PlayerMaxJoinCount)
							{
								return MarryPartyResult.ZeroPlayerJoinCount;
							}
							if (!this.m_MarryPartyDataCache.IncPartyJoin(roleID, marryPartyConfigData.PartyMaxJoinCount, out flag))
							{
								return MarryPartyResult.ZeroPartyJoinCount;
							}
							num++;
							byte[] array = null;
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer3(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10190, string.Format("{0}:{1}:{2}", roleID, client.ClientData.RoleID, num), out array, client.ServerId);
							if (TCPProcessCmdResults.RESULT_FAILED == tcpprocessCmdResults || array == null || array.Length <= 6)
							{
								this.m_MarryPartyDataCache.IncPartyJoinCancel(roleID);
								return MarryPartyResult.ZeroPartyJoinCount;
							}
							myMarryPartyJoinList[roleID] = num;
						}
						if (marryPartyConfigData.JoinCost > 0)
						{
							if (!Global.SubBindTongQianAndTongQian(client, marryPartyConfigData.JoinCost, "參予婚宴"))
							{
								return MarryPartyResult.NotEnoughMoney;
							}
						}
						if (marryPartyConfigData.RewardExp > 0)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, (long)marryPartyConfigData.RewardExp, false, true, false, "none");
						}
						if (marryPartyConfigData.RewardShengWang > 0)
						{
							GameManager.ClientMgr.ModifyShengWangValue(client, marryPartyConfigData.RewardShengWang, "婚宴奖励", false, true);
						}
						if (marryPartyConfigData.RewardXingHun > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, marryPartyConfigData.RewardXingHun, "婚宴奖励", false, true);
						}
						if (flag)
						{
							this.MarryPartyRemoveInternal(roleID, true, client, null);
							GoodsData newGoodsData = Global.GetNewGoodsData(marryPartyConfigData.RewardItem.GoodsID, marryPartyConfigData.RewardItem.Binding);
							newGoodsData.GCount = marryPartyConfigData.JoinCost * marryPartyConfigData.PartyMaxJoinCount / marryPartyConfigData.GoodWillRatio / 2;
							List<GoodsData> list = new List<GoodsData>();
							list.Add(newGoodsData);
							string lang = GLang.GetLang(493, new object[0]);
							Global.UseMailGivePlayerAward3(roleID, list, GLang.GetLang(494, new object[0]), lang, 0, 0, 0);
							int nRoleID = (roleID == party.HusbandRoleID) ? party.WifeRoleID : party.HusbandRoleID;
							Global.UseMailGivePlayerAward3(nRoleID, list, GLang.GetLang(494, new object[0]), lang, 0, 0, 0);
						}
						this.SendMarryPartyJoinList(client);
						this.SendMarryPartyList(client, party, -1);
						result = MarryPartyResult.Success;
					}
				}
			}
			return result;
		}

		public void MarryPartyPeriodicUpdate(long ticks)
		{
			if (ticks >= this.NextUpdateTime)
			{
				this.NextUpdateTime = ticks + 10000L;
				bool flag = this.m_MarryPartyDataCache.HasPartyStarted(ticks);
				if (flag != this.MarryPartyNPCShow)
				{
					this.MarryPartyNPCShow = flag;
					if (flag)
					{
						GameMap gameMap = GameManager.MapMgr.DictMaps[this.MarryPartyNPCConfig.MapCode];
						NPC npcfromConfig = NPCGeneralManager.GetNPCFromConfig(this.MarryPartyNPCConfig.MapCode, this.MarryPartyNPCConfig.NpcID, this.MarryPartyNPCConfig.NpcX, this.MarryPartyNPCConfig.NpcY, this.MarryPartyNPCConfig.NpcDir);
						if (null != npcfromConfig)
						{
							if (NPCGeneralManager.AddNpcToMap(npcfromConfig))
							{
								this.MarryPartyNpc = npcfromConfig;
							}
							else
							{
								LogManager.WriteLog(2, string.Format("add marry party npc failure, MapCode={0}, NpcID={1}", this.MarryPartyNPCConfig.MapCode, this.MarryPartyNPCConfig.NpcID), null, true);
							}
						}
					}
					else if (null != this.MarryPartyNpc)
					{
						NPCGeneralManager.RemoveMapNpc(this.MarryPartyNPCConfig.MapCode, this.MarryPartyNPCConfig.NpcID);
						this.MarryPartyNpc = null;
					}
				}
			}
		}

		public void SendMarryPartyList(GameClient client, MarryPartyData partyData, int roleID = -1)
		{
			if (partyData != null || roleID > 0)
			{
				if (partyData == null)
				{
					partyData = this.m_MarryPartyDataCache.GetParty(roleID);
					if (partyData == null)
					{
						int spouseID = MarryLogic.GetSpouseID(roleID);
						partyData = this.m_MarryPartyDataCache.GetParty(spouseID);
						if (partyData == null)
						{
							partyData = new MarryPartyData();
						}
					}
				}
				client.sendCmd<Dictionary<int, MarryPartyData>>(880, new Dictionary<int, MarryPartyData>
				{
					{
						roleID,
						partyData
					}
				}, false);
			}
			else
			{
				client.sendCmd(this.m_MarryPartyDataCache.GetPartyList(TCPOutPacketPool.getInstance(), 880), true);
			}
		}

		public void SendMarryPartyJoinList(GameClient client)
		{
			if (null != client.ClientData.MyMarryPartyJoinList)
			{
				client.sendCmd<Dictionary<int, int>>(884, client.ClientData.MyMarryPartyJoinList, false);
			}
		}

		public TCPProcessCmdResults ProcessMarryPartyQuery(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
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
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
				return TCPProcessCmdResults.RESULT_DATA;
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
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (null == gameClient)
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientSocket.IsKuaFuLogin)
				{
					gameClient.sendCmd<Dictionary<int, MarryPartyData>>(880, new Dictionary<int, MarryPartyData>(), false);
				}
				else
				{
					this.SendMarryPartyList(gameClient, null, roleID);
				}
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 30767);
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public TCPProcessCmdResults ProcessMarryPartyCreate(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
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
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int partyType = Convert.ToInt32(array[1]);
				long startTime = Convert.ToInt64(array[2]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientSocket.IsKuaFuLogin)
				{
					gameClient.sendCmd(nID, string.Format("{0}:{1}", -12, num), false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				MarryPartyResult marryPartyResult = this.MarryPartyCreate(gameClient, partyType, startTime);
				string data2 = string.Format("{0}:{1}", (int)marryPartyResult, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessMarryPartyCreate", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessMarryPartyCancel(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
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
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (gameClient.ClientSocket.IsKuaFuLogin)
				{
					gameClient.sendCmd(nID, string.Format("{0}:{1}", -12, num), false);
					return TCPProcessCmdResults.RESULT_OK;
				}
				MarryPartyResult marryPartyResult = this.MarryPartyCancel(gameClient);
				string data2 = string.Format("{0}:{1}", (int)marryPartyResult, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessMarryPartyCancel", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public TCPProcessCmdResults ProcessMarryPartyJoin(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int roleID = Convert.ToInt32(array[1]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				MarryPartyResult marryPartyResult = this.MarryPartyJoin(client, roleID);
				string data2 = string.Format("{0}:{1}", (int)marryPartyResult, num);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessJoinQingGongYanCMD", false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			this.m_MarryPartyDataCache.OnChangeName(roleId, oldName, newName);
		}

		private MarryPartyDataCache m_MarryPartyDataCache = new MarryPartyDataCache();

		private Dictionary<int, MarryPartyConfigData> MarryPartyConfigList = new Dictionary<int, MarryPartyConfigData>();

		private MarryPartyNPCData MarryPartyNPCConfig = new MarryPartyNPCData();

		private int MarryPartyPlayerMaxJoinCount;

		private int MarryPartyJoinListResetTime = 0;

		private bool MarryPartyNPCShow = false;

		private NPC MarryPartyNpc = null;

		private static MarryPartyLogic Instance = new MarryPartyLogic();

		private long NextUpdateTime = 0L;
	}
}
