using System;
using System.Collections.Generic;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class MarryPartyDataCache
	{
		public Dictionary<int, MarryPartyData> MarryPartyList { private get; set; }

		public MarryPartyData AddParty(int roleID, int partyType, long startTime, int husbandRoleID, int wifeRoleID, string husbandName, string wifeName)
		{
			MarryPartyData marryPartyData = null;
			lock (this.MarryPartyList)
			{
				if (!this.MarryPartyList.ContainsKey(husbandRoleID) && !this.MarryPartyList.ContainsKey(wifeRoleID))
				{
					marryPartyData = new MarryPartyData
					{
						RoleID = roleID,
						PartyType = partyType,
						JoinCount = 0,
						StartTime = startTime,
						HusbandRoleID = husbandRoleID,
						WifeRoleID = wifeRoleID,
						HusbandName = husbandName,
						WifeName = wifeName
					};
					this.MarryPartyList.Add(roleID, marryPartyData);
				}
			}
			return marryPartyData;
		}

		public void SetPartyTime(MarryPartyData data, long startTime)
		{
			lock (this.MarryPartyList)
			{
				data.StartTime = startTime;
			}
		}

		public bool RemoveParty(int roleid)
		{
			bool result;
			lock (this.MarryPartyList)
			{
				result = this.MarryPartyList.Remove(roleid);
			}
			return result;
		}

		public void RemovePartyCancel(MarryPartyData partyData)
		{
			lock (this.MarryPartyList)
			{
				try
				{
					this.MarryPartyList.Add(partyData.RoleID, partyData);
				}
				catch
				{
				}
			}
		}

		public bool IncPartyJoin(int roleid, int maxJoin, out bool remove)
		{
			remove = false;
			MarryPartyData marryPartyData = null;
			bool result;
			lock (this.MarryPartyList)
			{
				bool flag2 = this.MarryPartyList.TryGetValue(roleid, out marryPartyData);
				if (flag2)
				{
					if (marryPartyData.JoinCount < maxJoin)
					{
						marryPartyData.JoinCount++;
						if (marryPartyData.JoinCount == maxJoin)
						{
							remove = true;
						}
					}
					else
					{
						flag2 = false;
					}
				}
				result = flag2;
			}
			return result;
		}

		public void IncPartyJoinCancel(int roleid)
		{
			MarryPartyData marryPartyData = null;
			lock (this.MarryPartyList)
			{
				if (this.MarryPartyList.TryGetValue(roleid, out marryPartyData))
				{
					marryPartyData.JoinCount--;
				}
			}
		}

		public MarryPartyData GetParty(int roleid)
		{
			MarryPartyData marryPartyData = null;
			MarryPartyData result;
			lock (this.MarryPartyList)
			{
				this.MarryPartyList.TryGetValue(roleid, out marryPartyData);
				result = marryPartyData;
			}
			return result;
		}

		public int GetPartyCount()
		{
			int count;
			lock (this.MarryPartyList)
			{
				count = this.MarryPartyList.Count;
			}
			return count;
		}

		public TCPOutPacket GetPartyList(TCPOutPacketPool pool, int cmdID)
		{
			TCPOutPacket result;
			lock (this.MarryPartyList)
			{
				result = DataHelper.ObjectToTCPOutPacket<Dictionary<int, MarryPartyData>>(this.MarryPartyList, pool, cmdID);
			}
			return result;
		}

		public bool HasPartyStarted(long ticks)
		{
			bool result = false;
			lock (this.MarryPartyList)
			{
				foreach (KeyValuePair<int, MarryPartyData> keyValuePair in this.MarryPartyList)
				{
					if (ticks > keyValuePair.Value.StartTime)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleId);
				if (safeClientDataFromLocalOrDB != null && safeClientDataFromLocalOrDB.MyMarriageData != null && safeClientDataFromLocalOrDB.MyMarriageData.nSpouseID != -1)
				{
					lock (this.MarryPartyList)
					{
						MarryPartyData marryPartyData = null;
						this.MarryPartyList.TryGetValue(safeClientDataFromLocalOrDB.RoleID, out marryPartyData);
						if (marryPartyData != null)
						{
							if (!string.IsNullOrEmpty(marryPartyData.HusbandName) && marryPartyData.HusbandName == oldName)
							{
								marryPartyData.HusbandName = newName;
							}
							else if (!string.IsNullOrEmpty(marryPartyData.WifeName) && marryPartyData.WifeName == oldName)
							{
								marryPartyData.WifeName = newName;
							}
						}
						marryPartyData = null;
						this.MarryPartyList.TryGetValue(safeClientDataFromLocalOrDB.MyMarriageData.nSpouseID, out marryPartyData);
						if (marryPartyData != null)
						{
							if (!string.IsNullOrEmpty(marryPartyData.HusbandName) && marryPartyData.HusbandName == oldName)
							{
								marryPartyData.HusbandName = newName;
							}
							else if (!string.IsNullOrEmpty(marryPartyData.WifeName) && marryPartyData.WifeName == oldName)
							{
								marryPartyData.WifeName = newName;
							}
						}
					}
				}
			}
		}
	}
}
