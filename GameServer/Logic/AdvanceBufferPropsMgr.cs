using System;
using System.Collections.Generic;
using Server.Data;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class AdvanceBufferPropsMgr
	{
		private static int[] GetCachingIDsByID(int id)
		{
			int[] array = null;
			lock (AdvanceBufferPropsMgr.CachingIDsDict)
			{
				if (!AdvanceBufferPropsMgr.CachingIDsDict.TryGetValue(id, out array))
				{
					string name = "";
					if (AdvanceBufferPropsMgr.BufferId2ConfigParamsNameDict.TryGetValue((BufferItemTypes)id, out name))
					{
						array = GameManager.systemParamsList.GetParamValueIntArrayByName(name, ',');
					}
					AdvanceBufferPropsMgr.CachingIDsDict[id] = array;
				}
			}
			return array;
		}

		public static void ResetCache()
		{
			lock (AdvanceBufferPropsMgr.CachingIDsDict)
			{
				AdvanceBufferPropsMgr.CachingIDsDict.Clear();
				foreach (KeyValuePair<BufferItemTypes, string> keyValuePair in AdvanceBufferPropsMgr.BufferId2ConfigParamsNameDict)
				{
					int key = (int)keyValuePair.Key;
					string value = keyValuePair.Value;
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName(value, ',');
					AdvanceBufferPropsMgr.CachingIDsDict[key] = paramValueIntArrayByName;
				}
			}
		}

		public static int GetGoodsID(BufferItemTypes bufferItemType, int goodsIndex)
		{
			int[] cachingIDsByID = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferItemType);
			int result;
			if (null == cachingIDsByID)
			{
				result = -1;
			}
			else if (goodsIndex < 0 || goodsIndex >= cachingIDsByID.Length)
			{
				result = -1;
			}
			else
			{
				int num = cachingIDsByID[goodsIndex];
				result = num;
			}
			return result;
		}

		public static double GetExtProp(BufferItemTypes bufferItemType, ExtPropIndexes extPropIndexe, int goodsIndex)
		{
			int[] cachingIDsByID = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferItemType);
			double result;
			if (null == cachingIDsByID)
			{
				result = 0.0;
			}
			else if (goodsIndex < 0 || goodsIndex >= cachingIDsByID.Length)
			{
				result = 0.0;
			}
			else
			{
				int equipID = cachingIDsByID[goodsIndex];
				EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(equipID);
				if (null == equipPropItem)
				{
					result = 0.0;
				}
				else
				{
					result = equipPropItem.ExtProps[(int)extPropIndexe];
				}
			}
			return result;
		}

		public static double GetExtPropByGoodsID(BufferItemTypes bufferItemType, ExtPropIndexes extPropIndexe, int goodsID)
		{
			EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(goodsID);
			double result;
			if (null == equipPropItem)
			{
				result = 0.0;
			}
			else
			{
				result = equipPropItem.ExtProps[(int)extPropIndexe];
			}
			return result;
		}

		public static void AddTempBufferProp(GameClient client, BufferItemTypes bufferID, int type)
		{
			EquipPropItem equipPropItem = null;
			if (Global.CanMapUseBuffer(client.ClientData.MapCode, (int)bufferID))
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)bufferID);
				if (null != bufferDataByID)
				{
					if (!Global.IsBufferDataOver(bufferDataByID, 0L))
					{
						int num = 0;
						if (type == 0)
						{
							int num2;
							if (bufferID == BufferItemTypes.ZuanHuang)
							{
								num2 = client.ClientData.VipLevel;
							}
							else
							{
								num2 = (int)bufferDataByID.BufferVal;
							}
							int[] cachingIDsByID = AdvanceBufferPropsMgr.GetCachingIDsByID((int)bufferID);
							if (null == cachingIDsByID)
							{
								goto IL_F8;
							}
							if (num2 < 0 || num2 >= cachingIDsByID.Length)
							{
								goto IL_F8;
							}
							num = cachingIDsByID[num2];
						}
						else if (type == 1)
						{
							num = (int)bufferDataByID.BufferVal;
						}
						if (num > 0)
						{
							equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(num);
						}
					}
				}
			}
			IL_F8:
			if (null != equipPropItem)
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.BufferByGoodsProps,
					bufferID,
					equipPropItem.ExtProps
				});
			}
			else
			{
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.BufferByGoodsProps,
					bufferID,
					PropsCacheManager.ConstExtProps
				});
			}
		}

		public static void DoSpriteBuffers(GameClient client)
		{
			int age = client.ClientData.PropsCacheManager.GetAge();
			foreach (KeyValuePair<BufferItemTypes, int> keyValuePair in AdvanceBufferPropsMgr.BufferId2ConfigTypeDict)
			{
				if (keyValuePair.Value >= 0)
				{
					AdvanceBufferPropsMgr.AddTempBufferProp(client, keyValuePair.Key, keyValuePair.Value);
				}
			}
			if (age != client.ClientData.PropsCacheManager.GetAge())
			{
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					2
				});
			}
		}

		private static readonly Dictionary<BufferItemTypes, string> BufferId2ConfigParamsNameDict = new Dictionary<BufferItemTypes, string>
		{
			{
				BufferItemTypes.ChengJiu,
				"ChengJiuBufferGoodsIDs"
			},
			{
				BufferItemTypes.JingMai,
				"JingMaiBufferGoodsIDs"
			},
			{
				BufferItemTypes.WuXue,
				"WuXueBufferGoodsIDs"
			},
			{
				BufferItemTypes.ZuanHuang,
				"ZhuanhuangBufferGoodsIDs"
			},
			{
				BufferItemTypes.ZhanHun,
				"ZhanhunBufferGoodsIDs"
			},
			{
				BufferItemTypes.RongYu,
				"RongyaoBufferGoodsIDs"
			},
			{
				BufferItemTypes.JunQi,
				"JunQiBufferGoodsIDs"
			},
			{
				BufferItemTypes.MU_FRESHPLAYERBUFF,
				"FreshPlayerBufferGoodsIDs"
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF1,
				"AngelTempleGoldBuffGoodsID"
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF2,
				"AngelTempleGoldBuffGoodsID"
			},
			{
				BufferItemTypes.MU_JINGJICHANG_JUNXIAN,
				"JunXianBufferGoodsIDs"
			},
			{
				BufferItemTypes.MU_WORLDLEVEL,
				"WorldLevelGoodsIDs"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI,
				"ZhanMengZhanQiBUFF"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JITAN,
				"ZhanMengJiTanBUFF"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JUNXIE,
				"ZhanMengJunXieBUFF"
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_GUANGHUAN,
				"ZhanMengGuangHuanBUFF"
			}
		};

		private static readonly Dictionary<BufferItemTypes, int> BufferId2ConfigTypeDict = new Dictionary<BufferItemTypes, int>
		{
			{
				BufferItemTypes.ChengJiu,
				0
			},
			{
				BufferItemTypes.ZuanHuang,
				0
			},
			{
				BufferItemTypes.ZhanHun,
				0
			},
			{
				BufferItemTypes.RongYu,
				0
			},
			{
				BufferItemTypes.JunQi,
				0
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF1,
				0
			},
			{
				BufferItemTypes.MU_ANGELTEMPLEBUFF2,
				0
			},
			{
				BufferItemTypes.MU_JINGJICHANG_JUNXIAN,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_ZHANQI,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JITAN,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_JUNXIE,
				0
			},
			{
				BufferItemTypes.MU_ZHANMENGBUILD_GUANGHUAN,
				0
			},
			{
				BufferItemTypes.JieRiChengHao,
				1
			}
		};

		private static Dictionary<int, int[]> CachingIDsDict = new Dictionary<int, int[]>();
	}
}
