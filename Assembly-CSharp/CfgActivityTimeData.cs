using System;

public struct CfgActivityTimeData
{
	public int dayOfWeek { get; set; }

	public string beginPoint { get; set; }

	public string endPoint { get; set; }

	public int timeInterval { get; set; }

	public DateTime beginTime { get; set; }

	public DateTime endTime { get; set; }

	public int OpenActivityCount { get; set; }
}
