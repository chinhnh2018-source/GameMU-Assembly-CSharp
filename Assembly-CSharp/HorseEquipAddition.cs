using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;

public class HorseEquipAddition
{
	public Dictionary<string, double> AdditionPropsDic
	{
		get
		{
			if (0 >= this.mAdditionPropsDic.Count && !string.IsNullOrEmpty(this.AdditionProps))
			{
				string[] array = this.AdditionProps.Split(new char[]
				{
					'|'
				});
				if (0 < array.Length)
				{
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							string[] array3 = text.Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								this.mAdditionPropsDic[array3[0]] = Global.SafeConvertToDouble(array3[1]);
							}
						}
					}
				}
			}
			return this.mAdditionPropsDic;
		}
	}

	public int ID;

	public int Type;

	public int NextID;

	public int NeedStrengthenLevel;

	public int NeedAdditionLevel;

	public int NeedOrderNum;

	public string AdditionProps;

	private Dictionary<string, double> mAdditionPropsDic = new Dictionary<string, double>();
}
