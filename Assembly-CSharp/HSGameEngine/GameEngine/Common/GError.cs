using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.Common
{
	public class GError
	{
		public static List<string> GetErrMsgList()
		{
			return GError._ErrMsgList;
		}

		private static void ClearErrMsgList()
		{
			if (GError._ErrMsgList.Count > 150)
			{
				while (GError._ErrMsgList.Count > 100)
				{
					GError._ErrMsgList.RemoveAt(0);
				}
			}
		}

		public static void AddErrMsg(string errMsg)
		{
			GError._ErrMsgList.Add(errMsg);
			GError.ClearErrMsgList();
		}

		public static void AddErrMsg(Exception ex)
		{
			GError.AddErrMsg(ex.Message);
		}

		public static void AddErrMsg2(string errMsg)
		{
			GError.AddErrMsg(errMsg);
		}

		public static void AddErrMsg2(Exception ex)
		{
			GError.AddErrMsg(ex);
		}

		private static List<string> _ErrMsgList = new List<string>();
	}
}
