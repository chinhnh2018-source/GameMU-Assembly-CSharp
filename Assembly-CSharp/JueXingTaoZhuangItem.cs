using System;
using System.Collections.Generic;
using UnityEngine;
using XMLCreater;

public class JueXingTaoZhuangItem : MonoBehaviour
{
	private void Awake()
	{
		for (int i = 0; i < this.JueXingShiItems.Count; i++)
		{
			this.JueXingShiItems[i].OnSelectAction = new Action<JueXingShiItem>(this.OnSelectItem);
		}
	}

	public void OnSelectItem(JueXingShiItem item)
	{
		if (this.m_selectedItem == item)
		{
			return;
		}
		item.SetSelectState(true);
		if (this.m_selectedItem != null)
		{
			this.m_selectedItem.SetSelectState(false);
		}
		this.m_selectedItem = item;
		if (this.OnSelectJueXingShi != null)
		{
			this.OnSelectJueXingShi.Invoke(this.m_selectedItem);
		}
	}

	public void SetJueXingShis(MUAwakenSuitDetail taoZhuang)
	{
		if (taoZhuang == null)
		{
			return;
		}
		this.m_taoZhuangInfo = taoZhuang;
		if (this.m_selectedItem != null)
		{
			this.m_selectedItem.SetSelectState(false);
			this.m_selectedItem = null;
		}
		if (this.JueXingShiItems.Count != this.m_taoZhuangInfo.AwakenIDs.Count)
		{
			MUDebug.LogError<string>(new string[]
			{
				"觉醒石总数填充数组数目不对"
			});
			return;
		}
		for (int i = 0; i < this.JueXingShiItems.Count; i++)
		{
			this.JueXingShiItems[i].SetJueXingShiId(this.m_taoZhuangInfo.AwakenIDs[i], taoZhuang.ID);
		}
		this.imgEquip.URL = JueXingData.GetJueXingTaoZhuangIconUrl(taoZhuang);
		this.OnSelectItem(this.JueXingShiItems[0]);
	}

	public void SetNoTaoZhuang()
	{
		this.imgEquip.URL = string.Empty;
		for (int i = 0; i < this.JueXingShiItems.Count; i++)
		{
			this.JueXingShiItems[i].SetNotVisable();
		}
	}

	private JueXingShiItem GetItemByPositionId(int position)
	{
		if (this.type == JueXingTaoZhuangType.DefenseType)
		{
			int num = position - 4 - 1;
			if (num < 0 || num >= this.JueXingShiItems.Count)
			{
				return null;
			}
			return this.JueXingShiItems[num];
		}
		else
		{
			int num2 = position - 1;
			if (num2 < 0 || num2 >= this.JueXingShiItems.Count)
			{
				return null;
			}
			return this.JueXingShiItems[num2];
		}
	}

	public ShowNetImage imgEquip;

	public List<JueXingShiItem> JueXingShiItems;

	public JueXingTaoZhuangType type;

	public Action<JueXingShiItem> OnSelectJueXingShi;

	private JueXingShiItem m_selectedItem;

	private MUAwakenSuitDetail m_taoZhuangInfo;
}
