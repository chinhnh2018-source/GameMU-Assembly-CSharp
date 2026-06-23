using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class LianShaMgr
{
	public static LianShaMgr Instance
	{
		get
		{
			if (LianShaMgr.mInstance == null)
			{
				LianShaMgr.mInstance = new LianShaMgr();
			}
			return LianShaMgr.mInstance;
		}
	}

	public void OnRenderGame()
	{
		if (Global.Data != null && Global.Data.roleData != null && IConfigbase<ConfigRebirth>.Instance.GetLianShaCanUseByMap(Global.Data.roleData.MapCode) && null != this.mLianShaShow && this.mLianShaShow.Active && !this.mLianShaShow.UpdateUI())
		{
			this.RemoveLianSha();
		}
	}

	public void NoticeRoleLianShaDataChange(int RoleID, int Num, long delaySecs)
	{
		if (IConfigbase<ConfigRebirth>.Instance.GetLianShaCanUseByMap(Global.Data.roleData.MapCode) && RoleID == Global.Data.RoleID)
		{
			long num = delaySecs - Global.GetCorrectLocalTime();
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"联杀剩余时间：",
					num,
					" 联杀结束时间：",
					delaySecs,
					" 連殺個數： ",
					Num
				})
			});
			if (0L < num)
			{
				this.RefreshLianSha(Num, delaySecs, num);
			}
		}
	}

	public void NoticeRoleLianShaDataChange(BufferData bufferData)
	{
	}

	private void RemoveLianSha()
	{
		if (null != this.mLianShaShow)
		{
			if (IConfigbase<ConfigRebirth>.Instance.GetLianShaCanUseByMap(Global.Data.roleData.MapCode))
			{
				this.mLianShaShow.RemoveSelf();
			}
			else
			{
				Object.Destroy(this.mLianShaShow.gameObject);
				this.mLianShaShow = null;
			}
		}
	}

	private void RefreshLianSha(int Num, long OverTicks, long BufferSecs)
	{
		if (0 < Num && OverTicks > Global.GetCorrectLocalTime())
		{
			if (null == this.mLianShaShow)
			{
				this.mLianShaShow = U3DUtils.NEW<LianShaShow>();
				PlayZone.GlobalPlayZone.Children.Add(this.mLianShaShow);
			}
			this.mLianShaShow.RefreshData(OverTicks, BufferSecs);
			this.mLianShaShow.RefreshValue(Num);
			this.mLianShaShow.ActiveSelf();
		}
		else
		{
			this.RemoveLianSha();
		}
	}

	public void ClearData()
	{
		this.RemoveLianSha();
	}

	private static LianShaMgr mInstance;

	private LianShaShow mLianShaShow;
}
