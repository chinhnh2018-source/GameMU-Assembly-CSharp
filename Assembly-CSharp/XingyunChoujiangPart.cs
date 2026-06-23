using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class XingyunChoujiangPart : UserControl
{
	public XingyunChoujiangPart()
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(new Vector3(0f, 0f, 0f));
		list.Add(new Vector3(-648f, 127f, 0f));
		list.Add(new Vector3(-557f, 127f, 0f));
		list.Add(new Vector3(-466f, 127f, 0f));
		list.Add(new Vector3(-376f, 127f, 0f));
		list.Add(new Vector3(-287f, 127f, 0f));
		list.Add(new Vector3(-196f, 127f, 0f));
		list.Add(new Vector3(-196f, 41f, 0f));
		list.Add(new Vector3(-196f, -44f, 0f));
		list.Add(new Vector3(-196f, -130f, 0f));
		list.Add(new Vector3(-287f, -130f, 0f));
		list.Add(new Vector3(-377f, -130f, 0f));
		list.Add(new Vector3(-467f, -130f, 0f));
		list.Add(new Vector3(-557f, -130f, 0f));
		list.Add(new Vector3(-648f, -130f, 0f));
		list.Add(new Vector3(-648f, -44f, 0f));
		list.Add(new Vector3(-648f, 41f, 0f));
		this.mGoodsposition = list;
		this._ListObj = new List<GameObject>();
		this._LocationY = -186f;
		this.startPosition = 1;
		this.awardGoodsID = string.Empty;
		this.mDicIDtoZhuanpan = new Dictionary<int, Zhuanpan>();
		this.UsedTime = 0.001f;
		this.Round = 1;
		this.Control = true;
		this.i = 1;
		base..ctor();
	}

	private void InitPerfabText()
	{
		this.m_lblDesc.text = string.Empty;
		if (this.m_btnJinbi.Label != null)
		{
			this.m_btnJinbi.Text = Global.GetLang("普通抽奖");
		}
		if (this.m_btnBangzuan.Label != null)
		{
			this.m_btnBangzuan.Text = Global.GetLang("高级抽奖");
		}
		if (this.m_btnZuanshi.Label != null)
		{
			this.m_btnZuanshi.Text = Global.GetLang("豪华抽奖");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitPerfabText();
		this.Infolist = this.m_Infolist.ItemsSource;
		GameInstance.Game.GetXingyunChoujiangData();
		this.mGetLableInfo();
		this.m_Infolist.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedTips);
		this.m_btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isRunning)
			{
				Super.HintMainText(Global.GetLang("上一轮抽奖还未结束"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		UIEventListener.Get(this.m_btnJia.gameObject).onClick = delegate(GameObject s)
		{
			if (this.isRunning)
			{
				Super.HintMainText(Global.GetLang("上一轮抽奖还未结束"), 10, 3);
				return;
			}
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ZhuanPanCost", '|');
			List<int> list = new List<int>();
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				int num = Convert.ToInt32(systemParamStringArrayByName[i].Split(new char[]
				{
					','
				})[0]);
				list.Add(num);
			}
			if (list.Contains(163))
			{
				PlayZone.GlobalPlayZone.ShowMallWindow(false, 0, 2);
			}
			else
			{
				PlayZone.GlobalPlayZone.ShowChongZhiWindow();
			}
		};
		this.m_btnJinbi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isRunning)
			{
				Super.HintMainText(Global.GetLang("上一轮抽奖还未结束"), 10, 3);
				return;
			}
			if (this.ChackIsBgFull())
			{
				return;
			}
			GameInstance.Game.SendXingyunChoujiangInfo(1);
		};
		this.m_btnBangzuan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isRunning)
			{
				Super.HintMainText(Global.GetLang("上一轮抽奖还未结束"), 10, 3);
				return;
			}
			if (this.ChackIsBgFull())
			{
				return;
			}
			GameInstance.Game.SendXingyunChoujiangInfo(2);
		};
		this.m_btnZuanshi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isRunning)
			{
				Super.HintMainText(Global.GetLang("上一轮抽奖还未结束"), 10, 3);
				return;
			}
			if (this.ChackIsBgFull())
			{
				return;
			}
			if (this.m_costTypeNum[2][0] == 163 && this._NeedZuanshi > Global.GetRoleOwnNumByMoneyType(163))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = this._NeedZuanshi - Global.GetRoleOwnNumByMoneyType(163);
				string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi))
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
					}
					return true;
				};
				return;
			}
			GameInstance.Game.SendXingyunChoujiangInfo(3);
		};
	}

	private bool ChackIsBgFull()
	{
		if (Global.IsBagFull())
		{
			Super.HintMainText(Global.GetLang("背包已满"), 10, 3);
			return true;
		}
		if (Global.IsRebornBagFull())
		{
			Super.HintMainText(Global.GetLang("重生背包已满"), 10, 3);
			return true;
		}
		return false;
	}

	private void SelectedTips(object sender, MouseEvent e)
	{
		XingyunLableItem xingyunLableItem = U3DUtils.AS<XingyunLableItem>(this.m_Infolist.SelectedItem);
		if (xingyunLableItem == null)
		{
			return;
		}
		XingyunChoujiangItem component = this.m_Goodslist.getChildAt(xingyunLableItem.GoodsIndex).GetComponent<XingyunChoujiangItem>();
		GGoodIcon componentInChildren = component.mGoodIcon.GetComponentInChildren<GGoodIcon>();
		if (componentInChildren == null)
		{
			return;
		}
		GoodsData goodData = componentInChildren.ItemObject as GoodsData;
		GTipServiceEx.ShowTip(componentInChildren, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	private void mGetConfigInfo(List<ZhuanPanItem> mZhuanpanItemList)
	{
		this.mAddRuningObj();
		if (mZhuanpanItemList == null)
		{
			return;
		}
		for (int i = 0; i < mZhuanpanItemList.Count; i++)
		{
			ZhuanPanItem zhuanPanItem = mZhuanpanItemList[i];
			if (zhuanPanItem == null)
			{
				return;
			}
			Zhuanpan zhuanpan = new Zhuanpan();
			zhuanpan.ID = zhuanPanItem.ID;
			zhuanpan.GoodsID = zhuanPanItem.GoodsID;
			zhuanpan.AwardLevel = zhuanPanItem.AwardLevel.ToString();
			if (Context.IsHaiwai)
			{
				zhuanpan.AwardLabel = zhuanPanItem.AwardLabel;
			}
			if (!this.mDicIDtoZhuanpan.ContainsKey(zhuanpan.ID))
			{
				this.mDicIDtoZhuanpan.Add(zhuanpan.ID, zhuanpan);
			}
		}
		this.mLoadGoodsInfo();
	}

	private void mLoadGoodsInfo()
	{
		Dictionary<int, Zhuanpan>.Enumerator enumerator = this.mDicIDtoZhuanpan.GetEnumerator();
		while (enumerator.MoveNext())
		{
			XingyunChoujiangItem xingyunChoujiangItem = U3DUtils.NEW<XingyunChoujiangItem>();
			Transform transform = xingyunChoujiangItem.gameObject.transform;
			List<Vector3> list = this.mGoodsposition;
			KeyValuePair<int, Zhuanpan> keyValuePair = enumerator.Current;
			transform.localPosition = list[keyValuePair.Key];
			XingyunChoujiangItem xingyunChoujiangItem2 = xingyunChoujiangItem;
			KeyValuePair<int, Zhuanpan> keyValuePair2 = enumerator.Current;
			xingyunChoujiangItem2.MZhuanpan = keyValuePair2.Value;
			this.m_Goodslist.Add(xingyunChoujiangItem);
		}
	}

	private void mGetreadyGoRunning()
	{
		this.UsedTime = this.m_BeginSpeed / 10000f;
		this.Round = 1;
		this.Count = 0;
		this.Control = true;
		NGUITools.SetActive(this.walkItem.gameObject, true);
		if (this.stopItem != null)
		{
			this.stopItem.MXingYunSelectState = XingYunSelectState.None;
		}
		this.runCount = ((this.startPosition <= this.endPosition) ? ((this.endPosition - this.startPosition != 0) ? (this.endPosition - this.startPosition) : 16) : (16 - (this.startPosition - this.endPosition)));
	}

	public bool IsRunning
	{
		get
		{
			return this.isRunning;
		}
		set
		{
			this.isRunning = value;
			NGUITools.SetActive(this._Shandeng.gameObject, value);
			Global.IsRunning = value;
		}
	}

	private void mLetsUsBegin()
	{
		this.IsRunning = true;
		this.mGetreadyGoRunning();
		base.StartCoroutine<bool>(this.mBegainRunning(this.startPosition, this.endPosition));
	}

	private void mAddRuningObj()
	{
		this.walkItem = U3DUtils.NEW<XingyunChoujiangItem>();
		this.walkItem.transform.localPosition = this.mGoodsposition[this.startPosition];
		this.walkItem.MXingYunSelectState = XingYunSelectState.Walking;
		this.m_Goodslist.Add(this.walkItem);
		NGUITools.SetActive(this.walkItem.gameObject, false);
	}

	private IEnumerator mStopObj(int end)
	{
		NGUITools.SetActive(this.walkItem.gameObject, false);
		this.stopItem = this.m_Goodslist.getChildAt(end).GetComponent<XingyunChoujiangItem>();
		if (this.stopItem != null)
		{
			this.stopItem.MXingYunSelectState = XingYunSelectState.Selected;
		}
		yield return new WaitForSeconds(0.5f);
		PlayZone.GlobalPlayZone.OpenXingyunChoujiangAwardWindow(this.awardGoodsID, this.mZhuanPanAwardType);
		NGUITools.SetActive(this._Shandeng.gameObject, false);
		base.StopAllCoroutines();
		yield break;
	}

	protected IEnumerator mBegainRunning(int begin, int end)
	{
		for (;;)
		{
			if (begin > 16)
			{
				begin = 1;
			}
			if (this.Count == 16)
			{
				this.Round++;
				this.Count = 0;
			}
			if (this.Round < 4)
			{
				this.walkItem.gameObject.transform.localPosition = Vector3.Lerp(this.mGoodsposition[begin], this.mGoodsposition[(begin + 1 <= 16) ? (begin + 1) : 1], 1f);
				yield return new WaitForSeconds(this.UsedTime * 5f);
				begin++;
				this.Count++;
			}
			else if (this.Round < 5 && this.runCount < 8)
			{
				this.walkItem.gameObject.transform.localPosition = Vector3.Lerp(this.mGoodsposition[begin], this.mGoodsposition[(begin + 1 <= 16) ? (begin + 1) : 1], 1f);
				yield return new WaitForSeconds(this.UsedTime);
				this.UsedTime = ((this.UsedTime < 0.2f) ? (this.UsedTime += Time.deltaTime) : 0.2f);
				begin++;
				this.Count++;
				this.Control = false;
			}
			else
			{
				this.walkItem.gameObject.transform.localPosition = Vector3.Lerp(this.mGoodsposition[begin], this.mGoodsposition[(begin + 1 <= 16) ? (begin + 1) : 1], 1f);
				yield return new WaitForSeconds(this.UsedTime);
				if (this.Control)
				{
					this.UsedTime = this.m_StopSpeed / 100f;
					this.Control = false;
				}
				this.UsedTime = ((this.UsedTime < 0.6f) ? (this.UsedTime += Time.deltaTime) : 0.6f);
				if (begin == ((end - 1 != 0) ? (end - 1) : 16))
				{
					base.StartCoroutine(this.mStopObj(end));
					this.startPosition = end;
				}
				else
				{
					begin++;
				}
			}
		}
		yield break;
	}

	public void mGetLableInfo()
	{
		if (this.m_costTypeNum == null)
		{
			this.m_lblZuanshiNum.text = "0";
			this.m_lblBangzuanNum.text = "0";
			this.m_lblJinbiNum.text = "0";
			return;
		}
		int selfMoneyTypeNum = this.GetSelfMoneyTypeNum((ChouJiangMoneyTypes)this.m_costTypeNum[0][0]);
		int selfMoneyTypeNum2 = this.GetSelfMoneyTypeNum((ChouJiangMoneyTypes)this.m_costTypeNum[1][0]);
		int selfMoneyTypeNum3 = this.GetSelfMoneyTypeNum((ChouJiangMoneyTypes)this.m_costTypeNum[2][0]);
		this.m_lblZuanshiNum.text = selfMoneyTypeNum3.ToString();
		this.m_lblBangzuanNum.text = selfMoneyTypeNum2.ToString();
		this.m_lblJinbiNum.text = selfMoneyTypeNum.ToString();
	}

	private void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.nextTime.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			this.m_lblNeedZuanshiNum.Text = this._NeedZuanshi.ToString();
			this.m_lblTime.text = Global.GetTimeStrBySecNoWord((double)num2) + Global.GetLang(" 后免费");
			this.m_lblNeedZuanshiNum.gameObject.transform.localPosition = new Vector3(this.m_btnZuanshi.gameObject.transform.localPosition.x + 55f, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.y, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.z);
			NGUITools.SetActive(this._zuanshiIcon.gameObject, true);
		}
		else if (num == correctLocalTime)
		{
			this.m_lblTime.text = Global.GetLang(string.Empty);
			this.m_lblNeedZuanshiNum.Text = Global.GetLang("本次免费");
			this.m_lblNeedZuanshiNum.gameObject.transform.localPosition = new Vector3(this.m_btnZuanshi.gameObject.transform.localPosition.x + 40f, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.y, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.z);
			NGUITools.SetActive(this._zuanshiIcon.gameObject, false);
			base.CancelInvoke("TickProc");
			GameInstance.Game.GetXingyunChoujiangData();
		}
		else
		{
			this.m_lblTime.text = Global.GetLang(string.Empty);
			this.m_lblNeedZuanshiNum.Text = Global.GetLang("本次免费");
			this.m_lblNeedZuanshiNum.gameObject.transform.localPosition = new Vector3(this.m_btnZuanshi.gameObject.transform.localPosition.x + 40f, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.y, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.z);
			NGUITools.SetActive(this._zuanshiIcon.gameObject, false);
			base.CancelInvoke("TickProc");
		}
	}

	private void _SetBtnPosition(List<GameObject> mListObj)
	{
		if (mListObj.Count == 1)
		{
			mListObj[0].transform.localPosition = new Vector3(-85f, this._LocationY, 0f);
		}
		else if (mListObj.Count == 2)
		{
			mListObj[0].transform.localPosition = new Vector3(-260f, this._LocationY, 0f);
			mListObj[1].transform.localPosition = new Vector3(65f, this._LocationY, 0f);
		}
		else if (mListObj.Count == 3)
		{
			mListObj[0].transform.localPosition = new Vector3(-314f, this._LocationY, 0f);
			mListObj[1].transform.localPosition = new Vector3(-85f, this._LocationY, 0f);
			mListObj[2].transform.localPosition = new Vector3(150f, this._LocationY, 0f);
		}
	}

	private void mInitGongGaoData()
	{
	}

	public void mGetZhuanPanMainData(ZhuanPanMainData mZhuanPanMainData)
	{
		if (mZhuanPanMainData == null)
		{
			return;
		}
		if (mZhuanPanMainData.FreeTime != DateTime.MaxValue)
		{
			base.CancelInvoke("TickProc");
			this.nextTime = mZhuanPanMainData.FreeTime;
			base.InvokeRepeating("TickProc", 0f, 1f);
		}
		else
		{
			NGUITools.SetActive(this.m_lblTime.gameObject, false);
		}
		this.m_Goodslist.Clear();
		this._ListObj.Clear();
		this.mGetConfigInfo(mZhuanPanMainData.ZhuanPanAwardItemList);
		string text = mZhuanPanMainData.LeftFuLiCount.ToString();
		if (!string.IsNullOrEmpty("strDesc"))
		{
			this.m_lblDesc.Text = string.Format(Global.GetLang("豪华抽取{0}次时,必定获得珍贵道具"), text);
		}
		List<List<int>> list = this.LoadCostData(mZhuanPanMainData.ZhuanPanCostArray);
		if (list == null)
		{
			list = this.LoadTestData();
		}
		if (list != null && list.Count > 2)
		{
			this.m_costTypeNum = list;
			if (list[0][0] != -1 && list[0][1] != -1)
			{
				ChouJiangMoneyTypes type = (ChouJiangMoneyTypes)list[0][0];
				this.m_lblNeedJinbiNum.text = list[0][1].ToString();
				this._ListObj.Add(this._Jinbi);
				NGUITools.SetActive(this._Jinbi, true);
				this.SetBtnIconSprite(this._Jinbi, type);
				NGUITools.SetActive(this._SelfJinbi, true);
				this.SetBtnIconSprite(this._SelfJinbi, type);
			}
			else
			{
				NGUITools.SetActive(this._Jinbi, false);
				NGUITools.SetActive(this._SelfJinbi, false);
			}
			if (list[1][0] != -1 && list[1][1] != -1)
			{
				this.m_lblNeedBangzuanNum.text = list[1][1].ToString();
				ChouJiangMoneyTypes type2 = (ChouJiangMoneyTypes)list[1][0];
				this._ListObj.Add(this._Bangzuan);
				NGUITools.SetActive(this._Bangzuan, true);
				this.SetBtnIconSprite(this._Bangzuan, type2);
				NGUITools.SetActive(this._SelfBangzuan, true);
				this.SetBtnIconSprite(this._SelfBangzuan, type2);
			}
			else
			{
				NGUITools.SetActive(this._Bangzuan, false);
				NGUITools.SetActive(this._SelfBangzuan, false);
			}
			if (list[2][0] != -1 && list[2][1] != -1)
			{
				this.m_lblNeedZuanshiNum.text = list[2][1].ToString();
				ChouJiangMoneyTypes chouJiangMoneyTypes = (ChouJiangMoneyTypes)list[2][0];
				this._ListObj.Add(this._Zuanshi);
				this._NeedZuanshi = list[2][1];
				this.m_lblNeedZuanshiNum.gameObject.transform.localPosition = new Vector3(this.m_btnZuanshi.gameObject.transform.localPosition.x + 55f, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.y, this.m_lblNeedZuanshiNum.gameObject.transform.localPosition.z);
				NGUITools.SetActive(this._zuanshiIcon.gameObject, true);
				NGUITools.SetActive(this._Zuanshi, true);
				this.SetBtnIconSprite(this._Zuanshi, chouJiangMoneyTypes);
				if (chouJiangMoneyTypes == ChouJiangMoneyTypes.YuanBao)
				{
					this.m_btnJia.gameObject.SetActive(true);
				}
				else
				{
					this.m_btnJia.gameObject.SetActive(false);
				}
				NGUITools.SetActive(this._SelfZuanshi, true);
				this.SetBtnIconSprite(this._SelfZuanshi, chouJiangMoneyTypes);
			}
			else
			{
				NGUITools.SetActive(this._Zuanshi, false);
				NGUITools.SetActive(this._SelfZuanshi, false);
			}
			this._SetBtnPosition(this._ListObj);
			this.mGetLableInfo();
		}
		if (mZhuanPanMainData.GoodsItem != null)
		{
			PlayZone.GlobalPlayZone.OpenXingyunChoujiangAwardWindow(mZhuanPanMainData.GoodsItem.GoodsID, (ZhuanPanAwardType)mZhuanPanMainData.GoodsItem.AwardLevel);
		}
		if (mZhuanPanMainData.GongGaoList != null && mZhuanPanMainData.GongGaoList.Count > 0)
		{
			for (int i = 0; i < mZhuanPanMainData.GongGaoList.Count; i++)
			{
				XingyunLableItem xingyunLableItem = U3DUtils.NEW<XingyunLableItem>();
				xingyunLableItem.mSetText(mZhuanPanMainData.GongGaoList[i]);
				this.Infolist.Add(xingyunLableItem);
			}
		}
	}

	public void mGetChoujiangResult(ZhuanPanChouJiangData mZhuanPanChouJiangData)
	{
		if (mZhuanPanChouJiangData == null)
		{
			return;
		}
		int result = mZhuanPanChouJiangData.Result;
		int num = result;
		switch (num + 4)
		{
		case 0:
			Super.HintMainText(Global.GetLang("背包空间不足！"), 10, 3);
			break;
		case 1:
		{
			ChouJiangMoneyTypes moneyType = (ChouJiangMoneyTypes)this.m_costTypeNum[2][0];
			this.ShowNotEnough(moneyType);
			break;
		}
		case 2:
		{
			ChouJiangMoneyTypes moneyType2 = (ChouJiangMoneyTypes)this.m_costTypeNum[1][0];
			this.ShowNotEnough(moneyType2);
			break;
		}
		case 3:
		{
			ChouJiangMoneyTypes moneyType3 = (ChouJiangMoneyTypes)this.m_costTypeNum[0][0];
			this.ShowNotEnough(moneyType3);
			break;
		}
		default:
			switch (num + 202)
			{
			case 0:
				Super.HintMainText(Global.GetLang("领取奖励后才可以再次抽奖！"), 10, 3);
				break;
			case 1:
				Super.HintMainText(Global.GetLang("服务器奖励列表配置错误！"), 10, 3);
				break;
			case 2:
				Super.HintMainText(Global.GetLang("非法操作！"), 10, 3);
				break;
			default:
				if (num != -101)
				{
					if (num != -100)
					{
						Super.HintMainText(Global.GetLang("其他错误！"), 10, 3);
					}
					else
					{
						Super.HintMainText(Global.GetLang("活动暂未开放！"), 10, 3);
					}
				}
				else
				{
					Super.HintMainText(Global.GetLang("服务器配置出错！"), 10, 3);
				}
				break;
			}
			break;
		case 5:
		{
			base.CancelInvoke("TickProc");
			this.endPosition = mZhuanPanChouJiangData.GoodsItem.ID;
			this.awardGoodsID = mZhuanPanChouJiangData.GoodsItem.GoodsID;
			this.mZhuanPanAwardType = (ZhuanPanAwardType)mZhuanPanChouJiangData.GoodsItem.AwardLevel;
			this.nextTime = mZhuanPanChouJiangData.FreeTime;
			base.InvokeRepeating("TickProc", 0f, 1f);
			string text = mZhuanPanChouJiangData.LeftFuLiCount.ToString();
			if (!string.IsNullOrEmpty("strDesc"))
			{
				this.m_lblDesc.Text = string.Format(Global.GetLang("豪华抽取{0}次时,必定获得珍贵道具"), text);
			}
			this.mGetLableInfo();
			this.mLetsUsBegin();
			break;
		}
		}
	}

	private void JustforTest()
	{
		XingyunLableItem xingyunLableItem = U3DUtils.NEW<XingyunLableItem>();
		xingyunLableItem.name = this.i.ToString();
		xingyunLableItem.mSetText(new ZhuanPanGongGaoData
		{
			GoodsId = "2005,1,1,1,1,1,1",
			RoleName = Global.GetLang("奇迹小玩家") + this.i.ToString()
		});
		this.i++;
		this.Infolist.Add(xingyunLableItem);
		if (this.Infolist.Length > 20)
		{
			this.Infolist.RemoveAt(0);
		}
	}

	public void mGetGongGaoData(ZhuanPanGongGaoData mZhuanPanGongGaoData)
	{
		if (mZhuanPanGongGaoData == null)
		{
			return;
		}
		XingyunLableItem xingyunLableItem = U3DUtils.NEW<XingyunLableItem>();
		xingyunLableItem.mSetText(mZhuanPanGongGaoData);
		this.Infolist.Add(xingyunLableItem);
		if (this.Infolist.Length > 20)
		{
			this.Infolist.RemoveAt(0);
		}
	}

	private string GetMoneyTpyeSprite(ChouJiangMoneyTypes type)
	{
		return type.ToString();
	}

	private int GetSelfMoneyTypeNum(ChouJiangMoneyTypes moneyType)
	{
		int result = 0;
		switch (moneyType)
		{
		case ChouJiangMoneyTypes.LangHunFenMo:
			result = Global.GetRoleCommonUseParamsValue(34);
			break;
		default:
			switch (moneyType)
			{
			case ChouJiangMoneyTypes.FuWenZhiChen:
				result = Global.GetRoleCommonUseParamsValue(49);
				break;
			default:
				if (moneyType != ChouJiangMoneyTypes.OrnamentCharmPoint)
				{
					if (moneyType != ChouJiangMoneyTypes.ShenLiJingHua)
					{
						if (moneyType != ChouJiangMoneyTypes.TongQian)
						{
							if (moneyType != ChouJiangMoneyTypes.YinLiang)
							{
								if (moneyType != ChouJiangMoneyTypes.JingYuanZhi)
								{
									if (moneyType != ChouJiangMoneyTypes.YuanBao)
									{
										if (moneyType != ChouJiangMoneyTypes.BindYuanBao)
										{
											if (moneyType != ChouJiangMoneyTypes.HunJing)
											{
												if (moneyType == ChouJiangMoneyTypes.xingyunzhixing)
												{
													result = Global.GetRoleOwnNumByMoneyType(163);
												}
											}
											else
											{
												result = (int)Global.Data.roleData.MoneyData[139];
											}
										}
										else
										{
											result = Global.Data.roleData.Gold;
										}
									}
									else
									{
										result = Global.Data.roleData.UserMoney;
									}
								}
								else
								{
									result = Global.GetRoleCommonUseParamsValue(5);
								}
							}
							else
							{
								result = Global.Data.roleData.YinLiang;
							}
						}
						else
						{
							result = Global.Data.roleData.Money1;
						}
					}
					else
					{
						result = Global.GetRoleCommonUseParamsValue(39);
					}
				}
				else
				{
					result = Global.GetRoleCommonUseParamsValue(38);
				}
				break;
			case ChouJiangMoneyTypes.JueXing:
				result = (int)Global.Data.roleData.MoneyData[132];
				break;
			}
			break;
		case ChouJiangMoneyTypes.ZaiZao:
			result = Global.GetRoleCommonUseParamsValue(27);
			break;
		case ChouJiangMoneyTypes.MUMoHe:
			result = Global.GetRoleCommonUseParamsValue(25);
			break;
		case ChouJiangMoneyTypes.YuanSuFenMo:
			result = Global.GetRoleCommonUseParamsValue(23);
			break;
		case ChouJiangMoneyTypes.GuardPoint:
			break;
		case ChouJiangMoneyTypes.Fluorescent:
			result = Global.GetRoleCommonUseParamsValue(31);
			break;
		}
		return result;
	}

	private void SetBtnIconSprite(GameObject btn, ChouJiangMoneyTypes type)
	{
		string moneyTpyeSprite = this.GetMoneyTpyeSprite(type);
		UISprite component = btn.transform.FindChild("Icon").gameObject.GetComponent<UISprite>();
		component.spriteName = moneyTpyeSprite;
	}

	private List<List<int>> LoadCostData(int[] data)
	{
		List<List<int>> list = new List<List<int>>();
		if (data == null)
		{
			return null;
		}
		if (data.Length % 2 != 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"格式错误"
			});
			return null;
		}
		for (int i = 0; i < data.Length; i += 2)
		{
			List<int> list2 = new List<int>();
			list2.Add(data[i]);
			list2.Add(data[i + 1]);
			list.Add(list2);
		}
		return list;
	}

	private void ShowNotEnough(ChouJiangMoneyTypes moneyType)
	{
		if (moneyType == ChouJiangMoneyTypes.TongQian || moneyType == ChouJiangMoneyTypes.YinLiang)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
		}
		else if (moneyType == ChouJiangMoneyTypes.BindYuanBao || moneyType == ChouJiangMoneyTypes.YuanBao)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
		else
		{
			Super.HintMainText(Global.GetLang("抽奖所需货币不足"), 10, 3);
		}
	}

	private List<List<int>> LoadTestData()
	{
		List<List<int>> list = new List<List<int>>();
		for (int i = 0; i < this.lstTest.Count; i++)
		{
			string[] array = this.lstTest[i].Split(new char[]
			{
				','
			});
			List<int> list2 = new List<int>();
			for (int j = 0; j < array.Length; j++)
			{
				list2.Add(array[j].SafeToInt32(0));
			}
			list.Add(list2);
		}
		return list;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL m_Goodslist;

	public ListBox m_Infolist;

	private ObservableCollection Infolist;

	public UIButton m_btnJia;

	public GButton m_btnClose;

	public GButton m_btnJinbi;

	public GButton m_btnBangzuan;

	public GButton m_btnZuanshi;

	public GButton m_btnChoujian;

	public TextBlock m_lblTime;

	public TextBlock m_lblZuanshiNum;

	public TextBlock m_lblBangzuanNum;

	public TextBlock m_lblJinbiNum;

	public TextBlock m_lblNeedZuanshiNum;

	public TextBlock m_lblNeedJinbiNum;

	public TextBlock m_lblNeedBangzuanNum;

	public TextBlock m_lblDesc;

	public GameObject _Jinbi;

	public GameObject _Zuanshi;

	public GameObject _Bangzuan;

	public GameObject _SelfJinbi;

	public GameObject _SelfZuanshi;

	public GameObject _SelfBangzuan;

	public UISprite _zuanshiIcon;

	public GameObject _Shandeng;

	private ZhuanPanAwardType mZhuanPanAwardType;

	public float m_BeginSpeed = 10f;

	public float m_StopSpeed = 1f;

	public List<string> lstTest;

	private List<List<int>> m_costTypeNum;

	private List<Vector3> mGoodsposition;

	private List<GameObject> _ListObj;

	private float _LocationY;

	private int _NeedZuanshi;

	private int startPosition;

	private int endPosition;

	private string awardGoodsID;

	private int runCount;

	private Dictionary<int, Zhuanpan> mDicIDtoZhuanpan;

	private bool isRunning;

	private XingyunChoujiangItem walkItem;

	private XingyunChoujiangItem stopItem;

	private float UsedTime;

	private int Round;

	private new int Count;

	private bool Control;

	private DateTime nextTime;

	private int i;
}
