using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PropertyPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	private void InitHandler()
	{
	}

	private void SetContent(string Title, string Content)
	{
		this.m_TitleLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Title
		});
		this.m_Content.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Content
		});
		float num = (float)this.m_Content.ActualHeight * 2f;
		this.m_Cillider.size = new Vector3(340f / this.m_Content.transform.localScale.x, (float)this.m_Content.ActualHeight / this.m_Content.transform.localScale.y, 1f);
		Vector3 center = this.m_Cillider.center;
		center.y = -this.m_Cillider.size.y / 2f;
		this.m_Cillider.center = center;
	}

	private void SetPos(Component obj, Vector3 pos)
	{
		obj.transform.localPosition = pos;
	}

	private void SetScale(Component obj, Vector3 size)
	{
		obj.transform.localScale = size;
	}

	private void ChangeType(int type)
	{
		this.SetPos(this.m_CloseBtn, this.m_ColseBtnPos[type]);
		this.SetPos(this.m_TitleLabel, this.m_TitleLabelPos[type]);
		this.SetPos(this.m_TitleSp, this.m_TitleBgPos[type]);
		this.SetScale(this.m_BgSP, this.m_BgSize[type]);
		this.SetPos(this.m_Content, this.m_ContenLabelpos[type]);
		this.SetPos(this.m_TipsLine, this.m_TipsLinePos[type]);
		Vector4 clipRange = this.m_ContenPanel.clipRange;
		clipRange.z = this.m_contentPanelSize[type].x;
		clipRange.w = this.m_contentPanelSize[type].y;
		this.m_ContenPanel.clipRange = clipRange;
		NGUITools.SetActive(this.m_TitleSp, 0 != type);
		NGUITools.SetActive(this.m_TipsLine, 0 == type);
	}

	public void SetFashionPropertyInf(string Title, string Content)
	{
		this.SetContent(Title, Content);
	}

	public void SetPropertyInf(string Title, string Content)
	{
		this.SetContent(Title, Content);
	}

	public void SetTYpe(int type)
	{
		if (type == 0)
		{
			this.ChangeType(type);
		}
		else if (type == 1)
		{
			this.ChangeType(type);
		}
	}

	public Transform m_TitleSp;

	public Transform m_BgSP;

	public Transform m_TipsLine;

	public GButton m_CloseBtn;

	public UILabel m_TitleLabel;

	public TextBlock m_Content;

	public BoxCollider m_Cillider;

	public UIPanel m_ContenPanel;

	private Vector3[] m_BgSize = new Vector3[]
	{
		new Vector3(388f, 400f, 1f),
		new Vector3(340f, 450f, 1f)
	};

	private Vector3[] m_ColseBtnPos = new Vector3[]
	{
		new Vector3(192f, 194f, -0.5f),
		new Vector3(166.42f, 225f, -0.5f)
	};

	private Vector3[] m_TitleBgPos = new Vector3[]
	{
		new Vector3(0f, 128.38f, -0.01f),
		new Vector3(0f, 205f, -0.01f)
	};

	private Vector3[] m_TitleLabelPos = new Vector3[]
	{
		new Vector3(0f, 174f, -1f),
		new Vector3(0f, 205f, -1f)
	};

	private Vector2[] m_contentPanelSize = new Vector2[]
	{
		new Vector2(330f, 330f),
		new Vector2(330f, 380f)
	};

	private Vector3[] m_ContenLabelpos = new Vector3[]
	{
		new Vector3(-138f, 112f, -1f),
		new Vector3(-138f, 184f, -1f)
	};

	private Vector3[] m_TipsLinePos = new Vector3[]
	{
		new Vector3(0f, 154f, -0.1f),
		default(Vector3)
	};

	public DPSelectedItemEventHandler Hander;
}
