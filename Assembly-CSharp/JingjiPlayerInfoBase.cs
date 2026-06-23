using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class JingjiPlayerInfoBase : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = -10
				});
			}
		};
		this.m_PersonTex.ImageURL = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation),
			Global.Data.roleData.RoleSex
		});
		this.m_PersonTex.ForceShow();
		this.collection = this.m_ListBox.ItemsSource;
	}

	protected void initPlayerInfo(int ranking, int winCount, List<XElement> list)
	{
		this.m_RankingLabel.Text = ((ranking >= 1) ? (string.Empty + ranking) : Global.GetLang("500名后"));
		this.m_LianshengLabel.Text = string.Empty + winCount;
		this.m_JunxianLevel = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
		this.JunxianXmlList = list;
		if (this.m_JunxianLevel <= 0)
		{
			this.m_JunxianLabel.Text = Global.GetLang("无");
		}
		else
		{
			this.m_JunxianLabel.Text = Global.GetXElementAttributeStr(this.getElementByLevel(this.m_JunxianLevel), "Name");
		}
		this.refBuff();
	}

	public void refBuff()
	{
		this.m_Shengwang = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWang);
		this.m_ShengwangLabel.Text = string.Empty + this.m_Shengwang;
		long correctLocalTime = Global.GetCorrectLocalTime();
		BufferData bufferDataByID = Global.GetBufferDataByID(87);
		if (bufferDataByID == null || Global.IsBufferDataOver(bufferDataByID, correctLocalTime, false) || bufferDataByID.BufferVal < 0L)
		{
			this.m_DangqianxiaoguoLabel.Text = Global.GetLang("无");
			this.m_ShengyushijianLabel.Text = string.Empty;
		}
		else
		{
			int num = (int)bufferDataByID.BufferVal;
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JunXianBufferGoodsIDs", ',');
			int id = systemParamIntArrayByName[num];
			string goodsNameByID = Global.GetGoodsNameByID(id, false);
			this.m_DangqianxiaoguoLabel.Text = goodsNameByID;
			long num2 = bufferDataByID.StartTime + (long)(bufferDataByID.BufferSecs * 1000);
			long num3 = (num2 - correctLocalTime) / 1000L;
			if (num3 >= 0L)
			{
				this.m_ShengyushijianLabel.Text = UIHelper.FormatSecs(num3, "-");
			}
			else
			{
				this.m_ShengyushijianLabel.Text = string.Empty;
			}
		}
	}

	protected IEnumerator TimeProc()
	{
		for (;;)
		{
			long currentTime = Global.GetCorrectLocalTime();
			BufferData bufferData = Global.GetBufferDataByID(87);
			if (bufferData == null || Global.IsBufferDataOver(bufferData, currentTime, false))
			{
				if (this.m_ShengyushijianLabel.Text != string.Empty)
				{
					this.m_DangqianxiaoguoLabel.Text = Global.GetLang("无");
					this.m_ShengyushijianLabel.Text = string.Empty;
				}
			}
			else
			{
				long doneTiem = bufferData.StartTime + (long)(bufferData.BufferSecs * 1000);
				long cd = (doneTiem - currentTime) / 1000L;
				if (cd >= 0L)
				{
					this.m_ShengyushijianLabel.Text = UIHelper.FormatSecs(cd, "-");
				}
				else if (this.m_ShengyushijianLabel.Text != string.Empty)
				{
					this.m_DangqianxiaoguoLabel.Text = Global.GetLang("无");
					this.m_ShengyushijianLabel.Text = string.Empty;
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	protected XElement getElementByLevel(int value)
	{
		int count = this.JunxianXmlList.Count;
		for (int i = 0; i < count; i++)
		{
			XElement xelement = this.JunxianXmlList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
				if (value == xelementAttributeInt)
				{
					return xelement;
				}
			}
		}
		return null;
	}

	public TextBlock m_RankingLabel;

	public TextBlock m_JunxianLabel;

	public TextBlock m_ShengwangLabel;

	public TextBlock m_LianshengLabel;

	public TextBlock m_DangqianxiaoguoLabel;

	public TextBlock m_ShengyushijianLabel;

	public GButton m_CloseBtn;

	public ShowNetImage m_PersonTex;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListBox;

	protected ObservableCollection collection;

	protected List<XElement> JunxianXmlList;

	protected int m_Shengwang;

	protected int m_JunxianLevel;
}
