using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ZhuTiFuZhiGouItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_Btn.Text = Global.GetLang("购买");
	}

	public int Numbers
	{
		get
		{
			return this.mNumbers;
		}
		set
		{
			this.mNumbers = value;
		}
	}

	public string ChongZhiID { get; set; }

	public int ZhiGouID { get; set; }

	public int Number
	{
		get
		{
			return this.mShenYuNumber;
		}
		set
		{
			this.mShenYuNumber = value;
			this.m_LabNumber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("个人限购：" + value.ToString())
			});
		}
	}

	public int JiaGe
	{
		set
		{
			this.m_LabJiaGe.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang(value + "RMB")
			});
		}
	}

	public int ID
	{
		get
		{
			return this.m_ID;
		}
		set
		{
			this.m_ID = value;
		}
	}

	public int Time
	{
		set
		{
			base.StartCoroutine<bool>(this.StarTime(value));
		}
	}

	protected override void OnDestroy()
	{
		base.StopCoroutine(this.StarTime(1));
		base.OnDestroy();
	}

	private IEnumerator TimCorutine(DateTime datetTime)
	{
		long time = datetTime.Ticks / 10000L - Global.GetCorrectLocalTime();
		for (;;)
		{
			time -= 1000L;
			if (time < 86400000L)
			{
				UILabel labTime = this.m_LabTime;
				object[] array = new object[2];
				array[0] = "17e43e";
				int num = 1;
				DateTime dateTime;
				dateTime..ctor(time * 10000L);
				array[num] = dateTime.ToString(Global.GetLang("剩余时间：HH:mm:ss "));
				labTime.text = Global.GetColorStringForNGUIText(array);
			}
			else
			{
				UILabel labTime2 = this.m_LabTime;
				object[] array2 = new object[2];
				array2[0] = "17e43e";
				int num2 = 1;
				string lang = Global.GetLang("剩余时间：");
				string text = (time / 86400000L).ToString();
				DateTime dateTime2;
				dateTime2..ctor(time * 10000L);
				array2[num2] = lang + text + dateTime2.ToString("HH:mm:ss");
				labTime2.text = Global.GetColorStringForNGUIText(array2);
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private IEnumerator StarTime(int day = 1)
	{
		for (;;)
		{
			int times = day * 24 * 3600 - (Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second);
			int hour = times / 3600;
			int minute = times % 3600 / 60;
			int second = times % 3600 % 60;
			string strMinute = minute.ToString();
			string strsecond = second.ToString();
			if (hour > 0)
			{
				if (minute < 10)
				{
					strMinute = string.Format("0{0}", minute.ToString());
				}
				if (second < 10)
				{
					strsecond = string.Format("0{0}", second.ToString());
				}
				this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format(Global.GetLang("剩余时间：{0}:{1}:{2}"), hour, strMinute, strsecond))
				});
			}
			else
			{
				if (minute < 10)
				{
					strMinute = string.Format("0{0}", minute.ToString());
				}
				if (second < 10)
				{
					strsecond = string.Format("0{0}", second.ToString());
				}
				this.m_LabTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format(Global.GetLang("剩余时间：0:{0}:{1}"), strMinute, strsecond))
				});
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	public ListBox m_ListGoods;

	public UILabel m_LabTime;

	public UILabel m_LabJiaGe;

	public UILabel m_LabNumber;

	public GButton m_Btn;

	public int m_ID = -1;

	private int mShenYuNumber = -1;

	private int mNumbers = -1;
}
