using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using ComponentAce.Compression.Libs.zlib;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using ProtoBuf;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	internal class RobotTaskValidator
	{
		private RobotTaskValidator()
		{
		}

		public static RobotTaskValidator getInstance()
		{
			return RobotTaskValidator.instance;
		}

		public bool UseWorkThread
		{
			get
			{
				return this._useWorkThread;
			}
			set
			{
				this._useWorkThread = value;
			}
		}

		public bool Initialize(bool client, int seed, int randomCount, string pubKey)
		{
			try
			{
				if (null == this.BackgroundThread)
				{
					this.BackgroundThread = new Thread(new ThreadStart(this.TimerProc));
					this.BackgroundThread.IsBackground = true;
					this.BackgroundThread.Start();
				}
				if (!client)
				{
					this.m_TaskListVerifySeed = Environment.TickCount;
					this.m_TaskListRSA = new RSACryptoServiceProvider(1024);
					this.m_TaskListRSA.PersistKeyInCsp = false;
					this.m_TaskListRSAPubKey = this.m_TaskListRSA.ToXmlString(false);
					ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("RobotTaskValidator.FlushTaskListToFile()", new EventHandler(this.FlushTaskListToFile)), 1800000, 1800000);
					return this.LoadRobotTaskData();
				}
				this.m_TaskListVerifySeed = seed;
				this.m_TaskListVerifyRandomCount = randomCount;
				this.m_TaskListRSA = new RSACryptoServiceProvider();
				this.m_TaskListRSA.PersistKeyInCsp = false;
				this.m_TaskListRSA.FromXmlString(pubKey);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(string.Format("rsa create failure: {0}", ex.ToString()));
				return false;
			}
			return true;
		}

		private uint GenMagicRandom(uint seed, int loop)
		{
			uint num = seed;
			uint num2 = 362436069U;
			for (int i = 0; i <= loop; i++)
			{
				num2 = 36969U * (num2 & 65535U) + (num2 >> 16);
				num = 18000U * (num & 65535U) + (num >> 16);
			}
			return (num2 << 16) + num;
		}

		private void AddTaskToHashSet(string taskId)
		{
			using (this.m_Mutex.Enter(4))
			{
				if (this.m_AllTaskHashSet.Add(taskId))
				{
					if (this.m_AllTaskHashSet.Count >= 3000)
					{
						this.FlushTaskListToFile(null, null);
					}
				}
			}
		}

		public void FlushTaskListToFile(object sender, EventArgs e)
		{
			try
			{
				StringBuilder stringBuilder = new StringBuilder("-------------------------\r\n");
				using (this.m_Mutex.Enter(5))
				{
					try
					{
						foreach (string value in this.m_AllTaskHashSet)
						{
							stringBuilder.AppendLine(value);
						}
					}
					finally
					{
						this.m_AllTaskHashSet.Clear();
					}
				}
				LogManager.WriteLog(11, stringBuilder.ToString(), null, true);
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public void SendTaskListKey(GameClient client)
		{
			if (!client.CheckCheatData.RobotTaskCheckInitialed)
			{
				if (!client.ClientSocket.session.IsGM)
				{
					if (this.BanIsOpen())
					{
						this.UpdateTaskListTimeout(client);
						string cmdData = string.Format("{0}:{1}:{2}", this.m_TaskListVerifySeed, this.m_TaskListVerifyRandomCount, this.m_TaskListRSAPubKey);
						client.sendCmd(30000, cmdData, false);
						client.sendCmd<RobotTaskValidator.TaskCheckList>(699, this.m_TaskCheckList, false);
						client.CheckCheatData.RobotTaskCheckInitialed = true;
					}
				}
			}
		}

		public bool BanIsOpen()
		{
			return this._BanIsOpen;
		}

		public bool YueYuIsOpen()
		{
			using (this.m_Mutex.Enter(6))
			{
				if (this._yyOpenPlatform != null && this._yyOpenPlatform.ContainsKey(this._platformType) && this._yyOpenPlatform[this._platformType] > 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsYueYu(GameClient client, string appStr)
		{
			using (this.m_Mutex.Enter(7))
			{
				if (!client.IsYueYu || !this.BanIsOpen() || !this.YueYuIsOpen())
				{
					return false;
				}
				if (this._yueyuAppDic == null || this._yueyuAppDic.Count <= 0 || this._yueyuAppDic.ContainsKey("null"))
				{
					return false;
				}
				if (this._yueyuAppDic.ContainsKey("*") || this._yueyuAppDic.ContainsKey(appStr))
				{
					return true;
				}
			}
			return false;
		}

		public bool BanYueYu(GameClient client, out bool isLog, string clientData = "0", int jailbreak = 1, int autoStart = 0, string taskStr = "*", int banCount = 0)
		{
			isLog = false;
			switch (this._banReasonDic[BanReasonType.YueYu])
			{
			case 1:
				isLog = true;
				return this.BanLog(client, BanType.BanLog, clientData, jailbreak, autoStart, taskStr, banCount, false);
			case 2:
				return this.BanKick(client, BanType.BanKick, clientData, jailbreak, autoStart, taskStr, banCount, true);
			case 3:
				return this.BanClose(client, BanType.BanClose, clientData, jailbreak, autoStart, taskStr, banCount, true);
			case 6:
				return this.BanRate(client, BanType.BanRate, clientData, jailbreak, autoStart, taskStr, banCount, true);
			}
			return false;
		}

		public bool LoadRobotTaskData()
		{
			this._platformType = GameCoreInterface.getinstance().GetPlatformType();
			this._BanIsOpen = false;
			string text = "";
			using (this.m_Mutex.Enter(8))
			{
				try
				{
					text = Global.GameResPath("Config/TaskCheckList.xml");
					if (File.Exists(text))
					{
						this.m_TaskCheckList.TaskCheckListData = File.ReadAllBytes(text);
					}
					text = Global.GameResPath("Config/AssInfo.xml");
					XElement xelement = CheckHelper.LoadXml(text, false);
					if (null == xelement)
					{
						return false;
					}
					IEnumerable<XElement> enumerable = xelement.Elements();
					this.m_RobotBuildList.Clear();
					this.m_RobotCPUList.Clear();
					this.m_RobotPhoneList.Clear();
					foreach (XElement xelement2 in enumerable)
					{
						string text2 = Global.GetSafeAttributeStr(xelement2, "ID");
						text2 = text2.ToLower();
						if (xelement2.Name.LocalName == "RobotBuild")
						{
							if (!this.m_RobotBuildList.ContainsKey(text2))
							{
								this.m_RobotBuildList.Add(text2, 0);
							}
						}
						else if (xelement2.Name.LocalName == "RobotCPU")
						{
							if (!this.m_RobotCPUList.ContainsKey(text2))
							{
								this.m_RobotCPUList.Add(text2, 0);
							}
						}
						else if (xelement2.Name.LocalName == "RobotPhone")
						{
							if (!this.m_RobotPhoneList.ContainsKey(text2))
							{
								this.m_RobotPhoneList.Add(text2, 0);
							}
						}
						else if (xelement2.Name.LocalName == "RobotSign")
						{
							if (!this.m_RobotSignList.ContainsKey(text2))
							{
								this.m_RobotSignList.Add(text2, 0);
							}
						}
						else if (xelement2.Name.LocalName == "YueYuDev")
						{
							this.m_YueYuDevList[text2] = 0;
						}
						else if (xelement2.Name.LocalName == "YueYuOSVer")
						{
							this.m_YueYuOSVerList[text2] = 0;
						}
						else if (string.Compare(xelement2.Name.LocalName, "NoYueYuDev", true) == 0)
						{
							string item = Global.GetSafeAttributeStr(xelement2, "OS").ToLower();
							List<string> list;
							if (!this.m_NewYueYuDevOSVerListDict.TryGetValue(text2, out list))
							{
								list = new List<string>();
								this.m_NewYueYuDevOSVerListDict[text2] = list;
							}
							list.Add(item);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
			}
			using (this.m_Mutex.Enter(9))
			{
				try
				{
					text = Global.GameResPath("Config/AssList.xml");
					XElement xelement = CheckHelper.LoadXml(text, false);
					if (null == xelement)
					{
						return false;
					}
					IEnumerable<XElement> enumerable = xelement.Elements();
					this.m_RobotTaskList.Clear();
					foreach (XElement xelement2 in enumerable)
					{
						string text2 = Global.GetSafeAttributeStr(xelement2, "ID");
						int level = int.Parse(Global.GetSafeAttributeStr(xelement2, "Level"));
						int banType = int.Parse(Global.GetSafeAttributeStr(xelement2, "BanType"));
						if (!this.m_RobotTaskList.ContainsKey(text2))
						{
							BanInfo banInfo = new BanInfo();
							banInfo.BanID = text2;
							banInfo.Level = level;
							banInfo.BanType = (BanType)banType;
							this.m_RobotTaskList.Add(text2, banInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
			}
			using (this.m_Mutex.Enter(10))
			{
				try
				{
					text = Global.GameResPath("Config/AssConfig.xml");
					XElement xelement = CheckHelper.LoadXml(text, false);
					if (null == xelement)
					{
						return false;
					}
					this._useWorkThread = (ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "UseWorkThread", "Value", 0L) > 0L);
					int[] elementAttributeValueIntArray = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "BanOpenPlayform", "Value", null);
					this._banOpenPlatform.Clear();
					if (elementAttributeValueIntArray != null && elementAttributeValueIntArray.Length > 1)
					{
						for (int i = 0; i < elementAttributeValueIntArray.Length; i++)
						{
							PlatformTypes key = elementAttributeValueIntArray[i++];
							int value = elementAttributeValueIntArray[i];
							if (!this._banOpenPlatform.ContainsKey(key))
							{
								this._banOpenPlatform.Add(key, value);
							}
						}
					}
					int[] elementAttributeValueIntArray2 = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "YueYuPlayform", "Value", null);
					this._yyOpenPlatform.Clear();
					if (elementAttributeValueIntArray2 != null && elementAttributeValueIntArray2.Length > 1)
					{
						for (int i = 0; i < elementAttributeValueIntArray2.Length; i++)
						{
							PlatformTypes key = elementAttributeValueIntArray2[i++];
							int value = elementAttributeValueIntArray2[i];
							if (!this._yyOpenPlatform.ContainsKey(key))
							{
								this._yyOpenPlatform.Add(key, value);
							}
						}
					}
					this._jiaoBenIsOpen = (ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "JiaoBenIsOpen", "Value", 0L) > 0L);
					this._vmIsOpen = (ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "VmIsOpen", "Value", 0L) > 0L);
					this._vmSignGameOpenSeven = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "VmSignGameOpenSeveb", "Value", new int[]
					{
						0,
						7,
						210
					});
					this._yueYuGameOpenSeveb = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "YueYuGameOpenSeveb", "Value", new int[]
					{
						0,
						7,
						210
					});
					int[] elementAttributeValueIntArray3 = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "MapCode", "Value", new int[0]);
					foreach (int item2 in elementAttributeValueIntArray3)
					{
						this.VMBanMapCodes.Add(item2);
					}
					this._timeOutCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "TimeOutCount", "Value", 5L);
					this._timeOutMinute = ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "TimeOutMinute", "Value", 5L) * 60L * 1000L;
					this._banCheckMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "BanCheckMaxCount", "Value", 10L);
					this._kickWarnMaxCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "KickWarnMaxCount", "Value", 2L);
					this._banLevelLimit = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "BanLevelLimit", "Value", 401L);
					this._banVipLimit = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "BanVipLimit", "Value", 13L);
					XElement xelement3 = xelement;
					string text3 = "Param";
					string text4 = "Name";
					string text5 = "RobotBanMinutes";
					string text6 = "Value";
					int[] array2 = new int[1];
					this.m_BanMiuteList = ConfigHelper.GetElementAttributeValueIntArray(xelement3, text3, text4, text5, text6, array2);
					this.m_BanMinutes = this.m_BanMiuteList[0];
					this._logCountDic.Clear();
					int[] elementAttributeValueIntArray4 = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "BanLogCountLimit", "Value", null);
					if (elementAttributeValueIntArray4 != null && elementAttributeValueIntArray4.Length > 1)
					{
						for (int k = 0; k < elementAttributeValueIntArray4.Length; k++)
						{
							this._logCountDic.Add(elementAttributeValueIntArray4[k], elementAttributeValueIntArray4[++k]);
						}
					}
					this._banReasonDic.Clear();
					int[] elementAttributeValueIntArray5 = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "BanReason", "Value", null);
					if (elementAttributeValueIntArray5 != null && elementAttributeValueIntArray5.Length > 1)
					{
						for (int k = 0; k < elementAttributeValueIntArray5.Length; k++)
						{
							this._banReasonDic.Add((BanReasonType)elementAttributeValueIntArray5[k], elementAttributeValueIntArray5[++k]);
						}
					}
					string[] elementAttributeValueStrArray = ConfigHelper.GetElementAttributeValueStrArray(xelement, "Param", "Name", "CancelApp", "Value", null);
					this._cancelAppDic.Clear();
					if (elementAttributeValueStrArray != null && elementAttributeValueStrArray.Length > 0)
					{
						foreach (string key2 in elementAttributeValueStrArray)
						{
							this._cancelAppDic.Add(key2, 0);
						}
					}
					string[] elementAttributeValueStrArray2 = ConfigHelper.GetElementAttributeValueStrArray(xelement, "Param", "Name", "YueYuApp", "Value", null);
					this._yueyuAppDic.Clear();
					if (elementAttributeValueStrArray2 != null && elementAttributeValueStrArray2.Length > 0)
					{
						foreach (string key2 in elementAttributeValueStrArray2)
						{
							this._yueyuAppDic.Add(key2, 0);
						}
					}
					this._specialAppList = ConfigHelper.GetElementAttributeValueStrArray(xelement, "Param", "Name", "SpecialApp", "Value", new string[0]);
					this._appCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "AppCount", "Value", 15L);
					this._multiCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "MultiCount", "Value", 2L);
					this._decryptCount = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "DecryptCount", "Value", 5L);
					this._banRateNum = (ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "BanRateNum", "Value", 0L) > 0L);
					this._canLogIp = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "Param", "Name", "CanLogIp", "Value", 0L);
					this._appPlatformCount = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "AppPlayformCount", "Value", new int[]
					{
						0,
						1,
						5
					});
					this.m_DropRateDownMonsterTypeHashSet.Clear();
					int[] elementAttributeValueIntArray6 = ConfigHelper.GetElementAttributeValueIntArray(xelement, "Param", "Name", "DropRateDownMonsterType", "Value", null);
					if (elementAttributeValueIntArray6 != null && elementAttributeValueIntArray6.Length > 0)
					{
						foreach (int key3 in elementAttributeValueIntArray6)
						{
							this.m_DropRateDownMonsterTypeHashSet.TryAdd(key3, 0);
						}
					}
					this._DropRateDownMapDict.Clear();
					string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement, "Param", "Name", "DropRateDownMap", "Value", "");
					string[] array4 = elementAttributeValue.Split(new char[]
					{
						'|'
					});
					for (int k = 0; k < array4.Length; k++)
					{
						string[] array5 = array4[k].Split(new char[]
						{
							','
						});
						if (array5.Length == 2)
						{
							int num = Convert.ToInt32(array5[0]);
							if (num > 0)
							{
								double value2 = Convert.ToDouble(array5[1]);
								this._DropRateDownMapDict.TryAdd(num, value2);
							}
						}
					}
					bool flag = false;
					bool flag2 = false;
					if (this._banOpenPlatform != null && this._banOpenPlatform.ContainsKey(this._platformType) && this._banOpenPlatform[this._platformType] > 0)
					{
						flag = true;
					}
					if (this._jiaoBenIsOpen || this._vmIsOpen)
					{
						flag2 = true;
					}
					this._BanIsOpen = (flag2 && flag);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return true;
		}

		public void UpdateTaskListTimeout(GameClient client)
		{
			if (this.BanIsOpen())
			{
				client.CheckCheatData.NextTaskListTimeout = TimeUtil.NOW() + this._timeOutMinute;
			}
		}

		public bool KickTimeout(GameClient client)
		{
			bool result;
			if (!this.BanIsOpen())
			{
				result = false;
			}
			else
			{
				if (client.CheckCheatData.NextTaskListTimeout > 0L)
				{
					long num = TimeUtil.NOW();
					if (num > client.CheckCheatData.NextTaskListTimeout)
					{
						if (client.ClientSocket.session.TimeOutCount >= this._timeOutCount)
						{
							using (this.m_Mutex.Enter(11))
							{
								switch (this._banReasonDic[BanReasonType.TimeOut])
								{
								case 81:
									this.BanKick(client, BanType.BanKickTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, false);
									break;
								case 82:
									this.BanClose(client, BanType.BanCloseTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, false);
									break;
								case 83:
									this.BanRate(client, BanType.BanRateTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, false);
									break;
								}
								this.UpdateTaskListTimeout(client);
								return false;
							}
						}
						int cmdID = client.ClientSocket.session.CmdID;
						long cmdTime = client.ClientSocket.session.CmdTime;
						if (cmdID > 0 && cmdTime + this._timeOutMinute > num)
						{
							if (this._platformType == 1 || this._platformType == 2)
							{
								if (string.IsNullOrEmpty(client.deviceID))
								{
									switch (this._banReasonDic[BanReasonType.TimeOutDeviceNull])
									{
									case 41:
										this.BanKick(client, BanType.BanKickDeviceNull, client.CheckCheatData.RobotTaskListData, 0, 0, "", -1, false);
										break;
									case 42:
										this.BanClose(client, BanType.BanCloseDeviceNull, client.CheckCheatData.RobotTaskListData, 0, 0, "", -1, false);
										break;
									case 43:
										this.BanRate(client, BanType.BanRateDeviceNull, client.CheckCheatData.RobotTaskListData, 0, 0, "", -1, false);
										break;
									}
								}
							}
							client.ClientSocket.session.TimeOutCountAdd();
							this.BanLog(client, BanType.BanLogTimeOut, client.CheckCheatData.RobotTaskListData, 0, 0, client.ClientSocket.session.TimeOutCount.ToString(), -1, true);
							this.UpdateTaskListTimeout(client);
							return false;
						}
					}
					if (client.CheckCheatData.RobotDetectedKickTime > 0L && num > client.CheckCheatData.RobotDetectedKickTime)
					{
						Global.ForceCloseClient(client, client.CheckCheatData.RobotDetectedReason, true);
						this.RobotDataReset(client);
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public TCPProcessCmdResults ProcessTaskList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				if (!this.BanIsOpen())
				{
					return TCPProcessCmdResults.RESULT_OK;
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (gameClient != null)
				{
					if (this._useWorkThread)
					{
						byte[] array = new byte[count];
						Array.Copy(data, array, count);
						this.AddRobotDataItem(nID, gameClient, array);
					}
					else
					{
						this.ValidateTaskList(data, gameClient);
						this.UpdateTaskListTimeout(gameClient);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		private void AddRobotDataItem(int cmdId, GameClient client, byte[] data)
		{
			RobotDataItem item = new RobotDataItem
			{
				CmdId = cmdId,
				Client = client,
				Data = data
			};
			lock (this.RobotDataItemQueue)
			{
				this.RobotDataItemQueue.Enqueue(item);
			}
		}

		public void TimerProc()
		{
			for (;;)
			{
				try
				{
					Thread.Sleep(60000);
					RobotDataItem[] array = null;
					lock (this.RobotDataItemQueue)
					{
						if (this.RobotDataItemQueue.Count <= 0)
						{
							continue;
						}
						array = this.RobotDataItemQueue.ToArray();
						this.RobotDataItemQueue.Clear();
					}
					if (array != null && array.Length > 0)
					{
						foreach (RobotDataItem robotDataItem in array)
						{
							try
							{
								if (robotDataItem != null && robotDataItem.Client != null && robotDataItem.Data != null)
								{
									this.ValidateTaskList(robotDataItem.Data, robotDataItem.Client);
									this.UpdateTaskListTimeout(robotDataItem.Client);
								}
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
				}
			}
		}

		public int BanYueYu(GameClient client, string devInfo)
		{
			int result;
			if (this._platformType != 1)
			{
				result = 0;
			}
			else if (string.IsNullOrEmpty(client.deviceModel) || string.IsNullOrEmpty(client.deviceOSVersion))
			{
				result = 0;
			}
			else
			{
				using (this.m_Mutex.Enter(16))
				{
					try
					{
						if (this._yueYuGameOpenSeveb[0] == 0)
						{
							return 0;
						}
						bool flag = false;
						if (client.IsYueYu)
						{
							foreach (KeyValuePair<string, int> keyValuePair in this.m_YueYuDevList)
							{
								string text = client.deviceModel.ToLower();
								if (text == keyValuePair.Key)
								{
									foreach (string text2 in this.m_YueYuOSVerList.Keys)
									{
										string text3 = client.deviceOSVersion.ToLower();
										if (text3.StartsWith(text2))
										{
											flag = true;
											break;
										}
									}
									break;
								}
							}
						}
						else
						{
							string text = client.deviceModel.ToLower();
							List<string> list;
							if (this.m_NewYueYuDevOSVerListDict.TryGetValue(text, out list))
							{
								string text2 = client.deviceOSVersion.ToLower();
								foreach (string text3 in list)
								{
									if (text3 == text2)
									{
										flag = true;
										break;
									}
								}
							}
						}
						if (flag)
						{
							if (this.VMBanMapCodes.Contains(client.ClientData.MapCode) || Global.GetKaiFuTime().AddDays((double)this._yueYuGameOpenSeveb[1]) > DateTime.Now)
							{
								switch (this._yueYuGameOpenSeveb[2])
								{
								case 1:
									this.BanLog(client, BanType.BanLog, devInfo, 0, 0, "", -1, false);
									break;
								case 2:
									this.BanKick(client, BanType.BanKick, devInfo, 0, 0, "", -1, false);
									break;
								case 3:
									this.BanClose(client, BanType.BanClose, devInfo, 0, 0, "", -1, false);
									break;
								case 6:
									this.BanRate(client, BanType.BanRate, devInfo, 0, 0, "", -1, false);
									break;
								}
							}
							return 0;
						}
					}
					catch
					{
					}
				}
				result = 0;
			}
			return result;
		}

		public bool ValidateTaskList(byte[] data, GameClient client)
		{
			bool result;
			if (client.CheckCheatData.BanCheckMaxCount >= this._banCheckMaxCount)
			{
				result = true;
			}
			else
			{
				bool flag = false;
				string text = "";
				List<byte> list = new List<byte>();
				try
				{
					using (this.m_Mutex.Enter(12))
					{
						List<byte> list2 = new List<byte>(data);
						int i = 0;
						while (i < data.Length)
						{
							byte b = list2[i];
							byte[] array = new byte[(int)b];
							list2.CopyTo(i + 1, array, 0, (int)b);
							i++;
							i += (int)b;
							byte[] array2 = this.m_TaskListRSA.Decrypt(array, false);
							if (list == null)
							{
								text += new UTF8Encoding().GetString(array2, 0, array2.Length);
							}
							else
							{
								for (int j = 0; j < array2.Length; j++)
								{
									list.Add(array2[j]);
								}
							}
						}
						if (list != null)
						{
							this.m_Mutex.SetTag(13);
							byte[] array3;
							using (MemoryStream memoryStream = new MemoryStream())
							{
								using (ZOutputStream zoutputStream = new ZOutputStream(memoryStream))
								{
									zoutputStream.Write(list.ToArray(), 0, list.Count);
									zoutputStream.Flush();
								}
								array3 = memoryStream.ToArray();
							}
							text = new UTF8Encoding().GetString(array3, 0, array3.Length);
						}
					}
				}
				catch (Exception ex)
				{
					if (text == null)
					{
						text = "";
					}
					if (client.ClientSocket.session.DecryptCount >= this._decryptCount)
					{
						using (this.m_Mutex.Enter(14))
						{
							switch (this._banReasonDic[BanReasonType.Decrypt])
							{
							case 91:
								this.BanKick(client, BanType.BanKickDecrypt, text, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, false);
								break;
							case 92:
								this.BanClose(client, BanType.BanCloseDecrypt, text, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, false);
								break;
							case 93:
								this.BanRate(client, BanType.BanRateDecrypt, text, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, false);
								break;
							}
							return false;
						}
					}
					client.ClientSocket.session.DecryptCountAdd();
					this.BanLog(client, BanType.BanLogDecrypt, text, 0, 0, client.ClientSocket.session.DecryptCount.ToString(), -1, true);
					return false;
				}
				string[] array4 = text.Split(new char[]
				{
					':'
				});
				if (array4.Length < 5)
				{
					result = this.BanLog(client, BanType.BanLogInvalid, text, 0, 0, "data_length", -1, false);
				}
				else
				{
					int num = 0;
					int num2 = 0;
					int jailbreak = 0;
					int autoStart = 0;
					try
					{
						num = Convert.ToInt32(array4[1]);
						num2 = Convert.ToInt32(array4[2]);
						jailbreak = Convert.ToInt32(array4[3]);
						autoStart = Convert.ToInt32(array4[4]);
					}
					catch (Exception ex)
					{
						return this.BanLog(client, BanType.BanLogInvalid, text, jailbreak, autoStart, "data_type", -1, false);
					}
					int value = num % this.m_TaskListVerifyRandomCount;
					int num3 = (int)this.GenMagicRandom((uint)this.m_TaskListVerifySeed, Math.Abs(value));
					if (num3 != num2)
					{
						result = this.BanLog(client, BanType.BanLogInvalid, text, jailbreak, autoStart, "random", -1, false);
					}
					else
					{
						client.CheckCheatData.RobotTaskListData = text;
						string[] array5 = array4[0].Split(new char[]
						{
							'*'
						});
						int num4 = 0;
						using (this.m_Mutex.Enter(15))
						{
							int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
							if (this._jiaoBenIsOpen)
							{
								num4 = array5.Length;
								if (num4 == 1 && array4[0].Trim() == "")
								{
									num4 = 0;
								}
								this.m_Mutex.SetTag(16);
								if ((this._platformType == 1 || this._platformType == 2) && !string.IsNullOrEmpty(text) && text.IndexOf("MODEL|") > 0 && text.IndexOf("MANUFACTURER|") > 0 && text.IndexOf("PRODUCT|") > 0)
								{
									switch (this._banReasonDic[BanReasonType.AppVM])
									{
									case 20:
										flag = true;
										this.BanLog(client, BanType.BanLogAppVM, text, jailbreak, autoStart, array5.Length.ToString(), roleParamsInt32FromDB, false);
										break;
									case 21:
										this.BanKick(client, BanType.BanKickAppVM, text, jailbreak, autoStart, array5.Length.ToString(), roleParamsInt32FromDB, false);
										break;
									case 22:
										this.BanClose(client, BanType.BanCloseAppVM, text, jailbreak, autoStart, array5.Length.ToString(), roleParamsInt32FromDB, false);
										break;
									case 23:
										this.BanRate(client, BanType.BanRateAppVM, text, jailbreak, autoStart, array5.Length.ToString(), roleParamsInt32FromDB, false);
										break;
									}
								}
								if (num4 <= this._appCount)
								{
									this.m_Mutex.SetTag(17);
									switch (this._banReasonDic[BanReasonType.AppCount])
									{
									case 60:
										flag = true;
										this.BanLog(client, BanType.BanLogAppCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
										break;
									case 61:
										return this.BanKick(client, BanType.BanKickAppCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
									case 62:
										return this.BanClose(client, BanType.BanCloseAppCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
									case 63:
										return this.BanRate(client, BanType.BanRateAppCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
									}
								}
								if (this._platformType == 1 && this._appPlatformCount != null && this._appPlatformCount.Length == 3 && this._appPlatformCount[0] > 0 && num4 >= this._appPlatformCount[1] && num4 <= this._appPlatformCount[2])
								{
									this.m_Mutex.SetTag(18);
									switch (this._banReasonDic[BanReasonType.AppPlatformCount])
									{
									case 30:
										flag = true;
										this.BanLog(client, BanType.BanLogAppPlatformCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
										break;
									case 31:
										return this.BanKick(client, BanType.BanKickAppPlatformCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
									case 32:
										return this.BanClose(client, BanType.BanCloseAppPlatformCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
									case 33:
										return this.BanRate(client, BanType.BanRateAppPlatformCount, text, jailbreak, autoStart, num4.ToString(), roleParamsInt32FromDB, false);
									}
								}
								Dictionary<string, int> dictionary = new Dictionary<string, int>();
								for (int j = 0; j < num4; j++)
								{
									this.AddTaskToHashSet(array5[j]);
									string taskName = this.GetTaskName(array5[j]);
									if (!this._cancelAppDic.ContainsKey(taskName))
									{
										if (dictionary.ContainsKey(taskName))
										{
											Dictionary<string, int> dictionary2;
											string key;
											(dictionary2 = dictionary)[key = taskName] = dictionary2[key] + 1;
										}
										else
										{
											dictionary.Add(taskName, 1);
										}
									}
								}
								this.m_Mutex.SetTag(19);
								int num5 = 0;
								StringBuilder stringBuilder = new StringBuilder();
								foreach (KeyValuePair<string, int> keyValuePair in dictionary)
								{
									if (keyValuePair.Value >= this._multiCount)
									{
										stringBuilder.Append(keyValuePair.Key).Append("*").Append(keyValuePair.Value).Append("|");
										if (num5 < keyValuePair.Value)
										{
											num5 = keyValuePair.Value;
										}
									}
								}
								if (num5 >= this._multiCount)
								{
									this.m_Mutex.SetTag(20);
									switch (this._banReasonDic[BanReasonType.MultiApp])
									{
									case 70:
										flag = true;
										this.BanLog(client, BanType.BanLogMulti, text, jailbreak, autoStart, stringBuilder.ToString(), roleParamsInt32FromDB, false);
										break;
									case 71:
										return this.BanKick(client, BanType.BanKickMulti, text, jailbreak, autoStart, stringBuilder.ToString(), roleParamsInt32FromDB, false);
									case 72:
										return this.BanClose(client, BanType.BanCloseMulti, text, jailbreak, autoStart, stringBuilder.ToString(), roleParamsInt32FromDB, false);
									case 73:
										return this.BanRate(client, BanType.BanRateMulti, text, jailbreak, autoStart, stringBuilder.ToString(), roleParamsInt32FromDB, false);
									}
								}
								foreach (string text2 in this._specialAppList)
								{
									string text2;
									if (dictionary.ContainsKey(text2) && dictionary[text2] >= this._multiCount)
									{
										switch (this._banReasonDic[BanReasonType.SpecialApp])
										{
										case 50:
											flag = true;
											this.BanLog(client, BanType.BanLogSpecialApp, text, jailbreak, autoStart, text2, roleParamsInt32FromDB, false);
											break;
										case 51:
											return this.BanKick(client, BanType.BanKickSpecialApp, text, jailbreak, autoStart, text2, roleParamsInt32FromDB, false);
										case 52:
											return this.BanClose(client, BanType.BanCloseSpecialApp, text, jailbreak, autoStart, text2, roleParamsInt32FromDB, false);
										case 53:
											return this.BanRate(client, BanType.BanRateSpecialApp, text, jailbreak, autoStart, text2, roleParamsInt32FromDB, false);
										}
									}
								}
								this.m_Mutex.SetTag(21);
								for (int i = 0; i < num4; i++)
								{
									string taskName = this.GetTaskName(array5[i]);
									if (this.IsYueYu(client, taskName))
									{
										bool flag2 = false;
										this.BanYueYu(client, out flag2, text, jailbreak, autoStart, taskName, roleParamsInt32FromDB);
									}
									if (this.m_RobotTaskList.ContainsKey(taskName))
									{
										BanInfo banInfo = this.m_RobotTaskList[taskName];
										int num6 = client.ClientData.Level + client.ClientData.ChangeLifeCount * 100;
										if (num6 < banInfo.Level)
										{
											switch (banInfo.BanType)
											{
											case BanType.Old:
												return this.KickWarn(client, banInfo.BanType, text, jailbreak, autoStart, array5[i], roleParamsInt32FromDB, false);
											case BanType.BanLog:
												flag = true;
												this.BanLog(client, banInfo.BanType, text, jailbreak, autoStart, array5[i], roleParamsInt32FromDB, false);
												break;
											case BanType.BanKick:
											case BanType.BanKickBreak:
												return this.BanKick(client, banInfo.BanType, text, jailbreak, autoStart, array5[i], roleParamsInt32FromDB, true);
											case BanType.BanClose:
											case BanType.BanCloseBreak:
												return this.BanClose(client, banInfo.BanType, text, jailbreak, autoStart, array5[i], roleParamsInt32FromDB, true);
											case BanType.BanRate:
												return this.BanRate(client, banInfo.BanType, text, jailbreak, autoStart, array5[i], roleParamsInt32FromDB, true);
											}
										}
										else
										{
											flag = true;
											this.BanLog(client, BanType.BanLogBig, text, jailbreak, autoStart, array5[i], -1, false);
										}
									}
									else
									{
										try
										{
											if (!client.CheckCheatData.DropRateDown && this._banRateNum)
											{
												string text2 = array5[i];
												char[] array6 = text2.ToCharArray();
												for (int l = 0; l < array6.Length; l++)
												{
													int num7 = 0;
													if (int.TryParse(array6[l].ToString(), out num7))
													{
														long num8 = 0L;
														long.TryParse(array5[i], NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out num8);
														if (num8 != 0L)
														{
															this.BanRate(client, BanType.BanRateNum, text, jailbreak, autoStart, array5[i], roleParamsInt32FromDB, false);
															break;
														}
													}
												}
											}
										}
										catch (Exception ex)
										{
											LogManager.WriteException("反外挂：数值进程检查异常");
										}
									}
								}
							}
							if (this._vmIsOpen && array4.Length > 5 && array4[5].Length != 0)
							{
								bool flag3 = false;
								string taskID = "";
								string[] array7 = array4[5].Split(new char[]
								{
									'*'
								});
								if (array7.Length < 4)
								{
									return this.BanLog(client, BanType.BanLogInvalid, text, jailbreak, autoStart, "vm_info", -1, false);
								}
								try
								{
									bool flag4 = false;
									foreach (KeyValuePair<string, int> keyValuePair2 in this.m_RobotSignList)
									{
										if (array4[5].Contains(keyValuePair2.Key))
										{
											flag4 = true;
											taskID = keyValuePair2.Key;
											break;
										}
									}
									this.m_Mutex.SetTag(22);
									if (flag4)
									{
										int num9 = this._banReasonDic[BanReasonType.VMSign];
										if (this._vmSignGameOpenSeven[0] > 0 && (this.VMBanMapCodes.Contains(client.ClientData.MapCode) || Global.GetKaiFuTime().AddDays((double)this._vmSignGameOpenSeven[1]) > DateTime.Now))
										{
											num9 = this._vmSignGameOpenSeven[2];
										}
										switch (num9)
										{
										case 120:
											flag = true;
											this.BanLog(client, BanType.VmLogSign, text, jailbreak, autoStart, taskID, -1, false);
											break;
										case 121:
											return this.BanKick(client, BanType.VmKickSign, text, jailbreak, autoStart, taskID, -1, false);
										case 122:
											return this.BanClose(client, BanType.VmCloseSign, text, jailbreak, autoStart, taskID, -1, false);
										case 123:
											return this.BanRate(client, BanType.VmRateSign, text, jailbreak, autoStart, taskID, -1, false);
										}
										return true;
									}
								}
								catch
								{
									flag = true;
									this.BanLog(client, BanType.BanLogInvalid, text, jailbreak, autoStart, "vm_sign", -1, false);
								}
								bool flag5 = false;
								bool flag6 = false;
								for (int i = 0; i < 3; i++)
								{
									Dictionary<string, int> dictionary3 = null;
									bool flag7 = true;
									bool flag8 = false;
									array7[i] = array7[i].ToLower();
									switch (i)
									{
									case 0:
										dictionary3 = this.m_RobotBuildList;
										flag7 = false;
										if (array7[i].Contains("abi:x86"))
										{
											flag5 = true;
										}
										break;
									case 1:
										dictionary3 = this.m_RobotCPUList;
										if (array7[i].Contains("arm") || array7[i].Contains("aarch"))
										{
											flag6 = true;
										}
										break;
									case 2:
										dictionary3 = this.m_RobotPhoneList;
										flag8 = true;
										break;
									}
									if (!flag8)
									{
										if (flag6 && flag5)
										{
											taskID = "isArm&isX86";
											flag3 = true;
											break;
										}
										this.m_Mutex.SetTag(23);
										if (flag7)
										{
											flag3 = (dictionary3.Count != 0);
											foreach (KeyValuePair<string, int> keyValuePair2 in dictionary3)
											{
												if (array7[i].Contains(keyValuePair2.Key))
												{
													flag3 = false;
													break;
												}
											}
										}
										else
										{
											flag3 = false;
											foreach (KeyValuePair<string, int> keyValuePair2 in dictionary3)
											{
												if (array7[i].Contains(keyValuePair2.Key))
												{
													flag3 = true;
													taskID = keyValuePair2.Key;
													break;
												}
											}
										}
										if (flag3)
										{
											break;
										}
									}
								}
								if (flag3)
								{
									switch (this._banReasonDic[BanReasonType.VM])
									{
									case 110:
										flag = true;
										this.BanLog(client, BanType.VmLog, text, jailbreak, autoStart, taskID, -1, false);
										break;
									case 111:
										this.BanKick(client, BanType.VmKick, text, jailbreak, autoStart, taskID, -1, false);
										break;
									case 112:
										this.BanClose(client, BanType.VmClose, text, jailbreak, autoStart, taskID, -1, false);
										break;
									case 113:
										this.BanRate(client, BanType.VmRate, text, jailbreak, autoStart, taskID, -1, false);
										break;
									}
								}
							}
						}
						if (!flag)
						{
							this.BanLog(client, BanType.BanLogNormal, text, jailbreak, autoStart, "", -1, false);
						}
						result = true;
					}
				}
			}
			return result;
		}

		public bool KickWarn(GameClient client, BanType banType, string clientData, int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool forceKick = false)
		{
			client.CheckCheatData.KickWarnMaxCount++;
			this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, false);
			if (this.m_BanMinutes > 0 && this._jiaoBenIsOpen)
			{
				if (client.CheckCheatData.KickWarnMaxCount >= this._kickWarnMaxCount)
				{
					int num = this.BanRoleByCount(client, true);
					client.CheckCheatData.RobotDetectedReason = banType.ToString();
					client.CheckCheatData.RobotDetectedKickTime = TimeUtil.NOW() + 1000L;
					return false;
				}
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(32, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
			}
			return true;
		}

		private int BanRoleByCount(GameClient client, bool addBanCount)
		{
			int num = this.m_BanMinutes;
			using (this.m_Mutex.Enter(1))
			{
				if (addBanCount && !client.CheckCheatData.KickState)
				{
					client.CheckCheatData.KickState = true;
					int num2 = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
					Global.SaveRoleParamsInt32ValueToDB(client, "BanRobotCount", num2 + 1, false);
					num2 = Global.Clamp(num2, 0, this.m_BanMiuteList.Length - 1);
					num = this.m_BanMiuteList[num2];
				}
			}
			if (num > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(33, new object[0]), new object[]
				{
					num
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
				BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), num, 2);
			}
			else if (num < 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(34, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
				GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 2, 1), null, client.ServerId);
			}
			return num;
		}

		private bool BanLog(GameClient client, BanType banType, string clientData, int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (!this.BanIsOpen())
			{
				result = false;
			}
			else
			{
				client.CheckCheatData.BanCheckMaxCount++;
				using (this.m_Mutex.Enter(2))
				{
					if (this._logCountDic.ContainsKey((int)banType) && !isForce)
					{
						if (!client.CheckCheatData.LogCountDic.ContainsKey((int)banType))
						{
							client.CheckCheatData.LogCountDic.Add((int)banType, 0);
						}
						if (client.CheckCheatData.LogCountDic[(int)banType] >= this._logCountDic[(int)banType])
						{
							return false;
						}
						Dictionary<int, int> logCountDic;
						(logCountDic = client.CheckCheatData.LogCountDic)[(int)banType] = logCountDic[(int)banType] + 1;
					}
				}
				string strUserID = client.strUserID;
				int num = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				int vipLevel = client.ClientData.VipLevel;
				string ip = this.GetIp(client);
				LogManager.WriteLog(4, string.Format("Reason={3} BanCount={8} userID={9} level={10} vip={11} RoleID={0} RoleName={1} Exp={2} DeviceID={12} JailBreak={4} AutoStart={5} TaskID={7} Server={13} Data={6}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					ip,
					banType.ToString(),
					jailbreak,
					autoStart,
					clientData,
					taskID,
					banCount,
					strUserID,
					num,
					vipLevel,
					client.deviceID,
					GameManager.ServerId
				}), null, true);
				result = true;
			}
			return result;
		}

		private bool BanKick(GameClient client, BanType banType, string clientData = "", int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (!this.BanIsOpen() || this.m_BanMinutes <= 0)
			{
				result = true;
			}
			else
			{
				bool flag = false;
				int num = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				int vipLevel = client.ClientData.VipLevel;
				if (!isForce && (num >= this._banLevelLimit || vipLevel >= this._banVipLimit))
				{
					flag = true;
				}
				if (banCount < 0)
				{
					banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
				}
				if (flag)
				{
					client.CheckCheatData.BanCheckMaxCount++;
					taskID = banType.ToString() + "|" + taskID;
					this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskID, banCount, false);
					result = false;
				}
				else
				{
					this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, true);
					int num2 = this.m_BanMinutes;
					using (this.m_Mutex.Enter(3))
					{
						if (!client.CheckCheatData.KickState)
						{
							client.CheckCheatData.KickState = true;
							Global.SaveRoleParamsInt32ValueToDB(client, "BanRobotCount", banCount + 1, false);
							int num3 = Global.Clamp(banCount, 0, this.m_BanMiuteList.Length - 1);
							num2 = this.m_BanMiuteList[num3];
							this.BanDBLog(client, banType, taskID, banCount + 1);
						}
					}
					if (num2 > 0)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(33, new object[0]), new object[]
						{
							num2
						}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
						BanManager.BanRoleName(Global.FormatRoleName(client, client.ClientData.RoleName), num2, 2);
					}
					else if (num2 < 0)
					{
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(34, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
						GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 2, 1), null, client.ServerId);
					}
					client.CheckCheatData.RobotDetectedReason = banType.ToString();
					client.CheckCheatData.RobotDetectedKickTime = TimeUtil.NOW() + 1000L;
					result = true;
				}
			}
			return result;
		}

		private bool BanClose(GameClient client, BanType banType, string clientData = "", int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (!this.BanIsOpen())
			{
				result = true;
			}
			else
			{
				bool flag = false;
				int num = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
				int vipLevel = client.ClientData.VipLevel;
				if (!isForce && (num >= this._banLevelLimit || vipLevel >= this._banVipLimit))
				{
					flag = true;
				}
				if (banCount < 0)
				{
					banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
				}
				if (flag)
				{
					client.CheckCheatData.BanCheckMaxCount++;
					taskID = banType.ToString() + "|" + taskID;
					this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskID, banCount, false);
					result = false;
				}
				else
				{
					this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, true);
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GLang.GetLang(34, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.HintAndBox, 0);
					GameManager.DBCmdMgr.AddDBCmd(10119, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 2, 1), null, client.ServerId);
					Global.SaveRoleParamsInt32ValueToDB(client, "BanRobotCount", banCount + 1, false);
					this.BanDBLog(client, banType, taskID, banCount + 1);
					client.CheckCheatData.RobotDetectedReason = banType.ToString();
					client.CheckCheatData.RobotDetectedKickTime = TimeUtil.NOW() + 1000L;
					result = true;
				}
			}
			return result;
		}

		private bool BanRate(GameClient client, BanType banType, string clientData = "", int jailbreak = 0, int autoStart = 0, string taskID = "", int banCount = -1, bool isForce = false)
		{
			bool result;
			if (client.CheckCheatData.BanCheckMaxCount >= this._banCheckMaxCount)
			{
				result = true;
			}
			else
			{
				client.CheckCheatData.BanCheckMaxCount++;
				if (!this.BanIsOpen() || client.CheckCheatData.DropRateDown)
				{
					result = true;
				}
				else
				{
					bool flag = false;
					int num = client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level;
					int vipLevel = client.ClientData.VipLevel;
					if (!isForce && (num >= this._banLevelLimit || vipLevel >= this._banVipLimit))
					{
						flag = true;
					}
					if (banCount < 0)
					{
						banCount = Global.GetRoleParamsInt32FromDB(client, "BanRobotCount");
					}
					if (flag)
					{
						taskID = banType.ToString() + "|" + taskID;
						this.BanLog(client, BanType.BanLogBig, clientData, jailbreak, autoStart, taskID, banCount, false);
						result = false;
					}
					else
					{
						this.BanLog(client, banType, clientData, jailbreak, autoStart, taskID, banCount, true);
						client.CheckCheatData.DropRateDown = true;
						this.BanDBLog(client, banType, taskID, banCount + 1);
						result = true;
					}
				}
			}
			return result;
		}

		private void BanDBLog(GameClient client, BanType banType, string banID, int banCount)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				client.ClientData.ZoneID,
				client.strUserID,
				client.ClientData.RoleID,
				(int)banType,
				banID,
				banCount,
				client.deviceID
			});
			Global.ExecuteDBCmd(13112, strcmd, client.ServerId);
		}

		public void RobotDataReset(GameClient client)
		{
			if (client != null)
			{
				client.CheckCheatData.RobotTaskListData = "";
				client.CheckCheatData.BanCheckMaxCount = 0;
				client.CheckCheatData.KickWarnMaxCount = 0;
				client.CheckCheatData.DropRateDown = false;
				client.CheckCheatData.KickState = false;
				client.CheckCheatData.RobotDetectedKickTime = 0L;
				client.CheckCheatData.RobotDetectedReason = "";
				client.CheckCheatData.NextTaskListTimeout = 0L;
				client.CheckCheatData.LogCountDic = new Dictionary<int, int>();
			}
		}

		public double GetRobotSceneDropRate(GameClient client, int mapID, double dropRate, int monsterType)
		{
			double result;
			if (null == client)
			{
				result = dropRate;
			}
			else if (!this.BanIsOpen())
			{
				result = dropRate;
			}
			else
			{
				if (this.m_DropRateDownMonsterTypeHashSet.ContainsKey(monsterType))
				{
					double num;
					if (client.CheckCheatData.DropRateDown && this._DropRateDownMapDict.TryGetValue(mapID, out num))
					{
						dropRate *= num;
					}
				}
				result = dropRate;
			}
			return result;
		}

		public string GetIp(GameClient client)
		{
			string result = "0";
			switch (this._canLogIp)
			{
			case 1:
				result = Global.GetIPAddress(client.ClientSocket);
				break;
			case 2:
			{
				string ipaddress = Global.GetIPAddress(client.ClientSocket);
				result = IpHelper.IpToInt(ipaddress).ToString();
				break;
			}
			}
			return result;
		}

		private string GetTaskName(string task)
		{
			string text = task;
			int num = text.LastIndexOf('-');
			if (num > 0)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		private const int BAN_REASON = 2;

		private const long KICK_DELAY_TIME = 1000L;

		private static RobotTaskValidator instance = new RobotTaskValidator();

		private MyTagLock m_Mutex = new MyTagLock(true);

		private int m_TaskListVerifySeed;

		private int m_TaskListVerifyRandomCount = 50;

		private RSACryptoServiceProvider m_TaskListRSA;

		private string m_TaskListRSAPubKey;

		private RobotTaskValidator.TaskCheckList m_TaskCheckList = new RobotTaskValidator.TaskCheckList();

		private PlatformTypes _platformType = 0;

		private bool _useWorkThread = true;

		private int _banCheckMaxCount = 5;

		private int _kickWarnMaxCount = 2;

		private int _banLevelLimit = 401;

		private int _banVipLimit = 6;

		private int _canLogIp = 0;

		private int m_BanMinutes = 0;

		private int[] m_BanMiuteList;

		private HashSet<string> m_AllTaskHashSet = new HashSet<string>();

		private Dictionary<int, int> _logCountDic = new Dictionary<int, int>();

		private bool _jiaoBenIsOpen = false;

		private bool _vmIsOpen = false;

		private int _timeOutCount = 5;

		private long _timeOutMinute = 5L;

		private Dictionary<string, int> _cancelAppDic = new Dictionary<string, int>();

		private string[] _specialAppList = null;

		private Dictionary<PlatformTypes, int> _yyOpenPlatform = new Dictionary<PlatformTypes, int>();

		private Dictionary<string, int> _yueyuAppDic = new Dictionary<string, int>();

		private int _appCount = 10;

		private int _multiCount = 3;

		private int _decryptCount = 5;

		private bool _banRateNum = false;

		private int[] _appPlatformCount = new int[]
		{
			0,
			1,
			5
		};

		private Dictionary<BanReasonType, int> _banReasonDic = new Dictionary<BanReasonType, int>();

		private Dictionary<string, BanInfo> m_RobotTaskList = new Dictionary<string, BanInfo>();

		private Dictionary<string, int> m_RobotBuildList = new Dictionary<string, int>();

		private Dictionary<string, int> m_RobotCPUList = new Dictionary<string, int>();

		private Dictionary<string, int> m_RobotPhoneList = new Dictionary<string, int>();

		private Dictionary<string, int> m_RobotSignList = new Dictionary<string, int>();

		private Dictionary<string, int> m_YueYuDevList = new Dictionary<string, int>();

		private Dictionary<string, int> m_YueYuOSVerList = new Dictionary<string, int>();

		private Dictionary<string, List<string>> m_NewYueYuDevOSVerListDict = new Dictionary<string, List<string>>();

		private int[] _vmSignGameOpenSeven = new int[]
		{
			0,
			7,
			120
		};

		private int[] _yueYuGameOpenSeveb = new int[]
		{
			0,
			7,
			120
		};

		private HashSet<int> VMBanMapCodes = new HashSet<int>();

		private Thread BackgroundThread;

		public ConcurrentDictionary<int, double> _DropRateDownMapDict = new ConcurrentDictionary<int, double>();

		public ConcurrentDictionary<int, int> m_DropRateDownMonsterTypeHashSet = new ConcurrentDictionary<int, int>();

		private Dictionary<PlatformTypes, int> _banOpenPlatform = new Dictionary<PlatformTypes, int>();

		private bool _BanIsOpen = false;

		private Queue<RobotDataItem> RobotDataItemQueue = new Queue<RobotDataItem>();

		[ProtoContract]
		public class TaskCheckList : ICompressed
		{
			[ProtoMember(1)]
			public byte[] TaskCheckListData;
		}
	}
}
