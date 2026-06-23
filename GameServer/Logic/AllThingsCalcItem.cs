using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class AllThingsCalcItem
	{
		public static void InitAllForgeLevelInfo()
		{
			lock (AllThingsCalcItem.QiangHuaFuJiaItemList)
			{
				SystemXmlItems systemXmlItems = new SystemXmlItems();
				systemXmlItems.LoadFromXMlFile("Config/QiangHuaFuJia.xml", "", "ID", 0);
				AllThingsCalcItem.QiangHuaFuJiaItemList.Clear();
				foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in systemXmlItems.SystemXmlItemDict)
				{
					SystemXmlItem value = keyValuePair.Value;
					QiangHuaFuJiaItem qiangHuaFuJiaItem = new QiangHuaFuJiaItem();
					qiangHuaFuJiaItem.Id = value.GetIntValue("ID", -1);
					qiangHuaFuJiaItem.Level = value.GetIntValue("QiangHuaLevel", -1);
					qiangHuaFuJiaItem.Num = value.GetIntValue("Num", -1);
					qiangHuaFuJiaItem.AddAttackInjurePercent = value.GetDoubleValue("AddAttackInjurePercent");
					qiangHuaFuJiaItem.MaxLifePercent = value.GetDoubleValue("MaxLifePercent");
					AllThingsCalcItem.QiangHuaFuJiaItemList.Add(qiangHuaFuJiaItem);
				}
				AllThingsCalcItem.QiangHuaFuJiaItemList.Sort((QiangHuaFuJiaItem x, QiangHuaFuJiaItem y) => x.Id - y.Id);
				for (int i = 0; i < AllThingsCalcItem.QiangHuaFuJiaItemList.Count; i++)
				{
					AllThingsCalcItem.QiangHuaFuJiaItemList[i].Id = i + 1;
				}
			}
		}

		public void ChangeTotalForgeLevel(int level, bool toAdd)
		{
			lock (this.TotalForgeLevelAccDict)
			{
				int num = 0;
				foreach (QiangHuaFuJiaItem qiangHuaFuJiaItem in AllThingsCalcItem.QiangHuaFuJiaItemList)
				{
					if (qiangHuaFuJiaItem.Level <= level)
					{
						if (toAdd)
						{
							if (this.TotalForgeLevelAccDict.TryGetValue(qiangHuaFuJiaItem.Level, out num))
							{
								this.TotalForgeLevelAccDict[qiangHuaFuJiaItem.Level] = num + 1;
							}
							else
							{
								this.TotalForgeLevelAccDict[qiangHuaFuJiaItem.Level] = 1;
							}
						}
						else if (this.TotalForgeLevelAccDict.TryGetValue(qiangHuaFuJiaItem.Level, out num))
						{
							this.TotalForgeLevelAccDict[qiangHuaFuJiaItem.Level] = num - 1;
						}
					}
				}
			}
		}

		public int GetTotalForgeLevelValidIndex()
		{
			lock (this.TotalForgeLevelAccDict)
			{
				foreach (QiangHuaFuJiaItem qiangHuaFuJiaItem in AllThingsCalcItem.QiangHuaFuJiaItemList)
				{
					int num;
					if (this.TotalForgeLevelAccDict.TryGetValue(qiangHuaFuJiaItem.Level, out num) && qiangHuaFuJiaItem.Num <= num)
					{
						return qiangHuaFuJiaItem.Id;
					}
				}
			}
			return 0;
		}

		public static QiangHuaFuJiaItem GetQiangHuaFuJiaItem(int index)
		{
			if (index >= 0)
			{
				lock (AllThingsCalcItem.QiangHuaFuJiaItemList)
				{
					if (index < AllThingsCalcItem.QiangHuaFuJiaItemList.Count)
					{
						return AllThingsCalcItem.QiangHuaFuJiaItemList[index];
					}
				}
			}
			return null;
		}

		public int TotalPurpleQualityNum = 0;

		public int TotalGoldQualityNum = 0;

		public int TotalForge5LevelNum = 0;

		public int TotalForge7LevelNum = 0;

		public int TotalForge9LevelNum = 0;

		public int TotalForge11LevelNum = 0;

		public int TotalForge13LevelNum = 0;

		public int TotalForge15LevelNum = 0;

		public int TotalJewel4LevelNum = 0;

		public int TotalJewel5LevelNum = 0;

		public int TotalJewel6LevelNum = 0;

		public int TotalJewel7LevelNum = 0;

		public int TotalJewel8LevelNum = 0;

		public int TotalGreenZhuoYueNum = 0;

		public int TotalBlueZhuoYueNum = 0;

		public int TotalPurpleZhuoYueNum = 0;

		public Dictionary<int, int> TotalForgeLevelAccDict = new Dictionary<int, int>();

		public static List<QiangHuaFuJiaItem> QiangHuaFuJiaItemList = new List<QiangHuaFuJiaItem>();
	}
}
