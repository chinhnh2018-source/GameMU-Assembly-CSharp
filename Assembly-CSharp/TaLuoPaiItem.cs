using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class TaLuoPaiItem : UserControl
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
		this.m_TeXiao = Global.LoadTeXiaoObj("UITeXiao/Perfabs/taluopai/taluopai_lg", this.m_JingYanProgress.transform);
		this.m_TeXiao.transform.localScale = Vector3.one / 0.56f;
		Vector3 localPosition = this.m_TeXiao.transform.localPosition;
		localPosition.x = 75.93f;
		localPosition.y = 2.5f;
		localPosition.z = -0.5f;
		this.m_TeXiao.transform.localPosition = localPosition;
		UIPanel component = this.m_TeXiao.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
	}

	private void InitTexture()
	{
		this.Level = 0;
	}

	private void InitHandler()
	{
	}

	public bool JingYanActive
	{
		set
		{
			if (null != this.m_JingYans)
			{
				this.m_JingYans.SetActive(value);
			}
		}
	}

	public int Pos
	{
		get
		{
			return this.m_Pos;
		}
		set
		{
			this.m_Pos = value;
		}
	}

	public bool SetFlag
	{
		get
		{
			return this.m_SetFlag;
		}
		set
		{
			if (null != this.m_TeXiao)
			{
				if (this.m_NowEXP >= this.m_UPEXP && this.m_UPEXP > 0)
				{
					UISprite component = this.m_JingYanProgress.foreground.GetComponent<UISprite>();
					if (null != component)
					{
						component.spriteName = "ShengjijindutiaoMax";
						component.transform.localScale = new Vector3(176f, 17f, 0f);
						component.transform.localPosition = new Vector3(-0.5f, 0f, -0.5f);
					}
					if (this.m_NowEXP > 9999)
					{
						this.m_EXPProgress = (float)this.m_NowEXP / (float)this.m_UPEXP;
						this.m_JingYanLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							string.Format("{0}+/{1}", 9999, this.m_UPEXP)
						});
					}
					else
					{
						this.m_EXPProgress = (float)this.m_NowEXP / (float)this.m_UPEXP;
						this.m_JingYanLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							string.Format("{0}/{1}", this.m_NowEXP, this.m_UPEXP)
						});
					}
					this.m_JingYanProgress.sliderValue = this.m_EXPProgress;
					this.m_SetFlag = value;
					this.m_TeXiao.SetActive(this.m_SetFlag);
				}
				else
				{
					this.m_SetFlag = false;
					this.m_TeXiao.SetActive(false);
				}
			}
		}
	}

	public bool BPeiDai
	{
		get
		{
			return this.m_PeiDai;
		}
		set
		{
			this.m_PeiDai = value;
			GImgProgressBar[] componentsInChildren = base.transform.GetComponentsInChildren<GImgProgressBar>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (null != componentsInChildren[i])
				{
					componentsInChildren[i].enabled = this.m_PeiDai;
				}
			}
			UIWidget[] componentsInChildren2 = base.transform.GetComponentsInChildren<UIWidget>(true);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (null != componentsInChildren2[j])
				{
					componentsInChildren2[j].enabled = this.m_PeiDai;
				}
			}
		}
	}

	public bool BScale
	{
		get
		{
			return 0.56f == base.transform.localScale.x;
		}
		set
		{
			this.m_LevelLabe.transform.localScale = Vector3.one * 13f / 0.56f;
			this.m_NameLabel.transform.localScale = Vector3.one * 13f / 0.56f;
			this.m_JingYanLabel.transform.localScale = Vector3.one * 11f / 0.56f;
			base.transform.localScale = ((!value) ? Vector3.one : new Vector3(0.56f, 0.56f, 1f));
		}
	}

	public int MaxLevel
	{
		get
		{
			return this.m_MaxLevel;
		}
		set
		{
			this.m_MaxLevel = value;
			if (this.m_Level >= this.m_MaxLevel && this.m_MaxLevel != 0)
			{
				NGUITools.SetActive(this.m_JingYans.gameObject, false);
			}
		}
	}

	public GoodsData GoodsData
	{
		get
		{
			return this.m_GoodsData;
		}
		set
		{
			this.m_GoodsData = value;
		}
	}

	public int Type
	{
		get
		{
			return this.m_Type;
		}
		set
		{
			this.m_Type = value;
		}
	}

	public TarotDataAndXmlData TarotDataAndXml
	{
		get
		{
			return this.m_TarotDataAndXml;
		}
		set
		{
			this.m_TarotDataAndXml = value;
		}
	}

	public int UPEXP
	{
		get
		{
			return this.m_UPEXP;
		}
		set
		{
			this.m_UPEXP = value;
		}
	}

	public int ItemId
	{
		get
		{
			return this.m_ItemId;
		}
		set
		{
			this.m_ItemId = value;
		}
	}

	public int ItemGoodsId
	{
		get
		{
			return this.m_ItemGoodsId;
		}
		set
		{
			this.m_ItemGoodsId = value;
			string text = string.Format("NetImages/GameRes/Images/TaLuoPai/Taluopaichatu{0}.png", this.m_ItemGoodsId);
			if (this.m_IconImage.URL != text)
			{
				this.m_IconImage.enabled = false;
				this.m_IconImage.URL = text;
				this.m_IconImage.enabled = true;
			}
		}
	}

	public int NowEXP
	{
		get
		{
			return this.m_NowEXP;
		}
		set
		{
			this.m_NowEXP = value;
			if (this.m_UPEXP > 0)
			{
				if (this.m_NowEXP < this.m_UPEXP)
				{
					this.m_EXPProgress = 0f;
					this.m_EXPProgress = (float)this.m_NowEXP / (float)this.m_UPEXP;
					this.m_JingYanProgress.sliderValue = this.m_EXPProgress;
					UISprite component = this.m_JingYanProgress.foreground.GetComponent<UISprite>();
					if (null != component)
					{
						component.spriteName = "Shengjijindutiao";
						component.transform.localScale = new Vector3(176f * this.m_EXPProgress, 17f, 0f);
						component.transform.localPosition = new Vector3(-0.5f, 0f, -0.5f);
					}
					this.m_JingYanLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}/{1}", this.m_NowEXP, this.m_UPEXP)
					});
				}
				else
				{
					UISprite component2 = this.m_JingYanProgress.foreground.GetComponent<UISprite>();
					if (null != component2)
					{
						component2.spriteName = "ShengjijindutiaoMax";
						component2.transform.localScale = new Vector3(176f, 17f, 0f);
						component2.transform.localPosition = new Vector3(-0.5f, 0f, -0.5f);
					}
					if (this.m_NowEXP > 9999)
					{
						this.m_EXPProgress = (float)this.m_NowEXP / (float)this.m_UPEXP;
						this.m_JingYanLabel.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							string.Format("{0}/{1}", 9999, this.m_UPEXP)
						});
					}
					this.m_EXPProgress = (float)this.m_NowEXP / (float)this.m_UPEXP;
					this.m_JingYanLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}/{1}", this.m_NowEXP, this.m_UPEXP)
					});
				}
			}
		}
	}

	public int Level
	{
		get
		{
			return this.m_Level;
		}
		set
		{
			this.m_Level = ((0 <= value) ? value : 0);
			string text = "NetImages/GameRes/Images/TaLuoPai/";
			this.m_LevelLabe.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("Lv{0}", this.m_Level)
			});
			if (0 <= this.m_Level && 5 >= this.m_Level)
			{
				text += "Taluopai_bai.png";
			}
			else if (5 < this.m_Level && 15 >= this.m_Level)
			{
				text += "Taluopai_lv.png";
			}
			else if (15 < this.m_Level && 25 >= this.m_Level)
			{
				text += "Taluopai_lan.png";
			}
			else
			{
				text += "Taluopai_zi.png";
			}
			if (this.m_BGImage.URL != text)
			{
				this.m_BGImage.URL = text;
			}
			if (this.m_TarotDataAndXml != null)
			{
				this.m_TarotDataAndXml.xmlData.Level = this.m_Level;
				if (this.m_TarotDataAndXml.data != null)
				{
					this.m_TarotDataAndXml.data.Level = this.m_Level;
				}
			}
			if (this.m_Level >= this.m_MaxLevel && this.m_MaxLevel != 0)
			{
				NGUITools.SetActive(this.m_JingYans.gameObject, false);
			}
			else if (this.m_Level < this.m_MaxLevel && this.m_MaxLevel != 0)
			{
				NGUITools.SetActive(this.m_JingYans.gameObject, true);
			}
		}
	}

	public int ExtraLevel
	{
		get
		{
			return this.m_ExtraLevel;
		}
		set
		{
			this.m_ExtraLevel = value;
			if (0 < this.m_ExtraLevel)
			{
				this.m_LevelLabe.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("Lv{0}", this.m_Level)
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(" +{0}", this.m_ExtraLevel)
				});
			}
			else
			{
				this.m_LevelLabe.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("Lv{0}", this.m_Level)
				});
			}
		}
	}

	public new string Name
	{
		get
		{
			return this.m_Name;
		}
		set
		{
			if (value != null)
			{
				this.m_Name = value;
				this.m_NameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format("{0}", Global.GetLang(this.m_Name))
				});
			}
		}
	}

	public bool IsActivate
	{
		get
		{
			return this.m_IsActivate;
		}
		set
		{
			this.m_IsActivate = value;
			UITexture component = this.m_IconImage.gameObject.GetComponent<UITexture>();
			UITexture component2 = this.m_BGImage.gameObject.GetComponent<UITexture>();
			Shader shader = Shader.Find((!this.m_IsActivate) ? "Unlit/Gray" : "Unlit/Transparent Colored");
			if (null != component)
			{
				component.shader = shader;
			}
			if (null != component2)
			{
				component2.shader = shader;
			}
		}
	}

	public UILabel m_NameLabel;

	public UILabel m_LevelLabe;

	public ShowNetImage m_BGImage;

	public GImgProgressBar m_JingYanProgress;

	public UILabel m_JingYanLabel;

	public ShowNetImage m_IconImage;

	public GameObject m_JingYans;

	public bool m_SetFlag;

	private int m_MaxLevel;

	private int m_ItemId;

	private int m_ItemGoodsId;

	private int m_Level;

	private int m_ExtraLevel;

	private float m_EXPProgress;

	private int m_UPEXP;

	private int m_NowEXP;

	private string m_Name = string.Empty;

	private bool m_IsActivate;

	private TarotDataAndXmlData m_TarotDataAndXml;

	private GoodsData m_GoodsData;

	private bool m_PeiDai;

	private int m_Type;

	private int m_Pos;

	private GameObject m_TeXiao;
}
