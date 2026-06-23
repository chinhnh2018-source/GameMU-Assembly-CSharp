using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Xml;

public class HorseFashionXml
{
	private void InitXml()
	{
		XElement gameResXml = Global.GetGameResXml("Config/HorseFashion.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "HorseFashion");
			if (0 < xelementList.Count)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					HorseFashionVO horseFashionVO = new HorseFashionVO();
					horseFashionVO.CopyForm(xelementList[i]);
					this.dic.Add(horseFashionVO.ID, horseFashionVO);
				}
			}
		}
	}

	public void ClearData()
	{
		if (0 < this.dic.Count)
		{
			this.dic.Clear();
		}
	}

	public HorseFashionVO GetHorseFashionVOByID(int ID)
	{
		if (0 >= this.dic.Count)
		{
			this.InitXml();
		}
		HorseFashionVO result = null;
		this.dic.TryGetValue(ID, ref result);
		return result;
	}

	public List<GoodsData> GetHorseFashionUpNeedsGoodsByID(int ID)
	{
		HorseFashionVO horseFashionVOByID = this.GetHorseFashionVOByID(ID);
		if (horseFashionVOByID != null)
		{
			return horseFashionVOByID.NeedGood;
		}
		return null;
	}

	public List<HorseFashionVO> GetHorseFashionVOlistByGoodsID(int GoodsID)
	{
		List<HorseFashionVO> list = new List<HorseFashionVO>();
		if (0 >= this.dic.Count)
		{
			this.InitXml();
		}
		foreach (KeyValuePair<int, HorseFashionVO> keyValuePair in this.dic)
		{
			if (GoodsID == keyValuePair.Value.GoodsID)
			{
				List<HorseFashionVO> list2 = list;
				Dictionary<int, HorseFashionVO>.Enumerator enumerator;
				KeyValuePair<int, HorseFashionVO> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Value);
			}
		}
		return list;
	}

	public HorseFashionVO GetHorseFashionVOByGoodsIDAndLevel(int GoodsID, int Level)
	{
		if (0 >= this.dic.Count)
		{
			this.InitXml();
		}
		foreach (KeyValuePair<int, HorseFashionVO> keyValuePair in this.dic)
		{
			if (GoodsID == keyValuePair.Value.GoodsID)
			{
				Dictionary<int, HorseFashionVO>.Enumerator enumerator;
				KeyValuePair<int, HorseFashionVO> keyValuePair2 = enumerator.Current;
				if (Level == keyValuePair2.Value.level)
				{
					KeyValuePair<int, HorseFashionVO> keyValuePair3 = enumerator.Current;
					return keyValuePair3.Value;
				}
			}
		}
		return null;
	}

	public int GetHorseFashionMaxLevelByGoodsId(int GoodsID)
	{
		int num = 0;
		if (0 >= this.dic.Count)
		{
			this.InitXml();
		}
		foreach (KeyValuePair<int, HorseFashionVO> keyValuePair in this.dic)
		{
			if (GoodsID == keyValuePair.Value.GoodsID)
			{
				int num2 = num;
				Dictionary<int, HorseFashionVO>.Enumerator enumerator;
				KeyValuePair<int, HorseFashionVO> keyValuePair2 = enumerator.Current;
				if (num2 < keyValuePair2.Value.level)
				{
					KeyValuePair<int, HorseFashionVO> keyValuePair3 = enumerator.Current;
					num = keyValuePair3.Value.level;
				}
			}
		}
		return num;
	}

	private Dictionary<int, HorseFashionVO> dic = new Dictionary<int, HorseFashionVO>();
}
