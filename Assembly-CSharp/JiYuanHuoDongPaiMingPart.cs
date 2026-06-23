using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JiYuanHuoDongPaiMingPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_PaiHang_btnJiangLi.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Global.GetLang("名次奖励")
		});
		this.m_BangZhu_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("名次奖励")
		});
		this.m_BangZhu_LabBenTuan.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("本团奖励")
		});
		this.m_PaiHang_LabMingCi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff3319",
			Global.GetLang("名次")
		});
		this.m_PaiHang_LabJunTuanName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff3319",
			Global.GetLang("军团名称")
		});
		this.m_PaiHang_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ff3319",
			Global.GetLang("纪元进度")
		});
		this.AddPaiHangPanel();
		this.m_PaiHang_GameJiangLiPanel.SetActive(false);
		this.m_PaiHang_btnJiangLi.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_PaiHang_GameJiangLiPanel.SetActive(true);
		};
		this.m_PaiHang_close.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.m_PaiHang_GameJiangLiPanel.SetActive(false);
		};
	}

	public void AddPaiHangPanel()
	{
		this.m_ObservableCollection_ListBox_PaiHang = this.m_PaiHang_ListPaiHang.ItemsSource;
		this.m_ObservableCollection_ListBox_JiangLi = this.m_PaiHang_ListJiangLi.ItemsSource;
		this.m_ObservableCollection_ListBox_BenTuan_JiangLi = this.m_BangZhu_ListBenTuan.ItemsSource;
		if (this.m_ObservableCollection_ListBox_PaiHang != null)
		{
			this.m_ObservableCollection_ListBox_PaiHang.Clear();
		}
		if (this.m_ObservableCollection_ListBox_JiangLi != null)
		{
			this.m_ObservableCollection_ListBox_JiangLi.Clear();
		}
		if (this.m_JiYuanConfig.data.EraRankList != null)
		{
			this.m_BangZhu_LabBenTuan.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本团奖励")
			});
			for (int i = 0; i < this.m_JiYuanConfig.data.EraRankList.Count; i++)
			{
				if (this.m_JiYuanConfig.data.EraRankList[i].RankValue > 0 && this.m_JiYuanConfig.data.EraRankList[i].RankValue < 6)
				{
					JiYuanPaiHangItem jiYuanPaiHangItem = U3DUtils.NEW<JiYuanPaiHangItem>();
					jiYuanPaiHangItem.SetMingCi = this.m_JiYuanConfig.data.EraRankList[i].RankValue;
					jiYuanPaiHangItem.SetName = this.m_JiYuanConfig.data.EraRankList[i].JunTuanName;
					jiYuanPaiHangItem.SetJinDu = Global.GetColorStringForNGUIText(new object[]
					{
						"ffffff",
						string.Concat(new object[]
						{
							Global.GetLang("第"),
							this.m_JiYuanConfig.data.EraRankList[i].EraStage,
							Global.GetLang("纪元"),
							" ",
							this.m_JiYuanConfig.data.EraRankList[i].EraStageProcess
						})
					}) + "%";
					this.m_ObservableCollection_ListBox_PaiHang.AddNoUpdate(jiYuanPaiHangItem);
					if (jiYuanPaiHangItem.GetComponent<UIPanel>() != null)
					{
						Object.Destroy(jiYuanPaiHangItem.GetComponent<UIPanel>());
					}
				}
			}
		}
		else
		{
			this.m_BangZhu_LabBenTuan.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("暂无排行")
			});
		}
		if (this.m_JiYuanConfig.data.EraRankList != null)
		{
			for (int j = 0; j < this.m_JiYuanConfig.data.EraRankList.Count; j++)
			{
				if (this.m_JiYuanConfig.data.EraRankList[j].JunTuanID == Global.Data.roleData.JunTuanId)
				{
					this.JiangLiID = this.m_JiYuanConfig.DicEraRewardPaiHang[this.m_JiYuanConfig.data.EraRankList[j].RankValue].ID;
					this.SetBenTuanData(this.m_JiYuanConfig.DicEraRewardPaiHang[this.m_JiYuanConfig.data.EraRankList[j].RankValue]);
				}
				if (this.JiangLiID == -1 && j >= this.m_JiYuanConfig.data.EraRankList.Count - 1)
				{
					this.SetBenTuanData(this.m_JiYuanConfig.DicEraRewardPaiHang[-1]);
				}
			}
		}
		Dictionary<int, EraReward>.Enumerator enumerator = this.m_JiYuanConfig.DicEraRewardPaiHang.GetEnumerator();
		while (enumerator.MoveNext())
		{
			JiYuanPaiHangJiangLiItem jiYuanPaiHangJiangLiItem = U3DUtils.NEW<JiYuanPaiHangJiangLiItem>();
			JiYuanPaiHangJiangLiItem jiYuanPaiHangJiangLiItem2 = jiYuanPaiHangJiangLiItem;
			KeyValuePair<int, EraReward> keyValuePair = enumerator.Current;
			jiYuanPaiHangJiangLiItem2.SetMingCi = keyValuePair.Value.EraRanking;
			JiYuanPaiHangJiangLiItem jiYuanPaiHangJiangLiItem3 = jiYuanPaiHangJiangLiItem;
			KeyValuePair<int, EraReward> keyValuePair2 = enumerator.Current;
			jiYuanPaiHangJiangLiItem3.SetJiYuan = keyValuePair2.Value.Progress;
			string text = string.Empty;
			KeyValuePair<int, EraReward> keyValuePair3 = enumerator.Current;
			text = keyValuePair3.Value.Reward;
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				KeyValuePair<int, EraReward> keyValuePair4 = enumerator.Current;
				text = keyValuePair4.Value.LeaderReward;
			}
			else if (Global.Data.roleData.JunTuanZhiWu == 2)
			{
				KeyValuePair<int, EraReward> keyValuePair5 = enumerator.Current;
				text = keyValuePair5.Value.MasterReward;
			}
			else
			{
				KeyValuePair<int, EraReward> keyValuePair6 = enumerator.Current;
				text = keyValuePair6.Value.Reward;
			}
			if (!string.IsNullOrEmpty(text))
			{
				jiYuanPaiHangJiangLiItem.m_ObservableCollection_ListBox_JiangLi = jiYuanPaiHangJiangLiItem.m_List.ItemsSource;
				Super.LoadGoodsList(text, jiYuanPaiHangJiangLiItem.m_ObservableCollection_ListBox_JiangLi);
				UIPanel[] componentsInChildren = jiYuanPaiHangJiangLiItem.GetComponentsInChildren<UIPanel>();
				if (componentsInChildren != null)
				{
					for (int k = 0; k < componentsInChildren.Length; k++)
					{
						Object.Destroy(componentsInChildren[k]);
					}
				}
			}
			this.m_ObservableCollection_ListBox_JiangLi.AddNoUpdate(jiYuanPaiHangJiangLiItem);
			UILabel labContent = this.m_LabContent;
			object[] array = new object[2];
			array[0] = "e3b36c";
			int num = 1;
			object lang = Global.GetLang("活动结束后，贡献度大于");
			KeyValuePair<int, EraReward> keyValuePair7 = enumerator.Current;
			array[num] = lang + keyValuePair7.Value.Contribution + Global.GetLang("会收到邮件发放的排行奖励");
			labContent.text = Global.GetColorStringForNGUIText(array);
		}
	}

	public void SetBenTuanData(EraReward eraRankData)
	{
		if (eraRankData == null)
		{
			return;
		}
		string text = DateTime.Parse(Global.GetLang(eraRankData.StartTime)).ToString(Global.GetLang("yyyy年MM月dd日"));
		string text2 = DateTime.Parse(Global.GetLang(eraRankData.EndTime)).ToString(Global.GetLang("yyyy年MM月dd日"));
		this.m_PaiHang_LabTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("排行奖励领取时间：") + text + "-" + text2
		});
		if (eraRankData.EraRanking < 0 || eraRankData.EraRanking > 5)
		{
			this.m_PaiHang_LabBenTuanPaiHang.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本团排名:6名后")
			});
			this.m_BangZhu_LabPaiMing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本团排名:6名后")
			});
		}
		else
		{
			this.m_PaiHang_LabBenTuanPaiHang.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本团排名:") + eraRankData.EraRanking
			});
			this.m_BangZhu_LabPaiMing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("本团排名:") + eraRankData.EraRanking
			});
		}
		this.m_PaiHang_LabBenTuanJinDu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Concat(new object[]
			{
				Global.GetLang("本团进度：第"),
				this.m_JiYuanConfig.data.EraStage,
				Global.GetLang("纪元 "),
				this.m_JiYuanConfig.data.EraStateProcess,
				"%"
			})
		});
		string text3 = string.Empty;
		if (eraRankData.EraRanking > 0 && eraRankData.EraRanking <= 5)
		{
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				text3 = eraRankData.LeaderReward;
			}
			else if (Global.Data.roleData.JunTuanZhiWu == 2)
			{
				text3 = eraRankData.MasterReward;
			}
			else
			{
				text3 = eraRankData.Reward;
			}
		}
		else if (Global.Data.roleData.JunTuanZhiWu == 1)
		{
			text3 = this.m_JiYuanConfig.DicEraRewardPaiHang[-1].LeaderReward;
		}
		else if (Global.Data.roleData.JunTuanZhiWu == 2)
		{
			text3 = this.m_JiYuanConfig.DicEraRewardPaiHang[-1].MasterReward;
		}
		else
		{
			text3 = this.m_JiYuanConfig.DicEraRewardPaiHang[-1].Reward;
		}
		if (!string.IsNullOrEmpty(text3))
		{
			Super.LoadGoodsList(text3, this.m_ObservableCollection_ListBox_BenTuan_JiangLi);
		}
	}

	public GButton m_PaiHang_close;

	public UILabel m_PaiHang_LabMingCi;

	public UILabel m_PaiHang_LabJunTuanName;

	public UILabel m_PaiHang_LabJinDu;

	public UILabel m_PaiHang_LabTime;

	public UILabel m_PaiHang_LabBenTuanPaiHang;

	public UILabel m_PaiHang_LabBenTuanJinDu;

	public GButton m_PaiHang_btnJiangLi;

	public ListBox m_PaiHang_ListPaiHang;

	public ListBox m_PaiHang_ListJiangLi;

	public GameObject m_PaiHang_GameJiangLiPanel;

	public UILabel m_LabContent;

	public UILabel m_BangZhu_LabTitle;

	public UILabel m_BangZhu_LabBenTuan;

	public UILabel m_BangZhu_LabPaiMing;

	public ListBox m_BangZhu_ListBenTuan;

	public UISprite m_SpYiLingQu;

	private ObservableCollection m_ObservableCollection_ListBox_PaiHang;

	private ObservableCollection m_ObservableCollection_ListBox_JiangLi;

	private ObservableCollection m_ObservableCollection_ListBox_BenTuan_JiangLi;

	public JiYuanConfig m_JiYuanConfig;

	public string m_PathParent = "NetImages/GameRes/Images/JiYuanHuoDong/";

	public DPSelectedItemEventHandler DPSelectedItem;

	public int JiangLiID = -1;
}
