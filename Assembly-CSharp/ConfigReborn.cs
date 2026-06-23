using System;
using System.Collections.Generic;
using XMLCreater;

public class ConfigReborn : IConfigbase<ConfigReborn>, ConfigBase
{
	public ConfigReborn()
	{
		this.XmlClearType = ClearType.ClearOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public MURebornEquipAll GetAllRebornEquips()
	{
		if (this.m_allRebornEquip == null)
		{
			this.m_allRebornEquip = XMLHelper.InstanceConfig<MURebornEquipAll>("Config/RebornEquip.xml");
		}
		return this.m_allRebornEquip;
	}

	public string GetRebornEquipsByGoodsIDAndOcc(int GoodsID, int occ)
	{
		MURebornEquipAll allRebornEquips = this.GetAllRebornEquips();
		List<MURebornEquip> rebornEquips = allRebornEquips.RebornEquips;
		if (0 < rebornEquips.Count)
		{
			for (int i = 0; i < rebornEquips.Count; i++)
			{
				if (rebornEquips[i] != null && rebornEquips[i].GoodsID == GoodsID)
				{
					if (occ == 0)
					{
						return rebornEquips[i].ZSMod;
					}
					if (occ == 1)
					{
						return rebornEquips[i].FSMod;
					}
					if (occ == 2)
					{
						return rebornEquips[i].GSMOd;
					}
					if (occ == 3)
					{
						return rebornEquips[i].FMJSMod;
					}
					if (occ == 4)
					{
						return rebornEquips[i].LMJSMod;
					}
					if (occ == 5)
					{
						return rebornEquips[i].ZHSMod;
					}
				}
			}
		}
		return string.Empty;
	}

	public GoodVO GetRebornEquipsByGoodsIDAndOccForShengWuAndShengQi(int GoodsID, int occ)
	{
		string rebornEquipsByGoodsIDAndOcc = this.GetRebornEquipsByGoodsIDAndOcc(GoodsID, occ);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(rebornEquipsByGoodsIDAndOcc.SafeToInt32(0));
		if (goodsXmlNodeByID != null)
		{
			return goodsXmlNodeByID;
		}
		return null;
	}

	public MURebornEquip GetRebornEquipByEquipId(int equipID)
	{
		return this.GetAllRebornEquips().GetRebornEquipByID(equipID);
	}

	public MURebornEquipEvolutionAll GetAllRebornEquipEvolutions()
	{
		if (this.m_allRebornEquipEvolution == null)
		{
			this.m_allRebornEquipEvolution = XMLHelper.InstanceConfig<MURebornEquipEvolutionAll>("Config/RebornEquipEvolution.xml");
		}
		return this.m_allRebornEquipEvolution;
	}

	public MURebornEquipEvolution GetRebornEquipEvolutionByGoodsID(int id)
	{
		return this.GetAllRebornEquipEvolutions().GetRebornEquipEvolutionByGoodsID(id);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.m_allRebornEquip = null;
		this.m_allRebornEquipEvolution = null;
	}

	private const string RebornEquipPath = "Config/RebornEquip.xml";

	private const string RebornEquipEvolutionPath = "Config/RebornEquipEvolution.xml";

	private MURebornEquipAll m_allRebornEquip;

	private MURebornEquipEvolutionAll m_allRebornEquipEvolution;
}
