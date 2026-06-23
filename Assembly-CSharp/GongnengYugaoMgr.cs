using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class GongnengYugaoMgr : UserControl
{
	public static void RefreshAllIcons(bool reload = false)
	{
		if (reload)
		{
			GongnengYugaoMgr.IconsOpenDict.Clear();
			GongnengYugaoMgr.IconsOpenDictReal.Clear();
		}
		GongnengYugaoMgr.UpdateIcons(true);
		Global.GlobalEventDispatcher.dispatchEvent(new PlayGameContolEvent("EventMenuIconBoxAddIcon", 0));
	}

	public static int GetNewIconOnLevelUp(int oldLevel, int newLevel)
	{
		int result = -1;
		if (Global.Data.roleData.IsFlashPlayer != 0)
		{
			return -1;
		}
		int unionLevel;
		int unionLevel2;
		if (oldLevel < newLevel)
		{
			unionLevel = Global.GetUnionLevel(Global.Data.roleData.ChangeLifeCount, oldLevel);
			unionLevel2 = Global.GetUnionLevel(Global.Data.roleData.ChangeLifeCount, newLevel);
		}
		else
		{
			unionLevel = Global.GetUnionLevel(Global.Data.roleData.ChangeLifeCount - 1, oldLevel);
			unionLevel2 = Global.GetUnionLevel(Global.Data.roleData.ChangeLifeCount, newLevel);
		}
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		for (int i = 0; i < systemOpenVOList.Count; i++)
		{
			SystemOpenVO systemOpenVO = systemOpenVOList[i];
			if (GongnengYugaoMgr.ValidateOccupation(systemOpenVO.Occupation))
			{
				if (systemOpenVO.TriggerCondition == 1)
				{
					if (!GongnengYugaoMgr.IsVersionSystemOpen(systemOpenVO))
					{
						systemOpenVO.IsOpened = false;
					}
					else
					{
						int unionLevel3 = Global.GetUnionLevel(systemOpenVO.TimeParameters, systemOpenVO.TimeParameters2);
						if (unionLevel >= unionLevel3)
						{
							systemOpenVO.IsOpened = true;
							GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
						}
						else if (unionLevel3 <= unionLevel2)
						{
							if (systemOpenVO.Cartoon <= 0)
							{
								systemOpenVO.IsOpened = true;
							}
							else
							{
								GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
								if (systemOpenVO.SpecialOpenType == 0)
								{
									result = systemOpenVO.ID;
								}
							}
						}
						else if (unionLevel3 > unionLevel2)
						{
							break;
						}
					}
				}
			}
		}
		return result;
	}

	public static int GetNewIconOnCompleteTask(int CompletedMainTaskID)
	{
		int result = -1;
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		for (int i = 0; i < systemOpenVOList.Count; i++)
		{
			SystemOpenVO systemOpenVO = systemOpenVOList[i];
			if (GongnengYugaoMgr.ValidateOccupation(systemOpenVO.Occupation))
			{
				if (systemOpenVO.TriggerCondition == 7)
				{
					if (systemOpenVO.TimeParameters == CompletedMainTaskID)
					{
						if (!GongnengYugaoMgr.IsVersionSystemOpen(systemOpenVO))
						{
							systemOpenVO.IsOpened = false;
						}
						else if (systemOpenVO.Cartoon <= 0)
						{
							systemOpenVO.IsOpened = true;
						}
						else
						{
							GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
							if (systemOpenVO.SpecialOpenType == 0)
							{
								result = systemOpenVO.ID;
							}
						}
					}
					else if (systemOpenVO.TimeParameters > CompletedMainTaskID)
					{
						break;
					}
				}
			}
		}
		return result;
	}

	public static int GetNewIconOnChongShengYinJi(int chongShengLevel)
	{
		int result = -1;
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		for (int i = 0; i < systemOpenVOList.Count; i++)
		{
			SystemOpenVO systemOpenVO = systemOpenVOList[i];
			if (GongnengYugaoMgr.ValidateOccupation(systemOpenVO.Occupation))
			{
				if (systemOpenVO.TriggerCondition == 21)
				{
					if (GongnengYugaoMgr.IsVersionSystemOpen(systemOpenVO))
					{
						if (systemOpenVO.IsOpened)
						{
							return systemOpenVO.ID;
						}
						if (!systemOpenVO.IsOpened && chongShengLevel >= systemOpenVO.TimeParameters)
						{
							systemOpenVO.IsOpened = true;
							GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
							if (systemOpenVO.DongHua == 1)
							{
								GongnengYugaoMgr.ReadyFlyingImgAnimation(PlayZone.GlobalPlayZone, systemOpenVO.ID);
							}
							return systemOpenVO.ID;
						}
					}
				}
			}
		}
		return result;
	}

	public static int GetNewIconOnWingUp(int WingID)
	{
		int result = -1;
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		for (int i = 0; i < systemOpenVOList.Count; i++)
		{
			SystemOpenVO systemOpenVO = systemOpenVOList[i];
			if (GongnengYugaoMgr.ValidateOccupation(systemOpenVO.Occupation))
			{
				if (systemOpenVO.TriggerCondition == 14)
				{
					if (!GongnengYugaoMgr.IsVersionSystemOpen(systemOpenVO))
					{
						systemOpenVO.IsOpened = false;
					}
					else if (systemOpenVO.TimeParameters < WingID)
					{
						systemOpenVO.IsOpened = true;
						GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
					}
					else if (!systemOpenVO.IsOpened && systemOpenVO.TimeParameters == WingID)
					{
						systemOpenVO.IsOpened = true;
						GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
						return systemOpenVO.ID;
					}
				}
			}
		}
		return result;
	}

	private static bool IsVersionSystemOpen(SystemOpenVO item)
	{
		return !ConfigVersionSystemOpen.VersionSystemOpenHaveTheId(ConfigSystemOpen.GetVersionSystemOpenIDBySystemOpenID(item.ID), 1) || ConfigVersionSystemOpen.IsVersionSystemOpen(ConfigSystemOpen.GetVersionSystemOpenIDBySystemOpenID(item.ID));
	}

	public static void ShowAllIcon(Dictionary<int, bool> dict)
	{
		foreach (KeyValuePair<int, bool> keyValuePair in GongnengYugaoMgr.IconsOpenDict)
		{
			dict[keyValuePair.Key] = keyValuePair.Value;
			GongnengYugaoMgr.IconsOpenDictReal[keyValuePair.Key] = true;
			GongnengYugaoMgr.SetIcon(true, keyValuePair.Key);
		}
	}

	public static void RestoreOpenState(Dictionary<int, bool> dict)
	{
		foreach (KeyValuePair<int, bool> keyValuePair in dict)
		{
			GongnengYugaoMgr.IconsOpenDictReal[keyValuePair.Key] = dict[keyValuePair.Key];
		}
	}

	public static bool ValidateOccupation(int needOccupation)
	{
		return needOccupation < 0 || needOccupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
	}

	private static void UpDateHuiJiIcon()
	{
		bool active = false;
		if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.HuiJiHuTi) && Global.GetTerraceOpen("EmblemOpen"))
		{
			active = true;
			if (ConfigVersionSystemOpen.VersionSystemOpenHaveTheId(ConfigSystemOpen.GetVersionSystemOpenIDBySystemOpenID(89), 1))
			{
				active = ConfigVersionSystemOpen.IsVersionSystemOpen(ConfigSystemOpen.GetVersionSystemOpenIDBySystemOpenID(89));
			}
		}
		if (null != UICtrlBar.singleton.HuiJiIcon)
		{
			UICtrlBar.singleton.HuiJiIcon.gameObject.SetActive(active);
		}
	}

	public static void UpdateIcons(bool force = false)
	{
		int num = -1;
		int unionLevel = Global.GetUnionLevel(-1, -1);
		int completedMainTaskID = Global.Data.roleData.CompletedMainTaskID;
		int wingID = Global.Data.roleData.MyWingData.WingID;
		bool flag = false;
		bool flag2 = false;
		List<int> list = new List<int>();
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		for (int i = 0; i < systemOpenVOList.Count; i++)
		{
			SystemOpenVO systemOpenVO = systemOpenVOList[i];
			bool flag3 = false;
			if (GongnengYugaoMgr.ValidateOccupation(systemOpenVO.Occupation))
			{
				if (!systemOpenVO.IsOpened)
				{
					bool isOpened = false;
					if (systemOpenVO.TriggerCondition == 1)
					{
						int unionLevel2 = Global.GetUnionLevel(systemOpenVO.TimeParameters, systemOpenVO.TimeParameters2);
						if (unionLevel2 <= unionLevel)
						{
							isOpened = true;
						}
					}
					else if (systemOpenVO.TriggerCondition == 7)
					{
						if (systemOpenVO.TimeParameters <= completedMainTaskID)
						{
							isOpened = true;
						}
					}
					else if (systemOpenVO.TriggerCondition == 14)
					{
						if (systemOpenVO.TimeParameters <= wingID)
						{
							isOpened = true;
						}
					}
					else if (systemOpenVO.TriggerCondition == 15)
					{
						if (Global.GetChengJiuLevel(0) >= systemOpenVO.TimeParameters)
						{
							isOpened = true;
						}
					}
					else if (systemOpenVO.TriggerCondition == 16)
					{
						int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
						if (roleCommonUseParamsValue >= systemOpenVO.TimeParameters)
						{
							isOpened = true;
						}
					}
					else if (systemOpenVO.TriggerCondition == 21)
					{
						int rebornLevel = Global.Data.roleData.RebornLevel;
						if (rebornLevel >= systemOpenVO.TimeParameters)
						{
							isOpened = true;
						}
					}
					else if (systemOpenVO.TriggerCondition == 20)
					{
						int faction = Global.Data.roleData.Faction;
						if (faction >= systemOpenVO.TimeParameters)
						{
							isOpened = true;
						}
					}
					if (ConfigVersionSystemOpen.VersionSystemOpenHaveTheId(ConfigSystemOpen.GetVersionSystemOpenIDBySystemOpenID(systemOpenVO.ID), 1) && !ConfigVersionSystemOpen.IsVersionSystemOpen(ConfigSystemOpen.GetVersionSystemOpenIDBySystemOpenID(systemOpenVO.ID)))
					{
						isOpened = false;
					}
					systemOpenVO.IsOpened = isOpened;
					if (systemOpenVO.ID == 103 && !ConfigVersionSystemOpen.IsShenHunHuTiOpen())
					{
						systemOpenVO.IsOpened = false;
					}
				}
				if (!GongnengYugaoMgr.IconsOpenDict.TryGetValue(systemOpenVO.Order, ref flag3))
				{
					GongnengYugaoMgr.IconsOpenDictReal.Add(systemOpenVO.Order, systemOpenVO.IsOpened);
					GongnengYugaoMgr.IconsOpenDict.Add(systemOpenVO.Order, systemOpenVO.IsOpened);
				}
				else if (flag3)
				{
					goto IL_2DA;
				}
				if (systemOpenVO.IsOpened || systemOpenVO.Cartoon <= 0)
				{
					GongnengYugaoMgr.IconsOpenDict[systemOpenVO.Order] = true;
					GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
					list.Add(systemOpenVO.Order);
				}
				if (systemOpenVO.ID == 38)
				{
					flag = systemOpenVO.IsOpened;
				}
				if (systemOpenVO.ID == 17)
				{
					flag2 = systemOpenVO.IsOpened;
				}
			}
			IL_2DA:;
		}
		if (flag && PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameTaskBoxMini != null && null != PlayZone.GlobalPlayZone.GameTaskBoxMini && PlayZone.GlobalPlayZone.GameTaskBoxMini.gameObject.activeSelf)
		{
			PlayZone.GlobalPlayZone.GameTaskBoxMini.RefreshTasks(-1, -1, true);
		}
		if (flag2 && PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameTaskBoxMini != null && null != PlayZone.GlobalPlayZone.GameTaskBoxMini && PlayZone.GlobalPlayZone.GameTaskBoxMini.gameObject.activeSelf)
		{
			PlayZone.GlobalPlayZone.GameTaskBoxMini.RefreshTasks(-1, -1, true);
		}
		if (force)
		{
			foreach (KeyValuePair<int, bool> keyValuePair in GongnengYugaoMgr.IconsOpenDict)
			{
				GongnengYugaoMgr.SetIcon(keyValuePair.Value, keyValuePair.Key);
			}
		}
		else
		{
			list.Sort();
			foreach (int num2 in list)
			{
				if (num2 != num)
				{
					num = num2;
					GongnengYugaoMgr.SetIcon(true, num2);
				}
			}
		}
		GongnengYugaoMgr.UpDateHuiJiIcon();
	}

	public static bool IsShiLiOpen()
	{
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		SystemOpenVO systemOpenVO = systemOpenVOList.Find((SystemOpenVO info) => info.ID == 98);
		return systemOpenVO != null && GongnengYugaoMgr.IsOpenedByCondition(systemOpenVO);
	}

	public static bool IsOpenedByCondition(SystemOpenVO item)
	{
		int unionLevel = Global.GetUnionLevel(-1, -1);
		int completedMainTaskID = Global.Data.roleData.CompletedMainTaskID;
		int wingID = Global.Data.roleData.MyWingData.WingID;
		if (!GongnengYugaoMgr.ValidateOccupation(item.Occupation))
		{
			return false;
		}
		bool result = false;
		if (item.TriggerCondition == 1)
		{
			int unionLevel2 = Global.GetUnionLevel(item.TimeParameters, item.TimeParameters2);
			if (unionLevel2 <= unionLevel)
			{
				result = true;
			}
		}
		else if (item.TriggerCondition == 7)
		{
			if (item.TimeParameters <= completedMainTaskID)
			{
				result = true;
			}
		}
		else if (item.TriggerCondition == 14)
		{
			if (item.TimeParameters <= wingID)
			{
				result = true;
			}
		}
		else if (item.TriggerCondition == 15)
		{
			if (Global.GetChengJiuLevel(0) >= item.TimeParameters)
			{
				result = true;
			}
		}
		else if (item.TriggerCondition == 16)
		{
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
			if (roleCommonUseParamsValue >= item.TimeParameters)
			{
				result = true;
			}
		}
		return result;
	}

	public static bool IsGongNengOpened(GongNengIDs id)
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID((int)id);
		return systemOpenVOByID == null || systemOpenVOByID.IsOpened;
	}

	public static bool IsGongNengOpened(GongNengIDs id, ref GongNengYuGaoIconType type, ref string hintStr)
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID((int)id);
		if (systemOpenVOByID != null)
		{
			int notOpenShow = systemOpenVOByID.NotOpenShow;
			if (notOpenShow != 1)
			{
				if (notOpenShow != 2)
				{
					type = GongNengYuGaoIconType.None;
				}
				else
				{
					type = GongNengYuGaoIconType.GrayAndLock;
				}
			}
			else
			{
				type = GongNengYuGaoIconType.Hidden;
			}
			hintStr = UIHelper.HintGongNengOpenCondition(id, systemOpenVOByID.TriggerCondition, systemOpenVOByID.TimeParameters, systemOpenVOByID.TimeParameters2, false);
			return systemOpenVOByID.IsOpened;
		}
		return true;
	}

	public static string GetGongNengName(GongNengIDs id)
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID((int)id);
		if (systemOpenVOByID != null)
		{
			return systemOpenVOByID.Name;
		}
		return Global.GetLang("该");
	}

	public static bool IsGongNengOpened(GongNengIDs id, ref int trigger, ref int param1, ref int param2)
	{
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID((int)id);
		if (systemOpenVOByID != null)
		{
			trigger = systemOpenVOByID.TriggerCondition;
			param1 = systemOpenVOByID.TimeParameters;
			param2 = systemOpenVOByID.TimeParameters2;
			return systemOpenVOByID.IsOpened;
		}
		return true;
	}

	public static bool IsIconOpened(int order)
	{
		bool flag = false;
		if (GongnengYugaoMgr.IconsOpenDictReal.TryGetValue(order, ref flag))
		{
			if (flag && FashionWardrobePart.FashionWardrobeOrder == order)
			{
				flag = false;
				if (Global.Data.fashionAndTitleList != null)
				{
					int i = 0;
					int count = Global.Data.fashionAndTitleList.Count;
					while (i < count)
					{
						if (Global.Data.fashionAndTitleList[i] != null)
						{
							int categoriyByGoodsID = Global.GetCategoriyByGoodsID(Global.Data.fashionAndTitleList[i].GoodsID);
							if (categoriyByGoodsID == 24 || categoriyByGoodsID == 26 || categoriyByGoodsID == 25 || categoriyByGoodsID == 8)
							{
								return true;
							}
						}
						i++;
					}
				}
			}
			return flag;
		}
		return true;
	}

	public void SetItemByLevel(bool init = false)
	{
		int maxActiveSystemIndex = Global.GetMaxActiveSystemIndex();
		int level = Global.Data.roleData.Level;
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(maxActiveSystemIndex + 1);
		GongnengYugaoMgr.obj = systemOpenVOByID;
		if (maxActiveSystemIndex != GongnengYugaoMgr.currentIndex && maxActiveSystemIndex >= 5)
		{
			return;
		}
		this.DPSelectedItem(null, new DPSelectedItemEventArgs
		{
			ID = 1,
			Data = GongnengYugaoMgr.obj
		});
	}

	public static void AddIcon(int id, Component btn)
	{
		if (null != btn)
		{
			GongnengYugaoMgr.IconsDict[id] = btn;
		}
		else
		{
			GongnengYugaoMgr.IconsDict.Remove(id);
		}
	}

	public static void SetIcon(bool visible, int id = -1)
	{
		if (id == 1)
		{
		}
		if (id <= 2)
		{
			return;
		}
		if (id == 14 && Global.IsXinFuActivityEnd())
		{
			return;
		}
		if (Global.Data.roleData.IsFlashPlayer == 0 && id >= 30 && id <= 33)
		{
			return;
		}
		if (id == 41)
		{
			return;
		}
		if (id == 63 && Global.IsFundMergiWindow())
		{
			return;
		}
		if (id == 65)
		{
			return;
		}
		if (id == 20000)
		{
			return;
		}
		Component comp = null;
		if (id == -1)
		{
			int maxActiveSystemIndex = Global.GetMaxActiveSystemIndex();
			for (int i = maxActiveSystemIndex + 1; i <= GongnengYugaoMgr.IconsDict.Count; i++)
			{
				if (GongnengYugaoMgr.IconsDict.TryGetValue(id, ref comp))
				{
					GongnengYugaoMgr.SetCompVisibility(id, comp, visible);
				}
			}
		}
		else if (GongnengYugaoMgr.IconsDict.TryGetValue(id, ref comp))
		{
			GongnengYugaoMgr.SetCompVisibility(id, comp, visible);
		}
	}

	public static void SetCompVisibility(int id, Component comp, bool visible)
	{
		if (id == 40)
		{
			visible &= !Global.IsInSafeRegion;
		}
		if (null != comp)
		{
			comp.gameObject.SetActive(visible);
			if (visible && comp is UIButton)
			{
				(comp as UIButton).isEnabled = visible;
			}
		}
	}

	public static SystemOpenVO GetMinTaskIdByOrder(GongNengOrders order)
	{
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		if (systemOpenVOList != null)
		{
			List<SystemOpenVO> list = new List<SystemOpenVO>();
			for (int i = 0; i < systemOpenVOList.Count; i++)
			{
				if (systemOpenVOList[i].Order == (int)order && systemOpenVOList[i].TriggerCondition == 7)
				{
					list.Add(systemOpenVOList[i]);
				}
			}
			if (list.Count > 0)
			{
				int timeParameters = list[0].TimeParameters;
				int num = 0;
				for (int j = 0; j < list.Count; j++)
				{
					if (list[j].TimeParameters < timeParameters)
					{
						timeParameters = list[j].TimeParameters;
						num = j;
					}
				}
				return list[num];
			}
		}
		return null;
	}

	public static void SetCompLockState(int id, Component comp, bool visible, string hint = "")
	{
		if (null != comp && id != 40)
		{
			GButton btn = comp.GetComponent<GButton>();
			if (btn == null)
			{
				UIButton component = comp.GetComponent<UIButton>();
				if (component != null)
				{
					if (component && !component.gameObject.activeSelf)
					{
						component.gameObject.SetActive(true);
					}
					if (visible)
					{
						if (component && component.tweenTarget)
						{
							Transform transform = component.transform.FindChild("Animation/Background");
							if (transform)
							{
								UISprite component2 = transform.GetComponent<UISprite>();
								if (component2)
								{
									transform.gameObject.SetActive(false);
								}
								GameObject gameObject = Object.Instantiate<GameObject>(component2.gameObject);
								gameObject.transform.parent = component2.transform.parent;
								gameObject.transform.localPosition = component2.transform.localPosition;
								gameObject.transform.localScale = Vector3.one;
								gameObject.name = "UISpriteClone";
								UISprite component3 = gameObject.GetComponent<UISprite>();
								component3.spriteName = "xian";
								BoxCollider component4 = comp.GetComponent<BoxCollider>();
								if (component4)
								{
									component4.enabled = false;
									BoxCollider boxCollider = component3.gameObject.AddComponent<BoxCollider>();
									boxCollider.size = component4.size;
									boxCollider.center = component4.center - component3.gameObject.transform.localPosition;
									component3.gameObject.AddComponent<UIEventListener>();
									UIEventListener.Get(component3.gameObject).onClick = delegate(GameObject s)
									{
										Super.HintMainText(Global.GetLang("暂时不能用"), 10, 3);
									};
								}
							}
						}
					}
					else
					{
						if (component && component.tweenTarget)
						{
							Transform transform2 = component.transform.FindChild("Animation/Background");
							if (transform2)
							{
								UISprite component5 = transform2.GetComponent<UISprite>();
								if (component5)
								{
									transform2.gameObject.SetActive(false);
								}
								GameObject gameObject2 = Object.Instantiate<GameObject>(component5.gameObject);
								gameObject2.SetActive(true);
								gameObject2.transform.parent = component5.transform.parent;
								gameObject2.transform.localPosition = component5.transform.localPosition;
								gameObject2.transform.localScale = transform2.transform.localScale;
								gameObject2.name = "UISpriteClone";
								UISprite component6 = gameObject2.GetComponent<UISprite>();
								component6.spriteName = "xian";
								component6.MakePixelPerfect();
								BoxCollider component7 = comp.GetComponent<BoxCollider>();
								if (component7)
								{
									component7.enabled = false;
									BoxCollider boxCollider2 = gameObject2.AddComponent<BoxCollider>();
									boxCollider2.size = Vector3.one;
									boxCollider2.center = Vector3.zero;
									gameObject2.AddComponent<UIEventListener>();
									UIEventListener.Get(gameObject2).onClick = delegate(GameObject s)
									{
										Super.HintMainText(Global.GetLang("暂时不能用111"), 10, 3);
									};
								}
							}
						}
						if (btn && btn.BtnState)
						{
							Transform transform3 = btn.transform.FindChild("Icon");
							if (transform3)
							{
								UISprite component8 = transform3.GetComponent<UISprite>();
								if (component8)
								{
									transform3.gameObject.SetActive(false);
								}
							}
							btn.BtnState.SetActive(true);
							BoxCollider component9 = comp.GetComponent<BoxCollider>();
							if (component9)
							{
								component9.enabled = false;
								BoxCollider boxCollider3 = btn.BtnState.AddComponent<BoxCollider>();
								boxCollider3.size = component9.size;
								boxCollider3.center = component9.center - btn.BtnState.transform.localPosition;
								btn.BtnTag = hint;
								UIEventListener.Get(btn.BtnState).onClick = delegate(GameObject s)
								{
									Super.HintMainText(btn.BtnTag, 10, 3);
								};
							}
						}
						if (btn && btn.Label)
						{
							btn.Label.gameObject.SetActive(false);
							GameObject gameObject3 = Object.Instantiate<GameObject>(btn.Label.gameObject);
							gameObject3.transform.parent = btn.Label.transform.parent;
							gameObject3.transform.localPosition = btn.Label.transform.localPosition;
							gameObject3.transform.localScale = Vector3.one;
							gameObject3.name = "LabelClone";
							UILabel component10 = gameObject3.GetComponent<UILabel>();
							component10.gameObject.SetActive(true);
						}
						if (btn)
						{
							Animation[] componentsInChildren = btn.GetComponentsInChildren<Animation>();
							if (componentsInChildren != null)
							{
								for (int i = 0; i < componentsInChildren.Length; i++)
								{
									if (componentsInChildren[i].gameObject != btn.gameObject)
									{
										componentsInChildren[i].gameObject.SetActive(false);
									}
								}
							}
						}
					}
				}
			}
			if (btn && !btn.gameObject.activeSelf)
			{
				btn.gameObject.SetActive(true);
			}
			if (visible)
			{
				if (btn && btn.BtnState)
				{
					Transform transform4 = btn.transform.FindChild("Icon");
					if (transform4)
					{
						UISprite component11 = transform4.GetComponent<UISprite>();
						if (component11)
						{
							transform4.gameObject.SetActive(false);
						}
					}
					btn.BtnState.SetActive(false);
					BoxCollider component12 = comp.GetComponent<BoxCollider>();
					if (component12)
					{
						component12.enabled = true;
					}
				}
				if (btn && btn.Label)
				{
					UILabel[] componentsInChildren2 = btn.GetComponentsInChildren<UILabel>();
					if (componentsInChildren2 != null)
					{
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							if (componentsInChildren2[j].name == "LabelClone")
							{
								btn.Label.gameObject.SetActive(true);
								Object.Destroy(componentsInChildren2[j].gameObject);
								break;
							}
						}
					}
				}
			}
			else
			{
				if (btn && btn.BtnState)
				{
					Transform transform5 = btn.transform.FindChild("Icon");
					if (transform5)
					{
						UISprite component13 = transform5.GetComponent<UISprite>();
						if (component13)
						{
							transform5.gameObject.SetActive(false);
						}
					}
					btn.BtnState.SetActive(true);
					BoxCollider component14 = comp.GetComponent<BoxCollider>();
					if (component14)
					{
						component14.enabled = false;
						BoxCollider boxCollider4 = btn.BtnState.AddComponent<BoxCollider>();
						boxCollider4.size = component14.size;
						boxCollider4.center = component14.center - btn.BtnState.transform.localPosition;
						btn.BtnTag = hint;
						UIEventListener.Get(btn.BtnState).onClick = delegate(GameObject s)
						{
							Super.HintMainText(btn.BtnTag, 10, 3);
						};
					}
				}
				if (btn && btn.Label)
				{
					btn.Label.gameObject.SetActive(false);
					GameObject gameObject4 = Object.Instantiate<GameObject>(btn.Label.gameObject);
					gameObject4.transform.parent = btn.Label.transform.parent;
					gameObject4.transform.localPosition = btn.Label.transform.localPosition;
					gameObject4.transform.localScale = Vector3.one;
					gameObject4.name = "LabelClone";
					UILabel component15 = gameObject4.GetComponent<UILabel>();
					component15.gameObject.SetActive(true);
				}
				if (btn)
				{
					Animation[] componentsInChildren3 = btn.GetComponentsInChildren<Animation>();
					if (componentsInChildren3 != null)
					{
						for (int k = 0; k < componentsInChildren3.Length; k++)
						{
							if (componentsInChildren3[k].gameObject != btn.gameObject)
							{
								componentsInChildren3[k].gameObject.SetActive(false);
							}
						}
					}
				}
			}
		}
	}

	public static Component GetIconByID(int id)
	{
		return GongnengYugaoMgr.IconsDict[id];
	}

	public static bool ReadyFlyingImgAnimation(Canvas root, int id)
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		if (GongnengYugaoMgr.Start > 0L && GongnengYugaoMgr.Start < correctLocalTime + 30000L)
		{
			return false;
		}
		GongnengYugaoMgr.Start = correctLocalTime;
		SystemOpenVO systemOpenVOByID = ConfigSystemOpen.GetSystemOpenVOByID(id);
		if (systemOpenVOByID == null || systemOpenVOByID.DongHua == -1)
		{
			Global.Data.WaitingForSystemHelp = false;
			GongnengYugaoMgr.Start = 0L;
			GongnengYugaoMgr.UpdateIcons(false);
			if (id == 32)
			{
				GameInstance.Game.SpriteQueryStarConstelltionInfoCmd();
			}
			if (id == 37)
			{
				Global.ShowZhuangBeiZhuiJiaRedPoint();
			}
			if (id == 99)
			{
				Global.ShowZhuangBeiPeiYangRedPoint();
			}
			if (id == 6)
			{
				Global.ShowZhuangBeiQiangHuaRedPoint();
			}
			return false;
		}
		Component iconByID = GongnengYugaoMgr.GetIconByID(systemOpenVOByID.Order);
		GongnengYugaoMgr.img = U3DUtils.NEW<GFyingImageEx>();
		if (null != GongnengYugaoMgr.img)
		{
			GongnengYugaoMgr.img.ID = systemOpenVOByID.ID;
			GongnengYugaoMgr.img.GongNengID = systemOpenVOByID.Order;
			GongnengYugaoMgr.img.PostWizardID = systemOpenVOByID.PostWizardID;
			GongnengYugaoMgr.img.Music = systemOpenVOByID.Music;
			root.Children.Add(GongnengYugaoMgr.img);
			GongnengYugaoMgr.img.DPSelectItem = new EventHandler(GongnengYugaoMgr.OnFlyFinish);
			GongnengYugaoMgr.img.Target = iconByID;
			GongnengYugaoMgr.img._Text.Text = systemOpenVOByID.Description;
			GongnengYugaoMgr.img.ImageOne = systemOpenVOByID.ImageOne;
			GongnengYugaoMgr.img.ImageTwo = systemOpenVOByID.ImageTwo;
			GongnengYugaoMgr.img.IconID = systemOpenVOByID.Order;
			GongnengYugaoMgr.img.InitPart();
		}
		WindowManage.AddWindows(GongnengYugaoMgr.img, false, null);
		return true;
	}

	public static void OnFlyFinish(object sender, EventArgs e)
	{
		int postWizardID = (sender as GFyingImageEx).PostWizardID;
		GongnengYugaoMgr.UpdateIcons(false);
		if ((sender as GFyingImageEx).ID == 32)
		{
			GameInstance.Game.SpriteQueryStarConstelltionInfoCmd();
		}
		if ((sender as GFyingImageEx).ID == 37)
		{
			Global.ShowZhuangBeiZhuiJiaRedPoint();
		}
		if ((sender as GFyingImageEx).ID == 99)
		{
			Global.ShowZhuangBeiPeiYangRedPoint();
		}
		if ((sender as GFyingImageEx).ID == 6)
		{
			Global.ShowZhuangBeiQiangHuaRedPoint();
		}
		if ((sender as GFyingImageEx).ID == 103)
		{
			UICtrlBar.singleton.RefershBianShenButton();
		}
		if (null != GongnengYugaoMgr.img)
		{
			WindowManage.RemoveWindows(GongnengYugaoMgr.img);
			Object.Destroy(GongnengYugaoMgr.img.gameObject);
			GongnengYugaoMgr.img = null;
		}
		GongnengYugaoMgr.Start = 0L;
		Global.Data.WaitingForSystemHelp = false;
		if (postWizardID > 0)
		{
			SystemHelpMgr.ShowHint(postWizardID, 1);
		}
	}

	private static void FlyingImgAnimation(GFyingImageEx img, Point start, Point end, int id)
	{
		img.X = (double)start.X;
		img.Y = (double)start.Y;
	}

	public static int GetNewIconOnShengWangLevelUp(int shengWangLevel)
	{
		int result = -1;
		List<SystemOpenVO> systemOpenVOList = ConfigSystemOpen.GetSystemOpenVOList();
		for (int i = 0; i < systemOpenVOList.Count; i++)
		{
			SystemOpenVO systemOpenVO = systemOpenVOList[i];
			if (GongnengYugaoMgr.ValidateOccupation(systemOpenVO.Occupation))
			{
				if (systemOpenVO.TriggerCondition == 16)
				{
					if (systemOpenVO.TimeParameters < shengWangLevel)
					{
						systemOpenVO.IsOpened = true;
						GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
					}
					else if (!systemOpenVO.IsOpened && systemOpenVO.TimeParameters == shengWangLevel)
					{
						systemOpenVO.IsOpened = true;
						GongnengYugaoMgr.IconsOpenDictReal[systemOpenVO.Order] = true;
						return systemOpenVO.ID;
					}
				}
			}
		}
		return result;
	}

	public const float ConstIconFlyTime = 0.75f;

	private const int MaxIndex = 5;

	public DPSelectedItemEventHandler DPSelectedItem;

	private new static long Start = 0L;

	private static SystemOpenVO obj = null;

	private static int currentIndex = -1;

	private static Dictionary<int, Component> IconsDict = new Dictionary<int, Component>();

	private static Dictionary<int, bool> IconsOpenDict = new Dictionary<int, bool>();

	private static Dictionary<int, bool> IconsOpenDictReal = new Dictionary<int, bool>();

	public static GFyingImageEx img = null;
}
