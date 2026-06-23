using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class GonggaoPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnClose.Text = Global.GetLang("关闭");
		this.lblTitle.text = Global.GetLang("公告");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
	}

	public void InitPartData(string noticeContent)
	{
		this.ParseContent(this.contentHolder, noticeContent);
	}

	public void ParseContent(GameObject contentHolder, string theNoticeContent)
	{
		XElement xelement = XElement.Parse(theNoticeContent);
		List<XElement> xelementList = Global.GetXElementList(xelement, "content");
		float num = 0f;
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		for (int i = 0; i < xelementList.Count; i++)
		{
			text = Global.GetXElementAttributeStr(xelementList[i], "text");
			text2 = Global.GetXElementAttributeStr(xelementList[i], "color");
			text3 = Global.GetXElementAttributeStr(xelementList[i], "align");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "size");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "incIndent");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList[i], "lineInterval");
			UILabel uilabel = SpawnManager.Instantiate(this.lblPrefab) as UILabel;
			U3DUtils.AddChild(contentHolder, uilabel.gameObject, true);
			uilabel.transform.localScale = new Vector3((float)xelementAttributeInt, (float)xelementAttributeInt, 0f);
			uilabel.lineWidth = 700;
			uilabel.text = string.Concat(new string[]
			{
				"{",
				text2,
				"}",
				text,
				"{-}"
			});
			uilabel.transform.name = "lbl_" + i;
			uilabel.pivot = 0;
			uilabel.lineWidth = 700 - xelementAttributeInt2;
			uilabel.transform.localPosition = new Vector3((float)(-350 + xelementAttributeInt2), 135f - num, -1f);
			num += (float)((int)(uilabel.relativeSize.y * uilabel.transform.localScale.y)) + (float)xelementAttributeInt3;
			if (text3.ToLower() == "top")
			{
				uilabel.pivot = 1;
			}
			else if (text3.ToLower() == "right")
			{
				uilabel.pivot = 2;
			}
		}
		BoxCollider component = contentHolder.transform.GetComponent<BoxCollider>();
		component.size = new Vector3(700f, num, 0f);
		component.center = new Vector3(0f, -(num - 270f) / 2f, -1f);
	}

	public static GameObject CreateGonggaoContentObj(GameObject parent)
	{
		return null;
	}

	public UILabel lblTitle;

	public UILabel lblPrefab;

	public UIPanel contentPanel;

	public GButton btnClose;

	public GameObject contentHolder;

	public DPSelectedItemEventHandler DPSelectedItem;
}
