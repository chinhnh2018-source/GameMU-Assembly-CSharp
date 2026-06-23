using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiGloryhallPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.mOwnPerformanceRoot.SetActive(false);
		this.mObjTeamOrOwnDataRoot.SetActive(false);
		this.mObjTeamOrOwnEnterBtns.SetActive(false);
		this.mObjTeamOrOwnEnterBtns.SetActive(true);
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendZhanMengLianSaiGetRankOInfoMini();
		GameInstance.Game.SendZhanMengLianSaiGetMilitaryMes();
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("荣耀殿堂")
			});
			this.mBtnTeamOrOwn.Label.text = Global.GetLang("返回");
			for (int i = 0; i < this.mLabLiansaiType.Length; i++)
			{
				this.mLabLiansaiType[i].lineWidth = 210;
			}
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
		try
		{
			this.mIamgeTeamOrOwnQiZibg.URL = "NetImages/GameRes/Images/Plate/LianSaiQiZi.png";
			this.mIamgeTeamOrOwnbg2.URL = "NetImages/GameRes/Images/Plate/LianSaiMyGlory.png";
			this.mLiftImage.URL = "NetImages/GameRes/Images/Plate/LianSaiMyGlory.png";
			this.mRightImage.URL = "NetImages/GameRes/Images/Plate/LianSaiMyGlory.png";
		}
		catch (Exception ex)
		{
		}
	}

	private void InitHandler()
	{
		try
		{
			ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass teamOrOwnBtnsBtnClass = new ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass(this.mObjTeamOrOwnBtn1, 0);
			ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass teamOrOwnBtnsBtnClass2 = new ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass(this.mObjTeamOrOwnBtn2, 1);
			ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass teamOrOwnBtnsBtnClass3 = new ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass(this.mObjTeamOrOwnBtn3, 2);
			ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass teamOrOwnBtnsBtnClass4 = new ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass(this.mObjTeamOrOwnBtn4, 3);
			teamOrOwnBtnsBtnClass.Hander = new DPSelectedItemEventHandler(this.TeamOrOwnBtnsBtnClick);
			this.mTeamOrOwnBtnsBtnClassDic.Add(0, teamOrOwnBtnsBtnClass);
			this.mTeamOrOwnBtnsBtnClassDic.Add(4, teamOrOwnBtnsBtnClass);
			teamOrOwnBtnsBtnClass2.Hander = new DPSelectedItemEventHandler(this.TeamOrOwnBtnsBtnClick);
			this.mTeamOrOwnBtnsBtnClassDic.Add(1, teamOrOwnBtnsBtnClass2);
			this.mTeamOrOwnBtnsBtnClassDic.Add(5, teamOrOwnBtnsBtnClass2);
			teamOrOwnBtnsBtnClass3.Hander = new DPSelectedItemEventHandler(this.TeamOrOwnBtnsBtnClick);
			this.mTeamOrOwnBtnsBtnClassDic.Add(2, teamOrOwnBtnsBtnClass3);
			this.mTeamOrOwnBtnsBtnClassDic.Add(6, teamOrOwnBtnsBtnClass3);
			teamOrOwnBtnsBtnClass4.Hander = new DPSelectedItemEventHandler(this.TeamOrOwnBtnsBtnClick);
			this.mTeamOrOwnBtnsBtnClassDic.Add(3, teamOrOwnBtnsBtnClass4);
			this.mTeamOrOwnBtnsBtnClassDic.Add(7, teamOrOwnBtnsBtnClass4);
			ZhanMengLianSaiGloryhallPart.TopBtnTabClass topBtnTabClass = new ZhanMengLianSaiGloryhallPart.TopBtnTabClass(this.mSpTeamInfBtnSp, this.mLabelTeamInfBtn, 1);
			topBtnTabClass.RefreshInf(Global.GetLang("战盟数据"));
			topBtnTabClass.Hander = new DPSelectedItemEventHandler(this.TopBtnTabClassClick);
			topBtnTabClass.BSelect = true;
			this.mlistTopBtnTabClass.Add(topBtnTabClass);
			ZhanMengLianSaiGloryhallPart.TopBtnTabClass topBtnTabClass2 = new ZhanMengLianSaiGloryhallPart.TopBtnTabClass(this.mSpOweinfBtnSp, this.mLabelOweinfBtn, 2);
			topBtnTabClass2.RefreshInf(Global.GetLang("个人数据"));
			topBtnTabClass2.Hander = new DPSelectedItemEventHandler(this.TopBtnTabClassClick);
			topBtnTabClass2.BSelect = false;
			this.mlistTopBtnTabClass.Add(topBtnTabClass2);
			ZhanMengLianSaiGloryhallPart.TopBtnTabClass topBtnTabClass3 = new ZhanMengLianSaiGloryhallPart.TopBtnTabClass(this.mSpOweRloryInfBtnSp, this.mLabelOweRloryInf, 3);
			topBtnTabClass3.RefreshInf(Global.GetLang("我的成就"));
			topBtnTabClass3.Hander = new DPSelectedItemEventHandler(this.TopBtnTabClassClick);
			topBtnTabClass3.BSelect = false;
			this.mlistTopBtnTabClass.Add(topBtnTabClass3);
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this.mBtnTeamOrOwn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.mObjTeamOrOwnDataRoot.SetActive(false);
				this.mObjTeamOrOwnEnterBtns.SetActive(true);
			};
			this.mObservableCollectionTeamOrOwnRank = this.mListBoxTeamOrOwnRank_.ItemsSource;
			this.mObservableCollectionZhanMengGrade = this.mOwnGradeListBox.ItemsSource;
			this.mObservableCollectionOwnGrade = this.mZhanMengListBox.ItemsSource;
		}
		catch (Exception ex)
		{
		}
	}

	private void TopBtnTabClassClick(object sender, DPSelectedItemEventArgs args)
	{
		int i = 1;
		int num = this.mlistTopBtnTabClass.Count + 1;
		while (i < num)
		{
			if (i == args.ID)
			{
				this.mlistTopBtnTabClass[i - 1].BSelect = true;
			}
			else
			{
				this.mlistTopBtnTabClass[i - 1].BSelect = false;
			}
			i++;
		}
		this.RefreshBtnClick();
	}

	private void RefreshBtnClick()
	{
		int num = 0;
		int i = 1;
		int num2 = this.mlistTopBtnTabClass.Count + 1;
		while (i < num2)
		{
			if (this.mlistTopBtnTabClass[i - 1].BSelect)
			{
				this.mlistTopBtnTabClass[i - 1].BSelect = true;
				num = i;
			}
			i++;
		}
		this.mTeamOrOwn = (ZhanMengLianSaiGloryhallPart.TeamOrOwn)num;
		if (this.mTeamOrOwn == ZhanMengLianSaiGloryhallPart.TeamOrOwn.Glory)
		{
			GameInstance.Game.SendZhanMengLianSaiGetMilitaryMes();
		}
		if (3 > num)
		{
			this.mOwnPerformanceRoot.SetActive(false);
			this.mObjTeamOrOwnDataRoot.SetActive(false);
			this.mObjTeamOrOwnEnterBtns.SetActive(true);
			this.ChangeTeamOrOwnBtnsBtnClassState(this.mTeamOrOwn);
		}
		else
		{
			this.mObjTeamOrOwnDataRoot.SetActive(false);
			this.mObjTeamOrOwnEnterBtns.SetActive(false);
			this.mOwnPerformanceRoot.SetActive(true);
			this.RfreshAnalysisView();
		}
	}

	private void TeamOrOwnBtnsBtnClick(object sender, DPSelectedItemEventArgs args)
	{
		this.mObjTeamOrOwnDataRoot.SetActive(true);
		this.mObjTeamOrOwnEnterBtns.SetActive(false);
		BangHuiMatchRankType bangHuiMatchRankType;
		if (this.mTeamOrOwn == ZhanMengLianSaiGloryhallPart.TeamOrOwn.Team)
		{
			bangHuiMatchRankType = args.ID;
		}
		else
		{
			bangHuiMatchRankType = args.ID + 4;
		}
		this.RefrshTeamOrOwnData(bangHuiMatchRankType);
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetZhanMengLianSaiRankInfo(bangHuiMatchRankType);
		this.RefreshTeamOrOwnRankTitle(bangHuiMatchRankType);
	}

	private ZhanMengLianSaiGloryhallItem GetZhanMengLianSaiGloryhallItem(int index)
	{
		ZhanMengLianSaiGloryhallItem zhanMengLianSaiGloryhallItem = null;
		GameObject at = this.mObservableCollectionTeamOrOwnRank.GetAt(index);
		if (null != at)
		{
			zhanMengLianSaiGloryhallItem = at.GetComponent<ZhanMengLianSaiGloryhallItem>();
			if (null == zhanMengLianSaiGloryhallItem)
			{
				Object.Destroy(at);
			}
		}
		if (null == zhanMengLianSaiGloryhallItem)
		{
			zhanMengLianSaiGloryhallItem = U3DUtils.NEW<ZhanMengLianSaiGloryhallItem>();
			this.mObservableCollectionTeamOrOwnRank.AddNoUpdate(zhanMengLianSaiGloryhallItem);
		}
		zhanMengLianSaiGloryhallItem.gameObject.SetActive(true);
		return zhanMengLianSaiGloryhallItem;
	}

	private void RefrshTeamOrOwnData(BangHuiMatchRankType index)
	{
	}

	private void RefreshRankDataView(List<BangHuiMatchRankInfo> RankList)
	{
		if (RankList != null && 0 < RankList.Count)
		{
			for (int i = 0; i < RankList.Count; i++)
			{
				BangHuiMatchRankInfo bangHuiMatchRankInfo = RankList[i];
				if (bangHuiMatchRankInfo != null)
				{
					ZhanMengLianSaiGloryhallItem zhanMengLianSaiGloryhallItem = this.GetZhanMengLianSaiGloryhallItem(i);
					zhanMengLianSaiGloryhallItem.RefreshInf(i + 1, bangHuiMatchRankInfo.Param1, bangHuiMatchRankInfo.Value.ToString());
					zhanMengLianSaiGloryhallItem.DragPanel = this.mDragPanelTeamOrOwnRankView;
				}
			}
			if (this.mObservableCollectionTeamOrOwnRank.Count > RankList.Count)
			{
				for (int j = RankList.Count; j < this.mObservableCollectionTeamOrOwnRank.Count; j++)
				{
					GameObject at = this.mObservableCollectionTeamOrOwnRank.GetAt(j);
					if (null != at)
					{
						at.SetActive(false);
					}
				}
			}
			this.mListBoxTeamOrOwnRank_.repositionNow = true;
			this.mDragPanelTeamOrOwnRankView.ResetPosition();
			SpringPanel.Begin(this.mDragPanelTeamOrOwnRankView.gameObject, new Vector3(-12f, -69f, 0f), 6f);
			Transform transform = this.mDragPanelTeamOrOwnRankView.transform.FindChild("Label");
			if (null != transform)
			{
				transform.gameObject.SetActive(false);
			}
		}
		else
		{
			Transform transform2 = this.mDragPanelTeamOrOwnRankView.transform.FindChild("Label");
			if (null != transform2)
			{
				UILabel component = transform2.GetComponent<UILabel>();
				transform2.gameObject.SetActive(true);
				if (null != component)
				{
					component.text = Global.GetLang("暂无数据");
				}
				else
				{
					Object.Destroy(transform2.gameObject);
				}
			}
			if (0 < this.mObservableCollectionTeamOrOwnRank.Count)
			{
				for (int k = 0; k < this.mObservableCollectionTeamOrOwnRank.Count; k++)
				{
					GameObject at2 = this.mObservableCollectionTeamOrOwnRank.GetAt(k);
					if (null != at2)
					{
						at2.SetActive(false);
					}
				}
			}
		}
	}

	private string[] GetTemaOrOwnInfString(BangHuiMatchRankType type)
	{
		string[] array = new string[]
		{
			string.Empty,
			string.Empty
		};
		switch (type)
		{
		case 0:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("黄金联赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("上届冠军")
			});
			break;
		case 1:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("新星赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("上届冠军")
			});
			break;
		case 2:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("黄金联赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("冠军次数最多")
			});
			break;
		case 3:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("新星赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("冠军次数最多")
			});
			break;
		case 4:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("黄金联赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("上届MVP之王")
			});
			break;
		case 5:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("新星赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("上届MVP之王")
			});
			break;
		case 6:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("黄金联赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("获得MVP最多")
			});
			break;
		case 7:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("新星赛")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("获得MVP最多")
			});
			break;
		}
		return array;
	}

	private string GetContentBtnString(BangHuiMatchRankType type)
	{
		switch (type)
		{
		case 0:
			return Global.GetLang("上届黄金联赛冠军");
		case 1:
			return Global.GetLang("上届新星赛冠军");
		case 2:
			return Global.GetLang("黄金联赛冠军次数最多");
		case 3:
			return Global.GetLang("新星赛冠军次数最多");
		case 4:
			return Global.GetLang("上届黄金联赛MVP之王");
		case 5:
			return Global.GetLang("上届新星赛MVP之王");
		case 6:
			return Global.GetLang("黄金联赛获得MVP最多");
		case 7:
			return Global.GetLang("新星赛获得MVP最多");
		}
		return string.Empty;
	}

	private string[] GetAnalysisString(BangHuiAnalysisType type)
	{
		string[] array = new string[]
		{
			string.Empty,
			string.Empty
		};
		switch (type)
		{
		case 0:
			array[0] = Global.GetLang("黄金联赛冠军:");
			array[1] = Global.GetLang("次");
			break;
		case 1:
			array[0] = Global.GetLang("黄金联赛最好成绩:");
			break;
		case 2:
			array[0] = Global.GetLang("黄金联赛MVP:");
			array[1] = Global.GetLang("次");
			break;
		case 3:
			array[0] = Global.GetLang("黄金联赛MVP次数排名:");
			break;
		case 4:
			array[0] = Global.GetLang("新星赛冠军:");
			array[1] = Global.GetLang("次");
			break;
		case 5:
			array[0] = Global.GetLang("新星赛最好成就:");
			break;
		case 6:
			array[0] = Global.GetLang("新星赛MVP:");
			array[1] = Global.GetLang("次");
			break;
		case 7:
			array[0] = Global.GetLang("新星赛MVP次数排名:");
			break;
		case 8:
			array[0] = Global.GetLang("本赛季击杀数:");
			break;
		case 9:
			array[0] = Global.GetLang("总击杀数:");
			break;
		case 10:
			array[0] = Global.GetLang("胜场数:");
			break;
		case 11:
			array[0] = Global.GetLang("比赛胜率:");
			break;
		case 12:
			array[0] = Global.GetLang("黄金联赛冠军:");
			array[1] = Global.GetLang("次");
			break;
		case 13:
			array[0] = Global.GetLang("黄金联赛冠军次数排名:");
			break;
		case 14:
			array[0] = Global.GetLang("黄金联赛最好成绩:");
			break;
		case 15:
			array[0] = Global.GetLang("黄金联赛总胜场数:");
			break;
		case 16:
			array[0] = Global.GetLang("黄金联赛总胜率:");
			break;
		case 17:
			array[0] = Global.GetLang("新星联赛冠军:");
			array[1] = Global.GetLang("次");
			break;
		case 18:
			array[0] = Global.GetLang("新星联赛冠军次数排名:");
			break;
		case 19:
			array[0] = Global.GetLang("新星联赛最好成绩:");
			break;
		case 20:
			array[0] = Global.GetLang("新星联赛总胜场数:");
			break;
		case 21:
			array[0] = Global.GetLang("黄金联赛总胜率:");
			break;
		case 22:
			array[0] = Global.GetLang("总击杀数:");
			break;
		case 23:
			array[0] = Global.GetLang("积分落后胜利:");
			array[1] = Global.GetLang("次");
			break;
		}
		array[0] = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			array[0]
		});
		array[1] = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			array[1]
		});
		return array;
	}

	private void ChangeTeamOrOwnBtnsBtnClassState(ZhanMengLianSaiGloryhallPart.TeamOrOwn type)
	{
		int i = 0;
		int num = 0;
		if (type == ZhanMengLianSaiGloryhallPart.TeamOrOwn.Team)
		{
			i = 0;
			num = 3;
		}
		else if (type == ZhanMengLianSaiGloryhallPart.TeamOrOwn.Own)
		{
			i = 4;
			num = 7;
		}
		while (i <= num)
		{
			BangHuiMatchRankType bangHuiMatchRankType = i;
			if (this.mBangHuiMatchRankInfoMiniDic.ContainsKey(bangHuiMatchRankType))
			{
				if (i < 2)
				{
					this.mTeamOrOwnBtnsBtnClassDic[bangHuiMatchRankType].RefreshInf(bangHuiMatchRankType.ToString(), this.GetContentBtnString(bangHuiMatchRankType), this.mBangHuiMatchRankInfoMiniDic[bangHuiMatchRankType].Param1, string.Empty);
				}
				else
				{
					this.mTeamOrOwnBtnsBtnClassDic[bangHuiMatchRankType].RefreshInf(bangHuiMatchRankType.ToString(), this.GetContentBtnString(bangHuiMatchRankType), this.mBangHuiMatchRankInfoMiniDic[bangHuiMatchRankType].Param1, this.mBangHuiMatchRankInfoMiniDic[bangHuiMatchRankType].Value.ToString());
				}
			}
			else
			{
				MUDebug.LogError<BangHuiMatchRankType>(new BangHuiMatchRankType[]
				{
					bangHuiMatchRankType
				});
			}
			i++;
		}
	}

	private void RefreshTeamOrOwnRankTitle(BangHuiMatchRankType type)
	{
		string lang = Global.GetLang("排名");
		string lang2 = Global.GetLang("战盟");
		string lang3 = Global.GetLang("胜场数");
		Vector3 one = Vector3.one;
		switch (type)
		{
		case 0:
			one..ctor(192f, 30f, 0f);
			break;
		case 1:
			one..ctor(170f, 30f, 0f);
			lang3 = Global.GetLang("胜利积分");
			break;
		case 2:
			one..ctor(236f, 30f, 0f);
			lang3 = Global.GetLang("次数");
			break;
		case 3:
			one..ctor(216f, 30f, 0f);
			lang3 = Global.GetLang("次数");
			break;
		case 4:
			one..ctor(296f, 30f, 0f);
			lang2 = Global.GetLang("玩家名");
			lang3 = Global.GetLang("次数");
			break;
		case 5:
			one..ctor(270f, 30f, 0f);
			lang2 = Global.GetLang("玩家名");
			lang3 = Global.GetLang("次数");
			break;
		case 6:
			one..ctor(290f, 30f, 0f);
			lang2 = Global.GetLang("玩家名");
			lang3 = Global.GetLang("次数");
			break;
		case 7:
			one..ctor(270f, 30f, 0f);
			lang2 = Global.GetLang("玩家名");
			lang3 = Global.GetLang("次数");
			break;
		}
		this.mLabelTeamOrOwnRankTitle0.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang(lang)
		});
		this.mLabelTeamOrOwnRankTitle1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang(lang2)
		});
		this.mLabelTeamOrOwnRankTitle2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang(lang3)
		});
		this.mIamgeTeamOrOwnIcon.URL = "NetImages/GameRes/Images/Plate/" + type.ToString() + ".png";
		this.mIamgeTeamOrOwnIcon.ImageDownloaded = delegate(object s)
		{
			this.mIamgeTeamOrOwnIcon.transform.localScale = new Vector3((float)this.mIamgeTeamOrOwnIcon.ItsSizeWidth, (float)this.mIamgeTeamOrOwnIcon.ItsSizeHeight, 0f);
		};
		this.mLabelTeamOrOwnRankTypeTitle.spriteName = type.ToString();
		this.mLabelTeamOrOwnRankTypeTitle.transform.localScale = one;
		if (this.mBangHuiMatchRankInfoMiniDic.ContainsKey(type))
		{
			this.mLabelTeamOrOwnBangHuiName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this.mBangHuiMatchRankInfoMiniDic[type].Param1
			});
			this.mLabelTeamOrOwnMunber.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetColorStringForNGUIText(new object[]
				{
					"ffcc19",
					Global.GetLang(lang3) + Global.GetLang("：")
				}) + this.mBangHuiMatchRankInfoMiniDic[type].Value
			});
		}
		else
		{
			this.mLabelTeamOrOwnBangHuiName.text = string.Empty;
		}
		this.mLabelTeamOrOwnType.text = this.GetTemaOrOwnInfString(type)[0];
		this.mLabelTeamOrOwnLianSaiTypr.text = this.GetTemaOrOwnInfString(type)[1];
	}

	private void RfreshAnalysisView()
	{
		if (this.mOwnPerformanceRoot.activeSelf)
		{
			byte b = 0;
			for (int i = 0; i < 24; i++)
			{
				BangHuiAnalysisType bangHuiAnalysisType = 24;
				BangHuiAnalysisType bangHuiAnalysisType2 = i;
				if (this.GetBangHuiAnalysisTypeShowType(bangHuiAnalysisType2, out bangHuiAnalysisType) == 3)
				{
					this.DisEnabelItems(bangHuiAnalysisType2);
				}
				else
				{
					int num = 0;
					if (this.mAnalysisDic.TryGetValue(bangHuiAnalysisType2, ref num))
					{
						if (0 >= num && (this.GetBangHuiAnalysisTypeShowType(bangHuiAnalysisType2, out bangHuiAnalysisType) == 1 || this.GetBangHuiAnalysisTypeShowType(bangHuiAnalysisType2, out bangHuiAnalysisType) == 2))
						{
							this.DisEnabelItems(bangHuiAnalysisType2);
						}
						else if (this.GetBangHuiAnalysisTypeShowType(bangHuiAnalysisType2, out bangHuiAnalysisType) == 2 && this.mAnalysisDic.ContainsKey(bangHuiAnalysisType) && this.mAnalysisDic[bangHuiAnalysisType] > 0)
						{
							this.DisEnabelItems(bangHuiAnalysisType2);
						}
						else
						{
							string[] analysisString = this.GetAnalysisString(bangHuiAnalysisType2);
							ZhanMengLianSaiGloryhallItemOwn zhanMengLianSaiGloryhallItemOwn = this.GetZhanMengLianSaiGloryhallItemOwn(bangHuiAnalysisType2);
							if (null != zhanMengLianSaiGloryhallItemOwn)
							{
								if (b == 0)
								{
									zhanMengLianSaiGloryhallItemOwn.ObjBgShow = true;
									b = 1;
								}
								else
								{
									zhanMengLianSaiGloryhallItemOwn.ObjBgShow = false;
									b = 0;
								}
								if (bangHuiAnalysisType2 == 2 || bangHuiAnalysisType2 == 6 || bangHuiAnalysisType2 == 12 || bangHuiAnalysisType2 == 17)
								{
									int rank = 0;
									if (this.mAnalysisDic.TryGetValue(i + 1, ref rank))
									{
										zhanMengLianSaiGloryhallItemOwn.LabelInfStr = analysisString[0] + Global.GetColorStringForNGUIText(new object[]
										{
											"fdf7dd",
											num.ToString()
										}) + analysisString[1];
										zhanMengLianSaiGloryhallItemOwn.Rank = rank;
									}
								}
								else
								{
									zhanMengLianSaiGloryhallItemOwn.LabelInfStr = analysisString[0] + num.ToString() + analysisString[1];
								}
							}
						}
					}
					else
					{
						this.DisEnabelItems(bangHuiAnalysisType2);
					}
				}
			}
			this.mOwnGradeListBox.repositionNow = true;
			this.mZhanMengListBox.repositionNow = true;
		}
	}

	private void DisEnabelItems(BangHuiAnalysisType TypeName)
	{
		ZhanMengLianSaiGloryhallItemOwn zhanMengLianSaiGloryhallItemOwn = null;
		GameObject gameObject = this.mOwnGradeListBox.FindName(TypeName.ToString());
		if (null != gameObject)
		{
			zhanMengLianSaiGloryhallItemOwn = gameObject.GetComponent<ZhanMengLianSaiGloryhallItemOwn>();
			if (null == zhanMengLianSaiGloryhallItemOwn)
			{
				Object.Destroy(gameObject);
			}
		}
		GameObject gameObject2 = this.mZhanMengListBox.FindName(TypeName.ToString());
		if (null != gameObject2)
		{
			zhanMengLianSaiGloryhallItemOwn = gameObject2.GetComponent<ZhanMengLianSaiGloryhallItemOwn>();
			if (null == zhanMengLianSaiGloryhallItemOwn)
			{
				Object.Destroy(gameObject2);
			}
		}
		if (null != zhanMengLianSaiGloryhallItemOwn)
		{
			zhanMengLianSaiGloryhallItemOwn.gameObject.SetActive(false);
		}
	}

	private ZhanMengLianSaiGloryhallItemOwn GetZhanMengLianSaiGloryhallItemOwn(BangHuiAnalysisType TypeName)
	{
		ZhanMengLianSaiGloryhallItemOwn zhanMengLianSaiGloryhallItemOwn = null;
		GameObject gameObject = this.mOwnGradeListBox.FindName(TypeName.ToString());
		if (null != gameObject)
		{
			zhanMengLianSaiGloryhallItemOwn = gameObject.GetComponent<ZhanMengLianSaiGloryhallItemOwn>();
			if (null == zhanMengLianSaiGloryhallItemOwn)
			{
				Object.Destroy(gameObject);
			}
		}
		GameObject gameObject2 = this.mZhanMengListBox.FindName(TypeName.ToString());
		if (null != gameObject2)
		{
			zhanMengLianSaiGloryhallItemOwn = gameObject2.GetComponent<ZhanMengLianSaiGloryhallItemOwn>();
			if (null == zhanMengLianSaiGloryhallItemOwn)
			{
				Object.Destroy(gameObject2);
			}
		}
		if (null == zhanMengLianSaiGloryhallItemOwn)
		{
			zhanMengLianSaiGloryhallItemOwn = U3DUtils.NEW<ZhanMengLianSaiGloryhallItemOwn>();
			zhanMengLianSaiGloryhallItemOwn.name = TypeName.ToString();
			if (TypeName < 12)
			{
				this.mObservableCollectionZhanMengGrade.AddNoUpdate(zhanMengLianSaiGloryhallItemOwn);
			}
			else
			{
				this.mObservableCollectionOwnGrade.AddNoUpdate(zhanMengLianSaiGloryhallItemOwn);
			}
		}
		zhanMengLianSaiGloryhallItemOwn.gameObject.SetActive(true);
		return zhanMengLianSaiGloryhallItemOwn;
	}

	private void RefreshRankBtnsInf()
	{
		try
		{
			int i = 0;
			int num = 0;
			if (this.mTeamOrOwn == ZhanMengLianSaiGloryhallPart.TeamOrOwn.Team)
			{
				i = 0;
				num = 3;
			}
			else if (this.mTeamOrOwn == ZhanMengLianSaiGloryhallPart.TeamOrOwn.Own)
			{
				i = 4;
				num = 7;
			}
			while (i <= num)
			{
				BangHuiMatchRankType bangHuiMatchRankType = i;
				if (this.mBangHuiMatchRankInfoMiniDic.ContainsKey(bangHuiMatchRankType))
				{
					if (bangHuiMatchRankType == 1 || bangHuiMatchRankType == null)
					{
						this.mTeamOrOwnBtnsBtnClassDic[bangHuiMatchRankType].RefreshInf(bangHuiMatchRankType.ToString(), this.GetContentBtnString(bangHuiMatchRankType), this.mBangHuiMatchRankInfoMiniDic[bangHuiMatchRankType].Param1, string.Empty);
					}
					else
					{
						this.mTeamOrOwnBtnsBtnClassDic[bangHuiMatchRankType].RefreshInf(bangHuiMatchRankType.ToString(), this.GetContentBtnString(bangHuiMatchRankType), this.mBangHuiMatchRankInfoMiniDic[bangHuiMatchRankType].Param1, this.mBangHuiMatchRankInfoMiniDic[bangHuiMatchRankType].Value.ToString());
					}
				}
				i++;
			}
		}
		catch (Exception ex)
		{
		}
	}

	private byte GetBangHuiAnalysisTypeShowType(BangHuiAnalysisType type, out BangHuiAnalysisType LastType)
	{
		LastType = 24;
		switch (type)
		{
		case 0:
			return 1;
		case 1:
			LastType = 0;
			return 2;
		case 2:
			return 1;
		case 3:
			return 3;
		case 4:
			return 1;
		case 5:
			LastType = 4;
			return 2;
		case 6:
			return 1;
		case 7:
			return 3;
		case 8:
			return 0;
		case 9:
			return 0;
		case 10:
			return 0;
		case 11:
			return 3;
		case 12:
			return 1;
		case 13:
			return 3;
		case 14:
			LastType = 14;
			return 2;
		case 15:
			return 0;
		case 16:
			return 3;
		case 17:
			return 1;
		case 18:
			return 3;
		case 19:
			LastType = 18;
			return 2;
		case 20:
			return 0;
		case 21:
			return 1;
		case 22:
			return 0;
		case 23:
			return 0;
		case 24:
			return 3;
		default:
			return 3;
		}
	}

	public void NotifyGetLianSaiAnalysisCallBack(List<int> dataList)
	{
		this.mAnalysisDic.Clear();
		if (dataList != null && 0 < dataList.Count)
		{
			int i = 0;
			int count = dataList.Count;
			while (i < count)
			{
				this.mAnalysisDic[i] = dataList[i];
				i++;
			}
		}
		else
		{
			int j = 0;
			int num = 24;
			while (j < num)
			{
				this.mAnalysisDic[j] = 0;
				j++;
			}
		}
		this.RfreshAnalysisView();
	}

	public void NoticeGetSaiJiGetRankInfMiniCallBack(List<BangHuiMatchRankInfo> Datalist)
	{
		this.mBangHuiMatchRankInfoMiniDic.Clear();
		if (Datalist != null)
		{
			for (int i = 0; i < Datalist.Count; i++)
			{
				this.mBangHuiMatchRankInfoMiniDic.Add(i, Datalist[i]);
			}
		}
		this.RefreshRankBtnsInf();
		this.RefreshBtnClick();
	}

	public void RefreshTeamOrOwnData(List<BangHuiMatchRankInfo> list)
	{
		this.RefreshRankDataView(list);
	}

	private const string mIamgeTeamOrOwnBg = "mIamgeTeamOrOwnBg";

	private const string mIamgeTeamOrOwnIcons = "mIamgeTeamOrOwnIcon";

	private const string mLabelTeamOrOwnTypes = "mLabelTeamOrOwnType";

	private const string mLabelTeamOrOwnBangHuiNames = "mLabelTeamOrOwnBangHuiName";

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UISprite mSpOweRloryInfBtnSp;

	[SerializeField]
	private UILabel mLabelOweRloryInf;

	[SerializeField]
	private UISprite mSpTeamInfBtnSp;

	[SerializeField]
	private UILabel mLabelTeamInfBtn;

	[SerializeField]
	private UISprite mSpOweinfBtnSp;

	[SerializeField]
	private UILabel mLabelOweinfBtn;

	[SerializeField]
	private GameObject mObjTeamOrOwnDataRoot;

	[SerializeField]
	private ShowNetImage mIamgeTeamOrOwnbg2;

	[SerializeField]
	private ShowNetImage mIamgeTeamOrOwnQiZibg;

	[SerializeField]
	private GButton mBtnTeamOrOwn;

	[SerializeField]
	private UILabel mLabelTeamOrOwnMunber;

	[SerializeField]
	private UILabel mLabelTeamOrOwnBangHuiName;

	[SerializeField]
	private UILabel mLabelTeamOrOwnType;

	[SerializeField]
	private UILabel mLabelTeamOrOwnLianSaiTypr;

	[SerializeField]
	private ShowNetImage mIamgeTeamOrOwnIcon;

	[SerializeField]
	private UISprite mLabelTeamOrOwnRankTypeTitle;

	[SerializeField]
	private UILabel mLabelTeamOrOwnRankTitle0;

	[SerializeField]
	private UILabel mLabelTeamOrOwnRankTitle1;

	[SerializeField]
	private UILabel mLabelTeamOrOwnRankTitle2;

	[SerializeField]
	private UIDraggablePanel mDragPanelTeamOrOwnRankView;

	[SerializeField]
	private ListBox mListBoxTeamOrOwnRank_;

	[SerializeField]
	private GameObject mObjTeamOrOwnEnterBtns;

	[SerializeField]
	private GameObject mObjTeamOrOwnBtn1;

	[SerializeField]
	private GameObject mObjTeamOrOwnBtn2;

	[SerializeField]
	private GameObject mObjTeamOrOwnBtn3;

	[SerializeField]
	private GameObject mObjTeamOrOwnBtn4;

	[SerializeField]
	private GameObject mOwnPerformanceRoot;

	[SerializeField]
	private ListBox mOwnGradeListBox;

	[SerializeField]
	private ListBox mZhanMengListBox;

	[SerializeField]
	private ShowNetImage mLiftImage;

	[SerializeField]
	private ShowNetImage mRightImage;

	[SerializeField]
	private UILabel mTitleLabel;

	[SerializeField]
	private UILabel[] mLabLiansaiType;

	private List<ZhanMengLianSaiGloryhallPart.TopBtnTabClass> mlistTopBtnTabClass = new List<ZhanMengLianSaiGloryhallPart.TopBtnTabClass>();

	private ObservableCollection mObservableCollectionTeamOrOwnRank;

	private ZhanMengLianSaiGloryhallPart.TeamOrOwn mTeamOrOwn = ZhanMengLianSaiGloryhallPart.TeamOrOwn.Team;

	private Dictionary<BangHuiMatchRankType, ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass> mTeamOrOwnBtnsBtnClassDic = new Dictionary<BangHuiMatchRankType, ZhanMengLianSaiGloryhallPart.TeamOrOwnBtnsBtnClass>();

	private Dictionary<BangHuiAnalysisType, int> mAnalysisDic = new Dictionary<BangHuiAnalysisType, int>();

	private ObservableCollection mObservableCollectionOwnGrade;

	private ObservableCollection mObservableCollectionZhanMengGrade;

	private Dictionary<BangHuiMatchRankType, BangHuiMatchRankInfo> mBangHuiMatchRankInfoMiniDic = new Dictionary<BangHuiMatchRankType, BangHuiMatchRankInfo>();

	public enum TeamOrOwn
	{
		Team = 1,
		Own,
		Glory
	}

	private class TeamOrOwnBtnsBtnClass
	{
		public TeamOrOwnBtnsBtnClass(GameObject obj, int ID)
		{
			try
			{
				this.mBgImage = obj.transform.FindChild("mIamgeTeamOrOwnBg").GetComponent<ShowNetImage>();
				this.mIconImage = obj.transform.FindChild("mIamgeTeamOrOwnIcon").GetComponent<ShowNetImage>();
				this.mTypeLabel = obj.transform.FindChild("mLabelTeamOrOwnType").GetComponent<UILabel>();
				this.mNameLabel = obj.transform.FindChild("mLabelTeamOrOwnBangHuiName").GetComponent<UILabel>();
				this.mID = ID;
				BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
				if (null == boxCollider)
				{
					boxCollider = obj.gameObject.AddComponent<BoxCollider>();
				}
				boxCollider.size = new Vector3(380f, 190f, 0f);
				UIEventListener.Get(obj).onClick = delegate(GameObject g)
				{
					if (this.mCanClisk && this.Hander != null)
					{
						this.Hander(g, new DPSelectedItemEventArgs
						{
							ID = this.mID
						});
					}
				};
				this.mBgImage.URL = "NetImages/GameRes/Images/Plate/LianSaiTeamInf.png";
				this.mBgImage.transform.localScale = new Vector3(378f, 182f, 0f);
			}
			catch (Exception ex)
			{
				MUDebug.LogError<string>(new string[]
				{
					ex.Message
				});
			}
		}

		public void RefreshInf(string IconUrl, string TypeStr, string name, string value = "")
		{
			if (!string.IsNullOrEmpty(IconUrl))
			{
				this.mIconImage.URL = "NetImages/GameRes/Images/Plate/" + IconUrl + ".png";
			}
			this.mTypeLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				TypeStr
			});
			if (string.IsNullOrEmpty(name))
			{
				this.mNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("暂无数据")
				});
				this.mCanClisk = false;
			}
			else
			{
				this.mNameLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					name + "\n" + (string.IsNullOrEmpty(value) ? string.Empty : (Global.GetLang("次数：") + value))
				});
				this.mCanClisk = true;
			}
		}

		public DPSelectedItemEventHandler Hander;

		private ShowNetImage mBgImage;

		private ShowNetImage mIconImage;

		private UILabel mTypeLabel;

		private UILabel mNameLabel;

		private int mID;

		private bool mCanClisk = true;
	}

	private class TopBtnTabClass
	{
		public TopBtnTabClass(UISprite bg, UILabel Label, int id)
		{
			ZhanMengLianSaiGloryhallPart.TopBtnTabClass <>f__this = this;
			this.mBGSp = bg;
			this.mLabel = Label;
			this.mID = id;
			BoxCollider boxCollider = bg.GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				boxCollider = bg.gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.center = new Vector3(0f, 0.5f, 0f);
			UIEventListener.Get(bg.gameObject).onClick = delegate(GameObject g)
			{
				if (<>f__this.Hander != null)
				{
					<>f__this.Hander(bg, new DPSelectedItemEventArgs
					{
						ID = <>f__this.mID
					});
				}
			};
		}

		public void RefreshInf(string LabelStr)
		{
			this.mLabel.text = LabelStr;
			this.mBtnStr = LabelStr;
			this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.mBtnStr
			});
		}

		public bool BSelect
		{
			get
			{
				return this.mBSelect;
			}
			set
			{
				this.mBSelect = value;
				if (this.mBSelect)
				{
					this.mBGSp.spriteName = "TopTabBtn_hover";
					Vector3 localPosition = this.mBGSp.transform.localPosition;
					localPosition.y = -26f;
					this.mBGSp.transform.localPosition = localPosition;
					this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						this.mBtnStr
					});
				}
				else
				{
					this.mBGSp.spriteName = "TopTabBtn_normal";
					Vector3 localPosition2 = this.mBGSp.transform.localPosition;
					localPosition2.y = -30f;
					this.mBGSp.transform.localPosition = localPosition2;
					this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						this.mBtnStr
					});
				}
			}
		}

		public DPSelectedItemEventHandler Hander;

		private UISprite mBGSp;

		private UILabel mLabel;

		private int mID;

		private string mBtnStr = string.Empty;

		private bool mBSelect;
	}
}
