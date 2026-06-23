using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MergedActivityIcon : UserControl
{
	protected override void InitializeComponent()
	{
		for (int i = 0; i < 24; i++)
		{
			Vector3 vector;
			vector..ctor((float)(-100 + i % 4 * 65), (float)(35 - i / 4 * 70), 0f);
			this.originalVecList.Add(vector);
		}
		int num = 0;
		LinkedList<string> linkedList = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
		if (linkedList.Contains("EveryDayFanLi"))
		{
			this.everyFanLiIcon.gameObject.SetActive(true);
			this.everyFanLiIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.everyFanLiIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("HuiGuiHuoDong"))
		{
			this.HuiGuiHuoDongIcon.gameObject.SetActive(true);
			this.HuiGuiHuoDongIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.HuiGuiHuoDongIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("TeQuanHuoDong"))
		{
			this.TeQuanHuoDongIcon.gameObject.SetActive(true);
			this.TeQuanHuoDongIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.TeQuanHuoDongIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("XingYunYiYuan"))
		{
			this.xingyunyiyuanIcon.gameObject.SetActive(true);
			this.xingyunyiyuanIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.xingyunyiyuanIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("ChaoJiYiYuan"))
		{
			this.chaojiyiyuanIcon.gameObject.SetActive(true);
			this.chaojiyiyuanIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.chaojiyiyuanIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("JieRi"))
		{
			this.activityIcon.gameObject.SetActive(true);
			this.activityIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.activityIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("ZhuanShu"))
		{
			this.zhuanxiangIcon.gameObject.SetActive(true);
			this.zhuanxiangIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.zhuanxiangIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("ZhuTiFu"))
		{
			this.zhuTiFuIcon.gameObject.SetActive(true);
			this.zhuTiFuIcon.gameObject.transform.localPosition = this.originalVecList[num++];
			if (this.zhuTiFuIcon.GetComponentInChildren<ShowNetImage>() != null)
			{
				this.zhuTiFuIcon.GetComponentInChildren<ShowNetImage>().URL = Global.GetZhuTiFuNetImg("Logo", this.zhuTiFuIcon.GetComponentInChildren<ShowNetImage>().URL);
			}
		}
		else
		{
			this.zhuTiFuIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("EveryDay"))
		{
			this.everyDayIcon.gameObject.SetActive(true);
			this.everyDayIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.everyDayIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("JiYuan"))
		{
			this.jiYuanIcon.gameObject.SetActive(true);
			this.jiYuanIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.jiYuanIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("AoYunDaTi"))
		{
			this.aoYunDaTiIcon.gameObject.SetActive(false);
		}
		else
		{
			this.aoYunDaTiIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("CaiShuZi"))
		{
			this.CaiShuZiIcon.gameObject.SetActive(true);
			this.CaiShuZiIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.CaiShuZiIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("CaiDaXiao"))
		{
			this.CaiDaXiaoIcon.gameObject.SetActive(true);
			this.CaiDaXiaoIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.CaiDaXiaoIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("ZhaoHui"))
		{
			this.playerRecallIcon.gameObject.SetActive(true);
			this.playerRecallIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.playerRecallIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("Olympics"))
		{
			this.olympicsIcon.gameObject.SetActive(false);
		}
		else
		{
			this.olympicsIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("TuiGuang"))
		{
			this.tuiGuangIcon.gameObject.SetActive(true);
			this.tuiGuangIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.tuiGuangIcon.gameObject.SetActive(false);
		}
		if (linkedList.Contains("MengHuanStone"))
		{
			this.MengHuanStoneIcon.gameObject.SetActive(true);
			this.MengHuanStoneIcon.gameObject.transform.localPosition = this.originalVecList[num++];
		}
		else
		{
			this.MengHuanStoneIcon.gameObject.SetActive(false);
		}
		UIEventListener.Get(this.activityIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.playerRecallIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
		};
		UIEventListener.Get(this.tuiGuangIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
		};
		UIEventListener.Get(this.zhuanxiangIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3
			});
		};
		UIEventListener.Get(this.everyDayIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 8
			});
		};
		UIEventListener.Get(this.chaojiyiyuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 6
			});
		};
		UIEventListener.Get(this.xingyunyiyuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 7
			});
		};
		UIEventListener.Get(this.olympicsIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 4
			});
		};
		UIEventListener.Get(this.aoYunDaTiIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 5
			});
		};
		UIEventListener.Get(this.jiYuanIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 9
			});
		};
		UIEventListener.Get(this.zhuTiFuIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 10
			});
		};
		UIEventListener.Get(this.everyFanLiIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 11
			});
		};
		UIEventListener.Get(this.CaiShuZiIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 12
			});
		};
		UIEventListener.Get(this.CaiDaXiaoIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 13
			});
		};
		UIEventListener.Get(this.TeQuanHuoDongIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 122
			});
		};
		UIEventListener.Get(this.HuiGuiHuoDongIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 123
			});
		};
		UIEventListener.Get(this.MengHuanStoneIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 15
			});
		};
		ActivityTipManager.RegActivityTipItem(14000, new ActivityTipEventHandler(this.JieRiActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14100, new ActivityTipEventHandler(this.PlayerRecallActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14105, new ActivityTipEventHandler(this.TuiguangActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(14110, new ActivityTipEventHandler(this.ZhuanXiangEventHandler));
		ActivityTipManager.RegActivityTipItem(14114, new ActivityTipEventHandler(this.EveryDayEventHandler));
		ActivityTipManager.RegActivityTipItem(20000, new ActivityTipEventHandler(this.OlympicsActivityEventHandler));
		ActivityTipManager.RegActivityTipItem(18000, new ActivityTipEventHandler(this.AoYunDaTiActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(15052, new ActivityTipEventHandler(this.ChaoJiYiYuanEventHandler));
		ActivityTipManager.RegActivityTipItem(15051, new ActivityTipEventHandler(this.XingYunYiYuanEventHandler));
		ActivityTipManager.RegActivityTipItem(15053, new ActivityTipEventHandler(this.JiYuanEventHandler));
		ActivityTipManager.RegActivityTipItem(11502, new ActivityTipEventHandler(this.ZhuTiFuEventHandler));
		ActivityTipManager.RegActivityTipItem(11501, new ActivityTipEventHandler(this.ZhuTiFuEventHandler));
		ActivityTipManager.RegActivityTipItem(15054, new ActivityTipEventHandler(this.EveryFanLiEventHandler));
		ActivityTipManager.RegActivityTipItem(15055, new ActivityTipEventHandler(this.HuiGuiEventHandler));
		ActivityTipManager.RegActivityTipItem(14115, new ActivityTipEventHandler(this.SpecPActAwardEventHandler));
		if (num >= 8)
		{
			NGUITools.SetActive(this.separateLine2.gameObject, true);
			for (int j = 0; j < num - 12; j += 4)
			{
				GameObject gameObject = U3DUtils.Clone(this.separateLine2.transform.parent.gameObject, this.separateLine2.gameObject);
				Vector3 localPosition = this.separateLine2.transform.localPosition;
				localPosition.y -= (float)(((num - 12) / 4 + (((num - 12) % 4 <= 0) ? 0 : 1)) * 70);
				gameObject.transform.localPosition = localPosition;
			}
			int num2 = num / 4 + ((num % 4 <= 0) ? 0 : 1);
			this.background.transform.localScale = new Vector3(280f, (float)(40 + num2 * 64), 1f);
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(14000, new ActivityTipEventHandler(this.JieRiActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14100, new ActivityTipEventHandler(this.PlayerRecallActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14105, new ActivityTipEventHandler(this.TuiguangActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14110, new ActivityTipEventHandler(this.ZhuanXiangEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14114, new ActivityTipEventHandler(this.EveryDayEventHandler));
		ActivityTipManager.UnRegActivityTipItem(20000, new ActivityTipEventHandler(this.OlympicsActivityEventHandler));
		ActivityTipManager.UnRegActivityTipItem(18000, new ActivityTipEventHandler(this.AoYunDaTiActivityTipEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15052, new ActivityTipEventHandler(this.ChaoJiYiYuanEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15051, new ActivityTipEventHandler(this.XingYunYiYuanEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15053, new ActivityTipEventHandler(this.JiYuanEventHandler));
		ActivityTipManager.UnRegActivityTipItem(11501, new ActivityTipEventHandler(this.ZhuTiFuEventHandler));
		ActivityTipManager.UnRegActivityTipItem(11502, new ActivityTipEventHandler(this.ZhuTiFuEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15054, new ActivityTipEventHandler(this.EveryFanLiEventHandler));
		ActivityTipManager.UnRegActivityTipItem(15055, new ActivityTipEventHandler(this.HuiGuiEventHandler));
		ActivityTipManager.UnRegActivityTipItem(14115, new ActivityTipEventHandler(this.SpecPActAwardEventHandler));
	}

	private void SpecPActAwardEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.TeQuanHuoDongTipIcon)
		{
			this.TeQuanHuoDongTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void PlayerRecallActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.playerRecallTipIcon)
		{
			this.playerRecallTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void JieRiActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.activityTipIcon)
		{
			this.activityTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void AoYunDaTiActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.aoYunDaTiTipIcon)
		{
			this.aoYunDaTiTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void TuiguangActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (!Global.IsTuiGuangOpen(true))
		{
			return;
		}
		if (null != this.tuiGuangTipIcon)
		{
			this.tuiGuangTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void ZhuanXiangEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.zhuanxiangTipIcon)
		{
			this.zhuanxiangTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void ChaoJiYiYuanEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.chaojiyiyuanIcon)
		{
			this.chaojiyiyuanTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void XingYunYiYuanEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.xingyunyiyuanIcon)
		{
			this.xingyunyiyuanTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void OlympicsActivityEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.olympicsTipIcon)
		{
			this.olympicsTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void EveryDayEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.everyDayTipIcon)
		{
			this.everyDayTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void JiYuanEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.jiYuanIcon)
		{
			this.jiYuanTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void ZhuTiFuEventHandler(int type, ActivityTipItem args)
	{
		if (type == 11501)
		{
			this.ZhuTiFuBoolTipZhiGou = args.IsActive;
		}
		if (type == 11502)
		{
			this.ZhuTiFuBoolTipDaLiBao = args.IsActive;
		}
		if (this.ZhuTiFuBoolTipDaLiBao || this.ZhuTiFuBoolTipZhiGou)
		{
			if (null != this.zhuTiFuIcon)
			{
				this.ZhuTiFuTipIcon.gameObject.SetActive(true);
			}
		}
		else
		{
			this.ZhuTiFuTipIcon.gameObject.SetActive(false);
		}
	}

	private void EveryFanLiEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.everyFanLiIcon)
		{
			this.everyFanLiTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	private void HuiGuiEventHandler(int type, ActivityTipItem args)
	{
		if (null != this.HuiGuiHuoDongTipIcon)
		{
			this.HuiGuiHuoDongTipIcon.gameObject.SetActive(args.IsActive);
		}
	}

	[ContextMenu("打开所有活动")]
	private void ShouMoYuDuoBao()
	{
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_RegressActive, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_OneDollarChongZhi, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.ChaoJiYiYuan, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_InputFanLiNew, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.JieRi, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.ZhaoHui, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.ZhuanShu, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_ThemeActivity, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_Everyday, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_JunTuanEra, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AoYunDaTi, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_SpecialPrirotiy, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_BoCaiCaiShuZi, OpenActivityState.Begin);
		HuoDongCommonFlag.SetActivityState(OpenActivityType.AST_BoCaiCaiDaXiao, OpenActivityState.Begin);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton activityIcon;

	public UIButton playerRecallIcon;

	public UIButton olympicsIcon;

	public UIButton tuiGuangIcon;

	public UIButton zhuanxiangIcon;

	public UIButton zhuTiFuIcon;

	public UIButton aoYunDaTiIcon;

	public UIButton chaojiyiyuanIcon;

	public UIButton xingyunyiyuanIcon;

	public UIButton everyDayIcon;

	public UIButton jiYuanIcon;

	public UIButton everyFanLiIcon;

	public UIButton TeQuanHuoDongIcon;

	public UIButton HuiGuiHuoDongIcon;

	public UIButton CaiShuZiIcon;

	public UIButton CaiDaXiaoIcon;

	public Transform activityTipIcon;

	public Transform playerRecallTipIcon;

	public Transform olympicsTipIcon;

	public Transform tuiGuangTipIcon;

	public Transform zhuanxiangTipIcon;

	public Transform ZhuTiFuTipIcon;

	public Transform aoYunDaTiTipIcon;

	public Transform chaojiyiyuanTipIcon;

	public Transform xingyunyiyuanTipIcon;

	public Transform everyDayTipIcon;

	public Transform jiYuanTipIcon;

	public Transform everyFanLiTipIcon;

	public Transform TeQuanHuoDongTipIcon;

	public Transform HuiGuiHuoDongTipIcon;

	public Transform CaiShuZiTipIcon;

	public Transform CaiDaXiaoTipIcon;

	public UIButton MengHuanStoneIcon;

	public Transform MengHuanStone0TipIcon;

	public GameObject separateLine2;

	public UISprite background;

	private List<Vector3> originalVecList = new List<Vector3>();

	private bool ZhuTiFuBoolTipDaLiBao;

	private bool ZhuTiFuBoolTipZhiGou;
}
