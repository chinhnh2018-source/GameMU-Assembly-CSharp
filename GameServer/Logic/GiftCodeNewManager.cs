using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class GiftCodeNewManager : IManager
	{
		public static GiftCodeNewManager getInstance()
		{
			return GiftCodeNewManager.instance;
		}

		public bool initialize()
		{
			return this.initGiftCode();
		}

		public bool startup()
		{
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public static bool IsFuncOpen()
		{
			return true;
		}

		public bool initGiftCode()
		{
			bool result = true;
			string text = "";
			lock (this._lockConfig)
			{
				try
				{
					GiftCodeNewManager._GiftCodeCfgAwards.Clear();
					Dictionary<string, GiftCodeInfo> dictionary = new Dictionary<string, GiftCodeInfo>();
					text = Global.GameResPath("Config/GiftCodeNew.xml");
					XElement xelement = CheckHelper.LoadXml(text, true);
					if (null == xelement)
					{
						return false;
					}
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							GiftCodeInfo giftCodeInfo = new GiftCodeInfo();
							giftCodeInfo.GiftCodeTypeID = Global.GetDefAttributeStr(xelement2, "TypeID", "");
							giftCodeInfo.GiftCodeName = Global.GetDefAttributeStr(xelement2, "TypeName", "");
							giftCodeInfo.Description = Global.GetDefAttributeStr(xelement2, "Description", "");
							giftCodeInfo.ChannelList.Clear();
							string[] array = Global.GetDefAttributeStr(xelement2, "Channel", "").Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array)
							{
								if (!string.IsNullOrEmpty(text2))
								{
									giftCodeInfo.ChannelList.Add(text2);
								}
							}
							giftCodeInfo.PlatformList.Clear();
							string[] array3 = Global.GetDefAttributeStr(xelement2, "Platform", "").Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array3)
							{
								if (!string.IsNullOrEmpty(text2))
								{
									giftCodeInfo.PlatformList.Add(Global.SafeConvertToInt32(text2));
								}
							}
							string defAttributeStr = Global.GetDefAttributeStr(xelement2, "TimeBegin", "");
							string defAttributeStr2 = Global.GetDefAttributeStr(xelement2, "TimeEnd", "");
							if (!string.IsNullOrEmpty(defAttributeStr))
							{
								giftCodeInfo.TimeBegin = DateTime.Parse(defAttributeStr);
							}
							if (!string.IsNullOrEmpty(defAttributeStr2))
							{
								giftCodeInfo.TimeEnd = DateTime.Parse(defAttributeStr2);
							}
							giftCodeInfo.ZoneList.Clear();
							string[] array4 = Global.GetDefAttributeStr(xelement2, "Zone", "").Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array4)
							{
								if (!string.IsNullOrEmpty(text2))
								{
									giftCodeInfo.ZoneList.Add(Global.SafeConvertToInt32(text2));
								}
							}
							giftCodeInfo.UserType = (GiftCodeUserType)Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "UserType", "0"));
							giftCodeInfo.UseCount = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "UseCount", "0"));
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsOne");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array5 = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array5.Length > 0)
								{
									giftCodeInfo.GoodsList = GoodsHelper.ParseGoodsDataList(array5, text);
								}
							}
							safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "GoodsTwo");
							if (!string.IsNullOrEmpty(safeAttributeStr))
							{
								string[] array5 = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array5.Length > 0)
								{
									giftCodeInfo.ProGoodsList = GoodsHelper.ParseGoodsDataList(array5, text);
								}
							}
							dictionary.Add(giftCodeInfo.GiftCodeTypeID, giftCodeInfo);
						}
					}
					GiftCodeNewManager._GiftCodeCfgAwards = dictionary;
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("[GiftCodeNew]加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		private GiftCodeInfo GetGiftCodeInfo(string giftid)
		{
			GiftCodeInfo result;
			lock (this._lockConfig)
			{
				GiftCodeInfo giftCodeInfo = null;
				GiftCodeNewManager._GiftCodeCfgAwards.TryGetValue(giftid, out giftCodeInfo);
				result = giftCodeInfo;
			}
			return result;
		}

		public void ProcessGiftCodeList(string strcmd)
		{
			if (null != strcmd)
			{
				if (!GiftCodeNewManager.IsFuncOpen())
				{
					LogManager.WriteLog(0, string.Format("[GiftCodeNew]礼包码功能未开放，礼包码信息={0}", strcmd), null, true);
				}
				else
				{
					try
					{
						string[] array = strcmd.Split(new char[]
						{
							'#'
						});
						if (array.Length > 0)
						{
							GiftCodeAwardData giftCodeAwardData = new GiftCodeAwardData();
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Split(new char[]
								{
									','
								});
								if (array2.Length != 4)
								{
									LogManager.WriteLog(2, string.Format("[GiftCodeNew]ProcessGiftCodeList[{0}]参数错误。", array[i]), null, true);
								}
								else
								{
									giftCodeAwardData.reset();
									giftCodeAwardData.UserId = array2[0];
									giftCodeAwardData.RoleID = Convert.ToInt32(array2[1]);
									giftCodeAwardData.GiftId = array2[2];
									giftCodeAwardData.CodeNo = array2[3];
									if (giftCodeAwardData.RoleID <= 0)
									{
										LogManager.WriteLog(2, string.Format("[GiftCodeNew]ProcessGiftCodeList[{0}]角色id错误。", giftCodeAwardData.RoleID), null, true);
									}
									else
									{
										this.SendAward(null, giftCodeAwardData);
									}
								}
							}
						}
					}
					catch (Exception e)
					{
						DataHelper.WriteFormatExceptionLog(e, "[GiftCodeNew]ProcessGiftCodeList error", false, false);
					}
				}
			}
		}

		public void ProcessGiftCodeCmd(GameClient client, string userId, int roleId, string giftId, string codeNo)
		{
			try
			{
				this.SendAward(client, new GiftCodeAwardData
				{
					UserId = userId,
					RoleID = roleId,
					GiftId = giftId,
					CodeNo = codeNo
				});
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("GiftCodeNew#ProcessGiftCodeCmd#发放领取礼包码失败#rid={0},codeNo={1}", roleId, codeNo), null, true);
				LogManager.WriteException(ex.ToString());
			}
		}

		private void SendAward(GameClient client, GiftCodeAwardData ItemData)
		{
			if (null != ItemData)
			{
				try
				{
					GiftCodeInfo giftCodeInfo = this.GetGiftCodeInfo(ItemData.GiftId);
					if (null == giftCodeInfo)
					{
						this.AddLogEvent(ItemData, -2);
					}
					else if (null != giftCodeInfo.GoodsList)
					{
						int num = 0;
						List<GoodsData> list = new List<GoodsData>();
						foreach (GoodsData item in giftCodeInfo.GoodsList)
						{
							num++;
							list.Add(item);
							if (num % 5 == 0)
							{
								this.SendMailForGiftCode(list, ItemData, giftCodeInfo.GiftCodeName, giftCodeInfo.Description);
								list.Clear();
							}
						}
						if (list.Count > 0)
						{
							this.SendMailForGiftCode(list, ItemData, giftCodeInfo.GiftCodeName, giftCodeInfo.Description);
							list.Clear();
						}
						if (null != client)
						{
							client.ClientData.AddAwardRecord(RoleAwardMsg.LiPinDuiHuan, giftCodeInfo.GoodsList, false);
							GameManager.ClientMgr.NotifyGetAwardMsg(client, RoleAwardMsg.LiPinDuiHuan, "");
						}
					}
				}
				catch (Exception e)
				{
					this.AddLogEvent(ItemData, -4);
					DataHelper.WriteFormatExceptionLog(e, "[GiftCodeNew]SendAward error", false, false);
				}
			}
		}

		private void SendMailForGiftCode(List<GoodsData> GoodList, GiftCodeAwardData ItemData, string subject, string content)
		{
			if (GoodList != null && null != ItemData)
			{
				subject = (string.IsNullOrEmpty(subject) ? GLang.GetLang(121, new object[0]) : subject);
				content = (string.IsNullOrEmpty(content) ? GLang.GetLang(122, new object[0]) : content);
				content = string.Format(content, ItemData.GiftId, ItemData.CodeNo);
				bool flag = Global.UseMailGivePlayerAward3(ItemData.RoleID, GoodList, subject, content, 0, 0, 0);
				if (flag)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(ItemData.RoleID);
					if (null != gameClient)
					{
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, GLang.GetLang(123, new object[0]));
					}
					this.AddLogEvent(ItemData, 1);
				}
				else
				{
					this.AddLogEvent(ItemData, -3);
				}
			}
		}

		private void AddLogEvent(GiftCodeAwardData ItemData, int result)
		{
			if (null != ItemData)
			{
				EventLogManager.SystemRoleEvents[80].AddImporEvent(new object[]
				{
					GameManager.ServerId,
					ItemData.UserId,
					CacheManager.GetZoneIdByRoleId((long)ItemData.RoleID, GameManager.ServerId),
					ItemData.RoleID,
					ItemData.GiftId,
					ItemData.CodeNo,
					result
				});
			}
		}

		private static GiftCodeNewManager instance = new GiftCodeNewManager();

		private object _lockConfig = new object();

		private static Dictionary<string, GiftCodeInfo> _GiftCodeCfgAwards = new Dictionary<string, GiftCodeInfo>();
	}
}
