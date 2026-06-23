using System;
using System.Collections.Generic;

public class MURichBindingContainer : MUBindingContainer
{
	protected override void DoSpecialButtons(MUControllerButtons buttonType)
	{
		base.DoSpecialButtons(buttonType);
		if (buttonType == this.downSelectBype)
		{
			this.OnSelectNextButton();
		}
		else if (buttonType == this.upSelectBype)
		{
			this.OnSelectFormerButton();
		}
	}

	protected override void MUAwake()
	{
		base.MUAwake();
		if (this.lstSpectialButtons.Count == 0)
		{
			return;
		}
		this.ReSortButton();
		this.OnSeletButton(this.lstSpectialButtons[0]);
	}

	public void Reset()
	{
		if (this.lstSpectialButtons.Count > 0)
		{
			this.ReSortButton();
			this.OnSeletButton(this.lstSpectialButtons[0]);
		}
	}

	private void ReSortButton()
	{
		this.lstSpectialButtons.Sort((MUBindingButton btn1, MUBindingButton btn2) => btn1.tabIndex.CompareTo(btn2.tabIndex));
	}

	private void OnSelectNextButton()
	{
		if (this.lstSpectialButtons.Count < 1)
		{
			return;
		}
		int num = 0;
		int nowIndex = this.GetNowIndex();
		if (nowIndex > -1)
		{
			num = this.GetNextIndex(nowIndex);
		}
		this.OnSeletButton(this.lstSpectialButtons[num]);
	}

	private void OnSelectFormerButton()
	{
		if (this.lstSpectialButtons.Count < 1)
		{
			return;
		}
		int num = 0;
		int nowIndex = this.GetNowIndex();
		if (nowIndex > -1)
		{
			num = this.GetFormIndex(nowIndex);
		}
		this.OnSeletButton(this.lstSpectialButtons[num]);
	}

	public int GetNowIndex()
	{
		return this.lstSpectialButtons.IndexOf(this.m_selectButton);
	}

	public int GetNextIndex(int index)
	{
		int num = index + 1;
		if (num >= this.lstSpectialButtons.Count)
		{
			num = 0;
		}
		return num;
	}

	public int GetFormIndex(int index)
	{
		int num = index - 1;
		if (num < 0)
		{
			num = this.lstSpectialButtons.Count - 1;
		}
		return num;
	}

	private void OnSeletButton(MUBindingButton btn)
	{
		if (btn == this.m_selectButton)
		{
			return;
		}
		if (btn == null)
		{
			return;
		}
		this.m_selectButton = btn;
		this.m_selectButton.ExecuteClick();
	}

	public MUControllerButtons downSelectBype = MUControllerButtons.DOWN;

	public MUControllerButtons upSelectBype = MUControllerButtons.UP;

	public List<MUBindingButton> lstSpectialButtons;

	private MUBindingButton m_selectButton;
}
