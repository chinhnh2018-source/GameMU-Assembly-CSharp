using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LoversWishPartAward : UserControl
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
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("奖励预览")
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.AwardList.ItemsSource;
		this.InitAwardList();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public override void Destroy()
	{
		base.Destroy();
		base.parent = null;
	}

	public void InitAwardList()
	{
		if (LoversWishPart.GetWishAwardDic().Count <= 0)
		{
			return;
		}
		for (int i = 1; i <= LoversWishPart.GetWishAwardDic().Count; i++)
		{
			LoversWishPartAwardList loversWishPartAwardList = U3DUtils.NEW<LoversWishPartAwardList>();
			string text = LoversWishPart.GetWishAwardDic()[i].GoodsOne;
			if (LoversWishPart.GetWishAwardDic()[i].GoodsTwo != null)
			{
				text = text + "|" + LoversWishPart.GetWishAwardDic()[i].GoodsTwo;
			}
			loversWishPartAwardList.Goods = text;
			loversWishPartAwardList.Level = LoversWishPart.GetWishAwardDic()[i].Name;
			this.ItemCollection.AddNoUpdate(loversWishPartAwardList);
			UIPanel component = loversWishPartAwardList.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	public GButton Close;

	public ListBox AwardList;

	public UILabel Title;

	public DPSelectedItemEventHandler CloseHandler;

	private ObservableCollection _ItemCollection;
}
