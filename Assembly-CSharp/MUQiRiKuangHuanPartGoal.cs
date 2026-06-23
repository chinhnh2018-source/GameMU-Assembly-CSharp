using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MUQiRiKuangHuanPartGoal : UserControl
{
	public int Day
	{
		get
		{
			return this.day;
		}
		set
		{
			this.day = value;
			this.initList();
		}
	}

	protected override void InitializeComponent()
	{
		this.ItemList.IsPosYFixed = true;
	}

	private string InitBtnItem(int typeID)
	{
		XElement gameResXml = Global.GetGameResXml("Config/GoalType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "GoalType");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TypeID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				if (typeID == xelementAttributeInt)
				{
					return xelementAttributeStr;
				}
			}
		}
		return null;
	}

	private void initList()
	{
		if (this.items == null)
		{
			this.items = new MUQiRiKuangHuanPartGoalItem[this.itemsCount];
		}
		int num = this.day * 2 - 1;
		for (int i = 0; i < this.itemsCount; i++)
		{
			if (this.items[i] == null)
			{
				this.items[i] = U3DUtils.NEW<MUQiRiKuangHuanPartGoalItem>();
				this.items[i].GoalType = num;
				this.items[i].NameInfo = this.InitBtnItem(num);
				this.items[i].ItemIndex = i;
			}
			if (i == 1)
			{
				this.initItemPart(i, num);
			}
			this.items[i].DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (!this.IsTween)
				{
					this.initItemPart(e.ID, e.Type);
				}
			};
			num++;
			U3DUtils.AddChild(this.ItemList.gameObject, this.items[i].gameObject, false);
		}
	}

	private void setTab(int index = -1)
	{
		if (index >= 0)
		{
			if (this.selectedIndex >= 0)
			{
				if (index == this.selectedIndex)
				{
					this.items[this.selectedIndex].ToggleState = !this.items[this.selectedIndex].ToggleState;
					if (this.selectedIndex == 0)
					{
						this.items[1].ToggleState = true;
						this.initItemPart(1);
						this.selectedIndex = 1;
						return;
					}
					if (this.selectedIndex == 1)
					{
						this.items[0].ToggleState = true;
						this.initItemPart(0);
						this.selectedIndex = 0;
					}
					return;
				}
				else if (this.items[this.selectedIndex].ToggleState)
				{
					this.items[this.selectedIndex].ToggleState = false;
				}
			}
			this.selectedIndex = index;
			this.items[this.selectedIndex].ToggleState = true;
		}
	}

	private void tweenPosPart(int index)
	{
		this.toggleState = !this.toggleState;
		if (index == this.selectedIndex)
		{
			Vector3 vector = this.fromPos;
			this.fromPos = this.toPos;
			this.toPos = vector;
			this.IsTween = true;
			this.PartTweenPosition.Play(this.toggleState);
		}
		else
		{
			float num = -((float)index * (40f + this.ItemList.padding.y * 2f));
			this.fromPos = this.toPos;
			this.toPos = new Vector3(this.toPos.x, this.toPos.y, this.toPos.z);
			this.PartTweenPosition.from = this.fromPos;
			this.PartTweenPosition.to = this.toPos;
			this.IsTween = true;
			this.PartTweenPosition.Play(this.toggleState);
		}
		this.PartTweenPosition.onFinished = new UITweener.OnFinished(this.onFinished);
	}

	private void onFinished(UITweener tween)
	{
		this.IsTween = false;
		foreach (MUQiRiKuangHuanPartGoalItem muqiRiKuangHuanPartGoalItem in this.items)
		{
			muqiRiKuangHuanPartGoalItem.transform.localPosition = NGUITools.Round(muqiRiKuangHuanPartGoalItem.transform.localPosition);
			muqiRiKuangHuanPartGoalItem.m_LocalPosition = NGUITools.Round(muqiRiKuangHuanPartGoalItem.transform.localPosition);
		}
	}

	public void SetBtnState(SevenDayActQueryData sevendayactquerydata)
	{
		if (this.whichone == 0)
		{
			this.firstChongzhiPart.SetBtnState(sevendayactquerydata);
		}
		if (this.whichone == 1)
		{
			this.meiriChongzhiPart.SetBtnState(sevendayactquerydata);
		}
	}

	public void SetBtnState(int ID)
	{
		if (this.whichone == 0)
		{
			this.firstChongzhiPart.setCompletedInfo(ID);
		}
		if (this.whichone == 1)
		{
			this.meiriChongzhiPart.setCompletedInfo(ID);
		}
	}

	private void initItemPart(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				this.meiriChongzhiPart.ResetPanelGiftsPos(19, -104, 756, 291, 323);
			}
		}
		else
		{
			if (null == this.firstChongzhiPart)
			{
				this.firstChongzhiPart = U3DUtils.NEW<MUQiRiKuangHuanPartGoalItemAttr>();
				this.firstChongzhiPart.GetGoalTypeInfo(this.items[0].GoalType);
				this.whichone = 0;
				this.items[index].Content.Add(this.firstChongzhiPart);
			}
			this.firstChongzhiPart.ResetPanelGiftsPos(12, -129, 730, 340, 365);
		}
		this.tweenPosPart(index);
		this.OpenItem(index);
	}

	public void initItemPart(int index, int goaltype)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				if (null == this.meiriChongzhiPart)
				{
					this.meiriChongzhiPart = U3DUtils.NEW<MUQiRiKuangHuanPartGoalItemAttr>();
					this.meiriChongzhiPart.GetGoalTypeInfo(goaltype);
					this.whichone = 1;
					this.items[index].Content.Add(this.meiriChongzhiPart);
				}
				this.meiriChongzhiPart.ResetPanelGiftsPos(19, -104, 756, 291, 323);
			}
		}
		else
		{
			if (null == this.firstChongzhiPart)
			{
				this.firstChongzhiPart = U3DUtils.NEW<MUQiRiKuangHuanPartGoalItemAttr>();
				this.firstChongzhiPart.GetGoalTypeInfo(goaltype);
				this.whichone = 0;
				this.items[index].Content.Add(this.firstChongzhiPart);
			}
			this.firstChongzhiPart.ResetPanelGiftsPos(12, -129, 730, 340, 365);
		}
		this.tweenPosPart(index);
		this.OpenItem(index);
		this.setTab(index);
	}

	private void OpenItem(int index)
	{
		for (int i = 0; i < this.items.Length; i++)
		{
			bool btnStat = false;
			if (index == i)
			{
				btnStat = true;
			}
			this.items[i].SetBtnStat(btnStat);
		}
	}

	public MUQiRiKuangHuanPartGoalItemAttr firstChongzhiPart;

	public MUQiRiKuangHuanPartGoalItemAttr meiriChongzhiPart;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UITable ItemList;

	public TweenPosition PartTweenPosition;

	private MUQiRiKuangHuanPartGoalItem[] items;

	private int itemsCount = 2;

	private int selectedIndex = -1;

	private bool IsTween;

	private int whichone;

	private int day;

	private Vector3 fromPos = new Vector3(-295f, 180f, 0f);

	private Vector3 toPos = new Vector3(-295f, 180f, 0f);

	private bool toggleState;
}
