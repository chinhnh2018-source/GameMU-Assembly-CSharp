using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class TianTiZhanbaoPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_LabelDuanWeiJiFeng.transform.localPosition = new Vector3(-373f, -45f, -1f);
		this.m_LabelRongYaoDian.transform.localPosition = new Vector3(-373f, -76f, -1f);
		this.m_LabelShengLv.transform.localPosition = new Vector3(-373f, 18f, -1f);
		this.m_LabelLianSheng.transform.localPosition = new Vector3(-373f, -14f, -1f);
		this.m_LabelDuanWei.transform.localPosition = new Vector3(-373f, 48f, -1f);
		base.InitializeComponent();
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			TianTiArenaPart.Instance.m_RankingGroup.localPosition = new Vector3(0f, 10f, 0f);
			Object.Destroy(base.gameObject.GetComponentInParent<GChildWindow>().gameObject);
		};
		string text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.RoleID
		});
		TCPGameServerCmds.CMD_SPR_TIANTI_GET_LOG.SendDataUseRoleID();
		TianTiDataAndDayPaiHang tianTiDataAndDayPaiHangDataBag = TianTiArenaPart.Instance.TianTiDataAndDayPaiHangDataBag;
		if (tianTiDataAndDayPaiHangDataBag == null || tianTiDataAndDayPaiHangDataBag.TianTiData == null)
		{
			return;
		}
		XElement xelementByID = TianTiArenaPart.getXElementByID(tianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
		this.m_LabelDuanWei.text = TianTiArenaPart.getDuanWeiByID(tianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiId.ToString());
		if (tianTiDataAndDayPaiHangDataBag.TianTiData.FightCount != 0)
		{
			this.m_LabelShengLv.text = (float)tianTiDataAndDayPaiHangDataBag.TianTiData.SuccessCount / (float)tianTiDataAndDayPaiHangDataBag.TianTiData.FightCount * 100f + "%";
		}
		else
		{
			this.m_LabelShengLv.text = "0%";
		}
		this.m_LabelLianSheng.text = tianTiDataAndDayPaiHangDataBag.TianTiData.LianSheng + string.Empty;
		this.m_LabelDuanWeiJiFeng.text = tianTiDataAndDayPaiHangDataBag.TianTiData.DuanWeiJiFen.ToString();
		this.m_LabelRongYaoDian.text = Global.Data.roleData.TianTiRongYao + string.Empty;
		this.m_TexPersonImg.URL = "NetImages/Face/" + Global.Data.roleData.Occupation + "0_0.png";
	}

	public void aciton_CMD_SPR_TIANTI_GET_LOG(MUSocketConnectEventArgs e)
	{
		List<TianTiLogItemData> list = DataHelper.BytesToObject<List<TianTiLogItemData>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			return;
		}
		string text = "00FF00";
		string text2 = "FF0000";
		string text3 = "DEC69C";
		string text4 = "F9F702";
		for (int i = 0; i < list.Count; i++)
		{
			TianTiLogItemData tianTiLogItemData = list[i];
			string text5 = string.Empty;
			int success = tianTiLogItemData.Success;
			if (success != 0)
			{
				if (success == 1)
				{
					text5 = string.Format(Global.GetLang("你挑战{0},{1},{2}{3},获得段位积分{4}"), new object[]
					{
						Global.GetColorStringForNGUIText(new object[]
						{
							text,
							tianTiLogItemData.RoleName2
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							text4,
							Global.GetLang("你胜利了")
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							text,
							Global.GetLang("获得")
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							text3,
							tianTiLogItemData.RongYaoAward + Global.GetLang("荣耀点数")
						}),
						Global.GetColorStringForNGUIText(new object[]
						{
							text3,
							tianTiLogItemData.DuanWeiJiFenAward
						})
					});
				}
			}
			else
			{
				text5 = string.Format(Global.GetLang("你挑战{0},{1},{2}{3},扣除段位积分{4}"), new object[]
				{
					Global.GetColorStringForNGUIText(new object[]
					{
						text,
						tianTiLogItemData.RoleName2
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						text2,
						Global.GetLang("你失败了")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						text3,
						Global.GetLang("获得")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						text3,
						tianTiLogItemData.RongYaoAward + Global.GetLang("荣耀点数")
					}),
					Global.GetColorStringForNGUIText(new object[]
					{
						text2,
						Math.Abs(tianTiLogItemData.DuanWeiJiFenAward)
					})
				});
			}
			ZhanbaoItem zhanbaoItem = U3DUtils.NEW<ZhanbaoItem>();
			zhanbaoItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			zhanbaoItem.m_Text.Text = text5;
			this.m_listbox.Items.Add(zhanbaoItem);
		}
	}

	public GButton m_BtnClose;

	public UIScrollBar m_scrollBar;

	public UILabel m_LabelDuanWei;

	public UILabel m_LabelShengLv;

	public UILabel m_LabelLianSheng;

	public UILabel m_LabelDuanWeiJiFeng;

	public UILabel m_LabelRongYaoDian;

	public ShowNetImage m_TexPersonImg;

	public ListBox m_listbox;
}
