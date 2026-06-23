using System;

public class ChongzhiInfo
{
	public string Icon = string.Empty;

	public string money = string.Empty;

	public string zuanshiCount = string.Empty;

	public string productId = string.Empty;

	public string freeDiamond = string.Empty;

	public ChongzhiInfo.ChongZhiType Type;

	public enum ChongZhiType
	{
		Normal,
		YueKa
	}
}
