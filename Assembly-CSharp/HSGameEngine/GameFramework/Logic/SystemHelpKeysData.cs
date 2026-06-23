using System;
using UnityEngine;

namespace HSGameEngine.GameFramework.Logic
{
	public class SystemHelpKeysData
	{
		public int ID;

		public int GropID;

		public int[] PrevIDs;

		public int[] NextIDs;

		public int OccupCondition;

		public int TriggerCondition;

		public int TimeParameters;

		public int Dark;

		public uint EventMask;

		public SystemHelpPartStates State;

		public int[] ItemIDs;

		public int[] ItemValues;

		public string Deco1;

		public Vector2 Pos1;

		public string Deco2;

		public Vector2 Pos2;

		public string HintText;

		public int HintType;

		public int HintID;

		public int PostTaskPlotID;

		public int PostOpenID;

		public bool Active;
	}
}
