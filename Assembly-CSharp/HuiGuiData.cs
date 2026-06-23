using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class HuiGuiData
{
	public static string GetSelfCreateTime()
	{
		return HuiGuiData.selfCreateTime;
	}

	public static MUHuiGuiHuoDong GetSelfHuoDongLevel()
	{
		if (HuiGuiData.m_selfHuoDong == null)
		{
			HuiGuiData.m_selfHuoDong = IConfigbase<ConfigHuiGuiHuoDong>.Instance.GetHuiGuiHuoDongByID(HuiGuiData.huiGuiId);
		}
		return HuiGuiData.m_selfHuoDong;
	}

	public static int GetSelfChongZhiNum()
	{
		return HuiGuiData.selfChongZhiNum;
	}

	public static int GetCurrentHuiGuiDay()
	{
		return HuiGuiData.currentDay;
	}

	public static void OpenBuyItemWindow(int id, GoodVO goodVO, int price, int maxNum, Action<int, GoodVO, int> OnBuyItem)
	{
		if (HuiGuiData.m_buyWindow == null)
		{
			HuiGuiData.m_buyWindow = U3DUtils.NEW<GChildWindow>();
			HuiGuiData.m_buyWindow.IsShowModal = true;
			HuiGuiData.m_buyWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(HuiGuiData.m_buyWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(HuiGuiData.m_buyWindow);
		}
		if (HuiGuiData.m_commonBuyPart == null)
		{
			HuiGuiData.m_commonBuyPart = U3DUtils.NEW<CommonBuyWindow>();
			HuiGuiData.m_commonBuyPart.OnBuyItem = OnBuyItem;
			HuiGuiData.m_commonBuyPart.InitWindow(id, goodVO, price, maxNum);
			HuiGuiData.m_commonBuyPart.OnClose = delegate()
			{
				HuiGuiData.CloseBuyItemWindow();
			};
		}
		HuiGuiData.m_buyWindow.SetContent(HuiGuiData.m_buyWindow.BodyPresenter, HuiGuiData.m_commonBuyPart, 0.0, 0.0, true);
	}

	public static void CloseBuyItemWindow()
	{
		if (null != HuiGuiData.m_commonBuyPart)
		{
			HuiGuiData.m_commonBuyPart.transform.parent = null;
			Object.Destroy(HuiGuiData.m_commonBuyPart.gameObject);
			HuiGuiData.m_commonBuyPart = null;
		}
		if (null != HuiGuiData.m_buyWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, HuiGuiData.m_buyWindow);
			HuiGuiData.m_buyWindow = null;
		}
	}

	public static string selfCreateTime = string.Empty;

	public static int huiGuiId = 0;

	public static int selfChongZhiNum = 0;

	public static int currentDay = 1;

	public static Dictionary<int, int> cacheLoginInfo = new Dictionary<int, int>();

	public static Dictionary<int, int> cacheStoreInfo = new Dictionary<int, int>();

	public static MUHuiGuiHuoDong m_selfHuoDong;

	protected static GChildWindow m_buyWindow = null;

	public static CommonBuyWindow m_commonBuyPart = null;
}
