using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class JiYuanJuanXianPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_PositionJinDu = this.m_ShowJinDuTiao.transform.localPosition;
		this.InitVlaue();
		this.InitOnClick();
		this.AddPartList();
		this.AddListTween();
		this.AddGongXian();
		this.RefreshJinDu();
	}

	private void InitVlaue()
	{
		this.m_BtnJiangLi.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang("贡献奖励")
		});
		this.m_BtnJiangLi.Label.lineWidth = 115;
	}

	private void InitOnClick()
	{
		this.m_GameLingQuPanel.SetActive(false);
		this.m_BtnJiangLi.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_GameLingQuPanel.SetActive(true);
		};
		this.m_BtnLingQuColse.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_GameLingQuPanel.SetActive(false);
		};
	}

	private void AddListTween()
	{
		for (int i = 0; i < this.m_ListBack.Count; i++)
		{
			int key = i;
			UIEventListener.Get(this.m_ListBack[key]).onClick = delegate(GameObject go)
			{
				if ((int)this.m_JiYuanConfig.data.EraStage < this.m_ListItem[key].EraStage)
				{
					return;
				}
				TweenScale tweenScale = this.m_ListTweenScale[key];
				if (tweenScale.transform.localScale.x >= 1f)
				{
					this.m_ListItem[key].m_Panel.transform.localPosition = this.m_ListItem[key].m_VecPnl;
					this.m_ListItem[key].m_Panel.clipRange = this.m_ListItem[key].m_VecClipPnl;
					TweenPosition component = this.m_Table.GetComponent<TweenPosition>();
					component.from = new Vector3(this.m_Table.transform.localPosition.x, 138f, -1f);
					component.to = new Vector3(-325.7f, 138f, -1f);
					component.Reset();
					component.Play(true);
					this.m_ListItem[key].m_BtnJuanXian.Text = Global.GetLang("捐献");
					tweenScale.from = new Vector3(1f, 1f, 1f);
					tweenScale.to = new Vector3(0f, 0f, 0f);
					if (tweenScale != null)
					{
						tweenScale.Reset();
						tweenScale.Play(true);
					}
				}
				else if (tweenScale.transform.localScale.x <= 0f)
				{
					TweenPosition component2 = this.m_Table.GetComponent<TweenPosition>();
					component2.from = new Vector3(-325.7f, 138f, -1f);
					component2.to = new Vector3(this.m_Table.transform.localPosition.x - (float)key * 199f, 138f, -1f);
					component2.Reset();
					component2.Play(true);
					this.m_ListItem[key].m_BtnJuanXian.Text = Global.GetLang("返回");
					tweenScale.from = new Vector3(0f, 0f, 0f);
					tweenScale.to = new Vector3(1f, 1f, 1f);
					if (tweenScale != null)
					{
						tweenScale.Reset();
						tweenScale.Play(true);
					}
				}
			};
		}
	}

	public void RefreshItem(int EraStage, int EraStateProcess)
	{
		for (int i = 0; i < this.m_ListItem.Count; i++)
		{
			JiYuanJuanXianPartItem component = this.m_ListItem[i].GetComponent<JiYuanJuanXianPartItem>();
			if (component != null)
			{
				if (component.EraStage < EraStage)
				{
					component.m_LabJinDu.gameObject.SetActive(true);
					component.m_LabWeiWanCheng.gameObject.SetActive(false);
					component.m_BtnJuanXian.gameObject.SetActive(true);
					component.EraStateProcess = 100;
					component.m_ShowImg.URL = this.m_PathParent + this.str[i];
					component.m_ShowImg.transform.localScale = new Vector3(106f, 106f, 1f);
				}
				else if (component.EraStage == EraStage)
				{
					component.m_LabJinDu.gameObject.SetActive(true);
					component.m_LabWeiWanCheng.gameObject.SetActive(false);
					component.m_BtnJuanXian.gameObject.SetActive(true);
					component.EraStateProcess = EraStateProcess;
					component.m_ShowImg.URL = this.m_PathParent + this.str[i];
					component.m_ShowImg.transform.localScale = new Vector3(106f, 106f, 1f);
				}
				this.RefreshGoods(component, component.goodsids);
			}
		}
	}

	private void AddPartList()
	{
		Dictionary<int, EraTask>.Enumerator enumerator = this.m_JiYuanConfig.DicEraRewardJuanXian.GetEnumerator();
		int num = 0;
		this.str = this.m_JiYuanConfig.EraUI.ProgressIcon.Split(new char[]
		{
			'|'
		});
		while (enumerator.MoveNext())
		{
			JiYuanJuanXianPart.<AddPartList>c__AnonStorey26A <AddPartList>c__AnonStorey26A = new JiYuanJuanXianPart.<AddPartList>c__AnonStorey26A();
			<AddPartList>c__AnonStorey26A.<>f__this = this;
			JiYuanJuanXianPartItem jiYuanJuanXianPartItem = U3DUtils.NEW<JiYuanJuanXianPartItem>();
			UILabel labTitle = jiYuanJuanXianPartItem.m_LabTitle;
			object[] array = new object[2];
			array[0] = "dac7ae";
			int num2 = 1;
			KeyValuePair<int, EraTask> keyValuePair = enumerator.Current;
			array[num2] = Global.GetLang(keyValuePair.Value.StageName);
			labTitle.text = Global.GetColorStringForNGUIText(array);
			jiYuanJuanXianPartItem.m_BtnJuanXian.Text = Global.GetLang("捐献");
			jiYuanJuanXianPartItem.m_BtnAll.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("全部捐献")
			});
			JiYuanJuanXianPart.<AddPartList>c__AnonStorey26A <AddPartList>c__AnonStorey26A2 = <AddPartList>c__AnonStorey26A;
			KeyValuePair<int, EraTask> keyValuePair2 = enumerator.Current;
			<AddPartList>c__AnonStorey26A2.keyID = keyValuePair2.Value.EraStage;
			jiYuanJuanXianPartItem.m_BtnAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				<AddPartList>c__AnonStorey26A.<>f__this.DPSelectedItem(<AddPartList>c__AnonStorey26A.<>f__this, new DPSelectedItemEventArgs
				{
					ID = <AddPartList>c__AnonStorey26A.keyID
				});
			};
			JiYuanJuanXianPartItem jiYuanJuanXianPartItem2 = jiYuanJuanXianPartItem;
			KeyValuePair<int, EraTask> keyValuePair3 = enumerator.Current;
			jiYuanJuanXianPartItem2.EraStage = keyValuePair3.Value.EraStage;
			int eraStage = (int)this.m_JiYuanConfig.data.EraStage;
			KeyValuePair<int, EraTask> keyValuePair4 = enumerator.Current;
			if (eraStage > keyValuePair4.Value.EraStage)
			{
				jiYuanJuanXianPartItem.m_LabWeiWanCheng.gameObject.SetActive(false);
				jiYuanJuanXianPartItem.EraStateProcess = 100;
				if (num < this.str.Length)
				{
					jiYuanJuanXianPartItem.m_ShowImg.URL = this.m_PathParent + this.str[num];
					jiYuanJuanXianPartItem.m_ShowImg.transform.localScale = new Vector3(106f, 106f, 1f);
					num++;
				}
			}
			else
			{
				int eraStage2 = (int)this.m_JiYuanConfig.data.EraStage;
				KeyValuePair<int, EraTask> keyValuePair5 = enumerator.Current;
				if (eraStage2 == keyValuePair5.Value.EraStage)
				{
					jiYuanJuanXianPartItem.m_LabWeiWanCheng.gameObject.SetActive(false);
					JiYuanJuanXianPartItem jiYuanJuanXianPartItem3 = jiYuanJuanXianPartItem;
					KeyValuePair<int, EraTask> keyValuePair6 = enumerator.Current;
					jiYuanJuanXianPartItem3.EraStage = keyValuePair6.Value.EraStage;
					jiYuanJuanXianPartItem.EraStateProcess = this.m_JiYuanConfig.data.EraStateProcess;
					if (num < this.str.Length)
					{
						jiYuanJuanXianPartItem.m_ShowImg.URL = this.m_PathParent + this.str[num];
						jiYuanJuanXianPartItem.m_ShowImg.transform.localScale = new Vector3(106f, 106f, 1f);
						num++;
					}
				}
				else
				{
					jiYuanJuanXianPartItem.m_LabJinDu.gameObject.SetActive(false);
					jiYuanJuanXianPartItem.m_LabWeiWanCheng.gameObject.SetActive(true);
					jiYuanJuanXianPartItem.m_BtnJuanXian.gameObject.SetActive(false);
					UILabel labWeiWanCheng = jiYuanJuanXianPartItem.m_LabWeiWanCheng;
					object[] array2 = new object[2];
					array2[0] = "ecb36c";
					int num3 = 1;
					string lang = Global.GetLang("需要完成第");
					KeyValuePair<int, EraTask> keyValuePair7 = enumerator.Current;
					array2[num3] = lang + (keyValuePair7.Value.EraStage - 1).ToString() + Global.GetLang("纪元");
					labWeiWanCheng.text = Global.GetColorStringForNGUIText(array2);
					if (num < this.str.Length)
					{
						jiYuanJuanXianPartItem.m_ShowImg.URL = this.m_PathParent + "suo.png";
						jiYuanJuanXianPartItem.m_ShowImg.transform.localScale = new Vector3(24f, 36f, 1f);
						num++;
					}
				}
			}
			UIPanel component = jiYuanJuanXianPartItem.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			string text = string.Empty;
			int num4 = 0;
			for (;;)
			{
				int num5 = num4;
				KeyValuePair<int, EraTask> keyValuePair8 = enumerator.Current;
				if (num5 >= keyValuePair8.Value.CompletionCondition.Split(new char[]
				{
					'|'
				}).Length)
				{
					break;
				}
				string text2 = string.Empty;
				int num6 = num4;
				KeyValuePair<int, EraTask> keyValuePair9 = enumerator.Current;
				if (num6 >= keyValuePair9.Value.CompletionCondition.Split(new char[]
				{
					'|'
				}).Length - 1)
				{
					KeyValuePair<int, EraTask> keyValuePair10 = enumerator.Current;
					text2 = keyValuePair10.Value.CompletionCondition.Split(new char[]
					{
						'|'
					})[num4] + ",1,0,0,0,0";
				}
				else
				{
					KeyValuePair<int, EraTask> keyValuePair11 = enumerator.Current;
					text2 = keyValuePair11.Value.CompletionCondition.Split(new char[]
					{
						'|'
					})[num4] + ",1,0,0,0,0|";
				}
				text += text2;
				num4++;
			}
			if (!string.IsNullOrEmpty(text))
			{
				jiYuanJuanXianPartItem.m_ObservableCollection = jiYuanJuanXianPartItem.m_ListBox.ItemsSource;
				Super.LoadGoodsList(text, jiYuanJuanXianPartItem.m_ObservableCollection);
			}
			UIPanel[] componentsInChildren = jiYuanJuanXianPartItem.m_ListBox.GetComponentsInChildren<UIPanel>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (component != null)
					{
						Object.Destroy(componentsInChildren[i]);
					}
				}
			}
			jiYuanJuanXianPartItem.goodsids = text;
			this.RefreshGoods(jiYuanJuanXianPartItem, text);
			this.m_ListBack.Add(jiYuanJuanXianPartItem.m_Bck);
			this.m_ListTweenScale.Add(jiYuanJuanXianPartItem.m_Tween);
			this.m_ListItem.Add(jiYuanJuanXianPartItem);
			jiYuanJuanXianPartItem.transform.SetParent(this.m_Table.transform, false);
		}
	}

	private void RefreshGoods(JiYuanJuanXianPartItem item, string goodsids)
	{
		GGoodIcon[] GGoodIcons = item.m_ListBox.GetComponentsInChildren<GGoodIcon>();
		int num = 0;
		if (GGoodIcons != null)
		{
			for (int i = 0; i < GGoodIcons.Length; i++)
			{
				bool flag = true;
				int key = i;
				int num2 = 0;
				if (Global.Data.roleData.GoodsDataList == null || Global.Data.roleData.GoodsDataList.Count <= 0)
				{
					GGoodIcons[i].SecondText.Text = "0/" + goodsids.Split(new char[]
					{
						'|'
					})[i].Split(new char[]
					{
						','
					})[1];
					GGoodIcons[i].ContentText.gameObject.SetActive(false);
				}
				else
				{
					for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
					{
						if (GGoodIcons[i].ItemCode == Global.Data.roleData.GoodsDataList[j].GoodsID)
						{
							num2 += Global.Data.roleData.GoodsDataList[j].GCount;
							if (num2 > 9999)
							{
								GGoodIcons[i].SecondText.Text = "9999/" + goodsids.Split(new char[]
								{
									'|'
								})[i].Split(new char[]
								{
									','
								})[1];
							}
							else
							{
								GGoodIcons[i].SecondText.Text = num2.ToString() + "/" + goodsids.Split(new char[]
								{
									'|'
								})[i].Split(new char[]
								{
									','
								})[1];
							}
							GGoodIcons[i].ContentText.gameObject.SetActive(false);
							flag = false;
						}
						if (flag && j >= Global.Data.roleData.GoodsDataList.Count - 1)
						{
							GGoodIcons[i].SecondText.Text = "0/" + goodsids.Split(new char[]
							{
								'|'
							})[i].Split(new char[]
							{
								','
							})[1];
							GGoodIcons[i].ContentText.gameObject.SetActive(false);
						}
					}
				}
				GGoodIcons[key].addEventListener("click", delegate(MouseEvent s)
				{
					GGoodIcon ggoodIcon = s.target.SafeGetComponent<GGoodIcon>();
					if (null == ggoodIcon)
					{
						return;
					}
					GoodsData goodsData = GGoodIcons[key].ItemObject as GoodsData;
					if (goodsData == null)
					{
						return;
					}
					GTipServiceEx.ShowTip(GGoodIcons[key], TipTypes.GoodsText, GoodsOwnerTypes.JiYuanShouJi, goodsData);
				});
				num += num2;
			}
			if (num <= 0)
			{
				item.m_BtnAll.isEnabled = false;
				item.m_SpGanTanHao.gameObject.SetActive(false);
			}
			else
			{
				item.m_BtnAll.isEnabled = true;
				if (this.m_JiYuanConfig.PaiHangBangTime > Global.GetCorrectLocalTime())
				{
					item.m_SpGanTanHao.gameObject.SetActive(true);
				}
				else
				{
					item.m_SpGanTanHao.gameObject.SetActive(false);
				}
			}
		}
	}

	public void RefreshGongXian(int ID)
	{
		for (int i = 0; i < this.m_ObservableCollection.Length; i++)
		{
			JiYuanJuanXianJiangLiItem component = this.m_ObservableCollection.GetAt(i).GetComponent<JiYuanJuanXianJiangLiItem>();
			if (component != null && component.ID == ID)
			{
				component.m_Btn.gameObject.SetActive(false);
				component.m_spYiLingQu.gameObject.SetActive(true);
				this.m_ListSp[i].spriteName = "GongXianDu_BaoXiang02";
				this.m_ListJinDuNumber[i].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_ListJinDuNumber[i].text
				});
				this.m_LingQuCount--;
				this.m_ListGmTeXiao[i].SetActive(false);
			}
		}
		if (this.m_LingQuCount > 0)
		{
			this.m_SpJiangLiGanTan.gameObject.SetActive(true);
		}
		else
		{
			this.m_SpJiangLiGanTan.gameObject.SetActive(false);
		}
	}

	public void AddGongXian()
	{
		this.m_LingQuCount = 0;
		this.m_SpJiangLiGanTan.gameObject.SetActive(false);
		this.m_ObservableCollection = this.m_ListLingJiang.ItemsSource;
		this.m_ObservableCollection.Clear();
		for (int i = 0; i < this.m_VecLab.Length; i++)
		{
			this.m_ListJinDuNumber[i].transform.localPosition = new Vector3(this.m_VecLab[i], 0f, 0f);
		}
		for (int j = 0; j < this.m_VecImg.Length; j++)
		{
			this.m_ListSp[j].transform.localPosition = new Vector3(this.m_VecImg[j], 0f, 0f);
		}
		Dictionary<int, EraReward>.Enumerator enumerator = this.m_JiYuanConfig.DicEraRewardGongXian.GetEnumerator();
		while (enumerator.MoveNext())
		{
			JiYuanJuanXianPart.<AddGongXian>c__AnonStorey26D <AddGongXian>c__AnonStorey26D = new JiYuanJuanXianPart.<AddGongXian>c__AnonStorey26D();
			JiYuanJuanXianJiangLiItem jiYuanJuanXianJiangLiItem = U3DUtils.NEW<JiYuanJuanXianJiangLiItem>();
			UILabel labTitle = jiYuanJuanXianJiangLiItem.m_LabTitle;
			object[] array = new object[2];
			array[0] = "e3b36c";
			int num = 1;
			KeyValuePair<int, EraReward> keyValuePair = enumerator.Current;
			array[num] = Global.GetLang(keyValuePair.Value.RewardName);
			labTitle.text = Global.GetColorStringForNGUIText(array);
			JiYuanJuanXianPart.<AddGongXian>c__AnonStorey26D <AddGongXian>c__AnonStorey26D2 = <AddGongXian>c__AnonStorey26D;
			KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
			<AddGongXian>c__AnonStorey26D2.awardid = keyValuePair2.Value.ID;
			jiYuanJuanXianJiangLiItem.ID = <AddGongXian>c__AnonStorey26D.awardid;
			int num2 = -1;
			if (this.m_JiYuanConfig.data.EraAwardStateDict != null && this.m_JiYuanConfig.data.EraAwardStateDict.ContainsKey(<AddGongXian>c__AnonStorey26D.awardid))
			{
				num2 = this.m_JiYuanConfig.data.EraAwardStateDict[<AddGongXian>c__AnonStorey26D.awardid];
			}
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.EraDonate);
			KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
			if (roleCommonUseParamsValue >= keyValuePair3.Value.Contribution)
			{
				if (num2 == 0 || num2 == -1)
				{
					jiYuanJuanXianJiangLiItem.m_Btn.isEnabled = true;
					jiYuanJuanXianJiangLiItem.m_Btn.gameObject.SetActive(true);
					jiYuanJuanXianJiangLiItem.m_Btn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
					{
						GameInstance.Game.SenEraLingQu(<AddGongXian>c__AnonStorey26D.awardid);
					};
					this.m_SpJiangLiGanTan.gameObject.SetActive(true);
					this.m_LingQuCount++;
				}
				else if (num2 == 1)
				{
					jiYuanJuanXianJiangLiItem.m_Btn.isEnabled = false;
					jiYuanJuanXianJiangLiItem.m_Btn.gameObject.SetActive(false);
					jiYuanJuanXianJiangLiItem.m_spYiLingQu.gameObject.SetActive(true);
				}
			}
			else
			{
				jiYuanJuanXianJiangLiItem.m_Btn.isEnabled = false;
				jiYuanJuanXianJiangLiItem.m_Btn.gameObject.SetActive(true);
			}
			string text = string.Empty;
			KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
			text = keyValuePair4.Value.LeaderReward;
			if (!string.IsNullOrEmpty(text))
			{
				jiYuanJuanXianJiangLiItem.m_ObservableCollection = jiYuanJuanXianJiangLiItem.m_ListBOX.ItemsSource;
				Super.LoadGoodsList(text, jiYuanJuanXianJiangLiItem.m_ObservableCollection);
				GGoodIcon[] componentsInChildren = jiYuanJuanXianJiangLiItem.m_ListBOX.GetComponentsInChildren<GGoodIcon>();
				if (componentsInChildren != null)
				{
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						if (componentsInChildren[k].GetComponent<UIPanel>() != null)
						{
							Object.Destroy(componentsInChildren[k].GetComponent<UIPanel>());
						}
					}
				}
			}
			this.m_ObservableCollection.AddNoUpdate(jiYuanJuanXianJiangLiItem);
			if (jiYuanJuanXianJiangLiItem.GetComponent<UIPanel>() != null)
			{
				Object.Destroy(jiYuanJuanXianJiangLiItem.GetComponent<UIPanel>());
			}
		}
	}

	private float RefreshJinDuValue(int value)
	{
		float num = 0f;
		if ((float)value <= this.m_Nmbers[0])
		{
			return num + (float)value / this.m_Nmbers[0] * (this.m_VecLab[0] / this.m_VecLab[4]);
		}
		num += this.m_VecLab[0] / this.m_VecLab[4];
		if ((float)value <= this.m_Nmbers[1])
		{
			return num + ((float)value - this.m_Nmbers[0]) / (this.m_Nmbers[1] - this.m_Nmbers[0]) * ((this.m_VecLab[1] - this.m_VecLab[0]) / this.m_VecLab[4]);
		}
		num += (this.m_VecLab[1] - this.m_VecLab[0]) / this.m_VecLab[4];
		if ((float)value <= this.m_Nmbers[2])
		{
			return num + ((float)value - this.m_Nmbers[1]) / (this.m_Nmbers[2] - this.m_Nmbers[1]) * ((this.m_VecLab[2] - this.m_VecLab[1]) / this.m_VecLab[4]);
		}
		num += (this.m_VecLab[2] - this.m_VecLab[1]) / this.m_VecLab[4];
		if ((float)value <= this.m_Nmbers[3])
		{
			return num + ((float)value - this.m_Nmbers[2]) / (this.m_Nmbers[3] - this.m_Nmbers[2]) * ((this.m_VecLab[3] - this.m_VecLab[2]) / this.m_VecLab[4]);
		}
		num += (this.m_VecLab[3] - this.m_VecLab[2]) / this.m_VecLab[4];
		if ((float)value <= this.m_Nmbers[4])
		{
			return num + ((float)value - this.m_Nmbers[3]) / (this.m_Nmbers[4] - this.m_Nmbers[3]) * ((this.m_VecLab[4] - this.m_VecLab[3]) / this.m_VecLab[4]);
		}
		return num + (this.m_VecLab[4] - this.m_VecLab[3]) / this.m_VecLab[4];
	}

	public void RefreshJinDu()
	{
		this.m_ShowJinDuTiao.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.DonationScheduleColor;
		this.m_LabGongXianDu.text = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.EraDonate).ToString();
		int[] array = new int[this.m_JiYuanConfig.DicEraRewardGongXian.Count];
		int num = 0;
		Dictionary<int, EraReward>.Enumerator enumerator = this.m_JiYuanConfig.DicEraRewardGongXian.GetEnumerator();
		while (enumerator.MoveNext())
		{
			int[] array2 = array;
			int num2 = num;
			KeyValuePair<int, EraReward> keyValuePair = enumerator.Current;
			array2[num2] = keyValuePair.Key;
			float[] nmbers = this.m_Nmbers;
			int num3 = num;
			KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
			nmbers[num3] = (float)keyValuePair2.Key;
			UILabel uilabel = this.m_ListJinDuNumber[num];
			object[] array3 = new object[2];
			array3[0] = "e3b36c";
			int num4 = 1;
			KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
			array3[num4] = keyValuePair3.Key;
			uilabel.text = Global.GetColorStringForNGUIText(array3);
			num++;
		}
		float num5 = this.RefreshJinDuValue(Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.EraDonate));
		if (num5 >= 1f)
		{
			num5 = 1f;
		}
		else if (num5 <= 0f)
		{
			num5 = 0f;
		}
		this.m_ShowJinDuTiao.transform.localPosition = new Vector3(this.m_PositionJinDu.x + (1f - num5) * this.m_ShowJinDuTiao.transform.localScale.x, this.m_PositionJinDu.y, this.m_PositionJinDu.z);
		this.m_UIpanelJinDu.transform.localPosition = new Vector3(-(1f - num5) * this.m_ShowJinDuTiao.transform.localScale.x, 0f, 0f);
		for (int i = 0; i < this.m_JiYuanConfig.DicEraRewardGongXian.Count; i++)
		{
			if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.EraDonate) >= array[i])
			{
				int num6 = -1;
				if (this.m_JiYuanConfig.data.EraAwardStateDict != null && this.m_JiYuanConfig.DicEraRewardGongXian.ContainsKey(array[i]) && this.m_JiYuanConfig.data.EraAwardStateDict.ContainsKey(this.m_JiYuanConfig.DicEraRewardGongXian[array[i]].ID))
				{
					num6 = this.m_JiYuanConfig.data.EraAwardStateDict[this.m_JiYuanConfig.DicEraRewardGongXian[array[i]].ID];
				}
				if (i <= this.m_ListSp.Count - 1 && num6 == 1)
				{
					this.m_ListSp[i].spriteName = "GongXianDu_BaoXiang02";
					this.m_ListJinDuNumber[i].text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						array[i]
					});
					this.m_ListGmTeXiao[i].SetActive(false);
				}
				else
				{
					this.m_ListGmTeXiao[i].SetActive(true);
				}
			}
			else
			{
				this.m_ListGmTeXiao[i].SetActive(false);
			}
		}
	}

	public UISprite m_SpJiangLiGanTan;

	public GButton m_BtnJiangLi;

	public UITable m_Table;

	public GameObject m_GameLingQuPanel;

	public GButton m_BtnLingQuColse;

	public ListBox m_ListLingJiang;

	public UILabel m_LabGongXianDu;

	public ShowNetImage m_ShowJinDuTiao;

	public UIPanel m_UIpanelJinDu;

	public List<UILabel> m_ListJinDuNumber;

	public List<UISprite> m_ListSp;

	public List<GameObject> m_ListGmTeXiao;

	public List<GameObject> m_ListBack = new List<GameObject>();

	public List<TweenScale> m_ListTweenScale = new List<TweenScale>();

	public List<JiYuanJuanXianPartItem> m_ListItem = new List<JiYuanJuanXianPartItem>();

	public ObservableCollection m_ObservableCollection;

	public JiYuanConfig m_JiYuanConfig;

	public string m_PathParent = "NetImages/GameRes/Images/JiYuanHuoDong/";

	public DPSelectedItemEventHandler DPSelectedItem;

	public Vector3 m_PositionJinDu;

	public int m_LingQuCount;

	private string[] str = new string[4];

	private float[] m_VecImg = new float[]
	{
		64.3f,
		167.1f,
		267.6f,
		372.7f,
		474.3f
	};

	private float[] m_VecLab = new float[]
	{
		92f,
		195f,
		293f,
		395f,
		498f
	};

	private float[] m_Nmbers = new float[]
	{
		100f,
		500f,
		800f,
		1000f,
		2000f
	};
}
