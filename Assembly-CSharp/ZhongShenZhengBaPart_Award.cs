using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhongShenZhengBaPart_Award : UserControl
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
		if (ZhongShenZhengBaPart.GetDicMatchAward().Count <= 0)
		{
			return;
		}
		for (int i = ZhongShenZhengBaPart.GetDicMatchAward().Count; i >= 1; i--)
		{
			ZhongShenZhengBaPart_AwardList zhongShenZhengBaPart_AwardList = U3DUtils.NEW<ZhongShenZhengBaPart_AwardList>();
			zhongShenZhengBaPart_AwardList.Goods = ZhongShenZhengBaPart.GetDicMatchAward()[i].Award;
			zhongShenZhengBaPart_AwardList.Level = ZhongShenZhengBaPart.GetDicMatchAward()[i].Name + Global.GetLang("奖励");
			this.ItemCollection.AddNoUpdate(zhongShenZhengBaPart_AwardList);
			UIPanel component = zhongShenZhengBaPart_AwardList.GetComponent<UIPanel>();
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
