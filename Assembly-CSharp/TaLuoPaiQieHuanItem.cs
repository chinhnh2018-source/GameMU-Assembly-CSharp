using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TaLuoPaiQieHuanItem : UserControl
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
		this.m_UseBtn.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			string.Format("{0}", Global.GetLang("使用"))
		});
		this.m_NameLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"b266ff",
			string.Format("{0}", Global.GetLang(string.Empty))
		});
		this.m_Levellabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0}", Global.GetLang(string.Empty))
		});
		this.m_PropertyLabel0.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0}", Global.GetLang(string.Empty))
		});
		this.m_PropertyLabel1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0}", Global.GetLang(string.Empty))
		});
		this.m_PropertyLabel0.pivot = 2;
		this.m_PropertyLabel0.transform.localPosition = new Vector3(130f, 30f, this.m_PropertyLabel0.transform.localPosition.z);
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_UseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(e, new DPSelectedItemEventArgs
				{
					ID = ((this.btnType != 0) ? 11 : 10),
					MyID = this.m_ItemId
				});
			}
		};
	}

	public void Refresh()
	{
		this.btnType = ((this.btnType != 0) ? 0 : 1);
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
		}
	}

	public void SetContent(TarotCardData data)
	{
		string text = string.Empty;
		int goodId = data.GoodId;
		this.m_ItemId = data.GoodId;
		this.m_Level = data.Level;
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodId);
		if (goodsXmlNodeByID != null)
		{
			double[] equipProps = goodsXmlNodeByID.EquipProps;
			for (int i = 0; i < equipProps.Length; i++)
			{
				if (0.0 < equipProps[i])
				{
					if (dictionary.ContainsKey(i))
					{
						dictionary[i] = equipProps[i];
					}
					else
					{
						dictionary.Add(i, equipProps[i]);
					}
				}
			}
			foreach (KeyValuePair<int, double> keyValuePair in dictionary)
			{
				int key = keyValuePair.Key;
				if (key != 0)
				{
					if (ConfigExtPropIndexes.GetPercentByID(key))
					{
						if (equipProps.Length == 177)
						{
							if (0 < this.m_ExtraLevel)
							{
								double num = equipProps[key] * 100.0 * (double)(this.m_Level + this.m_ExtraLevel);
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format("{0}", ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
								}) + Global.GetColorStringForNGUIText(new object[]
								{
									"17e43e",
									string.Format(":{0}%", num)
								}) + Environment.NewLine;
							}
							else
							{
								double num2 = equipProps[key] * 100.0 * (double)(this.m_Level + this.m_ExtraLevel);
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format("{0}", ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
								}) + Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format(":{0}%", num2)
								}) + Environment.NewLine;
							}
						}
					}
					else if (equipProps.Length == 177)
					{
						if (0 < this.m_ExtraLevel)
						{
							int[] array = Array.ConvertAll<double, int>(goodsXmlNodeByID.EquipProps, (double d) => (int)d);
							double num3 = (double)(array[key] * (this.m_Level + this.m_ExtraLevel));
							text = text + Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								string.Format("{0}", ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
							}) + Global.GetColorStringForNGUIText(new object[]
							{
								"17e43e",
								string.Format(":{0}", num3)
							}) + Environment.NewLine;
						}
						else
						{
							int[] array2 = Array.ConvertAll<double, int>(goodsXmlNodeByID.EquipProps, (double d) => (int)d);
							double num4 = (double)(array2[key] * (this.m_Level + this.m_ExtraLevel));
							text = text + Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								string.Format("{0}", ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
							}) + Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								string.Format(":{0}", num4)
							}) + Environment.NewLine;
						}
					}
				}
			}
		}
		this.m_PropertyLabel0.text = text;
		NGUITools.SetActive(this.m_PropertyLabel1, false);
		this.m_NameLabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"b266ff",
			string.Format("{0}", Global.GetLang(this.m_Name))
		});
		if (this.m_ExtraLevel != 0)
		{
			this.m_Levellabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("Lv{0}", this.m_Level) + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format("+{0}", this.m_ExtraLevel)
				})
			});
		}
		else
		{
			this.m_Levellabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("Lv{0}", this.m_Level)
			});
		}
		GGoodIcon ggoodIcon = this.initGood(Global.GetEmptyGoodsData(goodId, 1, 1, 0, 1, 1, 1, 1, 1), true);
		this.DeletePanel(ggoodIcon.gameObject);
		this.m_GoodsRoot.transform.localScale = Vector3.one;
		ggoodIcon.transform.SetParent(this.m_GoodsRoot, false);
		this.btnType = ((data.Postion != 0) ? 1 : 0);
	}

	public void DeletePanel(GameObject obj = null)
	{
		UIPanel component;
		if (null != obj)
		{
			component = obj.GetComponent<UIPanel>();
		}
		else
		{
			component = base.GetComponent<UIPanel>();
		}
		if (null != component)
		{
			Object.Destroy(component);
		}
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			if (BHaveTips)
			{
				ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.ShowGoodsTip(e);
				};
			}
		}
		return ggoodIcon;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
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

	public new string Name
	{
		get
		{
			return this.m_Name;
		}
		set
		{
			this.m_Name = value;
		}
	}

	public int btnType
	{
		get
		{
			return this.m_BtnType;
		}
		set
		{
			this.m_BtnType = value;
			if (value == 0)
			{
				this.m_UseBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format("{0}", Global.GetLang("使用"))
				});
			}
			else
			{
				this.m_UseBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					string.Format("{0}", Global.GetLang("卸下"))
				});
			}
		}
	}

	public GButton m_UseBtn;

	public UILabel m_NameLabel;

	public UILabel m_Levellabel;

	public UILabel m_PropertyLabel0;

	public UILabel m_PropertyLabel1;

	public Transform m_GoodsRoot;

	private int m_Level;

	private int m_ExtraLevel;

	private int m_ItemId;

	private int m_BtnType;

	private string m_Name = string.Empty;

	public DPSelectedItemEventHandler Hander;
}
