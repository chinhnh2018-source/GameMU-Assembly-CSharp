using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic.WanMota
{
	internal class SweepWanMotaManager
	{
		public static int GetSweepCount(GameClient client)
		{
			FuBenData fuBenData = Global.GetFuBenData(client, SweepWanMotaManager.nWanMoTaSweepFuBenOrder);
			int result;
			if (null == fuBenData)
			{
				result = 0;
			}
			else
			{
				result = fuBenData.EnterNum;
			}
			return result;
		}

		public static void SweepBegin(GameClient client)
		{
			if (client.ClientData.WanMoTaProp.nPassLayerCount >= SweepWanMotaManager.nSweepReqMinLayerOrder)
			{
				if (null == client.ClientData.WanMoTaSweeping)
				{
					client.ClientData.WanMoTaSweeping = new SweepWanmota(client);
				}
				client.ClientData.WanMoTaSweeping.nSweepingOrder = 1;
				client.ClientData.WanMoTaSweeping.nSweepingMaxOrder = client.ClientData.WanMoTaProp.nPassLayerCount;
				client.ClientData.WanMoTaProp.lFlushTime = TimeUtil.NOW();
				client.ClientData.WanMoTaSweeping.BeginSweeping();
				if (-1 != WanMoTaDBCommandManager.SweepBeginDBCommand(client, 1))
				{
					Global.UpdateFuBenData(client, SweepWanMotaManager.nWanMoTaSweepFuBenOrder, 1, 1);
				}
			}
		}

		public static void SweepContinue(GameClient client)
		{
			if (client.ClientData.WanMoTaProp.nPassLayerCount >= SweepWanMotaManager.nSweepReqMinLayerOrder)
			{
				if (null == client.ClientData.WanMoTaSweeping)
				{
					client.ClientData.WanMoTaSweeping = new SweepWanmota(client);
				}
				client.ClientData.WanMoTaSweeping.nSweepingOrder = client.ClientData.WanMoTaProp.nSweepLayer;
				client.ClientData.WanMoTaSweeping.nSweepingMaxOrder = client.ClientData.WanMoTaProp.nPassLayerCount;
				client.ClientData.WanMoTaSweeping.BeginSweeping();
			}
		}

		public static void UpdataSweepInfo(GameClient client, List<SingleLayerRewardData> listRewardData)
		{
			client.sendCmd<List<SingleLayerRewardData>>(617, listRewardData, false);
		}

		public static List<SingleLayerRewardData> SummarySweepRewardInfo(GameClient client)
		{
			List<SingleLayerRewardData> list = null;
			List<SingleLayerRewardData> result;
			if (client.ClientData.LayerRewardData == null || client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count < 1)
			{
				result = list;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				List<GoodsData> list2 = new List<GoodsData>();
				lock (client.ClientData.LayerRewardData)
				{
					for (int i = 0; i < client.ClientData.LayerRewardData.WanMoTaLayerRewardList.Count; i++)
					{
						num += client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].nExp;
						num2 += client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].nMoney;
						num3 += client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].nXinHun;
						if (null != client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].sweepAwardGoodsList)
						{
							for (int j = 0; j < client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].sweepAwardGoodsList.Count; j++)
							{
								SweepWanMotaManager.CombineGoodList(list2, client.ClientData.LayerRewardData.WanMoTaLayerRewardList[i].sweepAwardGoodsList[j]);
							}
						}
					}
					SingleLayerRewardData singleLayerRewardData = WanMotaCopySceneManager.AddSingleSweepReward(client, list2, 0, num, num2, num3, out list);
				}
				result = list;
			}
			return result;
		}

		public static void CombineGoodList(List<GoodsData> goodList, GoodsData goodData)
		{
			int goodsGridNumByID = Global.GetGoodsGridNumByID(goodData.GoodsID);
			if (goodsGridNumByID > 1)
			{
				for (int i = 0; i < goodList.Count; i++)
				{
					if (goodList[i].GoodsID == goodData.GoodsID)
					{
						if (goodList[i].GCount + goodData.GCount <= goodsGridNumByID)
						{
							goodList[i].GCount += goodData.GCount;
							return;
						}
					}
				}
			}
			goodList.Add(goodData);
		}

		public static readonly int nSweepReqMinLayerOrder = 1;

		public static readonly int nWanMoTaSweepFuBenOrder = 19999;

		public static readonly int nWanMoTaMaxSweepNum = 1;
	}
}
