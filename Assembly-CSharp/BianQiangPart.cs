using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class BianQiangPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_UpFightingBtn.Text = Global.GetLang("提升战力");
		this.m_TaoZhuangBtn.Text = Global.GetLang("套装展示");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_UpFightingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.m_TaoZhuangBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
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
		try
		{
			this.RefreshUI();
			this.InitPartData(1);
		}
		catch (Exception ex)
		{
		}
	}

	private new void OnDestroy()
	{
	}

	private void InitPartData(int type = 1)
	{
		this.m_UpFightingBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.m_TaoZhuangBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.SetPart(type);
	}

	public void RefreshUI()
	{
		this.m_TextZhanLi.text = Global.Data.roleData.CombatForce.ToString();
	}

	private void SetPart(int type)
	{
		if (type != 1)
		{
			if (type == 2)
			{
				this.SetBtnStat(this.m_UpFightingBtn, false);
				this.SetBtnStat(this.m_TaoZhuangBtn, true);
				if (this.m_TaoZhuangZhanShiPart == null)
				{
					this.m_TaoZhuangZhanShiPart = U3DUtils.NEW<TaoZhuangZhanShiPart>();
					this.m_TaoZhuangZhanShiPart.callback = delegate(object s, DPSelectedItemEventArgs e)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = -1
						});
					};
					U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_TaoZhuangZhanShiPart.gameObject, true);
				}
			}
		}
		else
		{
			this.SetBtnStat(this.m_UpFightingBtn, true);
			this.SetBtnStat(this.m_TaoZhuangBtn, false);
			if (this.m_TiShengZhanLiPart == null)
			{
				this.m_TiShengZhanLiPart = U3DUtils.NEW<TiShengZhanLiPart>();
				this.m_TiShengZhanLiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (this.DPSelectedItem != null)
					{
						this.DPSelectedItem(s, e);
					}
				};
				U3DUtils.AddChild(this.m_Pnl.gameObject, this.m_TiShengZhanLiPart.gameObject, true);
			}
		}
		if (this.m_TiShengZhanLiPart != null)
		{
			this.m_TiShengZhanLiPart.gameObject.SetActive(type == 1);
		}
		if (this.m_TaoZhuangZhanShiPart != null)
		{
			this.m_TaoZhuangZhanShiPart.gameObject.SetActive(type == 2);
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

	public GButton m_UpFightingBtn;

	public GButton m_TaoZhuangBtn;

	public GButton m_CloseBtn;

	public UILabel m_TextZhanLi;

	public GameObject m_Pnl;

	public DPSelectedItemEventHandler DPSelectedItem;

	public TiShengZhanLiPart m_TiShengZhanLiPart;

	public TaoZhuangZhanShiPart m_TaoZhuangZhanShiPart;
}
