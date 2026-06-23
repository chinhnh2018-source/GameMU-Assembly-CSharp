using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SkipPart : UserControl
{
	protected override void InitializeComponent()
	{
		if (!NGUITools.GetActive(base.gameObject))
		{
			NGUITools.SetActive(base.gameObject, true);
		}
		UIEventListener.Get(this.SkipBtn.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0
			});
		};
		this.ResetLayer(base.transform, "DecorationUI");
	}

	public void ResetLayer(Transform trans, string layerName)
	{
		Transform[] componentsInChildren = trans.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer(layerName);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton SkipBtn;
}
