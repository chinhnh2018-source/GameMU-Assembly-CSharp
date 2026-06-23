using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HuodongPartChongzhiHuikuiPart : UserControl
{
	// Note: this type is marked as 'beforefieldinit'.
	static HuodongPartChongzhiHuikuiPart()
	{
		string[,] array = new string[4, 2];
		array[0, 0] = Global.GetLang("首充大礼");
		array[0, 1] = "Config/Gifts/FirstCharge.xml";
		array[1, 0] = Global.GetLang("每日充值");
		array[1, 1] = "Config/Gifts/DayChongZhi.xml";
		array[2, 0] = Global.GetLang("累计充值");
		array[2, 1] = "Config/Gifts/LeiJiChongZhi.xml";
		array[3, 0] = Global.GetLang("累计消费");
		array[3, 1] = "Config/Gifts/LeiJiXiaoFei.xml";
		HuodongPartChongzhiHuikuiPart.chongzhiHuikuiItemNames = array;
	}

	protected override void InitializeComponent()
	{
		this.ItemList.IsPosYFixed = true;
		Super.ShowNetWaiting(string.Empty);
		GameInstance.Game.GetAllRepayActivityInfo(Global.Data.roleData.RoleID);
		this.initList();
		for (int i = 0; i < this.itemsCount; i++)
		{
			if (this.items[i]._ActivityTipIcon.activeSelf)
			{
				this.items[i].m_LblShowState.gameObject.SetActive(false);
			}
		}
	}

	public void ReSetUIByCommd(string[] strArr)
	{
		if (null != this.items[2])
		{
			this.items[2].m_LblShowInfo.text = string.Format("{0}", strArr[2]);
		}
		if (null != this.items[3])
		{
			this.items[3].m_LblShowInfo.text = string.Format("{0}", strArr[3]);
		}
	}

	private void initList()
	{
		if (this.items == null)
		{
			this.items = new HuodongPartChongzhiHuikuiPartItem[this.itemsCount];
		}
		for (int i = 0; i < this.itemsCount; i++)
		{
			if (this.items[i] == null)
			{
				this.items[i] = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartItem>();
				if (i == 1 && Global.IsInWeekendRechargePeriod())
				{
					this.items[i].ItemHeadBak.URL = Global.GetGameResImageString("fuliItem_bak_weekend.jpg");
				}
				else
				{
					this.items[i].ItemHeadBak.URL = Global.GetGameResImageString(string.Format("fuliItem_bak{0}.jpg", i + 1));
				}
				this.items[i].ItemIndex = i;
			}
			this.items[i].DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (!this.IsTween)
				{
					this.initItemPart(e.ID);
				}
			};
			U3DUtils.AddChild(this.ItemList.gameObject, this.items[i].gameObject, false);
			if (i == 0)
			{
				ActivityTipManager.RegActivityTipItem(3002, new ActivityTipEventHandler(this.ActivityTipEventHandler));
			}
			else if (i == 1)
			{
				ActivityTipManager.RegActivityTipItem(3003, new ActivityTipEventHandler(this.ActivityTipEventHandler));
			}
			else if (i == 2)
			{
				ActivityTipManager.RegActivityTipItem(3004, delegate(int s, ActivityTipItem e)
				{
					this.items[2]._ActivityTipIcon.SetActive(e.IsActive);
					if (!e.IsActive && this.selectedIndex == 2)
					{
						this.items[2].m_LblShowState.gameObject.SetActive(true);
						this.items[2].m_LblShowState.text = Global.GetLang("点击收起");
					}
				});
			}
			else if (i == 3)
			{
				ActivityTipManager.RegActivityTipItem(3005, delegate(int s, ActivityTipItem e)
				{
					this.items[3]._ActivityTipIcon.SetActive(e.IsActive);
					if (!e.IsActive && this.selectedIndex == 3)
					{
						this.items[3].m_LblShowState.gameObject.SetActive(true);
						this.items[3].m_LblShowState.text = Global.GetLang("点击收起");
					}
				});
			}
		}
	}

	private void ActivityTipEventHandler(int type, ActivityTipItem args)
	{
		if (args == null)
		{
			return;
		}
		int num = -1;
		if (type == 3002)
		{
			num = 0;
		}
		if (type == 3003)
		{
			num = 1;
		}
		if (this.items == null)
		{
			return;
		}
		if (null == this.items[num]._ActivityTipIcon)
		{
			return;
		}
		this.items[num]._ActivityTipIcon.SetActive(args.IsActive);
		if (!args.IsActive && num == this.selectedIndex)
		{
			this.items[num].m_LblShowState.gameObject.SetActive(true);
			this.items[num].m_LblShowState.text = Global.GetLang("点击收起");
		}
	}

	private new void OnDestroy()
	{
		if (this.items != null)
		{
			for (int i = 0; i < this.itemsCount; i++)
			{
				if (null != this.items[i])
				{
					Object.Destroy(this.items[i].gameObject);
					this.items[i] = null;
				}
			}
		}
		this.items = null;
		if (null != this.firstChongzhiPart)
		{
			Object.Destroy(this.firstChongzhiPart.gameObject);
			this.firstChongzhiPart = null;
		}
		if (null != this.meiriChongzhiPart)
		{
			Object.Destroy(this.meiriChongzhiPart.gameObject);
			this.meiriChongzhiPart = null;
		}
		if (null != this.leijiChongzhiPart)
		{
			Object.Destroy(this.leijiChongzhiPart.gameObject);
			this.leijiChongzhiPart = null;
		}
		if (null != this.leijiXiaofeiPart)
		{
			Object.Destroy(this.leijiXiaofeiPart.gameObject);
			this.leijiXiaofeiPart = null;
		}
		ActivityTipManager.UnRegActivityTipItem(3002, new ActivityTipEventHandler(this.ActivityTipEventHandler));
		ActivityTipManager.RegActivityTipItem(3004, null);
		ActivityTipManager.RegActivityTipItem(3005, null);
	}

	private void setTab(int index = -1)
	{
		if (index >= 0)
		{
			if (this.selectedIndex >= 0)
			{
				if (index == this.selectedIndex)
				{
					this.items[this.selectedIndex].ToggleState = !this.items[this.selectedIndex].ToggleState;
					return;
				}
				if (this.items[this.selectedIndex].ToggleState)
				{
					this.items[this.selectedIndex].ToggleState = false;
				}
			}
			this.selectedIndex = index;
			this.items[this.selectedIndex].ToggleState = true;
		}
	}

	private void tweenPosPart(int index)
	{
		this.toggleState = !this.toggleState;
		if (index == this.selectedIndex)
		{
			Vector3 vector = this.fromPos;
			this.fromPos = this.toPos;
			this.toPos = vector;
			this.IsTween = true;
			this.PartTweenPosition.Play(this.toggleState);
		}
		else
		{
			float num = -((float)index * (161f + this.ItemList.padding.x * 2f));
			this.fromPos = this.toPos;
			this.toPos = new Vector3(num, this.toPos.y, this.toPos.z);
			this.PartTweenPosition.from = this.fromPos;
			this.PartTweenPosition.to = this.toPos;
			this.IsTween = true;
			this.PartTweenPosition.Play(this.toggleState);
		}
		this.PartTweenPosition.onFinished = new UITweener.OnFinished(this.onFinished);
	}

	private void onFinished(UITweener tween)
	{
		this.IsTween = false;
		foreach (HuodongPartChongzhiHuikuiPartItem huodongPartChongzhiHuikuiPartItem in this.items)
		{
			huodongPartChongzhiHuikuiPartItem.transform.localPosition = NGUITools.Round(huodongPartChongzhiHuikuiPartItem.transform.localPosition);
			huodongPartChongzhiHuikuiPartItem.m_LocalPosition = NGUITools.Round(huodongPartChongzhiHuikuiPartItem.transform.localPosition);
		}
	}

	public void initItemPart(int index)
	{
		switch (index)
		{
		case 0:
			if (null == this.firstChongzhiPart)
			{
				this.firstChongzhiPart = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartFirstChongzhiPart>();
				this.firstChongzhiPart.InitPartData(HuodongPartChongzhiHuikuiPart.chongzhiHuikuiItemNames[index, 1]);
				this.firstChongzhiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 1)
					{
					}
				};
				this.items[index].Content.Add(this.firstChongzhiPart);
			}
			break;
		case 1:
			if (null == this.meiriChongzhiPart)
			{
				this.meiriChongzhiPart = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartMeiriChongzhiPart>();
				this.meiriChongzhiPart.InitPartData(HuodongPartChongzhiHuikuiPart.chongzhiHuikuiItemNames[index, 1]);
				this.meiriChongzhiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 1)
					{
					}
				};
				this.items[index].Content.Add(this.meiriChongzhiPart);
			}
			break;
		case 2:
			if (null == this.leijiChongzhiPart)
			{
				this.leijiChongzhiPart = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartLeijiChongzhiPart>();
				if (null != this.items[2])
				{
					this.leijiChongzhiPart.sumDepositDiamonds = Global.SafeConvertToInt32(this.items[2].m_LblShowInfo.text);
				}
				this.leijiChongzhiPart.InitPartData(HuodongPartChongzhiHuikuiPart.chongzhiHuikuiItemNames[index, 1]);
				this.leijiChongzhiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 1)
					{
					}
				};
				this.items[index].Content.Add(this.leijiChongzhiPart);
			}
			break;
		case 3:
			if (null == this.leijiXiaofeiPart)
			{
				this.leijiXiaofeiPart = U3DUtils.NEW<HuodongPartChongzhiHuikuiPartLeijiXiaofeiPart>();
				if (null != this.items[3])
				{
					this.leijiXiaofeiPart.sumConsumedDiamonds = Global.SafeConvertToInt32(this.items[3].m_LblShowInfo.text);
				}
				this.leijiXiaofeiPart.InitPartData(HuodongPartChongzhiHuikuiPart.chongzhiHuikuiItemNames[index, 1]);
				this.leijiXiaofeiPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
				{
					if (e.IDType == 1)
					{
					}
				};
				this.items[index].Content.Add(this.leijiXiaofeiPart);
			}
			break;
		}
		this.tweenPosPart(index);
		this.items[index].m_LblShowState.gameObject.SetActive(!this.items[index]._ActivityTipIcon.gameObject.activeInHierarchy);
		this.items[index].m_LblShowState.text = Global.GetLang("点击收起");
		if (index != this.selectedIndex && this.selectedIndex != -1)
		{
			this.items[this.selectedIndex].m_LblShowState.text = Global.GetLang("点击查看");
		}
		else if (index == this.selectedIndex && this.selectedIndex != -1)
		{
			if (this.items[index].ToggleState)
			{
				this.items[index].m_LblShowState.text = Global.GetLang("点击查看");
			}
			else
			{
				this.items[index].m_LblShowState.text = Global.GetLang("点击收起");
			}
		}
		this.setTab(index);
	}

	public void ReloadDailyDesposit()
	{
		if (this.items != null && this.items.Length > 0)
		{
			this.items[1].ItemHeadBak.URL = Global.GetGameResImageString((!Global.IsInWeekendRechargePeriod()) ? "fuliItem_bak2.jpg" : "fuliItem_bak_weekend.jpg");
		}
		if (null != this.meiriChongzhiPart)
		{
			this.meiriChongzhiPart.LoadBanner();
			this.meiriChongzhiPart.ReloadDailyRewards();
		}
	}

	public HuodongPartChongzhiHuikuiPartFirstChongzhiPart firstChongzhiPart;

	public HuodongPartChongzhiHuikuiPartMeiriChongzhiPart meiriChongzhiPart;

	public HuodongPartChongzhiHuikuiPartLeijiChongzhiPart leijiChongzhiPart;

	public HuodongPartChongzhiHuikuiPartLeijiXiaofeiPart leijiXiaofeiPart;

	public UITable ItemList;

	public TweenPosition PartTweenPosition;

	private HuodongPartChongzhiHuikuiPartItem[] items;

	private int itemsCount = 4;

	private int selectedIndex = -1;

	private bool IsTween;

	private Vector3 fromPos = Vector3.zero;

	private Vector3 toPos = Vector3.zero;

	private bool toggleState;

	private static string[,] chongzhiHuikuiItemNames;
}
