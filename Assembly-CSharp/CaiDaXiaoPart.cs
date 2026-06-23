using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class CaiDaXiaoPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitOnClick();
		this.btnJiaZhuBuy.Text = Global.GetLang("确定");
		this.btnJiaZhuOFF.Text = Global.GetLang("取消");
		this.xmlVo = IConfigbase<ConfigCaiDaXiao>.Instance.GetCaiDaXiaoDataByTime(Global.GetCorrectDateTime());
		if (this.xmlVo == null)
		{
			MUDebug.Log<string>(new string[]
			{
				Global.GetLang("配置表错误：CaiDaXiao.Xml")
			});
			this.handlerClose(this, new DPSelectedItemEventArgs());
		}
		this.listPeiLv.Add(new CaiDaXiaoPart.BoCaiTypeData(DiceValueEnum.DiceMin, "xiao", "xiao_p"));
		this.listPeiLv.Add(new CaiDaXiaoPart.BoCaiTypeData(DiceValueEnum.DiceLeopard, "baozi", "baozi_p"));
		this.listPeiLv.Add(new CaiDaXiaoPart.BoCaiTypeData(DiceValueEnum.DiceMax, "da", "da_p"));
		this.InitPrefabText();
		GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_Dice);
	}

	protected override void OnDestroy()
	{
		GameInstance.Game.SendBoCaiCloseTuiSong(1);
	}

	private void InitPrefabText()
	{
		this.labJiaZhuNumberTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("购买数量：")
		});
		this.labJiaZhuTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("快速购买")
		});
		this.labKaiJiangTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("开  奖")
		});
		this.labKaiJiangMingDanTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("中奖名单")
		});
		this.labXuanGouZhuShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			string.Format(Global.GetLang("注：每注{0}欢乐代币"), this.xmlVo.XiaoHaoDaiBi)
		});
		if (this.xmlVo.ShangChengKaiGuan == 1)
		{
			this.btnShangDian.gameObject.SetActive(true);
		}
		else
		{
			this.btnShangDian.gameObject.SetActive(false);
		}
		if (this.panelJiaZhu.gameObject.activeSelf)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		}
	}

	private void InitOnClick()
	{
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handlerClose(this, new DPSelectedItemEventArgs());
		};
		this.btnAddHuoBi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenBoCaiBuyHuoBiWindow();
		};
		this.btnShangDian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenBoCaiStoreWindow(BoCaiTypeEnum.Bocai_Dice);
		};
		this.btnBangZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow("Config/CaiDaXiaoIntro.xml");
		};
		this.btnJiaZhuClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		};
		this.btnJiaZhuOFF.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		};
		this.btnJiaBack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		};
		this.btnJiaZhuNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object e2, DPSelectedItemEventArgs s2)
			{
				this.RefreshItemNumber(s2.ID);
			}, null, 0, -100);
		};
		this.btnJiaZhuClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		};
		this.btnJiaZhuJian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.jiaZhuNumber--;
			this.RefreshItemNumber(this.jiaZhuNumber);
		};
		this.btnJiaZhuJia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.jiaZhuNumber++;
			this.RefreshItemNumber(this.jiaZhuNumber);
		};
		this.btnJiaZhuBuy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendOneDice(this.onItem.ValueEnum, this.jiaZhuNumber);
		};
		this.listDice[0].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendOneDice(DiceValueEnum.DiceMin, 1);
		};
		this.listDice[1].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendOneDice(DiceValueEnum.DiceLeopard, 1);
		};
		this.listDice[2].MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SendOneDice(DiceValueEnum.DiceMax, 1);
		};
	}

	public void SendOneDice(DiceValueEnum type, int diceCount)
	{
		if (this.GetTimeType != BoCaiStageEnum.Stage_Buy)
		{
			Super.HintMainText(Global.GetLang("当前不在下注期间"), 10, 3);
			return;
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			CaiDaXiaoYiGouItem component = this.obserEndNumber.GetAt(i).GetComponent<CaiDaXiaoYiGouItem>();
			if (component == null)
			{
				break;
			}
			num += component.Number;
		}
		if (num >= this.xmlVo.ZhuShuShangXian)
		{
			Super.HintMainText(Global.GetLang("当前已经为可购最大值，不可进行购买"), 10, 3);
			return;
		}
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId);
		if (diceCount * this.xmlVo.XiaoHaoDaiBi > totalGoodsCountByID)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("当前欢乐代币不足，是否购买欢乐代币？")
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					PlayZone.GlobalPlayZone.OpenBoCaiBuyHuoBiWindow();
				}
				return true;
			};
			return;
		}
		this.panelJiaZhu.gameObject.SetActive(false);
		TCPGame game = GameInstance.Game;
		BoCaiTypeEnum bocaiType = BoCaiTypeEnum.Bocai_Dice;
		string[] array = new string[1];
		int num2 = 0;
		int num3 = (int)type;
		array[num2] = num3.ToString();
		game.SendBoCaiXiaZhu(bocaiType, diceCount, array);
	}

	private void RefrshKaiJiang(string numbers)
	{
		if (string.IsNullOrEmpty(numbers))
		{
			numbers = "-1,-1,-1";
		}
		string[] array = numbers.Split(new char[]
		{
			','
		});
		if (array.Length != 3)
		{
			this.labKaiJiangNumber.text = string.Empty;
			return;
		}
		if (this.obserKaiJiang == null)
		{
			this.obserKaiJiang = this.listBoxKaiJiang.ItemsSource;
		}
		for (int i = 0; i < array.Length; i++)
		{
			CaiShuZiItem caiShuZiItem;
			if (this.obserKaiJiang.Count != 3)
			{
				caiShuZiItem = U3DUtils.NEW<CaiShuZiItem>();
				this.obserKaiJiang.AddNoUpdate(caiShuZiItem);
				if (caiShuZiItem.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(caiShuZiItem.GetComponent<UIPanel>());
				}
			}
			else
			{
				caiShuZiItem = this.obserKaiJiang.GetAt(i).GetComponent<CaiShuZiItem>();
			}
			if (caiShuZiItem != null)
			{
				caiShuZiItem.SetIitemType(CaiShuZiType.KaiJiang);
				caiShuZiItem.SpNumber = array[i].SafeToInt32(0);
			}
		}
	}

	private void RefreshEndnumber(List<BoCaiBuyItem> list = null)
	{
		if (this.obserEndNumber == null)
		{
			this.obserEndNumber = this.listBoxEndNumber.ItemsSource;
		}
		for (int i = 0; i < 3; i++)
		{
			CaiDaXiaoYiGouItem caiDaXiaoYiGouItem;
			if (this.obserEndNumber.Count == 3)
			{
				caiDaXiaoYiGouItem = this.obserEndNumber.GetAt(i).GetComponent<CaiDaXiaoYiGouItem>();
			}
			else
			{
				caiDaXiaoYiGouItem = U3DUtils.NEW<CaiDaXiaoYiGouItem>();
				this.obserEndNumber.AddNoUpdate(caiDaXiaoYiGouItem);
			}
			if (caiDaXiaoYiGouItem == null)
			{
				break;
			}
			caiDaXiaoYiGouItem.ValueEnum = i + DiceValueEnum.DiceMin;
			int number = 0;
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].strBuyValue.SafeToInt32(0) == (int)caiDaXiaoYiGouItem.ValueEnum)
					{
						number = list[j].BuyNum;
						break;
					}
				}
			}
			caiDaXiaoYiGouItem.Number = number;
			caiDaXiaoYiGouItem.handlerNumber = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.Tag as CaiDaXiaoYiGouItem)
				{
					if (this.GetTimeType != BoCaiStageEnum.Stage_Buy)
					{
						Super.HintMainText(Global.GetLang("当前不在下注期间"), 10, 3);
						return;
					}
					int num = 0;
					for (int k = 0; k < 3; k++)
					{
						CaiDaXiaoYiGouItem component = this.obserEndNumber.GetAt(k).GetComponent<CaiDaXiaoYiGouItem>();
						if (component == null)
						{
							break;
						}
						num += component.Number;
					}
					if (num >= this.xmlVo.ZhuShuShangXian)
					{
						Super.HintMainText(Global.GetLang("当前已经为可购最大值，不可进行购买"), 10, 3);
						return;
					}
					this.onItem = (e.Tag as CaiDaXiaoYiGouItem);
					this.OpenJiaZhu();
				}
			};
		}
	}

	private void RefreshPeiLv(string value = "")
	{
		if (this.listPeiLv.Count != 3)
		{
			return;
		}
		if (string.IsNullOrEmpty(value))
		{
			this.listLabTitle[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("赔率：1.00")
			});
			this.listLabTitle[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("赔率：1.00")
			});
			this.listLabTitle[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("赔率：1.00")
			});
		}
		else
		{
			this.listLabTitle[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(Global.GetLang("赔率：{0}"), this.CompensateRate(this.listPeiLv[0].enumClass, this.BoCaiResult.Value1).ToString("f2")))
			});
			this.listLabTitle[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(Global.GetLang("赔率：{0}"), this.CompensateRate(this.listPeiLv[1].enumClass, this.BoCaiResult.Value1).ToString("f2")))
			});
			this.listLabTitle[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang(string.Format(Global.GetLang("赔率：{0}"), this.CompensateRate(this.listPeiLv[2].enumClass, this.BoCaiResult.Value1).ToString("f2")))
			});
		}
	}

	private void RefreshShangQi(List<KFBoCaoHistoryData> dataList)
	{
		if (this.BoCaiResult.OpenHistory == null || this.BoCaiResult.OpenHistory.Count <= 0)
		{
			this.labKaiJiangContent.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("                      敬请期待")
			}));
			return;
		}
		if (dataList == null)
		{
			dataList = new List<KFBoCaoHistoryData>();
		}
		StringBuilder stringBuilder = new StringBuilder();
		dataList.Sort(new Comparison<KFBoCaoHistoryData>(this.Sort));
		for (int i = 0; i < this.BoCaiResult.OpenHistory.Count; i++)
		{
			int num = 0;
			long num2 = 0L;
			for (int j = 0; j < dataList.Count; j++)
			{
				if (dataList[j].DataPeriods == this.BoCaiResult.OpenHistory[i].DataPeriods)
				{
					if (num2 != dataList[j].DataPeriods)
					{
						num2 = dataList[j].DataPeriods;
						stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							string.Format(Global.GetLang("                  {0}期{1}"), num2, Environment.NewLine)
						}));
					}
					num++;
					string[] array = dataList[j].OpenData.Split(new char[]
					{
						','
					});
					int count = array[0].SafeToInt32(0) + array[1].SafeToInt32(0) + array[2].SafeToInt32(0);
					string enumName = this.GetEnumName(count);
					string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format(Global.GetLang("{0}({1})区中{2}{3}注"), new object[]
						{
							dataList[j].RoleName,
							dataList[j].ServerID,
							enumName,
							dataList[j].BuyNum
						})
					});
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("恭喜{0}"), colorStringForNGUIText + Environment.NewLine)
					}));
				}
			}
			if (num <= 0)
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("                  {0}期{1}"), this.BoCaiResult.OpenHistory[i].DataPeriods, Environment.NewLine)
				}));
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("                  本期无人中奖")
				}) + Environment.NewLine);
			}
		}
		this.labKaiJiangContent.text = Global.GetLang(stringBuilder.ToString());
	}

	private int Sort(KFBoCaoHistoryData a, KFBoCaoHistoryData b)
	{
		if (b.DataPeriods == a.DataPeriods)
		{
			return b.BuyNum.CompareTo(a.BuyNum);
		}
		return b.DataPeriods.CompareTo(a.DataPeriods);
	}

	private string GetEnumName(int count)
	{
		if (count == 3 || count == 18)
		{
			return Global.GetLang("豹子");
		}
		if (count >= 4 && count <= 10)
		{
			return Global.GetLang("小");
		}
		if (count >= 11 && count <= 17)
		{
			return Global.GetLang("大");
		}
		return string.Empty;
	}

	public double CompensateRate(DiceValueEnum val, string info)
	{
		string[] array = info.Split(new char[]
		{
			','
		});
		long num = Convert.ToInt64(array[val - DiceValueEnum.DiceMin]);
		long num2 = Convert.ToInt64(array[0]) + Convert.ToInt64(array[1]) + Convert.ToInt64(array[2]);
		if (num2 > 0L && num > 0L)
		{
			return Math.Truncate(100.0 * (double)num2 / (double)num) / 100.0;
		}
		return 1.0;
	}

	private BoCaiStageEnum GetTimeType
	{
		get
		{
			return (BoCaiStageEnum)this.BoCaiResult.Stage;
		}
	}

	public void RefreshDaiBi()
	{
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId);
		this.labHuoBiNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			totalGoodsCountByID
		});
	}

	private void RefreshItemNumber(int number)
	{
		if (this.onItem == null)
		{
			return;
		}
		if (number < 0)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 3; i++)
		{
			CaiDaXiaoYiGouItem component = this.obserEndNumber.GetAt(i).GetComponent<CaiDaXiaoYiGouItem>();
			if (component == null)
			{
				break;
			}
			if (this.onItem.ValueEnum == component.ValueEnum)
			{
				num += number + component.Number;
			}
			else
			{
				num += component.Number;
			}
			num2 += component.Number;
		}
		int num3 = Mathf.Min(Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId) / this.xmlVo.XiaoHaoDaiBi, this.xmlVo.ZhuShuShangXian - num2);
		if (num3 <= 0)
		{
			GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("当前欢乐代币不足，是否购买欢乐代币？")
			}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					PlayZone.GlobalPlayZone.OpenBoCaiBuyHuoBiWindow();
				}
				return true;
			};
			this.panelJiaZhu.gameObject.SetActive(false);
			return;
		}
		this.jiaZhuNumber = number;
		if (this.jiaZhuNumber <= 1)
		{
			this.jiaZhuNumber = 1;
			this.btnJiaZhuJian.isEnabled = false;
		}
		else
		{
			this.btnJiaZhuJian.isEnabled = true;
		}
		if (this.jiaZhuNumber >= num3)
		{
			this.btnJiaZhuJia.isEnabled = false;
			this.jiaZhuNumber = num3;
		}
		else
		{
			this.btnJiaZhuJia.isEnabled = true;
		}
		if (this.jiaZhuNumber > 0)
		{
			this.btnJiaZhuBuy.isEnabled = true;
		}
		else
		{
			this.btnJiaZhuBuy.isEnabled = false;
		}
		this.labJiaZhuHuoBi.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("消费代币：")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.jiaZhuNumber * this.xmlVo.XiaoHaoDaiBi
		}));
		this.btnJiaZhuNumber.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			this.jiaZhuNumber
		});
	}

	public void OpenJiaZhu()
	{
		this.panelJiaZhu.gameObject.SetActive(true);
		this.RefreshItemNumber(1);
	}

	private GetBoCaiResult BoCaiResult { get; set; }

	public BoCaiOpenHistory ZhongJiangNumber
	{
		get
		{
			long num = 0L;
			int num2 = -1;
			if (this.BoCaiResult.OpenHistory != null)
			{
				for (int i = 0; i < this.BoCaiResult.OpenHistory.Count; i++)
				{
					if (this.BoCaiResult.OpenHistory[i].DataPeriods > num && !string.IsNullOrEmpty(this.BoCaiResult.OpenHistory[i].OpenValue))
					{
						num = this.BoCaiResult.OpenHistory[i].DataPeriods;
						num2 = i;
					}
				}
			}
			if (num2 >= 0)
			{
				return this.BoCaiResult.OpenHistory[num2];
			}
			return null;
		}
	}

	public void RefreshInit(GetBoCaiResult boCaiResult)
	{
		this.xmlVo = IConfigbase<ConfigCaiDaXiao>.Instance.GetCaiDaXiaoDataByTime(Global.GetCorrectDateTime());
		if (this.xmlVo == null)
		{
			MUDebug.Log<string>(new string[]
			{
				Global.GetLang("配置表错误：CaiDaXiao.Xml")
			});
			this.handlerClose(this, new DPSelectedItemEventArgs());
		}
		if (boCaiResult.BocaiType != 1)
		{
			return;
		}
		base.StopAllCoroutines();
		this.BoCaiResult = boCaiResult;
		this.RefreshDaiBi();
		this.RefreshShangQi(this.BoCaiResult.WinLotteryRoleList);
		if (this.BoCaiResult.Stage == 2)
		{
			this.listDice[0].isEnabled = true;
			this.listDice[1].isEnabled = true;
			this.listDice[2].isEnabled = true;
		}
		else
		{
			this.listDice[0].isEnabled = false;
			this.listDice[0].target.spriteName = "xiao";
			this.listDice[1].isEnabled = false;
			this.listDice[1].target.spriteName = "baozi";
			this.listDice[2].isEnabled = false;
			this.listDice[2].target.spriteName = "da";
		}
		string[] array = this.xmlVo.MeiRiKaiQi.Split(new char[]
		{
			':'
		});
		string[] array2 = this.xmlVo.MeiRiJieSu.Split(new char[]
		{
			':'
		});
		int num = Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second;
		this.timeSatr = array[0].SafeToInt32(0) * 3600 + array[1].SafeToInt32(0) * 60 + array[2].SafeToInt32(0);
		this.timeEnd = array2[0].SafeToInt32(0) * 3600 + array2[1].SafeToInt32(0) * 60 + array2[2].SafeToInt32(0);
		if (this.ZhongJiangNumber != null && !string.IsNullOrEmpty(this.ZhongJiangNumber.OpenValue) && num >= this.timeSatr + 240 + 30)
		{
			this.RefrshKaiJiang(this.ZhongJiangNumber.OpenValue);
			if (this.BoCaiResult.Stage == 5)
			{
				string[] array3 = this.ZhongJiangNumber.OpenValue.Split(new char[]
				{
					','
				});
				int num2 = array3[0].SafeToInt32(0) + array3[1].SafeToInt32(0) + array3[2].SafeToInt32(0);
				string enumName = this.GetEnumName(num2);
				this.labKaiJiangNumber.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("{0}点    {1}"), num2, enumName)
				});
			}
		}
		else
		{
			this.RefrshKaiJiang("-1,-1,-1");
			this.labKaiJiangNumber.text = string.Empty;
		}
		if (num <= this.timeSatr)
		{
			this.labTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("每日{0}:{1}-{2}:{3}开启"), new object[]
				{
					array[0],
					array[1],
					array2[0],
					array2[1]
				})
			});
			this.labKaiJiangTitme.text = string.Empty;
			this.labKaiJiangNumber.text = string.Empty;
			this.RefrshKaiJiang("-1,-1,-1");
			this.BoCaiResult.Stage = 6;
			this.RefreshEndnumber(null);
			this.RefreshPeiLv(string.Empty);
			this.labKaiJiangQi.text = string.Empty;
		}
		else if (num > this.timeSatr && num <= this.timeEnd)
		{
			base.StartCoroutine<bool>(this.StartTime());
			base.StartCoroutine<bool>(this.StartKaiJiang());
			this.RefreshEndnumber(this.BoCaiResult.ItemList);
			this.RefreshPeiLv(this.BoCaiResult.Value1);
			if (this.BoCaiResult.NowPeriods > 0L)
			{
				this.labKaiJiangQi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("{0}期"), this.BoCaiResult.NowPeriods)
				});
			}
			else
			{
				this.labKaiJiangQi.text = string.Empty;
			}
		}
		else if (num > this.timeEnd)
		{
			this.labTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("每日{0}:{1}-{2}:{3}开启"), new object[]
				{
					array[0],
					array[1],
					array2[0],
					array2[1]
				})
			});
			this.labKaiJiangTitme.text = string.Empty;
			this.BoCaiResult.Stage = 7;
			this.RefreshEndnumber(null);
			this.RefreshPeiLv(string.Empty);
			if (this.BoCaiResult.NowPeriods > 0L)
			{
				this.labKaiJiangQi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("{0}期"), this.BoCaiResult.NowPeriods)
				});
			}
			else
			{
				this.labKaiJiangQi.text = string.Empty;
			}
		}
		base.StartCoroutine<bool>(this.StartTimeNext(num));
	}

	public void RefreshBuyDaiBi(int result)
	{
		this.RefreshDaiBi();
	}

	public void RefreshBuyCaiPiao(BuyBoCaiResult boCaiResult)
	{
		if (boCaiResult.Info != 0)
		{
			Super.HintMainText(IConfigbase<ConfigCaiShuZi>.Instance.ErrorString((BocaiSysMsgErr)boCaiResult.Info), 10, 3);
			return;
		}
		Super.HintMainText(Global.GetLang("购买成功"), 10, 3);
		this.RefreshEndnumber(boCaiResult.ItemList);
		this.RefreshDaiBi();
	}

	public void RefreshCaiChi(BoCaiUpdate result)
	{
		if (result == null)
		{
			return;
		}
		if (result.BocaiType != 1)
		{
			return;
		}
		if (this.BoCaiResult.Stage == 6)
		{
			base.StopAllCoroutines();
			base.StartCoroutine<bool>(this.StartTime());
			base.StartCoroutine<bool>(this.StartKaiJiang());
		}
		this.BoCaiResult.Stage = result.Stage;
		this.BoCaiResult.OpenTime = result.OpenTime;
		this.BoCaiResult.Value1 = result.Value1;
		if (result.DataPeriods == this.BoCaiResult.NowPeriods && this.BoCaiResult.Stage == 5)
		{
			GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_Dice);
			return;
		}
		if (result.DataPeriods != this.BoCaiResult.NowPeriods && this.BoCaiResult.Stage == 2)
		{
			this.BoCaiResult.NowPeriods = result.DataPeriods;
			this.labKaiJiangQi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("{0}期"), this.BoCaiResult.NowPeriods)
			});
			List<BoCaiBuyItem> list = new List<BoCaiBuyItem>();
			this.RefreshEndnumber(list);
		}
		if (result.Stage == 2)
		{
			this.RefreshPeiLv(result.Value1);
			this.listDice[0].isEnabled = true;
			this.listDice[1].isEnabled = true;
			this.listDice[2].isEnabled = true;
		}
		else if (result.Stage == 3)
		{
			this.RefreshPeiLv(result.Value1);
			this.listDice[0].isEnabled = false;
			this.listDice[1].isEnabled = false;
			this.listDice[2].isEnabled = false;
			if (this.panelJiaZhu.gameObject.activeSelf)
			{
				this.panelJiaZhu.gameObject.SetActive(false);
			}
		}
		else
		{
			this.listDice[0].isEnabled = false;
			this.listDice[1].isEnabled = false;
			this.listDice[2].isEnabled = false;
			if (this.panelJiaZhu.gameObject.activeSelf)
			{
				this.panelJiaZhu.gameObject.SetActive(false);
			}
		}
	}

	private void OpenHelpWindow(string path = "Config/CaiDaXiaoIntro.xml")
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.mChildTransform.localPosition = new Vector3(100f, 0f, 0f);
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		ChangeableRulePart.RuleXml ruleXml = null;
		if (ruleXml == null)
		{
			XElement gameResXml = Global.GetGameResXml(path);
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Format("加载{0}出现错误", path)
				});
			}
			ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		}
		this.m_helpPart.SetHelpInfo(ruleXml.list, false);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	private IEnumerator StartTimeNext(int dateTime)
	{
		for (;;)
		{
			yield return new WaitForSeconds(2f);
			if (dateTime >= 86400 && this.BoCaiResult.Stage != 7)
			{
				GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_Dice);
			}
			else if (dateTime >= this.timeEnd && this.BoCaiResult.Stage != 7)
			{
				GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_Dice);
			}
			else if (dateTime >= this.timeSatr && dateTime <= this.timeEnd && this.BoCaiResult.Stage == 6)
			{
				GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_Dice);
			}
			dateTime += 2;
		}
		yield break;
	}

	private IEnumerator StartTime()
	{
		long nextTime = 1L;
		string second = string.Empty;
		string minue = string.Empty;
		while (this.BoCaiResult.OpenTime / 1000L <= 300L)
		{
			if (this.GetTimeType == BoCaiStageEnum.Stage_Buy && this.BoCaiResult.OpenTime / 1000L >= 30L)
			{
				long xiaZhuTime = this.BoCaiResult.OpenTime / 1000L - 30L;
				if (xiaZhuTime / 60L / 10L > 0L)
				{
					minue = (xiaZhuTime / 60L).ToString();
				}
				else
				{
					minue = string.Format("0{0}", (xiaZhuTime / 60L).ToString());
				}
				if (xiaZhuTime % 60L / 10L > 0L)
				{
					second = (xiaZhuTime % 60L).ToString();
				}
				else
				{
					second = string.Format("0{0}", (xiaZhuTime % 60L).ToString());
				}
				if (xiaZhuTime / 60L > 0L)
				{
					this.labTime.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("下注时间：")
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}:{1}", minue, second)
					}));
				}
				else
				{
					this.labTime.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("下注时间：")
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("00:{0}", second)
					}));
				}
			}
			else if (this.GetTimeType == BoCaiStageEnum.Stage_Stop || this.GetTimeType == BoCaiStageEnum.Stage_End)
			{
				this.labTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("停止下注")
				});
			}
			if (this.BoCaiResult.OpenTime / 1000L >= 0L)
			{
				long xiaZhuTime2 = this.BoCaiResult.OpenTime / 1000L;
				if (xiaZhuTime2 / 60L / 10L > 0L)
				{
					minue = (xiaZhuTime2 / 60L).ToString();
				}
				else
				{
					minue = string.Format("0{0}", (xiaZhuTime2 / 60L).ToString());
				}
				if (xiaZhuTime2 % 60L / 10L > 0L)
				{
					second = (xiaZhuTime2 % 60L).ToString();
				}
				else
				{
					second = string.Format("0{0}", (xiaZhuTime2 % 60L).ToString());
				}
				if (this.GetTimeType == BoCaiStageEnum.Stage_Buy || this.GetTimeType == BoCaiStageEnum.Stage_Stop)
				{
					this.labKaiJiangTitme.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("开奖时间：")
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}:{1}", minue, second)
					}));
				}
				else if (this.GetTimeType == BoCaiStageEnum.Stage_End)
				{
					this.labKaiJiangTitme.text = string.Format("{0}{1}", Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("进入下期：")
					}), Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}:{1}", minue, second)
					}));
				}
			}
			this.BoCaiResult.OpenTime -= nextTime * 1000L;
			yield return new WaitForSeconds((float)nextTime);
		}
		yield break;
	}

	private IEnumerator StartKaiJiang()
	{
		float nextTime = 0.1f;
		int number = 2;
		for (;;)
		{
			if (this.GetTimeType == BoCaiStageEnum.Stage_Stop)
			{
				string[] endNumbers = new string[3];
				for (int i = 0; i < endNumbers.Length; i++)
				{
					endNumbers[i] = Random.Range(1, 7).ToString();
				}
				this.RefrshKaiJiang(string.Format("{0},{1},{2}", endNumbers[0], endNumbers[1], endNumbers[2]));
				this.listDice[number].target.spriteName = this.listPeiLv[number].normal;
				number++;
				if (number > 2)
				{
					number = 0;
				}
				this.listDice[number].target.spriteName = this.listPeiLv[number].press;
			}
			yield return new WaitForSeconds(nextTime);
		}
		yield break;
	}

	public UILabel labKaiJiangTitle;

	public UILabel labKaiJiangQi;

	public UILabel labKaiJiangTitme;

	public UILabel labKaiJiangNumber;

	public UILabel labKaiJiangMingDanTitle;

	public UILabel labKaiJiangContent;

	public UILabel labXuanGouZhuShi;

	public GButton btnClose;

	public GButton btnGouMai;

	public GButton btnAddHuoBi;

	public GButton btnShangDian;

	public GButton btnBangZhu;

	public List<UILabel> listLabTitle;

	public UILabel labTime;

	public UILabel labHuoBiNumber;

	public UIPanel panelJiaZhu;

	public GButton btnJiaZhuClose;

	public GButton btnJiaZhuOFF;

	public GButton btnJiaZhuJian;

	public GButton btnJiaZhuJia;

	public GButton btnJiaZhuNumber;

	public UILabel labJiaZhuHuoBi;

	public UILabel labJiaZhuTitle;

	public UILabel labJiaZhuNumberTitle;

	public GButton btnJiaZhuBuy;

	public GButton btnJiaBack;

	public ListBox listBoxEndNumber;

	public ListBox listBoxKaiJiang;

	public List<GButton> listDice;

	public DPSelectedItemEventHandler handlerClose;

	private ObservableCollection obserEndNumber;

	private ObservableCollection obserKaiJiang;

	private CaiDaXiaoVO xmlVo;

	private int jiaZhuNumber = 1;

	private int timeSatr;

	private int timeEnd;

	private List<CaiDaXiaoPart.BoCaiTypeData> listPeiLv = new List<CaiDaXiaoPart.BoCaiTypeData>();

	private CaiDaXiaoYiGouItem onItem;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;

	public class BoCaiTypeData
	{
		public BoCaiTypeData(DiceValueEnum enumClass, string normal, string press)
		{
			this.enumClass = enumClass;
			this.normal = normal;
			this.press = press;
		}

		public DiceValueEnum enumClass;

		public string normal;

		public string press;
	}
}
