using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class WolfSoulField_Attack : UserControl
{
	private int nextCityLevel
	{
		get
		{
			if (this.CityLevel >= 10)
			{
				return 10;
			}
			return this.CityLevel + 1;
		}
	}

	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("圣域进攻报名")
		});
		this.FightGoal.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("战斗目标 ：")
		});
		this.FightTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("战斗时间 ：")
		});
		this.ApplyTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("报名时间 ：")
		});
		this.ApplyExpend.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("报名消耗 ：")
		});
		this.Attack.Label.text = Global.GetLang("报名进攻");
		this.chengong.text = Global.GetLang("报名成功");
		this.FightGoalLabel.transform.localPosition = new Vector3(-40f, 74f, -1f);
		this.FightTimeLabel.transform.localPosition = new Vector3(-40f, 31f, -1f);
		this.ApplyTimeLabel.transform.localPosition = new Vector3(-40f, -36f, -1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetJoinState();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		};
		this.Attack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction) && Global.IsBaoMingTime(this.nextCityLevel))
			{
				if (this.CityLevel >= 10)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您已占领最高城池!"), new object[0]), 0, -1, -1, 0);
					return;
				}
				if (Global.zhanmengZiJin < (long)Global.GetCityInfo()[this.nextCityLevel].ZhanMengZiJin)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("战盟资金不足!"), new object[0]), 0, -1, -1, 0);
					return;
				}
				GameInstance.Game.WolfSoulFieldJoin();
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前不在报名时间内!"), new object[0]), 0, -1, -1, 0);
			}
		};
	}

	private void SetJoinState()
	{
		this.chengong.transform.gameObject.SetActive(false);
		if (WolfSoulField_Part.langhunlingyuRoleData == null || WolfSoulField_Part.langhunlingyuRoleData.SignUpState == 0)
		{
			this.Attack.gameObject.SetActive(true);
			if (Global.IsBaoMingTime(this.nextCityLevel))
			{
				this.Attack.isEnabled = true;
			}
			else
			{
				this.Attack.isEnabled = false;
			}
			this.chengong.transform.gameObject.SetActive(false);
		}
		else
		{
			this.chengong.transform.gameObject.SetActive(true);
			this.Attack.gameObject.SetActive(false);
		}
		this.FightGoalLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"feedc5",
			string.Format(Global.GetLang("{0}级据点"), (WolfSoulField_Part.langhunlingyuRoleData != null) ? (this.GetCityLevel(WolfSoulField_Part.langhunlingyuRoleData.SelfCityList) + 1) : 1)
		});
		this.GetFightTime();
		this.Money.text = ((Global.zhanmengZiJin >= (long)Global.GetCityInfo()[this.nextCityLevel].ZhanMengZiJin) ? Global.GetColorStringForNGUIText(new object[]
		{
			"fef5e5",
			Global.GetCityInfo()[this.nextCityLevel].ZhanMengZiJin
		}) : Global.GetColorStringForNGUIText(new object[]
		{
			"d02929",
			Global.GetCityInfo()[this.nextCityLevel].ZhanMengZiJin
		}));
	}

	private int GetCityLevel(List<LangHunLingYuCityData> SelfCityList)
	{
		this.CityLevel = 0;
		if (SelfCityList == null)
		{
			return this.CityLevel;
		}
		for (int i = 0; i < SelfCityList.Count; i++)
		{
			if (SelfCityList[i].Owner != null && SelfCityList[i].Owner.BHID == Global.Data.roleData.Faction && this.CityLevel < SelfCityList[i].CityLevel)
			{
				this.CityLevel = SelfCityList[i].CityLevel;
			}
		}
		return this.CityLevel;
	}

	private void GetFightTime()
	{
		DateTime dateTime = Global.GetCorrectDateTime();
		DayOfWeek dayOfWeek = dateTime.DayOfWeek;
		int num = 0;
		string[] array = Global.GetCityInfo()[this.nextCityLevel].AttackWeekDay.Split(new char[]
		{
			','
		});
		string[] array2 = Global.GetCityInfo()[this.nextCityLevel].BaoMingIntro.Split(new char[]
		{
			'|'
		});
		int num2 = Global.GetCorrectDateTime().DayOfWeek;
		int num3 = num2;
		MUDebug.Log<string>(new string[]
		{
			"配置表：AttackWeekDay---" + Global.GetCityInfo()[this.nextCityLevel].AttackWeekDay.ToString() + "===BaoMingIntros---" + Global.GetCityInfo()[this.nextCityLevel].BaoMingIntro.ToString()
		});
		string text;
		if (this.nextCityLevel == 10)
		{
			if (Global.IsDayWarTime(this.nextCityLevel) && Convert.ToInt32(dayOfWeek) == Global.GetCityInfo()[this.nextCityLevel].AttackWeekDay.SafeToInt32(0))
			{
				text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			}
			else
			{
				int num4;
				if (Global.GetCityInfo()[this.nextCityLevel].AttackWeekDay.SafeToInt32(0) == 0)
				{
					num4 = 7 - Convert.ToInt32(dayOfWeek);
				}
				else
				{
					num4 = Global.GetCityInfo()[this.nextCityLevel].AttackWeekDay.SafeToInt32(0) - Convert.ToInt32(dayOfWeek);
				}
				dateTime = dateTime.AddDays((double)num4);
				text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			}
		}
		else
		{
			if (array.Length != array2.Length)
			{
				MUDebug.Log<string>(new string[]
				{
					"配置表：AttackWeekDay------BaoMingIntro不匹配"
				});
			}
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (num2 == array[j].SafeToInt32(0))
					{
						num = j;
						break;
					}
				}
				if (num2 == array[num].SafeToInt32(0))
				{
					break;
				}
				num2++;
				dateTime = dateTime.AddDays(1.0);
				if (num2 > 6)
				{
					num2 = 0;
				}
			}
			text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			if (Global.IsDayWarTime(this.nextCityLevel))
			{
				text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
			}
			else if (num2 == num3)
			{
				for (int k = 0; k < 7; k++)
				{
					dateTime = dateTime.AddDays(1.0);
					bool flag = false;
					for (int l = 0; l < array.Length; l++)
					{
						if (Convert.ToInt32(dateTime.DayOfWeek) == array[l].SafeToInt32(0))
						{
							num = l;
							text = string.Format(Global.GetLang("{0}年{1}月{2}日"), dateTime.Year, dateTime.Month, dateTime.Day);
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		this.ApplyTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"d02929",
			array2[num]
		});
		string attackTime = Global.GetCityInfo()[this.nextCityLevel].AttackTime;
		string[] array3 = Global.GetCityInfo()[this.nextCityLevel].BaoMingTime.Split(new char[]
		{
			';'
		});
		DayOfWeek dayOfWeek2 = dateTime.DayOfWeek;
		this.FightTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"feedc5",
			text
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"3eb431",
			string.Format("{0}\r\n{1}", this.GetWeek(Convert.ToInt32(dayOfWeek2)), attackTime)
		});
	}

	private string GetBaoMingTime(string[] baomingTimes, DayOfWeek week, int nextCityLevel)
	{
		string text = string.Empty;
		string[] array;
		if (nextCityLevel != 10)
		{
			array = baomingTimes[Convert.ToInt32(week)].Split(new char[]
			{
				'|'
			});
		}
		else
		{
			array = baomingTimes;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			text += ((nextCityLevel == 10) ? string.Concat(new string[]
			{
				this.GetWeek(Convert.ToInt32(array2[0])),
				array2[1].Split(new char[]
				{
					'-'
				})[0],
				"-",
				array2[1].Split(new char[]
				{
					'-'
				})[1],
				"\r\n"
			}) : string.Concat(new string[]
			{
				Global.GetLang("(每日)"),
				array2[1].Split(new char[]
				{
					'-'
				})[0],
				"-",
				array2[1].Split(new char[]
				{
					'-'
				})[1],
				"\r\n"
			}));
		}
		return text;
	}

	private string GetWeek(int week)
	{
		string empty = string.Empty;
		if (week == 0)
		{
			return Global.GetLang("(周日)");
		}
		if (week == 1)
		{
			return Global.GetLang("(周一)");
		}
		if (week == 2)
		{
			return Global.GetLang("(周二)");
		}
		if (week == 3)
		{
			return Global.GetLang("(周三)");
		}
		if (week == 4)
		{
			return Global.GetLang("(周四)");
		}
		if (week == 5)
		{
			return Global.GetLang("(周五)");
		}
		if (week == 6)
		{
			return Global.GetLang("(周六)");
		}
		return empty;
	}

	public void SetBaoMingState(int error)
	{
		if (error >= 0)
		{
			this.chengong.transform.gameObject.SetActive(true);
			this.Attack.transform.gameObject.SetActive(false);
		}
		else
		{
			this.chengong.transform.gameObject.SetActive(false);
			this.Attack.transform.gameObject.SetActive(true);
			this.Attack.isEnabled = true;
		}
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public GButton Close;

	public GButton Attack;

	public UILabel Title;

	public UILabel FightTime;

	public UILabel FightGoal;

	public UILabel ApplyTime;

	public UILabel ApplyExpend;

	public UILabel Money;

	public UILabel FightGoalLabel;

	public UILabel FightTimeLabel;

	public UILabel ApplyTimeLabel;

	public UILabel chengong;

	private int CityLevel;
}
