using System;
using UnityEngine;

public class MonsterFacePart : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public string VSName
	{
		get
		{
			return this.MonsterName.Text;
		}
		set
		{
			this.MonsterName.Text = value;
			int num = (int)(this.MonsterName.Label.relativeSize.x * this.MonsterName.Label.transform.localScale.x) + 15;
			this.MonsterLevel.transform.localPosition = this.MonsterName.Label.transform.localPosition + new Vector3((float)num, 0f, 0f);
		}
	}

	public int SpriteType
	{
		get
		{
			return this._SpriteType;
		}
		set
		{
			this._SpriteType = value;
		}
	}

	public string VLevel
	{
		set
		{
			this.MonsterLevel.Text = "LV" + value;
		}
	}

	public double LifePercent
	{
		set
		{
			this.HPBar.Percent = value;
		}
	}

	public string LifeText
	{
		set
		{
			this.HPBar.ProgessText = value;
		}
	}

	public string LifeTip
	{
		get
		{
			return this._LifeTip;
		}
		set
		{
			this._LifeTip = value;
		}
	}

	public object ItemObject
	{
		get
		{
			return this._ItemObject;
		}
		set
		{
			this._ItemObject = value;
		}
	}

	public int RoleID;

	public TextBlock MonsterLevel;

	public TextBlock MonsterName;

	public GImgProgressBar HPBar;

	private int _SpriteType;

	private string _LifeTip = string.Empty;

	private object _ItemObject;
}
