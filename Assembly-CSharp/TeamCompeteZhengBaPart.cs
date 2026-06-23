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

public class TeamCompeteZhengBaPart : UserControl
{
	public TeamCompeteZhengBaPart()
	{
		List<int> list = new List<int>();
		list.Add(0);
		list.Add(1);
		list.Add(2);
		list.Add(3);
		this.Eight = list;
		list = new List<int>();
		list.Add(4);
		list.Add(5);
		this.Four = list;
		list = new List<int>();
		list.Add(6);
		this.Two = list;
		list = new List<int>();
		list.Add(7);
		this.One = list;
		this.dataInfos = new List<ZhanDuiZhengBaZhanDuiData>();
		this.grayColor = "#808081";
		this.red = "zdzb_xinxi_hong";
		this.blue = "zdzb_xinxi_lan";
		this.gray = "zdzb_xinxi_hui";
		this.mTimePoints = new List<string[]>();
		this.mJinChengID = new List<int>();
		this.mCurrentMatchJingCheng = 64;
		this.mNextMatchJingCheng = -1;
		this.checkDateTime = default(DateTime);
		this.nowCheckTime = default(DateTime);
		this.mServerMatchDataDict = new Dictionary<int, ZhanDuiZhengBaZhanDuiData>();
		base..ctor();
	}

	private bool IsHighLightRongYaoBg
	{
		set
		{
			this.rongYaoBg.shader = ((!value) ? Shader.Find("Unlit/Gray") : Shader.Find("Unlit/Transparent Colored"));
		}
	}

	private int GetDictIndex(int value)
	{
		return (value - 1) / 4 + 1;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitBtnAndPos();
		this.InitEvent();
		this.RequestMainInfoData();
		base.InvokeRepeating("RequestMainInfoFlag", 5f, 5f);
		this.LblMoney.Text = Global.Data.roleData.MoneyData[162].ToString();
		TeamCompeteDataManager.RefreshJingCaiDianCallBack = delegate()
		{
			this.LblMoney.Text = Global.Data.roleData.MoneyData[162].ToString();
		};
	}

	private void RequestMainInfoFlag()
	{
		this.RequestMainInfoData();
	}

	private void InitTextInPrefabs()
	{
		this.LblJinCheng.Text = Global.GetLang("比赛进程");
		this.LblTime.Text = Global.GetLang("比赛时间：");
		this.LblMoney.Text = "0";
		this.BtnRule.Label.text = Global.GetLang("详细规则");
		this.BtnAward.Label.text = Global.GetLang("奖励预览");
		this.BtnStatus.Label.text = Global.GetLang("参赛状态");
		this.BtnGuess.Label.text = Global.GetLang("竞      猜");
		this.BtnShop.Label.text = Global.GetLang("商      店");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnRule.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenCommonHelpWindow();
		};
		this.BtnAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenJiangLiYuLanPart();
		};
		this.BtnStatus.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RequestTeamList();
		};
		this.BtnGuess.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.RequestGuessInfo();
		};
		this.BtnShop.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenMUDuiHuanPart();
		};
		UIEventListener.Get(this.RongYaoClick).onClick = delegate(GameObject s)
		{
			this.DisplayBattleInfo(7, this.GetData());
		};
	}

	private void InitBtnAndPos()
	{
		this.leftBtns = new GButton[this.leftPos.Length];
		for (int i = 0; i < this.leftPos.Length; i++)
		{
			this.rightPos[i].localPosition = new Vector3(-this.leftPos[i].localPosition.x, this.leftPos[i].localPosition.y, this.leftPos[i].localPosition.z);
			GameObject gameObject = Object.Instantiate<GameObject>(this.BtnChaKan);
			gameObject.SetActive(true);
			gameObject.transform.SetParent(this.leftPos[i]);
			gameObject.transform.localPosition = Vector3.zero;
			GButton component = gameObject.GetComponent<GButton>();
			component.TagIndex = i;
			component.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.LeftBtnEventCallBack(s, e);
			};
			this.leftBtns[i] = component;
		}
		this.rightBtns = new GButton[this.rightPos.Length];
		for (int j = 0; j < this.rightPos.Length; j++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.BtnChaKan);
			gameObject2.SetActive(true);
			gameObject2.transform.SetParent(this.rightPos[j]);
			gameObject2.transform.localPosition = Vector3.zero;
			GButton component2 = gameObject2.GetComponent<GButton>();
			component2.TagIndex = j;
			component2.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.RightBtnEventCallBack(s, e);
			};
			this.rightBtns[j] = component2;
		}
	}

	private void LeftBtnEventCallBack(object sender, MouseEvent e)
	{
		GButton component = (sender as GameObject).GetComponent<GButton>();
		if (component != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"left " + component.TagIndex
			});
			this.DisplayBattleInfo(component.TagIndex, this.GetLeftDatas);
		}
	}

	private void RightBtnEventCallBack(object sender, MouseEvent e)
	{
		GButton component = (sender as GameObject).GetComponent<GButton>();
		if (component != null)
		{
			MUDebug.Log<string>(new string[]
			{
				"right " + component.TagIndex
			});
			this.DisplayBattleInfo(component.TagIndex, this.GetRightDatas);
		}
	}

	private List<ZhanDuiZhengBaZhanDuiData> GetLeftDatas
	{
		get
		{
			List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
			for (int i = 1; i <= 8; i++)
			{
				if (this.mServerMatchDataDict != null && this.mServerMatchDataDict.ContainsKey(i))
				{
					list.Add(this.mServerMatchDataDict[i]);
				}
				else
				{
					list.Add(null);
				}
			}
			return list;
		}
	}

	private List<ZhanDuiZhengBaZhanDuiData> GetRightDatas
	{
		get
		{
			List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
			for (int i = 9; i <= 16; i++)
			{
				if (this.mServerMatchDataDict != null && this.mServerMatchDataDict.ContainsKey(i))
				{
					list.Add(this.mServerMatchDataDict[i]);
				}
				else
				{
					list.Add(null);
				}
			}
			return list;
		}
	}

	private void InitValue()
	{
		if (this.leftBttleInfos.Count > 0)
		{
			this.leftBttleInfos.Clear();
		}
		if (this.rightBttleInfos.Count > 0)
		{
			this.rightBttleInfos.Clear();
		}
		for (int i = 1; i <= 8; i++)
		{
			if (this.mServerMatchDataDict != null && this.mServerMatchDataDict.ContainsKey(i))
			{
				this.leftBttleInfos.Add(this.mServerMatchDataDict[i]);
			}
			else
			{
				this.leftBttleInfos.Add(null);
			}
		}
		for (int j = 9; j <= 16; j++)
		{
			if (this.mServerMatchDataDict != null && this.mServerMatchDataDict.ContainsKey(j))
			{
				this.rightBttleInfos.Add(this.mServerMatchDataDict[j]);
			}
			else
			{
				this.rightBttleInfos.Add(null);
			}
		}
		bool isEqual = false;
		if (this.leftBttleInfos.Count == this.LeftRoot.childCount && this.leftBttleInfos.Count != 0 && this.LeftRoot.childCount != 0)
		{
			isEqual = true;
		}
		for (int k = 0; k < this.leftBttleInfos.Count; k++)
		{
			this.InstanceSingleInfo(this.left, k, this.leftBttleInfos[k], isEqual);
		}
		bool isEqual2 = false;
		if (this.rightBttleInfos.Count == this.RightRoot.childCount && this.rightBttleInfos.Count != 0 && this.RightRoot.childCount != 0)
		{
			isEqual2 = true;
		}
		for (int l = 0; l < this.rightBttleInfos.Count; l++)
		{
			this.InstanceSingleInfo(this.right, l, this.rightBttleInfos[l], isEqual2);
		}
	}

	private void DisplayBattleInfo(int index, List<ZhanDuiZhengBaZhanDuiData> battleInfos)
	{
		if (battleInfos == null || battleInfos.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("暂无比赛数据"), 10, 3);
			return;
		}
		List<ZhanDuiZhengBaZhanDuiData> list = battleInfos;
		switch (index)
		{
		case 0:
			list = battleInfos.GetRange(0, 2);
			break;
		case 1:
			list = battleInfos.GetRange(2, 2);
			break;
		case 2:
			list = battleInfos.GetRange(4, 2);
			break;
		case 3:
			list = battleInfos.GetRange(6, 2);
			break;
		case 4:
			list = battleInfos.GetRange(0, 4);
			break;
		case 5:
			list = battleInfos.GetRange(4, 4);
			break;
		case 6:
			list = battleInfos;
			break;
		case 7:
			list = battleInfos;
			break;
		}
		List<ZhanDuiZhengBaZhanDuiData> list2 = null;
		if (this.Eight.Contains(index))
		{
			if (list.Count == 2)
			{
				if (this.mCurrentMatchJingCheng == 8)
				{
					list2 = list;
					if (this.IsAllNull(list2))
					{
						Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
						return;
					}
				}
				else if (this.mCurrentMatchJingCheng > 8)
				{
					if (this.mNextMatchJingCheng == 8)
					{
						list2 = this.GetJinJiDatas(list);
					}
					else if (this.IsActivityOver)
					{
						list2 = this.SortAndGetTwoDatas(list);
					}
					else if (this.mNextMatchJingCheng < 0)
					{
						if (!this.IsActivityOver)
						{
							Super.HintMainText(Global.GetLang("8强比赛尚未开始"), 10, 3);
							return;
						}
						list2 = this.SortAndGetTwoDatas(list);
					}
					else
					{
						list2 = list;
						if (!this.HaveCurrentMatch(list2, 8))
						{
							Super.HintMainText(Global.GetLang("8强比赛尚未开始"), 10, 3);
							return;
						}
					}
					if (this.IsAllNull(list2))
					{
						Super.HintMainText(Global.GetLang("8强比赛尚未开始"), 10, 3);
						return;
					}
				}
				else
				{
					list2 = list;
					if (this.IsAllNull(list2))
					{
						Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
						return;
					}
				}
			}
		}
		else if (this.Four.Contains(index))
		{
			if (list.Count == 4)
			{
				if (this.mCurrentMatchJingCheng == 4)
				{
					list2 = this.SortAndGetTwoDatas(list);
					if (this.IsAllNull(list2))
					{
						Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
						return;
					}
				}
				else if (this.mCurrentMatchJingCheng > 4)
				{
					if (this.mNextMatchJingCheng == 4)
					{
						list2 = this.GetJinJiDatas(list);
					}
					else if (this.IsActivityOver)
					{
						list2 = this.SortAndGetTwoDatas(list);
					}
					else if (this.mNextMatchJingCheng < 0)
					{
						if (!this.IsActivityOver)
						{
							Super.HintMainText(Global.GetLang("4强比赛尚未开始"), 10, 3);
							return;
						}
						list2 = this.SortAndGetTwoDatas(list);
					}
					else
					{
						list2 = this.GetSplitNewDatas(list, 2);
						if (!this.HaveCurrentMatch(list2, 4))
						{
							Super.HintMainText(Global.GetLang("4强比赛尚未开始"), 10, 3);
							return;
						}
					}
					if (this.IsAllNull(list2))
					{
						Super.HintMainText(Global.GetLang("4强比赛尚未开始"), 10, 3);
						return;
					}
				}
				else
				{
					list2 = this.GetSplitNewDatas(list, 2);
					if (this.IsAllNull(list2))
					{
						Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
						return;
					}
				}
			}
		}
		else if (this.Two.Contains(index))
		{
			if (this.mCurrentMatchJingCheng == 2)
			{
				list2 = this.SortAndGetTwoDatas(list);
				if (this.IsAllNull(list2))
				{
					Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
					return;
				}
			}
			else if (this.mCurrentMatchJingCheng > 2)
			{
				if (this.mNextMatchJingCheng == 2)
				{
					list2 = this.GetJinJiDatas(list);
				}
				else if (this.IsActivityOver)
				{
					list2 = this.SortAndGetTwoDatas(list);
				}
				else if (this.mNextMatchJingCheng < 0)
				{
					if (!this.IsActivityOver)
					{
						Super.HintMainText(Global.GetLang("半决赛尚未开始"), 10, 3);
						return;
					}
					list2 = this.SortAndGetTwoDatas(list);
				}
				else
				{
					list2 = this.GetSplitNewDatas(list, 3);
					if (!this.HaveCurrentMatch(list2, 2))
					{
						Super.HintMainText(Global.GetLang("半决赛尚未开始"), 10, 3);
						return;
					}
				}
				if (this.IsAllNull(list2))
				{
					Super.HintMainText(Global.GetLang("半决赛尚未开始"), 10, 3);
					return;
				}
			}
			else
			{
				list2 = this.GetSplitNewDatas(list, 3);
				if (this.IsAllNull(list2))
				{
					Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
					return;
				}
			}
		}
		else if (this.One.Contains(index))
		{
			if (this.mCurrentMatchJingCheng == 1)
			{
				list2 = this.SortAndGetTwoDatas(list);
				if (this.IsAllNull(list2))
				{
					Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
					return;
				}
			}
			else if (this.mCurrentMatchJingCheng > 1)
			{
				if (this.mNextMatchJingCheng == 1)
				{
					list2 = this.GetJinJiDatas(list);
				}
				else if (this.IsActivityOver)
				{
					list2 = this.SortAndGetTwoDatas(list);
				}
				else if (this.mNextMatchJingCheng < 0)
				{
					if (!this.IsActivityOver)
					{
						Super.HintMainText(Global.GetLang("决赛尚未开始"), 10, 3);
						return;
					}
					list2 = this.SortAndGetTwoDatas(list);
				}
				else
				{
					list2 = this.GetSplitNewDatas(list, 8);
					if (!this.HaveCurrentMatch(list2, 1))
					{
						Super.HintMainText(Global.GetLang("决赛尚未开始"), 10, 3);
						return;
					}
				}
				if (this.IsAllNull(list2))
				{
					Super.HintMainText(Global.GetLang("决赛尚未开始"), 10, 3);
					return;
				}
			}
			else
			{
				list2 = this.GetSplitNewDatas(list, 8);
				if (this.IsAllNull(list2))
				{
					Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
					return;
				}
			}
		}
		if (list2 == null || list2.Count <= 0)
		{
			Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
			return;
		}
		if (list2 != null && list2.Count == 2 && list2[0] == null && list2[1] == null)
		{
			Super.HintMainText(Global.GetLang("无对战信息"), 10, 3);
			return;
		}
		if (list2 != null && list2.Count == 1)
		{
			return;
		}
		if (list2 != null && list2.Count == 2 && (list2[0] == null || list2[1] == null))
		{
			return;
		}
		this.DisplayTeamBattleInfo(list2);
	}

	private bool HaveCurrentMatch(List<ZhanDuiZhengBaZhanDuiData> datas, int jinDu)
	{
		bool result = false;
		if (datas == null || datas.Count <= 0)
		{
			return result;
		}
		for (int i = 0; i < datas.Count; i++)
		{
			ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = datas[i];
			if (zhanDuiZhengBaZhanDuiData != null && zhanDuiZhengBaZhanDuiData.Grade == jinDu)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private bool IsAllNull(List<ZhanDuiZhengBaZhanDuiData> newDatas)
	{
		bool result = true;
		if (newDatas != null && newDatas.Count > 0)
		{
			for (int i = 0; i < newDatas.Count; i++)
			{
				if (newDatas[i] != null)
				{
					result = false;
				}
			}
		}
		return result;
	}

	private List<ZhanDuiZhengBaZhanDuiData> GetJinJiDatas(List<ZhanDuiZhengBaZhanDuiData> datas)
	{
		if (datas == null || datas.Count <= 0)
		{
			return null;
		}
		List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
		for (int i = 0; i < datas.Count; i++)
		{
			ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = datas[i];
			if (zhanDuiZhengBaZhanDuiData != null && zhanDuiZhengBaZhanDuiData.State == 1)
			{
				list.Add(zhanDuiZhengBaZhanDuiData);
			}
		}
		return list;
	}

	private List<ZhanDuiZhengBaZhanDuiData> GetSplitNewDatas(List<ZhanDuiZhengBaZhanDuiData> datas, int index)
	{
		List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
		List<ZhanDuiZhengBaZhanDuiData> range = datas.GetRange(0, index);
		if (range != null)
		{
			range.Sort(delegate(ZhanDuiZhengBaZhanDuiData x, ZhanDuiZhengBaZhanDuiData y)
			{
				if (x == null || y == null)
				{
					return 0;
				}
				return x.Grade - y.Grade;
			});
		}
		List<ZhanDuiZhengBaZhanDuiData> range2 = datas.GetRange(index, index);
		if (range2 != null)
		{
			range2.Sort(delegate(ZhanDuiZhengBaZhanDuiData x, ZhanDuiZhengBaZhanDuiData y)
			{
				if (x == null || y == null)
				{
					return 0;
				}
				return x.Grade - y.Grade;
			});
		}
		if (range != null)
		{
			for (int i = 0; i < range.Count; i++)
			{
				if (range[i] != null && range[i].Grade > 0)
				{
					list.Add(range[i]);
					break;
				}
			}
		}
		if (range2 != null)
		{
			for (int j = 0; j < range2.Count; j++)
			{
				if (range2[j] != null && range2[j].Grade > 0)
				{
					list.Add(range2[j]);
					break;
				}
			}
		}
		return list;
	}

	private List<ZhanDuiZhengBaZhanDuiData> SortAndGetTwoDatas(List<ZhanDuiZhengBaZhanDuiData> datas)
	{
		List<ZhanDuiZhengBaZhanDuiData> list = datas.FindAll((ZhanDuiZhengBaZhanDuiData s) => s != null);
		if (list != null)
		{
			list.Sort(delegate(ZhanDuiZhengBaZhanDuiData x, ZhanDuiZhengBaZhanDuiData y)
			{
				if (x == null || y == null)
				{
					return -1;
				}
				return x.Grade - y.Grade;
			});
		}
		if (list != null)
		{
			if (list.Count == 0)
			{
				list.Add(null);
				list.Add(null);
			}
			else if (list.Count == 1)
			{
				list.Add(null);
			}
		}
		return list.GetRange(0, 2);
	}

	private List<ZhanDuiZhengBaZhanDuiData> GetNewDatas(List<ZhanDuiZhengBaZhanDuiData> datas, int grade)
	{
		List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
		for (int i = 0; i < datas.Count; i++)
		{
			ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = datas[i];
			if (zhanDuiZhengBaZhanDuiData != null && zhanDuiZhengBaZhanDuiData.Grade == grade)
			{
				list.Add(zhanDuiZhengBaZhanDuiData);
			}
		}
		return list;
	}

	private List<ZhanDuiZhengBaZhanDuiData> GetData()
	{
		List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
		list.AddRange(this.GetLeftDatas);
		list.AddRange(this.GetRightDatas);
		return list;
	}

	private void DisplayTeamBattleInfo(List<ZhanDuiZhengBaZhanDuiData> datas)
	{
		if (this.mTeamCompeteBattlePart == null)
		{
			this.OpenTeamCompeteBattlePart();
		}
		this.mTeamCompeteBattlePart.InitValue(datas);
	}

	private void InstanceSingleInfo(int whichSide, int index, ZhanDuiZhengBaZhanDuiData data, bool isEqual)
	{
		GameObject gameObject = null;
		if (isEqual)
		{
			if (whichSide == this.left)
			{
				if (index < this.LeftRoot.childCount)
				{
					gameObject = this.LeftRoot.GetChild(index).gameObject;
				}
			}
			else if (whichSide == this.right && index < this.RightRoot.childCount)
			{
				gameObject = this.RightRoot.GetChild(index).gameObject;
			}
		}
		else
		{
			gameObject = Object.Instantiate<GameObject>(this.SingleInfo);
			gameObject.SetActive(true);
			gameObject.transform.SetParent((whichSide != this.left) ? this.RightRoot : this.LeftRoot);
		}
		if (gameObject == null)
		{
			gameObject = Object.Instantiate<GameObject>(this.SingleInfo);
			gameObject.SetActive(true);
			gameObject.transform.SetParent((whichSide != this.left) ? this.RightRoot : this.LeftRoot);
		}
		Vector3 localPosition = this.SingleInfoPositions[index].localPosition;
		gameObject.transform.localPosition = new Vector3(localPosition.x * (float)((whichSide != 0) ? -1 : 1), localPosition.y, localPosition.z);
		gameObject.transform.localScale = Vector3.one;
		UISprite component = gameObject.transform.Find("Bg").GetComponent<UISprite>();
		string spriteName = this.SpriteName(index);
		component.spriteName = spriteName;
		component.transform.localEulerAngles = new Vector3(0f, (float)((whichSide != this.left) ? 180 : 0));
		TextBlock component2 = gameObject.transform.Find("LblName").GetComponent<TextBlock>();
		if (data != null)
		{
			if (data.State == 0)
			{
				component2.Text = data.ZhanDuiName.ToString();
				component.spriteName = spriteName;
			}
			else if (data.State == 1)
			{
				component2.Text = data.ZhanDuiName.ToString();
				component.spriteName = spriteName;
			}
			else if (data.State == 2)
			{
				component2.Text = data.ZhanDuiName.ToString();
				component2.textColor = Global.ParseStringColorToUint(this.grayColor);
				component.spriteName = this.gray;
			}
		}
		else
		{
			component.spriteName = spriteName;
			component2.Text = Global.GetLang("虚位以待");
			component2.textColor = Global.ParseStringColorToUint(this.grayColor);
			component.spriteName = this.gray;
		}
	}

	private string SpriteName(int index)
	{
		if (index == 0 || index == 1 || index == 4 || index == 5)
		{
			return this.red;
		}
		if (index == 2 || index == 3 || index == 6 || index == 7)
		{
			return this.blue;
		}
		return null;
	}

	public void OpenCommonHelpWindow()
	{
		if (this.mCommonHelpWindowWind != null || this.mCommonHelpWindow != null)
		{
			this.CloseCommonHelpWindow();
		}
		this.mCommonHelpWindowWind = U3DUtils.NEW<GChildWindow>();
		this.mCommonHelpWindowWind.ModalType = ChildWindowModalType.Translucent;
		this.mCommonHelpWindowWind.Modal = true;
		this.mCommonHelpWindowWind.IsShowModal = false;
		Super.InitChildWindow(this.mCommonHelpWindowWind, "mCommonHelpWindowWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mCommonHelpWindowWind);
		this.mCommonHelpWindow = U3DUtils.NEW<CommonHelpWindow>();
		this.mCommonHelpWindowWind.Body.Add(this.mCommonHelpWindow);
		this.mCommonHelpWindow.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseCommonHelpWindow();
		};
		this.mCommonHelpWindow.SetHelpInfo(IConfigbase<ConfigTeamCompete>.Instance.GetTeamCompeteHelpInfo(1).list);
	}

	private void CloseCommonHelpWindow()
	{
		if (null != this.mCommonHelpWindowWind)
		{
			Super.CloseChildWindow(base.Children, this.mCommonHelpWindowWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mCommonHelpWindowWind, true);
			this.mCommonHelpWindowWind = null;
		}
		if (null != this.mCommonHelpWindow)
		{
			this.mCommonHelpWindow.transform.parent = null;
			Object.Destroy(this.mCommonHelpWindow.gameObject);
			this.mCommonHelpWindow = null;
		}
	}

	public void OpenMUDuiHuanPart()
	{
		if (this.mMUDuiHuanPartWind != null || this.mMUDuiHuanPart != null)
		{
			this.CloseMUDuiHuanPart();
		}
		this.mMUDuiHuanPartWind = U3DUtils.NEW<GChildWindow>();
		this.mMUDuiHuanPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mMUDuiHuanPartWind.Modal = true;
		this.mMUDuiHuanPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mMUDuiHuanPartWind, "mMUDuiHuanPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mMUDuiHuanPartWind);
		this.mMUDuiHuanPart = U3DUtils.NEW<MUDuiHuanPart>();
		this.mMUDuiHuanPartWind.Body.Add(this.mMUDuiHuanPart);
		this.mMUDuiHuanPart.InitPartData(11, 0);
		this.mMUDuiHuanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseMUDuiHuanPart();
			return true;
		};
	}

	private void CloseMUDuiHuanPart()
	{
		if (null != this.mMUDuiHuanPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mMUDuiHuanPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mMUDuiHuanPartWind, true);
			this.mMUDuiHuanPartWind = null;
		}
		if (null != this.mMUDuiHuanPart)
		{
			this.mMUDuiHuanPart.transform.parent = null;
			Object.Destroy(this.mMUDuiHuanPart.gameObject);
			this.mMUDuiHuanPart = null;
		}
	}

	public void OpenJiangLiYuLanPart()
	{
		if (this.mJiangLiYuLanPartWind != null || this.mJiangLiYuLanPart != null)
		{
			this.CloseJiangLiYuLanPart();
		}
		this.initXmlData();
		this.mJiangLiYuLanPartWind = U3DUtils.NEW<GChildWindow>();
		this.mJiangLiYuLanPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mJiangLiYuLanPartWind.Modal = true;
		this.mJiangLiYuLanPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mJiangLiYuLanPartWind, "mJiangLiYuLanPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mJiangLiYuLanPartWind);
		this.mJiangLiYuLanPart = U3DUtils.NEW<JiangLiYuLanPart>();
		this.mJiangLiYuLanPartWind.Body.Add(this.mJiangLiYuLanPart);
		this.mJiangLiYuLanPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseJiangLiYuLanPart();
		};
		base.StartCoroutine<bool>(this.mJiangLiYuLanPart.InitNormalAward(this.awardXmlList, Global.GetLang("奖励预览")));
	}

	private void CloseJiangLiYuLanPart()
	{
		if (null != this.mJiangLiYuLanPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mJiangLiYuLanPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mJiangLiYuLanPartWind, true);
			this.mJiangLiYuLanPartWind = null;
		}
		if (null != this.mJiangLiYuLanPart)
		{
			this.mJiangLiYuLanPart.transform.parent = null;
			Object.Destroy(this.mJiangLiYuLanPart.gameObject);
			this.mJiangLiYuLanPart = null;
		}
	}

	private void initXmlData()
	{
		if (this.awardXmlList != null)
		{
			return;
		}
		XElement gameResXml = Global.GetGameResXml("Config/TeamMatchAward.xml");
		if (gameResXml != null)
		{
			this.awardXmlList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
		}
	}

	public void OpenTeamCompeteMatchStatusPart(List<ZhanDuiZhengBaZhanDuiData> dataList)
	{
		if (this.mTeamCompeteMatchStatusPartWind != null || this.mTeamCompeteMatchStatusPart != null)
		{
			this.CloseTeamCompeteMatchStatusPart();
		}
		this.mTeamCompeteMatchStatusPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteMatchStatusPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteMatchStatusPartWind.Modal = true;
		this.mTeamCompeteMatchStatusPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteMatchStatusPartWind, "mTeamCompeteMatchStatusPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteMatchStatusPartWind);
		this.mTeamCompeteMatchStatusPart = U3DUtils.NEW<TeamCompeteMatchStatusPart>();
		this.mTeamCompeteMatchStatusPart.InitData(dataList);
		this.mTeamCompeteMatchStatusPartWind.Body.Add(this.mTeamCompeteMatchStatusPart);
		this.mTeamCompeteMatchStatusPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteMatchStatusPart();
		};
	}

	private void CloseTeamCompeteMatchStatusPart()
	{
		if (null != this.mTeamCompeteMatchStatusPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteMatchStatusPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteMatchStatusPartWind, true);
			this.mTeamCompeteMatchStatusPartWind = null;
		}
		if (null != this.mTeamCompeteMatchStatusPart)
		{
			this.mTeamCompeteMatchStatusPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteMatchStatusPart.gameObject);
			this.mTeamCompeteMatchStatusPart = null;
		}
	}

	public void OpenTeamCompeteGuessPart(List<ZhanDuiZhengBaZhanDuiData> dataList)
	{
		if (this.mTeamCompeteGuessPartWind != null || this.mTeamCompeteGuessPart != null)
		{
			this.CloseTeamCompeteGuessPart();
		}
		this.mTeamCompeteGuessPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteGuessPartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteGuessPartWind.Modal = true;
		this.mTeamCompeteGuessPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteGuessPartWind, "mTeamCompeteGuessPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteGuessPartWind);
		this.mTeamCompeteGuessPart = U3DUtils.NEW<TeamCompeteGuessPart>();
		this.mTeamCompeteGuessPart.InitValue(dataList);
		this.mTeamCompeteGuessPartWind.Body.Add(this.mTeamCompeteGuessPart);
		this.mTeamCompeteGuessPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteGuessPart();
		};
		this.mTeamCompeteGuessPart.ClickHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteGuessPart();
			if (this.mTeamCompeteMatchStatusPart == null)
			{
				this.RequestTeamList();
			}
		};
	}

	private void CloseTeamCompeteGuessPart()
	{
		if (null != this.mTeamCompeteGuessPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteGuessPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteGuessPartWind, true);
			this.mTeamCompeteGuessPartWind = null;
		}
		if (null != this.mTeamCompeteGuessPart)
		{
			this.mTeamCompeteGuessPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteGuessPart.gameObject);
			this.mTeamCompeteGuessPart = null;
		}
	}

	public void OpenTeamCompeteBattlePart()
	{
		if (this.mTeamCompeteBattlePartWind != null || this.mTeamCompeteBattlePart != null)
		{
			this.CloseTeamCompeteBattlePart();
		}
		this.mTeamCompeteBattlePartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteBattlePartWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteBattlePartWind.Modal = true;
		this.mTeamCompeteBattlePartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteBattlePartWind, "mTeamCompeteBattlePartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteBattlePartWind);
		this.mTeamCompeteBattlePart = U3DUtils.NEW<TeamCompeteBattlePart>();
		this.mTeamCompeteBattlePartWind.Body.Add(this.mTeamCompeteBattlePart);
		this.mTeamCompeteBattlePart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteBattlePart();
		};
	}

	private void CloseTeamCompeteBattlePart()
	{
		if (null != this.mTeamCompeteBattlePartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteBattlePartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteBattlePartWind, true);
			this.mTeamCompeteBattlePartWind = null;
		}
		if (null != this.mTeamCompeteBattlePart)
		{
			this.mTeamCompeteBattlePart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteBattlePart.gameObject);
			this.mTeamCompeteBattlePart = null;
		}
	}

	private void DisplayActivityTime()
	{
		this.mTimePoints.Clear();
		this.mJinChengID.Clear();
		DateTime correctDateTime = Global.GetCorrectDateTime();
		Dictionary<int, TeamMatchVo>.Enumerator enumerator = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchXMLDict().GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<string[]> list = this.mTimePoints;
			KeyValuePair<int, TeamMatchVo> keyValuePair = enumerator.Current;
			list.Add(keyValuePair.Value.TimePoints.Split(new char[]
			{
				','
			}));
			List<int> list2 = this.mJinChengID;
			KeyValuePair<int, TeamMatchVo> keyValuePair2 = enumerator.Current;
			list2.Add(keyValuePair2.Key);
		}
		this.DayOfOpenActivity = this.mTimePoints[0][0].SafeToInt32(0);
		string[] array = this.mTimePoints[0][1].Split(new char[]
		{
			'-'
		});
		if (correctDateTime.Day == this.DayOfOpenActivity)
		{
			DateTime begin = this.ParseStringToDateTime(this.mTimePoints[0][1].Split(new char[]
			{
				'-'
			})[0]);
			DateTime end = this.ParseStringToDateTime(this.mTimePoints[this.mTimePoints.Count - 1][1].Split(new char[]
			{
				'-'
			})[1]);
			this.IsInCurrentDayActivityTime(begin, end);
			switch (this.mTimeStatus)
			{
			case TeamCompeteZhengBaPart.TimeStatus.NotBegin:
				this.DisplayLblJinDu = Global.GetLang("活动未开启");
				this.DisplayLblTime = Global.GetString(new object[]
				{
					correctDateTime.Year,
					".",
					correctDateTime.Month,
					".",
					this.DayOfOpenActivity,
					"   ",
					array[0],
					"-",
					array[1]
				});
				this.CheckActivityTime(this.ParseCfgTimeToDateTime(array[0]));
				break;
			case TeamCompeteZhengBaPart.TimeStatus.Beginning:
				this.ActivityBeginning(this.mTimePoints, this.mJinChengID);
				break;
			case TeamCompeteZhengBaPart.TimeStatus.End:
			{
				this.DisplayLblJinDu = Global.GetLang("活动未开启");
				DateTime dateTime = correctDateTime.AddMonths(1);
				this.DisplayLblTime = Global.GetString(new object[]
				{
					dateTime.Year,
					".",
					dateTime.Month,
					".",
					this.DayOfOpenActivity,
					"   ",
					array[0],
					"-",
					array[1]
				});
				break;
			}
			}
		}
		else if (correctDateTime.Day < this.DayOfOpenActivity)
		{
			this.DisplayLblJinDu = Global.GetLang("活动未开启");
			this.DisplayLblTime = Global.GetString(new object[]
			{
				correctDateTime.Year,
				".",
				correctDateTime.Month,
				".",
				this.DayOfOpenActivity,
				"   ",
				array[0],
				"-",
				array[1]
			});
		}
		else
		{
			this.DisplayLblJinDu = Global.GetLang("活动未开启");
			DateTime dateTime2 = correctDateTime.AddMonths(1);
			this.DisplayLblTime = Global.GetString(new object[]
			{
				dateTime2.Year,
				".",
				dateTime2.Month,
				".",
				this.DayOfOpenActivity,
				"   ",
				array[0],
				"-",
				array[1]
			});
		}
	}

	private void ActivityBeginning(List<string[]> timePoints, List<int> jinChengID)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		string[] array = timePoints[0][1].Split(new char[]
		{
			'-'
		});
		for (int i = 0; i < timePoints.Count; i++)
		{
			string[] array2 = timePoints[i][1].Split(new char[]
			{
				'-'
			});
			DateTime dateTime = this.ParseStringToDateTime(array2[0]);
			DateTime dateTime2 = this.ParseStringToDateTime(array2[1]);
			if (dateTime <= correctDateTime && correctDateTime <= dateTime2)
			{
				this.mCurrentMatchJingCheng = this.ParseMatchJinChengByID(jinChengID[i]);
				this.DisplayLblJinDu = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchJinDuByTimePoint(array2[0]);
				this.DisplayLblTime = Global.GetString(new object[]
				{
					correctDateTime.Year,
					".",
					correctDateTime.Month,
					".",
					this.DayOfOpenActivity,
					"   ",
					array2[0],
					"-",
					array2[1]
				});
				this.CheckActivityTime(this.ParseCfgTimeToDateTime(array2[1]));
				break;
			}
			if (correctDateTime > dateTime2)
			{
				if (i + 1 < timePoints.Count)
				{
					string[] array3 = timePoints[i + 1][1].Split(new char[]
					{
						'-'
					});
					DateTime dateTime3 = this.ParseStringToDateTime(array3[0]);
					if (correctDateTime < dateTime3)
					{
						this.mCurrentMatchJingCheng = this.ParseMatchJinChengByID(jinChengID[i]);
						this.mNextMatchJingCheng = this.ParseMatchJinChengByID(jinChengID[i + 1]);
						this.DisplayLblJinDu = IConfigbase<ConfigTeamCompete>.Instance.GetTeamMatchJinDuByTimePoint(array3[0]);
						this.DisplayLblTime = Global.GetString(new object[]
						{
							correctDateTime.Year,
							".",
							correctDateTime.Month,
							".",
							this.DayOfOpenActivity,
							"   ",
							array3[0],
							"-",
							array3[1]
						});
						MUDebug.Log<string>(new string[]
						{
							Global.GetLang("活动时间间隔间隙 now ") + correctDateTime.ToLongDateString()
						});
						MUDebug.Log<string>(new string[]
						{
							Global.GetLang("活动时间间隔间隙：DisplayLblTime time2[0] ~ time2[1]") + array3[0] + "   " + array3[1]
						});
						this.CheckActivityTime(this.ParseCfgTimeToDateTime(array3[0]));
						break;
					}
				}
				else
				{
					this.DisplayLblJinDu = Global.GetLang("活动未开启");
					DateTime dateTime4 = correctDateTime.AddMonths(1);
					this.DisplayLblTime = Global.GetString(new object[]
					{
						dateTime4.Year,
						".",
						dateTime4.Month,
						".",
						this.DayOfOpenActivity,
						"   ",
						array[0],
						"-",
						array[1]
					});
				}
			}
		}
	}

	private void CheckActivityTime(DateTime dateTime)
	{
		base.CancelInvoke("CheckTime");
		this.checkDateTime = dateTime;
		MUDebug.Log<string>(new string[]
		{
			"checkDateTime  " + this.checkDateTime.ToLongDateString()
		});
		base.InvokeRepeating("CheckTime", 0f, 1f);
	}

	private void CheckTime()
	{
		this.nowCheckTime = Global.GetCorrectDateTime();
		if (this.nowCheckTime > this.checkDateTime)
		{
			base.CancelInvoke("CheckTime");
			this.RequestMainInfoData();
		}
	}

	private int ParseMatchJinChengByID(int id)
	{
		int result = 64;
		switch (id)
		{
		case 1:
			result = 32;
			break;
		case 2:
			result = 16;
			break;
		case 3:
			result = 8;
			break;
		case 4:
			result = 4;
			break;
		case 5:
			result = 2;
			break;
		case 6:
			result = 1;
			break;
		}
		return result;
	}

	private string DisplayLblTime
	{
		set
		{
			this.LblTime.Text = Global.GetString(new object[]
			{
				"{e3b36c}",
				Global.GetLang("比赛时间："),
				"{17e43e}",
				value
			});
		}
	}

	private string DisplayLblJinDu
	{
		set
		{
			this.LblJinDu.Text = value;
		}
	}

	private void IsInCurrentDayActivityTime(DateTime begin, DateTime end)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime < begin)
		{
			this.mTimeStatus = TeamCompeteZhengBaPart.TimeStatus.NotBegin;
		}
		else if (begin <= correctDateTime && correctDateTime <= end)
		{
			this.mTimeStatus = TeamCompeteZhengBaPart.TimeStatus.Beginning;
		}
		else
		{
			this.mTimeStatus = TeamCompeteZhengBaPart.TimeStatus.End;
		}
	}

	private DateTime ParseStringToDateTime(string timeOfDay)
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime result = default(DateTime);
		string text = string.Format("{0}-{1}-{2} {3}", new object[]
		{
			correctDateTime.Year,
			correctDateTime.Month,
			correctDateTime.Day,
			timeOfDay
		});
		DateTime.TryParse(text, ref result);
		return result;
	}

	private DateTime ParseCfgTimeToDateTime(string time)
	{
		return this.ParseStringToDateTime(time);
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_MAIN_INFO", new Action<MUSocketConnectEventArgs>(this.RespondMainInfoData));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_SUPPORT_LIST", new Action<MUSocketConnectEventArgs>(this.RespondGuessInfoData));
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_ZHANDUI_LIST", new Action<MUSocketConnectEventArgs>(this.RespondTeamList));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_MAIN_INFO", new Action<MUSocketConnectEventArgs>(this.RespondMainInfoData));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_SUPPORT_LIST", new Action<MUSocketConnectEventArgs>(this.RespondGuessInfoData));
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_ZHANDUIZHENGBA_ZHANDUI_LIST", new Action<MUSocketConnectEventArgs>(this.RespondTeamList));
	}

	private void RequestTeamList()
	{
		GameInstance.Game.RequestTeamMatchList();
	}

	public void RespondTeamList(MUSocketConnectEventArgs e)
	{
		List<ZhanDuiZhengBaZhanDuiData> list = DataHelper.BytesToObject<List<ZhanDuiZhengBaZhanDuiData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("当前没有比赛队伍"), 10, 3);
			return;
		}
		this.OpenTeamCompeteMatchStatusPart(list);
	}

	public void RequestMainInfoData()
	{
		GameInstance.Game.RequestTeamZhengBaMainInfo(TeamCompeteDataManager.ZhengBaRequestFlag);
	}

	public void RespondMainInfoData(MUSocketConnectEventArgs e)
	{
		AgeDataT<List<ZhanDuiZhengBaZhanDuiData>> ageDataT = DataHelper.BytesToObject<AgeDataT<List<ZhanDuiZhengBaZhanDuiData>>>(e.bytesData, 0, e.bytesData.Length);
		if (ageDataT == null)
		{
			MUDebug.Log<string>(new string[]
			{
				Global.GetLang(" AgeDataT<List<ZhanDuiZhengBaZhanDuiData>> dataList 数据为空")
			});
			return;
		}
		if (ageDataT != null)
		{
			if (TeamCompeteDataManager.ZhengBaRequestFlag == ageDataT.Age)
			{
				return;
			}
			TeamCompeteDataManager.ZhengBaRequestFlag = ageDataT.Age;
		}
		this.mNextMatchJingCheng = -1;
		base.CancelInvoke("CheckTime");
		this.IsActivityOver = false;
		this.DisplayActivityTime();
		if (ageDataT != null && ageDataT.V != null && ageDataT.V.Count > 0)
		{
			ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = ageDataT.V.Find((ZhanDuiZhengBaZhanDuiData result) => result.Grade == 1);
			if (zhanDuiZhengBaZhanDuiData != null)
			{
				this.mLblWinner.Text = zhanDuiZhengBaZhanDuiData.ZhanDuiName;
				this.IsHighLightRongYaoBg = true;
				this.IsActivityOver = true;
			}
			else
			{
				this.mLblWinner.Text = null;
				this.IsHighLightRongYaoBg = false;
			}
		}
		else
		{
			this.mLblWinner.Text = null;
			this.IsHighLightRongYaoBg = false;
		}
		if (ageDataT != null && ageDataT.V == null)
		{
			ageDataT.V = new List<ZhanDuiZhengBaZhanDuiData>();
			for (int i = 0; i < 16; i++)
			{
				ageDataT.V.Add(null);
			}
		}
		if (ageDataT != null && ageDataT.V.Count < 16)
		{
			int num = 16 - ageDataT.V.Count;
			for (int j = 0; j < num; j++)
			{
				ageDataT.V.Add(null);
			}
		}
		this.ListToDict(ageDataT.V);
		this.InitValue();
	}

	private void ListToDict(List<ZhanDuiZhengBaZhanDuiData> dataList)
	{
		if (this.mServerMatchDataDict.Count > 0)
		{
			this.mServerMatchDataDict.Clear();
		}
		for (int i = 0; i < dataList.Count; i++)
		{
			ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = dataList[i];
			if (zhanDuiZhengBaZhanDuiData != null)
			{
				MUDebug.Log<string>(new string[]
				{
					"GetDictIndex(data.Group) " + this.GetDictIndex(zhanDuiZhengBaZhanDuiData.Group)
				});
				try
				{
					this.mServerMatchDataDict.Add(this.GetDictIndex(zhanDuiZhengBaZhanDuiData.Group), zhanDuiZhengBaZhanDuiData);
				}
				catch (Exception ex)
				{
					MUDebug.LogError<string>(new string[]
					{
						"比赛中不应该出现相同的data.Group " + zhanDuiZhengBaZhanDuiData.Group
					});
					MUDebug.LogError<string>(new string[]
					{
						ex.ToString()
					});
				}
			}
		}
	}

	private void RequestGuessInfo()
	{
		GameInstance.Game.RequestTeamMatchYaZhuList();
	}

	public void RespondGuessInfoData(MUSocketConnectEventArgs e)
	{
		List<ZhanDuiZhengBaZhanDuiData> list = DataHelper.BytesToObject<List<ZhanDuiZhengBaZhanDuiData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("您还没有竞猜战队，请选择您要投注的战队"), 10, 3);
			return;
		}
		if (TeamCompeteDataManager.ZhangBaGuessCallBack != null)
		{
			TeamCompeteDataManager.ZhangBaGuessCallBack.Invoke(list);
		}
		else
		{
			this.OpenTeamCompeteGuessPart(list);
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.awardXmlList = null;
		base.CancelInvoke("CheckTime");
		TeamCompeteDataManager.ZhengBaRequestFlag = 0L;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblJinCheng;

	public TextBlock LblJinDu;

	public TextBlock LblTime;

	public TextBlock LblMoney;

	public GButton BtnClose;

	public GButton BtnRule;

	public GButton BtnAward;

	public GButton BtnStatus;

	public GButton BtnGuess;

	public GButton BtnShop;

	public GameObject RongYaoClick;

	public UITexture rongYaoBg;

	public GameObject SingleInfo;

	public TextBlock mLblWinner;

	private int left;

	private int right = 1;

	public GameObject BtnChaKan;

	public Transform[] leftPos;

	public Transform[] rightPos;

	public Transform[] SingleInfoPositions;

	private GButton[] leftBtns;

	private GButton[] rightBtns;

	public Transform LeftRoot;

	public Transform RightRoot;

	private List<ZhanDuiZhengBaZhanDuiData> leftBttleInfos = new List<ZhanDuiZhengBaZhanDuiData>();

	private List<ZhanDuiZhengBaZhanDuiData> rightBttleInfos = new List<ZhanDuiZhengBaZhanDuiData>();

	private List<int> Eight;

	private List<int> Four;

	private List<int> Two;

	private List<int> One;

	private List<ZhanDuiZhengBaZhanDuiData> dataInfos;

	private string grayColor;

	private string red;

	private string blue;

	private string gray;

	protected GChildWindow mCommonHelpWindowWind;

	protected CommonHelpWindow mCommonHelpWindow;

	protected GChildWindow mMUDuiHuanPartWind;

	protected MUDuiHuanPart mMUDuiHuanPart;

	protected GChildWindow mJiangLiYuLanPartWind;

	protected JiangLiYuLanPart mJiangLiYuLanPart;

	private List<XElement> awardXmlList;

	protected GChildWindow mTeamCompeteMatchStatusPartWind;

	protected TeamCompeteMatchStatusPart mTeamCompeteMatchStatusPart;

	protected GChildWindow mTeamCompeteGuessPartWind;

	protected TeamCompeteGuessPart mTeamCompeteGuessPart;

	protected GChildWindow mTeamCompeteBattlePartWind;

	protected TeamCompeteBattlePart mTeamCompeteBattlePart;

	private TeamCompeteZhengBaPart.TimeStatus mTimeStatus;

	private int DayOfOpenActivity;

	private List<string[]> mTimePoints;

	private List<int> mJinChengID;

	private bool IsActivityOver;

	private int mCurrentMatchJingCheng;

	private int mNextMatchJingCheng;

	private DateTime checkDateTime;

	private DateTime nowCheckTime;

	private Dictionary<int, ZhanDuiZhengBaZhanDuiData> mServerMatchDataDict;

	public class MatchData
	{
		public string Name { get; set; }

		public int Value { get; set; }
	}

	private enum TimeStatus
	{
		NotBegin,
		Beginning,
		End
	}
}
