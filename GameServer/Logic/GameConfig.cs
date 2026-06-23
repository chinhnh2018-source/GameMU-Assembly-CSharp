using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class GameConfig
	{
		public void LoadGameConfigFromDBServer()
		{
			this._GameConfigDict = Global.LoadDBGameConfigDict();
			if (null == this._GameConfigDict)
			{
				this._GameConfigDict = new Dictionary<string, string>();
			}
		}

		public void SetGameConfigItem(string paramName, string paramValue)
		{
			lock (this._GameConfigDict)
			{
				this._GameConfigDict[paramName] = paramValue;
			}
			this.ChangeParams(paramName, paramValue);
		}

		public void UpdateGameConfigItem(string paramName, string paramValue, bool force = false)
		{
			lock (this._GameConfigDict)
			{
				string a;
				if (this._GameConfigDict.TryGetValue(paramName, out a))
				{
					if (a == paramValue && !force)
					{
						return;
					}
				}
			}
			this.SetGameConfigItem(paramName, paramValue);
			Global.UpdateDBGameConfigg(paramName, paramValue);
		}

		public void ModifyGameConfigItem(string paramName, int paramValue)
		{
			int num = 0;
			lock (this._GameConfigDict)
			{
				num = this.GetGameConfigItemInt(paramName, 0) + paramValue;
				this._GameConfigDict[paramName] = num.ToString();
			}
			this.ChangeParams(paramName, num.ToString());
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

		public double GetGameConfigItemDouble(string paramName, double defVal)
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

		public void SendAllGameConfigItemsToGM(GameClient client)
		{
			lock (this._GameConfigDict)
			{
				foreach (string text in this._GameConfigDict.Keys)
				{
					string arg = this._GameConfigDict[text];
					string textMsg = string.Format("{0}={1}", text, arg);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
				}
			}
		}

		private void ChangeParams(string paramName, string paramValue)
		{
			bool flag = false;
			if ("big_award_id" == paramName)
			{
				flag = true;
			}
			else if ("songli_id" == paramName)
			{
				flag = true;
			}
			if (flag)
			{
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("big_award_id", 0);
				int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("songli_id", 0);
				GameManager.ClientMgr.NotifyAllChangeHuoDongID(gameConfigItemInt, gameConfigItemInt2);
			}
			if ("half_yinliang_period" == paramName)
			{
				int gameConfigItemInt3 = GameManager.GameConfigMgr.GetGameConfigItemInt("half_yinliang_period", 0);
				GameManager.ClientMgr.NotifyAllChangeHalfYinLiangPeriod(gameConfigItemInt3);
			}
		}

		private Dictionary<string, string> _GameConfigDict = new Dictionary<string, string>();
	}
}
