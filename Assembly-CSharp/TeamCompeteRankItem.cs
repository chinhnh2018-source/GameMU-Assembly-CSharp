using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class TeamCompeteRankItem : UserControl
{
	public TianTi5v5ZhanDuiData zhanDuiData { get; set; }

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitDict();
	}

	private void InitTextInPrefabs()
	{
		this.LblRank.Label.text = Global.GetLang(string.Empty);
		this.LblName.Label.text = Global.GetLang(string.Empty);
		this.LblDuanWei.Label.text = Global.GetLang(string.Empty);
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

	public int RankId { get; set; }

	public void InitValue(int rankId, TianTi5v5ZhanDuiData data)
	{
		this.zhanDuiData = data;
		this.RankId = rankId;
		NGUITools.SetActive(this.LblRank.gameObject, true);
		NGUITools.SetActive(this.SprtRank.gameObject, false);
		if (rankId <= 3)
		{
			NGUITools.SetActive(this.SprtRank.gameObject, true);
			NGUITools.SetActive(this.LblRank.gameObject, false);
			this.SprtRank.spriteName = "no." + rankId;
		}
		else
		{
			this.LblRank.Text = this.GetRankDes(rankId);
		}
		this.LblName.Text = data.ZhanDuiName;
		this.LblDuanWei.Text = Global.GetString(new object[]
		{
			Global.GetLang("段位："),
			TeamCompeteDataManager.GetDuanWeiNameByID(data.DuanWeiId)
		});
		if (data.teamerList.Count > 0)
		{
			TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = data.teamerList.Find((TianTi5v5ZhanDuiRoleData result) => result.RoleID == data.LeaderRoleID);
			if (tianTi5v5ZhanDuiRoleData != null)
			{
				this.mTextureTouXiang.URL = this.GetTouXiangPathByOccu(tianTi5v5ZhanDuiRoleData.RoleOcc);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"服务器排行数据错误！没有找到队长数据，不能正常显示队长形象信息！！！"
				});
			}
		}
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

	private string GetRankDes(int rankId)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Global.GetLang("第"));
		stringBuilder.Append(rankId);
		stringBuilder.Append(Global.GetLang("名"));
		return stringBuilder.ToString();
	}

	public bool IsSelect
	{
		set
		{
			this.SprtBak.spriteName = ((!value) ? this.names[0] : this.names[1]);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public UISprite SprtRank;

	public TextBlock LblRank;

	public TextBlock LblName;

	public TextBlock LblDuanWei;

	public UISprite SprtBak;

	public ShowNetImage mTextureTouXiang;

	private string[] names = new string[]
	{
		"DuanWeiDiKuang1",
		"DuanWeiDiKuang2"
	};

	private Dictionary<int, string> DictTouXiangPath = new Dictionary<int, string>();
}
