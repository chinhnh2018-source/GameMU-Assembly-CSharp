using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ZhanMengLianSaiGloryhallItem : UserControl
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
		try
		{
			this.mLabelRankLabeel.text = string.Empty;
			this.mLabelRankBangHuiName.text = string.Empty;
			this.mLabelRankBangValue.text = string.Empty;
			this.mSpRankSp.gameObject.SetActive(false);
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
	}

	public void RefreshInf(int Rank, string banghuiName, string value)
	{
		if (4 > Rank)
		{
			this.mSpRankSp.gameObject.SetActive(true);
			this.mSpRankSp.spriteName = Rank.ToString();
		}
		else
		{
			this.mSpRankSp.gameObject.SetActive(false);
			this.mLabelRankLabeel.text = Rank.ToString();
		}
		this.mLabelRankBangHuiName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9cbee3",
			banghuiName
		});
		this.mLabelRankBangValue.text = value;
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
		}
	}

	public UIDraggablePanel DragPanel
	{
		set
		{
			UIDragPanelContents uidragPanelContents = base.gameObject.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	[SerializeField]
	private UISprite mSpRankSp;

	[SerializeField]
	private UILabel mLabelRankLabeel;

	[SerializeField]
	private UILabel mLabelRankBangHuiName;

	[SerializeField]
	private UILabel mLabelRankBangValue;
}
