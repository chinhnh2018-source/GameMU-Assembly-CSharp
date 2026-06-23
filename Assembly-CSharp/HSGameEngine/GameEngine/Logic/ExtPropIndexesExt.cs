using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class ExtPropIndexesExt
	{
		public static string[] ExtPropIndexNames = new string[]
		{
			"weight",
			"strong",
			"defense",
			"maxdefense",
			"mdefense",
			"maxmdefense",
			"attack",
			"maxattack",
			"mattack",
			"maxmattack",
			"dsattack",
			"maxdsattack",
			"maxlifev",
			"maxlifepercent",
			"maxmagicv",
			"lucky",
			"curse",
			"hitv",
			"dodge",
			"magicdodgepercent",
			"poisoningreoverpercent",
			"poisoningdodge",
			"liferecoverpercent",
			"magicrecoverpercent",
			"subattackinjurepercent",
			"submattackinjurepercent",
			"maxmagicpercent",
			"ignoredefensepercent",
			"ignoremdefensepercent"
		};

		public static string[] ExtPropIndexChineseNames = new string[]
		{
			Global.GetLang("重量"),
			Global.GetLang("耐久"),
			Global.GetLang("物理防御"),
			Global.GetLang("最大物防"),
			Global.GetLang("魔法防御"),
			Global.GetLang("最大魔防"),
			Global.GetLang("物理攻击"),
			Global.GetLang("最大物攻"),
			Global.GetLang("魔法攻击"),
			Global.GetLang("最大魔攻"),
			Global.GetLang("道术攻击"),
			Global.GetLang("最大道攻"),
			Global.GetLang("生命上限"),
			Global.GetLang("生命上限"),
			Global.GetLang("魔法上限"),
			Global.GetLang("幸    运"),
			Global.GetLang("诅    咒"),
			Global.GetLang("准    确"),
			Global.GetLang("闪    避"),
			Global.GetLang("魔法闪避"),
			Global.GetLang("中毒恢复"),
			Global.GetLang("中毒闪避"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("物伤吸收"),
			Global.GetLang("魔伤吸收"),
			Global.GetLang("魔法上限"),
			Global.GetLang("无视物防"),
			Global.GetLang("无视魔防")
		};

		public static int[] ExtPropIndexPercents = new int[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1
		};
	}
}
