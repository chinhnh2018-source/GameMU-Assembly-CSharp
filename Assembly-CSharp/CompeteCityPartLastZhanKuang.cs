using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class CompeteCityPartLastZhanKuang : UserControl
{
	public int SetTimeLab
	{
		set
		{
			this.Title.spriteName = "mingdan";
			if (value == 0)
			{
				this.BisaiJincheng.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("海选阶段")
				});
				this.BisaiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("周三：{0} - {1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
				});
				this.Title.spriteName = "zhankuang";
			}
			else if (value == 1)
			{
				this.BisaiJincheng.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("海选阶段")
				});
				this.BisaiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("周三：{0} - {1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
				});
			}
			else if (value == 2)
			{
				this.BisaiJincheng.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("16-8阶段")
				});
				this.BisaiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("周四：{0} - {1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
				});
			}
			else if (value == 3)
			{
				this.BisaiJincheng.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("8-4阶段")
				});
				this.BisaiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("周四：{0} - {1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
				});
			}
			else if (value == 4)
			{
				this.BisaiJincheng.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("4-2阶段")
				});
				this.BisaiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("周五：{0} - {1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
				});
			}
			else if (value == 5)
			{
				this.BisaiJincheng.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("冠军争夺")
				});
				this.BisaiTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("周五：{0} - {1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime)
				});
			}
		}
	}

	private void InitTextInPrefabs()
	{
		int i = 0;
		int num = this.players.Length;
		while (i < num)
		{
			this.players[i].Name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"826e53",
				Global.GetLang("虚位以待")
			});
			this.players[i].Group = this.weizhishunxu[i];
			i++;
		}
		this.kingName.text = string.Empty;
		this.Bisai1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("比赛进程：")
		});
		this.Bisai2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("比赛时间：")
		});
		this.Bisai1.pivot = 5;
		this.Bisai1.transform.localPosition = new Vector3(-25f, -142f, -0.5f);
		this.BisaiJincheng.pivot = 3;
		this.BisaiJincheng.transform.localPosition = new Vector3(-15f, -142f, -0.5f);
		this.barkGround.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang.png.qj";
		this.LooksKing.gameObject.GetComponent<ShowNetImage>().URL = "NetImages/GameRes/Images/Plate/zhongshen/xunzhang.png.qj";
		if (this.LooksKing != null)
		{
			UITexture component = this.LooksKing.gameObject.GetComponent<UITexture>();
			component.shader = Shader.Find("Unlit/Gray");
		}
		this.touxiangKuang.SetActive(false);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		GameInstance.Game.SendCompeteCityHaiXuanRankData();
		Super.ShowNetWaiting(null);
	}

	public void DisposeServerData(List<ZhengDuoRankData> RankList)
	{
		if (RankList.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < RankList.Count; i++)
		{
			for (int j = 0; j < this.players.Length; j++)
			{
				if (RankList[i].Rank1 + 1 == this.players[j].Group)
				{
					ZtBuffServerInfo ztBuffServerInfo = null;
					if (Global.GetNowServerIsZhuTiFu(RankList[i].ZoneId, out ztBuffServerInfo))
					{
						this.players[j].Name.text = Global.GetColorStringForNGUIText(new object[]
						{
							"826e53",
							Global.FormatRoleNameZoneid(RankList[i].ZoneId, RankList[i].BhName, 0, 1)
						});
					}
					else if (RankList[i].Lose == 0)
					{
						this.players[j].Name.text = Global.GetColorStringForNGUIText(new object[]
						{
							"c3b096",
							Global.FormatRoleNameZoneid(RankList[i].ZoneId, RankList[i].BhName, 0, 1)
						});
					}
					else
					{
						this.players[j].Name.text = Global.GetColorStringForNGUIText(new object[]
						{
							"826e53",
							Global.FormatRoleNameZoneid(RankList[i].ZoneId, RankList[i].BhName, 0, 1)
						});
					}
					this.players[j].Grade = RankList[i].Rank2;
					this.players[j].State = RankList[i].Lose;
					this.GetLuXian(RankList[i].Rank2, RankList[i].Lose, j + 1);
				}
				if (RankList[i].Rank2 == 1)
				{
					this.kingName.text = Global.FormatRoleNameZoneid(RankList[i].ZoneId, RankList[i].BhName, 0, 1);
					this.Bisai1.text = string.Empty;
					this.Bisai2.text = string.Empty;
					this.BisaiJincheng.text = string.Empty;
					this.BisaiTime.text = string.Empty;
					this.Title.spriteName = "zhankuang";
				}
			}
		}
	}

	private void GetLuXian(int grade, int state, int group)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (group == 1 || group == 2)
		{
			num = 1;
			num2 = 1;
			num3 = 1;
		}
		else if (group == 3 || group == 4)
		{
			num = 2;
			num2 = 1;
			num3 = 1;
		}
		else if (group == 5 || group == 6)
		{
			num = 3;
			num2 = 2;
			num3 = 1;
		}
		else if (group == 7 || group == 8)
		{
			num = 4;
			num2 = 2;
			num3 = 1;
		}
		else if (group == 9 || group == 10)
		{
			num = 5;
			num2 = 3;
			num3 = 2;
		}
		else if (group == 11 || group == 12)
		{
			num = 6;
			num2 = 3;
			num3 = 2;
		}
		else if (group == 13 || group == 14)
		{
			num = 7;
			num2 = 4;
			num3 = 2;
		}
		else if (group == 15 || group == 16)
		{
			num = 8;
			num2 = 4;
			num3 = 2;
		}
		if (grade == 16 && state == 0)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			return;
		}
		if (grade == 8)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			if (state == 0)
			{
				this.luXian8_4[num - 1].spriteName = "yelow2";
			}
			return;
		}
		if (grade == 4)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yelow2";
			if (state == 0)
			{
				this.luXian4_2[num2 - 1].spriteName = "yellow3";
			}
			return;
		}
		if (grade == 2)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yelow2";
			this.luXian4_2[num2 - 1].spriteName = "yellow3";
			if (state == 0)
			{
				this.luXian2_1[num3 - 1].spriteName = "yellow4";
			}
			return;
		}
		if (grade == 1)
		{
			this.luXian16_8[group - 1].spriteName = "yellow1";
			this.luXian8_4[num - 1].spriteName = "yelow2";
			this.luXian4_2[num2 - 1].spriteName = "yellow3";
			this.luXian2_1[num3 - 1].spriteName = "yellow4";
			if (this.LooksKing != null)
			{
				UITexture component = this.LooksKing.gameObject.GetComponent<UITexture>();
				component.shader = Shader.Find("Unlit/Transparent Colored");
			}
			return;
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ItemHandler;

	public ShowNetImage barkGround;

	public GButton Close;

	public Players[] players;

	public UISprite[] luXian16_8;

	public UISprite[] luXian8_4;

	public UISprite[] luXian4_2;

	public UISprite[] luXian2_1;

	private int[] weizhishunxu = new int[]
	{
		1,
		16,
		8,
		9,
		4,
		13,
		5,
		12,
		2,
		15,
		7,
		10,
		3,
		14,
		6,
		11
	};

	public UISprite Title;

	public UIEventListener LooksKing;

	public UILabel kingName;

	public ShowNetImage touxiangKing;

	public GameObject touxiangKuang;

	public UILabel BisaiJincheng;

	public UILabel BisaiTime;

	public UILabel Bisai1;

	public UILabel Bisai2;
}
