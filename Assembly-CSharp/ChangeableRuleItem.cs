using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ChangeableRuleItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public void SetInf(List<ChangeableRulePart.RuleVO>.Enumerator ito, int ID)
	{
		string text = string.Empty;
		if (this.mRuleType == ChangeableRulePart.ChangeabelRulePartType.ZhanmengLianSai)
		{
			if (ID == 1)
			{
				text = "NetImages/GameRes/Images/Plate/LianSaiRuleSuperEX.jpg";
			}
			else if (ID == 2)
			{
				text = "NetImages/GameRes/Images/Plate/LianSaiRuleNewEX.jpg";
			}
			else
			{
				text = "NetImages/GameRes/Images/Plate/LianSaiRuleZhanChangEX.jpg";
			}
		}
		else if (this.mRuleType == ChangeableRulePart.ChangeabelRulePartType.KuaFuPluder)
		{
			if (ID == 1)
			{
				text = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_TaoFaGuiZe.png";
			}
			else if (ID == 2)
			{
				text = "NetImages/GameRes/Images/KuaFuPlunderImage/PeiTu_ZhanChangGuiZe.png";
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			this.mIamgeIcon.URL = text;
			this.mIamgeIcon.ImageDownloaded = delegate(object g)
			{
				this.mIamgeIcon.transform.localScale = new Vector3((float)this.mIamgeIcon.ItsSizeWidth, (float)this.mIamgeIcon.ItsSizeHeight, 0f);
			};
		}
		float num = 0f;
		while (ito.MoveNext())
		{
			ChangeableRulePart.RuleVO ruleVO = ito.Current;
			if (!string.IsNullOrEmpty(ruleVO.Intro))
			{
				UILabel uilabel = NGUITools.AddWidget<UILabel>(this.mTabel.gameObject);
				uilabel.font = this.mFont;
				uilabel.lineWidth = 380;
				uilabel.pivot = 0;
				uilabel.transform.localScale = Vector3.one * 18f;
				uilabel.transform.localPosition = new Vector3(0f, num, -0.001f);
				uilabel.color = new Color(218f, 199f, 174f);
				uilabel.renderStyle = ((ruleVO.Bold != 1) ? 0 : 1);
				TextBlock textBlock = uilabel.gameObject.AddComponent<TextBlock>();
				textBlock.Label = uilabel;
				textBlock._CharMargin = new Vector2(0f, 16f);
				textBlock.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					ruleVO.Intro
				});
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

	public ChangeableRulePart.ChangeabelRulePartType RuleType
	{
		set
		{
			this.mRuleType = value;
			if (this.mRuleType != ChangeableRulePart.ChangeabelRulePartType.ZhanmengLianSai)
			{
				if (this.mRuleType == ChangeableRulePart.ChangeabelRulePartType.KuaFuPluder)
				{
				}
			}
		}
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
	private ShowNetImage mIamgeIcon;

	[SerializeField]
	private UIDraggablePanel mDragPanelContent;

	[SerializeField]
	private UITable mTabel;

	[SerializeField]
	private UIFont mFont;

	private ChangeableRulePart.ChangeabelRulePartType mRuleType = ChangeableRulePart.ChangeabelRulePartType.ZhanmengLianSai;
}
