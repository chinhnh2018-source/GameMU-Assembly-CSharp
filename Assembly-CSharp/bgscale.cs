using System;
using UnityEngine;

public class bgscale : MonoBehaviour
{
	private void Awake()
	{
		if (this.m_fScreenWidth != 0f && this.m_fScreenHeight != 0f)
		{
			this.m_fScaleWidth = (float)Screen.width / this.m_fScreenWidth;
			this.m_fScaleHeight = (float)Screen.height / this.m_fScreenHeight;
		}
	}

	private void OnGUI()
	{
		GUI.matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, new Vector3(this.m_fScaleWidth, this.m_fScaleHeight, 1f));
		GUI.skin.label.fontSize = 40;
		GUI.skin.button.fontSize = 50;
	}

	public float m_fScreenWidth = 1920f;

	public float m_fScreenHeight = 1080f;

	public float m_fScaleWidth;

	public float m_fScaleHeight;
}
