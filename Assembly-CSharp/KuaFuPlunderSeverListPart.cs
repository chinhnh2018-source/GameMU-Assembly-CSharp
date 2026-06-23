using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderSeverListPart : UserControl
{
	protected override void OnDestroy()
	{
		this.StopTimeTicks();
		IConfigbase<ConfigKuaFuPlunder>.Instance.DisposeCrusadeWarXml();
		base.OnDestroy();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.mCrusadeWarXml = IConfigbase<ConfigKuaFuPlunder>.Instance.GetCrusadeWarXmlInstance();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (mapSceneUIClass == SceneUIClasses.KuaFuPlunderMap)
		{
			this.mKuaFuLueDuoMainInfo = KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo;
			if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null)
			{
				this.mKuaFuPlunderGameStateType = KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.GameState;
			}
			if (this.mKuaFuLueDuoMainInfo == null)
			{
				GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
			}
			else
			{
				base.StartCoroutine<bool>(this.RefreshRankItem());
				this.StartTimeTicks();
			}
		}
		else
		{
			GameInstance.Game.SendGetKuFuPlubderServerDataList(-1L, -1L);
		}
		Vector3 localPosition = this.mRankListBox.transform.localPosition;
		localPosition.z = -1f;
		this.mRankListBox.transform.localPosition = localPosition;
	}

	private void ChangeBtnState(GButton btn, bool isEnabled, string btnStr)
	{
		if (null != btn)
		{
			if (btnStr != null && null != btn.Label && !string.IsNullOrEmpty(btnStr))
			{
				btn.Label.text = btnStr;
			}
			btn.isEnabled = isEnabled;
			string text = string.Empty;
			string text2 = string.Empty;
			if (isEnabled)
			{
				text = "btn_green";
				text2 = "btn_green_selected";
			}
			else
			{
				text = "btn_green_disable";
				text2 = "btn_green_disable";
			}
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				btn.normalSprite = text;
				btn.hoverSprite = text2;
				btn.pressedSprite = text2;
				btn.disabledSprite = text2;
			}
			btn.Refresh();
		}
	}

	private void StopTimeTicks()
	{
		if (this.mDispatcherTimer != null)
		{
			this.mDispatcherTimer.Stop();
			this.mDispatcherTimer.Dispose();
			this.mDispatcherTimer = null;
		}
	}

	private void StartTimeTicks()
	{
		this.StopTimeTicks();
		this.mDispatcherTimer = null;
		this.mDispatcherTimer = new DispatcherTimer("KuaFuPlubderRankPart");
		this.mDispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mDispatcherTimer.Tick = new DispatcherTimerEventHandler(this.UITimeTicks);
		this.mDispatcherTimer.Start();
		this.UITimeTicks(null, null);
	}

	private void UITimeTicks(object sender, EventArgs args)
	{
		DateTime minValue = DateTime.MinValue;
		bool flag = false;
		int nextStateTimeData = this.mCrusadeWarXml.GetNextStateTimeData(out minValue, out flag, this.mKuaFuPlunderGameStateType);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (!flag)
		{
			if (this.mKuaFuPlunderGameStateType == 1)
			{
				if (nextStateTimeData == -1)
				{
					UILabel uilabel = this.mTimeLabel;
					uilabel.text += Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						Global.GetLang("竞价已结束")
					});
				}
				else if (0 < nextStateTimeData && 5 > nextStateTimeData)
				{
					string text = string.Empty;
					if (nextStateTimeData == 1)
					{
						text = Global.GetLang("距第一轮竞价结束：");
					}
					else if (nextStateTimeData == 2)
					{
						text = Global.GetLang("距第二轮竞价结束：");
					}
					else if (nextStateTimeData == 3)
					{
						text = Global.GetLang("距第三轮竞价结束：");
					}
					else
					{
						text = Global.GetLang("距第四轮竞价结束：");
					}
					int num = (int)(minValue - correctDateTime).TotalSeconds;
					this.mTimeLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fdf7dd",
						text,
						"17e43e",
						Global.GetTimeStrBySecEx((double)num, true, 2)
					});
				}
			}
			else
			{
				this.mTimeLabel.text = string.Empty;
			}
		}
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTimeLabel.text = string.Empty;
			this.mTitlesLabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("服务器")
			});
			this.mTitlesLabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("结果公布")
			});
			this.mTitlesLabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("当前资源")
			});
			this.mTitlesLabel[3].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("与本服关系")
			});
			this.mTitlesLabel[4].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("征服情况")
			});
			this.mSelectLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("只显示已竞价的服务器")
			});
			this.mRefreshBtn.Label.text = Global.GetLang("刷新");
			this.mBiddingBtn.Label.text = Global.GetLang("竞价");
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
			this.mBgImage1.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/MainPartBgImage.png";
			this.mBgImage1.ImageDownloaded = delegate(object g)
			{
				this.mBgImage1.transform.localScale = new Vector3((float)this.mBgImage1.ItsSizeWidth, (float)this.mBgImage1.ItsSizeHeight, 0f);
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mOBCRankView = this.mRankListBox.ItemsSource;
			this.mSelectedSp.SetActive(false);
			UIEventListener.Get(this.mSelectBoxRoot).onClick = delegate(GameObject g)
			{
				if (this.mSelectedSp.activeSelf)
				{
					this.mSelectedSp.SetActive(false);
					this.mSelectBoxHaveSelect = 0;
				}
				else
				{
					this.mSelectedSp.SetActive(true);
					this.mSelectBoxHaveSelect = 1;
				}
				base.StopCoroutine("RefreshRankItem");
				base.StartCoroutine<bool>(this.RefreshRankItem());
			};
			this.mRefreshBtn.gameObject.SetActive(false);
			this.mRefreshBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
			};
			this.ChangeBtnState(this.mBiddingBtn, true, (!Global.IsBangHuiLeader(Global.Data.roleData, Global.Data.roleData.Faction)) ? Global.GetLang("查看") : Global.GetLang("竞价"));
			this.mBiddingBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				int num = 0;
				int i = 0;
				int count = this.mOBCRankView.Count;
				while (i < count)
				{
					GameObject at = this.mOBCRankView.GetAt(i);
					if (null != at)
					{
						KuaFuPlunderSeverListItem component = at.GetComponent<KuaFuPlunderSeverListItem>();
						if (null != component && component.BSlelect)
						{
							num = component.ServerID;
						}
					}
					i++;
				}
				if (0 < num)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 1510,
						MyID = num
					});
				}
				else
				{
					Super.HintMainText(Global.GetLang("请选中需竞价的服务器"), 10, 3);
				}
			};
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void ItemClick(object sender, DPSelectedItemEventArgs s)
	{
		int i = 0;
		int count = this.mOBCRankView.Count;
		while (i < count)
		{
			GameObject at = this.mOBCRankView.GetAt(i);
			if (null != at)
			{
				if (!(at.name == s.ID.ToString()))
				{
					KuaFuPlunderSeverListItem component = at.GetComponent<KuaFuPlunderSeverListItem>();
					if (null != component)
					{
						component.BSlelect = false;
					}
				}
			}
			i++;
		}
	}

	private KuaFuPlunderSeverListItem GetViewItem(int index)
	{
		KuaFuPlunderSeverListItem kuaFuPlunderSeverListItem = null;
		GameObject at = this.mOBCRankView.GetAt(index);
		if (null != at)
		{
			kuaFuPlunderSeverListItem = at.GetComponent<KuaFuPlunderSeverListItem>();
			if (null == kuaFuPlunderSeverListItem)
			{
				Object.Destroy(at);
			}
		}
		if (null == kuaFuPlunderSeverListItem)
		{
			kuaFuPlunderSeverListItem = U3DUtils.NEW<KuaFuPlunderSeverListItem>();
			this.mOBCRankView.AddNoUpdate(kuaFuPlunderSeverListItem);
			kuaFuPlunderSeverListItem.DraggablePanel = this.mUIDraggablePanel;
			kuaFuPlunderSeverListItem.Hander = new DPSelectedItemEventHandler(this.ItemClick);
			kuaFuPlunderSeverListItem.name = index.ToString();
		}
		kuaFuPlunderSeverListItem.gameObject.SetActive(true);
		return kuaFuPlunderSeverListItem;
	}

	private IEnumerator RefreshRankItem()
	{
		Super.ShowNetWaiting(null);
		List<KuaFuLueDuoServerJingJiaState> list = null;
		if (this.mKuaFuLueDuoMainInfo != null)
		{
			if (this.mSelectBoxHaveSelect == 1)
			{
				list = this.mKuaFuLueDuoMainInfo.CloneStateList.FindAll((KuaFuLueDuoServerJingJiaState e) => e.State == 1);
			}
			else
			{
				list = this.mKuaFuLueDuoMainInfo.CloneStateList;
			}
		}
		list = this.Sort(list);
		if (list != null)
		{
			int i = 0;
			int max = list.Count;
			while (i < max)
			{
				KuaFuPlunderSeverListItem item = this.GetViewItem(i);
				int haveOffer = 0;
				if (list[i].State == 1)
				{
					if (this.mKuaFuLueDuoMainInfo.JingJiaData != null && 0 < this.mKuaFuLueDuoMainInfo.JingJiaData.ZiJin && list[i].ServerId == this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId)
					{
						haveOffer = 1;
					}
				}
				else if (list[i].State == 2 && this.mKuaFuLueDuoMainInfo.JingJiaData != null && 0 < this.mKuaFuLueDuoMainInfo.JingJiaData.ZiJin && list[i].ServerId == this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId)
				{
					haveOffer = 2;
				}
				KuaFuLueDuoServerInfo serverInf = this.mKuaFuLueDuoMainInfo.GetKuaFuLueDuoServerInfoDataByID(list[i].ServerId);
				item.SetInf(haveOffer, list[i].ServerId, list[i].State, list[i].Round, list[i].ZiYuan, this.ValueIsInList(serverInf.ZhengFuList), this.ValueIsInList(serverInf.ShiChouList));
				if (++i % 3 == 0)
				{
					yield return null;
				}
			}
			if (list.Count < this.mOBCRankView.Count)
			{
				for (int j = list.Count; j < this.mOBCRankView.Count; j++)
				{
					GameObject obj = this.mOBCRankView.GetAt(j);
					if (null != obj)
					{
						obj.SetActive(false);
					}
				}
			}
			this.mRankListBox.repositionNow = true;
		}
		SpringPanel.Begin(this.mUIDraggablePanel.gameObject, new Vector3(0f, 0f, 0f), 10f);
		Super.HideNetWaiting();
		yield break;
	}

	private List<KuaFuLueDuoServerJingJiaState> Sort(List<KuaFuLueDuoServerJingJiaState> list)
	{
		List<KuaFuLueDuoServerJingJiaState> list2 = new List<KuaFuLueDuoServerJingJiaState>();
		try
		{
			List<KuaFuLueDuoServerJingJiaState> list3 = new List<KuaFuLueDuoServerJingJiaState>();
			int i = list.Count - 1;
			int num = 0;
			while (i >= num)
			{
				if (list[i].State == 2)
				{
					list3.Add(list[i]);
					list.RemoveAt(i);
				}
				i--;
			}
			if (0 < list3.Count)
			{
				list3.Sort(delegate(KuaFuLueDuoServerJingJiaState b, KuaFuLueDuoServerJingJiaState a)
				{
					if (b.ZiYuan == a.ZiYuan)
					{
						return b.ServerId - a.ServerId;
					}
					return -(b.ZiYuan - a.ZiYuan);
				});
				list2.AddRange(list3);
				list3.Clear();
			}
			int j = list.Count - 1;
			int num2 = 0;
			while (j >= num2)
			{
				if (list[j].State == 1 && this.mKuaFuLueDuoMainInfo != null && this.mKuaFuLueDuoMainInfo.JingJiaData != null && this.mKuaFuLueDuoMainInfo.JingJiaData.ServerId == list[j].ServerId)
				{
					list3.Add(list[j]);
					list.RemoveAt(j);
				}
				j--;
			}
			if (0 < list3.Count)
			{
				list3.Sort(delegate(KuaFuLueDuoServerJingJiaState b, KuaFuLueDuoServerJingJiaState a)
				{
					if (b.ZiYuan == a.ZiYuan)
					{
						return b.ServerId - a.ServerId;
					}
					return -(b.ZiYuan - a.ZiYuan);
				});
				list2.AddRange(list3);
				list3.Clear();
			}
			int k = list.Count - 1;
			int num3 = 0;
			while (k >= num3)
			{
				if (list[k].State == 1)
				{
					list3.Add(list[k]);
					list.RemoveAt(k);
				}
				k--;
			}
			if (0 < list3.Count)
			{
				list3.Sort(delegate(KuaFuLueDuoServerJingJiaState b, KuaFuLueDuoServerJingJiaState a)
				{
					if (b.ZiYuan == a.ZiYuan)
					{
						return b.ServerId - a.ServerId;
					}
					return -(b.ZiYuan - a.ZiYuan);
				});
				list2.AddRange(list3);
				list3.Clear();
			}
			list.Sort(delegate(KuaFuLueDuoServerJingJiaState a, KuaFuLueDuoServerJingJiaState b)
			{
				if (b.ZiYuan == a.ZiYuan)
				{
					return b.ServerId - a.ServerId;
				}
				return -(b.ZiYuan - a.ZiYuan);
			});
			int l = list.Count - 1;
			int num4 = 0;
			while (l >= num4)
			{
				list2.Add(list[l]);
				l--;
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
		return list2;
	}

	private bool SeverIsInRank(List<KuaFuLueDuoBangHuiJingJiaData> lst, int Value, int Rank)
	{
		int num = 0;
		try
		{
			for (int i = 0; i < lst.Count; i++)
			{
				if (lst[i].ZiJin > Value)
				{
					num++;
					if (num >= Rank)
					{
						return false;
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
		return false;
	}

	private bool ValueIsInList(List<int> list)
	{
		if (list != null)
		{
			foreach (int num in list)
			{
				if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && num == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	internal void NoticeJionCallBack(int ret)
	{
	}

	public void NoticeServerDataEX(KuaFuLueDuoMainInfo data)
	{
		bool flag = false;
		if (data != null)
		{
			if (this.mKuaFuLueDuoMainInfo != null)
			{
				if (data.ServerListAge != this.mKuaFuLueDuoMainInfo.ServerListAge)
				{
					this.mKuaFuLueDuoMainInfo.ServerList = data.ServerList;
					this.mKuaFuLueDuoMainInfo.ServerListAge = data.ServerListAge;
					flag = true;
				}
				if (data.StateListAge != this.mKuaFuLueDuoMainInfo.StateListAge)
				{
					this.mKuaFuLueDuoMainInfo.StateList = data.StateList;
					this.mKuaFuLueDuoMainInfo.JingJiaData = data.JingJiaData;
					this.mKuaFuLueDuoMainInfo.StateListAge = data.StateListAge;
					flag = true;
				}
			}
			else
			{
				this.mKuaFuLueDuoMainInfo = data;
				flag = true;
			}
		}
		if (flag)
		{
			this.StartTimeTicks();
			base.StartCoroutine<bool>(this.RefreshRankItem());
		}
		if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null)
		{
			this.mKuaFuPlunderGameStateType = KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.GameState;
		}
	}

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private ShowNetImage mBgImage1;

	[SerializeField]
	private UILabel[] mTitlesLabel;

	[SerializeField]
	private UIDraggablePanel mUIDraggablePanel;

	[SerializeField]
	private ListBox mRankListBox;

	[SerializeField]
	private GameObject mSelectBoxRoot;

	[SerializeField]
	private UILabel mSelectLabel;

	[SerializeField]
	private GameObject mSelectedSp;

	[SerializeField]
	private GButton mRefreshBtn;

	[SerializeField]
	private GButton mBiddingBtn;

	[SerializeField]
	private UILabel mTimeLabel;

	private ObservableCollection mOBCRankView;

	private byte mSelectBoxHaveSelect;

	private KuaFuLueDuoMainInfo mKuaFuLueDuoMainInfo;

	private DispatcherTimer mDispatcherTimer;

	private KuaFuLueDuoGameStates mKuaFuPlunderGameStateType;

	private CrusadeWarXml mCrusadeWarXml;

	public DPSelectedItemEventHandler Hander;

	private class SynchronousData
	{
		public SynchronousData()
		{
			this.Synchronous = true;
		}

		public bool Synchronous
		{
			set
			{
				if (!this.mSynchronous)
				{
					this.mSynchronous = value;
					this.mSynchronousTime = 5;
				}
				if (!value)
				{
					this.mSynchronous = value;
				}
			}
		}

		public bool UpDate()
		{
			if (!this.mSynchronous)
			{
				return false;
			}
			this.mSynchronousTime--;
			if (0 > this.mSynchronousTime)
			{
				this.Synchronous = false;
				return true;
			}
			return false;
		}

		public void SenderGetData()
		{
			GameInstance.Game.SendGetKuFuPlubderGameStateData(-1L);
		}

		private int mSynchronousTime;

		private bool mSynchronous;
	}
}
