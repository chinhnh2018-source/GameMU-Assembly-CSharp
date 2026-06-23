using System;
using System.Collections.Generic;
using System.Linq;
using Server.TCP;

namespace GameServer.Logic
{
	public class UserSession
	{
		public List<TMSKSocket> GetSocketList()
		{
			List<TMSKSocket> result;
			lock (this)
			{
				result = this._S2UDict.Keys.ToList<TMSKSocket>();
			}
			return result;
		}

		public bool AddSession(TMSKSocket clientSocket, string userID)
		{
			lock (this)
			{
				string text = "";
				if (this._S2UDict.TryGetValue(clientSocket, out text))
				{
					return false;
				}
				TMSKSocket tmsksocket = null;
				if (this._U2SDict.TryGetValue(userID, out tmsksocket))
				{
					return false;
				}
				this._S2UDict[clientSocket] = userID;
				this._U2SDict[userID] = clientSocket;
			}
			return true;
		}

		public void RemoveSession(TMSKSocket clientSocket)
		{
			if (null != clientSocket)
			{
				string key = "";
				lock (this)
				{
					if (this._S2UDict.TryGetValue(clientSocket, out key))
					{
						this._S2UDict.Remove(clientSocket);
						this._U2SDict.Remove(key);
					}
				}
			}
		}

		public string FindUserID(TMSKSocket clientSocket)
		{
			string result = "";
			lock (this)
			{
				this._S2UDict.TryGetValue(clientSocket, out result);
			}
			return result;
		}

		public TMSKSocket FindSocketByUserID(string userID)
		{
			TMSKSocket result = null;
			lock (this)
			{
				this._U2SDict.TryGetValue(userID, out result);
			}
			return result;
		}

		public void AddUserName(TMSKSocket clientSocket, string userName)
		{
			lock (this)
			{
				this._S2UNameDict[clientSocket] = userName;
				this._UName2SDict[userName] = clientSocket;
			}
		}

		public void RemoveUserName(TMSKSocket clientSocket)
		{
			if (null != clientSocket)
			{
				lock (this)
				{
					string key = null;
					if (this._S2UNameDict.TryGetValue(clientSocket, out key))
					{
						this._S2UNameDict.Remove(clientSocket);
						this._UName2SDict.Remove(key);
					}
				}
			}
		}

		public string FindUserName(TMSKSocket clientSocket)
		{
			string result = null;
			lock (this)
			{
				this._S2UNameDict.TryGetValue(clientSocket, out result);
			}
			return result;
		}

		public TMSKSocket FindSocketByUserName(string userName)
		{
			TMSKSocket result = null;
			lock (this)
			{
				this._UName2SDict.TryGetValue(userName, out result);
			}
			return result;
		}

		public void AddUserAdult(TMSKSocket clientSocket, int isAdult)
		{
			lock (this)
			{
				this._S2UAdultDict[clientSocket] = isAdult;
			}
		}

		public void RemoveUserAdult(TMSKSocket clientSocket)
		{
			if (null != clientSocket)
			{
				lock (this)
				{
					this._S2UAdultDict.Remove(clientSocket);
				}
			}
		}

		public int FindUserAdult(TMSKSocket clientSocket)
		{
			int result = 0;
			lock (this)
			{
				this._S2UAdultDict.TryGetValue(clientSocket, out result);
			}
			return result;
		}

		private Dictionary<TMSKSocket, string> _S2UDict = new Dictionary<TMSKSocket, string>(1000);

		private Dictionary<string, TMSKSocket> _U2SDict = new Dictionary<string, TMSKSocket>(1000);

		private Dictionary<TMSKSocket, string> _S2UNameDict = new Dictionary<TMSKSocket, string>(1000);

		private Dictionary<string, TMSKSocket> _UName2SDict = new Dictionary<string, TMSKSocket>(1000);

		private Dictionary<TMSKSocket, int> _S2UAdultDict = new Dictionary<TMSKSocket, int>(1000);
	}
}
