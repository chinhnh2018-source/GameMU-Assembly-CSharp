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

public class HuiJiPart : UserControl
{
	protected override void InitializeComponent()
	{
		if (Global.Data.roleData.HuiJiData == null)
		{
			Global.Data.roleData.HuiJiData = new RoleHuiJiData();
			Global.Data.roleData.HuiJiData.huiji = 1;
		}
		else if (Global.Data.roleData.HuiJiData.huiji == 0)
		{
			Global.Data.roleData.HuiJiData.huiji = 1;
		}
		this.m_HuiJiConfig.Init();
		this.m_CheStarZuanShi.Check = false;
		this.InitText();
		this.BtnClick();
		this.InitLevel();
		this.RefreshMain(this.m_Data.UpLevel, this.m_Data.StartLevel);
	}

	private void InitText()
	{
		this.m_BtnStar.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetLang("升星")
		});
		this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetLang("自动升星")
		});
		this.m_BtnUp.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetLang("升阶")
		});
		this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetLang("自动升阶")
		});
		this.m_LabEWai.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("额外使用效果")
		});
		this.m_LabYongJiu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("永久属性")
		});
		this.m_LabUpLevelZhuFu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("祝福越高则下次成功概率越高")
		});
		this.m_LabBangZhuTitle0.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("激活说明：")
		});
		this.m_LabBangZhuNext0.text = string.Concat(new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("1、")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("提升至1、2、5、8阶")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("可")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("激活")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("额外的徽记")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("使用效果")
			}),
			Environment.NewLine,
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("2、提升阶数可提升徽记")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("使用效果")
			})
		});
		this.m_LabBangZhuTitle1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("控制效果：")
		});
		this.m_LabBangZhuNext1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("减速、冰冻、眩晕、击退、定身、昏迷、麻痹等")
		});
		this.m_LabBangZhuTitle2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("特殊效果：")
		});
		this.m_LabBangZhuNext2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("致命一击、幸运一击、卓越一击")
		});
		this.m_LabWeiJiHuoText.text = Global.GetColorStringForNGUIText(new object[]
		{
			"9d8667",
			Global.GetLang("需要将上一阶徽记升至顶级")
		});
		try
		{
			this.m_LabBangZhuNext1.lineWidth = 420;
			if (this.m_CheStarZuanShi.transform.GetChild(3).name.Equals("zuanshi"))
			{
				this.m_CheStarZuanShi.transform.GetChild(3).gameObject.transform.localPosition = new Vector3(335f, 0f, 0f);
			}
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"赋值出错！"
			});
		}
	}

	private void BtnClick()
	{
		this.m_CheStarZuanShi.CheckChanged = delegate(object e, BaseEventArgs s)
		{
			if (this.m_CheStarZuanShi.Check)
			{
				this.m_CheStarZuanShi.Check = false;
				if (this.BtnType != HuiJiBtnType.ZiDongUp)
				{
					this.strDaiBi = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(this.strDaiBi, "HuiJiShengJie", this.m_ZuanShi);
				}
				else if (this.BtnType != HuiJiBtnType.ZiDongStart)
				{
					this.strDaiBi = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(this.strDaiBi, "HuiJiShengXing", this.m_ZuanShi);
				}
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("选择后每次需要消耗{0}，确定执行吗？"), this.strDaiBi)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.m_CheStarZuanShi.Check = true;
					}
					return true;
				};
				return;
			}
			this.m_CheStarZuanShi.Check = false;
		};
		this.m_BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			PlayZone.GlobalPlayZone.CloseHuiJiWindow();
		};
		this.m_BtnBangZhuFanHui.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_GameBangZhuPanel.SetActive(false);
		};
		this.m_BtnBangZhu.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_GameBangZhuPanel.SetActive(true);
		};
		this.m_BtnModelLeft.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.LeftAndRight(-1);
		};
		this.m_BtnModelRight.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.LeftAndRight(1);
		};
		this.m_BtnStar.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.BtnType = HuiJiBtnType.Start;
			this.SetBtn();
		};
		this.m_BtnStarZiDong.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.BtnType != HuiJiBtnType.ZiDongStart)
			{
				this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("取消自动")
				});
				this.BtnType = HuiJiBtnType.ZiDongStart;
				this.SetBtn();
			}
			else
			{
				this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("自动升星")
				});
				this.BtnType = HuiJiBtnType.Start;
			}
		};
		this.m_BtnUp.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.BtnType = HuiJiBtnType.Up;
			this.SetBtn();
		};
		this.m_BtnUpZiDong.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.BtnType != HuiJiBtnType.ZiDongUp)
			{
				this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("取消自动")
				});
				this.BtnType = HuiJiBtnType.ZiDongUp;
				this.SetBtn();
			}
			else
			{
				this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					Global.GetLang("自动升阶")
				});
				this.BtnType = HuiJiBtnType.Up;
			}
		};
	}

	private void InitLevel()
	{
		this.m_Data.huiJiData = new HuiJiUpdateResultData();
		this.m_Data.huiJiData.HuiJi = Global.Data.roleData.HuiJiData.huiji;
		this.m_Data.huiJiData.Exp = Global.Data.roleData.HuiJiData.Exp;
		EmblemStarXml emblemStarXml = this.m_HuiJiConfig.HuiJiStar(Global.Data.roleData.HuiJiData.huiji);
		this.m_Data.UpLevel = emblemStarXml.EmblemLevel;
		this.m_Data.StartLevel = emblemStarXml.EmblemStar;
	}

	private void RefreshMain(int upLevel, int star)
	{
		EmblemStarXml dataStar = this.m_HuiJiConfig.EmblemStar(upLevel, star);
		EmblemUp emblemUp = this.m_HuiJiConfig.GetEmblemUp(upLevel);
		this.playModelKey = upLevel;
		this.m_LabZuanShi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.Data.roleData.UserMoney
		}));
		if (upLevel > this.m_HuiJiConfig.MinUpLevel && upLevel < 10)
		{
			this.m_GameJieShu10.SetActive(false);
			this.m_PriteJieShu.gameObject.SetActive(true);
			this.m_PriteJieShu.spriteName = string.Format("{0}", upLevel.ToString());
		}
		else if (upLevel == 10)
		{
			this.m_GameJieShu10.SetActive(true);
			this.m_PriteJieShu.gameObject.SetActive(false);
		}
		this.m_LabChuFa.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(emblemUp.Instructions)
		});
		this.m_Data.huiJiData.Type = (int)this.m_HuiJiConfig.GetStateType(upLevel, star);
		if (upLevel <= this.m_HuiJiConfig.MinUpLevel)
		{
			this.m_BtnModelLeft.gameObject.SetActive(false);
			this.m_BtnModelRight.gameObject.SetActive(true);
		}
		else if (upLevel >= this.m_HuiJiConfig.MaxUpLevel)
		{
			this.m_BtnModelLeft.gameObject.SetActive(true);
			this.m_BtnModelRight.gameObject.SetActive(false);
		}
		else if (this.m_Data.huiJiData.Type != 2)
		{
			this.m_BtnModelLeft.gameObject.SetActive(true);
			this.m_BtnModelRight.gameObject.SetActive(true);
		}
		this.RefreshStype((HuiJiStateType)this.m_Data.huiJiData.Type, dataStar, emblemUp);
		base.StartCoroutine(this.UpLevelModel(emblemUp.ModID));
		this.AddYongJiu(upLevel, star);
		this.AddXiaoGuo(upLevel, star);
		Super.HideNetWaiting();
	}

	private void RefreshStype(HuiJiStateType StateType, EmblemStarXml dataStar, EmblemUp dataUp)
	{
		if (StateType == HuiJiStateType.StartLevel)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "HuiJiShengXing", dataStar.NeedDiamond, string.Empty);
			this.m_GameStars.SetActive(true);
			this.m_GameUpLevel.SetActive(false);
			this.m_GameMaxLevel.SetActive(false);
			this.m_BarStar.gameObject.SetActive(true);
			this.m_CheStarZuanShi.gameObject.SetActive(true);
			this.m_GoodStarGoods.gameObject.SetActive(true);
			this.m_GameWeiJiHuoPanel.SetActive(false);
			this.m_LabWeiJiHuoText.gameObject.SetActive(false);
			this.m_BarStarEXP.gameObject.SetActive(true);
			this.m_BarStar.sliderValue = (float)dataStar.EmblemStar / (float)this.m_HuiJiConfig.GetMaxStarLevel(dataStar.EmblemLevel);
			this.m_CheStarZuanShi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("道具不足时，自动消耗      {0}"), dataStar.NeedDiamond)
			});
			this.m_ZuanShi = dataStar.NeedDiamond;
			int goodsID = -1;
			int goodsCount = -1;
			if (!string.IsNullOrEmpty(dataStar.NeedGoods) && dataStar.NeedGoods.Split(new char[]
			{
				','
			}).Length == 2)
			{
				goodsID = dataStar.NeedGoods.Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
				goodsCount = dataStar.NeedGoods.Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
			}
			this.AddIcon(goodsID, this.m_GoodStarGoods, goodsCount);
			this.m_BarStarEXP.sliderValue = (float)Global.Data.roleData.HuiJiData.Exp / (float)dataStar.StarExp;
			this.m_LabStarExp.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang(string.Format("{0}/{1}", Global.Data.roleData.HuiJiData.Exp.ToString(), dataStar.StarExp.ToString()))
			});
		}
		else if (StateType == HuiJiStateType.UpLevel)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "HuiJiShengJie", dataStar.NeedDiamond, string.Empty);
			this.m_GameStars.SetActive(false);
			this.m_GameUpLevel.SetActive(true);
			this.m_GameMaxLevel.SetActive(false);
			this.m_BarStar.gameObject.SetActive(false);
			this.m_CheStarZuanShi.gameObject.SetActive(true);
			this.m_GoodStarGoods.gameObject.SetActive(true);
			this.m_BarStarEXP.gameObject.SetActive(true);
			this.m_GameWeiJiHuoPanel.SetActive(false);
			this.m_LabWeiJiHuoText.gameObject.SetActive(false);
			this.m_LabStarExp.text = string.Empty;
			this.m_CheStarZuanShi.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("道具不足时，自动消耗      {0}"), dataUp.NeedDiamond)
			});
			this.m_ZuanShi = dataUp.NeedDiamond;
			this.m_BarStarEXP.sliderValue = (float)Global.Data.roleData.HuiJiData.Exp / (float)(110000 - dataUp.LuckyOne) * 0.8f;
			int goodsID2 = -1;
			int goodsCount2 = -1;
			if (!string.IsNullOrEmpty(dataUp.NeedGoods) && dataUp.NeedGoods.Split(new char[]
			{
				','
			}).Length == 2)
			{
				goodsID2 = dataUp.NeedGoods.Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
				goodsCount2 = dataUp.NeedGoods.Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
			}
			this.AddIcon(goodsID2, this.m_GoodStarGoods, goodsCount2);
		}
		else if (StateType == HuiJiStateType.MaxLevel)
		{
			this.m_GameStars.SetActive(false);
			this.m_GameUpLevel.SetActive(false);
			this.m_GameMaxLevel.SetActive(true);
			this.m_GoodStarGoods.gameObject.SetActive(false);
			this.m_CheStarZuanShi.gameObject.SetActive(false);
			this.m_BarStar.gameObject.SetActive(true);
			this.m_LabStarExp.text = string.Empty;
			this.m_BarStarEXP.gameObject.SetActive(true);
			this.m_GameWeiJiHuoPanel.SetActive(false);
			this.m_LabWeiJiHuoText.gameObject.SetActive(false);
			this.m_BarStar.sliderValue = 1f;
			this.m_BarStarEXP.sliderValue = 1f;
		}
		else if (StateType == HuiJiStateType.YiLevel)
		{
			this.m_GameStars.SetActive(true);
			this.m_GameUpLevel.SetActive(false);
			this.m_GameMaxLevel.SetActive(true);
			this.m_BarStar.gameObject.SetActive(true);
			this.m_CheStarZuanShi.gameObject.SetActive(false);
			this.m_GoodStarGoods.gameObject.SetActive(false);
			this.m_BarStarEXP.gameObject.SetActive(true);
			this.m_LabWeiJiHuoText.gameObject.SetActive(true);
			this.m_GameWeiJiHuoPanel.SetActive(false);
			this.m_BarStar.sliderValue = 1f;
			this.m_BarStarEXP.sliderValue = 1f;
			this.m_LabStarExp.text = string.Empty;
		}
		else if (StateType == HuiJiStateType.None)
		{
			this.m_GameStars.SetActive(false);
			this.m_GameUpLevel.SetActive(false);
			this.m_GameMaxLevel.SetActive(false);
			this.m_BarStar.gameObject.SetActive(true);
			this.m_BarStar.sliderValue = 0f;
			this.m_CheStarZuanShi.gameObject.SetActive(false);
			this.m_GoodStarGoods.gameObject.SetActive(false);
			this.m_BarStarEXP.gameObject.SetActive(false);
			this.m_LabWeiJiHuoText.gameObject.SetActive(true);
			this.m_GameWeiJiHuoPanel.SetActive(true);
			this.m_LabStarExp.text = string.Empty;
		}
	}

	private void SetBtn()
	{
		if (this.m_Data.huiJiData.Type == 3 && this.m_Data.huiJiData.Type == 2)
		{
			return;
		}
		if (this.BtnType == HuiJiBtnType.ZiDongStart || this.BtnType == HuiJiBtnType.ZiDongUp)
		{
			this.m_Data.huiJiData.Auto = 1;
		}
		else
		{
			this.m_Data.huiJiData.Auto = 0;
		}
		if (this.BtnType == HuiJiBtnType.ZiDongStart && this.m_CheStarZuanShi.Check)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("HuiJiShengXing", this.m_ZuanShi, false);
		}
		else if (this.BtnType == HuiJiBtnType.ZiDongUp && this.m_CheStarZuanShi.Check)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("HuiJiShengJie", this.m_ZuanShi, false);
		}
		if (this.m_CheStarZuanShi.Check)
		{
			this.m_Data.huiJiData.ZuanShi = this.m_ZuanShi;
		}
		else
		{
			this.m_Data.huiJiData.ZuanShi = 0;
		}
		GameInstance.Game.SenEmblemUpLevel(this.m_Data.huiJiData);
	}

	public void GetBtn(HuiJiUpdateResultData data)
	{
		Global.Data.roleData.HuiJiData.huiji = data.HuiJi;
		Global.Data.roleData.HuiJiData.Exp = data.Exp;
		if (data == null)
		{
			return;
		}
		if (data.HuiJi == 0)
		{
			return;
		}
		EmblemStarXml emblemStarXml = this.m_HuiJiConfig.HuiJiStar(data.HuiJi);
		if (emblemStarXml == null)
		{
			return;
		}
		EmblemUp emblemUp = this.m_HuiJiConfig.GetEmblemUp(emblemStarXml.EmblemLevel);
		if (emblemUp == null)
		{
			return;
		}
		int stateType = (int)this.m_HuiJiConfig.GetStateType(emblemStarXml.EmblemLevel, emblemStarXml.EmblemStar);
		if (data.Result != 0)
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(data.Result, false, false)), 10, 3);
			this.BtnType = HuiJiBtnType.None;
			this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("自动升星")
			});
			this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("自动升阶")
			});
			if (data.Result == -10)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
			}
			else if (data.Result == -6)
			{
				if (stateType == 0)
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedHuiJiZhiGuang, null, string.Empty, string.Empty);
				}
				else
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedHuiJiShouHu, null, string.Empty, string.Empty);
				}
			}
			return;
		}
		if (this.playModelKey != this.m_Data.UpLevel)
		{
			this.m_Data.UpLevel = emblemStarXml.EmblemLevel;
			this.m_Data.StartLevel = emblemStarXml.EmblemStar;
			this.m_Data.huiJiData.HuiJi = data.HuiJi;
			this.m_Data.huiJiData.Exp = data.Exp;
			this.m_Data.huiJiData.Type = (int)this.m_HuiJiConfig.GetStateType(this.m_Data.UpLevel, this.m_Data.StartLevel);
			this.Refresh(this.m_Data.huiJiData.Result);
			return;
		}
		if (emblemStarXml.EmblemLevel >= this.m_HuiJiConfig.MaxUpLevel && emblemStarXml.EmblemStar >= this.m_HuiJiConfig.GetMaxStarLevel(emblemStarXml.EmblemLevel))
		{
			this.RefreshStype(HuiJiStateType.MaxLevel, emblemStarXml, emblemUp);
			this.AddYongJiu(emblemStarXml.EmblemLevel, emblemStarXml.EmblemStar);
		}
		if (this.m_Data.huiJiData.ZuanShi == 0)
		{
			if (data.Exp > 0 && data.Exp - this.m_Data.huiJiData.Exp > emblemStarXml.GoodsExp)
			{
				this.SetBaoJi();
			}
		}
		else if (data.Exp > 0 && data.Exp - this.m_Data.huiJiData.Exp > emblemStarXml.ZuanShiExp)
		{
			this.SetBaoJi();
		}
		if (stateType == this.m_Data.huiJiData.Type)
		{
			if (stateType == 0)
			{
				int goodsID = -1;
				int goodsCount = -1;
				if (!string.IsNullOrEmpty(emblemStarXml.NeedGoods) && emblemStarXml.NeedGoods.Split(new char[]
				{
					','
				}).Length == 2)
				{
					goodsID = emblemStarXml.NeedGoods.Split(new char[]
					{
						','
					})[0].SafeToInt32(0);
					goodsCount = emblemStarXml.NeedGoods.Split(new char[]
					{
						','
					})[1].SafeToInt32(0);
				}
				this.AddIcon(goodsID, this.m_GoodStarGoods, goodsCount);
			}
			else if (stateType == 1)
			{
				int goodsID2 = -1;
				int goodsCount2 = -1;
				if (!string.IsNullOrEmpty(emblemUp.NeedGoods) && emblemUp.NeedGoods.Split(new char[]
				{
					','
				}).Length == 2)
				{
					goodsID2 = emblemUp.NeedGoods.Split(new char[]
					{
						','
					})[0].SafeToInt32(0);
					goodsCount2 = emblemUp.NeedGoods.Split(new char[]
					{
						','
					})[1].SafeToInt32(0);
				}
				this.AddIcon(goodsID2, this.m_GoodStarGoods, goodsCount2);
			}
		}
		this.m_LabZuanShi.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.Data.roleData.UserMoney
		}));
		if (stateType == this.m_Data.huiJiData.Type)
		{
			if (data.Exp - this.m_Data.huiJiData.Exp > 0 && stateType == 0)
			{
				Super.HintMainText(Global.GetLang(string.Format("+{0}", data.Exp - this.m_Data.huiJiData.Exp)), 10, 3);
			}
			else if (stateType == 0)
			{
				Super.HintMainText(string.Format("{0}", Global.GetLang("升星成功")), 10, 3);
			}
			if (this.m_Data.huiJiData.Type == 0)
			{
				this.m_BarStar.sliderValue = (float)emblemStarXml.EmblemStar / (float)this.m_HuiJiConfig.GetMaxStarLevel(emblemStarXml.EmblemLevel);
				this.m_BarStarEXP.sliderValue = (float)data.Exp / (float)emblemStarXml.StarExp;
				this.m_LabStarExp.text = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					string.Format("{0}/{1}", data.Exp.ToString(), emblemStarXml.StarExp.ToString())
				}));
				if (this.playModelKey == emblemUp.EmblemLevel)
				{
					this.AddYongJiu(emblemStarXml.EmblemLevel, emblemStarXml.EmblemStar);
				}
			}
			else if (this.m_Data.huiJiData.Type == 1)
			{
				this.m_BarStarEXP.sliderValue = (float)data.Exp / (float)(110000 - emblemUp.LuckyOne) * 0.8f;
			}
		}
		else
		{
			this.BtnType = HuiJiBtnType.None;
			if (stateType == 0)
			{
				Super.HintMainText(string.Format("{0}", Global.GetLang("升阶成功")), 10, 3);
				this.m_Data.huiJiData.Exp = data.Exp;
				this.playModelKey = emblemStarXml.EmblemLevel;
				this.RefreshMain(emblemStarXml.EmblemLevel, emblemStarXml.EmblemStar);
				this.m_teXiao = Global.LoadTeXiaoObj("UITeXiao/Perfabs/huiji/huiji_shengjie", this.m_GameUpTeXiao.transform);
				this.m_teXiao.transform.localPosition = new Vector3(-173f, 0f, -510f);
				Vector3 localPosition = this.m_teXiao.transform.localPosition;
				localPosition.x = 0f;
				localPosition.y = 0f;
				localPosition.z = -150f;
				this.m_teXiao.transform.localPosition = localPosition;
				this.m_teXiao.SetActive(true);
				base.StartCoroutine<bool>(this.DestroyTeXiao());
			}
			else if (stateType == 1)
			{
				Super.HintMainText(string.Format("{0}", Global.GetLang("升星成功")), 10, 3);
				this.m_Data.huiJiData.Exp = data.Exp;
				base.StartCoroutine<bool>(this.UpMaxStarIEnum((HuiJiStateType)stateType, emblemStarXml, emblemUp));
				if (this.playModelKey == emblemUp.EmblemLevel)
				{
					this.AddYongJiu(emblemStarXml.EmblemLevel, emblemStarXml.EmblemStar);
					this.AddXiaoGuo(emblemStarXml.EmblemLevel, emblemStarXml.EmblemStar);
				}
			}
		}
		if (stateType == 0)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "HuiJiShengXing", this.m_ZuanShi, string.Empty);
		}
		else if (stateType == 1)
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "HuiJiShengJie", this.m_ZuanShi, string.Empty);
		}
		this.m_Data.UpLevel = emblemStarXml.EmblemLevel;
		this.m_Data.StartLevel = emblemStarXml.EmblemStar;
		this.m_Data.huiJiData.HuiJi = data.HuiJi;
		this.m_Data.huiJiData.Exp = data.Exp;
		this.m_Data.huiJiData.Type = (int)this.m_HuiJiConfig.GetStateType(this.m_Data.UpLevel, this.m_Data.StartLevel);
		this.Refresh(this.m_Data.huiJiData.Result);
	}

	public void Refresh(int ret)
	{
		if (ret != 0)
		{
			this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("自动升星")
			});
			this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("自动升阶")
			});
			return;
		}
		if (this.BtnType == HuiJiBtnType.ZiDongStart && !IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("HuiJiShengJie", this.m_ZuanShi))
		{
			base.StartCoroutine<bool>(this.UpLevelIEnum());
		}
		else if (this.BtnType == HuiJiBtnType.ZiDongUp && !IConfigbase<ConfigDaiBiShiYong>.Instance.CloseZiDong("HuiJiShengJie", this.m_ZuanShi))
		{
			base.StartCoroutine<bool>(this.UpLevelIEnum());
		}
		else
		{
			this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("自动升星")
			});
			this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				Global.GetLang("自动升阶")
			});
		}
	}

	private IEnumerator DestroyTeXiao()
	{
		yield return new WaitForSeconds(1.5f);
		if (null != this.m_teXiao)
		{
			Object.Destroy(this.m_teXiao);
			this.m_teXiao = null;
		}
		yield break;
	}

	private IEnumerator UpMaxStarIEnum(HuiJiStateType type, EmblemStarXml starData, EmblemUp upData)
	{
		this.m_BarStar.sliderValue = 1f;
		yield return new WaitForSeconds(0.1f);
		this.RefreshStype(type, starData, upData);
		yield break;
	}

	private IEnumerator UpLevelIEnum()
	{
		yield return new WaitForSeconds(0.2f);
		if (this.BtnType == HuiJiBtnType.ZiDongStart || this.BtnType == HuiJiBtnType.ZiDongUp)
		{
			this.SetBtn();
		}
		yield break;
	}

	public override void Destroy()
	{
		if (this.resourceResLoader != null)
		{
			this.resourceResLoader.Stop();
			this.resourceResLoader = null;
		}
		base.Destroy();
	}

	private IEnumerator UpLevelModel(int model)
	{
		this.m_HuiJiConfig.m_ModelAddBool = true;
		if (this.m_ModelBool)
		{
			yield return new WaitForSeconds(0f);
			this.m_ModelBool = false;
		}
		else
		{
			yield return new WaitForSeconds(1.3f);
		}
		if (this.resourceResLoader != null)
		{
			this.resourceResLoader.Stop();
		}
		this.resourceResLoader = this.m_HuiJiConfig.DicUIModel(model, this.m_GameModel);
		this.m_HuiJiConfig.m_ModelAddBool = false;
		yield break;
	}

	private void SetBaoJi()
	{
		if (this.m_ListTeXiao[2].activeSelf)
		{
			this.m_ListTeXiao[2].SetActive(false);
		}
		this.m_ListTeXiao[2].SetActive(true);
	}

	private void AddYongJiu(int upLevel, int star)
	{
		HuiJiTextItem[] componentsInChildren = this.m_GameYongJiu.GetComponentsInChildren<HuiJiTextItem>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.DestroyObject(componentsInChildren[i].gameObject);
			}
		}
		EmblemUp emblemUp = this.m_HuiJiConfig.GetEmblemUp(upLevel);
		EmblemStarXml emblemStarXml = this.m_HuiJiConfig.EmblemStar(upLevel, star);
		HuiJiStateType stateType = this.m_HuiJiConfig.GetStateType(upLevel, star);
		if (emblemUp == null || emblemStarXml == null)
		{
			return;
		}
		int[] array = new int[]
		{
			emblemUp.LifeV + emblemStarXml.LifeV,
			emblemUp.AddAttack + emblemStarXml.AddAttack,
			emblemUp.AddDefense + emblemStarXml.AddDefense,
			emblemUp.DecreaseInjureValue + emblemStarXml.DecreaseInjureValue
		};
		int[] array2 = new int[4];
		this.m_HuiJiConfig.EmblemStarAdd(stateType, upLevel, star, out array2[0], out array2[1], out array2[2], out array2[3]);
		string[] array3 = new string[]
		{
			Global.GetLang("生命上限："),
			Global.GetLang("攻  击  力："),
			Global.GetLang("防  御  力："),
			Global.GetLang("抵挡伤害：")
		};
		for (int j = 0; j < array.Length; j++)
		{
			HuiJiTextItem huiJiTextItem = U3DUtils.NEW<HuiJiTextItem>();
			U3DUtils.AddChild(this.m_GameYongJiu, huiJiTextItem.gameObject, true);
			huiJiTextItem.transform.localPosition = new Vector3(0f, (float)(-30 * j), -1f);
			string lang = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				array3[j]
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				array[j].ToString()
			}));
			string lang2 = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				array2[j].ToString()
			}));
			bool imgBool = (stateType == HuiJiStateType.StartLevel || stateType == HuiJiStateType.UpLevel) && emblemStarXml.ID == Global.Data.roleData.HuiJiData.huiji;
			if (upLevel >= this.m_HuiJiConfig.MaxUpLevel && star >= this.m_HuiJiConfig.GetMaxStarLevel(upLevel))
			{
				imgBool = false;
			}
			huiJiTextItem.SetText(lang, lang2, imgBool);
		}
	}

	private void AddXiaoGuo(int upLevel, int star)
	{
		HuiJiTextItem[] componentsInChildren = this.m_GameXiaoGuo.GetComponentsInChildren<HuiJiTextItem>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.DestroyObject(componentsInChildren[i].gameObject);
			}
		}
		EmblemUp emblemUp = this.m_HuiJiConfig.GetEmblemUp(upLevel);
		if (emblemUp == null)
		{
			return;
		}
		string[] array = new string[]
		{
			Global.GetLang("1阶激活："),
			Global.GetLang("2阶激活："),
			Global.GetLang("5阶激活："),
			Global.GetLang("8阶激活："),
			Global.GetLang("持续时间："),
			Global.GetLang("冷却时间：")
		};
		float[] array2 = new float[]
		{
			emblemUp.SubAttackInjurePercent,
			emblemUp.SPAttackInjurePercent,
			emblemUp.AttackInjurePercent,
			emblemUp.ElementAttackInjurePercent,
			emblemUp.DurationTime / 1000f,
			emblemUp.CDTime / 1000f
		};
		HuiJiStateType stateType = this.m_HuiJiConfig.GetStateType(upLevel, star);
		float[] array3 = new float[6];
		EmblemUp emblemUp2 = this.m_HuiJiConfig.EmblemUpAdd(stateType, upLevel);
		array3[0] = emblemUp2.SubAttackInjurePercent;
		array3[1] = emblemUp2.SPAttackInjurePercent;
		array3[2] = emblemUp2.AttackInjurePercent;
		array3[3] = emblemUp2.ElementAttackInjurePercent;
		array3[4] = emblemUp2.DurationTime / 1000f;
		array3[5] = emblemUp2.CDTime / 1000f;
		float[] array4 = new float[]
		{
			this.m_HuiJiConfig.GetEmblemUp(1).SubAttackInjurePercent,
			this.m_HuiJiConfig.GetEmblemUp(2).SPAttackInjurePercent,
			this.m_HuiJiConfig.GetEmblemUp(5).AttackInjurePercent,
			this.m_HuiJiConfig.GetEmblemUp(8).ElementAttackInjurePercent
		};
		for (int j = 0; j < array2.Length; j++)
		{
			HuiJiTextItem huiJiTextItem = U3DUtils.NEW<HuiJiTextItem>();
			U3DUtils.AddChild(this.m_GameXiaoGuo, huiJiTextItem.gameObject, false);
			huiJiTextItem.transform.localPosition = new Vector3(0f, (float)(-30 * j), -1f);
			string title = string.Empty;
			string addNumber = string.Empty;
			bool imgBool = stateType == HuiJiStateType.UpLevel && this.playModelKey == this.m_HuiJiConfig.HuiJiStar(Global.Data.roleData.HuiJiData.huiji).EmblemLevel;
			if (array3[j] == 0f)
			{
				imgBool = false;
			}
			if (j == 0)
			{
				addNumber = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					(array3[j] * 100f).ToString("f0") + "%"
				});
				if (upLevel < 1)
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"808081",
						string.Format("{0}{1}{2}{3}", new object[]
						{
							array[j],
							Global.GetLang("伤害吸收加成"),
							(array4[j] * 100f).ToString(),
							"%"
						})
					}));
					imgBool = false;
				}
				else
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						array[j]
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}{1}", Global.GetLang("伤害吸收加成"), (array2[j] * 100f).ToString() + "%")
					}));
				}
			}
			else if (j == 1)
			{
				addNumber = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					(array3[j] * 100f).ToString("f0") + "%"
				});
				if (upLevel < 2)
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"808081",
						string.Format("{0}{1}{2}{3}", new object[]
						{
							array[j],
							Global.GetLang("特殊效果伤害减免"),
							(array4[j] * 100f).ToString(),
							"%"
						})
					}));
					imgBool = false;
				}
				else
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						array[j]
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}{1}", Global.GetLang("特殊效果伤害减免"), (array2[j] * 100f).ToString() + "%")
					}));
				}
			}
			else if (j == 2)
			{
				addNumber = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					(array3[j] * 100f).ToString("f0") + "%"
				});
				if (upLevel < 5)
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"808081",
						string.Format("{0}{1}{2}{3}", new object[]
						{
							array[j],
							Global.GetLang("物理/魔法伤害减免"),
							(array4[j] * 100f).ToString(),
							"%"
						})
					}));
					imgBool = false;
				}
				else
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						array[j]
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}{1}", Global.GetLang("物理/魔法伤害减免"), (array2[j] * 100f).ToString() + "%")
					}));
				}
			}
			else if (j == 3)
			{
				addNumber = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					(array3[j] * 100f).ToString("f0") + "%"
				});
				if (upLevel < 8)
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"808081",
						string.Format("{0}{1}{2}{3}", new object[]
						{
							array[j],
							Global.GetLang("元素伤害减免"),
							(array4[j] * 100f).ToString(),
							"%"
						})
					}));
					imgBool = false;
				}
				else
				{
					title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						array[j]
					}) + Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						string.Format("{0}{1}", Global.GetLang("元素伤害减免"), (array2[j] * 100f).ToString() + "%")
					}));
				}
			}
			else
			{
				title = Global.GetLang(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					array[j]
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					array2[j].ToString()
				}) + "s");
				addNumber = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					array3[j].ToString("f0") + "s"
				});
			}
			if (stateType == HuiJiStateType.UpLevel || stateType == HuiJiStateType.StartLevel)
			{
				huiJiTextItem.SetText(title, addNumber, imgBool);
			}
			else
			{
				huiJiTextItem.SetText(title, addNumber, false);
			}
		}
	}

	private void LeftAndRight(int count)
	{
		if (this.m_HuiJiConfig.m_ModelAddBool)
		{
			return;
		}
		this.m_ShowZhiHui.gameObject.SetActive(false);
		this.BtnType = HuiJiBtnType.None;
		this.m_BtnStarZiDong.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetLang("自动升星")
		});
		this.m_BtnUpZiDong.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fffffe",
			Global.GetLang("自动升阶")
		});
		this.playModelKey += count;
		HuiJiStateType stateType = this.m_HuiJiConfig.GetStateType(this.playModelKey, this.m_Data.StartLevel);
		HuiJiStateType stateType2 = this.m_HuiJiConfig.GetStateType(this.playModelKey - 1, this.m_Data.StartLevel);
		HuiJiStateType stateType3 = this.m_HuiJiConfig.GetStateType(this.playModelKey + 1, this.m_Data.StartLevel);
		if (this.playModelKey <= this.m_HuiJiConfig.MinUpLevel)
		{
			this.m_BtnModelLeft.gameObject.SetActive(false);
			this.m_BtnModelRight.gameObject.SetActive(true);
		}
		else if (this.playModelKey >= this.m_HuiJiConfig.MaxUpLevel)
		{
			this.m_BtnModelLeft.gameObject.SetActive(true);
			this.m_BtnModelRight.gameObject.SetActive(false);
		}
		else if ((stateType2 == HuiJiStateType.StartLevel || stateType2 == HuiJiStateType.UpLevel) && (stateType3 == HuiJiStateType.StartLevel || stateType3 == HuiJiStateType.UpLevel || stateType3 == HuiJiStateType.MaxLevel))
		{
			this.m_BtnModelLeft.gameObject.SetActive(true);
			this.m_BtnModelRight.gameObject.SetActive(true);
		}
		else if ((stateType2 == HuiJiStateType.StartLevel || stateType2 == HuiJiStateType.UpLevel) && stateType3 == HuiJiStateType.None)
		{
			if (stateType == HuiJiStateType.StartLevel || stateType == HuiJiStateType.UpLevel)
			{
				this.m_BtnModelLeft.gameObject.SetActive(true);
				this.m_BtnModelRight.gameObject.SetActive(true);
			}
			else
			{
				this.m_BtnModelLeft.gameObject.SetActive(true);
				this.m_BtnModelRight.gameObject.SetActive(false);
				this.m_ShowZhiHui.gameObject.SetActive(true);
			}
		}
		if (this.playModelKey >= this.m_HuiJiConfig.MinUpLevel && this.playModelKey < 10)
		{
			this.m_GameJieShu10.SetActive(false);
			this.m_PriteJieShu.gameObject.SetActive(true);
			this.m_PriteJieShu.spriteName = string.Format("{0}", this.playModelKey.ToString());
		}
		else if (this.playModelKey == 10)
		{
			this.m_GameJieShu10.SetActive(true);
			this.m_PriteJieShu.gameObject.SetActive(false);
		}
		EmblemStarXml dataStar = this.m_HuiJiConfig.EmblemStar(this.playModelKey, this.m_Data.StartLevel);
		EmblemUp emblemUp = this.m_HuiJiConfig.GetEmblemUp(this.playModelKey);
		if (stateType != HuiJiStateType.None && this.playModelKey < this.m_Data.UpLevel)
		{
			this.AddYongJiu(this.playModelKey, this.m_HuiJiConfig.GetMaxStarLevel(this.playModelKey));
			this.RefreshStype(HuiJiStateType.MaxLevel, dataStar, emblemUp);
		}
		else if (this.playModelKey == this.m_Data.UpLevel)
		{
			this.RefreshStype(stateType, dataStar, emblemUp);
			this.AddYongJiu(this.playModelKey, this.m_Data.StartLevel);
		}
		else
		{
			this.AddYongJiu(this.playModelKey, this.m_HuiJiConfig.GetMinStarLevel(this.playModelKey));
			this.RefreshStype(HuiJiStateType.None, dataStar, emblemUp);
		}
		this.AddXiaoGuo(this.playModelKey, this.m_Data.StartLevel);
		if (this.resourceResLoader != null)
		{
			this.resourceResLoader.Stop();
		}
		this.resourceResLoader = this.m_HuiJiConfig.DicUIModel(emblemUp.ModID, this.m_GameModel);
	}

	private void AddIcon(int GoodsID, GameObject parent, int GoodsCount = -1)
	{
		if (GoodsID == -1)
		{
			return;
		}
		GGoodIcon componentInChildren = parent.GetComponentInChildren<GGoodIcon>();
		int num = -1;
		if (componentInChildren != null)
		{
			if (componentInChildren.ItemCode == GoodsID)
			{
				if (GoodsID != -1)
				{
					num = Global.GetRoleGoodsNumberCountByGoodsID(GoodsID);
				}
				if (GoodsCount == -1)
				{
					return;
				}
				if (num >= GoodsCount)
				{
					componentInChildren.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}/{1}", num, GoodsCount)
					});
				}
				else
				{
					componentInChildren.SText = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}/{1}", num, GoodsCount)
					});
				}
				return;
			}
			else
			{
				Object.DestroyObject(componentInChildren.gameObject);
			}
		}
		GGoodIcon ggoodIcon = this.initGood(Global.GetEmptyGoodsData(GoodsID, 1, 1, 0, 1, 1, 1, 1, 1), true);
		ggoodIcon.transform.SetParent(parent.transform, false);
		ggoodIcon.transform.localPosition = new Vector3(0f, 0f, -0.8f);
		ggoodIcon.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
		ggoodIcon.SecondText.Label.supportEncoding = true;
		if (GoodsID != -1)
		{
			num = Global.GetRoleGoodsNumberCountByGoodsID(GoodsID);
		}
		if (GoodsCount == -1)
		{
			return;
		}
		if (num >= GoodsCount)
		{
			ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format("{0}/{1}", num, GoodsCount)
			});
		}
		else
		{
			ggoodIcon.SText = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				string.Format("{0}/{1}", num, GoodsCount)
			});
		}
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
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
			if (BHaveTips)
			{
				ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.ShowGoodsTip(e);
				};
			}
		}
		return ggoodIcon;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	public GameObject m_GameXiaoGuo;

	public GameObject m_GameYongJiu;

	public GameObject m_GameModel;

	public GameObject m_GameUpTeXiao;

	public GameObject m_teXiao;

	public GameObject m_GameStars;

	public GameObject m_GameUpLevel;

	public GameObject m_GameMaxLevel;

	public GameObject m_GoodStarGoods;

	public GameObject m_GameBangZhuPanel;

	public GameObject m_GameJieShu10;

	public GameObject m_GameWeiJiHuoPanel;

	public UILabel m_LabEWai;

	public UILabel m_LabYongJiu;

	public UILabel m_LabZuanShi;

	public UILabel m_LabChuFa;

	public UILabel m_LabUpLevelZhuFu;

	public UILabel m_LabStarExp;

	public UILabel m_LabStarNumber;

	public UILabel m_LabBangZhuTitle0;

	public UILabel m_LabBangZhuNext0;

	public UILabel m_LabBangZhuTitle1;

	public UILabel m_LabBangZhuNext1;

	public UILabel m_LabBangZhuTitle2;

	public UILabel m_LabBangZhuNext2;

	public UILabel m_LabWeiJiHuoText;

	public UISprite m_PriteJieShu;

	public GButton m_BtnClose;

	public GButton m_BtnBangZhuFanHui;

	public GButton m_BtnBangZhu;

	public GButton m_BtnModelLeft;

	public GButton m_BtnModelRight;

	public GButton m_BtnStar;

	public GButton m_BtnStarZiDong;

	public GButton m_BtnUp;

	public GButton m_BtnUpZiDong;

	public GCheckBox m_CheStarZuanShi;

	public GImgProgressBar m_BarStar;

	public GImgProgressBar m_BarStarEXP;

	public ShowNetImage m_ShowZhiHui;

	public List<GameObject> m_ListTeXiao;

	private HuiJiBtnType BtnType;

	private HuiJiData m_Data = new HuiJiData();

	private int playModelKey = 1;

	private HuiJiConfig m_HuiJiConfig = new HuiJiConfig();

	private int m_ZuanShi = -1;

	private bool m_ModelBool = true;

	public List<UISprite> listDaiBi = new List<UISprite>();

	private string strDaiBi = Global.GetLang("钻石");

	private ResourceResLoader resourceResLoader;
}
