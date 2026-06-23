using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class SystemOpenVO : Comparer<SystemOpenVO>
{
	public override int Compare(SystemOpenVO x, SystemOpenVO y)
	{
		if (x.TriggerCondition != y.TriggerCondition)
		{
			return x.TriggerCondition - y.TriggerCondition;
		}
		if (x.TriggerCondition == 1)
		{
			return Global.GetUnionLevel(x.TimeParameters, x.TimeParameters2) - Global.GetUnionLevel(y.TimeParameters, y.TimeParameters2);
		}
		if (x.TriggerCondition == 7)
		{
			return x.TimeParameters - y.TimeParameters;
		}
		return 0;
	}

	public void CopyFrom(XElement xml)
	{
		this.ID = Global.GetXElementAttributeInt(xml, "ID");
		this.Order = Global.GetXElementAttributeInt(xml, "Order");
		this.Cartoon = Global.GetXElementAttributeInt(xml, "Cartoon");
		this.Name = Global.GetXElementAttributeStr(xml, "Name");
		this.Occupation = Global.GetXElementAttributeInt(xml, "occupation");
		this.TriggerCondition = Global.GetXElementAttributeInt(xml, "TriggerCondition");
		int[] array = Global.String2IntArray(Global.GetXElementAttributeStr(xml, "TimeParameters"), ',');
		if (array != null)
		{
			if (array.Length >= 1)
			{
				this.TimeParameters = array[0];
			}
			if (array.Length >= 2)
			{
				this.TimeParameters2 = array[1];
			}
		}
		this.PostWizardID = Global.GetXElementAttributeInt(xml, "PostWizardID");
		this.ImageOne = Global.GetXElementAttributeStr(xml, "ImageOne");
		this.ImageTwo = Global.GetXElementAttributeStr(xml, "ImageTwo");
		this.Description = Global.GetXElementAttributeStr(xml, "Description");
		this.Music = Global.GetXElementAttributeStr(xml, "Music");
		this.SpecialOpenType = Global.GetXElementAttributeInt(xml, "SpecialOpenType");
		this.NotOpenShow = Global.GetXElementAttributeInt(xml, "NotOpenShow");
		this.DongHua = Global.GetXElementAttributeInt(xml, "DongHua");
	}

	public int ID;

	public int Order;

	public string Name;

	public int Occupation;

	public int TriggerCondition;

	public int TimeParameters;

	public string ImageOne;

	public string ImageTwo;

	public string Description;

	public string Music;

	public int Cartoon;

	public int TimeParameters2;

	public int PostWizardID;

	public int SpecialOpenType;

	public bool IsOpened;

	public int NotOpenShow;

	public int DongHua;
}
