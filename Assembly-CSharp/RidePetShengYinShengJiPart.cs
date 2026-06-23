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

public class RidePetShengYinShengJiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitPage();
		this.InitCheckBox();
		this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, 0);
		this.RefreshBagPage();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		for (int i = 0; i < this.mStarlist.Count; i++)
		{
			if (null != this.mStarlist[i])
			{
				Object.Destroy(this.mStarlist[i]);
			}
		}
		IConfigbase<ConfigShengYinShengJi>.Instance.Hander = null;
	}

	private GameObject GetStar()
	{
		for (int i = 0; i < this.mStarlist.Count; i++)
		{
			if (null != this.mStarlist[i] && !NGUITools.GetActive(this.mStarlist[i]))
			{
				this.mStarlist[i].SetActive(true);
				return this.mStarlist[i];
			}
		}
		this.mStarlist.Add(Object.Instantiate<GameObject>(this._PropStarSp.gameObject));
		this.mStarlist[this.mStarlist.Count - 1].transform.SetParent(this._PropStarSp.transform.parent, false);
		this.mStarlist[this.mStarlist.Count - 1].SetActive(true);
		return this.mStarlist[this.mStarlist.Count - 1];
	}

	private void RrfreshProp(int Level, int NextLevel)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		for (int i = 0; i < this.mStarlist.Count; i++)
		{
			if (null != this.mStarlist[i] && NGUITools.GetActive(this.mStarlist[i]))
			{
				this.mStarlist[i].SetActive(false);
			}
		}
		if (this.mSelectGoodsData != null)
		{
			Dictionary<string, double> propByGoodsIDAndLevel = IConfigbase<ConfigShengYinShengJi>.Instance.GetPropByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, Level);
			Dictionary<string, double> dictionary = null;
			if (propByGoodsIDAndLevel != null)
			{
				if (Level < IConfigbase<ConfigShengYinShengJi>.Instance.GetMaxLevelByGoodsID(this.mSelectGoodsData.GoodsID) && Level < NextLevel)
				{
					dictionary = IConfigbase<ConfigShengYinShengJi>.Instance.GetPropByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, NextLevel);
				}
				List<byte> list = new List<byte>();
				Dictionary<string, double>.Enumerator enumerator = propByGoodsIDAndLevel.GetEnumerator();
				while (enumerator.MoveNext())
				{
					string text4 = text;
					KeyValuePair<string, double> keyValuePair = enumerator.Current;
					text = text4 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(keyValuePair.Key, true) + "\n";
					KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
					if (ConfigExtPropIndexes.GetPercentByWord(keyValuePair2.Key))
					{
						KeyValuePair<string, double> keyValuePair3 = enumerator.Current;
						text2 = (keyValuePair3.Value * 100.0).ToString("f1") + "%\n";
						if (dictionary != null)
						{
							Dictionary<string, double> dictionary2 = dictionary;
							KeyValuePair<string, double> keyValuePair4 = enumerator.Current;
							if (dictionary2.ContainsKey(keyValuePair4.Key))
							{
								string text5 = text3;
								Dictionary<string, double> dictionary3 = dictionary;
								KeyValuePair<string, double> keyValuePair5 = enumerator.Current;
								text3 = text5 + (Math.Abs(dictionary3[keyValuePair5.Key]) * 100.0).ToString("f1") + "%\n";
								list.Add(1);
							}
							else
							{
								list.Add(0);
							}
						}
					}
					else
					{
						string text6 = text2;
						KeyValuePair<string, double> keyValuePair6 = enumerator.Current;
						text2 = text6 + keyValuePair6.Value.ToString("f0") + "\n";
						if (dictionary != null)
						{
							Dictionary<string, double> dictionary4 = dictionary;
							KeyValuePair<string, double> keyValuePair7 = enumerator.Current;
							if (dictionary4.ContainsKey(keyValuePair7.Key))
							{
								string text7 = text3;
								Dictionary<string, double> dictionary5 = dictionary;
								KeyValuePair<string, double> keyValuePair8 = enumerator.Current;
								text3 = text7 + Math.Abs(dictionary5[keyValuePair8.Key]).ToString("f0") + "\n";
								list.Add(1);
							}
							else
							{
								list.Add(0);
							}
						}
					}
				}
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j] == 1)
					{
						GameObject star = this.GetStar();
						Vector3 localPosition = star.transform.localPosition;
						localPosition.y = 48f + (float)j * -26f;
						star.transform.localPosition = localPosition;
					}
				}
			}
		}
		this._PropNameLabel.text = text;
		this._PropValueLabel.text = text2;
		this._PropNextValueLabel.text = text3;
		this._PropColliderControl.updataNow = true;
	}

	private void InitCheckBox()
	{
		this._CheckBoxs[1].isChecked = false;
		this._CheckBoxs[2].isChecked = false;
		this._CheckBoxs[3].isChecked = false;
		this._CheckBoxs[0].isChecked = true;
		this._CheckBoxs[0].CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (this._CheckBoxs[0].isChecked)
			{
				for (byte b = 1; b < 4; b += 1)
				{
					this._CheckBoxs[(int)b].isChecked = false;
				}
			}
			else
			{
				byte b2 = 1;
				for (byte b3 = 1; b3 < 4; b3 += 1)
				{
					if (this._CheckBoxs[(int)b3].isChecked)
					{
						b2 = 0;
						break;
					}
				}
				if (b2 == 1)
				{
					this._CheckBoxs[0].isChecked = true;
					return;
				}
			}
			if (Global.Data != null && Global.Data.roleData.HolyGoodsDataList != null)
			{
				this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, 0);
			}
			this.CheckRoleSleletGoods();
		};
		this._CheckBoxs[1].CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (!this._CheckBoxs[1].isChecked || !this._CheckBoxs[2].isChecked || !this._CheckBoxs[3].isChecked)
			{
				this._CheckBoxs[0].isChecked = false;
			}
			for (byte b = 1; b < 4; b += 1)
			{
				if (b != 1)
				{
					this._CheckBoxs[(int)b].isChecked = false;
				}
			}
			byte b2 = 1;
			for (byte b3 = 1; b3 < 4; b3 += 1)
			{
				if (this._CheckBoxs[(int)b3].isChecked)
				{
					b2 = 0;
					break;
				}
			}
			if (b2 == 1)
			{
				this._CheckBoxs[0].isChecked = true;
				this._CheckBoxs[0].CheckChanged(null, null);
			}
			if (Global.Data != null && Global.Data.roleData.HolyGoodsDataList != null)
			{
				this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, 0);
			}
			this.CheckRoleSleletGoods();
		};
		this._CheckBoxs[2].CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (!this._CheckBoxs[1].isChecked || !this._CheckBoxs[2].isChecked || !this._CheckBoxs[3].isChecked)
			{
				this._CheckBoxs[0].isChecked = false;
			}
			for (byte b = 1; b < 4; b += 1)
			{
				if (b != 2)
				{
					this._CheckBoxs[(int)b].isChecked = false;
				}
			}
			byte b2 = 1;
			for (byte b3 = 1; b3 < 4; b3 += 1)
			{
				if (this._CheckBoxs[(int)b3].isChecked)
				{
					b2 = 0;
					break;
				}
			}
			if (b2 == 1)
			{
				this._CheckBoxs[0].isChecked = true;
				this._CheckBoxs[0].CheckChanged(null, null);
			}
			if (Global.Data != null && Global.Data.roleData.HolyGoodsDataList != null)
			{
				this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, 0);
			}
			this.CheckRoleSleletGoods();
		};
		this._CheckBoxs[3].CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (!this._CheckBoxs[1].isChecked || !this._CheckBoxs[2].isChecked || !this._CheckBoxs[3].isChecked)
			{
				this._CheckBoxs[0].isChecked = false;
			}
			for (byte b = 1; b < 4; b += 1)
			{
				if (b != 3)
				{
					this._CheckBoxs[(int)b].isChecked = false;
				}
			}
			byte b2 = 1;
			for (byte b3 = 1; b3 < 4; b3 += 1)
			{
				if (this._CheckBoxs[(int)b3].isChecked)
				{
					b2 = 0;
					break;
				}
			}
			if (b2 == 1)
			{
				this._CheckBoxs[0].isChecked = true;
				this._CheckBoxs[0].CheckChanged(null, null);
			}
			if (Global.Data != null && Global.Data.roleData.HolyGoodsDataList != null)
			{
				this.RefreshBag(Global.Data.roleData.HolyGoodsDataList, 0);
			}
			this.CheckRoleSleletGoods();
		};
		this._CheckBoxs[0].Text = Global.GetLang("全选");
		this._CheckBoxs[1].Text = Global.GetLang("紫色");
		this._CheckBoxs[2].Text = Global.GetLang("蓝色");
		this._CheckBoxs[3].Text = Global.GetLang("绿色");
	}

	private void CheckRoleSleletGoods()
	{
		this.SetAllNotSelect();
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
		newGoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		newGoodIcon.Width = 78.0;
		newGoodIcon.Height = 78.0;
		if (goodsData != null)
		{
			newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				Super.GetIconCode(goodsData.GoodsID)
			}), false, 0);
			newGoodIcon.ItemCode = goodsData.GoodsID;
			newGoodIcon.ItemObject = goodsData;
			newGoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		}
		else
		{
			newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				-1
			}), false, 0);
			newGoodIcon.ItemCode = -1;
			newGoodIcon.ItemObject = goodsData;
			newGoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(-1);
		}
		newGoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
		{
		};
		return newGoodIcon;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData gd = ggoodIcon.ItemObject as GoodsData;
		if (gd == null)
		{
			return;
		}
		if (this.mSelectGoodsData != null)
		{
			if (gd.Id == this.mSelectGoodsData.Id)
			{
				return;
			}
			if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 980)
			{
				if (this.mSelectGoodsID.ContainsKey(gd.Id) && this.mSelectGoodsID[gd.Id] != null)
				{
					this.mSelectGoodsID[gd.Id] = null;
					this.SetIconSelectState(ggoodIcon, false);
				}
				else
				{
					this.mSelectGoodsID[gd.Id] = gd;
					this.SetIconSelectState(ggoodIcon, true);
				}
			}
			else if (this.mSelectGoodsID.ContainsKey(gd.Id) && this.mSelectGoodsID[gd.Id] != null)
			{
				this.mSelectGoodsID[gd.Id] = null;
				this.SetIconSelectState(ggoodIcon, false);
			}
			else
			{
				this.mSelectGoodsID[gd.Id] = gd;
				this.SetIconSelectState(ggoodIcon, true);
			}
		}
		else if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 980)
		{
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, (!this.mSelectGoodsID.ContainsKey(gd.Id) || this.mSelectGoodsID[gd.Id] == null) ? GoodsOwnerTypes.HolyBagShengJi : GoodsOwnerTypes.HolyBagShengJiXuanZhong, gd);
			GTipServiceEx.TipsSender.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs args)
			{
				int id = gd.Id;
				TipsOperationTypes idtype = (TipsOperationTypes)args.IDType;
				if (idtype == TipsOperationTypes.Fangru)
				{
					this.SetPartSelectGoods(gd.Id);
				}
			};
		}
		else
		{
			Super.HintMainText(Global.GetLang("请添加要升级的道具"), 10, 3);
		}
	}

	private void SetIconSelectState(GGoodIcon gIcon, bool state)
	{
		if (!state)
		{
			gIcon.BackgroundSprite15.spriteName = string.Empty;
			gIcon.BackgroundSprite15.gameObject.SetActive(false);
		}
		else
		{
			gIcon.BackgroundSprite15.spriteName = "iconState_highlight";
			gIcon.BackgroundSprite15.gameObject.SetActive(true);
			gIcon.BackgroundSprite15.transform.localScale = new Vector3(78f, 78f, 1f);
			gIcon.BackgroundSprite15.transform.localPosition = new Vector3(0f, 0f, -0.01f);
		}
		this.RefreshEXP();
	}

	private void RefreshEXP()
	{
		int num = 0;
		int num2 = 1;
		int num3 = 1;
		int num4 = 0;
		int num5 = 0;
		if (this.mSelectGoodsData != null)
		{
			num = this.mSelectGoodsData.ElementhrtsProps[1];
			num2 = this.mSelectGoodsData.ElementhrtsProps[0];
			num3 = IConfigbase<ConfigShengYinShengJi>.Instance.GetMaxLevelByGoodsID(this.mSelectGoodsData.GoodsID);
			num4 = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengJiNeedXEPByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, num2);
		}
		if (1 < num2 && num3 > num2 && this.mSelectGoodsData != null)
		{
			int shengJiNeedXEPByGoodsIDAndLevel = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengJiNeedXEPByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, num2 - 1);
			num -= shengJiNeedXEPByGoodsIDAndLevel;
			num4 -= shengJiNeedXEPByGoodsIDAndLevel;
		}
		foreach (KeyValuePair<int, GoodsData> keyValuePair in this.mSelectGoodsID)
		{
			if (keyValuePair.Value != null)
			{
				ConfigShengYinShengJi instance = IConfigbase<ConfigShengYinShengJi>.Instance;
				Dictionary<int, GoodsData>.Enumerator enumerator;
				KeyValuePair<int, GoodsData> keyValuePair2 = enumerator.Current;
				int goodsID = keyValuePair2.Value.GoodsID;
				KeyValuePair<int, GoodsData> keyValuePair3 = enumerator.Current;
				ShengYinShengJiVO shengYinShengJiVODataByGoodsIdAndLevel = instance.GetShengYinShengJiVODataByGoodsIdAndLevel(goodsID, keyValuePair3.Value.ElementhrtsProps[0]);
				if (shengYinShengJiVODataByGoodsIdAndLevel != null && 0 < shengYinShengJiVODataByGoodsIdAndLevel.TunShiJingYan)
				{
					int num6 = num;
					int tunShiJingYan = shengYinShengJiVODataByGoodsIdAndLevel.TunShiJingYan;
					KeyValuePair<int, GoodsData> keyValuePair4 = enumerator.Current;
					num = num6 + tunShiJingYan * keyValuePair4.Value.GCount;
				}
			}
		}
		this._LevelLabel1.text = num2.ToString();
		if (num2 + 1 > num3)
		{
			this._LevelJianTouObj.SetActive(false);
			this._LevelLabel2.text = string.Empty;
			if (this._UpLevelBtn.isEnabled)
			{
				this._UpLevelBtn.isEnabled = false;
			}
		}
		else
		{
			num5 = num2;
			int shengJiNeedXEPByGoodsIDAndLevel2 = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengJiNeedXEPByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, num2 - 1);
			for (byte b = (byte)num2; b < 51; b += 1)
			{
				int shengJiNeedXEPByGoodsIDAndLevel3 = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengJiNeedXEPByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, (int)b);
				if (shengJiNeedXEPByGoodsIDAndLevel2 + num >= shengJiNeedXEPByGoodsIDAndLevel3)
				{
					num5 = (int)(b + 1);
				}
			}
			if (num3 <= num5)
			{
				num5 = num3;
			}
			if (num5 == num2)
			{
				this._LevelJianTouObj.SetActive(false);
				this._LevelLabel2.text = string.Empty;
			}
			else
			{
				this._LevelJianTouObj.SetActive(true);
				this._LevelLabel2.text = num5.ToString();
			}
			if (!this._UpLevelBtn.isEnabled)
			{
				this._UpLevelBtn.isEnabled = true;
			}
		}
		if (this.mSelectGoodsData == null)
		{
			this._LevelEXPLabel.text = string.Empty;
			this._EXPProgressBar.Percent = 0.0;
		}
		else if (num2 + 1 > num3)
		{
			this._LevelEXPLabel.text = "Max";
		}
		else
		{
			this._LevelEXPLabel.text = num + "/" + num4;
			this._EXPProgressBar.Percent = (double)num / (double)num4;
		}
		this.RrfreshProp(num2, num5);
	}

	private int Getindex(int dex)
	{
		int num = 50;
		this._GGoodsBox.listBox.maxPerLine = num;
		int num2 = dex / 5 / 4;
		int num3 = dex % 20;
		int num4 = num3 % 5;
		int num5 = num3 / 5 % 4;
		return num4 + num5 * num + num2 * 5;
	}

	private void GoPage(int Page)
	{
		if (0 >= Page)
		{
			Page = 0;
		}
		if (10 <= Page)
		{
			Page = 9;
		}
		this.CurrentSelectedPage = Page;
		this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPage();
	}

	private void InitPage()
	{
		this._GGoodsBox.RowCount = 4;
		this._GGoodsBox.ColCount = 50;
		this._GGoodsBox.InitBox();
	}

	private void RefreshBagPage()
	{
		if (this.mTempPaneStat != null)
		{
			this.mTempPaneStat.spriteName = "selectState_normal2";
		}
		this._Pages[this.CurrentSelectedPage].spriteName = "selectState_hover2";
		this.mTempPaneStat = this._Pages[this.CurrentSelectedPage];
	}

	private void InitPrefabText()
	{
		try
		{
			this._UpLevelBtn.Text = Global.GetLang("升级");
			this._AllSelectBtn.Text = Global.GetLang("全选");
			this._TrimBtn.Text = Global.GetLang("整理");
			this._PutOffSelectGoodsBtn.Text = Global.GetLang("卸下");
			this._LevelLabel.text = Global.GetLang("强化等级");
			this._LevelLabel1.text = "0";
			this._LevelLabel2.text = "0";
			this._EXPProgressBar.Percent = 0.0;
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
			this.UIDragPl.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this._TrimBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._TrimBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._TrimBtn.GetInstanceID(), 1f);
				GameInstance.Game.SendRidePetShengYinTrimBag();
			};
			this._AllSelectBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._AllSelectBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._AllSelectBtn.GetInstanceID(), 1f);
				base.StartCoroutine<bool>(this.SelectAllGoods());
			};
			this._UpLevelBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				RidePetShengYinShengJiPart.<InitHandler>c__AnonStorey37B <InitHandler>c__AnonStorey37B = new RidePetShengYinShengJiPart.<InitHandler>c__AnonStorey37B();
				<InitHandler>c__AnonStorey37B.<>f__this = this;
				if (0f < Global.GetBtnCD(this._UpLevelBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._UpLevelBtn.GetInstanceID(), 0.5f);
				if (this.mSelectGoodsData == null)
				{
					Super.HintMainText(Global.GetLang("请选择要升级的道具"), 10, 3);
					return;
				}
				<InitHandler>c__AnonStorey37B.GoodsStr = string.Empty;
				int num = 0;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(this.mSelectGoodsData.GoodsID);
				foreach (KeyValuePair<int, GoodsData> keyValuePair in this.mSelectGoodsID)
				{
					if (keyValuePair.Value != null)
					{
						Dictionary<int, GoodsData>.Enumerator enumerator;
						KeyValuePair<int, GoodsData> keyValuePair2 = enumerator.Current;
						GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(keyValuePair2.Value.GoodsID);
						KeyValuePair<int, GoodsData> keyValuePair3 = enumerator.Current;
						if (keyValuePair3.Value.ElementhrtsProps[0] > 1 && goodsXmlNodeByID2.Categoriy != 981)
						{
							num++;
						}
						if (goodsXmlNodeByID2 != null && goodsXmlNodeByID.SuitID < goodsXmlNodeByID2.SuitID && goodsXmlNodeByID2.Categoriy != 981)
						{
							num++;
						}
						RidePetShengYinShengJiPart.<InitHandler>c__AnonStorey37B <InitHandler>c__AnonStorey37B2 = <InitHandler>c__AnonStorey37B;
						string goodsStr = <InitHandler>c__AnonStorey37B.GoodsStr;
						object[] array = new object[5];
						array[0] = goodsStr;
						int num2 = 1;
						KeyValuePair<int, GoodsData> keyValuePair4 = enumerator.Current;
						array[num2] = keyValuePair4.Value.Id;
						array[2] = ",";
						int num3 = 3;
						KeyValuePair<int, GoodsData> keyValuePair5 = enumerator.Current;
						array[num3] = keyValuePair5.Value.GCount;
						array[4] = "|";
						<InitHandler>c__AnonStorey37B2.GoodsStr = string.Concat(array);
					}
				}
				if (0 < num)
				{
					string[] buttons = new string[]
					{
						Global.GetLang("确认"),
						Global.GetLang("取消")
					};
					Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("被作为材料的圣印中有") + num + Global.GetLang("个材料比当前所升级的圣印品质高，是否继续"), delegate(object h, DPSelectedItemEventArgs d)
					{
						if (d.ID == 0)
						{
							if (!string.Empty.Equals(<InitHandler>c__AnonStorey37B.GoodsStr))
							{
								GameInstance.Game.SendRidePetShengYinShengJi(<InitHandler>c__AnonStorey37B.<>f__this.mSelectGoodsData.Id, <InitHandler>c__AnonStorey37B.GoodsStr);
							}
							else
							{
								Super.HintMainText(Global.GetLang("可吞噬的道具个数为0"), 10, 3);
							}
						}
					}, buttons);
				}
				else if (!string.Empty.Equals(<InitHandler>c__AnonStorey37B.GoodsStr))
				{
					GameInstance.Game.SendRidePetShengYinShengJi(this.mSelectGoodsData.Id, <InitHandler>c__AnonStorey37B.GoodsStr);
				}
				else
				{
					Super.HintMainText(Global.GetLang("可吞噬的道具个数为0"), 10, 3);
				}
			};
			this._PutOffSelectGoodsBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mSelectGoodsData != null)
				{
					this.SetPartSelectGoods(-1);
					this.mIsAllSelect = 0;
					this._AllSelectBtn.Text = Global.GetLang("全选");
					for (int i = 0; i < 200; i++)
					{
						GGoodIcon goodsIcon = this._GGoodsBox.GetGoodsIcon(this.Getindex(i));
						if (null != goodsIcon)
						{
							GoodsData goodsData = goodsIcon.ItemObject as GoodsData;
							if (goodsData != null)
							{
								this.mSelectGoodsID[goodsData.Id] = null;
								if (goodsData.Using == 0 && 0 < goodsData.GCount)
								{
									this.SetIconSelectState(goodsIcon, false);
								}
								else if (goodsData.Using == 1)
								{
									this.SetIconSelectState(goodsIcon, false);
								}
							}
						}
					}
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

	private void SetAllNotSelect()
	{
		this.mIsAllSelect = 1;
		base.StartCoroutine<bool>(this.SelectAllGoods());
	}

	private IEnumerator SelectAllGoods()
	{
		this.mSelectGoodsID.Clear();
		byte RefreshCount = 0;
		if (this.mIsAllSelect == 0)
		{
			this.mIsAllSelect = 1;
			this._AllSelectBtn.Text = Global.GetLang("取消");
			int i = this.CurrentSelectedPage * 20;
			while (i < this.CurrentSelectedPage * 20 + 20)
			{
				GGoodIcon icon = this._GGoodsBox.GetGoodsIcon(this.Getindex(i));
				if (!(null != icon))
				{
					goto IL_20D;
				}
				GoodsData gd = icon.ItemObject as GoodsData;
				if (gd == null)
				{
					goto IL_20D;
				}
				if (this.mSelectGoodsData == null || gd.Id != this.mSelectGoodsData.Id)
				{
					if (gd.Using == 0 && 0 < gd.GCount)
					{
						this.mSelectGoodsID[gd.Id] = gd;
						this.SetIconSelectState(icon, true);
						RefreshCount += 1;
						goto IL_20D;
					}
					if (gd.Using == 1)
					{
						this.mSelectGoodsID[gd.Id] = null;
						this.SetIconSelectState(icon, false);
						RefreshCount += 1;
						goto IL_20D;
					}
					MUDebug.Log<string>(new string[]
					{
						Global.GetGoodsNameByID(gd.GoodsID, false) + " count = " + gd.GCount
					});
					goto IL_20D;
				}
				IL_234:
				i++;
				continue;
				IL_20D:
				if (20 <= RefreshCount)
				{
					RefreshCount = 0;
					yield return null;
					goto IL_234;
				}
				goto IL_234;
			}
		}
		else
		{
			this.mIsAllSelect = 0;
			this._AllSelectBtn.Text = Global.GetLang("全选");
			for (int j = 0; j < 200; j++)
			{
				GGoodIcon icon2 = this._GGoodsBox.GetGoodsIcon(this.Getindex(j));
				if (null != icon2)
				{
					GoodsData gd2 = icon2.ItemObject as GoodsData;
					if (gd2 != null)
					{
						this.mSelectGoodsID[gd2.Id] = null;
						if (gd2.Using == 0 && 0 < gd2.GCount)
						{
							this.SetIconSelectState(icon2, false);
							RefreshCount += 1;
						}
						else if (gd2.Using == 1)
						{
							this.SetIconSelectState(icon2, false);
							RefreshCount += 1;
						}
					}
				}
				if (20 <= RefreshCount)
				{
					RefreshCount = 0;
					yield return null;
				}
			}
		}
		this.RefreshEXPAndLevel(-1, 0);
		yield break;
	}

	private void onDragFinished()
	{
		if (Math.Abs(Math.Abs(this.UIDragPl.transform.localPosition.x) - (float)(390 * this.CurrentSelectedPage)) > 30f)
		{
			if (this.UIDragPl.transform.localPosition.x > (float)(-390 * this.CurrentSelectedPage))
			{
				this.CurrentSelectedPage--;
				if (this.CurrentSelectedPage <= 0)
				{
					this.CurrentSelectedPage = 0;
				}
			}
			else
			{
				this.CurrentSelectedPage++;
				if (this.CurrentSelectedPage >= 10)
				{
					this.CurrentSelectedPage = 9;
				}
			}
		}
		this.UIDragPl.target.x = (float)(-390 * this.CurrentSelectedPage);
		this.UIDragPl.enabled = true;
		this.RefreshBagPage();
	}

	private void RefreshEXPAndLevel(int OldLevel, int OldExp)
	{
		if (this.mSelectGoodsData == null)
		{
			this._LevelJianTouObj.SetActive(false);
			this._LevelLabel1.text = "1";
			this._LevelLabel2.text = string.Empty;
			this._EXPProgressBar.Percent = 0.0;
			return;
		}
		int num = this.mSelectGoodsData.ElementhrtsProps[0];
		int shengJiNeedXEPByGoodsIDAndLevel = IConfigbase<ConfigShengYinShengJi>.Instance.GetShengJiNeedXEPByGoodsIDAndLevel(this.mSelectGoodsData.GoodsID, num);
		int maxLevelByGoodsID = IConfigbase<ConfigShengYinShengJi>.Instance.GetMaxLevelByGoodsID(this.mSelectGoodsData.GoodsID);
		this.RefreshEXP();
		if (0 <= OldLevel && OldLevel != num)
		{
			this._ShengJiTeXiao.SetActive(false);
			this._ShengJiTeXiao.SetActive(true);
		}
	}

	public void SetPartSelectGoods(int DBID)
	{
		if (Global.Data == null)
		{
			return;
		}
		GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(DBID, Global.Data.roleData.HolyGoodsDataList);
		GoodsData goodsData = null;
		if (this.mSelectGoodsData != null)
		{
			goodsData = this.mSelectGoodsData.Clone();
		}
		if (null == this.mNeedUpLevelGoodsIocn)
		{
			this.mNeedUpLevelGoodsIocn = this.AddIcon(goodsDataByDbID);
			this.mNeedUpLevelGoodsIocn.transform.SetParent(this._NeedUpLevelGoodsRoot.transform, false);
			this.mNeedUpLevelGoodsIocn.transform.localPosition = Vector2.zero;
		}
		this.mSelectGoodsData = goodsDataByDbID;
		this.SetAllNotSelect();
		if (goodsDataByDbID != null)
		{
			this.mNeedUpLevelGoodsIocn.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				Super.GetIconCode(goodsDataByDbID.GoodsID)
			}), false, 0);
			Super.InitGoodsGIcon(this.mNeedUpLevelGoodsIocn, goodsDataByDbID, true, IconTextTypes.Qianghua);
		}
		else
		{
			this.mNeedUpLevelGoodsIocn.GoodImg.SetTexture(null);
			this.mNeedUpLevelGoodsIocn.BackgroundSprite1.spriteName = string.Empty;
			if (null != this.mNeedUpLevelGoodsIocn.TeXiao)
			{
				this.mNeedUpLevelGoodsIocn.TeXiao.gameObject.SetActive(false);
			}
		}
		if (!this.mNeedUpLevelGoodsIocn.GoodImg.enabled)
		{
			this.mNeedUpLevelGoodsIocn.GoodImg.enabled = true;
		}
		this.mNeedUpLevelGoodsIocn.BackgroundSprite15.transform.localScale = new Vector3(78f, 78f, 1f);
		this.mNeedUpLevelGoodsIocn.BackgroundSprite15.gameObject.SetActive(true);
		this.mNeedUpLevelGoodsIocn.BackgroundSprite15.spriteName = "bagGrid4_bak";
		if (NGUITools.GetActive(this.mNeedUpLevelGoodsIocn.BackgroundSprite1.gameObject))
		{
			this.mNeedUpLevelGoodsIocn.BackgroundSprite1.depth = this.mNeedUpLevelGoodsIocn.BackgroundSprite15.depth + 1;
		}
		this.RefreshEXPAndLevel(-1, 0);
		if (goodsData != null && goodsData.Using != 1)
		{
			this.RefreshGoods(goodsData);
		}
		if (this.mSelectGoodsData != null)
		{
			this.RefreshGoods(this.mSelectGoodsData);
		}
	}

	public void RefreshGoods(GoodsData goodsData)
	{
		if (goodsData != null)
		{
			if (this.mSelectGoodsData != null && this.mSelectGoodsData.Id == goodsData.Id)
			{
				int num = this._GGoodsBox.FindByGoodsDbID(goodsData.Id);
				if (num != -1)
				{
					GGoodIcon goodsIcon = this._GGoodsBox.GetGoodsIcon(num);
					if (null != goodsIcon)
					{
						bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
						goodsIcon.ItemObject = goodsData;
						Super.InitGoodsGIcon(goodsIcon, goodsData, canUse, IconTextTypes.Qianghua);
						goodsIcon.GoodImg.ToGrayBitmap = true;
					}
				}
				return;
			}
			int num2 = this._GGoodsBox.FindByGoodsDbID(goodsData.Id);
			if (num2 != -1)
			{
				if (goodsData.GCount <= 0 || goodsData.Using > 0)
				{
					this._GGoodsBox.RemoveGoodsIcon(num2);
				}
				else
				{
					GGoodIcon goodsIcon2 = this._GGoodsBox.GetGoodsIcon(num2);
					if (null != goodsIcon2)
					{
						bool canUse2 = Global.CanUseGoods(goodsData.GoodsID, false, true);
						Super.InitGoodsGIcon(goodsIcon2, goodsData, canUse2, IconTextTypes.Qianghua);
						goodsIcon2.GoodImg.ToGrayBitmap = false;
					}
				}
				return;
			}
			if (!this._CheckBoxs[0].isChecked)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int suitID = goodsXmlNodeByID.SuitID;
					if (suitID <= 3)
					{
						if (!this._CheckBoxs[3].isChecked)
						{
							return;
						}
					}
					else if (suitID == 4)
					{
						if (!this._CheckBoxs[2].isChecked)
						{
							return;
						}
					}
					else if (suitID > 4 && suitID <= 6 && !this._CheckBoxs[1].isChecked)
					{
						return;
					}
				}
			}
			if (goodsData.GCount > 0 || goodsData.Using == 0)
			{
				int num3 = -1;
				for (int i = 0; i < 200; i++)
				{
					if (null == this._GGoodsBox.GetGoodsIcon(this.Getindex(i)))
					{
						num3 = i;
						break;
					}
				}
				if (0 <= num3)
				{
					GGoodIcon ggoodIcon = this.AddIcon(goodsData);
					this._GGoodsBox.SetGoodsIcon(this.Getindex(num3), ggoodIcon);
					ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
					bool canUse3 = Global.CanUseGoods(goodsData.GoodsID, false, true);
					Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse3, IconTextTypes.Qianghua);
				}
			}
		}
	}

	public void RefreshBag(List<GoodsData> BagDataList, byte AllNetSelet = 0)
	{
		List<GoodsData> list = new List<GoodsData>();
		if (BagDataList != null)
		{
			for (int i = 0; i < BagDataList.Count; i++)
			{
				list.Add(BagDataList[i]);
			}
		}
		list.Sort((GoodsData a, GoodsData b) => -(a.BagIndex - b.BagIndex));
		base.StartCoroutine<bool>(this.RefreshBagEX(list, AllNetSelet));
	}

	private IEnumerator RefreshBagEX(List<GoodsData> BagDataList, byte AllNetSelet)
	{
		int Index = 0;
		int selectID = -1;
		if (this.mSelectGoodsData != null)
		{
			selectID = this.mSelectGoodsData.Id;
		}
		Dictionary<int, GoodsData> indexDict = new Dictionary<int, GoodsData>();
		if (BagDataList != null && 0 < BagDataList.Count)
		{
			bool AllCheckBoxIsFalse = !this._CheckBoxs[0].isChecked && !this._CheckBoxs[1].isChecked && !this._CheckBoxs[2].isChecked && !this._CheckBoxs[3].isChecked;
			int subIndex = 0;
			int max = BagDataList.Count;
			while (subIndex < max)
			{
				GoodsData gd = BagDataList[subIndex];
				if (gd != null)
				{
					if (gd.Using != 1)
					{
						if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 981 && gd.GCount > 0 && gd.GCount > 0)
						{
							if (this._CheckBoxs[0].isChecked)
							{
								Dictionary<int, GoodsData> dictionary = indexDict;
								int num;
								Index = (num = Index) + 1;
								dictionary[num] = gd;
							}
							else if (AllCheckBoxIsFalse)
							{
								Dictionary<int, GoodsData> dictionary2 = indexDict;
								int num;
								Index = (num = Index) + 1;
								dictionary2[num] = gd;
							}
							else
							{
								indexDict[Index] = gd;
								Index++;
							}
						}
					}
				}
				subIndex++;
			}
			int subIndex2 = 0;
			int max2 = BagDataList.Count;
			while (subIndex2 < max2)
			{
				GoodsData gd2 = BagDataList[subIndex2];
				if (gd2 != null)
				{
					if (gd2.Using != 1)
					{
						if (Global.GetCategoriyByGoodsID(gd2.GoodsID) == 980 && gd2.GCount > 0)
						{
							if (this._CheckBoxs[0].isChecked)
							{
								Dictionary<int, GoodsData> dictionary3 = indexDict;
								int num;
								Index = (num = Index) + 1;
								dictionary3[num] = gd2;
							}
							else if (AllCheckBoxIsFalse)
							{
								Dictionary<int, GoodsData> dictionary4 = indexDict;
								int num;
								Index = (num = Index) + 1;
								dictionary4[num] = gd2;
							}
							else
							{
								GoodVO goodVO = ConfigGoods.GetGoodsXmlNodeByID(gd2.GoodsID);
								if (goodVO != null)
								{
									int suit = goodVO.SuitID;
									if (suit <= 3)
									{
										if (!this._CheckBoxs[3].isChecked)
										{
											goto IL_3FE;
										}
									}
									else if (suit == 4)
									{
										if (!this._CheckBoxs[2].isChecked)
										{
											goto IL_3FE;
										}
									}
									else if (suit > 4 && suit <= 6 && !this._CheckBoxs[1].isChecked)
									{
										goto IL_3FE;
									}
									indexDict[Index] = gd2;
									Index++;
								}
							}
						}
					}
				}
				IL_3FE:
				subIndex2++;
			}
		}
		byte refreshIconCount = 0;
		for (int i = 0; i < 200; i++)
		{
			GoodsData gd3 = null;
			if (indexDict.TryGetValue(i, ref gd3))
			{
				GGoodIcon icon = this.AddIcon(gd3);
				this._GGoodsBox.SetGoodsIcon(this.Getindex(i), icon);
				icon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
				bool canUseGoods = Global.CanUseGoods(gd3.GoodsID, false, true);
				Super.InitGoodsGIcon(icon, gd3, canUseGoods, IconTextTypes.Qianghua);
				if (selectID == gd3.Id)
				{
					icon.GoodImg.ToGrayBitmap = true;
				}
				if ((refreshIconCount += 1) > 25)
				{
					refreshIconCount = 0;
					yield return null;
				}
			}
			else
			{
				this._GGoodsBox.RemoveGoodsIcon(this.Getindex(i));
			}
		}
		if (this.mSelectGoodsData != null)
		{
			this.RefreshGoods(this.mSelectGoodsData);
		}
		if (AllNetSelet != 0)
		{
			this.SetAllNotSelect();
		}
		yield break;
	}

	public void NoticeUpLevelCallBack(GoodsData data)
	{
		int oldLevel = this.mSelectGoodsData.ElementhrtsProps[0];
		int oldExp = this.mSelectGoodsData.ElementhrtsProps[1];
		if (this.mSelectGoodsData != null)
		{
			this.mSelectGoodsData = data;
		}
		this.mSelectGoodsID.Clear();
		this.RefreshEXPAndLevel(oldLevel, oldExp);
		this.RefreshGoods(data);
		this.CheckRoleSleletGoods();
	}

	private const float TestStarStartY = 48f;

	private const float TestStarY = -26f;

	private const int PageConut = 10;

	private const int LineConnt = 5;

	private const int PageLine = 4;

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UISprite[] _Pages;

	[SerializeField]
	private GGoodsBox _GGoodsBox;

	[SerializeField]
	private ListBox _GoodsListBox;

	[SerializeField]
	private SpringPanel UIDragPl;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GCheckBox[] _CheckBoxs;

	[SerializeField]
	private GButton _TrimBtn;

	[SerializeField]
	private GButton _AllSelectBtn;

	[SerializeField]
	private GButton _UpLevelBtn;

	[SerializeField]
	private UICollider _PropColliderControl;

	[SerializeField]
	private UILabel _PropNameLabel;

	[SerializeField]
	private UILabel _PropValueLabel;

	[SerializeField]
	private UILabel _PropNextValueLabel;

	[SerializeField]
	private UISprite _PropStarSp;

	[SerializeField]
	private UILabel _LevelEXPLabel;

	[SerializeField]
	private UILabel _LevelLabel1;

	[SerializeField]
	private UILabel _LevelLabel2;

	[SerializeField]
	private UILabel _LevelLabel;

	[SerializeField]
	private GameObject _LevelJianTouObj;

	[SerializeField]
	private GameObject _NeedUpLevelGoodsRoot;

	[SerializeField]
	private GImgProgressBar _EXPProgressBar;

	[SerializeField]
	private GameObject _ShengJiTeXiao;

	[SerializeField]
	private GButton _PutOffSelectGoodsBtn;

	private UISprite mTempPaneStat;

	private int CurrentSelectedPage;

	private GoodsData mSelectGoodsData;

	private GGoodIcon mNeedUpLevelGoodsIocn;

	private List<GameObject> mStarlist = new List<GameObject>();

	private Dictionary<int, GoodsData> mSelectGoodsID = new Dictionary<int, GoodsData>();

	private byte mIsAllSelect;
}
