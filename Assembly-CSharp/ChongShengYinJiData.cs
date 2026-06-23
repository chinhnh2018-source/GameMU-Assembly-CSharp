using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ChongShengYinJiData
{
	public static ZhuYinJiCfgData GetZhuYinJiCfgById(int id)
	{
		return ChongShengYinJiConfigManager.GetInstance().GetZhuYinJiCfgDataByID(id);
	}

	public static ZhuYinJiCfgData GetZhuYinJiCfgByIdAndType(int id, int type)
	{
		return ChongShengYinJiConfigManager.GetInstance().GetZhuYinJiCfgDataByIDAndType(id, type);
	}

	public static int GetZhuYinJiCfgTypeById(int id)
	{
		ZhuYinJiCfgData zhuYinJiCfgDataByID = ChongShengYinJiConfigManager.GetInstance().GetZhuYinJiCfgDataByID(id);
		if (zhuYinJiCfgDataByID != null)
		{
			return zhuYinJiCfgDataByID.TypeZhu;
		}
		return -1;
	}

	public static string CurrentZhuYinJiInfo(int id, int type)
	{
		ZhuYinJiCfgData zhuYinJiCfgByIdAndType = ChongShengYinJiData.GetZhuYinJiCfgByIdAndType(id, type);
		if (zhuYinJiCfgByIdAndType != null)
		{
			return zhuYinJiCfgByIdAndType.Name;
		}
		return null;
	}

	public static string NextZhuYinJiInfo(int id, int type)
	{
		ZhuYinJiCfgData zhuYinJiCfgByIdAndType = ChongShengYinJiData.GetZhuYinJiCfgByIdAndType(id + 1, type);
		if (zhuYinJiCfgByIdAndType != null)
		{
			return zhuYinJiCfgByIdAndType.Name;
		}
		return null;
	}

	public static int GetZhuIDByTypeAndLevel(int type, int level)
	{
		Dictionary<int, ZhuYinJiCfgData> dictZhuYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZhuYinJiCfgData();
		foreach (KeyValuePair<int, ZhuYinJiCfgData> keyValuePair in dictZhuYinJiCfgData)
		{
			if (keyValuePair.Value.TypeZhu == type)
			{
				Dictionary<int, ZhuYinJiCfgData>.Enumerator enumerator;
				KeyValuePair<int, ZhuYinJiCfgData> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.Level == level)
				{
					KeyValuePair<int, ZhuYinJiCfgData> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value.ID;
				}
			}
		}
		return -1;
	}

	public static ZiYinJiCfgData GetZiYinJiCfgById(int id)
	{
		return ChongShengYinJiConfigManager.GetInstance().GetZiYinJiCfgDataByID(id);
	}

	public static ZiYinJiCfgData GetZiYinJiCfgByIdAndType(int id, int type)
	{
		return ChongShengYinJiConfigManager.GetInstance().GetZiYinJiCfgDataByIDAndType(id, type);
	}

	public static int GetZiYinJiCfgTypeById(int id)
	{
		ZiYinJiCfgData ziYinJiCfgDataByID = ChongShengYinJiConfigManager.GetInstance().GetZiYinJiCfgDataByID(id);
		if (ziYinJiCfgDataByID != null)
		{
			return ziYinJiCfgDataByID.Type;
		}
		return -1;
	}

	public static string CurrentZiYinJiInfo(int id, int type)
	{
		ZiYinJiCfgData ziYinJiCfgByIdAndType = ChongShengYinJiData.GetZiYinJiCfgByIdAndType(id, type);
		if (ziYinJiCfgByIdAndType != null)
		{
			return ziYinJiCfgByIdAndType.Name;
		}
		return null;
	}

	public static string NextZiYinJiInfo(int id, int type)
	{
		ZiYinJiCfgData ziYinJiCfgByIdAndType = ChongShengYinJiData.GetZiYinJiCfgByIdAndType(id + 1, type);
		if (ziYinJiCfgByIdAndType != null)
		{
			return ziYinJiCfgByIdAndType.Name;
		}
		return null;
	}

	public static int GetZiIDByTypeAndLevel(int type, int level)
	{
		Dictionary<int, ZiYinJiCfgData> dictZiYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZiYinJiCfgData();
		foreach (KeyValuePair<int, ZiYinJiCfgData> keyValuePair in dictZiYinJiCfgData)
		{
			if (keyValuePair.Value.Type == type)
			{
				Dictionary<int, ZiYinJiCfgData>.Enumerator enumerator;
				KeyValuePair<int, ZiYinJiCfgData> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.Level == level)
				{
					KeyValuePair<int, ZiYinJiCfgData> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value.ID;
				}
			}
		}
		return -1;
	}

	public static string GetShuXingStr(string shuXing, string color = null)
	{
		if (string.IsNullOrEmpty(shuXing))
		{
			return null;
		}
		string[] array = shuXing.Split(new char[]
		{
			'|'
		});
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[0]);
			if (extPropIndexesVOByWord == null)
			{
				return Global.GetLang("暂无属性");
			}
			string description = extPropIndexesVOByWord.Description;
			int id = extPropIndexesVOByWord.ID;
			if (ConfigExtPropIndexes.GetPercentByID(id))
			{
				stringBuilder.Append((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
				{
					color,
					description
				}) : description);
				stringBuilder.Append((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
				{
					color,
					":   "
				}) : ":   ");
				float num = 0f;
				float.TryParse(array2[1], ref num);
				if (num != 0f)
				{
					string text = num * 100f + "%";
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						text
					}));
				}
				else
				{
					stringBuilder.Append(num * 100f + "%");
				}
				stringBuilder.Append(Environment.NewLine);
			}
			else
			{
				stringBuilder.Append((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
				{
					color,
					description
				}) : description);
				stringBuilder.Append((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
				{
					color,
					":   "
				}) : ":   ");
				float num2 = 0f;
				float.TryParse(array2[1], ref num2);
				if (num2 != 0f)
				{
					stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
					{
						"17e43e",
						num2
					}));
				}
				else
				{
					stringBuilder.Append(num2);
				}
				stringBuilder.Append(Environment.NewLine);
			}
		}
		return stringBuilder.ToString();
	}

	public static List<ChongShengYinJiPropData> GetShuXingList(string shuXing, string color = null)
	{
		if (string.IsNullOrEmpty(shuXing))
		{
			return null;
		}
		List<ChongShengYinJiPropData> list = new List<ChongShengYinJiPropData>();
		string[] array = shuXing.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(array2[0]);
			if (extPropIndexesVOByWord != null)
			{
				ChongShengYinJiPropData chongShengYinJiPropData = default(ChongShengYinJiPropData);
				string description = extPropIndexesVOByWord.Description;
				int id = extPropIndexesVOByWord.ID;
				if (ConfigExtPropIndexes.GetPercentByID(id))
				{
					chongShengYinJiPropData.Title = ((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
					{
						color,
						description
					}) : description) + ((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
					{
						color,
						":"
					}) : ":");
					float num = 0f;
					float.TryParse(array2[1], ref num);
					if (num != 0f)
					{
						string text = num * 100f + "%";
						chongShengYinJiPropData.Value = Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							text
						});
					}
					else
					{
						chongShengYinJiPropData.Value = num * 100f + "%";
					}
					list.Add(chongShengYinJiPropData);
				}
				else
				{
					chongShengYinJiPropData.Title = ((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
					{
						color,
						description
					}) : description) + ((!string.IsNullOrEmpty(color)) ? Global.GetColorStringForNGUIText(new object[]
					{
						color,
						":"
					}) : ":");
					float num2 = 0f;
					float.TryParse(array2[1], ref num2);
					if (num2 != 0f)
					{
						chongShengYinJiPropData.Value = Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							num2
						});
					}
					else
					{
						chongShengYinJiPropData.Value = num2.ToString();
					}
					list.Add(chongShengYinJiPropData);
				}
			}
		}
		return list;
	}

	public static void ClearAll()
	{
		ChongShengYinJiConfigManager.GetInstance().Clear();
	}

	public static string GetErrorMsg(int code)
	{
		string result = string.Empty;
		switch (code)
		{
		case 1:
			result = Global.GetLang("升级成功");
			break;
		case 2:
			result = Global.GetLang("升级失败");
			break;
		case 3:
			result = Global.GetLang("功能未开启");
			break;
		case 4:
			result = Global.GetLang("重生印记选择失败");
			break;
		case 5:
			result = Global.GetLang("未激活");
			break;
		case 6:
			result = Global.GetLang("已激活");
			break;
		case 7:
			result = Global.GetLang("激活失败");
			break;
		case 8:
			result = Global.GetLang("钻石不足");
			break;
		case 9:
			result = Global.GetLang("重置印记信息失败");
			break;
		case 10:
			result = Global.GetLang("获取印记信息失败");
			break;
		case 11:
			result = Global.GetLang("升级印记点不足");
			break;
		case 12:
			result = Global.GetLang("升级类型错误");
			break;
		case 13:
			result = Global.GetLang("升级检测未通过");
			break;
		case 14:
			result = Global.GetLang("已达最大等级");
			break;
		case 15:
			result = Global.GetLang("数据存盘失败");
			break;
		case 16:
			result = Global.GetLang("数据升级次数出错");
			break;
		case 17:
			result = Global.GetLang("级数已达上限");
			break;
		default:
			result = Global.GetLang("位置错误码：") + code;
			break;
		}
		return result;
	}

	public static ChangeableRulePart.RuleXml GetHelpData()
	{
		if (ChongShengYinJiData.m_helperData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ChongShengYinJiIntro.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/ChongShengYinJiIntro.xml 出现错误"
				});
				return null;
			}
			ChongShengYinJiData.m_helperData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return ChongShengYinJiData.m_helperData;
	}

	public static void ResetChongShengYinJiEffectPosition(bool isOn)
	{
		Global.Data.GameScene.ResetChongShengYinJiEffectPosition(Global.FindSpriteByID(Global.Data.roleData.RoleID), Global.Data.roleData.RoleID, isOn);
	}

	public static void ResetChongShengYinJiEffect()
	{
		Global.Data.GameScene.AddChongShengYinJiEffect(Global.FindSpriteByID(Global.Data.roleData.RoleID), Global.Data.roleData.RoleID);
	}

	public static List<int> GetEffectIDs(RoleData roleData)
	{
		ChongShengYinJiData.YinJiTypes(roleData);
		List<int> list = new List<int>();
		for (int i = 0; i < ChongShengYinJiData.yinjis.Count; i++)
		{
			list.Add(ChongShengYinJiData.EffectName(ChongShengYinJiData.yinjis[i]));
		}
		return list;
	}

	public static void YinJiTypes(RoleData roleData)
	{
		ChongShengYinJiData.yinjis.Clear();
		if (roleData.RebornYinJi == null)
		{
			return;
		}
		if (roleData.RebornYinJi == null)
		{
			return;
		}
		List<int> stampInfo = roleData.RebornYinJi.StampInfo;
		if (stampInfo == null || stampInfo.Count <= 0)
		{
			return;
		}
		List<int> range = stampInfo.GetRange(0, 8);
		List<int> range2 = stampInfo.GetRange(8, stampInfo.Count - 8);
		ChongShengYinJiData.SaveCurretRoleYinJiTypes(range[0], range2[0]);
	}

	public static void SaveCurretRoleYinJiTypes(int yinji1, int yinji2)
	{
		ChongShengYinJiData.yinjis.Clear();
		ChongShengYinJiData.yinjis.Add(yinji1);
		ChongShengYinJiData.yinjis.Add(yinji2);
	}

	public static int EffectName(int type)
	{
		switch (type)
		{
		case 1:
			return 15558;
		case 2:
			return 15557;
		case 3:
			return 15555;
		case 4:
			return 15559;
		case 5:
			return 15556;
		default:
			return -1;
		}
	}

	private static ChangeableRulePart.RuleXml m_helperData = null;

	private static List<int> yinjis = new List<int>();

	public static int EffectRootID = 15554;

	public enum ResOpcode
	{
		Succ = 1,
		Fail,
		GongNengWeiKaiQi,
		ChooseYinJiTypeErr,
		ChooseGetInfoYinJiNotActive,
		ChooseGetInfoYinJiIsActive,
		ChooseYinJiIsActiveErr,
		ResetYinJiZuanShiErr,
		ResetYinJiInfoErr,
		GetYinJiInfoErr,
		LevelUpYinJiPointErr,
		LevelUpYinJiTypeErr,
		LevelUpYinJiCheckErr,
		LevelUpYinJiMaxLv,
		LevelUpYinJiSaveErr,
		LevelUpYinJiUpNumErr,
		LevelUpYinJiOverUpLvErr
	}
}
