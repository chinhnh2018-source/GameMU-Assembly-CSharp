using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class ZhangHaoJingGao : UserControl
{
	public string HintText
	{
		set
		{
			if (null != this.m_HintText)
			{
				this.m_HintText.text = "       " + value;
			}
		}
	}

	public string HintComfirmIconText
	{
		set
		{
			if (null != this.m_HintComfirmIconText)
			{
				this.m_HintComfirmIconText.text = "{ffff00}" + value;
			}
		}
	}

	public int Operate
	{
		set
		{
			this.operate = value;
		}
	}

	public bool DaoJiShiBl
	{
		set
		{
			this.m_DaoJiShiBl = value;
		}
	}

	public bool FengHaoBl
	{
		set
		{
			this.m_FengHaoBl = value;
		}
	}

	public int Second
	{
		set
		{
			this.second = value;
		}
	}

	public int T
	{
		set
		{
			this.t = value;
		}
	}

	public int MyMessageBoxPartReturn
	{
		get
		{
			return this._MyMessageBoxPartReturn;
		}
	}

	public void ShowInfo()
	{
		if (this.m_DaoJiShiBl)
		{
			this.m_DaoJiShi.text = StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
			{
				this.t
			});
			this.m_DaoJiShi.gameObject.SetActive(true);
			this.m_ConfirmIcon.gameObject.SetActive(false);
			this.m_HintTimeText.text = string.Empty;
		}
		else
		{
			this.m_DaoJiShi.gameObject.SetActive(false);
			this.m_ConfirmIcon.gameObject.SetActive(true);
		}
		this.m_HintTimeText.gameObject.SetActive(this.m_FengHaoBl);
		if (this.m_FengHaoBl)
		{
			this.m_DaoJiShi.gameObject.SetActive(false);
			this.m_ConfirmIcon.gameObject.SetActive(true);
			int num = this.second / 60 / 60;
			int num2 = this.second / 60 - num * 60;
			int num3 = this.second - num * 60 * 60 - num2 * 60;
			string text;
			if (num >= 1)
			{
				text = StringUtil.substitute(Global.GetLang("{0}时"), new object[]
				{
					num
				});
				if (num2 <= 9 & num2 != 0)
				{
					text = text + "0" + StringUtil.substitute(Global.GetLang("{0}分"), new object[]
					{
						num2
					});
				}
				else
				{
					text += StringUtil.substitute(Global.GetLang("{0}分"), new object[]
					{
						num2
					});
				}
				if (num3 <= 9 && num3 != 0)
				{
					text = text + "0" + StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
					{
						num3
					});
				}
				else
				{
					text += StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
					{
						num3
					});
				}
			}
			else if (num2 >= 1)
			{
				text = StringUtil.substitute(Global.GetLang("{0}分"), new object[]
				{
					num2
				});
				if (num3 <= 9 && num3 != 0)
				{
					text = text + "0" + StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
					{
						num3
					});
				}
				else
				{
					text += StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
					{
						num3
					});
				}
			}
			else
			{
				text = StringUtil.substitute(Global.GetLang("{0}秒"), new object[]
				{
					num3
				});
			}
			this.m_HintTimeText.text = "{17e43f}" + Global.GetLang("解除封号剩余时间:") + text;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_HintTitle.text = "{ffff00}" + Global.GetLang("提  示");
		this.ShowInfo();
		this._Timer = new DispatcherTimer("RelifePart_Timer");
		this._Timer.Interval = TimeSpan.FromMilliseconds(1000.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.RelifePartTimer_Tick);
		this._Timer.Start();
		this.m_ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ConfirmIcon_MouseLeftButtonUp1);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void ConfirmIcon_MouseLeftButtonUp1(object sender, MouseEvent e)
	{
		this.StopHeart();
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = 1;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		Object.Destroy(base.gameObject);
	}

	private void ConfirmIcon_MouseLeftButtonUp2(object sender, MouseEvent e)
	{
		this.StopHeart();
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = 1;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		if (!this.m_FengHaoBl)
		{
			Global.MainCamera.backgroundColor = Color.black;
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 101
			});
		}
		Object.Destroy(base.gameObject);
	}

	public void RelifePartTimer_Tick(object sender, EventArgs e)
	{
		if (this.m_FengHaoBl)
		{
			this.second--;
			if (this.second == 0)
			{
				this.StopHeart();
				this.m_FengHaoBl = false;
				Object.Destroy(base.gameObject);
			}
		}
		if (this.m_DaoJiShiBl)
		{
			this.t--;
			if (this.t <= 0)
			{
				this.StopHeart();
				this.m_DaoJiShiBl = false;
				this.m_ConfirmIcon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.ConfirmIcon_MouseLeftButtonUp2);
				this.m_DaoJiShi.gameObject.SetActive(false);
				this.m_ConfirmIcon.gameObject.SetActive(true);
			}
		}
		this.ShowInfo();
	}

	public void StopHeart()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
	}

	public GButton m_ConfirmIcon;

	public UILabel m_HintTitle;

	public UILabel m_DaoJiShi;

	public UILabel m_HintText;

	public UILabel m_HintTimeText;

	public UILabel m_HintComfirmIconText;

	private int operate;

	private bool m_DaoJiShiBl;

	private int t;

	private bool m_FengHaoBl;

	private int second;

	public EventHandler ButtonClick;

	public DPSelectedItemEventHandler DPSelectedItem;

	private DispatcherTimer _Timer;

	private int _MyMessageBoxPartReturn = -1;
}
