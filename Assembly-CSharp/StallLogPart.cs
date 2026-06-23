using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class StallLogPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_ListLogObC = this.m_ListLog.ItemsSource;
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
		GameInstance.Game.SpriteGetBaiTanLogCmd(0);
	}

	public void InitLogElement(List<BaiTanLogItemData> loglist)
	{
		if (loglist == null)
		{
			return;
		}
		for (int i = 0; i < loglist.Count; i++)
		{
			StallLogItem stallLogItem = U3DUtils.NEW<StallLogItem>();
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(loglist[i].GoodsID);
			dummyGoodsData.GCount = loglist[i].GoodsNum;
			dummyGoodsData.Forge_level = loglist[i].ForgeLevel;
			dummyGoodsData.SaleMoney1 = loglist[i].YinLiang;
			dummyGoodsData.SaleYuanBao = loglist[i].TotalPrice;
			stallLogItem.goodsdata = dummyGoodsData;
			stallLogItem.m_LblGouMaiZheName.text = loglist[i].OtherRName;
			stallLogItem.m_LblGouMaiTime.text = loglist[i].BuyTime;
			this.m_ListLogObC.AddNoUpdate(stallLogItem);
			this.m_ListLogObC.DelayUpdate();
		}
	}

	private List<GoodsData> GetBeiBaoWuPin()
	{
		List<GoodsData> list = new List<GoodsData>();
		if (list == null || Global.Data.roleData.GoodsDataList == null)
		{
			return list;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.GoodsDataList[i].Using == 0)
			{
				list.Add(Global.Data.roleData.GoodsDataList[i]);
			}
		}
		return list;
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListLog = new ListBox();

	private ObservableCollection m_ListLogObC;

	public GButton m_BtnClose;
}
