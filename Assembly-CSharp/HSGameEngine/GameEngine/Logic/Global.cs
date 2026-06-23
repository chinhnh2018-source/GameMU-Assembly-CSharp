using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.GoodsPack;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameEngine.Teleport;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Server.Tools.AStarEx;
using Tmsk.Xml;
using Umeng;
using UnityEngine;
using XMLCreater;

namespace HSGameEngine.GameEngine.Logic
{
	public static class Global
	{
		public static bool SyscTimeCheck(long nowTicks, long oldTicks)
		{
			long num = (nowTicks - oldTicks) / 2L / 10000L;
			long num2 = (long)((int)ConfigSystemParam.GetSystemParamIntByName("PingSyncTimeMSMax"));
			if (num2 <= 100L)
			{
				num2 = 2000L;
			}
			long num3 = (long)((int)ConfigSystemParam.GetSystemParamIntByName("PingSyncTimeMSCout"));
			if (num3 <= 1L)
			{
				num3 = 5L;
			}
			if (num > num2 && (long)Global.SyscTimeCout < num3)
			{
				Global.SyscTimeCout++;
				bool flag = GameInstance.Game.SendTimeSynchronization();
				if (flag)
				{
					return false;
				}
			}
			Global.SyscTimeCout = 0;
			return true;
		}

		public static EmptePart GetNewEmptyPart()
		{
			if (Global.EmptePartInstance == null)
			{
				Global.EmptePartInstance = U3DUtils.NEW<EmptePart>();
				if (MainGame._current != null)
				{
					U3DUtils.AddChild(MainGame._current.DialogLayer.gameObject, Global.EmptePartInstance.gameObject, false);
				}
				Global.EmptePartInstance.transform.localPosition = new Vector3(-22500f, -22500f, 0f);
			}
			return SpawnManager.Instantiate(Global.EmptePartInstance) as EmptePart;
		}

		public static GGoodIcon GetNewGoodIcon()
		{
			if (Global.GGoodIconInstance == null)
			{
				Global.GGoodIconInstance = U3DUtils.NEW<GGoodIcon>();
				if (MainGame._current != null)
				{
					U3DUtils.AddChild(MainGame._current.DialogLayer.gameObject, Global.GGoodIconInstance.gameObject, false);
				}
				Global.GGoodIconInstance.transform.localPosition = new Vector3(-22500f, -22500f, 0f);
			}
			return SpawnManager.Instantiate(Global.GGoodIconInstance) as GGoodIcon;
		}

		public static int GameLang
		{
			get
			{
				return Context.GameLang;
			}
		}

		public static string MainExeVer
		{
			get
			{
				return Context.MainExeVer;
			}
		}

		public static string ResSwfVer
		{
			get
			{
				return Context.ResSwfVer;
			}
		}

		public static string IsolateResID
		{
			get
			{
				return Context.IsolateResID;
			}
		}

		public static string PingTaiName
		{
			get
			{
				return Context.PingTaiName;
			}
		}

		public static string XapAbsoluteWebPath
		{
			get
			{
				return Context.XapAbsoluteWebPath;
			}
		}

		public static string WatchSprite
		{
			get
			{
				return Global.Data.WatchSprite;
			}
			set
			{
				Global.Data.WatchSprite = value;
			}
		}

		public static string ViewSprite
		{
			get
			{
				return Global.Data.ViewSprite;
			}
			set
			{
				Global.Data.ViewSprite = value;
			}
		}

		public static void AddMenuIconBoxMap(Component _transform)
		{
			if (_transform != null)
			{
				if (!Global.m_MenuIconBoxMapTransform.ContainsKey(_transform.GetInstanceID()))
				{
					Global.m_MenuIconBoxMapTransform.Add(_transform.GetInstanceID(), _transform.transform);
				}
				else
				{
					Global.m_MenuIconBoxMapTransform[_transform.GetInstanceID()] = _transform.transform;
				}
				if (!Global.m_MenuIconBoxMapVector3.ContainsKey(_transform.GetInstanceID()))
				{
					Global.m_MenuIconBoxMapVector3.Add(_transform.GetInstanceID(), _transform.transform.localPosition);
				}
				else
				{
					Global.m_MenuIconBoxMapVector3[_transform.GetInstanceID()] = _transform.transform.localPosition;
				}
			}
		}

		public static void SetMenuIconBoxMapPosition(bool flag)
		{
			Dictionary<int, Transform>.Enumerator enumerator = Global.m_MenuIconBoxMapTransform.GetEnumerator();
			List<int> list = new List<int>();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Transform> keyValuePair = enumerator.Current;
				Transform value = keyValuePair.Value;
				if (value != null)
				{
					if (flag)
					{
						Dictionary<int, Vector3> menuIconBoxMapVector = Global.m_MenuIconBoxMapVector3;
						KeyValuePair<int, Transform> keyValuePair2 = enumerator.Current;
						if (menuIconBoxMapVector.ContainsKey(keyValuePair2.Key))
						{
							value.localScale = new Vector3(1f, 1f, 1f);
						}
					}
					else
					{
						value.localScale = new Vector3(0f, 1f, 1f);
					}
				}
				else
				{
					List<int> list2 = list;
					KeyValuePair<int, Transform> keyValuePair3 = enumerator.Current;
					list2.Add(keyValuePair3.Key);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (Global.m_MenuIconBoxMapVector3.ContainsKey(list[i]))
				{
					Global.m_MenuIconBoxMapVector3.Remove(list[i]);
				}
				if (Global.m_MenuIconBoxMapTransform.ContainsKey(list[i]))
				{
					Global.m_MenuIconBoxMapTransform.Remove(list[i]);
				}
			}
		}

		public static string GetGameResImageURL(string uri)
		{
			return Global.WebPath("NetImages/GameRes/" + uri);
		}

		public static string GetGameResImageString(string uri)
		{
			return "NetImages/GameRes/Images/Plate/" + uri;
		}

		public static string GetGameResTaskZhangJieTitle(int zhangJieID)
		{
			return string.Format("NetImages/GameRes/Images/TaskZhangJie/{0}.png", zhangJieID);
		}

		public static string GetGameResInteractionString(string uri)
		{
			return "NetImages/GameRes/Images/Interaction/" + uri;
		}

		public static string GetAudioYinDaoString(string uri)
		{
			return "Audio/YinDao/" + uri;
		}

		public static string GetGameResHybridString(string uri)
		{
			return "NetImages/GameRes/Images/Hybrid/" + uri;
		}

		public static string GetZhuanShengImageString(string uri)
		{
			return string.Format("NetImages/Zhuan/{0}.png", uri);
		}

		public static string GetZhuanZhiImageString(string uri)
		{
			return string.Format("NetImages/Zhuan/{0}.png", uri);
		}

		public static string GetBossImageString(int monsterID)
		{
			return string.Format("NetImages/Boss/{0}.png", monsterID);
		}

		public static string GetNPCImageString(int npcID)
		{
			return string.Format("NetImages/NPCs/{0:000}.png", npcID);
		}

		public static string GetBossIconString(int picCode)
		{
			return string.Format("NetImages/Monsters/{0:000}.png", picCode);
		}

		public static string GetPrefabString(string score, bool isHasFlag = true)
		{
			if (score.EndsWith("_atlas") || !isHasFlag)
			{
				return string.Format("Prefabs/Atlas/{0}", score);
			}
			return string.Format("Prefabs/Atlas/{0}_atlas", score);
		}

		public static string GetSkillIconString(int iconID)
		{
			return string.Format("NetImages/GameRes/Images/Skill/{0}.png", iconID);
		}

		public static string GetSkillBuffIconString(int iconID)
		{
			return string.Format("NetImages/GameRes/Images/SkillBuf/{0}.png", iconID);
		}

		public static string GetGoodsIconString(int iconID)
		{
			return string.Format("NetImages/GameRes/Images/Goods/{0}.png", iconID);
		}

		public static BitmapData GetGameResImage(string uri)
		{
			uri = Global.GetGameResImageURL(uri);
			BitmapData bitmapData = new BitmapData(0.0, 0.0, true, uint.MaxValue);
			Downloader downloader = new Downloader(bitmapData);
			downloader.GetResourceByVer(uri, Global.ResSwfVer, false);
			return bitmapData;
		}

		public static BitmapData GetLoginResImage(string uri)
		{
			return null;
		}

		public static BitmapData GetIsolateResImage(string uri)
		{
			return null;
		}

		public static string ChatVoiceServerURL
		{
			get
			{
				return Global.m_ChatVoiceServerURL;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_ChatVoiceServerURL = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_ChatVoiceServerURL = value;
				}
				else
				{
					Global.m_ChatVoiceServerURL = "http://" + value;
				}
			}
		}

		public static string PushServerURL
		{
			get
			{
				return Global.m_PushServerURL;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_PushServerURL = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_PushServerURL = value;
				}
				else
				{
					Global.m_PushServerURL = "http://" + value;
				}
			}
		}

		public static string AdServerUrl
		{
			get
			{
				return Global.m_AdServerUrl;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_AdServerUrl = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_AdServerUrl = value;
				}
				else
				{
					Global.m_AdServerUrl = "http://" + value;
				}
			}
		}

		public static string VerifyAccountServerURL
		{
			get
			{
				return Global.m_strVerifyAccountServerURL;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_strVerifyAccountServerURL = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_strVerifyAccountServerURL = value;
				}
				else
				{
					Global.m_strVerifyAccountServerURL = "http://" + value;
				}
			}
		}

		public static string PayServerURL
		{
			get
			{
				return Global.m_strPayServerURL;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_strPayServerURL = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_strPayServerURL = value;
				}
				else
				{
					Global.m_strPayServerURL = "http://" + value;
				}
			}
		}

		public static string ServerListURL
		{
			get
			{
				return Global.m_ServerListURL;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_ServerListURL = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_ServerListURL = value;
				}
				else
				{
					Global.m_ServerListURL = "http://" + value;
				}
			}
		}

		public static string ServerListURLSecond
		{
			get
			{
				return Global.m_ServerListURLSecond;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_ServerListURLSecond = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_ServerListURLSecond = value;
				}
				else
				{
					Global.m_ServerListURLSecond = "http://" + value;
				}
			}
		}

		public static string ServerListCrossPlatfomURL
		{
			get
			{
				return Global.m_ServerListCrossPlatfomURL;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_ServerListCrossPlatfomURL = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_ServerListCrossPlatfomURL = value;
				}
				else
				{
					Global.m_ServerListCrossPlatfomURL = "http://" + value;
				}
			}
		}

		public static string AppealURL
		{
			get
			{
				return Global.m_appealUrl;
			}
			set
			{
				if (value.StartsWith("http://"))
				{
					Global.m_appealUrl = value;
				}
				else if (value.StartsWith("https://"))
				{
					Global.m_appealUrl = value;
				}
				else
				{
					Global.m_appealUrl = "http://" + value;
				}
			}
		}

		public static string GetLoginMode()
		{
			return Super.GetXapParamByName("login", "0");
		}

		public static AssetBundle CurrentMapLoader
		{
			get
			{
				return AssetBundleManager.CurrentMapLoader;
			}
			set
			{
				AssetBundleManager.CurrentMapLoader = value;
			}
		}

		public static void ClearAssetBundle()
		{
			AssetBundleManager.ClearAssetBundleM();
		}

		public static AssetBundle CurrentMapSettingsLoader
		{
			get
			{
				return AssetBundleManager.CurrentMapSettingsLoader;
			}
			set
			{
				AssetBundleManager.CurrentMapSettingsLoader = value;
			}
		}

		public static AssetBundle CurrentTerrainLoader
		{
			get
			{
				return AssetBundleManager.CurrentTerrainLoader;
			}
			set
			{
				AssetBundleManager.CurrentTerrainLoader = value;
			}
		}

		public static string ProjectPath()
		{
			return PathUtils.ProjectPath();
		}

		public static string GameResPath(string path)
		{
			return PathUtils.GameResPath(path);
		}

		public static string LoginResPath(string path)
		{
			return PathUtils.LoginResPath(path);
		}

		public static string IsolateResPath(string path)
		{
			return PathUtils.IsolateResPath(path);
		}

		public static string WebPath(string uri)
		{
			return PathUtils.WebPath(uri);
		}

		public static XElement GetGameResXml(string xmlName)
		{
			return XmlManager.GetGameResXml(xmlName);
		}

		public static XElement GetLoginResXml(string xmlName)
		{
			return XmlManager.GetLoginResXml(xmlName);
		}

		public static XElement GetIsolateResXml(string xmlName)
		{
			return XmlManager.GetIsolateResXml(xmlName);
		}

		public static XElement GetLangResXml(string xmlName)
		{
			return XmlManager.GetLangResXml(xmlName);
		}

		public static XElement GetGameMapXml(int mapPicCode, string xmlName)
		{
			return XmlManager.GetGameMapXml(mapPicCode, xmlName);
		}

		public static XElement GetGameMapSettingsXml(int mapCode, string xmlName)
		{
			return XmlManager.GetGameMapSettingsXml(mapCode, xmlName);
		}

		public static void AddXElement(string key, XElement element)
		{
			XmlManager.AddXElement(key, element);
		}

		public static XElement GetCachedXElement(string key)
		{
			return XmlManager.GetXElement(key);
		}

		public static string GetXElementNodePath(XElement element)
		{
			return XmlManager.GetXElementNodePath(element);
		}

		public static XElement GetXElement(XElement XElement, string newroot)
		{
			return XmlManager.GetXElement(XElement, newroot);
		}

		public static XElement GetXElement(XElement XElement, string newroot, string attribute, string value)
		{
			return XmlManager.GetXElement(XElement, newroot, attribute, value);
		}

		public static List<int> GetXElementAttributeIntList(XElement xElement, string attributeName, string newroot = "*")
		{
			return XmlManager.GetXElementAttributeIntList(xElement, attributeName, newroot);
		}

		public static string[] GetXElementAttributeStrArray(XElement xElement, string attributeName, string newroot = "*", char spliteChar = ',')
		{
			string xelementAttributeStr = XmlManager.GetXElementAttributeStr(xElement, attributeName);
			string[] result;
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				result = new string[0];
			}
			else
			{
				result = xelementAttributeStr.Split(new char[]
				{
					spliteChar
				});
			}
			return result;
		}

		public static int[] GetXElementAttributeIntArray(XElement xElement, string attributeName, string newroot = "*")
		{
			string xelementAttributeStr = XmlManager.GetXElementAttributeStr(xElement, attributeName);
			int[] result;
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				result = new int[0];
			}
			else
			{
				result = ConvertExt.String2IntArray(xelementAttributeStr, ',');
			}
			return result;
		}

		public static XElement GetXElement(XElement XElement, string newroot, string attribute1, string value1, string attribute2, string value2)
		{
			return XmlManager.GetXElement(XElement, newroot, attribute1, value1, attribute2, value2);
		}

		public static XAttribute GetAttribute(XElement XElement, string attribute)
		{
			return XmlManager.GetAttribute(XElement, attribute);
		}

		public static string GetXElementAttributeStr(XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeStr(XElement, attributeName);
		}

		public static string AttributeStr(this XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeStr(XElement, attributeName);
		}

		public static string GetXElementAttrStr(this XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeStr(XElement, attributeName);
		}

		public static int GetXElementAttributeInt(XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeInt(XElement, attributeName);
		}

		public static int AttributeInt(this XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeInt(XElement, attributeName);
		}

		public static long GetXElementAttributeLong(XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeLong(XElement, attributeName);
		}

		public static long AttributeLong(this XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeLong(XElement, attributeName);
		}

		public static double GetXElementAttributeDouble(XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeDouble(XElement, attributeName);
		}

		public static double AttributeDouble(this XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeDouble(XElement, attributeName);
		}

		public static float GetXElementAttributeFloat(XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeFloat(XElement, attributeName);
		}

		public static float AttributeFloat(this XElement XElement, string attributeName)
		{
			return XmlManager.GetXElementAttributeFloat(XElement, attributeName);
		}

		public static List<XElement> GetXElementList(XElement XElement, string newroot)
		{
			return XmlManager.GetXElementList(XElement, newroot);
		}

		public static List<XElement> XElementList(this XElement XElement, string newroot)
		{
			return XmlManager.GetXElementList(XElement, newroot);
		}

		public static List<XElement> GetXElementList(XElement XElement, string newroot, string attribute, string value)
		{
			if (XElement != null)
			{
				List<XElement> list = new List<XElement>();
				IEnumerator<XElement> enumerator;
				if ("*" == newroot)
				{
					enumerator = XElement.Elements().GetEnumerator();
				}
				else
				{
					enumerator = XElement.DescendantsAndSelf(newroot).GetEnumerator();
				}
				if (enumerator != null)
				{
					while (enumerator.MoveNext())
					{
						XElement xelement = enumerator.Current;
						XAttribute xattribute = null;
						if (xelement != null)
						{
							xattribute = xelement.Attribute(attribute);
						}
						if (xattribute != null && xattribute.Value == value)
						{
							list.Insert(list.Count, xelement);
						}
					}
				}
				return list;
			}
			return null;
		}

		public static List<XElement> GetXElementList(XElement XElement, string newroot, string attribute, int start, int end)
		{
			if (XElement != null)
			{
				List<XElement> list = new List<XElement>();
				IEnumerator<XElement> enumerator;
				if ("*" == newroot)
				{
					enumerator = XElement.Elements().GetEnumerator();
				}
				else
				{
					enumerator = XElement.DescendantsAndSelf(newroot).GetEnumerator();
				}
				if (enumerator != null)
				{
					while (enumerator.MoveNext())
					{
						XElement xelement = enumerator.Current;
						XAttribute xattribute = null;
						if (xelement != null)
						{
							xattribute = xelement.Attribute(attribute);
						}
						if (xattribute != null)
						{
							int num = Convert.ToInt32(xattribute.Value);
							if (num >= start && num <= end)
							{
								list.Insert(list.Count, xelement);
							}
						}
					}
				}
				return list;
			}
			return null;
		}

		public static string GetZhuanShengXmlFilePath()
		{
			int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			return string.Format("Config/Roles/ZhuanSheng_{0}.xml", num);
		}

		public static string GetGameResTxt(string textName)
		{
			return TextManager.GetGameResTxt(textName);
		}

		public static string GetLoginResTxt(string textName)
		{
			return TextManager.GetLoginResTxt(textName);
		}

		public static string GetIsolateResTxt(string textName)
		{
			return TextManager.GetLoginResTxt(textName);
		}

		public static int GetRandomNumber(int minVal, int maxVal)
		{
			return Global.GlobalRandom.Next(minVal, maxVal);
		}

		public static Quaternion GetQuaternionByDir(int dir)
		{
			Quaternion result = Quaternion.identity;
			if (dir == 0)
			{
				result = Quaternion.Euler(0f, 0f, 0f);
			}
			else if (dir == 1)
			{
				result = Quaternion.Euler(0f, 45f, 0f);
			}
			else if (dir == 2)
			{
				result = Quaternion.Euler(0f, 90f, 0f);
			}
			else if (dir == 3)
			{
				result = Quaternion.Euler(0f, 135f, 0f);
			}
			else if (dir == 4)
			{
				result = Quaternion.Euler(0f, 180f, 0f);
			}
			else if (dir == 5)
			{
				result = Quaternion.Euler(0f, -135f, 0f);
			}
			else if (dir == 6)
			{
				result = Quaternion.Euler(0f, -90f, 0f);
			}
			else if (dir == 7)
			{
				result = Quaternion.Euler(0f, -45f, 0f);
			}
			return result;
		}

		public static double GetDirectionByTan(double targetX, double targetY, double currentX, double currentY)
		{
			return Global.GetDirectionByAspect((int)targetX, (int)targetY, (int)currentX, (int)currentY);
		}

		public static double GetDirectionByAspect(int targetX, int targetY, int currentX, int currentY)
		{
			int num = 0;
			if (targetX < currentX)
			{
				if (targetY < currentY)
				{
					num = 5;
				}
				else if (targetY == currentY)
				{
					num = 6;
				}
				else if (targetY > currentY)
				{
					num = 7;
				}
			}
			else if (targetX == currentX)
			{
				if (targetY < currentY)
				{
					num = 4;
				}
				else if (targetY > currentY)
				{
					num = 0;
				}
			}
			else if (targetX > currentX)
			{
				if (targetY < currentY)
				{
					num = 3;
				}
				else if (targetY == currentY)
				{
					num = 2;
				}
				else if (targetY > currentY)
				{
					num = 1;
				}
			}
			return (double)num;
		}

		public static double GetAngle(double y, double x)
		{
			return Math.Atan2(y, x) / 3.1415926535897931 * 180.0;
		}

		public static void GetAngleRangeByDirection(int direction, double angleLimit, out double loAngle, out double hiAngle)
		{
			loAngle = 0.0;
			hiAngle = 0.0;
			if (angleLimit <= 0.0 || angleLimit >= 360.0)
			{
				loAngle = 0.0;
				hiAngle = 360.0;
				return;
			}
			double num = 45.0 * (double)direction;
			hiAngle = num + angleLimit / 2.0;
			if (hiAngle >= 360.0)
			{
				hiAngle -= 360.0;
			}
			loAngle = num - angleLimit / 2.0;
			if (loAngle < 0.0)
			{
				loAngle = 360.0 + loAngle;
			}
		}

		public static double GetCircleAngle(Point p0, Point p1)
		{
			if (p0 == p1)
			{
				return 0.0;
			}
			double num = Math.Atan2((double)Math.Abs(p1.Y - p0.Y), (double)Math.Abs(p1.X - p0.X)) * 180.0 / 3.1415926535897931;
			if (p1.X >= p0.X && p0.Y < p1.Y)
			{
				num = 90.0 - num;
			}
			else if (p1.X >= p0.X && p0.Y >= p1.Y)
			{
				num = 90.0 + num;
			}
			else if (p1.X < p0.X && p0.Y >= p1.Y)
			{
				num = 270.0 - num;
			}
			else if (p1.X < p0.X && p0.Y < p1.Y)
			{
				num = 270.0 + num;
			}
			return num;
		}

		public static bool InAngleRange(double angle, double loAngle, double hiAngle)
		{
			if (hiAngle > loAngle)
			{
				return angle >= loAngle && angle <= hiAngle;
			}
			return (angle >= loAngle && angle < 360.0) || (angle >= 0.0 && angle <= hiAngle);
		}

		public static bool InCircleByAngle(Point target, Point center, double radius, double loAngle, double hiAngle)
		{
			double circleAngle = Global.GetCircleAngle(center, target);
			return Global.InAngleRange(circleAngle, loAngle, hiAngle) && Math.Pow((double)(target.X - center.X), 2.0) + Math.Pow((double)(target.Y - center.Y), 2.0) <= Math.Pow(radius, 2.0);
		}

		public static bool InCircle(Point target, Point center, double radius)
		{
			double num = Math.Pow((double)(target.X - center.X), 2.0) + Math.Pow((double)(target.Y - center.Y), 2.0);
			double num2 = Math.Pow(radius, 2.0);
			return num <= num2;
		}

		public static bool InCircleByGridNum(Point target, Point center, double radiusGridNum)
		{
			if (Global.CurrentMapData.GridSizeX == 0 || Global.CurrentMapData.GridSizeY == 0)
			{
				return true;
			}
			int num = center.X / Global.CurrentMapData.GridSizeX;
			int num2 = center.Y / Global.CurrentMapData.GridSizeY;
			int num3 = target.X / Global.CurrentMapData.GridSizeX;
			int num4 = target.Y / Global.CurrentMapData.GridSizeY;
			return num3 < num || (double)num3 > (double)num + radiusGridNum || num4 < num2 || (double)num4 > (double)num2 + radiusGridNum || true;
		}

		public static Point GetExtensionPoint(Point start, Point end, int Lenght)
		{
			if (Lenght == 0)
			{
				return new Point(0, 0);
			}
			double num = Math.Sqrt(Math.Pow((double)(end.Y - start.Y), 2.0) + Math.Pow((double)(end.X - start.X), 2.0)) / (double)Lenght;
			if (num == 0.0)
			{
				return new Point(0, 0);
			}
			return new Point((int)((double)start.X + (double)(end.X - start.X) / num), (int)((double)start.Y + (double)(end.Y - start.Y) / num));
		}

		public static Point GetExtensionPoint(Point op, double angle, double length)
		{
			if (angle == 0.0)
			{
				return new Point(op.X, op.Y + (int)length);
			}
			if (angle > 0.0 && angle < 90.0)
			{
				double num = angle * 3.1415926535897931 / 180.0;
				double num2 = Math.Sin(num) * length;
				double num3 = Math.Cos(num) * length;
				return new Point(op.X + (int)num2, op.Y + (int)num3);
			}
			if (angle == 90.0)
			{
				return new Point(op.X + (int)length, op.Y);
			}
			if (angle > 90.0 && angle < 180.0)
			{
				double num4 = (180.0 - angle) * 3.1415926535897931 / 180.0;
				double num5 = Math.Sin(num4) * length;
				double num6 = Math.Cos(num4) * length;
				return new Point(op.X + (int)num5, op.Y - (int)num6);
			}
			if (angle == 180.0)
			{
				return new Point(op.X, op.Y - (int)length);
			}
			if (angle > 180.0 && angle < 270.0)
			{
				double num7 = (angle - 180.0) * 3.1415926535897931 / 180.0;
				double num8 = Math.Sin(num7) * length;
				double num9 = Math.Cos(num7) * length;
				return new Point(op.X - (int)num8, op.Y - (int)num9);
			}
			if (angle == 270.0)
			{
				return new Point(op.X - (int)length, op.Y);
			}
			if (angle > 270.0 && angle < 360.0)
			{
				double num10 = (360.0 - angle) * 3.1415926535897931 / 180.0;
				double num11 = Math.Sin(num10) * length;
				double num12 = Math.Cos(num10) * length;
				return new Point(op.X - (int)num11, op.Y + (int)num12);
			}
			return op;
		}

		public static double GetTwoPointDistance(Point start, Point end)
		{
			return Math.Sqrt(Math.Pow((double)(end.X - start.X), 2.0) + Math.Pow((double)(end.Y - start.Y), 2.0));
		}

		public static double GetTwoPointDistanceSquare(Point start, Point end)
		{
			return Math.Pow((double)(end.X - start.X), 2.0) + Math.Pow((double)(end.Y - start.Y), 2.0);
		}

		public static double GetAnimationTimeConsuming(Point start, Point end, int zoomX, int zoomY, double unitCost)
		{
			if (zoomX == 0 || zoomY == 0)
			{
				return 0.0;
			}
			return Math.Sqrt(Math.Pow((double)((end.X - start.X) / zoomX), 2.0) + Math.Pow((double)((end.Y - start.Y) / zoomY), 2.0)) * unitCost;
		}

		public static bool Bresenham(List<ANode> s, int x1, int y1, int x2, int y2, byte[,] obs)
		{
			bool flag = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
			if (flag)
			{
				int num = x1;
				x1 = y1;
				y1 = num;
				num = x2;
				x2 = y2;
				y2 = num;
			}
			bool flag2 = false;
			if (x1 > x2)
			{
				int num = x1;
				x1 = x2;
				x2 = num;
				num = y1;
				y1 = y2;
				y2 = num;
				flag2 = true;
			}
			int num2 = x2 - x1;
			int num3 = Math.Abs(y2 - y1);
			int num4 = num2 / 2;
			int i = x1;
			int num5 = y1;
			while (i <= x2)
			{
				if (flag)
				{
					if (s != null)
					{
						s.Add(new ANode(num5, i));
					}
				}
				else if (s != null)
				{
					s.Add(new ANode(i, num5));
				}
				num4 -= num3;
				if (num4 < 0)
				{
					if (y1 < y2)
					{
						num5++;
					}
					else
					{
						num5--;
					}
					num4 += num2;
				}
				i++;
			}
			if (s == null)
			{
				return false;
			}
			if (flag2)
			{
				s.Reverse();
			}
			List<ANode> linearPath = Global.GetLinearPath(s, obs);
			bool result = linearPath.Count == s.Count;
			s.Clear();
			for (int j = 0; j < linearPath.Count; j++)
			{
				s.Add(linearPath[j]);
			}
			return result;
		}

		private static List<ANode> GetLinearPath(List<ANode> s, byte[,] obs)
		{
			List<ANode> list = new List<ANode>();
			for (int i = 0; i < s.Count; i++)
			{
				if (s[i].x < obs.GetUpperBound(0) && s[i].y < obs.GetUpperBound(1))
				{
					if (obs[s[i].x, s[i].y] == 0)
					{
						break;
					}
					list.Add(s[i]);
				}
			}
			return list;
		}

		public static Point GetAPointIn4Direction(Point p, int offset, byte[,] obs, int mapWidth, int mapHeight, int gridWidth, int gridHeight)
		{
			if (gridWidth == 0 || gridHeight == 0)
			{
				return p;
			}
			int num = p.X;
			int num2 = p.Y;
			num = Math.Max(0, num);
			num2 = Math.Max(0, num2);
			num = Math.Min(mapWidth - 1, num);
			num2 = Math.Min(mapHeight - 1, num2);
			if (obs != null && obs[num / gridWidth, num2 / gridHeight] > 0)
			{
				return new Point(num, num2);
			}
			num = p.X - offset;
			num2 = p.Y;
			num = Math.Max(0, num);
			num2 = Math.Max(0, num2);
			num = Math.Min(mapWidth, num);
			num2 = Math.Min(mapHeight, num2);
			if (obs != null && obs[num / gridWidth, num2 / gridHeight] > 0)
			{
				return new Point(num, num2);
			}
			num = p.X + offset;
			num2 = p.Y;
			num = Math.Max(0, num);
			num2 = Math.Max(0, num2);
			num = Math.Min(mapWidth, num);
			num2 = Math.Min(mapHeight, num2);
			if (obs != null && obs[num / gridWidth, num2 / gridHeight] > 0)
			{
				return new Point(num, num2);
			}
			num = p.X;
			num2 = p.Y - offset;
			num = Math.Max(0, num);
			num2 = Math.Max(0, num2);
			num = Math.Min(mapWidth, num);
			num2 = Math.Min(mapHeight, num2);
			if (obs != null && obs[num / gridWidth, num2 / gridHeight] > 0)
			{
				return new Point(num, num2);
			}
			num = p.X;
			num2 = p.Y + offset;
			num = Math.Max(0, num);
			num2 = Math.Max(0, num2);
			num = Math.Min(mapWidth, num);
			num2 = Math.Min(mapHeight, num2);
			if (obs != null && obs[num / gridWidth, num2 / gridHeight] > 0)
			{
				return new Point(num, num2);
			}
			return p;
		}

		public static Point GetAGridPointIn4Direction(Point gridPoint, byte[,] obs, GMapData currentMapData)
		{
			int x = gridPoint.X;
			int y = gridPoint.Y;
			if (x >= obs.GetUpperBound(0) || y >= obs.GetUpperBound(1))
			{
				return gridPoint;
			}
			if (obs[x, y] == 1)
			{
				return gridPoint;
			}
			Point result = gridPoint;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return gridPoint;
			}
			int num = (currentMapData.MapWidth - 1) / currentMapData.GridSizeX + 1;
			int num2 = (currentMapData.MapHeight - 1) / currentMapData.GridSizeY + 1;
			int num3 = 1;
			int num4;
			int num5;
			int num6;
			int num7;
			for (;;)
			{
				num4 = x + num3;
				num5 = y + num3;
				num6 = x - num3;
				num7 = y - num3;
				int num8 = 8;
				if (0 <= num4 && num4 < num && 0 <= num5 && num5 < num2)
				{
					num8--;
					if (obs[num4, num5] == 1)
					{
						break;
					}
				}
				if (0 <= num4 && num4 < num && 0 <= num7 && num7 < num2)
				{
					num8--;
					if (obs[num4, num7] == 1)
					{
						goto Block_13;
					}
				}
				if (0 <= num6 && num6 < num && 0 <= num5 && num5 < num2)
				{
					num8--;
					if (obs[num6, num5] == 1)
					{
						goto Block_18;
					}
				}
				if (0 <= num6 && num6 < num && 0 <= num7 && num7 < num2)
				{
					num8--;
					if (obs[num6, num7] == 1)
					{
						goto Block_23;
					}
				}
				if (0 <= num4 && num4 < num)
				{
					num8--;
					if (obs[num4, y] == 1)
					{
						goto Block_26;
					}
				}
				if (0 <= num5 && num5 < num2)
				{
					num8--;
					if (obs[x, num5] == 1)
					{
						goto Block_29;
					}
				}
				if (0 <= num6 && num6 < num)
				{
					num8--;
					if (obs[num6, y] == 1)
					{
						goto Block_32;
					}
				}
				if (0 <= num7 && num7 < num2)
				{
					num8--;
					if (obs[x, num7] == 1)
					{
						goto Block_35;
					}
				}
				if (num8 >= 8)
				{
					return result;
				}
				num3++;
			}
			result = new Point(num4, num5);
			return result;
			Block_13:
			result = new Point(num4, num7);
			return result;
			Block_18:
			result = new Point(num6, num5);
			return result;
			Block_23:
			result = new Point(num6, num7);
			return result;
			Block_26:
			result = new Point(num4, y);
			return result;
			Block_29:
			result = new Point(x, num5);
			return result;
			Block_32:
			result = new Point(num6, y);
			return result;
			Block_35:
			result = new Point(x, num7);
			return result;
		}

		public static Point ConvertPointToGirdXY(Point p)
		{
			if (Global.CurrentMapData.GridSizeX == 0 || Global.CurrentMapData.GridSizeY == 0)
			{
				return new Point(0, 0);
			}
			int x = p.X / Global.CurrentMapData.GridSizeX * Global.CurrentMapData.GridSizeX + Global.CurrentMapData.GridSizeX / 2;
			int y = p.Y / Global.CurrentMapData.GridSizeY * Global.CurrentMapData.GridSizeY + Global.CurrentMapData.GridSizeY / 2;
			return new Point(x, y);
		}

		public static bool CompareTowPointByGridXY(Point p1, Point p2)
		{
			Point point = Global.ConvertPointToGirdXY(p1);
			Point point2 = Global.ConvertPointToGirdXY(p2);
			return point.X == point2.X && point.Y == point2.Y;
		}

		public static bool InArea(int centerGridX, int centerGridY, int radius, Point grid)
		{
			int num = Math.Abs(grid.X - centerGridX);
			int num2 = Math.Abs(grid.Y - centerGridY);
			if (num > radius || num2 > radius)
			{
				return false;
			}
			num2 = radius - num;
			int num3 = centerGridY - num2;
			int num4 = centerGridY + num2;
			return grid.Y >= num3 && grid.Y <= num4;
		}

		public static List<Point> GetGridPointByDirection(int direction, int gridX, int gridY, int nNum)
		{
			List<Point> list = new List<Point>();
			int num = gridX;
			int num2 = gridY;
			for (int i = 0; i < nNum; i++)
			{
				switch (direction)
				{
				case 0:
					num2++;
					break;
				case 1:
					num++;
					num2++;
					break;
				case 2:
					num++;
					break;
				case 3:
					num++;
					num2--;
					break;
				case 4:
					num2--;
					break;
				case 5:
					num--;
					num2--;
					break;
				case 6:
					num--;
					break;
				case 7:
					num--;
					num2++;
					break;
				}
				list.Add(new Point(num, num2));
			}
			return list;
		}

		public static void SetOpacity(IObject obj, GMapData currentMapData)
		{
		}

		public static bool OnObstruction(Point p, GMapData currentMapData)
		{
			if (currentMapData == null)
			{
				return false;
			}
			int num = Math.Max(0, p.X);
			int num2 = Math.Max(0, p.Y);
			num = Math.Min(currentMapData.MapWidth, num);
			num2 = Math.Min(currentMapData.MapHeight, num2);
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return false;
			}
			int x = num / currentMapData.GridSizeX;
			int y = num2 / currentMapData.GridSizeY;
			return Global.OnObstructionByGrid(new Point(x, y), currentMapData);
		}

		public static bool OnObstructionByGrid(Point grid, GMapData currentMapData)
		{
			if (currentMapData == null)
			{
				return false;
			}
			int x = grid.X;
			int y = grid.Y;
			return currentMapData.fixedObstruction[x, y] == 0;
		}

		public static StoryBoard NewStoryboard(string key, object tag)
		{
			StoryBoard.RemoveStoryBoard(key);
			StoryBoard storyBoard = new StoryBoard(key);
			storyBoard.Tag = tag;
			storyBoard.Binding();
			return storyBoard;
		}

		public static StoryBoard FindStoryboard(string key)
		{
			return StoryBoard.FindStoryBoard(key);
		}

		public static void RemoveStoryboard(string key)
		{
			if (StoryBoard.ContainStoryBoard(key))
			{
				StoryBoard.RemoveStoryBoard(key);
			}
		}

		public static int StopStoryboard(string key, int stopIndex = -1)
		{
			if (StoryBoard.ContainStoryBoard(key))
			{
				return StoryBoard.StopStoryBoard(key, stopIndex);
			}
			return -1;
		}

		public static bool FindMoveStroyboard(string key)
		{
			return StoryBoard.ContainStoryBoard(key);
		}

		public static int GetSpriteBodyCode(int spriteType)
		{
			int result = -1;
			switch (spriteType)
			{
			case 0:
			case 1:
			case 7:
				result = 0;
				break;
			case 2:
				result = 1;
				break;
			case 3:
				result = 2;
				break;
			case 4:
				result = 3;
				break;
			case 5:
				result = 4;
				break;
			case 6:
				result = 5;
				break;
			}
			return result;
		}

		public static void CheckSpirteActions(GSprite sprite)
		{
			if (sprite.SpriteType == GSpriteTypes.NPC)
			{
				return;
			}
			GActions gactions = GActions.Walk;
			if (sprite.Action == GActions.Stand)
			{
				if (StoryBoard.FindStoryBoard(sprite.Name) != null)
				{
					gactions = GActions.Run;
				}
			}
			else if (sprite.Action == GActions.PreAttack)
			{
				if (StoryBoard.FindStoryBoard(sprite.Name) != null)
				{
					gactions = GActions.Run;
				}
			}
			else if ((sprite.Action == GActions.Run || sprite.Action == GActions.Walk) && StoryBoard.FindStoryBoard(sprite.Name) == null)
			{
				gactions = GActions.Stand;
			}
			if (gactions < GActions.Stand)
			{
				return;
			}
			sprite.Action = gactions;
		}

		public static void DoInjure(GSprite attacker, GSprite enemy, Point targetPos, int magicCode)
		{
			GameInstance.Game.SpriteAttack(attacker.Coordinate, (enemy != null) ? enemy.RoleID : -1, targetPos, (enemy != null) ? enemy.Coordinate : new Point(-1, -1), magicCode);
		}

		public static void DoInjure2(GSprite attacker, GSprite enemy, Point targetPos, int magicCode)
		{
			GameInstance.Game.SpriteAttack2(attacker.RoleID, attacker.Coordinate, (enemy != null) ? enemy.RoleID : -1, targetPos, (enemy != null) ? enemy.Coordinate : new Point(-1, -1), magicCode);
		}

		public static void BattleHandleEx(GSprite attacker, GSprite injuredSprite, int burst, double injure, double injuredLife, double newExperience, bool levelUp, SpriteAttackResultData attackResultData = null)
		{
			if (injuredSprite == null)
			{
				return;
			}
			if (injuredSprite.RoleID != Global.Data.roleData.RoleID && ((attacker != null && attacker.RoleID == Global.Data.roleData.RoleID) || (attacker != null && injuredSprite != null && attacker.SpriteType == GSpriteTypes.Other && injuredSprite.SpriteType == GSpriteTypes.Other)) && injure >= 0.0)
			{
				if (injure != 0.0)
				{
					if (injure != 28.0)
					{
						switch (burst)
						{
						case 0:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(16777215U), string.Empty, -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Normal);
							break;
						case 1:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(64250U), Global.GetLang("无视防御"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 2:
							Global.ShowText(injuredSprite, 0.0, 0.0, Color.yellow, Global.GetLang("双倍一击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 3:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(65430U), Global.GetLang("卓越一击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 4:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(38650U), Global.GetLang("幸运一击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 5:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(14418140U), Global.GetLang("反弹伤害"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Normal);
							break;
						case 6:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(16757760U), Global.GetLang("无情一击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 7:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(16757760U), Global.GetLang("冷血一击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 8:
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(16757760U), Global.GetLang("野蛮一击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						case 11:
							MUDebug.Log<string>(new string[]
							{
								"<color=yellow>重生暴击 人物  Value = " + (int)injure + "</color>"
							});
							Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(16777215U), Global.GetLang("重生暴击"), -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Special);
							break;
						}
						if (attackResultData != null && attackResultData.MerlinInjuer > 0)
						{
							switch (attackResultData.MerlinType)
							{
							case 0:
								Global.ShowText(injuredSprite, 0.0, 0.0, Color.red, Global.GetLang("冻结伤害"), -1, (long)attackResultData.MerlinInjuer, 1, 500.0, (attacker == null) ? 4 : attacker.Direction, HUDTextCustom.TextType.Miss2);
								break;
							case 1:
								Global.ShowText(injuredSprite, 0.0, 0.0, Color.red, Global.GetLang("麻痹伤害"), -1, (long)attackResultData.MerlinInjuer, 2, 500.0, (attacker == null) ? 7 : attacker.Direction, HUDTextCustom.TextType.Miss2);
								break;
							case 2:
								Global.ShowText(injuredSprite, 0.0, 0.0, Color.red, Global.GetLang("减速伤害"), -1, (long)attackResultData.MerlinInjuer, 3, 500.0, (attacker == null) ? 7 : attacker.Direction, HUDTextCustom.TextType.Miss2);
								break;
							case 3:
								Global.ShowText(injuredSprite, 0.0, 0.0, Color.red, Global.GetLang("重击伤害"), -1, (long)attackResultData.MerlinInjuer, 4, 500.0, (attacker == null) ? 7 : attacker.Direction, HUDTextCustom.TextType.Miss2);
								break;
							}
						}
					}
				}
				else if (burst == 10)
				{
					Global.ShowText(injuredSprite, 0.0, 0.0, Color.white, Global.GetLang("魔法免疫"), -1, 0L, 2, 500.0, (attacker == null) ? 2 : attacker.Direction, HUDTextCustom.TextType.Miss);
				}
				else if (burst == 9)
				{
					Global.ShowText(injuredSprite, 0.0, 0.0, Color.white, Global.GetLang("物理免疫"), -1, 0L, 2, 500.0, (attacker == null) ? 2 : attacker.Direction, HUDTextCustom.TextType.Miss);
				}
				else
				{
					Global.ShowText(injuredSprite, 0.0, 0.0, Color.white, Global.GetLang("闪避"), -1, 0L, 2, 500.0, (attacker == null) ? 2 : attacker.Direction, HUDTextCustom.TextType.Miss);
				}
			}
			injuredSprite.VLife = injuredLife;
			if (injuredSprite.VLife <= 0.0)
			{
				injuredSprite.VLife = 0.0;
				injuredSprite.LockObject = null;
				string text = injuredSprite.Name;
				if (injure != 28.0)
				{
					StoryBoard storyBoard = Global.FindStoryboard(text);
					if (storyBoard != null && !storyBoard.NoAction)
					{
						Global.RemoveStoryboard(text);
					}
					injuredSprite.Action = GActions.Death;
				}
				else
				{
					StoryBoard storyBoard2 = Global.FindStoryboard(text);
					if (storyBoard2 != null && !storyBoard2.NoAction)
					{
						Global.RemoveStoryboard(text);
					}
					injuredSprite.ExternalDeath();
				}
				if (attacker != null && attacker.LockObject == text)
				{
					attacker.LockObject = null;
				}
			}
		}

		public static void BattleHandleEx_(GSprite attacker, GSprite injuredSprite, int burst, double injure, double injuredLife, double newExperience, bool levelUp, int attackerRoleID, int injuredRoleID, SpriteAttackResultData attackResultData = null)
		{
			if (injuredSprite != null)
			{
				if (injuredSprite.RoleID != Global.Data.roleData.RoleID && attacker != null)
				{
					if (injure > 0.0)
					{
						if (injure != 2147483647.0 && burst == 0)
						{
							if (Global.IsOwnZhaoHuanShou(injuredRoleID))
							{
								Global.ShowText(injuredSprite, 0.0, 0.0, Color.red, string.Empty, -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Normal);
							}
							else
							{
								Global.ShowText(injuredSprite, 0.0, 0.0, ColorSL.ParseArgb(16777215U), string.Empty, -1, (long)((int)injure), 0, 1000.0, 2, HUDTextCustom.TextType.Normal);
							}
						}
					}
					else if (burst == 10)
					{
						Global.ShowText(injuredSprite, 0.0, 0.0, Color.white, Global.GetLang("魔法免疫"), -1, 0L, 2, 500.0, (attacker == null) ? 2 : attacker.Direction, HUDTextCustom.TextType.Miss);
					}
					else if (burst == 9)
					{
						Global.ShowText(injuredSprite, 0.0, 0.0, Color.white, Global.GetLang("物理免疫"), -1, 0L, 2, 500.0, (attacker == null) ? 2 : attacker.Direction, HUDTextCustom.TextType.Miss);
					}
					else
					{
						Global.ShowText(injuredSprite, 0.0, 0.0, Color.white, Global.GetLang("闪避"), -1, 0L, 2, 500.0, (attacker == null) ? 2 : attacker.Direction, HUDTextCustom.TextType.Miss);
					}
				}
				injuredSprite.VLife = injuredLife;
			}
		}

		public static void BattleHandleEx(GSprite self, int burst, double injure, double myLife, int direction)
		{
			if (injure >= 0.0)
			{
				if (injure != 0.0)
				{
					if (injure != 28.0)
					{
						if (burst == 5)
						{
							Global.ShowText(self, ColorSL.ParseArgb(14418140U), Global.GetLang("伤害反弹") + injure.ToString(), 1000f, 0f, 0f);
						}
						else
						{
							Global.ShowText(self, ColorSL.ParseArgb(14417920U), injure.ToString(), 1000f, 0f, 0f);
						}
					}
				}
				else if (burst == 10)
				{
					Global.ShowText(self, 0.0, 0.0, Color.white, Global.GetLang("魔法免疫"), -1, 0L, 2, 500.0, direction, HUDTextCustom.TextType.Miss);
				}
				else if (burst == 9)
				{
					Global.ShowText(self, 0.0, 0.0, Color.white, Global.GetLang("物理免疫"), -1, 0L, 2, 500.0, direction, HUDTextCustom.TextType.Miss);
				}
				else
				{
					Global.ShowText(self, 0.0, 0.0, Color.white, Global.GetLang("闪避"), -1, 0L, 2, 500.0, direction, HUDTextCustom.TextType.Miss);
				}
			}
			self.VLife = myLife;
			if (self.VLife <= 0.0)
			{
				if (injure != 29.0)
				{
					StoryBoard storyBoard = Global.FindStoryboard(self.Name);
					if (storyBoard != null && !storyBoard.NoAction)
					{
						Global.RemoveStoryboard(self.Name);
					}
					self.Action = GActions.Death;
				}
				else
				{
					StoryBoard storyBoard2 = Global.FindStoryboard(self.Name);
					if (storyBoard2 != null && !storyBoard2.NoAction)
					{
						Global.RemoveStoryboard(self.Name);
					}
					self.ExternalDeath();
				}
			}
		}

		public static void EarnLevel(GSprite sprite, bool levelUp)
		{
			if (levelUp)
			{
				string text = "UpLevel";
				if (sprite != null && sprite.FindName(text) == null)
				{
					GDecoration decoration = Global.GetDecoration(72, GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
					decoration.Name = "UpLevel";
					sprite.Root.Children.Add(decoration);
				}
			}
		}

		public static void RemoveObject(IObject obj, bool clearFromParent = true)
		{
			if (obj is GSprite)
			{
				Global.RemoveStoryboard((obj as GSprite).Name);
			}
			if (clearFromParent)
			{
				if (obj is GDecoration)
				{
					if (obj.Tag == null || null == obj.Parent)
					{
						ObjectsManager.Remove(obj);
					}
					else
					{
						GSprite gsprite = obj.Tag as GSprite;
						if (gsprite != null)
						{
							gsprite.Remove(obj);
						}
					}
				}
				else
				{
					ObjectsManager.Remove(obj);
				}
			}
			obj.Destroy();
			obj = null;
		}

		public static string GetActionName(GSprite sprite, GActions action, bool inSafeRegion, out WrapMode wrapMode)
		{
			wrapMode = 2;
			if (sprite.SpriteType == GSpriteTypes.Leader || sprite.SpriteType == GSpriteTypes.Other)
			{
				RoleData roleData = null;
				if (sprite.RoleID == Global.Data.RoleID)
				{
					roleData = Global.Data.roleData;
				}
				else
				{
					Global.Data.OtherRoles.TryGetValue(sprite.RoleID, ref roleData);
				}
				if (roleData != null && Global.IsBufferExist(121, roleData))
				{
					return Global.GetBianShenActionName(sprite, action, inSafeRegion, out wrapMode);
				}
			}
			bool isFlying = sprite.IsFlying;
			bool inWater = sprite.InWater;
			bool onHorseEX = sprite.OnHorseEX;
			WeaponStates weaponState = sprite.WeaponState;
			bool isFjs_F = true;
			if (sprite.Occupation == 3)
			{
				if (sprite.RoleID == Global.Data.RoleID)
				{
					if (Global.GetMJSType() == MJSSkillType.Strength_Sword)
					{
						isFjs_F = false;
					}
				}
				else if (Global.Data.OtherRoles.ContainsKey(sprite.RoleID) && Global.GetMJSType(Global.Data.OtherRoles[sprite.RoleID]) == MJSSkillType.Strength_Sword)
				{
					isFjs_F = false;
				}
			}
			return Global.GetActionName(sprite.SpriteType, sprite.Occupation, action, weaponState, isFlying, inWater, onHorseEX, inSafeRegion, out wrapMode, sprite.GetAttackNum((int)action), sprite.IsCloneRole, isFjs_F);
		}

		public static string GetBianShenActionName(GSprite sprite, GActions action, bool inSafeRegion, out WrapMode wrapMode)
		{
			wrapMode = 2;
			string result = "Stand";
			switch (action)
			{
			case GActions.Stand:
				result = "Stand";
				break;
			case GActions.Walk:
				result = "Walk";
				break;
			case GActions.Run:
				result = "Run";
				break;
			case GActions.Attack:
				wrapMode = 1;
				result = "Attack01";
				break;
			case GActions.Injured:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Death:
				wrapMode = 1;
				result = "Die";
				break;
			case GActions.Sit:
			case GActions.PreAttack:
			case GActions.IdleStand:
			case GActions.Italic:
			case GActions.Collect:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Wenhao:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Genwolai:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Guzhang:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Huanhu:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Jushang:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Xingli:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Chongfeng:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Mobai:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Tiaoxin:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Zuoxia:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.Shuijiao:
				wrapMode = 1;
				result = "Stand";
				break;
			case GActions.ZS_Orz:
				wrapMode = 2;
				result = "Stand";
				break;
			case GActions.GS_Orz:
				wrapMode = 2;
				result = "Stand";
				break;
			case GActions.FS_Orz:
				wrapMode = 2;
				result = "Stand";
				break;
			case GActions.MJ_Orz:
				wrapMode = 2;
				result = "Stand";
				break;
			}
			return result;
		}

		public static string GetActionName(GSpriteTypes spriteType, int occupation, GActions action, WeaponStates weaponState, bool isFlying, bool inWater, bool onHorse, bool inSafeRegion, out WrapMode wrapMode, int attackNum = 1, bool isCloneRole = false, bool isFjs_F = true)
		{
			wrapMode = 2;
			string text = "Stand";
			if (action == GActions.Walk && isFlying && !inSafeRegion)
			{
				action = GActions.Run;
			}
			if (weaponState == WeaponStates.SD && action != GActions.Attack)
			{
				weaponState = WeaponStates.D;
			}
			switch (action)
			{
			case GActions.Stand:
				text = "Stand";
				if (spriteType == GSpriteTypes.Leader || spriteType == GSpriteTypes.Other || spriteType == GSpriteTypes.FakeRole || isCloneRole)
				{
					if (inSafeRegion)
					{
						if (onHorse)
						{
							text = "R_Stand";
						}
						else
						{
							text = WeaponStates.K.ToString() + text;
						}
					}
					else if (onHorse)
					{
						text = "R_Stand";
					}
					else if (inWater || isFlying)
					{
						if (occupation == 3)
						{
							text = "Fly_" + text;
						}
						else
						{
							text = "Fly_" + weaponState.ToString() + text;
						}
					}
					else if (occupation == 3 && weaponState != WeaponStates.K && weaponState != WeaponStates.MJ)
					{
						if (isFjs_F)
						{
							text = "F" + weaponState.ToString() + text;
						}
						else
						{
							text = "Z" + weaponState.ToString() + text;
						}
					}
					else
					{
						text = weaponState.ToString() + text;
					}
				}
				break;
			case GActions.Walk:
				text = "Walk";
				if (spriteType == GSpriteTypes.Leader || spriteType == GSpriteTypes.Other || spriteType == GSpriteTypes.FakeRole || isCloneRole)
				{
					if (inSafeRegion)
					{
						if (onHorse)
						{
							text = "R_Walk";
						}
						else
						{
							text = WeaponStates.K.ToString() + text;
						}
					}
					else if (onHorse)
					{
						text = "R_Walk";
					}
					else if (inWater)
					{
						text = "Swim_" + text;
					}
					else if (occupation == 3 && weaponState != WeaponStates.K && weaponState != WeaponStates.MJ)
					{
						if (isFjs_F)
						{
							text = "F" + weaponState.ToString() + text;
						}
						else
						{
							text = "Z" + weaponState.ToString() + text;
						}
					}
					else
					{
						text = weaponState.ToString() + text;
					}
				}
				break;
			case GActions.Run:
				text = "Run";
				if (spriteType == GSpriteTypes.Leader || spriteType == GSpriteTypes.Other || spriteType == GSpriteTypes.FakeRole || isCloneRole)
				{
					if (onHorse)
					{
						text = "R_Run";
					}
					else if (isFlying)
					{
						if (occupation == 3)
						{
							text = "Fly_" + text;
						}
						else
						{
							text = "Fly_" + weaponState.ToString() + text;
						}
					}
					else if (inWater)
					{
						text = "Swim_" + text;
					}
					else if (occupation == 3 && weaponState != WeaponStates.K && weaponState != WeaponStates.MJ)
					{
						if (isFjs_F)
						{
							text = "F" + weaponState.ToString() + text;
						}
						else
						{
							text = "Z" + weaponState.ToString() + text;
						}
					}
					else
					{
						text = weaponState.ToString() + text;
					}
				}
				break;
			case GActions.Attack:
				wrapMode = 1;
				text = "Attack";
				if (spriteType == GSpriteTypes.Leader || spriteType == GSpriteTypes.Other || spriteType == GSpriteTypes.FakeRole || isCloneRole)
				{
					if (onHorse)
					{
						text = "Ride_" + text;
					}
					else
					{
						text = string.Format("Attack{0:00}", attackNum);
						if (occupation == 3 && weaponState != WeaponStates.K)
						{
							if (isFjs_F)
							{
								text = "F" + weaponState.ToString() + text;
							}
							else
							{
								text = "Z" + weaponState.ToString() + text;
							}
						}
						else
						{
							text = weaponState.ToString() + text;
						}
					}
				}
				break;
			case GActions.Injured:
				wrapMode = 1;
				text = "Hit";
				if ((spriteType == GSpriteTypes.Leader || spriteType == GSpriteTypes.Other || isCloneRole) && onHorse)
				{
					text = "Ride_" + text;
				}
				break;
			case GActions.Death:
				wrapMode = 1;
				text = "Die";
				break;
			case GActions.Sit:
				text = "Sit_Relax";
				break;
			case GActions.PreAttack:
				wrapMode = 1;
				text = "Stand";
				break;
			case GActions.IdleStand:
				wrapMode = 1;
				text = "Relax";
				break;
			case GActions.Italic:
				text = "Lean_Relax";
				break;
			case GActions.Collect:
				text = "Collect";
				break;
			case GActions.Wenhao:
				wrapMode = 1;
				text = "wenhao";
				break;
			case GActions.Genwolai:
				wrapMode = 1;
				text = "guolai";
				break;
			case GActions.Guzhang:
				wrapMode = 1;
				text = "guzhang";
				break;
			case GActions.Huanhu:
				wrapMode = 1;
				text = "huanhu";
				break;
			case GActions.Jushang:
				wrapMode = 1;
				text = "jushang";
				break;
			case GActions.Xingli:
				wrapMode = 1;
				text = "xingli";
				break;
			case GActions.Chongfeng:
				wrapMode = 1;
				text = "chongfeng";
				break;
			case GActions.Mobai:
				wrapMode = 1;
				text = "mobai";
				break;
			case GActions.Tiaoxin:
				wrapMode = 1;
				text = "tiaoxin";
				break;
			case GActions.Zuoxia:
				wrapMode = 1;
				text = "zuoxia";
				break;
			case GActions.Shuijiao:
				wrapMode = 1;
				text = "shuijiao";
				break;
			case GActions.ZS_Orz:
				wrapMode = 2;
				text = "ZS_Orz";
				break;
			case GActions.GS_Orz:
				wrapMode = 2;
				text = "GS_Orz";
				break;
			case GActions.FS_Orz:
				wrapMode = 2;
				text = "FS_Orz";
				break;
			case GActions.MJ_Orz:
				wrapMode = 2;
				text = "MJ_Orz";
				break;
			}
			return text;
		}

		public static int GetMovingAction(Point standPos, Point toPos)
		{
			GMapData currentMapData = Global.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return -1;
			}
			int num = standPos.X / currentMapData.GridSizeX;
			int num2 = standPos.Y / currentMapData.GridSizeY;
			int num3 = toPos.X / currentMapData.GridSizeX;
			int num4 = toPos.Y / currentMapData.GridSizeY;
			if (num == num3 && num2 == num4)
			{
				return -1;
			}
			if (Global.Data.MoveMode == 0)
			{
				return 1;
			}
			int num5 = (int)Global.GetDirectionByTan((double)toPos.X, (double)toPos.Y, (double)standPos.X, (double)standPos.Y);
			if (num5 == 0 || num5 == 4)
			{
				if (Global.GetTwoPointDistance(standPos, toPos) <= 80.0)
				{
					return 1;
				}
			}
			else if (num5 == 2 || num5 == 6)
			{
				if (Global.GetTwoPointDistance(standPos, toPos) <= 160.0)
				{
					return 1;
				}
			}
			else if (Global.GetTwoPointDistance(standPos, toPos) <= 70.0)
			{
				return 1;
			}
			return 2;
		}

		public static string GetMonsterActionName(int currentMagic, out WrapMode wrapMode)
		{
			wrapMode = 1;
			string result = "Attack";
			MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(currentMagic);
			if (maigcInfoVOByCode != null)
			{
				string skillAction = maigcInfoVOByCode.SkillAction;
				if (!string.IsNullOrEmpty(skillAction))
				{
					result = skillAction;
				}
			}
			return result;
		}

		public static void ShowTextForExpOrGold(Color picTextColor, string text, float moveTicks, float offsetX = 0f, float fontSize = 0f, float pointX = 280f, float pointY = -150f)
		{
			if (null == Global.LeaderGainTextHeadText)
			{
				GameObject gameObject = Resources.Load("Prefabs/HUD/HUDText") as GameObject;
				GameObject gameObject2 = NGUITools.AddChild(HUDTextRoot.go, gameObject);
				Global.LeaderGainTextHeadText = gameObject2.GetComponentInChildren<HUDText>();
				if (null == Global.LeaderGainTextHeadText)
				{
					return;
				}
				Global.LeaderGainTextHeadText.MaxCount = 6;
			}
			Global.LeaderGainTextHeadText.transform.localPosition = new Vector3(pointX, pointY, 0f);
			Global.LeaderGainTextHeadText.Add(text, picTextColor, moveTicks / 1000f, offsetX, fontSize);
		}

		public static void ShowText(GSprite sprite, double offsetX, double offsetY, Color picTextColor, string text, int numType, long numVal, int moveMode, double moveTicks, int direction = 2, HUDTextCustom.TextType textType = HUDTextCustom.TextType.Normal)
		{
			if (numType == 0)
			{
				sprite.AddHeadText(text + "+" + numVal, picTextColor, 0.5f, 0f, 0f, textType);
			}
			else if (numVal == 0L)
			{
				sprite.AddHeadText(text, picTextColor, 0.5f, 0f, 0f, textType);
			}
			else
			{
				sprite.AddHeadText(text + "-" + numVal, picTextColor, 0.5f, 0f, 0f, textType);
			}
		}

		public static void ShowText(GSprite sprite, Color picTextColor, string text, float moveTicks, float offsetX = 0f, float fontSize = 0f)
		{
			sprite.AddHeadText(text, picTextColor, moveTicks / 1000f, offsetX, fontSize, HUDTextCustom.TextType.Normal);
		}

		public static void ShowText(GSprite sprite, string text, float moveTicks)
		{
			sprite.AddHeadText(text, Color.white, moveTicks / 1000f, 0f, 0f, HUDTextCustom.TextType.Normal);
		}

		private static int GetDianjiangTeamID(int roleID)
		{
			if (Global.Data.CurrentDJRoomRolesData.Team1 != null)
			{
				for (int i = 0; i < Global.Data.CurrentDJRoomRolesData.Team1.Count; i++)
				{
					if (Global.Data.CurrentDJRoomRolesData.Team1[i].RoleID == roleID)
					{
						return 1;
					}
				}
			}
			return 2;
		}

		public static bool IsOwnZhaoHuanShou(int ID)
		{
			return Global.IsZhaoHuanShou(ID) && Global.Data.SystemMonsters[ID].MasterRoleID == Global.Data.RoleID;
		}

		public static bool IsZhaoHuanShou(int ID)
		{
			if (Global.Data.SystemMonsters.ContainsKey(ID) && 0 < Global.Data.SystemMonsters[ID].MasterRoleID)
			{
				if (Global.Data.SystemMonsters[ID].MasterRoleID == Global.Data.RoleID)
				{
					return true;
				}
				if (Global.Data.OtherRoles.ContainsKey(Global.Data.SystemMonsters[ID].MasterRoleID))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsOpposition(GSprite me, GSprite obj, bool safeRegion, int mapPKMode)
		{
			SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
			if (Global.Data.CurrentDJRoomRolesData != null && Global.GetMapType(Global.Data.roleData.MapCode) == MapTypes.DianJiangCopy && obj.SpriteType == GSpriteTypes.Other)
			{
				int dianjiangTeamID = Global.GetDianjiangTeamID(Global.Data.roleData.RoleID);
				int dianjiangTeamID2 = Global.GetDianjiangTeamID(obj.RoleID);
				return dianjiangTeamID != dianjiangTeamID2;
			}
			SceneUIClasses sceneUIClasses = mapSceneUIClass;
			switch (sceneUIClasses)
			{
			case SceneUIClasses.JingJiChang:
				if (obj.SpriteType == GSpriteTypes.Other || obj.SpriteType == GSpriteTypes.FakeRole)
				{
					return true;
				}
				break;
			default:
				if (sceneUIClasses == SceneUIClasses.Battle)
				{
					if (obj.SpriteType == GSpriteTypes.Other)
					{
						RoleData roleData;
						return Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData) && roleData.BattleWhichSide != me.BattleWhichSide;
					}
					if (obj.SpriteType == GSpriteTypes.Monster)
					{
						if (obj.MonsterType == MonsterTypes.XianFactionGuard)
						{
							return me.BattleWhichSide == 2;
						}
						if (obj.MonsterType == MonsterTypes.MoFactionGuard)
						{
							return me.BattleWhichSide == 1;
						}
					}
				}
				break;
			case SceneUIClasses.HuanYingSiYuan:
			case SceneUIClasses.TianTi:
			case SceneUIClasses.YongZheZhanChang:
			case SceneUIClasses.ElementWar:
			case SceneUIClasses.MoRiJudge:
			case SceneUIClasses.KuaFuBoss:
			case SceneUIClasses.CopyWolf:
			case SceneUIClasses.ZhongShenZhengBa:
			case SceneUIClasses.PKLovers:
			case SceneUIClasses.KuaFuWangZhe:
			case SceneUIClasses.ZhengDuoZhiDi:
			case SceneUIClasses.ZhanMengLianSaiBiSai:
			case SceneUIClasses.WanMoXiaGu:
			case SceneUIClasses.KuaFuTeamCompete:
			case SceneUIClasses.KuaFuTeamCompeteZhengBa:
			case SceneUIClasses.MoYuDuoBao:
			case SceneUIClasses.DaTaoSha:
				if (obj.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData2;
					return Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData2) && roleData2.BattleWhichSide != me.BattleWhichSide;
				}
				break;
			case SceneUIClasses.LoveFuBen:
				if (obj.SpriteType == GSpriteTypes.Other || obj.SpriteType == GSpriteTypes.FakeRole)
				{
					return false;
				}
				break;
			case SceneUIClasses.LingDiCaiJi:
			{
				bool result = true;
				if (obj.SpriteType == GSpriteTypes.Monster)
				{
					if (obj.MonsterType == MonsterTypes.LingDiDiGongShouWei || obj.MonsterType == MonsterTypes.LingDiHuangMoShouWei)
					{
						if (Global.Data.roleData.BufferDataList != null)
						{
							for (int i = 0; i < Global.Data.roleData.BufferDataList.Count; i++)
							{
								if (Global.Data.roleData.BufferDataList[i].BufferID == 115)
								{
									result = false;
									break;
								}
							}
						}
					}
					else if (obj.MonsterType == MonsterTypes.CaiJi || obj.MonsterType == MonsterTypes.CaiJiByTime)
					{
						result = false;
					}
					return result;
				}
				if (obj.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData3 = null;
					if (!Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData3))
					{
						result = true;
					}
					if (roleData3 != null)
					{
						result = ((Global.Data.roleData.JunTuanId == 0 && roleData3.JunTuanId == 0) || Global.Data.roleData.JunTuanId != roleData3.JunTuanId);
					}
					return result;
				}
				break;
			}
			case SceneUIClasses.KuaFuPlunderBattle:
				if (obj.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData4;
					return Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData4) && roleData4.BattleWhichSide != me.BattleWhichSide;
				}
				break;
			case SceneUIClasses.Comp:
			case SceneUIClasses.CompBattle:
			case SceneUIClasses.CompBattleMiDong:
				if (obj.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData5;
					return Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData5) && roleData5.CompType != Global.Data.roleData.CompType;
				}
				if (obj.SpriteType == GSpriteTypes.Monster && Global.Data.SystemMonsters != null)
				{
					MonsterData value = Global.Data.SystemMonsters.GetValue(obj.RoleID);
					if (value != null && Global.Data.OtherRoles.ContainsKey(value.MasterRoleID) && value != null && Global.Data.OtherRoles[value.MasterRoleID].CompType == Global.Data.roleData.CompType)
					{
						return false;
					}
				}
				break;
			case SceneUIClasses.RebornMap:
				if (obj.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData6;
					return Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData6) && Global.Data.roleData.PKMode != 0 && (!Global.IsHavingBangHui(Global.Data.roleData) || !Global.IsHavingBangHui(roleData6) || roleData6.Faction != Global.Data.roleData.Faction);
				}
				if (obj.SpriteType == GSpriteTypes.Monster && Global.Data.SystemMonsters != null)
				{
					MonsterData value2 = Global.Data.SystemMonsters.GetValue(obj.RoleID);
					if (value2 != null)
					{
						RoleData roleData7;
						if (Global.Data.OtherRoles.TryGetValue(value2.MasterRoleID, ref roleData7))
						{
							if (Global.Data.roleData.PKMode == 0)
							{
								return false;
							}
							if (Global.IsHavingBangHui(Global.Data.roleData) && Global.IsHavingBangHui(roleData7) && roleData7.Faction == Global.Data.roleData.Faction)
							{
								return false;
							}
						}
						return true;
					}
				}
				break;
			}
			return (mapSceneUIClass == SceneUIClasses.PKKing && obj.SpriteType == GSpriteTypes.Other) || Global._IsOpposition(me, obj, safeRegion, mapPKMode);
		}

		private static bool _IsOpposition(GSprite me, GSprite obj, bool safeRegion, int mapPKMode)
		{
			if (me == obj)
			{
				return false;
			}
			if (obj == null)
			{
				return false;
			}
			if (obj.SpriteType == GSpriteTypes.NPC)
			{
				return false;
			}
			if (obj.SpriteType == GSpriteTypes.BiaoChe)
			{
				BiaoCheData biaoCheData = null;
				if (!Global.Data.BiaoChes.TryGetValue(obj.RoleID, ref biaoCheData))
				{
					return false;
				}
				if (biaoCheData.OwnerRoleID == me.RoleID)
				{
					return false;
				}
			}
			if (obj.SpriteType == GSpriteTypes.JunQi)
			{
				JunQiData junQiData = null;
				if (!Global.Data.JunQis.TryGetValue(obj.RoleID, ref junQiData))
				{
					return false;
				}
				if (junQiData.BHID == me.FactionID)
				{
					return false;
				}
			}
			if (obj.SpriteType == GSpriteTypes.FakeRole)
			{
				FakeRoleData fakeRoleData = null;
				if (!Global.Data.FakeRoles.TryGetValue(obj.RoleID, ref fakeRoleData))
				{
					return false;
				}
				if (fakeRoleData.FakeRoleType != 2)
				{
					return false;
				}
				if (fakeRoleData.MyRoleDataMini.Faction == me.FactionID)
				{
				}
			}
			if (obj.VLife <= 0.0)
			{
				return false;
			}
			if (obj.SpriteType == GSpriteTypes.Monster)
			{
				MonsterData value = Global.Data.SystemMonsters.GetValue(obj.RoleID);
				if (value != null)
				{
					if (value.MonsterType == 1101 || value.MonsterType == 1601)
					{
						return false;
					}
					if (value.BattleWitchSide != 0 && value.BattleWitchSide == Global.Data.roleData.BattleWhichSide)
					{
						return false;
					}
				}
				if (!Global.IsZhaoHuanShou(obj.RoleID))
				{
					return !Global.Data.SystemMonsters.ContainsKey(obj.RoleID) || 0 >= Global.Data.SystemMonsters[obj.RoleID].MasterRoleID || Global.Data.OtherRoles.ContainsKey(Global.Data.SystemMonsters[obj.RoleID].MasterRoleID);
				}
				if (Global.IsOwnZhaoHuanShou(obj.RoleID))
				{
					return false;
				}
			}
			if (mapPKMode == 2)
			{
				return false;
			}
			if (Global.GetUnionLevel(Global.Data.roleData.ChangeLifeCount, Global.Data.roleData.Level) < 60)
			{
				return false;
			}
			int num = -1;
			int num2 = 0;
			string text = null;
			int num3 = 0;
			int num4 = -1;
			RoleData roleData = null;
			if (obj.SpriteType == GSpriteTypes.Other)
			{
				if (!Global.Data.OtherRoles.TryGetValue(obj.RoleID, ref roleData))
				{
					return false;
				}
				if (Global.GetUnionLevel(roleData.ChangeLifeCount, roleData.Level) < 60)
				{
					return false;
				}
				num = roleData.TeamID;
				num2 = roleData.PKPoint;
				text = roleData.BHName;
				num3 = roleData.JunTuanId;
				num4 = roleData.PTID;
			}
			else if (obj.SpriteType == GSpriteTypes.FakeRole)
			{
				FakeRoleData fakeRoleData2 = null;
				if (!Global.Data.FakeRoles.TryGetValue(obj.RoleID, ref fakeRoleData2))
				{
					return false;
				}
				if (Global.GetUnionLevel(fakeRoleData2.MyRoleDataMini.ChangeLifeCount, fakeRoleData2.MyRoleDataMini.Level) < 60)
				{
					return false;
				}
				num = fakeRoleData2.MyRoleDataMini.TeamID;
				text = fakeRoleData2.MyRoleDataMini.BHName;
				num3 = fakeRoleData2.MyRoleDataMini.JunTuanId;
				num4 = fakeRoleData2.MyRoleDataMini.PTID;
			}
			else if (!Global.IsOwnZhaoHuanShou(obj.RoleID) && Global.Data.SystemMonsters.ContainsKey(obj.RoleID) && Global.Data.OtherRoles.ContainsKey(Global.Data.SystemMonsters[obj.RoleID].MasterRoleID))
			{
				num = Global.Data.OtherRoles[Global.Data.SystemMonsters[obj.RoleID].MasterRoleID].TeamID;
				text = Global.Data.OtherRoles[Global.Data.SystemMonsters[obj.RoleID].MasterRoleID].BHName;
				num2 = Global.Data.OtherRoles[Global.Data.SystemMonsters[obj.RoleID].MasterRoleID].PKPoint;
				num3 = Global.Data.OtherRoles[Global.Data.SystemMonsters[obj.RoleID].MasterRoleID].JunTuanId;
				num4 = Global.Data.OtherRoles[Global.Data.SystemMonsters[obj.RoleID].MasterRoleID].PTID;
			}
			switch (me.PKMode)
			{
			case GPKModes.Normal:
				return mapPKMode > 0;
			case GPKModes.Whole:
				return true;
			case GPKModes.Faction:
				return me.VFaction == string.Empty || Global.Data.roleData.BHName != text;
			case GPKModes.Team:
				return Global.Data.roleData.TeamID == 0 || Global.Data.roleData.TeamID != num;
			case GPKModes.Kind:
				return num2 >= 200 || ColorSL.FromArgb(255, 255, 255, 255) != me.SNameBrush.Color;
			case GPKModes.ArmyGroup:
				return Global.Data.roleData.JunTuanId == 0 || num3 != Global.Data.roleData.JunTuanId;
			case GPKModes.Platform:
				return num4 != Global.Data.roleData.PTID;
			}
			return false;
		}

		public static int GMode(int num1, int num2)
		{
			if (num2 == 0)
			{
				return 0;
			}
			return num1 % num2;
		}

		public static int GMin(int l, int r)
		{
			return HSGameEngine.GameEngine.Common.MathUtils.GMin(l, r);
		}

		public static int GMax(int l, int r)
		{
			return HSGameEngine.GameEngine.Common.MathUtils.GMax(l, r);
		}

		public static double GMin(double l, double r)
		{
			return HSGameEngine.GameEngine.Common.MathUtils.GMin(l, r);
		}

		public static double GMax(double l, double r)
		{
			return HSGameEngine.GameEngine.Common.MathUtils.GMax(l, r);
		}

		public static int SafeConvertToInt32(string str)
		{
			return ConvertExt.SafeConvertToInt32(str);
		}

		public static string GetTimeStrBySecFilterZero(int sec, bool fixNumLen = true, int MaxShowNum = -1)
		{
			string text = string.Empty;
			int num = 86400;
			int num2 = 3600;
			int num3 = 60;
			if (num == 0 || num2 == 0 || num3 == 0)
			{
				return text;
			}
			int[] array = new int[]
			{
				sec / num,
				sec % num / num2,
				sec % num % num2 / num3,
				sec % num % num2 % num3
			};
			string[] array2 = new string[]
			{
				Global.GetLang("天"),
				Global.GetLang("小时"),
				Global.GetLang("分"),
				Global.GetLang("秒")
			};
			List<int> list = Enumerable.ToList<int>(array);
			List<string> list2 = Enumerable.ToList<string>(array2);
			while (list.Count > 0 && list[0] == 0)
			{
				list.RemoveAt(0);
				list2.RemoveAt(0);
			}
			int num4 = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (0 < list[0])
				{
					if (fixNumLen)
					{
						text += ((list[i] >= 10) ? list[i].ToString() : ("0" + list[i].ToString()));
					}
					else
					{
						text += list[i].ToString();
					}
					num4++;
					text += list2[i];
					if (0 < MaxShowNum && num4 >= MaxShowNum)
					{
						break;
					}
				}
			}
			return text;
		}

		public static long SafeConvertToTicks(string strDateTime)
		{
			return ConvertExt.SafeConvertToTicks(strDateTime);
		}

		public static string GetTimeStrBySec(double sec, bool showDay = true)
		{
			int num = 86400;
			int num2 = 3600;
			int num3 = 60;
			if (num == 0 || num2 == 0 || num3 == 0)
			{
				return string.Empty;
			}
			if (!showDay)
			{
				return StringUtil.substitute(Global.GetLang("{1}小时{2}分{3}秒"), new object[]
				{
					(int)(sec % (double)num / (double)num2),
					(int)(sec % (double)num % (double)num2 / (double)num3),
					(int)(sec % (double)num % (double)num2 % (double)num3)
				});
			}
			return StringUtil.substitute(Global.GetLang("{0}天{1}小时{2}分{3}秒"), new object[]
			{
				(int)(sec / (double)num),
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3),
				(int)(sec % (double)num % (double)num2 % (double)num3)
			});
		}

		public static string GetTimeStrBySecEx(double sec, bool fixNumLen = true, int MaxShowNum = -1)
		{
			int num = 86400;
			int num2 = 3600;
			int num3 = 60;
			if (num == 0 || num2 == 0 || num3 == 0)
			{
				return string.Empty;
			}
			int[] array = new int[]
			{
				(int)(sec / (double)num),
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3),
				(int)(sec % (double)num % (double)num2 % (double)num3)
			};
			string[] array2 = new string[]
			{
				Global.GetLang("天"),
				Global.GetLang("小时"),
				Global.GetLang("分"),
				Global.GetLang("秒")
			};
			List<int> list = Enumerable.ToList<int>(array);
			List<string> list2 = Enumerable.ToList<string>(array2);
			while (list.Count > 0 && list[0] == 0)
			{
				list.RemoveAt(0);
				list2.RemoveAt(0);
			}
			string text = string.Empty;
			int num4 = list.Count;
			if (0 < MaxShowNum && MaxShowNum < list.Count)
			{
				num4 = MaxShowNum;
			}
			for (int i = 0; i < num4; i++)
			{
				if (fixNumLen)
				{
					text += ((list[i] >= 10) ? list[i].ToString() : ("0" + list[i].ToString()));
				}
				else
				{
					text += list[i].ToString();
				}
				text += list2[i];
			}
			return text;
		}

		public static double SafeConvertToDouble(string str)
		{
			return ConvertExt.SafeConvertToDouble(str);
		}

		public static double[] String2DoubleArray(string str, char ch)
		{
			return ConvertExt.String2DoubleArray(str, ch);
		}

		public static int[] String2IntArray(string str, char ch)
		{
			return ConvertExt.String2IntArray(str, ch);
		}

		public static List<int> String2IntList(string str, char ch)
		{
			return ConvertExt.String2IntList(str, ch);
		}

		public static double[] StringArray2DoubleArray(string[] sa)
		{
			return ConvertExt.StringArray2DoubleArray(sa);
		}

		public static int[] StringArray2IntArray(string[] sa)
		{
			return ConvertExt.StringArray2IntArray(sa);
		}

		public static Point[] StrToPointArray(string str)
		{
			return ConvertExt.StrToPointArray(str);
		}

		public static Point[] StrToPointArray2(string str)
		{
			return ConvertExt.StrToPointArray2(str);
		}

		public static DateTime SafeConvertDateTime(string str)
		{
			DateTime result = default(DateTime);
			DateTime.TryParse(str, ref result);
			return result;
		}

		public static long GetTimeStamp()
		{
			return TimeManager.GetTimeStamp();
		}

		public static long GetCorrectLocalTime()
		{
			return TimeManager.GetCorrectLocalTime();
		}

		public static DateTime GetCorrectDateTime()
		{
			return TimeManager.GetCorrectDateTime();
		}

		public static void SetLocalTimeSubServerTime(long subTicks)
		{
			if (Global.myDateTimeTicksBak != 0L)
			{
				long localTimeSubServerTime = TimeManager.LocalTimeSubServerTime;
				long num = localTimeSubServerTime - subTicks;
				long num2 = ConfigSystemParam.GetSystemParamIntByName("SetLocalTimeSubServerTime");
				if (num2 <= 1L)
				{
					num2 = 10L;
				}
				if (Mathf.Abs((float)num) > (float)(num2 * 10000000L))
				{
					string text = "!TIMER TICK JUNP. preTime=";
					DateTime dateTime;
					dateTime..ctor(Global.GetCorrectLocalTime() * 10000L);
					string text2 = text + dateTime.ToString();
					string text3 = text2;
					text2 = string.Concat(new object[]
					{
						text3,
						"subTicks pre/now=",
						localTimeSubServerTime,
						"/",
						subTicks
					});
					text3 = text2;
					text2 = string.Concat(new object[]
					{
						text3,
						"MyDateTimeTicks pre/now=",
						Global.myDateTimeTicksBak,
						"/",
						MyDateTime.Now().Ticks
					});
					MUDebug.LogError<string>(new string[]
					{
						text2
					});
					if (MyDateTime.Now().Ticks - Global.myDateTimeTicksBak > 700000L)
					{
					}
				}
			}
			TimeManager.LocalTimeSubServerTime = subTicks;
			Global.myDateTimeTicksBak = MyDateTime.Now().Ticks;
		}

		public static bool IsWeekend()
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			return correctDateTime.DayOfWeek == 6 || correctDateTime.DayOfWeek == 0;
		}

		public static long GetFangZhiJiaSuTime()
		{
			return Global.GetCorrectLocalTime();
		}

		public static bool IsTargetDay(int day)
		{
			if (day <= 0 || day > 7)
			{
				return false;
			}
			DayOfWeek dayOfWeek = (day != 7) ? day : 0;
			return Global.GetCorrectDateTime().DayOfWeek == dayOfWeek;
		}

		public static bool IsInWeekendRechargePeriod()
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ZhouMoChongZhiTime", '|');
			if (systemParamStringArrayByName.Length <= 0)
			{
				return false;
			}
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				if (array.Length > 0)
				{
					if (Global.IsTargetDay(Global.SafeConvertToInt32(array[0])))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void SetPartialSafeRegion(Point grid, int gridNum)
		{
			if (Global.CurrentMapData == null)
			{
				return;
			}
			byte[,] terrainWithTeleports = Global.CurrentMapData.TerrainWithTeleports;
			int num = Math.Max(0, grid.X - gridNum);
			int num2 = Math.Max(0, grid.Y - gridNum);
			int num3 = Math.Min(Global.CurrentMapData.GridSizeXNum - 1, grid.X + gridNum);
			int num4 = Math.Min(Global.CurrentMapData.GridSizeYNum - 1, grid.Y + gridNum);
			for (int i = num; i <= num3; i++)
			{
				for (int j = num2; j <= num4; j++)
				{
					terrainWithTeleports[i, j] = byte.MaxValue;
				}
			}
		}

		public static void SetPartialCollideRegion(Point grid, XElement npcXmlNode)
		{
			if (Global.CurrentMapData == null)
			{
				return;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(npcXmlNode, "ID");
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(xelementAttributeInt);
			if (npcvobyID != null && (npcvobyID.ObstacleX != 0 || npcvobyID.ObstacleY != 0))
			{
				int obstacleX = npcvobyID.ObstacleX;
				int obstacleY = npcvobyID.ObstacleY;
				byte[,] fixedObstruction = Global.CurrentMapData.fixedObstruction;
				int num = Math.Max(0, grid.X - obstacleX);
				int num2 = Math.Max(0, grid.Y - obstacleY);
				int num3 = Math.Min(Global.CurrentMapData.GridSizeXNum - 1, grid.X + obstacleX);
				int num4 = Math.Min(Global.CurrentMapData.GridSizeYNum - 1, grid.Y + obstacleY);
				for (int i = num; i <= num3; i++)
				{
					for (int j = num2; j <= num4; j++)
					{
						fixedObstruction[i, j] = 0;
					}
				}
			}
		}

		public static GDecoration GetDecoration(int code, GDecorationTypes decoType, Point pos, bool toGround = false, string ownerName = null, int triggerType = -1, int layer = -1, bool forceSyncLoad = true, bool isCache = false)
		{
			DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(code);
			if (decorationVOByCode == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"YN_Log，产品缺少特效配置code=" + code + ";请检查！"
				});
			}
			string resName = decorationVOByCode.ResName;
			GDecoration gdecoration = new GDecoration(resName);
			gdecoration.OrigCoordinate = new Point(pos.X, pos.Y);
			gdecoration.ToGround = toGround;
			if (!toGround)
			{
				gdecoration.cx = pos.X;
				gdecoration.cy = pos.Y;
			}
			else
			{
				Vector3 groundPosition = U3DUtils.GetGroundPosition2((float)pos.X / 100f, (float)pos.Y / 100f, 50f);
				Vector3 position3D;
				position3D..ctor((float)pos.X / 100f, groundPosition.y, (float)pos.Y / 100f);
				gdecoration.Position3D = position3D;
			}
			gdecoration.DecorationType = decoType;
			gdecoration.OwnerName = ownerName;
			gdecoration.TriggerType = triggerType;
			gdecoration.Layer = layer;
			gdecoration.HangPos = decorationVOByCode.HangPos;
			gdecoration.SoundFileName = decorationVOByCode.Sound;
			gdecoration.ForceSyncLoad = forceSyncLoad;
			gdecoration.IsCache = isCache;
			gdecoration.Start();
			return gdecoration;
		}

		public static GDecoration GetDecoration(int code, GDecorationTypes decoType, GSprite sprite, string ToObjectName, int layer = -1, bool forceSyncLoad = true)
		{
			DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(code);
			string resName = decorationVOByCode.ResName;
			GDecoration gdecoration = new GDecoration(resName);
			int num = 0;
			int num2 = 0;
			GameObject gameObject = sprite.Find3DObject(ToObjectName);
			if (null != gameObject)
			{
				num = (int)gameObject.transform.localPosition.x;
				num2 = (int)gameObject.transform.localPosition.z;
			}
			gdecoration.OrigCoordinate = new Point(num, num2);
			gdecoration.cx = num;
			gdecoration.cy = num2;
			gdecoration.DecorationType = decoType;
			gdecoration.Layer = layer;
			gdecoration.HangPos = decorationVOByCode.HangPos;
			gdecoration.ForceSyncLoad = forceSyncLoad;
			gdecoration.Start();
			return gdecoration;
		}

		public static GTeleport GetTeleport(XElement teleXml, int codeID = 0)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(teleXml, "Code");
			if (xelementAttributeInt == -1)
			{
				return null;
			}
			DecorationVO decorationVOByCode;
			if (codeID == 0)
			{
				decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(xelementAttributeInt);
			}
			else
			{
				decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(codeID);
			}
			if (decorationVOByCode == null)
			{
				return null;
			}
			return new GTeleport(decorationVOByCode.ResName)
			{
				Name = StringUtil.substitute("Teleport{0}", new object[]
				{
					Global.GetXElementAttributeInt(teleXml, "Key")
				}),
				Key = (byte)Global.GetXElementAttributeInt(teleXml, "Key"),
				To = Global.GetXElementAttributeInt(teleXml, "To"),
				ToX = Global.GetXElementAttributeDouble(teleXml, "ToX"),
				ToY = Global.GetXElementAttributeDouble(teleXml, "ToY"),
				ToDirection = Global.GetXElementAttributeDouble(teleXml, "ToDirection"),
				cx = (int)Global.GetXElementAttributeDouble(teleXml, "X"),
				cy = (int)Global.GetXElementAttributeDouble(teleXml, "Y"),
				Radius = Global.GetXElementAttributeDouble(teleXml, "Radius"),
				Tip = Global.GetXElementAttributeStr(teleXml, "Tip")
			};
		}

		public static Dictionary<int, ConfigLuolanFazhen> GetLuolanFazhenConfig()
		{
			XElement gameResXml = Global.GetGameResXml("Config/LuoLanFaZhen.xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "FuBen");
			Dictionary<int, ConfigLuolanFazhen> dictionary = new Dictionary<int, ConfigLuolanFazhen>();
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement == null)
				{
					return null;
				}
				ConfigLuolanFazhen configLuolanFazhen = new ConfigLuolanFazhen();
				configLuolanFazhen.ID = Global.GetXElementAttributeInt(xelement, "ID");
				configLuolanFazhen.MapID = Global.GetXElementAttributeInt(xelement, "MapID");
				configLuolanFazhen.ChuanSongMenID = Global.GetXElementAttributeStr(xelement, "ChuanSongMenID");
				configLuolanFazhen.MapToMen = Global.GetXElementAttributeStr(xelement, "MuDidiID");
				configLuolanFazhen.TeShuMapID = Global.GetXElementAttributeStr(xelement, "TeShuMapID");
				if (!dictionary.ContainsKey(configLuolanFazhen.MapID))
				{
					dictionary.Add(configLuolanFazhen.MapID, configLuolanFazhen);
				}
			}
			return dictionary;
		}

		public static int GetLuolanchengzhanTime()
		{
			XElement gameResXml = Global.GetGameResXml("Config/SiegeWarfare.Xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[0], "FightingSecs");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[0], "TimePoints");
			DateTime correctDateTime = Global.GetCorrectDateTime();
			string text = correctDateTime.ToShortDateString();
			DateTime dateTime = Convert.ToDateTime(text + " " + xelementAttributeStr).AddSeconds((double)xelementAttributeInt);
			return (int)(dateTime - correctDateTime).TotalSeconds;
		}

		public static GGuangMuData GetGuangMu(XElement teleXml)
		{
			GGuangMuData gguangMuData = new GGuangMuData();
			gguangMuData.ID = Global.GetXElementAttributeInt(teleXml, "ID");
			gguangMuData.Show = Global.GetXElementAttributeInt(teleXml, "Show");
			gguangMuData.Path = Global.GetXElementAttributeStr(teleXml, "Path");
			gguangMuData.Description = Global.GetXElementAttributeStr(teleXml, "Description");
			gguangMuData.Animation = Global.GetXElementAttributeStr(teleXml, "Animation");
			int[] xelementAttributeIntArray = Global.GetXElementAttributeIntArray(teleXml, "Pos", "*");
			if (xelementAttributeIntArray.Length == 3 && Global.CurrentMapData.MapWidth != 0 && Global.CurrentMapData.MapHeight != 0)
			{
				gguangMuData.Pos = new Vector3((float)xelementAttributeIntArray[0] / (float)Global.CurrentMapData.MapWidth, (float)xelementAttributeIntArray[1] / (float)Global.CurrentMapData.MapHeight, (float)xelementAttributeIntArray[2] / 100f);
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(teleXml, "ZuDang");
			gguangMuData.ZuDangs = ConvertExt.StrToPointArray2(xelementAttributeStr);
			if (gguangMuData.ZuDangs != null)
			{
				if (Global.CurrentMapData.GridSizeX == 0 || Global.CurrentMapData.GridSizeY == 0)
				{
					return gguangMuData;
				}
				for (int i = 0; i < gguangMuData.ZuDangs.Length; i++)
				{
					gguangMuData.ZuDangs[i].X = gguangMuData.ZuDangs[i].X / Global.CurrentMapData.GridSizeX;
					gguangMuData.ZuDangs[i].Y = gguangMuData.ZuDangs[i].Y / Global.CurrentMapData.GridSizeX;
				}
			}
			return gguangMuData;
		}

		public static GGoodsPack GetGoodsPack(int autoID, int goodsPackID, int owerRoleID, string owerRoleName, Point pos, int goodsID, int goodsNum, long productTicks, long teamID, string teamRoleIDs)
		{
			GGoodsPack ggoodsPack = new GGoodsPack();
			ggoodsPack.AutoID = autoID;
			ggoodsPack.Name = StringUtil.substitute("GoodsPack{0}", new object[]
			{
				autoID
			});
			ggoodsPack.OwnerText = StringUtil.substitute("{0}", new object[]
			{
				Global.GetGoodsNameByID(goodsID, false)
			});
			ggoodsPack.Coordinate = pos;
			ggoodsPack.GoodsPackID = goodsPackID;
			ggoodsPack.OwnerRoleID = owerRoleID;
			ggoodsPack.ProduceTicks = productTicks;
			ggoodsPack.TeamID = teamID;
			int[] array = Global.String2IntArray(teamRoleIDs, ',');
			if (array != null)
			{
				ggoodsPack.TeamRoleIDs = Enumerable.ToList<int>(array);
			}
			ggoodsPack.GoodsID = goodsID;
			ggoodsPack.GoodsNum = goodsNum;
			return ggoodsPack;
		}

		public static bool CanAutoGetThing(GGoodsPack goodsPack)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsPack.GoodsID);
			if (categoriyByGoodsID == 501)
			{
				if (Global.Data.AutoFightData.AutoGetMoney)
				{
					return true;
				}
			}
			else if (categoriyByGoodsID == 180)
			{
				if (Global.Data.AutoFightData.AutoGetDrugs)
				{
					return true;
				}
			}
			else if (Global.Data.AutoFightData.AutoGetThings)
			{
				return true;
			}
			return false;
		}

		public static bool CanOpenGoodsPack(GGoodsPack goodsPack)
		{
			double num = (double)Global.GetCorrectLocalTime();
			if (num - (double)goodsPack.ProduceTicks >= (double)(Global.Data.GoodsPackOvertimeTick * 1000))
			{
				return true;
			}
			if (goodsPack.TeamRoleIDs != null && goodsPack.TeamRoleIDs.Count > 1 && Global.Data.CurrentTeamData != null)
			{
				int roleID = Global.Data.roleData.RoleID;
				if (goodsPack.TeamRoleIDs.IndexOf(roleID) != -1)
				{
					bool flag = true;
					if (Global.IsInArenaBattleMap())
					{
						flag = false;
					}
					if (Global.Data.CurrentTeamData.GetThingOpt > 0 && flag)
					{
						return true;
					}
				}
			}
			return goodsPack.OwnerRoleID == Global.Data.roleData.RoleID || goodsPack.OwnerRoleID <= 0;
		}

		public static GGoodsPack FindGoodsPackByGridXY(int gridX, int gridY)
		{
			if (Global.CurrentMapData == null)
			{
				return null;
			}
			List<object> list = Global.CurrentMapData._MapGrid.FindObjects(gridX, gridY);
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] is GGoodsPack)
				{
					return list[i] as GGoodsPack;
				}
			}
			return null;
		}

		public static SkillData GetSkillDataByID(int skillID)
		{
			if (!ConfigMagicInfos.IsHorseSkill(skillID))
			{
				if (ShenHunData.IsInBianShenState())
				{
					List<int> selfBianShenSkill = ShenHunData.GetSelfBianShenSkill();
					for (int i = 0; i < selfBianShenSkill.Count; i++)
					{
						if (selfBianShenSkill[i] == skillID)
						{
							return new SkillData
							{
								SkillID = skillID,
								SkillLevel = ShenHunData.GetSelfShenHunLevel().BianShen
							};
						}
					}
				}
				if (Global.Data.roleData.SkillDataList != null && 0 < Global.Data.roleData.SkillDataList.Count)
				{
					for (int j = 0; j < Global.Data.roleData.SkillDataList.Count; j++)
					{
						if (Global.Data.roleData.SkillDataList[j].SkillID == skillID)
						{
							return Global.Data.roleData.SkillDataList[j];
						}
					}
				}
			}
			else
			{
				List<HorseSkillData> roleHorseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID);
				if (0 < roleHorseSkillData.Count)
				{
					for (int k = 0; k < roleHorseSkillData.Count; k++)
					{
						if (roleHorseSkillData[k].SkillID == skillID)
						{
							return new SkillData
							{
								DbID = -1,
								SkillID = roleHorseSkillData[k].SkillID,
								SkillLevel = roleHorseSkillData[k].SkillLevel
							};
						}
					}
				}
			}
			return null;
		}

		public static SkillData GetSkillDataByDbID(int skillDbID)
		{
			if (Global.Data.roleData.SkillDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.SkillDataList.Count; i++)
			{
				if (Global.Data.roleData.SkillDataList[i].DbID == skillDbID)
				{
					return Global.Data.roleData.SkillDataList[i];
				}
			}
			return null;
		}

		public static void AddSkillData(int skillDbID, int skillID, int skillLevel)
		{
			if (Global.Data.roleData.SkillDataList == null)
			{
				Global.Data.roleData.SkillDataList = new List<SkillData>();
			}
			SkillData skillData = new SkillData
			{
				DbID = skillDbID,
				SkillID = skillID,
				SkillLevel = skillLevel
			};
			Global.Data.roleData.SkillDataList.Add(skillData);
		}

		public static List<int> GetManuLearnSkill()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, MagicInfoVO> keyValuePair in ConfigMagicInfos.GetMaigcInfoVODict())
			{
				MagicInfoVO value = keyValuePair.Value;
				if (value.AutoStart == 0)
				{
					int toOcuupation = value.ToOcuupation;
					if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == toOcuupation)
					{
						list.Add(value.ID);
					}
				}
			}
			return list;
		}

		public static bool HasNotLearnManuSkill()
		{
			List<int> manuLearnSkill = Global.GetManuLearnSkill();
			for (int i = 0; i < manuLearnSkill.Count; i++)
			{
				if (Global.GetSkillDataByID(manuLearnSkill[i]) == null)
				{
					return true;
				}
			}
			return false;
		}

		public static MagicInfoVO GetSkillXmlNode(int id)
		{
			return ConfigMagicInfos.GetMaigcInfoVOByCode(id);
		}

		public static List<MagicInfoVO> GetSkillListByOccupation(int occupation)
		{
			List<int> occupationsInherit = Global.GetOccupationsInherit(occupation);
			List<HorseSkillData> roleHorseSkillData = Global.GetRoleHorseSkillData(Global.Data.RoleID);
			List<MagicInfoVO> list = new List<MagicInfoVO>();
			foreach (KeyValuePair<int, MagicInfoVO> keyValuePair in ConfigMagicInfos.GetMaigcInfoVODict())
			{
				MagicInfoVO value = keyValuePair.Value;
				for (int i = 0; i < occupationsInherit.Count; i++)
				{
					if (value.ToOcuupation == occupationsInherit[i])
					{
						list.Add(value);
						break;
					}
				}
				if (0 < roleHorseSkillData.Count)
				{
					for (int j = 0; j < roleHorseSkillData.Count; j++)
					{
						if (roleHorseSkillData[j].SkillID == value.ID)
						{
							list.Add(value);
							break;
						}
					}
				}
			}
			return list;
		}

		public static bool GetUpLeveConditions(int skillid, int skilllev, int skillUsedNum, bool isBtn = false)
		{
			double num = Convert.ToDouble(skillUsedNum);
			MagicItemVO magicItemVO = ConfigMagics.GetMagicItemVO(Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation), skillid, skilllev);
			if (magicItemVO == null)
			{
				return false;
			}
			if (skilllev >= 100)
			{
				return false;
			}
			if (((double)magicItemVO.ShuLianDu - num) / 5.0 > (double)Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan))
			{
				return false;
			}
			if (magicItemVO.NeedJinBi > Global.GetRoleOwnNumByMoneyType(8))
			{
				return false;
			}
			if (Global.Data.roleData.ChangeLifeCount < magicItemVO.NeedZhuanSheng)
			{
				return false;
			}
			if (Global.Data.roleData.ChangeLifeCount == magicItemVO.NeedZhuanSheng && Global.Data.roleData.Level < magicItemVO.NeedRoleLevel)
			{
				return false;
			}
			if (isBtn)
			{
				int num2 = (int)(((double)magicItemVO.ShuLianDu - num) / 5.0);
				if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) < num2)
				{
					return false;
				}
			}
			else if (num < (double)magicItemVO.ShuLianDu)
			{
				return false;
			}
			return true;
		}

		public static bool CanUseSkill(int occupation, int skillTypeID)
		{
			return Global.GetSkillDataByID(skillTypeID) != null;
		}

		public static int GetSkillIDByBook(int occupation, int bookGoodsID)
		{
			if (!ConfigMagicInfos.IsValid())
			{
				return -1;
			}
			foreach (KeyValuePair<int, MagicInfoVO> keyValuePair in ConfigMagicInfos.GetMaigcInfoVODict())
			{
				MagicInfoVO value = keyValuePair.Value;
				int toOcuupation = value.ToOcuupation;
				if (toOcuupation == occupation)
				{
					int magicsBook = value.MagicsBook;
					if (magicsBook == bookGoodsID)
					{
						return value.ID;
					}
				}
			}
			return -1;
		}

		public static int GetNumSkillID()
		{
			if (Global.Data.roleData.SkillDataList == null)
			{
				return -1;
			}
			bool flag = false;
			if (Global.Data.roleData.NumSkillID <= 0)
			{
				flag = true;
			}
			for (int i = 0; i < Global.Data.roleData.SkillDataList.Count; i++)
			{
				int skillID = Global.Data.roleData.SkillDataList[i].SkillID;
				MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
				if (skillXmlNode != null && skillXmlNode.MagicType < 0 && (flag || Global.Data.roleData.NumSkillID == skillID))
				{
					return skillID;
				}
			}
			return -1;
		}

		public static void GetUpSkillLearCondition(int skillID, int skillLevel, out int RoleLevel, out int ShuLianDu, MagicInfoVO skillXmlNode = null)
		{
			RoleLevel = 1;
			ShuLianDu = 1;
			if (skillXmlNode == null)
			{
				skillXmlNode = Global.GetSkillXmlNode(skillID);
			}
			if (skillXmlNode == null)
			{
				return;
			}
			string learnCondition = skillXmlNode.LearnCondition;
			if (string.IsNullOrEmpty(learnCondition))
			{
				return;
			}
			string[] array = learnCondition.Split(new char[]
			{
				'|'
			});
			if (skillLevel > array.Length)
			{
				return;
			}
			string[] array2 = array[skillLevel - 1].Split(new char[]
			{
				','
			});
			if (array2.Length != 3)
			{
				return;
			}
			RoleLevel = Convert.ToInt32(array2[1]);
			ShuLianDu = Convert.ToInt32(array2[2]);
		}

		public static int GetSkillUsedMagicV(int occupation, int skillID, int skillLevel)
		{
			string text = string.Format("{0}_{1}_{2}", occupation, skillID, skillLevel);
			if (Global.MagicVDict.ContainsKey(text))
			{
				return Global.MagicVDict[text];
			}
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillID);
			if (skillXmlNode == null)
			{
				Global.MagicVDict[text] = 0;
				return 0;
			}
			int baseMagic = skillXmlNode.BaseMagic;
			Global.MagicVDict[text] = baseMagic;
			return Global.MagicVDict[text];
		}

		public static int GetBaseSkillID(int occupation)
		{
			if (occupation != 3)
			{
				return Global.BaseSkillIDs[Global.CalcOriginalOccupationID(occupation)];
			}
			if (Global.GetMJSType() == MJSSkillType.Strength_Sword)
			{
				return 10000;
			}
			return 10100;
		}

		public static int GetDefaultSkillID(int occupation, int preferSkill = -1)
		{
			if (preferSkill > 0 && Global.GetSkillDataByID(preferSkill) != null)
			{
				return preferSkill;
			}
			if (Global.AutoFightSkillIDs == null)
			{
				Global.AutoFightSkillIDs = ConfigSystemParam.GetSystemParamIntArrayByName("AutoFightSkillIDs", ',');
			}
			int num = Global.Data.GameScene.GetDefaultSkillID();
			if (Global.GetSkillDataByID(num) != null)
			{
				if (num == Global.GetBaseSkillID(Global.Data.roleData.Occupation))
				{
					num = -1;
				}
				return num;
			}
			return -1;
		}

		public static bool IsAutoSkillPriority(int skillID)
		{
			return Global.skillPriorityDict.Count > 0 && Global.GetSkillPriorityDict().ContainsValue(skillID);
		}

		public static bool IsAutoFightSkill(int skillID)
		{
			int occupation = Global.Data.roleData.Occupation;
			int num = Global.CalcOriginalOccupationID(occupation);
			Dictionary<int, int[]> systemParamIntDictByName = ConfigSystemParam.GetSystemParamIntDictByName("SheZhiMagic", '|', ',');
			int[] array;
			if (systemParamIntDictByName.TryGetValue(occupation, ref array) || systemParamIntDictByName.TryGetValue(num, ref array))
			{
				for (int i = array.Length - 1; i >= 0; i--)
				{
					if (skillID == array[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public static int GetAutoFightSkillID()
		{
			int occupation = Global.Data.roleData.Occupation;
			int num = Global.CalcOriginalOccupationID(occupation);
			int result = Global.GetBaseSkillID(occupation);
			Dictionary<int, int[]> systemParamIntDictByName = ConfigSystemParam.GetSystemParamIntDictByName("SheZhiMagic", '|', ',');
			int[] array;
			if (systemParamIntDictByName.TryGetValue(occupation, ref array) || systemParamIntDictByName.TryGetValue(num, ref array))
			{
				for (int i = array.Length - 1; i >= 0; i--)
				{
					if (Global.GetSkillDataByID(array[i]) != null)
					{
						result = array[i];
						break;
					}
				}
			}
			return result;
		}

		public static RoleData FindRoleDataByName(string name)
		{
			RoleData result = null;
			Global.Data.OtherRolesByName.TryGetValue(name, ref result);
			return result;
		}

		public static RoleData FindRoleDataByID(int roleID)
		{
			RoleData result = null;
			Global.Data.OtherRoles.TryGetValue(roleID, ref result);
			return result;
		}

		public static string GetOccupationStr(int occup)
		{
			switch (occup)
			{
			case 0:
				return Global.GetLang("剑士");
			case 1:
				return Global.GetLang("魔法师");
			case 2:
				return Global.GetLang("弓箭手");
			case 3:
				return Global.GetLang("魔剑士");
			default:
				switch (occup)
				{
				case 21:
					return Global.GetLang("魔导士");
				case 22:
					return Global.GetLang("魔导师");
				case 23:
					return Global.GetLang("神导师");
				default:
					switch (occup)
					{
					case 31:
						return Global.GetLang("射手");
					case 32:
						return Global.GetLang("圣射手");
					case 33:
						return Global.GetLang("神射手");
					default:
						return Global.GetLang("箭圣");
					}
					break;
				}
				break;
			case 5:
				return Global.GetLang("召唤师");
			case 11:
				return Global.GetLang("骑士");
			case 12:
				return Global.GetLang("圣骑士");
			case 13:
				return Global.GetLang("神骑士");
			}
		}

		public static string GetOccupationStrByGoods(int occup)
		{
			string text = string.Empty;
			if ((1 & occup) != 0)
			{
				text = Global.GetLang("剑士");
			}
			if ((2 & occup) != 0)
			{
				if (string.Empty != text)
				{
					text += ',';
				}
				text += Global.GetLang("魔法师");
			}
			if ((4 & occup) != 0)
			{
				if (string.Empty != text)
				{
					text += ',';
				}
				text += Global.GetLang("弓箭手");
			}
			if ((8 & occup) != 0)
			{
				if (string.Empty != text)
				{
					text += ',';
				}
				text += Global.GetLang("魔剑士");
			}
			if ((32 & occup) != 0)
			{
				if (string.Empty != text)
				{
					text += ',';
				}
				text += Global.GetLang("召唤师");
			}
			return text;
		}

		public static int GetOccupationJiaDian(int occup)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Roles/OccupationAddPoint.xml");
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "ShuXings");
			if (xelement == null)
			{
				return 0;
			}
			XElement xelement2 = Global.GetXElement(xelement, "ShuXing", "OccupationID", occup.ToString());
			if (xelement2 == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement2, "JiaDian");
		}

		public static MJSSkillType GetMJSTypeByAttr()
		{
			if ((int)Global.GetCurrentRoleProp(1, 1) > (int)Global.GetCurrentRoleProp(1, 0))
			{
				return MJSSkillType.Magic_Sword;
			}
			return MJSSkillType.Strength_Sword;
		}

		public static MJSSkillType GetMJSTypeByAttr(GSprite _GSprite)
		{
			if ((int)Global.GetCurrentRoleProp(1, 1) > (int)Global.GetCurrentRoleProp(1, 0))
			{
				return MJSSkillType.Magic_Sword;
			}
			return MJSSkillType.Strength_Sword;
		}

		public static byte GetMapShowEquipType(RoleData rd)
		{
			byte result = 0;
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				if (rd.RebornShowModel == 0)
				{
					result = 1;
				}
				else
				{
					result = 3;
				}
			}
			else if (rd.RebornShowEquip == 1)
			{
				result = 2;
			}
			return result;
		}

		public static bool CheckMJSTypeLeader()
		{
			MJSSkillType mjstype = Global.GetMJSType();
			if (mjstype != Global.currentMJSType)
			{
				Global.currentMJSType = mjstype;
				return false;
			}
			return true;
		}

		public static MJSSkillType GetMJSType()
		{
			MJSSkillType result = MJSSkillType.Strength_Sword;
			if (Global.Data == null || Global.Data.roleData == null)
			{
				return result;
			}
			List<GoodsData> list = new List<GoodsData>();
			byte mapShowEquipType = Global.GetMapShowEquipType(Global.Data.roleData);
			List<GoodsData> list2;
			if (mapShowEquipType == 1)
			{
				list2 = Global.Data.roleData.RebornGoodsDataList;
			}
			else if (mapShowEquipType == 0)
			{
				list2 = Global.Data.roleData.GoodsDataList;
			}
			else
			{
				list2 = Global.Data.roleData.RebornGoodsDataList;
			}
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (list2[i].Using > 0)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(list2[i].GoodsID);
						if ((categoriyByGoodsID >= 11 && categoriyByGoodsID <= 21) || categoriyByGoodsID == 37 || categoriyByGoodsID == 38)
						{
							list.Add(list2[i]);
						}
					}
				}
			}
			return Global.GetMJSType(list, Global.CheckRoleOcc(Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation), mapShowEquipType);
		}

		public static MJSSkillType GetMJSType(List<GoodsData> lst, int Occ, byte ShowRebornEquip = 0)
		{
			MJSSkillType result = MJSSkillType.Strength_Sword;
			if (lst != null)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					if (lst[i].Using > 0)
					{
						GoodVO goodVO = null;
						if (ShowRebornEquip != 0)
						{
							if (Global.GetCategoriyByGoodsID(lst[i].GoodsID) == 37 || Global.GetCategoriyByGoodsID(lst[i].GoodsID) == 38)
							{
								goodVO = IConfigbase<ConfigReborn>.Instance.GetRebornEquipsByGoodsIDAndOccForShengWuAndShengQi(lst[i].GoodsID, Occ);
							}
						}
						else
						{
							goodVO = ConfigGoods.GetGoodsXmlNodeByID(lst[i].GoodsID);
						}
						if (goodVO != null && goodVO.Categoriy >= 11 && goodVO.Categoriy <= 21)
						{
							if (goodVO.Intelligence > goodVO.Strength)
							{
								result = MJSSkillType.Magic_Sword;
								if (lst[i].BagIndex == 0 || ShowRebornEquip != 0)
								{
									break;
								}
							}
							else
							{
								result = MJSSkillType.Strength_Sword;
								if (lst[i].BagIndex == 0 || ShowRebornEquip != 0)
								{
									break;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static MJSSkillType GetMJSType(RoleData rd)
		{
			List<GoodsData> list = new List<GoodsData>();
			byte b = 0;
			if (Global.Data != null && rd != null)
			{
				b = Global.GetMapShowEquipType(rd);
				List<GoodsData> list2;
				if (b == 1)
				{
					list2 = rd.RebornGoodsDataList;
				}
				else if (b == 0)
				{
					list2 = rd.GoodsDataList;
				}
				else
				{
					list2 = rd.RebornGoodsDataList;
				}
				if (list2 != null)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (list2[i].Using > 0)
						{
							int categoriyByGoodsID = Global.GetCategoriyByGoodsID(list2[i].GoodsID);
							if ((categoriyByGoodsID >= 11 && categoriyByGoodsID <= 21) || categoriyByGoodsID == 37 || categoriyByGoodsID == 38)
							{
								list.Add(list2[i]);
							}
						}
					}
				}
			}
			return Global.GetMJSType(list, Global.CheckRoleOcc(rd.Occupation, rd.SubOccupation), b);
		}

		public static RoleData ClientDataToRoleDataMini(RoleDataMini roleDataMini)
		{
			RoleData roleData = new RoleData
			{
				RoleID = roleDataMini.RoleID,
				RoleName = roleDataMini.RoleName,
				RoleSex = roleDataMini.RoleSex,
				Occupation = roleDataMini.Occupation,
				Level = roleDataMini.Level,
				Faction = roleDataMini.Faction,
				PKMode = roleDataMini.PKMode,
				PKValue = roleDataMini.PKValue,
				MapCode = roleDataMini.MapCode,
				RoleDirection = roleDataMini.RoleDirection,
				PosX = roleDataMini.PosX,
				PosY = roleDataMini.PosY,
				MaxLifeV = roleDataMini.MaxLifeV,
				LifeV = roleDataMini.LifeV,
				MaxMagicV = roleDataMini.MagicV,
				MagicV = roleDataMini.MagicV,
				BodyCode = roleDataMini.BodyCode,
				WeaponCode = roleDataMini.WeaponCode,
				OtherName = roleDataMini.OtherName,
				TeamID = roleDataMini.TeamID,
				TeamLeaderRoleID = roleDataMini.TeamLeaderRoleID,
				PKPoint = roleDataMini.PKPoint,
				StartPurpleNameTicks = roleDataMini.StartPurpleNameTicks,
				BattleNameStart = roleDataMini.BattleNameStart,
				BattleNameIndex = roleDataMini.BattleNameIndex,
				ZoneID = roleDataMini.ZoneID,
				BHName = roleDataMini.BHName,
				BHVerify = roleDataMini.BHVerify,
				BHZhiWu = roleDataMini.BHZhiWu,
				FSHuDunStart = roleDataMini.FSHuDunStart,
				BattleWhichSide = roleDataMini.BattleWhichSide,
				DSHideStart = roleDataMini.DSHideStart,
				FSHuDunSeconds = roleDataMini.FSHuDunSeconds,
				ZhongDuStart = roleDataMini.ZhongDuStart,
				ZhongDuSeconds = roleDataMini.ZhongDuSeconds,
				JieriChengHao = roleDataMini.JieriChengHao,
				DongJieStart = roleDataMini.DongJieStart,
				DongJieSeconds = roleDataMini.DongJieSeconds,
				DongJieMills = roleDataMini.DongJieSeconds * 1000,
				GoodsDataList = roleDataMini.GoodsDataList,
				ChangeLifeCount = roleDataMini.ChangeLifeLev,
				StallName = roleDataMini.StallName,
				MyWingData = roleDataMini.MyWingData,
				SettingBitFlags = roleDataMini.SettingBitFlags,
				SpouseId = roleDataMini.SpouseId,
				OccupationList = roleDataMini.OccupationList,
				JunTuanId = roleDataMini.JunTuanId,
				JunTuanName = roleDataMini.JunTuanName,
				HuiJiData = roleDataMini.HuiJiData,
				JueXingData = roleDataMini.JueXingData,
				CompType = roleDataMini.CompType,
				CompZhiWu = roleDataMini.CompZhiWu,
				MountEquipList = new List<GoodsData>(),
				SubOccupation = roleDataMini.SubOccupation,
				CurrentArmorV = roleDataMini.CurrentArmorV,
				MaxArmorV = roleDataMini.MaxArmorV,
				JingLingYuanSuJueXingData = roleDataMini.JingLingYuanSuJueXingData,
				RebornCount = roleDataMini.RebornCount,
				RebornLevel = roleDataMini.RebornLevel,
				RebornShowEquip = roleDataMini.RebornEquipShow,
				RebornGoodsDataList = roleDataMini.RebornGoodsDataList,
				RebornYinJi = roleDataMini.RebornYinJi,
				RebornShowModel = roleDataMini.RebornModelShow
			};
			roleData.BufferDataList = new List<BufferData>();
			if (roleDataMini.BufferMiniInfo != null)
			{
				for (int i = 0; i < roleDataMini.BufferMiniInfo.Count; i++)
				{
					BufferDataMini bufferDataMini = roleDataMini.BufferMiniInfo[i];
					BufferData bufferData = new BufferData
					{
						BufferID = bufferDataMini.BufferID,
						StartTime = bufferDataMini.StartTime,
						BufferSecs = bufferDataMini.BufferSecs,
						BufferVal = bufferDataMini.BufferVal,
						BufferType = bufferDataMini.BufferType
					};
					Global.RestoreBufferDataBufferSecs(bufferData);
					roleData.BufferDataList.Add(bufferData);
				}
			}
			roleData.PTID = roleDataMini.PTID;
			roleData.IsVIP = roleDataMini.IsVIP;
			roleData.BodyCode = roleDataMini.BodyCode;
			roleData.WeaponCode = roleDataMini.WeaponCode;
			roleData.RoleCommonUseIntPamams = roleDataMini.RoleCommonUseIntPamams;
			if (roleDataMini.EquipMount != null)
			{
				roleData.MountEquipList.Add(roleDataMini.EquipMount);
			}
			roleData.ZuoQiMainData = roleDataMini.ZuoQiMainData;
			return roleData;
		}

		public static int GetMagicCode(MagicInfoVO magicData, string attributeName, GSprite gSprite)
		{
			int result = -1;
			if (magicData == null)
			{
				return result;
			}
			string text = string.Empty;
			if (attributeName.Equals("AttackDistance"))
			{
				text = magicData.AttackDistance;
			}
			else if (attributeName.Equals("DelayDecoration"))
			{
				text = magicData.DelayDecoration;
			}
			else if (attributeName.Equals("TargetDecoration"))
			{
				text = magicData.TargetDecoration;
			}
			else if (attributeName.Equals("MagicCode"))
			{
				text = magicData.MagicCode;
			}
			else
			{
				if (!attributeName.Equals("FlyDecoration"))
				{
					return -1;
				}
				text = magicData.FlyDecoration;
			}
			string[] array = text.Split(new char[]
			{
				';'
			});
			if (array.Length > 1)
			{
				if (gSprite != null)
				{
					switch (Global.CalcOriginalOccupationID(gSprite.Occupation))
					{
					case 2:
						if (gSprite.WeaponState == WeaponStates.G)
						{
							result = Convert.ToInt32(array[0]);
						}
						else
						{
							result = Convert.ToInt32(array[1]);
						}
						break;
					}
				}
				else
				{
					result = Convert.ToInt32(array[0]);
				}
			}
			else if (!string.IsNullOrEmpty(text))
			{
				result = ConvertExt.SafeConvertToInt32(text);
			}
			return result;
		}

		public static NPCTaskState GetNPCTaskState(int npcID)
		{
			if (Global.Data.roleData.NPCTaskStateList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.NPCTaskStateList.Count; i++)
			{
				if (Global.Data.roleData.NPCTaskStateList[i].NPCID == npcID)
				{
					return Global.Data.roleData.NPCTaskStateList[i];
				}
			}
			return null;
		}

		public static Point GetNPCPointByID(int mapCode, int npcID)
		{
			string xmlName = StringUtil.substitute("npcs.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return new Point(-1, -1);
			}
			IEnumerable<XElement> enumerable = gameMapSettingsXml.Element("NPCs").Elements();
			for (int i = 0; i < Enumerable.Count<XElement>(enumerable); i++)
			{
				XElement xelement = Enumerable.ElementAt<XElement>(enumerable, i);
				if (npcID == Global.GetXElementAttributeInt(xelement, "Code"))
				{
					Point result = new Point(Global.GetXElementAttributeInt(xelement, "X"), Global.GetXElementAttributeInt(xelement, "Y"));
					return result;
				}
			}
			return new Point(-1, -1);
		}

		public static Point GetFirstNPCPointAndID(int mapCode, out int npcID)
		{
			npcID = -1;
			Point result = new Point(-1, -1);
			string xmlName = StringUtil.substitute("npcs.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return result;
			}
			IEnumerator<XElement> enumerator = gameMapSettingsXml.Element("NPCs").Elements().GetEnumerator();
			if (enumerator == null)
			{
				return result;
			}
			if (!enumerator.MoveNext())
			{
				return result;
			}
			XElement xelement = enumerator.Current;
			npcID = Global.GetXElementAttributeInt(xelement, "Code");
			result = new Point(Global.GetXElementAttributeInt(xelement, "X"), Global.GetXElementAttributeInt(xelement, "Y"));
			return result;
		}

		public static void CloseNPCAllSound(int NpcId = 0)
		{
			List<IObject> objectsList = ObjectsManager.GetObjectsList();
			if (objectsList != null && objectsList.Count > 0)
			{
				for (int i = 0; i < objectsList.Count; i++)
				{
					IObject @object = objectsList[i];
					if (@object != null && @object is GSprite)
					{
						GSprite gsprite = (GSprite)@object;
						if (gsprite.SpriteType == GSpriteTypes.NPC && gsprite.RoleID != NpcId)
						{
							gsprite.StopSpriteSound();
						}
					}
				}
			}
		}

		public static int GetBossUnionLevel(int bossID, int type = 0)
		{
			int result = 0;
			if (Global.MonsterUnionLevel == null)
			{
				Global.MonsterUnionLevel = new Dictionary<int, int>();
				for (int i = 0; i < 2; i++)
				{
					XElement gameResXml;
					if (i == 0)
					{
						gameResXml = Global.GetGameResXml("Config/Activity/BossInfo.Xml");
					}
					else
					{
						gameResXml = Global.GetGameResXml("Config/HuangJin.xml");
					}
					List<XElement> xelementList = Global.GetXElementList(gameResXml, "Boss");
					if (xelementList != null)
					{
						for (int j = 0; j < xelementList.Count; j++)
						{
							int num = 0;
							XElement xelement = xelementList[j];
							int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
							int[] xelementAttributeIntArray = Global.GetXElementAttributeIntArray(xelement, "Level", "*");
							if (xelementAttributeIntArray.Length == 2)
							{
								num = Global.GetUnionLevel(xelementAttributeIntArray[0], xelementAttributeIntArray[1]);
							}
							else if (xelementAttributeIntArray.Length == 1)
							{
								num = xelementAttributeIntArray[0];
							}
							Global.MonsterUnionLevel[xelementAttributeInt] = num;
						}
					}
				}
			}
			if (!Global.MonsterUnionLevel.TryGetValue(bossID, ref result))
			{
				result = int.MaxValue;
			}
			return result;
		}

		public static bool GetJuQingFuBenBossModalScale(int taskID, out float scale)
		{
			scale = 1f;
			double[] array = null;
			if (Global.JuQingFuBenBossScale == null)
			{
				Global.JuQingFuBenBossScale = ConfigSystemParam.GetSystemParamIntDoubleDictByName("JuQingFuBen", '|', ',');
			}
			if (Global.JuQingFuBenBossScale.TryGetValue(taskID, ref array))
			{
				scale = (float)array[1];
				return true;
			}
			return false;
		}

		public static Point GetMonsterPointByID(int mapCode, int monsterID)
		{
			string xmlName = StringUtil.substitute("Monsters.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return new Point(-1, -1);
			}
			long ticks = DateTime.Now.Ticks;
			Random random = new Random((int)(ticks & (long)((ulong)-1)) | (int)(ticks >> 32));
			List<Point> list = new List<Point>();
			IEnumerable<XElement> enumerable = gameMapSettingsXml.Element("Monsters").Elements();
			if (enumerable == null)
			{
				return new Point(-1, -1);
			}
			for (int i = 0; i < Enumerable.Count<XElement>(enumerable); i++)
			{
				XElement xelement = Enumerable.ElementAt<XElement>(enumerable, i);
				if (monsterID == Global.GetXElementAttributeInt(xelement, "Code"))
				{
					double num = Global.GetXElementAttributeDouble(xelement, "Radius");
					Point point = new Point(Global.GetXElementAttributeInt(xelement, "X"), Global.GetXElementAttributeInt(xelement, "Y"));
					point = new Point(random.Next(point.X - (int)(num / 2.0), point.X + (int)(num / 2.0)), random.Next(point.Y - (int)(num / 2.0), point.Y + (int)(num / 2.0)));
					list.Add(point);
				}
			}
			if (list.Count <= 0)
			{
				return new Point(-1, -1);
			}
			return list[random.Next(0, list.Count)];
		}

		public static void DestroyGoods(GoodsData gd)
		{
			GameInstance.Game.SpriteModGoods(4, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
		}

		public static int GetGoodsGridNumByID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return 1;
			}
			return goodsXmlNodeByID.GridNum;
		}

		public static int GetTotalMaxBagGridCount()
		{
			if (Global.Data.roleData.BagNum > Global.MaxBagGridNum)
			{
				return Global.MaxBagGridNum;
			}
			return Global.Data.roleData.BagNum;
		}

		public static bool CanAddGoods(int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool useOldGrid = false)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			bool flag = goodsXmlNodeByID.RebornEquip == 1;
			List<GoodsData> list;
			if (flag)
			{
				list = Global.Data.roleData.RebornGoodsDataList;
			}
			else
			{
				list = Global.Data.roleData.GoodsDataList;
			}
			if (list == null)
			{
				return true;
			}
			int goodsGridNumByID = Global.GetGoodsGridNumByID(goodsID);
			bool flag2 = false;
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Using <= 0)
				{
					num++;
					if (useOldGrid && goodsGridNumByID > 1 && list[i].GoodsID == goodsID && list[i].Binding == binding && Global.DateTimeEqual(list[i].Endtime, endTime) && list[i].GCount + newGoodsNum <= goodsGridNumByID)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (flag2)
			{
				return true;
			}
			int num2;
			if (!flag)
			{
				num2 = Global.GetTotalMaxBagGridCount();
			}
			else
			{
				num2 = Global.GetTotalMaxChongShengBagGridCount();
			}
			return num < num2;
		}

		public static bool IsBagFull()
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Using <= 0)
				{
					num++;
				}
			}
			int totalMaxBagGridCount = Global.GetTotalMaxBagGridCount();
			return num >= totalMaxBagGridCount;
		}

		public static int GetBaoGuoSpaceCount()
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return Global.GetTotalMaxBagGridCount();
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Using <= 0)
				{
					num++;
				}
			}
			int totalMaxBagGridCount = Global.GetTotalMaxBagGridCount();
			return totalMaxBagGridCount - num;
		}

		public static int GetTotalMaxChongShengBagGridCount()
		{
			if (Global.Data.roleData.RebornBagNum > Global.MaxBagGridNum)
			{
				return Global.MaxRebornBagGridNum;
			}
			return Global.Data.roleData.RebornBagNum;
		}

		public static bool IsRebornBagFull()
		{
			if (Global.Data.roleData.RebornGoodsDataList == null)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.RebornGoodsDataList[i].Using <= 0)
				{
					num++;
				}
			}
			int totalMaxChongShengBagGridCount = Global.GetTotalMaxChongShengBagGridCount();
			return num >= totalMaxChongShengBagGridCount;
		}

		public static int GetRebornBaoGuoSpaceCount()
		{
			if (Global.Data.roleData.RebornGoodsDataList == null)
			{
				return Global.GetTotalMaxChongShengBagGridCount();
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.RebornGoodsDataList[i].Using <= 0)
				{
					num++;
				}
			}
			int totalMaxChongShengBagGridCount = Global.GetTotalMaxChongShengBagGridCount();
			return totalMaxChongShengBagGridCount - num;
		}

		public static string GetGoodsUseLeveByID(int nID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nID);
			if (goodsXmlNodeByID == null)
			{
				return Global.GetLang("无");
			}
			return Convert.ToString(goodsXmlNodeByID.ToLevel);
		}

		public static string GetGoodsUseZhuanShengByID(int nID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nID);
			if (goodsXmlNodeByID == null)
			{
				return Global.GetLang("无");
			}
			return Convert.ToString(goodsXmlNodeByID.ToZhuanSheng);
		}

		public static string GetGoodsNameByID(int id, bool color = false)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return Global.GetLang("无");
			}
			if (color)
			{
				return Global.GetColorStringForNGUIText(new object[]
				{
					goodsXmlNodeByID.GoodsColor,
					goodsXmlNodeByID.Title
				});
			}
			return goodsXmlNodeByID.Title;
		}

		public static int GetGoodsOccByID(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.ToOccupation;
		}

		public static int GetGoodsIconCodeByID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return int.Parse(goodsXmlNodeByID.IconCode);
		}

		public static int GetGoodsCatetoriy(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.Categoriy;
		}

		public static string GetGoodsSoundByID(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return string.Empty;
			}
			return goodsXmlNodeByID.Sound;
		}

		public static string GetGoodsFallSoundByID(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return string.Empty;
			}
			return goodsXmlNodeByID.DropSound;
		}

		public static string GetGoodsGetSoundByID(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return string.Empty;
			}
			return goodsXmlNodeByID.GetSound;
		}

		public static SolidColorBrush ParseStringColor(string textColor)
		{
			try
			{
				if (string.IsNullOrEmpty(textColor))
				{
					return new SolidColorBrush(uint.MaxValue);
				}
				string text = Global.StringReplaceAll(textColor, "#", string.Empty);
				int a = (int)Convert.ToByte("ff", 16);
				int num = 0;
				if (text.Length == 8)
				{
					a = (int)Convert.ToByte(text.Substring(num, 2), 16);
					num = 2;
				}
				int r = (int)Convert.ToByte(text.Substring(num, 2), 16);
				num += 2;
				int g = (int)Convert.ToByte(text.Substring(num, 2), 16);
				num += 2;
				int b = (int)Convert.ToByte(text.Substring(num, 2), 16);
				uint color = ColorSL.FromArgb((uint)a, (uint)r, (uint)g, (uint)b);
				return new SolidColorBrush(color);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return new SolidColorBrush(uint.MaxValue);
		}

		public static uint ParseStringColorToUint(string textColor)
		{
			try
			{
				if (string.IsNullOrEmpty(textColor))
				{
					return uint.MaxValue;
				}
				string text = Global.StringReplaceAll(textColor, "#", string.Empty);
				int a = (int)Convert.ToByte("ff", 16);
				int num = 0;
				if (text.Length == 8)
				{
					a = (int)Convert.ToByte(text.Substring(num, 2), 16);
					num = 2;
				}
				int r = (int)Convert.ToByte(text.Substring(num, 2), 16);
				num += 2;
				int g = (int)Convert.ToByte(text.Substring(num, 2), 16);
				num += 2;
				int b = (int)Convert.ToByte(text.Substring(num, 2), 16);
				return ColorSL.FromArgb((uint)a, (uint)r, (uint)g, (uint)b);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return uint.MaxValue;
		}

		public static string GetGoodsColorString(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return "FFFFFF";
			}
			return goodsXmlNodeByID.GoodsColor;
		}

		public static SolidColorBrush GetGoodsColor(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return new SolidColorBrush(uint.MaxValue);
			}
			string goodsColor = goodsXmlNodeByID.GoodsColor;
			return Global.ParseStringColor(goodsColor);
		}

		public static SolidColorBrush GetFallGoodsColor(int id)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(id);
			if (goodsXmlNodeByID == null)
			{
				return new SolidColorBrush(uint.MaxValue);
			}
			string fallGoodsColor = goodsXmlNodeByID.FallGoodsColor;
			return Global.ParseStringColor(fallGoodsColor);
		}

		public static int GetTotalGoodsCountByID(int goodsID)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].GoodsID == goodsID)
				{
					if (!Global.IsGoodsTimeOver(Global.Data.roleData.GoodsDataList[i]))
					{
						num += Global.Data.roleData.GoodsDataList[i].GCount;
					}
				}
			}
			return num;
		}

		public static int GetTotalBindingGoodsCountByID(int goodsID)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Binding > 0)
				{
					if (Global.Data.roleData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(Global.Data.roleData.GoodsDataList[i]))
						{
							num += Global.Data.roleData.GoodsDataList[i].GCount;
						}
					}
				}
			}
			return num;
		}

		public static int GetTotalEndTimeGoodsCountByID(int goodsID)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].GoodsID == goodsID)
				{
					if (Global.IsTimeLimitGoods(Global.Data.roleData.GoodsDataList[i]))
					{
						if (!Global.IsGoodsTimeOver(Global.Data.roleData.GoodsDataList[i]))
						{
							num += Global.Data.roleData.GoodsDataList[i].GCount;
						}
					}
				}
			}
			return num;
		}

		public static int GetTotalGoodsCountByRequire(string strRequire = "")
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return 0;
			}
			int num = 0;
			if (strRequire != string.Empty)
			{
				string[] array = strRequire.Split(new char[]
				{
					','
				});
				if (array.Length != 7)
				{
					return num;
				}
				int num2 = Convert.ToInt32(array[0]);
				int num3 = Convert.ToInt32(array[2]);
				int num4 = Convert.ToInt32(array[3]);
				int num5 = Convert.ToInt32(array[4]);
				int num6 = Convert.ToInt32(array[5]);
				int num7 = Convert.ToInt32(array[6]);
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.GoodsID == num2)
					{
						if (!Global.IsGoodsTimeOver(goodsData))
						{
							if (goodsData.Quality >= num4 && goodsData.AppendPropLev >= num5 && goodsData.Lucky >= num6 && goodsData.ExcellenceInfo >= num7)
							{
								num += Global.Data.roleData.GoodsDataList[i].GCount;
							}
						}
					}
				}
			}
			return num;
		}

		public static GoodsData GetGoodsDataByDbID(int dbID, List<GoodsData> goodsDataList = null)
		{
			if (goodsDataList == null)
			{
				goodsDataList = Global.Data.roleData.GoodsDataList;
			}
			if (goodsDataList != null && 0 < goodsDataList.Count)
			{
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					if (goodsDataList[i].Id == dbID)
					{
						return goodsDataList[i];
					}
				}
			}
			if (Global.Data.fashionAndTitleList != null && 0 < Global.Data.fashionAndTitleList.Count)
			{
				for (int j = 0; j < Global.Data.fashionAndTitleList.Count; j++)
				{
					if (Global.Data.fashionAndTitleList[j] != null && Global.Data.fashionAndTitleList[j].Id == dbID)
					{
						return Global.Data.fashionAndTitleList[j];
					}
				}
			}
			if (Global.Data.roleData.RebornGoodsDataList != null)
			{
				goodsDataList = Global.Data.roleData.RebornGoodsDataList;
				if (goodsDataList != null && 0 < goodsDataList.Count)
				{
					for (int k = 0; k < goodsDataList.Count; k++)
					{
						if (goodsDataList[k].Id == dbID)
						{
							return goodsDataList[k];
						}
					}
				}
			}
			goodsDataList = Global.Data.roleData.HolyGoodsDataList;
			if (goodsDataList != null && 0 < goodsDataList.Count)
			{
				for (int l = 0; l < goodsDataList.Count; l++)
				{
					if (goodsDataList[l].Id == dbID)
					{
						return goodsDataList[l];
					}
				}
			}
			return null;
		}

		public static GoodsData GetEquipDataByID(int dbID)
		{
			if (Global.Data.roleData.GoodsDataList != null && Global.Data.roleData.GoodsDataList != null && 0 < Global.Data.roleData.GoodsDataList.Count)
			{
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					if (Global.Data.roleData.GoodsDataList[i].Id == dbID)
					{
						return Global.Data.roleData.GoodsDataList[i];
					}
				}
			}
			if (Global.Data.equipPet != null && Global.Data.equipPet != null && 0 < Global.Data.equipPet.Count)
			{
				for (int j = 0; j < Global.Data.equipPet.Count; j++)
				{
					if (Global.Data.equipPet[j].Id == dbID)
					{
						return Global.Data.equipPet[j];
					}
				}
			}
			return null;
		}

		public static GoodsData GetGoodsDataByID(int goodsID)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].GoodsID == goodsID)
				{
					return Global.Data.roleData.GoodsDataList[i];
				}
			}
			return null;
		}

		public static GoodsData GetIsBindGoodsDataByID(int goodsID, bool isBind)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return null;
			}
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (num != -1 && num2 != -1)
				{
					break;
				}
				if (Global.Data.roleData.GoodsDataList[i].GoodsID == goodsID)
				{
					if (Global.Data.roleData.GoodsDataList[i].Binding == 1)
					{
						num = i;
					}
					else
					{
						num2 = i;
					}
				}
			}
			if (isBind)
			{
				if (num != -1)
				{
					return Global.Data.roleData.GoodsDataList[num];
				}
				if (num2 != -1)
				{
					return Global.Data.roleData.GoodsDataList[num2];
				}
			}
			else
			{
				if (num2 != -1)
				{
					return Global.Data.roleData.GoodsDataList[num2];
				}
				if (num != -1)
				{
					return Global.Data.roleData.GoodsDataList[num];
				}
			}
			return null;
		}

		public static GoodsData GetGoodsDataByAdd(AddGoodsData addGoodsData)
		{
			GoodsData goodsData = new GoodsData();
			string text = addGoodsData.newEndTime;
			text = Global.StringReplaceAll(text, "$", ":");
			goodsData.Id = addGoodsData.id;
			goodsData.GoodsID = addGoodsData.goodsID;
			goodsData.Using = 0;
			goodsData.Forge_level = addGoodsData.forgeLevel;
			goodsData.Starttime = "1900-01-01 12:00:00";
			goodsData.Endtime = text;
			goodsData.Site = addGoodsData.site;
			goodsData.Quality = addGoodsData.quality;
			goodsData.Props = string.Empty;
			goodsData.GCount = addGoodsData.goodsNum;
			goodsData.Binding = addGoodsData.binding;
			goodsData.Jewellist = addGoodsData.jewellist;
			goodsData.AddPropIndex = addGoodsData.addPropIndex;
			goodsData.BornIndex = addGoodsData.bornIndex;
			goodsData.Lucky = addGoodsData.lucky;
			goodsData.Strong = addGoodsData.strong;
			goodsData.ExcellenceInfo = addGoodsData.ExcellenceProperty;
			goodsData.AppendPropLev = addGoodsData.nAppendPropLev;
			goodsData.ChangeLifeLevForEquip = addGoodsData.ChangeLifeLevForEquip;
			goodsData.BagIndex = addGoodsData.bagIndex;
			goodsData.WashProps = addGoodsData.washProps;
			goodsData.JuHunID = addGoodsData.juHunLevel;
			goodsData.ElementhrtsProps = addGoodsData.ElementhrtsProps;
			goodsData.Props = addGoodsData.prop;
			return goodsData;
		}

		public static void AddChongShengGoodsData(GoodsData goodsData)
		{
			if (Global.Data.roleData.RebornGoodsDataList == null)
			{
				Global.Data.roleData.RebornGoodsDataList = new List<GoodsData>();
			}
			Global.Data.roleData.RebornGoodsDataList.Add(goodsData);
			if (goodsData != null)
			{
				bool flag = Super.ShowGoodRedTip(goodsData);
				if (flag)
				{
					ActivityTipManager.SetActivityTipItemActive(30001, flag);
				}
			}
		}

		public static void RemoveChongShengGoodsData(int id)
		{
			if (Global.Data.roleData.RebornGoodsDataList == null)
			{
				return;
			}
			for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
			{
				if (id == Global.Data.roleData.RebornGoodsDataList[i].Id)
				{
					Global.Data.roleData.RebornGoodsDataList.RemoveAt(i);
					break;
				}
			}
		}

		public static void AddGoodsData(GoodsData goodsData)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (Global.IsRebornGood(goodsXmlNodeByID))
			{
				if (Global.Data.roleData.RebornGoodsDataList == null)
				{
					Global.Data.roleData.RebornGoodsDataList = new List<GoodsData>();
				}
				Global.Data.roleData.RebornGoodsDataList.Add(goodsData);
			}
			else
			{
				if (Global.Data.roleData.GoodsDataList == null)
				{
					Global.Data.roleData.GoodsDataList = new List<GoodsData>();
				}
				Global.Data.roleData.GoodsDataList.Add(goodsData);
				if (goodsData != null)
				{
					bool flag = Super.ShowGoodRedTip(goodsData);
					if (flag)
					{
						ActivityTipManager.SetActivityTipItemActive(30001, flag);
					}
				}
			}
		}

		public static void RemoveGoodsData(GoodsData goodsData)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			List<GoodsData> list;
			if (Global.IsRebornGood(goodsXmlNodeByID))
			{
				list = Global.Data.roleData.RebornGoodsDataList;
			}
			else
			{
				list = Global.Data.roleData.GoodsDataList;
			}
			if (list == null)
			{
				return;
			}
			NGUITools.ListFastRemove<GoodsData>(list, goodsData);
			if (!Global.IsRebornGood(goodsXmlNodeByID))
			{
				bool active = false;
				for (int i = 0; i < list.Count; i++)
				{
					GoodsData data = Global.Data.roleData.GoodsDataList[i];
					if (Super.ShowGoodRedTip(data))
					{
						active = true;
						break;
					}
				}
				ActivityTipManager.SetActivityTipItemActive(30001, active);
			}
		}

		public static void RemoveGoodsDataByDbId(int id)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return;
			}
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
				if (id == goodsData.Id)
				{
					Global.Data.roleData.GoodsDataList.Remove(goodsData);
					break;
				}
			}
		}

		public static void RemoveGoodsData(int id)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return;
			}
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (id == Global.Data.roleData.GoodsDataList[i].Id)
				{
					Global.Data.roleData.GoodsDataList.RemoveAt(i);
					break;
				}
			}
		}

		public static bool CanTakeNewGoodsByGridNum(int newGoodsCount)
		{
			int goodsUsedGrid = Global.GetGoodsUsedGrid();
			int totalMaxBagGridCount = Global.GetTotalMaxBagGridCount();
			return newGoodsCount + goodsUsedGrid <= totalMaxBagGridCount;
		}

		public static int GetGoodsUsedGrid()
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Using <= 0)
				{
					num++;
				}
			}
			return num;
		}

		public static bool UsingGoodsByID(int goodsID)
		{
			GoodsData goodsDataByID = Global.GetGoodsDataByID(goodsID);
			if (goodsDataByID != null && goodsDataByID.GCount > 0)
			{
				bool flag = Global.GetGoodsUsingModeByGoodsID(goodsID) > 0;
				if (flag && !Global.GoodsCoolDown(goodsDataByID.GoodsID))
				{
					Global.ToUseGoods(goodsDataByID, true, false);
					return true;
				}
			}
			return false;
		}

		public static int GetGoodsDataIndexByDbID(int dbID, bool ignoreUsing = true)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return -1;
			}
			int num = -1;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (!ignoreUsing || Global.Data.roleData.GoodsDataList[i].Using <= 0)
				{
					num++;
					if (Global.Data.roleData.GoodsDataList[i].Id == dbID)
					{
						break;
					}
				}
			}
			return num;
		}

		public static GoodsData GetGoodsDataByType(int dbID, int goodsID, int isusing = 0)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return null;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Id != dbID)
				{
					if (isusing == Global.Data.roleData.GoodsDataList[i].Using)
					{
						goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.GoodsDataList[i].GoodsID);
						if (goodsXmlNodeByID != null)
						{
							int categoriy2 = goodsXmlNodeByID.Categoriy;
							if (categoriy == categoriy2)
							{
								return Global.Data.roleData.GoodsDataList[i];
							}
						}
					}
				}
			}
			return null;
		}

		public static GoodsData GetGoodsDataByCatetoriy(List<GoodsData> goodsDataList, int categoriy, int isusing = 0)
		{
			if (goodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (isusing == goodsDataList[i].Using)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataList[i].GoodsID);
					if (goodsXmlNodeByID != null)
					{
						int categoriy2 = goodsXmlNodeByID.Categoriy;
						if (categoriy2 == categoriy)
						{
							return goodsDataList[i];
						}
					}
				}
			}
			return null;
		}

		public static int GetGoodsUsingModeByGoodsID(int goodsID)
		{
			int result = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return result;
			}
			return goodsXmlNodeByID.UsingMode;
		}

		public static bool CanDirectUseGoods(int goodsID, bool errHint = true)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 0 && categoriy < 25)
			{
				return true;
			}
			if (Global.IsRebornEquip(categoriy))
			{
				return true;
			}
			int glUI = goodsXmlNodeByID.GlUI;
			if (glUI > 0)
			{
				return true;
			}
			string title = goodsXmlNodeByID.Title;
			int usingMode = goodsXmlNodeByID.UsingMode;
			if (usingMode <= 0)
			{
				if (errHint)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("【{0}】不能被直接使用"), title), 0, -1, -1, 0);
				}
				return false;
			}
			return true;
		}

		public static int GetGoodsToSex(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.ToSex;
		}

		public static Dictionary<string, int> GetGoodsToTypeLimitString(int goodsID)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return dictionary;
			}
			string text = goodsXmlNodeByID.ToType.ToString();
			string text2 = goodsXmlNodeByID.ToTypeProperty.ToString();
			if ("-1" == text || text.Length <= 0)
			{
				return dictionary;
			}
			string[] array = text.Split(new char[]
			{
				','
			});
			string[] array2 = text2.Split(new char[]
			{
				','
			});
			if (array.Length != array2.Length)
			{
				return dictionary;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string type = array[i];
				string gateValue = array2[i];
				if (!Global.IsRoleReachLimit(type, gateValue))
				{
					dictionary[Global.GetEquipLimitString(type, gateValue)] = 0;
				}
				else
				{
					dictionary[Global.GetEquipLimitString(type, gateValue)] = 1;
				}
			}
			return dictionary;
		}

		public static bool CanUseGoodsByExtraTypeProperty(string toType, string toTypeProperty, bool errHint = true)
		{
			if ("-1" == toType || toType.Length <= 0)
			{
				return true;
			}
			string[] array = toType.Split(new char[]
			{
				','
			});
			string[] array2 = toTypeProperty.Split(new char[]
			{
				','
			});
			if (array.Length != array2.Length)
			{
				return true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string text2 = array2[i];
				if (!Global.IsRoleReachLimit(text, text2))
				{
					if (errHint)
					{
						if (StringUtil.isEqualIgnoreCase(text, "NeedTask"))
						{
							int num = ConvertExt.SafeConvertToInt32(text2);
							string text3 = string.Empty;
							if (ConfigTasks.TaskXmlNodeDict.ContainsKey(num))
							{
								TaskVO taskVO = ConfigTasks.TaskXmlNodeDict[num];
								if (taskVO != null)
								{
									string text4 = taskVO.Title;
									if (5 < text4.Length)
									{
										text4 = text4.Insert(3, "\r\n");
									}
									text3 = StringUtil.substitute(Global.GetLang("完成{0}转{1}级主线任务{2}"), new object[]
									{
										taskVO.MinZhuanSheng,
										taskVO.MinLevel,
										text4
									});
								}
							}
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("需要{0}才能使用"), text3), 0, -1, -1, 0);
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("需要{0}才能使用"), Global.GetEquipLimitString(text, text2)), 0, -1, -1, 0);
						}
					}
					return false;
				}
			}
			return true;
		}

		public static bool IsRoleReachLimit(string type, string gateValue)
		{
			int num = ConvertExt.SafeConvertToInt32(gateValue);
			int num2 = num + 1;
			if (StringUtil.isEqualIgnoreCase(type, "WingSuit"))
			{
				if (Global.Data.roleData.MyWingData != null)
				{
					num2 = Global.Data.roleData.MyWingData.WingID;
				}
			}
			else if (StringUtil.isEqualIgnoreCase(type, "ChengJiuLevel"))
			{
				num2 = Global.GetRoleCommonUseParamsValue(0);
			}
			else if (StringUtil.isEqualIgnoreCase(type, "JunXianLevel"))
			{
				num2 = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
			}
			else if (StringUtil.isEqualIgnoreCase(type, "ZhuanShengLevel"))
			{
				num2 = Global.Data.roleData.ChangeLifeCount;
			}
			else if (StringUtil.isEqualIgnoreCase(type, "Level"))
			{
				num2 = Global.Data.roleData.Level;
			}
			else if (StringUtil.isEqualIgnoreCase(type, "HuFuSuit"))
			{
				num2 = Global.GetHuShengFuLevel();
			}
			else if (StringUtil.isEqualIgnoreCase(type, "DaTianShiSuit"))
			{
				num2 = Global.GetMaxDaTianShiLevel();
			}
			else if (StringUtil.isEqualIgnoreCase(type, "VIP"))
			{
				num2 = Global.GetVIPLeve();
			}
			else if (StringUtil.isEqualIgnoreCase(type, "NeedTask"))
			{
				num2 = Global.Data.roleData.CompletedMainTaskID;
			}
			return num2 < 0 || num2 >= num;
		}

		public static string GetEquipLimitString(string type, string gateValue)
		{
			int num = ConvertExt.SafeConvertToInt32(gateValue);
			string text = string.Empty;
			if (StringUtil.isEqualIgnoreCase(type, "WingSuit"))
			{
				text = StringUtil.substitute(Global.GetLang("翅膀阶数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "EquipSuit"))
			{
				text = StringUtil.substitute(Global.GetLang("装备阶数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "QiangHuaLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("强化等级{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "ZhuiJiaLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("追加等级{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "ChengJiuLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("成就阶数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "JunXianLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("军衔阶数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "ZhuanShengLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("角色转生数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "Level"))
			{
				text = StringUtil.substitute(Global.GetLang("角色等级{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "HuFuSuit"))
			{
				text = StringUtil.substitute(Global.GetLang("护身符阶数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "PetLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("宠物等级{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "YuanSuZhiXinLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("元素之心等级{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "DaTianShiSuit"))
			{
				text = StringUtil.substitute(Global.GetLang("大天使阶数{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "VIP"))
			{
				text = StringUtil.substitute(Global.GetLang("VIP等级{0}"), new object[]
				{
					num
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "NeedTask"))
			{
				if (ConfigTasks.TaskXmlNodeDict.ContainsKey(num))
				{
					TaskVO taskVO = ConfigTasks.TaskXmlNodeDict[num];
					if (taskVO != null)
					{
						string text2 = taskVO.Title;
						if (5 < text2.Length)
						{
							text2 = text2.Insert(3, "\r\n");
						}
						text = StringUtil.substitute(Global.GetLang("需要完成{0}转{1}级主线任务{2}"), new object[]
						{
							taskVO.MinZhuanSheng,
							taskVO.MinLevel,
							text2
						});
					}
				}
			}
			else if (StringUtil.isEqualIgnoreCase(type, "CanNotBeyondLevel"))
			{
				text = StringUtil.substitute(Global.GetLang("达到{0}转{1}级以上无法使用"), new object[]
				{
					gateValue.Split(new char[]
					{
						'|'
					})[0],
					gateValue.Split(new char[]
					{
						'|'
					})[1]
				});
			}
			else if (StringUtil.isEqualIgnoreCase(type, "FEIANQUANQU"))
			{
				text = Global.GetLang("不能在安全区使用");
			}
			else if (StringUtil.isEqualIgnoreCase(type, "NeedMarry"))
			{
				text = Global.GetLang("需结婚");
			}
			if (StringUtil.isEqualIgnoreCase(type, "JunXianLevel"))
			{
				XElement gameResXml = Global.GetGameResXml("Config/JunXian.xml");
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (gameResXml != null)
				{
					List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
					for (int i = 0; i < xelementList.Count; i++)
					{
						XElement xelement = xelementList[i];
						dictionary.Add(xelement.Attribute("Level").Value, xelement.Attribute("Name").Value);
					}
				}
				text = text.Replace(gateValue, dictionary[gateValue]);
			}
			else if (StringUtil.isEqualIgnoreCase(type, "JunXianLevel"))
			{
				text = text.Replace(gateValue, Global.GetGoodsNameByID(ConfigSystemParam.GetSystemParamIntArrayByName("ChengJiuBufferGoodsIDs", ',')[int.Parse(gateValue)], false));
			}
			return text;
		}

		public static bool JugeOccupationGoodsID(int goodsID)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("SkillBooksGoodsIDs", ',');
			for (int i = 0; i < systemParamIntArrayByName.Length; i++)
			{
				if (systemParamIntArrayByName[i] == goodsID)
				{
					return true;
				}
			}
			return false;
		}

		public static bool CanUseGoods(int goodsID, bool errHint = true, bool jugeOccupation = true)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			string title = goodsXmlNodeByID.Title;
			int categoriy = goodsXmlNodeByID.Categoriy;
			int toOccupation = goodsXmlNodeByID.ToOccupation;
			int toLevel = goodsXmlNodeByID.ToLevel;
			string execMagic = goodsXmlNodeByID.ExecMagic;
			int toSex = goodsXmlNodeByID.ToSex;
			string text = goodsXmlNodeByID.ToType.ToString();
			string text2 = goodsXmlNodeByID.ToTypeProperty.ToString();
			string text3 = goodsXmlNodeByID.BaoguoID.ToString();
			if (!jugeOccupation)
			{
				jugeOccupation = Global.JugeOccupationGoodsID(goodsID);
			}
			if (jugeOccupation && toOccupation >= 0 && (1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & toOccupation) == 0)
			{
				if (errHint)
				{
					Super.HintMainText(StringUtil.substitute(Global.GetLang("职业不对应，不能使用！"), new object[0]), 10, 3);
				}
				return false;
			}
			if (!Global.CanUseGoodsAttr(goodsID, errHint))
			{
				return false;
			}
			if (24 <= categoriy && 27 >= categoriy && Global.IsOperateUnPermitInKuaFuMapCheck(errHint, true))
			{
				return false;
			}
			if (errHint)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AutoAddLifeVGoodsIDs", ',');
				if (systemParamIntArrayByName.IndexOf(goodsID) != -1 && (Global.Data.roleData.LifeV >= Global.Data.roleData.MaxLifeV || Global.Data.roleData.LifeV <= 0))
				{
					Super.HintMainText(Global.GetLang("您当前的状态无需使用生命药品！"), 10, 3);
					return false;
				}
				int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("AutoAddMagicVGoodsIDs", ',');
				if (systemParamIntArrayByName2.IndexOf(goodsID) != -1 && (Global.Data.roleData.MagicV >= Global.Data.roleData.MaxMagicV || Global.Data.roleData.LifeV <= 0))
				{
					Super.HintMainText(Global.GetLang("您当前的状态无需使用魔法药品！"), 10, 3);
					return false;
				}
				if (execMagic.IndexOf("SUB_ZUIEZHI") != -1 && Global.Data.roleData.PKValue <= 0)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("罪恶值已经为0，不需要使用【{0}】"), new object[]
					{
						title
					}), 0, -1, -1, 0);
					return false;
				}
			}
			return true;
		}

		public static bool CanUseGoodsAttr(int goodsID, bool errHint)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			int num = Global.CalcOriginalOccupationID(goodsXmlNodeByID.ToOccupation);
			int toLevel = goodsXmlNodeByID.ToLevel;
			int toSex = goodsXmlNodeByID.ToSex;
			int toZhuanSheng = goodsXmlNodeByID.ToZhuanSheng;
			string toType = goodsXmlNodeByID.ToType.ToString();
			string toTypeProperty = goodsXmlNodeByID.ToTypeProperty.ToString();
			int baoguoID = goodsXmlNodeByID.BaoguoID;
			if (!Global.CanUseGoodsByExtraTypeProperty(toType, toTypeProperty, errHint) && baoguoID == -1)
			{
				return false;
			}
			if (Global.IsRebornEquip(categoriy))
			{
				if (!ChongShengData.BeCanEquip(goodsXmlNodeByID))
				{
					if (errHint)
					{
						Super.HintMainText(StringUtil.substitute(Global.GetLang("重生等级未达到使用要求 ！"), new object[0]), 10, 3);
					}
					return false;
				}
				return true;
			}
			else
			{
				if (toSex >= 0 && Global.Data.roleData.RoleSex != toSex)
				{
					if (errHint)
					{
						Super.HintMainText(StringUtil.substitute(Global.GetLang("性别不对应，不能使用！"), new object[0]), 10, 3);
					}
					return false;
				}
				if (toZhuanSheng >= 0 && categoriy != 301 && categoriy != 302)
				{
					if (Global.Data.roleData.ChangeLifeCount == toZhuanSheng)
					{
						if (toLevel >= 0 && Global.Data.roleData.Level < toLevel)
						{
							if (errHint)
							{
								Super.HintMainText(StringUtil.substitute(Global.GetLang("只有等级达到要求才能可使用！"), new object[0]), 10, 3);
							}
							return false;
						}
					}
					else if (Global.Data.roleData.ChangeLifeCount < toZhuanSheng)
					{
						if (errHint)
						{
							Super.HintMainText(StringUtil.substitute(Global.GetLang("只有转生等级达到要求才能可使用！"), new object[0]), 10, 3);
						}
						return false;
					}
				}
				if (categoriy >= 0 && categoriy < 25)
				{
					if ((int)Global.GetCurrentRoleProp(1, 0) < goodsXmlNodeByID.Strength)
					{
						if (errHint)
						{
							Super.HintMainText(StringUtil.substitute(Global.GetLang("您当前的力量值不够，无法佩戴！"), new object[0]), 10, 3);
						}
						return false;
					}
					if ((int)Global.GetCurrentRoleProp(1, 1) < goodsXmlNodeByID.Intelligence)
					{
						if (errHint)
						{
							Super.HintMainText(StringUtil.substitute(Global.GetLang("您当前的智力值不够，无法佩戴！"), new object[0]), 10, 3);
						}
						return false;
					}
					if ((int)Global.GetCurrentRoleProp(1, 2) < goodsXmlNodeByID.Dexterity)
					{
						if (errHint)
						{
							Super.HintMainText(StringUtil.substitute(Global.GetLang("您当前的敏捷值不够，无法佩戴！"), new object[0]), 10, 3);
						}
						return false;
					}
					if ((int)Global.GetCurrentRoleProp(1, 3) < goodsXmlNodeByID.Constitution)
					{
						if (errHint)
						{
							Super.HintMainText(StringUtil.substitute(Global.GetLang("您当前的体力值不够，无法佩戴！"), new object[0]), 10, 3);
						}
						return false;
					}
				}
				return true;
			}
		}

		public static int GetGoodsDataPrice(GoodsData goodsData, bool calcTotal = true)
		{
			if (goodsData == null)
			{
				return 0;
			}
			if (goodsData.GCount <= 0)
			{
				return 0;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return 0;
			}
			int suitID = goodsXmlNodeByID.SuitID;
			int num = goodsData.Forge_level;
			if (num > 20)
			{
				num = 0;
			}
			int num2 = 0;
			int num3 = 0;
			while (num3 <= num && num3 <= Global.MaxForgeLevel)
			{
				num2 += num3;
				num3++;
			}
			int num4 = goodsData.Quality;
			if (num4 >= 7)
			{
				num4 = 6;
			}
			int num5 = 0;
			for (int i = 0; i <= num4; i++)
			{
				num5 += i;
			}
			double num6 = -1.0;
			double num7 = num6 + num6 * ((double)num5 * 0.2 + (double)num2 * 0.05);
			int num8 = (int)Math.Floor(num7);
			if (num > Global.MaxForgeLevel)
			{
				num8 += (int)((0.5 + (double)(num - 9) * 0.01) * (double)(num - 10) * num6);
			}
			if (calcTotal)
			{
				num8 *= goodsData.GCount;
			}
			return Math.Abs(num8);
		}

		public static long DateTimeTicks(string strDateTime)
		{
			try
			{
				DateTime dateTime;
				if (!DateTime.TryParse(strDateTime, ref dateTime))
				{
					return 0L;
				}
				return dateTime.Ticks;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return 0L;
		}

		public static bool DateTimeEqual(string strDateTime1, string strDateTime2)
		{
			try
			{
				DateTime dateTime;
				if (!DateTime.TryParse(strDateTime1, ref dateTime))
				{
					return false;
				}
				DateTime dateTime2;
				if (!DateTime.TryParse(strDateTime2, ref dateTime2))
				{
					return false;
				}
				return dateTime.Ticks == dateTime2.Ticks;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return false;
		}

		public static bool IsTimeLimitGoods(GoodsData goodsData)
		{
			return !string.IsNullOrEmpty(goodsData.Endtime) && !Global.DateTimeEqual(goodsData.Endtime, "1900-01-01 12:00:00");
		}

		public static bool IsGoodsTimeOver(GoodsData goodsData)
		{
			if (!Global.IsTimeLimitGoods(goodsData))
			{
				return false;
			}
			long ticks = Global.GetCorrectDateTime().Ticks;
			long num = Global.DateTimeTicks(goodsData.Endtime);
			return ticks >= num;
		}

		public static int GetTotalUsingGoodsCount()
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				if (Global.Data.roleData.GoodsDataList[i].Using > 0)
				{
					num++;
				}
			}
			return num;
		}

		public static GoodsData GetDummyGoodsData(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			return new GoodsData
			{
				Id = -1,
				GoodsID = goodsID,
				Using = -1,
				Forge_level = 0,
				Starttime = "1900-01-01 12:00:00",
				Endtime = "1900-01-01 12:00:00",
				Site = 0,
				Quality = 0,
				Props = string.Empty,
				GCount = Math.Max(1, goodsXmlNodeByID.UsingNum),
				Binding = 0,
				Jewellist = string.Empty,
				BagIndex = 0,
				AddPropIndex = 0,
				BornIndex = 0,
				Lucky = 0,
				Strong = 0,
				ExcellenceInfo = 0,
				AppendPropLev = 0,
				ChangeLifeLevForEquip = 0
			};
		}

		public static GoodsData GetDummyGoodsDataEx(int goodsID, int forgeLevel, int quality, int binding, int gcount, int bornIndex)
		{
			return new GoodsData
			{
				Id = -1,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = "1900-01-01 12:00:00",
				Site = 0,
				Quality = quality,
				Props = string.Empty,
				GCount = gcount,
				Binding = binding,
				Jewellist = string.Empty,
				BagIndex = 0,
				SaleMoney1 = 0,
				SaleYuanBao = 0,
				SaleYinPiao = 0,
				AddPropIndex = 0,
				BornIndex = bornIndex,
				Lucky = 0,
				Strong = 0,
				ExcellenceInfo = 0,
				AppendPropLev = 0,
				ChangeLifeLevForEquip = 0
			};
		}

		public static GoodsData GetDummyGoodsDataMu(int goodsID, int forgeLevel = 0, int zhuijiaLevel = 0, int zhuoyueIndex = 0, int lucky = 0, int binding = 0, int gcount = 0, int zhuanshengLevel = 0, List<int> washProps = null, string startTime = "1900-01-01 12:00:00", string endTime = "1900-01-01 12:00:00")
		{
			return new GoodsData
			{
				Id = -1,
				GoodsID = goodsID,
				Using = -1,
				Starttime = startTime,
				Endtime = endTime,
				Site = 0,
				Props = string.Empty,
				Jewellist = string.Empty,
				BagIndex = 0,
				SaleMoney1 = 0,
				SaleYuanBao = 0,
				SaleYinPiao = 0,
				AddPropIndex = 0,
				Strong = 0,
				Forge_level = forgeLevel,
				AppendPropLev = zhuijiaLevel,
				ExcellenceInfo = zhuoyueIndex,
				Lucky = lucky,
				Binding = binding,
				GCount = gcount,
				ChangeLifeLevForEquip = zhuanshengLevel,
				WashProps = washProps
			};
		}

		public static GoodsData GetDummyGoodsData(string value)
		{
			string[] array = value.Split(new char[]
			{
				','
			});
			if (array.Length >= 7)
			{
				return Global.GetDummyGoodsDataMu(int.Parse(array[0]), int.Parse(array[3]), int.Parse(array[4]), int.Parse(array[6]), int.Parse(array[5]), int.Parse(array[2]), int.Parse(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			}
			return null;
		}

		public static GoodsData CloneGoodsData(GoodsData gd, bool bCloneUsing = false)
		{
			return new GoodsData
			{
				Id = ((!bCloneUsing) ? -1 : gd.Id),
				GoodsID = gd.GoodsID,
				Using = ((!bCloneUsing) ? -1 : gd.Using),
				Starttime = "1900-01-01 12:00:00",
				Endtime = "1900-01-01 12:00:00",
				Site = gd.Site,
				Props = gd.Props,
				Jewellist = gd.Jewellist,
				BagIndex = gd.BagIndex,
				SaleMoney1 = gd.SaleMoney1,
				SaleYuanBao = gd.SaleYuanBao,
				SaleYinPiao = gd.SaleYinPiao,
				AddPropIndex = gd.AddPropIndex,
				Strong = gd.Strong,
				Forge_level = gd.Forge_level,
				AppendPropLev = gd.AppendPropLev,
				ExcellenceInfo = gd.ExcellenceInfo,
				Lucky = gd.Lucky,
				Binding = gd.Binding,
				GCount = gd.GCount,
				ChangeLifeLevForEquip = gd.ChangeLifeLevForEquip,
				WashProps = gd.WashProps,
				ElementhrtsProps = gd.ElementhrtsProps,
				JuHunID = gd.JuHunID
			};
		}

		public static GGoodIcon AddGoodsIcon(GoodsData gd, GameObject parent, bool grayShow = false, GoodsOwnerTypes type = GoodsOwnerTypes.SysGifts)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			GGoodIcon icon = null;
			if (goodsXmlNodeByID != null)
			{
				string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
				string backSpriteName = "bagGrid4_bak";
				icon = U3DUtils.NEW<GGoodIcon>();
				icon.Width = 78.0;
				icon.Height = 78.0;
				icon.BackSpriteName0 = backSpriteName;
				icon.TipType = 1;
				icon.ItemCategory = goodsXmlNodeByID.Categoriy;
				icon.ItemCode = gd.GoodsID;
				icon.ItemObject = gd;
				icon.BoxTypes = -1;
				if (!grayShow)
				{
					icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
				}
				else
				{
					icon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
				}
				bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
				Super.InitGoodsGIcon(icon, gd, canUse, IconTextTypes.Qianghua);
				icon.transform.SetParent(parent.transform, false);
				icon.gameObject.AddComponent<UIDragPanelContents>();
				icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					GGoodIcon component = icon.GetComponent<GGoodIcon>();
					if (null == component)
					{
						return;
					}
					GoodsData goodsData = icon.ItemObject as GoodsData;
					if (goodsData == null)
					{
						return;
					}
					GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, type, goodsData);
				};
			}
			return icon;
		}

		public static string GetGoods3DResNameByID(int goodsID, int Occ = -1)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return string.Empty;
			}
			if (Occ != -1 && 30 <= goodsXmlNodeByID.Categoriy && 36 >= goodsXmlNodeByID.Categoriy)
			{
				return IConfigbase<ConfigReborn>.Instance.GetRebornEquipsByGoodsIDAndOcc(goodsID, Occ);
			}
			return goodsXmlNodeByID.ResName;
		}

		public static int CheckCategoriy(int Cate)
		{
			if (30 <= Cate && 36 >= Cate)
			{
				switch (Cate)
				{
				case 30:
					return 0;
				case 31:
					return 1;
				case 32:
					return 2;
				case 33:
					return 3;
				case 34:
					return 4;
				}
			}
			return Cate;
		}

		public static string GetModelResNameByID(int modelID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/UIModel.xml");
			if (gameResXml == null)
			{
				return string.Empty;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Mod", "ID", modelID.ToString());
			if (xelement == null)
			{
				return string.Empty;
			}
			return Global.GetXElementAttributeStr(xelement, "ResName");
		}

		public static string GetModelNameByID(int modelID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/UIModel.xml");
			if (gameResXml == null)
			{
				return string.Empty;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Mod", "ID", modelID.ToString());
			if (xelement == null)
			{
				return string.Empty;
			}
			return Global.GetXElementAttributeStr(xelement, "Name");
		}

		public static string GetFallGoods3DResNameByID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return string.Empty;
			}
			return goodsXmlNodeByID.FallGoodsIcon;
		}

		public static WeaponStates GetGoodsActionNameByID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return WeaponStates.K;
			}
			return (WeaponStates)goodsXmlNodeByID.ActionType;
		}

		public static Dictionary<int, GoodsData> GetUsingGoodsDataList()
		{
			List<GoodsData> fashionAndTitleList = Global.Data.fashionAndTitleList;
			Super.GData.RoleUsingGoodsDataList.Clear();
			if (fashionAndTitleList != null)
			{
				for (int i = 0; i < fashionAndTitleList.Count; i++)
				{
					GoodsData goodsData = fashionAndTitleList[i];
					if (goodsData.Using == 1)
					{
						Super.GData.RoleUsingGoodsDataList.Add(goodsData.Id, goodsData);
					}
				}
			}
			List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
			for (int j = 0; j < roleDecorationList.Count; j++)
			{
				if (roleDecorationList[j] != null)
				{
					Super.GData.RoleUsingGoodsDataList[roleDecorationList[j].Id] = roleDecorationList[j];
				}
			}
			if (Global.Data.roleData.GoodsDataList != null)
			{
				for (int k = 0; k < Global.Data.roleData.GoodsDataList.Count; k++)
				{
					if (Global.Data.roleData.GoodsDataList[k].Using > 0)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.GoodsDataList[k].GoodsID);
						if (goodsXmlNodeByID == null || goodsXmlNodeByID.Categoriy < 40 || goodsXmlNodeByID.Categoriy > 45)
						{
							Super.GData.RoleUsingGoodsDataList[Global.Data.roleData.GoodsDataList[k].Id] = Global.Data.roleData.GoodsDataList[k];
						}
					}
				}
			}
			if (Global.Data.equipPet != null)
			{
				for (int l = 0; l < Global.Data.equipPet.Count; l++)
				{
					if (Global.Data.equipPet[l].Using > 0)
					{
						Super.GData.RoleUsingGoodsDataList[Global.Data.equipPet[l].Id] = Global.Data.equipPet[l];
					}
				}
			}
			return Super.GData.RoleUsingGoodsDataList;
		}

		public static void ClearFashionAndTitleData()
		{
			Global.Data.fashionAndTitleList = null;
			Global.Data.DecorationList = null;
			Global.m_HaveYanHuiKaiQi = false;
			Global.m_MenuIconBoxYinCan = false;
			Global.m_MenuIconBoxFlag = true;
			Global.Data.IsZhaoHuanShouLiving = 0;
			Global.Data.RoleNameColor.Clear();
			Global.zuoQiLevelByBianQiang = 0;
			Global.JieriXML_Version = 0;
			Global.ZhuanxiangXML_Version = -1;
			Global.ZhuTiFuXML_Version = -1;
			Global.everyDayXML_Version = -1;
			Global.hongBaoPaiHang_Version = -1L;
			Global.requstRetainCount = 0;
			if (Global.BianQiangStandardList != null)
			{
				Global.BianQiangStandardList.Clear();
			}
			Global.listData.Clear();
			Global.m_DicActiveStar.Clear();
			Global.lingyuTotalLevel = 0;
			Global._zuoQiLevelByBianQiang = 0;
			Global.guardStatue = null;
			Global.CurrentData = null;
			Global.shenQiData = null;
			if (null != PlayZone.GlobalPlayZone && null != PlayZone.GlobalPlayZone.GameTaskBoxMini)
			{
				PlayZone.GlobalPlayZone.GameTaskBoxMini.TaskCompletedEX = false;
			}
			ChatBox.LastChatChannel = -1;
		}

		public static void UpdateRoleUsingGoodsDataList(GoodsData gd)
		{
			if (gd == null)
			{
				return;
			}
			if (Super.GData.RoleUsingGoodsDataList.ContainsKey(gd.Id))
			{
				Super.GData.RoleUsingGoodsDataList[gd.Id] = gd;
			}
		}

		public static int[] GetGoodsShaderIDArraysByID(int goodsID)
		{
			int[] array = null;
			if (Global.GoodsShaderIDsCachingDict.TryGetValue(goodsID, ref array))
			{
				return array;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			string shaderID = goodsXmlNodeByID.ShaderID;
			array = Global.String2IntArray(shaderID, ',');
			if (array == null)
			{
				return null;
			}
			Global.GoodsShaderIDsCachingDict.Add(goodsID, array);
			return array;
		}

		public static int GetGoodsShaderIDsByID(int goodsID, int forge_Level)
		{
			if (forge_Level <= 0)
			{
				if (!Global.IsRebornEquip(Global.GetCategoriyByGoodsID(goodsID)))
				{
					return 0;
				}
				if (forge_Level < 0)
				{
					return 0;
				}
				if (forge_Level == 0)
				{
					forge_Level = 1;
				}
			}
			int[] goodsShaderIDArraysByID = Global.GetGoodsShaderIDArraysByID(goodsID);
			if (goodsShaderIDArraysByID == null || forge_Level > goodsShaderIDArraysByID.Length)
			{
				return 0;
			}
			return goodsShaderIDArraysByID[forge_Level - 1];
		}

		public static WeaponStates CalcWeaponState(List<GoodsData> weaponGoodsDataList, List<GameObject> weaponGameObjectList, int occupation)
		{
			WeaponStates weaponStates = WeaponStates.K;
			if (weaponGoodsDataList != null && weaponGoodsDataList.Count > 0)
			{
				for (int i = 0; i < weaponGoodsDataList.Count; i++)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(weaponGoodsDataList[i].GoodsID);
					if (goodsXmlNodeByID != null)
					{
						int categoriy = goodsXmlNodeByID.Categoriy;
						if (categoriy >= 11 && categoriy <= 19)
						{
							if (categoriy != 18)
							{
								WeaponStates goodsActionNameByID = Global.GetGoodsActionNameByID(weaponGoodsDataList[i].GoodsID);
								if (goodsActionNameByID == WeaponStates.D && weaponStates == WeaponStates.D)
								{
									weaponStates = WeaponStates.SD;
									break;
								}
								if (goodsActionNameByID != WeaponStates.K)
								{
									weaponStates = goodsActionNameByID;
								}
								if (weaponStates != WeaponStates.K && weaponStates != WeaponStates.D)
								{
									break;
								}
							}
						}
					}
				}
			}
			if (Global.CalcOriginalOccupationID(occupation) == 0 && weaponGameObjectList != null && weaponGameObjectList.Count > 0)
			{
				for (int j = 0; j < weaponGameObjectList.Count; j++)
				{
					if (!(null == weaponGameObjectList[j]) && !(weaponGameObjectList[j].transform == null) && !(weaponGameObjectList[j].transform.parent == null))
					{
						if (weaponGameObjectList[j].transform.parent.name == "zuoshou")
						{
							weaponStates = WeaponStates.SD;
							break;
						}
					}
				}
			}
			return weaponStates;
		}

		public static void ToUseGoodsDBId(int goodsDBId, bool needCheck = true)
		{
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(goodsDBId, null);
			if (goodsDataByDbID != null)
			{
				Global.ToUseGoods(goodsDataByDbID, needCheck, false);
			}
		}

		public static void ToUseGoods(GoodsData goodsData_, bool needCheck = true, bool isBatchUse = false)
		{
			GoodsData goodsData = goodsData_.Clone();
			if (needCheck)
			{
				if (!Global.CanUseGoods(goodsData.GoodsID, true, true))
				{
					return;
				}
				if (!Global.CanDirectUseGoods(goodsData.GoodsID, true))
				{
					return;
				}
				if (!Global.CheckBuffer(goodsData))
				{
					return;
				}
				if (Global.GetDecorationHaveActive(goodsData.GoodsID))
				{
					return;
				}
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int category = goodsXmlNodeByID.Categoriy;
			if (category >= 0 && category < 25)
			{
				int actionType = goodsXmlNodeByID.ActionType;
				int handType = goodsXmlNodeByID.HandType;
				List<GoodsData> list;
				if (category >= 11 && category <= 21)
				{
					list = Super.FindWuQi(category, actionType, handType);
					Global.currentMJSType = Global.GetMJSType();
				}
				else
				{
					list = Super.FindEquip(category);
				}
				if (category != 10 && category != 9 && category != 24 && category != 23)
				{
					int baoGuoSpaceCount = Global.GetBaoGuoSpaceCount();
					if (baoGuoSpaceCount < list.Count)
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("背包已满，请先清理出空闲位置后，再卸载装备..."), new object[0]), 1, -1, -1, 0);
						return;
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						int bagIndex = goodsData.BagIndex;
						if (list[i].Using == 1)
						{
							list[i].Using = 0;
							int handType2 = ConfigGoods.GetGoodsXmlNodeByID(list[i].GoodsID).HandType;
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(list[i].GoodsID);
							if (goodsXmlNodeByID2.Categoriy != 17 || Global.Data.roleData.Occupation != 3 || goodsXmlNodeByID2.ActionType != 1)
							{
								if (category != 6 && handType2 != 2)
								{
									list[i].BagIndex = bagIndex;
								}
							}
							if (category != 24 && category != 23 && category != 8 && category != 27)
							{
								GameInstance.Game.SpriteModGoods(2, list[i].Id, list[i].GoodsID, list[i].Using, list[i].Site, list[i].GCount, list[i].BagIndex, string.Empty);
								if (Super.GData.RoleUsingGoodsDataList.ContainsKey(list[i].Id))
								{
									Super.GData.RoleUsingGoodsDataList.Remove(list[i].Id);
								}
							}
						}
					}
				}
				if (goodsData.Using == 0)
				{
					if (category == 23 || category == 24 || category == 8 || (25 <= category && 27 >= category) || category == 28)
					{
						goodsData.Using = 0;
					}
					else
					{
						goodsData.Using = 1;
					}
					if (category == 6 || handType == 2)
					{
						if (category >= 11 && category <= 21)
						{
							category = 11;
						}
						if (category == 10)
						{
							category = 9;
						}
						goodsData.BagIndex = Super.FindEquipBagIndex(category);
					}
					if (Global.Data.roleData.Occupation == 3 && category == 17)
					{
						goodsData.BagIndex = Super.FindEquipBagIndex(category);
					}
					if (category == 18)
					{
						goodsData.BagIndex = Super.FindEquipBagIndex(category);
					}
					if (category == 18)
					{
						goodsData.BagIndex = Super.FindEquipBagIndex(category);
					}
					if (category == 24 || category == 8)
					{
						GameInstance.Game.SendActivateFashion(goodsData.Id);
					}
					else if (category == 23)
					{
						GameInstance.Game.SendActiveShiPin(goodsData.GoodsID, goodsData.Id);
					}
					else
					{
						GameInstance.Game.SpriteModGoods(1, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
					}
				}
			}
			if (Global.IsRebornEquip(category))
			{
				int actionType2 = goodsXmlNodeByID.ActionType;
				int handType3 = goodsXmlNodeByID.HandType;
				List<GoodsData> list2 = Super.FindChongShengEquip(category);
				int rebornBaoGuoSpaceCount = Global.GetRebornBaoGuoSpaceCount();
				if (rebornBaoGuoSpaceCount < list2.Count)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("重生背包已满，请先清理出空闲位置后，再卸载装备..."), new object[0]), 1, -1, -1, 0);
					return;
				}
				for (int j = 0; j < list2.Count; j++)
				{
					if (list2[j] != null)
					{
						int bagIndex2 = goodsData.BagIndex;
						if (list2[j].Using == 1)
						{
							list2[j].Using = 0;
							int handType4 = ConfigGoods.GetGoodsXmlNodeByID(list2[j].GoodsID).HandType;
							GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(list2[j].GoodsID);
							if (category != 6 && handType4 != 2)
							{
								list2[j].BagIndex = bagIndex2;
							}
							GameInstance.Game.SpriteModGoods(2, list2[j].Id, list2[j].GoodsID, list2[j].Using, list2[j].Site, list2[j].GCount, list2[j].BagIndex, string.Empty);
							if (Super.GData.RoleUsingChongShengGoodsDataList != null && Super.GData.RoleUsingChongShengGoodsDataList.ContainsKey(list2[j].Id))
							{
								Super.GData.RoleUsingChongShengGoodsDataList.Remove(list2[j].Id);
							}
						}
					}
				}
				if (goodsData.Using == 0)
				{
					goodsData.Using = 1;
					if (category == 36)
					{
						goodsData.BagIndex = Super.FindEquipBagIndex(category);
					}
					GameInstance.Game.SpriteModGoods(1, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
			}
			else if (25 <= category && 27 >= category)
			{
				if (category == 27 && !UIHelper.IsGongNengOpenedOrHint(GongNengIDs.Horese, true))
				{
					return;
				}
				GameInstance.Game.SendActivateFashion(goodsData.Id);
			}
			else if (category == 28)
			{
				GameInstance.Game.SendActivateFashion(goodsData.Id);
			}
			else if (category == 340)
			{
				if (goodsData.Using == 0)
				{
					goodsData.Using = 1;
					GameInstance.Game.SpriteModGoods(1, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
			}
			else if (category >= 40 && category <= 45)
			{
				if (!UIHelper.IsGongNengOpenedOrHint(GongNengIDs.Horese, true))
				{
					return;
				}
				if (goodsData != null && !Global.HorseEquipOpen(goodsData.GoodsID))
				{
					Super.HintMainText(Global.GetLang("本功能未开启"), 10, 3);
					return;
				}
				GoodsData goodsData2 = Global.GetRoleHorseEquipGoodsDataList(Global.Data.RoleID, 1).Find((GoodsData e) => category == Global.GetCategoriyByGoodsID(e.GoodsID));
				if (goodsData2 != null)
				{
					goodsData2 = goodsData2.Clone();
					goodsData2.Using = 0;
					GameInstance.Game.SpriteModGoods(2, goodsData2.Id, goodsData2.GoodsID, goodsData2.Using, goodsData2.Site, goodsData2.GCount, goodsData2.BagIndex, string.Empty);
				}
				if (goodsData.Using == 0)
				{
					goodsData.Using = 1;
					GameInstance.Game.SpriteModGoods(1, goodsData.Id, goodsData.GoodsID, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, string.Empty);
				}
			}
			else if (!Global.Data.GameScene.IsInStalling() && !Super.IsDisableUsingGoods())
			{
				GoodsData goodsData3 = goodsData;
				if (goodsData3 != null)
				{
					bool flag = Global.GetGoodsUsingModeByGoodsID(goodsData3.GoodsID) > 0;
					if (flag)
					{
						if (!Global.GoodsCoolDown(goodsData3.GoodsID))
						{
							if (isBatchUse && goodsData3.GCount > 1)
							{
								if (goodsXmlNodeByID.Categoriy == 121)
								{
									GameInstance.Game.SpriteUseGoods(goodsData3.Id, goodsData3.GoodsID, goodsData.GCount);
								}
								else
								{
									(Super.GData.PlayZoneRoot as PlayZone).ToShowUI(goodsData3.Id);
								}
							}
							else if (goodsXmlNodeByID.Categoriy == 704)
							{
								GameInstance.Game.SendTOUseTaLuopaiSuiPian(goodsData3.Id, goodsData3.GoodsID, 1);
							}
							else
							{
								GameInstance.Game.SpriteUseGoods(goodsData3.Id, goodsData3.GoodsID, 1);
							}
						}
						else
						{
							string goodsNameByID = Global.GetGoodsNameByID(goodsData3.GoodsID, false);
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("【{0}】在冷却中, 无法使用"), new object[]
							{
								goodsNameByID
							}), 0, -1, -1, 0);
						}
					}
					else
					{
						int glUI = goodsXmlNodeByID.GlUI;
						if (glUI > 0)
						{
							(Super.GData.PlayZoneRoot as PlayZone).ToShowUI(glUI, goodsData3.GoodsID);
						}
					}
				}
			}
		}

		private static bool CheckBuffer(GoodsData goodsData)
		{
			int goodsBufferID = Global.GetGoodsBufferID(goodsData.GoodsID);
			if (goodsBufferID < 0)
			{
				return true;
			}
			if (goodsBufferID == 29)
			{
				return true;
			}
			bool flag = false;
			if (goodsBufferID == 13)
			{
				return true;
			}
			if (goodsBufferID == 48 || goodsBufferID == 37 || goodsBufferID == 38 || goodsBufferID == 64 || goodsBufferID == 65)
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(goodsBufferID);
				if (bufferDataByID == null)
				{
					return true;
				}
				if (Global.IsBufferDataOver(bufferDataByID, 0L, false))
				{
					return true;
				}
				int num = (int)(bufferDataByID.BufferVal & (long)((ulong)-1));
				double num2 = (double)(bufferDataByID.BufferVal - (long)num) / Math.Pow(2.0, 32.0);
				if ((double)goodsData.GoodsID == num2)
				{
					return true;
				}
				int num3 = (int)bufferDataByID.BufferVal;
				if (goodsData.GoodsID == num3)
				{
					return true;
				}
				flag = true;
			}
			List<int> list = new List<int>();
			list.Add(46);
			list.Add(36);
			list.Add(18);
			list.Add(1);
			List<int> list2 = list;
			int num4 = list2.IndexOf(goodsBufferID);
			if (num4 >= 0)
			{
				bool flag2 = false;
				if (Global.GetBufferDataByGoodsID(goodsData.GoodsID) != null)
				{
					flag2 = false;
				}
				else
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (Global.IsBufferExist(list2[i]))
						{
							flag2 = true;
						}
					}
				}
				if (!flag2)
				{
					return true;
				}
				flag = true;
			}
			list = new List<int>();
			list.Add(24);
			list.Add(26);
			list.Add(25);
			List<int> list3 = list;
			num4 = list3.IndexOf(goodsBufferID);
			if (num4 >= 0 && Global.IsBufferExist(39))
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您已拥有怒斩·PK王BUFFER,不能使用该物品"), new object[0]), 0, -1, -1, 0);
				return false;
			}
			if (!flag && !Global.IsBufferExist(goodsBufferID))
			{
				return true;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				string title = goodsXmlNodeByID.Title;
				if (null != Super._ParcelPart)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super._ParcelPart.Container, 1, Global.GetLang("提示"), StringUtil.substitute(Global.GetLang("{0}关联buffer已经存在，使用后将覆盖原有状态，继续使用吗？"), new object[]
					{
						title
					}), (int)((Super._ParcelPart.Width - 253.0) / 2.0), (int)((Super._ParcelPart.Height - 171.0) / 2.0), (int)Super._ParcelPart.Width, (int)Super._ParcelPart.Height, 0.01, -Super._ParcelPart.transform.parent.localPosition, null, null);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(Super._ParcelPart.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.ToUseGoods(goodsData, false, false);
						}
						else if (messageBoxReturn == 1)
						{
						}
						return true;
					};
				}
			}
			return false;
		}

		public static bool IsEquip(int goodsID)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsID);
			return (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25 && categoriyByGoodsID != 6 && categoriyByGoodsID != 5) || (Global.IsRebornEquip(categoriyByGoodsID) && categoriyByGoodsID != 35 && categoriyByGoodsID != 36);
		}

		public static void SetEquipGoodsZhanLiStat(GGoodIcon icon, GoodsData goodsData)
		{
			if (icon == null || goodsData == null)
			{
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if ((categoriy >= 0 && categoriy < 25) || (categoriy >= 40 && categoriy <= 45))
			{
				if (categoriy == 23)
				{
					return;
				}
				string toType = goodsXmlNodeByID.ToType.ToString();
				string toTypeProperty = goodsXmlNodeByID.ToTypeProperty.ToString();
				string text = goodsXmlNodeByID.BaoguoID.ToString();
				if (!Global.CanUseGoodsByExtraTypeProperty(toType, toTypeProperty, false) && string.IsNullOrEmpty(text))
				{
					return;
				}
				if ((1 << Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) & goodsXmlNodeByID.ToOccupation) == 0)
				{
					return;
				}
				double num = 0.0;
				int actionType = goodsXmlNodeByID.ActionType;
				int handType = goodsXmlNodeByID.HandType;
				List<GoodsData> list;
				if (categoriy >= 11 && categoriy <= 21)
				{
					list = Super.FindWuQi(categoriy, actionType, handType);
				}
				else
				{
					list = Super.FindEquip(categoriy);
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						num += Global.GetGoodsDataZhanLi(list[i]);
					}
				}
				if (categoriy >= 40 && categoriy <= 45 && Global.Data.roleData != null)
				{
					for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
					{
						if (Global.Data.roleData.GoodsDataList[j].Using > 0)
						{
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(Global.Data.roleData.GoodsDataList[j].GoodsID);
							if (goodsXmlNodeByID2.Categoriy == categoriy)
							{
								num = Global.GetGoodsDataZhanLi(Global.Data.roleData.GoodsDataList[j]);
							}
						}
					}
				}
				double goodsDataZhanLi = Global.GetGoodsDataZhanLi(goodsData);
				if (goodsDataZhanLi > num)
				{
					icon.ZhanLiSprite.gameObject.SetActive(true);
					icon.ZhanLiSprite.spriteName = "up";
				}
				else if (goodsDataZhanLi < num)
				{
					icon.ZhanLiSprite.gameObject.SetActive(true);
					icon.ZhanLiSprite.spriteName = "down";
				}
				else
				{
					icon.ZhanLiSprite.gameObject.SetActive(false);
				}
			}
		}

		public static bool IsCanUseGoodsByGongnengID(int goodsID)
		{
			if (Global.CanUseGoodsByTaskDict == null)
			{
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ItemUse", '|');
				if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length <= 0)
				{
					return true;
				}
				Global.CanUseGoodsByTaskDict = new Dictionary<int, int>();
				for (int i = 0; i < systemParamStringArrayByName.Length; i++)
				{
					string[] array = systemParamStringArrayByName[i].Split(new char[]
					{
						','
					});
					int num = array[0].SafeToInt32(0);
					int num2 = array[1].SafeToInt32(0);
					if (!Global.CanUseGoodsByTaskDict.ContainsKey(num))
					{
						Global.CanUseGoodsByTaskDict.Add(num, num2);
					}
				}
			}
			int id = 0;
			if (!Global.CanUseGoodsByTaskDict.TryGetValue(goodsID, ref id))
			{
				return true;
			}
			int trigger = 0;
			int param = 0;
			int param2 = 0;
			if (GongnengYugaoMgr.IsGongNengOpened((GongNengIDs)id, ref trigger, ref param, ref param2))
			{
				return true;
			}
			UIHelper.HintGongNengOpenCondition((GongNengIDs)id, trigger, param, param2, true);
			return false;
		}

		public static XElement GetWingXmlNodeByID(int id, int occupation)
		{
			if (id <= 0)
			{
				return null;
			}
			occupation = Global.CalcOriginalOccupationID(occupation);
			Dictionary<int, XElement> dictionary = null;
			if (!Global.WingDict.TryGetValue(occupation, ref dictionary))
			{
				dictionary = new Dictionary<int, XElement>();
				Global.WingDict.Add(occupation, dictionary);
			}
			XElement xelement = null;
			if (dictionary != null && dictionary.TryGetValue(id, ref xelement))
			{
				return xelement;
			}
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/Wing/Wing_{0}.xml", occupation));
			if (gameResXml == null)
			{
				return null;
			}
			xelement = Global.GetXElement(gameResXml, "Level", "ID", id.ToString());
			if (xelement == null)
			{
				return null;
			}
			dictionary[id] = xelement;
			return xelement;
		}

		public static GoodsData GetSaleGoodsDataByDbID(int id)
		{
			if (Global.Data.SaleGoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.SaleGoodsDataList.Count; i++)
			{
				if (Global.Data.SaleGoodsDataList[i].Id == id)
				{
					return Global.Data.SaleGoodsDataList[i];
				}
			}
			return null;
		}

		public static bool RemoveSaleGoodsData(GoodsData gd)
		{
			if (gd == null)
			{
				return false;
			}
			if (Global.Data.SaleGoodsDataList == null)
			{
				return false;
			}
			int num = Global.Data.SaleGoodsDataList.IndexOf(gd);
			if (num >= 0)
			{
				Global.Data.SaleGoodsDataList.RemoveAt(num);
			}
			return num >= 0;
		}

		public static void AddSaleGoodsData(GoodsData gd)
		{
			if (gd == null)
			{
				return;
			}
			if (Global.Data.SaleGoodsDataList == null)
			{
				Global.Data.SaleGoodsDataList = new List<GoodsData>();
			}
			Global.Data.SaleGoodsDataList.Add(gd);
		}

		public static GoodsData GetOtherSaleGoodsDataByDbID(int id)
		{
			if (Global.Data.OtherSaleGoodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.OtherSaleGoodsDataList.Count; i++)
			{
				if (Global.Data.OtherSaleGoodsDataList[i].Id == id)
				{
					return Global.Data.OtherSaleGoodsDataList[i];
				}
			}
			return null;
		}

		public static void UpdateCompleteTaskID(int taskID)
		{
			if (Global.GetTaskClassByID(taskID) == 0)
			{
				Global.Data.roleData.CompletedMainTaskID = taskID;
			}
		}

		public static string GetTaskTitleByID(int id)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(id);
			if (taskXmlNodeByID == null)
			{
				return null;
			}
			return taskXmlNodeByID.Title;
		}

		public static int GetTaskClassByID(int id)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(id);
			if (taskXmlNodeByID == null)
			{
				return -1;
			}
			return taskXmlNodeByID.TaskClass;
		}

		public static bool CompleteTaskZhangJieByTask(TaskVO taskVO, out int zhangJieID)
		{
			zhangJieID = 0;
			if (taskVO != null && taskVO.TaskZhangJieID > 0)
			{
				TaskZhangJieVO taskZhangJieVO = ConfigTasks.GetTaskZhangJieVO(taskVO.TaskZhangJieID);
				if (taskZhangJieVO != null && taskZhangJieVO.EndTaskID == taskVO.ID)
				{
					zhangJieID = taskZhangJieVO.ID;
					return true;
				}
			}
			return false;
		}

		public static string GetTaskPropNameByID(int taskID)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return null;
			}
			string text = StringUtil.substitute("PropsName{0}", new object[]
			{
				1
			});
			return taskXmlNodeByID.PropsName1;
		}

		public static int GetTaskTeleportsByID(int taskID)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return 0;
			}
			return taskXmlNodeByID.Teleports;
		}

		public static void ParsePropNameInfo(string propName, out string goodsName, out int forge_level, out int quality)
		{
			goodsName = propName;
			forge_level = 0;
			quality = 0;
			if (string.IsNullOrEmpty(propName))
			{
				return;
			}
			string[] array = propName.Split(new char[]
			{
				'|'
			});
			if (array == null || array.Length < 3)
			{
				return;
			}
			goodsName = array[0];
			forge_level = Convert.ToInt32(array[1]);
			quality = Convert.ToInt32(array[2]);
		}

		public static TaskData GetTaskDataByID(int id)
		{
			if (Global.Data.roleData.TaskDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				if (id == Global.Data.roleData.TaskDataList[i].DoingTaskID)
				{
					return Global.Data.roleData.TaskDataList[i];
				}
			}
			return null;
		}

		public static TaskStarInfo GetTaskStarInfo(int starLevel)
		{
			TaskStarInfo result = null;
			if (Global.TaskStarInfoDict == null)
			{
				Global.TaskStarInfoDict = new Dictionary<int, TaskStarInfo>();
				XElement isolateResXml = Global.GetIsolateResXml("Config/TaskStarInfos.xml");
				if (isolateResXml == null)
				{
					return null;
				}
				List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Star");
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					TaskStarInfo taskStarInfo = new TaskStarInfo();
					taskStarInfo.StarLevel = Global.GetXElementAttributeInt(xelement, "ID");
					taskStarInfo.ExpModule = (float)Global.GetXElementAttributeDouble(xelement, "EXPXiShu");
					taskStarInfo.BindZuanModule = (float)Global.GetXElementAttributeDouble(xelement, "BindZhuanShiXiShu");
					taskStarInfo.XingHunXiShu = Mathf.Max(0f, (float)Global.GetXElementAttributeDouble(xelement, "XingHunXiShu"));
					taskStarInfo.Gailv = (float)Global.GetXElementAttributeDouble(xelement, "GaiLv");
					Global.TaskStarInfoDict.Add(taskStarInfo.StarLevel, taskStarInfo);
				}
			}
			if (Global.TaskStarInfoDict != null && !Global.TaskStarInfoDict.TryGetValue(starLevel, ref result))
			{
				result = null;
			}
			return result;
		}

		public static DailyTaskData FindDailyTaskDataByTaskClass(int taskClass)
		{
			if (Global.Data.roleData.MyDailyTaskDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.MyDailyTaskDataList.Count; i++)
			{
				if (Global.Data.roleData.MyDailyTaskDataList[i].TaskClass == taskClass)
				{
					return Global.Data.roleData.MyDailyTaskDataList[i];
				}
			}
			return null;
		}

		public static int GetPaoHuanID(XElement taskXmlNode)
		{
			return 1;
		}

		public static int GetPaoHuanID(TaskVO taskVO)
		{
			return 1;
		}

		public static int GetPaoHuanMaxNum(XElement taskXmlNode)
		{
			int paoHuanID = Global.GetPaoHuanID(taskXmlNode);
			int r = (int)ConfigSystemParam.GetSystemParamIntByName(string.Format("DailyTaskMaxNum{0}", paoHuanID));
			return Global.GMax(0, r);
		}

		private static int GetMaxDailyTaskNumByTaskXmlMode(XElement taskXmlNode)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXmlNode, "TaskClass");
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(xelementAttributeInt);
			return Global.GetMaxDailyTaskNum(xelementAttributeInt, dailyTaskData);
		}

		private static int GetMaxDailyTaskNumByTaskXmlMode(TaskVO taskVO)
		{
			int taskClass = taskVO.TaskClass;
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(taskClass);
			return Global.GetMaxDailyTaskNum(taskClass, dailyTaskData);
		}

		public static int GetMaxDailyTaskNum(int taskClass, DailyTaskData dailyTaskData)
		{
			int result = 0;
			int dayOfYear = DateTime.Now.DayOfYear;
			int num = 0;
			if (dailyTaskData != null && dayOfYear == dailyTaskData.ExtDayID)
			{
				num = Math.Max(0, dailyTaskData.ExtNum);
			}
			if (taskClass == 3)
			{
				if (Global.IsVip())
				{
					return 15 + num;
				}
				result = 10 + num;
			}
			else if (taskClass == 4)
			{
				result = 10 + num;
			}
			else if (taskClass == 5)
			{
				result = 6 + num;
			}
			else if (taskClass == 6)
			{
				result = 1 + num;
			}
			else if (taskClass == 7)
			{
				result = 10 + num;
			}
			else if (taskClass == 8)
			{
				result = 10 + num;
			}
			else if (taskClass == 9)
			{
				result = 5 + num;
			}
			return result;
		}

		public static int GetPaoHuanNum(XElement taskXmlNode)
		{
			if (Global.Data.roleData.MyDailyTaskDataList == null)
			{
				return 1;
			}
			int paoHuanID = Global.GetPaoHuanID(taskXmlNode);
			int xelementAttributeInt = Global.GetXElementAttributeInt(taskXmlNode, "TaskClass");
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(xelementAttributeInt);
			if (dailyTaskData == null)
			{
				return 1;
			}
			return dailyTaskData.RecNum + 1;
		}

		public static int GetPaoHuanNum(TaskVO taskVO)
		{
			if (Global.Data.roleData.MyDailyTaskDataList == null)
			{
				return 1;
			}
			int paoHuanID = Global.GetPaoHuanID(taskVO);
			int taskClass = taskVO.TaskClass;
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(taskClass);
			if (dailyTaskData == null)
			{
				return 1;
			}
			return dailyTaskData.RecNum + 1;
		}

		public static string GetPaoHuanTaskTitle(XElement taskXmlNode, string title)
		{
			return StringUtil.substitute(Global.GetLang("{0}（{1}/{2}）"), new object[]
			{
				title,
				Global.GetPaoHuanNum(taskXmlNode),
				Global.GetMaxDailyTaskNumByTaskXmlMode(taskXmlNode)
			});
		}

		public static string GetPaoHuanTaskTitle(TaskVO taskVO, string title)
		{
			return StringUtil.substitute(Global.GetLang("{0}（{1}/{2}）"), new object[]
			{
				title,
				Global.GetPaoHuanNum(taskVO),
				Global.GetMaxDailyTaskNumByTaskXmlMode(taskVO)
			});
		}

		public static bool FindPaoHuanTask(int taskClass)
		{
			if (Global.Data.roleData.TaskDataList == null)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(Global.Data.roleData.TaskDataList[i].DoingTaskID);
				if (taskXmlNodeByID != null)
				{
					if (taskClass == taskXmlNodeByID.TaskClass)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public static bool CanTaskPaoHuanTask(int taskClass, out int recNum, out int dailyTaskMaxNum, bool ignoreCanTake = false)
		{
			recNum = 0;
			dailyTaskMaxNum = 0;
			if (!ignoreCanTake && Global.FindPaoHuanTask(taskClass))
			{
				return false;
			}
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(taskClass);
			dailyTaskMaxNum = Global.GetMaxDailyTaskNum(taskClass, dailyTaskData);
			dailyTaskMaxNum = Math.Max(0, dailyTaskMaxNum);
			if (dailyTaskData == null)
			{
				recNum = 1;
				return true;
			}
			string text = Global.GetCorrectDateTime().ToString("yyyy-MM-dd");
			if (dailyTaskData.RecTime == text)
			{
				recNum = dailyTaskData.RecNum + 1;
				return dailyTaskData.RecNum < dailyTaskMaxNum;
			}
			if (dailyTaskData.RecNum >= dailyTaskMaxNum)
			{
				dailyTaskData.RecNum = 0;
			}
			recNum = dailyTaskData.RecNum + 1;
			return true;
		}

		public static bool CanDoPaoHuanTask(int taskClass)
		{
			int num;
			int num2;
			return Global.CanDoPaoHuanTask(taskClass, out num, out num2);
		}

		public static bool CanDoPaoHuanTask(int taskClass, out int recNum, out int dailyTaskMaxNum)
		{
			recNum = 0;
			dailyTaskMaxNum = 0;
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(taskClass);
			dailyTaskMaxNum = Global.GetMaxDailyTaskNum(taskClass, dailyTaskData);
			dailyTaskMaxNum = Math.Max(0, dailyTaskMaxNum);
			if (dailyTaskData == null)
			{
				recNum = 1;
				return true;
			}
			string text = Global.GetCorrectDateTime().ToString("yyyy-MM-dd");
			if (dailyTaskData.RecTime == text)
			{
				recNum = dailyTaskData.RecNum + 1;
				return dailyTaskData.RecNum < dailyTaskMaxNum;
			}
			recNum = dailyTaskData.RecNum + 1;
			return true;
		}

		public static int GetTaskYuanBaoCompeteNum(int taskID)
		{
			TaskVO taskXmlNodeByID = ConfigTasks.GetTaskXmlNodeByID(taskID);
			if (taskXmlNodeByID == null)
			{
				return 0;
			}
			return Math.Max(0, taskXmlNodeByID.YuanBaoComplete);
		}

		public static string GetDailyTasksInfoStr()
		{
			string text = string.Empty;
			DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(3);
			int num = 0;
			if (dailyTaskData != null)
			{
				num = dailyTaskData.RecNum;
			}
			int num2 = Global.GetMaxDailyTaskNum(3, dailyTaskData);
			num2 -= num;
			if (num2 > 0)
			{
				text += StringUtil.substitute(Global.GetLang("★猎杀日常剩余{0}次\r\n"), new object[]
				{
					num2
				});
			}
			dailyTaskData = Global.FindDailyTaskDataByTaskClass(4);
			num = 0;
			if (dailyTaskData != null)
			{
				num = dailyTaskData.RecNum;
			}
			num2 = Global.GetMaxDailyTaskNum(4, dailyTaskData);
			num2 -= num;
			if (num2 > 0)
			{
				text = StringUtil.substitute(Global.GetLang("★武学日常剩余{0}次\r\n"), new object[]
				{
					num2
				});
			}
			dailyTaskData = Global.FindDailyTaskDataByTaskClass(5);
			num = 0;
			if (dailyTaskData != null)
			{
				num = dailyTaskData.RecNum;
			}
			num2 = Global.GetMaxDailyTaskNum(5, dailyTaskData);
			num2 -= num;
			if (num2 > 0)
			{
				text = StringUtil.substitute(Global.GetLang("★军功日常剩余{0}次\r\n"), new object[]
				{
					num2
				});
			}
			dailyTaskData = Global.FindDailyTaskDataByTaskClass(7);
			num = 0;
			if (dailyTaskData != null)
			{
				num = dailyTaskData.RecNum;
			}
			num2 = Global.GetMaxDailyTaskNum(7, dailyTaskData);
			num2 -= num;
			if (num2 > 0)
			{
				text = StringUtil.substitute(Global.GetLang("★战盟日常剩余{0}次\r\n"), new object[]
				{
					num2
				});
			}
			return text;
		}

		public static int GetFirstMainTaskID()
		{
			if (Global.FirstMainTaskID <= 0)
			{
				Global.FirstMainTaskID = (int)ConfigSystemParam.GetSystemParamIntByName("FirstMainTaskID");
			}
			return Global.FirstMainTaskID;
		}

		public static bool CanDoJingYanFuBen()
		{
			int num = 0;
			int num2 = 0;
			Global.GetJingYanFuBenID(out num, out num2);
			if (num <= 0 || num2 <= 0)
			{
				return false;
			}
			FuBenData fuBenData = Global.GetFuBenData(num2);
			return fuBenData == null || fuBenData.EnterNum < num;
		}

		private static void GetJingYanFuBenID(out int maxCount, out int fuBenID)
		{
			Global.riChangFuBenItems.Clear();
			Global.LoadRiChangFuBenItemConfig(500);
			maxCount = -1;
			fuBenID = -1;
			if (Global.riChangFuBenItems.Count <= 0)
			{
				return;
			}
			Global.JingYanFuBenConfigData jingYanFuBenConfigData = Global.riChangFuBenItems.Find((Global.JingYanFuBenConfigData result) => result.LevelAllow);
			if (jingYanFuBenConfigData != null)
			{
				maxCount = jingYanFuBenConfigData.MaxEnterNum;
				fuBenID = jingYanFuBenConfigData.MapCode;
			}
		}

		private static void LoadRiChangFuBenItemConfig(int tab = 500)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (Global.GetXElementAttributeInt(xelement, "TabID") == tab)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
					Global.JingYanFuBenConfigData jingYanFuBenConfigData = new Global.JingYanFuBenConfigData();
					jingYanFuBenConfigData.MinZhuanSheng = xelementAttributeInt3;
					jingYanFuBenConfigData.MaxZhuanSheng = xelementAttributeInt4;
					jingYanFuBenConfigData.MinLevel = xelementAttributeInt;
					jingYanFuBenConfigData.MaxLevel = xelementAttributeInt2;
					jingYanFuBenConfigData.MapCode = Global.GetXElementAttributeInt(xelement, "MapCode");
					jingYanFuBenConfigData.MaxEnterNum = Global.GetXElementAttributeInt(xelement, "EnterNumber");
					jingYanFuBenConfigData.MaxFinishNum = Global.GetXElementAttributeInt(xelement, "FinishNumber");
					jingYanFuBenConfigData.LevelAllow = (0 == UIHelper.AvalidLevel(xelementAttributeInt, xelementAttributeInt2, xelementAttributeInt3, xelementAttributeInt4));
					Global.riChangFuBenItems.Add(jingYanFuBenConfigData);
				}
			}
		}

		public static bool SkillCoolDown(int magicCode)
		{
			CoolDownItem coolDownItem = null;
			if (!Global.SkillCoolDownDict.TryGetValue(magicCode, ref coolDownItem))
			{
				return false;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			return correctLocalTime <= coolDownItem.StartTicks + coolDownItem.CDTicks;
		}

		public static long GetSkillCoolDownTicks(int magicCode)
		{
			CoolDownItem coolDownItem = null;
			if (!Global.SkillCoolDownDict.TryGetValue(magicCode, ref coolDownItem))
			{
				return 0L;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime > coolDownItem.StartTicks + coolDownItem.CDTicks)
			{
				return 0L;
			}
			long num = coolDownItem.StartTicks + coolDownItem.CDTicks - correctLocalTime;
			if (num > 3L * coolDownItem.CDTicks)
			{
				return 0L;
			}
			return num;
		}

		public static void RemoveSkillCoolDownTicks(int magicCode)
		{
			if (Global.SkillCoolDownDict.ContainsKey(magicCode))
			{
				Global.SkillCoolDownDict.Remove(magicCode);
			}
			if (Global.NotifyAddCoolDown != null)
			{
				Global.NotifyAddCoolDown.Invoke(null, new CoolDownEventArgs
				{
					SkillID = magicCode,
					ToDrawTicks = 0L
				});
			}
		}

		public static void AddCoolDownItem(Dictionary<int, CoolDownItem> coolDownDict, int id, long startTicks, long cdTicks)
		{
			CoolDownItem coolDownItem = null;
			coolDownDict.TryGetValue(id, ref coolDownItem);
			if (coolDownItem == null)
			{
				coolDownItem = new CoolDownItem
				{
					ID = id,
					StartTicks = startTicks,
					CDTicks = cdTicks
				};
				coolDownDict[id] = coolDownItem;
			}
			else if (startTicks + cdTicks > coolDownItem.StartTicks + coolDownItem.CDTicks)
			{
				coolDownItem.StartTicks = startTicks;
				coolDownItem.CDTicks = cdTicks;
			}
		}

		public static void ChangeEmblemCoolDownData(long StartTime = -1L, long BufferSecs = -1L)
		{
			if (Global.Data.RoleEmblemData == null)
			{
				Global.Data.RoleEmblemData = new EmblemCoolDownItem();
			}
			Global.Data.RoleEmblemData.ChangeRoleEmblemItem(StartTime, BufferSecs);
			if (Global.NotifyEmblemAddCoolDown != null)
			{
				Global.NotifyEmblemAddCoolDown.Invoke(null, new EmblemCoolDownEventArgs
				{
					EmblemID = Global.Data.RoleEmblemData.ID,
					CDTicks = Global.Data.RoleEmblemData.GetCDTicks(),
					ContinuedTicks = Global.Data.RoleEmblemData.GetContinuedTicks()
				});
			}
		}

		public static bool IsEmblemInCool()
		{
			return Global.GetEmblemItem().IsEmblemInCool();
		}

		public static EmblemCoolDownItem GetEmblemItem()
		{
			return Global.Data.RoleEmblemData;
		}

		public static int GetRoleEmblemLevel(int RoleID)
		{
			RoleData roleData = null;
			if (RoleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (Global.Data.OtherRoles.ContainsKey(RoleID))
			{
				roleData = Global.Data.OtherRoles[RoleID];
			}
			if (roleData != null && roleData.HuiJiData != null)
			{
				if (Global.m_DicStar == null || Global.m_DicStar.Count <= 0)
				{
					Global.m_DicStar = Global.AddDicEmblemStart();
				}
				if (Global.m_DicStar.ContainsKey(roleData.HuiJiData.huiji))
				{
					return Global.m_DicStar[roleData.HuiJiData.huiji].EmblemLevel;
				}
			}
			return 0;
		}

		public static BufferData GetEmblemBuffData(int RoleID)
		{
			BufferData result = null;
			if (RoleID == Global.Data.roleData.RoleID)
			{
				if (Global.Data.roleData.BufferDataList != null)
				{
					result = Global.Data.roleData.BufferDataList.Find((BufferData e) => 116 == e.BufferID);
				}
			}
			else if (Global.Data.OtherRoles.ContainsKey(RoleID) && Global.Data.OtherRoles[RoleID].BufferDataList != null)
			{
				result = Global.Data.OtherRoles[RoleID].BufferDataList.Find((BufferData e) => 116 == e.BufferID);
			}
			return result;
		}

		public static void RoleEmblemClick(byte CheckBtn = 0)
		{
			BufferData emblemBuffData = Global.GetEmblemBuffData(Global.Data.roleData.RoleID);
			if (emblemBuffData != null)
			{
				if (CheckBtn == 0)
				{
					Global.ChangeEmblemCoolDownData(emblemBuffData.StartTime, (long)emblemBuffData.BufferSecs);
				}
			}
			else if (CheckBtn == 0)
			{
				Global.ChangeEmblemCoolDownData(Global.GetFangZhiJiaSuTime(), 0L);
			}
		}

		public static bool CheckRoleEmblemCDIsReset()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("EmblemFull", ',');
			if (systemParamIntArrayByName != null && 0 < systemParamIntArrayByName.Length)
			{
				if (systemParamIntArrayByName[0] == 0)
				{
					return false;
				}
				for (int i = 1; i < systemParamIntArrayByName.Length; i++)
				{
					if (systemParamIntArrayByName[i] == Global.Data.roleData.MapCode)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static void AddSkillCoolDown(int magicCode, bool isDrawTicks = true, long StartTicks = -1L)
		{
			SkillData skillData = Global.GetSkillDataByID(magicCode);
			if (skillData == null)
			{
				return;
			}
			MagicInfoVO skillXmlNode = Global.GetSkillXmlNode(skillData.SkillID);
			if (skillXmlNode == null)
			{
				return;
			}
			int cdtime = skillXmlNode.CDTime;
			long correctLocalTime = Global.GetCorrectLocalTime();
			long num = (long)(cdtime * 1000);
			if (Global.Data.CoolDownSkillID == skillData.SkillID)
			{
				num = (long)((double)num * Global.Data.CoolDownSkillPercent);
			}
			if (cdtime > 0)
			{
				Global.SkillCoolDownDict[magicCode] = new CoolDownItem
				{
					ID = magicCode,
					StartTicks = ((StartTicks != -1L) ? StartTicks : correctLocalTime),
					CDTicks = num
				};
				Global.AddCoolDownItem(Global.SkillCoolDownDict, magicCode, (StartTicks != -1L) ? StartTicks : correctLocalTime, num);
				if (Global.NotifyAddCoolDown != null)
				{
					Global.NotifyAddCoolDown.Invoke(null, new CoolDownEventArgs
					{
						SkillID = magicCode,
						ToDrawTicks = ((StartTicks == -1L) ? num : Global.GetSkillCoolDownTicks(magicCode))
					});
				}
			}
			Global.Data.CoolDownSkillPercent = 1.0;
			int pubCDTime = skillXmlNode.PubCDTime;
			if (pubCDTime > 0)
			{
				for (int i = 0; i < Global.Data.roleData.SkillDataList.Count; i++)
				{
					skillData = Global.Data.roleData.SkillDataList[i];
					if (skillData != null && skillData.SkillID != magicCode)
					{
						if (pubCDTime > 0 && !Global.SkillCoolDownDict.ContainsKey(skillData.SkillID))
						{
							Global.SkillCoolDownDict[skillData.SkillID] = new CoolDownItem
							{
								ID = skillData.SkillID,
								StartTicks = correctLocalTime,
								CDTicks = (long)pubCDTime
							};
							Global.AddCoolDownItem(Global.SkillCoolDownDict, skillData.SkillID, correctLocalTime, (long)pubCDTime);
							if (Global.NotifyAddCoolDown != null)
							{
								Global.NotifyAddCoolDown.Invoke(null, new CoolDownEventArgs
								{
									SkillID = skillData.SkillID,
									ToDrawTicks = (long)pubCDTime
								});
							}
						}
					}
				}
			}
		}

		public static void AddSkillCoolDown(int magicCode, int secs)
		{
			if (Global.GetSkillDataByID(magicCode) == null)
			{
				return;
			}
			if (secs <= 0)
			{
				return;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			Global.SkillCoolDownDict[magicCode] = new CoolDownItem
			{
				ID = magicCode,
				StartTicks = correctLocalTime,
				CDTicks = (long)(secs * 1000)
			};
			Global.AddCoolDownItem(Global.SkillCoolDownDict, magicCode, correctLocalTime, (long)(secs * 1000));
		}

		public static bool GoodsCoolDown(int goodsID)
		{
			CoolDownItem coolDownItem = null;
			if (!Global.GoodsCoolDownDict.TryGetValue(goodsID, ref coolDownItem))
			{
				return false;
			}
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			return fangZhiJiaSuTime <= coolDownItem.StartTicks + coolDownItem.CDTicks;
		}

		public static long GetGoodsCoolDownTicks(int goodsID)
		{
			CoolDownItem coolDownItem = null;
			if (!Global.GoodsCoolDownDict.TryGetValue(goodsID, ref coolDownItem))
			{
				return 0L;
			}
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			if (fangZhiJiaSuTime > coolDownItem.StartTicks + coolDownItem.CDTicks)
			{
				return 0L;
			}
			return coolDownItem.StartTicks + coolDownItem.CDTicks - fangZhiJiaSuTime;
		}

		public static void AddGoodsCoolDown(int goodsID)
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			int cdtime = goodsXmlNodeByID.CDTime;
			if (cdtime <= 0)
			{
				return;
			}
			int pubCDTime = goodsXmlNodeByID.PubCDTime;
			int shareGroupID = goodsXmlNodeByID.ShareGroupID;
			long fangZhiJiaSuTime = Global.GetFangZhiJiaSuTime();
			Global.GoodsCoolDownDict[goodsID] = new CoolDownItem
			{
				ID = goodsID,
				StartTicks = fangZhiJiaSuTime,
				CDTicks = (long)(cdtime * 1000)
			};
			Global.AddCoolDownItem(Global.GoodsCoolDownDict, goodsID, fangZhiJiaSuTime, (long)(cdtime * 1000));
			if (shareGroupID > 0)
			{
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData != null)
					{
						if (goodsData.Using <= 0)
						{
							goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
							if (goodsXmlNodeByID != null)
							{
								if (shareGroupID == goodsXmlNodeByID.ShareGroupID)
								{
									Global.AddCoolDownItem(Global.GoodsCoolDownDict, goodsData.GoodsID, fangZhiJiaSuTime, (long)(pubCDTime * 1000));
								}
							}
						}
					}
				}
			}
			if (Global.NotifyGoodsAddCoolDown != null)
			{
				Global.NotifyGoodsAddCoolDown.Invoke(null, EventArgs.Empty);
			}
		}

		private static bool FintRoadItems(int currentMapCode, int toMapCode, Point pos, List<RoadItem> roadItemsList, Dictionary<int, bool> mapDict)
		{
			MapTeleports mapTeleports = null;
			if (!Global.Data.MapTeleportsDict.TryGetValue(currentMapCode, ref mapTeleports))
			{
				return false;
			}
			for (int i = 0; i < mapTeleports.TeleportsList.Count; i++)
			{
				if (toMapCode == mapTeleports.TeleportsList[i].ToMapID)
				{
					RoadItem roadItem = new RoadItem
					{
						MapID = currentMapCode,
						ToPos = mapTeleports.TeleportsList[i].TeleportPos,
						TeleportKey = mapTeleports.TeleportsList[i].TeleportKey
					};
					roadItemsList.Add(roadItem);
					roadItem = new RoadItem
					{
						MapID = toMapCode,
						ToPos = pos,
						TeleportKey = -1
					};
					roadItemsList.Add(roadItem);
					return true;
				}
			}
			for (int j = 0; j < mapTeleports.TeleportsList.Count; j++)
			{
				if (Global.Data.roleData.MapCode != mapTeleports.TeleportsList[j].ToMapID)
				{
					if (!mapDict.ContainsKey(mapTeleports.TeleportsList[j].ToMapID))
					{
						mapDict[mapTeleports.TeleportsList[j].ToMapID] = true;
						RoadItem roadItem2 = new RoadItem
						{
							MapID = currentMapCode,
							ToPos = mapTeleports.TeleportsList[j].TeleportPos,
							TeleportKey = mapTeleports.TeleportsList[j].TeleportKey
						};
						roadItemsList.Add(roadItem2);
						if (Global.FintRoadItems(mapTeleports.TeleportsList[j].ToMapID, toMapCode, pos, roadItemsList, mapDict))
						{
							return true;
						}
						roadItemsList.RemoveAt(roadItemsList.Count - 1);
					}
				}
			}
			return false;
		}

		public static List<RoadItem> FindAutoRoadItems(int toMapCode, Point pos)
		{
			if (Global.Data.MapTeleportsDict == null)
			{
				return null;
			}
			List<RoadItem> list = new List<RoadItem>();
			if (toMapCode == Global.Data.roleData.MapCode)
			{
				RoadItem roadItem = new RoadItem
				{
					MapID = toMapCode,
					ToPos = pos
				};
				list.Add(roadItem);
				return list;
			}
			Dictionary<int, bool> mapDict = new Dictionary<int, bool>();
			Stack<RoadItem> stack = new Stack<RoadItem>();
			Global.FintRoadItems(Global.Data.roleData.MapCode, toMapCode, pos, list, mapDict);
			return list;
		}

		public static bool CanBeTransport(int mapCode, int teleportKey)
		{
			if (!Global.Data.PlayGame)
			{
				return false;
			}
			if (Global.IsAutoFighting())
			{
				return false;
			}
			if (Global.Data.AutoRoadItemsList == null)
			{
				return true;
			}
			if (Global.Data.AutoRoadItemsList.Count <= 0)
			{
				return true;
			}
			for (int i = 0; i < Global.Data.AutoRoadItemsList.Count; i++)
			{
				if (mapCode == Global.Data.AutoRoadItemsList[i].MapID)
				{
					if (Global.Data.AutoRoadItemsList[i].TeleportKey == teleportKey)
					{
						return true;
					}
				}
			}
			return Global.Data.AutoRoadItemsList[Global.Data.AutoRoadItemsList.Count - 1].MapID == mapCode;
		}

		public static int MaxForgeLevel
		{
			get
			{
				string systemParamByName = ConfigSystemParam.GetSystemParamByName(Global.name, true);
				if (string.IsNullOrEmpty(systemParamByName))
				{
					return 15;
				}
				if (systemParamByName.Split(new char[]
				{
					','
				}).Length == 1)
				{
					return (!systemParamByName.Equals("1")) ? 15 : 20;
				}
				string[] array = Enumerable.ToArray<string>(systemParamByName.Split(new char[]
				{
					','
				}));
				return (!array[0].Equals("2")) ? 15 : 20;
			}
		}

		public static int GetForgeLuckyPercentByGoodsID(int goodsID)
		{
			if (!Global.IsLuckyGoodsID(goodsID))
			{
				return 0;
			}
			int num = (goodsID % 100 - 20) * 10;
			if (num == 0)
			{
				return 5;
			}
			return num;
		}

		public static bool IsLuckyGoodsID(int goodId)
		{
			if (goodId != 0)
			{
				if (Global.LuckyGoodsIDs == null)
				{
					Global.LuckyGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeLuckyGoodsIDs", ',');
				}
				if (Array.IndexOf<int>(Global.LuckyGoodsIDs, goodId) != -1)
				{
					return true;
				}
			}
			return false;
		}

		private static int GetForgeLuckyPercent(int luckyNum)
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("ForgeLuckyGoodsID");
			if (num < 0)
			{
				return 0;
			}
			luckyNum = Global.GMin(Global.GetTotalGoodsCountByID(num), luckyNum);
			if (luckyNum <= 0)
			{
				return 0;
			}
			int num2 = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeLuckyGoodsRate", ',')[0];
			return luckyNum * num2;
		}

		public static int GetForgePercent(GoodsData goodsData, int luckyID = 0)
		{
			if (goodsData.Forge_level >= Global.MaxForgeLevel)
			{
				return 0;
			}
			int num = 0;
			if (luckyID != 0)
			{
				num = Global.GetForgeLuckyPercentByGoodsID(luckyID);
			}
			if (Global.ForgeLevelRocksPercent == null)
			{
				Global.ForgeLevelRocksPercent = ConfigSystemParam.GetSystemParamDoubleArrayByName("ForgeGoodsRate");
			}
			int num2 = (int)Math.Abs(Global.ForgeLevelRocksPercent[goodsData.Forge_level + 1]);
			if (Global.ProcessMonthVIP() > 0.0)
			{
				num2 += 5;
			}
			num2 += num;
			num2 = Math.Max(5, num2);
			return Math.Min(100, num2);
		}

		public static int GetForgeNextLevelYinLiang(GoodsData goodsData)
		{
			if (Global.ForgeLevelNeedYinLiang == null)
			{
				Global.ForgeLevelNeedYinLiang = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeLevelNeedYinLiang", ',');
			}
			return Math.Abs(Global.ForgeLevelNeedYinLiang[goodsData.Forge_level + 1]);
		}

		public static int GetForgeNextLevelRock(GoodsData goodsData)
		{
			return 1;
		}

		public static string[] GetSubForgeNextLevelParams(GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return null;
			}
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("SubForgeGoodsIDs", true);
			if (systemParamByName == null)
			{
				return null;
			}
			int num = goodsData.AddPropIndex + 1;
			string[] array = systemParamByName.Split(new char[]
			{
				','
			});
			if (num >= array.Length)
			{
				return null;
			}
			string[] array2 = array[num].Split(new char[]
			{
				'|'
			});
			if (array2.Length != 3)
			{
				return null;
			}
			return array2;
		}

		public static int[] GetForgeNeedGoodsList(int forgeLevel)
		{
			if (Global.ForgeGoodsListDict.ContainsKey(forgeLevel))
			{
				return Global.ForgeGoodsListDict[forgeLevel];
			}
			if (Global.ForgeGoodsList == null)
			{
				string systemParamByName = ConfigSystemParam.GetSystemParamByName("ForgeNeedGoodsIDs", true);
				if (systemParamByName == null)
				{
					return null;
				}
				string[] array = systemParamByName.Split(new char[]
				{
					'|'
				});
				if (forgeLevel >= array.Length)
				{
					return null;
				}
				Global.ForgeGoodsList = array;
			}
			Global.ForgeGoodsListDict[forgeLevel] = ConvertExt.String2IntArray(Global.ForgeGoodsList[forgeLevel], ',');
			return Global.ForgeGoodsListDict[forgeLevel];
		}

		public static int[] GetForgeNeedGoodsNumList(int forgeLevel)
		{
			if (Global.ForgeGoodsNumListDict.ContainsKey(forgeLevel))
			{
				return Global.ForgeGoodsNumListDict[forgeLevel];
			}
			if (Global.ForgeGoodsNumList == null)
			{
				string systemParamByName = ConfigSystemParam.GetSystemParamByName("ForgeNeedGoodsNum", true);
				if (systemParamByName == null)
				{
					return null;
				}
				string[] array = systemParamByName.Split(new char[]
				{
					'|'
				});
				if (forgeLevel >= array.Length)
				{
					return null;
				}
				Global.ForgeGoodsNumList = array;
			}
			Global.ForgeGoodsNumListDict[forgeLevel] = ConvertExt.String2IntArray(Global.ForgeGoodsNumList[forgeLevel], ',');
			return Global.ForgeGoodsNumListDict[forgeLevel];
		}

		public static void GetBaoHuGoodsIDs(int forgeLevel, out int goodsID, out int goodsNum)
		{
			goodsID = -1;
			goodsNum = 0;
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ForgeProtectStoneGoodsIDS", '|');
			if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length <= 0)
			{
				return;
			}
			string[] array = systemParamStringArrayByName[forgeLevel].Split(new char[]
			{
				','
			});
			if (array.Length != 2)
			{
				return;
			}
			goodsID = Convert.ToInt32(array[0]);
			goodsNum = Convert.ToInt32(array[1]);
		}

		public static int GetForgeRockGoodsID(GoodsData goodsData)
		{
			int num = (goodsData == null) ? 1 : (goodsData.Forge_level + 1);
			if (Global.ForgeGoodsIDs == null)
			{
				Global.ForgeGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeGoodsIDs", ',');
			}
			return Math.Abs(Global.ForgeGoodsIDs[num]);
		}

		public static bool IsForgeRockGoodsID(int goodId)
		{
			if (goodId != 0)
			{
				if (Global.ForgeGoodsIDs == null)
				{
					Global.ForgeGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ForgeGoodsIDs", ',');
				}
				if (Array.IndexOf<int>(Global.ForgeGoodsIDs, goodId) != -1)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetForgeNextLevelShenYou(GoodsData goodsData)
		{
			return 1;
		}

		public static string GetForgeRockLevelNames(int forgeRockGoodsID)
		{
			return Global.GetLang(Global.ForgeRockNames[forgeRockGoodsID % 100]);
		}

		public static int GetChuanchengMoney(int equipLevel, int moneyIndex)
		{
			if (moneyIndex == 0)
			{
				if (Global.ChuanchengMoney == null)
				{
					Global.ChuanchengMoney = ConfigSystemParam.GetSystemParamIntArrayByName("ChuanChengXiaoHaoJinBi", ',');
				}
				return Global.ChuanchengMoney[equipLevel];
			}
			if (moneyIndex == 1)
			{
				if (Global.ChuanchengYuanbao == null)
				{
					Global.ChuanchengYuanbao = ConfigSystemParam.GetSystemParamIntArrayByName("ChuanChengXiaoHaoZhuanShi", ',');
				}
				return Global.ChuanchengYuanbao[equipLevel];
			}
			return 0;
		}

		public static int GetChuanchengDiaojilv(int equipLevel)
		{
			if (Global.ChuanchengDiaojilv == null)
			{
				Global.ChuanchengDiaojilv = ConfigSystemParam.GetSystemParamDoubleArrayByName("ChuanChengGoodsRate");
			}
			return (int)Math.Abs(Global.ChuanchengDiaojilv[equipLevel]);
		}

		public static int GetZhuijiaChuanchengDiaojilv(int equipLevel)
		{
			if (Global.ZhuijiaChuanchengDiaojilv == null)
			{
				Global.ZhuijiaChuanchengDiaojilv = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuiJiaChuanChengGoodsRate");
			}
			return (int)Math.Abs(Global.ZhuijiaChuanchengDiaojilv[equipLevel]);
		}

		public static int GetJuHunChuanchengDiaojilv(int equipLevel)
		{
			if (Global.JuHunChuanchengDiaojilv == null)
			{
				Global.JuHunChuanchengDiaojilv = ConfigSystemParam.GetSystemParamDoubleArrayByName("JuHunChuanChengGoodsRate");
			}
			return (int)Math.Abs(Global.JuHunChuanchengDiaojilv[equipLevel]);
		}

		public static int GetZhuijiaChuanchengMoney(int equipLevel, int moneyIndex)
		{
			if (moneyIndex == 0)
			{
				if (Global.ZhuijiaChuanchengMoney == null)
				{
					Global.ZhuijiaChuanchengMoney = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuiJiaChuanChengXiaoHaoJinBi", ',');
				}
				return Global.ZhuijiaChuanchengMoney[equipLevel];
			}
			if (moneyIndex == 1)
			{
				if (Global.ZhuijiaChuanchengYuanbao == null)
				{
					Global.ZhuijiaChuanchengYuanbao = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuiJiaChuanChengXiaoHaoZhuanShi", ',');
				}
				return Global.ZhuijiaChuanchengYuanbao[equipLevel];
			}
			return 0;
		}

		public static int GetXilianChuanchengDiaojilv(int washCount)
		{
			if (Global.XilianChuanchengDiaojilv == null)
			{
				Global.XilianChuanchengDiaojilv = ConfigSystemParam.GetSystemParamDoubleArrayByName("XiLianChuanChengGoodsRate");
			}
			return (int)Math.Abs(Global.XilianChuanchengDiaojilv[0]);
		}

		public static int GetXilianChuanchengMoney(int equipLevel, int moneyIndex)
		{
			if (equipLevel == 0)
			{
				return 0;
			}
			if (moneyIndex == 0)
			{
				if (Global.XilianChuanchengMoney == null)
				{
					Global.XilianChuanchengMoney = ConfigSystemParam.GetSystemParamIntArrayByName("XiLianChuanChengXiaoHaoJinBi", ',');
				}
				return Global.XilianChuanchengMoney[0];
			}
			if (moneyIndex == 1)
			{
				if (Global.XilianChuanchengYuanbao == null)
				{
					Global.XilianChuanchengYuanbao = ConfigSystemParam.GetSystemParamIntArrayByName("XiLianChuanChengXiaoHaoZhuanShi", ',');
				}
				return Global.XilianChuanchengYuanbao[0];
			}
			return 0;
		}

		public static int GetJuHunChuanChengMoney(int level, int moneyIndex)
		{
			if (level == 0)
			{
				return 0;
			}
			if (moneyIndex == 0)
			{
				if (Global.juHunChuanchengMoney == null)
				{
					Global.juHunChuanchengMoney = ConfigSystemParam.GetSystemParamIntArrayByName("JuHunChuanChengXiaoHaoJinBi", ',');
				}
				return Global.juHunChuanchengMoney[level];
			}
			if (moneyIndex == 1)
			{
				if (Global.juHunChuanchengYuanbao == null)
				{
					Global.juHunChuanchengYuanbao = ConfigSystemParam.GetSystemParamIntArrayByName("JuHunChuanChengXiaoHaoZhuanShi", ',');
				}
				return Global.juHunChuanchengYuanbao[level];
			}
			return 0;
		}

		public static int GetCategoriyByGoodsID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.Categoriy;
		}

		public static bool CheckShengyoufuIsHefa(int goodsID, int equipLevel)
		{
			if (Global.ShengyoufuGoodsIDs == null)
			{
				Global.ShengyoufuGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ShenyoufuGoodsIDs", ',');
			}
			if (Global.ShengyoufuQianghuaDengji == null)
			{
				Global.ShengyoufuQianghuaDengji = ConfigSystemParam.GetSystemParamIntArrayByName("ShengyoufuQianghuaDengji", ',');
			}
			int num = -1;
			for (int i = 0; i < Global.ShengyoufuQianghuaDengji.Length; i++)
			{
				if (Global.ShengyoufuQianghuaDengji[i] >= equipLevel)
				{
					num = i;
					break;
				}
			}
			return Array.IndexOf<int>(Global.ShengyoufuGoodsIDs, goodsID, num) != -1;
		}

		public static bool IsShengyoufuGoodsID(int goodId)
		{
			if (goodId != 0)
			{
				if (Global.ShengyoufuGoodsIDs == null)
				{
					Global.ShengyoufuGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ShenyoufuGoodsIDs", ',');
				}
				if (Array.IndexOf<int>(Global.ShengyoufuGoodsIDs, goodId) != -1)
				{
					return true;
				}
			}
			return false;
		}

		public static string GetEnchanceText(GoodsQuality gq)
		{
			string result = string.Empty;
			if (gq == GoodsQuality.White)
			{
				result = Global.GetLang("白色");
			}
			else if (gq == GoodsQuality.Green)
			{
				result = Global.GetLang("绿色");
			}
			else if (gq == GoodsQuality.Blue)
			{
				result = Global.GetLang("蓝色");
			}
			else if (gq == GoodsQuality.Purple)
			{
				result = Global.GetLang("紫色");
			}
			else if (gq == GoodsQuality.Gold)
			{
				result = Global.GetLang("金色");
			}
			else if (gq == GoodsQuality.Orange)
			{
				result = Global.GetLang("橙色");
			}
			else if (gq == GoodsQuality.FlashPurple)
			{
				result = Global.GetLang("紫闪");
			}
			return result;
		}

		public static int GetEnchanceQualityByColorName(string colorName)
		{
			if (colorName == Global.GetLang("白色"))
			{
				return 0;
			}
			if (colorName == Global.GetLang("绿色"))
			{
				return 1;
			}
			if (colorName == Global.GetLang("蓝色"))
			{
				return 2;
			}
			if (colorName == Global.GetLang("紫色"))
			{
				return 3;
			}
			if (colorName == Global.GetLang("金色"))
			{
				return 5;
			}
			if (colorName == Global.GetLang("橙色"))
			{
				return 6;
			}
			return 0;
		}

		public static uint GetEnchanceColor(int gq)
		{
			uint result = 0U;
			if (gq == 1)
			{
				result = 65280U;
			}
			else if (gq == 2)
			{
				result = 2067905U;
			}
			else if (gq == 3)
			{
				result = 16711935U;
			}
			else if (gq == 5)
			{
				result = 16776960U;
			}
			else if (gq == 6)
			{
				result = 16736512U;
			}
			return result;
		}

		public static string GetEnchanceStrColor(GoodsQuality gq)
		{
			string result = "FFFFFFFF";
			if (gq == GoodsQuality.Green)
			{
				result = "FF00FF00";
			}
			else if (gq == GoodsQuality.Blue)
			{
				result = "FF1F8DC1";
			}
			else if (gq == GoodsQuality.Purple)
			{
				result = "FFFF00FF";
			}
			else if (gq == GoodsQuality.Gold)
			{
				result = "FFFFD700";
			}
			else if (gq == GoodsQuality.Orange)
			{
				result = "FFFF6100";
			}
			return result;
		}

		private static int GetEnchanceLuckyPercent(int luckyNum)
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("EnchanceLuckyGoodsID");
			if (num < 0)
			{
				return 0;
			}
			luckyNum = Global.GMin(Global.GetTotalGoodsCountByID(num), luckyNum);
			if (luckyNum <= 0)
			{
				return 0;
			}
			int num2 = (int)ConfigSystemParam.GetSystemParamIntByName("EnchanceLuckyGoodsRate");
			return luckyNum * num2;
		}

		public static int GetEnchanceNextLevelRock(GoodsData goodsData)
		{
			return 1;
		}

		public static int GetQualityIDByGoodsID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.QualityID;
		}

		public static XElement GetQualityUpXmlNode(int categoriy, int suitID, int qualityID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/QualityUp.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Equip", "Categoriy", categoriy.ToString());
			if (xelement == null)
			{
				return null;
			}
			xelement = Global.GetXElement(xelement, "Item", "ShouShiSuitID", suitID.ToString(), "QualityID", qualityID.ToString());
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static XElement GetEquipUpXmlNode(int categoriy, int suitID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/MuEquipUp.xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Equip", "Categoriy", categoriy.ToString());
			if (xelement == null)
			{
				return null;
			}
			xelement = Global.GetXElement(xelement, "Item", "SuitID", suitID.ToString());
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static int GetZhuiJiaGoodsID(GoodsData goodsData)
		{
			int num = (goodsData == null) ? 1 : (goodsData.AppendPropLev + 1);
			num = Math.Min(num, 80);
			if (Global.ZhuiJiaGoodsIDs == null)
			{
				Global.ZhuiJiaGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuiJiaForgeGoodsIDs", ',');
			}
			return Math.Abs(Global.ZhuiJiaGoodsIDs[num]);
		}

		public static int GetZhuiJiaGoodsIDNums(GoodsData goodsData)
		{
			int num = (goodsData == null) ? 1 : (goodsData.AppendPropLev + 1);
			num = Math.Min(num, 80);
			if (Global.ZhuiJiaGoodsIDNums == null)
			{
				Global.ZhuiJiaGoodsIDNums = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuiJiaForgeGoodsNum", ',');
			}
			return Math.Abs(Global.ZhuiJiaGoodsIDNums[num]);
		}

		public static int GetZhuiJiaForgeLuckyGoodsIDs()
		{
			if (Global.ZhuiJiaForgeLuckyGoodsIDs == 0)
			{
				Global.ZhuiJiaForgeLuckyGoodsIDs = (int)ConfigSystemParam.GetSystemParamIntByName("ZhuiJiaForgeLuckyGoodsIDs");
			}
			return Global.ZhuiJiaForgeLuckyGoodsIDs;
		}

		public static int GetZhuijiaForgeLevelNeedMoney(GoodsData goodsData)
		{
			if (Global.ZhuijiaForgeLevelNeedMoney == null)
			{
				Global.ZhuijiaForgeLevelNeedMoney = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuiJiaXiaoHaoJinBi", ',');
			}
			int num = Math.Min(goodsData.AppendPropLev + 1, 80);
			return Math.Abs(Global.ZhuijiaForgeLevelNeedMoney[num]);
		}

		public static int GetZhuijiaChenggonglvs(GoodsData goodsData)
		{
			if (Global.ZhuijiaChenggonglvs == null)
			{
				Global.ZhuijiaChenggonglvs = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuiJiaGoodsRate");
			}
			int num = Math.Min(goodsData.AppendPropLev + 1, 80);
			return (int)Math.Abs(Global.ZhuijiaChenggonglvs[num]);
		}

		public static int GetZhuanshengGoodsID(GoodsData goodsData)
		{
			int num = (goodsData == null) ? 1 : (goodsData.ChangeLifeLevForEquip + 1);
			num = Math.Min(num, 10);
			if (Global.ZhuanshengGoodsIDs == null)
			{
				Global.ZhuanshengGoodsIDs = ConfigSystemParam.GetSystemParamIntArrayByName("EquipZhuanShengNeedGoods", ',');
			}
			return Math.Abs(Global.ZhuanshengGoodsIDs[num]);
		}

		public static double GetZhuanshengExpRate()
		{
			if (Global.ZhuanshengExpRates == null)
			{
				Global.ZhuanshengExpRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuanShengExpXiShu");
			}
			if (Global.ZhuanshengExpRates != null && Global.Data.roleData.ChangeLifeCount >= 0 && Global.Data.roleData.ChangeLifeCount <= Global.ZhuanshengExpRates.Length)
			{
				return Global.ZhuanshengExpRates[Mathf.Clamp(Global.Data.roleData.ChangeLifeCount, 0, Global.ZhuanshengExpRates.Length)];
			}
			return 1.0;
		}

		public static long GetExpMultiByZhuanShengExpXiShu(long exp)
		{
			return (long)((double)exp * Global.GetZhuanshengExpRate());
		}

		public static int GetZhuanshengLevelNeedMoney(GoodsData goodsData)
		{
			if (Global.ZhuanshengLevelNeedMoney == null)
			{
				Global.ZhuanshengLevelNeedMoney = ConfigSystemParam.GetSystemParamIntArrayByName("EquipZhuanShengNeedMoJing", ',');
			}
			int num = Math.Min(goodsData.ChangeLifeLevForEquip + 1, 10);
			return Math.Abs(Global.ZhuanshengLevelNeedMoney[num]);
		}

		public static int GetZhuanshengChenggonglvs(GoodsData goodsData)
		{
			if (Global.ZhuanshengChenggonglvs == null)
			{
				Global.ZhuanshengChenggonglvs = ConfigSystemParam.GetSystemParamDoubleArrayByName("EquipZhuanShengRate");
			}
			int num = Math.Min(goodsData.ChangeLifeLevForEquip + 1, 10);
			return (int)Math.Abs(Global.ZhuanshengChenggonglvs[num]);
		}

		public static int GetZhuanshengBoliMoney(int equipLevel, int moneyIndex)
		{
			if (moneyIndex == 0)
			{
				if (Global.ZhuanshengBoliMoney == null)
				{
					Global.ZhuanshengBoliMoney = ConfigSystemParam.GetSystemParamIntArrayByName("EquipZhuanShengBoLiJinBi", ',');
				}
				return Global.ZhuanshengBoliMoney[equipLevel];
			}
			if (moneyIndex == 1)
			{
				if (Global.ZhuanshengBoliYuanbao == null)
				{
					Global.ZhuanshengBoliYuanbao = ConfigSystemParam.GetSystemParamIntArrayByName("EquipZhuanShengBoLiZhuanShi", ',');
				}
				return Global.ZhuanshengBoliYuanbao[equipLevel];
			}
			return 0;
		}

		public static int GetZhuanshengBoliChenggonglvs(int equipLevel)
		{
			if (Global.ZhuanshengBoliChenggonglvs == null)
			{
				Global.ZhuanshengBoliChenggonglvs = ConfigSystemParam.GetSystemParamDoubleArrayByName("EquipZhuanShengBoLiRates");
			}
			return (int)Math.Abs(Global.ZhuanshengBoliChenggonglvs[equipLevel]);
		}

		public static XElement GetNextEquipXML(int suitID, int type)
		{
			XElement gameResXml = Global.GetGameResXml("Config/EquipUpgrade.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Equip", "Categoriy", type.ToString());
			if (xelement == null)
			{
				return null;
			}
			xelement = Global.GetXElement(xelement, "Item", "SuitID", (suitID + 1).ToString());
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static int GetEquipGoodsSuitID(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.SuitID;
		}

		public static int GetEquipGoodsPropsByJinJie(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			return goodsXmlNodeByID.JinJie;
		}

		private static int GetJinjieLuckyPercent(int luckyNum)
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieLuckyGoodsID");
			if (num < 0)
			{
				return 0;
			}
			luckyNum = Global.GMin(Global.GetTotalGoodsCountByID(num), luckyNum);
			if (luckyNum <= 0)
			{
				return 0;
			}
			int num2 = (int)ConfigSystemParam.GetSystemParamIntByName("JinjieLuckyGoodsRate");
			return luckyNum * num2;
		}

		public static XElement GetEquipUpgradeItemByGoodsID(int goodsID, int maxSuiItID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy < 0 || categoriy >= 25)
			{
				return null;
			}
			int num = goodsXmlNodeByID.SuitID;
			if (num < 1 || num > maxSuiItID)
			{
				num = 1;
			}
			XElement gameResXml = Global.GetGameResXml("Config/EquipUpgrade.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Equip", "Categoriy", categoriy.ToString());
			if (xelement == null)
			{
				return null;
			}
			return Global.GetXElement(xelement, "Item", "SuitID", num.ToString());
		}

		public static int GetJinjieNextPercent(int goodsID, int luckyNum)
		{
			int num = 0;
			XElement equipUpgradeItemByGoodsID = Global.GetEquipUpgradeItemByGoodsID(goodsID, Global.MaxSuitID);
			if (equipUpgradeItemByGoodsID == null)
			{
				return num;
			}
			num = Global.GetXElementAttributeInt(equipUpgradeItemByGoodsID, "Succeed");
			num += Global.GetJinjieLuckyPercent(luckyNum);
			return Math.Min(num, 100);
		}

		public static int GetJinjieNextLevelYinLiang(int goodsID)
		{
			int num = 100000000;
			XElement equipUpgradeItemByGoodsID = Global.GetEquipUpgradeItemByGoodsID(goodsID, Global.MaxSuitID);
			if (equipUpgradeItemByGoodsID == null)
			{
				return num;
			}
			num = Global.GetXElementAttributeInt(equipUpgradeItemByGoodsID, "YinLiang");
			return Math.Max(num, 0);
		}

		public static int GetJinjieNextRocks(int goodsID)
		{
			int num = 100000000;
			XElement equipUpgradeItemByGoodsID = Global.GetEquipUpgradeItemByGoodsID(goodsID, Global.MaxSuitID);
			if (equipUpgradeItemByGoodsID == null)
			{
				return num;
			}
			num = Global.GetXElementAttributeInt(equipUpgradeItemByGoodsID, "GoodsNum");
			return Math.Max(num, 0);
		}

		public static int GetJinjieNextRocksGoodsID(int goodsID)
		{
			int num = 100000000;
			XElement equipUpgradeItemByGoodsID = Global.GetEquipUpgradeItemByGoodsID(goodsID, Global.MaxSuitID);
			if (equipUpgradeItemByGoodsID == null)
			{
				return num;
			}
			num = Global.GetXElementAttributeInt(equipUpgradeItemByGoodsID, "NeedGoodsID");
			return Math.Max(num, 0);
		}

		public static int GetQianghuashiFenliMoney(int equipLevel)
		{
			if (Global.QianghuashiFenliMoney == null)
			{
				Global.QianghuashiFenliMoney = ConfigSystemParam.GetSystemParamIntArrayByName("QianghuashiFenliMoney", ',');
			}
			return Global.QianghuashiFenliMoney[equipLevel - 1];
		}

		public static bool CanEnchaseJewel(int jewelGoodsID)
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("XiangqianRockStartID");
			int num2 = (int)ConfigSystemParam.GetSystemParamIntByName("XiangqianRockEndID");
			return jewelGoodsID >= num && jewelGoodsID <= num2;
		}

		public static int GetJewelLevel(int jewelGoodsID)
		{
			return jewelGoodsID % 1000;
		}

		public static bool CanAddJewelIntoEquip(int equipGoodsID, int jewelGoodsID)
		{
			return false;
		}

		public static string GetEquipEchaseJewelHint(int equipGoodsID)
		{
			return Global.GetLang("装备无法镶嵌宝石...");
		}

		public static XElement GetXilianXmlNode(string id)
		{
			XElement gameResXml = Global.GetGameResXml("Config/XiLianShuXing.xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "XiLian", "ID", id);
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static Dictionary<int, int> GetXilianPropsUpLimitDict(GoodsData gd)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			XElement xilianXmlNode = Global.GetXilianXmlNode(goodsXmlNodeByID.XiLian.ToString());
			if (xilianXmlNode == null)
			{
				return null;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < Global.washPropsNames.Length; i++)
			{
				string text = Global.washPropsNames[i].ToLower();
				if (text.Substring(0, 3) == "min")
				{
					text = text.Substring(3);
				}
				int num = ExtPropIndexes.ExtPropIndexNames.IndexOf(text);
				int xelementAttributeInt = Global.GetXElementAttributeInt(xilianXmlNode, Global.washPropsNames[i]);
				if (!dictionary.ContainsKey(num))
				{
					dictionary.Add(num, xelementAttributeInt);
				}
			}
			return dictionary;
		}

		public static float GetXilianPropsUpFactor(GoodsData gd)
		{
			float result = 0f;
			if (Global.XilianPropsUpFactorDict == null)
			{
				XElement gameResXml = Global.GetGameResXml("Config/XiLianType.xml");
				if (gameResXml == null)
				{
					return result;
				}
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "XiLian");
				Global.XilianPropsUpFactorDict = new Dictionary<int, float>();
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					float xelementAttributeFloat = Global.GetXElementAttributeFloat(xelement, "Multiplying");
					if (!Global.XilianPropsUpFactorDict.ContainsKey(xelementAttributeInt))
					{
						Global.XilianPropsUpFactorDict.Add(xelementAttributeInt, xelementAttributeFloat);
					}
				}
			}
			int goodsColorByGoodsData = (int)Global.GetGoodsColorByGoodsData(gd);
			if (Global.XilianPropsUpFactorDict.TryGetValue(goodsColorByGoodsData, ref result))
			{
				return result;
			}
			return result;
		}

		public static bool IsToXilianPropsUpLimit(GoodsData gd)
		{
			if (gd == null)
			{
				return false;
			}
			Dictionary<int, int> xilianPropsUpLimitDict = Global.GetXilianPropsUpLimitDict(gd);
			float xilianPropsUpFactor = Global.GetXilianPropsUpFactor(gd);
			if (xilianPropsUpLimitDict == null)
			{
				return false;
			}
			if (gd.WashProps != null)
			{
				int num = 0;
				for (int i = 0; i < gd.WashProps.Count; i += 2)
				{
					int num2 = gd.WashProps[i];
					int num3 = gd.WashProps[i + 1];
					if (xilianPropsUpLimitDict.TryGetValue(num2, ref num))
					{
						num = (int)((float)num * xilianPropsUpFactor);
						if (num3 < num)
						{
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}

		public static int GetSumOfAllEquipXilianValue()
		{
			int num = 0;
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			if (usingGoodsDataList == null || usingGoodsDataList.Count <= 0)
			{
				return num;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				if (value.WashProps != null)
				{
					for (int i = 0; i < value.WashProps.Count; i += 2)
					{
						num += value.WashProps[i + 1];
					}
				}
			}
			return num;
		}

		public static int GetSumOfAllEquipZhuijiaValue()
		{
			int num = 0;
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			if (usingGoodsDataList == null || usingGoodsDataList.Count <= 0)
			{
				return num;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (categoriy != 9 && categoriy != 10 && categoriy != 7)
					{
						num += value.AppendPropLev;
					}
				}
			}
			return num;
		}

		public static int GetSumOfAllEquipQianghuaValue()
		{
			int num = 0;
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			if (usingGoodsDataList == null || usingGoodsDataList.Count <= 0)
			{
				return num;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (categoriy != 9 && categoriy != 10 && categoriy != 7 && categoriy != 24)
					{
						num += value.Forge_level;
					}
				}
			}
			return num;
		}

		public static bool IsXilianProps1MoreThan2UpLimit(GoodsData gd1, GoodsData gd2)
		{
			if (gd1 == null)
			{
				return false;
			}
			if (gd1.WashProps == null)
			{
				return false;
			}
			Dictionary<int, int> xilianPropsUpLimitDict = Global.GetXilianPropsUpLimitDict(gd2);
			float xilianPropsUpFactor = Global.GetXilianPropsUpFactor(gd2);
			if (xilianPropsUpLimitDict == null)
			{
				return false;
			}
			for (int i = 0; i < gd1.WashProps.Count; i += 2)
			{
				int num = gd1.WashProps[i + 1];
				int num2 = 0;
				if (xilianPropsUpLimitDict.TryGetValue(gd1.WashProps[i], ref num2))
				{
					num2 = (int)((float)num2 * xilianPropsUpFactor);
					if (num > num2)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsContainDBIDByXilian(int dbID)
		{
			return Global.DBIdsDictByXilian != null && Global.DBIdsDictByXilian.ContainsKey(dbID);
		}

		public static void AddDBIdToDictByXilian(int dbID)
		{
			if (Global.DBIdsDictByXilian == null)
			{
				Global.DBIdsDictByXilian = new Dictionary<int, int>();
			}
			if (!Global.DBIdsDictByXilian.ContainsKey(dbID))
			{
				Global.DBIdsDictByXilian.Add(dbID, 0);
			}
		}

		public static void RemoveDBIdFromDictByXilian(int dbID)
		{
			if (Global.DBIdsDictByXilian == null)
			{
				return;
			}
			if (Global.DBIdsDictByXilian.ContainsKey(dbID))
			{
				Global.DBIdsDictByXilian.Remove(dbID);
			}
		}

		public static void ClearDictByXilian()
		{
			if (Global.DBIdsDictByXilian == null)
			{
				return;
			}
			Global.DBIdsDictByXilian.Clear();
		}

		public static bool IsShengqi(GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return false;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			if (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HorseZaiZaoSuitID", ',');
				return goodsXmlNodeByID.SuitID >= Mathf.Min(systemParamIntArrayByName) + 1;
			}
			return goodsXmlNodeByID.SuitID >= Global.ShenqiZaizaoSuit + 1 && ((goodsXmlNodeByID.Categoriy >= 0 && goodsXmlNodeByID.Categoriy <= 6) || (goodsXmlNodeByID.Categoriy >= 11 && goodsXmlNodeByID.Categoriy <= 21) || (goodsXmlNodeByID.Categoriy >= 40 && goodsXmlNodeByID.Categoriy <= 45));
		}

		public static XElement GetEquipZaizaoXmlNode(GoodsData goodsData)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ZaiZao.xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "ZaiSheng", "NeedEquitID", goodsData.GoodsID.ToString());
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static ShenqiZaizaoXmlData GetShenqiZaizaoXmlData(GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return null;
			}
			if (Global.ShenqiZaizaoXmlDataDict == null)
			{
				Global.InitShenqiZaizaoXmlDataDict();
			}
			ShenqiZaizaoXmlData result = null;
			Global.ShenqiZaizaoXmlDataDict.TryGetValue(goodsData.GoodsID, ref result);
			return result;
		}

		public static void InitShenqiZaizaoXmlDataDict()
		{
			if (Global.ShenqiZaizaoXmlDataDict != null)
			{
				return;
			}
			Global.ShenqiZaizaoXmlDataDict = new Dictionary<int, ShenqiZaizaoXmlData>();
			XElement gameResXml = Global.GetGameResXml("Config/ZaiZao.xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				ShenqiZaizaoXmlData shenqiZaizaoXmlData = new ShenqiZaizaoXmlData();
				shenqiZaizaoXmlData.NeedEquitID = Global.GetXElementAttributeInt(xelement, "NeedEquitID");
				shenqiZaizaoXmlData.NewEquitID = Global.GetXElementAttributeInt(xelement, "NewEquitID");
				shenqiZaizaoXmlData.NeedBandJinBi = Global.GetXElementAttributeInt(xelement, "NeedBandJinBi");
				shenqiZaizaoXmlData.NeedZaiSheng = Global.GetXElementAttributeInt(xelement, "NeedZaiZao");
				shenqiZaizaoXmlData.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				shenqiZaizaoXmlData.SuccessRate = Global.GetXElementAttributeDouble(xelement, "SuccessRate");
				int needEquitID = shenqiZaizaoXmlData.NeedEquitID;
				if (Global.ShenqiZaizaoXmlDataDict != null && !Global.ShenqiZaizaoXmlDataDict.ContainsKey(needEquitID))
				{
					Global.ShenqiZaizaoXmlDataDict.Add(needEquitID, shenqiZaizaoXmlData);
				}
			}
		}

		public static bool IsEnabledZaizao(GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return false;
			}
			if (Global.ShenqiZaizaoXmlDataDict == null)
			{
				Global.InitShenqiZaizaoXmlDataDict();
			}
			ShenqiZaizaoXmlData shenqiZaizaoXmlData = null;
			if (Global.ShenqiZaizaoXmlDataDict.TryGetValue(goodsData.GoodsID, ref shenqiZaizaoXmlData))
			{
				if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < shenqiZaizaoXmlData.NeedBandJinBi)
				{
					return false;
				}
				int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ZaizaoDian);
				if (roleCommonUseParamsValue < shenqiZaizaoXmlData.NeedZaiSheng)
				{
					return false;
				}
				if (!Global.IsGoodsEnough(shenqiZaizaoXmlData.NeedGoods))
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsGoodsEnough(string goodsStr)
		{
			if (string.IsNullOrEmpty(goodsStr))
			{
				return false;
			}
			string[] array = goodsStr.Split(new char[]
			{
				'|'
			});
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				if (array2 != null && array2.Length >= 2)
				{
					int num = array2[1].SafeToInt32(0);
					int totalGoodsCountByID = Global.GetTotalGoodsCountByID(array2[0].SafeToInt32(0));
					if (totalGoodsCountByID < num)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static XElement GetAddNewEquipPropsItem(string type, int occupation, int categoriy, int suitID)
		{
			if (categoriy < 0 || categoriy >= 25)
			{
				return null;
			}
			if (suitID < 1 || suitID > Global.MaxSuitID)
			{
				suitID = 1;
			}
			string xmlName = StringUtil.substitute("Config/SingleEquipAddProp/{0}/{1}.Xml", new object[]
			{
				type,
				occupation
			});
			XElement gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Equip", "Categoriy", categoriy.ToString());
			if (xelement == null)
			{
				return null;
			}
			return Global.GetXElement(xelement, "Item", "SuitID", suitID.ToString());
		}

		public static int GetJewelsJiaChengLevel(GoodsData goodsData)
		{
			AllThingsCalcItem jewelsJiaChengInfo = Global.GetJewelsJiaChengInfo(goodsData);
			if (jewelsJiaChengInfo == null)
			{
				return -1;
			}
			if (jewelsJiaChengInfo.TotalJewel8LevelNum >= 6)
			{
				return 2;
			}
			if (jewelsJiaChengInfo.TotalJewel6LevelNum + jewelsJiaChengInfo.TotalJewel7LevelNum + jewelsJiaChengInfo.TotalJewel8LevelNum >= 6)
			{
				return 1;
			}
			if (jewelsJiaChengInfo.TotalJewel4LevelNum + jewelsJiaChengInfo.TotalJewel5LevelNum + jewelsJiaChengInfo.TotalJewel6LevelNum + jewelsJiaChengInfo.TotalJewel7LevelNum + jewelsJiaChengInfo.TotalJewel8LevelNum >= 6)
			{
				return 0;
			}
			return -1;
		}

		public static int GetForgeJiaChengLevel(GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return -1;
			}
			if (goodsData.Forge_level >= 10)
			{
				return 3;
			}
			if (goodsData.Forge_level >= 9)
			{
				return 2;
			}
			if (goodsData.Forge_level >= 7)
			{
				return 1;
			}
			if (goodsData.Forge_level >= 5)
			{
				return 0;
			}
			return -1;
		}

		public static AllThingsCalcItem GetJewelsJiaChengInfo(GoodsData goodsData)
		{
			AllThingsCalcItem allThingsCalcItem = new AllThingsCalcItem();
			allThingsCalcItem.TotalJewel8LevelNum = 0;
			allThingsCalcItem.TotalJewel7LevelNum = 0;
			allThingsCalcItem.TotalJewel6LevelNum = 0;
			allThingsCalcItem.TotalJewel5LevelNum = 0;
			allThingsCalcItem.TotalJewel4LevelNum = 0;
			if (!string.IsNullOrEmpty(goodsData.Jewellist))
			{
				string[] array = goodsData.Jewellist.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					int jewelGoodsID = Convert.ToInt32(array[i]);
					int jewelLevel = Global.GetJewelLevel(jewelGoodsID);
					if (jewelLevel >= 8)
					{
						allThingsCalcItem.TotalJewel8LevelNum++;
					}
					else if (jewelLevel >= 7)
					{
						allThingsCalcItem.TotalJewel7LevelNum++;
					}
					else if (jewelLevel >= 6)
					{
						allThingsCalcItem.TotalJewel6LevelNum++;
					}
					else if (jewelLevel >= 5)
					{
						allThingsCalcItem.TotalJewel5LevelNum++;
					}
					else if (jewelLevel >= 4)
					{
						allThingsCalcItem.TotalJewel4LevelNum++;
					}
				}
			}
			return allThingsCalcItem;
		}

		private static List<double[]> ParseJiaChengProps(string props)
		{
			if (string.IsNullOrEmpty(props))
			{
				return null;
			}
			List<double[]> list = new List<double[]>();
			string[] array = props.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				double[] array2 = Global.String2DoubleArray(array[i], ',');
				list.Add(array2);
			}
			return list;
		}

		private static List<double[]> GetCachingSingleJiaChengPropList(string type, GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return null;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			int suitID = goodsXmlNodeByID.SuitID;
			int num = goodsXmlNodeByID.ToOccupation;
			num = Math.Max(0, num);
			string text = StringUtil.substitute("{0}_{1}_{2}_{3}", new object[]
			{
				type,
				categoriy,
				suitID,
				num
			});
			if (Global.AddNewPropsDict.ContainsKey(text))
			{
				return Global.AddNewPropsDict[text];
			}
			XElement addNewEquipPropsItem = Global.GetAddNewEquipPropsItem(type, num, categoriy, suitID);
			if (addNewEquipPropsItem == null)
			{
				return null;
			}
			List<double[]> list = Global.ParseJiaChengProps(Global.GetXElementAttributeStr(addNewEquipPropsItem, "EquipProps"));
			Global.AddNewPropsDict[text] = list;
			return list;
		}

		public static List<string> GetSingleForgeJiaCheng(GoodsData goodsData)
		{
			List<double[]> cachingSingleJiaChengPropList = Global.GetCachingSingleJiaChengPropList("QiangHua", goodsData);
			if (cachingSingleJiaChengPropList == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			int num = 0;
			while (num < cachingSingleJiaChengPropList.Count && num < 4)
			{
				double[] array = cachingSingleJiaChengPropList[num];
				int num2 = 0;
				string text = string.Empty;
				if (array != null)
				{
					int num3 = 0;
					while (num3 < array.Length && num3 < 10)
					{
						if (Convert.ToDouble(array[num3]) > 0.0)
						{
							if (!string.IsNullOrEmpty(text))
							{
								text += " ";
							}
							string text2 = StringUtil.substitute("{0}+{1}", new object[]
							{
								Global.AddNewPropNames[num3],
								array[num3]
							});
							text += text2;
							num2++;
							if (num2 >= 3)
							{
								break;
							}
						}
						num3++;
					}
				}
				string text3 = StringUtil.substitute("{0} {1}", new object[]
				{
					Global.ForgeAddNewPropPreNames[num],
					text
				});
				list.Add(text3);
				num++;
			}
			return list;
		}

		public static List<string> GetSingleJewelJiaCheng(GoodsData goodsData)
		{
			List<double[]> cachingSingleJiaChengPropList = Global.GetCachingSingleJiaChengPropList("Jewels", goodsData);
			if (cachingSingleJiaChengPropList == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			int num = 0;
			while (num < cachingSingleJiaChengPropList.Count && num < 4)
			{
				double[] array = cachingSingleJiaChengPropList[num];
				int num2 = 0;
				string text = string.Empty;
				if (array != null)
				{
					int num3 = 0;
					while (num3 < array.Length && num3 < 10)
					{
						if (Convert.ToDouble(array[num3]) > 0.0)
						{
							if (!string.IsNullOrEmpty(text))
							{
								text += " ";
							}
							string text2 = StringUtil.substitute("{0}+{1}", new object[]
							{
								Global.AddNewPropNames[num3],
								array[num3]
							});
							text += text2;
							num2++;
							if (num2 >= 3)
							{
								break;
							}
						}
						num3++;
					}
				}
				string text3 = StringUtil.substitute("{0} {1}", new object[]
				{
					Global.JewelsAddNewPropPreNames[num],
					text
				});
				list.Add(text3);
				num++;
			}
			return list;
		}

		public static List<string> GetNewAddProp(GoodsData goodsData)
		{
			List<double[]> cachingSingleJiaChengPropList = Global.GetCachingSingleJiaChengPropList("FuJia", goodsData);
			if (cachingSingleJiaChengPropList == null)
			{
				return null;
			}
			int num;
			if (goodsData.Quality >= 4)
			{
				num = 3;
			}
			else if (goodsData.Quality >= 3)
			{
				num = 2;
			}
			else if (goodsData.Quality >= 2)
			{
				num = 1;
			}
			else
			{
				if (goodsData.Quality < 1)
				{
					return null;
				}
				num = 0;
			}
			if (num >= cachingSingleJiaChengPropList.Count)
			{
				return null;
			}
			double[] array = cachingSingleJiaChengPropList[num];
			if (array == null || array.Length != 10)
			{
				return null;
			}
			int num2 = goodsData.AddPropIndex;
			num2 = Global.GMax(num2, 0);
			num2 = Global.GMin(num2, 10);
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (Convert.ToDouble(array[i]) > 0.0)
				{
					double num3 = array[i];
					double num4 = num3 * (double)num2;
					string text = StringUtil.substitute("{0}+{1}", new object[]
					{
						Global.AddNewPropNames[i],
						num3
					});
					string text2 = StringUtil.substitute(Global.GetLang("精锻+{0}"), new object[]
					{
						num4
					});
					string text3 = StringUtil.substitute("｛color=#{0} uline=false tag= text={1}｝  ｛color=#{2} uline=false tag= text={3}｝", new object[]
					{
						Global.GetEnchanceStrColor((GoodsQuality)goodsData.Quality),
						text,
						"FFFF00FF",
						text2
					});
					list.Add(text3);
				}
			}
			return list;
		}

		private static int CalcEquipPropsJiFen(double[] equipFields)
		{
			double num = 0.0;
			double num2 = equipFields[7] + equipFields[8];
			num2 /= 5.0;
			num += num2;
			num2 = equipFields[4];
			num2 /= 20.0;
			num += num2;
			num2 = equipFields[9] + equipFields[10];
			num2 /= 5.0;
			num += num2;
			num2 = equipFields[6];
			num2 /= 20.0;
			num += num2;
			num2 = equipFields[17];
			num2 /= 36.0;
			num += num2;
			num2 = equipFields[18];
			num2 /= 3.0;
			num += num2;
			num2 = equipFields[19];
			num2 /= 36.0;
			num += num2;
			num2 = equipFields[13];
			num2 /= 50.0;
			num += num2;
			num2 = equipFields[15];
			num2 /= 50.0;
			num += num2;
			num2 = equipFields[18];
			num2 /= 3.0;
			num += num2;
			return (int)num;
		}

		public static int CalcEquipPropsJiFen(int goodsID, GoodsData goodsData = null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return 0;
			}
			int categoriy = goodsXmlNodeByID.Categoriy;
			if (categoriy >= 25 && categoriy != 90)
			{
				return 0;
			}
			double[] array = goodsXmlNodeByID.EquipProps;
			if (array == null || array.Length <= 0)
			{
				return 0;
			}
			if (array.Length != 13)
			{
				return 0;
			}
			if (categoriy >= 0 && categoriy < 25 && goodsData != null)
			{
				array = Global.RecalcEquipProp(goodsXmlNodeByID, goodsData, array);
			}
			return Global.CalcEquipPropsJiFen(array);
		}

		public static int CalcEquipPropsJiFen(GoodsData goodsData)
		{
			int num = Global.CalcEquipPropsJiFen(goodsData.GoodsID, goodsData);
			if (string.IsNullOrEmpty(goodsData.Jewellist))
			{
				return num;
			}
			string[] array = goodsData.Jewellist.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = Global.SafeConvertToInt32(array[i]);
				if (num2 > 0)
				{
					num += Global.CalcEquipPropsJiFen(num2, null);
				}
			}
			return num;
		}

		public static int GetSystemHelpItemByMode(int prevID, SystemHelpModes mode, out XElement xmlItem)
		{
			xmlItem = null;
			XElement gameResXml = Global.GetGameResXml("Config/SystemHelp.Xml");
			if (gameResXml == null)
			{
				return -1;
			}
			int num = -1;
			foreach (XElement xelement in gameResXml.Elements())
			{
				num++;
				if (prevID == -1 || num > prevID)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "OccupCondition");
					if (xelementAttributeInt == -1 || xelementAttributeInt == Global.Data.roleData.Occupation)
					{
						int num2 = Global.GetXElementAttributeInt(xelement, "TriggerCondition");
						if (num2 < 0)
						{
							num2 = 5;
						}
						if (mode == (SystemHelpModes)num2)
						{
							int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "TimeParameters");
							if (mode == SystemHelpModes.ToLevel)
							{
								if (xelementAttributeInt2 == Global.Data.roleData.Level)
								{
									xmlItem = xelement;
									break;
								}
							}
							else if (mode == SystemHelpModes.ToMap)
							{
								if (xelementAttributeInt2 == Global.Data.roleData.MapCode)
								{
									xmlItem = xelement;
									break;
								}
							}
							else if (mode == SystemHelpModes.FirstLogin)
							{
								if (Global.Data.roleData.LoginNum <= 0)
								{
									xmlItem = xelement;
									break;
								}
							}
							else if (mode == SystemHelpModes.FirstGoods)
							{
								if (Global.Data.FirstNewGoodsIDList.Count > 0)
								{
									int num3 = Global.Data.FirstNewGoodsIDList[0];
									if (xelementAttributeInt2 == num3)
									{
										xmlItem = xelement;
										break;
									}
								}
							}
							else
							{
								if (mode == SystemHelpModes.Login)
								{
									xmlItem = xelement;
									break;
								}
								if (mode == SystemHelpModes.NewTask)
								{
									if (Global.Data.LastedNewTaskIDList.Count > 0)
									{
										int num4 = Global.Data.LastedNewTaskIDList[0];
										if (xelementAttributeInt2 == num4)
										{
											xmlItem = xelement;
											break;
										}
									}
								}
								else if (mode == SystemHelpModes.CompTask)
								{
									if (Global.Data.LastedCompTaskIDList.Count > 0)
									{
										int num5 = Global.Data.LastedCompTaskIDList[0];
										if (xelementAttributeInt2 == num5)
										{
											xmlItem = xelement;
											break;
										}
									}
								}
								else if (mode == SystemHelpModes.LeaveSafeArea)
								{
									xmlItem = xelement;
									break;
								}
							}
						}
					}
				}
			}
			if (mode == SystemHelpModes.FirstGoods && Global.Data.FirstNewGoodsIDList.Count > 0 && xmlItem == null)
			{
				Global.Data.FirstNewGoodsIDList.RemoveAt(0);
			}
			if (mode == SystemHelpModes.NewTask && Global.Data.LastedNewTaskIDList.Count > 0 && xmlItem == null)
			{
				Global.Data.LastedNewTaskIDList.RemoveAt(0);
			}
			if (mode == SystemHelpModes.CompTask && Global.Data.LastedCompTaskIDList.Count > 0 && xmlItem == null)
			{
				Global.Data.LastedCompTaskIDList.RemoveAt(0);
			}
			return num;
		}

		public static MapTypes GetMapType(int mapCode)
		{
			if (Global.mapTypeDict.ContainsKey(mapCode))
			{
				return (MapTypes)Global.mapTypeDict[mapCode];
			}
			string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return MapTypes.Normal;
			}
			XElement xelement = Global.GetXElement(gameMapSettingsXml, "Settings");
			if (xelement == null)
			{
				return MapTypes.Normal;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "IsolatedMap");
			Global.mapTypeDict[mapCode] = xelementAttributeInt;
			return (MapTypes)xelementAttributeInt;
		}

		public static int GetMapPKMode(int mapCode)
		{
			string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameMapSettingsXml, "Settings");
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "PKMode");
		}

		public static int GetMapRealiveMode(int mapCode)
		{
			string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameMapSettingsXml, "Settings");
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "RealiveMode");
		}

		public static int GetMapCanUseHorse(int mapCode)
		{
			string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameMapSettingsXml, "Settings");
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "RealiveMode");
		}

		public static void GetMapMinLevelAndZhuanSheng(int mapCode, out int minLevel, out int minZhuanSheng)
		{
			minLevel = 0;
			minZhuanSheng = 0;
			string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return;
			}
			XElement xelement = Global.GetXElement(gameMapSettingsXml, "Limits");
			if (xelement == null)
			{
				return;
			}
			minZhuanSheng = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
			minLevel = Global.GetXElementAttributeInt(xelement, "MinLevel");
		}

		public static string GetMapPKModeName(int mapCode)
		{
			int mapPKMode = Global.GetMapPKMode(mapCode);
			if (mapPKMode < 0 || mapPKMode >= Global.MapPKModes.Length)
			{
				return Global.GetLang("未知");
			}
			return Global.MapPKModes[mapPKMode];
		}

		public static string GetMapPKModeHint(int mapCode)
		{
			int mapPKMode = Global.GetMapPKMode(mapCode);
			if (mapPKMode < 0 || mapPKMode >= Global.MapPKHints.Length)
			{
				return string.Empty;
			}
			return Global.MapPKHints[mapPKMode];
		}

		public static string GetPKModeName(int pkMode)
		{
			switch (pkMode)
			{
			case 0:
				return Global.GetLang("和平");
			case 1:
				return Global.GetLang("全体");
			case 2:
				return Global.GetLang("战盟");
			case 3:
				return Global.GetLang("队伍");
			case 4:
				return Global.GetLang("善恶");
			case 7:
				return Global.GetLang("军团");
			}
			return "未知";
		}

		public static int GetNPCOrMonsterMapCodeByID(XElement xmlItem)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xmlItem, "MapCode");
			if (xelementAttributeInt < 0)
			{
				return Global.Data.roleData.MapCode;
			}
			return xelementAttributeInt;
		}

		public static int GetNPCOrMonsterMapCodeByID(int mapCode)
		{
			if (mapCode < 0)
			{
				return Global.Data.roleData.MapCode;
			}
			return mapCode;
		}

		public static bool CanMapAutoFight(int mapCode)
		{
			string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
			if (gameMapSettingsXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameMapSettingsXml, "Limits");
			if (xelement == null)
			{
				return false;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "AutoFight");
			return xelementAttributeInt > 0;
		}

		public static bool CanMapHelpHint(int mapCode, int helpHintID)
		{
			string text = string.Format("{0}_{1}", mapCode, helpHintID);
			int[] array = null;
			if (!Global.MapHelpHintDict.TryGetValue(text, ref array))
			{
				string xmlName = StringUtil.substitute("MapConfig.xml", new object[0]);
				XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, xmlName);
				if (gameMapSettingsXml != null)
				{
					XElement xelement = Global.GetXElement(gameMapSettingsXml, "Limits");
					if (xelement != null)
					{
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "HelpHint");
						if (!string.IsNullOrEmpty(xelementAttributeStr))
						{
							array = Global.String2IntArray(xelementAttributeStr, ',');
						}
					}
				}
				Global.MapHelpHintDict[text] = array;
			}
			if (array == null)
			{
				return true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == helpHintID)
				{
					return false;
				}
			}
			return true;
		}

		public static bool CanMapUseMagic(GMapData currentMapData, int magicCode)
		{
			if (currentMapData.LimitMagicIDs == null || currentMapData.LimitMagicIDs.Count <= 0)
			{
				return true;
			}
			for (int i = 0; i < currentMapData.LimitMagicIDs.Count; i++)
			{
				if (magicCode == currentMapData.LimitMagicIDs[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool CanMapUseGoods(GMapData currentMapData, int goodsID)
		{
			if (currentMapData.LimitGoodsIDs == null || currentMapData.LimitGoodsIDs.Count <= 0)
			{
				return true;
			}
			for (int i = 0; i < currentMapData.LimitGoodsIDs.Count; i++)
			{
				if (goodsID == currentMapData.LimitGoodsIDs[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool IsSpecialNormalMapCode(int mapCode)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("SpecialMapCodes", ',');
			if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length <= 0)
			{
				return false;
			}
			for (int i = 0; i < systemParamIntArrayByName.Length; i++)
			{
				if (mapCode == systemParamIntArrayByName[i])
				{
					return true;
				}
			}
			return false;
		}

		public static SceneUIClasses GetMapSceneUIClass()
		{
			return SceneUIClasseExt.GetMapSceneUIClass();
		}

		public static bool IsKuaFuHuoDongMapSceneUIClass(SceneUIClasses sceneUI)
		{
			bool result = false;
			if (sceneUI == SceneUIClasses.HuanYingSiYuan || sceneUI == SceneUIClasses.TianTi || sceneUI == SceneUIClasses.YongZheZhanChang || sceneUI == SceneUIClasses.KuaFuWangZhe || sceneUI == SceneUIClasses.ElementWar || sceneUI == SceneUIClasses.MoRiJudge || sceneUI == SceneUIClasses.CopyWolf || sceneUI == SceneUIClasses.KuaFuBoss || sceneUI == SceneUIClasses.LangHunLingYu || sceneUI == SceneUIClasses.ZhongShenZhengBa || sceneUI == SceneUIClasses.PKLovers || sceneUI == SceneUIClasses.ZhengDuoZhiDi || sceneUI == SceneUIClasses.LingDiCaiJi || sceneUI == SceneUIClasses.AKaLunXi || sceneUI == SceneUIClasses.AKaLunDong || sceneUI == SceneUIClasses.ZhanMengLianSaiBiSai || sceneUI == SceneUIClasses.KuaFuPlunderBattle || sceneUI == SceneUIClasses.WanMoXiaGu || sceneUI == SceneUIClasses.KaLiMaTemple || sceneUI == SceneUIClasses.CompBattle || sceneUI == SceneUIClasses.CompBattleMiDong || sceneUI == SceneUIClasses.KuaFuTeamCompete || sceneUI == SceneUIClasses.KuaFuTeamCompeteZhengBa || sceneUI == SceneUIClasses.MoYuDuoBao || sceneUI == SceneUIClasses.DaTaoSha || sceneUI == SceneUIClasses.DaTaoShaPrepare)
			{
				result = true;
			}
			return result;
		}

		public static bool InActivityMap()
		{
			SceneUIClasses mapSceneUIClass = Global.GetMapSceneUIClass();
			SceneUIClasses sceneUIClasses = mapSceneUIClass;
			switch (sceneUIClasses)
			{
			case SceneUIClasses.HuanYingSiYuan:
			case SceneUIClasses.TianTi:
			case SceneUIClasses.YongZheZhanChang:
			case SceneUIClasses.ElementWar:
			case SceneUIClasses.MoRiJudge:
			case SceneUIClasses.KuaFuBoss:
			case SceneUIClasses.CopyWolf:
			case SceneUIClasses.ZhongShenZhengBa:
			case SceneUIClasses.PKLovers:
			case SceneUIClasses.KuaFuWangZhe:
			case SceneUIClasses.ZhengDuoZhiDi:
			case SceneUIClasses.AKaLunXi:
			case SceneUIClasses.AKaLunDong:
			case SceneUIClasses.LingDiCaiJi:
			case SceneUIClasses.ZhanMengLianSaiBiSai:
			case SceneUIClasses.WanMoXiaGu:
			case SceneUIClasses.CompBattle:
				break;
			default:
				switch (sceneUIClasses)
				{
				case SceneUIClasses.BloodCastle:
				case SceneUIClasses.Demon:
				case SceneUIClasses.Battle:
				case SceneUIClasses.PKKing:
				case SceneUIClasses.AngelTemple:
					return true;
				}
				return false;
			}
			return true;
		}

		public static double GetEquipBasePropsItemVal(GoodsData gd, double[] da, int itemIndex)
		{
			return 0.0;
		}

		public static double GetEquipForgeAddActiveExtraValue(GoodsData gd, int extPropIndex)
		{
			if (gd.Forge_level <= 0 || gd.Forge_level > Global.MaxForgeLevel)
			{
				return 0.0;
			}
			if (extPropIndex < 0 || extPropIndex >= 177)
			{
				return 0.0;
			}
			string text = ExtPropIndexesExt.ExtPropIndexNames[extPropIndex];
			text = text.ToLower();
			bool flag = false;
			if (extPropIndex >= 3 && extPropIndex <= 10 && text.Length > 3 && (text.IndexOf("min") == 0 || text.IndexOf("max") == 0))
			{
				flag = ("max" == text.Substring(0, 3));
				text = text.Substring(3);
			}
			int forge_level = gd.Forge_level;
			long num = 0L;
			string[] equipForgeAddActivateList = Global.GetEquipForgeAddActivateList(gd.GoodsID);
			for (int i = 0; i < equipForgeAddActivateList.Length; i++)
			{
				string[] array = equipForgeAddActivateList[i].ToString().Split(new char[]
				{
					','
				});
				if (array.Length == 3)
				{
					int num2 = Global.SafeConvertToInt32(array[0]);
					string text2 = array[1].ToString().ToLower();
					if (forge_level >= num2 && text == text2)
					{
						long num3;
						if (flag && array[2].ToString().Split(new char[]
						{
							'-'
						}).Length == 2)
						{
							num3 = (long)Global.SafeConvertToDouble(array[2].Split(new char[]
							{
								'-'
							})[1]);
						}
						else
						{
							num3 = (long)Global.SafeConvertToDouble(array[2].Split(new char[]
							{
								'-'
							})[0]);
						}
						num += num3;
					}
				}
			}
			return (double)num;
		}

		public static int GetEquipForgeAddActiveExtraLifeValue(GoodsData gd)
		{
			int roleBaseLifeV = Global.GetRoleBaseLifeV(Global.Data.roleData.Occupation, Global.Data.roleData.Level);
			double equipForgeAddActiveExtraValue = Global.GetEquipForgeAddActiveExtraValue(gd, 13);
			double equipForgeAddActiveExtraValue2 = Global.GetEquipForgeAddActiveExtraValue(gd, 14);
			double num = (double)roleBaseLifeV * equipForgeAddActiveExtraValue2;
			return (int)(equipForgeAddActiveExtraValue + num);
		}

		public static int GetRoleBaseLifeV(int occupation, int level)
		{
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/Roles/{0}.Xml", occupation));
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Level", "Level", level.ToString());
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "LifeV");
		}

		public static double[] GetGoodsEquipPropsDoubleList(int goodsID)
		{
			List<double> list = new List<double>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID != null)
			{
				double[] equipProps = goodsXmlNodeByID.EquipProps;
				list = Enumerable.ToList<double>(equipProps);
			}
			while (list.Count < 177)
			{
				list.Add(0.0);
			}
			return list.ToArray();
		}

		public static int GetBufferGoodsID(int bufferType, int index)
		{
			if (index < 0)
			{
				return 0;
			}
			string text = string.Empty;
			if (bufferType == 31)
			{
				text = "ChengJiuBufferGoodsIDs";
			}
			else if (bufferType == 32)
			{
				text = "JingMaiBufferGoodsIDs";
			}
			else if (bufferType == 33)
			{
				text = "WuXueBufferGoodsIDs";
			}
			else if (bufferType == 49)
			{
				text = "ZhuanhuangBufferGoodsIDs";
			}
			else if (bufferType == 50)
			{
				text = "ZhanhunBufferGoodsIDs";
			}
			else if (bufferType == 51)
			{
				text = "RongyaoBufferGoodsIDs";
			}
			else if (bufferType == 16)
			{
				text = "JunQiBufferGoodsIDs";
			}
			else if (bufferType == 86)
			{
				text = "AngelTempleGoldBuffGoodsID";
			}
			else if (bufferType == 49)
			{
				text = "ZhuanhuangBufferGoodsIDs";
			}
			else if (bufferType == 88)
			{
				text = "ZhanMengZhanQiBUFF";
			}
			else if (bufferType == 89)
			{
				text = "ZhanMengJiTanBUFF";
			}
			else if (bufferType == 90)
			{
				text = "ZhanMengJunXieBUFF";
			}
			else if (bufferType == 91)
			{
				text = "ZhanMengGuangHuanBUFF";
			}
			else if (bufferType == 39)
			{
				text = "PkKingBuff";
			}
			else if (bufferType == 87)
			{
				text = "JunXianBufferGoodsIDs";
			}
			else if (bufferType == 85 || bufferType == 86)
			{
				text = "AngelTempleGoldBuffGoodsID";
			}
			else if (bufferType == 103)
			{
				text = "LangHunLingYuBuff";
			}
			else if (bufferType == 111)
			{
				text = "ZhongShenZhiShenBuff";
			}
			else if (bufferType == 2080011)
			{
				text = "CoupleBuffSpecificHurt";
			}
			else if (bufferType == 2080010)
			{
				text = "CoupleVictoryNeedTime";
			}
			if (text.Length <= 0)
			{
				return 0;
			}
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName(text, ',');
			if (systemParamIntArrayByName == null || index > systemParamIntArrayByName.Length - 1)
			{
				return 0;
			}
			return systemParamIntArrayByName[index];
		}

		public static string GetGoodsEquipPropsStringForBufferTips(int goodsID)
		{
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsID);
			string text = string.Empty;
			for (int i = 0; i < goodsEquipPropsDoubleList.Length; i++)
			{
				if (goodsEquipPropsDoubleList[i] > 0.0)
				{
					string text2 = ExtPropIndexes.ExtPropIndexChineseNames[i];
					string text3 = goodsEquipPropsDoubleList[i].ToString();
					if (ExtPropIndexes.ExtPropIndexPercents[i] > 0)
					{
						text3 = string.Format("+{0}", goodsEquipPropsDoubleList[i].ToString("p0"));
					}
					else if (i >= 2 && i <= 10)
					{
						text3 = string.Format("{0}-{1}", text3, (int)goodsEquipPropsDoubleList[i + 1]);
						i++;
					}
					if (text.Length > 0)
					{
						text += "\n";
					}
					text += string.Format("{0}: {1}", text2, text3);
				}
			}
			return text;
		}

		public static string[] GetEquipForgeAddActivateList(int goodsID)
		{
			string[] result = null;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return result;
			}
			string execMagic = goodsXmlNodeByID.ExecMagic;
			if (execMagic.IndexOf("DB_ADD_YINYONG") < 0)
			{
				return result;
			}
			int num = execMagic.IndexOf("(");
			int num2 = execMagic.IndexOf(",");
			if (num2 - (num + 1) <= 0)
			{
				return result;
			}
			int num3 = Global.SafeConvertToInt32(execMagic.Substring(num + 1, num2 - (num + 1)));
			if (num3 < 0)
			{
				return result;
			}
			XElement gameResXml = Global.GetGameResXml("Config/QiangHua.Xml");
			if (gameResXml == null)
			{
				return result;
			}
			XElement xelement = Global.GetXElement(gameResXml, "QiangHua", "ID", num3.ToString());
			if (xelement == null)
			{
				return result;
			}
			string text = Global.GetXElementAttributeStr(xelement, "QiangHua");
			text = text.ToLower();
			return text.Split(new char[]
			{
				'|'
			});
		}

		public static double GetEquipForgeAddBaseValue(GoodsData gd, int extPropIndex)
		{
			int forge_level = gd.Forge_level;
			if ((forge_level > 20 || forge_level <= 0) && Global.GetCategoriyByGoodsID(gd.GoodsID) != 10 && Global.GetCategoriyByGoodsID(gd.GoodsID) != 9)
			{
				return 0.0;
			}
			if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 8)
			{
				if (extPropIndex == 4 || extPropIndex == 6)
				{
					if (Global.ChiBangForgeLevelAddDefenseRates == null)
					{
						Global.ChiBangForgeLevelAddDefenseRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("WingForgeLevelAddDefenseRates");
					}
					return Global.ChiBangForgeLevelAddDefenseRates[forge_level];
				}
				if (extPropIndex == 26)
				{
					if (Global.ChiBangForgeLevelAddShangHaiJiaCheng == null)
					{
						Global.ChiBangForgeLevelAddShangHaiJiaCheng = ConfigSystemParam.GetSystemParamDoubleArrayByName("WingForgeLevelAddShangHaiJiaCheng");
					}
					return Global.ChiBangForgeLevelAddShangHaiJiaCheng[forge_level];
				}
				if (extPropIndex == 24)
				{
					if (Global.ChiBangForgeLevelAddShangHaiXiShou == null)
					{
						Global.ChiBangForgeLevelAddShangHaiXiShou = ConfigSystemParam.GetSystemParamDoubleArrayByName("WingForgeLevelAddShangHaiXiShou");
					}
					return Global.ChiBangForgeLevelAddShangHaiXiShou[forge_level];
				}
			}
			else
			{
				if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 10 || Global.GetCategoriyByGoodsID(gd.GoodsID) == 9)
				{
					if (Global.ForgeLevelPet == null)
					{
						Global.ForgeLevelPet = new double[177];
						string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PetQiangHuaProps", '|');
						for (int i = 0; i < systemParamStringArrayByName.Length; i++)
						{
							string[] array = systemParamStringArrayByName[i].Split(new char[]
							{
								','
							});
							Global.ForgeLevelPet[Convert.ToInt32(array[0])] = Convert.ToDouble(array[1]);
						}
					}
					return Global.ForgeLevelPet[extPropIndex] * (double)gd.Forge_level;
				}
				if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 340)
				{
					if (IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(gd.GoodsID, gd.Forge_level + 1) != null)
					{
						return (double)IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(gd.GoodsID, gd.Forge_level + 1).AdvancedEffectFloat;
					}
				}
				else
				{
					if (extPropIndex == 8 || extPropIndex == 10)
					{
						if (Global.ForgeLevelAddAttackRates == null)
						{
							Global.ForgeLevelAddAttackRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ForgeLevelAddAttackRates");
						}
						return Global.ForgeLevelAddAttackRates[forge_level];
					}
					if (extPropIndex == 4 || extPropIndex == 6)
					{
						if (Global.ForgeLevelAddDefenseRates == null)
						{
							Global.ForgeLevelAddDefenseRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ForgeLevelAddDefenseRates");
						}
						return Global.ForgeLevelAddDefenseRates[forge_level];
					}
					if (extPropIndex == 13)
					{
						if (Global.ForgeLevelAddMaxLifeVRates == null)
						{
							Global.ForgeLevelAddMaxLifeVRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ForgeLevelAddMaxLifeVRates");
						}
						return Global.ForgeLevelAddMaxLifeVRates[forge_level];
					}
				}
			}
			return 0.0;
		}

		public static double GetZhuoYueAddDefenseRates(int flag)
		{
			if (Global.ZhuoYueAddDefenseRates == null)
			{
				Global.ZhuoYueAddDefenseRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuoYueAddDefenseRates");
			}
			return Global.ZhuoYueAddDefenseRates[Global.GetZhuoYueAddIndex(flag)];
		}

		public static double GetZhuoYueAddDefenseRates(GoodsData goodData)
		{
			if (Global.ZhuoYueAddDefenseRates == null)
			{
				Global.ZhuoYueAddDefenseRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuoYueAddDefenseRates");
			}
			return Global.ZhuoYueAddDefenseRates[Global.GetZhuoYueAddIndex(goodData)];
		}

		public static double GetZhuoYueAddAttackRates(int flag)
		{
			if (Global.ZhuoYueAddAttackRates == null)
			{
				Global.ZhuoYueAddAttackRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuoYueAddAttackRates");
			}
			return Global.ZhuoYueAddAttackRates[Global.GetZhuoYueAddIndex(flag)];
		}

		public static double GetZhuoYueAddAttackRates(GoodsData goodData)
		{
			if (Global.ZhuoYueAddAttackRates == null)
			{
				Global.ZhuoYueAddAttackRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuoYueAddAttackRates");
			}
			return Global.ZhuoYueAddAttackRates[Global.GetZhuoYueAddIndex(goodData)];
		}

		public static int GetZhuoYueAddIndex(int flag)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(flag);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				return 0;
			}
			if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				return 1;
			}
			if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				return 2;
			}
			return 0;
		}

		public static int GetZhuoYueAddIndex(GoodsData goodsData)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				return 0;
			}
			if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				return 1;
			}
			if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				return 2;
			}
			return 0;
		}

		public static int GetBornAttackValue(int bornIndex, int attackType)
		{
			bornIndex >>= attackType * 8;
			return bornIndex & 255;
		}

		public static double GetEquipBornAddBaseValue(GoodsData gd, int extPropIndex)
		{
			if (extPropIndex != 8 && extPropIndex != 10)
			{
				return 0.0;
			}
			if (gd.BornIndex <= 0)
			{
				return 0.0;
			}
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(gd.GoodsID);
			if (goodsEquipPropsDoubleList == null || goodsEquipPropsDoubleList[extPropIndex] <= 0.0)
			{
				return 0.0;
			}
			int attackType = 0;
			if (extPropIndex == 8)
			{
				attackType = 0;
			}
			else if (extPropIndex == 10)
			{
				attackType = 1;
			}
			return (double)Global.GetBornAttackValue(gd.BornIndex, attackType);
		}

		public static double GetEquipZhuijiaAddBaseValue(GoodsData gd, int extPropIndex)
		{
			int appendPropLev = gd.AppendPropLev;
			if (appendPropLev > 80 || appendPropLev < 0)
			{
				return 0.0;
			}
			if (extPropIndex == 13)
			{
				if (Global.ZhuijiaLevelAddDefenseRates == null)
				{
					Global.ZhuijiaLevelAddDefenseRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuiJiaLevelAddDefenseRates");
				}
				return Global.ZhuijiaLevelAddDefenseRates[appendPropLev];
			}
			if (extPropIndex == 8 || extPropIndex == 10)
			{
				if (Global.ZhuijiaLevelAddAttackRates == null)
				{
					Global.ZhuijiaLevelAddAttackRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuiJiaLevelAddAttackRates");
				}
				return Global.ZhuijiaLevelAddAttackRates[appendPropLev];
			}
			return 0.0;
		}

		public static double GetEquipZhuanshengAddBaseValue(GoodsData gd, int extPropIndex)
		{
			int changeLifeLevForEquip = gd.ChangeLifeLevForEquip;
			if (changeLifeLevForEquip > 10 || changeLifeLevForEquip < 0)
			{
				return 0.0;
			}
			if (extPropIndex == 4 || extPropIndex == 6)
			{
				if (Global.ZhuanshengLevelAddDefenseRates == null)
				{
					Global.ZhuanshengLevelAddDefenseRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("EquipZhuanShengAddDefenseRates");
				}
				return Global.ZhuanshengLevelAddDefenseRates[changeLifeLevForEquip];
			}
			if (extPropIndex == 8 || extPropIndex == 10)
			{
				if (Global.ZhuanshengLevelAddAttackRates == null)
				{
					Global.ZhuanshengLevelAddAttackRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("EquipZhuanShengAddAttackRates");
				}
				return Global.ZhuanshengLevelAddAttackRates[changeLifeLevForEquip];
			}
			return 0.0;
		}

		public static string GetBaseAttributeStrFromPropertyList(double[] equipFields, bool defaultExpression = true, int fillCount = 0)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = "e3b36c";
			int level = Global.guardStatueLevel;
			int grade = Global.guardStatueGrade;
			string text4 = string.Empty;
			string text5 = " ";
			for (int i = 0; i < fillCount; i++)
			{
				text4 += text5;
			}
			for (int j = 1; j <= 10; j += 2)
			{
				if (equipFields[j] != 0.0)
				{
					text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
					text2 = Global.GetColorStringForNGUIText(new object[]
					{
						text3,
						text2 + ": " + text4
					});
					double num = equipFields[j];
					if (j == 1)
					{
						if (equipFields[j] != 0.0)
						{
							text += string.Format("{0}{1}%", text2, (int)equipFields[j]);
							text += "\n";
						}
					}
					else
					{
						int num2 = j;
						int num3 = j + 1;
						if (equipFields[num2] != 0.0 || equipFields[num3] != 0.0)
						{
							double num4 = equipFields[num2];
							double num5 = equipFields[num3];
							text += string.Format("{0}{1}", text2, (!defaultExpression) ? ((int)Global.CalStatueExpress(num5, grade, level)) : ((int)num5));
						}
					}
					text += "\n";
				}
			}
			for (int j = 11; j < 177; j++)
			{
				if (equipFields[j] != 0.0)
				{
					text2 = Global.GetLang(ExtPropIndexes.ExtPropIndexChineseNames[j]);
					text2 = Global.GetColorStringForNGUIText(new object[]
					{
						text3,
						text2 + ": " + text4
					});
					double num6 = equipFields[j];
					if (ExtPropIndexes.ExtPropIndexPercents[j] == 1)
					{
						double num7 = (!defaultExpression) ? (Global.CalStatueExpress(num6, grade, level) * 100.0) : (num6 * 100.0);
						text += string.Format("{0}{1}%", text2, Math.Round(num7, 2));
					}
					else if (ExtPropIndexes.ExtPropIndexPercents[j] == 0)
					{
						text += string.Format("{0}{1}", text2, (!defaultExpression) ? ((int)Global.CalStatueExpress(num6, grade, level)) : ((int)num6));
					}
					text += "\n";
				}
			}
			return Global.ProcessStr(text);
		}

		public static double CalStatueExpress(double baseAttr, int grade, int level)
		{
			double levelFactor = 0.0;
			double gradeFactor = 0.0;
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("ShouHuLevel", true);
			string systemParamByName2 = ConfigSystemParam.GetSystemParamByName("ShouHuSuit", true);
			if (!string.IsNullOrEmpty(systemParamByName))
			{
				levelFactor = Convert.ToDouble(systemParamByName);
			}
			if (!string.IsNullOrEmpty(systemParamByName2))
			{
				gradeFactor = Convert.ToDouble(systemParamByName2);
			}
			return Global.CalStatueExpression(baseAttr, grade, level, gradeFactor, levelFactor);
		}

		public static double CalStatueExpression(double baseAttr, int grade, int level, double gradeFactor = 0.0, double levelFactor = 0.0)
		{
			return baseAttr * (1.0 + (double)level * levelFactor + (double)(grade - 1) * gradeFactor);
		}

		public static string ProcessStr(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}
			if (str.Length > 0 && str.Substring(str.Length - 1) == "\n")
			{
				str = str.Substring(0, str.Length - 1);
			}
			return str;
		}

		public static double GetEquipExtPropsItemVal(GoodsData gd, double[] da, int itemIndex, GoodVO goodVO)
		{
			if (goodVO == null)
			{
				return 0.0;
			}
			if (gd == null)
			{
				return 0.0;
			}
			int suitID = goodVO.SuitID;
			double num = da[itemIndex];
			if (itemIndex != 8 && itemIndex != 10)
			{
				return num;
			}
			num += Global.GetEquipBornAddBaseValue(gd, itemIndex);
			num += Global.GetEquipForgeAddBaseValue(gd, itemIndex);
			return (double)((int)Math.Floor(num));
		}

		public static double[] RecalcEquipProp(GoodVO goodVO, GoodsData goodsData, double[] sa)
		{
			if (goodsData == null)
			{
				return sa;
			}
			for (int i = 0; i < 177; i++)
			{
				sa[i] = Global.GetEquipExtPropsItemVal(goodsData, sa, i, goodVO);
			}
			return sa;
		}

		public static double CalcEquipQualityProp(long[] sa, int extPropIndexes1, int extPropIndexes2, GoodsData goodsData)
		{
			if (sa == null)
			{
				return 0.0;
			}
			if (goodsData == null)
			{
				return 0.0;
			}
			if (5 + extPropIndexes1 >= sa.Length)
			{
				return 0.0;
			}
			if (5 + extPropIndexes2 >= sa.Length)
			{
				return 0.0;
			}
			int num = (int)((sa[extPropIndexes1] + sa[extPropIndexes2]) / 2L);
			int num2 = goodsData.Quality;
			if (num2 >= 7)
			{
				num2 = 6;
			}
			int num3 = num2;
			long num4 = (long)num;
			if (Global.EnchanceLevelAddRates == null)
			{
				Global.EnchanceLevelAddRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("EnchanceLevelAddRates");
			}
			if (Global.EnchanceLevelAddRates == null || Global.EnchanceLevelAddRates.Length != 5)
			{
				return 0.0;
			}
			return (double)num4 * Global.EnchanceLevelAddRates[num3];
		}

		public static double CalcEquipBornProp(double[] sa, int extPropIndexes1, int extPropIndexes2, GoodsData goodsData)
		{
			if (sa == null)
			{
				return 0.0;
			}
			if (goodsData == null)
			{
				return 0.0;
			}
			if (5 + extPropIndexes1 >= sa.Length)
			{
				return 0.0;
			}
			if (5 + extPropIndexes2 >= sa.Length)
			{
				return 0.0;
			}
			int num = (int)((sa[extPropIndexes1] + sa[extPropIndexes2]) / 2.0);
			if (goodsData.BornIndex <= 0)
			{
				return 0.0;
			}
			int num2 = Global.GMin(goodsData.BornIndex, 100);
			double num3 = (double)num2 / 100.0;
			return (double)num * num3;
		}

		public static double CalcEquipBornWithQualityProp(double[] sa, int extPropIndexes1, int extPropIndexes2, GoodsData goodsData)
		{
			if (sa == null)
			{
				return 0.0;
			}
			if (goodsData == null)
			{
				return 0.0;
			}
			if (5 + extPropIndexes1 >= sa.Length)
			{
				return 0.0;
			}
			if (5 + extPropIndexes2 >= sa.Length)
			{
				return 0.0;
			}
			int num = (int)((sa[extPropIndexes1] + sa[extPropIndexes2]) / 2.0);
			if (goodsData.BornIndex <= 0)
			{
				return 0.0;
			}
			int num2 = Global.GMin(goodsData.BornIndex, 100);
			double num3 = (double)num2 / 100.0;
			num = (int)((double)num * num3);
			int num4 = goodsData.Quality;
			if (num4 >= 7)
			{
				num4 = 6;
			}
			int num5 = num4;
			double num6 = (double)num;
			if (Global.EnchanceLevelAddRates == null)
			{
				Global.EnchanceLevelAddRates = ConfigSystemParam.GetSystemParamDoubleArrayByName("EnchanceLevelAddRates");
			}
			if (Global.EnchanceLevelAddRates == null || Global.EnchanceLevelAddRates.Length != 5)
			{
				return 0.0;
			}
			return num6 * Global.EnchanceLevelAddRates[num5];
		}

		public static int GetZhuoyueAttributeCount(GoodsData gd)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(gd.GoodsID);
			if (Global.IsRebornEquip(categoriyByGoodsID))
			{
				if (gd.WashProps == null)
				{
					return 0;
				}
				return gd.WashProps.Count / 2;
			}
			else
			{
				if (categoriyByGoodsID != 980)
				{
					return Global.GetZhuoyueAttributeCount(gd.ExcellenceInfo);
				}
				if (gd.WashProps == null)
				{
					return 0;
				}
				return gd.WashProps.Count / 3;
			}
		}

		public static int GetZhuoyueAttributeCount(int excellenceInfo)
		{
			if (excellenceInfo <= 0)
			{
				return 0;
			}
			int num = 0;
			int num2 = 32;
			for (int i = 0; i < num2; i++)
			{
				if (Global.GetIntSomeBit(excellenceInfo, i) == 1)
				{
					num++;
				}
			}
			return num;
		}

		public static int GetMaxZhuijiaLevelByGoodsData(GoodsData goodsData)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount == 0)
			{
				return 20;
			}
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				return 40;
			}
			if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				return 60;
			}
			if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				return 80;
			}
			return 0;
		}

		public static int GetMaxWashPropsCountByGoodsData(GoodsData goodsData)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount == 0)
			{
				return 0;
			}
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				return 2;
			}
			if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				return 4;
			}
			if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				return 6;
			}
			return 0;
		}

		public static uint GetColorByGoodsData(GoodsData goodsData)
		{
			uint result = 16777215U;
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				result = 65280U;
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				result = 39423U;
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				result = 16713983U;
			}
			return result;
		}

		public static string GetStrColorByGoodsData(GoodsData goodsData)
		{
			string result = "ffffff";
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
			if ((goodsCatetoriy >= 0 && goodsCatetoriy <= 22) || Global.IsRebornEquip(goodsCatetoriy))
			{
				int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
				if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
				{
					result = "00ff00";
				}
				else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
				{
					result = "0099ff";
				}
				else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
				{
					result = "ff08ff";
				}
			}
			else
			{
				result = Global.GetGoodsColorString(goodsData.GoodsID);
			}
			return result;
		}

		public static GoodsColor GetGoodsColorByGoodsData(GoodsData goodsData)
		{
			GoodsColor result = GoodsColor.White;
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				return GoodsColor.Green;
			}
			if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				return GoodsColor.Blue;
			}
			if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				return GoodsColor.Purple;
			}
			return result;
		}

		public static Color GetColorByGoodsDataUnityColor(GoodsData goodsData)
		{
			uint colorByGoodsData = Global.GetColorByGoodsData(goodsData);
			return new Color((colorByGoodsData >> 16 & 255U) / 255f, (colorByGoodsData >> 8 & 255U) / 255f, (colorByGoodsData & 255U) / 255f);
		}

		public static bool AssertException(bool result, string msg)
		{
			if (result)
			{
				return true;
			}
			GError.AddErrMsg(string.Format("AssertException: {0}", msg));
			return false;
		}

		public static bool IsInArenaBattleMap()
		{
			if (Global.mapCode_XuezhanDifu == 0)
			{
				Global.mapCode_XuezhanDifu = (int)ConfigSystemParam.GetSystemParamIntByName("ArenaMapCode");
			}
			return Global.Data.roleData.MapCode == Global.mapCode_XuezhanDifu;
		}

		public static bool ToArenaBattleMap(int toMapCode)
		{
			if (Global.mapCode_XuezhanDifu == 0)
			{
				Global.mapCode_XuezhanDifu = (int)ConfigSystemParam.GetSystemParamIntByName("ArenaMapCode");
			}
			return toMapCode == Global.mapCode_XuezhanDifu;
		}

		public static bool IsInWangChengOrHuangGong()
		{
			if (Global.shaChengMapCode == 0)
			{
				Global.shaChengMapCode = (int)ConfigSystemParam.GetSystemParamIntByName("WangChengMapCode");
			}
			if (Global.palaceMapCode == 0)
			{
				Global.palaceMapCode = (int)ConfigSystemParam.GetSystemParamIntByName("PalaceMapCode");
			}
			return Global.Data.roleData.MapCode == Global.shaChengMapCode || Global.Data.roleData.MapCode == Global.palaceMapCode;
		}

		public static bool IsInWangChengOrHuangGongEx(int mapCode)
		{
			if (Global.shaChengMapCode == 0)
			{
				Global.shaChengMapCode = (int)ConfigSystemParam.GetSystemParamIntByName("WangChengMapCode");
			}
			if (Global.palaceMapCode == 0)
			{
				Global.palaceMapCode = (int)ConfigSystemParam.GetSystemParamIntByName("PalaceMapCode");
			}
			return mapCode == Global.shaChengMapCode || mapCode == Global.palaceMapCode;
		}

		public static bool IsInShaChangeBattleTime()
		{
			return true;
		}

		public static bool IsInMingJieMap()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("MingJieMapList", ',');
			if (systemParamIntArrayByName == null)
			{
				return false;
			}
			List<int> list = Enumerable.ToList<int>(systemParamIntArrayByName);
			return list.IndexOf(Global.Data.roleData.MapCode) >= 0;
		}

		public static bool IsMingJieMap(int mapCode)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("MingJieMapList", ',');
			if (systemParamIntArrayByName == null)
			{
				return false;
			}
			List<int> list = Enumerable.ToList<int>(systemParamIntArrayByName);
			return list.IndexOf(mapCode) >= 0;
		}

		public static string GetCurrentMapName()
		{
			return ConfigSettings.GetMapNameByCode(Global.Data.roleData.MapCode, false);
		}

		public static bool IsInGuMuMap()
		{
			List<int> list = new List<int>();
			list = Global.GetGuMuMapList();
			return list.IndexOf(Global.Data.roleData.MapCode) >= 0;
		}

		public static bool IsGuMuMap(int mapCode)
		{
			List<int> list = new List<int>();
			list = Global.GetGuMuMapList();
			return list.IndexOf(mapCode) >= 0;
		}

		public static List<int> GetGuMuMapList()
		{
			if (Global.GuMuMapList != null)
			{
				return Global.GuMuMapList;
			}
			Global.GuMuMapList = new List<int>();
			foreach (string text in ConfigSystemParam.GetSystemParamStringArrayByName("GuMuLevelMapList", ','))
			{
				string[] array = text.Split(new char[]
				{
					'_'
				});
				if (array.Length == 3)
				{
					Global.GuMuMapList.Add(Convert.ToInt32(array[2]));
				}
			}
			return Global.GuMuMapList;
		}

		public static long GetMapLimitTime(int mapCode = -1)
		{
			BufferData bufferData = null;
			int num = -1;
			if (mapCode == -1)
			{
				if (Global.IsInMingJieMap())
				{
					num = 0;
					bufferData = Global.GetBufferDataByID(35);
				}
				else if (Global.IsInGuMuMap())
				{
					num = 1;
					bufferData = Global.GetBufferDataByID(34);
				}
			}
			else if (Global.IsMingJieMap(mapCode))
			{
				num = 0;
				bufferData = Global.GetBufferDataByID(35);
			}
			else if (Global.IsGuMuMap(mapCode))
			{
				num = 1;
				bufferData = Global.GetBufferDataByID(34);
			}
			long result = 0L;
			if (bufferData != null)
			{
				if (num == 0)
				{
					result = Math.Max(0L, bufferData.BufferVal);
				}
				else if (num == 1)
				{
					result = Math.Max(0L, bufferData.BufferVal + (long)bufferData.BufferSecs);
				}
			}
			return result;
		}

		public static bool IsBattleMap()
		{
			if (Global.mapCode_YanhuangZhanchang == 0)
			{
				Global.mapCode_YanhuangZhanchang = (int)ConfigSystemParam.GetSystemParamIntByName("BattleMapCode");
			}
			return Global.Data.roleData.MapCode == Global.mapCode_YanhuangZhanchang || Global.IsInKuafuHuodongYongZheZhanChang() || Global.IsInKuaFuHuoDongWangZhe();
		}

		public static bool ToBattleMap(int toMapCode)
		{
			if (Global.mapCode_YanhuangZhanchang == 0)
			{
				Global.mapCode_YanhuangZhanchang = (int)ConfigSystemParam.GetSystemParamIntByName("BattleMapCode");
			}
			return toMapCode == Global.mapCode_YanhuangZhanchang || Global.IsInKuafuHuodongYongZheZhanChang();
		}

		public static string GetBattleBufferName(int bufferIndex)
		{
			if (bufferIndex < 0 || bufferIndex >= Global.BattleBufferNames.Length)
			{
				return string.Empty;
			}
			return Global.BattleBufferNames[bufferIndex];
		}

		public static int GetBattleBufferNum(int bufferVal)
		{
			if (bufferVal == 20)
			{
				return 0;
			}
			if (bufferVal == 15)
			{
				return 1;
			}
			if (bufferVal == 10)
			{
				return 2;
			}
			return -1;
		}

		public static bool IsAutoFighting()
		{
			return Global.Data.AutoFightData.Fighting;
		}

		public static bool IsMingXiang()
		{
			return Global.Data.MeditateState > 0;
		}

		public static int GetAutoGetThingsFlag(LocalAutoFightData autoFightData)
		{
			int num = 0;
			if (autoFightData.Color_Zi)
			{
				num = Global.SetIntSomeBit(3, num, true);
			}
			if (autoFightData.Color_Lan)
			{
				num = Global.SetIntSomeBit(2, num, true);
			}
			if (autoFightData.Color_Lv)
			{
				num = Global.SetIntSomeBit(1, num, true);
			}
			if (autoFightData.Color_Bai)
			{
				num = Global.SetIntSomeBit(0, num, true);
			}
			if (autoFightData.BaoShi)
			{
				num = Global.SetIntSomeBit(24, num, true);
			}
			if (autoFightData.YuMao)
			{
				num = Global.SetIntSomeBit(25, num, true);
			}
			if (autoFightData.YaoPin)
			{
				num = Global.SetIntSomeBit(26, num, true);
			}
			if (autoFightData.JinBi)
			{
				num = Global.SetIntSomeBit(27, num, true);
			}
			if (autoFightData.MenPiaoCaiLiao)
			{
				num = Global.SetIntSomeBit(28, num, true);
			}
			if (autoFightData.QiTaDaoJu)
			{
				num = Global.SetIntSomeBit(29, num, true);
			}
			autoFightData.AutoPickThingFlags = num;
			return num;
		}

		public static void SetAutoGetThingsValus(LocalAutoFightData autoFightData, int flags)
		{
			autoFightData.AutoPickThingFlags = flags;
			int intSomeBit = Global.GetIntSomeBit(flags, 3);
			autoFightData.Color_Zi = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 2);
			autoFightData.Color_Lan = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 1);
			autoFightData.Color_Lv = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 0);
			autoFightData.Color_Bai = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 24);
			autoFightData.BaoShi = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 25);
			autoFightData.YuMao = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 26);
			autoFightData.YaoPin = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 27);
			autoFightData.JinBi = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 28);
			autoFightData.MenPiaoCaiLiao = (intSomeBit == 1);
			intSomeBit = Global.GetIntSomeBit(flags, 29);
			autoFightData.QiTaDaoJu = (intSomeBit == 1);
		}

		public static bool IsAddPetGoods(int goodsID)
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("PetCardGoodsIDs", true);
			return !string.IsNullOrEmpty(systemParamByName) && systemParamByName.IndexOf(goodsID.ToString()) != -1;
		}

		public static PetData GetPetDataByDbID(int dbID)
		{
			if (Global.Data.PetsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.PetsDataList.Count; i++)
			{
				if (Global.Data.PetsDataList[i].DbID == dbID)
				{
					return Global.Data.PetsDataList[i];
				}
			}
			return null;
		}

		public static int GetPetDataIndexByDbID(int dbID)
		{
			if (Global.Data.PetsDataList == null)
			{
				return -1;
			}
			for (int i = 0; i < Global.Data.PetsDataList.Count; i++)
			{
				if (Global.Data.PetsDataList[i].DbID == dbID)
				{
					return i;
				}
			}
			return -1;
		}

		public static void AddPetData(PetData petData)
		{
			if (Global.Data.PetsDataList == null)
			{
				Global.Data.PetsDataList = new List<PetData>();
			}
			Global.Data.PetsDataList.Add(petData);
		}

		public static void RemovePetData(int dbID)
		{
			if (Global.Data.PetsDataList == null)
			{
				return;
			}
			for (int i = 0; i < Global.Data.PetsDataList.Count; i++)
			{
				if (Global.Data.PetsDataList[i].DbID == dbID)
				{
					Global.Data.PetsDataList.RemoveAt(i);
					break;
				}
			}
		}

		public static void UpdatePetData(PetData petData)
		{
			if (Global.Data.PetsDataList == null)
			{
				Global.Data.PetsDataList = new List<PetData>();
				Global.Data.PetsDataList.Add(petData);
				return;
			}
			for (int i = 0; i < Global.Data.PetsDataList.Count; i++)
			{
				if (Global.Data.PetsDataList[i].DbID == petData.DbID)
				{
					Global.Data.PetsDataList[i] = petData;
					break;
				}
			}
		}

		public static int GetPetLifeV(PetData petData)
		{
			if (petData == null)
			{
				return 0;
			}
			return 100;
		}

		public static bool IsPetNotUseGoods(PetData petData)
		{
			return Global.GetPetLifeV(petData) <= 35;
		}

		public static bool IsPetDead(PetData petData)
		{
			return Global.GetPetLifeV(petData) <= 0;
		}

		public static bool IsMyTeamate(GSprite other)
		{
			RoleData roleData = null;
			return Global.Data.OtherRoles.TryGetValue(other.RoleID, ref roleData) && Global.Data.roleData.TeamID > 0 && Global.Data.roleData.TeamID == roleData.TeamID;
		}

		public static string GetTeamLeaderName()
		{
			if (Global.Data.CurrentTeamData != null)
			{
				for (int i = 0; i < Global.Data.CurrentTeamData.TeamRoles.Count; i++)
				{
					if (Global.Data.CurrentTeamData.LeaderRoleID == Global.Data.CurrentTeamData.TeamRoles[i].RoleID)
					{
						return Global.Data.CurrentTeamData.TeamRoles[i].RoleName;
					}
				}
			}
			return string.Empty;
		}

		public static FriendData FindFriendData(string name)
		{
			if (Global.Data.FriendDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.FriendDataList.Count; i++)
			{
				if (Global.Data.FriendDataList[i].OtherRoleName == name)
				{
					return Global.Data.FriendDataList[i];
				}
			}
			return null;
		}

		private static string GetSpriteTalkText(int extensionID, GSpriteTypes spriteType)
		{
			if (spriteType == GSpriteTypes.NPC)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(extensionID);
				if (npcvobyID != null)
				{
					Random random = new Random();
					string talk = npcvobyID.Talk;
					if (string.Empty != talk.Trim())
					{
						string[] array = talk.Split(new char[]
						{
							'|'
						});
						if (array.Length > 0)
						{
							return array[random.Next(0, array.Length)];
						}
					}
				}
			}
			else if (spriteType == GSpriteTypes.Monster)
			{
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(extensionID);
				if (monsterXmlNodeByID != null)
				{
					Random random2 = new Random();
					string talk2 = monsterXmlNodeByID.Talk;
					if (string.Empty != talk2.Trim())
					{
						string[] array2 = talk2.Split(new char[]
						{
							'|'
						});
						if (array2.Length > 0)
						{
							return array2[random2.Next(0, array2.Length)];
						}
					}
				}
			}
			return string.Empty;
		}

		public static bool SpriteRondomTalk(GSprite sprite, bool showTalk)
		{
			if (Global.Data.SysSetting.HideChatPopupWin)
			{
				return false;
			}
			if (sprite.SpriteType != GSpriteTypes.Monster)
			{
				return false;
			}
			if (sprite.SpriteType == GSpriteTypes.Monster && sprite.VLife <= 0.0)
			{
				return false;
			}
			bool result = false;
			long num = 0L;
			if (Global.Data.TalkDict.ContainsKey(sprite.RoleID))
			{
				Global.Data.TalkDict.TryGetValue(sprite.RoleID, ref num);
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (sprite.VTalk == string.Empty)
			{
				if (correctLocalTime - num >= 60000L)
				{
					Global.Data.TalkDict[sprite.RoleID] = correctLocalTime;
					if (showTalk)
					{
						Random random = new Random();
						if (random.Next(0, 3) == 0)
						{
							sprite.VTalk = Global.GetSpriteTalkText(sprite.ExtensionID, sprite.SpriteType);
							result = true;
						}
					}
				}
			}
			else if (correctLocalTime - num >= 30000L)
			{
				Global.Data.TalkDict[sprite.RoleID] = correctLocalTime;
				sprite.VTalk = string.Empty;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public static void SpriteTalk(GSprite sprite)
		{
			if (Global.Data.SysSetting.HideChatPopupWin)
			{
				return;
			}
			if (sprite.SpriteType != GSpriteTypes.Leader && sprite.SpriteType != GSpriteTypes.Other)
			{
				return;
			}
			if (sprite.VLife <= 0.0)
			{
				return;
			}
			long num = 0L;
			if (Global.Data.TalkDict.ContainsKey(sprite.RoleID))
			{
				Global.Data.TalkDict.TryGetValue(sprite.RoleID, ref num);
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (sprite.VTalk != string.Empty && correctLocalTime - num >= 30000L)
			{
				Global.Data.TalkDict[sprite.RoleID] = correctLocalTime;
				sprite.VTalk = string.Empty;
			}
		}

		public static int GetMinLevelForSendWorldMessage()
		{
			long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("MinLevelForSendWorldMessage");
			if (systemParamIntByName <= 0L)
			{
				return 40;
			}
			return (int)systemParamIntByName;
		}

		public static int GetMinIntervalSecsForSendWorldMessage()
		{
			long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("MinIntervalSecsForSendWorldMessage");
			if (systemParamIntByName < 0L)
			{
				return 0;
			}
			return (int)systemParamIntByName;
		}

		public static void InitBagParams()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("BagGridParams", ',');
			if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 6)
			{
				if (systemParamIntArrayByName[0] > 0)
				{
					Global.MaxPortableGridNum = systemParamIntArrayByName[0];
				}
				if (systemParamIntArrayByName[1] > 0)
				{
					Global.OnePortableGridYuanBao = systemParamIntArrayByName[1];
					Global.OneCangKuGridYuanBao = Global.OnePortableGridYuanBao;
				}
				if (systemParamIntArrayByName[2] > 0)
				{
					Global.DefaultPortableGridNum = systemParamIntArrayByName[2];
				}
				if (systemParamIntArrayByName[3] > 0)
				{
					Global.MaxBagGridNum = systemParamIntArrayByName[3];
				}
				if (systemParamIntArrayByName[4] > 0)
				{
					Global.OneBagGridYuanBao = systemParamIntArrayByName[4];
				}
				if (systemParamIntArrayByName[5] > 0)
				{
					Global.DefaultBagGridNum = systemParamIntArrayByName[5];
				}
			}
		}

		public static int GetTimeForGrid(int pos, BaoGuoMode mode)
		{
			if (mode == BaoGuoMode.None)
			{
				return (pos - Global.DefaultBagGridNum) * 3000;
			}
			if (mode == BaoGuoMode.CangKu)
			{
				return (pos - Global.DefaultPortableGridNum) * 1500;
			}
			if (mode == BaoGuoMode.horseCangKu)
			{
				return (pos - Global.DefaultRebornBagGridNum) * 3000;
			}
			if (mode == BaoGuoMode.ChongShengCangKu)
			{
				return (pos - Global.DefaultRebornPortableGridNum) * 1500;
			}
			return 864000;
		}

		public static int GetExtBagGridNeedYuanBao(int addNum, int onlineExtTime, BaoGuoMode mode, out int leftTime)
		{
			leftTime = 86400;
			int num = 0;
			int num2 = Global.DefaultBagGridNum;
			int nOneBagGridYuanBao = Global.OneBagGridYuanBao;
			double num3 = 0.0;
			if (mode == BaoGuoMode.None)
			{
				num = Global.GetSelfBagCapacity();
				num2 = Global.DefaultBagGridNum;
				nOneBagGridYuanBao = Global.OneBagGridYuanBao;
				num3 = (double)onlineExtTime / (double)((num + 1 - num2) * 3000);
				leftTime = (int)((double)((num + 1 - num2) * 3000) * (1.0 - num3));
			}
			else if (mode == BaoGuoMode.CangKu)
			{
				num = Global.GetPortableBagCapacity();
				num2 = Global.DefaultPortableGridNum;
				nOneBagGridYuanBao = Global.OneCangKuGridYuanBao;
				num3 = (double)onlineExtTime / (double)((num + 1 - num2) * 1500);
				leftTime = (int)((double)((num + 1 - num2) * 1500) * (1.0 - num3));
			}
			int num4 = num + 1;
			int num5 = num + addNum;
			int num6 = 0;
			for (int i = num4; i <= num5; i++)
			{
				num6 += Global.GetOneBagGridExtendNeedYuanBao(i, num2, nOneBagGridYuanBao);
				if (i == num4)
				{
					num6 = (int)((double)num6 * Math.Max(0.0, 1.0 - num3));
					leftTime = (int)((double)Global.GetTimeForGrid(i, mode) * Math.Max(0.0, 1.0 - num3));
				}
				else
				{
					leftTime += Global.GetTimeForGrid(i, mode);
				}
			}
			return num6;
		}

		public static int GetRebornBagGridNeedYuanBao(int addNum, BaoGuoMode mode)
		{
			int num = 0;
			int num2 = Global.DefaultBagGridNum;
			int num3 = 0;
			int num4 = 0;
			if (mode == BaoGuoMode.horseCangKu)
			{
				num = Global.GetSelfRebornBagCapacity();
				num2 = Global.DefaultBagGridNum;
				num3 = ChongShengData.GetNeedOpenBagMoney();
				num4 = ChongShengData.GetNeedOpenBagMoneyMax();
			}
			else if (mode == BaoGuoMode.ChongShengCangKu)
			{
				num = Global.GetRebornPortableBagCapacity();
				num2 = Global.DefaultRebornPortableGridNum;
				num3 = ChongShengData.GetNeedOpenPortableMoney();
				num4 = ChongShengData.GetNeedOpenPortableMoneyMax();
			}
			int num5 = 0;
			int num6 = num - num2 + 1;
			for (int i = 0; i < addNum; i++)
			{
				int num7 = (i + num6) * num3;
				if (num7 > num4)
				{
					num7 = num4;
				}
				num5 += num7;
			}
			return num5;
		}

		public static int GetOneBagGridExtendNeedYuanBao(int extendPos, int DefaultGridNum, int nOneBagGridYuanBao)
		{
			int num = (extendPos - DefaultGridNum) * nOneBagGridYuanBao;
			if (num > 10 * nOneBagGridYuanBao)
			{
				num = 10 * nOneBagGridYuanBao;
			}
			return num;
		}

		public static void AutoExpandBagGrid()
		{
			if (Global.Data.roleData.BagNum < Global.MaxBagGridNum)
			{
				int num = (int)Global.Data.OnLineTimeBagGrid + Global.Data.roleData.OpenGridTime;
				if (num >= Global.GetTimeForGrid(Global.GetSelfBagCapacity() + 1, BaoGuoMode.None))
				{
					GameInstance.Game.SpriteExtBagNumByYuanBao(1.0, 1, 0);
				}
			}
			if (Global.Data.roleData.MyPortableBagData.ExtGridNum < 216)
			{
				int num2 = (int)Global.Data.OnLineTimePortableGrid + Global.Data.roleData.OpenPortableGridTime;
				if (num2 >= Global.GetTimeForGrid(Global.GetPortableBagCapacity() + 1, BaoGuoMode.CangKu))
				{
					GameInstance.Game.SpriteExtGridByYuanBao(1.0, 1, 0);
				}
			}
		}

		public static void MoveGoodsToParcel(GoodsData gd, int newBagIndex = 0)
		{
			if (Global.CanAddGoods(gd.GoodsID, gd.GCount, gd.Binding, gd.Endtime, false))
			{
				if (gd.Site == -1000 || gd.Site == 2000 || gd.Site == 4000 || gd.Site == 12000)
				{
					gd.BagIndex = newBagIndex;
					gd.Site = 0;
					GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("背包已满，请先清理出空闲位置后，再存放物品..."), new object[0]), 1, -1, -1, 0);
			}
		}

		public static void MoveGoodsToPortableBag(GoodsData gd, int newBagIndex = 0)
		{
			if (gd.Site == 0)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (categoriy != 50)
					{
						if (categoriy != 302)
						{
							if (Global.CanPortableAddGoods((Global.Data.PortableGoodsDataList != null) ? Global.Data.PortableGoodsDataList.Count : 0))
							{
								gd.BagIndex = newBagIndex;
								gd.Site = -1000;
								GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
							}
							else
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("随身仓库空闲的格子不足，请先扩展空格后，再放入物品"), new object[0]), 0, -1, -1, 0);
							}
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("升级礼包，无法放入随身仓库中..."), new object[0]), 0, -1, -1, 0);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("任务道具，无法放入随身仓库中..."), new object[0]), 0, -1, -1, 0);
					}
				}
			}
		}

		public static void MoveGoodsToChongShengParcel(GoodsData gd, int newBagIndex = 0)
		{
			if (Global.CanAddGoods(gd.GoodsID, gd.GCount, gd.Binding, gd.Endtime, false))
			{
				if (gd.Site == 15001)
				{
					gd.BagIndex = newBagIndex;
					gd.Site = 15000;
					GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
				}
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("重生背包已满，请先清理出空闲位置后，再存放物品..."), new object[0]), 1, -1, -1, 0);
			}
		}

		public static void MoveGoodsToPortableChongShengBag(GoodsData gd, int newBagIndex = 0)
		{
			if (gd.Site == 15000)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (Global.IsRebornGood(goodsXmlNodeByID))
					{
						if (Global.CanPortableChongShengAddGoods((Global.Data.roleData.RebornGoodsStoreList != null) ? Global.Data.roleData.RebornGoodsStoreList.Count : 0))
						{
							gd.BagIndex = newBagIndex;
							gd.Site = 15001;
							GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("重生仓库空闲的格子不足，请先扩展空格后，再放入物品"), new object[0]), 0, -1, -1, 0);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("非重生道具"), new object[0]), 0, -1, -1, 0);
					}
				}
			}
		}

		public static void MoveGoodsToJinDanBag(GoodsData gd, int newBagIndex = 0)
		{
			if (gd.Site == 0)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				if (goodsXmlNodeByID != null)
				{
					int categoriy = goodsXmlNodeByID.Categoriy;
					if (categoriy != 50)
					{
						if (categoriy != 302)
						{
							if (Global.CanPortableAddGoods((Global.Data.PortableGoodsDataList != null) ? Global.Data.PortableGoodsDataList.Count : 0))
							{
								gd.BagIndex = newBagIndex;
								gd.Site = 2000;
								GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, gd.Site, gd.GCount, gd.BagIndex, string.Empty);
							}
							else
							{
								GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("金蛋仓库空闲的格子不足，请先移动到包裹再操作"), new object[0]), 0, -1, -1, 0);
							}
						}
						else
						{
							GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("升级礼包，无法放入随身仓库中..."), new object[0]), 0, -1, -1, 0);
						}
					}
					else
					{
						GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("任务道具，无法放入随身仓库中..."), new object[0]), 0, -1, -1, 0);
					}
				}
			}
		}

		public static GoodsData GetPortableGoodsDataByDbID(int id, bool beChongSheng = false)
		{
			List<GoodsData> list;
			if (beChongSheng)
			{
				list = Global.Data.roleData.RebornGoodsStoreList;
			}
			else
			{
				list = Global.Data.PortableGoodsDataList;
			}
			if (list == null)
			{
				return null;
			}
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Id == id)
					{
						return list[i];
					}
				}
			}
			return null;
		}

		public static void AddPortableGoodsData(GoodsData goodsData)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			bool flag = Global.IsRebornGood(goodsXmlNodeByID);
			List<GoodsData> list;
			if (flag)
			{
				list = Global.Data.roleData.RebornGoodsStoreList;
				if (goodsData.Site != 15001)
				{
					return;
				}
			}
			else
			{
				list = Global.Data.PortableGoodsDataList;
				if (goodsData.Site != -1000)
				{
					return;
				}
			}
			if (list == null)
			{
				list = new List<GoodsData>();
				if (flag)
				{
					Global.Data.roleData.RebornGoodsStoreList = list;
				}
				else
				{
					Global.Data.PortableGoodsDataList = list;
				}
			}
			list.Add(goodsData);
		}

		public static void RemovePortableGoodsData(GoodsData goodsData)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			bool flag = Global.IsRebornGood(goodsXmlNodeByID);
			List<GoodsData> list;
			if (flag)
			{
				list = Global.Data.roleData.RebornGoodsStoreList;
			}
			else
			{
				list = Global.Data.PortableGoodsDataList;
			}
			if (list == null)
			{
				return;
			}
			int num = list.IndexOf(goodsData);
			if (num >= 0)
			{
				list.RemoveAt(num);
			}
		}

		public static bool CanPortableAddGoods(int usedGridNum)
		{
			int portableBagCapacity = Global.GetPortableBagCapacity();
			return usedGridNum < portableBagCapacity;
		}

		public static bool CanPortableChongShengAddGoods(int usedGridNum)
		{
			int rebornPortableBagCapacity = Global.GetRebornPortableBagCapacity();
			return usedGridNum < rebornPortableBagCapacity;
		}

		public static bool IsExtGridGoods(int goodsID)
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("PetAddGridGoodsIDs", true);
			return !string.IsNullOrEmpty(systemParamByName) && systemParamByName.IndexOf(goodsID.ToString()) != -1;
		}

		public static bool CanExtPetGrid(int goodsID)
		{
			if (!Global.IsExtGridGoods(goodsID))
			{
				return true;
			}
			int portableBagCapacity = Global.GetPortableBagCapacity();
			int num = 0;
			if (goodsID == 504001)
			{
				num = 1;
			}
			else if (goodsID == 504002)
			{
				num = 5;
			}
			else if (goodsID == 504003)
			{
				num = 10;
			}
			if (portableBagCapacity + num >= Global.MaxPortableGridNum)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang("随身仓库最大的格子容量限制为{0}个，无法再扩展"), Global.MaxPortableGridNum), 0, -1, -1, 0);
				return false;
			}
			return true;
		}

		public static int GetPortableBagCapacity()
		{
			if (Global.Data.roleData.MyPortableBagData.ExtGridNum > Global.MaxPortableGridNum)
			{
				return Global.MaxPortableGridNum;
			}
			return Global.Data.roleData.MyPortableBagData.ExtGridNum;
		}

		public static int GetRebornPortableBagCapacity()
		{
			if (Global.Data.roleData.RebornGirdData.ExtGridNum > Global.MaxRebornPortableGridNum)
			{
				return Global.MaxRebornPortableGridNum;
			}
			return Global.Data.roleData.RebornGirdData.ExtGridNum;
		}

		public static int GetSelfBagCapacity()
		{
			return Global.Data.roleData.BagNum;
		}

		public static int GetSelfRebornBagCapacity()
		{
			return Global.Data.roleData.RebornBagNum;
		}

		public static int GetJinDanBagCapacity()
		{
			return Global.MaxJinDanGridNum;
		}

		public static bool CanJinDanAddGoods(int usedGridNum)
		{
			int jinDanBagCapacity = Global.GetJinDanBagCapacity();
			return usedGridNum < jinDanBagCapacity;
		}

		public static GoodsData GetJinDanGoodsDataByDbID(int id)
		{
			if (Global.Data.JinDanGoodsDataList == null)
			{
				return null;
			}
			if (Global.Data.JinDanGoodsDataList != null)
			{
				for (int i = 0; i < Enumerable.Count<GoodsData>(Global.Data.JinDanGoodsDataList); i++)
				{
					if (Global.Data.JinDanGoodsDataList[i].Id == id)
					{
						return Global.Data.JinDanGoodsDataList[i];
					}
				}
			}
			return null;
		}

		public static GoodsData GetJingLingPrepareGoodsDataByDbID(int id)
		{
			if (Global.Data.equipPet == null)
			{
				return null;
			}
			for (int i = 0; i < Enumerable.Count<GoodsData>(Global.Data.equipPet); i++)
			{
				if (Global.Data.equipPet[i].Id == id)
				{
					return Global.Data.equipPet[i];
				}
			}
			return null;
		}

		public static void RemoveJingLingPrepareGoodsData(GoodsData goodsData)
		{
			if (Global.Data.equipPet == null)
			{
				return;
			}
			int num = Global.Data.equipPet.IndexOf(goodsData);
			if (num >= 0)
			{
				Global.Data.equipPet.RemoveAt(num);
			}
		}

		public static void AddJinDanGoodsData(GoodsData goodsData)
		{
			if (goodsData.Site != 2000)
			{
				return;
			}
			if (Global.Data.JinDanGoodsDataList == null)
			{
				Global.Data.JinDanGoodsDataList = new List<GoodsData>();
			}
			Global.Data.JinDanGoodsDataList.Add(goodsData);
		}

		public static void RemoveJinDanGoodsData(GoodsData goodsData)
		{
			if (Global.Data.JinDanGoodsDataList == null)
			{
				return;
			}
			int num = Global.Data.JinDanGoodsDataList.IndexOf(goodsData);
			if (num >= 0)
			{
				Global.Data.JinDanGoodsDataList.RemoveAt(num);
			}
		}

		public static void AddYuansuGoodsData(GoodsData goodsData, bool isUse = false)
		{
			List<GoodsData> list = Global.GetYuansuGoodsDataList(isUse);
			if (list == null)
			{
				list = new List<GoodsData>();
			}
			list.Add(goodsData);
		}

		public static void RemoveYuansuGoodsData(GoodsData goodsData, bool isUse = false)
		{
			List<GoodsData> yuansuGoodsDataList = Global.GetYuansuGoodsDataList(isUse);
			if (yuansuGoodsDataList == null)
			{
				return;
			}
			int num = yuansuGoodsDataList.IndexOf(goodsData);
			if (num >= 0)
			{
				yuansuGoodsDataList.RemoveAt(num);
			}
		}

		public static int GetYuansuGoodsDataLevel(GoodsData goodsData)
		{
			int result = 1;
			if (goodsData.ElementhrtsProps != null && goodsData.ElementhrtsProps.Count >= 2)
			{
				result = goodsData.ElementhrtsProps[0];
			}
			return result;
		}

		public static void GetYuansuGoodsDataLevelAndExp(GoodsData goodsData, out int level, out int exp)
		{
			level = 1;
			exp = 0;
			if (goodsData.ElementhrtsProps != null && goodsData.ElementhrtsProps.Count >= 2)
			{
				level = goodsData.ElementhrtsProps[0];
				exp = goodsData.ElementhrtsProps[1];
			}
		}

		public static GoodsData GetYuansuGoodsDataByDbID(int dbID, List<GoodsData> goodsDataList = null, bool isUse = false)
		{
			if (goodsDataList == null)
			{
				goodsDataList = Global.GetYuansuGoodsDataList(isUse);
			}
			if (goodsDataList == null)
			{
				return null;
			}
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (goodsDataList[i].Id == dbID)
				{
					return goodsDataList[i];
				}
			}
			return null;
		}

		public static void ReplaceYuansuGoodsData(GoodsData gd, List<GoodsData> goodsDataList = null, bool isUse = false)
		{
			if (gd == null)
			{
				return;
			}
			if (goodsDataList == null)
			{
				goodsDataList = Global.GetYuansuGoodsDataList(isUse);
			}
			int id = gd.Id;
			if (goodsDataList == null)
			{
				return;
			}
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (goodsDataList[i].Id == id)
				{
					goodsDataList[i] = gd;
				}
			}
		}

		public static List<GoodsData> GetYuansuGoodsDataList(bool isUse = false)
		{
			List<GoodsData> result;
			if (!isUse)
			{
				if (Global.Data.YuansuGoodsDataList == null)
				{
					Global.Data.YuansuGoodsDataList = new List<GoodsData>();
				}
				result = Global.Data.YuansuGoodsDataList;
			}
			else
			{
				if (Global.Data.YuansuGoodsDataListByUseing == null)
				{
					Global.Data.YuansuGoodsDataListByUseing = new List<GoodsData>();
				}
				result = Global.Data.YuansuGoodsDataListByUseing;
			}
			return result;
		}

		public static void SetGoodsDataYuansuProps(GoodsData gd, int level, int exp)
		{
			if (gd == null)
			{
				return;
			}
			List<int> list = new List<int>();
			list.Add(level);
			list.Add(exp);
			gd.ElementhrtsProps = list;
		}

		public static int GetYuansuBagEmptyGridCount()
		{
			List<GoodsData> yuansuGoodsDataList = Global.GetYuansuGoodsDataList(false);
			if (yuansuGoodsDataList == null)
			{
				return Global.YuansuBagMaxGridCount;
			}
			int count = yuansuGoodsDataList.Count;
			int num = Global.YuansuBagMaxGridCount - count;
			return (num < 0) ? 0 : num;
		}

		public static void PushGoodsDataToTempQueue(GoodsData gd)
		{
			if (Global.TempYuansuGoodsDataQueue == null)
			{
				Global.TempYuansuGoodsDataQueue = new Queue<GoodsData>();
			}
			if (gd == null)
			{
				return;
			}
			Global.TempYuansuGoodsDataQueue.Enqueue(gd);
		}

		public static GoodsData PopGoodsDataFromTempQueue()
		{
			if (Global.TempYuansuGoodsDataQueue == null || Global.TempYuansuGoodsDataQueue.Count <= 0)
			{
				return null;
			}
			return Global.TempYuansuGoodsDataQueue.Dequeue();
		}

		public static int GetSumOfAllUsedYuansuLevel()
		{
			int num = 0;
			List<GoodsData> yuansuGoodsDataList = Global.GetYuansuGoodsDataList(true);
			if (yuansuGoodsDataList == null || yuansuGoodsDataList.Count <= 0)
			{
				return num;
			}
			for (int i = 0; i < yuansuGoodsDataList.Count; i++)
			{
				num += Global.GetYuansuGoodsDataLevel(yuansuGoodsDataList[i]);
			}
			return num;
		}

		public static int GetJingLingBagCapacity()
		{
			return Global.MaxJingLingGridNum;
		}

		public static bool CanJingLingAddGoods(int usedGridNum)
		{
			int jingLingBagCapacity = Global.GetJingLingBagCapacity();
			return usedGridNum < jingLingBagCapacity;
		}

		public static GoodsData GetJingLingGoodsDataByDbID(int id)
		{
			if (Global.Data.JingLingGoodsDataList == null)
			{
				return null;
			}
			if (Global.Data.JingLingGoodsDataList != null)
			{
				for (int i = 0; i < Enumerable.Count<GoodsData>(Global.Data.JingLingGoodsDataList); i++)
				{
					if (Global.Data.JingLingGoodsDataList[i].Id == id)
					{
						return Global.Data.JingLingGoodsDataList[i];
					}
				}
			}
			return null;
		}

		public static void AddJingLingGoodsData(GoodsData goodsData)
		{
			if (goodsData.Site != 4000)
			{
				return;
			}
			if (Global.Data.JingLingGoodsDataList == null)
			{
				Global.Data.JingLingGoodsDataList = new List<GoodsData>();
			}
			Global.Data.JingLingGoodsDataList.Add(goodsData);
		}

		public static void RemoveJingLingGoodsData(GoodsData goodsData)
		{
			if (Global.Data.JingLingGoodsDataList == null)
			{
				return;
			}
			int num = Global.Data.JingLingGoodsDataList.IndexOf(goodsData);
			if (num >= 0)
			{
				Global.Data.JingLingGoodsDataList.RemoveAt(num);
			}
		}

		public static int GetWuxueJingjieBuffID(int index)
		{
			if (index > 0)
			{
				if (Global.WuxueBuffIDs == null)
				{
					Global.WuxueBuffIDs = ConfigSystemParam.GetSystemParamIntArrayByName("WuXueBufferGoodsIDs", ',');
				}
				return Global.WuxueBuffIDs[index - 1];
			}
			return 0;
		}

		public static float GetFuWenLv()
		{
			if (Global.Data.ChengjiuFuWen == null)
			{
				return 0f;
			}
			if (Global.Data.ChengjiuFuWen.RuneID >= 7)
			{
				return (float)Global.Data.ChengjiuFuWen.RuneID - 1f;
			}
			XElement gameResXml = Global.GetGameResXml("Config/ChengJiuFuWen.Xml");
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "ChengJiuFuWen", "ID", Global.Data.ChengjiuFuWen.RuneID.ToString());
			if (xelementList == null)
			{
				return 0f;
			}
			XElement xelement = xelementList[0];
			float num = float.Parse(Global.GetXElementAttributeStr(xelement, "Dodge"));
			float num2 = float.Parse(Global.GetXElementAttributeStr(xelement, "AddDefense"));
			float num3 = float.Parse(Global.GetXElementAttributeStr(xelement, "AddAttack"));
			float num4 = float.Parse(Global.GetXElementAttributeStr(xelement, "LifeV"));
			if (num == 0f || num2 == 0f || num3 == 0f || num4 == 0f)
			{
				return 0f;
			}
			float num5 = (float)Global.Data.ChengjiuFuWen.DodgeAdd / num * 0.25f;
			float num6 = (float)Global.Data.ChengjiuFuWen.DefenseAdd / num2 * 0.25f;
			float num7 = (float)Global.Data.ChengjiuFuWen.AttackAdd / num3 * 0.25f;
			float num8 = (float)Global.Data.ChengjiuFuWen.LifeAdd / num4 * 0.25f;
			float num9 = num5 + num6 + num7 + num8;
			float num10 = (float)(Global.Data.ChengjiuFuWen.RuneID - 1);
			if (Global.Data.ChengjiuFuWen.UpResultType == 3)
			{
				num10 = 6f;
				num9 = 0f;
			}
			num10 += num9;
			return num10;
		}

		public static float GetShengWangXunZhangLv()
		{
			if (Global.Data.adendaData == null)
			{
				return 0f;
			}
			if (Global.Data.adendaData.MedalID >= 7)
			{
				return (float)Global.Data.adendaData.MedalID - 1f;
			}
			XElement gameResXml = Global.GetGameResXml("Config/ShengWangXunZhang.xml");
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "ShengWangXunZhang", "ID", Global.Data.adendaData.MedalID.ToString());
			if (xelementList == null)
			{
				return 0f;
			}
			XElement xelement = xelementList[0];
			float num = float.Parse(Global.GetXElementAttributeStr(xelement, "HitV"));
			float num2 = float.Parse(Global.GetXElementAttributeStr(xelement, "AddDefense"));
			float num3 = float.Parse(Global.GetXElementAttributeStr(xelement, "AddAttack"));
			float num4 = float.Parse(Global.GetXElementAttributeStr(xelement, "LifeV"));
			if (num == 0f || num2 == 0f || num3 == 0f || num4 == 0f)
			{
				return 0f;
			}
			float num5 = (float)Global.Data.adendaData.HitAdd / num * 0.25f;
			float num6 = (float)Global.Data.adendaData.DefenseAdd / num2 * 0.25f;
			float num7 = (float)Global.Data.adendaData.AttackAdd / num3 * 0.25f;
			float num8 = (float)Global.Data.adendaData.LifeAdd / num4 * 0.25f;
			float num9 = num5 + num6 + num7 + num8;
			float num10 = (float)(Global.Data.adendaData.MedalID - 1);
			if (Global.Data.adendaData.UpResultType == 3)
			{
				num10 = 6f;
				num9 = 0f;
			}
			num10 += num9;
			return num10;
		}

		public static string GetWuXueName(int level)
		{
			int bufferGoodsID = Global.GetBufferGoodsID(33, level - 1);
			if (bufferGoodsID > 0)
			{
				return Global.GetGoodsNameByID(bufferGoodsID, false);
			}
			return Global.GetLang("无");
		}

		public static bool CanActiveNextWuXueLevel()
		{
			int num = 1 + Global.GetRoleCommonUseParamsValue(8);
			if (num >= 10)
			{
				return false;
			}
			if (Global._CacheNextWuXueLevel != num)
			{
				Global._CacheNextWuXueLevel = num;
				Global._CacheNextWuXueLevelDayXiaoHao = 1.0;
				XElement gameResXml = Global.GetGameResXml("Config/WuXue.Xml");
				if (gameResXml == null)
				{
					return false;
				}
				XElement xelement = Global.GetXElement(gameResXml, "WuXue", "ID", num.ToString());
				if (xelement == null)
				{
					return false;
				}
				Global._CacheNextWuXueLevelNeedRoleLevel = Global.GetXElementAttributeInt(xelement, "LevelLimit");
				Global._CacheNextWuXueLevelNeedWuXin = Global.GetXElementAttributeInt(xelement, "WuXing");
				Global._CacheNextWuXueLevelNeedYinLiang = Global.GetXElementAttributeInt(xelement, "NeedMoney");
				Global._CacheNextWuXueLevelDayXiaoHao = Global.GetXElementAttributeDouble(xelement, "DayXiaoHao");
			}
			return Global._CacheNextWuXueLevelDayXiaoHao <= 0.0 && Global.Data.roleData.Level >= Global._CacheNextWuXueLevelNeedRoleLevel && Global.GetRoleCommonUseParamsValue(3) >= Global._CacheNextWuXueLevelNeedWuXin;
		}

		public static string GetJingMaiName(int jingMaiID)
		{
			if (jingMaiID < 0 || jingMaiID >= 8)
			{
				return string.Empty;
			}
			return Global.JingMaiNames[jingMaiID];
		}

		public static string GetJingMaiBodyLevelName(int jingMaiBodyLevel)
		{
			if (jingMaiBodyLevel < 0 || jingMaiBodyLevel >= Global.JingMaiBodyLevelNames.Length)
			{
				return string.Empty;
			}
			return Global.GetLang(Global.JingMaiBodyLevelNames[jingMaiBodyLevel]);
		}

		public static string GetJingMaiNameNew(int level)
		{
			int bufferGoodsID = Global.GetBufferGoodsID(32, level - 1);
			if (bufferGoodsID > 0)
			{
				return Global.GetGoodsNameByID(bufferGoodsID, false);
			}
			return Global.GetLang("无");
		}

		public static string GetJingMaiBodyLevelPicName(int jingMaiBodyLevel)
		{
			if (jingMaiBodyLevel < 0 || jingMaiBodyLevel >= Global.JingMaiBodyLevelPicNames.Length)
			{
				return string.Empty;
			}
			return Global.GetLang(Global.JingMaiBodyLevelPicNames[jingMaiBodyLevel]);
		}

		public static XElement GetJingMaiXmlItem(int jingMaiID, int occupation, int level)
		{
			string jingMaiName = Global.GetJingMaiName(jingMaiID);
			if (string.Empty == jingMaiName)
			{
				return null;
			}
			XElement xelement = null;
			string text = string.Format("{0}_{1}_{2}", jingMaiID, occupation, level);
			if (Global.JingMaiXueWeiDict.ContainsKey(text))
			{
				Global.JingMaiXueWeiDict.TryGetValue(text, ref xelement);
				return xelement;
			}
			string text2 = string.Format("Config/JingMais/{0}.Xml", occupation);
			XElement gameResXml = Global.GetGameResXml(text2);
			if (gameResXml == null)
			{
				GError.AddErrMsg(string.Format(Global.GetLang("加载经脉配置文件失败: {0}"), text2));
				return null;
			}
			XElement xelement2 = Global.GetXElement(gameResXml, "JingMai", "ID", jingMaiID.ToString());
			if (xelement2 == null)
			{
				return null;
			}
			xelement = Global.GetXElement(xelement2, "Chong", "ID", level.ToString());
			if (xelement != null)
			{
				Global.JingMaiXueWeiDict[text] = xelement;
			}
			return xelement;
		}

		public static int GetJingMaiDbIDByJMID(int jingMaiBodyLevel, int jingMaiID, List<JingMaiData> jingMaiDataList)
		{
			if (jingMaiDataList == null)
			{
				return -1;
			}
			for (int i = 0; i < jingMaiDataList.Count; i++)
			{
				if (jingMaiDataList[i].JingMaiID == jingMaiID && jingMaiDataList[i].JingMaiBodyLevel == jingMaiBodyLevel)
				{
					return jingMaiDataList[i].DbID;
				}
			}
			return -1;
		}

		public static JingMaiData GetJingMaiDataByJMID(int jingMaiBodyLevel, int jingMaiID, List<JingMaiData> jingMaiDataList)
		{
			if (jingMaiDataList == null)
			{
				return null;
			}
			for (int i = 0; i < jingMaiDataList.Count; i++)
			{
				if (jingMaiDataList[i].JingMaiID == jingMaiID && jingMaiDataList[i].JingMaiBodyLevel == jingMaiBodyLevel)
				{
					return jingMaiDataList[i];
				}
			}
			return null;
		}

		public static JingMaiData GetJingMaiDataByDbID(int dbID, List<JingMaiData> jingMaiDataList)
		{
			if (jingMaiDataList == null)
			{
				return null;
			}
			for (int i = 0; i < jingMaiDataList.Count; i++)
			{
				if (jingMaiDataList[i].DbID == dbID)
				{
					return jingMaiDataList[i];
				}
			}
			return null;
		}

		public static int CalcJingMaiXueWeiNum(List<JingMaiData> jingMaiDataList)
		{
			if (jingMaiDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < jingMaiDataList.Count; i++)
			{
				num += jingMaiDataList[i].JingMaiLevel;
			}
			return num;
		}

		public static int CalcJingMaiOkNum(List<JingMaiData> jingMaiDataList)
		{
			if (jingMaiDataList == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < jingMaiDataList.Count; i++)
			{
				if (jingMaiDataList[i].JingMaiLevel >= Global.MaxJingMaiLevel)
				{
					num++;
				}
			}
			return num;
		}

		public static void CalcJingMaiBodyLevel(List<JingMaiData> jingMaiDataList)
		{
			int num = Global.CalcJingMaiXueWeiNum(jingMaiDataList);
			Global.Data.roleData.JingMaiBodyLevel = num / (Global.MaxJingMaiLevel * 8) + 1;
		}

		public static int GetJingMaiUpDecoIDByIndex(int index)
		{
			if (Global.Data.JingMaiUpDecoIDs == null)
			{
				Global.Data.JingMaiUpDecoIDs = ConfigSystemParam.GetSystemParamIntArrayByName("JingMaiLevelDecoIDs", ',');
			}
			if (index < 0 || index >= Global.Data.JingMaiUpDecoIDs.Length)
			{
				return -1;
			}
			return Global.Data.JingMaiUpDecoIDs[index];
		}

		public static int GetJingMaiUpDecoID(RoleData roleData)
		{
			int index = -1;
			if (roleData.JingMaiOkNum > 0)
			{
				index = roleData.JingMaiOkNum / 8;
			}
			return Global.GetJingMaiUpDecoIDByIndex(index);
		}

		public static void AddSpriteJingMaiUpDeco(GSprite sprite, RoleData roleData)
		{
		}

		public static int GetDailyJingMaiNum()
		{
			if (Global.Data.roleData.MyDailyJingMaiData == null)
			{
				return 0;
			}
			string text = Global.GetCorrectDateTime().ToString("yyyy-MM-dd");
			DailyJingMaiData myDailyJingMaiData = Global.Data.roleData.MyDailyJingMaiData;
			if (myDailyJingMaiData.JmTime != text)
			{
				return 0;
			}
			return myDailyJingMaiData.JmNum;
		}

		public static int TodayChongXueNum()
		{
			int dailyJingMaiNum = Global.GetDailyJingMaiNum();
			return Global.MaxDailyJingMaiNum - dailyJingMaiNum;
		}

		public static bool TodayCanChongXue()
		{
			return Global.TodayChongXueNum() > 0;
		}

		public static bool CanActiveNextJingMaiLevel()
		{
			int num = 1 + Global.GetRoleCommonUseParamsValue(7);
			if (num > ConfigSystemParam.GetSystemParamIntArrayByName("JingMaiBufferGoodsIDs", ',').Length)
			{
				return false;
			}
			if (Global._CacheNextJingMaiLevel != num)
			{
				Global._CacheNextJingMaiLevel = num;
				Global._CacheNextJingMaiLevelNeedRoleLevel = 16777215;
				XElement gameResXml = Global.GetGameResXml("Config/JingMai.Xml");
				if (gameResXml == null)
				{
					return false;
				}
				XElement xelement = Global.GetXElement(gameResXml, "JinMai", "ID", num.ToString());
				if (xelement == null)
				{
					return false;
				}
				Global._CacheNextJingMaiLevelNeedRoleLevel = Global.GetXElementAttributeInt(xelement, "LevelLimit");
				Global._CacheNextJingMaiLevelNeedZhenQi = Global.GetXElementAttributeInt(xelement, "ZhenQi");
				Global._CacheNextJingMaiLevelNeedTongQian = Global.GetXElementAttributeInt(xelement, "BindMoney");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				if (xelementAttributeStr.Split(new char[]
				{
					','
				}).Length == 2)
				{
					Global._CacheNextJingMaiLevelNeedGoods = Convert.ToInt32(xelementAttributeStr.Split(new char[]
					{
						','
					})[0]);
					Global._CacheNextJingMaiLevelNeedGoodsNum = Convert.ToInt32(xelementAttributeStr.Split(new char[]
					{
						','
					})[1]);
				}
			}
			return Global.Data.roleData.Money1 >= Global._CacheNextJingMaiLevelNeedTongQian && Global.Data.roleData.Level >= Global._CacheNextJingMaiLevelNeedRoleLevel && Global.GetRoleCommonUseParamsValue(4) >= Global._CacheNextJingMaiLevelNeedZhenQi && (Global._CacheNextJingMaiLevelNeedGoods <= 0 || Global.GetTotalGoodsCountByID(Global._CacheNextJingMaiLevelNeedGoods) >= Global._CacheNextJingMaiLevelNeedGoodsNum);
		}

		public static int GetZhanhunBuffID(int index)
		{
			if (index > 0)
			{
				if (Global.ZhanhunBuffIDs == null)
				{
					Global.ZhanhunBuffIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ZhanhunBufferGoodsIDs", ',');
				}
				return Global.ZhanhunBuffIDs[index - 1];
			}
			return 0;
		}

		public static string GetZhanhunName(int level)
		{
			int bufferGoodsID = Global.GetBufferGoodsID(50, level - 1);
			if (bufferGoodsID > 0)
			{
				return Global.GetGoodsNameByID(bufferGoodsID, false);
			}
			return Global.GetLang("无");
		}

		public static int GetRongyaoBuffID(int index)
		{
			if (index > 0)
			{
				if (Global.RongyaoBuffIDs == null)
				{
					Global.RongyaoBuffIDs = ConfigSystemParam.GetSystemParamIntArrayByName("RongyaoBufferGoodsIDs", ',');
				}
				return Global.RongyaoBuffIDs[index - 1];
			}
			return 0;
		}

		public static void InitFilterFields()
		{
			try
			{
				if (Global.FilterFieldsDict == null)
				{
					Global.FilterFieldsDict = new Dictionary<string, List<string>>();
					string gameResTxt = Global.GetGameResTxt(string.Format("filterwords", new object[0]));
					if (!string.IsNullOrEmpty(gameResTxt))
					{
						string[] array = gameResTxt.Split(new char[]
						{
							'；'
						});
						int num = array.Length;
						string text = string.Empty;
						List<string> list = null;
						for (int i = 0; i < num; i++)
						{
							text = array[i].Substring(0, 1);
							if (string.IsNullOrEmpty(text))
							{
								text = array[i].Trim().Substring(0, 1);
							}
							list = null;
							if (Global.FilterFieldsDict.TryGetValue(text, ref list) && list != null)
							{
								list.Add(array[i]);
							}
							else
							{
								List<string> list2 = new List<string>();
								list2.Add(array[i]);
								list = list2;
								Global.FilterFieldsDict.Add(text, list);
							}
						}
					}
					else
					{
						Global.FilterFieldsDict = null;
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static string ReplaceFilterFileds(string text)
		{
			if (Global.FilterFieldsDict == null)
			{
				return text;
			}
			string text2 = text;
			char[] array = text2.ToCharArray();
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				string text3 = array[i].ToString();
				if (!string.IsNullOrEmpty(text3) && !Enumerable.Contains<string>(list, text3))
				{
					list.Add(text3);
					List<string> list2 = null;
					if (Global.FilterFieldsDict.TryGetValue(text3, ref list2) && list2 != null)
					{
						int count = list2.Count;
						for (int j = 0; j < count; j++)
						{
							text2 = Global.StringReplaceAll(text2, list2[j], "***");
						}
					}
				}
			}
			return text2;
		}

		public static bool IncludeReplaceFilterFileds(string text)
		{
			if (Global.FilterFieldsDict == null)
			{
				return false;
			}
			char[] array = text.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					List<string> list = null;
					if (Global.FilterFieldsDict.TryGetValue(text2, ref list) && list != null)
					{
						int count = list.Count;
						for (int j = 0; j < count; j++)
						{
							if (text.IndexOf(list[j]) != -1)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		private static char ConvertSpecialText(char ch)
		{
			char result = ch;
			if (ch >= '0' && ch <= '9')
			{
				return Global.SpecialDigits.get_Chars((int)(ch - '0'));
			}
			if (ch == '[')
			{
				result = '［';
			}
			else if (ch == ']')
			{
				result = '］';
			}
			else if (ch == '(')
			{
				result = '（';
			}
			else if (ch == ')')
			{
				result = '）';
			}
			else if (ch == '-')
			{
				result = '－';
			}
			else if (ch == '@')
			{
				result = '＠';
			}
			else if (ch == ',')
			{
				result = '，';
			}
			else if (ch == '%')
			{
				result = '％';
			}
			else if (ch == '/')
			{
				result = '∕';
			}
			return result;
		}

		public static string ConvertSpecialText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				stringBuilder.Append(Global.ConvertSpecialText(text.get_Chars(i)));
			}
			return stringBuilder.ToString();
		}

		public static string GetSystemTipByID(string id)
		{
			XElement gameResXml = Global.GetGameResXml("Config/SystemTips.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Tip", "ID", id);
			if (xelement == null)
			{
				return null;
			}
			return Global.GetXElementAttributeStr(xelement, "Description");
		}

		public static int[] GetCallMagicByOccupation(int occu)
		{
			if (Global.callMagicDict.Count <= 0)
			{
				Global.GetCallMagic();
			}
			int[] result;
			if (occu < Global.callMagicDict.Count)
			{
				result = Global.callMagicDict[occu];
			}
			else
			{
				result = new int[]
				{
					-1,
					-1
				};
			}
			return result;
		}

		private static void GetCallMagic()
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("CallMagic", '|');
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				int[] array2 = new int[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = Convert.ToInt32(array[j]);
				}
				if (!Global.callMagicDict.ContainsKey(i))
				{
					Global.callMagicDict.Add(i, array2);
				}
				else
				{
					Global.callMagicDict[i] = array2;
				}
			}
		}

		public static bool GetTerraceOpen(string path = "EmblemOpen")
		{
			bool result = false;
			string systemParamByName = ConfigSystemParam.GetSystemParamByName(path, true);
			if (string.IsNullOrEmpty(systemParamByName))
			{
				return result;
			}
			string[] array = systemParamByName.Split(new char[]
			{
				'|'
			});
			int num = 3;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Split(new char[]
				{
					','
				}).Length == 2 && array[i].Split(new char[]
				{
					','
				})[0].SafeToInt32(0) == num && array[i].Split(new char[]
				{
					','
				})[1].SafeToInt32(0) == 1)
				{
					result = true;
				}
			}
			return result;
		}

		public static List<TaoZhuangVO> GetTaoZhuangList()
		{
			if (Global.TaoZhuangVOList == null)
			{
				Global.TaoZhuangVOList = new List<TaoZhuangVO>();
				XElement gameResXml = Global.GetGameResXml("Config/QiangHuaFuJia.xml");
				if (gameResXml != null)
				{
					List<XElement> xelementList = Global.GetXElementList(gameResXml, "QiangHuaFuJia");
					if (xelementList != null)
					{
						for (int i = 0; i < xelementList.Count; i++)
						{
							TaoZhuangVO taoZhuangVO = new TaoZhuangVO();
							taoZhuangVO.Level = Global.GetXElementAttributeInt(xelementList[i], "QiangHuaLevel");
							taoZhuangVO.HPAdded = Global.GetXElementAttributeFloat(xelementList[i], "MaxLifePercent");
							taoZhuangVO.HurtAdded = Global.GetXElementAttributeFloat(xelementList[i], "AddAttackInjurePercent");
							taoZhuangVO.Decoration1 = Global.GetXElementAttributeStr(xelementList[i], "Decorations");
							Global.TaoZhuangVOList.Add(taoZhuangVO);
						}
					}
				}
			}
			return Global.TaoZhuangVOList;
		}

		public static double[] GetZhuoYueBuffs()
		{
			return ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuoYueBuff");
		}

		public static int[] GetChengJiuBufferGoodsIDs()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ChengJiuBufferGoodsIDs", ',');
			int num = systemParamIntArrayByName.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = systemParamIntArrayByName[i];
			}
			return array;
		}

		public static int[] GetJunXianBufferGoodsIDs()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JunXianBufferGoodsIDs", ',');
			int num = systemParamIntArrayByName.Length;
			int[] array = new int[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = systemParamIntArrayByName[i];
			}
			return array;
		}

		public static int GetSystemParamVipLeveValue(string name)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName(name, ',');
			if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length >= Global.GetVIPLeve())
			{
				return systemParamIntArrayByName[Global.GetVIPLeve()];
			}
			return 0;
		}

		public static int GetTaskTalkText(int id, out string npcText, out string roleText)
		{
			npcText = string.Empty;
			roleText = string.Empty;
			XElement gameResXml = Global.GetGameResXml("Config/TaskTalks.Xml");
			if (id != -1 && gameResXml != null)
			{
				XElement xelement = Global.GetXElement(gameResXml, "Talk", "ID", id.ToString());
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "NextID");
					npcText = Global.GetXElementAttributeStr(xelement, "NPCText");
					roleText = Global.GetXElementAttributeStr(xelement, "RoleText");
					return xelementAttributeInt;
				}
			}
			return -1;
		}

		public static GoodsData GetPackGoodsDataByDbID(int goodsDbID)
		{
			if (Global.Data.ViewGoodsPackDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.ViewGoodsPackDataList.Count; i++)
			{
				if (Global.Data.ViewGoodsPackDataList[i].Id == goodsDbID)
				{
					return Global.Data.ViewGoodsPackDataList[i];
				}
			}
			return null;
		}

		public static List<GoodsData> UnPackGoodsID(int baoguoID)
		{
			string xmlName = string.Format("Config/GoodsPack.Xml", new object[0]);
			XElement gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Goods", "ID", baoguoID.ToString());
			if (xelement == null)
			{
				return null;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Item");
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				return null;
			}
			string[] array = xelementAttributeStr.Split(new char[]
			{
				'|'
			});
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Trim().Split(new char[]
				{
					','
				});
				if (array2 != null && array2.Length == 7)
				{
					list.Add(new GoodsData
					{
						Id = i,
						GoodsID = Global.SafeConvertToInt32(array2[0]),
						Using = 0,
						Forge_level = Global.SafeConvertToInt32(array2[3]),
						AppendPropLev = Global.SafeConvertToInt32(array2[4]),
						Starttime = "1900-01-01 12:00:00",
						Endtime = "1900-01-01 12:00:00",
						Site = 0,
						Quality = 0,
						Props = string.Empty,
						GCount = Global.SafeConvertToInt32(array2[1]),
						Binding = Global.SafeConvertToInt32(array2[2]),
						Jewellist = string.Empty,
						BagIndex = 0,
						AddPropIndex = 0,
						BornIndex = 0,
						Lucky = Global.SafeConvertToInt32(array2[5]),
						ExcellenceInfo = Global.SafeConvertToInt32(array2[6]),
						Strong = 0
					});
				}
			}
			return list;
		}

		public static BufferData GetBufferDataByGoodsID(int goodsID)
		{
			if (Global.Data.roleData.BufferDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Enumerable.Count<BufferData>(Global.Data.roleData.BufferDataList); i++)
			{
				BufferData bufferData = Global.Data.roleData.BufferDataList[i];
				if (bufferData.BufferID == 46)
				{
					int num = (int)(bufferData.BufferVal & (long)((ulong)-1));
					int num2 = (int)((double)(bufferData.BufferVal - (long)num) / Math.Pow(2.0, 32.0));
					if (num2 == goodsID)
					{
						return bufferData;
					}
				}
				else
				{
					int num2 = (int)bufferData.BufferVal;
					if (num2 == goodsID)
					{
						return bufferData;
					}
				}
			}
			return null;
		}

		public static BufferData GetBufferDataByID(int bufferID)
		{
			if (Global.Data.roleData.BufferDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Enumerable.Count<BufferData>(Global.Data.roleData.BufferDataList); i++)
			{
				if (Global.Data.roleData.BufferDataList[i].BufferID == bufferID)
				{
					return Global.Data.roleData.BufferDataList[i];
				}
			}
			return null;
		}

		public static BufferData GetBufferDataByID(int bufferID, RoleData roleData)
		{
			if (roleData == null)
			{
				return null;
			}
			if (roleData.BufferDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Enumerable.Count<BufferData>(roleData.BufferDataList); i++)
			{
				if (roleData.BufferDataList[i].BufferID == bufferID)
				{
					return roleData.BufferDataList[i];
				}
			}
			return null;
		}

		public static bool IsVisiableBuff(BufferData bufferData)
		{
			bool result = true;
			if (bufferData.BufferID == 70)
			{
				result = false;
			}
			return result;
		}

		public static int GetVisibalBufferCount()
		{
			if (Global.Data.roleData.BufferDataList == null)
			{
				return 0;
			}
			Global.VisiableBuffCount = 0;
			int count = Global.Data.roleData.BufferDataList.Count;
			for (int i = 0; i < count; i++)
			{
				BufferData bufferData = Global.Data.roleData.BufferDataList[i];
				if (!Global.IsDummyBuffer(bufferData.BufferID))
				{
					if (!Global.IsBufferDataOver(bufferData, Global.GetCorrectLocalTime(), false))
					{
						Global.VisiableBuffCount++;
					}
				}
			}
			return Global.VisiableBuffCount;
		}

		public static void AddBufferData(BufferData bufferData)
		{
			if (Global.Data.roleData.BufferDataList == null)
			{
				Global.Data.roleData.BufferDataList = new List<BufferData>();
			}
			int num = -1;
			for (int i = 0; i < Global.Data.roleData.BufferDataList.Count; i++)
			{
				if (Global.Data.roleData.BufferDataList[i].BufferID == bufferData.BufferID)
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				Global.Data.roleData.BufferDataList.Add(bufferData);
			}
			else
			{
				Global.Data.roleData.BufferDataList[num] = bufferData;
			}
		}

		public static bool RestoreBufferDataBufferSecs(BufferData bufferData)
		{
			if (bufferData.BufferID == 39)
			{
				bufferData.BufferSecs = 86400;
				return true;
			}
			return false;
		}

		public static bool IsTitleBufferDataOver(BufferData bufferData, long nowTicks = 0L, bool ignoreBufferType = false)
		{
			if (!Global.SpecialTitleBuffer(bufferData.BufferID, true))
			{
				return Global.IsBufferDataOver(bufferData, 0L, false);
			}
			if (bufferData.BufferID == 39)
			{
				return bufferData.BufferVal <= 0L || bufferData.StartTime + (long)bufferData.BufferSecs * 1000L < Global.GetCorrectLocalTime();
			}
			return bufferData.BufferVal <= 0L;
		}

		public static bool SpecialTitleBuffer(int BuffID, bool bContainPKKing = false)
		{
			switch (BuffID)
			{
			case 10001:
			case 10002:
			case 10003:
			case 10004:
			case 10005:
			case 10006:
			case 10007:
			case 10008:
			case 10009:
			case 10010:
			case 10011:
			case 10012:
			case 10013:
			case 10020:
			case 10022:
			case 10023:
				break;
			default:
				switch (BuffID)
				{
				case 9000:
				case 9001:
				case 9002:
				case 9003:
				case 9004:
				case 9005:
				case 9006:
				case 9007:
				case 9008:
				case 9009:
				case 9010:
				case 9011:
					break;
				default:
					if (BuffID != 9051 && BuffID != 9052)
					{
						if (BuffID == 39)
						{
							return bContainPKKing;
						}
						if (BuffID != 103 && BuffID != 111)
						{
							return 20000 <= BuffID && BuffID <= 29999;
						}
					}
					break;
				}
				break;
			}
			return true;
		}

		public static bool IsBufferDataOver(BufferData bufferData, long nowTicks = 0L, bool ignoreBufferType = false)
		{
			if (bufferData.BufferID == 4 || bufferData.BufferID == 5 || bufferData.BufferID == 10)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 28)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 48)
			{
				return bufferData.BufferSecs <= 0;
			}
			if (bufferData.BufferID == 100)
			{
				return false;
			}
			if (bufferData.BufferID == 99)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 101)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 103)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 111)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 2080010)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 2080011)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 2080001 || bufferData.BufferID == 2080007 || bufferData.BufferID == 2080008 || bufferData.BufferID == 2080009)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 2080002)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 115)
			{
				return bufferData.BufferVal <= 0L;
			}
			if (bufferData.BufferID == 34)
			{
				if (bufferData.StartTime != (long)DateTime.Now.DayOfYear)
				{
					return bufferData.BufferSecs <= 0;
				}
				return bufferData.BufferVal + (long)bufferData.BufferSecs <= 0L;
			}
			else
			{
				if (bufferData.BufferID == 49)
				{
					return false;
				}
				if (bufferData.BufferID == 87)
				{
					return false;
				}
				if (bufferData.BufferID == 10005)
				{
					return false;
				}
				if (bufferData.BufferID == 10006)
				{
					return false;
				}
				if (bufferData.BufferID == 8999)
				{
					return !Global.IsInShiLiZhengBaMap() || bufferData.BufferVal == 0L;
				}
				if (bufferData.BufferID == 9000 || bufferData.BufferID == 9001 || bufferData.BufferID == 9002)
				{
					return bufferData.BufferVal == 0L;
				}
				if (bufferData.BufferID >= 9012 && bufferData.BufferID <= 9017)
				{
					return bufferData.BufferVal == 0L;
				}
				if (bufferData.BufferID >= 2000853 && bufferData.BufferID <= 2000857)
				{
					return bufferData.BufferVal == 0L;
				}
				if (bufferData.BufferID == 2090001 || bufferData.BufferID == 2090002)
				{
					return bufferData.BufferVal == 0L;
				}
				if (20000 <= bufferData.BufferID && bufferData.BufferID <= 29999)
				{
					return bufferData.BufferVal == 0L;
				}
				if (bufferData.BufferType > 0 && !ignoreBufferType)
				{
					if (bufferData.StartTime > 0L)
					{
						if (bufferData.BufferSecs <= 0)
						{
							return true;
						}
						if (nowTicks <= 0L)
						{
							nowTicks = Global.GetCorrectLocalTime();
						}
						long num = (long)bufferData.BufferSecs * 1000L;
						if (nowTicks - bufferData.StartTime < num)
						{
							return false;
						}
					}
					return true;
				}
				if (bufferData.BufferSecs >= 0)
				{
					if (nowTicks <= 0L)
					{
						nowTicks = Global.GetCorrectLocalTime();
					}
					long num2 = (long)bufferData.BufferSecs * 1000L;
					return nowTicks - bufferData.StartTime >= num2;
				}
				return false;
			}
		}

		public static bool IsGoodsRelateBufferExist(int goodsID)
		{
			int goodsBindBufferID = Global.GetGoodsBindBufferID(goodsID);
			if (goodsBindBufferID < 0)
			{
				return false;
			}
			if (goodsBindBufferID == 13)
			{
				return Global.IsVip();
			}
			if (goodsBindBufferID == 48)
			{
				BufferData bufferDataByID = Global.GetBufferDataByID(goodsBindBufferID);
				if (bufferDataByID == null)
				{
					return false;
				}
				if (Global.IsBufferDataOver(bufferDataByID, 0L, false))
				{
					return false;
				}
				int num = (int)(bufferDataByID.BufferVal & (long)((ulong)-1));
				long num2 = (long)((double)(bufferDataByID.BufferVal - (long)num) / Math.Pow(2.0, 32.0));
				return (long)goodsID != num2;
			}
			else
			{
				BufferItemTypes[] array = new BufferItemTypes[]
				{
					BufferItemTypes.MutilExperience,
					BufferItemTypes.FiveExperience,
					BufferItemTypes.ThreeExperience,
					BufferItemTypes.DblExperience
				};
				int num3 = Convert.ToInt32(array.ToString().IndexOf(goodsBindBufferID.ToString()));
				if (num3 >= 0)
				{
					bool result = false;
					for (int i = 0; i < array.Length; i++)
					{
						if (Global.IsBufferExist((int)array[i]))
						{
							result = true;
						}
					}
					return result;
				}
				return Global.IsBufferExist(goodsBindBufferID);
			}
		}

		public static bool IsAttackFuZhouBufferExist()
		{
			BufferItemTypes[] array = new BufferItemTypes[]
			{
				BufferItemTypes.TimeAddAttack,
				BufferItemTypes.TimeAddDSAttack,
				BufferItemTypes.TimeAddMAttack
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (Global.IsBufferExist((int)array[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsExpBufferExist()
		{
			BufferItemTypes[] array = new BufferItemTypes[]
			{
				BufferItemTypes.MutilExperience,
				BufferItemTypes.FiveExperience,
				BufferItemTypes.ThreeExperience,
				BufferItemTypes.DblExperience
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (Global.IsBufferExist((int)array[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsDummyBuffer(int bufferID)
		{
			bool result = false;
			switch (bufferID)
			{
			case 34:
			case 35:
			case 40:
			case 41:
			case 42:
			case 43:
			case 44:
			case 45:
				break;
			default:
				switch (bufferID)
				{
				case 93:
				case 94:
				case 95:
				case 96:
				case 97:
				case 98:
				case 103:
					break;
				default:
					if (bufferID != 16 && bufferID != 70 && bufferID != 111 && bufferID != 116)
					{
						goto IL_9E;
					}
					break;
				}
				break;
			}
			result = true;
			IL_9E:
			if (20000 <= bufferID && bufferID <= 29999)
			{
				result = Global.IsShowSpecialTitleBuff(bufferID);
			}
			return result;
		}

		public static bool IsLittleBuffer(int bufferID)
		{
			return bufferID == 40 || bufferID == 41 || bufferID == 42 || bufferID == 43 || bufferID == 44 || bufferID == 45;
		}

		public static bool IsBufferExist(int bufferID)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(bufferID);
			return bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L, false);
		}

		public static bool IsBufferExist(int bufferID, RoleData roleData)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(bufferID, roleData);
			return bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L, false);
		}

		public static string GetPKKingSpriteName(RoleData roleData)
		{
			return (!Global.IsBufferExist(39, roleData)) ? null : "PKKing";
		}

		public static double ProcessMonthVIP()
		{
			double result = 0.0;
			BufferData bufferDataByID = Global.GetBufferDataByID(13);
			if (bufferDataByID == null)
			{
				return result;
			}
			if (!Global.IsBufferDataOver(bufferDataByID, 0L, false))
			{
				result = 1.0;
			}
			return result;
		}

		public static int GetVipType()
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(13);
			if (bufferDataByID == null)
			{
				return 0;
			}
			if (Global.IsBufferDataOver(bufferDataByID, 0L, false))
			{
				return 0;
			}
			return (int)bufferDataByID.BufferVal;
		}

		public static bool IsVip()
		{
			return Global.Data.roleData.VIPLevel > 0;
		}

		public static bool IsVipOfMonth()
		{
			return Global.GetVipType() == 1;
		}

		public static bool IsVipOfSeason()
		{
			return Global.GetVipType() == 3;
		}

		public static bool IsVipOfHalfYear()
		{
			return Global.GetVipType() == 6;
		}

		public static string GetVipTypeNameString()
		{
			if (Global.IsVipOfMonth())
			{
				return Global.GetLang("白银VIP");
			}
			if (Global.IsVipOfSeason())
			{
				return Global.GetLang("黄金VIP");
			}
			if (Global.IsVipOfHalfYear())
			{
				return Global.GetLang("钻石VIP");
			}
			return string.Empty;
		}

		public static long GetVipOnceAwardFlagField()
		{
			if (Global.IsVipOfMonth())
			{
				return 2L;
			}
			if (Global.IsVipOfSeason())
			{
				return 4L;
			}
			if (Global.IsVipOfHalfYear())
			{
				return 8L;
			}
			return 0L;
		}

		public static int GetVipBindGoodsID()
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(13);
			if (bufferDataByID == null)
			{
				return -1;
			}
			if (bufferDataByID.BufferVal == 1L)
			{
				return 30000;
			}
			if (bufferDataByID.BufferVal == 3L)
			{
				return 30001;
			}
			if (bufferDataByID.BufferVal == 6L)
			{
				return 30002;
			}
			return -1;
		}

		public static bool IsMonthVipOnceAwardTaked()
		{
			long num = 2L;
			return (Global.Data.roleData.OnceAwardFlag & num) > 0L;
		}

		public static bool IsSeasonVipOnceAwardTaked()
		{
			long num = 4L;
			return (Global.Data.roleData.OnceAwardFlag & num) > 0L;
		}

		public static bool IsHalfYearVipOnceAwardTaked()
		{
			long num = 8L;
			return (Global.Data.roleData.OnceAwardFlag & num) > 0L;
		}

		public static bool IsVipOnceAwardTaked()
		{
			long vipOnceAwardFlagField = Global.GetVipOnceAwardFlagField();
			return (Global.Data.roleData.OnceAwardFlag & vipOnceAwardFlagField) > 0L;
		}

		public static bool IsVipOnceAwardTakedEx(int vipType)
		{
			long num = 0L;
			if (vipType == 1)
			{
				num = 2L;
			}
			else if (vipType == 3)
			{
				num = 4L;
			}
			else if (vipType == 6)
			{
				num = 8L;
			}
			return (Global.Data.roleData.OnceAwardFlag & num) > 0L;
		}

		public static void SaveSystemSettings()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(StringUtil.substitute("{0}", new object[]
			{
				Global.Data.SysSetting.HideOtherRoles.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideOtherRolesBloodBar.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideChatPopupWin.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideGameEffect.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideTeamMembersFace.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.RefuseTeamRequest.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.RefusePrivateChat.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.RefuseExchangeRequest.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.CloseGameMusic.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.CloseGameAudio.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.AutoAcceptTeamApply.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.AutoAcceptTeamInvite.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.ShowMonsterName.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HidePaiHangHeaderPics.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.ShowGoodsPackName.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.GraphicsQuality.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.ScreenLockSize.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.ScreenLockSizeWindowFinish.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				0
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideChiBang.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideOtherRolesChiBangStatus.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.SysSetting.HideOtherRolesStatus.ToString()
			}));
			LocalStorage.SetString(StringUtil.substitute("MstSettings_{0}", new object[]
			{
				Global.Data.roleData.RoleID
			}), stringBuilder.ToString());
			LocalStorage.SetString("IsSetGraphicsQuality", "Setted");
		}

		public static void LoadSystemSettings()
		{
			string @string = LocalStorage.GetString(StringUtil.substitute("MstSettings_{0}", new object[]
			{
				Global.Data.roleData.RoleID
			}));
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			string[] array = @string.Split(new char[]
			{
				'|'
			});
			if (array.Length >= 16)
			{
				Global.Data.SysSetting.HideOtherRoles = Convert.ToBoolean(array[0]);
				Global.Data.SysSetting.HideOtherRolesBloodBar = Convert.ToBoolean(array[1]);
				Global.Data.SysSetting.HideChatPopupWin = Convert.ToBoolean(array[2]);
				Global.Data.SysSetting.HideGameEffect = Convert.ToBoolean(array[3]);
				Global.Data.SysSetting.HideTeamMembersFace = Convert.ToBoolean(array[4]);
				Global.Data.SysSetting.RefuseTeamRequest = Convert.ToBoolean(array[5]);
				Global.Data.SysSetting.RefusePrivateChat = Convert.ToBoolean(array[6]);
				Global.Data.SysSetting.RefuseExchangeRequest = Convert.ToBoolean(array[7]);
				Global.Data.SysSetting.CloseGameMusic = Convert.ToBoolean(array[8]);
				Global.Data.SysSetting.CloseGameAudio = Convert.ToBoolean(array[9]);
				Global.Data.SysSetting.AutoAcceptTeamApply = Convert.ToBoolean(array[10]);
				Global.Data.SysSetting.AutoAcceptTeamInvite = Convert.ToBoolean(array[11]);
				Global.Data.SysSetting.ShowMonsterName = Convert.ToBoolean(array[12]);
				Global.Data.SysSetting.HidePaiHangHeaderPics = Convert.ToBoolean(array[13]);
				Global.Data.SysSetting.ShowGoodsPackName = Convert.ToBoolean(array[14]);
				Global.Data.SysSetting.GraphicsQuality = Convert.ToBoolean(array[15]);
			}
			if (array.Length >= 18)
			{
				if (array.Length >= 17)
				{
					Global.Data.SysSetting.ScreenLockSize = Convert.ToBoolean(array[16]);
				}
				if (array.Length >= 18)
				{
					Global.Data.SysSetting.ScreenLockSizeWindowFinish = Convert.ToBoolean(array[17]);
				}
			}
			if (array.Length >= 19)
			{
				Global.Data.SysSetting.HideFashion = Convert.ToBoolean(0);
			}
			if (array.Length >= 20)
			{
				Global.Data.SysSetting.HideChiBang = Convert.ToBoolean(array[19]);
			}
			if (array.Length >= 21)
			{
				Global.Data.SysSetting.HideOtherRolesChiBangStatus = Convert.ToBoolean(array[20]);
			}
			if (array.Length >= 22)
			{
				Global.Data.SysSetting.HideOtherRolesStatus = Convert.ToBoolean(array[21]);
			}
			if (string.IsNullOrEmpty(LocalStorage.GetString("IsSetGraphicsQuality")) || LocalStorage.GetString("IsSetGraphicsQuality") != "Setted")
			{
				Global.Data.SysSetting.GraphicsQuality = false;
			}
		}

		public static void SaveAutoFightSettings()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(StringUtil.substitute("{0}", new object[]
			{
				Global.Data.AutoFightData.FightRadius.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.MaxFightSecs.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGetEquips.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGetThings.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGoBack.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGoBackWhenNoLifeDrugs.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGoBackWhenNoMagicDrugs.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.DontAttackBigBoss.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoRealive.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoUseExpCard.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoUseLifeReserveDrugs.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoUseMagicReserveDrugs.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGetMoney.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoGetDrugs.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoAntiAttack.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoOpenLieHuoJianQi.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoOpenFSHunDun.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoZhaohuanShenshou.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.LifeLessThanXDo.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.MagicLessThanXDo.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.LifeLessThanX.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.MagicLessThanX.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.LifeLessThanXAutoUse.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.MagicLessThanXAutoUse.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.IsOnlineGuaJi.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.Multi.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.TimeMax.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.RemainExper.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.RemainExperMax.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.GoldUsed.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoPickThingFlags.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.SkillID.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.AutoBuyMedicine.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.RoleID.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.SkillPriority
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.IsUseDefaultSkillList.ToString()
			}));
			stringBuilder.Append(StringUtil.substitute("|{0}", new object[]
			{
				Global.Data.AutoFightData.SkillPriorityDict
			}));
			LocalStorage.SetString(StringUtil.substitute("MstFightSettings_{0}", new object[]
			{
				Global.Data.roleData.RoleID
			}), stringBuilder.ToString());
		}

		public static void LoadAutoFightSettings()
		{
			string @string = LocalStorage.GetString(StringUtil.substitute("MstFightSettings_{0}", new object[]
			{
				Global.Data.roleData.RoleID
			}));
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			string[] array = @string.Split(new char[]
			{
				'|'
			});
			if (array.Length != 37)
			{
				Global.Data.AutoFightData = new LocalAutoFightData();
				return;
			}
			Global.Data.AutoFightData.FightRadius = Convert.ToInt32(array[0]);
			Global.Data.AutoFightData.MaxFightSecs = Convert.ToInt32(array[1]);
			Global.Data.AutoFightData.AutoGetEquips = Convert.ToBoolean(array[2]);
			Global.Data.AutoFightData.AutoGetThings = Convert.ToBoolean(array[3]);
			Global.Data.AutoFightData.AutoGoBack = Convert.ToBoolean(array[4]);
			Global.Data.AutoFightData.AutoGoBackWhenNoLifeDrugs = Convert.ToBoolean(array[5]);
			Global.Data.AutoFightData.AutoGoBackWhenNoMagicDrugs = Convert.ToBoolean(array[6]);
			Global.Data.AutoFightData.DontAttackBigBoss = Convert.ToBoolean(array[7]);
			Global.Data.AutoFightData.AutoRealive = Convert.ToBoolean(array[8]);
			Global.Data.AutoFightData.AutoUseExpCard = Convert.ToBoolean(array[9]);
			Global.Data.AutoFightData.AutoUseLifeReserveDrugs = Convert.ToBoolean(array[10]);
			Global.Data.AutoFightData.AutoUseMagicReserveDrugs = Convert.ToBoolean(array[11]);
			Global.Data.AutoFightData.AutoGetMoney = Convert.ToBoolean(array[12]);
			Global.Data.AutoFightData.AutoGetDrugs = Convert.ToBoolean(array[13]);
			Global.Data.AutoFightData.AutoAntiAttack = Convert.ToBoolean(array[14]);
			Global.Data.AutoFightData.AutoOpenLieHuoJianQi = Convert.ToBoolean(array[15]);
			Global.Data.AutoFightData.AutoOpenFSHunDun = Convert.ToBoolean(array[16]);
			Global.Data.AutoFightData.AutoZhaohuanShenshou = Convert.ToBoolean(array[17]);
			Global.Data.AutoFightData.LifeLessThanXDo = Convert.ToBoolean(array[18]);
			Global.Data.AutoFightData.MagicLessThanXDo = Convert.ToBoolean(array[19]);
			Global.Data.AutoFightData.LifeLessThanX = Convert.ToInt32(array[20]);
			Global.Data.AutoFightData.MagicLessThanX = Convert.ToInt32(array[21]);
			Global.Data.AutoFightData.LifeLessThanXAutoUse = Math.Max(0, Convert.ToInt32(array[22]));
			Global.Data.AutoFightData.MagicLessThanXAutoUse = Math.Max(0, Convert.ToInt32(array[23]));
			Global.Data.AutoFightData.IsOnlineGuaJi = Convert.ToBoolean(array[24]);
			Global.Data.AutoFightData.Multi = Convert.ToInt32(array[25]);
			Global.Data.AutoFightData.TimeMax = Convert.ToSingle(array[26]);
			Global.Data.AutoFightData.RemainExper = Convert.ToInt64(array[27]);
			Global.Data.AutoFightData.RemainExperMax = Convert.ToInt64(array[28]);
			Global.Data.AutoFightData.GoldUsed = Convert.ToInt64(array[29]);
			Global.Data.AutoFightData.AutoPickThingFlags = Convert.ToInt32(array[30]);
			Global.Data.AutoFightData.SkillID = Convert.ToInt32(array[31]);
			Global.Data.AutoFightData.AutoBuyMedicine = Convert.ToBoolean(array[32]);
			Global.Data.AutoFightData.RoleID = Convert.ToInt32(array[33]);
			Global.Data.AutoFightData.SkillPriority = Convert.ToString(array[34]);
			Global.Data.AutoFightData.IsUseDefaultSkillList = Convert.ToBoolean(array[35]);
			Global.Data.AutoFightData.SkillPriorityDict = Convert.ToString(array[36]);
		}

		public static LocalAutoFightData CopyAutoFight(LocalAutoFightData autoFight)
		{
			LocalAutoFightData localAutoFightData = new LocalAutoFightData
			{
				Fighting = autoFight.Fighting,
				FightStartTicks = autoFight.FightStartTicks,
				FightRadius = autoFight.FightRadius,
				MaxFightSecs = autoFight.MaxFightSecs,
				AutoGetEquips = autoFight.AutoGetEquips,
				AutoGetThings = autoFight.AutoGetThings,
				AutoGoBack = autoFight.AutoGoBack,
				AutoGoBackWhenNoLifeDrugs = autoFight.AutoGoBackWhenNoLifeDrugs,
				AutoGoBackWhenNoMagicDrugs = autoFight.AutoGoBackWhenNoMagicDrugs,
				DontAttackBigBoss = autoFight.DontAttackBigBoss,
				AutoRealive = autoFight.AutoRealive,
				AutoUseExpCard = autoFight.AutoUseExpCard,
				AutoUseLifeReserveDrugs = autoFight.AutoUseLifeReserveDrugs,
				AutoUseMagicReserveDrugs = autoFight.AutoUseMagicReserveDrugs,
				AutoGetMoney = autoFight.AutoGetMoney,
				AutoGetDrugs = autoFight.AutoGetDrugs,
				AutoAntiAttack = autoFight.AutoAntiAttack,
				AutoOpenLieHuoJianQi = autoFight.AutoOpenLieHuoJianQi,
				AutoOpenFSHunDun = autoFight.AutoOpenFSHunDun,
				AutoZhaohuanShenshou = autoFight.AutoZhaohuanShenshou,
				LifeLessThanXDo = autoFight.LifeLessThanXDo,
				MagicLessThanXDo = autoFight.MagicLessThanXDo,
				LifeLessThanX = autoFight.LifeLessThanX,
				MagicLessThanX = autoFight.MagicLessThanX,
				LifeLessThanXAutoUse = autoFight.LifeLessThanXAutoUse,
				MagicLessThanXAutoUse = autoFight.MagicLessThanXAutoUse,
				Multi = autoFight.Multi,
				TimeMax = autoFight.TimeMax,
				RemainExper = autoFight.RemainExper,
				RemainExperMax = autoFight.RemainExperMax,
				IsOnlineGuaJi = autoFight.IsOnlineGuaJi,
				GoldUsed = autoFight.GoldUsed,
				AutoPickThingFlags = autoFight.AutoPickThingFlags,
				SkillID = autoFight.SkillID,
				FightPoint = autoFight.FightPoint,
				AutoBuyMedicine = autoFight.AutoBuyMedicine,
				RoleID = autoFight.RoleID,
				SkillPriority = autoFight.SkillPriority,
				IsUseDefaultSkillList = autoFight.IsUseDefaultSkillList,
				SkillPriorityDict = autoFight.SkillPriorityDict
			};
			Global.SetAutoGetThingsValus(localAutoFightData, localAutoFightData.AutoPickThingFlags);
			return localAutoFightData;
		}

		public static Dictionary<int, int> StringConvertToDictionary(string skillPriorityStr)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			string[] array = skillPriorityStr.Split(new char[]
			{
				'*'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'_'
				});
				int num = int.Parse(array2[0]);
				int num2 = int.Parse(array2[1]);
				dictionary.Add(num, num2);
			}
			return dictionary;
		}

		public static string DictionaryToString(Dictionary<int, int> skillPriorityDict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<int, int>.Enumerator enumerator = skillPriorityDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StringBuilder stringBuilder2 = stringBuilder;
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				stringBuilder2.Append(keyValuePair.Key);
				stringBuilder.Append("_");
				StringBuilder stringBuilder3 = stringBuilder;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				stringBuilder3.Append(keyValuePair2.Value);
				stringBuilder.Append("*");
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'*'
			});
		}

		public static Dictionary<string, Dictionary<int, int>> ConvertStringToDictionary(string origionalStr)
		{
			Dictionary<string, Dictionary<int, int>> dictionary = new Dictionary<string, Dictionary<int, int>>();
			string[] array = origionalStr.Split(new char[]
			{
				'#'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'-'
				});
				if (!dictionary.ContainsKey(array2[0]))
				{
					dictionary.Add(array2[0], Global.StringToDictionary2(array2[1]));
				}
			}
			return dictionary;
		}

		private static Dictionary<int, int> StringToDictionary2(string skillStr)
		{
			if (!string.IsNullOrEmpty(skillStr))
			{
				string skillPriorityStr = skillStr.TrimEnd(new char[]
				{
					'*'
				});
				return Global.StringConvertToDictionary(skillPriorityStr);
			}
			return new Dictionary<int, int>();
		}

		public static string ConvertDictionaryToString(Dictionary<string, Dictionary<int, int>> dicts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<string, Dictionary<int, int>>.Enumerator enumerator = dicts.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StringBuilder stringBuilder2 = stringBuilder;
				KeyValuePair<string, Dictionary<int, int>> keyValuePair = enumerator.Current;
				stringBuilder2.Append(keyValuePair.Key);
				KeyValuePair<string, Dictionary<int, int>> keyValuePair2 = enumerator.Current;
				Dictionary<int, int>.Enumerator enumerator2 = keyValuePair2.Value.GetEnumerator();
				stringBuilder.Append("-");
				while (enumerator2.MoveNext())
				{
					StringBuilder stringBuilder3 = stringBuilder;
					KeyValuePair<int, int> keyValuePair3 = enumerator2.Current;
					stringBuilder3.Append(keyValuePair3.Key);
					stringBuilder.Append("_");
					StringBuilder stringBuilder4 = stringBuilder;
					KeyValuePair<int, int> keyValuePair4 = enumerator2.Current;
					stringBuilder4.Append(keyValuePair4.Value);
					stringBuilder.Append("*");
				}
				stringBuilder.Append("#");
			}
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'#'
			});
		}

		private static void InitSkillPriorityDict(int key, int value)
		{
			Global.skillPriorityDict.Add(key, value);
		}

		public static void InitSkillPriority(string _skillPriorityDict, bool isSkillPartInit = false, bool isLocalSkillStatus = false)
		{
			if (Global.skillPriorityDict.Count > 0)
			{
				Global.skillPriorityDict.Clear();
			}
			if (string.IsNullOrEmpty(_skillPriorityDict))
			{
				Global.isOpenSkillPriority = false;
				return;
			}
			Global.skillDicts = Global.ConvertStringToDictionary(_skillPriorityDict);
			string currentSkillPriorityKey = Global.GetCurrentSkillPriorityKey();
			if (Global.skillDicts.ContainsKey(currentSkillPriorityKey))
			{
				if (Global.skillDicts[currentSkillPriorityKey] != null)
				{
					foreach (KeyValuePair<int, int> keyValuePair in Global.skillDicts[currentSkillPriorityKey])
					{
						if (keyValuePair.Value != 0)
						{
							Dictionary<int, int>.Enumerator enumerator;
							KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
							int key = keyValuePair2.Key;
							KeyValuePair<int, int> keyValuePair3 = enumerator.Current;
							Global.InitSkillPriorityDict(key, keyValuePair3.Value);
						}
					}
					if (Global.skillPriorityDict.Count > 0)
					{
						Global.isOpenSkillPriority = true;
					}
					else
					{
						Global.isOpenSkillPriority = false;
					}
				}
				else
				{
					Global.isOpenSkillPriority = false;
				}
			}
			else
			{
				Global.isOpenSkillPriority = false;
			}
		}

		public static Dictionary<string, Dictionary<int, int>> GetSkillDicts()
		{
			if (Global.skillDicts.Count > 0)
			{
				return Global.skillDicts;
			}
			return new Dictionary<string, Dictionary<int, int>>();
		}

		public static Dictionary<int, int> GetSkillDictsVaueByKey(string key)
		{
			if (Global.skillDicts.ContainsKey(key))
			{
				return Global.skillDicts[key];
			}
			return new Dictionary<int, int>();
		}

		public static void SetSkillDictsVaue(string key, Dictionary<int, int> value)
		{
			if (Global.skillDicts.ContainsKey(key))
			{
				Global.skillDicts[key] = value;
			}
			else
			{
				Global.skillDicts.Add(key, value);
			}
		}

		public static void ResetSkillPriorityDict(Dictionary<int, int> tmpDict)
		{
			Global.skillPriorityDict = tmpDict;
		}

		public static Dictionary<int, int> GetSkillPriorityDict()
		{
			if (Global.skillPriorityDict.Count > 0)
			{
				return Global.skillPriorityDict;
			}
			return new Dictionary<int, int>();
		}

		public static string GetCurrentSkillPriorityKey()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (Global.Data.roleData.Occupation == 3)
			{
				stringBuilder.Append(Global.Data.RoleID);
				stringBuilder.Append("*");
				stringBuilder.Append(Global.Data.roleData.Occupation);
				stringBuilder.Append("*");
				stringBuilder.Append((int)Global.GetMJSType());
			}
			else
			{
				stringBuilder.Append(Global.Data.RoleID);
				stringBuilder.Append("*");
				stringBuilder.Append(Global.Data.roleData.Occupation);
				stringBuilder.Append("*");
				stringBuilder.Append(9);
			}
			return stringBuilder.ToString();
		}

		public static void SaveZhuanShengPromptingSettings()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(StringUtil.substitute("{0}", new object[]
			{
				Global.Data.ZSPSetting.DontPromptingZhuanSheng.ToString()
			}));
			LocalStorage.SetString(StringUtil.substitute("ZSPSettings_{0}", new object[]
			{
				Global.Data.roleData.RoleID
			}), stringBuilder.ToString());
		}

		public static void LoadZhuanShengPromptingSettings()
		{
			string @string = LocalStorage.GetString(StringUtil.substitute("ZSPSettings_{0}", new object[]
			{
				Global.Data.roleData.RoleID
			}));
			if (string.IsNullOrEmpty(@string))
			{
				return;
			}
			Global.Data.ZSPSetting.DontPromptingZhuanSheng = Convert.ToBoolean(@string);
		}

		public static bool IsChineseLetter(string input, int index)
		{
			return !string.IsNullOrEmpty(input) && Global.IsChineseLetter(input.get_Chars(index));
		}

		public static bool IsChineseLetter(char charVal)
		{
			try
			{
				int num = Convert.ToInt32("4e00", 16);
				int num2 = Convert.ToInt32("9fa5", 16);
				Encoding encoding = Encoding.GetEncoding("Unicode");
				string text = string.Format("{0}", charVal);
				byte[] bytes = encoding.GetBytes(text);
				int num3 = (int)bytes[0] + (int)bytes[1] * 256;
				return num3 >= num && num3 <= num2;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return false;
		}

		public static int FindChineseLetterNum(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (Global.IsChineseLetter(str.get_Chars(i)))
				{
					num++;
				}
			}
			return num;
		}

		private static string GenerateUniqueId()
		{
			long num = 1L;
			byte[] array = Guid.NewGuid().ToByteArray();
			for (int i = 0; i < array.Length; i++)
			{
				num *= (long)(array[i] + 1);
			}
			return string.Format("{0:x}", num - DateTime.Now.Ticks);
		}

		public static string GetUniqueName(string prevName)
		{
			return string.Format("{0}_{1}", prevName, Global.GenerateUniqueId());
		}

		public static GoodsData AddGiftGoodsData(int goodsID, int forgeLevel, int quality, int binding, int gcount, int bornIndex)
		{
			int count = Global.Data.GiftsGoodsDataList.Count;
			GoodsData goodsData = new GoodsData();
			goodsData.Id = count;
			goodsData.GoodsID = goodsID;
			goodsData.Using = 0;
			goodsData.Forge_level = forgeLevel;
			goodsData.Starttime = "1900-01-01 12:00:00";
			goodsData.Endtime = "1900-01-01 12:00:00";
			goodsData.Site = 0;
			goodsData.Quality = quality;
			goodsData.Props = string.Empty;
			goodsData.GCount = gcount;
			goodsData.Binding = binding;
			goodsData.Jewellist = string.Empty;
			goodsData.BagIndex = 0;
			goodsData.SaleMoney1 = 0;
			goodsData.SaleYuanBao = 0;
			goodsData.SaleYinPiao = 0;
			goodsData.AddPropIndex = 0;
			goodsData.BornIndex = bornIndex;
			goodsData.Lucky = 0;
			goodsData.Strong = 0;
			Global.Data.GiftsGoodsDataList.Add(goodsData);
			return goodsData;
		}

		public static GoodsData GetGiftGoodsDataByID(int id)
		{
			for (int i = 0; i < Global.Data.GiftsGoodsDataList.Count; i++)
			{
				if (Global.Data.GiftsGoodsDataList[i].Id == id)
				{
					return Global.Data.GiftsGoodsDataList[i];
				}
			}
			return null;
		}

		public static GoodsData GetEmailGoodsDataByID(int id)
		{
			for (int i = 0; i < Global.Data.EmailFujianGoodsDataList.Count; i++)
			{
				if (Global.Data.EmailFujianGoodsDataList[i].Id == id)
				{
					return Global.Data.EmailFujianGoodsDataList[i];
				}
			}
			return null;
		}

		public static GoodsData GetNpcSaleGoodsDataByID(int id)
		{
			for (int i = 0; i < Global.Data.NpcSaleGoodsDataList.Count; i++)
			{
				if (Global.Data.NpcSaleGoodsDataList[i].Id == id)
				{
					return Global.Data.NpcSaleGoodsDataList[i];
				}
			}
			return null;
		}

		public static GoodsData GetBaoKuJiangLiGoodsDataByID(int id)
		{
			for (int i = 0; i < Global.Data.BaoKuJiangLiGoodsDataList.Count; i++)
			{
				if (Global.Data.BaoKuJiangLiGoodsDataList[i].Id == id)
				{
					return Global.Data.BaoKuJiangLiGoodsDataList[i];
				}
			}
			return null;
		}

		private static string ParseDateTime(string str)
		{
			int num = Global.GMax(1, Global.GetLang("月").Length);
			int num2 = Global.GMax(1, Global.GetLang("日").Length);
			int num3 = Global.GMax(1, Global.GetLang("时").Length);
			int num4 = Global.GMax(1, Global.GetLang("分").Length);
			int num5 = str.IndexOf(Global.GetLang("月"));
			if (num5 == -1)
			{
				return string.Empty;
			}
			int num6 = str.IndexOf(Global.GetLang("日"));
			if (num6 == -1)
			{
				return string.Empty;
			}
			int num7 = str.IndexOf(Global.GetLang("时"));
			if (num7 == -1)
			{
				return string.Empty;
			}
			int num8 = str.IndexOf(Global.GetLang("分"));
			if (num8 == -1)
			{
				return string.Empty;
			}
			string text = str.Substring(0, num5);
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			string text2 = str.Substring(num5 + num, num6 - num5 - num);
			if (string.IsNullOrEmpty(text2))
			{
				return string.Empty;
			}
			string text3 = str.Substring(num6 + num2, num7 - num6 - num2);
			if (string.IsNullOrEmpty(text3))
			{
				return string.Empty;
			}
			string text4 = str.Substring(num7 + num3, num8 - num7 - num3);
			if (string.IsNullOrEmpty(text4))
			{
				return string.Empty;
			}
			int year = DateTime.Now.Year;
			return StringUtil.substitute("{0}-{1}-{2} {3}:{4}:{5}", new object[]
			{
				Global.FormatStr("0000", year),
				Global.FormatStr("00", text),
				Global.FormatStr("00", text2),
				Global.FormatStr("00", text3),
				Global.FormatStr("00", text4),
				Global.FormatStr("00", 0)
			});
		}

		public static long GetHuoDongDateTime(string str)
		{
			string strDateTime = Global.ParseDateTime(str);
			return Global.SafeConvertToTicks(strDateTime);
		}

		public static long GetHuoDongDateTimeForCommonTimeString(string str)
		{
			return Global.SafeConvertToTicks(str);
		}

		public static bool InBigAwardPeriod()
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/BigGift.Xml");
			if (isolateResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(isolateResXml, "GiftTime");
			long huoDongDateTimeForCommonTimeString = Global.GetHuoDongDateTimeForCommonTimeString(Global.GetXElementAttributeStr(xelement, "FromDate"));
			long huoDongDateTimeForCommonTimeString2 = Global.GetHuoDongDateTimeForCommonTimeString(Global.GetXElementAttributeStr(xelement, "ToDate"));
			long correctLocalTime = Global.GetCorrectLocalTime();
			return correctLocalTime >= huoDongDateTimeForCommonTimeString && correctLocalTime < huoDongDateTimeForCommonTimeString2;
		}

		public static bool InSongLiPeriod()
		{
			string text = "dl_app";
			XElement xelement = new XElement("config");
			XElement xelement2 = Global.GetIsolateResXml("Config/Gifts/MU_Activities.xml");
			foreach (XElement xelement3 in xelement2.Elements())
			{
				if (xelement3.Attribute("TypeID").Value.ToString() == text)
				{
					xelement.Add(xelement3);
					break;
				}
			}
			xelement2 = xelement;
			if (xelement2 == null)
			{
				return false;
			}
			XElement xelement4 = Global.GetXElement(xelement2, "Activities");
			long huoDongDateTimeForCommonTimeString = Global.GetHuoDongDateTimeForCommonTimeString(Global.GetXElementAttributeStr(xelement4, "FromDate"));
			long huoDongDateTimeForCommonTimeString2 = Global.GetHuoDongDateTimeForCommonTimeString(Global.GetXElementAttributeStr(xelement4, "ToDate"));
			long correctLocalTime = Global.GetCorrectLocalTime();
			return correctLocalTime >= huoDongDateTimeForCommonTimeString && correctLocalTime < huoDongDateTimeForCommonTimeString2;
		}

		public static FuBenData GetFuBenData(int fuBenID)
		{
			if (Global.Data == null || Global.Data.roleData == null)
			{
				return null;
			}
			if (Global.Data.roleData.FuBenDataList == null)
			{
				return null;
			}
			for (int i = 0; i < Global.Data.roleData.FuBenDataList.Count; i++)
			{
				if (Global.Data.roleData.FuBenDataList[i].FuBenID == fuBenID)
				{
					return Global.Data.roleData.FuBenDataList[i];
				}
			}
			return null;
		}

		public static FuBenData AddFuBenData(int fuBenID, int dayID, int enterNum, int finishNum)
		{
			if (Global.Data.roleData.FuBenDataList == null)
			{
				Global.Data.roleData.FuBenDataList = new List<FuBenData>();
			}
			FuBenData fuBenData = new FuBenData();
			fuBenData.FuBenID = fuBenID;
			fuBenData.DayID = dayID;
			fuBenData.EnterNum = enterNum;
			fuBenData.FinishNum = finishNum;
			Global.Data.roleData.FuBenDataList.Add(fuBenData);
			return fuBenData;
		}

		public static void UpdateFuBenData(int fuBenID, int dayID, int enterNum, int finishNum)
		{
			FuBenData fuBenData = Global.GetFuBenData(fuBenID);
			if (fuBenData == null)
			{
				fuBenData = Global.AddFuBenData(fuBenID, dayID, enterNum, finishNum);
			}
			else
			{
				fuBenData.FuBenID = fuBenID;
				fuBenData.DayID = dayID;
				fuBenData.EnterNum = enterNum;
				fuBenData.FinishNum = finishNum;
			}
		}

		public static int GetFuBenEnterNum(FuBenData fuBenData, out int finishNum)
		{
			finishNum = 0;
			if (fuBenData == null)
			{
				return 0;
			}
			int dayOfYear = Global.GetCorrectDateTime().DayOfYear;
			if (fuBenData.DayID == dayOfYear)
			{
				finishNum = fuBenData.FinishNum;
				return fuBenData.EnterNum;
			}
			return 0;
		}

		public static int GetFuBenMaxNumOfDay(int fuBenID, out int maxFinishNum)
		{
			maxFinishNum = 0;
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", fuBenID.ToString());
			if (xelement == null)
			{
				return 0;
			}
			maxFinishNum = Global.GetXElementAttributeInt(xelement, "FinishNumber");
			return Global.GetXElementAttributeInt(xelement, "EnterNumber");
		}

		public static bool CanTakFuBenByLevel(int fuBenID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", fuBenID.ToString());
			if (xelement == null)
			{
				return false;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
			int num = Global.GetXElementAttributeInt(xelement, "MaxLevel");
			if (num <= 0)
			{
				num = 1000;
			}
			int level = Global.Data.roleData.Level;
			return level >= xelementAttributeInt && level <= num;
		}

		public static SceneUIClasses CanEnterByTypeID(int fuBenID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return SceneUIClasses.Normal;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", fuBenID.ToString());
			if (xelement == null)
			{
				return SceneUIClasses.Normal;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TabID");
			if (xelementAttributeInt == 600)
			{
				return SceneUIClasses.PaTa;
			}
			return SceneUIClasses.Normal;
		}

		public static int GetFuBenIdKeyMap(int mapCode)
		{
			int result = -1;
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return result;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
			if (xelementList == null)
			{
				return result;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "MapCode");
				if (xelementAttributeInt == mapCode)
				{
					return Global.GetXElementAttributeInt(xelementList[i], "ID");
				}
			}
			return result;
		}

		public static bool CanEnterFuBenByZhanLi(int fuBenID, out int zhanLi)
		{
			zhanLi = 0;
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", fuBenID.ToString());
			if (xelement == null)
			{
				return false;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinZhanLi");
			zhanLi = xelementAttributeInt;
			return Global.Data.roleData.CombatForce >= xelementAttributeInt;
		}

		public static int GetBossFuBenIDByLevel(int level)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			foreach (int result in ConfigSystemParam.GetSystemParamIntArrayByName("BossFuBenIDs", ','))
			{
				XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", result.ToString());
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					if (level >= xelementAttributeInt && level <= xelementAttributeInt2)
					{
						return result;
					}
				}
			}
			return 0;
		}

		public static bool GetIsOnePieceFuBen(int FuBenId)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", FuBenId.ToString());
			if (xelement == null)
			{
				return false;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "FuBenUse");
			return xelementAttributeInt == 13;
		}

		public static int GetBossFuBenMinLevel()
		{
			int num = 9999;
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			foreach (int num2 in ConfigSystemParam.GetSystemParamIntArrayByName("BossFuBenIDs", ','))
			{
				XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", num2.ToString());
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					if (num > xelementAttributeInt)
					{
						num = xelementAttributeInt;
					}
				}
			}
			return num;
		}

		public static bool CanTodayTakeFuBen(int fuBenID)
		{
			FuBenData fuBenData = Global.GetFuBenData(fuBenID);
			if (fuBenData == null)
			{
				return true;
			}
			int num;
			int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			int num2;
			int fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(fuBenID, out num2);
			return (fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2);
		}

		public static bool CanTodayTakeAnyFuBen()
		{
			if (Global.Data.roleData.FuBenDataList == null)
			{
				return true;
			}
			for (int i = 0; i < Global.Data.roleData.FuBenDataList.Count; i++)
			{
				int num;
				int fuBenEnterNum = Global.GetFuBenEnterNum(Global.Data.roleData.FuBenDataList[i], out num);
				int num2;
				int fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(Global.Data.roleData.FuBenDataList[i].FuBenID, out num2);
				if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2))
				{
					return true;
				}
			}
			return false;
		}

		public static int GetCurrentFuBenMapLimitTimeMinutes()
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "MapCode", Global.Data.roleData.MapCode.ToString());
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "MaxTime");
		}

		public static int GetFuBenMapMinSaoDangTime(int mapCode)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "MapCode", mapCode.ToString());
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "MinSaoDangTime");
		}

		public static string GetFuBenNameByID(int fuBenID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return string.Empty;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", fuBenID.ToString());
			if (xelement == null)
			{
				return string.Empty;
			}
			return Global.GetXElementAttributeStr(xelement, "CopyName");
		}

		public static XElement GetFuBenMapElement(int mapCode, int copyID = -1)
		{
			XElement result = null;
			XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			if (mapCode >= 0 && copyID > 0)
			{
				result = Global.GetXElement(gameResXml, "Copy", "MapCode", copyID.ToString());
			}
			if (copyID >= 0 && mapCode <= 0)
			{
				result = Global.GetXElement(gameResXml, "Copy", "CopyID", copyID.ToString());
			}
			else if (mapCode >= 0 && copyID <= 0)
			{
				result = Global.GetXElement(gameResXml, "Copy", "MapCode", mapCode.ToString());
			}
			return result;
		}

		public static XElement GetFallPackageElement(int fallID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Activity/MonsterGoodsList.xml");
			if (gameResXml == null)
			{
				return null;
			}
			return Global.GetXElement(gameResXml, "Goods", "ID", fallID.ToString());
		}

		public static XElement GetExcellencePropertyRandomElement(int fallID)
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/ExcellencePropertyRandom.xml");
			if (isolateResXml == null)
			{
				return null;
			}
			return Global.GetXElement(isolateResXml, "ExcellencePropertyList", "ID", fallID.ToString());
		}

		public static bool CanGetFuBenMapAwards()
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "MapCode", Global.Data.roleData.MapCode.ToString());
			if (xelement == null)
			{
				return false;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsIDs");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Moneyaward");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Experienceaward");
			return !string.IsNullOrEmpty(xelementAttributeStr) || xelementAttributeInt2 > 0 || xelementAttributeInt2 > 0;
		}

		public static bool CanTodayEnterAnyFuBen()
		{
			FuBenData fuBenData = Global.GetFuBenData(20);
			int num = -1;
			int num2;
			int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num2);
			int fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(20, out num);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num < 0 || num2 < num))
			{
				return Global.CanTakFuBenByLevel(20);
			}
			fuBenData = Global.GetFuBenData(21);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num2);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(21, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num < 0 || num2 < num))
			{
				return Global.CanTakFuBenByLevel(21);
			}
			fuBenData = Global.GetFuBenData(22);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num2);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(22, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num < 0 || num2 < num))
			{
				return Global.CanTakFuBenByLevel(22);
			}
			fuBenData = Global.GetFuBenData(23);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num2);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(23, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num < 0 || num2 < num))
			{
				return Global.CanTakFuBenByLevel(23);
			}
			fuBenData = Global.GetFuBenData(24);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num2);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(24, out num2);
			return (fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num < 0 || num2 < num) && Global.CanTakFuBenByLevel(24);
		}

		public static string GetFuBenNumInfoStr()
		{
			string text = string.Empty;
			FuBenData fuBenData = Global.GetFuBenData(20);
			int num;
			int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			int num2;
			int fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(20, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2))
			{
				text += StringUtil.substitute(Global.GetLang("★莫邪洞窟副本剩余{0}次\r\n"), new object[]
				{
					fuBenMaxNumOfDay
				});
			}
			fuBenData = Global.GetFuBenData(21);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(21, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2))
			{
				text += StringUtil.substitute(Global.GetLang("★剑冢密室副本剩余{0}次\r\n"), new object[]
				{
					fuBenMaxNumOfDay
				});
			}
			fuBenData = Global.GetFuBenData(22);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(22, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2))
			{
				text += StringUtil.substitute(Global.GetLang("★浊世魔窟副本剩余{0}次\r\n"), new object[]
				{
					fuBenMaxNumOfDay
				});
			}
			fuBenData = Global.GetFuBenData(23);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(23, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2))
			{
				text += StringUtil.substitute(Global.GetLang("★仙踪大殿副本剩余{0}次\r\n"), new object[]
				{
					fuBenMaxNumOfDay
				});
			}
			fuBenData = Global.GetFuBenData(24);
			fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
			fuBenMaxNumOfDay = Global.GetFuBenMaxNumOfDay(24, out num2);
			if ((fuBenMaxNumOfDay < 0 || fuBenEnterNum < fuBenMaxNumOfDay) && (num2 < 0 || num < num2))
			{
				text += StringUtil.substitute(Global.GetLang("★天绝魔狱副本剩余{0}次\r\n"), new object[]
				{
					fuBenMaxNumOfDay
				});
			}
			return text;
		}

		public static string GetFuBenRewardTypeStr(int type)
		{
			switch (type)
			{
			case 1:
				return Global.GetLang("经验");
			case 2:
				return Global.GetLang("金币");
			case 3:
				return Global.GetLang("材料");
			case 4:
				return Global.GetLang("装备");
			case 5:
				return Global.GetLang("晶石");
			case 6:
				return Global.GetLang("成就");
			case 7:
				return Global.GetLang("魔晶");
			case 8:
				return Global.GetLang("声望");
			case 9:
				return Global.GetLang("绑金");
			default:
				return Global.GetLang(string.Empty);
			}
		}

		public static CopyScoreDataInfo GetCopyScoreDataInfo(int copyID = -1, int score = -1)
		{
			CopyScoreDataInfo copyScoreDataInfo = null;
			CopyScoreDataInfo copyScoreDataInfo2 = null;
			List<CopyScoreDataInfo> list = null;
			if (Global.Data.CopyScoreDataInfoList == null)
			{
				Global.Data.CopyScoreDataInfoList = new Dictionary<int, List<CopyScoreDataInfo>>();
			}
			else if (Global.Data.CopyScoreDataInfoList.Count > 0)
			{
				Global.Data.CopyScoreDataInfoList.TryGetValue(copyID, ref list);
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						CopyScoreDataInfo copyScoreDataInfo3 = list[i];
						if (copyScoreDataInfo3.MinScore <= score && copyScoreDataInfo3.MaxScore >= score)
						{
							copyScoreDataInfo = copyScoreDataInfo3;
							return copyScoreDataInfo;
						}
					}
				}
			}
			try
			{
				XElement gameResXml = Global.GetGameResXml("Config/FuBenPingFen.Xml");
				if (gameResXml == null)
				{
					return null;
				}
				foreach (XElement xelement in gameResXml.Elements("CopyScoreInfos"))
				{
					if (xelement != null)
					{
						int nCopyMapID = Global.GetXElementAttributeInt(xelement, "ID");
						int MinScore = Global.GetXElementAttributeInt(xelement, "MinFen");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MaxFen");
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "PingFenName");
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ExpXiShu");
						int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "JinBiXiShu");
						int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "GoodsList");
						if (!Global.Data.CopyScoreDataInfoList.TryGetValue(nCopyMapID, ref list))
						{
							list = new List<CopyScoreDataInfo>();
							Global.Data.CopyScoreDataInfoList.Add(nCopyMapID, list);
						}
						copyScoreDataInfo = list.Find((CopyScoreDataInfo x) => x.CopyMapID == nCopyMapID && x.MinScore == MinScore);
						if (copyScoreDataInfo == null)
						{
							copyScoreDataInfo = new CopyScoreDataInfo
							{
								CopyMapID = nCopyMapID,
								MinScore = MinScore,
								MaxScore = xelementAttributeInt,
								ScoreName = xelementAttributeStr,
								ExpModulus = xelementAttributeInt2,
								MoneyModulus = xelementAttributeInt3,
								FallPacketID = xelementAttributeInt4
							};
							list.Add(copyScoreDataInfo);
						}
						if (copyID == nCopyMapID)
						{
							if (copyScoreDataInfo.MinScore <= score && copyScoreDataInfo.MaxScore >= score)
							{
								break;
							}
							if (copyScoreDataInfo2 == null || copyScoreDataInfo2.MaxScore < copyScoreDataInfo.MaxScore)
							{
								copyScoreDataInfo2 = copyScoreDataInfo;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			if (copyScoreDataInfo == null)
			{
				return copyScoreDataInfo2;
			}
			return copyScoreDataInfo;
		}

		public static int GetFuBenTabNeedLevel(int fuBenTabID)
		{
			int result = -1;
			if (Global.Data.fuBenNeedLevelDict == null)
			{
				Global.Data.fuBenNeedLevelDict = ConfigSystemParam.GetSystemParamIntDict1ByName("RiChangFuBenNeed", '|', ',');
			}
			int[] array = null;
			if (Global.Data.fuBenNeedLevelDict.TryGetValue(fuBenTabID, ref array))
			{
				result = array[1];
			}
			return result;
		}

		public static int GetFuBenTabNeedCompleteTask(int fuBenTabID)
		{
			int result = -1;
			if (Global.Data.fuBenNeedDict == null)
			{
				Global.Data.fuBenNeedDict = ConfigSystemParam.GetSystemParamIntDict1ByName("FuBenNeed", '|', ',');
			}
			int[] array = null;
			if (Global.Data.fuBenNeedDict.TryGetValue(fuBenTabID, ref array))
			{
				result = array[1];
			}
			return result;
		}

		public static int GetNameColorIndexByPKPoints(int pkPoints)
		{
			if (pkPoints >= 0 && pkPoints <= 99)
			{
				return 0;
			}
			if (pkPoints >= 100 && pkPoints <= 199)
			{
				return 1;
			}
			return 2;
		}

		public static int GetLaoFangMapCode()
		{
			if (Global.LaoFangMapCode == -1)
			{
				Global.LaoFangMapCode = (int)ConfigSystemParam.GetSystemParamIntByName("LaoFangMapCode");
			}
			return Global.LaoFangMapCode;
		}

		public static bool CanBringToLaoFang()
		{
			if (Global.Data.roleData.MapCode == Global.GetLaoFangMapCode())
			{
				return false;
			}
			if (Global.Data.roleData.PKPoint < Global.MinEnterJailPKPoints)
			{
				return false;
			}
			int laoFangMapCode = Global.GetLaoFangMapCode();
			return laoFangMapCode != -1;
		}

		public static string GetPKValueName(int pkv)
		{
			string result = string.Empty;
			if (pkv >= 3 && pkv < 10)
			{
				result = Global.GetLang("恶人");
			}
			else if (pkv >= 10 && pkv < 50)
			{
				result = Global.GetLang("杀手");
			}
			else if (pkv >= 50 && pkv < 100)
			{
				result = Global.GetLang("魔头");
			}
			else if (pkv >= 100)
			{
				result = Global.GetLang("魔王");
			}
			return result;
		}

		public static bool IsPurpleName(RoleData roleData)
		{
			long correctLocalTime = Global.GetCorrectLocalTime();
			return correctLocalTime - roleData.StartPurpleNameTicks < Global.MaxPurpleNameTicks;
		}

		public static XElement GetYaBiaoXmlNodeByID(int yaBiaoID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Yabiao.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Item", "ID", yaBiaoID.ToString());
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static void GetYaBiaoReward(int yaBiaoID, out int yinLiang, out int experience, out int yaJin)
		{
			yinLiang = 0;
			experience = 0;
			yaJin = 0;
			XElement yaBiaoXmlNodeByID = Global.GetYaBiaoXmlNodeByID(yaBiaoID);
			if (yaBiaoXmlNodeByID == null)
			{
				return;
			}
			yinLiang = Global.GetXElementAttributeInt(yaBiaoXmlNodeByID, "RewardYL");
			experience = Global.GetXElementAttributeInt(yaBiaoXmlNodeByID, "RewardExp");
			yaJin = Global.GetXElementAttributeInt(yaBiaoXmlNodeByID, "YaJin");
		}

		public static int GetTouBaoYinPiaoNum(int yinLiang)
		{
			return yinLiang / 5;
		}

		public static int GetMaxDayYaBiaoNum()
		{
			return 1 + (int)Global.ProcessMonthVIP();
		}

		public static int GetTodayYaBiaoNum()
		{
			if (Global.Data.roleData.MyYaBiaoData == null)
			{
				return 0;
			}
			int dayOfYear = DateTime.Now.DayOfYear;
			if (Global.Data.roleData.MyYaBiaoData.YaBiaoDayID != dayOfYear)
			{
				return 0;
			}
			return Global.Data.roleData.MyYaBiaoData.YaBiaoNum;
		}

		public static string GetYaBiaoName(int yaBiaoID)
		{
			if (yaBiaoID == 1)
			{
				return Global.GetLang("驮镖毛驴");
			}
			if (yaBiaoID == 2)
			{
				return Global.GetLang("驮镖白马");
			}
			if (yaBiaoID == 3)
			{
				return Global.GetLang("驮镖骆驼");
			}
			return string.Empty;
		}

		public static string GetLianZhanBufferName(int bufferVal)
		{
			if (bufferVal < 0 || bufferVal >= Global.LianZhanBufferNames.Length)
			{
				return string.Empty;
			}
			return Global.LianZhanBufferNames[bufferVal];
		}

		public static int GetChongZhiSecondTaskID()
		{
			return (int)ConfigSystemParam.GetSystemParamIntByName("ChongZhiSecondTaskID");
		}

		public static int GetChongZhiSecondTaskMinLevel()
		{
			return (int)ConfigSystemParam.GetSystemParamIntByName("ChongZhiSecondTaskMinLevel");
		}

		public static int GetChongZhiSecondTaskMaxLevel()
		{
			return (int)ConfigSystemParam.GetSystemParamIntByName("ChongZhiSecondTaskMaxLevel");
		}

		public static bool HavingChongZhiSecondTask(int taskID)
		{
			if (Global.Data.roleData.TaskDataList == null)
			{
				return false;
			}
			for (int i = 0; i < Global.Data.roleData.TaskDataList.Count; i++)
			{
				if (Global.Data.roleData.TaskDataList[i].DoingTaskID == taskID)
				{
					return true;
				}
			}
			return false;
		}

		public static void JugeCompleteChongZhiSecondTask(int taskID)
		{
			if (Global.GetChongZhiSecondTaskID() != taskID)
			{
				return;
			}
			if (Global.Data.roleData.CZTaskID == taskID)
			{
				return;
			}
			Global.Data.roleData.CZTaskID = taskID;
		}

		public static int GetAllQualityDefensePercent(int allQualityIndex)
		{
			if (allQualityIndex < 0 || allQualityIndex >= Global.AllQualityDefensePercents.Length)
			{
				return 0;
			}
			return Global.AllQualityDefensePercents[allQualityIndex];
		}

		public static int GetAllForgeLevelAttackPercent(int allForgeLevelIndex)
		{
			if (allForgeLevelIndex < 0 || allForgeLevelIndex >= Global.AllForgeLevelAttackPercents.Length)
			{
				return 0;
			}
			return Global.AllForgeLevelAttackPercents[allForgeLevelIndex];
		}

		public static int GetAllJewelLevelOccupPercent(int allJewelLevelIndex)
		{
			if (allJewelLevelIndex < 0 || allJewelLevelIndex >= Global.AllJewelLevelOccupPercents.Length)
			{
				return 0;
			}
			return Global.AllJewelLevelOccupPercents[allJewelLevelIndex];
		}

		public static int GetAllJewelLevelOccupProp(int occupation)
		{
			if (occupation == 0)
			{
				return 13;
			}
			if (occupation == 1)
			{
				return 15;
			}
			return 13;
		}

		public static string GetAllJewelLevelOccupPropName(int occupation)
		{
			return Global.GetLang("生命上限");
		}

		public static int GetAllJewelLevelOtherPercent(int allJewelLevelIndex)
		{
			if (allJewelLevelIndex < 0 || allJewelLevelIndex >= Global.AllJewelLevelOtherPercents.Length)
			{
				return 0;
			}
			return Global.AllJewelLevelOtherPercents[allJewelLevelIndex];
		}

		public static bool InLimitTimeRange(string pubStartTime, string pubEndTime)
		{
			if (!string.IsNullOrEmpty(pubStartTime) && !string.IsNullOrEmpty(pubEndTime))
			{
				long num = Global.SafeConvertToTicks(pubStartTime);
				long num2 = Global.SafeConvertToTicks(pubEndTime);
				long correctLocalTime = Global.GetCorrectLocalTime();
				if (correctLocalTime < num || correctLocalTime > num2)
				{
					return false;
				}
			}
			return true;
		}

		public static int RecalcNeedYinLiang(int needYinLiang)
		{
			int halfYinLiangPeriod = Global.Data.roleData.HalfYinLiangPeriod;
			if (halfYinLiangPeriod <= 0)
			{
				return needYinLiang;
			}
			return needYinLiang / 2;
		}

		public static string FormatRoleID(int nRoleTargetID)
		{
			if (Global.Data.roleData.RoleID == nRoleTargetID)
			{
				return "Leader";
			}
			return StringUtil.substitute("Role_{0}", new object[]
			{
				nRoleTargetID
			});
		}

		public static string FormatRoleName(RoleData roleData)
		{
			return Global.FormatRoleName(roleData.ZoneID, roleData.RoleName);
		}

		public static string FormatRoleName(RoleDataMini roleDataMini)
		{
			return Global.FormatRoleName(roleDataMini.ZoneID, roleDataMini.RoleName);
		}

		public static string FormatShowName(RoleData roleData, byte showPTName = 0)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			bool flag = false;
			if (Global.GetNowServerIsZhuTiFu(roleData.ZoneID, out ztBuffServerInfo) && Global.IsKuaFuMap(Global.Data.roleData.MapCode, true))
			{
				flag = true;
			}
			if (!flag)
			{
				if (showPTName == 1 && SceneUIClasses.RebornMap.IsTheScene())
				{
					return IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(roleData.PTID, true) + Global.FormatRoleNameZoneid(roleData.ZoneID, roleData.RoleName, 0, 0);
				}
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || Global.GetMapSceneUIClass() == SceneUIClasses.Comp || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu || SceneUIClasses.RebornMap.IsTheScene())
				{
					return Global.FormatRoleNameZoneid(roleData.ZoneID, roleData.RoleName, 0, 0);
				}
				if (Global.IsBattleMap())
				{
					if (roleData.BattleWhichSide == 1)
					{
						return Global.GetLang("教团");
					}
					if (roleData.BattleWhichSide == 2)
					{
						return Global.GetLang("盟军");
					}
				}
				return Global.FormatRoleName(roleData.ZoneID, roleData.RoleName);
			}
			else
			{
				if (showPTName == 1 && SceneUIClasses.RebornMap.IsTheScene())
				{
					return IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(roleData.PTID, true) + Global.FormatRoleNameZoneid(roleData.ZoneID, roleData.RoleName, 0, 0);
				}
				if (!string.IsNullOrEmpty(ztBuffServerInfo.strServerName))
				{
					return Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, roleData.RoleName, 0);
				}
				return Global.FormatRoleName(roleData.ZoneID, roleData.RoleName);
			}
		}

		public static string FormatRoleNameByChat(RoleData roleData)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(roleData.ZoneID, out ztBuffServerInfo) && Global.IsKuaFuMap(roleData.MapCode, true) && !string.IsNullOrEmpty(ztBuffServerInfo.strServerName))
			{
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					return IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(roleData.PTID, true) + Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, roleData.RoleName, 0);
				}
				return Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, roleData.RoleName, 0);
			}
			else
			{
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu)
				{
					return Global.FormatRoleNameZoneid(roleData.ZoneID, roleData.RoleName, 0, 0);
				}
				if (Global.IsInShiLiZhengBaMap())
				{
					return Global.FormatRoleNameZoneid(roleData.ZoneID, roleData.RoleName, 0, 0);
				}
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					return IConfigbase<ConfigChannelName>.Instance.GetPTNameByPTID(roleData.PTID, true) + Global.FormatRoleNameZoneid(roleData.ZoneID, roleData.RoleName, 0, 0);
				}
				return Global.FormatRoleName(roleData.ZoneID, roleData.RoleName);
			}
		}

		public static string FormatRoleNameZoneid(int zoneID, string roleName, byte NameType = 0, byte CheckKuaFu = 0)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(zoneID, out ztBuffServerInfo))
			{
				if (CheckKuaFu == 0)
				{
					if (Global.IsKuaFuMap(Global.Data.roleData.MapCode, true) && !string.IsNullOrEmpty(ztBuffServerInfo.strServerName))
					{
						return Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, roleName, 0);
					}
				}
				else if (!string.IsNullOrEmpty(ztBuffServerInfo.strServerName))
				{
					return Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, roleName, 0);
				}
			}
			else if (NameType == 0)
			{
				if (zoneID != 0)
				{
					return string.Format(Global.GetLang("[{0}区]{1}"), zoneID, roleName);
				}
			}
			else if (NameType == 1 && zoneID != 0)
			{
				return string.Format("S{0}.{1}", zoneID, roleName);
			}
			return roleName;
		}

		public static string FormatRoleNameZhuTiFu(string ServerName, string RoleName, byte NameType = 0)
		{
			return "[" + ServerName + "]" + RoleName;
		}

		public static string GetZoneName(int zoneId)
		{
			ZtBuffServerInfo ztBuffServerInfo = null;
			if (Global.GetNowServerIsZhuTiFu(zoneId, out ztBuffServerInfo) && !string.IsNullOrEmpty(ztBuffServerInfo.strServerName))
			{
				return ztBuffServerInfo.strServerName;
			}
			return string.Format(Global.GetLang("[{0}区]"), zoneId);
		}

		public static string FormatSeflRoleNameShiLiZoon()
		{
			string result = string.Empty;
			if (Global.Data.roleData.CompType < 1)
			{
				result = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
			}
			else
			{
				MUCompLevel selfCompLevel = ShiLiData.GetSelfCompLevel();
				if (selfCompLevel == null)
				{
					result = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
				}
				else if (selfCompLevel.Level < 1)
				{
					result = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0);
				}
				else
				{
					result = Global.FormatRoleNameZoneid(Global.Data.roleData.ZoneID, Global.Data.roleData.RoleName, 0, 0) + "-" + selfCompLevel.ChatName;
				}
			}
			return result;
		}

		public static string FormatRoleNameCLine(int zoneID, string roleName)
		{
			if (zoneID != 0)
			{
				return string.Format(Global.GetLang("S{0} 区\n{1}"), zoneID, roleName);
			}
			return roleName;
		}

		public static string FormatShowName(RoleDataMini roleDataMini)
		{
			if (Global.IsBattleMap())
			{
				if (roleDataMini.BattleWhichSide == 1)
				{
					return Global.GetLang("教团");
				}
				if (roleDataMini.BattleWhichSide == 2)
				{
					return Global.GetLang("盟军");
				}
			}
			return Global.FormatRoleName(roleDataMini.ZoneID, roleDataMini.RoleName);
		}

		public static string FormatRoleName(int zoneID, string roleName)
		{
			return roleName;
		}

		public static string FormatRoleLevel(RoleData roleData)
		{
			if (Global.IsBattleMap())
			{
				return string.Empty;
			}
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				return StringUtil.substitute(Global.GetLang("重生{0}"), new object[]
				{
					roleData.RebornLevel.ToString()
				});
			}
			return StringUtil.substitute(Global.GetLang("LV:{0} [{1}转]"), new object[]
			{
				roleData.Level,
				roleData.ChangeLifeCount
			});
		}

		public static string FormatRoleLevel(RoleDataMini roleDataMini)
		{
			if (Global.IsBattleMap())
			{
				return string.Empty;
			}
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				return StringUtil.substitute(Global.GetLang("重生{0}"), new object[]
				{
					roleDataMini.RebornLevel.ToString()
				});
			}
			return StringUtil.substitute(Global.GetLang("LV:{0} [{1}转]"), new object[]
			{
				roleDataMini.Level,
				roleDataMini.ChangeLifeCount
			});
		}

		public static string FormatLevel(int zhuanSheng, int level)
		{
			if (zhuanSheng < 0 && level < 0)
			{
				return Global.GetLang("0转1级");
			}
			if (zhuanSheng < 0)
			{
				return StringUtil.substitute(Global.GetLang("{0}级"), new object[]
				{
					level
				});
			}
			if (level < 0)
			{
				return StringUtil.substitute(Global.GetLang("{0}转"), new object[]
				{
					zhuanSheng
				});
			}
			return StringUtil.substitute(Global.GetLang("{0}转{1}级"), new object[]
			{
				zhuanSheng,
				level
			});
		}

		public static string FormatBangHuiName(int zoneID, string bangHuiName)
		{
			return bangHuiName;
		}

		public static int CreateBangHuiNeedTongQian
		{
			get
			{
				string text = Global.MUGetZhanMengParamsAt(2);
				if (text != null)
				{
					return int.Parse(text);
				}
				return 500000;
			}
		}

		public static int CreateBangHuiNeedLevel
		{
			get
			{
				string text = Global.MUGetZhanMengParamsAt(1);
				if (text != null)
				{
					return int.Parse(text);
				}
				return 40;
			}
		}

		public static int CreateBangHuiNeedDaoju
		{
			get
			{
				string text = Global.MUGetZhanMengParamsAt(3);
				if (text != null)
				{
					return int.Parse(text);
				}
				return (int)ConfigSystemParam.GetSystemParamIntByName("CreateBangHuiGoodsID");
			}
		}

		public static int CreateBangHuiZhuansheng
		{
			get
			{
				string text = Global.MUGetZhanMengParamsAt(0);
				if (text != null)
				{
					return int.Parse(text);
				}
				return 0;
			}
		}

		public static string MUGetZhanMengParamsAt(int idx)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ZhanMengNeed", '|');
			if (idx < systemParamStringArrayByName.Length)
			{
				return systemParamStringArrayByName[idx];
			}
			return null;
		}

		public static bool IsHavingBangHui()
		{
			return Global.IsHavingBangHui(Global.Data.roleData);
		}

		public static bool IsHavingBangHui(RoleData roleData)
		{
			return roleData.Faction > 0 && !string.IsNullOrEmpty(roleData.BHName);
		}

		public static string GetBHZhiWu(int bhZhiWu)
		{
			if (bhZhiWu <= 0)
			{
				return Global.GetLang("成员");
			}
			if (bhZhiWu == 1)
			{
				return Global.GetLang("首领");
			}
			if (bhZhiWu == 2)
			{
				return Global.GetLang("副首领");
			}
			if (bhZhiWu == 3)
			{
				return Global.GetLang("左将军");
			}
			if (bhZhiWu == 4)
			{
				return Global.GetLang("右将军");
			}
			return Global.GetLang("未知");
		}

		public static bool IsBangHuiLeader(RoleData roleData, int bhid)
		{
			return Global.IsHavingBangHui(roleData) && roleData.Faction == bhid && roleData.BHZhiWu == 1;
		}

		public static bool IsInBangHui(int bhid)
		{
			return Global.IsHavingBangHui() && Global.Data.roleData.Faction == bhid;
		}

		public static bool IsBangHuiLeader(RoleData roleData, BangHuiDetailData bangHuiDetailData)
		{
			return bangHuiDetailData.BHID == roleData.Faction && bangHuiDetailData.BZRoleID == roleData.RoleID;
		}

		public static int GetBangHuiZhiWuByRoleID(RoleData roleData, BangHuiDetailData bangHuiDetailData)
		{
			return Global.GetBangHuiZhiWuByRoleID(roleData.RoleID, bangHuiDetailData);
		}

		public static int GetBangHuiZhiWuByRoleID(int roleID, BangHuiDetailData bangHuiDetailData)
		{
			if (bangHuiDetailData.MgrItemList == null)
			{
				return 0;
			}
			for (int i = 0; i < bangHuiDetailData.MgrItemList.Count; i++)
			{
				if (bangHuiDetailData.MgrItemList[i].RoleID == roleID)
				{
					return bangHuiDetailData.MgrItemList[i].BHZhiwu;
				}
			}
			return 0;
		}

		public static BangHuiMgrItemData GetBangHuiMgrItemDataByZhiWu(int zhiWu, BangHuiDetailData bangHuiDetailData)
		{
			if (bangHuiDetailData.MgrItemList == null)
			{
				return null;
			}
			for (int i = 0; i < bangHuiDetailData.MgrItemList.Count; i++)
			{
				if (bangHuiDetailData.MgrItemList[i].BHZhiwu == zhiWu)
				{
					return bangHuiDetailData.MgrItemList[i];
				}
			}
			return null;
		}

		public static bool IsHuangDi(RoleData roleData)
		{
			if (roleData.Faction <= 0)
			{
				return false;
			}
			if (roleData.BHZhiWu != 1)
			{
				return false;
			}
			BangHuiLingDiItemData bhidbyLingDiID = Global.GetBHIDByLingDiID(2);
			return bhidbyLingDiID != null && bhidbyLingDiID.BHID == roleData.Faction;
		}

		public static bool IsWangZuLeader(RoleData roleData)
		{
			if (roleData.Faction <= 0)
			{
				return false;
			}
			if (roleData.BHZhiWu != 1)
			{
				return false;
			}
			BangHuiLingDiItemData bhidbyLingDiID = Global.GetBHIDByLingDiID(6);
			return bhidbyLingDiID != null && bhidbyLingDiID.BHID == roleData.Faction;
		}

		public static string MUGetZhanMengParamsAt(string paramname, int idx)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName(paramname, ',');
			if (idx < systemParamStringArrayByName.Length)
			{
				return systemParamStringArrayByName[idx];
			}
			return "-1";
		}

		public static int JuanZengJinBiShuLiang
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengJinBiJuanZeng", 0));
			}
		}

		public static int JuanZengJinBiHuoDeZhanGong
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengJinBiJuanZeng", 1));
			}
		}

		public static int JuanZengJinBiHuoDeZhanGongShangXian
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengJinBiJuanZeng", 3));
			}
		}

		public static int JuanZengZuanShiShuLiang
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengZuanShiJuanZeng", 0));
			}
		}

		public static int JuanZengZuanShiHuoDeZhanGongShangXian
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengZuanShiJuanZeng", 3));
			}
		}

		public static int JuanZengGoodsHuoDeZhanGong
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengGoodsJuanZeng", 0));
			}
		}

		public static int JuanZengGoodsHuoDeZhanGongShangXian
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengGoodsJuanZeng", 2));
			}
		}

		public static string[] JuanZengGoodsIds
		{
			get
			{
				return ConfigSystemParam.GetSystemParamStringArrayByName("ZhanMengGoods", ',');
			}
		}

		public static int ZhanMengChuShiZiJin
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengZiJin", 0));
			}
		}

		public static int ZhanMengWeiHuXiaoHao
		{
			get
			{
				return int.Parse(Global.MUGetZhanMengParamsAt("ZhanMengZiJin", 1));
			}
		}

		public static string GetWeekNums(int index)
		{
			if (index == 7)
			{
				index = 0;
			}
			string[] array = new string[]
			{
				Global.GetLang("周日"),
				Global.GetLang("周一"),
				Global.GetLang("周二"),
				Global.GetLang("周三"),
				Global.GetLang("周四"),
				Global.GetLang("周五"),
				Global.GetLang("周六")
			};
			return array[index];
		}

		public static int GetTotalLingDiNumByBHID(int bhid)
		{
			if (Global.Data.roleData.BangHuiLingDiItemsDict == null)
			{
				return 0;
			}
			int num = 0;
			foreach (KeyValuePair<int, BangHuiLingDiItemData> keyValuePair in Global.Data.roleData.BangHuiLingDiItemsDict)
			{
				if (keyValuePair.Value.BHID == bhid)
				{
					num++;
				}
			}
			return num;
		}

		public static BangHuiLingDiItemData GetBHIDByLingDiID(int lindDiID)
		{
			if (Global.Data.roleData.BangHuiLingDiItemsDict == null)
			{
				return null;
			}
			if (!Global.Data.roleData.BangHuiLingDiItemsDict.ContainsKey(lindDiID))
			{
				return null;
			}
			return Global.Data.roleData.BangHuiLingDiItemsDict[lindDiID];
		}

		public static int GetLingDiIDByMapCode2(int mapCode)
		{
			if (Global.LingDiIDs2MapCodes == null)
			{
				Global.LingDiIDs2MapCodes = ConfigSystemParam.GetSystemParamIntArrayByName("LingDiIDs2MapCodes", ',');
			}
			if (Global.LingDiIDs2MapCodes == null)
			{
				return 0;
			}
			int result = 0;
			for (int i = 0; i < Global.LingDiIDs2MapCodes.Length; i++)
			{
				if (Global.LingDiIDs2MapCodes[i] == mapCode)
				{
					result = i + 1;
					break;
				}
			}
			return result;
		}

		public static string GetNextLingDiZhanDateTime()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LingDiZhanWeekDays", ',');
			if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length <= 0)
			{
				return Global.GetLang("未知");
			}
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("LingDiZhanFightingDayTimes", '-');
			if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length != 2)
			{
				return Global.GetLang("未知");
			}
			string[] array = systemParamStringArrayByName[0].Split(new char[]
			{
				';'
			});
			if (array == null || array.Length != 2)
			{
				return Global.GetLang("未知");
			}
			DateTime now = DateTime.Now;
			int num = Global.SafeConvertToInt32(array[0]);
			int num2 = Global.SafeConvertToInt32(array[1]);
			int num3 = now.Hour * 60 + now.Minute;
			int num4 = num * 60 + num2;
			int index = systemParamIntArrayByName[0];
			int dayOfWeek = DateTime.Now.DayOfWeek;
			for (int i = 0; i < systemParamIntArrayByName.Length; i++)
			{
				if (num3 < num4)
				{
					if (systemParamIntArrayByName[i] >= dayOfWeek)
					{
						index = systemParamIntArrayByName[i];
						break;
					}
				}
				else if (systemParamIntArrayByName[i] > dayOfWeek)
				{
					index = systemParamIntArrayByName[i];
					break;
				}
			}
			return StringUtil.substitute(Global.GetLang("{0} {1}"), new object[]
			{
				Global.GetLang(Global.GetWeekNums(index)),
				systemParamStringArrayByName[0]
			});
		}

		public static int CalcNeedKunLunJingNum(int clickNum)
		{
			if (clickNum <= 0)
			{
				return 0;
			}
			return (int)Math.Pow(2.0, (double)clickNum);
		}

		public static GoodsData AddBaoKuGoodsData(int goodsID, int forgeLevel, int quality, int binding, int gcount, int bornIndex)
		{
			int count = Global.Data.BaoKuJiangLiGoodsDataList.Count;
			GoodsData goodsData = new GoodsData();
			goodsData.Id = count;
			goodsData.GoodsID = goodsID;
			goodsData.Using = 0;
			goodsData.Forge_level = forgeLevel;
			goodsData.Starttime = "1900-01-01 12:00:00";
			goodsData.Endtime = "1900-01-01 12:00:00";
			goodsData.Site = 0;
			goodsData.Quality = quality;
			goodsData.Props = string.Empty;
			goodsData.GCount = gcount;
			goodsData.Binding = binding;
			goodsData.Jewellist = string.Empty;
			goodsData.BagIndex = 0;
			goodsData.SaleMoney1 = 0;
			goodsData.SaleYuanBao = 0;
			goodsData.SaleYinPiao = 0;
			goodsData.AddPropIndex = 0;
			goodsData.BornIndex = bornIndex;
			goodsData.Lucky = 0;
			goodsData.Strong = 0;
			Global.Data.BaoKuJiangLiGoodsDataList.Add(goodsData);
			return goodsData;
		}

		public static List<T> RandomSortList<T>(List<T> ListT)
		{
			Random random = new Random();
			List<T> list = new List<T>();
			for (int i = 0; i < list.Count; i++)
			{
				T t = list[i];
				int num = random.Next(0, list.Count);
				list.Insert(num, t);
			}
			return list;
		}

		public static int GetMaxHavingSheLiZhiYuanSecs()
		{
			if (Global.MaxHavingSheLiZhiYuanSecs <= 0)
			{
				Global.MaxHavingSheLiZhiYuanSecs = (int)ConfigSystemParam.GetSystemParamIntByName("MaxHavingSheLiZhiYuanSecs");
				Global.MaxHavingSheLiZhiYuanSecs *= 1000;
			}
			return Global.MaxHavingSheLiZhiYuanSecs;
		}

		public static string GetNextHuangChengZhanDateTime()
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HuangChengZhanWeekDays", ',');
			if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length <= 0)
			{
				return Global.GetLang("未知");
			}
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("HuangChengZhanFightingDayTimes", '-');
			if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length != 2)
			{
				return Global.GetLang("未知");
			}
			string[] array = systemParamStringArrayByName[0].Split(new char[]
			{
				';'
			});
			if (array == null || array.Length != 2)
			{
				return Global.GetLang("未知");
			}
			DateTime now = DateTime.Now;
			int num = Global.SafeConvertToInt32(array[0]);
			int num2 = Global.SafeConvertToInt32(array[1]);
			int num3 = now.Hour * 60 + now.Minute;
			int num4 = num * 60 + num2;
			int index = systemParamIntArrayByName[0];
			int dayOfWeek = DateTime.Now.DayOfWeek;
			for (int i = 0; i < systemParamIntArrayByName.Length; i++)
			{
				if (num3 < num4)
				{
					if (systemParamIntArrayByName[i] >= dayOfWeek)
					{
						index = systemParamIntArrayByName[i];
						break;
					}
				}
				else if (systemParamIntArrayByName[i] > dayOfWeek)
				{
					index = systemParamIntArrayByName[i];
					break;
				}
			}
			return StringUtil.substitute(Global.GetLang("{0} {1}"), new object[]
			{
				Global.GetLang(Global.GetWeekNums(index)),
				systemParamStringArrayByName[0]
			});
		}

		public static bool CanShowRoleHeadPic(bool canShow)
		{
			if (!canShow)
			{
				return false;
			}
			int lingDiIDByMapCode = Global.GetLingDiIDByMapCode2(Global.Data.roleData.MapCode);
			return (lingDiIDByMapCode < 2 || lingDiIDByMapCode > 5) && !Global.IsBattleMap() && canShow;
		}

		public static bool CanShowRolePet(bool canShow)
		{
			if (!canShow)
			{
				return false;
			}
			int lingDiIDByMapCode = Global.GetLingDiIDByMapCode2(Global.Data.roleData.MapCode);
			return (lingDiIDByMapCode < 2 || lingDiIDByMapCode > 5) && canShow;
		}

		public static XElement GetWindowConfigXmlNodeByID(int windowID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/WndCfg.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Window");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					if (windowID == xelementAttributeInt)
					{
						return xelement;
					}
				}
			}
			return null;
		}

		public static string GetCfgWndNameByID(int id)
		{
			return StringUtil.substitute("CfgWnd_{0}", new object[]
			{
				id
			});
		}

		public static List<string> GetIconCfgWndNameList()
		{
			if (Global.ListIconCfgWndName != null)
			{
				return Global.ListIconCfgWndName;
			}
			Global.ListIconCfgWndName = new List<string>();
			XElement gameResXml = Global.GetGameResXml("Config/WndCfg.Xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Window");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					if (xelementAttributeInt % 2 == 1)
					{
						Global.ListIconCfgWndName.Add(Global.GetCfgWndNameByID(xelementAttributeInt));
					}
				}
			}
			return Global.ListIconCfgWndName;
		}

		public static int TryGetInt(string str)
		{
			try
			{
				return ConvertExt.SafeConvertToInt32(str);
			}
			catch (Exception ex)
			{
				GError.AddErrMsg(StringUtil.substitute(Global.GetLang("初始化活动信息时字符串 {0} 转换成整数出错"), new object[]
				{
					str
				}));
				MUDebug.LogException(ex);
			}
			return 268435455;
		}

		public static string MergeProtocolFields(int cmdID, string[] fields)
		{
			string text = cmdID + string.Empty;
			for (int i = 0; i < fields.Length; i++)
			{
				if (text.Length > 0)
				{
					text += "\r\n";
				}
				text += StringUtil.substitute(Global.GetLang("fileds[{0}]: {1}"), new object[]
				{
					i,
					fields[i]
				});
			}
			return text;
		}

		public static bool IsNeedToQiZhenGe(int toBuyGoodID)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ToQiZhenGeGoodsIDs", ',');
			if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length <= 0)
			{
				return false;
			}
			for (int i = 0; i < systemParamIntArrayByName.Length; i++)
			{
				if (toBuyGoodID == systemParamIntArrayByName[i])
				{
					return true;
				}
			}
			return false;
		}

		public static string GetXapParamByName(string name, string defVal = "")
		{
			if (Global.RootParams.ContainsKey(name))
			{
				return Global.RootParams[name];
			}
			return defVal;
		}

		public static int GetUserLoginPort()
		{
			return Global.SafeConvertToInt32(Global.GetXapParamByName("loginport", "4502"));
		}

		public static int GetLineServerPort()
		{
			return Global.SafeConvertToInt32(Global.GetXapParamByName("lineserverport", "4504"));
		}

		public static int GetGameServerPort()
		{
			return Global.SafeConvertToInt32(Global.GetXapParamByName("gameport", "4503"));
		}

		public static bool IsYueNan()
		{
			string xapParamByName = Global.GetXapParamByName("country", string.Empty);
			return "vietnam" == xapParamByName;
		}

		public static bool IsHanGuo()
		{
			string xapParamByName = Global.GetXapParamByName("country", string.Empty);
			return "korea" == xapParamByName;
		}

		public static bool IsTaiWan()
		{
			string xapParamByName = Global.GetXapParamByName("country", string.Empty);
			return "taiwan" == xapParamByName;
		}

		public static int[] GetGoodsExecMagicNumberParamsList(int goodsId, string funcName = "")
		{
			List<int> list = new List<int>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsId);
			if (goodsXmlNodeByID == null)
			{
				return list.ToArray();
			}
			string execMagic = goodsXmlNodeByID.ExecMagic;
			if (funcName.Length > 0 && execMagic.IndexOf(funcName) < 0)
			{
				return list.ToArray();
			}
			int num = execMagic.IndexOf('(');
			int num2 = execMagic.IndexOf(')');
			if (num < 0 || num2 <= 0 || num2 - num < 1 || num + 1 >= execMagic.Length)
			{
				return list.ToArray();
			}
			string text = execMagic.Substring(num + 1, num2);
			string[] array = text.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(Global.SafeConvertToInt32(array[i]));
			}
			return list.ToArray();
		}

		public static double[] GetGoodsExecMagicdoubleParamsList(int goodsId, string funcName = "")
		{
			List<double> list = new List<double>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsId);
			if (goodsXmlNodeByID == null)
			{
				return list.ToArray();
			}
			string execMagic = goodsXmlNodeByID.ExecMagic;
			if (funcName.Length > 0 && execMagic.IndexOf(funcName) < 0)
			{
				return list.ToArray();
			}
			int num = execMagic.IndexOf('(');
			int num2 = execMagic.IndexOf(')');
			if (num < 0 || num2 <= 0 || num2 - num < 1 || num + 1 >= execMagic.Length)
			{
				return list.ToArray();
			}
			string text = execMagic.Substring(num + 1, num2);
			string[] array = text.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(Convert.ToDouble(array[i]));
			}
			return list.ToArray();
		}

		public static int GetGoodsBufferID(int goodsId)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsId);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			string execMagic = goodsXmlNodeByID.ExecMagic;
			int num = execMagic.IndexOf('(');
			string text = execMagic;
			if (num > 0)
			{
				text = execMagic.Substring(0, num);
			}
			int num2 = Global.GetBufferIdByFuncName(text);
			if (num2 == -1 && !string.IsNullOrEmpty(text))
			{
				if (text.Equals("DB_TIME_LIFE_NOSHOW"))
				{
					num2 = 37;
				}
				else if (text.Equals("DB_TIME_MAGIC_NOSHOW"))
				{
					num2 = 38;
				}
			}
			return num2;
		}

		public static int GetGoodsBindBufferID(int goodsId)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsId);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			string execMagic = goodsXmlNodeByID.ExecMagic;
			int num = execMagic.IndexOf('(');
			string funcName = execMagic;
			if (num > 0)
			{
				funcName = execMagic.Substring(0, num);
			}
			return Global.GetBufferIdByFuncName(funcName);
		}

		public static int GetBufferIdByFuncName(string funcName)
		{
			foreach (KeyValuePair<string, BufferItemTypes> keyValuePair in Global.GetFuncNameToBufferIdMap())
			{
				if (StringUtil.isEqualIgnoreCase(keyValuePair.Key, funcName))
				{
					Dictionary<string, BufferItemTypes>.Enumerator enumerator;
					KeyValuePair<string, BufferItemTypes> keyValuePair2 = enumerator.Current;
					return (int)keyValuePair2.Value;
				}
			}
			return -1;
		}

		public static Dictionary<string, BufferItemTypes> GetFuncNameToBufferIdMap()
		{
			if (Global.FuncNameToBufferIdMap == null)
			{
				Global.FuncNameToBufferIdMap = new Dictionary<string, BufferItemTypes>();
				Global.FuncNameToBufferIdMap["DB_ADD_MAXATTACKV"] = BufferItemTypes.TimeAddAttack;
				Global.FuncNameToBufferIdMap["DB_ADD_MAXMATTACKV"] = BufferItemTypes.TimeAddMAttack;
				Global.FuncNameToBufferIdMap["DB_ADD_MAXDSATTACKV"] = BufferItemTypes.TimeAddDSAttack;
				Global.FuncNameToBufferIdMap["DB_ADD_MAXDEFENSEV"] = BufferItemTypes.TimeAddDefense;
				Global.FuncNameToBufferIdMap["DB_ADD_MAXMDEFENSEV"] = BufferItemTypes.TimeAddMDefense;
				Global.FuncNameToBufferIdMap["DB_ADD_DBL_MONEY"] = BufferItemTypes.DblMoney;
				Global.FuncNameToBufferIdMap["DB_ADD_DBL_LINGLI"] = BufferItemTypes.DblLingLi;
				Global.FuncNameToBufferIdMap["DB_ADD_DBL_EXP"] = BufferItemTypes.DblExperience;
				Global.FuncNameToBufferIdMap["DB_ADD_THREE_EXP"] = BufferItemTypes.ThreeExperience;
				Global.FuncNameToBufferIdMap["DB_ADD_FIVE_EXP"] = BufferItemTypes.FiveExperience;
				Global.FuncNameToBufferIdMap["DB_ADD_MULTIEXP"] = BufferItemTypes.MutilExperience;
				Global.FuncNameToBufferIdMap["DB_ADD_MONTHVIP"] = BufferItemTypes.MonthVIP;
				Global.FuncNameToBufferIdMap["DB_ADD_SEASONVIP"] = BufferItemTypes.MonthVIP;
				Global.FuncNameToBufferIdMap["DB_ADD_HALFYEARVIP"] = BufferItemTypes.MonthVIP;
				Global.FuncNameToBufferIdMap["DB_TIME_LIFE_MAGIC"] = BufferItemTypes.TimeAddLifeMagic;
				Global.FuncNameToBufferIdMap["DB_NEW_ADD_ZHUFUTIME"] = BufferItemTypes.ZhuFu;
				Global.FuncNameToBufferIdMap["DB_ADD_ERGUOTOU"] = BufferItemTypes.ErGuoTou;
				Global.FuncNameToBufferIdMap["DB_ADD_LUCKYATTACKPERCENTTIMER"] = BufferItemTypes.MU_ADDLUCKYATTACKPERCENTTIMER;
				Global.FuncNameToBufferIdMap["DB_ADD_FATALATTACKPERCENTTIMER"] = BufferItemTypes.MU_ADDFATALATTACKPERCENTTIMER;
			}
			return Global.FuncNameToBufferIdMap;
		}

		public static Dictionary<string, string> GetYaoShiDiaoLuoForXiangZhi(int idXiangZi, List<int> yaoShiArr = null)
		{
			if (yaoShiArr != null)
			{
				yaoShiArr.Clear();
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(idXiangZi);
			if (goodsXmlNodeByID == null)
			{
				return dictionary;
			}
			string text = string.Empty;
			text = goodsXmlNodeByID.ExecMagic;
			int num = text.IndexOf('(');
			int num2 = text.IndexOf(')');
			if (num < 0 || num2 <= 0 || num2 - num < 1 || num + 1 >= text.Length)
			{
				return dictionary;
			}
			string text2 = text.Substring(num + 1, num2);
			string[] array = text2.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && !StringUtil.isWhitespace(array[i].get_Chars(0)))
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 2)
					{
						dictionary[array2[1]] = array2[0];
						if (yaoShiArr != null)
						{
							yaoShiArr.Add(Global.SafeConvertToInt32(array2[1]));
						}
					}
				}
			}
			return dictionary;
		}

		public static string GetBornIndexName(GoodsData goodsData)
		{
			string result = string.Empty;
			XElement gameResXml = Global.GetGameResXml("Config/BornName.Xml");
			if (gameResXml == null)
			{
				return result;
			}
			if (goodsData == null)
			{
				return result;
			}
			int bornIndex = goodsData.BornIndex;
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int num = (int)(100.0 * Global.GetXElementAttributeDouble(xelement, "MinProportion"));
				int num2 = (int)(100.0 * Global.GetXElementAttributeDouble(xelement, "MaxProportion"));
				if (bornIndex >= num && bornIndex <= num2)
				{
					result = Global.GetXElementAttributeStr(xelement, "Name");
					break;
				}
			}
			return result;
		}

		public static int GetBornIndexNameLevel(GoodsData goodsData)
		{
			int num = 0;
			XElement gameResXml = Global.GetGameResXml("Config/BornName.Xml");
			if (gameResXml == null)
			{
				return num;
			}
			int bornIndex = goodsData.BornIndex;
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int num2 = (int)(100.0 * Global.GetXElementAttributeDouble(xelement, "MinProportion"));
				int num3 = (int)(100.0 * Global.GetXElementAttributeDouble(xelement, "MaxProportion"));
				if (bornIndex >= num2 && bornIndex <= num3)
				{
					num = Global.GetXElementAttributeInt(xelement, "ID") - 1;
					break;
				}
			}
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		public static bool GetEquipBornUpdateParams(int oldBornIndex, out int bornUpdateNeedGoodsID, out int needNum, out int needYL)
		{
			bornUpdateNeedGoodsID = 0;
			needNum = 0;
			needYL = 0;
			XElement gameResXml = Global.GetGameResXml("Config/EquipBorn.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = null;
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement2 = xelementList[i];
				int num = (int)(100.0 * Global.GetXElementAttributeDouble(xelement2, "MinBorn"));
				int num2 = (int)(100.0 * Global.GetXElementAttributeDouble(xelement2, "MaxBorn"));
				if (oldBornIndex >= num && oldBornIndex <= num2)
				{
					xelement = xelement2;
					break;
				}
			}
			if (xelement == null)
			{
				return false;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Goods");
			string[] array = xelementAttributeStr.Split(new char[]
			{
				','
			});
			if (array.Length != 2)
			{
				return false;
			}
			bornUpdateNeedGoodsID = Global.SafeConvertToInt32(array[0]);
			needNum = Global.SafeConvertToInt32(array[1]);
			if (needNum <= 0)
			{
				return false;
			}
			needYL = Global.GetXElementAttributeInt(xelement, "YinLiang");
			return needYL >= 0;
		}

		public static GoodsData GetEmailFujianGoodsData(int goodsID, int forgeLevel, int quality, int binding, int gcount, int addPropIndex, int bornIndex, int lucky, int strong)
		{
			int count = Global.Data.EmailFujianGoodsDataList.Count;
			GoodsData goodsData = new GoodsData();
			goodsData.Id = count;
			goodsData.GoodsID = goodsID;
			goodsData.Using = 0;
			goodsData.Forge_level = forgeLevel;
			goodsData.Starttime = "1900-01-01 12:00:00";
			goodsData.Endtime = "1900-01-01 12:00:00";
			goodsData.Site = 0;
			goodsData.Quality = quality;
			goodsData.Props = string.Empty;
			goodsData.GCount = gcount;
			goodsData.Binding = binding;
			goodsData.Jewellist = string.Empty;
			goodsData.BagIndex = 0;
			goodsData.SaleMoney1 = 0;
			goodsData.SaleYuanBao = 0;
			goodsData.SaleYinPiao = 0;
			goodsData.AddPropIndex = addPropIndex;
			goodsData.BornIndex = bornIndex;
			goodsData.Lucky = lucky;
			goodsData.Strong = strong;
			Global.Data.EmailFujianGoodsDataList.Add(goodsData);
			return goodsData;
		}

		public static DateTime GetServerStartTime()
		{
			if (Global.Data == null || Global.Data.roleData == null)
			{
				return Global._ServerStartTime;
			}
			if (string.IsNullOrEmpty(Global.Data.roleData.KaiFuStartDay))
			{
				return Global._ServerStartTime;
			}
			DateTime serverStartTime = default(DateTime);
			string[] array = Global.Data.roleData.KaiFuStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 01:01:01", ref serverStartTime);
			Global._ServerStartTime = serverStartTime;
			return Global._ServerStartTime;
		}

		public static string GetHuodongTimeStr(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return "-1";
			}
			DateTime serverStartTime = Global.GetServerStartTime();
			DateTime dateTime;
			dateTime..ctor(serverStartTime.Year, serverStartTime.Month, serverStartTime.Day, hour, minu, sec, 0);
			dateTime = dateTime.AddDays((double)day);
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static DateTime GetHuodongTimeDateTime(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return new DateTime(2000, 1, 1);
			}
			DateTime serverStartTime = Global.GetServerStartTime();
			DateTime result;
			result..ctor(serverStartTime.Year, serverStartTime.Month, serverStartTime.Day, hour, minu, sec, 0);
			result = result.AddDays((double)day);
			return result;
		}

		public static DateTime GetServerMergeTime()
		{
			if (Global.Data == null || Global.Data.roleData == null)
			{
				return Global._ServerMergeTime;
			}
			if (string.IsNullOrEmpty(Global.Data.roleData.HefuStartDay))
			{
				return Global._ServerMergeTime;
			}
			DateTime serverMergeTime = default(DateTime);
			string[] array = Global.Data.roleData.HefuStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 00:00:00", ref serverMergeTime);
			Global._ServerMergeTime = serverMergeTime;
			return Global._ServerMergeTime;
		}

		public static string GetServerMergeHuodongTimeStr(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return "-1";
			}
			DateTime serverMergeTime = Global.GetServerMergeTime();
			DateTime dateTime;
			dateTime..ctor(serverMergeTime.Year, serverMergeTime.Month, serverMergeTime.Day + day, hour, minu, sec, 0);
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static bool IsHefuActivityEnd()
		{
			bool result = false;
			DateTime serverMergeHuodongTimeDateTime = Global.GetServerMergeHuodongTimeDateTime(7, 23, 59, 59);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime > serverMergeHuodongTimeDateTime)
			{
				result = true;
			}
			return result;
		}

		public static bool IsInHefuActivity()
		{
			if (HuoDongCommonFlag.IsActivityStateBegin(OpenActivityType.HeFu))
			{
				return true;
			}
			bool result = false;
			DateTime serverMergeTime = Global.GetServerMergeTime();
			DateTime serverMergeHuodongTimeDateTime = Global.GetServerMergeHuodongTimeDateTime(7, 23, 59, 59);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime >= serverMergeTime && correctDateTime <= serverMergeHuodongTimeDateTime)
			{
				result = true;
			}
			return result;
		}

		public static DateTime GetServerMergeHuodongTimeDateTime(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return new DateTime(2000, 1, 1);
			}
			return Global.GetServerMergeTime().Add(new TimeSpan(day, hour, minu, sec));
		}

		public static DateTime GetTodayDateTime(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return new DateTime(2000, 1, 1);
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime result;
			result..ctor(correctDateTime.Year, correctDateTime.Month, correctDateTime.Day + day, hour, minu, sec, 0);
			return result;
		}

		public static DateTime GetJieriTime()
		{
			if (Global.Data.roleData == null)
			{
				return Global._JieriTime;
			}
			if (string.IsNullOrEmpty(Global.Data.roleData.JieriStartDay))
			{
				return Global._JieriTime;
			}
			DateTime jieriTime = default(DateTime);
			string[] array = Global.Data.roleData.JieriStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 00:00:00", ref jieriTime);
			Global._JieriTime = jieriTime;
			return Global._JieriTime;
		}

		public static int GetJieriDaysNum()
		{
			if (Global.Data.roleData == null)
			{
				return 0;
			}
			return Global.Data.roleData.JieriDaysNum;
		}

		public static bool IsJieRiActivityEnd()
		{
			bool result = false;
			int jieriDaysNum = Global.GetJieriDaysNum();
			DateTime jieriTimeDateTime = Global.GetJieriTimeDateTime(jieriDaysNum, 0, 0, 0);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime > jieriTimeDateTime)
			{
				result = true;
			}
			return result;
		}

		public static bool IsInJieriActivity()
		{
			bool result = false;
			int jieriDaysNum = Global.GetJieriDaysNum();
			DateTime jieriTime = Global.GetJieriTime();
			DateTime jieriTimeDateTime = Global.GetJieriTimeDateTime(jieriDaysNum, 0, 0, 0);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime >= jieriTime && correctDateTime <= jieriTimeDateTime)
			{
				result = true;
			}
			return result;
		}

		public static string GetJieriTimeStr(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return "-1";
			}
			DateTime jieriTime = Global.GetJieriTime();
			DateTime dateTime;
			dateTime..ctor(jieriTime.Year, jieriTime.Month, jieriTime.Day + day, hour, minu, sec, 0);
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static DateTime GetJieriTimeDateTime(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return new DateTime(2000, 1, 1);
			}
			return Global.GetJieriTime().Add(new TimeSpan(day, hour, minu, sec));
		}

		public static DateTime GetYueduDazhunpanTime()
		{
			RoleData roleData = Global.Data.roleData;
			if (string.IsNullOrEmpty(Global.Data.roleData.YueduDazhunpanStartDay))
			{
				return Global._YueduDazhunpanTime;
			}
			DateTime yueduDazhunpanTime = default(DateTime);
			string[] array = Global.Data.roleData.YueduDazhunpanStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 01:01:01", ref yueduDazhunpanTime);
			Global._YueduDazhunpanTime = yueduDazhunpanTime;
			return Global._YueduDazhunpanTime;
		}

		public static int GetYueduDazhunpanDaysNum()
		{
			return Global.Data.roleData.YueduDazhunpanStartDayNum;
		}

		public static string GetYueduDazhunpanTimeStr(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return "-1";
			}
			DateTime yueduDazhunpanTime = Global.GetYueduDazhunpanTime();
			DateTime dateTime;
			dateTime..ctor(yueduDazhunpanTime.Year, yueduDazhunpanTime.Month, yueduDazhunpanTime.Day + day, hour, minu, sec, 0);
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		public static DateTime GetYueduDazhunpanTimeDateTime(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return new DateTime(2000, 1, 1);
			}
			DateTime yueduDazhunpanTime = Global.GetYueduDazhunpanTime();
			DateTime result;
			result..ctor(yueduDazhunpanTime.Year, yueduDazhunpanTime.Month, yueduDazhunpanTime.Day + day, hour, minu, sec, 0);
			return result;
		}

		public static bool IsActivityTipInShowTime(int type)
		{
			Global.LoadActivityTipCfg();
			if (Global.DictAcitvityMinLevle == null)
			{
				return false;
			}
			if (!Global.DictAcitvityMinLevle.ContainsKey(type))
			{
				return false;
			}
			int unionZhuanShengLevel = UIHelper.GetUnionZhuanShengLevel(Global.Data.roleData.Level, Global.Data.roleData.ChangeLifeCount);
			if (unionZhuanShengLevel < Global.DictAcitvityMinLevle[type].MinUnionLevel)
			{
				return false;
			}
			List<ActivityTime> timeList = Global.DictAcitvityMinLevle[type].TimeList;
			if (timeList.Count < 1)
			{
				return false;
			}
			for (int i = 0; i < timeList.Count; i++)
			{
				DateTime dateTimeLeft = timeList[i].DateTimeLeft;
				DateTime dateTimeRight = timeList[i].DateTimeRight;
				DateTime correctDateTime = Global.GetCorrectDateTime();
				long ticks = correctDateTime.Ticks;
				if (dateTimeLeft.Year != correctDateTime.Year)
				{
					Global.DictAcitvityMinLevle.Clear();
					Global.DictAcitvityMinLevle = null;
					return false;
				}
				if (ticks >= dateTimeLeft.Ticks && ticks < dateTimeRight.Ticks)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetActivityTipCountInShowTime(ActivityTime activityTime, out int id, out string name, out bool isKuafu)
		{
			id = -1;
			name = null;
			isKuafu = false;
			if (Global.Data.roleData == null)
			{
				return 0;
			}
			int num = 0;
			Global.LoadActivityTipCfg();
			if (Global.DictAcitvityMinLevle == null)
			{
				return 0;
			}
			foreach (KeyValuePair<int, ActivityTipConfigItem> keyValuePair in Global.DictAcitvityMinLevle)
			{
				if (Global.Data.roleData.CompletedMainTaskID >= keyValuePair.Value.MinMainTask)
				{
					int unionZhuanShengLevel = UIHelper.GetUnionZhuanShengLevel(Global.Data.roleData.Level, Global.Data.roleData.ChangeLifeCount);
					if (unionZhuanShengLevel >= keyValuePair.Value.MinUnionLevel)
					{
						if (keyValuePair.Key == 5)
						{
							ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(1007);
							if (activityTipItem != null && !activityTipItem.IsActive)
							{
								continue;
							}
						}
						List<ActivityTime> timeList = keyValuePair.Value.TimeList;
						if (timeList.Count >= 1)
						{
							int i = 0;
							while (i < timeList.Count)
							{
								DateTime dateTimeLeft = timeList[i].DateTimeLeft;
								DateTime dateTimeRight = timeList[i].DateTimeRight;
								DateTime correctDateTime = Global.GetCorrectDateTime();
								long ticks = correctDateTime.Ticks;
								if (dateTimeLeft.Year == correctDateTime.Year)
								{
									if (ticks >= dateTimeLeft.Ticks && ticks < dateTimeRight.Ticks)
									{
										if (keyValuePair.Key == 10 && !Global.HaveYanHui)
										{
											GameInstance.Game.HaveYanHuiNPC();
											break;
										}
										if (keyValuePair.Key == 11)
										{
											isKuafu = true;
										}
										else
										{
											num++;
											if (id < 0)
											{
												activityTime.DateTimeLeft = dateTimeLeft;
												activityTime.DateTimeRight = dateTimeRight;
												id = keyValuePair.Key;
												name = keyValuePair.Value.Name;
												break;
											}
										}
									}
									else if (keyValuePair.Key == 10)
									{
										Global.HaveYanHui = false;
									}
									IL_202:
									i++;
									continue;
									goto IL_202;
								}
								Global.DictAcitvityMinLevle.Clear();
								Global.DictAcitvityMinLevle = null;
								return 0;
							}
						}
					}
				}
			}
			return num;
		}

		public static DateTime GetBuchangTime()
		{
			if (string.IsNullOrEmpty(Global.Data.roleData.BuChangStartDay))
			{
				return Global._BuchangTime;
			}
			DateTime buchangTime = default(DateTime);
			string[] array = Global.Data.roleData.BuChangStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 01:01:01", ref buchangTime);
			Global._BuchangTime = buchangTime;
			return Global._BuchangTime;
		}

		public static string GetBuchangTimeStr(int day, int hour, int minu, int sec)
		{
			if (day == -1 || hour == -1 || minu == -1 || sec == -1)
			{
				return "-1";
			}
			DateTime buchangTime = Global.GetBuchangTime();
			DateTime dateTime;
			dateTime..ctor(buchangTime.Year, buchangTime.Month, buchangTime.Day + day, hour, minu, sec, 0);
			return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
		}

		private static bool CanShowIconByTime(int startDay, DateTime kaiFuDayTime, XElement item)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (startDay <= 0)
			{
				return false;
			}
			if (correctDateTime.DayOfYear - kaiFuDayTime.DayOfYear >= startDay)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(item, "ShowType");
				return xelementAttributeInt <= 0 || correctDateTime.DayOfYear - kaiFuDayTime.DayOfYear < startDay + xelementAttributeInt;
			}
			return false;
		}

		private static void LoadActivityTipCfg()
		{
			if (Global.DictAcitvityMinLevle != null)
			{
				return;
			}
			if (string.IsNullOrEmpty(Global.Data.roleData.KaiFuStartDay))
			{
				return;
			}
			DateTime dateTime = default(DateTime);
			string[] array = Global.Data.roleData.KaiFuStartDay.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 01:01:01", ref dateTime);
			DateTime serverMergeTime = Global.GetServerMergeTime();
			DateTime jieriTime = Global.GetJieriTime();
			Global.DictAcitvityMinLevle = new Dictionary<int, ActivityTipConfigItem>();
			XElement gameResXml = Global.GetGameResXml("Config/Activity/ActivityTip.Xml");
			if (gameResXml == null)
			{
				return;
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Tip");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (xelement != null)
				{
					ActivityTipConfigItem activityTipConfigItem = new ActivityTipConfigItem();
					activityTipConfigItem.ID = Global.GetXElementAttributeInt(xelement, "ID");
					activityTipConfigItem.Name = Global.GetXElementAttributeStr(xelement, "Name");
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					activityTipConfigItem.MinUnionLevel = UIHelper.GetUnionZhuanShengLevel(xelementAttributeInt, xelementAttributeInt2);
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShowTimes");
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "WeekDays");
					string[] array2 = xelementAttributeStr2.Split(new char[]
					{
						','
					});
					int dayOfWeek = correctDateTime.DayOfWeek;
					if (StringUtil.trim(xelementAttributeStr2).Length == 0 || Enumerable.ToList<string>(array2).IndexOf(StringUtil.substitute("{0}", new object[]
					{
						dayOfWeek
					})) >= 0)
					{
						int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "TimeType");
						DateTime kaiFuDayTime = dateTime;
						if (xelementAttributeInt3 == 2)
						{
							kaiFuDayTime = serverMergeTime;
						}
						else if (xelementAttributeInt3 == 3)
						{
							kaiFuDayTime = jieriTime;
						}
						string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "StartDay");
						if (!string.IsNullOrEmpty(xelementAttributeStr3))
						{
							bool flag = true;
							string[] array3 = xelementAttributeStr3.Split(new char[]
							{
								','
							});
							for (int j = 0; j < array3.Length; j++)
							{
								int startDay = Global.SafeConvertToInt32(array3[j]);
								if (Global.CanShowIconByTime(startDay, kaiFuDayTime, xelement))
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								goto IL_3C2;
							}
						}
						string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "OpenDay");
						if (!string.IsNullOrEmpty(xelementAttributeStr4))
						{
							DateTime correctDateTime2 = Global.GetCorrectDateTime();
							DateTime dateTime2 = default(DateTime);
							DateTime.TryParse(xelementAttributeStr4, ref dateTime2);
							if (correctDateTime2 < dateTime2)
							{
								goto IL_3C2;
							}
						}
						List<ActivityTime> list = new List<ActivityTime>();
						string[] array4 = xelementAttributeStr.Split(new char[]
						{
							'|'
						});
						for (int k = 0; k < array4.Length; k++)
						{
							string[] array5 = array4[k].Split(new char[]
							{
								'-'
							});
							if (array5.Length == 2)
							{
								if (StringUtil.trim(array5[1]) == "24:00")
								{
									array5[1] = "23:59";
								}
								string text = correctDateTime.ToString("yyyy-MM-dd ") + StringUtil.trim(array5[0]) + string.Empty;
								string text2 = correctDateTime.ToString("yyyy-MM-dd ") + StringUtil.trim(array5[1]) + string.Empty;
								DateTime dateTimeLeft = default(DateTime);
								if (DateTime.TryParse(text, ref dateTimeLeft))
								{
									DateTime dateTimeRight = default(DateTime);
									if (DateTime.TryParse(text2, ref dateTimeRight))
									{
										list.Add(new ActivityTime
										{
											DateTimeLeft = dateTimeLeft,
											DateTimeRight = dateTimeRight
										});
									}
								}
							}
						}
						activityTipConfigItem.TimeList = list;
						if (!Global.DictAcitvityMinLevle.ContainsKey(activityTipConfigItem.ID))
						{
							Global.DictAcitvityMinLevle.Add(activityTipConfigItem.ID, activityTipConfigItem);
						}
					}
				}
				IL_3C2:;
			}
		}

		public static int GetIconsMinLevel(int index)
		{
			if (Global.IconsMinLevel == null)
			{
				Global.IconsMinLevel = ConfigSystemParam.GetSystemParamIntArrayByName("IconMinLevel", ',');
			}
			return Global.IconsMinLevel[index];
		}

		public static bool IsShowEffect(int level)
		{
			if (Global.HuodongIconEffectNeedRoleLevel == null)
			{
				Global.HuodongIconEffectNeedRoleLevel = ConfigSystemParam.GetSystemParamIntArrayByName("HuodongIconEffectNeedRoleLevel", ',');
			}
			return Enumerable.ToList<int>(Global.HuodongIconEffectNeedRoleLevel).IndexOf(level) != -1;
		}

		public static long GetYangGongBKJiFenMaskValue(int awardNo)
		{
			XElement gameResXml = Global.GetGameResXml("Config/LuckyAward.Xml");
			if (gameResXml == null)
			{
				return 0L;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
			List<int> list = new List<int>();
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
				list.Add(xelementAttributeInt);
			}
			list.Sort((int x, int y) => x - y);
			int num = list.IndexOf(awardNo);
			long num2 = 1L;
			if (num > 0)
			{
				num2 <<= num;
			}
			return num2;
		}

		public static bool IsYangGongBKAwardHasBeenGot(int awardNo, YangGongBKDailyJiFenData dailyData)
		{
			if (dailyData == null)
			{
				return false;
			}
			long awardHistory = dailyData.AwardHistory;
			int dayID = dailyData.DayID;
			if (DateTime.Now.DayOfYear != dayID)
			{
				return false;
			}
			long yangGongBKJiFenMaskValue = Global.GetYangGongBKJiFenMaskValue(awardNo);
			return (awardHistory & yangGongBKJiFenMaskValue) != 0L;
		}

		public static bool IsYangGongBKAwardCanBeenGot(int awardNo, YangGongBKDailyJiFenData dailyData)
		{
			if (dailyData == null || dailyData.DayID != DateTime.Now.DayOfYear)
			{
				return false;
			}
			XElement gameResXml = Global.GetGameResXml("Config/LuckyAward.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
			int num = 99999999;
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (xelementAttributeInt == awardNo)
				{
					num = Global.GetXElementAttributeInt(xelement, "MinLucky");
					break;
				}
			}
			return dailyData.JiFen >= num;
		}

		public static double GetYangGongBKMaxLuckyValue()
		{
			XElement gameResXml = Global.GetGameResXml("Config/LuckyAward.Xml");
			if (gameResXml == null)
			{
				return 0.0;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
			double num = 0.0;
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "MinLucky");
				if (num < (double)xelementAttributeInt)
				{
					num = (double)xelementAttributeInt;
				}
			}
			return num;
		}

		public static GoodsData GetNPCSaleGoodsData(int goodsID)
		{
			int count = Global.Data.NpcSaleGoodsDataList.Count;
			GoodsData goodsData = new GoodsData();
			goodsData.Id = count;
			goodsData.GoodsID = goodsID;
			goodsData.Using = 0;
			goodsData.Forge_level = 0;
			goodsData.Starttime = "1900-01-01 12:00:00";
			goodsData.Endtime = "1900-01-01 12:00:00";
			goodsData.Site = 0;
			goodsData.Quality = 0;
			goodsData.Props = string.Empty;
			goodsData.GCount = 1;
			goodsData.Binding = 0;
			goodsData.Jewellist = string.Empty;
			goodsData.BagIndex = 0;
			goodsData.SaleMoney1 = 0;
			goodsData.SaleYuanBao = 0;
			goodsData.SaleYinPiao = 0;
			goodsData.AddPropIndex = 0;
			goodsData.BornIndex = 0;
			goodsData.Lucky = 0;
			goodsData.Strong = 0;
			Global.Data.NpcSaleGoodsDataList.Add(goodsData);
			return goodsData;
		}

		public static int GetYunChengGeMapCode()
		{
			if (Global.YunChengGeMapCode == -1)
			{
				Global.YunChengGeMapCode = ConfigSystemParam.GetSystemParamIntArrayByName("ShengXiaoGuessParams", ',')[0];
			}
			return Global.YunChengGeMapCode;
		}

		public static int GetYunChengGuessNeedGoodsID()
		{
			return ConfigSystemParam.GetSystemParamIntArrayByName("ShengXiaoGuessParams", ',')[2];
		}

		public static string GetShengXiaoNameByCode(int type)
		{
			switch (type)
			{
			case 1:
				return Global.GetLang("鼠");
			case 2:
				return Global.GetLang("牛");
			default:
				if (type == 16)
				{
					return Global.GetLang("龙");
				}
				if (type == 32)
				{
					return Global.GetLang("蛇");
				}
				if (type == 64)
				{
					return Global.GetLang("马");
				}
				if (type == 128)
				{
					return Global.GetLang("羊");
				}
				if (type == 256)
				{
					return Global.GetLang("猴");
				}
				if (type == 512)
				{
					return Global.GetLang("鸡");
				}
				if (type == 1024)
				{
					return Global.GetLang("狗");
				}
				if (type != 2048)
				{
					return string.Empty;
				}
				return Global.GetLang("猪");
			case 4:
				return Global.GetLang("虎");
			case 8:
				return Global.GetLang("兔");
			}
		}

		public static void GotoShengXiaoGuessNPC()
		{
			int mapCode = 2;
			int targetNpcID = 241;
			if (Global.Data.roleData.MapCode == Global.GetYunChengGeMapCode())
			{
				mapCode = 5000;
				targetNpcID = 242;
			}
			Global.Data.TargetNpcID = targetNpcID;
			Point npcpointByID = Global.GetNPCPointByID(mapCode, Global.Data.TargetNpcID);
		}

		public static string TransPathToString(List<ANode> path)
		{
			string text = string.Empty;
			if (path != null)
			{
				for (int i = 0; i < path.Count; i++)
				{
					ANode anode = path[i];
					if (text.Length > 0)
					{
						text += "|";
					}
					text += StringUtil.substitute("{0}_{1}", new object[]
					{
						anode.x,
						anode.y
					});
				}
			}
			return text;
		}

		public static List<ANode> TransStringToPathArr(string pathStr)
		{
			List<ANode> list = new List<ANode>();
			if (pathStr != null && pathStr.Length > 0)
			{
				foreach (string text in pathStr.Split(new char[]
				{
					'|'
				}))
				{
					string[] array2 = text.Split(new char[]
					{
						'_'
					});
					if (array2.Length == 2)
					{
						list.Add(new ANode(0, 0)
						{
							x = Global.SafeConvertToInt32(array2[0]),
							y = Global.SafeConvertToInt32(array2[1])
						});
					}
				}
			}
			return list;
		}

		public static List<ANode> FindSubPathFromExistPath(string existPath, int currentX, int currentY, int toX, int toY)
		{
			return Global.TransStringToPathArr(existPath);
		}

		public static List<ANode> FindSubPathFromExistPathEx(List<ANode> existPath, int currentX, int currentY, int toX, int toY)
		{
			return existPath;
		}

		public static bool IsBPointOnLineAToC(ANode a, ANode b, ANode c)
		{
			int num = a.x - b.x;
			int num2 = b.x - c.x;
			int num3 = a.y - b.y;
			int num4 = b.y - c.y;
			return (num >= 0 && num2 >= 0 && num3 >= 0 && num4 >= 0) || (num <= 0 && num2 <= 0 && num3 <= 0 && num4 <= 0);
		}

		public static int GetChengJiuLevel(int chengJiuPoints = 0)
		{
			return Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ChengJiuLevel);
		}

		public static string GetChengJiuName(int level)
		{
			int bufferGoodsID = Global.GetBufferGoodsID(31, level - 1);
			if (bufferGoodsID > 0)
			{
				return Global.GetGoodsNameByID(bufferGoodsID, false);
			}
			return Global.GetLang("无");
		}

		public static string GetChengJiuFetchAwardOkMsgByChengJiuID(int chengJiuID)
		{
			ChengJiuVO chengJiuVOByChengJiuID = ConfigChengJiu.GetChengJiuVOByChengJiuID(chengJiuID);
			if (chengJiuVOByChengJiuID != null)
			{
				string text = StringUtil.substitute(Global.GetLang("恭喜您达成了成就[{0}],领取了"), new object[]
				{
					chengJiuVOByChengJiuID.Name
				});
				bool flag = false;
				int num = chengJiuVOByChengJiuID.ChengJiu;
				if (num > 0)
				{
					text += StringUtil.substitute(Global.GetLang("成就点{0}点"), new object[]
					{
						num
					});
					flag = true;
				}
				num = chengJiuVOByChengJiuID.BindZuanShi;
				if (num > 0)
				{
					if (flag)
					{
						text += ",";
					}
					text += StringUtil.substitute(Global.GetLang("绑定钻石{0}"), new object[]
					{
						num
					});
					flag = true;
				}
				num = chengJiuVOByChengJiuID.BindMoney;
				if (num > 0)
				{
					if (flag)
					{
						text += ",";
					}
					text += StringUtil.substitute(Global.GetLang("绑定金币{0}"), new object[]
					{
						num
					});
				}
				return text;
			}
			return string.Empty;
		}

		public static int GetCompletedChengJiuWithoutGetingAwardNum()
		{
			if (Global.Data.ChengJiuData == null || Global.Data.ChengJiuData.ChengJiuFlags == null)
			{
				return 0;
			}
			List<ushort> chengJiuFlags = Global.Data.ChengJiuData.ChengJiuFlags;
			if (chengJiuFlags == null)
			{
				return 0;
			}
			uint num = 1U;
			int num2 = 2;
			int num3 = 0;
			for (int i = 0; i < chengJiuFlags.Count; i++)
			{
				uint num4 = (uint)chengJiuFlags[i];
				if ((num4 & num) != 0U)
				{
					if (((ulong)num4 & (ulong)((long)num2)) == 0UL)
					{
						num3++;
					}
				}
			}
			return num3;
		}

		public static void TryShowMendPrice(int goodsDbId, bool show = true)
		{
		}

		public static int GetGoodsMendPrice(int goodsDbId)
		{
			int result = 0;
			GoodsData goodsDataByDbID = Global.GetGoodsDataByDbID(goodsDbId, null);
			if (goodsDataByDbID == null)
			{
				return 0;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataByDbID.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy >= 0 && categoriy <= 25)
				{
					int priceTwo = goodsXmlNodeByID.PriceTwo;
					double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsDataByDbID.GoodsID);
					if (goodsEquipPropsDoubleList.Length >= 2)
					{
						long num = (long)goodsEquipPropsDoubleList[1];
						if (num != 0L)
						{
							result = (int)((double)priceTwo / 3.0 * (double)goodsDataByDbID.Strong / (double)num);
						}
					}
				}
			}
			return result;
		}

		public static void TryShowSalePriceToNpc(int goodsDbId, bool show = true)
		{
		}

		public static int GetGoodsSaleToNpcPrice(GoodsData goodsData)
		{
			int num = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				int num2 = goodsXmlNodeByID.PriceTwo;
				num2 = Math.Max(0, num2);
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy < 0 || categoriy > 25)
				{
					if (goodsXmlNodeByID.UsingNum <= 1)
					{
						return (int)((double)num2 / 5.0) * goodsData.GCount;
					}
					return (int)((double)num2 / 5.0);
				}
				else
				{
					double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
					if (goodsEquipPropsDoubleList.Length >= 2)
					{
						long num3 = (long)goodsEquipPropsDoubleList[0];
						num3 = Math.Max(num3, 1L);
						if (num3 <= 1L)
						{
							return 0;
						}
						num = (int)((double)num2 / 5.0 * (double)Math.Max(0L, num3 - (long)goodsData.Strong) / (double)num3);
						if (goodsXmlNodeByID.UsingNum <= 1)
						{
							num = goodsData.GCount * num;
						}
					}
				}
			}
			return num;
		}

		public static int GetGoodsSaleToNpJiFen(GoodsData goodsData)
		{
			int num = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuoYueHuiShouXiShu", ',');
				num = goodsXmlNodeByID.ChangeJinYuan;
				if (num != 1 && goodsXmlNodeByID.Categoriy != 340)
				{
					int num2 = Global.GetZhuoyueAttributeCount(goodsData);
					if (num2 > 0)
					{
						num2 = Math.Min(num2, systemParamIntArrayByName.Length);
						num *= systemParamIntArrayByName[num2 - 1];
					}
				}
				num = Math.Max(0, num);
			}
			return num;
		}

		public static int GetZaiZaoDian(GoodsData goodsData)
		{
			int result = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				int num = Global.GetZhuoyueAttributeCount(goodsData);
				if (0 < goodsXmlNodeByID.ChangeZaiZao)
				{
					if (num == 0)
					{
						num = 1;
					}
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuoYueHuiShouZaiZaoXiShu", ',');
					if (systemParamIntArrayByName.Length == 6)
					{
						result = goodsXmlNodeByID.ChangeZaiZao * systemParamIntArrayByName[(num <= 6) ? (num - 1) : 5];
					}
					else
					{
						result = goodsXmlNodeByID.ChangeZaiZao * num;
					}
				}
				else
				{
					result = goodsXmlNodeByID.ChangeZaiZao * num;
				}
			}
			return result;
		}

		public static int GetGoodsSaleToNpcZaizao(GoodsData goodsData)
		{
			int num = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				double[] systemParamDoubleArrayByName = ConfigSystemParam.GetSystemParamDoubleArrayByName("ZhuoYueHuiShouZaiZaoXiShu");
				if (systemParamDoubleArrayByName == null)
				{
					return num;
				}
				num = goodsXmlNodeByID.ChangeZaiZao;
				int num2 = Global.GetZhuoyueAttributeCount(goodsData);
				if (num2 > 0)
				{
					num2 = Math.Min(num2, systemParamDoubleArrayByName.Length);
					num = (int)((double)num * systemParamDoubleArrayByName[num2 - 1]);
				}
				num = Math.Max(0, num);
			}
			return num;
		}

		public static int GetGoodsSaleToNpcChongShengExp(GoodsData goodsData)
		{
			int result = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				result = goodsXmlNodeByID.ChangeRebornExp;
			}
			return result;
		}

		public static int GetPetPrice(GoodsData goodsData)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int num = goodsXmlNodeByID.ZhanHunPrice;
			if (Global.PetExp == null)
			{
				XElement gameResXml = Global.GetGameResXml("Config/PetLevelUp.Xml");
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "PetLevelUp");
				Global.PetExp = new int[xelementList.Count + 1];
				for (int i = 0; i < xelementList.Count; i++)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "NeedEXP");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "Level");
					Global.PetExp[xelementAttributeInt2] = xelementAttributeInt;
				}
			}
			for (int j = 1; j < goodsData.Forge_level + 2; j++)
			{
				num += Global.PetExp[j];
			}
			if (goodsData.ElementhrtsProps != null)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				XElement gameResXml2 = Global.GetGameResXml("Config/PetSkillLevelup.xml");
				if (gameResXml2 != null)
				{
					List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "Levelup");
					for (int k = 0; k < xelementList2.Count; k++)
					{
						if (xelementList2 != null)
						{
							int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList2[k], "Level");
							int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelementList2[k], "Cost");
							dictionary.Add(xelementAttributeInt3, xelementAttributeInt4);
						}
					}
				}
				for (int l = 0; l < goodsData.ElementhrtsProps.Count; l++)
				{
					if (l % 3 == 1)
					{
						int num2 = goodsData.ElementhrtsProps[l];
						for (int m = 1; m <= num2; m++)
						{
							if (dictionary.ContainsKey(m))
							{
								num += dictionary[m];
							}
						}
					}
				}
			}
			return num;
		}

		public static string GetColorStringForHtmlText(params object[] _args)
		{
			string text = string.Empty;
			int num = _args.Length;
			object[] array;
			if (num == 1 && _args[0] is string[])
			{
				array = (_args[0] as string[]);
				num = array.Length;
			}
			else
			{
				array = _args;
			}
			int num2 = 0;
			while (num2 + 1 < num)
			{
				text += StringUtil.substitute("<font color=\"{0}\">{1}", new object[]
				{
					array[num2],
					array[num2 + 1]
				});
				num2 += 2;
			}
			return text;
		}

		public static string GetColorStringForNGUIText(params object[] _args)
		{
			string text = string.Empty;
			int num = _args.Length;
			object[] array;
			if (num == 1 && _args[0] is string[])
			{
				array = (_args[0] as string[]);
				num = array.Length;
			}
			else
			{
				array = _args;
			}
			int num2 = 0;
			while (num2 + 1 < num)
			{
				text += StringUtil.substitute("{0}{1}{2}", new object[]
				{
					"{" + array[num2] + "}",
					array[num2 + 1],
					"{-}"
				});
				num2 += 2;
			}
			return text;
		}

		public static void ParseXmlAttributeValueExItemDict(Dictionary<int, AttributeValueEx> dict, string attributeStr, char spliteChar = '|')
		{
			if (!string.IsNullOrEmpty(attributeStr))
			{
				foreach (string attributeStr2 in attributeStr.Split(new char[]
				{
					spliteChar
				}))
				{
					AttributeValueEx attributeValueEx = Global.ParseXmlAttributeValueExItem(attributeStr2, ',', '-');
					if (attributeValueEx != null)
					{
						AttributeValueEx attributeValueEx2;
						if (dict.TryGetValue(attributeValueEx.AttributeIndex, ref attributeValueEx2))
						{
							attributeValueEx2.AttributeValue0 += attributeValueEx.AttributeValue0;
							attributeValueEx2.AttributeValue1 += attributeValueEx.AttributeValue1;
						}
						else
						{
							dict[attributeValueEx.AttributeIndex] = attributeValueEx;
						}
					}
				}
			}
		}

		public static AttributeValueEx ParseXmlAttributeValueExItem(string attributeStr, char spliteChar1 = ',', char spliteChar2 = '-')
		{
			if (string.IsNullOrEmpty(attributeStr))
			{
				return null;
			}
			AttributeValueEx attributeValueEx = new AttributeValueEx();
			string[] array = attributeStr.Split(new char[]
			{
				spliteChar1
			});
			if (array.Length == 2)
			{
				double[] array2 = ConvertExt.String2DoubleArray(array[1], spliteChar2);
				if (array2 != null)
				{
					attributeValueEx.AttributeIndex = ExtPropIndexes.GetPropIndex(array[0]);
					if (attributeValueEx.AttributeIndex >= 0)
					{
						attributeValueEx.AttributeValue0 = array2[0];
						if (array2.Length >= 2)
						{
							attributeValueEx.AttributeValue1 = array2[1];
						}
						return attributeValueEx;
					}
				}
			}
			return null;
		}

		public static int GetTianShengParamsIndex(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return -1;
			}
			string execMagic = goodsXmlNodeByID.ExecMagic;
			if (execMagic.ToUpper().IndexOf("DB_ADD_YINYONG(") < 0)
			{
				return -1;
			}
			int num = execMagic.IndexOf(",");
			int num2 = execMagic.IndexOf(")");
			if (num2 <= num + 1)
			{
				return -1;
			}
			return Global.SafeConvertToInt32(execMagic.Substring(num + 1, num2 - num - 1));
		}

		public static int GetRoleCommonUseParamsValue(int index)
		{
			if (index >= 0 && index < Global.Data.roleData.RoleCommonUseIntPamams.Count)
			{
				return Global.Data.roleData.RoleCommonUseIntPamams[index];
			}
			return 0;
		}

		public static int GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs index)
		{
			return Global.GetRoleCommonUseParamsValue((int)index);
		}

		public static int GetRoleCommonUseParamsValueForOtherRole(string roleName, int index)
		{
			RoleData roleData = Global.FindRoleDataByName(roleName);
			return Global.GetRoleCommonUseParamsValueForOtherRole2(roleData, index);
		}

		public static int GetRoleCommonUseParamsValueForOtherRole(string roleName, RoleCommonUseIntParamsIndexs index)
		{
			return Global.GetRoleCommonUseParamsValueForOtherRole(roleName, (int)index);
		}

		public static int GetRoleCommonUseParamsValueForOtherRole(int roleID, RoleCommonUseIntParamsIndexs index)
		{
			RoleData roleData = Global.FindRoleDataByID(roleID);
			return Global.GetRoleCommonUseParamsValueForOtherRole2(roleData, index);
		}

		public static int GetRoleCommonUseParamsValueForOtherRole2(RoleData roleData, int index)
		{
			if (roleData == null || roleData.RoleCommonUseIntPamams == null)
			{
				return 0;
			}
			if (index >= 0 && index < roleData.RoleCommonUseIntPamams.Count)
			{
				return roleData.RoleCommonUseIntPamams[index];
			}
			return 0;
		}

		public static int GetRoleCommonUseParamsValueForOtherRole2(RoleData roleData, RoleCommonUseIntParamsIndexs index)
		{
			return Global.GetRoleCommonUseParamsValueForOtherRole2(roleData, (int)index);
		}

		public static int GetOtherRoleVIPLeve(RoleData roleData)
		{
			if (roleData == null || roleData.RoleCommonUseIntPamams == null)
			{
				return 0;
			}
			return roleData.VIPLevel;
		}

		public static int GetOtherRoleVIPLeve2(string roleName)
		{
			RoleData roleData = Global.FindRoleDataByName(roleName);
			return Global.GetOtherRoleVIPLeve(roleData);
		}

		public static int GetVIPLeve()
		{
			return Global.Data.roleData.VIPLevel;
		}

		public static bool IsSystemOpen(int sytemIndex)
		{
			int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(10);
			return (roleCommonUseParamsValue & 1 << sytemIndex) > 0;
		}

		public static int GetMaxActiveSystemIndex()
		{
			int num = 0;
			for (int i = 1; i <= 31; i++)
			{
				if (Global.IsSystemOpen(i))
				{
					num++;
				}
			}
			return num;
		}

		public static string GetVipPriorityString(int vipType)
		{
			string text = string.Empty;
			XElement gameResXml = Global.GetGameResXml("Config/VIP.Xml");
			if (gameResXml == null)
			{
				return text;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
			for (int i = 0; i < Enumerable.Count<XElement>(xelementList); i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "GetID");
				if (vipType == Global.GetXElementAttributeInt(xelement, "Type") && xelementAttributeInt != 100 && xelementAttributeInt != 101 && xelementAttributeInt != 102)
				{
					text += StringUtil.substitute("{0}\n", new object[]
					{
						Global.GetXElementAttributeStr(xelement, "Description")
					});
				}
			}
			return text;
		}

		public static string GetUnitPropIndexeName(int index)
		{
			if (index < 4 && index >= 0)
			{
				return UnitPropIndexes.UnitPropIndexeNames[index];
			}
			return string.Empty;
		}

		public static string GetExtPropIndexeName(int index)
		{
			if (index < 177 && index >= 0)
			{
				return ExtPropIndexes.ExtPropIndexChineseNames[index];
			}
			return string.Empty;
		}

		private static List<XElement> LoadCachingTaskPlotItems()
		{
			if (Global._CachingTaskPlotItems != null)
			{
				return Global._CachingTaskPlotItems;
			}
			Global._CachingTaskPlotItems = new List<XElement>();
			XElement isolateResXml = Global.GetIsolateResXml("Config/TaskPlot.Xml");
			if (isolateResXml == null)
			{
				return Global._CachingTaskPlotItems;
			}
			List<XElement> xelementList = Global.GetXElementList(isolateResXml, "*");
			for (int i = 0; i < xelementList.Count; i++)
			{
				Global._CachingTaskPlotItems.Add(xelementList[i]);
			}
			return Global._CachingTaskPlotItems;
		}

		public static int GetTaskPlotItemByMode(int mode, int timeParameters)
		{
			List<XElement> list = Global.LoadCachingTaskPlotItems();
			if (list == null)
			{
				return -1;
			}
			int result = -1;
			for (int i = 0; i < list.Count; i++)
			{
				XElement xelement = list[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "OccupCondition");
				if (xelementAttributeInt == -1 || xelementAttributeInt == Global.Data.roleData.Occupation)
				{
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "TriggerCondition");
					if (mode == xelementAttributeInt2)
					{
						int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "TimeParameters");
						if (mode == 1)
						{
							if (xelementAttributeInt3 == Global.Data.roleData.Level)
							{
								result = Global.GetXElementAttributeInt(xelement, "ID");
								break;
							}
						}
						else if (mode == 2)
						{
							if (xelementAttributeInt3 == Global.Data.roleData.MapCode)
							{
								result = Global.GetXElementAttributeInt(xelement, "ID");
								break;
							}
						}
						else if (mode == 3)
						{
							if (Global.Data.roleData.LoginNum <= 0)
							{
								result = Global.GetXElementAttributeInt(xelement, "ID");
								break;
							}
						}
						else
						{
							if (mode == 4)
							{
								break;
							}
							if (mode == 5)
							{
								break;
							}
							if ((mode == 6 || mode == 7 || mode == 8 || mode == 9) && xelementAttributeInt3 == timeParameters)
							{
								result = Global.GetXElementAttributeInt(xelement, "ID");
								break;
							}
						}
					}
				}
			}
			return result;
		}

		public static XElement GetTaskPlotXmlItemByID(int id)
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/TaskPlot.Xml");
			if (isolateResXml == null)
			{
				return null;
			}
			return Global.GetXElement(isolateResXml, "help", "ID", id.ToString());
		}

		public static void UpdataMallData(MallSaleData mallSaleData)
		{
			if (Global.Data.MallData == null)
			{
				Global.Data.MallData = mallSaleData;
			}
			else
			{
				if (mallSaleData.MallXmlString != string.Empty)
				{
					Global.Data.MallData.MallXmlString = mallSaleData.MallXmlString;
				}
				if (mallSaleData.MallTabXmlString != string.Empty)
				{
					Global.Data.MallData.MallTabXmlString = mallSaleData.MallTabXmlString;
				}
				if (mallSaleData.QiangGouXmlString != string.Empty)
				{
					Global.Data.MallData.QiangGouXmlString = mallSaleData.QiangGouXmlString;
				}
			}
		}

		public static void UpdataActivitData(ActivitiesData activitData)
		{
			Global.Data.ActivitData = activitData;
		}

		public static string GetSexString(int sex)
		{
			if (sex == 0)
			{
				return Global.GetLang("男性");
			}
			if (sex == 1)
			{
				return Global.GetLang("女性");
			}
			return Global.GetLang("所有");
		}

		public static string GetMoneyNameByType(int moneyType)
		{
			string result = string.Empty;
			if (moneyType != 13)
			{
				if (moneyType != 14)
				{
					if (moneyType != 1)
					{
						if (moneyType != 8)
						{
							if (moneyType != 20)
							{
								if (moneyType != 30)
								{
									if (moneyType == 90)
									{
										result = Global.GetLang("战魂值");
									}
								}
								else
								{
									result = Global.GetLang("神器之魂");
								}
							}
							else
							{
								result = Global.GetLang("猎杀值");
							}
						}
						else
						{
							result = Global.GetLang("金币");
						}
					}
					else
					{
						result = Global.GetLang("绑定金币");
					}
				}
				else
				{
					result = Global.GetLang("军功值");
				}
			}
			else
			{
				result = Global.GetLang("积分");
			}
			return result;
		}

		public static int GetGoodsPriceByMoneyType(int goodsID, int moneyType)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return 0;
			}
			int num = -1;
			if (moneyType != 13)
			{
				if (moneyType != 14)
				{
					if (moneyType != 1)
					{
						if (moneyType != 8)
						{
							if (moneyType != 20)
							{
								if (moneyType != 30)
								{
									if (moneyType == 90)
									{
										num = goodsXmlNodeByID.ZhanHunPrice;
									}
								}
								else
								{
									num = goodsXmlNodeByID.JiFenPrice;
								}
							}
							else
							{
								num = goodsXmlNodeByID.LieShaPrice;
							}
						}
						else
						{
							num = goodsXmlNodeByID.PriceTwo;
						}
					}
					else
					{
						num = goodsXmlNodeByID.PriceOne;
					}
				}
				else
				{
					num = goodsXmlNodeByID.JunGongPrice;
				}
			}
			else
			{
				num = goodsXmlNodeByID.JinYuanPrice;
			}
			return Math.Max(0, num);
		}

		public static int GetRoleOwnNumByMoneyType(int moneyType)
		{
			int result = 0;
			switch (moneyType)
			{
			case 138:
				result = (int)Global.Data.roleData.MoneyData[138];
				break;
			case 139:
				result = (int)Global.Data.roleData.MoneyData[139];
				break;
			case 140:
				result = (int)Global.Data.roleData.MoneyData[140];
				break;
			default:
				switch (moneyType)
				{
				case 155:
					result = (int)Global.Data.roleData.MoneyData[155];
					break;
				case 156:
					result = (int)Global.Data.roleData.MoneyData[156];
					break;
				case 157:
					result = (int)Global.Data.roleData.MoneyData[157];
					break;
				default:
					if (moneyType != 13)
					{
						if (moneyType != 14)
						{
							if (moneyType != 1)
							{
								if (moneyType != 8)
								{
									if (moneyType != 20)
									{
										if (moneyType != 30)
										{
											if (moneyType != 40)
											{
												if (moneyType != 50)
												{
													if (moneyType != 90)
													{
														if (moneyType != 112)
														{
															if (moneyType == 132)
															{
																result = (int)Global.Data.roleData.MoneyData[132];
															}
														}
														else
														{
															result = Global.GetRoleCommonUseParamsValue(37);
														}
													}
													else
													{
														result = Global.GetRoleCommonUseParamsValue(14);
													}
												}
												else
												{
													result = Global.Data.roleData.Gold;
												}
											}
											else
											{
												result = Global.Data.roleData.UserMoney;
											}
										}
										else
										{
											result = Global.GetRoleCommonUseParamsValue(1);
										}
									}
									else
									{
										result = Global.GetRoleCommonUseParamsValue(2);
									}
								}
								else
								{
									result = Global.Data.roleData.YinLiang;
								}
							}
							else
							{
								result = Global.Data.roleData.Money1;
							}
						}
						else
						{
							result = Global.GetRoleCommonUseParamsValue(11);
						}
					}
					else
					{
						result = Global.GetRoleCommonUseParamsValue(5);
					}
					break;
				case 161:
					result = (int)Global.Data.roleData.MoneyData[161];
					break;
				case 163:
					result = (int)Global.Data.roleData.MoneyData[163];
					break;
				}
				break;
			case 144:
				result = (int)Global.Data.roleData.MoneyData[144];
				break;
			case 146:
				result = (int)Global.Data.roleData.MoneyData[146];
				break;
			}
			return result;
		}

		public static bool IsAnyEquipNeedMend()
		{
			if (Global.Data.roleData.GoodsDataList != null)
			{
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.Using > 0)
					{
						if (goodsData.Strong > 0)
						{
							return true;
						}
					}
				}
			}
			if (Global.Data.roleData.RebornGoodsDataList != null)
			{
				for (int j = 0; j < Global.Data.roleData.RebornGoodsDataList.Count; j++)
				{
					GoodsData goodsData2 = Global.Data.roleData.RebornGoodsDataList[j];
					if (goodsData2.Using > 0)
					{
						if (goodsData2.Strong > 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public static GoodsData GetAnyBrokenEquip()
		{
			if (Global.Data.roleData.GoodsDataList != null)
			{
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.Using > 0)
					{
						int num;
						if (!Global.GoodsStrongDict.TryGetValue(goodsData.GoodsID, ref num))
						{
							double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsData.GoodsID);
							num = (int)goodsEquipPropsDoubleList[0];
							Global.GoodsStrongDict.Add(goodsData.GoodsID, num);
						}
						if (goodsData.Strong >= num)
						{
							return goodsData;
						}
					}
				}
			}
			if (Global.Data.roleData.RebornGoodsDataList != null)
			{
				for (int j = 0; j < Global.Data.roleData.RebornGoodsDataList.Count; j++)
				{
					GoodsData goodsData2 = Global.Data.roleData.RebornGoodsDataList[j];
					if (goodsData2.Using > 0)
					{
						int num2;
						if (!Global.GoodsStrongDict.TryGetValue(goodsData2.GoodsID, ref num2))
						{
							double[] goodsEquipPropsDoubleList2 = Global.GetGoodsEquipPropsDoubleList(goodsData2.GoodsID);
							num2 = (int)goodsEquipPropsDoubleList2[0];
							Global.GoodsStrongDict.Add(goodsData2.GoodsID, num2);
						}
						if (goodsData2.Strong >= num2)
						{
							return goodsData2;
						}
					}
				}
			}
			return null;
		}

		public static bool IsAnyEquipInvalid()
		{
			if (Global.Data.roleData.GoodsDataList == null)
			{
				return false;
			}
			for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
				if (goodsData.Using > 0)
				{
					if (Global.IsOneOfEquipInvalid(goodsData.GoodsID))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsOneOfEquipInvalid(int goodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			int num = (int)Global.GetCurrentRoleProp(1, 0);
			if (num >= 0 && num < goodsXmlNodeByID.Strength)
			{
				return true;
			}
			int num2 = (int)Global.GetCurrentRoleProp(1, 2);
			if (num2 >= 0 && num2 < goodsXmlNodeByID.Dexterity)
			{
				return true;
			}
			int num3 = (int)Global.GetCurrentRoleProp(1, 1);
			if (num3 >= 0 && num3 < goodsXmlNodeByID.Intelligence)
			{
				return true;
			}
			int num4 = (int)Global.GetCurrentRoleProp(1, 3);
			return num4 >= 0 && num4 < goodsXmlNodeByID.Constitution;
		}

		public static string GetExitGameInfoStr()
		{
			string text = Global.GetLang("《怒斩》\r\n");
			text += Global.GetLang("★★打玩家得装备，热血PK游戏★★\r\n");
			text += Global.GetDailyTasksInfoStr();
			text += Global.GetFuBenNumInfoStr();
			return text + Global.GetLang("您确认要退出游戏吗？");
		}

		public static int GetCookieGoodsUsedNum(int goodsID)
		{
			string key = StringUtil.substitute("Mst_GoodsID_{0}_{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				goodsID
			});
			string @string = LocalStorage.GetString(key);
			return Global.SafeConvertToInt32(@string);
		}

		public static void AddCookieGoodsUsedNum(int goodsID)
		{
			int num = Global.GetCookieGoodsUsedNum(goodsID);
			num = Math.Max(0, num);
			num++;
			string key = StringUtil.substitute("Mst_GoodsID_{0}_{1}", new object[]
			{
				Global.Data.roleData.RoleID,
				goodsID
			});
			LocalStorage.SetString(key, num.ToString());
		}

		public static int GetMaxDaTianShiLevel()
		{
			int num = 0;
			if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
			{
				Global.GetUsingGoodsDataList();
			}
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("DaTianShi", ',');
			foreach (KeyValuePair<int, GoodsData> keyValuePair in Super.GData.RoleUsingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null && Super.CheckArrayHaveValue<string>(systemParamStringArrayByName, goodsXmlNodeByID.ID.ToString()))
				{
					int suitID = goodsXmlNodeByID.SuitID;
					if (suitID > num)
					{
						num = suitID;
					}
				}
			}
			return num;
		}

		public static int GetHuShengFuLevel()
		{
			int result = 0;
			if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
			{
				Global.GetUsingGoodsDataList();
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in Super.GData.RoleUsingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null && goodsXmlNodeByID.Categoriy == 22)
				{
					result = goodsXmlNodeByID.SuitID;
					break;
				}
			}
			return result;
		}

		public static int GetMaxAttackV()
		{
			if (Global.Data.CurrentRolePropFields == null || Global.Data.CurrentRolePropFields.Length <= 2)
			{
				return -1;
			}
			return (int)Global.Data.CurrentRolePropFields[2];
		}

		public static int GetMaxMAttackV()
		{
			if (Global.Data.CurrentRolePropFields == null || Global.Data.CurrentRolePropFields.Length <= 4)
			{
				return -1;
			}
			return (int)Global.Data.CurrentRolePropFields[4];
		}

		public static int GetMaxDSAttackV()
		{
			if (Global.Data.CurrentRolePropFields == null || Global.Data.CurrentRolePropFields.Length <= 10)
			{
				return -1;
			}
			return (int)Global.Data.CurrentRolePropFields[10];
		}

		public static double GetCurrentRoleProp(int level, int index)
		{
			double num = 0.0;
			if (Global.Data.CurrentRolePropFields == null || Global.Data.CurrentRolePropFields.Length < 20)
			{
				return -1.0;
			}
			if (level == 0)
			{
				if (index == 0)
				{
					num = Global.Data.CurrentRolePropFields[20];
				}
				else if (index == 1)
				{
					num = Global.Data.CurrentRolePropFields[20];
					for (int i = 1; i < 5; i++)
					{
						num -= Global.Data.CurrentRolePropFields[i];
					}
				}
			}
			else if (level == 1)
			{
				if (index < 4)
				{
					num = Global.Data.CurrentRolePropFields[index + 1];
				}
			}
			else if (level == 2)
			{
				if (index < 177)
				{
					switch (index)
					{
					case 1:
						num = Global.Data.CurrentRolePropFields[17];
						break;
					default:
						switch (index)
						{
						case 31:
							num = Global.Data.CurrentRolePropFields[14];
							break;
						default:
							if (index != 101)
							{
								if (index == 119)
								{
									num = (double)Global.Data.roleData.MaxArmorV;
								}
							}
							else
							{
								num = Global.Data.CurrentRolePropFields[27];
							}
							break;
						case 33:
							num = Global.Data.CurrentRolePropFields[9];
							break;
						}
						break;
					case 3:
						num = Global.Data.CurrentRolePropFields[7];
						break;
					case 4:
						num = Global.Data.CurrentRolePropFields[8];
						break;
					case 5:
						num = Global.Data.CurrentRolePropFields[12];
						break;
					case 6:
						num = Global.Data.CurrentRolePropFields[13];
						break;
					case 7:
						num = Global.Data.CurrentRolePropFields[5];
						break;
					case 8:
						num = Global.Data.CurrentRolePropFields[6];
						break;
					case 9:
						num = Global.Data.CurrentRolePropFields[10];
						break;
					case 10:
						num = Global.Data.CurrentRolePropFields[11];
						break;
					case 13:
						num = Global.Data.CurrentRolePropFields[15];
						break;
					case 15:
						num = Global.Data.CurrentRolePropFields[16];
						break;
					case 18:
						num = Global.Data.CurrentRolePropFields[18];
						break;
					case 19:
						num = Global.Data.CurrentRolePropFields[19];
						break;
					}
				}
			}
			else if (level == 3)
			{
				num = Global.Data.CurrentRolePropFields[index];
			}
			return num;
		}

		public static double SetCurrentRoleProp(int level, int index, double value)
		{
			double num = 0.0;
			if (Global.Data.CurrentRolePropFields == null || Global.Data.CurrentRolePropFields.Length < 20)
			{
				return -1.0;
			}
			if (level == 0)
			{
				if (index == 0)
				{
					num = Global.Data.CurrentRolePropFields[20];
				}
				else if (index == 1)
				{
					num = Global.Data.CurrentRolePropFields[20];
					for (int i = 1; i < 5; i++)
					{
						num -= Global.Data.CurrentRolePropFields[i];
					}
				}
			}
			else if (level == 1)
			{
				if (index < 4)
				{
					Global.Data.CurrentRolePropFields[index + 1] = value;
					num = value;
				}
			}
			else if (level == 2 && index < 177)
			{
				switch (index)
				{
				case 1:
					Global.Data.CurrentRolePropFields[17] = value;
					num = value;
					break;
				default:
					switch (index)
					{
					case 31:
						Global.Data.CurrentRolePropFields[14] = value;
						num = value;
						break;
					case 33:
						Global.Data.CurrentRolePropFields[9] = value;
						num = value;
						break;
					}
					break;
				case 3:
					Global.Data.CurrentRolePropFields[7] = value;
					num = value;
					break;
				case 4:
					Global.Data.CurrentRolePropFields[8] = value;
					num = value;
					break;
				case 5:
					Global.Data.CurrentRolePropFields[12] = value;
					num = value;
					break;
				case 6:
					Global.Data.CurrentRolePropFields[13] = value;
					num = value;
					break;
				case 7:
					Global.Data.CurrentRolePropFields[5] = value;
					num = value;
					break;
				case 8:
					Global.Data.CurrentRolePropFields[6] = value;
					num = value;
					break;
				case 9:
					Global.Data.CurrentRolePropFields[10] = value;
					num = value;
					break;
				case 10:
					Global.Data.CurrentRolePropFields[11] = value;
					num = value;
					break;
				case 13:
					Global.Data.CurrentRolePropFields[15] = value;
					num = value;
					break;
				case 15:
					Global.Data.CurrentRolePropFields[16] = value;
					num = value;
					break;
				case 18:
					Global.Data.CurrentRolePropFields[18] = value;
					num = value;
					break;
				case 19:
					Global.Data.CurrentRolePropFields[19] = value;
					num = value;
					break;
				}
			}
			return num;
		}

		public static bool IsTrueOfDengluDaliTime()
		{
			if (Global.DengluDaliStartTime == string.Empty || Global.DengluDaliEndTime == string.Empty)
			{
				XElement isolateResXml = Global.GetIsolateResXml("Config/Gifts/HuoDongLoginNumGift.Xml");
				if (isolateResXml == null)
				{
					return false;
				}
				List<XElement> xelementList = Global.GetXElementList(isolateResXml, "Activities");
				if (xelementList == null)
				{
					return false;
				}
				XElement xelement = xelementList[0];
				if (xelement != null)
				{
					Global.DengluDaliStartTime = Global.GetHuodongTimeStr(0, 0, 0, 0);
					Global.DengluDaliEndTime = Global.GetHuodongTimeStr(6, 23, 59, 59);
				}
			}
			return Global.InLimitTimeRange(Global.DengluDaliStartTime, Global.DengluDaliEndTime);
		}

		public static bool IsLingquDengluDali(int index)
		{
			HuodongData myHuoDongData = Global.Data.MyHuoDongData;
			int resource = (Global.Data.MyHuoDongData != null) ? Global.Data.MyHuoDongData.LimitTimeGiftState : 0;
			return Global.GetIntSomeBit(resource, index) > 0;
		}

		public static int GetMyTimer()
		{
			long num = (DateTime.Now.Ticks - MyDateTime.Before1970Ticks) / 10000L;
			return (int)num;
		}

		public static string GetStatURLByPingTaiName(string eventName)
		{
			string result = string.Empty;
			if ("360" == Global.PingTaiName)
			{
				result = StringUtil.substitute("http://s.1360.cn/game_event?game=mst&server=S{0}&qid={1}&event={2}&time={3}", new object[]
				{
					Global.Data.GameServerID,
					Global.Data.UserID,
					eventName,
					DateTime.Now.Ticks / 10000L / 1000L
				});
			}
			else if (!string.IsNullOrEmpty(Global.Data.ReportStatURL))
			{
				int num = 1;
				if ("beforeloadflash" == eventName)
				{
					num = 1;
				}
				else if ("flashloaded" == eventName)
				{
					num = 2;
				}
				else if ("playercreated" == eventName)
				{
					num = 3;
				}
				else if ("entergame" == eventName)
				{
					num = 4;
				}
				result = StringUtil.substitute("http://{0}/api/game_event.php?serverid={1}&userid={2}&event={3}&time={4}", new object[]
				{
					Global.Data.ReportStatURL,
					Global.Data.GameServerID,
					Global.Data.UserID,
					num,
					Global.GetMyTimer()
				});
			}
			return result;
		}

		public static void NotifyStatURL(string eventName)
		{
			string statURLByPingTaiName = Global.GetStatURLByPingTaiName(eventName);
			if (!string.IsNullOrEmpty(statURLByPingTaiName))
			{
				Global.NotifyURL(statURLByPingTaiName);
			}
		}

		public static void NotifyURL(string url)
		{
		}

		public static bool GetGoodsSaleBackJingyuaAndExp(GoodsData goodsData, out int jingYuan, out int exp)
		{
			jingYuan = 0;
			exp = 0;
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("JingYuanExchange", ',');
			if (systemParamIntArrayByName == null || systemParamIntArrayByName.Length != 2)
			{
				return false;
			}
			int num = systemParamIntArrayByName[0];
			int num2 = systemParamIntArrayByName[1];
			if (goodsData == null || goodsData.Site != 0 || goodsData.Using > 0)
			{
				return false;
			}
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsData.GoodsID);
			if (goodsCatetoriy < 0 || goodsCatetoriy > 25)
			{
				return false;
			}
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			if (goodsXmlNodeByID.ToLevel < num)
			{
				return false;
			}
			int changeJinYuan = goodsXmlNodeByID.ChangeJinYuan;
			int num3 = changeJinYuan * num2;
			jingYuan = changeJinYuan;
			exp = num3;
			return true;
		}

		public static int GetZhuanhuangBuffID(int index)
		{
			if (index > 0)
			{
				if (Global.WuxueBuffIDs == null)
				{
					Global.zhuanhuangBuffIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ZhuanhuangBufferGoodsIDs", ',');
				}
				return Global.zhuanhuangBuffIDs[index - 1];
			}
			return 0;
		}

		public static long GetZaJinDanJiFenMaskValue(int awardNo)
		{
			XElement gameResXml = Global.GetGameResXml("Config/LuckyAward2.Xml");
			if (gameResXml == null)
			{
				return 0L;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
			List<int> list = new List<int>();
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
				list.Add(xelementAttributeInt);
			}
			list.Sort((int x, int y) => x - y);
			int num = list.IndexOf(awardNo);
			long num2 = 1L;
			if (num > 0)
			{
				num2 <<= num;
			}
			return num2;
		}

		public static bool IsZaJinDanAwardHasBeenGot(int awardNo, int bits)
		{
			long zaJinDanJiFenMaskValue = Global.GetZaJinDanJiFenMaskValue(awardNo);
			return ((long)bits & zaJinDanJiFenMaskValue) != 0L;
		}

		public static bool IsZaJinDanAwardCanBeenGot(int awardNo, int jiFen)
		{
			XElement gameResXml = Global.GetGameResXml("Config/LuckyAward2.Xml");
			if (gameResXml == null)
			{
				return false;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
			int num = 99999999;
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (xelementAttributeInt == awardNo)
				{
					num = Global.GetXElementAttributeInt(xelement, "MinLucky");
					break;
				}
			}
			return jiFen >= num;
		}

		public static double GetZaJinDanMaxLuckyValue()
		{
			XElement gameResXml = Global.GetGameResXml("Config/LuckyAward2.Xml");
			if (gameResXml == null)
			{
				return 0.0;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Award");
			double num = 0.0;
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "MinLucky");
				if (num < (double)xelementAttributeInt)
				{
					num = (double)xelementAttributeInt;
				}
			}
			return num;
		}

		public static bool IsDongJieSprite(GSprite sprite)
		{
			if (sprite == null)
			{
				return true;
			}
			RoleData roleData = null;
			if (sprite.SpriteType == GSpriteTypes.Leader)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(sprite.RoleID, ref roleData))
			{
				return false;
			}
			return roleData != null && roleData.DongJieStart > 0L && roleData.DongJieMills > 0;
		}

		public static string StringReplaceAll(string source, string find, string replacement)
		{
			string result = string.Empty;
			if (source != null && replacement != null && find != null)
			{
				result = source.Replace(find, replacement);
			}
			return result;
		}

		public static string StringTrim(string source)
		{
			return StringUtil.trim(source);
		}

		public static string FormatStr(string mask, string value)
		{
			int num = mask.Length - value.Length;
			if (num > 0)
			{
				return mask.Substring(0, num) + value;
			}
			return value;
		}

		public static string FormatStr(string mask, int value)
		{
			return Global.FormatStr(mask, value.ToString());
		}

		public static string FormatStr(string mask, double value)
		{
			return Global.FormatStr(mask, value.ToString());
		}

		public static XElement GetShaderXmlNodeByID(int id, bool bFashion = false)
		{
			if (id <= 0)
			{
				return null;
			}
			XElement xelement = null;
			if (Global.ShaderXmlNodeDict.TryGetValue(id, ref xelement))
			{
				return xelement;
			}
			if (bFashion)
			{
				XElement gameResXml = Global.GetGameResXml("Config/ShiZhuangShaders.xml");
				if (gameResXml == null)
				{
					return null;
				}
				xelement = Global.GetXElement(gameResXml, "MaterialConfig", "ID", id.ToString());
			}
			else
			{
				XElement gameResXml2 = Global.GetGameResXml("Config/Shaders.xml");
				if (gameResXml2 == null)
				{
					return null;
				}
				xelement = Global.GetXElement(gameResXml2, "Shader", "ID", id.ToString());
			}
			if (xelement == null)
			{
				return null;
			}
			Global.ShaderXmlNodeDict[id] = xelement;
			return xelement;
		}

		internal static string GetErrorMsg(DownloadEventArgs e)
		{
			return e.ToString();
		}

		public static void PlaySoundAudio(string playingMusicFile, bool loop = false)
		{
			if (Global.Data.SysSetting.CloseGameAudio)
			{
				return;
			}
			if (null != Super.GData.GlobalUIAudioSource)
			{
				Super.GData.GlobalUIAudioSource.PlayAudio(playingMusicFile, loop, false);
			}
			else
			{
				GameObject gameObject = GameObject.Find("UI Root (2D)");
				if (gameObject != null)
				{
					if (gameObject.GetComponent<AudioListener>() != null)
					{
						gameObject.GetComponent<AudioListener>().enabled = true;
					}
					if (gameObject.GetComponent<MainGame>() != null)
					{
						Super.GData.GlobalUIAudioSource = gameObject.GetComponent<MainGame>().GlobalUIAudioSource;
					}
					Global.PlaySoundAudio(playingMusicFile, loop);
				}
			}
		}

		public static void SetGlobalGameCursor(object obj, int arg)
		{
		}

		public static void ArrayRemove<T>(List<T> list, T type) where T : class
		{
			list.Remove(type);
		}

		public static bool CanUpgradeNow(object obj)
		{
			return true;
		}

		public static BitmapData GetSaleBitmapData(BitmapData src, int width, int height)
		{
			return null;
		}

		public static int CalcOriginalOccupationID(int Occupation)
		{
			if (Occupation < 10)
			{
				return Occupation;
			}
			int num = Occupation % 10;
			return (Occupation - num) / 10 - 1;
		}

		public static int CalcChangeOccupationID(int nCurOcc)
		{
			if (nCurOcc > 10)
			{
				return nCurOcc + 1;
			}
			return (nCurOcc + 1) * 10 + 1;
		}

		public static List<int> GetOccupationsInherit(int occupation)
		{
			List<int> list = new List<int>();
			int num = Global.CalcOriginalOccupationID(occupation);
			do
			{
				list.Add(num);
				num = Global.CalcChangeOccupationID(num);
			}
			while (num <= occupation);
			return list;
		}

		public static bool ValidOccupation(int toOccupation, int myOccupation = -1)
		{
			if (toOccupation < 0)
			{
				return true;
			}
			if (myOccupation < 0)
			{
				myOccupation = Global.Data.roleData.Occupation;
			}
			if ((toOccupation & 1 << myOccupation) != 0)
			{
				return true;
			}
			int num = Global.CalcOriginalOccupationID(myOccupation);
			while ((toOccupation & 1 << num) == 0)
			{
				num = Global.CalcChangeOccupationID(num);
				if (num > toOccupation)
				{
					return false;
				}
			}
			return true;
		}

		public static int GetUnionLevel(int zhuanSheng = -1, int level = -1)
		{
			if (Global.Data.roleData == null)
			{
				return 1;
			}
			if (zhuanSheng < 0)
			{
				return Global.Data.roleData.ChangeLifeCount * 65536 + Global.Data.roleData.Level;
			}
			return zhuanSheng * 65536 + level;
		}

		public static bool CanHintZhuanZhi(bool checkFee = true)
		{
			return false;
		}

		public static bool CanHintZhuanSheng(bool checkFee = true)
		{
			bool result = true;
			if (Global.Data == null || Global.Data.roleData == null)
			{
				return false;
			}
			if (Global.Data.roleData.Level < 100)
			{
				return false;
			}
			int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
			int num = changeLifeCount + 1;
			int occupation = Global.Data.roleData.Occupation;
			if (ConfigSystemParam.GetSystemParamIntByName("ChangeLifeMaxValue") < (long)num)
			{
				return false;
			}
			XElement gameResXml = Global.GetGameResXml(Global.GetZhuanShengXmlFilePath());
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameResXml, "ZhuanShengs");
			if (xelement == null)
			{
				return false;
			}
			XElement xelement2 = Global.GetXElement(xelement, "ZhuanSheng", "ChangeLifeID", num.ToString());
			if (xelement2 == null)
			{
				return false;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Level");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "NeedJinBi");
			int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement2, "NeedMoJing");
			string[] array = Global.GetXElementAttributeStr(xelement2, "NeedGoods").Split(new char[]
			{
				'|'
			});
			int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement2, "AwardShuXing");
			string[] array2 = Global.GetXElementAttributeStr(xelement2, "AwardGoods").Split(new char[]
			{
				'|'
			});
			if (Global.Data.roleData.Level < xelementAttributeInt)
			{
				result = false;
			}
			if (!checkFee)
			{
				return true;
			}
			if (xelementAttributeInt2 > 0 && Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < xelementAttributeInt2)
			{
				result = false;
			}
			if (xelementAttributeInt3 > 0 && Global.GetRoleOwnNumByMoneyType(13) < xelementAttributeInt3)
			{
				result = false;
			}
			if (array.Length > 0 && array[0].Length > 10)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array3 = array[i].Split(new char[]
					{
						','
					});
					if (array3.Length >= 2)
					{
						int num2 = Convert.ToInt32(array3[0]);
						int num3 = Convert.ToInt32(array3[1]);
						string goodsNameByID = Global.GetGoodsNameByID(num2, false);
						int totalGoodsCountByID = Global.GetTotalGoodsCountByID(num2);
						if (totalGoodsCountByID < num3)
						{
							result = false;
							break;
						}
					}
				}
			}
			return result;
		}

		public static int GetItemCategoriyByBodyPartID(int bodyPartID)
		{
			return bodyPartID;
		}

		public static void AddSpecialGameObjects(GameObject root, List<string> emptyObjectNames, List<List<string>> specialObjectsList, int layer, float particlescale = 1f, AssetbundleLoaderComplete assetLoaderComplete = null)
		{
			if (emptyObjectNames == null || specialObjectsList == null)
			{
				return;
			}
			for (int i = 0; i < emptyObjectNames.Count; i++)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(root, emptyObjectNames[i]);
				if (!(null == gameObject))
				{
					for (int j = 0; j < specialObjectsList[i].Count; j++)
					{
						DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(Global.SafeConvertToInt32(specialObjectsList[i][j]));
						if (decorationVOByCode != null)
						{
							string bundleID = MuAssetManager.GetBundleID("Decoration", decorationVOByCode.ResName);
							GameObject emptyLoader = U3DUtils.GetEmptyLoader(specialObjectsList[i][j], bundleID, false, assetLoaderComplete, null, -1, null, layer, particlescale, true, false, null);
							U3DUtils.AddChild(gameObject, emptyLoader, true);
						}
					}
				}
			}
		}

		public static void AddSpecialGameObjects(GameObject root, XElement xmlNode, int layer, float particlescale = 1f)
		{
			List<string> emptyObjectNames = null;
			List<List<string>> specialObjectsList = null;
			if (!Global.Get3DSubObjectsByXmlNode(xmlNode, out emptyObjectNames, out specialObjectsList))
			{
				return;
			}
			Global.AddSpecialGameObjects(root, emptyObjectNames, specialObjectsList, layer, particlescale, null);
		}

		public static void AddSpecialGameObjects(GameObject root, NPCInfoVO npcVO, int layer, float particlescale = 1f)
		{
			List<string> emptyObjectNames = null;
			List<List<string>> specialObjectsList = null;
			if (!Global.Get3DSubObjectsByXmlNode(npcVO, out emptyObjectNames, out specialObjectsList))
			{
				return;
			}
			Global.AddSpecialGameObjects(root, emptyObjectNames, specialObjectsList, layer, particlescale, null);
		}

		public static void AddSpecialGameObjects(GameObject root, MonsterVO monsterVO, int layer, float particlescale = 1f)
		{
			List<string> emptyObjectNames = null;
			List<List<string>> specialObjectsList = null;
			if (!Global.Get3DSubObjectsByXmlNode(monsterVO, out emptyObjectNames, out specialObjectsList))
			{
				return;
			}
			Global.AddSpecialGameObjects(root, emptyObjectNames, specialObjectsList, layer, particlescale, null);
		}

		public static void AddSpecialGameObjects(GameObject root, GoodVO goodVO, int layer, float particlescale = 1f, AssetbundleLoaderComplete assetLoaderComplete = null)
		{
			List<string> emptyObjectNames = null;
			List<List<string>> specialObjectsList = null;
			if (!Global.Get3DSubObjectsByXmlNode(goodVO, out emptyObjectNames, out specialObjectsList))
			{
				return;
			}
			Global.AddSpecialGameObjects(root, emptyObjectNames, specialObjectsList, layer, particlescale, assetLoaderComplete);
		}

		public static void AddSpecialGameObjects4Monster(GameObject root, int monsterID, int layer = -1, float particlescale = 1f)
		{
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
			if (monsterXmlNodeByID == null)
			{
				return;
			}
			Global.AddSpecialGameObjects(root, monsterXmlNodeByID, layer, particlescale);
		}

		public static void AddSpecialGameObjects4NPC(GameObject root, int npcID)
		{
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
			if (npcvobyID == null)
			{
				return;
			}
			Global.AddSpecialGameObjects(root, npcvobyID, -1, 1f);
		}

		public static void AddSpecialGameObjects4Goods(GameObject root, int goodsID, int layer, float particlescale = 1f, AssetbundleLoaderComplete assetLoaderComplete = null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
			if (goodsXmlNodeByID == null)
			{
				return;
			}
			Global.AddSpecialGameObjects(root, goodsXmlNodeByID, layer, particlescale, assetLoaderComplete);
		}

		private static bool Get3DSubObjectsByXmlNode(XElement xmlNode, out List<string> emtpyObjectNames, out List<List<string>> subObjectsList)
		{
			emtpyObjectNames = null;
			subObjectsList = null;
			if (xmlNode == null)
			{
				return false;
			}
			Caching3DSubObjectsItem caching3DSubObjectsItem = null;
			if (Global.Caching3DSubObjectsItemDict.TryGetValue(xmlNode, ref caching3DSubObjectsItem))
			{
				emtpyObjectNames = caching3DSubObjectsItem.emtpyObjectNames;
				subObjectsList = caching3DSubObjectsItem.subObjectsList;
				return true;
			}
			string xelementAttributeStr = Global.GetXElementAttributeStr(xmlNode, "GuaJieDian");
			if (string.IsNullOrEmpty(xelementAttributeStr))
			{
				return false;
			}
			string[] array = xelementAttributeStr.Split(new char[]
			{
				'|'
			});
			string xelementAttributeStr2 = Global.GetXElementAttributeStr(xmlNode, "GuaJieTeXiao");
			if (string.IsNullOrEmpty(xelementAttributeStr2))
			{
				return false;
			}
			string[] array2 = xelementAttributeStr2.Split(new char[]
			{
				'|'
			});
			if (array.Length != array2.Length)
			{
				return false;
			}
			emtpyObjectNames = Enumerable.ToList<string>(array);
			subObjectsList = new List<List<string>>();
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					','
				});
				subObjectsList.Add(Enumerable.ToList<string>(array3));
			}
			caching3DSubObjectsItem = new Caching3DSubObjectsItem
			{
				emtpyObjectNames = emtpyObjectNames,
				subObjectsList = subObjectsList
			};
			Global.Caching3DSubObjectsItemDict[xmlNode] = caching3DSubObjectsItem;
			return true;
		}

		private static bool Get3DSubObjectsByXmlNode(NPCInfoVO npcVO, out List<string> emtpyObjectNames, out List<List<string>> subObjectsList)
		{
			emtpyObjectNames = null;
			subObjectsList = null;
			if (npcVO == null)
			{
				return false;
			}
			Caching3DSubObjectsItem caching3DSubObjectsItem = null;
			if (Global.Caching3DSubObjectsItemVODict.TryGetValue("NPC_" + npcVO.ID, ref caching3DSubObjectsItem))
			{
				emtpyObjectNames = caching3DSubObjectsItem.emtpyObjectNames;
				subObjectsList = caching3DSubObjectsItem.subObjectsList;
				return true;
			}
			string guaJieDian = npcVO.GuaJieDian;
			if (string.IsNullOrEmpty(guaJieDian))
			{
				return false;
			}
			string[] array = guaJieDian.Split(new char[]
			{
				'|'
			});
			string guaJieTeXiao = npcVO.GuaJieTeXiao;
			if (string.IsNullOrEmpty(guaJieTeXiao))
			{
				return false;
			}
			string[] array2 = guaJieTeXiao.Split(new char[]
			{
				'|'
			});
			if (array.Length != array2.Length)
			{
				return false;
			}
			emtpyObjectNames = Enumerable.ToList<string>(array);
			subObjectsList = new List<List<string>>();
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					','
				});
				subObjectsList.Add(Enumerable.ToList<string>(array3));
			}
			caching3DSubObjectsItem = new Caching3DSubObjectsItem
			{
				emtpyObjectNames = emtpyObjectNames,
				subObjectsList = subObjectsList
			};
			Global.Caching3DSubObjectsItemVODict["NPC_" + npcVO.ID] = caching3DSubObjectsItem;
			return true;
		}

		private static bool Get3DSubObjectsByXmlNode(MonsterVO monsterVO, out List<string> emtpyObjectNames, out List<List<string>> subObjectsList)
		{
			emtpyObjectNames = null;
			subObjectsList = null;
			if (monsterVO == null)
			{
				return false;
			}
			Caching3DSubObjectsItem caching3DSubObjectsItem = null;
			if (Global.Caching3DSubObjectsItemMonsterVODict.TryGetValue("Monster_" + monsterVO.ID, ref caching3DSubObjectsItem))
			{
				emtpyObjectNames = caching3DSubObjectsItem.emtpyObjectNames;
				subObjectsList = caching3DSubObjectsItem.subObjectsList;
				return true;
			}
			string guaJieDian = monsterVO.GuaJieDian;
			if (string.IsNullOrEmpty(guaJieDian))
			{
				return false;
			}
			string[] array = guaJieDian.Split(new char[]
			{
				'|'
			});
			string guaJieTeXiao = monsterVO.GuaJieTeXiao;
			if (string.IsNullOrEmpty(guaJieTeXiao))
			{
				return false;
			}
			string[] array2 = guaJieTeXiao.Split(new char[]
			{
				'|'
			});
			if (array.Length != array2.Length)
			{
				return false;
			}
			emtpyObjectNames = Enumerable.ToList<string>(array);
			subObjectsList = new List<List<string>>();
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					','
				});
				subObjectsList.Add(Enumerable.ToList<string>(array3));
			}
			caching3DSubObjectsItem = new Caching3DSubObjectsItem
			{
				emtpyObjectNames = emtpyObjectNames,
				subObjectsList = subObjectsList
			};
			Global.Caching3DSubObjectsItemMonsterVODict["Monster_" + monsterVO.ID] = caching3DSubObjectsItem;
			return true;
		}

		private static bool Get3DSubObjectsByXmlNode(GoodVO goodVO, out List<string> emtpyObjectNames, out List<List<string>> subObjectsList)
		{
			emtpyObjectNames = null;
			subObjectsList = null;
			if (goodVO == null)
			{
				return false;
			}
			Caching3DSubObjectsItem caching3DSubObjectsItem = null;
			if (Global.Caching3DSubObjectsItemGoodVODict.TryGetValue("Good_" + goodVO.ID, ref caching3DSubObjectsItem))
			{
				emtpyObjectNames = caching3DSubObjectsItem.emtpyObjectNames;
				subObjectsList = caching3DSubObjectsItem.subObjectsList;
				return true;
			}
			string guaJieDian = goodVO.GuaJieDian;
			if (string.IsNullOrEmpty(guaJieDian))
			{
				return false;
			}
			string[] array = guaJieDian.Split(new char[]
			{
				'|'
			});
			string guaJieTeXiao = goodVO.GuaJieTeXiao;
			if (string.IsNullOrEmpty(guaJieTeXiao))
			{
				return false;
			}
			string[] array2 = guaJieTeXiao.Split(new char[]
			{
				'|'
			});
			if (array.Length != array2.Length)
			{
				return false;
			}
			emtpyObjectNames = Enumerable.ToList<string>(array);
			subObjectsList = new List<List<string>>();
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					','
				});
				subObjectsList.Add(Enumerable.ToList<string>(array3));
			}
			caching3DSubObjectsItem = new Caching3DSubObjectsItem
			{
				emtpyObjectNames = emtpyObjectNames,
				subObjectsList = subObjectsList
			};
			Global.Caching3DSubObjectsItemGoodVODict["Good_" + goodVO.ID] = caching3DSubObjectsItem;
			return true;
		}

		public static void ReplaceMaterials(GameObject go, XElement xmlNode)
		{
			if (xmlNode == null)
			{
				return;
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xmlNode, "ShaderID");
			if (xelementAttributeInt <= 0)
			{
				return;
			}
			U3DUtils.ReplaceMaterials(go, xelementAttributeInt, false);
		}

		public static void ReplaceMaterials(GameObject go, int shaderID)
		{
			if (shaderID <= 0)
			{
				return;
			}
			U3DUtils.ReplaceMaterials(go, shaderID, false);
		}

		public static void ReplaceMaterials4Monster(GameObject go, int monsterID)
		{
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
			if (monsterXmlNodeByID == null)
			{
				return;
			}
			if (monsterXmlNodeByID.ShaderID <= 0)
			{
				return;
			}
			U3DUtils.ReplaceMaterials4Monster(go, monsterXmlNodeByID.ShaderID, false);
		}

		public static void ReplaceMaterials4NPC(GameObject go, int npcID)
		{
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
			if (npcvobyID == null)
			{
				return;
			}
			Global.ReplaceMaterials(go, npcvobyID.ShaderID);
		}

		public static void HandleHandWeapon(MonsterNPCLoaderData monsertNPCLoader, int id, GSpriteTypes spriteType)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			if (spriteType == GSpriteTypes.Monster)
			{
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(id);
				text = monsterXmlNodeByID.YouShou;
				text2 = monsterXmlNodeByID.ZuoShou;
			}
			else if (spriteType == GSpriteTypes.NPC)
			{
				NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(id);
				text = npcvobyID.YouShou;
				text2 = npcvobyID.ZuoShou;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				if (array.Length == 2)
				{
					int rightWeaponID = Global.SafeConvertToInt32(array[0]);
					int rightShaderID = Global.SafeConvertToInt32(array[1]);
					monsertNPCLoader.rightWeaponID = rightWeaponID;
					monsertNPCLoader.rightShaderID = rightShaderID;
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				string[] array2 = text2.Split(new char[]
				{
					','
				});
				if (array2.Length == 2)
				{
					int leftWeaponID = Global.SafeConvertToInt32(array2[0]);
					int leftShaderID = Global.SafeConvertToInt32(array2[1]);
					monsertNPCLoader.leftWeaponID = leftWeaponID;
					monsertNPCLoader.leftShaderID = leftShaderID;
				}
			}
		}

		public static void HandleNpcWingsAndPet(GameObject go, int extensionID, OnWingsLoadComplete wingsLoaderComplete, OnShouHuChongLoadComplete shouHuChongLoaderComplete)
		{
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(extensionID);
			string wing = npcvobyID.Wing;
			if (!string.IsNullOrEmpty(wing))
			{
				int num = Global.SafeConvertToInt32(wing);
				if (num > 0)
				{
					GoodsData fakeEquipGoodsData = Global.GetFakeEquipGoodsData(num, 0, 0);
					if (fakeEquipGoodsData != null)
					{
						WingsLoadData wingsLoadData = new WingsLoadData();
						wingsLoadData.parent = go;
						wingsLoadData.HideGameEffect = Global.Data.SysSetting.HideGameEffect;
						string hangPointName = null;
						Global.ParseWingsGoodsDataInfo(out hangPointName);
						wingsLoadData.data = fakeEquipGoodsData;
						wingsLoadData.hangPointName = hangPointName;
						new WingsResLoader(wingsLoadData, wingsLoaderComplete);
					}
				}
			}
			string flyPet = npcvobyID.FlyPet;
			if (!string.IsNullOrEmpty(flyPet))
			{
				int num2 = Global.SafeConvertToInt32(flyPet);
				if (num2 > 0)
				{
					ShouHuChongLoadData shouHuChongLoadData = new ShouHuChongLoadData();
					shouHuChongLoadData.parent = go;
					shouHuChongLoadData.Occupation = 0;
					shouHuChongLoadData.HideGameEffect = Global.Data.SysSetting.HideGameEffect;
					GoodsData fakeEquipGoodsData2 = Global.GetFakeEquipGoodsData(num2, 0, 0);
					string emptyName = null;
					Global.ParseShouHuChongGoodsDataInfo(fakeEquipGoodsData2, out fakeEquipGoodsData2, out emptyName);
					shouHuChongLoadData.data = fakeEquipGoodsData2;
					shouHuChongLoadData.EmptyName = emptyName;
					if (fakeEquipGoodsData2 != null)
					{
						shouHuChongLoadData.Categoriy = (ItemCategories)Global.GetCategoriyByGoodsID(fakeEquipGoodsData2.GoodsID);
					}
					shouHuChongLoadData.SpecialGameObjectsComplete = new AssetbundleLoaderComplete(Global.ShouHuChongLoaderSpecialGameObjectsComplete);
					new ShouHuChongResLoader(shouHuChongLoadData, shouHuChongLoaderComplete);
				}
			}
		}

		public static void ShouHuChongLoaderSpecialGameObjectsComplete(AssetbundleLoader loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			go.AddComponent<LoadRoleShaderAgain>();
		}

		public static string GetSkeletonNameByOccupation(int occupation)
		{
			string result = string.Empty;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				result = "ZS_Skeleton";
			}
			else if (occupation == 1)
			{
				result = "FS_Skeleton";
			}
			else if (occupation == 2)
			{
				result = "GS_Skeleton";
			}
			else if (occupation == 3)
			{
				result = "MJS_Skeleton";
			}
			else if (occupation == 5)
			{
				result = "ZHS_Skeleton";
			}
			return result;
		}

		public static int GetOccupationBySkeletonName(string skeletonName)
		{
			int result = 0;
			if (skeletonName != null)
			{
				if (skeletonName.Equals("FS_Skeleton"))
				{
					result = 1;
				}
				else if (skeletonName.Equals("GS_Skeleton"))
				{
					result = 2;
				}
				else if (skeletonName.Equals("MJS_Skeleton"))
				{
					result = 3;
				}
				else if (skeletonName.Equals("ZHS_Skeleton"))
				{
					result = 5;
				}
			}
			return result;
		}

		public static string[] GetNakePartsList(int occupation)
		{
			string[] array = null;
			if (Global.NakePartsListCachingDict.TryGetValue(occupation, ref array))
			{
				return array;
			}
			List<string> list = new List<string>();
			if (occupation == 0)
			{
				list.Add("ZS_Head_01");
				list.Add("ZS_chest_01");
				list.Add("ZS_hand_01");
				list.Add("ZS_leg_01");
				list.Add("ZS_foot_01");
			}
			else if (occupation == 1)
			{
				list.Add("FS_Head_01");
				list.Add("FS_chest_01");
				list.Add("FS_hand_01");
				list.Add("FS_leg_01");
				list.Add("FS_foot_01");
			}
			else if (occupation == 2)
			{
				list.Add("GS_Head_01");
				list.Add("GS_chest_01");
				list.Add("GS_hand_01");
				list.Add("GS_leg_01");
				list.Add("GS_foot_01");
			}
			else if (occupation == 3)
			{
				list.Add("MJS_Head_01");
				list.Add("MJS_chest_01");
				list.Add("MJS_hand_01");
				list.Add("MJS_leg_01");
				list.Add("MJS_foot_01");
			}
			else if (occupation == 5)
			{
				list.Add("ZHS_Head_01");
				list.Add("ZHS_chest_01");
				list.Add("ZHS_hand_01");
				list.Add("ZHS_leg_01");
				list.Add("ZHS_foot_01");
			}
			array = Enumerable.ToArray<string>(list);
			Global.NakePartsListCachingDict.Add(occupation, array);
			return array;
		}

		public static GoodsData GetFakeEquipGoodsData(int goodsID, int forge_Level, int bagIndex = 0)
		{
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsID);
			if (dummyGoodsData == null)
			{
				return null;
			}
			dummyGoodsData.Forge_level = forge_Level;
			dummyGoodsData.Using = 1;
			dummyGoodsData.BagIndex = bagIndex;
			return dummyGoodsData;
		}

		public static List<GoodsData> GetTopLevelEquipGoodsDataList(int occupation, int equipIndex = 5, int forgeLevel = 13)
		{
			List<GoodsData> list = new List<GoodsData>();
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1000000 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000100 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000200 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000300 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000400 + equipIndex, forgeLevel, 0));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1010000 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010100 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010200 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010300 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010400 + equipIndex, forgeLevel, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1020000 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020100 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020200 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020300 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020400 + equipIndex, forgeLevel, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1030000 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030100 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030200 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030300 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030400 + equipIndex, forgeLevel, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1050000 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050100 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050200 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050300 + equipIndex, forgeLevel, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050400 + equipIndex, forgeLevel, 0));
			}
			return list;
		}

		public static List<GoodsData> GetBattleEquipGoodsDataList(int side, int occupation)
		{
			List<GoodsData> list = new List<GoodsData>();
			int num = (side != 1) ? 98 : 97;
			int forge_Level = 0;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1000000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000400 + num, forge_Level, 0));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1010000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010400 + num, forge_Level, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1020000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020400 + num, forge_Level, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1030000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030400 + num, forge_Level, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1050000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050400 + num, forge_Level, 0));
			}
			return list;
		}

		public static List<GoodsData> GetBattleWeaponGoodsDataList(int side, int occupation)
		{
			List<GoodsData> list = new List<GoodsData>();
			int forge_Level = 0;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (side == 2)
			{
				if (occupation == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1005080, forge_Level, 0));
					list.Add(Global.GetFakeEquipGoodsData(1005080, forge_Level, 1));
				}
				else if (occupation == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1015080, forge_Level, 0));
				}
				else if (occupation == 2)
				{
					list.Add(Global.GetFakeEquipGoodsData(1022480, forge_Level, 0));
				}
				else if (occupation == 3)
				{
					list.Add(Global.GetFakeEquipGoodsData(1035180, forge_Level, 0));
				}
				else if (occupation == 5)
				{
					list.Add(Global.GetFakeEquipGoodsData(1055181, forge_Level, 0));
				}
			}
			else if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1005081, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1005081, forge_Level, 1));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1015081, forge_Level, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1022481, forge_Level, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1035181, forge_Level, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1055180, forge_Level, 0));
			}
			return list;
		}

		public static List<GoodsData> GetPlunderBattleEquipGoodsDataList(int side, int occupation)
		{
			List<GoodsData> list = new List<GoodsData>();
			int num;
			if (side == 1)
			{
				num = 98;
			}
			else if (side == 2)
			{
				num = 97;
			}
			else
			{
				num = 99;
			}
			int forge_Level = 0;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1000000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000400 + num, forge_Level, 0));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1010000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010400 + num, forge_Level, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1020000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020400 + num, forge_Level, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1030000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030400 + num, forge_Level, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1050000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050400 + num, forge_Level, 0));
			}
			return list;
		}

		public static List<GoodsData> GetPlunderBattleWeaponGoodsDataList(int side, int occupation)
		{
			List<GoodsData> list = new List<GoodsData>();
			int forge_Level = 0;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (side == 1)
			{
				if (occupation == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1005080, forge_Level, 0));
					list.Add(Global.GetFakeEquipGoodsData(1005080, forge_Level, 1));
				}
				else if (occupation == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1015080, forge_Level, 0));
				}
				else if (occupation == 2)
				{
					list.Add(Global.GetFakeEquipGoodsData(1022480, forge_Level, 0));
				}
				else if (occupation == 3)
				{
					list.Add(Global.GetFakeEquipGoodsData(1035180, forge_Level, 0));
				}
				else if (occupation == 5)
				{
					list.Add(Global.GetFakeEquipGoodsData(1055181, forge_Level, 0));
				}
			}
			else if (side == 2)
			{
				if (occupation == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1005081, forge_Level, 0));
					list.Add(Global.GetFakeEquipGoodsData(1005081, forge_Level, 1));
				}
				else if (occupation == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1015081, forge_Level, 0));
				}
				else if (occupation == 2)
				{
					list.Add(Global.GetFakeEquipGoodsData(1022481, forge_Level, 0));
				}
				else if (occupation == 3)
				{
					list.Add(Global.GetFakeEquipGoodsData(1035181, forge_Level, 0));
				}
				else if (occupation == 5)
				{
					list.Add(Global.GetFakeEquipGoodsData(1055180, forge_Level, 0));
				}
			}
			else if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1005082, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1005082, forge_Level, 1));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1015082, forge_Level, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1022482, forge_Level, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1035182, forge_Level, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1055182, forge_Level, 0));
			}
			return list;
		}

		public static List<GoodsData> GetPKKingEquipGoodsDataList(int side, int occupation)
		{
			List<GoodsData> list = new List<GoodsData>();
			int num = (side != 1) ? 98 : 97;
			int forge_Level = 0;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1000000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1000400 + num, forge_Level, 0));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1010000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1010400 + num, forge_Level, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1020000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1020400 + num, forge_Level, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1030000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1030400 + num, forge_Level, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1050000 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050100 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050200 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050300 + num, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1050400 + num, forge_Level, 0));
			}
			return list;
		}

		public static List<GoodsData> GetPKKingWeaponGoodsDataList(int side, int occupation)
		{
			List<GoodsData> list = new List<GoodsData>();
			int forge_Level = 0;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1005005, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1005005, forge_Level, 1));
			}
			else if (occupation == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1015005, forge_Level, 0));
			}
			else if (occupation == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1025505, forge_Level, 0));
			}
			else if (occupation == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1035180, forge_Level, 0));
			}
			else if (occupation == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1055180, forge_Level, 0));
			}
			return list;
		}

		public static List<GoodsData> GetTopLevelWeaponGoodsData(int occupation, int equipIndex = 5, int forgeLevel = 13)
		{
			List<GoodsData> list = new List<GoodsData>();
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				if (Global.FakeEquipsIndex == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1005000 + equipIndex, forgeLevel, 0));
					list.Add(Global.GetFakeEquipGoodsData(1005000 + equipIndex, forgeLevel, 1));
				}
				else if (Global.FakeEquipsIndex == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1005099, forgeLevel, 0));
					list.Add(Global.GetFakeEquipGoodsData(1005099, forgeLevel, 1));
				}
			}
			else if (occupation == 1)
			{
				if (Global.FakeEquipsIndex == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1015000 + equipIndex, forgeLevel, 0));
				}
				else if (Global.FakeEquipsIndex == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1015099, forgeLevel, 0));
				}
				list.Add(Global.GetFakeEquipGoodsData(1015302, forgeLevel, 1));
			}
			else if (occupation == 2)
			{
				if (Global.FakeEquipsIndex == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1025400 + equipIndex, forgeLevel, 0));
				}
				else if (Global.FakeEquipsIndex == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1025599, forgeLevel, 0));
				}
			}
			else if (occupation == 3)
			{
				if (Global.FakeEquipsIndex == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1035400 + equipIndex, forgeLevel, 0));
				}
				else if (Global.FakeEquipsIndex == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1035599, forgeLevel, 0));
				}
			}
			else if (occupation == 5)
			{
				if (Global.FakeEquipsIndex == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1055400 + equipIndex, forgeLevel, 0));
				}
				else if (Global.FakeEquipsIndex == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1055599, forgeLevel, 0));
				}
			}
			return list;
		}

		public static GoodsData GetTopLevelShouHuGoodsData(int occupation)
		{
			GoodsData result = null;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				result = Global.GetFakeEquipGoodsData(1030903, 11, 0);
			}
			else if (occupation == 1)
			{
				result = Global.GetFakeEquipGoodsData(1030904, 11, 0);
			}
			else if (occupation == 2)
			{
				result = Global.GetFakeEquipGoodsData(1030904, 11, 0);
			}
			else if (occupation == 3)
			{
				result = Global.GetFakeEquipGoodsData(1030904, 11, 0);
			}
			else if (occupation == 5)
			{
				result = Global.GetFakeEquipGoodsData(1030904, 11, 0);
			}
			return result;
		}

		public static GoodsData GetTopLevelWingsHuGoodsData(int occupation)
		{
			GoodsData result = null;
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				result = Global.GetFakeEquipGoodsData(1000809, 13, 0);
			}
			else if (occupation == 1)
			{
				result = Global.GetFakeEquipGoodsData(1010809, 13, 0);
			}
			else if (occupation == 2)
			{
				result = Global.GetFakeEquipGoodsData(1020809, 13, 0);
			}
			else if (occupation == 3)
			{
				result = Global.GetFakeEquipGoodsData(1030809, 13, 0);
			}
			else if (occupation == 5)
			{
				result = Global.GetFakeEquipGoodsData(1050809, 13, 0);
			}
			return result;
		}

		public static int CheckRoleOcc(int Occ, int subOcc)
		{
			if ((Occ == 3 || Occ == 4) && subOcc != 0)
			{
				Occ = ((subOcc != 1) ? 3 : 4);
			}
			return Occ;
		}

		public static void CheckBagIndex(List<GoodsData> goodsDataList, int Occupation)
		{
			byte b = 0;
			if (goodsDataList != null)
			{
				for (int i = 0; i < goodsDataList.Count; i++)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataList[i].GoodsID);
					if (goodsXmlNodeByID.Categoriy == 17 && (Occupation == 3 || Occupation == 4) && goodsXmlNodeByID.ActionType == 1)
					{
						if (b == 0)
						{
							goodsDataList[i].BagIndex = 1;
							b = 1;
						}
						else
						{
							goodsDataList[i].BagIndex = 0;
						}
					}
				}
			}
		}

		public static void ParseWeaponGoodsDataInfo(List<GoodsData> goodsDataList, List<GoodsData> weaponGoodsDataList, List<string> emptyNamesList, List<string> safeEmptyNamesList, int Occupation)
		{
			if (goodsDataList == null || goodsDataList.Count <= 0)
			{
				return;
			}
			byte b = 0;
			List<GoodsData> list = new List<GoodsData>();
			for (int i = goodsDataList.Count - 1; i >= 0; i--)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataList[i].GoodsID);
				if (categoriyByGoodsID == 25 && 0 < goodsDataList[i].Using)
				{
					WuQiShiZhuangMoXingVO wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion = ConfigFashion.GetWuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion(goodsDataList[i].GoodsID, Occupation);
					if (wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion != null)
					{
						GoodsData goodsData = goodsDataList[i].Clone();
						GoodsData goodsData2 = goodsDataList[i].Clone();
						goodsData.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Left;
						goodsData.Id = -1;
						goodsData2.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Right;
						goodsData2.Id = -1;
						goodsData2.BagIndex = 1;
						if (0 < goodsData.GoodsID)
						{
							list.Add(goodsData);
							b += 1;
						}
						if (0 < goodsData2.GoodsID)
						{
							list.Add(goodsData2);
							b += 1;
						}
						break;
					}
				}
			}
			for (int j = 0; j < goodsDataList.Count; j++)
			{
				if (0 < b)
				{
					int categoriyByGoodsID2 = Global.GetCategoriyByGoodsID(goodsDataList[j].GoodsID);
					if (11 > categoriyByGoodsID2 && 21 < categoriyByGoodsID2)
					{
						list.Add(goodsDataList[j]);
					}
				}
				else
				{
					list.Add(goodsDataList[j]);
				}
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].Using > 0)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(list[k].GoodsID);
					if (goodsXmlNodeByID != null)
					{
						int categoriy = goodsXmlNodeByID.Categoriy;
						if (categoriy >= 11 && categoriy < 25)
						{
							int num = goodsXmlNodeByID.HandType;
							if (num > -1)
							{
								int actionType = goodsXmlNodeByID.ActionType;
								if (actionType > 0)
								{
									if (goodsXmlNodeByID.Categoriy == 17 && (Occupation == 3 || Occupation == 4) && goodsXmlNodeByID.ActionType == 1)
									{
										num = 2;
									}
									if (num == 1)
									{
										weaponGoodsDataList.Add(list[k]);
										if (categoriy == 20)
										{
											safeEmptyNamesList.Add(string.Empty);
											emptyNamesList.Add("gongjiantong");
										}
										else
										{
											int num2 = Global.CalcOriginalOccupationID(Occupation);
											if (actionType == 5 || (actionType == 2 && (num2 == 1 || (num2 == 3 && Global.GetMJSType(weaponGoodsDataList, Occupation, 0) == MJSSkillType.Magic_Sword) || num2 == 5)))
											{
												safeEmptyNamesList.Add("BW_changwu");
											}
											else if (categoriy == 15)
											{
												safeEmptyNamesList.Add("BW_nu");
											}
											else
											{
												safeEmptyNamesList.Add("BW_01");
											}
											emptyNamesList.Add("youshou");
										}
									}
									else if (num == 0)
									{
										weaponGoodsDataList.Add(list[k]);
										if (categoriy == 21)
										{
											safeEmptyNamesList.Add("nujiantong");
											emptyNamesList.Add("nujiantong");
										}
										else
										{
											if (categoriy == 18)
											{
												safeEmptyNamesList.Add("BW_dunpai");
											}
											else if (categoriy == 14)
											{
												safeEmptyNamesList.Add("BW_gong");
											}
											else
											{
												safeEmptyNamesList.Add("BW_02");
											}
											if (categoriy == 18)
											{
												emptyNamesList.Add("dun");
											}
											else
											{
												emptyNamesList.Add("zuoshou");
											}
										}
									}
									else if (num == 2)
									{
										weaponGoodsDataList.Add(list[k]);
										if (list[k].BagIndex <= 0)
										{
											safeEmptyNamesList.Add("BW_01");
										}
										else
										{
											safeEmptyNamesList.Add("BW_02");
										}
										if (list[k].BagIndex <= 0)
										{
											emptyNamesList.Add("youshou");
										}
										else
										{
											emptyNamesList.Add("zuoshou");
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void ParseShouHuChongGoodsDataInfo(GoodsData goodsData, out GoodsData shouHuChongGoodsData, out string emptyName)
		{
			shouHuChongGoodsData = null;
			emptyName = null;
			List<GoodsData> list = new List<GoodsData>();
			list.Add(goodsData);
			Global.ParseShouHuChongGoodsDataInfo(list, out shouHuChongGoodsData, out emptyName);
		}

		public static void ParseShouHuChongGoodsDataInfo(List<GoodsData> goodsDataList, out GoodsData shouHuChongGoodsData, out string emptyName)
		{
			shouHuChongGoodsData = null;
			emptyName = null;
			if (goodsDataList == null || goodsDataList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (goodsDataList[i].Using > 0)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataList[i].GoodsID);
					if (goodsXmlNodeByID != null)
					{
						int categoriy = goodsXmlNodeByID.Categoriy;
						if (categoriy == 9 || categoriy == 10)
						{
							shouHuChongGoodsData = goodsDataList[i];
							emptyName = "jingling";
							break;
						}
					}
				}
			}
		}

		public static void ParseWingsGoodsDataInfo(out string emptyName)
		{
			emptyName = "wing";
		}

		public static void ParseWingsGoodsDataInfo(WingData wingData, out GoodsData wingsGoodsData, out string emptyName, int occupation)
		{
			wingsGoodsData = null;
			emptyName = null;
			if (wingData == null)
			{
				return;
			}
			if (wingData.Using > 0)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(Global.GetWingXmlNodeByID(wingData.WingID, occupation), "GLGoods");
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(xelementAttributeInt);
				if (goodsXmlNodeByID == null)
				{
					return;
				}
				int categoriy = goodsXmlNodeByID.Categoriy;
				if (categoriy != 8)
				{
					return;
				}
				wingsGoodsData = new GoodsData();
				wingsGoodsData.Id = wingData.DbID;
				wingsGoodsData.GoodsID = xelementAttributeInt;
				wingsGoodsData.Using = wingData.Using;
				Global.ParseWingsGoodsDataInfo(out emptyName);
			}
		}

		public static void ParseStrongthenDecorationGoodsDataInfo(GoodsData goodsData, out GoodsData shouHuChongGoodsData, out string emptyName)
		{
			shouHuChongGoodsData = null;
			emptyName = null;
			List<GoodsData> list = new List<GoodsData>();
			list.Add(goodsData);
			Global.ParseStrongthenDecorationGoodsDataInfo(list, out shouHuChongGoodsData, out emptyName);
		}

		public static void ParseStrongthenDecorationGoodsDataInfo(List<GoodsData> goodsDataList, out GoodsData shouHuChongGoodsData, out string emptyName)
		{
			shouHuChongGoodsData = null;
			emptyName = null;
			if (goodsDataList == null || goodsDataList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (goodsDataList[i].Using > 0)
				{
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataList[i].GoodsID);
					if (goodsXmlNodeByID != null)
					{
						int categoriy = goodsXmlNodeByID.Categoriy;
						if (categoriy == 9 || categoriy == 10)
						{
							shouHuChongGoodsData = goodsDataList[i];
							emptyName = "jingling";
							break;
						}
					}
				}
			}
		}

		public static string GetSpecialSkillActionName(int occupation, string defaultName, WeaponStates weaponState, out string standName, List<GoodsData> goodData)
		{
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 3 || occupation == 4)
			{
				string text = "Z";
				if (goodData != null && Global.GetMJSType(goodData, occupation, 0) == MJSSkillType.Magic_Sword)
				{
					text = "F";
				}
				standName = text + weaponState + "XuanJuestand";
				if (weaponState == WeaponStates.K)
				{
					standName = weaponState + "XuanJuestand";
				}
				string result = text + weaponState + "XuanJueshow";
				if (weaponState == WeaponStates.K)
				{
					result = weaponState + "XuanJueshow";
				}
				return result;
			}
			standName = weaponState + "XuanJuestand";
			return weaponState + "XuanJueshow";
		}

		public static void PlaySpecialSkillDeco(int occupation, GameObject go)
		{
			occupation = Global.CalcOriginalOccupationID(occupation);
			if (occupation == 0)
			{
				GDecoration decoration = Global.GetDecoration(56, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, LayerMask.NameToLayer("MUUI"), true, false);
				decoration.Parent = go.transform;
			}
			else if (occupation == 1)
			{
				GDecoration decoration2 = Global.GetDecoration(56, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, LayerMask.NameToLayer("MUUI"), true, false);
				decoration2.Parent = go.transform;
			}
			else if (occupation == 2)
			{
				GDecoration decoration3 = Global.GetDecoration(56, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, LayerMask.NameToLayer("MUUI"), true, false);
				decoration3.Parent = go.transform;
			}
			else if (occupation != 3)
			{
				if (occupation == 5)
				{
				}
			}
		}

		public static GSprite FindSprite(string name)
		{
			if (Global.Data == null || Global.Data.GameScene == null)
			{
				return null;
			}
			GSprite gsprite = Global.Data.GameScene.FindSprite(name);
			if (gsprite == null)
			{
				return null;
			}
			return gsprite;
		}

		public static GSprite FindSpriteByID(int roleID)
		{
			string text = Global.FormatRoleID(roleID);
			GSprite gsprite = Global.Data.GameScene.FindSprite(text);
			if (gsprite == null)
			{
				return null;
			}
			return gsprite;
		}

		public static Transform GetTargetTrans(GSprite sprite, string layerName, out Vector3 meshSize, string[] ignoreList = null)
		{
			meshSize = Vector3.zero;
			if (sprite == null)
			{
				return null;
			}
			sprite.ModifyLayers(layerName, ignoreList);
			meshSize = sprite.GetOrigMeshSize();
			return sprite.The3DGameObject.transform;
		}

		public static string GetNPCName(int npcID)
		{
			npcID = 2130706432 + npcID;
			return string.Format("Role_{0}", npcID);
		}

		public static bool IsBloodCastleChengMen(int monsterID)
		{
			if (monsterID >= 600000 && monsterID < 700000 && monsterID % 100 == 6)
			{
				return true;
			}
			if (Global.BloodCastleChengMenIDs == null)
			{
				Global.BloodCastleChengMenIDs = ConfigSystemParam.GetSystemParamIntArrayByName("ChengMeng", ',');
				if (Global.BloodCastleChengMenIDs == null)
				{
					Global.BloodCastleChengMenIDs = new int[1];
				}
			}
			for (int i = 0; i < Global.BloodCastleChengMenIDs.Length; i++)
			{
				if (monsterID == Global.BloodCastleChengMenIDs[i])
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsBloodCastleLingGuan(int monsterID)
		{
			if (Global.BloodCastleLingGuanIDs == null)
			{
				Global.BloodCastleLingGuanIDs = ConfigSystemParam.GetSystemParamIntArrayByName("LingGuan", ',');
				if (Global.BloodCastleLingGuanIDs == null)
				{
					Global.BloodCastleLingGuanIDs = new int[1];
				}
			}
			for (int i = 0; i < Global.BloodCastleLingGuanIDs.Length; i++)
			{
				if (monsterID == Global.BloodCastleLingGuanIDs[i])
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsLangHunYaoSai(int monsterID)
		{
			if (Global.LangHunYaoSaiMonsters == null)
			{
				Global.LangHunYaoSaiMonsters = ConfigSystemParam.GetSystemParamIntArrayByName("LangHunYaoSaiMonsters", ',');
				if (Global.LangHunYaoSaiMonsters == null)
				{
					Global.LangHunYaoSaiMonsters = new int[1];
				}
			}
			return monsterID == Global.LangHunYaoSaiMonsters[0];
		}

		public static bool CanShowTaskBoxMiniRiChangTask()
		{
			return Global.Data.roleData.IsFlashPlayer == 0 && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.RiChangRenWu);
		}

		public static bool IsXinFuActivityEnd()
		{
			bool result = false;
			DateTime huodongTimeDateTime = Global.GetHuodongTimeDateTime(9, 0, 0, 0);
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime > huodongTimeDateTime)
			{
				result = true;
			}
			return result;
		}

		public static bool IsInOlympicsActivity()
		{
			if (Global.Data.roleData.ActivityList == null || Global.Data.roleData.ActivityList.Count == 0)
			{
				return false;
			}
			bool result = false;
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = default(DateTime);
			for (int i = 0; i < Global.Data.roleData.ActivityList.Count; i++)
			{
				if (Global.Data.roleData.ActivityList[i].ActivityType == 4)
				{
					dateTime2 = Global.Data.roleData.ActivityList[i].TimeBegin;
					dateTime = Global.Data.roleData.ActivityList[i].TimeAwardEnd;
					break;
				}
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime >= dateTime2 && correctDateTime <= dateTime)
			{
				result = true;
			}
			return result;
		}

		public static bool IsInOlympicsAwardActivity()
		{
			bool result = false;
			DateTime dateTime = default(DateTime);
			DateTime dateTime2 = default(DateTime);
			for (int i = 0; i < Global.Data.roleData.ActivityList.Count; i++)
			{
				if (Global.Data.roleData.ActivityList[i].ActivityType == 4)
				{
					dateTime2 = Global.Data.roleData.ActivityList[i].TimeEnd;
					dateTime = Global.Data.roleData.ActivityList[i].TimeAwardEnd;
					break;
				}
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime >= dateTime2 && correctDateTime <= dateTime)
			{
				result = true;
			}
			return result;
		}

		public static bool IsInZhuTiFuActivity()
		{
			return Global.Data != null && Global.Data.roleData != null && Global.Data.roleData.ThemeState == 1;
		}

		public static bool CanMoveByRay(GSprite sprite, int dir)
		{
			return true;
		}

		public static bool CanMoveByRay2(GSprite sprite, int dir)
		{
			return true;
		}

		public static int GetInputDirection(UIJoystick moveJoystick, out Vector2 joyPosition)
		{
			joyPosition = Vector2.zero;
			int num = -1;
			if (null == moveJoystick)
			{
				return -1;
			}
			if (MUBindingManager.Instance.beOpenMockMouse)
			{
				float analog = MUBindingManager.Instance.GetJoystickController().GetAnalog(MUControllerAnalogs.LEFTX);
				float num2 = 0f - MUBindingManager.Instance.GetJoystickController().GetAnalog(MUControllerAnalogs.LEFTY);
				moveJoystick.position = new Vector2(analog, num2);
			}
			if (moveJoystick.position.x != 0f || moveJoystick.position.y != 0f)
			{
				double angle = Global.GetAngle((double)((int)(moveJoystick.position.x * 1000f)), (double)((int)(moveJoystick.position.y * 1000f)));
				double num3 = Math.Abs(angle);
				if (num3 < 22.5)
				{
					num = 0;
				}
				else if (num3 >= 22.5 && num3 < 67.5)
				{
					num = ((angle <= 0.0) ? 7 : 1);
				}
				else if (num3 >= 67.5 && num3 < 112.5)
				{
					num = ((angle <= 0.0) ? 6 : 2);
				}
				else if (num3 >= 112.5 && num3 < 157.5)
				{
					num = ((angle <= 0.0) ? 5 : 3);
				}
				else if (num3 >= 157.5)
				{
					num = 4;
				}
				if (num >= 0)
				{
					num = (num + 1) % 8;
				}
				joyPosition = moveJoystick.position;
			}
			return num;
		}

		public static int GetInput36Direction(UIJoystick moveJoystick, out Vector2 joyPosition)
		{
			joyPosition = Vector2.zero;
			int num = -1;
			if (null == moveJoystick)
			{
				return -1;
			}
			if (!Global.IsCanMove)
			{
				return -1;
			}
			if (MUBindingManager.Instance.beOpenMockMouse)
			{
				float analog = MUBindingManager.Instance.GetJoystickController().GetAnalog(MUControllerAnalogs.LEFTX);
				float num2 = 0f - MUBindingManager.Instance.GetJoystickController().GetAnalog(MUControllerAnalogs.LEFTY);
				moveJoystick.position = new Vector2(analog, num2);
			}
			if (moveJoystick.position.x != 0f || moveJoystick.position.y != 0f)
			{
				double angle = Global.GetAngle((double)((int)(moveJoystick.position.x * 1000f)), (double)((int)(moveJoystick.position.y * 1000f)));
				double num3 = Math.Abs(angle);
				num = (int)(num3 / 10.0);
				num = ((angle <= 0.0) ? (36 - num) : num);
				if (num >= 0)
				{
					num = (num + 1) % 36;
				}
				joyPosition = moveJoystick.position;
			}
			return num;
		}

		public static bool NeedToMove()
		{
			UIJoystick joystick = Global.Joystick;
			if (null == joystick)
			{
				return false;
			}
			Vector2 zero = Vector2.zero;
			int inputDirection = Global.GetInputDirection(joystick, out zero);
			return inputDirection >= 0;
		}

		public static double GetGoodsDataZhanLi(GoodsData gd)
		{
			double num = 0.0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return num;
			}
			Dictionary<int, double> dictionary;
			if (!Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
			{
				dictionary = Global.GetCombatForceInfo(goodsXmlNodeByID.ToOccupation);
			}
			else
			{
				dictionary = Global.GetRebornCombatForceInfo();
			}
			if (goodsXmlNodeByID.Categoriy == 980)
			{
				Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
				if (dictionary.Count > 0)
				{
					Dictionary<string, double>.Enumerator enumerator = IConfigbase<ConfigShengYinShengJi>.Instance.GetPropByGoodsIDAndLevel(gd.GoodsID, gd.ElementhrtsProps[0]).GetEnumerator();
					while (enumerator.MoveNext())
					{
						double num2 = 0.0;
						KeyValuePair<string, double> keyValuePair = enumerator.Current;
						if (num2 < keyValuePair.Value)
						{
							KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
							int extPropIndexesIDByWord = ConfigExtPropIndexes.GetExtPropIndexesIDByWord(keyValuePair2.Key);
							if (extPropIndexesIDByWord != -1)
							{
								if (dictionary2.ContainsKey(extPropIndexesIDByWord))
								{
									Dictionary<int, double> dictionary4;
									Dictionary<int, double> dictionary3 = dictionary4 = dictionary2;
									int num4;
									int num3 = num4 = extPropIndexesIDByWord;
									double num5 = dictionary4[num4];
									double num6 = num5;
									KeyValuePair<string, double> keyValuePair3 = enumerator.Current;
									dictionary3[num3] = num6 + keyValuePair3.Value;
								}
								else
								{
									Dictionary<int, double> dictionary5 = dictionary2;
									int num7 = extPropIndexesIDByWord;
									KeyValuePair<string, double> keyValuePair4 = enumerator.Current;
									dictionary5.Add(num7, keyValuePair4.Value);
								}
							}
						}
					}
					List<int> washProps = gd.WashProps;
					if (washProps != null)
					{
						int i = 0;
						while (i < washProps.Count)
						{
							int num8 = washProps[++i];
							int num9 = washProps[++i] / 1000;
							i++;
							if (dictionary2.ContainsKey(num8))
							{
								Dictionary<int, double> dictionary7;
								Dictionary<int, double> dictionary6 = dictionary7 = dictionary2;
								int num4;
								int num10 = num4 = num8;
								double num5 = dictionary7[num4];
								dictionary6[num10] = num5 + (double)num9;
							}
							else
							{
								dictionary2.Add(num8, (double)num9);
							}
						}
					}
				}
				if (0 < dictionary2.Count)
				{
					Dictionary<int, double>.Enumerator enumerator2 = dictionary2.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						double num11 = 0.0;
						KeyValuePair<int, double> keyValuePair5 = enumerator2.Current;
						if (num11 < keyValuePair5.Value)
						{
							Dictionary<int, double> dictionary8 = dictionary;
							KeyValuePair<int, double> keyValuePair6 = enumerator2.Current;
							if (dictionary8.ContainsKey(keyValuePair6.Key))
							{
								KeyValuePair<int, double> keyValuePair7 = enumerator2.Current;
								double value = keyValuePair7.Value;
								Dictionary<int, double> dictionary9 = dictionary;
								KeyValuePair<int, double> keyValuePair8 = enumerator2.Current;
								int num12 = (int)(value / dictionary9[keyValuePair8.Key]);
								KeyValuePair<int, double> keyValuePair9 = enumerator2.Current;
								if (keyValuePair9.Key == 3)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair10 = enumerator2.Current;
								if (keyValuePair10.Key == 5)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair11 = enumerator2.Current;
								if (keyValuePair11.Key == 7)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair12 = enumerator2.Current;
								if (keyValuePair12.Key == 9)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair13 = enumerator2.Current;
								if (keyValuePair13.Key == 4)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair14 = enumerator2.Current;
								if (keyValuePair14.Key == 6)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair15 = enumerator2.Current;
								if (keyValuePair15.Key == 8)
								{
									goto IL_2EB;
								}
								KeyValuePair<int, double> keyValuePair16 = enumerator2.Current;
								if (keyValuePair16.Key == 10)
								{
									goto IL_2EB;
								}
								IL_2F1:
								num += (double)num12;
								continue;
								IL_2EB:
								num12 /= 2;
								goto IL_2F1;
							}
						}
					}
				}
			}
			else
			{
				double[] equipProps = goodsXmlNodeByID.EquipProps;
				if (equipProps != null && dictionary.Count > 0)
				{
					int[] array = Array.ConvertAll<double, int>(goodsXmlNodeByID.EquipProps, (double d) => (int)d);
					if (equipProps.Length == 177)
					{
						if ((double)array[13] != 0.0 && dictionary[13] != 0.0)
						{
							num += (double)Global.GetAttributeAddBaseValue(gd, array, 13) / dictionary[13];
						}
						if ((double)array[15] != 0.0 && dictionary[15] != 0.0)
						{
							num += (double)array[15] / dictionary[15];
						}
						if ((double)array[4] != 0.0 && dictionary[4] != 0.0)
						{
							int attributeAddBaseValue = Global.GetAttributeAddBaseValue(gd, array, 4);
							int attributeAddBaseValue2 = Global.GetAttributeAddBaseValue(gd, array, 3);
							num += ((double)attributeAddBaseValue / dictionary[4] + (double)attributeAddBaseValue2 / dictionary[3]) / 2.0;
						}
						if ((double)array[6] != 0.0 && dictionary[6] != 0.0)
						{
							int attributeAddBaseValue3 = Global.GetAttributeAddBaseValue(gd, array, 6);
							int attributeAddBaseValue4 = Global.GetAttributeAddBaseValue(gd, array, 5);
							num += ((double)attributeAddBaseValue3 / dictionary[6] + (double)attributeAddBaseValue4 / dictionary[5]) / 2.0;
							num += (double)(Global.GetGoodsWashProps(gd, 6) + Global.GetGoodsWashProps(gd, 5));
						}
						if ((double)array[8] != 0.0 && dictionary[8] != 0.0)
						{
							int attributeAddBaseValue5 = Global.GetAttributeAddBaseValue(gd, array, 8);
							int attributeAddBaseValue6 = Global.GetAttributeAddBaseValue(gd, array, 7);
							num += ((double)attributeAddBaseValue5 / dictionary[8] + (double)attributeAddBaseValue6 / dictionary[7]) / 2.0;
						}
						if ((double)array[10] != 0.0 && dictionary[10] != 0.0)
						{
							int attributeAddBaseValue7 = Global.GetAttributeAddBaseValue(gd, array, 10);
							int attributeAddBaseValue8 = Global.GetAttributeAddBaseValue(gd, array, 9);
							num += ((double)attributeAddBaseValue7 / dictionary[10] + (double)attributeAddBaseValue8 / dictionary[9]) / 2.0;
						}
						if ((double)array[18] != 0.0 && dictionary[18] != 0.0)
						{
							num += (double)array[18] / dictionary[18];
						}
						if ((double)array[19] != 0.0 && dictionary[19] != 0.0)
						{
							num += (double)array[19] / dictionary[19];
						}
						if ((double)array[27] != 0.0 && dictionary[27] != 0.0)
						{
							num += (double)array[27] / dictionary[27];
						}
						if ((double)array[38] != 0.0 && dictionary[38] != 0.0)
						{
							num += (double)array[38] / dictionary[38];
						}
						if ((double)array[44] != 0.0 && dictionary[44] != 0.0)
						{
							num += (double)array[44] / dictionary[44];
						}
						if ((double)array[45] != 0.0 && dictionary[45] != 0.0)
						{
							num += (double)array[45] / dictionary[45];
						}
						if ((double)array[46] != 0.0 && dictionary[46] != 0.0)
						{
							num += (double)array[46] / dictionary[46];
						}
						if (dictionary[13] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 13) / dictionary[13];
						}
						if (dictionary[15] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 15) / dictionary[15];
						}
						if (dictionary[18] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 18) / dictionary[18];
						}
						if (dictionary[19] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 19) / dictionary[19];
						}
						if (dictionary[4] != 0.0)
						{
							num += ((double)Global.GetGoodsWashProps(gd, 4) / dictionary[4] + (double)Global.GetGoodsWashProps(gd, 3) / dictionary[4]) / 2.0;
						}
						if (dictionary[6] != 0.0)
						{
							num += ((double)Global.GetGoodsWashProps(gd, 6) / dictionary[6] + (double)Global.GetGoodsWashProps(gd, 5) / dictionary[5]) / 2.0;
						}
						if (dictionary[8] != 0.0)
						{
							num += ((double)Global.GetGoodsWashProps(gd, 8) / dictionary[8] + (double)Global.GetGoodsWashProps(gd, 7) / dictionary[7]) / 2.0;
						}
						if (dictionary[10] != 0.0)
						{
							num += ((double)Global.GetGoodsWashProps(gd, 10) / dictionary[10] + (double)Global.GetGoodsWashProps(gd, 9) / dictionary[9]) / 2.0;
						}
						if (dictionary[27] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 27) / dictionary[27];
						}
						if (dictionary[38] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 38) / dictionary[38];
						}
						if (dictionary[44] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 44) / dictionary[44];
						}
						if (dictionary[45] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 45) / dictionary[45];
						}
						if (dictionary[46] != 0.0)
						{
							num += (double)Global.GetGoodsWashProps(gd, 46) / dictionary[46];
						}
						if ((double)array[69] != 0.0 && dictionary[69] != 0.0)
						{
							num += (double)array[69] / dictionary[69];
						}
						if ((double)array[70] != 0.0 && dictionary[70] != 0.0)
						{
							num += (double)array[70] / dictionary[70];
						}
						if ((double)array[71] != 0.0 && dictionary[71] != 0.0)
						{
							num += (double)array[71] / dictionary[71];
						}
						if ((double)array[72] != 0.0 && dictionary[72] != 0.0)
						{
							num += (double)array[72] / dictionary[72];
						}
						if ((double)array[73] != 0.0 && dictionary[73] != 0.0)
						{
							num += (double)array[73] / dictionary[73];
						}
						if ((double)array[74] != 0.0 && dictionary[74] != 0.0)
						{
							num += (double)array[74] / dictionary[74];
						}
						for (int j = 122; j <= 157; j += 7)
						{
							int num13 = j;
							float num14 = 0f;
							if ((double)array[num13] != 0.0 && dictionary[num13] != 0.0)
							{
								num14 = (float)array[num13];
								num14 = Global.GetEnlargeChongShengValue(num14, gd);
							}
							if (Global.GetGoodsWashProps(gd, num13) != 0)
							{
								num14 += (float)Global.GetGoodsWashProps(gd, num13) / 1000f;
							}
							int num15 = j + 1;
							float num16 = 0f;
							if ((double)array[num15] != 0.0 && dictionary[num15] != 0.0)
							{
								num16 = (float)array[num15];
								num16 = Global.GetEnlargeChongShengValue(num16, gd);
							}
							if (Global.GetGoodsWashProps(gd, num15) != 0)
							{
								num16 += (float)Global.GetGoodsWashProps(gd, num15) / 1000f;
							}
							if (j == 157)
							{
								for (int k = 122; k <= 150; k += 7)
								{
									if (num14 > 0f)
									{
										num += (double)num14 / dictionary[k];
									}
									if (num16 > 0f)
									{
										num += (double)num16 / dictionary[k + 1];
									}
								}
							}
							else
							{
								if (num14 > 0f)
								{
									num += (double)num14 / dictionary[num13];
								}
								if (num16 > 0f)
								{
									num += (double)num16 / dictionary[num15];
								}
							}
						}
					}
				}
			}
			return Math.Round(num);
		}

		private static float GetEnlargeChongShengValue(float baseValue, GoodsData gd)
		{
			int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
			float enlargeByZhuoYue = ChongShengData.GetEnlargeByZhuoYue(zhuoyueAttributeCount);
			return baseValue * enlargeByZhuoYue;
		}

		private static int GetGoodsWashProps(GoodsData gd, int extPropIndexes)
		{
			int num = 0;
			if (Global.GetCategoriyByGoodsID(gd.GoodsID) == 980)
			{
				List<int> washProps = gd.WashProps;
				if (washProps != null)
				{
					int i = 0;
					while (i < washProps.Count)
					{
						int num2 = washProps[++i];
						int num3 = washProps[++i] / 1000;
						i++;
						num += num3;
					}
				}
			}
			else if (gd.WashProps != null)
			{
				for (int j = 0; j < Enumerable.Count<int>(gd.WashProps); j += 2)
				{
					if (gd.WashProps[j] == extPropIndexes)
					{
						num += gd.WashProps[j + 1];
					}
				}
			}
			return num;
		}

		private static Dictionary<int, double> GetForceInfo(XElement xmlCombatForceInfo)
		{
			Dictionary<int, double> dictionary = new Dictionary<int, double>();
			dictionary[13] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "LifeV");
			dictionary[15] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MagicV");
			dictionary[4] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MaxDefenseV");
			dictionary[3] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MinDefenseV");
			dictionary[6] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MaxMDefenseV");
			dictionary[5] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MinMDefenseV");
			dictionary[8] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MaxAttackV");
			dictionary[7] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MinAttackV");
			dictionary[10] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MaxMAttackV");
			dictionary[9] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "MinMAttackV");
			dictionary[18] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "HitV");
			dictionary[19] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "Dodge");
			dictionary[27] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "AddAttackInjure");
			dictionary[38] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "DecreaseInjureValue");
			dictionary[44] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "LifeSteal");
			dictionary[45] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "AddAttack");
			dictionary[46] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "AddDefense");
			dictionary[69] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "FireAttack");
			dictionary[70] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "WaterAttack");
			dictionary[71] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "LightningAttack");
			dictionary[72] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "SoilAttack");
			dictionary[73] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "IceAttack");
			dictionary[74] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "WindAttack");
			dictionary[122] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "HolyAttack");
			dictionary[123] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "HolyDefense");
			dictionary[129] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "ShadowAttack");
			dictionary[130] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "ShadowDefense");
			dictionary[136] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "NatureAttack");
			dictionary[137] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "NatureDefense");
			dictionary[143] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "ChaosAttack");
			dictionary[144] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "ChaosDefense");
			dictionary[150] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "IncubusAttack");
			dictionary[151] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "IncubusDefense");
			dictionary[157] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "RebornAttack");
			dictionary[158] = Global.GetXElementAttributeDouble(xmlCombatForceInfo, "RebornDefense");
			return dictionary;
		}

		private static Dictionary<int, double> GetCombatForceInfo(int goodsOccupation)
		{
			Dictionary<int, double> result = new Dictionary<int, double>();
			XElement gameResXml = Global.GetGameResXml("Config/Roles/CombatForceInfo.xml");
			if (gameResXml == null)
			{
				return result;
			}
			if (goodsOccupation == -1)
			{
				goodsOccupation = 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "ZhanLi", "ID", "1");
			if (xelement == null)
			{
				return result;
			}
			return Global.GetForceInfo(xelement);
		}

		private static Dictionary<int, double> GetRebornCombatForceInfo()
		{
			Dictionary<int, double> result = new Dictionary<int, double>();
			XElement gameResXml = Global.GetGameResXml("Config/Roles/RebornCombatForce.xml");
			if (gameResXml == null)
			{
				return result;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Rebornzhanli", "ID", "1");
			if (xelement == null)
			{
				return result;
			}
			return Global.GetForceInfo(xelement);
		}

		public static Dictionary<int, int> GetGoodsBasicAttribute(GoodsData gd)
		{
			if (gd == null)
			{
				return null;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return dictionary;
			}
			int[] array = Array.ConvertAll<double, int>(goodsXmlNodeByID.EquipProps, (double d) => (int)d);
			if (array.Length == 177)
			{
				for (int i = 1; i < 177; i++)
				{
					dictionary[i] = Global.GetAttributeAddBaseValue(gd, array, i);
				}
				dictionary[0] = (int)Global.GetGoodsDataZhanLi(gd);
				dictionary[177] = gd.Lucky;
				dictionary[178] = 0;
				dictionary[179] = gd.ExcellenceInfo;
				if (Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
				{
					if (gd.WashProps != null)
					{
						for (int j = 0; j < gd.WashProps.Count; j += 2)
						{
							int num = gd.WashProps[j];
							int num2;
							if (ConfigExtPropIndexes.GetPercentByID(num))
							{
								num2 = gd.WashProps[j + 1] / 10;
							}
							else
							{
								num2 = gd.WashProps[j + 1] / 1000;
							}
							if (dictionary.ContainsKey(num))
							{
								Dictionary<int, int> dictionary3;
								Dictionary<int, int> dictionary2 = dictionary3 = dictionary;
								int num4;
								int num3 = num4 = num;
								num4 = dictionary3[num4];
								dictionary2[num3] = num4 + num2;
							}
						}
					}
				}
				else if (goodsXmlNodeByID.Categoriy == 980)
				{
					List<int> washProps = gd.WashProps;
					if (washProps != null)
					{
						int k = 0;
						while (k < washProps.Count)
						{
							int num5 = washProps[++k];
							int num6 = washProps[++k] / 1000;
							k++;
							Dictionary<int, int> dictionary5;
							Dictionary<int, int> dictionary4 = dictionary5 = dictionary;
							int num4;
							int num7 = num4 = num5;
							num4 = dictionary5[num4];
							dictionary4[num7] = num4 + num6;
						}
					}
					Dictionary<string, double>.Enumerator enumerator = IConfigbase<ConfigShengYinShengJi>.Instance.GetPropByGoodsIDAndLevel(gd.GoodsID, gd.ElementhrtsProps[0]).GetEnumerator();
					while (enumerator.MoveNext())
					{
						Dictionary<int, int> dictionary7;
						Dictionary<int, int> dictionary6 = dictionary7 = dictionary;
						KeyValuePair<string, double> keyValuePair = enumerator.Current;
						int num4;
						int num8 = num4 = ConfigExtPropIndexes.GetExtPropIndexesIDByWord(keyValuePair.Key);
						num4 = dictionary7[num4];
						int num9 = num4;
						KeyValuePair<string, double> keyValuePair2 = enumerator.Current;
						dictionary6[num8] = num9 + (int)keyValuePair2.Value;
					}
				}
				else if (gd.WashProps != null)
				{
					for (int l = 0; l < gd.WashProps.Count; l += 2)
					{
						int num10 = gd.WashProps[l];
						int num11 = gd.WashProps[l + 1];
						if (dictionary.ContainsKey(num10))
						{
							Dictionary<int, int> dictionary9;
							Dictionary<int, int> dictionary8 = dictionary9 = dictionary;
							int num4;
							int num12 = num4 = num10;
							num4 = dictionary9[num4];
							dictionary8[num12] = num4 + num11;
						}
					}
				}
			}
			return dictionary;
		}

		public static Dictionary<int, int> GetCompareAttributeInfo(GoodsData gd, HandTypes CompareType = HandTypes.None)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			Dictionary<int, int> goodsBasicAttribute = Global.GetGoodsBasicAttribute(gd);
			List<GoodsData> list = new List<GoodsData>();
			HandTypes handTypes = HandTypes.None;
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(gd.GoodsID);
			if (categoriyByGoodsID >= 11 && categoriyByGoodsID <= 21)
			{
				int goodsActionNameByID = (int)Global.GetGoodsActionNameByID(gd.GoodsID);
				GoodsData goodsData = Super.FindUsingEuip(categoriyByGoodsID, 0);
				GoodsData goodsData2 = Super.FindUsingEuip(categoriyByGoodsID, 1);
				Dictionary<int, int> goodsBasicAttribute2 = Global.GetGoodsBasicAttribute(goodsData);
				Dictionary<int, int> goodsBasicAttribute3 = Global.GetGoodsBasicAttribute(goodsData2);
				if (categoriyByGoodsID == 11 || categoriyByGoodsID == 12 || categoriyByGoodsID == 13 || categoriyByGoodsID == 16 || categoriyByGoodsID == 19)
				{
					if (goodsActionNameByID == 1)
					{
						if (goodsData2 == null || goodsData == null)
						{
							if (goodsData2 != null)
							{
								GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
								int categoriy = goodsXmlNodeByID.Categoriy;
								int handType = goodsXmlNodeByID.HandType;
								int actionType = goodsXmlNodeByID.ActionType;
								if (actionType == 2 || actionType == 7)
								{
									dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
								}
								else
								{
									dictionary = goodsBasicAttribute;
								}
							}
							else
							{
								dictionary = goodsBasicAttribute;
							}
						}
						else if (CompareType == HandTypes.ZuoShou)
						{
							GoodVO goodsXmlNodeByID2 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
							int categoriy2 = goodsXmlNodeByID2.Categoriy;
							int handType2 = goodsXmlNodeByID2.HandType;
							if (categoriy2 != 11 || categoriy2 != 12 || categoriy2 != 13 || categoriy2 != 16 || categoriy2 != 19)
							{
								dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
								handTypes = HandTypes.ZuoShou;
							}
						}
						else if (CompareType == HandTypes.YouShou)
						{
							GoodVO goodsXmlNodeByID3 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
							int categoriy3 = goodsXmlNodeByID3.Categoriy;
							int handType3 = goodsXmlNodeByID3.HandType;
							int actionType2 = goodsXmlNodeByID3.ActionType;
							if (actionType2 == 2 || actionType2 == 7)
							{
								dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
							}
							else if (categoriy3 != 11 || categoriy3 != 12 || categoriy3 != 13 || categoriy3 != 16 || categoriy3 != 19)
							{
								dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
								handTypes = HandTypes.YouShou;
							}
						}
						else if (CompareType == HandTypes.None)
						{
							dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
							handTypes = HandTypes.YouShou;
						}
					}
					else if (goodsActionNameByID == 2 || goodsActionNameByID == 5 || goodsActionNameByID == 7)
					{
						dictionary = Global.CalculatAttribute(goodsBasicAttribute2, goodsBasicAttribute3, "JIA");
						dictionary = Global.CalculatAttribute(goodsBasicAttribute, dictionary, "JIAN");
					}
				}
				else if (categoriyByGoodsID != 12)
				{
					if (categoriyByGoodsID != 13)
					{
						if (categoriyByGoodsID == 14)
						{
							if (goodsActionNameByID == 3)
							{
								if (goodsData != null)
								{
									dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
								}
								if (goodsData2 != null)
								{
									GoodVO goodsXmlNodeByID4 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
									int categoriy4 = goodsXmlNodeByID4.Categoriy;
									if (categoriy4 != 20)
									{
										dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
									}
								}
							}
						}
						else if (categoriyByGoodsID == 15)
						{
							if (goodsActionNameByID == 4)
							{
								if (goodsData != null)
								{
									int categoriy5 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).Categoriy;
									if (categoriy5 != 21)
									{
										dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
									}
								}
								if (goodsData2 != null)
								{
									dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
								}
							}
						}
						else if (categoriyByGoodsID != 16)
						{
							if (categoriyByGoodsID == 17)
							{
								if (Global.Data.roleData.Occupation == 3 && goodsActionNameByID == 1)
								{
									if (goodsData2 == null || goodsData == null)
									{
										if (goodsData2 != null)
										{
											GoodVO goodsXmlNodeByID5 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
											int categoriy6 = goodsXmlNodeByID5.Categoriy;
											int handType4 = goodsXmlNodeByID5.HandType;
											int actionType3 = goodsXmlNodeByID5.ActionType;
											if (actionType3 == 2 || actionType3 == 7)
											{
												dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
											}
											else
											{
												dictionary = goodsBasicAttribute;
											}
										}
										else
										{
											dictionary = goodsBasicAttribute;
										}
									}
									else if (CompareType == HandTypes.ZuoShou)
									{
										GoodVO goodsXmlNodeByID6 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
										int categoriy7 = goodsXmlNodeByID6.Categoriy;
										if (categoriy7 != 11 || categoriy7 != 12 || categoriy7 != 13 || categoriy7 != 16 || categoriy7 != 19)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
											handTypes = HandTypes.ZuoShou;
										}
									}
									else if (CompareType == HandTypes.YouShou)
									{
										GoodVO goodsXmlNodeByID7 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
										int categoriy8 = goodsXmlNodeByID7.Categoriy;
										int actionType4 = goodsXmlNodeByID7.ActionType;
										if (actionType4 == 2 || actionType4 == 7)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										}
										else if (categoriy8 != 11 || categoriy8 != 12 || categoriy8 != 13 || categoriy8 != 16 || categoriy8 != 19)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
											handTypes = HandTypes.YouShou;
										}
									}
									else if (CompareType == HandTypes.None)
									{
										dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										handTypes = HandTypes.YouShou;
									}
								}
								else if (goodsActionNameByID == 1)
								{
									if (goodsData != null)
									{
										int categoriy9 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).Categoriy;
										if (categoriy9 != 18)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
										}
									}
									if (goodsData2 != null)
									{
										dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
									}
								}
								else if (goodsActionNameByID == 2 || goodsActionNameByID == 5 || goodsActionNameByID == 7)
								{
									dictionary = Global.CalculatAttribute(goodsBasicAttribute2, goodsBasicAttribute3, "JIA");
									dictionary = Global.CalculatAttribute(goodsBasicAttribute, dictionary, "JIAN");
								}
							}
							else if (categoriyByGoodsID == 18)
							{
								if (goodsActionNameByID == 1)
								{
									if (goodsData != null)
									{
										dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
									}
									if (goodsData2 != null)
									{
										GoodVO goodsXmlNodeByID8 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
										int categoriy10 = goodsXmlNodeByID8.Categoriy;
										int handType5 = goodsXmlNodeByID8.HandType;
										int actionType5 = goodsXmlNodeByID8.ActionType;
										if (actionType5 == 2 || actionType5 == 7)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										}
										else if ((categoriy10 != 11 || categoriy10 != 12 || categoriy10 != 13 || categoriy10 != 16 || categoriy10 != 19) && categoriy10 != 17 && handType5 != 2)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										}
									}
								}
							}
							else if (categoriyByGoodsID != 19)
							{
								if (categoriyByGoodsID == 20)
								{
									if (goodsActionNameByID == 1)
									{
										if (goodsData != null)
										{
											int categoriy11 = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID).Categoriy;
											if (categoriy11 != 14)
											{
												dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
											}
										}
										if (goodsData2 != null)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										}
									}
								}
								else if (categoriyByGoodsID == 21 && goodsActionNameByID == 1)
								{
									if (goodsData != null)
									{
										dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute2, "JIAN");
									}
									if (goodsData2 != null)
									{
										GoodVO goodsXmlNodeByID9 = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
										int categoriy12 = goodsXmlNodeByID9.Categoriy;
										int actionType6 = goodsXmlNodeByID9.ActionType;
										if (actionType6 == 2 || actionType6 == 7)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										}
										else if (categoriy12 != 15)
										{
											dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute3, "JIAN");
										}
									}
								}
							}
						}
					}
				}
			}
			else if (categoriyByGoodsID == 6)
			{
				GoodsData goodsData3 = Super.FindUsingEuip(categoriyByGoodsID, 1);
				GoodsData goodsData4 = Super.FindUsingEuip(categoriyByGoodsID, 0);
				if (goodsData4 == null || goodsData3 == null)
				{
					dictionary = goodsBasicAttribute;
				}
				else if (CompareType == HandTypes.ZuoShou)
				{
					dictionary = Global.CalculatAttribute(goodsBasicAttribute, Global.GetGoodsBasicAttribute(goodsData3), "JIAN");
					handTypes = HandTypes.ZuoShou;
				}
				else if (CompareType == HandTypes.YouShou)
				{
					dictionary = Global.CalculatAttribute(goodsBasicAttribute, Global.GetGoodsBasicAttribute(goodsData4), "JIAN");
					handTypes = HandTypes.YouShou;
				}
				else if (CompareType == HandTypes.None)
				{
					dictionary = Global.CalculatAttribute(goodsBasicAttribute, Global.GetGoodsBasicAttribute(goodsData4), "JIAN");
					handTypes = HandTypes.YouShou;
				}
			}
			else if (categoriyByGoodsID == 10 || categoriyByGoodsID == 9)
			{
				GoodsData goodsData5 = Super.FindUsingEuip(9, -1);
				if (goodsData5 != null)
				{
					Dictionary<int, int> goodsBasicAttribute4 = Global.GetGoodsBasicAttribute(goodsData5);
					dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute4, "JIAN");
				}
			}
			else if (Global.IsRebornEquip(categoriyByGoodsID))
			{
				if (categoriyByGoodsID != 36)
				{
					GoodsData goodsData6 = Super.FindChongShengUsingEuip(categoriyByGoodsID, -1);
					if (goodsData6 != null)
					{
						Dictionary<int, int> goodsBasicAttribute5 = Global.GetGoodsBasicAttribute(goodsData6);
						dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute5, "JIAN");
					}
				}
				else
				{
					GoodsData goodsData7 = Super.FindChongShengUsingEuip(categoriyByGoodsID, 1);
					GoodsData goodsData8 = Super.FindChongShengUsingEuip(categoriyByGoodsID, 0);
					if (goodsData8 == null || goodsData7 == null)
					{
						dictionary = goodsBasicAttribute;
					}
					else if (CompareType == HandTypes.ZuoShou)
					{
						dictionary = Global.CalculatAttribute(goodsBasicAttribute, Global.GetGoodsBasicAttribute(goodsData7), "JIAN");
						handTypes = HandTypes.ZuoShou;
					}
					else if (CompareType == HandTypes.YouShou)
					{
						dictionary = Global.CalculatAttribute(goodsBasicAttribute, Global.GetGoodsBasicAttribute(goodsData8), "JIAN");
						handTypes = HandTypes.YouShou;
					}
					else if (CompareType == HandTypes.None)
					{
						dictionary = Global.CalculatAttribute(goodsBasicAttribute, Global.GetGoodsBasicAttribute(goodsData8), "JIAN");
						handTypes = HandTypes.YouShou;
					}
				}
			}
			else if (categoriyByGoodsID == 980)
			{
				GoodsData roleRidePetShenShenYinJiSlotUsingGoodsData = Global.GetRoleRidePetShenShenYinJiSlotUsingGoodsData(Global.Data.roleData.HolyGoodsDataList, IConfigbase<ConfigShengYinShengJi>.Instance.GetCaoWeiIndexByGoodsID(gd.GoodsID));
				if (roleRidePetShenShenYinJiSlotUsingGoodsData != null)
				{
					Dictionary<int, int> goodsBasicAttribute6 = Global.GetGoodsBasicAttribute(roleRidePetShenShenYinJiSlotUsingGoodsData);
					dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute6, "JIAN");
				}
			}
			else
			{
				GoodsData goodsData9 = Super.FindUsingEuip(categoriyByGoodsID, -1);
				if (goodsData9 != null)
				{
					Dictionary<int, int> goodsBasicAttribute7 = Global.GetGoodsBasicAttribute(goodsData9);
					dictionary = Global.CalculatAttribute(goodsBasicAttribute, goodsBasicAttribute7, "JIAN");
				}
			}
			dictionary[180] = (int)handTypes;
			return dictionary;
		}

		public static GoodsData GetRoleRidePetShenShenYinJiSlotUsingGoodsData(List<GoodsData> lst, int Slot)
		{
			if (lst != null)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					if (lst[i] != null)
					{
						if (lst[i].Using == 1 && Slot == lst[i].BagIndex)
						{
							return lst[i];
						}
					}
				}
			}
			return null;
		}

		private static Dictionary<int, int> CalculatAttribute(Dictionary<int, int> addDtc, Dictionary<int, int> beaddDtc, string type = "JIAN")
		{
			if (addDtc == null || beaddDtc == null)
			{
				return (addDtc != null) ? addDtc : beaddDtc;
			}
			if (addDtc.Count != 180 || beaddDtc.Count != 180)
			{
				return (addDtc.Count == 180) ? addDtc : beaddDtc;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if ("JIA" == type)
			{
				for (int i = 0; i < 180; i++)
				{
					if (i == 179)
					{
						dictionary[i] = (addDtc[i] ^ beaddDtc[i]);
					}
					else
					{
						dictionary[i] = addDtc[i] + beaddDtc[i];
					}
				}
			}
			else if ("JIAN" == type)
			{
				for (int j = 0; j < 180; j++)
				{
					if (j == 178)
					{
						dictionary[j] = ((addDtc[179] ^ beaddDtc[179]) & beaddDtc[179]);
					}
					else if (j == 179)
					{
						dictionary[j] = ((addDtc[j] ^ beaddDtc[j]) & addDtc[j]);
					}
					else
					{
						dictionary[j] = addDtc[j] - beaddDtc[j];
					}
				}
			}
			return dictionary;
		}

		public static List<string> GetPetAttribute(GoodsData goodData, int type)
		{
			List<string> list = new List<string>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				double[] equipProps = goodsXmlNodeByID.EquipProps;
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("PetQiangHuaProps", '|');
				for (int i = 1; i < 177; i++)
				{
					if (equipProps[i] != 0.0)
					{
						if (type == 1)
						{
							if (i != 3 && i != 5 && i != 7 && i != 9)
							{
								string text = string.Empty;
								if (!ConfigExtPropIndexes.GetPercentByID(i))
								{
									for (int j = 0; j < systemParamStringArrayByName.Length; j++)
									{
										string[] array = systemParamStringArrayByName[j].Split(new char[]
										{
											','
										});
										int num = Convert.ToInt32(array[0]);
										if (i == num)
										{
											double num2 = Convert.ToDouble(array[1]);
											text = (equipProps[i] * num2).ToString("0");
										}
									}
								}
								else
								{
									for (int k = 0; k < systemParamStringArrayByName.Length; k++)
									{
										string[] array2 = systemParamStringArrayByName[k].Split(new char[]
										{
											','
										});
										int num3 = Convert.ToInt32(array2[0]);
										if (i == num3)
										{
											double num2 = Convert.ToDouble(array2[1]);
											text = (equipProps[i] * num2 * 100.0).ToString("f1") + "%";
										}
									}
								}
								if (string.Empty != text)
								{
									list.Add(text);
								}
							}
						}
						else if (i != 4 && i != 6 && i != 8 && i != 10)
						{
							string text2 = string.Empty;
							if (!ConfigExtPropIndexes.GetPercentByID(i))
							{
								if (i == 3 || i == 5 || i == 7 || i == 9)
								{
									for (int l = 0; l < systemParamStringArrayByName.Length; l++)
									{
										string[] array3 = systemParamStringArrayByName[l].Split(new char[]
										{
											','
										});
										int num4 = Convert.ToInt32(array3[0]);
										if (i == num4)
										{
											double num5 = Convert.ToDouble(array3[1]);
											text2 = ExtPropIndexes.ExtPropIndexChineseNames[i] + ": " + (equipProps[i] * (1.0 + (double)goodData.Forge_level * num5)).ToString("0");
										}
									}
									for (int m = 0; m < systemParamStringArrayByName.Length; m++)
									{
										string[] array4 = systemParamStringArrayByName[m].Split(new char[]
										{
											','
										});
										int num6 = Convert.ToInt32(array4[0]);
										if (i + 1 == num6)
										{
											double num5 = Convert.ToDouble(array4[1]);
											text2 = text2 + " - " + (equipProps[i + 1] * (1.0 + (double)goodData.Forge_level * num5)).ToString("0");
										}
									}
								}
								else
								{
									for (int n = 0; n < systemParamStringArrayByName.Length; n++)
									{
										string[] array5 = systemParamStringArrayByName[n].Split(new char[]
										{
											','
										});
										int num7 = Convert.ToInt32(array5[0]);
										if (i == num7)
										{
											double num5 = Convert.ToDouble(array5[1]);
											text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false) + ": " + (equipProps[i] * (1.0 + (double)goodData.Forge_level * num5)).ToString("0");
										}
									}
								}
							}
							else
							{
								for (int num8 = 0; num8 < systemParamStringArrayByName.Length; num8++)
								{
									string[] array6 = systemParamStringArrayByName[num8].Split(new char[]
									{
										','
									});
									int num9 = Convert.ToInt32(array6[0]);
									if (i == num9)
									{
										double num5 = Convert.ToDouble(array6[1]);
										text2 = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByID(i, false) + ": " + (equipProps[i] * (1.0 + (double)goodData.Forge_level * num5) * 100.0).ToString("f1") + "%";
									}
								}
							}
							if (string.Empty != text2)
							{
								list.Add(text2);
							}
						}
					}
				}
			}
			return list;
		}

		public static List<string> GetZhuoYueAttribute(GoodsData goodData)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 32; i++)
			{
				if ((goodData.ExcellenceInfo & 1 << i) != 0)
				{
					string text = string.Empty;
					if (ZhuoyuePropIndexes.ZhuoyuePropIndexPercents[i] == 1)
					{
						text = string.Concat(new object[]
						{
							ZhuoyuePropIndexes.ZhuoyuePropIndexChineseNames[i],
							": +",
							ZhuoyuePropIndexes.ZhuoyuePropIndexValues[i],
							"%"
						});
					}
					else
					{
						text = ZhuoyuePropIndexes.ZhuoyuePropIndexChineseNames[i] + Global.GetLang(": +人物等级/") + ZhuoyuePropIndexes.ZhuoyuePropIndexValues[i];
					}
					list.Add(text);
				}
			}
			return list;
		}

		private static int GetAttributeAddBaseValue(GoodsData gd, int[] da, int extPropIndex)
		{
			double num = (double)da[extPropIndex];
			if (num <= 0.0)
			{
				return 0;
			}
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(gd.GoodsID);
			double num2;
			if (categoriyByGoodsID != 10 && categoriyByGoodsID != 9 && categoriyByGoodsID != 340)
			{
				int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(gd);
				num2 = ((zhuoyueAttributeCount <= 0) ? 0.0 : Global.GetZhuoYueAddAttackRates(gd));
			}
			else
			{
				num2 = 0.0;
			}
			double num3 = Global.GetEquipForgeAddBaseValue(gd, extPropIndex);
			double num4 = Global.GetEquipZhuijiaAddBaseValue(gd, extPropIndex);
			double equipZhuanshengAddBaseValue = Global.GetEquipZhuanshengAddBaseValue(gd, extPropIndex);
			if (categoriyByGoodsID == 7)
			{
				if (gd.Forge_level <= 0)
				{
					gd.Forge_level = 1;
					gd.AppendPropLev = 0;
				}
				double num5 = Global.SafeConvertToDouble(ConfigSystemParam.GetSystemParamByName("GoodWillXiShu", true));
				num3 = (double)((gd.Forge_level - 1) * 2);
				num4 = (double)gd.AppendPropLev * num5;
			}
			if (extPropIndex >= 3 && extPropIndex <= 6)
			{
				double num6 = 0.0;
				if (gd.Forge_level > 0)
				{
					num6 = (double)da[extPropIndex] * num3;
					num6 = ((num6 >= 3.0) ? num6 : 3.0);
				}
				num = (double)da[extPropIndex] * (1.0 + num2) * (1.0 + equipZhuanshengAddBaseValue);
				num += num6;
			}
			else if (extPropIndex >= 7 && extPropIndex <= 10)
			{
				double num7 = 0.0;
				if (gd.Forge_level > 0)
				{
					num7 = (double)da[extPropIndex] * num3;
					num7 = ((num7 >= 3.0) ? num7 : 3.0);
				}
				num = (double)da[extPropIndex] * (1.0 + num2) * (1.0 + num4 + equipZhuanshengAddBaseValue);
				num += num7;
			}
			else if (extPropIndex == 13)
			{
				num = (double)da[extPropIndex] * (1.0 + num3 + num4);
			}
			return (int)num;
		}

		public static int GetYAngle(Quaternion rotation)
		{
			int num = (int)rotation.eulerAngles.y;
			if (num < 0)
			{
				num = 360 - Math.Abs(num);
			}
			return num;
		}

		public static Quaternion GetRotaionByAngle(int angle)
		{
			if (angle > 180)
			{
				angle -= 360;
			}
			return Quaternion.Euler(new Vector3(0f, (float)angle, 0f));
		}

		public static Quaternion GetRotationByTwoPoint(Point orig, Point dest)
		{
			Vector3 vector;
			vector..ctor((float)orig.X / 100f, 0f, (float)orig.Y / 100f);
			Vector3 vector2;
			vector2..ctor((float)dest.X / 100f, 0f, (float)dest.Y / 100f);
			return Quaternion.LookRotation(vector2 - vector, Vector3.up);
		}

		public static Vector3 GetGroundPos(LeaderInfo leaderInfo, GSpriteTypes spriteType, float x, float z, float y = 0f)
		{
			if (!Global.initLayerMask)
			{
				Global.initLayerMask = true;
				Global.layerMask = 1 << LayerMask.NameToLayer("Terrain");
				Global.layerMask |= 1 << LayerMask.NameToLayer("Non-Barrier");
			}
			int num = Global.layerMask;
			if ((null != leaderInfo && leaderInfo.TriggerByCancel) || spriteType == GSpriteTypes.NPC)
			{
				num |= 1 << LayerMask.NameToLayer("Default");
			}
			Vector3 groundPosition = U3DUtils.GetGroundPosition3(x, z, y, num);
			if (groundPosition == Vector3.zero)
			{
				return groundPosition;
			}
			return groundPosition + new Vector3(0f, 0.05f, 0f);
		}

		public static string GetUTF8StringFromBytes(byte[] bytes)
		{
			int num = 0;
			while (num < bytes.Length && bytes[num] > 127)
			{
				num++;
			}
			return Encoding.UTF8.GetString(bytes, num, bytes.Length - num);
		}

		public static void setIOSNoBackup()
		{
		}

		public static bool SaveBytesToFile(string path, byte[] bytes)
		{
			try
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				int num = path.LastIndexOf('/');
				if (num != -1)
				{
					Directory.CreateDirectory(path.Substring(0, num));
				}
				using (FileStream fileStream = new FileStream(path, 2, 2))
				{
					fileStream.Write(bytes, 0, bytes.Length);
					fileStream.Flush();
					fileStream.Close();
					fileStream.Dispose();
				}
				return true;
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return false;
		}

		public static string ReadXmlConfigStr(XElement xml, string root, string attrribName)
		{
			if (xml == null)
			{
				return string.Empty;
			}
			XElement xelement;
			if (string.IsNullOrEmpty(root))
			{
				xelement = xml;
			}
			else
			{
				xelement = Global.GetXElement(xml, root);
			}
			if (xelement == null)
			{
				return string.Empty;
			}
			return Global.GetXElementAttributeStr(xelement, attrribName);
		}

		public static int ReadXmlConfigInt(XElement xml, string root, string attrribName)
		{
			if (xml == null)
			{
				return -1;
			}
			XElement xelement = Global.GetXElement(xml, root);
			if (xelement == null)
			{
				return -1;
			}
			return Global.GetXElementAttributeInt(xelement, attrribName);
		}

		public static long GetMingXiangExpr(out int xingHun)
		{
			xingHun = 0;
			if (!Global.IsLoadMingXiangExprInfo)
			{
				Global.IsLoadMingXiangExprInfo = true;
				Global.LoadMingXiangExpr();
			}
			long num = 0L;
			int i = 1;
			int num2 = Global.Data.MeditateInfoList.Count;
			while (i <= num2)
			{
				int num3 = (i + num2) / 2;
				MeditateData meditateData = Global.Data.MeditateInfoList[num3];
				int num4 = UIHelper.AvalidLevel(meditateData.MinLevel, meditateData.MaxLevel, meditateData.MinZhuanSheng, meditateData.MaxZhuanSheng);
				if (num4 == 0)
				{
					num = (long)meditateData.Experience;
					xingHun = meditateData.Xinghun;
					break;
				}
				if (num4 < 0)
				{
					num2 = num3 - 1;
				}
				else
				{
					i = num3 + 1;
				}
			}
			if (num < 0L)
			{
				num = 0L;
				xingHun = 0;
			}
			return num;
		}

		public static void LoadMingXiangExpr()
		{
			XElement gameResXml = Global.GetGameResXml("Config/MingXiang.xml");
			if (gameResXml == null)
			{
				return;
			}
			Global.Data.MeditateInfoList.Clear();
			foreach (XElement xelement in gameResXml.Elements())
			{
				if (xelement != null)
				{
					MeditateData meditateData = new MeditateData();
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
					meditateData.MeditateID = xelementAttributeInt;
					meditateData.MinZhuanSheng = Global.GetXElementAttributeInt(xelement, "MinZhuanSheng");
					meditateData.MaxZhuanSheng = Global.GetXElementAttributeInt(xelement, "MaxZhuanSheng");
					meditateData.MinLevel = Global.GetXElementAttributeInt(xelement, "MinLevel");
					meditateData.MaxLevel = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					meditateData.Experience = Global.GetXElementAttributeInt(xelement, "Experience");
					meditateData.Xinghun = Global.GetXElementAttributeInt(xelement, "Xinghun");
					Global.Data.MeditateInfoList.Add(xelementAttributeInt, meditateData);
				}
			}
		}

		public static string GetRandomCreatorRoleName(int sex)
		{
			string text = string.Empty;
			List<XElement> xelementList = Global.GetXElementList(Global.GetGameResXml("Config/name.Xml"), "Experience");
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < xelementList.Count; i++)
			{
				if (xelementList[i] != null)
				{
					if (string.Empty != Global.GetXElementAttributeStr(xelementList[i], "xing"))
					{
						list.Add(Global.GetXElementAttributeStr(xelementList[i], "xing"));
					}
					if (sex == 0)
					{
						if (Global.GetXElementAttributeStr(xelementList[i], "nan") != string.Empty)
						{
							list2.Add(Global.GetXElementAttributeStr(xelementList[i], "nan"));
						}
					}
					else if (Global.GetXElementAttributeStr(xelementList[i], "nv") != string.Empty)
					{
						list2.Add(Global.GetXElementAttributeStr(xelementList[i], "nv"));
					}
				}
			}
			if (list.Count > 0 && list2.Count > 0)
			{
				Random random = new Random();
				int num = random.Next(0, list2.Count);
				text = list2[num].Trim();
				text = text + Global.GetLang("·") + list[random.Next(0, list.Count)].Trim();
			}
			return text;
		}

		public static bool IsBloodCastleMap()
		{
			return Global.Data.roleData.MapCode >= 6000 && Global.Data.roleData.MapCode < 6090;
		}

		public static bool IsCalcTaoZhuangProps(GoodsData goodsData)
		{
			if (goodsData == null)
			{
				return false;
			}
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
			return categoriyByGoodsID != 9 && categoriyByGoodsID != 10 && categoriyByGoodsID != 22 && categoriyByGoodsID != 24;
		}

		public static int GetCurrentTaoZhuangIndex()
		{
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			List<int> list = new List<int>();
			if (usingGoodsDataList == null)
			{
				return -1;
			}
			if (usingGoodsDataList.Count <= 0)
			{
				return -1;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				if (Global.IsCalcTaoZhuangProps(keyValuePair.Value))
				{
					double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(keyValuePair.Value.GoodsID);
					if (keyValuePair.Value.Strong < (int)goodsEquipPropsDoubleList[0])
					{
						int num = (int)(goodsEquipPropsDoubleList[0] / (double)Global.MaxNotifyEquipStrongValue) - keyValuePair.Value.Strong / Global.MaxNotifyEquipStrongValue;
					}
					list.Add(keyValuePair.Value.Forge_level);
				}
			}
			int result = -1;
			if (list.Count >= 8)
			{
				list.Sort();
				int num2 = list[list.Count - 8];
				List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
				int count = taoZhuangList.Count;
				if (num2 < taoZhuangList[0].Level)
				{
					result = -1;
				}
				else if (num2 >= taoZhuangList[count - 1].Level)
				{
					result = count - 1;
				}
				else
				{
					for (int i = 0; i < count - 1; i++)
					{
						if (num2 >= taoZhuangList[i].Level && num2 < taoZhuangList[i + 1].Level)
						{
							result = i;
						}
					}
				}
			}
			return result;
		}

		public static string FormatBuffDesc(int buffID)
		{
			int num = 0;
			string text = null;
			double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(buffID);
			for (int i = 0; i < ExtPropIndexes.ExtPropIndexShows.Length; i++)
			{
				int num2 = ExtPropIndexes.ExtPropIndexShows[i];
				if (goodsEquipPropsDoubleList[num2] > 0.0)
				{
					if (num++ > 0)
					{
						text += ",";
					}
					string text2 = ExtPropIndexes.ExtPropIndexBuffNames[num2];
					if (num2 >= 3 && num2 <= 7)
					{
						text += string.Format("{0}{1}-{2}", text2, goodsEquipPropsDoubleList[num2], goodsEquipPropsDoubleList[num2 + 1]);
					}
					else
					{
						int num3 = ExtPropIndexes.ExtPropIndexPercents[num2];
						if (num3 > 0)
						{
							text += string.Format("{0}{1}%", text2, goodsEquipPropsDoubleList[num2] * 100.0);
						}
						else
						{
							text += string.Format("{0}{1}", text2, goodsEquipPropsDoubleList[num2]);
						}
					}
				}
			}
			return text;
		}

		public static int GetCurrentZhuoYueIndex()
		{
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			if (usingGoodsDataList == null)
			{
				return -1;
			}
			if (usingGoodsDataList.Count <= 0)
			{
				return -1;
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(keyValuePair.Value.GoodsID);
				if (keyValuePair.Value.Strong < (int)goodsEquipPropsDoubleList[0])
				{
					int num = (int)(goodsEquipPropsDoubleList[0] / (double)Global.MaxNotifyEquipStrongValue) - keyValuePair.Value.Strong / Global.MaxNotifyEquipStrongValue;
				}
				list.Add(Global.GetZhuoyueAttributeCount(keyValuePair.Value));
			}
			int result = -1;
			if (list.Count >= 8)
			{
				list.Sort();
				int num2 = list[list.Count - 8];
				if (num2 > 0 && num2 <= 2)
				{
					result = 0;
				}
				else if (num2 >= 3 && num2 <= 4)
				{
					result = 1;
				}
				else if (num2 >= 5)
				{
					result = 2;
				}
			}
			return result;
		}

		public static int GetIntSomeBit(int resource, int mask)
		{
			return resource >> mask & 1;
		}

		public static int SetIntSomeBit(int mask, int resource, bool flag)
		{
			if (flag)
			{
				resource |= 1 << mask;
			}
			else
			{
				resource &= ~(1 << mask);
			}
			return resource;
		}

		public static int GetBitValue(int whichOne)
		{
			return (int)Math.Pow(2.0, (double)(whichOne - 1));
		}

		public static int GetBitValue(List<int> values, int whichOne)
		{
			int num = whichOne / 32;
			int num2 = whichOne % 32;
			if (values.Count <= num)
			{
				return 0;
			}
			int num3 = values[num];
			if ((num3 & 1 << num2) != 0)
			{
				return 1;
			}
			return 0;
		}

		public static void SetBitValue(ref List<int> values, int whichOne, int toValue)
		{
			int num = whichOne / 32;
			int num2 = whichOne % 32;
			while (values.Count <= num)
			{
				values.Add(0);
			}
			int num3 = values[num];
			if (toValue == 0)
			{
				num3 &= ~(1 << num2);
			}
			else
			{
				num3 |= 1 << num2;
			}
			values[num] = num3;
		}

		public static string GetMobaiNumber()
		{
			return ConfigSystemParam.GetSystemParamByName("MoBaiNumber", true);
		}

		public static string GetMobaiJinbiXiaohao()
		{
			return Global.GetMobaiParamsAt("JiBiMoBai", 0);
		}

		public static string GetMobaiJinbiJiangliJingyan()
		{
			return Global.GetMobaiParamsAt("JiBiMoBai", 1);
		}

		public static string GetMobaiJinbiJiangliShengwang()
		{
			return Global.GetMobaiParamsAt("JiBiMoBai", 2);
		}

		public static string GetMobaiZuanshiXiaohao()
		{
			return Global.GetMobaiParamsAt("ZuanShiMoBai", 0);
		}

		public static string GetMobaiZuanshiJiangliJingyan()
		{
			return Global.GetMobaiParamsAt("ZuanShiMoBai", 1);
		}

		public static string GetMobaiZuanshiJiangliShengwang()
		{
			return Global.GetMobaiParamsAt("ZuanShiMoBai", 2);
		}

		public static string GetMobaiParamsAt(string paramName, int idx)
		{
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName(paramName, ',');
			if (idx < systemParamStringArrayByName.Length)
			{
				return systemParamStringArrayByName[idx];
			}
			return null;
		}

		public static string GetMobaiExteraVIPNumber(int vipLevel = 0)
		{
			return Global.GetMobaiParamsAt("VIPMoBaiNum", vipLevel);
		}

		public static string GetMobaiPKKingNumber()
		{
			return ConfigSystemParam.GetSystemParamByName("PkKingMoBaiNum", true);
		}

		public static int GetMaxShaodangNum()
		{
			if (Global.MaxShaodangNum == 0)
			{
				Global.MaxShaodangNum = (int)ConfigSystemParam.GetSystemParamIntByName("WanMoTaSaoDang");
			}
			return Global.MaxShaodangNum;
		}

		public static string[] GetPataIndexRange(int index)
		{
			if (Global.PataIndexRangeBySets == null)
			{
				Global.PataIndexRangeBySets = ConfigSystemParam.GetSystemParamStringArrayByName("WanMoTaFenZu", '|');
			}
			if (index > Global.PataIndexRangeBySets.Length)
			{
				index = Global.PataIndexRangeBySets.Length;
			}
			return Global.PataIndexRangeBySets[Math.Max(index - 1, 0)].Split(new char[]
			{
				','
			});
		}

		public static int[] GetPataIndexRange()
		{
			if (Global.PataIndexRange == null)
			{
				Global.PataIndexRange = new int[2];
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("WanMoTaFenZu", '|');
				if (systemParamStringArrayByName != null)
				{
					Global.PataIndexRange[0] = systemParamStringArrayByName[0].Split(new char[]
					{
						','
					})[1].SafeToInt32(0);
					Global.PataIndexRange[1] = systemParamStringArrayByName[systemParamStringArrayByName.Length - 1].Split(new char[]
					{
						','
					})[2].SafeToInt32(0);
				}
			}
			return Global.PataIndexRange;
		}

		public static int GetPataMaxIndex()
		{
			if (Global.MaxPataIndex <= 0)
			{
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("WanMoTaFenZu", '|');
				if (systemParamStringArrayByName != null)
				{
					Global.MaxPataIndex = systemParamStringArrayByName[systemParamStringArrayByName.Length - 1].Split(new char[]
					{
						','
					})[2].SafeToInt32(0) - systemParamStringArrayByName[0].Split(new char[]
					{
						','
					})[1].SafeToInt32(0) + 1;
				}
			}
			return Global.MaxPataIndex;
		}

		public static string GetStrByTwoSplitChar(string str, char startChar, char endChar)
		{
			string empty = string.Empty;
			if (string.IsNullOrEmpty(str))
			{
				return empty;
			}
			int num = str.IndexOf(startChar);
			int num2 = str.IndexOf(endChar);
			if (num < 0 || num2 <= 0 || num2 - num < 1 || num + 1 >= str.Length)
			{
				return empty;
			}
			return str.Substring(num + 1, num2 - num - 1);
		}

		public static int GetTiJiaoTuJianNum(int nID)
		{
			int result = 0;
			if (Global.Data.roleData.PictureJudgeReferInfo != null && Global.Data.roleData.PictureJudgeReferInfo.TryGetValue(nID, ref result))
			{
				return result;
			}
			return result;
		}

		public static Dictionary<int, int> GetComparedTujianDict(Dictionary<int, int> newDict, Dictionary<int, int> oldDict)
		{
			if (oldDict == null)
			{
				return newDict;
			}
			Dictionary<int, int>.Enumerator enumerator = newDict.GetEnumerator();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				int key = keyValuePair.Key;
				if (!oldDict.ContainsKey(key))
				{
					dictionary.Add(key, newDict[key]);
				}
				else
				{
					int num = newDict[key] - oldDict[key];
					if (num > 0)
					{
						dictionary.Add(key, num);
					}
				}
			}
			return dictionary;
		}

		public static bool IsAllowCache
		{
			get
			{
				return FPSCounter.CountMemory > 900 && FPSCounter.ResidualMemory > 150;
			}
		}

		public static bool IsMemoryProtection
		{
			get
			{
				return FPSCounter.ResidualMemory <= 50;
			}
		}

		public static void ClearOptimizationCache()
		{
			MuAssetManager.Instance.UnLoadAllUnuseResource();
		}

		public static void ExecutionMemoryProtection()
		{
			try
			{
				Global.MemoryProtectionTotalElapsedTime += Time.deltaTime;
				if (Global.MemoryProtectionTotalElapsedTime >= 30f)
				{
					Global.MemoryProtectionTotalElapsedTime = 0f;
					FPSCounter.MemorySync();
					if (Global.IsMemoryProtection)
					{
						MuAssetManager.Instance.UnLoadAllUnuseResource();
						Resources.UnloadUnusedAssets();
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static bool IsShowOtherRole
		{
			get
			{
				return Global.isShowOtherRole;
			}
		}

		public static bool IsShowFakeRole
		{
			get
			{
				return Global.isShowFakeRole;
			}
		}

		public static void SetNearRoleShow(GSprite Leader)
		{
			if (Leader == null || Global.Data.SysSetting.HideOtherRoles || Leader.SpriteType != GSpriteTypes.Leader)
			{
				return;
			}
			if (!Global.IsInSafeRegion)
			{
				Global.isShowOtherRole = true;
				return;
			}
			Global.TotalElapsedTime += Time.deltaTime;
			if (Global.TotalElapsedTime >= 1f)
			{
				Global.TotalElapsedTime = 0f;
				if (Time.deltaTime >= 0.1f)
				{
					if (++Global.LowFPS >= 3)
					{
						Global.UpFPS = 0;
						Global.MiddleFPS = 0;
					}
					else
					{
						Global.UpFPS = ((Global.UpFPS >= 10) ? Global.UpFPS : 0);
						Global.MiddleFPS = ((Global.MiddleFPS >= 10) ? Global.MiddleFPS : 0);
					}
				}
				else if (Time.deltaTime < 0.033f)
				{
					if (++Global.UpFPS >= 10)
					{
						Global.LowFPS = 0;
						Global.MiddleFPS = 0;
					}
					else
					{
						Global.LowFPS = ((Global.LowFPS >= 3) ? Global.LowFPS : 0);
						Global.MiddleFPS = ((Global.MiddleFPS >= 10) ? Global.MiddleFPS : 0);
					}
				}
				else if (Time.deltaTime >= 0.033f && Time.deltaTime <= 0.04f)
				{
					if (++Global.MiddleFPS >= 10)
					{
						Global.UpFPS = 0;
						Global.LowFPS = 0;
					}
					else
					{
						Global.UpFPS = ((Global.UpFPS >= 10) ? Global.UpFPS : 0);
						Global.LowFPS = ((Global.LowFPS >= 3) ? Global.LowFPS : 0);
					}
				}
				if (Global.LowFPS < 3 && Global.UpFPS < 10 && Global.MiddleFPS < 10)
				{
					return;
				}
				if (Global.UpFPS >= 10)
				{
					Global.isShowOtherRole = true;
				}
				else
				{
					Global.isShowOtherRole = false;
				}
				int num = 0;
				int num2 = 0;
				if (Global.Data.OtherRoles != null && Global.IsInSafeRegion)
				{
					foreach (KeyValuePair<int, RoleData> keyValuePair in Global.Data.OtherRoles)
					{
						RoleData value = keyValuePair.Value;
						if (Global.Data.roleData.TeamID <= 0 || Global.Data.roleData.TeamID != value.TeamID)
						{
							GSprite gsprite = ObjectsManager.FindSprite(value.RoleID);
							if (gsprite != null && !(gsprite.The3DGameObject == null) && gsprite.The3DGameObject.name.StartsWith("Role") && !(Leader.The3DGameObject == null))
							{
								if (!Global.isShowOtherRole)
								{
									if (!(null == gsprite.Trans) && !(null == Leader.Trans))
									{
										float num3 = Vector3.Distance(gsprite.Trans.localPosition, Leader.Trans.localPosition);
										if (num3 > ((Global.MiddleFPS < 10) ? 4f : 8f))
										{
											gsprite.HideObject();
										}
										else if ((num < 5 && Global.LowFPS >= 3 && num3 < 4f) || (num < 20 && Global.MiddleFPS >= 10 && num3 < 8f))
										{
											Global.isShowOtherRole = true;
											gsprite.ShowObject();
											Global.isShowOtherRole = false;
											num++;
										}
										else
										{
											gsprite.HideObject();
										}
									}
								}
								else
								{
									num++;
									gsprite.ShowObject();
								}
							}
						}
					}
				}
				if (Global.Data.FakeRoles != null)
				{
					List<FakeRoleData> list = new List<FakeRoleData>();
					List<FakeRoleData> list2 = new List<FakeRoleData>();
					foreach (KeyValuePair<int, FakeRoleData> keyValuePair2 in Global.Data.FakeRoles)
					{
						FakeRoleData value2 = keyValuePair2.Value;
						if (value2.FakeRoleType == 1)
						{
							list.Add(value2);
						}
						else if (value2.FakeRoleType == 2)
						{
							list2.Add(value2);
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						Global.ShowOrHideFakeRole(list[i], ref num2, Leader);
					}
					for (int j = 0; j < list2.Count; j++)
					{
						Global.ShowOrHideFakeRole(list2[j], ref num2, Leader);
					}
				}
			}
		}

		private static void ShowOrHideFakeRole(FakeRoleData fakeRD, ref int currentFakeCount, GSprite Leader)
		{
			GSprite gsprite = Global.Data.GameScene.FindSprite(Global.FormatRoleID(fakeRD.FakeRoleID));
			if (gsprite != null && gsprite.The3DGameObject != null && gsprite.The3DGameObject.name.StartsWith("Role") && Leader.The3DGameObject != null)
			{
				if (null == gsprite.Trans || null == Leader.Trans)
				{
					return;
				}
				float num = Vector3.Distance(gsprite.Trans.localPosition, Leader.Trans.localPosition);
				if (Global.UpFPS >= 10)
				{
					if (num < 8f && currentFakeCount < 20)
					{
						Global.isShowFakeRole = true;
						gsprite.ShowObject();
						Global.isShowFakeRole = false;
						currentFakeCount++;
					}
					else
					{
						gsprite.HideObject();
					}
				}
				else if (num > ((Global.MiddleFPS < 10) ? 0f : 6f))
				{
					gsprite.HideObject();
				}
				else if ((currentFakeCount < 0 && Global.LowFPS >= 3 && num < 3f) || (currentFakeCount < 10 && Global.MiddleFPS >= 10 && num < 6f))
				{
					Global.isShowFakeRole = true;
					gsprite.ShowObject();
					Global.isShowFakeRole = false;
					currentFakeCount++;
				}
				else
				{
					gsprite.HideObject();
				}
			}
		}

		public static bool IsInGameScene
		{
			set
			{
				if (value == Global.isInGameScene)
				{
					return;
				}
				if (Global.originHeight == 0)
				{
					Global.originHeight = Screen.height;
					Global.ratio = (float)Screen.width / (float)Screen.height;
					int num = 1;
					string systemParamByName = ConfigSystemParam.GetSystemParamByName("ScreenResClamp", true);
					if (!string.IsNullOrEmpty(systemParamByName))
					{
						int.TryParse(systemParamByName, ref num);
					}
					Global.EnabledHalfRes = (num == 1 && Global.originHeight > 720);
				}
				if (!Global.EnabledHalfRes)
				{
					return;
				}
				Global.isInGameScene = value;
				Global.ResetRes();
			}
		}

		private static bool HalfRes
		{
			set
			{
				if (Global.halfRes == value)
				{
					return;
				}
				if (Global.originHeight > 1080)
				{
					Global.originHeight = 1080;
				}
				Global.halfRes = value;
				if (Global.halfRes)
				{
					int num = Mathf.Max(720, Mathf.RoundToInt((float)Global.originHeight * 0.7f));
					int num2 = Mathf.RoundToInt((float)num * Global.ratio);
					Screen.SetResolution(num2, num, true);
				}
				else
				{
					int num3 = Mathf.RoundToInt((float)Global.originHeight * Global.ratio);
					Screen.SetResolution(num3, Global.originHeight, true);
				}
			}
		}

		public static void UpdateFrameRate()
		{
			if (!Global.isInGameScene)
			{
				return;
			}
			Global.timeFor150Frame = Global.timeFor150Frame * 0.99333f + Time.deltaTime;
			if (!Global.halfRes || Global.timeFor150Frame >= 4.2857f)
			{
				if (!Global.halfRes && Global.timeFor150Frame > 7.5f)
				{
					MySettingLockScreenMessageBoxPart.ShowSettingScreenSizeTip();
				}
			}
		}

		private static void ResetRes()
		{
			Global.timeFor150Frame = 5f;
			if (Global.Data.SysSetting.ScreenLockSize)
			{
				Global.HalfRes = true;
			}
			else
			{
				Global.HalfRes = false;
			}
		}

		public static void ApplyScreenSizeSetting()
		{
			Global.timeFor150Frame = 5f;
			if (Global.Data.SysSetting.ScreenLockSize)
			{
				Global.HalfRes = true;
			}
			else
			{
				Global.HalfRes = false;
			}
		}

		public static void SendEvent(string strEventID, string strEventName)
		{
			Analytics.Event(strEventID, strEventName);
		}

		public static bool IsEnableCaiji()
		{
			if (Global.Data.roleData.MapCode == 12000)
			{
				if (TaskBoxMini.ShuiJingHuanJingNum <= 0)
				{
					Super.HintMainText(Global.GetLang("今日已无剩余采集次数"), 10, 3);
					return false;
				}
			}
			else if (Global.GetMapSceneUIClass() == SceneUIClasses.LingDiCaiJi && TaskBoxMini.ArmyCaiJiNum <= 0)
			{
				Super.HintMainText(Global.GetLang("本周已无剩余采集次数"), 10, 3);
				return false;
			}
			return true;
		}

		public static int GetFashionGoodsID(int fashionID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Fashion.xml");
			if (gameResXml == null)
			{
				return 0;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
			if (xelementList == null || xelementList.Count <= 0)
			{
				return 0;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (fashionID == xelementAttributeInt)
				{
					return Global.GetXElementAttributeInt(xelement, "Goods");
				}
			}
			return 0;
		}

		public static int GetDressTabIDByGoodsID(int goodsID)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Fashion.xml");
			if (gameResXml == null)
			{
				return -1;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
			if (xelementList == null || xelementList.Count <= 0)
			{
				return -1;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Goods");
				if (xelementAttributeInt == goodsID)
				{
					return Global.GetXElementAttributeInt(xelement, "Tab");
				}
			}
			return -1;
		}

		public static int GetLuolanFashionWingID(int id)
		{
			int result = 0;
			XElement gameResXml = Global.GetGameResXml("Config/Fashion.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fashion");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				if (Global.GetXElementAttributeInt(xelement, "Type") == 1 && Global.GetXElementAttributeInt(xelement, "ID") == id)
				{
					result = Global.GetXElementAttributeInt(xelement, "Goods");
					break;
				}
			}
			return result;
		}

		public static int GetLuolanchengzhanClearSecs()
		{
			XElement gameResXml = Global.GetGameResXml("Config/SiegeWarfare.Xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
			return Global.GetXElementAttributeInt(xelementList[0], "ClearRolesSecs");
		}

		public static int GetBufferDecID(int bufferID)
		{
			int result = -1;
			switch (bufferID)
			{
			case 2080001:
				result = 15002;
				break;
			case 2080002:
			{
				string path = "LegionsEastFlag.xml";
				result = Global.GetLegionsBufferCode(path, 1);
				break;
			}
			default:
				if (bufferID == 101)
				{
					result = 10212;
				}
				break;
			case 2080007:
				result = 15003;
				break;
			case 2080008:
				result = 15004;
				break;
			case 2080009:
				result = 15005;
				break;
			case 2080010:
			{
				string path2 = "CoupleBuff.xml";
				result = Global.GetBufferCode(path2, 1);
				break;
			}
			case 2080011:
			{
				string path3 = "CoupleBuff.xml";
				result = Global.GetBufferCode(path3, 2);
				break;
			}
			}
			return result;
		}

		private static int GetBufferCode(string path, int typeId)
		{
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/{0}", path));
			if (gameResXml == null)
			{
				return -1;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "CoupleBuff");
			for (int i = 0; i < xelementList.Count; i++)
			{
				if (typeId == Global.GetXElementAttributeInt(xelementList[i], "TypeID"))
				{
					return Global.GetXElementAttributeInt(xelementList[i], "Decorations");
				}
			}
			return -1;
		}

		private static int GetLegionsBufferCode(string path, int typeId)
		{
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/{0}", path));
			if (gameResXml == null)
			{
				return -1;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "LegionsEastFlag");
			for (int i = 0; i < xelementList.Count; i++)
			{
				if (typeId == Global.GetXElementAttributeInt(xelementList[i], "ID"))
				{
					return Global.GetXElementAttributeInt(xelementList[i], "Decorations");
				}
			}
			return -1;
		}

		public static string ChangeChatboxRoleName(string roleName, int ZoneID)
		{
			if ((Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu) && !roleName.Contains(Global.GetLang("区]")))
			{
				return Global.FormatRoleNameZoneid(ZoneID, roleName, 0, 0);
			}
			return roleName;
		}

		public static bool IsYueKaOpen()
		{
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("YueKa_Andrid");
			return num == 1;
		}

		public static bool IsTuiGuangOpen(bool isTopIndexBox = true)
		{
			if (!isTopIndexBox)
			{
				return false;
			}
			int num = (int)ConfigSystemParam.GetSystemParamIntByName("TuiGuang_Android");
			return num == 1;
		}

		public static void HandleSecondPasswordErrorCode(int errorCode)
		{
			if (errorCode == 7)
			{
				if (Global.HasSecondPassword == 1 && Global.IsSetSecondPassword)
				{
					Global.IsSetSecondPassword = false;
					(Super.GData.PlayZoneRoot as PlayZone).CloseSetSecondPasswordWindow();
					Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码设置成功"), -1, -1, -1, -1, false);
				}
				else if (Global.HasSecondPassword == 1 && !Global.IsSetSecondPassword)
				{
					(Super.GData.PlayZoneRoot as PlayZone).CloseChangeSecondPasswordWindow();
					Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码修改成功"), -1, -1, -1, -1, false);
				}
			}
			else if (errorCode == 4)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("新二级密码为空"), -1, -1, -1, -1, false);
			}
			else if (errorCode == 3)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码只能由英文、数字、下划线组成"), -1, -1, -1, -1, false);
			}
			else if (errorCode == 2)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("您未设置二级密码"), -1, -1, -1, -1, false);
			}
			else if (errorCode == 0)
			{
				if (Global.VerifySuccess != null)
				{
					Global.VerifySuccess.Invoke();
				}
				Super.CloseVerifySecondPasswordWindow();
			}
			else if (errorCode == 1)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码输入错误"), -1, -1, -1, -1, false);
			}
			else if (errorCode == 6 || errorCode == 5)
			{
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码需要6-8个字符"), -1, -1, -1, -1, false);
			}
			else if (errorCode == 8)
			{
				Super.ShowMessageBox(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("服务器内部错误"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
			}
			else if (errorCode == 9)
			{
				Global.HasSecondPassword = 0;
				(Super.GData.PlayZoneRoot as PlayZone).CloseCancelSecondPasswordWindow();
				Super.ShowMessageBoxEx(Super.MainWindowRoot, 0, Global.GetLang("提示"), Global.GetLang("二级密码取消"), -1, -1, -1, -1, false);
			}
		}

		public static string GetInviteAwardGoodsInfo(string id)
		{
			XElement gameResXml = Global.GetGameResXml("Config/TenAward.xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Award", "ID", id);
			if (xelement == null)
			{
				return null;
			}
			return Global.GetXElementAttributeStr(xelement, "AwardGoods");
		}

		public static string RoleIDToCode(int roleID)
		{
			char[] array = Enumerable.ToArray<char>(roleID.ToString());
			for (int i = 0; i < array.Length; i += 2)
			{
				char[] array2 = array;
				int num = i;
				array2[num] += 'A';
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < array.Length; j++)
			{
				stringBuilder.Append(array[j].ToString());
			}
			return stringBuilder.ToString().ToUpper();
		}

		public static int CodeToRoleID(string code)
		{
			char[] array = Enumerable.ToArray<char>(code.ToLower());
			for (int i = 0; i < array.Length; i += 2)
			{
				char[] array2 = array;
				int num = i;
				array2[num] -= 'A';
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < array.Length; j++)
			{
				stringBuilder.Append(array[j].ToString());
			}
			int result = 0;
			int.TryParse(stringBuilder.ToString(), ref result);
			return result;
		}

		public static int getSkillAddPoin(int id)
		{
			int num = 0;
			if (Global.Data.roleData.MyTalentData != null)
			{
				if (Global.Data.roleData.MyTalentData.SkillOneValue.ContainsKey(id))
				{
					num = Global.Data.roleData.MyTalentData.SkillOneValue[id];
				}
				return num + Global.Data.roleData.MyTalentData.SkillAllValue;
			}
			return num;
		}

		public static int GetBujianID(int nType, int nSlot, int nSuit)
		{
			return nType * 1000 + (nSlot - 1) * 100 + nSuit;
		}

		public static int GetShengwuID(int nSuit, int nType)
		{
			return nType * 100 + nSuit;
		}

		public static void GetHolyPartConfig()
		{
			if (Global.dic_holyPartAttr.Keys.Count > 0)
			{
				return;
			}
			XElement gameResXml = Global.GetGameResXml("Config/BuJian.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuJian");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				HolyPartAttribute holyPartAttribute = new HolyPartAttribute();
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Name");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Picture");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "Color");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Quality");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Type");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "SuitID");
				string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "Property");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "CostBandJinBi");
				string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement, "FailCost");
				string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement, "NeedItem");
				string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement, "FailureConsumption");
				float xelementAttributeFloat = Global.GetXElementAttributeFloat(xelement, "SuccessProbability");
				holyPartAttribute.id = xelementAttributeInt;
				holyPartAttribute.name = ((!string.IsNullOrEmpty(xelementAttributeStr)) ? xelementAttributeStr : string.Empty);
				holyPartAttribute.Picture = ((!string.IsNullOrEmpty(xelementAttributeStr2)) ? xelementAttributeStr2 : string.Empty);
				holyPartAttribute.Color = ((!string.IsNullOrEmpty(xelementAttributeStr3)) ? xelementAttributeStr3 : string.Empty);
				holyPartAttribute.Quality = xelementAttributeInt2;
				holyPartAttribute.Type = xelementAttributeInt3;
				holyPartAttribute.SuitID = xelementAttributeInt4;
				holyPartAttribute.Property = ((!string.IsNullOrEmpty(xelementAttributeStr4)) ? xelementAttributeStr4 : string.Empty);
				holyPartAttribute.CostBandJianBi = xelementAttributeInt5;
				holyPartAttribute.NeedGoods = ((!string.IsNullOrEmpty(xelementAttributeStr5)) ? xelementAttributeStr5 : string.Empty);
				holyPartAttribute.FailCost = ((!string.IsNullOrEmpty(xelementAttributeStr6)) ? xelementAttributeStr6 : string.Empty);
				holyPartAttribute.NeedItem = ((!string.IsNullOrEmpty(xelementAttributeStr5)) ? xelementAttributeStr7 : string.Empty);
				holyPartAttribute.FailureConsumption = ((!string.IsNullOrEmpty(xelementAttributeStr8)) ? xelementAttributeStr8 : string.Empty);
				holyPartAttribute.SuccessProbability = xelementAttributeFloat;
				if (!Global.dic_holyPartAttr.ContainsKey(xelementAttributeInt))
				{
					Global.dic_holyPartAttr.Add(xelementAttributeInt, holyPartAttribute);
				}
			}
		}

		public static HolyPartAttribute GetHolyPartByID(int id)
		{
			if (Global.dic_holyPartAttr == null || Global.dic_holyPartAttr.Keys.Count <= 0)
			{
				return null;
			}
			return Global.dic_holyPartAttr[id];
		}

		public static Dictionary<int, DiamondAttribute> GetDiamondsConfig()
		{
			if (Global._dic_diamond != null && Global._dic_diamond.Count > 0)
			{
				return Global._dic_diamond;
			}
			XElement gameResXml = Global.GetGameResXml("Config/GemLevelup.xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Levelup");
			if (xelementList == null || xelementList.Count <= 0)
			{
				return null;
			}
			Global._dic_diamond = new Dictionary<int, DiamondAttribute>(xelementList.Count);
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				DiamondAttribute diamondAttribute = new DiamondAttribute();
				diamondAttribute.goodsID = Global.GetXElementAttributeInt(xelement, "GoodsID");
				diamondAttribute.name = Global.GetXElementAttributeStr(xelement, "Name");
				diamondAttribute.type = Global.GetXElementAttributeInt(xelement, "ElementsTypeID");
				diamondAttribute.shapeType = Global.GetXElementAttributeInt(xelement, "GemTypeID");
				diamondAttribute.level = Global.GetXElementAttributeInt(xelement, "Level");
				diamondAttribute.previousGoodsID = Global.GetXElementAttributeInt(xelement, "OldGoodsID");
				diamondAttribute.nextLevelGoodsID = Global.GetXElementAttributeInt(xelement, "NewGoodsID");
				diamondAttribute.levelupNeedsNum = Global.GetXElementAttributeInt(xelement, "NeedOldGoodsNum");
				diamondAttribute.needLevelOneNum = Global.GetXElementAttributeInt(xelement, "NeedOneLevelNum");
				diamondAttribute.needCoins = Global.GetXElementAttributeInt(xelement, "CostBandJinBi");
				if (!Global._dic_diamond.ContainsKey(diamondAttribute.goodsID))
				{
					Global._dic_diamond.Add(diamondAttribute.goodsID, diamondAttribute);
				}
			}
			return Global._dic_diamond;
		}

		public static DiamondAttribute GetDiamondAttributeByGoodsID(int goodsID, out int shapeType)
		{
			shapeType = -1;
			if (Global._dic_diamond == null)
			{
				Global.GetDiamondsConfig();
			}
			DiamondAttribute value = Global._dic_diamond.GetValue(goodsID);
			if (value != null)
			{
				shapeType = value.shapeType;
			}
			return value;
		}

		public static DiamondAttribute GetNextLevelDiamondData(int goodsID, out GoodsData gd, SaleGoodsConsts newGoodsSite)
		{
			int num = -1;
			gd = null;
			if (goodsID <= 0)
			{
				return null;
			}
			DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(goodsID, out num);
			if (diamondAttributeByGoodsID != null)
			{
				int nextLevelGoodsID = diamondAttributeByGoodsID.nextLevelGoodsID;
				gd = Global.GetDummyGoodsData(nextLevelGoodsID);
				if (gd != null)
				{
					gd.Site = (int)newGoodsSite;
				}
			}
			return diamondAttributeByGoodsID;
		}

		public static int GetNextLevelDiamondGoodsID(int goodsID)
		{
			int num = -1;
			if (goodsID <= 0)
			{
				return -1;
			}
			DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(goodsID, out num);
			if (diamondAttributeByGoodsID != null)
			{
				return diamondAttributeByGoodsID.nextLevelGoodsID;
			}
			return -1;
		}

		public static DiamondAttribute GetLevelOneDiamondByType(int type, int shapeType, out int goodsID)
		{
			goodsID = -1;
			if (shapeType < 1 || shapeType > 3)
			{
				return null;
			}
			if (Global._dic_diamond == null)
			{
				Global.GetDiamondsConfig();
			}
			foreach (KeyValuePair<int, DiamondAttribute> keyValuePair in Global._dic_diamond)
			{
				DiamondAttribute value = keyValuePair.Value;
				if (value.level == 1 && value.type == type && value.shapeType == shapeType)
				{
					goodsID = value.goodsID;
					return value;
				}
			}
			return null;
		}

		public static GoodsData GetDiamondGoodsDataByDBID(int dbID)
		{
			Dictionary<int, GoodsData> bagDiamondList = Global.GetBagDiamondList();
			if (bagDiamondList == null)
			{
				return null;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in bagDiamondList)
			{
				GoodsData value = keyValuePair.Value;
				if (value != null && value.Id == dbID)
				{
					return value;
				}
			}
			return null;
		}

		public static int GetDiamondLevelByGoodsID(int goodsID)
		{
			int result = 1;
			if (Global._dic_diamond == null)
			{
				Global.GetDiamondsConfig();
			}
			DiamondAttribute value = Global._dic_diamond.GetValue(goodsID);
			if (value != null)
			{
				result = value.level;
			}
			return result;
		}

		public static bool IsGoodsMatchShape(int goodsID, int shapeID)
		{
			if (goodsID < 0 || shapeID < 1 || shapeID > 3)
			{
				return false;
			}
			int num = -1;
			DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(goodsID, out num);
			return diamondAttributeByGoodsID != null && num == shapeID;
		}

		public static bool IsGoodsMatchType(int goodsID, int type, int shapeID)
		{
			if (goodsID < 0 || shapeID < 1 || shapeID > 3)
			{
				return false;
			}
			int num = -1;
			DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(goodsID, out num);
			return diamondAttributeByGoodsID != null && diamondAttributeByGoodsID.type == type && num == shapeID;
		}

		public static int GetFluorescentDiamondTotalLevel()
		{
			Dictionary<int, Dictionary<int, GoodsData>> equipedDiamondList = Global.GetEquipedDiamondList();
			if (equipedDiamondList == null || equipedDiamondList.Count <= 0)
			{
				return 0;
			}
			int num = 0;
			foreach (KeyValuePair<int, Dictionary<int, GoodsData>> keyValuePair in equipedDiamondList)
			{
				Dictionary<int, GoodsData> value = keyValuePair.Value;
				if (value != null && value.Count > 0)
				{
					foreach (KeyValuePair<int, GoodsData> keyValuePair2 in value)
					{
						GoodsData value2 = keyValuePair2.Value;
						if (value2 != null)
						{
							num += Global.GetDiamondLevelByGoodsID(value2.GoodsID);
						}
					}
				}
			}
			return num;
		}

		public static void OnEquipedDiamondDataChanged(int slotID, int shapeID, GoodsData gd, EquipedDiamondModifyType type)
		{
			if (slotID < 1 || slotID > 10)
			{
				return;
			}
			Dictionary<int, Dictionary<int, GoodsData>> dictionary = Global.GetEquipedDiamondList();
			if (dictionary == null)
			{
				dictionary = new Dictionary<int, Dictionary<int, GoodsData>>();
			}
			Dictionary<int, GoodsData> dictionary2 = dictionary.GetValue(slotID);
			if (dictionary2 == null)
			{
				dictionary2 = new Dictionary<int, GoodsData>();
			}
			GoodsData value = dictionary2.GetValue(shapeID);
			switch (type)
			{
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Add:
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Replace:
				if (value == null)
				{
					dictionary2.Add(shapeID, gd);
				}
				else
				{
					dictionary2[shapeID] = gd;
				}
				break;
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Remove:
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Destroy:
				if (value != null)
				{
					dictionary2.Remove(shapeID);
				}
				break;
			}
			if (dictionary.GetValue(slotID) == null)
			{
				dictionary.Add(slotID, dictionary2);
			}
			else
			{
				dictionary[slotID] = dictionary2;
			}
			Global.SetEquipedDiamondList(dictionary);
		}

		public static Dictionary<int, Dictionary<int, GoodsData>> GetEquipedDiamondList()
		{
			if (Global.Data.roleData.FluorescentDiamondData == null || Global.Data.roleData.FluorescentDiamondData.GemInstalList == null)
			{
				return null;
			}
			return Global.Data.roleData.FluorescentDiamondData.GemInstalList;
		}

		public static void SetEquipedDiamondList(Dictionary<int, Dictionary<int, GoodsData>> dic_diamond)
		{
			if (Global.Data.roleData.FluorescentDiamondData == null)
			{
				Global.Data.roleData.FluorescentDiamondData = new FluorescentGemData();
			}
			Global.Data.roleData.FluorescentDiamondData.GemInstalList = dic_diamond;
		}

		public static Dictionary<int, GoodsData> GetEquipedDiamondsBySlotID(int slotID)
		{
			if (slotID < 1 || slotID > 10)
			{
				return null;
			}
			Dictionary<int, Dictionary<int, GoodsData>> equipedDiamondList = Global.GetEquipedDiamondList();
			if (equipedDiamondList == null)
			{
				return null;
			}
			return equipedDiamondList.GetValue(slotID);
		}

		public static void OnGoodsChangedInDimaondBag(GoodsData gd, BagModifyType type)
		{
			int bagIndex = gd.BagIndex;
			Dictionary<int, GoodsData> dictionary = Global.GetBagDiamondList();
			if (dictionary == null)
			{
				dictionary = new Dictionary<int, GoodsData>();
			}
			GoodsData value = dictionary.GetValue(bagIndex);
			switch (type)
			{
			case BagModifyType.BagModifyType_Add:
				if (value == null)
				{
					dictionary.Add(bagIndex, gd);
				}
				else
				{
					int gcount = value.GCount + gd.GCount;
					value.GCount = gcount;
					dictionary[bagIndex] = value;
				}
				break;
			case BagModifyType.BagModifyType_Remove:
				if (value != null)
				{
					int num = value.GCount - gd.GCount;
					value.GCount = num;
					if (num <= 0)
					{
						dictionary.Remove(bagIndex);
					}
					else
					{
						dictionary[bagIndex] = value;
					}
				}
				break;
			case BagModifyType.BagModifyType_Replace:
				if (value == null)
				{
					dictionary.Add(bagIndex, gd);
				}
				else
				{
					dictionary[bagIndex] = gd;
				}
				break;
			case BagModifyType.BagModifyType_Destroy:
				if (value != null)
				{
					dictionary.Remove(bagIndex);
				}
				break;
			}
			if (Global.Data.roleData.FluorescentDiamondData == null)
			{
				Global.Data.roleData.FluorescentDiamondData = new FluorescentGemData();
			}
			Global.Data.roleData.FluorescentDiamondData.GemStoreList = dictionary;
		}

		public static Dictionary<int, GoodsData> GetBagDiamondList()
		{
			if (Global.Data.roleData.FluorescentDiamondData == null || Global.Data.roleData.FluorescentDiamondData.GemStoreList == null)
			{
				return null;
			}
			return Global.Data.roleData.FluorescentDiamondData.GemStoreList;
		}

		public static void SetBagDiamondList(Dictionary<int, GoodsData> diamondList)
		{
			if (Global.Data.roleData.FluorescentDiamondData == null)
			{
				Global.Data.roleData.FluorescentDiamondData = new FluorescentGemData();
			}
			Global.Data.roleData.FluorescentDiamondData.GemStoreList = diamondList;
		}

		public static GoodsData GetBagDiamondGoodsDataAtIndex(int bagIndex)
		{
			Dictionary<int, GoodsData> bagDiamondList = Global.GetBagDiamondList();
			GoodsData result = null;
			if (bagDiamondList != null)
			{
				result = bagDiamondList.GetValue(bagIndex);
			}
			return result;
		}

		public static List<GoodsData> GetAvailableDiamondByShape(int shapeID)
		{
			Dictionary<int, GoodsData> bagDiamondList = Global.GetBagDiamondList();
			if (bagDiamondList == null || bagDiamondList.Count <= 0)
			{
				return null;
			}
			List<GoodsData> list = new List<GoodsData>();
			foreach (KeyValuePair<int, GoodsData> keyValuePair in bagDiamondList)
			{
				GoodsData value = keyValuePair.Value;
				if (Global.IsGoodsMatchShape(value.GoodsID, shapeID))
				{
					list.Add(value);
				}
			}
			return list;
		}

		public static List<GoodsData> GetUpgradableDiamondByType(int type, int shapeID, int goodsID)
		{
			Dictionary<int, GoodsData> bagDiamondList = Global.GetBagDiamondList();
			if (bagDiamondList == null || bagDiamondList.Count <= 0)
			{
				return null;
			}
			List<GoodsData> list = new List<GoodsData>();
			foreach (KeyValuePair<int, GoodsData> keyValuePair in bagDiamondList)
			{
				GoodsData value = keyValuePair.Value;
				if (Global.IsGoodsMatchType(value.GoodsID, type, shapeID) && value.GoodsID <= goodsID)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public static bool IsAvailableDiamondInBag(int shapeID)
		{
			Dictionary<int, GoodsData> bagDiamondList = Global.GetBagDiamondList();
			if (bagDiamondList == null || bagDiamondList.Count <= 0)
			{
				return false;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in bagDiamondList)
			{
				GoodsData value = keyValuePair.Value;
				if (Global.IsGoodsMatchShape(value.GoodsID, shapeID))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsUpgradableDiamond()
		{
			Dictionary<int, Dictionary<int, GoodsData>> equipedDiamondList = Global.GetEquipedDiamondList();
			if (equipedDiamondList == null || equipedDiamondList.Count <= 0)
			{
				return false;
			}
			for (int i = 1; i <= 10; i++)
			{
				if (Global.IsUpgradableDiamondInEquipPart(i))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsUpgradableDiamondInEquipPart(int partID)
		{
			Dictionary<int, Dictionary<int, GoodsData>> equipedDiamondList = Global.GetEquipedDiamondList();
			if (equipedDiamondList == null || equipedDiamondList.Count <= 0)
			{
				return false;
			}
			Dictionary<int, GoodsData> value = equipedDiamondList.GetValue(partID);
			if (value == null || value.Count <= 0)
			{
				return false;
			}
			for (int i = 1; i <= 3; i++)
			{
				if (Global.IsUpgradableDiamondAtSlot(value, i))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsUpgradableDiamondAtSlot(Dictionary<int, GoodsData> dic_diamond, int shapeID)
		{
			if (dic_diamond == null || dic_diamond.Count <= 0)
			{
				return false;
			}
			GoodsData value = dic_diamond.GetValue(shapeID);
			if (value == null || value.GoodsID <= 0)
			{
				return false;
			}
			int num = -1;
			DiamondAttribute diamondAttributeByGoodsID = Global.GetDiamondAttributeByGoodsID(value.GoodsID, out num);
			if (diamondAttributeByGoodsID == null)
			{
				return false;
			}
			int nextLevelGoodsID = diamondAttributeByGoodsID.nextLevelGoodsID;
			DiamondAttribute diamondAttributeByGoodsID2 = Global.GetDiamondAttributeByGoodsID(nextLevelGoodsID, out num);
			if (diamondAttributeByGoodsID2 == null || num < 0)
			{
				return false;
			}
			if (nextLevelGoodsID <= 0)
			{
				return false;
			}
			List<GoodsData> upgradableDiamondByType = Global.GetUpgradableDiamondByType(diamondAttributeByGoodsID2.type, num, value.GoodsID);
			if (upgradableDiamondByType == null)
			{
				return false;
			}
			upgradableDiamondByType.Sort(delegate(GoodsData x, GoodsData y)
			{
				if (y.GoodsID == x.GoodsID)
				{
					return y.GCount - x.GCount;
				}
				return y.GoodsID - x.GoodsID;
			});
			int num2 = diamondAttributeByGoodsID2.needLevelOneNum - diamondAttributeByGoodsID.needLevelOneNum;
			if (upgradableDiamondByType == null || upgradableDiamondByType.Count <= 0)
			{
				return false;
			}
			int num3 = 0;
			for (int i = 0; i < upgradableDiamondByType.Count; i++)
			{
				GoodsData goodsData = upgradableDiamondByType[i];
				DiamondAttribute diamondAttributeByGoodsID3 = Global.GetDiamondAttributeByGoodsID(goodsData.GoodsID, out num);
				if (diamondAttributeByGoodsID3 != null && diamondAttributeByGoodsID3.needLevelOneNum != 0)
				{
					int num4 = (num2 - num3 + diamondAttributeByGoodsID3.needLevelOneNum - 1) / diamondAttributeByGoodsID3.needLevelOneNum;
					int num5 = Mathf.Min(num4, goodsData.GCount);
					num3 += num5 * diamondAttributeByGoodsID3.needLevelOneNum;
					if (num3 >= num2)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool InitTiShi()
		{
			if (Global.dic_holyItem == null || Global.dic_holyItem.Keys.Count == 0)
			{
				return false;
			}
			sbyte b = 1;
			while ((int)b < 5)
			{
				HolyItemData holyItemData = null;
				if (Global.dic_holyItem.ContainsKey(b))
				{
					Global.dic_holyItem.TryGetValue(b, ref holyItemData);
					if (holyItemData != null)
					{
						Global.m_PartArray = holyItemData.m_PartArray;
					}
				}
				int num = (int)ConfigSystemParam.GetSystemParamIntByName("ShengWuMax");
				sbyte b2 = 0;
				while ((int)b2 < 6)
				{
					HolyItemPartData value = Global.m_PartArray.GetValue((sbyte)((int)b2 + 1));
					int bujianID = Global.GetBujianID((int)b, (int)b2 + 1, (int)value.m_sSuit);
					int num2 = 0;
					if (!Global.dic_holyPartAttr[bujianID].NeedGoods.Equals("-1"))
					{
						num2 = int.Parse(Global.dic_holyPartAttr[bujianID].NeedGoods.Split(new char[]
						{
							','
						})[1]);
					}
					int costBandJianBi = Global.dic_holyPartAttr[bujianID].CostBandJianBi;
					int num3 = (int)value.m_sSuit;
					if (num2 <= value.m_nSlice && costBandJianBi <= Global.Data.roleData.Money1 + Global.GetRoleOwnNumByMoneyType(8) && num3 < num - 1)
					{
						return true;
					}
					b2 += 1;
				}
				b += 1;
			}
			return false;
		}

		public static float GetShengWuTotalLevel()
		{
			float num = 0f;
			if (Global.dic_holyItem.Keys.Count <= 0)
			{
				return 0f;
			}
			sbyte b = 1;
			while ((int)b < 5)
			{
				HolyItemData holyItemData = null;
				if (Global.dic_holyItem.ContainsKey(b))
				{
					Global.dic_holyItem.TryGetValue(b, ref holyItemData);
					if (holyItemData != null)
					{
						Global.m_PartArray = holyItemData.m_PartArray;
					}
				}
				sbyte b2 = 0;
				while ((int)b2 < 6)
				{
					HolyItemPartData value = Global.m_PartArray.GetValue((sbyte)((int)b2 + 1));
					num += (float)value.m_sSuit;
					b2 += 1;
				}
				b += 1;
			}
			return num;
		}

		public static void SetPartInfo(int stype, int part, int suit, int slice)
		{
			HolyItemData holyItemData = null;
			if (Global.dic_holyItem.ContainsKey((sbyte)stype))
			{
				Global.dic_holyItem.TryGetValue((sbyte)stype, ref holyItemData);
				if (holyItemData != null)
				{
					Global.m_PartArray = holyItemData.m_PartArray;
				}
				HolyItemPartData value = Global.m_PartArray.GetValue((sbyte)part);
				value.m_sSuit = (sbyte)suit;
				value.m_nSlice = slice;
			}
		}

		public static void GetStrenthenDecoration1(GSprite sprite, int roleID)
		{
			RoleData roleData;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (Global.Data.SysSetting.HideGameEffect && ConfigSystemParam.GetSystemParamIntByName("IsOpenQiangHua20Effect") == 1L)
			{
				roleData = null;
			}
			else
			{
				roleData = Global.FindRoleDataByID(roleID);
			}
			if (roleData == null)
			{
				return;
			}
			List<GoodsData> goodsDataList = roleData.GoodsDataList;
			List<int> list = new List<int>();
			if (goodsDataList == null)
			{
				return;
			}
			if (goodsDataList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataList[i].GoodsID);
				if (goodsXmlNodeByID != null && goodsXmlNodeByID.Categoriy != 10 && goodsXmlNodeByID.Categoriy != 9 && goodsXmlNodeByID.Categoriy != 22 && goodsXmlNodeByID.Categoriy != 24 && goodsXmlNodeByID.Categoriy != 23 && goodsXmlNodeByID.Categoriy != 7 && goodsXmlNodeByID.Categoriy != 340 && (goodsXmlNodeByID.Categoriy < 40 || goodsXmlNodeByID.Categoriy > 45))
				{
					if (goodsDataList[i].Using != 0)
					{
						list.Add(goodsDataList[i].Forge_level);
					}
				}
			}
			if (list.Count >= 8)
			{
				list.Sort();
				int num = list[list.Count - 8];
				List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
				int count = taoZhuangList.Count;
				if (num >= taoZhuangList[count - 1].Level)
				{
					int num2 = count - 1;
					if (!string.IsNullOrEmpty(taoZhuangList[count - 1].Decoration1))
					{
						string[] array = taoZhuangList[count - 1].Decoration1.Split(new char[]
						{
							'|'
						});
						if (array != null)
						{
							for (int j = 0; j < array.Length; j++)
							{
								int num3 = array[j].SafeToInt32(0);
								if (num3 >= 0)
								{
									string text = string.Format("DelayDecoration_{0}", num3);
									GDecoration gdecoration = sprite.Root.FindName(text) as GDecoration;
									if (gdecoration != null)
									{
										Global.RemoveObject(gdecoration, true);
									}
									gdecoration = (ObjectsManager.FindSprite(text) as GDecoration);
									if (gdecoration != null)
									{
										Global.RemoveObject(gdecoration, true);
									}
									Point pos = new Point(sprite.CenterX, sprite.CenterY);
									gdecoration = Global.GetDecoration(num3, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
									if (gdecoration != null)
									{
										gdecoration.Name = text;
										sprite.Root.Children.Add(gdecoration);
										if (Global.IsBufferExist(121, roleData))
										{
											float qiang20Scale = ShenHunData.GetQiang20Scale(roleData);
											gdecoration.The3DGameObject.transform.localScale = new Vector3(qiang20Scale, qiang20Scale, qiang20Scale);
										}
										else
										{
											gdecoration.The3DGameObject.transform.localScale = Vector3.one;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public static void GetStrenthenDecoration()
		{
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			List<int> list = new List<int>();
			if (usingGoodsDataList == null)
			{
				return;
			}
			if (usingGoodsDataList.Count <= 0)
			{
				return;
			}
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(keyValuePair.Value.GoodsID);
				if (goodsXmlNodeByID != null && goodsXmlNodeByID.Categoriy != 10 && goodsXmlNodeByID.Categoriy != 9 && goodsXmlNodeByID.Categoriy != 22 && goodsXmlNodeByID.Categoriy != 24 && goodsXmlNodeByID.Categoriy != 23 && goodsXmlNodeByID.Categoriy != 7)
				{
					list.Add(keyValuePair.Value.Forge_level);
				}
			}
			if (list.Count >= 8)
			{
				list.Sort();
				int num = list[list.Count - 8];
				List<TaoZhuangVO> taoZhuangList = Global.GetTaoZhuangList();
				if (taoZhuangList != null)
				{
					int count = taoZhuangList.Count;
					if (num >= taoZhuangList[count - 1].Level)
					{
						GSprite gsprite = Global.FindSprite("Leader");
						if (!string.IsNullOrEmpty(taoZhuangList[count - 1].Decoration1))
						{
							int num2 = int.Parse(taoZhuangList[count - 1].Decoration1);
							if (num2 >= 0 && gsprite != null)
							{
								string text = string.Format("DelayDecoration_{0}", num2);
								GDecoration gdecoration = gsprite.Root.FindName(text) as GDecoration;
								if (gdecoration != null)
								{
									Global.RemoveObject(gdecoration, true);
								}
								gdecoration = (ObjectsManager.FindSprite(text) as GDecoration);
								if (gdecoration != null)
								{
									Global.RemoveObject(gdecoration, true);
								}
								Point pos = new Point(gsprite.CenterX, gsprite.CenterY);
								gdecoration = Global.GetDecoration(num2, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
								if (gdecoration != null)
								{
									gdecoration.Name = text;
									gsprite.Root.Children.Add(gdecoration);
								}
							}
						}
					}
				}
			}
		}

		public static List<XElement> GetJingGao()
		{
			XElement isolateResXml = Global.GetIsolateResXml("Config/JingGao.xml");
			return Global.GetXElementList(isolateResXml, "JingGao");
		}

		public static Dictionary<int, string> GetJinDeng()
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			dictionary.Add(-1, Global.GetLang("您的账号存在{ff0000}异常情况{ffffff}，不能登录，如有疑问请联系客服{17e43f}QQ:4009933199{ffffff}咨询！"));
			dictionary.Add(-2, Global.GetLang("您的账号为{ff0000}黑名单账户{ffffff}，不能登录，如有疑问请联系客服{17e43f}QQ:4009933199{ffffff}咨询！"));
			dictionary.Add(-3, Global.GetLang("您的账号处于{ff0000}封号状态{ffffff}，不能登录，如有疑问请联系客服{17e43f}QQ:4009933199{ffffff}咨询！"));
			dictionary.Add(-4, Global.GetLang("您的账号已被{ff0000}封停{ffffff}，不能登录，如有疑问请联系客服{17e43f}QQ:4009933199{ffffff}咨询！"));
			dictionary.Add(-5, Global.GetLang("您的账号设备存在{ff0000}异常情况{ffffff}，不能登录，如有疑问请联系客服{17e43f}QQ:4009933199{ffffff}咨询！"));
			return dictionary;
		}

		public static void PlatWeiGuiJingGao(int strPlatUID)
		{
			List<XElement> jingGao = Global.GetJingGao();
			if (jingGao.Count <= 0)
			{
				return;
			}
			Super.HideNetWaiting();
			string message = string.Empty;
			if (strPlatUID == 1)
			{
				for (int i = 0; i < jingGao.Count; i++)
				{
					XElement xelement = jingGao[i];
					if (Global.GetXElementAttributeInt(xelement, "ID") == 1)
					{
						message = Global.GetXElementAttributeStr(xelement, "Description");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Operate");
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Time");
						PlayZone.GlobalPlayZone.ShowWeiGuiJingGao(Global.MainStage, Global.GetLang("我知道了"), message, xelementAttributeInt, xelementAttributeInt2, true, default(Vector3));
						break;
					}
				}
			}
			else if (strPlatUID == 2)
			{
				for (int j = 0; j < jingGao.Count; j++)
				{
					XElement xelement2 = jingGao[j];
					if (Global.GetXElementAttributeInt(xelement2, "ID") == 2)
					{
						message = Global.GetXElementAttributeStr(xelement2, "Description");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement2, "Operate");
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement2, "Time");
						PlayZone.GlobalPlayZone.ShowWeiGuiJingGao(Global.MainStage, Global.GetLang("我知道了"), message, xelementAttributeInt, xelementAttributeInt2, true, default(Vector3));
						break;
					}
				}
			}
			else if (strPlatUID == 3)
			{
				for (int k = 0; k < jingGao.Count; k++)
				{
					XElement xelement3 = jingGao[k];
					if (Global.GetXElementAttributeInt(xelement3, "ID") == 3)
					{
						message = Global.GetXElementAttributeStr(xelement3, "Description");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement3, "Operate");
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement3, "Time");
						PlayZone.GlobalPlayZone.ShowWeiGuiJingGao(Global.MainStage, Global.GetLang("我知道了"), message, xelementAttributeInt, xelementAttributeInt2, true, default(Vector3));
						break;
					}
				}
			}
			else if (strPlatUID == 4)
			{
				for (int l = 0; l < jingGao.Count; l++)
				{
					XElement xelement4 = jingGao[l];
					if (Global.GetXElementAttributeInt(xelement4, "ID") == 4)
					{
						message = Global.GetXElementAttributeStr(xelement4, "Description");
						int xelementAttributeInt = Global.GetXElementAttributeInt(xelement4, "Operate");
						int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement4, "Time");
						PlayZone.GlobalPlayZone.ShowWeiGuiJingGao(Global.MainStage, Global.GetLang("我知道了"), message, xelementAttributeInt, xelementAttributeInt2, true, default(Vector3));
						break;
					}
				}
			}
		}

		public static bool IsInKuafuHuodongZhenYing()
		{
			return Global.IsInKuafuHuodongYongZheZhanChang() || Global.IsInHuanYingSiYuan() || Global.IsInKuaFuHuoDongWangZhe();
		}

		public static bool InZhanMengLianSaiScene()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 45;
		}

		public static bool IsInLangHunLingYuScene()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 35;
		}

		public static bool InKuafuHuodongSingle()
		{
			return Global.IsInKuafuHuodongMoLian();
		}

		public static bool IsInKuaFuHuoDongWangZhe()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 39;
		}

		public static bool IsInHuanYingSiYuan()
		{
			return Global.Data.roleData.MapCode == 13000;
		}

		public static bool IsInKuafuHuodongYongZheZhanChang()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 27;
		}

		public static bool IsInKuafuHuodongMoLian()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 31;
		}

		public static bool IsInKuaFuTeamCompete()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && (settingMapVOByCode.MapType == 55 || settingMapVOByCode.MapType == 56 || settingMapVOByCode.MapType == 57);
		}

		public static bool IsInMoYuDuoBao()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 57;
		}

		public static bool IsInDaTaoSha()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 59;
		}

		public static bool IsInDaTaoShaPrepare()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 58;
		}

		public static bool InKuafuFuben()
		{
			return Global.Data.roleData.MapCode == 70000 || Global.Data.roleData.MapCode == 70100 || Global.Data.roleData.MapCode == 70200 || Global.Data.roleData.MapCode == 4000 || Global.Data.roleData.MapCode == 70300;
		}

		public static bool IsInJingLingYaoSai()
		{
			return Global.Data.roleData.MapCode >= 84000 && Global.Data.roleData.MapCode <= 84000;
		}

		public static bool IsOperateUnPermitInKuaFuMapCheck(bool showHint = false, bool checkFuBen = false)
		{
			bool flag = false;
			if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || SceneUIClasses.RebornMap.IsTheScene())
			{
				flag = true;
			}
			if (checkFuBen && Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
			{
				flag = true;
			}
			if (flag && showHint)
			{
				Super.HintMainText(Global.GetLang("跨服地图中不能使用此功能！"), 10, 3);
			}
			else
			{
				if (Global.GetMapSceneUIClass() == SceneUIClasses.Comp)
				{
					flag = true;
				}
				if (flag && showHint)
				{
					Super.HintMainText(Global.GetLang("势力地图中不能使用此功能！"), 10, 3);
				}
			}
			return flag;
		}

		public static string GetValueByAnyKeyInMoRiByID(int targetID, string key)
		{
			XElement gameResXml = Global.GetGameResXml("Config/MoRiShenPan.xml");
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "MoRiShenPan", "ID", targetID.ToString());
			if (xelement == null)
			{
				return null;
			}
			return Global.GetXElementAttributeStr(xelement, key);
		}

		public static int SetShuijingEffect(int ID)
		{
			if (Global.MoRiShenPanShuijingState.ContainsKey(ID))
			{
				return Global.MoRiShenPanShuijingState[ID];
			}
			return 0;
		}

		public static string GetEffectName(int ID)
		{
			string result = string.Empty;
			switch (ID)
			{
			case 1:
				result = "BaiShuiJing_effect_0{0}.unity3d";
				break;
			case 2:
				result = "HongShuiJing_effect_0{0}.unity3d";
				break;
			case 3:
				result = "HeiShuiJing_effect_0{0}.unity3d";
				break;
			case 4:
				result = "LvShuiJing_effect_0{0}.unity3d";
				break;
			}
			return result;
		}

		public static bool IsKoreanOrEn(string value)
		{
			foreach (char c in value.ToCharArray())
			{
				if ((c <= '꬀' || c >= 'ꭟ') && !char.IsDigit(c) && !char.IsLower(c) && !char.IsUpper(c) && !(c.ToString() == "-"))
				{
					return false;
				}
			}
			return true;
		}

		public static void SetShuijingState(int ID, int thisState)
		{
			if (Global.MoRiShenPanShuijingState.ContainsKey(ID))
			{
				Global.MoRiShenPanShuijingState[ID] = thisState;
			}
			else
			{
				Global.MoRiShenPanShuijingState.Add(ID, thisState);
			}
			GSprite gsprite = Global.FindSprite(Global.GetNPCName(ID - 1 + 91000));
			if (gsprite != null)
			{
				int num = Global.SetShuijingEffect(ID);
				int num2 = (ID - 1) * 3 + 13000;
				string text = string.Format(Global.GetEffectName(ID), Global.endState);
				string text2 = string.Format(Global.GetEffectName(ID), num);
				if (Global.endState != thisState)
				{
					if (Global.shuijingtexiao != null)
					{
						Object.Destroy(Global.shuijingtexiao);
						Global.shuijingtexiao = null;
					}
					string bundleID = MuAssetManager.GetBundleID("Decoration", ConfigDecoration.GetDecorationVOByCode((num != 0) ? (num - 1 + num2) : num2).ResName);
					Global.shuijingtexiao = U3DUtils.GetEmptyLoader(ConfigDecoration.GetDecorationVOByCode((num != 0) ? (num - 1 + num2) : num2).ResName, bundleID, false, null, null, -1, new AssetbundleLoaderComplete(Global.LoadShuijingOK), -1, 1f, true, false, null);
					Global.shuijingtexiao.transform.localPosition = new Vector3(0f, 0f, 0f);
					Global.shuijingtexiao.GetComponent<AssetbundleLoader>().AutoDestroySelf = false;
					U3DUtils.AddChild(gsprite.The3DGameObject, Global.shuijingtexiao, true);
				}
				Global.endState = num;
			}
		}

		public static void LoadShuijingOK(AssetbundleLoader loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			LoadRoleShaderAgain loadRoleShaderAgain = go.AddComponent<LoadRoleShaderAgain>();
		}

		public static bool IsSupportThread()
		{
			return false;
		}

		public static bool IsUsingXMLVO()
		{
			long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("IsUsingXMLVO_Android");
			return systemParamIntByName > 0L;
		}

		public static bool IsQiRiKuangHuanActivityEnd()
		{
			bool result = false;
			if (Global.GetQiRiKuanghuanDaysNum() > 7)
			{
				result = true;
			}
			return result;
		}

		public static int GetQiRiKuanghuanDaysNum()
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			long ticks = Global.GetQiRiKuangHuanStartTime().Ticks;
			long ticks2 = correctDateTime.Ticks;
			long num = ticks2 - ticks;
			long num2 = num / 10000000L;
			int num3 = (int)(num2 / 86400L) + 1;
			if (num3 < 0 && num3 > 6)
			{
				num3 = -1;
			}
			return num3;
		}

		public static DateTime GetQiRiKuangHuanStartTime()
		{
			if (Global.Data == null || Global.Data.roleData == null)
			{
				return Global._QiRiKuangHuanTime;
			}
			if (string.IsNullOrEmpty(Global.Data.roleData.RegTime))
			{
				return Global._QiRiKuangHuanTime;
			}
			DateTime qiRiKuangHuanTime = default(DateTime);
			string[] array = Global.Data.roleData.RegTime.Split(new char[]
			{
				' '
			});
			DateTime.TryParse(array[0] + " 00:00:00", ref qiRiKuangHuanTime);
			Global._QiRiKuangHuanTime = qiRiKuangHuanTime;
			return Global._QiRiKuangHuanTime;
		}

		public static void LoadSevenDayGoalxml()
		{
			XElement gameResXml = Global.GetGameResXml("Config/SevenDayGoal.xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Goal");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				SevendayGoal sevendayGoal = new SevendayGoal();
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Day");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "GoalType");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "FunctionType");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Describe");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "TypeGoal");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "Award");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "ShowNum");
				sevendayGoal.ID = xelementAttributeInt;
				sevendayGoal.Day = xelementAttributeInt2;
				sevendayGoal.GoalType = xelementAttributeInt3;
				sevendayGoal.FunctionType = xelementAttributeInt4;
				sevendayGoal.Describe = xelementAttributeStr;
				sevendayGoal.TypeGoal = xelementAttributeStr2;
				sevendayGoal.Award = xelementAttributeStr3;
				sevendayGoal.ShowNum = xelementAttributeInt5;
				if (!Global.dic_SevenDayGoal.ContainsKey(xelementAttributeInt))
				{
					Global.dic_SevenDayGoal.Add(xelementAttributeInt, sevendayGoal);
				}
			}
		}

		public static void LoadXml()
		{
			XElement gameResXml = Global.GetGameResXml("SevenDayQiangGou.xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Goods");
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				SevenDayQiangGou sevenDayQiangGou = new SevenDayQiangGou();
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Day");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "GoodsID");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "OrigPrice");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "Price");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "Purchase");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Name");
				sevenDayQiangGou.ID = xelementAttributeInt;
				sevenDayQiangGou.Day = xelementAttributeInt2;
				sevenDayQiangGou.GoodsID = xelementAttributeStr;
				sevenDayQiangGou.OrigPrice = xelementAttributeInt3;
				sevenDayQiangGou.Price = xelementAttributeInt4;
				sevenDayQiangGou.Purchase = xelementAttributeInt5;
				sevenDayQiangGou.Name = xelementAttributeStr2;
				if (!Global.dic_SevenDayQiangGou.ContainsKey(xelementAttributeInt))
				{
					Global.dic_SevenDayQiangGou.Add(xelementAttributeInt, sevenDayQiangGou);
				}
			}
		}

		public static int GetItemCount(int day)
		{
			int num = 0;
			foreach (KeyValuePair<int, SevenDayQiangGou> keyValuePair in Global.dic_SevenDayQiangGou)
			{
				if (day == keyValuePair.Value.Day)
				{
					num++;
				}
			}
			return num;
		}

		public static AppPlatform DevicePlatform()
		{
			return AppPlatform.Android;
		}

		public static List<GoodsData> GetFashionEquipGoodsDataList(int GoodsID, int occupation, int FashionForgeLev)
		{
			List<GoodsData> list = new List<GoodsData>();
			int forge_Level = FashionForgeLev + 1;
			occupation = Global.CalcOriginalOccupationID(occupation);
			Dictionary<int, Dictionary<int, Global.ShiZhuangResInfo>> fashionInfo = Global.GetFashionInfo();
			Dictionary<int, Global.ShiZhuangResInfo> dictionary;
			if (fashionInfo.TryGetValue(GoodsID, ref dictionary))
			{
				list.Add(Global.GetFakeEquipGoodsData(dictionary[occupation].TouRes, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(dictionary[occupation].XiongRes, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(dictionary[occupation].ShouRes, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(dictionary[occupation].TuiRes, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(dictionary[occupation].XieRes, forge_Level, 0));
			}
			return list;
		}

		public static GoodsData GetFashionChiBangGoodsData(List<GoodsData> lst)
		{
			if (lst != null)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					if (Global.GetCategoriyByGoodsID(lst[i].GoodsID) == 8 && lst[i].Using == 1)
					{
						return lst[i];
					}
				}
			}
			return null;
		}

		public static GoodsData[] DealWithFashion(List<GoodsData> usingGoodsDataList)
		{
			GoodsData[] array = new GoodsData[4];
			byte b = 0;
			while ((int)b < array.Length)
			{
				array[(int)b] = null;
				b += 1;
			}
			byte b2 = 0;
			if (usingGoodsDataList != null)
			{
				for (int i = 0; i < usingGoodsDataList.Count; i++)
				{
					if (Global.IsFashion(usingGoodsDataList[i].GoodsID) && usingGoodsDataList[i].Using == 1)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(usingGoodsDataList[i].GoodsID);
						if (categoriyByGoodsID == 24)
						{
							array[0] = usingGoodsDataList[i];
							b2 += 1;
						}
						else if (categoriyByGoodsID == 8)
						{
							array[1] = usingGoodsDataList[i];
							b2 += 1;
						}
						else if (categoriyByGoodsID == 25)
						{
							array[2] = usingGoodsDataList[i];
							b2 += 1;
						}
						else if (categoriyByGoodsID == 26)
						{
							array[3] = usingGoodsDataList[i];
							b2 += 1;
						}
						if (3 < b2)
						{
							break;
						}
					}
				}
			}
			return array;
		}

		public static bool IsFashioned(out GoodsData[] GoodsIDList)
		{
			bool result = false;
			GoodsIDList = new GoodsData[4];
			byte b = 0;
			while ((int)b < GoodsIDList.Length)
			{
				GoodsIDList[(int)b] = null;
				b += 1;
			}
			List<GoodsData> list = new List<GoodsData>();
			Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
			if (usingGoodsDataList != null)
			{
				Dictionary<int, GoodsData>.Enumerator enumerator = usingGoodsDataList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					List<GoodsData> list2 = list;
					KeyValuePair<int, GoodsData> keyValuePair = enumerator.Current;
					list2.Add(keyValuePair.Value);
				}
			}
			GoodsIDList = Global.DealWithFashion(list);
			byte b2 = 0;
			while ((int)b2 < GoodsIDList.Length)
			{
				if (GoodsIDList[(int)b2] != null)
				{
					result = true;
					break;
				}
				b2 += 1;
			}
			return result;
		}

		public static bool IsOtherFashioned(RoleData roleData, out GoodsData[] GoodsIDList)
		{
			bool result = false;
			GoodsIDList = new GoodsData[4];
			byte b = 0;
			while ((int)b < GoodsIDList.Length)
			{
				GoodsIDList[(int)b] = null;
				b += 1;
			}
			GoodsIDList = Global.DealWithFashion(roleData.GoodsDataList);
			byte b2 = 0;
			while ((int)b2 < GoodsIDList.Length)
			{
				if (GoodsIDList[(int)b2] != null)
				{
					result = true;
					break;
				}
				b2 += 1;
			}
			return result;
		}

		public static List<GoodsData> GetOtherBattleWeaponGoodsDataListInZhanMengLianSai(RoleData roleData)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (roleData.GoodsDataList == null)
			{
				return Global.GetBattleWeaponGoodsDataList(roleData.BattleWhichSide, roleData.Occupation);
			}
			int forge_Level = 0;
			int num = Global.CalcOriginalOccupationID(roleData.Occupation);
			if (roleData.BattleWhichSide == 2)
			{
				if (num == 0)
				{
					list.Add(Global.GetFakeEquipGoodsData(1005080, forge_Level, 0));
					list.Add(Global.GetFakeEquipGoodsData(1005080, forge_Level, 1));
				}
				else if (num == 1)
				{
					list.Add(Global.GetFakeEquipGoodsData(1015080, forge_Level, 0));
				}
				else if (num == 2)
				{
					list.Add(Global.GetFakeEquipGoodsData(1022480, forge_Level, 0));
				}
				else if (num == 3)
				{
					list.Add(Global.GetFakeEquipGoodsData(1035180, forge_Level, 0));
				}
				else if (num == 5)
				{
					list.Add(Global.GetFakeEquipGoodsData(1055181, forge_Level, 0));
				}
			}
			else if (num == 0)
			{
				list.Add(Global.GetFakeEquipGoodsData(1005081, forge_Level, 0));
				list.Add(Global.GetFakeEquipGoodsData(1005081, forge_Level, 1));
			}
			else if (num == 1)
			{
				list.Add(Global.GetFakeEquipGoodsData(1015081, forge_Level, 0));
			}
			else if (num == 2)
			{
				list.Add(Global.GetFakeEquipGoodsData(1022481, forge_Level, 0));
			}
			else if (num == 3)
			{
				list.Add(Global.GetFakeEquipGoodsData(1035181, forge_Level, 0));
			}
			else if (num == 5)
			{
				list.Add(Global.GetFakeEquipGoodsData(1055180, forge_Level, 0));
			}
			List<GoodsData> goodsDataList = roleData.GoodsDataList;
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (goodsDataList[i].Using == 1)
				{
					int goodsID = goodsDataList[i].GoodsID;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
					if (goodsXmlNodeByID.Categoriy == 10 || goodsXmlNodeByID.Categoriy == 9)
					{
						list.Add(Global.GetFakeEquipGoodsData(goodsDataList[i].GoodsID, goodsDataList[i].Forge_level, goodsDataList[i].BagIndex));
					}
				}
			}
			return list;
		}

		public static List<GoodsData> GetOtherBattleWeaponGoodsDataList(RoleData roleData)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (roleData.GoodsDataList == null)
			{
				return list;
			}
			List<GoodsData> goodsDataList = roleData.GoodsDataList;
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (goodsDataList[i].Using == 1)
				{
					int goodsID = goodsDataList[i].GoodsID;
					GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
					if ((goodsXmlNodeByID.Categoriy >= 11 && goodsXmlNodeByID.Categoriy <= 21) || goodsXmlNodeByID.Categoriy == 10 || goodsXmlNodeByID.Categoriy == 9)
					{
						list.Add(Global.GetFakeEquipGoodsData(goodsDataList[i].GoodsID, goodsDataList[i].Forge_level, goodsDataList[i].BagIndex));
					}
				}
			}
			return list;
		}

		public static bool IsFakeRoleFashioned(RoleDataMini roleData, out GoodsData[] goodsIDList)
		{
			bool result = false;
			goodsIDList = new GoodsData[4];
			byte b = 0;
			while ((int)b < goodsIDList.Length)
			{
				goodsIDList[(int)b] = null;
				b += 1;
			}
			if (roleData.GoodsDataList == null)
			{
				return false;
			}
			List<GoodsData> goodsDataList = roleData.GoodsDataList;
			byte b2 = 0;
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				if (Global.IsFashion(goodsDataList[i].GoodsID))
				{
					result = true;
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataList[i].GoodsID);
					if (categoriyByGoodsID == 24)
					{
						goodsIDList[0] = goodsDataList[i];
						b2 += 1;
					}
					else if (categoriyByGoodsID == 8)
					{
						goodsIDList[1] = goodsDataList[i];
						b2 += 1;
					}
					else if (categoriyByGoodsID == 25)
					{
						goodsIDList[2] = goodsDataList[i];
						b2 += 1;
					}
					else if (categoriyByGoodsID == 26)
					{
						goodsIDList[3] = goodsDataList[i];
						b2 += 1;
					}
					if (3 < b2)
					{
						break;
					}
				}
			}
			return result;
		}

		public static List<GoodsData> GetFakeRoleBattleWeaponGoodsDataList(RoleDataMini roleData)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (roleData.GoodsDataList == null)
			{
				return list;
			}
			List<GoodsData> goodsDataList = roleData.GoodsDataList;
			for (int i = 0; i < goodsDataList.Count; i++)
			{
				int goodsID = goodsDataList[i].GoodsID;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
				if ((goodsXmlNodeByID.Categoriy >= 11 && goodsXmlNodeByID.Categoriy <= 21) || goodsXmlNodeByID.Categoriy == 9 || goodsXmlNodeByID.Categoriy == 10)
				{
					list.Add(Global.GetFakeEquipGoodsData(goodsDataList[i].GoodsID, goodsDataList[i].Forge_level, goodsDataList[i].BagIndex));
				}
			}
			return list;
		}

		public static bool IsFashion(int GoodsID)
		{
			return Global.GetFashionInfo().ContainsKey(GoodsID) || ConfigFashion.IsFashion(GoodsID, (ItemCategories)Global.GetCategoriyByGoodsID(GoodsID));
		}

		public static Dictionary<int, Dictionary<int, Global.ShiZhuangResInfo>> GetFashionInfo()
		{
			if (Global.shiZhuangDic.Count > 0)
			{
				return Global.shiZhuangDic;
			}
			XElement gameResXml = Global.GetGameResXml("Config/ShiZhuangRes.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShiZhuangRes");
				for (int i = 0; i < xelementList.Count; i++)
				{
					Global.ShiZhuangResInfo shiZhuangResInfo = new Global.ShiZhuangResInfo();
					shiZhuangResInfo.TouRes = Global.GetXElementAttributeInt(xelementList[i], "TouRes");
					shiZhuangResInfo.XiongRes = Global.GetXElementAttributeInt(xelementList[i], "XiongRes");
					shiZhuangResInfo.ShouRes = Global.GetXElementAttributeInt(xelementList[i], "ShouRes");
					shiZhuangResInfo.TuiRes = Global.GetXElementAttributeInt(xelementList[i], "TuiRes");
					shiZhuangResInfo.XieRes = Global.GetXElementAttributeInt(xelementList[i], "XieRes");
					shiZhuangResInfo.MainOccupation = Global.GetXElementAttributeInt(xelementList[i], "MainOccupation");
					shiZhuangResInfo.GoodsID = Global.GetXElementAttributeInt(xelementList[i], "GoodID");
					if (!Global.shiZhuangDic.ContainsKey(shiZhuangResInfo.GoodsID))
					{
						Dictionary<int, Global.ShiZhuangResInfo> dictionary = new Dictionary<int, Global.ShiZhuangResInfo>();
						dictionary.Add(shiZhuangResInfo.MainOccupation, shiZhuangResInfo);
						Global.shiZhuangDic.Add(shiZhuangResInfo.GoodsID, dictionary);
					}
					else
					{
						Global.shiZhuangDic[shiZhuangResInfo.GoodsID].Add(shiZhuangResInfo.MainOccupation, shiZhuangResInfo);
					}
				}
			}
			return Global.shiZhuangDic;
		}

		public static Dictionary<int, CityInfo> GetCityInfo()
		{
			if (Global.dic_CityInfo.Count > 0)
			{
				return Global.dic_CityInfo;
			}
			string text = "android";
			XElement gameResXml = Global.GetGameResXml("Config/MU_City.xml");
			List<XElement> list = new List<XElement>();
			foreach (XElement xelement in gameResXml.Elements())
			{
				if (xelement.Attribute("TypeID").Value.ToString() == text)
				{
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						list.Add(xelement2);
					}
					break;
				}
			}
			if (gameResXml == null)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(list[i], "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(list[i], "CityLevel");
				string xelementAttributeStr = Global.GetXElementAttributeStr(list[i], "Icon");
				int xelementAttributeInt3 = Global.GetXElementAttributeInt(list[i], "CityNum");
				int xelementAttributeInt4 = Global.GetXElementAttributeInt(list[i], "MaxNum");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(list[i], "BaoMingTime");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(list[i], "AttackWeekDay");
				string xelementAttributeStr4 = Global.GetXElementAttributeStr(list[i], "AttackTime");
				string xelementAttributeStr5 = Global.GetXElementAttributeStr(list[i], "Award");
				string xelementAttributeStr6 = Global.GetXElementAttributeStr(list[i], "DayAward");
				int xelementAttributeInt5 = Global.GetXElementAttributeInt(list[i], "ZhanMengZiJin");
				string xelementAttributeStr7 = Global.GetXElementAttributeStr(list[i], "BaoMingIntro");
				CityInfo cityInfo = new CityInfo();
				cityInfo.ID = xelementAttributeInt;
				cityInfo.CityLevel = xelementAttributeInt2;
				cityInfo.Icon = xelementAttributeStr;
				cityInfo.CityNum = xelementAttributeInt3;
				cityInfo.MaxNum = xelementAttributeInt4;
				cityInfo.BaoMingTime = xelementAttributeStr2;
				cityInfo.AttackWeekDay = xelementAttributeStr3;
				cityInfo.AttackTime = xelementAttributeStr4;
				cityInfo.Award = xelementAttributeStr5;
				cityInfo.DayAward = xelementAttributeStr6;
				cityInfo.ZhanMengZiJin = xelementAttributeInt5;
				cityInfo.BaoMingIntro = xelementAttributeStr7;
				if (!Global.dic_CityInfo.ContainsKey(xelementAttributeInt))
				{
					Global.dic_CityInfo.Add(xelementAttributeInt, cityInfo);
				}
			}
			return Global.dic_CityInfo;
		}

		public static int GetLangHunLingYuClearSecs()
		{
			XElement gameResXml = Global.GetGameResXml("Config/CityWar.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
			return Global.GetXElementAttributeInt(xelementList[0], "ClearRolesSecs");
		}

		public static int GetLangHunLingYuTime()
		{
			XElement gameResXml = Global.GetGameResXml("Config/CityWar.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[0], "FightingSecs");
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[0], "TimePoints");
			DateTime correctDateTime = Global.GetCorrectDateTime();
			string text = correctDateTime.ToShortDateString();
			DateTime dateTime = Convert.ToDateTime(text + " " + xelementAttributeStr).AddSeconds((double)xelementAttributeInt);
			return (int)(dateTime - correctDateTime).TotalSeconds;
		}

		public static bool IsBaoMingTime(int cityLevel)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (cityLevel != 10)
			{
				string[] array = Global.GetCityInfo()[cityLevel].AttackTime.Split(new char[]
				{
					'-'
				});
				string[] array2 = array[0].Split(new char[]
				{
					':'
				});
				string[] array3 = array[1].Split(new char[]
				{
					':'
				});
				long num = (long)(correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second);
				long num2 = (long)((Convert.ToInt32(array2[0]) - 1) * 3600 + 3300);
				long num3 = (long)(Convert.ToInt32(array3[0]) * 3600 + (Convert.ToInt32(array3[1]) + 10) * 60);
				if (num < num2 || num > num3)
				{
					return true;
				}
				if (Global.GetCityInfo()[cityLevel].AttackWeekDay.Split(new char[]
				{
					','
				}).Length <= 0)
				{
					return false;
				}
				for (int i = 0; i < Global.GetCityInfo()[cityLevel].AttackWeekDay.Split(new char[]
				{
					','
				}).Length; i++)
				{
					if (Global.GetCorrectDateTime().DayOfWeek != Global.GetCityInfo()[cityLevel].AttackWeekDay.Split(new char[]
					{
						','
					})[i].SafeToInt32(0))
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				string[] array = Global.GetCityInfo()[cityLevel].BaoMingTime.Split(new char[]
				{
					';'
				});
				string[] array2 = array[0].Split(new char[]
				{
					','
				});
				string[] array3 = array[1].Split(new char[]
				{
					','
				});
				string[] array4 = array[2].Split(new char[]
				{
					','
				});
				if (Convert.ToInt32(correctDateTime.DayOfWeek) == Convert.ToInt32(array2[0]))
				{
					long num4 = (long)(correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second);
					long num5 = (long)(Convert.ToInt32(array2[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[0]) * 3600 + Convert.ToInt32(array2[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[1]) * 60 + Convert.ToInt32(array2[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[2]));
					long num6 = (long)(Convert.ToInt32(array2[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[0]) * 3600 + Convert.ToInt32(array2[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[1]) * 60 + Convert.ToInt32(array2[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[2]));
					return num4 > num5 && num4 < num6;
				}
				if (Convert.ToInt32(correctDateTime.DayOfWeek) == Convert.ToInt32(array3[0]))
				{
					long num4 = (long)(correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second);
					long num5 = (long)(Convert.ToInt32(array3[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[0]) * 3600 + Convert.ToInt32(array3[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[1]) * 60 + Convert.ToInt32(array3[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[2]));
					long num6 = (long)(Convert.ToInt32(array3[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[0]) * 3600 + Convert.ToInt32(array3[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[1]) * 60 + Convert.ToInt32(array3[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[2]));
					return num4 > num5 && num4 < num6;
				}
				if (Convert.ToInt32(correctDateTime.DayOfWeek) == Convert.ToInt32(array4[0]))
				{
					long num4 = (long)(correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second);
					long num5 = (long)(Convert.ToInt32(array4[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[0]) * 3600 + Convert.ToInt32(array4[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[1]) * 60 + Convert.ToInt32(array4[1].Split(new char[]
					{
						'-'
					})[0].Split(new char[]
					{
						':'
					})[2]));
					long num6 = (long)(Convert.ToInt32(array4[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[0]) * 3600 + Convert.ToInt32(array4[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[1]) * 60 + Convert.ToInt32(array4[1].Split(new char[]
					{
						'-'
					})[1].Split(new char[]
					{
						':'
					})[2]));
					return num4 > num5 && num4 < num6;
				}
				return false;
			}
		}

		public static bool IsWarTime(int cityLevel)
		{
			string[] array = Global.GetCityInfo()[cityLevel].AttackWeekDay.Split(new char[]
			{
				','
			});
			string[] array2 = Global.GetCityInfo()[cityLevel].BaoMingIntro.Split(new char[]
			{
				'|'
			});
			int dayOfWeek = Global.GetCorrectDateTime().DayOfWeek;
			if (array.Length != array2.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (dayOfWeek == array[i].SafeToInt32(0))
				{
					break;
				}
				if (i >= array.Length - 1)
				{
					return false;
				}
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			string[] array3 = Global.GetCityInfo()[cityLevel].AttackTime.Split(new char[]
			{
				'-'
			});
			string[] array4 = array3[0].Split(new char[]
			{
				':'
			});
			string[] array5 = array3[1].Split(new char[]
			{
				':'
			});
			long num = (long)(correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second);
			long num2 = (long)(Convert.ToInt32(array4[0]) * 3600 + Convert.ToInt32(array4[1]) * 60);
			long num3 = (long)(Convert.ToInt32(array5[0]) * 3600 + Convert.ToInt32(array5[1]) * 60);
			return num >= num2 && num <= num3;
		}

		public static bool IsDayWarTime(int cityLevel)
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			string[] array = Global.GetCityInfo()[cityLevel].AttackTime.Split(new char[]
			{
				'-'
			});
			string[] array2 = array[0].Split(new char[]
			{
				':'
			});
			string[] array3 = array[1].Split(new char[]
			{
				':'
			});
			long num = (long)(correctDateTime.Hour * 3600 + correctDateTime.Minute * 60 + correctDateTime.Second);
			long num2 = (long)(Convert.ToInt32(array2[0]) * 3600 + Convert.ToInt32(array2[1]) * 60);
			long num3 = (long)(Convert.ToInt32(array3[0]) * 3600 + Convert.ToInt32(array3[1]) * 60);
			return num < num3;
		}

		public static Dictionary<int, Global.LeagueWarData> GetLeagueWarDataDict()
		{
			if (Global.LeagueWarDataDict.Count > 0)
			{
				return Global.LeagueWarDataDict;
			}
			XElement gameResXml = Global.GetGameResXml("Config/LeagueWar.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "LeagueWar");
				for (int i = 0; i < xelementList.Count; i++)
				{
					Global.LeagueWarData leagueWarData = new Global.LeagueWarData();
					leagueWarData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
					leagueWarData.QiZuoID = Global.GetXElementAttributeInt(xelementList[i], "QiZuoID");
					leagueWarData.QiZuoSite = Global.GetXElementAttributeStr(xelementList[i], "QiZuoSite");
					if (!string.IsNullOrEmpty(leagueWarData.QiZuoSite))
					{
						string[] array = leagueWarData.QiZuoSite.Split(new char[]
						{
							'|'
						});
						leagueWarData.PosX = Global.SafeConvertToInt32(array[0]);
						leagueWarData.PosY = Global.SafeConvertToInt32(array[1]);
					}
					if (!Global.LeagueWarDataDict.ContainsKey(leagueWarData.ID))
					{
						Global.LeagueWarDataDict.Add(leagueWarData.QiZuoID, leagueWarData);
					}
				}
			}
			return Global.LeagueWarDataDict;
		}

		public static Vector2 GetZhanMengLianSaiBiSaiQiZiFixedPosition(int cx, int cy)
		{
			Vector2 vector;
			vector..ctor((float)cx, (float)cy);
			Global.LeagueWarData leagueWarData = null;
			foreach (KeyValuePair<int, Global.LeagueWarData> keyValuePair in Global.GetLeagueWarDataDict())
			{
				Global.LeagueWarData value = keyValuePair.Value;
				if (leagueWarData == null)
				{
					leagueWarData = value;
				}
				else
				{
					Vector2 vector2;
					vector2..ctor((float)leagueWarData.PosX, (float)leagueWarData.PosY);
					Vector2 vector3;
					vector3..ctor((float)value.PosX, (float)value.PosY);
					if (Vector2.Distance(vector, vector3) < Vector2.Distance(vector, vector2))
					{
						leagueWarData = value;
					}
				}
			}
			if (leagueWarData != null)
			{
				return new Vector2((float)leagueWarData.PosX, (float)leagueWarData.PosY);
			}
			return vector;
		}

		public static Vector2 GetCompBattleQiZiFixedPosition(int cx, int cy)
		{
			Vector2 vector;
			vector..ctor((float)cx, (float)cy);
			Vector2 vector2 = Vector2.zero;
			if (!Global.IsInShiLiZhengBaBattleMap())
			{
				return vector;
			}
			List<MUForceStronghold> forceStrongholdsByMapCode = ShiLiData.GetAllForceStronghold().GetForceStrongholdsByMapCode(Global.Data.roleData.MapCode);
			for (int i = 0; i < forceStrongholdsByMapCode.Count; i++)
			{
				Vector2 qiZuoSitePosition = forceStrongholdsByMapCode[i].QiZuoSitePosition;
				if (Vector2.Distance(vector, qiZuoSitePosition) < Vector2.Distance(vector, vector2))
				{
					vector2 = qiZuoSitePosition;
				}
			}
			if (vector2.x == 0f && vector2.y == 0f)
			{
				return vector;
			}
			return vector2;
		}

		public static Dictionary<int, Global.MonsterSite> GetDicMonsterSite()
		{
			if (Global.DicMonsterSite.Count > 0)
			{
				return Global.DicMonsterSite;
			}
			XElement gameResXml = Global.GetGameResXml("Config/MonstersSite.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "Monsters");
				for (int i = 0; i < xelementList.Count; i++)
				{
					Global.MonsterSite monsterSite = new Global.MonsterSite();
					monsterSite.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
					monsterSite.PosX = Global.GetXElementAttributeInt(xelementList[i], "PosX");
					monsterSite.PosY = Global.GetXElementAttributeInt(xelementList[i], "PosY");
					monsterSite.FixedDirection = Global.GetXElementAttributeInt(xelementList[i], "FixedDirection");
					if (!Global.DicMonsterSite.ContainsKey(monsterSite.ID))
					{
						Global.DicMonsterSite.Add(monsterSite.ID, monsterSite);
					}
				}
			}
			return Global.DicMonsterSite;
		}

		public static void IsFixedDirectionMonster(int _ExtensionID, out int fixedDirection, out Point fixedPosition, out bool isFixed)
		{
			fixedDirection = 0;
			fixedPosition = new Point(0, 0);
			isFixed = false;
			if (Global.GetDicMonsterSite().ContainsKey(_ExtensionID))
			{
				fixedDirection = Global.GetDicMonsterSite()[_ExtensionID].FixedDirection;
				fixedPosition = new Point(Global.GetDicMonsterSite()[_ExtensionID].PosX, Global.GetDicMonsterSite()[_ExtensionID].PosY);
				isFixed = true;
			}
		}

		public static int GetSoulCometStoneLevel(GoodsData gd, out int exp)
		{
			exp = 0;
			if (gd == null || gd.ElementhrtsProps == null || gd.ElementhrtsProps.Count <= 0)
			{
				return 0;
			}
			if (gd.ElementhrtsProps.Count >= 2)
			{
				exp = gd.ElementhrtsProps[1];
			}
			return gd.ElementhrtsProps[0];
		}

		public static void PushSoulCometStoneToQueue(GoodsData gd)
		{
			if (Global.soulCometStoneGoodsDataQueue == null)
			{
				Global.soulCometStoneGoodsDataQueue = new Queue<GoodsData>();
			}
			if (gd == null)
			{
				return;
			}
			Global.soulCometStoneGoodsDataQueue.Enqueue(gd);
		}

		public static GoodsData PopSoulCometStoneFromQueue()
		{
			if (Global.soulCometStoneGoodsDataQueue == null || Global.soulCometStoneGoodsDataQueue.Count <= 0)
			{
				return null;
			}
			return Global.soulCometStoneGoodsDataQueue.Dequeue();
		}

		public static void ClearSoulCometStoneQueue()
		{
			if (Global.soulCometStoneGoodsDataQueue == null || Global.soulCometStoneGoodsDataQueue.Count <= 0)
			{
				return;
			}
			Global.soulCometStoneGoodsDataQueue.Clear();
		}

		public static bool IsSoulCometStoneSlotID(int slotID)
		{
			int num = slotID / 100;
			int num2 = slotID % 100;
			return num >= 1 && num <= 3 && num2 >= 1 && num2 <= 6;
		}

		public static void OnEquipedSoulCometStoneDataChanged(int dbid, GoodsData gd, EquipedDiamondModifyType type)
		{
			if (dbid <= 0 || gd == null)
			{
				return;
			}
			List<GoodsData> list = Global.GetEquipedSoulCometStoneList();
			if (list == null)
			{
				list = new List<GoodsData>();
			}
			GoodsData goodsData = list.Find((GoodsData g) => g.Id == dbid);
			switch (type)
			{
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Add:
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Replace:
				if (goodsData == null)
				{
					list.Add(gd);
				}
				else
				{
					goodsData.Id = gd.Id;
					goodsData.GoodsID = gd.GoodsID;
					goodsData.BagIndex = gd.BagIndex;
					goodsData.Forge_level = gd.Forge_level;
					goodsData.ElementhrtsProps = gd.ElementhrtsProps;
				}
				break;
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Remove:
			case EquipedDiamondModifyType.EquipedDiamondModifyType_Destroy:
				if (goodsData != null)
				{
					list.Remove(goodsData);
				}
				break;
			}
			Global.SetEquipedSoulCometStoneList(list);
		}

		public static List<GoodsData> GetEquipedSoulCometStoneList()
		{
			if (Global.Data.roleData.soulStoneData == null || Global.Data.roleData.soulStoneData.StonesInUsing == null)
			{
				return null;
			}
			return Global.Data.roleData.soulStoneData.StonesInUsing;
		}

		public static void SetEquipedSoulCometStoneList(List<GoodsData> list_soulCometStone)
		{
			if (Global.Data.roleData.soulStoneData == null)
			{
				Global.Data.roleData.soulStoneData = new SoulStoneData();
			}
			Global.Data.roleData.soulStoneData.StonesInUsing = list_soulCometStone;
		}

		public static GoodsData GetEquipedSoulCometStoneBySlotID(int slotID)
		{
			if (!Global.IsSoulCometStoneSlotID(slotID))
			{
				return null;
			}
			List<GoodsData> equipedSoulCometStoneList = Global.GetEquipedSoulCometStoneList();
			if (equipedSoulCometStoneList == null)
			{
				return null;
			}
			return equipedSoulCometStoneList.Find((GoodsData g) => g.BagIndex == slotID);
		}

		public static GoodsData GetEquipedSoulCometStoneByDBID(int dbid)
		{
			List<GoodsData> equipedSoulCometStoneList = Global.GetEquipedSoulCometStoneList();
			if (equipedSoulCometStoneList == null)
			{
				return null;
			}
			return equipedSoulCometStoneList.Find((GoodsData g) => g.Id == dbid);
		}

		public static void OnGoodsChangedInSoulCometStoneBag(GoodsData gd, BagModifyType type)
		{
			if (gd == null || gd.GoodsID <= 0)
			{
				return;
			}
			List<GoodsData> list = Global.GetBagSoulCometStoneList();
			if (list == null)
			{
				list = new List<GoodsData>();
			}
			GoodsData goodsData = list.Find((GoodsData g) => g.Id == gd.Id);
			switch (type)
			{
			case BagModifyType.BagModifyType_Add:
			case BagModifyType.BagModifyType_Replace:
				if (goodsData == null)
				{
					list.Add(gd);
				}
				else
				{
					goodsData.Id = gd.Id;
					goodsData.GoodsID = gd.GoodsID;
					goodsData.BagIndex = gd.BagIndex;
					goodsData.Forge_level = gd.Forge_level;
					goodsData.ElementhrtsProps = gd.ElementhrtsProps;
				}
				break;
			case BagModifyType.BagModifyType_Remove:
			case BagModifyType.BagModifyType_Destroy:
				if (goodsData != null)
				{
					list.Remove(goodsData);
				}
				break;
			}
			Global.SetBagSoulCometStoneList(list);
		}

		public static List<GoodsData> GetBagSoulCometStoneList()
		{
			if (Global.Data.roleData.soulStoneData == null || Global.Data.roleData.soulStoneData.StonesInBag == null)
			{
				return null;
			}
			return Global.Data.roleData.soulStoneData.StonesInBag;
		}

		public static void SetBagSoulCometStoneList(List<GoodsData> soulCometStoneList)
		{
			if (Global.Data.roleData.soulStoneData == null)
			{
				Global.Data.roleData.soulStoneData = new SoulStoneData();
			}
			Global.Data.roleData.soulStoneData.StonesInBag = soulCometStoneList;
		}

		public static GoodsData GetBagSoulCometStoneGoodsDataByDBID(int dbid)
		{
			List<GoodsData> bagSoulCometStoneList = Global.GetBagSoulCometStoneList();
			if (bagSoulCometStoneList == null || bagSoulCometStoneList.Count <= 0)
			{
				return null;
			}
			return bagSoulCometStoneList.Find((GoodsData g) => g.Id == dbid);
		}

		public static int GetSoulCometStoneBagEmptyGrid()
		{
			List<GoodsData> bagSoulCometStoneList = Global.GetBagSoulCometStoneList();
			if (bagSoulCometStoneList == null || bagSoulCometStoneList.Count <= 0)
			{
				return 100;
			}
			int count = bagSoulCometStoneList.Count;
			int num = 100 - count;
			return (num < 0) ? 0 : num;
		}

		public static Dictionary<int, Dictionary<int, Global.FundSetXml>> GetFundSetXmlDic()
		{
			if (Global.fundSetXmlDic.Count > 0)
			{
				return Global.fundSetXmlDic;
			}
			XElement gameResXml = Global.GetGameResXml("Config/Fund/FundSet.xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "FundSet");
			if (xelementList == null)
			{
				return null;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "PageID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "MainID");
				Global.FundSetXml fundSetXml = new Global.FundSetXml();
				fundSetXml.MinVip = Global.GetXElementAttributeInt(xelementList[i], "MinVip");
				fundSetXml.NextID = Global.GetXElementAttributeInt(xelementList[i], "NextID");
				fundSetXml.Price = Global.GetXElementAttributeInt(xelementList[i], "Price");
				fundSetXml.Tips_1 = Global.GetXElementAttributeStr(xelementList[i], "Tips_1");
				fundSetXml.Tips_2 = Global.GetXElementAttributeStr(xelementList[i], "Tips_2");
				fundSetXml.Tips_3 = Global.GetXElementAttributeStr(xelementList[i], "Tips_3");
				if (!Global.fundSetXmlDic.ContainsKey(xelementAttributeInt))
				{
					Dictionary<int, Global.FundSetXml> dictionary = new Dictionary<int, Global.FundSetXml>();
					dictionary.Add(xelementAttributeInt2, fundSetXml);
					Global.fundSetXmlDic.Add(xelementAttributeInt, dictionary);
				}
				else
				{
					Global.fundSetXmlDic[xelementAttributeInt].Add(xelementAttributeInt2, fundSetXml);
				}
			}
			return Global.fundSetXmlDic;
		}

		public static Dictionary<int, Dictionary<int, Global.FundXml>> GetFundxmlDic()
		{
			if (Global.fundxmlDic.Count > 0)
			{
				return Global.fundxmlDic;
			}
			XElement gameResXml = Global.GetGameResXml("Config/Fund/Fund.xml");
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Fund");
			if (xelementList == null)
			{
				return null;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "MainID");
				Global.FundXml fundXml = new Global.FundXml();
				fundXml.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				fundXml.GoalType = Global.GetXElementAttributeInt(xelementList[i], "GoalType");
				fundXml.RewardType = Global.GetXElementAttributeInt(xelementList[i], "RewardType");
				fundXml.RewardCount = Global.GetXElementAttributeInt(xelementList[i], "RewardCount");
				fundXml.GoalNum = Global.GetXElementAttributeStr(xelementList[i], "GoalNum");
				fundXml.Tips = Global.GetXElementAttributeStr(xelementList[i], "Tips");
				if (!Global.fundxmlDic.ContainsKey(xelementAttributeInt))
				{
					Dictionary<int, Global.FundXml> dictionary = new Dictionary<int, Global.FundXml>();
					dictionary.Add(fundXml.ID, fundXml);
					Global.fundxmlDic.Add(xelementAttributeInt, dictionary);
				}
				else
				{
					Global.fundxmlDic[xelementAttributeInt].Add(fundXml.ID, fundXml);
				}
			}
			return Global.fundxmlDic;
		}

		public static bool IsOpenGrowFund()
		{
			return GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GrowFund);
		}

		public static bool IsFundMergiWindow()
		{
			bool result = true;
			LinkedList<string> topIconBoxDownRightIconIsOpen = HuoDongCommonFlag.GetTopIconBoxDownRightIconIsOpen();
			if (0 < topIconBoxDownRightIconIsOpen.Count)
			{
				string text = Enumerable.ElementAt<string>(topIconBoxDownRightIconIsOpen, 0);
				if (text == "Fund")
				{
					result = false;
				}
			}
			return result;
		}

		public static string GetGoodsType(int category)
		{
			if (category == 11)
			{
				return Global.GetLang("剑");
			}
			if (category == 12)
			{
				return Global.GetLang("斧");
			}
			if (category == 13)
			{
				return Global.GetLang("槌");
			}
			if (category == 14)
			{
				return Global.GetLang("弓");
			}
			if (category == 15)
			{
				return Global.GetLang("弩");
			}
			if (category == 16)
			{
				return Global.GetLang("矛");
			}
			if (category == 17)
			{
				return Global.GetLang("杖");
			}
			if (category == 18)
			{
				return Global.GetLang("盾");
			}
			if (category == 19)
			{
				return Global.GetLang("刀");
			}
			if (category == 20)
			{
				return Global.GetLang("弓箭筒");
			}
			if (category == 21)
			{
				return Global.GetLang("弩箭筒");
			}
			if (category == 0)
			{
				return Global.GetLang("头盔");
			}
			if (category == 1)
			{
				return Global.GetLang("铠甲");
			}
			if (category == 2)
			{
				return Global.GetLang("护手");
			}
			if (category == 3)
			{
				return Global.GetLang("护腿");
			}
			if (category == 4)
			{
				return Global.GetLang("靴子");
			}
			if (category == 6)
			{
				return Global.GetLang("戒指");
			}
			if (category == 7)
			{
				return Global.GetLang("婚戒");
			}
			if (category == 8)
			{
				return Global.GetLang("翅膀");
			}
			if (category == 9)
			{
				return Global.GetLang("精灵");
			}
			if (category == 10)
			{
				return Global.GetLang("精灵");
			}
			if (category == 50)
			{
				return Global.GetLang("任务道具");
			}
			if (category == 60)
			{
				return Global.GetLang("骑宠道具");
			}
			if (category == 70)
			{
				return Global.GetLang("书籍");
			}
			if (category == 80)
			{
				return Global.GetLang("杂物");
			}
			if (category == 90)
			{
				return Global.GetLang("宝石");
			}
			if (category == 100)
			{
				return Global.GetLang("卷轴");
			}
			if (category == 110)
			{
				return Global.GetLang("合成材料");
			}
			if (category == 120)
			{
				return Global.GetLang("消耗材料");
			}
			if (category == 121)
			{
				return Global.GetLang("消耗材料");
			}
			if (category == 180)
			{
				return Global.GetLang("药品");
			}
			if (category == 230)
			{
				return Global.GetLang("果实");
			}
			if (category == 250)
			{
				return Global.GetLang("Buffer类");
			}
			if (category == 301)
			{
				return Global.GetLang("普通包裹");
			}
			if (category == 302)
			{
				return Global.GetLang("升级包裹");
			}
			if (category == 401)
			{
				return Global.GetLang("金币包");
			}
			if (category == 701)
			{
				return Global.GetLang("宝箱");
			}
			if (category == 251)
			{
				return Global.GetLang("经脉");
			}
			if (category == 252)
			{
				return Global.GetLang("武学");
			}
			if (category == 253)
			{
				return Global.GetLang("成就");
			}
			if (category == 601)
			{
				return Global.GetLang("招财符");
			}
			if (category == 255)
			{
				return Global.GetLang("荣耀护体");
			}
			if (category == 256)
			{
				return Global.GetLang("战魂护体");
			}
			if (category == 258)
			{
				return Global.GetLang("战旗护体");
			}
			if (category == 22)
			{
				return Global.GetLang("护符");
			}
			if (category == 5)
			{
				return Global.GetLang("项链");
			}
			if (category == 700)
			{
				return Global.GetLang("翅膀");
			}
			if (category == 702)
			{
				return Global.GetLang("称号");
			}
			if (category == 23)
			{
				return Global.GetLang("饰品");
			}
			if (category == 24)
			{
				return Global.GetLang("时装");
			}
			if (category == 26)
			{
				return Global.GetLang("脚印时装");
			}
			if (category == 25)
			{
				return Global.GetLang("武器时装");
			}
			if (category == 27)
			{
				return Global.GetLang("坐骑时装");
			}
			if (category >= 800 && category <= 815)
			{
				return Global.GetLang("元素之心");
			}
			if (category == 901)
			{
				return Global.GetLang("荧光宝石");
			}
			if (category >= 910 && category <= 928)
			{
				return Global.GetLang("魂石");
			}
			if (category == 263)
			{
				return Global.GetLang("徽记");
			}
			if (category == 330)
			{
				return Global.GetLang("觉醒碎片");
			}
			if (category == 30)
			{
				return Global.GetLang("重生头盔");
			}
			if (category == 31)
			{
				return Global.GetLang("重生铠甲");
			}
			if (category == 32)
			{
				return Global.GetLang("重生护手");
			}
			if (category == 33)
			{
				return Global.GetLang("重生护腿");
			}
			if (category == 34)
			{
				return Global.GetLang("重生靴子");
			}
			if (category == 36)
			{
				return Global.GetLang("重生戒指");
			}
			if (category == 35)
			{
				return Global.GetLang("重生项链");
			}
			if (category == 37)
			{
				return Global.GetLang("圣物");
			}
			if (category == 38)
			{
				return Global.GetLang("圣器");
			}
			return string.Empty;
		}

		public static bool GetJingLingSkillIsOpen()
		{
			string empty = string.Empty;
			return UIHelper.IsGongNengOpenedOrHint(GongNengIDs.JingLingJiNeng, false, out empty);
		}

		public static int GetJIngLingSkillAddAttack(string MagicScripts, int Lev)
		{
			string[] array = MagicScripts.Split(new char[]
			{
				'|'
			});
			if (array[0] == string.Empty)
			{
				return 0;
			}
			int num = array[0].IndexOf('(') + 1;
			int num2 = array[0].IndexOf(')') - num;
			string[] array2 = array[0].Substring(num, num2).Split(new char[]
			{
				','
			});
			double num3 = Convert.ToDouble(array2[1]);
			return (int)(num3 * 100.0) * Lev;
		}

		public static string ClearSpaceOfString(string Content)
		{
			string text = string.Empty;
			for (int i = 0; i < Content.Length; i++)
			{
				if (" " != Content.get_Chars(i).ToString())
				{
					text += Content.get_Chars(i).ToString();
				}
			}
			return text;
		}

		public static string GetJingLingSkillSignURL(int SkillID)
		{
			if (0 >= Global.m_DicSkillURLID.Count)
			{
				if (Global.XElementxml_Magics == null)
				{
					Global.XElementxml_Magics = Global.GetGameResXml("Config/Magics.xml");
				}
				List<XElement> xelementList = Global.GetXElementList(Global.XElementxml_Magics, "Magic");
				for (int i = 0; i < xelementList.Count; i++)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "MagicIcon");
					Global.m_DicSkillURLID.Add(xelementAttributeInt, xelementAttributeInt2);
				}
			}
			if (Global.m_DicSkillURLID.ContainsKey(SkillID))
			{
				return string.Format("NetImages/GameRes/Images/Skill/{0}.png", Global.m_DicSkillURLID[SkillID]);
			}
			return "NoImage";
		}

		public static GameObject LoadTeXiaoObj(string Path, Transform parent)
		{
			Object @object = Resources.Load(Path);
			if (null != @object)
			{
				GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
				U3DUtils.ReplaceLayerInChildren(gameObject, LayerMask.NameToLayer("MUUI"), null);
				gameObject.transform.SetParent(parent, false);
				return gameObject;
			}
			return null;
		}

		public static string GetJingLinfSkillName(int SkillId, bool HaveColor = true)
		{
			if (Global.m_DicJingLingSkillName.ContainsKey(SkillId))
			{
				string[] array = Global.m_DicJingLingSkillName[SkillId].Split(new char[]
				{
					'|'
				});
				if (array.Length == 2)
				{
					return (!HaveColor) ? array[0] : Global.GetColorStringForNGUIText(new object[]
					{
						array[1],
						array[0]
					});
				}
			}
			if (Global.XElementxml_Magics == null)
			{
				Global.XElementxml_Magics = Global.GetGameResXml("Config/Magics.xml");
			}
			List<XElement> xelementList = Global.GetXElementList(Global.XElementxml_Magics, "Magic");
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
				if (xelementAttributeInt == SkillId)
				{
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelementList[i], "Name");
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelementList[i], "MagicColor");
					string text = (!(xelementAttributeStr2 == "1")) ? ((!(xelementAttributeStr2 == "2")) ? "b266ff" : "3681f3") : "17e43e";
					string text2 = string.Format("{0}|{1}", xelementAttributeStr, text);
					Global.m_DicJingLingSkillName.Add(SkillId, text2);
					return (!HaveColor) ? xelementAttributeStr : Global.GetColorStringForNGUIText(new object[]
					{
						text,
						xelementAttributeStr
					});
				}
			}
			return string.Empty;
		}

		public static float GetSkillCDTime(int skillid)
		{
			float result = 0f;
			if (0 >= Global.m_DicJingLingSkillCD.Count)
			{
				if (Global.XElementxml_Magics == null)
				{
					Global.XElementxml_Magics = Global.GetGameResXml("Config/Magics.xml");
				}
				List<XElement> list = Global.GetXElementList(Global.XElementxml_Magics, "Magic").FindAll((XElement s) => s.Attribute("SkillType").Value == "201");
				for (int i = 0; i < list.Count; i++)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(list[i], "ID");
					string xelementAttributeStr = Global.GetXElementAttributeStr(list[i], "MagicScripts");
					string[] array = xelementAttributeStr.Split(new char[]
					{
						'|'
					});
					if (0 < array.Length && 0 < array[0].Length)
					{
						int num = array[0].IndexOf('(') + 1;
						int num2 = array[0].IndexOf(')') - num;
						if (num + num2 <= array[0].Length - 1)
						{
							string[] array2 = array[0].Substring(num, num2).Split(new char[]
							{
								','
							});
							if (array2.Length != 0)
							{
								float num3 = float.Parse(array2[2]);
								Global.m_DicJingLingSkillCD.Add(xelementAttributeInt, num3);
							}
						}
					}
				}
			}
			if (Global.m_DicJingLingSkillCD.ContainsKey(skillid))
			{
				result = Global.m_DicJingLingSkillCD[skillid];
			}
			return result;
		}

		public static int GetSkillMagicColor(int skillid)
		{
			int result = -1;
			if (0 >= Global.m_DicJingLingSkillMagicColor.Count)
			{
				if (Global.XElementxml_Magics == null)
				{
					Global.XElementxml_Magics = Global.GetGameResXml("Config/Magics.xml");
				}
				List<XElement> xelementList = Global.GetXElementList(Global.XElementxml_Magics, "Magic");
				for (int i = 0; i < xelementList.Count; i++)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "ID");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "MagicColor");
					Global.m_DicJingLingSkillMagicColor.Add(xelementAttributeInt, xelementAttributeInt2);
				}
			}
			if (Global.m_DicJingLingSkillMagicColor.ContainsKey(skillid))
			{
				result = Global.m_DicJingLingSkillMagicColor[skillid];
			}
			return result;
		}

		public static int[] GetJingLingRecoverAward(GoodsData gd)
		{
			int[] array = new int[3];
			if (gd != null)
			{
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
				int zhanHunPrice = goodsXmlNodeByID.ZhanHunPrice;
				array[0] = zhanHunPrice;
				XElement gameResXml = Global.GetGameResXml("Config/PetLevelUp.Xml");
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "PetLevelUp");
				for (int i = 0; i < xelementList.Count; i++)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Level");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "NeedEXP");
					if (xelementAttributeInt > gd.Forge_level + 1)
					{
						break;
					}
					if (xelementAttributeInt2 >= 0)
					{
						array[1] += xelementAttributeInt2;
					}
				}
				if (gd.ElementhrtsProps != null)
				{
					Dictionary<int, int> dictionary = new Dictionary<int, int>();
					XElement gameResXml2 = Global.GetGameResXml("Config/PetSkillLevelup.xml");
					if (gameResXml2 != null)
					{
						List<XElement> xelementList2 = Global.GetXElementList(gameResXml2, "Levelup");
						for (int j = 0; j < xelementList2.Count; j++)
						{
							if (xelementList2[j] != null)
							{
								int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelementList2[j], "Level");
								int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelementList2[j], "Cost");
								dictionary.Add(xelementAttributeInt3, xelementAttributeInt4);
							}
						}
					}
					for (int k = 0; k < gd.ElementhrtsProps.Count; k++)
					{
						if (k % 3 == 1)
						{
							int num = gd.ElementhrtsProps[k];
							for (int l = 1; l <= num; l++)
							{
								if (dictionary.ContainsKey(l))
								{
									array[2] += dictionary[l];
								}
							}
						}
					}
				}
			}
			return array;
		}

		public static int[] GetJingLingSRecoverAward(List<GoodsData> gdLst)
		{
			int[] array = new int[3];
			for (int i = 0; i < gdLst.Count; i++)
			{
				int[] jingLingRecoverAward = Global.GetJingLingRecoverAward(gdLst[i]);
				for (int j = 0; j < 3; j++)
				{
					array[j] += jingLingRecoverAward[j];
				}
			}
			return array;
		}

		public static void RefreshUI(GameObject obj)
		{
			UIWidget[] componentsInChildren = obj.GetComponentsInChildren<UIWidget>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
				componentsInChildren[i].enabled = true;
			}
			if (obj.activeSelf)
			{
				obj.SetActive(false);
				obj.SetActive(true);
			}
		}

		public static bool GetRolePatHaveJingling()
		{
			return Global.Data.equipPet != null && 0 < Global.Data.equipPet.Count;
		}

		public static bool DetectionIDBinding()
		{
			return false;
		}

		public static void RemoveFashionListGoodsDataByDbId(int id)
		{
			if (Global.Data.fashionAndTitleList != null && 0 < Global.Data.fashionAndTitleList.Count)
			{
				int i = 0;
				while (i < Global.Data.fashionAndTitleList.Count)
				{
					if (Global.Data.fashionAndTitleList[i] != null)
					{
						if (id == Global.Data.fashionAndTitleList[i].Id)
						{
							Global.Data.fashionAndTitleList.RemoveAt(i);
							break;
						}
						i++;
					}
				}
			}
		}

		public static void CheckRoleGoodsDataList()
		{
			if (Global.Data.roleData.GoodsDataList != null && Global.Data.fashionAndTitleList != null)
			{
				for (int i = Global.Data.roleData.GoodsDataList.Count - 1; i >= 0; i--)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					for (int j = 0; j < Global.Data.fashionAndTitleList.Count; j++)
					{
						if (goodsData.Id == Global.Data.fashionAndTitleList[j].Id)
						{
							Global.Data.roleData.GoodsDataList.Remove(goodsData);
							break;
						}
					}
				}
			}
		}

		public static List<GoodsData> GetRoleChiBangFashionList(bool bClone = true)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.fashionAndTitleList != null)
			{
				for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
				{
					GoodsData goodsData;
					if (bClone)
					{
						goodsData = Global.Data.fashionAndTitleList[i].Clone();
					}
					else
					{
						goodsData = Global.Data.fashionAndTitleList[i];
					}
					if (goodsData != null)
					{
						if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) == 8)
						{
							if (goodsData.Using == 1)
							{
								list.Insert(0, goodsData);
							}
							else
							{
								list.Add(goodsData);
							}
						}
					}
				}
			}
			return list;
		}

		public static List<GoodsData> GetRoleFashionList(ItemCategories cate = ItemCategories.Fashion)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.fashionAndTitleList != null)
			{
				for (int i = 0; i < Global.Data.fashionAndTitleList.Count; i++)
				{
					GoodsData goodsData = Global.Data.fashionAndTitleList[i];
					if (goodsData != null)
					{
						if (cate == (ItemCategories)Global.GetCategoriyByGoodsID(goodsData.GoodsID))
						{
							if (goodsData.Using == 1)
							{
								list.Insert(0, goodsData);
							}
							else
							{
								list.Add(goodsData);
							}
						}
					}
				}
			}
			return list;
		}

		public static List<GoodsData> GetRoleHorseFashionList(byte Clone = 0)
		{
			return Global.GetRoleFashionList(ItemCategories.ShiZhuang_ZuoQi);
		}

		public static int GetRoleGoodsNumberCountByGoodsID(int GoodsId)
		{
			int num = 0;
			if (Global.IsRebornGood(ConfigGoods.GetGoodsXmlNodeByID(GoodsId)) || Global.IsRebornEquip(Global.GetCategoriyByGoodsID(GoodsId)))
			{
				if (Global.Data.roleData != null && Global.Data.roleData.RebornGoodsDataList != null)
				{
					for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
					{
						GoodsData goodsData = Global.Data.roleData.RebornGoodsDataList[i];
						if (GoodsId == goodsData.GoodsID)
						{
							if (goodsData.Using != 1)
							{
								num += Global.Data.roleData.RebornGoodsDataList[i].GCount;
							}
						}
					}
				}
			}
			else if (Global.Data.roleData != null && Global.Data.roleData.GoodsDataList != null)
			{
				for (int j = 0; j < Global.Data.roleData.GoodsDataList.Count; j++)
				{
					GoodsData goodsData2 = Global.Data.roleData.GoodsDataList[j];
					if (GoodsId == goodsData2.GoodsID)
					{
						if (goodsData2.Using != 1)
						{
							num += Global.Data.roleData.GoodsDataList[j].GCount;
						}
					}
				}
			}
			return num;
		}

		public static Dictionary<int, Global.MonsterSite> WangZheZhanChangQiZuoDic
		{
			get
			{
				if (Global._WangZheZhanChangQiZuoDic.Count > 0)
				{
					return Global._WangZheZhanChangQiZuoDic;
				}
				XElement gameResXml = Global.GetGameResXml("Config/KingOfBattleQiZuo.xml");
				if (gameResXml != null)
				{
					List<XElement> xelementList = Global.GetXElementList(gameResXml, "Item");
					for (int i = 0; i < xelementList.Count; i++)
					{
						Global.MonsterSite monsterSite = new Global.MonsterSite();
						monsterSite.ID = Global.GetXElementAttributeInt(xelementList[i], "NPCID");
						monsterSite.PosX = Global.GetXElementAttributeInt(xelementList[i], "PosX");
						monsterSite.PosY = Global.GetXElementAttributeInt(xelementList[i], "PosY");
						monsterSite.FixedDirection = 4;
						if (!Global._WangZheZhanChangQiZuoDic.ContainsKey(monsterSite.ID))
						{
							Global._WangZheZhanChangQiZuoDic.Add(monsterSite.ID, monsterSite);
						}
					}
				}
				return Global._WangZheZhanChangQiZuoDic;
			}
		}

		public static int GetWangZheQiZhiNPCID(int _ExtensionID, int cx, int cy)
		{
			Vector2 vector;
			vector..ctor((float)cx, (float)cy);
			Global.MonsterSite monsterSite = null;
			if (Global.IsWangZheZhanChangQi(_ExtensionID))
			{
				foreach (KeyValuePair<int, Global.MonsterSite> keyValuePair in Global.WangZheZhanChangQiZuoDic)
				{
					Global.MonsterSite value = keyValuePair.Value;
					if (monsterSite == null)
					{
						monsterSite = value;
					}
					else
					{
						Vector2 vector2;
						vector2..ctor((float)monsterSite.PosX, (float)monsterSite.PosY);
						Vector2 vector3;
						vector3..ctor((float)value.PosX, (float)value.PosY);
						if (Vector2.Distance(vector, vector3) < Vector2.Distance(vector, vector2))
						{
							monsterSite = value;
						}
					}
				}
			}
			if (monsterSite != null)
			{
				return monsterSite.ID;
			}
			return 0;
		}

		public static void IsFixedDirectionMonsterInWangZheZhanChang(int _ExtensionID, int cx, int cy, out int fixedDirection, out Point fixedPosition, out bool isFixed)
		{
			fixedDirection = 0;
			fixedPosition = new Point(0, 0);
			isFixed = false;
			int wangZheQiZhiNPCID = Global.GetWangZheQiZhiNPCID(_ExtensionID, cx, cy);
			if (wangZheQiZhiNPCID == 0)
			{
				return;
			}
			if (Global.WangZheZhanChangQiZuoDic.ContainsKey(wangZheQiZhiNPCID))
			{
				fixedDirection = Global.WangZheZhanChangQiZuoDic[wangZheQiZhiNPCID].FixedDirection;
				fixedPosition = new Point(Global.WangZheZhanChangQiZuoDic[wangZheQiZhiNPCID].PosX, Global.WangZheZhanChangQiZuoDic[wangZheQiZhiNPCID].PosY);
				isFixed = true;
			}
		}

		public static bool IsWangZheZhanChangQiZuo(int _ExtensionID)
		{
			return Global.WangZheZhanChangQiZuoDic.ContainsKey(_ExtensionID);
		}

		public static bool IsWangZheZhanChangQi(int _ExtensionID)
		{
			return _ExtensionID == 8800003 || _ExtensionID == 8800004;
		}

		public static bool isFanbei(int type)
		{
			JieRiFanBeiItemData jieRiFanBeiItemData = null;
			bool result = false;
			if (Global.JieriFanbeiInfo != null && Global.JieriFanbeiInfo.TryGetValue(type, ref jieRiFanBeiItemData))
			{
				result = (jieRiFanBeiItemData.IsOpen == 1);
			}
			return result;
		}

		public static GoodsData GetEmptyGoodsData(int goodsID, int forgeLevel, int quality, int binding, int gcount, int addPropIndex, int bornIndex, int lucky, int strong)
		{
			return new GoodsData
			{
				Id = 0,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = "1900-01-01 12:00:00",
				Site = 0,
				Quality = quality,
				Props = string.Empty,
				GCount = gcount,
				Binding = binding,
				Jewellist = string.Empty,
				BagIndex = 0,
				SaleMoney1 = 0,
				SaleYuanBao = 0,
				SaleYinPiao = 0,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong
			};
		}

		public static void ShowHelpPart(string content0, string content1, bool bDrag = false, float TitleYAddValue = 0f)
		{
			GChildWindow gw = U3DUtils.NEW<GChildWindow>();
			gw.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(gw, "HelpPart");
			Super.GData.PlayZoneRoot.Children.Add(gw);
			HelpPart help = U3DUtils.NEW<HelpPart>();
			gw.Body.Add(help);
			help.SetContent(content0, content1, TitleYAddValue);
			if (bDrag)
			{
				UIDraggablePanel componentInChildren = help.transform.GetComponentInChildren<UIDraggablePanel>();
				if (null != componentInChildren)
				{
					componentInChildren.Press(false);
				}
			}
			UIEventListener.Get(gw.ModalBak).onClick = delegate(GameObject go)
			{
				Object.Destroy(help);
				Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, gw);
			};
			gw.ChildWindowClose = delegate(object s, EventArgs e)
			{
				Object.Destroy(help);
				Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, gw);
				return true;
			};
			help.ClseoHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				Object.Destroy(help);
				Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, gw);
			};
		}

		public static void ShowProPerty(int type, string[] Content, DPSelectedItemEventHandler hander = null)
		{
			GChildWindow gw = U3DUtils.NEW<GChildWindow>();
			gw.ModalType = ChildWindowModalType.Translucent;
			gw.IsShowModal = true;
			Super.InitChildWindow(gw, "ProPerty");
			Super.GData.PlayZoneRoot.Children.Add(gw);
			PropertyPart property = U3DUtils.NEW<PropertyPart>();
			if (type == 0)
			{
				if (Content != null && 2 <= Content.Length)
				{
					property.SetTYpe(0);
					property.SetFashionPropertyInf(Content[0], Content[1]);
				}
			}
			else if (type == 1)
			{
				property.SetTYpe(1);
				property.SetPropertyInf(Content[0], Content[1]);
			}
			UIEventListener.Get(gw.ModalBak).onClick = delegate(GameObject go)
			{
				Object.Destroy(property);
				Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, gw);
			};
			gw.ChildWindowClose = delegate(object s, EventArgs e)
			{
				Object.Destroy(property);
				Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, gw);
				return true;
			};
			property.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				Object.Destroy(property);
				Super.CloseChildWindow(Super.GData.PlayZoneRoot.Children, gw);
			};
			gw.Body.Add(property);
		}

		public static void InitVioceToWordFilterFields()
		{
			try
			{
				if (Global.VioceToWordFilterFields == null)
				{
					string gameResTxt = Global.GetGameResTxt(string.Format("CiKu", new object[0]));
					if (gameResTxt != null)
					{
						Global.VioceToWordFilterFields = gameResTxt.Split(new char[]
						{
							'；'
						});
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static string ReplaceVioceToWordFilterFileds(string text)
		{
			text = Global.ReplaceFilterFileds(text);
			text = Global.ReplaceVioceToWordFuHao(text);
			Global.InitVioceToWordFilterFields();
			if (text.Length > 43)
			{
				text = text.Substring(0, 44);
				text += "...";
			}
			return text;
		}

		public static string ReplaceVioceToWordFuHao(string str)
		{
			string text = Global.StringReplaceAll(str, "#", Global.GetLang("井号"));
			text = Global.StringReplaceAll(text, "@", Global.GetLang("艾特"));
			return text.ToString();
		}

		public static List<GoodsData> GetRoleDecorationList()
		{
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.DecorationList != null && 0 < Global.Data.DecorationList.Count)
			{
				list.AddRange(Global.Data.DecorationList);
			}
			return list;
		}

		public static bool GetDecorationHaveActive(int GoodsID)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(GoodsID);
			if (goodsXmlNodeByID.Categoriy == 23)
			{
				List<GoodsData> roleDecorationList = Global.GetRoleDecorationList();
				GoodsData goodsData = roleDecorationList.Find((GoodsData e) => GoodsID == e.GoodsID);
				if (goodsData != null)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetDecorationSaleToNpcPrice(int GoodsID)
		{
			int result = 0;
			XElement gameResXml = Global.GetGameResXml("Config/Ornament.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "Ornament");
				XElement xelement = xelementList.Find((XElement e) => Global.GetXElementAttributeInt(e, "GoodsID") == GoodsID);
				if (xelement != null)
				{
					result = Global.GetXElementAttributeInt(xelement, "Recover");
				}
			}
			return result;
		}

		public static void RoleDecorationDataInit(List<GoodsData> data)
		{
			if (data != null)
			{
				if (0 < data.Count && Global.Data.DecorationList != null)
				{
					Global.Data.DecorationList.Clear();
				}
				Global.Data.DecorationList = data;
			}
		}

		public static void RoleDecorationDataChange(SCModGoods data)
		{
			if (Global.Data.DecorationList != null && Global.Data.DecorationList.Count > 0)
			{
				GoodsData goodsData = Global.Data.DecorationList.Find((GoodsData e) => e.Id == data.ID);
				if (goodsData != null)
				{
					goodsData.BagIndex = data.BagIndex;
					goodsData.GCount = data.Count;
					goodsData.Using = data.IsUsing;
				}
			}
		}

		public static bool DialyTaskAlertIsUsing
		{
			get
			{
				return Global.m_DialyTaskAlertIsUsing;
			}
			set
			{
				Global.m_DialyTaskAlertIsUsing = value;
			}
		}

		public static bool IsGoToKuaFuMap(int mapCode)
		{
			return Global.IsKuaFuMap(mapCode, false) && Global.Data.roleData.MapCode != mapCode;
		}

		public static bool IsKuaFuMap(int mapCode, bool IsAllKuaFuMap = false)
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
			return (settingMapVOByCode != null && settingMapVOByCode.MapType == 32) || (IsAllKuaFuMap && ((settingMapVOByCode != null && settingMapVOByCode.MapType == 48) || SceneUIClasses.RebornMap.IsTheScene() || Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass())));
		}

		public static void AddRoleNameColor(int RoleID, Color color)
		{
			if (Global.Data.RoleNameColor == null)
			{
				Global.Data.RoleNameColor = new Dictionary<int, Color>();
			}
			if (Global.Data.RoleNameColor.ContainsKey(RoleID))
			{
				Global.Data.RoleNameColor[RoleID] = color;
			}
			else
			{
				Global.Data.RoleNameColor.Add(RoleID, color);
			}
			if (100 < Global.Data.RoleNameColor.Count)
			{
				List<int> list = new List<int>();
				Dictionary<int, Color>.Enumerator enumerator = Global.Data.RoleNameColor.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Dictionary<int, RoleData> otherRoles = Global.Data.OtherRoles;
					KeyValuePair<int, Color> keyValuePair = enumerator.Current;
					if (!otherRoles.ContainsKey(keyValuePair.Key))
					{
						List<int> list2 = list;
						KeyValuePair<int, Color> keyValuePair2 = enumerator.Current;
						list2.Add(keyValuePair2.Key);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					Global.RemoveRoleNameColor(list[i]);
				}
			}
		}

		public static void RemoveRoleNameColor(int RoleID)
		{
			if (0 < Global.Data.RoleNameColor.Count && Global.Data.RoleNameColor.ContainsKey(RoleID))
			{
				Global.Data.RoleNameColor.Remove(RoleID);
			}
		}

		public static Color GetRoleNameColor(int RoleID)
		{
			Color white = Color.white;
			Global.Data.RoleNameColor.TryGetValue(RoleID, ref white);
			return white;
		}

		public static bool IsCanMove
		{
			get
			{
				if (!Global._IsCanMove)
				{
					if (Super.roleCreator != null && Super.roleCreator.Visibility)
					{
						Global._IsCanMove = false;
					}
					else
					{
						Global._IsCanMove = true;
					}
				}
				return Global._IsCanMove;
			}
			set
			{
				Global._IsCanMove = value;
			}
		}

		public static string GetNguiStrColor(GoodsQuality gq)
		{
			string result = "f0f0f0";
			if (gq == GoodsQuality.Green)
			{
				result = "17e43e";
			}
			else if (gq == GoodsQuality.Blue)
			{
				result = "3681f3";
			}
			else if (gq == GoodsQuality.Purple || gq == GoodsQuality.FlashPurple)
			{
				result = "b266ff";
			}
			else if (gq == GoodsQuality.Gold)
			{
				result = "ffcc19";
			}
			else if (gq == GoodsQuality.Orange)
			{
				result = "ffcc19";
			}
			return result;
		}

		public static string GetTimeStrBySecNoWord(double sec)
		{
			int num = 86400;
			int num2 = 3600;
			int num3 = 60;
			string result = "0:0:0";
			if (num2 != 0 && num3 != 0 && num != 0)
			{
				result = StringUtil.substitute("{0}:{1}:{2}", new object[]
				{
					((int)(sec / (double)num2) <= 9) ? ("0" + ((int)(sec / (double)num2)).ToString()) : ((int)(sec / (double)num2)).ToString(),
					((int)(sec % (double)num % (double)num2 / (double)num3) <= 9) ? ("0" + ((int)(sec % (double)num % (double)num2 / (double)num3)).ToString()) : ((int)(sec % (double)num % (double)num2 / (double)num3)).ToString(),
					((int)(sec % (double)num % (double)num2 % (double)num3) <= 9) ? ("0" + ((int)(sec % (double)num % (double)num2 % (double)num3)).ToString()) : ((int)(sec % (double)num % (double)num2 % (double)num3)).ToString()
				});
			}
			return result;
		}

		public static bool RoleHaveArmyGroup()
		{
			return 0 < Global.Data.roleData.JunTuanId && !string.IsNullOrEmpty(Global.Data.roleData.JunTuanName) && Global.IsHavingBangHui();
		}

		public static bool RoleHaveComp()
		{
			return 0 < Global.Data.roleData.CompType;
		}

		public static bool CheckWingFashionData(List<GoodsData> lst, out GoodsData wingsGoodsData, out string emptyName)
		{
			byte b = 0;
			emptyName = null;
			wingsGoodsData = null;
			if (lst != null && 0 < lst.Count)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					if (Global.GetCategoriyByGoodsID(lst[i].GoodsID) == 8 && lst[i].Using == 1)
					{
						wingsGoodsData = lst[i].Clone();
						b = 1;
						break;
					}
				}
			}
			if (b != 0)
			{
				Global.ParseWingsGoodsDataInfo(out emptyName);
				return true;
			}
			return false;
		}

		public static List<SpecialTitle> TeShuTitleListXml
		{
			get
			{
				if (Global._TeShuTitleListXml == null)
				{
					Global._InitSpecialTitleConfig();
				}
				return Global._TeShuTitleListXml;
			}
			set
			{
				Global._TeShuTitleListXml = value;
			}
		}

		private static void _InitSpecialTitleConfig()
		{
			Global._TeShuTitleListXml = new List<SpecialTitle>();
			XElement gameResXml = Global.GetGameResXml("Config/SpecialTitle.xml");
			if (gameResXml == null)
			{
				return;
			}
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "Config"), "LegionTasks");
			for (int i = 0; i < xelementList.Count; i++)
			{
				SpecialTitle specialTitle = new SpecialTitle();
				specialTitle.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				specialTitle.Title = Global.GetXElementAttributeStr(xelementList[i], "Title");
				specialTitle.Describtion = Global.GetXElementAttributeStr(xelementList[i], "Describtion");
				specialTitle.IconCode = Global.GetXElementAttributeStr(xelementList[i], "IconCode");
				specialTitle.BuffID = Global.GetXElementAttributeInt(xelementList[i], "BuffID");
				specialTitle.IconCode = specialTitle.IconCode.Replace(".png", string.Empty);
				specialTitle.GoodsID = Global.GetXElementAttributeInt(xelementList[i], "GoodsID");
				specialTitle.KaiGuan = Global.GetXElementAttributeInt(xelementList[i], "KaiGuan");
				Global._TeShuTitleListXml.Add(specialTitle);
			}
		}

		public static bool IsSpecialTitle(int buffId)
		{
			return Global.TeShuTitleListXml.Find((SpecialTitle s) => s.BuffID == buffId) != null;
		}

		public static int GetSpecialTitleBuffGoodsID(int buffId)
		{
			SpecialTitle specialTitle = Global.TeShuTitleListXml.Find((SpecialTitle s) => s.BuffID == buffId);
			if (specialTitle != null)
			{
				return specialTitle.GoodsID;
			}
			return -1;
		}

		public static bool IsShowSpecialTitleBuff(int buffId)
		{
			SpecialTitle specialTitle = Global.TeShuTitleListXml.Find((SpecialTitle s) => s.BuffID == buffId);
			return specialTitle != null && specialTitle.KaiGuan == 1;
		}

		public static List<GoodsData> GetRolePaiPets(bool bClone = false)
		{
			if (Global.Data.PaiZhuPetList == null)
			{
				return new List<GoodsData>();
			}
			if (bClone)
			{
				List<GoodsData> list = new List<GoodsData>();
				for (int i = 0; i < Global.Data.PaiZhuPetList.Count; i++)
				{
					if (Global.Data.PaiZhuPetList[i].Site == 10001 || Global.Data.PaiZhuPetList[i].Site == 10000)
					{
						list.Add(Global.Data.PaiZhuPetList[i].Clone());
					}
				}
				return list;
			}
			return Global.Data.PaiZhuPetList;
		}

		public static void PaiZhuListAddData(GoodsData data)
		{
			GoodsData jingLingYaoSaiGoodsDataByDbID = Global.GetJingLingYaoSaiGoodsDataByDbID(data.Id, 1);
			if (jingLingYaoSaiGoodsDataByDbID == null)
			{
				Global.Data.PaiZhuPetList.Add(data);
			}
			else
			{
				jingLingYaoSaiGoodsDataByDbID.Forge_level = data.Forge_level;
				jingLingYaoSaiGoodsDataByDbID.Site = data.Site;
			}
		}

		public static void RemoveJingLingPaiZhuData(int DBID)
		{
			List<GoodsData> rolePaiPets = Global.GetRolePaiPets(false);
			if (0 < rolePaiPets.Count)
			{
				GoodsData goodsData = rolePaiPets.Find((GoodsData e) => e.Id == DBID);
				if (goodsData != null)
				{
					rolePaiPets.Remove(goodsData);
				}
			}
		}

		public static GoodsData GetJingLingYaoSaiGoodsDataByDbID(int id, byte bClone = 0)
		{
			List<GoodsData> rolePaiPets = Global.GetRolePaiPets(0 == bClone);
			if (0 < rolePaiPets.Count)
			{
				for (int i = 0; i < rolePaiPets.Count; i++)
				{
					if (id == rolePaiPets[i].Id)
					{
						return rolePaiPets[i];
					}
				}
			}
			return null;
		}

		public static GoodsData GetRoleHorseGoodsDataInHorseCangKuByDbId(int RoleID, int id, byte bClone = 0)
		{
			RoleData roleData;
			if (RoleID == Global.Data.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else
			{
				roleData = Global.FindRoleDataByID(RoleID);
			}
			if (roleData != null && roleData.MountStoreList != null)
			{
				GoodsData goodsData = roleData.MountStoreList.Find((GoodsData e) => id == e.Id);
				if (goodsData != null)
				{
					return (bClone != 0) ? goodsData : goodsData.Clone();
				}
			}
			return null;
		}

		public static GoodsData GetRoleHorseGoodsDataInBeiZhanBaoGuoByDbId(int RoleID, int id, byte bClone = 1)
		{
			RoleData roleData;
			if (RoleID == Global.Data.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else
			{
				roleData = Global.FindRoleDataByID(RoleID);
			}
			if (roleData != null && roleData.MountEquipList != null)
			{
				GoodsData goodsData = roleData.MountEquipList.Find((GoodsData e) => id == e.Id);
				if (goodsData != null)
				{
					return (bClone != 0) ? goodsData : goodsData.Clone();
				}
			}
			return null;
		}

		public static int GetHorsePrice(GoodsData goodsData, out byte huiShouType)
		{
			int num = 0;
			huiShouType = 0;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (goodsXmlNodeByID != null)
			{
				if (goodsXmlNodeByID.Categoriy == 340)
				{
					HorseAdvancedVO horseAdvancedVOByID = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(goodsData.GoodsID, goodsData.Forge_level + 1);
					if (horseAdvancedVOByID != null)
					{
						num += horseAdvancedVOByID.ChangeHunJing;
						huiShouType = 1;
					}
				}
				else if (40 <= goodsXmlNodeByID.Categoriy && 45 >= goodsXmlNodeByID.Categoriy)
				{
					num += ((Global.GetGoodsSaleToNpJiFen(goodsData) != 1) ? Global.GetGoodsSaleToNpJiFen(goodsData) : 0);
					huiShouType = 0;
				}
				if (0 < goodsXmlNodeByID.ChangeHunJing)
				{
					num += goodsXmlNodeByID.ChangeHunJing;
					huiShouType = 1;
				}
			}
			return num;
		}

		internal static bool IsInJingLingMap()
		{
			return JingLingMap.inst.mapmini != null && JingLingMap.inst.mapmini.gameObject.activeSelf;
		}

		public static void QuiteZhanMengDeleteZhanMengHongBaoIcon()
		{
			if (Global.Data == null)
			{
				return;
			}
			ZhanMengHongBaoTipIcon zhanMengHongBaoTipIcon = null;
			if (PlayZone.GlobalPlayZone != null)
			{
				zhanMengHongBaoTipIcon = PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon;
			}
			if (zhanMengHongBaoTipIcon == null)
			{
				return;
			}
			Global.Data.mZhanMengHongBaoTipsData.Clear();
			if (zhanMengHongBaoTipIcon.gameObject.activeSelf)
			{
				zhanMengHongBaoTipIcon.NotifyInitUIDataByServerData();
				zhanMengHongBaoTipIcon.gameObject.SetActive(false);
			}
		}

		public static void RefreshZhanMengHongBaoTipIconNum(int hongBaoId)
		{
			if (Global.Data == null)
			{
				return;
			}
			List<HongBaoTipData> mZhanMengHongBaoTipsData = Global.Data.mZhanMengHongBaoTipsData;
			if (mZhanMengHongBaoTipsData == null || mZhanMengHongBaoTipsData.Count <= 0)
			{
				return;
			}
			HongBaoTipData hongBaoTipData = mZhanMengHongBaoTipsData.Find((HongBaoTipData d) => d.hongBaoID == hongBaoId);
			if (hongBaoTipData == null)
			{
				return;
			}
			mZhanMengHongBaoTipsData.Remove(hongBaoTipData);
			ZhanMengHongBaoTipIcon zhanMengHongBaoTipIcon = null;
			if (PlayZone.GlobalPlayZone != null)
			{
				zhanMengHongBaoTipIcon = PlayZone.GlobalPlayZone.GameHintZhanMengHongBaoTipIcon;
			}
			if (zhanMengHongBaoTipIcon == null)
			{
				return;
			}
			if (mZhanMengHongBaoTipsData.Count > 0)
			{
				if (!zhanMengHongBaoTipIcon.gameObject.activeSelf)
				{
					zhanMengHongBaoTipIcon.gameObject.SetActive(true);
				}
				zhanMengHongBaoTipIcon.NotifyInitUIDataByServerData();
			}
			else
			{
				zhanMengHongBaoTipIcon.gameObject.SetActive(false);
			}
		}

		public static void RefreshSystemHongBaoTipIconNum(int type, int hongBaoId)
		{
			if (Global.Data == null)
			{
				return;
			}
			Dictionary<int, List<HongBaoTipData>> mSystemHongBaoTipsDataDict = Global.Data.mSystemHongBaoTipsDataDict;
			if (mSystemHongBaoTipsDataDict == null || mSystemHongBaoTipsDataDict.Count <= 0)
			{
				return;
			}
			List<HongBaoTipData> list = null;
			if (mSystemHongBaoTipsDataDict.ContainsKey(type))
			{
				list = mSystemHongBaoTipsDataDict[type];
			}
			if (list == null || list.Count <= 0)
			{
				return;
			}
			HongBaoTipData hongBaoTipData = list.Find((HongBaoTipData d) => d.hongBaoID == hongBaoId);
			if (hongBaoTipData == null)
			{
				return;
			}
			list.Remove(hongBaoTipData);
			SystemHongBaoTipIcon systemHongBaoTipIcon = null;
			if (PlayZone.GlobalPlayZone != null)
			{
				systemHongBaoTipIcon = PlayZone.GlobalPlayZone.GameHintSystemHongBaoTipIcon;
			}
			if (systemHongBaoTipIcon == null)
			{
				return;
			}
			if (list.Count > 0)
			{
				if (!systemHongBaoTipIcon.gameObject.activeSelf)
				{
					systemHongBaoTipIcon.gameObject.SetActive(true);
				}
				systemHongBaoTipIcon.NotifyInitUIDataByServerData();
			}
			else
			{
				systemHongBaoTipIcon.NotifyInitUIDataByServerData();
			}
		}

		public static int GetHuiJiGoodsId(int level)
		{
			int result = 8043;
			if (Global.m_DicUp == null || Global.m_DicUp.Count <= 0)
			{
				Global.m_DicUp = Global.AddDicEmblemUp();
			}
			if (Global.m_DicUp.ContainsKey(level))
			{
				result = Global.m_DicUp[level].EmblemIcon.SafeToInt32(0);
			}
			return result;
		}

		public static int SetEmbelemUpLevel
		{
			get
			{
				if (Global.m_DicStar == null || Global.m_DicStar.Count <= 0)
				{
					Global.m_DicStar = Global.AddDicEmblemStart();
				}
				if (Global.m_DicStar.ContainsKey(Global.Data.roleData.HuiJiData.huiji))
				{
					return Global.m_DicStar[Global.Data.roleData.HuiJiData.huiji].EmblemLevel;
				}
				return -1;
			}
		}

		public static int SetEmbelemStarLevel
		{
			get
			{
				if (Global.m_DicStar == null || Global.m_DicStar.Count <= 0)
				{
					Global.m_DicStar = Global.AddDicEmblemStart();
				}
				if (Global.m_DicStar.ContainsKey(Global.Data.roleData.HuiJiData.huiji))
				{
					return Global.m_DicStar[Global.Data.roleData.HuiJiData.huiji].EmblemStar;
				}
				return -1;
			}
		}

		public static EmblemUp SetEmbelemUpData(int roleId = -1)
		{
			if (Global.m_DicUp == null || Global.m_DicUp.Count <= 0)
			{
				Global.m_DicUp = Global.AddDicEmblemUp();
			}
			if (roleId == -1 || roleId == Global.Data.RoleID)
			{
				if (Global.m_DicUp.ContainsKey(Global.SetEmbelemUpLevel))
				{
					return Global.m_DicUp[Global.SetEmbelemUpLevel];
				}
			}
			else if (Global.Data.OtherRoles.ContainsKey(roleId))
			{
				if (Global.m_DicStar == null || Global.m_DicStar.Count <= 0)
				{
					Global.m_DicStar = Global.AddDicEmblemStart();
				}
				int emblemLevel = Global.m_DicStar[Global.Data.OtherRoles[roleId].HuiJiData.huiji].EmblemLevel;
				if (Global.m_DicUp.ContainsKey(emblemLevel))
				{
					return Global.m_DicUp[emblemLevel];
				}
			}
			return null;
		}

		public static string SetHuiJiTeXiao(int code)
		{
			DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(code);
			if (decorationVOByCode != null)
			{
				return decorationVOByCode.ResName;
			}
			return string.Empty;
		}

		public static Dictionary<int, EmblemUp> AddDicEmblemUp()
		{
			Dictionary<int, EmblemUp> dictionary = new Dictionary<int, EmblemUp>();
			XElement gameResXml = Global.GetGameResXml("Config/EmblemUp.xml");
			if (gameResXml == null)
			{
				return dictionary;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "EmblemUp");
			if (xelementList == null)
			{
				return dictionary;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				EmblemUp emblemUp = new EmblemUp();
				emblemUp.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				emblemUp.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
				emblemUp.EmblemLevel = Global.GetXElementAttributeInt(xelementList[i], "EmblemLevel");
				emblemUp.ModID = Global.GetXElementAttributeInt(xelementList[i], "ModID");
				emblemUp.EmblemIcon = Global.GetXElementAttributeStr(xelementList[i], "EmblemIcon");
				emblemUp.LuckyOne = Global.GetXElementAttributeInt(xelementList[i], "LuckyOne");
				emblemUp.LuckyTwo = Global.GetXElementAttributeInt(xelementList[i], "LuckyTwo");
				emblemUp.LuckyTwoRate = Global.GetXElementAttributeStr(xelementList[i], "LuckyTwoRate");
				emblemUp.Instructions = Global.GetXElementAttributeStr(xelementList[i], "Instructions");
				emblemUp.DurationTime = Global.GetXElementAttributeFloat(xelementList[i], "DurationTime");
				emblemUp.CDTime = Global.GetXElementAttributeFloat(xelementList[i], "CDTime");
				emblemUp.SubAttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "SubAttackInjurePercent");
				emblemUp.SPAttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "SPAttackInjurePercent");
				emblemUp.AttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "AttackInjurePercent");
				emblemUp.ElementAttackInjurePercent = Global.GetXElementAttributeFloat(xelementList[i], "ElementAttackInjurePercent");
				emblemUp.LifeV = Global.GetXElementAttributeInt(xelementList[i], "LifeV");
				emblemUp.AddAttack = Global.GetXElementAttributeInt(xelementList[i], "AddAttack");
				emblemUp.AddDefense = Global.GetXElementAttributeInt(xelementList[i], "AddDefense");
				emblemUp.DecreaseInjureValue = Global.GetXElementAttributeInt(xelementList[i], "DecreaseInjureValue");
				emblemUp.NeedGoods = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
				emblemUp.NeedDiamond = Global.GetXElementAttributeInt(xelementList[i], "NeedDiamond");
				emblemUp.Decorations = Global.GetXElementAttributeStr(xelementList[i], "Decorations");
				if (!dictionary.ContainsKey(emblemUp.ID))
				{
					dictionary.Add(emblemUp.ID, emblemUp);
				}
				else
				{
					dictionary[emblemUp.ID] = emblemUp;
				}
			}
			return dictionary;
		}

		public static Dictionary<int, EmblemStarXml> AddDicEmblemStart()
		{
			Dictionary<int, EmblemStarXml> dictionary = new Dictionary<int, EmblemStarXml>();
			XElement gameResXml = Global.GetGameResXml("Config/EmblemStar.xml");
			if (gameResXml == null)
			{
				return dictionary;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "EmblemStar");
			if (xelementList == null)
			{
				return dictionary;
			}
			for (int i = 0; i < xelementList.Count; i++)
			{
				EmblemStarXml emblemStarXml = new EmblemStarXml();
				emblemStarXml.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
				emblemStarXml.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
				emblemStarXml.EmblemLevel = Global.GetXElementAttributeInt(xelementList[i], "EmblemLevel");
				emblemStarXml.EmblemStar = Global.GetXElementAttributeInt(xelementList[i], "EmblemStar");
				emblemStarXml.ModID = Global.GetXElementAttributeInt(xelementList[i], "ModID");
				emblemStarXml.LifeV = Global.GetXElementAttributeInt(xelementList[i], "LifeV");
				emblemStarXml.AddAttack = Global.GetXElementAttributeInt(xelementList[i], "AddAttack");
				emblemStarXml.AddDefense = Global.GetXElementAttributeInt(xelementList[i], "AddDefense");
				emblemStarXml.DecreaseInjureValue = Global.GetXElementAttributeInt(xelementList[i], "DecreaseInjureValue");
				emblemStarXml.StarExp = Global.GetXElementAttributeInt(xelementList[i], "StarExp");
				emblemStarXml.GoodsExp = Global.GetXElementAttributeInt(xelementList[i], "GoodsExp");
				emblemStarXml.ZuanShiExp = Global.GetXElementAttributeInt(xelementList[i], "ZuanShiExp");
				emblemStarXml.NeedGoods = Global.GetXElementAttributeStr(xelementList[i], "NeedGoods");
				emblemStarXml.NeedDiamond = Global.GetXElementAttributeInt(xelementList[i], "NeedDiamond");
				if (!Global.m_DicStar.ContainsKey(emblemStarXml.ID))
				{
					dictionary[emblemStarXml.ID] = emblemStarXml;
				}
				else
				{
					dictionary.Add(emblemStarXml.ID, emblemStarXml);
				}
			}
			return dictionary;
		}

		public static string VoiceAPP_ID
		{
			get
			{
				return Global.mVoiceAPP_ID;
			}
			set
			{
				Global.mVoiceAPP_ID = value;
			}
		}

		public static string VoiceAPP_Key
		{
			get
			{
				return Global.mVoiceAPP_Key;
			}
			set
			{
				Global.mVoiceAPP_Key = value;
			}
		}

		public static bool IsInZhanMengLianSaiCompetetionMap()
		{
			return Global.InZhanMengLianSaiScene();
		}

		public static bool CanGuanZhan()
		{
			return Global.Data != null && Global.Data.roleData.HideGM > 0 && (Global.IsInZhanMengLianSaiCompetetionMap() || Global.IsInLangHunLingYuScene());
		}

		public static bool IsCompetitionGuanKan
		{
			get
			{
				if (Global.Data != null)
				{
					return Global.CanGuanZhan();
				}
				return Global.mIsCompetitionGuanKan;
			}
			set
			{
				Global.mIsCompetitionGuanKan = value;
				if (PlayZone.GlobalPlayZone != null)
				{
					PlayZone.GlobalPlayZone.HideUIInGuanZhanMode = value;
				}
				Global.HideLeader = value;
			}
		}

		private static bool HideLeader
		{
			set
			{
				if (value)
				{
					GameObject gameObject = GameObject.Find("Leader");
					if (gameObject != null)
					{
						Transform transform = gameObject.transform;
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
					GameObject gameObject2 = GameObject.Find("RoleInfo(Clone)");
					if (gameObject2 != null)
					{
						gameObject2.SetActive(false);
					}
				}
			}
		}

		public static bool IsInKuaFuPlunderMainMap()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 46;
		}

		public static bool IsInKuaFuPlunderBattleMap()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 47;
		}

		public static bool IsInShiLiZhengBaMap()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 48;
		}

		public static bool IsInShiLiZhengBaBattleMap()
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 52;
		}

		public static bool IsShiLiMap(int mapCode)
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
			return settingMapVOByCode != null && settingMapVOByCode.MapType == 48;
		}

		public static bool ReadHaveDownloadInfo()
		{
			Global.DownLoadInfos.Clear();
			StreamReader streamReader = null;
			string persistentPath = PathUtils.GetPersistentPath("HaveDownLoadInfos.txt");
			if (!File.Exists(persistentPath))
			{
				StreamWriter streamWriter = File.CreateText(persistentPath);
				streamWriter.Close();
				streamWriter.Dispose();
				return false;
			}
			try
			{
				streamReader = File.OpenText(persistentPath);
			}
			catch (Exception)
			{
				MUDebug.LogError<string>(new string[]
				{
					"持久化目录没有找到HaveDownLoadInfos"
				});
				return false;
			}
			string text = streamReader.ReadToEnd();
			string[] array = text.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					Global.DownLoadInfos.Add(array[i]);
				}
			}
			streamReader.Close();
			streamReader.Dispose();
			return true;
		}

		public static void SaveDownloadInfo(string info)
		{
		}

		public static List<int> GetMonsterListByMapCod(int mapCode)
		{
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, "fenbao.xml");
			if (gameMapSettingsXml == null)
			{
				gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, "Monsters.xml");
			}
			if (gameMapSettingsXml == null)
			{
				return null;
			}
			List<int> list = new List<int>();
			List<XElement> xelementList = Global.GetXElementList(gameMapSettingsXml, "Monster");
			if (xelementList != null)
			{
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Code");
					if (!list.Contains(xelementAttributeInt))
					{
						list.Add(xelementAttributeInt);
					}
				}
			}
			return list;
		}

		public static List<int> GetNPCListByMapCod(int mapCode)
		{
			List<int> list = new List<int>();
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(mapCode, "npcs.xml");
			if (gameMapSettingsXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameMapSettingsXml, "NPC");
			for (int i = 0; i < xelementList.Count; i++)
			{
				list.Add(Global.GetXElementAttributeInt(xelementList[i], "Code"));
			}
			return list;
		}

		public static List<int> GetMonsterDecoId(int monsterID)
		{
			List<int> list = new List<int>();
			List<int> list2 = new List<int>();
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterID);
			if (!string.IsNullOrEmpty(monsterXmlNodeByID.SkillIDs))
			{
				string[] array = monsterXmlNodeByID.SkillIDs.Split(new char[]
				{
					','
				});
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(Convert.ToInt32(array[i]));
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(list[j]);
				if (maigcInfoVOByCode == null)
				{
					MUDebug.LogError<string>(new string[]
					{
						"YN产品Magics.Xml配错，请检查!～～技能表里不存在此技能，magicID= " + list[j]
					});
				}
				else
				{
					if (!string.IsNullOrEmpty(maigcInfoVOByCode.MagicCode))
					{
						int num = Convert.ToInt32(maigcInfoVOByCode.MagicCode);
						if (num != -1)
						{
							list2.Add(num);
						}
					}
					if (!string.IsNullOrEmpty(maigcInfoVOByCode.FlyDecoration))
					{
						int num2 = Convert.ToInt32(maigcInfoVOByCode.FlyDecoration);
						if (num2 != -1)
						{
							list2.Add(num2);
						}
					}
					if (Global.SafeConvertToInt32(maigcInfoVOByCode.TargetDecoration) > 0)
					{
						list2.Add(Global.SafeConvertToInt32(maigcInfoVOByCode.TargetDecoration));
					}
					if (Global.SafeConvertToInt32(maigcInfoVOByCode.DelayDecoration) > 0)
					{
						list2.Add(Global.SafeConvertToInt32(maigcInfoVOByCode.DelayDecoration));
					}
				}
			}
			return list2;
		}

		public static bool isLoadMap(int mapCode)
		{
			if (Global.FenBaoType == FenBaoDownloadType.None)
			{
				return false;
			}
			if (!Global.isOpenCheckMap)
			{
				return false;
			}
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
			return (settingMapVOByCode.FenBao == 1 || Global.FenBaoType == FenBaoDownloadType.TuiGuang) && (BackStageDownloadManager.instance.IsNeedDownloadMapAsset("Map/" + settingMapVOByCode.ResName) || !Global.IsCurrentMapAllResDownloaded(mapCode));
		}

		private static bool IsCurrentMapAllResDownloaded(int MapCode)
		{
			bool result = true;
			List<int> monsterListByMapCod = Global.GetMonsterListByMapCod(MapCode);
			if (monsterListByMapCod != null)
			{
				for (int i = 0; i < monsterListByMapCod.Count; i++)
				{
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(monsterListByMapCod[i]);
					if (BackStageDownloadManager.instance.IsNeedDownloadMapAsset("Monster/" + monsterXmlNodeByID.ResName))
					{
						return false;
					}
				}
			}
			List<int> npclistByMapCod = Global.GetNPCListByMapCod(MapCode);
			if (npclistByMapCod != null && npclistByMapCod.Count > 0)
			{
				for (int j = 0; j < npclistByMapCod.Count; j++)
				{
					NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npclistByMapCod[j]);
					if (BackStageDownloadManager.instance.IsNeedDownloadMapAsset("Npc/" + npcvobyID.ResName))
					{
						return false;
					}
				}
			}
			if (monsterListByMapCod != null && monsterListByMapCod.Count > 0)
			{
				for (int k = 0; k < monsterListByMapCod.Count; k++)
				{
					List<int> monsterDecoId = Global.GetMonsterDecoId(monsterListByMapCod[k]);
					if (monsterDecoId != null && monsterDecoId.Count > 0)
					{
						for (int l = 0; l < monsterDecoId.Count; l++)
						{
							DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(monsterDecoId[l]);
							if (BackStageDownloadManager.instance.IsNeedDownloadMapAsset("Decoration/" + decorationVOByCode.ResName))
							{
								return false;
							}
						}
					}
				}
			}
			return result;
		}

		public static bool IsPopupDownloadMapWindow(int mapCode)
		{
			return Global.isLoadMap(mapCode) && 1 == Application.internetReachability;
		}

		public static int GetFenBaoMapSize(int mapCode)
		{
			SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(mapCode);
			return settingMapVOByCode.FileSize;
		}

		public static int GetFuBenMapCode(int FuBenId)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
			if (gameResXml == null)
			{
				return 0;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Copy", "ID", FuBenId.ToString());
			if (xelement == null)
			{
				return 0;
			}
			return Global.GetXElementAttributeInt(xelement, "MapCode");
		}

		public static void InitAllDownloadedMapCode()
		{
			Dictionary<int, SettingMapVO> settingsMapVODict = ConfigSettings.GetSettingsMapVODict();
			foreach (KeyValuePair<int, SettingMapVO> keyValuePair in settingsMapVODict)
			{
				if (keyValuePair.Value.FenBao == 0)
				{
					List<int> list = Global.mDownloadedMapList;
					Dictionary<int, SettingMapVO>.Enumerator enumerator;
					KeyValuePair<int, SettingMapVO> keyValuePair2 = enumerator.Current;
					if (!list.Contains(keyValuePair2.Key))
					{
						List<int> list2 = Global.mDownloadedMapList;
						KeyValuePair<int, SettingMapVO> keyValuePair3 = enumerator.Current;
						list2.Add(keyValuePair3.Key);
					}
				}
				else
				{
					List<int> list3 = Global.mDownloadMapQueue;
					Dictionary<int, SettingMapVO>.Enumerator enumerator;
					KeyValuePair<int, SettingMapVO> keyValuePair4 = enumerator.Current;
					if (!list3.Contains(keyValuePair4.Key) && Global.DownLoadInfos != null && Global.DownLoadInfos.Count > 0)
					{
						List<string> downLoadInfos = Global.DownLoadInfos;
						KeyValuePair<int, SettingMapVO> keyValuePair5 = enumerator.Current;
						if (!downLoadInfos.Contains(keyValuePair5.Value.ResName))
						{
							List<int> list4 = Global.mDownloadMapQueue;
							KeyValuePair<int, SettingMapVO> keyValuePair6 = enumerator.Current;
							list4.Add(keyValuePair6.Key);
						}
					}
				}
			}
			MUDebug.Log<string>(new string[]
			{
				"=== InitAllDownloadedMapCode 配置加载完成！"
			});
		}

		public static void AddDownloadedMap(int mapCode)
		{
			if (!Global.mDownloadedMapList.Contains(mapCode))
			{
				Global.mDownloadedMapList.Add(mapCode);
			}
			if (Global.mDownloadMapQueue.Contains(mapCode))
			{
				Global.mDownloadMapQueue.Remove(mapCode);
			}
		}

		public static List<int> GetDownloadMapQueue()
		{
			return Global.mDownloadMapQueue;
		}

		public static List<GoodsData> GetRoleHorseDataList(List<GoodsData> list)
		{
			List<GoodsData> list2 = new List<GoodsData>();
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null && list[i].Site == 13000)
					{
						list2.Add(list[i]);
					}
				}
			}
			return list2;
		}

		public static GoodsData GetRoleFightHorseData(int RoleID)
		{
			List<GoodsData> list = null;
			try
			{
				if (RoleID == Global.Data.RoleID)
				{
					list = Global.Data.roleData.MountEquipList;
				}
				else if (Global.Data.OtherRoles.ContainsKey(RoleID))
				{
					list = Global.Data.OtherRoles[RoleID].MountEquipList;
				}
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != null && list[i].Using == 1)
						{
							return list[i];
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
			return null;
		}

		public static List<GoodsData> GetRoleHorseEquipGoodsDataList(int RoleID, int includeUsingType = -1)
		{
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> list2 = null;
			if (RoleID == Global.Data.RoleID)
			{
				list2 = Global.Data.roleData.GoodsDataList;
			}
			else if (Global.Data.OtherRoles.ContainsKey(RoleID))
			{
				list2 = Global.Data.OtherRoles[RoleID].GoodsDataList;
			}
			if (list2 != null)
			{
				for (int i = 0; i < list2.Count; i++)
				{
					if (40 <= Global.GetGoodsCatetoriy(list2[i].GoodsID) && 45 >= Global.GetGoodsCatetoriy(list2[i].GoodsID))
					{
						if (includeUsingType != -1)
						{
							if (includeUsingType == list2[i].Using)
							{
								list.Add(list2[i]);
							}
						}
						else
						{
							list.Add(list2[i]);
						}
					}
				}
			}
			return list;
		}

		public static bool RoleHaveFightHorse(int RoleID)
		{
			return null != Global.GetRoleFightHorseData(RoleID);
		}

		public static List<HorseSkillData> GetRoleHorseSkillData(int RoleID)
		{
			List<HorseSkillData> list = new List<HorseSkillData>();
			try
			{
				List<GoodsData> list2 = null;
				if (RoleID == Global.Data.RoleID)
				{
					list2 = Global.Data.roleData.MountEquipList;
				}
				else if (Global.Data.OtherRoles.ContainsKey(RoleID))
				{
					list2 = Global.Data.OtherRoles[RoleID].MountEquipList;
				}
				if (list2 != null)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (list2[i] != null && list2[i].Site == 13000)
						{
							HorseSkillData horseSkillData = new HorseSkillData(list2[i]);
							if (0 < horseSkillData.SkillID)
							{
								list.Add(horseSkillData);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
			return list;
		}

		public static bool IsInHorseRequestRideCD()
		{
			return false;
		}

		public static float GetClipLength(Animator anim, string clipName)
		{
			if (anim != null && anim.runtimeAnimatorController != null)
			{
				float num = anim.GetFloat(clipName + "Time");
				if (num <= 0f)
				{
					num = 1f;
				}
				return num;
			}
			return 1f;
		}

		public static AnimationClip GetClip(Animator anim, string clipName)
		{
			if (null == anim)
			{
				return null;
			}
			AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
			for (int i = 0; i < animationClips.Length; i++)
			{
				if (animationClips[i].name == clipName)
				{
					return animationClips[i];
				}
			}
			MUDebug.LogError<string>(new string[]
			{
				"当前动画不存在：" + clipName
			});
			return null;
		}

		public static void PlayAnimatorClip(Animator anim, string clipName)
		{
			if (!anim.isInitialized)
			{
				return;
			}
			if (anim.runtimeAnimatorController != null)
			{
				AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
				for (int i = 0; i < animationClips.Length; i++)
				{
					if (animationClips[i])
					{
						anim.SetBool(animationClips[i].name, false);
					}
				}
			}
			anim.SetBool(clipName, true);
		}

		public static void ZuDuiFuBenTeam(DPSelectedItemEventHandler handler, int fubenId = -1)
		{
			if (Global.Data.CurrentCopyTeamData != null)
			{
				if (fubenId > 0 && Global.Data.CurrentCopyTeamData.SceneIndex == fubenId)
				{
					handler(null, new DPSelectedItemEventArgs
					{
						ID = 0
					});
					return;
				}
				string message = "当前正在多人副本房间中，进入其他活动或副本会导致退出房间，确定进入吗？";
				string[] buttons = new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				};
				Super.ShowMessageBoxEx(Global.GetLang("提示"), message, delegate(object s2, DPSelectedItemEventArgs e2)
				{
					if (e2.ID == 0)
					{
						Global.Data.CurrentCopyTeamData = null;
						if (PlayZone.GlobalPlayZone._ActivityPart != null && PlayZone.GlobalPlayZone._ActivityPart._ZuDuiFuBen != null)
						{
							PlayZone.GlobalPlayZone._ActivityPart._ZuDuiFuBen.QuitTeam();
						}
						else
						{
							GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
						}
					}
					handler(s2, new DPSelectedItemEventArgs
					{
						ID = e2.ID
					});
				}, buttons);
			}
			else
			{
				handler(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		}

		public static Dictionary<int, bool> DicHorseEquipOpen
		{
			get
			{
				if (Global.dicHorseEquipOpen.Count <= 0)
				{
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("HorseEquipBarOpen", ',');
					if (systemParamIntArrayByName.Length == 6)
					{
						Global.dicHorseEquipOpen.Add(40, systemParamIntArrayByName[0] == 1);
						Global.dicHorseEquipOpen.Add(41, systemParamIntArrayByName[1] == 1);
						Global.dicHorseEquipOpen.Add(42, systemParamIntArrayByName[2] == 1);
						Global.dicHorseEquipOpen.Add(43, systemParamIntArrayByName[3] == 1);
						Global.dicHorseEquipOpen.Add(44, systemParamIntArrayByName[4] == 1);
						Global.dicHorseEquipOpen.Add(45, systemParamIntArrayByName[5] == 1);
					}
				}
				return Global.dicHorseEquipOpen;
			}
		}

		public static bool HorseEquipOpen(int goodId)
		{
			int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodId);
			return Global.DicHorseEquipOpen.ContainsKey(categoriyByGoodsID) && Global.DicHorseEquipOpen[categoriyByGoodsID];
		}

		public static Dictionary<string, int> MaxZhuoYurZuoQi(int goodsID)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			HorsePokedexVO horsePokedexByHorseID = IConfigbase<ConfigRidePet>.Instance.GetHorsePokedexByHorseID(goodsID);
			if (horsePokedexByHorseID == null)
			{
				return dictionary;
			}
			int superiorAttributeID = horsePokedexByHorseID.SuperiorAttributeID;
			if (!IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop.ContainsKey(superiorAttributeID))
			{
				return dictionary;
			}
			string[] array = string.Format("{0},{1}", IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop[superiorAttributeID].CommonSuperiorBank, IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorDrop[superiorAttributeID].SeniorSuperiorBank).Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				foreach (KeyValuePair<int, HorseSuperiorType> keyValuePair in IConfigbase<ConfigRidePet>.Instance.DicHorseSuperiorType)
				{
					if (keyValuePair.Value.ID == array[i].SafeToInt32(0))
					{
						Dictionary<int, HorseSuperiorType>.Enumerator enumerator;
						KeyValuePair<int, HorseSuperiorType> keyValuePair2 = enumerator.Current;
						string[] array2 = keyValuePair2.Value.Parameter.Split(new char[]
						{
							'|'
						});
						int[] array3 = new int[array2.Length];
						for (int j = 0; j < array2.Length; j++)
						{
							array3[j] = array2[j].Split(new char[]
							{
								','
							})[0].SafeToInt32(0);
						}
						Dictionary<string, int> dictionary2 = dictionary;
						KeyValuePair<int, HorseSuperiorType> keyValuePair3 = enumerator.Current;
						if (dictionary2.ContainsKey(keyValuePair3.Value.Type))
						{
							Dictionary<string, int> dictionary3 = dictionary;
							KeyValuePair<int, HorseSuperiorType> keyValuePair4 = enumerator.Current;
							dictionary3[keyValuePair4.Value.Type] = Mathf.Max(array3);
						}
						else
						{
							Dictionary<string, int> dictionary4 = dictionary;
							KeyValuePair<int, HorseSuperiorType> keyValuePair5 = enumerator.Current;
							dictionary4.Add(keyValuePair5.Value.Type, Mathf.Max(array3));
						}
					}
				}
			}
			return dictionary;
		}

		public static void StartRequest()
		{
			Super.ShowNetWaiting(null);
			Global.requstRetainCount = 0;
			GameInstance.Game.GetLingyuList();
			Global.requstRetainCount++;
			GameInstance.Game.SpriteQueryStarConstelltionInfoCmd();
			Global.requstRetainCount++;
			GameInstance.Game.FuWenChengJiuInfo();
			Global.requstRetainCount++;
			GameInstance.Game.GetAdendaInfo();
			Global.requstRetainCount++;
			GameInstance.Game.GetGuardStatueInfo();
			Global.requstRetainCount++;
			GameInstance.Game.GetGetTaLuopaiData();
			GameInstance.Game.SpritQueryYuansuBagByUsed(3001);
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.MeiLanZhiShu))
			{
				TCPGameServerCmds.CMD_SPR_MERLIN_QUERY.SendDataUseRoleID();
				Global.requstRetainCount++;
			}
			GameInstance.Game.SendShenDianInfo();
			Global.requstRetainCount++;
			GameInstance.Game.SendGetShenQiDataRequest();
			Global.requstRetainCount++;
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese))
			{
				GameInstance.Game.GetRidePetMainData();
				Global.requstRetainCount++;
			}
		}

		public static void OnRequestCallBack_CheckPending()
		{
			Global.requstRetainCount--;
			if (Global.requstRetainCount > 0)
			{
				return;
			}
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.m_FuBenTiShiPart != null)
			{
				PlayZone.GlobalPlayZone.m_FuBenTiShiPart.RefreshImprovingView();
			}
		}

		public static List<TiShengZhanLiItemVO> GetDataList(int index)
		{
			XElement gameResXml = Global.GetGameResXml("BianQiang");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "BianQiang");
			XElement gameResXml2 = Global.GetGameResXml("Standard");
			Global.BianQiangStandardList = Global.GetXElementList(gameResXml2, "BianQiang");
			Global.listData.Clear();
			int count = xelementList.Count;
			for (int i = 0; i < count; i++)
			{
				XElement xelement = xelementList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "PlaceType");
				if (xelementAttributeInt2 == 1)
				{
					int level = Global.Data.roleData.Level;
					int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
					string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Parameter");
					string[] array = xelementAttributeStr.Split(new char[]
					{
						','
					});
					int num = int.Parse(array[0]);
					int num2 = int.Parse(array[1]);
					if (changeLifeCount * 100 + level >= num * 100 + num2)
					{
						TiShengZhanLiItemVO itemVO = Global.GetItemVO(xelementAttributeInt, xelement);
						if (itemVO != null)
						{
							if (index == 0)
							{
								if (itemVO.percent <= 0.5f)
								{
									Global.listData.Add(itemVO);
								}
							}
							else if (index == 1)
							{
								if (itemVO.percent > 0.5f && itemVO.percent < 1f)
								{
									Global.listData.Add(itemVO);
								}
							}
							else if (index == 2 && itemVO.percent == 1f)
							{
								Global.listData.Add(itemVO);
							}
						}
					}
				}
				else if (xelementAttributeInt2 == 2)
				{
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "Parameter");
					int completedMainTaskID = Global.Data.roleData.CompletedMainTaskID;
					if (completedMainTaskID >= xelementAttributeInt3)
					{
						TiShengZhanLiItemVO itemVO2 = Global.GetItemVO(xelementAttributeInt, xelement);
						if (itemVO2 != null)
						{
							if (index == 0)
							{
								if (itemVO2.percent <= 0.5f)
								{
									Global.listData.Add(itemVO2);
								}
							}
							else if (index == 1)
							{
								if (itemVO2.percent > 0.5f && itemVO2.percent < 1f)
								{
									Global.listData.Add(itemVO2);
								}
							}
							else if (index == 2 && itemVO2.percent == 1f)
							{
								Global.listData.Add(itemVO2);
							}
						}
					}
				}
				else if (xelementAttributeInt2 == 3)
				{
					string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Parameter");
					string[] array2 = xelementAttributeStr2.Split(new char[]
					{
						','
					});
					if ((long)Global.Data.roleData.MyWingData.WingID >= (long)((ulong)Convert.ToUInt32(array2[0])))
					{
						TiShengZhanLiItemVO itemVO3 = Global.GetItemVO(xelementAttributeInt, xelement);
						if (itemVO3 != null)
						{
							if (index == 0)
							{
								if (itemVO3.percent <= 0.5f)
								{
									Global.listData.Add(itemVO3);
								}
							}
							else if (index == 1)
							{
								if (itemVO3.percent > 0.5f && itemVO3.percent < 1f)
								{
									Global.listData.Add(itemVO3);
								}
							}
							else if (index == 2 && itemVO3.percent == 1f)
							{
								Global.listData.Add(itemVO3);
							}
						}
					}
				}
				else if (xelementAttributeInt2 == 4 && Global.GetChengJiuLevel(0) >= 4)
				{
					TiShengZhanLiItemVO itemVO4 = Global.GetItemVO(xelementAttributeInt, xelement);
					if (itemVO4 != null)
					{
						if (index == 0)
						{
							if (itemVO4.percent <= 0.5f)
							{
								Global.listData.Add(itemVO4);
							}
						}
						else if (index == 1)
						{
							if (itemVO4.percent > 0.5f && itemVO4.percent < 1f)
							{
								Global.listData.Add(itemVO4);
							}
						}
						else if (index == 2 && itemVO4.percent == 1f)
						{
							Global.listData.Add(itemVO4);
						}
					}
				}
				else if (xelementAttributeInt2 == 5 && Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel) >= 4)
				{
					TiShengZhanLiItemVO itemVO5 = Global.GetItemVO(xelementAttributeInt, xelement);
					if (itemVO5 != null)
					{
						if (index == 0)
						{
							if (itemVO5.percent <= 0.5f)
							{
								Global.listData.Add(itemVO5);
							}
						}
						else if (index == 1)
						{
							if (itemVO5.percent > 0.5f && itemVO5.percent < 1f)
							{
								Global.listData.Add(itemVO5);
							}
						}
						else if (index == 2 && itemVO5.percent == 1f)
						{
							Global.listData.Add(itemVO5);
						}
					}
				}
			}
			return Global.listData;
		}

		public static void RecvShenDianData(UnionPalaceData Data)
		{
			Global.CurrentData = Data;
			Global.OnRequestCallBack_CheckPending();
		}

		public static void OnRequestCallBack_QueryStar(Dictionary<int, int> dic)
		{
			if (dic != null)
			{
				Global.m_DicActiveStar = dic;
				int num = 0;
				foreach (KeyValuePair<int, int> keyValuePair in Global.m_DicActiveStar)
				{
					num += keyValuePair.Value;
				}
			}
			Global.OnRequestCallBack_CheckPending();
		}

		public static void OnRequestCallBack_GuardStatue(GuardStatueData data)
		{
			Global.guardStatue = data;
			Global.OnRequestCallBack_CheckPending();
		}

		public static XElement GetBianQiangStandardItemXML(int id)
		{
			XElement result = null;
			int count = Global.BianQiangStandardList.Count;
			for (int i = 0; i < count; i++)
			{
				XElement xelement = Global.BianQiangStandardList[i];
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (xelementAttributeInt == id)
				{
					int level = Global.Data.roleData.Level;
					int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "MinZhuanShengLevel");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "MinLevel");
					int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MaxZhuanShengLevel");
					int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "MaxLevel");
					if (xelementAttributeInt2 * 100 + xelementAttributeInt3 <= changeLifeCount * 100 + level && changeLifeCount * 100 + level <= xelementAttributeInt4 * 100 + xelementAttributeInt5)
					{
						result = xelement;
						break;
					}
				}
			}
			return result;
		}

		public static float GetAverageSkillLevel()
		{
			if (Global.Data.roleData.SkillDataList == null)
			{
				return 0f;
			}
			int num = 0;
			int num2 = 0;
			List<MagicInfoVO> skillListByOccupation = Global.GetSkillListByOccupation(Global.Data.roleData.Occupation);
			for (int i = 0; i < skillListByOccupation.Count; i++)
			{
				MagicInfoVO magicInfoVO = skillListByOccupation[i];
				int id = magicInfoVO.ID;
				int magicIcon = magicInfoVO.MagicIcon;
				int actionIndex = magicInfoVO.ActionIndex;
				if (actionIndex < 1000)
				{
					SkillData skillDataByID = Global.GetSkillDataByID(id);
					if (skillDataByID != null && skillDataByID.SkillLevel > 0)
					{
						num += skillDataByID.SkillLevel;
						num2++;
					}
				}
			}
			float result = 0f;
			if (num2 > 0)
			{
				result = (float)(num / num2);
			}
			return result;
		}

		public static float GetAvergeEquipJieShu()
		{
			int num = 0;
			int num2 = 0;
			if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
			{
				Global.GetUsingGoodsDataList();
			}
			Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
			foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null && (goodsXmlNodeByID.Categoriy == 0 || goodsXmlNodeByID.Categoriy == 1 || goodsXmlNodeByID.Categoriy == 2 || goodsXmlNodeByID.Categoriy == 3 || goodsXmlNodeByID.Categoriy == 4 || goodsXmlNodeByID.Categoriy == 5 || goodsXmlNodeByID.Categoriy == 6 || goodsXmlNodeByID.Categoriy == 11 || goodsXmlNodeByID.Categoriy == 12 || goodsXmlNodeByID.Categoriy == 13 || goodsXmlNodeByID.Categoriy == 14 || goodsXmlNodeByID.Categoriy == 15 || goodsXmlNodeByID.Categoriy == 16 || goodsXmlNodeByID.Categoriy == 17 || goodsXmlNodeByID.Categoriy == 18 || goodsXmlNodeByID.Categoriy == 19 || goodsXmlNodeByID.Categoriy == 21))
				{
					num += goodsXmlNodeByID.SuitID;
					num2++;
				}
			}
			float result = 0f;
			if (num2 > 0)
			{
				result = (float)(num / num2);
			}
			return result;
		}

		public static float GetAvergeEquipQiangHuaLevel()
		{
			int num = 0;
			int num2 = 0;
			if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
			{
				Global.GetUsingGoodsDataList();
			}
			Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
			foreach (KeyValuePair<int, GoodsData> keyValuePair in roleUsingGoodsDataList)
			{
				GoodsData value = keyValuePair.Value;
				GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
				if (goodsXmlNodeByID != null && (goodsXmlNodeByID.Categoriy == 0 || goodsXmlNodeByID.Categoriy == 1 || goodsXmlNodeByID.Categoriy == 2 || goodsXmlNodeByID.Categoriy == 3 || goodsXmlNodeByID.Categoriy == 4 || goodsXmlNodeByID.Categoriy == 5 || goodsXmlNodeByID.Categoriy == 6 || goodsXmlNodeByID.Categoriy == 11 || goodsXmlNodeByID.Categoriy == 12 || goodsXmlNodeByID.Categoriy == 13 || goodsXmlNodeByID.Categoriy == 14 || goodsXmlNodeByID.Categoriy == 15 || goodsXmlNodeByID.Categoriy == 16 || goodsXmlNodeByID.Categoriy == 17 || goodsXmlNodeByID.Categoriy == 18 || goodsXmlNodeByID.Categoriy == 19 || goodsXmlNodeByID.Categoriy == 21))
				{
					num += value.Forge_level;
					num2++;
				}
			}
			float result = 0f;
			if (num2 > 0)
			{
				result = (float)(num / num2);
			}
			return result;
		}

		public static int GetPetBattlingLevel()
		{
			int result = 0;
			if (Global.Data.equipPet != null)
			{
				int count = Global.Data.equipPet.Count;
				for (int i = 0; i < count; i++)
				{
					GoodsData goodsData = Global.Data.equipPet[i];
					if (goodsData != null && goodsData.Using == 1)
					{
						result = goodsData.Forge_level + 1;
						break;
					}
				}
			}
			return result;
		}

		public static float GetAvergeEquipZhuiJia()
		{
			int num = 0;
			int num2 = 0;
			if (Super.GData.RoleUsingGoodsDataList.Count <= 0)
			{
				Global.GetUsingGoodsDataList();
			}
			Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
			KeyValuePair<int, GoodsData> keyValuePair = roleUsingGoodsDataList.GetEnumerator().Current;
			GoodsData value = keyValuePair.Value;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(value.GoodsID);
			if (goodsXmlNodeByID != null && (goodsXmlNodeByID.Categoriy == 0 || goodsXmlNodeByID.Categoriy == 1 || goodsXmlNodeByID.Categoriy == 2 || goodsXmlNodeByID.Categoriy == 3 || goodsXmlNodeByID.Categoriy == 4 || goodsXmlNodeByID.Categoriy == 5 || goodsXmlNodeByID.Categoriy == 6 || goodsXmlNodeByID.Categoriy == 11 || goodsXmlNodeByID.Categoriy == 12 || goodsXmlNodeByID.Categoriy == 13 || goodsXmlNodeByID.Categoriy == 14 || goodsXmlNodeByID.Categoriy == 15 || goodsXmlNodeByID.Categoriy == 16 || goodsXmlNodeByID.Categoriy == 17 || goodsXmlNodeByID.Categoriy == 18 || goodsXmlNodeByID.Categoriy == 19 || goodsXmlNodeByID.Categoriy == 21))
			{
				num += value.AppendPropLev;
				num2++;
			}
			float result = 0f;
			if (num2 > 0)
			{
				result = (float)(num / num2);
			}
			return result;
		}

		public static void GetDataBagMerlinGrowthSaveDBData(MerlinGrowthSaveDBData data)
		{
			Global.DataBagMerlinGrowthSaveDBData = data;
			Global.OnRequestCallBack_CheckPending();
		}

		public static void CountLingyuTotalLevel(List<LingYuData> lingyuList)
		{
			for (int i = 0; i < lingyuList.Count; i++)
			{
				Global.lingyuTotalLevel += lingyuList[i].Suit;
			}
			Global.OnRequestCallBack_CheckPending();
		}

		public static int zuoQiLevelByBianQiang
		{
			get
			{
				return Global._zuoQiLevelByBianQiang;
			}
			set
			{
				Global._zuoQiLevelByBianQiang = value;
				Global.OnRequestCallBack_CheckPending();
			}
		}

		public static void RecvShenQiData(ShenQiData Data)
		{
			Global.shenQiData = Data;
			Global.OnRequestCallBack_CheckPending();
		}

		public static int GetGuardStatueGrade()
		{
			if (Global.guardStatue == null)
			{
				return 1;
			}
			return Global.guardStatue.grade;
		}

		public static float GetShenDianLevel()
		{
			if (Global.CurrentData == null)
			{
				return 0f;
			}
			return (float)Global.CurrentData.StatueLevel;
		}

		public static int GetRoleUsingJInglingSkillAllLev()
		{
			if (Global.Data.equipPet != null)
			{
				int count = Global.Data.equipPet.Count;
				int i = 0;
				while (i < count)
				{
					GoodsData goodsData = Global.Data.equipPet[i];
					if (goodsData.Using == 1)
					{
						if (goodsData.ElementhrtsProps != null)
						{
							int num = 0;
							int num2 = 0;
							for (int j = 0; j < goodsData.ElementhrtsProps.Count; j++)
							{
								if (j % 3 == 1)
								{
									int num3 = goodsData.ElementhrtsProps[j];
									if (1 > num3)
									{
										num3 = 1;
									}
									num += num3;
									num2 = num3;
								}
								if (j % 3 == 2)
								{
									if (0 >= goodsData.ElementhrtsProps[j])
									{
										num -= num2;
									}
									num2 = 0;
								}
							}
							return num;
						}
						return 0;
					}
					else
					{
						i++;
					}
				}
			}
			return 0;
		}

		public static TiShengZhanLiItemVO GetItemVO(int ID, XElement item)
		{
			TiShengZhanLiItemVO tiShengZhanLiItemVO = new TiShengZhanLiItemVO();
			XElement bianQiangStandardItemXML = Global.GetBianQiangStandardItemXML(ID);
			tiShengZhanLiItemVO.ID = ID;
			tiShengZhanLiItemVO.ImageName = Global.GetXElementAttributeStr(item, "Image");
			tiShengZhanLiItemVO.LinkID = Global.GetXElementAttributeInt(item, "Link");
			if (bianQiangStandardItemXML == null)
			{
				return null;
			}
			tiShengZhanLiItemVO.Name = Global.GetXElementAttributeStr(bianQiangStandardItemXML, "Name");
			tiShengZhanLiItemVO.StandardValue = Global.GetXElementAttributeInt(bianQiangStandardItemXML, "Value");
			tiShengZhanLiItemVO.ItemType = Global.GetXElementAttributeInt(bianQiangStandardItemXML, "Type");
			if (tiShengZhanLiItemVO.StandardValue == 0)
			{
				return null;
			}
			float num = 0f;
			if (tiShengZhanLiItemVO.ItemType == 1)
			{
				WingData myWingData = Global.Data.roleData.MyWingData;
				float num2 = (float)myWingData.ForgeLevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num2;
			}
			else if (tiShengZhanLiItemVO.ItemType == 2)
			{
				int wingID = Global.Data.roleData.MyWingData.WingID;
				float num3 = (float)wingID / (float)tiShengZhanLiItemVO.StandardValue;
				num = num3;
			}
			else if (tiShengZhanLiItemVO.ItemType == 3)
			{
				float averageSkillLevel = Global.GetAverageSkillLevel();
				num = averageSkillLevel / (float)tiShengZhanLiItemVO.StandardValue;
			}
			else if (tiShengZhanLiItemVO.ItemType == 4)
			{
				float avergeEquipJieShu = Global.GetAvergeEquipJieShu();
				num = avergeEquipJieShu / (float)tiShengZhanLiItemVO.StandardValue;
			}
			else if (tiShengZhanLiItemVO.ItemType == 5)
			{
				float avergeEquipQiangHuaLevel = Global.GetAvergeEquipQiangHuaLevel();
				num = avergeEquipQiangHuaLevel / (float)tiShengZhanLiItemVO.StandardValue;
			}
			else if (tiShengZhanLiItemVO.ItemType == 6)
			{
				float avergeEquipZhuiJia = Global.GetAvergeEquipZhuiJia();
				num = avergeEquipZhuiJia / (float)tiShengZhanLiItemVO.StandardValue;
			}
			else if (tiShengZhanLiItemVO.ItemType == 7)
			{
				float num4 = (float)Global.GetHuShengFuLevel();
				num = num4 / (float)tiShengZhanLiItemVO.StandardValue;
			}
			else if (tiShengZhanLiItemVO.ItemType == 8)
			{
				int viplevel = Global.Data.roleData.VIPLevel;
				float num5 = (float)viplevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num5;
			}
			else if (tiShengZhanLiItemVO.ItemType == 9)
			{
				int chengJiuLevel = Global.GetChengJiuLevel(0);
				float num6 = (float)chengJiuLevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num6;
			}
			else if (tiShengZhanLiItemVO.ItemType == 10)
			{
				int roleCommonUseParamsValue = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
				float num7 = (float)roleCommonUseParamsValue / (float)tiShengZhanLiItemVO.StandardValue;
				num = num7;
			}
			else if (tiShengZhanLiItemVO.ItemType == 11)
			{
				int opne_XIN_HUN_COUNT = TiShengZhanLiPart.OPNE_XIN_HUN_COUNT;
				float num8 = (float)opne_XIN_HUN_COUNT / (float)tiShengZhanLiItemVO.StandardValue;
				num = num8;
			}
			else if (tiShengZhanLiItemVO.ItemType == 12)
			{
				int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
				float num9 = (float)changeLifeCount / (float)tiShengZhanLiItemVO.StandardValue;
				num = num9;
			}
			else if (tiShengZhanLiItemVO.ItemType == 13)
			{
				int sumOfAllEquipXilianValue = Global.GetSumOfAllEquipXilianValue();
				float num10 = (float)sumOfAllEquipXilianValue / (float)tiShengZhanLiItemVO.StandardValue;
				num = num10;
			}
			else if (tiShengZhanLiItemVO.ItemType == 14)
			{
				int petBattlingLevel = Global.GetPetBattlingLevel();
				float num11 = (float)petBattlingLevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num11;
			}
			else if (tiShengZhanLiItemVO.ItemType == 15)
			{
				float num12 = (float)Global.GetSumOfAllUsedYuansuLevel();
				float num13 = num12 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num13;
			}
			else if (tiShengZhanLiItemVO.ItemType == 16)
			{
				float num14 = (float)Global.lingyuTotalLevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num14;
			}
			else if (tiShengZhanLiItemVO.ItemType == 17)
			{
				float num15 = (float)Global.Data.roleData.MyWingData.ZhuLingNum;
				float num16 = num15 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num16 + 0.0001f;
			}
			else if (tiShengZhanLiItemVO.ItemType == 18)
			{
				float num17 = (float)Global.Data.roleData.MyWingData.ZhuHunNum;
				float num18 = num17 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num18 + 0.0001f;
			}
			else if (tiShengZhanLiItemVO.ItemType == 19)
			{
				float fuWenLv = Global.GetFuWenLv();
				float num19 = fuWenLv / (float)tiShengZhanLiItemVO.StandardValue;
				num = num19;
			}
			else if (tiShengZhanLiItemVO.ItemType == 20)
			{
				float shengWangXunZhangLv = Global.GetShengWangXunZhangLv();
				float num20 = shengWangXunZhangLv / (float)tiShengZhanLiItemVO.StandardValue;
				num = num20;
			}
			else if (tiShengZhanLiItemVO.ItemType == 21)
			{
				float num21 = (float)Global.DataBagMerlinGrowthSaveDBData._Level;
				float num22 = num21 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num22;
			}
			else if (tiShengZhanLiItemVO.ItemType == 22)
			{
				float num23 = (float)Global.GetGuardStatueGrade();
				float num24 = num23 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num24;
			}
			else if (tiShengZhanLiItemVO.ItemType == 23)
			{
				float num25 = (float)Global.GetFluorescentDiamondTotalLevel();
				float num26 = num25 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num26;
			}
			else if (tiShengZhanLiItemVO.ItemType == 24)
			{
				float shengWuTotalLevel = Global.GetShengWuTotalLevel();
				float num27 = shengWuTotalLevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num27;
			}
			else if (tiShengZhanLiItemVO.ItemType == 25)
			{
				float shenDianLevel = Global.GetShenDianLevel();
				float num28 = shenDianLevel / (float)tiShengZhanLiItemVO.StandardValue;
				num = num28;
			}
			else if (tiShengZhanLiItemVO.ItemType == 26)
			{
				float num29 = (float)Global.GetRoleUsingJInglingSkillAllLev();
				float num30 = num29 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num30;
			}
			else if (tiShengZhanLiItemVO.ItemType == 27)
			{
				float num31 = (float)Global.Data.taLuoPaiLevel;
				float num32 = num31 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num32;
			}
			else if (tiShengZhanLiItemVO.ItemType == 28)
			{
				if (Global.shenQiData != null)
				{
					float num33 = (float)(Global.shenQiData.LifeAdd + Global.shenQiData.DefenseAdd + Global.shenQiData.ToughnessAdd + Global.shenQiData.AttackAdd);
					ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(Global.shenQiData.ShenQiID);
					float num34 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
					bool flag = num34 == num33;
					float num35 = (float)((!flag) ? (Global.shenQiData.ShenQiID - 1) : Global.shenQiData.ShenQiID);
					float num36 = num35 / (float)tiShengZhanLiItemVO.StandardValue;
					num = num36;
				}
			}
			else if (tiShengZhanLiItemVO.ItemType == 29)
			{
				float num37 = 0f;
				if (Global.Data.roleData.ShenJiDict != null && Global.Data.roleData.ShenJiDict.Count != 0)
				{
					Dictionary<int, ShenJiFuWenData>.Enumerator enumerator = Global.Data.roleData.ShenJiDict.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Dictionary<int, ShenJiFuWen> dicShenJiFuWen = SpiritTrackPart.GetDicShenJiFuWen();
						KeyValuePair<int, ShenJiFuWenData> keyValuePair = enumerator.Current;
						if (dicShenJiFuWen.ContainsKey(keyValuePair.Value.ID))
						{
							float num38 = num37;
							Dictionary<int, ShenJiFuWen> dicShenJiFuWen2 = SpiritTrackPart.GetDicShenJiFuWen();
							KeyValuePair<int, ShenJiFuWenData> keyValuePair2 = enumerator.Current;
							float upNeed = (float)dicShenJiFuWen2[keyValuePair2.Value.ID].UpNeed;
							KeyValuePair<int, ShenJiFuWenData> keyValuePair3 = enumerator.Current;
							num37 = num38 + upNeed * (float)keyValuePair3.Value.Level;
						}
					}
				}
				num37 += (float)Global.Data.roleData.RoleCommonUseIntPamams[46];
				float num39 = num37 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num39;
			}
			else if (tiShengZhanLiItemVO.ItemType == 30)
			{
				float num40 = (float)Global.SetEmbelemUpLevel;
				float num41 = num40 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num41;
			}
			else if (tiShengZhanLiItemVO.ItemType == 31)
			{
				float num42 = (float)(Global.zuoQiLevelByBianQiang + 1);
				float num43 = num42 / (float)tiShengZhanLiItemVO.StandardValue;
				num = num43;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num > 1f)
			{
				num = 1f;
			}
			tiShengZhanLiItemVO.percent = num;
			return tiShengZhanLiItemVO;
		}

		public static bool IsBaoXiangTips(int index)
		{
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("NotShowTips", ',');
			return systemParamIntArrayByName.IndexOf(index) == -1;
		}

		public static void TuJianRedPoint()
		{
			if (Global.tuJianRedDatas.Count <= 0)
			{
				XElement isolateResXml = Global.GetIsolateResXml("Config/TuJianType.xml");
				if (isolateResXml != null)
				{
					List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "TuJian"), "*");
					for (int i = 0; i < xelementList.Count; i++)
					{
						XElement xelement = xelementList[i];
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ID");
						string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "KaiQiLevel");
						Global.TuJianRedData tuJianRedData = new Global.TuJianRedData();
						tuJianRedData.OpenStr = xelementAttributeStr2;
						tuJianRedData.strTypeID = xelementAttributeStr;
						tuJianRedData.PropsStr = Global.GetXElementAttributeStr(xelement, "ShuXiangJiaCheng");
						Global.tuJianRedDatas.Add(tuJianRedData);
					}
				}
			}
			for (int j = 0; j < Global.tuJianRedDatas.Count; j++)
			{
				Global.TuJianRedData tuJianRedData2 = Global.tuJianRedDatas[j];
				int num = 0;
				int activedTujianTypeNumByTypeIDByRedPoint = Global.GetActivedTujianTypeNumByTypeIDByRedPoint(tuJianRedData2.strTypeID.SafeToInt32(0), out num);
				tuJianRedData2.IsActived = (activedTujianTypeNumByTypeIDByRedPoint >= num);
			}
			List<Global.TuJianRedData> list = new List<Global.TuJianRedData>();
			for (int k = 0; k < Global.tuJianRedDatas.Count; k++)
			{
				if (!string.IsNullOrEmpty(Global.tuJianRedDatas[k].OpenStr))
				{
					string[] array = Global.tuJianRedDatas[k].OpenStr.Split(new char[]
					{
						','
					});
					if (array != null)
					{
						if (UIHelper.AvalidLevel(array[1].SafeToInt32(0), array[0].SafeToInt32(0), false))
						{
							list.Add(Global.tuJianRedDatas[k]);
						}
					}
				}
			}
			Global.InitTujianListInBagByRedPoint();
			int num2 = 0;
			for (int l = 0; l < list.Count; l++)
			{
				if (Global.GetIsCanSubmitByTypeID(Global.SafeConvertToInt32(list[l].strTypeID)))
				{
					num2++;
				}
			}
			ActivityTipManager.SetActivityTipItemActive(19000, num2 > 0);
		}

		public static int GetActivedTujianTypeNumByTypeIDByRedPoint(int typeID, out int totalTypeNum)
		{
			totalTypeNum = 0;
			if (Global.TujiaXmlDataDictByRedPoint == null)
			{
				Global.InitTujiaXmlDataDictByRedPoint();
			}
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<int, TujianXmlData> keyValuePair in Global.TujiaXmlDataDictByRedPoint)
			{
				TujianXmlData value = keyValuePair.Value;
				if (value.TypeID == typeID)
				{
					totalTypeNum++;
					string[] array = value.NeedGoods.Split(new char[]
					{
						','
					});
					if (array != null)
					{
						num2 = array[1].SafeToInt32(0);
					}
					int tiJiaoTuJianNum = Global.GetTiJiaoTuJianNum(value.ID);
					if (tiJiaoTuJianNum >= num2)
					{
						value.IsActived = true;
						num++;
					}
					else
					{
						value.IsActived = false;
					}
				}
			}
			return num;
		}

		private static void InitTujiaXmlDataDictByRedPoint()
		{
			if (Global.TujiaXmlDataDictByRedPoint == null)
			{
				XElement isolateResXml = Global.GetIsolateResXml("Config/TuJianItems.xml");
				if (isolateResXml == null)
				{
					return;
				}
				Global.TujiaXmlDataDictByRedPoint = new Dictionary<int, TujianXmlData>();
				List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(isolateResXml, "config"), "*");
				for (int i = 0; i < xelementList.Count; i++)
				{
					XElement xelement = xelementList[i];
					TujianXmlData tujianXmlData = new TujianXmlData();
					tujianXmlData.ID = Global.GetXElementAttributeInt(xelement, "ID");
					tujianXmlData.TypeID = Global.GetXElementAttributeInt(xelement, "Type");
					tujianXmlData.MonsterID = Global.GetXElementAttributeInt(xelement, "GLMonsterID");
					tujianXmlData.Props = Global.GetXElementAttributeStr(xelement, "ShuXing");
					tujianXmlData.NeedGoods = Global.GetXElementAttributeStr(xelement, "NeedGoods");
					tujianXmlData.Name = Global.GetXElementAttributeStr(xelement, "Name");
					if (!Global.TujiaXmlDataDictByRedPoint.ContainsKey(tujianXmlData.ID))
					{
						Global.TujiaXmlDataDictByRedPoint.Add(tujianXmlData.ID, tujianXmlData);
					}
				}
			}
		}

		public static void InitTujianListInBagByRedPoint()
		{
			if (Global.TujiaXmlDataDictByRedPoint == null)
			{
				Global.InitTujiaXmlDataDictByRedPoint();
			}
			if (Global.TujianListInBagDictByRedPoint == null)
			{
				Global.TujianListInBagDictByRedPoint = new Dictionary<int, int>();
			}
			Global.TujianListInBagDictByRedPoint.Clear();
			foreach (KeyValuePair<int, TujianXmlData> keyValuePair in Global.TujiaXmlDataDictByRedPoint)
			{
				TujianXmlData value = keyValuePair.Value;
				if (!value.IsActived)
				{
					string[] array = value.NeedGoods.Split(new char[]
					{
						','
					});
					if (array != null)
					{
						int goodsID = array[0].SafeToInt32(0);
						if (Global.GetTotalGoodsCountByID(goodsID) > 0 && !Global.TujianListInBagDictByRedPoint.ContainsKey(value.ID))
						{
							Global.TujianListInBagDictByRedPoint.Add(value.ID, value.TypeID);
						}
					}
				}
			}
		}

		private static bool GetIsCanSubmitByTypeID(int typeID)
		{
			if (Global.TujianListInBagDictByRedPoint == null || Global.TujianListInBagDictByRedPoint.Keys.Count <= 0)
			{
				return false;
			}
			Dictionary<int, int>.Enumerator enumerator = Global.TujianListInBagDictByRedPoint.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<int, int> tujianListInBagDictByRedPoint = Global.TujianListInBagDictByRedPoint;
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				if (tujianListInBagDictByRedPoint[keyValuePair.Key] == typeID)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetRedPointCount(RedPointType type, int count)
		{
			RedPointVO dataByType = RedPointManager.GetDataByType(type);
			if (dataByType != null)
			{
				int num = Global.SafeConvertToInt32(dataByType.Parameter);
				if (count == -1)
				{
					return num;
				}
				if (count >= num)
				{
					GameInstance.Game.SpriteSetSystemOpenParams((int)type);
				}
			}
			return 0;
		}

		public static void SaveQiangHuaTimes()
		{
			int num = Global.GetQiangHuaTimes();
			PlayerPrefs.SetInt("RedPointQiangHua" + Global.Data.RoleID, ++num);
			MUDebug.Log<string>(new string[]
			{
				"装备强化次数 " + num
			});
			Global.GetRedPointCount(RedPointType.QiangHua, num);
		}

		public static int GetQiangHuaTimes()
		{
			if (Global.IsSystemOpen(4))
			{
				int redPointCount = Global.GetRedPointCount(RedPointType.QiangHua, -1);
				return redPointCount + 1;
			}
			return PlayerPrefs.GetInt("RedPointQiangHua" + Global.Data.RoleID);
		}

		public static void SaveZhuiJiaTimes()
		{
			int num = Global.GetZhuiJiaTimes();
			PlayerPrefs.SetInt("RedPointZhuiJia" + Global.Data.RoleID, ++num);
			MUDebug.Log<string>(new string[]
			{
				"装备追加次数 " + num
			});
			Global.GetRedPointCount(RedPointType.ZhuiJia, num);
		}

		public static int GetZhuiJiaTimes()
		{
			if (Global.IsSystemOpen(5))
			{
				int redPointCount = Global.GetRedPointCount(RedPointType.ZhuiJia, -1);
				return redPointCount + 1;
			}
			return PlayerPrefs.GetInt("RedPointZhuiJia" + Global.Data.RoleID);
		}

		public static void SavePeiYangTimes()
		{
			int num = Global.GetPeiYangTimes();
			PlayerPrefs.SetInt("RedPointPeiYang" + Global.Data.RoleID, ++num);
			MUDebug.Log<string>(new string[]
			{
				"装备培养次数 " + num
			});
			Global.GetRedPointCount(RedPointType.PeiYang, num);
		}

		public static int GetPeiYangTimes()
		{
			if (Global.IsSystemOpen(6))
			{
				int redPointCount = Global.GetRedPointCount(RedPointType.PeiYang, -1);
				return redPointCount + 1;
			}
			return PlayerPrefs.GetInt("RedPointPeiYang" + Global.Data.RoleID);
		}

		public static void ShowZhuangBeiPeiYangRedPoint()
		{
			ActivityTipManager.SetActivityTipItemActive(31003, Global.IsShowZhuangBeiPeiYangRedPoint());
		}

		private static bool IsShowZhuangBeiPeiYangRedPoint()
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuangBeiPeiYang))
			{
				return false;
			}
			LianluTypes lianluTypes = LianluTypes.ZhuangbeiXilian;
			RedPointVO dataByType = RedPointManager.GetDataByType(RedPointType.PeiYang);
			if (dataByType == null)
			{
				return false;
			}
			int num = Global.SafeConvertToInt32(dataByType.Parameter);
			if (Global.GetPeiYangTimes() >= num)
			{
				return false;
			}
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.roleData.GoodsDataList != null)
			{
				List<GoodsData> list2 = new List<GoodsData>();
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.GCount > 0)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
						if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
						{
							categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
							if (categoriyByGoodsID != 9 && categoriyByGoodsID != 10 && categoriyByGoodsID != 7)
							{
								list2.Add(goodsData);
							}
						}
					}
				}
				for (int j = 0; j < list2.Count; j++)
				{
					GoodsData goodsData = list2[j];
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if ((categoriyByGoodsID <= 6 || categoriyByGoodsID >= 10) && (categoriyByGoodsID <= 19 || categoriyByGoodsID == 21))
					{
						if (lianluTypes == LianluTypes.Qianghua)
						{
							if (goodsData.Forge_level >= Global.MaxForgeLevel)
							{
								goto IL_1C9;
							}
						}
						else if (lianluTypes == LianluTypes.Zhuijia)
						{
							if (goodsData.AppendPropLev >= 80)
							{
								goto IL_1C9;
							}
						}
						else if (lianluTypes == LianluTypes.ZhuangbeiXilian)
						{
							int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
							if (zhuoyueAttributeCount <= 0)
							{
								goto IL_1C9;
							}
						}
						if (goodsData.Using == 1)
						{
							list.Add(list2[j]);
						}
					}
					IL_1C9:;
				}
			}
			List<bool> list3 = new List<bool>();
			for (int k = 0; k < list.Count; k++)
			{
				list3.Add(Global.JudgePeiYangCaiLiaoEnough(list[k]));
			}
			return list3.Contains(true);
		}

		private static bool JudgePeiYangCaiLiaoEnough(GoodsData data)
		{
			string text = string.Empty;
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return false;
			}
			XElement xilianXmlNode = Global.GetXilianXmlNode(goodsXmlNodeByID.XiLian.ToString());
			int num = 0;
			int num2 = 0;
			text = string.Empty;
			text = Global.GetXElementAttributeStr(xilianXmlNode, "NeedGoods");
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					','
				});
				if (array != null && array.Length == 2)
				{
					num = array[0].SafeToInt32(0);
					num2 = array[1].SafeToInt32(0);
				}
			}
			int xelementAttributeInt = Global.GetXElementAttributeInt(xilianXmlNode, "NeedJinBi");
			int xelementAttributeInt2 = Global.GetXElementAttributeInt(xilianXmlNode, "NeedZuanShi");
			int equipGoodsSuitID = Global.GetEquipGoodsSuitID(data.GoodsID);
			int num3 = Global.GetTotalGoodsCountByID(num);
			int num4 = 0;
			bool flag = false;
			num4 += ConfigReplaceGoodVO.GetReplaceGoodCount(num, "EquipSuit", ref flag, (long)equipGoodsSuitID);
			num3 += num4;
			bool flag2 = false;
			bool flag3 = false;
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= xelementAttributeInt)
			{
				flag2 = true;
			}
			if (Global.Data.roleData.UserMoney >= xelementAttributeInt2)
			{
				flag3 = true;
			}
			return num3 >= num2 && true && (flag2 || flag3);
		}

		public static void ShowZhuangBeiZhuiJiaRedPoint()
		{
			ActivityTipManager.SetActivityTipItemActive(31002, Global.IsShowZhuangBeiZhuiJiaRedPoint());
		}

		private static bool IsShowZhuangBeiZhuiJiaRedPoint()
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuiJia))
			{
				return false;
			}
			LianluTypes lianluTypes = LianluTypes.Zhuijia;
			RedPointVO dataByType = RedPointManager.GetDataByType(RedPointType.ZhuiJia);
			if (dataByType == null)
			{
				return false;
			}
			int num = Global.SafeConvertToInt32(dataByType.Parameter);
			if (Global.GetZhuiJiaTimes() >= num)
			{
				return false;
			}
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.roleData.GoodsDataList != null)
			{
				List<GoodsData> list2 = new List<GoodsData>();
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.GCount > 0)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
						if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
						{
							categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
							if (categoriyByGoodsID != 9 && categoriyByGoodsID != 10 && categoriyByGoodsID != 7)
							{
								list2.Add(goodsData);
							}
						}
					}
				}
				for (int j = 0; j < list2.Count; j++)
				{
					GoodsData goodsData = list2[j];
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if ((categoriyByGoodsID <= 6 || categoriyByGoodsID >= 10) && (categoriyByGoodsID <= 19 || categoriyByGoodsID == 21))
					{
						if (lianluTypes == LianluTypes.Qianghua)
						{
							if (goodsData.Forge_level >= Global.MaxForgeLevel)
							{
								goto IL_1C9;
							}
						}
						else if (lianluTypes == LianluTypes.Zhuijia)
						{
							if (goodsData.AppendPropLev >= 80)
							{
								goto IL_1C9;
							}
						}
						else if (lianluTypes == LianluTypes.ZhuangbeiXilian)
						{
							int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
							if (zhuoyueAttributeCount <= 0)
							{
								goto IL_1C9;
							}
						}
						if (goodsData.Using == 1)
						{
							list.Add(list2[j]);
						}
					}
					IL_1C9:;
				}
			}
			List<bool> list3 = new List<bool>();
			for (int k = 0; k < list.Count; k++)
			{
				list3.Add(Global.JudgeZhuiJiaCaiLiaoEnough(list[k]));
			}
			return list3.Contains(true);
		}

		private static bool JudgeZhuiJiaCaiLiaoEnough(GoodsData data)
		{
			int zhuiJiaGoodsIDNums = Global.GetZhuiJiaGoodsIDNums(data);
			int zhuiJiaGoodsID = Global.GetZhuiJiaGoodsID(data);
			int equipGoodsSuitID = Global.GetEquipGoodsSuitID(data.GoodsID);
			GoodsData isBindGoodsDataByID = Global.GetIsBindGoodsDataByID(zhuiJiaGoodsID, false);
			if (isBindGoodsDataByID == null)
			{
				return false;
			}
			int appendPropLev = isBindGoodsDataByID.AppendPropLev;
			int num = 0;
			bool flag = false;
			num += ConfigReplaceGoodVO.GetReplaceGoodCount(isBindGoodsDataByID.GoodsID, "EquipSuit", ref flag, (long)equipGoodsSuitID);
			num += ConfigReplaceGoodVO.GetReplaceGoodCount(isBindGoodsDataByID.GoodsID, "ZhuiJiaLevel", ref flag, (long)appendPropLev);
			int num2 = Global.GetTotalGoodsCountByID(isBindGoodsDataByID.GoodsID);
			num2 += num;
			bool flag2 = num2 >= zhuiJiaGoodsIDNums;
			GoodsData isBindGoodsDataByID2 = Global.GetIsBindGoodsDataByID(zhuiJiaGoodsID, true);
			if (isBindGoodsDataByID2 == null)
			{
				return false;
			}
			bool flag3 = false;
			int appendPropLev2 = isBindGoodsDataByID2.AppendPropLev;
			int num3 = 0;
			num3 += ConfigReplaceGoodVO.GetReplaceGoodCount(isBindGoodsDataByID2.GoodsID, "EquipSuit", ref flag3, (long)equipGoodsSuitID);
			num3 += ConfigReplaceGoodVO.GetReplaceGoodCount(isBindGoodsDataByID2.GoodsID, "ZhuiJiaLevel", ref flag3, (long)appendPropLev2);
			int num4 = Global.GetTotalGoodsCountByID(isBindGoodsDataByID2.GoodsID);
			num4 += num3;
			bool flag4 = num4 >= zhuiJiaGoodsIDNums;
			bool flag5 = false;
			int num5 = Global.GetZhuijiaForgeLevelNeedMoney(data);
			num5 = Global.RecalcNeedYinLiang(num5);
			if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= num5)
			{
				flag5 = true;
			}
			return (flag4 || flag2) && flag5;
		}

		public static void ShowZhuangBeiQiangHuaRedPoint()
		{
			ActivityTipManager.SetActivityTipItemActive(31001, Global.IsShowZhuangBeiQiangHuaRedPoint());
		}

		private static bool IsShowZhuangBeiQiangHuaRedPoint()
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LianLu))
			{
				return false;
			}
			LianluTypes lianluTypes = LianluTypes.Qianghua;
			RedPointVO dataByType = RedPointManager.GetDataByType(RedPointType.QiangHua);
			if (dataByType == null)
			{
				return false;
			}
			int num = Global.SafeConvertToInt32(dataByType.Parameter);
			if (Global.GetQiangHuaTimes() >= num)
			{
				return false;
			}
			List<GoodsData> list = new List<GoodsData>();
			if (Global.Data.roleData.GoodsDataList != null)
			{
				List<GoodsData> list2 = new List<GoodsData>();
				for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = Global.Data.roleData.GoodsDataList[i];
					if (goodsData.GCount > 0)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
						if (categoriyByGoodsID >= 0 && categoriyByGoodsID < 25)
						{
							categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
							if (categoriyByGoodsID != 9 && categoriyByGoodsID != 10 && categoriyByGoodsID != 7)
							{
								list2.Add(goodsData);
							}
						}
					}
				}
				for (int j = 0; j < list2.Count; j++)
				{
					GoodsData goodsData = list2[j];
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if ((categoriyByGoodsID <= 6 || categoriyByGoodsID >= 10) && (categoriyByGoodsID <= 19 || categoriyByGoodsID == 21))
					{
						if (lianluTypes == LianluTypes.Qianghua)
						{
							if (goodsData.Forge_level >= Global.MaxForgeLevel)
							{
								goto IL_1C8;
							}
						}
						else if (lianluTypes == LianluTypes.Zhuijia)
						{
							if (goodsData.AppendPropLev >= 80)
							{
								goto IL_1C8;
							}
						}
						else if (lianluTypes == LianluTypes.ZhuangbeiXilian)
						{
							int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
							if (zhuoyueAttributeCount <= 0)
							{
								goto IL_1C8;
							}
						}
						if (goodsData.Using == 1)
						{
							list.Add(list2[j]);
						}
					}
					IL_1C8:;
				}
			}
			List<bool> list3 = new List<bool>();
			for (int k = 0; k < list.Count; k++)
			{
				list3.Add(Global.JudgeCaiLiaoEnough(list[k]));
			}
			return list3.Contains(true);
		}

		private static bool JudgeCaiLiaoEnough(GoodsData data)
		{
			int forgeLevel = data.Forge_level + 1;
			int[] forgeNeedGoodsList = Global.GetForgeNeedGoodsList(forgeLevel);
			int[] forgeNeedGoodsNumList = Global.GetForgeNeedGoodsNumList(forgeLevel);
			if (forgeNeedGoodsNumList == null)
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			if (forgeNeedGoodsList != null && forgeNeedGoodsList.Length > 0)
			{
				for (int i = 0; i < forgeNeedGoodsList.Length; i++)
				{
					int num3 = forgeNeedGoodsList[i];
					int num4 = forgeNeedGoodsNumList[i];
					int num5 = Global.GetTotalGoodsCountByID(num3);
					int equipGoodsSuitID = Global.GetEquipGoodsSuitID(data.GoodsID);
					int forge_level = data.Forge_level;
					int num6 = 0;
					bool flag = false;
					num6 += ConfigReplaceGoodVO.GetReplaceGoodCount(num3, "EquipSuit", ref flag, (long)equipGoodsSuitID);
					if (flag)
					{
					}
					num6 += ConfigReplaceGoodVO.GetReplaceGoodCount(num3, "QiangHuaLevel", ref flag, (long)forge_level);
					if (flag)
					{
					}
					num5 += num6;
					if (num5 < num4)
					{
						num++;
						break;
					}
					int num7 = Global.GetForgeNextLevelYinLiang(data);
					num7 = Global.RecalcNeedYinLiang(num7);
					if (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= num7)
					{
						num2++;
					}
				}
			}
			return num2 > 0 && num <= 0;
		}

		public static void ShowXingHunRedPoint(Dictionary<int, int> dicStarConstellation)
		{
			if (dicStarConstellation == null || dicStarConstellation.Count <= 0)
			{
				if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartXingZuo))
				{
					ActivityTipManager.SetActivityTipItemActive(32003, false);
					return;
				}
				bool flag = false;
				int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
				string xmlName = string.Format("Config/XingZuo/XingZuo_{0}.xml", num);
				XElement gameResXml = Global.GetGameResXml("Config/XingZuo/XingZuoType.xml");
				gameResXml = Global.GetGameResXml(xmlName);
				if (gameResXml != null)
				{
					List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "XingZuo"), "*");
					xelementList = Global.GetXElementList(gameResXml, "*");
					for (int i = 0; i < xelementList.Count; i++)
					{
						XElement xelement = xelementList[i];
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Title");
						List<XElement> xelementList2 = Global.GetXElementList(xelement, "*");
						if (Global.m_DicActiveStar.ContainsKey(i + 1))
						{
							int num2 = Global.m_DicActiveStar[i + 1];
						}
						for (int j = 0; j < xelementList2.Count; j++)
						{
							XElement xelement2 = xelementList2[j];
							string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "ID");
							string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement2, "NeedJinBi");
							flag = (Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= Convert.ToInt32(xelementAttributeStr3));
							string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement2, "XingHun");
							flag = (Global.Data.roleData.StarSoulValue >= Convert.ToInt32(xelementAttributeStr4));
							if (flag)
							{
								break;
							}
						}
						if (flag)
						{
							break;
						}
					}
				}
				ActivityTipManager.SetActivityTipItemActive(32003, flag);
			}
			else
			{
				ActivityTipManager.SetActivityTipItemActive(32003, Global.IsShowXingHunRedPoint(dicStarConstellation));
			}
		}

		private static bool IsShowXingHunRedPoint(Dictionary<int, int> dicStarConstellation)
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartXingZuo))
			{
				return false;
			}
			Global.xingHunCount = 0;
			RedPointVO dataByType = RedPointManager.GetDataByType(RedPointType.XingHun);
			if (dataByType == null)
			{
				return false;
			}
			Dictionary<int, int>.Enumerator enumerator = dicStarConstellation.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int num = Global.xingHunCount;
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				Global.xingHunCount = num + keyValuePair.Value;
			}
			if (Global.xingHunCount >= Global.SafeConvertToInt32(dataByType.Parameter))
			{
				return false;
			}
			int starSoulValue = Global.Data.roleData.StarSoulValue;
			if (starSoulValue <= 0)
			{
				return false;
			}
			Global.lstXingZuo.Clear();
			Global.dicStarConstellations.Clear();
			Global.dicStarConstellations = Global.FixedNode(dicStarConstellation);
			XElement gameResXml = Global.GetGameResXml("Config/XingZuo/XingZuoType.xml");
			if (gameResXml == null)
			{
				return false;
			}
			List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "XingZuo"), "*");
			Dictionary<string, XingZuo> dictionary = new Dictionary<string, XingZuo>();
			for (int i = 0; i < xelementList.Count; i++)
			{
				XElement xelement = xelementList[i];
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "ShuXiangJiaCheng");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "ID");
				string xelementAttributeStr3 = Global.GetXElementAttributeStr(xelement, "Name");
				string xelementAttributeStr4 = Global.GetXElementAttributeStr(xelement, "KaiQiLevel");
				string xelementAttributeStr5 = Global.GetXElementAttributeStr(xelement, "JiaChengBiLie");
				XingZuo xingZuo = new XingZuo();
				xingZuo.nXingZuoID = Convert.ToInt32(xelementAttributeStr2);
				xingZuo.strXingZuoName = xelementAttributeStr3;
				xingZuo.strKaiQiLevel = xelementAttributeStr4;
				xingZuo.strShuXingJiaCheng = xelementAttributeStr;
				xingZuo.strJiaChengBiLi = xelementAttributeStr5;
				dictionary.Add(xelementAttributeStr3, xingZuo);
				string[] array = xelementAttributeStr4.Split(new char[]
				{
					','
				});
				int nZhuanSheng = Convert.ToInt32(array[0]);
				int nLevel = Convert.ToInt32(array[1]);
				Global.SetXingZuoSate(nZhuanSheng, nLevel, xingZuo);
			}
			int num2 = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			string xmlName = string.Format("Config/XingZuo/XingZuo_{0}.xml", num2);
			gameResXml = Global.GetGameResXml(xmlName);
			if (gameResXml == null)
			{
				return false;
			}
			xelementList = Global.GetXElementList(gameResXml, "*");
			for (int j = 0; j < xelementList.Count; j++)
			{
				XElement xelement2 = xelementList[j];
				string xelementAttributeStr6 = Global.GetXElementAttributeStr(xelement2, "Title");
				List<XElement> xelementList2 = Global.GetXElementList(xelement2, "*");
				if (Global.m_DicActiveStar.ContainsKey(j + 1))
				{
					int num3 = Global.m_DicActiveStar[j + 1];
				}
				for (int k = 0; k < xelementList2.Count; k++)
				{
					XElement xelement3 = xelementList2[k];
					string xelementAttributeStr7 = Global.GetXElementAttributeStr(xelement3, "ID");
					string xelementAttributeStr8 = Global.GetXElementAttributeStr(xelement3, "NeedJinBi");
					string xelementAttributeStr9 = Global.GetXElementAttributeStr(xelement3, "ShuXing");
					string xelementAttributeStr10 = Global.GetXElementAttributeStr(xelement3, "XingHun");
					string xelementAttributeStr11 = Global.GetXElementAttributeStr(xelement3, "Succeed");
					string xelementAttributeStr12 = Global.GetXElementAttributeStr(xelement3, "LevelLimit");
					StarNode starNode = new StarNode();
					starNode.nXingWeiID = Convert.ToInt32(xelementAttributeStr7);
					starNode.strNeedGoods = xelementAttributeStr8;
					starNode.nXingHun = Convert.ToInt32(xelementAttributeStr10);
					starNode.strLevelLimit = xelementAttributeStr12;
					starNode.strProperty = xelementAttributeStr9;
					starNode.strSucceed = xelementAttributeStr11;
					dictionary[xelementAttributeStr6].LstNode.Add(starNode);
				}
			}
			List<bool> list = new List<bool>();
			if (Global.lstXingZuo != null && Global.lstXingZuo.Count >= 0)
			{
				for (int l = 0; l < Global.lstXingZuo.Count; l++)
				{
					list.Add(Global.JudgeCondition(Global.dicStarConstellations, dictionary, 2, l, Global.lstXingZuo[l].strXingZuoName));
				}
			}
			return list.Contains(true);
		}

		private static bool JudgeCondition(Dictionary<int, int> dicStarConstellationss, Dictionary<string, XingZuo> m_DicXingZuo, int nType, int m_nSelectIndex, string m_strCurrentXingZuoName)
		{
			int num = 0;
			if (dicStarConstellationss.ContainsKey(m_nSelectIndex + 1))
			{
				num = dicStarConstellationss[m_nSelectIndex + 1];
			}
			if (num >= Global.m_nMaxJieShu)
			{
				num--;
			}
			if (nType != 1)
			{
				return nType == 2 && Global.Data.roleData.StarSoulValue >= m_DicXingZuo[m_strCurrentXingZuoName].LstNode[num].nXingHun;
			}
			string strNeedGoods = m_DicXingZuo[m_strCurrentXingZuoName].LstNode[num].strNeedGoods;
			int num2 = Convert.ToInt32(strNeedGoods);
			return num2 <= 0 || num2 <= Global.Data.roleData.YinLiang + Global.Data.roleData.Money1;
		}

		private static Dictionary<int, int> FixedNode(Dictionary<int, int> dic)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (KeyValuePair<int, int> keyValuePair in dic)
			{
				if (keyValuePair.Value > Global.m_nMaxNode)
				{
					dictionary.Add(keyValuePair.Key, Global.m_nMaxNode);
				}
				else
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return dictionary;
		}

		public static void SetXingZuoSate(int nZhuanSheng, int nLevel, XingZuo xingZuo)
		{
			if (Global.Data.roleData.ChangeLifeCount > nZhuanSheng)
			{
				Global.lstXingZuo.Add(xingZuo);
			}
			else if (Global.Data.roleData.ChangeLifeCount == nZhuanSheng)
			{
				if (Global.Data.roleData.Level >= nLevel)
				{
					Global.lstXingZuo.Add(xingZuo);
				}
			}
		}

		public static void ShowChiBangRedPoint()
		{
			ActivityTipManager.SetActivityTipItemActive(32002, Global.IsShowChiBangRedPoint());
		}

		private static bool IsShowChiBangRedPoint()
		{
			if (!GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.GamePayerRolePartChiBang))
			{
				return false;
			}
			RedPointVO dataByType = RedPointManager.GetDataByType(RedPointType.ChiBang);
			if (dataByType == null)
			{
				return false;
			}
			WingData myWingData = Global.Data.roleData.MyWingData;
			if (myWingData == null)
			{
				return false;
			}
			int occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			bool flag = false;
			int num = myWingData.WingID * 1000 + myWingData.ForgeLevel;
			string[] array = dataByType.Parameter.Split(new char[]
			{
				','
			});
			if (array != null && array.Length >= 2)
			{
				int num2 = Global.SafeConvertToInt32(array[0]) * 1000 + Global.SafeConvertToInt32(array[1]);
				flag = (num < num2);
			}
			if (!flag)
			{
				return false;
			}
			int num3 = 0;
			int num4 = 0;
			if (myWingData.ForgeLevel != 10)
			{
				XElement wingStarXmlNode = Global.GetWingStarXmlNode(myWingData.WingID.ToString(), (myWingData.ForgeLevel + 1).ToString(), occupation);
				if (wingStarXmlNode != null)
				{
					string[] array2 = Global.GetXElementAttributeStr(wingStarXmlNode, "NeedGoods").Split(new char[]
					{
						','
					});
					num3 = Convert.ToInt32(array2[1]);
				}
				bool flag2 = false;
				int replaceGoodCount = ConfigReplaceGoodVO.GetReplaceGoodCount(2016, "WingSuit", ref flag2, (long)Global.Data.roleData.MyWingData.WingID);
				return num3 <= Global.GetTotalGoodsCountByID(2016) + replaceGoodCount;
			}
			XElement gameResXml = Global.GetGameResXml("Config/Wing/WingUp.xml");
			if (gameResXml == null)
			{
				return false;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Star", "Level", (myWingData.WingID + 1).ToString());
			if (xelement != null)
			{
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NeedGoods");
				string[] array3 = xelementAttributeStr.Split(new char[]
				{
					','
				});
				num4 = Convert.ToInt32(array3[1]);
			}
			bool flag3 = false;
			int replaceGoodCount2 = ConfigReplaceGoodVO.GetReplaceGoodCount(2017, "WingSuit", ref flag3, (long)Global.Data.roleData.MyWingData.WingID);
			return num4 <= Global.GetTotalGoodsCountByID(2017) + replaceGoodCount2;
		}

		private static XElement GetWingStarXmlNode(string id, string starid, int occupation)
		{
			XElement gameResXml = Global.GetGameResXml(string.Format("Config/Wing/WingStar_{0}.xml", occupation));
			if (gameResXml == null)
			{
				return null;
			}
			XElement xelement = Global.GetXElement(gameResXml, "Wing", "ID", id);
			if (xelement == null)
			{
				return null;
			}
			xelement = Global.GetXElement(xelement, "Item", "Star", starid);
			if (xelement == null)
			{
				return null;
			}
			return xelement;
		}

		public static void ShowSkillRedPoint()
		{
			ActivityTipManager.SetActivityTipItemActive(32001, Global.IsShowSkillRedPoint());
		}

		private static bool IsShowSkillRedPoint()
		{
			RedPointVO dataByType = RedPointManager.GetDataByType(RedPointType.JiNeng);
			if (dataByType == null)
			{
				return false;
			}
			List<SkillData> skillDataList = Global.Data.roleData.SkillDataList;
			if (skillDataList == null || skillDataList.Count <= 0)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < skillDataList.Count; i++)
			{
				num += skillDataList[i].SkillLevel;
			}
			if (num >= Global.SafeConvertToInt32(dataByType.Parameter))
			{
				return false;
			}
			List<SkillData> list = new List<SkillData>();
			List<MagicInfoVO> skillListByOccupation = Global.GetSkillListByOccupation();
			if (skillListByOccupation != null)
			{
				for (int j = 0; j < skillListByOccupation.Count; j++)
				{
					MagicInfoVO magicInfoVO = skillListByOccupation[j];
					int id = magicInfoVO.ID;
					int magicIcon = magicInfoVO.MagicIcon;
					if (magicIcon < 0)
					{
					}
					int actionIndex = magicInfoVO.ActionIndex;
					if (actionIndex < 1000)
					{
						int parentMagicID = magicInfoVO.ParentMagicID;
						if (parentMagicID <= 0)
						{
							list.Add(Global.GetSkillDataByID(id));
						}
					}
				}
			}
			if (list == null || list.Count <= 0)
			{
				return false;
			}
			int skillLevel = list[0].SkillLevel;
			int num2 = 0;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k] != null)
				{
					MagicItemVO magicItemVO = ConfigMagics.GetMagicItemVO(Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation), list[k].SkillID, list[k].SkillLevel);
					if (magicItemVO != null)
					{
						int num3 = (magicItemVO.ShuLianDu - list[k].UsedNum) / 5;
						if (num3 <= 0)
						{
							num3 = 0;
						}
						else if (num3 < 1)
						{
							num3 = 1;
						}
						if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TianDiJingYuan) >= num3 && Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 >= Convert.ToInt32(magicItemVO.NeedJinBi))
						{
							num2++;
						}
					}
				}
			}
			return num2 > 0;
		}

		private static List<MagicInfoVO> GetSkillListByOccupation()
		{
			if (!ConfigMagicInfos.IsValid())
			{
				return null;
			}
			List<MagicInfoVO> list = new List<MagicInfoVO>();
			foreach (KeyValuePair<int, MagicInfoVO> keyValuePair in ConfigMagicInfos.GetMaigcInfoVODict())
			{
				MagicInfoVO value = keyValuePair.Value;
				if (value.ToOcuupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation))
				{
					list.Add(value);
				}
			}
			return list;
		}

		public static bool GetNowServerIsZhuTiFu(ZtBuffServerInfo serverInf)
		{
			return serverInf != null && 10 != serverInf.nFirstLevelServerID;
		}

		public static bool GetNowServerIsZhuTiFu(int zoneId, out ZtBuffServerInfo serverInf)
		{
			serverInf = Global.FindServerInfo(zoneId);
			return serverInf != null && 10 != serverInf.nFirstLevelServerID;
		}

		public static ZtBuffServerInfo FindServerInfo(int serverID)
		{
			ZtBuffServerInfo result = null;
			if (Global.Data != null && Global.Data.ServerData != null && Global.Data.ServerData.ServerListData != null && Global.Data.ServerData.ServerListData.listServerData != null)
			{
				int count = Global.Data.ServerData.ServerListData.listServerData.Count;
				for (int i = 0; i < count; i++)
				{
					FistLevelServerListData fistLevelServerListData = Global.Data.ServerData.ServerListData.listServerData[i];
					if (Global.Data.ServerData.LastServer == null || fistLevelServerListData.nFirstLevelServerID == Global.Data.ServerData.LastServer.nFirstLevelServerID)
					{
						int count2 = fistLevelServerListData.listServerData.Count;
						for (int j = 0; j < count2; j++)
						{
							SecondLevelServerListData secondLevelServerListData = fistLevelServerListData.listServerData[j];
							int count3 = secondLevelServerListData.listServerData.Count;
							for (int k = 0; k < count3; k++)
							{
								if (secondLevelServerListData.listServerData[k].nServerID == serverID)
								{
									return secondLevelServerListData.listServerData[k];
								}
							}
						}
					}
				}
			}
			return result;
		}

		public static float[] GetHorseYinJiDecoH(int HorseID)
		{
			if (0 >= Global.mHorseYinJiDecorations.Count)
			{
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("HorseYinJiDecorations", '|');
				if (0 < systemParamStringArrayByName.Length)
				{
					for (int i = 0; i < systemParamStringArrayByName.Length; i++)
					{
						if (!string.IsNullOrEmpty(systemParamStringArrayByName[i]))
						{
							string[] array = systemParamStringArrayByName[i].Split(new char[]
							{
								','
							});
							if (array.Length == 4)
							{
								float num = 0f;
								float num2 = 0f;
								float num3 = 0f;
								float.TryParse(array[1], ref num);
								float.TryParse(array[2], ref num2);
								float.TryParse(array[3], ref num3);
								Global.mHorseYinJiDecorations[array[0].SafeToInt32(0)] = new float[]
								{
									num,
									num2,
									num3
								};
							}
						}
					}
				}
			}
			if (Global.mHorseYinJiDecorations.ContainsKey(HorseID))
			{
				return Global.mHorseYinJiDecorations[HorseID];
			}
			return new float[2];
		}

		public static float[] GetHorseDecoH(int HorseID)
		{
			if (0 >= Global.mHorseDecorations.Count)
			{
				string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("HorseDecorations", '|');
				if (0 < systemParamStringArrayByName.Length)
				{
					for (int i = 0; i < systemParamStringArrayByName.Length; i++)
					{
						if (!string.IsNullOrEmpty(systemParamStringArrayByName[i]))
						{
							string[] array = systemParamStringArrayByName[i].Split(new char[]
							{
								','
							});
							if (array.Length == 4)
							{
								float num = 0f;
								float num2 = 0f;
								float num3 = 0f;
								float.TryParse(array[1], ref num);
								float.TryParse(array[2], ref num2);
								float.TryParse(array[3], ref num3);
								Global.mHorseDecorations[array[0].SafeToInt32(0)] = new float[]
								{
									num,
									num2,
									num3
								};
							}
						}
					}
				}
			}
			if (Global.mHorseDecorations.ContainsKey(HorseID))
			{
				return Global.mHorseDecorations[HorseID];
			}
			return new float[2];
		}

		public static void IsShowGongNengYuGaoItemPart(bool isShow = false)
		{
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.gongNeItem != null)
			{
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.InActivityMap())
				{
					PlayZone.GlobalPlayZone.gongNeItem.Visibility = false;
				}
				else
				{
					PlayZone.GlobalPlayZone.gongNeItem.Visibility = isShow;
				}
			}
		}

		public static bool HaveDuoRenTeamData(int mapCode)
		{
			return Global.Data != null && Global.Data.CurrentCopyTeamData != null && Global.CancelZuDuiMap(mapCode);
		}

		private static bool CancelZuDuiMap(int mapCode)
		{
			return mapCode == 84000 || mapCode == 91000;
		}

		public static string XmlMoYu
		{
			get
			{
				return Global.xmlMoYu;
			}
			set
			{
				Global.xmlMoYu = value;
			}
		}

		public static Dictionary<int, ThemeActivityMoYu> DicThemeActivityNpc
		{
			get
			{
				if (Global.m_DicThemeActivityNpc.Count <= 0)
				{
					XElement xelement;
					if (!string.IsNullOrEmpty(Global.XmlMoYu))
					{
						xelement = XElement.Parse(Global.XmlMoYu);
					}
					else
					{
						xelement = Global.GetGameResXml("Config/ThemeActivityMoYu.xml");
					}
					List<XElement> xelementList = Global.GetXElementList(xelement, "ThemeActivityMoYu");
					for (int i = 0; i < xelementList.Count; i++)
					{
						ThemeActivityMoYu themeActivityMoYu = new ThemeActivityMoYu();
						themeActivityMoYu.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
						themeActivityMoYu.MonstersID = Global.GetXElementAttributeInt(xelementList[i], "MonstersID");
						themeActivityMoYu.MapId = Global.GetXElementAttributeInt(xelementList[i], "MapId");
						themeActivityMoYu.X = Global.GetXElementAttributeInt(xelementList[i], "X");
						themeActivityMoYu.Y = Global.GetXElementAttributeInt(xelementList[i], "Y");
						themeActivityMoYu.Radius = Global.GetXElementAttributeInt(xelementList[i], "Radius");
						themeActivityMoYu.NpcID = Global.GetXElementAttributeInt(xelementList[i], "NpcID");
						themeActivityMoYu.ChengJiuAward = Global.GetXElementAttributeStr(xelementList[i], "ChengJiuAward");
						themeActivityMoYu.ShengWangAward = Global.GetXElementAttributeStr(xelementList[i], "ShengWangAward");
						themeActivityMoYu.Hurt = Global.GetXElementAttributeInt(xelementList[i], "Hurt");
						if (!Global.m_DicThemeActivityNpc.ContainsKey(themeActivityMoYu.NpcID))
						{
							Global.m_DicThemeActivityNpc.Add(themeActivityMoYu.NpcID, themeActivityMoYu);
						}
					}
				}
				return Global.m_DicThemeActivityNpc;
			}
		}

		public static bool GetDataMoYuNpc(int NpcID)
		{
			return Global.DicThemeActivityNpc.ContainsKey(NpcID);
		}

		public static int ShiLianEndTime
		{
			get
			{
				return Global.ShiLianEndTime;
			}
			set
			{
				Global.ShiLianEndTime = value;
			}
		}

		public static string GetZhuTiFuBossTime(int MapId, int BossId)
		{
			string xmlName = StringUtil.substitute("Monsters.xml", new object[0]);
			XElement gameMapSettingsXml = Global.GetGameMapSettingsXml(MapId, xmlName);
			if (gameMapSettingsXml == null)
			{
				Super.HintMainText(Global.GetLang("地图加载配置错误MapID:" + MapId), 10, 3);
				return null;
			}
			List<XElement> xelementList = XmlManager.GetXElementList(gameMapSettingsXml, "Monster");
			for (int i = 0; i < xelementList.Count; i++)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[i], "Code");
				if (xelementAttributeInt == BossId)
				{
					string[] array = Global.GetXElementAttributeStr(xelementList[i], "TimePoints").Split(new char[]
					{
						'|'
					});
					for (int j = 0; j < array.Length; j++)
					{
						int num = array[j].Split(new char[]
						{
							':'
						})[0].SafeToInt32(0) * 60 + array[j].Split(new char[]
						{
							':'
						})[1].SafeToInt32(0);
						int num2 = Global.GetCorrectDateTime().Hour * 60 + Global.GetCorrectDateTime().Minute;
						if (num2 < num)
						{
							return array[j];
						}
						if (j >= array.Length - 1)
						{
							return array[j];
						}
					}
				}
			}
			return null;
		}

		public static string GetZhuTiFuNetImg(string newImg, string old)
		{
			string result = old;
			if (Global.IsInZhuTiFuActivity())
			{
				XElement gameResXml = Global.GetGameResXml("Config/ThemeActivityOpen.xml");
				if (gameResXml != null)
				{
					XElement xelement = Global.GetXElement(gameResXml, "ThemeActivityOpen");
					if (!string.IsNullOrEmpty(Global.GetXElementAttributeStr(xelement, newImg)))
					{
						if (newImg.Equals("Logo"))
						{
							result = "NetImages/GameRes/Images/Plate/" + Global.GetXElementAttributeStr(xelement, newImg);
						}
						else if (newImg.Equals("HuanYing"))
						{
							result = "NetImages/GameRes/Images/Plate/" + Global.GetXElementAttributeStr(xelement, newImg);
						}
						else if (newImg.Equals("Loading"))
						{
							result = "LoadGame/" + Global.GetXElementAttributeStr(xelement, newImg);
						}
						else if (newImg.Equals("Title"))
						{
							result = "NetImages/GameRes/Images/" + Global.GetXElementAttributeStr(xelement, newImg);
						}
					}
				}
			}
			return result;
		}

		public static bool GetZuanShi(ZuanShiPartClass classKey)
		{
			if (!Global.mDicZuanShi.ContainsKey((int)classKey))
			{
				Global.mDicZuanShi.Add((int)classKey, true);
				return true;
			}
			return Global.mDicZuanShi[(int)classKey];
		}

		public static bool ZuanShiIsCheck
		{
			get
			{
				return false;
			}
		}

		public static void SetZuanShi(ZuanShiPartClass classKey, bool flag)
		{
			if (Global.mDicZuanShi.ContainsKey((int)classKey))
			{
				Global.mDicZuanShi[(int)classKey] = flag;
			}
			else
			{
				Global.mDicZuanShi.Add((int)classKey, flag);
			}
		}

		public static string GetRoleBHName(RoleData roleData)
		{
			if (!Global.IsHavingBangHui(roleData))
			{
				return string.Empty;
			}
			string empty = string.Empty;
			if (!Global.IsHavingBangHui(roleData))
			{
				return string.Empty;
			}
			if (0 >= roleData.BHZhiWu)
			{
				return empty + roleData.BHName;
			}
			return empty + roleData.BHName + "·" + Global.GetBHZhiWu(roleData.BHZhiWu);
		}

		public static float GetBtnCD(int InstanceID)
		{
			if (0 < Global.BtnCDList.Count)
			{
				int i = Global.BtnCDList.Count - 1;
				while (i >= 0)
				{
					if (0f < Global.BtnCDList[i].CD)
					{
						if ((float)(Global.GetCorrectLocalTime() - Global.BtnCDList[i].CDBeginTicks) >= Global.BtnCDList[i].CD * 1000f)
						{
							Global.BtnCDList.RemoveAt(i);
							return -1f;
						}
						return (float)(Global.GetCorrectLocalTime() - Global.BtnCDList[i].CDBeginTicks) / 1000f;
					}
					else
					{
						Global.BtnCDList.RemoveAt(i);
						i--;
					}
				}
			}
			return -1f;
		}

		public static void AddBtnCD(int InstanceID, float CD = 1f)
		{
			int num = Global.BtnCDList.FindIndex((BtnCD e) => e.InstanceID == InstanceID);
			if (0 > num)
			{
				Global.BtnCDList.Add(new BtnCD(InstanceID, CD, Global.GetCorrectLocalTime()));
			}
		}

		public static void SetButtonSprite(GButton btn, string spriteName)
		{
			btn.normalSprite = spriteName;
			btn.hoverSprite = spriteName;
			btn.pressedSprite = spriteName;
			btn.target.spriteName = spriteName;
		}

		public static GoodsData GetGoodsDataByStr(string goodsStr, byte ContainsDouble = 0)
		{
			if (string.IsNullOrEmpty(goodsStr))
			{
				return null;
			}
			string[] array = goodsStr.Split(new char[]
			{
				','
			});
			if (array.Length == 1)
			{
				int goodsID = array[0].SafeToInt32(0);
				return Global.GetDummyGoodsData(goodsID);
			}
			if (array.Length != 7)
			{
				return null;
			}
			return Global.GetDummyGoodsDataMu(Convert.ToInt32(array[0]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[6]), Convert.ToInt32(array[5]), Convert.ToInt32(array[2]), (ContainsDouble != 0) ? 1 : Convert.ToInt32(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		}

		public static GGoodIcon LoadRewardItemGoodsIcon(string goodsStr, bool isOccupation = false, bool autoListen = true, bool activeBackground = true)
		{
			if (string.IsNullOrEmpty(goodsStr))
			{
				return null;
			}
			GoodsData goodsDataByStr = Global.GetGoodsDataByStr(goodsStr, 0);
			if (goodsDataByStr == null)
			{
				return null;
			}
			return Global.LoadRewardItemGoodsIcon(goodsDataByStr, autoListen);
		}

		public static GGoodIcon LoadRewardItemGoodsIconByGoodsID(int goodsId, bool autoListen = true)
		{
			GoodsData dummyGoodsData = Global.GetDummyGoodsData(goodsId);
			return Global.LoadRewardItemGoodsIcon(dummyGoodsData, autoListen);
		}

		public static GGoodIcon LoadRewardItemGoodsIcon(GoodsData goodData, bool autoListen = true)
		{
			if (goodData == null)
			{
				return null;
			}
			GGoodIcon ggoodIcon = Global.CreateGoodsIcon(goodData, false, true);
			if (autoListen && null != ggoodIcon)
			{
				ggoodIcon.addEventListener("click", new MouseEventHandler(Global.MouseLeftButtonUp));
			}
			return ggoodIcon;
		}

		public static GGoodIcon CreateGoodsIcon(GoodsData goodData, bool grayShow = false, bool activeBackground = true)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
			if (goodsXmlNodeByID == null)
			{
				return null;
			}
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string text = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = ((!activeBackground) ? null : text);
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodData.GoodsID;
			ggoodIcon.ItemObject = goodData;
			ggoodIcon.BoxTypes = -1;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(goodData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodData, canUse, IconTextTypes.Qianghua);
			return ggoodIcon;
		}

		private static void MouseLeftButtonUp(MouseEvent evt)
		{
			GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
			if (null == ggoodIcon)
			{
				return;
			}
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
		}

		public static byte[] NameLengthRange
		{
			get
			{
				if (Global.mNameLengthRange == null)
				{
					Global.mNameLengthRange = new byte[2];
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("NameLengthRange", ',');
					if (systemParamIntArrayByName.Length == 2)
					{
						Global.mNameLengthRange[0] = (byte)systemParamIntArrayByName[0];
						Global.mNameLengthRange[1] = (byte)systemParamIntArrayByName[1];
					}
				}
				return Global.mNameLengthRange;
			}
			set
			{
				Global.mNameLengthRange = value;
			}
		}

		public static byte CheckRoleNameLenght(string Text)
		{
			if (Text.Length < (int)Global.NameLengthRange[0])
			{
				return Global.NameLengthRange[0];
			}
			if (Text.Length > (int)Global.NameLengthRange[1])
			{
				return Global.NameLengthRange[1];
			}
			return 0;
		}

		public static void ClearXmlDataOnLoadConfig()
		{
			Global.NameLengthRange = null;
			Global.DicHorseEquipOpen.Clear();
			Global.m_DicUp.Clear();
			Global.m_DicStar.Clear();
			if (Global._TeShuTitleListXml != null)
			{
				Global._TeShuTitleListXml.Clear();
			}
			Global._WangZheZhanChangQiZuoDic.Clear();
			Global.fundSetXmlDic.Clear();
			Global.dic_CityInfo.Clear();
			Global.PataIndexRangeBySets = null;
			Global.IsLoadMingXiangExprInfo = false;
			Global.LangHunYaoSaiMonsters = null;
			Global.BloodCastleLingGuanIDs = null;
			Global.BloodCastleChengMenIDs = null;
			Global.zhuanhuangBuffIDs = null;
			Global._CachingTaskPlotItems = null;
			Global.PetExp = null;
			Global.MaxHavingSheLiZhiYuanSecs = 0;
			Global.LingDiIDs2MapCodes = null;
			Global.TaoZhuangVOList = null;
			Global.callMagicDict.Clear();
			Global.FilterFieldsDict = null;
			Global.mapCode_YanhuangZhanchang = 0;
			Global.GuMuMapList = null;
			Global.shaChengMapCode = 0;
			Global.palaceMapCode = 0;
			Global.mapCode_XuezhanDifu = 0;
			Global.ZhuanshengLevelAddAttackRates = null;
			Global.ZhuanshengLevelAddDefenseRates = null;
			Global.ZhuijiaLevelAddAttackRates = null;
			Global.ZhuijiaLevelAddDefenseRates = null;
			Global.ZhuoYueAddAttackRates = null;
			Global.ZhuoYueAddDefenseRates = null;
			Global.ForgeLevelAddAttackRates = null;
			Global.ForgeLevelAddDefenseRates = null;
			Global.ForgeLevelAddMaxLifeVRates = null;
			Global.ForgeLevelPet = null;
			Global.ChiBangForgeLevelAddDefenseRates = null;
			Global.ChiBangForgeLevelAddShangHaiJiaCheng = null;
			Global.ChiBangForgeLevelAddShangHaiXiShou = null;
			Global.EnchanceLevelAddRates = null;
			Global.mapTypeDict.Clear();
			Global.MapHelpHintDict.Clear();
			Global.AddNewPropsDict.Clear();
			Global.QianghuashiFenliMoney = null;
			Global.ForgeLevelRocksPercent = null;
			Global.ForgeLevelNeedYinLiang = null;
			Global.WingDict.Clear();
			Global.CanUseGoodsByTaskDict = null;
			Global.JuQingFuBenBossScale = null;
			Global.MonsterUnionLevel = null;
			Global.AutoFightSkillIDs = null;
			Global.BuffMagicDict = null;
			Global.MagicVDict.Clear();
			Global.TaskStarInfoDict = null;
			Global.FirstMainTaskID = 0;
			Global.riChangFuBenItems.Clear();
			Global.LuckyGoodsIDs = null;
			Global.ForgeGoodsList = null;
			Global.ForgeGoodsListDict.Clear();
			Global.ForgeGoodsNumList = null;
			Global.ForgeGoodsNumListDict.Clear();
			Global.ForgeGoodsIDs = null;
			Global.ChuanchengMoney = null;
			Global.ChuanchengYuanbao = null;
			Global.ChuanchengDiaojilv = null;
			Global.ZhuijiaChuanchengDiaojilv = null;
			Global.JuHunChuanchengDiaojilv = null;
			Global.ZhuijiaChuanchengMoney = null;
			Global.ZhuijiaChuanchengYuanbao = null;
			Global.XilianChuanchengDiaojilv = null;
			Global.XilianChuanchengMoney = null;
			Global.XilianChuanchengYuanbao = null;
			Global.juHunChuanchengMoney = null;
			Global.juHunChuanchengYuanbao = null;
			Global.ShengyoufuGoodsIDs = null;
			Global.ShengyoufuQianghuaDengji = null;
			Global.ZhuiJiaGoodsIDs = null;
			Global.ZhuiJiaGoodsIDNums = null;
			Global.ZhuiJiaForgeLuckyGoodsIDs = 0;
			Global.ZhuijiaForgeLevelNeedMoney = null;
			Global.ZhuijiaChenggonglvs = null;
			Global.ZhuanshengGoodsIDs = null;
			Global.ZhuanshengExpRates = null;
			Global.ZhuanshengLevelNeedMoney = null;
			Global.ZhuanshengChenggonglvs = null;
			Global.ZhuanshengBoliMoney = null;
			Global.ZhuanshengBoliYuanbao = null;
			Global.ZhuanshengBoliChenggonglvs = null;
			Global.XilianPropsUpFactorDict = null;
			Global.ShenqiZaizaoXmlDataDict = null;
			Global.LaoFangMapCode = -1;
			Global.ListIconCfgWndName = null;
			Global._ServerStartTime = new DateTime(2000, 1, 1);
			Global._ServerMergeTime = new DateTime(2000, 1, 1);
			Global._JieriTime = new DateTime(2000, 1, 1);
			Global._BuchangTime = new DateTime(2000, 1, 1);
			Global.IconsMinLevel = null;
			Global.HuodongIconEffectNeedRoleLevel = null;
			Global.YunChengGeMapCode = -1;
			Global.GoodsStrongDict.Clear();
			Global.DengluDaliStartTime = string.Empty;
			Global.DengluDaliEndTime = string.Empty;
			Global.CanGiveFakeEquips = false;
			Global.FakeEquipsIndex = 0;
			Global.ShowTeleport = false;
			Global.MaxShaodangNum = 0;
			Global.PataIndexRange = null;
			Global._QiRiKuangHuanTime = new DateTime(2000, 1, 1);
			Global.DicMonsterSite.Clear();
			Global.m_DicSkillURLID.Clear();
			Global.m_DicJingLingSkillName.Clear();
			Global.XElementxml_Magics = null;
			Global.m_DicJingLingSkillCD.Clear();
			Global.m_DicJingLingSkillMagicColor.Clear();
			Global.dicHorseEquipOpen.Clear();
			Global.requstRetainCount = 0;
			Global.tuJianRedDatas.Clear();
			Global.TujianListInBagDictByRedPoint = null;
			Global.TujiaXmlDataDictByRedPoint = null;
			Global.mHorseDecorations.Clear();
			Global.dic_holyPartAttr.Clear();
			Global.mHorseYinJiDecorations.Clear();
		}

		public static void ClearXmlDataOnChangeScene()
		{
			Global.m_DicThemeActivityNpc.Clear();
			Global.dic_SevenDayQiangGou.Clear();
			Global.dic_SevenDayGoal.Clear();
			if (Global._dic_diamond != null && Global._dic_diamond.Count > 0)
			{
				Global._dic_diamond.Clear();
			}
			Global.RongyaoBuffIDs = null;
			Global.ZhanhunBuffIDs = null;
			Global.JingMaiXueWeiDict.Clear();
			Global.WuxueBuffIDs = null;
			Global.LeagueWarDataDict.Clear();
			Global.fundxmlDic.Clear();
		}

		public static void ClearJieRiHuoDongConfig()
		{
			if (Global.Data != null)
			{
				Global.Data.JieriData = null;
			}
			Global.JieriXML_Version = 0;
		}

		internal static bool IsCompMiDongMap()
		{
			if (Global.Data != null && Global.Data.roleData != null)
			{
				SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
				if (settingMapVOByCode != null && settingMapVOByCode.MapType == 53)
				{
					return true;
				}
			}
			return false;
		}

		public static long GetNowDayFrom20111111()
		{
			DateTime correctDateTime = Global.GetCorrectDateTime();
			DateTime dateTime;
			dateTime..ctor(2011, 11, 11);
			return (long)(correctDateTime - dateTime).TotalDays;
		}

		public static bool IsRebornEquip(int type)
		{
			return ChongShengData.IsChongShengEquip(type);
		}

		public static bool IsRebornBaoShi(int type)
		{
			return type == 950;
		}

		public static bool IsRebornXuanCai(int type)
		{
			return type == 960;
		}

		public static bool IsRebornGood(GoodVO good)
		{
			return good != null && good.RebornEquip == 1;
		}

		public static bool IsNeedSaleRebornTiShi(GoodsData gd)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			if (!Global.IsRebornGood(goodsXmlNodeByID))
			{
				return false;
			}
			if (Global.IsRebornEquip(goodsXmlNodeByID.Categoriy))
			{
				if (Global.GetZhuoyueAttributeCount(gd) > 5)
				{
					return true;
				}
			}
			else if (Global.IsRebornBaoShi(goodsXmlNodeByID.Categoriy))
			{
				if (goodsXmlNodeByID.SuitID > 5)
				{
					return true;
				}
			}
			else if (Global.IsRebornXuanCai(goodsXmlNodeByID.Categoriy) && goodsXmlNodeByID.SuitID > 3)
			{
				return true;
			}
			return false;
		}

		public static bool IsChongShengOpen()
		{
			return ChongShengData.IsChongShengOpen();
		}

		public static string GetString(params object[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				stringBuilder.Append(args[i]);
			}
			return stringBuilder.ToString();
		}

		public static bool IsTuiGuangFenBao
		{
			get
			{
				return Global.FenBaoType == FenBaoDownloadType.TuiGuang;
			}
		}

		public static string GetRechargeItemCfgTypeByPlatform()
		{
			string result = "dl_app";
			if (Context.IsHaiwai)
			{
				result = "dl_app";
			}
			return result;
		}

		public static void LoadReward(List<string> rewardStr, UIGrid itemContainer, float size = 64f, bool beDragable = true)
		{
			for (int i = 0; i < rewardStr.Count; i++)
			{
				GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(rewardStr[i], false, true, true);
				if (!(ggoodIcon == null))
				{
					if (beDragable)
					{
						ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
					}
					ggoodIcon.transform.SetParent(itemContainer.transform);
					ggoodIcon.isAutoSize = false;
					ggoodIcon.Width = (double)size;
					ggoodIcon.Height = (double)size;
					ggoodIcon.transform.localPosition = Vector3.zero;
					ggoodIcon.transform.localScale = Vector3.one;
				}
			}
			itemContainer.Reposition();
		}

		public static void LoadReward(List<GoodsData> rewardStr, UIGrid itemContainer, float size = 64f, bool beDragable = true)
		{
			for (int i = 0; i < rewardStr.Count; i++)
			{
				GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIcon(rewardStr[i], true);
				if (!(ggoodIcon == null))
				{
					if (beDragable)
					{
						ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
					}
					ggoodIcon.transform.SetParent(itemContainer.transform);
					ggoodIcon.isAutoSize = false;
					ggoodIcon.Width = (double)size;
					ggoodIcon.Height = (double)size;
					ggoodIcon.transform.localPosition = Vector3.zero;
					ggoodIcon.transform.localScale = Vector3.one;
				}
			}
			itemContainer.Reposition();
		}

		public static bool HasReturn()
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("GoogleReturn", true);
			MUDebug.Log<string>(new string[]
			{
				"strReturn" + systemParamByName
			});
			return !string.IsNullOrEmpty(systemParamByName) && systemParamByName == "1";
		}

		public static bool HasReturnPingTaiName()
		{
			string systemParamByName = ConfigSystemParam.GetSystemParamByName("GoogleReturn_PlatName", true);
			MUDebug.Log<string>(new string[]
			{
				"strPlatName === " + systemParamByName + "PlatSDKMgr.PlatName===" + PlatSDKMgr.PlatName
			});
			if (!string.IsNullOrEmpty(systemParamByName) && systemParamByName.Contains(PlatSDKMgr.PlatName))
			{
				MUDebug.Log<string>(new string[]
				{
					"HasReturnPingTaiName()====PlatSDKMgr.PlatName" + PlatSDKMgr.PlatName
				});
				return true;
			}
			return false;
		}

		public static string LoadGamePicName()
		{
			if (Context.IsAPPVerify)
			{
				return "LoadGame/12.jpg";
			}
			return "LoadGame/11.jpg";
		}

		public static void LoadLangDict(bool isNotCN = false)
		{
			try
			{
				if (Global.LangDict != null && Global.LangDict.Count > 0)
				{
					Global.LangDict.Clear();
				}
				XElement xelement = Global.GetGameResXml("Config/Language.xml");
				if (xelement == null)
				{
					xelement = Global.GetXmlFromResource("Language.xml");
				}
				if (xelement != null)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						string xelementAttributeStr = Global.GetXElementAttributeStr(xelement2, "ChineseText");
						if (!string.IsNullOrEmpty(xelementAttributeStr))
						{
							string text = Global.StringReplaceAll(xelementAttributeStr, "\\n", "_");
							text = Global.StringReplaceAll(text, "\\t", "_");
							text = Global.StringReplaceAll(text, "\\r", "_");
							string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement2, "OtherLangText");
							dictionary[text] = xelementAttributeStr2;
						}
					}
					Global.LangDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public static XElement GetXmlFromResource(string xmlName)
		{
			try
			{
				xmlName = xmlName.Remove(xmlName.LastIndexOf("."));
				TextAsset textAsset = Resources.Load(xmlName, typeof(TextAsset)) as TextAsset;
				if (null == textAsset)
				{
					return null;
				}
				return XElement.Parse(textAsset.text);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			return null;
		}

		public static string GetLang(string chineseText)
		{
			if (MUDebug.IsOpenDebug)
			{
				return chineseText;
			}
			if (Global.LangDict == null)
			{
				return chineseText;
			}
			string text = Global.StringReplaceAll(chineseText, "\\n", "_");
			text = Global.StringReplaceAll(text, "\\t", "_");
			text = Global.StringReplaceAll(text, "\r", "_");
			text = Global.StringReplaceAll(text, "\n", "_");
			string text2 = null;
			if (Global.LangDict.ContainsKey(text))
			{
				text2 = Global.LangDict[text];
			}
			if (text2 == null)
			{
				return chineseText;
			}
			if (string.IsNullOrEmpty(text2))
			{
				return chineseText;
			}
			text2 = Global.StringReplaceAll(text2, "\\n", "\n");
			text2 = Global.StringReplaceAll(text2, "\\t", "\t");
			return Global.StringReplaceAll(text2, "\\r", "\r");
		}

		public static string GetLang(string chineseText, string color)
		{
			if (Global.LangDict == null)
			{
				return ColorCode.EncodingText(chineseText, color);
			}
			string text = Global.StringReplaceAll(chineseText, "\\n", "_");
			text = Global.StringReplaceAll(text, "\\t", "_");
			text = Global.StringReplaceAll(text, "\\r", "_");
			string text2 = Global.LangDict[text];
			if (text2 == null)
			{
				return ColorCode.EncodingText(chineseText, color);
			}
			if (string.IsNullOrEmpty(text2))
			{
				return ColorCode.EncodingText(chineseText, color);
			}
			text2 = Global.StringReplaceAll(text2, "\\n", "\n");
			text2 = Global.StringReplaceAll(text2, "\\t", "\t");
			text2 = Global.StringReplaceAll(text2, "\\r", "\r");
			return ColorCode.EncodingText(text2, color);
		}

		public static List<string> GetShowGoodsIds(List<string> goodsOne, List<string> goodsTwo)
		{
			List<string> list = new List<string>(goodsOne);
			int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
			for (int i = 0; i < goodsTwo.Count; i++)
			{
				string[] array = goodsTwo[i].Split(new char[]
				{
					','
				});
				if (!MUJieripartChongzhiKingItem.IsTongGuo(array[0], roleOcc))
				{
					list.Add(goodsTwo[i]);
					break;
				}
			}
			return list;
		}

		public static int HuoDongPageById(RiChangHuoDongTypes types)
		{
			if (Global.dicHuoDongPage == null || Global.dicHuoDongPage.Count <= 0)
			{
				Global.dicHuoDongPage = new Dictionary<RiChangHuoDongTypes, int>();
				XElement gameResXml = Global.GetGameResXml("Config/HuoDongTab.Xml");
				if (gameResXml != null)
				{
					List<XElement> xelementList = Global.GetXElementList(gameResXml, "HuoDong");
					for (int i = 0; i < Enumerable.Count<XElement>(xelementList); i++)
					{
						Global.dicHuoDongPage.Add((RiChangHuoDongTypes)Global.GetXElementAttributeInt(xelementList[i], "ID"), i + 1);
					}
				}
			}
			if (Global.dicHuoDongPage.ContainsKey(types))
			{
				return Global.dicHuoDongPage[types];
			}
			return 0;
		}

		public static void SetSprite(GameObject gameObject)
		{
			if (Context.IsHaiwai)
			{
				UISprite[] componentsInChildren = gameObject.GetComponentsInChildren<UISprite>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].gameObject.name.Equals("Sprite"))
					{
						componentsInChildren[i].spriteName = "tongyongdikuangbiaoti";
						break;
					}
				}
			}
		}

		public const string GAME_CONFIG_SYSCHAT_FILE = "Config/AutoSystemChat.xml";

		public const string GAME_CONFIG_SYSCHAT_NAME = "ConfigAutoSystemChat";

		public const string GAME_CONFIG_MAGICATIONS_FILE = "Config/Magications.Xml";

		public const string GAME_CONFIG_MAGICATIONS_NAME = "ConfigMagications";

		public const string GAME_CONFIG_LEVELUP_FILE = "Config/LevelUp.Xml";

		public const string GAME_CONFIG_LEVELUP_NAME = "ConfigLevelUp";

		public const string GAME_CONFIG_OPERATIONS_FILE = "Config/SystemOperations.Xml";

		public const string GAME_CONFIG_OPERATIONS_NAME = "ConfigSystemOperations";

		public const string GAME_CONFIG_NPCSCRIPTS_FILE = "Config/NPCScripts.Xml";

		public const string GAME_CONFIG_NPCSCRIPTS_NAME = "ConfigNPCScripts";

		public const string GAME_CONFIG_JUNQI_FILE = "Config/JunQi.Xml";

		public const string GAME_CONFIG_JUNQI_NAME = "ConfigJunQi";

		public const string GAME_CONFIG_NPCSALELIST_FILE = "Config/NPCSaleList.Xml";

		public const string GAME_CONFIG_NPCSALELIST_NAME = "ConfigNPCSaleList";

		public const string GAME_CONFIG_SYSHELP_FILE = "Config/SystemHelp.Xml";

		public const string GAME_CONFIG_SYSHELP_NAME = "ConfigSystemHelp";

		public const string GAME_CONFIG_SYSTEMNAVI_FILE = "Config/SystemNavi.Xml";

		public const string GAME_CONFIG_SYSTEMNAVI_NAME = "ConfigSystemNavi";

		public const string GAME_CONFIG_PET_FILE = "Config/Pet.Xml";

		public const string GAME_CONFIG_PET_LEVEL = "Config/PetLevelUp.Xml";

		public const string GAME_CONFIG_PET_SCALE = "Config/PetHabitus.Xml";

		public const string GAME_CONFIG_PET_NAME = "ConfigPet";

		public const string GAME_CONFIG_ZLTYPE = "Config/ZhuLingType.xml";

		public const string GAME_CONFIG_ZLWIN = "Config/WinZhuLing.xml";

		public const string GAME_CONFIG_ZLMAX = "Config/MaxWinZhuLing.xml";

		public const string GAME_CONFIG_SIEGEWAR_FILE = "Config/SiegeWarfare.Xml";

		public const string GAME_CONFIG_SIEGEAWR_REWARD_FILE = "Config/SiegeWarfareEveryDayAward.xml";

		public const string GAME_CONFIG_SIEGEWAR_FLAG_FILE = "Config/QiZuoConfig.xml";

		public const string GAME_CONFIG_FASHION_FILE = "Config/Fashion.xml";

		public const string GAME_CONFIG_FASHION_TAB_FILE = "Config/FashionTab";

		public const string GAME_CONFIG_MERGETYPES_FILE = "Config/GoodsMergeType.Xml";

		public const string GAME_CONFIG_MERGETYPES_NAME = "ConfigGoodsMergeType";

		public const string GAME_CONFIG_MERGEITEMS_FILE = "Config/GoodsMergeItems.Xml";

		public const string GAME_CONFIG_MERGEITEMS_NAME = "ConfigGoodsMergeItems";

		public const string GAME_CONFIG_TIPS_FILE = "Config/SystemTips.Xml";

		public const string GAME_CONFIG_TIPS_NAME = "ConfigSystemTips";

		public const string GAME_CONFIG_PARAMS_FILE = "Config/SystemParams.Xml";

		public const string GAME_CONFIG_PARAMS_NAME = "ConfigSystemParams";

		public const string GAME_CONFIG_REPLACE_GOOD_FILE = "Config/ReplaceGoods.Xml";

		public const string GAME_CONFIG_REPLACE_GOOD_NAME = "ReplaceGoods";

		public const string GAME_CONFIG_TASKTALKS_FILE = "Config/TaskTalks.Xml";

		public const string GAME_CONFIG_TASKTALKS_NAME = "ConfigTaskTalks";

		public const string GAME_CONFIG_FUBEN_FILE = "Config/FuBen.Xml";

		public const string GAME_CONFIG_FUBEN_NAME = "ConfigFuBen";

		public const string GAME_CONFIG_TASKSTARINFOS_FILE = "Config/TaskStarInfos.xml";

		public const string GAME_CONFIG_TASKSTARINFOS_NAME = "ConfigTaskStarInfos";

		public const string GAME_CONFIG_ACTIVITY_BOSSINFO_FILE = "Config/Activity/BossInfo.Xml";

		public const string GAME_CONFIG_ACTIVITY_BOSSINFO_NAME = "ConfigActivityBossInfo.Xml";

		public const string GAME_CONFIG_FUBENMAP_FILE = "Config/FuBenMap.Xml";

		public const string GAME_CONFIG_FUBENMAP_NAME = "ConfigFuBenMap";

		public const string GAME_CONFIG_FUBENTAB_FILE = "Config/FuBenTab.Xml";

		public const string GAME_CONFIG_FUBENTAB_NAME = "ConfigFuBenTab";

		public const string GAME_CONFIG_FUBENPINGFEN_FILE = "Config/FuBenPingFen.Xml";

		public const string GAME_CONFIG_FUBENPINGFEN_NAME = "ConfigFuBenPingFen";

		public const string GAME_CONFIG_HUODONGTAB_FILE = "Config/HuoDongTab.Xml";

		public const string GAME_CONFIG_HUODONGTAB_NAME = "ConfigHuoDongTab";

		public const string GAME_CONFIG_BLOODCASTLE_FILE = "Config/BloodCastleInfo.xml";

		public const string GAME_CONFIG_BLOODCASTLE_NAME = "ConfigBloodCastleInfo";

		public const string GAME_CONFIG_FRESHPLAYERCOPYSCENEINFO_FILE = "Config/FreshPlayerCopySceneInfo.xml";

		public const string GAME_CONFIG_FRESHPLAYERCOPYSCENEINFO_NAME = "ConfigFreshPlayerCopySceneInfo";

		public const string GAME_CONFIG_SHENGBEI_FILE = "Config/HolyGrail.xml";

		public const string GAME_CONFIG_SHENGBEI_NAME = "Config/HolyGrail";

		public const string GAME_CONFIG_DEMON_FILE = "Config/Demon.xml";

		public const string GAME_CONFIG_DEMON_NAME = "ConfigDemon";

		public const string GAME_CONFIG_HUANGJIN_FILE = "Config/HuangJin.xml";

		public const string GAME_CONFIG_HUANGJIN_NAME = "ConfigHuangJin";

		public const string GAME_CONFIG_BATTLE_FILE = "Config/Battle.xml";

		public const string GAME_CONFIG_BATTLE_NAME = "ConfigBattle";

		public const string GAME_CONFIG_PKKING_FILE = "Config/ArenaBattle.xml";

		public const string GAME_CONFIG_PKKING_NAME = "ConfigArenaBattle";

		public const string GAME_CONFIG_ANGELTEMPLE_FILE = "Config/AngelTemple.xml";

		public const string GAME_CONFIG_ANGELTEMPLE = "ConfigAngelTemple";

		public const string GAME_CONFIG_VIPTAB_FILE = "Config/VipTab.Xml";

		public const string GAME_CONFIG_VIPTAB_NAME = "ConfigVipTab";

		public const string GAME_CONFIG_BOSSZHIJIA_FILE = "Config/BossZhiJia.Xml";

		public const string GAME_CONFIG_BOSSZHIJIA_NAME = "ConfigBossZhiJia";

		public const string GAME_CONFIG_HUANGJINSHENGDIAN_FILE = "Config/HuangJinShengDian.Xml";

		public const string GAME_CONFIG_HUANGJINSHENGDIAN_NAME = "ConfigHuangJinShengDian";

		public const string GAME_CONFIG_MONSTERGOODSLIST_FILE = "Config/Activity/MonsterGoodsList.xml";

		public const string GAME_CONFIG_MONSTERGOODSLIST_NAME = "ConfigMonsterGoodsList";

		public const string GAME_CONFIG_EXCELLENCEPROPERTYLIST_FILE = "Config/ExcellencePropertyRandom.xml";

		public const string GAME_CONFIG_EXCELLENCEPROPERTYLIST_NAME = "ExcellencePropertyList";

		public const string GAME_CONFIG_QUALITYUP_FILE = "Config/QualityUp.Xml";

		public const string GAME_CONFIG_QUALITYUP_NAME = "ConfigQualityUp";

		public const string GAME_CONFIG_EQUIPUPGRADE_FILE = "Config/EquipUpgrade.Xml";

		public const string GAME_CONFIG_EQUIPUPGRADE_NAME = "ConfigEquipUpgrade";

		public const string GAME_CONFIG_EQUIPUPGRADE_FILE_MU = "Config/MuEquipUp.xml";

		public const string GAME_CONFIG_EQUIPUPGRADE_NAME_MU = "ConfigMuEquipUp";

		public const string GAME_CONFIG_EQUIPXILIANSHUXING_FILE_MU = "Config/XiLianShuXing.xml";

		public const string GAME_CONFIG_EQUIPXILIANSHUXING_NAME_MU = "ConfigXiLianShuXing";

		public const string GAME_CONFIG_EQUIPXILIANTYPE_FILE_MU = "Config/XiLianType.xml";

		public const string GAME_CONFIG_EQUIPXILIANTYPE_NAME_MU = "ConfigXiLianType";

		public const string GAME_CONFIG_QIANGHUA_FILE = "Config/QiangHua.Xml";

		public const string GAME_CONFIG_QIANGHUA_NAME = "ConfigQiangHua";

		public const string GAME_CONFIG_EQUIPZAIZAO_FILE_MU = "Config/ZaiZao.xml";

		public const string GAME_CONFIG_EQUIPZAIZAO_NAME_MU = "ConfigZaiZao";

		public const string GAME_CONFIG_EQUIPTAOZHUANGPROPS_FILE_MU = "Config/TaoZhuangProps.xml";

		public const string GAME_CONFIG_EQUIPTAOZHUANGPROPS_NAME_MU = "ConfigTaoZhuangProps";

		public const string GAME_CONFIG_JINGMAI_FILE = "Config/JingMai.Xml";

		public const string GAME_CONFIG_JINGMAI_NAME = "ConfigJingMai";

		public const string GAME_CONFIG_WUXUE_FILE = "Config/WuXue.Xml";

		public const string GAME_CONFIG_WUXUE_NAME = "ConfigWuXue";

		public const string GAME_CONFIG_WNDCFG_FILE = "Config/WndCfg.Xml";

		public const string GAME_CONFIG_WNDCFG_NAME = "ConfigWndCfg";

		public const string GAME_CONFIG_BORNNAME_FILE = "Config/BornName.Xml";

		public const string GAME_CONFIG_BORNNAME_NAME = "ConfigBornName";

		public const string GAME_CONFIG_EQUIPBORN_FILE = "Config/EquipBorn.Xml";

		public const string GAME_CONFIG_EQUIPBORN_NAME = "ConfigEquipBorn";

		public const string GAME_CONFIG_ACTIVITYTIP_FILE = "Config/Activity/ActivityTip.Xml";

		public const string GAME_CONFIG_ACTIVITYTIP_NAME = "ConfigActivityActivityTip";

		public const string GAME_CONFIG_LUCKYAWARD_FILE = "Config/LuckyAward.Xml";

		public const string GAME_CONFIG_LUCKYAWARD_NAME = "ConfigLuckyAward";

		public const string GAME_CONFIG_LUCKAWARD2_FILE = "Config/LuckyAward2.Xml";

		public const string GAME_CONFIG_LUCKAWARD2_NAME = "ConfigLuckyAward2";

		public const string GAME_CONFIG_CHENGJIUFUWEN_FILE = "Config/ChengJiuFuWen.Xml";

		public const string GAME_CONFIG_CHENGJIUFUWEN_NAME = "ConfigChengJiuFuWen";

		public const string GAME_CONFIG_CHENGJIUATTRIBUTE_FILE = "Config/ChengJiuSpecialAttribute.Xml";

		public const string GAME_CONFIG_CHENGJIUATTRIBUTE_NAME = "ConfigChengJiuSpecialAttribute";

		public const string GAME_CONFIG_TIANFUPROPERTY_0_FILE = "Config/TianFuProperty_0.Xml";

		public const string GAME_CONFIG_TIANFUPROPERTY_0_NAME = "ConfigTianFuProperty_0";

		public const string GAME_CONFIG_TIANFUPROPERTY_1_FILE = "Config/TianFuProperty_1.Xml";

		public const string GAME_CONFIG_TIANFUPROPERTY_1_NAME = "ConfigTianFuProperty_1";

		public const string GAME_CONFIG_TIANFUPROPERTY_2_FILE = "Config/TianFuProperty_2.Xml";

		public const string GAME_CONFIG_TIANFUPROPERTY_2_NAME = "ConfigTianFuProperty_2";

		public const string GAME_CONFIG_TIANFUPROPERTY_3_FILE = "Config/TianFuProperty_3.Xml";

		public const string GAME_CONFIG_TIANFUPROPERTY_3_NAME = "ConfigTianFuProperty_3";

		public const string GAME_CONFIG_TIANFUPROPERTY_5_FILE = "Config/TianFuProperty_5.Xml";

		public const string GAME_CONFIG_TIANFUPROPERTY_5_NAME = "ConfigTianFuProperty_5";

		public const string GAME_CONFIG_TIANFUDIAN_FILE = "Config/TianFuDian.Xml";

		public const string GAME_CONFIG_TIANFUDIAN_NAME = "ConfigTianFuDian";

		public const string GAME_CONFIG_TIANFUGROUPPROPERTY_FILE = "Config/TianFuGroupProperty.Xml";

		public const string GAME_CONFIG_TIANFUGROUPPROPERTY_NAME = "ConfigTianFuGroupProperty";

		public const string GAME_CONFIG_HUOYUEINFO_FILE = "Config/DailyActiveInfor.Xml";

		public const string GAME_CONFIG_HUOYUEINFO_NAME = "ConfigDailyActiveInfor";

		public const string GAME_CONFIG_HUOYUEKA_FILE = "Config/Activity/Card.Xml";

		public const string GAME_CONFIG_WEDDINGRING = "Config/WeddingRing.xml";

		public const string GAME_CONFIG_HUOYUEAWARD_FILE = "Config/DailyActiveAward.Xml";

		public const string GAME_CONFIG_HUOYUEAWARD_NAME = "ConfigDailyActiveAward";

		public const string GAME_CONFIG_CHENGJIU_BUFFER_FILE = "Config/ChengJiuBuff.Xml";

		public const string GAME_CONFIG_CHENGJIU_BUFFER_NAME = "ConfigChengJiuBuff";

		public const string GAME_CONFIG_TASKPLOT_FILE = "Config/TaskPlot.Xml";

		public const string GAME_CONFIG_TASKPLOT_NAME = "ConfigTaskPlot";

		public const string GAME_CONFIG_UPLEVELGIFT_FILE = "Config/Gifts/UpLevelGift.xml";

		public const string GAME_CONFIG_RONGYAO_FILE = "Config/RongYu.xml";

		public const string GAME_CONFIG_CHENGJIUTAB_FILE = "Config/ChengJiuTab.xml";

		public const string GAME_CONFIG_ZHUANZHI_FILE = "Config/Roles/ZhuanZhi.xml";

		public const string GAME_CONFIG_ZHUANSHENG_FILE = "Config/Roles/ZhuanSheng_{0}.xml";

		public const string GAME_CONFIG_OCCUPATIONADDPOINT_FILE = "Config/Roles/OccupationAddPoint.xml";

		public const string GAME_CONFIG_REBORNCOMBATFORCEINFO_FILE = "Config/Roles/RebornCombatForce.xml";

		public const string GAME_CONFIG_COMBATFORCEINFO_FILE = "Config/Roles/CombatForceInfo.xml";

		public const string GAME_CONFIG_JINGJI_FILE = "Config/JingJi.xml";

		public const string GAME_CONFIG_JINGJICONFIG_FILE = "Config/JingJiConfig.xml";

		public const string GAME_CONFIG_JUNXIAN_FILE = "Config/JunXian.xml";

		public const string GAME_CONFIG_SOCIALACT_FILE = "Config/SocialAct.xml";

		public const string GAME_CONFIG_SHADERS_FILE = "Config/Shaders.xml";

		public const string GAME_CONFIG_SHADERS_NAME = "ConfigShaders";

		public const string GAME_CONFIG_FASHIIONSHADERS_FILE = "Config/ShiZhuangShaders.xml";

		public const string GAME_CONFIG_MINGXIANG_FILE = "Config/MingXiang.xml";

		public const string GAME_CONFIG_MINGXIANG_NAME = "ConfigMingXiang";

		public const string GAME_CONFIG_ZHANMENG_BUILD_FILE = "Config/ZhanMengBuild.xml";

		public const string GAME_CONFIG_ZHANMENG_BUILD_NAME = "ConfigZhanMengBuild";

		public const string GAME_CONFIG_BAODIAN_CONTENT_FILE = "Config/BaoDian.xml";

		public const string GAME_CONFIG_BAODIAN_CONTENT_NAME = "ConfigBaoDian";

		public const string GAME_CONFIG_BAODIAN_TAB_FILE = "Config/BaoDianTab.xml";

		public const string GAME_CONFIG_BAODIAN_TAB_NAME = "ConfigBaoDianTab";

		public const string GAME_CONFIG_GONGGAO_FILE = "Config/Gonggao.xml";

		public const string GAME_CONFIG_GONGGAO_Name = "Config/Gonggao";

		public const string GAME_CONFIG_SHARE_FILE = "Config/Share.xml";

		public const string GAME_CONFIG_SHARE_NAME = "Config/Share";

		public const string GAME_CONFIG_INVITEFRIEND_FILE = "Config/TenAward.xml";

		public const string GAME_CONFIG_CHONGZHI_FILE = "Config/MU_ChongZhi.xml";

		public const string GAME_CONFIG_GIFT_FILE = "Config/Gifts/MU_Activities.xml";

		public const string GAME_CONFIG_HEFU_TYPE_FILE = "Config/HeFuType.xml";

		public const string GAME_CONFIG_HEFU_TYPE_NAME = "Config/HeFuType";

		public const string GAME_CONFIG_HEFU_DENGLUHAOLI_FILE = "Config/HeFuLiBao.xml";

		public const string GAME_CONFIG_HEFU_DENGLUHAOLI_NAME = "Config/HeFuLiBao";

		public const string GAME_CONFIG_HEFU_LEIJIDENGLU_FILE = "Config/HeFuDengLu.xml";

		public const string GAME_CONFIG_HEFU_LEIJIDENGLU_NAME = "Config/HeFuDengLu";

		public const string GAME_CONFIG_HEFU_CHONGZHIFANLI_FILE = "Config/HeFuFanLi.xml";

		public const string GAME_CONFIG_HEFU_CHONGZHIFANLI_NAME = "Config/HeFuFanLi";

		public const string GAME_CONFIF_HEFU_ZHANCHANGZHISHEN_FILE = "Config/PKJiangLi.xml";

		public const string GAME_CONFIF_HEFU_ZHANCHANGZHISHEN_NAME = "Config/PKJiangLi";

		public const string GAME_CONFIG_HEFU_BOSSZHIZHAN_FILE = "Config/HeFuBOSS.xml";

		public const string GAME_CONFIG_HEFU_BOSSZHIZHAN_NAME = "Config/HeFuBOSS";

		public const string GAME_CONFIG_HEFU_QIANGGOU_FILE = "Config/HeFuQiangGou.xml";

		public const string GAME_CONFIG_HEFU_QIANGGOU_NAME = "Config/HeFuQiangGou";

		public const string GAME_CONFIG_CRYSTALMONSTER_FILE = "Config/CrystalMonster.xml";

		public const string GAME_CONFIG_CRYSTALMONSTER_NAME = "Config/CrystalMonster";

		public const string GAME_CONFIG_lUOLAN_FILE = "Config/HeFuLuoLan.xml";

		public const string GAME_CONFIG_LUOLAN_NAME = "Config/HeFuLuoLan";

		public const string GAME_CONFIG_FAMILYACTIVITY_FILE = "Config/ZhanMengHuoDongTab.xml";

		public const string GAME_CONFIG_FAMILYACTIVITY_NAME = "Config/ZhanMengHuoDongTab";

		public const string GAME_CONFIG_FAMILYACTIVITY_YANHUI = "Config/GleeFeastAward.Xml";

		public const string GAME_CONFIG_LUOLANFAZHAN_FILE = "Config/LuoLanFaZhen.xml";

		public const string GAME_CONFIG_LUOLANFAZHAN_NAME = "Config/LuoLanFaZhen";

		public const string GAME_CONFIG_LINGYU_FILE = "Config/LingyuType.xml";

		public const string GAME_CONFIG_LINGYU_NAME = "Config/LingyuType";

		public const string GAME_CONFIG_LINGYULEVEL_FILE = "Config/LingyuLevelUp.xml";

		public const string GAME_CONFIG_LINGYULEVEL_NAME = "Config/LingyuLevelUp";

		public const string GAME_CONFIG_LINGYUSUIT_FILE = "Config/LingyuSuitUp.xml";

		public const string GAME_CONFIG_LINGYUSUIT_NAME = "Config/LingyuSuitUp";

		public const string GAME_CONFIG_LINGYUJIACHENG_FILE = "Config/LingYucollect.xml";

		public const string GAME_CONFIG_LINGYUJIACHENG_NAME = "Config/LingYucollect";

		public const string GAME_CONFIG_KUAFU_FILE = "Config/KuaFuHuoDongTab.xml";

		public const string GAME_CONFIG_KUAFU_NAME = "Config/KuaFuHuoDongTab";

		public const string GAME_CONFIG_ZHANDUI_FILE = "Config/ZhanDuiHuoDongTab.xml";

		public const string GAME_CONFIG_ZHANDUI_NAME = "Config/ZhanDuiHuoDongTab";

		public const string GAME_CONFIG_ADENDA_FILE = "Config/ShengWangXunZhang.xml";

		public const string GAME_CONFIG_ADENDA_ATTRIBUTE_FILE = "Config/ShengWangSpecialAttribute.xml";

		public const string GAME_CONFIG_HUNYAN_CONFIG = "Config/WeddingFeasttAward.xml";

		public const string GAME_CONFIG_UI_MODEL_FILE = "Config/UIModel.xml";

		public const string GAME_CONFIG_MAGICBOOK_FILE = "Config/MagicBook.xml";

		public const string GAME_CONFIG_SHENGWU_FILE = "Config/ShengWu.xml";

		public const string GAME_CONFIG_BUJIAN_FILE = "Config/BuJian.xml";

		public const string GAME_CONFIG_SHENGWUPROPERTY_FILE = "Config/ExtPropIndexes.xml";

		public const string GAME_CONFIG_JINGGAO_FILE = "Config/JingGao.xml";

		public const string GAME_CONFIG_JINDENG_FILE = "Config/JinZhiDengLu.xml";

		public const string GAME_CONFIG_TIANTIJIANGLI_FILE = "Config/DuanWeiRankAward.xml";

		public const string GAME_CONFIG_TEAMCOMPETEAWRD_FILE = "Config/TeamDuanWeiAward.xml";

		public const string GAME_CONFIG_TEAMZHENGBAAWRD_FILE = "Config/TeamMatchAward.xml";

		public const string GAME_CONFIG_DATAOSHAAWRD_FILE = "Config/EscapeRankAward.xml";

		public const string GAME_CONFIG_TUIGUANG_LEIJI_FILE = "Config/TuiGuang/TuiGuangYuanLeiJi.xml";

		public const string GAME_CONFIG_TUIGUANG_VIPAWARD_FILE = "Config/TuiGuang/TuiGuangYuanVip.xml";

		public const string GAME_CONFIG_TUIGUANG_LEVELAWARD_FILE = "Config/TuiGuang/TuiGuangYuanLevel.xml";

		public const string GAME_CONFIG_TUIGUANG_NEWBOY_FILE = "Config/TuiGuang/TuiGuangXinYongHu.xml";

		public const string GAME_CONFIG_CANGBAOMIJING_BOX = "GameRes/Config/Treasure/TreasureBox.xml";

		public const string GAME_CONFIG_CANGBAOMIJING_EVENT = "GameRes/Config/Treasure/TreasureEvent.xml";

		public const string GAME_CONFIG_CANGBAOMIJING_MAP = "GameRes/Config/Treasure/TreasureMap.xml";

		public const string GAME_CONFIG_CANGBAOMIJING_ROUTE = "GameRes/Config/Treasure/TreasureRoute.xml";

		public const string GAME_CONFIG_CITY_FILE = "Config/MU_City.xml";

		public const string GAME_CONFIG_CITYWAR_FILE = "Config/CityWar.xml";

		public const string GAME_CONFIG_CITYWARQIZUO_FILE = "Config/CityWarQiZuo.xml";

		public const string GAME_CONFIG_MONSTERSSITE_FILE = "Config/MonstersSite.xml";

		public const string GAME_CONFIG_LEAGUE_WAR = "Config/LeagueWar.xml";

		public const string GAME_CONFIG_LANGHUNYAOSAI_MONSTER = "Config/LangHunYaoSai.xml";

		public const string GAME_CONFIG_FUNDSET_FILE = "Config/Fund/FundSet.xml";

		public const string GAME_CONFIG_FUND_FILE = "Config/Fund/Fund.xml";

		public const string GAME_CONFIG_WANGZHEQIZUO_FILE = "Config/KingOfBattleQiZuo.xml";

		public const string GAME_CONFIG_EMBLEMSTART = "Config/EmblemStar.xml";

		public const string GAME_CONFIG_EMBLEMUP = "Config/EmblemUp.xml";

		public const string GAME_CONFIG_ANNOUNCED = "Config/SystemTrailer.xml";

		public const string GAME_CONFIG_REDPOINT = "Config/RedPoint.xml";

		public const int RoleEquipMaxValue = 25;

		public const int ConstNewPlayerMapCode = 6090;

		public const int ConstShuiJingHuanJingMapCode = 12000;

		public const int AutoFindRoadCaiJi = 60;

		public const int AutoFindRoadOffset60 = 120;

		public const int AutoFindRoadOffset100 = 120;

		public const int AutoFindRoadOffset150 = 120;

		public const int ConstRiChangTaskMinLevel_0 = 40;

		public const int ConstRiChangTaskMinZhuanSheng_0 = 0;

		public const int ConstRiChangTaskMinLevel_1 = 40;

		public const int ConstRiChangTaskMinZhuanSheng_1 = 0;

		public const int ConstHintMinZhuanSheng_1 = 2;

		public const int ConstPKValueRedName = 200;

		public const int MinAutoDrinkInterval = 1000;

		public const int MinAutoDrinkInterval2 = 1000;

		public const int ConstPKNeedLevel = 60;

		public const int ConstMaxMingXiang = 43200;

		public const int ConstBloodCastleTaskGoodsID0 = 10000;

		public const int ConstBloodCastleTaskGoodsID1 = 10001;

		public const int ConstBloodCastleTaskGoodsID2 = 10002;

		public const int ConstRiChangTaskNPCID = 119;

		public const int ConstTaofaTaskNPCID = 120;

		public const string ConstGoodsEndTime = "1900-01-01 12:00:00";

		private const int MaxLieShaDailyNum = 10;

		private const int MaxLieShaDailyNumForVIP = 15;

		private const int MaxWuXueDailyNum = 10;

		private const int MaxJunGongDailyNum = 6;

		private const int MaxMoZuShiLiDailyNum = 1;

		private const int MaxBangHuiDailyNum = 10;

		public const int MaxMURiChangDailyNum = 10;

		public const int MaxMUTaoFaDailyNum = 5;

		public const int MaxAddPropIndex = 10;

		public const int MaxZhuijiaLevel = 80;

		public const int MaxEquipLevel = 20;

		public const int MaxZhuanshengLevel = 10;

		public const int OPENBAGGRID_ONEGRIDNEEDTIME1 = 3000;

		public const int OPENBAGGRID_ONEGRIDNEEDTIME2 = 1500;

		public const int PKValueEqPKPoints = 100;

		public const int MinRedNamePKPoints = 200;

		private const int MaxDayYaBiaoNum = 1;

		public const int TAO_ZHUANG_COUNT = 8;

		public const int OnLoadMoney = 150;

		public const int MemoryProtectionValue = 50;

		public const bool IsCacheStrategy = true;

		private const int LowGSprite = 5;

		private const int MiddleGSprite = 20;

		public const string PLATFORM_CONFIG_CHENGJIU_FILE = "Config/TongJiPlatform.Xml";

		public const string PLATFORM_CONFIG_CHENGJIU_NAME = "ConfigTongJiPlatform";

		public static int SyscTimeCout = 0;

		public static int SyscByClientFailCount = 0;

		public static Queue<long> subTicksQueue = new Queue<long>();

		public static Dictionary<string, string> RootParams = new Dictionary<string, string>();

		public static StageSL MainStage = null;

		public static StageSL GlobalMainWindow = null;

		private static EmptePart EmptePartInstance = null;

		private static GGoodIcon GGoodIconInstance = null;

		public static UIJoystick Joystick = null;

		public static bool DisableInput = false;

		public static NetAudioSource BackgroundAudio4UI = null;

		public static AudioListener AudioListener4UI = null;

		public static NetAudioSource BackgroundAudio43D = null;

		public static AudioListener AudioListener43D = null;

		public static bool g_bReconnRoleManager = false;

		public static string g_strUserName = string.Empty;

		public static string g_strPassWord = string.Empty;

		public static int g_nReconnTimes = 0;

		public static bool g_bIsYaoQingCeShi = false;

		public static long g_nLoginTime = 0L;

		public static bool g_bIsYaoPinTip = false;

		public static bool g_bIsTipsShowCuiHuiBtn = false;

		public static StallStateType g_StallStateType = StallStateType.StallNull;

		public static bool IsMainCamera = true;

		public static Camera MainCamera = null;

		public static Camera UICamera = null;

		public static Camera DecorationUICamera = null;

		public static Light DirectLight = null;

		public static AssetBundle Login3DBakMapLoader = null;

		public static AssetBundle RoleSel3DBakMapLoader = null;

		public static WWW RoleSel3DBakMapWWW = null;

		public static AssetBundle RoleCreate3DBakMapLoader = null;

		public static WWW RoleCreate3DBakMapWWW = null;

		public static bool HaveYanHui = false;

		public static bool m_HaveYanHuiKaiQi = false;

		public static List<int> m_HaveYanHuiData = new List<int>();

		public static bool m_MenuIconBoxYinCan = false;

		public static bool m_MenuIconBoxFlag = true;

		private static Dictionary<int, Transform> m_MenuIconBoxMapTransform = new Dictionary<int, Transform>();

		private static Dictionary<int, Vector3> m_MenuIconBoxMapVector3 = new Dictionary<int, Vector3>();

		public static EventHandler NotifyAddCoolDown;

		private static Dictionary<int, CoolDownItem> SkillCoolDownDict = new Dictionary<int, CoolDownItem>();

		public static EventHandler NotifyGoodsAddCoolDown;

		private static Dictionary<int, CoolDownItem> GoodsCoolDownDict = new Dictionary<int, CoolDownItem>();

		public static List<LineData> LineDataList = null;

		public static LineData CurrentListData = null;

		public static MJSSkillType currentMJSType = MJSSkillType.Strength_Sword;

		public static GData Data = null;

		public static ZtBuffServerInfo CurrentZtServerInfo = null;

		public static GMapData CurrentMapData = null;

		private static string m_ChatVoiceServerURL = string.Empty;

		private static string m_PushServerURL = string.Empty;

		private static string m_AdServerUrl = string.Empty;

		private static string m_strVerifyAccountServerURL = string.Empty;

		private static string m_strPayServerURL = string.Empty;

		private static string m_ServerListURL = string.Empty;

		private static string m_ServerListURLSecond = string.Empty;

		private static string m_ServerListCrossPlatfomURL = string.Empty;

		private static string m_appealUrl = string.Empty;

		public static EventHandler NotifyEmblemAddCoolDown;

		private static Random GlobalRandom = new Random();

		public static HUDText LeaderGainTextHeadText = null;

		private static long myDateTimeTicksBak = 0L;

		public static bool IsInSafeRegion = true;

		private static Dictionary<string, int> MagicVDict = new Dictionary<string, int>();

		private static int[] BaseSkillIDs = new int[]
		{
			100,
			200,
			300,
			10000,
			0,
			11000
		};

		public static Dictionary<int, int[]> BuffMagicDict = null;

		private static int[] AutoFightSkillIDs = null;

		private static Dictionary<int, int> MonsterUnionLevel = null;

		private static Dictionary<int, double[]> JuQingFuBenBossScale = null;

		private static Dictionary<int, int[]> GoodsShaderIDsCachingDict = new Dictionary<int, int[]>();

		public static Dictionary<int, int> CanUseGoodsByTaskDict = null;

		private static Dictionary<int, Dictionary<int, XElement>> WingDict = new Dictionary<int, Dictionary<int, XElement>>();

		public static Dictionary<int, TaskStarInfo> TaskStarInfoDict = null;

		public static int FirstMainTaskID = 0;

		public static int CurrentJingYanFuBenTimes = 0;

		private static List<Global.JingYanFuBenConfigData> riChangFuBenItems = new List<Global.JingYanFuBenConfigData>();

		private static string name = "ForgeMaxOpen";

		public static double[] ForgeLevelRocksPercent = null;

		public static int[] ForgeLevelNeedYinLiang = null;

		public static int[] LuckyGoodsIDs = null;

		public static string[] ForgeGoodsList = null;

		public static Dictionary<int, int[]> ForgeGoodsListDict = new Dictionary<int, int[]>();

		public static string[] ForgeGoodsNumList = null;

		public static Dictionary<int, int[]> ForgeGoodsNumListDict = new Dictionary<int, int[]>();

		public static int[] ForgeGoodsIDs = null;

		public static string[] ForgeRockNames = new string[]
		{
			Global.GetLang("一品"),
			Global.GetLang("二品"),
			Global.GetLang("三品"),
			Global.GetLang("四品"),
			Global.GetLang("五品"),
			Global.GetLang("六品"),
			Global.GetLang("七品"),
			Global.GetLang("八品"),
			Global.GetLang("九品"),
			Global.GetLang("十品"),
			Global.GetLang("十一品"),
			Global.GetLang("十二品")
		};

		public static int[] ChuanchengMoney = null;

		public static int[] ChuanchengYuanbao = null;

		public static double[] ChuanchengDiaojilv = null;

		public static double[] ZhuijiaChuanchengDiaojilv = null;

		public static double[] JuHunChuanchengDiaojilv = null;

		public static int[] ZhuijiaChuanchengMoney = null;

		public static int[] ZhuijiaChuanchengYuanbao = null;

		public static double[] XilianChuanchengDiaojilv = null;

		public static int[] XilianChuanchengMoney = null;

		public static int[] XilianChuanchengYuanbao = null;

		public static int[] juHunChuanchengMoney = null;

		public static int[] juHunChuanchengYuanbao = null;

		public static int[] ShengyoufuGoodsIDs = null;

		public static int[] ShengyoufuQianghuaDengji = null;

		public static int[] ZhuiJiaGoodsIDs = null;

		public static int[] ZhuiJiaGoodsIDNums = null;

		public static int ZhuiJiaForgeLuckyGoodsIDs = 0;

		public static int[] ZhuijiaForgeLevelNeedMoney = null;

		public static double[] ZhuijiaChenggonglvs = null;

		public static int[] ZhuanshengGoodsIDs = null;

		public static double[] ZhuanshengExpRates = null;

		public static int[] ZhuanshengLevelNeedMoney = null;

		public static double[] ZhuanshengChenggonglvs = null;

		public static int[] ZhuanshengBoliMoney = null;

		public static int[] ZhuanshengBoliYuanbao = null;

		public static double[] ZhuanshengBoliChenggonglvs = null;

		public static int MaxSuitID = 5;

		public static int[] QianghuashiFenliMoney = null;

		private static string[] washPropsNames = new string[]
		{
			"MaxLifeV",
			"AddAttackInjure",
			"DecreaseInjureValue",
			"AddAttack",
			"AddDefense",
			"HitV",
			"Dodge",
			"LifeSteal"
		};

		private static Dictionary<int, float> XilianPropsUpFactorDict = null;

		public static Dictionary<int, int> DBIdsDictByXilian = null;

		public static int ShenqiZaizaoSuit = 10;

		public static Dictionary<int, ShenqiZaizaoXmlData> ShenqiZaizaoXmlDataDict = null;

		private static Dictionary<string, List<double[]>> AddNewPropsDict = new Dictionary<string, List<double[]>>();

		private static string[] AddNewPropNames = new string[]
		{
			Global.GetLang("物攻"),
			Global.GetLang("魔攻"),
			Global.GetLang("物防"),
			Global.GetLang("魔防"),
			Global.GetLang("暴击"),
			Global.GetLang("暴抗"),
			Global.GetLang("命中"),
			Global.GetLang("闪避"),
			Global.GetLang("HP"),
			Global.GetLang("MP")
		};

		private static string[] ForgeAddNewPropPreNames = new string[]
		{
			Global.GetLang("强化+5 追加:"),
			Global.GetLang("强化+7 追加:"),
			Global.GetLang("强化+9 追加:"),
			Global.GetLang("强化+10追加:")
		};

		private static string[] JewelsAddNewPropPreNames = new string[]
		{
			Global.GetLang("全4级宝石追加:"),
			Global.GetLang("全6级宝石追加:"),
			Global.GetLang("全8级宝石追加:")
		};

		private static Dictionary<int, int> mapTypeDict = new Dictionary<int, int>();

		private static string[] MapPKModes = new string[]
		{
			Global.GetLang("普通地图"),
			Global.GetLang("战斗地图"),
			Global.GetLang("安全地图")
		};

		private static string[] MapPKHints = new string[]
		{
			Global.GetLang("你已经进入了PK受限制的普通地图, 决斗将增加PK值"),
			Global.GetLang("你已经进入了不受保护的战斗地图, 决斗将不增加PK值"),
			Global.GetLang("你已经进入了受保护的安全地图")
		};

		private static Dictionary<string, int[]> MapHelpHintDict = new Dictionary<string, int[]>();

		public static double[] ForgeLevelAddAttackRates = null;

		public static double[] ForgeLevelAddDefenseRates = null;

		public static double[] ForgeLevelAddMaxLifeVRates = null;

		public static double[] ForgeLevelPet = null;

		public static double[] ChiBangForgeLevelAddDefenseRates = null;

		public static double[] ChiBangForgeLevelAddShangHaiJiaCheng = null;

		public static double[] ChiBangForgeLevelAddShangHaiXiShou = null;

		public static double[] EnchanceLevelAddRates = null;

		public static string[] ExtPropIndexNames = new string[]
		{
			"Weight",
			string.Empty
		};

		public static double[] ZhuoYueAddDefenseRates = null;

		public static double[] ZhuoYueAddAttackRates = null;

		public static double[] ZhuijiaLevelAddAttackRates = null;

		public static double[] ZhuijiaLevelAddDefenseRates = null;

		public static double[] ZhuanshengLevelAddAttackRates = null;

		public static double[] ZhuanshengLevelAddDefenseRates = null;

		private static int mapCode_XuezhanDifu = 0;

		private static int shaChengMapCode = 0;

		private static int palaceMapCode = 0;

		private static List<int> GuMuMapList = null;

		private static int mapCode_YanhuangZhanchang = 0;

		public static string[] BattleBufferNames = new string[]
		{
			Global.GetLang("怒斩·PK王")
		};

		public static int MaxPortableGridNum = 216;

		public static int MaxRebornPortableGridNum = 216;

		public static int OnePortableGridYuanBao = 5;

		public static int OneJinDanGridYuanBao = 5;

		public static int MaxBagGridNum = 100;

		public static int OneBagGridYuanBao = 10;

		public static int OneCangKuGridYuanBao = 5;

		public static int DefaultBagGridNum = 50;

		public static int DefaultPortableGridNum = 60;

		public static int MaxRebornBagGridNum = 100;

		public static int DefaultRebornBagGridNum = 50;

		public static int DefaultRebornPortableGridNum = 60;

		public static int MaxJinDanGridNum = 240;

		public static int MaxJingLingGridNum = 240;

		public static Queue<GoodsData> TempYuansuGoodsDataQueue = null;

		public static int YuansuBagMaxGridCount = 100;

		public static int guardStatueLevel = 0;

		public static int guardStatueGrade = 1;

		public static int MaxWuxueLevel = 10;

		public static int[] WuxueBuffIDs = null;

		private static int _CacheNextWuXueLevel = -1;

		private static int _CacheNextWuXueLevelNeedRoleLevel = 0;

		private static int _CacheNextWuXueLevelNeedYinLiang = 0;

		private static double _CacheNextWuXueLevelDayXiaoHao = 0.0;

		private static int _CacheNextWuXueLevelNeedWuXin = 0;

		public static int MaxJingMaiDecoNum = 9;

		public static int MaxJingMaiBodyLevel = 9;

		public static int MaxJingMaiLevel = 25;

		public static int MaxDailyJingMaiNum = 10;

		public static int JingMaiLingZhiLuckyNum = 10;

		public static string[] JingMaiNames = new string[]
		{
			Global.GetLang("阳维脉"),
			Global.GetLang("阴维脉"),
			Global.GetLang("阳跷脉"),
			Global.GetLang("阴跷脉"),
			Global.GetLang("带脉"),
			Global.GetLang("冲脉"),
			Global.GetLang("任脉"),
			Global.GetLang("督脉")
		};

		public static string[] JingMaiBodyLevelNames = new string[]
		{
			Global.GetLang("醒我"),
			Global.GetLang("蜕凡"),
			Global.GetLang("识藏"),
			Global.GetLang("御空"),
			Global.GetLang("涅盘"),
			Global.GetLang("长生"),
			Global.GetLang("蕴神"),
			Global.GetLang("入神"),
			Global.GetLang("归神")
		};

		public static string[] JingMaiBodyLevelPicNames = new string[]
		{
			"xingwo.png",
			"tuifan.png",
			"shizang.png",
			"yukong.png",
			"niepan.png",
			"changsheng.png",
			"wenshen.png",
			"rushen.png",
			"guishen.png"
		};

		private static Dictionary<string, XElement> JingMaiXueWeiDict = new Dictionary<string, XElement>();

		private static int _CacheNextJingMaiLevel = -1;

		private static int _CacheNextJingMaiLevelNeedRoleLevel = 0;

		private static int _CacheNextJingMaiLevelNeedTongQian = 0;

		private static int _CacheNextJingMaiLevelNeedGoods = 0;

		private static int _CacheNextJingMaiLevelNeedGoodsNum = 0;

		private static int _CacheNextJingMaiLevelNeedZhenQi = 0;

		public static int MaxZhanhun = 12;

		public static int[] ZhanhunBuffIDs = null;

		public static int MaxRongyao = 50;

		public static int[] RongyaoBuffIDs = null;

		public static int MapTransGoodsID = 32000;

		public static int MapTransGoodsID2 = 32003;

		public static Dictionary<string, List<string>> FilterFieldsDict = null;

		private static string SpecialDigits = "０１２３４５６７８９";

		private static Dictionary<int, int[]> callMagicDict = new Dictionary<int, int[]>();

		private static List<TaoZhuangVO> TaoZhuangVOList = null;

		private static int VisiableBuffCount;

		public static bool isOpenSkillPriority = false;

		private static Dictionary<int, int> skillPriorityDict = new Dictionary<int, int>();

		private static Dictionary<string, Dictionary<int, int>> skillDicts = new Dictionary<string, Dictionary<int, int>>();

		public static int MinEnterJailPKPoints = 500;

		public static int MinLeaveJailPKPoints = 400;

		public static int LaoFangMapCode = -1;

		public static long MaxPurpleNameTicks = 60000L;

		public static int MaxYaBiaoTicks = 2400000;

		public static string[] LianZhanBufferNames = new string[]
		{
			Global.GetLang("百连"),
			Global.GetLang("如麻"),
			Global.GetLang("主宰"),
			Global.GetLang("暴走"),
			Global.GetLang("无敌"),
			Global.GetLang("妖孽"),
			Global.GetLang("至尊"),
			Global.GetLang("魔尊"),
			Global.GetLang("魔神")
		};

		private static int[] AllQualityDefensePercents = new int[]
		{
			default(int),
			2,
			5
		};

		private static int[] AllForgeLevelAttackPercents = new int[]
		{
			0,
			1,
			3,
			5,
			7,
			9
		};

		private static int[] AllJewelLevelOccupPercents = new int[]
		{
			0,
			0,
			0,
			0,
			10,
			20,
			30,
			40,
			50,
			50,
			50
		};

		private static int[] AllJewelLevelOtherPercents = new int[]
		{
			0,
			0,
			0,
			0,
			1,
			2,
			3,
			4,
			5,
			5,
			5
		};

		public static int RenameBangQiNameNeedTongQian = 5000000;

		public static int JoinBangHuiNeedLevel = 0;

		public static int MinDonateTongQianPerBangGong = 200000;

		public static int MinDonateBangGongTongQian = 10000;

		public static int MaxBangHuiFlagLevel = 4;

		private static int[] LingDiIDs2MapCodes = null;

		public static int MaxFreeRefreshNum = 3;

		public static int MaxClickYangGongBKNum = 4;

		private static int MaxHavingSheLiZhiYuanSecs = 0;

		private static List<string> ListIconCfgWndName = null;

		public static int limitTabID = -100000;

		private static Dictionary<string, BufferItemTypes> FuncNameToBufferIdMap = null;

		public static bool IsLingquChongzhiFanli = false;

		public static DateTime _ServerStartTime = new DateTime(2000, 1, 1);

		public static DateTime _ServerMergeTime = new DateTime(2000, 1, 1);

		public static DateTime _JieriTime = new DateTime(2000, 1, 1);

		public static DateTime _YueduDazhunpanTime = new DateTime(2000, 1, 1);

		public static DateTime _BuchangTime = new DateTime(2000, 1, 1);

		private static Dictionary<int, ActivityTipConfigItem> DictAcitvityMinLevle = null;

		public static int[] IconsMinLevel = null;

		public static int[] HuodongIconEffectNeedRoleLevel = null;

		public static int YunChengGeMapCode = -1;

		public static int[] PetExp = null;

		private static List<XElement> _CachingTaskPlotItems = null;

		public static int MaxNotifyEquipStrongValue = 500;

		private static Dictionary<int, int> GoodsStrongDict = new Dictionary<int, int>();

		public static string DengluDaliStartTime = string.Empty;

		public static string DengluDaliEndTime = string.Empty;

		public static int MaxZhuanhuangLevel = 10;

		public static int[] zhuanhuangBuffIDs = null;

		private static Dictionary<int, XElement> ShaderXmlNodeDict = new Dictionary<int, XElement>();

		private static Dictionary<XElement, Caching3DSubObjectsItem> Caching3DSubObjectsItemDict = new Dictionary<XElement, Caching3DSubObjectsItem>();

		private static Dictionary<string, Caching3DSubObjectsItem> Caching3DSubObjectsItemVODict = new Dictionary<string, Caching3DSubObjectsItem>();

		private static Dictionary<string, Caching3DSubObjectsItem> Caching3DSubObjectsItemMonsterVODict = new Dictionary<string, Caching3DSubObjectsItem>();

		private static Dictionary<string, Caching3DSubObjectsItem> Caching3DSubObjectsItemGoodVODict = new Dictionary<string, Caching3DSubObjectsItem>();

		private static Dictionary<int, string[]> NakePartsListCachingDict = new Dictionary<int, string[]>();

		public static bool CanGiveFakeEquips = false;

		public static int FakeEquipsIndex = 0;

		public static bool ShowTeleport = false;

		private static int[] BloodCastleChengMenIDs = null;

		private static int[] BloodCastleLingGuanIDs = null;

		private static int[] LangHunYaoSaiMonsters = null;

		private static bool initLayerMask = false;

		private static int layerMask = -1;

		public static XElement VersionXml = null;

		public static XElement PersistentVersionXML = null;

		public static XElement NetVersionXML = null;

		public static XElement IndexXML = null;

		public static XElement PersistentIndexXML = null;

		public static bool IsMiniPackage = false;

		public static FenBaoDownloadType FenBaoType = FenBaoDownloadType.None;

		public static bool IsMiniPackUpdated = false;

		public static bool IsFullPackUpdated = false;

		public static XElement NetIndexXML = null;

		private static bool IsLoadMingXiangExprInfo = false;

		private static int MaxPataIndex = 0;

		public static int MaxShaodangNum = 0;

		public static string[] PataIndexRangeBySets = null;

		public static int[] PataIndexRange = null;

		private static float MemoryProtectionTotalElapsedTime;

		private static bool isShowOtherRole = true;

		private static bool isShowFakeRole = true;

		private static float TotalElapsedTime = 0f;

		private static int LowFPS = 0;

		private static int UpFPS = 0;

		private static int MiddleFPS = 0;

		public static int originHeight = 0;

		public static float ratio;

		private static float timeFor150Frame = 5f;

		private static bool EnabledHalfRes = false;

		private static bool isInGameScene = false;

		private static bool halfRes = false;

		public static List<object> listLianShaObject = new List<object>();

		public static Action VerifySuccess;

		public static int HasSecondPassword = -1;

		public static int NeedVerifySecondPassword = -1;

		public static bool IsSetSecondPassword = false;

		public static Dictionary<sbyte, HolyItemData> dic_holyItem = new Dictionary<sbyte, HolyItemData>();

		public static Dictionary<sbyte, HolyItemPartData> m_PartArray = new Dictionary<sbyte, HolyItemPartData>();

		public static Dictionary<int, HolyPartAttribute> dic_holyPartAttr = new Dictionary<int, HolyPartAttribute>();

		public static bool shengewuTip = false;

		private static Dictionary<int, DiamondAttribute> _dic_diamond = null;

		public static int MoRiShenPanOnTimeKillCount = 0;

		public static Dictionary<int, int> MoRiShenPanShuijingState = new Dictionary<int, int>();

		public static GameObject shuijingtexiao = null;

		private static int endState = 2;

		public static int JieriXML_Version = 0;

		public static DateTime _QiRiKuangHuanTime = new DateTime(2000, 1, 1);

		public static Dictionary<int, SevendayGoal> dic_SevenDayGoal = new Dictionary<int, SevendayGoal>();

		public static Dictionary<int, SevenDayQiangGou> dic_SevenDayQiangGou = new Dictionary<int, SevenDayQiangGou>();

		private static Dictionary<int, Dictionary<int, Global.ShiZhuangResInfo>> shiZhuangDic = new Dictionary<int, Dictionary<int, Global.ShiZhuangResInfo>>();

		private static Dictionary<int, CityInfo> dic_CityInfo = new Dictionary<int, CityInfo>();

		private static Dictionary<int, Global.LeagueWarData> LeagueWarDataDict = new Dictionary<int, Global.LeagueWarData>();

		public static long zhanmengZiJin = 0L;

		public static int zhanmengLevel = 0;

		private static Dictionary<int, Global.MonsterSite> DicMonsterSite = new Dictionary<int, Global.MonsterSite>();

		public static Queue<GoodsData> soulCometStoneGoodsDataQueue = null;

		public static bool isSoulCometStoneGathering = false;

		private static Dictionary<int, Dictionary<int, Global.FundSetXml>> fundSetXmlDic = new Dictionary<int, Dictionary<int, Global.FundSetXml>>();

		private static Dictionary<int, Dictionary<int, Global.FundXml>> fundxmlDic = new Dictionary<int, Dictionary<int, Global.FundXml>>();

		public static FundData fundData = null;

		public static int YongzhezhanchangLianShaInf = 0;

		public static int ZhuanxiangXML_Version = -1;

		public static Dictionary<int, SpecialActivityTime> g_ZhuanXiangTime = new Dictionary<int, SpecialActivityTime>();

		public static Dictionary<int, SpecialActivityXML> g_ZhuanXiangDic = new Dictionary<int, SpecialActivityXML>();

		public static int ZhuTiFuXML_Version = -1;

		public static int everyDayXML_Version = -1;

		public static Dictionary<int, EveryDayActivity> g_everyDayDic = new Dictionary<int, EveryDayActivity>();

		public static long hongBaoPaiHang_Version = -1L;

		private static Dictionary<int, int> m_DicSkillURLID = new Dictionary<int, int>();

		private static Dictionary<int, string> m_DicJingLingSkillName = new Dictionary<int, string>();

		private static XElement XElementxml_Magics = null;

		private static Dictionary<int, float> m_DicJingLingSkillCD = new Dictionary<int, float>();

		private static Dictionary<int, int> m_DicJingLingSkillMagicColor = new Dictionary<int, int>();

		public static bool IsBinding = false;

		public static bool HaveGetAward = false;

		private static Dictionary<int, Global.MonsterSite> _WangZheZhanChangQiZuoDic = new Dictionary<int, Global.MonsterSite>();

		public static Dictionary<int, JieRiFanBeiItemData> JieriFanbeiInfo = new Dictionary<int, JieRiFanBeiItemData>();

		private static string[] VioceToWordFilterFields = null;

		public static int secondRoleId = -1;

		private static bool m_DialyTaskAlertIsUsing = false;

		private static bool _IsCanMove = true;

		public static bool DangQianHuaZhi = false;

		public static bool IsRunning = false;

		private static List<SpecialTitle> _TeShuTitleListXml = null;

		public static List<KarenScoreData> KarenScorepProvisionalData = new List<KarenScoreData>();

		public static List<int> OldMineResourceList = new List<int>();

		public static Dictionary<int, EmblemStarXml> m_DicStar = new Dictionary<int, EmblemStarXml>();

		public static Dictionary<int, EmblemUp> m_DicUp = new Dictionary<int, EmblemUp>();

		private static string mVoiceAPP_ID = string.Empty;

		private static string mVoiceAPP_Key = string.Empty;

		private static bool mIsCompetitionGuanKan = false;

		public static List<string> DownLoadInfos = new List<string>();

		private static bool isOpenCheckMap = true;

		public static bool IsFirstPopupDownloadMapWindowInWorldMap = false;

		public static bool IsKuaFuHuoDongFenBaoMap = false;

		public static bool IsFuBenFenBaoMap = false;

		private static List<int> mDownloadedMapList = new List<int>();

		private static List<int> mDownloadMapQueue = new List<int>();

		public static long LastRequestRideHorseTick = 0L;

		private static Dictionary<int, bool> dicHorseEquipOpen = new Dictionary<int, bool>();

		public static int requstRetainCount = 0;

		public static List<XElement> BianQiangStandardList = null;

		public static List<TiShengZhanLiItemVO> listData = new List<TiShengZhanLiItemVO>();

		public static Dictionary<int, int> m_DicActiveStar = new Dictionary<int, int>();

		public static MerlinGrowthSaveDBData DataBagMerlinGrowthSaveDBData = new MerlinGrowthSaveDBData();

		public static int lingyuTotalLevel = 0;

		private static int _zuoQiLevelByBianQiang = 0;

		public static GuardStatueData guardStatue = null;

		public static UnionPalaceData CurrentData = null;

		public static ShenQiData shenQiData = null;

		public static int TeamMaxCount = 5;

		public static ActivityCategorys CurrentActivityCategorys = ActivityCategorys.GuZhanChang;

		private static List<Global.TuJianRedData> tuJianRedDatas = new List<Global.TuJianRedData>();

		public static Dictionary<int, TujianXmlData> TujiaXmlDataDictByRedPoint = null;

		public static Dictionary<int, int> TujianListInBagDictByRedPoint = null;

		private static int xingHunCount = 0;

		private static List<XingZuo> lstXingZuo = new List<XingZuo>();

		private static Dictionary<int, int> dicStarConstellations = new Dictionary<int, int>();

		private static int m_nMaxJieShu = 5;

		private static int m_nMaxNode = 60;

		public static Dictionary<int, float[]> mHorseYinJiDecorations = new Dictionary<int, float[]>();

		public static Dictionary<int, float[]> mHorseDecorations = new Dictionary<int, float[]>();

		private static string xmlMoYu = string.Empty;

		private static Dictionary<int, ThemeActivityMoYu> m_DicThemeActivityNpc = new Dictionary<int, ThemeActivityMoYu>();

		private static int mShiLianEndTime = -1;

		private static Dictionary<int, bool> mDicZuanShi = new Dictionary<int, bool>();

		private static List<BtnCD> BtnCDList = new List<BtnCD>();

		private static byte[] mNameLengthRange = null;

		public static bool IsWatchingVedio = false;

		public static bool IsOpenGameFromGameCenter = false;

		public static bool IsChangeNewMap = false;

		public static int SekletonVersion = 1;

		public static bool IsOpenABLoadAnim = true;

		private static Dictionary<string, string> LangDict = null;

		public static Dictionary<RiChangHuoDongTypes, int> dicHuoDongPage;

		public static float VersionCode = 8f;

		public static bool isHaiWai = true;

		public static XElement PersistentZIPIndexXML = null;

		public static bool IsMiniPackZipUpdated = false;

		public static XElement NetZIPIndexXML = null;

		public class GlobalEventDispatcher
		{
			public static void addEventListener(string eventName, Global.GlobalEventDispatcher.PlayGameContolEventHandler eventHandler)
			{
				Global.GlobalEventDispatcher.eventDic[eventName] = eventHandler;
			}

			public static void dispatchEvent(PlayGameContolEvent ent)
			{
				Global.GlobalEventDispatcher.PlayGameContolEventHandler playGameContolEventHandler = null;
				Global.GlobalEventDispatcher.eventDic.TryGetValue(ent.type, ref playGameContolEventHandler);
				if (playGameContolEventHandler != null)
				{
					playGameContolEventHandler(ent);
				}
			}

			public static void ClearEventListener()
			{
				Global.GlobalEventDispatcher.eventDic.Clear();
			}

			private static Dictionary<string, Global.GlobalEventDispatcher.PlayGameContolEventHandler> eventDic = new Dictionary<string, Global.GlobalEventDispatcher.PlayGameContolEventHandler>();

			public delegate void PlayGameContolEventHandler(PlayGameContolEvent ent);
		}

		public class BackgroundSound
		{
			public static bool IsPlaying { get; set; }

			public static void stop()
			{
			}

			public static void play(string path, bool arg)
			{
			}
		}

		public class JingYanFuBenConfigData
		{
			public int MinZhuanSheng { get; set; }

			public int MaxZhuanSheng { get; set; }

			public int MinLevel { get; set; }

			public int MaxLevel { get; set; }

			public int MapCode { get; set; }

			public int MaxEnterNum { get; set; }

			public int MaxFinishNum { get; set; }

			public bool LevelAllow { get; set; }
		}

		public class CacheResule
		{
			public Object gameObj;

			public long ticks;

			public int refCount;
		}

		public class ShiZhuangResInfo
		{
			public int GoodsID;

			public int MainOccupation;

			public int TouRes = -1;

			public int XiongRes = -1;

			public int ShouRes = -1;

			public int TuiRes = -1;

			public int XieRes = -1;
		}

		public class LeagueWarData
		{
			public int ID;

			public int QiZuoID;

			public string QiZuoSite;

			public int PosX;

			public int PosY;
		}

		public class MonsterSite
		{
			public int ID;

			public int PosX;

			public int PosY;

			public int FixedDirection;
		}

		public class FundSetXml
		{
			public int MinVip;

			public int NextID;

			public int Price;

			public string Tips_1;

			public string Tips_2;

			public string Tips_3;
		}

		public class FundXml
		{
			public int ID;

			public int GoalType;

			public int RewardType;

			public int RewardCount;

			public string GoalNum;

			public string Tips;
		}

		private class TuJianRedData
		{
			public string OpenStr { get; set; }

			public string strTypeID { get; set; }

			public string PropsStr { get; set; }

			public bool IsActived { get; set; }
		}
	}
}
