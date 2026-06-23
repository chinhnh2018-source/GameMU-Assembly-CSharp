using System;
using UnityEngine;

public class ZhanMengLianSaiGloryhallItemOwn : UserControl
{
	protected override void InitializeComponent()
	{
		this.ObjBgShow = false;
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.Rank = -1;
		BoxCollider boxCollider = base.gameObject.GetComponent<BoxCollider>();
		if (null == boxCollider)
		{
			boxCollider = base.gameObject.AddComponent<BoxCollider>();
		}
		boxCollider.size = this.mObjBg.transform.localScale;
		UIDragPanelContents uidragPanelContents = base.gameObject.GetComponent<UIDragPanelContents>();
		if (null == uidragPanelContents)
		{
			uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
		}
	}

	private void InitPrefabText()
	{
		try
		{
			this.LabelInfStr = string.Empty;
			this.mLabelInf.transform.localPosition = new Vector3(-155f, this.mLabelInf.transform.localPosition.y, this.mLabelInf.transform.localPosition.z);
			this.mLabelInf.transform.localScale = new Vector3(15f, 15f, 1f);
			this.mLabelInf.lineWidth = 252;
			this.mSpInf.pivot = 5;
			this.mSpInf.transform.localPosition = new Vector3(160f, this.mLabelInf.transform.localPosition.y, this.mLabelInf.transform.localPosition.z);
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public string LabelInfStr
	{
		set
		{
			if (null != this.mLabelInf)
			{
				this.mLabelInf.text = value;
			}
		}
	}

	public int Rank
	{
		set
		{
			if (0 > value)
			{
				this.mSpInf.gameObject.SetActive(false);
			}
			else if (3 >= value)
			{
				if (null != this.mSpInf)
				{
					this.mSpInf.gameObject.SetActive(true);
					this.mSpInf.spriteName = "Rank" + value.ToString();
				}
			}
			else
			{
				this.mSpInf.gameObject.SetActive(false);
			}
		}
	}

	public bool ObjBgShow
	{
		set
		{
			if (null != this.mObjBg)
			{
				this.mObjBg.SetActive(value);
			}
		}
	}

	[SerializeField]
	private GameObject mObjBg;

	[SerializeField]
	private UILabel mLabelInf;

	[SerializeField]
	private UISprite mSpInf;
}
