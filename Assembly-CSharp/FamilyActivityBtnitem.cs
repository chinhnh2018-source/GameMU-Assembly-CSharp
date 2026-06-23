using System;
using UnityEngine;

public class FamilyActivityBtnitem : UserControl
{
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

	public UISprite Bak;

	public GameObject TipIcon;

	public UILabel label;

	private int id;
}
