using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class DaTaoShaSceneRightBox : UserControl
{
	private bool IsShowTaskRootDes
	{
		set
		{
			NGUITools.SetActive(this.TaskRoot, value);
			NGUITools.SetActive(this.Des, value);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.LblDes.Text = Global.GetLang("距离天神怒焰扩散");
		this.LblTime.Text = Global.GetLang("剩余：3分60秒");
		this.LblTitle.Text = Global.GetLang("生命种子采集数量");
		this.LblCaiJiTipDes.Text = Global.GetLang("战队累积采集数量达到50颗，战队成员复活次数+1");
		this.LblDiamond.Text = Global.GetLang("X50");
		this.BtnGuanZhan.Label.text = Global.GetLang("选择角色");
		this.BtnBuy.Label.text = Global.GetLang("购买");
		this.LblTips.Text = Global.GetLang("战场已进入最后阶段，为战队奋勇而战");
	}

	private void InitEvent()
	{
		DaTaoShaDataManager.BianShenGuanZhanCallBack = delegate(int s)
		{
			if (this.CurrentTrackRoleId > 0 && s == this.CurrentTrackRoleId)
			{
				this.GuanZhanOtherPlayer(this.CurrentTrackRoleId);
			}
		};
		DaTaoShaDataManager.HideBuyAbutton = delegate()
		{
			NGUITools.SetActive(this.LblDiamond.gameObject, false);
			NGUITools.SetActive(this.mBuy.gameObject, false);
		};
		this.BtnGuanZhan.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GetGameCtrlBar != null)
			{
				PlayZone.GlobalPlayZone.GetGameCtrlBar.IsShowSkill = false;
			}
			GameInstance.Game.GetGuanZhanRoleMiniDatalist();
		};
		UIEventListener.Get(this.mBuy.gameObject).onClick = delegate(GameObject s)
		{
			if (this.IsBuy)
			{
				return;
			}
			this.BuyBuff();
		};
		UIEventListener.Get(this.mSwitch.gameObject).onClick = delegate(GameObject s)
		{
			this.IsShowFlag = !this.IsShowFlag;
			this.IsShowTaskRootDes = this.IsShowFlag;
			this.mSwitch.spriteName = ((!this.IsShowFlag) ? "Taskarrow_02" : "Taskarrow1");
		};
		DaTaoShaDataManager.BuyMoShenBuffCallBack = delegate(bool s)
		{
			this.IsBuy = s;
			this.mBuy.shader = Shader.Find("Unlit/Gray");
			NGUITools.SetActive(this.mZuanShiNum, false);
			NGUITools.SetActive(this.mZuanShiIcon, false);
		};
		DaTaoShaDataManager.ShowBuyMoShenBuffCallBack = delegate(bool s)
		{
			this.IsBuy = s;
			this.mBuy.shader = Shader.Find("Unlit/Transparent Colored");
			NGUITools.SetActive(this.mZuanShiNum, true);
			NGUITools.SetActive(this.mZuanShiIcon, true);
		};
		DaTaoShaDataManager.ShowGuanZhanBtnCallBack = delegate()
		{
			this.GuanZhanStage();
		};
	}

	private void InitValue()
	{
		this.TeamNames.Add(this.LblTeam1);
		this.TeamNames.Add(this.LblTeam2);
		this.TeamNames.Add(this.LblTeam3);
		this.TeamCaiJiCount.Add(this.LblTeam1Score);
		this.TeamCaiJiCount.Add(this.LblTeam2Score);
		this.TeamCaiJiCount.Add(this.LblTeam3Score);
		this.buySingleBuffCostDiamond = (int)ConfigSystemParam.GetSystemParamIntByName("BuyDevilLossDiamonds");
		this.MaxBuffCounts = ConfigSystemParam.GetSystemParamIntArrayByName("BuffMaxLayerNum", '|');
	}

	private void PrepareSafeStage()
	{
		NGUITools.SetActive(this.mObjTianShenNuYan, false);
		NGUITools.SetActive(this.mObjCaiJi, false);
		NGUITools.SetActive(this.mObjGuanZhan, false);
		NGUITools.SetActive(this.mObjShopping, false);
	}

	private void CaiJiStage(List<EscapeBattleTeamInfo> BattleTeamList)
	{
		NGUITools.SetActive(this.mObjTianShenNuYan, false);
		NGUITools.SetActive(this.mObjCaiJi, true);
		NGUITools.SetActive(this.mObjGuanZhan, false);
		NGUITools.SetActive(this.mObjShopping, false);
		for (int i = 0; i < this.TeamNames.Count; i++)
		{
			if (BattleTeamList != null && BattleTeamList.Count > 0)
			{
				if (i <= BattleTeamList.Count - 1)
				{
					this.TeamNames[i].Text = BattleTeamList[i].TeamName;
					int lifeSeed = BattleTeamList[i].LifeSeed;
					if (BattleTeamList[i].TeamID == Global.Data.roleData.ZhanDuiID)
					{
						this.UpdateCaiJiInfo(lifeSeed);
					}
					this.TeamCaiJiCount[i].Text = lifeSeed.ToString();
				}
				else
				{
					this.TeamNames[i].Text = string.Empty;
					this.TeamCaiJiCount[i].Text = string.Empty;
				}
			}
			else
			{
				this.TeamNames[i].Text = string.Empty;
				this.TeamCaiJiCount[i].Text = string.Empty;
			}
		}
	}

	private void UpdateCaiJiInfo(int count)
	{
		bool flag = false;
		if (count >= DaTaoShaDataManager.MaxCaiJiShengMingCount)
		{
			flag = true;
		}
		this.LblCaiJiTipDes.Text = ((!flag) ? Global.GetString(new object[]
		{
			Global.GetLang("战队累积采集数量达到"),
			DaTaoShaDataManager.GetLimitLifeSeed(count),
			Global.GetLang("颗，战队成员复活次数+1")
		}) : Global.GetLang("战队累积采集数量已达上限，获得复活次数+1"));
	}

	private void ShaLuStage(DateTime endDateTime, bool isMax = false)
	{
		this.MaxBuffCount = this.MaxBuffCounts[0];
		if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GetGameCtrlBar != null)
		{
			PlayZone.GlobalPlayZone.GetGameCtrlBar.IsShowSkill = true;
		}
		NGUITools.SetActive(this.mObjTianShenNuYan, true);
		NGUITools.SetActive(this.mObjCaiJi, false);
		NGUITools.SetActive(this.mObjGuanZhan, false);
		NGUITools.SetActive(this.mObjShopping, false);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		MUDebug.LogError<string>(new string[]
		{
			"ShaLuStage endDateTime " + endDateTime.ToString("yyyy-MM-dd HH:mm:ss")
		});
		long num = (endDateTime.Ticks - correctDateTime.Ticks) / 10000000L;
		if (num > 0L)
		{
			base.StopAllCoroutines();
			base.StartCoroutine<bool>(this.CountDown(num, isMax));
		}
	}

	private IEnumerator CountDown(long seconds, bool isMax = false)
	{
		while (seconds > 0L)
		{
			yield return new WaitForSeconds(1f);
			this.LblTime.Text = Global.GetLang("剩余：") + ActivityTimeManager.GetTimeStrBySecEx((double)seconds);
			seconds -= 1L;
			if (isMax && !this.hasHide)
			{
				NGUITools.SetActive(this.mObjTianShenNuYan, false);
				this.ShowLastTips();
				this.hasHide = true;
			}
		}
		yield break;
	}

	private void KuangReStage(DateTime endDateTime, bool isMax = false)
	{
		if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GetGameCtrlBar != null)
		{
			PlayZone.GlobalPlayZone.GetGameCtrlBar.IsShowSkill = true;
		}
		this.MaxBuffCount = this.MaxBuffCounts[1];
		this.buffCount = DaTaoShaDataManager.EscapeBattleDevilCount;
		int num = this.MaxBuffCount - this.buffCount;
		if (num > 0)
		{
			NGUITools.SetActive(this.mObjTianShenNuYan, true);
			NGUITools.SetActive(this.mObjCaiJi, false);
			NGUITools.SetActive(this.mObjGuanZhan, false);
			NGUITools.SetActive(this.mObjShopping, true);
			this.LblDiamond.Text = Global.GetString(new object[]
			{
				"X",
				this.buySingleBuffCostDiamond * num
			});
		}
		else
		{
			NGUITools.SetActive(this.mObjTianShenNuYan, true);
			NGUITools.SetActive(this.mObjCaiJi, false);
			NGUITools.SetActive(this.mObjGuanZhan, false);
			NGUITools.SetActive(this.mObjShopping, false);
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		MUDebug.LogError<string>(new string[]
		{
			"KuangReStage endDateTime " + endDateTime.ToString("yyyy-MM-dd HH:mm:ss")
		});
		long num2 = (endDateTime.Ticks - correctDateTime.Ticks) / 10000000L;
		if (num2 > 0L)
		{
			base.StopAllCoroutines();
			base.StartCoroutine<bool>(this.CountDown(num2, isMax));
		}
		else
		{
			NGUITools.SetActive(this.mObjTianShenNuYan, false);
		}
	}

	private void BuyBuff()
	{
		if (this.MaxBuffCount - this.buffCount > 0)
		{
			this.PopupBuyBuffWindow(this.MaxBuffCount - this.buffCount);
		}
	}

	public void PopupBuyBuffWindow(int diamondCount)
	{
		if (DaTaoShaSceneRightBox.EnterBuyBuffWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, DaTaoShaSceneRightBox.EnterBuyBuffWindow);
			DaTaoShaSceneRightBox.EnterBuyBuffWindow = null;
		}
		string message = Global.GetLang("魔神契约，借助魔神的力量，消耗") + diamondCount * this.buySingleBuffCostDiamond + Global.GetLang("钻石，可获得最高魔神狂热效果。");
		DaTaoShaSceneRightBox.EnterBuyBuffWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
		DaTaoShaSceneRightBox.EnterBuyBuffWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
		{
			int messageBoxReturn = DaTaoShaSceneRightBox.EnterBuyBuffWindow.MessageBoxReturn;
			Super.CloseMessageBox(Super.MainWindowRoot, DaTaoShaSceneRightBox.EnterBuyBuffWindow);
			if (messageBoxReturn == 0)
			{
				GameInstance.Game.RequestBuyMoShenBuff();
			}
			return true;
		};
	}

	public void GuanZhanStage()
	{
		NGUITools.SetActive(this.mObjTianShenNuYan, false);
		NGUITools.SetActive(this.mObjCaiJi, false);
		NGUITools.SetActive(this.mObjGuanZhan, true);
		NGUITools.SetActive(this.mObjShopping, false);
		if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GetGameCtrlBar != null)
		{
			PlayZone.GlobalPlayZone.GetGameCtrlBar.IsShowSkill = false;
		}
	}

	public void ShowLastTips()
	{
		NGUITools.SetActive(this.LblTips.gameObject, true);
		base.Invoke("ShowLastTipsTime", 5f);
	}

	private void ShowLastTipsTime()
	{
		NGUITools.SetActive(this.LblTips.gameObject, false);
	}

	private IEnumerator ShowTipsCountDown()
	{
		yield return new WaitForSeconds(5f);
		NGUITools.SetActive(this.LblTips.gameObject, false);
		yield break;
	}

	private new void Update()
	{
		if (this.IsTracking)
		{
			if (this.sp == null && Global.Data != null && Global.Data.roleData != null)
			{
				this.sp = ObjectsManager.FindSprite(Global.Data.roleData.RoleID);
			}
			if (this.sp != null && this.otherTransform != null)
			{
				this.sp.Coordinate = new Point((int)(this.otherTransform.localPosition.x + 1f) * 100, (int)(this.otherTransform.localPosition.z + 1f) * 100);
			}
		}
		else
		{
			this.sp = null;
		}
	}

	public void GuanZhanData(int trackrid, int result)
	{
		MUDebug.LogError<string>(new string[]
		{
			string.Concat(new object[]
			{
				"GuanZhanData trackrid ",
				trackrid,
				" result ",
				result
			})
		});
		if (trackrid > 0)
		{
			if (result < 0)
			{
				if (result == -21)
				{
					Super.HintMainText(Global.GetLang("目标角色已离开"), 10, 3);
				}
				else if (result == -12)
				{
					Super.HintMainText(Global.GetLang("当前无法操作"), 10, 3);
				}
				else
				{
					Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(result, false, false)), 10, 3);
				}
				GameInstance.Game.GetGuanZhanRoleMiniDatalist();
				return;
			}
			this.HideDeadBody();
			this.GuanZhanOtherPlayer(trackrid);
		}
	}

	public void RespondmDaTaoShaRightBoxMiniInfo(MUSocketConnectEventArgs e)
	{
		EscapeBattleSideScore escapeBattleSideScore = DataHelper.BytesToObject<EscapeBattleSideScore>(e.bytesData, 0, e.bytesData.Length);
		if (escapeBattleSideScore == null)
		{
			return;
		}
		this.RespondDaTaoShaBattleSceneInfo(escapeBattleSideScore);
	}

	private void RespondDaTaoShaBattleSceneInfo(EscapeBattleSideScore data)
	{
		if (data == null)
		{
			return;
		}
		DaTaoShaDataManager.RelifeCount = data.ReliveCount;
		if (DaTaoShaDataManager.RelifeCountCallBack != null)
		{
			DaTaoShaDataManager.RelifeCountCallBack.Invoke(data.ReliveCount);
		}
		EscapeBattleGameSceneStatuses eStatus = data.eStatus;
		DaTaoShaDataManager.EBattleStatus = eStatus;
		MUDebug.LogError<string>(new string[]
		{
			"RespondDaTaoShaBattleSceneInfo status " + eStatus.ToString()
		});
		switch (eStatus)
		{
		case EscapeBattleGameSceneStatuses.STATUS_NULL:
			this.PrepareSafeStage();
			break;
		case EscapeBattleGameSceneStatuses.STATUS_PREPARE:
			this.PrepareSafeStage();
			break;
		case EscapeBattleGameSceneStatuses.STATUS_BEGIN:
			this.CaiJiStage(data.BattleTeamList);
			break;
		case EscapeBattleGameSceneStatuses.STATUS_FIGHT:
			if (DaTaoShaDataManager.ShowShaLuTips != null)
			{
				DaTaoShaDataManager.ShowShaLuTips.Invoke();
			}
			if (DaTaoShaDataManager.IsGuanZhan)
			{
				this.GuanZhanStage();
			}
			else
			{
				int areaID = data.targetSafeArea.AreaID;
				int areaID2 = data.safeArea.AreaID;
				bool isMax = false;
				if (areaID > 1 && areaID2 > 1 && areaID == areaID2)
				{
					isMax = true;
					MUDebug.LogError<string>(new string[]
					{
						string.Concat(new object[]
						{
							"111活动已进入最后阶段： targetId ",
							areaID,
							" safeId ",
							areaID2
						})
					});
				}
				this.ShaLuStage(data.AreaChangeTm, isMax);
			}
			break;
		case EscapeBattleGameSceneStatuses.STATUS_ASS:
			if (DaTaoShaDataManager.IsGuanZhan)
			{
				this.GuanZhanStage();
			}
			else
			{
				int areaID3 = data.targetSafeArea.AreaID;
				int areaID4 = data.safeArea.AreaID;
				bool isMax2 = false;
				if (areaID3 > 1 && areaID4 > 1 && areaID3 == areaID4)
				{
					isMax2 = true;
					MUDebug.LogError<string>(new string[]
					{
						string.Concat(new object[]
						{
							"222活动已进入最后阶段： targetId ",
							areaID3,
							" safeId ",
							areaID4
						})
					});
				}
				this.KuangReStage(data.AreaChangeTm, isMax2);
			}
			break;
		}
		if (DaTaoShaDataManager.EGRadarMapBattleStatusCallBack != null)
		{
			DaTaoShaDataManager.EGRadarMapBattleStatusCallBack.Invoke(eStatus);
		}
		if (DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack != null)
		{
			DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack.Invoke(eStatus);
		}
		DaTaoShaDataManager.RefreshUICirclePos(data);
		EscapeBattleAreaInfo safeArea = data.safeArea;
		Vector3 pos;
		pos..ctor((float)(safeArea.PosX / 100), 50f, (float)(safeArea.PosY / 100));
		if (eStatus >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
		{
			DaTaoShaDataManager.LoadEffectCircle(safeArea.AreaID, pos);
		}
	}

	public void GuanZhanOtherPlayer(int otherRoleId)
	{
		if (this.CurrentTrackRoleId > 0 && this.CurrentTrackRoleId != otherRoleId)
		{
			MUDebug.LogError<string>(new string[]
			{
				"服务器主动推送换观战新角色"
			});
		}
		this.CurrentTrackRoleId = otherRoleId;
		if (this.otherSprite != null)
		{
			GameObject the3DGameObject = this.otherSprite.The3DGameObject;
			CameraController cameraController = null;
			if (the3DGameObject != null)
			{
				cameraController = the3DGameObject.GetComponent<CameraController>();
			}
			if (cameraController != null)
			{
				cameraController.enabled = false;
			}
			this.otherTransform = null;
			this.otherSprite = null;
		}
		this.otherSprite = Global.FindSpriteByID(otherRoleId);
		if (this.otherSprite != null)
		{
			GSprite gsprite = ObjectsManager.FindSprite(Global.Data.roleData.RoleID);
			if (gsprite != null)
			{
				GameObject the3DGameObject2 = gsprite.The3DGameObject;
				if (the3DGameObject2 != null)
				{
					the3DGameObject2.GetComponent<CameraController>().enabled = false;
				}
			}
			GameObject the3DGameObject3 = this.otherSprite.The3DGameObject;
			if (the3DGameObject3 != null)
			{
				CameraController cameraController2 = the3DGameObject3.GetComponent<CameraController>();
				if (cameraController2 == null)
				{
					cameraController2 = the3DGameObject3.AddComponent<CameraController>();
					cameraController2.enabled = true;
				}
				if (cameraController2 != null)
				{
					if (cameraController2.Cam == null)
					{
						cameraController2.Cam = Global.MainCamera.gameObject;
					}
					cameraController2.enabled = true;
				}
				this.otherTransform = the3DGameObject3.transform;
				this.IsTracking = true;
			}
		}
		else
		{
			this.IsTracking = false;
			this.otherTransform = null;
			MUDebug.LogError<string>(new string[]
			{
				"未找到该玩家，重新锁定！"
			});
			if (this.CurrentTrackRoleId > 0)
			{
				GameInstance.Game.SendGuanZhanTrackOtherPlayer(this.CurrentTrackRoleId);
			}
		}
	}

	private void HideDeadBody()
	{
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.CloseRoleLowLifeWindow();
		}
		GSprite gsprite = Global.FindSpriteByID(Global.Data.roleData.RoleID);
		Transform transform = null;
		if (gsprite != null)
		{
			transform = gsprite.The3DGameObject.transform;
		}
		if (transform != null)
		{
			int childCount = transform.childCount;
			if (childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
			{
				for (int i = 0; i < childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(false);
				}
				SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
				if (component)
				{
					component.enabled = false;
				}
				BoxCollider component2 = transform.GetComponent<BoxCollider>();
				if (component2)
				{
					component2.enabled = false;
				}
			}
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.StopAllCoroutines();
		base.CancelInvoke();
		if (DaTaoShaSceneRightBox.EnterBuyBuffWindow != null)
		{
			Super.CloseMessageBox(Super.MainWindowRoot, DaTaoShaSceneRightBox.EnterBuyBuffWindow);
			DaTaoShaSceneRightBox.EnterBuyBuffWindow = null;
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public TextBlock LblDes;

	public TextBlock LblTime;

	public TextBlock LblTitle;

	public TextBlock LblTeam1;

	public TextBlock LblTeam1Score;

	public TextBlock LblTeam2;

	public TextBlock LblTeam2Score;

	public TextBlock LblTeam3;

	public TextBlock LblTeam3Score;

	public TextBlock LblCaiJiTipDes;

	public TextBlock LblDiamond;

	public TextBlock LblTips;

	public GButton BtnGuanZhan;

	public GButton BtnBuy;

	public UITexture mBuy;

	public GameObject mObjTianShenNuYan;

	public GameObject mObjCaiJi;

	public GameObject mObjGuanZhan;

	public GameObject mObjShopping;

	public GameObject mZuanShiNum;

	public GameObject mZuanShiIcon;

	public UISprite mSwitch;

	public GameObject TaskRoot;

	public GameObject Des;

	private bool IsBuy;

	private bool IsShowFlag = true;

	private int buySingleBuffCostDiamond;

	private int MaxBuffCount;

	private int[] MaxBuffCounts;

	private List<TextBlock> TeamNames = new List<TextBlock>();

	private List<TextBlock> TeamCaiJiCount = new List<TextBlock>();

	private bool hasHide;

	private int buffCount;

	private static GChildWindow EnterBuyBuffWindow;

	private bool IsTracking;

	private Transform leaderTransform;

	private Transform otherTransform;

	private GSprite sp;

	private GSprite otherSprite;

	private int CurrentTrackRoleId;
}
