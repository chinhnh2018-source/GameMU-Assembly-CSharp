using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenShiPartGetShenShi : UserControl
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
		this.BtnSure.Label.text = Global.GetLang("确定");
		this.BtnGo.Label.text = Global.GetLang("前往激活");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.ListAward.ItemsSource;
		this.BtnSure.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.BtnGo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs
			{
				ID = 22
			});
		};
	}

	public void SetServerData(string goodid)
	{
		string[] array = goodid.Split(new char[]
		{
			','
		});
		if (array.Length <= 0)
		{
			return;
		}
		this.AnimWin.gameObject.SetActive(true);
		int num = array.Length;
		if (array.Length > 10)
		{
			num = 10;
		}
		if (num == 1)
		{
			this.ListAward.transform.localPosition = new Vector3(0f, 30f, -2f);
		}
		else if (num == 2)
		{
			this.ListAward.transform.localPosition = new Vector3(-50f, 30f, -2f);
		}
		else if (num == 3)
		{
			this.ListAward.transform.localPosition = new Vector3(-100f, 30f, -2f);
		}
		for (int i = 0; i < num; i++)
		{
			ShenShiPartShenShiItem shenShiPartShenShiItem = U3DUtils.NEW<ShenShiPartShenShiItem>();
			int num2 = int.Parse(array[i]) / 100;
			int num3 = int.Parse(array[i]) % 100;
			int goodID = 0;
			if (ShenShiPart.GetDicFuWenGod().ContainsKey(num2) && ShenShiPart.GetDicFuWenGod()[num2].ContainsKey(num3))
			{
				goodID = ShenShiPart.GetDicFuWenGod()[num2][num3].GodID;
			}
			shenShiPartShenShiItem.goodID = goodID;
			shenShiPartShenShiItem.ShowLevel.text = string.Format("Lv.{0}", num3);
			shenShiPartShenShiItem.isSelect = false;
			shenShiPartShenShiItem.openBoxC0llider = true;
			shenShiPartShenShiItem.openBtnClose = true;
			this.ItemCollection.AddNoUpdate(shenShiPartShenShiItem);
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public Animator AnimWin;

	public GButton BtnSure;

	public GButton BtnGo;

	public ListBox ListAward;

	private ObservableCollection _ItemCollection;
}
