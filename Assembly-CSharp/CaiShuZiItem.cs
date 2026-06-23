using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class CaiShuZiItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.btnNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.dpsNumber(this, new DPSelectedItemEventArgs());
		};
		this.btnCloseNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Number = -1;
			this.dpsCloseNumber(this, new DPSelectedItemEventArgs());
		};
	}

	public int Number
	{
		get
		{
			return this.m_Number;
		}
		set
		{
			this.m_Number = value;
			this.btnNumber.Text = value.ToString();
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
			if (this.m_Key == 0)
			{
				this.labKeyWei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("万位")
				});
			}
			else if (this.m_Key == 1)
			{
				this.labKeyWei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("千位")
				});
			}
			else if (this.m_Key == 2)
			{
				this.labKeyWei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("百位")
				});
			}
			else if (this.m_Key == 3)
			{
				this.labKeyWei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("十位")
				});
			}
			else if (this.m_Key == 4)
			{
				this.labKeyWei.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("个位")
				});
			}
		}
	}

	public int EndNumber
	{
		get
		{
			return this.m_EndNumber;
		}
		set
		{
			this.m_EndNumber = value;
			if (this.m_EndNumber < 0)
			{
				this.btnCloseNumber.gameObject.SetActive(false);
				this.btnNumber.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					"?"
				});
			}
			else
			{
				this.btnCloseNumber.gameObject.SetActive(true);
				this.btnNumber.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					value.ToString()
				});
			}
		}
	}

	public void SetIitemType(CaiShuZiType type)
	{
		if (type == CaiShuZiType.KaiJiang)
		{
			this.btnNumber.gameObject.SetActive(false);
			this.btnCloseNumber.gameObject.SetActive(false);
			this.spNumberBack.gameObject.SetActive(true);
			this.spNumber.gameObject.SetActive(true);
			this.spOnBack.gameObject.SetActive(false);
			this.labKeyWei.gameObject.SetActive(true);
		}
		else if (type == CaiShuZiType.JiaZhu)
		{
			this.btnNumber.gameObject.SetActive(false);
			this.btnCloseNumber.gameObject.SetActive(false);
			this.spNumberBack.gameObject.SetActive(true);
			this.spNumber.gameObject.SetActive(true);
			this.spOnBack.gameObject.SetActive(false);
		}
		else if (type == CaiShuZiType.YiGou)
		{
			this.btnNumber.gameObject.SetActive(false);
			this.btnCloseNumber.gameObject.SetActive(false);
			this.spNumberBack.gameObject.SetActive(true);
			this.spNumber.gameObject.SetActive(true);
			this.spOnBack.gameObject.SetActive(false);
		}
		else if (type == CaiShuZiType.EndNumber)
		{
			this.labKeyWei.gameObject.SetActive(true);
		}
	}

	public int SpNumber
	{
		set
		{
			if (value >= 0 && value <= 9)
			{
				this.spNumber.spriteName = value.ToString();
			}
			else
			{
				this.spNumber.spriteName = "wenhao";
			}
		}
	}

	public bool SpOnBack
	{
		set
		{
			this.spOnBack.gameObject.SetActive(value);
		}
	}

	public GButton btnNumber;

	public GButton btnCloseNumber;

	public UILabel labKeyWei;

	public UISprite spOnBack;

	public UISprite spNumberBack;

	public UISprite spNumber;

	public DPSelectedItemEventHandler dpsNumber;

	public DPSelectedItemEventHandler dpsCloseNumber;

	private int m_Number = -1;

	private int m_Key = -1;

	private int m_EndNumber = -1;
}
