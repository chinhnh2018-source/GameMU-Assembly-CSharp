using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ShenJiFuWenManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static ShenJiFuWenManager getInstance()
		{
			return ShenJiFuWenManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1080, 2, 2, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1081, 1, 1, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1082, 2, 2, ShenJiFuWenManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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
			if (!GlobalNew.IsGongNengOpened(client, 70, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1080:
					result = this.ProcessShenJiAddEffectCmd(client, nID, bytes, cmdParams);
					break;
				case 1081:
					result = this.ProcessShenJiAddExpCmd(client, nID, bytes, cmdParams);
					break;
				case 1082:
					result = this.ProcessShenJiWashCmd(client, nID, bytes, cmdParams);
					break;
				default:
					result = true;
					break;
				}
			}
			return result;
		}

		public void OnLogin(GameClient client)
		{
			this.RefreshShenJiFuWenProps(client);
		}

		private void RefreshShenJiFuWenProps(GameClient client)
		{
			Dictionary<int, ShenJiFuWenConfigData> dictionary = null;
			lock (this.ConfigMutex)
			{
				dictionary = this.ShenJiConfig;
			}
			double[] array = new double[177];
			foreach (ShenJiFuWenData shenJiFuWenData in client.ClientData.ShenJiDataDict.Values)
			{
				ShenJiFuWenConfigData shenJiFuWenConfigData = null;
				if (dictionary.TryGetValue(shenJiFuWenData.ShenJiID, out shenJiFuWenConfigData))
				{
					ShenJiFuWenEffectData effect = shenJiFuWenConfigData.GetEffect(shenJiFuWenData.Level);
					for (int i = 0; i < array.Length; i++)
					{
						array[i] += effect.ExtProps[i];
					}
				}
			}
			client.ClientData.PropsCacheManager.SetExtProps(new object[]
			{
				PropsSystemTypes.ShenJiFuWen,
				array
			});
			client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
			{
				default(DelayExecProcIds),
				2
			});
		}

		private ShenJiPointConfigData GetShenJiPointConfigInfo(GameClient client)
		{
			int costShenJiPointNum = this.GetCostShenJiPointNum(client);
			int shenJiPointValue = GameManager.ClientMgr.GetShenJiPointValue(client);
			List<ShenJiPointConfigData> list = null;
			lock (this.ConfigMutex)
			{
				list = this.ShenJiPointConfig;
			}
			ShenJiPointConfigData result;
			if (costShenJiPointNum + shenJiPointValue >= list.Count - 1)
			{
				result = null;
			}
			else
			{
				result = list[costShenJiPointNum + shenJiPointValue];
			}
			return result;
		}

		private int GetCostShenJiPointNum(GameClient client)
		{
			Dictionary<int, ShenJiFuWenConfigData> dictionary = null;
			lock (this.ConfigMutex)
			{
				dictionary = this.ShenJiConfig;
			}
			int num = 0;
			foreach (ShenJiFuWenData shenJiFuWenData in client.ClientData.ShenJiDataDict.Values)
			{
				ShenJiFuWenConfigData shenJiFuWenConfigData = null;
				if (dictionary.TryGetValue(shenJiFuWenData.ShenJiID, out shenJiFuWenConfigData))
				{
					num += shenJiFuWenConfigData.UpNeed * shenJiFuWenData.Level;
				}
			}
			return num;
		}

		public int GetAllShenJiPointNum(GameClient client)
		{
			return this.GetCostShenJiPointNum(client) + GameManager.ClientMgr.GetShenJiPointValue(client);
		}

		private ShenJiFuWenData GetShenJiFuWenData(GameClient client, int shenjiID)
		{
			ShenJiFuWenData result;
			if (null == client.ClientData.ShenJiDataDict)
			{
				result = null;
			}
			else
			{
				ShenJiFuWenData shenJiFuWenData = null;
				if (!client.ClientData.ShenJiDataDict.TryGetValue(shenjiID, out shenJiFuWenData))
				{
					result = null;
				}
				else
				{
					result = shenJiFuWenData;
				}
			}
			return result;
		}

		private bool UpdateShenJiFuWenDataDB(GameClient client, int shenjiID, int lev)
		{
			return Global.sendToDB<bool, string>(13095, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, shenjiID, lev), client.ServerId);
		}

		private bool ClearShenJiFuWenDataDB(GameClient client)
		{
			return Global.sendToDB<bool, string>(13096, string.Format("{0}", client.ClientData.RoleID), client.ServerId);
		}

		public bool ProcessShenJiAddEffectCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int num3 = Convert.ToInt32(cmdParams[1]);
				Dictionary<int, ShenJiFuWenConfigData> dictionary = null;
				lock (this.ConfigMutex)
				{
					dictionary = this.ShenJiConfig;
				}
				ShenJiFuWenConfigData shenJiFuWenConfigData = null;
				if (!dictionary.TryGetValue(num3, out shenJiFuWenConfigData))
				{
					num = 1;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				ShenJiFuWenData shenJiFuWenData = this.GetShenJiFuWenData(client, num3);
				if (shenJiFuWenData != null && shenJiFuWenData.Level >= shenJiFuWenConfigData.MaxLevel)
				{
					num = 2;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				if (shenJiFuWenConfigData.UpNeed > GameManager.ClientMgr.GetShenJiPointValue(client))
				{
					num = 2;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						num2,
						num3,
						0
					}), false);
					return true;
				}
				ShenJiFuWenConfigData shenJiFuWenConfigData2 = null;
				if (dictionary.TryGetValue(shenJiFuWenConfigData.PreShenJiID, out shenJiFuWenConfigData2))
				{
					ShenJiFuWenData shenJiFuWenData2 = this.GetShenJiFuWenData(client, shenJiFuWenConfigData.PreShenJiID);
					if (shenJiFuWenData2 == null || shenJiFuWenConfigData.PreShenJiLev > shenJiFuWenData2.Level)
					{
						num = 2;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							num2,
							num3,
							0
						}), false);
						return true;
					}
				}
				if (null == shenJiFuWenData)
				{
					shenJiFuWenData = new ShenJiFuWenData
					{
						ShenJiID = num3
					};
					client.ClientData.ShenJiDataDict[num3] = shenJiFuWenData;
				}
				GameManager.ClientMgr.ModifyShenJiPointValue(client, -shenJiFuWenConfigData.UpNeed, "精灵神迹升级|激活", true, true);
				shenJiFuWenData.Level++;
				this.UpdateShenJiFuWenDataDB(client, num3, shenJiFuWenData.Level);
				this.RefreshShenJiFuWenProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					num2,
					num3,
					shenJiFuWenData.Level
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessShenJiAddExpCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				if (GameManager.ClientMgr.GetShenJiJiFenValue(client) <= 0)
				{
					num = 4;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
					return true;
				}
				ShenJiPointConfigData shenJiPointConfigInfo = this.GetShenJiPointConfigInfo(client);
				if (null == shenJiPointConfigInfo)
				{
					num = 6;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
					return true;
				}
				if (Global.GetUnionLevel2(client.ClientData.ChangeLifeCount, client.ClientData.Level) < shenJiPointConfigInfo.NeedLevel)
				{
					num = 7;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
					return true;
				}
				int shenJiJiFenValue = GameManager.ClientMgr.GetShenJiJiFenValue(client);
				int shenJiJiFenAddValue = GameManager.ClientMgr.GetShenJiJiFenAddValue(client);
				int num3 = shenJiPointConfigInfo.NeedJiFen - shenJiJiFenAddValue;
				if (shenJiJiFenValue >= num3)
				{
					GameManager.ClientMgr.ModifyShenJiJiFenValue(client, -num3, "精灵神迹积分注入", true, true);
					GameManager.ClientMgr.ModifyShenJiJiFenAddValue(client, -shenJiJiFenAddValue, "精灵神迹积分注入", true, true);
					GameManager.ClientMgr.ModifyShenJiPointValue(client, 1, "精灵神迹积分注入", true, true);
				}
				else
				{
					GameManager.ClientMgr.ModifyShenJiJiFenValue(client, -shenJiJiFenValue, "精灵神迹积分注入", true, true);
					GameManager.ClientMgr.ModifyShenJiJiFenAddValue(client, shenJiJiFenValue, "精灵神迹积分注入", true, true);
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessShenJiWashCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				ShenJiWashType shenJiWashType = (ShenJiWashType)Convert.ToInt32(cmdParams[1]);
				int costShenJiPointNum = this.GetCostShenJiPointNum(client);
				if (costShenJiPointNum <= 0)
				{
					num = 1;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
					return true;
				}
				if (shenJiWashType == ShenJiWashType.SJWT_UseZuanShi)
				{
					int subMoney = Math.Min(costShenJiPointNum * this.WashCostPerOne, this.MaxWashCost);
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subMoney, "精灵神迹洗点", true, true, false, DaiBiSySType.None))
					{
						num = 3;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
				}
				else
				{
					GoodsData goodsByID = Global.GetGoodsByID(client, this.WashGoodsID);
					if (goodsByID == null)
					{
						num = 5;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByID, 1, false, false))
					{
						num = 5;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
						return true;
					}
				}
				GameManager.ClientMgr.ModifyShenJiPointValue(client, costShenJiPointNum, "精灵神迹点重置", true, true);
				this.ClearShenJiFuWenDataDB(client);
				client.ClientData.ShenJiDataDict.Clear();
				this.RefreshShenJiFuWenProps(client);
				client.sendCmd(nID, string.Format("{0}:{1}", num, num2), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ResettingShenJiFuWen");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					','
				});
				if (array.Length == 3)
				{
					this.WashGoodsID = Global.SafeConvertToInt32(array[0]);
					this.WashCostPerOne = Global.SafeConvertToInt32(array[1]);
					this.MaxWashCost = Global.SafeConvertToInt32(array[2]);
				}
			}
			return this.LoadShenJiFuWenConfigFile() && this.LoadShenJiPointConfigFile();
		}

		private ShenJiFuWenEffectData ParseShenJiFuWenEffectData(XElement xmlItem, string Key)
		{
			string safeAttributeStr = Global.GetSafeAttributeStr(xmlItem, string.Format("Effect{0}", Key));
			string[] array = safeAttributeStr.Split(new char[]
			{
				'|'
			});
			ShenJiFuWenEffectData result;
			if (array.Length == 0)
			{
				result = null;
			}
			else
			{
				ShenJiFuWenEffectData shenJiFuWenEffectData = new ShenJiFuWenEffectData();
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length == 2)
					{
						ExtPropIndexes propIndexByPropName = ConfigParser.GetPropIndexByPropName(array3[0]);
						if (propIndexByPropName != ExtPropIndexes.Max)
						{
							shenJiFuWenEffectData.ExtProps[(int)propIndexByPropName] = Global.SafeConvertToDouble(array3[1]);
						}
					}
				}
				result = shenJiFuWenEffectData;
			}
			return result;
		}

		public bool LoadShenJiFuWenConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ShenJiFuWen.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ShenJiFuWen.xml"));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, ShenJiFuWenConfigData> dictionary = new Dictionary<int, ShenJiFuWenConfigData>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					ShenJiFuWenConfigData shenJiFuWenConfigData = new ShenJiFuWenConfigData();
					shenJiFuWenConfigData.ShenJiID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
					shenJiFuWenConfigData.PreShenJiID = (int)Global.GetSafeAttributeLong(xelement2, "Prev");
					shenJiFuWenConfigData.PreShenJiLev = (int)Global.GetSafeAttributeLong(xelement2, "PrevLevel");
					shenJiFuWenConfigData.MaxLevel = (int)Global.GetSafeAttributeLong(xelement2, "MaxLevel");
					shenJiFuWenConfigData.UpNeed = (int)Global.GetSafeAttributeLong(xelement2, "UpNeed");
					for (int i = 1; i <= 5; i++)
					{
						ShenJiFuWenEffectData shenJiFuWenEffectData = this.ParseShenJiFuWenEffectData(xelement2, i.ToString());
						if (null == shenJiFuWenEffectData)
						{
							break;
						}
						shenJiFuWenConfigData.ShenJiEffectList.Add(shenJiFuWenEffectData);
					}
					dictionary[shenJiFuWenConfigData.ShenJiID] = shenJiFuWenConfigData;
				}
				lock (this.ConfigMutex)
				{
					this.ShenJiConfig = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/ShenJiFuWen.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadShenJiPointConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath("Config/ShenJiDian.xml"));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath("Config/ShenJiDian.xml"));
				if (null == xelement)
				{
					return false;
				}
				List<ShenJiPointConfigData> list = new List<ShenJiPointConfigData>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					ShenJiPointConfigData shenJiPointConfigData = new ShenJiPointConfigData();
					shenJiPointConfigData.ShenJiPoint = (int)Global.GetSafeAttributeLong(xml, "ShenJiDian");
					shenJiPointConfigData.NeedJiFen = (int)Global.GetSafeAttributeLong(xml, "NeedShenJi");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "NeedLevel");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					if (array.Length == 2)
					{
						int zhuansheng = Global.SafeConvertToInt32(array[0]);
						int level = Global.SafeConvertToInt32(array[1]);
						shenJiPointConfigData.NeedLevel = Global.GetUnionLevel2(zhuansheng, level);
					}
					list.Add(shenJiPointConfigData);
				}
				lock (this.ConfigMutex)
				{
					this.ShenJiPointConfig = list;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", "Config/ShenJiDian.xml", ex.Message), null, true);
				return false;
			}
			return true;
		}

		private const string ShenJi_FuWenFileName = "Config/ShenJiFuWen.xml";

		private const string ShenJi_PointFileName = "Config/ShenJiDian.xml";

		private object ConfigMutex = new object();

		protected Dictionary<int, ShenJiFuWenConfigData> ShenJiConfig = null;

		protected List<ShenJiPointConfigData> ShenJiPointConfig = null;

		protected int WashGoodsID = 0;

		protected int WashCostPerOne = 0;

		protected int MaxWashCost = 0;

		private static ShenJiFuWenManager instance = new ShenJiFuWenManager();
	}
}
