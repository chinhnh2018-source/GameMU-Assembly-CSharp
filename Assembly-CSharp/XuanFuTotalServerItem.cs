using System;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class XuanFuTotalServerItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public bool ToggleState
	{
		get
		{
			return this._ToggleState;
		}
		set
		{
			if (this._ToggleState != value)
			{
				this._ToggleState = value;
				NGUITools.SetActive(this.BackgroundSprite, this._ToggleState);
				if (this._ToggleState)
				{
					this.m_Title.color = NGUIMath.HexToColorEx(15790320U);
				}
				else
				{
					this.m_Title.color = NGUIMath.HexToColorEx(10323559U);
				}
			}
		}
	}

	public GameObject m_BtnServerInfo;

	public UILabel m_Title;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int StartIndex;

	public int EndIndex;

	public int Index;

	public UISprite BackgroundSprite;

	public SecondLevelServerListData secondLevelListData;

	private bool _ToggleState;
}
