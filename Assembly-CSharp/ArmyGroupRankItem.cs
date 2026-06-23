using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ArmyGroupRankItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.MBSelect = false;
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		Vector3 localScale = this.BgSP.transform.localScale;
		this.ItemBoxCollider.size = localScale;
		this.ItemBoxCollider.isTrigger = false;
	}

	public void SetContent(string[] content)
	{
		for (int i = 0; i < this.InFLabels.Length; i++)
		{
			if (i == 0)
			{
				string text = content[i];
				text = Super.ClearStringColor(text);
				if (4 >= text.SafeToInt32(0))
				{
					NGUITools.SetActive(this.InFLabels[i], false);
					NGUITools.SetActive(this.RankSp, true);
					this.RankSp.spriteName = text;
				}
				else
				{
					NGUITools.SetActive(this.InFLabels[i], true);
					NGUITools.SetActive(this.RankSp, false);
					this.InFLabels[i].text = content[i];
				}
			}
			else
			{
				this.InFLabels[i].text = content[i];
			}
		}
	}

	public bool MBSelect
	{
		set
		{
			this.mBSelect = value;
			NGUITools.SetActive(this.SelectEffectObj, this.mBSelect);
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == this.mUIDragPanelContents)
			{
				this.mUIDragPanelContents = base.GetComponent<UIDragPanelContents>();
			}
			if (null == this.mUIDragPanelContents)
			{
				this.mUIDragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			this.mUIDragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public UISprite BgSP;

	public GameObject SelectEffectObj;

	public BoxCollider ItemBoxCollider;

	public UILabel[] InFLabels;

	public UISprite RankSp;

	private bool mBSelect;

	private UIDragPanelContents mUIDragPanelContents;
}
