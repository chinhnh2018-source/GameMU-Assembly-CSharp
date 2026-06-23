using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class OlympicAwardPart : UserControl
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
		this.title.Text = Global.GetLang("总积分排行奖励");
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

	public void InitData()
	{
		this.ItemCollection.Clear();
		if (OlympicsDataManage.GetAwardData().Count <= 0)
		{
			return;
		}
		for (int i = 1; i <= OlympicsDataManage.GetAwardData().Count; i++)
		{
			OlympicAwardItem olympicAwardItem = U3DUtils.NEW<OlympicAwardItem>();
			olympicAwardItem.Goods = OlympicsDataManage.GetAwardData()[i].Award;
			olympicAwardItem.Level = OlympicsDataManage.GetAwardData()[i].Name;
			UIPanel component = olympicAwardItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			U3DUtils.AddChild(this.itemList.gameObject, olympicAwardItem.gameObject, true);
			this.ItemCollection.AddNoUpdate(olympicAwardItem);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		base.parent = null;
	}

	public TextBlock title;

	public GButton btnClose;

	public ListBox itemList;

	private ObservableCollection _ItemCollection;

	public DPSelectedItemEventHandler Hander;
}
