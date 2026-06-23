using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Common
{
	public class GException
	{
		public static List<string> GetExceptionMsgList()
		{
			return GException._ExceptionMsgList;
		}

		public static void ClearAllExceptionMsgList()
		{
			if (GException._ExceptionMsgList.Count > 0)
			{
				GException._ExceptionMsgList.Clear();
				GException._ExceptionMsgKeyList.Clear();
				GException._ExceptionMsgDict.Clear();
			}
		}

		private static void ClearExceptionMsgList()
		{
			if (GException._ExceptionMsgList.Count > 150)
			{
				while (GException._ExceptionMsgList.Count > 100)
				{
					GException._ExceptionMsgList.RemoveAt(0);
					string text = GException._ExceptionMsgKeyList[0];
					GException._ExceptionMsgKeyList.RemoveAt(0);
					GException._ExceptionMsgDict.Remove(text);
				}
			}
		}

		public static void AddExceptionMsg(Exception ex)
		{
		}

		private static List<string> _ExceptionMsgList = new List<string>();

		private static List<string> _ExceptionMsgKeyList = new List<string>();

		private static Dictionary<string, int> _ExceptionMsgDict = new Dictionary<string, int>();
	}
}
