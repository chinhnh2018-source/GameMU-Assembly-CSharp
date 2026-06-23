using System;
using System.Runtime.InteropServices;

public class EventTracer
{
	public static void Start()
	{
	}

	public static void Payment(string item_name, int item_num, string item_price_currency, double item_price)
	{
	}

	public static void CreateRole()
	{
	}

	public static void LevelUp(int oldLevel, int oldZhuanSheng, int theZLevel, int theLevel)
	{
	}

	public static void SetRoleId(int roleId)
	{
	}

	public static void EnterGame(string enterGameEvent = "EnterGame")
	{
	}

	public static void CustomRecord(string theEvent, string value = "")
	{
	}

	[DllImport("__Internal")]
	public static extern void ALPReportEvent(string theEvent);

	[DllImport("__Internal")]
	public static extern void ALPReportPayment(int amount, string currency);

	[DllImport("__Internal")]
	public static extern void ACTConversionReportEvent(string theEvent);
}
