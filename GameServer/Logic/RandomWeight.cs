using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	internal class RandomWeight
	{
		public static int GetWeightIndex(List<int> WeightList, string info = "")
		{
			List<WeightObject> list;
			int maxWeight = RandomWeight.GetMaxWeight(WeightList, out list);
			int randomValue = Global.GetRandomNumber(0, maxWeight);
			if (!string.IsNullOrEmpty(info))
			{
				LogManager.WriteLog(0, string.Format("[ljl]{0}, 权重={1}, maxWeight={2}", info, randomValue, maxWeight), null, true);
			}
			return list.Find((WeightObject x) => x.RegionMin <= randomValue && x.RegionMax > randomValue).Index;
		}

		private static int GetMaxWeight(List<int> WeightList, out List<WeightObject> dList)
		{
			int num = 0;
			int num2 = 0;
			dList = new List<WeightObject>();
			foreach (int num3 in WeightList)
			{
				WeightObject weightObject = new WeightObject();
				weightObject.RegionMin = num2;
				num2 += num3;
				weightObject.RegionMax = num2;
				weightObject.Index = num;
				num++;
				dList.Add(weightObject);
			}
			return num2;
		}
	}
}
