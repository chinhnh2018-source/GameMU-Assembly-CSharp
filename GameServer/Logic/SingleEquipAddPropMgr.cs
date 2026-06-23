using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class SingleEquipAddPropMgr
	{
		private static List<double[]> GetCachingPropsList(SingleEquipProps singleEquipPropsMgr, int occupation, SystemXmlItem systemGoods)
		{
			int intValue = systemGoods.GetIntValue("Categoriy", -1);
			int intValue2 = systemGoods.GetIntValue("SuitID", -1);
			return singleEquipPropsMgr.GetSingleEquipPropsList(occupation, intValue, intValue2);
		}

		private static void ApplyNewPropsToEquipProps(double[] equipProps, double[] newProps, bool toAdd)
		{
		}

		public static void LoadAllSingleEquipProps()
		{
			SingleEquipAddPropMgr.LoadSingleEquipPropsForge();
			SingleEquipAddPropMgr.LoadSingleEquipPropsJewels();
			SingleEquipAddPropMgr.LoadSingleEquipPropsFuJia();
		}

		private static void LoadSingleEquipPropsForge()
		{
			SingleEquipAddPropMgr._SingleEquipPropsForgeMgr.LoadEquipPropItems("Config/SingleEquipAddProp/QiangHua/");
		}

		public static void ProcessSingleEquipPropsForge(double[] equipProps, int occupation, GoodsData goodsData, SystemXmlItem systemGoods, bool toAdd)
		{
			List<double[]> cachingPropsList = SingleEquipAddPropMgr.GetCachingPropsList(SingleEquipAddPropMgr._SingleEquipPropsForgeMgr, occupation, systemGoods);
			if (cachingPropsList != null && cachingPropsList.Count > 0)
			{
				int num;
				if (goodsData.Forge_level >= 10)
				{
					num = 3;
				}
				else if (goodsData.Forge_level >= 9)
				{
					num = 2;
				}
				else if (goodsData.Forge_level >= 7)
				{
					num = 1;
				}
				else
				{
					if (goodsData.Forge_level < 5)
					{
						return;
					}
					num = 0;
				}
				if (num < cachingPropsList.Count)
				{
					for (int i = 0; i <= num; i++)
					{
						double[] array = cachingPropsList[i];
						if (array == null || array.Length != 10)
						{
							break;
						}
						SingleEquipAddPropMgr.ApplyNewPropsToEquipProps(equipProps, array, toAdd);
					}
				}
			}
		}

		private static void LoadSingleEquipPropsJewels()
		{
			SingleEquipAddPropMgr._SingleEquipPropsJewelsMgr.LoadEquipPropItems("Config/SingleEquipAddProp/Jewels/");
		}

		public static void ProcessSingleEquipPropsJewels(double[] equipProps, int occupation, AllThingsCalcItem singleEquipJewels, SystemXmlItem systemGoods, bool toAdd)
		{
			List<double[]> cachingPropsList = SingleEquipAddPropMgr.GetCachingPropsList(SingleEquipAddPropMgr._SingleEquipPropsJewelsMgr, occupation, systemGoods);
			if (cachingPropsList != null && cachingPropsList.Count > 0)
			{
				int num;
				if (singleEquipJewels.TotalJewel8LevelNum >= 6)
				{
					num = 2;
				}
				else if (singleEquipJewels.TotalJewel6LevelNum + singleEquipJewels.TotalJewel7LevelNum + singleEquipJewels.TotalJewel8LevelNum >= 6)
				{
					num = 1;
				}
				else
				{
					if (singleEquipJewels.TotalJewel4LevelNum + singleEquipJewels.TotalJewel5LevelNum + singleEquipJewels.TotalJewel6LevelNum + singleEquipJewels.TotalJewel7LevelNum + singleEquipJewels.TotalJewel8LevelNum < 6)
					{
						return;
					}
					num = 0;
				}
				if (num < cachingPropsList.Count)
				{
					for (int i = 0; i <= num; i++)
					{
						double[] array = cachingPropsList[i];
						if (array == null || array.Length != 10)
						{
							break;
						}
						SingleEquipAddPropMgr.ApplyNewPropsToEquipProps(equipProps, array, toAdd);
					}
				}
			}
		}

		private static void LoadSingleEquipPropsFuJia()
		{
			SingleEquipAddPropMgr._SingleEquipPropsFuJiaMgr.LoadEquipPropItems("Config/SingleEquipAddProp/FuJia/");
		}

		public static void ProcessSingleEquipPropsFuJia(double[] equipProps, int occupation, GoodsData goodsData, SystemXmlItem systemGoods, bool toAdd)
		{
			List<double[]> cachingPropsList = SingleEquipAddPropMgr.GetCachingPropsList(SingleEquipAddPropMgr._SingleEquipPropsFuJiaMgr, occupation, systemGoods);
			if (cachingPropsList != null && cachingPropsList.Count > 0)
			{
				int num;
				if (goodsData.Quality >= 4)
				{
					num = 3;
				}
				else if (goodsData.Quality >= 3)
				{
					num = 2;
				}
				else if (goodsData.Quality >= 2)
				{
					num = 1;
				}
				else
				{
					if (goodsData.Quality < 1)
					{
						return;
					}
					num = 0;
				}
				if (num < cachingPropsList.Count)
				{
					double[] array = cachingPropsList[num];
					if (array != null && array.Length == 10)
					{
						int num2 = goodsData.AddPropIndex;
						num2 = Global.GMax(num2, 0);
						num2 = Global.GMin(num2, 10);
						double[] array2 = new double[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							double num3 = array[i];
							double num4 = num3 * (double)(1 + num2);
							array2[i] = num4;
						}
						SingleEquipAddPropMgr.ApplyNewPropsToEquipProps(equipProps, array2, toAdd);
					}
				}
			}
		}

		private static SingleEquipProps _SingleEquipPropsForgeMgr = new SingleEquipProps();

		private static SingleEquipProps _SingleEquipPropsJewelsMgr = new SingleEquipProps();

		private static SingleEquipProps _SingleEquipPropsFuJiaMgr = new SingleEquipProps();
	}
}
