using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class CopyScoreDataInfo
	{
		public int CopyMapID { get; set; }

		public string ScoreName { get; set; }

		public int MinScore { get; set; }

		public int MaxScore { get; set; }

		public int ExpModulus { get; set; }

		public int MoneyModulus { get; set; }

		public int FallPacketID { get; set; }
	}
}
