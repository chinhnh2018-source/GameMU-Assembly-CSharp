using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ShareQQPart : UserControl
{
	public ShareQQPart()
	{
		string[,] array = new string[3, 2];
		array[0, 0] = Global.GetLang("微信分享");
		array[0, 1] = "1";
		array[1, 0] = Global.GetLang("QQ分享");
		array[1, 1] = "2";
		array[2, 0] = Global.GetLang("空间分享");
		array[2, 1] = "3";
		this.Types = array;
		base..ctor();
	}

	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public ObservableCollection ItemCollectionAward
	{
		get
		{
			return this._ItemCollectionAward;
		}
		set
		{
			this._ItemCollectionAward = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.staticTxt.text = Global.GetLang("每日首次分享奖励 : ");
		this.BtnLingqu.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollectionAward = this.AwardList.ItemsSource;
		this.ItemCollection = this.TypeList.ItemsSource;
		this.BtnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
		};
		this.BtnLingqu.gameObject.SetActive(false);
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -10
				});
			}
		};
	}

	public void InitPartData()
	{
		this.InitTypeList();
		this.InitRewardData();
		GameInstance.Game.GetShareStat();
	}

	private void InitRewardData()
	{
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("ShareAward", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			UIHelper.AddAwardGoods(this.ItemCollectionAward, systemParamByName);
		}
	}

	private void InitTypeList()
	{
		for (int i = 0; i < this.Types.GetLength(0); i++)
		{
			ShareQQItem shareQQItem = U3DUtils.NEW<ShareQQItem>();
			shareQQItem.Name = this.Types[i, 0];
			shareQQItem.ImgUrl = this.Types[i, 1];
			shareQQItem.ItemID = this.Types[i, 1].SafeToInt32(0);
			shareQQItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = shareQQItem.ItemID
				});
			};
			this.ItemCollection.AddNoUpdate(shareQQItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	public void StartSubmit()
	{
		this.ShowModalDialog();
		GameInstance.Game.GetShareReward();
	}

	public void OnGetReward(string status)
	{
		this.CloseModalDialog();
		GameInstance.Game.GetShareStat();
	}

	public void UpdateBtnStatus(string btnStatus)
	{
		this.CloseModalDialog();
		this.BtnLingqu.gameObject.SetActive(true);
		if (btnStatus == "0")
		{
			this.BtnLingqu.isEnabled = false;
		}
		else if (btnStatus == "1")
		{
			this.BtnLingqu.isEnabled = true;
		}
		else if (btnStatus == "2")
		{
			this.BtnLingqu.isEnabled = false;
			this.BtnLingqu.Label.text = Global.GetLang("已领取");
		}
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox TypeList;

	public ListBox AwardList;

	public GButton BtnLingqu;

	public GButton BtnClose;

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollectionAward;

	public TextBlock staticTxt;

	private string[,] Types;
}
