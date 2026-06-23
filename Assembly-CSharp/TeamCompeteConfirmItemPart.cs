using System;
using System.Collections.Generic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteConfirmItemPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitDict();
		this.InitEvent();
		this.IsPrepare = false;
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
	}

	public void InitValue(TianTi5v5PiPeiRoleState data)
	{
		this.mLblName.Text = this.GetFormatName(data.RoleName);
		this.Img.URL = this.GetTouXiangPathByOccu(data.Occupation);
		this.IsPrepare = (data.State == 1);
		this.IsOffline = (data.State == 4);
		if (data.State != 4)
		{
			if (data.State == 2 || data.State == 3)
			{
				this.IsRefuseState = true;
			}
			else
			{
				this.IsRefuseState = false;
			}
		}
	}

	private string GetFormatName(string name)
	{
		string result = name;
		if (name.Length >= 5)
		{
			result = name.Substring(0, 4) + "...";
		}
		return result;
	}

	private void InitDict()
	{
		this.DictTouXiangPath.Add(0, "NetImages/Face/00_0.png.qj");
		this.DictTouXiangPath.Add(1, "NetImages/Face/10_0.png.qj");
		this.DictTouXiangPath.Add(2, "NetImages/Face/20_0.png.qj");
		this.DictTouXiangPath.Add(3, "NetImages/Face/30_0.png.qj");
		this.DictTouXiangPath.Add(5, "NetImages/Face/50_0.png.qj");
	}

	private string GetTouXiangPathByOccu(int occu)
	{
		string empty = string.Empty;
		if (this.DictTouXiangPath.TryGetValue(occu, ref empty))
		{
			return empty;
		}
		MUDebug.LogError<string>(new string[]
		{
			"头像不存在！"
		});
		return empty;
	}

	private bool IsPrepare
	{
		set
		{
			if (this.mSelectObj != null)
			{
				NGUITools.SetActive(this.mSelectObj, value);
			}
		}
	}

	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	public bool HaveRefuse
	{
		get
		{
			return this.haveRefuse;
		}
		set
		{
			this.haveRefuse = value;
		}
	}

	public bool IsRefuseState
	{
		set
		{
			this.HaveRefuse = value;
			if (value)
			{
				this.Img.Texture.shader = Shader.Find("Unlit/Gray");
				this.mLblName.textColor = 8816262U;
			}
			else
			{
				this.Img.Texture.shader = Shader.Find("Unlit/Transparent Colored");
				this.mLblName.textColor = 16513009U;
			}
		}
	}

	public bool IsOffline
	{
		set
		{
			if (value)
			{
				this.Img.Texture.shader = Shader.Find("Unlit/Gray");
				this.mLblName.textColor = 8816262U;
			}
			else
			{
				this.Img.Texture.shader = Shader.Find("Unlit/Transparent Colored");
				this.mLblName.textColor = 14598555U;
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public ShowNetImage Img;

	public GameObject mSelectObj;

	public TextBlock mLblName;

	private Dictionary<int, string> DictTouXiangPath = new Dictionary<int, string>();

	private int id;

	private bool haveRefuse;
}
