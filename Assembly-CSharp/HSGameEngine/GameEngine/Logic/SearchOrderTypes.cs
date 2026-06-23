using System;

namespace HSGameEngine.GameEngine.Logic
{
	public enum SearchOrderTypes
	{
		OrderByMoney = 1,
		OrderByMoneyPerItem,
		OrderBySuit = 4,
		OrderByNameAndColor = 8,
		Max = 8
	}
}
