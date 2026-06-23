using System;
using HSGameEngine.GameFramework.Logic;

public class SystemHintData
{
	public ActivityTipTypes HintType
	{
		get
		{
			return this.m_hintType;
		}
		set
		{
			this.m_hintType = value;
		}
	}

	public bool BeActived
	{
		get
		{
			return this.m_beActived;
		}
		set
		{
			this.m_beActived = value;
		}
	}

	private ActivityTipTypes m_hintType;

	private bool m_beActived;
}
