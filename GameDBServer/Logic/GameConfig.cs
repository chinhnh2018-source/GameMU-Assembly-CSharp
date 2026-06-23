using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Protocol;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class GameConfig
	{
		public void InitGameDBManagerFlags(bool init = false)
		{
			GameDBManager.Flag_t_goods_delete_immediately = (GameDBManager.GameConfigMgr.GetGameConfigItemInt("flag_t_goods_delete_immediately", 1) > 0);
			GameDBManager.Flag_Query_Total_UserMoney_Minute = GameDBManager.GameConfigMgr.GetGameConfigItemInt("query_total_usermoney_minute", 60);
			GameDBManager.PreDeleteRoleDelaySeconds = GameDBManager.GameConfigMgr.GetGameConfigItemInt("DeleteRoleNeedTime", 120) * 60;
			GameDBManager.DisableSomeLog = ((GameDBManager.GameConfigMgr.GetGameConfigItemInt("opflags", 0) & 1) != 0);
			GameDBManager.PTID = GameDBManager.GameConfigMgr.GetGameConfigItemInt("ptid", 0);
			BangHuiNumLevelMgr.MaxQueryTimeSlotTicks = (long)(GameDBManager.GameConfigMgr.GetGameConfigItemInt("banghuiproctime1", 60) * 1000 * 10000);
			string gameConfigItemStr = GameDBManager.GameConfigMgr.GetGameConfigItemStr("rolepaihangkeys", "yinliang,combatforce,killboss");
			if (!string.IsNullOrEmpty(gameConfigItemStr))
			{
				lock (this.RolePaiHangKeys)
				{
					this.RolePaiHangKeys.Clear();
					string[] array = gameConfigItemStr.Split(new char[]
					{
						','
					});
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							this.RolePaiHangKeys.Add(text);
						}
					}
				}
			}
			if (!this.Initialized)
			{
				this.Initialized = true;
				GameDBManager.Flag_Splite_RoleParams_Table = GameDBManager.GameConfigMgr.GetGameConfigItemInt("opt_roleparams", 1);
			}
		}

		public bool IsPaiHangKey(string key)
		{
			bool result;
			if (string.IsNullOrEmpty(key))
			{
				result = false;
			}
			else
			{
				lock (this.RolePaiHangKeys)
				{
					result = this.RolePaiHangKeys.Contains(key);
				}
			}
			return result;
		}

		public void LoadGameConfigFromDB(DBManager dbMgr)
		{
			this._GameConfigDict = DBQuery.QueryGameConfigDict(dbMgr);
			if (null == this._GameConfigDict)
			{
				this._GameConfigDict = new Dictionary<string, string>();
			}
			if (!string.IsNullOrEmpty(GameDBManager.serverDBInfo.ServerKey))
			{
				this._GameConfigDict["loginwebkey"] = GameDBManager.serverDBInfo.ServerKey;
			}
			this.InitGameDBManagerFlags(false);
		}

		public void UpdateGameConfigItem(string paramName, string paramValue)
		{
			lock (this._GameConfigDict)
			{
				this._GameConfigDict[paramName] = paramValue;
			}
			this.InitGameDBManagerFlags(false);
		}

		public string GetGameConifgItem(string paramName)
		{
			string result = null;
			lock (this._GameConfigDict)
			{
				if (!this._GameConfigDict.TryGetValue(paramName, out result))
				{
					result = null;
				}
			}
			return result;
		}

		public string GetGameConfigItemStr(string paramName, string defVal)
		{
			string gameConifgItem = this.GetGameConifgItem(paramName);
			string result;
			if (string.IsNullOrEmpty(gameConifgItem))
			{
				result = defVal;
			}
			else
			{
				result = gameConifgItem;
			}
			return result;
		}

		public int GetGameConfigItemInt(string paramName, int defVal)
		{
			string gameConifgItem = this.GetGameConifgItem(paramName);
			int result;
			if (string.IsNullOrEmpty(gameConifgItem))
			{
				result = defVal;
			}
			else
			{
				int num = 0;
				try
				{
					num = Convert.ToInt32(gameConifgItem);
				}
				catch (Exception)
				{
					num = defVal;
				}
				result = num;
			}
			return result;
		}

		public double GetGameConfigItemInt(string paramName, double defVal)
		{
			string gameConifgItem = this.GetGameConifgItem(paramName);
			double result;
			if (string.IsNullOrEmpty(gameConifgItem))
			{
				result = defVal;
			}
			else
			{
				double num = 0.0;
				try
				{
					num = Convert.ToDouble(gameConifgItem);
				}
				catch (Exception)
				{
					num = defVal;
				}
				result = num;
			}
			return result;
		}

		public TCPOutPacket GetGameConfigDictTCPOutPacket(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result = null;
			lock (this._GameConfigDict)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<string, string>>(this._GameConfigDict, pool, cmdID);
			}
			return result;
		}

		private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();

		private bool Initialized = false;

		private HashSet<string> RolePaiHangKeys = new HashSet<string>();
	}
}
