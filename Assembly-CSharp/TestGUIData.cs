using System;
using System.Collections.Generic;
using System.Reflection;
using HSGameEngine.GameEngine.Logic;

public static class TestGUIData
{
	public static void ParsePropsResult(int fromRoleID, string resultStr)
	{
		if (fromRoleID == -2 && !string.IsNullOrEmpty(resultStr) && resultStr.StartsWith("角色属性信息详表"))
		{
			string[] array = resultStr.Split(new char[]
			{
				'#'
			});
			if (array != null && array.Length >= 4)
			{
				TestGUIData.Props_Titles.Clear();
				TestGUIData.Props_VArray.Clear();
				TestGUIData.Props_RoleName = array[1];
				for (int i = 2; i < array.Length - 1; i += 2)
				{
					string text = array[i];
					double[] array2 = Global.String2DoubleArray(array[i + 1], ',');
					TestGUIData.Props_Titles.Add(text);
					TestGUIData.Props_VArray.Add(array2);
				}
			}
		}
	}

	public const int ClientViewGridArray_Length = 564;

	public static readonly short[] ClientViewGridArray = new short[]
	{
		0,
		0,
		1,
		0,
		0,
		1,
		0,
		-1,
		-1,
		0,
		-1,
		1,
		1,
		-1,
		0,
		-2,
		2,
		0,
		0,
		2,
		-1,
		-1,
		1,
		1,
		-2,
		0,
		-2,
		1,
		-2,
		-1,
		-1,
		-2,
		0,
		-3,
		-3,
		0,
		1,
		-2,
		1,
		2,
		-1,
		2,
		0,
		3,
		2,
		-1,
		3,
		0,
		2,
		1,
		-4,
		0,
		0,
		-4,
		-3,
		-1,
		-1,
		-3,
		-1,
		3,
		-2,
		-2,
		3,
		-1,
		1,
		3,
		2,
		2,
		0,
		4,
		-3,
		1,
		1,
		-3,
		-2,
		2,
		2,
		-2,
		4,
		0,
		3,
		1,
		-5,
		0,
		2,
		-3,
		3,
		2,
		-3,
		-2,
		4,
		1,
		-4,
		-1,
		-4,
		1,
		1,
		-4,
		5,
		0,
		2,
		3,
		1,
		4,
		-3,
		2,
		-1,
		4,
		3,
		-2,
		-1,
		-4,
		-2,
		-3,
		-2,
		3,
		0,
		5,
		4,
		-1,
		0,
		-5,
		-6,
		0,
		4,
		-2,
		2,
		-4,
		-4,
		2,
		-3,
		-3,
		-4,
		-2,
		-2,
		-4,
		5,
		-1,
		-3,
		3,
		-1,
		5,
		3,
		-3,
		-5,
		-1,
		-2,
		4,
		0,
		6,
		1,
		5,
		-5,
		1,
		4,
		2,
		5,
		1,
		6,
		0,
		1,
		-5,
		-1,
		-5,
		0,
		-6,
		2,
		4,
		3,
		3,
		-7,
		0,
		-6,
		1,
		-2,
		-5,
		0,
		7,
		-1,
		-6,
		4,
		-3,
		-5,
		-2,
		-3,
		4,
		-6,
		-1,
		-2,
		5,
		1,
		-6,
		5,
		-2,
		-4,
		-3,
		-3,
		-4,
		2,
		-5,
		7,
		0,
		4,
		3,
		3,
		4,
		-5,
		2,
		2,
		5,
		1,
		6,
		-1,
		6,
		6,
		1,
		0,
		-7,
		6,
		-1,
		3,
		-4,
		-4,
		3,
		5,
		2,
		2,
		6,
		7,
		1,
		-1,
		7,
		5,
		-3,
		0,
		8,
		-2,
		6,
		7,
		-1,
		8,
		0,
		3,
		5,
		4,
		4,
		6,
		-2,
		6,
		2,
		1,
		7,
		5,
		3,
		2,
		-6,
		-1,
		-7,
		1,
		-7,
		-6,
		2,
		-4,
		4,
		4,
		-4,
		-5,
		3,
		3,
		-5,
		0,
		-8,
		-4,
		-4,
		-2,
		-6,
		-3,
		-5,
		-7,
		-1,
		-7,
		1,
		-5,
		-3,
		-6,
		-2,
		-3,
		5,
		-8,
		0,
		-1,
		8,
		8,
		-1,
		3,
		-6,
		2,
		7,
		6,
		-3,
		1,
		8,
		7,
		-2,
		-6,
		3,
		5,
		4,
		-7,
		2,
		6,
		3,
		5,
		-4,
		2,
		-7,
		3,
		6,
		-4,
		5,
		7,
		2,
		4,
		-5,
		-5,
		4,
		-3,
		6,
		8,
		1,
		0,
		9,
		0,
		-9,
		-2,
		7,
		4,
		5,
		9,
		0,
		1,
		-8,
		9,
		-1,
		-1,
		9,
		0,
		10,
		1,
		-9,
		-4,
		6,
		6,
		-4,
		2,
		-8,
		9,
		1,
		8,
		2,
		-5,
		5,
		4,
		-6,
		4,
		6,
		-6,
		4,
		-3,
		7,
		7,
		3,
		7,
		-3,
		5,
		5,
		6,
		4,
		-2,
		8,
		8,
		-2,
		10,
		0,
		5,
		-5,
		3,
		-7,
		3,
		7,
		2,
		8,
		1,
		9,
		0,
		11,
		11,
		0,
		1,
		10,
		6,
		5,
		5,
		6,
		4,
		7,
		8,
		3,
		9,
		2,
		2,
		9,
		10,
		-1,
		5,
		-6,
		9,
		-2,
		-4,
		7,
		3,
		-8,
		-2,
		9,
		8,
		-3,
		-3,
		8,
		4,
		-7,
		1,
		-10,
		6,
		-5,
		-1,
		10,
		2,
		-9,
		7,
		-4,
		-5,
		6,
		12,
		0,
		7,
		-5,
		8,
		-4,
		-4,
		8,
		6,
		-6,
		5,
		-7,
		0,
		12,
		4,
		-8,
		-2,
		10,
		3,
		-9,
		11,
		-1,
		-1,
		11,
		2,
		-10,
		9,
		-3,
		-3,
		9,
		10,
		-2,
		-2,
		11,
		8,
		-5,
		11,
		-2,
		2,
		-11,
		7,
		-6,
		12,
		-1,
		4,
		-9,
		5,
		-8,
		-1,
		12,
		9,
		-4,
		6,
		-7,
		-3,
		10,
		3,
		-10,
		10,
		-3,
		-2,
		12,
		-11,
		2,
		-11,
		3,
		-10,
		3,
		-9,
		4,
		-9,
		5,
		-8,
		5,
		-8,
		6,
		-7,
		6,
		-7,
		7,
		-6,
		7,
		-5,
		8,
		-5,
		9,
		-4,
		9,
		-4,
		10,
		-10,
		2,
		-9,
		3,
		-8,
		4,
		-7,
		5,
		-6,
		6,
		-5,
		7,
		-9,
		2,
		-8,
		3,
		-7,
		4,
		-6,
		5,
		-10,
		1,
		-9,
		1,
		-8,
		2,
		-7,
		3,
		-8,
		1,
		-9,
		0
	};

	public static List<ServerCommandResult> ServerCommandResultList = new List<ServerCommandResult>();

	public static string FuBenID = "5100";

	public static string FuBenSeqID = "0";

	public static string FindGoodsName = "女神";

	public static string SaleType = "1";

	public static string SaleID = "1";

	public static string SaleColor = "31";

	public static string SaleMoney = "3";

	public static string SaleDesc = "0";

	public static int rebornexptype = 1;

	public static string rebornexp = "怪物";

	public static string SetBangLevel = "9";

	public static string JTTaskID = "任务";

	public static string JTTaskJF = "个数";

	public static FieldInfo[] fields = null;

	public static object obj = null;

	public static string widthPercent = "0.52";

	public static bool wudi = false;

	public static int speed = 1;

	public static int AddAttribCount = 0;

	public static long[] TempAttrib = new long[8];

	public static string GoodsID = "1";

	public static string GoodsName = "空";

	public static string GoodsCount = "1";

	public static string GoodsBind = "0";

	public static string GoodsLevel = "0";

	public static string GoodsAppLev = "0";

	public static string GoodsLuck = "0";

	public static string GoodsExcelence = "0";

	public static string monstersName = "空";

	public static bool GoM = false;

	public static string[] tmpsr = new string[]
	{
		"1",
		"2",
		"3"
	};

	public static string SuitID = "5";

	public static string SuitColorLevel = "6";

	public static string moneyValue = "10000000";

	public static string FamilyID = string.Empty;

	public static string ChengZhanBidSite = "2";

	public static string ChengZhanBidMoney = "20000";

	public static string AddJueXingDianShu = "10000000";

	public static string AddShenShiFenMo = "10000000";

	public static string TP_ProcID = "2";

	public static string TP_Param1 = "0";

	public static string GuangMuID = "1";

	public static string GuangMuState = "0";

	public static string followName = "Name";

	public static string SubStrong = "10";

	public static string HuiJiUp = "阶";

	public static string HuiJiStar = "星";

	public static string HuiJiEXP = "经验";

	public static string HuoDongCount = "0";

	public static string FuBenCount = "-10";

	public static string CommandString = "-baseinfo";

	public static int FreshPlayerSceneFlag;

	public static Dictionary<int, object[]> ZuDangPoints = new Dictionary<int, object[]>();

	public static bool ShowZuDang = false;

	public static bool ShowViewRange = false;

	public static string AngelTempleMonsterUpgradePercent = "1";

	public static string CheatMapCode = "地图ID";

	public static string CheatPosX = "X";

	public static string CheatPosY = "Y";

	public static string UnityTimeScale = "1.00";

	public static string WingSuit = "阶";

	public static string WingStar = "星";

	public static string JXSuit = "阶";

	public static string JXStar = "星";

	public static string MeiLIn = "10";

	public static string ShenQiLevel = "3";

	public static string GuardPoint = "1000000";

	public static string Props_RoleName = string.Empty;

	public static List<string> Props_Titles = new List<string>();

	public static List<double[]> Props_VArray = new List<double[]>();

	public static bool Props_Show = false;

	public static bool Props_Show2 = true;

	public static string pname = string.Empty;

	public static string pv = string.Empty;

	public static string KuaFuMapCode = "501";

	public static string KuaFuLine = "1";

	public static string LangHunFenMo = "10000000";

	public static string YingGuang = "10000000";

	public static string ShenJiJiFen = "10000000";

	public static string PK = "0";

	public static string tianfu = "120";

	public static string CangBaoMiJingPos = "1001";

	public static string CangBaoMiJingJiFen = "10000000";

	public static string CangBaoMiJingXueZuan = "10000000";

	public static string ClienLocalTimeSubServerTimeMinus = "1";

	public static string ClienLocalTimeSubServerTime = "0";

	public static string LianSaiGuanZhanBHID = string.Empty;

	public static string PackedGoodsID = "物品ID";

	public static string PackedGoodsCount = "个数";

	public static string PackedGoodsLevel = "0";

	public static string PackedGoodsAppLev = "0";

	public static string PackedGoodsLuck = "0";

	public static string PackedGoodsExcelence = "0";

	public static string PackedGoodsOffset_X = "X";

	public static string PackedGoodsOffset_Y = "Y";

	public static string NPCID = "0";

	public static string NPCDirection = "5";

	public static string NPCOffset = "150";

	public static string NPCSType = "0";

	public static string U3dName = string.Empty;

	public static string NPCIDLabel = string.Empty;

	public static string ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

	public static string buchangTime = DateTime.Now.ToString("yyyy-MM-dd");

	public static string hefuTime = TestGUIData.buchangTime;

	public static string huodongTime = TestGUIData.buchangTime;

	public static string xinfuTime = TestGUIData.buchangTime;

	public static string KuafuTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

	public static GChildWindow WindowBak;

	public static float VoiceLength = 0f;

	public static float CompressLength = 0f;

	public static float DecompressionLength = 0f;

	public static float TransmitLength = 0f;

	public static string DecryptName = string.Empty;

	public static float DecryptBeginTime = 0f;

	public static float DecryptEndTime = 0f;

	public static string val1 = "1";

	public static string val2 = "2";

	public static string cmd = "1446";

	public static string strcmd = "2";

	public static string val5 = "1";

	public static string val6 = "2";

	public static string typeName = "HuiJiUpdateResultData";

	public static string nameSpace = "Server.Data.";

	public static string[] value = null;

	public static Type type = null;

	public static string yaosaiLevel = "4";

	public static string shengwuLevel = "总等级";

	public static string shengwuid = "圣物ID";

	public static string bujianid = "部件";

	public static string bujianlevel = "数";

	public static string freemodname = "次数";

	public static string citybh = "城池等级";

	public static string lingdi = "地宫0荒漠1";

	public static string lingdioff = "1开2关";

	public static string pkking = Global.Data.roleData.RoleName + string.Empty;

	public static string luolan = Global.Data.roleData.Faction + string.Empty;

	public static string chongzhi = Global.Data.roleData.RoleID + string.Empty;

	public static string wanmota = "10";
}
