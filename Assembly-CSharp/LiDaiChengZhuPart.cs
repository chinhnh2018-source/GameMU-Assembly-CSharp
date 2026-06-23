using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class LiDaiChengZhuPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs());
			}
		};
		this.left.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurPage == 2 && this.maxList > 3)
			{
				this.groupPrePage.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.CurPage = 1;
				this.right.gameObject.SetActive(true);
				this.left.gameObject.SetActive(false);
			}
			if (this.CurPage == 3 && this.maxList > 6)
			{
				this.groupPrePage.transform.localPosition = new Vector3(-930f, 0f, 0f);
				this.CurPage = 2;
				this.right.gameObject.SetActive(true);
				this.left.gameObject.SetActive(true);
			}
		};
		this.right.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CurPage == 1 && this.maxList > 3)
			{
				this.groupPrePage.transform.localPosition = new Vector3(-930f, 0f, 0f);
				this.CurPage = 2;
				this.left.gameObject.SetActive(true);
				this.right.gameObject.SetActive(false);
				if (this.maxList > 6)
				{
					this.right.gameObject.SetActive(true);
					this.left.gameObject.SetActive(true);
				}
				return;
			}
			if (this.CurPage == 2 && this.maxList > 6)
			{
				this.groupPrePage.transform.localPosition = new Vector3(-1860f, 0f, 0f);
				this.CurPage = 3;
				this.right.gameObject.SetActive(false);
				this.left.gameObject.SetActive(true);
			}
		};
		this.ShowLiDaiChengZhu();
	}

	public void ShowLiDaiChengZhu()
	{
		List<LangHunLingYuKingShowDataHist> langHunLingYuKingShowDataHist = Global.Data.langHunLingYuKingShowDataHist;
		if (langHunLingYuKingShowDataHist == null)
		{
			MUDebug.Log<string>(new string[]
			{
				"历代圣域城主数据为空"
			});
			return;
		}
		if (langHunLingYuKingShowDataHist.Count > 9)
		{
			Debug.Log(Global.GetLang("历代圣域城主数据最多为9条"));
			return;
		}
		this.maxList = langHunLingYuKingShowDataHist.Count;
		if (langHunLingYuKingShowDataHist.Count <= 3)
		{
			this.left.gameObject.SetActive(false);
			this.right.gameObject.SetActive(false);
		}
		if (langHunLingYuKingShowDataHist.Count > 3)
		{
			this.left.gameObject.SetActive(false);
			this.right.gameObject.SetActive(true);
		}
		for (int i = 0; i < langHunLingYuKingShowDataHist.Count; i++)
		{
			this.liDaiChengZhuItem = U3DUtils.NEW<LiDaiChengZhuItem>();
			this.childTrans = this.groupPrePage.transform.FindChild(string.Empty + i);
			if (this.childTrans != null)
			{
				this.modal = this.childTrans.GetComponent<Modal3DShow>();
				this.liDaiChengZhuItem.transform.parent = this.modal.gameObject.transform;
				this.liDaiChengZhuItem.transform.localScale = new Vector3(0.66f, 0.66f, 0f);
				this.liDaiChengZhuItem.transform.localPosition = new Vector3(0f, 125f, -600f);
			}
			this.liDaiChengZhuItem.RoleName = langHunLingYuKingShowDataHist[i].RoleData4Selector.RoleName;
			this.liDaiChengZhuItem.BangHuiName = langHunLingYuKingShowDataHist[i].BHName;
			this.liDaiChengZhuItem.BeiMoBaiCiShu = Global.GetLang("被膜拜次数    ：") + langHunLingYuKingShowDataHist[i].AdmireCount.ToString();
			this.liDaiChengZhuItem.GetChengZhuTime = string.Format(Global.GetLang("获得城主时间：{0}年{1}月{2}日"), langHunLingYuKingShowDataHist[i].CompleteTime.Year, langHunLingYuKingShowDataHist[i].CompleteTime.Month, langHunLingYuKingShowDataHist[i].CompleteTime.Day);
			this.liDaiChengZhuItem.transform.parent = this.childTrans;
			this.modal.transform.parent = this.childTrans;
			RoleData4Selector roleData4Selector = langHunLingYuKingShowDataHist[i].RoleData4Selector;
			this.LoadRoleRes(roleData4Selector);
		}
		if (langHunLingYuKingShowDataHist.Count == 1)
		{
			this.liDaiChengZhuItem.transform.localScale = new Vector3(0.65f, 0.65f, 0f);
			this.modal.transform.localPosition = new Vector3(0f, -200f, -600f);
		}
		this.CurPage = 1;
	}

	public override void Destroy()
	{
		int count = this.roleLoaderList.Count;
		for (int i = 0; i < count; i++)
		{
			RoleResLoader roleResLoader = this.roleLoaderList[i];
			roleResLoader.Stop();
		}
		this.roleLoaderList.Clear();
		base.Destroy();
	}

	private void LoadRoleRes(RoleData4Selector roleData4Selector)
	{
		if (null != this.oldRole)
		{
			Object.Destroy(this.oldRole);
			this.oldRole = null;
		}
		if (roleData4Selector == null || roleData4Selector.RoleID < 0)
		{
			return;
		}
		if (null != this.modal)
		{
			this.modal.Clear();
		}
		int fashionGoodsID = Global.GetFashionGoodsID(roleData4Selector.FashionWingsID);
		RoleResLoader roleResLoader = UIHelper.LoadRoleRes(this.modal, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.RoleName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, fashionGoodsID, null, false);
		if (roleResLoader != null)
		{
			this.roleLoaderList.Add(roleResLoader);
		}
	}

	public GButton btnClose;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton left;

	public GButton right;

	private int CurPage;

	private Transform childTrans;

	private Modal3DShow modal;

	private LiDaiChengZhuItem liDaiChengZhuItem;

	public GameObject groupPrePage;

	private int maxList;

	private GameObject oldRole;

	private List<RoleResLoader> roleLoaderList = new List<RoleResLoader>();
}
