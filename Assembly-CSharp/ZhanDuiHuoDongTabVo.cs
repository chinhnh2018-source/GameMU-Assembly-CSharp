using System;
using Tmsk.Xml;

public class ZhanDuiHuoDongTabVo
{
	public ZhanDuiHuoDongTabVo(XElement element)
	{
		this.ID = element.GetAttributeInt("ID", -1);
		this.Name = element.GetAttributeStr("Name");
		this.Preview = element.GetAttributeInt("Preview", -1);
		this.RewardExplain = element.GetAttributeStr("RewardExplain");
	}

	public int ID;

	public string Name;

	public int Preview;

	public string RewardExplain;

	public string GLXml;
}
