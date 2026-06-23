using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class LianSaiLeagueSuperListVO
{
	public LianSaiLeagueSuperListVO(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.mMatch1 = Global.GetXElementAttributeStr(xml, "Match1");
			this.mMatch2 = Global.GetXElementAttributeStr(xml, "Match2");
			this.mMatch3 = Global.GetXElementAttributeStr(xml, "Match3");
			this.mMatch4 = Global.GetXElementAttributeStr(xml, "Match4");
		}
	}

	public int[] Match1
	{
		get
		{
			return this.GetMath(this.mMatch1);
		}
	}

	public int[] Match2
	{
		get
		{
			return this.GetMath(this.mMatch2);
		}
	}

	public int[] Match3
	{
		get
		{
			return this.GetMath(this.mMatch3);
		}
	}

	public int[] Match4
	{
		get
		{
			return this.GetMath(this.mMatch4);
		}
	}

	private int[] GetMath(string str)
	{
		int[] array = new int[2];
		if (!string.IsNullOrEmpty(str))
		{
			string[] array2 = str.Split(new char[]
			{
				'|'
			});
			if (array2 != null && array2.Length == 2)
			{
				array[0] = array2[0].SafeToInt32(0);
				array[1] = array2[1].SafeToInt32(0);
			}
		}
		return array;
	}

	public int ID;

	public string Name;

	private string mMatch1;

	private string mMatch2;

	private string mMatch3;

	private string mMatch4;
}
