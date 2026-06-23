using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using Server.Data;

namespace HSGameEngine.GameFramework.Logic
{
	public class SuperData
	{
		public UserControl GlobalPlayZone;

		public UserControl PlayZoneRoot;

		public GoodsPackListData CurrentGoodsPackListData;

		public ListBox DragingListBox;

		public GIcon DragingItem;

		public bool AllowToCloseWindow;

		public QuickKeyItem[] MainQuickKeyItems = new QuickKeyItem[4];

		public QuickKeyItem[] OtherQuickKeyItems = new QuickKeyItem[4];

		public RoleData OtherRoleData;

		public RoleData OtherRoleData2;

		public List<SaleGoodsData> OtherRolesSaleGoodsDataList = new List<SaleGoodsData>();

		public List<GoodsData> ViewTaskInfoGoodsDataList;

		public int NoTitleWindowCount;

		public List<TeamUIItem> TeamUIItemQueue = new List<TeamUIItem>();

		public List<BangHuiUIItem> BangHuiUIItemQueue = new List<BangHuiUIItem>();

		public List<ShowTextItem> RoleTextQueue = new List<ShowTextItem>();

		public GoodsData UpgradeEquipGoodsData;

		public GoodsData QuickEnchanceEquipGoodsData;

		public GoodsData QuickForgeEquipGoodsData;

		public List<GoodsData> YangGongGoodsDataList;

		public bool AutoUpgradeSkillLevel = true;

		public bool IsChangingLine;

		public double StartConnectTicks;

		public List<string> PersonalTextItemList = new List<string>();

		public bool FirstEnterGameServer = true;

		public Point MenuItemMousePoint = new Point(0, 0);

		public GoodsData CurrentChatGoodsData;

		public LeadInfo LeadInfo = new LeadInfo();

		public Dictionary<int, GoodsData> RoleUsingGoodsDataList = new Dictionary<int, GoodsData>();

		public Dictionary<int, GoodsData> RoleUsingChongShengGoodsDataList = new Dictionary<int, GoodsData>();

		public NetAudioSource GlobalUIAudioSource;

		public List<RoleDamage> BattleScoreList;

		public long LastUpdateBattleScoreListTicks;
	}
}
