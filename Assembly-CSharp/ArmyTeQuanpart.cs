using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ArmyTeQuanpart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnCallBack();
	}

	private new void Start()
	{
		this.m_Tab.TabIndex = 1;
		NGUITools.SetActive(this.m_ShuangBei.gameObject, false);
	}

	private void InitTextInPrefabs()
	{
		try
		{
			this.m_Title.Text = Global.GetLang("领地特权");
			this.Btn1Label.text = Global.GetLang("部署弩塔");
			this.Btn1Label.lineWidth = 115;
			this.Btn2Label.text = Global.GetLang("开启双倍");
			this.Btn2Label.lineWidth = 115;
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "越南东南亚英文：预制可能报空了</color>"
			});
		}
	}

	private void InitBtnCallBack()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.m_Tab.TabClick += delegate(GameObject s, int index)
		{
			this.ShowTabs(index);
		};
	}

	private void ShowTabs(int index)
	{
		this.m_Nuta.SetActive(1 == index);
		if (index != 1)
		{
			if (index != 2)
			{
			}
		}
		else
		{
			GameInstance.Game.SendGetShouWeiRequest();
			GameInstance.Game.SendGetRoleArmyGroupData(Global.Data.RoleID);
		}
	}

	public void NotifyRefreshUI(LingZhuShouWeiData data)
	{
		bool flag = false;
		LingDiResultCode result = (LingDiResultCode)data.Result;
		switch (result + 15)
		{
		case (LingDiResultCode)0:
			Super.HintMainText(Global.GetLang("已经部署守卫"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)2:
			Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)6:
			Super.HintMainText(Global.GetLang("不在领地地图"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)7:
			Super.HintMainText(Global.GetLang("不存在领地数据"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)8:
			Super.HintMainText(Global.GetLang("存库失败"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)9:
			Super.HintMainText(Global.GetLang("领主数据不存在"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)10:
			goto IL_163;
		case (LingDiResultCode)11:
			goto IL_163;
		case (LingDiResultCode)12:
			Super.HintMainText(Global.GetLang("不在开启时间内"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)13:
			Super.HintMainText(Global.GetLang("不是领主"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)14:
			Super.HintMainText(Global.GetLang("跨服服务器连接错误"), 10, 3);
			goto IL_163;
		case (LingDiResultCode)16:
			flag = true;
			goto IL_163;
		}
		MUDebug.Log(new object[]
		{
			"军团特权未处理错误类型：",
			data.Result
		});
		IL_163:
		if (!flag)
		{
			return;
		}
		List<LingDiShouWeiData> shouWeiList = data.ShouWeiList;
		int count = shouWeiList.Count;
		for (int i = 0; i < count; i++)
		{
			LingDiShouWeiData lingDiShouWeiData = shouWeiList[i];
			this.m_NuTaScript.InitVaue(i, lingDiShouWeiData.State, lingDiShouWeiData.FreeBuShuTime, lingDiShouWeiData.ZuanShiCost);
		}
	}

	protected override void OnDestroy()
	{
		this.CloseHandler = null;
		this.m_BtnClose = null;
		this.m_Tab = null;
		this.m_Title = null;
		this.m_Nuta = null;
		this.m_ShuangBei = null;
		this.m_NuTaScript = null;
		this.m_ShuangBeiScript = null;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public TextBlock m_Title;

	public GButton m_BtnClose;

	public UITab m_Tab;

	public ShowNetImage mShuangBeiBg;

	public UILabel Btn1Label;

	public UILabel Btn2Label;

	public GameObject m_Nuta;

	public GameObject m_ShuangBei;

	public ArmyTeQuanNuTaContent m_NuTaScript;

	public ArmyTeQuanShuangBeiContent m_ShuangBeiScript;

	private enum TabTypes
	{
		NuTa = 1,
		ShuangBei
	}
}
