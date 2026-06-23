using System;
using UnityEngine;

public class FashionPropretyItem : UserControl
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
		this.m_Contentlabel_1.pivot = 5;
		this.m_Contentlabel_1.transform.localPosition = new Vector3(55f, this.m_Contentlabel_1.transform.localPosition.y, this.m_Contentlabel_1.transform.localPosition.z);
		this.m_UpSp.transform.localPosition = new Vector3(5f, this.m_UpSp.transform.localPosition.y, this.m_UpSp.transform.localPosition.z);
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public void SetContent(string Content, string Content_1 = "")
	{
		NGUITools.SetActive(this.m_UpSp, !string.IsNullOrEmpty(Content_1));
		NGUITools.SetActive(this.m_Contentlabel_1, !string.IsNullOrEmpty(Content_1));
		this.m_Contentlabel_0.text = Content;
		this.m_Contentlabel_1.text = Content_1;
		if ("0" == this.m_Contentlabel_1.text)
		{
			NGUITools.SetActive(this.m_UpSp, false);
			NGUITools.SetActive(this.m_Contentlabel_1, false);
		}
	}

	public void DeletePanel()
	{
		UIPanel component = base.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null != value)
			{
				if (null == this.m_DragpanelContents)
				{
					this.m_DragpanelContents = base.GetComponent<UIDragPanelContents>();
					if (null == this.m_DragpanelContents)
					{
						this.m_DragpanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
					}
				}
				this.m_DragpanelContents.draggablePanel = value;
			}
		}
	}

	public UILabel m_Contentlabel_0;

	public UILabel m_Contentlabel_1;

	public UISprite m_UpSp;

	private UIDragPanelContents m_DragpanelContents;
}
