using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class OlympicsYesterdayRecordPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.title.Text = Global.GetLang("昨日记录");
		this.btnConfirm.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.itemList.ItemsSource;
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
		this.btnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void InitData(List<OlympicsGuessData> tmpList)
	{
		if (tmpList == null || tmpList.Count <= 0)
		{
			return;
		}
		this.ItemCollection.Clear();
		for (int i = 0; i < tmpList.Count; i++)
		{
			OlympicsYesterdayRecordItem olympicsYesterdayRecordItem = U3DUtils.NEW<OlympicsYesterdayRecordItem>();
			OlympicsGuessData data = tmpList[i];
			olympicsYesterdayRecordItem.SetValue(i, data);
			UIPanel component = olympicsYesterdayRecordItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, olympicsYesterdayRecordItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(olympicsYesterdayRecordItem);
		}
	}

	public TextBlock title;

	public GButton btnClose;

	public GButton btnConfirm;

	public ListBox itemList;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler Hander;
}
