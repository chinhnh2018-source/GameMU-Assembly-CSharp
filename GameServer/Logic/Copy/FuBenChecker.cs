using System;
using GameServer.Logic.WanMota;

namespace GameServer.Logic.Copy
{
	public static class FuBenChecker
	{
		public static bool HasFinishedPreTask(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int intValue = fubenItem.GetIntValue("TabID", -1);
				int fuBenTabNeedTask = GlobalNew.GetFuBenTabNeedTask(intValue);
				result = (fuBenTabNeedTask <= client.ClientData.MainTaskID);
			}
			return result;
		}

		public static bool HasPassedPreCopy(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int intValue = fubenItem.GetIntValue("UpCopyID", -1);
				int intValue2 = fubenItem.GetIntValue("FinishNumber", -1);
				if (intValue > 0 && intValue2 > 0)
				{
					if (!Global.FuBenPassed(client, intValue))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public static bool IsInCopyLevelLimit(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int num = fubenItem.GetIntValue("MinLevel", -1);
				int num2 = fubenItem.GetIntValue("MaxLevel", -1);
				num2 = ((num2 <= 0) ? 1000 : num2);
				int intValue = fubenItem.GetIntValue("MinZhuanSheng", -1);
				int num3 = fubenItem.GetIntValue("MaxZhuanSheng", -1);
				num3 = ((num3 <= 0) ? 1000 : num3);
				num = Global.GetUnionLevel(intValue, num, false);
				num2 = Global.GetUnionLevel(num3, num2, true);
				int unionLevel = Global.GetUnionLevel(client.ClientData.ChangeLifeCount, client.ClientData.Level, false);
				result = (unionLevel >= num && unionLevel <= num2);
			}
			return result;
		}

		public static bool IsInCopyTimesLimit(GameClient client, SystemXmlItem fubenItem)
		{
			bool result;
			if (client == null || fubenItem == null)
			{
				result = false;
			}
			else
			{
				int intValue = fubenItem.GetIntValue("ID", -1);
				if (WanMotaCopySceneManager.IsWanMoTaMapCode(intValue))
				{
					result = true;
				}
				else
				{
					int intValue2 = fubenItem.GetIntValue("EnterNumber", -1);
					int intValue3 = fubenItem.GetIntValue("FinishNumber", -1);
					int num;
					int fuBenEnterNum = Global.GetFuBenEnterNum(Global.GetFuBenData(client, intValue), out num);
					if (intValue2 <= 0 && intValue3 <= 0)
					{
						result = true;
					}
					else
					{
						int[] array;
						if (Global.IsInExperienceCopyScene(intValue))
						{
							array = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinYanFuBenNum", ',');
						}
						else if (intValue == 5100)
						{
							array = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPJinBiFuBenNum", ',');
						}
						else
						{
							array = null;
						}
						int num2 = 0;
						int vipLevel = client.ClientData.VipLevel;
						if (vipLevel > 0 && vipLevel <= VIPEumValue.VIPENUMVALUE_MAXLEVEL && array != null && array.Length > vipLevel)
						{
							num2 = array[vipLevel];
						}
						result = ((intValue2 <= 0 || fuBenEnterNum < intValue2 + num2) && (intValue3 <= 0 || num < intValue3 + num2));
					}
				}
			}
			return result;
		}
	}
}
