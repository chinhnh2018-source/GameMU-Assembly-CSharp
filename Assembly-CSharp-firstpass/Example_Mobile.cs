using System;
using UnityEngine;

public class Example_Mobile : MonoBehaviour
{
	private void ResetAllEffects()
	{
		foreach (XffectComponent xffectComponent in this.Effects)
		{
			xffectComponent.Active();
		}
	}

	private void Start()
	{
		this.ResetAllEffects();
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f, 80f, 120f, 20f), "Reset"))
		{
			this.ResetAllEffects();
		}
	}

	public XffectComponent[] Effects;
}
