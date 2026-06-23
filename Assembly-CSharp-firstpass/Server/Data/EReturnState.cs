using System;

namespace Server.Data
{
	public enum EReturnState
	{
		EDouble = -100,
		EFailShow,
		ShowReturn = -52,
		ShowNoCheck,
		ShowNoSign,
		EShopMax = -14,
		ELevel,
		EVip,
		EPlatform,
		ENoOpen,
		ENoRecall,
		EIsSelf,
		ENoReturn,
		EWait,
		EIsReturn,
		ETimeOut,
		EFail,
		ESign,
		ECheck,
		Default,
		WaitCheck,
		Check,
		WaitSign,
		CheckAndSign
	}
}
