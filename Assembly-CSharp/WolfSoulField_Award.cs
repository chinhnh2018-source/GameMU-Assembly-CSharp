using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class WolfSoulField_Award : UserControl
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
			Global.GetLang("每日奖励")
		});
		this.Lingqu.Label.text = Global.GetLang("领取");
		this.chenggong.text = Global.GetLang("今日已领取");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetLingQuState();
		this.ItemCollection = this.RewardList.ItemsSource;
		this.InitData();
		this.Lingqu.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.Lingqu.isEnabled)
			{
				return;
			}
			GameInstance.Game.GetDayAward();
		};
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelcetItem(this, new DPSelectedItemEventArgs
			{
				ID = -10
			});
		};
	}

	private void InitData()
	{
		if (WolfSoulField_Part.langhunlingyuRoleData == null || WolfSoulField_Part.langhunlingyuRoleData.SelfCityList == null)
		{
			return;
		}
		List<int> list = new List<int>();
		int i = 0;
		int count = WolfSoulField_Part.langhunlingyuRoleData.SelfCityList.Count;
		while (i < count)
		{
			if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].Owner != null && WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].Owner.BHID == Global.Data.roleData.Faction)
			{
				int cityLevel = WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].CityLevel;
				list.Add(cityLevel);
			}
			i++;
		}
		if (list.Count <= 0)
		{
			DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
			{
				if (args.ID == 0)
				{
					PlayZone.GlobalPlayZone.ColseWolfSoulFieldAwardWindow();
				}
			};
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("             暂无可领取的奖励！")
			});
			Super.ShowMessageBoxEx(Global.GetLang("提示"), colorStringForNGUIText, handler, new string[]
			{
				Global.GetLang("确定")
			}, false);
			return;
		}
		list.Sort();
		if (list.Count > 3)
		{
			this.Pnl.scale = new Vector3(0f, 1f, 0f);
		}
		else
		{
			this.Pnl.scale = new Vector3(0f, 0f, 0f);
		}
		for (int j = list.Count - 1; j >= 0; j--)
		{
			WolfSoulField_AwardList wolfSoulField_AwardList = U3DUtils.NEW<WolfSoulField_AwardList>();
			wolfSoulField_AwardList.Goods = Global.GetCityInfo()[list[j]].DayAward;
			wolfSoulField_AwardList.Level = Global.GetCityInfo()[list[j]].CityLevel;
			if (this.IsContainLevel(list[j]))
			{
				wolfSoulField_AwardList.GetOver.SetActive(false);
			}
			else
			{
				wolfSoulField_AwardList.GetOver.SetActive(true);
			}
			this.ItemCollection.AddNoUpdate(wolfSoulField_AwardList);
			UIPanel component = wolfSoulField_AwardList.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	private bool IsContainLevel(int level)
	{
		if (WolfSoulField_Part.langhunlingyuRoleData.GetDayAwardsState == null)
		{
			return false;
		}
		for (int i = 0; i < WolfSoulField_Part.langhunlingyuRoleData.GetDayAwardsState.Count; i++)
		{
			if (WolfSoulField_Part.langhunlingyuRoleData.GetDayAwardsState[i] == level)
			{
				return true;
			}
		}
		return false;
	}

	public void SetLingQuState(int error)
	{
		if (error == 0)
		{
			this.chenggong.transform.gameObject.SetActive(false);
			this.Lingqu.gameObject.SetActive(true);
			this.Lingqu.isEnabled = true;
			return;
		}
		if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList != null)
		{
			for (int i = 0; i < WolfSoulField_Part.langhunlingyuRoleData.SelfCityList.Count; i++)
			{
				if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].Owner != null)
				{
					this.chenggong.transform.gameObject.SetActive(true);
					this.Lingqu.gameObject.SetActive(false);
					this.ChangeLeftCountState();
					return;
				}
			}
		}
		else
		{
			this.chenggong.transform.gameObject.SetActive(false);
			this.Lingqu.gameObject.SetActive(true);
			this.Lingqu.isEnabled = false;
		}
	}

	private void ChangeLeftCountState()
	{
		for (int i = 0; i < this.ItemCollection.Count; i++)
		{
			WolfSoulField_AwardList wolfSoulField_AwardList = U3DUtils.AS<WolfSoulField_AwardList>(this.ItemCollection[i]);
			wolfSoulField_AwardList.GetOver.SetActive(true);
		}
	}

	private void SetLingQuState()
	{
		if (WolfSoulField_Part.langhunlingyuRoleData == null)
		{
			return;
		}
		if (WolfSoulField_Part.langhunlingyuRoleData.GetDayAwardsState != null)
		{
			this.chenggong.transform.gameObject.SetActive(false);
			this.Lingqu.gameObject.SetActive(true);
			this.Lingqu.isEnabled = true;
		}
		else if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList != null)
		{
			for (int i = 0; i < WolfSoulField_Part.langhunlingyuRoleData.SelfCityList.Count; i++)
			{
				if (WolfSoulField_Part.langhunlingyuRoleData.SelfCityList[i].Owner != null)
				{
					this.chenggong.transform.gameObject.SetActive(true);
					this.Lingqu.gameObject.SetActive(false);
					return;
				}
			}
		}
		else
		{
			this.chenggong.transform.gameObject.SetActive(false);
			this.Lingqu.gameObject.SetActive(true);
			this.Lingqu.isEnabled = false;
		}
	}

	public DPSelectedItemEventHandler DPSelcetItem;

	public ListBox RewardList;

	public UILabel Title;

	public GButton Close;

	public GButton Lingqu;

	public UILabel chenggong;

	public UIDraggablePanel Pnl;

	private ObservableCollection _ItemCollection;
}
