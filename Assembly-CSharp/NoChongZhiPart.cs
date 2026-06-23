using System;
using UnityEngine;

public class NoChongZhiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	private new void OnDestroy()
	{
		Object.Destroy(base.gameObject);
	}
}
