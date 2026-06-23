using System;
using GameServer.Interface;

namespace GameServer.Logic
{
	public class StateRate
	{
		public static double GetStateDingShengRate(IObject self, IObject obj, double selfBaseRate, double objBaseRate)
		{
			double num;
			if (self is GameClient)
			{
				num = selfBaseRate + RoleAlgorithm.GetRoleStateDingSheng(self as GameClient, selfBaseRate);
			}
			else if (self is Monster)
			{
				num = selfBaseRate + 0.1 * (double)(self as Monster).MonsterInfo.ChangeLifeCount;
			}
			else
			{
				num = selfBaseRate;
			}
			double num2;
			if (obj is GameClient)
			{
				num2 = objBaseRate + RoleAlgorithm.GetRoleStateDingSheng(obj as GameClient, objBaseRate);
			}
			else if (obj is Monster)
			{
				num2 = objBaseRate + 0.1 * (double)(obj as Monster).MonsterInfo.ChangeLifeCount;
			}
			else if (obj is FakeRoleItem)
			{
				num2 = selfBaseRate + 0.1 * (double)(obj as FakeRoleItem).GetFakeRoleData().MyRoleDataMini.ChangeLifeCount;
			}
			else
			{
				num2 = 0.0;
			}
			return num - num2;
		}

		public static double GetStateMoveSpeed(IObject self, IObject obj, double selfBaseRate, double objBaseRate)
		{
			double num;
			if (self is GameClient)
			{
				num = selfBaseRate + RoleAlgorithm.GetRoleStateMoveSpeed(self as GameClient, selfBaseRate);
			}
			else if (self is Monster)
			{
				num = selfBaseRate + 0.1 * (double)(self as Monster).MonsterInfo.ChangeLifeCount;
			}
			else
			{
				num = selfBaseRate;
			}
			double num2;
			if (obj is GameClient)
			{
				num2 = objBaseRate + RoleAlgorithm.GetRoleStateMoveSpeed(obj as GameClient, objBaseRate);
			}
			else if (obj is Monster)
			{
				num2 = objBaseRate + 0.1 * (double)(obj as Monster).MonsterInfo.ChangeLifeCount;
			}
			else if (obj is FakeRoleItem)
			{
				num2 = selfBaseRate + 0.1 * (double)(obj as FakeRoleItem).GetFakeRoleData().MyRoleDataMini.ChangeLifeCount;
			}
			else
			{
				num2 = 0.0;
			}
			return num - num2;
		}

		public static double GetNegativeRate(IObject self, IObject obj, double baseRate, ExtPropIndexes extPropIndex, MagicActionIDs actionId)
		{
			int num = 0;
			if (self is GameClient)
			{
				num = (self as GameClient).ClientData.ChangeLifeCount;
				baseRate = RoleAlgorithm.GetRoleNegativeRate(self as GameClient, baseRate, extPropIndex);
			}
			else if (self is Monster)
			{
				num = (self as Monster).MonsterInfo.ChangeLifeCount;
			}
			int num2 = 0;
			if (obj is GameClient)
			{
				if ((obj as GameClient).buffManager.IsBuffEnabled(116))
				{
					return 0.0;
				}
				if (actionId != MagicActionIDs.MU_ADD_JITUI && (obj as GameClient).buffManager.IsBuffEnabled(113))
				{
					return 0.0;
				}
				if (CaiJiLogic.IsCaiJiState(obj as GameClient) && (extPropIndex == ExtPropIndexes.StateDingShen || extPropIndex == ExtPropIndexes.StateMoveSpeed || extPropIndex == ExtPropIndexes.StateJiTui || extPropIndex == ExtPropIndexes.StateHunMi))
				{
					return 0.0;
				}
				num2 = (obj as GameClient).ClientData.ChangeLifeCount;
			}
			else if (obj is Monster)
			{
				num2 = (obj as Monster).MonsterInfo.ChangeLifeCount;
			}
			else if (obj is FakeRoleItem)
			{
				num2 = (obj as FakeRoleItem).GetFakeRoleData().MyRoleDataMini.ChangeLifeCount;
			}
			double result;
			if (num > num2)
			{
				result = baseRate + 0.1 * Math.Pow((double)(num - num2), 2.0);
			}
			else
			{
				result = baseRate - 0.1 * Math.Pow((double)(num - num2), 2.0);
			}
			return result;
		}

		public static double GetStateJiTui(IObject self, IObject obj, double selfBaseRate, double objBaseRate)
		{
			double num;
			if (self is GameClient)
			{
				num = selfBaseRate + RoleAlgorithm.GetRoleStateJiTui(self as GameClient, selfBaseRate);
			}
			else if (self is Monster)
			{
				num = selfBaseRate + 0.1 * (double)(self as Monster).MonsterInfo.ChangeLifeCount;
			}
			else
			{
				num = selfBaseRate;
			}
			double num2;
			if (obj is GameClient)
			{
				num2 = objBaseRate + RoleAlgorithm.GetRoleStateJiTui(obj as GameClient, objBaseRate);
			}
			else if (obj is Monster)
			{
				num2 = objBaseRate + 0.1 * (double)(obj as Monster).MonsterInfo.ChangeLifeCount;
			}
			else if (obj is FakeRoleItem)
			{
				num2 = selfBaseRate + 0.1 * (double)(obj as FakeRoleItem).GetFakeRoleData().MyRoleDataMini.ChangeLifeCount;
			}
			else
			{
				num2 = 0.0;
			}
			return num - num2;
		}

		public static double GetStateHunMi(IObject self, IObject obj, double selfBaseRate, double objBaseRate)
		{
			double num;
			if (self is GameClient)
			{
				num = selfBaseRate + RoleAlgorithm.GetRoleStateHunMi(self as GameClient, selfBaseRate);
			}
			else if (self is Monster)
			{
				num = selfBaseRate + 0.1 * (double)(self as Monster).MonsterInfo.ChangeLifeCount;
			}
			else
			{
				num = selfBaseRate;
			}
			double num2;
			if (obj is GameClient)
			{
				num2 = objBaseRate + RoleAlgorithm.GetRoleStateHunMi(obj as GameClient, objBaseRate);
			}
			else if (obj is Monster)
			{
				num2 = objBaseRate + 0.1 * (double)(obj as Monster).MonsterInfo.ChangeLifeCount;
			}
			else if (obj is FakeRoleItem)
			{
				num2 = selfBaseRate + 0.1 * (double)(obj as FakeRoleItem).GetFakeRoleData().MyRoleDataMini.ChangeLifeCount;
			}
			else
			{
				num2 = 0.0;
			}
			return num - num2;
		}
	}
}
