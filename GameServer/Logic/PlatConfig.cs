using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class PlatConfig
	{
		public void LoadPlatConfig()
		{
			string text = Global.GameResPath(this.fileName);
			this._PlatConfigNormalDict = new Dictionary<string, string>();
			this._PlatConfigWaitingDict = new Dictionary<int, WaitingConfig>();
			this._PlatConfigTradeLevelLimitList = new List<TradeLevelLimitConfig>();
			this._PlatConfigChatLevelLimitDic = new Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>>();
			try
			{
				XElement xml = XElement.Load(text);
				this.LoadNormalConfig(xml, this._PlatConfigNormalDict);
				this.LoadWaitingConfig(xml, this._PlatConfigWaitingDict);
				this.LoadTradeLevelLimitConfig(xml, this._PlatConfigTradeLevelLimitList);
				this.LoadChatLevelLimitConfig(xml, this._PlatConfigChatLevelLimitDic);
				this._PlatConfigTradeLimitConfigDict = this.LoadTradeLimitsConfig(xml);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "区域平台特殊配置文件加载失败" + text + "\r\n" + ex.ToString(), ex, false);
			}
		}

		public int ReloadPlatConfig()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<int, WaitingConfig> dictionary2 = new Dictionary<int, WaitingConfig>();
			List<TradeLevelLimitConfig> list = new List<TradeLevelLimitConfig>();
			Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>> dictionary3 = new Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>>();
			try
			{
				XElement xml = XElement.Load(Global.GameResPath(this.fileName));
				this.LoadNormalConfig(xml, dictionary);
				this.LoadWaitingConfig(xml, dictionary2);
				this.LoadTradeLevelLimitConfig(xml, list);
				this.LoadChatLevelLimitConfig(xml, dictionary3);
				this._PlatConfigTradeLimitConfigDict = this.LoadTradeLimitsConfig(xml);
			}
			catch (Exception ex)
			{
				LogManager.WriteException("重新加载配置文件 PlatConfig.xml  失败！！！" + ex.ToString());
				return -1;
			}
			lock (this._PlatConfigNormalDict)
			{
				this._PlatConfigNormalDict = dictionary;
			}
			lock (this._PlatConfigWaitingDict)
			{
				this._PlatConfigWaitingDict = dictionary2;
			}
			lock (this._PlatConfigTradeLevelLimitList)
			{
				this._PlatConfigTradeLevelLimitList = list;
			}
			lock (this._PlatConfigChatLevelLimitDic)
			{
				this._PlatConfigChatLevelLimitDic = dictionary3;
			}
			TCPSession.SetMaxPosCmdNumPer5Seconds(8);
			GameManager.loginWaitLogic.LoadConfig();
			return 0;
		}

		private void LoadNormalConfig(XElement xml, Dictionary<string, string> normalDict)
		{
			lock (normalDict)
			{
				try
				{
					XElement xml2 = Global.GetSafeXElement(xml, "DCLogs").Element("DCLog");
					normalDict.Add("tw_log_pid", Global.GetSafeAttributeStr(xml2, "pid"));
					normalDict.Add("tw_log_path", Global.GetSafeAttributeStr(xml2, "path"));
					normalDict.Add("tw_log_head", Global.GetSafeAttributeStr(xml2, "logHead"));
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "DCLog") + ex.ToString());
				}
				try
				{
					XElement xml2 = Global.GetSafeXElement(xml, "GuestTradeLevelLimits").Element("GuestTradeLevelLimit");
					normalDict.Add("GuestTradeLevelLimit", Global.GetSafeAttributeStr(xml2, "Limit"));
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "DCLog") + ex.ToString());
				}
				try
				{
					XElement xml2 = Global.GetSafeXElement(xml, "FileBans").Element("FileBanPros");
					normalDict.Add("fileban_hour", Global.GetSafeAttributeStr(xml2, "FileBanHour"));
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "FileBanPros") + ex.ToString());
				}
				try
				{
					XElement xml2 = Global.GetSafeXElement(xml, "Speeds").Element("Speed");
					normalDict.Add("ban-speed-up-minutes2", Global.GetSafeAttributeStr(xml2, "BanMins"));
					normalDict.Add("maxposcmdnum", Global.GetSafeAttributeStr(xml2, "MaxPosCmdNum"));
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "Speed") + ex.ToString());
				}
				try
				{
					IEnumerable<XElement> enumerable = xml.DescendantsAndSelf("addSettings");
					foreach (XElement xelement in enumerable)
					{
						foreach (XElement xml3 in xelement.DescendantsAndSelf("add"))
						{
							normalDict[Global.GetSafeAttributeStr(xml3, "key")] = Global.GetSafeAttributeStr(xml3, "value");
						}
					}
					enumerable = xml.DescendantsAndSelf("addSettings");
					foreach (XElement xelement in enumerable)
					{
						if (0 == string.Compare(ConfigHelper.GetElementAttributeValue(xelement, "platfromtype", ""), GameManager.PlatformType.ToString(), true))
						{
							foreach (XElement xml3 in xelement.DescendantsAndSelf("add"))
							{
								normalDict[Global.GetSafeAttributeStr(xml3, "key")] = Global.GetSafeAttributeStr(xml3, "value");
							}
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！", this.fileName, "addSettings") + ex.ToString());
				}
				foreach (KeyValuePair<string, string> keyValuePair in normalDict)
				{
					if (this.SyncDBConfigNames.Contains(keyValuePair.Key) && !string.IsNullOrEmpty(keyValuePair.Value))
					{
						GameManager.GameConfigMgr.UpdateGameConfigItem(keyValuePair.Key, keyValuePair.Value, false);
					}
				}
			}
		}

		public string GetGameConfigItemStr(string paramName, string defVal)
		{
			string text = GameManager.GameConfigMgr.GetGameConifgItem(paramName);
			if (text == null)
			{
				if (paramName.Equals("trade_level_limit"))
				{
					text = this.GetPlatTradeLevelLimitConfig(paramName);
				}
				else if (paramName.Equals("userwaitconfig") || paramName.Equals("vipwaitconfig") || paramName.Equals("loginallow_vipexp"))
				{
					text = this.GetWaitingConfig(paramName);
				}
				else if (paramName.Equals("chat_world_level") || paramName.Equals("chat_family_level") || paramName.Equals("chat_team_level") || paramName.Equals("chat_private_level") || paramName.Equals("chat_near_level"))
				{
					text = this.GetPlatChatLevelLimitConfig(paramName);
				}
				else
				{
					text = this.GetNormalConfig(paramName);
				}
			}
			return (!string.IsNullOrEmpty(text)) ? text : defVal;
		}

		public int GetGameConfigItemInt(string paramName, int defVal)
		{
			string text = GameManager.GameConfigMgr.GetGameConifgItem(paramName);
			int result = 0;
			if (null == text)
			{
				if (paramName.Equals("trade_level_limit"))
				{
					text = this.GetPlatTradeLevelLimitConfig(paramName);
				}
				else if (paramName.Equals("userwaitconfig") || paramName.Equals("vipwaitconfig") || paramName.Equals("loginallow_vipexp"))
				{
					text = this.GetWaitingConfig(paramName);
				}
				else if (paramName.Equals("chat_world_level") || paramName.Equals("chat_family_level") || paramName.Equals("chat_team_level") || paramName.Equals("chat_private_level") || paramName.Equals("chat_near_level"))
				{
					text = this.GetPlatChatLevelLimitConfig(paramName);
				}
				else
				{
					text = this.GetNormalConfig(paramName);
				}
			}
			try
			{
				if (string.IsNullOrEmpty(text))
				{
					return defVal;
				}
				result = Convert.ToInt32(text);
			}
			catch (Exception ex)
			{
				return defVal;
			}
			return result;
		}

		private string GetNormalConfig(string paramName)
		{
			string result = null;
			lock (this._PlatConfigNormalDict)
			{
				if (!this._PlatConfigNormalDict.TryGetValue(paramName, out result))
				{
					result = null;
				}
			}
			return result;
		}

		private string GetWaitingConfig(string paramName)
		{
			WaitingConfig waitingConfig = null;
			lock (this._PlatConfigWaitingDict)
			{
				if (!this._PlatConfigWaitingDict.TryGetValue(GameManager.ServerId, out waitingConfig))
				{
					this._PlatConfigWaitingDict.TryGetValue(0, out waitingConfig);
				}
			}
			if (waitingConfig != null)
			{
				if (paramName.Equals("userwaitconfig"))
				{
					return waitingConfig.UserWaitConfig;
				}
				if (paramName.Equals("vipwaitconfig"))
				{
					return waitingConfig.VIPWaitConfig;
				}
				if (paramName.Equals("loginallow_vipexp"))
				{
					return waitingConfig.LoginAllow_VIPExp.ToString();
				}
			}
			return null;
		}

		private string GetPlatTradeLevelLimitConfig(string paramName)
		{
			string result = null;
			IEnumerable<TradeLevelLimitConfig> enumerable = null;
			lock (this._PlatConfigTradeLevelLimitList)
			{
				enumerable = from items in this._PlatConfigTradeLevelLimitList
				orderby items.Day
				select items;
			}
			DateTime dateTime = TimeUtil.NowDateTime();
			DateTime kaiFuTime = Global.GetKaiFuTime();
			int daysSpanNum = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), kaiFuTime, true);
			foreach (TradeLevelLimitConfig tradeLevelLimitConfig in enumerable)
			{
				result = tradeLevelLimitConfig.Limit;
				if (daysSpanNum <= tradeLevelLimitConfig.Day)
				{
					break;
				}
			}
			return result;
		}

		public TradeLimitConfig GetTradeLimitConfig()
		{
			if (null != this._PlatConfigTradeLimitConfigDict)
			{
				string key = GameCoreInterface.getinstance().GetPlatformType().ToString().ToLower();
				lock (this._PlatConfigTradeLimitConfigDict)
				{
					TradeLimitConfig result;
					if (this._PlatConfigTradeLimitConfigDict.TryGetValue(key, out result))
					{
						return result;
					}
				}
			}
			return null;
		}

		public bool CanTrade(DateTime now, int realMoney, int level)
		{
			bool result;
			if (null == this._PlatConfigTradeLimitConfigDict)
			{
				result = true;
			}
			else
			{
				string key = GameCoreInterface.getinstance().GetPlatformType().ToString().ToLower();
				lock (this._PlatConfigTradeLimitConfigDict)
				{
					TradeLimitConfig tradeLimitConfig;
					if (!this._PlatConfigTradeLimitConfigDict.TryGetValue(key, out tradeLimitConfig))
					{
						result = true;
					}
					else if (tradeLimitConfig.ZuanShiOpen != 1)
					{
						result = false;
					}
					else if (now < tradeLimitConfig.StartTime || now > tradeLimitConfig.EndTime)
					{
						result = true;
					}
					else if (realMoney >= tradeLimitConfig.ZuanShiLimit || level >= tradeLimitConfig.LevelLimit)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private string GetPlatChatLevelLimitConfig(string paramName)
		{
			string text = null;
			ChatTypeIndexes chatKeyName = this.GetChatKeyName(paramName);
			List<ChatLevelLimitConfig> list = null;
			lock (this._PlatConfigChatLevelLimitDic)
			{
				list = this._PlatConfigChatLevelLimitDic[chatKeyName];
			}
			string result;
			if (list == null)
			{
				result = null;
			}
			else
			{
				IOrderedEnumerable<ChatLevelLimitConfig> orderedEnumerable = from items in list
				orderby items.Day
				select items;
				DateTime dateTime = TimeUtil.NowDateTime();
				DateTime kaiFuTime = Global.GetKaiFuTime();
				int daysSpanNum = Global.GetDaysSpanNum(TimeUtil.NowDateTime(), kaiFuTime, true);
				foreach (ChatLevelLimitConfig chatLevelLimitConfig in orderedEnumerable)
				{
					text = chatLevelLimitConfig.Limit;
					if (daysSpanNum <= chatLevelLimitConfig.Day)
					{
						break;
					}
				}
				try
				{
					string[] array = text.Split(new char[]
					{
						','
					});
					int num = Convert.ToInt32(array[0]);
					int num2 = Convert.ToInt32(array[1]);
					text = (num * 100 + num2).ToString();
				}
				catch (Exception ex)
				{
					return null;
				}
				result = text;
			}
			return result;
		}

		private void LoadWaitingConfig(XElement xml, Dictionary<int, WaitingConfig> waitingDict)
		{
			lock (waitingDict)
			{
				try
				{
					XElement safeXElement = Global.GetSafeXElement(xml, "Waiting");
					IEnumerable<XElement> enumerable = safeXElement.Elements();
					foreach (XElement xml2 in enumerable)
					{
						WaitingConfig waitingConfig = new WaitingConfig();
						string safeAttributeStr = Global.GetSafeAttributeStr(xml2, "ID");
						waitingConfig.SeverID = Convert.ToInt32(safeAttributeStr);
						int[] safeAttributeIntArray = Global.GetSafeAttributeIntArray(xml2, "NeedWaitNumber", 2, ',');
						waitingConfig.NormalNeedWaitNumber = safeAttributeIntArray[0];
						waitingConfig.VIPNeedWaitNumber = safeAttributeIntArray[1];
						int[] safeAttributeIntArray2 = Global.GetSafeAttributeIntArray(xml2, "MaxNumber", 2, ',');
						waitingConfig.NormalMaxNumber = safeAttributeIntArray2[0];
						waitingConfig.VIPMaxNumber = safeAttributeIntArray2[1];
						int[] safeAttributeIntArray3 = Global.GetSafeAttributeIntArray(xml2, "WaitingMaxNumber", 2, ',');
						waitingConfig.NormalWaitingMaxNumber = safeAttributeIntArray3[0];
						waitingConfig.VIPWaitingMaxNumber = safeAttributeIntArray3[1];
						int[] safeAttributeIntArray4 = Global.GetSafeAttributeIntArray(xml2, "EnterMinInt", 2, ',');
						waitingConfig.NormalEnterMinInt = safeAttributeIntArray4[0];
						waitingConfig.VIPEnterMinInt = safeAttributeIntArray4[1];
						int[] safeAttributeIntArray5 = Global.GetSafeAttributeIntArray(xml2, "AllowMSecs", 2, ',');
						waitingConfig.NormalAllowMSecs = safeAttributeIntArray5[0];
						waitingConfig.VIPAllowMSecs = safeAttributeIntArray5[1];
						int[] safeAttributeIntArray6 = Global.GetSafeAttributeIntArray(xml2, "LogoutAllowMSecs", 2, ',');
						waitingConfig.NormalLogoutAllowMSecs = safeAttributeIntArray6[0];
						waitingConfig.VIPLogoutAllowMSecs = safeAttributeIntArray6[1];
						waitingConfig.VipExp = Convert.ToInt32(Global.GetSafeAttributeStr(xml2, "vipexp"));
						waitingDict.Add(waitingConfig.SeverID, waitingConfig);
					}
					if (!waitingDict.ContainsKey(0))
					{
						throw new Exception(string.Format("配置文件 {0} 可能没有配置 {1} 项或者没有默认配置项，请正确配置后重新加载文件。", this.fileName, "waiting"));
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！! !  {2}", this.fileName, "Waiting", ex.ToString()));
				}
			}
		}

		private void LoadTradeLevelLimitConfig(XElement xml, List<TradeLevelLimitConfig> tradeLevelLimitList)
		{
			lock (tradeLevelLimitList)
			{
				try
				{
					XElement safeXElement = Global.GetSafeXElement(xml, "TradeLevelLimit");
					IEnumerable<XElement> enumerable = safeXElement.Elements();
					foreach (XElement xml2 in enumerable)
					{
						TradeLevelLimitConfig tradeLevelLimitConfig = new TradeLevelLimitConfig();
						string safeAttributeStr = Global.GetSafeAttributeStr(xml2, "ID");
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml2, "Day");
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml2, "Limit");
						tradeLevelLimitConfig.ID = Convert.ToInt32(safeAttributeStr);
						tradeLevelLimitConfig.Day = Convert.ToInt32(safeAttributeStr2);
						tradeLevelLimitConfig.Limit = safeAttributeStr3;
						tradeLevelLimitList.Add(tradeLevelLimitConfig);
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！ {2}", this.fileName, "TradeLevelLimit", ex.ToString()));
				}
			}
		}

		private Dictionary<string, TradeLimitConfig> LoadTradeLimitsConfig(XElement xml)
		{
			Dictionary<string, TradeLimitConfig> dictionary = new Dictionary<string, TradeLimitConfig>();
			try
			{
				XElement safeXElement = Global.GetSafeXElement(xml, "TradeLimits");
				IEnumerable<XElement> enumerable = safeXElement.Elements();
				foreach (XElement xml2 in enumerable)
				{
					TradeLimitConfig tradeLimitConfig = new TradeLimitConfig();
					tradeLimitConfig.Platform = Global.GetSafeAttributeStr(xml2, "Platform");
					dictionary[tradeLimitConfig.Platform.ToLower()] = tradeLimitConfig;
					int[] safeAttributeIntArray = Global.GetSafeAttributeIntArray(xml2, "LevelLimit", -1, ',');
					tradeLimitConfig.LevelLimit = Global.GetUnionLevel(safeAttributeIntArray[0], safeAttributeIntArray[1], false);
					tradeLimitConfig.ZuanShiLimit = (int)Global.GetSafeAttributeLong(xml2, "ZuanShiLimit");
					tradeLimitConfig.ZuanShiOpen = (int)Global.GetSafeAttributeLong(xml2, "ZuanShiOpen");
					tradeLimitConfig.Message = Global.GetSafeAttributeStr(xml2, "Message");
					if (!DateTime.TryParse(Global.GetSafeAttributeStr(xml2, "BeginTime"), out tradeLimitConfig.StartTime))
					{
						tradeLimitConfig.StartTime = DateTime.MinValue;
					}
					if (!DateTime.TryParse(Global.GetSafeAttributeStr(xml2, "EndTime"), out tradeLimitConfig.EndTime))
					{
						tradeLimitConfig.EndTime = DateTime.MinValue;
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！ {2}", this.fileName, "TradeLevelLimit", ex.ToString()));
			}
			return dictionary;
		}

		private ChatTypeIndexes GetChatKeyName(string name)
		{
			ChatTypeIndexes result = ChatTypeIndexes.Max;
			if (name.Equals("WorldChats") || name.Equals("chat_world_level"))
			{
				result = ChatTypeIndexes.World;
			}
			else if (name.Equals("FamilyChats") || name.Equals("chat_family_level"))
			{
				result = ChatTypeIndexes.Faction;
			}
			else if (name.Equals("TeamChats") || name.Equals("chat_team_level"))
			{
				result = ChatTypeIndexes.Team;
			}
			else if (name.Equals("PrivateChats") || name.Equals("chat_private_level"))
			{
				result = ChatTypeIndexes.Private;
			}
			else if (name.Equals("NearChats") || name.Equals("chat_near_level"))
			{
				result = ChatTypeIndexes.Map;
			}
			return result;
		}

		private void LoadChatLevelLimitConfig(XElement xml, Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>> chatLevelLimitDic)
		{
			lock (chatLevelLimitDic)
			{
				try
				{
					XElement safeXElement = Global.GetSafeXElement(xml, "Chats");
					IEnumerable<XElement> enumerable = safeXElement.Elements();
					foreach (XElement xelement in enumerable)
					{
						ChatTypeIndexes chatKeyName = this.GetChatKeyName(xelement.Name.LocalName);
						if (chatKeyName == ChatTypeIndexes.Max)
						{
							throw new InvalidDataException(string.Format("{0} 不在聊天类型中 ！！！", xelement.Name.LocalName));
						}
						List<ChatLevelLimitConfig> list = new List<ChatLevelLimitConfig>();
						chatLevelLimitDic[chatKeyName] = list;
						foreach (XElement xml2 in xelement.Elements())
						{
							ChatLevelLimitConfig chatLevelLimitConfig = new ChatLevelLimitConfig();
							string safeAttributeStr = Global.GetSafeAttributeStr(xml2, "ID");
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xml2, "Day");
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xml2, "Limit");
							chatLevelLimitConfig.ID = Convert.ToInt32(safeAttributeStr);
							chatLevelLimitConfig.Day = Convert.ToInt32(safeAttributeStr2);
							chatLevelLimitConfig.Limit = safeAttributeStr3;
							list.Add(chatLevelLimitConfig);
						}
					}
				}
				catch (Exception ex)
				{
					throw new Exception(string.Format("加载系统配置参数配置文件:{0}, 失败。{1} 节点配置错误！ {2}", this.fileName, "Chats", ex.ToString()));
				}
			}
		}

		private Dictionary<string, string> _PlatConfigNormalDict = null;

		private Dictionary<int, WaitingConfig> _PlatConfigWaitingDict = null;

		private List<TradeLevelLimitConfig> _PlatConfigTradeLevelLimitList = null;

		private Dictionary<string, TradeLimitConfig> _PlatConfigTradeLimitConfigDict = new Dictionary<string, TradeLimitConfig>();

		private Dictionary<ChatTypeIndexes, List<ChatLevelLimitConfig>> _PlatConfigChatLevelLimitDic = null;

		private string fileName = string.Format("Config/PlatConfig.xml", new object[0]);

		private HashSet<string> SyncDBConfigNames = new HashSet<string>
		{
			"lipinma_v1"
		};
	}
}
