using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteInviteFriendItem : UserControl
{
	public int ID { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.SetColor(this.LblName, this.normal);
		this.SetColor(this.LblLevel, this.normal);
		this.SetColor(this.LblBattleValue, this.normal);
		this.SetColor(this.LblTeamInfo, this.normal);
	}

	private void InitTextInPrefabs()
	{
		this.LblName.Label.text = Global.GetLang("New Label");
		this.LblLevel.Label.text = Global.GetLang("New Label");
		this.LblBattleValue.Label.text = Global.GetLang(string.Empty);
		this.LblTeamInfo.Label.text = Global.GetLang("sdfsf");
	}

	private void InitEvent()
	{
		UIEventListener.Get(this.SprtSelectedObj).onClick = delegate(GameObject s)
		{
			this.IsClick = !this.IsClick;
		};
	}

	public void InitValue(FriendData data)
	{
		this.ID = data.OtherRoleID;
		this.LblName.Label.text = data.OtherRoleName;
		this.LblLevel.Label.text = string.Concat(new object[]
		{
			data.FriendChangeLifeLev,
			Global.GetLang("转"),
			data.OtherLevel,
			Global.GetLang("级")
		});
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("TeamLevelLimit", ',');
		int num = data.FriendChangeLifeLev * 1000 + data.OtherLevel;
		int num2 = systemParamIntArrayByName[0] * 1000 + systemParamIntArrayByName[1];
		this.canAddTeam = (num >= num2);
		this.LblBattleValue.Label.text = data.FriendCombatForce.ToString();
		this.LblTeamInfo.Label.text = ((data.ZhanDuiID > 0) ? Global.GetLang("已加入") : Global.GetLang("未加入"));
		if (data.ZhanDuiID > 0)
		{
			BoxCollider component = base.gameObject.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		if (!this.canAddTeam)
		{
			this.SetColor(this.LblName, this.gray);
			this.SetColor(this.LblLevel, this.gray);
			this.SetColor(this.LblBattleValue, this.gray);
			this.SetColor(this.LblTeamInfo, this.gray);
		}
		else
		{
			this.SetColor(this.LblName, this.normal);
			this.SetColor(this.LblLevel, this.normal);
			this.SetColor(this.LblBattleValue, this.normal);
			this.SetColor(this.LblTeamInfo, this.normal);
		}
	}

	private void ChangeLabelColor(bool isSelected)
	{
		if (isSelected)
		{
			this.SetColor(this.LblName, this.normal);
			this.SetColor(this.LblLevel, this.normal);
			this.SetColor(this.LblBattleValue, this.normal);
			this.SetColor(this.LblTeamInfo, this.normal);
		}
		else
		{
			this.SetColor(this.LblName, this.gray);
			this.SetColor(this.LblLevel, this.gray);
			this.SetColor(this.LblBattleValue, this.gray);
			this.SetColor(this.LblTeamInfo, this.gray);
		}
	}

	private void SetColor(TextBlock lbl, string _color)
	{
		lbl.textColor = Global.ParseStringColorToUint(_color);
	}

	public bool IsClick
	{
		get
		{
			return this.misClick;
		}
		set
		{
			if (this.canAddTeam)
			{
				this.misClick = value;
				this.SprtSelected.gameObject.SetActive(this.misClick);
			}
			else
			{
				this.misClick = false;
				this.SprtSelected.gameObject.SetActive(this.misClick);
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblName;

	public TextBlock LblLevel;

	public TextBlock LblBattleValue;

	public TextBlock LblTeamInfo;

	public UISprite SprtSelected;

	public GameObject SprtSelectedObj;

	private string SelectColor = "f0f0f0";

	private string NormalColor = "dac7ae";

	private bool canAddTeam;

	private string gray = "#808081";

	private string normal = "#dac7ae";

	private bool misClick;
}
