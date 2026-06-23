using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class BuildVO
{
	public void CopyForm(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.Icon = Global.GetXElementAttributeStr(xml, "Icon");
			this.Pic = Global.GetXElementAttributeStr(xml, "Pic");
			this.MaxLevel = Global.GetXElementAttributeInt(xml, "MaxLevel");
			this.FreeRandomTask = Global.GetXElementAttributeStr(xml, "FreeRandomTask");
			this.Award = Global.GetXElementAttributeStr(xml, "Award");
			this.RandomTask = Global.GetXElementAttributeStr(xml, "RandomTask");
		}
	}

	public int[] DignLevel
	{
		get
		{
			string pic = this.Pic;
			string[] array = pic.Split(new char[]
			{
				'|'
			});
			int[] array2 = new int[array.Length];
			byte b = 0;
			while ((int)b < array.Length)
			{
				string[] array3 = array[(int)b].Split(new char[]
				{
					','
				});
				array2[(int)b] = Convert.ToInt32(array3[0]);
				b += 1;
			}
			return array2;
		}
	}

	public string[] PicArray
	{
		get
		{
			string pic = this.Pic;
			string[] array = pic.Split(new char[]
			{
				'|'
			});
			string[] array2 = new string[array.Length];
			byte b = 0;
			while ((int)b < array.Length)
			{
				string[] array3 = array[(int)b].Split(new char[]
				{
					','
				});
				array2[(int)b] = array3[1];
				b += 1;
			}
			return array2;
		}
	}

	public int ID;

	public string Name;

	public string Icon;

	public string Pic;

	public int MaxLevel;

	public string Award;

	public string FreeRandomTask;

	public string RandomTask;
}
