using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;
using XMLCreater;

public class JueXingJiHuoMenu : MonoBehaviour
{
	public void Awake()
	{
		this.btnTaoAttackMenu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetAttTaoZhuangShow(!this.m_beAttackShow);
		};
		this.btnTaoDefMenu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetDefTaoZhuangShow(!this.m_beDefShow);
		};
		BoxCollider component = this.btnTaoAttackMenu.GetComponent<BoxCollider>();
		this.m_menuBtnHeight = component.size.y;
	}

	public void InitTaoZhuangInfos(List<MUAwakenSuitDetail> attackTaoZhuangs, List<MUAwakenSuitDetail> defTaoZhuangs)
	{
		this.m_attackTaoZhuangs = attackTaoZhuangs;
		this.m_defTaoZhuangs = defTaoZhuangs;
		this.m_items.Clear();
		JueXingJiHuoMenuItem jueXingJiHuoMenuItem = null;
		for (int i = 0; i < this.m_attackTaoZhuangs.Count; i++)
		{
			JueXingJiHuoMenuItem jueXingJiHuoMenuItem2 = U3DUtils.NEW<JueXingJiHuoMenuItem>();
			jueXingJiHuoMenuItem2.transform.SetParent(this.attackMenuContent, false);
			this.m_menuItemHeight = jueXingJiHuoMenuItem2.GetHeight();
			jueXingJiHuoMenuItem2.transform.localPosition = new Vector3(0f, 0f - this.m_menuItemHeight * (float)i, 0f);
			jueXingJiHuoMenuItem2.SetMenuContent(this.m_attackTaoZhuangs[i]);
			jueXingJiHuoMenuItem2.OnMenuItemSelect = new Action<JueXingJiHuoMenuItem>(this.OnMenuItemSelect);
			this.m_items.Add(jueXingJiHuoMenuItem2);
			if (jueXingJiHuoMenuItem == null)
			{
				jueXingJiHuoMenuItem = jueXingJiHuoMenuItem2;
			}
		}
		for (int j = 0; j < this.m_defTaoZhuangs.Count; j++)
		{
			JueXingJiHuoMenuItem jueXingJiHuoMenuItem3 = U3DUtils.NEW<JueXingJiHuoMenuItem>();
			jueXingJiHuoMenuItem3.transform.SetParent(this.defMenuContent, false);
			this.m_menuItemHeight = jueXingJiHuoMenuItem3.GetHeight();
			jueXingJiHuoMenuItem3.transform.localPosition = new Vector3(0f, 0f - this.m_menuItemHeight * (float)j, 0f);
			jueXingJiHuoMenuItem3.SetMenuContent(this.m_defTaoZhuangs[j]);
			jueXingJiHuoMenuItem3.OnMenuItemSelect = new Action<JueXingJiHuoMenuItem>(this.OnMenuItemSelect);
			this.m_items.Add(jueXingJiHuoMenuItem3);
			if (jueXingJiHuoMenuItem == null)
			{
				jueXingJiHuoMenuItem = jueXingJiHuoMenuItem3;
			}
		}
		this.SetAttTaoZhuangShow(true);
		this.SetDefTaoZhuangShow(true);
		if (jueXingJiHuoMenuItem != null)
		{
			this.OnMenuItemSelect(jueXingJiHuoMenuItem);
		}
	}

	private void OnMenuItemSelect(JueXingJiHuoMenuItem item)
	{
		if (this.m_selectedItem == item)
		{
			return;
		}
		if (this.m_selectedItem != null)
		{
			this.m_selectedItem.SetSelectState(false);
		}
		item.SetSelectState(true);
		this.m_selectedItem = item;
		if (this.OnSelectTaoZhuang != null)
		{
			this.OnSelectTaoZhuang.Invoke(item.GetTaoZhuang());
		}
	}

	private void SetAttTaoZhuangShow(bool beShow)
	{
		this.m_beAttackShow = beShow;
		this.btnTaoAttackMenu.Text = Global.GetLang("攻击觉醒");
		this.attackMenuContent.gameObject.SetActive(beShow);
		string text = "bg2";
		if (this.m_beAttackShow)
		{
			text = "bg1";
		}
		this.btnTaoAttackMenu.target.spriteName = text;
		this.btnTaoAttackMenu.normalSprite = text;
		this.btnTaoAttackMenu.hoverSprite = text;
		this.btnTaoAttackMenu.pressedSprite = text;
		float num;
		if (beShow)
		{
			num = this.m_menuBtnHeight + this.m_menuItemHeight * (float)this.m_attackTaoZhuangs.Count + this.m_offHeight;
		}
		else
		{
			num = this.m_menuBtnHeight + this.m_offHeight;
		}
		this.defMenuTrans.localPosition = new Vector3(0f, 0f - num, 0f);
	}

	private void SetDefTaoZhuangShow(bool beShow)
	{
		this.m_beDefShow = beShow;
		string text = "bg2";
		if (this.m_beDefShow)
		{
			text = "bg1";
		}
		this.btnTaoDefMenu.Text = Global.GetLang("防御觉醒");
		this.btnTaoDefMenu.target.spriteName = text;
		this.btnTaoDefMenu.normalSprite = text;
		this.btnTaoDefMenu.hoverSprite = text;
		this.btnTaoDefMenu.pressedSprite = text;
		this.defMenuContent.gameObject.SetActive(beShow);
	}

	public void RefershContent()
	{
		for (int i = 0; i < this.m_items.Count; i++)
		{
			this.m_items[i].RefershContent();
		}
	}

	public GButton btnTaoAttackMenu;

	public GButton btnTaoDefMenu;

	public Transform attackMenuContent;

	public Transform defMenuContent;

	public Transform defMenuTrans;

	public Action<MUAwakenSuitDetail> OnSelectTaoZhuang;

	private List<MUAwakenSuitDetail> m_attackTaoZhuangs;

	private List<MUAwakenSuitDetail> m_defTaoZhuangs;

	private bool m_beAttackShow;

	private bool m_beDefShow;

	private List<JueXingJiHuoMenuItem> m_items = new List<JueXingJiHuoMenuItem>();

	private float m_menuBtnHeight = 55f;

	private float m_menuItemHeight;

	private float m_offHeight = 5f;

	private JueXingJiHuoMenuItem m_selectedItem;
}
