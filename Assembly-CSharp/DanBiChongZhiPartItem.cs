using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class DanBiChongZhiPartItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_ListOBC = this.m_ListBox.ItemsSource;
	}

	public string Title
	{
		get
		{
			return this.m_StrTitle;
		}
		set
		{
			this.m_StrTitle = value;
			this.m_LabTitle.text = Global.GetLang(string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("活动期间：")
			}), Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				this.m_StrTitle
			})));
		}
	}

	public int Number
	{
		get
		{
			return this.m_IntNum;
		}
		set
		{
			this.m_IntNum = value;
			if (this.m_IntNum <= 0)
			{
				this.m_LabNumber.text = Global.GetLang(string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("剩余次数：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					this.m_IntNum
				})));
				this.m_Btn.isEnabled = false;
			}
			else
			{
				this.m_LabNumber.text = Global.GetLang(string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("剩余次数：")
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_IntNum
				})));
			}
		}
	}

	public void RefreshBtn(int key = 0)
	{
		this.m_key = key;
		if (key == 2)
		{
			UISprite component = this.m_Btn.transform.Find("Background").GetComponent<UISprite>();
			component.spriteName = "tongyongBtn_disable";
			this.m_Btn.normalSprite = "tongyongBtn_disable";
			this.m_Btn.hoverSprite = "tongyongBtn_disable";
			this.m_Btn.pressedSprite = "tongyongBtn_disable";
			this.m_Btn.Text = Global.GetLang("已领取");
		}
		else if (key == 1)
		{
			UISprite component2 = this.m_Btn.transform.Find("Background").GetComponent<UISprite>();
			component2.spriteName = "weidacheng";
			this.m_Btn.normalSprite = "weidacheng";
			this.m_Btn.hoverSprite = "weidacheng";
			this.m_Btn.pressedSprite = "weidacheng";
			this.m_Btn.Text = string.Empty;
		}
		else if (key == 3)
		{
			UISprite component3 = this.m_Btn.transform.Find("Background").GetComponent<UISprite>();
			component3.spriteName = "btn_chongzhi_normal";
			this.m_Btn.normalSprite = "btn_chongzhi_normal";
			this.m_Btn.hoverSprite = "btn_chongzhi_hover";
			this.m_Btn.pressedSprite = "btn_chongzhi_hover";
			this.m_Btn.Text = Global.GetLang("充值");
		}
		else if (key == 4)
		{
			UISprite component4 = this.m_Btn.transform.Find("Background").GetComponent<UISprite>();
			component4.spriteName = "btn_green_normal ";
			this.m_Btn.normalSprite = "btn_green_normal ";
			this.m_Btn.hoverSprite = "btn_green_hover";
			this.m_Btn.pressedSprite = "btn_green_hover";
			this.m_Btn.Text = Global.GetLang("领取");
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
			this.m_Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.m_ID,
					Type = this.m_key
				});
			};
		}
	}

	public UILabel m_LabTitle;

	public UILabel m_LabNumber;

	public GButton m_Btn;

	public ListBox m_ListBox;

	public ObservableCollection m_ListOBC;

	private string m_StrTitle = string.Empty;

	public UISprite m_SpriteYiLingQu;

	private int m_IntNum;

	private int m_ID;

	public int m_KeLingQu = -1;

	public int m_YiLingQu = -1;

	private int m_key;

	public DPSelectedItemEventHandler DPSelectedItem;
}
