using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class BasePlayZone : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.Init();
	}

	public new void OnDestroy()
	{
		if (null != this.gameGuide)
		{
			Object.Destroy(this.gameGuide.gameObject);
			this.gameGuide = null;
		}
		base.OnDestroy();
	}

	public void Init()
	{
		this.Root = this;
		if (null == this.gameGuide)
		{
			this.gameGuide = U3DUtils.NEW<PlayGameGuide>();
			this.gameGuide.Visibility = false;
			this.gameGuide.StartTalk = new EventHandler(this.StartTalk);
			this.gameGuide.CompleteTalk = new EventHandler(this.CompleteTalk);
			Global.MainStage.Children.Add(this.gameGuide);
		}
		Super.GData.GlobalUIAudioSource = this._NetAudioSource;
	}

	protected long GetExperienceByLevel(List<XElement> levelList, int level)
	{
		for (int i = 0; i < levelList.Count; i++)
		{
			if (Global.GetXElementAttributeInt(levelList[i], "Level") == level)
			{
				return (long)Global.GetXElementAttributeInt(levelList[i], "Value");
			}
		}
		return long.MaxValue;
	}

	protected MapTeleports InitMapTeleports(int mapCode)
	{
		MapTeleports mapTeleports = new MapTeleports
		{
			MapID = mapCode,
			TeleportsList = new List<TeleportItem>()
		};
		string xmlName = StringUtil.substitute("teleports.xml", new object[0]);
		XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
		if (gameMapSettingsXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameMapSettingsXml, "Teleports"), "Teleport");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					TeleportItem teleportItem = new TeleportItem
					{
						TeleportKey = Global.GetXElementAttributeInt(xelement, "Key"),
						TeleportPos = new Point(Global.GetXElementAttributeInt(xelement, "X"), Global.GetXElementAttributeInt(xelement, "Y")),
						ToMapID = Global.GetXElementAttributeInt(xelement, "To"),
						ToMapPos = new Point(Global.GetXElementAttributeInt(xelement, "ToX"), Global.GetXElementAttributeInt(xelement, "ToY"))
					};
					mapTeleports.TeleportsList.Add(teleportItem);
				}
			}
		}
		return mapTeleports;
	}

	protected void InitMapTeleportsList()
	{
		Global.Data.MapTeleportsDict.Clear();
		foreach (KeyValuePair<int, SettingMapVO> keyValuePair in ConfigSettings.GetSettingsMapVODict())
		{
			int code = keyValuePair.Value.Code;
			MapTeleports mapTeleports = this.InitMapTeleports(code);
			if (mapTeleports != null)
			{
				if (!Global.Data.MapTeleportsDict.ContainsKey(code))
				{
					Global.Data.MapTeleportsDict.Add(code, mapTeleports);
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						code + "重复了的mapCode  "
					});
				}
			}
		}
	}

	protected void InitializeGameArguments()
	{
		SettingsSpeedConfig settingsSpeedConfig = ConfigSettings.GetSettingsSpeedConfig();
		Global.Data.WalkUnitCost = settingsSpeedConfig.WalkUnitCost;
		Global.Data.RunUnitCost = settingsSpeedConfig.RunUnitCost;
		string[] array = settingsSpeedConfig.Tick.Split(new char[]
		{
			','
		});
		Global.Data.SpeedTickList = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			Global.Data.SpeedTickList[i] = Convert.ToInt32(array[i]);
		}
		Global.Data.MaxAttackSlotTick = settingsSpeedConfig.MaxAttackSlotTick;
		SettingsDistanceConfig settingsDistanceConfig = ConfigSettings.GetSettingsDistanceConfig();
		Global.Data.WalkStepWidth = settingsDistanceConfig.WalkStepWidth;
		Global.Data.RunStepWidth = settingsDistanceConfig.RunStepWidth;
		Global.Data.MaxAttackDistance = settingsDistanceConfig.MaxAttackDistance;
		Global.Data.MinAttackDistance = settingsDistanceConfig.MinAttackDistance;
		Global.Data.MaxMagicDistance = settingsDistanceConfig.MaxMagicDistance;
		Global.Data.MaxUnWatchDistance = settingsDistanceConfig.MaxUnWatchDistance;
		SettingsSpriteConfig settingsSpriteConfig = ConfigSettings.GetSettingsSpriteConfig();
		Global.Data.LifeTotalWidth = settingsSpriteConfig.LifeTotalWidth;
		Global.Data.HoldWidth = settingsSpriteConfig.HoldWidth;
		Global.Data.HoldHeight = settingsSpriteConfig.HoldHeight;
		SettingsSpriteBrushes settingsSpriteBrushes = ConfigSettings.GetSettingsSpriteBrushes();
		string[] array2 = settingsSpriteBrushes.FactionBrushColor.Split(new char[]
		{
			','
		});
		Global.Data.FactionBrushColor = ColorSL.FromArgb(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]));
		array2 = settingsSpriteBrushes.ClanBrushColor.Split(new char[]
		{
			','
		});
		Global.Data.OtherNameBrushColor = ColorSL.FromArgb(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]));
		array2 = settingsSpriteBrushes.SnameBrushColor.Split(new char[]
		{
			','
		});
		Global.Data.SnameBrushColor = ColorSL.FromArgb(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]));
		Global.Data.FactionBrushColor = Global.Data.SnameBrushColor;
		array2 = settingsSpriteBrushes.LifeBrushColor.Split(new char[]
		{
			','
		});
		Global.Data.LifeBrushColor = ColorSL.FromArgb(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[3]));
		SettingsGoodsPack settingsGoodsPack = ConfigSettings.GetSettingsGoodsPack();
		Global.Data.GoodsPackOvertimeTick = settingsGoodsPack.MaxOvertimeTick;
		Global.Data.GoodsDestroytimeTick = settingsGoodsPack.PackDestroyTimeTick;
		SettingsAlive settingsAlive = ConfigSettings.GetSettingsAlive();
		Global.Data.AliveGoodsID = settingsAlive.GoodsID;
		Global.Data.AliveMaxLevel = settingsAlive.MaxLevel;
		Global.Data.TaskMaxFocusCount = ConfigSettings.GetSettingsTask().MaxFocusNum;
		Global.Data.ShowObstruction = ConfigSettings.ObstructionShow;
		Global.Data.ShowObsIndex = ConfigSettings.ObsZIndexShow;
		Global.Data.ShowCPUUsage = ConfigSettings.ShowCPUUsageShow;
		Global.Data.ShowFindWay = ConfigSettings.ShowFindWayShow;
		Global.Data.ShowPunish = ConfigSettings.PunishShow;
		Global.Data.ShowKeyPoints = ConfigSettings.KeyPointsShow;
		string xmlName = "Config/LevelUp.Xml";
		XElement gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Experience");
			int count = xelementList.Count;
			Global.Data.LevelUpExperienceList = new long[count];
			for (int j = 0; j < count; j++)
			{
				Global.Data.LevelUpExperienceList[j] = this.GetExperienceByLevel(xelementList, j);
			}
		}
		xmlName = Global.GetZhuanShengXmlFilePath();
		gameResXml = Global.GetGameResXml(xmlName);
		if (gameResXml != null)
		{
			List<XElement> xelementList2 = Global.GetXElementList(gameResXml, "ZhuanSheng");
			int count2 = xelementList2.Count;
			Global.Data.LevelUpExpProportionList = new Dictionary<int, double>();
			for (int k = 0; k < count2; k++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList2[k], "ChangeLifeID");
				double xelementAttributeDouble = Global.GetXElementAttributeDouble(xelementList2[k], "ExpProportion");
				Global.Data.LevelUpExpProportionList[xelementAttributeInt] = xelementAttributeDouble;
			}
		}
		else
		{
			Global.Data.LevelUpExpProportionList = new Dictionary<int, double>();
			MUDebug.LogError<string>(new string[]
			{
				"加载转生表失败"
			});
		}
		this.InitMapTeleportsList();
		ConfigNPCs.LoadNPCNamesDict();
		ConfigMonsters.LoadMonsterNamesDict();
		ConfigGoods.LoadGoodsNamesDict();
	}

	public void MapConversion(object sender, MapConversionByTeleportCodeEventArgs e)
	{
		if (Global.Data.WaitingForMapChange)
		{
			return;
		}
		Global.Data.WaitingForMapChange = true;
		Super.ShowNetWaiting(Global.GetLang("切换地图..."));
		this.scene.ToMapConversionByTeleportCode((int)e.Teleport.Key);
	}

	public void MapConversion(int toMapCode, int mapX, int mapY, int direction, int relife)
	{
		if (Global.Data.WaitingForMapChange)
		{
			return;
		}
		Super.HideNetWaiting();
		if (Global.IsFirstPopupDownloadMapWindowInWorldMap || Global.IsFuBenFenBaoMap)
		{
			Global.IsFirstPopupDownloadMapWindowInWorldMap = false;
			Global.IsFuBenFenBaoMap = false;
			Global.Data.WaitingForMapChange = true;
			Super.ShowNetWaiting(Global.GetLang("切换地图..."));
			this.scene.ToMapConversionByMapCode(toMapCode, mapX, mapY, direction, relife);
		}
		else if (Global.IsPopupDownloadMapWindow(toMapCode))
		{
			string[] buttons = new string[]
			{
				Global.GetLang("立即下载"),
				Global.GetLang("稍后下载")
			};
			string message = string.Format(Global.GetLang("当前需要下载场景资源【{0}KB】，是否立即下载？"), Global.GetFenBaoMapSize(toMapCode));
			Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s1, DPSelectedItemEventArgs e1)
			{
				if (e1.ID == 0)
				{
					Global.Data.WaitingForMapChange = true;
					Super.ShowNetWaiting(Global.GetLang("切换地图..."));
					this.scene.ToMapConversionByMapCode(toMapCode, mapX, mapY, direction, relife);
				}
				else
				{
					GameInstance.Game.SendCancelChangMap(toMapCode, mapX, mapY, direction, relife);
				}
			}, buttons);
		}
		else
		{
			Global.Data.WaitingForMapChange = true;
			Super.ShowNetWaiting(Global.GetLang("切换地图..."));
			this.scene.ToMapConversionByMapCode(toMapCode, mapX, mapY, direction, relife);
		}
	}

	protected virtual void ClearScene()
	{
		if (this.scene != null)
		{
			this.scene.ClearScene();
		}
	}

	public virtual void LoadScene(int mapCode, int leaderX, int leaderY, double direction, bool newLifeDeco)
	{
		int timer = U3DUtils.GetTimer();
		this.ClearScene();
		this.scene.LoadScene(mapCode, (double)leaderX, (double)leaderY, direction, newLifeDeco);
	}

	private IEnumerator StartTalkExe()
	{
		yield return 0;
		this.Root.Visibility = false;
		this.gameGuide.Visibility = true;
		NGUITools.SetActive(Global.Joystick.gameObject, false);
		NGUITools.SetActive(HUDTextRoot.go, false);
		yield break;
	}

	protected void StartTalk(object sender, EventArgs e)
	{
		if (NGUITools.GetActive(base.gameObject))
		{
			base.StartCoroutine<bool>(this.StartTalkExe());
		}
		else
		{
			this.Root.Visibility = false;
			this.gameGuide.Visibility = true;
			NGUITools.SetActive(Global.Joystick.gameObject, false);
			NGUITools.SetActive(HUDTextRoot.go, false);
		}
	}

	protected virtual void CompleteTalk(object sender, EventArgs e)
	{
		this.Root.Visibility = true;
		this.gameGuide.Visibility = false;
		NGUITools.SetActive(Global.Joystick.gameObject, true);
		NGUITools.SetActive(HUDTextRoot.go, true);
		this.gameGuide.ReturnCamera();
		if (this.gameGuide.ExtCallBack != null)
		{
			this.gameGuide.ExtCallBack(this.gameGuide, EventArgs.Empty);
		}
	}

	protected bool TriggerTaskPlot(SystemHelpModes mode, int timeParameters, ExtCallBackHandler extCallBack = null)
	{
		int taskPlotItemByMode = Global.GetTaskPlotItemByMode((int)mode, timeParameters);
		if (taskPlotItemByMode < 0)
		{
			return false;
		}
		this.gameGuide.TaskPlotID = taskPlotItemByMode;
		this.gameGuide.ExtCallBack = extCallBack;
		return true;
	}

	public bool TriggerTaskPlotByID(int taskPlotID)
	{
		this.gameGuide.TaskPlotID = taskPlotID;
		this.gameGuide.ExtCallBack = null;
		return true;
	}

	protected bool TriggerTaskPlot(int taskPlot)
	{
		this.StartTalk(null, null);
		this.gameGuide.TestCameraAnimation(0);
		return true;
	}

	protected int MouseLeaveTicks;

	protected RandomAS rand = new RandomAS(0);

	protected Canvas BottomMenu;

	protected GScene scene;

	protected Canvas Root;

	protected PlayGameGuide gameGuide;

	protected Point CurrentMousePos = new Point(0, 0);

	public NetAudioSource _NetAudioSource;

	public static bool IsSpeedCheck = true;

	public static int InWaitPingCount;

	public static int PING_TIMEOUT = 20000;

	public static bool alreadyRequestedNotice;

	public string showPages = "000";
}
