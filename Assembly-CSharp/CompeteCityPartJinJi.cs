using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class CompeteCityPartJinJi : UserControl
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

	public int SetMyPaiMing
	{
		set
		{
			if (value == 1)
			{
				this.MyPaiMing.text = Global.GetLang("未通关");
			}
			else
			{
				this.MyPaiMing.text = Global.GetLang("未上榜");
			}
		}
	}

	private void InitTextInPerfabs()
	{
		this.PaiMing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("排名")
		});
		this.Name.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战盟名称")
		});
		this.DengJi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战盟等级")
		});
		this.Zhanli.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战盟战力")
		});
		this.YongShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("战斗用时")
		});
		this.MyPaiMing.text = Global.GetLang("未上榜");
		this.MyYongShi.text = string.Empty;
		this.back.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang.png.qj";
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPerfabs();
		this.ItemCollection = this.list.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.SendCompeteCityHaiXuanRankData();
		Super.ShowNetWaiting(null);
	}

	public void InitPlayerStateListItem(CompeteCityRankData data)
	{
		this.MyYongShi.text = Global.GetTimeStrBySecEx((double)(data.UsedMillisecond / 1000), true, -1);
		int i = 0;
		int count = data.RankList.Count;
		while (i < count)
		{
			CompeteCityPartJinJiItem competeCityPartJinJiItem = U3DUtils.NEW<CompeteCityPartJinJiItem>();
			competeCityPartJinJiItem.Rank = data.RankList[i].Rank1 + 1;
			competeCityPartJinJiItem.SetName = Global.FormatRoleNameZoneid(data.RankList[i].ZoneId, data.RankList[i].BhName, 0, 1);
			competeCityPartJinJiItem.DengJi.text = data.RankList[i].BhLevel.ToString();
			competeCityPartJinJiItem.Zhanli.text = data.RankList[i].ZhanLi.ToString();
			competeCityPartJinJiItem.YongShi.text = Global.GetTimeStrBySecEx((double)(data.RankList[i].UsedMillisecond / 1000), true, -1);
			this.ItemCollection.AddNoUpdate(competeCityPartJinJiItem);
			UIPanel component = competeCityPartJinJiItem.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			if (data.RankList[i].Bhid == Global.Data.roleData.Faction)
			{
				this.MyPaiMing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					(data.RankList[i].Rank1 + 1).ToString()
				});
				this.MyYongShi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("用时："),
					"dac7ae",
					Global.GetTimeStrBySecEx((double)(data.RankList[i].UsedMillisecond / 1000), true, -1)
				});
			}
			i++;
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public ShowNetImage back;

	public UILabel PaiMing;

	public new UILabel Name;

	public UILabel DengJi;

	public UILabel Zhanli;

	public UILabel YongShi;

	public UILabel MyPaiMing;

	public UILabel MyYongShi;

	public GButton BtnClose;

	public ListBox list;

	private ObservableCollection _ItemCollection;
}
