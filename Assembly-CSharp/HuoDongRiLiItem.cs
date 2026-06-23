using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class HuoDongRiLiItem : UserControl
{
	public EventCalendar Data
	{
		get
		{
			return this.m_Data;
		}
		set
		{
			this.m_Data = value;
			this.Time = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("活动时间：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				this.m_Data.BeginTime + "~" + this.m_Data.EndTime
			}));
			this.Name = this.m_Data.EventName;
			this.Key = this.m_Data.Key;
			this.ID = this.m_Data.ID;
			this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.Key,
					Index = this.m_Data.LinkID
				});
			};
		}
	}

	public string Time
	{
		get
		{
			return this.m_Time.text;
		}
		set
		{
			this.m_Time.text = Global.GetLang(value);
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

	public new string Name
	{
		get
		{
			return this.m_TitleName.text;
		}
		set
		{
			this.m_TitleName.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("活动名称：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				this.m_Data.EventName
			}));
		}
	}

	public int Key
	{
		get
		{
			return this.m_Key;
		}
		set
		{
			this.m_Key = value;
			this.m_Data.Key = this.m_Key;
			if (this.m_Key == -1)
			{
				return;
			}
			if (this.m_Key == 0)
			{
				this.m_Btn.Text = Global.GetLang("前往");
				this.m_Btn.normalSprite = "Btn_on";
				this.m_Btn.hoverSprite = "Btn_off";
				this.m_Btn.pressedSprite = "Btn_off";
				this.m_BtnSprite.spriteName = "Btn_on";
				MUDebug.Log<string>(new string[]
				{
					"前往"
				});
			}
			else if (this.m_Key == 1)
			{
				this.m_Btn.Text = Global.GetLang("未开启");
				this.m_Btn.normalSprite = "Btn_End";
				this.m_Btn.hoverSprite = "Btn_End";
				this.m_Btn.pressedSprite = "Btn_End";
				this.m_BtnSprite.spriteName = "Btn_End";
			}
			else if (this.m_Key == 2)
			{
				NGUITools.SetActive(this.m_ZhiHuibak, true);
				this.m_TitleName.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("活动名称：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					this.m_Data.EventName
				}));
				this.Time = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("活动时间：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					this.m_Data.BeginTime + "~" + this.m_Data.EndTime
				}));
				this.m_Btn.Text = Global.GetLang("已结束");
				this.m_Btn.normalSprite = "Btn_End";
				this.m_Btn.hoverSprite = "Btn_End";
				this.m_Btn.pressedSprite = "Btn_End";
				this.m_BtnSprite.spriteName = "Btn_End";
			}
		}
	}

	public UILabel m_Time;

	public UILabel m_TitleName;

	public ListBox m_List;

	public GButton m_Btn;

	public UITexture m_ZhiHuibak;

	public UISprite m_BtnSprite;

	public DPSelectedItemEventHandler DPSelectedItem;

	private EventCalendar m_Data = new EventCalendar();

	private int m_Key = -1;

	private int m_ID = -1;
}
