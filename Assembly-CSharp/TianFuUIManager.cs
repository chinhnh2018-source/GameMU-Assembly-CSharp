using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TianFuUIManager : UserControl
{
	protected override void InitializeComponent()
	{
	}

	private new void Start()
	{
	}

	private void CloseElseUI()
	{
		if (this._TianFuEWSX.gameObject.activeSelf)
		{
			this._TianFuEWSX.gameObject.SetActive(false);
		}
		if (this._TianFuSkillPro.tft.gameObject.activeSelf)
		{
			this._TianFuSkillPro.tft.gameObject.SetActive(false);
		}
		if (this._TianFuXiDian.gameObject.activeSelf)
		{
			this._TianFuXiDian.gameObject.SetActive(false);
		}
	}

	public void CloseTips()
	{
		this._TianFuSkillPro.tft.CloseTips();
	}

	public void OpenXiDian()
	{
		this.OpenXiDian_();
	}

	public void OpenXiDian_()
	{
		this.CloseTips();
		if (null == this.m_TianFuXiDian)
		{
			Object.Destroy(this.m_TianFuXiDian);
			this.m_TianFuXiDian = null;
		}
		if (null == this.m_TianFuXinDianWind)
		{
			this.m_TianFuXinDianWind = U3DUtils.NEW<GChildWindow>();
			this.m_TianFuXinDianWind.ModalType = ChildWindowModalType.Translucent2;
			Super.InitChildWindow(this.m_TianFuXinDianWind, "TianFuXinDian");
			Super.GData.PlayZoneRoot.Children.Add(this.m_TianFuXinDianWind);
		}
		this.m_TianFuXinDianWind.Visibility = true;
		this.m_TianFuXiDian = U3DUtils.NEW<TianFuXiDian>();
		this.m_TianFuXiDian.InitStr();
		this.m_TianFuXiDian.initIcon();
		this.m_TianFuXiDian.KeXiDianShu = TianFuSystemPart.getUsePoint();
		this.m_TianFuXinDianWind.Body.Add(this.m_TianFuXiDian);
		this.m_TianFuXiDian.Close.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.CloseXiDian_();
		};
	}

	private void CloseXiDian_()
	{
		if (null != this.m_TianFuXiDian)
		{
			Object.Destroy(this.m_TianFuXiDian);
			this.m_TianFuXiDian = null;
		}
		if (null != this.m_TianFuXinDianWind)
		{
			Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, this.m_TianFuXinDianWind);
		}
	}

	public void OpenEWSX()
	{
		if (this._TianFuEWSX != null)
		{
			this.CloseTips();
			if (!this._TianFuEWSX.gameObject.activeSelf)
			{
				this._TianFuEWSX.gameObject.SetActive(true);
				if (Global.Data.roleData.MyTalentData.CountList != null)
				{
					this._TianFuEWSX.SetProPoint(Global.Data.roleData.MyTalentData.CountList[1], Global.Data.roleData.MyTalentData.CountList[2], Global.Data.roleData.MyTalentData.CountList[3]);
				}
				this._TianFuEWSX.SetEWSX_Pro();
			}
		}
	}

	public void OpenSYDS()
	{
		if (this._TianFuSYDS != null)
		{
			this.CloseTips();
			if (!this._TianFuSYDS.gameObject.activeSelf)
			{
				this.CloseElseUI();
				this._TianFuSYDS.gameObject.SetActive(true);
				this._TianFuSYDS.Init();
			}
		}
	}

	public void ZRFullExp(int type)
	{
		this._TianFuSYDS.SetJinDuTiaoTianChong(type);
	}

	public void InitTianFu()
	{
		this._TianFuSkillPro.InitTianFu();
	}

	public void CloseXiDian()
	{
		this.CloseXiDian_();
		this.LianluAnim.gameObject.SetActive(true);
		this.PlayStart(this.LianluAnim, new ActiveAnimation.OnFinished(this.PlayFinished));
	}

	private void PlayStart(Animation anim, ActiveAnimation.OnFinished onFinished)
	{
		ActiveAnimation activeAnimation = ActiveAnimation.Play(anim, 1);
		if (activeAnimation == null)
		{
			return;
		}
		activeAnimation.onFinished = onFinished;
	}

	private void PlayFinished(ActiveAnimation anim)
	{
		anim.gameObject.SetActive(false);
	}

	public TianFuSkillPro _TianFuSkillPro;

	public TianFuXiDian _TianFuXiDian;

	public TianFuEWSX _TianFuEWSX;

	public TianFuSYDS _TianFuSYDS;

	public Animation LianluAnim;

	private TianFuXiDian m_TianFuXiDian;

	private GChildWindow m_TianFuXinDianWind;
}
