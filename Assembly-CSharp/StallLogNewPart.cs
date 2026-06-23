using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class StallLogNewPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_ListLogObC = this.m_ListLog.ItemsSource;
	}

	public void InitLogElement(List<BaiTanLogItemData> loglist)
	{
		if (loglist == null)
		{
			return;
		}
		this.m_ListLogObC.Clear();
		int count = loglist.Count;
		for (int i = 0; i < count; i++)
		{
			StallLogItem stallLogItem = U3DUtils.NEW<StallLogItem>();
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(loglist[i].GoodsID);
			dummyGoodsData.GCount = loglist[i].GoodsNum;
			dummyGoodsData.Forge_level = loglist[i].ForgeLevel;
			dummyGoodsData.ExcellenceInfo = loglist[i].Excellenceinfo;
			if (!string.IsNullOrEmpty(loglist[i].Washprops))
			{
				byte[] array = Convert.FromBase64String(loglist[i].Washprops);
				List<int> washProps = DataHelper.BytesToObject<List<int>>(array, 0, array.Length);
				dummyGoodsData.WashProps = washProps;
			}
			if (loglist[i].rid == Global.Data.RoleID)
			{
				dummyGoodsData.SaleMoney1 = loglist[i].YinLiang - loglist[i].Tax;
				dummyGoodsData.SaleYuanBao = loglist[i].TotalPrice - loglist[i].Tax;
			}
			else
			{
				dummyGoodsData.SaleMoney1 = loglist[i].YinLiang;
				dummyGoodsData.SaleYuanBao = loglist[i].TotalPrice;
			}
			stallLogItem.IsBuyedState = (Global.Data.roleData.RoleID == loglist[i].OtherRoleID);
			stallLogItem.goodsdata = dummyGoodsData;
			stallLogItem.m_LblGouMaiZheName.text = loglist[i].OtherRName;
			stallLogItem.m_LblGouMaiTime.text = loglist[i].BuyTime;
			this.m_ListLogObC.AddNoUpdate(stallLogItem);
			this.m_ListLogObC.DelayUpdate();
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox m_ListLog = new ListBox();

	private ObservableCollection m_ListLogObC;
}
