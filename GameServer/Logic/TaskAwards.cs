using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class TaskAwards
	{
		private void ParseTaskAwardsItem(string awardsStr, out AwardsItemData taskAwards)
		{
			taskAwards = null;
			string[] array = awardsStr.Split(new char[]
			{
				','
			});
			if (array.Length == 7)
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(Convert.ToInt32(array[0]), out systemXmlItem))
				{
					LogManager.WriteLog(2, string.Format("解析任务装备奖励时，物品不存在: GoodsID={0}", Convert.ToInt32(array[0])), null, true);
				}
				else
				{
					taskAwards = new AwardsItemData
					{
						Occupation = ((systemXmlItem == null) ? -1 : Global.GetMainOccupationByGoodsID(Convert.ToInt32(array[0]))),
						RoleSex = ((systemXmlItem == null) ? -1 : systemXmlItem.GetIntValue("ToSex", -1)),
						GoodsID = Convert.ToInt32(array[0]),
						GoodsNum = Convert.ToInt32(array[1]),
						Binding = Convert.ToInt32(array[2]),
						Level = Convert.ToInt32(array[3]),
						AppendLev = Convert.ToInt32(array[4]),
						IsHaveLuckyProp = Convert.ToInt32(array[5]),
						ExcellencePorpValue = Convert.ToInt32(array[6]),
						EndTime = "1900-01-01 12:00:00"
					};
				}
			}
		}

		private void ParseOtherAwardsItem(string awardsStr, out AwardsItemData otherAwards, string goodsEndTime)
		{
			otherAwards = null;
			string[] array = awardsStr.Split(new char[]
			{
				','
			});
			if (array.Length == 7)
			{
				if (string.IsNullOrEmpty(goodsEndTime) || Global.DateTimeTicks(goodsEndTime) <= 0L)
				{
					goodsEndTime = "1900-01-01 12:00:00";
				}
				otherAwards = new AwardsItemData
				{
					Occupation = -1,
					RoleSex = -1,
					GoodsID = Convert.ToInt32(array[0]),
					GoodsNum = Convert.ToInt32(array[1]),
					Binding = Convert.ToInt32(array[2]),
					Level = Convert.ToInt32(array[3]),
					AppendLev = Convert.ToInt32(array[4]),
					IsHaveLuckyProp = Convert.ToInt32(array[5]),
					ExcellencePorpValue = Convert.ToInt32(array[6]),
					EndTime = goodsEndTime
				};
			}
		}

		private void ParseAwards(SystemXmlItem systemTask, out List<AwardsItemData> taskAwardsList, out List<AwardsItemData> otherAwardsList)
		{
			List<AwardsItemData> list;
			otherAwardsList = (list = null);
			taskAwardsList = list;
			AwardsItemData awardsItemData = null;
			string text = systemTask.GetStringValue("Taskaward").Trim();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					'|'
				});
				if (null != array)
				{
					taskAwardsList = new List<AwardsItemData>();
					for (int i = 0; i < array.Length; i++)
					{
						awardsItemData = null;
						this.ParseTaskAwardsItem(array[i], out awardsItemData);
						if (null != awardsItemData)
						{
							taskAwardsList.Add(awardsItemData);
						}
						else
						{
							LogManager.WriteLog(2, string.Format("解析任务装备奖励失败: TaskID={0}", systemTask.GetIntValue("ID", -1)), null, true);
						}
					}
				}
			}
			string goodsEndTime = systemTask.GetStringValue("GoodsEndTime").Trim();
			string text2 = systemTask.GetStringValue("OtherTaskaward").Trim();
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array2 = text2.Split(new char[]
				{
					'|'
				});
				if (null != array2)
				{
					otherAwardsList = new List<AwardsItemData>();
					for (int i = 0; i < array2.Length; i++)
					{
						awardsItemData = null;
						this.ParseOtherAwardsItem(array2[i], out awardsItemData, goodsEndTime);
						if (null != awardsItemData)
						{
							otherAwardsList.Add(awardsItemData);
						}
						else
						{
							LogManager.WriteLog(2, string.Format("解析任务其他奖励失败: TaskID={0}", systemTask.GetIntValue("ID", -1)), null, true);
						}
					}
				}
			}
		}

		public List<AwardsItemData> FindTaskAwards(int taskID)
		{
			List<AwardsItemData> list = null;
			lock (this._TaskAwardsDict)
			{
				if (this._TaskAwardsDict.TryGetValue(taskID, out list))
				{
					return list;
				}
			}
			SystemXmlItem systemTask = null;
			List<AwardsItemData> result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = null;
			}
			else
			{
				List<AwardsItemData> list2 = null;
				this.ParseAwards(systemTask, out list, out list2);
				if (null != list)
				{
					lock (this._TaskAwardsDict)
					{
						this._TaskAwardsDict[taskID] = list;
					}
				}
				result = list;
			}
			return result;
		}

		public List<AwardsItemData> FindOtherAwards(int taskID)
		{
			List<AwardsItemData> list = null;
			lock (this._OtherAwardsDict)
			{
				if (this._OtherAwardsDict.TryGetValue(taskID, out list))
				{
					return list;
				}
			}
			SystemXmlItem systemTask = null;
			List<AwardsItemData> result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = null;
			}
			else
			{
				List<AwardsItemData> list2 = null;
				this.ParseAwards(systemTask, out list2, out list);
				if (null != list)
				{
					lock (this._OtherAwardsDict)
					{
						this._OtherAwardsDict[taskID] = list;
					}
				}
				result = list;
			}
			return result;
		}

		public int FindMoney(int taskID)
		{
			int num = -1;
			lock (this._MoneyDict)
			{
				if (this._MoneyDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("BindMoneyaward", -1);
				lock (this._MoneyDict)
				{
					this._MoneyDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public long FindExperience(GameClient client, int taskID)
		{
			long num = -1L;
			lock (this._ExperienceDict)
			{
				if (this._ExperienceDict.TryGetValue(taskID, out num))
				{
					if (num < 0L)
					{
						num = this.CalcLuaScript(client, taskID, null, "ExpLua");
					}
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			long result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetLongValue("Experienceaward");
				lock (this._ExperienceDict)
				{
					this._ExperienceDict[taskID] = num;
				}
				if (num < 0L)
				{
					num = this.CalcLuaScript(client, taskID, systemXmlItem, "ExpLua");
				}
				result = num;
			}
			return result;
		}

		public int FindYinLiang(int taskID)
		{
			int num = -1;
			lock (this._YinLiangDict)
			{
				if (this._YinLiangDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("Moneyaward", -1);
				lock (this._YinLiangDict)
				{
					this._YinLiangDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindBindYuanBao(int taskID)
		{
			int num = -1;
			lock (this._BindYuanBaoDict)
			{
				if (this._BindYuanBaoDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("BindYuanBao", -1);
				lock (this._BindYuanBaoDict)
				{
					this._BindYuanBaoDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindLingLi(int taskID)
		{
			int num = -1;
			lock (this._LingLiDict)
			{
				if (this._LingLiDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("LingLi", -1);
				lock (this._LingLiDict)
				{
					this._LingLiDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindBlessPoint(int taskID)
		{
			int num = -1;
			lock (this._BlessPointDict)
			{
				if (this._BlessPointDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("BlessPoint", -1);
				lock (this._BlessPointDict)
				{
					this._BlessPointDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindZhenQi(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._ZhenQiDict)
			{
				if (this._ZhenQiDict.TryGetValue(taskID, out num))
				{
					if (num < 0)
					{
						num = (int)this.CalcLuaScript(client, taskID, null, "ZhenQiLua");
					}
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("ZhenQi", -1);
				lock (this._ZhenQiDict)
				{
					this._ZhenQiDict[taskID] = num;
				}
				if (num < 0)
				{
					num = (int)this.CalcLuaScript(client, taskID, systemXmlItem, "ZhenQiLua");
				}
				result = num;
			}
			return result;
		}

		public int FindLieSha(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._LieShaDict)
			{
				if (this._LieShaDict.TryGetValue(taskID, out num))
				{
					if (num < 0)
					{
						num = (int)this.CalcLuaScript(client, taskID, null, "LieShaLua");
					}
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("LieSha", -1);
				lock (this._LieShaDict)
				{
					this._LieShaDict[taskID] = num;
				}
				if (num < 0)
				{
					num = (int)this.CalcLuaScript(client, taskID, systemXmlItem, "LieShaLua");
				}
				result = num;
			}
			return result;
		}

		public int FindWuXing(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._WuXingDict)
			{
				if (this._WuXingDict.TryGetValue(taskID, out num))
				{
					if (num < 0)
					{
						num = (int)this.CalcLuaScript(client, taskID, null, "WuXingLua");
					}
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("WuXing", -1);
				lock (this._WuXingDict)
				{
					this._WuXingDict[taskID] = num;
				}
				if (num < 0)
				{
					num = (int)this.CalcLuaScript(client, taskID, systemXmlItem, "WuXingLua");
				}
				result = num;
			}
			return result;
		}

		public int FindNeedYuanBao(GameClient client, int taskID)
		{
			int num = -1;
			SystemXmlItem systemTask = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
			{
				result = num;
			}
			else
			{
				num = (int)this.CalcLuaScript(client, taskID, systemTask, "DoubleAwardLua");
				result = num;
			}
			return result;
		}

		public int FindJunGong(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._JunGongDict)
			{
				if (this._JunGongDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("JunGong", -1);
				lock (this._JunGongDict)
				{
					this._JunGongDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindRongYu(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._RongYuDict)
			{
				if (this._RongYuDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("RongYu", -1);
				lock (this._RongYuDict)
				{
					this._RongYuDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindMoJing(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._JingYuanDict)
			{
				if (this._JingYuanDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("MoJing", -1);
				lock (this._JingYuanDict)
				{
					this._JingYuanDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindXingHun(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._XinHunAwardDict)
			{
				if (this._XinHunAwardDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("Xinghun", -1);
				lock (this._XinHunAwardDict)
				{
					this._XinHunAwardDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindCompDonate(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._CompDonateAwardDict)
			{
				if (this._CompDonateAwardDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("AwardCompHonor", -1);
				lock (this._CompDonateAwardDict)
				{
					this._CompDonateAwardDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public int FindCompJunXian(GameClient client, int taskID)
		{
			int num = -1;
			lock (this._CompJunXianAwardDict)
			{
				if (this._CompJunXianAwardDict.TryGetValue(taskID, out num))
				{
					return num;
				}
			}
			SystemXmlItem systemXmlItem = null;
			int result;
			if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemXmlItem))
			{
				result = num;
			}
			else
			{
				num = systemXmlItem.GetIntValue("AwardCompFeast", -1);
				lock (this._CompJunXianAwardDict)
				{
					this._CompJunXianAwardDict[taskID] = num;
				}
				result = num;
			}
			return result;
		}

		public void ClearAllDictionary()
		{
			lock (this._TaskAwardsDict)
			{
				this._TaskAwardsDict.Clear();
			}
			lock (this._OtherAwardsDict)
			{
				this._OtherAwardsDict.Clear();
			}
			lock (this._MoneyDict)
			{
				this._MoneyDict.Clear();
			}
			lock (this._ExperienceDict)
			{
				this._ExperienceDict.Clear();
			}
			lock (this._YinLiangDict)
			{
				this._YinLiangDict.Clear();
			}
			lock (this._BindYuanBaoDict)
			{
				this._BindYuanBaoDict.Clear();
			}
			lock (this._LingLiDict)
			{
				this._LingLiDict.Clear();
			}
			lock (this._BlessPointDict)
			{
				this._BlessPointDict.Clear();
			}
			lock (this._ZhenQiDict)
			{
				this._ZhenQiDict.Clear();
			}
			lock (this._LieShaDict)
			{
				this._LieShaDict.Clear();
			}
			lock (this._WuXingDict)
			{
				this._WuXingDict.Clear();
			}
			lock (this._NeedYuanBaoDict)
			{
				this._NeedYuanBaoDict.Clear();
			}
			lock (this._JunGongDict)
			{
				this._JunGongDict.Clear();
			}
			lock (this._RongYuDict)
			{
				this._RongYuDict.Clear();
			}
		}

		private long CalcLuaScript(GameClient client, int taskID, SystemXmlItem systemTask, string itemName)
		{
			if (null == systemTask)
			{
				if (!GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskID, out systemTask))
				{
					return -1L;
				}
			}
			long num = -1L;
			string text = systemTask.GetStringValue(itemName);
			long result;
			if (string.IsNullOrEmpty(text))
			{
				result = num;
			}
			else
			{
				text = DataHelper.CurrentDirectory + "scripts/tasks/" + text;
				object[] array = Global.ExcuteLuaFunction(client, text, "calcTaskAwards", null, null);
				if (array != null && array.Length > 0)
				{
					num = (long)array[0];
				}
				result = num;
			}
			return result;
		}

		private Dictionary<int, List<AwardsItemData>> _TaskAwardsDict = new Dictionary<int, List<AwardsItemData>>();

		private Dictionary<int, List<AwardsItemData>> _OtherAwardsDict = new Dictionary<int, List<AwardsItemData>>();

		private Dictionary<int, int> _MoneyDict = new Dictionary<int, int>();

		private Dictionary<int, long> _ExperienceDict = new Dictionary<int, long>();

		private Dictionary<int, int> _YinLiangDict = new Dictionary<int, int>();

		private Dictionary<int, int> _BindYuanBaoDict = new Dictionary<int, int>();

		private Dictionary<int, int> _LingLiDict = new Dictionary<int, int>();

		private Dictionary<int, int> _BlessPointDict = new Dictionary<int, int>();

		private Dictionary<int, int> _ZhenQiDict = new Dictionary<int, int>();

		private Dictionary<int, int> _LieShaDict = new Dictionary<int, int>();

		private Dictionary<int, int> _WuXingDict = new Dictionary<int, int>();

		private Dictionary<int, int> _NeedYuanBaoDict = new Dictionary<int, int>();

		private Dictionary<int, int> _JunGongDict = new Dictionary<int, int>();

		private Dictionary<int, int> _RongYuDict = new Dictionary<int, int>();

		private Dictionary<int, int> _JingYuanDict = new Dictionary<int, int>();

		private Dictionary<int, int> _XinHunAwardDict = new Dictionary<int, int>();

		private Dictionary<int, int> _CompDonateAwardDict = new Dictionary<int, int>();

		private Dictionary<int, int> _CompJunXianAwardDict = new Dictionary<int, int>();
	}
}
