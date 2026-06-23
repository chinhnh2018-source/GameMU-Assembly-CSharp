using System;
using Server.Tools;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class CheatMgr
	{
		public CheatMgr()
		{
			this._HasCheatTimes = 0;
			this.ResetReferenceTicks();
		}

		public int HasCheatTimes
		{
			get
			{
				return this._HasCheatTimes;
			}
		}

		public double PatientTicks
		{
			get
			{
				return this._PatientTicks;
			}
			set
			{
				this._PatientTicks = value;
			}
		}

		public int PatientCount
		{
			get
			{
				return this._PatientCount;
			}
			set
			{
				this._PatientCount = value;
			}
		}

		public string ErrorStr
		{
			get
			{
				return this._Error;
			}
		}

		public bool CheckCheat()
		{
			if (this.CanDisconnectClient())
			{
				return false;
			}
			if ((double)(DateTime.Now.Ticks / 10000L) - this._LastCheckTicks < 1000.0)
			{
				return false;
			}
			this._LastCheckTicks = (double)(DateTime.Now.Ticks / 10000L);
			double num = (double)Time.realtimeSinceStartup - this._ReferenceTicks1;
			double num2 = (double)(DateTime.Now.Ticks / 10000L) - this._ReferenceTicks2;
			double lastSubReferenceTicks = this._LastSubReferenceTicks;
			this._LastSubReferenceTicks = Math.Abs(num - num2);
			if (this._LastSubReferenceTicks > this._PatientTicks && this._LastSubReferenceTicks > lastSubReferenceTicks)
			{
				this._HasCheatTimes++;
				this._Error = StringUtil.substitute("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
				{
					this._ReferenceTicks1,
					this._ReferenceTicks2,
					num,
					num2,
					num - num2,
					this._PatientTicks
				});
				this.ResetReferenceTicks();
				return true;
			}
			return false;
		}

		public bool CanDisconnectClient()
		{
			return this._HasCheatTimes >= this._PatientCount;
		}

		private void ResetReferenceTicks()
		{
			this._ReferenceTicks1 = (double)Time.realtimeSinceStartup;
			this._ReferenceTicks2 = (double)(DateTime.Now.Ticks / 10000L);
		}

		public static double LastClientServerSubTicks;

		public static int LastClientServerSubNum;

		private double _ReferenceTicks1;

		private double _ReferenceTicks2;

		private double _PatientTicks = 500.0;

		private int _HasCheatTimes;

		private int _PatientCount = 3;

		private double _LastCheckTicks;

		private double _LastSubReferenceTicks;

		private string _Error = string.Empty;
	}
}
