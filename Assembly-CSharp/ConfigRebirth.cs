using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class ConfigRebirth : IConfigbase<ConfigRebirth>, ConfigBase
{
	public ConfigRebirth()
	{
		this.XmlClearType = ClearType.ClearOnChangeSceneAndOnLondConfigNoDispose;
		ConfigManager.AddConfig(this);
	}

	public ClearType XmlClearType { get; set; }

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		if (clearType == 1)
		{
			if (this.mRebornStageXML != null)
			{
				this.mRebornStageXML.ClearData();
				this.mRebornStageXML = null;
			}
			if (this.mRebornLevelXML != null)
			{
				this.mRebornLevelXML.ClearData();
				this.mRebornLevelXML = null;
			}
			if (this.mRebornLianZhanXML != null)
			{
				this.mRebornLianZhanXML.ClearData();
				this.mRebornLianZhanXML = null;
			}
			if (0 < this.mRebornLianZhan.Count)
			{
				this.mRebornLianZhan.Clear();
			}
			if (this.mRebornIntroXML != null)
			{
				this.mRebornIntroXML = null;
			}
			if (this.mRebornIntroBossXML != null)
			{
				this.mRebornIntroBossXML = null;
			}
			if (this.mRebornPerBossXML != null)
			{
				this.mRebornPerBossXML = null;
			}
		}
		else
		{
			this.mCanUseLanShaMapCode = null;
		}
	}

	private void InitmRebornBossAward()
	{
		if (this.mRebornBossAwardXML == null)
		{
			this.mRebornBossAwardXML = new RebornBossAwardXML();
		}
	}

	private void InitRebornLianZhan()
	{
		if (this.mRebornLianZhanXML == null)
		{
			this.mRebornLianZhanXML = new RebornLianZhanXML();
		}
	}

	private void initRebornLevelXML()
	{
		if (this.mRebornLevelXML == null)
		{
			this.mRebornLevelXML = new RebornLevelXML();
		}
	}

	private void InitRebornStageXML()
	{
		if (this.mRebornStageXML == null)
		{
			this.mRebornStageXML = new RebornStageXML();
		}
	}

	private void InitRebornBossXML()
	{
		if (this.mRebornBossXML == null)
		{
			this.mRebornBossXML = new RebornBossXML();
		}
	}

	private void InitRebornPerBossXML()
	{
		if (this.mRebornPerBossXML == null)
		{
			this.mRebornPerBossXML = new RebornPerBossXML();
		}
	}

	public RebornStageVO GetRebornStageVOByID(int ID)
	{
		this.InitRebornStageXML();
		return this.mRebornStageXML.GetItemByID(ID);
	}

	public List<GoodsData> GetRebornStageAwardGoodsByID(int ID)
	{
		this.InitRebornStageXML();
		return this.mRebornStageXML.GetAwardGoodsByID(ID);
	}

	public List<GoodsData> GetRebornStageModalShowGoodsDataListByID(int ID)
	{
		this.InitRebornStageXML();
		return this.mRebornStageXML.GetModalShowGoodsDataListByID(ID);
	}

	public RebornLevelVO GetRebornLevelVOByID(int ID)
	{
		this.initRebornLevelXML();
		return this.mRebornLevelXML.GetItemByID(ID);
	}

	public RebornLianZhanVO GetRebornLianZhanVObyID(int ID)
	{
		this.InitRebornLianZhan();
		return this.mRebornLianZhanXML.GetItemByID(ID);
	}

	public RebornBossVO GetRebornBossVOByID(int ID)
	{
		this.InitRebornBossXML();
		return this.mRebornBossXML.GetItemByID(ID);
	}

	public RebornBossVO GetRebornBossVOByMonsterID(int ID)
	{
		this.InitRebornBossXML();
		return this.mRebornBossXML.GetItemByMonsterID(ID);
	}

	public RebornBossAwardVO GetRebornBossAwardXMLByID(int ID)
	{
		this.InitmRebornBossAward();
		return this.mRebornBossAwardXML.GetItemByID(ID);
	}

	public List<RebornBossAwardVO> GetAwardsByListMonsterID(int ID, bool ChackType = false)
	{
		this.InitmRebornBossAward();
		return this.mRebornBossAwardXML.GetAwardsByMonsterID(ID, ChackType);
	}

	public RebornBossAwardVO GetRebornBossAwardVOLastAttAward(int ID)
	{
		this.InitmRebornBossAward();
		return this.mRebornBossAwardXML.GetRebornBossAwardVOLastAttAward(ID);
	}

	public int GetLianZhandelaySecs(int LianZhanMuber)
	{
		if (0 >= this.mRebornLianZhan.Count)
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("RebornLianZhan", true);
			if (!string.IsNullOrEmpty(systemParamByName))
			{
				string[] array = systemParamByName.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						int num = array2[0].SafeToInt32(0);
						if (this.mRebornLianZhan.ContainsKey(num))
						{
							this.mRebornLianZhan[num] = array2[1].SafeToInt32(0);
						}
						else
						{
							this.mRebornLianZhan.Add(num, array2[1].SafeToInt32(0));
						}
					}
				}
			}
		}
		foreach (KeyValuePair<int, int> keyValuePair in this.mRebornLianZhan)
		{
			if (LianZhanMuber <= keyValuePair.Key)
			{
				Dictionary<int, int>.Enumerator enumerator;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value * 1000;
			}
		}
		return 0;
	}

	public bool GetLianShaCanUseByMap(int mapCode)
	{
		if (this.mCanUseLanShaMapCode == null)
		{
			this.mCanUseLanShaMapCode = ConfigSystemParam.GetSystemParamIntArrayByName("RebornLianZhanMap", '|');
		}
		if (this.mCanUseLanShaMapCode != null)
		{
			int i = 0;
			int num = this.mCanUseLanShaMapCode.Length;
			while (i < num)
			{
				if (this.mCanUseLanShaMapCode[i] == mapCode)
				{
					return true;
				}
				i++;
			}
		}
		return false;
	}

	public BetterList<RebornBossVO> GetRebirthBossItems()
	{
		this.InitRebornBossXML();
		return this.mRebornBossXML.GetAllItem();
	}

	public RebornPerBossVO GetRebornPerBossVOByID(int ID)
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetItemByID(ID);
	}

	public RebornPerBossVO GetRebornPerBossVOByMonsterID(int MonsterID)
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetItemByMonsterID(MonsterID);
	}

	public BetterList<RebornPerBossVO> GetRebornPerBossItems()
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetAllItem();
	}

	public BetterList<GoodsData> GetRebornPerBossAwardGoodsDatas(int MonsterID)
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetAwardGoodsDatas(MonsterID);
	}

	public int GetRebornPerBossMoppingConditions(int MonsterID)
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetMoppingConditions(MonsterID);
	}

	public int GetRebornPerBossFreeChallgeNum(int MonsterID)
	{
		this.InitRebornPerBossXML();
		RebornPerBossVO itemByMonsterID = this.mRebornPerBossXML.GetItemByMonsterID(MonsterID);
		if (itemByMonsterID != null)
		{
			return itemByMonsterID.FreeChallengeNum;
		}
		return 0;
	}

	public List<GoodsData> GetRebornPerBossChallengeReward(int MonsterID, int num)
	{
		this.InitRebornPerBossXML();
		List<GoodsData> list = new List<GoodsData>();
		RebornPerBossVO itemByMonsterID = this.mRebornPerBossXML.GetItemByMonsterID(MonsterID);
		if (itemByMonsterID != null && !string.IsNullOrEmpty(itemByMonsterID.ChallengeReward))
		{
			string[] array = itemByMonsterID.ChallengeReward.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					';'
				});
				if (num.ToString() == array2[0])
				{
					GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array2[1], 0);
					list.Add(goodsDataByStr);
				}
			}
		}
		return list;
	}

	public int GetRebornPerBossPayChallege(int MonsterID, int Num)
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetPayChallegeNum(MonsterID, Num);
	}

	public int GetRebornPerBossDayMaXEnterNum(int MonsterID)
	{
		this.InitRebornPerBossXML();
		return this.mRebornPerBossXML.GetDayMaXEnterNum(MonsterID);
	}

	public int[] GetRoleRebirthNeedZhuanShengByRoleRebirthLevel(int RebirthCount)
	{
		this.InitRebornStageXML();
		return this.mRebornStageXML.GetRoleRebirthNeedZhuanShengByRoleRebirthLevel(RebirthCount);
	}

	public int GetMaxLevel()
	{
		this.initRebornLevelXML();
		return this.mRebornLevelXML.GetMaxLevel();
	}

	public int GetMaxRebornCount()
	{
		this.InitRebornStageXML();
		return this.mRebornStageXML.GeiMaxStage();
	}

	private void initRebornIntroXML()
	{
		if (this.mRebornIntroXML == null)
		{
			this.mRebornIntroXML = new RebornIntroXML();
		}
	}

	public string GetReborthHelpInf()
	{
		this.initRebornIntroXML();
		return this.mRebornIntroXML.GetHelpContent();
	}

	public string GetReborthTitle()
	{
		this.initRebornIntroXML();
		return this.mRebornIntroXML.GetTitle();
	}

	public bool GetReborthTitleBold()
	{
		this.initRebornIntroXML();
		return this.mRebornIntroXML.GetItemBoldByID(1);
	}

	private void initRebornIntroBossXML()
	{
		if (this.mRebornIntroBossXML == null)
		{
			this.mRebornIntroBossXML = new RebornIntroBossXML();
		}
	}

	public string GetRebornIntroBossHelpInf()
	{
		this.initRebornIntroBossXML();
		return this.mRebornIntroBossXML.GetHelpContent();
	}

	public string GetRebornIntroBossTitle()
	{
		this.initRebornIntroBossXML();
		return this.mRebornIntroBossXML.GetTitle();
	}

	public bool GetRebornIntroBossTitleBold()
	{
		this.initRebornIntroBossXML();
		return this.mRebornIntroBossXML.GetItemBoldByID(1);
	}

	private RebornStageXML mRebornStageXML;

	private RebornLevelXML mRebornLevelXML;

	private RebornLianZhanXML mRebornLianZhanXML;

	private RebornBossXML mRebornBossXML;

	private RebornPerBossXML mRebornPerBossXML;

	private RebornBossAwardXML mRebornBossAwardXML;

	private Dictionary<int, int> mRebornLianZhan = new Dictionary<int, int>();

	private int[] mCanUseLanShaMapCode;

	private RebornIntroXML mRebornIntroXML;

	private RebornIntroBossXML mRebornIntroBossXML;
}
