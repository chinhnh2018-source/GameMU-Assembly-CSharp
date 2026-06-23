using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class StageSL : Canvas
	{
		public static int stageWidth
		{
			get
			{
				return Screen.width;
			}
		}

		public static int stageHeight
		{
			get
			{
				return Screen.height;
			}
		}
	}
}
