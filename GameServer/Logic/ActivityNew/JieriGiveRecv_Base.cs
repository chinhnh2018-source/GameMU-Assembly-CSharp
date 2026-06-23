using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.ActivityNew
{
	public class JieriGiveRecv_Base : Activity
	{
		public virtual string GetConfigFile()
		{
			throw new Exception("GetConfigFile未实现");
		}

		public virtual string QueryActInfo(GameClient client)
		{
			throw new Exception("QueryActInfo未实现");
		}

		public virtual void FlushIcon(GameClient client)
		{
			throw new Exception("OnGetAwardSuccess未实现");
		}

		public virtual bool IsReachConition(RoleGiveRecvInfo info, int condValue)
		{
			throw new Exception("IsReachConition未实现");
		}

		protected RoleGiveRecvInfo GetRoleGiveRecvInfo(int roleid)
		{
			bool flag;
			return this.GetRoleGiveRecvInfo(roleid, out flag);
		}

		protected RoleGiveRecvInfo GetRoleGiveRecvInfo(int roleid, out bool bLoadFromDb)
		{
			bLoadFromDb = false;
			RoleGiveRecvInfo result;
			lock (this.roleGiveRecvDict_dont_use_directly)
			{
				if (this.roleGiveRecvDict_dont_use_directly.ContainsKey(roleid))
				{
					RoleGiveRecvInfo roleGiveRecvInfo = this.roleGiveRecvDict_dont_use_directly[roleid];
					if (roleGiveRecvInfo.TodayIdxInActPeriod == Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(DateTime.Parse(this.FromDate)) + 1)
					{
						return roleGiveRecvInfo;
					}
				}
				RoleGiveRecvInfo roleGiveRecvInfo2 = new RoleGiveRecvInfo();
				this.roleGiveRecvDict_dont_use_directly[roleid] = roleGiveRecvInfo2;
				DateTime now = TimeUtil.NowDateTime();
				bool flag2 = Global.GetOffsetDay(now) == Global.GetOffsetDay(DateTime.Parse(this.FromDate));
				bool flag3 = Global.GetOffsetDay(now) == Global.GetOffsetDay(DateTime.Parse(this.ToDate));
				string text = flag2 ? this.FromDate : (now.ToString("yyyy-MM-dd") + " 00:00:00");
				string text2 = flag3 ? this.ToDate : (now.ToString("yyyy-MM-dd") + " 23:59:59");
				int num = Global.GetOffsetDay(now) - Global.GetOffsetDay(DateTime.Parse(this.FromDate)) + 1;
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					roleid,
					this.ActivityType,
					text.Replace(':', '$'),
					text2.Replace(':', '$'),
					num
				});
				string[] array = Global.ExecuteDBCmd(13202, strcmd, 0);
				if (array == null || array.Length != 3)
				{
					roleGiveRecvInfo2.TotalGive = 0;
					roleGiveRecvInfo2.TotalRecv = 0;
					roleGiveRecvInfo2.AwardFlag = 0;
				}
				else
				{
					bLoadFromDb = true;
					roleGiveRecvInfo2.TotalGive = Convert.ToInt32(array[0]);
					roleGiveRecvInfo2.TotalRecv = Convert.ToInt32(array[1]);
					roleGiveRecvInfo2.AwardFlag = Convert.ToInt32(array[2]);
				}
				roleGiveRecvInfo2.TodayStart = text;
				roleGiveRecvInfo2.TodayEnd = text2;
				roleGiveRecvInfo2.TodayIdxInActPeriod = num;
				result = roleGiveRecvInfo2;
			}
			return result;
		}

		public bool Init()
		{
			string configFile = this.GetConfigFile();
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(configFile));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(configFile));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
					this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							AwardItem awardItem = new AwardItem();
							AwardItem awardItem2 = new AwardItem();
							AwardEffectTimeItem awardEffectTimeItem = new AwardEffectTimeItem();
							awardItem.MinAwardCondionValue = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Num"));
							awardItem.MinAwardCondionValue2 = Global.GMax(0, (int)Global.GetSafeAttributeLong(xelement3, "Goods"));
							awardItem.AwardYuanBao = 0;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取{0}配置文件中的物品配置项1失败", configFile), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取{0}活动配置文件中的物品配置项失败", configFile), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, configFile);
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, configFile, null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, configFile);
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "GoodsThr");
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement3, "EffectiveTime");
							awardEffectTimeItem.Init(safeAttributeStr2, safeAttributeStr3, configFile + " 时效性物品");
							string safeAttributeStr4 = Global.GetSafeAttributeStr(xelement3, "ID");
							int key = Convert.ToInt32(safeAttributeStr4);
							this.allAwardDict.Add(key, awardItem);
							this.occAwardDict.Add(key, awardItem2);
							this.timeAwardDict.Add(key, awardEffectTimeItem);
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", configFile, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public string ProcRoleGetAward(GameClient client, int awardid)
		{
			JieriGiveErrorCode jieriGiveErrorCode;
			if (!this.InAwardTime())
			{
				jieriGiveErrorCode = JieriGiveErrorCode.NotAwardTime;
			}
			else if (!this.HasEnoughBagSpaceForAwardGoods(client, awardid))
			{
				jieriGiveErrorCode = JieriGiveErrorCode.NoBagSpace;
			}
			else
			{
				AwardItem awardItem = null;
				AwardItem myAwardItem = null;
				AwardEffectTimeItem awardEffectTimeItem = null;
				if (!this.allAwardDict.TryGetValue(awardid, out awardItem) || !this.occAwardDict.TryGetValue(awardid, out myAwardItem) || !this.timeAwardDict.TryGetValue(awardid, out awardEffectTimeItem))
				{
					jieriGiveErrorCode = JieriGiveErrorCode.ConfigError;
				}
				else
				{
					RoleGiveRecvInfo roleGiveRecvInfo = this.GetRoleGiveRecvInfo(client.ClientData.RoleID);
					if (!this.IsReachConition(roleGiveRecvInfo, awardItem.MinAwardCondionValue) || (roleGiveRecvInfo.AwardFlag & 1 << awardid) != 0)
					{
						jieriGiveErrorCode = JieriGiveErrorCode.NotMeetAwardCond;
					}
					else
					{
						int num = roleGiveRecvInfo.AwardFlag | 1 << awardid;
						string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							client.ClientData.RoleID,
							roleGiveRecvInfo.TodayStart.Replace(':', '$'),
							roleGiveRecvInfo.TodayEnd.Replace(':', '$'),
							this.ActivityType,
							roleGiveRecvInfo.TodayIdxInActPeriod,
							num
						});
						string[] array = Global.ExecuteDBCmd(13201, strcmd, client.ServerId);
						if (array == null || array.Length < 1 || Convert.ToInt32(array[0]) <= 0)
						{
							jieriGiveErrorCode = JieriGiveErrorCode.DBFailed;
						}
						else
						{
							roleGiveRecvInfo.AwardFlag = num;
							if (!base.GiveAward(client, awardItem) || !base.GiveAward(client, myAwardItem) || !base.GiveEffectiveTimeAward(client, awardEffectTimeItem.ToAwardItem()))
							{
								LogManager.WriteLog(2, string.Format("节日赠送活动奖品发送失败，但是已经设置为已发放，roleid={0}, rolename={1}, awardid={3}", client.ClientData.RoleID, client.ClientData.RoleName, awardid), null, true);
							}
							jieriGiveErrorCode = JieriGiveErrorCode.Success;
						}
					}
				}
			}
			if (jieriGiveErrorCode == JieriGiveErrorCode.Success)
			{
				this.FlushIcon(client);
			}
			return string.Format("{0}:{1}", (int)jieriGiveErrorCode, awardid);
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int id)
		{
			AwardItem awardItem = null;
			AwardItem awardItem2 = null;
			AwardEffectTimeItem awardEffectTimeItem = null;
			this.allAwardDict.TryGetValue(id, out awardItem);
			this.occAwardDict.TryGetValue(id, out awardItem2);
			this.timeAwardDict.TryGetValue(id, out awardEffectTimeItem);
			int num = 0;
			if (awardItem != null && awardItem.GoodsDataList != null)
			{
				num += awardItem.GoodsDataList.Count;
			}
			if (awardItem2 != null && awardItem2.GoodsDataList != null)
			{
				num += awardItem2.GoodsDataList.Count((GoodsData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
			}
			if (awardEffectTimeItem != null)
			{
				num += awardEffectTimeItem.GoodsCnt();
			}
			return Global.CanAddGoodsNum(client, num);
		}

		protected bool IsGiveGoodsID(int goodsID)
		{
			foreach (KeyValuePair<int, AwardItem> keyValuePair in this.allAwardDict)
			{
				if (keyValuePair.Value.MinAwardCondionValue2 == goodsID)
				{
					return true;
				}
			}
			foreach (KeyValuePair<int, AwardItem> keyValuePair in this.occAwardDict)
			{
				if (keyValuePair.Value.MinAwardCondionValue2 == goodsID)
				{
					return true;
				}
			}
			return false;
		}

		public bool CanGetAnyAward(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!this.InAwardTime())
			{
				result = false;
			}
			else
			{
				RoleGiveRecvInfo roleGiveRecvInfo = this.GetRoleGiveRecvInfo(client.ClientData.RoleID);
				foreach (KeyValuePair<int, AwardItem> keyValuePair in this.allAwardDict)
				{
					int key = keyValuePair.Key;
					AwardItem value = keyValuePair.Value;
					if (this.IsReachConition(roleGiveRecvInfo, value.MinAwardCondionValue) && (roleGiveRecvInfo.AwardFlag & 1 << key) == 0)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public void UpdateNewDay(GameClient client)
		{
			if (client != null)
			{
				bool flag = false;
				lock (this.roleGiveRecvDict_dont_use_directly)
				{
					if (this.roleGiveRecvDict_dont_use_directly.ContainsKey(client.ClientData.RoleID))
					{
						this.roleGiveRecvDict_dont_use_directly.Remove(client.ClientData.RoleID);
						flag = true;
					}
				}
				if (flag)
				{
					this.FlushIcon(client);
				}
			}
		}

		private Dictionary<int, AwardItem> allAwardDict = new Dictionary<int, AwardItem>();

		private Dictionary<int, AwardItem> occAwardDict = new Dictionary<int, AwardItem>();

		private Dictionary<int, AwardEffectTimeItem> timeAwardDict = new Dictionary<int, AwardEffectTimeItem>();

		private Dictionary<int, RoleGiveRecvInfo> roleGiveRecvDict_dont_use_directly = new Dictionary<int, RoleGiveRecvInfo>();
	}
}
