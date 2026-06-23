using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmorPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mShenshenghudunJie = IConfigbase<ConfigShenShengHuJia>.Instance.GetShenshenghudunJieInstance();
		this.mShenshenghudunXing = IConfigbase<ConfigShenShengHuJia>.Instance.GetShenshenghudunXingInstance();
		if (Global.Data.roleData.ArmorData == null)
		{
			Global.Data.roleData.ArmorData = new RoleArmorData
			{
				Armor = 1,
				Exp = 1200
			};
		}
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.mShenshenghudunXing.GetShenshenghudunXingVOByID(Global.Data.roleData.ArmorData.Armor);
		if (shenshenghudunXingVOByID != null)
		{
			this.mFakeArmorStep = shenshenghudunXingVOByID.ArmorupStage;
		}
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.StarOrStep = 0;
		this.AutoUp = 0;
		base.StartCoroutine<bool>(this.InitStar());
	}

	public override void Update()
	{
		base.Update();
		if (!this.WaitingNet && 1.1f <= this.mAutoIntervalTime)
		{
			this.mAutoIntervalTime = 0f;
			if (this.AutoUp == 1)
			{
				this.SendUp();
			}
		}
		this.mAutoIntervalTime += Time.deltaTime;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.laoder.Stop();
		this.laoder = null;
		IConfigbase<ConfigShenShengHuJia>.Instance.SubJieRecommendCount();
		IConfigbase<ConfigShenShengHuJia>.Instance.SubXingRecommendCount();
	}

	private void RefreshStar(byte addTeXiao = 0)
	{
		for (int i = 0; i < this.mStarList.Count; i++)
		{
			string spriteName = this.mStarList[i].spriteName;
			if (i + 1 <= this.Star)
			{
				this.mStarList[i].spriteName = "Star_N";
			}
			else
			{
				this.mStarList[i].spriteName = "Star_D";
			}
			if (addTeXiao == 1 && !spriteName.Equals(this.mStarList[i].spriteName))
			{
				Transform transform = this.mStarList[i].transform.parent.FindChild("Star");
				if (null != transform)
				{
					transform.gameObject.SetActive(false);
					transform.gameObject.SetActive(true);
					DelayDeactivateObj delayDeactivateObj = transform.GetComponent<DelayDeactivateObj>();
					if (null == delayDeactivateObj)
					{
						delayDeactivateObj = transform.gameObject.AddComponent<DelayDeactivateObj>();
					}
					delayDeactivateObj.delayTime = 1.4f;
				}
			}
		}
	}

	private IEnumerator InitStar()
	{
		int Count = this.mShenshenghudunXing.GetMaxStar();
		for (int i = 0; i < Count; i++)
		{
			if (i != 0 && i % 5 == 0)
			{
				yield return null;
			}
			GameObject star = Object.Instantiate<GameObject>(this._StarObj);
			if (null != star)
			{
				float a = -148f - 260f / (float)Count * (float)i;
				star.SetActive(true);
				star.transform.SetParent(this._StarRoot, false);
				star.transform.localPosition = new Vector3(175f * Mathf.Cos(a * 3.14159274f / 180f), 175f * Mathf.Sin(a * 3.14159274f / 180f), -1f);
				star.name = i.ToString();
				UISprite sp = star.transform.FindChild("Star1").GetComponent<UISprite>();
				sp.depth = 4;
				this.mStarList.Add(sp);
			}
		}
		this.RefreshStar(0);
		this.NoticeGetMainDataCallBack(Global.Data.roleData.ArmorData, 0);
		yield break;
	}

	private void SendUp()
	{
		GoodsData[] array;
		int upUseDiamond;
		if (this.mStarOrStep == 0)
		{
			array = this.mShenshenghudunXing.GetNeedGoodsByID(this.mShenshenghudunXing.GetIDByStarAndJie(this.Star, this.Step));
			upUseDiamond = this.mShenshenghudunXing.GetUpUseDiamond(this.Star, this.Step);
		}
		else
		{
			array = this.mShenshenghudunJie.GetNeedGoodsByJie(this.Step);
			upUseDiamond = this.mShenshenghudunJie.GetUpUseDiamond(this.Step);
		}
		if (array != null && 0 < array.Length)
		{
			if (Global.GetRoleGoodsNumberCountByGoodsID(array[0].GoodsID) >= array[0].GCount)
			{
				ArmorUpdateResultData armorUpdateResultData = new ArmorUpdateResultData();
				armorUpdateResultData.Armor = Global.Data.roleData.ArmorData.Armor;
				armorUpdateResultData.Auto = ((this.AutoUp != 0) ? 1 : 0);
				armorUpdateResultData.Exp = Global.Data.roleData.ArmorData.Exp;
				armorUpdateResultData.Type = ((this.mStarOrStep != 0) ? 1 : 0);
				armorUpdateResultData.ZuanShi = 0;
				GameInstance.Game.SendArmorUpLevel(armorUpdateResultData);
				this.WaitingNet = true;
			}
			else if (this._UseDiamond.Check)
			{
				if (Global.Data.roleData.UserMoney >= upUseDiamond)
				{
					ArmorUpdateResultData armorUpdateResultData2 = new ArmorUpdateResultData();
					armorUpdateResultData2.Armor = Global.Data.roleData.ArmorData.Armor;
					armorUpdateResultData2.Auto = ((this.AutoUp != 0) ? 1 : 0);
					armorUpdateResultData2.Exp = Global.Data.roleData.ArmorData.Exp;
					armorUpdateResultData2.Type = ((this.mStarOrStep != 0) ? 1 : 0);
					armorUpdateResultData2.ZuanShi = upUseDiamond;
					GameInstance.Game.SendArmorUpLevel(armorUpdateResultData2);
					this.WaitingNet = true;
				}
				else
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
					this.AutoUp = 0;
				}
			}
			else
			{
				Super.ShowGoodsGuideForGoodsTips(array[0].GoodsID, null);
				this.AutoUp = 0;
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("( ⊙ o ⊙ )！配置出错"), 10, 3);
		}
	}

	private void RefreshUpBtnText()
	{
		if (this.StarOrStep == 0)
		{
			this._UpBtns[0].Label.text = Global.GetLang("升星");
			if (this.AutoUp == 0)
			{
				this._UpBtns[1].Label.text = Global.GetLang("自动升星");
			}
			else
			{
				this._UpBtns[1].Label.text = Global.GetLang("取消自动");
			}
		}
		else
		{
			this._UpBtns[0].Label.text = Global.GetLang("升阶");
			if (this.AutoUp == 0)
			{
				this._UpBtns[1].Label.text = Global.GetLang("自动升阶");
			}
			else
			{
				this._UpBtns[1].Label.text = Global.GetLang("取消自动");
			}
		}
	}

	private void InitPrefabText()
	{
		try
		{
			this.RefreshUpBtnText();
			this._StarLabel[0].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("永久属性")
			});
			this._HuDunLabel[0].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("护盾属性")
			});
			this._HuDunLabel[1].Text = string.Empty;
			this._StarLabel[1].Text = string.Empty;
			this._StarLabel[2].Text = string.Empty;
			this._StarLabel[2].Pivot = 0;
			this._StarLabel[2].X = 183.0;
			this._JieLable.pivot = 4;
			this._JieLable.transform.localPosition = new Vector3(0f, this._JieLable.transform.localPosition.y, this._JieLable.transform.localPosition.z);
			this._UseDiamond.Check = false;
			this.RefreshCostdiamond();
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void RefreshCostdiamond()
	{
		this._UseDiamond._Lable.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("道具不足时，自动消耗")
		}) + "        " + ((this.mStarOrStep != 0) ? this.mShenshenghudunJie.GetUpUseDiamond(this.Step) : this.mShenshenghudunXing.GetUpUseDiamond(this.Star, this.Step));
	}

	private void InitTexture()
	{
		try
		{
			this._BakModalImage0.URL = "NetImages/GameRes/Images/Armor/ModalShowBak2.png";
			this.ZuanSpr.transform.localPosition = new Vector3(300f, this.ZuanSpr.transform.localPosition.y, this.ZuanSpr.transform.localPosition.z);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this._UpBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._UpBtns[0].GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._UpBtns[0].GetInstanceID(), 1.1f);
				this.AutoUp = 0;
				this.SendUp();
			};
			this._UpBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._UpBtns[1].GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._UpBtns[1].GetInstanceID(), 1.1f);
				if (this.AutoUp == 0)
				{
					this.AutoUp = 1;
				}
				else
				{
					this.AutoUp = 0;
				}
			};
			this._JianTouBtns[0].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._JianTouBtns[0].GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._JianTouBtns[0].GetInstanceID(), 0.5f);
				this.RefrehFakeStep(--this.mFakeArmorStep * 11);
			};
			this._JianTouBtns[1].MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._JianTouBtns[1].GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._JianTouBtns[1].GetInstanceID(), 0.5f);
				if (this.mIsFakeData == 0)
				{
					this.RefrehFakeStep(-10 + ++this.mFakeArmorStep * 11);
				}
				else
				{
					this.RefrehFakeStep(++this.mFakeArmorStep * 11);
				}
			};
			this._BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this._UseDiamond.CheckChanged = delegate(object e, BaseEventArgs s)
			{
				if (this._UseDiamond.Check && Global.GetZuanShi(ZuanShiPartClass.RidePetChouQu))
				{
					if (this.messageBoxWindow != null)
					{
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
					}
					this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("需要消耗钻石，确定吗？")
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
					}
					this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
						if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							Global.SetZuanShi(ZuanShiPartClass.RidePetChouQu, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
						}
						Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						if (messageBoxReturn != 0)
						{
							this._UseDiamond.Check = false;
						}
						return true;
					};
					return;
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void RefreshUpNeedsGoods()
	{
		this._GoodsIcon.BackgroundSprite0.gameObject.SetActive(true);
		this._GoodsIcon.SecondText.Label.supportEncoding = true;
		this._GoodsIcon.GoodImg.SetTexture(null);
		this._GoodsIcon.SText = " ";
		if (this.StarOrStep == 0)
		{
			GoodsData[] needGoodsByID = this.mShenshenghudunXing.GetNeedGoodsByID(this.mShenshenghudunXing.GetIDByStarAndJie(this.Star, this.Step));
			if (needGoodsByID != null && 0 < needGoodsByID.Length)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(needGoodsByID[0].GoodsID);
				if (goodsXmlNodeByID != null)
				{
					this._GoodsIcon.GoodImg.URL = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
					this._GoodsIcon.Width = 78.0;
					this._GoodsIcon.Height = 78.0;
					this._GoodsIcon.BackSpriteName0 = "bagGrid4_bak";
					this._GoodsIcon.TipType = 1;
					this._GoodsIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
					this._GoodsIcon.ItemCode = needGoodsByID[0].GoodsID;
					this._GoodsIcon.ItemObject = needGoodsByID[0];
					this._GoodsIcon.BoxTypes = -1;
					int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(needGoodsByID[0].GoodsID);
					this._GoodsIcon.SText = ((roleGoodsNumberCountByGoodsID < needGoodsByID[0].GCount) ? Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						roleGoodsNumberCountByGoodsID + "/" + needGoodsByID[0].GCount
					}) : Global.GetColorStringForNGUIText(new object[]
					{
						"fffff0",
						roleGoodsNumberCountByGoodsID.ToString() + "/" + needGoodsByID[0].GCount
					}));
					this._GoodsIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
					{
						GTipServiceEx.ShowTip(this._GoodsIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, this._GoodsIcon.ItemObject as GoodsData);
					};
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						Global.GetLang("策划配的物品客户端没有")
					});
				}
			}
		}
		else
		{
			GoodsData[] needGoodsByJie = this.mShenshenghudunJie.GetNeedGoodsByJie(this.Step);
			if (needGoodsByJie != null && 0 < needGoodsByJie.Length)
			{
				if (0 >= needGoodsByJie[0].GCount)
				{
					this._GoodsIcon.GoodImg.SetTexture(null);
					this._GoodsIcon.SText = "0/0";
				}
				else
				{
					GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(needGoodsByJie[0].GoodsID);
					if (goodsXmlNodeByID2 != null)
					{
						this._GoodsIcon.GoodImg.URL = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID2), "NetImages/GameRes/");
						this._GoodsIcon.Width = 78.0;
						this._GoodsIcon.Height = 78.0;
						this._GoodsIcon.BackSpriteName0 = "bagGrid4_bak";
						this._GoodsIcon.TipType = 1;
						this._GoodsIcon.ItemCategory = goodsXmlNodeByID2.Categoriy;
						this._GoodsIcon.ItemCode = needGoodsByJie[0].GoodsID;
						this._GoodsIcon.ItemObject = needGoodsByJie[0];
						this._GoodsIcon.BoxTypes = -1;
						int roleGoodsNumberCountByGoodsID2 = Global.GetRoleGoodsNumberCountByGoodsID(needGoodsByJie[0].GoodsID);
						this._GoodsIcon.SText = ((roleGoodsNumberCountByGoodsID2 < needGoodsByJie[0].GCount) ? Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							roleGoodsNumberCountByGoodsID2 + "/" + needGoodsByJie[0].GCount
						}) : Global.GetColorStringForNGUIText(new object[]
						{
							"fffff0",
							roleGoodsNumberCountByGoodsID2.ToString() + "/" + needGoodsByJie[0].GCount
						}));
						this._GoodsIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
						{
							GTipServiceEx.ShowTip(this._GoodsIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, this._GoodsIcon.ItemObject as GoodsData);
						};
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							Global.GetLang("策划配的物品客户端没有")
						});
					}
				}
			}
		}
		this.RefreshCostdiamond();
	}

	private string GetJieAttStr(int Jie, byte Gray)
	{
		StringBuilder stringBuilder = new StringBuilder();
		ShenshenghudunJieVO shenshenghudunXingVOByJie = this.mShenshenghudunJie.GetShenshenghudunXingVOByJie(Jie);
		if (shenshenghudunXingVOByJie != null)
		{
			if (0f < shenshenghudunXingVOByJie.Damageabsorption)
			{
				if (Gray == 0)
				{
					stringBuilder.AppendLine(Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord("ArmorPercen", false) + Global.GetLang("：")
					}) + ((!ConfigExtPropIndexes.GetPercentByWord("ArmorPercen")) ? shenshenghudunXingVOByJie.Damageabsorption.ToString("f0") : ((shenshenghudunXingVOByJie.Damageabsorption * 100f).ToString("f0") + "%")));
				}
				else
				{
					stringBuilder.AppendLine(ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord("ArmorPercen", false) + Global.GetLang("：") + ((!ConfigExtPropIndexes.GetPercentByWord("ArmorPercen")) ? shenshenghudunXingVOByJie.Damageabsorption.ToString("f0") : ((shenshenghudunXingVOByJie.Damageabsorption * 100f).ToString("f0") + "%")));
				}
			}
			if (!string.IsNullOrEmpty(shenshenghudunXingVOByJie.HuDunHuiFuTips))
			{
				if (Gray == 0)
				{
					stringBuilder.AppendLine(Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord("ArmorRecover", false) + Global.GetLang("：")
					}) + shenshenghudunXingVOByJie.HuDunHuiFuTips + (shenshenghudunXingVOByJie.Armorrecovery * 100f).ToString("f0") + "%");
				}
				else
				{
					stringBuilder.AppendLine(string.Concat(new string[]
					{
						ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord("ArmorRecover", false),
						Global.GetLang("："),
						shenshenghudunXingVOByJie.HuDunHuiFuTips,
						(shenshenghudunXingVOByJie.Armorrecovery * 100f).ToString("f0"),
						"%"
					}));
				}
			}
		}
		string text = stringBuilder.ToString();
		if (!string.IsNullOrEmpty(text) && Gray == 1)
		{
			return Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				text
			});
		}
		return text;
	}

	private string GetStarAttStr(int Star, int Jie)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int maxStar = this.mShenshenghudunXing.GetMaxStar();
		Dictionary<string, double> attByStarAndJie = this.mShenshenghudunXing.GetAttByStarAndJie(Star, Jie);
		int num = Star;
		if (this.mIsFakeData == 1)
		{
			num = maxStar;
		}
		Dictionary<string, double> dictionary = (num < maxStar) ? this.mShenshenghudunXing.GetAttByStarAndJie(Star + 1, Jie) : new Dictionary<string, double>();
		if (attByStarAndJie != null && 0 < attByStarAndJie.Count)
		{
			foreach (KeyValuePair<string, double> keyValuePair in attByStarAndJie)
			{
				if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair.Key))
				{
					Dictionary<string, double> dictionary2 = dictionary;
					Dictionary<string, double>.Enumerator enumerator;
					KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
					if (dictionary2.ContainsKey(keyValuePair2.Key))
					{
						StringBuilder stringBuilder2 = stringBuilder;
						object[] array = new object[2];
						array[0] = "e3b36c";
						int num2 = 1;
						KeyValuePair<string, double> keyValuePair3 = enumerator.Current;
						array[num2] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair3.Key, false) + Global.GetLang("：");
						string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
						KeyValuePair<string, double> keyValuePair4 = enumerator.Current;
						stringBuilder2.Append(colorStringForNGUIText + (keyValuePair4.Value * 100.0).ToString("f0") + "%");
						StringBuilder stringBuilder3 = stringBuilder;
						string text = "@";
						object[] array2 = new object[2];
						array2[0] = "17e43e";
						int num3 = 1;
						Dictionary<string, double> dictionary3 = dictionary;
						KeyValuePair<string, double> keyValuePair5 = enumerator.Current;
						double num4 = dictionary3[keyValuePair5.Key];
						KeyValuePair<string, double> keyValuePair6 = enumerator.Current;
						array2[num3] = ((num4 - keyValuePair6.Value) * 100.0).ToString("f0") + "%";
						stringBuilder3.AppendLine(text + Global.GetColorStringForNGUIText(array2));
					}
					else
					{
						StringBuilder stringBuilder4 = stringBuilder;
						object[] array3 = new object[2];
						array3[0] = "e3b36c";
						int num5 = 1;
						KeyValuePair<string, double> keyValuePair7 = enumerator.Current;
						array3[num5] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair7.Key, false) + Global.GetLang("：");
						string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(array3);
						KeyValuePair<string, double> keyValuePair8 = enumerator.Current;
						stringBuilder4.AppendLine(colorStringForNGUIText2 + (keyValuePair8.Value * 100.0).ToString("f0") + "%");
					}
				}
				else
				{
					Dictionary<string, double> dictionary4 = dictionary;
					Dictionary<string, double>.Enumerator enumerator;
					KeyValuePair<string, double> keyValuePair9 = enumerator.Current;
					if (dictionary4.ContainsKey(keyValuePair9.Key))
					{
						StringBuilder stringBuilder5 = stringBuilder;
						object[] array4 = new object[2];
						array4[0] = "e3b36c";
						int num6 = 1;
						KeyValuePair<string, double> keyValuePair10 = enumerator.Current;
						array4[num6] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair10.Key, false) + Global.GetLang("：");
						string colorStringForNGUIText3 = Global.GetColorStringForNGUIText(array4);
						KeyValuePair<string, double> keyValuePair11 = enumerator.Current;
						stringBuilder5.Append(colorStringForNGUIText3 + (keyValuePair11.Value * 1.0).ToString("f0"));
						StringBuilder stringBuilder6 = stringBuilder;
						string text2 = "@";
						object[] array5 = new object[2];
						array5[0] = "17e43e";
						int num7 = 1;
						Dictionary<string, double> dictionary5 = dictionary;
						KeyValuePair<string, double> keyValuePair12 = enumerator.Current;
						double num8 = dictionary5[keyValuePair12.Key];
						KeyValuePair<string, double> keyValuePair13 = enumerator.Current;
						array5[num7] = ((num8 - keyValuePair13.Value) * 1.0).ToString("f0");
						stringBuilder6.AppendLine(text2 + Global.GetColorStringForNGUIText(array5));
					}
					else
					{
						StringBuilder stringBuilder7 = stringBuilder;
						object[] array6 = new object[2];
						array6[0] = "e3b36c";
						int num9 = 1;
						KeyValuePair<string, double> keyValuePair14 = enumerator.Current;
						array6[num9] = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair14.Key, false) + Global.GetLang("：");
						string colorStringForNGUIText4 = Global.GetColorStringForNGUIText(array6);
						KeyValuePair<string, double> keyValuePair15 = enumerator.Current;
						stringBuilder7.AppendLine(colorStringForNGUIText4 + (keyValuePair15.Value * 1.0).ToString("f0"));
					}
				}
			}
		}
		return stringBuilder.ToString();
	}

	private void RefreshModal()
	{
		if (this.laoder != null)
		{
			this.laoder.Stop();
			this.laoder = null;
		}
		this._Modal.Clear();
		this.laoder = UIHelper.LoadModelResource(this._Modal, this.mShenshenghudunJie.GetModelID(this.Step), 1f, delegate(object e, DPSelectedItemEventArgs s)
		{
			if (null != this._Modal._Target)
			{
				UIHelper.SetReanderQueue(this._Modal._Target);
			}
		});
	}

	private void RefreshEXP()
	{
		if (this.StarOrStep == 0)
		{
			this._StepGImgProgressBar.gameObject.SetActive(false);
			this._BakModalImage0.URL = "NetImages/GameRes/Images/Armor/ModalShowBak2.png";
			int upNeedEXP = this.mShenshenghudunXing.GetUpNeedEXP(this.Star, this.Step);
			float num = (float)this.StarExp / (float)upNeedEXP;
			this._ProgressSp.fillAmount = 0.07f + num * 0.86f;
		}
		else
		{
			if (this.Step >= this.mShenshenghudunJie.GetMaxkJie())
			{
				this._StepGImgProgressBar.gameObject.SetActive(false);
			}
			else
			{
				this._StepGImgProgressBar.sliderValue = (float)this.StepExp / (float)this.mShenshenghudunJie.GetEXP(this.Step);
				this._StepGImgProgressBar.gameObject.SetActive(true);
			}
			this._ProgressSp.fillAmount = 0f;
			this._BakModalImage0.URL = "NetImages/GameRes/Images/Armor/ModalShowBak1.png";
		}
	}

	private void RefeshAtt()
	{
		byte b = 0;
		while ((int)b < this._UpJianTou.Length)
		{
			this._UpJianTou[(int)b].enabled = false;
			b += 1;
		}
		string starAttStr = this.GetStarAttStr(this.Star, this.Step);
		string text = string.Empty;
		string text2 = string.Empty;
		string[] array = starAttStr.Split(new char[]
		{
			'\n'
		});
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				string[] array2 = array[i].Split(new char[]
				{
					'@'
				});
				text = text + array2[0].Replace("@", " ") + "\n";
				if (array2.Length == 2)
				{
					text2 = text2 + array2[1] + "\n";
					if (this._UpJianTou.Length > i)
					{
						this._UpJianTou[i].enabled = true;
					}
					this._UpJianTou[i].transform.localPosition = new Vector3(175f, this._UpJianTou[i].transform.localPosition.y, this._UpJianTou[i].transform.localPosition.z);
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			this._StarLabel[1].Text = text;
			this._StarLabel[2].Text = text2;
		}
		string text3 = this.GetJieAttStr(this.Step, 0) + "\n\n\n" + ((this.mShenshenghudunJie.GetMaxkJie() > this.Step) ? (Global.GetColorStringForNGUIText(new object[]
		{
			"808081",
			Global.GetLang("下一阶段")
		}) + "\n" + this.GetJieAttStr(this.Step + 1, 1)) : string.Empty);
		if (!string.IsNullOrEmpty(text3))
		{
			this._HuDunLabel[1].Text = text3;
		}
	}

	private void RefrehFakeStep(int AromrID)
	{
		this.AutoUp = 0;
		this.mIsFakeData = 1;
		RoleArmorData roleArmorData = new RoleArmorData();
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.mShenshenghudunXing.GetShenshenghudunXingVOByID(AromrID);
		if (shenshenghudunXingVOByID != null)
		{
			roleArmorData.Armor = shenshenghudunXingVOByID.ID;
			ShenshenghudunXingVO shenshenghudunXingVOByID2 = this.mShenshenghudunXing.GetShenshenghudunXingVOByID(Global.Data.roleData.ArmorData.Armor);
			if (shenshenghudunXingVOByID2 != null && shenshenghudunXingVOByID.ArmorupStage <= shenshenghudunXingVOByID2.ArmorupStage)
			{
				if (shenshenghudunXingVOByID.ArmorupStage < shenshenghudunXingVOByID2.ArmorupStage)
				{
					this.mIsFakeData = 2;
					roleArmorData.Armor = shenshenghudunXingVOByID.ID;
					roleArmorData.Exp = shenshenghudunXingVOByID2.StarExp;
				}
				else
				{
					this.mIsFakeData = 0;
					roleArmorData.Armor = Global.Data.roleData.ArmorData.Armor;
					roleArmorData.Exp = Global.Data.roleData.ArmorData.Exp;
				}
				this._JianTouBtns[1].isEnabled = true;
				this._JianTouBtns[0].isEnabled = true;
			}
			if (this.mIsFakeData == 1)
			{
				if (shenshenghudunXingVOByID.ArmorupStage < this.Step)
				{
					this._JianTouBtns[1].isEnabled = true;
					this._JianTouBtns[0].isEnabled = false;
				}
				else if (shenshenghudunXingVOByID.ArmorupStage > this.Step)
				{
					this._JianTouBtns[1].isEnabled = false;
					this._JianTouBtns[0].isEnabled = true;
				}
			}
			this.NoticeGetMainDataCallBack(roleArmorData, 0);
		}
	}

	private void RefreshBtns()
	{
		if (this.mIsFakeData == 1)
		{
			this._NotOpenObj.gameObject.SetActive(true);
			this._UseDiamond.gameObject.SetActive(false);
			this._UpBtns[0].isEnabled = false;
			this._UpBtns[1].isEnabled = false;
		}
		else if (this.mIsFakeData == 2)
		{
			this._NotOpenObj.gameObject.SetActive(false);
			this._UseDiamond.gameObject.SetActive(true);
			this._UpBtns[0].isEnabled = false;
			this._UpBtns[1].isEnabled = false;
		}
		else
		{
			this._NotOpenObj.gameObject.SetActive(false);
			this._UseDiamond.gameObject.SetActive(true);
			if (this.Step == 1)
			{
				this._JianTouBtns[0].isEnabled = false;
				this._JianTouBtns[1].isEnabled = true;
			}
			else if (this.mShenshenghudunJie.GetMaxkJie() == this.Step)
			{
				this._JianTouBtns[0].isEnabled = true;
				this._JianTouBtns[1].isEnabled = false;
			}
			this._UpBtns[0].isEnabled = true;
			this._UpBtns[1].isEnabled = true;
		}
		if (this.Step >= this.mShenshenghudunJie.GetMaxkJie() && this.Star == this.mShenshenghudunXing.GetMaxStar())
		{
			this._NotOpenObj.gameObject.SetActive(true);
			this._UseDiamond.gameObject.SetActive(false);
			this._NotOpenObj.spriteName = "MaXLevel";
			this._NotOpenObj.transform.localScale = new Vector3(118f, 34f, 0f);
			this._UpBtns[0].isEnabled = false;
			this._UpBtns[1].isEnabled = false;
		}
		else if (1 >= this.Step)
		{
			this._JianTouBtns[0].isEnabled = false;
			this._JianTouBtns[1].isEnabled = true;
			this._NotOpenObj.spriteName = "Fake";
			this._NotOpenObj.transform.localScale = new Vector3(76f, 34f, 0f);
		}
		else
		{
			this._NotOpenObj.spriteName = "Fake";
			this._NotOpenObj.transform.localScale = new Vector3(76f, 34f, 0f);
		}
	}

	private void RefreshStep()
	{
		this._JieLable.text = this.Step.ToString();
	}

	private void AddTeXiao(byte type)
	{
		try
		{
			for (int i = 0; i < this._TeXiao.Length; i++)
			{
				this._TeXiao[i].SetActive(false);
			}
			this._TeXiao[(int)type].SetActive(true);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	internal void NoticeUpCallBack(ArmorUpdateResultData data)
	{
		int star = this.Star;
		int step = this.Step;
		if (data.Result == 0)
		{
			int num = data.Exp - Global.Data.roleData.ArmorData.Exp;
			Global.Data.roleData.ArmorData.Armor = data.Armor;
			Global.Data.roleData.ArmorData.Exp = data.Exp;
			byte upLevel = 0;
			if (data.Type == 0)
			{
				ShenshenghudunXingVO shenshenghudunXingVOByID = this.mShenshenghudunXing.GetShenshenghudunXingVOByID(data.Armor);
				if (shenshenghudunXingVOByID != null && shenshenghudunXingVOByID.StarLevel > star)
				{
					upLevel = 1;
				}
			}
			this.NoticeGetMainDataCallBack(Global.Data.roleData.ArmorData, upLevel);
			if (data.Type == 0)
			{
				if (this.Star > star)
				{
					if (this.Star == this.mShenshenghudunXing.GetMaxStar())
					{
						this.AutoUp = 0;
					}
					Super.HintMainText(Global.GetLang("升星成功"), 10, 3);
				}
				ShenshenghudunXingVO shenshenghudunXingVOByID2 = this.mShenshenghudunXing.GetShenshenghudunXingVOByID(Global.Data.roleData.ArmorData.Armor);
				if (shenshenghudunXingVOByID2 != null)
				{
					if (0 < data.ZuanShi)
					{
						if (num > shenshenghudunXingVOByID2.ZuanShiExp)
						{
							this.AddTeXiao(2);
						}
					}
					else if (num > shenshenghudunXingVOByID2.GoodsExp)
					{
						this.AddTeXiao(2);
					}
				}
				if (num > 0)
				{
					Super.HintMainText(Global.GetLang(string.Format("+{0}", num)), 10, 3);
				}
			}
			else if (data.Type == 1 && this.Step > step)
			{
				this.AddTeXiao(3);
				Super.HintMainText(Global.GetLang("升阶成功"), 10, 3);
				this.AutoUp = 0;
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(data.Result, false, false)), 10, 3);
			this.AutoUp = 0;
		}
	}

	public void NoticeGetMainDataCallBack(RoleArmorData data, byte UpLevel = 0)
	{
		this.WaitingNet = false;
		if (data == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"RoleArmorData == null"
			});
			return;
		}
		this.mRoleArmorData = data;
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.mShenshenghudunXing.GetShenshenghudunXingVOByID(this.mRoleArmorData.Armor);
		if (shenshenghudunXingVOByID != null)
		{
			this.Star = shenshenghudunXingVOByID.StarLevel;
			this.Step = shenshenghudunXingVOByID.ArmorupStage;
			this.StarExp = data.Exp;
			this.StepExp = data.Exp;
			this.mFakeArmorStep = shenshenghudunXingVOByID.ArmorupStage;
			if (this.Star == this.mShenshenghudunXing.GetMaxStar())
			{
				this.StarOrStep = 1;
				this.StarExp = shenshenghudunXingVOByID.StarExp;
			}
			else
			{
				this.StarOrStep = 0;
			}
			this.RefreshEXP();
			this.RefreshStar(UpLevel);
			this.RefreshUpNeedsGoods();
			this.RefreshModal();
			this.RefeshAtt();
			this.RefreshBtns();
			this.RefreshStep();
		}
		this.RoleMoneyChange();
	}

	public void RoleMoneyChange()
	{
		this._DiamondValue.text = Global.Data.roleData.UserMoney.ToString();
	}

	public byte AutoUp
	{
		get
		{
			return this.mAutoUp;
		}
		set
		{
			this.mAutoUp = value;
			this.WaitingNet = false;
			this.RefreshUpBtnText();
		}
	}

	public byte StarOrStep
	{
		get
		{
			return this.mStarOrStep;
		}
		set
		{
			this.mStarOrStep = value;
			if (this.mStarOrStep == 0)
			{
				this._UpTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("升星消耗")
				});
			}
			else
			{
				this._UpTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("升阶消耗")
				});
			}
			this.RefreshUpBtnText();
		}
	}

	public int Star
	{
		get
		{
			return this.mStar;
		}
		set
		{
			this.mStar = value;
		}
	}

	public bool WaitingNet
	{
		get
		{
			return this.mWaitingNet;
		}
		set
		{
			this.mWaitingNet = value;
			if (this.mWaitingNet)
			{
				Super.ShowNetWaiting(null);
			}
			else
			{
				Super.HideNetWaiting();
			}
		}
	}

	public int Step
	{
		get
		{
			return this.mStep;
		}
		set
		{
			this.mStep = value;
		}
	}

	public int StarExp
	{
		get
		{
			return this.mStarExp;
		}
		private set
		{
			this.mStarExp = value;
		}
	}

	public int StepExp
	{
		get
		{
			return this.mStepExp;
		}
		private set
		{
			this.mStepExp = value;
		}
	}

	private const float AutoIntervalTime = 1.1f;

	private const int angle = 311;

	private const float CenterRadius = 175f;

	private const float BeignPos = -148f;

	[SerializeField]
	private GButton _BtnClose;

	[SerializeField]
	private ShowNetImage _CenterBakImage;

	[SerializeField]
	private GButton[] _JianTouBtns;

	[SerializeField]
	private UISprite _ProgressSp;

	[SerializeField]
	private GButton[] _UpBtns;

	[SerializeField]
	private GameObject _StarObj;

	[SerializeField]
	private Transform _StarRoot;

	[SerializeField]
	private GGoodIcon _GoodsIcon;

	[SerializeField]
	private GCheckBox _UseDiamond;

	[SerializeField]
	private UISprite _NotOpenObj;

	[SerializeField]
	private TextBlock[] _StarLabel;

	[SerializeField]
	private TextBlock[] _HuDunLabel;

	[SerializeField]
	private Modal3DShow _Modal;

	[SerializeField]
	private UILabel _JieLable;

	[SerializeField]
	private GImgProgressBar _StepGImgProgressBar;

	[SerializeField]
	private UILabel _DiamondValue;

	[SerializeField]
	private GameObject[] _TeXiao;

	[SerializeField]
	private ShowNetImage _BakModalImage0;

	[SerializeField]
	private UILabel _UpTitleLabel;

	[SerializeField]
	private UISprite[] _UpJianTou;

	public DPSelectedItemEventHandler Hander;

	private byte mAutoUp;

	private float mAutoIntervalTime;

	private byte mStarOrStep;

	private int mStar;

	private int mFakeArmorStep = 1;

	private ShenshenghudunJie mShenshenghudunJie;

	private ShenshenghudunXing mShenshenghudunXing;

	private List<UISprite> mStarList = new List<UISprite>();

	private RoleArmorData mRoleArmorData;

	private int mStarExp;

	private int mStepExp;

	private bool mWaitingNet;

	private int mStep;

	private byte mIsFakeData;

	public UISprite ZuanSpr;

	private GChildWindow messageBoxWindow;

	private ResourceResLoader laoder;
}
