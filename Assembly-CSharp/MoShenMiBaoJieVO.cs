using System;
using System.Collections.Generic;
using Tmsk.Xml;
using XMLCreater;

public class MoShenMiBaoJieVO
{
	public MoShenMiBaoJieVO()
	{
	}

	public MoShenMiBaoJieVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.MibaoStage = xe.GetAttributeInt("MiBaoStageLevel", -1);
		this.MibaoModerID = xe.GetAttributeInt("MibaoModerID", -1);
		this.MiBaoType = xe.GetAttributeInt("MiBaoType", -1);
		this.LuckyOne = xe.GetAttributeInt("LuckyOne", -1);
		this.LuckyTwo = xe.GetAttributeInt("LuckyTwo", -1);
		this.LuckyTwoRate = xe.GetAttributeFloat("LuckyTwoRate", -1f);
		string attributeStr = xe.GetAttributeStr("NeedGoods");
		this.NeedGoods = new List<MUMaterialInfo>();
		if (attributeStr != string.Empty)
		{
			string[] array = attributeStr.Split(new char[]
			{
				'|'
			});
			string content = array[0] + "," + array[1];
			MUMaterialInfo mumaterialInfo = new MUMaterialInfo(content);
			this.NeedGoods.Add(mumaterialInfo);
		}
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public int MibaoStage { get; set; }

	public int MibaoModerID { get; set; }

	public int MiBaoType { get; set; }

	public int LuckyOne { get; set; }

	public int LuckyTwo { get; set; }

	public float LuckyTwoRate { get; set; }

	public List<MUMaterialInfo> NeedGoods { get; set; }
}
