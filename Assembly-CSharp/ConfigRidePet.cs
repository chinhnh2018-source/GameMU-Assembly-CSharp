using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ConfigRidePet : IConfigbase<ConfigRidePet>, ConfigBase
{
	public ConfigRidePet()
	{
		this.XmlClearType = ClearType.ClearOnChangeSceneAndOnLondConfigNoDispose;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void ClearXMLData(byte clearType)
	{
		if (clearType == 1)
		{
			this.ClearDataChangeScene();
		}
		else
		{
			this.ClearData();
		}
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearDataChangeScene()
	{
		this.m_DicHorseIntro.Clear();
		this.m_DicHorseEquipAddition.Clear();
		this.ClearHorseSuitData();
		this.ClearHorseArrayAdditionData();
	}

	public void ClearData()
	{
		this.ClearHorsePokedexXml();
		this.m_DicHorseLevelUp.Clear();
		this.ClearHorseAdvancedVOXml();
		this.m_DicHorseSuperiorType.Clear();
		this.m_DicHorseSuperiorDrop.Clear();
	}

	public HorseFashionXml GetHorseFashionXmlInstance()
	{
		if (this.mHorseFashionXml == null)
		{
			this.mHorseFashionXml = new HorseFashionXml();
		}
		return this.mHorseFashionXml;
	}

	public void ClearHorseFashionXmlInstance()
	{
		if (this.mHorseFashionXml != null)
		{
			this.mHorseFashionXml.ClearData();
			this.mHorseFashionXml = null;
		}
	}

	private void AddXmlHorsePokedex()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorsePokedex.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorsePokedex");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorsePokedexVO horsePokedexVO = new HorsePokedexVO();
			horsePokedexVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horsePokedexVO.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			horsePokedexVO.MOD = Global.GetXElementAttributeStr(xelementList[i], "MOD");
			horsePokedexVO.HorseGoods = Global.GetXElementAttributeInt(xelementList[i], "HorseGoods");
			horsePokedexVO.PokedexAttribute = Global.GetXElementAttributeStr(xelementList[i], "PokedexAttribute");
			horsePokedexVO.SuperiorAttributeID = Global.GetXElementAttributeInt(xelementList[i], "SuperiorAttributeID");
			horsePokedexVO.HorseSpeed = Global.GetXElementAttributeFloat(xelementList[i], "HorseSpeed");
			horsePokedexVO.SkillID = Global.GetXElementAttributeInt(xelementList[i], "SkillID");
			if (!this.m_DicHorsePokedex.ContainsKey(horsePokedexVO.ID))
			{
				this.m_DicHorsePokedex.Add(horsePokedexVO.ID, horsePokedexVO);
			}
		}
	}

	private void AddXmlHorseLevelUp()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseLevelUp.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseLevelUp");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseLevelUpVO horseLevelUpVO = new HorseLevelUpVO();
			horseLevelUpVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseLevelUpVO.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			horseLevelUpVO.Exp = Global.GetXElementAttributeInt(xelementList[i], "Exp");
			horseLevelUpVO.AdvancedEffect = Global.GetXElementAttributeFloat(xelementList[i], "AdvancedEffect");
			if (!this.m_DicHorseLevelUp.ContainsKey(horseLevelUpVO.ID))
			{
				this.m_DicHorseLevelUp.Add(horseLevelUpVO.Level, horseLevelUpVO);
			}
		}
	}

	public void ClearHorseAdvancedVOXml()
	{
		this.m_DicHorseAdvancedVO.Clear();
	}

	private void AddXmlHorseAdvancedVO()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseAdvanced.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseAdvanced");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseAdvancedVO horseAdvancedVO = new HorseAdvancedVO();
			horseAdvancedVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseAdvancedVO.Level = Global.GetXElementAttributeInt(xelementList[i], "Level");
			horseAdvancedVO.NeedGoods = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
			horseAdvancedVO.AdvancedEffect = Global.GetXElementAttributeStr(xelementList[i], "AdvancedEffect");
			horseAdvancedVO.SkillID = Global.GetXElementAttributeInt(xelementList[i], "SkillID");
			horseAdvancedVO.HorseID = Global.GetXElementAttributeInt(xelementList[i], "HorseID");
			horseAdvancedVO.ChangeHunJing = Global.GetXElementAttributeInt(xelementList[i], "ChangeHunJing");
			if (!this.m_DicHorseAdvancedVO.ContainsKey(horseAdvancedVO.ID))
			{
				this.m_DicHorseAdvancedVO.Add(horseAdvancedVO.ID, horseAdvancedVO);
			}
		}
	}

	private void AddXmlHorseSuit()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseSuit.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseSuit");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseSuit horseSuit = new HorseSuit();
			horseSuit.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseSuit.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			horseSuit.HorseID = Global.GetXElementAttributeStr(xelementList[i], "HorseID");
			horseSuit.HorseSuitProps = Global.GetXElementAttributeStr(xelementList[i], "HorseSuitProps");
			if (!this.m_DicHorseSuit.ContainsKey(horseSuit.ID))
			{
				this.m_DicHorseSuit.Add(horseSuit.ID, horseSuit);
			}
		}
	}

	public Dictionary<int, HorseSuit>.Enumerator GetHorseSuitEnumerator()
	{
		if (this.m_DicHorseSuit.Count <= 0)
		{
			this.AddXmlHorseSuit();
		}
		return this.m_DicHorseSuit.GetEnumerator();
	}

	public void ClearHorseSuitData()
	{
		this.m_DicHorseSuit.Clear();
	}

	public Dictionary<int, HorseArrayAdditionVO>.Enumerator GetHorseArrayAdditionEnumerator()
	{
		if (0 >= this.m_DicHorseArrayAddition.Count)
		{
			this.InitXmlHorseArrayAddition();
		}
		return this.m_DicHorseArrayAddition.GetEnumerator();
	}

	public HorseArrayAdditionVO GetHorseArrayAdditionVOByID(int ID)
	{
		if (0 >= this.m_DicHorseArrayAddition.Count)
		{
			this.InitXmlHorseArrayAddition();
		}
		if (this.m_DicHorseArrayAddition.ContainsKey(ID))
		{
			return this.m_DicHorseArrayAddition[ID];
		}
		return null;
	}

	public void ClearHorseArrayAdditionData()
	{
		this.m_DicHorseArrayAddition.Clear();
	}

	private void InitXmlHorseArrayAddition()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseArrayAddition.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseArrayAddition");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseArrayAdditionVO horseArrayAdditionVO = new HorseArrayAdditionVO();
			horseArrayAdditionVO.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseArrayAdditionVO.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			horseArrayAdditionVO.NeedOrderNum = Global.GetXElementAttributeInt(xelementList[i], "NeedOrderNum");
			horseArrayAdditionVO.NeedLevel = Global.GetXElementAttributeInt(xelementList[i], "NeedLevel");
			horseArrayAdditionVO.NeedSuperiorNum = Global.GetXElementAttributeInt(xelementList[i], "NeedSuperiorNum");
			horseArrayAdditionVO.AdditionProps = Global.GetXElementAttributeStr(xelementList[i], "AdditionProps");
			horseArrayAdditionVO.NextID = Global.GetXElementAttributeInt(xelementList[i], "NextID");
			if (!this.m_DicHorseArrayAddition.ContainsKey(horseArrayAdditionVO.ID))
			{
				this.m_DicHorseArrayAddition.Add(horseArrayAdditionVO.ID, horseArrayAdditionVO);
			}
		}
	}

	private void AddXmlHorseSuperiorType()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseSuperiorType.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseSuperiorType");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseSuperiorType horseSuperiorType = new HorseSuperiorType();
			horseSuperiorType.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseSuperiorType.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			horseSuperiorType.Type = Global.GetXElementAttributeStr(xelementList[i], "Type");
			horseSuperiorType.Parameter = Global.GetXElementAttributeStr(xelementList[i], "Parameter");
			if (!this.m_DicHorseSuperiorType.ContainsKey(horseSuperiorType.ID))
			{
				this.m_DicHorseSuperiorType.Add(horseSuperiorType.ID, horseSuperiorType);
			}
		}
	}

	private void AddXmlHorseSuperiorDrop()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseSuperiorDrop.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseSuperiorDrop");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseSuperiorDrop horseSuperiorDrop = new HorseSuperiorDrop();
			horseSuperiorDrop.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseSuperiorDrop.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			horseSuperiorDrop.CommonSuperiorRate = Global.GetXElementAttributeStr(xelementList[i], "CommonSuperiorRate");
			horseSuperiorDrop.CommonSuperiorBank = Global.GetXElementAttributeStr(xelementList[i], "CommonSuperiorBank");
			horseSuperiorDrop.SeniorSuperiorRate = Global.GetXElementAttributeStr(xelementList[i], "SeniorSuperiorRate");
			horseSuperiorDrop.SeniorSuperiorBank = Global.GetXElementAttributeStr(xelementList[i], "SeniorSuperiorBank");
			if (!this.m_DicHorseSuperiorDrop.ContainsKey(horseSuperiorDrop.ID))
			{
				this.m_DicHorseSuperiorDrop.Add(horseSuperiorDrop.ID, horseSuperiorDrop);
			}
		}
	}

	private void AddXmlHorseIntro()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseIntro.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseIntro horseIntro = new HorseIntro();
			horseIntro.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseIntro.Intro = Global.GetXElementAttributeStr(xelementList[i], "Intro");
			horseIntro.Bold = Global.GetXElementAttributeInt(xelementList[i], "Bold");
			if (!this.m_DicHorseIntro.ContainsKey(horseIntro.ID))
			{
				this.m_DicHorseIntro.Add(horseIntro.ID, horseIntro);
			}
		}
	}

	private void AddXmlHorseEquipAddition()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseEquipAddition.xml");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseEquipAddition");
		for (int i = 0; i < xelementList.Count; i++)
		{
			HorseEquipAddition horseEquipAddition = new HorseEquipAddition();
			horseEquipAddition.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			horseEquipAddition.Type = Global.GetXElementAttributeInt(xelementList[i], "Type");
			horseEquipAddition.NextID = Global.GetXElementAttributeInt(xelementList[i], "NextID");
			horseEquipAddition.NeedStrengthenLevel = Global.GetXElementAttributeInt(xelementList[i], "NeedStrengthenLevel");
			horseEquipAddition.NeedAdditionLevel = Global.GetXElementAttributeInt(xelementList[i], "NeedAdditionLevel");
			horseEquipAddition.NeedOrderNum = Global.GetXElementAttributeInt(xelementList[i], "NeedOrderNum");
			horseEquipAddition.AdditionProps = Global.GetXElementAttributeStr(xelementList[i], "AdditionProps");
			if (!this.m_DicHorseEquipAddition.ContainsKey(horseEquipAddition.ID))
			{
				this.m_DicHorseEquipAddition.Add(horseEquipAddition.ID, horseEquipAddition);
			}
		}
	}

	public HorsePokedexVO GetHorsePokedexByHorseID(int HorseID)
	{
		foreach (KeyValuePair<int, HorsePokedexVO> keyValuePair in this.DicHorsePokedex)
		{
			if (HorseID == keyValuePair.Value.HorseGoods)
			{
				Dictionary<int, HorsePokedexVO>.Enumerator enumerator;
				KeyValuePair<int, HorsePokedexVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value;
			}
		}
		return null;
	}

	public Dictionary<int, HorsePokedexVO>.Enumerator GetHorsePokedexEnumerator()
	{
		return this.DicHorsePokedex.GetEnumerator();
	}

	public Dictionary<int, HorseIntro> DicHorseIntro
	{
		get
		{
			if (this.m_DicHorseIntro.Count <= 0)
			{
				this.AddXmlHorseIntro();
			}
			return this.m_DicHorseIntro;
		}
	}

	private Dictionary<int, HorsePokedexVO> DicHorsePokedex
	{
		get
		{
			if (this.m_DicHorsePokedex.Count <= 0)
			{
				this.AddXmlHorsePokedex();
			}
			return this.m_DicHorsePokedex;
		}
	}

	public Dictionary<int, HorseLevelUpVO> DicHorseLevelUp
	{
		get
		{
			if (this.m_DicHorseLevelUp.Count <= 0)
			{
				this.AddXmlHorseLevelUp();
			}
			return this.m_DicHorseLevelUp;
		}
	}

	private Dictionary<int, HorseAdvancedVO> DicHorseAdvanced
	{
		get
		{
			if (this.m_DicHorseAdvancedVO.Count <= 0)
			{
				this.AddXmlHorseAdvancedVO();
			}
			return this.m_DicHorseAdvancedVO;
		}
	}

	public HorseAdvancedVO GetHorseAdvancedVOByID(int HorseID, int Level)
	{
		if (0 >= this.m_DicHorseAdvancedVO.Count)
		{
			this.AddXmlHorseAdvancedVO();
		}
		foreach (KeyValuePair<int, HorseAdvancedVO> keyValuePair in this.DicHorseAdvanced)
		{
			if (HorseID == keyValuePair.Value.HorseID)
			{
				Dictionary<int, HorseAdvancedVO>.Enumerator enumerator;
				KeyValuePair<int, HorseAdvancedVO> keyValuePair2 = enumerator.Current;
				if (Level == keyValuePair2.Value.Level)
				{
					KeyValuePair<int, HorseAdvancedVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value;
				}
			}
		}
		return null;
	}

	public Dictionary<int, HorseSuperiorType> DicHorseSuperiorType
	{
		get
		{
			if (this.m_DicHorseSuperiorType.Count <= 0)
			{
				this.AddXmlHorseSuperiorType();
			}
			return this.m_DicHorseSuperiorType;
		}
	}

	public Dictionary<int, HorseSuperiorDrop> DicHorseSuperiorDrop
	{
		get
		{
			if (this.m_DicHorseSuperiorDrop.Count <= 0)
			{
				this.AddXmlHorseSuperiorDrop();
			}
			return this.m_DicHorseSuperiorDrop;
		}
	}

	public int MaxStar
	{
		get
		{
			if (0 >= this.m_MaxStar)
			{
				if (0 >= this.m_DicHorseAdvancedVO.Count)
				{
					this.AddXmlHorseAdvancedVO();
				}
				foreach (KeyValuePair<int, HorseAdvancedVO> keyValuePair in this.m_DicHorseAdvancedVO)
				{
					if (keyValuePair.Value.Level > this.m_MaxStar)
					{
						Dictionary<int, HorseAdvancedVO>.Enumerator enumerator;
						KeyValuePair<int, HorseAdvancedVO> keyValuePair2 = enumerator.Current;
						this.m_MaxStar = keyValuePair2.Value.Level;
					}
				}
			}
			return this.m_MaxStar;
		}
	}

	public Dictionary<int, HorseEquipAddition> DicHorseEquipAddition
	{
		get
		{
			if (this.m_DicHorseEquipAddition.Count <= 0)
			{
				this.AddXmlHorseEquipAddition();
			}
			return this.m_DicHorseEquipAddition;
		}
	}

	public void ClearHorsePokedexXml()
	{
		this.m_DicHorsePokedex.Clear();
	}

	private const string HORSEPOKEDEX_PATH = "Config/HorsePokedex.xml";

	private const string HORSELEVELUP_PATH = "Config/HorseLevelUp.xml";

	private const string HorseAdvanced_PATH = "Config/HorseAdvanced.xml";

	private const string HORSESUIT_PATH = "Config/HorseSuit.xml";

	private const string HORSEARRAYADDITION_PATH = "Config/HorseArrayAddition.xml";

	private const string HORSEFREERANDOM_PATH = "Config/HorseFreeRandom.xml";

	private const string HORSESUPERIORTYPE_PATH = "Config/HorseSuperiorType.xml";

	private const string HORSESUPERIORDROP_PATH = "Config/HorseSuperiorDrop.xml";

	private const string HORSEINTRO_PATH = "Config/HorseIntro.xml";

	private const string HORSEEQUIPADDITION_PATH = "Config/HorseEquipAddition.xml";

	private Dictionary<int, HorsePokedexVO> m_DicHorsePokedex = new Dictionary<int, HorsePokedexVO>();

	private Dictionary<int, HorseLevelUpVO> m_DicHorseLevelUp = new Dictionary<int, HorseLevelUpVO>();

	private Dictionary<int, HorseAdvancedVO> m_DicHorseAdvancedVO = new Dictionary<int, HorseAdvancedVO>();

	private Dictionary<int, HorseSuit> m_DicHorseSuit = new Dictionary<int, HorseSuit>();

	private Dictionary<int, HorseArrayAdditionVO> m_DicHorseArrayAddition = new Dictionary<int, HorseArrayAdditionVO>();

	private Dictionary<int, HorseSuperiorType> m_DicHorseSuperiorType = new Dictionary<int, HorseSuperiorType>();

	private Dictionary<int, HorseSuperiorDrop> m_DicHorseSuperiorDrop = new Dictionary<int, HorseSuperiorDrop>();

	private Dictionary<int, HorseIntro> m_DicHorseIntro = new Dictionary<int, HorseIntro>();

	private Dictionary<int, HorseEquipAddition> m_DicHorseEquipAddition = new Dictionary<int, HorseEquipAddition>();

	private HorseFashionXml mHorseFashionXml;

	private int m_MaxStar = -1;
}
