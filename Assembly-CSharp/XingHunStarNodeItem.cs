using System;
using UnityEngine;

public class XingHunStarNodeItem : UserControl
{
	public bool m_bActive
	{
		get
		{
			return this.bActive;
		}
		set
		{
			if (value)
			{
				this.m_ObjMove.gameObject.SetActive(true);
			}
			else
			{
				this.m_ObjTeXiaoXuanZhuan.gameObject.SetActive(true);
				this.m_SprFor.gameObject.SetActive(false);
				this.m_ObjMove.gameObject.SetActive(false);
			}
			this.bActive = value;
		}
	}

	private new void Start()
	{
	}

	private new void Update()
	{
		if (null != this.m_SprBak && 255f > this.m_SprBak.alpha)
		{
			this.m_SprBak.alpha += 0.02f;
		}
		if (null != this.m_SprFor && 255f > this.m_SprFor.alpha)
		{
			this.m_SprFor.alpha += 0.02f;
		}
	}

	public UISprite m_SprBak;

	public UISprite m_SprFor;

	public UISprite m_SprTeXiao;

	public GameObject m_AniJiHuo;

	public GameObject m_ObjMove;

	public GameObject m_ObjTeXiaoYiDong;

	public GameObject m_ObjTeXiaoXuanZhuan;

	private bool bActive;
}
