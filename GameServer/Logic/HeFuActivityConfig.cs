using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class HeFuActivityConfig
	{
		public bool InList(int type)
		{
			foreach (int num in this.openList)
			{
				if (num == type)
				{
					return true;
				}
			}
			return false;
		}

		public List<int> openList = new List<int>();
	}
}
