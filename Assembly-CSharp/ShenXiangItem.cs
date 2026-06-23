using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenXiangItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitButtons();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.m_Title.Text = Global.GetLang("神  像");
		if (this.m_ShenXiangDetailsBtn != null && this.m_ShenXiangDetailsBtn.Text != null)
		{
			this.m_ShenXiangDetailsBtn.Text = Global.GetLang("查看属性");
		}
		if (this.staticText != null)
		{
			this.staticText.text = Global.GetLang("需下方神器全部升至满级");
		}
	}

	public void InitButtons()
	{
		this.m_Button1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.openContionIDs.Length > 0)
			{
				int count = int.Parse(this.openContionIDs[0]);
				this.OpenShenQiPropertyPart(1, string.Empty, count, string.Empty);
			}
		};
		this.m_Button2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.openContionIDs.Length > 0)
			{
				int count = int.Parse(this.openContionIDs[1]);
				this.OpenShenQiPropertyPart(1, string.Empty, count, string.Empty);
			}
		};
		this.m_Button3.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.openContionIDs.Length > 0)
			{
				int count = int.Parse(this.openContionIDs[2]);
				this.OpenShenQiPropertyPart(1, string.Empty, count, string.Empty);
			}
		};
		this.m_ShenXiangDetailsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenShenQiPropertyPart(2, this.m_data.Name, 0, this.m_data.ActivationProperty);
		};
	}

	public void InitValue(ShenXiangData data, int shenQiCount = 0)
	{
		this.m_data = data;
		this.m_Title.Text = Global.GetLang(data.Name);
		this.m_Texture.URL = "NetImages/GameRes/Images/ShenQiTexture/" + data.GodIcon + ".png";
		this.openContionIDs = data.OpenCondition.Split(new char[]
		{
			'|'
		});
		this.activationPropertys = data.ActivationProperty;
		this.m_God.color = ((!this.isOpen) ? new Color(0.3f, 0.3f, 0.3f, 1f) : Color.white);
		this.m_ShenXiangDetailsBtn.gameObject.SetActive(this.isOpen);
		this.m_NotActive.SetActive(!this.isOpen);
		this.SetBtnSprite();
		switch (shenQiCount)
		{
		case 0:
			this.SetButtonActivity(this.m_Button1, false);
			this.SetButtonActivity(this.m_Button2, false);
			this.SetButtonActivity(this.m_Button3, false);
			break;
		case 1:
			this.SetButtonActivity(this.m_Button1, true);
			this.SetButtonActivity(this.m_Button2, false);
			this.SetButtonActivity(this.m_Button3, false);
			break;
		case 2:
			this.SetButtonActivity(this.m_Button1, true);
			this.SetButtonActivity(this.m_Button2, true);
			this.SetButtonActivity(this.m_Button3, false);
			break;
		case 3:
			this.SetButtonActivity(this.m_Button1, true);
			this.SetButtonActivity(this.m_Button2, true);
			this.SetButtonActivity(this.m_Button3, true);
			break;
		}
	}

	private void SetButtonActivity(GButton btn, bool isShow)
	{
		btn.target.color = ((!isShow) ? new Color(0.3f, 0.3f, 0.3f, 1f) : Color.white);
		btn.gameObject.GetComponent<BoxCollider>().enabled = isShow;
	}

	private void SetBtnSprite()
	{
		if (this.openContionIDs.Length > 0)
		{
			this.SetSpriteName(this.m_Button1, ShenQiManager.GetShenQiDataByID(int.Parse(this.openContionIDs[0])).ArtifactIcon);
			this.SetSpriteName(this.m_Button2, ShenQiManager.GetShenQiDataByID(int.Parse(this.openContionIDs[1])).ArtifactIcon);
			this.SetSpriteName(this.m_Button3, ShenQiManager.GetShenQiDataByID(int.Parse(this.openContionIDs[2])).ArtifactIcon);
		}
	}

	private void SetSpriteName(GButton btn, string name)
	{
		btn.target.spriteName = name;
		btn.normalSprite = name;
		btn.hoverSprite = name;
		btn.pressedSprite = name;
		btn.disabledSprite = name;
	}

	public void InitValue()
	{
		this.SetActive(this.m_Active, true);
		this.SetActive(this.m_NotActive, false);
	}

	private void SetActive(GameObject ob, bool isOpen)
	{
		if (null != ob)
		{
			ob.SetActive(isOpen);
		}
	}

	protected override void OnDestroy()
	{
		this.activationPropertys = null;
		this.m_Title = null;
		this.m_Texture = null;
		this.m_Button1 = null;
		this.m_Button2 = null;
		this.m_Button3 = null;
		this.m_Active = null;
		this.m_NotActive = null;
	}

	public void OpenShenQiPropertyPart(int type, string titleName, int count, string content)
	{
		if (null == this.shenQiPropertyWindow)
		{
			this.shenQiPropertyWindow = U3DUtils.NEW<GChildWindow>();
			this.shenQiPropertyWindow.ModalType = ChildWindowModalType.Translucent;
			this.shenQiPropertyWindow.IsShowModal = true;
			Super.InitChildWindow(this.shenQiPropertyWindow, Global.GetLang("ShenQiSinglePropertyWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.shenQiPropertyWindow);
		}
		if (null == this.shenQiPropertyPart)
		{
			this.shenQiPropertyPart = U3DUtils.NEW<ShenQiSingleProperty>();
			this.shenQiPropertyPart.Show(type, titleName, count, content);
			this.shenQiPropertyWindow.Body.Add(this.shenQiPropertyPart);
			this.shenQiPropertyPart.CloseCallback = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s.IDType == 0)
				{
					this.CloseShenQiPropertyPart();
				}
			};
		}
	}

	private void CloseShenQiPropertyPart()
	{
		if (null != this.shenQiPropertyWindow)
		{
			Object.Destroy(this.shenQiPropertyPart);
			this.shenQiPropertyPart = null;
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.shenQiPropertyWindow);
			this.shenQiPropertyWindow = null;
		}
	}

	public TextBlock m_Title;

	public ShowNetImage m_Texture;

	public UITexture m_God;

	public GButton m_Button1;

	public GButton m_Button2;

	public GButton m_Button3;

	public GameObject m_Active;

	public GameObject m_NotActive;

	public GButton m_ShenXiangDetailsBtn;

	private string[] openContionIDs;

	private string activationPropertys;

	[HideInInspector]
	public ShenXiangData m_data;

	[HideInInspector]
	public bool isOpen;

	public TextBlock staticText;

	public GChildWindow shenQiPropertyWindow;

	public ShenQiSingleProperty shenQiPropertyPart;
}
