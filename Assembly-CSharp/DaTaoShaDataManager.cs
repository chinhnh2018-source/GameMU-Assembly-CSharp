using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class DaTaoShaDataManager
{
	public static void Clear()
	{
		DaTaoShaDataManager.IsOver = false;
		DaTaoShaDataManager.EBattleStatus = EscapeBattleGameSceneStatuses.STATUS_NULL;
		DaTaoShaDataManager.ShowGuanZhanBtnCallBack = null;
		DaTaoShaDataManager.HideBuyAbutton = null;
		DaTaoShaDataManager.BuyMoShenBuffCallBack = null;
		DaTaoShaDataManager.ShowBuyMoShenBuffCallBack = null;
		DaTaoShaDataManager.WorldNavigationRadiusAndPoint = null;
		DaTaoShaDataManager.EGRadarMapBattleStatusCallBack = null;
		DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack = null;
		DaTaoShaDataManager.WorldNavigationRadiusAndPointCallBack = null;
		DaTaoShaDataManager.RadiusAndPointCallBack = null;
		DaTaoShaDataManager.ClearResWhenChangeMap();
		DaTaoShaDataManager.IsFirstInGreenCircle = false;
		DaTaoShaDataManager.IsFirstInShaLuStage = false;
		DaTaoShaDataManager.IsGuanZhan = false;
		DaTaoShaDataManager.EscapeBattleGodCount = 0;
		DaTaoShaDataManager.EscapeBattleDevilCount = 0;
		DaTaoShaDataManager.RelifeCount = 0;
		DaTaoShaDataManager.LifeSeedNums.Clear();
		DaTaoShaDataManager.CacheEffectID = 0;
		DaTaoShaDataManager.ShowShaLuTips = null;
		DaTaoShaDataManager.TianShenBuffDes = string.Empty;
		DaTaoShaDataManager.EMoBuffDes = string.Empty;
	}

	public static void LoadEffectCircle(int key, Vector3 pos)
	{
		if (DaTaoShaDataManager.EffectDict.Count > 0 && DaTaoShaDataManager.EffectDict.ContainsKey(key))
		{
			return;
		}
		if (key == DaTaoShaDataManager.CacheEffectID)
		{
			return;
		}
		DaTaoShaDataManager.CacheEffectID = key;
		DaTaoShaDataManager.DeleteEffectCircle();
		key = IConfigbase<ConfigDaTaoSha>.Instance.GetEffectCircleResNameById(key);
		if (key < 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"LoadEffectCircle key " + key
			});
			return;
		}
		string bundleID = MuAssetManager.GetBundleID("Decoration", ConfigDecoration.GetDecorationVOByCode(key).ResName);
		string name = "DaTaoShaCircle" + key;
		GameObject emptyLoader = U3DUtils.GetEmptyLoader(name, bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
		DaTaoShaDataManager.EffectDict[key] = emptyLoader;
		emptyLoader.transform.localPosition = pos;
	}

	public static GameObject GetEffectCircleByID(int id)
	{
		GameObject result = null;
		if (DaTaoShaDataManager.EffectDict.TryGetValue(id, ref result))
		{
			return result;
		}
		return result;
	}

	public static void DeleteEffectCircleByID(int id)
	{
		GameObject gameObject = null;
		if (DaTaoShaDataManager.EffectDict.TryGetValue(id, ref gameObject) && gameObject != null)
		{
			Object.Destroy(DaTaoShaDataManager.EffectDict[id]);
			DaTaoShaDataManager.EffectDict.Remove(id);
		}
	}

	public static void DeleteEffectCircle()
	{
		DaTaoShaDataManager.ClearResWhenChangeMap();
	}

	public static void ClearResWhenChangeMap()
	{
		foreach (KeyValuePair<int, GameObject> keyValuePair in DaTaoShaDataManager.EffectDict)
		{
			Object.Destroy(keyValuePair.Value);
		}
		DaTaoShaDataManager.EffectDict.Clear();
	}

	public static void RefreshUICirclePos(EscapeBattleSideScore data)
	{
		if (DaTaoShaDataManager.RadiusAndPointCallBack != null)
		{
			DaTaoShaDataManager.RadiusAndPointCallBack.Invoke(data);
		}
		if (DaTaoShaDataManager.WorldNavigationRadiusAndPointCallBack != null)
		{
			DaTaoShaDataManager.WorldNavigationRadiusAndPointCallBack.Invoke(data);
		}
		DaTaoShaDataManager.WorldNavigationRadiusAndPoint = data;
	}

	public static Vector2 LastSafeArea
	{
		get
		{
			return DaTaoShaDataManager.lastSafeArea;
		}
		set
		{
			DaTaoShaDataManager.lastSafeArea = value;
		}
	}

	public static bool IsDisplayInviteWindowInCurrentMap()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EscapeTeamApply", ',');
		for (int i = 0; i < systemParamIntArrayByName.Length; i++)
		{
			if (Global.Data.roleData != null && systemParamIntArrayByName[i] == Global.Data.roleData.MapCode)
			{
				return true;
			}
		}
		return false;
	}

	public static int MaxCaiJiShengMingCount
	{
		get
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("LifeSeedNum", '|');
			string[] array = systemParamStringArrayByName[systemParamStringArrayByName.Length - 1].Split(new char[]
			{
				','
			});
			return array[0].SafeToInt32(0);
		}
	}

	public static List<int> GetCaiJiNum()
	{
		if (DaTaoShaDataManager.LifeSeedNums != null && DaTaoShaDataManager.LifeSeedNums.Count > 0)
		{
			return DaTaoShaDataManager.LifeSeedNums;
		}
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("LifeSeedNum", '|');
		if (systemParamStringArrayByName.Length >= 3)
		{
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				if (array.Length > 0)
				{
					int num = array[0].SafeToInt32(0);
					DaTaoShaDataManager.LifeSeedNums.Add(num);
				}
			}
		}
		return DaTaoShaDataManager.LifeSeedNums;
	}

	public static int GetLimitLifeSeed(int caiJiCount)
	{
		List<int> caiJiNum = DaTaoShaDataManager.GetCaiJiNum();
		int num = 0;
		for (int i = 0; i < caiJiNum.Count; i++)
		{
			int num2 = caiJiNum[i];
			if (num <= caiJiCount && caiJiCount < num2)
			{
				return num2;
			}
			num = num2;
		}
		return num;
	}

	public static void BuffDataDes(EscapeBattlePropNotify buffDatas)
	{
		int type = buffDatas.Type;
		if (type < 0)
		{
			return;
		}
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("BuffAttributeType", ',');
		if (buffDatas.MergeProp == null)
		{
			return;
		}
		double[] array = null;
		StringBuilder stringBuilder = new StringBuilder();
		if (buffDatas.MergeProp.TryGetValue(type, ref array))
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (i < systemParamIntArrayByName.Length)
				{
					stringBuilder.Append(ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(systemParamIntArrayByName[i], false));
					stringBuilder.Append(":");
					stringBuilder.Append((int)array[systemParamIntArrayByName[i]]);
					stringBuilder.Append("\n");
				}
			}
		}
		if (type == 0)
		{
			DaTaoShaDataManager.TianShenBuffDes = stringBuilder.ToString();
		}
		else
		{
			DaTaoShaDataManager.EMoBuffDes = stringBuilder.ToString();
		}
	}

	public static List<EscapeBattleJoinRoleInfo> CacheEscapeBattleJoinRoleInfo = null;

	public static bool IsOver = false;

	public static bool IsGuanZhan = false;

	public static int EscapeBattleGodCount = 0;

	public static int EscapeBattleDevilCount = 0;

	public static int RelifeCount = 0;

	private static Dictionary<int, GameObject> EffectDict = new Dictionary<int, GameObject>();

	private static int CacheEffectID = 0;

	public static Action ShowShaLuTips = null;

	public static Action<int> BianShenGuanZhanCallBack = null;

	public static Action<EscapeBattleGameStates> DaTaoShaSwitchCallBak = null;

	public static Action ShowGuanZhanBtnCallBack = null;

	public static Action HideBuyAbutton = null;

	public static Action<bool> BuyMoShenBuffCallBack = null;

	public static Action<bool> ShowBuyMoShenBuffCallBack = null;

	public static Action<int> RelifeCountCallBack = null;

	public static Action<EscapeBattleGameSceneStatuses> EGRadarMapBattleStatusCallBack = null;

	public static Action<EscapeBattleGameSceneStatuses> EWorldNavigationBattleStatusCallBack = null;

	public static EscapeBattleGameSceneStatuses EBattleStatus = EscapeBattleGameSceneStatuses.STATUS_NULL;

	public static Action<EscapeBattleSideScore> RadiusAndPointCallBack = null;

	public static Action<EscapeBattleSideScore> WorldNavigationRadiusAndPointCallBack = null;

	public static EscapeBattleSideScore WorldNavigationRadiusAndPoint = null;

	public static bool IsFirstInGreenCircle = false;

	public static bool IsFirstInShaLuStage = false;

	private static Vector2 lastSafeArea = default(Vector2);

	private static List<int> LifeSeedNums = new List<int>();

	public static string TianShenBuffDes = string.Empty;

	public static string EMoBuffDes = string.Empty;
}
