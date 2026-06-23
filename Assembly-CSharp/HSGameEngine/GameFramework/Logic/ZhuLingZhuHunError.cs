using System;

namespace HSGameEngine.GameFramework.Logic
{
	internal enum ZhuLingZhuHunError
	{
		Success,
		ZhuLingNotOpen,
		ZhuHunNotOpen,
		ZhuLingFull,
		ZhuHunFull,
		ZhuLingMaterialNotEnough,
		ZhuLingJinBiNotEnough,
		ZhuHunMaterialNotEnough,
		ZhuHunJinBiNotEnough,
		ErrorConfig,
		ErrorParams,
		ZuanShiNotEnough,
		DBSERVERERROR
	}
}
