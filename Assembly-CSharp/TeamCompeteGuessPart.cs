using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteGuessPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.ClickHandler != null)
			{
				this.ClickHandler(null, null);
			}
		};
	}

	public void InitValue(List<ZhanDuiZhengBaZhanDuiData> dataList)
	{
		for (int i = 0; i < dataList.Count; i++)
		{
			TeamCompeteGuessItem teamCompeteGuessItem = U3DUtils.NEW<TeamCompeteGuessItem>();
			NGUITools.AddChild2(this.Positions[i].gameObject, teamCompeteGuessItem);
			teamCompeteGuessItem.InitValue(dataList[i]);
		}
		if (dataList != null && dataList.Count == 1)
		{
			NGUITools.SetActive(this.rightObj, false);
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public GButton BtnClose;

	public GButton BtnConfirm;

	public Transform[] Positions;

	public GameObject rightObj;
}
