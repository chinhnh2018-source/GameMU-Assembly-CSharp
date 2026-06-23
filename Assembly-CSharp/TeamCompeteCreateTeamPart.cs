using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeamCompeteCreateTeamPart : UserControl, IMUEventManagerHandler
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitDict()
	{
		this.DictTouXiangPath.Add(0, "NetImages/Face/00_0.png.qj");
		this.DictTouXiangPath.Add(1, "NetImages/Face/10_0.png.qj");
		this.DictTouXiangPath.Add(2, "NetImages/Face/20_0.png.qj");
		this.DictTouXiangPath.Add(3, "NetImages/Face/30_0.png.qj");
		this.DictTouXiangPath.Add(5, "NetImages/Face/50_0.png.qj");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.mListBox.ItemsSource;
		this.mListBox.SelectionChanged = delegate(object s, MouseEvent e)
		{
			GameObject selectedItem = this.mListBox.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			if (this.item != null)
			{
				this.item.IsSelect = false;
			}
			this.item = selectedItem.GetComponent<TeamCompeteCreateTeamItem>();
			this.item.IsSelect = true;
			this.SingleZhanDuiData = this.item.miniData;
			this.RefreshRoleInfoList(this.item.miniData);
		};
		this.InitEvent();
		this.InitDict();
		this.LoadRoleInfo();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblTitle.Label.text = Global.GetLang("创建战队");
		this.LblZhanDuiName.Label.text = Global.GetLang("战队名称");
		this.LblZhanDuiLeader.Label.text = Global.GetLang("战队队长");
		this.LblZhanDuiDuanWei.Label.text = Global.GetLang("战队段位");
		this.BtnJoinTeam.Label.text = Global.GetLang("申请加入");
		this.BtnCreateTeam.Label.text = Global.GetLang("创建战队");
	}

	private void InitEvent()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnCreateTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
			if (PlayZone.GlobalPlayZone != null)
			{
				PlayZone.GlobalPlayZone.OpenTeamCompeteCreatePart();
			}
		};
		this.BtnJoinTeam.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.SingleZhanDuiData != null)
			{
				GameInstance.Game.RequestJoinTeamCompete(this.SingleZhanDuiData.ZhanDuiID);
			}
			else
			{
				MUDebug.LogError<string>(new string[]
				{
					"无战队信息"
				});
			}
		};
	}

	private void InitValue()
	{
		this.CostDiamond = ConfigSystemParam.GetSystemParamIntByName("TeamNeedZuan");
		this.mCfgNameLengthLimit = ConfigSystemParam.GetSystemParamIntArrayByName("TeamBattleNameRange", ',');
		if (this.mCfgNameLengthLimit.Length <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"SystemParam---TeamBattleNameRange---有误！"
			});
		}
	}

	private void LoadRoleInfo()
	{
		if (this.RoleInfoList.Count <= 0)
		{
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.mRoleInfoObj);
				gameObject.SetActive(true);
				gameObject.transform.SetParent(this.parent.transform);
				gameObject.transform.localPosition = new Vector3(this.firstPosition.x + (float)(i * this.offset), this.firstPosition.y, this.firstPosition.z);
				this.RoleInfoList.Add(gameObject);
			}
		}
		else
		{
			this.ClearRoleInfo();
		}
	}

	private void RefreshRoleInfoList(TianTi5v5ZhanDuiMiniData data)
	{
		if (this.RoleInfoList.Count <= 0)
		{
			return;
		}
		this.LoadRoleInfo();
		if (string.IsNullOrEmpty(data.XuanYan))
		{
			this.LblXuanYanContent.Text = Global.GetLang("暂无宣言");
		}
		else
		{
			this.LblXuanYanContent.Text = data.XuanYan;
		}
		if (data.MemberList == null || data.MemberList.Count <= 0)
		{
			MUDebug.LogError<string>(new string[]
			{
				"data.MemberList 为空 ，右下角角色信息无法显示"
			});
			return;
		}
		for (int i = 0; i < data.MemberList.Count; i++)
		{
			RoleNameLevelData roleNameLevelData = data.MemberList[i];
			GameObject gameObject = this.RoleInfoList[i];
			if (roleNameLevelData.ZhiWu)
			{
				Transform transform = gameObject.transform.FindChild("SpriteTeamLeaderFlag");
				if (transform != null)
				{
					transform.gameObject.SetActive(true);
				}
			}
			ShowNetImage component = gameObject.transform.FindChild("ImgIcon").GetComponent<ShowNetImage>();
			if (component != null)
			{
				component.URL = TeamCompeteDataManager.GetTouXiangPathByOccu(roleNameLevelData.Occupation);
			}
			TextBlock component2 = gameObject.transform.FindChild("LblLevel").GetComponent<TextBlock>();
			if (component2 != null)
			{
				component2.Text = Global.GetString(new object[]
				{
					roleNameLevelData.ZhuanSheng,
					Global.GetLang("转"),
					"\n",
					roleNameLevelData.Level,
					Global.GetLang("级")
				});
			}
		}
	}

	private void ClearRoleInfo()
	{
		for (int i = 0; i < this.RoleInfoList.Count; i++)
		{
			GameObject gameObject = this.RoleInfoList[i];
			Transform transform = gameObject.transform.FindChild("SpriteTeamLeaderFlag");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
			ShowNetImage component = gameObject.transform.FindChild("ImgIcon").GetComponent<ShowNetImage>();
			if (component != null)
			{
				component.URL = this.GetTouXiangPathByOccu(-1);
			}
			TextBlock component2 = gameObject.transform.FindChild("LblLevel").GetComponent<TextBlock>();
			if (component2 != null)
			{
				component2.Text = string.Empty;
			}
		}
	}

	private string GetTouXiangPathByOccu(int occu)
	{
		string empty = string.Empty;
		if (occu < 0)
		{
			return empty;
		}
		if (this.DictTouXiangPath.TryGetValue(occu, ref empty))
		{
			return empty;
		}
		return empty;
	}

	private bool IsDiamondEnough
	{
		get
		{
			return (long)Global.Data.roleData.UserMoney >= this.CostDiamond;
		}
	}

	private bool IsLevelEnough
	{
		get
		{
			this.levels = ConfigSystemParam.GetSystemParamIntArrayByName("TeamLevelLimit", ',');
			return Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 1000 >= this.levels[0] * 1000 + this.levels[1];
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_REQUEST_TO_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondJoinTeam));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<MUSocketConnectEventArgs>("CMD_SPR_KF5V5_REQUEST_TO_ZHANDUI", new Action<MUSocketConnectEventArgs>(this.RespondJoinTeam));
	}

	public void RespondJoinTeam(MUSocketConnectEventArgs e)
	{
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num >= 0)
		{
			Super.HintMainText(Global.GetLang("申请成功，等待队长确认"), 10, 3);
		}
		else
		{
			TeamCompeteDataManager.ErrorTips(num);
		}
	}

	public void RequestCreateTeamMsg(string name)
	{
		if (0 < name.Length && name.Length < this.mCfgNameLengthLimit[this.min])
		{
			Super.HintMainText(string.Format(Global.GetLang("战队名称不能少于{0}个字符，请重新输入！"), this.mCfgNameLengthLimit[this.min]), 10, 3);
			return;
		}
		if (name.Length > this.mCfgNameLengthLimit[this.max])
		{
			Super.HintMainText(string.Format(Global.GetLang("战队名称已超过{0}个字符，请重新输入！"), this.mCfgNameLengthLimit[this.max]), 10, 3);
			return;
		}
		WordsFilterMgr.ExecWordsFilter(name, delegate(object content, ExecWordsFilterEventArgs result)
		{
			if (result.ret > 0)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("与过滤词服务器通讯失败:{0}, {1}"), new object[]
				{
					result.ret,
					result.msg
				}), 10, 3);
				return;
			}
			if (result.is_dirty > 0)
			{
				Super.HintMainText(Global.GetLang("战队宣言不能包含国家规定禁止的词汇!"), 10, 3);
				return;
			}
			GameInstance.Game.SendCreateTeamMsg(name, null);
		});
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	public void LoadItems(List<TianTi5v5ZhanDuiMiniData> data)
	{
		if (data == null || data.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < data.Count; i++)
		{
			TeamCompeteCreateTeamItem teamCompeteCreateTeamItem = U3DUtils.NEW<TeamCompeteCreateTeamItem>();
			teamCompeteCreateTeamItem.InitValue(data[i]);
			NGUITools.AddChild2(this.mListBox.gameObject, teamCompeteCreateTeamItem);
			this.ItemCollection.Add(teamCompeteCreateTeamItem);
		}
		if (this.ItemCollection.Count > 0)
		{
			this.mListBox.SelectedIndex = 0;
		}
	}

	private void CreateTeamCallBack(bool result)
	{
		if (this.OnCreateTeamCallBack != null)
		{
			this.OnCreateTeamCallBack.Invoke(result);
		}
	}

	public void OpenTeamCompeteCreateTeamErJiWindow()
	{
		if (this.mTeamCompeteCreateTeamErJiWindowWind != null || this.mTeamCompeteCreateTeamErJiWindow != null)
		{
			this.CloseTeamCompeteCreateTeamErJiWindow();
		}
		this.mTeamCompeteCreateTeamErJiWindowWind = U3DUtils.NEW<GChildWindow>();
		this.mTeamCompeteCreateTeamErJiWindowWind.ModalType = ChildWindowModalType.Translucent;
		this.mTeamCompeteCreateTeamErJiWindowWind.Modal = true;
		this.mTeamCompeteCreateTeamErJiWindowWind.IsShowModal = false;
		Super.InitChildWindow(this.mTeamCompeteCreateTeamErJiWindowWind, "mTeamCompeteCreateTeamErJiWindowWind");
		Super.GData.GlobalPlayZone.Children.Add(this.mTeamCompeteCreateTeamErJiWindowWind);
		this.mTeamCompeteCreateTeamErJiWindow = U3DUtils.NEW<TeamCompeteCreateTeamErJiWindow>();
		this.mTeamCompeteCreateTeamErJiWindowWind.Body.Add(this.mTeamCompeteCreateTeamErJiWindow);
		this.mTeamCompeteCreateTeamErJiWindow.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteCreateTeamErJiWindow();
		};
		this.mTeamCompeteCreateTeamErJiWindow.ClickHandler = delegate(object s, DPSelectedItemEventArgs e)
		{
			this.CloseTeamCompeteCreateTeamErJiWindow();
			this.CloseUI();
		};
	}

	private void CloseTeamCompeteCreateTeamErJiWindow()
	{
		if (null != this.mTeamCompeteCreateTeamErJiWindowWind)
		{
			Super.CloseChildWindow(base.Children, this.mTeamCompeteCreateTeamErJiWindowWind);
			Super.GData.GlobalPlayZone.Children.Remove(this.mTeamCompeteCreateTeamErJiWindowWind, true);
			this.mTeamCompeteCreateTeamErJiWindowWind = null;
		}
		if (null != this.mTeamCompeteCreateTeamErJiWindow)
		{
			this.mTeamCompeteCreateTeamErJiWindow.transform.parent = null;
			Object.Destroy(this.mTeamCompeteCreateTeamErJiWindow.gameObject);
			this.mTeamCompeteCreateTeamErJiWindow = null;
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public Action<bool> OnCreateTeamCallBack;

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblTitle;

	public TextBlock LblZhanDuiName;

	public TextBlock LblZhanDuiLeader;

	public TextBlock LblZhanDuiDuanWei;

	public GButton BtnClose;

	public TextBlock LblXuanYanContent;

	public GButton BtnCreateTeam;

	public GButton BtnJoinTeam;

	public GameObject mRoleInfoObj;

	public new GameObject parent;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	private long CostDiamond;

	private int[] levels;

	private int[] mCfgNameLengthLimit;

	private int min;

	private int max = 1;

	private Vector3 firstPosition = new Vector3(-153f, -109f, 0f);

	private int offset = 77;

	private List<GameObject> RoleInfoList = new List<GameObject>();

	private Dictionary<int, string> DictTouXiangPath = new Dictionary<int, string>();

	private TianTi5v5ZhanDuiMiniData SingleZhanDuiData;

	private TeamCompeteCreateTeamItem item;

	protected GChildWindow mTeamCompeteCreateTeamErJiWindowWind;

	protected TeamCompeteCreateTeamErJiWindow mTeamCompeteCreateTeamErJiWindow;
}
