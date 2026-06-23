using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MUQiRiKuangHuanPartGoalItem : UserControl
{
	public int GoalType
	{
		get
		{
			return this.goaltype;
		}
		set
		{
			this.goaltype = value;
		}
	}

	public string NameInfo
	{
		get
		{
			return this.nameInfo;
		}
		set
		{
			this.nameInfo = value;
			this.ShuXing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.nameInfo
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.BtnOneDay.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				GameInstance.Game.DemandQiRiKuangHuanInfo(3);
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					ID = this._ItemIndex,
					Type = this.goaltype
				});
			}
		};
	}

	private new void Start()
	{
		this.m_LocalPosition = base.gameObject.transform.localPosition;
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
			this.JianTou.spriteName = (this._ToggleState ? "jiantou" : "jiantouweixuanzhong");
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

	public void SetBtnStat(bool selected)
	{
		if (null != this.BtnOneDay)
		{
			if (selected)
			{
				this.BtnOneDay.Label.color = NGUIMath.HexToColorEx(14922604U);
				this.ShuXing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.nameInfo
				});
				this.BtnOneDay.Pressed = true;
				this.BtnOneDay.Refresh();
			}
			else
			{
				this.BtnOneDay.Label.color = NGUIMath.HexToColorEx(10323559U);
				this.ShuXing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					this.nameInfo
				});
				this.BtnOneDay.Pressed = false;
				this.BtnOneDay.Refresh();
			}
		}
	}

	public UISprite JianTou;

	public UILabel ShuXing;

	public DPSelectedItemEventHandler DPSelectedItem;

	public Vector3 m_LocalPosition;

	public MUQiRiKuangHuanPartGoalItemAttr GoalItemAttrPart;

	public GButton BtnOneDay;

	public UIButtonTween ButtonTween;

	public TweenScale ItemTweenScale;

	public SpriteSL Content;

	public GameObject _ActivityTipIcon;

	private int goaltype;

	private string nameInfo = string.Empty;

	private bool _ToggleState;

	private int _ItemIndex = -1;
}
