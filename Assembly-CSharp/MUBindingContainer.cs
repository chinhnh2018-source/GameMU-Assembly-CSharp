using System;
using System.Collections.Generic;
using UnityEngine;

public class MUBindingContainer : MonoBehaviour
{
	private void Awake()
	{
		this.MUAwake();
	}

	protected virtual void MUAwake()
	{
	}

	private void OnEnable()
	{
		this.m_beEnable = true;
		MUBindingManager.Instance.OnEnableChange(this, true);
		if (this.OnEnableChange != null)
		{
			this.OnEnableChange.Invoke(this, true);
		}
	}

	private void OnDisable()
	{
		this.m_beEnable = false;
		MUBindingManager.Instance.OnEnableChange(this, false);
		if (this.OnEnableChange != null)
		{
			this.OnEnableChange.Invoke(this, false);
		}
	}

	public void ExecuteClick(MUControllerButtons buttonType)
	{
		if (this.m_beEnable && this.isCanDealClick)
		{
			this.DoSpecialButtons(buttonType);
			for (int i = 0; i < this.lstBindingMap.Count; i++)
			{
				if (this.lstBindingMap[i].buttonType == buttonType)
				{
					this.ButtonDoClick(this.lstBindingMap[i]);
				}
			}
		}
	}

	protected virtual void ButtonDoClick(MuBindingMap btnMap)
	{
		btnMap.buttonBinding.ExecuteClick();
	}

	protected virtual void DoSpecialButtons(MUControllerButtons buttonType)
	{
	}

	public List<MUBindingButton> lstButtons;

	[SerializeField]
	private List<MuBindingMap> lstBindingMap;

	public Action<MUBindingContainer, bool> OnEnableChange;

	public bool isCanDealClick = true;

	private bool m_beEnable;
}
