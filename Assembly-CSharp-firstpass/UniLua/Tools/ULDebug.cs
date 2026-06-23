using System;
using UnityEngine;

namespace UniLua.Tools
{
	public class ULDebug
	{
		static ULDebug()
		{
			ULDebug.Log = new Action<object>(Debug.Log);
			ULDebug.LogError = new Action<object>(Debug.LogError);
		}

		private static void NoAction(object msg)
		{
		}

		public static Action<object> Log = new Action<object>(ULDebug.NoAction);

		public static Action<object> LogError = new Action<object>(ULDebug.NoAction);
	}
}
