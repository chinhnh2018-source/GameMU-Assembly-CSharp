using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Interface;
using GameServer.Logic.NewBufferExt;
using GameServer.Logic.RefreshIconState;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class GameClient : IObject
	{
		public GameClient()
		{
			int[] allyTip = new int[2];
			this.AllyTip = allyTip;
			this.CodeRevision = 0;
			this.MainExeVer = 0;
			this.ResVer = 0;
			this.KuaFuContextData = null;
			this.SceneContextData = null;
			this.SceneContextData2 = null;
			this.CheckCheatData = new CheckCheat();
			this.InterestingData = new InterestingData();
			this._RoleBuffer = new SpriteBuffer();
			this._RoleOnceBuffer = new SpriteOnceBuffer();
			this._RoleMagicHelper = new SpriteMagicHelper();
			this._RoleMultipliedBuffer = new SpriteMultipliedBuffer();
			this._AllThingsMultipliedBuffer = new SpriteMultipliedBuffer();
			this._LastLifeMagicTick = TimeUtil.NOW();
			this._LastCheckGMailTick = TimeUtil.NOW();
			this._OldGridPoint = new Point(-1.0, -1.0);
			this._OldAreaLuaIDList = null;
			this.InSafeRegion = true;
			this.lastAutoGetthingsTicks = 0L;
			this.BackgroundHandling = 0;
			this.Current9GridMutex = new object();
			this.LastRefresh9GridObjectsTicks = new long[5];
			this.CurrentSlotTicks = 2000L;
			this.ClientEffectHideFlag1 = 0;
			this.DelayStartPlayGameMsgQueue = new Queue<TCPOutPacket>();
			this.SumDamageForCopyTeam = 0L;
			this.bufferPropsManager = new BufferPropsModule();
			this.passiveSkillModule = new PassiveSkillModule();
			this.MyMagicsManyTimeDmageQueue = new MagicsManyTimeDmageQueue();
			this.propsCacheModule = new PropsCacheModule();
			this.delayExecModule = new DelayExecModule();
			this.buffManager = new BuffManager();
			this.TimedActionMgr = new TimedActionManager();
			this.OneSecsTimerEventObject = new TimerEventObject();
			this.MyBufferExtManager = new BufferExtManager();
			base..ctor();
		}

		public TMSKSocket ClientSocket { get; set; }

		public string strUserID { get; set; }

		public string deviceID { get; set; }

		public SafeClientData ClientData
		{
			get
			{
				return this._ClientData;
			}
			set
			{
				this._ClientData = value;
				if (null != this._ClientData)
				{
					this._ClientData.PropsCacheManager = new PropsCacheManager(this);
					this.bufferPropsManager.Init(this.ClientData.PropsCacheManager);
					this._ClientData.ChangePosHandler += this.ChangeGrid;
					this._ClientData.PurePropsCacheManager = new PropsCacheManager(this);
					this._ClientData.PctPropsCacheManager = new PropsCacheManager(this);
				}
			}
		}

		public UsingEquipManager UsingEquipMgr
		{
			get
			{
				return this._UsingEquipMgr;
			}
		}

		public bool ClientLogOutOnce()
		{
			bool result;
			lock (this)
			{
				if (!this._ClientLogOut)
				{
					this._ClientLogOut = true;
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public int ServerId
		{
			get
			{
				return this.ClientSocket.ServerId;
			}
		}

		public ObjectTypes ObjectType
		{
			get
			{
				return ObjectTypes.OT_CLIENT;
			}
		}

		public SpriteBuffer RoleBuffer
		{
			get
			{
				return this._RoleBuffer;
			}
			set
			{
				this._RoleBuffer = value;
			}
		}

		public SpriteOnceBuffer RoleOnceBuffer
		{
			get
			{
				return this._RoleOnceBuffer;
			}
			set
			{
				this._RoleOnceBuffer = value;
			}
		}

		public SpriteMagicHelper RoleMagicHelper
		{
			get
			{
				return this._RoleMagicHelper;
			}
			set
			{
				this._RoleMagicHelper = value;
			}
		}

		public SpriteMultipliedBuffer RoleMultipliedBuffer
		{
			get
			{
				return this._RoleMultipliedBuffer;
			}
			set
			{
				this._RoleMultipliedBuffer = value;
			}
		}

		public SpriteMultipliedBuffer AllThingsMultipliedBuffer
		{
			get
			{
				return this._AllThingsMultipliedBuffer;
			}
			set
			{
				this._AllThingsMultipliedBuffer = value;
			}
		}

		public int GetObjectID()
		{
			return this.ClientData.RoleID;
		}

		public long LastLifeMagicTick
		{
			get
			{
				return this._LastLifeMagicTick;
			}
			set
			{
				this._LastLifeMagicTick = value;
			}
		}

		public long LastCheckGMailTick
		{
			get
			{
				return this._LastCheckGMailTick;
			}
			set
			{
				this._LastCheckGMailTick = value;
			}
		}

		public Point CurrentGrid
		{
			get
			{
				GameMap gameMap = null;
				Point result;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(this.ClientData.MapCode, out gameMap))
				{
					LogManager.WriteLog(2, string.Format("GameClient CurrentGrid Error MapCode={0}", this.ClientData.MapCode), null, true);
					result = new Point(0.0, 0.0);
				}
				else
				{
					result = new Point((double)(this.ClientData.PosX / gameMap.MapGridWidth), (double)(this.ClientData.PosY / gameMap.MapGridHeight));
				}
				return result;
			}
			set
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this.ClientData.MapCode];
				this.ClientData.PosX = (int)(value.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2));
				this.ClientData.PosY = (int)(value.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
			}
		}

		public Point CurrentPos
		{
			get
			{
				return new Point((double)this.ClientData.PosX, (double)this.ClientData.PosY);
			}
			set
			{
				this.ClientData.PosX = (int)value.X;
				this.ClientData.PosY = (int)value.Y;
			}
		}

		public int CurrentMapCode
		{
			get
			{
				return this.ClientData.MapCode;
			}
		}

		public int CurrentCopyMapID
		{
			get
			{
				return this.ClientData.CopyMapID;
			}
		}

		public Dircetions CurrentDir
		{
			get
			{
				return (Dircetions)this.ClientData.RoleDirection;
			}
			set
			{
				this.ClientData.RoleDirection = (int)value;
			}
		}

		public List<int> PassiveEffectList
		{
			get
			{
				return this.ClientData.PassiveEffectList;
			}
			set
			{
				this.ClientData.PassiveEffectList = value;
			}
		}

		public void ClearChangeGrid()
		{
			if (this._OldAreaLuaIDList != null)
			{
				this._OldAreaLuaIDList.Clear();
			}
		}

		public void ChangeGrid()
		{
			if (this._ClientData.MapCode >= 0)
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[this._ClientData.MapCode];
				int num = this._ClientData.PosX / gameMap.MapGridWidth;
				int num2 = this._ClientData.PosY / gameMap.MapGridHeight;
				if (this._OldGridPoint.X != (double)num || this._OldGridPoint.Y != (double)num2)
				{
					this._OldGridPoint = new Point((double)num, (double)num2);
					this.ProcessAreaLua(gameMap, this._OldGridPoint);
					if (!gameMap.InSafeRegionList(this.CurrentGrid))
					{
						this.InSafeRegion = false;
						this.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
						{
							3
						});
					}
					else
					{
						this.InSafeRegion = true;
					}
				}
			}
		}

		public void AutoGetThingsOnAutoFight(long ticks)
		{
			if (ticks - this.lastAutoGetthingsTicks >= 20000L)
			{
				this.lastAutoGetthingsTicks = ticks;
				if (this.ClientData.AutoFighting && this.ClientData.AutoFightGetThings != 0)
				{
					GameMap gameMap = GameManager.MapMgr.DictMaps[this._ClientData.MapCode];
					if (!gameMap.InSafeRegionList(this.CurrentGrid))
					{
						GameManager.GoodsPackMgr.ProcessClickGoodsPackWhenMovingToOtherGrid(this, 5);
					}
				}
			}
		}

		private void ProcessAreaLua(GameMap gameMap, Point p)
		{
			if (gameMap != null)
			{
				Point grid = new Point(p.X, p.Y);
				List<int> areaLuaIDListByPoint = gameMap.GetAreaLuaIDListByPoint(grid);
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				if (areaLuaIDListByPoint != null)
				{
					if (this._OldAreaLuaIDList != null)
					{
						foreach (int item in areaLuaIDListByPoint)
						{
							if (!this._OldAreaLuaIDList.Contains(item))
							{
								list.Add(item);
							}
						}
					}
					else
					{
						list = areaLuaIDListByPoint;
					}
				}
				if (this._OldAreaLuaIDList != null)
				{
					if (areaLuaIDListByPoint != null)
					{
						foreach (int item in this._OldAreaLuaIDList)
						{
							if (!areaLuaIDListByPoint.Contains(item))
							{
								list2.Add(item);
							}
						}
					}
					else
					{
						list2 = this._OldAreaLuaIDList;
					}
				}
				if (list2 != null && list2.Count > 0)
				{
				}
				if (list != null && list.Count > 0)
				{
					this.RunAreaLuaFile(gameMap, RunAreaLuaType.AreaLuaIDList, list, "enterArea", 0);
				}
				this._OldAreaLuaIDList = areaLuaIDListByPoint;
			}
		}

		public void RunAreaLuaFile(GameMap gameMap, int areaLuaID, string functionName)
		{
			GAreaLua areaLuaByID = gameMap.GetAreaLuaByID(areaLuaID);
			if (null != areaLuaByID)
			{
				string luaScriptFileName = areaLuaByID.LuaScriptFileName;
				if (!string.IsNullOrEmpty(luaScriptFileName))
				{
					ProcessAreaScripts.ProcessScripts(this, luaScriptFileName, functionName, areaLuaID);
				}
			}
		}

		public void RunAreaLuaFile(GameMap gameMap, RunAreaLuaType runAreaLuaType, List<int> areaLuaIDList, string functionName, int taskId = 0)
		{
			List<GAreaLua> list = null;
			if (RunAreaLuaType.SelfPoint == runAreaLuaType)
			{
				int num = this._ClientData.PosX / gameMap.MapGridWidth;
				int num2 = this._ClientData.PosY / gameMap.MapGridHeight;
				list = gameMap.GetAreaLuaListByPoint(new Point((double)num, (double)num2));
			}
			else if (RunAreaLuaType.AreaLuaIDList == runAreaLuaType)
			{
				if (areaLuaIDList == null || areaLuaIDList.Count == 0)
				{
					return;
				}
				foreach (int areaLuaID in areaLuaIDList)
				{
					GAreaLua areaLuaByID = gameMap.GetAreaLuaByID(areaLuaID);
					if (areaLuaByID != null)
					{
						if (list == null)
						{
							list = new List<GAreaLua>();
						}
						list.Add(areaLuaByID);
					}
				}
			}
			if (list != null)
			{
				using (List<GAreaLua>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						GAreaLua areaLuaEv = enumerator2.Current;
						bool flag = false;
						if (areaLuaEv.AddtionType != AddtionType.NowTrigger)
						{
							switch (areaLuaEv.AddtionType)
							{
							case AddtionType.AccessTask:
							{
								TaskData taskData;
								lock (this.ClientData.TaskDataList)
								{
									taskData = this.ClientData.TaskDataList.Find((TaskData x) => x.DoingTaskID == areaLuaEv.TaskId);
									if (taskData == null)
									{
										break;
									}
								}
								SystemXmlItem systemXmlItem = null;
								if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(areaLuaEv.TaskId, out systemXmlItem))
								{
									if (taskData.DoingTaskVal1 < systemXmlItem.GetIntValue(string.Format("TargetNum1", new object[0]), -1) || taskData.DoingTaskVal2 < systemXmlItem.GetIntValue(string.Format("TargetNum2", new object[0]), -1))
									{
										flag = true;
									}
								}
								break;
							}
							case AddtionType.FinishTask:
							{
								TaskData taskData;
								lock (this.ClientData.TaskDataList)
								{
									taskData = this.ClientData.TaskDataList.Find((TaskData x) => x.DoingTaskID == areaLuaEv.TaskId);
									if (taskData == null)
									{
										break;
									}
								}
								if (Global.JugeTaskComplete(this, areaLuaEv.TaskId, taskData.DoingTaskVal1, taskData.DoingTaskVal2))
								{
									flag = true;
								}
								break;
							}
							case AddtionType.BackTask:
								if (taskId != 0)
								{
									if (areaLuaEv.TaskId == taskId)
									{
										flag = true;
									}
								}
								break;
							case AddtionType.NewMainTask:
								if (functionName == "takeNewMainTask" && taskId == areaLuaEv.TaskId)
								{
									flag = true;
								}
								break;
							}
							if (!flag)
							{
								continue;
							}
						}
						foreach (KeyValuePair<AreaEventType, List<int>> keyValuePair in areaLuaEv.AreaEventDict)
						{
							if (keyValuePair.Key == AreaEventType.FinishTask)
							{
								int eventTaskId = keyValuePair.Value[0];
								TaskData taskData = this.ClientData.TaskDataList.Find((TaskData x) => x.DoingTaskID == eventTaskId);
								SystemXmlItem systemXmlItem = null;
								if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(eventTaskId, out systemXmlItem))
								{
									if (taskData.DoingTaskVal1 < systemXmlItem.GetIntValue("TargetNum1", -1))
									{
										taskData.DoingTaskVal1 = systemXmlItem.GetIntValue("TargetNum1", -1);
									}
									if (taskData.DoingTaskVal2 < systemXmlItem.GetIntValue("TargetNum2", -1))
									{
										taskData.DoingTaskVal2 = systemXmlItem.GetIntValue("TargetNum2", -1);
									}
									GameManager.DBCmdMgr.AddDBCmd(10007, string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										this.ClientData.RoleID,
										taskData.DoingTaskID,
										taskData.DbID,
										taskData.DoingTaskFocus,
										taskData.DoingTaskVal1,
										taskData.DoingTaskVal2
									}), null, this.ServerId);
									GameManager.ClientMgr.NotifyUpdateTask(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this, taskData.DbID, taskData.DoingTaskID, taskData.DoingTaskVal1, taskData.DoingTaskVal2, taskData.DoingTaskFocus, taskData.ChengJiuVal);
									int intValue = systemXmlItem.GetIntValue("DestNPC", -1);
									if (-1 != intValue)
									{
										int state = Global.ComputeNPCTaskState(this, this.ClientData.TaskDataList, intValue, 0);
										GameManager.ClientMgr.NotifyUpdateNPCTaskSate(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, this, intValue + 2130706432, state);
									}
									ProcessTask.CheckAutoCompleteTask(this);
								}
							}
							else if (keyValuePair.Key == AreaEventType.CallMonsters && functionName == "enterArea")
							{
								int monsterID = keyValuePair.Value[0];
								Global.SystemKillSummonMonster(this, MonsterTypes.AreaCallMonster);
								GameManager.LuaMgr.CallMonstersForGameClient(this, monsterID, 1, 0, 1002, 1);
							}
							else if (keyValuePair.Key == AreaEventType.RemoveMonsters && functionName == "enterArea")
							{
								int monsterID2 = keyValuePair.Value[0];
								Global.SystemKillSummonMonster(this, monsterID2);
							}
							else
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									(int)keyValuePair.Key,
									keyValuePair.Value[0],
									areaLuaEv.CenterPoint.X,
									areaLuaEv.CenterPoint.Y
								});
								this.sendCmd(3000, cmdData, false);
							}
						}
						string luaScriptFileName = areaLuaEv.LuaScriptFileName;
						if (!string.IsNullOrEmpty(luaScriptFileName))
						{
							ProcessAreaScripts.ProcessScripts(this, luaScriptFileName, functionName, areaLuaEv.ID);
						}
					}
				}
			}
		}

		public void ClearVisibleObjects(bool recalcMonsterVisibleNum)
		{
			lock (this.ClientData.VisibleGrid9Objects)
			{
				if (recalcMonsterVisibleNum)
				{
					List<object> list = this.ClientData.VisibleGrid9Objects.Keys.ToList<object>();
					for (int i = 0; i < list.Count; i++)
					{
						object obj = list[i];
						if (obj is Monster)
						{
							if ((obj as Monster).CurrentCopyMapID == this.ClientData.CopyMapID)
							{
								(obj as Monster).VisibleClientsNum--;
							}
						}
					}
				}
				this.ClientData.VisibleGrid9Objects.Clear();
				this.ClientData.VisibleMeGrid9GameClients.Clear();
			}
		}

		public void SendCmdAfterStartPlayGame<T>(int cmdId, T cmdData)
		{
			lock (this.DelayStartPlayGameMsgQueue)
			{
				TCPOutPacket item = DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId);
				this.DelayStartPlayGameMsgQueue.Enqueue(item);
			}
		}

		public void SendCmdAfterStartPlayGame(int cmdId, string cmdData)
		{
			lock (this.DelayStartPlayGameMsgQueue)
			{
				TCPOutPacket item = TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId);
				this.DelayStartPlayGameMsgQueue.Enqueue(item);
			}
		}

		public void SendCmdAfterStartPlayGame(int cmdId, byte[] cmdData)
		{
			lock (this.DelayStartPlayGameMsgQueue)
			{
				TCPOutPacket item = TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId);
				this.DelayStartPlayGameMsgQueue.Enqueue(item);
			}
		}

		public void SendCmdOnStartPlayGame()
		{
			lock (this.DelayStartPlayGameMsgQueue)
			{
				while (this.DelayStartPlayGameMsgQueue.Count > 0)
				{
					TCPOutPacket tcpOutPacket = this.DelayStartPlayGameMsgQueue.Dequeue();
					TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		public void sendCmd(int cmdId, string cmdData, bool waitEnterScene = false)
		{
			if (waitEnterScene && this.ClientData.FirstPlayStart)
			{
				this.SendCmdAfterStartPlayGame(cmdId, cmdData);
			}
			else
			{
				TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId), true);
			}
		}

		public void sendCmd(int cmdId, byte[] cmdData, bool waitEnterScene = false)
		{
			if (waitEnterScene && this.ClientData.FirstPlayStart)
			{
				this.SendCmdAfterStartPlayGame(cmdId, cmdData);
			}
			else
			{
				TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, TCPOutPacket.MakeTCPOutPacket(TCPOutPacketPool.getInstance(), cmdData, cmdId), true);
			}
		}

		public void sendOthersCmd(int cmdId, string cmdData)
		{
			List<object> all9Clients = Global.GetAll9Clients(this);
			if (null != all9Clients)
			{
				GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, all9Clients, cmdData, cmdId);
			}
		}

		public void sendOthersCmd(int cmdId, byte[] cmdData)
		{
			List<object> all9Clients = Global.GetAll9Clients(this);
			if (null != all9Clients)
			{
				GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, all9Clients, cmdData, cmdId);
			}
		}

		public void sendCmd<T>(int cmdId, T cmdData, bool waitEnterScene = false)
		{
			if (waitEnterScene && this.ClientData.FirstPlayStart)
			{
				this.SendCmdAfterStartPlayGame<T>(cmdId, cmdData);
			}
			else
			{
				TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, DataHelper.ObjectToTCPOutPacket<T>(cmdData, TCPOutPacketPool.getInstance(), cmdId), true);
			}
		}

		public void sendCmd(TCPOutPacket cmdData, bool pushBack = true)
		{
			TCPManager.getInstance().MySocketListener.SendData(this.ClientSocket, cmdData, pushBack);
		}

		public void PushVersion(string MainExeVer = "", string ResVer = "")
		{
			this.sendCmd(673, string.Format("{0}:{1}:{2}", this.CodeRevision, MainExeVer, ResVer), false);
		}

		public void ExecuteEnterMap(int mapCode)
		{
			if (this._ClientData.MapCode >= 0 && this._ClientData.CopyMapID >= 0)
			{
				MapTypes mapType = Global.GetMapType(mapCode);
				if (mapType >= MapTypes.NormalCopy && mapType <= MapTypes.MarriageCopy)
				{
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(this._ClientData.CopyMapID);
					if (null != copyMap)
					{
						lock (copyMap.EventQueue)
						{
							foreach (MapAIEvent mapAIEvent in copyMap.EventQueue)
							{
								int guangMuID = mapAIEvent.GuangMuID;
								int show = mapAIEvent.Show;
								this.sendCmd(667, string.Format("{0}:{1}", guangMuID, show), false);
							}
						}
						if (!copyMap.ExecEnterMapLuaFile)
						{
							copyMap.ExecEnterMapLuaFile = true;
							GameMap gameMap = GameManager.MapMgr.DictMaps[this._ClientData.MapCode];
							if (!string.IsNullOrEmpty(gameMap.EnterMapLuaFile))
							{
								Global.ExcuteLuaFunction(this, gameMap.EnterMapLuaFile, "comeOn", null, null);
							}
						}
					}
				}
			}
		}

		public T GetExtComponent<T>(ExtComponentTypes type) where T : class
		{
			T result;
			if (type != ExtComponentTypes.ManyTimeDamageQueue)
			{
				result = default(T);
			}
			else
			{
				result = (this.MyMagicsManyTimeDmageQueue as T);
			}
			return result;
		}

		public string deviceModel;

		public string deviceOSVersion;

		public string IsReconnect;

		public string deviceIdfa;

		public bool IsYueYu = false;

		public string strUserName;

		private SafeClientData _ClientData = null;

		private UsingEquipManager _UsingEquipMgr = new UsingEquipManager();

		public IconStateManager _IconStateMgr = new IconStateManager();

		public bool LogoutState = false;

		private bool _ClientLogOut = false;

		public long KuaFuSwitchServerTicks;

		public int[] AllyTip;

		public int CodeRevision;

		public int MainExeVer;

		public int ResVer;

		public object KuaFuContextData;

		public object SceneContextData;

		public object SceneContextData2;

		public int SceneType;

		public object SceneObject;

		public object SceneInfoObject;

		public long SceneGameId;

		public long SceneAge;

		public CheckCheat CheckCheatData;

		public InterestingData InterestingData;

		private SpriteBuffer _RoleBuffer;

		private SpriteOnceBuffer _RoleOnceBuffer;

		private SpriteMagicHelper _RoleMagicHelper;

		private SpriteMultipliedBuffer _RoleMultipliedBuffer;

		private SpriteMultipliedBuffer _AllThingsMultipliedBuffer;

		private long _LastLifeMagicTick;

		private long _LastCheckGMailTick;

		private Point _OldGridPoint;

		private List<int> _OldAreaLuaIDList;

		public bool InSafeRegion;

		private long lastAutoGetthingsTicks;

		public int BackgroundHandling;

		public object Current9GridMutex;

		public long[] LastRefresh9GridObjectsTicks;

		public long CurrentSlotTicks;

		public int ClientEffectHideFlag1;

		private Queue<TCPOutPacket> DelayStartPlayGameMsgQueue;

		public long SumDamageForCopyTeam;

		public BufferPropsModule bufferPropsManager;

		public PassiveSkillModule passiveSkillModule;

		public MagicsManyTimeDmageQueue MyMagicsManyTimeDmageQueue;

		public PropsCacheModule propsCacheModule;

		public DelayExecModule delayExecModule;

		public BuffManager buffManager;

		public TimedActionManager TimedActionMgr;

		public ExtData extData;

		public TimerEventObject OneSecsTimerEventObject;

		public BufferExtManager MyBufferExtManager;
	}
}
