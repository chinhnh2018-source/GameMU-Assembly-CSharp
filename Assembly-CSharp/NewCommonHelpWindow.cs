using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class NewCommonHelpWindow : UserControl
{
	private void InitTextInPrefabs()
	{
		this.mLblTitle.Text = Global.GetLang("详细说明");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			UIFont.IndentCount = 0;
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void SetHelpInfo(List<ChangeableRulePart.RuleVO> ruleVO, bool isCenter = false)
	{
		List<ChangeableRulePart.RuleVO>.Enumerator enumerator = ruleVO.GetEnumerator();
		if (isCenter)
		{
			this.mChildTransform.localPosition = new Vector3(80f, 0f, 0f);
		}
		float num = 0f;
		while (enumerator.MoveNext())
		{
			ChangeableRulePart.RuleVO ruleVO2 = enumerator.Current;
			if (!string.IsNullOrEmpty(ruleVO2.Intro))
			{
				if (ruleVO2.ID != 1)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.mLblObj);
					gameObject.SetActive(true);
					TextBlock component = gameObject.GetComponent<TextBlock>();
					component.Label.renderStyle = ((ruleVO2.Bold != 1) ? 0 : 1);
					component._CharMargin = new Vector2(0f, 16f);
					component.Label.lineWidth = this.lineWidth;
					component.Text = ruleVO2.Intro;
					UIFont.IndentCount = 1;
					component.Label.pivot = 3;
					Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(component.transform);
					BoxCollider component2 = component.transform.GetComponent<BoxCollider>();
					if (component2 == null)
					{
						component.gameObject.AddComponent<BoxCollider>();
					}
					component2 = component.transform.GetComponent<BoxCollider>();
					component2.center = new Vector3(bounds.size.x / 2f, 0f, 0f);
					component2.size = bounds.size;
					num -= bounds.size.y * component.transform.localScale.y;
					UIEventListener component3 = component.transform.GetComponent<UIEventListener>();
					if (component3 == null)
					{
						component.gameObject.AddComponent<UIEventListener>();
					}
					UIDragPanelContents component4 = component.transform.GetComponent<UIDragPanelContents>();
					if (component4 == null)
					{
						component.gameObject.AddComponent<UIDragPanelContents>();
					}
					component.transform.localPosition = new Vector3(1f, num, component.transform.localPosition.z);
					component.transform.localScale = Vector3.one * (float)this.fontSize;
					U3DUtils.AddChild(this.mTabel.gameObject, component.gameObject, true);
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

	protected override void OnDestroy()
	{
		UIFont.IndentCount = 0;
		base.OnDestroy();
	}

	[SerializeField]
	private TextBlock mLblTitle;

	[SerializeField]
	private UIDraggablePanel mDragPanelContent;

	[SerializeField]
	private UITable mTabel;

	[SerializeField]
	private UIFont mFont;

	public GButton BtnClose;

	public UISprite mBak;

	public int lineWidth = 360;

	private int fontSize = 20;

	public DPSelectedItemEventHandler CloseHandler;

	public GameObject mLblObj;

	public Transform mChildTransform;
}
