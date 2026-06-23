using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Reborn;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Logic
{
	public class RoleAlgorithm
	{
		static RoleAlgorithm()
		{
			RoleAlgorithm.ExtListArray = new List<ExtPropIndexes>[177];
			RoleAlgorithm.BaseListArray = new List<ExtPropIndexes>[4];
			RoleAlgorithm.CreateNewExtArray(RoleAlgorithm.roleExtPropDic, RoleAlgorithm.ExtListArray);
			RoleAlgorithm.CreateNewBaseArray(RoleAlgorithm.roleExtPropDic, RoleAlgorithm.BaseListArray);
		}

		public static void CreateNewExtArray(Dictionary<ExtPropIndexes, ExtPropItem> oldDic, List<ExtPropIndexes>[] ExtArray)
		{
			foreach (ExtPropIndexes extPropIndexes in oldDic.Keys)
			{
				if (oldDic[extPropIndexes].ExtProp != ExtPropIndexes.Max)
				{
					if (ExtArray[(int)oldDic[extPropIndexes].ExtProp] == null)
					{
						ExtArray[(int)oldDic[extPropIndexes].ExtProp] = new List<ExtPropIndexes>();
					}
					ExtArray[(int)oldDic[extPropIndexes].ExtProp].Add(extPropIndexes);
				}
				if (oldDic[extPropIndexes].ExtPropPercent != ExtPropIndexes.Max)
				{
					if (ExtArray[(int)oldDic[extPropIndexes].ExtPropPercent] == null)
					{
						ExtArray[(int)oldDic[extPropIndexes].ExtPropPercent] = new List<ExtPropIndexes>();
					}
					ExtArray[(int)oldDic[extPropIndexes].ExtPropPercent].Add(extPropIndexes);
				}
			}
		}

		public static void CreateNewBaseArray(Dictionary<ExtPropIndexes, ExtPropItem> oldDic, List<ExtPropIndexes>[] BaseArray)
		{
			foreach (ExtPropIndexes extPropIndexes in oldDic.Keys)
			{
				if (oldDic[extPropIndexes].UnitProp != UnitPropIndexes.Max)
				{
					if (BaseArray[(int)oldDic[extPropIndexes].UnitProp] == null)
					{
						BaseArray[(int)oldDic[extPropIndexes].UnitProp] = new List<ExtPropIndexes>();
					}
					BaseArray[(int)oldDic[extPropIndexes].UnitProp].Add(extPropIndexes);
				}
			}
		}

		public static bool NeedNotifyClient(ExtPropIndexes attribute)
		{
			return RoleAlgorithm.NotifyList.Contains(attribute);
		}

		public static double GetPureExtProp(GameClient client, int extProp)
		{
			double num = 0.0;
			ExtPropItem extPropItem = null;
			RoleAlgorithm.roleExtPropDic.TryGetValue((ExtPropIndexes)extProp, out extPropItem);
			double result;
			if (extPropItem == null)
			{
				result = 0.0;
			}
			else
			{
				int num2 = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[num2][client.ClientData.Level];
				num += roleBasePropItem.arrRoleExtProp[extProp];
				double num3 = 0.0;
				switch (extPropItem.UnitProp)
				{
				case UnitPropIndexes.Strength:
					num3 = RoleAlgorithm.GetStrength(client, true);
					break;
				case UnitPropIndexes.Intelligence:
					num3 = RoleAlgorithm.GetIntelligence(client, true);
					break;
				case UnitPropIndexes.Dexterity:
					num3 = RoleAlgorithm.GetDexterity(client, true);
					break;
				case UnitPropIndexes.Constitution:
					num3 = RoleAlgorithm.GetConstitution(client, true);
					break;
				case UnitPropIndexes.Max:
					num3 = 0.0;
					break;
				}
				if (extPropItem.UnitProp != UnitPropIndexes.Max)
				{
					num3 *= extPropItem.Coefficient[num2];
				}
				num += num3;
				num += client.ClientData.EquipProp.ExtProps[extProp] + client.RoleBuffer.GetExtProp(extProp) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[extProp] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[extProp];
				num += client.AllThingsMultipliedBuffer.GetExtProp(extProp);
				num += client.ClientData.PropsCacheManager.GetExtProp(extProp);
				num += client.RoleMultipliedBuffer.GetExtProp(extProp);
				num += client.RoleOnceBuffer.GetExtProp(extProp);
				if (extPropItem.ExtProp != ExtPropIndexes.Max)
				{
					num += RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtProp);
				}
				if (extPropItem.ExcellentProp != null)
				{
					for (int i = 0; i < extPropItem.ExcellentProp.Length; i++)
					{
						num += client.ClientData.ExcellenceProp[(int)extPropItem.ExcellentProp[i]];
					}
				}
				if (extPropItem.BufferProp != null)
				{
					for (int i = 0; i < extPropItem.BufferProp.Length; i++)
					{
						num += DBRoleBufferManager.ProcessTimeAddProp(client, extPropItem.BufferProp[i]);
					}
				}
				if (extProp == 17)
				{
					num += client.ClientData.LuckProp * 0.01;
				}
				if (extPropItem.ExtPropPercent != ExtPropIndexes.Max)
				{
					double extProp2 = RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtPropPercent);
					num *= 1.0 + extProp2;
				}
				result = num;
			}
			return result;
		}

		public static double GetExtProp(GameClient client, int extProp)
		{
			double num = 0.0;
			ExtPropItem extPropItem = null;
			RoleAlgorithm.roleExtPropDic.TryGetValue((ExtPropIndexes)extProp, out extPropItem);
			double result;
			if (extPropItem == null)
			{
				result = 0.0;
			}
			else
			{
				int num2 = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[num2][client.ClientData.Level];
				num += roleBasePropItem.arrRoleExtProp[extProp];
				double num3 = 0.0;
				switch (extPropItem.UnitProp)
				{
				case UnitPropIndexes.Strength:
					num3 = RoleAlgorithm.GetStrength(client, true);
					break;
				case UnitPropIndexes.Intelligence:
					num3 = RoleAlgorithm.GetIntelligence(client, true);
					break;
				case UnitPropIndexes.Dexterity:
					num3 = RoleAlgorithm.GetDexterity(client, true);
					break;
				case UnitPropIndexes.Constitution:
					num3 = RoleAlgorithm.GetConstitution(client, true);
					break;
				case UnitPropIndexes.Max:
					num3 = 0.0;
					break;
				}
				if (extPropItem.UnitProp != UnitPropIndexes.Max)
				{
					num3 *= extPropItem.Coefficient[num2];
				}
				num += num3;
				num += client.ClientData.EquipProp.ExtProps[extProp] + client.RoleBuffer.GetExtProp(extProp) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[extProp] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[extProp];
				num += client.AllThingsMultipliedBuffer.GetExtProp(extProp);
				num += client.ClientData.PropsCacheManager.GetExtProp(extProp);
				num += client.RoleMultipliedBuffer.GetExtProp(extProp);
				num += client.RoleOnceBuffer.GetExtProp(extProp);
				if (extPropItem.ExtProp != ExtPropIndexes.Max)
				{
					num += RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtProp);
				}
				if (extPropItem.ExcellentProp != null)
				{
					for (int i = 0; i < extPropItem.ExcellentProp.Length; i++)
					{
						num += client.ClientData.ExcellenceProp[(int)extPropItem.ExcellentProp[i]];
					}
				}
				num *= extPropItem.PropCoef;
				if (extPropItem.BufferProp != null)
				{
					for (int i = 0; i < extPropItem.BufferProp.Length; i++)
					{
						num += DBRoleBufferManager.ProcessTimeAddProp(client, extPropItem.BufferProp[i]);
					}
				}
				if (extProp == 17)
				{
					num += client.ClientData.LuckProp;
				}
				if (extPropItem.ExtPropPercent != ExtPropIndexes.Max)
				{
					double extProp2 = RoleAlgorithm.GetExtProp(client, (int)extPropItem.ExtPropPercent);
					num *= 1.0 + extProp2;
				}
				num += client.ClientData.PurePropsCacheManager.GetExtProp(extProp);
				double extProp3 = client.ClientData.PctPropsCacheManager.GetExtProp(extProp);
				if (extProp3 > 0.0)
				{
					num *= extProp3;
				}
				result = num;
			}
			return result;
		}

		public static double GetBaseExtProp(GameClient client, ExtPropItem extPropItem)
		{
			double result = 0.0;
			int index = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
			return result;
		}

		public static double GetStrength(GameClient client, bool bAddBuff = true)
		{
			double num = (double)client.ClientData.PropStrength + client.RoleBuffer.GetBaseProp(0) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[0] + client.ClientData.PropsCacheManager.GetBaseProp(0);
			if (bAddBuff)
			{
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPStrength);
			}
			return num;
		}

		public static double GetIntelligence(GameClient client, bool bAddBuff = true)
		{
			double num = (double)client.ClientData.PropIntelligence + client.RoleBuffer.GetBaseProp(1) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[1] + client.ClientData.PropsCacheManager.GetBaseProp(1);
			if (bAddBuff)
			{
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPIntelligsence);
			}
			return num;
		}

		public static double GetDexterity(GameClient client, bool bAddBuff = true)
		{
			double num = (double)client.ClientData.PropDexterity + client.RoleBuffer.GetBaseProp(2) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[2] + client.ClientData.PropsCacheManager.GetBaseProp(2);
			if (bAddBuff)
			{
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDexterity);
			}
			return num;
		}

		public static double GetConstitution(GameClient client, bool bAddBuff = true)
		{
			double num = (double)client.ClientData.PropConstitution + client.RoleBuffer.GetBaseProp(3) + client.ClientData.RoleStarConstellationProp.StarConstellationFirstProps[3] + client.ClientData.PropsCacheManager.GetBaseProp(3);
			if (bAddBuff)
			{
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPConstitution);
			}
			return num;
		}

		public static double GetMagicSkillIncrease(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(33, delegate
			{
				double num = 0.0;
				int num2 = Global.CalcOriginalOccupationID(client);
				EOccupationType eoccupationType = (EOccupationType)num2;
				if (EOccupationType.EOT_Magician == eoccupationType || EOccupationType.EOT_MagicSword == eoccupationType || EOccupationType.EOT_Summoner == eoccupationType)
				{
					RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[num2][client.ClientData.Level];
					num = RoleAlgorithm.GetStrength(client, true) / 100000.0;
					num += roleBasePropItem.MagicSkillIncreasePercent;
				}
				return num;
			});
		}

		public static double GetPhySkillIncrease(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(31, delegate
			{
				double num = 0.0;
				int num2 = Global.CalcOriginalOccupationID(client);
				EOccupationType eoccupationType = (EOccupationType)num2;
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[num2][client.ClientData.Level];
				if (eoccupationType == EOccupationType.EOT_Warrior || EOccupationType.EOT_Bow == eoccupationType || EOccupationType.EOT_MagicSword == eoccupationType)
				{
					num = RoleAlgorithm.GetIntelligence(client, true) / 100000.0;
				}
				return num + roleBasePropItem.PhySkillIncreasePercent;
			});
		}

		public static double GetAttackSpeed(GameClient client)
		{
			int index = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
			return roleBasePropItem.AttackSpeed;
		}

		public static double GetAttackSpeedServer(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(1, delegate
			{
				int index = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
				return roleBasePropItem.AttackSpeed;
			});
		}

		public static double GetFatalAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(35, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[35] + client.RoleBuffer.GetExtProp(35) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[35] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[35];
				num += client.ClientData.PropsCacheManager.GetExtProp(35);
				num += client.ClientData.ExcellenceProp[0];
				num += client.ClientData.ExcellenceProp[18];
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPFATALATTACK);
				num *= 100.0;
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.FatalAttack);
				return num + client.RoleMultipliedBuffer.GetExtProp(35);
			});
		}

		public static double GetDeFatalAttack(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(52);
			num += client.ClientData.ExcellenceProp[30];
			return num * 100.0;
		}

		public static double GetFatalHurt(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(90);
		}

		public static double GetDeLuckyAttack(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(51);
			num += client.ClientData.ExcellenceProp[29];
			return num * 100.0;
		}

		public static double GetFatalAttack(Monster monster)
		{
			return monster.MonsterInfo.MonsterFatalAttack * 100.0;
		}

		public static double GetDoubleAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(36, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[36] + client.RoleBuffer.GetExtProp(36) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[36] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[36];
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPDOUBLEATTACK);
				num += client.ClientData.ExcellenceProp[23];
				num += client.ClientData.PropsCacheManager.GetExtProp(36);
				num *= 100.0;
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DoubleAttack);
				return num + client.RoleMultipliedBuffer.GetExtProp(36);
			});
		}

		public static double GetDeDoubleAttack(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(53);
			num += client.ClientData.ExcellenceProp[31];
			return num * 100.0;
		}

		public static double GetSavagePercent(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(61);
			num *= 100.0;
			return Math.Max(num, 0.0);
		}

		public static double GetDeSavagePercent(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(64);
			num *= 100.0;
			return Math.Max(num, 0.0);
		}

		public static double GetColdPercent(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(62);
			num *= 100.0;
			return Math.Max(num, 0.0);
		}

		public static double GetDeColdPercent(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(65);
			num *= 100.0;
			return Math.Max(num, 0.0);
		}

		public static double GetRuthlessPercent(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(63);
			num *= 100.0;
			return Math.Max(num, 0.0);
		}

		public static double GetDeRuthlessPercent(GameClient client)
		{
			double num = 0.0;
			num += client.ClientData.PropsCacheManager.GetExtProp(66);
			num *= 100.0;
			return Math.Max(num, 0.0);
		}

		public static double GetDoubleAttack(Monster monster)
		{
			return monster.MonsterInfo.MonsterDoubleAttack * 100.0;
		}

		public static double GetMoveSpeed(GameClient client)
		{
			double result;
			if (client.RoleBuffer.GetExtProp(47) > 0.1)
			{
				result = 0.0;
			}
			else
			{
				result = client.propsCacheModule.GetExtPropsValue(2, delegate
				{
					double num = 1.0;
					num = num * (1.0 + client.ClientData.EquipProp.ExtProps[2]) * (1.0 + client.RoleBuffer.GetExtProp(2));
					num += client.ClientData.PropsCacheManager.GetExtProp(2);
					num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MoveSpeed);
					num += client.RoleMultipliedBuffer.GetExtProp(2);
					if (num < 0.0)
					{
						num = 0.0;
					}
					return num;
				});
			}
			return result;
		}

		public static double GetDamageThornPercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(29, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[29] + client.RoleBuffer.GetExtProp(29) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[29] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[29];
				num += client.AllThingsMultipliedBuffer.GetExtProp(29);
				num += client.ClientData.PropsCacheManager.GetExtProp(29);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DamageThornPercent);
				num += client.RoleMultipliedBuffer.GetExtProp(29);
				num += client.ClientData.ExcellenceProp[11];
				return num + client.ClientData.ExcellenceProp[28];
			});
		}

		public static double GetDamageThornPercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterDamageThornPercent;
		}

		public static double GetDamageThorn(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(30, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[30] + client.RoleBuffer.GetExtProp(30) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[30] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[30];
				num += client.ClientData.PropsCacheManager.GetExtProp(30);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DamageThorn);
				num += client.RoleMultipliedBuffer.GetExtProp(30);
				return Global.GMax(0.0, num);
			});
		}

		public static double GetDamageThorn(Monster monster)
		{
			double monsterDamageThorn = monster.MonsterInfo.MonsterDamageThorn;
			return Global.GMax(0.0, monsterDamageThorn);
		}

		public static double GetStrong(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(0, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[0] + client.RoleBuffer.GetExtProp(0) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[0] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[0];
				num += client.AllThingsMultipliedBuffer.GetExtProp(0);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Strong);
				return num + client.RoleMultipliedBuffer.GetExtProp(0);
			});
		}

		public static double GetStrong(Monster monster)
		{
			return 0.0;
		}

		public static double GetMinADefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 3);
		}

		public static double GetMinADefenseV(Monster monster)
		{
			double num = (double)monster.MonsterInfo.Defense;
			return num * (1.0 + monster.TempPropsBuffer.GetExtProp(42));
		}

		public static double GetMaxADefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 4);
		}

		public static double GetIncreasePhyDefense(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(42, delegate
			{
				double num = DBRoleBufferManager.ProcessAddTempDefense(client);
				num += client.ClientData.PropsCacheManager.GetExtProp(42);
				num += client.ClientData.PropsCacheManager.GetExtProp(92);
				num += client.RoleBuffer.GetExtProp(42);
				num += client.ClientData.EquipProp.ExtProps[92];
				return num + (client.ClientData.ExcellenceProp[13] + client.ClientData.ExcellenceProp[27]);
			});
		}

		public static double GetMaxADefenseV(Monster monster)
		{
			double num = (double)monster.MonsterInfo.Defense;
			return num * (1.0 + monster.TempPropsBuffer.GetExtProp(42));
		}

		public static double GetMinMDefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 5);
		}

		public static double GetIncreaseMagDefense(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(43, delegate
			{
				double num = DBRoleBufferManager.ProcessAddTempDefense(client);
				num += client.ClientData.PropsCacheManager.GetExtProp(43);
				num += client.ClientData.PropsCacheManager.GetExtProp(92);
				num += client.RoleBuffer.GetExtProp(43);
				num += client.ClientData.EquipProp.ExtProps[92];
				return num + (client.ClientData.ExcellenceProp[13] + client.ClientData.ExcellenceProp[27]);
			});
		}

		public static double GetMinMDefenseV(Monster monster)
		{
			double num = (double)monster.MonsterInfo.MDefense;
			return num * (1.0 + monster.TempPropsBuffer.GetExtProp(43));
		}

		public static double GetMaxMDefenseV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 6);
		}

		public static double GetMaxMDefenseV(Monster monster)
		{
			double num = (double)monster.MonsterInfo.MDefense;
			return num * (1.0 + monster.TempPropsBuffer.GetExtProp(43));
		}

		public static double GetMinAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 7);
		}

		public static double GetMinAttackV(Monster monster)
		{
			double num = (double)monster.MonsterInfo.MinAttack;
			return num * (1.0 + monster.TempPropsBuffer.GetExtProp(11));
		}

		public static double GetMaxAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 8);
		}

		public static double GetIncreasePhyAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(11, delegate
			{
				double num = DBRoleBufferManager.ProcessAddTempAttack(client);
				num += client.ClientData.PropsCacheManager.GetExtProp(11);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreasePhyAttack);
				num += client.RoleBuffer.GetExtProp(11);
				num += client.ClientData.ExcellenceProp[4];
				num += client.ClientData.ExcellenceProp[3];
				num += client.ClientData.ExcellenceProp[24];
				num += client.ClientData.EquipProp.ExtProps[91];
				return num + client.ClientData.PropsCacheManager.GetExtProp(91);
			});
		}

		public static double GetMaxAttackV(Monster monster)
		{
			int maxAttack = monster.MonsterInfo.MaxAttack;
			return (double)maxAttack;
		}

		public static double GetMinMagicAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 9);
		}

		public static double GetMinMagicAttackV(Monster monster)
		{
			double num = (double)monster.MonsterInfo.MinAttack;
			return num * (1.0 + monster.TempPropsBuffer.GetExtProp(12));
		}

		public static double GetMaxMagicAttackV(GameClient client)
		{
			return RoleAlgorithm.GetExtProp(client, 10);
		}

		public static double GetIncreaseMagAttack(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(12, delegate
			{
				double num = DBRoleBufferManager.ProcessAddTempAttack(client);
				num += client.ClientData.PropsCacheManager.GetExtProp(12);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IncreaseMagAttack);
				num += client.RoleBuffer.GetExtProp(12);
				num += client.ClientData.ExcellenceProp[4];
				num += client.ClientData.ExcellenceProp[3];
				num += client.ClientData.ExcellenceProp[24];
				num += client.ClientData.EquipProp.ExtProps[91];
				return num + client.ClientData.PropsCacheManager.GetExtProp(91);
			});
		}

		public static double GetMaxMagicAttackV(Monster monster)
		{
			int maxAttack = monster.MonsterInfo.MaxAttack;
			return (double)maxAttack;
		}

		public static double GetMaxLifeV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(13, delegate
			{
				int num = Global.CalcOriginalOccupationID(client);
				EOccupationType eoccupationType = (EOccupationType)num;
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[num][client.ClientData.Level];
				double num2 = roleBasePropItem.LifeV;
				if (EOccupationType.EOT_Warrior == eoccupationType)
				{
					num2 += RoleAlgorithm.GetConstitution(client, true) * 5.0;
				}
				else if (EOccupationType.EOT_Magician == eoccupationType)
				{
					num2 += RoleAlgorithm.GetConstitution(client, true) * 3.6;
				}
				else if (EOccupationType.EOT_Bow == eoccupationType)
				{
					num2 += RoleAlgorithm.GetConstitution(client, true) * 4.2;
				}
				else if (EOccupationType.EOT_MagicSword == eoccupationType)
				{
					num2 += RoleAlgorithm.GetConstitution(client, true) * 4.4;
				}
				else if (EOccupationType.EOT_Summoner == eoccupationType)
				{
					num2 += RoleAlgorithm.GetConstitution(client, true) * 3.4;
				}
				num2 = num2 + client.ClientData.EquipProp.ExtProps[13] + client.RoleBuffer.GetExtProp(13) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[13] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[13];
				num2 += client.AllThingsMultipliedBuffer.GetExtProp(13);
				num2 += client.ClientData.PropsCacheManager.GetExtProp(13);
				num2 += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxLifeV);
				num2 += DBRoleBufferManager.ProcessTimeAddJunQiProp(client, ExtPropIndexes.MaxLifeV);
				num2 += client.RoleMultipliedBuffer.GetExtProp(13);
				num2 += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDMAXHPVALUE);
				double maxLifePercentV = RoleAlgorithm.GetMaxLifePercentV(client);
				num2 *= Math.Max(0.0, 1.0 + maxLifePercentV);
				return num2 + client.ClientData.PurePropsCacheManager.GetExtProp(13);
			});
		}

		public static double GetMaxLifePercentV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(14, delegate
			{
				double num = 0.0;
				num = num + client.ClientData.EquipProp.ExtProps[14] + client.RoleBuffer.GetExtProp(14) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[14] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[14];
				num += client.AllThingsMultipliedBuffer.GetExtProp(14);
				num += client.ClientData.PropsCacheManager.GetExtProp(14);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxLifePercent);
				num += client.RoleMultipliedBuffer.GetExtProp(14);
				num += client.ClientData.ExcellenceProp[8];
				num += client.ClientData.ExcellenceProp[20];
				num += DBRoleBufferManager.ProcessUpLifeLimit(client);
				double extProp = client.ClientData.PctPropsCacheManager.GetExtProp(14);
				if (extProp > 0.0)
				{
					num *= extProp;
				}
				return num;
			});
		}

		public static double GetLifeStealV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(44, delegate
			{
				double num = 0.0;
				num = num + client.ClientData.EquipProp.ExtProps[44] + client.RoleBuffer.GetExtProp(44) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[44] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[44];
				num += client.AllThingsMultipliedBuffer.GetExtProp(44);
				num += client.ClientData.PropsCacheManager.GetExtProp(44);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.LifeSteal);
				num += client.RoleMultipliedBuffer.GetExtProp(44);
				double lifeStealPercentV = RoleAlgorithm.GetLifeStealPercentV(client);
				return num * (1.0 + lifeStealPercentV);
			});
		}

		public static double GetLifeStealPercentV(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(67);
		}

		public static double GetPotionPercentV(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(68);
		}

		public static double GetAddAttackV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(45, delegate
			{
				double num = 0.0;
				num = num + client.ClientData.EquipProp.ExtProps[45] + client.RoleBuffer.GetExtProp(45) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[45] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[45];
				num += client.AllThingsMultipliedBuffer.GetExtProp(45);
				num += client.ClientData.PropsCacheManager.GetExtProp(45);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttack);
				return num + client.RoleMultipliedBuffer.GetExtProp(45);
			});
		}

		public static double GetAddDefenseV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(46, delegate
			{
				double num = 0.0;
				num = num + client.ClientData.EquipProp.ExtProps[46] + client.RoleBuffer.GetExtProp(46) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[46] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[46];
				num += client.AllThingsMultipliedBuffer.GetExtProp(46);
				num += client.ClientData.PropsCacheManager.GetExtProp(46);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddDefense);
				return num + client.RoleMultipliedBuffer.GetExtProp(46);
			});
		}

		public static double GetAddAttackPercent(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(91);
		}

		public static double GetAddDefensePercent(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(92);
		}

		public static double GetMaxMagicV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(15, delegate
			{
				int index = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
				double num = roleBasePropItem.MagicV;
				num = num + client.ClientData.EquipProp.ExtProps[15] + client.RoleBuffer.GetExtProp(15) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[15] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[15];
				num += client.AllThingsMultipliedBuffer.GetExtProp(15);
				num += client.ClientData.PropsCacheManager.GetExtProp(15);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMagicV);
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDMAXMPVALUE);
				num += client.RoleMultipliedBuffer.GetExtProp(15);
				double maxMagicPercent = RoleAlgorithm.GetMaxMagicPercent(client);
				return num * Math.Max(0.0, 1.0 + maxMagicPercent);
			});
		}

		public static double GetLuckV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(17, delegate
			{
				double num = 0.0;
				num = num + client.ClientData.EquipProp.ExtProps[17] + client.RoleBuffer.GetExtProp(17) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[17] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[17];
				num += client.ClientData.ExcellenceProp[17];
				num += client.ClientData.PropsCacheManager.GetExtProp(17);
				num *= 100.0;
				num += client.AllThingsMultipliedBuffer.GetExtProp(17);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Lucky);
				num += client.RoleMultipliedBuffer.GetExtProp(17);
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.ADDTEMPLUCKYATTACK);
				return num + client.ClientData.LuckProp;
			});
		}

		public static double GetLuckV(Monster monster)
		{
			return monster.MonsterInfo.MonsterLucky;
		}

		public static double GetHitV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(18, delegate
			{
				int index = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
				double num = roleBasePropItem.HitV;
				num += RoleAlgorithm.GetDexterity(client, true) * 0.5;
				num += client.RoleBuffer.GetExtProp(18);
				num += client.ClientData.EquipProp.ExtProps[18] + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[18] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[18];
				num += client.AllThingsMultipliedBuffer.GetExtProp(18);
				num += client.ClientData.PropsCacheManager.GetExtProp(18);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.HitV);
				num += client.RoleMultipliedBuffer.GetExtProp(18);
				double num2 = client.ClientData.ExcellenceProp[6] + client.ClientData.ExcellenceProp[19] + client.RoleBuffer.GetExtProp(54) + client.ClientData.PropsCacheManager.GetExtProp(54);
				num *= 1.0 + num2;
				return num + client.ClientData.PurePropsCacheManager.GetExtProp(18);
			});
		}

		public static double GetHitPercent(GameClient client)
		{
			return client.ClientData.ExcellenceProp[6] + client.ClientData.ExcellenceProp[19] + client.RoleBuffer.GetExtProp(54) + client.ClientData.PropsCacheManager.GetExtProp(54);
		}

		public static double GetHitV(Monster monster)
		{
			double hitV = monster.MonsterInfo.HitV;
			return hitV * (1.0 + monster.TempPropsBuffer.GetExtProp(18));
		}

		public static double GetDodgeV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(19, delegate
			{
				int index = Global.CalcOriginalOccupationID(client);
				RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
				double num = roleBasePropItem.Dodge;
				num += RoleAlgorithm.GetDexterity(client, true) * 0.5;
				num += client.ClientData.EquipProp.ExtProps[19] + client.RoleBuffer.GetExtProp(19) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[19] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[19];
				num += client.AllThingsMultipliedBuffer.GetExtProp(19);
				num += client.ClientData.PropsCacheManager.GetExtProp(19);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.Dodge);
				num += client.RoleMultipliedBuffer.GetExtProp(19);
				double num2 = client.ClientData.ExcellenceProp[12] + client.ClientData.ExcellenceProp[25] + client.RoleBuffer.GetExtProp(55) + client.ClientData.PropsCacheManager.GetExtProp(55);
				num *= 1.0 + num2;
				return num + client.ClientData.PurePropsCacheManager.GetExtProp(19);
			});
		}

		public static double GetDodgePercent(GameClient client)
		{
			return client.ClientData.ExcellenceProp[12] + client.ClientData.ExcellenceProp[25] + client.RoleBuffer.GetExtProp(55) + client.ClientData.PropsCacheManager.GetExtProp(55);
		}

		public static double GetDodgeV(Monster monster)
		{
			return monster.MonsterInfo.Dodge;
		}

		public static double GetLifeRecoverValPercentV(GameClient client)
		{
			int index = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
			return roleBasePropItem.RecoverLifeV;
		}

		public static double GetLifeRecoverAddPercentV(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(20, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[20] + client.RoleBuffer.GetExtProp(20) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[20] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[20];
				num += client.AllThingsMultipliedBuffer.GetExtProp(20);
				num += client.ClientData.PropsCacheManager.GetExtProp(20);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.LifeRecoverPercent);
				num += DBRoleBufferManager.ProcessTimeAddProp(client, BufferItemTypes.MU_ADDLIFERECOVERPERCENT);
				return num + client.RoleMultipliedBuffer.GetExtProp(20);
			});
		}

		public static double GetLifeRecoverAddPercentOnlySandR(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(88);
		}

		public static double GetLifeRecoverValPercentV(Monster monster)
		{
			return monster.MonsterInfo.RecoverLifeV;
		}

		public static double GetMagicRecoverValPercentV(GameClient client)
		{
			int index = Global.CalcOriginalOccupationID(client);
			RoleBasePropItem roleBasePropItem = Data.RoleBasePropList[index][client.ClientData.Level];
			return roleBasePropItem.RecoverMagicV;
		}

		public static double GetMagicRecoverAddPercentV(GameClient client)
		{
			return 0.0;
		}

		public static double GetMagicRecoverAddPercentOnlySandR(GameClient client)
		{
			double num = 0.0;
			return num + client.ClientData.PropsCacheManager.GetExtProp(89);
		}

		public static double GetMagicRecoverValPercentV(Monster monster)
		{
			return monster.MonsterInfo.RecoverMagicV;
		}

		public static double GetSubAttackInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(24, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[24] + client.RoleBuffer.GetExtProp(24) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[24] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[24];
				num += client.AllThingsMultipliedBuffer.GetExtProp(24);
				num += client.ClientData.PropsCacheManager.GetExtProp(24);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubAttackInjurePercent);
				num += client.RoleMultipliedBuffer.GetExtProp(24);
				return Math.Max(0.0, num);
			});
		}

		public static double GetInjurePenetrationPercent(GameClient client)
		{
			double extProp = client.ClientData.PropsCacheManager.GetExtProp(93);
			return Math.Max(0.0, extProp);
		}

		public static double GetSubAttackInjurePercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterSubAttackInjurePercent;
		}

		public static double GetSubAttackInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(25, delegate
			{
				double num = 0.0;
				num = num + client.ClientData.EquipProp.ExtProps[25] + client.RoleBuffer.GetExtProp(25) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[25] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[25];
				num += client.AllThingsMultipliedBuffer.GetExtProp(25);
				num += client.ClientData.PropsCacheManager.GetExtProp(25);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.SubAttackInjure);
				return num + client.RoleMultipliedBuffer.GetExtProp(25);
			});
		}

		public static double GetSubAttackInjureValue(Monster monster)
		{
			return monster.MonsterInfo.MonsterSubAttackInjure;
		}

		public static double GetMaxMagicPercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(16, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[16] + client.RoleBuffer.GetExtProp(16) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[16] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[16];
				num += client.AllThingsMultipliedBuffer.GetExtProp(16);
				num += client.ClientData.PropsCacheManager.GetExtProp(16);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.MaxMagicPercent);
				return num + client.RoleMultipliedBuffer.GetExtProp(16);
			});
		}

		public static double GetIgnoreDefensePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(28, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[28] + client.RoleBuffer.GetExtProp(28) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[28] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[28];
				num += client.AllThingsMultipliedBuffer.GetExtProp(28);
				num += client.ClientData.PropsCacheManager.GetExtProp(28);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreDefensePercent);
				num += client.RoleMultipliedBuffer.GetExtProp(28);
				num += client.ClientData.ExcellenceProp[14];
				return num + client.ClientData.ExcellenceProp[26];
			});
		}

		public static double GetIgnoreDefensePercent(Monster monster)
		{
			return monster.MonsterInfo.MonsterIgnoreDefensePercent;
		}

		public static double GetDecreaseInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(37, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[37] + client.RoleBuffer.GetExtProp(37) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[37] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[37];
				num += client.AllThingsMultipliedBuffer.GetExtProp(37);
				num += client.ClientData.PropsCacheManager.GetExtProp(37);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DecreaseInjurePercent);
				num += client.RoleMultipliedBuffer.GetExtProp(37);
				num += client.ClientData.ExcellenceProp[10];
				return num + client.ClientData.ExcellenceProp[22];
			});
		}

		public static double GetDecreaseInjurePercent(Monster monster)
		{
			return 0.0;
		}

		public static double GetDecreaseInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(38, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[38] + client.RoleBuffer.GetExtProp(38) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[38] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[38];
				num += client.AllThingsMultipliedBuffer.GetExtProp(38);
				num += client.ClientData.PropsCacheManager.GetExtProp(38);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.DecreaseInjureValue);
				return num + client.RoleMultipliedBuffer.GetExtProp(38);
			});
		}

		public static double GetDecreaseInjureValue(Monster monster)
		{
			return 0.0;
		}

		public static double GetCounteractInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(39, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[39] + client.RoleBuffer.GetExtProp(39) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[39] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[39];
				num += client.AllThingsMultipliedBuffer.GetExtProp(39);
				num += client.ClientData.PropsCacheManager.GetExtProp(39);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.CounteractInjurePercent);
				return num + client.RoleMultipliedBuffer.GetExtProp(39);
			});
		}

		public static double GetCounteractInjurePercent(Monster monster)
		{
			return 0.0;
		}

		public static double GetCounteractInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(40, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[40] + client.RoleBuffer.GetExtProp(40) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[40] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[40];
				num += client.AllThingsMultipliedBuffer.GetExtProp(40);
				num += client.ClientData.PropsCacheManager.GetExtProp(40);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.CounteractInjureValue);
				return num + client.RoleMultipliedBuffer.GetExtProp(40);
			});
		}

		public static double GetCounteractInjureValue(Monster monster)
		{
			return 0.0;
		}

		public static double GetAddAttackInjurePercent(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(26, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[26] + client.RoleBuffer.GetExtProp(26) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[26] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[26];
				num += client.AllThingsMultipliedBuffer.GetExtProp(26);
				num += client.ClientData.PropsCacheManager.GetExtProp(26);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttackInjurePercent);
				num += client.ClientData.ExcellenceProp[5];
				num += client.ClientData.ExcellenceProp[21];
				num += DBRoleBufferManager.ProcessTimeAddPkKingAttackProp(client, ExtPropIndexes.AddAttackInjurePercent);
				num += client.RoleMultipliedBuffer.GetExtProp(26);
				double extProp = client.ClientData.PctPropsCacheManager.GetExtProp(26);
				if (extProp > 0.0)
				{
					num *= extProp;
				}
				return num;
			});
		}

		public static double GetAddAttackInjurePercent(Monster monster)
		{
			return 0.0;
		}

		public static double GetAddAttackInjureValue(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(27, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[27] + client.RoleBuffer.GetExtProp(27) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[27] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[27];
				num += client.AllThingsMultipliedBuffer.GetExtProp(27);
				num += client.ClientData.PropsCacheManager.GetExtProp(27);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.AddAttackInjure);
				return num + client.RoleMultipliedBuffer.GetExtProp(27);
			});
		}

		public static double GetAddAttackInjureValue(Monster monster)
		{
			return 0.0;
		}

		public static double GetIgnoreDefenseRate(GameClient client)
		{
			return client.propsCacheModule.GetExtPropsValue(41, delegate
			{
				double num = 0.0;
				num += client.ClientData.EquipProp.ExtProps[41] + client.RoleBuffer.GetExtProp(41) + client.ClientData.RoleStarConstellationProp.StarConstellationSecondProps[41] + client.ClientData.RoleChangeLifeProp.ChangeLifeSecondProps[41];
				num += client.AllThingsMultipliedBuffer.GetExtProp(41);
				num += client.ClientData.PropsCacheManager.GetExtProp(41);
				num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.IgnoreDefenseRate);
				num += client.ClientData.ExcellenceProp[7];
				return num + client.RoleMultipliedBuffer.GetExtProp(41);
			});
		}

		public static double GetIgnoreDefenseRate(Monster monster)
		{
			return monster.MonsterInfo.MonsterIgnoreDefenseRate;
		}

		public static double GetFrozenPercent(IObject obj)
		{
			double num = 0.0;
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				if (null != gameClient)
				{
					num += gameClient.ClientData.EquipProp.ExtProps[56];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(56);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					num = robot.FrozenPercent;
					if (num > 1.0)
					{
						num /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(num, 0.0);
		}

		public static double GetPalsyPercent(IObject obj)
		{
			double num = 0.0;
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				if (null != gameClient)
				{
					num += gameClient.ClientData.EquipProp.ExtProps[57];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(57);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					num = robot.PalsyPercent;
					if (num > 1.0)
					{
						num /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(num, 0.0);
		}

		public static double GetSpeedDownPercent(IObject obj)
		{
			double num = 0.0;
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				if (null != gameClient)
				{
					num += gameClient.ClientData.EquipProp.ExtProps[58];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(58);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					num = robot.SpeedDownPercent;
					if (num > 1.0)
					{
						num /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(num, 0.0);
		}

		public static double GetBlowPercent(IObject obj)
		{
			double num = 0.0;
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				if (null != gameClient)
				{
					num += gameClient.ClientData.EquipProp.ExtProps[59];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(59);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
					num = robot.BlowPercent;
					if (num > 1.0)
					{
						num /= 100.0;
					}
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return Math.Max(num, 0.0);
		}

		public static double GetAutoRevivePercent(object obj)
		{
			double num = 0.0;
			if (obj is GameClient)
			{
				GameClient gameClient = obj as GameClient;
				if (null != gameClient)
				{
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(60);
				}
			}
			else if (obj is Robot)
			{
				Robot robot = obj as Robot;
				if (null != robot)
				{
				}
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
				}
			}
			return num;
		}

		public static double GetExtPropValue(GameClient client, ExtPropIndexes extPropIndex)
		{
			double num = 0.0;
			num += client.ClientData.EquipProp.ExtProps[(int)extPropIndex];
			return num + client.ClientData.PropsCacheManager.GetExtProp((int)extPropIndex);
		}

		public static double CalRebornAttackInjureValue(IObject obj, IObject objTarget, ExtPropIndexes propIdx, ref int damageType)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			double num8 = 0.0;
			double num9 = 0.0;
			bool flag = true;
			if (RebornEquip.ExtPropIndexMap == null || !RebornEquip.ExtPropIndexMap.ContainsKey((int)propIdx) || RebornEquip.ExtPropIndexMap[(int)propIdx] == null || RebornEquip.ExtPropIndexMap[(int)propIdx].Count != 2)
			{
				flag = false;
			}
			if (obj is GameClient)
			{
				num = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx);
				num3 = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx + 2);
				num6 = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx + 5);
				num7 = RoleAlgorithm.GetExtPropValue(obj as GameClient, propIdx + 6);
				num += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornAttack);
				num3 += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornPenetratePercent);
				num6 += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornDoubleAttackPercent);
				num7 += RoleAlgorithm.GetExtPropValue(obj as GameClient, ExtPropIndexes.RebornDoubleAttackInjure);
			}
			else if (obj is Monster)
			{
				Monster monster = obj as Monster;
				num = monster.DynamicData.ExtProps[(int)propIdx];
				num3 = monster.DynamicData.ExtProps[(int)(propIdx + 2)];
				num6 = monster.DynamicData.ExtProps[(int)(propIdx + 5)];
				num7 = monster.DynamicData.ExtProps[(int)(propIdx + 6)];
				num += monster.DynamicData.ExtProps[157];
				num3 += monster.DynamicData.ExtProps[159];
				num6 += monster.DynamicData.ExtProps[162];
				num7 += monster.DynamicData.ExtProps[163];
			}
			if (objTarget is GameClient)
			{
				num2 = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, propIdx + 1);
				num4 = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, propIdx + 3);
				num5 = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, propIdx + 4);
				num2 += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornDefense);
				num4 += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornAbsorbPercent);
				num5 += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornWeakPercent);
				if (flag)
				{
					num8 = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, (ExtPropIndexes)RebornEquip.ExtPropIndexMap[(int)propIdx][0]);
					num9 = RoleAlgorithm.GetExtPropValue(objTarget as GameClient, (ExtPropIndexes)RebornEquip.ExtPropIndexMap[(int)propIdx][1]);
					num8 += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornDoubleAttackResistance);
					num9 += RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.RebornDoubleAttackInjureResistance);
				}
			}
			else if (objTarget is Monster)
			{
				num2 = (objTarget as Monster).DynamicData.ExtProps[(int)(propIdx + 1)];
				num4 = (objTarget as Monster).DynamicData.ExtProps[(int)(propIdx + 3)];
				num5 = (objTarget as Monster).DynamicData.ExtProps[(int)(propIdx + 4)];
				num2 += (objTarget as Monster).DynamicData.ExtProps[158];
				num4 += (objTarget as Monster).DynamicData.ExtProps[160];
				num5 += (objTarget as Monster).DynamicData.ExtProps[161];
				if (flag)
				{
					num8 = (objTarget as Monster).DynamicData.ExtProps[RebornEquip.ExtPropIndexMap[(int)propIdx][0]];
					num9 = (objTarget as Monster).DynamicData.ExtProps[RebornEquip.ExtPropIndexMap[(int)propIdx][1]];
					num8 += (objTarget as Monster).DynamicData.ExtProps[175];
					num9 += (objTarget as Monster).DynamicData.ExtProps[176];
				}
			}
			bool flag2 = false;
			double num10 = (double)Global.GetRandomNumber(1, 101);
			if (num10 <= (num6 - num8) * 100.0)
			{
				flag2 = true;
			}
			double num11;
			if (flag2)
			{
				num11 = Math.Max(0.0, num - num2) * (2.0 + Math.Min(Math.Max(0.0, num7 - num9), 1.0));
				damageType = 11;
			}
			else
			{
				num11 = Math.Max(0.0, num - num2);
			}
			double num12 = Math.Max(-1.0, num3 - num4);
			num12 = Math.Min(num12, 0.0);
			num5 = Math.Max(0.0, 1.0 + num5);
			return num11 * (1.0 + num12) * num5;
		}

		private static double GetAttackPower(IObject obj, int damage, int val, int luck, int nFatalValue, out int nDamageType, int nMaxAttackValue, double subSpPercent = 0.0)
		{
			nDamageType = 0;
			if (val < 0)
			{
				val = 0;
			}
			int randomNumber = Global.GetRandomNumber(1, 101);
			int num2;
			if (randomNumber <= nFatalValue)
			{
				double num = 0.2;
				GameClient gameClient = obj as GameClient;
				if (gameClient != null)
				{
					num *= 1.0 + RoleAlgorithm.GetFatalHurt(gameClient);
					num += DBRoleBufferManager.ProcessSpecialAttackValueBuff(gameClient, 65);
					gameClient.CheckCheatData.LastDamageType = Global.SetIntSomeBit(3, gameClient.CheckCheatData.LastDamageType, true);
				}
				num2 = (int)((double)nMaxAttackValue * (1.0 + num * (1.0 - subSpPercent)));
				nDamageType = 3;
			}
			else if (randomNumber <= luck)
			{
				num2 = damage + val;
				double num = 0.0;
				GameClient gameClient = obj as GameClient;
				if (gameClient != null)
				{
					num = DBRoleBufferManager.ProcessSpecialAttackValueBuff(gameClient, 64);
					gameClient.CheckCheatData.LastDamageType = Global.SetIntSomeBit(4, gameClient.CheckCheatData.LastDamageType, true);
				}
				num2 += (int)((double)num2 * num * (1.0 - subSpPercent));
				nDamageType = 4;
			}
			else
			{
				num2 = damage + Global.GetRandomNumber(0, val + 1);
				if (obj is GameClient)
				{
					GameClient gameClient = obj as GameClient;
					gameClient.CheckCheatData.LastDamageType = Global.SetIntSomeBit(0, gameClient.CheckCheatData.LastDamageType, true);
				}
			}
			return (double)num2;
		}

		public static double CalcInjureValue(IObject obj, IObject objTarget, double damage, ref int damageType, double elementInjurePercnet)
		{
			double num = damage;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			if (obj is Robot)
			{
				Robot robot = obj as Robot;
				num2 = robot.RuthlessValue;
				num2 -= (double)((int)RoleAlgorithm.GetDeRuthlessPercent(objTarget as GameClient));
				num3 = robot.ColdValue;
				num3 -= (double)((int)RoleAlgorithm.GetDeColdPercent(objTarget as GameClient));
				num4 = robot.SavageValue;
				num4 -= (double)((int)RoleAlgorithm.GetDeSavagePercent(objTarget as GameClient));
				num5 = (double)robot.DoubleValue;
				num5 -= (double)((int)RoleAlgorithm.GetDeDoubleAttack(objTarget as GameClient));
			}
			else if (obj is GameClient)
			{
				num2 = (double)((int)RoleAlgorithm.GetRuthlessPercent(obj as GameClient));
				num3 = (double)((int)RoleAlgorithm.GetColdPercent(obj as GameClient));
				num4 = (double)((int)RoleAlgorithm.GetSavagePercent(obj as GameClient));
				num5 = (double)((int)RoleAlgorithm.GetDoubleAttack(obj as GameClient));
				if (objTarget is GameClient)
				{
					num2 -= (double)((int)RoleAlgorithm.GetDeRuthlessPercent(objTarget as GameClient));
					num3 -= (double)((int)RoleAlgorithm.GetDeColdPercent(objTarget as GameClient));
					num4 -= (double)((int)RoleAlgorithm.GetDeSavagePercent(objTarget as GameClient));
					num5 -= (double)((int)RoleAlgorithm.GetDeDoubleAttack(objTarget as GameClient));
				}
				else if (objTarget is Robot)
				{
					Robot robot = objTarget as Robot;
					num2 -= robot.DeRuthlessValue;
					num3 -= robot.DeColdValue;
					num4 -= robot.DeSavageValue;
					num5 -= robot.DeDoubleValue;
				}
			}
			num2 = Math.Max(0.0, num2);
			num3 = Math.Max(0.0, num3);
			num4 = Math.Max(0.0, num4);
			num5 = Math.Max(0.0, num5);
			double[] rateArr = new double[]
			{
				num2,
				num3,
				num4,
				num5
			};
			switch (RoleAlgorithm.GetRateIndex(rateArr, 100))
			{
			case 0:
			{
				num = damage * 1.5;
				double[] array = new double[2];
				array[0] = (double)((int)(num * 0.1));
				double[] actionParams = array;
				MagicAction.ProcessAction(obj, objTarget, MagicActionIDs.MU_ADD_HP, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
				damageType = 6;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			}
			case 1:
			{
				num = damage * 2.0;
				double[] actionParams = new double[]
				{
					0.5,
					4.0
				};
				MagicAction.ProcessAction(obj, objTarget, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
				damageType = 7;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			}
			case 2:
				num = damage * 3.0;
				damageType = 8;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			case 3:
			{
				double num6 = 1.0;
				if (obj is GameClient)
				{
					num6 += DBRoleBufferManager.ProcessSpecialAttackValueBuff(obj as GameClient, 66);
				}
				if (objTarget is GameClient)
				{
					num6 *= 1.0 - RoleAlgorithm.GetExtPropValue(objTarget as GameClient, ExtPropIndexes.SPAttackInjurePercent);
				}
				num = (double)((int)(damage * (1.0 + num6)));
				damageType = 2;
				if (obj is GameClient)
				{
					(obj as GameClient).CheckCheatData.LastDamageType = Global.SetIntSomeBit(damageType, (obj as GameClient).CheckCheatData.LastDamageType, true);
				}
				break;
			}
			}
			num += (double)GameManager.ElementsAttackMgr.CalcAllElementDamage(obj, objTarget) * elementInjurePercnet;
			return num * (1.0 + SingletonTemplate<CoupleArenaManager>.Instance().CalcBuffHurt(obj, objTarget));
		}

		public static int GetRateIndexPercent(double[] rateArr)
		{
			int num = -1;
			int result;
			if (rateArr == null || rateArr.Length <= 0)
			{
				result = num;
			}
			else
			{
				double num2 = 0.0;
				double random = Global.GetRandom();
				for (int i = 0; i < rateArr.Length; i++)
				{
					num2 += rateArr[i];
					if (num2 > random)
					{
						return i;
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetRateIndex(double[] rateArr, int max)
		{
			int num = -1;
			int result;
			if (rateArr == null || rateArr.Length <= 0)
			{
				result = num;
			}
			else
			{
				double num2 = 0.0;
				double num3 = (double)Global.GetRandomNumber(0, max);
				for (int i = 0; i < rateArr.Length; i++)
				{
					num2 += rateArr[i];
					if (num2 > num3)
					{
						return i;
					}
				}
				result = num;
			}
			return result;
		}

		private static double GetDefensePower(int baseDefense, int val)
		{
			if (val < 0)
			{
				val = 0;
			}
			return (double)(baseDefense + Global.GetRandomNumber(0, val + 1));
		}

		private static double GetDefenseValue(int minDefense, int maxDefense)
		{
			return RoleAlgorithm.GetDefensePower(minDefense, maxDefense - minDefense);
		}

		public static double CalcAttackValue(IObject obj, int minAttackV, int maxAttackV, int lucky, int nFatalValue, out int nDamageType, double subSpPercent = 0.0)
		{
			nDamageType = 0;
			return RoleAlgorithm.GetAttackPower(obj, minAttackV, maxAttackV - minAttackV, lucky, nFatalValue, out nDamageType, maxAttackV, subSpPercent);
		}

		public static double GetRealInjuredValue(long attackV, long defenseV)
		{
			return (double)((int)Math.Max(attackV - defenseV, (long)((int)Math.Max((double)attackV * 0.1, 5.0))));
		}

		public static double GetRealHitRate(double hitV, double dodgeV)
		{
			double result;
			if (dodgeV <= 0.0)
			{
				result = 1.0;
			}
			else
			{
				int randomNumber = Global.GetRandomNumber(0, 101);
				int l = (int)(hitV / (hitV + dodgeV / 10.0) * 100.0);
				int num = Global.GMax(l, 3);
				result = (double)((randomNumber <= num) ? 1 : 0);
			}
			return result;
		}

		public static double GetRealHitRate(double DodgePercent)
		{
			double result;
			if (DodgePercent <= 0.0)
			{
				result = 1.0;
			}
			else
			{
				int randomNumber = Global.GetRandomNumber(0, 101);
				int num = (int)(DodgePercent * 100.0);
				result = (double)((randomNumber <= num) ? 0 : 1);
			}
			return result;
		}

		public static int CallAttackArmor(IObject attacker, IObject obj, ref int injure1, ref int injure2)
		{
			int num = -1;
			int num2 = injure1 + injure2;
			if (num2 > 0 && (attacker is GameClient || attacker is Robot))
			{
				if (obj is GameClient)
				{
					GameClient gameClient = obj as GameClient;
					if (gameClient.ClientData.CurrentArmorV > 0)
					{
						int num3 = (int)Global.GMin((double)gameClient.ClientData.CurrentArmorV, (double)num2 * gameClient.ClientData.ArmorPercent);
						int num4 = num2 - num3;
						num = Global.GMax(gameClient.ClientData.CurrentArmorV - num3, 0);
						gameClient.ClientData.CurrentArmorV = num;
						injure2 = (int)((long)injure2 * (long)num4 / (long)num2);
						injure1 = num4 - injure2;
					}
				}
				else if (obj is Robot)
				{
					Robot robot = obj as Robot;
					num = (int)robot.DynamicData.ExtProps[119];
					if (num > 0)
					{
						double num5 = robot.DynamicData.ExtProps[120];
						int num3 = (int)Global.GMin((double)num, (double)num2 * num5);
						int num4 = num2 - num3;
						num = Global.GMax(num - num3, 0);
						robot.DynamicData.ExtProps[119] = (double)num;
						injure2 = (int)((long)injure2 * (long)num4 / (long)num2);
						injure1 = num4 - injure2;
					}
				}
			}
			return num;
		}

		public static int CalcAttackInjure(int attackType, IObject obj, int injured, IObject attacker = null)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			double num7 = 0.0;
			if (attacker is GameClient)
			{
				num7 = RoleAlgorithm.GetInjurePenetrationPercent(attacker as GameClient);
			}
			else if (attacker is Monster)
			{
				num7 = (attacker as Monster).DynamicData.ExtProps[93];
			}
			if (obj is GameClient)
			{
				num = RoleAlgorithm.GetSubAttackInjurePercent(obj as GameClient);
				num2 = RoleAlgorithm.GetSubAttackInjureValue(obj as GameClient);
				num3 = RoleAlgorithm.GetDecreaseInjurePercent(obj as GameClient);
				num4 = RoleAlgorithm.GetDecreaseInjureValue(obj as GameClient);
				num5 = RoleAlgorithm.GetCounteractInjurePercent(obj as GameClient);
				num6 = RoleAlgorithm.GetCounteractInjureValue(obj as GameClient);
			}
			else if (obj is Monster)
			{
				num = RoleAlgorithm.GetSubAttackInjurePercent(obj as Monster);
				num2 = RoleAlgorithm.GetSubAttackInjureValue(obj as Monster);
				num3 = RoleAlgorithm.GetDecreaseInjurePercent(obj as Monster);
				num4 = RoleAlgorithm.GetDecreaseInjureValue(obj as Monster);
				num5 = RoleAlgorithm.GetCounteractInjurePercent(obj as Monster);
				num6 = RoleAlgorithm.GetCounteractInjureValue(obj as Monster);
			}
			if (obj is GameClient || obj is Robot)
			{
				double item = Data.ExtPropThreshold[24].Item1;
				double item2 = Data.ExtPropThreshold[24].Item2;
				num = Math.Max(item, num - num7);
				num = Math.Min(item2, num);
			}
			injured -= (int)(num2 + num4 + num6);
			injured = (int)((double)injured * (1.0 - num) * (1.0 - num3) * (1.0 - num5));
			return injured;
		}

		public static int GetDefenseByCalcingIgnoreDefense(int attackType, IObject self, int defense, ref int burst)
		{
			double num = 0.0;
			if (self is GameClient)
			{
				num = RoleAlgorithm.GetIgnoreDefensePercent(self as GameClient);
			}
			else if (self is Monster)
			{
				num = RoleAlgorithm.GetIgnoreDefensePercent(self as Monster);
			}
			int result;
			if (num <= 0.0)
			{
				result = defense;
			}
			else
			{
				int randomNumber = Global.GetRandomNumber(0, 101);
				int num2 = (int)(num * 100.0);
				if (randomNumber <= num2)
				{
					burst = 1;
					result = 0;
				}
				else
				{
					result = defense;
				}
			}
			return result;
		}

		private static bool ClientIgnorePhyAttack(GameClient client, ref int burst)
		{
			double extPropValue = RoleAlgorithm.GetExtPropValue(client, ExtPropIndexes.IgnorePhyAttackPercent);
			bool result;
			if (extPropValue > 0.0 && extPropValue >= Global.GetRandom())
			{
				burst = 9;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool ClientIgnoreMagicAttack(GameClient client, ref int burst)
		{
			double extPropValue = RoleAlgorithm.GetExtPropValue(client, ExtPropIndexes.IgnoreMagyAttackPercent);
			bool result;
			if (extPropValue > 0.0 && extPropValue >= Global.GetRandom())
			{
				burst = 10;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static void AttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(monster);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					injure = 0;
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinAttackV(client);
			int num3 = (int)RoleAlgorithm.GetMaxAttackV(client);
			int num4 = (int)RoleAlgorithm.GetLuckV(client);
			int num5 = (int)RoleAlgorithm.GetFatalAttack(client);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				num4 -= (int)robot.DeLucky;
				num5 -= (int)robot.DeFatalValue;
			}
			int num6 = (int)RoleAlgorithm.CalcAttackValue(client, num2 + addAttackMin, num3 + addAttackMax, num4, num5, out burst, 0.0);
			num6 = (int)((double)num6 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinADefenseV(monster);
			int maxDefense = (int)RoleAlgorithm.GetMaxADefenseV(monster);
			int num7 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num7 = 0;
				burst = 1;
			}
			else
			{
				num7 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, client, num7, ref burst);
			}
			if (num7 > 0)
			{
				num7 = (int)((double)num7 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			injure += (int)RoleAlgorithm.GetRealInjuredValue((long)num6, (long)num7);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetPhySkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 119);
			if (null != monsterBufferDataByID)
			{
				long num8 = TimeUtil.NOW();
				if (num8 - monsterBufferDataByID.StartTime < (long)monsterBufferDataByID.BufferSecs * 1000L)
				{
					injure = (int)((double)injure * (1.0 + (double)monsterBufferDataByID.BufferVal / 1000.0));
				}
			}
			injure = RoleAlgorithm.CalcAttackInjure(0, monster, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, monster, (double)injure, ref burst, injurePercnet);
				double num9 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, monster, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, monster, magicCode);
				injure = (int)((double)injure * (baseRate + num9) + (double)addVlue + magicCodeAddInjure);
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = monster.GetObjectID();
				client.CheckCheatData.LastEnemyName = monster.MonsterInfo.VSName;
				client.CheckCheatData.LastEnemyPos = monster.CurrentPos;
			}
		}

		public static void AttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnorePhyAttack(client, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(client);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					injure = 0;
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinAttackV(monster);
			int num3 = (int)RoleAlgorithm.GetMaxAttackV(monster);
			int num4 = (int)RoleAlgorithm.GetLuckV(monster);
			int num5 = (int)RoleAlgorithm.GetFatalAttack(monster);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				num4 = robot.Lucky;
				num5 = robot.FatalValue;
				num4 -= (int)RoleAlgorithm.GetDeLuckyAttack(client);
				num5 -= (int)RoleAlgorithm.GetDeFatalAttack(client);
			}
			int num6 = (int)RoleAlgorithm.CalcAttackValue(monster, num2 + addAttackMin, num3 + addAttackMax, num4, num5, out burst, 0.0);
			num6 = (int)((double)num6 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinADefenseV(client);
			int maxDefense = (int)RoleAlgorithm.GetMaxADefenseV(client);
			int num7 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num7 = 0;
			}
			else
			{
				num7 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, num7, ref burst);
			}
			if (num7 > 0)
			{
				num7 = (int)((double)num7 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num6, (long)num7);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			if (monster is Robot)
			{
				injure /= 2;
			}
			injure = RoleAlgorithm.CalcAttackInjure(0, client, injure, monster);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(monster, client, (double)injure, ref burst, injurePercnet);
				double num8 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, client, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, client, magicCode);
				injure = (int)((double)injure * (baseRate + num8) + (double)addVlue + magicCodeAddInjure);
			}
		}

		public static void AttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate = 1.0, int addVlue = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					injure = 0;
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinAttackV(monster);
			int num3 = (int)RoleAlgorithm.GetMaxAttackV(monster);
			int lucky = (int)RoleAlgorithm.GetLuckV(monster);
			int nFatalValue = (int)RoleAlgorithm.GetFatalAttack(monster);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky = robot.Lucky;
				nFatalValue = robot.FatalValue;
			}
			int num4 = (int)RoleAlgorithm.CalcAttackValue(monster, num2 + addAttackMin, num3 + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			num4 = (int)((double)num4 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinADefenseV(enemy);
			int maxDefense = (int)RoleAlgorithm.GetMaxADefenseV(enemy);
			int num5 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num5 = 0;
			}
			else
			{
				num5 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, num5, ref burst);
			}
			if (num5 > 0)
			{
				num5 = (int)((double)num5 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num4, (long)num5);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(0, enemy, injure, null);
			double num6 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, enemy, magicCode);
			double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, enemy, magicCode);
			injure = (int)((double)injure * (baseRate + num6) + (double)addVlue + magicCodeAddInjure);
			if (injure <= 0)
			{
				injure = -1;
			}
		}

		public static void AttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnorePhyAttack(enemy, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					injure = 0;
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinAttackV(client);
			int num3 = (int)RoleAlgorithm.GetMaxAttackV(client);
			int num4 = (int)RoleAlgorithm.GetLuckV(client);
			int num5 = (int)RoleAlgorithm.GetFatalAttack(client);
			num4 -= (int)RoleAlgorithm.GetDeLuckyAttack(enemy);
			num5 -= (int)RoleAlgorithm.GetDeFatalAttack(enemy);
			double extPropValue = RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.SPAttackInjurePercent);
			int num6 = (int)RoleAlgorithm.CalcAttackValue(client, num2 + addAttackMin, num3 + addAttackMax, num4, num5, out burst, extPropValue);
			num6 = (int)((double)num6 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinADefenseV(enemy);
			int maxDefense = (int)RoleAlgorithm.GetMaxADefenseV(enemy);
			int num7 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num7 = 0;
				burst = 1;
			}
			else
			{
				num7 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, client, num7, ref burst);
			}
			if (num7 > 0)
			{
				num7 = (int)((double)num7 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num6, (long)num7);
			injure = (int)((double)injure * (1.0 - RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.AttackInjurePercent)));
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetPhySkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(0, enemy, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, enemy, (double)injure, ref burst, injurePercnet);
				double num8 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, enemy, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, enemy, magicCode);
				injure = (int)((double)injure * (baseRate + num8) + (double)addVlue + magicCodeAddInjure);
				BufferData bufferDataByID = Global.GetBufferDataByID(enemy, 119);
				if (null != bufferDataByID)
				{
					long num9 = TimeUtil.NOW();
					if (num9 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						injure = (int)((double)injure * (1.0 + (double)bufferDataByID.BufferVal / 1000.0));
					}
				}
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = enemy.ClientData.RoleID;
				client.CheckCheatData.LastEnemyName = enemy.ClientData.RoleName;
				client.CheckCheatData.LastEnemyPos = enemy.CurrentPos;
			}
		}

		public static void MAttackEnemy(GameClient client, Monster monster, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(monster);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					injure = 0;
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinMagicAttackV(client);
			int num3 = (int)RoleAlgorithm.GetMaxMagicAttackV(client);
			int num4 = (int)RoleAlgorithm.GetLuckV(client);
			int num5 = (int)RoleAlgorithm.GetFatalAttack(client);
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				num4 -= (int)robot.DeLucky;
				num5 -= (int)robot.DeFatalValue;
			}
			int num6 = (int)RoleAlgorithm.CalcAttackValue(client, num2 + addAttackMin, num3 + addAttackMax, num4, num5, out burst, 0.0);
			num6 = (int)((double)num6 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinMDefenseV(monster);
			int maxDefense = (int)RoleAlgorithm.GetMaxMDefenseV(monster);
			int num7 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num7 = 0;
				burst = 1;
			}
			else
			{
				num7 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(1, client, num7, ref burst);
			}
			if (num7 > 0)
			{
				num7 = (int)((double)num7 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num6, (long)num7);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetMagicSkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, monster, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, monster, (double)injure, ref burst, injurePercnet);
				double num8 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, monster, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, monster, magicCode);
				injure = (int)((double)injure * (baseRate + num8) + (double)addVlue + magicCodeAddInjure);
				BufferData monsterBufferDataByID = Global.GetMonsterBufferDataByID(monster, 119);
				if (null != monsterBufferDataByID)
				{
					long num9 = TimeUtil.NOW();
					if (num9 - monsterBufferDataByID.StartTime < (long)monsterBufferDataByID.BufferSecs * 1000L)
					{
						injure = (int)((double)injure * (1.0 + (double)monsterBufferDataByID.BufferVal / 1000.0));
					}
				}
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = monster.GetObjectID();
				client.CheckCheatData.LastEnemyName = monster.MonsterInfo.VSName;
				client.CheckCheatData.LastEnemyPos = monster.CurrentPos;
			}
		}

		public static void MAttackEnemy(Monster monster, GameClient client, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnoreMagicAttack(client, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(client);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					injure = 0;
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinMagicAttackV(monster);
			int num3 = (int)RoleAlgorithm.GetMaxMagicAttackV(monster);
			int num4 = 0;
			int num5 = 0;
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				num4 = robot.Lucky;
				num5 = robot.FatalValue;
				num4 -= (int)RoleAlgorithm.GetDeLuckyAttack(client);
				num5 -= (int)RoleAlgorithm.GetDeFatalAttack(client);
			}
			int num6 = (int)RoleAlgorithm.CalcAttackValue(monster, num2 + addAttackMin, num3 + addAttackMax, num4, num5, out burst, 0.0);
			num6 = (int)((double)num6 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinMDefenseV(client);
			int maxDefense = (int)RoleAlgorithm.GetMaxMDefenseV(client);
			int num7 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num7 = 0;
			}
			else
			{
				num7 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, num7, ref burst);
			}
			if (num7 > 0)
			{
				num7 = (int)((double)num7 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num6, (long)num7);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, client, injure, monster);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(monster, client, (double)injure, ref burst, injurePercnet);
				double num8 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, client, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, client, magicCode);
				injure = (int)((double)injure * (baseRate + num8) + (double)addVlue + magicCodeAddInjure);
			}
		}

		public static void MAttackEnemy(Monster monster, Monster enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate = 1.0, int addVlue = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				double hitV = RoleAlgorithm.GetHitV(monster);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinMagicAttackV(monster);
			int num3 = (int)RoleAlgorithm.GetMaxMagicAttackV(monster);
			int lucky = 0;
			int nFatalValue = 0;
			if (monster is Robot)
			{
				Robot robot = monster as Robot;
				lucky = robot.Lucky;
				nFatalValue = robot.FatalValue;
			}
			int num4 = (int)RoleAlgorithm.CalcAttackValue(monster, num2 + addAttackMin, num3 + addAttackMax, lucky, nFatalValue, out burst, 0.0);
			num4 = (int)((double)num4 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
			int maxDefense = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
			int num5 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num5 = 0;
			}
			else
			{
				num5 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(0, monster, num5, ref burst);
			}
			if (num5 > 0)
			{
				num5 = (int)((double)num5 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(monster)));
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num4, (long)num5);
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(monster)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(monster) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, enemy, injure, null);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				double num6 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(monster, enemy, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(monster, enemy, magicCode);
				injure = (int)((double)injure * (baseRate + num6) + (double)addVlue + magicCodeAddInjure);
			}
		}

		public static void MAttackEnemy(GameClient client, GameClient enemy, bool forceBurst, double injurePercnet, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, out int burst, out int injure, bool ignoreDefenseAndDodge, double baseRate, int addVlue, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			burst = 0;
			injure = 0;
			if (!ignoreDefenseAndDodge)
			{
				if (RoleAlgorithm.ClientIgnoreMagicAttack(enemy, ref burst))
				{
					return;
				}
				double hitV = RoleAlgorithm.GetHitV(client);
				double dodgeV = RoleAlgorithm.GetDodgeV(enemy);
				int num = (int)RoleAlgorithm.GetRealHitRate(hitV, dodgeV);
				if (num <= 0)
				{
					return;
				}
			}
			int num2 = (int)RoleAlgorithm.GetMinMagicAttackV(client);
			int num3 = (int)RoleAlgorithm.GetMaxMagicAttackV(client);
			int num4 = (int)RoleAlgorithm.GetLuckV(client);
			int num5 = (int)RoleAlgorithm.GetFatalAttack(client);
			num4 -= (int)RoleAlgorithm.GetDeLuckyAttack(enemy);
			num5 -= (int)RoleAlgorithm.GetDeFatalAttack(enemy);
			double extPropValue = RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.SPAttackInjurePercent);
			int num6 = (int)RoleAlgorithm.CalcAttackValue(client, num2 + addAttackMin, num3 + addAttackMax, num4, num5, out burst, extPropValue);
			num6 = (int)((double)num6 * attackPercent);
			int minDefense = (int)RoleAlgorithm.GetMinMDefenseV(enemy);
			int maxDefense = (int)RoleAlgorithm.GetMaxMDefenseV(enemy);
			int num7 = (int)RoleAlgorithm.GetDefenseValue(minDefense, maxDefense);
			if (ignoreDefenseAndDodge)
			{
				num7 = 0;
				burst = 1;
			}
			else
			{
				num7 = RoleAlgorithm.GetDefenseByCalcingIgnoreDefense(1, client, num7, ref burst);
			}
			if (num7 > 0)
			{
				num7 = (int)((double)num7 * (1.0 - RoleAlgorithm.GetIgnoreDefenseRate(client)));
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			injure = (int)RoleAlgorithm.GetRealInjuredValue((long)num6, (long)num7);
			injure = (int)((double)injure * (1.0 - RoleAlgorithm.GetExtPropValue(enemy, ExtPropIndexes.AttackInjurePercent)));
			injure = (int)((double)injure * (1.0 + RoleAlgorithm.GetAddAttackInjurePercent(client) + RoleAlgorithm.GetMagicSkillIncrease(client)));
			injure = (int)((double)injure * injurePercnet + RoleAlgorithm.GetAddAttackInjureValue(client) + (double)addInjure);
			injure = RoleAlgorithm.CalcAttackInjure(1, enemy, injure, client);
			if (injure <= 0)
			{
				injure = -1;
			}
			else
			{
				injure = (int)RoleAlgorithm.CalcInjureValue(client, enemy, (double)injure, ref burst, injurePercnet);
				double num8 = shenShiInjurePercent + ShenShiManager.getInstance().GetMagicCodeAddPercent(client, enemy, magicCode);
				double magicCodeAddInjure = ShenShiManager.getInstance().GetMagicCodeAddInjure(client, enemy, magicCode);
				injure = (int)((double)injure * (baseRate + num8) + (double)addVlue + magicCodeAddInjure);
				BufferData bufferDataByID = Global.GetBufferDataByID(enemy, 119);
				if (null != bufferDataByID)
				{
					long num9 = TimeUtil.NOW();
					if (num9 - bufferDataByID.StartTime < (long)bufferDataByID.BufferSecs * 1000L)
					{
						injure = (int)((double)injure * (1.0 + (double)bufferDataByID.BufferVal / 1000.0));
					}
				}
				client.CheckCheatData.LastDamage = (long)injure;
				client.CheckCheatData.LastEnemyID = enemy.ClientData.RoleID;
				client.CheckCheatData.LastEnemyName = enemy.ClientData.RoleName;
				client.CheckCheatData.LastEnemyPos = enemy.CurrentPos;
			}
		}

		public static double GetRoleNegativeRate(GameClient client, double baseVal, ExtPropIndexes extPropIndex)
		{
			double num = client.ClientData.EquipProp.ExtProps[(int)extPropIndex] + client.RoleBuffer.GetExtProp((int)extPropIndex);
			num += client.ClientData.PropsCacheManager.GetExtProp((int)extPropIndex);
			num += DBRoleBufferManager.ProcessTempBufferProp(client, extPropIndex);
			num += client.RoleMultipliedBuffer.GetExtProp((int)extPropIndex);
			return num + baseVal;
		}

		public static double GetRoleStateDingSheng(GameClient client, double baseVal)
		{
			double num = client.ClientData.EquipProp.ExtProps[47] + client.RoleBuffer.GetExtProp(47);
			num += client.ClientData.PropsCacheManager.GetExtProp(47);
			num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateDingShen);
			num = client.RoleMultipliedBuffer.GetExtProp(47, num);
			num += baseVal;
			return num + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		public static double GetRoleStateMoveSpeed(GameClient client, double baseVal)
		{
			double num = client.ClientData.EquipProp.ExtProps[48] + client.RoleBuffer.GetExtProp(48);
			num += client.ClientData.PropsCacheManager.GetExtProp(48);
			num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateMoveSpeed);
			num = client.RoleMultipliedBuffer.GetExtProp(48, num);
			num += baseVal;
			return num + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		public static double GetRoleStateJiTui(GameClient client, double baseVal)
		{
			double num = client.ClientData.EquipProp.ExtProps[49] + client.RoleBuffer.GetExtProp(49);
			num += client.ClientData.PropsCacheManager.GetExtProp(49);
			num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateJiTui);
			num = client.RoleMultipliedBuffer.GetExtProp(49, num);
			return num + baseVal;
		}

		public static double GetRoleStateHunMi(GameClient client, double baseVal)
		{
			double num = client.ClientData.EquipProp.ExtProps[50] + client.RoleBuffer.GetExtProp(50);
			num += client.ClientData.PropsCacheManager.GetExtProp(50);
			num += DBRoleBufferManager.ProcessTempBufferProp(client, ExtPropIndexes.StateHunMi);
			num = client.RoleMultipliedBuffer.GetExtProp(50, num);
			num += baseVal;
			return num + 0.1 * (double)client.ClientData.ChangeLifeCount;
		}

		public static List<ExtPropIndexes>[] ExtListArray;

		public static List<ExtPropIndexes>[] BaseListArray;

		public static Dictionary<ExtPropIndexes, ExtPropItem> roleExtPropDic = new Dictionary<ExtPropIndexes, ExtPropItem>
		{
			{
				ExtPropIndexes.Strong,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AttackSpeed,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.MoveSpeed,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.MinDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreasePhyDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.528,
						0.384,
						0.45599999999999996,
						0.48,
						0.0,
						0.336
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					}
				}
			},
			{
				ExtPropIndexes.MaxDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreasePhyDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.88,
						0.64,
						0.76,
						0.8,
						0.0,
						0.56
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddDefense
					}
				}
			},
			{
				ExtPropIndexes.MinMDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreaseMagDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.36,
						0.504,
						0.432,
						0.45599999999999996,
						0.0,
						0.528
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddMDefense
					}
				}
			},
			{
				ExtPropIndexes.MaxMDefense,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddDefense,
					ExtPropPercent = ExtPropIndexes.IncreaseMagDefense,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.6,
						0.84,
						0.72,
						0.76,
						0.0,
						0.88
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP9
					}
				}
			},
			{
				ExtPropIndexes.MinAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreasePhyAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Strength,
					Coefficient = new double[]
					{
						0.45599999999999996,
						0.0,
						0.48,
						0.504,
						0.0,
						0.0
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					}
				}
			},
			{
				ExtPropIndexes.MaxAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreasePhyAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Strength,
					Coefficient = new double[]
					{
						0.76,
						0.0,
						0.8,
						0.84,
						0.0,
						0.0
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddAttack
					}
				}
			},
			{
				ExtPropIndexes.MinMAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreaseMagAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Intelligence,
					Coefficient = new double[]
					{
						0.0,
						0.528,
						0.0,
						0.552,
						0.0,
						0.6
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					}
				}
			},
			{
				ExtPropIndexes.MaxMAttack,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.AddAttack,
					ExtPropPercent = ExtPropIndexes.IncreaseMagAttack,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Intelligence,
					Coefficient = new double[]
					{
						0.0,
						0.88,
						0.0,
						0.92,
						0.0,
						1.0
					},
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP1,
						ExcellencePorp.EXCELLENCEPORP2
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.TimeAddMAttack
					}
				}
			},
			{
				ExtPropIndexes.IncreasePhyAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP3,
						ExcellencePorp.EXCELLENCEPORP4,
						ExcellencePorp.EXCELLENCEPORP24
					}
				}
			},
			{
				ExtPropIndexes.IncreaseMagAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP3,
						ExcellencePorp.EXCELLENCEPORP4,
						ExcellencePorp.EXCELLENCEPORP24
					}
				}
			},
			{
				ExtPropIndexes.MaxLifeV,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.MaxLifePercent,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Constitution,
					Coefficient = new double[]
					{
						5.0,
						3.6,
						4.2,
						4.4,
						0.0,
						3.4
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.MU_ADDMAXHPVALUE
					}
				}
			},
			{
				ExtPropIndexes.MaxLifePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP8,
						ExcellencePorp.EXCELLENCEPORP20
					}
				}
			},
			{
				ExtPropIndexes.MaxMagicV,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.MaxMagicPercent,
					PropCoef = 1.0,
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.MU_ADDMAXMPVALUE
					}
				}
			},
			{
				ExtPropIndexes.MaxMagicPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Lucky,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.HitV,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.HitPercent,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.5,
						0.5,
						0.5,
						0.5,
						0.5,
						0.5
					}
				}
			},
			{
				ExtPropIndexes.Dodge,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.DodgePercent,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Dexterity,
					Coefficient = new double[]
					{
						0.5,
						0.5,
						0.5,
						0.5,
						0.5,
						0.5
					}
				}
			},
			{
				ExtPropIndexes.LifeRecoverPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.MU_ADDLIFERECOVERPERCENT
					}
				}
			},
			{
				ExtPropIndexes.LifeRecover,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.MagicRecover,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SubAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SubAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP5,
						ExcellencePorp.EXCELLENCEPORP21
					}
				}
			},
			{
				ExtPropIndexes.AddAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreDefensePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP14,
						ExcellencePorp.EXCELLENCEPORP26
					}
				}
			},
			{
				ExtPropIndexes.DamageThornPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP11,
						ExcellencePorp.EXCELLENCEPORP28
					}
				}
			},
			{
				ExtPropIndexes.DamageThorn,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.PhySkillIncreasePercent,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Intelligence,
					Coefficient = new double[]
					{
						1E-05,
						0.0,
						1E-05,
						1E-05,
						0.0,
						0.0
					}
				}
			},
			{
				ExtPropIndexes.MagicSkillIncreasePercent,
				new ExtPropItem
				{
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					UnitProp = UnitPropIndexes.Strength,
					Coefficient = new double[]
					{
						0.0,
						1E-05,
						0.0,
						1E-05,
						0.0,
						0.0001
					}
				}
			},
			{
				ExtPropIndexes.FatalAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP0,
						ExcellencePorp.EXCELLENCEPORP18
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.ADDTEMPFATALATTACK
					}
				}
			},
			{
				ExtPropIndexes.DoubleAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP23
					},
					BufferProp = new BufferItemTypes[]
					{
						BufferItemTypes.ADDTEMPDOUBLEATTACK
					}
				}
			},
			{
				ExtPropIndexes.DecreaseInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP10,
						ExcellencePorp.EXCELLENCEPORP22
					}
				}
			},
			{
				ExtPropIndexes.DecreaseInjureValue,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.CounteractInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.CounteractInjureValue,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreDefenseRate,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP7
					}
				}
			},
			{
				ExtPropIndexes.IncreasePhyDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddDefensePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP13,
						ExcellencePorp.EXCELLENCEPORP27
					}
				}
			},
			{
				ExtPropIndexes.IncreaseMagDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.AddDefensePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP13,
						ExcellencePorp.EXCELLENCEPORP27
					}
				}
			},
			{
				ExtPropIndexes.LifeSteal,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.LifeStealPercent,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateDingShen,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateMoveSpeed,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateJiTui,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.StateHunMi,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeLucky,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP29
					}
				}
			},
			{
				ExtPropIndexes.DeFatalAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP30
					}
				}
			},
			{
				ExtPropIndexes.DeDoubleAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP31
					}
				}
			},
			{
				ExtPropIndexes.HitPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP6,
						ExcellencePorp.EXCELLENCEPORP19
					}
				}
			},
			{
				ExtPropIndexes.DodgePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0,
					ExcellentProp = new ExcellencePorp[]
					{
						ExcellencePorp.EXCELLENCEPORP12,
						ExcellencePorp.EXCELLENCEPORP25
					}
				}
			},
			{
				ExtPropIndexes.FrozenPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.PalsyPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SpeedDownPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.BlowPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AutoRevivePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SavagePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.ColdPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.RuthlessPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.DeSavagePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.DeColdPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.DeRuthlessPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 100.0
				}
			},
			{
				ExtPropIndexes.LifeStealPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Potion,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FireAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IceAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FirePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IcePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeFirePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeWaterPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeLightningPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeSoilPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeIcePenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeWindPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Holywater,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RecoverLifeV,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RecoverMagicV,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Fatalhurt,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AddDefensePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.InjurePenetrationPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ElementInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnorePhyAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreMagyAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeFrozenPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DePalsyPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeSpeedDownPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.DeBlowPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.Toughness,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SPAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.AttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ElementAttackInjurePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WeaponEffect,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FireEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IceEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindEnhance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.FireReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WaterReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.LightningReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.SoilReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IceReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.WindReduce,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ElementPenetration,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ArmorMax,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ArmorPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ArmorRecover,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NaturePenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAttack,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDefense,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornPenetratePercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornAbsorbPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornWeakPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackPercent,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjure,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornAttack,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDefense,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornPenetratePercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornAbsorbPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornWeakPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackInjure,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.HolyRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ShadowRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.NatureRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.ChaosRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusRebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IncubusRebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.RebornDoubleAttackInjureResistance,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.RebornDoubleAttackInjureResistance,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			},
			{
				ExtPropIndexes.IgnoreDamageThornPercent,
				new ExtPropItem
				{
					UnitProp = UnitPropIndexes.Max,
					ExtProp = ExtPropIndexes.Max,
					ExtPropPercent = ExtPropIndexes.Max,
					PropCoef = 1.0
				}
			}
		};

		public static List<ExtPropIndexes> NotifyList = new List<ExtPropIndexes>
		{
			ExtPropIndexes.MinAttack,
			ExtPropIndexes.MaxAttack,
			ExtPropIndexes.MinDefense,
			ExtPropIndexes.MaxDefense,
			ExtPropIndexes.MinMAttack,
			ExtPropIndexes.MaxMAttack,
			ExtPropIndexes.MinMDefense,
			ExtPropIndexes.MaxMDefense,
			ExtPropIndexes.HitV,
			ExtPropIndexes.Dodge,
			ExtPropIndexes.AddAttackInjure,
			ExtPropIndexes.DecreaseInjureValue,
			ExtPropIndexes.MaxLifeV,
			ExtPropIndexes.MaxMagicV,
			ExtPropIndexes.LifeSteal,
			ExtPropIndexes.FireAttack,
			ExtPropIndexes.WaterAttack,
			ExtPropIndexes.LightningAttack,
			ExtPropIndexes.SoilAttack,
			ExtPropIndexes.IceAttack,
			ExtPropIndexes.WindAttack,
			ExtPropIndexes.AddAttack,
			ExtPropIndexes.IncreasePhyAttack,
			ExtPropIndexes.IncreaseMagAttack,
			ExtPropIndexes.AddAttackPercent,
			ExtPropIndexes.IncreasePhyDefense,
			ExtPropIndexes.AddDefense,
			ExtPropIndexes.AddDefensePercent,
			ExtPropIndexes.IncreaseMagDefense,
			ExtPropIndexes.HitPercent,
			ExtPropIndexes.DodgePercent,
			ExtPropIndexes.MaxLifePercent,
			ExtPropIndexes.MaxMagicPercent,
			ExtPropIndexes.LifeStealPercent,
			ExtPropIndexes.ArmorMax
		};
	}
}
