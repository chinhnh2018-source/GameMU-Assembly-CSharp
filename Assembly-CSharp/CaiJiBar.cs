using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CaiJiBar : UserControl
{
	protected override void InitializeComponent()
	{
	}

	protected IEnumerator CaiJiTimer_Tick()
	{
		for (;;)
		{
			double ticks = (double)Global.GetCorrectLocalTime();
			double subTicks = ticks - this.LastTicks;
			this.LastTicks = ticks;
			this.ElapsedNumTicks += subTicks;
			this.progressBar.Percent = this.ElapsedNumTicks / this.TotalTicks;
			if (this.progressBar.Percent >= 1.0)
			{
				this.StopCaiJi();
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						IDType = 1,
						ID = (int)this.CaijiType
					});
				}
			}
			yield return new WaitForSeconds(0.05f);
		}
		yield break;
	}

	public int CaiJiRoleID
	{
		get
		{
			return this._CaiJiRoleID;
		}
	}

	public void StartCaiJi(int roleID, int value, CaijiTypes type = CaijiTypes.ByLife)
	{
		this.Leader = Global.FindSprite("Leader");
		this.StopCaiJi();
		this._CaiJiRoleID = roleID;
		this.progressBar.Percent = 0.0;
		this.ElapsedNumTicks = 0.0;
		this.CaijiType = type;
		if (type == CaijiTypes.ByLife)
		{
			this.TotalTicks = Math.Max(1.0, (double)value * 500.0);
		}
		else
		{
			this.TotalTicks = Math.Max(1.0, (double)value * 1000.0);
		}
		this.LastTicks = (double)Global.GetCorrectLocalTime();
		base.StartCoroutine("CaiJiTimer_Tick");
	}

	public void StopCaiJi()
	{
		base.StopCoroutine("CaiJiTimer_Tick");
	}

	private new void Update()
	{
		if (base.IsActive && this.Leader != null && this.Leader.Action != GActions.Collect && this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -1,
				ID = (int)this.CaijiType
			});
		}
	}

	public GImgProgressBar progressBar;

	public DPSelectedItemEventHandler DPSelectedItem;

	private double TotalTicks;

	private double ElapsedNumTicks;

	private double LastTicks;

	private CaijiTypes CaijiType = CaijiTypes.None;

	private GSprite Leader;

	private int _CaiJiRoleID;
}
