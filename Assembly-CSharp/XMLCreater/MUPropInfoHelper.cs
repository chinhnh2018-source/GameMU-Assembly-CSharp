using System;
using System.Collections.Generic;

namespace XMLCreater
{
	public class MUPropInfoHelper
	{
		public static List<MUPropInfo> DeserializeToPropList(string content)
		{
			List<MUPropInfo> list = new List<MUPropInfo>();
			string[] array = content.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				MUPropInfo mupropInfo = new MUPropInfo(array[i]);
				if (!(mupropInfo.PropName == string.Empty))
				{
					list.Add(mupropInfo);
				}
			}
			return list;
		}
	}
}
