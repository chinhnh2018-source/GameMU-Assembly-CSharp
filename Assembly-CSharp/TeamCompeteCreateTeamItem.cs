using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class TeamCompeteCreateTeamItem : UserControl
{
	public TianTi5v5ZhanDuiMiniData miniData { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblTeamName.Label.text = Global.GetLang(string.Empty);
		this.LblLeaderName.Label.text = Global.GetLang(string.Empty);
		this.LblDuanWei.Label.text = Global.GetLang(string.Empty);
	}

	private void InitEvent()
	{
	}

	public void InitValue(TianTi5v5ZhanDuiMiniData data)
	{
		this.miniData = data;
		this.LblTeamName.Text = data.DuiZhangName;
		this.LblLeaderName.Text = data.Name;
		this.LblDuanWei.Text = TeamCompeteDataManager.GetDuanWeiNameByID((int)data.DuanWeiID);
	}

	public bool IsSelect
	{
		set
		{
			NGUITools.SetActive(this.mSelect.gameObject, value);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTeamName;

	public TextBlock LblLeaderName;

	public TextBlock LblDuanWei;

	public UISprite mSelect;
}
