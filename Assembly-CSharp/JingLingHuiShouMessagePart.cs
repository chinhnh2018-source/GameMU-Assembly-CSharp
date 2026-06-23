using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class JingLingHuiShouMessagePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		this.m_TitleLabel.text = Global.GetLang("提示");
		this.m_Btns[0].Label.text = Global.GetLang("确定");
		this.m_Btns[1].Label.text = Global.GetLang("取消");
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		if (null != this.m_Btns[0])
		{
			this.m_Btns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.MessageBtnHander != null)
				{
					this.MessageBtnHander(e, new DPSelectedItemEventArgs
					{
						ID = 0
					});
				}
			};
		}
		if (null != this.m_Btns[1])
		{
			this.m_Btns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.MessageBtnHander != null)
				{
					this.MessageBtnHander(e, new DPSelectedItemEventArgs
					{
						ID = 1
					});
				}
			};
		}
		if (null != this.m_CloseBtn)
		{
			this.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.MessageBtnHander != null)
				{
					this.MessageBtnHander(e, new DPSelectedItemEventArgs
					{
						ID = 2
					});
				}
			};
		}
	}

	public string Contentlabel
	{
		set
		{
			if (value != null)
			{
				this.m_Contentlabel.text = value;
			}
		}
	}

	public string AwardDescribe0
	{
		set
		{
			if (value != null)
			{
				this.m_AwardDescribe[0].text = value;
			}
		}
	}

	public string AwardDescribe1
	{
		set
		{
			if (value != null)
			{
				this.m_AwardDescribe[1].text = value;
			}
		}
	}

	public string AwardDescribe2
	{
		set
		{
			if (value != null)
			{
				this.m_AwardDescribe[2].text = value;
			}
		}
	}

	public int Money0
	{
		set
		{
			if (0 <= value)
			{
				this.m_Money[0].text = value.ToString();
			}
		}
	}

	public int Money1
	{
		set
		{
			if (0 <= value)
			{
				this.m_Money[1].text = value.ToString();
			}
		}
	}

	public int Money2
	{
		set
		{
			if (0 <= value)
			{
				this.m_Money[2].text = value.ToString();
			}
		}
	}

	public string BtnSureStr
	{
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				this.m_Btns[0].Label.text = value;
			}
		}
	}

	public string BtnCancleStr
	{
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				this.m_Btns[1].Label.text = value;
			}
		}
	}

	public GButton m_CloseBtn;

	public GButton[] m_Btns;

	public UILabel m_TitleLabel;

	public UILabel m_Contentlabel;

	public UILabel[] m_AwardDescribe;

	public UILabel[] m_Money;

	public DPSelectedItemBoolEventHandler MessageBtnHander;
}
