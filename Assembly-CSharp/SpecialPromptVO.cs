using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SpecialPromptVO
{
	public SpecialPromptVO(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Icon = Global.GetXElementAttributeStr(xml, "Icon");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.Intro = Global.GetXElementAttributeStr(xml, "Intro");
		this.NeedTask = Global.GetXElementAttributeInt(xml, "NeedTask");
		this.ShowLevel = Global.GetXElementAttributeInt(xml, "ShowLevel");
		this.ShowTime = Global.GetXElementAttributeStr(xml, "ShowTime");
		this.mCountDown = Global.GetXElementAttributeStr(xml, "CountDown");
		this.List = Global.GetXElementAttributeInt(xml, "List");
	}

	public List<SpecialPromptVO.TimePoints> MathTimes
	{
		get
		{
			if (0 < this.mMathTimes.Count)
			{
				return this.mMathTimes;
			}
			try
			{
				DateTime correctDateTime = Global.GetCorrectDateTime();
				if (!string.IsNullOrEmpty(this.ShowTime))
				{
					string[] array = this.ShowTime.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						SpecialPromptVO.TimePoints timePoints = new SpecialPromptVO.TimePoints();
						timePoints.Week = array2[0].SafeToInt32(0);
						string[] array3 = array2[1].Split(new char[]
						{
							'-'
						});
						string[] array4 = array3[0].Split(new char[]
						{
							':'
						});
						timePoints.SetTime1(array4[0].SafeToInt32(0), array4[1].SafeToInt32(0), array4[2].SafeToInt32(0));
						string[] array5 = array3[1].Split(new char[]
						{
							':'
						});
						timePoints.SetTime2(array5[0].SafeToInt32(0), array5[1].SafeToInt32(0), array5[2].SafeToInt32(0));
						this.mMathTimes.Add(timePoints);
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
			return this.mMathTimes;
		}
	}

	public bool ThisTimesIsInActivity
	{
		get
		{
			List<SpecialPromptVO.TimePoints> mathTimes = this.MathTimes;
			DateTime correctDateTime = Global.GetCorrectDateTime();
			for (int i = 0; i < mathTimes.Count; i++)
			{
				if (ConfigSpecialPrompt.WeekToIntValue(mathTimes[i].Week) == ConfigSpecialPrompt.WeekToIntValue(correctDateTime.DayOfWeek))
				{
					DateTime dateTime;
					dateTime..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, mathTimes[i].time1[0], mathTimes[i].time1[1], mathTimes[i].time1[2]);
					DateTime dateTime2;
					dateTime2..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day, mathTimes[i].time2[0], mathTimes[i].time2[1], mathTimes[i].time2[2]);
					if (correctDateTime >= dateTime && correctDateTime < dateTime2)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	public SpecialPromptVO.TimePoints CountDownTime
	{
		get
		{
			if (this.mCountDownTime != null)
			{
				return this.mCountDownTime;
			}
			try
			{
				if (!string.IsNullOrEmpty(this.mCountDown))
				{
					string[] array = this.mCountDown.Split(new char[]
					{
						','
					});
					this.mCountDownTime = new SpecialPromptVO.TimePoints();
					this.mCountDownTime.Week = array[0].SafeToInt32(0);
					string[] array2 = array[1].Split(new char[]
					{
						':'
					});
					this.mCountDownTime.SetTime1(array2[0].SafeToInt32(0), array2[1].SafeToInt32(0), array2[2].SafeToInt32(0));
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow>" + ex.Message + "</color>"
				});
			}
			return this.mCountDownTime;
		}
	}

	public int ID;

	public string Icon;

	public string Name;

	public string Intro;

	public int NeedTask;

	public int ShowLevel;

	public string ShowTime;

	public string mCountDown;

	public int List;

	public List<SpecialPromptVO.TimePoints> mMathTimes = new List<SpecialPromptVO.TimePoints>();

	private SpecialPromptVO.TimePoints mCountDownTime;

	public class TimePoints
	{
		public void SetTime1(int a, int b, int c)
		{
			this.time1[0] = a;
			this.time1[1] = b;
			this.time1[2] = c;
		}

		public void SetTime2(int a, int b, int c)
		{
			this.time2[0] = a;
			this.time2[1] = b;
			this.time2[2] = c;
		}

		public int Week;

		public int[] time1 = new int[3];

		public int[] time2 = new int[3];
	}
}
