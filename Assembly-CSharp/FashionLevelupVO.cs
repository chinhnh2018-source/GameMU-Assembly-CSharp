using System;
using System.Collections.Generic;

public class FashionLevelupVO
{
	public string ProPerty
	{
		get
		{
			return this._ProPerty;
		}
		set
		{
			this._ProPerty = value;
		}
	}

	public string[] ProPerty_Array
	{
		get
		{
			string[] result = null;
			if (!string.IsNullOrEmpty(this._ProPerty))
			{
				result = this._ProPerty.Split(new char[]
				{
					'|'
				});
			}
			return result;
		}
	}

	public Dictionary<int, double> AttDic
	{
		get
		{
			if (this.mAttDic.Count == 0)
			{
				for (int i = 0; i < this.ProPerty_Array.Length; i++)
				{
					string[] array = this.ProPerty_Array[i].Split(new char[]
					{
						','
					});
					string word = array[0];
					ExtPropIndexesVO extPropIndexesVOByWord = ConfigExtPropIndexes.GetExtPropIndexesVOByWord(word);
					if (extPropIndexesVOByWord != null)
					{
						if (!this.mAttDic.ContainsValue((double)extPropIndexesVOByWord.ID))
						{
							this.mAttDic.Add(extPropIndexesVOByWord.ID, double.Parse(array[1]));
						}
						else
						{
							this.mAttDic[extPropIndexesVOByWord.ID] = double.Parse(array[1]);
						}
					}
				}
			}
			return this.mAttDic;
		}
	}

	public int ID;

	public int GoodsID;

	public string Name;

	public int level;

	public string NeedGoods;

	private string _ProPerty;

	public string MaxLifeV;

	public string Dodge;

	public string HitV;

	public string Defense;

	public int Time;

	public int MainOccupation;

	public string MOD;

	public int Previev;

	public int Scene;

	public string Description;

	private Dictionary<int, double> mAttDic = new Dictionary<int, double>();
}
