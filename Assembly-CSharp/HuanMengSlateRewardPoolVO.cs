using System;
using Tmsk.Xml;

public class HuanMengSlateRewardPoolVO
{
	public HuanMengSlateRewardPoolVO()
	{
	}

	public HuanMengSlateRewardPoolVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.RandomPoolType = xe.GetAttributeStr("RandomPoolType");
		this.RewardName = xe.GetAttributeStr("RewardName");
		this.ActivatedPro = xe.GetAttributeStr("ActivatedPro");
		this.ActivatedNum = xe.GetAttributeInt("ActivatedNum", -1);
		this.ActivatedPool = xe.GetAttributeInt("ActivatedPool", -1);
		this.NeedLuckStar = xe.GetAttributeStr("NeedLuckStar");
		this.StartIsLock = xe.GetAttributeInt("StartIsLock", -1);
		this.ContinueIsLock = xe.GetAttributeInt("ContinueIsLock", -1);
	}

	public int ID { get; set; }

	public string RandomPoolType { get; set; }

	public string RewardName { get; set; }

	public string ActivatedPro { get; set; }

	public int ActivatedNum { get; set; }

	public int ActivatedPool { get; set; }

	public string NeedLuckStar { get; set; }

	public int StartIsLock { get; set; }

	public int ContinueIsLock { get; set; }
}
