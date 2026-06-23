using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;

public class ConfigLuolanFazhen
{
	public string ChuanSongMenID
	{
		get
		{
			return this.chuanSongMenID;
		}
		set
		{
			this.chuanSongMenID = value;
			this.GetMenID(value);
		}
	}

	public string MapToMen
	{
		get
		{
			return this.mapToMen;
		}
		set
		{
			this.mapToMen = value;
			this.GetMapToMen(value);
		}
	}

	private void GetMenID(string str)
	{
		string[] array = str.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			this.chuansongMen.Add(ConvertExt.SafeConvertToInt32(array[i]));
		}
	}

	private void GetMapToMen(string str)
	{
		string[] array = str.Split(new char[]
		{
			'|'
		});
		if (array.Length > 1)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2.Length == 2 && !this.DicMapToMen.ContainsKey(array2[0]))
				{
					this.DicMapToMen.Add(array2[0], array2[1]);
				}
			}
		}
	}

	public int ID;

	public int MapID;

	public string TeShuMapID;

	public List<int> chuansongMen = new List<int>();

	public Dictionary<string, string> DicMapToMen = new Dictionary<string, string>();

	private string chuanSongMenID = string.Empty;

	private string mapToMen = string.Empty;
}
