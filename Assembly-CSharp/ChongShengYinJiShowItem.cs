using System;
using System.Text;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongShengYinJiShowItem : UserControl
{
	public bool IsMaxLevel { get; set; }

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			if (this.ClickHandler != null)
			{
				this.ClickHandler.Invoke(this, this.mData);
			}
		};
	}

	public void InitValue(YinJiData data, int ZhuYinJiType)
	{
		this.mData = data;
		this.IsMaxLevel = this.mData.IsFullLevel;
		this.Level = this.mData.Level;
		this.mImg.URL = ((!data.IsMainYinJi) ? this.ZiImgPath(ZhuYinJiType, this.mData.Type) : this.ZhuImgPath(this.mData.Type));
		if (data.IsMainYinJi)
		{
			this.mImg.transform.localScale = new Vector3(92f, 92f, 1f);
		}
		if (this.mData.Level != this.lastLevel && !this.isFirstOpen)
		{
			if (data.IsMainYinJi)
			{
				this.IsShowZhuEffect = false;
				this.IsShowZhuEffect = true;
			}
			else
			{
				this.IsShowZiEffect = false;
				this.IsShowZiEffect = true;
			}
		}
		this.lastLevel = this.mData.Level;
		this.isFirstOpen = false;
	}

	public int Level
	{
		get
		{
			return this.level;
		}
		set
		{
			if (this.IsMaxLevel)
			{
				this.mLblLevel.Text = "Max";
			}
			else
			{
				this.mLblLevel.Text = "Lv" + value;
			}
		}
	}

	private string ZiImgPath(int zhuType, int ziType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.ImgPath);
		stringBuilder.Append(zhuType);
		stringBuilder.Append("_");
		stringBuilder.Append(ziType);
		stringBuilder.Append(".png.qj");
		return stringBuilder.ToString();
	}

	private string ZhuImgPath(int zhuType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.ImgPath);
		stringBuilder.Append(zhuType);
		stringBuilder.Append(".png.qj");
		return stringBuilder.ToString();
	}

	public bool IsDisable
	{
		set
		{
			this.mImg.Texture.shader = ((!value) ? Shader.Find("Unlit/Gray") : Shader.Find("Unlit/Transparent Colored"));
		}
	}

	private bool IsShowZhuEffect
	{
		set
		{
			this.mZhuEffect.SetActive(value);
		}
	}

	private bool IsShowZiEffect
	{
		set
		{
			this.mZiEffect.SetActive(value);
		}
	}

	public Action<object, YinJiData> ClickHandler;

	public DPSelectedItemEventHandler ChangeHandler;

	public TextBlock mLblLevel;

	public ShowNetImage mImg;

	private YinJiData mData;

	private string ImgPath = "NetImages/GameRes/Images/ChongShengYinJi/";

	public GameObject mZhuEffect;

	public GameObject mZiEffect;

	private int lastLevel;

	private bool isFirstOpen = true;

	private int level;
}
