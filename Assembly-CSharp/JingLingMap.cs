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
using Tmsk.Xml;
using UnityEngine;

public class JingLingMap
{
	public JingLingMap.JingLingMapType showType
	{
		get
		{
			if (this._showType == JingLingMap.JingLingMapType.NuLiSearch)
			{
				return this._showType;
			}
			if (JingLingMap.IsInHome())
			{
				this._showType = JingLingMap.JingLingMapType.MyHome;
			}
			else
			{
				this._showType = JingLingMap.JingLingMapType.FriendHome;
			}
			return this._showType;
		}
		set
		{
			if (value == JingLingMap.JingLingMapType.NuLiSearch)
			{
				this._showType = JingLingMap.JingLingMapType.NuLiSearch;
			}
		}
	}

	public JingLingMapMini mapmini
	{
		get
		{
			return JingLingMap.pz.mJingLingMapMini;
		}
	}

	public GChildWindow mapminiwindow
	{
		get
		{
			return JingLingMap.pz.mJingLingMapWindow;
		}
	}

	public static JingLingMap inst
	{
		get
		{
			if (JingLingMap._map == null)
			{
				JingLingMap._map = new JingLingMap();
			}
			return JingLingMap._map;
		}
	}

	public static PlayZone pz
	{
		get
		{
			return PlayZone.GlobalPlayZone;
		}
	}

	public int curRoleID
	{
		get
		{
			return this._curRoleID;
		}
	}

	public static void ClearStaticData()
	{
		if (JingLingMap._map != null)
		{
			JingLingMap._map.DestroyMapScene();
			JingLingMap._map = null;
		}
	}

	public void DestroyMapScene()
	{
		if (this.localEulerAnglesBak.Key)
		{
			Global.MainCamera.transform.localEulerAngles = (Vector3)this.localEulerAnglesBak.Value[0];
			Global.MainCamera.fieldOfView = (float)this.localEulerAnglesBak.Value[1];
		}
		while (GameObject.Find(typeof(JingLingMapEvent).ToString()))
		{
			Object.Destroy(GameObject.Find(typeof(JingLingMapEvent).ToString()));
		}
		JingLingMap.StopTimer();
		QualitySettings.shadowDistance = 0f;
		if (JingLingMap.pz != null)
		{
			JingLingMap.pz.CloseJingLingMapWindow();
		}
		this.homeface = null;
		this.bossface = null;
		this.taskfaces.Clear();
		this._showType = JingLingMap.JingLingMapType.MyHome;
		if (JingLingMap.pz != null)
		{
			while (U3DUtils.FindGameObjectByName(JingLingMap.pz.gameObject, "GChildWindow(Clone)"))
			{
				NGUITools.Destroy(U3DUtils.FindGameObjectByName(JingLingMap.pz.gameObject, "GChildWindow(Clone)"));
			}
		}
		JingLingMap._map = null;
	}

	public void EnterMapScene()
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (mapSceneUIClass != this.JingLingFuBenClass)
		{
			JingLingMap.inst.MapChange();
			return;
		}
		if (JingLingMap.debugShowShadow)
		{
			QualitySettings.shadowDistance = 50f;
			QualitySettings.shadowProjection = 0;
		}
		if (Global.IsInJingLingMap())
		{
			JingLingMap.RequestInitData();
			return;
		}
		if (GameObject.Find("Scene/building"))
		{
			GameObject.Find("Scene/building").SetActive(JingLingMap.debugShowBuild);
		}
		if (GameObject.Find("Scene"))
		{
			Renderer[] componentsInChildren = GameObject.Find("Scene").gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				for (int j = 0; j < componentsInChildren[i].sharedMaterials.Length; j++)
				{
					if (componentsInChildren[i].sharedMaterials[j] != null)
					{
						string name = componentsInChildren[i].sharedMaterials[j].shader.name;
						if (componentsInChildren[i].gameObject.name.StartsWith("79_terrain_001"))
						{
							componentsInChildren[i].sharedMaterials[j].shader = Shader.Find("lingdi");
						}
					}
				}
			}
		}
		JingLingMap.StartUITimer();
		for (int k = 0; k < JingLingMapObjectData.ObjectDataList.Count; k++)
		{
			GameObject gameObject = GameObject.CreatePrimitive(3);
			gameObject.AddComponent<JingLingMapObj>().index = k;
		}
		this.ResetCamera();
		JingLingMap.pz.OpenJingLingMapWindow();
		EffectRoot[] array = Object.FindObjectsOfType<EffectRoot>();
		foreach (EffectRoot effectRoot in array)
		{
			effectRoot.gameObject.SetActive(false);
		}
		if (this.showType == JingLingMap.JingLingMapType.Nodata || this.showType == JingLingMap.JingLingMapType.MyHome || this.showType == JingLingMap.JingLingMapType.FriendHome)
		{
			this._curRoleID = Global.Data.roleData.RoleID;
			JingLingMap.RequestInitData();
		}
		else if (this.showType == JingLingMap.JingLingMapType.NuLiSearch && Global.IsInJingLingMap())
		{
			JingLingMap.inst.OnRequetYaoSaiDataBack(Global.Data.yaosaiData);
		}
	}

	public void ResetCamera()
	{
		if (!this.localEulerAnglesBak.Key)
		{
			bool flag = true;
			ArrayList arrayList = new ArrayList(2);
			arrayList.Add(Global.MainCamera.transform.localEulerAngles);
			arrayList.Add(Global.MainCamera.fieldOfView);
			this.localEulerAnglesBak = new KeyValuePair<bool, ArrayList>(flag, arrayList);
		}
		Global.MainCamera.gameObject.SetActive(true);
		Global.MainCamera.transform.position = new Vector3(25.1f, 64.1f, 9.1f);
		Global.MainCamera.transform.localEulerAngles = new Vector3(46.44f, 0f, 0f);
		Global.MainCamera.fieldOfView = 45f;
	}

	public void JingLingMapMiniHanders(object sender, DPSelectedItemEventArgs args)
	{
		if (args != null)
		{
			if (args.IDType == 1)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("PetMissionMax", ',');
				if (systemParamIntArrayByName.Length == 2 && JingLingMap.inst.ExcuteMissionCount >= systemParamIntArrayByName[0])
				{
					Super.HintMainText(Global.GetLang("达到当天任务完成上限，无法刷新任务"), 10, 3);
					return;
				}
				int num = 0;
				int i = 0;
				int count = this.taskfaces.Count;
				while (i < count)
				{
					if (this.taskfaces[i].TaskState == JingLingMapTaskFace.EState.JieRenWuing)
					{
						num++;
					}
					if (this.taskfaces[i].YaoSaiMissionData == null)
					{
						num++;
					}
					i++;
				}
				if (5 < num)
				{
					num = 5;
				}
				if (0 >= num)
				{
					Super.HintMainText(Global.GetLang("当前无可刷新的任务"), 10, 3);
				}
				else
				{
					long RefreshMissionCost = ConfigSystemParam.GetSystemParamIntByName("RefreshMissionCost");
					string[] buttons = new string[]
					{
						Global.GetLang("确认"),
						Global.GetLang("取消")
					};
					if (0L <= Global.GetCorrectDateTime().Ticks - JingLingMap.inst.FreeRefreshTime.Ticks)
					{
						Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("确定刷新当前") + num + Global.GetLang("个可刷新任务？"), delegate(object e, DPSelectedItemEventArgs s)
						{
							if (s.ID == 0)
							{
								GameInstance.Game.SendRefreshJingLingYaoSaiTask();
							}
						}, buttons);
					}
					else
					{
						Super.ZuanShiShowMessageBox(Global.GetLang("提示"), string.Concat(new object[]
						{
							Global.GetLang("确定花费      "),
							RefreshMissionCost.ToString(),
							Global.GetLang("刷新当前"),
							num,
							Global.GetLang("个可刷\n\r新任务？")
						}), 1, delegate(object e, DPSelectedItemEventArgs s)
						{
							if (s.ID == 0)
							{
								if (RefreshMissionCost > (long)Global.Data.roleData.UserMoney && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("JingLingYaoSaiShuaXin", (int)RefreshMissionCost, false))
								{
									Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
								}
								else
								{
									GameInstance.Game.SendRefreshJingLingYaoSaiTask();
								}
							}
						}, MessBoxIsHintTypes.JjingLingYaoSai, -130f, "JingLingYaoSaiShuaXin", (int)RefreshMissionCost);
					}
				}
			}
			else if (args.IDType == 2)
			{
			}
		}
	}

	public void MapChange()
	{
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (JingLingMap.preUIClass == mapSceneUIClass)
		{
			return;
		}
		if (JingLingMap.preUIClass == SceneUIClasses.JingLingFuBenClass)
		{
			this.setGameUIVisiable(false);
			this.DestroyMapScene();
			JingLingMap.preUIClass = mapSceneUIClass;
			return;
		}
		if (mapSceneUIClass == SceneUIClasses.JingLingFuBenClass)
		{
			this.setGameUIVisiable(true);
		}
		JingLingMap.preUIClass = mapSceneUIClass;
	}

	private void setGameUIVisiable(bool enter)
	{
		if (enter)
		{
			this.pzChildDic.Clear();
			for (int i = 0; i < JingLingMap.pz.transform.childCount; i++)
			{
				Transform child = JingLingMap.pz.transform.GetChild(i);
				if (child.name.StartsWith("GChildWindow"))
				{
					child.gameObject.SetActive(false);
					JingLingMap.pz.CloseYaoSaiBossInfoPart();
					JingLingMap.pz.CloseYaoSaiCallBossPart();
					JingLingMap.pz.CloseFortressPart();
					JingLingMap.pz.CloseYaoSaiJingLingBattleAwardsPart();
					JingLingMap.pz.CloseYaoSaiXinXiPartWindow();
					JingLingMap.pz.CloseYaoSaiJianYuPartWindow();
				}
				else if (child.name.StartsWith("JingLingMapMini"))
				{
					child.gameObject.SetActive(true);
				}
				else if (child.name.StartsWith("GetGoodsHintPart(Clone)"))
				{
					child.gameObject.SetActive(true);
				}
				else if (child.name.StartsWith("RoleExpProgressBar(Clone)"))
				{
					child.gameObject.SetActive(true);
				}
				else
				{
					this.pzChildDic[child.name] = child.gameObject.activeSelf;
					child.gameObject.SetActive(false);
				}
			}
			Global.Joystick.gameObject.SetActive(false);
		}
		else
		{
			for (int j = 0; j < JingLingMap.pz.transform.childCount; j++)
			{
				Transform child2 = JingLingMap.pz.transform.GetChild(j);
				if (child2.name.StartsWith("GChildWindow"))
				{
					child2.gameObject.SetActive(false);
				}
				else if (child2.name.StartsWith("JingLingMapMini"))
				{
					child2.gameObject.SetActive(false);
				}
				else if (this.pzChildDic.ContainsKey(child2.name))
				{
					child2.gameObject.SetActive(this.pzChildDic[child2.name]);
				}
			}
			Global.Joystick.gameObject.SetActive(true);
		}
	}

	protected static void StartUITimer()
	{
		if (JingLingMap.UITimer == null)
		{
			JingLingMap.UITimer = new DispatcherTimer(typeof(JingLingMapEvent).ToString());
			JingLingMap.UITimer.Interval = TimeSpan.FromSeconds(0.5);
			JingLingMap.UITimer.Tick = new DispatcherTimerEventHandler(JingLingMap.UITimer_Tick);
		}
		JingLingMap.UITimer.Start();
	}

	private static void StopTimer()
	{
		if (JingLingMap.UITimer != null)
		{
			JingLingMap.UITimer.Tick = null;
			JingLingMap.UITimer.Stop();
			JingLingMap.UITimer = null;
		}
	}

	protected static void UITimer_Tick(object sender, object e)
	{
		if (PlayZone.GlobalPlayZone == null || PlayZone.GlobalPlayZone.GameFubenBoxMini == null)
		{
			return;
		}
		SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
		if (mapSceneUIClass != SceneUIClasses.JingLingFuBenClass)
		{
			return;
		}
		if (JingLingMap.inst.mapmini == null)
		{
			return;
		}
		if (JingLingMap.inst.mapmini)
		{
			JingLingMap.inst.mapmini.UITimer_Tick(sender, e);
		}
		if (JingLingMap.inst.homeface)
		{
			JingLingMap.inst.homeface.UITimer_Tick(sender, e);
		}
		if (JingLingMap.inst.bossface)
		{
			JingLingMap.inst.bossface.UITimer_Tick(sender, e);
		}
		for (int i = 0; i < JingLingMap.inst.taskfaces.Count; i++)
		{
			if (JingLingMap.inst.taskfaces[i])
			{
				JingLingMap.inst.taskfaces[i].UITimer_Tick(sender, e);
			}
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		if (correctDateTime.Hour == 0 && correctDateTime.Minute == 0 && correctDateTime.Second == 1 && JingLingMap.IsInHome())
		{
			GameInstance.Game.SendYaoSaiMainBossMainInfoRequest(Global.Data.roleData.RoleID);
			GameInstance.Game.SendGetJingLingYaiSaiData(Global.Data.roleData.RoleID);
			GameInstance.Game.SendYaoSaiData(Global.Data.roleData.RoleID);
		}
	}

	public static void ShowSomething()
	{
	}

	public static bool IsInHome()
	{
		YaoSaiBossMainData yaoSaiBossMainData = JingLingMap.inst.CurYaoSaiBossMainData;
		return Global.Data != null && Global.Data.roleData != null && yaoSaiBossMainData != null && yaoSaiBossMainData.OtherID == Global.Data.roleData.RoleID;
	}

	public static void RequestInitData()
	{
		JingLingMap.inst.RequestChangeMap(Global.Data.roleData.RoleID);
	}

	public static void OnRequetInitDataBack(object obj)
	{
		if (obj == null)
		{
			return;
		}
		if (obj.GetType() == typeof(YaoSaiBossMainData))
		{
			YaoSaiBossMainData yaoSaiBossMainData = JingLingMap.inst.CurYaoSaiBossMainData;
			if (yaoSaiBossMainData.BossMiniInfo == null || yaoSaiBossMainData.BossMiniInfo.BossID > 0)
			{
			}
		}
		if (JingLingMap.inst.CurYaoSaiBossMainData.OtherID != JingLingMap.inst.curRoleID)
		{
			return;
		}
		if (JingLingMap.inst.bossface)
		{
			JingLingMap.inst.bossface.Clear();
			JingLingMap.inst.bossface.ResetState();
			JingLingMap.inst.bossface.UpdateUI();
			JingLingMap.inst.bossface.UpdateGameObject();
		}
		if (JingLingMap.inst.homeface)
		{
			JingLingMap.inst.homeface.ResetState();
			JingLingMap.inst.homeface.UpdateUI();
			JingLingMap.inst.homeface.UpdateGameObject();
		}
		if (JingLingMap.inst.mapmini)
		{
			JingLingMap.inst.mapmini.UpdateUI();
		}
		if (JingLingMap.inst.curMissionDataLst.Count > 0)
		{
			for (int i = 0; i < JingLingMap.inst.taskfaces.Count; i++)
			{
				JingLingMap.inst.taskfaces[i].ResetState();
				JingLingMap.inst.taskfaces[i].UpdateUI();
			}
		}
	}

	public void GetTaskAwarkCallBack(int ret)
	{
		if (ret == 0 && this.taskfaces != null)
		{
			Dictionary<int, JingLingMapTaskFace>.Enumerator enumerator = this.taskfaces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Object @object = null;
				KeyValuePair<int, JingLingMapTaskFace> keyValuePair = enumerator.Current;
				if (@object != keyValuePair.Value)
				{
					KeyValuePair<int, JingLingMapTaskFace> keyValuePair2 = enumerator.Current;
					if (keyValuePair2.Value.ClickGetAwark == 1)
					{
						KeyValuePair<int, JingLingMapTaskFace> keyValuePair3 = enumerator.Current;
						string[] awardStrAray = keyValuePair3.Value.GetAwardStrAray();
						if (awardStrAray != null && 0 < awardStrAray.Length)
						{
							PlayZone.GlobalPlayZone.OpenYaoSaiJingLingBattleAwardsPart(4, awardStrAray, 0, 0, 0f, true, 0, null);
							KeyValuePair<int, JingLingMapTaskFace> keyValuePair4 = enumerator.Current;
							keyValuePair4.Value.ClickGetAwark = 0;
							break;
						}
					}
				}
			}
		}
	}

	public void OnRequetTaskDataBack(YaoSaiMissionMainData missionMainData)
	{
		this.curMissionDataLst.Clear();
		for (int i = 0; i < JingLingMapObjectData.TaskCout; i++)
		{
			this.curMissionDataLst.Add(null);
		}
		if (missionMainData != null)
		{
			if (missionMainData.MissionDataList != null)
			{
				for (int j = 0; j < missionMainData.MissionDataList.Count; j++)
				{
					int missionID = missionMainData.MissionDataList[j].MissionID;
					ConfigPetMissionXml configPetMissionXml = new ConfigPetMissionXml(missionID);
					if (configPetMissionXml.GetPetMissionVO() != null)
					{
						int num = configPetMissionXml.GetPetMissionVO().Type;
						num--;
						if (num < this.curMissionDataLst.Count)
						{
							this.curMissionDataLst[num] = missionMainData.MissionDataList[j];
						}
					}
				}
			}
			this.FreeRefreshTime = missionMainData.FreeRefreshTime;
			this.ExcuteMissionCount = missionMainData.ExcuteMissionCount;
			if (null != this.mapmini)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("PetMissionMax", ',');
				if (systemParamIntArrayByName.Length == 2)
				{
					if (JingLingMap.inst.ExcuteMissionCount >= systemParamIntArrayByName[0])
					{
						this.mapmini.renwuKeLingQuLbl.text = Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("可领取任务：")
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"ff0000",
							this.ExcuteMissionCount + " /" + systemParamIntArrayByName[0]
						});
					}
					else
					{
						this.mapmini.renwuKeLingQuLbl.text = Global.GetColorStringForNGUIText(new object[]
						{
							"e3b36c",
							Global.GetLang("可领取任务：")
						}) + Global.GetColorStringForNGUIText(new object[]
						{
							"17e43e",
							this.ExcuteMissionCount + " /" + systemParamIntArrayByName[0]
						});
					}
				}
			}
		}
		if (null != this.mapmini)
		{
			this.mapmini.UpdateUI();
		}
		if (this.curMissionDataLst.Count > 0)
		{
			for (int k = 0; k < this.taskfaces.Count; k++)
			{
				this.taskfaces[k].Clear();
				this.taskfaces[k].ResetState();
				this.taskfaces[k].UpdateUI();
			}
		}
	}

	public void OnRequetYaoSaiDataBack(YaoSaiData yaoSaiData)
	{
		if (null != this.mapmini)
		{
			this.mapmini.UpdateUI();
		}
		if (null != this.homeface)
		{
			this.homeface.ResetState();
			this.homeface.UpdateUI();
			this.homeface.UpdateGameObject();
		}
	}

	public void ProcessNpcClickOnStart(SpriteNotifyEventArgs e)
	{
	}

	public void OpenWorldBattle(JingLingMap.JingLingMapType showtype = JingLingMap.JingLingMapType.MyHome)
	{
		this._showType = showtype;
		if (showtype == JingLingMap.JingLingMapType.MyHome)
		{
			this._curRoleID = Global.Data.roleData.RoleID;
		}
		else
		{
			this._curRoleID = -1;
		}
		JingLingMapEvent.ProcessEvent(EmJingMapEvent.FromJingLingMap_WorldBattle);
	}

	public YaoSaiBossMainData CurYaoSaiBossMainData
	{
		get
		{
			return this.curYaoSaiBossMainData;
		}
		set
		{
			if (value == null)
			{
				this.curYaoSaiBossMainData = null;
				return;
			}
			if (this.curYaoSaiBossMainData == null)
			{
				this.curYaoSaiBossMainData = new YaoSaiBossMainData();
			}
			this.curYaoSaiBossMainData.BossMiniInfo = value.BossMiniInfo;
			this.curYaoSaiBossMainData.TaoFaCount = value.TaoFaCount;
			this.curYaoSaiBossMainData.HaveZhaoHuanCount = value.HaveZhaoHuanCount;
			this.curYaoSaiBossMainData.ZhaoHuanBossIDss = value.ZhaoHuanBossIDss;
			this.curYaoSaiBossMainData.OtherID = value.OtherID;
			this.curYaoSaiBossMainData.FightCount = value.FightCount;
		}
	}

	public void RequestChangeMap(int tarRoleID)
	{
		if (tarRoleID != Global.Data.roleData.RoleID && this.curRoleID == tarRoleID)
		{
			return;
		}
		this._curRoleID = tarRoleID;
		this.curMissionDataLst.Clear();
		for (int i = 0; i < JingLingMapObjectData.TaskCout; i++)
		{
			this.curMissionDataLst.Add(null);
		}
		Global.Data.mYaoSaiBossMainData = null;
		JingLingMap.inst.CurYaoSaiBossMainData = null;
		Global.Data.yaosaiData = null;
		GameInstance.Game.SendYaoSaiMainBossMainInfoRequest(this.curRoleID);
		GameInstance.Game.SendGetJingLingYaiSaiData(this.curRoleID);
		GameInstance.Game.SendYaoSaiData(this.curRoleID);
		this.ResetMapUIState();
	}

	public void ResetMapUIState()
	{
		this.curMissionDataLst.Clear();
		for (int i = 0; i < JingLingMapObjectData.TaskCout; i++)
		{
			this.curMissionDataLst.Add(null);
		}
		if (this.curMissionDataLst.Count > 0)
		{
			for (int j = 0; j < this.taskfaces.Count; j++)
			{
				this.taskfaces[j].ResetState();
				this.taskfaces[j].UpdateUI();
			}
		}
	}

	public GSprite ForceCreateFadeNPC(int npcID, float posX, float posY, GSpriteTypes spriteType, string resname, string modelObjectName, int dir)
	{
		int mapCode = Global.Data.roleData.MapCode;
		string text = string.Concat(new object[]
		{
			"<NPC ID=\"",
			npcID,
			"\" Code=\"\" PicCode=\"\" ResName=\"",
			resname,
			"\" YouShou=\"\" ZuoShou=\"\" Wing=\"\" FlyPet=\"\" Scale=\"1.1\" ShaderID=\"\" GuaJieDian=\"\" GuaJieTeXiao=\"\" Function=\"药店\" SName=\"莱雅\" MapCode=\"1\" Talk=\"勇士\" Sex=\"1\" Tasks=\"\" Operations=\"0\" SaleID=\"3,31\" LuaScriptFile=\"\" Scripts=\"-1\" Display=\"1\" PlaySound=\"\" Interval=\"\" TakeSound=\"\" Collide=\"\" Obstacle=\"\" IsSafe=\"2\" />"
		});
		XElement npcXmlNode = XElement.Parse(text);
		int num = (int)posX / Global.Data.GameScene.CurrentMapData.GridSizeX;
		int num2 = (int)posY / Global.Data.GameScene.CurrentMapData.GridSizeY;
		return this._AddNewNPCEx(npcXmlNode, (double)posX, (double)posY, dir, spriteType, resname, modelObjectName);
	}

	private GSprite _AddNewNPCEx(XElement npcXmlNode, double posX, double posY, int dir, GSpriteTypes spriteType, string resname, string modelObjectName)
	{
		int roleID = 2130706432 + Global.GetXElementAttributeInt(npcXmlNode, "ID");
		if (GameObject.Find(modelObjectName) != null)
		{
			return GameObject.Find(modelObjectName).GetComponent<GSprite>();
		}
		GSprite gsprite = new GSprite();
		gsprite.SpriteType = GSpriteTypes.NPC;
		gsprite.CreateByClient = true;
		string xelementAttributeStr = Global.GetXElementAttributeStr(npcXmlNode, "SName");
		this._LoadSprite(gsprite, roleID, Global.GetXElementAttributeInt(npcXmlNode, "Sex"), modelObjectName, string.Empty, string.Empty, xelementAttributeStr, 0, Global.GetXElementAttributeInt(npcXmlNode, "ID"), 65280U, 65280U, 65280U, 0.0, 0, 0, -1.0, Global.GetXElementAttributeInt(npcXmlNode, "Code"), -1, new Point((int)posX, (int)posY), dir, 0.0, 0.0, 0, true);
		int xelementAttributeInt = Global.GetXElementAttributeInt(npcXmlNode, "ID");
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(100);
		string resName = npcvobyID.ResName;
		if (spriteType == GSpriteTypes.Monster)
		{
			gsprite.ExtensionID = xelementAttributeInt;
			gsprite.SpriteType = spriteType;
			npcvobyID.ResName = resname;
		}
		else if (spriteType == GSpriteTypes.NPC)
		{
			gsprite.ExtensionID = xelementAttributeInt;
			gsprite.SpriteType = spriteType;
			npcvobyID.ResName = resname;
		}
		gsprite.Start();
		gsprite.SpriteType = GSpriteTypes.NPC;
		npcvobyID.ResName = resName;
		gsprite.IsOpposition = false;
		Global.CurrentMapData._MapGrid.MoveObject(-1, -1, gsprite.cx, gsprite.cy, gsprite);
		return gsprite;
	}

	private GSprite _LoadSprite(GSprite sprite, int roleID, int roleSex, string name, string faction, string otherName, string sname, int occupation, int extensionID, uint factionColor, uint otherNameColor, uint snameColor, double life, int pkMode, int pkValue, double currentMagic, int equipmentBody, int equipmentWeapon, Point coordinate, int direction, double lifeTotalWidth, double moveSpeed, int factionID, bool addToCanvas)
	{
		sprite.Name = name;
		sprite.RoleID = roleID;
		sprite.RoleSex = roleSex;
		sprite.FactionID = factionID;
		if (!string.IsNullOrEmpty(faction))
		{
			sprite.VFaction = faction;
		}
		if (!string.IsNullOrEmpty(otherName))
		{
			sprite.VOtherName = otherName;
		}
		sprite.VSName = sname;
		sprite.ShowName = Global.FormatShowName(Global.Data.roleData, 0);
		sprite.Occupation = occupation;
		sprite.ExtensionID = extensionID;
		sprite.SNameBrush = new SolidColorBrush(snameColor);
		sprite.VLife = life;
		sprite.PKMode = (GPKModes)pkMode;
		sprite.CurrentMagic = (int)currentMagic;
		sprite.EquipmentBody = equipmentBody;
		sprite.EquipmentWeapon = equipmentWeapon;
		sprite.cx = coordinate.X;
		sprite.cy = coordinate.Y;
		sprite.OrigCoordinate = coordinate;
		sprite.Direction = direction;
		sprite.LifeTotalWidth = lifeTotalWidth;
		sprite.SpriteMoveSpeed = moveSpeed;
		if (addToCanvas)
		{
			ObjectsManager.Add(sprite);
		}
		sprite.Action = GActions.Stand;
		return sprite;
	}

	public void NpcLoaderComplete(GSprite spr, MonsterNPCLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
		string[] array = go.name.Split(new char[]
		{
			'_'
		});
		if (array.Length == 5)
		{
			int npcID = Global.SafeConvertToInt32(array[2]);
			int num = Global.SafeConvertToInt32(array[3]);
			int missonType = Global.SafeConvertToInt32(array[4]);
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
			go.transform.localPosition = JingLingPos.taskpos(missonType, 0);
			go.transform.localEulerAngles = JingLingPos.taskpos(missonType, 1);
			go.transform.localScale = JingLingPos.taskpos(missonType, 2);
			JingLingMap.ReplaceMaterials(go, npcvobyID.ShaderID);
		}
		else
		{
			JingLingMap.ReplaceMaterials(go, -1);
		}
		if (go.name.StartsWith("jinglingmap_home"))
		{
			go.transform.localEulerAngles = new Vector3(0f, 45f, 0f);
			go.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
		}
		NPCInfoDisplay component = go.GetComponent<NPCInfoDisplay>();
		component.NPCNameText = string.Empty;
		component.enabled = false;
	}

	public static void ReplaceMaterials(GameObject go, int shaderID)
	{
		if (go == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"ReplaceMaterials对象为空,不应该为空"
			});
			return;
		}
		Renderer[] componentsInChildren;
		if (shaderID <= 0)
		{
			componentsInChildren = go.GetComponentsInChildren<Renderer>();
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
					for (int j = 0; j < sharedMaterials.Length; j++)
					{
						Texture texture = sharedMaterials[j].GetTexture("_MainTex");
						if (texture.name.IndexOf("_alpha") >= 0)
						{
							sharedMaterials[j].shader = Shader.Find("Custom/Mobile/Diffuse");
						}
						else
						{
							sharedMaterials[j].shader = Shader.Find(componentsInChildren[i].sharedMaterial.shader.name);
						}
					}
				}
			}
			return;
		}
		componentsInChildren = go.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				Renderer renderer = componentsInChildren[k];
				if (!(null == renderer))
				{
					if (!(renderer is ParticleRenderer) && !(renderer is ParticleSystemRenderer))
					{
						Material[] array = new Material[renderer.sharedMaterials.Length];
						for (int l = 0; l < renderer.sharedMaterials.Length; l++)
						{
							if (renderer.sharedMaterials[l].shader.name.StartsWith("Custom/Mobile/Particles") || renderer.sharedMaterials[l].shader.name.StartsWith("Artist/") || renderer.sharedMaterials[l].shader.name.StartsWith("Particles/") || renderer.sharedMaterials[l].shader.name.StartsWith("Mobile/Particles/"))
							{
								array[l] = renderer.sharedMaterials[l];
							}
							else
							{
								Texture texture2 = renderer.sharedMaterials[l].GetTexture("_MainTex");
								Material materialReflByShaderID = U3DUtils.GetMaterialReflByShaderID(renderer.sharedMaterials[l], shaderID, texture2.name.IndexOf("_alpha") >= 0);
								if (null == materialReflByShaderID)
								{
									array[l] = materialReflByShaderID;
								}
								else
								{
									materialReflByShaderID.SetTexture("_MainTex", texture2);
									array[l] = materialReflByShaderID;
								}
							}
						}
						componentsInChildren[k].materials = array;
					}
				}
			}
		}
	}

	public void MonsterLoaderComplete(GSprite spr, MonsterNPCLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
	}

	public void RoleLoaderComplete(GSprite spr, RoleLoaderData loader, GameObject go)
	{
		if (null == go)
		{
			return;
		}
	}

	private JingLingMap.JingLingMapType _showType = JingLingMap.JingLingMapType.MyHome;

	private static JingLingMap _map;

	public int JingLingFubenID = 84000;

	public SceneUIClasses JingLingFuBenClass = SceneUIClasses.JingLingFuBenClass;

	public JingLingMapHomeFace homeface;

	public JingLingMapBossFace bossface;

	public Dictionary<int, JingLingMapTaskFace> taskfaces = new Dictionary<int, JingLingMapTaskFace>();

	public List<YaoSaiMissionData> curMissionDataLst = new List<YaoSaiMissionData>();

	private KeyValuePair<bool, ArrayList> localEulerAnglesBak = new KeyValuePair<bool, ArrayList>(false, null);

	public DateTime FreeRefreshTime;

	public static bool debugShowBuild = true;

	public static bool debugShowShadow;

	public int ExcuteMissionCount;

	private int _curRoleID;

	private static SceneUIClasses preUIClass;

	private Dictionary<string, bool> pzChildDic = new Dictionary<string, bool>();

	private static DispatcherTimer UITimer;

	private YaoSaiBossMainData curYaoSaiBossMainData;

	public enum JingLingMapType
	{
		Nodata,
		MyHome,
		FriendHome,
		NuLiSearch
	}
}
