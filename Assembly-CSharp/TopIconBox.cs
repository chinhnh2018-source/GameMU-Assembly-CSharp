using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TopIconBox : UserControl
{
	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 11)
			{
				SystemHelpPart.SetMask(this.HuodongIcon, default(Vector4));
			}
			else if (id == 12)
			{
				SystemHelpPart.SetMask(this.FuBenIcon, default(Vector4));
			}
			else if (id == 13)
			{
				SystemHelpPart.SetMask(this.FuliIcon, default(Vector4));
			}
			else if (id == 14)
			{
				SystemHelpPart.SetMask(this.JingJiChangIcon, default(Vector4));
			}
			else if (id != 15)
			{
				if (id == 16)
				{
					SystemHelpPart.SetMask(this.XinFuIcon, default(Vector4));
				}
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected override void InitializeComponent()
	{
		TopIconBox.ActivityTipDic = new Dictionary<ActivityTipTypes, bool>();
		Vector3 localPosition = base.transform.localPosition;
		localPosition.z = -1f;
		base.transform.localPosition = localPosition;
		this.SwitchIconBak = this.SwitchIcon.GetComponentInChildren<UISprite>();
		this.SwitchIconBak.spriteName = this.SwitchIconBakNames[1];
		if (SceneUIClasses.RebornMap.IsTheScene() && this.BodyVisible)
		{
			this.BodyVisible = false;
		}
		if (!this.BodyVisible)
		{
			this.SwitchIconBak.spriteName = this.SwitchIconBakNames[0];
		}
		else
		{
			this.SwitchIconBak.spriteName = this.SwitchIconBakNames[1];
		}
		UIEventListener.Get(this.SwitchIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				if (this.BodyVisible)
				{
					this.BodyVisible = false;
					this.SwitchIconBak.spriteName = this.SwitchIconBakNames[0];
				}
				Super.HintMainText(Global.GetLang("当前地图无法使用此功能"), 10, 3);
			}
			else
			{
				this.BodyVisible = !this.BodyVisible;
				if (this.BodyVisible)
				{
					this.SwitchIconBak.spriteName = this.SwitchIconBakNames[1];
				}
				else
				{
					this.SwitchIconBak.spriteName = this.SwitchIconBakNames[0];
				}
			}
		};
		if (null != this.m_BtnChongZhi)
		{
			UIEventListener.Get(this.m_BtnChongZhi.gameObject).onClick = delegate(GameObject s)
			{
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
					return;
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 7 + Convert.ToInt32(this.m_BtnChongZhi.tag)
				});
			};
		}
		UIEventListener.Get(this.FuliIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			SystemHelpMgr.OnAction(UIObjIDs.FuLi, HelpStateEvents.Clicked, -1);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.HuodongIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			SystemHelpMgr.OnAction(UIObjIDs.HuoDong, HelpStateEvents.Clicked, -1);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		UIEventListener.Get(this.FuBenIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		};
		UIEventListener.Get(this.JingJiChangIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 4
			});
		};
		UIEventListener.Get(this.XinFuIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 5,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.GongnengIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 6,
				Tag = this.GongnengIcon.transform
			});
		};
		UIEventListener.Get(this.HefuIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 10,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.JieRiIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 11,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.PlayerRecallIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 20,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.QiRiKuangHuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 21,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.TuiGuangIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 22,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.ChengzhangjijinIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 23
			});
		};
		UIEventListener.Get(this.ZhuanXiangIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 24,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.olympicsIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 26,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.ChaoJiYiYuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 28,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.XingYunYiYuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 29,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.EveryDayIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 30,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.JiYuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 31,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.ZhuTiFuIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 32,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.EveryFanLiIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 33,
				Tag = this.JieRiIcon.transform
			});
		};
		UIEventListener.Get(this.CaiShuZiIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 34,
				Tag = this.CaiShuZiIcon.transform
			});
		};
		UIEventListener.Get(this.CaiDaXiaoIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 35,
				Tag = this.CaiDaXiaoIcon.transform
			});
		};
		UIEventListener.Get(this.TeQuanHuoDongIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 122,
				Tag = this.TeQuanHuoDongIcon.transform
			});
		};
		UIEventListener.Get(this.HuiGuiHuoDongIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 123,
				Tag = this.HuiGuiHuoDongIcon.transform
			});
		};
		UIEventListener.Get(this.MengHuanStoneIcon.gameObject).onClick = delegate(GameObject s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 37,
				Tag = this.MengHuanStoneIcon.transform
			});
		};
		NGUITools.SetActive(this.m_IDBindingIcon, false);
		ActivityTipManager.RegActivityTipItem(1000, delegate(int s, ActivityTipItem e)
		{
			this._HuoDongTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(2000, delegate(int s, ActivityTipItem e)
		{
			this._FuBenTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(3000, delegate(int s, ActivityTipItem e)
		{
			this._FuLiTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(4000, delegate(int s, ActivityTipItem e)
		{
			this._JingJiChangTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(5000, delegate(int s, ActivityTipItem e)
		{
			this._GongNengTipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(18000, new ActivityTipEventHandler(this.AoYunDaTiActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(20000, new ActivityTipEventHandler(this.OlympicsActivityTipEventHandler));
		this.InitAcitivityIcons();
		ActivityTipManager.RegActivityTipItem(3011, delegate(int s, ActivityTipItem e)
		{
			this.GetChongBtnState();
		});
		ActivityTipManager.RegActivityTipItem(3012, delegate(int s, ActivityTipItem e)
		{
			this.GetChongBtnState();
		});
		ActivityTipManager.RegActivityTipItem(3002, new ActivityTipEventHandler(this.ActivityTipEventShouCiHandler));
		ActivityTipManager.RegActivityTipItem(3003, new ActivityTipEventHandler(this.ActivityTipEventMeiRiHandler));
		this.StartTimer();
	}

	private void CheckIdBindingData(int ChenckCount = 4)
	{
		base.StopCoroutine("SendData");
		this.CheckIdBindingDataCount = ChenckCount;
		this.CheckTime = ((ChenckCount != 2) ? 0.5f : 1f);
		base.StartCoroutine<bool>(this.SendData());
	}

	private IEnumerator SendData()
	{
		if (!Global.Data.PlayGame)
		{
			yield return null;
		}
		for (int i = 0; i < this.CheckIdBindingDataCount; i++)
		{
			yield return new WaitForSeconds(this.CheckTime);
			this.CheckTime *= 3f;
			this.ChangeIDBindingState(Global.DetectionIDBinding());
		}
		yield break;
	}

	private void ActivityTipEventShouCiHandler(int type, ActivityTipItem args)
	{
		this.SetChongZhiIcon();
	}

	private void ActivityTipEventMeiRiHandler(int type, ActivityTipItem args)
	{
		this.SetChongZhiIcon();
	}

	private void SetChongZhiIcon()
	{
		this.m_ChongZhiIcon.gameObject.SetActive(true);
		ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(3002);
		ActivityTipItem activityTipItem2 = ActivityTipManager.GetActivityTipItem(3003);
		if (!activityTipItem.IsActive && !activityTipItem2.IsActive)
		{
			this.m_ChongZhiIcon.gameObject.SetActive(false);
		}
	}

	private int GetChongBtnState()
	{
		ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(3011);
		if (!activityTipItem.IsActive)
		{
			this.m_BtnChongZhi.tag = Global.GetLang("0");
			this.m_SprChongZhi.spriteName = "mainFirstChongzhi";
			return 0;
		}
		ActivityTipItem activityTipItem2 = ActivityTipManager.GetActivityTipItem(3012);
		if (!activityTipItem2.IsActive)
		{
			this.m_BtnChongZhi.tag = Global.GetLang("1");
			this.m_SprChongZhi.spriteName = ((!Global.IsInWeekendRechargePeriod()) ? "mainMeiriChongzhi" : "mainWeekendDeposit");
			return 1;
		}
		if (activityTipItem2.IsActive && activityTipItem.IsActive)
		{
			this.m_ChongZhiIcon.gameObject.SetActive(false);
		}
		this.m_BtnChongZhi.tag = Global.GetLang("2");
		this.m_SprChongZhi.spriteName = "mainChongzhi";
		return 2;
	}

	protected new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(1000, null);
		ActivityTipManager.RegActivityTipItem(2000, null);
		ActivityTipManager.RegActivityTipItem(3000, null);
		ActivityTipManager.RegActivityTipItem(4000, null);
		ActivityTipManager.RegActivityTipItem(5000, null);
		ActivityTipManager.RegActivityTipItem(6000, null);
		ActivityTipManager.RegActivityTipItem(12000, null);
		ActivityTipManager.RegActivityTipItem(14000, new ActivityTipEventHandler(this.JieRiActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(3011, null);
		ActivityTipManager.RegActivityTipItem(3012, null);
		ActivityTipManager.UnRegActivityTipItem(3002, new ActivityTipEventHandler(this.ActivityTipEventShouCiHandler));
		ActivityTipManager.UnRegActivityTipItem(3003, new ActivityTipEventHandler(this.ActivityTipEventMeiRiHandler));
		ActivityTipManager.UnRegActivityTipItem(14100, new ActivityTipEventHandler(this.PlayerRecallActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(17000, null);
		ActivityTipManager.UnRegActivityTipItem(14105, new ActivityTipEventHandler(this.TuiGuangActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14109, null);
		ActivityTipManager.UnRegActivityTipItem(15051, new ActivityTipEventHandler(this.XingYunYiYuanActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15052, new ActivityTipEventHandler(this.ChaoJiYiYuanActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14110, new ActivityTipEventHandler(this.ZhuanxiangActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14114, new ActivityTipEventHandler(this.EveryDayActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(20000, new ActivityTipEventHandler(this.OlympicsActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(18000, new ActivityTipEventHandler(this.AoYunDaTiActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15053, new ActivityTipEventHandler(this.JiYuanActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(11502, new ActivityTipEventHandler(this.ZhuTiFuActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(11501, new ActivityTipEventHandler(this.ZhuTiFuActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15054, new ActivityTipEventHandler(this.EveryFanLiTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14115, new ActivityTipEventHandler(this.SpecPActAwardActivityTipEventHandler));
		base.OnDestroy();
	}

	public bool VisibleSelf
	{
		set
		{
		}
	}

	public bool BodyVisible
	{
		get
		{
			return this.Body.Visibility;
		}
		set
		{
			this.Body.Visibility = value;
		}
	}

	public void ChangeIDBindingState(bool bShow = true)
	{
		NGUITools.SetActive(this.m_IDBindingIcon, false);
	}

	private void UpdateMergedActivityIcons()
	{
		LinkedList<string> linkedList = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
		if (linkedList.Contains("EveryDayFanLi"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(true);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("HuiGuiHuoDong"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(true);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("ChaoJiYiYuan"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(true);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("XingYunYiYuan"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(true);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("TeQuanHuoDong"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(true);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("JieRi"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(true);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("ZhuanShu"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(true);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("ZhuTiFu"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(true);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon.GetComponentInChildren<ShowNetImage>() != null)
			{
				this.ZhuTiFuIcon.GetComponentInChildren<ShowNetImage>().URL = Global.GetZhuTiFuNetImg("Logo", this.ZhuTiFuIcon.GetComponentInChildren<ShowNetImage>().URL);
			}
			return;
		}
		if (linkedList.Contains("EveryDay"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(true);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("JiYuan"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(true);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("CaiShuZi"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(true);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("CaiDaXiao"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(true);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("ZhaoHui"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(true);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("TuiGuang"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(true);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(false);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("Olympics"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(true);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(false);
			}
			return;
		}
		if (linkedList.Contains("MengHuanStone"))
		{
			if (this.XingYunYiYuanIcon)
			{
				this.XingYunYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.ChaoJiYiYuanIcon)
			{
				this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
			}
			if (this.EveryFanLiIcon)
			{
				this.EveryFanLiIcon.gameObject.SetActive(false);
			}
			if (this.JieRiIcon)
			{
				this.JieRiIcon.gameObject.SetActive(false);
			}
			if (this.AoYunDaTiIcon)
			{
				this.AoYunDaTiIcon.gameObject.SetActive(false);
			}
			if (this.ZhuanXiangIcon)
			{
				this.ZhuanXiangIcon.gameObject.SetActive(false);
			}
			if (this.ZhuTiFuIcon)
			{
				this.ZhuTiFuIcon.gameObject.SetActive(false);
			}
			if (this.EveryDayIcon)
			{
				this.EveryDayIcon.gameObject.SetActive(false);
			}
			if (this.PlayerRecallIcon)
			{
				this.PlayerRecallIcon.gameObject.SetActive(false);
			}
			if (this.TuiGuangIcon)
			{
				this.TuiGuangIcon.gameObject.SetActive(false);
			}
			if (this.olympicsIcon)
			{
				this.olympicsIcon.gameObject.SetActive(true);
			}
			if (this.JiYuanIcon)
			{
				this.JiYuanIcon.gameObject.SetActive(false);
			}
			if (this.TeQuanHuoDongIcon)
			{
				this.TeQuanHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.HuiGuiHuoDongIcon)
			{
				this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
			}
			if (this.CaiShuZiIcon)
			{
				this.CaiShuZiIcon.gameObject.SetActive(false);
			}
			if (this.CaiDaXiaoIcon)
			{
				this.CaiDaXiaoIcon.gameObject.SetActive(false);
			}
			if (this.MengHuanStoneIcon)
			{
				this.MengHuanStoneIcon.gameObject.SetActive(true);
			}
			return;
		}
		if (this.XingYunYiYuanIcon)
		{
			this.XingYunYiYuanIcon.gameObject.SetActive(false);
		}
		if (this.EveryFanLiIcon)
		{
			this.EveryFanLiIcon.gameObject.SetActive(false);
		}
		if (this.JieRiIcon)
		{
			this.JieRiIcon.gameObject.SetActive(false);
		}
		if (this.AoYunDaTiIcon)
		{
			this.AoYunDaTiIcon.gameObject.SetActive(false);
		}
		if (this.PlayerRecallIcon)
		{
			this.PlayerRecallIcon.gameObject.SetActive(false);
		}
		if (this.TuiGuangIcon)
		{
			this.TuiGuangIcon.gameObject.SetActive(false);
		}
		if (this.ZhuanXiangIcon)
		{
			this.ZhuanXiangIcon.gameObject.SetActive(false);
		}
		if (this.ZhuTiFuIcon)
		{
			this.ZhuTiFuIcon.gameObject.SetActive(false);
		}
		if (this.ZhuTiFuIcon)
		{
			this.ZhuTiFuIcon.gameObject.SetActive(false);
		}
		if (this.EveryDayIcon)
		{
			this.EveryDayIcon.gameObject.SetActive(false);
		}
		if (this.olympicsIcon)
		{
			this.olympicsIcon.gameObject.SetActive(false);
		}
		if (this.ChaoJiYiYuanIcon)
		{
			this.ChaoJiYiYuanIcon.gameObject.SetActive(false);
		}
		if (this.JiYuanIcon)
		{
			this.JiYuanIcon.gameObject.SetActive(false);
		}
		if (this.TeQuanHuoDongIcon)
		{
			this.TeQuanHuoDongIcon.gameObject.SetActive(false);
		}
		if (this.CaiShuZiIcon)
		{
			this.CaiShuZiIcon.gameObject.SetActive(false);
		}
		if (this.CaiDaXiaoIcon)
		{
			this.CaiDaXiaoIcon.gameObject.SetActive(false);
		}
		if (this.MengHuanStoneIcon)
		{
			this.MengHuanStoneIcon.gameObject.SetActive(false);
		}
	}

	public void SetIcon()
	{
		this.UpdateMergedXinFuActivityIcons();
		this.UpdateMergedActivityIcons();
	}

	private void InitAcitivityIcons()
	{
		this.UpdateMergedActivityIcons();
		this.UpdateMergedXinFuActivityIcons();
		ActivityTipManager.RegActivityTipItem(12000, delegate(int s, ActivityTipItem e)
		{
			TopIconBox.ActivityTipDic[ActivityTipTypes.HeFuActivity] = e.IsActive;
			if (!this.HefuIcon)
			{
				return;
			}
			this.UpdateMergedXinFuActivityTips();
		});
		ActivityTipManager.RegActivityTipItem(6000, delegate(int s, ActivityTipItem e)
		{
			TopIconBox.ActivityTipDic[ActivityTipTypes.MainXinFuIcon] = e.IsActive;
			if (!this.XinFuIcon)
			{
				return;
			}
			this.UpdateMergedXinFuActivityTips();
		});
		ActivityTipManager.RegActivityTipItem(17000, delegate(int s, ActivityTipItem e)
		{
			TopIconBox.ActivityTipDic[ActivityTipTypes.SevenDayActivity] = e.IsActive;
			if (!this.QiRiKuangHuanTrans)
			{
				return;
			}
			this.UpdateMergedXinFuActivityTips();
		});
		ActivityTipManager.RegActivityTipItem(14109, delegate(int s, ActivityTipItem e)
		{
			TopIconBox.ActivityTipDic[ActivityTipTypes.Fund] = e.IsActive;
			if (!this.ChengzhangjijinTrans)
			{
				return;
			}
			this.UpdateMergedXinFuActivityTips();
		});
		ActivityTipManager.RegActivityTipItem(14000, new ActivityTipEventHandler(this.JieRiActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14100, new ActivityTipEventHandler(this.PlayerRecallActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14105, new ActivityTipEventHandler(this.TuiGuangActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14110, new ActivityTipEventHandler(this.ZhuanxiangActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14114, new ActivityTipEventHandler(this.EveryDayActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(15052, new ActivityTipEventHandler(this.ChaoJiYiYuanActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(15051, new ActivityTipEventHandler(this.XingYunYiYuanActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(15053, new ActivityTipEventHandler(this.JiYuanActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(11501, new ActivityTipEventHandler(this.ZhuTiFuActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(11502, new ActivityTipEventHandler(this.ZhuTiFuActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(15054, new ActivityTipEventHandler(this.EveryFanLiTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14115, new ActivityTipEventHandler(this.SpecPActAwardActivityTipEventHandler));
	}

	private void SpecPActAwardActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.TeQuanHuoDongTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void PlayerRecallActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.PlayerRecallTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void TuiGuangActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.TuiGuangTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void XingYunYiYuanActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.XingYunYiYuanTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void ChaoJiYiYuanActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.ChaoJiYiYuanIcon)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void JieRiActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this._JieRiIcon)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void ZhuanxiangActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.ZhuanXiangIcon)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void EveryDayActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.EveryDayIcon)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void OlympicsActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.olympicsTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void AoYunDaTiActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.AoYunDaTiTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void JiYuanActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.JiYuanTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void EveryFanLiTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.EveryFanLiTrans)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void ZhuTiFuActivityTipEventHandler(int type, ActivityTipItem args)
	{
		TopIconBox.ActivityTipDic[(ActivityTipTypes)type] = args.IsActive;
		if (!this.ZhuTiFuIcon)
		{
			return;
		}
		this.UpdateActivityIconTip();
	}

	private void UpdateActivityIconTip()
	{
		this.UpdateMergedActivityIcons();
		LinkedList<string> linkedList = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
		if (linkedList.Count > 0)
		{
			bool active = false;
			if (linkedList.Contains("HuiGuiHuoDong") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.TriennialRegressOpen) && TopIconBox.ActivityTipDic[ActivityTipTypes.TriennialRegressOpen])
			{
				active = true;
			}
			if (linkedList.Contains("XingYunYiYuan") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.OneDollarChongZhi) && TopIconBox.ActivityTipDic[ActivityTipTypes.OneDollarChongZhi])
			{
				active = true;
			}
			if (linkedList.Contains("ChaoJiYiYuan") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.ChaoJiYiYuan) && TopIconBox.ActivityTipDic[ActivityTipTypes.ChaoJiYiYuan])
			{
				active = true;
			}
			if (linkedList.Contains("EveryFanLi") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.InputFanLiNew))
			{
				active = true;
			}
			if (linkedList.Contains("JieRi") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.JieRiActivity) && TopIconBox.ActivityTipDic[ActivityTipTypes.JieRiActivity])
			{
				active = true;
			}
			if (linkedList.Contains("ZhuanShu") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.ZhuanXiang) && TopIconBox.ActivityTipDic[ActivityTipTypes.ZhuanXiang])
			{
				active = true;
			}
			if (linkedList.Contains("ZhuTiFu") && ((TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.ThemeZhiGou) && TopIconBox.ActivityTipDic[ActivityTipTypes.ThemeZhiGou]) || (TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.ThemeDaLiBao) && TopIconBox.ActivityTipDic[ActivityTipTypes.ThemeDaLiBao])))
			{
				active = true;
			}
			if (linkedList.Contains("EveryDay") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.EveryDayAct) && TopIconBox.ActivityTipDic[ActivityTipTypes.EveryDayAct])
			{
				active = true;
			}
			if (linkedList.Contains("ZhaoHui") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.PlayerRecallActivity) && TopIconBox.ActivityTipDic[ActivityTipTypes.PlayerRecallActivity])
			{
				active = true;
			}
			if (linkedList.Contains("TuiGuang") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.TipSpread) && TopIconBox.ActivityTipDic[ActivityTipTypes.TipSpread])
			{
				active = true;
			}
			if (linkedList.Contains("Olympics") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.OlympicsActivity) && TopIconBox.ActivityTipDic[ActivityTipTypes.OlympicsActivity])
			{
				active = true;
			}
			if (linkedList.Contains("JiYuan") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.JunTuanEra) && TopIconBox.ActivityTipDic[ActivityTipTypes.JunTuanEra])
			{
				active = true;
			}
			if (linkedList.Contains("AoYunDaTi") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.AoYunDaTi) && TopIconBox.ActivityTipDic[ActivityTipTypes.AoYunDaTi])
			{
				active = true;
			}
			if (linkedList.Contains(string.Empty) && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.SpecPActAward) && TopIconBox.ActivityTipDic[ActivityTipTypes.SpecPActAward])
			{
				active = true;
			}
			if (null != this.TeQuanHuoDongTrans)
			{
				this.TeQuanHuoDongTrans.gameObject.SetActive(false);
			}
			if (this._JieRiIcon)
			{
				this._JieRiIcon.gameObject.SetActive(active);
			}
			if (this.AoYunDaTiTrans)
			{
				this.AoYunDaTiTrans.gameObject.SetActive(active);
			}
			if (this.PlayerRecallTrans)
			{
				this.PlayerRecallTrans.gameObject.SetActive(active);
			}
			if (this.TuiGuangTrans)
			{
				this.TuiGuangTrans.gameObject.SetActive(active);
			}
			if (this.ZhuanXiangTrans)
			{
				this.ZhuanXiangTrans.gameObject.SetActive(active);
			}
			if (this.EveryDayTrans)
			{
				this.EveryDayTrans.gameObject.SetActive(active);
			}
			if (this.olympicsTrans)
			{
				this.olympicsTrans.gameObject.SetActive(active);
			}
			if (this.ChaoJiYiYuanTrans)
			{
				this.ChaoJiYiYuanTrans.gameObject.SetActive(active);
			}
			if (this.XingYunYiYuanTrans)
			{
				this.XingYunYiYuanTrans.gameObject.SetActive(active);
			}
			if (this.JiYuanTrans)
			{
				this.JiYuanTrans.gameObject.SetActive(active);
			}
			if (this.ZhuTiFuTrans)
			{
				this.ZhuTiFuTrans.gameObject.SetActive(active);
			}
			if (this.EveryFanLiTrans)
			{
				this.EveryFanLiTrans.gameObject.SetActive(active);
			}
		}
	}

	private void UpdateMergedXinFuActivityIcons()
	{
		LinkedList<string> topIconBoxDownRightIconIsOpen = HuoDongCommonFlag.GetTopIconBoxDownRightIconIsOpen();
		if (this.XinFuIcon)
		{
			this.XinFuIcon.gameObject.SetActive(false);
		}
		if (this.HefuIcon)
		{
			this.HefuIcon.gameObject.SetActive(false);
		}
		if (this.QiRiKuangHuanIcon)
		{
			this.QiRiKuangHuanIcon.gameObject.SetActive(false);
		}
		if (this.ChengzhangjijinIcon)
		{
			this.ChengzhangjijinIcon.gameObject.SetActive(false);
		}
		if (null != this.m_IDBindingIcon)
		{
			this.m_IDBindingIcon.gameObject.SetActive(false);
		}
		if (topIconBoxDownRightIconIsOpen.Contains("XinFu"))
		{
			this.XinFuIcon.gameObject.SetActive(true);
			if (this.LoadXinFuObj && this.OpenDoubleXinFuTime())
			{
				string path = "UITeXiao/Perfabs/Jiemianhuodongtishi/Xinfuhuodong_UI";
				GameObject gameObject = this.LoadTeXiaoObj(path, this.XinFuIcon.transform);
				if (gameObject)
				{
					gameObject.transform.localPosition = new Vector3(-77.5f, -163f, -10f);
					this.LoadXinFuObj = false;
				}
			}
			return;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("Hefu"))
		{
			this.HefuIcon.gameObject.SetActive(true);
			return;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("QiRiKuanghuan"))
		{
			this.QiRiKuangHuanIcon.gameObject.SetActive(true);
			return;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("Fund"))
		{
			this.ChengzhangjijinIcon.gameObject.SetActive(true);
			return;
		}
	}

	private void UpdateMergedXinFuActivityTips()
	{
		this.UpdateMergedXinFuActivityIcons();
		LinkedList<string> topIconBoxDownRightIconIsOpen = HuoDongCommonFlag.GetTopIconBoxDownRightIconIsOpen();
		if (0 >= topIconBoxDownRightIconIsOpen.Count)
		{
			return;
		}
		bool active = false;
		if (topIconBoxDownRightIconIsOpen.Contains("XinFu") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.MainXinFuIcon) && TopIconBox.ActivityTipDic[ActivityTipTypes.MainXinFuIcon])
		{
			active = true;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("Hefu") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.HeFuActivity) && TopIconBox.ActivityTipDic[ActivityTipTypes.HeFuActivity])
		{
			active = true;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("IDBinding"))
		{
			active = true;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("QiRiKuanghuan") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.SevenDayActivity) && TopIconBox.ActivityTipDic[ActivityTipTypes.SevenDayActivity])
		{
			active = true;
		}
		if (topIconBoxDownRightIconIsOpen.Contains("Fund") && TopIconBox.ActivityTipDic.ContainsKey(ActivityTipTypes.Fund) && TopIconBox.ActivityTipDic[ActivityTipTypes.Fund])
		{
			active = true;
		}
		if (this._XinFuTipIcon)
		{
			this._XinFuTipIcon.gameObject.SetActive(active);
		}
		if (this._HefuIcon)
		{
			this._HefuIcon.gameObject.SetActive(active);
		}
		if (this.QiRiKuangHuanTrans)
		{
			this.QiRiKuangHuanTrans.gameObject.SetActive(active);
		}
		if (this.ChengzhangjijinTrans)
		{
			this.ChengzhangjijinTrans.gameObject.SetActive(active);
		}
	}

	public void StartTimer()
	{
		this.StopTimer();
		this._Timer = new DispatcherTimer("TopIconIndex_Timer");
		this._Timer.Interval = TimeSpan.FromSeconds(2.0);
		this._Timer.Tick = new DispatcherTimerEventHandler(this.Timer_Tick);
		this._Timer.Start();
	}

	public void StopTimer()
	{
		if (this._Timer == null)
		{
			return;
		}
		this._Timer.Stop();
		this._Timer.Tick = null;
		this._Timer = null;
	}

	private void Timer_Tick(object sender, object e)
	{
		this.UpdateMergedActivityIcons();
	}

	private bool OpenDoubleXinFuTime()
	{
		DateTime correctDateTime = Global.GetCorrectDateTime();
		DateTime dateTime = default(DateTime);
		DateTime dateTime2 = default(DateTime);
		DateTime dateTime3 = default(DateTime);
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("DoubleXinFu", '|');
		if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length == 0)
		{
			return false;
		}
		DateTime.TryParse(string.Format("{0}", Global.Data.roleData.KaiFuStartDay), ref dateTime3);
		DateTime.TryParse(systemParamStringArrayByName[0], ref dateTime);
		DateTime.TryParse(systemParamStringArrayByName[1], ref dateTime2);
		return dateTime3.Ticks >= dateTime.Ticks && dateTime3.Ticks <= dateTime2.Ticks && dateTime3.AddDays(9.0).Ticks >= correctDateTime.Ticks;
	}

	private GameObject LoadTeXiaoObj(string Path, Transform parent)
	{
		Object @object = Resources.Load(Path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(parent, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			return gameObject;
		}
		return null;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL Body;

	public UIButton SwitchIcon;

	public UIButton FuliIcon;

	public UIButton HuodongIcon;

	public UIButton FuBenIcon;

	public UIButton MingXiangIcon;

	public UIButton JingJiChangIcon;

	public UIButton XinFuIcon;

	public UIButton GongnengIcon;

	public UIButton m_BtnChongZhi;

	public UIButton HefuIcon;

	public UIButton JieRiIcon;

	public Transform _HuoDongTipIcon;

	public Transform _FuBenTipIcon;

	public Transform _FuLiTipIcon;

	public Transform _JingJiChangTipIcon;

	public Transform _XinFuTipIcon;

	public Transform _GongNengTipIcon;

	public Transform SwitchIconTrans;

	public Transform m_ChongZhiIcon;

	public Transform _HefuIcon;

	public Transform _JieRiIcon;

	public UIButton PlayerRecallIcon;

	public Transform PlayerRecallTrans;

	public UIButton TuiGuangIcon;

	public Transform TuiGuangTrans;

	public UIButton ZhuanXiangIcon;

	public Transform ZhuanXiangTrans;

	public UIButton EveryDayIcon;

	public Transform EveryDayTrans;

	public UIButton ChaoJiYiYuanIcon;

	public Transform ChaoJiYiYuanTrans;

	public UIButton XingYunYiYuanIcon;

	public Transform XingYunYiYuanTrans;

	public UIButton MengHuanStoneIcon;

	public Transform MengHuanStoneTrans;

	public UIButton QiRiKuangHuanIcon;

	public Transform QiRiKuangHuanTrans;

	public UIButton ChengzhangjijinIcon;

	public Transform ChengzhangjijinTrans;

	public UISprite m_SprChongZhi;

	public UIButton m_IDBindingIcon;

	public Transform m_IDBindingTrans;

	public UIButton olympicsIcon;

	public Transform olympicsTrans;

	public UIButton AoYunDaTiIcon;

	public Transform AoYunDaTiTrans;

	public UIButton JiYuanIcon;

	public Transform JiYuanTrans;

	public UIButton ZhuTiFuIcon;

	public Transform ZhuTiFuTrans;

	public UIButton EveryFanLiIcon;

	public Transform EveryFanLiTrans;

	private UISprite SwitchIconBak;

	private string[] SwitchIconBakNames = new string[]
	{
		"mainShowTopIconBox",
		"mainHideTopIconBox"
	};

	public UIButton TeQuanHuoDongIcon;

	public Transform TeQuanHuoDongTrans;

	public UIButton HuiGuiHuoDongIcon;

	public Transform HuiGuiHuoDongTrans;

	public UIButton CaiShuZiIcon;

	public Transform CaiShuZiTrans;

	public UIButton CaiDaXiaoIcon;

	public Transform CaiDaXiaoTrans;

	public static Dictionary<ActivityTipTypes, bool> ActivityTipDic = new Dictionary<ActivityTipTypes, bool>();

	private int CheckIdBindingDataCount = 4;

	private float CheckTime = 0.5f;

	private DispatcherTimer _Timer;

	private bool LoadXinFuObj = true;
}
