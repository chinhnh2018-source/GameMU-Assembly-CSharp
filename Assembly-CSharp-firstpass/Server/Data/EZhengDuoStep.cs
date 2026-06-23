using System;

namespace Server.Data
{
	public enum EZhengDuoStep
	{
		None,
		Preliminarises,
		Rank16To8,
		Rank8To4,
		Rank4To2,
		Rank2To1,
		End
	}
}
