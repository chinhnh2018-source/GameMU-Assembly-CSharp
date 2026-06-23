using System;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class TuJianTypeItem : UserControl
{
	public bool IsCanSubmit
	{
		get
		{
			return this._IsCanSubmit;
		}
		set
		{
			this._IsCanSubmit = value;
			this.HintIcon.gameObject.SetActive(this._IsCanSubmit);
		}
	}

	public string OpenStr
	{
		get
		{
			return this._OpenStr;
		}
		set
		{
			this._OpenStr = value;
		}
	}

	public bool IsOpen
	{
		get
		{
			return this._IsOpen;
		}
		set
		{
			this._IsOpen = value;
			if (this._IsOpen)
			{
				this.m_TypeImageURL.ToGrayBitmap = false;
				this.m_LblInfo.color = Colors.Uint2Color(15366693U);
			}
			else
			{
				this.m_TypeImageURL.ToGrayBitmap = true;
				this.m_LblInfo.color = Colors.Uint2Color(9211020U);
			}
		}
	}

	public string PropsStr
	{
		get
		{
			return this._PropsStr;
		}
		set
		{
			this._PropsStr = value;
		}
	}

	public string strTypeID
	{
		get
		{
			return this.m_strTypeID;
		}
		set
		{
			this.m_strTypeID = value;
			this.m_TypeImageURL.URL = this.GetTypeImageString(value);
		}
	}

	public bool IsActived
	{
		get
		{
			return this._IsActived;
		}
		set
		{
			this._IsActived = value;
			this.m_LblInfo.gameObject.SetActive(!value);
			this.SpriteActived.gameObject.SetActive(value);
		}
	}

	private string GetTypeImageString(string strIconID)
	{
		return string.Format("NetImages/GameRes/Images/TuJian/{0}.png", strIconID);
	}

	public ShowNetImage m_TypeImageURL;

	private string m_strTypeID = string.Empty;

	public UILabel m_LblName;

	public UILabel m_LblInfo;

	public UISprite SpriteActived;

	public GameObject HintIcon;

	private bool _IsCanSubmit;

	private string _OpenStr = string.Empty;

	private bool _IsOpen;

	private string _PropsStr = string.Empty;

	private bool _IsActived;
}
