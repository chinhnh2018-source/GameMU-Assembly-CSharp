using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TuiGuangPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.TuiGuangAmount.pivot = 5;
		this.LevelAwardLeft.pivot = 5;
		this.tab1Btn.Text = Global.GetLang("推广员");
		this.tab2Btn.Text = Global.GetLang("验证推荐人");
		this.TuiGuangAmount.text = this.CreateNameOrangeValueWhiteString(Global.GetLang("已推广用户:"), 0);
		this.TuiGuangID.text = this.CreateNameOrangeValueWhiteString(Global.GetLang("推广员ID:"), 0);
		this.ShareWeChatBtn.Text = Global.GetLang("分享朋友圈");
		this.VIPAwardTip.text = string.Format(Global.GetLang("被推广用户成为{0}可领取一次（最多可领取{1}次）"), Global.GetColorStringForNGUIText(new object[]
		{
			"ffd460",
			"VIP" + TuiGuangPart.AwardXmlData.VipLevel
		}), ConfigSystemParam.GetSystemParamIntByName("TuiGuangVIPRewardNum"));
		this.VIPAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), 0);
		this.VIPAwardBtn.Text = Global.GetLang("领取奖励");
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"ffd460",
			string.Format(Global.GetLang("{0}转{1}级"), TuiGuangPart.AwardXmlData.MinZhuanSheng, TuiGuangPart.AwardXmlData.MinLevel)
		});
		this.LevelAwardTip.text = string.Format(Global.GetLang("被推广用户等级达到{0}可领取一次（最多可领取{1}次）"), colorStringForNGUIText, ConfigSystemParam.GetSystemParamIntByName("TuiGuangLevelRewardNum"));
		this.LevelAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), 0);
		this.LevelAwardBtn.Text = Global.GetLang("领取奖励");
		this.BoxAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("可领取宝箱:"), 0);
		this.BoxAwardBtn.Text = Global.GetLang("领取奖励");
		this.CheckIDBtn.Text = Global.GetLang("确定");
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("TuiGuangCreatData", ' ');
		this.BeiTuiJianDateTip.text = string.Format(Global.GetLang("创建帐号日期在{0}之后的玩家可以填写推荐人"), Global.GetColorStringForNGUIText(new object[]
		{
			"ffd460",
			systemParamStringArrayByName[0]
		}));
		this.BeiTuiJianAwardTip.text = Global.GetLang("被推荐人领取奖励");
		this.TuiJianRenInfo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format(Global.GetLang("我的推荐人ID:{0}"), this.Data.TuiJianRenID)
		});
		this.PhoneNumTitleLabel.text = Global.GetLang("手机验证");
		this.PhoneNumLabel.text = Global.GetLang("输入手机号");
		this.PhoneKeywordLabel.text = Global.GetLang("输入验证码");
		this.PhoneNumBtn.Text = Global.GetLang("获取验证码");
		this.PhoneKeywordBtn.Text = Global.GetLang("确定");
		this.PhoneNumInput.text = Global.GetLang("输入11位手机号码");
		this.PhoneKeywordInput.text = Global.GetLang("点击输入验证码");
		for (int i = 0; i < this.AwardBoxLabel.Length; i++)
		{
			if (this.LeiJiAwardsXmlDataDict.ContainsKey(i))
			{
				this.AwardBoxLabel[i].text = string.Format(Global.GetLang("{0}人"), this.LeiJiAwardsXmlDataDict[i].MinNum);
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.InitVIPAwardsXmlDataDict();
		this.InitLevelAwardsXmlDataDict();
		this.InitLeiJiAwardsXmlDataDict();
		this.InitNewAwardsXmlDataDict();
		this.InitTextInPrefabs();
		this.SendCMD_TuiGuangInfo();
		this.tab1Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.tab2Btn.Label.color = NGUIMath.HexToColorEx(7697781U);
		this.BoxAwardBar.Percent = 0.0;
		this.itemsListArr = new List<GGoodIcon>[4];
		this.itemsListArrLineLimit = new int[]
		{
			5,
			5,
			1,
			10
		};
		this.itemsListArrAllLimit = new int[]
		{
			5,
			5,
			6,
			10
		};
		this.ItemCollections = new ObservableCollection[]
		{
			this.AwardItemsListBoxArray[0].ItemsSource,
			this.AwardItemsListBoxArray[1].ItemsSource,
			this.AwardItemsListBoxArray[2].ItemsSource,
			this.AwardItemsListBoxArray[3].ItemsSource
		};
		this.InitControlProc();
		this.SetPart(this.m_nCurrentTab);
		this.InitTipWindow();
		this.InitPhoneNumCheckWindow();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	private void InitControlProc()
	{
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.tab1Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(1);
		};
		this.tab2Btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SetPart(2);
		};
		this.ShareWeChatBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.WeChatShareForTuiguang();
		};
		this.CheckIDBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CheckTuiJianRenID();
		};
		this.VIPAwardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAward(TuiGuangPart.TuiGuangAwardType.VIP, -1);
		};
		this.LevelAwardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAward(TuiGuangPart.TuiGuangAwardType.Level, -1);
		};
		for (int i = 0; i < 6; i++)
		{
			if (i == 0)
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, 0);
				};
			}
			else if (i == 1)
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, 1);
				};
			}
			else if (i == 2)
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, 2);
				};
			}
			else if (i == 3)
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, 3);
				};
			}
			else if (i == 4)
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, 4);
				};
			}
			else if (i == 5)
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, 5);
				};
			}
			else
			{
				this.AwardBoxBtn[i].MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, -1);
				};
			}
		}
		this.BoxAwardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAward(TuiGuangPart.TuiGuangAwardType.Amount, -1);
		};
		this.BeiTuiJianAwardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAward(TuiGuangPart.TuiGuangAwardType.BeTuiJian, -1);
		};
		this.TuiJianRenIDInput.TextChanged = new EventHandler(this.InputTextChangedEventHandler);
		this.PhoneNumInput.TextChanged = new EventHandler(this.InputTextChangedEventHandler);
		this.PhoneKeywordInput.TextChanged = new EventHandler(this.InputTextChangedEventHandler);
		this.TuiJianRenIDInput.GotFocus = new EventHandler(this.InputGotFocusEventHander);
		this.PhoneNumInput.GotFocus = new EventHandler(this.InputGotFocusEventHander);
		this.PhoneKeywordInput.GotFocus = new EventHandler(this.InputGotFocusEventHander);
	}

	private void InputGotFocusEventHander(object sender, EventArgs e)
	{
		if (sender as TextBox == this.TuiJianRenIDInput)
		{
			if ((this.m_nInputFocusTimeC8B8A8 & 255) == 0)
			{
				this.TuiJianRenIDInput.Text = string.Empty;
				this.m_nInputFocusTimeC8B8A8 |= 15;
			}
		}
		else if (sender as TextBox == this.PhoneNumInput)
		{
			if ((this.m_nInputFocusTimeC8B8A8 & 65280) == 0)
			{
				this.PhoneNumInput.Text = string.Empty;
				this.m_nInputFocusTimeC8B8A8 |= 3840;
			}
		}
		else if (sender as TextBox == this.PhoneKeywordInput && (this.m_nInputFocusTimeC8B8A8 & 16711680) == 0)
		{
			this.PhoneKeywordInput.Text = string.Empty;
			this.m_nInputFocusTimeC8B8A8 |= 983040;
		}
	}

	private void InputTextChangedEventHandler(object sender, EventArgs e)
	{
		if (sender as TextBox == this.TuiJianRenIDInput)
		{
			string text = this.TuiJianRenIDInput.Text.Trim();
			if (text == string.Empty)
			{
				this.TuiJianRenIDInput.Text = string.Empty;
				return;
			}
			string text2 = string.Empty;
			string text3 = text;
			for (int i = 0; i < text3.Length; i++)
			{
				char c = text3.get_Chars(i);
				if (char.IsLetterOrDigit(c) || c.Equals('#'))
				{
					text2 += c;
				}
			}
			this.TuiJianRenIDInput.Text = text2;
		}
		else if (sender as TextBox == this.PhoneNumInput)
		{
			string text4 = this.PhoneNumInput.Text.Trim();
			if (text4 == string.Empty)
			{
				this.PhoneNumInput.Text = string.Empty;
				return;
			}
			string text5 = string.Empty;
			string text6 = text4;
			for (int j = 0; j < text6.Length; j++)
			{
				char c2 = text6.get_Chars(j);
				if (char.IsDigit(c2))
				{
					text5 += c2;
				}
			}
			this.PhoneNumInput.Text = text5;
		}
		else if (sender as TextBox == this.PhoneKeywordInput)
		{
			string text7 = this.PhoneKeywordInput.Text.Trim();
			if (text7 == string.Empty)
			{
				this.PhoneKeywordInput.Text = string.Empty;
				return;
			}
			string text8 = string.Empty;
			string text9 = text7;
			for (int k = 0; k < text9.Length; k++)
			{
				char c3 = text9.get_Chars(k);
				if (char.IsLetterOrDigit(c3))
				{
					text8 += c3;
				}
			}
			this.PhoneKeywordInput.Text = text8;
		}
	}

	private string CreateNameOrangeValueWhiteString(string strName, int nValue)
	{
		return Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(strName),
			"dac7ae",
			nValue
		});
	}

	private string CreateNameOrangeValueWhiteString(string strName, string nValue)
	{
		return Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(strName),
			"dac7ae",
			nValue
		});
	}

	private string CreateNameOrangeValueGreenString(string strName, int nValue)
	{
		return Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(strName),
			"00ff00",
			nValue
		});
	}

	private void SetPart(int type)
	{
		if (type != 1)
		{
			if (type == 2)
			{
				this.m_nCurrentTab = type;
				this.tabView1.gameObject.SetActive(false);
				this.tabView2.gameObject.SetActive(true);
				this.SetBtnStat(this.tab2Btn);
				this.UpdateTabContext(type);
			}
		}
		else
		{
			this.m_nCurrentTab = type;
			this.tabView1.gameObject.SetActive(true);
			this.tabView2.gameObject.SetActive(false);
			this.SetBtnStat(this.tab1Btn);
			this.UpdateTabContext(type);
		}
	}

	private void SetBtnStat(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(4294967294U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(4294967294U);
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(7697781U);
			this.tempBtn.Pressed = false;
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(4294967294U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	private void UpdateTabContext(int type)
	{
		for (int i = 0; i < this.itemsListArr.Length; i++)
		{
			if (this.itemsListArr[i] != null)
			{
				this.itemsListArr[i].Clear();
			}
		}
		if (type == 1)
		{
			Dictionary<int, TuiGuangPart.AwardXmlData>[] array = new Dictionary<int, TuiGuangPart.AwardXmlData>[]
			{
				this.VIPAwardsXmlDataDict,
				this.LevelAwardsXmlDataDict,
				this.LeiJiAwardsXmlDataDict
			};
			for (int j = 0; j < array.Length; j++)
			{
				int num = j;
				if (this.itemsListArr[num] == null)
				{
					this.itemsListArr[num] = new List<GGoodIcon>();
					this.ItemCollections[num].Clear();
				}
				else
				{
					this.itemsListArr[num].Clear();
					this.ItemCollections[num].Clear();
				}
				foreach (TuiGuangPart.AwardXmlData awardXmlData in array[j].Values)
				{
					if (this.itemsListArr[num].Count < this.itemsListArrAllLimit[num])
					{
						int num2 = this.itemsListArrLineLimit[num];
						int count = this.itemsListArr[num].Count;
						this.AddGoodsIconList(num, num2, awardXmlData.GoodsOne, false);
						this.AddGoodsIconList(num, num2 - (this.itemsListArr[num].Count - count), awardXmlData.GoodsTwo, true);
					}
				}
			}
			this.ShowGoodsIconList();
			this.UpdateAwardBoxSurface();
			this.UpdateAwardBtnSurStat();
			if (this.Data.IsRegist)
			{
				this.ShareWeChatBtn.Text = Global.GetLang("分享朋友圈");
				this.TuiGuangID.gameObject.SetActive(true);
				this.TuiGuangIDBack.gameObject.SetActive(true);
			}
			else
			{
				this.ShareWeChatBtn.Text = Global.GetLang("成为推广员");
				this.TuiGuangID.gameObject.SetActive(false);
				this.TuiGuangIDBack.gameObject.SetActive(false);
			}
		}
		else if (type == 2)
		{
			int num3 = 3;
			Dictionary<int, TuiGuangPart.AwardXmlData>[] array2 = new Dictionary<int, TuiGuangPart.AwardXmlData>[]
			{
				this.NewBoyAwardsXmlDataDict
			};
			for (int k = 0; k < array2.Length; k++)
			{
				if (this.itemsListArr[num3] == null)
				{
					this.itemsListArr[num3] = new List<GGoodIcon>();
					this.ItemCollections[num3].Clear();
				}
				else
				{
					this.itemsListArr[num3].Clear();
					this.ItemCollections[num3].Clear();
				}
				foreach (TuiGuangPart.AwardXmlData awardXmlData2 in array2[k].Values)
				{
					if (this.itemsListArr[num3].Count < this.itemsListArrAllLimit[num3])
					{
						int num4 = this.itemsListArrLineLimit[num3];
						int count2 = this.itemsListArr[num3].Count;
						this.AddGoodsIconList(num3, num4, awardXmlData2.GoodsOne, false);
						this.AddGoodsIconList(num3, num4 - (this.itemsListArr[num3].Count - count2), awardXmlData2.GoodsTwo, true);
					}
				}
			}
			this.ShowGoodsIconList();
			if (this.Data.IsPhoneNumChecked)
			{
				this.TuiJianRenIDInput.gameObject.SetActive(false);
				this.CheckIDBtn.gameObject.SetActive(false);
				this.TuiJianRenInfo.gameObject.SetActive(true);
				this.TuiJianRenInfo.text = string.Format(Global.GetLang("我的推荐人ID:{0}"), this.Data.TuiJianRenID);
				if (this.Data.IsNewAwardReceived)
				{
					this.BeiTuiJianAwardBtn.isEnabled = false;
				}
				else
				{
					this.BeiTuiJianAwardBtn.isEnabled = true;
				}
			}
			else
			{
				this.TuiJianRenIDInput.gameObject.SetActive(true);
				this.CheckIDBtn.gameObject.SetActive(true);
				this.TuiJianRenInfo.gameObject.SetActive(false);
				this.TuiJianRenInfo.text = string.Format(Global.GetLang("我的推荐人ID:{0}"), this.Data.TuiJianRenID);
				this.BeiTuiJianAwardBtn.isEnabled = false;
			}
		}
		this.TuiGuangAmount.text = this.CreateNameOrangeValueWhiteString(Global.GetLang("已推广用户:"), this.Data.Amount);
		this.TuiGuangID.text = this.CreateNameOrangeValueWhiteString(Global.GetLang("推广员ID:"), this.Data.TuiGuangYuanID);
	}

	private void ShowGoodsIconList()
	{
		if (this.m_nCurrentTab == 1)
		{
			for (int i = 0; i < this.itemsListArr.Length - 1; i++)
			{
				List<GGoodIcon> list = this.itemsListArr[i];
				if (list != null)
				{
					ObservableCollection observableCollection = this.ItemCollections[i];
					observableCollection.Clear();
					for (int j = 0; j < list.Count; j++)
					{
						observableCollection.AddNoUpdate(list[j]);
					}
					observableCollection.DelayUpdate();
				}
			}
		}
		else if (this.m_nCurrentTab == 2)
		{
			for (int k = this.itemsListArr.Length - 1; k < this.itemsListArr.Length; k++)
			{
				List<GGoodIcon> list2 = this.itemsListArr[k];
				if (list2 != null)
				{
					ObservableCollection observableCollection2 = this.ItemCollections[k];
					observableCollection2.Clear();
					for (int l = 0; l < list2.Count; l++)
					{
						observableCollection2.AddNoUpdate(list2[l]);
					}
					observableCollection2.DelayUpdate();
				}
			}
		}
	}

	private void AddGoodsIconList(int nItemListIndex, int nAddLimit, string goodsStr, bool isOcc = false)
	{
		string[] array = goodsStr.Split(new char[]
		{
			'|'
		});
		if (isOcc)
		{
			int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					','
				});
				if (array3.Length == 7)
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[3]), Convert.ToInt32(array3[4]), Convert.ToInt32(array3[6]), Convert.ToInt32(array3[5]), Convert.ToInt32(array3[2]), Convert.ToInt32(array3[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					if (TuiGuangPart.IsGoodsToOccupation(dummyGoodsDataMu.GoodsID) && nAddLimit-- > 0)
					{
						this.AddGoodsIcon(this.itemsListArr[nItemListIndex], dummyGoodsDataMu, true, false);
					}
				}
			}
		}
		else
		{
			foreach (string text2 in array)
			{
				string[] array3 = text2.Split(new char[]
				{
					','
				});
				if (array3.Length == 7)
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array3[0]), Convert.ToInt32(array3[3]), Convert.ToInt32(array3[4]), Convert.ToInt32(array3[6]), Convert.ToInt32(array3[5]), Convert.ToInt32(array3[2]), Convert.ToInt32(array3[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					if (nAddLimit-- > 0)
					{
						this.AddGoodsIcon(this.itemsListArr[nItemListIndex], dummyGoodsDataMu, false, false);
					}
				}
			}
		}
	}

	private static bool IsGoodsToOccupation(int nGoodsID)
	{
		if (0 >= nGoodsID)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nGoodsID);
		int mainOccupation = goodsXmlNodeByID.MainOccupation;
		return mainOccupation == -1 || (mainOccupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) && (mainOccupation != 3 || (Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && goodsXmlNodeByID.Strength > goodsXmlNodeByID.Intelligence) || (Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && goodsXmlNodeByID.Intelligence > goodsXmlNodeByID.Strength)));
	}

	private void AddGoodsIcon(List<GGoodIcon> itemsList, GoodsData gd, bool isOcc = false, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BackSpriteName0 = backSpriteName;
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = gd.GoodsID;
			icon.ItemObject = gd;
			icon.BoxTypes = -1;
			icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, gd);
			};
			if (!grayShow)
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
			itemsList.Add(icon);
		}
	}

	private void OpenAward(TuiGuangPart.TuiGuangAwardType type, int nBoxIndex = -1)
	{
		switch (type)
		{
		case TuiGuangPart.TuiGuangAwardType.VIP:
			this.SendCMD_GetAward(type);
			break;
		case TuiGuangPart.TuiGuangAwardType.Level:
			this.SendCMD_GetAward(type);
			break;
		case TuiGuangPart.TuiGuangAwardType.Amount:
			if (nBoxIndex == -1)
			{
				this.SendCMD_GetAward(type);
			}
			else if (nBoxIndex < this.itemsListArr[2].Count)
			{
				GTipServiceEx.ShowTip(this.itemsListArr[2][nBoxIndex], TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, this.itemsListArr[2][nBoxIndex].ItemObject as GoodsData);
			}
			break;
		case TuiGuangPart.TuiGuangAwardType.BeTuiJian:
			this.SendCMD_GetAward(type);
			break;
		}
	}

	private void WeChatShareForTuiguang()
	{
		Debug.Log("Enter WeChatShareForTuiguang......");
		if (this.Data.IsRegist)
		{
			PlatSDKMgr.WXShareUrl("http://www.tmsk.cn/xinbingzhaomu/", string.Concat(new object[]
			{
				Global.GetLang("我在"),
				Global.Data.GameServerID,
				Global.GetLang("区，ID："),
				this.Data.TuiGuangYuanID,
				Global.GetLang(",战火不熄，新兵礼包助你创造奇迹!")
			}));
		}
		else
		{
			this.SendCMD_RegistAsTuiGuangYuan();
		}
	}

	private void ShareToWeChat()
	{
		if (this.Data.IsRegist)
		{
			PlatSDKMgr.WXShareUrl("http://www.tmsk.cn/xinbingzhaomu/", string.Concat(new object[]
			{
				Global.GetLang("我在"),
				Global.Data.GameServerID,
				Global.GetLang("区，ID："),
				this.Data.TuiGuangYuanID,
				Global.GetLang(",战火不熄，新兵礼包助你创造奇迹!")
			}));
		}
		else
		{
			this.SendCMD_RegistAsTuiGuangYuan();
		}
	}

	private void UpdateAwardBtnSurStat()
	{
		if (this.Data.AwardVipAmount > 0)
		{
			this.VIPAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), this.Data.AwardVipAmount);
			this.VIPAwardBtn.isEnabled = true;
		}
		else
		{
			this.VIPAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), 0);
			this.VIPAwardBtn.isEnabled = false;
		}
		if (this.Data.AwardLevelAmount > 0)
		{
			this.LevelAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), this.Data.AwardLevelAmount);
			this.LevelAwardBtn.isEnabled = true;
		}
		else
		{
			this.LevelAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), 0);
			this.LevelAwardBtn.isEnabled = false;
		}
	}

	private void UpdateAwardBoxSurface()
	{
		int amount = this.Data.Amount;
		int boxOpenedAmount = this.Data.BoxOpenedAmount;
		List<TuiGuangPart.AwardXmlData> list = new List<TuiGuangPart.AwardXmlData>();
		for (int i = 0; i < this.LeiJiAwardsXmlDataDict.Count; i++)
		{
			int num = i + 1;
			if (this.LeiJiAwardsXmlDataDict.ContainsKey(num))
			{
				list.Add(this.LeiJiAwardsXmlDataDict[num]);
			}
		}
		int num2 = 0;
		for (int j = list.Count - 1; j >= 0; j--)
		{
			if (amount >= list[j].MinNum)
			{
				num2 = j + 1;
				break;
			}
		}
		int num3 = num2 - boxOpenedAmount;
		if (num3 > 0)
		{
			this.BoxAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), num3);
			this.BoxAwardBtn.isEnabled = true;
		}
		else
		{
			this.BoxAwardLeft.text = this.CreateNameOrangeValueGreenString(Global.GetLang("剩余次数:"), 0);
			this.BoxAwardBtn.isEnabled = false;
		}
		int num4 = boxOpenedAmount - 1;
		int num5 = list.Count - 6;
		if (num4 < 0)
		{
			num4 = 0;
		}
		if (num4 > num5)
		{
			num4 = num5;
		}
		int num6 = num4;
		while (num6 < num4 + 6 && num6 < list.Count)
		{
			int num7 = num6 - num4;
			if (num7 < this.AwardBoxBtn.Length)
			{
				this.AwardBoxLabel[num7].text = string.Format(Global.GetLang("{0}人"), list[num6].MinNum);
				if (num6 <= boxOpenedAmount - 1)
				{
					this.AwardBoxBtn[num7].normalSprite = "opopen";
					this.AwardBoxBtn[num7].pressedSprite = "opopen";
					this.AwardBoxBtn[num7].hoverSprite = "opopen";
					this.AwardBoxBtn[num7].disabledSprite = "opopen";
					this.AwardBoxBtn[num7].Refresh();
				}
				else if (num6 > boxOpenedAmount - 1 && num6 < num2)
				{
					this.AwardBoxBtn[num7].normalSprite = "opopenwait";
					this.AwardBoxBtn[num7].pressedSprite = "opopenwait";
					this.AwardBoxBtn[num7].hoverSprite = "opopenwait";
					this.AwardBoxBtn[num7].disabledSprite = "opopenwait";
					this.AwardBoxBtn[num7].Refresh();
				}
				else
				{
					this.AwardBoxBtn[num7].normalSprite = "opopenclosed";
					this.AwardBoxBtn[num7].pressedSprite = "opopenclosed";
					this.AwardBoxBtn[num7].hoverSprite = "opopenclosed";
					this.AwardBoxBtn[num7].disabledSprite = "opopenclosed";
					this.AwardBoxBtn[num7].Refresh();
				}
			}
			num6++;
		}
		float num8 = 0f;
		float num9 = 1f;
		int num10 = num4 + 1;
		while (num10 < num4 + 6 && num10 < list.Count)
		{
			int num11 = num10 - num4;
			if (num11 < list.Count && num11 > 0)
			{
				if (amount > list[num11 - 1].MinNum)
				{
					num8 = (float)(num11 - 1) / 5f;
				}
				if (amount >= list[num11 - 1].MinNum && amount <= list[num11].MinNum)
				{
					float num12 = (float)(amount - list[num11 - 1].MinNum);
					float num13 = (float)(list[num11].MinNum - list[num11 - 1].MinNum);
					num9 = num12 / num13;
					break;
				}
			}
			num10++;
		}
		num8 += num9 / 5f;
		if (amount <= 2)
		{
			num8 = (float)amount / 5f / 10f;
		}
		this.BoxAwardBar.Percent = (double)num8;
	}

	private void CheckTuiJianRenID()
	{
		string text = this.TuiJianRenIDInput.Text;
		if (text == string.Empty)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("推荐人帐号不能为空！"), new object[0]), 10, 3);
			return;
		}
		this.SendCMD_CheckTuiJianRenID(text);
	}

	private void InitTipWindow()
	{
		this.HideTipWindow();
	}

	private void ShowTipWindow()
	{
		this.TipOkBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnTipCloseClick();
		};
		this.TipOkBtn.Text = Global.GetLang("确定");
		this.TipContextLabel.text = string.Format(Global.GetLang("恭喜你成为推广员!推广员ID:{0}"), this.Data.TuiGuangYuanID);
		this.TipPanel.gameObject.SetActive(true);
	}

	private void HideTipWindow()
	{
		this.TipPanel.gameObject.SetActive(false);
	}

	private void OnTipCloseClick()
	{
		this.TipPanel.gameObject.SetActive(false);
	}

	private void ShowUnopenTipWindow()
	{
		this.TipOkBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnUnopenTipCloseClick();
		};
		this.TipOkBtn.Text = Global.GetLang("确定");
		this.TipContextLabel.text = Global.GetLang("推广员功能不可用，点击确定关闭窗口。");
		this.TipPanel.gameObject.SetActive(true);
	}

	private void OnUnopenTipCloseClick()
	{
		this.TipPanel.gameObject.SetActive(false);
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = 0
		});
	}

	private void InitPhoneNumCheckWindow()
	{
		this.PhoneNumCeckPanelCloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnPhoneNumCheckPanelCloseClick();
		};
		this.PhoneNumBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnPhoneNumBtnClick();
		};
		this.PhoneKeywordBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnKeywordBtnClick();
		};
		this.PhoneNumCheckPanel.gameObject.SetActive(false);
	}

	private void ShowPhoneNumCheckWindow()
	{
		this.PhoneNumCheckPanel.gameObject.SetActive(true);
	}

	private void HidePhoneNumCheckWindow()
	{
		this.PhoneNumCheckPanel.gameObject.SetActive(false);
	}

	private void OnPhoneNumCheckPanelCloseClick()
	{
		this.PhoneNumCheckPanel.gameObject.SetActive(false);
	}

	protected void StartUITimer()
	{
		if (this.UITimer != null)
		{
			return;
		}
		this.UITimer = new DispatcherTimer("TuiGuangPart_Timer" + Random.Range(0, int.MaxValue));
		this.UITimer.Interval = TimeSpan.FromMilliseconds(0.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.UITimer.Start();
		this.PhoneNumBtn.isEnabled = false;
		this.PhoneKeywordBtn.isEnabled = true;
		this.m_nTickCount = 0;
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
		this.PhoneNumBtn.isEnabled = true;
		this.PhoneKeywordBtn.isEnabled = true;
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (this.UITimer.Interval.Ticks <= 0L)
		{
			this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		}
		this.m_nTickCount++;
		if (this.m_nTickCount >= 60)
		{
			this.StopTimer();
			this.PhoneNumBtn.Text = Global.GetLang("获取验证码");
		}
		else
		{
			this.PhoneNumBtn.Text = string.Format(Global.GetLang("{0}秒后再获取"), 60 - this.m_nTickCount);
		}
	}

	private void OnPhoneNumBtnClick()
	{
		if ((this.m_nInputFocusTimeC8B8A8 & 65280) == 0)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("手机号不能为空！"), new object[0]), 10, 3);
			return;
		}
		string text = this.PhoneNumInput.Text;
		if (text == string.Empty)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("手机号不能为空！"), new object[0]), 10, 3);
			return;
		}
		this.SendCMD_PhoneNum(text);
	}

	private void OnKeywordBtnClick()
	{
		if ((this.m_nInputFocusTimeC8B8A8 & 16711680) == 0)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("验证码不能为空！"), new object[0]), 10, 3);
			return;
		}
		string text = this.PhoneKeywordInput.Text;
		if (text == string.Empty)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("验证码不能为空！"), new object[0]), 10, 3);
			return;
		}
		this.SendCMD_PhoneNumKeyword(text);
	}

	private static bool IsNetResultSuccess(MUSocketConnectEventArgs e)
	{
		int eSpreadState = Global.SafeConvertToInt32(e.fields[0]);
		return TuiGuangPart.IsNetResultSuccess(eSpreadState);
	}

	private static bool IsNetResultSuccess(int eSpreadState)
	{
		switch (eSpreadState + 36)
		{
		case 0:
		{
			string lang = Global.GetLang("验证次数过多");
			Super.HintMainText(Global.GetLang(lang), 10, 3);
			return false;
		}
		case 1:
		{
			string lang2 = Global.GetLang("验证码过期");
			Super.HintMainText(Global.GetLang(lang2), 10, 3);
			return false;
		}
		case 2:
		{
			string lang3 = Global.GetLang("验证码错误");
			Super.HintMainText(Global.GetLang(lang3), 10, 3);
			return false;
		}
		case 3:
		{
			string lang4 = Global.GetLang("验证码获取失败");
			Super.HintMainText(Global.GetLang(lang4), 10, 3);
			return false;
		}
		case 4:
		{
			string lang5 = Global.GetLang("手机号已被验证");
			Super.HintMainText(Global.GetLang(lang5), 10, 3);
			return false;
		}
		case 5:
		{
			string lang6 = Global.GetLang("手机号不正确！");
			Super.HintMainText(Global.GetLang(lang6), 10, 3);
			return false;
		}
		case 6:
		{
			string lang7 = Global.GetLang("手机号为空！");
			Super.HintMainText(Global.GetLang(lang7), 10, 3);
			return false;
		}
		case 15:
		{
			string lang8 = Global.GetLang("已注册推荐人");
			Super.HintMainText(Global.GetLang(lang8), 10, 3);
			return false;
		}
		case 16:
		{
			string lang9 = Global.GetLang("不是推广员");
			Super.HintMainText(Global.GetLang(lang9), 10, 3);
			return false;
		}
		case 20:
		{
			string lang10 = Global.GetLang("验证次数过多，禁止验证24小时");
			Super.HintMainText(Global.GetLang(lang10), 10, 3);
			return false;
		}
		case 21:
		{
			string lang11 = Global.GetLang("不能填写自己的推广员ID");
			Super.HintMainText(Global.GetLang(lang11), 10, 3);
			return false;
		}
		case 22:
		{
			string lang12 = Global.GetLang("推广码不正确！");
			Super.HintMainText(Global.GetLang(lang12), 10, 3);
			return false;
		}
		case 23:
		{
			string lang13 = Global.GetLang("超过推荐日期");
			Super.HintMainText(Global.GetLang(lang13), 10, 3);
			return false;
		}
		case 24:
		{
			string lang14 = Global.GetLang("已经验证");
			Super.HintMainText(Global.GetLang(lang14), 10, 3);
			return false;
		}
		case 25:
		{
			string lang15 = Global.GetLang("验证为空");
			Super.HintMainText(Global.GetLang(lang15), 10, 3);
			return false;
		}
		case 26:
		{
			string lang16 = Global.GetLang("未验证推荐人");
			Super.HintMainText(Global.GetLang(lang16), 10, 3);
			return false;
		}
		case 31:
		{
			string lang17 = Global.GetLang("服务器错误");
			Super.HintMainText(Global.GetLang(lang17), 10, 3);
			return false;
		}
		case 32:
		{
			string lang18 = Global.GetLang("没有奖励可以领取");
			Super.HintMainText(Global.GetLang(lang18), 10, 3);
			return false;
		}
		case 33:
		{
			string lang19 = Global.GetLang("背包已满");
			Super.HintMainText(Global.GetLang(lang19), 10, 3);
			return false;
		}
		case 34:
		{
			string lang20 = Global.GetLang("功能未开启");
			Super.HintMainText(Global.GetLang(lang20), 10, 3);
			return false;
		}
		case 35:
			if (eSpreadState < 0)
			{
				string errMsg = StdErrorCode.GetErrMsg(eSpreadState, true, false);
				Super.HintMainText(Global.GetLang(errMsg), 10, 3);
			}
			return false;
		case 36:
			return true;
		case 37:
			return true;
		}
		return false;
	}

	private void SendCMD_TuiGuangInfo()
	{
		GameInstance.Game.GetTuiGuangInfo();
	}

	public void RecvCMD_TuiGuangInfo(MUSocketConnectEventArgs e)
	{
		this.Data.UpdateData = DataHelper.BytesToObject<TuiGuangData.SpreadData>(e.bytesData, 0, e.bytesData.Length);
		if (!this.Data.UpdateData.IsOpen)
		{
			this.ShowUnopenTipWindow();
			return;
		}
		if (!TuiGuangPart.IsNetResultSuccess(this.Data.UpdateData.State))
		{
			return;
		}
		this.UpdateTabContext(this.m_nCurrentTab);
	}

	private void SendCMD_RegistAsTuiGuangYuan()
	{
		GameInstance.Game.SendBeTuiGuangYuan();
	}

	public void RecvCMD_RegistAsTuiGuangYuan(MUSocketConnectEventArgs e)
	{
		if (!TuiGuangPart.IsNetResultSuccess(e))
		{
			return;
		}
		string text = e.fields[1];
		if (!string.IsNullOrEmpty(text))
		{
			this.Data.TuiGuangYuanID = text;
			this.TuiGuangAmount.text = this.CreateNameOrangeValueWhiteString(Global.GetLang("已推广用户:"), this.Data.Amount);
			this.TuiGuangID.text = this.CreateNameOrangeValueWhiteString(Global.GetLang("推广员ID:"), this.Data.TuiGuangYuanID);
			this.UpdateTabContext(this.m_nCurrentTab);
			this.ShowTipWindow();
		}
	}

	private void SendCMD_GetAward(TuiGuangPart.TuiGuangAwardType type)
	{
		GameInstance.Game.SendTuiGuangGetAward((int)type);
	}

	public void RecvCMD_GetAward(MUSocketConnectEventArgs e)
	{
		this.Data.UpdateData = DataHelper.BytesToObject<TuiGuangData.SpreadData>(e.bytesData, 0, e.bytesData.Length);
		if (!TuiGuangPart.IsNetResultSuccess(this.Data.UpdateData.State))
		{
			return;
		}
		this.UpdateTabContext(this.m_nCurrentTab);
	}

	private void SendCMD_CheckTuiJianRenID(string strID)
	{
		TuiGuangPart._sTuiJianRenID = strID;
		GameInstance.Game.SendTuiGuangTuiJianRenID(strID);
	}

	public void RecvCMD_CheckTuiJianRenID(MUSocketConnectEventArgs e)
	{
		if (!TuiGuangPart.IsNetResultSuccess(e))
		{
			return;
		}
		this.ShowPhoneNumCheckWindow();
	}

	private void SendCMD_PhoneNum(string strNum)
	{
		GameInstance.Game.SendTuiGuangGetCode(strNum);
	}

	public void RecvCMD_PhoneNum(MUSocketConnectEventArgs e)
	{
		if (!TuiGuangPart.IsNetResultSuccess(e))
		{
			return;
		}
		this.StartUITimer();
	}

	private void SendCMD_PhoneNumKeyword(string strNum)
	{
		GameInstance.Game.SendTuiGuangCheckCode(strNum);
	}

	public void RecvCMD_PhoneNumKeyword(MUSocketConnectEventArgs e)
	{
		if (!TuiGuangPart.IsNetResultSuccess(e))
		{
			return;
		}
		this.Data.IsPhoneNumChecked = true;
		this.SendCMD_TuiGuangInfo();
		this.HidePhoneNumCheckWindow();
	}

	public void InitLeiJiAwardsXmlDataDict()
	{
		if (this.LeiJiAwardsXmlDataDict != null)
		{
			return;
		}
		this.LeiJiAwardsXmlDataDict = new Dictionary<int, TuiGuangPart.AwardXmlData>();
		XElement isolateResXml = Global.GetIsolateResXml("Config/TuiGuang/TuiGuangYuanLeiJi.xml");
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Config");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			TuiGuangPart.AwardXmlData awardXmlData = new TuiGuangPart.AwardXmlData();
			awardXmlData.ID = Global.GetXElementAttributeInt(xelement2, "ID");
			awardXmlData.MinNum = Global.GetXElementAttributeInt(xelement2, "MinNum");
			awardXmlData.GoodsOne = Global.GetXElementAttributeStr(xelement2, "GoodsOne");
			awardXmlData.GoodsTwo = Global.GetXElementAttributeStr(xelement2, "GoodsTwo");
			int id = awardXmlData.ID;
			if (!this.LeiJiAwardsXmlDataDict.ContainsKey(id))
			{
				this.LeiJiAwardsXmlDataDict.Add(id, awardXmlData);
			}
		}
	}

	public void InitVIPAwardsXmlDataDict()
	{
		if (this.VIPAwardsXmlDataDict != null)
		{
			return;
		}
		this.VIPAwardsXmlDataDict = new Dictionary<int, TuiGuangPart.AwardXmlData>();
		XElement isolateResXml = Global.GetIsolateResXml("Config/TuiGuang/TuiGuangYuanVip.xml");
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Config");
		if (isolateResXml == null)
		{
			return;
		}
		TuiGuangPart.AwardXmlData.VipLevel = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "TuiGuangYuanVip"), "VipLevel");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			TuiGuangPart.AwardXmlData awardXmlData = new TuiGuangPart.AwardXmlData();
			awardXmlData.GoodsOne = Global.GetXElementAttributeStr(xelement2, "GoodsOne");
			awardXmlData.GoodsTwo = Global.GetXElementAttributeStr(xelement2, "GoodsTwo");
			int id = awardXmlData.ID;
			if (!this.VIPAwardsXmlDataDict.ContainsKey(id))
			{
				this.VIPAwardsXmlDataDict.Add(id, awardXmlData);
			}
		}
	}

	public void InitLevelAwardsXmlDataDict()
	{
		if (this.LevelAwardsXmlDataDict != null)
		{
			return;
		}
		this.LevelAwardsXmlDataDict = new Dictionary<int, TuiGuangPart.AwardXmlData>();
		XElement isolateResXml = Global.GetIsolateResXml("Config/TuiGuang/TuiGuangYuanLevel.xml");
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Config");
		if (isolateResXml == null)
		{
			return;
		}
		TuiGuangPart.AwardXmlData.MinZhuanSheng = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "TuiGuangYuanLevel"), "MinZhuanSheng");
		TuiGuangPart.AwardXmlData.MinLevel = Global.GetXElementAttributeInt(Global.GetXElement(xelement, "TuiGuangYuanLevel"), "MinLevel");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			TuiGuangPart.AwardXmlData awardXmlData = new TuiGuangPart.AwardXmlData();
			awardXmlData.GoodsOne = Global.GetXElementAttributeStr(xelement2, "GoodsOne");
			awardXmlData.GoodsTwo = Global.GetXElementAttributeStr(xelement2, "GoodsTwo");
			int id = awardXmlData.ID;
			if (!this.LevelAwardsXmlDataDict.ContainsKey(id))
			{
				this.LevelAwardsXmlDataDict.Add(id, awardXmlData);
			}
		}
	}

	public void InitNewAwardsXmlDataDict()
	{
		if (this.NewBoyAwardsXmlDataDict != null)
		{
			return;
		}
		this.NewBoyAwardsXmlDataDict = new Dictionary<int, TuiGuangPart.AwardXmlData>();
		XElement isolateResXml = Global.GetIsolateResXml("Config/TuiGuang/TuiGuangXinYongHu.xml");
		if (isolateResXml == null)
		{
			return;
		}
		XElement xelement = Global.GetXElement(isolateResXml, "Config");
		if (isolateResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(xelement, "GiftList"), "*");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			TuiGuangPart.AwardXmlData awardXmlData = new TuiGuangPart.AwardXmlData();
			awardXmlData.GoodsOne = Global.GetXElementAttributeStr(xelement2, "GoodsOne");
			awardXmlData.GoodsTwo = Global.GetXElementAttributeStr(xelement2, "GoodsTwo");
			int id = awardXmlData.ID;
			if (!this.NewBoyAwardsXmlDataDict.ContainsKey(id))
			{
				this.NewBoyAwardsXmlDataDict.Add(id, awardXmlData);
			}
		}
	}

	public const int MAX_AWARD_LISTSIZE = 4;

	public const int LISTBOX_LEIJI = 2;

	public const int LISTBOX_NEW = 3;

	public const int MAX_AWARD_BOX = 6;

	public const int TAB1 = 1;

	public const int TAB2 = 2;

	public const int TABSIZE = 2;

	public TuiGuangData Data = new TuiGuangData();

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton btnClose;

	public GButton tab1Btn;

	public GButton tab2Btn;

	public GameObject tabView1;

	public GameObject tabView2;

	public UILabel TuiGuangAmount;

	public UILabel TuiGuangID;

	public UISprite TuiGuangIDBack;

	public GButton ShareWeChatBtn;

	public UILabel VIPAwardTip;

	public UILabel VIPAwardLeft;

	public GButton VIPAwardBtn;

	public UILabel LevelAwardTip;

	public UILabel LevelAwardLeft;

	public GButton LevelAwardBtn;

	public UILabel BoxAwardLeft;

	public UILabel BoxAwardTip;

	public GButton BoxAwardBtn;

	public GImgProgressBar BoxAwardBar;

	public GButton[] AwardBoxBtn;

	public UILabel[] AwardBoxLabel;

	public UILabel TuiJianRenInfo;

	public TextBox TuiJianRenIDInput;

	public GButton CheckIDBtn;

	public UILabel BeiTuiJianDateTip;

	public UILabel BeiTuiJianAwardTip;

	public GButton BeiTuiJianAwardBtn;

	public ListBox[] AwardItemsListBoxArray;

	public UIPanel PhoneNumCheckPanel;

	public UILabel PhoneNumTitleLabel;

	public GButton PhoneNumCeckPanelCloseBtn;

	public UILabel PhoneNumLabel;

	public TextBox PhoneNumInput;

	public GButton PhoneNumBtn;

	public UILabel PhoneKeywordLabel;

	public TextBox PhoneKeywordInput;

	public GButton PhoneKeywordBtn;

	public UIPanel TipPanel;

	public UILabel TipContextLabel;

	public GButton TipOkBtn;

	private GButton tempBtn;

	private int m_nInputFocusTimeC8B8A8;

	private DispatcherTimer UITimer;

	private int m_nTickCount;

	private int m_nCurrentTab = 1;

	private List<GGoodIcon>[] itemsListArr;

	private ObservableCollection[] ItemCollections;

	private int[] itemsListArrLineLimit;

	private int[] itemsListArrAllLimit;

	private static string _sTuiJianRenID = string.Empty;

	public Dictionary<int, TuiGuangPart.AwardXmlData> LeiJiAwardsXmlDataDict;

	public Dictionary<int, TuiGuangPart.AwardXmlData> VIPAwardsXmlDataDict;

	public Dictionary<int, TuiGuangPart.AwardXmlData> LevelAwardsXmlDataDict;

	public Dictionary<int, TuiGuangPart.AwardXmlData> NewBoyAwardsXmlDataDict;

	public class AwardXmlData
	{
		public static int VipLevel;

		public static int MinZhuanSheng;

		public static int MinLevel;

		public int ID;

		public int MinNum;

		public string GoodsOne;

		public string GoodsTwo;
	}

	private enum TuiGuangAwardType
	{
		VIP = 1,
		Level,
		Amount,
		BeTuiJian
	}
}
