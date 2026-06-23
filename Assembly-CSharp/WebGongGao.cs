using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using UnityEngine;

public class WebGongGao : UserControl
{
	public void SetVisiable(bool bVisiable)
	{
		UniWebView component = base.GetComponent<UniWebView>();
		if (bVisiable)
		{
			component.Show(false, 0, 0.4f, null);
		}
		else
		{
			component.Hide(false, 0, 0.4f, null);
		}
	}

	private new void Update()
	{
		if (this.initHeight != Screen.height || this.initWidth != Screen.width)
		{
			this.initHeight = Screen.height;
			this.initWidth = Screen.width;
			UniWebView component = base.GetComponent<UniWebView>();
			component.Reload();
		}
	}

	private void ReShow()
	{
		UniWebView component = base.GetComponent<UniWebView>();
		component.Show(false, 0, 0.4f, null);
	}

	private void OnApplicationPause()
	{
		UniWebView component = base.GetComponent<UniWebView>();
		component.Hide(false, 0, 0.4f, null);
	}

	private void OnApplicationFocus()
	{
		base.Invoke("ReShow", 0.5f);
	}

	private void OnApplicationQuit()
	{
	}

	protected override void InitializeComponent()
	{
		this.initHeight = Screen.height;
		this.initWidth = Screen.width;
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -10
			});
		};
	}

	public void InitGongGaoAddress(string address)
	{
		MUDebug.Log<string>(new string[]
		{
			Global.GetLang("加载GongGaoAddress ") + address
		});
		UniWebView component = base.GetComponent<UniWebView>();
		component.OnLoadComplete += new UniWebView.LoadCompleteDelegate(this.OnLoadComplete);
		component.InsetsForScreenOreitation += new UniWebView.InsetsForScreenOreitationDelegate(this.InsetsForScreenOreitation);
		component.url = address;
		component.Load();
		component.zoomEnable = false;
		component.backButtonEnable = false;
	}

	private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (success)
		{
			webView.Show(false, 0, 0.4f, null);
		}
	}

	private UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
	{
		int mobileScreenSize = QMQJJava.GetMobileScreenSize();
		double num = (double)Screen.height - (double)Screen.width * 0.5625;
		if (num > 0.0)
		{
			if (mobileScreenSize > Screen.width)
			{
				return new UniWebViewEdgeInsets((int)(0.12 * (double)Screen.width * 0.5625) + (int)num / 2, (int)(0.083 * (double)((float)Screen.width)), (int)(0.05 * (double)Screen.width * 0.5625) + (int)num / 2, (int)(0.083 * (double)((float)Screen.width)) + mobileScreenSize - Screen.width);
			}
			return new UniWebViewEdgeInsets((int)(0.12 * (double)Screen.width * 0.5625) + (int)num / 2, (int)(0.083 * (double)((float)Screen.width)), (int)(0.05 * (double)Screen.width * 0.5625) + (int)num / 2, (int)(0.083 * (double)((float)Screen.width)));
		}
		else
		{
			if (mobileScreenSize > Screen.width)
			{
				return new UniWebViewEdgeInsets((int)(0.12 * (double)((float)Screen.height)), (int)(0.083 * (double)((float)Screen.width)), (int)(0.05 * (double)((float)Screen.height)), (int)(0.083 * (double)((float)Screen.width)) + mobileScreenSize - Screen.width);
			}
			return new UniWebViewEdgeInsets((int)(0.12 * (double)((float)Screen.height)), (int)(0.083 * (double)((float)Screen.width)), (int)(0.05 * (double)((float)Screen.height)), (int)(0.083 * (double)((float)Screen.width)));
		}
	}

	public GButton close;

	public Transform title;

	public Transform bak;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite spriteBak;

	private int initHeight;

	private int initWidth;
}
