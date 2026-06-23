using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class AutoSkillPriorityItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitButtonEvent();
		NGUITools.SetActive(this.m_CloseButton.gameObject, false);
	}

	public void InitIndex(bool isCheck, int _skillIndex, int _skillID)
	{
		this.m_Index = _skillIndex;
		this.leftUp_Label.Text = this.m_Index.ToString();
		if (isCheck)
		{
			this.leftUp_Front.gameObject.SetActive(true);
			this.leftUp_Bg.gameObject.SetActive(false);
			Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				this.leftUp_Label.Text
			});
		}
		else
		{
			this.leftUp_Front.gameObject.SetActive(false);
			this.leftUp_Bg.gameObject.SetActive(true);
			Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.leftUp_Label.Text
			});
		}
	}

	public void InitSkillIcon(int _skillID)
	{
		this.SetSkillID(_skillID);
	}

	public void RefreshSkillIcon(bool isCheck, int _skillIndex, int _skillID)
	{
		this.m_Index = _skillIndex;
		this.leftUp_Label.Text = this.m_Index.ToString();
		if (isCheck)
		{
			this.leftUp_Front.gameObject.SetActive(true);
			this.leftUp_Bg.gameObject.SetActive(false);
			Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				this.leftUp_Label.Text
			});
		}
		else
		{
			this.leftUp_Front.gameObject.SetActive(false);
			this.leftUp_Bg.gameObject.SetActive(true);
			Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				this.leftUp_Label.Text
			});
		}
		this.SetSkillID(_skillID);
	}

	private void InitButtonEvent()
	{
		this.m_ClickButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowSkillSelectPart(this.m_Index, this.m_SkillID);
		};
		this.m_CloseButton.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_SkillID < 0)
			{
				return;
			}
			this.RefreshSkillIcon(0);
			if (this.RefreshSkillList != null)
			{
				if (this.m_Index < 0)
				{
					this.m_Index = 0;
				}
				this.RefreshSkillList(null, new DPSelectedItemEventArgs
				{
					ID = 0,
					Index = this.m_Index
				});
				NGUITools.SetActive(this.m_CloseButton.gameObject, false);
			}
		};
	}

	public void SetSkillID(int _skillId)
	{
		this.m_SkillID = _skillId;
		this.SkillImg.URL = Global.GetSkillIconString(ConfigMagicInfos.GetSkillIconIDByID(_skillId));
		this.SkillImg.transform.localScale = new Vector3(55f, 55f, 0f);
		if (this.m_SkillID > 0)
		{
			this.skillIcon_Bg.gameObject.SetActive(true);
			this.skillIcon_Front.gameObject.SetActive(false);
			NGUITools.SetActive(this.m_CloseButton.gameObject, true);
		}
		else
		{
			this.skillIcon_Bg.gameObject.SetActive(false);
			this.skillIcon_Front.gameObject.SetActive(true);
			NGUITools.SetActive(this.m_CloseButton.gameObject, false);
		}
	}

	private void ShowSkillSelectPart(int index, int skillID)
	{
		GChildWindow window = U3DUtils.NEW<GChildWindow>();
		Super.InitChildWindow(window, "Skill Select");
		this.Container.Children.Add(window);
		window.ModalType = ChildWindowModalType.TransBak;
		SkillSelectPart part = SkillSelectPart.GShow();
		part.isGuaJiModel = true;
		part.isSkillPriorityModel = true;
		window.SetContent(window.BodyPresenter, part, 0.0, 0.0, true);
		part.InitPartData(index - 1, skillID, delegate(object s, DPSelectedItemEventArgs e1)
		{
			Super.CloseChildWindow(this, window);
			if (e1 != null)
			{
				this.RefreshSkillIcon(e1.ID);
				part.isSkillPriorityModel = false;
				if (e1.ID < 0)
				{
					NGUITools.SetActive(this.m_CloseButton.gameObject, false);
				}
				bool flag = e1.ID > 0;
				NGUITools.SetActive(this.m_CloseButton.gameObject, flag);
				if (this.RefreshSkillList != null)
				{
					e1.Index++;
					if (e1.Index < 0)
					{
						e1.Index = 0;
					}
					if (e1.ID < 0)
					{
						e1.ID = 0;
					}
					this.RefreshSkillList(null, new DPSelectedItemEventArgs
					{
						ID = e1.ID,
						Index = e1.Index
					});
				}
			}
		}, false);
	}

	private void RefreshSkillIcon(int callBackSkillID)
	{
		if (callBackSkillID != this.m_SkillID)
		{
			this.SkillImg.URL = Global.GetSkillIconString(ConfigMagicInfos.GetSkillIconIDByID(callBackSkillID));
			this.SkillImg.transform.localScale = new Vector3(60f, 60f, 0f);
			this.skillIcon_Bg.gameObject.SetActive(true);
			this.skillIcon_Front.gameObject.SetActive(false);
		}
	}

	public UISprite leftUp_Bg;

	public UISprite leftUp_Front;

	public TextBlock leftUp_Label;

	public UISprite skillIcon_Bg;

	public UISprite skillIcon_Front;

	public GButton m_ClickButton;

	public GButton m_CloseButton;

	public int m_Index;

	public int m_SkillID;

	public DPSelectedItemEventHandler SkillSelectCallBack;

	public DPSelectedItemEventHandler RefreshSkillList;

	public ShowNetImage SkillImg;
}
