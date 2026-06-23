using System;
using System.Collections.Generic;

public class JuHunData
{
	public int ID { get; set; }

	public int Type { get; set; }

	public int Level { get; set; }

	public string shenShi { get; set; }

	public float GrowProportion { get; set; }

	public int CostBandJinBi { get; set; }

	public float SuccessProportion { get; set; }

	public List<CaiLiaoData> caiLiaos = new List<CaiLiaoData>();
}
