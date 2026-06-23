using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Logic
{
	public class AdvanceBufferPropsMgr
	{
		private static int[] GetCachingIDsByID(int id)
		{
			int[] array = null;
			Dictionary<int, int[]> cachingIDsDict = AdvanceBufferPropsMgr.CachingIDsDict;
			lock (cachingIDsDict)
			{
				if (!AdvanceBufferPropsMgr.CachingIDsDict.TryGetValue(id, ref array))
				{
					string name = string.Empty;
					if (id == 31)
					{
						name = "ChengJiuBufferGoodsIDs";
					}
					else if (id == 32)
					{
						name = "JingMaiBufferGoodsIDs";
					}
					else if (id == 33)
					{
						name = "WuXueBufferGoodsIDs";
					}
					else if (id == 49)
					{
						name = "ZhuanhuangBufferGoodsIDs";
					}
					else if (id == 50)
					{
						name = "ZhanhunBufferGoodsIDs";
					}
					else if (id == 51)
					{
						name = "RongyaoBufferGoodsIDs";
					}
					else if (id == 16)
					{
						name = "JunQiBufferGoodsIDs";
					}
					else if (id == 70)
					{
						name = "FreshPlayerBufferGoodsIDs";
					}
					else if (id == 85)
					{
						name = "AngelTempleGoldBuffGoodsID";
					}
					else if (id == 86)
					{
						name = "AngelTempleGoldBuffGoodsID";
					}
					else if (id == 87)
					{
						name = "JunXianBufferGoodsIDs";
					}
					else if (id == 88)
					{
						name = "ZhanMengZhanQiBUFF";
					}
					else if (id == 89)
					{
						name = "ZhanMengJiTanBUFF";
					}
					else if (id == 90)
					{
						name = "ZhanMengJunXieBUFF";
					}
					else if (id == 91)
					{
						name = "ZhanMengGuangHuanBUFF";
					}
					else if (id == 2080011)
					{
						name = "CoupleBuffSpecificHurt";
					}
					else if (id == 2080010)
					{
						name = "CoupleVictoryNeedTime";
					}
					array = ConfigSystemParam.GetSystemParamIntArrayByName(name, ',');
					AdvanceBufferPropsMgr.CachingIDsDict[id] = array;
				}
			}
			return array;
		}

		public static int GetGoodsID(BufferItemTypes bufferItemType, int goodsIndex)
		{
			int[] cachingIDsByID = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferItemType);
			if (cachingIDsByID == null)
			{
				return -1;
			}
			if (goodsIndex < 0 || goodsIndex >= cachingIDsByID.Length)
			{
				return -1;
			}
			return cachingIDsByID[goodsIndex];
		}

		private static Dictionary<int, int[]> CachingIDsDict = new Dictionary<int, int[]>();
	}
}
