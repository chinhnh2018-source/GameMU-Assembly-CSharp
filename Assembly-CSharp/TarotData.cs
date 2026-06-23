using System;
using System.Collections.Generic;

public class TarotData
{
	public void Add(int Lev, TarotXmlData data)
	{
		if (this.m_TarotXmlData.ContainsKey(Lev))
		{
			this.m_TarotXmlData[Lev] = data;
		}
		else
		{
			this.m_TarotXmlData.Add(Lev, data);
		}
	}

	public TarotXmlData Get(int Lev)
	{
		TarotXmlData result = null;
		if (this.m_TarotXmlData.ContainsKey(Lev))
		{
			result = this.m_TarotXmlData[Lev];
		}
		return result;
	}

	private void InitMaxLevel()
	{
		if (this.m_MaxLevel == 0)
		{
			Dictionary<int, TarotXmlData>.Enumerator enumerator = this.m_TarotXmlData.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int maxLevel = this.m_MaxLevel;
				KeyValuePair<int, TarotXmlData> keyValuePair = enumerator.Current;
				int maxLevel2;
				if (maxLevel <= keyValuePair.Key)
				{
					KeyValuePair<int, TarotXmlData> keyValuePair2 = enumerator.Current;
					maxLevel2 = keyValuePair2.Key;
				}
				else
				{
					maxLevel2 = this.m_MaxLevel;
				}
				this.m_MaxLevel = maxLevel2;
			}
		}
	}

	public int Maxlevel
	{
		get
		{
			this.InitMaxLevel();
			return this.m_MaxLevel;
		}
	}

	private void InitNeedsGoods()
	{
		if (this.m_NeedsGoodsId == null)
		{
			this.m_NeedsGoodsId = new int[this.m_TarotXmlData.Count];
			this.m_NeedsGoodsNum = new int[this.m_TarotXmlData.Count];
			int num = 0;
			foreach (KeyValuePair<int, TarotXmlData> keyValuePair in this.m_TarotXmlData)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Value.NeedGoods))
				{
					Dictionary<int, TarotXmlData>.Enumerator enumerator;
					KeyValuePair<int, TarotXmlData> keyValuePair2 = enumerator.Current;
					string[] array = keyValuePair2.Value.NeedGoods.Split(new char[]
					{
						','
					});
					if (array != null && array.Length == 2)
					{
						int num2 = 0;
						if (int.TryParse(array[0], ref num2))
						{
							this.m_NeedsGoodsId[num] = num2;
						}
						int num3 = 0;
						if (int.TryParse(array[1], ref num3))
						{
							this.m_NeedsGoodsNum[num] = num3;
						}
					}
				}
				num++;
			}
		}
	}

	public int GetNeedsGoodsId(int Level)
	{
		if (this.m_NeedsGoodsId == null)
		{
			this.InitNeedsGoods();
		}
		if (Level < this.m_NeedsGoodsId.Length)
		{
			return this.m_NeedsGoodsId[Level];
		}
		return 0;
	}

	public int GetNeedsGoodsNum(int Level)
	{
		if (this.m_NeedsGoodsNum == null)
		{
			this.InitNeedsGoods();
		}
		if (Level < this.m_NeedsGoodsNum.Length)
		{
			return this.m_NeedsGoodsNum[Level];
		}
		return 0;
	}

	public Dictionary<int, TarotXmlData> m_TarotXmlData = new Dictionary<int, TarotXmlData>();

	private int m_MaxLevel;

	private int[] m_NeedsGoodsId;

	private int[] m_NeedsGoodsNum;
}
