using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PlayerRecallItem : UserControl
{
	public bool toggleState
	{
		get
		{
			return this._toggleState;
		}
		set
		{
			if (this._toggleState != value)
			{
				this._toggleState = value;
				this.buttonTween.Play(this._toggleState);
				this.SetPanelPosition();
			}
		}
	}

	public int itemIndex
	{
		get
		{
			return this._itemIndex;
		}
		set
		{
			this._itemIndex = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.detailLabel.text = Global.GetLang("点击查看");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.background.gameObject).onClick = delegate(GameObject s)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(null, new DPSelectedItemEventArgs
				{
					ID = this._itemIndex
				});
			}
		};
	}

	private new void Start()
	{
		this.position = base.gameObject.transform.localPosition;
	}

	private new void Update()
	{
		float num = this.position.y - base.gameObject.transform.localPosition.y;
		if (this.content.gameObject.activeInHierarchy && this.position.y - base.gameObject.transform.localPosition.y != 0f && this.position.y != base.gameObject.transform.localPosition.y)
		{
			base.gameObject.transform.localPosition = this.position;
		}
	}

	private void SetPanelPosition()
	{
		if (!this._toggleState)
		{
			if (this._itemIndex == 4)
			{
				RecallRewards componentInChildren = base.gameObject.GetComponentInChildren<RecallRewards>();
				if (null != componentInChildren)
				{
					componentInChildren.draggablePanel.gameObject.transform.localPosition = componentInChildren.panelPosition;
					componentInChildren.draggablePanel.clipRange = componentInChildren.panelClipRange;
				}
			}
			else if (this._itemIndex == 0)
			{
				RecallSignIn componentInChildren2 = base.gameObject.GetComponentInChildren<RecallSignIn>();
				if (null != componentInChildren2)
				{
					componentInChildren2.draggablePanel.gameObject.transform.localPosition = componentInChildren2.panelPosition;
					componentInChildren2.draggablePanel.clipRange = componentInChildren2.panelClipRange;
				}
			}
		}
	}

	public void SetRecallItemInfo(string info)
	{
		if (null != this.detail)
		{
			this.detail.text = info;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public Vector3 position = new Vector3(0f, 0f, 0f);

	public ShowNetImage background;

	public UIButtonTween buttonTween;

	public TweenScale tweenScale;

	public SpriteSL content;

	public GameObject activityTipIcon;

	public TextBlock detailLabel;

	public TextBlock detail;

	private bool _toggleState;

	private int _itemIndex = -1;
}
