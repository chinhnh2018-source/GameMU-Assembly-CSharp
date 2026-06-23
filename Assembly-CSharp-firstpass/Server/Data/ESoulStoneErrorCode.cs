using System;

namespace Server.Data
{
	public enum ESoulStoneErrorCode
	{
		Success,
		UnknownFailed,
		VisitParamsError,
		SelectExtFuncNotOpen,
		ConfigError,
		LangHunFenMoNotEnough,
		ExtCostNotEnough,
		BagNoSpace,
		LevelIsFull,
		CanNotEquip,
		DbFailed,
		NotOpen
	}
}
