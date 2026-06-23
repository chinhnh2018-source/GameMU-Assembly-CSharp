using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class FamilyBosslistItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.tetIconUrl.transform.localPosition = new Vector3(32f, -145.5f, -0.1f);
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.labName.text = Global.GetLang(string.Empty);
		this.btnLingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.btnLingqu.isEnabled)
			{
				return;
			}
			GameInstance.Game.GetFamilyBossRewardCmd(this.id);
		};
	}

	private void InitTextInPrefabs()
	{
		this.btnLingqu.Text = Global.GetLang("领  取");
	}

	public string Reward
	{
		get
		{
			return this.reward;
		}
		set
		{
			this.reward = value;
			this.Labtongguan.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.strFontColor,
				string.Format(Global.GetLang("通关奖励:     {0}"), this.Reward)
			});
		}
	}

	public bool FontColor
	{
		get
		{
			return this.fontColor;
		}
		set
		{
			this.fontColor = value;
			if (this.fontColor)
			{
				this.strFontColor = "fac60d";
				this.Labtongguan.text = Global.GetColorStringForNGUIText(new object[]
				{
					this.strFontColor,
					string.Format(Global.GetLang("通关奖励:      {0}"), this.Reward)
				});
			}
			else
			{
				this.strFontColor = "757575";
				this.Labtongguan.text = Global.GetColorStringForNGUIText(new object[]
				{
					this.strFontColor,
					string.Format(Global.GetLang("通关奖励:      {0}"), this.Reward)
				});
			}
		}
	}

	public int Id
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public int UpCopyID
	{
		get
		{
			return this.upCopyID;
		}
		set
		{
			this.upCopyID = value;
		}
	}

	public int DownCopyID
	{
		get
		{
			return this.downCopyID;
		}
		set
		{
			this.downCopyID = value;
		}
	}

	public int MapCode
	{
		get
		{
			return this.mapCode;
		}
		set
		{
			this.mapCode = value;
		}
	}

	public int WeekEnterNumber
	{
		get
		{
			return this.weekEnterNumber;
		}
		set
		{
			this.weekEnterNumber = value;
		}
	}

	public void SetBakUrl(string name)
	{
		this.tetUrl.URL = this.GetImageUrlString(name);
	}

	private string GetImageUrlString(string name)
	{
		return string.Format("NetImages/GameRes/Images/Preview/{0}.jpg", name);
	}

	public int BossID
	{
		get
		{
			return this.bossID;
		}
		set
		{
			this.bossID = value;
		}
	}

	public FamilyBossState FamilybossState
	{
		get
		{
			return this.familybossState;
		}
		set
		{
			this.familybossState = value;
			switch (this.familybossState)
			{
			case FamilyBossState.NotOpen:
				this.tetUrl.ToGrayBitmap = true;
				this.tetIconUrl.ToGrayBitmap = true;
				this.FontColor = false;
				this.btnLingqu.gameObject.SetActive(false);
				this.sprBlackBak.gameObject.SetActive(true);
				this.sprNotOpen.gameObject.SetActive(true);
				this.sprPassed.gameObject.SetActive(false);
				this.labName.gameObject.SetActive(false);
				this.IsEnable = false;
				break;
			case FamilyBossState.Passed:
				this.tetUrl.ToGrayBitmap = true;
				this.tetIconUrl.ToGrayBitmap = true;
				this.FontColor = false;
				this.btnLingqu.gameObject.SetActive(false);
				this.sprBlackBak.gameObject.SetActive(true);
				this.sprNotOpen.gameObject.SetActive(false);
				this.sprPassed.gameObject.SetActive(true);
				this.labName.gameObject.SetActive(true);
				this.IsEnable = true;
				break;
			case FamilyBossState.IsOpen:
				this.tetUrl.ToGrayBitmap = false;
				this.tetIconUrl.ToGrayBitmap = false;
				this.FontColor = true;
				this.btnLingqu.gameObject.SetActive(false);
				this.sprBlackBak.gameObject.SetActive(false);
				this.sprNotOpen.gameObject.SetActive(false);
				this.sprPassed.gameObject.SetActive(false);
				this.labName.gameObject.SetActive(false);
				this.IsEnable = true;
				break;
			}
		}
	}

	public FamilyBossAwardState FamilyBossAwardState
	{
		get
		{
			return this.familyBossAwardState;
		}
		set
		{
			this.familyBossAwardState = value;
			FamilyBossAwardState familyBossAwardState = this.familyBossAwardState;
			if (familyBossAwardState != FamilyBossAwardState.CanGain)
			{
				if (familyBossAwardState == FamilyBossAwardState.Gained)
				{
					this.btnLingqu.gameObject.SetActive(false);
				}
			}
			else
			{
				this.btnLingqu.gameObject.SetActive(true);
			}
		}
	}

	public bool SelectState
	{
		get
		{
			return this.selectState;
		}
		set
		{
			this.selectState = value;
			this.sprSelected.gameObject.SetActive(this.selectState);
		}
	}

	public string StrName
	{
		get
		{
			return this.strName;
		}
		set
		{
			this.strName = value;
			this.labName.text = string.Format(Global.GetLang("击杀者:{0}"), this.strName);
		}
	}

	public bool IsEnable
	{
		get
		{
			return this.isEnable;
		}
		set
		{
			this.isEnable = value;
		}
	}

	public ShowNetImage tetUrl;

	public ShowNetImage tetIconUrl;

	public UITexture tetBak;

	public TextBlock Labtongguan;

	public GButton btnLingqu;

	public UISprite sprSelected;

	public UISprite sprBlackBak;

	public UISprite sprNotOpen;

	public UISprite sprPassed;

	public TextBlock labName;

	private string reward = string.Empty;

	private string strFontColor = "fac60d";

	private bool fontColor = true;

	private int id;

	private int upCopyID;

	private int downCopyID;

	private int mapCode;

	private int weekEnterNumber;

	private int bossID;

	private FamilyBossState familybossState;

	private FamilyBossAwardState familyBossAwardState = FamilyBossAwardState.Gained;

	private bool selectState;

	private string strName = string.Empty;

	private bool isEnable = true;
}
