using System;
using System.Collections.Generic;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Logic;
using ProtoBuf;
using Server.Tools;

namespace Server.Data
{
	[ProtoContract]
	public class PlayerJingJiData
	{
		public PlayerJingJiData()
		{
			this.rankingData = new PlayerJingJiRankingData(this);
		}

		public void convertString()
		{
			this.stringBaseProps = this.convertBasePropsToString(this.baseProps);
			this.stringExtProps = this.convertExtPropsToString(this.extProps);
			this.stringEquipDatas = this.convertEquipDatasToString(this.equipDatas);
			this.stringSkillDatas = this.convertSkillDatasToString(this.skillDatas);
			this.stringWingData = this.convertWingDataToString(this.wingData);
			this.stringShenShiEuipSkill = this.convertSkillEquipDataToString(this.shenShiEquipData);
			this.stringPassiveEffect = this.convertPassiveEffectDataToString(this.PassiveEffectList);
		}

		public void convertObject()
		{
			if (this.version == JingJiChangConstants.Current_Data_Version)
			{
				this.baseProps = this.convertStringToBaseProps(this.stringBaseProps);
				this.extProps = this.convertStringToExtProps(this.stringExtProps);
				this.equipDatas = this.convertStringToEquipDatas(this.stringEquipDatas);
				this.skillDatas = this.convertStringToSkillDatas(this.stringSkillDatas);
				this.wingData = this.convertStringToWingData(this.stringWingData);
				this.shenShiEquipData = this.convertStringToSkillEquipData(this.stringShenShiEuipSkill);
				this.PassiveEffectList = this.convertStringToPassiveEffectData(this.stringPassiveEffect);
			}
		}

		private string convertBasePropsToString(double[] baseProps)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < baseProps.Length; i++)
			{
				if (i == baseProps.Length - 1)
				{
					stringBuilder.Append(Convert.ToString(baseProps[i]));
				}
				else
				{
					stringBuilder.Append(Convert.ToString(baseProps[i]));
					stringBuilder.Append(',');
				}
			}
			return (stringBuilder.Length != 0) ? stringBuilder.ToString() : "";
		}

		private string convertExtPropsToString(double[] extProps)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < extProps.Length; i++)
			{
				if (i == extProps.Length - 1)
				{
					stringBuilder.Append(Convert.ToString(extProps[i]));
				}
				else
				{
					stringBuilder.Append(Convert.ToString(extProps[i]));
					stringBuilder.Append(',');
				}
			}
			return (stringBuilder.Length != 0) ? stringBuilder.ToString() : "";
		}

		private string convertEquipDatasToString(List<PlayerJingJiEquipData> equipDatas)
		{
			string result;
			if (equipDatas == null)
			{
				result = "";
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < equipDatas.Count; i++)
				{
					if (i == equipDatas.Count - 1)
					{
						stringBuilder.Append(equipDatas[i].getStringValue());
					}
					else
					{
						stringBuilder.Append(equipDatas[i].getStringValue());
						stringBuilder.Append('|');
					}
				}
				result = ((stringBuilder.Length != 0) ? stringBuilder.ToString() : "");
			}
			return result;
		}

		private string convertSkillDatasToString(List<PlayerJingJiSkillData> skillDatas)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < skillDatas.Count; i++)
			{
				if (i == skillDatas.Count - 1)
				{
					stringBuilder.Append(skillDatas[i].getStringValue());
				}
				else
				{
					stringBuilder.Append(skillDatas[i].getStringValue());
					stringBuilder.Append('|');
				}
			}
			return (stringBuilder.Length != 0) ? stringBuilder.ToString() : "";
		}

		private string convertWingDataToString(WingData wingData)
		{
			string result;
			if (wingData != null)
			{
				byte[] inArray = DataHelper.ObjectToBytes<WingData>(wingData);
				result = Convert.ToBase64String(inArray);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private string convertSkillEquipDataToString(SkillEquipData shenShiEquipData)
		{
			string result;
			if (shenShiEquipData != null)
			{
				byte[] inArray = DataHelper.ObjectToBytes<SkillEquipData>(shenShiEquipData);
				result = Convert.ToBase64String(inArray);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private string convertPassiveEffectDataToString(List<int> passiveEffectList)
		{
			string result;
			if (this.shenShiEquipData != null)
			{
				byte[] inArray = DataHelper.ObjectToBytes<List<int>>(passiveEffectList);
				result = Convert.ToBase64String(inArray);
			}
			else
			{
				result = string.Empty;
			}
			return result;
		}

		private double[] convertStringToBaseProps(string value)
		{
			string[] array = value.Split(new char[]
			{
				','
			});
			double[] array2 = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Convert.ToDouble(array[i]);
			}
			return array2;
		}

		private double[] convertStringToExtProps(string value)
		{
			string[] array = value.Split(new char[]
			{
				','
			});
			double[] array2 = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Convert.ToDouble(array[i]);
			}
			return array2;
		}

		private List<PlayerJingJiEquipData> convertStringToEquipDatas(string value)
		{
			List<PlayerJingJiEquipData> list = new List<PlayerJingJiEquipData>();
			List<PlayerJingJiEquipData> result;
			if (value == null || value.Equals(""))
			{
				result = list;
			}
			else
			{
				string[] array = value.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					PlayerJingJiEquipData playerJingJiEquipData = PlayerJingJiEquipData.createPlayerJingJiEquipData(array[i]);
					if (null != playerJingJiEquipData)
					{
						list.Add(playerJingJiEquipData);
					}
				}
				result = list;
			}
			return result;
		}

		private List<PlayerJingJiSkillData> convertStringToSkillDatas(string value)
		{
			List<PlayerJingJiSkillData> list = new List<PlayerJingJiSkillData>();
			List<PlayerJingJiSkillData> result;
			if (value == null || value.Equals(""))
			{
				result = list;
			}
			else
			{
				string[] array = value.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					PlayerJingJiSkillData playerJingJiSkillData = PlayerJingJiSkillData.createPlayerJingJiSkillData(array[i]);
					if (null != playerJingJiSkillData)
					{
						list.Add(playerJingJiSkillData);
					}
				}
				result = list;
			}
			return result;
		}

		private WingData convertStringToWingData(string value)
		{
			WingData result;
			if (!string.IsNullOrEmpty(value))
			{
				byte[] array = Convert.FromBase64String(value);
				result = DataHelper.BytesToObject<WingData>(array, 0, array.Length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private SkillEquipData convertStringToSkillEquipData(string value)
		{
			SkillEquipData result;
			if (!string.IsNullOrEmpty(value))
			{
				byte[] array = Convert.FromBase64String(value);
				result = DataHelper.BytesToObject<SkillEquipData>(array, 0, array.Length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		private List<int> convertStringToPassiveEffectData(string value)
		{
			List<int> result;
			if (!string.IsNullOrEmpty(value))
			{
				byte[] array = Convert.FromBase64String(value);
				result = DataHelper.BytesToObject<List<int>>(array, 0, array.Length);
			}
			else
			{
				result = null;
			}
			return result;
		}

		public PlayerJingJiMiniData getPlayerJingJiMiniData()
		{
			this.miniData.roleId = this.roleId;
			this.miniData.roleName = this.roleName;
			this.miniData.ranking = this.ranking;
			this.miniData.occupationId = this.occupationId;
			this.miniData.combatForce = this.combatForce;
			return this.miniData;
		}

		public PlayerJingJiRankingData getPlayerJingJiRankingData()
		{
			this.rankingData.ranking = this.ranking;
			return this.rankingData;
		}

		public bool isOnline = false;

		[DBMapping(ColumnName = "roleId"), ProtoMember(1)]
		public int roleId;

		[DBMapping(ColumnName = "roleName"), ProtoMember(2)]
		public string roleName;

		[ProtoMember(3), DBMapping(ColumnName = "level")]
		public int level;

		[DBMapping(ColumnName = "changeLiveCount"), ProtoMember(4)]
		public int changeLiveCount;

		[DBMapping(ColumnName = "occupationId"), ProtoMember(5)]
		public int occupationId;

		[ProtoMember(6), DBMapping(ColumnName = "winCount")]
		public int winCount = 0;

		[ProtoMember(7), DBMapping(ColumnName = "ranking")]
		public int ranking = -1;

		[DBMapping(ColumnName = "nextRewardTime"), ProtoMember(8)]
		public long nextRewardTime;

		[DBMapping(ColumnName = "nextChallengeTime"), ProtoMember(9)]
		public long nextChallengeTime;

		[ProtoMember(10)]
		public double[] baseProps;

		[DBMapping(ColumnName = "baseProps")]
		public string stringBaseProps;

		[ProtoMember(11)]
		public double[] extProps;

		[DBMapping(ColumnName = "extProps")]
		public string stringExtProps;

		[ProtoMember(12)]
		public List<PlayerJingJiEquipData> equipDatas;

		[DBMapping(ColumnName = "equipDatas")]
		public string stringEquipDatas;

		[ProtoMember(13)]
		public List<PlayerJingJiSkillData> skillDatas;

		[DBMapping(ColumnName = "skillDatas")]
		public string stringSkillDatas;

		[DBMapping(ColumnName = "CombatForce"), ProtoMember(14)]
		public int combatForce = 0;

		[DBMapping(ColumnName = "version")]
		public int version;

		[DBMapping(ColumnName = "sex"), ProtoMember(15)]
		public int sex;

		[DBMapping(ColumnName = "name"), ProtoMember(16)]
		public string name;

		[DBMapping(ColumnName = "zoneId"), ProtoMember(17)]
		public int zoneId;

		[DBMapping(ColumnName = "maxwincnt"), ProtoMember(18)]
		public int MaxWinCnt = 0;

		[DBMapping(ColumnName = "wingData")]
		public string stringWingData;

		[ProtoMember(19)]
		public WingData wingData;

		[DBMapping(ColumnName = "settingFlags"), ProtoMember(20)]
		public long settingFlags;

		[ProtoMember(21)]
		public int AdmiredCount;

		[ProtoMember(28)]
		public SkillEquipData shenShiEquipData;

		[DBMapping(ColumnName = "shenshiequip")]
		public string stringShenShiEuipSkill;

		[ProtoMember(29)]
		public List<int> PassiveEffectList;

		[DBMapping(ColumnName = "passiveEffect")]
		public string stringPassiveEffect;

		[ProtoMember(32), DBMapping(ColumnName = "suboccupation")]
		public int SubOccupation;

		private PlayerJingJiMiniData miniData = new PlayerJingJiMiniData();

		private PlayerJingJiRankingData rankingData;
	}
}
