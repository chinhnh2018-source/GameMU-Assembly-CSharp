using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChengJiuSystemPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnLevel.Text = Global.GetLang("成就称号");
		this.m_BtnOverview.Text = Global.GetLang("成就总览");
		this.m_BtnFuWen.Text = Global.GetLang("成就符文");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_BtnLevel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.m_BtnOverview.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
		};
		this.m_BtnFuWen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(3);
		};
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.UpdateChengJiuFuWen();
		this.RefreshChengJiuDianShu();
		this.RefreshStoneDianShu();
		this.InitPartData(1);
		GameInstance.Game.SpriteQueryChengJiuData();
	}

	public void UpdateChengJiuFuWen()
	{
		if (Global.GetChengJiuLevel(0) >= 4)
		{
			if (this.m_BtnFuWen != null)
			{
				this.m_BtnFuWen.gameObject.SetActive(true);
			}
		}
		else
		{
			this.m_BtnFuWen.gameObject.SetActive(false);
		}
	}

	private void InitPartData(int type = 1)
	{
		this.m_BtnLevel.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.m_BtnOverview.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.SetPart(type);
	}

	public void RefreshChengJiuDianShu()
	{
		if (Global.Data.ChengJiuData != null)
		{
			this.m_TextChengJiuDianShu.text = Global.Data.ChengJiuData.ChengJiuPoints.ToString();
			if (this.m_ChengJiuChengHaoPart != null)
			{
				this.m_ChengJiuChengHaoPart.RefreshUI();
			}
		}
		else
		{
			this.m_TextChengJiuDianShu.text = "0";
		}
	}

	public void RefreshStoneDianShu()
	{
		if (Global.Data.ChengJiuData != null)
		{
			this.m_TextStoneDianShu.text = Global.Data.roleData.UserMoney.ToString();
		}
		else
		{
			this.m_TextStoneDianShu.text = "0";
		}
	}

	public void SetPart(int type)
	{
		switch (type)
		{
		case 1:
			this.SetBtnStat(this.m_BtnLevel, true);
			this.SetBtnStat(this.m_BtnOverview, false);
			this.SetBtnStat(this.m_BtnFuWen, false);
			if (this.m_ChengJiuChengHaoPart == null)
			{
				this.m_ChengJiuChengHaoPart = U3DUtils.NEW<ChengJiuChengHaoPart>();
				this.m_ChengJiuChengHaoPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_ChengJiuChengHaoPart.gameObject, true);
			}
			break;
		case 2:
			this.SetBtnStat(this.m_BtnOverview, true);
			this.SetBtnStat(this.m_BtnLevel, false);
			this.SetBtnStat(this.m_BtnFuWen, false);
			if (this.m_ChengJiuOverviewPart == null)
			{
				this.m_ChengJiuOverviewPart = U3DUtils.NEW<ChengJiuOverviewPart>();
				this.m_ChengJiuOverviewPart.InitPartData();
				this.m_ChengJiuOverviewPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.ID == 0)
					{
					}
				};
				this.m_ChengJiuOverviewPart.transform.localPosition = new Vector3(0f, -47f, 0f);
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_ChengJiuOverviewPart.gameObject, true);
			}
			break;
		case 3:
			this.SetBtnStat(this.m_BtnOverview, false);
			this.SetBtnStat(this.m_BtnLevel, false);
			this.SetBtnStat(this.m_BtnFuWen, true);
			if (this.m_ChengJiuFuWenPart == null)
			{
				this.m_ChengJiuFuWenPart = U3DUtils.NEW<ChengJiuFuWen>();
				this.m_ChengJiuFuWenPart.initData();
			}
			this.m_ChengJiuFuWenPart.transform.localPosition = new Vector3(8f, -54f, 0f);
			U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_ChengJiuFuWenPart.gameObject, true);
			break;
		}
		if (this.m_ChengJiuChengHaoPart != null)
		{
			this.m_ChengJiuChengHaoPart.gameObject.SetActive(type == 1);
		}
		if (this.m_ChengJiuOverviewPart != null)
		{
			this.m_ChengJiuOverviewPart.gameObject.SetActive(type == 2);
		}
		if (this.m_ChengJiuFuWenPart != null)
		{
			this.m_ChengJiuFuWenPart.gameObject.SetActive(type == 3);
			int count = this.m_ChengJiuFuWenPart.cjPro.baoji.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.m_ChengJiuFuWenPart.cjPro.baoji[i].activeSelf)
				{
					this.m_ChengJiuFuWenPart.cjPro.baoji[i].SetActive(false);
				}
			}
		}
	}

	private void SetBtnStat(GButton btn, bool selected)
	{
		if (null != btn)
		{
			if (selected)
			{
				btn.Label.color = NGUIMath.HexToColorEx(15790320U);
				btn.Pressed = true;
				btn.Refresh();
			}
			else
			{
				btn.Label.color = NGUIMath.HexToColorEx(10323559U);
				btn.Pressed = false;
				btn.Refresh();
			}
		}
	}

	public GButton m_BtnLevel;

	public GButton m_BtnOverview;

	public GButton m_CloseBtn;

	public GButton m_BtnFuWen;

	public UILabel m_TextChengJiuDianShu;

	public GameObject m_Pnl;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ChengJiuChengHaoPart m_ChengJiuChengHaoPart;

	public ChengJiuOverviewPart m_ChengJiuOverviewPart;

	public ChengJiuFuWen m_ChengJiuFuWenPart;

	public UILabel m_TextStoneDianShu;
}
