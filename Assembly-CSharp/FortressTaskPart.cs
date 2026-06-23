using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FortressTaskPart : UserControl
{
	public static void ErrorHint(int ret)
	{
		switch (ret + 1)
		{
		case 0:
			Super.HintMainText(Global.GetLang("任务领取失败"), 10, 3);
			break;
		case 1:
			Super.HintMainText(Global.GetLang("任务领取成功"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("没有创建要塞"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("当天任务次数已达上限，不能进行任务"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("没有未执行的任务"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
			break;
		case 6:
			Super.HintMainText(Global.GetLang("配置出错"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("没有任务数据"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("不存在空闲的精灵可以指派"), 10, 3);
			break;
		case 9:
			Super.HintMainText(Global.GetLang("任务还没有完成"), 10, 3);
			break;
		default:
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
			break;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	public override void Update()
	{
		base.Update();
		if (this.TaskState == 3 && this.mYaoSaiMissionData != null && DateTime.MaxValue != this.mYaoSaiMissionData.StartTime && DateTime.MinValue != this.mYaoSaiMissionData.StartTime && this.mConfigPetMissionXml.GetPetMissionVO() != null)
		{
			float time = this.mConfigPetMissionXml.GetPetMissionVO().Time;
			long num = Global.GetCorrectDateTime().Ticks - this.mYaoSaiMissionData.StartTime.Ticks;
			if (0L <= num)
			{
				TimeSpan timeSpan;
				timeSpan..ctor(num);
				this.TaskInfLabels[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("持续时间：") + this.TimeToString(Mathf.CeilToInt((float)((double)time - timeSpan.TotalSeconds)))
				});
				if ((double)time <= timeSpan.TotalSeconds)
				{
					this.TaskState = 1;
				}
				else
				{
					this.TaskState = 3;
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=red>服务器往前调时间</color>"
				});
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		PlayZone.GlobalPlayZone.CloseBaodianGuideWindow();
		if (null != this.MSGGChildWindow)
		{
			this.MSGGChildWindow.NotifyClose(-1);
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.MSGGChildWindow);
		}
	}

	private void InitPrefabText()
	{
		this.TitleLabels[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("要塞任务")
		});
		this.TitleLabels[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("精灵阵容")
		});
		this.TaskInfLabels[0].text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("持续时间：")
		});
		this.TaskInfLabels[1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("推荐精灵:")
		});
		this.TaskInfLabels[2].text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("任务成功率:")
		});
		this.TaskInfLabels[3].text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("任务奖励：")
		});
		this.PetShowInfLabel.text = string.Empty;
		this.TaskInfLabels[1].pivot = 5;
		this.TaskInfLabels[1].transform.localPosition = new Vector3(115f, 0f, 0f);
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		try
		{
			this.BtnColse.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0
					});
				}
			};
			this.mLiftOBC = this.LiftViewListBox.ItemsSource;
			this.mRightAwardIocnOBC = this.TaskAwardlistbox.ItemsSource;
			this.LiftViewListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.PetSelectChange);
			byte b = 0;
			while ((int)b < this.PetArrayIocnRoots.Length)
			{
				FortressTaskPart.PetArrayBtnHander petArrayBtnHander = new FortressTaskPart.PetArrayBtnHander(this.PetArrayColseBtnObjs[(int)b], this.PetArrayIocnRoots[(int)b], (int)b);
				petArrayBtnHander.ClosePet();
				petArrayBtnHander.Hander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (s != null)
					{
						FortressTaskPart.PetArrayBtnHander petArrayBtnHander2 = this.mPetArrayBtnHanders.Find((FortressTaskPart.PetArrayBtnHander f) => f.Index == s.ID);
						if (petArrayBtnHander2 != null)
						{
							petArrayBtnHander2.ClosePet();
							if (petArrayBtnHander2.PetGoodsData != null)
							{
								int id = petArrayBtnHander2.PetGoodsData.Id;
								for (int i = 0; i < this.mLiftOBC.Count; i++)
								{
									GameObject at = this.mLiftOBC.GetAt(i);
									if (null != at)
									{
										FortressTaskPetItem component = at.GetComponent<FortressTaskPetItem>();
										if (null != component && component.GGoodsData.Id == id)
										{
											component.PetState = 0;
											break;
										}
									}
								}
							}
							this.RefreshPetViewRank();
							this.RefreshProbability();
						}
					}
				};
				this.mPetArrayBtnHanders.Add(petArrayBtnHander);
				b += 1;
			}
			this.TaskZhiXingBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mTaskState == 0)
				{
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("PetMissionMax", ',');
					if (systemParamIntArrayByName.Length > 0 && JingLingMap.inst.ExcuteMissionCount >= systemParamIntArrayByName[0])
					{
						FortressTaskPart.ErrorHint(2);
						return;
					}
					string text = string.Empty;
					List<int> list = new List<int>();
					byte b2 = 0;
					while ((int)b2 < this.PetArrayIocnRoots.Length)
					{
						FortressTaskPart.PetArrayBtnHander petArrayBtnHander2 = this.mPetArrayBtnHanders[(int)b2];
						if (petArrayBtnHander2 != null && petArrayBtnHander2.HavePet == 1)
						{
							list.Add(petArrayBtnHander2.PetGoodsData.Id);
						}
						b2 += 1;
					}
					if (0 < list.Count)
					{
						byte b3 = 0;
						byte b4 = (byte)list.Count;
						while (b3 < b4)
						{
							if (b3 != b4 - 1)
							{
								text = text + list[(int)b3] + "|";
							}
							else
							{
								text += list[(int)b3];
							}
							b3 += 1;
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						GameInstance.Game.SendJingLingYaiSaiZhiXingtask(this.mYaoSaiMissionData.SiteID, text);
					}
					else
					{
						Super.HintMainText(Global.GetLang("最少派出一只精灵才能执行任务"), 10, 3);
					}
				}
				else if (this.mTaskState == 3 || this.mTaskState == 2)
				{
					string[] buttons = new string[]
					{
						Global.GetLang("确认"),
						Global.GetLang("取消")
					};
					this.MSGGChildWindow = Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("确认放弃当前已执行的任务？"), delegate(object a, DPSelectedItemEventArgs b)
					{
						if (b.ID == 0)
						{
							MUDebug.Log<string>(new string[]
							{
								"<color=yellow>小爷要放弃任务  建筑ID为：" + this.mYaoSaiMissionData.SiteID + "</color>"
							});
							GameInstance.Game.SendJingLingYaiSaitaskQuit(this.mYaoSaiMissionData.SiteID);
						}
						this.MSGGChildWindow = null;
					}, buttons);
				}
			};
			this.HelpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				int num = ConfigSystemParam.GetSystemParamIntArrayByName("PetMissionMax", ',')[1] / 60 / 60;
				string[] array = new string[]
				{
					Global.GetLang("刷新任务规则："),
					string.Concat(new string[]
					{
						Global.GetLang("1、通过刷新任务，"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("最多")
						}),
						Global.GetLang("可以在地图中刷满"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("5个\n\r任务")
						}),
						Environment.NewLine,
						Global.GetLang("2、任务有："),
						Global.GetColorStringForNGUIText(new object[]
						{
							"fdf7dd",
							Global.GetLang("白")
						}),
						Global.GetLang("、"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("绿")
						}),
						Global.GetLang("、"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"3681f3",
							Global.GetLang("蓝")
						}),
						Global.GetLang("、"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"b266ff",
							Global.GetLang("紫")
						}),
						Global.GetLang("四个品质，"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("品质\n\r")
						}),
						Global.GetLang("越高奖励越好"),
						Environment.NewLine,
						Global.GetLang("3、"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("进行中")
						}),
						Global.GetLang("和"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("完成未领奖")
						}),
						Global.GetLang("的任务"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							Global.GetLang("不会")
						}),
						Global.GetLang("被刷新"),
						Environment.NewLine,
						Global.GetLang("4、每"),
						num.ToString(),
						Global.GetLang("小时可以"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("免费")
						}),
						Global.GetLang("刷新一次任务，"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							Global.GetLang("非免费")
						}),
						Global.GetLang("时\n\r刷新消耗"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("钻石")
						}),
						Environment.NewLine,
						Global.GetLang("5、每天执行任务有"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							Global.GetLang("上限")
						}),
						Global.GetLang("，推荐刷新执行"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("高品\n\r质")
						}),
						Global.GetLang("的任务，可以获得"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("更多奖励")
						}),
						Environment.NewLine,
						Global.GetColorStringForNGUIText(new object[]
						{
							"fac60d",
							Global.GetLang("任务成功率规则：")
						}),
						Environment.NewLine,
						Global.GetLang("1、上阵精灵"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("总等级")
						}),
						Global.GetLang("越高，提升"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("成功率")
						}),
						Global.GetLang("越多"),
						Environment.NewLine,
						Global.GetLang("2、上阵精灵"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("总卓越属性条数")
						}),
						Global.GetLang("越多，提高"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("成功\n\r率")
						}),
						Global.GetLang("越多"),
						Environment.NewLine,
						Global.GetLang("3、上阵任务要求的"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("特殊精灵")
						}),
						Global.GetLang("可以额外提升"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("成\n\r功率")
						}),
						Environment.NewLine,
						Global.GetLang("4、指派精灵进行任务后，任务"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("倒计时结束，\n\r根据")
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("成功率")
						}),
						Global.GetLang("判断任务结果"),
						Environment.NewLine,
						Global.GetLang("5、任务失败，只能获得"),
						Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							Global.GetLang("部分")
						}),
						Global.GetLang("奖励"),
						Environment.NewLine
					})
				};
				Global.ShowHelpPart(Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					array[0]
				}), Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					array[1]
				}), true, -15f);
			};
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void PetSelectChange(object sender, MouseEvent e)
	{
		GameObject selectedItem = this.LiftViewListBox.SelectedItem;
		if (null != selectedItem)
		{
			if (this.mTaskState == 0)
			{
				FortressTaskPetItem component = selectedItem.GetComponent<FortressTaskPetItem>();
				if (component.PetState == 0)
				{
					byte b = 0;
					while ((int)b < this.mPetArrayBtnHanders.Count)
					{
						FortressTaskPart.PetArrayBtnHander petArrayBtnHander = this.mPetArrayBtnHanders[(int)b];
						if (petArrayBtnHander.HavePet == 0)
						{
							this.mPetArrayBtnHanders[(int)b].AddPetArray(component.GGoodsData, (int)b);
							component.PetState = 2;
							break;
						}
						b += 1;
					}
					List<int> list = new List<int>();
					byte b2 = 0;
					while ((int)b2 < this.mPetArrayBtnHanders.Count)
					{
						FortressTaskPart.PetArrayBtnHander petArrayBtnHander2 = this.mPetArrayBtnHanders[(int)b2];
						if (petArrayBtnHander2.HavePet == 1 && petArrayBtnHander2.PetGoodsData.GoodsID == this.mConfigPetMissionXml.GetPetMissionVO().SpecialPet)
						{
							list.Add(petArrayBtnHander2.PetGoodsData.Id);
						}
						b2 += 1;
					}
					byte b3 = 0;
					for (int i = 0; i < this.mLiftOBC.Count; i++)
					{
						GameObject at = this.mLiftOBC.GetAt(i);
						if (null != at)
						{
							FortressTaskPetItem component2 = at.GetComponent<FortressTaskPetItem>();
							if (null != component2 && component2.GGoodsData != null && 0 < list.Count && this.mConfigPetMissionXml.GetPetMissionVO().SpecialPet == component2.GGoodsData.GoodsID && !list.Contains(component2.GGoodsData.Id))
							{
								component2.Percent = this.GetPetPercent(component2.GGoodsData, 1);
								b3 = 1;
							}
						}
					}
					if (b3 == 1)
					{
						this.RefreshPetViewRank();
					}
				}
			}
			this.RefreshProbability();
		}
	}

	private void RefreshPetViewRank()
	{
		List<FortressTaskPart.FortressGoodsData> list = new List<FortressTaskPart.FortressGoodsData>();
		List<FortressTaskPart.FortressGoodsData> list2 = new List<FortressTaskPart.FortressGoodsData>();
		List<int> list3 = new List<int>();
		byte b2 = 0;
		while ((int)b2 < this.mPetArrayBtnHanders.Count)
		{
			FortressTaskPart.PetArrayBtnHander petArrayBtnHander = this.mPetArrayBtnHanders[(int)b2];
			if (petArrayBtnHander.HavePet == 1 && petArrayBtnHander.PetGoodsData.GoodsID == this.mConfigPetMissionXml.GetPetMissionVO().SpecialPet)
			{
				list3.Add(petArrayBtnHander.PetGoodsData.Id);
			}
			b2 += 1;
		}
		for (int i = 0; i < this.mLiftOBC.Count; i++)
		{
			GameObject at = this.mLiftOBC.GetAt(i);
			if (null != at)
			{
				FortressTaskPetItem component = at.GetComponent<FortressTaskPetItem>();
				if (null != component)
				{
					FortressTaskPart.FortressGoodsData pgd = default(FortressTaskPart.FortressGoodsData);
					pgd.GGoodsData = component.GGoodsData;
					pgd.Precent = component.Percent;
					pgd.State = component.PetState;
					int num = list3.FindIndex((int e) => e == pgd.GGoodsData.Id);
					if (component.PetState == 2)
					{
						if (num == 0)
						{
							pgd.Precent = this.GetPetPercent(pgd.GGoodsData, 0);
						}
						else
						{
							pgd.Precent = this.GetPetPercent(pgd.GGoodsData, 1);
						}
					}
					else if (component.PetState == 0)
					{
						if (0 < list3.Count)
						{
							pgd.Precent = this.GetPetPercent(pgd.GGoodsData, 1);
						}
						else
						{
							pgd.Precent = this.GetPetPercent(pgd.GGoodsData, 0);
						}
					}
					if (pgd.GGoodsData.Site == 10001)
					{
						list2.Add(pgd);
					}
					else
					{
						list.Add(pgd);
					}
				}
			}
		}
		List<FortressTaskPart.FortressGoodsData> list4 = new List<FortressTaskPart.FortressGoodsData>();
		if (0 < list.Count)
		{
			list.Sort((FortressTaskPart.FortressGoodsData b, FortressTaskPart.FortressGoodsData a) => (int)a.Precent - (int)b.Precent);
			list4.AddRange(list);
		}
		if (0 < list2.Count)
		{
			list2.Sort((FortressTaskPart.FortressGoodsData b, FortressTaskPart.FortressGoodsData a) => (int)a.Precent - (int)b.Precent);
			list4.AddRange(list2);
		}
		try
		{
			for (int j = 0; j < list4.Count; j++)
			{
				GameObject at2 = this.mLiftOBC.GetAt(j);
				if (null != at2)
				{
					FortressTaskPetItem component2 = at2.GetComponent<FortressTaskPetItem>();
					if (null != component2)
					{
						component2.RefreshPetInf(list4[j].GGoodsData, this.mConfigPetMissionXml.GetPetMissionVO(), 1);
						component2.Percent = list4[j].Precent;
						component2.PetState = list4[j].State;
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
	}

	private void RefreshProbability()
	{
		float num = 0f;
		byte b = 0;
		for (int i = 0; i < this.mPetArrayBtnHanders.Count; i++)
		{
			if (this.mPetArrayBtnHanders[i].HavePet == 1)
			{
				if (this.mConfigPetMissionXml.GetPetMissionVO().SpecialPet == this.mPetArrayBtnHanders[i].PetGoodsData.GoodsID)
				{
					b += 1;
				}
				if (2 > b)
				{
					num += this.GetPetPercent(this.mPetArrayBtnHanders[i].PetGoodsData, 0);
				}
				else
				{
					num += this.GetPetPercent(this.mPetArrayBtnHanders[i].PetGoodsData, 1);
				}
			}
		}
		Transform transform = this.TaskSpecialPetRoot.FindChild("specialPetIocn");
		if (null != transform)
		{
			GGoodIcon component = transform.GetComponent<GGoodIcon>();
			if (null != component)
			{
				if (b == 0)
				{
					component.GoodImg.ToGrayBitmap = true;
				}
				else
				{
					component.GoodImg.ToGrayBitmap = false;
				}
			}
		}
		this.Probability += num;
	}

	private GGoodIcon GetGoodIcon(GoodsData data)
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
			int goodsQuality = Super.GetGoodsQuality(data.GoodsID);
			if (goodsQuality == 4 || goodsQuality == 6)
			{
				ggoodIcon.TeXiao.gameObject.SetActive(true);
			}
			else
			{
				ggoodIcon.TeXiao.gameObject.SetActive(false);
			}
			ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				GGoodIcon ggoodIcon2 = e as GGoodIcon;
				if (null != ggoodIcon2)
				{
					GoodsData goodsData = ggoodIcon2.ItemObject as GoodsData;
					if (goodsData != null)
					{
						GTipServiceEx.ShowTip(ggoodIcon2, TipTypes.GoodsText, GoodsOwnerTypes.jingLingYaoSai, goodsData);
					}
				}
			};
		}
		return ggoodIcon;
	}

	private void AddAwardIcon(GoodsData gd, string ObjName)
	{
		GGoodIcon ggoodIcon = null;
		Transform transform = this.TaskAwardlistbox.transform.FindChild(ObjName);
		if (null != transform)
		{
			ggoodIcon = transform.GetComponent<GGoodIcon>();
			if (null == ggoodIcon)
			{
				Object.Destroy(transform.gameObject);
			}
		}
		if (null == ggoodIcon)
		{
			ggoodIcon = this.GetGoodIcon(gd);
			ggoodIcon.name = ObjName;
			this.mRightAwardIocnOBC.Add(ggoodIcon);
		}
	}

	private float GetPetPercent(GoodsData gd, byte containSpecialPet = 0)
	{
		float num = 0f;
		PetMissionVO petMissionVO = this.mConfigPetMissionXml.GetPetMissionVO();
		num += (float)(Mathf.FloorToInt((float)((gd.Forge_level + 1) / petMissionVO.PetLevelStep)) + 1) * petMissionVO.PetLevelStepRate;
		num += (float)(Mathf.FloorToInt((float)(Global.GetZhuoYueAttribute(gd).Count / petMissionVO.ExcellentStep)) + 1) * petMissionVO.ExcellentStepRate;
		if (containSpecialPet == 0 && petMissionVO.SpecialPet == gd.GoodsID)
		{
			num += (float)petMissionVO.SpecialPetRate;
		}
		return num;
	}

	private string TimeToString(int second)
	{
		int num = second % 60;
		int num2 = (second - num) / 3600;
		int number = (second - num) / 60 - num2 * 60;
		return string.Concat(new string[]
		{
			this.GetTwoOrderNumber(num2),
			":",
			this.GetTwoOrderNumber(number),
			":",
			this.GetTwoOrderNumber(num)
		});
	}

	private string GetTwoOrderNumber(int number)
	{
		if (10 > number)
		{
			return "0" + number.ToString();
		}
		return number.ToString();
	}

	private IEnumerator RefreshViewContent(List<GoodsData> PetList)
	{
		if (0 >= PetList.Count)
		{
			this.PetShowInfLabel.text = Global.GetLang("请将背包中的精灵派驻到精灵要塞");
			this.PetShowInfLabel.transform.localScale = Vector3.one * 18f;
		}
		else
		{
			this.PetShowInfLabel.text = string.Empty;
		}
		this.mConfigPetMissionXml = new ConfigPetMissionXml(this.mYaoSaiMissionData.MissionID);
		if (this.mConfigPetMissionXml != null)
		{
			PetMissionVO vo = this.mConfigPetMissionXml.GetPetMissionVO();
			if (vo != null)
			{
				this.TitleLabels[0].text = vo.TitleColorString;
				this.TaskInfLabels[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("持续时间：") + this.TimeToString((int)vo.Time)
				});
				this.Probability = this.Probability;
				GGoodIcon specialPetIocn = null;
				Transform specialPetIocnTrans = this.TaskSpecialPetRoot.FindChild("specialPetIocn");
				if (null != specialPetIocnTrans)
				{
					specialPetIocn = specialPetIocnTrans.GetComponent<GGoodIcon>();
					if (null == specialPetIocn)
					{
						Object.Destroy(specialPetIocnTrans.gameObject);
					}
				}
				if (null == specialPetIocn)
				{
					specialPetIocn = this.GetGoodIcon(Global.GetEmptyGoodsData(vo.SpecialPet, 0, vo.Quality, 0, 1, 0, 1, 0, 0));
					specialPetIocn.name = "specialPetIocn";
					specialPetIocn.transform.SetParent(this.TaskSpecialPetRoot, false);
					specialPetIocn.transform.localPosition = new Vector3(0f, 0f, -0.5f);
					specialPetIocn.petLevel.text = string.Empty;
					this.RefreshProbability();
				}
				string[] AddAwardArray = this.mConfigPetMissionXml.GetAwardStrAray(true);
				if (AddAwardArray != null)
				{
					for (int i = 0; i < AddAwardArray.Length; i++)
					{
						string[] strarray_ = AddAwardArray[i].Split(new char[]
						{
							','
						});
						string ObjName = string.Empty;
						if (strarray_[0].SafeToInt32(0) == 8038)
						{
							ObjName = "lingJing";
						}
						else if (strarray_[0].SafeToInt32(0) == 8036)
						{
							ObjName = "shenJiJiFen";
						}
						else if (8039 <= strarray_[0].SafeToInt32(0) && 8043 > strarray_[0].SafeToInt32(0))
						{
							ObjName = "nenglinghexin" + strarray_[0];
						}
						else
						{
							ObjName = "JianZhu";
						}
						this.AddAwardIcon(Global.GetEmptyGoodsData(strarray_[0].SafeToInt32(0), 1, 1, 0, strarray_[1].SafeToInt32(0), 1, 1, 1, 1), ObjName);
					}
					this.TaskAwardlistbox.repositionNow = true;
				}
				if (PetList != null)
				{
					yield return null;
					List<GoodsData> NowRenWuPetGoodsDatalst = new List<GoodsData>();
					List<GoodsData> HaveRenWuPetGoodsDatalst = new List<GoodsData>();
					for (int j = 0; j < PetList.Count; j++)
					{
						if (PetList[j].Site == 10001)
						{
							HaveRenWuPetGoodsDatalst.Add(PetList[j]);
						}
						else
						{
							NowRenWuPetGoodsDatalst.Add(PetList[j]);
						}
					}
					List<GoodsData> RenWuPetGoodsDatalst = new List<GoodsData>();
					if (0 < NowRenWuPetGoodsDatalst.Count)
					{
						NowRenWuPetGoodsDatalst.Sort((GoodsData b, GoodsData a) => (int)this.GetPetPercent(a, 0) - (int)this.GetPetPercent(b, 0));
						RenWuPetGoodsDatalst.AddRange(NowRenWuPetGoodsDatalst);
					}
					if (0 < HaveRenWuPetGoodsDatalst.Count)
					{
						HaveRenWuPetGoodsDatalst.Sort((GoodsData b, GoodsData a) => (int)this.GetPetPercent(a, 0) - (int)this.GetPetPercent(b, 0));
						RenWuPetGoodsDatalst.AddRange(HaveRenWuPetGoodsDatalst);
					}
					yield return null;
					this.LiftViewListBox.transform.localPosition = new Vector3(0f, 185f, 0f);
					for (int k = 0; k < RenWuPetGoodsDatalst.Count; k++)
					{
						byte BReturn = 0;
						GoodsData d = RenWuPetGoodsDatalst[k];
						FortressTaskPetItem item = null;
						GameObject obj = this.mLiftOBC.GetAt(k);
						if (null != obj)
						{
							item = obj.GetComponent<FortressTaskPetItem>();
							if (null == item)
							{
								Object.DestroyImmediate(obj.gameObject);
							}
						}
						if (null == item)
						{
							item = U3DUtils.NEW<FortressTaskPetItem>();
							this.mLiftOBC.AddNoUpdate(item);
							BReturn = 1;
						}
						item.RefreshPetInf(d, this.mConfigPetMissionXml.GetPetMissionVO(), 0);
						item.Percent = this.GetPetPercent(d, 0);
						item.DraggablePanel = this.DragPanelPet;
						if (d.Site == 10001)
						{
							item.PetState = 1;
						}
						if (BReturn == 1)
						{
							yield return null;
						}
					}
				}
			}
		}
		this.RefreshProbability();
		yield break;
	}

	private void RefreshTaskState(YaoSaiMissionData data)
	{
		if (data != null)
		{
			if (data.State == 3)
			{
				if (data.StartTime != DateTime.MinValue && data.StartTime != DateTime.MaxValue)
				{
					if (this.mConfigPetMissionXml.GetPetMissionVO() != null)
					{
						float time = this.mConfigPetMissionXml.GetPetMissionVO().Time;
						long num = Global.GetCorrectDateTime().Ticks - data.StartTime.Ticks;
						if (0L <= num)
						{
							TimeSpan timeSpan;
							timeSpan..ctor(num);
							if ((double)time <= timeSpan.TotalSeconds)
							{
								this.TaskState = 1;
							}
							else
							{
								this.TaskState = 3;
							}
						}
						else
						{
							this.TaskState = 3;
							MUDebug.Log<string>(new string[]
							{
								"<color=red>服务器往前调时间</color>"
							});
						}
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"<color=red>xml读取失败 任务的ID是：" + data.MissionID + "</color>"
						});
					}
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"<color=red>服务器的开始时间不对</color>"
					});
				}
			}
			else
			{
				this.TaskState = (byte)data.State;
			}
		}
	}

	private void RefreshTaskPet(YaoSaiMissionData data)
	{
		if (data.State == 3)
		{
			string zhiPaiJingLing = data.ZhiPaiJingLing;
			if (!string.IsNullOrEmpty(zhiPaiJingLing))
			{
				string[] array = zhiPaiJingLing.Split(new char[]
				{
					'|'
				});
				if (0 < array.Length)
				{
					for (int i = 0; i < array.Length; i++)
					{
						int num = array[i].SafeToInt32(0);
						if (0 < num)
						{
							GoodsData jingLingYaoSaiGoodsDataByDbID = Global.GetJingLingYaoSaiGoodsDataByDbID(num, 0);
							if (jingLingYaoSaiGoodsDataByDbID != null)
							{
								if (i < this.mPetArrayBtnHanders.Count)
								{
									this.mPetArrayBtnHanders[i].AddPetArray(jingLingYaoSaiGoodsDataByDbID, i);
								}
							}
							else
							{
								MUDebug.Log<string>(new string[]
								{
									"<color=yellow>PetDBId = " + num.ToString() + "</color>"
								});
							}
						}
					}
				}
			}
		}
	}

	public void YaiSaitaskQuit(int ret)
	{
		if (ret == 0)
		{
			this.TaskState = 0;
			byte b = 0;
			while ((int)b < this.mPetArrayBtnHanders.Count)
			{
				this.mPetArrayBtnHanders[(int)b].ClosePet();
				b += 1;
			}
			this.TaskInfLabels[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang("持续时间：") + this.TimeToString((int)this.mConfigPetMissionXml.GetPetMissionVO().Time)
			});
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
		}
	}

	public void YaiSaiTaskZhiXingCallBack(int ret)
	{
		if (ret == 0)
		{
			GameInstance.Game.SendGetJingLingYaiSaiData(Global.Data.roleData.RoleID);
			PlayZone.GlobalPlayZone.CloseFortressPart();
		}
		else
		{
			FortressTaskPart.ErrorHint(ret);
		}
	}

	public void YaoSaiTaskPetStateHaveChange(GoodsData pet)
	{
		for (int i = 0; i < this.mLiftOBC.Count; i++)
		{
			GameObject at = this.mLiftOBC.GetAt(i);
			if (null != at)
			{
				FortressTaskPetItem component = at.GetComponent<FortressTaskPetItem>();
				if (null != component && pet.Id == component.GGoodsData.Id)
				{
					component.RefreshPetInf(pet, this.mConfigPetMissionXml.GetPetMissionVO(), 0);
				}
			}
		}
	}

	public void RefreshTaskInF(YaoSaiMissionData data)
	{
		this.mYaoSaiMissionData = data;
		if (this.mYaoSaiMissionData != null)
		{
			base.StartCoroutine<bool>(this.RefreshViewContent(Global.GetRolePaiPets(true)));
			this.RefreshTaskPet(data);
			this.RefreshTaskState(data);
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=red>错误信息 YaoSaiMissionData 为null</color>"
			});
			if (this.Hander != null)
			{
				this.Hander(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		}
	}

	public float Probability
	{
		get
		{
			PetMissionVO petMissionVO = this.mConfigPetMissionXml.GetPetMissionVO();
			if (petMissionVO != null)
			{
				return petMissionVO.SuccessRate;
			}
			return 0f;
		}
		set
		{
			try
			{
				this.TaskInfLabels[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("任务成功率:")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					value.ToString("0") + "%"
				});
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=red>" + ex.Message + "</color>"
				});
			}
		}
	}

	public byte TaskState
	{
		get
		{
			return this.mTaskState;
		}
		set
		{
			this.mTaskState = value;
			if (this.mTaskState == 3 || this.mTaskState == 2 || this.mTaskState == 1)
			{
				for (int i = 0; i < this.mPetArrayBtnHanders.Count; i++)
				{
					this.mPetArrayBtnHanders[i].HaveTask = 1;
				}
				if (this.mTaskState == 2 || this.mTaskState == 1)
				{
					if (this.Hander != null)
					{
						this.Hander(this, new DPSelectedItemEventArgs
						{
							ID = 0
						});
					}
				}
				else
				{
					this.TaskZhiXingBtn.Label.text = Global.GetLang("放弃任务");
					this.TaskZhiXingBtn.normalSprite = "GetAwardBtn0";
					this.TaskZhiXingBtn.hoverSprite = "GetAwardBtn1";
					this.TaskZhiXingBtn.pressedSprite = "GetAwardBtn1";
					this.TaskZhiXingBtn.disabledSprite = "GetAwardBtn1";
					this.TaskZhiXingBtn.Refresh();
				}
			}
			else if (this.mTaskState == 0)
			{
				for (int j = 0; j < this.mPetArrayBtnHanders.Count; j++)
				{
					this.mPetArrayBtnHanders[j].HaveTask = 0;
				}
				this.TaskZhiXingBtn.Label.text = Global.GetLang("执行任务");
				this.TaskZhiXingBtn.normalSprite = "BtnN";
				this.TaskZhiXingBtn.hoverSprite = "BtnP";
				this.TaskZhiXingBtn.pressedSprite = "BtnP";
				this.TaskZhiXingBtn.disabledSprite = "BtnP";
				this.TaskZhiXingBtn.Refresh();
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=red>错误，未知状态" + value.ToString() + "</color>"
				});
			}
		}
	}

	[SerializeField]
	private UIDraggablePanel DragPanelPet;

	[SerializeField]
	private ListBox LiftViewListBox;

	[SerializeField]
	private GButton BtnColse;

	[SerializeField]
	private UILabel[] TitleLabels;

	[SerializeField]
	private UILabel[] TaskInfLabels;

	[SerializeField]
	private Transform TaskSpecialPetRoot;

	[SerializeField]
	private ListBox TaskAwardlistbox;

	[SerializeField]
	private UILabel PetShowInfLabel;

	[SerializeField]
	private Transform[] PetArrayIocnRoots;

	[SerializeField]
	private GameObject[] PetArrayColseBtnObjs;

	[SerializeField]
	private GButton TaskZhiXingBtn;

	[SerializeField]
	private GButton HelpBtn;

	private ObservableCollection mLiftOBC;

	private ConfigPetMissionXml mConfigPetMissionXml;

	private ObservableCollection mRightAwardIocnOBC;

	private List<FortressTaskPart.PetArrayBtnHander> mPetArrayBtnHanders = new List<FortressTaskPart.PetArrayBtnHander>();

	private YaoSaiMissionData mYaoSaiMissionData;

	private byte mTaskState;

	private GChildWindow MSGGChildWindow;

	public DPSelectedItemEventHandler Hander;

	private struct FortressGoodsData
	{
		public float Precent;

		public GoodsData GGoodsData;

		public byte State;
	}

	private class PetArrayBtnHander
	{
		public PetArrayBtnHander(GameObject CloseBtns, Transform petArrayRoot, int index)
		{
			this.Index = index;
			this.mCloseBtns = CloseBtns;
			this.mPetArrayRoot = petArrayRoot;
		}

		private bool ActiveColseBtn
		{
			get
			{
				return NGUITools.GetActive(this.mCloseBtns.gameObject);
			}
			set
			{
				if (this.mHaveTask == 1)
				{
					NGUITools.SetActive(this.mCloseBtns, false);
				}
				else
				{
					NGUITools.SetActive(this.mCloseBtns, value);
				}
			}
		}

		public byte HaveTask
		{
			set
			{
				this.mHaveTask = value;
				if (this.mHaveTask == 1)
				{
					if (this.ActiveColseBtn)
					{
						this.ActiveColseBtn = false;
					}
				}
			}
		}

		public byte HavePet
		{
			get
			{
				if (0 < this.mPetArrayRoot.childCount)
				{
					Transform child = this.mPetArrayRoot.GetChild(0);
					if (null != child)
					{
						return (!NGUITools.GetActive(child.gameObject)) ? 0 : 1;
					}
				}
				return 0;
			}
		}

		public GoodsData PetGoodsData
		{
			get
			{
				return this.mPetGoodsData;
			}
		}

		public void ClosePet()
		{
			if (0 < this.mPetArrayRoot.childCount)
			{
				Transform child = this.mPetArrayRoot.GetChild(0);
				if (null != child)
				{
					NGUITools.SetActive(child.gameObject, false);
				}
			}
			this.ActiveColseBtn = false;
			this.HaveTask = 0;
		}

		public void AddPetArray(GoodsData gd, int index)
		{
			try
			{
				this.mPetGoodsData = gd;
				GGoodIcon ggoodIcon = null;
				if (0 < this.mPetArrayRoot.childCount)
				{
					Transform child = this.mPetArrayRoot.GetChild(0);
					if (null != child)
					{
						ggoodIcon = child.GetComponent<GGoodIcon>();
						if (null == ggoodIcon)
						{
							Object.Destroy(child.gameObject);
						}
					}
				}
				GoodVO goodVO = null;
				if (null == ggoodIcon)
				{
					ggoodIcon = this.GetGoodIcon(gd, out goodVO);
					ggoodIcon.transform.SetParent(this.mPetArrayRoot, false);
				}
				else
				{
					goodVO = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				}
				ggoodIcon.Width = 50.0;
				ggoodIcon.Height = 50.0;
				ggoodIcon.ItemObject = gd;
				ggoodIcon.ItemCode = goodVO.ID;
				ggoodIcon.TipType = 1;
				ggoodIcon.ItemCategory = goodVO.Categoriy;
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodVO), string.Empty);
				ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
				NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
				ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
				ggoodIcon.BackgroundSprite1.transform.localPosition = new Vector3(ggoodIcon.BackgroundSprite1.transform.localPosition.x, ggoodIcon.BackgroundSprite1.transform.localPosition.y, -0.001f);
				ggoodIcon.BackSpriteName1 = string.Empty;
				ggoodIcon.TeXiao.gameObject.SetActive(false);
				Super.InitGoodsGIcon(ggoodIcon, gd, Global.CanUseGoods(goodVO.ID, false, true), IconTextTypes.Qianghua);
				ggoodIcon.petLevel.text = string.Empty;
				NGUITools.SetActive(ggoodIcon.gameObject, true);
				NGUITools.SetActive(this.mCloseBtns, true);
				UIEventListener.Get(this.mCloseBtns).onClick = delegate(GameObject e)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						ID = index
					});
				};
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		private GGoodIcon GetGoodIcon(GoodsData data, out GoodVO vo)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			vo = goodsXmlNodeByID;
			GGoodIcon ggoodIcon = null;
			if (goodsXmlNodeByID != null)
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			}
			return ggoodIcon;
		}

		public DPSelectedItemEventHandler Hander;

		private GameObject mCloseBtns;

		private Transform mPetArrayRoot;

		public int Index;

		private GoodsData mPetGoodsData;

		private byte mHaveTask;
	}
}
