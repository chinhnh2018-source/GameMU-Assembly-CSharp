using System;

namespace HSGameEngine.GameFramework.Logic
{
	public enum SecondPasswordError
	{
		SecPwdVerifySuccess,
		SecPwdVerifyFailed,
		SecPwdIsNotSet,
		SecPwdCharInvalid,
		SecPwdIsNull,
		SecPwdIsTooShort,
		SecPwdIsTooLong,
		SecPwdSetSuccess,
		SecPwdDBFailed,
		SecPwdClearSuccess
	}
}
