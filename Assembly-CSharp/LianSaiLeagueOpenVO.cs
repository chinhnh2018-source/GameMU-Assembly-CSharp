using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class LianSaiLeagueOpenVO
{
	public LianSaiLeagueOpenVO(XElement xml)
	{
		if (xml != null)
		{
			this.ID = Global.GetXElementAttributeInt(xml, "ID");
			this.PingTai = Global.GetXElementAttributeStr(xml, "PingTai");
			this.mOpenDate = Global.GetXElementAttributeStr(xml, "OpenDate");
			this.mEndDate = Global.GetXElementAttributeStr(xml, "EndDate");
		}
	}

	public DateTime EndDate
	{
		get
		{
			if (0 >= this.mEndDate.SafeToInt32(0))
			{
				return DateTime.MinValue;
			}
			return Global.SafeConvertDateTime(this.mEndDate);
		}
	}

	public DateTime OpenDate
	{
		get
		{
			if (0 >= this.mOpenDate.SafeToInt32(0))
			{
				return DateTime.MinValue;
			}
			return Global.SafeConvertDateTime(this.mOpenDate);
		}
	}

	public bool PingTaiISOpen
	{
		get
		{
			return true;
		}
	}

	public int ID;

	public string PingTai;

	private string mOpenDate;

	private string mEndDate;
}
