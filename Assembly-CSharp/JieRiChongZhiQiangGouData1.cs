using System;

public class JieRiChongZhiQiangGouData1
{
	public JieRiChongZhiQiangGouData1(string _FromDate, string _ToDate, string _AwardStartDate, string _AwardEndDate)
	{
		if (!string.IsNullOrEmpty(_FromDate))
		{
			this.FromDate = DateTime.Parse(_FromDate);
		}
		else
		{
			this.FromDate = default(DateTime);
		}
		if (!string.IsNullOrEmpty(_ToDate))
		{
			this.ToDate = DateTime.Parse(_ToDate);
		}
		else
		{
			this.ToDate = default(DateTime);
		}
		if (!string.IsNullOrEmpty(_AwardStartDate))
		{
			this.AwardStartDate = DateTime.Parse(_AwardStartDate);
		}
		else
		{
			this.AwardStartDate = default(DateTime);
		}
		if (!string.IsNullOrEmpty(_AwardEndDate))
		{
			this.AwardEndDate = DateTime.Parse(_AwardEndDate);
		}
		else
		{
			this.AwardEndDate = default(DateTime);
		}
	}

	public DateTime FromDate;

	public DateTime ToDate;

	public DateTime AwardStartDate;

	public DateTime AwardEndDate;
}
