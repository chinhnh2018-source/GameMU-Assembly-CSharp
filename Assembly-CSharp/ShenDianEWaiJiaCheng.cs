using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenDianEWaiJiaCheng : UserControl
{
	private void InitTextInPrefabs()
	{
		for (int i = 0; i < this.wenben1.Length; i++)
		{
			this.wenben1[i].text = string.Empty;
		}
		this.Title.text = string.Empty;
		for (int j = 0; j < this.wenben2.Length; j++)
		{
			this.wenben2[j].text = string.Empty;
		}
		this.Curtitle.text = string.Empty;
		this.Lowtitle.text = string.Empty;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs());
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		};
	}

	public UILabel Title;

	public UILabel Curtitle;

	public UILabel Lowtitle;

	public UILabel[] wenben1;

	public UILabel[] wenben2;

	public GButton Close;

	public DPSelectedItemEventHandler DPSelectedItem;
}
