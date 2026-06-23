using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class MUJieriZengsongZengKingItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.labNeed.gameObject.SetActive(false);
		this.ItemCollection = this.goodList.ItemsSource;
		this.btnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ActivityType == 1)
			{
				GameInstance.Game.GetJieriZengsongKingRewardCmd(this.Id);
			}
			else if (this.ActivityType == 2)
			{
				GameInstance.Game.GetJieriShouliKingRewardCmd(this.Id);
			}
			else if (this.ActivityType == 3)
			{
			}
		};
	}

	private void InitTextInPrefabs()
	{
		this.btnLingqu.Label.text = Global.GetLang("领 取");
		this.labDi.text = Global.GetLang("第");
		this.labMing.text = Global.GetLang("名");
		this.labRank.X = -180.0;
		this.labName.Z = -1.0;
	}

	public bool IsInLingqu
	{
		get
		{
			return this.isInLingqu;
		}
		set
		{
			this.isInLingqu = value;
		}
	}

	public int ActivityType
	{
		get
		{
			return this._ActivityType;
		}
		set
		{
			this._ActivityType = value;
		}
	}

	public JieriAwardGiftGainState AwardGiftGainState
	{
		get
		{
			return this.m_AwardGiftGainState;
		}
		set
		{
			this.m_AwardGiftGainState = value;
			switch (this.m_AwardGiftGainState)
			{
			case JieriAwardGiftGainState.CanGain:
				this.btnLingqu.gameObject.SetActive(true);
				this.btnLingqu.isEnabled = true;
				this.StateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.Gained:
				this.btnLingqu.gameObject.SetActive(false);
				this.StateSpriteGained.gameObject.SetActive(true);
				this.labNeed.gameObject.SetActive(false);
				break;
			case JieriAwardGiftGainState.CanNotGain:
				this.btnLingqu.isEnabled = false;
				this.StateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.OverTime:
				this.btnLingqu.gameObject.SetActive(true);
				this.btnLingqu.isEnabled = false;
				this.StateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.SetActive(true);
				break;
			case JieriAwardGiftGainState.NotNeedGain:
				this.btnLingqu.gameObject.SetActive(false);
				this.StateSpriteGained.gameObject.SetActive(false);
				this.labNeed.gameObject.transform.localPosition = new Vector3(420f, 30f, 0f);
				this.labNeed.gameObject.SetActive(true);
				this.labDi.gameObject.transform.localPosition = new Vector3(-196f, 45f, 0f);
				this.labMing.gameObject.transform.localPosition = new Vector3(-133f, 45f, 0f);
				this.labRank.gameObject.transform.localPosition = new Vector3(-164f, 45f, 0f);
				this.labName.gameObject.transform.localPosition = new Vector3(-165f, 14f, 1f);
				this.goodList.gameObject.transform.localPosition = new Vector3(-48f, 33f, 0f);
				this.labDi.gameObject.transform.localScale = new Vector3(20f, 20f, 1f);
				this.labMing.gameObject.transform.localScale = new Vector3(20f, 20f, 1f);
				this.labRank.gameObject.transform.localScale = new Vector3(20f, 20f, 1f);
				this.labName.gameObject.transform.localScale = new Vector3(16f, 16f, 1f);
				break;
			}
		}
	}

	public int Id
	{
		get
		{
			return this._id;
		}
		set
		{
			this._id = value;
			this.labRank.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffd801",
				this.Id.ToString()
			});
		}
	}

	public int Need
	{
		get
		{
			return this._need;
		}
		set
		{
			this._need = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this.roleName;
		}
		set
		{
			this.roleName = value;
			this.labName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"6d8599",
				this.RoleName
			});
		}
	}

	public void SetLabText(int needNum, int type)
	{
		if (type == 1)
		{
			this.labNeed.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2e1bd",
				string.Format(Global.GetLang("最低赠送{0}个"), needNum)
			});
		}
		else if (type == 2)
		{
			this.labNeed.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2e1bd",
				string.Format(Global.GetLang("最低收取{0}个"), needNum)
			});
		}
		else if (type == 3)
		{
			this.labNeed.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f2e1bd",
				string.Format(Global.GetLang("最低充值{0}钻"), needNum)
			});
		}
	}

	public TextBlock labRank;

	public TextBlock labName;

	public ListBox goodList;

	public GButton btnLingqu;

	public TextBlock labNeed;

	public UISprite StateSpriteGained;

	public TextBlock labDi;

	public TextBlock labMing;

	public ObservableCollection ItemCollection;

	private bool isInLingqu = true;

	private int _ActivityType;

	private JieriAwardGiftGainState m_AwardGiftGainState = JieriAwardGiftGainState.CanNotGain;

	private int _id;

	private int _need;

	private string roleName = string.Empty;
}
