using System;

public enum LingDiResultCode
{
	KuaFu_Conect_err = -1,
	Not_LingZhu = -2,
	Not_Between_OpenTime = -3,
	Have_OpenDouble = -4,
	Over_OpenCount = -5,
	Not_Found_LingZhuData = -6,
	Save_DB_err = -7,
	Not_Found_LingDiData = -8,
	Not_In_LingDiCaiJi = -9,
	Not_Found_ShouWei = -11,
	Not_Use_ZuanShi = -12,
	Not_Enough_ZuanShi = -13,
	Not_Enough_FanRongDu = -14,
	Have_ShouWei = -15,
	Not_ZuanShi = -16,
	Success = 1
}
