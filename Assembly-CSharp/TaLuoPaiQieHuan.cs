using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TaLuoPaiQieHuan : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_UILabel.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			string.Format("{0}", Global.GetLang("选择塔罗牌"))
		});
		this.m_ObservableCollection_ListBox = this.m_ListBox.ItemsSource;
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0,
					MyID = 0
				});
			}
		};
	}

	public Dictionary<int, int> KingData
	{
		get
		{
			return this.m_KingData;
		}
		set
		{
			this.m_KingData = value;
		}
	}

	public void RefreshItem(int GoodId)
	{
		for (int i = 0; i < this.m_ObservableCollection_ListBox.Count; i++)
		{
			GameObject at = this.m_ObservableCollection_ListBox.GetAt(i);
			if (null != at)
			{
				TaLuoPaiQieHuanItem component = at.GetComponent<TaLuoPaiQieHuanItem>();
				if (GoodId == component.ItemId)
				{
					component.Refresh();
				}
				else
				{
					component.btnType = 0;
				}
			}
		}
	}

	public void RefreshContent(List<TarotDataAndXmlData> Lst)
	{
		base.StartCoroutine<bool>(this.AddItemToListBox(Lst));
	}

	private IEnumerator AddItemToListBox(List<TarotDataAndXmlData> Lst)
	{
		for (int i = 0; i < Lst.Count; i++)
		{
			if (i % 5 == 0 && i != 0)
			{
				yield return null;
			}
			TaLuoPaiQieHuanItem item = U3DUtils.NEW<TaLuoPaiQieHuanItem>();
			if (this.m_KingData != null && this.m_KingData.Count == 3)
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.m_KingData)
				{
					int goodID = keyValuePair.Key;
					Dictionary<int, int>.Enumerator enKingData;
					KeyValuePair<int, int> keyValuePair2 = enKingData.Current;
					int level = keyValuePair2.Value;
					if (Lst[i].data.GoodId == goodID)
					{
						item.ExtraLevel = level;
					}
				}
			}
			item.Name = Lst[i].xmlData.Name;
			item.SetContent(Lst[i].data);
			item.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = s.ID,
						MyID = s.MyID
					});
				}
			};
			this.m_ObservableCollection_ListBox.AddNoUpdate(item);
			item.DeletePanel(null);
		}
		this.m_ListBox.repositionNow = true;
		yield break;
	}

	private int PaiXu(TarotDataAndXmlData a, TarotDataAndXmlData b)
	{
		if (a.data == null || b.data == null)
		{
			return a.xmlData.ID - b.xmlData.ID;
		}
		if (a.data.Level == b.data.Level)
		{
			return a.xmlData.ID - b.xmlData.ID;
		}
		return b.data.Level - a.data.Level;
	}

	public int GoodsID
	{
		get
		{
			return this.m_GoodsID;
		}
		set
		{
			this.m_GoodsID = value;
		}
	}

	public UILabel m_UILabel;

	public GButton m_CloseBtn;

	public ListBox m_ListBox;

	private ObservableCollection m_ObservableCollection_ListBox;

	private int m_GoodsID;

	private Dictionary<int, int> m_KingData = new Dictionary<int, int>();

	public DPSelectedItemEventHandler Hander;
}
