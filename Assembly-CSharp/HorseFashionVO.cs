using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class HorseFashionVO
{
	public void CopyForm(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.GoodsID = Global.GetXElementAttributeInt(xml, "GoodsID");
			this.Name = Global.GetXElementAttributeStr(xml, "Name");
			this.MOD = Global.GetXElementAttributeStr(xml, "MOD");
			this.level = Global.GetXElementAttributeInt(xml, "level");
			this.NeedGoods = Global.GetXElementAttributeStr(xml, "NeedGoods");
			this.ProPerty = Global.GetXElementAttributeStr(xml, "ProPerty");
			this.Time = Global.GetXElementAttributeLong(xml, "Time");
		}
	}

	public Dictionary<string, double> AddValue
	{
		get
		{
			if (0 >= this.addValue.Count && !string.IsNullOrEmpty(this.ProPerty))
			{
				string[] array = this.ProPerty.Split(new char[]
				{
					'|'
				});
				if (0 < array.Length)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								double num = 0.0;
								double.TryParse(array2[1], ref num);
								this.addValue[array2[0]] = num;
							}
						}
					}
				}
			}
			return this.addValue;
		}
	}

	public List<GoodsData> NeedGood
	{
		get
		{
			if (0 >= this.needGood.Count)
			{
				string[] array = this.NeedGoods.Split(new char[]
				{
					'|'
				});
				if (0 < array.Length)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!string.IsNullOrEmpty(array[i]))
						{
							string[] array2 = array[i].Split(new char[]
							{
								','
							});
							if (array2.Length == 2)
							{
								this.needGood.Add(Global.GetEmptyGoodsData(array2[0].SafeToInt32(0), 0, 0, 0, array2[1].SafeToInt32(0), 0, 0, 0, 0));
							}
						}
					}
				}
			}
			return this.needGood;
		}
	}

	public int ID;

	public int GoodsID;

	public string Name;

	public string MOD;

	public int level;

	public string NeedGoods;

	public string ProPerty;

	public long Time;

	private Dictionary<string, double> addValue = new Dictionary<string, double>();

	private List<GoodsData> needGood = new List<GoodsData>();
}
