using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using XMLCreater;

public class ConfigShenHun : IConfigbase<ConfigShenHun>, ConfigBase
{
	public ConfigShenHun()
	{
		this.XmlClearType = ClearType.ClearOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public MUTransfigurationLevelAll GetAllTransfigurationLevel()
	{
		if (this.m_allTransfigurationLevel == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/TransfigurationLevel.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/TransfigurationLevel.xml 出现错误"
				});
				return null;
			}
			this.m_allTransfigurationLevel = new MUTransfigurationLevelAll(gameResXml);
		}
		return this.m_allTransfigurationLevel;
	}

	public MUTransfigurationLevel GetTransfigurationLevelByID(int id)
	{
		return this.GetAllTransfigurationLevel().GetTransfigurationLevelByID(id);
	}

	public MUTransfigurationLevel GetTransfigurationLevelByOccupationLevel(int occupation, int level)
	{
		return this.GetAllTransfigurationLevel().GetTransfigurationLevelByOccupationLevel(occupation, level);
	}

	public MUTransfigurationLevel GetLastLevelTransfigurationLevelByOccupation(int occupation)
	{
		List<MUTransfigurationLevel> allTransfigurationLevelByOccupation = this.GetAllTransfigurationLevel().GetAllTransfigurationLevelByOccupation(occupation);
		if (allTransfigurationLevelByOccupation != null && allTransfigurationLevelByOccupation.Count > 0)
		{
			return allTransfigurationLevelByOccupation[allTransfigurationLevelByOccupation.Count - 1];
		}
		return null;
	}

	public List<MUTransfigurationLevel> GetAllTransfigurationLevelByOccupation(int occupation)
	{
		return this.GetAllTransfigurationLevel().GetAllTransfigurationLevelByOccupation(occupation);
	}

	public int GetJieSuoLevel(int occupation, int skillIndex)
	{
		int result = 1;
		List<MUTransfigurationLevel> allTransfigurationLevelByOccupation = this.GetAllTransfigurationLevel().GetAllTransfigurationLevelByOccupation(occupation);
		for (int i = 0; i < allTransfigurationLevelByOccupation.Count; i++)
		{
			if (allTransfigurationLevelByOccupation[i].AttackSkill.Count > skillIndex)
			{
				return allTransfigurationLevelByOccupation[i].Level;
			}
		}
		return result;
	}

	public int GetTopBianShenLevel()
	{
		List<MUTransfigurationLevel> transfigurationLevels = this.GetAllTransfigurationLevel().TransfigurationLevels;
		return transfigurationLevels[transfigurationLevels.Count - 1].Level;
	}

	public MUTransfigurationFashionAll GetAllTransfigurationFashion()
	{
		if (this.m_allTransfigurationFashion == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/TransfigurationFashion.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/TransfigurationFashion.xml 出现错误"
				});
				return null;
			}
			this.m_allTransfigurationFashion = new MUTransfigurationFashionAll(gameResXml);
		}
		return this.m_allTransfigurationFashion;
	}

	public MUTransfigurationFashion GetTransfigurationFashionByID(int id)
	{
		return this.GetAllTransfigurationFashion().GetTransfigurationFashionByID(id);
	}

	public MUTransfigurationFashion GetTransfigurationFashion(int goodsId, int level)
	{
		return this.GetAllTransfigurationFashion().GetTransfigurationFashion(goodsId, level);
	}

	public MUTransfigurationFashion GetTopLevelFashion(int goodsId)
	{
		List<MUTransfigurationFashion> transfigurationFashions = this.GetAllTransfigurationFashion().TransfigurationFashions;
		for (int i = transfigurationFashions.Count - 1; i >= 0; i--)
		{
			if (transfigurationFashions[i].GoodsID == goodsId)
			{
				return transfigurationFashions[i];
			}
		}
		return null;
	}

	public int GetFashionSkillJieSuoLevel(int goodsId, int skillIndex)
	{
		int result = 1;
		List<MUTransfigurationFashion> transfigurationFashionByGoodsId = this.GetAllTransfigurationFashion().GetTransfigurationFashionByGoodsId(goodsId);
		for (int i = 0; i < transfigurationFashionByGoodsId.Count; i++)
		{
			if (transfigurationFashionByGoodsId[i].AttackSkill.Count > skillIndex)
			{
				return transfigurationFashionByGoodsId[i].level;
			}
		}
		return result;
	}

	public int GetFashionEffectJieSuoLevel(int goodsId)
	{
		int result = 1;
		List<MUTransfigurationFashion> transfigurationFashionByGoodsId = this.GetAllTransfigurationFashion().GetTransfigurationFashionByGoodsId(goodsId);
		for (int i = 0; i < transfigurationFashionByGoodsId.Count; i++)
		{
			if (transfigurationFashionByGoodsId[i].Effect > 0)
			{
				return transfigurationFashionByGoodsId[i].level;
			}
		}
		return result;
	}

	public List<MUTransfigurationFashion> GetAllFashionTopLevel()
	{
		List<MUTransfigurationFashion> transfigurationFashions = this.GetAllTransfigurationFashion().TransfigurationFashions;
		List<int> list = new List<int>();
		for (int i = 0; i < transfigurationFashions.Count; i++)
		{
			if (list.IndexOf(transfigurationFashions[i].GoodsID) <= -1)
			{
				list.Add(transfigurationFashions[i].GoodsID);
			}
		}
		List<MUTransfigurationFashion> list2 = new List<MUTransfigurationFashion>();
		for (int j = 0; j < list.Count; j++)
		{
			list2.Add(this.GetTopLevelFashion(list[j]));
		}
		return list2;
	}

	public MUTransfigurationFashion GetNextEffectByNowEffectID(int goodsId, int effectId)
	{
		List<MUTransfigurationFashion> transfigurationFashionByGoodsId = this.GetAllTransfigurationFashion().GetTransfigurationFashionByGoodsId(goodsId);
		if (transfigurationFashionByGoodsId[transfigurationFashionByGoodsId.Count - 1].Effect == effectId)
		{
			return null;
		}
		for (int i = transfigurationFashionByGoodsId.Count - 1; i > 0; i--)
		{
			if (transfigurationFashionByGoodsId[i - 1].Effect == effectId)
			{
				return transfigurationFashionByGoodsId[i];
			}
		}
		return null;
	}

	public MUTransfigurationFashionEffectAll GetAllTransfigurationFashionEffect()
	{
		if (this.m_allTransfigurationFashionEffect == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/TransfigurationFashionEffect.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/TransfigurationFashionEffect.xml 出现错误"
				});
				return null;
			}
			this.m_allTransfigurationFashionEffect = new MUTransfigurationFashionEffectAll(gameResXml);
		}
		return this.m_allTransfigurationFashionEffect;
	}

	public MUTransfigurationFashionEffect GetFashionEffectByID(int id)
	{
		return this.GetAllTransfigurationFashionEffect().GetTransfigurationFashionEffectByID(id);
	}

	public float[] GetDecorationHeights(int fashionGoodsId)
	{
		if (this.m_decorationHeights == null)
		{
			this.m_decorationHeights = new Dictionary<int, float[]>();
			float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("TransfigurationDecorations", ',');
			this.m_decorationHeights[0] = systemParamFloatArrayByName;
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("TransfigurationDecorations", '|');
			if (systemParamStringArrayByName != null && 0 < systemParamStringArrayByName.Length)
			{
				for (int i = 0; i < systemParamStringArrayByName.Length; i++)
				{
					if (!string.IsNullOrEmpty(systemParamStringArrayByName[i]))
					{
						string[] array = systemParamStringArrayByName[i].Split(new char[]
						{
							','
						});
						if (array.Length == 4)
						{
							float num = 0f;
							float num2 = 0f;
							float num3 = 0f;
							float.TryParse(array[1], ref num);
							float.TryParse(array[2], ref num2);
							float.TryParse(array[3], ref num3);
							this.m_decorationHeights[array[0].SafeToInt32(0)] = new float[]
							{
								num,
								num2,
								num3
							};
						}
					}
				}
			}
		}
		if (this.m_decorationHeights.ContainsKey(fashionGoodsId))
		{
			return this.m_decorationHeights[fashionGoodsId];
		}
		return this.m_decorationHeights[0];
	}

	public ChangeableRulePart.RuleXml GetShenHunHelpData()
	{
		if (this.m_shenHunHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/TransfigurationIntro.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/TransfigurationIntro.xml 出现错误"
				});
				return null;
			}
			this.m_shenHunHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return this.m_shenHunHelpData;
	}

	public void ClearXMLData(byte clearType)
	{
		this.m_allTransfigurationLevel = null;
		this.m_allTransfigurationFashion = null;
		this.m_allTransfigurationFashionEffect = null;
		this.m_decorationHeights.Clear();
		this.m_decorationHeights = null;
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	private MUTransfigurationLevelAll m_allTransfigurationLevel;

	private MUTransfigurationFashionAll m_allTransfigurationFashion;

	private MUTransfigurationFashionEffectAll m_allTransfigurationFashionEffect;

	private Dictionary<int, float[]> m_decorationHeights;

	private ChangeableRulePart.RuleXml m_shenHunHelpData;
}
