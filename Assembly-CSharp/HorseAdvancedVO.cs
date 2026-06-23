using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class HorseAdvancedVO
{
	public float AdvancedEffectFloat
	{
		get
		{
			float result = 0f;
			if (float.TryParse(this.AdvancedEffect, ref result))
			{
				return result;
			}
			return 0f;
		}
	}

	public List<GoodsData> NeedGoodsLst
	{
		get
		{
			List<GoodsData> list = new List<GoodsData>();
			if (!string.IsNullOrEmpty(this.NeedGoods))
			{
				string[] array = this.NeedGoods.Split(new char[]
				{
					'|'
				});
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						GoodsData emptyGoodsData = Global.GetEmptyGoodsData(array2[0].SafeToInt32(0), 0, 0, 0, array2[1].SafeToInt32(0), 0, 0, 0, 0);
						list.Add(emptyGoodsData);
					}
				}
			}
			return list;
		}
	}

	public int ID;

	public int Level;

	public string NeedGoods;

	public string AdvancedEffect;

	public int SkillID;

	public int HorseID;

	public int ChangeHunJing;
}
