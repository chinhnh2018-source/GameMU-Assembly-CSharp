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

public class YaoSaiCallBossPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.ItemCollection = this.mAwards.ItemsSource;
	}

	private void IsShowMask(bool isShow)
	{
		NGUITools.SetActive(this.mMaskWaiting, isShow);
	}

	private void InitTextInPrefabs()
	{
		this.m_BtnYiJianWuXing.Text = Global.GetLang("一键五星");
		this.m_BangZhuTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("一键五星")
		});
		this.m_BangZhuBtnZhaoHuan.Text = Global.GetLang("召唤");
		this.mBtnNormalCallBoss.Text = Global.GetLang("普通召唤");
		this.mBtnDiamondCallBoss.Text = Global.GetLang("钻石召唤");
		this.mBtnTaoFaBoss.Text = Global.GetLang("讨伐(0/0)");
		this.mLblBossAwardTitle.Text = Global.GetLang("讨伐奖励");
		string chineseText = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", new object[]
		{
			Global.GetLang("{fac60d}召唤BOSS规则：{-}\n"),
			Global.GetLang("{dac7ae}1、召唤BOSS有{17e43e}免费召唤{-}和{17e43e}钻石召唤{-}两种 {-}\n"),
			Global.GetLang("{dac7ae}2、免费召唤每天可以召唤{ff0000}1{-}次, 每天{17e43e}0{-}点\n"),
			Global.GetLang("刷新{-}\n"),
			Global.GetLang("{dac7ae}3、BOSS有{17e43e}星级{-}, {17e43e}星级{-}越高奖励越好{-}\n"),
			Global.GetLang("{dac7ae}4、召唤出BOSS后可以{17e43e}继续召唤{-}, 新的\n"),
			Global.GetLang("BOSS会顶替{17e43e}已召唤{-}BOSS{-}\n"),
			Global.GetLang("{dac7ae}5、召唤到合适的BOSS{17e43e}进行讨伐{-}{-}\n"),
			Global.GetLang("{dac7ae}6、有{17e43e}讨伐中{-}的BOSS{ff0000}不能召唤{-}新的BOSS{-}\n")
		});
		this.mHelpContent.text = Global.GetLang(chineseText);
		this.mHelpContent.pivot = 3;
		this.mHelpContent.transform.localPosition = new Vector3(-202f, this.mHelpContent.transform.localPosition.y, this.mHelpContent.transform.localPosition.z);
		this.m_BangZhuCheck._Lable.lineWidth = 311;
	}

	private void InitEvent()
	{
		if (this.m_BtnYiJianWuXing != null)
		{
			this.m_BtnYiJianWuXing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.m_BossStarLevel >= 5)
				{
					Super.HintMainText(Global.GetLang("当前BOSS已经达到五星"), 10, 3);
				}
				else
				{
					this.m_BangZhuPanel.gameObject.SetActive(true);
				}
			};
		}
		if (this.m_BangZhuClose != null)
		{
			this.m_BangZhuClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.m_BangZhuPanel.gameObject.SetActive(false);
			};
		}
		if (this.m_BangZhuBtnZhaoHuan != null)
		{
			this.m_BangZhuBtnZhaoHuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				int num = 0;
				if (Global.Data.roleData.GoodsDataList != null)
				{
					for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
					{
						if (Global.Data.roleData.GoodsDataList[i].GoodsID == ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", ',')[0].SafeToInt32(0))
						{
							num = Global.Data.roleData.GoodsDataList[i].GCount;
						}
					}
				}
				if (this.m_BangZhuCheck.isChecked)
				{
					if (num >= ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|')[0].Split(new char[]
					{
						','
					})[1].SafeToInt32(0))
					{
						this.m_BangZhuPanel.gameObject.SetActive(false);
						GameInstance.Game.SendZhaoHuanBossRequest(2);
					}
					else if (Global.Data.roleData.UserMoney >= ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|')[1].SafeToInt32(0) || IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingYaoSaiZhaoHuanBoss", ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|')[1].SafeToInt32(0), false))
					{
						string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "JingLingYaoSaiZhaoHuanBoss", ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|')[1].SafeToInt32(0));
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|')[1].SafeToInt32(0), text)
						}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								GameInstance.Game.SendZhaoHuanBossRequest(3);
								this.m_BangZhuPanel.gameObject.SetActive(false);
							}
							return true;
						};
					}
					else
					{
						Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
					}
				}
				else if (num >= ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|')[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0))
				{
					this.m_BangZhuPanel.gameObject.SetActive(false);
					GameInstance.Game.SendZhaoHuanBossRequest(2);
				}
				else
				{
					Super.HintMainText(Global.GetLang("道具不足"), 10, 3);
				}
			};
		}
		if (this.mBtnClose != null)
		{
			this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.CloseHandler != null)
				{
					this.CloseHandler(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
		}
		if (this.mBtnNormalCallBoss != null)
		{
			this.mBtnNormalCallBoss.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.m3DModelParent.transform.childCount > 0)
				{
					NoTitleWindow noTitleWindow = Super.ShowDialogBox(PlayZone.GlobalPlayZone, 1, Global.GetLang("本次召唤将会替代已召BOSS, 确定要召唤吗？"), 300, 100, 0, string.Empty, "确定", "取消");
					noTitleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						if ((s1 as NoTitleWindow).DialogBoxReturn == 0)
						{
							GameInstance.Game.SendZhaoHuanBossRequest(0);
						}
						Super.CloseNoTitleWindow(PlayZone.GlobalPlayZone, s1 as NoTitleWindow);
						return true;
					};
				}
				else
				{
					GameInstance.Game.SendZhaoHuanBossRequest(0);
				}
			};
		}
		if (this.m_BtnYiJianWuXing != null)
		{
		}
		if (this.mBtnDiamondCallBoss != null)
		{
			this.mBtnDiamondCallBoss.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("钻石"), "JingLingYaoSaiPuTongZhaoHuan", this.costDiamond_cfg);
				string chineseText;
				if (this.isFirstCostDiamondCallBoss)
				{
					if (this.m3DModelParent.transform.childCount > 0)
					{
						chineseText = string.Format(Global.GetLang("本次召唤将会消耗{0}{1}，并替代已召BOSS，确定要召唤吗？"), this.costDiamond_cfg, text);
					}
					else
					{
						chineseText = string.Format(Global.GetLang("本次召唤将会消耗{0}{1}，确定要召唤吗？"), this.costDiamond_cfg, text);
					}
				}
				else if (this.m3DModelParent.transform.childCount > 0)
				{
					chineseText = Global.GetLang("本次召唤将会替代已召BOSS，确定要召唤吗？");
				}
				else
				{
					chineseText = string.Format(Global.GetLang("本次召唤将会消耗{0}{1}，确定要召唤吗？"), this.costDiamond_cfg, text);
				}
				NoTitleWindow noTitleWindow = Super.ShowDialogBox(PlayZone.GlobalPlayZone, 1, Global.GetLang(chineseText), 300, 100, 0, string.Empty, "确定", "取消");
				noTitleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					if ((s1 as NoTitleWindow).DialogBoxReturn == 0)
					{
						this.isFirstCostDiamondCallBoss = false;
						GameInstance.Game.SendZhaoHuanBossRequest(1);
					}
					Super.CloseNoTitleWindow(PlayZone.GlobalPlayZone, s1 as NoTitleWindow);
					return true;
				};
			};
		}
		if (this.mBtnTaoFaBoss != null)
		{
			this.mBtnTaoFaBoss.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GameInstance.Game.SendTaoFaBossRequest();
			};
		}
		if (this.mHelpBtn != null)
		{
			UIEventListener.Get(this.mHelpBtn.gameObject).onClick = delegate(GameObject s)
			{
				if (this.mHelpObj != null)
				{
					this.mHelpObj.SetActive(!this.mHelpObj.activeSelf);
				}
			};
		}
		if (this.mHelpDisplayBtn != null)
		{
			this.mHelpDisplayBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.mHelpObj != null)
				{
					this.mHelpObj.SetActive(!this.mHelpObj.activeSelf);
				}
			};
		}
		NGUITools.SetActive(this.mBtnNormalCallBoss.gameObject, false);
		NGUITools.SetActive(this.mBtnDiamondCallBoss.gameObject, false);
	}

	private void InitValue()
	{
		if (Global.isFanbei(14))
		{
			this.m_ShowFanBei.gameObject.SetActive(true);
			this.m_ShowFanBei.transform.localScale = new Vector3(91.8f, 21.6f, 0f);
		}
		else
		{
			this.m_ShowFanBei.gameObject.SetActive(false);
		}
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ManorSuperBoss", '|');
		if (systemParamStringArrayByName != null && systemParamStringArrayByName.Length == 2)
		{
			string text = string.Empty;
			text = systemParamStringArrayByName[0].ToString() + ",0,0,0,0,0";
			if (!string.IsNullOrEmpty(text))
			{
				this.m_ObservableCollection = this.m_ListBox.ItemsSource;
				Super.LoadGoodsList(text, this.m_ObservableCollection);
			}
			if (systemParamStringArrayByName[0].Split(new char[]
			{
				','
			}) != null && systemParamStringArrayByName[0].Split(new char[]
			{
				','
			}).Length > 0)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(systemParamStringArrayByName[0].Split(new char[]
				{
					','
				})[0].SafeToInt32(0));
				this.m_BangZhuCheck._Lable.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("没有")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"b266ff",
					Global.GetLang("【") + goodsXmlNodeByID.Title + Global.GetLang("】")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("，自动消耗") + systemParamStringArrayByName[1].ToString()
				});
				string text2 = string.Concat(new string[]
				{
					"没有",
					Global.GetLang("【"),
					goodsXmlNodeByID.Title,
					Global.GetLang("】"),
					Global.GetLang("，自动消耗")
				});
				float num = this.m_BangZhuCheck._Lable.transform.localPosition.x + (float)text2.Length * this.m_BangZhuCheck._Lable.transform.localScale.x + (float)(systemParamStringArrayByName[1].ToString().Length * 10);
				this.m_BangZhuZuanShi.transform.localPosition = new Vector3(num, 2f, -0.5f);
			}
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBiYiJian[0], "JingLingYaoSaiZhaoHuanBoss", systemParamStringArrayByName[1].SafeToInt32(0), string.Empty);
			if (IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingYaoSaiZhaoHuanBoss", systemParamStringArrayByName[1].SafeToInt32(0), false))
			{
				this.mBtnDiamondCallBoss.Text = Global.GetLang("代币召唤");
			}
			else
			{
				this.mBtnDiamondCallBoss.Text = Global.GetLang("钻石召唤");
			}
		}
		this.RefreshSumDiamond();
		this.BossDefaultInfo();
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossCall", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length > 0)
		{
			this.freeCallBoss_cfg = systemParamIntArrayByName[0];
			this.costDiamond_cfg = systemParamIntArrayByName[1];
			this.taoFaTimes_cfg = systemParamIntArrayByName[2];
		}
	}

	private void RefreshSumDiamond()
	{
		this.mLblDiamond.Text = Global.Data.roleData.UserMoney.ToString();
	}

	public void InitCallBoss(int callBossID, int taoFaTimes = 0, int freeCallBossTimes = 0)
	{
		if (callBossID > 0)
		{
			this.mTaoFaBossID = callBossID;
			this.TaoFaTimes = taoFaTimes;
			this.FreeCallBossTimes = freeCallBossTimes;
			this.LoadBossModel();
		}
		else
		{
			this.CallBossCostDiamond = "0";
			this.FreeCallBossTimes = freeCallBossTimes;
		}
	}

	private void LoadBossModel()
	{
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(this.mTaoFaBossID);
		if (dataByID != null)
		{
			this.Load3DBossModel(dataByID.MonsterID, dataByID.Scaling);
			this.BossName = Global.GetColorStringForNGUIText(new object[]
			{
				this.GetColorOfBossNameByQuality(dataByID.Quality),
				dataByID.Name
			});
			this.BossDescribeInfo = ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID).Talk;
			this.BossStarLevel = dataByID.Star;
			this.CallBossCostDiamond = this.costDiamond_cfg.ToString();
			this.SetBtnStatus(this.mBtnTaoFaBoss, true);
			if (!string.IsNullOrEmpty(dataByID.KillAward))
			{
				List<string> list = new List<string>();
				string[] array = dataByID.KillAward.Split(new char[]
				{
					'|'
				});
				string[] array2 = dataByID.KillExtraAward.Split(new char[]
				{
					'|'
				});
				list.Clear();
				if (array.Length > 0)
				{
					for (int i = 0; i < array.Length; i++)
					{
						list.Add(array[i]);
					}
				}
				if (array2.Length > 0)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						list.Add(array2[j]);
					}
				}
				this.LoadGoodsList(list.ToArray());
			}
		}
	}

	private string BossName
	{
		set
		{
			this.mLblBossName.Text = value;
		}
	}

	private string BossDescribeInfo
	{
		set
		{
			this.mLblBossDescribe.Text = value;
		}
	}

	private string CallBossCostDiamond
	{
		set
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBiZhaoHuan[0], "JingLingYaoSaiPuTongZhaoHuan", value.SafeToInt32(0), string.Empty);
			this.mLblCostDiamond.Text = value;
		}
	}

	private int FreeCallBossTimes
	{
		get
		{
			return this.mFreeCallBossTimes;
		}
		set
		{
			this.mFreeCallBossTimes = value;
			if (this.mFreeCallBossTimes >= this.freeCallBoss_cfg)
			{
				NGUITools.SetActive(this.mBtnNormalCallBoss.gameObject, false);
				NGUITools.SetActive(this.mBtnDiamondCallBoss.gameObject, true);
				this.mFreeCallBossTimes = this.freeCallBoss_cfg;
				this.CallBossCostDiamond = this.costDiamond_cfg.ToString();
			}
			else
			{
				NGUITools.SetActive(this.mBtnNormalCallBoss.gameObject, true);
				NGUITools.SetActive(this.mBtnDiamondCallBoss.gameObject, false);
			}
		}
	}

	private int TaoFaTimes
	{
		set
		{
			if (value < 0)
			{
				return;
			}
			if (value > this.taoFaTimes_cfg)
			{
				value = this.taoFaTimes_cfg;
				this.SetBtnStatus(this.mBtnTaoFaBoss, false);
			}
			string chineseText = string.Format("{0}({1}/{2})", Global.GetLang("讨伐"), value, this.taoFaTimes_cfg);
			this.mBtnTaoFaBoss.Text = Global.GetLang(chineseText);
		}
	}

	private int BossStarLevel
	{
		set
		{
			this.m_BossStarLevel = value;
			if (this.mStars.Length == 5 && value <= this.mStars.Length && value >= 0)
			{
				for (int i = 0; i < this.mStars.Length; i++)
				{
					NGUITools.SetActive(this.mStars[i].gameObject, false);
				}
				if (value == 0)
				{
					return;
				}
				for (int j = 0; j < value; j++)
				{
					NGUITools.SetActive(this.mStars[j].gameObject, true);
				}
			}
		}
	}

	private void BossDefaultInfo()
	{
		this.BossStarLevel = 0;
		this.BossDescribeInfo = Global.GetLang("                       未刷新");
		this.BossName = "BOSS";
		this.mBtnTaoFaBoss.Text = Global.GetLang("讨伐(0/0)");
		this.SetBtnStatus(this.mBtnTaoFaBoss, false);
	}

	private void SetBtnStatus(GButton btn, bool isActivity)
	{
		if (null == btn)
		{
			return;
		}
		btn.GetComponent<BoxCollider>().enabled = isActivity;
		btn.GetComponentInChildren<UISprite>().color = ((!isActivity) ? Color.gray : Color.white);
	}

	private string GetColorOfBossNameByQuality(int quality)
	{
		if (quality == 1)
		{
			return "17e43e";
		}
		if (quality == 2)
		{
			return "3681f3";
		}
		if (quality == 3)
		{
			return "b266ff";
		}
		return "fac60d";
	}

	private void Load3DBossModel(int monsterID, float scale = 1f)
	{
		if (this.m3DModelParent != null)
		{
			this.DestroyBossModel();
			if (this.resLoader != null)
			{
				this.resLoader.Stop();
			}
			this.resLoader = UIHelper.LoadMonsterRes(this.m3DModelParent, monsterID, scale);
			this.IsShowMask(false);
		}
	}

	public void NotifyRefreshCallBossInfo(int callBossID, int freeCallBossTimes)
	{
		if (callBossID > 0)
		{
			this.LoadEffect("yaosai_boss_zhaohuan", 1);
			this.mTaoFaBossID = callBossID;
			this.FreeCallBossTimes = freeCallBossTimes;
			if (Global.Data.mYaoSaiBossMainData != null)
			{
				this.TaoFaTimes = Global.Data.mYaoSaiBossMainData.TaoFaCount;
			}
			this.RefreshSumDiamond();
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBiZhaoHuan[0], "JingLingYaoSaiPuTongZhaoHuan", this.costDiamond_cfg, string.Empty);
	}

	private void LoadEffect(string name, int type = 1)
	{
		this.IsShowMask(true);
		this.DestroyBossModel();
		base.StopCoroutine(this.LightingEffect());
		this.ClearChildrenOfTransform(this.mEffect.transform);
		this.mTeXiao = this.GetEffect(name);
		if (this.mTeXiao == null)
		{
			this.IsShowMask(false);
			return;
		}
		U3DUtils.AddChild(this.mEffect, this.mTeXiao, false);
		if (type == 1)
		{
			this.mTeXiao.transform.localPosition = new Vector3(-176f, 0f, 0f);
			base.StartCoroutine<bool>(this.LightingEffect());
		}
	}

	private IEnumerator LightingEffect()
	{
		yield return new WaitForSeconds(this.loadLightingEffect);
		if (null != this.mTeXiao)
		{
			Object.Destroy(this.mTeXiao);
			this.mTeXiao = null;
			this.ClearChildrenOfTransform(this.mEffect.transform);
		}
		this.DestroyBossModel();
		this.LoadBossModel();
		this.ClearChildrenOfTransform(this.mEffect.transform);
		this.IsShowMask(false);
		yield break;
	}

	private GameObject GetEffect(string effectName)
	{
		string text = string.Format("{0}{1}", "UITeXiao/Perfabs/yaosai/", effectName);
		return Object.Instantiate(Resources.Load(text)) as GameObject;
	}

	private void ClearChildrenOfTransform(Transform t)
	{
		if (t == null)
		{
			return;
		}
		int childCount = t.childCount;
		if (childCount > 0)
		{
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(t.GetChild(i).gameObject);
			}
		}
	}

	private void DestroyBossModel()
	{
		if (this.m3DModelParent != null)
		{
			int childCount = this.m3DModelParent.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Object.Destroy(this.m3DModelParent.transform.GetChild(i).gameObject);
			}
		}
	}

	private void LoadGoodsList(string[] goods)
	{
		this.ItemCollection.Clear();
		for (int i = 0; i < goods.Length; i++)
		{
			string[] array = goods[i].Split(new char[]
			{
				','
			});
			if (array.Length == 7)
			{
				if (Global.isFanbei(14))
				{
					this.AddGoodsIcon(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]) * 2, Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
				}
				else
				{
					this.AddGoodsIcon(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
				}
			}
		}
	}

	private void AddGoodsIcon(int goodsID, int gcount, int binding, int forgeLevel, int zhuijiaLevel = 0, int lucky = 0, int zhuoyueIndex = 0)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, gcount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon;
			if (dummyGoodsDataMu != null)
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
				ggoodIcon.Width = 64.0;
				ggoodIcon.Height = 64.0;
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemObject = dummyGoodsDataMu;
				ggoodIcon.isAutoSize = true;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
				ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
				bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			}
			else
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 64.0;
				ggoodIcon.Height = 64.0;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			}
			this.ItemCollection.AddNoUpdate(ggoodIcon);
			ggoodIcon.transform.localPosition = new Vector3(ggoodIcon.transform.localPosition.x, ggoodIcon.transform.localPosition.y, -0.2f);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	public static bool ErrorCode(int errorCode)
	{
		bool result = false;
		switch (errorCode)
		{
		case 0:
			result = true;
			break;
		case 1:
			Super.HintMainText(Global.GetLang("需要先创建要塞"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("先领取上个BOSS奖励，再召唤新的BOSS"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("已讨伐的BOSS消失后，才可以召唤新的BOSS"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("免费召唤次数已用完"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
			break;
		case 6:
			Super.HintMainText(Global.GetLang("没有随机到Boss"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("没有Boss"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("精灵阵容不能为空"), 10, 3);
			break;
		case 9:
			Super.HintMainText(Global.GetLang("没有战斗次数"), 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("背包已满"), 10, 3);
			break;
		case 11:
			Super.HintMainText(Global.GetLang("今日讨伐已达上限，明日再来讨伐"), 10, 3);
			break;
		case 12:
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			break;
		case 13:
			Super.HintMainText(Global.GetLang("道具不足"), 10, 3);
			break;
		default:
			Super.HintMainText(string.Format("{0}{1}", Global.GetLang("未知错误码： "), errorCode), 10, 3);
			break;
		}
		return result;
	}

	private void OnEnable()
	{
		base.StopCoroutine(this.LightingEffect());
		this.IsShowMask(false);
	}

	protected override void OnDestroy()
	{
		this.IsShowMask(false);
		base.StopCoroutine(this.LightingEffect());
		this.mTeXiao = null;
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mBtnNormalCallBoss = null;
		this.mBtnDiamondCallBoss = null;
		this.mBtnTaoFaBoss = null;
		this.mLblDiamond = null;
		this.mLblCostDiamond = null;
		this.mLblBossDescribe = null;
		this.mStars = null;
		this.mLblBossName = null;
		this.mLblBossAwardTitle = null;
		this.m3DModelParent = null;
		if (this.resLoader != null)
		{
			this.resLoader.Stop();
		}
	}

	private const string effectDianLiang = "yaosai_boss_zhaohuan";

	private const string effectYunCeng = "yaosai_yunceng";

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public GButton mBtnNormalCallBoss;

	public GButton mBtnDiamondCallBoss;

	public GButton mBtnTaoFaBoss;

	public GButton m_BtnYiJianWuXing;

	public TextBlock mLblDiamond;

	public TextBlock mLblCostDiamond;

	public TextBlock mLblBossDescribe;

	public UISprite[] mStars;

	public TextBlock mLblBossName;

	public TextBlock mLblBossAwardTitle;

	public Modal3DShow m3DModelParent;

	public GameObject mEffect;

	public ListBox mAwards;

	public ShowNetImage m_ShowFanBei;

	public UIPanel m_BangZhuPanel;

	public UILabel m_BangZhuTitle;

	public UICheckbox m_BangZhuCheck;

	public UISprite m_BangZhuZuanShi;

	public GButton m_BangZhuBtnZhaoHuan;

	public GButton m_BangZhuClose;

	public ListBox m_ListBox;

	private int m_BossStarLevel;

	public ObservableCollection m_ObservableCollection;

	public List<UISprite> listDaiBiYiJian = new List<UISprite>();

	public List<UISprite> listDaiBiZhaoHuan = new List<UISprite>();

	private ObservableCollection _ItemCollection;

	private int freeCallBoss_cfg;

	private int costDiamond_cfg;

	private int taoFaTimes_cfg;

	private int mTaoFaBossID;

	private int mFreeCallBossTimes;

	public GameObject mHelpObj;

	public UIButton mHelpBtn;

	public UILabel mHelpContent;

	public GButton mHelpDisplayBtn;

	public float loadLightingEffect = 2f;

	public float loadCloudEffectTime = 1.5f;

	public float loadBossCompleteCallBackTime = 1f;

	public float destroyCloudEffectTime = 1.5f;

	private GameObject mTeXiao;

	private bool isFirstCostDiamondCallBoss = true;

	public GameObject mMaskWaiting;

	private MonsterNPCResLoader resLoader;
}
