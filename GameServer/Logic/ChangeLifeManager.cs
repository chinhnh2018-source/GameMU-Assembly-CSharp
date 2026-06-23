using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Server.Tools;

namespace GameServer.Logic
{
	public class ChangeLifeManager
	{
		public void LoadRoleZhuanShengInfo()
		{
			for (int i = 0; i < 6; i++)
			{
				if (i != 4)
				{
					XElement xelement = null;
					try
					{
						xelement = Global.GetGameResXml(string.Format("Config/Roles/ZhuanSheng_{0}.xml", i));
					}
					catch (Exception ex)
					{
						throw new Exception(string.Format("启动时加载xml文件: {0} 失败", string.Format("Config/Roles/ZhuanSheng_{0}.xml", i)));
					}
					IEnumerable<XElement> enumerable = xelement.Elements("ZhuanShengs").Elements<XElement>();
					Dictionary<int, ChangeLifeDataInfo> dictionary = new Dictionary<int, ChangeLifeDataInfo>();
					foreach (XElement xelement2 in enumerable)
					{
						ChangeLifeDataInfo changeLifeDataInfo = new ChangeLifeDataInfo();
						int num = 0;
						if (null != xelement2)
						{
							num = (int)Global.GetSafeAttributeLong(xelement2, "ChangeLifeID");
							changeLifeDataInfo.ChangeLifeID = (int)Global.GetSafeAttributeLong(xelement2, "ChangeLifeID");
							changeLifeDataInfo.NeedLevel = (int)Global.GetSafeAttributeLong(xelement2, "Level");
							changeLifeDataInfo.NeedMoney = (int)Global.GetSafeAttributeLong(xelement2, "NeedJinBi");
							changeLifeDataInfo.NeedMoJing = (int)Global.GetSafeAttributeLong(xelement2, "NeedMoJing");
							changeLifeDataInfo.ExpProportion = Global.GetSafeAttributeLong(xelement2, "ExpProportion");
							string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "NeedGoods");
							if (string.IsNullOrEmpty(safeAttributeStr))
							{
								LogManager.WriteLog(1, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] array = safeAttributeStr.Split(new char[]
								{
									'|'
								});
								if (array.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									changeLifeDataInfo.NeedGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(safeAttributeStr, "转生文件");
								}
							}
							string safeAttributeStr2 = Global.GetSafeAttributeStr(xelement2, "AwardShuXing");
							string[] array2 = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array2 != null)
							{
								changeLifeDataInfo.Propertyinfo = new ChangeLifePropertyInfo();
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
										changeLifeDataInfo.Propertyinfo.PhyDefenseMin = 3;
										changeLifeDataInfo.Propertyinfo.AddPhyDefenseMinValue = Global.SafeConvertToInt32(array4[0]);
										changeLifeDataInfo.Propertyinfo.PhyDefenseMax = 4;
										changeLifeDataInfo.Propertyinfo.AddPhyDefenseMaxValue = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Mdefense")
									{
										changeLifeDataInfo.Propertyinfo.MagDefenseMin = 5;
										changeLifeDataInfo.Propertyinfo.AddMagDefenseMinValue = Global.SafeConvertToInt32(array4[0]);
										changeLifeDataInfo.Propertyinfo.MagDefenseMax = 6;
										changeLifeDataInfo.Propertyinfo.AddMagDefenseMaxValue = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Attack")
									{
										changeLifeDataInfo.Propertyinfo.PhyAttackMin = 7;
										changeLifeDataInfo.Propertyinfo.AddPhyAttackMinValue = Global.SafeConvertToInt32(array4[0]);
										changeLifeDataInfo.Propertyinfo.PhyAttackMax = 8;
										changeLifeDataInfo.Propertyinfo.AddPhyAttackMaxValue = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "Mattack")
									{
										changeLifeDataInfo.Propertyinfo.MagAttackMin = 9;
										changeLifeDataInfo.Propertyinfo.AddMagAttackMinValue = Global.SafeConvertToInt32(array4[0]);
										changeLifeDataInfo.Propertyinfo.MagAttackMax = 10;
										changeLifeDataInfo.Propertyinfo.AddMagAttackMaxValue = Global.SafeConvertToInt32(array4[1]);
									}
									else if (a == "HitV")
									{
										changeLifeDataInfo.Propertyinfo.HitProp = 18;
										changeLifeDataInfo.Propertyinfo.AddHitPropValue = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "Dodge")
									{
										changeLifeDataInfo.Propertyinfo.DodgeProp = 19;
										changeLifeDataInfo.Propertyinfo.AddDodgePropValue = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "MaxLifeV")
									{
										changeLifeDataInfo.Propertyinfo.MaxLifeProp = 13;
										changeLifeDataInfo.Propertyinfo.AddMaxLifePropValue = Global.SafeConvertToInt32(array4[0]);
									}
									else if (a == "AddAttack")
									{
										changeLifeDataInfo.Propertyinfo.PhyAttackMin = 7;
										changeLifeDataInfo.Propertyinfo.AddPhyAttackMinValue = Global.SafeConvertToInt32(array4[0]);
										changeLifeDataInfo.Propertyinfo.PhyAttackMax = 8;
										changeLifeDataInfo.Propertyinfo.AddPhyAttackMaxValue = Global.SafeConvertToInt32(array4[1]);
										changeLifeDataInfo.Propertyinfo.MagAttackMin = 9;
										changeLifeDataInfo.Propertyinfo.AddMagAttackMinValue = Global.SafeConvertToInt32(array4[0]);
										changeLifeDataInfo.Propertyinfo.MagAttackMax = 10;
										changeLifeDataInfo.Propertyinfo.AddMagAttackMaxValue = Global.SafeConvertToInt32(array4[1]);
									}
								}
							}
							string safeAttributeStr3 = Global.GetSafeAttributeStr(xelement2, "AwardGoods");
							if (string.IsNullOrEmpty(safeAttributeStr3))
							{
								LogManager.WriteLog(1, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
							}
							else
							{
								string[] array5 = safeAttributeStr3.Split(new char[]
								{
									'|'
								});
								if (array5.Length <= 0)
								{
									LogManager.WriteLog(1, string.Format("转生文件NeedGoods为空", new object[0]), null, true);
								}
								else
								{
									changeLifeDataInfo.AwardGoodsDataList = Global.LoadChangeOccupationNeedGoodsInfo(safeAttributeStr3, "转生文件");
								}
							}
						}
						if (num > this.m_MaxChangeLifeCount)
						{
							this.m_MaxChangeLifeCount = num;
						}
						if (num > 1)
						{
							changeLifeDataInfo.Propertyinfo.AddFrom(dictionary[num - 1].Propertyinfo);
						}
						dictionary.Add(num, changeLifeDataInfo);
					}
					this.m_ChangeLifeInfoList.Add(i, dictionary);
				}
			}
		}

		public ChangeLifeDataInfo GetChangeLifeDataInfo(GameClient Client, int nChangeLife = 0)
		{
			if (nChangeLife == 0)
			{
				nChangeLife = Client.ClientData.ChangeLifeCount;
			}
			Dictionary<int, ChangeLifeDataInfo> dictionary = new Dictionary<int, ChangeLifeDataInfo>();
			ChangeLifeDataInfo result;
			if (!GameManager.ChangeLifeMgr.m_ChangeLifeInfoList.TryGetValue(Client.ClientData.Occupation, out dictionary))
			{
				result = null;
			}
			else
			{
				ChangeLifeDataInfo changeLifeDataInfo = new ChangeLifeDataInfo();
				if (!dictionary.TryGetValue(nChangeLife, out changeLifeDataInfo))
				{
					result = null;
				}
				else
				{
					result = changeLifeDataInfo;
				}
			}
			return result;
		}

		public void InitPlayerChangeLifePorperty(GameClient client)
		{
			if (client.ClientData.ChangeLifeCount > 0)
			{
				int occupation = client.ClientData.Occupation;
				Dictionary<int, ChangeLifeDataInfo> dictionary = null;
				if (this.m_ChangeLifeInfoList.TryGetValue(occupation, out dictionary) && dictionary != null)
				{
					ChangeLifeDataInfo changeLifeDataInfo = new ChangeLifeDataInfo();
					if (dictionary.TryGetValue(client.ClientData.ChangeLifeCount, out changeLifeDataInfo) && changeLifeDataInfo != null)
					{
						ChangeLifePropertyInfo propertyinfo = changeLifeDataInfo.Propertyinfo;
						if (propertyinfo != null)
						{
							this.ActivationChangeLifeProp(client, propertyinfo);
						}
					}
				}
			}
		}

		public void ProcessRoleChangeLifeProp(GameClient client)
		{
			this.InitPlayerChangeLifePorperty(client);
			GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
		}

		public void ActivationChangeLifeProp(GameClient client, ChangeLifePropertyInfo tmpProp)
		{
			client.ClientData.RoleChangeLifeProp.ResetChangeLifeProps();
			if (tmpProp.PhyAttackMin >= 0 && tmpProp.AddPhyAttackMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyAttackMin] += (double)tmpProp.AddPhyAttackMinValue;
			}
			if (tmpProp.PhyAttackMax >= 0 && tmpProp.AddPhyAttackMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyAttackMax] += (double)tmpProp.AddPhyAttackMaxValue;
			}
			if (tmpProp.MagAttackMin >= 0 && tmpProp.AddMagAttackMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagAttackMin] += (double)tmpProp.AddMagAttackMinValue;
			}
			if (tmpProp.MagAttackMax >= 0 && tmpProp.AddMagAttackMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagAttackMax] += (double)tmpProp.AddMagAttackMaxValue;
			}
			if (tmpProp.PhyDefenseMin >= 0 && tmpProp.AddPhyDefenseMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyDefenseMin] += (double)tmpProp.AddPhyDefenseMinValue;
			}
			if (tmpProp.PhyDefenseMax >= 0 && tmpProp.AddPhyDefenseMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.PhyDefenseMax] += (double)tmpProp.AddPhyDefenseMaxValue;
			}
			if (tmpProp.MagDefenseMin >= 0 && tmpProp.AddMagDefenseMinValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagDefenseMin] += (double)tmpProp.AddMagDefenseMinValue;
			}
			if (tmpProp.MagDefenseMax >= 0 && tmpProp.AddMagDefenseMaxValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MagDefenseMax] += (double)tmpProp.AddMagDefenseMaxValue;
			}
			if (tmpProp.HitProp >= 0 && tmpProp.AddHitPropValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.HitProp] += (double)tmpProp.AddHitPropValue;
			}
			if (tmpProp.DodgeProp >= 0 && tmpProp.AddDodgePropValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.DodgeProp] += (double)tmpProp.AddDodgePropValue;
			}
			if (tmpProp.MaxLifeProp >= 0 && tmpProp.AddMaxLifePropValue > 0)
			{
				client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[tmpProp.MaxLifeProp] += (double)tmpProp.AddMaxLifePropValue;
			}
		}

		public Dictionary<int, Dictionary<int, ChangeLifeDataInfo>> m_ChangeLifeInfoList = new Dictionary<int, Dictionary<int, ChangeLifeDataInfo>>();

		public int m_MaxChangeLifeCount = 0;
	}
}
