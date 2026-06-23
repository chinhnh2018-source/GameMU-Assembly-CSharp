using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class BtnCD
	{
		public BtnCD(int instanceID, float Cd, long cDBeginTicks)
		{
			this.InstanceID = instanceID;
			this.CD = Cd;
			this.CDBeginTicks = cDBeginTicks;
		}

		public int InstanceID;

		public float CD;

		public long CDBeginTicks;
	}
}
