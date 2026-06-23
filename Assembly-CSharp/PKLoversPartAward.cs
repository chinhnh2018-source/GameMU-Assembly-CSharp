using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PKLoversPartAward : UserControl
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
		if (PKLoversPart.GetCoupleWarAwardDic().Count <= 0)
		{
			return;
		}
		for (int i = 1; i < PKLoversPart.GetCoupleWarAwardDic().Count; i++)
		{
			PKLoversPartAwardList pkloversPartAwardList = U3DUtils.NEW<PKLoversPartAwardList>();
			pkloversPartAwardList.Goods = PKLoversPart.GetCoupleWarAwardDic()[i].Award;
			pkloversPartAwardList.Level = PKLoversPart.GetCoupleWarAwardDic()[i].Name;
			this.ItemCollection.AddNoUpdate(pkloversPartAwardList);
			UIPanel component = pkloversPartAwardList.GetComponent<UIPanel>();
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
