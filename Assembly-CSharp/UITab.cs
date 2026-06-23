using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("NGUI/UI/Tab")]
public class UITab : MonoBehaviour
{
	public event UITab.TabClickHandler TabClick;

	private void Awake()
	{
		if (null == this.myTransform)
		{
			this.myTransform = base.transform;
		}
		this.tabBtns = NGUITools.GetComponentList<GButton>(this.myTransform);
		this.InitEvent();
	}

	public int TabIndex
	{
		get
		{
			return this._TabIndex;
		}
		set
		{
			this.SetTab(value);
		}
	}

	public GButton this[int i]
	{
		get
		{
			foreach (GButton gbutton in this.tabBtns)
			{
				if (gbutton.TagIndex == i)
				{
					return gbutton;
				}
			}
			return null;
		}
	}

	private void SetTabBtn(List<GButton> list, int index)
	{
		if (list != null)
		{
			foreach (GButton gbutton in list)
			{
				if (gbutton.TagIndex == index)
				{
					this._TabIndex = index;
					gbutton.Pressed = true;
					if (this.m_bIsSetTextColor)
					{
						UILabel componentInChildren = gbutton.gameObject.GetComponentInChildren<UILabel>();
						if (null != componentInChildren)
						{
							componentInChildren.color = this.m_HotTextColor;
						}
					}
				}
				else
				{
					gbutton.Pressed = false;
					if (this.m_bIsSetTextColor)
					{
						UILabel componentInChildren2 = gbutton.gameObject.GetComponentInChildren<UILabel>();
						if (null != componentInChildren2)
						{
							componentInChildren2.color = this.m_NorMalTextColor;
						}
					}
				}
				gbutton.Refresh();
			}
		}
	}

	private void InitEvent()
	{
		if (this.tabBtns != null)
		{
			foreach (GButton gbutton in this.tabBtns)
			{
				gbutton.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.SetTab);
			}
		}
	}

	private void SetTab(int index)
	{
		this.SetTabBtn(this.tabBtns, index);
		if (this.TabClick != null)
		{
			this.TabClick(null, index);
		}
	}

	private void SetTab(object sender, MouseEvent e)
	{
		GButton component = (sender as GameObject).GetComponent<GButton>();
		if (null == component)
		{
			return;
		}
		int tagIndex = component.TagIndex;
		if (this.PreChangeTabCallback != null && !this.PreChangeTabCallback(sender, new DPSelectedItemEventArgs
		{
			ID = tagIndex
		}))
		{
			return;
		}
		this.SetTabBtn(this.tabBtns, tagIndex);
		if (this.TabClick != null)
		{
			this.TabClick(null, tagIndex);
		}
	}

	public Transform myTransform;

	private List<GButton> tabBtns;

	public Color m_HotTextColor = Color.white;

	public Color m_NorMalTextColor = Color.cyan;

	public bool m_bIsSetTextColor;

	public DPSelectedItemBoolEventHandler PreChangeTabCallback;

	private int _TabIndex = -1;

	public delegate void TabClickHandler(GameObject sender, int index);
}
