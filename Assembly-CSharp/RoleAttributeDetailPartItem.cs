using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class RoleAttributeDetailPartItem : UserControl
{
	public bool ToggleState
	{
		get
		{
			return this._ToggleState;
		}
		set
		{
			if (this._ToggleState != value)
			{
				this._ToggleState = value;
				this._ButtonTween.Play(true);
				this._Attribute.Pressed = value;
				this._Attribute.Label.color = ((!value) ? NGUIMath.HexToColorEx(15900220U) : Color.white);
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Attribute.Label.color = NGUIMath.HexToColorEx(15900220U);
		this._Submit.Label.color = NGUIMath.HexToColorEx(15900220U);
		this._Bak.URL = Global.GetGameResImageString("Item_bak280x87.png");
	}

	public GButton _Attribute;

	public GButton _Submit;

	public TextBlock LabelText;

	public ShowNetImage _Bak;

	public UIButtonTween _ButtonTween;

	public TweenScale _TweenScale;

	public int tag;

	private bool _ToggleState;
}
