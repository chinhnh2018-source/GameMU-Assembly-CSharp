using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using XMLCreater;

public class ShenHunData
{
	public static MUTransfigurationLevel GetTransfigurationLevelByOccupationLevel(int occupation, int level)
	{
		return IConfigbase<ConfigShenHun>.Instance.GetTransfigurationLevelByOccupationLevel(occupation, level);
	}

	public static List<int> GetAllSkills(int occupation)
	{
		MUTransfigurationLevel lastLevelTransfigurationLevelByOccupation = IConfigbase<ConfigShenHun>.Instance.GetLastLevelTransfigurationLevelByOccupation(occupation);
		if (lastLevelTransfigurationLevelByOccupation == null)
		{
			return new List<int>();
		}
		return ShenHunData.GetSelfSkill(lastLevelTransfigurationLevelByOccupation);
	}

	public static int GetJieSuoSkillLevel(int occupation, int skillIndex)
	{
		return IConfigbase<ConfigShenHun>.Instance.GetJieSuoLevel(occupation, skillIndex);
	}

	public static List<int> GetAllFashionSkills(int goodsId)
	{
		MUTransfigurationFashion topLevelFashion = IConfigbase<ConfigShenHun>.Instance.GetTopLevelFashion(goodsId);
		if (topLevelFashion == null)
		{
			return new List<int>();
		}
		return ShenHunData.GetSelfSkill(topLevelFashion);
	}

	public static List<MUTransfigurationFashion> GetAllFashionTopLevel()
	{
		return IConfigbase<ConfigShenHun>.Instance.GetAllFashionTopLevel();
	}

	public static RoleBianShenData GetSelfShenHunLevel()
	{
		RoleBianShenData roleBianShenData = Global.Data.roleData.BianShenData;
		if (roleBianShenData == null)
		{
			roleBianShenData = new RoleBianShenData();
			roleBianShenData.BianShen = 1;
			roleBianShenData.Exp = 0;
		}
		if (roleBianShenData.BianShen == 0)
		{
			roleBianShenData.BianShen = 1;
		}
		return roleBianShenData;
	}

	public static long GetFreeBianShenNum()
	{
		long num = Global.Data.roleData.MoneyData[147];
		long num2 = num / 10000L;
		long nowDayFrom = Global.GetNowDayFrom20111111();
		long num3 = nowDayFrom - num2;
		if (num3 > 0L)
		{
			return ShenHunData.GetTotalFreeNum();
		}
		long num4 = num % 10000L;
		return ShenHunData.GetTotalFreeNum() - num4;
	}

	public static int GetBianShenGoodsNum()
	{
		int bianShenGoodsId = ShenHunData.GetBianShenGoodsId();
		return Global.GetRoleGoodsNumberCountByGoodsID(bianShenGoodsId);
	}

	public static long GetTotalFreeNum()
	{
		return ConfigSystemParam.GetSystemParamIntByName("TransfigurationFree");
	}

	public static int GetBianShenGoodsId()
	{
		return ConfigSystemParam.GetSystemParamIntArrayByName("TransfigurationGoods", ',')[0];
	}

	public static long GetBianShenCD()
	{
		return ConfigSystemParam.GetSystemParamIntByName("TransfigurationCD") * 1000L;
	}

	public static int GetBianModel(RoleData roleData)
	{
		List<GoodsData> list = roleData.GoodsDataList;
		if (roleData == Global.Data.roleData)
		{
			list = Global.Data.fashionAndTitleList;
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				int goodsID = list[i].GoodsID;
				int forge_level = list[i].Forge_level;
				if (Global.GetGoodsCatetoriy(goodsID) == 28 && list[i].Using == 1)
				{
					MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsID, forge_level);
					if (transfigurationFashion != null)
					{
						return transfigurationFashion.MOD;
					}
				}
			}
		}
		int level = 1;
		if (roleData.BianShenData != null)
		{
			if (roleData.BianShenData.BianShen <= 0)
			{
				roleData.BianShenData.BianShen = 1;
			}
			level = roleData.BianShenData.BianShen;
		}
		MUTransfigurationLevel transfigurationLevelByOccupationLevel = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationLevelByOccupationLevel(roleData.Occupation, level);
		return transfigurationLevelByOccupationLevel.GodMod;
	}

	public static List<int> GetSelfBianShenSkill()
	{
		RoleData roleData = Global.Data.roleData;
		if (Global.Data.fashionAndTitleList != null)
		{
			for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
			{
				int goodsID = Global.Data.fashionAndTitleList[i].GoodsID;
				int forge_level = Global.Data.fashionAndTitleList[i].Forge_level;
				if (Global.GetGoodsCatetoriy(goodsID) == 28 && Global.Data.fashionAndTitleList[i].Using == 1)
				{
					MUTransfigurationFashion transfigurationFashion = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationFashion(goodsID, forge_level);
					if (transfigurationFashion != null)
					{
						return ShenHunData.GetSelfSkill(transfigurationFashion);
					}
				}
			}
		}
		int level = 1;
		if (roleData.BianShenData != null)
		{
			if (roleData.BianShenData.BianShen <= 0)
			{
				roleData.BianShenData.BianShen = 1;
			}
			level = roleData.BianShenData.BianShen;
		}
		MUTransfigurationLevel transfigurationLevelByOccupationLevel = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationLevelByOccupationLevel(roleData.Occupation, level);
		return ShenHunData.GetSelfSkill(transfigurationLevelByOccupationLevel);
	}

	public static List<int> GetSelfSkill(MUTransfigurationLevel info)
	{
		int num = Global.Data.roleData.Occupation;
		if (num == 3)
		{
			if (Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword)
			{
				num = 3;
			}
			else
			{
				num = 4;
			}
		}
		switch (num)
		{
		case 0:
		case 2:
		case 3:
			return info.AttackSkill;
		case 1:
		case 4:
		case 5:
			return info.MagicSkill;
		default:
			return info.AttackSkill;
		}
	}

	public static List<int> GetSelfSkill(MUTransfigurationFashion info)
	{
		int num = Global.Data.roleData.Occupation;
		if (num == 3)
		{
			if (Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword)
			{
				num = 3;
			}
			else
			{
				num = 4;
			}
		}
		switch (num)
		{
		case 0:
		case 2:
		case 3:
			return info.AttackSkill;
		case 1:
		case 4:
		case 5:
			return info.MagicSkill;
		default:
			return info.AttackSkill;
		}
	}

	public static List<GoodsData> GetBianShenWeapon(RoleData roleData)
	{
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> list2 = roleData.GoodsDataList;
		if (roleData == Global.Data.roleData)
		{
			list2 = Global.Data.fashionAndTitleList;
		}
		if (list2 != null)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				int goodsID = list2[i].GoodsID;
				if (Global.GetGoodsCatetoriy(goodsID) == 28 && list2[i].Using == 1)
				{
					return list;
				}
			}
		}
		int level = 1;
		if (roleData.BianShenData != null)
		{
			if (roleData.BianShenData.BianShen <= 0)
			{
				roleData.BianShenData.BianShen = 1;
			}
			level = roleData.BianShenData.BianShen;
		}
		MUTransfigurationLevel transfigurationLevelByOccupationLevel = IConfigbase<ConfigShenHun>.Instance.GetTransfigurationLevelByOccupationLevel(roleData.Occupation, level);
		int weaponMOd = transfigurationLevelByOccupationLevel.WeaponMOd;
		GoodsData dummyGoodsData = Global.GetDummyGoodsData(weaponMOd);
		dummyGoodsData.Using = 1;
		list.Add(dummyGoodsData);
		return list;
	}

	public static float[] GetDecorationHeights(RoleData roleData)
	{
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> list2 = roleData.GoodsDataList;
		if (roleData == Global.Data.roleData)
		{
			list2 = Global.Data.fashionAndTitleList;
		}
		if (list2 != null)
		{
			for (int i = 0; i < list2.Count; i++)
			{
				int goodsID = list2[i].GoodsID;
				if (Global.GetGoodsCatetoriy(goodsID) == 28 && list2[i].Using == 1)
				{
					return IConfigbase<ConfigShenHun>.Instance.GetDecorationHeights(goodsID);
				}
			}
		}
		return IConfigbase<ConfigShenHun>.Instance.GetDecorationHeights(0);
	}

	public static float GetNameDisplayHeight(RoleData roleData)
	{
		float[] decorationHeights = ShenHunData.GetDecorationHeights(roleData);
		if (decorationHeights.Length >= 3)
		{
			return decorationHeights[2];
		}
		return 0f;
	}

	public static float GetQiang20Scale(RoleData roleData)
	{
		float[] decorationHeights = ShenHunData.GetDecorationHeights(roleData);
		if (decorationHeights.Length >= 3)
		{
			return decorationHeights[0];
		}
		return 1f;
	}

	public static float GetHuiJiScale(RoleData roleData)
	{
		float[] decorationHeights = ShenHunData.GetDecorationHeights(roleData);
		if (decorationHeights.Length >= 3)
		{
			return decorationHeights[1];
		}
		return 1f;
	}

	public static List<GoodsData> GetSelfBianShenFashions()
	{
		List<GoodsData> list = new List<GoodsData>();
		List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
		if (fashionAndTitleList != null)
		{
			for (int i = 0; i < fashionAndTitleList.Count; i++)
			{
				int goodsID = fashionAndTitleList[i].GoodsID;
				if (Global.GetGoodsCatetoriy(goodsID) == 28)
				{
					list.Add(fashionAndTitleList[i]);
				}
			}
		}
		return list;
	}

	public static bool IsFashionEquiped(int goodsId)
	{
		RoleData roleData = Global.Data.roleData;
		List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
		if (fashionAndTitleList != null)
		{
			for (int i = 0; i < fashionAndTitleList.Count; i++)
			{
				int forge_level = fashionAndTitleList[i].Forge_level;
				if (fashionAndTitleList[i].GoodsID == goodsId && Global.Data.fashionAndTitleList[i].Using == 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static GoodsData GetEquipedBianShenFashion()
	{
		RoleData roleData = Global.Data.roleData;
		List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
		if (fashionAndTitleList != null)
		{
			for (int i = 0; i < fashionAndTitleList.Count; i++)
			{
				if (Global.GetGoodsCatetoriy(fashionAndTitleList[i].GoodsID) == 28 && fashionAndTitleList[i].Using == 1)
				{
					return fashionAndTitleList[i];
				}
			}
		}
		return null;
	}

	public static bool IsBianShenNumEnough()
	{
		int num = (int)ShenHunData.GetFreeBianShenNum();
		if (num > 0)
		{
			return true;
		}
		int bianShenGoodsNum = ShenHunData.GetBianShenGoodsNum();
		return bianShenGoodsNum > 0;
	}

	public static bool IsInBianShenState()
	{
		return Global.IsBufferExist(121);
	}

	public static bool IsShenHunOpen()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		return GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ShenHunFuTi, ref num, ref num2, ref num3) && ConfigVersionSystemOpen.IsShenHunHuTiOpen();
	}
}
