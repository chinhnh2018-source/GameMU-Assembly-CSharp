using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GTxtMenuPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.BakTrans = this.Bak.transform;
		UIEventListener.Get(this.MaskBak.gameObject).onClick = delegate(GameObject e)
		{
			if (this.Closehandler != null)
			{
				this.Closehandler(e, new DPSelectedItemEventArgs());
			}
		};
	}

	public int ItemCount
	{
		get
		{
			return this.ItemIDList.Count;
		}
	}

	public int SelectIndex
	{
		get
		{
			return this._SelectIndex;
		}
		set
		{
			this._SelectIndex = value;
			for (int i = 0; i < this.MenuList.children.Count; i++)
			{
				GTxtMenuItem component = this.MenuList.children[i].GetComponent<GTxtMenuItem>();
				if (this._SelectIndex == i)
				{
					component.BakVisible = true;
				}
				else
				{
					component.BakVisible = false;
				}
			}
		}
	}

	public int ItemHeight
	{
		set
		{
			this._ItemHeight = value;
		}
	}

	public void AddMenuItem(int id, string text)
	{
		this.ItemIDList.Add(id);
		this.ItemTextList.Add(text);
	}

	public void RenderMenu()
	{
		if (this.ItemIDList.Count <= 0)
		{
			return;
		}
		this.SetMenuSize();
		Vector3 bakSize;
		bakSize..ctor(this.BakTrans.localScale.x - 20f, (float)(this._ItemHeight - 5), 1f);
		for (int i = 0; i < this.ItemIDList.Count; i++)
		{
			GTxtMenuItem gtxtMenuItem = U3DUtils.NEW<GTxtMenuItem>();
			gtxtMenuItem.BakSize = bakSize;
			gtxtMenuItem.MenuItemID = this.ItemIDList[i];
			gtxtMenuItem.MenuItemText = this.ItemTextList[i];
			gtxtMenuItem.MenuItemClick = new EventHandler(this.MyMenuItemClick);
			U3DUtils.AddChild(this.MenuList.gameObject, gtxtMenuItem.gameObject, true);
		}
	}

	private void SetMenuSize()
	{
		float num = (float)this.Width;
		float num2 = (float)(this._ItemHeight * this.ItemIDList.Count + 20);
		this.BakTrans.localScale = new Vector3(num, num2, this.BakTrans.localScale.z);
	}

	private void MyMenuItemClick(object sender, object e)
	{
		if (this.MenuItemClick != null)
		{
			GTxtMenuItem gtxtMenuItem = sender as GTxtMenuItem;
			this.SelectIndex = gtxtMenuItem.MenuItemID;
			this.MenuItemClick.Invoke(sender, e as EventArgs);
		}
	}

	public int Speech
	{
		set
		{
			if (value == 2)
			{
				int num = this.ItemTextList.FindIndex((string tmp) => tmp == Global.GetLang("允许语音"));
				if (num >= 0)
				{
					this.ItemTextList[num] = Global.GetLang("禁止语音");
				}
				int num2 = this.ItemTextList.FindIndex((string tmp) => tmp == Global.GetLang("禁止语音"));
				if (num2 >= 0)
				{
					this.ItemTextList[num2] = Global.GetLang("禁止语音");
				}
			}
			else if (value == 3)
			{
				int num3 = this.ItemTextList.FindIndex((string tmp) => tmp == Global.GetLang("允许语音"));
				if (num3 >= 0)
				{
					this.ItemTextList[num3] = Global.GetLang("允许语音");
				}
				int num4 = this.ItemTextList.FindIndex((string tmp) => tmp == Global.GetLang("禁止语音"));
				if (num4 >= 0)
				{
					this.ItemTextList[num4] = Global.GetLang("允许语音");
				}
			}
			for (int i = 0; i < this.MenuList.children.Count; i++)
			{
				GTxtMenuItem component = this.MenuList.children[i].GetComponent<GTxtMenuItem>();
				component.MenuItemText = this.ItemTextList[i];
			}
		}
	}

	public DPSelectedItemEventHandler Closehandler;

	public UITable MenuList;

	public UISprite MaskBak;

	public UISprite Bak;

	private Transform BakTrans;

	public EventHandler MenuItemClick;

	private List<int> ItemIDList = new List<int>();

	private List<string> ItemTextList = new List<string>();

	private int _SelectIndex;

	private int _ItemHeight;
}
