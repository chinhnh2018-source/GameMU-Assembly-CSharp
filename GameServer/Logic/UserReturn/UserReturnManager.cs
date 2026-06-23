using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.UserReturn
{
	public class UserReturnManager : ICmdProcessorEx, ICmdProcessor
	{
		public static UserReturnManager getInstance()
		{
			return UserReturnManager.instance;
		}

		public bool initialize()
		{
			return this.initConfigInfo();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(900, 1, 1, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(901, 2, 2, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(902, 3, 3, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(903, 1, 1, UserReturnManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			try
			{
				switch (nID)
				{
				case 900:
					return this.ProCmdReturnData(client, nID, bytes, cmdParams);
				case 901:
					return this.ProCmdReturnCheck(client, nID, bytes, cmdParams);
				case 902:
					return this.ProCmdReturnAward(client, nID, bytes, cmdParams);
				case 903:
					return this.ProCmdReturnXml(client, nID, bytes, cmdParams);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				client.sendCmd(1373, -11003.ToString(), false);
			}
			return true;
		}

		public bool ProCmdReturnData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				UserReturnData userReturnData = this.GetUserReturnData(client);
				client.sendCmd<UserReturnData>(nID, userReturnData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProCmdReturnCheck(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				string text = cmdParams[1];
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProCmdReturnAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 3))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int awardID = Convert.ToInt32(cmdParams[1]);
				int awardCount = Convert.ToInt32(cmdParams[2]);
				string text = "{0}:{1}:{2}";
				string arg = "0";
				EReturnAwardState ereturnAwardState = this.ReturnAward(client, num, awardID, awardCount);
				if (ereturnAwardState == EReturnAwardState.Succ)
				{
					UserReturnData userReturnData = this.GetUserReturnData(client);
					if (userReturnData.AwardDic.ContainsKey(num))
					{
						arg = string.Join<int>("*", userReturnData.AwardDic[num]);
					}
				}
				text = string.Format(text, (int)ereturnAwardState, num, arg);
				client.sendCmd(nID, text, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProCmdReturnXml(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				client.sendCmd<UserReturnXmlData>(nID, this._xmlData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private UserReturnData GetUserReturnData(GameClient client)
		{
			UserReturnData result;
			lock (this.Mutex)
			{
				UserReturnData returnData = client.ClientData.ReturnData;
				if (returnData != null && (returnData.ActivityIsOpen != this._returnActivityInfo.IsOpen || returnData.ActivityDay != this._returnActivityInfo.ActivityDay))
				{
					this.initUserReturnData(client);
					returnData = client.ClientData.ReturnData;
				}
				result = returnData;
			}
			return result;
		}

		private EReturnAwardState ReturnAward(GameClient client, int awardType, int awardID, int awardCount)
		{
			EReturnAwardState result;
			lock (this.Mutex)
			{
				if (!this._returnActivityInfo.IsOpen || TimeUtil.NowDateTime() >= this._returnActivityInfo.TimeEnd)
				{
					result = EReturnAwardState.ENoOpen;
				}
				else
				{
					UserReturnData userReturnData = this.GetUserReturnData(client);
					if (userReturnData.ReturnState != 2)
					{
						result = EReturnAwardState.ENoReturn;
					}
					else
					{
						switch (awardType)
						{
						case 1:
							result = EReturnAwardState.EFail;
							break;
						case 2:
							result = this.AwardReturn(client, userReturnData, awardID);
							break;
						case 3:
							result = this.AwardCheck(client, userReturnData, awardID);
							break;
						case 4:
							result = this.AwardShop(client, userReturnData, awardID, awardCount);
							break;
						case 5:
							result = this.AwardChongZhi(client, userReturnData, awardID);
							break;
						default:
							result = EReturnAwardState.EFail;
							break;
						}
					}
				}
			}
			return result;
		}

		private EReturnAwardState AwardReturn(GameClient client, UserReturnData myData, int awardID)
		{
			EReturnAwardState result;
			lock (this.Mutex)
			{
				bool flag2 = false;
				int num = 2;
				ReturnAwardInfo returnAwardInfo = null;
				if (!myData.AwardDic.ContainsKey(num))
				{
					flag2 = true;
					IOrderedEnumerable<ReturnAwardInfo> source = from info in this._returnAwardDic.Values
					orderby info.Vip
					select info;
					if (source.Any<ReturnAwardInfo>())
					{
						returnAwardInfo = source.First<ReturnAwardInfo>();
					}
				}
				else
				{
					int oldID = myData.AwardDic[num][0];
					IOrderedEnumerable<ReturnAwardInfo> source = from info in this._returnAwardDic.Values
					where info.ID > oldID
					orderby info.Vip
					select info;
					if (source.Any<ReturnAwardInfo>())
					{
						returnAwardInfo = source.First<ReturnAwardInfo>();
					}
				}
				if (returnAwardInfo == null || returnAwardInfo.ID != awardID)
				{
					result = EReturnAwardState.EFail;
				}
				else if (client.ClientData.VipLevel < returnAwardInfo.Vip)
				{
					result = EReturnAwardState.EVip;
				}
				else
				{
					List<GoodsData> list = new List<GoodsData>();
					if (returnAwardInfo.DefaultGoodsList != null)
					{
						list.AddRange(returnAwardInfo.DefaultGoodsList);
					}
					List<GoodsData> awardPro = GoodsHelper.GetAwardPro(client, returnAwardInfo.ProGoodsList);
					if (awardPro != null)
					{
						list.AddRange(awardPro);
					}
					if (!Global.CanAddGoodsDataList(client, list))
					{
						result = EReturnAwardState.ENoBag;
					}
					else
					{
						string award = returnAwardInfo.ID.ToString();
						bool flag3 = this.DBUserReturnAwardUpdate(client, num, award);
						if (flag3)
						{
							if (flag2)
							{
								myData.AwardDic.Add(num, new int[]
								{
									returnAwardInfo.ID
								});
							}
							else
							{
								myData.AwardDic[num] = new int[]
								{
									returnAwardInfo.ID
								};
							}
							for (int i = 0; i < list.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "回归奖励", "1900-01-01 12:00:00", list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
							}
							this.CheckActivityTip(client);
							result = EReturnAwardState.Succ;
						}
						else
						{
							result = EReturnAwardState.EFail;
						}
					}
				}
			}
			return result;
		}

		private EReturnAwardState AwardCheck(GameClient client, UserReturnData myData, int awardID)
		{
			bool flag = false;
			EReturnAwardState result;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.Mutex, ref flag);
				int num = 3;
				ReturnCheckAwardInfo returnCheckAwardInfo = null;
				if (!myData.AwardDic.ContainsKey(num))
				{
					Dictionary<int, int[]> awardDic = myData.AwardDic;
					int key = num;
					int[] value = new int[1];
					awardDic[key] = value;
				}
				int oldID = myData.AwardDic[num][0];
				IOrderedEnumerable<ReturnCheckAwardInfo> source = from info in this._returnCheckAwardDic.Values
				where myData.Level >= info.LevelMin && myData.Level <= info.LevelMax && info.ID > oldID
				orderby info.Day
				select info;
				if (source.Any<ReturnCheckAwardInfo>())
				{
					returnCheckAwardInfo = source.First<ReturnCheckAwardInfo>();
				}
				if (returnCheckAwardInfo == null || returnCheckAwardInfo.ID != awardID)
				{
					result = EReturnAwardState.EFail;
				}
				else
				{
					int num2 = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(myData.TimeReturn) + 1;
					if (returnCheckAwardInfo.Day > num2)
					{
						result = EReturnAwardState.EFail;
					}
					else
					{
						List<GoodsData> list = new List<GoodsData>();
						if (returnCheckAwardInfo.DefaultGoodsList != null)
						{
							list.AddRange(returnCheckAwardInfo.DefaultGoodsList);
						}
						List<GoodsData> awardPro = GoodsHelper.GetAwardPro(client, returnCheckAwardInfo.ProGoodsList);
						if (awardPro != null)
						{
							list.AddRange(awardPro);
						}
						if (!Global.CanAddGoodsDataList(client, list))
						{
							result = EReturnAwardState.ENoBag;
						}
						else
						{
							string award = returnCheckAwardInfo.ID.ToString();
							bool flag2 = this.DBUserReturnAwardUpdate(client, num, award);
							if (flag2)
							{
								myData.AwardDic[num][0] = returnCheckAwardInfo.ID;
								for (int i = 0; i < list.Count; i++)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "召回签到", "1900-01-01 12:00:00", list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
								}
								this.CheckActivityTip(client);
								result = EReturnAwardState.Succ;
							}
							else
							{
								result = EReturnAwardState.EFail;
							}
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
			return result;
		}

		private EReturnAwardState AwardShop(GameClient client, UserReturnData myData, int awardID, int awardCount)
		{
			EReturnAwardState result;
			lock (this.Mutex)
			{
				int num = 4;
				bool flag2 = true;
				int num2 = 0;
				int[] array = null;
				if (myData.AwardDic.ContainsKey(num))
				{
					flag2 = false;
					array = myData.AwardDic[num];
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i++] == awardID)
						{
							num2 = array[i];
							break;
						}
					}
				}
				ReturnShopAwardInfo returnShopAwardInfo = this.GetReturnShopAwardInfo(awardID);
				if (returnShopAwardInfo == null)
				{
					result = EReturnAwardState.EFail;
				}
				else if (num2 + awardCount > returnShopAwardInfo.LimitCount)
				{
					result = EReturnAwardState.EShopMax;
				}
				else
				{
					int num3 = returnShopAwardInfo.NewPrice * awardCount;
					if (num3 > client.ClientData.UserMoney)
					{
						result = EReturnAwardState.ENoMoney;
					}
					else if (!Global.CanAddGoodsNum(client, awardCount))
					{
						result = EReturnAwardState.ENoBag;
					}
					else if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num3, "召回商店", true, false, false, DaiBiSySType.None))
					{
						result = EReturnAwardState.ENoMoney;
					}
					else
					{
						List<int> list = new List<int>();
						if (flag2)
						{
							list.Add(awardID);
							list.Add(awardCount);
						}
						else
						{
							bool flag3 = false;
							for (int i = 0; i < array.Length; i++)
							{
								int num4 = array[i++];
								int num5 = array[i];
								if (num4 == awardID)
								{
									flag3 = true;
									num5 += awardCount;
								}
								list.Add(num4);
								list.Add(num5);
							}
							if (!flag3)
							{
								list.Add(awardID);
								list.Add(awardCount);
							}
						}
						string award = string.Join<int>("*", list.ToArray<int>());
						bool flag4 = this.DBUserReturnAwardUpdate(client, num, award);
						if (flag4)
						{
							if (flag2)
							{
								myData.AwardDic.Add(num, list.ToArray<int>());
							}
							else
							{
								myData.AwardDic[num] = list.ToArray<int>();
							}
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, returnShopAwardInfo.Goods.GoodsID, returnShopAwardInfo.Goods.GCount, returnShopAwardInfo.Goods.Quality, "", returnShopAwardInfo.Goods.Forge_level, returnShopAwardInfo.Goods.Binding, 0, "", true, awardCount, "召回商店", "1900-01-01 12:00:00", returnShopAwardInfo.Goods.AddPropIndex, returnShopAwardInfo.Goods.BornIndex, returnShopAwardInfo.Goods.Lucky, returnShopAwardInfo.Goods.Strong, returnShopAwardInfo.Goods.ExcellenceInfo, returnShopAwardInfo.Goods.AppendPropLev, 0, null, null, 0, true);
							result = EReturnAwardState.Succ;
						}
						else
						{
							result = EReturnAwardState.EFail;
						}
					}
				}
			}
			return result;
		}

		private EReturnAwardState AwardChongZhi(GameClient client, UserReturnData myData, int awardID)
		{
			bool flag = false;
			EReturnAwardState result;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.Mutex, ref flag);
				int num = 5;
				ReturnChonZhiAwardInfo returnChonZhiAwardInfo = null;
				if (!myData.AwardDic.ContainsKey(num))
				{
					Dictionary<int, int[]> awardDic = myData.AwardDic;
					int key = num;
					int[] value = new int[1];
					awardDic[key] = value;
				}
				int oldID = myData.AwardDic[num][0];
				IEnumerable<ReturnChonZhiAwardInfo> source = from info in this._recallChongZhiAwardDict.Values
				where myData.LeiJiChongZhi >= info.MinYuanBao && info.ID > oldID
				select info;
				if (source.Any<ReturnChonZhiAwardInfo>())
				{
					returnChonZhiAwardInfo = source.First<ReturnChonZhiAwardInfo>();
				}
				if (returnChonZhiAwardInfo == null || returnChonZhiAwardInfo.ID != awardID)
				{
					result = EReturnAwardState.EFail;
				}
				else
				{
					List<GoodsData> list = new List<GoodsData>();
					if (returnChonZhiAwardInfo.DefaultGoodsList != null)
					{
						list.AddRange(returnChonZhiAwardInfo.DefaultGoodsList);
					}
					List<GoodsData> awardPro = GoodsHelper.GetAwardPro(client, returnChonZhiAwardInfo.ProGoodsList);
					if (awardPro != null)
					{
						list.AddRange(awardPro);
					}
					if (!Global.CanAddGoodsDataList(client, list))
					{
						result = EReturnAwardState.ENoBag;
					}
					else
					{
						string award = returnChonZhiAwardInfo.ID.ToString();
						bool flag2 = this.DBUserReturnAwardUpdate(client, num, award);
						if (flag2)
						{
							myData.AwardDic[num][0] = returnChonZhiAwardInfo.ID;
							for (int i = 0; i < list.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "召回累充", "1900-01-01 12:00:00", list[i].AddPropIndex, list[i].BornIndex, list[i].Lucky, list[i].Strong, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
							}
							this.CheckActivityTip(client);
							result = EReturnAwardState.Succ;
						}
						else
						{
							result = EReturnAwardState.EFail;
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
			return result;
		}

		public bool initConfigInfo()
		{
			lock (this.Mutex)
			{
				this.InitXmlData();
				this.LoadReturnActivityInfo();
				this.LoadReturnAwardInfo();
				this.LoadReturnCheckAwardInfo();
				this.LoadReturnShopAwardInfo();
				this.LoadReturnChongZhiGiftInfo();
			}
			return true;
		}

		private void InitXmlData()
		{
			try
			{
				if (this._xmlData.XmlList == null)
				{
					this._xmlData.XmlList = new List<string>();
				}
				if (this._xmlData.XmlNameList == null)
				{
					this._xmlData.XmlNameList = new List<string>();
				}
				this._xmlData.XmlList.Clear();
				this._xmlData.XmlNameList.Clear();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "初始化老玩家数据时出现异常!!!", ex, true);
			}
		}

		private void LoadReturnActivityInfo()
		{
			string text = Global.IsolateResPath("Config/PlayerRecall/HuoDongZhaoHui.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					this._xmlData.XmlNameList.Add("HuoDongZhaoHui.xml");
					this._xmlData.XmlList.Add(File.ReadAllText(text));
					this._returnActivityInfo = new ReturnActivityInfo();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							this._returnActivityInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							this._returnActivityInfo.ActivityID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "HuoDongID", "0"));
							this._returnActivityInfo.TimeBegin = DateTime.Parse(Global.GetDefAttributeStr(xelement2, "BeginTime", "1970-01-01 00:00:00"));
							this._returnActivityInfo.TimeEnd = DateTime.Parse(Global.GetDefAttributeStr(xelement2, "FinishTime", "1970-01-01 00:00:00"));
							this._returnActivityInfo.TimeBeginNoLogin = DateTime.Parse(Global.GetDefAttributeStr(xelement2, "NotLoggedInBegin", "1970-01-01 00:00:00"));
							this._returnActivityInfo.TimeEndNoLogin = DateTime.Parse(Global.GetDefAttributeStr(xelement2, "NotLoggedInFinish", "1970-01-01 00:00:00"));
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "Level", "0,0");
							string[] array = defAttributeStr.Split(new char[]
							{
								','
							});
							this._returnActivityInfo.Level = int.Parse(array[0]) * 100 + int.Parse(array[1]);
							this._returnActivityInfo.Vip = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "VIP", "4"));
							if (TimeUtil.NowDateTime() >= this._returnActivityInfo.TimeBegin && TimeUtil.NowDateTime() < this._returnActivityInfo.TimeEnd)
							{
								this._returnActivityInfo.IsOpen = true;
								this._returnActivityInfo.ActivityDay = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd");
								this._returnActivityInfo.TimeBeginStr = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd HH:mm:ss");
								this._returnActivityInfo.TimeEndStr = this._returnActivityInfo.TimeEnd.ToString("yyyy-MM-dd HH:mm:ss");
							}
							this.DBReturnIsOpen();
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载IsolateRes/Config/PlayerRecall/HuoDongZhaoHui.xml时出现异常!!!", ex, true);
				}
			}
		}

		public bool IsUserReturnOpen()
		{
			return this._returnActivityInfo.IsOpen;
		}

		private void LoadReturnAwardInfo()
		{
			string text = Global.IsolateResPath("Config/PlayerRecall/OldLogin.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					this._xmlData.XmlNameList.Add("OldLogin.xml");
					this._xmlData.XmlList.Add(File.ReadAllText(text));
					this._returnAwardDic = new Dictionary<int, ReturnAwardInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ReturnAwardInfo returnAwardInfo = new ReturnAwardInfo();
						returnAwardInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
						returnAwardInfo.Vip = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinVip", "0"));
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID1");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								returnAwardInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
							}
						}
						safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID2");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length > 0)
							{
								returnAwardInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
							}
						}
						this._returnAwardDic.Add(returnAwardInfo.ID, returnAwardInfo);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载IsolateRes/Config/PlayerRecall/OldLogin.xml时出现异常!!!", ex, true);
				}
			}
		}

		private void LoadReturnCheckAwardInfo()
		{
			string text = Global.IsolateResPath("Config/PlayerRecall/OldHuoDongLoginNumGift.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					this._xmlData.XmlNameList.Add("OldHuoDongLoginNumGift.xml");
					this._xmlData.XmlList.Add(File.ReadAllText(text));
					this._returnCheckAwardDic = new Dictionary<int, ReturnCheckAwardInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ReturnCheckAwardInfo returnCheckAwardInfo = new ReturnCheckAwardInfo();
						returnCheckAwardInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
						string defAttributeStr = Global.GetDefAttributeStr(xml, "Level", "");
						if (!string.IsNullOrEmpty(defAttributeStr))
						{
							string[] array = defAttributeStr.Split(new char[]
							{
								'|'
							});
							string[] array2 = array[0].Split(new char[]
							{
								','
							});
							returnCheckAwardInfo.LevelMin = Convert.ToInt32(array2[0]) * 100 + Convert.ToInt32(array2[1]);
							array2 = array[1].Split(new char[]
							{
								','
							});
							returnCheckAwardInfo.LevelMax = Convert.ToInt32(array2[0]) * 100 + Convert.ToInt32(array2[1]);
							returnCheckAwardInfo.Day = Convert.ToInt32(Global.GetDefAttributeStr(xml, "TimeOl", "0"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID1");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array3 = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array3.Length > 0)
								{
									returnCheckAwardInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array3, text);
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID2");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array3 = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array3.Length > 0)
								{
									returnCheckAwardInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array3, text);
								}
							}
							this._returnCheckAwardDic.Add(returnCheckAwardInfo.ID, returnCheckAwardInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载IsolateRes/Config/PlayerRecall/OldHuoDongLoginNumGift.xml时出错!!!文件不存在", null, true);
				}
			}
		}

		private void LoadReturnShopAwardInfo()
		{
			string text = Global.IsolateResPath("Config/PlayerRecall/OldStore.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					this._xmlData.XmlNameList.Add("OldStore.xml");
					this._xmlData.XmlList.Add(File.ReadAllText(text));
					this._returnShopAwardDic = new Dictionary<int, ReturnShopAwardInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						ReturnShopAwardInfo returnShopAwardInfo = new ReturnShopAwardInfo();
						returnShopAwardInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
						returnShopAwardInfo.OldPrice = Convert.ToInt32(Global.GetDefAttributeStr(xml, "OrigPrice", "0"));
						returnShopAwardInfo.NewPrice = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Price", "0"));
						returnShopAwardInfo.LimitCount = Convert.ToInt32(Global.GetDefAttributeStr(xml, "SinglePurchase", "0"));
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "GoodsID");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							returnShopAwardInfo.Goods = GoodsHelper.ParseGoodsData(safeAttributeStr, text);
						}
						this._returnShopAwardDic.Add(returnShopAwardInfo.ID, returnShopAwardInfo);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载IsolateRes/Config/PlayerRecall/OldHuoDongLoginNumGift.xml时出错!!!文件不存在", null, true);
				}
			}
		}

		public ReturnShopAwardInfo GetReturnShopAwardInfo(int id)
		{
			ReturnShopAwardInfo result;
			if (this._returnShopAwardDic.ContainsKey(id))
			{
				result = this._returnShopAwardDic[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		private void LoadReturnChongZhiGiftInfo()
		{
			string text = Global.IsolateResPath("Config/PlayerRecall/OldHuoDongchongzhiGift.xml");
			XElement xelement = CheckHelper.LoadXml(text, true);
			if (null != xelement)
			{
				try
				{
					this._xmlData.XmlNameList.Add("OldHuoDongchongzhiGift.xml");
					this._xmlData.XmlList.Add(File.ReadAllText(text));
					this._recallChongZhiAwardDict = new Dictionary<int, ReturnChonZhiAwardInfo>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							ReturnChonZhiAwardInfo returnChonZhiAwardInfo = new ReturnChonZhiAwardInfo
							{
								ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
								MinYuanBao = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinYuanBao", "0"))
							};
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID1");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length > 0)
								{
									returnChonZhiAwardInfo.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsID2");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length > 0)
								{
									returnChonZhiAwardInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array, text);
								}
							}
							this._recallChongZhiAwardDict.Add(returnChonZhiAwardInfo.ID, returnChonZhiAwardInfo);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载IsolateRes/Config/PlayerRecall/OldHuoDongchongzhiGift.xml时出错!!!文件不存在", ex, true);
				}
			}
		}

		public void initUserReturnData(GameClient client)
		{
			lock (this.Mutex)
			{
				UserReturnData returnData = client.ClientData.ReturnData;
				UserReturnData userReturnData = new UserReturnData();
				if (this._returnActivityInfo.IsOpen)
				{
					userReturnData.ActivityIsOpen = this._returnActivityInfo.IsOpen;
					userReturnData.ActivityID = this._returnActivityInfo.ActivityID;
					userReturnData.ActivityDay = this._returnActivityInfo.ActivityDay;
					userReturnData.TimeBegin = this._returnActivityInfo.TimeBegin;
					userReturnData.TimeEnd = this._returnActivityInfo.TimeEnd;
					userReturnData.TimeAward = this._returnActivityInfo.TimeEnd;
					int platformType = GameCoreInterface.getinstance().GetPlatformType();
					userReturnData.MyCode = string.Format("{0}#{1}#{2}", StringUtil.IDToCode(platformType), StringUtil.IDToCode(client.ClientData.ZoneID), StringUtil.IDToCode(client.ClientData.RoleID));
					this.initReturnData(client, userReturnData, returnData);
					userReturnData.AwardDic = this.DBUserReturnAwardList(client);
				}
				client.ClientData.ReturnData = userReturnData;
				this.CheckActivityTip(client);
			}
		}

		private void initReturnData(GameClient client, UserReturnData newData, UserReturnData oldData)
		{
			lock (this.Mutex)
			{
				string cmd = string.Format("{0}:{1}:{2}", client.strUserID, client.ClientData.ZoneID, TimeUtil.NowDateTime().ToString("yyyy-MM-dd"));
				Global.sendToDB<int, string>(13107, cmd, 0);
				ReturnData returnData = this.DBUserReturnDataGet(client);
				if (returnData != null)
				{
					int platformType = GameCoreInterface.getinstance().GetPlatformType();
					newData.RecallCode = string.Format("{0}#{1}#{2}", StringUtil.IDToCode(platformType), StringUtil.IDToCode(returnData.PZoneID), StringUtil.IDToCode(returnData.PRoleID));
					newData.RecallZoneID = returnData.PZoneID;
					newData.RecallRoleID = returnData.PRoleID;
					newData.Level = returnData.Level;
					newData.Vip = returnData.Vip;
					newData.TimeReturn = returnData.LogTime;
					if (returnData.Level % 100 == 0)
					{
						newData.ZhuanSheng = returnData.Level / 100 - 1;
						newData.DengJi = 100;
					}
					else
					{
						newData.ZhuanSheng = returnData.Level / 100;
						newData.DengJi = returnData.Level % 100;
					}
					newData.LeiJiChongZhi = returnData.LeiJiChongZhi;
					switch (returnData.StateCheck)
					{
					case 0:
						newData.ReturnState = -7;
						break;
					case 1:
						newData.ReturnState = 2;
						break;
					}
					if ((oldData == null && newData != null && newData.ReturnState == 2) || (oldData != null && oldData.ActivityDay != newData.ActivityDay && newData.ReturnState == 2))
					{
						string msgText = StringUtil.substitute(GLang.GetLang(555, new object[0]), new object[]
						{
							client.ClientData.RoleName
						});
						Global.BroadcastRoleActionMsg(client, RoleActionsMsgTypes.HintMsg, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0, 0, 100, 100);
					}
				}
			}
		}

		private void ReturnDicAdd(List<ReturnData> list)
		{
			foreach (ReturnData returnData in list)
			{
				this._returnDic[returnData.CRoleID] = returnData;
			}
		}

		public void DBReturnIsOpen()
		{
			int num = Global.Clamp(this._returnActivityInfo.Vip, 0, Data.VIPLevAwardAndExpInfoList.Count - 1);
			ReturnActivity cmd = new ReturnActivity
			{
				IsOpen = this._returnActivityInfo.IsOpen,
				NotLoggedInBegin = this._returnActivityInfo.TimeBeginNoLogin,
				NotLoggedInFinish = this._returnActivityInfo.TimeEndNoLogin,
				Level = this._returnActivityInfo.Level,
				VIPNeedExp = ((num == 0) ? 0 : Data.VIPLevAwardAndExpInfoList[num].NeedExp),
				ActivityID = this._returnActivityInfo.ActivityID,
				ActivityDay = this._returnActivityInfo.ActivityDay
			};
			Global.sendToDB<int, ReturnActivity>(13100, cmd, 0);
		}

		public ReturnData DBUserReturnDataGet(GameClient client)
		{
			string cmd = string.Format("{0}:{1}", client.strUserID, client.ClientData.ZoneID);
			ReturnData returnData = Global.sendToDB<ReturnData, string>(13101, cmd, client.ServerId);
			this.ReturnDicAdd(new List<ReturnData>
			{
				returnData
			});
			return returnData;
		}

		public List<ReturnData> DBUserReturnDataList(GameClient client)
		{
			string cmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				this._returnActivityInfo.ActivityDay,
				this._returnActivityInfo.ActivityID,
				client.ClientData.ZoneID,
				client.ClientData.RoleID
			});
			List<ReturnData> list = Global.sendToDB<List<ReturnData>, string>(13104, cmd, client.ServerId);
			this.ReturnDicAdd(list);
			return list;
		}

		public bool DBUserReturnDataUpdate(GameClient client, ReturnData data)
		{
			return Global.sendToDB<bool, ReturnData>(13102, data, client.ServerId);
		}

		public bool DBUserReturnDataDel(GameClient client, ReturnData data)
		{
			return Global.sendToDB<bool, ReturnData>(13103, data, client.ServerId);
		}

		public Dictionary<int, int[]> DBUserReturnAwardList(GameClient client)
		{
			string cmd = string.Format("{0}:{1}:{2}", this._returnActivityInfo.ActivityDay, this._returnActivityInfo.ActivityID, client.strUserID);
			return Global.sendToDB<Dictionary<int, int[]>, string>(13105, cmd, client.ServerId);
		}

		public bool DBUserReturnAwardUpdate(GameClient client, int awardType, string award)
		{
			string cmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				this._returnActivityInfo.ActivityDay,
				this._returnActivityInfo.ActivityID,
				client.ClientData.ZoneID,
				client.strUserID,
				awardType,
				award
			});
			return Global.sendToDB<bool, string>(13106, cmd, client.ServerId);
		}

		public void CheckUserReturnOpenState(long ticks)
		{
			if (ticks - this._lastTicks >= 10000L)
			{
				this._lastTicks = ticks;
				this.UpdateUserReturnState();
			}
		}

		public void UpdateUserReturnState()
		{
			DateTime t = TimeUtil.NowDateTime();
			if (!this._returnActivityInfo.IsOpen && t >= this._returnActivityInfo.TimeBegin && t < this._returnActivityInfo.TimeEnd)
			{
				this._returnActivityInfo.IsOpen = true;
				this._returnActivityInfo.ActivityDay = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd");
				this._returnActivityInfo.TimeBeginStr = this._returnActivityInfo.TimeBegin.ToString("yyyy-MM-dd HH:mm:ss");
				this._returnActivityInfo.TimeEndStr = this._returnActivityInfo.TimeEnd.ToString("yyyy-MM-dd HH:mm:ss");
				GameManager.ClientMgr.NotifyAllActivityState(3, 1, this._returnActivityInfo.TimeBegin.ToString("yyyyMMddHHmmss"), this._returnActivityInfo.TimeEnd.ToString("yyyyMMddHHmmss"), this._returnActivityInfo.ActivityID);
				this.DBReturnIsOpen();
			}
			if (this._returnActivityInfo.IsOpen && (t > this._returnActivityInfo.TimeEnd || t < this._returnActivityInfo.TimeBegin))
			{
				this.DBReturnIsOpen();
				this._returnActivityInfo.IsOpen = false;
				this._returnActivityInfo.ActivityDay = "";
				this._returnActivityInfo.TimeBeginStr = "";
				this._returnActivityInfo.TimeEndStr = "";
				GameManager.ClientMgr.NotifyAllActivityState(3, 0, "", "", 0);
			}
		}

		private DateTime getUserReturnBeginTime()
		{
			string gameConfigItemStr = GameManager.GameConfigMgr.GetGameConfigItemStr("userbegintime", "");
			DateTime result;
			if (gameConfigItemStr == "")
			{
				result = DateTime.MinValue;
			}
			else
			{
				DateTime dateTime;
				DateTime.TryParse(gameConfigItemStr, out dateTime);
				result = dateTime;
			}
			return result;
		}

		public void VipChange(GameClient client)
		{
			this.CheckActivityTip(client);
		}

		public void CheckActivityTip(GameClient client)
		{
			UserReturnData myData = client.ClientData.ReturnData;
			if (myData == null || myData.ReturnState <= 0)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					3,
					0,
					"",
					"",
					0
				});
				client.sendCmd(770, cmdData, false);
			}
			else
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.Mutex, ref flag);
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = false;
					if (myData != null)
					{
						int oldID = 0;
						if (myData.ReturnState == 2)
						{
							int key = 2;
							ReturnAwardInfo returnAwardInfo = null;
							if (myData.AwardDic.ContainsKey(key) && myData.AwardDic[key].Length > 0)
							{
								oldID = myData.AwardDic[key][0];
							}
							IOrderedEnumerable<ReturnAwardInfo> source = from info in this._returnAwardDic.Values
							where info.ID > oldID
							orderby info.Vip
							select info;
							if (source.Any<ReturnAwardInfo>())
							{
								returnAwardInfo = source.First<ReturnAwardInfo>();
							}
							if (returnAwardInfo != null && client.ClientData.VipLevel >= returnAwardInfo.Vip)
							{
								flag2 = true;
								client._IconStateMgr.AddFlushIconState(14102, true);
							}
							else
							{
								client._IconStateMgr.AddFlushIconState(14102, false);
							}
							oldID = 0;
							key = 3;
							ReturnCheckAwardInfo returnCheckAwardInfo = null;
							if (myData.AwardDic.ContainsKey(key) && myData.AwardDic[key].Length > 0)
							{
								oldID = myData.AwardDic[key][0];
							}
							IOrderedEnumerable<ReturnCheckAwardInfo> source2 = from info in this._returnCheckAwardDic.Values
							where myData.Level >= info.LevelMin && myData.Level <= info.LevelMax && info.ID > oldID
							orderby info.Day
							select info;
							if (source2.Any<ReturnCheckAwardInfo>())
							{
								returnCheckAwardInfo = source2.First<ReturnCheckAwardInfo>();
							}
							int num = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(myData.TimeReturn) + 1;
							if (returnCheckAwardInfo != null && returnCheckAwardInfo.Day <= num)
							{
								flag2 = true;
								client._IconStateMgr.AddFlushIconState(14103, true);
							}
							else
							{
								client._IconStateMgr.AddFlushIconState(14103, false);
							}
							oldID = 0;
							key = 5;
							ReturnChonZhiAwardInfo returnChonZhiAwardInfo = null;
							if (myData.AwardDic.ContainsKey(key) && myData.AwardDic[key].Length > 0)
							{
								oldID = myData.AwardDic[key][0];
							}
							IEnumerable<ReturnChonZhiAwardInfo> source3 = from info in this._recallChongZhiAwardDict.Values
							where myData.LeiJiChongZhi >= info.MinYuanBao && info.ID > oldID
							select info;
							if (source3.Any<ReturnChonZhiAwardInfo>())
							{
								returnChonZhiAwardInfo = source3.First<ReturnChonZhiAwardInfo>();
							}
							if (returnChonZhiAwardInfo != null)
							{
								flag4 = true;
								client._IconStateMgr.AddFlushIconState(14115, true);
							}
							else
							{
								client._IconStateMgr.AddFlushIconState(14115, false);
							}
						}
						if (flag2 || flag3 || flag4)
						{
							client._IconStateMgr.AddFlushIconState(14100, true);
						}
						else
						{
							client._IconStateMgr.AddFlushIconState(14100, false);
						}
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				finally
				{
					if (flag)
					{
						object mutex;
						Monitor.Exit(mutex);
					}
				}
			}
		}

		private const int AWARD_DAY_COUNT = 6;

		private const int CHECK_WAIT_HOUR = 1;

		private static UserReturnManager instance = new UserReturnManager();

		private object Mutex = new object();

		public UserReturnXmlData _xmlData = new UserReturnXmlData();

		public ReturnActivityInfo _returnActivityInfo = new ReturnActivityInfo();

		public Dictionary<int, ReturnAwardInfo> _returnAwardDic = new Dictionary<int, ReturnAwardInfo>();

		public Dictionary<int, ReturnCheckAwardInfo> _returnCheckAwardDic = new Dictionary<int, ReturnCheckAwardInfo>();

		public Dictionary<int, ReturnShopAwardInfo> _returnShopAwardDic = new Dictionary<int, ReturnShopAwardInfo>();

		public Dictionary<int, ReturnChonZhiAwardInfo> _recallChongZhiAwardDict = new Dictionary<int, ReturnChonZhiAwardInfo>();

		private Dictionary<int, ReturnData> _returnDic = new Dictionary<int, ReturnData>();

		private long _lastTicks = 0L;
	}
}
