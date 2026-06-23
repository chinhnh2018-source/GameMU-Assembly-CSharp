using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class XingHunItem : UserControl
{
	protected override void InitializeComponent()
	{
		if (null != this.m_SprLineBak)
		{
			this.m_SprLineBak.gameObject.SetActive(false);
		}
		UIEventListener.Get(this.m_UITexBtnBak.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.m_UITexBtnBakGray.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public string strBtnRes
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public string strSprBak
	{
		get
		{
			return string.Empty;
		}
		set
		{
		}
	}

	public int nIndex
	{
		get
		{
			return this.m_nXingZuoIndex;
		}
		set
		{
			this.m_nXingZuoIndex = value;
		}
	}

	public string strXingZuoName
	{
		get
		{
			return this.m_strXingZuoName;
		}
		set
		{
			this.m_strXingZuoName = value;
		}
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			if (this.m_ObgTeXiao.activeInHierarchy && this.m_nFirstActive != 1)
			{
				XingHunLogoTeXiao logoTeXiao = this.m_ObgTeXiao.gameObject.GetComponentInChildren<XingHunLogoTeXiao>();
				if (null != logoTeXiao)
				{
					Object.Destroy(this.m_ObgTeXiao.gameObject.GetComponentInChildren<XingHunLogoTeXiao>().gameObject);
				}
				XingHunLogoTeXiao teXiao = U3DUtils.NEW<XingHunLogoTeXiao>();
				U3DUtils.AddChild(this.m_ObgTeXiao.gameObject, teXiao.gameObject, true);
			}
			this.m_nFirstActive = -1;
			yield return new WaitForSeconds(4f);
		}
		yield break;
	}

	private new void Start()
	{
		base.StartCoroutine<bool>(this.TimeProc());
	}

	public int m_nIndex;

	public int m_nFirstActive;

	public UISprite m_SprLineBak;

	public ShowNetImage m_TexBak;

	public ShowNetImage m_TexBtnBak;

	public ShowNetImage m_TexBtnGrayBak;

	public UITexture m_UITextBak;

	public UITexture m_UITexBtnBak;

	public UITexture m_UITexBtnBakGray;

	public string m_strLimit = string.Empty;

	public bool m_bIsGray;

	public GameObject m_ObgTeXiao;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int m_nXingZuoIndex = -1;

	private string m_strXingZuoName = string.Empty;
}
