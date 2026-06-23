using System;

public class Common
{
	public enum PPPayResultCode
	{
		PPPayResultCodeSucceed,
		PPPayResultCodePayAndExchange,
		PPPayResultCodeForbidden = 1001,
		PPPayResultCodeUserNotExist,
		PPPayResultCodeParamLost,
		PPPayResultCodeNotSufficientFunds,
		PPPayResultCodeGameDataNotExist,
		PPPayResultCodeDeveloperNotExist,
		PPPayResultCodeZoneNotExist,
		PPPayResultCodeSystemError,
		PPPayResultCodeFail,
		PPPayResultCodeCommunicationFail,
		PPPayResultCodeUntreatedBillNo,
		PPPayResultCodeCancel,
		PPPayResultCodeAccountIsLocked,
		PPPayResultCodeUserOffLine = 999
	}

	public enum PPSDKInterfaceOrientationMaskType
	{
		PPSDKInterfaceOrientationMaskTypeLandscape,
		PPSDKInterfaceOrientationMaskTypePortrait
	}
}
