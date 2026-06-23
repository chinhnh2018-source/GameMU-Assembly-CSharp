using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class JiYuanHuoDongPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.staticText.text = Global.GetLang("贡献奖励");
		this.m_LabZuiKuai.text = " ";
		this.m_JiYuanHuoDongPartZongLan.m_LabTime.text = " ";
		this.m_JiYuanHuoDongPartZongLan.m_LabContent.text = " ";
		base.InitializeComponent();
		if (this.m_BtnClose != null)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				PlayZone.GlobalPlayZone.CloseJiYuanHuoDongWindow();
			};
		}
		this.InitOnClick();
		this.m_PathParent = "NetImages/GameRes/Images/JiYuanHuoDong/";
		this.m_VectSelfJinDuTiao = this.m_ShowSelfJinDuTiao.transform.localPosition;
		this.m_VectZuiKuaiJinDuTiao = this.m_ShowZuiKuaiJinDuTiao.transform.localPosition;
		this.SenData();
	}

	private void InitOnClick()
	{
		if (this.m_BtnOpenBangZhuPanel != null)
		{
			this.m_BtnOpenBangZhuPanel.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.m_PanelBangZhu.gameObject.SetActive(true);
			};
		}
		if (this.m_BtnCloseBangZhuPanel != null)
		{
			this.m_BtnCloseBangZhuPanel.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.m_PanelBangZhu.gameObject.SetActive(false);
			};
		}
		if (this.m_JiYuanHuoDongPartZongLan != null)
		{
			this.m_JiYuanHuoDongPartZongLan.DPSelectedGanTan = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.m_SpZongLanGanTan.gameObject.SetActive(e.ShowFlagUpdate);
			};
		}
	}

	private void AddBangZhuPanel()
	{
		this.m_PanelBangZhu.gameObject.SetActive(false);
		this.m_LabBangZhuTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang(this.m_JiYuanConfig.StrTitle)
		});
		this.m_LabBangZhuContent.text = Global.GetLang(this.m_JiYuanConfig.StrShuoMing);
	}

	private void InitXml()
	{
		this.m_JiYuanHuoDongPartZongLan.m_JiYuanConfig = this.m_JiYuanConfig;
		this.m_JiYuanHuoDongPartZongLan.InitVlaue();
		this.m_JiYuanHuoDongPartZongLan.DPSelectedItem = new DPSelectedItemEventHandler(this.OnClick);
		this.m_JiYuanHuoDongPaiMingPart.m_JiYuanConfig = this.m_JiYuanConfig;
		this.m_JiYuanHuoDongPaiMingPart.DPSelectedItem = new DPSelectedItemEventHandler(this.OnClick);
		this.m_JiYuanHuoDongRenWuPart.m_JiYuanConfig = this.m_JiYuanConfig;
		this.m_JiYuanJuanXianPart.m_JiYuanConfig = this.m_JiYuanConfig;
		this.m_JiYuanJuanXianPart.DPSelectedItem = new DPSelectedItemEventHandler(this.OnJuanXian);
		this.AddBangZhuPanel();
	}

	private void InitValue()
	{
		if (this.m_Gtab == null || this.m_Gtab.TabBtns == null)
		{
			return;
		}
		List<GButton> listBtn = this.m_Gtab.TabBtns;
		this.m_ListStrBtnText.Add(Global.GetLang("纪元总览"));
		this.m_ListStrBtnText.Add(Global.GetLang("纪元任务"));
		this.m_ListStrBtnText.Add(Global.GetLang("纪元捐献"));
		this.m_ListStrBtnText.Add(Global.GetLang("纪元排行"));
		listBtn[0].Text = Global.GetColorStringForNGUIText(new object[]
		{
			"f0f0f0",
			Global.GetLang(this.m_ListStrBtnText[0])
		});
		listBtn[1].Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(this.m_ListStrBtnText[1])
		});
		listBtn[2].Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(this.m_ListStrBtnText[2])
		});
		listBtn[3].Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(this.m_ListStrBtnText[3])
		});
		this.m_Gtab.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
		{
			for (int i = 0; i < listBtn.Count; i++)
			{
				listBtn[i].Label.lineWidth = 130;
				if (i == s.ID)
				{
					listBtn[i].Text = Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						Global.GetLang(this.m_ListStrBtnText[i])
					});
				}
				else
				{
					listBtn[i].Text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						Global.GetLang(this.m_ListStrBtnText[i])
					});
				}
			}
			return true;
		};
	}

	private void RefreshJinDu()
	{
		this.m_ShowSelfJinDuTiao.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.OwnProgressBarColor;
		this.m_ShowZuiKuaiJinDuTiao.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.FastestProgressBarColor;
		if (this.m_JiYuanConfig.data.FastEraStage > 1)
		{
			this.m_ShowImge1.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar1Icon2;
		}
		else
		{
			this.m_ShowImge1.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar1Icon3;
		}
		if (this.m_JiYuanConfig.data.FastEraStage > 2)
		{
			this.m_ShowImge2.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar2Icon2;
		}
		else
		{
			this.m_ShowImge2.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar2Icon3;
		}
		if (this.m_JiYuanConfig.data.FastEraStage > 3)
		{
			this.m_ShowImge3.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar3Icon2;
		}
		else
		{
			this.m_ShowImge3.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar3Icon3;
		}
		if (this.m_JiYuanConfig.data.EraStage > 1)
		{
			this.m_ShowImge1.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar1Icon1;
		}
		if (this.m_JiYuanConfig.data.EraStage > 2)
		{
			this.m_ShowImge2.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar2Icon1;
		}
		if (this.m_JiYuanConfig.data.EraStage > 3)
		{
			this.m_ShowImge3.URL = this.m_PathParent + this.m_JiYuanConfig.EraUI.ProgressBar3Icon1;
		}
		float num = (float)((int)((this.m_JiYuanConfig.data.FastEraStage - 1) * 100) + this.m_JiYuanConfig.data.FastEraStateProcess) / 400f;
		this.m_ShowZuiKuaiJinDuTiao.transform.localPosition = new Vector3(this.m_VectZuiKuaiJinDuTiao.x + (1f - num) * this.m_ShowZuiKuaiJinDuTiao.transform.localScale.x, this.m_VectZuiKuaiJinDuTiao.y, this.m_VectZuiKuaiJinDuTiao.z);
		this.m_PanZuiKuai.transform.localPosition = new Vector3(-(1f - num) * this.m_ShowZuiKuaiJinDuTiao.transform.localScale.x, 0f, 0f);
		float num2 = (float)((int)((this.m_JiYuanConfig.data.EraStage - 1) * 100) + this.m_JiYuanConfig.data.EraStateProcess) / 400f;
		this.m_ShowSelfJinDuTiao.transform.localPosition = new Vector3(this.m_VectSelfJinDuTiao.x + (1f - num2) * this.m_ShowSelfJinDuTiao.transform.localScale.x, this.m_VectSelfJinDuTiao.y, this.m_VectSelfJinDuTiao.z);
		this.m_PanSelf.transform.localPosition = new Vector3(-(1f - num2) * this.m_ShowSelfJinDuTiao.transform.localScale.x, 0f, 0f);
		this.m_LabZuiKuai.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("最快军团")
		});
		this.m_LabZuiKuai.lineWidth = 70;
	}

	private void RefreshGanTan()
	{
		this.m_SpJuanXianGanTan.gameObject.SetActive(false);
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, EraTask> keyValuePair in this.m_JiYuanConfig.DicEraRewardJuanXian)
		{
			if (keyValuePair.Value.EraStage <= (int)this.m_JiYuanConfig.data.EraStage)
			{
				int num = 0;
				for (;;)
				{
					int num2 = num;
					Dictionary<int, EraTask>.Enumerator enumerator;
					KeyValuePair<int, EraTask> keyValuePair2 = enumerator.Current;
					if (num2 >= keyValuePair2.Value.CompletionCondition.Split(new char[]
					{
						'|'
					}).Length)
					{
						break;
					}
					List<int> list2 = list;
					KeyValuePair<int, EraTask> keyValuePair3 = enumerator.Current;
					list2.Add(keyValuePair3.Value.CompletionCondition.Split(new char[]
					{
						'|'
					})[num].Split(new char[]
					{
						','
					})[0].SafeToInt32(0));
					num++;
				}
			}
		}
		Dictionary<int, EraReward>.Enumerator enumerator2 = this.m_JiYuanConfig.DicEraRewardGongXian.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			int num3 = -1;
			if (this.m_JiYuanConfig.data.EraAwardStateDict != null)
			{
				Dictionary<int, int> eraAwardStateDict = this.m_JiYuanConfig.data.EraAwardStateDict;
				KeyValuePair<int, EraReward> keyValuePair4 = enumerator2.Current;
				if (eraAwardStateDict.ContainsKey(keyValuePair4.Value.ID))
				{
					Dictionary<int, int> eraAwardStateDict2 = this.m_JiYuanConfig.data.EraAwardStateDict;
					KeyValuePair<int, EraReward> keyValuePair5 = enumerator2.Current;
					num3 = eraAwardStateDict2[keyValuePair5.Value.ID];
				}
			}
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.EraDonate);
			KeyValuePair<int, EraReward> keyValuePair6 = enumerator2.Current;
			if (roleCommonUseParamsValue >= keyValuePair6.Value.Contribution && (num3 == -1 || num3 == 0))
			{
				this.m_SpJuanXianGanTan.gameObject.SetActive(true);
				break;
			}
		}
		if (Global.Data.roleData.GoodsDataList == null)
		{
			return;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			for (int j = 0; j < list.Count; j++)
			{
				if (Global.Data.roleData.GoodsDataList[i].GoodsID == list[j])
				{
					if (this.m_JiYuanConfig.PaiHangBangTime > Global.GetCorrectLocalTime())
					{
						this.m_SpJuanXianGanTan.gameObject.SetActive(true);
					}
					break;
				}
			}
		}
	}

	private void OnClick(object e, DPSelectedItemEventArgs s)
	{
		int id = s.ID;
		if (this.m_JiYuanConfig.DicEraReward.ContainsKey(s.ID))
		{
			int num = -1;
			if (this.m_JiYuanConfig.data.EraAwardStateDict != null && this.m_JiYuanConfig.data.EraAwardStateDict.ContainsKey(id))
			{
				num = this.m_JiYuanConfig.data.EraAwardStateDict[id];
			}
			if (num == 2)
			{
				Super.HintMainText(Global.GetLang("该纪元阶段未做贡献，无法领取奖励"), 10, 3);
			}
			else
			{
				GameInstance.Game.SenEraLingQu(id);
			}
		}
	}

	private void OnJuanXian(object e, DPSelectedItemEventArgs s)
	{
		int id = s.ID;
		this.SenEraJuanXian(id);
	}

	public void SenData()
	{
		GameInstance.Game.SenEraData();
	}

	public void RefreshData(EraData data)
	{
		if (this.m_JiYuanConfig == null)
		{
			this.m_JiYuanConfig = new JiYuanConfig();
			this.m_JiYuanConfig.InitValue();
			this.InitValue();
			this.m_JiYuanConfig.data = data;
			this.InitXml();
			this.RefreshJinDu();
			this.m_Gtab.SetActivePage(this.m_Page);
		}
		else
		{
			this.m_JiYuanConfig.data = data;
			if (this.m_JiYuanHuoDongRenWuPart != null)
			{
				this.m_JiYuanHuoDongRenWuPart.AddList();
			}
			if (this.m_JiYuanHuoDongPartZongLan != null)
			{
				this.m_JiYuanHuoDongPartZongLan.AddJiYuanZongLanPanel();
			}
			if (this.m_JiYuanJuanXianPart != null)
			{
				this.m_JiYuanJuanXianPart.RefreshJinDu();
				this.m_JiYuanJuanXianPart.AddGongXian();
			}
			this.RefreshJinDu();
			if (this.m_JiYuanJuanXianPart != null)
			{
				this.m_JiYuanHuoDongPaiMingPart.AddPaiHangPanel();
			}
		}
		this.RefreshGanTan();
	}

	public void SenEraJuanXian(int stage)
	{
		GameInstance.Game.SenEraJuanXian(stage);
	}

	public void RereshJuanXian(string[] str)
	{
		if (str[0].SafeToInt32(0) != 0)
		{
			if (str[0].SafeToInt32(0) == -2001)
			{
				Super.HintMainText(Global.GetLang("当前不在活动时间内"), 10, 3);
			}
			else if (str[0].SafeToInt32(0) == -2006)
			{
				Super.HintMainText(Global.GetLang("贡献不足，无法领取奖励"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(str[0].SafeToInt32(0), false, false)), 10, 3);
			}
		}
		else
		{
			this.m_JiYuanJuanXianPart.RefreshItem(str[1].SafeToInt32(0), str[2].SafeToInt32(0));
		}
	}

	public void RereshLingQu(string[] ret)
	{
		if (ret[0].SafeToInt32(0) != 0)
		{
			if (ret[0].SafeToInt32(0) == -2001)
			{
				Super.HintMainText(Global.GetLang("当前不在领取时间内"), 10, 3);
			}
			else
			{
				Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret[0].SafeToInt32(0), false, false)), 10, 3);
			}
		}
		else if (this.m_JiYuanConfig.DicEraReward.ContainsKey(ret[1].SafeToInt32(0)))
		{
			if (this.m_JiYuanConfig.DicEraReward[ret[1].SafeToInt32(0)].Type == 1)
			{
				this.m_JiYuanHuoDongPartZongLan.RefreshItem(this.m_JiYuanConfig.DicEraReward[ret[1].SafeToInt32(0)].Progress, 1);
			}
			else if (this.m_JiYuanConfig.DicEraReward[ret[1].SafeToInt32(0)].Type == 3)
			{
				this.m_JiYuanJuanXianPart.RefreshGongXian(ret[1].SafeToInt32(0));
				this.m_SpJuanXianGanTan.gameObject.SetActive(this.m_JiYuanJuanXianPart.m_SpJiangLiGanTan.gameObject.activeSelf);
			}
		}
	}

	public void SetChuanSong(int goodsId)
	{
		foreach (KeyValuePair<int, EraContribution> keyValuePair in this.m_JiYuanConfig.DicEraContribution)
		{
			if (keyValuePair.Value.EraID == this.m_JiYuanConfig.JiYuanKey)
			{
				Dictionary<int, EraContribution>.Enumerator enumerator;
				KeyValuePair<int, EraContribution> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.GoodsID == goodsId)
				{
					int num = -1;
					KeyValuePair<int, EraContribution> keyValuePair3 = enumerator.Current;
					if (keyValuePair3.Value.MonsterID.Split(new char[]
					{
						','
					}).Length > 0)
					{
						KeyValuePair<int, EraContribution> keyValuePair4 = enumerator.Current;
						string[] array = keyValuePair4.Value.MonsterID.Split(new char[]
						{
							','
						});
						int num2 = 0;
						KeyValuePair<int, EraContribution> keyValuePair5 = enumerator.Current;
						num = array[Random.Range(num2, keyValuePair5.Value.MonsterID.Split(new char[]
						{
							','
						}).Length)].SafeToInt32(0);
					}
					int num3 = -1;
					if (ConfigMonsters.MonsterXmlNode.ContainsKey(num))
					{
						num3 = ConfigMonsters.MonsterXmlNode[num].MapCode;
					}
					Point monsterPointByID = Global.GetMonsterPointByID(num3, num);
					if (Global.IsGoToKuaFuMap(num3))
					{
						PlayZone.GlobalPlayZone.OpenKuafuMapView(2, -1, num, num3, monsterPointByID.X, monsterPointByID.Y, false, 0, 0, false, false);
						return;
					}
					if (num3 != -1)
					{
						Global.Data.GameScene.AutoFindRoad(num3, monsterPointByID, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
						PlayZone.GlobalPlayZone.CloseJiYuanHuoDongWindow();
					}
				}
			}
		}
	}

	public UILabel staticText;

	public GTab m_Gtab;

	public GButton m_BtnClose;

	public UISprite m_SpZongLanGanTan;

	public UISprite m_SpJuanXianGanTan;

	public UIPanel m_PanelBangZhu;

	public GButton m_BtnOpenBangZhuPanel;

	public GButton m_BtnCloseBangZhuPanel;

	public UILabel m_LabBangZhuContent;

	public UILabel m_LabBangZhuTitle;

	public JiYuanHuoDongZongLanPart m_JiYuanHuoDongPartZongLan;

	public JiYuanHuoDongRenWuPart m_JiYuanHuoDongRenWuPart;

	public JiYuanJuanXianPart m_JiYuanJuanXianPart;

	public JiYuanHuoDongPaiMingPart m_JiYuanHuoDongPaiMingPart;

	public UILabel m_LabZuiKuai;

	public ShowNetImage m_ShowImge1;

	public ShowNetImage m_ShowImge2;

	public ShowNetImage m_ShowImge3;

	public ShowNetImage m_ShowSelfJinDuTiao;

	public ShowNetImage m_ShowZuiKuaiJinDuTiao;

	public UIPanel m_PanSelf;

	public UIPanel m_PanZuiKuai;

	public JiYuanConfig m_JiYuanConfig;

	public string m_PathParent = string.Empty;

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<string> m_ListStrBtnText = new List<string>();

	private Vector3 m_VectSelfJinDuTiao = new Vector3(0f, 0f, 0f);

	private Vector3 m_VectZuiKuaiJinDuTiao = new Vector3(0f, 0f, 0f);

	public int m_Page;
}
