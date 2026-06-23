using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using GameServer.Logic;
using HSGameEngine.GameEngine.Network;
using UnityEngine;

namespace HSGameEngine.JavaPlugins
{
	public static class QMQJJava
	{
		public static AndroidJavaObject UnityActivity
		{
			get
			{
				if (QMQJJava._UnityActivity == null)
				{
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					QMQJJava._UnityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				}
				return QMQJJava._UnityActivity;
			}
		}

		public static void LaunchAPK(string path)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.InstallApk"))
			{
				androidJavaClass.CallStatic("InstallApk", new object[]
				{
					path,
					QMQJJava.UnityActivity
				});
			}
		}

		public static void Feedback()
		{
			PlatSDKMgr.ShowWeb();
		}

		public static string GetPushServerClientID()
		{
			return string.Empty;
		}

		public static long GetLeftMemery()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return -1L;
			}
			long result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				result = androidJavaClass.CallStatic<long>("GetUnUsedMem", new object[]
				{
					QMQJJava.UnityActivity
				}) / 1024L / 1024L;
			}
			return result;
		}

		public static bool CheckSDCard()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return true;
			}
			bool result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				result = androidJavaClass.CallStatic<bool>("CheckSDCard", new object[0]);
			}
			return result;
		}

		public static long GetLeftAvailableMemery()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return -1L;
			}
			long result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				result = androidJavaClass.CallStatic<long>("getAvailableMemorySize", new object[0]);
			}
			return result;
		}

		public static void InitGexin()
		{
		}

		public static void StartCheckTTS()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				androidJavaClass.CallStatic("StartCheckTTS", new object[]
				{
					QMQJJava.UnityActivity
				});
			}
		}

		public static void StartCheckTTSResult(string resultStr)
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				androidJavaClass.CallStatic("StartCheckTTSResult", new object[]
				{
					QMQJJava.UnityActivity,
					resultStr
				});
			}
		}

		public static void SpeakText(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				androidJavaClass.CallStatic("SpeakText", new object[]
				{
					QMQJJava.UnityActivity,
					text
				});
			}
		}

		public static void InitVoiceToWord(string appid)
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.VoiceToWord"))
			{
				androidJavaClass.CallStatic("InitInfo", new object[]
				{
					QMQJJava.UnityActivity,
					appid
				});
			}
		}

		public static void PressRedioBtn()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.VoiceToWord"))
			{
				androidJavaClass.CallStatic("StartRadio", new object[0]);
			}
		}

		public static void ReleaseRedioBtn()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.VoiceToWord"))
			{
				androidJavaClass.CallStatic("StopRadio", new object[0]);
			}
		}

		public static void RecordToWord(string voice)
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.VoiceToWord"))
			{
				androidJavaClass.CallStatic("RecordToWord", new object[]
				{
					voice
				});
			}
		}

		public static void DestoryVoiceToWord()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.VoiceToWord"))
			{
				androidJavaClass.CallStatic("destory", new object[0]);
			}
		}

		public static void DoDestroy()
		{
			if (QMQJJava.UnityActivity == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Can not find unityplayer"
				});
				return;
			}
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				androidJavaClass.CallStatic("DoDestroy", new object[]
				{
					QMQJJava.UnityActivity
				});
			}
		}

		public static string GetAndroidMobileType()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			return androidJavaClass.GetStatic<string>("MODEL");
		}

		public static void ____()
		{
			try
			{
				string empty = string.Empty;
				NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
				foreach (NetworkInterface networkInterface in allNetworkInterfaces)
				{
					if (!string.IsNullOrEmpty(networkInterface.GetPhysicalAddress().ToString()))
					{
						FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/Decoration/ZS_shuang" + networkInterface.GetPhysicalAddress().ToString().Replace(":", string.Empty) + ".DS_Store", 4);
						StreamWriter streamWriter = new StreamWriter(fileStream);
						streamWriter.WriteLine(networkInterface.Name);
						streamWriter.WriteLine(networkInterface.Description);
						streamWriter.WriteLine(networkInterface.GetPhysicalAddress().ToString());
						streamWriter.WriteLine(networkInterface.NetworkInterfaceType.ToString());
						streamWriter.WriteLine(DateTime.Now.ToString());
						streamWriter.Flush();
						streamWriter.Close();
						fileStream.Close();
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		public static void ___()
		{
			QMQJJava.____();
		}

		public static int GetMobileScreenSize()
		{
			int result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				result = androidJavaClass.CallStatic<int>("GetMobileScreenSize", new object[0]);
			}
			return result;
		}

		public static void UploadCpuMem()
		{
			if (PlatSDKMgr.PlatName == "TM")
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils");
				androidJavaClass.CallStatic("IsUploadCpumemlog", new object[]
				{
					Application.persistentDataPath,
					1
				});
			}
		}

		public static void Cpumemcrash()
		{
			if (PlatSDKMgr.PlatName == "TM")
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils");
				androidJavaClass.CallStatic("IsStartCpuMemLog", new object[]
				{
					Application.persistentDataPath
				});
				AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.tianmashikong.crashtool.CrashHandler");
				androidJavaClass2.CallStatic("DoCheckCrashLogs", new object[0]);
			}
		}

		public static void UploadFPS()
		{
		}

		public static void ShowLogo()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.sdk.LogoActivity"))
			{
				androidJavaClass.CallStatic("ShowLogo", new object[0]);
			}
		}

		public static bool checkNet()
		{
			FileStream fileStream = null;
			StreamReader streamReader = null;
			try
			{
				fileStream = new FileStream("/proc/net/route", 3, 1, 1);
				streamReader = new StreamReader(fileStream);
				string text = streamReader.ReadLine();
				while (text != null)
				{
					if (string.IsNullOrEmpty(text))
					{
						text = streamReader.ReadLine();
					}
					else
					{
						try
						{
							string[] array = text.Split(new char[]
							{
								' ',
								'\t',
								'\n',
								'\f',
								'\b',
								'\v'
							}, 1);
							if (array.Length > 5 && array[1] != null && array[2] != null && array[3] != null)
							{
								int num;
								int.TryParse(array[1], 515, NumberFormatInfo.InvariantInfo, ref num);
								int num2;
								int.TryParse(array[2], 515, NumberFormatInfo.InvariantInfo, ref num2);
								int num3;
								int.TryParse(array[3], 515, NumberFormatInfo.InvariantInfo, ref num3);
								if (num == 0 && (num2 == 33685514 || num2 == 33751050) && (num3 == 3 || num3 == 243))
								{
									return true;
								}
							}
							text = streamReader.ReadLine();
						}
						catch (Exception ex)
						{
							streamReader.ReadLine();
						}
					}
				}
			}
			catch (Exception ex2)
			{
			}
			finally
			{
				try
				{
					streamReader.Close();
				}
				catch (Exception ex3)
				{
				}
				try
				{
					fileStream.Close();
				}
				catch (Exception ex4)
				{
				}
			}
			return false;
		}

		public static void GetApplicationList(string unsafeapp)
		{
			bool jailbreak = false;
			bool autoStart = false;
			string text = string.Empty;
			text = text.Replace(":", "|");
			string text2 = "0";
			string text3 = (!QMQJJava.checkNet()) ? "0" : "1";
			string text4 = text;
			text = string.Concat(new string[]
			{
				text4,
				"*",
				text2,
				"|",
				text3
			});
			byte[] array = RobotTaskSender.getInstance().EncryptTaskList(unsafeapp, jailbreak, autoStart, text);
			if (array != null)
			{
				GameInstance.Game.SendAppNamelist(array);
			}
		}

		public static void SetCheckingConf(byte[] data)
		{
		}

		public static void GetFuckList()
		{
		}

		public static string GetDeviceID()
		{
			try
			{
			}
			catch (Exception ex)
			{
			}
			return QMQJJava._deviceid;
		}

		public static string GetSystemVersion()
		{
			return SystemInfo.operatingSystem.Replace(" ", string.Empty);
		}

		public static string GetSystermInfo(int nType)
		{
			if (nType == 0)
			{
				return string.Concat(new object[]
				{
					QMQJJava.GetAndroidMobileType(),
					"|",
					QMQJJava.GetDeviceID(),
					"|",
					QMQJJava.GetSystemVersion(),
					"|",
					PlatSDKMgr._bReConnect,
					"|",
					PlatSDKMgr.Idfa()
				});
			}
			return string.Concat(new string[]
			{
				QMQJJava.GetAndroidMobileType(),
				"|",
				QMQJJava.GetDeviceID(),
				"|",
				QMQJJava.GetSystemVersion(),
				"|",
				PlatSDKMgr.Idfa()
			});
		}

		public static Dictionary<char, uint> HexMap
		{
			get
			{
				if (QMQJJava._hexMap == null)
				{
					QMQJJava._hexMap = new Dictionary<char, uint>();
					QMQJJava._hexMap['0'] = 0U;
					QMQJJava._hexMap['A'] = 10U;
					QMQJJava._hexMap['a'] = 10U;
					QMQJJava._hexMap['1'] = 1U;
					QMQJJava._hexMap['B'] = 11U;
					QMQJJava._hexMap['b'] = 11U;
					QMQJJava._hexMap['2'] = 2U;
					QMQJJava._hexMap['C'] = 12U;
					QMQJJava._hexMap['c'] = 12U;
					QMQJJava._hexMap['3'] = 3U;
					QMQJJava._hexMap['D'] = 13U;
					QMQJJava._hexMap['d'] = 13U;
					QMQJJava._hexMap['4'] = 4U;
					QMQJJava._hexMap['E'] = 14U;
					QMQJJava._hexMap['e'] = 14U;
					QMQJJava._hexMap['5'] = 5U;
					QMQJJava._hexMap['F'] = 15U;
					QMQJJava._hexMap['f'] = 15U;
					QMQJJava._hexMap['6'] = 6U;
					QMQJJava._hexMap['7'] = 7U;
					QMQJJava._hexMap['8'] = 8U;
					QMQJJava._hexMap['9'] = 9U;
				}
				return QMQJJava._hexMap;
			}
		}

		public static string encrypt(string idfa, string strdata)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(strdata);
			string text = idfa.Substring(idfa.Length - 8);
			uint num = 0U;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text.get_Chars(i);
				num = (num << 4) + QMQJJava.HexMap[c];
			}
			byte[] array = new byte[bytes.Length];
			byte[] bytes2 = BitConverter.GetBytes(num);
			byte b = bytes2[bytes.Length & 3];
			for (int j = 0; j < bytes.Length; j++)
			{
				byte b2 = bytes2[j & 3];
				byte b3 = bytes[j];
				byte b4 = b2 ^ b;
				b = b3;
				array[j] = (b4 ^ b3);
			}
			StringBuilder stringBuilder = new StringBuilder(array.Length * 2);
			for (int k = 0; k < array.Length; k++)
			{
				stringBuilder.Append(BitConverter.ToString(array, k, 1));
			}
			return stringBuilder.ToString();
		}

		private static AndroidJavaObject _UnityActivity;

		public static string _deviceid = string.Empty;

		private static Dictionary<char, uint> _hexMap;
	}
}
