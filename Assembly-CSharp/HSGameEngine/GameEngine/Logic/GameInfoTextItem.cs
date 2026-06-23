using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class GameInfoTextItem
	{
		public GameInfoTypeIndexes GameInfoTypeIndex { get; set; }

		public ShowGameInfoTypes ShowGameInfoType { get; set; }

		public string TextMsg { get; set; }

		public long Ticks { get; set; }

		public int ErrCode { get; set; }

		public int ToX { get; set; }

		public int ToY { get; set; }

		public int ToBuyGoodsID { get; set; }
	}
}
