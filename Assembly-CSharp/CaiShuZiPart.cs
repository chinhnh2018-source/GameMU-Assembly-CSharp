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

public class CaiShuZiPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.panelJiaZhu.gameObject.SetActive(false);
		this.InitPrefabText();
		this.InitXuanHao();
		this.InitOnClick();
		this.InitEndNumber(this.endNumbers);
		GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_CaiNum);
	}

	protected override void OnDestroy()
	{
		base.StopAllCoroutines();
		GameInstance.Game.SendBoCaiCloseTuiSong(2);
	}

	private void InitPrefabText()
	{
		this.btnJiaZhuBuy.Text = Global.GetLang("确定");
		this.btnRandom.Text = Global.GetLang("随机选码");
		this.btnGouMai.Text = Global.GetLang("购买");
		this.labKaiJiangTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("开奖区域")
		}));
		this.labYiGouTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("已购号码")
		}));
		this.labXuanNmberTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("选码区域")
		}));
		this.labOnNumberTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("待选区域")
		}));
		this.labJiaZhuTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("彩票加注")
		}));
		this.labJiLiTitle.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("购买记录")
		}));
		this.labJiLiNull.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("无开奖记录")
		}));
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
			PlayZone.GlobalPlayZone.OpenBoCaiStoreWindow(BoCaiTypeEnum.Bocai_CaiNum);
		};
		this.btnBangZhu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenHelpWindow("Config/CaiShuZiIntro.xml");
		};
		this.btnRandom.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RandomNumber();
		};
		this.btnGouMai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			for (int i = 0; i < this.endNumbers.Length; i++)
			{
				if (this.endNumbers[i].SafeToInt32(0) < 0 || this.endNumbers[i].SafeToInt32(0) > 9)
				{
					Super.HintMainText(Global.GetLang("请选择号码"), 10, 3);
					return;
				}
			}
			this.jiaZhuNumber = 1;
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId);
			if (this.jiaZhuNumber * this.xmlVo.XiaoHaoDaiBi > totalGoodsCountByID)
			{
				this.jiaZhuNumber = 0;
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
			GameInstance.Game.SendBoCaiXiaZhu(BoCaiTypeEnum.Bocai_CaiNum, this.jiaZhuNumber, this.endNumbers);
		};
		this.btnJiaZhuNumber.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object e2, DPSelectedItemEventArgs s2)
			{
				this.AddJiaZhuNumber(s2.ID);
			}, null, 0, -100);
		};
		this.btnJiaZhuClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		};
		this.btnJiaZhuJian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.jiaZhuNumber--;
			this.AddJiaZhuNumber(this.jiaZhuNumber);
		};
		this.btnJiaZhuJia.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.jiaZhuNumber++;
			this.AddJiaZhuNumber(this.jiaZhuNumber);
		};
		this.btnJiaZhuBuy.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
			int totalGoodsCountByID = Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId);
			if (this.jiaZhuNumber * this.xmlVo.XiaoHaoDaiBi > totalGoodsCountByID)
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
			GameInstance.Game.SendBoCaiXiaZhu(BoCaiTypeEnum.Bocai_CaiNum, this.jiaZhuNumber, this.jiaZhuNumbers);
		};
		this.btnJiaBack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiaZhu.gameObject.SetActive(false);
		};
		this.btnJiLuClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.panelJiLu.gameObject.SetActive(false);
		};
		this.btnJiLuOpen.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenJiLu();
		};
	}

	private void InitXuanHao()
	{
		this.obserXuanHao = this.listXuanHao.ItemsSource;
		for (int i = 0; i < 10; i++)
		{
			CaiShuZiItem item = U3DUtils.NEW<CaiShuZiItem>();
			if (item != null)
			{
				this.obserXuanHao.AddNoUpdate(item);
				if (item.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(item.GetComponent<UIPanel>());
				}
				item.SetIitemType(CaiShuZiType.XuanHao);
				item.Number = i;
				item.btnCloseNumber.gameObject.SetActive(false);
				item.dpsNumber = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.OnNumber(item.Number);
				};
			}
		}
	}

	private void InitEndNumber(string[] endNumbers)
	{
		if (this.obserEndNumber == null)
		{
			this.obserEndNumber = this.listEndNumber.ItemsSource;
		}
		if (this.obserEndNumber.Count > 0)
		{
			for (int i = 0; i < this.obserEndNumber.Count; i++)
			{
				CaiShuZiItem item = this.obserEndNumber.GetAt(i).GetComponent<CaiShuZiItem>();
				if (item != null)
				{
					if (item.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(item.GetComponent<UIPanel>());
					}
					item.SetIitemType(CaiShuZiType.EndNumber);
					item.Key = i;
					item.EndNumber = endNumbers[i].SafeToInt32(0);
					item.btnNumber.isEnabled = false;
					item.dpsCloseNumber = delegate(object s, DPSelectedItemEventArgs e)
					{
						this.OnCloseNumber(item.Key);
					};
				}
			}
		}
		else
		{
			this.obserEndNumber.Clear();
			for (int j = 0; j < endNumbers.Length; j++)
			{
				CaiShuZiItem item = U3DUtils.NEW<CaiShuZiItem>();
				if (item != null)
				{
					this.obserEndNumber.AddNoUpdate(item);
					if (item.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(item.GetComponent<UIPanel>());
					}
					item.SetIitemType(CaiShuZiType.EndNumber);
					item.Key = j;
					item.EndNumber = endNumbers[j].SafeToInt32(0);
					item.btnNumber.isEnabled = false;
					item.dpsCloseNumber = delegate(object s, DPSelectedItemEventArgs e)
					{
						this.OnCloseNumber(item.Key);
					};
				}
			}
		}
		this.listEndNumber.SelectionChanged = delegate(object s, MouseEvent e)
		{
			CaiShuZiItem component = this.listEndNumber.SelectedItem.GetComponent<CaiShuZiItem>();
			if (component.EndNumber < 0)
			{
				for (int k = 0; k < this.obserEndNumber.Count; k++)
				{
					CaiShuZiItem component2 = this.obserEndNumber.GetAt(k).GetComponent<CaiShuZiItem>();
					if (component2.Key != component.Key)
					{
						component2.SpOnBack = false;
					}
				}
				this.keyNumber = component.Key;
				component.SpOnBack = true;
			}
			else
			{
				this.listEndNumber.SelectedIndex = this.keyNumber;
			}
		};
	}

	public void AddJiaZhuNumber(int number)
	{
		this.jiaZhuNumber = number;
		int totalGoodsCountByID = Global.GetTotalGoodsCountByID(IConfigbase<ConfigDaiBiShiYong>.Instance.daiBiGoodId);
		int num = Mathf.Min(totalGoodsCountByID / this.xmlVo.XiaoHaoDaiBi, 9999);
		if (num > 0)
		{
			if (this.jiaZhuNumber >= num)
			{
				this.jiaZhuNumber = num;
				this.btnJiaZhuJia.isEnabled = false;
			}
			else
			{
				this.btnJiaZhuJia.isEnabled = true;
			}
			if (this.jiaZhuNumber <= 1)
			{
				this.jiaZhuNumber = 1;
				this.btnJiaZhuJian.isEnabled = false;
			}
			else
			{
				this.btnJiaZhuJian.isEnabled = true;
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
				"e3b36c",
				this.jiaZhuNumber * this.xmlVo.XiaoHaoDaiBi
			}));
			this.btnJiaZhuNumber.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				this.jiaZhuNumber
			});
			return;
		}
		this.panelJiaZhu.gameObject.SetActive(false);
		if (this.jiaZhuNumber * this.xmlVo.XiaoHaoDaiBi > totalGoodsCountByID)
		{
			this.jiaZhuNumber = 0;
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
	}

	public void OpenJiaZhu(string[] numbers)
	{
		if (numbers.Length != 5)
		{
			return;
		}
		for (int i = 0; i < numbers.Length; i++)
		{
			if (numbers[i].SafeToInt32(0) < 0 || numbers[i].SafeToInt32(0) > 9)
			{
				Super.HintMainText(Global.GetLang("请选择号码"), 10, 3);
				return;
			}
		}
		this.jiaZhuNumbers = numbers;
		if (this.obserJiaZhu == null)
		{
			this.obserJiaZhu = this.listBoxJiaZhu.ItemsSource;
		}
		this.obserJiaZhu.Clear();
		for (int j = 0; j < numbers.Length; j++)
		{
			CaiShuZiItem caiShuZiItem = U3DUtils.NEW<CaiShuZiItem>();
			if (caiShuZiItem != null)
			{
				this.obserJiaZhu.AddNoUpdate(caiShuZiItem);
				if (caiShuZiItem.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(caiShuZiItem.GetComponent<UIPanel>());
				}
				caiShuZiItem.SetIitemType(CaiShuZiType.JiaZhu);
				caiShuZiItem.SpNumber = numbers[j].SafeToInt32(0);
			}
		}
		this.panelJiaZhu.gameObject.SetActive(true);
		this.AddJiaZhuNumber(1);
	}

	private void OnNumber(int number)
	{
		for (int i = 0; i < this.obserEndNumber.Count; i++)
		{
			CaiShuZiItem component = this.obserEndNumber.GetAt(i).GetComponent<CaiShuZiItem>();
			if (component != null && this.keyNumber == component.Key)
			{
				component.EndNumber = number;
				this.endNumbers[i] = number.ToString();
				break;
			}
		}
		this.RefreshXuanNumber();
	}

	private void RefreshXuanNumber()
	{
		for (int i = this.obserEndNumber.Count - 1; i >= 0; i--)
		{
			CaiShuZiItem component = this.obserEndNumber.GetAt(i).GetComponent<CaiShuZiItem>();
			if (component != null && component.EndNumber < 0)
			{
				this.listEndNumber.SelectedIndex = component.Key;
				break;
			}
		}
	}

	private void OnCloseNumber(int key)
	{
		CaiShuZiItem component = this.obserEndNumber.GetAt(key).GetComponent<CaiShuZiItem>();
		if (component != null)
		{
			this.endNumbers[key] = "-1";
			component.EndNumber = -1;
		}
		this.listEndNumber.SelectedIndex = key;
	}

	private void RandomNumber()
	{
		for (int i = 0; i < this.endNumbers.Length; i++)
		{
			this.endNumbers[i] = Random.Range(0, 10).ToString();
		}
		this.InitEndNumber(this.endNumbers);
	}

	private void RefreshKaiJiang(string[] numbers)
	{
		if (numbers.Length != this.maxNumbers)
		{
			return;
		}
		if (this.obserKaiJiang == null)
		{
			this.obserKaiJiang = this.listKaiJiang.ItemsSource;
		}
		if (this.obserKaiJiang.Count <= 0)
		{
			for (int i = 0; i < numbers.Length; i++)
			{
				CaiShuZiItem caiShuZiItem = U3DUtils.NEW<CaiShuZiItem>();
				if (caiShuZiItem != null)
				{
					this.obserKaiJiang.AddNoUpdate(caiShuZiItem);
					if (caiShuZiItem.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(caiShuZiItem.GetComponent<UIPanel>());
					}
					caiShuZiItem.SetIitemType(CaiShuZiType.KaiJiang);
					caiShuZiItem.Key = i;
					caiShuZiItem.SpNumber = numbers[i].SafeToInt32(0);
				}
			}
		}
		else
		{
			for (int j = 0; j < this.obserKaiJiang.Count; j++)
			{
				CaiShuZiItem component = this.obserKaiJiang.GetAt(j).GetComponent<CaiShuZiItem>();
				if (component != null)
				{
					component.SetIitemType(CaiShuZiType.KaiJiang);
					component.SpNumber = numbers[j].SafeToInt32(0);
				}
			}
		}
	}

	private void RefreshYiGou(List<BoCaiBuyItem> numberLists)
	{
		if (this.obserYiGou == null)
		{
			this.obserYiGou = this.listYiGou.ItemsSource;
		}
		this.obserYiGou.Clear();
		for (int i = 0; i < numberLists.Count; i++)
		{
			if (numberLists[i].DataPeriods == this.BoCaiResult.NowPeriods && (this.BoCaiResult.Stage == 2 || this.BoCaiResult.Stage == 3))
			{
				CaiShuZiYiGouItem item = U3DUtils.NEW<CaiShuZiYiGouItem>();
				this.obserYiGou.AddNoUpdate(item);
				if (item.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(item.GetComponent<UIPanel>());
				}
				if (item.GetComponent<BoxCollider>() != null)
				{
					Object.Destroy(item.GetComponent<BoxCollider>());
				}
				item.SetData(numberLists[i].strBuyValue, numberLists[i].BuyNum);
				item.handler = delegate(object s, DPSelectedItemEventArgs e)
				{
					this.OpenJiaZhu(item.Number.Split(new char[]
					{
						','
					}));
				};
			}
		}
	}

	private int Sort(KFBoCaoHistoryData a, KFBoCaoHistoryData b)
	{
		if (a.WinNo == b.WinNo)
		{
			return b.BuyNum.CompareTo(a.BuyNum);
		}
		return a.WinNo.CompareTo(b.WinNo);
	}

	private void RefreshShangQi(List<KFBoCaoHistoryData> list)
	{
		StringBuilder stringBuilder = new StringBuilder();
		float num = 0f;
		float x = this.labShangQiContent.transform.localScale.x;
		if (this.ZhongJiangNumber != null && list != null && list.Count > 0)
		{
			list.Sort(new Comparison<KFBoCaoHistoryData>(this.Sort));
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("  上期中奖名单：")
			}));
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].DataPeriods == this.ZhongJiangNumber.DataPeriods)
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang("  恭喜")
					}));
					num += 10f * x;
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format(Global.GetLang("{0}{1}区中{2}等奖{3}注,"), new object[]
						{
							list[i].RoleName,
							list[i].ZoneID,
							list[i].WinNo,
							list[i].BuyNum
						})
					}));
					num = num + 5f * x + (float)list[i].RoleName.Length * x + (float)(list[i].ZoneID.ToString().Length / 2) * x + x * 2f;
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("获得欢乐代币{0}枚"), Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							list[i].WinMoney
						}))
					}));
					num += 7f * x + (float)(list[i].WinMoney.ToString().Length / 2) * x;
					num += 10f;
				}
			}
		}
		this.labShangQiContent.text = Global.GetLang(stringBuilder.ToString());
		TweenPosition component = this.labShangQiContent.GetComponent<TweenPosition>();
		if (component == null)
		{
			this.labShangQiContent.gameObject.AddComponent<TweenPosition>();
		}
		component.style = 1;
		component.delay = 0f;
		if (list.Count <= 2)
		{
			component.duration = 20f;
		}
		else
		{
			component.duration = (float)(list.Count * 10);
		}
		component.from = new Vector3(480f, -2f, -1f);
		component.to = new Vector3(-480f - num, -2f, -1f);
		component.Reset();
		component.Play(true);
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

	private GetBoCaiResult BoCaiResult { get; set; }

	private void OpenJiLu()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		if (this.BoCaiResult == null || this.BoCaiResult.OpenHistory == null || this.BoCaiResult.OpenHistory.Count <= 0 || this.BoCaiResult.OpenHistory[0].DataPeriods <= 0L)
		{
			this.labJiLiNull.gameObject.SetActive(true);
		}
		else
		{
			this.labJiLiNull.gameObject.SetActive(false);
			for (int i = 0; i < this.BoCaiResult.OpenHistory.Count; i++)
			{
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					Global.GetLang(string.Format(Global.GetLang("{0}期{1}"), this.BoCaiResult.OpenHistory[i].DataPeriods, Environment.NewLine))
				}));
				stringBuilder2.Append(Environment.NewLine);
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang(string.Format(Global.GetLang("中奖号码:{0}{1}"), this.BoCaiResult.OpenHistory[i].OpenValue.Replace(",", " "), Environment.NewLine))
				}));
				stringBuilder2.Append(Environment.NewLine);
				if (this.BoCaiResult.ItemList == null || this.BoCaiResult.ItemList.Count <= 0)
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("无购买记录") + Environment.NewLine
					}));
					stringBuilder2.Append(Environment.NewLine);
				}
				else
				{
					int num = 0;
					for (int j = 0; j < this.BoCaiResult.ItemList.Count; j++)
					{
						if (this.BoCaiResult.ItemList[j].DataPeriods == this.BoCaiResult.OpenHistory[i].DataPeriods)
						{
							num++;
							stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								Global.GetLang("已购号码:")
							}));
							string[] array = this.BoCaiResult.ItemList[j].strBuyValue.Split(new char[]
							{
								','
							});
							string[] array2 = this.BoCaiResult.OpenHistory[i].OpenValue.Split(new char[]
							{
								','
							});
							for (int k = 0; k < array.Length; k++)
							{
								if (array[k].SafeToInt32(0) == array2[k].SafeToInt32(0))
								{
									stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
									{
										"17e43e",
										string.Format("{0}  ", array[k])
									}));
								}
								else
								{
									stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
									{
										"ff0000",
										string.Format("{0}  ", array[k])
									}));
								}
							}
							stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								Environment.NewLine
							}));
							stringBuilder2.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								Global.GetLang(string.Format(Global.GetLang("{0}注{1}"), this.BoCaiResult.ItemList[j].BuyNum, Environment.NewLine))
							}));
						}
						if (j >= this.BoCaiResult.ItemList.Count - 1 && num <= 0)
						{
							stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
							{
								"ff0000",
								Global.GetLang("无购买记录") + Environment.NewLine
							}));
							stringBuilder2.Append(Environment.NewLine);
						}
					}
				}
			}
		}
		this.labJiLiContentNumber.text = stringBuilder.ToString();
		this.labJiLiContentCount.text = stringBuilder2.ToString();
		this.panelJiLu.gameObject.SetActive(true);
	}

	private void OpenHelpWindow(string path = "Config/CaiShuZiIntro.xml")
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

	public void RefreshBuyCaiPiao(BuyBoCaiResult boCaiResult)
	{
		if (boCaiResult.Info != 0)
		{
			Super.HintMainText(IConfigbase<ConfigCaiShuZi>.Instance.ErrorString((BocaiSysMsgErr)boCaiResult.Info), 10, 3);
			return;
		}
		Super.HintMainText(Global.GetLang("购买成功"), 10, 3);
		this.endNumbers = new string[]
		{
			"-1",
			"-1",
			"-1",
			"-1",
			"-1"
		};
		this.InitEndNumber(this.endNumbers);
		this.RefreshYiGou(boCaiResult.ItemList);
		this.RefreshDaiBi();
	}

	public void RefreshBuyDaiBi(int result)
	{
		this.RefreshDaiBi();
	}

	public void RefreshCaiChi(BoCaiUpdate result)
	{
		if (result == null)
		{
			return;
		}
		if (result.BocaiType != 2)
		{
			return;
		}
		if (this.BoCaiResult.NowPeriods == result.DataPeriods && this.BoCaiResult.Stage != result.Stage && result.Stage == 5)
		{
			GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_CaiNum);
			return;
		}
		if (this.BoCaiResult.NowPeriods != result.DataPeriods && this.BoCaiResult.Stage != result.Stage && result.Stage == 2)
		{
			GameInstance.Game.SendBoCaiNumberData(BoCaiTypeEnum.Bocai_CaiNum);
			return;
		}
		this.BoCaiResult.Stage = result.Stage;
		this.BoCaiResult.OpenTime = result.OpenTime;
		this.BoCaiResult.Value1 = result.Value1;
		this.labKaiJiangHuoBi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			string.Format(Global.GetLang("奖池欢乐代币值:{0}"), result.Value1)
		});
	}

	public void RefreshInit(GetBoCaiResult boCaiResult)
	{
		if (boCaiResult.Info != 0)
		{
			Super.HintMainText(Global.GetLang("界面初始化错误，抱歉"), 10, 3);
			return;
		}
		if (boCaiResult.BocaiType != 2)
		{
			return;
		}
		this.xmlVo = IConfigbase<ConfigCaiShuZi>.Instance.GetCaiShuZiDataByTime(Global.GetCorrectDateTime());
		if (this.xmlVo == null)
		{
			MUDebug.Log<string>(new string[]
			{
				Global.GetLang("配置表错误!")
			});
			return;
		}
		this.labOnNumberZhuShi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			string.Format(Global.GetLang("注：每注{0}欢乐代币"), this.xmlVo.XiaoHaoDaiBi)
		}));
		base.StopAllCoroutines();
		this.BoCaiResult = boCaiResult;
		if (this.xmlVo.ShangChengKaiGuan == 1)
		{
			this.btnShangDian.gameObject.SetActive(true);
		}
		else
		{
			this.btnShangDian.gameObject.SetActive(false);
			this.btnJiLuOpen.transform.localPosition = this.btnShangDian.transform.localPosition;
		}
		List<KFBoCaoHistoryData> winLotteryRoleList = this.BoCaiResult.WinLotteryRoleList;
		if (winLotteryRoleList.Count > 0)
		{
			this.RefreshShangQi(winLotteryRoleList);
		}
		else
		{
			this.labShangQiContent.text = string.Empty;
		}
		this.labKaiJiangTimer.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("开奖时间：{0}"), DateTime.Parse(this.xmlVo.KaiJiangShiJian).ToString("HH:mm:ss"))
		});
		this.labKaiJiangHuoBi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff0000",
			string.Format(Global.GetLang("奖池欢乐代币值:{0}"), this.BoCaiResult.Value1)
		});
		this.labKaiJiangNmberQi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("{0}期"), this.BoCaiResult.NowPeriods)
		});
		this.RefreshYiGou(this.BoCaiResult.ItemList);
		base.StartCoroutine<bool>(this.StartKaiJiang((double)(boCaiResult.OpenTime / 1000L)));
		if (boCaiResult.Stage != 2)
		{
			this.btnGouMai.isEnabled = false;
			this.btnRandom.isEnabled = false;
		}
		else
		{
			this.btnGouMai.isEnabled = true;
			this.btnRandom.isEnabled = true;
		}
		if (this.ZhongJiangNumber != null && !string.IsNullOrEmpty(this.ZhongJiangNumber.OpenValue) && boCaiResult.Stage == 5)
		{
			this.RefreshKaiJiang(this.ZhongJiangNumber.OpenValue.Split(new char[]
			{
				','
			}));
		}
		else
		{
			this.RefreshKaiJiang(new string[]
			{
				"-1",
				"-1",
				"-1",
				"-1",
				"-1"
			});
		}
		this.RefreshDaiBi();
		if (this.listEndNumber.SelectedIndex < 0)
		{
			this.listEndNumber.SelectedIndex = 4;
		}
	}

	private IEnumerator StartKaiJiang(double time)
	{
		double nextTime = 0.2;
		while (time >= 0.0)
		{
			time -= nextTime;
			if (time > 0.0 && time <= 1800.0)
			{
				string[] endNumbers = new string[5];
				for (int i = 0; i < endNumbers.Length; i++)
				{
					endNumbers[i] = Random.Range(0, 10).ToString();
				}
				this.RefreshKaiJiang(endNumbers);
			}
			yield return new WaitForSeconds((float)nextTime);
		}
		yield break;
	}

	public UILabel labXuanNmberTitle;

	public UILabel labOnNumberTitle;

	public UILabel labOnNumberZhuShi;

	public UILabel labKaiJiangTitle;

	public UILabel labKaiJiangNmberQi;

	public UILabel labKaiJiangTimer;

	public UILabel labKaiJiangHuoBi;

	public UILabel labShangQiContent;

	public UILabel labYiGouTitle;

	public UILabel labHuoBiNumber;

	public GButton btnAddHuoBi;

	public GButton btnShangDian;

	public GButton btnBangZhu;

	public GButton btnRandom;

	public GButton btnGouMai;

	public GButton btnClose;

	public ListBox listXuanHao;

	public ListBox listEndNumber;

	public ListBox listKaiJiang;

	public ListBox listYiGou;

	public UIPanel panelJiaZhu;

	public GButton btnJiaZhuClose;

	public ListBox listBoxJiaZhu;

	public GButton btnJiaZhuJian;

	public GButton btnJiaZhuJia;

	public GButton btnJiaZhuNumber;

	public UILabel labJiaZhuHuoBi;

	public UILabel labJiaZhuTitle;

	public GButton btnJiaZhuBuy;

	public GButton btnJiaBack;

	public GButton btnJiLuOpen;

	public GButton btnJiLuClose;

	public UILabel labJiLiTitle;

	public UILabel labJiLiContentNumber;

	public UILabel labJiLiContentCount;

	public UILabel labJiLiNull;

	public UIPanel panelJiLu;

	public DPSelectedItemEventHandler handlerClose;

	private ObservableCollection obserXuanHao;

	private ObservableCollection obserEndNumber;

	private ObservableCollection obserKaiJiang;

	private ObservableCollection obserYiGou;

	private ObservableCollection obserJiaZhu;

	private string[] endNumbers = new string[]
	{
		"-1",
		"-1",
		"-1",
		"-1",
		"-1"
	};

	private string[] jiaZhuNumbers = new string[]
	{
		"-1",
		"-1",
		"-1",
		"-1",
		"-1"
	};

	private int maxNumbers = 5;

	private int jiaZhuNumber = 1;

	private int keyNumber = 4;

	private CaiShuZiVO xmlVo;

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;
}
