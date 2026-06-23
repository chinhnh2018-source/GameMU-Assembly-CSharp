using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class MUQiRiHuoDongPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
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
		this.TabBtnOBC = this.ListTabBtn.ItemsSource;
		this.ListTabBtn.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedBtn);
		this.InitBtnItem();
		this.ListTabBtn.SelectedIndex = 0;
	}

	private void InitBtnItem()
	{
		XElement gameResXml = Global.GetGameResXml("Config/SevenDayActivityType.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ActivityType");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ActivityType");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "XML");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Tiptype");
				JieRiTypeItem jieRiTypeItem = U3DUtils.NEW<JieRiTypeItem>();
				jieRiTypeItem.label.text = xelementAttributeStr;
				jieRiTypeItem.TipIcon.gameObject.SetActive(false);
				jieRiTypeItem.label.color = NGUIMath.HexToColorEx(8350293U);
				jieRiTypeItem.Id = xelementAttributeInt;
				if (!this.ItemsDict.ContainsKey(xelementAttributeInt))
				{
					this.ItemsDict.Add(xelementAttributeInt, xelementAttributeStr2);
				}
				if (!this.TipDict.ContainsKey(xelementAttributeInt2))
				{
					this.TipDict.Add(xelementAttributeInt2, xelementAttributeInt);
				}
				if (!this.btnItemDict.ContainsKey(xelementAttributeInt))
				{
					this.btnItemDict.Add(xelementAttributeInt, jieRiTypeItem);
				}
				this.TabBtnOBC.AddNoUpdate(jieRiTypeItem);
			}
		}
		this.TabBtnOBC.DelayUpdate();
		ActivityTipManager.RegActivityTipItem(17001, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(17001))
			{
				num = this.TipDict[17001];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17002, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(17002))
			{
				num = this.TipDict[17002];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17003, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(17003))
			{
				num = this.TipDict[17003];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(17004, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(17004))
			{
				num = this.TipDict[17004];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
	}

	private void SelectedBtn(object sender, MouseEvent e)
	{
		JieRiTypeItem jieRiTypeItem = U3DUtils.AS<JieRiTypeItem>(this.ListTabBtn.SelectedItem);
		if (null == jieRiTypeItem)
		{
			return;
		}
		if (this.jieriBtnItem != null && this.jieriBtnItem != jieRiTypeItem)
		{
			this.jieriBtnItem.Bak.spriteName = "chatTab_normal";
			this.jieriBtnItem.label.color = NGUIMath.HexToColorEx(8350293U);
		}
		if (jieRiTypeItem == this.jieriBtnItem)
		{
			return;
		}
		this.jieriBtnItem = jieRiTypeItem;
		this.jieriBtnItem.Bak.spriteName = "chatTab_hover";
		jieRiTypeItem.label.color = NGUIMath.HexToColorEx(15461355U);
		this.ShowPage(jieRiTypeItem, this.ListTabBtn.SelectedIndex + 1);
	}

	private void ShowPage(JieRiTypeItem item, int index)
	{
		this.SprPnlContent.Clear();
		this.muQiRiKuangHuanPartDenglu = null;
		this.muQiRiKuangHuanPartChongzhi = null;
		this.muQiRiGoalPart = null;
		this.muQiRiQianggou = null;
		switch (item.Id)
		{
		case 1:
			this.muQiRiKuangHuanPartDenglu = U3DUtils.NEW<MUQiRiKuangHuanPartDenglu>();
			this.muQiRiKuangHuanPartDenglu.ThisXmlName = "SevenDayLogin.xml";
			U3DUtils.AddChild(this.PnlContent.gameObject, this.muQiRiKuangHuanPartDenglu.gameObject, true);
			break;
		case 2:
			this.muQiRiKuangHuanPartChongzhi = U3DUtils.NEW<MUQiRiKuangHuanPartChongZhi>();
			this.muQiRiKuangHuanPartChongzhi.ThisXmlName = "SevenDayChongZhi.xml";
			U3DUtils.AddChild(this.PnlContent.gameObject, this.muQiRiKuangHuanPartChongzhi.gameObject, true);
			break;
		case 3:
			this.muQiRiGoalPart = U3DUtils.NEW<MUQiRiGoalPart>();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.muQiRiGoalPart.gameObject, true);
			break;
		case 4:
			this.muQiRiQianggou = U3DUtils.NEW<MUQiRiQianggou>();
			this.muQiRiQianggou.ThisXmlName = "SevenDayQiangGou.xml";
			U3DUtils.AddChild(this.PnlContent.gameObject, this.muQiRiQianggou.gameObject, true);
			break;
		}
	}

	public void BuyCompleted(int errorCode, int id, int leftNum)
	{
		this.ErrorShow(errorCode);
		if (errorCode != 0)
		{
			return;
		}
		this.muQiRiQianggou.SetOldNum(id, leftNum);
	}

	public void GoalCompleted(int errorCode, int id, int leftNum)
	{
		this.ErrorShow(errorCode);
		if (errorCode != 0)
		{
			return;
		}
		this.muQiRiGoalPart.SetBtnStateInfo(leftNum);
	}

	public void LingquCompleted(int errorCode, int actType, int id)
	{
		this.ErrorShow(errorCode);
		if (errorCode != 0)
		{
			return;
		}
		switch (actType)
		{
		case 1:
			if (this.muQiRiKuangHuanPartDenglu != null)
			{
				this.muQiRiKuangHuanPartDenglu.setCompletedInfo(id);
			}
			break;
		case 2:
			if (this.muQiRiKuangHuanPartChongzhi != null)
			{
				this.muQiRiKuangHuanPartChongzhi.setCompletedInfo(id);
			}
			break;
		case 3:
			if (this.muQiRiGoalPart != null)
			{
			}
			break;
		case 4:
			if (this.muQiRiQianggou != null)
			{
			}
			break;
		}
	}

	private void ErrorShow(int errorCode)
	{
		if (errorCode >= 0)
		{
			if (errorCode == 0)
			{
				return;
			}
			if (errorCode == 1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不在活动时间"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("服务器配置出错"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("数据库异常"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 5)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足领奖条件"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("客户端访问参数错误"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 7)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("钻石不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (errorCode == 8)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("可抢购数量不足"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励错误:{0}"), new object[]
				{
					errorCode
				}), 0, -1, -1, 0);
			}
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(17001, null);
		ActivityTipManager.RegActivityTipItem(17002, null);
		ActivityTipManager.RegActivityTipItem(17003, null);
		ActivityTipManager.RegActivityTipItem(17004, null);
	}

	public GameObject PnlContent;

	public GameObject BtnItem;

	public SpriteSL SprPnlContent;

	private ObservableCollection TabBtnOBC;

	public ListBox ListTabBtn;

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public MUQiRiKuangHuanPartDenglu muQiRiKuangHuanPartDenglu;

	public MUQiRiKuangHuanPartChongZhi muQiRiKuangHuanPartChongzhi;

	public MUQiRiGoalPart muQiRiGoalPart;

	public MUQiRiQianggou muQiRiQianggou;

	private Dictionary<int, string> ItemsDict = new Dictionary<int, string>();

	private Dictionary<int, int> TipDict = new Dictionary<int, int>();

	private Dictionary<int, JieRiTypeItem> btnItemDict = new Dictionary<int, JieRiTypeItem>();

	private JieRiTypeItem jieriBtnItem;
}
