using System;
using System.Collections.Generic;
using Server.Data;

public class EmailData
{
	public int EmailID;

	public string Title = string.Empty;

	public string Time = string.Empty;

	public int Type;

	public bool State;

	public string From = string.Empty;

	public string NeiRong = string.Empty;

	public List<MailGoodsData> GoodsIDList;

	public int TongQianNum;

	public int YinLiangNum;

	public int YuanBaoNum;

	public int Hasfetchattachment;
}
