using System;
using Tmsk.Xml;

public class EquipQuenchingVO
{
	public EquipQuenchingVO()
	{
	}

	public EquipQuenchingVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.PerfusionName = xe.GetAttributeInt("PerfusionName", -1);
		this.PerfusionLevel = xe.GetAttributeInt("PerfusionLevel", -1);
		this.EverLelAttribute = xe.GetAttributeStr("EverLelAttribute");
		this.IncreaseProbability = xe.GetAttributeStr("IncreaseProbability");
		this.LossItem = xe.GetAttributeStr("LossItem");
		this.ReduceProbability = xe.GetAttributeStr("ReduceProbability");
		this.UpProbability = xe.GetAttributeStr("UpProbability");
		this.LelInterval = xe.GetAttributeFloat("LelInterval", -1f);
	}

	public int ID { get; set; }

	public int PerfusionName { get; set; }

	public int PerfusionLevel { get; set; }

	public string EverLelAttribute { get; set; }

	public string IncreaseProbability { get; set; }

	public string LossItem { get; set; }

	public string ReduceProbability { get; set; }

	public string UpProbability { get; set; }

	public float LelInterval { get; set; }
}
