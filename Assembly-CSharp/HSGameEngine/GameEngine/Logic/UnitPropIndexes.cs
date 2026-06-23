using System;

namespace HSGameEngine.GameEngine.Logic
{
	public static class UnitPropIndexes
	{
		public const int Strength = 0;

		public const int Intelligence = 1;

		public const int Dexterity = 2;

		public const int Constitution = 3;

		public const int Max = 4;

		public static readonly string[] Names = new string[]
		{
			"力量",
			"智力",
			"敏捷",
			"体力"
		};

		public static readonly string[] UnitPropIndexeNames = new string[]
		{
			Global.GetLang("力量"),
			Global.GetLang("智力"),
			Global.GetLang("敏捷"),
			Global.GetLang("体力")
		};
	}
}
