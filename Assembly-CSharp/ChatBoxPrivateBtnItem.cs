using System;
using UnityEngine;

public class ChatBoxPrivateBtnItem : UserControl
{
	public UIDraggablePanel Draggablepanel
	{
		set
		{
			UIDragPanelContents uidragPanelContents = base.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public PrivateRoleInfo RoleInfo
	{
		get
		{
			return this.m_roleInfo;
		}
		set
		{
			this.m_roleInfo = value;
			string text = string.Empty;
			if (this.m_roleInfo.RoleName.IndexOf('[') > 0 && this.m_roleInfo.RoleName.IndexOf(']') > 0)
			{
				text = this.m_roleInfo.RoleName.Substring(this.m_roleInfo.RoleName.IndexOf(']') + 1);
			}
			else
			{
				text = this.m_roleInfo.RoleName;
			}
			if (text.Length > 3)
			{
				text = text.Substring(0, 3) + "...";
			}
			this.lblName.text = text;
			this.RefershTipNum();
			this.SetSelectState(false);
		}
	}

	public bool BeSelected
	{
		get
		{
			return this.m_beSelected;
		}
		set
		{
			this.m_beSelected = value;
			this.SetSelectState(this.m_beSelected);
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickObj);
	}

	private void OnClickObj(GameObject go)
	{
		if (this.OnSelectItem != null)
		{
			this.OnSelectItem.Invoke(this);
		}
	}

	public void SetSelectState(bool beSelect)
	{
		if (beSelect)
		{
			this.imgBg.spriteName = "chatTab_hover";
			this.lblName.color = Color.white;
		}
		else
		{
			this.imgBg.spriteName = "chatTab_normal";
			this.lblName.color = NGUIMath.HexToColorEx(12301741U);
		}
	}

	public void RefershTipNum()
	{
		this.lblNum.text = string.Empty;
		if (this.m_roleInfo.UnReadMessageNum <= 0)
		{
			this.objNum.SetActive(false);
		}
		else
		{
			this.objNum.SetActive(true);
		}
	}

	public Action<ChatBoxPrivateBtnItem> OnSelectItem;

	public UILabel lblName;

	public UISprite imgBg;

	public UILabel lblNum;

	public GameObject objNum;

	private bool m_beSelected;

	private PrivateRoleInfo m_roleInfo;
}
