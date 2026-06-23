using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class OpenServerActivePartItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(this.ItemHeadBak.gameObject).onClick = delegate(GameObject s)
		{
			if (this.DPSelectedItem != null)
			{
				if (this.CurrentState == OpenServerActivePartItem.IconState.ClickForLook || this.CurrentState == OpenServerActivePartItem.IconState.CanGain)
				{
					this.CurrentState = OpenServerActivePartItem.IconState.ClickForContract;
				}
				else
				{
					this.CurrentState = OpenServerActivePartItem.IconState.ClickForLook;
				}
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					ID = this._ItemIndex
				});
			}
		};
		this.CurrentState = OpenServerActivePartItem.IconState.ClickForLook;
	}

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
				this.ButtonTween.Play(this._ToggleState);
			}
		}
	}

	public int ItemIndex
	{
		get
		{
			return this._ItemIndex;
		}
		set
		{
			this._ItemIndex = value;
		}
	}

	public void SetValueText(string value)
	{
		this.m_TxtValue.text = value;
	}

	public OpenServerActivePartItem.IconState CurrentState
	{
		get
		{
			return this._currentState;
		}
		set
		{
			this._currentState = value;
			GameObject iconCanGain = this.IconCanGain;
			switch (this._currentState)
			{
			case OpenServerActivePartItem.IconState.ClickForLook:
				this.SetValueText(Global.GetLang("点击查看"));
				this.IconCanGain.SetActive(false);
				iconCanGain.GetComponent<Animation>().Stop();
				break;
			case OpenServerActivePartItem.IconState.ClickForContract:
				this.SetValueText(Global.GetLang("点击收起"));
				this.IconCanGain.SetActive(false);
				iconCanGain.GetComponent<Animation>().Stop();
				break;
			case OpenServerActivePartItem.IconState.CanGain:
				this.SetValueText(string.Empty);
				this.IconCanGain.SetActive(true);
				iconCanGain.GetComponent<Animation>().Play();
				break;
			}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GTextBlockOutLine m_TxtValue;

	public ShowNetImage ItemHeadBak;

	public UIButtonTween ButtonTween;

	public GameObject IconCanGain;

	public GameObject IconDouble;

	public TweenScale ItemTweenScale;

	public SpriteSL Content;

	public GameObject ActivityTipIcon;

	private OpenServerActivePartItem.IconState _currentState = OpenServerActivePartItem.IconState.ClickForLook;

	private bool _ToggleState;

	private int _ItemIndex = -1;

	public enum IconState
	{
		ClickForLook = 1,
		ClickForContract,
		CanGain
	}
}
