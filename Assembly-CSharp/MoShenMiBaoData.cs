using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class MoShenMiBaoData
{
	public static MazingerStoreData GetMazingerStoreData(MoShenMiBaoType type)
	{
		if (Global.Data.roleData.MazingerStoreDataInfo == null)
		{
			Global.Data.roleData.MazingerStoreDataInfo = new Dictionary<int, MazingerStoreData>();
		}
		MazingerStoreData mazingerStoreData = null;
		if (Global.Data.roleData.MazingerStoreDataInfo.TryGetValue((int)type, ref mazingerStoreData))
		{
			return mazingerStoreData;
		}
		mazingerStoreData = new MazingerStoreData();
		mazingerStoreData.RoleID = Global.Data.roleData.RoleID;
		mazingerStoreData.Type = (int)type;
		mazingerStoreData.Stage = 1;
		mazingerStoreData.StarLevel = 0;
		mazingerStoreData.Exp = 0;
		Global.Data.roleData.MazingerStoreDataInfo[mazingerStoreData.Type] = mazingerStoreData;
		return mazingerStoreData;
	}

	public static int GetStageLevel(MoShenMiBaoType type)
	{
		if (Global.Data.roleData.MazingerStoreDataInfo == null)
		{
			return 1;
		}
		MazingerStoreData mazingerStoreData = null;
		if (Global.Data.roleData.MazingerStoreDataInfo.TryGetValue((int)type, ref mazingerStoreData))
		{
			return mazingerStoreData.Stage;
		}
		return 1;
	}

	public static int GetStarLevel(MoShenMiBaoType type)
	{
		if (Global.Data.roleData.MazingerStoreDataInfo == null)
		{
			return 0;
		}
		MazingerStoreData mazingerStoreData = null;
		if (Global.Data.roleData.MazingerStoreDataInfo.TryGetValue((int)type, ref mazingerStoreData))
		{
			return mazingerStoreData.StarLevel;
		}
		return 0;
	}

	public static int GetExp(MoShenMiBaoType type)
	{
		if (Global.Data.roleData.MazingerStoreDataInfo == null)
		{
			return 0;
		}
		MazingerStoreData mazingerStoreData = null;
		if (Global.Data.roleData.MazingerStoreDataInfo.TryGetValue((int)type, ref mazingerStoreData))
		{
			return mazingerStoreData.Exp;
		}
		return 0;
	}
}
