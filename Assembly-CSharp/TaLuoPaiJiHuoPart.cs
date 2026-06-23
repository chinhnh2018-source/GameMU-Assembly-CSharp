using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TaLuoPaiJiHuoPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTexture();
		this.InitPrefabText();
		this.m_JiHuoBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (0f < Global.GetBtnCD(this.m_JiHuoBtn.GetInstanceID()))
			{
				return;
			}
			Global.AddBtnCD(this.m_JiHuoBtn.GetInstanceID(), 0.8f);
			int upNeedGoodsCount = this.GetUpNeedGoodsCount();
			if (!this.m_TaLuoPaiItem.IsActivate)
			{
				if (0 < upNeedGoodsCount)
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.SendTaLuopai(this.m_TaLuoPaiItem.TarotDataAndXml.GetGoodsID(), upNeedGoodsCount);
				}
			}
			else if (0 < upNeedGoodsCount)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SendTaLuopai(this.m_TaLuoPaiItem.TarotDataAndXml.GetGoodsID(), upNeedGoodsCount);
			}
			else
			{
				Super.HintMainText(Global.GetLang("已达到最高级"), 10, 3);
			}
		};
	}

	private int GetUpNeedGoodsID()
	{
		this.InitNeedGoodsData();
		return this.m_UpNeedsGoodsId;
	}

	private int GetUpNeedGoodsCount()
	{
		this.InitNeedGoodsData();
		return this.m_UpNeedsCount;
	}

	private void InitNeedGoodsData()
	{
		if (null != this.m_TaLuoPaiItem)
		{
			string nextLevelNeesGoods = this.m_TaLuoPaiItem.TarotDataAndXml.GetNextLevelNeesGoods();
			if (!string.IsNullOrEmpty(nextLevelNeesGoods))
			{
				string[] array = nextLevelNeesGoods.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					this.m_UpNeedsGoodsId = int.Parse(array[0]);
					this.m_UpNeedsCount = int.Parse(array[1]);
				}
			}
		}
	}

	private void InitPrefabText()
	{
		this.m_Title1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("{0}", Global.GetLang("卡牌属性"))
		});
		this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("{0}", Global.GetLang("激活属性"))
		});
		this.m_Title3.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("{0}", Global.GetLang("激活消耗"))
		});
		this.m_Shuxing1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("{0}：+ {1}"), Global.GetLang(string.Empty), string.Empty)
		});
		this.m_Shuxing2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("{0}：+ {1}"), Global.GetLang(string.Empty), Global.GetLang(string.Empty))
		});
	}

	private void InitTexture()
	{
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

	public void RefreshProperty()
	{
		if (null != this.m_TaLuoPaiItem && this.m_TaLuoPaiItem.TarotDataAndXml != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			int goodsID = this.m_TaLuoPaiItem.TarotDataAndXml.GetGoodsID();
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
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
								if (0 < this.m_TaLuoPaiItem.ExtraLevel)
								{
									if (this.m_TaLuoPaiItem.IsActivate)
									{
										double num = equipProps[key] * 100.0 * (double)(this.Level + this.m_TaLuoPaiItem.ExtraLevel);
										text = text + Global.GetColorStringForNGUIText(new object[]
										{
											"dac7ae",
											string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
										}) + Global.GetColorStringForNGUIText(new object[]
										{
											"17e43e",
											string.Format("{0}%", num)
										}) + Environment.NewLine;
									}
									else
									{
										double num2 = equipProps[key] * 100.0 * (double)(1 + this.m_TaLuoPaiItem.ExtraLevel);
										text = text + Global.GetColorStringForNGUIText(new object[]
										{
											"dac7ae",
											string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
										}) + Global.GetColorStringForNGUIText(new object[]
										{
											"17e43e",
											string.Format("{0}%", num2)
										}) + Environment.NewLine;
									}
								}
								else if (this.m_TaLuoPaiItem.IsActivate)
								{
									double num3 = equipProps[key] * 100.0 * (double)(this.Level + this.m_TaLuoPaiItem.ExtraLevel);
									text += Global.GetColorStringForNGUIText(new object[]
									{
										"dac7ae",
										string.Format(Global.GetLang("{0}：{1}%"), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true), num3) + Environment.NewLine
									});
								}
								else
								{
									double num4 = equipProps[key] * 100.0 * (double)(1 + this.m_TaLuoPaiItem.ExtraLevel);
									text += Global.GetColorStringForNGUIText(new object[]
									{
										"dac7ae",
										string.Format(Global.GetLang("{0}：{1}%"), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true), num4) + Environment.NewLine
									});
								}
								if (this.Level < this.m_TaLuoPaiItem.MaxLevel && this.m_TaLuoPaiItem.IsActivate)
								{
									double num5 = equipProps[key] * 100.0 * 1.0;
									text2 += Global.GetColorStringForNGUIText(new object[]
									{
										"17e43e",
										string.Format(" {0}%", num5) + Environment.NewLine
									});
								}
							}
						}
						else if (equipProps.Length == 177)
						{
							int[] array = Array.ConvertAll<double, int>(goodsXmlNodeByID.EquipProps, (double d) => (int)d);
							if (0 < this.m_TaLuoPaiItem.ExtraLevel)
							{
								if (this.m_TaLuoPaiItem.IsActivate)
								{
									double num6 = (double)(array[key] * (this.Level + this.m_TaLuoPaiItem.ExtraLevel));
									text = text + Global.GetColorStringForNGUIText(new object[]
									{
										"dac7ae",
										string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
									}) + Global.GetColorStringForNGUIText(new object[]
									{
										"17e43e",
										string.Format("{0}", num6)
									}) + Environment.NewLine;
								}
								else
								{
									double num7 = (double)(array[key] * (1 + this.m_TaLuoPaiItem.ExtraLevel));
									text = text + Global.GetColorStringForNGUIText(new object[]
									{
										"dac7ae",
										string.Format(Global.GetLang("{0}："), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true))
									}) + Global.GetColorStringForNGUIText(new object[]
									{
										"17e43e",
										string.Format("{0}", num7)
									}) + Environment.NewLine;
								}
							}
							else if (this.m_TaLuoPaiItem.IsActivate)
							{
								double num8 = (double)(array[key] * this.Level);
								text += Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format(Global.GetLang("{0}：{1}"), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true), num8) + Environment.NewLine
								});
							}
							else
							{
								double num9 = (double)(array[key] * 1);
								text += Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format(Global.GetLang("{0}：{1}"), ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true), num9) + Environment.NewLine
								});
							}
							if (this.Level < this.m_TaLuoPaiItem.MaxLevel && this.m_TaLuoPaiItem.IsActivate)
							{
								double num10 = (double)(array[key] * 1);
								text2 += Global.GetColorStringForNGUIText(new object[]
								{
									"17e43e",
									string.Format(" {0}", num10) + Environment.NewLine
								});
							}
						}
					}
				}
			}
			if (this.Level < this.m_TaLuoPaiItem.MaxLevel && this.m_TaLuoPaiItem.IsActivate)
			{
				NGUITools.SetActive(this.m_Shuxing1Up, true);
				this.m_Shuxing1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					text
				});
				this.m_Shuxing1Up.text = text2;
				NGUITools.SetActive(this.m_Shuxing1Img, true);
				NGUITools.SetActive(this.m_Shuxing2Img, true);
			}
			else
			{
				this.m_Shuxing1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					text
				});
				NGUITools.SetActive(this.m_Shuxing1Up, false);
				NGUITools.SetActive(this.m_Shuxing1Img, false);
				NGUITools.SetActive(this.m_Shuxing2Img, false);
			}
			if (0 < this.m_TaLuoPaiItem.ExtraLevel)
			{
				if (this.m_TaLuoPaiItem.IsActivate)
				{
					this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(" + {0}", this.m_TaLuoPaiItem.ExtraLevel)
					});
				}
				else
				{
					this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						string.Format("{0}", Global.GetLang("激活属性："))
					});
				}
			}
			else if (this.m_TaLuoPaiItem.IsActivate)
			{
				this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
				});
			}
			else
			{
				this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format("{0}", Global.GetLang("激活属性："))
				});
			}
			NGUITools.SetActive(this.m_Shuxing2, false);
			NGUITools.SetActive(this.m_Shuxing2Up, false);
		}
	}

	public void ShowTeXiao(int type)
	{
		string text = "UITeXiao/Perfabs/taluopai/";
		if (type == 0)
		{
			text += "taluopai_shengjie_effect";
		}
		else if (type == 1)
		{
			text += "taluopai_shengji";
			GameObject gameObject = Global.LoadTeXiaoObj("UITeXiao/Perfabs/zhongshenzhengba/zhongshenzhengba_bianhao", this.m_kapaiTransform.transform);
			if (null != gameObject)
			{
				gameObject.SetActive(true);
				Vector3 localPosition = this.m_TaLuoPaiItem.m_LevelLabe.transform.localPosition;
				localPosition.x += this.m_TaLuoPaiItem.m_LevelLabe.transform.localScale.x;
				gameObject.transform.localPosition = localPosition;
				DelayDestroy delayDestroy = gameObject.AddComponent<DelayDestroy>();
				delayDestroy.delayTime = 3.5f;
			}
		}
		GameObject gameObject2 = Global.LoadTeXiaoObj(text, this.m_kapaiTransform.transform);
		if (null != gameObject2)
		{
			Vector3 localPosition2 = gameObject2.transform.localPosition;
			localPosition2.y = -20f;
			gameObject2.transform.localPosition = localPosition2;
			gameObject2.SetActive(true);
			DelayDestroy delayDestroy2 = gameObject2.AddComponent<DelayDestroy>();
			delayDestroy2.delayTime = 3.5f;
		}
	}

	public void SendData(string[] data)
	{
		if (data != null)
		{
			TaLuoPaiError taLuoPaiError = (TaLuoPaiError)int.Parse(data[0]);
			if (taLuoPaiError == TaLuoPaiError.Success)
			{
				if (null != this.m_TaLuoPaiItem)
				{
					if (this.m_TaLuoPaiItem.TarotDataAndXml != null)
					{
						if (this.m_TaLuoPaiItem.TarotDataAndXml.data != null)
						{
							this.m_TaLuoPaiItem.TarotDataAndXml.data.Level = this.m_TaLuoPaiItem.Level + 1;
							this.m_TaLuoPaiItem.TarotDataAndXml.xmlData.Level = this.m_TaLuoPaiItem.Level + 1;
							this.m_TaLuoPaiItem.Level = this.m_TaLuoPaiItem.Level + 1;
							this.ShowTeXiao(1);
							if (this.m_TaLuoPaiItem.Level >= this.m_TaLuoPaiItem.MaxLevel)
							{
								NGUITools.SetActive(this.m_maxTitle, true);
								NGUITools.SetActive(this.m_JiHuoBtn, false);
								NGUITools.SetActive(this.m_Title3, false);
								NGUITools.SetActive(this.m_Icons, false);
								this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
								});
							}
						}
						else
						{
							int goodId = 0;
							if (int.TryParse(data[1], ref goodId))
							{
								TarotCardData tarotCardData = new TarotCardData();
								tarotCardData.GoodId = goodId;
								tarotCardData.Level = 1;
								this.m_TaLuoPaiItem.TarotDataAndXml.data = tarotCardData;
								this.m_TaLuoPaiItem.Level = tarotCardData.Level;
								this.m_TaLuoPaiItem.TarotDataAndXml.data.Level = tarotCardData.Level;
								this.m_TaLuoPaiItem.TarotDataAndXml.data.Level = tarotCardData.Level;
								this.m_TaLuoPaiItem.IsActivate = true;
								this.ShowTeXiao(0);
								if (this.m_TaLuoPaiItem.IsActivate)
								{
									this.m_JiHuoBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
									{
										"fdf7dd",
										string.Format("{0}", Global.GetLang("升级"))
									});
								}
								this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
								{
									"dac7ae",
									string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
								});
								this.m_Title3.text = Global.GetColorStringForNGUIText(new object[]
								{
									"fac60d",
									string.Format("{0}", Global.GetLang("升级消耗"))
								});
							}
						}
						this.m_JiHuoBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							string.Format("{0}", Global.GetLang("升级"))
						});
						this.RefreshGoodsIcon();
						this.Level = this.m_TaLuoPaiItem.TarotDataAndXml.data.Level;
						if (null != this.m_TaLuoPaiItem)
						{
							if (this.m_TaLuoPaiItem.Level == 1)
							{
								Super.HintMainText(Global.GetLang("激活成功"), 10, 3);
							}
							else if (1 <= this.m_TaLuoPaiItem.Level && this.m_TaLuoPaiItem.Level <= this.m_TaLuoPaiItem.MaxLevel)
							{
								Super.HintMainText(Global.GetLang("升级成功"), 10, 3);
							}
						}
					}
					this.m_TaLuoPaiItem.JingYanActive = false;
				}
			}
			else
			{
				this.ErrorLog(taLuoPaiError);
			}
		}
	}

	public void RefreshGoodsIcon()
	{
		if (null != this.m_NeedsGoodsIcon && this.m_NeedsGoodsIcon.ItemCode != this.GetUpNeedGoodsID())
		{
			Object.Destroy(this.m_NeedsGoodsIcon.gameObject);
			this.m_NeedsGoodsIcon = null;
		}
		if (null == this.m_NeedsGoodsIcon)
		{
			this.m_NeedsGoodsIcon = this.initGood(Global.GetEmptyGoodsData(this.GetUpNeedGoodsID(), 1, 1, 0, 1, 1, 1, 1, 1), true);
			this.m_NeedsGoodsIcon.transform.SetParent(this.m_Icons, false);
			this.m_NeedsGoodsIcon.transform.localPosition = new Vector3(0f, 0f, -0.8f);
		}
		if (this.m_TaLuoPaiItem.NowEXP > 9999)
		{
			this.m_NeedsGoodsIcon.SText = string.Format("{1}+/{0}", this.m_TaLuoPaiItem.UPEXP, 9999);
		}
		else
		{
			this.m_NeedsGoodsIcon.SText = string.Format("{1}/{0}", this.m_TaLuoPaiItem.UPEXP, this.m_TaLuoPaiItem.NowEXP);
		}
	}

	public TaLuoPaiItem KaPaiTaLuoPaiItem
	{
		get
		{
			return this.m_TaLuoPaiItem;
		}
		set
		{
			if (null != value)
			{
				GameObject gameObject = U3DUtils.Clone(this.m_kapaiTransform, value.gameObject);
				if (null != gameObject)
				{
					this.m_TaLuoPaiItem = gameObject.GetComponent<TaLuoPaiItem>();
					if (null != this.m_TaLuoPaiItem.GetComponent<BoxCollider>())
					{
						Object.Destroy(this.m_TaLuoPaiItem.GetComponent<BoxCollider>());
					}
					if (null != this.m_TaLuoPaiItem.m_JingYans)
					{
						NGUITools.SetActive(this.m_TaLuoPaiItem.m_JingYans, false);
					}
					this.m_TaLuoPaiItem.Name = value.Name;
					this.m_TaLuoPaiItem.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.m_TaLuoPaiItem.transform.localScale = new Vector3(1f, 1f, 1f);
					this.m_TaLuoPaiItem.ItemId = value.ItemId;
					this.m_TaLuoPaiItem.ItemGoodsId = value.ItemGoodsId;
					this.Level = value.Level;
					this.m_TaLuoPaiItem.UPEXP = value.UPEXP;
					this.m_TaLuoPaiItem.NowEXP = value.NowEXP;
					this.m_TaLuoPaiItem.MaxLevel = value.MaxLevel;
					this.m_TaLuoPaiItem.IsActivate = value.IsActivate;
					this.m_TaLuoPaiItem.m_NameLabel.transform.localScale = new Vector3(22f, 22f, 1f);
					this.m_TaLuoPaiItem.m_LevelLabe.transform.localScale = new Vector3(21f, 21f, 1f);
					if (!this.m_TaLuoPaiItem.IsActivate)
					{
						this.m_JiHuoBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							string.Format("{0}", Global.GetLang("激活"))
						});
						this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							string.Format("{0}", Global.GetLang("激活属性："))
						});
						this.m_Title3.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							string.Format("{0}", Global.GetLang("激活消耗"))
						});
						if (null != this.m_maxTitle)
						{
							NGUITools.SetActive(this.m_maxTitle, false);
						}
						if (value.TarotDataAndXml != null)
						{
							this.m_TaLuoPaiItem.TarotDataAndXml = value.TarotDataAndXml;
							this.RefreshGoodsIcon();
						}
					}
					else if (this.m_TaLuoPaiItem.Level < this.m_TaLuoPaiItem.MaxLevel && this.m_TaLuoPaiItem.MaxLevel != 0)
					{
						this.m_JiHuoBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							string.Format("{0}", Global.GetLang("升级"))
						});
						this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
						});
						this.m_Title3.text = Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							string.Format("{0}", Global.GetLang("升级消耗"))
						});
						if (null != this.m_maxTitle)
						{
							NGUITools.SetActive(this.m_maxTitle, false);
						}
						if (value.TarotDataAndXml != null)
						{
							this.m_TaLuoPaiItem.TarotDataAndXml = value.TarotDataAndXml;
							this.RefreshGoodsIcon();
						}
					}
					else if (null != this.m_maxTitle)
					{
						NGUITools.SetActive(this.m_maxTitle, true);
						NGUITools.SetActive(this.m_JiHuoBtn, false);
						NGUITools.SetActive(this.m_Title3, false);
						NGUITools.SetActive(this.m_Icons, false);
						this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
						});
						if (value.TarotDataAndXml != null)
						{
							this.m_TaLuoPaiItem.TarotDataAndXml = value.TarotDataAndXml;
						}
					}
				}
				this.m_TaLuoPaiItem.ExtraLevel = value.ExtraLevel;
			}
			this.RefreshProperty();
		}
	}

	public int Level
	{
		get
		{
			if (null != this.m_TaLuoPaiItem)
			{
				return this.m_TaLuoPaiItem.Level;
			}
			return 0;
		}
		set
		{
			if (null != this.m_TaLuoPaiItem)
			{
				this.m_TaLuoPaiItem.Level = value;
				this.m_Title2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
				});
				if (this.m_TaLuoPaiItem.ExtraLevel > 0)
				{
					this.m_TaLuoPaiItem.m_LevelLabe.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						string.Format("{0}", Global.GetLang("等级：") + this.m_TaLuoPaiItem.Level)
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format(" + {0}", this.m_TaLuoPaiItem.ExtraLevel)
					});
				}
			}
			if (null != this.m_NeedsGoodsIcon)
			{
				int upNeedGoodsCount = this.GetUpNeedGoodsCount();
				if (upNeedGoodsCount != -1)
				{
					this.m_NeedsGoodsIcon.SecondText.Label.supportEncoding = true;
					if (this.roleCount > 9999)
					{
						this.m_NeedsGoodsIcon.SText = string.Format("{1}+/{0}", upNeedGoodsCount, 9999);
					}
					else
					{
						this.m_NeedsGoodsIcon.SText = string.Format("{1}/{0}", upNeedGoodsCount, (upNeedGoodsCount > this.roleCount) ? Global.GetColorStringForNGUIText(new object[]
						{
							"fffffe",
							this.roleCount
						}) : this.roleCount.ToString());
					}
				}
			}
			this.m_TaLuoPaiItem.JingYanActive = false;
		}
	}

	public Dictionary<int, TarotData> _DicTarotData
	{
		set
		{
			this.Dic_TarotData = value;
		}
	}

	public void ErrorLog(TaLuoPaiError error)
	{
		string chineseText = string.Empty;
		switch (error + 1)
		{
		case TaLuoPaiError.Success:
			chineseText = Global.GetLang("非常规出错");
			break;
		case TaLuoPaiError.Fail:
			chineseText = Global.GetLang("成功");
			break;
		case TaLuoPaiError.MaxLevel:
			chineseText = Global.GetLang("失败");
			break;
		case TaLuoPaiError.NeedPart:
			chineseText = Global.GetLang("已达到最高等级");
			break;
		case TaLuoPaiError.PartSuitIsMax:
			chineseText = Global.GetLang("塔罗牌不足");
			break;
		case TaLuoPaiError.NotOpen:
			chineseText = Global.GetLang("部件已满级");
			break;
		case TaLuoPaiError.PartNumError:
			chineseText = Global.GetLang("功能未开启");
			break;
		case TaLuoPaiError.PosError:
			chineseText = Global.GetLang("碎片使用过多");
			break;
		case TaLuoPaiError.ItemNotEnough:
			chineseText = Global.GetLang("上阵位置有卡牌");
			break;
		case TaLuoPaiError.HasMaxNum:
			chineseText = Global.GetLang("道具不足");
			break;
		}
		Super.HintMainText(Global.GetLang(chineseText), 10, 3);
	}

	public TarotCardData TarotCardData
	{
		get
		{
			return this.mTarotCardData;
		}
		set
		{
			this.mTarotCardData = value;
			if (this.mTarotCardData != null)
			{
				this.roleCount = this.mTarotCardData.TarotMoney;
				if (null != this.m_NeedsGoodsIcon)
				{
					int upNeedGoodsCount = this.GetUpNeedGoodsCount();
					if (upNeedGoodsCount != -1)
					{
						this.m_NeedsGoodsIcon.SecondText.Label.supportEncoding = true;
						if (this.roleCount > 9999)
						{
							this.m_NeedsGoodsIcon.SText = string.Format("{1}+/{0}", upNeedGoodsCount, 9999);
						}
						else
						{
							this.m_NeedsGoodsIcon.SText = string.Format("{1}/{0}", upNeedGoodsCount, (upNeedGoodsCount > this.roleCount) ? Global.GetColorStringForNGUIText(new object[]
							{
								"fffffe",
								this.roleCount
							}) : this.roleCount.ToString());
						}
					}
				}
			}
		}
	}

	public GButton m_CloseBtn;

	public GButton m_JiHuoBtn;

	public UILabel m_Title1;

	public UILabel m_Title2;

	public UILabel m_Title3;

	public UILabel m_Shuxing1;

	public UILabel m_Shuxing1Up;

	public UISprite m_Shuxing1Img;

	public UILabel m_Shuxing2;

	public UILabel m_Shuxing2Up;

	public UISprite m_Shuxing2Img;

	public UISprite m_IconImg;

	public UILabel m_IconCount;

	public TaLuoPaiItem m_TaLuoPaiItem;

	public GameObject m_kapaiTransform;

	public UISprite m_maxTitle;

	public Transform m_Icons;

	private int m_UpNeedsCount = -1;

	private int m_UpNeedsGoodsId = -1;

	private GGoodIcon m_NeedsGoodsIcon;

	private Dictionary<int, TarotData> Dic_TarotData = new Dictionary<int, TarotData>();

	private int roleCount;

	private TarotCardData mTarotCardData;
}
