using System;

public enum XingyunChoujiangResult
{
	None,
	OK,
	JinbiNot_enough = -1,
	BangzuanNot_enough = -2,
	ZuanshiNot_enough = -3,
	BeibaoFull = -4,
	NotOpen = -100,
	ConfigError = -101,
	CanshuError = -200,
	LiebiaoError = -201,
	CaozuoError = -202
}
