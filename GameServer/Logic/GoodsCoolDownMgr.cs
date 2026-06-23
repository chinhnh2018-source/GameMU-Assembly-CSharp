using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class GoodsCoolDownMgr
	{
		public bool GoodsCoolDown(int goodsID)
		{
			CoolDownItem coolDownItem = null;
			bool result;
			if (!this.GoodsCoolDownDict.TryGetValue(goodsID, out coolDownItem))
			{
				result = true;
			}
			else
			{
				long num = TimeUtil.NOW();
				result = (num > coolDownItem.StartTicks + coolDownItem.CDTicks);
			}
			return result;
		}

		public void AddGoodsCoolDown(GameClient client, int goodsID)
		{
			SystemXmlItem systemXmlItem = null;
			if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsID, out systemXmlItem))
			{
				int intValue = systemXmlItem.GetIntValue("CDTime", -1);
				if (intValue > 0)
				{
					int intValue2 = systemXmlItem.GetIntValue("PubCDTime", -1);
					int intValue3 = systemXmlItem.GetIntValue("ShareGroupID", -1);
					long startTicks = TimeUtil.NOW();
					Global.AddCoolDownItem(this.GoodsCoolDownDict, goodsID, startTicks, (long)(intValue * 1000));
					if (intValue3 > 0)
					{
						if (null != client.ClientData.GoodsDataList)
						{
							for (int i = 0; i < client.ClientData.GoodsDataList.Count; i++)
							{
								GoodsData goodsData = client.ClientData.GoodsDataList[i];
								if (null != goodsData)
								{
									if (goodsData.Using <= 0)
									{
										SystemXmlItem systemXmlItem2 = null;
										if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem2))
										{
											if (null != systemXmlItem2)
											{
												if (intValue3 == systemXmlItem2.GetIntValue("ShareGroupID", -1))
												{
													Global.AddCoolDownItem(this.GoodsCoolDownDict, goodsData.GoodsID, startTicks, (long)(intValue2 * 1000));
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private Dictionary<int, CoolDownItem> GoodsCoolDownDict = new Dictionary<int, CoolDownItem>();
	}
}
