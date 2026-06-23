using System;
using System.Collections.Generic;

public class YinJiData
{
	public YinJiData(int id, int index)
	{
		this.SetData(id, index);
	}

	public int ID { get; set; }

	public int Type { get; set; }

	public int Level { get; set; }

	public int MaxLevel { get; set; }

	public bool IsFullLevel { get; set; }

	public string NextIDAndType { get; set; }

	public bool HasNext { get; set; }

	public bool IsMainYinJi { get; set; }

	public void SetData(int id, int index)
	{
		this.ID = id;
		if (index == 0)
		{
			ZhuYinJiCfgData zhuYinJiCfgById = ChongShengYinJiData.GetZhuYinJiCfgById(this.ID);
			if (zhuYinJiCfgById != null)
			{
				this.IsMainYinJi = true;
				this.Type = zhuYinJiCfgById.TypeZhu;
				this.Level = zhuYinJiCfgById.Level;
				this.MaxLevel = this.GetZhuMaxLevel(this.Type);
				this.IsFullLevel = (this.Level >= this.MaxLevel);
				this.HasNext = this.HasNextZhuYinJi(this.ID + 1, this.Type);
			}
			else
			{
				this.ID = id - 1;
				ZhuYinJiCfgData zhuYinJiCfgById2 = ChongShengYinJiData.GetZhuYinJiCfgById(this.ID);
				if (zhuYinJiCfgById2 != null)
				{
					this.IsMainYinJi = true;
					this.Type = zhuYinJiCfgById2.TypeZhu;
					this.Level = zhuYinJiCfgById2.Level;
					this.MaxLevel = this.GetZhuMaxLevel(this.Type);
					this.IsFullLevel = (this.Level >= this.MaxLevel);
					this.HasNext = false;
				}
			}
		}
		else
		{
			ZiYinJiCfgData ziYinJiCfgById = ChongShengYinJiData.GetZiYinJiCfgById(this.ID);
			if (ziYinJiCfgById != null)
			{
				this.IsMainYinJi = false;
				this.Type = ziYinJiCfgById.Type;
				this.Level = ziYinJiCfgById.Level;
				this.MaxLevel = this.GetZiMaxLevel(this.Type);
				this.IsFullLevel = (this.Level >= this.MaxLevel);
			}
			else
			{
				this.ID = id - 1;
				ZiYinJiCfgData ziYinJiCfgById2 = ChongShengYinJiData.GetZiYinJiCfgById(this.ID);
				if (ziYinJiCfgById2 != null)
				{
					this.IsMainYinJi = false;
					this.Type = ziYinJiCfgById2.Type;
					this.Level = ziYinJiCfgById2.Level;
					this.MaxLevel = this.GetZiMaxLevel(this.Type);
					this.IsFullLevel = (this.Level >= this.MaxLevel);
				}
			}
		}
	}

	private int GetZhuMaxLevel(int type)
	{
		Dictionary<int, ZhuYinJiCfgData> dictZhuYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZhuYinJiCfgData();
		Dictionary<int, ZhuYinJiCfgData>.Enumerator enumerator = dictZhuYinJiCfgData.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, ZhuYinJiCfgData> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.TypeZhu == type)
			{
				num++;
			}
		}
		return num;
	}

	private bool HasNextZhuYinJi(int id, int type)
	{
		bool result = false;
		Dictionary<int, ZhuYinJiCfgData> dictZhuYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZhuYinJiCfgData();
		ZhuYinJiCfgData zhuYinJiCfgData = null;
		if (dictZhuYinJiCfgData.TryGetValue(id, ref zhuYinJiCfgData) && zhuYinJiCfgData.TypeZhu != type)
		{
			result = true;
		}
		return result;
	}

	private int GetZiMaxLevel(int type)
	{
		Dictionary<int, ZiYinJiCfgData> dictZiYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZiYinJiCfgData();
		Dictionary<int, ZiYinJiCfgData>.Enumerator enumerator = dictZiYinJiCfgData.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, ZiYinJiCfgData> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.Type == type)
			{
				num++;
			}
		}
		return num - 1;
	}
}
