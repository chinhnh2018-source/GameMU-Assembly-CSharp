using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class OlympicsSumRank : UserControl
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
		this.title.Text = Global.GetLang("总排名");
		this.leftName.Text = Global.GetLang("排行");
		this.middleName.Text = Global.GetLang("角色");
		this.rightName.Text = Global.GetLang("积分");
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
	}

	public IEnumerator InitData(List<KFRankData> tmpList)
	{
		if (OlympicsDataManage.GetCurrentPlayerIndexOfRank() < 0)
		{
			this.rankNum.Text = Global.GetLang("未上榜");
		}
		else
		{
			this.rankNum.Text = OlympicsDataManage.GetCurrentPlayerIndexOfRank().ToString();
		}
		this.ItemCollection.Clear();
		int currentListCount = tmpList.Count;
		if (currentListCount > 100)
		{
			currentListCount = 100;
		}
		for (int i = 0; i < currentListCount; i++)
		{
			if (i % 10 == 0)
			{
				yield return null;
			}
			OlympicsRankItem item = U3DUtils.NEW<OlympicsRankItem>();
			KFRankData data = tmpList[i];
			item.SetValue(data);
			UIPanel temppanel = item.transform.GetComponent<UIPanel>();
			if (temppanel != null)
			{
				Object.Destroy(temppanel);
			}
			U3DUtils.AddChild(this.itemList.gameObject, item.gameObject, true);
			this.ItemCollection.AddNoUpdate(item);
		}
		yield break;
	}

	public GButton btnClose;

	public TextBlock title;

	public TextBlock leftName;

	public TextBlock middleName;

	public TextBlock rightName;

	public TextBlock rankNum;

	public ListBox itemList;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler Hander;
}
