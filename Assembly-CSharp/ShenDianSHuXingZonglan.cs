using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenDianSHuXingZonglan : UserControl
{
	private void InitTextInPrefabs()
	{
		for (int i = 0; i < this.Shuzhi.Length; i++)
		{
			this.Shuzhi[i].text = string.Empty;
		}
		this.Title1.text = string.Empty;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			for (int i = 0; i < this.Shuzhi.Length; i++)
			{
				this.Shuzhi[i].transform.gameObject.SetActive(false);
			}
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

	public UILabel[] Shuzhi;

	public GButton Close;

	public UILabel Title1;

	public DPSelectedItemEventHandler DPSelectedItem;
}
