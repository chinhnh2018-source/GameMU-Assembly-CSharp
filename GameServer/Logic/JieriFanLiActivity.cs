using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class JieriFanLiActivity : Activity
	{
		public override bool GiveAward(GameClient client, int _params)
		{
			AwardItem awardItem = null;
			if (this.AwardDict.ContainsKey(_params))
			{
				awardItem = this.AwardDict[_params];
			}
			bool result;
			if (null == awardItem)
			{
				result = false;
			}
			else
			{
				this.GiveAward(client, _params, client.ClientData.Occupation);
				result = true;
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int _params1, int _params2)
		{
			AwardItem awardItem = null;
			if (this.AwardDict.ContainsKey(_params1))
			{
				awardItem = this.AwardDict[_params1];
			}
			bool result;
			if (null == awardItem)
			{
				result = false;
			}
			else
			{
				base.GiveAward(client, awardItem);
				awardItem = null;
				if (this.AwardDict2.ContainsKey(_params1))
				{
					awardItem = this.AwardDict2[_params1];
				}
				if (null != awardItem)
				{
					this.GiveAwardByOccupation(client, awardItem, _params2);
				}
				if (this.AwardDict3.ContainsKey(_params1))
				{
					awardItem = this.AwardDict3[_params1].ToAwardItem();
					base.GiveEffectiveTimeAward(client, awardItem);
				}
				result = true;
			}
			return result;
		}

		protected bool GiveAwardByOccupation(GameClient client, AwardItem myAwardItem, int occupation)
		{
			bool result;
			if (client == null || null == myAwardItem)
			{
				result = false;
			}
			else
			{
				if (myAwardItem.GoodsDataList != null && myAwardItem.GoodsDataList.Count > 0)
				{
					int count = myAwardItem.GoodsDataList.Count;
					for (int i = 0; i < count; i++)
					{
						GoodsData goodsData = myAwardItem.GoodsDataList[i];
						if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, "", goodsData.Forge_level, goodsData.Binding, 0, "", true, 1, Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), "1900-01-01 12:00:00", goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, null, null, 0, true);
						}
					}
				}
				if (myAwardItem.AwardYuanBao > 0)
				{
					GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType), ActivityTypes.None, "");
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(670, new object[0]) + myAwardItem.AwardYuanBao, new object[0]), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType)), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client)
		{
			int num = 0;
			int num2 = -1;
			int nOccu = Global.CalcOriginalOccupationID(client);
			foreach (int num3 in this.AwardDict.Keys)
			{
				int num4 = this.AwardDict[num3].GoodsDataList.Count;
				num4 += ((this.AwardDict2[num3].GoodsDataList.Count > 0) ? 1 : 0);
				num4 += this.AwardDict3[num3].GoodsCnt();
				if (num < num4)
				{
					num = num4;
					num2 = num3;
				}
			}
			bool result;
			if (-1 == num2)
			{
				result = true;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				foreach (GoodsData item in this.AwardDict[num2].GoodsDataList)
				{
					list.Add(item);
				}
				int count = this.AwardDict2[num2].GoodsDataList.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData goodsData = this.AwardDict2[num2].GoodsDataList[i];
					if (Global.IsRoleOccupationMatchGoods(nOccu, goodsData.GoodsID))
					{
						list.Add(goodsData);
					}
				}
				AwardItem awardItem = this.AwardDict3[num2].ToAwardItem();
				foreach (GoodsData item in awardItem.GoodsDataList)
				{
					list.Add(item);
				}
				result = Global.CanAddGoodsDataList(client, list);
			}
			return result;
		}

		public override bool CheckCondition(GameClient client, int extTag)
		{
			AwardItem awardItem = null;
			if (this.AwardDict.ContainsKey(extTag))
			{
				awardItem = this.AwardDict[extTag];
			}
			bool result;
			if (null == awardItem)
			{
				result = false;
			}
			else
			{
				ActivityTypes activityType = (ActivityTypes)this.ActivityType;
				switch (activityType)
				{
				case ActivityTypes.JieriWing:
					if (client.ClientData.MyWingData == null || client.ClientData.MyWingData.Using == 0)
					{
						return false;
					}
					if (client.ClientData.MyWingData.WingID < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					if (client.ClientData.MyWingData.WingID == awardItem.MinAwardCondionValue && client.ClientData.MyWingData.ForgeLevel < awardItem.MinAwardCondionValue2)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriAddon:
					if (client.UsingEquipMgr.GetUsingEquipAllAppendPropLeva() < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriStrengthen:
					if (client.UsingEquipMgr.GetUsingEquipAllForge() < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriAchievement:
					if (ChengJiuManager.GetChengJiuLevel(client) < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriMilitaryRank:
					if (GameManager.ClientMgr.GetShengWangLevelValue(client) < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriVIPFanli:
					if (client.ClientData.VipLevel < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriAmulet:
				{
					GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 22);
					if (null == goodsDataByCategoriy)
					{
						return false;
					}
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsDataByCategoriy.GoodsID, out systemXmlItem))
					{
						return false;
					}
					int intValue = systemXmlItem.GetIntValue("SuitID", -1);
					if (intValue < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				}
				case ActivityTypes.JieriArchangel:
					if (client.UsingEquipMgr.GetUsingEquipArchangelWeaponSuit() < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				case ActivityTypes.JieriLianXuCharge:
					break;
				case ActivityTypes.JieriMarriage:
					if (client.ClientData.MyMarriageData == null || -1 == client.ClientData.MyMarriageData.byMarrytype || (int)client.ClientData.MyMarriageData.byGoodwilllevel < awardItem.MinAwardCondionValue)
					{
						return false;
					}
					goto IL_3A3;
				default:
					switch (activityType)
					{
					case ActivityTypes.JieRiHuiJi:
					{
						EmblemStarInfo huiJiStartInfo = HuiJiManager.getInstance().GetHuiJiStartInfo(client);
						if (null == huiJiStartInfo)
						{
							return false;
						}
						if (huiJiStartInfo.EmblemLevel < awardItem.MinAwardCondionValue)
						{
							return false;
						}
						if (huiJiStartInfo.EmblemLevel == awardItem.MinAwardCondionValue && huiJiStartInfo.EmblemStar < awardItem.MinAwardCondionValue2)
						{
							return false;
						}
						goto IL_3A3;
					}
					case ActivityTypes.JieRiFuWen:
						if (ShenShiManager.getInstance().GetCurrentTabTotalLevel(client) < awardItem.MinAwardCondionValue)
						{
							return false;
						}
						goto IL_3A3;
					}
					break;
				}
				return false;
				IL_3A3:
				result = true;
			}
			return result;
		}

		public Dictionary<int, AwardItem> AwardDict = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardItem> AwardDict2 = new Dictionary<int, AwardItem>();

		public Dictionary<int, AwardEffectTimeItem> AwardDict3 = new Dictionary<int, AwardEffectTimeItem>();
	}
}
