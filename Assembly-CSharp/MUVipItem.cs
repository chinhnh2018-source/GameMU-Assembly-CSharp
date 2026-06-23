using System;
using UnityEngine;

public class MUVipItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public bool blstat
	{
		set
		{
			this.stat.gameObject.SetActive(!value);
			this.lingquBtn.gameObject.SetActive(value);
			this.image.ToGrayBitmap = !value;
		}
	}

	public string needLev
	{
		set
		{
			this.vipLev.Text = value;
		}
	}

	public int awardID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
			if (value <= 1012)
			{
				value = 1001;
			}
			this.image.URL = string.Format("NetImages/GameRes/Images/Vip/{0}.jpg", value);
		}
	}

	public GButton lingquBtn;

	public TextBlock vipLev;

	public GameObject stat;

	public ShowNetImage image;

	private int id;
}
