using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuodongPartChongzhiHuikuiPartItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_LblShowState.text = Global.GetLang("点击查看");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.ItemHeadBak.gameObject).onClick = delegate(GameObject s)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					ID = this._ItemIndex
				});
			}
		};
	}

	private new void Start()
	{
		this.m_LocalPosition = base.gameObject.transform.localPosition;
	}

	private new void Update()
	{
		float num = this.m_LocalPosition.y - base.gameObject.transform.localPosition.y;
		if (this.Content.gameObject.activeInHierarchy && this.m_LocalPosition.y - base.gameObject.transform.localPosition.y != 0f && this.m_LocalPosition.y != base.gameObject.transform.localPosition.y)
		{
			base.gameObject.transform.localPosition = this.m_LocalPosition;
		}
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
				this.SetPnlPos();
			}
		}
	}

	private void SetPnlPos()
	{
		if (!this._ToggleState)
		{
			if (this._ItemIndex == 2)
			{
				HuodongPartChongzhiHuikuiPartLeijiChongzhiPart componentInChildren = base.gameObject.GetComponentInChildren<HuodongPartChongzhiHuikuiPartLeijiChongzhiPart>();
				if (null != componentInChildren)
				{
					componentInChildren.m_Pnl.gameObject.transform.localPosition = componentInChildren.m_VecPnl;
					componentInChildren.m_Pnl.clipRange = componentInChildren.m_VecClipPnl;
				}
			}
			if (this._ItemIndex == 3)
			{
				HuodongPartChongzhiHuikuiPartLeijiXiaofeiPart componentInChildren2 = base.gameObject.GetComponentInChildren<HuodongPartChongzhiHuikuiPartLeijiXiaofeiPart>();
				if (null != componentInChildren2)
				{
					componentInChildren2.m_Pnl.gameObject.transform.localPosition = componentInChildren2.m_VecPnl;
					componentInChildren2.m_Pnl.clipRange = componentInChildren2.m_VecClipPnl;
				}
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

	public DPSelectedItemEventHandler DPSelectedItem;

	public Vector3 m_LocalPosition = new Vector3(0f, 0f, 0f);

	public ShowNetImage ItemHeadBak;

	public UIButtonTween ButtonTween;

	public TweenScale ItemTweenScale;

	public SpriteSL Content;

	public GameObject _ActivityTipIcon;

	public UILabel m_LblShowInfo;

	public UILabel m_LblShowState;

	public UISprite m_SprShowState;

	private bool _ToggleState;

	private int _ItemIndex = -1;
}
