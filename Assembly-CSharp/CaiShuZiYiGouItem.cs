using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CaiShuZiYiGouItem : UserControl
{
	public string Number { get; set; }

	public new int Count { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.btn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.handler(this, new DPSelectedItemEventArgs
			{
				Title = this.Number
			});
		};
	}

	public void SetData(string numbers, int count)
	{
		this.Count = count;
		this.Number = numbers;
		this.labNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36b",
			Global.GetLang(string.Format(Global.GetLang("购买注数：{0}注"), Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				this.Count
			})))
		});
		string[] array = this.Number.Split(new char[]
		{
			','
		});
		if (array.Length != 5)
		{
			return;
		}
		if (this.obser == null)
		{
			this.obser = this.listBox.ItemsSource;
		}
		this.obser.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			CaiShuZiItem caiShuZiItem = U3DUtils.NEW<CaiShuZiItem>();
			if (caiShuZiItem != null)
			{
				this.obser.AddNoUpdate(caiShuZiItem);
				if (caiShuZiItem.GetComponent<UIPanel>() != null)
				{
					Object.Destroy(caiShuZiItem.GetComponent<UIPanel>());
				}
				caiShuZiItem.SetIitemType(CaiShuZiType.KaiJiang);
				caiShuZiItem.SpNumber = array[i].SafeToInt32(0);
			}
		}
	}

	public ListBox listBox;

	public UILabel labNumber;

	public GButton btn;

	public DPSelectedItemEventHandler handler;

	public ObservableCollection obser;
}
