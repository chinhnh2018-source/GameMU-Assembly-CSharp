using System;
using HSGameEngine.Drawing;

namespace HSGameEngine.GameEngine.Data
{
	public class LocalAutoFightData
	{
		public LocalAutoFightData()
		{
			this.Fighting = false;
			this.CancelFightingNum = 0;
			this.FightPoint = new Point(0, 0);
			this.FightStartTicks = 0L;
			this.LastCheckBufferTicks = 0L;
			this.LastCheckNormalDrugsTicks = 0L;
			this.FightRadius = 100;
			this.MaxFightSecs = 604800;
			this.AutoGetEquips = true;
			this.AutoGetThings = true;
			this.AutoGetMoney = true;
			this.AutoGetDrugs = false;
			this.AutoGoBack = false;
			this.AutoGoBackWhenNoLifeDrugs = false;
			this.AutoGoBackWhenNoMagicDrugs = false;
			this.DontAttackBigBoss = false;
			this.AutoRealive = false;
			this.AutoUseExpCard = true;
			this.AutoUseLifeReserveDrugs = true;
			this.AutoUseMagicReserveDrugs = true;
			this.AutoAntiAttack = true;
			this.AutoOpenLieHuoJianQi = true;
			this.AutoOpenFSHunDun = true;
			this.AutoZhaohuanShenshou = true;
			this.LifeLessThanXDo = false;
			this.MagicLessThanXDo = false;
			this.LifeLessThanX = 60;
			this.MagicLessThanX = 60;
			this.LifeLessThanXAutoUse = 0;
			this.MagicLessThanXAutoUse = 0;
			this.AutoBuyMedicine = true;
			this.RoleID = 0;
			this.SkillPriority = null;
			this.IsUseDefaultSkillList = true;
			this.SkillPriorityDict = null;
			this.SkillID = -1;
		}

		public int CancelFightingNum;

		public bool Fighting;

		public Point FightPoint;

		public long FightStartTicks;

		public long LastCheckBufferTicks;

		public long LastCheckNormalDrugsTicks;

		public int FightRadius = 100;

		public int MaxFightSecs;

		public bool AutoGetEquips;

		public bool AutoGetMoney;

		public bool AutoGetThings;

		public bool AutoGetDrugs;

		public bool AutoGoBack;

		public bool AutoGoBackWhenNoLifeDrugs;

		public bool AutoGoBackWhenNoMagicDrugs;

		public bool DontAttackBigBoss;

		public bool AutoRealive;

		public bool AutoUseExpCard;

		public bool AutoUseLifeReserveDrugs;

		public bool AutoUseMagicReserveDrugs;

		public bool AutoAntiAttack;

		public bool AutoBuyMedicine;

		public bool AutoOpenLieHuoJianQi;

		public bool AutoOpenFSHunDun;

		public bool AutoZhaohuanShenshou;

		public bool LifeLessThanXDo = true;

		public bool MagicLessThanXDo = true;

		public int LifeLessThanX;

		public int MagicLessThanX;

		public int LifeLessThanXAutoUse;

		public int MagicLessThanXAutoUse;

		public bool IsGoingBackOnOutOfRange;

		public bool IsOnlineGuaJi = true;

		public int Multi;

		public float TimeMax = 24f;

		public long RemainExper;

		public long RemainExperMax;

		public long GoldUsed;

		public long GoldPerHour;

		public bool Color_Zi = true;

		public bool Color_Lan = true;

		public bool Color_Lv = true;

		public bool Color_Bai = true;

		public bool BaoShi = true;

		public bool YuMao = true;

		public bool YaoPin = true;

		public bool JinBi = true;

		public bool MenPiaoCaiLiao = true;

		public bool QiTaDaoJu = true;

		public int AutoPickThingFlags = -1;

		public int SkillID = -1;

		public int RoleID;

		public string SkillPriority;

		public bool IsUseDefaultSkillList = true;

		public string SkillPriorityDict;
	}
}
