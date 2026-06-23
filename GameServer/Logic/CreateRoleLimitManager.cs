using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class CreateRoleLimitManager : SingletonTemplate<CreateRoleLimitManager>
	{
		public bool IfCanCreateRole(string UserID, string UserName, string DeviceID, string IP, out int NotifyLeftTime)
		{
			NotifyLeftTime = 0;
			LimitResultData limitResultData = new LimitResultData();
			if (!this._UserWhiteList.Contains(UserID.ToLower()))
			{
				this.CheckByDeviceID(UserID, UserName, DeviceID, limitResultData);
				if (limitResultData.CanCreate)
				{
					this.CheckByIP(UserID, UserName, IP, limitResultData);
					if (!limitResultData.CanCreate)
					{
					}
				}
			}
			if (!limitResultData.CanCreate)
			{
				NotifyLeftTime = this.CaculateNextAvailableTime(limitResultData);
			}
			return limitResultData.CanCreate;
		}

		public void ModifyCreateRoleNum(string UserID, string UserName, string DeviceID, string IP)
		{
			lock (this.DeviceIDLimitData)
			{
				if (-1 != this.DeviceIDRestrictNum && !string.IsNullOrEmpty(DeviceID))
				{
					this.ModifyTotalNum(this.DeviceIDLimitData, DeviceID);
				}
			}
			lock (this.IPLimitData)
			{
				if (-1 != this.IPRestrictNum && !string.IsNullOrEmpty(IP))
				{
					this.ModifyTotalNum(this.IPLimitData, IP);
				}
			}
		}

		private int CaculateNextAvailableTime(LimitResultData CheckData)
		{
			int result;
			if (CheckData.CanCreate)
			{
				result = 0;
			}
			else
			{
				DateTime d = CheckData.AnalysisDataTime.AddMinutes((double)this.CreateRoleLimitMinutes);
				DateTime d2 = TimeUtil.NowDateTime();
				result = (int)(d - d2).TotalSeconds;
			}
			return result;
		}

		private void ModifyTotalNum(LinkedList<LimitAnalysisData> list, string key)
		{
			Dictionary<string, int> hourAnalysisData = this.GetHourAnalysisData(list);
			if (null != hourAnalysisData)
			{
				int num = 0;
				if (!hourAnalysisData.TryGetValue(key, out num))
				{
					hourAnalysisData.Add(key, 1);
				}
				else
				{
					hourAnalysisData[key] = num + 1;
				}
			}
		}

		private int ComputeTotalNum(LinkedList<LimitAnalysisData> list, string key, LimitResultData CheckData)
		{
			int num = 0;
			int result;
			if (list.Count == 0)
			{
				result = num;
			}
			else
			{
				this.DoHouseKeepingForAnalysisData(list);
				foreach (LimitAnalysisData limitAnalysisData in list)
				{
					int num2 = 0;
					if (limitAnalysisData.dict.TryGetValue(key, out num2))
					{
						num += num2;
					}
				}
				if (list.Count != 0)
				{
					CheckData.AnalysisDataTime = list.First.Value.Timestamp;
				}
				result = num;
			}
			return result;
		}

		private Dictionary<string, int> GetHourAnalysisData(LinkedList<LimitAnalysisData> list)
		{
			DateTime workDateTime = TimeUtil.NowDateTime();
			if (list.Count == 0 || this.WorkDateTime.Hour != workDateTime.Hour)
			{
				this.WorkDateTime = workDateTime;
				list.AddLast(new LimitAnalysisData());
			}
			return list.Last.Value.dict;
		}

		private void DoHouseKeepingForAnalysisData(LinkedList<LimitAnalysisData> list)
		{
			if (list != null && list.Count != 0)
			{
				DateTime d = TimeUtil.NowDateTime();
				LimitAnalysisData value = list.First.Value;
				int num = (int)(d - value.Timestamp).TotalMinutes;
				if (num >= this.CreateRoleLimitMinutes)
				{
					list.RemoveFirst();
				}
			}
		}

		private void CheckByDeviceID(string UserID, string UserName, string DeviceID, LimitResultData CheckData)
		{
			if (-1 != this.DeviceIDRestrictNum && !string.IsNullOrEmpty(DeviceID))
			{
				lock (this.DeviceIDLimitData)
				{
					int num = this.ComputeTotalNum(this.DeviceIDLimitData, DeviceID, CheckData);
					if (num >= this.DeviceIDRestrictNum)
					{
						CheckData.CanCreate = false;
						LogManager.WriteLog(2, string.Format("玩家创建角色被限制, UserID={0}, UserName={1}, DeviceID={2}, CountNum={3}", new object[]
						{
							UserID,
							UserName,
							DeviceID,
							num
						}), null, true);
					}
				}
			}
		}

		private bool IfIPInWhiteList(string IP)
		{
			List<IPWhiteList> list = null;
			lock (this)
			{
				list = this._IPWhiteList;
			}
			bool result;
			if (list == null || list.Count == 0)
			{
				result = false;
			}
			else
			{
				IPAddress ipaddress = IPAddress.Parse(IP);
				if (ipaddress == null)
				{
					result = false;
				}
				else
				{
					byte[] addressBytes = ipaddress.GetAddressBytes();
					uint num = (uint)((int)addressBytes[0] << 24 | (int)addressBytes[1] << 16 | (int)addressBytes[2] << 8 | (int)addressBytes[3]);
					foreach (IPWhiteList ipwhiteList in list)
					{
						if (ipwhiteList.MinIP <= num && ipwhiteList.MaxIP >= num)
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		private void CheckByIP(string UserID, string UserName, string IP, LimitResultData CheckData)
		{
			if (-1 != this.IPRestrictNum && !string.IsNullOrEmpty(IP))
			{
				if (!this.IfIPInWhiteList(IP))
				{
					lock (this.IPLimitData)
					{
						int num = this.ComputeTotalNum(this.IPLimitData, IP, CheckData);
						if (num >= this.IPRestrictNum)
						{
							CheckData.CanCreate = false;
							LogManager.WriteLog(2, string.Format("玩家创建角色被限制, UserID={0}, UserName={1}, IP={2}, CountNum={3}", new object[]
							{
								UserID,
								UserName,
								IP,
								num
							}), null, true);
						}
					}
				}
			}
		}

		public void LoadConfig()
		{
			string paramValueByName = GameManager.systemParamsList.GetParamValueByName("DeviceIDRestrict");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				this.DeviceIDRestrictNum = Global.SafeConvertToInt32(paramValueByName);
			}
			string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("IPRestrict");
			if (!string.IsNullOrEmpty(paramValueByName2))
			{
				this.IPRestrictNum = Global.SafeConvertToInt32(paramValueByName2);
			}
			string paramValueByName3 = GameManager.systemParamsList.GetParamValueByName("BagClearUpCD");
			if (!string.IsNullOrEmpty(paramValueByName3))
			{
				this.ResetBagSlotTicks = Global.SafeConvertToInt32(paramValueByName3);
			}
			string paramValueByName4 = GameManager.systemParamsList.GetParamValueByName("RefreshBourseCD");
			if (!string.IsNullOrEmpty(paramValueByName4))
			{
				this.RefreshMarketSlotTicks = Global.SafeConvertToInt32(paramValueByName4);
			}
			string paramValueByName5 = GameManager.systemParamsList.GetParamValueByName("AddFriendCD");
			if (!string.IsNullOrEmpty(paramValueByName5))
			{
				this.AddFriendSlotTicks = Global.SafeConvertToInt32(paramValueByName5);
				this.AddBHMemberSlotTicks = Global.SafeConvertToInt32(paramValueByName5);
			}
			string paramValueByName6 = GameManager.systemParamsList.GetParamValueByName("SpiritPutOnCD");
			if (!string.IsNullOrEmpty(paramValueByName6))
			{
				this.SpriteFightSlotTicks = Global.SafeConvertToInt32(paramValueByName6);
			}
			string paramValueByName7 = GameManager.systemParamsList.GetParamValueByName("DeleteRoleNeedTime");
			GameManager.GameConfigMgr.SetGameConfigItem("DeleteRoleNeedTime", paramValueByName7);
			Global.UpdateDBGameConfigg("DeleteRoleNeedTime", paramValueByName7);
			lock (this)
			{
				this.LoadIPWhiteList();
				this.LoadUserWhiteList();
			}
		}

		private void LoadIPWhiteList()
		{
			try
			{
				XElement xelement = XElement.Load(Global.IsolateResPath("Config/IPWhiteList.xml"));
				if (null != xelement)
				{
					List<IPWhiteList> list = new List<IPWhiteList>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						IPWhiteList ipwhiteList = new IPWhiteList();
						ipwhiteList.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						IPAddress ipaddress = IPAddress.Parse(Global.GetSafeAttributeStr(xml, "MinIP"));
						byte[] addressBytes = ipaddress.GetAddressBytes();
						ipwhiteList.MinIP = (uint)((int)addressBytes[0] << 24 | (int)addressBytes[1] << 16 | (int)addressBytes[2] << 8 | (int)addressBytes[3]);
						IPAddress ipaddress2 = IPAddress.Parse(Global.GetSafeAttributeStr(xml, "MaxIP"));
						byte[] addressBytes2 = ipaddress2.GetAddressBytes();
						ipwhiteList.MaxIP = (uint)((int)addressBytes2[0] << 24 | (int)addressBytes2[1] << 16 | (int)addressBytes2[2] << 8 | (int)addressBytes2[3]);
						list.Add(ipwhiteList);
					}
					this._IPWhiteList = list;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=IPWhiteList.xml");
			}
		}

		private void LoadUserWhiteList()
		{
			try
			{
				XElement xelement = ConfigHelper.Load(Global.IsolateResPath("Config/UserWhiteList.xml"));
				if (null != xelement)
				{
					HashSet<string> hashSet = new HashSet<string>();
					IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement, "WhiteList");
					foreach (XElement xelement2 in xelements)
					{
						string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "PinTai", "");
						if (0 == string.Compare(elementAttributeValue, GameCoreInterface.getinstance().GetPlatformType().ToString(), true))
						{
							string elementAttributeValue2 = ConfigHelper.GetElementAttributeValue(xelement2, "UserID", "");
							if (!string.IsNullOrEmpty(elementAttributeValue2))
							{
								hashSet.Add(elementAttributeValue2.ToLower());
							}
						}
					}
					this._UserWhiteList = hashSet;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString() + "xmlFileName=IPWhiteList.xml");
			}
		}

		private const string IPWhiteListfileName = "Config/IPWhiteList.xml";

		private const string UserWhiteListfileName = "Config/UserWhiteList.xml";

		public int ResetBagSlotTicks = 0;

		public int RefreshMarketSlotTicks = 0;

		public int SpriteFightSlotTicks = 0;

		public int AddBHMemberSlotTicks = 0;

		public int AddFriendSlotTicks = 0;

		public int CreateRoleLimitMinutes = 1440;

		private int DeviceIDRestrictNum = -1;

		private int IPRestrictNum = -1;

		private DateTime WorkDateTime = TimeUtil.NowDateTime();

		private List<IPWhiteList> _IPWhiteList = new List<IPWhiteList>();

		private HashSet<string> _UserWhiteList = new HashSet<string>();

		private LinkedList<LimitAnalysisData> DeviceIDLimitData = new LinkedList<LimitAnalysisData>();

		private LinkedList<LimitAnalysisData> IPLimitData = new LinkedList<LimitAnalysisData>();
	}
}
