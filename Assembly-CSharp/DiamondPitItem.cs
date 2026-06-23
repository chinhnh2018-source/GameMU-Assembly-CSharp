using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DiamondPitItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.index
			});
		};
	}

	public bool highlight
	{
		set
		{
			this.selected = value;
			this.SetHighlightState(this.selected);
		}
	}

	public void SetCosts(int num)
	{
		if (null != this.costs)
		{
			this.costs.Text = num.ToString();
		}
	}

	private void SetHighlightState(bool highlight)
	{
		if (null != this.highlightObj)
		{
			this.highlightObj.SetActive(highlight);
		}
	}

	public GameObject highlightObj;

	public TextBlock costs;

	private bool selected;

	public int index;

	public DPSelectedItemEventHandler DPSelectedItem;
}
