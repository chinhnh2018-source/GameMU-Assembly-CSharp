using System;
using System.Collections.Generic;
using XMLCreater;

public class ConfigHuiGuiHuoDong : IConfigbase<ConfigHuiGuiHuoDong>, ConfigBase
{
	public ConfigHuiGuiHuoDong()
	{
		this.XmlClearType = ClearType.ClearOnLondConfig;
		ConfigManager.AddConfig(this);
	}

	public MUHuiGuiHuoDongAll GetAllHuiGuiHuoDong()
	{
		if (this.m_allHuiGuiHuoDong == null)
		{
			this.m_allHuiGuiHuoDong = XMLHelper.InstanceConfig<MUHuiGuiHuoDongAll>("Config/HuiGuiHuoDong.xml");
		}
		return this.m_allHuiGuiHuoDong;
	}

	public MUHuiGuiHuoDong GetHuiGuiHuoDongByID(int id)
	{
		return this.GetAllHuiGuiHuoDong().GetHuiGuiHuoDongByID(id);
	}

	public MUHuiGuiChongZhiGiftAll GetAllHuiGuiChongZhiGift()
	{
		if (this.m_allHuiGuiChongZhiGift == null)
		{
			this.m_allHuiGuiChongZhiGift = XMLHelper.InstanceConfig<MUHuiGuiChongZhiGiftAll>("Config/HuiGuiChongZhiGift.xml");
		}
		return this.m_allHuiGuiChongZhiGift;
	}

	public List<MUHuiGuiChongZhiGift> GetChongZhiGiftByLevel(int huoDongLevel)
	{
		return this.GetAllHuiGuiChongZhiGift().GetChongZhiGiftByLevel(huoDongLevel);
	}

	public MUHuiGuiDayZhiGouAll GetAllHuiGuiDayZhiGou()
	{
		if (this.m_allHuiGuiDayZhiGou == null)
		{
			this.m_allHuiGuiDayZhiGou = XMLHelper.InstanceConfig<MUHuiGuiDayZhiGouAll>("Config/HuiGuiDayZhiGou.xml");
		}
		return this.m_allHuiGuiDayZhiGou;
	}

	public List<MUHuiGuiDayZhiGou> GetZhiGouByLevelAndDay(int level, int day)
	{
		return this.GetAllHuiGuiDayZhiGou().GetZhiGouByLevelAndDay(level, day);
	}

	public List<MUHuiGuiDayZhiGou> GetZhiGouByLevelAndDay(int level, int day, int chongZhiNum)
	{
		return this.GetAllHuiGuiDayZhiGou().GetZhiGouByLevelAndDay(level, day, chongZhiNum);
	}

	public MUHuiGuiLoginNumGiftAll GetAllHuiGuiLoginNumGift()
	{
		if (this.m_allHuiGuiLoginNumGift == null)
		{
			this.m_allHuiGuiLoginNumGift = XMLHelper.InstanceConfig<MUHuiGuiLoginNumGiftAll>("Config/HuiGuiLoginNumGift.xml");
		}
		return this.m_allHuiGuiLoginNumGift;
	}

	public MUHuiGuiLoginNumGift GetLoginNumGiftByLevelAndDay(int level, int day)
	{
		return this.GetAllHuiGuiLoginNumGift().GetLoginNumGiftByLevelAndDay(level, day);
	}

	public int GetTotalLoginDayNum()
	{
		return this.GetAllHuiGuiLoginNumGift().DayNum;
	}

	public MUHuiGuiStoreAll GetAllHuiGuiStore()
	{
		if (this.m_allHuiGuiStore == null)
		{
			this.m_allHuiGuiStore = XMLHelper.InstanceConfig<MUHuiGuiStoreAll>("Config/HuiGuiStore.xml");
		}
		return this.m_allHuiGuiStore;
	}

	public List<MUHuiGuiStore> GetStoreByLevelAndDay(int level, int day)
	{
		return this.GetAllHuiGuiStore().GetStoreByLevelAndDay(level, day);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.m_allHuiGuiChongZhiGift = null;
		this.m_allHuiGuiDayZhiGou = null;
		this.m_allHuiGuiHuoDong = null;
		this.m_allHuiGuiLoginNumGift = null;
		this.m_allHuiGuiStore = null;
	}

	private const string HuiGuiChongZhiGiftPath = "Config/HuiGuiChongZhiGift.xml";

	private const string HuiGuiDayZhiGouPath = "Config/HuiGuiDayZhiGou.xml";

	private const string HuiGuiHuoDongPath = "Config/HuiGuiHuoDong.xml";

	private const string HuiGuiLoginNumGiftPath = "Config/HuiGuiLoginNumGift.xml";

	private const string HuiGuiStorePath = "Config/HuiGuiStore.xml";

	private MUHuiGuiHuoDongAll m_allHuiGuiHuoDong;

	private MUHuiGuiChongZhiGiftAll m_allHuiGuiChongZhiGift;

	private MUHuiGuiDayZhiGouAll m_allHuiGuiDayZhiGou;

	private MUHuiGuiLoginNumGiftAll m_allHuiGuiLoginNumGift;

	private MUHuiGuiStoreAll m_allHuiGuiStore;
}
