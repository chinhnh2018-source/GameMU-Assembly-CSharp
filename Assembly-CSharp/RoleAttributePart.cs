using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RoleAttributePart : UserControl
{
	private bool IsCanChangeRole
	{
		get
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode.Transfer != -1;
		}
	}

	private void InitTextInPrefabs()
	{
		this.AttrTab.Text = Global.GetLang("属性");
		this.JiDianTab.Text = Global.GetLang("加点");
		this.GuoShiTab.Text = Global.GetLang("果实");
		this._Submit_TuiJian.Text = Global.GetLang("确定加点");
		this._Close_TuiJian.Text = Global.GetLang("取消");
		this._Submit_XiDian.Text = Global.GetLang("确定洗点");
		this._Close_XiDian.Text = Global.GetLang("取消");
		this._Avalid_XiDian.Text = Global.GetLang("可洗点数");
		this._Fee_XiJian.Text = Global.GetLang("消耗钻石");
		this.xindianBtn.Text = Global.GetLang("洗点");
		this.tuijianBtn.Text = Global.GetLang("推荐加点");
		this.JiaDianBtn.Text = Global.GetLang("确定");
		this.QuXiaoJiaDianBtn.Text = Global.GetLang("取消");
		this.gsBtn.Text = Global.GetLang("获取果实");
		this.GuoshiInfoText.Text = Global.GetLang("果实说明：\n转生等级越高，可提升属性\n上限越高");
		this.ZhanLi.Text = "0";
		this.PK.X = -120.0;
		this._Avalid_XiDian.X = -160.0;
		this.Level.X = -337.0;
		this.FamilyName.X = -150.0;
		this.jiaLiliangBtn.gameObject.SetActive(false);
		this.GuoshiInfoText.transform.localScale = new Vector3(18f, 18f, 1f);
		this._Fee_XiJian.Pivot = 3;
		this._Fee_XiJian.X = -185.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitFuZhuYeBtn();
		this.jdItemCollection = this.jdListBox.ItemsSource;
		this.gsItemCollection = this.gsListBox.ItemsSource;
		this._Submit_TuiJian.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit_TuiJian);
		this._Submit_XiDian.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit_XiDian);
		this.JiaDianBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit_JiaDian);
		this._Close_JiaDian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_JiaDian.SetActive(false);
		};
		this._Close_TuiJian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_TuiJian.SetActive(false);
		};
		this._Strength_CheckButton.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._Strength_Check.SetActive(true);
			this._Intelligence_Check.SetActive(false);
			RoleAttributePart.mStrengthActive = true;
			string text;
			string text2;
			string text3;
			string text4;
			RoleAttributePart.EncodingRecommendPointString(0, out text, out text2, out text3, out text4, false);
			this._liliangText.Text = text;
			this._zhiliText.Text = text2;
			this._minjieText.Text = text3;
			this._tiliText.Text = text4;
		};
		this._Intelligence_CheckButton.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._Strength_Check.SetActive(false);
			this._Intelligence_Check.SetActive(true);
			RoleAttributePart.mStrengthActive = false;
			string text;
			string text2;
			string text3;
			string text4;
			RoleAttributePart.EncodingRecommendPointString(0, out text, out text2, out text3, out text4, false);
			this._liliangText.Text = text;
			this._zhiliText.Text = text2;
			this._minjieText.Text = text3;
			this._tiliText.Text = text4;
		};
		this._Close_XiDian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_XiDian.SetActive(false);
		};
		this._Close_JiaDian2.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_JiaDian.SetActive(false);
		};
		this._Close_TuiJian2.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_TuiJian.SetActive(false);
		};
		this._Close_XiDian2.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_XiDian.SetActive(false);
		};
		this.jiaLiliangBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowAddPointPart(0);
		};
		this.jiaTiliBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowAddPointPart(3);
		};
		this.jiaZhiLiBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowAddPointPart(1);
		};
		this.jiaMinjieBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowAddPointPart(2);
		};
		this.xindianBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowClearPointPart();
		};
		this.tuijianBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowRecommendPointPart();
		};
		this._Max_JiaDian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._Input_JiaDian.Text = RoleAttributePart.nRemainPoint.ToString();
			this.RefreshJianDianDescs();
		};
		this.Checkbox.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			bool flag = Global.Data.roleData.AutoAssignPropertyPoint != 0;
			if (flag != this.Checkbox.isChecked)
			{
				int nFlag = (!this.Checkbox.isChecked) ? 0 : 1;
				GameInstance.Game.SpriteSetAutoAssignPropertyPointCmd(nFlag);
			}
		};
		this._Input_JiaDian.TextChanged = delegate(object s, EventArgs e)
		{
			this.RefreshJianDianDescs();
		};
		this.AttrTab.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.JiDianTab.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
		};
		this.GuoShiTab.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(3);
		};
		this.QuXiaoJiaDianBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetJiDianPanl();
			this.xidianPanl.gameObject.SetActive(true);
			this.jdPanl.gameObject.SetActive(false);
		};
		this.gsBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.m_renXingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenShenQiPropertyPart(ShenQiPropertyType.RenXing, Global.GetLang("韧性属性总览"), (int)Global.GetCurrentRoleProp(2, 101), null);
		};
		this.SetPart(1);
		this.m_PropretyBtn.MouseLeftButtonUp = delegate(object w, MouseEvent s)
		{
			if (0f < Global.GetBtnCD(this.m_PropretyBtn.GetInstanceID()))
			{
				return;
			}
			Global.AddBtnCD(this.m_PropretyBtn.GetInstanceID(), 1f);
			this.IsRebirthPropBtnClick = false;
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetRoleAllRib();
		};
		if (0 < Global.Data.roleData.RebornLevel)
		{
			this.m_PropretyRebirthBtn.gameObject.SetActive(true);
		}
		else
		{
			this.m_PropretyRebirthBtn.gameObject.SetActive(false);
		}
		this.m_PropretyRebirthBtn.MouseLeftButtonUp = delegate(object w, MouseEvent s)
		{
			if (0f < Global.GetBtnCD(this.m_PropretyRebirthBtn.GetInstanceID()))
			{
				return;
			}
			Global.AddBtnCD(this.m_PropretyRebirthBtn.GetInstanceID(), 1f);
			this.IsRebirthPropBtnClick = true;
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetRoleAllRib();
		};
		this.FuZhiYeBtn.MouseLeftButtonUp = delegate(object w, MouseEvent s)
		{
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuanZhi, ref trigger, ref param, ref param2))
			{
				UIHelper.HintGongNengOpenCondition(GongNengIDs.ZhuanZhi, trigger, param, param2, true);
				return;
			}
			if (!this.IsCanChangeRole)
			{
				Super.HintMainText(Global.GetLang("当前地图不可进行转职"), 10, 3);
				return;
			}
			if (!Global.IsInSafeRegion)
			{
				Super.HintMainText(Global.GetLang("非安全区不能操作"), 10, 3);
				return;
			}
			Global.Data.GameScene.CancelAutoFindRoad(true);
			PlayZone.GlobalPlayZone.OpenChangeOccupationWindow();
		};
		if (this.ZhuanZhiBtns != null && this.ZhuanZhiBtns[0].gameObject != null)
		{
			UIEventListener.Get(this.ZhuanZhiBtns[0].gameObject).onClick = delegate(GameObject s)
			{
				if (this.ZhuanZhiBtns[0].occupation == -1)
				{
					return;
				}
				if (!this.IsCanChangeRole)
				{
					Super.HintMainText(Global.GetLang("当前地图不可进行转职"), 10, 3);
					return;
				}
				if (!Global.IsInSafeRegion)
				{
					Super.HintMainText(Global.GetLang("非安全区不能操作"), 10, 3);
					return;
				}
				GameInstance.Game.SendChangeData(this.ZhuanZhiBtns[0].occupation);
			};
		}
		if (this.ZhuanZhiBtns != null && this.ZhuanZhiBtns[1].gameObject != null)
		{
			UIEventListener.Get(this.ZhuanZhiBtns[1].gameObject).onClick = delegate(GameObject s)
			{
				if (this.ZhuanZhiBtns[1].occupation == -1)
				{
					return;
				}
				if (!this.IsCanChangeRole)
				{
					Super.HintMainText(Global.GetLang("当前地图不可进行转职"), 10, 3);
					return;
				}
				if (!Global.IsInSafeRegion)
				{
					Super.HintMainText(Global.GetLang("非安全区不能操作"), 10, 3);
					return;
				}
				GameInstance.Game.SendChangeData(this.ZhuanZhiBtns[1].occupation);
			};
		}
		if (this.ZhuanZhiBtns != null && this.ZhuanZhiBtns[2].gameObject != null)
		{
			UIEventListener.Get(this.ZhuanZhiBtns[2].gameObject).onClick = delegate(GameObject s)
			{
				if (this.ZhuanZhiBtns[2].occupation == -1)
				{
					return;
				}
				if (!this.IsCanChangeRole)
				{
					Super.HintMainText(Global.GetLang("当前地图不可进行转职"), 10, 3);
					return;
				}
				if (!Global.IsInSafeRegion)
				{
					Super.HintMainText(Global.GetLang("非安全区不能操作"), 10, 3);
					return;
				}
				GameInstance.Game.SendChangeData(this.ZhuanZhiBtns[2].occupation);
			};
		}
		if (this.ZhuanZhiBtns != null && this.ZhuanZhiBtns[3].gameObject != null)
		{
			UIEventListener.Get(this.ZhuanZhiBtns[3].gameObject).onClick = delegate(GameObject s)
			{
				if (this.ZhuanZhiBtns[3].occupation == -1)
				{
					return;
				}
				if (!this.IsCanChangeRole)
				{
					Super.HintMainText(Global.GetLang("当前地图不可进行转职"), 10, 3);
					return;
				}
				if (!Global.IsInSafeRegion)
				{
					Super.HintMainText(Global.GetLang("非安全区不能操作"), 10, 3);
					return;
				}
				GameInstance.Game.SendChangeData(this.ZhuanZhiBtns[3].occupation);
			};
		}
	}

	public void ShowRoleProPerty(double[] attArray)
	{
		if (ConfigExtPropIndexes.GetExtPropIndexesVOByWord("MAX").ID == attArray.Length)
		{
			Global.ShowProPerty(1, this.GetAllPropertyStr(attArray, this.IsRebirthPropBtnClick), null);
		}
	}

	private string[] GetAllPropertyStr(double[] attArray, bool Rebirth = false)
	{
		string[] array = new string[]
		{
			(!Rebirth) ? Global.GetLang("总属性预览") : Global.GetLang("重生总属性预览"),
			string.Empty
		};
		List<RoleAttributePart.PropertyStr> list = new List<RoleAttributePart.PropertyStr>();
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		for (int i = 1; i < ConfigExtPropIndexes.GetExtPropIndexesVOByWord("MAX").ID; i++)
		{
			if (i < attArray.Length)
			{
				double num = attArray[i];
				if (0.0 < num)
				{
					if (!Rebirth)
					{
						if (!ConfigExtPropIndexes.GetExtPropIndexesIsRebirthPropByID(i))
						{
							if (dictionary.ContainsKey(i))
							{
								dictionary[i] = num;
							}
							else
							{
								dictionary.Add(i, num);
							}
						}
					}
					else if (ConfigExtPropIndexes.GetExtPropIndexesIsRebirthPropByID(i))
					{
						if (dictionary.ContainsKey(i))
						{
							dictionary[i] = num;
						}
						else
						{
							dictionary.Add(i, num);
						}
					}
				}
			}
		}
		foreach (KeyValuePair<int, double> keyValuePair in dictionary)
		{
			int key = keyValuePair.Key;
			int num2 = (!Rebirth) ? ConfigExtPropIndexes.GetExtPropIndexesShowListByID(key) : ConfigExtPropIndexes.GetExtPropIndexesShowList2ByID(key);
			if (0 < num2)
			{
				RoleAttributePart.PropertyStr propertyStr = default(RoleAttributePart.PropertyStr);
				propertyStr.ShowList = num2;
				propertyStr.Str = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, false);
				propertyStr.Percent = ConfigExtPropIndexes.GetPercentByID(key);
				Dictionary<int, double>.Enumerator enumerator;
				KeyValuePair<int, double> keyValuePair2 = enumerator.Current;
				propertyStr.Att = keyValuePair2.Value;
				list.Add(propertyStr);
			}
		}
		if (Rebirth)
		{
			string[] array2 = array;
			int num3 = 1;
			string text = array2[num3];
			array2[num3] = string.Concat(new object[]
			{
				text,
				Global.GetLang("重生等级："),
				Global.Data.roleData.RebornLevel,
				Environment.NewLine
			});
			string[] array3 = array;
			int num4 = 1;
			array3[num4] = array3[num4] + Global.GetLang("每日重生经验上限") + Environment.NewLine;
			string[] array4 = array;
			int num5 = 1;
			text = array4[num5];
			array4[num5] = string.Concat(new string[]
			{
				text,
				Global.GetLang("怪物产出： "),
				(Global.Data.roleData.MoneyData[158] - Global.Data.roleData.MoneyData[148]).ToString(),
				"/",
				Global.Data.roleData.MoneyData[158].ToString(),
				Environment.NewLine
			});
			string[] array5 = array;
			int num6 = 1;
			text = array5[num6];
			array5[num6] = string.Concat(new string[]
			{
				text,
				Global.GetLang("装备回收："),
				(Global.Data.roleData.MoneyData[159] - Global.Data.roleData.MoneyData[149]).ToString(),
				"/",
				Global.Data.roleData.MoneyData[159].ToString(),
				Environment.NewLine,
				Environment.NewLine
			});
		}
		list.Sort((RoleAttributePart.PropertyStr a, RoleAttributePart.PropertyStr b) => a.ShowList - b.ShowList);
		for (int j = 0; j < list.Count; j++)
		{
			RoleAttributePart.PropertyStr propertyStr2 = list[j];
			if (propertyStr2.Percent)
			{
				string[] array6 = array;
				int num7 = 1;
				array6[num7] = array6[num7] + string.Format(Global.GetLang("{0}：{1}%"), propertyStr2.Str, this.CutDoubleValue2(propertyStr2.Att * 100.0)) + Environment.NewLine;
			}
			else if (propertyStr2.Att > 1.0)
			{
				int num8 = this.MyDoubleToInt(propertyStr2.Att);
				string[] array7 = array;
				int num9 = 1;
				array7[num9] = array7[num9] + string.Format(Global.GetLang("{0}：{1}"), propertyStr2.Str, num8) + Environment.NewLine;
			}
		}
		return array;
	}

	private int MyDoubleToInt(double value)
	{
		string text = value.ToString();
		string[] array = text.Split(new char[]
		{
			'.'
		});
		if (array.Length < 1)
		{
			return Mathf.FloorToInt((float)value);
		}
		return int.Parse(array[0]);
	}

	private string CutDoubleValue2(double value)
	{
		string text = string.Empty;
		string text2 = value.ToString();
		string[] array = text2.Split(new char[]
		{
			'.'
		});
		if (array.Length == 2)
		{
			text = text + array[0] + ".";
			if (2 >= array[1].Length)
			{
				text += array[1];
			}
			else
			{
				text += array[1].get_Chars(0);
				text += array[1].get_Chars(1);
			}
			return text;
		}
		return value.ToString();
	}

	public Brush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
	}

	public void PauseAllEffect(bool pause)
	{
	}

	public void SetBonusIcon()
	{
	}

	public void RefreshFrontShow()
	{
		this.RoleModal.Clear();
		int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.FashionWingsID);
		List<GoodsData> list = new List<GoodsData>();
		Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
		if (usingGoodsDataList != null)
		{
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				GoodsData goodsData = Global.CloneGoodsData(keyValuePair.Value, true);
				list.Add(goodsData);
			}
		}
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
		if (roleCommonUseParamsValue > 0)
		{
			int num = 26;
			if (num < Global.Data.roleData.RoleCommonUseIntPamams.Count)
			{
				int fashionID = Global.Data.roleData.RoleCommonUseIntPamams[num];
				int fashionGoodsID = Global.GetFashionGoodsID(fashionID);
				this.roleResLoader = UIHelper.LoadRoleRes(this.RoleModal, Global.Data.roleData.SettingBitFlags, Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation, Global.Data.RoleName, list, Global.Data.equipPet, Global.Data.roleData.MyWingData, 1.5f, fashionGoodsID, null, false);
			}
		}
		else
		{
			this.roleResLoader = UIHelper.LoadRoleRes(this.RoleModal, Global.Data.roleData.SettingBitFlags, Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation, Global.Data.RoleName, list, Global.Data.equipPet, Global.Data.roleData.MyWingData, 1.5f, 0, null, false);
		}
	}

	public void SetJingMaiProgress()
	{
	}

	public void SetLearnedSkillProgress()
	{
	}

	public void SetLifeMagicValues()
	{
	}

	public void SetInterPowerValue()
	{
	}

	public void SetPKValues()
	{
		this.PK.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.PKPoint
		});
	}

	public void SetExpProgress()
	{
		if (Global.Data.roleData.Level < Global.Data.LevelUpExperienceList.Length - 1)
		{
			double num = (double)Global.Data.LevelUpExperienceList[Global.Data.roleData.Level + 1];
		}
	}

	public void SetRoleLevel()
	{
		this.Level.Text = StringUtil.substitute(Global.GetLang("等级:{0} 【{1}转】"), new object[]
		{
			Global.Data.roleData.Level,
			Global.Data.roleData.ChangeLifeCount
		});
	}

	public void InitPartData(bool refreshRole = true)
	{
		RoleAttributePart.Occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		if (refreshRole)
		{
			this.RefreshFrontShow();
		}
		RoleAttributePart.CalcPoint();
		if (HintQueueIcon.HintAddPoint && RoleAttributePart.nRemainPoint >= 500)
		{
			this.tuijianBtn.SetSpriteAnimationVisible(true);
		}
		else
		{
			this.tuijianBtn.SetSpriteAnimationVisible(false);
			HintQueueIcon.HintAddPoint = false;
		}
		this.Checkbox.Check = (Global.Data.roleData.AutoAssignPropertyPoint != 0);
	}

	public void GetNewData()
	{
		this.UnFreezeAllHint();
		if (!this.FirstGetNewData)
		{
			return;
		}
		this.FirstGetNewData = false;
		GameInstance.Game.SpriteGetAttrib2();
	}

	public void SetPart(int type)
	{
		switch (type)
		{
		case 1:
			this.SetAttrPanl();
			this.SetBtnStat(this.AttrTab);
			break;
		case 2:
			this.SetJiDianPanl();
			this.SetBtnStat(this.JiDianTab);
			break;
		case 3:
			this.SetGuoShiPanl();
			this.SetBtnStat(this.GuoShiTab);
			break;
		}
	}

	private void SetAttrPanl()
	{
		this.AttrPanl.gameObject.SetActive(true);
		this.JiDianPanl.gameObject.SetActive(false);
		this.GuoShiPanl.gameObject.SetActive(false);
	}

	private void SetJiDianPanl()
	{
		this.JiDianPanl.gameObject.SetActive(true);
		this.AttrPanl.gameObject.SetActive(false);
		this.GuoShiPanl.gameObject.SetActive(false);
		this.jdItemCollection.Clear();
		this.readyJiadianArr = new int[4];
		this.SetJiadianCount();
		int num = RoleAttributePart.CalcPoint();
		for (int i = 0; i < 4; i++)
		{
			RoleAttrItem roleAttrItem = U3DUtils.NEW<RoleAttrItem>();
			roleAttrItem.ImgVisible = false;
			if (i == 0)
			{
				roleAttrItem.Title.Text = Global.GetLang("力量");
				roleAttrItem.icon.spriteName = "li";
				roleAttrItem.PropIndexes = 0;
				roleAttrItem.AttrV.Text = StringUtil.substitute("{0}", new object[]
				{
					(int)Global.GetCurrentRoleProp(1, 0)
				});
			}
			else if (i == 1)
			{
				roleAttrItem.Title.Text = Global.GetLang("智力");
				roleAttrItem.icon.spriteName = "zhi";
				roleAttrItem.PropIndexes = 1;
				roleAttrItem.AttrV.Text = StringUtil.substitute("{0}", new object[]
				{
					(int)Global.GetCurrentRoleProp(1, 1)
				});
			}
			else if (i == 2)
			{
				roleAttrItem.Title.Text = Global.GetLang("敏捷");
				roleAttrItem.icon.spriteName = "min";
				roleAttrItem.PropIndexes = 2;
				roleAttrItem.AttrV.Text = StringUtil.substitute("{0}", new object[]
				{
					(int)Global.GetCurrentRoleProp(1, 2)
				});
			}
			else if (i == 3)
			{
				roleAttrItem.Title.Text = Global.GetLang("体力");
				roleAttrItem.icon.spriteName = "ti";
				roleAttrItem.PropIndexes = 3;
				roleAttrItem.AttrV.Text = StringUtil.substitute("{0}", new object[]
				{
					(int)Global.GetCurrentRoleProp(1, 3)
				});
			}
			if (num > 0)
			{
				NGUITools.SetActive(roleAttrItem.AddBtn.gameObject, true);
				roleAttrItem.Title.gameObject.SetActive(true);
			}
			else
			{
				roleAttrItem.AddBtn.gameObject.SetActive(false);
			}
			this.jdItemCollection.AddNoUpdate(roleAttrItem);
			roleAttrItem.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.ID == -1)
				{
					this.SubAttr(roleAttrItem);
				}
				else if (e.ID == 0)
				{
					this.AddAttr(roleAttrItem);
				}
				else if (e.ID == 1)
				{
					if (this.readyJiadianCount + e.IDType - this.readyJiadianArr[roleAttrItem.PropIndexes] > RoleAttributePart.CalcPoint())
					{
						e.IDType = RoleAttributePart.CalcPoint() - this.readyJiadianCount + this.readyJiadianArr[roleAttrItem.PropIndexes];
					}
					this.readyJiadianArr[roleAttrItem.PropIndexes] = e.IDType;
					roleAttrItem.addValue = e.IDType;
					this.SetJiadianCount();
				}
				return true;
			};
			roleAttrItem.DPSelectedItemTips = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.ShowTips(e.ID);
				return true;
			};
		}
	}

	private void SetGuoShiPanl()
	{
		this.GuoShiPanl.gameObject.SetActive(true);
		this.AttrPanl.gameObject.SetActive(false);
		this.JiDianPanl.gameObject.SetActive(false);
		this.gsItemCollection.Clear();
		XElement xelement = Global.GetXElement(Global.GetGameResXml("Config/Roles/ZhuanShengAddPoint.xml"), "ShuXing", "ZhuanShengLevel", Convert.ToString(Global.Data.roleData.ChangeLifeCount));
		if (xelement == null)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			RoleAttrItem roleAttrItem = U3DUtils.NEW<RoleAttrItem>();
			roleAttrItem.ImgVisible = true;
			roleAttrItem.ProgressBar.gameObject.SetActive(true);
			if (i == 0)
			{
				roleAttrItem.Img.URL = Global.GetGoodsIconString(5101);
				roleAttrItem.Title.Text = Global.GetLang("力量属性上限：");
				roleAttrItem.icon.spriteName = "li";
				roleAttrItem.PropIndexes = 0;
				double currentRoleProp = Global.GetCurrentRoleProp(3, 23);
				roleAttrItem.ProgressText.Text = string.Format("{0}/{1}", currentRoleProp, Global.GetXElementAttributeStr(xelement, "Strength"));
				roleAttrItem.ProgressBar.Percent = currentRoleProp / Global.GetXElementAttributeDouble(xelement, "Strength");
			}
			else if (i == 1)
			{
				roleAttrItem.Img.URL = Global.GetGoodsIconString(5103);
				roleAttrItem.Title.Text = Global.GetLang("智力属性上限：");
				roleAttrItem.icon.spriteName = "zhi";
				roleAttrItem.PropIndexes = 1;
				double currentRoleProp2 = Global.GetCurrentRoleProp(3, 24);
				roleAttrItem.ProgressText.Text = string.Format("{0}/{1}", currentRoleProp2, Global.GetXElementAttributeStr(xelement, "Intelligence"));
				roleAttrItem.ProgressBar.Percent = currentRoleProp2 / Global.GetXElementAttributeDouble(xelement, "Intelligence");
			}
			else if (i == 2)
			{
				roleAttrItem.Img.URL = Global.GetGoodsIconString(5102);
				roleAttrItem.Title.Text = Global.GetLang("敏捷属性上限：");
				roleAttrItem.icon.spriteName = "min";
				roleAttrItem.PropIndexes = 2;
				double currentRoleProp3 = Global.GetCurrentRoleProp(3, 25);
				roleAttrItem.ProgressText.Text = string.Format("{0}/{1}", currentRoleProp3, Global.GetXElementAttributeStr(xelement, "Dexterity"));
				roleAttrItem.ProgressBar.Percent = currentRoleProp3 / Global.GetXElementAttributeDouble(xelement, "Dexterity");
			}
			else if (i == 3)
			{
				roleAttrItem.Img.URL = Global.GetGoodsIconString(5104);
				roleAttrItem.Title.Text = Global.GetLang("体力属性上限：");
				roleAttrItem.icon.spriteName = "ti";
				roleAttrItem.PropIndexes = 3;
				double currentRoleProp4 = Global.GetCurrentRoleProp(3, 26);
				roleAttrItem.ProgressText.Text = string.Format("{0}/{1}", currentRoleProp4, Global.GetXElementAttributeStr(xelement, "Constitution"));
				roleAttrItem.ProgressBar.Percent = currentRoleProp4 / Global.GetXElementAttributeDouble(xelement, "Constitution");
			}
			this.gsItemCollection.AddNoUpdate(roleAttrItem);
		}
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(16777215U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(16777215U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(14599836U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(16777215U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	private void FreezeAllHint()
	{
	}

	private void UnFreezeAllHint()
	{
	}

	public void CleanUpChildWindows()
	{
		this.FreezeAllHint();
		Super.CleanUpAllChildWindows(this.Container);
	}

	public void ShowPartData(double[] fields2)
	{
		this.FamilyName.Label.color = NGUIMath.HexToColorEx(10079487U);
		if (Global.Data.CurrentRolePropFields == null)
		{
			return;
		}
		this.SName.Text = Global.FormatRoleName(Global.Data.roleData);
		string bhname = Global.Data.roleData.BHName;
		this.FamilyName.Text = ((!string.IsNullOrEmpty(bhname)) ? bhname : Global.GetLang("无"));
		this.Level.Text = StringUtil.substitute(Global.GetLang("{0}转{1}级"), new object[]
		{
			Global.Data.roleData.ChangeLifeCount,
			Global.Data.roleData.Level
		});
		this.Work.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.GetOccupationStr(Global.Data.roleData.Occupation)
		});
		this.ZhanLi.Text = StringUtil.substitute(Global.GetLang("战力{0}"), new object[]
		{
			Global.Data.roleData.CombatForce
		});
		this.RoleIDText.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			StringUtil.substitute(Global.GetLang("ID:{0}"), new object[]
			{
				(long)Global.Data.roleData.RoleID ^ (long)((ulong)-936551821)
			})
		});
		this.SetPKValues();
		this.SetInterPowerValue();
		this.AttObj.SetActive(false);
		this.RefreshAtt(13);
		this.RefreshAtt(119);
		this.RefreshAtt(101);
		this.RefreshAtt(7);
		this.RefreshAtt(9);
		this.RefreshAtt(3);
		this.RefreshAtt(5);
		this.RefreshAtt(18);
		this.RefreshAtt(19);
		this.RefreshAtt(31);
		this.RefreshAtt(33);
	}

	private void RefreshAtt(int key)
	{
		RoleAttributePart.AttItem attItem;
		if (this.AttItemDic.ContainsKey(key))
		{
			attItem = this.AttItemDic[key];
		}
		else
		{
			attItem = new RoleAttributePart.AttItem(this.AttObj, this._AttRoot);
			this.AttItemDic[key] = attItem;
		}
		if (key == 31)
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				Global.GetLang("物理增幅") + Global.GetLang("：")
			});
		}
		else if (key == 33)
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				Global.GetLang("魔法增幅") + Global.GetLang("：")
			});
		}
		else if (key == 7)
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(11, true) + Global.GetLang("：")
			});
		}
		else if (key == 9)
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(12, true) + Global.GetLang("：")
			});
		}
		else if (key == 3)
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(42, true) + Global.GetLang("：")
			});
		}
		else if (key == 5)
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(43, true) + Global.GetLang("：")
			});
		}
		else
		{
			attItem.NameStr = Global.GetColorStringForNGUIText(new object[]
			{
				"e4ba6e",
				ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(key, true) + Global.GetLang("：")
			});
		}
		switch (key)
		{
		case 3:
			attItem.ValueStr = (int)Global.GetCurrentRoleProp(2, 3) + "-" + (int)Global.GetCurrentRoleProp(2, 4);
			attItem.Pos = new Vector3(-20f, 0f, 0f);
			break;
		default:
			switch (key)
			{
			case 31:
				attItem.ValueStr = (100.0 * Global.GetCurrentRoleProp(2, 31)).ToString() + "%";
				attItem.Pos = new Vector3(-20f, -136f, 0f);
				break;
			default:
				if (key != 18)
				{
					if (key != 19)
					{
						if (key != 13)
						{
							if (key != 101)
							{
								if (key == 119)
								{
									attItem.ValueStr = ((int)Global.GetCurrentRoleProp(2, 119)).ToString();
									attItem.Pos = new Vector3(-20f, 136f, 0f);
								}
							}
							else
							{
								attItem.ValueStr = ((int)Global.GetCurrentRoleProp(2, 101)).ToString();
								attItem.Pos = new Vector3(-20f, 102f, 0f);
							}
						}
						else
						{
							attItem.ValueStr = ((int)Global.GetCurrentRoleProp(2, 13)).ToString();
							attItem.Pos = new Vector3(-20f, 170f, 0f);
						}
					}
					else
					{
						attItem.ValueStr = ((int)Global.GetCurrentRoleProp(2, 19)).ToString();
						attItem.Pos = new Vector3(-20f, -102f, 0f);
					}
				}
				else
				{
					attItem.ValueStr = ((int)Global.GetCurrentRoleProp(2, 18)).ToString();
					attItem.Pos = new Vector3(-20f, -68f, 0f);
				}
				break;
			case 33:
				attItem.ValueStr = (100.0 * Global.GetCurrentRoleProp(2, 33)).ToString() + "%";
				attItem.Pos = new Vector3(-20f, -170f, 0f);
				break;
			}
			break;
		case 5:
			attItem.ValueStr = (int)Global.GetCurrentRoleProp(2, 5) + "-" + (int)Global.GetCurrentRoleProp(2, 6);
			attItem.Pos = new Vector3(-20f, -34f, 0f);
			break;
		case 7:
			attItem.ValueStr = (int)Global.GetCurrentRoleProp(2, 7) + "-" + (int)Global.GetCurrentRoleProp(2, 8);
			attItem.Pos = new Vector3(-20f, 68f, 0f);
			break;
		case 9:
			attItem.ValueStr = (int)Global.GetCurrentRoleProp(2, 9) + "-" + (int)Global.GetCurrentRoleProp(2, 10);
			attItem.Pos = new Vector3(-20f, 34f, 0f);
			break;
		}
	}

	private string EncodingExtPropFrom(int unitPropIndex, int iPoint, out int lines)
	{
		double num = Convert.ToDouble(iPoint);
		string result = string.Empty;
		lines = 1;
		switch (unitPropIndex)
		{
		case 0:
			switch (RoleAttributePart.Occupation)
			{
			case 0:
				result = string.Format("{0}  {1}~{2}\r\n", Global.GetLang("物理攻击:", "666666"), (int)(num * 0.75999999046325684 * 0.60000002384185791), (int)(num * 0.75999999046325684 * 1.0));
				break;
			case 1:
				lines = 0;
				break;
			case 2:
				result = string.Format("{0}  {1}~{2}\r\n", Global.GetLang("物理攻击:", "666666"), (int)(num * 0.800000011920929 * 0.60000002384185791), (int)(num * 0.800000011920929 * 1.0));
				break;
			}
			break;
		case 1:
			switch (RoleAttributePart.Occupation)
			{
			case 0:
			case 2:
				lines = 0;
				break;
			case 1:
				result = string.Format("{0}  {1}~{2}\r\n", Global.GetLang("魔法攻击:", "666666"), (int)(num * 0.87999999523162842 * 0.60000002384185791), (int)(num * 0.87999999523162842 * 1.0));
				break;
			}
			break;
		case 2:
			switch (RoleAttributePart.Occupation)
			{
			case 0:
				result = string.Format("{0}  {1}~{2}\r\n{3}  {4}~{5}\r\n{6}  {7}\r\n{8}  {9}\r\n", new object[]
				{
					Global.GetLang("物理防御:", "666666"),
					(int)(num * 0.87999999523162842 * 0.60000002384185791),
					(int)(num * 0.87999999523162842 * 1.0),
					Global.GetLang("魔法防御:", "666666"),
					(int)(num * 0.60000002384185791 * 0.60000002384185791),
					(int)(num * 0.60000002384185791 * 1.0),
					Global.GetLang("命        中:", "666666"),
					(int)(num * 0.5),
					Global.GetLang("闪        避:", "666666"),
					(int)(num * 0.5)
				});
				lines = 4;
				break;
			case 1:
				result = string.Format("{0}  {1}~{2}\r\n{3}  {4}~{5}\r\n{6}  {7}\r\n{8}  {9}\r\n", new object[]
				{
					Global.GetLang("物理防御:", "666666"),
					(int)(num * 0.63999998569488525 * 0.60000002384185791),
					(int)(num * 0.63999998569488525 * 1.0),
					Global.GetLang("魔法防御:", "666666"),
					(int)(num * 0.8399999737739563 * 0.60000002384185791),
					(int)(num * 0.8399999737739563 * 1.0),
					Global.GetLang("命        中:", "666666"),
					(int)(num * 0.5),
					Global.GetLang("闪        避:", "666666"),
					(int)(num * 0.5)
				});
				lines = 4;
				break;
			case 2:
				result = string.Format("{0}  {1}~{2}\r\n{3}  {4}~{5}\r\n{6}  {7}\r\n{8}  {9}\r\n", new object[]
				{
					Global.GetLang("物理防御:", "666666"),
					(int)(num * 0.75999999046325684 * 0.60000002384185791),
					(int)(num * 0.75999999046325684 * 1.0),
					Global.GetLang("魔法防御:", "666666"),
					(int)(num * 0.72000002861022949 * 0.60000002384185791),
					(int)(num * 0.72000002861022949 * 1.0),
					Global.GetLang("命        中:", "666666"),
					(int)(num * 0.5),
					Global.GetLang("闪        避:", "666666"),
					(int)(num * 0.5)
				});
				lines = 4;
				break;
			}
			break;
		case 3:
			switch (RoleAttributePart.Occupation)
			{
			case 0:
				result = string.Format("{0}  {1}\r\n", Global.GetLang("生命上限:", "666666"), (int)(num * 5.0));
				break;
			case 1:
				result = string.Format("{0}  {1}\r\n", Global.GetLang("生命上限:", "666666"), (int)(num * 3.5999999046325684));
				break;
			case 2:
				result = string.Format("{0}  {1}\r\n", Global.GetLang("生命上限:", "666666"), (int)(num * 4.1999998092651367));
				break;
			}
			break;
		}
		return result;
	}

	private void CloseChildWindow(GChildWindow childWindow)
	{
	}

	public void OnZuanHuangWeekAwardGetCompleted(int result, double totalYuanBao)
	{
	}

	public void SetBuffGicon()
	{
	}

	private GIcon Geticon(string Type, string GoodsCod)
	{
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.TipType = 5;
		gicon.BodyURL = new ImageURL(StringUtil.substitute("Images/RoleAttr/{0}/{1}.png", new object[]
		{
			Type,
			GoodsCod
		}), false, 0);
		return gicon;
	}

	private void ShowAddPointPart(int type)
	{
		RoleAttributePart.CalcPoint();
		this._ModalBak.gameObject.SetActive(true);
		this._SelectedTab = type;
		this._Part_JiaDian.SetActive(true);
		this._Avalid_JiaDian.Text = RoleAttributePart.nRemainPoint.ToString();
		this._Input_JiaDian.text = string.Empty;
		this._Name_JiaDian.text = Global.GetUnitPropIndexeName(type) + ":";
		this.RefreshJianDianDescs();
	}

	private void RefreshJianDianDescs()
	{
		int num = this._Input_JiaDian.Text.SafeToInt32(0);
		if (num > RoleAttributePart.nRemainPoint)
		{
			num = RoleAttributePart.nRemainPoint;
			this._Input_JiaDian.Text = RoleAttributePart.nRemainPoint.ToString();
		}
		int num2 = 0;
		this._info1Text.Text = this.EncodingExtPropFrom(this._SelectedTab, num, out num2);
		for (int i = 0; i < this._JiaDianInfoBaks.Length; i++)
		{
			if (i < num2)
			{
				this._JiaDianInfoBaks[i].gameObject.SetActive(true);
			}
			else
			{
				this._JiaDianInfoBaks[i].gameObject.SetActive(false);
			}
		}
	}

	private void ShowRecommendPointPart()
	{
		RoleAttributePart.CalcPoint();
		this._ModalBak.gameObject.SetActive(true);
		this._Part_TuiJian.SetActive(true);
		RoleAttributePart.Occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		switch (RoleAttributePart.Occupation)
		{
		case 3:
			this._strengthText.Text = Global.GetLang("力魔剑士");
			this._intelligenceText.Text = Global.GetLang("智魔剑士");
			if (RoleAttributePart.nStrengthPoint >= RoleAttributePart.nIntelligencePoint)
			{
				this._Strength_Check.SetActive(true);
				this._Intelligence_Check.SetActive(false);
				RoleAttributePart.mStrengthActive = true;
			}
			else if (RoleAttributePart.nStrengthPoint < RoleAttributePart.nIntelligencePoint)
			{
				this._Strength_Check.SetActive(false);
				this._Intelligence_Check.SetActive(true);
				RoleAttributePart.mStrengthActive = false;
			}
			this._MJS_Select.SetActive(RoleAttributePart.nStrengthPoint == RoleAttributePart.nIntelligencePoint);
			break;
		}
		Vector3 localPosition = this._Submit_TuiJian.gameObject.transform.localPosition;
		Vector3 localPosition2 = this._Close_TuiJian.gameObject.transform.localPosition;
		if (!this._MJS_Select.active)
		{
			localPosition.y = -150f;
			localPosition2.y = -150f;
		}
		else
		{
			localPosition.y = -180f;
			localPosition2.y = -180f;
		}
		this._Submit_TuiJian.gameObject.transform.localPosition = localPosition;
		this._Close_TuiJian.gameObject.transform.localPosition = localPosition2;
		string text;
		string text2;
		string text3;
		string text4;
		RoleAttributePart.EncodingRecommendPointString(0, out text, out text2, out text3, out text4, false);
		this._liliangText.Text = text;
		this._zhiliText.Text = text2;
		this._minjieText.Text = text3;
		this._tiliText.Text = text4;
	}

	private void ShowClearPointPart()
	{
		GameInstance.Game.SpriteQueryCleanPropAddPointInfoCmd();
	}

	public void OnCanClearPointResult(int nValue)
	{
		double num = (double)RoleAttributePart.nUsedPoint - (Global.GetCurrentRoleProp(3, 23) + Global.GetCurrentRoleProp(3, 24) + Global.GetCurrentRoleProp(3, 25) + Global.GetCurrentRoleProp(3, 26) + (double)this.GetTalentCount() + (double)this.GetShengWuAddCount());
		int num2 = (int)num;
		this.KeXiDianShu = num2;
		this._ModalBak.gameObject.SetActive(true);
		long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("MianFeiXiDian");
		string text = Global.GetLang("钻石");
		if ((long)Global.Data.roleData.ChangeLifeCount > systemParamIntByName)
		{
			this.nFeeXidian = 100;
			text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(text, "ShuXingXiDian", this.nFeeXidian);
			this._Fee_XiJian.text = string.Format(Global.GetLang("{{c39550}}消耗{0}:{{-}}  {1}"), text, this.nFeeXidian);
		}
		else
		{
			this.nFeeXidian = 0;
			text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(text, "ShuXingXiDian", this.nFeeXidian);
			this._Fee_XiJian.text = string.Format(Global.GetLang("{{c39550}}消耗{0}:{{-}}  {1}"), text, this.nFeeXidian);
		}
		if (systemParamIntByName >= 0L)
		{
			this._Fee_Desc.text = string.Format(Global.GetLang("{0}转前免费"), systemParamIntByName);
		}
		else
		{
			this._Fee_Desc.text = null;
		}
		this._Avalid_XiDian.text = string.Format(Global.GetLang("{{c39550}}可洗点数:{{-}} {0}"), num2);
		this._Part_XiDian.SetActive(true);
	}

	public int GetTalentCount()
	{
		double num = 0.0;
		TalentData myTalentData = Global.Data.roleData.MyTalentData;
		if (myTalentData != null && myTalentData.IsOpen)
		{
			foreach (TalentEffectItem talentEffectItem in myTalentData.EffectList)
			{
				if (talentEffectItem.ItemEffectList != null && talentEffectItem.ItemEffectList.Count > 0)
				{
					for (int i = 0; i < talentEffectItem.ItemEffectList.Count; i++)
					{
						TalentEffectInfo talentEffectInfo = talentEffectItem.ItemEffectList[i];
						if (talentEffectInfo.EffectType == 1)
						{
							num += talentEffectInfo.EffectValue;
						}
					}
				}
			}
		}
		return Convert.ToInt32(num);
	}

	public int GetShengWuAddCount()
	{
		XElement gameResXml = Global.GetGameResXml("Config/ShengWu.xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		if (Global.dic_holyItem.Keys.Count <= 0)
		{
			return 0;
		}
		int num = 0;
		sbyte b = 1;
		while ((int)b < 5)
		{
			HolyItemData holyItemData = null;
			if (Global.dic_holyItem.ContainsKey(b))
			{
				Global.dic_holyItem.TryGetValue(b, ref holyItemData);
				if (holyItemData != null)
				{
					Global.m_PartArray = holyItemData.m_PartArray;
				}
			}
			HolyItemPartData value = Global.m_PartArray.GetValue(1);
			int num2 = (int)value.m_sSuit;
			sbyte b2 = 1;
			while ((int)b2 < 6)
			{
				value = Global.m_PartArray.GetValue((sbyte)((int)b2 + 1));
				if ((int)value.m_sSuit < num2)
				{
					num2 = (int)value.m_sSuit;
				}
				b2 += 1;
			}
			int shengwuID = Global.GetShengwuID(num2, (int)b);
			string text = string.Empty;
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (shengwuID == xelement.AttributeInt("ID"))
				{
					text = xelement.AttributeStr("ExtraProperty");
				}
			}
			if (!text.Equals("-1"))
			{
				for (int j = 0; j < text.Split(new char[]
				{
					'|'
				}).Length; j++)
				{
					string text2 = text.Split(new char[]
					{
						'|'
					})[j].Split(new char[]
					{
						','
					})[0];
					string text3 = "Constitution";
					string text4 = "Dexterity";
					string text5 = "Intelligence";
					string text6 = "Strength";
					if (text2.ToUpper().Equals(text3.ToUpper()) || text2.ToUpper().Equals(text4.ToUpper()) || text2.ToUpper().Equals(text5.ToUpper()) || text2.ToUpper().Equals(text6.ToUpper()))
					{
						if (text.Split(new char[]
						{
							'|'
						}).Length == 1)
						{
							num += int.Parse(text.Split(new char[]
							{
								','
							})[1]);
						}
						else
						{
							num += int.Parse(text.Split(new char[]
							{
								'|'
							})[j].Split(new char[]
							{
								','
							})[1]);
						}
					}
				}
			}
			b += 1;
		}
		return num;
	}

	public static int CalcPoint()
	{
		RoleAttributePart.nTotalPoint = (int)Global.GetCurrentRoleProp(0, 0);
		RoleAttributePart.nStrengthPoint = (int)Global.GetCurrentRoleProp(1, 0);
		RoleAttributePart.nIntelligencePoint = (int)Global.GetCurrentRoleProp(1, 1);
		RoleAttributePart.nDexterityPoint = (int)Global.GetCurrentRoleProp(1, 2);
		RoleAttributePart.nConstitutionPoint = (int)Global.GetCurrentRoleProp(1, 3);
		RoleAttributePart.nUsedPoint = RoleAttributePart.nStrengthPoint + RoleAttributePart.nIntelligencePoint + RoleAttributePart.nDexterityPoint + RoleAttributePart.nConstitutionPoint;
		RoleAttributePart.nRemainPoint = RoleAttributePart.nTotalPoint - RoleAttributePart.nUsedPoint;
		return RoleAttributePart.nRemainPoint;
	}

	public void NotifyAttribute2()
	{
		this.InitPartData(false);
	}

	private static void EncodingRecommendPointString(int avalidPoint, out string str1, out string str2, out string str3, out string str4, bool bAutoAddPoint = false)
	{
		RoleAttributePart.addStrengthPoint = 0;
		RoleAttributePart.addIntelligencePoint = 0;
		RoleAttributePart.addDexterityPoint = 0;
		RoleAttributePart.addConstitutionPoint = 0;
		float num = 0f;
		float num2 = 0f;
		float num3 = 2f;
		float num4 = 3f;
		if (avalidPoint > 0)
		{
			RoleAttributePart.nRemainPoint = avalidPoint;
		}
		RoleAttributePart.Occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		switch (RoleAttributePart.Occupation)
		{
		case 0:
			num = 5f;
			break;
		case 1:
			num2 = 5f;
			break;
		case 2:
			num = 5f;
			break;
		case 3:
			if (bAutoAddPoint)
			{
				num2 = 5f;
			}
			else if (RoleAttributePart.mStrengthActive)
			{
				num = 5f;
			}
			else
			{
				num2 = 5f;
			}
			break;
		case 5:
			num2 = 5f;
			break;
		}
		int num5;
		for (;;)
		{
			num5 = RoleAttributePart.nRemainPoint;
			if (num > 0f)
			{
				num5 += RoleAttributePart.nStrengthPoint;
			}
			if (num2 > 0f)
			{
				num5 += RoleAttributePart.nIntelligencePoint;
			}
			if (num3 > 0f)
			{
				num5 += RoleAttributePart.nDexterityPoint;
			}
			if (num4 > 0f)
			{
				num5 += RoleAttributePart.nConstitutionPoint;
			}
			if (num4 > 0f && (float)num5 * num4 / (num4 + num + num2 + num3) <= (float)RoleAttributePart.nConstitutionPoint)
			{
				num4 = 0f;
			}
			else if (num > 0f && (float)num5 * num / (num4 + num + num2 + num3) <= (float)RoleAttributePart.nStrengthPoint)
			{
				num = 0f;
			}
			else if (num2 > 0f && (float)num5 * num2 / (num4 + num + num2 + num3) <= (float)RoleAttributePart.nIntelligencePoint)
			{
				num2 = 0f;
			}
			else
			{
				if (num3 <= 0f || (float)num5 * num3 / (num4 + num + num2 + num3) > (float)RoleAttributePart.nDexterityPoint)
				{
					break;
				}
				num3 = 0f;
			}
		}
		if (num > 0f)
		{
			RoleAttributePart.addStrengthPoint = (int)((float)num5 * num / (num4 + num + num2 + num3) - (float)RoleAttributePart.nStrengthPoint);
		}
		if (num2 > 0f)
		{
			RoleAttributePart.addIntelligencePoint = (int)((float)num5 * num2 / (num4 + num + num2 + num3) - (float)RoleAttributePart.nIntelligencePoint);
		}
		if (num3 > 0f)
		{
			RoleAttributePart.addDexterityPoint = (int)((float)num5 * num3 / (num4 + num + num2 + num3) - (float)RoleAttributePart.nDexterityPoint);
		}
		if (num4 > 0f)
		{
			RoleAttributePart.addConstitutionPoint = (int)((float)num5 * num4 / (num4 + num + num2 + num3) - (float)RoleAttributePart.nConstitutionPoint);
		}
		int num6 = RoleAttributePart.nRemainPoint - (RoleAttributePart.addStrengthPoint + RoleAttributePart.addIntelligencePoint + RoleAttributePart.addDexterityPoint + RoleAttributePart.addConstitutionPoint);
		if (num4 > 0f)
		{
			RoleAttributePart.addConstitutionPoint += num6;
		}
		else if (num > 0f)
		{
			RoleAttributePart.addStrengthPoint += num6;
		}
		else if (num2 > 0f)
		{
			RoleAttributePart.addIntelligencePoint += num6;
		}
		else if (num3 > 0f)
		{
			RoleAttributePart.addDexterityPoint += num6;
		}
		str1 = string.Format("{{c39550}}{0}:{{-}} {{ffffff}}{1}{{-}}{{06ff00}} +{2}{{-}}", Global.GetLang("力量"), RoleAttributePart.nStrengthPoint, RoleAttributePart.addStrengthPoint);
		str2 = string.Format("{{c39550}}{0}:{{-}} {{ffffff}}{1}{{-}}{{06ff00}} +{2}{{-}}", Global.GetLang("智力"), RoleAttributePart.nIntelligencePoint, RoleAttributePart.addIntelligencePoint);
		str3 = string.Format("{{c39550}}{0}:{{-}} {{ffffff}}{1}{{-}}{{06ff00}} +{2}{{-}}", Global.GetLang("敏捷"), RoleAttributePart.nDexterityPoint, RoleAttributePart.addDexterityPoint);
		str4 = string.Format("{{c39550}}{0}:{{-}} {{ffffff}}{1}{{-}}{{06ff00}} +{2}{{-}}", Global.GetLang("体力"), RoleAttributePart.nConstitutionPoint, RoleAttributePart.addConstitutionPoint);
	}

	public void OnSubmit_JiaDian(object sender, MouseEvent e)
	{
		if (this.readyJiadianCount > 0 && this.readyJiadianCount <= RoleAttributePart.nRemainPoint)
		{
			GameInstance.Game.SpriteRecommendPoint(this.readyJiadianArr[0], this.readyJiadianArr[1], this.readyJiadianArr[2], this.readyJiadianArr[3]);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请输入要分配的点数!"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void OnSubmit_XiDian(object sender, MouseEvent e)
	{
		if (this.KeXiDianShu <= 0)
		{
			Super.HintMainText(Global.GetLang("无属性点可洗"), 10, 3);
			return;
		}
		int num = Global.Data.roleData.UserMoney + Global.Data.roleData.Gold;
		if (num >= this.nFeeXidian || IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("ShuXingXiDian", this.nFeeXidian, false))
		{
			GameInstance.Game.SpriteClearPoint();
			this._Close_XiDian.MouseLeftButtonUp(sender, e);
		}
		else
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
	}

	public void OnSubmit_TuiJian(object sender, MouseEvent e)
	{
		if (RoleAttributePart.nRemainPoint > 0)
		{
			GameInstance.Game.SpriteRecommendPoint(RoleAttributePart.addStrengthPoint, RoleAttributePart.addIntelligencePoint, RoleAttributePart.addDexterityPoint, RoleAttributePart.addConstitutionPoint);
		}
		this._Close_TuiJian.MouseLeftButtonUp(sender, e);
	}

	public static void AutoAddPoint(int avalidPoint)
	{
		string text;
		string text2;
		string text3;
		string text4;
		RoleAttributePart.EncodingRecommendPointString(avalidPoint, out text, out text2, out text3, out text4, true);
		GameInstance.Game.SpriteRecommendPoint(RoleAttributePart.addStrengthPoint, RoleAttributePart.addIntelligencePoint, RoleAttributePart.addDexterityPoint, RoleAttributePart.addConstitutionPoint);
	}

	public void NotifyResult(int ret, int type = 0)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (ret == -1)
		{
			Super.HintMainText(Global.GetLang("剩余属性点数不足!"), 10, 3);
		}
		else if (ret == -2)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
		else
		{
			this.SetPart(2);
			this.xidianPanl.gameObject.SetActive(true);
			this.jdPanl.gameObject.SetActive(false);
		}
	}

	public void SubAttr(RoleAttrItem item)
	{
		if (item.addValue > 0)
		{
			item.addValue--;
			this.readyJiadianArr[item.PropIndexes] = item.addValue;
			this.SetJiadianCount();
		}
	}

	public void AddAttr(RoleAttrItem item)
	{
		if (this.readyJiadianCount < RoleAttributePart.CalcPoint())
		{
			item.addValue++;
			this.readyJiadianArr[item.PropIndexes] = item.addValue;
			this.SetJiadianCount();
		}
		else
		{
			Super.HintMainText(Global.GetLang("可分配点数已全部输入！"), 10, 3);
		}
	}

	private void SetJiadianCount()
	{
		this.readyJiadianCount = this.readyJiadianArr[0] + this.readyJiadianArr[1] + this.readyJiadianArr[2] + this.readyJiadianArr[3];
		this.ShengYuDianshuText.Text = StringUtil.substitute(Global.GetLang("剩余点数：{{dec69c}}{0}{{-}}"), new object[]
		{
			RoleAttributePart.CalcPoint() - this.readyJiadianCount
		});
		if (this.readyJiadianCount > 0)
		{
			this.xidianPanl.gameObject.SetActive(false);
			this.jdPanl.gameObject.SetActive(true);
		}
		else
		{
			this.xidianPanl.gameObject.SetActive(true);
			this.jdPanl.gameObject.SetActive(false);
		}
	}

	public void ShowTips(int propsIndex)
	{
		if (propsIndex == -1)
		{
			this._Part_Tip.gameObject.SetActive(false);
		}
		else
		{
			this._Part_Tip.gameObject.SetActive(true);
			Transform transform = this._Part_Tip.transform;
			string text = string.Empty;
			if (propsIndex == 0)
			{
				text += Global.GetLang("力量影响：");
				text += "\n";
				text += Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[7]);
				transform.localPosition = this.Pos(transform.localPosition, 100);
			}
			else if (propsIndex == 1)
			{
				text += Global.GetLang("智力影响：");
				text += "\n";
				text += Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[9]);
				transform.localPosition = this.Pos(transform.localPosition, 27);
			}
			else if (propsIndex == 2)
			{
				text += Global.GetLang("敏捷影响：");
				text += "\n";
				text += string.Format(Global.GetLang("{0}、{1}"), Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[3]), Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[5]));
				text += "\n";
				text += string.Format(Global.GetLang("{0}、{1}"), Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[18]), Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[19]));
				transform.localPosition = this.Pos(transform.localPosition, -45);
			}
			else if (propsIndex == 3)
			{
				text += Global.GetLang("体力影响：");
				text += "\n";
				text += Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[13]);
				transform.localPosition = this.Pos(transform.localPosition, -115);
			}
			this._tipsText.Text = text;
		}
	}

	public Vector3 Pos(Vector3 v, int y)
	{
		v.y = (float)y;
		return v;
	}

	private void InitFuZhuYeBtn()
	{
		if (Global.Data == null || Global.Data.roleData == null || Global.Data.roleData.OccupationList == null || Global.Data.roleData.OccupationList.Count <= 1)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < Global.Data.roleData.OccupationList.Count; i++)
		{
			if (Global.Data.roleData.OccupationList[i] != Global.Data.roleData.Occupation)
			{
				this.ZhuanZhiBtns[num].gameObject.SetActive(true);
				this.ZhuanZhiBtns[num].occupation = Global.Data.roleData.OccupationList[i];
				if (i == 0)
				{
					this.ZhuanZhiBtns[num].zhiye.spriteName = "zhuzhiye";
				}
				else
				{
					this.ZhuanZhiBtns[num].zhiye.spriteName = "fuzhiye";
				}
				float num2 = (float)(-121 + 72 * num);
				this.ZhuanZhiBtns[num].transform.localPosition = new Vector3(-72f, num2, 0f);
				num++;
			}
		}
		float num3 = (float)(-121 + 72 * num);
		this.FuZhiYeBtn.transform.localPosition = new Vector3(-72f, num3, 0f);
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("PurchaseOccupationNum", true);
		int num4 = Global.Data.roleData.OccupationList.Count - 1;
		if (num4 >= int.Parse(systemParamByName))
		{
			this.FuZhiYeBtn.gameObject.SetActive(false);
		}
	}

	public GButton m_PropretyRebirthBtn;

	public GButton m_PropretyBtn;

	public GButton FuZhiYeBtn;

	public ZhuanZhiBtnItem[] ZhuanZhiBtns = new ZhuanZhiBtnItem[4];

	public TextBlock SName;

	public TextBlock Level;

	public TextBlock Work;

	public TextBlock FamilyName;

	public TextBlock PK;

	public TextBlock ZhanLi;

	public TextBlock RoleIDText;

	public GButton AttrTab;

	public GButton JiDianTab;

	public GButton GuoShiTab;

	public GameObject AttrPanl;

	public GameObject JiDianPanl;

	public GameObject GuoShiPanl;

	public Transform _AttRoot;

	public GameObject AttObj;

	public TextBlock LiLiangText;

	public TextBlock MinJieText;

	public TextBlock TiLiText;

	public TextBlock ZhiLiText;

	public TextBlock ShengYuDianshuText;

	public ListBox jdListBox;

	public GameObject xidianPanl;

	public GameObject jdPanl;

	public GButton JiaDianBtn;

	public GButton QuXiaoJiaDianBtn;

	private ObservableCollection jdItemCollection;

	private int readyJiadianCount;

	private int[] readyJiadianArr = new int[4];

	public GButton xindianBtn;

	public GButton tuijianBtn;

	public GButton jiaLiliangBtn;

	public GButton jiaTiliBtn;

	public GButton jiaZhiLiBtn;

	public GButton jiaMinjieBtn;

	public Modal3DShow RoleModal;

	public GCheckBox Checkbox;

	public int _SelectedTab;

	public UISprite _ModalBak;

	public GameObject _Part_JiaDian;

	public ShowNetImage _Bak_JiaDian;

	public GButton _Close_JiaDian;

	public GButton _Close_JiaDian2;

	public GButton _Submit_JIaDian;

	public GButton _Max_JiaDian;

	public TextBlock _Name_JiaDian;

	public TextBlock _Total_JiaDian;

	public TextBlock _Avalid_JiaDian;

	public TextBox _Input_JiaDian;

	public TextBlock _info1Text;

	public TextBlock _info2Text;

	public UISprite[] _JiaDianInfoBaks;

	public GameObject _Part_XiDian;

	public GButton _Close_XiDian;

	public GButton _Close_XiDian2;

	public GButton _Submit_XiDian;

	public TextBlock _Avalid_XiDian;

	public TextBlock _Fee_XiJian;

	public TextBlock _Fee_Desc;

	private int nFeeXidian;

	public GameObject _Part_TuiJian;

	public ShowNetImage _Bak_TuiJian;

	public GButton _Close_TuiJian;

	public GButton _Close_TuiJian2;

	public GButton _Submit_TuiJian;

	public TextBlock _Scheme_TuiJian;

	public TextBlock _tiliText;

	public TextBlock _zhiliText;

	public TextBlock _minjieText;

	public TextBlock _liliangText;

	public GameObject _MJS_Select;

	public GButton _Strength_CheckButton;

	public GButton _Intelligence_CheckButton;

	public GameObject _Strength_Check;

	public GameObject _Intelligence_Check;

	public TextBlock _strengthText;

	public TextBlock _intelligenceText;

	private static bool mStrengthActive = true;

	private static int addStrengthPoint;

	private static int addIntelligencePoint;

	private static int addDexterityPoint;

	private static int addConstitutionPoint;

	private static int Occupation;

	private static int nTotalPoint;

	private static int nRemainPoint;

	private static int nUsedPoint;

	private static int nStrengthPoint;

	private static int nIntelligencePoint;

	private static int nDexterityPoint;

	private static int nConstitutionPoint;

	public GameObject _Part_Tip;

	public TextBlock _tipsText;

	public ListBox gsListBox;

	public GButton gsBtn;

	public TextBlock GuoshiInfoText;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection gsItemCollection;

	public GButton m_renXingBtn;

	private bool IsRebirthPropBtnClick;

	private bool FirstGetNewData = true;

	public ObservableCollection[] equipIcon;

	private RoleResLoader roleResLoader;

	private GButton tempBtn;

	private Dictionary<int, RoleAttributePart.AttItem> AttItemDic = new Dictionary<int, RoleAttributePart.AttItem>();

	private int KeXiDianShu;

	private struct PropertyStr
	{
		public int ShowList;

		public bool Percent;

		public string Str;

		public double Att;
	}

	public enum TalentEffectType
	{
		PropBasic = 1,
		PropExt,
		SkillOne,
		SkillAll
	}

	private class AttItem
	{
		public AttItem(GameObject Obj, Transform Root)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(Obj);
			if (null != gameObject)
			{
				this.mNameLabel = U3DUtils.FindGameObjectByName(gameObject, "Name").GetComponent<UILabel>();
				this.mValueLabel = U3DUtils.FindGameObjectByName(gameObject, "Value").GetComponent<UILabel>();
				this.mTrans = gameObject.transform;
				this.mTrans.SetParent(Root, false);
				gameObject.SetActive(true);
			}
		}

		public string NameStr
		{
			set
			{
				this.mNameLabel.text = value;
			}
		}

		public string ValueStr
		{
			set
			{
				this.mValueLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f2e2be",
					value
				});
			}
		}

		public Vector3 Pos
		{
			set
			{
				this.mTrans.localPosition = value;
			}
		}

		private UILabel mNameLabel;

		private UILabel mValueLabel;

		private Transform mTrans;
	}
}
