using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RideFashionPart : UserControl
{
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (0 < this.mHorseResLoaderList.Count)
		{
			for (int i = 0; i < this.mHorseResLoaderList.Count; i++)
			{
				if (this.mHorseResLoaderList[i] != null)
				{
					this.mHorseResLoaderList[i].Stop();
				}
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mObs = this._FashionGoodsListBox.ItemsSource;
		this.mHorseFashionXml = IConfigbase<ConfigRidePet>.Instance.GetHorseFashionXmlInstance();
		this._StarSprite.gameObject.SetActive(false);
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitFashin();
		this.mHorseResLoaderList.Clear();
	}

	public override void Destroy()
	{
		base.Destroy();
		IConfigbase<ConfigRidePet>.Instance.ClearHorseFashionXmlInstance();
	}

	public override void Update()
	{
		base.Update();
		if (0f < this.mWearBtnCD)
		{
			this.mWearBtnCD -= Time.deltaTime;
		}
		if (0f < this.mUpBtnCD)
		{
			this.mUpBtnCD -= Time.deltaTime;
		}
	}

	private void InitFashin()
	{
		List<GoodsData> roleHorseFashionList = Global.GetRoleHorseFashionList(0);
		for (int i = 0; i < roleHorseFashionList.Count; i++)
		{
			RideFashionItem rideFashionItem = U3DUtils.NEW<RideFashionItem>();
			rideFashionItem.SetData(roleHorseFashionList[i]);
			this.mObs.AddNoUpdate(rideFashionItem);
			rideFashionItem.bSelect = false;
			rideFashionItem.DragPane = this._FashionGoodsView;
			rideFashionItem.Hander = new DPSelectedItemEventHandler(this.ItemHander);
		}
		if (0 < roleHorseFashionList.Count)
		{
			this.ItemHander(null, new DPSelectedItemEventArgs
			{
				Type = 1,
				ID = roleHorseFashionList[0].Id
			});
		}
	}

	private void RefreshTitle(GoodsData goodsData)
	{
		this._TitleLabel.text = Global.GetGoodsNameByID(goodsData.GoodsID, true);
	}

	private GameObject GetStar()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this._StarSprite.gameObject);
		gameObject.SetActive(true);
		return gameObject;
	}

	private void RefreshLevel(GoodsData goodsData)
	{
		for (int i = 0; i < this._GoodsRoot.transform.childCount; i++)
		{
			Transform child = this._GoodsRoot.transform.GetChild(i);
			if (null != child)
			{
				Object.Destroy(child.gameObject);
			}
		}
		int horseFashionMaxLevelByGoodsId = this.mHorseFashionXml.GetHorseFashionMaxLevelByGoodsId(goodsData.GoodsID);
		if (goodsData.Forge_level < horseFashionMaxLevelByGoodsId)
		{
			HorseFashionVO horseFashionVOByGoodsIDAndLevel = this.mHorseFashionXml.GetHorseFashionVOByGoodsIDAndLevel(goodsData.GoodsID, goodsData.Forge_level + 1);
			GoodsData goodsData2 = horseFashionVOByGoodsIDAndLevel.NeedGood[0];
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
			GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			newGoodIcon.GoodImg.URL = goodsImageURLFromIconCode;
			Super.InitGoodsGIcon(newGoodIcon, goodsData2, true, IconTextTypes.Qianghua);
			newGoodIcon.ItemObject = goodsData2;
			newGoodIcon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			U3DUtils.AddChild(this._GoodsRoot, newGoodIcon.gameObject, true);
			int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(goodsData2.GoodsID);
			newGoodIcon.SecondText.Label.supportEncoding = true;
			newGoodIcon.SecondText.text = Global.GetColorStringForNGUIText(new object[]
			{
				(roleGoodsNumberCountByGoodsID < goodsData2.GCount) ? "ff0000" : "f0f0f0",
				((roleGoodsNumberCountByGoodsID <= goodsData2.GCount) ? (roleGoodsNumberCountByGoodsID.ToString() + "/") : (goodsData2.GCount.ToString() + "/")) + goodsData2.GCount.ToString()
			});
			newGoodIcon.BackgroundSprite1.gameObject.SetActive(true);
			newGoodIcon.BackgroundSprite1.spriteName = "bagGrid3_bak";
			newGoodIcon.addEventListener("click", delegate(MouseEvent s)
			{
				GGoodIcon ggoodIcon = s.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData3 = ggoodIcon.ItemObject as GoodsData;
				if (goodsData3 == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData3);
			});
		}
	}

	private void RefreshModal(GoodsData goodsData)
	{
		if (this._ModalShow.ChildGameObjectList != null && 0 < this._ModalShow.ChildGameObjectList.Count)
		{
			this._ModalShow.Clear();
		}
		if (goodsData != null)
		{
			HorseResLoader horseResLoader = UIHelper.LoadHorseRes(this._ModalShow, goodsData.GoodsID, goodsData.Forge_level, Quaternion.Euler(new Vector3(0f, 135f, 0f)), new Vector3(110f, 110f, 110f), delegate(GameObject g)
			{
				if (this._ModalShow.ChildGameObjectList != null && 1 < this._ModalShow.ChildGameObjectList.Count)
				{
					for (int i = this._ModalShow.ChildGameObjectList.Count - 1; i > 0; i--)
					{
						if (null != this._ModalShow.ChildGameObjectList[i])
						{
							Object.Destroy(this._ModalShow.ChildGameObjectList[i]);
							this._ModalShow.ChildGameObjectList.RemoveAt(this._ModalShow.ChildGameObjectList.Count - 1);
						}
					}
					this._ModalShow._Target = this._ModalShow.ChildGameObjectList[0];
				}
			});
			this.mHorseResLoaderList.Add(horseResLoader);
		}
	}

	private void RefreshAtt(GoodsData goodsData)
	{
		if (0 < this.mStarObjList.Count)
		{
			for (int i = 0; i < this.mStarObjList.Count; i++)
			{
				if (null != this.mStarObjList[i])
				{
					Object.Destroy(this.mStarObjList[i]);
				}
			}
		}
		this._AttLabel.text = string.Empty;
		this._AttLabelEX.text = string.Empty;
		this.mStarObjList.Clear();
		if (goodsData != null)
		{
			Dictionary<string, double> dictionary = null;
			Dictionary<string, double> dictionary2 = null;
			int horseFashionMaxLevelByGoodsId = this.mHorseFashionXml.GetHorseFashionMaxLevelByGoodsId(goodsData.GoodsID);
			if (goodsData.Forge_level < horseFashionMaxLevelByGoodsId)
			{
				HorseFashionVO horseFashionVOByGoodsIDAndLevel = this.mHorseFashionXml.GetHorseFashionVOByGoodsIDAndLevel(goodsData.GoodsID, goodsData.Forge_level);
				if (horseFashionVOByGoodsIDAndLevel != null)
				{
					dictionary = horseFashionVOByGoodsIDAndLevel.AddValue;
				}
				HorseFashionVO horseFashionVOByGoodsIDAndLevel2 = this.mHorseFashionXml.GetHorseFashionVOByGoodsIDAndLevel(goodsData.GoodsID, goodsData.Forge_level + 1);
				if (horseFashionVOByGoodsIDAndLevel2 != null)
				{
					dictionary2 = horseFashionVOByGoodsIDAndLevel2.AddValue;
				}
				this._UpLevelBtn.disabledSprite = "btn2_normal";
				this._UpLevelBtn.pressedSprite = "btn2_pressred";
				this._UpLevelBtn.normalSprite = "btn2_normal";
				this._UpLevelBtn.hoverSprite = "btn2_pressred";
				this._UpLevelBtn.Refresh();
				this._UpLevelBtn.TextColor = Color.white;
				this._UpLevelBtn.GetComponent<BoxCollider>().enabled = true;
			}
			else
			{
				HorseFashionVO horseFashionVOByGoodsIDAndLevel3 = this.mHorseFashionXml.GetHorseFashionVOByGoodsIDAndLevel(goodsData.GoodsID, goodsData.Forge_level);
				if (dictionary == null)
				{
					dictionary = horseFashionVOByGoodsIDAndLevel3.AddValue;
				}
				this._UpLevelBtn.disabledSprite = "btn2_disabled";
				this._UpLevelBtn.pressedSprite = "btn2_disabled";
				this._UpLevelBtn.normalSprite = "btn2_disabled";
				this._UpLevelBtn.hoverSprite = "btn2_disabled";
				this._UpLevelBtn.Refresh();
				this._UpLevelBtn.TextColor = Color.gray;
				this._UpLevelBtn.GetComponent<BoxCollider>().enabled = false;
			}
			if (dictionary != null)
			{
				float num = 52f;
				string text = string.Empty;
				foreach (KeyValuePair<string, double> keyValuePair in dictionary)
				{
					string key = keyValuePair.Key;
					Dictionary<string, double>.Enumerator enumerator;
					KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
					double value = keyValuePair2.Value;
					if (0 < ConfigExtPropIndexes.GetExtPropIndexesShowListByWord(key))
					{
						string text2 = string.Empty;
						if (ConfigExtPropIndexes.GetPercentByWord(key))
						{
							double num2 = value * 100.0;
							if (dictionary2 != null && dictionary2.ContainsKey(key))
							{
								double num3 = dictionary2[key] - value;
								if (0.0 < num3)
								{
									text2 = text2 + (num3 * 100.0).ToString("f0") + "%";
								}
								text2 += "\n";
							}
							text = text + Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(key, true) + Global.GetLang("："),
								"fdf7dd",
								num2.ToString("f0") + "%"
							}) + Environment.NewLine;
						}
						else
						{
							double num4 = value;
							if (dictionary2 != null && dictionary2.ContainsKey(key))
							{
								double num5 = dictionary2[key] - value;
								if (0.0 < num5)
								{
									text2 += num5.ToString("f0");
								}
								text2 += "\n";
							}
							text = text + Global.GetColorStringForNGUIText(new object[]
							{
								"e3b36c",
								ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(key, true) + Global.GetLang("："),
								"fdf7ff",
								num4.ToString("f0")
							}) + Environment.NewLine;
						}
						if (!"\n".Equals(text2) && !string.Empty.Equals(text2))
						{
							GameObject star = this.GetStar();
							U3DUtils.AddChild(this._AttLabel.transform.parent.gameObject, star, true);
							Vector3 localPosition = star.transform.localPosition;
							localPosition.y = num;
							star.transform.localPosition = localPosition;
							this.mStarObjList.Add(star);
						}
						num -= 26f;
						UILabel attLabelEX = this._AttLabelEX;
						attLabelEX.text += text2;
					}
				}
				this._AttLabel.text = text;
			}
		}
	}

	private void ItemHander(object sender, DPSelectedItemEventArgs args)
	{
		GoodsData goodsData = null;
		if (args != null)
		{
			for (int i = 0; i < this.mObs.Count; i++)
			{
				GameObject at = this.mObs.GetAt(i);
				if (null != at)
				{
					RideFashionItem component = at.GetComponent<RideFashionItem>();
					if (null != component)
					{
						if (component.ID == args.ID)
						{
							goodsData = component.GoodsData;
							component.bSelect = true;
						}
						else
						{
							component.bSelect = false;
						}
					}
				}
			}
		}
		if (goodsData != null)
		{
			this.RefreshModal(goodsData);
			this.RefreshAtt(goodsData);
			this.RefreshTitle(goodsData);
			this.RefreshLevel(goodsData);
			GoodsData goodsData2 = Global.GetRoleHorseFashionList(0).Find((GoodsData e) => 1 == e.Using);
			if (goodsData2 != null)
			{
				if (goodsData2.Id == goodsData.Id)
				{
					this._WearBtn.Label.text = Global.GetLang("卸载");
				}
				else
				{
					this._WearBtn.Label.text = Global.GetLang("穿戴");
				}
			}
			else
			{
				this._WearBtn.Label.text = Global.GetLang("穿戴");
			}
		}
		this.mSelectGoodsData = goodsData;
	}

	private void InitPrefabText()
	{
		try
		{
			this._WearBtn.Label.text = ((Global.GetRoleHorseFashionList(0).Find((GoodsData e) => 1 == e.Using) != null) ? Global.GetLang("卸载") : Global.GetLang("穿戴"));
			this._TimeLabel.text = string.Empty;
			this._AttLabelEX.Margin = new Vector2(0f, 16f);
			this._AttLabel.Margin = new Vector2(0f, 16f);
			this._AttTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("时装属性")
			});
			this._UpConsumeTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("升级消耗")
			});
			this._UpLevelBtn.Label.text = Global.GetLang("升级");
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
			this._BakImage.URL = "NetImages/GameRes/Images/RidePet/ZuoQiDi.jpg";
			this._GoodsViewBakImage.URL = "NetImages/GameRes/Images/RidePet/CeBianKuang.png";
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
			this._PropBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				List<GoodsData> roleHorseFashionList = Global.GetRoleHorseFashionList(0);
				if (0 < roleHorseFashionList.Count)
				{
					string[] content = new string[]
					{
						Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							Global.GetLang("累计加成属性")
						}),
						this.GetProPertyStr(roleHorseFashionList)
					};
					Global.ShowProPerty(0, content, null);
				}
			};
			this._UpLevelBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < this.mUpBtnCD)
				{
					return;
				}
				this.mUpBtnCD = 1f;
				if (this.mSelectGoodsData != null)
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.SendFashionUp(this.mSelectGoodsData.Id);
				}
				else
				{
					Super.HintMainText(Global.GetLang("请选中要操作的时装"), 10, 3);
				}
			};
			this._WearBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < this.mWearBtnCD)
				{
					return;
				}
				this.mWearBtnCD = 1f;
				if (this.mSelectGoodsData != null)
				{
					if (Global.GetRoleFightHorseData(Global.Data.RoleID) == null)
					{
						Super.HintMainText(Global.GetLang("请先出战坐骑"), 10, 3);
						return;
					}
					if (!(Global.GetLang("穿戴") == this._WearBtn.Label.text))
					{
						this._WearBtn.Label.text = Global.GetLang("穿戴");
						GameInstance.Game.SpriteModGoods(2, this.mSelectGoodsData.Id, this.mSelectGoodsData.GoodsID, 0, 6000, this.mSelectGoodsData.GCount, this.mSelectGoodsData.BagIndex, string.Empty);
					}
					else
					{
						List<GoodsData> roleHorseFashionList = Global.GetRoleHorseFashionList(0);
						if (0 < roleHorseFashionList.Count)
						{
							Super.ShowNetWaiting(null);
							GoodsData goodsData = roleHorseFashionList.Find((GoodsData v) => v.Using == 1);
							if (goodsData != null)
							{
								GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, 0, 6000, goodsData.GCount, goodsData.BagIndex, string.Empty);
							}
							GameInstance.Game.SpriteModGoods(1, this.mSelectGoodsData.Id, this.mSelectGoodsData.GoodsID, 1, 6000, this.mSelectGoodsData.GCount, this.mSelectGoodsData.BagIndex, string.Empty);
						}
						this._WearBtn.Label.text = Global.GetLang("卸载");
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("请选中要操作的时装"), 10, 3);
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

	private string GetProPertyStr(List<GoodsData> lst)
	{
		string text = string.Empty;
		if (0 < lst.Count)
		{
			Dictionary<string, double> dictionary = new Dictionary<string, double>();
			for (int i = 0; i < lst.Count; i++)
			{
				GoodsData goodsData = lst[i];
				HorseFashionVO horseFashionVOByGoodsIDAndLevel = this.mHorseFashionXml.GetHorseFashionVOByGoodsIDAndLevel(goodsData.GoodsID, goodsData.Forge_level);
				if (horseFashionVOByGoodsIDAndLevel != null)
				{
					Dictionary<string, double>.Enumerator enumerator = horseFashionVOByGoodsIDAndLevel.AddValue.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Dictionary<string, double> dictionary2 = dictionary;
						KeyValuePair<string, double> keyValuePair = enumerator.Current;
						if (!dictionary2.ContainsKey(keyValuePair.Key))
						{
							double num = 0.0;
							KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
							if (num < keyValuePair2.Value)
							{
								Dictionary<string, double> dictionary3 = dictionary;
								KeyValuePair<string, double> keyValuePair3 = enumerator.Current;
								string key = keyValuePair3.Key;
								KeyValuePair<string, double> keyValuePair4 = enumerator.Current;
								dictionary3.Add(key, keyValuePair4.Value);
							}
						}
						else
						{
							double num2 = 0.0;
							KeyValuePair<string, double> keyValuePair5 = enumerator.Current;
							if (num2 < keyValuePair5.Value)
							{
								Dictionary<string, double> dictionary5;
								Dictionary<string, double> dictionary4 = dictionary5 = dictionary;
								KeyValuePair<string, double> keyValuePair6 = enumerator.Current;
								string key2;
								string text2 = key2 = keyValuePair6.Key;
								double num3 = dictionary5[key2];
								double num4 = num3;
								KeyValuePair<string, double> keyValuePair7 = enumerator.Current;
								dictionary4[text2] = num4 + keyValuePair7.Value;
							}
						}
					}
				}
			}
			Dictionary<string, double>.Enumerator enumerator2 = dictionary.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				int num5 = 0;
				KeyValuePair<string, double> keyValuePair8 = enumerator2.Current;
				if (num5 < ConfigExtPropIndexes.GetExtPropIndexesShowListByWord(keyValuePair8.Key))
				{
					KeyValuePair<string, double> keyValuePair9 = enumerator2.Current;
					if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair9.Key))
					{
						string text3 = text;
						string lang = Global.GetLang("{0}：{1}%");
						KeyValuePair<string, double> keyValuePair10 = enumerator2.Current;
						object extPropIndexesDescriptionByWord = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair10.Key, true);
						KeyValuePair<string, double> keyValuePair11 = enumerator2.Current;
						text = text3 + string.Format(lang, extPropIndexesDescriptionByWord, keyValuePair11.Value * 100.0) + Environment.NewLine;
					}
					else
					{
						string text4 = text;
						string lang2 = Global.GetLang("{0}：{1}");
						KeyValuePair<string, double> keyValuePair12 = enumerator2.Current;
						object extPropIndexesDescriptionByWord2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair12.Key, true);
						KeyValuePair<string, double> keyValuePair13 = enumerator2.Current;
						text = text4 + string.Format(lang2, extPropIndexesDescriptionByWord2, keyValuePair13.Value) + Environment.NewLine;
					}
				}
			}
		}
		return text;
	}

	internal void NoticeUpFashionCallBack(string[] p)
	{
		Super.HideNetWaiting();
		if ("0" == p[0])
		{
			GoodsData goodsData = null;
			for (int i = 0; i < this.mObs.Count; i++)
			{
				GameObject at = this.mObs.GetAt(i);
				if (null != at)
				{
					RideFashionItem component = at.GetComponent<RideFashionItem>();
					if (null != component && component.GoodsData.Id == p[2].SafeToInt32(0))
					{
						goodsData = component.GoodsData;
						component.Level = int.Parse(p[3]);
						break;
					}
				}
			}
			if (goodsData != null)
			{
				goodsData.Forge_level = int.Parse(p[3]);
				this.RefreshModal(goodsData);
				this.RefreshAtt(goodsData);
				this.RefreshTitle(goodsData);
				this.RefreshLevel(goodsData);
			}
		}
		else
		{
			string errMsg = StdErrorCode.GetErrMsg(p[0].SafeToInt32(0), true, false);
			Super.HintMainText(Global.GetLang(errMsg), 10, 3);
		}
	}

	internal void NoticeWearFashionCallBask(SCModGoods ModGoods)
	{
		Super.HideNetWaiting();
		GoodsData goodsData = null;
		for (int i = 0; i < this.mObs.Count; i++)
		{
			GameObject at = this.mObs.GetAt(i);
			if (null != at)
			{
				RideFashionItem component = at.GetComponent<RideFashionItem>();
				if (null != component && component.GoodsData.Id == ModGoods.ID)
				{
					goodsData = component.GoodsData;
					goodsData.Using = ModGoods.IsUsing;
					component.SetData(goodsData);
					break;
				}
			}
		}
		if (goodsData != null && (ModGoods.ModType == 1 || ModGoods.ModType == 2))
		{
			goodsData.Using = ModGoods.IsUsing;
			this.RefreshModal(goodsData);
			this.RefreshAtt(goodsData);
			this.RefreshTitle(goodsData);
			this.RefreshLevel(goodsData);
			if (goodsData.Using == 1)
			{
				Super.HintMainText(Global.GetLang("时装已穿戴"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang("时装已卸下"), 10, 3);
			}
		}
		List<GoodsData> roleHorseFashionList = Global.GetRoleHorseFashionList(0);
		if (0 < roleHorseFashionList.Count)
		{
			GoodsData goodsData2 = roleHorseFashionList.Find((GoodsData v) => v.Id == ModGoods.ID);
			if (goodsData2 != null)
			{
				goodsData2.Using = ModGoods.IsUsing;
			}
		}
		this._WearBtn.Label.text = ((Global.GetRoleHorseFashionList(0).Find((GoodsData e) => 1 == e.Using) != null) ? Global.GetLang("卸载") : Global.GetLang("穿戴"));
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private ShowNetImage _BakImage;

	[SerializeField]
	private ShowNetImage _GoodsViewBakImage;

	[SerializeField]
	private UILabel _AttTitleLabel;

	[SerializeField]
	private UILabel _UpConsumeTitleLabel;

	[SerializeField]
	private UISprite _StarSprite;

	[SerializeField]
	private UILabel _AttLabel;

	[SerializeField]
	private UILabel _AttLabelEX;

	[SerializeField]
	private GButton _UpLevelBtn;

	[SerializeField]
	private GButton _WearBtn;

	[SerializeField]
	private GameObject _GoodsRoot;

	[SerializeField]
	private Modal3DShow _ModalShow;

	[SerializeField]
	private UIDraggablePanel _FashionGoodsView;

	[SerializeField]
	private ListBox _FashionGoodsListBox;

	[SerializeField]
	private UILabel _TitleLabel;

	[SerializeField]
	private UILabel _TimeLabel;

	[SerializeField]
	private GButton _PropBtn;

	private ObservableCollection mObs;

	private HorseFashionXml mHorseFashionXml;

	private List<GameObject> mStarObjList = new List<GameObject>();

	private GoodsData mSelectGoodsData;

	private float mUpBtnCD;

	private float mWearBtnCD;

	private List<HorseResLoader> mHorseResLoaderList = new List<HorseResLoader>();
}
