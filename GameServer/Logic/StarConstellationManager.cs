using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class StarConstellationManager
	{
		public void LoadStarConstellationTypeInfo()
		{
			try
			{
				string format = "Config/XingZuo/XingZuoType.xml";
				XElement gameResXml = Global.GetGameResXml(string.Format(format, new object[0]));
				if (null != gameResXml)
				{
					IEnumerable<XElement> enumerable = gameResXml.Elements("XingZuo").Elements<XElement>();
					foreach (XElement xelement in enumerable)
					{
						if (null != xelement)
						{
							StarConstellationTypeInfo starConstellationTypeInfo = new StarConstellationTypeInfo();
							int num = (int)Global.GetSafeAttributeDouble(xelement, "ID");
							starConstellationTypeInfo.TypeID = num;
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement, "KaiQiLevel");
							string[] array = safeAttributeStr.Split(new char[]
							{
								','
							});
							starConstellationTypeInfo.ChangeLifeLimit = Global.SafeConvertToInt32(array[0]);
							starConstellationTypeInfo.LevelLimit = Global.SafeConvertToInt32(array[1]);
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "ShuXiangJiaCheng");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array2 != null)
							{
								starConstellationTypeInfo.Propertyinfo = new PropertyInfo();
								for (int i = 0; i < array2.Length; i++)
								{
									string[] array3 = array2[i].Split(new char[]
									{
										','
									});
									string a = array3[0];
									string text = array3[1];
									string[] array4 = text.Split(new char[]
									{
										'-'
									});
									if (a == "Defense")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID1 = 3;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue1 = Global.SafeConvertToInt32(array4[0]);
										starConstellationTypeInfo.Propertyinfo.PropertyID2 = 4;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMaxValue1 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Mdefense")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID3 = 5;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue2 = Global.SafeConvertToInt32(array4[0]);
										starConstellationTypeInfo.Propertyinfo.PropertyID4 = 6;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMaxValue2 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Attack")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID5 = 7;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue3 = Global.SafeConvertToInt32(array4[0]);
										starConstellationTypeInfo.Propertyinfo.PropertyID6 = 8;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMaxValue3 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Mattack")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID7 = 9;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue4 = Global.SafeConvertToInt32(array4[0]);
										starConstellationTypeInfo.Propertyinfo.PropertyID8 = 10;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMaxValue4 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "HitV")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID9 = 18;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue5 = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "Dodge")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID10 = 19;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue6 = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "MaxLifeV")
									{
										starConstellationTypeInfo.Propertyinfo.PropertyID11 = 13;
										starConstellationTypeInfo.Propertyinfo.AddPropertyMinValue7 = Global.SafeConvertToInt32(array4[0]);
									}
								}
							}
							safeAttributeStr2 = Global.GetSafeAttributeStr(xelement, "JiaChengBiLie");
							array2 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array2 != null)
							{
								starConstellationTypeInfo.AddPropStarSiteLimit = new int[array2.Length];
								starConstellationTypeInfo.AddPropModulus = new int[array2.Length];
								for (int i = 0; i < array2.Length; i++)
								{
									string[] array3 = array2[i].Split(new char[]
									{
										','
									});
									starConstellationTypeInfo.AddPropStarSiteLimit[i] = Global.SafeConvertToInt32(array3[0]);
									starConstellationTypeInfo.AddPropModulus[i] = Global.SafeConvertToInt32(array3[1]);
								}
							}
							this.m_StarConstellationTypeInfo.Add(num, starConstellationTypeInfo);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("load xml file : {0} fail" + ex.ToString(), string.Format("Config/XingZuoType.xml", new object[0])));
			}
		}

		public void LoadStarConstellationDetailInfo()
		{
			for (int i = 0; i < 6; i++)
			{
				if (i != 4)
				{
					XElement xelement = null;
					try
					{
						xelement = Global.GetGameResXml(string.Format("Config/XingZuo/XingZuo_{0}.xml", i));
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/XingZuo/XingZuo_{0}.xml", i)));
					}
					IEnumerable<XElement> enumerable = xelement.Elements("XingZuo");
					Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dictionary = new Dictionary<int, Dictionary<int, StarConstellationDetailInfo>>();
					foreach (XElement xelement2 in enumerable)
					{
						Dictionary<int, StarConstellationDetailInfo> dictionary2 = new Dictionary<int, StarConstellationDetailInfo>();
						int num = (int)Global.GetSafeAttributeDouble(xelement2, "ID");
						IEnumerable<XElement> enumerable2 = xelement2.Elements("XingWei");
						foreach (XElement xml in enumerable2)
						{
							StarConstellationDetailInfo starConstellationDetailInfo = new StarConstellationDetailInfo();
							int num2 = (int)Global.GetSafeAttributeDouble(xml, "ID");
							starConstellationDetailInfo.StarConstellationID = num2;
							string safeAttributeStr = Global.GetSafeAttributeStr(xml, "LevelLimit");
							string[] array = safeAttributeStr.Split(new char[]
							{
								','
							});
							starConstellationDetailInfo.ChangeLifeLimit = Global.SafeConvertToInt32(array[0]);
							starConstellationDetailInfo.LevelLimit = Global.SafeConvertToInt32(array[1]);
							starConstellationDetailInfo.SuccessRate = (int)(Global.GetSafeAttributeDouble(xml, "Succeed") * 10000.0);
							starConstellationDetailInfo.NeedGoodsID = 0;
							starConstellationDetailInfo.NeedGoodsNum = 0;
							starConstellationDetailInfo.NeedJinBi = (int)Global.GetSafeAttributeDouble(xml, "NeedJinBi");
							starConstellationDetailInfo.NeedStarSoul = (int)Global.GetSafeAttributeDouble(xml, "XingHun");
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "ShuXing");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array2 != null)
							{
								starConstellationDetailInfo.Propertyinfo = new PropertyInfo();
								for (int j = 0; j < array2.Length; j++)
								{
									string[] array3 = array2[j].Split(new char[]
									{
										','
									});
									string a = array3[0];
									string text = array3[1];
									string[] array4 = text.Split(new char[]
									{
										'-'
									});
									if (a == "Defense")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID1 = 3;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue1 = Global.SafeConvertToInt32(array4[0]);
										starConstellationDetailInfo.Propertyinfo.PropertyID2 = 4;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMaxValue1 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Mdefense")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID3 = 5;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue2 = Global.SafeConvertToInt32(array4[0]);
										starConstellationDetailInfo.Propertyinfo.PropertyID4 = 6;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMaxValue2 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Attack")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID5 = 7;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue3 = Global.SafeConvertToInt32(array4[0]);
										starConstellationDetailInfo.Propertyinfo.PropertyID6 = 8;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMaxValue3 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Mattack")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID7 = 9;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue4 = Global.SafeConvertToInt32(array4[0]);
										starConstellationDetailInfo.Propertyinfo.PropertyID8 = 10;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMaxValue4 = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "HitV")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID9 = 18;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue5 = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "Dodge")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID10 = 19;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue6 = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "MaxLifeV")
									{
										starConstellationDetailInfo.Propertyinfo.PropertyID11 = 13;
										starConstellationDetailInfo.Propertyinfo.AddPropertyMinValue7 = Global.SafeConvertToInt32(array4[0]);
									}
								}
							}
							dictionary2.Add(num2, starConstellationDetailInfo);
							if (num2 > this.m_MaxStarSlotID)
							{
								this.m_MaxStarSlotID = num2;
							}
						}
						if (num > this.m_MaxStarSiteID)
						{
							this.m_MaxStarSiteID = num;
						}
						dictionary.Add(num, dictionary2);
					}
					this.m_StarConstellationDetailInfo.Add(i, dictionary);
				}
			}
		}

		public int GetExtendPropIndex(int nValue, StarConstellationTypeInfo starInfo)
		{
			if (nValue > 0)
			{
				for (int i = 0; i < starInfo.AddPropStarSiteLimit.Length; i++)
				{
					if (nValue >= starInfo.AddPropStarSiteLimit[i])
					{
						if (nValue == starInfo.AddPropStarSiteLimit[i])
						{
							return starInfo.AddPropModulus[i];
						}
						if (nValue < starInfo.AddPropStarSiteLimit[i + 1])
						{
							return starInfo.AddPropModulus[i];
						}
					}
				}
			}
			return -1;
		}

		public void InitPlayerStarConstellationPorperty(GameClient client)
		{
			if (client.ClientData.RoleStarConstellationInfo != null && client.ClientData.RoleStarConstellationInfo.Count > 0)
			{
				int occupation = client.ClientData.Occupation;
				Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dictionary = null;
				if (this.m_StarConstellationDetailInfo.TryGetValue(occupation, out dictionary) && dictionary != null)
				{
					client.ClientData.RoleStarConstellationProp.ResetStarConstellationProps();
					foreach (KeyValuePair<int, int> keyValuePair in client.ClientData.RoleStarConstellationInfo)
					{
						int key = keyValuePair.Key;
						int value = keyValuePair.Value;
						if (key >= 0 && key <= this.m_MaxStarSiteID && value >= 0 && value <= this.m_MaxStarSlotID)
						{
							Dictionary<int, StarConstellationDetailInfo> dictionary2 = null;
							if (dictionary.TryGetValue(key, out dictionary2) && dictionary2 != null)
							{
								for (int i = 0; i <= value; i++)
								{
									StarConstellationDetailInfo starConstellationDetailInfo = null;
									if (dictionary2.TryGetValue(i, out starConstellationDetailInfo) && starConstellationDetailInfo != null)
									{
										PropertyInfo propertyinfo = starConstellationDetailInfo.Propertyinfo;
										if (propertyinfo == null)
										{
											return;
										}
										this.ActivationStarConstellationProp(client, propertyinfo, 1);
									}
								}
								this.ActivationStarConstellationExtendProp(client, keyValuePair.Key);
							}
						}
					}
				}
			}
		}

		public int ActivationStarConstellation(GameClient client, int nStarSiteID)
		{
			int result;
			if (nStarSiteID < 1 || nStarSiteID > this.m_MaxStarSiteID)
			{
				result = -1;
			}
			else
			{
				if (client.ClientData.RoleStarConstellationInfo == null)
				{
					client.ClientData.RoleStarConstellationInfo = new Dictionary<int, int>();
				}
				int num = 0;
				client.ClientData.RoleStarConstellationInfo.TryGetValue(nStarSiteID, out num);
				if (num >= this.m_MaxStarSlotID)
				{
					result = -1;
				}
				else if (!GlobalNew.IsGongNengOpened(client, 32, false))
				{
					result = -1;
				}
				else
				{
					num++;
					int occupation = client.ClientData.Occupation;
					Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dictionary = null;
					if (!this.m_StarConstellationDetailInfo.TryGetValue(occupation, out dictionary) || dictionary == null)
					{
						result = -2;
					}
					else
					{
						Dictionary<int, StarConstellationDetailInfo> dictionary2 = null;
						if (!dictionary.TryGetValue(nStarSiteID, out dictionary2) || dictionary2 == null)
						{
							result = -2;
						}
						else
						{
							StarConstellationDetailInfo starConstellationDetailInfo = null;
							if (!dictionary2.TryGetValue(num, out starConstellationDetailInfo) || starConstellationDetailInfo == null)
							{
								result = -2;
							}
							else
							{
								int changeLifeLimit = starConstellationDetailInfo.ChangeLifeLimit;
								int levelLimit = starConstellationDetailInfo.LevelLimit;
								int unionLevel = Global.GetUnionLevel(changeLifeLimit, levelLimit, false);
								if (Global.GetUnionLevel(client.ClientData.ChangeLifeCount, client.ClientData.Level, false) < unionLevel)
								{
									result = -3;
								}
								else
								{
									int needGoodsID = starConstellationDetailInfo.NeedGoodsID;
									int needGoodsNum = starConstellationDetailInfo.NeedGoodsNum;
									if (needGoodsID > 0 && needGoodsNum > 0)
									{
										GoodsData goodsByID = Global.GetGoodsByID(client, needGoodsID);
										if (goodsByID == null || goodsByID.GCount < needGoodsNum)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(533, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
											return -5;
										}
									}
									int needStarSoul = starConstellationDetailInfo.NeedStarSoul;
									if (needStarSoul > 0)
									{
										if (needStarSoul > client.ClientData.StarSoul)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(534, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
											return -9;
										}
									}
									int needJinBi = starConstellationDetailInfo.NeedJinBi;
									if (needJinBi > 0)
									{
										if (!Global.SubBindTongQianAndTongQian(client, needJinBi, "激活星座"))
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(535, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
											return -10;
										}
									}
									if (needGoodsID > 0 && needGoodsNum > 0)
									{
										bool flag = false;
										bool flag2 = false;
										if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, needGoodsID, needGoodsNum, false, out flag, out flag2, false))
										{
											return -6;
										}
									}
									if (needStarSoul > 0)
									{
										GameManager.ClientMgr.ModifyStarSoulValue(client, -needStarSoul, "激活星座", true, true);
									}
									int randomNumber = Global.GetRandomNumber(1, 10001);
									if (randomNumber > starConstellationDetailInfo.SuccessRate)
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(536, new object[0]), new object[0]), GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
										result = -100;
									}
									else
									{
										TCPOutPacket tcpOutPacket = null;
										string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, nStarSiteID, num);
										TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10166, strcmd, out tcpOutPacket, client.ServerId);
										if (TCPProcessCmdResults.RESULT_FAILED == tcpprocessCmdResults)
										{
											result = -7;
										}
										else
										{
											Global.PushBackTcpOutPacket(tcpOutPacket);
											PropertyInfo propertyinfo = starConstellationDetailInfo.Propertyinfo;
											if (propertyinfo == null)
											{
												result = -8;
											}
											else
											{
												client.ClientData.RoleStarConstellationInfo[nStarSiteID] = num;
												this.ActivationStarConstellationProp(client, propertyinfo, 1);
												if (0 == num % 12)
												{
													this.ActivationStarConstellationExtendProp(client, nStarSiteID);
													GameManager.StarConstellationMgr.InitPlayerStarConstellationPorperty(client);
												}
												GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ActiveXingZuo));
												ProcessTask.ProcessRoleTaskVal(client, TaskTypes.XingZuoStar, -1);
												GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
												client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
												GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
												result = 1;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public void ActivationStarConstellationExtendProp(GameClient client, int nSiteID)
		{
			if (client != null && client.ClientData.RoleStarConstellationInfo != null)
			{
				int num = 0;
				if (client.ClientData.RoleStarConstellationInfo.TryGetValue(nSiteID, out num))
				{
					StarConstellationTypeInfo starConstellationTypeInfo = null;
					if (this.m_StarConstellationTypeInfo.TryGetValue(nSiteID, out starConstellationTypeInfo) && starConstellationTypeInfo != null)
					{
						int num2 = 1;
						if (num > 0)
						{
							num2 = this.GetExtendPropIndex(num, starConstellationTypeInfo);
						}
						if (num2 > 0)
						{
							PropertyInfo propertyinfo = starConstellationTypeInfo.Propertyinfo;
							if (propertyinfo != null)
							{
								this.ActivationStarConstellationProp(client, propertyinfo, num2);
							}
						}
					}
				}
			}
		}

		public void ActivationStarConstellationAll(GameClient client)
		{
			int occupation = client.ClientData.Occupation;
			Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dictionary = null;
			if (this.m_StarConstellationDetailInfo.TryGetValue(occupation, out dictionary) && dictionary != null)
			{
				if (client.ClientData.RoleStarConstellationInfo == null)
				{
					client.ClientData.RoleStarConstellationInfo = new Dictionary<int, int>();
				}
				foreach (KeyValuePair<int, Dictionary<int, StarConstellationDetailInfo>> keyValuePair in dictionary)
				{
					TCPOutPacket tcpOutPacket = null;
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, keyValuePair.Key, this.m_MaxStarSlotID);
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10166, strcmd, out tcpOutPacket, client.ServerId);
					if (TCPProcessCmdResults.RESULT_FAILED != tcpprocessCmdResults)
					{
						Global.PushBackTcpOutPacket(tcpOutPacket);
					}
					client.ClientData.RoleStarConstellationInfo[keyValuePair.Key] = this.m_MaxStarSlotID;
					StarConstellationDetailInfo starConstellationDetailInfo = null;
					if (keyValuePair.Value.TryGetValue(this.m_MaxStarSlotID, out starConstellationDetailInfo) && null != starConstellationDetailInfo.Propertyinfo)
					{
						this.ActivationStarConstellationProp(client, starConstellationDetailInfo.Propertyinfo, 1);
					}
					this.ActivationStarConstellationExtendProp(client, keyValuePair.Key);
					GameManager.StarConstellationMgr.InitPlayerStarConstellationPorperty(client);
				}
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.ActiveXingZuo));
				ProcessTask.ProcessRoleTaskVal(client, TaskTypes.XingZuoStar, -1);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
				client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
				GameManager.ClientMgr.NotifySelfLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				client.sendCmd<Dictionary<int, int>>(661, client.ClientData.RoleStarConstellationInfo, false);
			}
		}

		public bool IfStarConstellationPerfect(GameClient client)
		{
			int occupation = client.ClientData.Occupation;
			Dictionary<int, Dictionary<int, StarConstellationDetailInfo>> dictionary = null;
			bool result;
			if (!this.m_StarConstellationDetailInfo.TryGetValue(occupation, out dictionary) || dictionary == null)
			{
				result = false;
			}
			else if (client.ClientData.RoleStarConstellationInfo == null || client.ClientData.RoleStarConstellationInfo.Count != dictionary.Count)
			{
				result = false;
			}
			else
			{
				foreach (int num in client.ClientData.RoleStarConstellationInfo.Values)
				{
					if (num < this.m_MaxStarSlotID)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public void ActivationStarConstellationProp(GameClient client, PropertyInfo tmpProp, int nModulus = 1)
		{
			if (tmpProp.PropertyID1 >= 0 && tmpProp.AddPropertyMinValue1 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID1] += (double)(tmpProp.AddPropertyMinValue1 * nModulus);
			}
			if (tmpProp.PropertyID2 >= 0 && tmpProp.AddPropertyMaxValue1 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID2] += (double)(tmpProp.AddPropertyMaxValue1 * nModulus);
			}
			if (tmpProp.PropertyID3 >= 0 && tmpProp.AddPropertyMinValue2 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID3] += (double)(tmpProp.AddPropertyMinValue2 * nModulus);
			}
			if (tmpProp.PropertyID4 >= 0 && tmpProp.AddPropertyMaxValue2 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID4] += (double)(tmpProp.AddPropertyMaxValue2 * nModulus);
			}
			if (tmpProp.PropertyID5 >= 0 && tmpProp.AddPropertyMinValue3 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID5] += (double)(tmpProp.AddPropertyMinValue3 * nModulus);
			}
			if (tmpProp.PropertyID6 >= 0 && tmpProp.AddPropertyMaxValue3 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID6] += (double)(tmpProp.AddPropertyMaxValue3 * nModulus);
			}
			if (tmpProp.PropertyID7 >= 0 && tmpProp.AddPropertyMinValue4 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID7] += (double)(tmpProp.AddPropertyMinValue4 * nModulus);
			}
			if (tmpProp.PropertyID8 >= 0 && tmpProp.AddPropertyMaxValue4 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID8] += (double)(tmpProp.AddPropertyMaxValue4 * nModulus);
			}
			if (tmpProp.PropertyID9 >= 0 && tmpProp.AddPropertyMinValue5 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID9] += (double)(tmpProp.AddPropertyMinValue5 * nModulus);
			}
			if (tmpProp.PropertyID10 >= 0 && tmpProp.AddPropertyMinValue6 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID10] += (double)(tmpProp.AddPropertyMinValue6 * nModulus);
			}
			if (tmpProp.PropertyID11 >= 0 && tmpProp.AddPropertyMinValue7 > 0)
			{
				client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[tmpProp.PropertyID11] += (double)(tmpProp.AddPropertyMinValue7 * nModulus);
			}
		}

		public Dictionary<int, StarConstellationTypeInfo> m_StarConstellationTypeInfo = new Dictionary<int, StarConstellationTypeInfo>();

		public Dictionary<int, Dictionary<int, Dictionary<int, StarConstellationDetailInfo>>> m_StarConstellationDetailInfo = new Dictionary<int, Dictionary<int, Dictionary<int, StarConstellationDetailInfo>>>();

		public int m_MaxStarSiteID = 0;

		public int m_MaxStarSlotID = 0;
	}
}
