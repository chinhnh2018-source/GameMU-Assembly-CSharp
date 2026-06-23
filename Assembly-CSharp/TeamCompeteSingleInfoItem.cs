using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class TeamCompeteSingleInfoItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.LblName.Text = Global.GetLang(string.Empty);
		this.LblZhanLi.Text = Global.GetLang("战力：");
		this.DosplayLblZhanLi = 0;
	}

	private void InitEvent()
	{
	}

	public void InitValue(RoleOccuNameZhanLi data)
	{
		this.LblName.Text = data.RoleName;
		this.DosplayLblZhanLi = data.ZhanLi;
		this.img.URL = TeamCompeteDataManager.GetTouXiangPathByOccu(data.Occupation);
	}

	private int DosplayLblZhanLi
	{
		set
		{
			this.LblZhanLi.Text = Global.GetString(new object[]
			{
				"{e3b35c}",
				Global.GetLang("战力："),
				"{ff0000}",
				value
			});
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblName;

	public TextBlock LblZhanLi;

	public ShowNetImage img;
}
