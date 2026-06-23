using System;
using System.Collections.Generic;

public class CommonFlagX
{
	public static bool HaveFlagData(string name)
	{
		return CommonFlagX.XDic.ContainsKey(name);
	}

	public static void AddFlagData(CommonFlagX.CommonFlagData fd)
	{
		if (fd.name.Equals(string.Empty))
		{
			return;
		}
		if (CommonFlagX.XDic.ContainsKey(fd.name))
		{
			CommonFlagX.XDic[fd.name] = fd;
		}
		else
		{
			CommonFlagX.XDic.Add(fd.name, fd);
		}
	}

	public static CommonFlagX.CommonFlagData GetFlagData(string name)
	{
		if (name.Equals(string.Empty))
		{
			return null;
		}
		if (CommonFlagX.XDic.ContainsKey(name))
		{
			return CommonFlagX.XDic[name];
		}
		return null;
	}

	public static void DelFlagData(string name)
	{
		if (name.Equals(string.Empty))
		{
			return;
		}
		if (CommonFlagX.XDic.ContainsKey(name))
		{
			CommonFlagX.XDic.Remove(name);
		}
	}

	protected static void ClearStaticData()
	{
		CommonFlagX.XDic.Clear();
	}

	private static Dictionary<string, CommonFlagX.CommonFlagData> XDic = new Dictionary<string, CommonFlagX.CommonFlagData>();

	public class CommonFlagData
	{
		public void Remove()
		{
			CommonFlagX.DelFlagData(this.name);
		}

		public string name = string.Empty;

		public string strdata = string.Empty;

		public object refdata;

		public Dictionary<string, object> dic = new Dictionary<string, object>();
	}
}
