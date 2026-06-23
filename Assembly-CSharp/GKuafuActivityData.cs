using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public static class GKuafuActivityData
{
	public static GKuafuActivityData.ItemData InitConfig(XElement args)
	{
		if (args == null)
		{
			return null;
		}
		GKuafuActivityData.ItemData itemData = new GKuafuActivityData.ItemData();
		itemData.configTabConfigLine.InitConfig(args);
		itemData.configItem.InitConfig(itemData.configTabConfigLine);
		if (itemData.configItem.IsHaveApplyTime)
		{
			itemData.applyTime = Convert.ToInt32(itemData.configItem.ApplyTime.Split(new char[]
			{
				','
			})[0]);
			itemData.readyTime = Convert.ToInt32(itemData.configItem.ApplyTime.Split(new char[]
			{
				','
			})[1]);
			string[] array = itemData.configItem.TimePoints.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					',',
					':',
					'-'
				});
				if (!itemData.dayOpenHHMMDics.ContainsKey(Convert.ToInt32(array2[0])))
				{
					List<string> list = new List<string>();
					list.Add(array[i]);
					itemData.dayOpenHHMMDics.Add(Convert.ToInt32(array2[0]), list);
				}
				else
				{
					itemData.dayOpenHHMMDics[Convert.ToInt32(array2[0])].Add(array[i]);
				}
			}
		}
		int nItemIndex = itemData.nItemIndex;
		if (nItemIndex >= GKuafuActivityData.datalst.Count)
		{
			GKuafuActivityData.datalst.Add(itemData);
		}
		else
		{
			GKuafuActivityData.datalst[nItemIndex] = itemData;
		}
		return itemData;
	}

	public static List<GKuafuActivityData.ItemData> datalst = new List<GKuafuActivityData.ItemData>();

	public struct HuoDongDateTime
	{
		public DateTime BaoMing;

		public DateTime ZhunBei;

		public DateTime KaiShi;

		public DateTime JieShu;

		public string DHHMM;
	}

	public class ItemData
	{
		public int ID
		{
			get
			{
				return this.configTabConfigLine.ID;
			}
		}

		public int nItemIndex
		{
			get
			{
				return this.configTabConfigLine.nItemIndex;
			}
		}

		public static int ToInt(string str)
		{
			return Global.SafeConvertToInt32(str);
		}

		public void RefreshNextDate()
		{
			if (!this.configItem.IsHaveApplyTime)
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				DateTime dateTime = default(DateTime);
				DateTime dateTime2 = default(DateTime);
				if (this.configItem.mCrusadeWarXml == null || this.configItem.id != 20009)
				{
					List<string> timePoints = this.configItem.timePoints;
					DateTime dateTime3;
					dateTime3..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, GKuafuActivityData.ItemData.ToInt(timePoints[timePoints.Count - 2]), GKuafuActivityData.ItemData.ToInt(timePoints[timePoints.Count - 1]), 0);
					for (int i = 0; i < timePoints.Count; i += 4)
					{
						dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, GKuafuActivityData.ItemData.ToInt(timePoints[i]), GKuafuActivityData.ItemData.ToInt(timePoints[i + 1]), 0);
						dateTime2..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, GKuafuActivityData.ItemData.ToInt(timePoints[i + 2]), GKuafuActivityData.ItemData.ToInt(timePoints[i + 3]), 0);
						if (correctDateTime < dateTime)
						{
							this.isWaitingState = true;
							this.nextDate = dateTime;
							break;
						}
						if (correctDateTime >= dateTime && correctDateTime <= dateTime2)
						{
							this.isWaitingState = false;
							this.nextDate = dateTime2;
							break;
						}
						if (correctDateTime > dateTime3)
						{
							DateTime dateTime4 = Global.GetCorrectDateTime().AddDays(1.0);
							this.nextDate = new DateTime(dateTime4.Year, dateTime4.Month, dateTime4.Day, GKuafuActivityData.ItemData.ToInt(timePoints[0]), GKuafuActivityData.ItemData.ToInt(timePoints[1]), 0);
							this.isWaitingState = true;
							break;
						}
					}
				}
			}
			else
			{
				DateTime dateTime5;
				dateTime5..ctor(Global.GetCorrectLocalTime() * 10000L);
				this.todayOfWeek = dateTime5.DayOfWeek;
				if (this.todayOfWeek == 0)
				{
					this.todayOfWeek = 7;
				}
				this.HuoDongDataTimeLst.Clear();
				foreach (int num in this.dayOpenHHMMDics.Keys)
				{
					foreach (string text in this.dayOpenHHMMDics[num])
					{
						string[] array = text.Split(new char[]
						{
							',',
							':',
							'-'
						});
						int num2 = Convert.ToInt32(array[0]);
						int num3 = Convert.ToInt32(array[1]);
						int num4 = Convert.ToInt32(array[2]);
						int num5 = Convert.ToInt32(array[3]);
						int num6 = Convert.ToInt32(array[4]);
						int num7 = 0;
						if (num2 == this.todayOfWeek)
						{
							num7 = 0;
						}
						else if (num2 < this.todayOfWeek)
						{
							num7 = 7 + num2 - this.todayOfWeek;
						}
						else if (num2 > this.todayOfWeek)
						{
							num7 = num2 - this.todayOfWeek;
						}
						List<int> list = new List<int>();
						if (num7 == 0)
						{
							list.Add(0);
							list.Add(7);
						}
						else
						{
							list.Add(num7);
						}
						for (int j = 0; j < list.Count; j++)
						{
							DateTime dateTime6 = dateTime5.AddDays((double)list[j]);
							dateTime6..ctor(dateTime6.Year, dateTime6.Month, dateTime6.Day, num3, num4, 0);
							DateTime jieShu;
							jieShu..ctor(dateTime6.Year, dateTime6.Month, dateTime6.Day, num5, num6, 0);
							DateTime zhunBei = dateTime6;
							zhunBei = zhunBei.AddMinutes((double)(-(double)this.readyTime / 60));
							DateTime baoMing = dateTime6;
							baoMing = baoMing.AddMinutes((double)(-(double)(this.applyTime + this.readyTime) / 60));
							GKuafuActivityData.HuoDongDateTime huoDongDateTime;
							huoDongDateTime.BaoMing = baoMing;
							huoDongDateTime.ZhunBei = zhunBei;
							huoDongDateTime.KaiShi = dateTime6;
							huoDongDateTime.JieShu = jieShu;
							huoDongDateTime.DHHMM = text;
							this.HuoDongDataTimeLst.Add(huoDongDateTime);
						}
					}
				}
				bool flag = false;
				foreach (GKuafuActivityData.HuoDongDateTime huoDongDateTime2 in this.HuoDongDataTimeLst)
				{
					if (flag)
					{
						break;
					}
					if ((dateTime5 - huoDongDateTime2.BaoMing).Ticks >= 0L && (dateTime5 - huoDongDateTime2.ZhunBei).Ticks < 0L)
					{
						flag = true;
						this.localState = KuafuActivityLocalState.SignUp;
						this.nextDate = huoDongDateTime2.ZhunBei;
						this.nextDateHuoDongDateTime = huoDongDateTime2;
					}
					else if ((dateTime5 - huoDongDateTime2.ZhunBei).Ticks >= 0L && (dateTime5 - huoDongDateTime2.KaiShi).Ticks < 0L)
					{
						flag = true;
						this.localState = KuafuActivityLocalState.Wait;
						this.nextDate = huoDongDateTime2.KaiShi;
						this.nextDateHuoDongDateTime = huoDongDateTime2;
					}
					else if ((dateTime5 - huoDongDateTime2.KaiShi).Ticks >= 0L && (dateTime5 - huoDongDateTime2.JieShu).Ticks < 0L)
					{
						flag = true;
						this.localState = KuafuActivityLocalState.Start;
						this.nextDate = huoDongDateTime2.JieShu;
						this.nextDateHuoDongDateTime = huoDongDateTime2;
					}
				}
				if (!flag)
				{
					KuafuActivityLocalState kuafuActivityLocalState = KuafuActivityLocalState.SignUp;
					if (this.configTabConfigLine.IsYongZheZhanChang || this.configTabConfigLine.IsKuaFuBOSS || this.configTabConfigLine.IsKuaFuWangZhe || this.configTabConfigLine.IsZhuongShenZhengBa)
					{
						kuafuActivityLocalState = KuafuActivityLocalState.SignUp;
					}
					else
					{
						kuafuActivityLocalState = KuafuActivityLocalState.Start;
					}
					this.localState = KuafuActivityLocalState.None;
					long num8 = 0L;
					foreach (GKuafuActivityData.HuoDongDateTime huoDongDateTime3 in this.HuoDongDataTimeLst)
					{
						long ticks = (huoDongDateTime3.KaiShi - dateTime5).Ticks;
						if (ticks >= 0L)
						{
							num8 = ticks;
							this.nextDateHuoDongDateTime = huoDongDateTime3;
							if (kuafuActivityLocalState == KuafuActivityLocalState.SignUp)
							{
								this.nextDate = huoDongDateTime3.BaoMing;
							}
							else
							{
								this.nextDate = huoDongDateTime3.KaiShi;
							}
							break;
						}
					}
					foreach (GKuafuActivityData.HuoDongDateTime huoDongDateTime4 in this.HuoDongDataTimeLst)
					{
						long ticks2 = (huoDongDateTime4.KaiShi - dateTime5).Ticks;
						if (ticks2 >= 0L)
						{
							if (ticks2 < num8)
							{
								num8 = ticks2;
								this.nextDateHuoDongDateTime = huoDongDateTime4;
								if (kuafuActivityLocalState == KuafuActivityLocalState.SignUp)
								{
									this.nextDate = huoDongDateTime4.BaoMing;
								}
								else
								{
									this.nextDate = huoDongDateTime4.KaiShi;
								}
							}
						}
					}
				}
				if (this.configTabConfigLine.IsYongZheZhanChang)
				{
					KuafuYongZheZhanChangPart.LocalState = this.localState;
				}
				else if (this.configTabConfigLine.IsKuaFuBOSS)
				{
					KuafuBossPart.LocalState = this.localState;
				}
				else if (this.configTabConfigLine.IsKuaFuWangZhe)
				{
					KuaFuWangZhePart.LocalState = this.localState;
				}
			}
		}

		public string GetGroup(string[] str, int nDefaultIndex = 0)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i].StartsWith("|") || str[i].StartsWith(",") || str[i].StartsWith(" ") || str[i].StartsWith(":"))
				{
					str[i] = str[i].Remove(0, 1);
				}
			}
			if (this.nextDateHuoDongDateTime.DHHMM.StartsWith("|") || this.nextDateHuoDongDateTime.DHHMM.StartsWith(",") || this.nextDateHuoDongDateTime.DHHMM.StartsWith(" ") || this.nextDateHuoDongDateTime.DHHMM.StartsWith(":"))
			{
				this.nextDateHuoDongDateTime.DHHMM = this.nextDateHuoDongDateTime.DHHMM.Remove(0, 1);
			}
			int num = str.IndexOf(this.nextDateHuoDongDateTime.DHHMM);
			int num2 = (num < 0) ? nDefaultIndex : num;
			string text = string.Empty;
			if (!str[1].Contains("|"))
			{
				str[1] = "|" + str[1];
				if (str.Length > 2 && !str[2].Contains("|"))
				{
					str[2] = "|" + str[2];
				}
			}
			List<string> list = new List<string>();
			string text2 = string.Empty;
			for (int j = 0; j < str.Length; j++)
			{
				string text3 = string.Empty;
				string text4 = str[j];
				if (j > 0 && text4.Length > 0 && text4.get_Chars(0) != '|')
				{
					text4 = "|" + text4;
				}
				if (j == 0 && text4.Length > 1 && text4.get_Chars(0) != '|')
				{
					text3 = text4.Substring(0, 2);
				}
				else if (j > 0 && text4.Length > 2 && text4.get_Chars(0) == '|')
				{
					text3 = text4.Substring(1, 2);
					if (text3.Equals(text2))
					{
						text4 = text4.Remove(1, 2);
					}
				}
				text2 = text3;
				list.Add(text4);
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (num2 == k)
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						list[k]
					});
				}
				else
				{
					text += Global.GetColorStringForNGUIText(new object[]
					{
						"9d8667",
						list[k]
					});
				}
			}
			text = text.Replace("1,", Global.GetLang("星期一 "));
			text = text.Replace("2,", Global.GetLang("星期二 "));
			text = text.Replace("3,", Global.GetLang("星期三 "));
			text = text.Replace("4,", Global.GetLang("星期四 "));
			text = text.Replace("5,", Global.GetLang("星期五 "));
			text = text.Replace("6,", Global.GetLang("星期六 "));
			text = text.Replace("7,", Global.GetLang("星期日 "));
			return text.Replace("0,", Global.GetLang("星期日 "));
		}

		public KuafuActivityTabConfigLine configTabConfigLine;

		public KuafuActivityItemConfig configItem = new KuafuActivityItemConfig();

		public KuafuActivityLocalState localState;

		public DateTime nextDate;

		public GKuafuActivityData.HuoDongDateTime nextDateHuoDongDateTime;

		public bool isWaitingState;

		public int todayOfWeek = -1;

		public Dictionary<int, List<string>> dayOpenHHMMDics = new Dictionary<int, List<string>>();

		public int nearWeekIndex = -1;

		public int applyTime;

		public int readyTime;

		private List<GKuafuActivityData.HuoDongDateTime> HuoDongDataTimeLst = new List<GKuafuActivityData.HuoDongDateTime>();
	}
}
