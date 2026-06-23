using System;
using System.Collections;
using UnityEngine;

public class FontControl : TTMonoBehaviour
{
	private void Start()
	{
		if (this._testlabel != null && this._testlabel.Length > 0)
		{
			Font dynamicFont = Resources.Load("Prefabs/Fonts/DroidSansFallback", typeof(Font)) as Font;
			this._testlabel[0].font.dynamicFont = dynamicFont;
		}
		base.StartCoroutine(this.DisableCacheLabel());
	}

	private void Awake()
	{
	}

	private IEnumerator InitFont()
	{
		yield return new WaitForSeconds(0.2f);
		for (int i = 0; i < this._testlabel.Length; i++)
		{
			if (this._testlabel[i].relativePixelWidth == 0)
			{
				Font ft = Resources.Load("Prefabs/Fonts/DroidSansFallback", typeof(Font)) as Font;
				this._testlabel[i].font.dynamicFont = ft;
				break;
			}
		}
		yield break;
	}

	private IEnumerator DisableCacheLabel()
	{
		yield return new WaitForSeconds(0.2f);
		this.cachedLabel.enabled = false;
		yield break;
	}

	public UILabel cachedLabel;

	public UILabel[] _testlabel;

	public static bool _changeFontInit;
}
