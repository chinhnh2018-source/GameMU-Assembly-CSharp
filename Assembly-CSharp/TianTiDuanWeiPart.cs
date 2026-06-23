using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TianTiDuanWeiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(0f, 10f, 0f);
			Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
		};
		base.StartCoroutine(this.aciton_CMD_SPR_TIANTI_PAIHANG());
	}

	private IEnumerator aciton_CMD_SPR_TIANTI_PAIHANG()
	{
		TianTiDataAndDayPaiHang TianTiDataAndDayPaiHangDataBag = TianTiArenaPart.Instance.TianTiDataAndDayPaiHangDataBag;
		if (TianTiDataAndDayPaiHangDataBag == null || TianTiDataAndDayPaiHangDataBag.PaiHangRoleDataList == null)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("暂无段位排行信息"), 0, -1, -1, 0);
			yield break;
		}
		this.m_LabelPaiMing.text = TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiRank.ToString();
		this.m_LabelDuanWei.text = TianTiArenaPart.getDuanWeiByID(TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
		this.m_LabelJiFeng.text = TianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiJiFen.ToString();
		for (int i = 0; i < TianTiDataAndDayPaiHangDataBag.PaiHangRoleDataList.Count; i++)
		{
			TianTiPaiHangRoleData _TianTiPaiHangRoleData = TianTiDataAndDayPaiHangDataBag.PaiHangRoleDataList[i];
			TianTiDuanWeiItemPart _DuanWeiTianTiItemPart = U3DUtils.NEW<TianTiDuanWeiItemPart>();
			_DuanWeiTianTiItemPart.transform.parent = this.m_TransformList.transform;
			_DuanWeiTianTiItemPart.transform.localScale = Vector3.one;
			_DuanWeiTianTiItemPart.transform.localPosition = Vector3.zero;
			_DuanWeiTianTiItemPart.GetComponent<UIDragObject>().target = this.m_TransformList;
			_DuanWeiTianTiItemPart._RoleData4Selector = _TianTiPaiHangRoleData.RoleData4Selector;
			_DuanWeiTianTiItemPart._TianTiPaiHangRoleData = _TianTiPaiHangRoleData;
			_DuanWeiTianTiItemPart.m_Background2.enabled = false;
			UIEventListener.Get(_DuanWeiTianTiItemPart.gameObject).onClick = delegate(GameObject s)
			{
				this.InnitDataSelect(s);
			};
			this.m_DuanWeiTianTiItemPartArr.Add(_DuanWeiTianTiItemPart);
			_DuanWeiTianTiItemPart.m_LabelItemMingCi.text = Global.GetLang("第") + (i + 1) + Global.GetLang("名");
			_DuanWeiTianTiItemPart.m_LabelItemName.text = Global.GetLang(_TianTiPaiHangRoleData.RoleName);
			_DuanWeiTianTiItemPart.m_ImgTuXiang.URL = "NetImages/Face/" + _TianTiPaiHangRoleData.Occupation + "0_0.png";
			_DuanWeiTianTiItemPart.m_LabelItemDuanWei.text = Global.GetLang("段位:") + TianTiArenaPart.getDuanWeiByID(_TianTiPaiHangRoleData.DuanWeiId.ToString());
			if (i + 1 <= 3)
			{
				_DuanWeiTianTiItemPart.m_LabelItemMingCi.text = string.Empty;
				_DuanWeiTianTiItemPart.m_SpriteMingCi.enabled = true;
				if (i + 1 == 1)
				{
					_DuanWeiTianTiItemPart.m_SpriteMingCi.spriteName = "DiYi";
				}
				else if (i + 1 == 2)
				{
					_DuanWeiTianTiItemPart.m_SpriteMingCi.spriteName = "DiEr";
				}
				else
				{
					_DuanWeiTianTiItemPart.m_SpriteMingCi.spriteName = "DiSan";
				}
			}
			yield return new WaitForSeconds(0.1f);
		}
		this.m_TransformList.GetComponent<UIGrid>().Reposition();
		this.InnitDataSelect(Enumerable.First<TianTiDuanWeiItemPart>(this.m_DuanWeiTianTiItemPartArr.FindAll((TianTiDuanWeiItemPart s) => s != null)).gameObject);
		yield break;
	}

	public void aciton_CMD_SPR_TIANTI_MONTH_PAIHANG(MUSocketConnectEventArgs e)
	{
		if (e == null || e.bytesData == null)
		{
			return;
		}
		TianTiMonthPaiHangData tianTiMonthPaiHangData = DataHelper.BytesToObject<TianTiMonthPaiHangData>(e.bytesData, 0, e.bytesData.Length);
		if (tianTiMonthPaiHangData.PaiHangRoleDataList == null)
		{
			return;
		}
		TianTiDataAndDayPaiHang tianTiDataAndDayPaiHangDataBag = TianTiArenaPart.Instance.TianTiDataAndDayPaiHangDataBag;
		if (tianTiDataAndDayPaiHangDataBag == null || tianTiDataAndDayPaiHangDataBag.TianTiData == null)
		{
			return;
		}
		this.m_LabelPaiMing.text = tianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiRank.ToString();
		this.m_LabelDuanWei.text = TianTiArenaPart.getDuanWeiByID(tianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
		this.m_LabelJiFeng.text = tianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiJiFen.ToString();
		for (int i = 0; i < tianTiMonthPaiHangData.PaiHangRoleDataList.Count; i++)
		{
			TianTiPaiHangRoleData tianTiPaiHangRoleData = tianTiMonthPaiHangData.PaiHangRoleDataList[i];
			TianTiDuanWeiItemPart tianTiDuanWeiItemPart = U3DUtils.NEW<TianTiDuanWeiItemPart>();
			tianTiDuanWeiItemPart.transform.parent = this.m_TransformList.transform;
			tianTiDuanWeiItemPart.transform.localScale = Vector3.one;
			tianTiDuanWeiItemPart.transform.localPosition = Vector3.zero;
			tianTiDuanWeiItemPart.GetComponent<UIDragObject>().target = this.m_TransformList;
			tianTiDuanWeiItemPart._RoleData4Selector = tianTiPaiHangRoleData.RoleData4Selector;
			tianTiDuanWeiItemPart._TianTiPaiHangRoleData = tianTiPaiHangRoleData;
			tianTiDuanWeiItemPart.m_Background2.enabled = false;
			UIEventListener.Get(tianTiDuanWeiItemPart.gameObject).onClick = delegate(GameObject s)
			{
				this.InnitDataSelect(s);
			};
			this.m_DuanWeiTianTiItemPartArr.Add(tianTiDuanWeiItemPart);
			tianTiDuanWeiItemPart.m_LabelItemMingCi.text = Global.GetLang("第") + (i + 1) + Global.GetLang("名");
			tianTiDuanWeiItemPart.m_LabelItemName.text = Global.GetLang(tianTiPaiHangRoleData.RoleName);
			tianTiDuanWeiItemPart.m_ImgTuXiang.URL = "NetImages/Face/" + tianTiPaiHangRoleData.Occupation + "0_0.png";
			tianTiDuanWeiItemPart.m_LabelItemDuanWei.text = Global.GetLang("段位:") + TianTiArenaPart.getDuanWeiByID(tianTiPaiHangRoleData.DuanWeiId.ToString());
			if (i + 1 <= 3)
			{
				tianTiDuanWeiItemPart.m_LabelItemMingCi.text = string.Empty;
				tianTiDuanWeiItemPart.m_SpriteMingCi.enabled = true;
				if (i + 1 == 1)
				{
					tianTiDuanWeiItemPart.m_SpriteMingCi.spriteName = "DiYi";
				}
				else if (i + 1 == 2)
				{
					tianTiDuanWeiItemPart.m_SpriteMingCi.spriteName = "DiEr";
				}
				else
				{
					tianTiDuanWeiItemPart.m_SpriteMingCi.spriteName = "DiSan";
				}
			}
		}
		this.m_TransformList.GetComponent<UIGrid>().Reposition();
		this.InnitDataSelect(Enumerable.First<TianTiDuanWeiItemPart>(this.m_DuanWeiTianTiItemPartArr.FindAll((TianTiDuanWeiItemPart s) => s != null)).gameObject);
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		base.Destroy();
	}

	private void InnitDataSelect(GameObject go)
	{
		TianTiDuanWeiItemPart component = go.GetComponent<TianTiDuanWeiItemPart>();
		if (component.m_Background2.enabled)
		{
			return;
		}
		foreach (TianTiDuanWeiItemPart tianTiDuanWeiItemPart in this.m_DuanWeiTianTiItemPartArr)
		{
			if (tianTiDuanWeiItemPart != null)
			{
				tianTiDuanWeiItemPart.m_Background2.enabled = false;
			}
		}
		component.m_Background2.enabled = true;
		RoleData4Selector roleData4Selector = component._RoleData4Selector;
		if (roleData4Selector != null)
		{
			if (this.roleResLoader != null)
			{
				this.roleResLoader.Stop();
			}
			this.roleResLoader = UIHelper.LoadRoleRes(this.m_ShowModal3D, roleData4Selector.SettingBitFlags, roleData4Selector.Occupation, roleData4Selector.SubOccupation, roleData4Selector.OtherName, roleData4Selector.GoodsDataList, null, roleData4Selector.MyWingData, 1f, 0, null, false);
		}
		UIHelper.SetModalPosZ(this.m_ShowModal3D.transform);
		TianTiPaiHangRoleData tianTiPaiHangRoleData = component._TianTiPaiHangRoleData;
		this.m_LabelXiangXiName.text = Global.GetLang(tianTiPaiHangRoleData.RoleName);
		this.m_LabelXiangXiPaiMing.text = Global.GetLang("第") + tianTiPaiHangRoleData.DuanWeiRank + Global.GetLang("名");
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(tianTiPaiHangRoleData.ZoneId, out ztBuffServerInfo))
		{
			this.m_LabelXiangXiFuWuQi.text = Global.GetLang("服务器:") + ztBuffServerInfo.strServerName;
		}
		else
		{
			this.m_LabelXiangXiFuWuQi.text = Global.GetLang("服务器:") + tianTiPaiHangRoleData.ZoneId;
		}
		this.m_LabelXiangXiZhanLi.text = Global.GetLang("战力:") + tianTiPaiHangRoleData.ZhanLi;
		this.m_LabelXiangDuanWei.text = Global.GetLang("段位:") + TianTiArenaPart.getDuanWeiByID(tianTiPaiHangRoleData.DuanWeiId.ToString());
		this.m_LabelXiangDuanWeiJiFeng.text = Global.GetLang("积分:") + tianTiPaiHangRoleData.DuanWeiJiFen;
		this.m_ImgDuanWei.URL = "NetImages/GameRes/Images/TianTi/" + tianTiPaiHangRoleData.DuanWeiId.ToString() + ".png";
		if (tianTiPaiHangRoleData.DuanWeiRank <= 3)
		{
			this.m_LabelXiangXiPaiMing.text = string.Empty;
			this.m_SpriteXiangMingCi.enabled = true;
			if (tianTiPaiHangRoleData.DuanWeiRank == 1)
			{
				this.m_SpriteXiangMingCi.spriteName = "DiYi";
			}
			else if (tianTiPaiHangRoleData.DuanWeiRank == 2)
			{
				this.m_SpriteXiangMingCi.spriteName = "DiEr";
			}
			else
			{
				this.m_SpriteXiangMingCi.spriteName = "DiSan";
			}
		}
		else
		{
			this.m_SpriteXiangMingCi.enabled = false;
		}
	}

	public UILabel m_LabelXiangXiName;

	public UILabel m_LabelXiangXiPaiMing;

	public UILabel m_LabelXiangXiFuWuQi;

	public UILabel m_LabelXiangXiZhanLi;

	public UILabel m_LabelXiangDuanWei;

	public UILabel m_LabelXiangDuanWeiJiFeng;

	public UISprite m_SpriteXiangMingCi;

	public UILabel m_LabelPaiMing;

	public UILabel m_LabelDuanWei;

	public UILabel m_LabelJiFeng;

	public Modal3DShow m_ShowModal3D;

	public GButton m_BtnClose;

	public UIScrollBar m_scrollBar;

	public List<TianTiDuanWeiItemPart> m_DuanWeiTianTiItemPartArr = new List<TianTiDuanWeiItemPart>();

	public Transform m_TransformList;

	public ShowNetImage m_ImgDuanWei;

	private RoleResLoader roleResLoader;
}
