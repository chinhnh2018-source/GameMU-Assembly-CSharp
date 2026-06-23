using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;

namespace GameServer.Logic
{
	public class LianZhiManager : IManager
	{
		public static LianZhiManager GetInstance()
		{
			return LianZhiManager.Instance;
		}

		public bool initialize()
		{
			this.InitConfig();
			TCPCmdDispatcher.getInstance().registerProcessor(668, 3, LianZhiCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_EXEC_LIANZHI));
			TCPCmdDispatcher.getInstance().registerProcessor(669, 1, LianZhiCmdProcessor.getInstance(TCPGameServerCmds.CMD_SPR_QUERY_LIANZHICOUNT));
			return true;
		}

		public bool startup()
		{
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

		public void InitConfig()
		{
			try
			{
				this.JinBiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("JinBiLianZhi", ',');
				this.BangZuanLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("BangZuanLianZhi", ',');
				this.ZuanShiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("ZuanShiLianZhi", ',');
				this.VIPJinBiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinBiLianZhi", ',');
				this.VIPBangZuanLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPBangZuanLianZhi", ',');
				this.VIPZuanShiLianZhi = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPZuanShiLianZhi", ',');
				this.ConfigLoadSuccess = true;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("加载炼制系统配置信息是出错: {0}", ex.ToString()));
			}
		}

		public bool QueryLianZhiCount(GameClient client)
		{
			List<int> list = new List<int>();
			int roleID = client.ClientData.RoleID;
			int vipLevel = client.ClientData.VipLevel;
			int cmdId = 669;
			list.Add(1);
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			bool result;
			if (!this.ConfigLoadSuccess)
			{
				list[0] = -3;
				list.Add(0);
				list.Add(0);
				list.Add(0);
				client.sendCmd<List<int>>(cmdId, list, false);
				result = true;
			}
			else
			{
				int item = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiCount");
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiDayID");
				int num = this.JinBiLianZhi[2] + this.VIPJinBiLianZhi[Math.Min(this.VIPJinBiLianZhi.Length - 1, vipLevel)];
				if (roleParamsInt32FromDB != dayOfYear)
				{
					item = 0;
				}
				list.Add(item);
				item = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanCount");
				roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanDayID");
				num = this.BangZuanLianZhi[2] + this.VIPBangZuanLianZhi[Math.Min(this.VIPBangZuanLianZhi.Length - 1, vipLevel)];
				if (roleParamsInt32FromDB != dayOfYear)
				{
					item = 0;
				}
				list.Add(item);
				item = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiCount");
				roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiDayID");
				num = this.ZuanShiLianZhi[4] + this.VIPZuanShiLianZhi[Math.Min(this.VIPZuanShiLianZhi.Length - 1, vipLevel)];
				if (roleParamsInt32FromDB != dayOfYear)
				{
					item = 0;
				}
				list.Add(item);
				client.sendCmd<List<int>>(cmdId, list, false);
				result = true;
			}
			return result;
		}

		public bool ExecLianZhi(GameClient client, int type, int count)
		{
			int roleID = client.ClientData.RoleID;
			int vipLevel = client.ClientData.VipLevel;
			int cmdId = 668;
			string text = "炼制系统";
			List<int> list = new List<int>();
			list.Add(1);
			list.Add(type);
			list.Add(count);
			if (!this.ConfigLoadSuccess)
			{
				list[0] = -3;
				client.sendCmd<List<int>>(cmdId, list, false);
			}
			else if (type < 0 || type > 2)
			{
				list[0] = -5;
				client.sendCmd<List<int>>(cmdId, list, false);
			}
			else
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				long num4 = 0L;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = -1;
				int num9 = 0;
				int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
				if (type == 0)
				{
					text = "金币炼制";
					num7 = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiCount");
					num8 = Global.GetRoleParamsInt32FromDB(client, "LianZhiJinBiDayID");
					num9 = this.JinBiLianZhi[2] + this.VIPJinBiLianZhi[Math.Min(this.VIPJinBiLianZhi.Length - 1, vipLevel)];
					num = this.JinBiLianZhi[0];
					num4 = (long)this.JinBiLianZhi[1];
					ProcessTask.ProcessAddTaskVal(client, TaskTypes.LianZhi_JinBi, -1, 1, new object[0]);
					double num10 = 0.0;
					double num11 = 0.0;
					double num12 = 0.0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(6);
						if (null != config)
						{
							num11 += config.GetMult();
						}
						config = jieRiMultAwardActivity.GetConfig(9);
						if (null != config)
						{
							num10 += config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num10 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanAward);
						num12 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanCount);
					}
					num10 = Math.Max(1.0, num10);
					num11 = Math.Max(1.0, num11);
					num4 = (long)((int)((double)num4 * num10));
					num9 = num9 * (int)num11 + (int)num12;
				}
				else if (type == 1)
				{
					text = "绑钻炼制";
					num7 = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanCount");
					num8 = Global.GetRoleParamsInt32FromDB(client, "LianZhiBangZuanDayID");
					num9 = this.BangZuanLianZhi[2] + this.VIPBangZuanLianZhi[Math.Min(this.VIPBangZuanLianZhi.Length - 1, vipLevel)];
					num2 = this.BangZuanLianZhi[0];
					num5 = this.BangZuanLianZhi[1];
					double num10 = 0.0;
					double num11 = 0.0;
					double num12 = 0.0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(6);
						if (null != config)
						{
							num11 += config.GetMult();
						}
						config = jieRiMultAwardActivity.GetConfig(9);
						if (null != config)
						{
							num10 += config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num10 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanAward);
						num12 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanCount);
					}
					num10 = Math.Max(1.0, num10);
					num11 = Math.Max(1.0, num11);
					num5 = (int)((double)num5 * num10);
					num9 = num9 * (int)num11 + (int)num12;
				}
				else if (type == 2)
				{
					text = "钻石炼制";
					num7 = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiCount");
					num8 = Global.GetRoleParamsInt32FromDB(client, "LianZhiZuanShiDayID");
					num9 = this.ZuanShiLianZhi[4] + this.VIPZuanShiLianZhi[Math.Min(this.VIPZuanShiLianZhi.Length - 1, vipLevel)];
					num3 = this.ZuanShiLianZhi[0];
					num4 = (long)this.ZuanShiLianZhi[1];
					num5 = this.ZuanShiLianZhi[2];
					num6 = this.ZuanShiLianZhi[3];
					double num10 = 0.0;
					double num11 = 0.0;
					double num12 = 0.0;
					JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
					if (null != jieRiMultAwardActivity)
					{
						JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(6);
						if (null != config)
						{
							num11 += config.GetMult();
						}
						config = jieRiMultAwardActivity.GetConfig(9);
						if (null != config)
						{
							num10 += config.GetMult();
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (null != specPriorityActivity)
					{
						num10 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanAward);
						num12 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_ZhuanHuanCount);
					}
					num10 = Math.Max(1.0, num10);
					num11 = Math.Max(1.0, num11);
					num4 = (long)((int)((double)num4 * num10));
					num5 = (int)((double)num5 * num10);
					num6 = (int)((double)num6 * num10);
					num9 = num9 * (int)num11 + (int)num12;
				}
				if (num8 != dayOfYear)
				{
					num7 = 0;
				}
				if (count <= 0)
				{
					count = num9 - num7;
				}
				if (count <= 0 || num7 + count > num9)
				{
					list[0] = -16;
					client.sendCmd<List<int>>(cmdId, list, false);
				}
				else
				{
					num *= count;
					num2 *= count;
					num3 *= count;
					num4 *= (long)count;
					num5 *= count;
					num6 *= count;
					num4 = Global.GetExpMultiByZhuanShengExpXiShu(client, num4);
					if (num > 0 && !Global.SubBindTongQianAndTongQian(client, num, text))
					{
						list[0] = -9;
						client.sendCmd<List<int>>(cmdId, list, false);
					}
					else if (num2 > 0 && !GameManager.ClientMgr.SubUserGold(client, num2, text))
					{
						list[0] = -17;
						client.sendCmd<List<int>>(cmdId, list, false);
					}
					else if (num3 > 0 && !GameManager.ClientMgr.SubUserMoney(client, num3, text, true, true, true, true, DaiBiSySType.None))
					{
						list[0] = -10;
						client.sendCmd<List<int>>(cmdId, list, false);
					}
					else
					{
						if (num4 > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(client, num4, true, true, false, "none");
						}
						if (num6 > 0)
						{
							GameManager.ClientMgr.AddMoney1(client, num6, text, true);
						}
						if (num5 > 0)
						{
							GameManager.ClientMgr.ModifyStarSoulValue(client, num5, text, true, true);
						}
						num7 += count;
						num8 = dayOfYear;
						if (type == 0)
						{
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JinBiZhuanHuanTimes));
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiJinBiCount", num7, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiJinBiDayID", num8, true);
						}
						else if (type == 1)
						{
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.BangZuanZhuanHuanTimes));
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiBangZuanCount", num7, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiBangZuanDayID", num8, true);
						}
						else if (type == 2)
						{
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ZuanShiZhuanHuanTimes));
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiZuanShiCount", num7, true);
							Global.SaveRoleParamsInt32ValueToDB(client, "LianZhiZuanShiDayID", num8, true);
						}
						client.sendCmd<List<int>>(cmdId, list, false);
					}
				}
			}
			return true;
		}

		private static LianZhiManager Instance = new LianZhiManager();

		private int[] JinBiLianZhi = null;

		private int[] VIPJinBiLianZhi = null;

		private int[] BangZuanLianZhi = null;

		private int[] VIPBangZuanLianZhi = null;

		private int[] ZuanShiLianZhi = null;

		private int[] VIPZuanShiLianZhi = null;

		private bool ConfigLoadSuccess = false;
	}
}
