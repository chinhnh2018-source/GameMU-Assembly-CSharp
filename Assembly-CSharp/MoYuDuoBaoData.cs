using System;
using System.Collections.Generic;
using Server.Data;

public class MoYuDuoBaoData
{
	public static ZorkBattleGameStates DuoBaoState { get; set; }

	public static int TimeOff { get; set; }

	public static int Round { get; set; }

	public static string BestTeam { get; set; }

	public static string GetDuanWeiName(int id)
	{
		ZorkDanAwardVO zorkDanAwardVODataById = IConfigbase<ConfigMoYuDuoBao>.Instance.GetZorkDanAwardVODataById(id);
		if (zorkDanAwardVODataById != null)
		{
			return zorkDanAwardVODataById.RankLevel;
		}
		return string.Empty;
	}

	public static bool beCanShowDuoBaoEnter(int mapCode)
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ZorkWarEnterMap", ',');
		return systemParamIntArrayByName.IndexOf(mapCode) > -1;
	}

	public static void SetAppearTime(Dictionary<int, string> time)
	{
		MoYuDuoBaoData.m_DateTime.Clear();
		if (time == null)
		{
			return;
		}
		Dictionary<int, string>.Enumerator enumerator = time.GetEnumerator();
		while (enumerator.MoveNext())
		{
			DateTime dateTime = default(DateTime);
			KeyValuePair<int, string> keyValuePair = enumerator.Current;
			DateTime.TryParse(keyValuePair.Value, ref dateTime);
			Dictionary<int, DateTime> dateTime2 = MoYuDuoBaoData.m_DateTime;
			KeyValuePair<int, string> keyValuePair2 = enumerator.Current;
			dateTime2[keyValuePair2.Key] = dateTime;
		}
	}

	public static DateTime GetAppearTime(int id)
	{
		DateTime result;
		if (MoYuDuoBaoData.m_DateTime.TryGetValue(id, ref result))
		{
			return result;
		}
		return default(DateTime);
	}

	public static Dictionary<int, string> m_DateTimeStr = new Dictionary<int, string>();

	public static Dictionary<int, DateTime> m_DateTime = new Dictionary<int, DateTime>();
}
