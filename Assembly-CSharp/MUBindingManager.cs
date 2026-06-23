using System;
using System.Collections.Generic;
using UnityEngine;

public class MUBindingManager : MonoBehaviour
{
	public static MUBindingManager Instance
	{
		get
		{
			return MUBindingManager.m_instance;
		}
	}

	public MUJoystickController GetJoystickController()
	{
		return this.m_joystickController;
	}

	private void Awake()
	{
		MUBindingManager.m_instance = this;
		this.m_joystickController = base.gameObject.AddComponent<MUJoystickController>();
		this.m_joystickController.beOpen = this.beOpenMockMouse;
		Object.DontDestroyOnLoad(base.gameObject);
		this.m_lstAtivityButtons = new List<MUBindingButton>();
		this.m_lstShowContaiers = new List<MUBindingContainer>();
		this.m_inputMockMouse = new MUInputMockMouse(this.mouse, this.m_joystickController);
		InputMockManager.RegisterMouse(this.m_inputMockMouse);
		this.mouse.UICamera.userMouseMock = this.beOpenMockMouse;
		this.mouse.gameObject.SetActive(this.beOpenMockMouse);
	}

	private void Update()
	{
		if (!this.beOpenMockMouse)
		{
			return;
		}
		for (int i = 1; i <= 13; i++)
		{
			MUControllerButtons mucontrollerButtons = (MUControllerButtons)i;
			if (this.m_joystickController.GetButtonDown(mucontrollerButtons))
			{
				if (mucontrollerButtons == this.nextButtonType)
				{
					this.OnSelectNextButton();
				}
				else if (mucontrollerButtons == this.formerButtonType)
				{
					this.OnSelectFormerButton();
				}
				else if (mucontrollerButtons == this.clickButtonType)
				{
					this.OnExcuteClick();
				}
				this.ProgressButtonDown(mucontrollerButtons);
			}
		}
	}

	private void ProgressButtonDown(MUControllerButtons btnType)
	{
		for (int i = 0; i < this.m_lstShowContaiers.Count; i++)
		{
			this.m_lstShowContaiers[i].ExecuteClick(btnType);
		}
	}

	public void OnEnableChange(MUBindingContainer container, bool beEnable)
	{
		if (beEnable)
		{
			if (this.m_lstShowContaiers.IndexOf(container) < 0)
			{
				this.m_lstShowContaiers.Add(container);
			}
		}
		else if (this.m_lstShowContaiers.IndexOf(container) >= 0)
		{
			this.m_lstShowContaiers.Remove(container);
		}
		this.RefershButtons();
	}

	private void RefershButtons()
	{
		this.m_lstAtivityButtons.Clear();
		for (int i = 0; i < this.m_lstShowContaiers.Count; i++)
		{
			this.m_lstAtivityButtons.AddRange(this.m_lstShowContaiers[i].lstButtons);
		}
		this.ReSortButton();
		if (this.GetNowIndex() < 0 && this.m_lstAtivityButtons.Count > 0)
		{
			this.OnSeletButton(this.m_lstAtivityButtons[0]);
		}
	}

	private void ReSortButton()
	{
		this.m_lstAtivityButtons.Sort((MUBindingButton btn1, MUBindingButton btn2) => btn1.tabIndex.CompareTo(btn2.tabIndex));
	}

	private void OnSelectNextButton()
	{
		if (this.m_lstAtivityButtons.Count < 1)
		{
			return;
		}
		int num = 0;
		int nowIndex = this.GetNowIndex();
		if (nowIndex > -1)
		{
			num = this.GetNextIndex(nowIndex);
		}
		this.OnSeletButton(this.m_lstAtivityButtons[num]);
	}

	private void OnSelectFormerButton()
	{
		if (this.m_lstAtivityButtons.Count < 1)
		{
			return;
		}
		int num = 0;
		int nowIndex = this.GetNowIndex();
		if (nowIndex > -1)
		{
			num = this.GetFormIndex(nowIndex);
		}
		this.OnSeletButton(this.m_lstAtivityButtons[num]);
	}

	private void OnExcuteClick()
	{
		if (this.m_selectButton != null)
		{
			this.m_selectButton.ExecuteClick();
			this.m_selectButton.OnButtonSelect();
		}
	}

	public int GetNowIndex()
	{
		return this.m_lstAtivityButtons.IndexOf(this.m_selectButton);
	}

	public int GetNextIndex(int index)
	{
		int num = index + 1;
		if (num >= this.m_lstAtivityButtons.Count)
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
			num = this.m_lstAtivityButtons.Count - 1;
		}
		return num;
	}

	private void OnSeletButton(MUBindingButton btn)
	{
		if (btn == this.m_selectButton)
		{
			return;
		}
		if (this.m_selectButton != null)
		{
			this.m_selectButton.OnButtonUnSelect();
		}
		if (btn != null)
		{
			btn.OnButtonSelect();
		}
		this.m_selectButton = btn;
	}

	private static MUBindingManager m_instance;

	private MUBindingButton m_selectButton;

	public MUControllerButtons nextButtonType = MUControllerButtons.RIGHTTOP;

	public MUControllerButtons formerButtonType = MUControllerButtons.LEFTTOP;

	public MUControllerButtons clickButtonType = MUControllerButtons.PAUSE;

	public MUMockMouse mouse;

	public bool beOpenMockMouse = true;

	private MUJoystickController m_joystickController;

	private List<MUBindingButton> m_lstAtivityButtons;

	private List<MUBindingContainer> m_lstShowContaiers;

	private MUInputMockMouse m_inputMockMouse;
}
