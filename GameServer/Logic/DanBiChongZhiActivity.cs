using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class DanBiChongZhiActivity : Activity
	{
		public override bool CheckCondition(GameClient client, int danBiID)
		{
			bool result;
			if (danBiID < 1 || danBiID > 9)
			{
				result = false;
			}
			else
			{
				lock (this.DanBiChongZhiAwardDic)
				{
					if (!this.DanBiChongZhiAwardDic.ContainsKey(danBiID))
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public string DBQueryInfoCmd()
		{
			string text = "";
			lock (this.DanBiChongZhiAwardDic)
			{
				foreach (KeyValuePair<int, DanBiChongZhiAwardDetail> keyValuePair in this.DanBiChongZhiAwardDic)
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						keyValuePair.Value.MinYuanBao,
						"_",
						keyValuePair.Value.MaxYuanBao,
						"_"
					});
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public bool CheckDanBiChongZhiCountOK(GameClient client, int danBiId)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = this.GetDanBiChongZhiAwardDetail(client, danBiId);
			bool result;
			if (danBiChongZhiAwardDetail == null)
			{
				result = false;
			}
			else
			{
				int num;
				if (danBiChongZhiAwardDetail.SinglePurchase > 127)
				{
					num = 127;
				}
				else
				{
					num = danBiChongZhiAwardDetail.SinglePurchase;
				}
				string cmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					this.FromDate.Replace(':', '$'),
					this.ToDate.Replace(':', '$'),
					this.DBQueryInfoCmd()
				});
				Dictionary<string, string> dictionary = Global.sendToDB<Dictionary<string, string>, string>(947, cmd, client.ServerId);
				if (dictionary != null && dictionary.Count<KeyValuePair<string, string>>() > 0)
				{
					string key = string.Format("{0}_{1}", danBiChongZhiAwardDetail.MinYuanBao, danBiChongZhiAwardDetail.MaxYuanBao);
					string text = null;
					if (dictionary.TryGetValue(key, out text))
					{
						string[] array = text.Split(new char[]
						{
							'_'
						});
						if (array.Length != 2)
						{
							result = false;
						}
						else
						{
							int num2 = Convert.ToInt32(array[0]);
							int num3 = Convert.ToInt32(array[1]);
							result = (num3 < num && num2 > num3);
						}
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public DanBiChongZhiAwardDetail GetDanBiChongZhiAwardDetail(GameClient client, int danBiID)
		{
			DanBiChongZhiAwardDetail result = null;
			lock (this.DanBiChongZhiAwardDic)
			{
				this.DanBiChongZhiAwardDic.TryGetValue(danBiID, out result);
			}
			return result;
		}

		public override int GetParamsValidateCode()
		{
			int result;
			if (this.DanBiChongZhiAwardDic.Count > 9)
			{
				this.CodeForParamsValidate = -50003;
				LogManager.WriteLog(2, string.Format("活动【{0}】的参数验证失败，错误码{1}", Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), this.CodeForParamsValidate), null, true);
				result = this.CodeForParamsValidate;
			}
			else
			{
				foreach (KeyValuePair<int, DanBiChongZhiAwardDetail> keyValuePair in this.DanBiChongZhiAwardDic)
				{
					if (keyValuePair.Value.SinglePurchase > 127 || keyValuePair.Value.ID < 1 || keyValuePair.Value.ID > 9)
					{
						this.CodeForParamsValidate = -50003;
						LogManager.WriteLog(2, string.Format("活动【{0}】的参数验证失败，错误码{1}", Activity.GetActivityChineseName((ActivityTypes)this.ActivityType), this.CodeForParamsValidate), null, true);
						return this.CodeForParamsValidate;
					}
				}
				result = base.GetParamsValidateCode();
			}
			return result;
		}

		public override bool GiveAward(GameClient client, int danBiID)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = null;
			lock (this.DanBiChongZhiAwardDic)
			{
				if (!this.DanBiChongZhiAwardDic.TryGetValue(danBiID, out danBiChongZhiAwardDetail))
				{
					return false;
				}
			}
			bool result;
			if (null == danBiChongZhiAwardDetail.AwardDict)
			{
				result = false;
			}
			else
			{
				base.GiveAward(client, danBiChongZhiAwardDetail.AwardDict);
				if (null == danBiChongZhiAwardDetail.AwardDict2)
				{
					result = false;
				}
				else
				{
					this.GiveAwardByOccupation(client, danBiChongZhiAwardDetail.AwardDict2, client.ClientData.Occupation);
					if (null == danBiChongZhiAwardDetail.EffectTimeAwardDict)
					{
						result = false;
					}
					else
					{
						base.GiveEffectiveTimeAward(client, danBiChongZhiAwardDetail.EffectTimeAwardDict.ToAwardItem());
						result = true;
					}
				}
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
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(386, new object[0]), new object[]
					{
						myAwardItem.AwardYuanBao
					}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyErr, 0);
					GameManager.DBCmdMgr.AddDBCmd(10113, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, myAwardItem.AwardYuanBao, string.Format("领取{0}活动奖励", (ActivityTypes)this.ActivityType)), null, client.ServerId);
				}
				result = true;
			}
			return result;
		}

		public bool init()
		{
			this.DanBiChongZhiAwardDic.Clear();
			try
			{
				string text = "Config/JieRiGifts/JieRiDanBiChongZhi.xml";
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(text));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(text));
				if (null == xelement)
				{
					return false;
				}
				XElement xelement2 = xelement.Element("Activities");
				if (null != xelement2)
				{
					this.FromDate = Global.GetSafeAttributeStr(xelement2, "FromDate");
					this.ToDate = Global.GetSafeAttributeStr(xelement2, "ToDate");
					this.AwardStartDate = Global.GetSafeAttributeStr(xelement2, "AwardStartDate");
					this.AwardEndDate = Global.GetSafeAttributeStr(xelement2, "AwardEndDate");
					this.ActivityType = (int)Global.GetSafeAttributeLong(xelement2, "ActivityType");
				}
				xelement2 = xelement.Element("GiftList");
				if (null != xelement2)
				{
					IEnumerable<XElement> enumerable = xelement2.Elements();
					foreach (XElement xelement3 in enumerable)
					{
						if (null != xelement3)
						{
							DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = new DanBiChongZhiAwardDetail();
							AwardItem awardItem = new AwardItem();
							AwardItem awardItem2 = new AwardItem();
							AwardEffectTimeItem awardEffectTimeItem = new AwardEffectTimeItem();
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsOne");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取单笔充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取单笔充值活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "单笔充值活动配置");
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement3, "GoodsTwo");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("读取单笔充值活动配置文件中的物品配置项1失败", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("读取单笔充值活动配置文件中的物品配置项失败", new object[0]), null, true);
								}
								else
								{
									awardItem2.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(array, "单笔充值活动配置");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement3, "GoodsThr");
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement3, "EffectiveTime");
							awardEffectTimeItem.Init(safeAttributeStr2, safeAttributeStr3, text + " 时效性物品");
							int minYuanBao = (int)Global.GetSafeAttributeLong(xelement3, "MinYuanBao");
							int maxYuanBao = (int)Global.GetSafeAttributeLong(xelement3, "MaxYuanBao");
							int singlePurchase = (int)Global.GetSafeAttributeLong(xelement3, "SinglePurchase");
							int num = (int)Global.GetSafeAttributeLong(xelement3, "ID");
							danBiChongZhiAwardDetail.ID = num;
							danBiChongZhiAwardDetail.AwardDict = awardItem;
							danBiChongZhiAwardDetail.AwardDict2 = awardItem2;
							danBiChongZhiAwardDetail.EffectTimeAwardDict = awardEffectTimeItem;
							danBiChongZhiAwardDetail.MinYuanBao = minYuanBao;
							danBiChongZhiAwardDetail.MaxYuanBao = maxYuanBao;
							danBiChongZhiAwardDetail.SinglePurchase = singlePurchase;
							this.DanBiChongZhiAwardDic[num] = danBiChongZhiAwardDetail;
						}
					}
				}
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "Config/JieRiGifts/JieRiDanBiChongZhi.xml解析出现异常", ex, true);
				return false;
			}
			return true;
		}

		public override bool HasEnoughBagSpaceForAwardGoods(GameClient client, int danBIID)
		{
			DanBiChongZhiAwardDetail danBiChongZhiAwardDetail = null;
			lock (this.DanBiChongZhiAwardDic)
			{
				if (!this.DanBiChongZhiAwardDic.TryGetValue(danBIID, out danBiChongZhiAwardDetail))
				{
					return false;
				}
			}
			int newGoodsCount = danBiChongZhiAwardDetail.TotalAwardCntWithOcc(client);
			return Global.CanAddGoodsNum(client, newGoodsCount);
		}

		public bool CanGetAnyAward(GameClient client)
		{
			DanBiChongZhiActivity danBiChongZhiActivity = HuodongCachingMgr.GetDanBiChongZhiActivity();
			string cmd = "";
			if (null != danBiChongZhiActivity)
			{
				cmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					danBiChongZhiActivity.FromDate.Replace(':', '$'),
					danBiChongZhiActivity.ToDate.Replace(':', '$'),
					danBiChongZhiActivity.DBQueryInfoCmd()
				});
			}
			Dictionary<string, string> dictionary = Global.sendToDB<Dictionary<string, string>, string>(947, cmd, client.ServerId);
			if (dictionary != null && dictionary.Count<KeyValuePair<string, string>>() > 0)
			{
				lock (this.DanBiChongZhiAwardDic)
				{
					foreach (KeyValuePair<int, DanBiChongZhiAwardDetail> keyValuePair in this.DanBiChongZhiAwardDic)
					{
						string key = string.Format("{0}_{1}", keyValuePair.Value.MinYuanBao, keyValuePair.Value.MaxYuanBao);
						string text = null;
						if (dictionary.TryGetValue(key, out text))
						{
							string[] array = text.Split(new char[]
							{
								'_'
							});
							if (array.Length == 2)
							{
								int num = Convert.ToInt32(array[0]);
								int num2 = Convert.ToInt32(array[1]);
								int singlePurchase = keyValuePair.Value.SinglePurchase;
								if (num > num2 && num2 < singlePurchase)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		public Dictionary<int, DanBiChongZhiAwardDetail> DanBiChongZhiAwardDic = new Dictionary<int, DanBiChongZhiAwardDetail>();
	}
}
