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

public class MUJierihuodongPart : UserControl
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
		Super.ShowNetWaiting(null);
		GameInstance.Game.SpriteGetJieriXmlData();
	}

	public static void SetGoodsIconBoxCollider(GGoodIcon icon)
	{
		BoxCollider component = icon.GetComponent<BoxCollider>();
		if (null != component)
		{
			component.isTrigger = false;
			component.center = new Vector3(0f, 0f, -1.5f);
		}
	}

	public void OnXmlDataResult(JieriXmlData jieriXmlData)
	{
		Super.HideNetWaiting();
		if (jieriXmlData == null)
		{
			return;
		}
		if (Global.Data.JieriData == null || Global.JieriXML_Version != jieriXmlData.Version)
		{
			Global.Data.JieriData = jieriXmlData;
		}
		Global.JieriXML_Version = jieriXmlData.Version;
		this.InitBtnItem();
		this.ListTabBtn.SelectedIndex = 0;
	}

	private void InitBtnItem()
	{
		if (Global.Data.JieriData == null)
		{
			return;
		}
		XElement xelement = XElement.Parse(Global.Data.JieriData.XmlList[0]);
		if (xelement == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(xelement, "Type");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement2 = xelementList[i];
			if (xelement2 != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Type");
				if (xelementAttributeInt != 67 || HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.SuperDirect))
				{
					if (xelementAttributeInt != 71 || HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.AST_SuperInputFanLi))
					{
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "ID");
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "Name");
						string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "PeiZhi");
						int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "Tiptype");
						JieRiTypeItem jieRiTypeItem = U3DUtils.NEW<JieRiTypeItem>();
						jieRiTypeItem.label.text = xelementAttributeStr;
						jieRiTypeItem.TipIcon.gameObject.SetActive(false);
						jieRiTypeItem.label.color = NGUIMath.HexToColorEx(8350293U);
						jieRiTypeItem.Id = xelementAttributeInt2;
						jieRiTypeItem.Index = i;
						if (!this.ItemsDict.ContainsKey(xelementAttributeInt2))
						{
							this.ItemsDict.Add(xelementAttributeInt2, xelementAttributeStr2);
						}
						if (!this.TipDict.ContainsKey(xelementAttributeInt3))
						{
							this.TipDict.Add(xelementAttributeInt3, xelementAttributeInt2);
						}
						if (!this.btnItemDict.ContainsKey(xelementAttributeInt2))
						{
							this.btnItemDict.Add(xelementAttributeInt2, jieRiTypeItem);
						}
						this.TabBtnOBC.AddNoUpdate(jieRiTypeItem);
					}
				}
			}
		}
		this.TabBtnOBC.DelayUpdate();
		ActivityTipManager.RegActivityTipItem(14001, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14001))
			{
				num = this.TipDict[14001];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14002, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14002))
			{
				num = this.TipDict[14002];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14003, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14003))
			{
				num = this.TipDict[14003];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14004, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14004))
			{
				num = this.TipDict[14004];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14005, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14005))
			{
				num = this.TipDict[14005];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14006, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14006))
			{
				num = this.TipDict[14006];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14007, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14007))
			{
				num = this.TipDict[14007];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14008, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14008))
			{
				num = this.TipDict[14008];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14009, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14009))
			{
				num = this.TipDict[14009];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14010, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14010))
			{
				num = this.TipDict[14010];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14011, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14011))
			{
				num = this.TipDict[14011];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14012, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14012))
			{
				num = this.TipDict[14012];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14013, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14013))
			{
				num = this.TipDict[14013];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14014, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14014))
			{
				num = this.TipDict[14014];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14015, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14015))
			{
				num = this.TipDict[14015];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14016, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14016))
			{
				num = this.TipDict[14016];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14017, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14017))
			{
				num = this.TipDict[14017];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14018, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14018))
			{
				num = this.TipDict[14018];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14021, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14021))
			{
				num = this.TipDict[14021];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14020, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14020))
			{
				num = this.TipDict[14020];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14019, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14019))
			{
				num = this.TipDict[14019];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14023, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14023))
			{
				num = this.TipDict[14023];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14027, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14027))
			{
				num = this.TipDict[14027];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14024, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14024))
			{
				num = this.TipDict[14024];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(1515, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(1515))
			{
				num = this.TipDict[1515];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14028, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14028))
			{
				num = this.TipDict[14028];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14035, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14035))
			{
				num = this.TipDict[14035];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14032, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14032))
			{
				num = this.TipDict[14032];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14033, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14033))
			{
				num = this.TipDict[14033];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
		ActivityTipManager.RegActivityTipItem(14034, delegate(int s, ActivityTipItem e)
		{
			int num = 0;
			if (this.TipDict.ContainsKey(14034))
			{
				num = this.TipDict[14034];
			}
			if (this.btnItemDict.ContainsKey(num))
			{
				JieRiTypeItem jieRiTypeItem2 = this.btnItemDict[num];
				jieRiTypeItem2.TipIcon.gameObject.SetActive(e.IsActive);
			}
		});
	}

	private void CheckXML(int type)
	{
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
		this.ShowPage(jieRiTypeItem, jieRiTypeItem.Index + 1);
	}

	private void ShowPage(JieRiTypeItem item, int index)
	{
		this.SprPnlContent.Clear();
		this.m_MUJieRipartLibao = null;
		this.m_MUJieRipartShouji = null;
		this.m_MUJieriPartDuihuan = null;
		this.m_MUJieRiPartLeijiDenglu = null;
		this.m_MUJieriPartLeijiChongzhi = null;
		this.m_MUJieriPartmeiriChongzhi = null;
		this.m_MUJieriPartLeijiXiaofei = null;
		this.m_MUJieripartChongzhiKing = null;
		this.m_MUJieriPartXiaofeiking = null;
		this.m_MUJieriPartQianggou = null;
		this.m_MUJieriPartBoss = null;
		this.m_MUJieriPartFanbei = null;
		this.m_MUJieriZengsongPart = null;
		this.m_MUJieriZengsongZengKingPart = null;
		this.m_MUJieriTongyongFanliPart = null;
		this.m_MUJieriLianxuChongzhiPart = null;
		this.m_MUJieriPartFuLi = null;
		this.m_SuperDirectBuyPart = null;
		this.m_MUJieriMeiriLeichongPart = null;
		this.m_JieRiVIPlibaoPart = null;
		this.m_DanBiChongZhiPart = null;
		this.m_HuoDongChongZhiFanLiPart = null;
		this.m_HongBaoChongZhiPart = null;
		this.m_HongBaoPaiHangPart = null;
		this.m_HongBaoQuanMinPart = null;
		switch (item.Id)
		{
		case 1:
			this.m_MUJieRipartLibao = U3DUtils.NEW<MUJieRipartLibao>();
			this.m_MUJieRipartLibao.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieRipartLibao.gameObject, true);
			break;
		case 2:
			this.m_MUJieRipartShouji = U3DUtils.NEW<MUJieRipartShouji>();
			this.m_MUJieRipartShouji.Type = item.Id;
			this.m_MUJieRipartShouji.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieRipartShouji.gameObject, true);
			break;
		case 3:
			this.m_MUJieriPartDuihuan = U3DUtils.NEW<MUJieriPartDuihuan>();
			this.m_MUJieriPartDuihuan.Type = 12;
			this.m_MUJieriPartDuihuan.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartDuihuan.gameObject, true);
			break;
		case 4:
			this.m_MUJieRiPartLeijiDenglu = U3DUtils.NEW<MUJieRiPartLeijiDenglu>();
			this.m_MUJieRiPartLeijiDenglu.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieRiPartLeijiDenglu.gameObject, true);
			break;
		case 5:
			this.m_MUJieriPartmeiriChongzhi = U3DUtils.NEW<MUJieriPartmeiriChongzhi>();
			this.m_MUJieriPartmeiriChongzhi.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartmeiriChongzhi.gameObject, true);
			break;
		case 6:
			this.m_MUJieriPartLeijiChongzhi = U3DUtils.NEW<MUJieriPartLeijiChongzhi>();
			this.m_MUJieriPartLeijiChongzhi.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartLeijiChongzhi.gameObject, true);
			break;
		case 7:
			this.m_MUJieriPartLeijiXiaofei = U3DUtils.NEW<MUJieriPartLeijiXiaofei>();
			this.m_MUJieriPartLeijiXiaofei.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartLeijiXiaofei.gameObject, true);
			break;
		case 8:
			this.m_MUJieripartChongzhiKing = U3DUtils.NEW<MUJieripartChongzhiKing>();
			this.m_MUJieripartChongzhiKing.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieripartChongzhiKing.gameObject, true);
			break;
		case 9:
			this.m_MUJieriPartXiaofeiking = U3DUtils.NEW<MUJieriPartXiaofeiking>();
			this.m_MUJieriPartXiaofeiking.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartXiaofeiking.gameObject, true);
			break;
		case 10:
			this.m_MUJieriPartQianggou = U3DUtils.NEW<MUJieriPartQianggou>();
			this.m_MUJieriPartQianggou.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartQianggou.gameObject, true);
			break;
		case 11:
			this.m_MUJieriPartBoss = U3DUtils.NEW<MUJieriPartBoss>();
			this.m_MUJieriPartBoss.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartBoss.gameObject, true);
			break;
		case 12:
			this.m_MUJieriPartFanbei = U3DUtils.NEW<MUJieriPartFanbei>();
			this.m_MUJieriPartFanbei.ThisXmlName = Global.Data.JieriData.XmlList[index];
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartFanbei.gameObject, true);
			break;
		case 13:
			this.m_MUJieriZengsongPart = U3DUtils.NEW<MUJieriZengsongPart>();
			this.m_MUJieriZengsongPart.InitData(Global.Data.JieriData.XmlList[index]);
			this.m_MUJieriZengsongPart.GetDataInfo();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriZengsongPart.gameObject, true);
			break;
		case 14:
			this.m_MUJieriZengsongZengKingPart = U3DUtils.NEW<MUJieriZengsongZengKingPart>();
			this.m_MUJieriZengsongZengKingPart.SetBak("mujierihuodong_songli");
			this.m_MUJieriZengsongZengKingPart.InitData(Global.Data.JieriData.XmlList[index], 1);
			this.m_MUJieriZengsongZengKingPart.GetDataInfo();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriZengsongZengKingPart.gameObject, true);
			break;
		case 15:
			this.m_MUJieriZengsongZengKingPart = U3DUtils.NEW<MUJieriZengsongZengKingPart>();
			this.m_MUJieriZengsongZengKingPart.SetBak("mujierihuodong_shouli");
			this.m_MUJieriZengsongZengKingPart.InitData(Global.Data.JieriData.XmlList[index], 2);
			this.m_MUJieriZengsongZengKingPart.GetDataInfo();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriZengsongZengKingPart.gameObject, true);
			break;
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 38:
		case 39:
			this.m_MUJieriTongyongFanliPart = U3DUtils.NEW<MUJieriTongyongFanliPart>();
			this.m_MUJieriTongyongFanliPart.SetActivityType(item.Id);
			this.m_MUJieriTongyongFanliPart.InitData(Global.Data.JieriData.XmlList[index]);
			this.m_MUJieriTongyongFanliPart.GetDataInfo();
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriTongyongFanliPart.gameObject, true);
			break;
		case 26:
			if (null == this.m_MUJieriLianxuChongzhiPart)
			{
				this.m_MUJieriLianxuChongzhiPart = U3DUtils.NEW<MUJieriLianxuChongzhiPart>();
				this.m_MUJieriLianxuChongzhiPart.InitData(Global.Data.JieriData.XmlList[index]);
				this.m_MUJieriLianxuChongzhiPart.GetDataInfo();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriLianxuChongzhiPart.gameObject, true);
			}
			break;
		case 27:
			if (null == this.m_MUJieriZengsongZengKingPart)
			{
				this.m_MUJieriZengsongZengKingPart = U3DUtils.NEW<MUJieriZengsongZengKingPart>();
				this.m_MUJieriZengsongZengKingPart.SetBak("mujierihuodong_pingtaiKing");
				this.m_MUJieriZengsongZengKingPart.InitData(Global.Data.JieriData.XmlList[index], 3);
				this.m_MUJieriZengsongZengKingPart.GetDataInfo();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriZengsongZengKingPart.gameObject, true);
			}
			break;
		case 28:
			if (null == this.redemptionActivity)
			{
				this.redemptionActivity = U3DUtils.NEW<RedemptionActivity>();
				this.redemptionActivity.xmlName = Global.Data.JieriData.XmlList[index];
				U3DUtils.AddChild(this.PnlContent.gameObject, this.redemptionActivity.gameObject, true);
			}
			break;
		case 29:
			if (null == this.m_MUJieriPartFuLi)
			{
				this.m_MUJieriPartFuLi = U3DUtils.NEW<MUJieriPartFuLi>();
				this.m_MUJieriPartFuLi.ThisXmlName = Global.Data.JieriData.XmlList[index];
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriPartFuLi.gameObject, true);
			}
			break;
		case 30:
			if (null == this.m_SuperDirectBuyPart)
			{
				this.m_SuperDirectBuyPart = U3DUtils.NEW<SuperDirectBuyPart>();
				this.m_SuperDirectBuyPart.InitXml(Global.Data.JieriData.XmlList[index]);
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_SuperDirectBuyPart.gameObject, true);
			}
			break;
		case 31:
			if (null == this.m_JieRiVIPlibaoPart)
			{
				this.m_JieRiVIPlibaoPart = U3DUtils.NEW<JieRiVIPlibaoPart>();
				this.m_JieRiVIPlibaoPart.InitXml(Global.Data.JieriData.XmlList[index]);
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_JieRiVIPlibaoPart.gameObject, true);
			}
			break;
		case 32:
			if (null == this.m_DanBiChongZhiPart)
			{
				this.m_DanBiChongZhiPart = U3DUtils.NEW<DanBiChongZhiPart>();
				this.m_DanBiChongZhiPart.InitXml(Global.Data.JieriData.XmlList[index]);
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_DanBiChongZhiPart.gameObject, true);
			}
			break;
		case 33:
			this.m_MUJieriMeiriLeichongPart = U3DUtils.NEW<MUJieriMeiriLeichongPart>();
			this.m_MUJieriMeiriLeichongPart.InitData(Global.Data.JieriData.XmlList[index]);
			this.m_MUJieriMeiriLeichongPart.mInitGoodsInfo(this.m_MUJieriMeiriLeichongPart.mGetJieriDays());
			this.m_MUJieriMeiriLeichongPart.SetBak("mujierihuodong_MeiriLeichong");
			U3DUtils.AddChild(this.PnlContent.gameObject, this.m_MUJieriMeiriLeichongPart.gameObject, true);
			break;
		case 34:
			if (null == this.m_HuoDongChongZhiFanLiPart)
			{
				this.m_HuoDongChongZhiFanLiPart = U3DUtils.NEW<HuoDongChongZhiFanLiPart>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HuoDongChongZhiFanLiPart.gameObject, true);
				this.m_HuoDongChongZhiFanLiPart.XmlTime(Global.Data.JieriData.XmlList[index]);
			}
			break;
		case 35:
			if (null == this.m_HongBaoQuanMinPart)
			{
				this.m_HongBaoQuanMinPart = U3DUtils.NEW<HongBaoQuanMinPart>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HongBaoQuanMinPart.gameObject, true);
				this.m_HongBaoQuanMinPart.SetXml(Global.Data.JieriData.XmlList[index]);
			}
			break;
		case 36:
			if (null == this.m_HongBaoChongZhiPart)
			{
				this.m_HongBaoChongZhiPart = U3DUtils.NEW<HongBaoChongZhiPart>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HongBaoChongZhiPart.gameObject, true);
				this.m_HongBaoChongZhiPart.SetXml(Global.Data.JieriData.XmlList[index]);
			}
			break;
		case 37:
			if (null == this.m_HongBaoPaiHangPart)
			{
				this.m_HongBaoPaiHangPart = U3DUtils.NEW<HongBaoPaiHangPart>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.m_HongBaoPaiHangPart.gameObject, true);
				this.m_HongBaoPaiHangPart.SetXml(Global.Data.JieriData.XmlList[index]);
			}
			break;
		case 40:
			if (this.mMUJieRiMeiRiPingTaiChongZhiKingPart == null)
			{
				this.mMUJieRiMeiRiPingTaiChongZhiKingPart = U3DUtils.NEW<MUJieRiMeiRiPingTaiChongZhiKingPart>();
				U3DUtils.AddChild(this.PnlContent.gameObject, this.mMUJieRiMeiRiPingTaiChongZhiKingPart.gameObject, true);
			}
			this.mMUJieRiMeiRiPingTaiChongZhiKingPart.InitData(Global.Data.JieriData.XmlList[index]);
			break;
		}
	}

	private void ChangeTrZ(Transform tr)
	{
		if (null != tr)
		{
			Vector3 localPosition = tr.localPosition;
			localPosition.z += 2.5f;
			tr.localPosition = localPosition;
		}
	}

	private void ChangeZAndDepth(UIWidget widget)
	{
		Vector3 localPosition = widget.transform.localPosition;
		localPosition.z = -2f;
		widget.transform.localPosition = localPosition;
		widget.depth += 10;
	}

	private void AddPanel(GameObject obj)
	{
		if (null != obj)
		{
			UIPanel uipanel = obj.GetComponent<UIPanel>();
			if (null == uipanel)
			{
				uipanel = obj.gameObject.AddComponent<UIPanel>();
			}
			Vector3 localPosition = uipanel.transform.localPosition;
			localPosition.z = -2f;
			uipanel.transform.localPosition = localPosition;
		}
	}

	public void LingquCompleted(int activityType, int result, int exTag = -1, int index = -1)
	{
		switch (activityType)
		{
		case 53:
		case 54:
		case 55:
		case 56:
		case 57:
		case 58:
		case 59:
		case 60:
		case 62:
		case 75:
		case 76:
			if (this.m_MUJieriTongyongFanliPart != null)
			{
				this.m_MUJieriTongyongFanliPart.SetCompletedInfo(result, exTag);
			}
			break;
		default:
			switch (activityType)
			{
			case 9:
				if (this.m_MUJieRipartLibao != null)
				{
					this.m_MUJieRipartLibao.setCompletedInfo(result);
				}
				break;
			case 10:
				if (this.m_MUJieRiPartLeijiDenglu != null)
				{
					this.m_MUJieRiPartLeijiDenglu.setCompletedInfo(result, exTag);
				}
				break;
			case 11:
				break;
			case 12:
				if (this.m_MUJieriPartmeiriChongzhi != null)
				{
					this.m_MUJieriPartmeiriChongzhi.setCompletedInfo(result, exTag);
				}
				break;
			case 13:
				if (this.m_MUJieriPartLeijiChongzhi != null)
				{
					this.m_MUJieriPartLeijiChongzhi.setCompletedInfo(result, exTag);
				}
				break;
			case 14:
				if (this.m_MUJieRipartShouji != null)
				{
					this.m_MUJieRipartShouji.setCompletedInfo(result, index, exTag);
				}
				if (this.m_MUJieriPartDuihuan != null)
				{
					this.m_MUJieriPartDuihuan.setCompletedInfo(result, index, exTag);
				}
				break;
			case 15:
				if (this.m_MUJieriPartXiaofeiking != null)
				{
					this.m_MUJieriPartXiaofeiking.setCompletedInfo(result, exTag);
				}
				break;
			case 16:
				if (this.m_MUJieripartChongzhiKing != null)
				{
					this.m_MUJieripartChongzhiKing.setCompletedInfo(result, exTag);
				}
				break;
			default:
				if (activityType == 40)
				{
					if (this.m_MUJieriPartLeijiXiaofei != null)
					{
						this.m_MUJieriPartLeijiXiaofei.setCompletedInfo(result, exTag);
					}
				}
				break;
			}
			break;
		case 64:
			if (null != this.redemptionActivity)
			{
				this.redemptionActivity.SetRedemptionItemLeftTimes(result, index, exTag);
			}
			break;
		case 70:
			if (this.m_MUJieriMeiriLeichongPart != null)
			{
				this.m_MUJieriMeiriLeichongPart.SetCompletedInfo(result, exTag);
			}
			break;
		}
		if (result < 0)
		{
			if (result == -10005)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你已经领取过了"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10006)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("活动期间充值额度为0，不能领取"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10007)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("不满足领取条件"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10077)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前的隔天登陆次数尚未达到, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10088)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前充值额度不足, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10099)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前还不是VIP, 无法领取VIP奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10888)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前积分不足, 无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -20000)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成节日礼盒今日的次数已经为0，请明日再进行合成操作"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -20003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("合成节日礼盒时需要的材料不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -20004)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedMojing, null, string.Empty, string.Empty);
			}
			else if (result == -20005)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedQifujifen, null, string.Empty, string.Empty);
			}
			else if (result == -1003)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你当前不在排行榜内，无法领取奖励"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("现在不是领取时间"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("你的背包空格不够"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
	}

	public void RespondChongZhiFanLiData(string fileds)
	{
		if (this.m_HuoDongChongZhiFanLiPart != null)
		{
			this.m_HuoDongChongZhiFanLiPart.RespondFanLiData(fileds);
		}
	}

	private new void OnDestroy()
	{
		ActivityTipManager.RegActivityTipItem(14001, null);
		ActivityTipManager.RegActivityTipItem(14002, null);
		ActivityTipManager.RegActivityTipItem(14003, null);
		ActivityTipManager.RegActivityTipItem(14004, null);
		ActivityTipManager.RegActivityTipItem(14005, null);
		ActivityTipManager.RegActivityTipItem(14006, null);
		ActivityTipManager.RegActivityTipItem(14007, null);
		ActivityTipManager.RegActivityTipItem(14008, null);
		ActivityTipManager.RegActivityTipItem(14009, null);
		ActivityTipManager.RegActivityTipItem(14010, null);
		ActivityTipManager.RegActivityTipItem(14011, null);
		ActivityTipManager.RegActivityTipItem(14012, null);
		ActivityTipManager.RegActivityTipItem(14013, null);
		ActivityTipManager.RegActivityTipItem(14014, null);
		ActivityTipManager.RegActivityTipItem(14015, null);
		ActivityTipManager.RegActivityTipItem(14016, null);
		ActivityTipManager.RegActivityTipItem(14017, null);
		ActivityTipManager.RegActivityTipItem(14018, null);
		ActivityTipManager.RegActivityTipItem(14019, null);
		ActivityTipManager.RegActivityTipItem(14020, null);
		ActivityTipManager.RegActivityTipItem(14021, null);
		ActivityTipManager.RegActivityTipItem(14023, null);
		ActivityTipManager.RegActivityTipItem(14024, null);
		ActivityTipManager.RegActivityTipItem(15052, null);
		ActivityTipManager.RegActivityTipItem(15051, null);
		ActivityTipManager.RegActivityTipItem(14028, null);
		ActivityTipManager.RegActivityTipItem(14032, null);
		ActivityTipManager.RegActivityTipItem(15053, null);
		ActivityTipManager.RegActivityTipItem(11501, null);
		ActivityTipManager.RegActivityTipItem(11502, null);
		ActivityTipManager.RegActivityTipItem(14033, null);
		ActivityTipManager.RegActivityTipItem(14034, null);
		ActivityTipManager.RegActivityTipItem(14035, null);
		ActivityTipManager.RegActivityTipItem(14027, null);
		ActivityTipManager.RegActivityTipItem(1515, null);
		ActivityTipManager.RegActivityTipItem(15054, null);
	}

	public GameObject PnlContent;

	public GameObject BtnItem;

	public SpriteSL SprPnlContent;

	private ObservableCollection TabBtnOBC;

	public ListBox ListTabBtn;

	public GButton BtnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public MUJieRipartLibao m_MUJieRipartLibao;

	public MUJieRipartShouji m_MUJieRipartShouji;

	public MUJieRiPartLeijiDenglu m_MUJieRiPartLeijiDenglu;

	public MUJieriPartmeiriChongzhi m_MUJieriPartmeiriChongzhi;

	public MUJieriPartLeijiXiaofei m_MUJieriPartLeijiXiaofei;

	public MUJieriPartLeijiChongzhi m_MUJieriPartLeijiChongzhi;

	public MUJieripartChongzhiKing m_MUJieripartChongzhiKing;

	public MUJieriPartXiaofeiking m_MUJieriPartXiaofeiking;

	public MUJieriPartBoss m_MUJieriPartBoss;

	public MUJieriPartQianggou m_MUJieriPartQianggou;

	public MUJieriPartFanbei m_MUJieriPartFanbei;

	public MUJieriPartDuihuan m_MUJieriPartDuihuan;

	private Dictionary<int, string> ItemsDict = new Dictionary<int, string>();

	private Dictionary<int, int> TipDict = new Dictionary<int, int>();

	private Dictionary<int, JieRiTypeItem> btnItemDict = new Dictionary<int, JieRiTypeItem>();

	public MUJieriZengsongPart m_MUJieriZengsongPart;

	public MUJieriZengsongZengKingPart m_MUJieriZengsongZengKingPart;

	public MUJieriTongyongFanliPart m_MUJieriTongyongFanliPart;

	public MUJieriLianxuChongzhiPart m_MUJieriLianxuChongzhiPart;

	public RedemptionActivity redemptionActivity;

	public MUJieriPartFuLi m_MUJieriPartFuLi;

	public SuperDirectBuyPart m_SuperDirectBuyPart;

	public JieRiVIPlibaoPart m_JieRiVIPlibaoPart;

	public DanBiChongZhiPart m_DanBiChongZhiPart;

	public HuoDongChongZhiFanLiPart m_HuoDongChongZhiFanLiPart;

	public HongBaoChongZhiPart m_HongBaoChongZhiPart;

	public HongBaoPaiHangPart m_HongBaoPaiHangPart;

	public HongBaoQuanMinPart m_HongBaoQuanMinPart;

	public MUJieriMeiriLeichongPart m_MUJieriMeiriLeichongPart;

	public MUJieRiMeiRiPingTaiChongZhiKingPart mMUJieRiMeiRiPingTaiChongZhiKingPart;

	private JieRiTypeItem jieriBtnItem;
}
