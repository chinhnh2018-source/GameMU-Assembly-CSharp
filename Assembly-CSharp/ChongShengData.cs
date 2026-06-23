using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class ChongShengData
{
	public static bool IsChongShengVersionOpen()
	{
		return ConfigVersionSystemOpen.IsVersionSystemOpen(100103);
	}

	public static bool IsChongShengOpen()
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(105);
		return systemOpenVOByID != null && systemOpenVOByID.IsOpened;
	}

	public static bool IsChongShengBgOpen()
	{
		return Global.Data.roleData.RebornCount > 0 || Global.Data.roleData.RebornGoodsDataList != null;
	}

	public static List<GoodsData> GetChongShengGoodsDatas()
	{
		return Global.Data.roleData.RebornGoodsDataList;
	}

	public static int GetChongShengBagNum()
	{
		return Global.Data.roleData.RebornBagNum;
	}

	public static int GetChongShengPortableBagNum()
	{
		if (Global.Data.roleData.RebornGirdData == null)
		{
			return Global.DefaultPortableGridNum;
		}
		return Global.Data.roleData.RebornGirdData.ExtGridNum;
	}

	public static void SetChongShengPortableBagNum(int num)
	{
		if (Global.Data.roleData.RebornGirdData == null)
		{
			return;
		}
		Global.Data.roleData.RebornGirdData.ExtGridNum = num;
	}

	public static bool IsChongShengEquip(int type)
	{
		return type >= 30 && type <= 38;
	}

	public static Dictionary<int, GoodsData> GetUsingChongShengGoodsDataList()
	{
		Super.GData.RoleUsingChongShengGoodsDataList.Clear();
		List<GoodsData> chongShengGoodsDatas = ChongShengData.GetChongShengGoodsDatas();
		if (chongShengGoodsDatas != null)
		{
			for (int i = 0; i < chongShengGoodsDatas.Count; i++)
			{
				if (chongShengGoodsDatas[i].Using > 0)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(chongShengGoodsDatas[i].GoodsID);
					if (goodsXmlNodeByID != null && ChongShengData.IsChongShengEquip(goodsXmlNodeByID.Categoriy))
					{
						Super.GData.RoleUsingChongShengGoodsDataList[chongShengGoodsDatas[i].Id] = chongShengGoodsDatas[i];
					}
				}
			}
		}
		return Super.GData.RoleUsingChongShengGoodsDataList;
	}

	public static long GetRebornCuiLianNum()
	{
		return Global.Data.roleData.MoneyData[152];
	}

	public static long GetRebornDuanZaoNum()
	{
		return Global.Data.roleData.MoneyData[153];
	}

	public static long GetRebornNiePanNum()
	{
		return Global.Data.roleData.MoneyData[154];
	}

	public static bool BeCanEquip(GoodVO good)
	{
		int rebornCount = Global.Data.roleData.RebornCount;
		int rebornLevel = Global.Data.roleData.RebornLevel;
		int toReborn = good.ToReborn;
		int toRebornLevel = good.ToRebornLevel;
		return toReborn <= rebornCount && (toReborn < rebornLevel || rebornLevel >= toRebornLevel);
	}

	public static int GetNeedOpenPortableMoney()
	{
		return ConfigSystemParam.GetSystemParamIntArrayByName("RebornBagGridParams", ',')[0];
	}

	public static int GetNeedOpenBagMoney()
	{
		return ConfigSystemParam.GetSystemParamIntArrayByName("RebornBagGridParams", ',')[1];
	}

	public static int GetNeedOpenPortableMoneyMax()
	{
		return ConfigSystemParam.GetSystemParamIntArrayByName("RebornBagGridParams", ',')[2];
	}

	public static int GetNeedOpenBagMoneyMax()
	{
		return ConfigSystemParam.GetSystemParamIntArrayByName("RebornBagGridParams", ',')[3];
	}

	public static float GetEnlargeByZhuoYue(int zhuoYue)
	{
		float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("RebornZhuoYueAddRates", ',');
		if (zhuoYue < 3)
		{
			return 1f;
		}
		if (zhuoYue >= 3 && zhuoYue <= 4)
		{
			return 1f + systemParamFloatArrayByName[0];
		}
		if (zhuoYue == 5)
		{
			return 1f + systemParamFloatArrayByName[1];
		}
		if (zhuoYue == 6)
		{
			return 1f + systemParamFloatArrayByName[2];
		}
		if (zhuoYue > 6)
		{
			return 1f + systemParamFloatArrayByName[3];
		}
		return 1f;
	}

	public static bool beContainBaoShi(GoodsData goodsData)
	{
		if (goodsData == null)
		{
			return false;
		}
		if (goodsData.Props == null)
		{
			return false;
		}
		string[] array = goodsData.Props.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split(new char[]
				{
					'_'
				});
				int num = array2[0].SafeToInt32(0);
				int num2 = array2[2].SafeToInt32(0);
				if (num2 > 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	private const int ChongShengVersionID = 100103;
}
