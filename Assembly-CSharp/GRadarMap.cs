using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class GRadarMap : UserControl
{
	public Transform PlayerTransTarget
	{
		set
		{
			this.playerTransform = value;
			if (null == value)
			{
			}
		}
	}

	public bool IsShowDaTaoShaCircle
	{
		get
		{
			return this.mIsShowDaTaoShaCircle;
		}
		set
		{
			this.mIsShowDaTaoShaCircle = value;
			NGUITools.SetActive(this.DaTaoShaGuideLine.gameObject, false);
			NGUITools.SetActive(this.Red.gameObject, value);
			NGUITools.SetActive(this.White.gameObject, value);
			NGUITools.SetActive(this.Green.gameObject, value);
		}
	}

	private void InitDatTaoShaWarningPart()
	{
		if (Global.IsInDaTaoSha())
		{
			this.Green.material.renderQueue = 3001;
			this.White.material.renderQueue = 3002;
			this.Red.material.renderQueue = 3003;
			this.IsDisplayDaTaoShaWarning = true;
			DaTaoShaDataManager.RadiusAndPointCallBack = delegate(EscapeBattleSideScore s)
			{
				EscapeBattleAreaInfo targetSafeArea = s.targetSafeArea;
				EscapeBattleAreaInfo safeArea = s.safeArea;
				this.SetGuideLinePosition((float)targetSafeArea.PosX, (float)targetSafeArea.PosY);
				this.WhitePosition((float)safeArea.PosX, (float)safeArea.PosY);
				this.WhiteSacle = this.GetUICircleSacle((float)safeArea.PosX, (float)safeArea.PosY, IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeMapSafeAreaRadiusById(safeArea.AreaID));
				this.GreenPosition((float)targetSafeArea.PosX, (float)targetSafeArea.PosY);
				this.DaTaoShaTargerSafeAreaRadius = IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeMapSafeAreaRadiusById(targetSafeArea.AreaID);
				this.GreenSacle = this.GetUICircleSacle((float)targetSafeArea.PosX, (float)targetSafeArea.PosY, this.DaTaoShaTargerSafeAreaRadius);
				this.DaTaoShaTargerSafeAreaPosition = new Vector2(this.WorldToLocal_X((float)targetSafeArea.PosX), this.WorldToLocal_Y((float)targetSafeArea.PosY));
				this.RedPosition(0f, 0f);
				this.RedSacle = 300f;
			};
		}
		else
		{
			this.IsDisplayDaTaoShaWarning = false;
		}
	}

	public bool IsDisplayDaTaoShaWarning
	{
		set
		{
			NGUITools.SetActive(this.DaTaoShaGuideLine.gameObject, false);
			this.Green.transform.gameObject.SetActive(value);
			this.White.transform.gameObject.SetActive(value);
			this.Red.transform.gameObject.SetActive(value);
		}
	}

	public void RedPosition(float x, float y)
	{
		this.Red.transform.localPosition = new Vector3(0f, 0f, this.Red.transform.localPosition.z);
	}

	public float RedSacle
	{
		set
		{
			this.Red.transform.localScale = new Vector3(value, value, 1f);
		}
	}

	public void WhitePosition(float x, float y)
	{
		this.White.transform.localPosition = new Vector3(this.WorldToLocal_X(x), this.WorldToLocal_Y(y), this.White.transform.localPosition.z);
	}

	public float WhiteSacle
	{
		set
		{
			this.White.transform.localScale = new Vector3(value, value, 1f);
		}
	}

	public void GreenPosition(float x, float y)
	{
		this.Green.transform.localPosition = new Vector3(this.WorldToLocal_X(x), this.WorldToLocal_Y(y), this.Green.transform.localPosition.z);
	}

	public float GreenSacle
	{
		set
		{
			this.safeRadiusArea = value;
			this.Green.transform.localScale = new Vector3(value, value, 1f);
		}
	}

	private float WorldToLocal_X(float x)
	{
		return x / 100f * this.ScalingX - this.MiniMap.transform.localScale.x / 2f;
	}

	private float WorldToLocal_Y(float y)
	{
		return y / 100f * this.ScalingY - this.MiniMap.transform.localScale.y / 2f;
	}

	public float SetTargetX
	{
		set
		{
			this.TargetX = value;
		}
	}

	public float SetTargetY
	{
		set
		{
			this.TargetY = value;
		}
	}

	public float SetOriginX
	{
		set
		{
			this.OriginX = value;
		}
	}

	public float SetOriginY
	{
		set
		{
			this.OriginY = value;
		}
	}

	private void InitDaTaoSha()
	{
		this.InitDatTaoShaWarningPart();
	}

	public float GetUICircleSacle(float x, float y, float radius)
	{
		Vector2 vector;
		vector..ctor(this.WorldToLocal_X(x), this.WorldToLocal_Y(y));
		Vector2 vector2;
		vector2..ctor(this.WorldToLocal_X(x + radius), this.WorldToLocal_Y(y));
		return Vector2.Distance(vector, vector2) * 2f;
	}

	public bool IsInDaTaoShaTargetSafeArea(float playerPosX, float playerPosY)
	{
		float num = Vector2.Distance(this.DaTaoShaTargerSafeAreaPosition, new Vector2(playerPosX, playerPosY));
		return num <= this.safeRadiusArea / 2f;
	}

	public void SetGuideLinePosition(float x, float y)
	{
		x = this.WorldToLocal_X(x);
		y = this.WorldToLocal_X(y);
		this.DaTaoShaGuideLine.transform.localPosition = new Vector3(0f, 0f, this.DaTaoShaGuideLine.transform.localPosition.z);
		this.SetTargetX = x;
		this.SetTargetY = y;
	}

	private float GetDegreeByAtan2(float x, float y)
	{
		return 57.29578f * Mathf.Atan2(y, x);
	}

	private float GetDistanceOfTwoPoint(float x1, float y1, float x2, float y2)
	{
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	private void InitDaTaoShaTexture()
	{
		ShowNetImage component = this.Red.GetComponent<ShowNetImage>();
		if (component != null)
		{
			component.URL = "NetImages/GameRes/Images/DaTaoSha/hongsequyu.png.qj";
		}
		ShowNetImage component2 = this.White.GetComponent<ShowNetImage>();
		if (component2 != null)
		{
			component2.URL = "NetImages/GameRes/Images/DaTaoSha/baisequyu.png.qj";
		}
		ShowNetImage component3 = this.Green.GetComponent<ShowNetImage>();
		if (component3 != null)
		{
			component3.URL = "NetImages/GameRes/Images/DaTaoSha/lvsequyu.png.qj";
		}
	}

	protected override void InitializeComponent()
	{
		this.IsDisplayDaTaoShaWarning = false;
		DaTaoShaDataManager.EGRadarMapBattleStatusCallBack = delegate(EscapeBattleGameSceneStatuses s)
		{
			if (s >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
			{
				this.InitDaTaoSha();
				DaTaoShaDataManager.EGRadarMapBattleStatusCallBack = null;
			}
		};
		if (Context.IsHaiwai)
		{
			NGUITools.SetActive(this.btnRecordVedio.gameObject, false);
			if (PlatSDKMgr.PlatName == "YNGoogle")
			{
				NGUITools.SetActive(this.btnGoogle.gameObject, true);
				this.btnGoogle.gameObject.transform.localPosition = this.btnRecordVedio.gameObject.transform.localPosition;
			}
			else
			{
				NGUITools.SetActive(this.btnGoogle.gameObject, false);
			}
		}
		else
		{
			NGUITools.SetActive(this.btnGoogle.gameObject, false);
		}
		this.MapName.GetComponent<UILabel>().lineWidth = 120;
		this.m_sprite18.gameObject.SetActive(true);
		this.SwitchIconBak = this.SwitchIcon.tweenTarget.GetComponent<UISprite>();
		this.SwitchIconBak.spriteName = this.SwitchIconBakNames[1];
		UIEventListener.Get(this.SwitchIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.BodyVisible = !this.BodyVisible;
			if (this.BodyVisible)
			{
				this.SwitchIconBak.spriteName = this.SwitchIconBakNames[1];
			}
			else
			{
				this.SwitchIconBak.spriteName = this.SwitchIconBakNames[0];
			}
		};
		UIEventListener.Get(this.EmailIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		UIEventListener.Get(this.GaujiIcon.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1
			});
			SystemHelpMgr.OnAction(UIObjIDs.GuaJi, HelpStateEvents.Clicked, -1);
		};
		this.ShowMiniMap.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (DaTaoShaDataManager.IsGuanZhan)
			{
				Super.HintMainText(Global.GetLang("观战模式下不能使用该功能"), 10, 3);
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 2
			});
			SystemHelpMgr.OnAction(UIObjIDs.GameRadarMap, HelpStateEvents.Clicked, -1);
			this.ShowHelpAnim(0, 0);
		};
		UIEventListener.Get(this.BugReportIcon.gameObject).onClick = delegate(GameObject s)
		{
			QMQJJava.Feedback();
		};
		UISprite sprite = this.btnShowSwitch.transform.FindChild("Animation/Background").GetComponent<UISprite>();
		sprite.spriteName = ((!Global.Data.SysSetting.HideOtherRoles && !Global.Data.SysSetting.HideChiBang) ? "xian" : "ping");
		UIEventListener.Get(this.btnShowSwitch.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.IsInDaTaoSha() && DaTaoShaDataManager.IsGuanZhan)
			{
				return;
			}
			this.HideOthers(sprite);
		};
		UIEventListener.Get(this.btnShowException.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 3
			});
		};
		if (this.btnInteractionIcon != null)
		{
			UIEventListener.Get(this.btnInteractionIcon.gameObject).onClick = delegate(GameObject s)
			{
				if (Global.IsCompetitionGuanKan || (Global.IsInDaTaoSha() && DaTaoShaDataManager.IsGuanZhan))
				{
					Super.HintMainText(Global.GetLang("观战模式无法该功能使用"), 10, 3);
					return;
				}
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 6
				});
			};
		}
		UIEventListener.Get(this.btnShare2.gameObject).onClick = delegate(GameObject s)
		{
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"海外指令状态：",
					Context.IsHaiwai,
					"---",
					Context.IsShowGmMenuInMobile
				})
			});
			if (PlayZone.GlobalPlayZone != null && Context.IsHaiwai && Context.IsShowGmMenuInMobile)
			{
				MUDebug.Log<string>(new string[]
				{
					"YN/DNY GM开启"
				});
				PlayZone.GlobalPlayZone.OnF2();
			}
		};
		UIEventListener.Get(this.btnShare.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 4
			});
		};
		this.btnShare.gameObject.SetActive(false);
		if (this.btnGoogle && this.btnGoogle.gameObject.activeInHierarchy)
		{
			UIEventListener.Get(this.btnGoogle.gameObject).onClick = delegate(GameObject s)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 7
				});
			};
		}
		if (this.btnMomo != null)
		{
			this.btnMomo.gameObject.SetActive(false);
			UIEventListener.Get(this.btnMomo.gameObject).onClick = delegate(GameObject s)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 5
				});
			};
			if (VideoSystem.GetInstance().IsActive())
			{
				this.btnMomo.gameObject.SetActive(true);
			}
			else
			{
				this.btnMomo.gameObject.SetActive(false);
			}
			this.VideoIconShow(true);
		}
		if (this.btnRecordVedio != null)
		{
			if (RecordVideoSystem.GetInstance().IsActive() || VideoSystem.GetInstance().IsActive())
			{
				this.btnRecordVedio.gameObject.SetActive(true);
			}
			else
			{
				this.btnRecordVedio.gameObject.SetActive(false);
			}
			UIEventListener.Get(this.btnRecordVedio.gameObject).onClick = delegate(GameObject s)
			{
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					Super.HintMainText(Global.GetLang("当前地图无法使用此功能"), 10, 3);
					return;
				}
				PlayZone.GlobalPlayZone.OpenLiveWindow();
			};
		}
	}

	public void DaTaoShaGuanZhanJinZhiHideOthers()
	{
		if (Global.Data.SysSetting.HideOtherRoles || Global.Data.SysSetting.HideChiBang)
		{
			UISprite component = this.btnShowSwitch.transform.FindChild("Animation/Background").GetComponent<UISprite>();
			component.spriteName = ((!Global.Data.SysSetting.HideOtherRoles && !Global.Data.SysSetting.HideChiBang) ? "xian" : "ping");
			this.HideOthers(component);
		}
	}

	private void HideOthers(UISprite sprite)
	{
		sprite = this.btnShowSwitch.transform.FindChild("Animation/Background").GetComponent<UISprite>();
		if (Global.Data.SysSetting.HideOtherRolesStatus && Global.Data.SysSetting.HideOtherRoles)
		{
			Global.Data.SysSetting.HideOtherRoles = false;
			sprite.spriteName = "xian";
			GameInstance.Game.SetEffectHideFlagsCmd(new int[]
			{
				(!Global.Data.SysSetting.HideOtherRoles) ? 0 : 1
			});
		}
		else if (Global.Data.SysSetting.HideOtherRolesChiBangStatus && Global.Data.SysSetting.HideChiBang)
		{
			Global.Data.SysSetting.HideChiBang = false;
			sprite.spriteName = "xian";
		}
		else
		{
			if (Global.Data.SysSetting.HideOtherRolesStatus)
			{
				Global.Data.SysSetting.HideOtherRoles = true;
			}
			if (Global.Data.SysSetting.HideOtherRolesChiBangStatus)
			{
				Global.Data.SysSetting.HideChiBang = true;
			}
			sprite.spriteName = "ping";
		}
		Global.SaveSystemSettings();
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (!this.BodyVisible)
		{
			this.BodyVisible = true;
		}
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.ShowMiniMap, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.GaujiIcon, default(Vector4));
			}
		}
	}

	public bool BodyVisible
	{
		get
		{
			return this.Body.Visibility;
		}
		set
		{
			this.Body.Visibility = value;
		}
	}

	public bool ExceptionVisible
	{
		get
		{
			return this.btnShowException.gameObject.activeSelf;
		}
		set
		{
			this.btnShowException.gameObject.SetActive(value);
		}
	}

	public bool IsPlayAnimBugReportIcon
	{
		set
		{
			if (null != this.AnimBugReportIcon)
			{
				if (value)
				{
					this.AnimBugReportIcon.Play();
				}
				else
				{
					this.AnimBugReportIcon.Stop();
					this.BugReportIcon.transform.localScale = new Vector3(1f, 1f, 1f);
				}
			}
		}
	}

	public string VSName
	{
		get
		{
			return this.MapName.Text;
		}
		set
		{
			this.MapName.Text = value;
		}
	}

	public string VMapName { get; set; }

	public string MiniMapURL
	{
		set
		{
		}
	}

	public void RefreshHideIcon()
	{
		UISprite component = this.btnShowSwitch.transform.FindChild("Animation/Background").GetComponent<UISprite>();
		component.spriteName = ((!Global.Data.SysSetting.HideOtherRoles && !Global.Data.SysSetting.HideChiBang) ? "xian" : "ping");
	}

	public GRadarMapPoint AddRolePoint(int roleID)
	{
		if (this.rolePointList.ContainsKey(roleID))
		{
			return null;
		}
		if (this.rolePointList.Count > 10)
		{
			return null;
		}
		GRadarMapPoint gradarMapPoint;
		if (this.rolePointPool.Count > 0)
		{
			gradarMapPoint = this.rolePointPool[this.rolePointPool.Count - 1];
			this.rolePointPool.RemoveAt(this.rolePointPool.Count - 1);
			gradarMapPoint.gameObject.SetActive(true);
		}
		else
		{
			gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
			gradarMapPoint.title.gameObject.SetActive(false);
			Transform transform = gradarMapPoint.transform;
			transform.parent = this.MiniMap.transform.parent;
			transform.localScale = new Vector3(1f, 1f, 1f);
			transform.rotation = Quaternion.identity;
			UIPanel component = gradarMapPoint.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
		}
		gradarMapPoint.gameObject.name = roleID.ToString();
		this.rolePointList.Add(roleID, gradarMapPoint);
		return gradarMapPoint;
	}

	public GRadarMapPoint AddRolePoint(int roleID, string site)
	{
		if (this.PointList.ContainsKey(roleID))
		{
			return null;
		}
		if (this.PointList.Count > 10)
		{
			return null;
		}
		GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
		gradarMapPoint.title.gameObject.SetActive(false);
		Transform transform = gradarMapPoint.transform;
		transform.parent = this.MiniMap.transform.parent;
		transform.localScale = new Vector3(1f, 1f, 1f);
		transform.rotation = Quaternion.identity;
		UIPanel component = gradarMapPoint.GetComponent<UIPanel>();
		if (component != null)
		{
			Object.Destroy(component);
		}
		gradarMapPoint.gameObject.name = roleID.ToString();
		if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong && !string.IsNullOrEmpty(site))
		{
			Vector3 localPosition;
			localPosition..ctor((float)site.Split(new char[]
			{
				'|'
			})[0].SafeToInt32(0) / 100f * this.ScalingX - this.MiniMap.transform.localScale.x / 2f, (float)site.Split(new char[]
			{
				'|'
			})[1].SafeToInt32(0) / 100f * this.ScalingY - this.MiniMap.transform.localScale.y / 2f, -0.5f);
			gradarMapPoint.transform.localPosition = localPosition;
			gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
			gradarMapPoint.sprite.spriteName = "zhongliyaosai";
		}
		gradarMapPoint.gameObject.SetActive(true);
		this.PointList.Add(roleID, gradarMapPoint);
		return gradarMapPoint;
	}

	public GRadarMapPoint GetRolePoint(int roleID)
	{
		GRadarMapPoint result = null;
		this.rolePointList.TryGetValue(roleID, ref result);
		return result;
	}

	public GRadarMapPoint GetPoint(int roleID)
	{
		GRadarMapPoint result = null;
		this.PointList.TryGetValue(roleID, ref result);
		return result;
	}

	public void RemoveRolePoint(int roleID)
	{
		GRadarMapPoint gradarMapPoint = null;
		if (!this.rolePointList.TryGetValue(roleID, ref gradarMapPoint))
		{
			return;
		}
		this.rolePointList.Remove(roleID);
		this.rolePointPool.Add(gradarMapPoint);
		gradarMapPoint.gameObject.SetActive(false);
	}

	public void RemoveDistanceRolePoint()
	{
		List<int> list = null;
		Dictionary<int, GRadarMapPoint>.Enumerator enumerator = this.rolePointList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Camera currentCamera = UICamera.currentCamera;
			KeyValuePair<int, GRadarMapPoint> keyValuePair = enumerator.Current;
			Vector3 vector = currentCamera.WorldToScreenPoint(keyValuePair.Value.transform.position);
			if (Vector3.Distance(vector, this.PlayerArrowScreenPos) > 30f)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				List<int> list2 = list;
				KeyValuePair<int, GRadarMapPoint> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.RemoveRolePoint(list[i]);
		}
	}

	public void ClearAllRolePoints()
	{
		Dictionary<int, GRadarMapPoint>.Enumerator enumerator = this.rolePointList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			List<GRadarMapPoint> list = this.rolePointPool;
			KeyValuePair<int, GRadarMapPoint> keyValuePair = enumerator.Current;
			list.Add(keyValuePair.Value);
			KeyValuePair<int, GRadarMapPoint> keyValuePair2 = enumerator.Current;
			keyValuePair2.Value.gameObject.SetActive(false);
		}
		this.rolePointList.Clear();
	}

	public Canvas MiniMapCanvas { get; set; }

	public int MapOriginalWidth { get; set; }

	public int MapOriginalHeight { get; set; }

	public int Scaling { get; set; }

	public float ScalingX
	{
		get
		{
			if (this._ScalingX <= 0f)
			{
				this._ScalingX = this.MiniMap.transform.localScale.x / ((float)Global.Data.GameScene.CurrentMapData.MapWidth / 100f);
			}
			return this._ScalingX;
		}
		set
		{
			this._ScalingX = value;
		}
	}

	public float ScalingY
	{
		get
		{
			if (this._scalingY <= 0f)
			{
				this._scalingY = this.MiniMap.transform.localScale.y / ((float)Global.Data.GameScene.CurrentMapData.MapHeight / 100f);
			}
			return this._scalingY;
		}
		set
		{
			this._scalingY = value;
		}
	}

	public double Left { get; set; }

	public double Top { get; set; }

	public double ZIndex { get; set; }

	public Vector3 PlayerArrowScreenPos
	{
		get
		{
			if (this._playerArrowScreenPos == Vector3.zero)
			{
				this._playerArrowScreenPos = UICamera.currentCamera.WorldToScreenPoint(Global.Data.GameRadarMap.playerArrow.transform.position);
			}
			return this._playerArrowScreenPos;
		}
	}

	public void InitTeleport()
	{
		for (int i = 0; i < this.teleportPointList.Count; i++)
		{
			Object.Destroy(this.teleportPointList[i].gameObject);
		}
		this.teleportPointList.Clear();
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, "teleports.xml");
		if (gameMapSettingsXml == null)
		{
			GError.AddErrMsg(string.Format(Global.GetLang("加载地图传送点文件失败: {0}"), base.name));
			return;
		}
		List<XElement> xelementList = XmlManager.GetXElementList(gameMapSettingsXml, "Teleport");
		if (xelementList == null)
		{
			return;
		}
		for (int j = 0; j < xelementList.Count; j++)
		{
			GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[j], "Tip");
			gradarMapPoint.name = "Obj" + xelementAttributeStr;
			gradarMapPoint.Title = xelementAttributeStr;
			gradarMapPoint.sprite.transform.localScale = new Vector3(18f, 18f, 18f);
			Transform transform = gradarMapPoint.transform;
			transform.parent = this.MiniMap.transform.parent;
			transform.localPosition = Vector3.zero;
			transform.rotation = Quaternion.identity;
			transform.localScale = new Vector3(1f, 1f, 1f);
			UIPanel component = transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			Vector3 localPosition;
			localPosition..ctor((float)Global.GetXElementAttributeInt(xelementList[j], "X") / 100f * this.ScalingX - this.MiniMap.transform.localScale.x / 2f, (float)Global.GetXElementAttributeInt(xelementList[j], "Y") / 100f * this.ScalingY - this.MiniMap.transform.localScale.y / 2f, -0.01f);
			gradarMapPoint.transform.localPosition = localPosition;
			gradarMapPoint.gameObject.SetActive(false);
			this.teleportPointList.Add(gradarMapPoint);
			gradarMapPoint.Img = "radarTeleport";
		}
	}

	public override void Update()
	{
		base.Update();
		Vector3 zero = Vector3.zero;
		if (this.playerTransform == null)
		{
			GSprite gsprite = ObjectsManager.FindSprite(Global.Data.RoleID);
			if (gsprite != null)
			{
				if (gsprite.OnHorseEX)
				{
					if (null != gsprite.HorseController.HorseTrans)
					{
						if (gsprite.HorseController != null)
						{
							this.playerTransform = gsprite.HorseController.HorseTrans;
						}
					}
					else if (gsprite.The3DGameObject != null)
					{
						this.playerTransform = gsprite.The3DGameObject.transform;
					}
				}
				else if (gsprite.The3DGameObject != null)
				{
					this.playerTransform = gsprite.The3DGameObject.transform;
				}
			}
			else
			{
				GameObject gameObject = GameObject.Find("Leader");
				if (null != gameObject)
				{
					this.playerTransform = gameObject.transform;
				}
				else
				{
					this.playerTransform = null;
				}
			}
		}
		if (this.playerTransform != null)
		{
			zero.x = this.MiniMap.transform.localScale.x / 2f - this.playerTransform.localPosition.x * this.ScalingX;
			zero.y = this.MiniMap.transform.localScale.y / 2f - this.playerTransform.localPosition.z * this.ScalingY;
			this.MiniMap.transform.parent.localPosition = zero;
			this.playerArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f - this.playerTransform.rotation.eulerAngles.y);
		}
		if (this.lblUserPos != null && null != this.playerTransform)
		{
			Vector3 localPosition = this.playerTransform.localPosition;
			if (this.lastPosition != localPosition)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(((int)localPosition.x).ToString("d2")).Append(":").Append(((int)localPosition.z).ToString("d2"));
				this.lblUserPos.text = stringBuilder.ToString();
				this.lastPosition = localPosition;
			}
		}
		if (Input.GetKeyDown(8))
		{
			VideoSystem.GetInstance().QJ_ListenerBackSpace();
		}
		if (Input.anyKey)
		{
			string inputString = Input.inputString;
			if (inputString != string.Empty)
			{
				VideoSystem.GetInstance().QJ_ListenerInput(inputString);
			}
		}
	}

	public void ShareImg(int type)
	{
		base.StartCoroutine<bool>(this.TakeShot(type));
	}

	private IEnumerator TakeShot(int type)
	{
		yield return new WaitForEndOfFrame();
		string picName = "MU_" + DateTime.Now.ToString("yyMMddHHmmss") + ".png";
		string fileName = Application.persistentDataPath + "/" + picName;
		if (Application.platform == 8)
		{
			fileName = fileName.Replace("Documents", "tmp");
		}
		this.MyScreenShot(fileName);
		FileInfo fileInfo = new FileInfo(fileName);
		for (;;)
		{
			if (!File.Exists(fileName))
			{
				yield return new WaitForSeconds(0.2f);
			}
			else
			{
				if (fileInfo.Length > 0L)
				{
					break;
				}
				yield return new WaitForSeconds(0.2f);
			}
		}
		PlatSDKMgr.WXShareImage(fileName);
		yield return new WaitForEndOfFrame();
		yield break;
	}

	private void MyScreenShot(string fileName)
	{
		Texture2D texture2D = new Texture2D(Screen.width, Screen.height, 3, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), 0, 0);
		texture2D.Apply();
		byte[] array = texture2D.EncodeToPNG();
		Object.Destroy(texture2D);
		FileStream fileStream = new FileStream(fileName, 2);
		fileStream.Write(array, 0, array.Length);
		fileStream.Flush();
		fileStream.Close();
	}

	public void VideoIconShow(bool p)
	{
		if (this.btnMomo == null)
		{
			return;
		}
		if (p)
		{
			UISprite componentInChildren = this.btnMomo.GetComponentInChildren<UISprite>();
			if (componentInChildren != null)
			{
				componentInChildren.spriteName = "videoOn";
			}
		}
		else
		{
			UISprite componentInChildren2 = this.btnMomo.GetComponentInChildren<UISprite>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.spriteName = "videoPause";
			}
		}
	}

	public void RecordVideoIconIng(bool p)
	{
		if (this.btnRecordVedio == null)
		{
			return;
		}
		if (p)
		{
			UISprite componentInChildren = this.btnRecordVedio.GetComponentInChildren<UISprite>();
			if (componentInChildren != null)
			{
				componentInChildren.spriteName = "lushipinIng";
			}
		}
		else
		{
			UISprite componentInChildren2 = this.btnRecordVedio.GetComponentInChildren<UISprite>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.spriteName = "lushipinStart";
			}
		}
	}

	public void ResetReplayKitBtn(bool isRecording)
	{
		if (this.btnRecordVedio == null)
		{
			return;
		}
		UISprite componentInChildren = this.btnRecordVedio.GetComponentInChildren<UISprite>();
		if (isRecording)
		{
			if (componentInChildren != null)
			{
				Super.HideNetWaiting();
				componentInChildren.spriteName = "lushipinIng";
			}
		}
		else if (componentInChildren != null)
		{
			Super.HideNetWaiting();
			componentInChildren.spriteName = "lushipinStart";
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton SwitchIcon;

	public UIButton EmailIcon;

	public UIButton GaujiIcon;

	public UIButton BugReportIcon;

	public UIButton btnShowSwitch;

	public UIButton btnShowException;

	public UIButton btnShare;

	public UIButton btnShare2;

	public UIButton btnGoogle;

	public UIButton btnMomo;

	public UIButton btnInteractionIcon;

	public UIButton btnRecordVedio;

	public Animation AnimBugReportIcon;

	public SpriteSL Body;

	public TextBlock MapName;

	public ShowNetImage MiniMap;

	public GButton ShowMiniMap;

	public GRadarMapPoint playerArrow;

	public UILabel lblUserPos;

	public GameObject m_sprite18;

	private Transform playerTransform;

	private Vector3 lastPosition = Vector3.zero;

	private List<GRadarMapPoint> teleportPointList = new List<GRadarMapPoint>();

	private Dictionary<int, GRadarMapPoint> rolePointList = new Dictionary<int, GRadarMapPoint>();

	private List<GRadarMapPoint> rolePointPool = new List<GRadarMapPoint>(10);

	private Dictionary<int, GRadarMapPoint> PointList = new Dictionary<int, GRadarMapPoint>();

	private UISprite SwitchIconBak;

	private string[] SwitchIconBakNames = new string[]
	{
		"mainHide",
		"mainShow"
	};

	public bool mIsShowDaTaoShaCircle;

	public UISprite DaTaoShaGuideLine;

	public UITexture Red;

	public UITexture White;

	public UITexture Green;

	private float safeRadiusArea;

	private float OriginX;

	private float OriginY;

	private float TargetX;

	private float TargetY;

	private float DaTaoShaTargerSafeAreaRadius;

	private Vector2 DaTaoShaTargerSafeAreaPosition = default(Vector2);

	private float _ScalingX;

	private float _scalingY;

	private Vector3 _playerArrowScreenPos = Vector3.zero;
}
