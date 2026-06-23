using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ZhanMengLianSaiMainRankItem : UserControl
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
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
		}
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

	public void SetInf(string rankStr, string ZhanMengname, string leaderName, string Value)
	{
		if (rankStr.Length == 1)
		{
			int num = rankStr.SafeToInt32(0);
			if (3 >= num && 0 < num)
			{
				this.mLabelRankNum.text = string.Empty;
				this.mSpRankNum.spriteName = num.ToString();
				this.mSpRankNum.gameObject.SetActive(true);
			}
			else
			{
				this.mSpRankNum.gameObject.SetActive(false);
				if (num == -1)
				{
					this.mLabelRankNum.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						Global.GetLang("未上榜")
					});
				}
				else
				{
					this.mLabelRankNum.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						num
					});
				}
			}
		}
		else
		{
			this.mLabelRankNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				rankStr
			});
			this.mSpRankNum.gameObject.SetActive(false);
		}
		this.mLabelValue.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Value
		});
		this.mLabelZhanMengLeaderName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			leaderName
		});
		this.mLabelZhanMengName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9cbee3",
			ZhanMengname
		});
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
			if (null != base.GetComponent<UIPanel>())
			{
				Object.Destroy(base.GetComponent<UIPanel>());
			}
		}
	}

	[SerializeField]
	private UISprite mSpRankNum;

	[SerializeField]
	private UILabel mLabelRankNum;

	[SerializeField]
	private UILabel mLabelZhanMengName;

	[SerializeField]
	private UILabel mLabelZhanMengLeaderName;

	[SerializeField]
	private UILabel mLabelValue;
}
