using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZhanbaoTianTiPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
		UIEventListener.Get(this.m_BtnClose.gameObject).onClick = delegate(GameObject s)
		{
			Object.Destroy(base.gameObject);
		};
		TCPGameServerCmds.CMD_SPR_TIANTI_GET_LOG.SendDataUseRoleID();
	}

	private void aciton_CMD_SPR_TIANTI_GET_LOG(MUSocketConnectEventArgs e)
	{
		List<TianTiLogItemData> list = DataHelper.BytesToObject<List<TianTiLogItemData>>(e.bytesData, 0, e.bytesData.Length);
	}

	public void refresh(List<JingJiChallengeInfoData> list)
	{
		if (list == null)
		{
			return;
		}
		string text = "00FF00";
		string text2 = "FF0000";
		string text3 = "F9F702";
		for (int i = 0; i < list.Count; i++)
		{
			JingJiChallengeInfoData jingJiChallengeInfoData = list[i];
			string text4 = string.Empty;
			switch (jingJiChallengeInfoData.zhanbaoType)
			{
			case 0:
				text4 = string.Format(Global.GetLang("你挑战{0},{1},排名上升至{2}"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					Global.GetLang("你胜利了")
				}), (jingJiChallengeInfoData.value >= 0) ? jingJiChallengeInfoData.value.ToString() : Global.GetLang("500名后"));
				break;
			case 1:
				text4 = string.Format(Global.GetLang("你挑战{0},{1},排名不变。"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					Global.GetLang("你失败了")
				}));
				break;
			case 2:
				text4 = string.Format(Global.GetLang("{0}挑战你,{1},排名不变。"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					Global.GetLang("你胜利了")
				}));
				break;
			case 3:
				text4 = string.Format(Global.GetLang("{0}挑战你,{1},排名下降至{2}"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text2,
					Global.GetLang("你失败了")
				}), (jingJiChallengeInfoData.value >= 0) ? jingJiChallengeInfoData.value.ToString() : Global.GetLang("500名后"));
				break;
			case 4:
				text4 = string.Format(Global.GetLang("{0}连胜次数达到了{1}次,勇不可挡！"), Global.GetColorStringForNGUIText(new object[]
				{
					text,
					jingJiChallengeInfoData.challengeName
				}), Global.GetColorStringForNGUIText(new object[]
				{
					text3,
					jingJiChallengeInfoData.value
				}));
				break;
			}
			ZhanbaoItem zhanbaoItem = U3DUtils.NEW<ZhanbaoItem>();
			zhanbaoItem.transform.localPosition = new Vector3(0f, 0f, -0.01f);
			zhanbaoItem.m_Text.Text = text4;
		}
		this.currPage++;
		base.Invoke("Continue", 1f);
	}

	public GButton m_BtnClose;

	public UIScrollBar m_scrollBar;

	private int currPage;

	public UILabel m_LabelDuanWei;

	public UILabel m_LabelShengLv;

	public UILabel m_LabelLianSheng;

	public UILabel m_LabelDuanWeiJiFeng;

	public UILabel m_LabelRongYaoDian;
}
