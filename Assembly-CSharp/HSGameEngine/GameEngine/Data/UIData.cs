using System;
using System.Collections.Generic;
using Server.Data;

namespace HSGameEngine.GameEngine.Data
{
	public class UIData
	{
		public List<FriendData> FriendDataList;

		public int ExchangeID = -1;

		public ExchangeData ExchangeDataItem;

		public StallData StallDataItem;

		public StallData OtherStallDataItem;

		public TeamData CurrentTeamData;

		public List<DJRoomData> DJRoomDataList;

		public int RoleSalesWindowCount;

		public List<int> FirstNewGoodsIDList = new List<int>();

		public List<int> LastedNewTaskIDList = new List<int>();

		public List<int> LastedCompTaskIDList = new List<int>();

		public List<HorseData> HorsesDataList;

		public HorseData CurrentSelectedHorseData;

		public List<HorseData> OtherHorsesDataList;

		public HorseData CurrentOtherSelectedHorseData;

		public List<PetData> PetsDataList;

		public List<GoodsData> PortableGoodsDataList;

		public DJRoomData CurrentDJRoomData;

		public DJRoomRolesData CurrentDJRoomRolesData;

		public List<JingMaiData> JingMaiDataList;

		public List<JingMaiData> OtherJingMaiDataList;

		public int GMAuth;

		public List<BulletinMsgData> BulletinMsgDataList = new List<BulletinMsgData>();

		public List<GoodsData> SaleGoodsDataList = new List<GoodsData>();

		public List<GoodsData> OtherSaleGoodsDataList = new List<GoodsData>();

		public List<GoodsData> ViewGoodsPackDataList;

		public double[] CurrentRolePropFields;

		public GoodsData WaBaoGoodsData;

		public HuodongData MyHuoDongData;

		public RoleData HuangDiRoleData;

		public List<GoodsData> GiftsGoodsDataList = new List<GoodsData>();

		public List<GoodsData> EmailFujianGoodsDataList = new List<GoodsData>();

		public List<GoodsData> NpcSaleGoodsDataList = new List<GoodsData>();

		public List<GoodsData> BaoKuJiangLiGoodsDataList = new List<GoodsData>();

		public ChengJiuData ChengJiuData;

		public int MoveMode = 1;

		public MallSaleData MallData;

		public JieriXmlData JieriData;

		public bool IsDoingZaJinDan;

		public ActivitiesData ActivitData;

		public bool IsArenaBattling;
	}
}
