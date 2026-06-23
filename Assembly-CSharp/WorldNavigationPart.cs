using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class WorldNavigationPart : UserControl
{
	public ImageBrush NewSource { get; set; }

	public ImageBrush BodySource { get; set; }

	public ImageBrush HitBodySource { get; set; }

	public float ScalingX
	{
		get
		{
			if (this._ScalingX <= 0f)
			{
				this._ScalingX = this.localMap.transform.localScale.x / ((float)Global.Data.GameScene.CurrentMapData.MapWidth / 100f);
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
				this._scalingY = this.localMap.transform.localScale.y / ((float)Global.Data.GameScene.CurrentMapData.MapHeight / 100f);
			}
			return this._scalingY;
		}
		set
		{
			this._scalingY = value;
		}
	}

	public float ScreenScaleX
	{
		get
		{
			if (this._ScreenScaleX <= 0f)
			{
				this._ScreenScaleX = (float)Screen.width / 960f;
			}
			return this._ScreenScaleX;
		}
		set
		{
			this._ScreenScaleX = value;
		}
	}

	public float ScreenScaleY
	{
		get
		{
			if (this._ScreenScaleY <= 0f)
			{
				this._ScreenScaleY = (float)Screen.height / 540f;
			}
			return this._ScreenScaleY;
		}
		set
		{
			this._ScreenScaleY = value;
		}
	}

	private float GetDegreeByAtan2(float x, float y)
	{
		return 57.29578f * Mathf.Atan2(y, x);
	}

	private float GetDistanceOfTwoPoint(float x1, float y1, float x2, float y2)
	{
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
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
			NGUITools.SetActive(this.DaTaoShaGuideLine.gameObject, value);
			NGUITools.SetActive(this.Red.gameObject, value);
			NGUITools.SetActive(this.White.gameObject, value);
			NGUITools.SetActive(this.Green.gameObject, value);
		}
	}

	private void InitDatTaoShaWarningPart()
	{
		if (Global.IsInDaTaoSha() && DaTaoShaDataManager.EBattleStatus >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
		{
			this.Green.material.renderQueue = 3001;
			this.White.material.renderQueue = 3002;
			this.Red.material.renderQueue = 3003;
			this.IsDisplayDaTaoShaWarning = true;
			this.IsShowDaTaoShaCircle = true;
			this.SetCirclePos();
			DaTaoShaDataManager.WorldNavigationRadiusAndPointCallBack = delegate(EscapeBattleSideScore s)
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
			this.IsShowDaTaoShaCircle = false;
		}
	}

	public bool IsDisplayDaTaoShaWarning
	{
		set
		{
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
		return x / 100f * this.ScalingX - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
	}

	private float WorldToLocal_Y(float y)
	{
		return y / 100f * this.ScalingY - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
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

	public void SetGuideLinePosition(float x, float y)
	{
		x = this.WorldToLocal_X(x);
		y = this.WorldToLocal_X(y);
		this.DaTaoShaGuideLine.transform.localPosition = new Vector3(x, y, this.DaTaoShaGuideLine.transform.localPosition.z);
		this.SetOriginX = x;
		this.SetOriginY = y;
	}

	private void InitDaTaoSha()
	{
		this.InitDatTaoShaWarningPart();
	}

	public void SetCirclePos()
	{
		if (DaTaoShaDataManager.WorldNavigationRadiusAndPoint == null)
		{
			return;
		}
		EscapeBattleAreaInfo targetSafeArea = DaTaoShaDataManager.WorldNavigationRadiusAndPoint.targetSafeArea;
		EscapeBattleAreaInfo safeArea = DaTaoShaDataManager.WorldNavigationRadiusAndPoint.safeArea;
		this.SetGuideLinePosition((float)targetSafeArea.PosX, (float)targetSafeArea.PosY);
		this.WhitePosition((float)safeArea.PosX, (float)safeArea.PosY);
		this.WhiteSacle = this.GetUICircleSacle((float)safeArea.PosX, (float)safeArea.PosY, IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeMapSafeAreaRadiusById(safeArea.AreaID));
		this.GreenPosition((float)targetSafeArea.PosX, (float)targetSafeArea.PosY);
		this.DaTaoShaTargerSafeAreaRadius = IConfigbase<ConfigDaTaoSha>.Instance.GetEscapeMapSafeAreaRadiusById(targetSafeArea.AreaID);
		this.GreenSacle = this.GetUICircleSacle((float)targetSafeArea.PosX, (float)targetSafeArea.PosY, this.DaTaoShaTargerSafeAreaRadius);
		this.DaTaoShaTargerSafeAreaPosition = new Vector2(this.WorldToLocal_X((float)targetSafeArea.PosX), this.WorldToLocal_Y((float)targetSafeArea.PosY));
		this.RedPosition(0f, 0f);
		this.RedSacle = 300f;
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

	private void InitTextInPrefabs()
	{
		this.gTabCtrl.TabBtns[0].Text = Global.GetLang("世界地图");
		this.gTabCtrl.TabBtns[1].Text = Global.GetLang("跨服地图");
		this.gTabCtrl.TabBtns[2].Text = Global.GetLang("区域地图");
		this.chkMonster.Text = Global.GetLang("怪物");
		this.chkTele.Text = Global.GetLang("传送点");
		if (this.ConstTexts != null && this.ConstTexts.Length == 1)
		{
			this.ConstTexts[0].text = Global.GetLang("VIP2以上玩家点击传送");
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.thisCtrl = this;
		this.ItemCollection = this.listBox.ItemsSource;
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.goodsItemCollection = this.goodsListBox.ItemsSource;
		this.Root = this.Container;
		this.gTabCtrl.BeforeTabBtnClick = delegate(object s, MouseEvent e)
		{
			if (Global.IsCompetitionGuanKan)
			{
				Super.HintMainText(Global.GetLang("观战模式无法该功能使用"), 10, 3);
				return;
			}
			if (e.Index == this.gTabCtrl.SelectIndex)
			{
				return;
			}
			if ((Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu && e.Index == 0) || (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) && (e.Index == 0 || e.Index == 1)))
			{
				Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
				return;
			}
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				string[] array = (s as GameObject).name.Split(new char[]
				{
					'_'
				});
				int num = int.Parse(array[1]);
				if (num == 1)
				{
					Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
					return;
				}
			}
			this.gTabCtrl.SetTab(s as GameObject);
		};
		this.gTabCtrl.OnTabBtnClick = delegate(object s, DPSelectedItemEventArgs e)
		{
			SystemHelpMgr.OnAction(UIObjIDs.WorldMapTab, HelpStateEvents.Clicked, e.Index);
		};
		this.m_localMap.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			Point point = new Point(e.ID, e.IDType);
			if (this.PartMapClick != null)
			{
				this.PartMapClick.Invoke(this.thisCtrl, new BaseEventArgs
				{
					IDType = -1,
					ID = 0,
					X = point.X,
					Y = point.Y
				});
			}
		};
		this.IsDisplayDaTaoShaWarning = false;
		this.IsShowDaTaoShaCircle = false;
		if (DaTaoShaDataManager.EBattleStatus >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
		{
			DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack = null;
			this.InitDaTaoSha();
		}
		else
		{
			DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack = delegate(EscapeBattleGameSceneStatuses s)
			{
				if (s >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
				{
					this.InitDaTaoSha();
					DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack = null;
				}
			};
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.gTabCtrl.TabBtns[id], default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.btnYongzhedalu, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private void listBox_SelectionChanged(object sender, EventArgs e)
	{
		if (Global.IsCompetitionGuanKan)
		{
			return;
		}
		PartMapListItem partMapListItem;
		if (null != this.listBox.LastSelectedItem)
		{
			partMapListItem = U3DUtils.AS<PartMapListItem>(this.listBox.LastSelectedItem);
			if (null != partMapListItem)
			{
				partMapListItem.SelectedState = false;
			}
		}
		partMapListItem = U3DUtils.AS<PartMapListItem>(this.listBox.SelectedItem);
		if (null != partMapListItem)
		{
			partMapListItem.SelectedState = true;
			this.GotoBtnClicked(partMapListItem.gameObject, null);
		}
	}

	public void InitPartSize(int width, int height)
	{
	}

	private void InitPartialWorld()
	{
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				DaTaoShaDataManager.EWorldNavigationBattleStatusCallBack = null;
				DaTaoShaDataManager.WorldNavigationRadiusAndPointCallBack = null;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = -10
				});
			}
		};
		this.chkMonster.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (Global.IsCompetitionGuanKan)
			{
				Super.HintMainText(Global.GetLang("观战模式无法该功能使用"), 10, 3);
				return;
			}
			if (this.chkMonster.Check)
			{
				this.scrollBar.scrollValue = 0f;
				this.ShowMonsterList();
			}
		};
		this.chkNPC.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (Global.IsCompetitionGuanKan)
			{
				Super.HintMainText(Global.GetLang("观战模式无法该功能使用"), 10, 3);
				return;
			}
			if (this.chkNPC.Check)
			{
				this.scrollBar.scrollValue = 0f;
				this.ShowNPCList();
			}
		};
		this.chkTele.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			if (Global.IsCompetitionGuanKan)
			{
				Super.HintMainText(Global.GetLang("观战模式无法该功能使用"), 10, 3);
				return;
			}
			if (this.chkTele.Check)
			{
				this.scrollBar.scrollValue = 0f;
				this.ShowTeleportList();
			}
		};
		UIEventListener.Get(this.btnYongzhedalu.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_YONGZHEDALU);
		};
		this.SetBtnEnable(this.btnYongzhedalu, this.MAPCODE_YONGZHEDALU);
		UIEventListener.Get(this.btnTiankong.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_TIANKONG);
		};
		this.SetBtnEnable(this.btnTiankong, this.MAPCODE_TIANKONG);
		UIEventListener.Get(this.btnShiluozhita.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_SHILUOZHITA);
		};
		this.SetBtnEnable(this.btnShiluozhita, this.MAPCODE_SHILUOZHITA);
		UIEventListener.Get(this.btnBingfenggu.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_BINGFENGGU);
		};
		this.SetBtnEnable(this.btnBingfenggu, this.MAPCODE_BINGFENGGU);
		UIEventListener.Get(this.btnSiwangshamo.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_SIWANGSHAMO);
		};
		this.SetBtnEnable(this.btnSiwangshamo, this.MAPCODE_SIWANGSHAMO);
		UIEventListener.Get(this.btnYatelandisi.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_YATELANDISI);
		};
		this.SetBtnEnable(this.btnYatelandisi, this.MAPCODE_YATELANDISI);
		UIEventListener.Get(this.btnXianzonglin.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_XIANZONGLIN);
		};
		this.SetBtnEnable(this.btnXianzonglin, this.MAPCODE_XIANZONGLIN);
		UIEventListener.Get(this.btnDixiacheng.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_DIXIACHENG);
		};
		this.SetBtnEnable(this.btnDixiacheng, this.MAPCODE_DIXIACHENG);
		UIEventListener.Get(this.btnYouansenlin.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_YOUANSENLIN);
		};
		this.SetBtnEnable(this.btnYouansenlin, this.MAPCODE_YOUANSENLIN);
		UIEventListener.Get(this.btnCantelufeixu.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_CANTELUFEIXU);
		};
		this.SetBtnEnable(this.btnCantelufeixu, this.MAPCODE_CANTELUFEIXU);
		UIEventListener.Get(this.btnCanteluyizhi.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_CANTELUYIZHI);
		};
		this.SetBtnEnable(this.btnCanteluyizhi, this.MAPCODE_CANTELUYIZHI);
		UIEventListener.Get(this.btnBingshuangzhicheng.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_BINGSHUANGZHICHENG);
		};
		this.SetBtnEnable(this.btnBingshuangzhicheng, this.MAPCODE_BINGSHUANGZHICHENG);
		UIEventListener.Get(this.btnAnNingChi.gameObject).onClick = delegate(GameObject s)
		{
			this.TransToMap(this.MAPCODE_ANNINGCHI);
		};
		this.SetBtnEnable(this.btnAnNingChi, this.MAPCODE_ANNINGCHI);
		UIEventListener.Get(this.btnKuafuzhuxianMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData.MapCode != this.MAPCODE_KUAFUZHUXIANDITU)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_KUAFUZHUXIANDITU, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetBtnEnable(this.btnKuafuzhuxianMap, this.MAPCODE_KUAFUZHUXIANDITU);
		UIEventListener.Get(this.btnHuanShuYuanMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData.MapCode != this.MAPCODE_HUANSHUYUAN)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_HUANSHUYUAN, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetBtnEnable(this.btnHuanShuYuanMap, this.MAPCODE_HUANSHUYUAN);
		UIEventListener.Get(this.btnKaLunXiMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData.MapCode != this.MAPCODE_KALUNXI)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_KALUNXI, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetBtnEnable(this.btnKaLunXiMap, this.MAPCODE_KALUNXI);
		UIEventListener.Get(this.btnKaLunDongMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData.MapCode != this.MAPCODE_KALUNDONG)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_KALUNDONG, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetBtnEnable(this.btnKaLunDongMap, this.MAPCODE_KALUNDONG);
		UIEventListener.Get(this.btnXieESiMiaoMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData.MapCode != this.MAPCODE_XIEESIMIAO)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_XIEESIMIAO, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetBtnEnable(this.btnXieESiMiaoMap, this.MAPCODE_XIEESIMIAO);
		UIEventListener.Get(this.btnMeiYinMiYaoMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.Data.roleData.MapCode != this.MAPCODE_MEIYINMIYAO)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_MEIYINMIYAO, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetBtnEnable(this.btnMeiYinMiYaoMap, this.MAPCODE_MEIYINMIYAO);
		UIEventListener.Get(this.btnShiLiMap.gameObject).onClick = delegate(GameObject s)
		{
			if (GameInstance.Game.CurrentSession.roleData.CompType > 0)
			{
				ShiLiData.EnterSelfShiLiMap();
			}
			else
			{
				ShiLiData.OnShiLiSelectWindow();
			}
		};
		this.SetShiLiBtnEnable(this.btnShiLiMap, this.MAPCODE_SHILI);
		UIEventListener.Get(this.btnRebirthMap.gameObject).onClick = delegate(GameObject s)
		{
			if (Global.IsKuaFuMap(Global.Data.roleData.MapCode, true) && !SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("请返回原服后前往"), 10, 3);
			}
			else if (Global.Data.roleData.MapCode != this.MAPCODE_REBIRTH)
			{
				PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_REBIRTH, -1, -1, true, 0, 0, false, false);
			}
			else
			{
				Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			}
		};
		this.SetRebirthBtnEnable(this.btnRebirthMap, this.MAPCODE_REBIRTH);
		if ((double)Global.VersionCode > 7.0)
		{
			UIEventListener.Get(this.btnMiZhong.gameObject).onClick = delegate(GameObject s)
			{
				if (Global.Data.roleData.MapCode != this.MAPCODE_MIZHONG)
				{
					PlayZone.GlobalPlayZone.OpenKuafuMapView(0, 0, 0, this.MAPCODE_MIZHONG, -1, -1, true, 0, 0, false, false);
				}
				else
				{
					Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
				}
			};
			this.SetMiZongBtnEnable(this.btnMiZhong, this.MAPCODE_MIZHONG);
		}
		else
		{
			this.btnMiZhong.gameObject.SetActive(false);
		}
	}

	private void SetMiZongBtnEnable(UIButton btn, int MAPCODE_MIZHONG)
	{
		bool flag = false;
		if (Global.Data != null && Global.Data.roleData != null && 15 <= Global.Data.roleData.ChangeLifeCount)
		{
			flag = true;
		}
		btn.isEnabled = flag;
		btn.tweenTarget.GetComponent<UISprite>().spriteName = string.Empty + MAPCODE_MIZHONG + ((!flag) ? "_1" : "_0");
	}

	private void SetRebirthBtnEnable(UIButton btn, int MAPCODE_REBIRTH)
	{
		bool flag = false;
		if (Global.Data != null && Global.Data.roleData != null && 0 < Global.Data.roleData.RebornLevel)
		{
			flag = true;
		}
		btn.isEnabled = flag;
		btn.tweenTarget.GetComponent<UISprite>().spriteName = string.Empty + MAPCODE_REBIRTH + ((!flag) ? "_1" : "_0");
	}

	private void SetBtnEnable(GButton btn)
	{
		int num = 0;
		int num2 = 0;
		int mapCode = btn.BtnTag.SafeToInt32(0);
		Global.GetMapMinLevelAndZhuanSheng(mapCode, out num2, out num);
		if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level < num * 100 + num2)
		{
			btn.isEnabled = false;
		}
		else
		{
			btn.isEnabled = true;
		}
	}

	private void SetBtnEnable(UIButton btn, int mapCode)
	{
		int num = 0;
		int num2 = 0;
		Global.GetMapMinLevelAndZhuanSheng(mapCode, out num2, out num);
		if (Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level < num * 100 + num2)
		{
			btn.isEnabled = false;
			btn.tweenTarget.GetComponent<UISprite>().spriteName = string.Empty + mapCode + "_1";
		}
		else
		{
			btn.isEnabled = true;
			btn.tweenTarget.GetComponent<UISprite>().spriteName = string.Empty + mapCode + "_0";
		}
	}

	private void SetShiLiBtnEnable(UIButton btn, int mapCode)
	{
		if (!ShiLiData.IsShiLiOpen())
		{
			btn.isEnabled = false;
			btn.tweenTarget.GetComponent<UISprite>().spriteName = string.Empty + mapCode + "_1";
		}
		else
		{
			btn.isEnabled = true;
			btn.tweenTarget.GetComponent<UISprite>().spriteName = string.Empty + mapCode + "_0";
		}
	}

	private void TransToMap(int mapCode)
	{
		if (PlayZone.OnPreChangeMap(mapCode, 0))
		{
			return;
		}
		if (Global.Data.roleData.MapCode == mapCode)
		{
			Super.HintMainText(Global.GetLang("相同地图，不需要传送！"), 10, 3);
			return;
		}
		SystemHelpMgr.OnAction(UIObjIDs.WorldMapTrans, HelpStateEvents.Clicked, mapCode);
		GameInstance.Game.SpriteGotToMap(mapCode);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = -10
			});
		}
	}

	public void RefreshPartialWorld()
	{
		this.MonstersDict.Clear();
		this.AddMiniMap();
		this.ClearRadarMapPoint();
		this.ClearRaderMapLinePoint();
		this.ScalingX = 0f;
		this.ScalingY = 0f;
		this.ShowTeleportOnMap();
		this.ShowMonstersOnMap();
		this.ShowNPCSOnMap();
		this.ShowArmyShuiJing();
		this.chkNPC.Check = true;
		this.ShowNPCList();
		this.ShowMoYuBuffState();
		Global.Data.GameScene.UpdateAutoFindWayOnPartMap();
		this.goodsItemCollection.Clear();
		this.InitGoodsData();
		this.ReMapPointImage();
	}

	public void AddMiniMap()
	{
		this.lblMapName.text = ConfigSettings.GetMapNameByCode(Global.Data.roleData.MapCode, false);
		this.localMap.URL = StringUtil.substitute("NetImages/MiniMap/{0}.png", new object[]
		{
			ConfigSettings.GetMapPicCodeByCode(Global.Data.roleData.MapCode)
		});
	}

	private void ShowNPCList()
	{
		this.ShowListType = 0;
		this.ItemCollection.Clear();
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, StringUtil.substitute("npcs.xml", new object[0]));
		if (gameMapSettingsXml == null)
		{
			return;
		}
		ConfigNPCs.PreCacheNPCVOs();
		Global.AssertException(ConfigNPCs.NPCVODict.Count > 0, StringUtil.substitute(Global.GetLang("加载xml失败: {0}"), new object[]
		{
			"Config/npcs.Xml"
		}));
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "NPCs"), "*");
		string text = string.Empty;
		if (xelementList != null)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(Global.GetXElementAttributeInt(xelement, "Code"));
				if (npcvobyID != null)
				{
					PartMapListItem partMapListItem = U3DUtils.NEW<PartMapListItem>();
					this.ItemCollection.AddNoUpdate(partMapListItem);
					text = npcvobyID.Function;
					if (text != string.Empty)
					{
						partMapListItem.NameText = StringUtil.substitute(Global.GetLang("{0}·{1}"), new object[]
						{
							npcvobyID.Function,
							npcvobyID.SName
						});
					}
					else
					{
						partMapListItem.NameText = npcvobyID.SName;
					}
					partMapListItem.NpcID = npcvobyID.ID;
					partMapListItem.Tag = StringUtil.substitute("{0},{1},{2},{3},{4}", new object[]
					{
						npcvobyID.ID,
						Global.GetXElementAttributeInt(xelement, "X"),
						Global.GetXElementAttributeInt(xelement, "Y"),
						GSpriteTypes.NPC,
						npcvobyID.MapCode
					});
					UIPanel component = partMapListItem.GetComponent<UIPanel>();
					if (component != null)
					{
						Object.Destroy(component);
					}
				}
			}
			this.ItemCollection.DelayUpdate();
		}
	}

	private void ShowMonsterList()
	{
		this.ShowListType = 1;
		this.ItemCollection.Clear();
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, StringUtil.substitute("Monsters.xml", new object[0]));
		if (gameMapSettingsXml == null)
		{
			return;
		}
		ConfigMonsters.PreCacheMonsterXmlNodes();
		Global.AssertException(ConfigMonsters.MonsterXmlNode.Count > 0, StringUtil.substitute(Global.GetLang("加载xml失败: {0}"), new object[]
		{
			"Config/Monsters.Xml"
		}));
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "Monsters"), "*");
		if (xelementList != null)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Code");
				if (!dictionary.ContainsKey(xelementAttributeInt))
				{
					dictionary.Add(xelementAttributeInt, new List<int>());
				}
				dictionary[xelementAttributeInt].Add(Global.GetXElementAttributeInt(xelement, "ID"));
			}
		}
		RandomAS randomAS = new RandomAS(0);
		foreach (int num in dictionary.Keys)
		{
			List<int> list = dictionary[num];
			if (list.Count > 0)
			{
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num);
				if (monsterXmlNodeByID != null)
				{
					XElement xelement2 = Global.GetXElement(gameMapSettingsXml, "Monster", "ID", list[randomAS.Next(0, list.Count)].ToString());
					if (xelement2 != null)
					{
						int monsterType = monsterXmlNodeByID.MonsterType;
						if (monsterType != 501 && monsterType != 601)
						{
							PartMapListItem partMapListItem = U3DUtils.NEW<PartMapListItem>();
							partMapListItem.NameText = monsterXmlNodeByID.SName;
							partMapListItem.NpcID = monsterXmlNodeByID.ID;
							partMapListItem.Tag = StringUtil.substitute("{0},{1},{2},{3},{4}", new object[]
							{
								Global.GetXElementAttributeInt(xelement2, "ID"),
								Global.GetXElementAttributeInt(xelement2, "X"),
								Global.GetXElementAttributeInt(xelement2, "Y"),
								GSpriteTypes.Monster,
								monsterXmlNodeByID.MapCode
							});
							this.ItemCollection.AddNoUpdate(partMapListItem);
						}
					}
				}
			}
		}
		this.ItemCollection.DelayUpdate();
		this.ShowArmyShuiJingList();
	}

	private void ShowTeleportOnMap()
	{
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, StringUtil.substitute("teleports.xml", new object[0]));
		if (gameMapSettingsXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "Teleports"), "*");
		if (xelementList != null)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
				gradarMapPoint.Name = StringUtil.substitute("teleport{0}", new object[]
				{
					Global.GetXElementAttributeInt(xelement, "Key")
				});
				gradarMapPoint.Img = "radarTeleport";
				gradarMapPoint.Title = Global.GetXElementAttributeStr(xelement, "Tip");
				gradarMapPoint.sprite.transform.localScale = new Vector3(18f, 18f, 18f);
				float num = (float)Global.GetXElementAttributeInt(xelement, "X") * this.ScalingX / 100f;
				float num2 = (float)Global.GetXElementAttributeInt(xelement, "Y") * this.ScalingY / 100f;
				this.AddPoint(gradarMapPoint.gameObject);
				this.radarMapPointList.Add(gradarMapPoint);
				float num3 = num - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
				float num4 = num2 - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
				gradarMapPoint.transform.localPosition = new Vector3(num3, num4, -0.01f);
			}
		}
	}

	private void ShowMonstersOnMap()
	{
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, StringUtil.substitute("Monsters.xml", new object[0]));
		if (gameMapSettingsXml == null)
		{
			return;
		}
		ConfigMonsters.PreCacheMonsterXmlNodes();
		Global.AssertException(ConfigMonsters.MonsterXmlNode.Count > 0, StringUtil.substitute(Global.GetLang("加载xml失败: {0}"), new object[]
		{
			"Config/Monsters.Xml"
		}));
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "Monsters"), "*");
		if (xelementList != null)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Code");
				if (this.IsShowMonster(Global.GetXElementAttributeStr(xelement, "Code")))
				{
					if (!dictionary.ContainsKey(xelementAttributeInt))
					{
						dictionary.Add(xelementAttributeInt, new List<int>());
					}
					dictionary[xelementAttributeInt].Add(Global.GetXElementAttributeInt(xelement, "ID"));
				}
			}
		}
		RandomAS randomAS = new RandomAS(0);
		foreach (int num in dictionary.Keys)
		{
			List<int> list = dictionary[num];
			if (list.Count > 0)
			{
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num);
				if (monsterXmlNodeByID != null)
				{
					XElement xelement2 = Global.GetXElement(gameMapSettingsXml, "Monster", "ID", list[0].ToString());
					if (xelement2 != null)
					{
						int monsterType = monsterXmlNodeByID.MonsterType;
						if (monsterType != 501 && monsterType != 601)
						{
							GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
							string img = "radarMonster";
							if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
							{
								img = "zhongli";
							}
							else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
							{
								img = "zhongliyaosai";
							}
							gradarMapPoint.Img = img;
							gradarMapPoint.Title = monsterXmlNodeByID.SName;
							gradarMapPoint.name = gradarMapPoint.Title;
							gradarMapPoint.id = monsterXmlNodeByID.ID;
							float num2 = (float)Global.GetXElementAttributeInt(xelement2, "X") / 100f * this.ScalingX;
							float num3 = (float)Global.GetXElementAttributeInt(xelement2, "Y") / 100f * this.ScalingY;
							this.AddPoint(gradarMapPoint.gameObject);
							this.radarMapPointList.Add(gradarMapPoint);
							float num4 = num2 - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
							float num5 = num3 - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
							gradarMapPoint.transform.localPosition = new Vector3(num4, num5, -0.01f);
						}
					}
				}
			}
		}
		this.ItemCollection.DelayUpdate();
	}

	private void ShowArmyShuiJingList()
	{
		if (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi && (Global.Data.roleData.MapCode == 83000 || Global.Data.roleData.MapCode == 83001))
		{
			this.ShowListType = 1;
			string xmlName = string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				"Config/ManorCrystal/",
				Global.Data.roleData.MapCode,
				"/CrystalMonster_",
				Global.Data.roleData.MapCode,
				".xml"
			});
			XElement gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				return;
			}
			ConfigMonsters.PreCacheMonsterXmlNodes();
			Global.AssertException(ConfigMonsters.MonsterXmlNode.Count > 0, StringUtil.substitute(Global.GetLang("加载xml失败: {0}"), new object[]
			{
				"Config/Monsters.Xml"
			}));
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Monsters"), "*");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Code");
					if (!dictionary.ContainsKey(xelementAttributeInt))
					{
						dictionary.Add(xelementAttributeInt, new List<int>());
					}
					dictionary[xelementAttributeInt].Add(Global.GetXElementAttributeInt(xelement, "ID"));
				}
			}
			RandomAS randomAS = new RandomAS(0);
			foreach (int num in dictionary.Keys)
			{
				List<int> list = dictionary[num];
				if (list.Count > 0)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num);
					if (monsterXmlNodeByID != null)
					{
						XElement xelement2 = Global.GetXElement(gameResXml, "Monster", "ID", list[randomAS.Next(0, list.Count)].ToString());
						if (xelement2 != null)
						{
							int monsterType = monsterXmlNodeByID.MonsterType;
							if (monsterType != 501 && monsterType != 601)
							{
								PartMapListItem partMapListItem = U3DUtils.NEW<PartMapListItem>();
								partMapListItem.NameText = monsterXmlNodeByID.SName;
								partMapListItem.NpcID = monsterXmlNodeByID.ID;
								partMapListItem.Tag = StringUtil.substitute("{0},{1},{2},{3},{4}", new object[]
								{
									Global.GetXElementAttributeInt(xelement2, "ID"),
									Global.GetXElementAttributeInt(xelement2, "X"),
									Global.GetXElementAttributeInt(xelement2, "Y"),
									GSpriteTypes.Monster,
									monsterXmlNodeByID.MapCode
								});
								this.ItemCollection.AddNoUpdate(partMapListItem);
							}
						}
					}
				}
			}
			this.ItemCollection.DelayUpdate();
		}
	}

	private void ShowArmyShuiJing()
	{
		if (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi && (Global.Data.roleData.MapCode == 83000 || Global.Data.roleData.MapCode == 83001))
		{
			string xmlName = string.Format("{0}{1}{2}{3}{4}", new object[]
			{
				"Config/ManorCrystal/",
				Global.Data.roleData.MapCode,
				"/CrystalMonster_",
				Global.Data.roleData.MapCode,
				".xml"
			});
			XElement gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				return;
			}
			int num = 0;
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Monster");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					if (xelement != null)
					{
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Code");
						if (num != xelementAttributeInt)
						{
							num = xelementAttributeInt;
							string name = num.ToString();
							GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
							string img = "radarMonster";
							gradarMapPoint.sprite.transform.localScale = new Vector3(6f, 6f, 6f);
							gradarMapPoint.Img = img;
							gradarMapPoint.name = name;
							gradarMapPoint.id = Global.GetXElementAttributeInt(xelement, "Code");
							MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(num);
							if (monsterXmlNodeByID != null)
							{
								gradarMapPoint.Title = monsterXmlNodeByID.SName;
								float num2 = (float)Global.GetXElementAttributeInt(xelement, "X") / 100f * this.ScalingX;
								float num3 = (float)Global.GetXElementAttributeInt(xelement, "Y") / 100f * this.ScalingY;
								this.AddPoint(gradarMapPoint.gameObject);
								this.radarMapPointList.Add(gradarMapPoint);
								float num4 = num2 - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
								float num5 = num3 - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
								gradarMapPoint.transform.localPosition = new Vector3(num4, num5, -0.01f);
							}
						}
					}
				}
			}
		}
	}

	private void ShowNPCSOnMap()
	{
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, StringUtil.substitute("npcs.xml", new object[0]));
		if (gameMapSettingsXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "NPCs"), "*");
		if (xelementList != null)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(Global.GetXElementAttributeInt(xelement, "Code"));
					if (npcvobyID != null)
					{
						if (this.IsShowNPC(Global.GetXElementAttributeStr(xelement, "Code")))
						{
							string sname = npcvobyID.SName;
							GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
							string img = "radarNpc";
							if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
							{
								img = "zhongli";
								gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
							}
							else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong)
							{
								img = "zhongliyaosai";
								gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
							}
							else if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle && ShiLiData.IsQiZuo(npcvobyID.ID))
							{
								switch (ShiLiData.GetQiZuoCompType(npcvobyID.ID))
								{
								case ShiLiType.None:
									img = "qibai";
									break;
								case ShiLiType.ShenShengJiaoTuan:
									img = "qihong";
									break;
								case ShiLiType.ZiYouTongMeng:
									img = "qilan";
									break;
								case ShiLiType.ZhiMengXieHui:
									img = "qihuang";
									break;
								}
								gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
							}
							gradarMapPoint.Img = img;
							gradarMapPoint.name = sname;
							gradarMapPoint.id = npcvobyID.ID;
							float num = (float)Global.GetXElementAttributeInt(xelement, "X") / 100f * this.ScalingX;
							float num2 = (float)Global.GetXElementAttributeInt(xelement, "Y") / 100f * this.ScalingY;
							this.AddPoint(gradarMapPoint.gameObject);
							this.radarMapPointList.Add(gradarMapPoint);
							float num3 = num - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
							float num4 = num2 - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
							gradarMapPoint.transform.localPosition = new Vector3(num3, num4, -0.01f);
						}
					}
				}
			}
		}
	}

	private void ShowMoYuBuffState()
	{
		if (SceneUIClasses.MoYuDuoBao.IsTheScene())
		{
			foreach (KeyValuePair<int, ZorkSceneVO> keyValuePair in IConfigbase<ConfigMoYuDuoBao>.Instance.DictZorkSceneVOCfg)
			{
				ZorkSceneVO value = keyValuePair.Value;
				if (value.ArmyType == 1)
				{
					GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
					gradarMapPoint.Name = "moyu" + value.BuffAreID;
					gradarMapPoint.Img = "radarTeleport";
					gradarMapPoint.Title = string.Empty;
					gradarMapPoint.title.color = Color.green;
					gradarMapPoint.sprite.gameObject.SetActive(false);
					float num = (float)value.BuffArePlace[0] * this.ScalingX / 100f;
					float num2 = (float)value.BuffArePlace[1] * this.ScalingY / 100f;
					this.AddPoint(gradarMapPoint.gameObject);
					this.moYuBuffDic[value.BuffAreID] = gradarMapPoint;
					float num3 = num - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x - 15f;
					float num4 = num2 - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y - 15f;
					gradarMapPoint.transform.localPosition = new Vector3(num3, num4, -0.01f);
				}
			}
			base.StartCoroutine(this.StartUpdateBuff());
		}
	}

	private IEnumerator StartUpdateBuff()
	{
		base.StopCoroutine(this.StartUpdateBuff());
		for (;;)
		{
			this.UpdateMoYuBuffState();
			yield return new WaitForSeconds(0.2f);
		}
		yield break;
	}

	private void UpdateMoYuBuffState()
	{
		Dictionary<int, GRadarMapPoint>.Enumerator enumerator = this.moYuBuffDic.GetEnumerator();
		DateTime correctDateTime = Global.GetCorrectDateTime();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, GRadarMapPoint> keyValuePair = enumerator.Current;
			int key = keyValuePair.Key;
			DateTime appearTime = MoYuDuoBaoData.GetAppearTime(key);
			long num = (long)(appearTime - correctDateTime).TotalSeconds;
			string title = string.Empty;
			if (num > 0L)
			{
				title = this.GetTimeStrBySecEx(num);
			}
			KeyValuePair<int, GRadarMapPoint> keyValuePair2 = enumerator.Current;
			keyValuePair2.Value.Title = title;
		}
	}

	private string GetTimeStrBySecEx(long sec)
	{
		int num = 3600;
		if (sec > (long)num)
		{
			MUDebug.LogError<string>(new string[]
			{
				"请检查时间是否正确"
			});
		}
		long num2 = sec / 60L;
		long num3 = sec % 60L;
		if (num2 > 0L)
		{
			return string.Concat(new object[]
			{
				num2,
				Global.GetLang("分"),
				num3,
				Global.GetLang("秒")
			});
		}
		return num3 + Global.GetLang("秒");
	}

	private void UpdatCompBattle()
	{
		if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle)
		{
			for (int i = 0; i < this.radarMapPointList.Count; i++)
			{
				GRadarMapPoint gradarMapPoint = this.radarMapPointList[i];
				if (ShiLiData.IsQiZuo(gradarMapPoint.id))
				{
					string img = "none";
					switch (ShiLiData.GetQiZuoCompType(gradarMapPoint.id))
					{
					case ShiLiType.None:
						img = "qibai";
						break;
					case ShiLiType.ShenShengJiaoTuan:
						img = "qihong";
						break;
					case ShiLiType.ZiYouTongMeng:
						img = "qilan";
						break;
					case ShiLiType.ZhiMengXieHui:
						img = "qihuang";
						break;
					}
					gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
					gradarMapPoint.Img = img;
				}
			}
		}
	}

	public void AddMapLinePoint(float x, float y)
	{
		GRadarMapPoint gradarMapPoint = U3DUtils.NEW<GRadarMapPoint>();
		gradarMapPoint.Img = "radarAStar";
		gradarMapPoint.name = string.Concat(new object[]
		{
			"line_",
			x,
			"_",
			y
		});
		gradarMapPoint.sprite.MakePixelPerfect();
		float num = x / 100f * this.ScalingX;
		float num2 = y / 100f * this.ScalingY;
		this.AddPointForLine(gradarMapPoint.gameObject);
		this.radarMapLinePointList.Add(gradarMapPoint);
		float num3 = num - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
		float num4 = num2 - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
		gradarMapPoint.transform.localPosition = new Vector3(num3, num4, 0f);
	}

	public void AddMapLinePoint2(GRadarMapPoint NpcDot, int px, int py)
	{
		NpcDot.name = string.Concat(new object[]
		{
			"line_",
			px * 100,
			"_",
			py * 100
		});
		int num = (int)((float)px * this.ScalingX);
		int num2 = (int)((float)py * this.ScalingY);
		this.radarMapLinePointList.Add(NpcDot);
		float num3 = (float)(num - (int)this.localMap.transform.localScale.x / 2) + this.localMap.transform.localPosition.x;
		float num4 = (float)(num2 - (int)this.localMap.transform.localScale.y / 2) + this.localMap.transform.localPosition.y;
		NpcDot.transform.localPosition = new Vector3(num3, num4, 0f);
	}

	public GRadarMapPoint InstantiateMapLinePoint(GRadarMapPoint NpcDot)
	{
		if (null == NpcDot)
		{
			NpcDot = U3DUtils.NEW<GRadarMapPoint>("GRadarMapPoint2");
			NpcDot.Img = "radarAStar";
			NpcDot.name = "line_Point";
			NpcDot.sprite.MakePixelPerfect();
		}
		else
		{
			NpcDot = Object.Instantiate<GameObject>(NpcDot.gameObject).GetComponent<GRadarMapPoint>();
		}
		this.AddPointForLine(NpcDot.gameObject);
		return NpcDot;
	}

	private void ShowTeleportList()
	{
		this.ShowListType = 2;
		this.ItemCollection.Clear();
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(Global.Data.roleData.MapCode, StringUtil.substitute("teleports.xml", new object[0]));
		if (gameMapSettingsXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "Teleports"), "*");
		if (xelementList != null)
		{
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				PartMapListItem partMapListItem = U3DUtils.NEW<PartMapListItem>();
				partMapListItem.NameText = Global.GetXElementAttributeStr(xelement, "Tip");
				partMapListItem.Tag = StringUtil.substitute("{0},{1},{2},{3},{4}", new object[]
				{
					Global.GetXElementAttributeInt(xelement, "Code"),
					Global.GetXElementAttributeInt(xelement, "X"),
					Global.GetXElementAttributeInt(xelement, "Y"),
					GSpriteTypes.Other,
					Global.GetXElementAttributeInt(xelement, "To")
				});
				this.ItemCollection.AddNoUpdate(partMapListItem);
			}
			this.ItemCollection.DelayUpdate();
		}
	}

	public void RefreshAllWorld()
	{
	}

	public void InitPartData()
	{
		this.InitPartialWorld();
	}

	private void GotoBtnClicked(object sender, MouseEvent e)
	{
		GameObject gameObject = sender as GameObject;
		PartMapListItem component = gameObject.GetComponent<PartMapListItem>();
		if (null == component)
		{
			return;
		}
		string tag = component.Tag;
		if (tag == null)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("无法自动寻路: {0}"), new object[]
			{
				component.NameText
			}), 10, 3);
			return;
		}
		string[] array = tag.Split(new char[]
		{
			','
		});
		if (array.Length != 5)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("无法自动寻路: {0}"), new object[]
			{
				component.NameText
			}), 10, 3);
			return;
		}
		Global.Data.TargetNpcID = Convert.ToInt32(array[0]);
		Point pos = new Point(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
		if (this.ShowListType <= 0)
		{
			Global.Data.GameScene.AutoFindRoad(Global.Data.roleData.MapCode, pos, 120, ExtActionTypes.EXTACTION_NPCDLG);
		}
		else
		{
			Global.Data.GameScene.AutoFindRoad(Global.Data.roleData.MapCode, pos, 0, ExtActionTypes.EXTACTION_KILLMONSTER);
		}
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("VIPChuanSong", true);
		bool flag = Global.GetVIPLeve() >= Convert.ToInt32(systemParamByName);
		if (flag)
		{
			if (Global.IsInShiLiZhengBaMap())
			{
				MUComp mucompById = ShiLiData.GetMUCompById(ShiLiData.GetSelfCompType());
				if (mucompById != null && Global.Data.roleData.MapCode != mucompById.MapCode)
				{
					Super.HintMainText(Global.GetLang("非本势力地图无法使用此功能"), 10, 3);
					return;
				}
			}
			if (Super.CanTransport(Global.Data.roleData.MapCode, true, false))
			{
				GameInstance.Game.SpriteTaskTransport(Global.Data.roleData.MapCode, pos.X, pos.Y, 0);
			}
		}
	}

	private void TransBtnClicked(object sender, MouseEvent e)
	{
		GameObject gameObject = sender as GameObject;
		PartMapListItem component = gameObject.transform.parent.GetComponent<PartMapListItem>();
		if (null == component)
		{
			return;
		}
		string tag = component.Tag;
		if (tag == null)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("无法自动寻路: {0}"), new object[]
			{
				component.NameText
			}), 10, 3);
			return;
		}
		string[] array = tag.Split(new char[]
		{
			','
		});
		if (array.Length != 5)
		{
			Super.HintMainText(StringUtil.substitute(Global.GetLang("无法自动寻路: {0}"), new object[]
			{
				component.NameText
			}), 10, 3);
			return;
		}
		Global.Data.TargetNpcID = Convert.ToInt32(array[0]);
		Point coordinate = new Point(Convert.ToInt32(array[1]), Convert.ToInt32(array[2]));
		Global.Data.GameScene.FindSprite("Leader").Coordinate = coordinate;
	}

	private string GetMonstersName(string MonstersID)
	{
		string result = string.Empty;
		string xmlName = StringUtil.substitute("Config/Monsters.Xml", new object[0]);
		XElement gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml == null)
		{
			return Global.GetLang("未知");
		}
		XElement xelement = Global.GetXElement(gameResXml, "Monster", "ID", MonstersID);
		if (xelement != null)
		{
			result = StringUtil.substitute("{0}(LV:{1})", new object[]
			{
				Global.GetXElementAttributeStr(xelement, "SName"),
				Global.GetXElementAttributeStr(xelement, "Level")
			});
		}
		return result;
	}

	private bool IsShowMonster(string monsterID)
	{
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(Global.SafeConvertToInt32(monsterID));
		if (monsterXmlNodeByID == null)
		{
			return false;
		}
		int display = monsterXmlNodeByID.Display;
		return display > 0;
	}

	private bool IsShowNPC(string npcID)
	{
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(Global.SafeConvertToInt32(npcID));
		if (npcvobyID == null)
		{
			return false;
		}
		int display = npcvobyID.Display;
		return display > 0;
	}

	public void RemovePoint(GameObject obj)
	{
		obj.transform.parent = null;
		Object.Destroy(obj);
		obj = null;
	}

	public void AddPoint(GameObject obj)
	{
		obj.transform.parent = this.localMap.transform.parent;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.rotation = Quaternion.identity;
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	private void AddPointForLine(GameObject obj)
	{
		obj.transform.parent = this.LineContainer.transform;
		obj.transform.localPosition = Vector3.zero;
		obj.transform.rotation = Quaternion.identity;
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public void OnClick()
	{
		if (Global.Data.DisableAutoRoad)
		{
			return;
		}
		if (this.localMap.transform.gameObject.activeInHierarchy)
		{
			float num = this.localMap.transform.localPosition.x + 480f;
			float num2 = this.localMap.transform.localPosition.y + 270f;
			float num3 = num - this.localMap.transform.localScale.x / 2f;
			float num4 = num2 - this.localMap.transform.localScale.y / 2f;
			float num5 = UICamera.lastTouchPosition.x / this.ScreenScaleX - num3;
			float num6 = UICamera.lastTouchPosition.y / this.ScreenScaleY - num4;
			num5 *= (float)Global.Data.GameScene.CurrentMapData.MapWidth / this.localMap.transform.localScale.x;
			num6 *= (float)Global.Data.GameScene.CurrentMapData.MapHeight / this.localMap.transform.localScale.y;
			Point point = new Point((int)num5, (int)num6);
			if (this.PartMapClick != null)
			{
				this.PartMapClick.Invoke(this.thisCtrl, new BaseEventArgs
				{
					IDType = -1,
					ID = 0,
					X = point.X,
					Y = point.Y
				});
			}
		}
	}

	public new void Update()
	{
		Vector3 zero = Vector3.zero;
		GSprite gsprite = ObjectsManager.FindSprite(Global.Data.RoleID);
		if (gsprite != null)
		{
			if (gsprite.OnHorseEX)
			{
				if (null != gsprite.HorseController.HorseTrans)
				{
					this.playerTransform = gsprite.HorseController.HorseTrans;
				}
				else
				{
					this.playerTransform = gsprite.The3DGameObject.transform;
				}
			}
			else
			{
				this.playerTransform = gsprite.The3DGameObject.transform;
			}
		}
		if (this.playerTransform == null)
		{
			GameObject gameObject = GameObject.Find("Leader");
			this.playerTransform = ((!(gameObject != null)) ? null : gameObject.transform);
		}
		if (this.playerTransform != null)
		{
			zero.x = this.playerTransform.localPosition.x * this.ScalingX - this.localMap.transform.localScale.x / 2f + this.localMap.transform.localPosition.x;
			zero.y = this.playerTransform.localPosition.z * this.ScalingY - this.localMap.transform.localScale.y / 2f + this.localMap.transform.localPosition.y;
			zero.z = -0.01f;
			this.playerArrow.transform.localPosition = zero;
			this.playerArrow.transform.localRotation = Quaternion.Euler(0f, 0f, 180f - this.playerTransform.rotation.eulerAngles.y);
			if (!this.IsInDaTaoShaTargetSafeArea(zero.x, zero.y) && Global.IsInDaTaoSha() && DaTaoShaDataManager.EBattleStatus >= EscapeBattleGameSceneStatuses.STATUS_FIGHT)
			{
				NGUITools.SetActive(this.DaTaoShaGuideLine.gameObject, true);
				this.TargetX = this.playerArrow.transform.localPosition.x;
				this.TargetY = this.playerArrow.transform.localPosition.y;
				float distanceOfTwoPoint = this.GetDistanceOfTwoPoint(this.TargetX, this.TargetY, this.OriginX, this.OriginY);
				float degreeByAtan = this.GetDegreeByAtan2(this.TargetX - this.OriginX, this.TargetY - this.OriginY);
				this.DaTaoShaGuideLine.transform.localScale = new Vector3(distanceOfTwoPoint, this.DaTaoShaGuideLine.transform.localScale.y, this.DaTaoShaGuideLine.transform.localScale.z);
				this.DaTaoShaGuideLine.transform.localRotation = Quaternion.Euler(0f, 0f, degreeByAtan);
			}
			else
			{
				NGUITools.SetActive(this.DaTaoShaGuideLine.gameObject, false);
			}
			int count = this.radarMapLinePointList.Count;
			if (count > 0)
			{
				int num = (int)this.playerTransform.localPosition.x * 100;
				int num2 = (int)this.playerTransform.localPosition.z * 100;
				for (int i = 0; i < count; i++)
				{
					GRadarMapPoint gradarMapPoint = this.radarMapLinePointList[i];
					if (gradarMapPoint.name == string.Concat(new object[]
					{
						"line_",
						num,
						"_",
						num2
					}))
					{
						if (i > 0)
						{
							for (int j = i; j >= 0; j--)
							{
								gradarMapPoint = this.radarMapLinePointList[j];
								this.radarMapLinePointList.RemoveAt(j);
								Object.Destroy(gradarMapPoint.gameObject);
							}
						}
						else
						{
							this.radarMapLinePointList.RemoveAt(i);
							Object.Destroy(gradarMapPoint.gameObject);
						}
						break;
					}
				}
			}
		}
	}

	public void ClearRadarMapPoint()
	{
		for (int i = 0; i < this.radarMapPointList.Count; i++)
		{
			this.radarMapPointList[i].transform.parent = null;
			Object.Destroy(this.radarMapPointList[i].gameObject);
			this.radarMapPointList[i] = null;
		}
		this.radarMapPointList.Clear();
		foreach (KeyValuePair<int, GRadarMapPoint> keyValuePair in this.moYuBuffDic)
		{
			keyValuePair.Value.transform.parent = null;
			Dictionary<int, GRadarMapPoint>.Enumerator enumerator;
			KeyValuePair<int, GRadarMapPoint> keyValuePair2 = enumerator.Current;
			Object.Destroy(keyValuePair2.Value.gameObject);
		}
		this.moYuBuffDic.Clear();
	}

	public void ClearRaderMapLinePoint()
	{
		for (int i = 0; i < this.radarMapLinePointList.Count; i++)
		{
			this.radarMapLinePointList[i].transform.parent = null;
			Object.Destroy(this.radarMapLinePointList[i].gameObject);
			this.radarMapLinePointList[i] = null;
		}
		this.radarMapLinePointList.Clear();
	}

	private void InitGoodsData()
	{
		SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
		if (settingMapVOByCode != null)
		{
			string goods = settingMapVOByCode.Goods;
			string[] array = goods.Split(new char[]
			{
				'|'
			});
			List<GoodsData> goodsList = UIHelper.ParseRewardGoodsList(goods, 0, int.MaxValue);
			UIHelper.AddAwardGoods(this.goodsListBox.ItemsSource, goodsList, null, false, "bagGrid4_bak", true);
		}
	}

	private void initGood(string[] goods)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.transform.localPosition = new Vector3(0f, 0f, -0.1f);
			this.goodsItemCollection.AddNoUpdate(ggoodIcon);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
			ggoodIcon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon2 = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon2)
				{
					return;
				}
				GoodsData goodsData2 = ggoodIcon2.ItemObject as GoodsData;
				if (goodsData2 == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(ggoodIcon2, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData2);
			});
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			component.center = new Vector3(0f, 0f, -1f);
		}
	}

	public GRadarMapPoint GetGRadarMapPoint(int ID)
	{
		GRadarMapPoint result = null;
		for (int i = 0; i < this.radarMapPointList.Count; i++)
		{
			if (ID == this.radarMapPointList[i].id)
			{
				result = this.radarMapPointList[i];
				break;
			}
		}
		return result;
	}

	public void ReMapPointImage()
	{
		if (Global.KarenScorepProvisionalData == null)
		{
			return;
		}
		for (int i = 0; i < Global.KarenScorepProvisionalData.Count; i++)
		{
			if (Global.KarenScorepProvisionalData[i].ResourceList != null && Global.KarenScorepProvisionalData[i].ResourceList.Count > 0)
			{
				for (int j = 0; j < Global.KarenScorepProvisionalData[i].ResourceList.Count; j++)
				{
					if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
					{
						if (PlayZone.GlobalPlayZone.GetDicLegionsWest().ContainsKey(Global.KarenScorepProvisionalData[i].ResourceList[j]))
						{
							this.QiZuoSite = PlayZone.GlobalPlayZone.GetDicLegionsWest()[Global.KarenScorepProvisionalData[i].ResourceList[j]].QiZuoID;
						}
					}
					else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong && PlayZone.GlobalPlayZone.GetDicLegionsEast().ContainsKey(Global.KarenScorepProvisionalData[i].ResourceList[j]))
					{
						this.QiZuoSite = PlayZone.GlobalPlayZone.GetDicLegionsEast()[Global.KarenScorepProvisionalData[i].ResourceList[j]].NPCID;
					}
					GRadarMapPoint gradarMapPoint = this.GetGRadarMapPoint(this.QiZuoSite);
					if (gradarMapPoint != null)
					{
						gradarMapPoint.sprite.transform.localScale = new Vector3(15f, 15f, 1f);
						if (i == Global.Data.roleData.BattleWhichSide - 1)
						{
							gradarMapPoint.Img = ((Global.GetMapSceneUIClass() != SceneUIClasses.AKaLunXi) ? "mineyaosai" : "mine");
						}
						else
						{
							gradarMapPoint.Img = ((Global.GetMapSceneUIClass() != SceneUIClasses.AKaLunXi) ? "direnyaosai" : "diren");
						}
					}
				}
			}
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
		MUEventManager.AddEventListener<List<CompBattleSideScore>>("CMD_SPR_COMP_BATTLE_SIDE_SCORE", new Action<List<CompBattleSideScore>>(this.ServerGetBattleSideScore));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<List<CompBattleSideScore>>("CMD_SPR_COMP_BATTLE_SIDE_SCORE", new Action<List<CompBattleSideScore>>(this.ServerGetBattleSideScore));
	}

	private void ServerGetBattleSideScore(List<CompBattleSideScore> scoreInfo)
	{
		this.UpdatCompBattle();
	}

	public ListBox listBox;

	public ShowNetImage localMap;

	public GCheckBox chkMonster;

	public GCheckBox chkNPC;

	public GCheckBox chkTele;

	private Transform playerTransform;

	public GRadarMapPoint playerArrow;

	private List<GRadarMapPoint> radarMapPointList = new List<GRadarMapPoint>();

	private Dictionary<int, GRadarMapPoint> moYuBuffDic = new Dictionary<int, GRadarMapPoint>();

	public GTabControl gTabCtrl;

	public UIScrollBar scrollBar;

	public UILabel lblDebug;

	public GButton BtnClose;

	private int MAPCODE_YONGZHEDALU = 1;

	public UIButton btnYongzhedalu;

	private int MAPCODE_TIANKONG = 8;

	public UIButton btnTiankong;

	private int MAPCODE_SHILUOZHITA = 5;

	public UIButton btnShiluozhita;

	private int MAPCODE_BINGFENGGU = 3;

	public UIButton btnBingfenggu;

	private int MAPCODE_SIWANGSHAMO = 7;

	public UIButton btnSiwangshamo;

	private int MAPCODE_YATELANDISI = 6;

	public UIButton btnYatelandisi;

	private int MAPCODE_XIANZONGLIN = 2;

	public UIButton btnXianzonglin;

	private int MAPCODE_DIXIACHENG = 4;

	public UIButton btnDixiacheng;

	private int MAPCODE_YOUANSENLIN = 9;

	public UIButton btnYouansenlin;

	private int MAPCODE_CANTELUFEIXU = 10;

	public UIButton btnCantelufeixu;

	private int MAPCODE_CANTELUYIZHI = 20;

	public UIButton btnCanteluyizhi;

	private int MAPCODE_BINGSHUANGZHICHENG = 30;

	public UIButton btnBingshuangzhicheng;

	private int MAPCODE_ANNINGCHI = 40;

	public UIButton btnAnNingChi;

	private int MAPCODE_KUAFUZHUXIANDITU = 50;

	public UIButton btnKuafuzhuxianMap;

	private int MAPCODE_HUANSHUYUAN = 60;

	public UIButton btnHuanShuYuanMap;

	private int MAPCODE_KALUNXI = 70;

	public UIButton btnKaLunXiMap;

	private int MAPCODE_KALUNDONG = 80;

	public UIButton btnKaLunDongMap;

	private int MAPCODE_XIEESIMIAO = 90;

	public UIButton btnXieESiMiaoMap;

	private int MAPCODE_MEIYINMIYAO = 100;

	public UIButton btnMeiYinMiYaoMap;

	private int MAPCODE_SHILI = 93000;

	public UIButton btnShiLiMap;

	private int MAPCODE_REBIRTH = 96000;

	public UIButton btnRebirthMap;

	private int MAPCODE_MIZHONG = 110;

	public UIButton btnMiZhong;

	public UILabel lblDebugInfo;

	public UILabel lblMapName;

	public GameObject LineContainer;

	public EventHandler PartMapClick;

	private SpriteSL thisCtrl;

	private Canvas Root;

	private int ShowListType;

	private Dictionary<string, bool> MonstersDict = new Dictionary<string, bool>();

	public DPSelectedItemEventHandler DPSelectedItem;

	public ObservableCollection ItemCollection;

	public localMap m_localMap;

	public ListBox goodsListBox;

	public ObservableCollection goodsItemCollection;

	public UILabel[] ConstTexts;

	public GameObject PnlContent;

	private float _ScalingX;

	private float _scalingY;

	private float _ScreenScaleX;

	private float _ScreenScaleY;

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

	public float DaTaoShaTargerSafeAreaRadius;

	private Vector2 DaTaoShaTargerSafeAreaPosition = default(Vector2);

	public bool NeedToRefresh;

	private List<GRadarMapPoint> radarMapLinePointList = new List<GRadarMapPoint>();

	private Vector3 zeroPos = Vector3.zero;

	private int QiZuoSite;
}
