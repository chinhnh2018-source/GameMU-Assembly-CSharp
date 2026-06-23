using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class KuaFuPlunderRankItem : UserControl
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
			this.mRankSp.gameObject.SetActive(false);
			this.mRankLabel.text = string.Empty;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
			this.mSelectEffestObj.SetActive(false);
			UIEventListener.Get(base.gameObject).onPress = delegate(GameObject g, bool s)
			{
				this.mSelectEffestObj.SetActive(s);
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void SetInf(int rankNum, string bhName, int valuee)
	{
		if (4 > rankNum)
		{
			this.mRankLabel.text = string.Empty;
			this.mRankSp.spriteName = rankNum.ToString();
			this.mRankSp.gameObject.SetActive(true);
		}
		else
		{
			this.mRankSp.gameObject.SetActive(false);
			this.mRankLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				rankNum.ToString()
			});
		}
		if (!string.IsNullOrEmpty(bhName))
		{
			this.mBHNameLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"9cbee3",
				bhName
			});
		}
		if (0 <= valuee)
		{
			this.mPlunderValueLbale.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				valuee
			});
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
			if (base.GetComponent<UIPanel>())
			{
				Object.Destroy(base.GetComponent<UIPanel>());
			}
		}
	}

	[SerializeField]
	private GameObject mSelectEffestObj;

	[SerializeField]
	private UISprite mRankSp;

	[SerializeField]
	private UILabel mRankLabel;

	[SerializeField]
	private UILabel mBHNameLable;

	[SerializeField]
	private UILabel mPlunderValueLbale;
}
