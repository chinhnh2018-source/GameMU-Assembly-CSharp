using System;
using System.Collections.Generic;
using Tmsk.Xml;
using XMLCreater;

public class MoShenMiBaoXingVO
{
	public MoShenMiBaoXingVO()
	{
	}

	public MoShenMiBaoXingVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.MiBaoStageLevel = xe.GetAttributeInt("MiBaoStageLevel", -1);
		this.MibaoStarLevel = xe.GetAttributeInt("MibaoStarLevel", -1);
		this.MiBaoType = xe.GetAttributeInt("MiBaoType", -1);
		string attributeStr = xe.GetAttributeStr("MiBaoAttribute");
		this.MiBaoAttribute = MUPropInfoHelper.DeserializeToPropList(attributeStr);
		this.MibaoStarExp = xe.GetAttributeInt("MibaoStarExp", -1);
		string attributeStr2 = xe.GetAttributeStr("NeedGoods");
		this.NeedGoods = new List<MUMaterialInfo>();
		if (attributeStr2 != string.Empty)
		{
			string[] array = attributeStr2.Split(new char[]
			{
				'|'
			});
			string content = string.Empty;
			if (array.Length == 1)
			{
				content = "0,0";
			}
			else
			{
				content = array[0] + "," + array[1];
			}
			MUMaterialInfo mumaterialInfo = new MUMaterialInfo(content);
			this.NeedGoods.Add(mumaterialInfo);
		}
		this.GoodsExp = xe.GetAttributeInt("GoodsExp", -1);
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public int MiBaoStageLevel { get; set; }

	public int MibaoStarLevel { get; set; }

	public int MiBaoType { get; set; }

	public List<MUPropInfo> MiBaoAttribute { get; set; }

	public int MibaoStarExp { get; set; }

	public List<MUMaterialInfo> NeedGoods { get; set; }

	public int GoodsExp { get; set; }
}
