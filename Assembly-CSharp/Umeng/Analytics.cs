using System;
using System.Collections.Generic;
using UnityEngine;

namespace Umeng
{
	public class Analytics
	{
		public static void StartWithAppKeyAndChannelId(string appKey, string channelId)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics._AppKey = appKey;
			Analytics._ChannelId = channelId;
			Analytics.UMGameAgentInit();
			if (!Analytics.hasInit)
			{
				Analytics.onResume();
				Analytics.CreateUmengManger();
				Analytics.hasInit = true;
			}
		}

		public static void SetLogEnabled(bool value)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("setDebugMode", new object[]
			{
				value
			});
		}

		public static void Event(string eventId)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEvent", new object[]
			{
				Analytics.Context,
				eventId
			});
		}

		public static void Event(string eventId, string label)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEvent", new object[]
			{
				Analytics.Context,
				eventId,
				label
			});
		}

		public static void Event(string eventId, Dictionary<string, string> attributes)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEvent", new object[]
			{
				Analytics.Context,
				eventId,
				Analytics.ToJavaHashMap(attributes)
			});
		}

		public static void EventBegin(string eventId)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventBegin", new object[]
			{
				Analytics.Context,
				eventId
			});
		}

		public static void EventEnd(string eventId)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventEnd", new object[]
			{
				Analytics.Context,
				eventId
			});
		}

		public static void EventBegin(string eventId, string label)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventBegin", new object[]
			{
				Analytics.Context,
				eventId,
				label
			});
		}

		public static void EventEnd(string eventId, string label)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventEnd", new object[]
			{
				Analytics.Context,
				eventId,
				label
			});
		}

		public static void EventBeginWithPrimarykeyAndAttributes(string eventId, string primaryKey, Dictionary<string, string> attributes)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onKVEventBegin", new object[]
			{
				Analytics.Context,
				eventId,
				Analytics.ToJavaHashMap(attributes),
				primaryKey
			});
		}

		public static void EventEndWithPrimarykey(string eventId, string primaryKey)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onKVEventEnd", new object[]
			{
				Analytics.Context,
				eventId,
				primaryKey
			});
		}

		public static void EventDuration(string eventId, int milliseconds)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventDuration", new object[]
			{
				Analytics.Context,
				eventId,
				(long)milliseconds
			});
		}

		public static void EventDuration(string eventId, string label, int milliseconds)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventDuration", new object[]
			{
				Analytics.Context,
				eventId,
				label,
				(long)milliseconds
			});
		}

		public static void EventDuration(string eventId, Dictionary<string, string> attributes, int milliseconds)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onEventDuration", new object[]
			{
				Analytics.Context,
				eventId,
				Analytics.ToJavaHashMap(attributes),
				(long)milliseconds
			});
		}

		public static void PageBegin(string pageName)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onPageStart", new object[]
			{
				pageName
			});
		}

		public static void PageEnd(string pageName)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onPageEnd", new object[]
			{
				pageName
			});
		}

		public static void Event(string eventId, Dictionary<string, string> attributes, int value)
		{
			try
			{
				if (attributes == null)
				{
					attributes = new Dictionary<string, string>();
				}
				if (attributes.ContainsKey("__ct__"))
				{
					attributes["__ct__"] = value.ToString();
					Analytics.Event(eventId, attributes);
				}
				else
				{
					attributes.Add("__ct__", value.ToString());
					Analytics.Event(eventId, attributes);
					attributes.Remove("__ct__");
				}
			}
			catch (Exception)
			{
			}
		}

		public static string GetDeviceInfo()
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return string.Empty;
			}
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.umeng.analytics.UnityUtil");
			return androidJavaClass.CallStatic<string>("getDeviceInfo", new object[]
			{
				Analytics.Context
			});
		}

		public static void SetLogEncryptEnabled(bool value)
		{
			Analytics.Agent.CallStatic("enableEncrypt", new object[]
			{
				value
			});
		}

		public static void SetLatency(int value)
		{
			Analytics.Agent.CallStatic("setLatencyWindow", new object[]
			{
				(long)value
			});
		}

		public static void Event(string[] keyPath, int value, string label)
		{
			string text = string.Join(";=umengUnity=;", keyPath);
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.umeng.analytics.UnityUtil");
			androidJavaClass.CallStatic("onEventForUnity", new object[]
			{
				Analytics.Context,
				text,
				value,
				label
			});
		}

		public static void SetContinueSessionMillis(long milliseconds)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("setSessionContinueMillis", new object[]
			{
				milliseconds
			});
		}

		[Obsolete("Flush")]
		public static void Flush()
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("flush", new object[]
			{
				Analytics.Context
			});
		}

		[Obsolete("SetEnableLocation已弃用")]
		public static void SetEnableLocation(bool reportLocation)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("setAutoLocation", new object[]
			{
				reportLocation
			});
		}

		public static void EnableActivityDurationTrack(bool isTraceActivity)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("openActivityDurationTrack", new object[]
			{
				isTraceActivity
			});
		}

		public static void SetCheckDevice(bool value)
		{
			Analytics.Agent.CallStatic("setCheckDevice", new object[]
			{
				value
			});
		}

		public static string AppKey
		{
			get
			{
				return Analytics._AppKey;
			}
		}

		public static string ChannelId
		{
			get
			{
				return Analytics._ChannelId;
			}
		}

		private static void CreateUmengManger()
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<UmengManager>();
			gameObject.name = "UmengManager";
		}

		public static void onResume()
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onResume", new object[]
			{
				Analytics.Context
			});
		}

		public static void onResume(string appkey, string channelId)
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onResume", new object[]
			{
				Analytics.Context,
				appkey,
				channelId
			});
		}

		public static void onPause()
		{
			if (PlatSDKMgr.PlatName == "QH360")
			{
				return;
			}
			Analytics.Agent.CallStatic("onPause", new object[]
			{
				Analytics.Context
			});
		}

		public static void onKillProcess()
		{
			Analytics.Agent.CallStatic("onKillProcess", new object[]
			{
				Analytics.Context
			});
		}

		private static AndroidJavaObject ToJavaHashMap(Dictionary<string, string> dic)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap", new object[0]);
			IntPtr methodID = AndroidJNIHelper.GetMethodID(androidJavaObject.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
			object[] array = new object[2];
			foreach (KeyValuePair<string, string> keyValuePair in dic)
			{
				using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("java.lang.String", new object[]
				{
					keyValuePair.Key
				}))
				{
					using (AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.lang.String", new object[]
					{
						keyValuePair.Value
					}))
					{
						array[0] = androidJavaObject2;
						array[1] = androidJavaObject3;
						AndroidJNI.CallObjectMethod(androidJavaObject.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(array));
					}
				}
			}
			return androidJavaObject;
		}

		protected static AndroidJavaClass Agent
		{
			get
			{
				return Analytics.SingletonHolder.instance_mobclick;
			}
		}

		protected static AndroidJavaClass UpdateAgent
		{
			get
			{
				if (Analytics._UpdateAgent == null)
				{
					Analytics._UpdateAgent = new AndroidJavaClass("com.umeng.update.UmengUpdateAgent");
				}
				return Analytics._UpdateAgent;
			}
		}

		protected static AndroidJavaObject Context
		{
			get
			{
				return Analytics.SingletonHolder.instance_context;
			}
		}

		public static void UMGameAgentInit()
		{
			Analytics.Agent.CallStatic("initUnity", new object[]
			{
				Analytics.Context
			});
		}

		public void Dispose()
		{
			Analytics.Agent.Dispose();
			Analytics.Context.Dispose();
		}

		public static void ShowFB()
		{
		}

		public static void ShowFB(string content)
		{
		}

		private static bool hasInit;

		private static string _AppKey;

		private static string _ChannelId;

		private static AndroidJavaClass _UpdateAgent;

		private static class SingletonHolder
		{
			static SingletonHolder()
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					Analytics.SingletonHolder.instance_context = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
			}

			public static AndroidJavaClass instance_mobclick = new AndroidJavaClass("com.umeng.analytics.game.UMGameAgent");

			public static AndroidJavaObject instance_context;
		}
	}
}
