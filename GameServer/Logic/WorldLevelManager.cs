using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	internal class WorldLevelManager
	{
		private WorldLevelManager()
		{
		}

		public static WorldLevelManager getInstance()
		{
			return WorldLevelManager.instance;
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.Mutex)
			{
				try
				{
					Dictionary<string, Tuple<int, int>> dictionary = new Dictionary<string, Tuple<int, int>>();
					Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
					text = "Config/JieRiGifts/JieRiLvType.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "ID");
						int item = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
						int item2 = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
						dictionary.Add(safeAttributeStr, new Tuple<int, int>(item, item2));
					}
					text = "Config/JieRiGifts/JieRiLv.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "NeedFile");
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "JiRiLv");
						string safeAttributeStr4 = Global.GetSafeAttributeStr(xml, "JieRiFile");
						if (!(safeAttributeStr2 == safeAttributeStr4))
						{
							Dictionary<string, string> dictionary3;
							if (!dictionary2.TryGetValue(safeAttributeStr3, out dictionary3))
							{
								dictionary3 = new Dictionary<string, string>();
								dictionary2[safeAttributeStr3] = dictionary3;
							}
							dictionary3[safeAttributeStr2] = safeAttributeStr4;
						}
					}
					this.JieRiLvTypeDict = dictionary;
					this.JieRiLvDict = dictionary2;
					this.JieRiStartDay = 0;
					this.JieRiWorldLevel = 0;
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		private string CalcJieRiLv()
		{
			string result = "";
			int offsetDay = TimeUtil.GetOffsetDay(Global.GetJieriStartDay());
			if (this.JieRiStartDay != offsetDay)
			{
				ServerDayData serverDayData = Global.sendToDB<ServerDayData, int>(11004, offsetDay, GameManager.ServerId);
				lock (this.Mutex)
				{
					if (serverDayData != null && serverDayData.Dayid == offsetDay)
					{
						this.JieRiWorldLevel = serverDayData.WorldLevel;
						this.JieRiStartDay = offsetDay;
					}
				}
			}
			lock (this.Mutex)
			{
				if (this.JieRiWorldLevel > 0)
				{
					foreach (KeyValuePair<string, Tuple<int, int>> keyValuePair in this.JieRiLvTypeDict)
					{
						if (this.JieRiWorldLevel >= keyValuePair.Value.Item1 && this.JieRiWorldLevel <= keyValuePair.Value.Item2)
						{
							result = keyValuePair.Key;
							break;
						}
					}
				}
			}
			return result;
		}

		public string GetJieRiConfigFileName(string filename)
		{
			string result;
			if (filename.Length <= Global.AbsoluteGameResPath.Length)
			{
				result = filename;
			}
			else
			{
				string text = filename.Substring(Global.AbsoluteGameResPath.Length);
				string fileName = Path.GetFileName(filename);
				string text2 = this.CalcJieRiLv();
				if (!string.IsNullOrEmpty(text2))
				{
					lock (this.Mutex)
					{
						Dictionary<string, string> dictionary;
						if (this.JieRiLvDict.TryGetValue(text2, out dictionary))
						{
							string str;
							if (dictionary.TryGetValue(text, out str))
							{
								string str2 = filename.Substring(0, filename.Length - text.Length);
								return str2 + str;
							}
							if (dictionary.TryGetValue(fileName, out str))
							{
								string str2 = filename.Substring(0, filename.Length - fileName.Length);
								return str2 + str;
							}
						}
					}
				}
				result = filename;
			}
			return result;
		}

		public void ResetWorldLevel()
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (this.m_nResetWorldLevelDayID != dayOfYear)
			{
				int offsetDayNow = TimeUtil.GetOffsetDayNow();
				string cdate = TimeUtil.GetRealDate(offsetDayNow).Date.ToString("yyyy-MM-dd");
				ServerDayData serverDayData = Global.sendToDB<ServerDayData, int>(11004, offsetDayNow, GameManager.ServerId);
				if (serverDayData != null && serverDayData.Dayid == offsetDayNow)
				{
					LogManager.WriteLog(0, string.Format("从数据加载世界等级:day={0},worldlevel={1}", serverDayData.CDate, serverDayData.WorldLevel), null, true);
					this.m_nWorldLevel = serverDayData.WorldLevel;
					this.m_nResetWorldLevelDayID = dayOfYear;
				}
				else
				{
					TCPOutPacket tcpoutPacket = null;
					string strcmd = string.Format("{0}:{1}", 0, 5);
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, TCPOutPacketPool.getInstance(), 269, strcmd, out tcpoutPacket, 0);
					if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
					{
						LogManager.WriteLog(2, "世界等级装入异常", null, true);
					}
					else
					{
						int nResetWorldLevelDayID = this.m_nResetWorldLevelDayID;
						this.m_nResetWorldLevelDayID = dayOfYear;
						PaiHangData paiHangData = DataHelper.BytesToObject<PaiHangData>(tcpoutPacket.GetPacketBytes(), 6, tcpoutPacket.PacketDataSize - 6);
						if (null != paiHangData)
						{
							int num = 0;
							int num2 = 0;
							if (null != paiHangData.PaiHangList)
							{
								int i = 0;
								while (i < 100 && i < paiHangData.PaiHangList.Count)
								{
									num2++;
									num += paiHangData.PaiHangList[i].Val2 * 100 + paiHangData.PaiHangList[i].Val1;
									i++;
								}
							}
							this.m_nWorldLevel = ((num2 > 0) ? (num / num2) : 1);
							serverDayData = new ServerDayData
							{
								Dayid = offsetDayNow,
								CDate = cdate,
								WorldLevel = this.m_nWorldLevel
							};
							for (;;)
							{
								int num3 = Global.sendToDB<int, ServerDayData>(11003, serverDayData, GameManager.ServerId);
								if (num3 >= 0)
								{
									break;
								}
								Thread.Sleep(1000);
							}
							if (0 != nResetWorldLevelDayID)
							{
								int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
								for (int i = 0; i < maxClientCount; i++)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
									if (null != gameClient)
									{
										this.UpddateWorldLevelBuff(gameClient);
									}
								}
							}
						}
						else
						{
							LogManager.WriteLog(2, "世界等级装入时，获取等级排行榜失败", null, true);
						}
					}
				}
			}
		}

		public void UpddateWorldLevelBuff(GameClient client)
		{
			int num = client.ClientData.GetRoleData().ChangeLifeCount * 100 + client.ClientData.GetRoleData().Level;
			double num2 = Math.Round((double)(this.m_nWorldLevel - num) / 100.0, 2) * GameManager.systemParamsList.GetParamValueDoubleByName("WorldLevel", 0.0);
			int num3 = (int)(num2 * 100.0);
			int num4 = -1;
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 99);
			if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
			{
				num4 = (int)bufferDataByID.BufferVal;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 != num3)
			{
				Global.UpdateBufferData(client, BufferItemTypes.MU_WORLDLEVEL, new double[]
				{
					(double)num3
				}, 1, true);
				client.ClientData.nTempWorldLevelPer = num2;
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
		}

		private object Mutex = new object();

		public int m_nWorldLevel = 0;

		public int m_nResetWorldLevelDayID = 0;

		public int JieRiStartDay;

		public int JieRiWorldLevel;

		private Dictionary<string, Tuple<int, int>> JieRiLvTypeDict = new Dictionary<string, Tuple<int, int>>();

		private Dictionary<string, Dictionary<string, string>> JieRiLvDict = new Dictionary<string, Dictionary<string, string>>();

		private static WorldLevelManager instance = new WorldLevelManager();
	}
}
