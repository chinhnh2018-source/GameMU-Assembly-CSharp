using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using XMLCreater;

public class ConfigShiLiMiDong : IConfigbase<ConfigShiLiMiDong>, ConfigBase
{
	public ConfigShiLiMiDong()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		if (this.CompMineWarXmlDic != null)
		{
			this.CompMineWarXmlDic.Clear();
			this.CompMineWarXmlDic = null;
		}
		if (this.CompMineTruckXmlDic != null)
		{
			this.CompMineTruckXmlDic.Clear();
			this.CompMineTruckXmlDic = null;
		}
		this.mMUForceCraftRewardAll = null;
	}

	public List<ChangeableRulePart.RuleVO> GetRule()
	{
		if (0 >= this.mRulelst.Count)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompMineIntro.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "HelpIntro");
				if (0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						if (xelementList[i] != null)
						{
							ChangeableRulePart.RuleVO ruleVO = new ChangeableRulePart.RuleVO(xelementList[i]);
							this.mRulelst.Add(ruleVO);
						}
					}
				}
			}
		}
		return this.mRulelst;
	}

	private void InitCompMineWarXml()
	{
		byte b = 0;
		if (this.CompMineWarXmlDic == null)
		{
			this.CompMineWarXmlDic = new Dictionary<int, CompMineWarVO>();
			b = 1;
		}
		if (b == 0 && 0 >= this.CompMineWarXmlDic.Count)
		{
			b = 1;
		}
		if (b == 1)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompMineWar.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "CompMineWar");
				if (0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						if (xelementList[i] != null)
						{
							CompMineWarVO compMineWarVO = new CompMineWarVO();
							compMineWarVO.CopyForm(xelementList[i]);
							this.CompMineWarXmlDic.Add(compMineWarVO.ID, compMineWarVO);
						}
					}
				}
			}
		}
	}

	public CompMineWarVO GetCompMineWarVOByID(int ID)
	{
		this.InitCompMineWarXml();
		if (this.CompMineWarXmlDic.ContainsKey(ID))
		{
			return this.CompMineWarXmlDic[ID];
		}
		return null;
	}

	public List<int> GetShowGoodsByGoodsID(int ID)
	{
		CompMineWarVO compMineWarVOByID = this.GetCompMineWarVOByID(ID);
		if (compMineWarVOByID != null)
		{
			return compMineWarVOByID.AwardGoods;
		}
		return new List<int>();
	}

	private void InitCompMineTruckXmlDic()
	{
		byte b = 0;
		if (this.CompMineTruckXmlDic == null)
		{
			this.CompMineTruckXmlDic = new Dictionary<int, CompMineTruckVO>();
			b = 1;
		}
		if (b == 0 && 0 >= this.CompMineTruckXmlDic.Count)
		{
			b = 1;
		}
		if (b == 1)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompMineTruck.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "CompMineTruck");
				if (0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						if (xelementList[i] != null)
						{
							CompMineTruckVO compMineTruckVO = new CompMineTruckVO();
							compMineTruckVO.CopyForm(xelementList[i]);
							this.CompMineTruckXmlDic.Add(compMineTruckVO.ID, compMineTruckVO);
						}
					}
				}
			}
		}
	}

	public CompMineTruckVO CompMineTruckVOByID(int ID)
	{
		this.InitCompMineTruckXmlDic();
		if (this.CompMineTruckXmlDic.ContainsKey(ID))
		{
			return this.CompMineTruckXmlDic[ID];
		}
		return null;
	}

	public List<int> GetAddLIfeListByGoodsID(int ID)
	{
		CompMineTruckVO compMineTruckVO = this.CompMineTruckVOByID(ID);
		if (compMineTruckVO != null)
		{
			return compMineTruckVO.AddLIfeList;
		}
		return new List<int>();
	}

	public List<float> GetFinishNumListByGoodsID(int ID)
	{
		CompMineTruckVO compMineTruckVO = this.CompMineTruckVOByID(ID);
		if (compMineTruckVO != null)
		{
			return compMineTruckVO.FinishNumList;
		}
		return new List<float>();
	}

	public List<float> GetBrokenNumListByGoodsID(int ID)
	{
		CompMineTruckVO compMineTruckVO = this.CompMineTruckVOByID(ID);
		if (compMineTruckVO != null)
		{
			return compMineTruckVO.BrokenNumList;
		}
		return new List<float>();
	}

	public int GetCompMineTruckCount()
	{
		this.InitCompMineTruckXmlDic();
		return this.CompMineTruckXmlDic.Count;
	}

	public MUForceCraftRewardAll GetRewardAllAllData()
	{
		if (this.mMUForceCraftRewardAll == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompMineAward.xml");
			if (gameResXml != null)
			{
				this.mMUForceCraftRewardAll = new MUForceCraftRewardAll(gameResXml, "CompMineAward");
			}
		}
		return this.mMUForceCraftRewardAll;
	}

	public const string CompMineWarPath = "Config/CompMineWar.xml";

	public const string CompMineTruckPath = "Config/CompMineTruck.xml";

	public const string CompCompMineAward = "Config/CompMineAward.xml";

	public const string CompMineIntro = "Config/CompMineIntro.xml";

	private List<ChangeableRulePart.RuleVO> mRulelst = new List<ChangeableRulePart.RuleVO>();

	private Dictionary<int, CompMineWarVO> CompMineWarXmlDic;

	private Dictionary<int, CompMineTruckVO> CompMineTruckXmlDic = new Dictionary<int, CompMineTruckVO>();

	private MUForceCraftRewardAll mMUForceCraftRewardAll;
}
