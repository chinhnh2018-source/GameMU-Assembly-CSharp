using System;
using System.Text;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;

namespace GameServer.Logic.ElementsAttack
{
	public class ElementsAttackManager
	{
		public string CalcElementInjureLog(IObject attacker, IObject defender, double injurePercent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			double passiveEffectAddPercent = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 1);
			double passiveEffectAddPercent2 = PassiveEffectManager.GetPassiveEffectAddPercent(defender, 5, 2);
			double passiveEffectAddPercent3 = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 3);
			for (int i = 1; i <= 6; i++)
			{
				double num = (double)this.CalcElementDamage(attacker, defender, (EElementDamageType)i, passiveEffectAddPercent, passiveEffectAddPercent2, passiveEffectAddPercent3) * injurePercent;
				stringBuilder.AppendFormat("{0}:{1}  ", ElementsAttackManager.ElementAttrName[i], num);
			}
			return stringBuilder.ToString();
		}

		public int CalcAllElementDamage(IObject attacker, IObject defender)
		{
			int result;
			if (!(attacker is GameClient) && !(attacker is Robot))
			{
				result = 0;
			}
			else if (!(defender is GameClient) && !(defender is Robot) && !(defender is Monster))
			{
				result = 0;
			}
			else
			{
				double passiveEffectAddPercent = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 1);
				double passiveEffectAddPercent2 = PassiveEffectManager.GetPassiveEffectAddPercent(defender, 5, 2);
				double passiveEffectAddPercent3 = PassiveEffectManager.GetPassiveEffectAddPercent(attacker, 4, 3);
				int num = 0;
				for (int i = 1; i <= 6; i++)
				{
					num += this.CalcElementDamage(attacker, defender, (EElementDamageType)i, passiveEffectAddPercent, passiveEffectAddPercent2, passiveEffectAddPercent3);
				}
				result = num;
			}
			return result;
		}

		private int CalcElementDamage(IObject attacker, IObject defender, EElementDamageType eEDT, double dmgAddPercent = 0.0, double dmgSubPercent = 0.0, double penetAddPercent = 0.0)
		{
			double num = this.GetElementDamagePenetration(attacker, eEDT) + penetAddPercent;
			double deElementDamagePenetration = this.GetDeElementDamagePenetration(defender, eEDT);
			double elementEnhance = this.GetElementEnhance(attacker, eEDT);
			double deElementReduce = this.GetDeElementReduce(defender, eEDT);
			double num2 = 1.0 + (num - deElementDamagePenetration);
			double num3 = Global.GMax(0.0, 1.0 + elementEnhance - deElementReduce);
			num3 = Global.GMin(2.0, num3);
			num2 = Global.GMax(0.01, num2);
			num2 = Global.GMin(1.0, num2);
			num2 *= num3;
			int num4 = this.GetElementAttack(attacker, eEDT);
			num4 = (int)((double)num4 * num2);
			if (attacker.ObjectType == ObjectTypes.OT_CLIENT)
			{
				double num5 = 1.0 + RoleAlgorithm.GetExtPropValue(attacker as GameClient, ExtPropIndexes.ElementInjurePercent);
				if (defender.ObjectType == ObjectTypes.OT_CLIENT)
				{
					num5 -= RoleAlgorithm.GetExtPropValue(defender as GameClient, ExtPropIndexes.ElementAttackInjurePercent);
					num5 -= dmgSubPercent;
				}
				num5 += dmgAddPercent;
				num4 = (int)((double)num4 * num5);
			}
			return (num4 > 0) ? num4 : 0;
		}

		public double GetElementDamagePenetration(IObject attacker, EElementDamageType eEDT)
		{
			double num = 0.0;
			if (attacker is GameClient)
			{
				GameClient gameClient = attacker as GameClient;
				if (null == gameClient)
				{
					return num;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					num += gameClient.ClientData.EquipProp.ExtProps[75];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(75);
					num += RoleAlgorithm.GetExtProp(gameClient, 118);
					break;
				case EElementDamageType.EEDT_Water:
					num += gameClient.ClientData.EquipProp.ExtProps[76];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(76);
					num += RoleAlgorithm.GetExtProp(gameClient, 118);
					break;
				case EElementDamageType.EEDT_Lightning:
					num += gameClient.ClientData.EquipProp.ExtProps[77];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(77);
					num += RoleAlgorithm.GetExtProp(gameClient, 118);
					break;
				case EElementDamageType.EEDT_Soil:
					num += gameClient.ClientData.EquipProp.ExtProps[78];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(78);
					num += RoleAlgorithm.GetExtProp(gameClient, 118);
					break;
				case EElementDamageType.EEDT_Ice:
					num += gameClient.ClientData.EquipProp.ExtProps[79];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(79);
					num += RoleAlgorithm.GetExtProp(gameClient, 118);
					break;
				case EElementDamageType.EEDT_Wind:
					num += gameClient.ClientData.EquipProp.ExtProps[80];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(80);
					num += RoleAlgorithm.GetExtProp(gameClient, 118);
					break;
				}
			}
			else if (attacker is Robot)
			{
				Robot robot = attacker as Robot;
				if (null == robot)
				{
					return num;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					num = robot.FirePenetration;
					num += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Water:
					num = robot.WaterPenetration;
					num += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Lightning:
					num = robot.LightningPenetration;
					num += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Soil:
					num = robot.SoilPenetration;
					num += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Ice:
					num = robot.IcePenetration;
					num += (attacker as Robot).ElementPenetration;
					break;
				case EElementDamageType.EEDT_Wind:
					num = robot.WindPenetration;
					num += (attacker as Robot).ElementPenetration;
					break;
				}
			}
			return Math.Max(num, 0.0);
		}

		public double GetDeElementDamagePenetration(IObject defender, EElementDamageType eEDT)
		{
			double num = 0.0;
			if (defender is GameClient)
			{
				GameClient gameClient = defender as GameClient;
				if (null == gameClient)
				{
					return num;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					num += gameClient.ClientData.EquipProp.ExtProps[81];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(81);
					break;
				case EElementDamageType.EEDT_Water:
					num += gameClient.ClientData.EquipProp.ExtProps[82];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(82);
					break;
				case EElementDamageType.EEDT_Lightning:
					num += gameClient.ClientData.EquipProp.ExtProps[83];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(83);
					break;
				case EElementDamageType.EEDT_Soil:
					num += gameClient.ClientData.EquipProp.ExtProps[84];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(84);
					break;
				case EElementDamageType.EEDT_Ice:
					num += gameClient.ClientData.EquipProp.ExtProps[85];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(85);
					break;
				case EElementDamageType.EEDT_Wind:
					num += gameClient.ClientData.EquipProp.ExtProps[86];
					num += gameClient.ClientData.PropsCacheManager.GetExtProp(86);
					break;
				}
			}
			else if (defender is Robot)
			{
				Robot robot = defender as Robot;
				if (null == robot)
				{
					return num;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					num = robot.DeFirePenetration;
					break;
				case EElementDamageType.EEDT_Water:
					num = robot.DeWaterPenetration;
					break;
				case EElementDamageType.EEDT_Lightning:
					num = robot.DeLightningPenetration;
					break;
				case EElementDamageType.EEDT_Soil:
					num = robot.DeSoilPenetration;
					break;
				case EElementDamageType.EEDT_Ice:
					num = robot.DeIcePenetration;
					break;
				case EElementDamageType.EEDT_Wind:
					num = robot.DeWindPenetration;
					break;
				}
			}
			return Math.Max(num, 0.0);
		}

		public int GetElementAttack(IObject attacker, EElementDamageType eEDT)
		{
			int num = 0;
			if (attacker is GameClient)
			{
				GameClient gameClient = attacker as GameClient;
				if (null == gameClient)
				{
					return num;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					num += (int)gameClient.ClientData.EquipProp.ExtProps[69];
					num += (int)gameClient.ClientData.PropsCacheManager.GetExtProp(69);
					break;
				case EElementDamageType.EEDT_Water:
					num += (int)gameClient.ClientData.EquipProp.ExtProps[70];
					num += (int)gameClient.ClientData.PropsCacheManager.GetExtProp(70);
					break;
				case EElementDamageType.EEDT_Lightning:
					num += (int)gameClient.ClientData.EquipProp.ExtProps[71];
					num += (int)gameClient.ClientData.PropsCacheManager.GetExtProp(71);
					break;
				case EElementDamageType.EEDT_Soil:
					num += (int)gameClient.ClientData.EquipProp.ExtProps[72];
					num += (int)gameClient.ClientData.PropsCacheManager.GetExtProp(72);
					break;
				case EElementDamageType.EEDT_Ice:
					num += (int)gameClient.ClientData.EquipProp.ExtProps[73];
					num += (int)gameClient.ClientData.PropsCacheManager.GetExtProp(73);
					break;
				case EElementDamageType.EEDT_Wind:
					num += (int)gameClient.ClientData.EquipProp.ExtProps[74];
					num += (int)gameClient.ClientData.PropsCacheManager.GetExtProp(74);
					break;
				}
			}
			else if (attacker is Robot)
			{
				Robot robot = attacker as Robot;
				if (null == robot)
				{
					return num;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					num = robot.FireAttack;
					break;
				case EElementDamageType.EEDT_Water:
					num = robot.WaterAttack;
					break;
				case EElementDamageType.EEDT_Lightning:
					num = robot.LightningAttack;
					break;
				case EElementDamageType.EEDT_Soil:
					num = robot.SoilAttack;
					break;
				case EElementDamageType.EEDT_Ice:
					num = robot.IceAttack;
					break;
				case EElementDamageType.EEDT_Wind:
					num = robot.WindAttack;
					break;
				}
			}
			return Math.Max(num, 0);
		}

		public double GetElementEnhance(IObject attacker, EElementDamageType eEDT)
		{
			double result = 0.0;
			if (attacker is GameClient)
			{
				GameClient gameClient = attacker as GameClient;
				if (null == gameClient)
				{
					return result;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					result = RoleAlgorithm.GetExtProp(gameClient, 106);
					break;
				case EElementDamageType.EEDT_Water:
					result = RoleAlgorithm.GetExtProp(gameClient, 107);
					break;
				case EElementDamageType.EEDT_Lightning:
					result = RoleAlgorithm.GetExtProp(gameClient, 108);
					break;
				case EElementDamageType.EEDT_Soil:
					result = RoleAlgorithm.GetExtProp(gameClient, 109);
					break;
				case EElementDamageType.EEDT_Ice:
					result = RoleAlgorithm.GetExtProp(gameClient, 110);
					break;
				case EElementDamageType.EEDT_Wind:
					result = RoleAlgorithm.GetExtProp(gameClient, 111);
					break;
				}
			}
			return result;
		}

		public double GetDeElementReduce(IObject attacker, EElementDamageType eEDT)
		{
			double result = 0.0;
			if (attacker is GameClient)
			{
				GameClient gameClient = attacker as GameClient;
				if (null == gameClient)
				{
					return result;
				}
				switch (eEDT)
				{
				case EElementDamageType.EEDT_Fire:
					result = RoleAlgorithm.GetExtProp(gameClient, 112);
					break;
				case EElementDamageType.EEDT_Water:
					result = RoleAlgorithm.GetExtProp(gameClient, 113);
					break;
				case EElementDamageType.EEDT_Lightning:
					result = RoleAlgorithm.GetExtProp(gameClient, 114);
					break;
				case EElementDamageType.EEDT_Soil:
					result = RoleAlgorithm.GetExtProp(gameClient, 115);
					break;
				case EElementDamageType.EEDT_Ice:
					result = RoleAlgorithm.GetExtProp(gameClient, 116);
					break;
				case EElementDamageType.EEDT_Wind:
					result = RoleAlgorithm.GetExtProp(gameClient, 117);
					break;
				}
			}
			return result;
		}

		public double GetJJCRobotExtProps(int nIndex, double[] extProps)
		{
			double result;
			if (nIndex > extProps.Length - 1)
			{
				result = 0.0;
			}
			else
			{
				result = extProps[nIndex];
			}
			return result;
		}

		public const bool LogElementInjure = false;

		public static readonly string[] ElementAttrName = new string[]
		{
			"unknown",
			"火伤",
			"水伤",
			"雷伤",
			"土伤",
			"冰伤",
			"风伤"
		};
	}
}
