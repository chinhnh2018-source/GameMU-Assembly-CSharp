using System;
using UnityEngine;

public class FriendRecommendItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.lblLevel.transform.localPosition = new Vector3(150f, 0f, 0f);
	}

	public string Occupation
	{
		get
		{
			return this.lblOccu.text;
		}
		set
		{
			this.lblOccu.text = value;
		}
	}

	public string RoleName
	{
		get
		{
			return this.lblName.text;
		}
		set
		{
			this.lblName.text = value;
		}
	}

	public string RoleLevel
	{
		get
		{
			return this.lblLevel.text;
		}
		set
		{
			this.lblLevel.text = value;
		}
	}

	public bool ItemSelected
	{
		set
		{
			this.chkSelected.Check = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._roleID;
		}
		set
		{
			this._roleID = value;
		}
	}

	public bool SelectedState
	{
		get
		{
			return this.chkSelected.Check;
		}
		set
		{
			this.chkSelected.Check = value;
		}
	}

	public UILabel lblOccu;

	public UILabel lblName;

	public UILabel lblLevel;

	public GCheckBox chkSelected;

	private int _roleID;
}
