using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class HefuhuodongPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.TabBtnOBC = this.ListTabBtn.ItemsSource;
		this.ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedBtn);
		this.InitBtnItem();
		this.ListTabBtn.SelectedIndex = 0;
	}

	private void InitBtnItem()
	{
		if (this.BtnClose != null)
		{
			this.BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					IDType = 0
				});
			};
		}
		XElement gameResXml = Global.GetGameResXml("Config/HeFuType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				HefuTypeItem hefuTypeItem = U3DUtils.NEW<HefuTypeItem>();
				hefuTypeItem.label.text = xelementAttributeStr;
				hefuTypeItem.TipIcon.gameObject.SetActive(false);
				hefuTypeItem.label.color = NGUIMath.HexToColorEx(8350293U);
				hefuTypeItem.id = Global.GetXElementAttributeInt(xelement, "ID");
				this.TabBtnOBC.AddNoUpdate(hefuTypeItem);
			}
		}
		this.TabBtnOBC.DelayUpdate();
		ActivityTipManager.RegActivityTipItem(12001, delegate(int s, ActivityTipItem e)
		{
			HefuTypeItem hefuTypeItem2 = U3DUtils.AS<HefuTypeItem>(this.TabBtnOBC[0]);
			hefuTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(12002, delegate(int s, ActivityTipItem e)
		{
			HefuTypeItem hefuTypeItem2 = U3DUtils.AS<HefuTypeItem>(this.TabBtnOBC[1]);
			hefuTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(12003, delegate(int s, ActivityTipItem e)
		{
			HefuTypeItem hefuTypeItem2 = U3DUtils.AS<HefuTypeItem>(this.TabBtnOBC[2]);
			hefuTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(12004, delegate(int s, ActivityTipItem e)
		{
			HefuTypeItem hefuTypeItem2 = U3DUtils.AS<HefuTypeItem>(this.TabBtnOBC[3]);
			hefuTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
		ActivityTipManager.RegActivityTipItem(12005, delegate(int s, ActivityTipItem e)
		{
			HefuTypeItem hefuTypeItem2 = U3DUtils.AS<HefuTypeItem>(this.TabBtnOBC[5]);
			hefuTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
		});
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		HefuTypeItem hefuTypeItem = U3DUtils.AS<HefuTypeItem>(this.ListTabBtn.SelectedItem);
		if (null == hefuTypeItem)
		{
			return;
		}
		if (this.hefuBtnItem != null && this.hefuBtnItem != hefuTypeItem)
		{
			this.hefuBtnItem.Bak.spriteName = "chatTab_normal";
			this.hefuBtnItem.label.color = NGUIMath.HexToColorEx(8350293U);
		}
		if (hefuTypeItem == this.hefuBtnItem)
		{
			return;
		}
		this.hefuBtnItem = hefuTypeItem;
		this.hefuBtnItem.Bak.spriteName = "chatTab_hover";
		hefuTypeItem.label.color = NGUIMath.HexToColorEx(15461355U);
		this.ShowPage(hefuTypeItem);
	}

	private void ShowPage(HefuTypeItem item)
	{
		this.SprPnlContent.Clear();
		this.m_HefuPartDengluhaoli = null;
		this.m_HefuPartLeijiDenglu = null;
		this.m_HefuPartChongzhiFanli = null;
		this.m_HefupartZhanchangZhishen = null;
		this.m_HefuPartBOSSzhizhan = null;
		this.m_HefuPartWeizhanErsheng = null;
		this.m_HefuPartQianggou = null;
		this.m_HefuPartLuolanZhengba = null;
		switch (item.id)
		{
		case 20:
			this.m_HefuPartDengluhaoli = U3DUtils.NEW<HefuPartDengluhaoli>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartDengluhaoli.gameObject, true);
			break;
		case 21:
			this.m_HefuPartLeijiDenglu = U3DUtils.NEW<HefuPartLeijiDenglu>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartLeijiDenglu.gameObject, true);
			break;
		case 22:
			this.m_HefuPartQianggou = U3DUtils.NEW<HefuPartQianggou>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartQianggou.gameObject, true);
			break;
		case 23:
			this.m_HefuPartChongzhiFanli = U3DUtils.NEW<HefuPartChongzhiFanli>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartChongzhiFanli.gameObject, true);
			break;
		case 24:
			this.m_HefupartZhanchangZhishen = U3DUtils.NEW<HefupartZhanchangZhishen>();
			this.m_HefupartZhanchangZhishen.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedItem != null && e.ID == 105)
				{
					this.DPSelectedItem(s, new DPSelectedItemEventArgs
					{
						ID = 105,
						IDType = 0
					});
				}
			};
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefupartZhanchangZhishen.gameObject, true);
			break;
		case 25:
			this.m_HefuPartWeizhanErsheng = U3DUtils.NEW<HefuPartWeizhanErsheng>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartWeizhanErsheng.gameObject, true);
			break;
		case 26:
			this.m_HefuPartBOSSzhizhan = U3DUtils.NEW<HefuPartBOSSzhizhan>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartBOSSzhizhan.gameObject, true);
			break;
		case 27:
			this.m_HefuPartLuolanZhengba = U3DUtils.NEW<HefuPartLuolanZhengba>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HefuPartLuolanZhengba.gameObject, true);
			this.m_HefuPartLuolanZhengba.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.DPSelectedItem != null)
				{
					GameInstance.Game.GetChongZhiJiangLi(Global.Data.roleData.RoleID, 43, e.ID);
				}
			};
			break;
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(12000, null);
		ActivityTipManager.RegActivityTipItem(12001, null);
		ActivityTipManager.RegActivityTipItem(12002, null);
		ActivityTipManager.RegActivityTipItem(12003, null);
		ActivityTipManager.RegActivityTipItem(12004, null);
		ActivityTipManager.RegActivityTipItem(12005, null);
	}

	public string GetErrMsg(int errCode)
	{
		string result = string.Empty;
		if (errCode < 0)
		{
			if (errCode != -50)
			{
				if (errCode != -40)
				{
					if (errCode != -30)
					{
						if (errCode != -20)
						{
							if (errCode != -10)
							{
								if (errCode != -5)
								{
									if (errCode != -1)
									{
										result = Global.GetLang("其他错误！错误码[") + errCode + "]";
									}
									else
									{
										result = Global.GetLang("活动不存在");
									}
								}
								else
								{
									result = Global.GetLang("每日充值豪礼充值不满足的返回值");
								}
							}
							else
							{
								result = Global.GetLang("已领取");
							}
						}
						else
						{
							result = Global.GetLang("背包不足");
						}
					}
					else
					{
						result = Global.GetLang("条件不满足");
					}
				}
				else
				{
					result = Global.GetLang("不在领取时间内");
				}
			}
			else
			{
				result = Global.GetLang("不在配置的领取时间内");
			}
		}
		return result;
	}

	public GameObject PnlContent;

	public GameObject BtnItem;

	public SpriteSL SprPnlContent;

	private ObservableCollection TabBtnOBC;

	public ListBox ListTabBtn;

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public HefuPartDengluhaoli m_HefuPartDengluhaoli;

	public HefuPartLeijiDenglu m_HefuPartLeijiDenglu;

	public HefuPartChongzhiFanli m_HefuPartChongzhiFanli;

	public HefupartZhanchangZhishen m_HefupartZhanchangZhishen;

	public HefuPartBOSSzhizhan m_HefuPartBOSSzhizhan;

	public HefuPartWeizhanErsheng m_HefuPartWeizhanErsheng;

	public HefuPartQianggou m_HefuPartQianggou;

	public HefuPartLuolanZhengba m_HefuPartLuolanZhengba;

	private HefuTypeItem hefuBtnItem;
}
