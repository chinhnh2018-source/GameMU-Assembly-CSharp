using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class ShenshenghudunXing
{
	private void InitVO()
	{
		if (0 >= this.dic.Count)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ShenshenghudunXing.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "JingLing");
				if (0 < xelementList.Count)
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						ShenshenghudunXingVO shenshenghudunXingVO = new ShenshenghudunXingVO();
						shenshenghudunXingVO.CopyForm(xelementList[i]);
						this.dic[shenshenghudunXingVO.ID] = shenshenghudunXingVO;
					}
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=red>ShenshenghudunXing.xml  读取失败  </color>"
				});
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

	public int GetUpNeedEXP(int Star, int step)
	{
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.GetShenshenghudunXingVOByID(this.GetIDByStarAndJie(Star, step));
		if (shenshenghudunXingVOByID != null)
		{
			return shenshenghudunXingVOByID.StarExp;
		}
		return 0;
	}

	public ShenshenghudunXingVO GetShenshenghudunXingVOByID(int ID)
	{
		this.InitVO();
		if (this.dic.ContainsKey(ID))
		{
			return this.dic[ID];
		}
		return null;
	}

	public int GetMaxStar()
	{
		this.InitVO();
		if (0 >= this.mMaxStar)
		{
			Dictionary<int, ShenshenghudunXingVO>.Enumerator enumerator = this.dic.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int num = this.mMaxStar;
				KeyValuePair<int, ShenshenghudunXingVO> keyValuePair = enumerator.Current;
				if (num < keyValuePair.Value.StarLevel)
				{
					KeyValuePair<int, ShenshenghudunXingVO> keyValuePair2 = enumerator.Current;
					this.mMaxStar = keyValuePair2.Value.StarLevel;
				}
			}
		}
		return this.mMaxStar;
	}

	public GoodsData[] GetNeedGoodsByID(int ID)
	{
		this.InitVO();
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.GetShenshenghudunXingVOByID(ID);
		if (shenshenghudunXingVOByID != null)
		{
			return shenshenghudunXingVOByID.GNeedGoods;
		}
		return null;
	}

	public int GetIDByStarAndJie(int Star, int step)
	{
		this.InitVO();
		foreach (KeyValuePair<int, ShenshenghudunXingVO> keyValuePair in this.dic)
		{
			if (keyValuePair.Value.ArmorupStage == step)
			{
				Dictionary<int, ShenshenghudunXingVO>.Enumerator enumerator;
				KeyValuePair<int, ShenshenghudunXingVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.StarLevel == Star)
				{
					KeyValuePair<int, ShenshenghudunXingVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Key;
				}
			}
		}
		return 0;
	}

	public Dictionary<string, double> GetAttByStarAndJie(int Star, int step)
	{
		this.InitVO();
		foreach (KeyValuePair<int, ShenshenghudunXingVO> keyValuePair in this.dic)
		{
			if (keyValuePair.Value.ArmorupStage == step)
			{
				Dictionary<int, ShenshenghudunXingVO>.Enumerator enumerator;
				KeyValuePair<int, ShenshenghudunXingVO> keyValuePair2 = enumerator.Current;
				if (keyValuePair2.Value.StarLevel == Star)
				{
					KeyValuePair<int, ShenshenghudunXingVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value.Att;
				}
			}
		}
		return null;
	}

	public int GetUpUseDiamond(int Star, int step)
	{
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.GetShenshenghudunXingVOByID(this.GetIDByStarAndJie(Star, step));
		if (shenshenghudunXingVOByID != null)
		{
			return shenshenghudunXingVOByID.NeedDiamond;
		}
		return -1;
	}

	public int GetModalURL(int Star, int step)
	{
		ShenshenghudunXingVO shenshenghudunXingVOByID = this.GetShenshenghudunXingVOByID(this.GetIDByStarAndJie(Star, step));
		if (shenshenghudunXingVOByID != null)
		{
			return shenshenghudunXingVOByID.ModID;
		}
		return -1;
	}

	private int mRecommendCount;

	private Dictionary<int, ShenshenghudunXingVO> dic = new Dictionary<int, ShenshenghudunXingVO>();

	private int mMaxStar;
}
