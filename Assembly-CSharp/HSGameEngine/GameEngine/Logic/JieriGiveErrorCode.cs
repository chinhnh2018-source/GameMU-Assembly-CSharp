using System;

namespace HSGameEngine.GameEngine.Logic
{
	public enum JieriGiveErrorCode
	{
		Success,
		ActivityNotOpen,
		NotAwardTime,
		GoodsIDError,
		GoodsNotEnough,
		ReceiverNotExist,
		ReceiverCannotSelf,
		DBFailed,
		ConfigError,
		NoBagSpace,
		NotMeetAwardCond
	}
}
