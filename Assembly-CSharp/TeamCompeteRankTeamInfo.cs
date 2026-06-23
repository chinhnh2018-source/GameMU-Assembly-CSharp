using System;
using System.Collections.Generic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class TeamCompeteRankTeamInfo : UserControl
{
	public TianTi5v5ZhanDuiData zhanDuiData { get; set; }

	public TianTi5v5ZhanDuiRoleData roleData { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitDict();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
	}

	private void InitDict()
	{
		this.DictTouXiangPath.Add(0, "NetImages/Face/00_0.png.qj");
		this.DictTouXiangPath.Add(1, "NetImages/Face/10_0.png.qj");
		this.DictTouXiangPath.Add(2, "NetImages/Face/20_0.png.qj");
		this.DictTouXiangPath.Add(3, "NetImages/Face/30_0.png.qj");
		this.DictTouXiangPath.Add(5, "NetImages/Face/50_0.png.qj");
	}

	public void InitValue(TianTi5v5ZhanDuiData data, TianTi5v5ZhanDuiRoleData _roleData)
	{
		this.zhanDuiData = data;
		this.roleData = _roleData;
		this.mTouXiang.URL = this.DictTouXiangPath[_roleData.RoleOcc];
	}

	private string GetTouXiangPathByOccu(int occu)
	{
		string empty = string.Empty;
		if (this.DictTouXiangPath.TryGetValue(occu, ref empty))
		{
			return empty;
		}
		MUDebug.LogError<string>(new string[]
		{
			"头像不存在！"
		});
		return empty;
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		this.DictTouXiangPath.Clear();
	}

	public DPSelectedItemEventHandler ClickHandler;

	public ShowNetImage mTouXiang;

	private Dictionary<int, string> DictTouXiangPath = new Dictionary<int, string>();
}
