using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class JueXingPartSelect : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("装备套装");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnConfig.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_selectedItem == null)
			{
				return;
			}
			int taoZhuangId;
			if (this.m_equipItem == this.m_selectedItem)
			{
				taoZhuangId = 0;
			}
			else
			{
				taoZhuangId = this.m_selectedItem.GetTaoZhuang().ID;
			}
			this.SendJuHuoInfo((int)this.m_type, taoZhuangId);
		};
	}

	public void InitTaoZhuangs(JueXingTaoZhuangType type)
	{
		this.lstAllTaoZhuangs.Clear();
		this.m_type = type;
		int equipTaoZhuangId = JueXingData.GetEquipTaoZhuangId(type);
		List<MUAwakenSuitDetail> taoZhuangsByType = JueXingData.GetTaoZhuangsByType(type);
		JueXingPartSelectItem jueXingPartSelectItem = null;
		int num = 0;
		for (int i = 0; i < taoZhuangsByType.Count; i++)
		{
			if (taoZhuangsByType[i].TaoZhuangProps1Num <= JueXingData.GetJiHuoJueXingShiNum(taoZhuangsByType[i]))
			{
				num++;
				JueXingPartSelectItem jueXingPartSelectItem2 = U3DUtils.NEW<JueXingPartSelectItem>();
				if (num == 1)
				{
					jueXingPartSelectItem = jueXingPartSelectItem2;
				}
				jueXingPartSelectItem2.transform.SetParent(this.itemsContainer);
				jueXingPartSelectItem2.transform.localPosition = Vector3.zero;
				jueXingPartSelectItem2.transform.localScale = Vector3.one;
				jueXingPartSelectItem2.OnItemSelect = new Action<JueXingPartSelectItem>(this.OnItemSelect);
				jueXingPartSelectItem2.SetTaoZhuang(taoZhuangsByType[i]);
				jueXingPartSelectItem2.SetEquip(false);
				jueXingPartSelectItem2.SetSelect(false);
				if (taoZhuangsByType[i].ID == equipTaoZhuangId)
				{
					jueXingPartSelectItem = jueXingPartSelectItem2;
				}
				this.lstAllTaoZhuangs.Add(jueXingPartSelectItem2);
			}
		}
		if (jueXingPartSelectItem != null)
		{
			if (jueXingPartSelectItem.GetTaoZhuang().ID == equipTaoZhuangId)
			{
				this.m_equipItem = jueXingPartSelectItem;
				jueXingPartSelectItem.SetEquip(true);
			}
			jueXingPartSelectItem.SetSelect(true);
			this.OnItemSelect(jueXingPartSelectItem);
		}
		else
		{
			this.SetSelectId(null);
		}
		int num2 = (num - 1) / 4 + 1;
		num2 = Mathf.Max(num2, 5);
		this.imgBg.localScale = new Vector3(this.imgBg.localScale.x, (float)(num2 * 78), 1f);
	}

	public void SetEquipTaoZhuang(int taoZhuangId)
	{
		JueXingPartSelectItem jueXingPartSelectItem = this.lstAllTaoZhuangs.Find((JueXingPartSelectItem item) => item.GetTaoZhuang().ID == taoZhuangId);
		if (jueXingPartSelectItem == null)
		{
			if (this.m_equipItem != null)
			{
				this.m_equipItem.SetEquip(false);
			}
			this.m_equipItem = null;
		}
		else if (!(jueXingPartSelectItem == this.m_equipItem))
		{
			if (this.m_equipItem == null)
			{
				this.m_equipItem = jueXingPartSelectItem;
				this.m_equipItem.SetEquip(true);
			}
			else if (this.m_equipItem != null)
			{
				this.m_equipItem.SetEquip(false);
				this.m_equipItem = jueXingPartSelectItem;
				this.m_equipItem.SetEquip(true);
			}
		}
		if (this.m_equipItem == this.m_selectedItem)
		{
			this.btnConfig.Text = Global.GetLang("取下");
		}
		else
		{
			this.btnConfig.Text = Global.GetLang("穿戴");
		}
	}

	private void OnItemSelect(JueXingPartSelectItem item)
	{
		if (this.m_selectedItem == item)
		{
			return;
		}
		item.SetSelect(true);
		if (this.m_selectedItem != null)
		{
			this.m_selectedItem.SetSelect(false);
		}
		this.m_selectedItem = item;
		this.SetSelectId(item.GetTaoZhuang());
	}

	private void SetSelectId(MUAwakenSuitDetail taoZhuangIfno)
	{
		if (taoZhuangIfno == null)
		{
			this.lblName.text = string.Empty;
			this.lblShuXing.text = string.Empty;
			this.imgIcon.gameObject.SetActive(false);
			this.btnConfig.isEnabled = false;
			this.btnConfig.Text = Global.GetLang("穿戴");
		}
		else
		{
			this.lblName.text = string.Concat(new object[]
			{
				Global.GetLang(taoZhuangIfno.Name),
				"(",
				JueXingData.GetJiHuoJueXingShiNum(taoZhuangIfno),
				"/",
				taoZhuangIfno.AwakenIDs.Count,
				")"
			});
			TaoZhuangDesSettingInfo settingInfo = new TaoZhuangDesSettingInfo();
			this.lblShuXing.text = JueXingData.GetTaoZhuangShuXingDes(taoZhuangIfno, settingInfo);
			this.imgIcon.URL = JueXingData.GetJueXingTaoZhuangIconUrl(taoZhuangIfno);
			this.imgIcon.gameObject.SetActive(true);
			this.btnConfig.isEnabled = true;
			if (this.m_equipItem == this.m_selectedItem)
			{
				this.btnConfig.Text = Global.GetLang("取下");
			}
			else
			{
				this.btnConfig.Text = Global.GetLang("穿戴");
			}
		}
	}

	private void SendJuHuoInfo(int type, int taoZhuangId)
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.JueXingTaoZhuangSelect(type, taoZhuangId);
	}

	private const string ShuXingNameColor = "cea46c";

	private const string ShuXingValuecolor = "f7f7de";

	private const int aGridSize = 78;

	private const int ColNum = 4;

	public GButton btnConfig;

	public GButton BtnClose;

	public Transform itemsContainer;

	public UILabel lblTitle;

	public UILabel lblName;

	public UILabel lblShuXing;

	public ShowNetImage imgIcon;

	public Transform imgBg;

	public DPSelectedItemEventHandler CloseHandler;

	private JueXingTaoZhuangType m_type;

	private JueXingPartSelectItem m_selectedItem;

	private JueXingPartSelectItem m_equipItem;

	private List<JueXingPartSelectItem> lstAllTaoZhuangs = new List<JueXingPartSelectItem>();
}
