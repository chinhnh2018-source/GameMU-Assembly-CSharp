using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeamCompeteTeamItemPart : UserControl
{
	public bool IsSelect
	{
		get
		{
			return this.mIsSelect;
		}
		set
		{
			this.mIsSelect = value;
			this.mSelect.gameObject.SetActive(value);
		}
	}

	private void IsShowInfoCompnt(bool isShow)
	{
		NGUITools.SetActive(this.LblName.gameObject, isShow);
		NGUITools.SetActive(this.LblZhuanShu.gameObject, isShow);
		NGUITools.SetActive(this.LblBattleValue.gameObject, isShow);
		NGUITools.SetActive(this.LblStatus.gameObject, isShow);
		NGUITools.SetActive(this.ImgTouXiang.gameObject, isShow);
		NGUITools.SetActive(this.TouXiangKuang.gameObject, isShow);
		NGUITools.SetActive(this.LblNull.gameObject, !isShow);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.IsTeamLeader = false;
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitDict();
	}

	private void InitTextInPrefabs()
	{
		this.LblName.Label.text = Global.GetLang(string.Empty);
		this.LblZhuanShu.Label.text = Global.GetLang("0转");
		this.LblBattleValue.Label.text = Global.GetLang("战斗力:");
		this.LblStatus.Label.text = Global.GetLang("离线");
		this.LblNull.Label.text = Global.GetLang("空");
	}

	private void InitDict()
	{
		this.DictTouXiangPath.Add(0, "NetImages/Face/00_0.png.qj");
		this.DictTouXiangPath.Add(1, "NetImages/Face/10_0.png.qj");
		this.DictTouXiangPath.Add(2, "NetImages/Face/20_0.png.qj");
		this.DictTouXiangPath.Add(3, "NetImages/Face/30_0.png.qj");
		this.DictTouXiangPath.Add(5, "NetImages/Face/50_0.png.qj");
	}

	private void InitEvent()
	{
	}

	public void InitValue(TianTi5v5ZhanDuiRoleData data, int teamLeaderId)
	{
		if (data == null || teamLeaderId <= 0)
		{
			this.ID = -1;
			this.IsTeamLeader = false;
			this.IsShowInfoCompnt(false);
		}
		else
		{
			this.IsShowInfoCompnt(true);
			this.ID = data.RoleID;
			this.IsTeamLeader = (teamLeaderId == this.ID);
			this.IsOnline = (data.OnlineState == 1);
			this.Name = data.RoleName;
			this.LblZhuanShu.Text = Global.GetString(new object[]
			{
				data.ZhuanSheng,
				Global.GetLang("转"),
				data.Level,
				Global.GetLang("级")
			});
			this.LblBattleValue.Text = Global.GetString(new object[]
			{
				Global.GetLang("战力："),
				data.ZhanLi
			});
			this.ImgTouXiang.URL = this.GetTouXiangPathByOccu(data.RoleOcc);
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

	private bool IsOnline
	{
		get
		{
			return this.isOnline;
		}
		set
		{
			this.isOnline = value;
			this.LblStatus.Label.text = ((!value) ? Global.GetLang("离线") : Global.GetLang("在线"));
			if (this.isOnline)
			{
				this.ImgTouXiang.Texture.shader = Shader.Find("Unlit/Transparent Colored");
			}
			else
			{
				this.ImgTouXiang.Texture.shader = Shader.Find("Unlit/Gray");
			}
		}
	}

	public new string Name
	{
		get
		{
			return this.mName;
		}
		set
		{
			this.mName = value;
			this.LblName.Text = value;
		}
	}

	public int ID
	{
		get
		{
			return this.mID;
		}
		set
		{
			this.mID = value;
		}
	}

	public bool IsTeamLeader
	{
		get
		{
			return this.mIsTeamLeader;
		}
		set
		{
			this.mIsTeamLeader = value;
			NGUITools.SetActive(this.SprtTeamLeader.gameObject, value);
		}
	}

	public bool IsYourSelf
	{
		get
		{
			return Global.Data != null && Global.Data.roleData.RoleID == this.ID;
		}
	}

	public void OpenTeamCompeteErJiTipsPart(int id, string name, Action<int> ChangeLeaderCallBack = null, Action<int> DeleteMemberCallBack = null)
	{
		if (this.mTeamCompeteErJiTipsPartWind != null || this.mTeamCompeteErJiTipsPart != null)
		{
			this.CloseTeamCompeteErJiTipsPart();
		}
		this.mTeamCompeteErJiTipsPartWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteErJiTipsPartWind.ModalType = ChildWindowModalType.TransBak;
		this.mTeamCompeteErJiTipsPartWind.Modal = true;
		this.mTeamCompeteErJiTipsPartWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteErJiTipsPartWind, "mTeamCompeteErJiTipsPartWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteErJiTipsPartWind);
		this.mTeamCompeteErJiTipsPart = U3DUtils.NEW<TeamCompeteErJiTipsPart>();
		this.mTeamCompeteErJiTipsPart.InitValue(id, name);
		this.mTeamCompeteErJiTipsPart.ChangeLeaderCallBack = ChangeLeaderCallBack;
		this.mTeamCompeteErJiTipsPart.DeleteMemberCallBack = DeleteMemberCallBack;
		this.mTeamCompeteErJiTipsPartWind.Body.Add(this.mTeamCompeteErJiTipsPart);
		this.mTeamCompeteErJiTipsPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteErJiTipsPart();
		};
	}

	private void CloseTeamCompeteErJiTipsPart()
	{
		if (null != this.mTeamCompeteErJiTipsPartWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteErJiTipsPartWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteErJiTipsPartWind, true);
			this.mTeamCompeteErJiTipsPartWind = null;
		}
		if (null != this.mTeamCompeteErJiTipsPart)
		{
			this.mTeamCompeteErJiTipsPart.transform.parent = null;
			Object.Destroy(this.mTeamCompeteErJiTipsPart.gameObject);
			this.mTeamCompeteErJiTipsPart = null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblName;

	public TextBlock LblZhuanShu;

	public TextBlock LblBattleValue;

	public TextBlock LblStatus;

	public UISprite SprtTeamLeader;

	public UISprite TouXiangKuang;

	public TextBlock LblNull;

	public ShowNetImage ImgTouXiang;

	private Dictionary<int, string> DictTouXiangPath = new Dictionary<int, string>();

	public UISprite mSelect;

	private bool mIsSelect;

	private bool isOnline;

	private string mName = string.Empty;

	private int mID;

	private bool mIsTeamLeader;

	protected GChildWindow mTeamCompeteErJiTipsPartWind;

	protected TeamCompeteErJiTipsPart mTeamCompeteErJiTipsPart;
}
