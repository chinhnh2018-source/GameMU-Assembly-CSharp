using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class RidePetZuoQiPart : UserControl
{
	public int CaoWeiLevel
	{
		get
		{
			if (this.mZuoQiMainData != null)
			{
				return this.mZuoQiMainData.MountLevel + 1;
			}
			return 1;
		}
	}

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
		this.InitText();
		this.InitOnClick();
		this.InitPanel();
		this.mHorseResLoaderList.Clear();
	}

	public void RefreshData()
	{
		GameInstance.Game.GetRidePetMainData();
		Super.ShowNetWaiting(null);
		this.InitHorseData(true);
	}

	private void InitPanel()
	{
		this.mCollection = this.mListBox.ItemsSource;
		this.mPanelShengJiAndShengJie.gameObject.SetActive(false);
	}

	private void InitText()
	{
		this.mBtnShengJi.Label.text = Global.GetLang("升级");
		this.mBtnShengJie.Label.text = Global.GetLang("进阶");
		this.mLabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑技能")
		});
		this.mLabShuXingTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑基础属性")
		});
		this.mLabZhuoYueTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("坐骑卓越属性")
		});
		this.RefreshBeiBaoCount();
	}

	public void RefreshBeiBaoCount()
	{
		if (Global.Data.roleData.MountStoreList != null && 0 < Global.Data.roleData.MountStoreList.Count)
		{
			this.mBtnBeiBaoHongDian.gameObject.SetActive(true);
			this.mBtnBeiBao.Text = Global.Data.roleData.MountStoreList.Count.ToString();
		}
		else
		{
			this.mBtnBeiBaoHongDian.gameObject.SetActive(false);
		}
	}

	private void InitOnClick()
	{
		this.mBtnLieQu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenRidePetLieQuPartWindow(this.mHaveAvtiviteList);
		};
		this.mBtnQiYuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				ID = 1520
			});
		};
		this.mBtnZuHe.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowRideZuHeSowPart();
		};
		this.mBtnBeiBao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(s, new DPSelectedItemEventArgs
				{
					ID = 10
				});
			}
		};
		this.mBtnZhaoHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.HorseUse();
		};
		this.mBtnShengJi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowShengJi();
		};
		this.mBtnShengJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowShengJie();
		};
		this.mBtnCloseShengJiAndShengJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.mPanelShengJiAndShengJie.gameObject.SetActive(false);
			this.mPanelShengJiAndShengJie.ClearData();
		};
	}

	private void ShowShengJie()
	{
		this.mPanelShengJiAndShengJie.gameObject.SetActive(true);
		RidePetItem selectItem = this.GetSelectItem();
		if (null != selectItem && selectItem.GoodsData != null)
		{
			GoodsData roleHorseGoodsDataInBeiZhanBaoGuoByDbId = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, selectItem.GoodsData.Id, 1);
			if (roleHorseGoodsDataInBeiZhanBaoGuoByDbId != null)
			{
				this.AddStarRefresh(roleHorseGoodsDataInBeiZhanBaoGuoByDbId);
			}
		}
	}

	private void ShowShengJi()
	{
		this.mPanelShengJiAndShengJie.gameObject.SetActive(true);
		RidePetItem selectItem = this.GetSelectItem();
		if (null != selectItem && selectItem.GoodsData != null)
		{
			GoodsData roleHorseGoodsDataInBeiZhanBaoGuoByDbId = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, selectItem.GoodsData.Id, 1);
			if (roleHorseGoodsDataInBeiZhanBaoGuoByDbId != null)
			{
				this.AddUpRefresh(roleHorseGoodsDataInBeiZhanBaoGuoByDbId, this.CaoWeiLevel);
			}
		}
	}

	public override void Update()
	{
		base.Update();
		this.mUseHorseCD -= Time.deltaTime;
	}

	private void HorseUse()
	{
		if (0f < this.mUseHorseCD)
		{
			return;
		}
		this.mUseHorseCD = 2f;
		RidePetItem selectItem = this.GetSelectItem();
		if (null != selectItem)
		{
			GoodsData goodsData = selectItem.GoodsData;
			if (goodsData != null)
			{
				if (goodsData.Using == 0)
				{
					GoodsData roleFightHorseData = Global.GetRoleFightHorseData(Global.Data.RoleID);
					if (roleFightHorseData != null)
					{
						roleFightHorseData.Using = 0;
						GameInstance.Game.SpriteModGoods(2, roleFightHorseData.Id, roleFightHorseData.GoodsID, roleFightHorseData.Using, roleFightHorseData.Site, roleFightHorseData.GCount, roleFightHorseData.BagIndex, string.Empty);
					}
					Global.ToUseGoods(selectItem.GoodsData, true, false);
				}
				else
				{
					Super.ShowNetWaiting(null);
					List<GoodsData> roleHorseFashionList = Global.GetRoleHorseFashionList(0);
					if (0 < roleHorseFashionList.Count)
					{
						GoodsData goodsData2 = roleHorseFashionList.Find((GoodsData v) => v.Using == 1);
						if (goodsData2 != null)
						{
							GameInstance.Game.SpriteModGoods(2, goodsData2.Id, goodsData2.GoodsID, 0, 6000, goodsData2.GCount, goodsData2.BagIndex, string.Empty);
						}
					}
					goodsData.Using = 0;
					GameInstance.Game.SpriteModGoods(2, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("所选择槽位没有坐骑"), 10, 3);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("请选择要操作的坐骑"), 10, 3);
		}
	}

	private void InitHorseData(bool refreshMode = false)
	{
		bool flag = false;
		List<GoodsData> roleHorseDataList = Global.GetRoleHorseDataList(Global.Data.roleData.MountEquipList);
		roleHorseDataList.Sort((GoodsData b, GoodsData a) => a.Using - b.Using);
		for (int i = 0; i < 4; i++)
		{
			GoodsData goodsData = (i >= roleHorseDataList.Count) ? null : roleHorseDataList[i];
			if (goodsData != null && refreshMode && goodsData.Using == 1)
			{
				this.RefreshShuXing(goodsData);
				this.RefreshMode(goodsData);
				flag = true;
			}
			this.InitHorseIcon(goodsData, i, refreshMode, false);
		}
		this.mListBox.repositionNow = true;
		if (!flag)
		{
			if (0 < roleHorseDataList.Count)
			{
				this.RefreshShuXing(roleHorseDataList[0]);
				this.RefreshMode(roleHorseDataList[0]);
				this.InitHorseIcon(roleHorseDataList[0], 0, refreshMode, true);
			}
			else
			{
				this.RefreshMode(null);
				this.RefreshShuXing(null);
			}
		}
		this.RefreshUseHorseBtn();
	}

	private void RefreshMode(GoodsData ModeGoodsdata)
	{
		if (this.mGameModal.ChildGameObjectList != null && 0 < this.mGameModal.ChildGameObjectList.Count)
		{
			this.mGameModal.Clear();
		}
		if (ModeGoodsdata != null)
		{
			HorseResLoader horseResLoader = UIHelper.LoadHorseRes(this.mGameModal, ModeGoodsdata.GoodsID, ModeGoodsdata.Forge_level + 1, Quaternion.Euler(new Vector3(0f, 135f, 0f)), new Vector3(110f, 110f, 110f), delegate(GameObject g)
			{
				if (this.mGameModal.ChildGameObjectList != null && 1 < this.mGameModal.ChildGameObjectList.Count)
				{
					for (int i = this.mGameModal.ChildGameObjectList.Count - 1; i > 0; i--)
					{
						if (null != this.mGameModal.ChildGameObjectList[i])
						{
							Object.Destroy(this.mGameModal.ChildGameObjectList[i]);
							this.mGameModal.ChildGameObjectList.RemoveAt(this.mGameModal.ChildGameObjectList.Count - 1);
						}
					}
					this.mGameModal._Target = this.mGameModal.ChildGameObjectList[0];
				}
			});
			this.mHorseResLoaderList.Add(horseResLoader);
		}
	}

	private void InitHorseIcon(GoodsData iconData, int index, bool RefreshChuZhan, bool forceSelect = false)
	{
		RidePetItem ridePetItem = null;
		GameObject at = this.mCollection.GetAt(index);
		if (null != at)
		{
			ridePetItem = at.GetComponent<RidePetItem>();
			if (null == ridePetItem)
			{
				this.mCollection.RemoveAt(index);
			}
		}
		if (null == ridePetItem)
		{
			ridePetItem = U3DUtils.NEW<RidePetItem>();
			this.mCollection.AddNoUpdate(ridePetItem);
			ridePetItem.ItemCout = index;
			ridePetItem.DPSelectedItem = new DPSelectedItemEventHandler(this.ItemOnClick);
		}
		if (forceSelect)
		{
			ridePetItem.ItemIsSelect = true;
		}
		else if (RefreshChuZhan)
		{
			if (iconData != null)
			{
				ridePetItem.ChuZhan = (1 == iconData.Using);
				ridePetItem.ItemIsSelect = (1 == iconData.Using);
			}
			else
			{
				ridePetItem.ChuZhan = false;
				ridePetItem.ItemIsSelect = false;
			}
		}
		if (ridePetItem.GetComponent<BoxCollider>() != null)
		{
			Object.Destroy(ridePetItem.GetComponent<BoxCollider>());
		}
		ridePetItem.SetData(iconData, RidePetUIType.ZuoQi);
	}

	private void ShowRideZuHeSowPart()
	{
		this.CloseRideZuHeSowPart();
		if (null != this.mRideZuHeSowPartWind)
		{
			this.mRideZuHeSowPartWind.Visibility = true;
		}
		else
		{
			this.mRideZuHeSowPartWind = U3DUtils.NEW<GChildWindow>();
			this.mRideZuHeSowPartWind.Modal = true;
			this.mRideZuHeSowPartWind.ModalType = ChildWindowModalType.Translucent;
			base.Add(this.mRideZuHeSowPartWind);
			WindowManage.AddWindows(this.mRideZuHeSowPartWind, true, null);
		}
		this.mRideZuHeSowPart = U3DUtils.NEW<RideZuHeSowPart>();
		this.mRideZuHeSowPart.hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.Type == 0)
			{
				this.CloseRideZuHeSowPart();
			}
		};
		this.mRideZuHeSowPartWind.Body.Add(this.mRideZuHeSowPart);
	}

	private void CloseRideZuHeSowPart()
	{
		if (null != this.mRideZuHeSowPart)
		{
			Object.Destroy(this.mRideZuHeSowPart.gameObject);
		}
		WindowManage.RemoveWindows(this.mRideZuHeSowPartWind);
		if (null != this.mRideZuHeSowPartWind)
		{
			Object.Destroy(this.mRideZuHeSowPartWind.gameObject);
		}
	}

	public void GetData(ZuoQiMainData data)
	{
		this.mZuoQiMainData = data;
		this.NoticeRideUpLevelCallBack(this.CaoWeiLevel, false);
		this.RefreshHorseUseing();
	}

	private void ItemOnClick(object s, DPSelectedItemEventArgs e)
	{
		if (e.ID == 1)
		{
			if (e.IDType == 1)
			{
				for (int i = 0; i < this.mCollection.Count; i++)
				{
					RidePetItem component = this.mCollection.GetAt(i).GetComponent<RidePetItem>();
					if (component.Id == e.MyID)
					{
						component.ItemIsSelect = true;
						if (component.GoodsData != null)
						{
							this.RefreshShuXing(component.GoodsData);
							this.RefreshMode(component.GoodsData);
						}
					}
					else
					{
						component.ItemIsSelect = false;
					}
					this.RefreshUseHorseBtn();
				}
			}
			else if (e.IDType == 3)
			{
			}
		}
		else if (e.ID == 2 && e.IDType == 1)
		{
			GoodsData roleFightHorseData = Global.GetRoleFightHorseData(Global.Data.RoleID);
			if (roleFightHorseData != null && roleFightHorseData.Id == e.MyID)
			{
				roleFightHorseData.Using = 0;
				Super.ShowNetWaiting(null);
				GameInstance.Game.SpriteModGoods(2, roleFightHorseData.Id, roleFightHorseData.GoodsID, roleFightHorseData.Using, roleFightHorseData.Site, roleFightHorseData.GCount, roleFightHorseData.BagIndex, string.Empty);
			}
			GoodsData roleHorseGoodsDataInBeiZhanBaoGuoByDbId = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, e.MyID, 1);
			if (roleHorseGoodsDataInBeiZhanBaoGuoByDbId != null)
			{
				Super.ShowNetWaiting(null);
				GameInstance.Game.SpriteModGoods(3, roleHorseGoodsDataInBeiZhanBaoGuoByDbId.Id, roleHorseGoodsDataInBeiZhanBaoGuoByDbId.GoodsID, roleHorseGoodsDataInBeiZhanBaoGuoByDbId.Using, 0, roleHorseGoodsDataInBeiZhanBaoGuoByDbId.GCount, roleHorseGoodsDataInBeiZhanBaoGuoByDbId.BagIndex, string.Empty);
			}
		}
	}

	private RidePetItem GetSelectItem()
	{
		for (int i = 0; i < this.mCollection.Count; i++)
		{
			RidePetItem component = this.mCollection.GetAt(i).GetComponent<RidePetItem>();
			if (component.ItemIsSelect)
			{
				return component;
			}
		}
		return null;
	}

	private void RefreshUseHorseBtn()
	{
		RidePetItem selectItem = this.GetSelectItem();
		if (null != selectItem)
		{
			if (selectItem.GoodsData != null)
			{
				if (selectItem.GoodsData.Using == 1)
				{
					this.ChangeBtnSatae(this.mBtnZhaoHui, true, "ZuoQiZhaoHui");
				}
				else
				{
					this.ChangeBtnSatae(this.mBtnZhaoHui, true, "ZuoQiChuZhan");
				}
				if (selectItem.ItemCout == 0)
				{
					this.mBtnShengJi.gameObject.SetActive(true);
					this.mBtnShengJie.gameObject.SetActive(true);
					this.mBtnShengJie.transform.localPosition = new Vector3(105f, 0f, 0f);
					this.mBtnShengJi.transform.localPosition = new Vector3(-105f, 0f, 0f);
				}
				else
				{
					this.mBtnShengJi.gameObject.SetActive(false);
					this.mBtnShengJie.gameObject.SetActive(true);
					this.mBtnShengJie.transform.localPosition = new Vector3(0f, 0f, 0f);
				}
			}
		}
		else
		{
			bool flag = false;
			for (int i = 0; i < this.mCollection.Count; i++)
			{
				RidePetItem component = this.mCollection.GetAt(i).GetComponent<RidePetItem>();
				if (component.GoodsData != null && component.GoodsData.Using == 1)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				this.ChangeBtnSatae(this.mBtnZhaoHui, true, "ZuoQiChuZhan");
				this.mBtnShengJi.gameObject.SetActive(false);
				this.mBtnShengJie.gameObject.SetActive(true);
				this.mBtnShengJie.transform.localPosition = new Vector3(0f, 0f, 0f);
			}
		}
		List<GoodsData> roleHorseDataList = Global.GetRoleHorseDataList(Global.Data.roleData.MountEquipList);
		if (0 >= roleHorseDataList.Count)
		{
			this.mBtnShengJi.gameObject.SetActive(false);
			this.mBtnShengJie.gameObject.SetActive(false);
		}
	}

	private void ChangeBtnSatae(GButton btn, bool CanClick, string BakName)
	{
		if (null != btn)
		{
			btn.enabled = CanClick;
			btn.normalSprite = BakName;
			btn.hoverSprite = BakName;
			btn.disabledSprite = BakName;
			btn.pressedSprite = BakName;
			btn.Refresh();
		}
	}

	public void RefreshShuXing(GoodsData data)
	{
		for (int i = 0; i < this.mUpSpList.Count; i++)
		{
			Object.DestroyObject(this.mUpSpList[i].gameObject);
		}
		this.mUpSpList.Clear();
		int level = (data != null) ? (data.Forge_level + 1) : 0;
		try
		{
			if (!Mathf.Approximately(this.mDraggablePanel.transform.localPosition.y, 109f))
			{
				SpringPanel.Begin(this.mDraggablePanel.gameObject, new Vector3(-1f, 109f, 0f), 10f);
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
		if (data == null)
		{
			this.mLabTitleLV.text = string.Empty;
			this.mLabTitleName.text = string.Empty;
			this.mLabShuXingTitle.text = string.Empty;
			this.mLabShuXingContent.text = string.Empty;
			this.mLabZhuoYueTitle.text = string.Empty;
			this.mLabZhuoYueContent.text = string.Empty;
			this.PrppSkillInfRoot.SetActive(false);
		}
		else
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			byte b = 0;
			HorseSkillData horseSkillData = new HorseSkillData(data);
			if (0 < horseSkillData.SkillID)
			{
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(horseSkillData.SkillID);
				if (skillXmlNode != null)
				{
					this.PrppSkillInfRoot.SetActive(true);
					this.mShowSkill.URL = StringUtil.substitute("NetImages/GameRes/Images/Skill/{0}.png", new object[]
					{
						skillXmlNode.MagicIcon
					});
					this.mLabSkillTitle.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						Global.GetLang(skillXmlNode.Name)
					});
					b = 1;
					SkillData skillDataByID = Global.GetSkillDataByID(horseSkillData.SkillID);
					if (skillDataByID != null)
					{
						this.mLabSkillContent.text = skillXmlNode.Description;
						this.mLabSkillLV.text = string.Format("LV{0}", skillDataByID.SkillLevel);
					}
				}
			}
			this.SkillAttBeginPosY = -48f;
			if (b == 0)
			{
				this.PrppSkillInfRoot.SetActive(false);
				this.SkillAttBeginPosY -= this.mLabSkillContent.relativeSize.y * this.mLabSkillContent.transform.localScale.y;
				this._AttViewRoot.transform.localPosition = new Vector3(0f, 50f, -1f);
			}
			else
			{
				this._AttViewRoot.transform.localPosition = new Vector3(0f, -70f, -1f);
			}
			HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(data.GoodsID);
			if (horsePokedexByHorseID == null)
			{
				return;
			}
			this.mLabTitleName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang(horsePokedexByHorseID.Name)
			}) + "  " + Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Concat(new object[]
				{
					(data.Forge_level + 1).ToString(),
					Global.GetLang("阶"),
					"LV ",
					this.CaoWeiLevel
				})
			});
			this.ChangeTransPosY(this.mLabShuXingTitle, this.SkillAttBeginPosY);
			this.SkillAttBeginPosY -= this.mLabShuXingTitle.relativeSize.y * this.mLabShuXingTitle.transform.localScale.y;
			float num = 0f;
			float num2 = 0f;
			if (IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp.ContainsKey(this.CaoWeiLevel))
			{
				num = IConfigbase<ConfigRidePet>.Instance.DicHorseLevelUp[this.CaoWeiLevel].AdvancedEffect;
			}
			if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, level) != null)
			{
				num2 = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(data.GoodsID, level).AdvancedEffectFloat;
			}
			if (0f >= num)
			{
				num = 0f;
			}
			if (0f >= num2)
			{
				num2 = 0f;
			}
			if (goodsXmlNodeByID != null)
			{
				this.mLabShuXingTitle.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("坐骑基础属性")
				});
				double[] equipProps = goodsXmlNodeByID.EquipProps;
				Dictionary<int, double> dictionary = new Dictionary<int, double>();
				string text = string.Empty;
				for (int j = 0; j < equipProps.Length; j++)
				{
					if (equipProps[j] > 0.0)
					{
						if (dictionary.ContainsKey(j))
						{
							Dictionary<int, double> dictionary3;
							Dictionary<int, double> dictionary2 = dictionary3 = dictionary;
							int num4;
							int num3 = num4 = j;
							double num5 = dictionary3[num4];
							dictionary2[num3] = num5 + (equipProps[j] + equipProps[j] * (double)num2 + equipProps[j] * (double)num);
						}
						else
						{
							dictionary.Add(j, equipProps[j] + equipProps[j] * (double)num2 + equipProps[j] * (double)num);
						}
					}
				}
				foreach (KeyValuePair<int, double> keyValuePair in dictionary)
				{
					int num6 = keyValuePair.Key;
					Dictionary<int, double>.Enumerator enumerator;
					KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
					double value = keyValuePair2.Value;
					if (num6 != 0)
					{
						if (num6 != 8 && num6 != 9 && num6 != 10 && num6 != 4 && num6 != 5 && num6 != 6)
						{
							if (num6 == 7)
							{
								num6 = 45;
							}
							if (num6 == 3)
							{
								num6 = 46;
							}
							if (ConfigExtPropIndexes.GetPercentByID(num6))
							{
								double num7 = value * 100.0;
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num6, true) + Global.GetLang("："),
									"fdf7dd",
									num7.ToString("f0")
								}) + "%" + Environment.NewLine;
							}
							else
							{
								double num8 = value;
								text = text + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(num6, true) + Global.GetLang("："),
									"fdf7ff",
									num8.ToString("f0")
								}) + Environment.NewLine;
							}
						}
					}
				}
				HorsePokedexVO horsePokedexByHorseID2 = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(data.GoodsID);
				if (horsePokedexByHorseID2 != null)
				{
					text = text + Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("移动速度：")
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7ff",
						(int)(horsePokedexByHorseID2.HorseSpeed * 100f) + "%"
					}) + Environment.NewLine;
				}
				this.ChangeTransPosY(this.mLabShuXingContent, this.SkillAttBeginPosY);
				this.mLabShuXingContent.text = text;
				if (!string.IsNullOrEmpty(text))
				{
					this.SkillAttBeginPosY -= this.mLabShuXingContent.relativeSize.y * this.mLabShuXingContent.transform.localScale.y;
				}
				if (data.WashProps != null && 0 < data.WashProps.Count)
				{
					this.mLabZhuoYueTitle.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fac60d",
						Global.GetLang("坐骑卓越属性")
					});
					this.ChangeTransPosY(this.mLabZhuoYueTitle, this.SkillAttBeginPosY);
					this.SkillAttBeginPosY -= this.mLabZhuoYueTitle.relativeSize.y * this.mLabZhuoYueTitle.transform.localScale.y;
					string text2 = string.Empty;
					Dictionary<string, int> dictionary4 = Global.MaxZhuoYurZuoQi(data.GoodsID);
					for (int k = 0; k < data.WashProps.Count; k += 2)
					{
						if (k < data.WashProps.Count - 1 && 0 < data.WashProps[k] && 0 < data.WashProps[1])
						{
							if (ConfigExtPropIndexes.GetPercentByID(data.WashProps[k]))
							{
								text2 = text2 + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(data.WashProps[k], false) + Global.GetLang("："),
									"17e43e",
									((float)data.WashProps[k + 1] / 1000f * 100f).ToString("f1") + "%"
								}) + "\n";
							}
							else
							{
								text2 = text2 + Global.GetColorStringForNGUIText(new object[]
								{
									"e3b36c",
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(data.WashProps[k], false) + Global.GetLang("："),
									"17e43e",
									((float)data.WashProps[k + 1] / 1000f).ToString("f0")
								}) + "\n";
							}
							if (ConfigExtPropIndexes.GetExtPropIndexesVoByID(data.WashProps[k]) != null)
							{
								string word = ConfigExtPropIndexes.GetExtPropIndexesVoByID(data.WashProps[k]).Word;
								if (dictionary4.ContainsKey(word) && dictionary4[word] <= data.WashProps[k + 1])
								{
									GameObject gameObject = Object.Instantiate<GameObject>(this.mUpSp.gameObject);
									gameObject.transform.SetParent(this.mDraggablePanel.transform, false);
									this.mUpSpList.Add(gameObject);
									gameObject.transform.localPosition = new Vector3(65f, this.SkillAttBeginPosY + 11.5f - (float)(23 * (k / 2)), -1f);
								}
							}
						}
					}
					for (int l = 0; l < this.mUpSpList.Count; l++)
					{
						this.mUpSpList[l].gameObject.SetActive(true);
					}
					this.ChangeTransPosY(this.mLabZhuoYueContent, this.SkillAttBeginPosY);
					this.mLabZhuoYueContent.text = text2;
				}
				else
				{
					this.mLabZhuoYueTitle.text = string.Empty;
					this.mLabZhuoYueContent.text = string.Empty;
				}
			}
		}
	}

	private void ChangeTransPosY(Component component, float y)
	{
		if (null != component)
		{
			Vector3 localPosition = component.transform.localPosition;
			localPosition.y = y;
			component.transform.localPosition = localPosition;
		}
	}

	private void AddUpRefresh(GoodsData goodsData, int level)
	{
		if (goodsData == null)
		{
			return;
		}
		int num = 0;
		int[] array = new int[3];
		int num2 = 0;
		int num3 = 0;
		foreach (KeyValuePair<int, int[]> keyValuePair in ConfigSystemParam.GetSystemParamIntDictByName("HorseLevelMax", '|', ','))
		{
			int num4 = keyValuePair.Value[0] * 1000;
			Dictionary<int, int[]>.Enumerator enumerator;
			KeyValuePair<int, int[]> keyValuePair2 = enumerator.Current;
			if (num4 + keyValuePair2.Value[1] >= Global.Data.roleData.ChangeLifeCount * 1000 + Global.Data.roleData.Level)
			{
				if (num3 == 1)
				{
					KeyValuePair<int, int[]> keyValuePair3 = enumerator.Current;
					array = keyValuePair3.Value;
					num3 = 2;
				}
			}
			else
			{
				num3 = 1;
				KeyValuePair<int, int[]> keyValuePair4 = enumerator.Current;
				num = keyValuePair4.Value[2];
			}
			KeyValuePair<int, int[]> keyValuePair5 = enumerator.Current;
			num2 = keyValuePair5.Value[2];
		}
		if (level >= num2)
		{
			this.mPanelShengJiAndShengJie.Refresh(RidePetUpType.MaxLevel, goodsData, level);
			this.mPanelShengJiAndShengJie.AddLevelStr = Global.GetColorStringForNGUIText(new object[]
			{
				string.Empty,
				Global.GetLang("坐骑已经升至顶级")
			});
		}
		else if (level >= num)
		{
			this.mPanelShengJiAndShengJie.Refresh(RidePetUpType.AddLevel, goodsData, level);
			this.mPanelShengJiAndShengJie.AddLevelStr = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				string.Concat(new object[]
				{
					Global.GetLang("人物"),
					array[0],
					Global.GetLang("转"),
					array[1],
					Global.GetLang("级后"),
					Environment.NewLine,
					Global.GetLang("坐骑等级上限提升至"),
					array[2],
					Global.GetLang("级")
				})
			});
		}
		else
		{
			this.mPanelShengJiAndShengJie.Refresh(RidePetUpType.UpLevel, goodsData, level);
		}
	}

	private void AddStarRefresh(GoodsData goodsData)
	{
		if (IConfigbase<ConfigRidePet>.Instance.MaxStar <= goodsData.Forge_level + 1)
		{
			this.mPanelShengJiAndShengJie.Refresh(RidePetUpType.MaxStar, goodsData, this.CaoWeiLevel);
		}
		else
		{
			this.mPanelShengJiAndShengJie.Refresh(RidePetUpType.UpStar, goodsData, this.CaoWeiLevel);
		}
	}

	internal void RefreshHorseUseing()
	{
		this.InitHorseData(true);
	}

	internal void NotcivGetMainDataCallBack(ZuoQiMainData data)
	{
		if (data != null)
		{
			this.mHaveAvtiviteList.Clear();
			if (data.MountList != null)
			{
				for (int i = 0; i < data.MountList.Count; i++)
				{
					if (data.MountList[i] != null)
					{
						this.mHaveAvtiviteList.Add(data.MountList[i].GoodsID);
					}
				}
			}
		}
		this.GetData(data);
	}

	internal void NoticeRideUpStarCallBack(bool falg = false)
	{
		RidePetItem selectItem = this.GetSelectItem();
		if (null != selectItem && selectItem.GoodsData != null)
		{
			GoodsData roleHorseGoodsDataInBeiZhanBaoGuoByDbId = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, selectItem.GoodsData.Id, 1);
			if (roleHorseGoodsDataInBeiZhanBaoGuoByDbId != null)
			{
				if (falg)
				{
					this.mPanelShengJiAndShengJie.AddTeXiao();
					base.StartCoroutine<bool>(this.YanChiShengJie(roleHorseGoodsDataInBeiZhanBaoGuoByDbId));
					return;
				}
				this.AddStarRefresh(roleHorseGoodsDataInBeiZhanBaoGuoByDbId);
				this.RefreshShuXing(roleHorseGoodsDataInBeiZhanBaoGuoByDbId);
				this.RefreshMode(roleHorseGoodsDataInBeiZhanBaoGuoByDbId);
			}
			selectItem.SetData(roleHorseGoodsDataInBeiZhanBaoGuoByDbId, RidePetUIType.ZuoQi);
		}
	}

	private IEnumerator YanChiShengJie(GoodsData data)
	{
		yield return new WaitForSeconds(0.8f);
		this.AddStarRefresh(data);
		this.RefreshShuXing(data);
		this.RefreshMode(data);
		yield break;
	}

	private IEnumerator YanChiShengJi(GoodsData data)
	{
		yield return new WaitForSeconds(0.8f);
		this.AddUpRefresh(data, this.CaoWeiLevel);
		yield break;
	}

	internal void NoticeRideUpLevelCallBack(int Level, bool falg = false)
	{
		RidePetItem selectItem = this.GetSelectItem();
		if (null != selectItem)
		{
			GoodsData roleHorseGoodsDataInBeiZhanBaoGuoByDbId = Global.GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(Global.Data.RoleID, selectItem.GoodsData.Id, 1);
			if (roleHorseGoodsDataInBeiZhanBaoGuoByDbId != null)
			{
				this.RefreshShuXing(roleHorseGoodsDataInBeiZhanBaoGuoByDbId);
				if (falg)
				{
					this.mPanelShengJiAndShengJie.AddTeXiaoLevel();
					base.StartCoroutine<bool>(this.YanChiShengJi(roleHorseGoodsDataInBeiZhanBaoGuoByDbId));
					return;
				}
			}
		}
	}

	internal void NOticeRideUnLoadCallBack(GoodsData goodsData)
	{
	}

	public DPSelectedItemEventHandler Hander;

	public ListBox mListBox;

	public UILabel mLabTitleName;

	public UILabel mLabTitleLV;

	[SerializeField]
	private GameObject PrppSkillInfRoot;

	public UILabel mLabSkillTitle;

	public ShowNetImage mShowSkill;

	public UILabel mLabSkillContent;

	public UILabel mLabSkillLV;

	public UILabel mLabShuXingTitle;

	public UILabel mLabShuXingContent;

	public UILabel mLabZhuoYueTitle;

	public UILabel mLabZhuoYueContent;

	public GButton mBtnShengJi;

	public GButton mBtnShengJie;

	public GButton mBtnLieQu;

	public GButton mBtnQiYuan;

	public GButton mBtnZuHe;

	public GButton mBtnBeiBao;

	public UISprite mBtnBeiBaoHongDian;

	public GButton mBtnZhaoHui;

	public Modal3DShow mGameModal;

	public ShengJiAndShengJiePart mPanelShengJiAndShengJie;

	public GButton mBtnCloseShengJiAndShengJie;

	[SerializeField]
	private UIDraggablePanel mDraggablePanel;

	[SerializeField]
	private Transform _AttViewRoot;

	[SerializeField]
	private UISprite mUpSp;

	private List<GameObject> mUpSpList = new List<GameObject>();

	private ObservableCollection mCollection;

	private ZuoQiMainData mZuoQiMainData;

	private float SkillAttBeginPosY = -22f;

	private RideZuHeSowPart mRideZuHeSowPart;

	private GChildWindow mRideZuHeSowPartWind;

	private List<HorseResLoader> mHorseResLoaderList = new List<HorseResLoader>();

	private float mUseHorseCD;

	private List<int> mHaveAvtiviteList = new List<int>();
}
