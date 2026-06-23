using System;
using GameServer.Interface;
using GameServer.Logic.JingJiChang;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic.MerlinMagicBook
{
	public class MerlinInjureManager
	{
		public int CalcMerlinInjure(IObject attacker, IObject defender, int nBaseInjure, ref EMerlinSecretAttrType eref)
		{
			eref = EMerlinSecretAttrType.EMSAT_None;
			try
			{
				if (!(attacker is GameClient) && !(attacker is Robot))
				{
					return 0;
				}
				if (!(defender is GameClient) && !(defender is Robot) && !(defender is Monster))
				{
					return 0;
				}
				if (nBaseInjure <= 0)
				{
					return 0;
				}
				double percent = 1.0;
				eref = this.GetMerlinInjureType(attacker, defender, ref percent);
				if (eref == EMerlinSecretAttrType.EMSAT_None)
				{
					return 0;
				}
				return this.TriggerEffect(attacker, defender, nBaseInjure, eref, percent);
			}
			catch (Exception e)
			{
				if (attacker is GameClient)
				{
					GameClient gameClient = attacker as GameClient;
					if (null != gameClient)
					{
						DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(gameClient.ClientSocket), false, false);
					}
				}
			}
			return 0;
		}

		private EMerlinSecretAttrType GetMerlinInjureType(IObject attacker, IObject defender, ref double percent)
		{
			try
			{
				double merlinInjurePercent = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_SpeedDownP);
				double merlinInjurePercent2 = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_FrozenP);
				double merlinInjurePercent3 = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_BlowP);
				double merlinInjurePercent4 = this.GetMerlinInjurePercent(attacker, defender, EMerlinSecretAttrType.EMSAT_PalsyP);
				double[] rateArr = new double[]
				{
					merlinInjurePercent,
					merlinInjurePercent2,
					merlinInjurePercent3,
					merlinInjurePercent4
				};
				switch (RoleAlgorithm.GetRateIndexPercent(rateArr))
				{
				case 0:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 58);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_SpeedDownP;
				case 1:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 56);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_FrozenP;
				case 2:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 59);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_BlowP;
				case 3:
					percent = DeControl.getInstance().OnControl(defender as GameClient, 57);
					if (percent <= 0.0)
					{
						return EMerlinSecretAttrType.EMSAT_None;
					}
					return EMerlinSecretAttrType.EMSAT_PalsyP;
				default:
					return EMerlinSecretAttrType.EMSAT_None;
				}
			}
			catch (Exception e)
			{
				if (attacker is GameClient)
				{
					GameClient gameClient = attacker as GameClient;
					if (null != gameClient)
					{
						DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(gameClient.ClientSocket), false, false);
					}
				}
			}
			return EMerlinSecretAttrType.EMSAT_None;
		}

		public double GetMerlinInjurePercent(IObject attacker, IObject defender, EMerlinSecretAttrType eType)
		{
			double num = 0.0;
			try
			{
				switch (eType)
				{
				case EMerlinSecretAttrType.EMSAT_FrozenP:
					num += RoleAlgorithm.GetFrozenPercent(attacker);
					break;
				case EMerlinSecretAttrType.EMSAT_PalsyP:
					num += RoleAlgorithm.GetPalsyPercent(attacker);
					break;
				case EMerlinSecretAttrType.EMSAT_SpeedDownP:
					num += RoleAlgorithm.GetSpeedDownPercent(attacker);
					break;
				case EMerlinSecretAttrType.EMSAT_BlowP:
					num += RoleAlgorithm.GetBlowPercent(attacker);
					break;
				}
				if (defender is Robot || defender is GameClient)
				{
					switch (eType)
					{
					case EMerlinSecretAttrType.EMSAT_FrozenP:
						num -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 97) : (defender as Robot).DeFrozenPercent);
						break;
					case EMerlinSecretAttrType.EMSAT_PalsyP:
						num -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 98) : (defender as Robot).DePalsyPercent);
						break;
					case EMerlinSecretAttrType.EMSAT_SpeedDownP:
						num -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 99) : (defender as Robot).DeSpeedDownPercent);
						break;
					case EMerlinSecretAttrType.EMSAT_BlowP:
						num -= ((defender is GameClient) ? RoleAlgorithm.GetExtProp(defender as GameClient, 100) : (defender as Robot).DeBlowPercent);
						break;
					}
				}
				return (num > 0.0) ? num : 0.0;
			}
			catch (Exception e)
			{
				if (attacker is GameClient)
				{
					GameClient gameClient = attacker as GameClient;
					if (null != gameClient)
					{
						DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(gameClient.ClientSocket), false, false);
					}
				}
			}
			return (num > 0.0) ? num : 0.0;
		}

		private int TriggerEffect(IObject attacker, IObject defender, int nBaseInjure, EMerlinSecretAttrType eType, double percent)
		{
			int result = 0;
			try
			{
				switch (eType)
				{
				case EMerlinSecretAttrType.EMSAT_FrozenP:
				{
					result = (int)((double)nBaseInjure * 0.5);
					double[] actionParams = new double[]
					{
						0.99,
						2.0 * percent
					};
					MagicAction.ProcessAction(attacker, defender, MagicActionIDs.MU_ADD_FROZEN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
					break;
				}
				case EMerlinSecretAttrType.EMSAT_PalsyP:
				{
					result = (int)((double)nBaseInjure * 0.5);
					double[] actionParams = new double[]
					{
						0.99,
						1.0 * percent
					};
					MagicAction.ProcessAction(attacker, defender, MagicActionIDs.MU_ADD_PALSY, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
					break;
				}
				case EMerlinSecretAttrType.EMSAT_SpeedDownP:
				{
					result = (int)((double)nBaseInjure * 0.5);
					double[] actionParams = new double[]
					{
						0.3,
						4.0 * percent
					};
					MagicAction.ProcessAction(attacker, defender, MagicActionIDs.MU_ADD_MOVE_SPEED_DOWN, actionParams, -1, -1, 0, 1, -1, 0, 0, -1, 0, false, false, 1.0, 1, 0.0);
					break;
				}
				case EMerlinSecretAttrType.EMSAT_BlowP:
					result = nBaseInjure;
					break;
				default:
					return 0;
				}
				return result;
			}
			catch (Exception e)
			{
				if (attacker is GameClient)
				{
					GameClient gameClient = attacker as GameClient;
					if (null != gameClient)
					{
						DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(gameClient.ClientSocket), false, false);
					}
				}
			}
			return result;
		}
	}
}
