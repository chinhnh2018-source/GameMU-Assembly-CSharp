using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class RecallSignInItem : UserControl
{
	public int MinUnionLevel
	{
		get
		{
			return Global.GetUnionLevel(this.minZh, this.minLv);
		}
	}

	public int MaxUnionLevel
	{
		get
		{
			return Global.GetUnionLevel(this.maxZh, this.maxLv);
		}
	}

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
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.xmlID
				});
			};
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
		}
		else if (this._PickupStatusNew == null)
		{
			this.signStatus.spriteName = "weidacheng";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
		}
		else if (this._PickupStatusNew == -1)
		{
			this.signStatus.spriteName = "yilingqu";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
		}
		else if (this._PickupStatusNew == 2)
		{
			this.signStatus.spriteName = "weihuigui";
			this.signStatus.gameObject.SetActive(true);
			this.signBtn.gameObject.SetActive(false);
		}
	}

	public UILabel signDay_TopLayer;

	public UILabel signDay_BottomLayer;

	public UISprite signStatus;

	public ListBox goodList = new ListBox();

	public ObservableCollection observer;

	public GButton signBtn;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int xmlID;

	public int minLv = 1;

	public int minZh = 1;

	public int maxZh = 1;

	public int maxLv = 1;

	public int day = 1;

	private EReturnAwardOperateState _PickupStatusNew;
}
