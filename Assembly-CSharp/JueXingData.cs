using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using XMLCreater;

public class JueXingData
{
	public static MUAwakenSuitInfos GetAwakenSuitInfos()
	{
		if (JueXingData.m_AwakenSuitInfos == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AwakenSuit.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AwakenSuit.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_AwakenSuitInfos = new MUAwakenSuitInfos(gameResXml);
		}
		return JueXingData.m_AwakenSuitInfos;
	}

	public static MUAwakenSuitDetail GetAwakenSuitDetailById(int id)
	{
		return JueXingData.GetAwakenSuitInfos().GetAwakenSuitByID(id);
	}

	public static MUAwakenActivationInfos GetAwakenActivationInfos()
	{
		if (JueXingData.m_AwakenActivationInfos == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AwakenActivation.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AwakenActivation.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_AwakenActivationInfos = new MUAwakenActivationInfos(gameResXml);
		}
		return JueXingData.m_AwakenActivationInfos;
	}

	public static MUAwakenActivationDetail GetJueXingShiInfoById(int id)
	{
		return JueXingData.GetAwakenActivationInfos().GetAwakenActivationDetailByID(id);
	}

	public static MUAwakenLevelInfos GetAwakenLevelInfos()
	{
		if (JueXingData.m_AwakenLevelInfos == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AwakenLevel.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AwakenLevel.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_AwakenLevelInfos = new MUAwakenLevelInfos(gameResXml);
		}
		return JueXingData.m_AwakenLevelInfos;
	}

	public static List<MUAwakenLevelDetail> GetLevelsByOrder(int order)
	{
		return JueXingData.GetAwakenLevelInfos().GetAwakenLevelByOrder(order);
	}

	public static MUAwakenLevelDetail GetLevelsByOrderAndstar(int order, int star)
	{
		return JueXingData.GetAwakenLevelInfos().GetAwakenLevelByOrderStar(order, star);
	}

	public static ChangeableRulePart.RuleXml GetHelpData()
	{
		if (JueXingData.m_helperData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AwakenIntro.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AwakenIntro.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_helperData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return JueXingData.m_helperData;
	}

	private static MUWeaponMasters GetweaponMasters()
	{
		if (JueXingData.m_weaponMasters == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/WeaponMaster.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/WeaponMaster.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_weaponMasters = new MUWeaponMasters(gameResXml);
		}
		return JueXingData.m_weaponMasters;
	}

	public static List<MUWeaponMasterInfo> GetweaponTaoZhuangMasterInfos(MUAwakenSuitDetail taoZhuang)
	{
		List<MUWeaponMasterInfo> result = new List<MUWeaponMasterInfo>();
		if (taoZhuang != null)
		{
			result = JueXingData.GetweaponMasters().GetWeaponMasterByType(taoZhuang.WeaponMasterType);
		}
		return result;
	}

	private static MUAllAwakenRecoverys GetAllAwakenRecoverys()
	{
		if (JueXingData.m_recoverys == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/AwakenRecovery.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/AwakenRecovery.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_recoverys = new MUAllAwakenRecoverys(gameResXml);
		}
		return JueXingData.m_recoverys;
	}

	public static int GetAwakenNumByGoodsID(int goodsId)
	{
		return JueXingData.GetAllAwakenRecoverys().GetAwakenNumByGoodsID(goodsId);
	}

	private static MUAllPassiveEffects GetAllPassiveEffects()
	{
		if (JueXingData.m_rassiveEffects == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/PassiveEffect.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/PassiveEffect.xml 出现错误"
				});
				return null;
			}
			JueXingData.m_rassiveEffects = new MUAllPassiveEffects(gameResXml);
		}
		return JueXingData.m_rassiveEffects;
	}

	public static MUPassiveEffect GetPassiveEffectByID(int id)
	{
		return JueXingData.GetAllPassiveEffects().GetPassiveEffectByID(id);
	}

	public static void CleanJueXingXMLData()
	{
		JueXingData.m_AwakenSuitInfos = null;
		JueXingData.m_AwakenActivationInfos = null;
		JueXingData.m_AwakenLevelInfos = null;
		JueXingData.m_helperData = null;
		JueXingData.m_weaponMasters = null;
		JueXingData.m_recoverys = null;
		JueXingData.m_rassiveEffects = null;
	}

	public static void SetSelfJueXingData(JueXingShiData data)
	{
		JueXingShiData jueXingShiData;
		if (data == null)
		{
			jueXingShiData = new JueXingShiData();
			jueXingShiData.AttackEquip = 0;
			jueXingShiData.DefenseEquip = 0;
			jueXingShiData.JueXingJi = 0;
			jueXingShiData.JueXingJie = 1;
			jueXingShiData.TaoZhuangList = new List<TaoZhuangData>();
		}
		else
		{
			jueXingShiData = data;
			if (jueXingShiData.TaoZhuangList == null)
			{
				jueXingShiData.TaoZhuangList = new List<TaoZhuangData>();
			}
		}
		GameInstance.Game.CurrentSession.roleData.JueXingData = jueXingShiData;
		JueXingData.ResetJiHuoShiList();
	}

	public static JueXingShiData GetSelfJueXingData()
	{
		return GameInstance.Game.CurrentSession.roleData.JueXingData;
	}

	public static TaoZhuangData GetTaoZhuangById(JueXingShiData data, int taoZhuangId)
	{
		if (data == null)
		{
			return null;
		}
		if (data.TaoZhuangList == null)
		{
			return null;
		}
		for (int i = 0; i < data.TaoZhuangList.Count; i++)
		{
			if (data.TaoZhuangList[i].ID == taoZhuangId)
			{
				return data.TaoZhuangList[i];
			}
		}
		return null;
	}

	public static void JueXingJiHuo(int taoZhuangId, int JueXingShiId)
	{
		TaoZhuangData taoZhuangData = JueXingData.GetTaoZhuangById(JueXingData.GetSelfJueXingData(), taoZhuangId);
		if (taoZhuangData == null)
		{
			taoZhuangData = new TaoZhuangData();
			taoZhuangData.ID = taoZhuangId;
			JueXingData.GetSelfJueXingData().TaoZhuangList.Add(taoZhuangData);
		}
		if (taoZhuangData.ActiviteList == null)
		{
			taoZhuangData.ActiviteList = new List<int>();
		}
		if (taoZhuangData.ActiviteList.IndexOf(JueXingShiId) < 0)
		{
			taoZhuangData.ActiviteList.Add(JueXingShiId);
		}
		JueXingData.ResetJiHuoShiList();
	}

	public static void ResetJiHuoShiList()
	{
		if (JueXingData.m_lstJiHuoJueXingShi == null)
		{
			JueXingData.m_lstJiHuoJueXingShi = new List<int>();
		}
		JueXingData.m_lstJiHuoJueXingShi.Clear();
		for (int i = 0; i < JueXingData.GetSelfJueXingData().TaoZhuangList.Count; i++)
		{
			if (JueXingData.GetSelfJueXingData().TaoZhuangList[i].ActiviteList == null)
			{
				JueXingData.GetSelfJueXingData().TaoZhuangList[i].ActiviteList = new List<int>();
			}
			for (int j = 0; j < JueXingData.GetSelfJueXingData().TaoZhuangList[i].ActiviteList.Count; j++)
			{
				JueXingData.m_lstJiHuoJueXingShi.Add(JueXingData.GetSelfJueXingData().TaoZhuangList[i].ActiviteList[j]);
			}
		}
	}

	public static bool IsSelfJiHuo(int jueXingShiId, int taoZhuangID)
	{
		JueXingShiData selfJueXingData = JueXingData.GetSelfJueXingData();
		if (selfJueXingData == null)
		{
			return false;
		}
		if (selfJueXingData.TaoZhuangList == null)
		{
			return false;
		}
		TaoZhuangData taoZhuangData = selfJueXingData.TaoZhuangList.Find((TaoZhuangData info) => info.ID == taoZhuangID);
		return taoZhuangData != null && taoZhuangData.ActiviteList != null && taoZhuangData.ActiviteList.IndexOf(jueXingShiId) > -1;
	}

	public static List<MUAwakenActivationDetail> GetSelfTaoZhuangJiHuoList()
	{
		List<MUAwakenActivationDetail> list = new List<MUAwakenActivationDetail>();
		JueXingShiData selfJueXingData = JueXingData.GetSelfJueXingData();
		if (selfJueXingData == null)
		{
			return list;
		}
		if (selfJueXingData.TaoZhuangList == null)
		{
			return list;
		}
		JueXingData.ResetSelfJueXingEquips();
		Dictionary<int, GoodsData> selfJueXingEquips = JueXingData.GetSelfJueXingEquips();
		JueXingData.ResetJiHuoShiList();
		List<int> selfJiHuoList = JueXingData.GetSelfJiHuoList();
		for (int i = 0; i < selfJiHuoList.Count; i++)
		{
			MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(selfJiHuoList[i]);
			if (jueXingShiInfoById != null)
			{
				int position = jueXingShiInfoById.Position;
				GoodsData equipInfo = JueXingData.GetEquipInfo(selfJueXingEquips, (JueXingPositionType)position);
				if (JueXingData.IsCanEffect(equipInfo))
				{
					list.Add(jueXingShiInfoById);
				}
			}
		}
		return list;
	}

	public static List<int> GetSelfJiHuoList()
	{
		if (JueXingData.m_lstJiHuoJueXingShi == null)
		{
			JueXingData.ResetJiHuoShiList();
		}
		return JueXingData.m_lstJiHuoJueXingShi;
	}

	public static List<int> GetEquipedJiHuoList(JueXingShiData jueXingData)
	{
		List<int> list = new List<int>();
		if (jueXingData == null)
		{
			return list;
		}
		TaoZhuangData taoZhuangById = JueXingData.GetTaoZhuangById(jueXingData, jueXingData.AttackEquip);
		if (taoZhuangById != null && taoZhuangById.ActiviteList != null && taoZhuangById.ActiviteList.Count > 0)
		{
			list.AddRange(taoZhuangById.ActiviteList);
		}
		TaoZhuangData taoZhuangById2 = JueXingData.GetTaoZhuangById(jueXingData, jueXingData.DefenseEquip);
		if (taoZhuangById2 != null && taoZhuangById2.ActiviteList != null && taoZhuangById2.ActiviteList.Count > 0)
		{
			list.AddRange(taoZhuangById2.ActiviteList);
		}
		return list;
	}

	public static int GetEquipTaoZhuangId(JueXingTaoZhuangType type)
	{
		if (type == JueXingTaoZhuangType.AttackType)
		{
			return JueXingData.GetSelfJueXingData().AttackEquip;
		}
		return JueXingData.GetSelfJueXingData().DefenseEquip;
	}

	public static void SetEquipTaoZhuangId(JueXingTaoZhuangType type, int taoZhuangId)
	{
		if (type == JueXingTaoZhuangType.AttackType)
		{
			JueXingData.GetSelfJueXingData().AttackEquip = taoZhuangId;
		}
		else
		{
			JueXingData.GetSelfJueXingData().DefenseEquip = taoZhuangId;
		}
	}

	public static int GetMaterialNum(int materialId)
	{
		return Global.GetTotalGoodsCountByID(materialId);
	}

	public static long GetJueXingZhiChenNum()
	{
		return Global.Data.roleData.MoneyData[133];
	}

	public static int GetJieShu()
	{
		return JueXingData.GetSelfJueXingData().JueXingJie;
	}

	public static int GetXingShu()
	{
		return JueXingData.GetSelfJueXingData().JueXingJi;
	}

	public static void SetOpenWindowType(JueXingWindowType type)
	{
		JueXingData.m_openWindowType = type;
	}

	public static JueXingWindowType GetOpenWindowType()
	{
		return JueXingData.m_openWindowType;
	}

	public static List<MUAwakenLevelDetail> GetAdvancedEffectLevels()
	{
		if (JueXingData.m_AdvancedEffectLevels == null)
		{
			JueXingData.m_AdvancedEffectLevels = new List<MUAwakenLevelDetail>();
			MUAwakenLevelInfos awakenLevelInfos = JueXingData.GetAwakenLevelInfos();
			for (int i = 0; i < awakenLevelInfos.AwakenLevels.Count; i++)
			{
				if (awakenLevelInfos.AwakenLevels[i].AdvancedEffects.Count > 0)
				{
					JueXingData.m_AdvancedEffectLevels.Add(awakenLevelInfos.AwakenLevels[i]);
				}
			}
		}
		return JueXingData.m_AdvancedEffectLevels;
	}

	public static int GetJiHuoJueXingShiNum(MUAwakenSuitDetail taoZhuang)
	{
		int num = 0;
		for (int i = 0; i < taoZhuang.AwakenIDs.Count; i++)
		{
			if (JueXingData.IsSelfJiHuo(taoZhuang.AwakenIDs[i], taoZhuang.ID))
			{
				num++;
			}
		}
		return num;
	}

	public static int GetJueXingShiEffectNum(MUAwakenSuitDetail taoZhuang, RoleData roleData)
	{
		int num = 0;
		if (roleData == null)
		{
			return 0;
		}
		JueXingShiData jueXingData = roleData.JueXingData;
		if (jueXingData == null)
		{
			return 0;
		}
		if (jueXingData.TaoZhuangList == null)
		{
			return 0;
		}
		Dictionary<int, GoodsData> dicEquips;
		if (roleData == GameInstance.Game.CurrentSession.roleData)
		{
			dicEquips = JueXingData.GetSelfJueXingEquips();
		}
		else
		{
			dicEquips = JueXingData.GetJueXingEquips(roleData);
		}
		TaoZhuangData taoZhuangData = jueXingData.TaoZhuangList.Find((TaoZhuangData info) => info.ID == taoZhuang.ID);
		if (taoZhuangData == null)
		{
			return 0;
		}
		if (taoZhuangData.ActiviteList == null)
		{
			return 0;
		}
		for (int i = 0; i < taoZhuang.AwakenIDs.Count; i++)
		{
			if (taoZhuangData.ActiviteList.IndexOf(taoZhuang.AwakenIDs[i]) > -1)
			{
				MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(taoZhuang.AwakenIDs[i]);
				if (jueXingShiInfoById != null)
				{
					int position = jueXingShiInfoById.Position;
					GoodsData equipInfo = JueXingData.GetEquipInfo(dicEquips, (JueXingPositionType)position);
					if (JueXingData.IsCanEffect(equipInfo))
					{
						num++;
					}
				}
			}
		}
		return num;
	}

	public static bool IsCanEffect(GoodsData equip)
	{
		if (equip == null)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(equip.GoodsID);
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AwakenCondition", ',');
		int num = systemParamIntArrayByName[0];
		int num2 = systemParamIntArrayByName[1];
		return goodsXmlNodeByID.SuitID >= num && Global.GetZhuoyueAttributeCount(equip) >= num2;
	}

	public static GoodsQuality GetEquipQualityByExcellNum(int excellenceNum)
	{
		if (6 <= excellenceNum)
		{
			return GoodsQuality.FlashPurple;
		}
		if (excellenceNum == 5)
		{
			return GoodsQuality.Purple;
		}
		if (3 <= excellenceNum && 5 > excellenceNum)
		{
			return GoodsQuality.Blue;
		}
		if (0 < excellenceNum && 3 > excellenceNum)
		{
			return GoodsQuality.Green;
		}
		return GoodsQuality.White;
	}

	public static bool BeHaveCanJuHuo()
	{
		List<MUAwakenSuitDetail> awakenSuits = JueXingData.GetAwakenSuitInfos().AwakenSuits;
		for (int i = 0; i < awakenSuits.Count; i++)
		{
			MUAwakenSuitDetail muawakenSuitDetail = awakenSuits[i];
			for (int j = 0; j < muawakenSuitDetail.AwakenIDs.Count; j++)
			{
				int num = muawakenSuitDetail.AwakenIDs[j];
				if (!JueXingData.IsSelfJiHuo(num, muawakenSuitDetail.ID))
				{
					MUAwakenActivationDetail jueXingShiInfoById = JueXingData.GetJueXingShiInfoById(num);
					int materialNum = JueXingData.GetMaterialNum(jueXingShiInfoById.Material.MaterialId);
					int num2 = jueXingShiInfoById.Material.Num;
					if (materialNum >= num2)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static string GetJueXingTaoZhuangIconUrl(MUAwakenSuitDetail suit)
	{
		if (suit == null)
		{
			return string.Empty;
		}
		return string.Format("NetImages/GameRes/Images/JueXing/{0}.qj", suit.Icon);
	}

	public static string GetJueXingShiIconURL(MUAwakenActivationDetail jueXingShi)
	{
		if (jueXingShi == null)
		{
			return string.Empty;
		}
		return string.Format("NetImages/GameRes/Images/JueXing/{0}.qj", jueXingShi.Icon);
	}

	public static string GetItemURL(int materidId)
	{
		return StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			materidId
		});
	}

	public static List<MUAwakenSuitDetail> GetTaoZhuangsByType(JueXingTaoZhuangType type)
	{
		List<MUAwakenSuitDetail> awakenSuits = JueXingData.GetAwakenSuitInfos().AwakenSuits;
		List<MUAwakenSuitDetail> list = new List<MUAwakenSuitDetail>();
		for (int i = 0; i < awakenSuits.Count; i++)
		{
			if (awakenSuits[i].Type == (int)type)
			{
				list.Add(awakenSuits[i]);
			}
		}
		return list;
	}

	public static Dictionary<string, string> GetShuXingString(List<MUPropInfo> infos, float enlargeRate = 1f)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		List<MUPropInfo> list = new List<MUPropInfo>(infos);
		while (list.Count > 0)
		{
			MUPropInfo mupropInfo = list[0];
			if (mupropInfo.PropName.StartsWith("Min") || mupropInfo.PropName.StartsWith("Max"))
			{
				string realName = mupropInfo.PropName.Substring(3);
				string chineaseName = JueXingData.GetChineaseName(realName);
				if (chineaseName == string.Empty)
				{
					float num = mupropInfo.PropNum * enlargeRate;
					string text = string.Empty;
					if (mupropInfo.BePercent)
					{
						text = (num * 100f).ToString("f0") + "%";
					}
					else
					{
						text = ((int)num).ToString();
					}
					dictionary[mupropInfo.ChinesePropName] = text;
					list.Remove(mupropInfo);
				}
				else
				{
					MUPropInfo mupropInfo2 = list.Find((MUPropInfo propInfo) => propInfo.PropName == "Min" + realName);
					MUPropInfo mupropInfo3 = list.Find((MUPropInfo propInfo) => propInfo.PropName == "Max" + realName);
					if (mupropInfo2 == null || mupropInfo3 == null)
					{
						MUDebug.LogError<string>(new string[]
						{
							"请检查配置文件 Max Min 是否配套"
						});
						float num2 = mupropInfo.PropNum * enlargeRate;
						string text2 = string.Empty;
						if (mupropInfo.BePercent)
						{
							text2 = (num2 * 100f).ToString("f0") + "%";
						}
						else
						{
							text2 = ((int)num2).ToString();
						}
						dictionary[chineaseName] = text2;
						list.Remove(mupropInfo);
					}
					else
					{
						dictionary[chineaseName] = ((int)(mupropInfo2.PropNum * enlargeRate)).ToString() + " - " + ((int)(mupropInfo3.PropNum * enlargeRate)).ToString();
						list.Remove(mupropInfo2);
						list.Remove(mupropInfo3);
					}
				}
			}
			else
			{
				float num3 = mupropInfo.PropNum * enlargeRate;
				string text3 = string.Empty;
				if (mupropInfo.BePercent)
				{
					text3 = (num3 * 100f).ToString("f0") + "%";
				}
				else
				{
					text3 = ((int)num3).ToString();
				}
				dictionary[mupropInfo.ChinesePropName] = text3;
				list.Remove(mupropInfo);
			}
		}
		return dictionary;
	}

	private static string GetChineaseName(string name)
	{
		string result = string.Empty;
		if (name == "Defense")
		{
			result = Global.GetLang("物理防御");
		}
		else if (name == "MDefense")
		{
			result = Global.GetLang("魔法防御");
		}
		else if (name == "Attack")
		{
			result = Global.GetLang("物理攻击");
		}
		else if (name == "MAttack")
		{
			result = Global.GetLang("魔法攻击");
		}
		return result;
	}

	public static void ShowErrorMessage(int errorCode)
	{
		Super.HintMainText(JueXingActionResultType.GetMessage(errorCode), 10, 3);
	}

	public static List<MUWeaponMasterInfo> GetCurrentWeaponMasters(MUAwakenSuitDetail taoZhuang, out MUWeaponMasterInfo effected, RoleData roleData)
	{
		List<MUWeaponMasterInfo> list = new List<MUWeaponMasterInfo>();
		list = JueXingData.GetweaponTaoZhuangMasterInfos(taoZhuang);
		effected = null;
		if (roleData == null)
		{
			return list;
		}
		Dictionary<int, GoodsData> dictionary;
		if (roleData == Global.Data.roleData)
		{
			dictionary = JueXingData.GetSelfJueXingEquips();
		}
		else
		{
			dictionary = JueXingData.GetJueXingEquips(roleData);
		}
		if (dictionary.Count <= 0)
		{
			return list;
		}
		int jueXingShiEffectNum = JueXingData.GetJueXingShiEffectNum(taoZhuang, roleData);
		if (jueXingShiEffectNum >= taoZhuang.GetWeaponMasterNeedJiHuoNum())
		{
			GoodsData equipInfo = JueXingData.GetEquipInfo(dictionary, JueXingPositionType.WuQiZuo);
			GoodsData equipInfo2 = JueXingData.GetEquipInfo(dictionary, JueXingPositionType.WuQiYou);
			for (int i = 0; i < list.Count; i++)
			{
				MUWeaponMasterInfo muweaponMasterInfo = list[i];
				if (muweaponMasterInfo.WeaponType1.Count > 0 && muweaponMasterInfo.WeaponType2.Count > 0 && equipInfo != null && equipInfo2 != null)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(equipInfo.GoodsID);
					GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(equipInfo2.GoodsID);
					if (muweaponMasterInfo.WeaponType1.IndexOf(goodsXmlNodeByID.Categoriy) != -1 && muweaponMasterInfo.WeaponType2.IndexOf(goodsXmlNodeByID2.Categoriy) != -1)
					{
						effected = muweaponMasterInfo;
						break;
					}
				}
				if (muweaponMasterInfo.WeaponType1.Count > 0 && muweaponMasterInfo.WeaponType2.Count == 0)
				{
					GoodVO goodVO = null;
					if (equipInfo != null && equipInfo2 == null)
					{
						goodVO = ConfigGoods.GetGoodsXmlNodeByID(equipInfo.GoodsID);
					}
					else if (equipInfo2 != null && equipInfo == null)
					{
						goodVO = ConfigGoods.GetGoodsXmlNodeByID(equipInfo2.GoodsID);
					}
					if (goodVO != null && muweaponMasterInfo.WeaponType1.IndexOf(goodVO.Categoriy) != -1)
					{
						effected = muweaponMasterInfo;
						break;
					}
				}
			}
		}
		return list;
	}

	public static string GetTaoZhuangShuXingDes(MUAwakenSuitDetail taoZhuang, TaoZhuangDesSettingInfo settingInfo)
	{
		if (taoZhuang == null)
		{
			return string.Empty;
		}
		string shuXingNameColor = settingInfo.shuXingNameColor;
		string shuXingValuecolor = settingInfo.shuXingValuecolor;
		string notVisableColor = settingInfo.notVisableColor;
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			shuXingNameColor,
			taoZhuang.Name
		});
		string result = string.Empty;
		if (settingInfo.beShowTaoZhuangName)
		{
			result = string.Format("{0}\n\r{1}\n\r{2}\n\r{3}", new object[]
			{
				colorStringForNGUIText,
				JueXingData.GetDesString(taoZhuang, JueXingPropIndex.TaoZhuangProps1, settingInfo),
				JueXingData.GetDesString(taoZhuang, JueXingPropIndex.TaoZhuangProps2, settingInfo),
				JueXingData.GetDesString(taoZhuang, JueXingPropIndex.TaoZhuangProps3, settingInfo)
			});
		}
		else
		{
			result = string.Format("{0}\n\r{1}\n\r{2}", JueXingData.GetDesString(taoZhuang, JueXingPropIndex.TaoZhuangProps1, settingInfo), JueXingData.GetDesString(taoZhuang, JueXingPropIndex.TaoZhuangProps2, settingInfo), JueXingData.GetDesString(taoZhuang, JueXingPropIndex.TaoZhuangProps3, settingInfo));
		}
		return result;
	}

	private static string GetDesString(MUAwakenSuitDetail taoZhuang, JueXingPropIndex index, TaoZhuangDesSettingInfo settingInfo)
	{
		if (taoZhuang == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				taoZhuang.ID + Global.GetLang(":该ID套装不存在")
			});
			return string.Empty;
		}
		RoleData roleData = settingInfo.roleData;
		string text = settingInfo.shuXingNameColor;
		string shuXingValuecolor = settingInfo.shuXingValuecolor;
		string notVisableColor = settingInfo.notVisableColor;
		int num;
		List<MUPropInfo> effects;
		if (index == JueXingPropIndex.TaoZhuangProps1)
		{
			num = taoZhuang.TaoZhuangProps1Num;
			effects = taoZhuang.TaoZhuangProps1;
		}
		else if (index == JueXingPropIndex.TaoZhuangProps2)
		{
			num = taoZhuang.TaoZhuangProps2Num;
			effects = taoZhuang.TaoZhuangProps2;
		}
		else
		{
			num = taoZhuang.TaoZhuangProps3Num;
			effects = taoZhuang.TaoZhuangProps3;
		}
		if (num == 0)
		{
			return string.Empty;
		}
		int jueXingShiEffectNum = JueXingData.GetJueXingShiEffectNum(taoZhuang, roleData);
		if (jueXingShiEffectNum < num)
		{
			text = notVisableColor;
		}
		bool flag = jueXingShiEffectNum >= num;
		StringBuilder stringBuilder = new StringBuilder();
		string text2 = string.Format(Global.GetLang("激活{0}件"), num);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			text,
			text2
		}));
		string effectsString = JueXingData.GetEffectsString(effects, false, flag, settingInfo);
		stringBuilder.Append(effectsString);
		if (num == taoZhuang.MagicEffectNum && taoZhuang.MagicId.Count > 0)
		{
			stringBuilder.Append(JueXingData.GetMagicDes(taoZhuang.MagicId, text));
		}
		if (num == taoZhuang.PassiveEffectNum && taoZhuang.PassiveEffectIds.Count > 0)
		{
			stringBuilder.Append(JueXingData.GetPassiveEffectDes(taoZhuang.PassiveEffectIds, text));
		}
		if (num == taoZhuang.GetWeaponMasterNeedJiHuoNum())
		{
			stringBuilder.Append("\n\r" + JueXingData.GetWeapMasterStr(taoZhuang, flag, settingInfo, roleData));
		}
		return stringBuilder.ToString();
	}

	public static string GetWeapMasterStr(MUAwakenSuitDetail taoZhuang, bool beWeaponJiHuo, TaoZhuangDesSettingInfo settingInfo, RoleData roleData)
	{
		MUWeaponMasterInfo muweaponMasterInfo = null;
		List<MUWeaponMasterInfo> currentWeaponMasters = JueXingData.GetCurrentWeaponMasters(taoZhuang, out muweaponMasterInfo, roleData);
		if (currentWeaponMasters.Count == 0)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		string text = settingInfo.shuXingNameColor;
		string shuXingValuecolor = settingInfo.shuXingValuecolor;
		string notVisableColor = settingInfo.notVisableColor;
		if (!beWeaponJiHuo)
		{
			text = notVisableColor;
		}
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			text,
			"    [" + currentWeaponMasters[0].Name + "]"
		}));
		for (int i = 0; i < currentWeaponMasters.Count; i++)
		{
			MUWeaponMasterInfo muweaponMasterInfo2 = currentWeaponMasters[i];
			string text2 = "\n\r        ";
			bool beJiHuo;
			if (muweaponMasterInfo2 != muweaponMasterInfo || !beWeaponJiHuo)
			{
				text = notVisableColor;
				beJiHuo = false;
			}
			else
			{
				text = settingInfo.shuXingNameColor;
				shuXingValuecolor = settingInfo.shuXingValuecolor;
				beJiHuo = true;
			}
			for (int j = 0; j < muweaponMasterInfo2.WeaponType1.Count; j++)
			{
				if (j == 0)
				{
					text2 += Global.GetGoodsType(muweaponMasterInfo2.WeaponType1[j]);
				}
				else
				{
					text2 = text2 + "/" + Global.GetGoodsType(muweaponMasterInfo2.WeaponType1[j]);
				}
			}
			if (muweaponMasterInfo2.WeaponType2.Count > 0)
			{
				text2 += "+";
				for (int k = 0; k < muweaponMasterInfo2.WeaponType2.Count; k++)
				{
					if (k == 0)
					{
						text2 += Global.GetGoodsType(muweaponMasterInfo2.WeaponType2[k]);
					}
					else
					{
						text2 = text2 + "/" + Global.GetGoodsType(muweaponMasterInfo2.WeaponType2[k]);
					}
				}
			}
			stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
			{
				text,
				text2
			}));
			string effectsString = JueXingData.GetEffectsString(muweaponMasterInfo2.WeaponMasterProps, true, beJiHuo, settingInfo);
			stringBuilder.Append(effectsString);
		}
		return stringBuilder.ToString();
	}

	public static string GetEffectsString(List<MUPropInfo> effects, bool beWeaponEffect, bool beJiHuo, TaoZhuangDesSettingInfo settingInfo)
	{
		string text = settingInfo.shuXingNameColor;
		string text2 = settingInfo.shuXingValuecolor;
		string notVisableColor = settingInfo.notVisableColor;
		if (!beJiHuo)
		{
			text = notVisableColor;
			text2 = notVisableColor;
		}
		string text3 = settingInfo.effectjiange;
		if (beWeaponEffect)
		{
			text3 = settingInfo.weaponEffectjiange;
		}
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<string, string> shuXingString = JueXingData.GetShuXingString(effects, 1f);
		Dictionary<string, string>.Enumerator enumerator = shuXingString.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object[] array = new object[2];
			array[0] = text;
			int num = 1;
			KeyValuePair<string, string> keyValuePair = enumerator.Current;
			array[num] = Global.GetLang(keyValuePair.Key + " : ");
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
			object[] array2 = new object[2];
			array2[0] = text2;
			int num2 = 1;
			KeyValuePair<string, string> keyValuePair2 = enumerator.Current;
			array2[num2] = keyValuePair2.Value;
			string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(array2);
			stringBuilder.AppendFormat("{0}{1}{2}", text3, colorStringForNGUIText, colorStringForNGUIText2);
		}
		return stringBuilder.ToString();
	}

	private static string GetPassiveEffectDes(List<int> passiveEffectIds, string color)
	{
		string text = string.Empty;
		for (int i = 0; i < passiveEffectIds.Count; i++)
		{
			MUPassiveEffect passiveEffectByID = JueXingData.GetPassiveEffectByID(passiveEffectIds[i]);
			if (passiveEffectByID != null)
			{
				text = text + "\n    " + Global.GetColorStringForNGUIText(new object[]
				{
					color,
					passiveEffectByID.Description
				});
			}
		}
		return text;
	}

	private static string GetMagicDes(List<int> majicIds, string color)
	{
		string text = string.Empty;
		for (int i = 0; i < majicIds.Count; i++)
		{
			MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(majicIds[i]);
			if (maigcInfoVOByCode != null)
			{
				text = text + "\n    " + Global.GetColorStringForNGUIText(new object[]
				{
					color,
					maigcInfoVOByCode.Description
				});
			}
		}
		return text;
	}

	public static string GetPadRightString(string str, int length)
	{
		int num = length - Encoding.UTF8.GetBytes(str).Length;
		num *= 2;
		if (num < 0)
		{
			num = 0;
		}
		return str + new string(' ', num);
	}

	public static Dictionary<int, GoodsData> GetSelfJueXingEquips()
	{
		if (JueXingData.m_jueXingEquips.Count == 0)
		{
			JueXingData.m_jueXingEquips = JueXingData.GetJueXingEquips(GameInstance.Game.CurrentSession.roleData);
		}
		return JueXingData.m_jueXingEquips;
	}

	public static void ResetSelfJueXingEquips()
	{
		JueXingData.m_jueXingEquips = JueXingData.GetJueXingEquips(GameInstance.Game.CurrentSession.roleData);
	}

	private static List<GoodsData> GetUsingGoodsList(RoleData roleData)
	{
		List<GoodsData> list = new List<GoodsData>();
		if (roleData == null)
		{
			return list;
		}
		if (roleData.GoodsDataList == null)
		{
			return list;
		}
		for (int i = 0; i < roleData.GoodsDataList.Count; i++)
		{
			if (roleData.GoodsDataList[i].Using == 1 && roleData.GoodsDataList[i].Site == 0)
			{
				list.Add(roleData.GoodsDataList[i]);
			}
		}
		return list;
	}

	public static Dictionary<int, GoodsData> GetJueXingEquips(RoleData roleData)
	{
		Dictionary<int, GoodsData> dictionary = new Dictionary<int, GoodsData>();
		if (roleData == null)
		{
			return dictionary;
		}
		List<GoodsData> usingGoodsList = JueXingData.GetUsingGoodsList(roleData);
		for (int i = 0; i < usingGoodsList.Count; i++)
		{
			JueXingData.SetJueXingEquipPosition(usingGoodsList[i], dictionary, roleData);
		}
		return dictionary;
	}

	public static GoodsData GetEquipInfo(Dictionary<int, GoodsData> dicEquips, JueXingPositionType position)
	{
		GoodsData result = null;
		if (position != JueXingPositionType.WuQi)
		{
			dicEquips.TryGetValue((int)position, ref result);
			return result;
		}
		GoodsData goodsData = null;
		GoodsData goodsData2 = null;
		dicEquips.TryGetValue(21, ref goodsData);
		dicEquips.TryGetValue(22, ref goodsData2);
		if (goodsData != null && goodsData2 != null)
		{
			if (JueXingData.EquipBatterThan(goodsData2, goodsData))
			{
				dicEquips[1] = goodsData2;
				return goodsData2;
			}
			dicEquips[1] = goodsData;
			return goodsData;
		}
		else
		{
			if (goodsData != null)
			{
				dicEquips[1] = goodsData;
				return goodsData;
			}
			if (goodsData2 != null)
			{
				dicEquips[1] = goodsData2;
				return goodsData2;
			}
			return null;
		}
	}

	public static void SetJueXingEquipPosition(GoodsData equip, Dictionary<int, GoodsData> ret, RoleData roleData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(equip.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			bool flag = equip.BagIndex == 0;
			JueXingPositionType jueXingPositin = JueXingData.GetJueXingPositin(categoriy, flag);
			int num = goodsXmlNodeByID.HandType;
			if (jueXingPositin == JueXingPositionType.WuQi)
			{
				if (categoriy == 17 && roleData.Occupation == 3 && num == 1 && goodsXmlNodeByID.ActionType == 1)
				{
					num = 2;
				}
				if (num == 0)
				{
					ret[22] = equip;
				}
				else if (num == 1)
				{
					ret[21] = equip;
				}
				else if (flag)
				{
					ret[21] = equip;
				}
				else
				{
					ret[22] = equip;
				}
				GoodsData goodsData = null;
				GoodsData goodsData2 = null;
				ret.TryGetValue(21, ref goodsData);
				ret.TryGetValue(22, ref goodsData2);
				if (goodsData != null && goodsData2 != null)
				{
					if (JueXingData.EquipBatterThan(goodsData2, goodsData))
					{
						ret[1] = goodsData2;
					}
					else
					{
						ret[1] = goodsData;
					}
				}
				else if (goodsData != null)
				{
					ret[1] = goodsData;
				}
				else if (goodsData2 != null)
				{
					ret[1] = goodsData2;
				}
			}
			else
			{
				ret[(int)jueXingPositin] = equip;
			}
		}
	}

	private static bool EquipBatterThan(GoodsData youGoodData, GoodsData zuoGoodData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(zuoGoodData.GoodsID);
		GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(youGoodData.GoodsID);
		bool flag = JueXingData.IsCanEffect(zuoGoodData);
		bool flag2 = JueXingData.IsCanEffect(youGoodData);
		return (!flag || !flag2) && (flag || flag2) && (!flag || flag2) && (flag2 && !flag);
	}

	public static JueXingPositionType GetJueXingPositin(int equipCategoriy, bool beZuo)
	{
		JueXingPositionType result = JueXingPositionType.None;
		switch (equipCategoriy)
		{
		case 0:
			result = JueXingPositionType.TouKui;
			break;
		case 1:
			result = JueXingPositionType.HuJia;
			break;
		case 2:
			result = JueXingPositionType.HuShou;
			break;
		case 3:
			result = JueXingPositionType.HuTui;
			break;
		case 4:
			result = JueXingPositionType.XieZi;
			break;
		case 5:
			result = JueXingPositionType.XiangLian;
			break;
		case 6:
			if (beZuo)
			{
				result = JueXingPositionType.ZuoJieZhi;
			}
			else
			{
				result = JueXingPositionType.YouJieZhi;
			}
			break;
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
			result = JueXingPositionType.WuQi;
			break;
		}
		return result;
	}

	private static MUAwakenSuitInfos m_AwakenSuitInfos = null;

	private static MUAwakenActivationInfos m_AwakenActivationInfos = null;

	private static MUAwakenLevelInfos m_AwakenLevelInfos = null;

	private static ChangeableRulePart.RuleXml m_helperData = null;

	private static MUWeaponMasters m_weaponMasters = null;

	private static MUAllAwakenRecoverys m_recoverys = null;

	private static MUAllPassiveEffects m_rassiveEffects = null;

	private static List<int> m_lstJiHuoJueXingShi;

	public static int mockMaterialNum;

	public static int jueXingZhiChenNum = 0;

	private static JueXingWindowType m_openWindowType = JueXingWindowType.JueXing_XiangQian;

	private static List<MUAwakenLevelDetail> m_AdvancedEffectLevels;

	private static Dictionary<int, GoodsData> m_jueXingEquips = new Dictionary<int, GoodsData>();
}
