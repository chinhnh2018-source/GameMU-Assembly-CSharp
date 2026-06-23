using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class OlympicsRankPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.rankName.Text = Global.GetLang("我的排名");
		this.scoreName.Text = Global.GetLang("累计活动\n积分");
		this.tips.Text = Global.GetLang("排名时间：2016年12月38日至2016年12月38日");
		this.tipsLabel.Text = Global.GetLang("目前无人获得积分");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("AoYunTime", ',');
		string text = string.Format("{0}{1}{2}", systemParamStringArrayByName[0].Split(new char[]
		{
			' '
		})[0], Global.GetLang(" 至 "), systemParamStringArrayByName[1].Split(new char[]
		{
			' '
		})[0]);
		this.tips.Text = string.Format("{0}{1}", Global.GetLang("排名时间："), text);
		base.InitializeComponent();
		this.RefreshTotalScore();
		this.btnSumRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.tmpDataList == null || this.tmpDataList.Count <= 0)
			{
				Super.HintMainText(Global.GetLang("暂无排名"), 10, 3);
				return;
			}
			if (null == this.olympicsSumRankWind)
			{
				this.olympicsSumRankWind = U3DUtils.NEW<GChildWindow>();
				this.olympicsSumRankWind.ModalType = ChildWindowModalType.Translucent;
				Super.InitChildWindow(this.olympicsSumRankWind, "OlympicsSumRankWindow");
				Super.GData.GlobalPlayZone.Children.Add(this.olympicsSumRankWind);
			}
			this.olympicsSumRank = U3DUtils.NEW<OlympicsSumRank>();
			this.olympicsSumRankWind.Body.Add(this.olympicsSumRank);
			this.olympicsSumRankWind.IsShowModal = true;
			base.StartCoroutine(this.olympicsSumRank.InitData(this.tmpDataList));
			this.olympicsSumRank.Hander = delegate(object sender, DPSelectedItemEventArgs args)
			{
				if (args.ID == 0 && null != this.olympicsSumRank)
				{
					Object.Destroy(this.olympicsSumRank.gameObject);
					this.olympicsSumRank = null;
					Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicsSumRankWind);
				}
			};
		};
		this.btnAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (null == this.olympicAwardPartWind)
			{
				this.olympicAwardPartWind = U3DUtils.NEW<GChildWindow>();
				this.olympicAwardPartWind.ModalType = ChildWindowModalType.Translucent;
				Super.InitChildWindow(this.olympicAwardPartWind, "OlympicAwardPartWindow");
				Super.GData.GlobalPlayZone.Children.Add(this.olympicAwardPartWind);
			}
			this.olympicAwardPart = U3DUtils.NEW<OlympicAwardPart>();
			this.olympicAwardPartWind.Body.Add(this.olympicAwardPart);
			this.olympicAwardPartWind.IsShowModal = true;
			this.olympicAwardPart.InitData();
			this.olympicAwardPart.Hander = delegate(object sender, DPSelectedItemEventArgs args)
			{
				if (args.ID == 0 && null != this.olympicAwardPart)
				{
					Object.Destroy(this.olympicAwardPart.gameObject);
					this.olympicAwardPart = null;
					Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, this.olympicAwardPartWind);
				}
			};
		};
	}

	public void InitData()
	{
		this.awardDictData = OlympicsDataManage.GetAwardData();
	}

	public void RefreshData()
	{
		this.tmpDataList = OlympicsDataManage.GetRankData();
		if (this.tmpDataList == null || this.tmpDataList.Count <= 0)
		{
			this.rankNum.Text = Global.GetLang("未上榜");
			this.tipsLabel.transform.gameObject.SetActive(true);
			this.title1.transform.parent.gameObject.SetActive(false);
			this.title2.transform.parent.gameObject.SetActive(false);
			this.title3.transform.parent.gameObject.SetActive(false);
			return;
		}
		this.tipsLabel.transform.gameObject.SetActive(false);
		if (OlympicsDataManage.GetCurrentPlayerIndexOfRank() < 0)
		{
			this.rankNum.Text = Global.GetLang("未上榜");
		}
		else
		{
			this.rankNum.Text = OlympicsDataManage.GetCurrentPlayerIndexOfRank().ToString();
		}
		if (this.tmpDataList.Count == 1)
		{
			this.title1.transform.parent.gameObject.SetActive(true);
			this.title2.transform.parent.gameObject.SetActive(false);
			this.title3.transform.parent.gameObject.SetActive(false);
			this.ShowModle1();
		}
		else if (this.tmpDataList.Count == 2)
		{
			this.title1.transform.parent.gameObject.SetActive(true);
			this.title2.transform.parent.gameObject.SetActive(true);
			this.title3.transform.parent.gameObject.SetActive(false);
			this.ShowModle1();
			this.ShowModle2();
		}
		else
		{
			this.title1.transform.parent.gameObject.SetActive(true);
			this.title2.transform.parent.gameObject.SetActive(true);
			this.title3.transform.parent.gameObject.SetActive(true);
			this.ShowModle1();
			this.ShowModle2();
			this.ShowModle3();
		}
	}

	private void ShowModle1()
	{
		if (this.tmpDataList[0] != null)
		{
			this.ShowFirstRankPlayerDetails(this.tmpDataList[0]);
		}
	}

	private void ShowModle2()
	{
		if (this.tmpDataList[1] != null)
		{
			this.ShowSecondRankPlayerDetails(this.tmpDataList[1]);
		}
	}

	private void ShowModle3()
	{
		if (this.tmpDataList[2] != null)
		{
			this.ShowThirdRankPlayerDetails(this.tmpDataList[2]);
		}
	}

	private void ShowFirstRankPlayerDetails(KFRankData data)
	{
		this.title1.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.GetRanlPlayerTitle(data)
		});
		this.score1.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("总活动积分：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.Grade
		});
		base.StartCoroutine(this.Load3DModual(data, this.role3Model1, 0f));
	}

	private void ShowSecondRankPlayerDetails(KFRankData data)
	{
		this.title2.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.GetRanlPlayerTitle(data)
		});
		this.score2.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("总活动积分：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.Grade
		});
		base.StartCoroutine(this.Load3DModual(data, this.role3Model2, 0f));
	}

	private void ShowThirdRankPlayerDetails(KFRankData data)
	{
		this.title3.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			this.GetRanlPlayerTitle(data)
		});
		this.score3.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("总活动积分：")
		}) + Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			data.Grade
		});
		base.StartCoroutine(this.Load3DModual(data, this.role3Model3, 0f));
	}

	private string GetRanlPlayerTitle(KFRankData data)
	{
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(data.ZoneID, out ztBuffServerInfo))
		{
			return Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.RoleName, 0)
			});
		}
		return Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			string.Format("{0}{1}{2}{3}", new object[]
			{
				"S",
				data.ZoneID,
				".",
				data.RoleName
			})
		});
	}

	private IEnumerator Load3DModual(KFRankData data, Transform modelTransform, float time)
	{
		yield return new WaitForSeconds(time);
		Modal3DShow modal = modelTransform.GetComponent<Modal3DShow>();
		RoleData4Selector _RoleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(data.RoleData, 0, data.RoleData.Length);
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
		this.roleResLoader = UIHelper.LoadRoleRes(modal, _RoleData4Selector.SettingBitFlags, _RoleData4Selector.Occupation, _RoleData4Selector.SubOccupation, _RoleData4Selector.OtherName, _RoleData4Selector.GoodsDataList, null, _RoleData4Selector.MyWingData, 1f, 0, null, false);
		UIHelper.SetModalPosZ(modelTransform.transform);
		yield break;
	}

	public void RefreshTotalScore()
	{
		this.score.Text = OlympicsDataManage.totalScore.ToString();
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		base.Destroy();
		base.StopAllCoroutines();
	}

	public GButton btnSumRank;

	public GButton btnAward;

	public TextBlock rankName;

	public TextBlock rankNum;

	public TextBlock scoreName;

	public TextBlock score;

	public Transform role3Model1;

	public TextBlock title1;

	public TextBlock score1;

	public Transform role3Model2;

	public TextBlock title2;

	public TextBlock score2;

	public Transform role3Model3;

	public TextBlock title3;

	public TextBlock score3;

	public TextBlock tips;

	public TextBlock tipsLabel;

	private List<KFRankData> tmpDataList;

	private Dictionary<int, OlympicsAwardData> awardDictData;

	private OlympicsSumRank olympicsSumRank;

	private GChildWindow olympicsSumRankWind;

	private OlympicAwardPart olympicAwardPart;

	private GChildWindow olympicAwardPartWind;

	private RoleResLoader roleResLoader;
}
