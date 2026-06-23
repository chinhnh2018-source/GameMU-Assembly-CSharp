using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CommonHelpWindow : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(this, new DPSelectedItemEventArgs());
			}
			if (this.ChildWindowClose != null)
			{
				this.ChildWindowClose(this, null);
			}
		};
	}

	public void SetHelpInfo(List<ChangeableRulePart.RuleVO> ruleVO)
	{
		List<ChangeableRulePart.RuleVO>.Enumerator enumerator = ruleVO.GetEnumerator();
		float num = 0f;
		while (enumerator.MoveNext())
		{
			ChangeableRulePart.RuleVO ruleVO2 = enumerator.Current;
			if (!string.IsNullOrEmpty(ruleVO2.Intro))
			{
				UILabel uilabel = NGUITools.AddWidget<UILabel>(this.mTabel.gameObject);
				uilabel.font = this.mFont;
				uilabel.lineWidth = this.lineWidth;
				uilabel.pivot = 0;
				uilabel.transform.localScale = Vector3.one * 18f;
				uilabel.transform.localPosition = new Vector3(0f, num, -0.001f);
				uilabel.color = new Color(0.854901969f, 0.78039217f, 0.68235296f);
				uilabel.renderStyle = ((ruleVO2.Bold != 1) ? 0 : 1);
				TextBlock textBlock = uilabel.gameObject.AddComponent<TextBlock>();
				textBlock.Label = uilabel;
				textBlock._CharMargin = new Vector2(0f, 16f);
				textBlock.Text = ruleVO2.Intro;
				num -= NGUIMath.CalculateRelativeWidgetBounds(uilabel.transform).size.y * uilabel.transform.localScale.y;
			}
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.mTabel.transform);
		BoxCollider boxCollider = this.mTabel.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = this.mTabel.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.center = new Vector3(bounds.size.x / 2f, -bounds.size.y / 2f, 0f);
		boxCollider.size = bounds.size;
		UIEventListener.Get(this.mTabel.gameObject);
		UIDragPanelContents uidragPanelContents = this.mTabel.GetComponent<UIDragPanelContents>();
		if (null == uidragPanelContents)
		{
			uidragPanelContents = this.mTabel.gameObject.AddComponent<UIDragPanelContents>();
		}
		uidragPanelContents.draggablePanel = this.mDragPanelContent;
	}

	public UIDraggablePanel DragPanel
	{
		set
		{
			if (null == base.GetComponent<UIDragPanelContents>())
			{
				base.gameObject.AddComponent<UIDragPanelContents>();
			}
			base.GetComponent<UIDragPanelContents>().draggablePanel = value;
			if (null == base.GetComponent<BoxCollider>())
			{
				base.gameObject.AddComponent<BoxCollider>();
			}
			base.GetComponent<BoxCollider>().size = new Vector3(960f, 470f, 0f);
		}
	}

	[SerializeField]
	private UIDraggablePanel mDragPanelContent;

	[SerializeField]
	private UITable mTabel;

	[SerializeField]
	private UIFont mFont;

	public GButton BtnClose;

	public int lineWidth = 480;

	public DPSelectedItemEventHandler CloseHandler;
}
