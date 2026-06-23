using System;
using Server.Data;

public class JingLingBangHuiMemberData : BangHuiMemberData
{
	public void InitWith(BangHuiMemberData data)
	{
		this.ZoneID = data.ZoneID;
		this.RoleID = data.RoleID;
		this.RoleName = data.RoleName;
		this.Occupation = data.Occupation;
		this.BHZhiwu = data.BHZhiwu;
		this.ChengHao = data.ChengHao;
		this.BangGong = data.BangGong;
		this.Level = data.Level;
		this.XueWeiNum = data.XueWeiNum;
		this.SkillLearnedNum = data.SkillLearnedNum;
		this.OnlineState = data.OnlineState;
		this.BangHuiMemberCombatForce = data.BangHuiMemberCombatForce;
		this.BangHuiMemberChangeLifeLev = data.BangHuiMemberChangeLifeLev;
		this.JunTuanZhiWu = data.JunTuanZhiWu;
		this.YaoSaiBossState = data.YaoSaiBossState;
		this.YaoSaiJianYuState = data.YaoSaiJianYuState;
	}
}
