using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YaoSaiBossInfoPart : UserControl
{
	public ObservableCollection TaoFaItemCollection
	{
		get
		{
			return this._TaoFaItemCollection;
		}
		set
		{
			this._TaoFaItemCollection = value;
		}
	}

	public ObservableCollection JingLingItemCollection
	{
		get
		{
			return this._JingLingItemCollection;
		}
		set
		{
			this._JingLingItemCollection = value;
		}
	}

	public ObservableCollection JingLingZuHeItemCollection
	{
		get
		{
			return this._JingLingZuHeCollection;
		}
		set
		{
			this._JingLingZuHeCollection = value;
		}
	}

	private int mBossID { get; set; }

	private string mRecommandJingLingZuHe { get; set; }

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.TaoFaItemCollection = this.mTaoFaAwards.ItemsSource;
		this.JingLingItemCollection = this.mJingLingZhenRongListBox.ItemsSource;
		this.JingLingZuHeItemCollection = this.mJingLingZuHeListBox.ItemsSource;
	}

	private void InitPlayerHasJingLingZhenRong(int jingLingIndex, int jingLingDBId)
	{
		if (this.mJingLingPositionDict.ContainsKey(jingLingIndex))
		{
			this.mJingLingPositionDict[jingLingIndex] = jingLingDBId;
		}
		else
		{
			this.mJingLingPositionDict.Add(jingLingIndex, jingLingDBId);
		}
	}

	private void InitMyJingLingZhenRong(string jingLingZhenRongStr)
	{
		if (string.IsNullOrEmpty(jingLingZhenRongStr))
		{
			return;
		}
		this.ResetAllZuHeJingLingShaderToGray();
		this.JingLingItemCollection.Clear();
		string[] array = jingLingZhenRongStr.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			YaoSaiJingLingZhenRongItem yaoSaiJingLingZhenRongItem = U3DUtils.NEW<YaoSaiJingLingZhenRongItem>();
			yaoSaiJingLingZhenRongItem.JingLingIconById = ConvertExt.SafeConvertToInt32(array[i]);
			yaoSaiJingLingZhenRongItem.JingLingIndex = i;
			yaoSaiJingLingZhenRongItem.mBossId = this.mBossID;
			yaoSaiJingLingZhenRongItem.mJingLingZhenRongStr = jingLingZhenRongStr;
			yaoSaiJingLingZhenRongItem.JingLingDBId = ConvertExt.SafeConvertToInt32(array[i]);
			this.InitPlayerHasJingLingZhenRong(yaoSaiJingLingZhenRongItem.JingLingIndex, yaoSaiJingLingZhenRongItem.JingLingDBId);
			yaoSaiJingLingZhenRongItem.RefreshMyJingLingHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				string title = e.Title;
				if (!string.IsNullOrEmpty(title))
				{
					this.InitMyJingLingZhenRong(title);
				}
			};
			UIPanel component = yaoSaiJingLingZhenRongItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			this.JingLingItemCollection.Add(yaoSaiJingLingZhenRongItem);
		}
		this.DisplayMyJingLingDamageValue(this.mJingLingPositionDict);
		this.JingLingZuHeListContaionsSpecialJingLing(this.GetMatchJingLingZuHeListToLightIcon());
	}

	private void SaveJingLingID(int jingLingIndex, int jingLingID)
	{
		if (this.mJingLingPositionDict.ContainsKey(jingLingIndex))
		{
			this.mJingLingPositionDict[jingLingIndex] = jingLingID;
		}
		else
		{
			this.mJingLingPositionDict.Add(jingLingIndex, jingLingID);
		}
		this.DisplayMyJingLingDamageValue(this.mJingLingPositionDict);
	}

	private string GetJingLingStrID()
	{
		if (this.mJingLingPositionDict == null || this.mJingLingPositionDict.Count <= 0)
		{
			return "0|0|0";
		}
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<int, int>.Enumerator enumerator = this.mJingLingPositionDict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			StringBuilder stringBuilder2 = stringBuilder;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			stringBuilder2.Append(keyValuePair.Value);
			stringBuilder.Append("|");
		}
		if (stringBuilder.Length > 0)
		{
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'|'
			});
		}
		return stringBuilder.ToString();
	}

	private void InitTextInPrefabs()
	{
		this.mLblJingLingBaseDamageValue.Pivot = 3;
		this.mLblJingLingBaseDamageValue.X = 85.0;
		this.JingLingZhenRong_Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("我的阵容")
		});
		this.mLblJingLingZuHeDamage.Label.text = Global.GetLang("推荐精灵伤害加成：");
		this.mLblHelpContent.pivot = 3;
		this.mLblHelpContent.transform.localPosition = new Vector3(-200f, this.mLblHelpContent.transform.localPosition.y, this.mLblHelpContent.transform.localPosition.z);
		this.mLblHelpContent.lineWidth = 445;
		this.mLblFreeFightTimes.Pivot = 3;
		this.mLblFreeFightTimes.X = 80.0;
		this.mBtnBattle.Text = Global.GetLang("开始战斗(0/0)");
		this.mLblKillAward.Text = Global.GetLang("战斗奖励：");
		this.mLblLeftTime.Text = Global.GetLang("剩余时间：");
		this.mLblJingLingSumDamage.Text = Global.GetLang("精灵预估伤害：");
		this.mLblJingLingZuHe.Text = Global.GetLang("推荐精灵:");
		this.mLblFreeFight.Text = Global.GetLang("免费战斗：");
		this.mLblCostFight.Text = Global.GetLang("消耗：");
		this.mXieZhuHelpContent = string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
		{
			Global.GetLang("{fac60d}协助攻击BOSS规则：{-}\n"),
			Global.GetLang("{dac7ae}1、通过要塞界面左侧的{17e43e}好友、战盟列表{-}，\n"),
			Global.GetLang("      进入好友或战盟成员的要塞，{17e43e}协助击杀{-}\n"),
			Global.GetLang("      他们讨伐的BOSS {-}\n"),
			Global.GetLang("{dac7ae}2、每个BOSS只能协助攻击{17e43e}1{-}次{-}\n"),
			Global.GetLang("{dac7ae}3、每天协助战斗前{17e43e}10{-}次有奖励，达到{ff0000}10{-}次\n"),
			Global.GetLang("      后，可以协助战斗但没有奖励{-}")
		});
		this.mSelfHelpContent = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}", new object[]
		{
			Global.GetLang("{fac60d}讨伐攻击BOSS规则：{-}\n"),
			Global.GetLang("{dac7ae}1、讨伐玩家有{17e43e}3{-}次免费攻击BOSS的机会 {-}\n"),
			Global.GetLang("{dac7ae}2、没有攻击次数时, 消耗{17e43e}钻石{-}可以继续\n"),
			Global.GetLang("攻击BOSS{-}\n"),
			Global.GetLang("{dac7ae}3、每次战斗都会获得{17e43e}战斗奖励{-}{-}\n"),
			Global.GetLang("{dac7ae}4、BOSS{17e43e}击杀{-}或{17e43e}过期{-}后, 讨伐BOSS玩家\n"),
			Global.GetLang("可以领取BOSS{17e43e}讨伐奖励{-}{-}{-}\n"),
			Global.GetLang("{dac7ae}5、{17e43e}所有{-}玩家对BOSS{17e43e}累积{-}造成伤害{17e43e}越多{-}, \n"),
			Global.GetLang("讨伐奖励{17e43e}越多{-}, 成功{17e43e}击杀{-}还有{17e43e}额外{-}奖励"),
			"\n",
			Global.GetLang("{fac60d}协助攻击BOSS规则：{-}\n"),
			Global.GetLang("{dac7ae}1、通过要塞界面左侧的{17e43e}好友、战盟列表{-}, \n"),
			Global.GetLang("进入好友或战盟成员的要塞, {17e43e}协助击杀{-}\n"),
			Global.GetLang("他们讨伐的BOSS {-}\n"),
			Global.GetLang("{dac7ae}2、每个BOSS只能协助攻击{17e43e}1{-}次{-}\n"),
			Global.GetLang("{dac7ae}3、每天协助战斗前{17e43e}10{-}次有奖励, 达到{ff0000}10{-}次\n"),
			Global.GetLang("后, 可以协助战斗但没有奖励{-}")
		});
	}

	private void InitEvent()
	{
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
		if (this.mBtnRecord != null)
		{
			this.mBtnRecord.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				GameInstance.Game.SendFightBossLogRequest(this.BossOwnerID);
			};
		}
		if (this.mBtnHelp != null)
		{
			this.mBtnHelp.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.mHelpWindow != null)
				{
					this.mLblHelpContent.text = Global.GetLang(this.mSelfHelpContent);
					this.mHelpWindow.SetActive(!this.mHelpWindow.activeSelf);
				}
			};
		}
		if (this.mBtnHelpClose != null)
		{
			UIEventListener.Get(this.mBtnHelpClose.gameObject).onClick = delegate(GameObject s)
			{
				if (this.mHelpWindow != null)
				{
					this.mHelpWindow.SetActive(!this.mHelpWindow.activeSelf);
				}
			};
		}
		if (this.mBtnCallBossAward != null)
		{
			this.mBtnCallBossAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				PlayZone.GlobalPlayZone.OpenYaoSaiJingLingBattleAwardsPart(1, this.mCallBossAwards.ToArray(), this.BossOwnerID, 0, 0f, true, 0, null);
			};
		}
		if (this.mBtnBattle != null)
		{
			this.mBtnBattle.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!this.isClick)
				{
					return;
				}
				this.isClick = false;
				if (this.mCostFightObj != null && this.mCostFightObj.activeSelf && this.isFirstPayment && this.mFightStatus != YaoSaiBossInfoPart.EFightStatus.Award)
				{
					string chineseText = string.Format(Global.GetLang("确定花费[{0}]钻进行战斗吗？"), this.mCostDiamondNum);
					NoTitleWindow noTitleWindow = Super.ShowDialogBox(PlayZone.GlobalPlayZone, 1, Global.GetLang(chineseText), 300, 100, 0, string.Empty, "确定", "取消");
					noTitleWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						if ((s1 as NoTitleWindow).DialogBoxReturn == 0)
						{
							this.isFirstPayment = false;
							this.Fight();
						}
						Super.CloseNoTitleWindow(PlayZone.GlobalPlayZone, s1 as NoTitleWindow);
						return true;
					};
				}
				else
				{
					this.Fight();
				}
			};
		}
	}

	private void Fight()
	{
		switch (this.mFightStatus)
		{
		case YaoSaiBossInfoPart.EFightStatus.Self:
			GameInstance.Game.SendFightBossResultRequest(this.BossOwnerID, this.GetJingLingStrID());
			break;
		case YaoSaiBossInfoPart.EFightStatus.XieZhu:
			GameInstance.Game.SendFightBossResultRequest(this.BossOwnerID, this.GetJingLingStrID());
			this.isFightXieZhu = true;
			break;
		case YaoSaiBossInfoPart.EFightStatus.Award:
			GameInstance.Game.SendFightBossAwardRequest();
			break;
		}
	}

	private new void Update()
	{
		if (!this.isClick)
		{
			this.CDTime -= Time.deltaTime;
			if (this.CDTime <= 0f)
			{
				this.CDTime = 1f;
				this.isClick = true;
			}
		}
	}

	private void InitValue()
	{
		this.mJingLingPositionDict.Clear();
		this.mJingLingPositionDict.Add(0, 0);
		this.mJingLingPositionDict.Add(1, 0);
		this.mJingLingPositionDict.Add(2, 0);
		this.effectBeiShuObj = this.GetEffect("yaosai_beishu");
		if (this.effectBeiShuObj != null)
		{
			U3DUtils.AddChild(this.mBeiShuObj, this.effectBeiShuObj, false);
			this.effectBeiShuObj.SetActive(false);
			this.effectBeiShuObj.transform.localPosition = new Vector3(21f, 2f, -2f);
			this.effectBeiShuObj.transform.localScale = new Vector3(1.5f, 1f, 1f);
		}
		if (Global.isFanbei(14))
		{
			this.m_ShowFanBei.gameObject.SetActive(true);
		}
		else
		{
			this.m_ShowFanBei.gameObject.SetActive(false);
		}
	}

	private string BossName
	{
		set
		{
			this.mLblTitle.Text = value;
		}
	}

	private bool IsSelf
	{
		get
		{
			return !(null == this.mFreeFightObj) && this.mFreeFightObj.activeSelf;
		}
		set
		{
			NGUITools.SetActive(this.mFreeFightObj, value);
		}
	}

	private int FreeFightNum
	{
		get
		{
			return this.mFreeFightNum;
		}
		set
		{
			NGUITools.SetActive(this.mFreeLblFightObj, true);
			NGUITools.SetActive(this.mCostFightObj, false);
			this.mFreeFightNum = value;
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossFight", ',');
			if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length > 0)
			{
				int num = systemParamIntArrayByName[0];
				this.mLblFreeFightTimes.Text = string.Format("{0}/{1}", this.mFreeFightNum, num);
				string chineseText = string.Format("{0}({1}/{2})", Global.GetLang("开始战斗"), this.mFreeFightNum, num);
				this.mBtnBattle.Text = Global.GetLang(chineseText);
			}
		}
	}

	private int CostDaimond
	{
		set
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossFight", ',');
			if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length > 0)
			{
				int num = systemParamIntArrayByName[0];
				if (this.FreeFightNum >= num)
				{
					NGUITools.SetActive(this.mFreeLblFightObj, false);
					NGUITools.SetActive(this.mCostFightObj, true);
					this.mLblCostFightValue.Text = value.ToString();
					this.mBtnBattle.Text = Global.GetLang(Global.GetLang("开始战斗"));
				}
			}
		}
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

	public void InitBossInfo(YaoSaiBossFightData data)
	{
		if (data == null)
		{
			Super.HintMainText(Global.GetLang("战斗信息为空"), 10, 3);
			return;
		}
		if (data.BossMiniInfo == null)
		{
			Super.HintMainText(Global.GetLang("Boss信息为空"), 10, 3);
			return;
		}
		this.mJingLingPositionDict.Clear();
		int bossID = data.BossMiniInfo.BossID;
		this.mBossID = data.BossMiniInfo.BossID;
		if (bossID != 0)
		{
			this.BossOwnerID = data.BossMiniInfo.OwnerID;
			PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(bossID);
			if (dataByID != null)
			{
				this.BossName = dataByID.Name;
				this.Load3DBossModel(dataByID.MonsterID, dataByID.Scaling);
				float num = (float)ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID).MaxLife;
				this.mBossLeftHPPercent = (float)data.BossMiniInfo.LifeV / num;
				if (!string.IsNullOrEmpty(dataByID.FightAward))
				{
					string[] array = dataByID.FightAward.Split(new char[]
					{
						'|'
					});
					this.mTaoFaAwardList.Clear();
					for (int i = 0; i < array.Length; i++)
					{
						this.mTaoFaAwardList.Add(array[i]);
					}
					this.LoadGoodsList(this.mTaoFaAwardList.ToArray(), 1);
				}
				if (!string.IsNullOrEmpty(dataByID.KillAward))
				{
					string[] array2 = dataByID.KillAward.Split(new char[]
					{
						'|'
					});
					string[] array3 = null;
					if (!string.IsNullOrEmpty(dataByID.KillExtraAward))
					{
						array3 = dataByID.KillExtraAward.Split(new char[]
						{
							'|'
						});
					}
					this.mCallBossAwards.Clear();
					if (array2.Length > 0)
					{
						for (int j = 0; j < array2.Length; j++)
						{
							this.mCallBossAwards.Add(array2[j]);
						}
					}
					if (array3 != null && array3.Length > 0)
					{
						for (int k = 0; k < array3.Length; k++)
						{
							this.mCallBossAwards.Add(array3[k]);
						}
					}
				}
				this.DisplayJingLingZuHe(dataByID.PetSuit);
			}
			bool flag = data.BossMiniInfo.OwnerID == Global.Data.RoleID;
			this.isSelfBoss = flag;
			this.IsSelf = flag;
			if (flag)
			{
				this.mFightStatus = YaoSaiBossInfoPart.EFightStatus.Self;
				this.FreeFightNum = data.HaveFightTime;
				this.CostDaimond = data.ZuanShiFightCost;
				this.mCostDiamondNum = data.ZuanShiFightCost;
			}
			else
			{
				this.mFightStatus = YaoSaiBossInfoPart.EFightStatus.XieZhu;
				this.mBtnBattle.Text = Global.GetLang("协助战斗");
			}
			NGUITools.SetActive(this.mJingLingKillStatus.gameObject, false);
			if (data.BossMiniInfo.LifeV <= 0.0)
			{
				NGUITools.SetActive(this.mJingLingKillStatus.gameObject, true);
				this.mFightStatus = YaoSaiBossInfoPart.EFightStatus.Award;
				NGUITools.SetActive(this.mFreeFightObj, false);
				this.mBtnBattle.Text = Global.GetLang("领取奖励");
				this.mJingLingKillStatus.spriteName = "yijisha";
				this.ShowHP(this.mBossID, (float)data.BossMiniInfo.LifeV);
			}
			else if (data.BossMiniInfo.DeadTime <= Global.GetCorrectDateTime())
			{
				NGUITools.SetActive(this.mJingLingKillStatus.gameObject, true);
				this.mFightStatus = YaoSaiBossInfoPart.EFightStatus.Award;
				NGUITools.SetActive(this.mFreeFightObj, false);
				this.mBtnBattle.Text = Global.GetLang("领取奖励");
				this.mJingLingKillStatus.spriteName = "yichaoshi";
				this.ShowHP(this.mBossID, (float)data.BossMiniInfo.LifeV);
			}
			this.ShowBossLeftTime(data.BossMiniInfo);
			this.RefreshBossLifeValue(data.BossMiniInfo);
			this.InitMyJingLingZhenRong(data.JingLingZhenRong);
			int num2 = 1;
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossFight", ',');
			if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length > 0)
			{
				num2 = systemParamIntArrayByName[1];
			}
			if (!flag && data.HaveFightTime >= num2)
			{
				this.SetBtnStatus(this.mBtnBattle, false);
			}
		}
	}

	private void Load3DBossModel(int monsterID, float scale = 1f)
	{
		if (this.m3DModelParent != null)
		{
			this.Destroy3DmodelChildren();
			if (this.resLoader != null)
			{
				this.resLoader.Stop();
			}
			this.resLoader = UIHelper.LoadMonsterRes(this.m3DModelParent, monsterID, scale);
		}
	}

	private void Destroy3DmodelChildren()
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

	public void ShowBossLeftTime(YaoSaiBossData bossData)
	{
		base.CancelInvoke("CountDownTime");
		if (this.mFightStatus == YaoSaiBossInfoPart.EFightStatus.Award)
		{
			return;
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		long num = bossData.DeadTime.Ticks - correctDateTime.Ticks;
		this.secondsCountDown = (int)(num / 10000000L);
		base.InvokeRepeating("CountDownTime", 0f, 1f);
	}

	private void CountDownTime()
	{
		this.mLblLeftTimeValue.Text = this.GetTimeStrBySec((double)this.secondsCountDown, false);
		this.secondsCountDown--;
		if (this.secondsCountDown < 0)
		{
			this.mLblLeftTimeValue.Text = "00:00:00";
			if (this.isSelfBoss)
			{
				NGUITools.SetActive(this.mJingLingKillStatus.gameObject, true);
				this.mFightStatus = YaoSaiBossInfoPart.EFightStatus.Award;
				NGUITools.SetActive(this.mFreeFightObj, false);
				this.mBtnBattle.Text = Global.GetLang("领取奖励");
				this.mJingLingKillStatus.spriteName = "yichaoshi";
			}
			else if (this.mFightStatus != YaoSaiBossInfoPart.EFightStatus.Award)
			{
				NGUITools.SetActive(this.mJingLingKillStatus.gameObject, true);
				NGUITools.SetActive(this.mFreeFightObj, false);
				this.SetBtnStatus(this.mBtnBattle, false);
				this.mJingLingKillStatus.spriteName = "yichaoshi";
				if (this.CloseHandler != null)
				{
					this.CloseHandler(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
				Super.HintMainText(Global.GetLang("该BOSS已不存在"), 10, 3);
			}
			base.CancelInvoke("CountDownTime");
		}
	}

	public string GetTimeStrBySec(double sec, bool showDay = true)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		if (!showDay)
		{
			return string.Format("{0:T}", StringUtil.substitute(Global.GetLang("{0}:{1}:{2}"), new object[]
			{
				((int)(sec % (double)num / (double)num2)).ToString("00"),
				((int)(sec % (double)num % (double)num2 / (double)num3)).ToString("00"),
				((int)(sec % (double)num % (double)num2 % (double)num3)).ToString("00")
			}));
		}
		return string.Format("{0:T}", StringUtil.substitute(Global.GetLang("{0}:{1}:{2}:{3}"), new object[]
		{
			((int)(sec / (double)num)).ToString("00"),
			((int)(sec % (double)num / (double)num2)).ToString("00"),
			((int)(sec % (double)num % (double)num2 / (double)num3)).ToString("00"),
			((int)(sec % (double)num % (double)num2 % (double)num3)).ToString("00")
		}));
	}

	public void RefreshBossLifeValue(YaoSaiBossData bossData)
	{
		if (bossData == null || bossData.BossID <= 0)
		{
			return;
		}
		if (bossData.BossID != this.mBossID)
		{
			return;
		}
		if (this.mFightStatus == YaoSaiBossInfoPart.EFightStatus.Award)
		{
			return;
		}
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(bossData.BossID);
		float num = 1f;
		if (dataByID != null)
		{
			num = (float)ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID).MaxLife;
		}
		float num2 = (float)bossData.LifeV / num;
		if (num2 >= 1f)
		{
			num2 = 1f;
		}
		else if (num2 <= 0f)
		{
			num2 = 0f;
			if (bossData.OwnerID != Global.Data.RoleID)
			{
				if (!this.isFightXieZhu)
				{
					if (this.CloseHandler != null)
					{
						this.CloseHandler(null, new DPSelectedItemEventArgs
						{
							IDType = 0
						});
					}
					Super.HintMainText(Global.GetLang("该BOSS已不存在"), 10, 3);
				}
			}
			else
			{
				NGUITools.SetActive(this.mJingLingKillStatus.gameObject, true);
				this.mFightStatus = YaoSaiBossInfoPart.EFightStatus.Award;
				NGUITools.SetActive(this.mFreeFightObj, false);
				this.mBtnBattle.Text = Global.GetLang("领取奖励");
				this.mJingLingKillStatus.spriteName = "yijisha";
			}
		}
		this.mLblLifeValue.Text = string.Format("{0}/{1}", bossData.LifeV, num);
		this.mImgLife.transform.localScale = new Vector3(this.bossLifeBarLength.x * num2, this.bossLifeBarLength.y, this.bossLifeBarLength.z);
	}

	private void ShowHP(int bossId, float HP)
	{
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(bossId);
		float num = 1f;
		if (dataByID != null)
		{
			num = (float)ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID).MaxLife;
		}
		float num2 = HP / num;
		if (num2 >= 1f)
		{
			num2 = 1f;
		}
		else if (num2 <= 0f)
		{
			num2 = 0f;
		}
		this.mLblLifeValue.Text = string.Format("{0}/{1}", HP, num);
		this.mImgLife.transform.localScale = new Vector3(this.bossLifeBarLength.x * num2, this.bossLifeBarLength.y, this.bossLifeBarLength.z);
	}

	public void DisplayJingLingZuHe(string zuHeStr)
	{
		if (string.IsNullOrEmpty(zuHeStr))
		{
			this.mRecommandJingLingZuHe = string.Empty;
			return;
		}
		this.mRecommandJingLingZuHe = zuHeStr;
		this.mCacheJingLingZuHe_Str = zuHeStr;
		this.JingLingZuHeItemCollection.Clear();
		string[] array = zuHeStr.Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			this.AddRecommandJingLingIcon(ConvertExt.SafeConvertToInt32(array[i]));
		}
		int childCount = this.mJingLingZuHeListBox.transform.childCount;
		if (childCount > 0)
		{
			for (int j = 0; j < childCount; j++)
			{
				GGoodIcon component = this.mJingLingZuHeListBox.transform.GetChild(j).GetComponent<GGoodIcon>();
				if (!(component == null))
				{
					UITexture component2 = component.GoodImg.GetComponent<UITexture>();
					if (component2 != null)
					{
						component2.shader = Shader.Find(this.ShaderGray);
					}
				}
			}
		}
	}

	public void NotifyBossInfoByServerData(YaoSaiBossFightResultData data)
	{
		if (data.BossInfo == null)
		{
			return;
		}
		if (!YaoSaiCallBossPart.ErrorCode(data.Result))
		{
			return;
		}
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(data.BossInfo.BossID);
		float num = 1f;
		if (dataByID != null)
		{
			num = (float)ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID).MaxLife;
		}
		float num2 = (float)data.BossInfo.LifeV / num;
		if (num2 >= 1f)
		{
			num2 = 1f;
		}
		else if (num2 <= 0f)
		{
			num2 = 0f;
		}
		PlayZone.GlobalPlayZone.OpenYaoSaiJingLingBattleAwardsPart(2, this.mTaoFaAwardList.ToArray(), data.BossInfo.OwnerID, data.FightLife, num2, data.NeedNotifyAward, 0, null);
		this.RefreshBossLifeValue(data.BossInfo);
		this.BossOwnerID = data.BossInfo.OwnerID;
		if (data.BossInfo.OwnerID == Global.Data.RoleID)
		{
			GameInstance.Game.SendFightBossInfoRequest(Global.Data.RoleID);
		}
	}

	public void NotifyBattleRecordDataByServerData(YaoSaiBossFightLogInfo data)
	{
		if (data.BossFightLogList == null || data.BossFightLogList.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无战斗记录"), 10, 3);
			return;
		}
		this.OpenYaoSaiJingLingBattleRecordPart(data);
	}

	public void NotifyBattleAwardsDataByServerData(int result)
	{
		string[] awards = null;
		string[] extraAwards = null;
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(this.mBossID);
		if (dataByID != null)
		{
			if (!string.IsNullOrEmpty(dataByID.KillAward))
			{
				awards = dataByID.KillAward.Split(new char[]
				{
					'|'
				});
			}
			if (!string.IsNullOrEmpty(dataByID.KillExtraAward))
			{
				extraAwards = dataByID.KillExtraAward.Split(new char[]
				{
					'|'
				});
			}
		}
		PlayZone.GlobalPlayZone.OpenYaoSaiJingLingBattleAwardsPart(3, awards, this.BossOwnerID, 0, this.mBossLeftHPPercent, true, this.mBossID, extraAwards);
	}

	private void LoadGoodsList(string[] goods, int type)
	{
		if (type == 1)
		{
			this.TaoFaItemCollection.Clear();
		}
		if (type == 2)
		{
		}
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
					this.AddGoodsIcon(type, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]) * 2, Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
				}
				else
				{
					this.AddGoodsIcon(type, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
				}
			}
		}
	}

	private void AddGoodsIcon(int type, int goodsID, int gcount, int binding, int forgeLevel, int zhuijiaLevel = 0, int lucky = 0, int zhuoyueIndex = 0)
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
			if (type == 1)
			{
				this.TaoFaItemCollection.Add(ggoodIcon);
			}
			if (type == 2)
			{
			}
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void AddRecommandJingLingIcon(int goodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsID,
				0,
				-1,
				-1
			});
			ggoodIcon.ItemCode = goodsID;
			ggoodIcon.ItemObject = dummyGoodsData;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.STextVisibility = false;
			ggoodIcon.SecondText.gameObject.SetActive(false);
			ggoodIcon.GoodImg.transform.localPosition = new Vector3(0f, 0f, -1.5f);
			ggoodIcon.BackgroundSprite1Visible = false;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			this.JingLingZuHeItemCollection.Add(ggoodIcon);
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

	private void OpenYaoSaiJingLingBattleRecordPart(YaoSaiBossFightLogInfo data)
	{
		this.ClosePaiZhuJingLingPart();
		if (null == this.mYaoSaiJingLingBattleRecordPartWindow)
		{
			this.mYaoSaiJingLingBattleRecordPartWindow = U3DUtils.NEW<GChildWindow>();
			this.mYaoSaiJingLingBattleRecordPartWindow.ModalType = ChildWindowModalType.Translucent;
			this.mYaoSaiJingLingBattleRecordPartWindow.IsShowModal = true;
			Super.InitChildWindow(this.mYaoSaiJingLingBattleRecordPartWindow, Global.GetLang("YaoSaiJingLingBattleRecordPartWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.mYaoSaiJingLingBattleRecordPartWindow);
			this.mYaoSaiJingLingBattleRecordPartWindow.ChildWindowClose = delegate(object s, EventArgs e)
			{
				this.ClosePaiZhuJingLingPart();
				return true;
			};
		}
		if (null == this.mYaoSaiJingLingBattleRecordPart)
		{
			this.mYaoSaiJingLingBattleRecordPart = U3DUtils.NEW<YaoSaiJingLingBattleRecordPart>();
			this.mYaoSaiJingLingBattleRecordPartWindow.Body.Add(this.mYaoSaiJingLingBattleRecordPart);
			this.mYaoSaiJingLingBattleRecordPart.RefreshItemDataByServerData(data);
			this.mYaoSaiJingLingBattleRecordPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (e.IDType == 0)
				{
					this.ClosePaiZhuJingLingPart();
				}
			};
		}
	}

	private void ClosePaiZhuJingLingPart()
	{
		if (null != this.mYaoSaiJingLingBattleRecordPartWindow)
		{
			if (this.mYaoSaiJingLingBattleRecordPart != null)
			{
				Object.Destroy(this.mYaoSaiJingLingBattleRecordPart);
				this.mYaoSaiJingLingBattleRecordPart = null;
			}
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.mYaoSaiJingLingBattleRecordPartWindow);
			this.mYaoSaiJingLingBattleRecordPartWindow = null;
		}
	}

	private void DisplayMyJingLingDamageValue(Dictionary<int, int> myJingLingdict)
	{
		if (myJingLingdict == null || myJingLingdict.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<int, int>.Enumerator enumerator = myJingLingdict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			StringBuilder stringBuilder2 = stringBuilder;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			stringBuilder2.Append(keyValuePair.Value);
			stringBuilder.Append("|");
		}
		string jingLingStr = stringBuilder.ToString().TrimEnd(new char[]
		{
			'|'
		});
		this.mLblJingLingBaseDamageValue.Text = this.GetBaseDamageValue(this.mBossID, jingLingStr).ToString();
		int extraValueDamageValue = this.GetExtraValueDamageValue(this.mBossID, jingLingStr);
		if (extraValueDamageValue <= 0)
		{
			NGUITools.SetActive(this.mLblJingLingRateDamageValue.gameObject, false);
		}
		else
		{
			NGUITools.SetActive(this.mLblJingLingRateDamageValue.gameObject, true);
			this.mLblJingLingRateDamageValue.Text = string.Format("{0}{1}", "+", extraValueDamageValue.ToString());
		}
		int jiaChengBeiShu = this.GetJiaChengBeiShu(this.mBossID, jingLingStr);
		if (jiaChengBeiShu > 0)
		{
			this.mBeiShuNumber.text = string.Format("{0}{1}", jiaChengBeiShu, Global.GetLang("倍"));
			if (this.effectBeiShuObj != null && !this.effectBeiShuObj.activeSelf)
			{
				this.effectBeiShuObj.SetActive(true);
			}
		}
		else
		{
			this.mBeiShuNumber.text = string.Empty;
			if (this.effectBeiShuObj != null)
			{
				this.effectBeiShuObj.SetActive(false);
			}
		}
	}

	private string GetMatchJingLingZuHeListToLightIcon()
	{
		if (this.mJingLingPositionDict == null || this.mJingLingPositionDict.Count <= 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<int, int> keyValuePair in this.mJingLingPositionDict)
		{
			if (keyValuePair.Value > 0)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				Dictionary<int, int>.Enumerator enumerator;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				stringBuilder2.Append(keyValuePair2.Value);
				stringBuilder.Append("|");
			}
		}
		if (stringBuilder.Length > 0)
		{
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'|'
			});
		}
		return string.Empty;
	}

	private void LightJingLingZuHeIcon()
	{
		string[] array = this.mRecommandJingLingZuHe.Split(new char[]
		{
			','
		});
		string matchJingLingZuHeListToLightIcon = this.GetMatchJingLingZuHeListToLightIcon();
		if (string.IsNullOrEmpty(matchJingLingZuHeListToLightIcon))
		{
			return;
		}
		string[] array2 = matchJingLingZuHeListToLightIcon.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array2.Length; i++)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (ConvertExt.SafeConvertToInt32(array2[i]) == ConvertExt.SafeConvertToInt32(array[j]))
				{
				}
			}
		}
	}

	private void JingLingZuHeListContaionsSpecialJingLing(string goodIds)
	{
		if (string.IsNullOrEmpty(this.mCacheJingLingZuHe_Str))
		{
			return;
		}
		if (string.IsNullOrEmpty(goodIds))
		{
			return;
		}
		this.ResetAllZuHeJingLingShaderToGray();
		string[] tmpZeHeList = goodIds.Split(new char[]
		{
			'|'
		});
		int childCount = this.mJingLingZuHeListBox.transform.childCount;
		for (int j = 0; j < childCount; j++)
		{
			if (childCount > 0)
			{
				int i;
				for (i = 0; i < tmpZeHeList.Length; i++)
				{
					GGoodIcon component = this.mJingLingZuHeListBox.transform.GetChild(j).GetComponent<GGoodIcon>();
					if (!(component == null))
					{
						GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData x) => x.Id == ConvertExt.SafeConvertToInt32(tmpZeHeList[i]));
						if (goodsData != null)
						{
							GoodsData goodsData2 = component.ItemObject as GoodsData;
							if (goodsData2 != null)
							{
								if (goodsData2.GoodsID == goodsData.GoodsID)
								{
									UITexture component2 = component.GoodImg.GetComponent<UITexture>();
									if (component2 != null && component2.shader.name == this.ShaderGray)
									{
										component2.shader = Shader.Find(this.ShaderColored);
										bool canUse = Global.CanUseGoods(goodsData2.GoodsID, false, true);
										Super.InitGoodsGIcon(component, goodsData2, canUse, IconTextTypes.Qianghua);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	private void ResetAllZuHeJingLingShaderToGray()
	{
		int childCount = this.mJingLingZuHeListBox.transform.childCount;
		if (childCount > 0)
		{
			for (int i = 0; i < childCount; i++)
			{
				GGoodIcon component = this.mJingLingZuHeListBox.transform.GetChild(i).GetComponent<GGoodIcon>();
				if (!(component == null))
				{
					UITexture component2 = component.GoodImg.GetComponent<UITexture>();
					if (component2 != null)
					{
						component2.shader = Shader.Find(this.ShaderGray);
					}
					component.BackgroundSprite1Visible = false;
				}
			}
		}
	}

	private GameObject GetEffect(string effectName)
	{
		string text = string.Format("{0}{1}", "UITeXiao/Perfabs/yaosai/", effectName);
		return Object.Instantiate(Resources.Load(text)) as GameObject;
	}

	private int GetBaseDamageValue(int bossId, string jingLingStr)
	{
		YaoSaiBossInfoPart.<GetBaseDamageValue>c__AnonStorey432 <GetBaseDamageValue>c__AnonStorey = new YaoSaiBossInfoPart.<GetBaseDamageValue>c__AnonStorey432();
		if (string.IsNullOrEmpty(jingLingStr))
		{
			return 0;
		}
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(bossId);
		if (dataByID == null)
		{
			return 0;
		}
		<GetBaseDamageValue>c__AnonStorey.zhenRongs = jingLingStr.Split(new char[]
		{
			'|'
		});
		List<GoodsData> list = new List<GoodsData>();
		int i;
		for (i = 0; i < <GetBaseDamageValue>c__AnonStorey.zhenRongs.Length; i++)
		{
			GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData x) => x.Id == ConvertExt.SafeConvertToInt32(<GetBaseDamageValue>c__AnonStorey.zhenRongs[i]));
			list.Add(goodsData);
		}
		float num = 0f;
		float num2 = 0f;
		for (int j = 0; j < list.Count; j++)
		{
			GoodsData goodsData2 = list[j];
			if (goodsData2 != null)
			{
				int num3 = 1 + (1 + goodsData2.Forge_level) / dataByID.PetLevelStep;
				string[] array = dataByID.PetLevelStepNum.Split(new char[]
				{
					','
				});
				int num4 = num3 * ConvertExt.SafeConvertToInt32(array[0]);
				int num5 = num3 * ConvertExt.SafeConvertToInt32(array[1]);
				string[] array2 = dataByID.ExcellentStepNum.Split(new char[]
				{
					','
				});
				int num6 = 1 + Global.GetZhuoyueAttributeCount(goodsData2) / dataByID.ExcellentStep;
				int num7 = num6 * ConvertExt.SafeConvertToInt32(array2[0]);
				int num8 = num6 * ConvertExt.SafeConvertToInt32(array2[1]);
				num += (float)(num4 + num7);
				num2 += (float)(num5 + num8);
			}
		}
		return Mathf.CeilToInt((num + num2) / 2f);
	}

	private int GetJiaChengBeiShu(int bossId, string jingLingStr)
	{
		YaoSaiBossInfoPart.<GetJiaChengBeiShu>c__AnonStorey434 <GetJiaChengBeiShu>c__AnonStorey = new YaoSaiBossInfoPart.<GetJiaChengBeiShu>c__AnonStorey434();
		if (string.IsNullOrEmpty(jingLingStr))
		{
			return 0;
		}
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(bossId);
		if (dataByID == null)
		{
			return 0;
		}
		<GetJiaChengBeiShu>c__AnonStorey.zhenRongs = jingLingStr.Split(new char[]
		{
			'|'
		});
		List<GoodsData> list = new List<GoodsData>();
		int i;
		for (i = 0; i < <GetJiaChengBeiShu>c__AnonStorey.zhenRongs.Length; i++)
		{
			GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData x) => x.Id == ConvertExt.SafeConvertToInt32(<GetJiaChengBeiShu>c__AnonStorey.zhenRongs[i]));
			list.Add(goodsData);
		}
		int num = 0;
		bool flag = true;
		string[] array = dataByID.PetSuit.Split(new char[]
		{
			','
		});
		if (array.Length <= 0)
		{
			return 0;
		}
		string[] array2 = dataByID.PetRate.Split(new char[]
		{
			','
		});
		if (array2.Length <= 0)
		{
			return 0;
		}
		for (int k = 0; k < array.Length; k++)
		{
			int j;
			for (j = 0; j < list.Count; j++)
			{
				GoodsData goodsData2 = list[j];
				if (goodsData2 != null)
				{
					if (ConvertExt.SafeConvertToInt32(array[k]) == goodsData2.GoodsID)
					{
						break;
					}
				}
			}
			if (j < list.Count)
			{
				list.RemoveAt(j);
				num += ConvertExt.SafeConvertToInt32(array2[k]);
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			num += dataByID.SuitRate;
		}
		return num;
	}

	private int GetExtraValueDamageValue(int bossId, string jingLingStr)
	{
		return this.GetBaseDamageValue(bossId, jingLingStr) * this.GetJiaChengBeiShu(bossId, jingLingStr);
	}

	protected override void OnDestroy()
	{
		base.CancelInvoke("CountDownTime");
		this.mJingLingPositionDict.Clear();
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mBtnRecord = null;
		this.mBtnHelp = null;
		this.mBtnBattle = null;
		this.mLblTitle = null;
		this.mLblKillAward = null;
		this.mLblLeftTime = null;
		this.mLblLeftTimeValue = null;
		this.mLblJingLingSumDamage = null;
		this.mLblJingLingZuHe = null;
		this.mLblJingLingZuHeDamage = null;
		this.mJingLingKillStatus = null;
		this.m3DModelParent = null;
		this.isFightXieZhu = false;
		if (this.resLoader != null)
		{
			this.resLoader.Stop();
			this.resLoader = null;
		}
	}

	private const int TaoFaFlag = 1;

	private const int KillFlag = 2;

	private const string effectBeiShu = "yaosai_beishu";

	private const int CallBossViewAward = 1;

	private const int BattleAward = 2;

	private const int CallAward = 3;

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public GButton mBtnRecord;

	public GButton mBtnHelp;

	public GButton mBtnBattle;

	public GButton mBtnCallBossAward;

	public TextBlock mLblTitle;

	public TextBlock mLblKillAward;

	public TextBlock mLblLeftTime;

	public TextBlock mLblLeftTimeValue;

	public TextBlock mLblJingLingSumDamage;

	public TextBlock mLblJingLingBaseDamageValue;

	public TextBlock mLblJingLingRateDamageValue;

	public TextBlock mLblJingLingZuHe;

	public TextBlock mLblJingLingZuHeDamage;

	public UISprite mJingLingKillStatus;

	public Modal3DShow m3DModelParent;

	public ListBox mTaoFaAwards;

	private ObservableCollection _TaoFaItemCollection;

	public ListBox mJingLingZhenRongListBox;

	private ObservableCollection _JingLingItemCollection;

	public ListBox mJingLingZuHeListBox;

	private ObservableCollection _JingLingZuHeCollection;

	public TextBlock mLblLifeValue;

	public UISprite mImgLife;

	public GameObject mFreeFightObj;

	public GameObject mFreeLblFightObj;

	public TextBlock mLblFreeFight;

	public TextBlock mLblFreeFightTimes;

	public GameObject mCostFightObj;

	public TextBlock mLblCostFight;

	public TextBlock mLblCostFightValue;

	public ShowNetImage m_ShowFanBei;

	public GameObject mBeiShuObj;

	public UILabel mBeiShuNumber;

	private GameObject effectBeiShuObj;

	private List<string> mCallBossAwards = new List<string>();

	private List<string> mTaoFaAwardList = new List<string>();

	private YaoSaiBossInfoPart.EFightStatus mFightStatus;

	private int BossOwnerID;

	private Dictionary<int, int> mJingLingPositionDict = new Dictionary<int, int>();

	private Vector3 bossLifeBarLength = new Vector3(292f, 12f, 1f);

	private string mCacheJingLingZuHe_Str;

	private string ShaderColored = "Unlit/Transparent Colored";

	private string ShaderGray = "Unlit/Gray";

	public GameObject mHelpWindow;

	public UIButton mBtnHelpClose;

	public UILabel mLblHelpContent;

	private string mXieZhuHelpContent;

	private string mSelfHelpContent;

	private float CDTime = 1f;

	private bool isClick = true;

	private bool isSelfBoss;

	private bool isFirstPayment = true;

	private int mCostDiamondNum;

	private bool isFightXieZhu;

	private float mBossLeftHPPercent;

	public UILabel JingLingZhenRong_Label;

	private int mFreeFightNum;

	private MonsterNPCResLoader resLoader;

	private int secondsCountDown;

	private GChildWindow mYaoSaiJingLingBattleRecordPartWindow;

	private YaoSaiJingLingBattleRecordPart mYaoSaiJingLingBattleRecordPart;

	private enum EBossStatus
	{
		None,
		HasLife,
		Dead,
		TimeOut
	}

	private enum EFightStatus
	{
		None,
		Self,
		XieZhu,
		Award
	}
}
