using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class RidePetZuHeItem : UserControl
{
	public UIDraggablePanel DragPanel
	{
		set
		{
			UIDragPanelContents uidragPanelContents = base.GetComponent<UIDragPanelContents>();
			if (null == uidragPanelContents)
			{
				uidragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			uidragPanelContents.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	public void SetInf(List<int> goods, string[] str, bool isGray)
	{
		RidePetZuHeItem.<SetInf>c__AnonStorey37F <SetInf>c__AnonStorey37F = new RidePetZuHeItem.<SetInf>c__AnonStorey37F();
		<SetInf>c__AnonStorey37F.goods = goods;
		ObservableCollection itemsSource = this.m_ListGoods.ItemsSource;
		int i;
		for (i = 0; i < <SetInf>c__AnonStorey37F.goods.Count; i++)
		{
			GGoodIcon ggoodIcon = Super.AddGoodsIcon(Global.GetDummyGoodsDataMu(<SetInf>c__AnonStorey37F.goods[i], 0, 0, 0, 0, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00"), itemsSource, isGray, true);
			if (null != ggoodIcon)
			{
				bool flag = Global.Data.roleData.MountEquipList != null && Global.Data.roleData.MountEquipList.Find((GoodsData e) => e.GoodsID == <SetInf>c__AnonStorey37F.goods[i]) != null;
				ggoodIcon.GoodImg.ToGrayBitmap = !flag;
				UIWidget[] componentsInChildren = ggoodIcon.TeXiao.GetComponentsInChildren<UIWidget>();
				if (0 < componentsInChildren.Length)
				{
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						if (null != componentsInChildren[k])
						{
							componentsInChildren[k].enabled = flag;
						}
					}
				}
				if (null != ggoodIcon.transform.GetComponent<BoxCollider>())
				{
					ggoodIcon.transform.GetComponent<BoxCollider>().enabled = false;
				}
				if (ggoodIcon.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(ggoodIcon.GetComponent<UIPanel>());
				}
			}
		}
		for (int j = 0; j < this.text.Length; j++)
		{
			if (j < str.Length && !string.IsNullOrEmpty(str[j]))
			{
				this.text[j].text = Global.GetColorStringForNGUIText(new object[]
				{
					(!isGray) ? "17e43e" : "808081",
					str[j]
				});
				this.text[j].transform.parent.gameObject.SetActive(true);
			}
			else
			{
				this.text[j].transform.parent.gameObject.SetActive(false);
			}
		}
	}

	public ListBox m_ListGoods;

	public UILabel[] text;
}
