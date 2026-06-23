using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public struct GridMagicHelperItemKey : IComparer<GridMagicHelperItemKey>
	{
		public int Compare(GridMagicHelperItemKey x, GridMagicHelperItemKey y)
		{
			int num = x.MapCode - y.MapCode;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				num = x.CopyMapID - y.CopyMapID;
				if (num != 0)
				{
					result = num;
				}
				else
				{
					num = x.PosX - y.PosX;
					if (num != 0)
					{
						result = num;
					}
					else
					{
						num = x.PosY - y.PosY;
						if (num != 0)
						{
							result = num;
						}
						else
						{
							num = x.MagicActionID - y.MagicActionID;
							if (num != 0)
							{
								result = num;
							}
							else
							{
								num = x.MagicActionID2 - y.MagicActionID2;
								result = num;
							}
						}
					}
				}
			}
			return result;
		}

		public static GridMagicHelperItemKey Comparer = default(GridMagicHelperItemKey);

		public int MapCode;

		public int PosX;

		public int PosY;

		public int CopyMapID;

		public MagicActionIDs MagicActionID;

		public MagicActionIDs MagicActionID2;
	}
}
