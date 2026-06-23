using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class LianDengEveryDayItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnLingJiang.Text = Global.GetLang("领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_ListWuPin)
		{
			this.m_ListWuPinObC = this.m_ListWuPin.ItemsSource;
		}
		if (null != this.m_BtnLingJiang)
		{
			this.m_BtnLingJiang.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = this.m_nEventID
				});
			};
		}
		this.SetCollider();
	}

	private void SetCollider()
	{
		BoxCollider[] componentsInChildren = base.GetComponentsInChildren<BoxCollider>();
		if (componentsInChildren != null && 0 < componentsInChildren.Length)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].isTrigger = false;
			}
		}
	}

	public UILabel m_LblDayNum;

	public UILabel m_LblDayNumLingQv;

	public UISprite m_SpriteState;

	public ListBox m_ListWuPin = new ListBox();

	public ObservableCollection m_ListWuPinObC;

	public GButton m_BtnLingJiang;

	public DPSelectedItemEventHandler DPSelectedItem;

	public int m_nEventID;
}
