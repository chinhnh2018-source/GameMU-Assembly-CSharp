using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class RecallLeiChongItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.signBtn.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.goodList)
		{
			this.observer = this.goodList.ItemsSource;
		}
		if (null != this.signBtn)
		{
			this.signBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this._PickupStatusNew == null)
				{
					PlayZone.GlobalPlayZone.ShowChongZhiWindow();
				}
				else
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = this.xmlID
					});
				}
			};
		}
	}

	public int ZuanShiNumber
	{
		set
		{
			this.zuanshiNumber = value;
			this.number.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				Global.GetLang("回归玩家累计充值") + this.zuanshiNumber + Global.GetLang("钻")
			});
		}
	}

	public EReturnAwardOperateState PickupStatusNew
	{
		set
		{
			this._PickupStatusNew = value;
		}
	}

	public void RefreshUI()
	{
		if (this._PickupStatusNew == 1)
		{
			this.signStatus.gameObject.SetActive(false);
			this.signBtn.gameObject.SetActive(true);
			this.signBtn.Text = Global.GetLang("领取");
			this.signBtn.normalSprite = "tongyongBtn4_normal";
			this.signBtn.hoverSprite = "tongyongBtn4_hover";
			this.signBtn.pressedSprite = "tongyongBtn4_hover";
			this.signBtn.disabledSprite = "tongyongBtn4_disable";
			this.number.gameObject.SetActive(true);
		}
		else if (this._PickupStatusNew == null)
		{
			this.signStatus.gameObject.SetActive(false);
			this.signBtn.gameObject.SetActive(true);
			this.signBtn.Text = Global.GetLang("充值");
			this.signBtn.normalSprite = "btnNormal";
			this.signBtn.hoverSprite = "btnHover";
			this.signBtn.pressedSprite = "btnHover";
			this.signBtn.disabledSprite = "tongyongBtn4_disable";
			this.number.gameObject.SetActive(true);
		}
		else if (this._PickupStatusNew == -1)
		{
			this.signStatus.spriteName = "yilingqu";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
			this.number.gameObject.SetActive(true);
			this.number.gameObject.SetActive(false);
		}
		else if (this._PickupStatusNew == 2)
		{
			this.signStatus.spriteName = "weihuigui";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
			this.number.gameObject.SetActive(false);
		}
	}

	public UILabel number;

	public UISprite signStatus;

	public ListBox goodList = new ListBox();

	public ObservableCollection observer;

	public GButton signBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int xmlID;

	public int zuanshiNumber;

	private EReturnAwardOperateState _PickupStatusNew;
}
