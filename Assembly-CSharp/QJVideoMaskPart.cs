using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class QJVideoMaskPart : UserControl
{
	private void OnApplicationPause()
	{
	}

	private void OnApplicationFocus()
	{
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
			QJVideoMaskPart.mCurQJVideoWindowState = QJVideoMaskPart.QJVideoWindowState.Close;
		};
		this.spriteBakHalf.gameObject.SetActive(false);
		this.spriteBakFull.gameObject.SetActive(false);
		this.close.gameObject.SetActive(false);
	}

	public static void ShowQJVedioMask()
	{
		if (QJVideoMaskPart.IsInit)
		{
			return;
		}
		QJVideoMaskPart.IsInit = true;
		Canvas stage = MainGame._current.Stage;
		QJVideoMaskPart component = U3DUtils.NEW<QJVideoMaskPart>();
		stage.Children.Add(component);
	}

	public new virtual void Update()
	{
		if (QJVideoMaskPart.mCurQJVideoWindowState != QJVideoMaskPart.mPreQJVideoWindowState)
		{
			QJVideoMaskPart.mPreQJVideoWindowState = QJVideoMaskPart.mCurQJVideoWindowState;
		}
		if (QJVideoMaskPart.mCurQJVideoWindowState == QJVideoMaskPart.QJVideoWindowState.Half)
		{
			if (this.spriteBakFull)
			{
				this.spriteBakFull.gameObject.SetActive(false);
			}
			if (this.spriteBakHalf)
			{
				this.spriteBakHalf.gameObject.SetActive(true);
			}
			if (this.close)
			{
				this.close.gameObject.SetActive(true);
			}
		}
		else if (QJVideoMaskPart.mCurQJVideoWindowState == QJVideoMaskPart.QJVideoWindowState.Full)
		{
			if (this.spriteBakFull)
			{
				this.spriteBakFull.gameObject.SetActive(true);
			}
			if (this.spriteBakHalf)
			{
				this.spriteBakHalf.gameObject.SetActive(false);
			}
			if (this.close)
			{
				this.close.gameObject.SetActive(true);
			}
		}
		else
		{
			if (this.spriteBakFull)
			{
				this.spriteBakFull.gameObject.SetActive(false);
			}
			if (this.spriteBakHalf)
			{
				this.spriteBakHalf.gameObject.SetActive(false);
			}
			if (this.close)
			{
				this.close.gameObject.SetActive(false);
			}
		}
	}

	public static QJVideoMaskPart.QJVideoWindowState mPreQJVideoWindowState;

	public static QJVideoMaskPart.QJVideoWindowState mCurQJVideoWindowState;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton close;

	public UISprite spriteBakHalf;

	public UISprite spriteBakFull;

	private int initHeight;

	private int initWidth;

	private static bool IsInit;

	public enum QJVideoWindowState
	{
		Close,
		Half,
		Full
	}
}
