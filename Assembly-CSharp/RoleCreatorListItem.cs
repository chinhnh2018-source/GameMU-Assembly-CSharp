using System;
using UnityEngine;

public class RoleCreatorListItem : UserControl
{
	public int Occupation { get; set; }

	public int Sex { get; set; }

	public void ChangeOccupationText(string text)
	{
		if (null != this.OccupationText)
		{
			this.OccupationText.Text = text;
		}
	}

	public void ChangeSelectedState(string path)
	{
		if (null != this.Image)
		{
			this.Image.URL = path;
		}
	}

	public bool ItemSelected
	{
		get
		{
			return this._ItemSelected;
		}
		set
		{
			this._ItemSelected = value;
			this.setOccFlag();
			if (this._ItemSelected)
			{
				this.ItemBak.spriteName = this.bakNames[1];
			}
			else
			{
				this.ItemBak.spriteName = this.bakNames[0];
			}
			this.setItemText(this._ItemSelected);
		}
	}

	private void setOccFlag()
	{
		this.RolePhotoBorder.spriteName = string.Format("{0}_{1}", this.Occupation, (!this.ItemSelected) ? "normal" : "hover");
		this.mGmOnClickBack.SetActive(this.ItemSelected);
	}

	private void setItemText(bool isSelected)
	{
		if (isSelected)
		{
			this.OccupationText.textColor = 16766048U;
		}
		else
		{
			this.OccupationText.textColor = 7697781U;
		}
	}

	public TextBlock OccupationText;

	public ShowNetImage Image;

	public UISprite ItemBak;

	public UISprite RolePhotoBorder;

	public GameObject mGmOnClickBack;

	private string[] bakNames = new string[]
	{
		"tab_normal2",
		"tab_hover2"
	};

	private bool _ItemSelected;
}
