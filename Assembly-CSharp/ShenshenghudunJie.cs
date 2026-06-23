using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ShenshenghudunJie
{
	private void InitVO()
	{
		if (0 >= this.dic.Count)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ShenshenghudunJie.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
				if (0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						ShenshenghudunJieVO shenshenghudunJieVO = new ShenshenghudunJieVO();
						shenshenghudunJieVO.CopyForm(xelementList[i]);
						this.dic[shenshenghudunJieVO.ID] = shenshenghudunJieVO;
					}
				}
			}
		}
	}

	public void AddRecommendCount()
	{
		this.mRecommendCount++;
	}

	public int SubRecommendCount()
	{
		this.mRecommendCount--;
		if (0 >= this.mRecommendCount)
		{
			this.dic.Clear();
		}
		return this.mRecommendCount;
	}

	public void ClearXMLData()
	{
		this.mRecommendCount = 0;
		this.SubRecommendCount();
	}

	public ShenshenghudunJieVO GetShenshenghudunXingVOByID(int ID)
	{
		this.InitVO();
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		return null;
	}

	public int GetIdByJie(int Jie)
	{
		this.InitVO();
		foreach (KeyValuePair<int, ShenshenghudunJieVO> keyValuePair in this.dic)
		{
			if (Jie == keyValuePair.Value.ArmorClass)
			{
				Dictionary<int, ShenshenghudunJieVO>.Enumerator enumerator;
				KeyValuePair<int, ShenshenghudunJieVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Key;
			}
		}
		return 0;
	}

	public int GetMaxkJie()
	{
		this.InitVO();
		if (0 >= this.mMaxkJie)
		{
			Dictionary<int, ShenshenghudunJieVO>.Enumerator enumerator = this.dic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int num = this.mMaxkJie;
				KeyValuePair<int, ShenshenghudunJieVO> keyValuePair = enumerator.Current;
				if (num < keyValuePair.Value.ArmorClass)
				{
					KeyValuePair<int, ShenshenghudunJieVO> keyValuePair2 = enumerator.Current;
					this.mMaxkJie = keyValuePair2.Value.ArmorClass;
				}
			}
		}
		return this.mMaxkJie;
	}

	public GoodsData[] GetNeedGoodsByJie(int jie)
	{
		this.InitVO();
		foreach (KeyValuePair<int, ShenshenghudunJieVO> keyValuePair in this.dic)
		{
			if (keyValuePair.Value.ArmorClass == jie)
			{
				Dictionary<int, ShenshenghudunJieVO>.Enumerator enumerator;
				KeyValuePair<int, ShenshenghudunJieVO> keyValuePair2 = enumerator.Current;
				return keyValuePair2.Value.GNeedGoods;
			}
		}
		return null;
	}

	public ShenshenghudunJieVO GetShenshenghudunXingVOByJie(int jie)
	{
		this.InitVO();
		int idByJie = this.GetIdByJie(jie);
		if (this.dic.ContainsKey(idByJie))
		{
			return this.dic[idByJie];
		}
		return null;
	}

	public int GetUpUseDiamond(int jie)
	{
		ShenshenghudunJieVO shenshenghudunXingVOByJie = this.GetShenshenghudunXingVOByJie(jie);
		if (shenshenghudunXingVOByJie != null)
		{
			return shenshenghudunXingVOByJie.NeedDiamond;
		}
		return -1;
	}

	public int GetEXP(int jie)
	{
		ShenshenghudunJieVO shenshenghudunXingVOByJie = this.GetShenshenghudunXingVOByJie(jie);
		if (shenshenghudunXingVOByJie != null)
		{
			return 110000 - shenshenghudunXingVOByJie.LuckyOne;
		}
		return -1;
	}

	internal int GetModelID(int Step)
	{
		ShenshenghudunJieVO shenshenghudunXingVOByJie = this.GetShenshenghudunXingVOByJie(Step);
		if (shenshenghudunXingVOByJie != null)
		{
			return shenshenghudunXingVOByJie.ModelID;
		}
		return -1;
	}

	private int mRecommendCount;

	private Dictionary<int, ShenshenghudunJieVO> dic = new Dictionary<int, ShenshenghudunJieVO>();

	private int mMaxkJie;
}
