using System;
using System.Collections.Generic;
using UnityEngine;

public class ChatBoxPageController : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitStartInfoIfNecessary();
		this.pageContainerGird.Reposition();
	}

	private void InitStartInfoIfNecessary()
	{
		if (!this.m_beStarted)
		{
			this.iconPageItem.name = "page0";
			this.m_lstPage.Add(this.iconPageItem);
			this.m_orgPosition = this.pageContainerGird.transform.localPosition;
			this.m_beStarted = true;
		}
	}

	public void SetPageNum(int pageNum, int currentPage = 0)
	{
		this.InitStartInfoIfNecessary();
		this.m_pageNum = pageNum;
		if (this.m_lstPage.Count < pageNum)
		{
			for (int i = this.m_lstPage.Count; i < pageNum; i++)
			{
				GameObject gameObject = NGUITools.AddChild(this.pageContainerGird.gameObject, this.iconPageItem.gameObject);
				gameObject.name = "page" + i;
				UISprite component = gameObject.GetComponent<UISprite>();
				if (Vector3.one == this.SelectTransScale)
				{
					component.MakePixelPerfect();
				}
				else
				{
					component.transform.localScale = this.SelectTransScale;
				}
				this.m_lstPage.Add(component);
			}
			this.pageContainerGird.Reposition();
		}
		else if (this.m_lstPage.Count > pageNum)
		{
			for (int j = pageNum; j < this.m_lstPage.Count; j++)
			{
				Object.Destroy(this.m_lstPage[j].gameObject);
			}
			this.pageContainerGird.Reposition();
		}
		this.pageContainerGird.transform.localPosition = this.m_orgPosition + new Vector3(0f - (float)(this.m_pageNum - 1) * this.pageContainerGird.cellWidth / 2f, 0f, 0f);
		this.SetCurrentPage(currentPage);
	}

	public void SetCurrentPage(int pageIndex)
	{
		if (pageIndex <= 0)
		{
			pageIndex = 0;
		}
		if (pageIndex > this.m_pageNum - 1)
		{
			pageIndex = this.m_pageNum - 1;
		}
		for (int i = 0; i < this.m_lstPage.Count; i++)
		{
			if (i == pageIndex)
			{
				this.m_lstPage[i].spriteName = this.selectPageSpriteName;
				if (Vector3.one != this.SelectTransScale)
				{
					this.m_lstPage[i].transform.localScale = this.SelectTransScale;
				}
			}
			else
			{
				this.m_lstPage[i].spriteName = this.unselectPageSpriteName;
				if (Vector3.one != this.unselectTransScale)
				{
					this.m_lstPage[i].transform.localScale = this.unselectTransScale;
				}
			}
		}
	}

	public UIGrid pageContainerGird;

	public UISprite iconPageItem;

	public string selectPageSpriteName = "fanye";

	public string unselectPageSpriteName = "fanye2";

	private List<UISprite> m_lstPage = new List<UISprite>();

	[SerializeField]
	private Vector3 SelectTransScale = Vector3.one;

	[SerializeField]
	private Vector3 unselectTransScale = Vector3.one;

	private int m_pageNum;

	private Vector3 m_orgPosition;

	private bool m_beStarted;
}
