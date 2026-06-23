using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Logic.Damon;
using GameServer.Logic.Goods;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class JingLingQiYuanManager : IManager
	{
		public static JingLingQiYuanManager getInstance()
		{
			return JingLingQiYuanManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
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

		public bool InitConfig()
		{
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.PetGroupPropertyList.Clear();
					text = "Config/PetGroupProperty.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						PetGroupPropertyItem petGroupPropertyItem = new PetGroupPropertyItem();
						petGroupPropertyItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						petGroupPropertyItem.Name = Global.GetSafeAttributeStr(xml, "Name");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "PetGoods");
						petGroupPropertyItem.PetGoodsList = ConfigParser.ParserIntArrayList(safeAttributeStr, true, '|', ',');
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "GroupProperty");
						petGroupPropertyItem.PropItem = ConfigParser.ParseEquipPropItem(safeAttributeStr2, true, '|', ',', '-');
						this.RuntimeData.PetGroupPropertyList.Add(petGroupPropertyItem);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.PetLevelAwardList.Clear();
					text = "Config/PetLevelAward.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						PetLevelAwardItem petLevelAwardItem = new PetLevelAwardItem();
						petLevelAwardItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						petLevelAwardItem.Level = (int)Global.GetSafeAttributeLong(xml, "Level");
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "ShuXing");
						petLevelAwardItem.PropItem = ConfigParser.ParseEquipPropItem(safeAttributeStr3, true, '|', ',', '-');
						this.RuntimeData.PetLevelAwardList.Add(petLevelAwardItem);
					}
					this.RuntimeData.PetLevelAwardList.Sort((PetLevelAwardItem x, PetLevelAwardItem y) => x.Level - y.Level);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.PetSkillLevelAwardList.Clear();
					text = "Config/PetSkillLevelAward.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						PetSkillLevelAwardItem petSkillLevelAwardItem = new PetSkillLevelAwardItem();
						petSkillLevelAwardItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						petSkillLevelAwardItem.Level = (int)Global.GetSafeAttributeLong(xml, "Level");
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "ShuXing");
						petSkillLevelAwardItem.PropItem = ConfigParser.ParseEquipPropItem(safeAttributeStr3, true, '|', ',', '-');
						this.RuntimeData.PetSkillLevelAwardList.Add(petSkillLevelAwardItem);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.PetTianFuAwardList.Clear();
					text = "Config/PetTianFuAward.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						PetTianFuAwardItem petTianFuAwardItem = new PetTianFuAwardItem();
						petTianFuAwardItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						petTianFuAwardItem.TianFuNum = (int)Global.GetSafeAttributeLong(xml, "TianFuNum");
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "ShuXing");
						petTianFuAwardItem.PropItem = ConfigParser.ParseEquipPropItem(safeAttributeStr3, true, '|', ',', '-');
						this.RuntimeData.PetTianFuAwardList.Add(petTianFuAwardItem);
					}
					this.RuntimeData.PetTianFuAwardList.Sort((PetTianFuAwardItem x, PetTianFuAwardItem y) => x.TianFuNum - y.TianFuNum);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
			}
			try
			{
				this.RuntimeData.PetSkillAwardList.Clear();
				text = "Config/PetSkillGroupProperty.xml";
				string uri = Global.GameResPath(text);
				XElement xelement = XElement.Load(uri);
				IEnumerable<XElement> enumerable2 = xelement.Elements();
				foreach (XElement xelement2 in enumerable2)
				{
					if (xelement2 != null)
					{
						PetSkillGroupInfo petSkillGroupInfo = new PetSkillGroupInfo();
						petSkillGroupInfo.GroupID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
						petSkillGroupInfo.SkillList = new List<int>();
						string defAttributeStr = Global.GetDefAttributeStr(xelement2, "SkillList", "");
						if (!string.IsNullOrEmpty(defAttributeStr))
						{
							string[] array = defAttributeStr.Split(new char[]
							{
								'|'
							});
							foreach (string s in array)
							{
								petSkillGroupInfo.SkillList.Add(int.Parse(s));
							}
						}
						petSkillGroupInfo.SkillNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "SkillNum", "0"));
						string defAttributeStr2 = Global.GetDefAttributeStr(xelement2, "Property", "0");
						petSkillGroupInfo.GroupProp = this.GetGroupProp(defAttributeStr2);
						this.RuntimeData.PetSkillAwardList.Add(petSkillGroupInfo);
					}
				}
				this.RuntimeData.PetSkillAwardList.Sort((PetSkillGroupInfo x, PetSkillGroupInfo y) => x.GroupID - y.GroupID);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载[{0}]时出错!!!", text), null, true);
			}
			return true;
		}

		private EquipPropItem GetGroupProp(string strEffect)
		{
			EquipPropItem result;
			if (string.IsNullOrEmpty(strEffect))
			{
				result = null;
			}
			else
			{
				EquipPropItem equipPropItem = new EquipPropItem();
				string[] array = strEffect.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					int num = (int)Enum.Parse(typeof(ExtPropIndexes), array3[0]);
					double num2 = double.Parse(array3[1]);
					equipPropItem.ExtProps[num] += num2;
				}
				result = equipPropItem;
			}
			return result;
		}

		public void RefreshProps(GameClient client, bool notifyPorpsChangeInfo = true)
		{
			if (!GameFuncControlManager.IsGameFuncDisabled(4))
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				List<PetSkillInfo> list = new List<PetSkillInfo>();
				EquipPropItem equipPropItem = null;
				EquipPropItem equipPropItem2 = null;
				EquipPropItem equipPropItem3 = null;
				EquipPropItem equipPropItem4 = null;
				Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
				Dictionary<int, EquipPropItem> dictionary2 = new Dictionary<int, EquipPropItem>();
				List<GoodsData> demonGoodsDataList = DamonMgr.GetDemonGoodsDataList(client);
				foreach (GoodsData goodsData in demonGoodsDataList)
				{
					GoodsData goodsData2;
					if (!dictionary.TryGetValue(goodsData.GoodsID, out goodsData2))
					{
						goodsData2 = new GoodsData();
						goodsData2.GoodsID = goodsData.GoodsID;
						goodsData2.GCount = 0;
						dictionary[goodsData2.GoodsID] = goodsData2;
					}
					goodsData2.GCount++;
					num += goodsData.Forge_level + 1;
					num3 += Global.GetEquipExcellencePropNum(goodsData);
					list.AddRange(PetSkillManager.GetPetSkillInfo(goodsData));
				}
				foreach (PetSkillInfo petSkillInfo in list)
				{
					num5 += (petSkillInfo.PitIsOpen ? petSkillInfo.Level : 0);
				}
				lock (this.RuntimeData.Mutex)
				{
					foreach (PetLevelAwardItem petLevelAwardItem in this.RuntimeData.PetLevelAwardList)
					{
						if (num >= petLevelAwardItem.Level && petLevelAwardItem.Level > num2)
						{
							num2 = petLevelAwardItem.Level;
							equipPropItem = petLevelAwardItem.PropItem;
						}
					}
					foreach (PetTianFuAwardItem petTianFuAwardItem in this.RuntimeData.PetTianFuAwardList)
					{
						if (num3 >= petTianFuAwardItem.TianFuNum && petTianFuAwardItem.TianFuNum > num4)
						{
							num4 = petTianFuAwardItem.TianFuNum;
							equipPropItem2 = petTianFuAwardItem.PropItem;
						}
					}
					foreach (PetGroupPropertyItem petGroupPropertyItem in this.RuntimeData.PetGroupPropertyList)
					{
						dictionary2[petGroupPropertyItem.Id] = null;
						bool flag2 = true;
						foreach (List<int> list2 in petGroupPropertyItem.PetGoodsList)
						{
							GoodsData goodsData2;
							if (!dictionary.TryGetValue(list2[0], out goodsData2) || goodsData2.GCount < list2[1])
							{
								flag2 = false;
								break;
							}
						}
						if (flag2)
						{
							dictionary2[petGroupPropertyItem.Id] = petGroupPropertyItem.PropItem;
						}
					}
					foreach (PetSkillGroupInfo petSkillGroupInfo in this.RuntimeData.PetSkillAwardList)
					{
						int num7 = 0;
						using (List<int>.Enumerator enumerator8 = petSkillGroupInfo.SkillList.GetEnumerator())
						{
							while (enumerator8.MoveNext())
							{
								int p = enumerator8.Current;
								IEnumerable<PetSkillInfo> source = from info in list
								where info.PitIsOpen && info.SkillID > 0 && info.SkillID == p
								select info;
								if (source.Any<PetSkillInfo>())
								{
									num7 += source.Count<PetSkillInfo>();
								}
							}
						}
						if (num7 < petSkillGroupInfo.SkillNum)
						{
							break;
						}
						equipPropItem3 = petSkillGroupInfo.GroupProp;
					}
					foreach (PetSkillLevelAwardItem petSkillLevelAwardItem in this.RuntimeData.PetSkillLevelAwardList)
					{
						if (num5 >= petSkillLevelAwardItem.Level && petSkillLevelAwardItem.Level > num6)
						{
							num6 = petSkillLevelAwardItem.Level;
							equipPropItem4 = petSkillLevelAwardItem.PropItem;
						}
					}
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					0,
					equipPropItem
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					1,
					equipPropItem2
				});
				foreach (KeyValuePair<int, EquipPropItem> keyValuePair in dictionary2)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.JingLingQiYuan,
						2,
						keyValuePair.Key,
						keyValuePair.Value
					});
				}
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					3,
					equipPropItem3
				});
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.JingLingQiYuan,
					4,
					equipPropItem4
				});
				if (notifyPorpsChangeInfo)
				{
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
				}
			}
		}

		private static JingLingQiYuanManager instance = new JingLingQiYuanManager();

		public JingLingQiYuanData RuntimeData = new JingLingQiYuanData();

		private static class SubPropsTypes
		{
			public const int Level = 0;

			public const int TianFuNum = 1;

			public const int PetGroup = 2;

			public const int PetSkill = 3;

			public const int PetSkillLev = 4;
		}
	}
}
