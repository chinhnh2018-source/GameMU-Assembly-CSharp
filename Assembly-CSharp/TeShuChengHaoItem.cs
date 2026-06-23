using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeShuChengHaoItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.equipBtn.Label.text = Global.GetLang("佩戴");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public BufferData BufferData
	{
		get
		{
			return this.m_BufferData;
		}
		set
		{
			this.m_BufferData = value;
		}
	}

	public int Index
	{
		get
		{
			return this.m_Index;
		}
		set
		{
			this.m_Index = value;
		}
	}

	public int ID
	{
		get
		{
			return this.m_ID;
		}
		set
		{
			this.m_ID = value;
			this.equipBtn.MouseLeftButtonUp = delegate(object Sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.m_ID,
					IDType = this.m_BufferData.BufferID,
					AllowAutoBuy = this._equipState,
					Index = this.m_Index
				});
			};
		}
	}

	public bool adorned
	{
		get
		{
			return this._equipState;
		}
		set
		{
			this._equipState = value;
			this.SetEquipState(this._equipState);
			this.SetEquipButtonState(this._equipState);
		}
	}

	private void SetEquipState(bool equiped = true)
	{
		if (null != this.equipState)
		{
			this.equipState.SetActive(equiped);
		}
	}

	private void SetEquipButtonState(bool equiped = true)
	{
		string text = (!equiped) ? Global.GetLang("佩戴") : Global.GetLang("卸下");
		this.equipBtn.Text = text;
	}

	public void SetRoleTitle(string titleImgName)
	{
		this.roleTitleImage.URL = "NetImages/GameRes/Images/ChengHaoTeShu/" + titleImgName + ".png";
		this.roleTitleImage.AutoResize = true;
		this.ImgName = titleImgName;
	}

	public void SetProperties(string equipState)
	{
		this.properties.Text = equipState;
		this.m_TitleData = equipState;
	}

	public string ImgName
	{
		get
		{
			return this.m_ImgName;
		}
		set
		{
			this.m_ImgName = value;
		}
	}

	public string TitleData
	{
		get
		{
			return this.m_TitleData;
		}
		set
		{
			this.m_TitleData = value;
		}
	}

	private const string fontColor = "0EFF04";

	private const string netImagePath = "NetImages/GameRes/Images/ChengHaoTeShu/";

	private const string imagePrefix = "SpecialTitle_01";

	private const string imageSuffix = ".png";

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject equipState;

	public ShowNetImage roleTitleImage;

	public TextBlock properties;

	public TextBlock timeLimit;

	public GButton equipBtn;

	private BufferData m_BufferData = new BufferData();

	private bool _equipState;

	private int m_ID = -1;

	private int m_Index = -1;

	private string m_ImgName = string.Empty;

	private string m_TitleData = string.Empty;
}
