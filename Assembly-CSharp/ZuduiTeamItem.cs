using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZuduiTeamItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnJoin.Text = Global.GetLang("加入");
		this.ConstText.text = Global.GetLang("战力需求:");
		this.lblTeamName.transform.localPosition = new Vector3(-290f, 0f, 0f);
		this.lblForceRequire.transform.localPosition = new Vector3(60f, 0f, 0f);
		this.lblMemberCount.transform.localPosition = new Vector3(165f, 0f, 0f);
		this.ConstText.transform.localPosition = new Vector3(-80f, 0f, 0f);
		this.lblMemberCountMax.transform.localPosition = new Vector3(165f, 0f, 0f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int zhanLi = 0;
			if (!Global.CanEnterFuBenByZhanLi(this._FuBenID, out zhanLi))
			{
				PlayZone.GlobalPlayZone.OpenFuBenTiShiPartWindow(1, zhanLi);
				PlayZone.GlobalPlayZone.m_FuBenTiShiPart.dpsHandler = delegate(object s3, DPSelectedItemEventArgs e3)
				{
					if (e3.ID == 1)
					{
						PlayZone.GlobalPlayZone.CloseFuBenTiShiPartWindow();
						ZuduiFubenPart.SendSpriteCopyTeam(TeamCmds.Apply, this.CopyTeamID, 0, 0);
					}
				};
				return;
			}
			ZuduiFubenPart.SendSpriteCopyTeam(TeamCmds.Apply, this.CopyTeamID, 0, 0);
		};
	}

	public string TeamName
	{
		get
		{
			return this.lblTeamName.text;
		}
		set
		{
			this.lblTeamName.text = value;
		}
	}

	public string ForceRequire
	{
		get
		{
			return this.lblForceRequire.text;
		}
		set
		{
			this.lblForceRequire.text = value;
		}
	}

	public string MemberCount
	{
		get
		{
			return this.lblMemberCount.text;
		}
		set
		{
			this.lblMemberCount.text = value;
		}
	}

	public string MemberCountMax
	{
		get
		{
			return this.lblMemberCountMax.text;
		}
		set
		{
			this.lblMemberCountMax.text = value;
		}
	}

	public long CopyTeamID
	{
		get
		{
			return this._copyTeamId;
		}
		set
		{
			this._copyTeamId = value;
		}
	}

	public int FuBenID
	{
		get
		{
			return this._FuBenID;
		}
		set
		{
			this._FuBenID = value;
		}
	}

	public UILabel lblTeamName;

	public UILabel lblForceRequire;

	public UILabel lblMemberCount;

	public UILabel lblMemberCountMax;

	public GButton btnJoin;

	public UILabel ConstText;

	private long _copyTeamId;

	private int _FuBenID;
}
