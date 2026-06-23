using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using Server.Tools;

namespace GameServer.Logic
{
	public class DeControl : IManager
	{
		public static DeControl getInstance()
		{
			return DeControl.instance;
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
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					for (int i = 0; i < this.RuntimeData.DeControlItemListArray.Length; i++)
					{
						List<DeControlItem> list = this.RuntimeData.DeControlItemListArray[i];
						if (null != list)
						{
							foreach (DeControlItem deControlItem in list)
							{
								deControlItem.Next = null;
								deControlItem.Head = null;
							}
							this.RuntimeData.DeControlItemListArray[i] = null;
						}
					}
					this.RuntimeData.IsGongNengOpend = false;
					int platformType = GameCoreInterface.getinstance().GetPlatformType();
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("DeControlOpen", '|');
					foreach (string str in paramValueStringListByName)
					{
						List<int> list2 = Global.StringToIntList(str, ',');
						if (list2 != null && list2[0] == platformType && list2[1] > 0)
						{
							this.RuntimeData.IsGongNengOpend = true;
						}
					}
					this.RuntimeData.IsGongNengOpend &= !GameFuncControlManager.IsGameFuncDisabled(14);
					if (this.RuntimeData.IsGongNengOpend)
					{
						text = "Config\\DeControl.xml";
						string uri = Global.GameResPath(text);
						XElement xelement = XElement.Load(uri);
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							int num = (int)Global.GetSafeAttributeLong(xml, "ExtPropID");
							int num2 = (int)Global.GetSafeAttributeLong(xml, "MaxFlood");
							double[] safeAttributeDoubleArray = Global.GetSafeAttributeDoubleArray(xml, "DeControlPercent", -1, ',');
							double[] safeAttributeDoubleArray2 = Global.GetSafeAttributeDoubleArray(xml, "DeControlTime", -1, ',');
							double[] safeAttributeDoubleArray3 = Global.GetSafeAttributeDoubleArray(xml, "DurationTime", -1, ',');
							if (safeAttributeDoubleArray.Length < num2 || safeAttributeDoubleArray2.Length < num2 || safeAttributeDoubleArray3.Length < num2)
							{
								LogManager.WriteLog(1000, string.Format("解析文件{0}的BaoMingTime出错", text), null, true);
							}
							List<DeControlItem> list = this.RuntimeData.DeControlItemListArray[num];
							if (list == null)
							{
								list = (this.RuntimeData.DeControlItemListArray[num] = new List<DeControlItem>());
							}
							DeControlItem head = null;
							DeControlItem deControlItem2 = null;
							for (int i = 0; i < num2; i++)
							{
								DeControlItem deControlItem = new DeControlItem();
								deControlItem.ExtPropIndex = num;
								deControlItem.DeControlPercent = safeAttributeDoubleArray[i];
								deControlItem.DeControlTime = safeAttributeDoubleArray2[i];
								deControlItem.DurationTime = safeAttributeDoubleArray3[i];
								if (deControlItem2 == null)
								{
									head = deControlItem;
								}
								else
								{
									deControlItem2.Next = deControlItem;
								}
								deControlItem.Head = head;
								deControlItem2 = deControlItem;
								list.Add(deControlItem);
							}
						}
					}
					this.OnReload();
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		private void OnReload()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(true))
			{
				this.OnInitGame(client);
			}
		}

		public void OnInitGame(GameClient client)
		{
			for (int i = 0; i < client.ClientData.DeControlItemArray.Length; i++)
			{
				client.ClientData.DeControlItemArray[i] = null;
			}
			foreach (List<DeControlItem> list in this.RuntimeData.DeControlItemListArray)
			{
				if (null != list)
				{
					int extPropIndex = list[0].ExtPropIndex;
					client.ClientData.DeControlItemArray[extPropIndex] = new DeControlItemData
					{
						Item = list[0]
					};
				}
			}
		}

		public double OnControl(GameClient client, int propIndex)
		{
			double result;
			if (null == client)
			{
				result = 1.0;
			}
			else
			{
				DeControlItemData deControlItemData = client.ClientData.DeControlItemArray[propIndex];
				if (null == deControlItemData)
				{
					result = 1.0;
				}
				else
				{
					DeControlItem item = deControlItemData.Item;
					if (null == item)
					{
						client.ClientData.DeControlItemArray[propIndex] = null;
						result = 1.0;
					}
					else
					{
						long num = TimeUtil.NOW();
						if (num > deControlItemData.EndTicks)
						{
							DeControlItem deControlItem = item.Head;
							if (null != deControlItem)
							{
								deControlItemData.Item = deControlItem;
								deControlItemData.EndTicks = num + (long)(deControlItem.DurationTime * 1000.0);
								LogManager.WriteLog(0, string.Format("控制效果递减#{0}#触发", propIndex), null, true);
							}
							result = 1.0;
						}
						else
						{
							double random = Global.GetRandom();
							if (random < item.DeControlPercent)
							{
								LogManager.WriteLog(0, string.Format("控制效果#{0}#未触发,rnd={1},percent={2}", propIndex, random, item.DeControlPercent), null, true);
								result = 0.0;
							}
							else
							{
								DeControlItem deControlItem = item.Next;
								if (null != deControlItem)
								{
									deControlItemData.Item = deControlItem;
									deControlItemData.EndTicks = num + (long)(deControlItem.DurationTime * 1000.0);
								}
								LogManager.WriteLog(0, string.Format("控制效果#{0}#触发,rnd={1},percent={2},endtime={3},DeControlTime={4}", new object[]
								{
									propIndex,
									random,
									item.DeControlPercent,
									TimeUtil.NowDateTime().AddSeconds(deControlItem.DurationTime),
									item.DeControlTime
								}), null, true);
								result = item.DeControlTime;
							}
						}
					}
				}
			}
			return result;
		}

		private DeControlRuntimeData RuntimeData = new DeControlRuntimeData();

		private static DeControl instance = new DeControl();
	}
}
