using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using UnityEngine;

public class CheckSFAndWorker : MonoBehaviour
{
	public static CheckSFAndWorker SingleInstance
	{
		get
		{
			if (null == CheckSFAndWorker._SingleInstance)
			{
				CheckSFAndWorker._SingleInstance = new GameObject("fucksf")
				{
					layer = LayerMask.NameToLayer("MUUI")
				}.AddComponent<CheckSFAndWorker>();
			}
			return CheckSFAndWorker._SingleInstance;
		}
	}

	public void Init()
	{
		base.StartCoroutine(this.MyLogic());
	}

	private void Awake()
	{
		this.startTime = Time.time;
	}

	private void Start()
	{
		string empty = string.Empty;
		this.mSubject = string.Format("{0}{1}_r{2}", DateTime.Now.ToString("yyyy_MM_dd_HH-mm-ss_i"), empty, Random.Range(0, 1000000));
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Update()
	{
	}

	private bool isLocalNetwork(string url)
	{
		string text = "(192\\.168)(\\.(25[0-5]|2[0-4]\\d|1\\d{2}|[1-9]\\d|\\d)){2}";
		string text2 = "(172\\.)(1[6-9]|2[0-9]|3[0-1])(\\.(25[0-5]|2[0-4]\\d|1\\d{2}|[1-9]\\d|\\d)){2}";
		string text3 = "(10)(\\.(25[0-5]|2[0-4]\\d|1\\d{2}|[1-9]\\d|\\d)){3}";
		return Regex.IsMatch(url, text) || Regex.IsMatch(url, text2) || Regex.IsMatch(url, text3);
	}

	public static bool IsVirtualMachine()
	{
		return CheckSFAndWorker.Doim();
	}

	private IEnumerator MyLogic()
	{
		string serlist_root = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "serverlisturl");
		string verify_root = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "verifyaccountserverip");
		string versionurl = string.Empty;
		versionurl = Global.ReadXmlConfigStr(Global.NetVersionXML, "Info", "versionurl");
		string[] StdUrlList = new string[]
		{
			"api1.android.qmqj.xy.com",
			"api1.ios.qmqj.xy.com",
			"api1.qmqj.xy.com",
			"login-muorigin.webzen.com",
			"alpha-kr-mapi.webzen.com",
			"login.mu.kimi.com.tw",
			"119.29.10.118",
			"115.159.1.140",
			"115.159.72.41",
			"115.159.75.99",
			"192.167.0.100",
			"192.168.0.100",
			"202.31.213.131",
			"202.31.212.8",
			"spare.qj.tianmashikong.com",
			"spare2.qj.tianmashikong.com",
			"spare3.qj.tianmashikong.com",
			"sdk.mu.siamgame.in.th",
			"f-th.koramgame.com",
			"spare4.qj.tianmashikong.com",
			"125.141.215.2"
		};
		string[] StdUrlList2 = new string[]
		{
			".webzen.com",
			".xy.com",
			".3dcq.com",
			".tmsk.cn",
			".tianmashikong.com",
			".tmskapp.com",
			"sed.com",
			"eew.com",
			".webzen.co.kr",
			".awww.com",
			".kimi.com.tw",
			"xxxx.add",
			"192.168.1.121"
		};
		bool bVirtualMachine = false;
		string phone_number = string.Empty;
		string imsi = string.Empty;
		string locationDetail = string.Empty;
		serlist_root = serlist_root.Replace("http://", string.Empty);
		serlist_root = serlist_root.Replace("https://", string.Empty);
		verify_root = verify_root.Replace("http://", string.Empty);
		verify_root = verify_root.Replace("https://", string.Empty);
		versionurl = versionurl.Replace("http://", string.Empty);
		versionurl = versionurl.Replace("https://", string.Empty);
		int idx = serlist_root.IndexOf("/", 0);
		if (idx > -1)
		{
			serlist_root = serlist_root.Substring(0, idx);
			serlist_root = serlist_root.Split(new char[]
			{
				':'
			})[0];
			MUDebug.Log<string>(new string[]
			{
				"serlist_root=" + serlist_root
			});
		}
		int idx2 = verify_root.IndexOf("/", 0);
		if (idx2 > -1)
		{
			verify_root = verify_root.Substring(0, idx2);
			verify_root = verify_root.Split(new char[]
			{
				':'
			})[0];
			MUDebug.Log<string>(new string[]
			{
				"verify_root=" + verify_root
			});
		}
		int idx3 = versionurl.IndexOf("/", 0);
		if (idx3 > -1)
		{
			versionurl = versionurl.Substring(0, idx3);
			versionurl = versionurl.Split(new char[]
			{
				':'
			})[0];
			MUDebug.Log<string>(new string[]
			{
				"version_url=" + versionurl
			});
		}
		bool server_list_ok = false;
		bool verify_ok = false;
		bool version_ok = false;
		int count = StdUrlList.Length;
		for (int i = 0; i < count; i++)
		{
			if (serlist_root.Equals(StdUrlList[i]))
			{
				server_list_ok = true;
			}
			if (verify_root.Equals(StdUrlList[i]))
			{
				verify_ok = true;
			}
			if (versionurl.Equals(StdUrlList[i]))
			{
				version_ok = true;
			}
		}
		count = StdUrlList2.Length;
		for (int j = 0; j < count; j++)
		{
			if (serlist_root.EndsWith(StdUrlList2[j]))
			{
				server_list_ok = true;
			}
			if (verify_root.EndsWith(StdUrlList2[j]))
			{
				verify_ok = true;
			}
			if (versionurl.EndsWith(StdUrlList2[j]))
			{
				version_ok = true;
			}
		}
		if (server_list_ok && verify_ok && version_ok)
		{
			Object.DestroyObject(base.gameObject);
			yield break;
		}
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"error url app can not run,server=",
				server_list_ok,
				" verify=",
				verify_ok,
				" version=",
				version_ok
			})
		});
		Application.Quit();
		yield return new WaitForEndOfFrame();
		phone_number = string.Empty;
		locationDetail = string.Empty;
		yield return new WaitForEndOfFrame();
		StringBuilder sb = new StringBuilder();
		sb.Append("serverListUrl:" + serlist_root + "\r\n");
		sb.Append("verifyAccountIp:" + verify_root + "\r\n");
		sb.Append("myIp:" + Network.player.ipAddress + "\r\n");
		sb.Append("vm:" + bVirtualMachine + "\r\n");
		sb.Append("phone_number:" + phone_number + "\r\n");
		sb.Append("locationDetail:" + locationDetail + "\r\n");
		if (!this.isLocalNetwork(serlist_root) && !this.isLocalNetwork(verify_root))
		{
			MUDebug.Log<string>(new string[]
			{
				"----------errr----------url----"
			});
			Application.Quit();
		}
		this.startTime = Time.time;
		yield break;
	}

	private string getLocationDetail()
	{
		string result;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.SignalLocationUtil"))
		{
			result = androidJavaClass.CallStatic<string>("GetLocationDetail", new object[]
			{
				true
			});
		}
		return result;
	}

	private void SendMail(string title, string content)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
		{
			androidJavaClass.CallStatic("SendMail", new object[]
			{
				"muclient@tmsk.cn",
				"d4iiUa8PbAQkG6A4",
				"wufc@tmsk.cn",
				title,
				content
			});
		}
	}

	public static string GetAnJianJingling()
	{
		string text = "0";
		string text2 = "0";
		if (CheckSFAndWorker.Doim())
		{
			text = "1";
		}
		if (CheckSFAndWorker.Dodododo())
		{
			text2 = "1";
		}
		return text + "-" + text2;
	}

	public static void Initlib()
	{
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
			{
				androidJavaClass.CallStatic("InitTelephony", new object[0]);
			}
		}
		catch (UnityException ex)
		{
			Application.Quit();
		}
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern IntPtr gbi();

	public static string DoGbi()
	{
		try
		{
			IntPtr intPtr = CheckSFAndWorker.gbi();
			string result = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}
		catch (Exception ex)
		{
			Application.Quit();
		}
		return string.Empty;
	}

	public static void GetWaigualist(string idlist)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
		{
			androidJavaClass.CallStatic("CheckUnsafeApp", new object[]
			{
				idlist
			});
		}
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern IntPtr gpl();

	public static string Dogpl()
	{
		try
		{
			string processList = CheckSFAndWorker.getProcessList();
			if (CheckSFAndWorker._appMap == null)
			{
				CheckSFAndWorker._appMap = new Dictionary<string, int>();
				string[] array = processList.Split(new char[]
				{
					'|'
				});
			}
			CheckSFAndWorker.GetWaigualist(processList);
			return processList;
		}
		catch (Exception ex)
		{
		}
		return string.Empty;
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern IntPtr gci();

	public static string DoGci()
	{
		string result;
		try
		{
			IntPtr intPtr = CheckSFAndWorker.gci();
			string text = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			result = text;
		}
		catch (Exception ex)
		{
			Application.Quit();
			result = string.Empty;
		}
		return result;
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern IntPtr gpi();

	public static string DoGpi()
	{
		try
		{
			IntPtr intPtr = CheckSFAndWorker.gpi();
			string result = Marshal.PtrToStringAnsi(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}
		catch (Exception ex)
		{
			Application.Quit();
		}
		return string.Empty;
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern bool dododo(int exitmode);

	public static bool Dodododo()
	{
		try
		{
			return CheckSFAndWorker.dododo(0);
		}
		catch (Exception ex)
		{
			Application.Quit();
		}
		return true;
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern int fkv(string path);

	public static int Dofkv()
	{
		try
		{
			return CheckSFAndWorker.fkv(Application.streamingAssetsPath);
		}
		catch (Exception ex)
		{
			Application.Quit();
		}
		return 1;
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern bool ir();

	public static bool Doir()
	{
		try
		{
			return CheckSFAndWorker.ir();
		}
		catch (Exception ex)
		{
			Application.Quit();
		}
		return true;
	}

	[DllImport("TinyXml10", ThrowOnUnmappableChar = true)]
	public static extern bool im();

	public static bool Doim()
	{
		try
		{
			return CheckSFAndWorker.im();
		}
		catch (Exception ex)
		{
			Application.Quit();
		}
		return true;
	}

	public static void ChkUsbd()
	{
	}

	public static bool checkNet()
	{
		return CheckSFAndWorker.isVmCpu();
	}

	public static void GetApplicationList(string unsafeapp)
	{
		bool jailbreak = false;
		bool autoStart = false;
		string text = string.Empty;
		CheckSFAndWorker._unsafeApp = unsafeapp;
		if (!PlatSDKMgr._hasEntergame)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
		{
			autoStart = (androidJavaClass.CallStatic<string>("GetAppStartType", new object[0]) == "1");
			jailbreak = (androidJavaClass.CallStatic<string>("getRootAhth", new object[0]) == "1");
			text = string.Concat(new string[]
			{
				CheckSFAndWorker.BuildInfo(),
				"*",
				CheckSFAndWorker.getCpuInfo(),
				"*",
				CheckSFAndWorker.GetPhoneInfo()
			});
		}
		text = text.Replace(":", "|").Trim(new char[]
		{
			' ',
			'\t',
			'\n',
			'\f',
			'\b',
			'\v'
		});
		string text2 = (!CheckSFAndWorker.isVmCpu()) ? "0" : "1";
		string text3 = (!CheckSFAndWorker.checkNet()) ? "0" : "1";
		string text4 = text;
		text = string.Concat(new object[]
		{
			text4,
			"*",
			text2,
			"|",
			text3,
			"|",
			CheckSFAndWorker.Checksimulator()
		});
		byte[] array = RobotTaskSender.getInstance().EncryptTaskList(CheckSFAndWorker._unsafeApp, jailbreak, autoStart, text);
		if (array != null)
		{
			GameInstance.Game.SendAppNamelist(array);
			MUDebug.Log<string>(new string[]
			{
				"RobotTaskSend" + CheckSFAndWorker._unsafeApp
			});
		}
	}

	public static int Checksimulator()
	{
		int result = 0;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils");
			result = androidJavaClass.CallStatic<int>("CheckSimulator", new object[0]);
		}
		catch (Exception ex)
		{
		}
		return result;
	}

	public static string getCmdLine(string number)
	{
		try
		{
			string text = string.Format("/proc/{0}/cmdline", number);
			string text2 = File.ReadAllText(text);
			text2 = text2.Trim(new char[]
			{
				' ',
				'\t',
				'\r',
				'\n'
			});
			char[] array = text2.ToCharArray();
			string text3 = string.Empty;
			if (text2.StartsWith("/"))
			{
				return null;
			}
			if (text2.StartsWith("."))
			{
				return null;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] > '\u001f' && array[i] < '\u0080')
				{
					text3 += array[i].ToString();
				}
			}
			return text3;
		}
		catch
		{
		}
		return null;
	}

	public static string getProcessList()
	{
		string[] directories = Directory.GetDirectories("/proc");
		if (directories == null)
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		for (int i = 0; i < directories.Length; i++)
		{
			if (!string.IsNullOrEmpty(directories[i]))
			{
				string text = directories[i].Substring("/proc/".Length);
				if (!text.StartsWith("."))
				{
					long num = 0L;
					if (long.TryParse(text, ref num))
					{
						string cmdLine = CheckSFAndWorker.getCmdLine(text);
						if (!string.IsNullOrEmpty(cmdLine))
						{
							if (!flag)
							{
								stringBuilder.Append("|");
								stringBuilder.Append(cmdLine);
							}
							else
							{
								stringBuilder.Append(cmdLine);
								flag = false;
							}
						}
					}
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static string getCpuInfo()
	{
		string text = string.Empty;
		try
		{
			using (FileStream fileStream = new FileStream("/proc/cpuinfo", 3, 1))
			{
				byte[] array = new byte[1024];
				int num = fileStream.Read(array, 0, 1024);
				text = Encoding.UTF8.GetString(array);
				text = text.ToLower().Trim();
				fileStream.Close();
			}
		}
		catch
		{
		}
		return text;
	}

	public static string GetPhoneInfo()
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils");
			string text = androidJavaClass.CallStatic<string>("getPhoneInfo", new object[0]);
			return text.Replace(":", "|").Trim();
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"getphoneinfo error " + ex.ToString()
			});
		}
		return string.Empty;
	}

	public static string BuildInfo()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.os.Build$VERSION");
		string @static = androidJavaClass.GetStatic<string>("MODEL");
		string static2 = androidJavaClass.GetStatic<string>("MODEL");
		string static3 = androidJavaClass.GetStatic<string>("PRODUCT");
		string static4 = androidJavaClass2.GetStatic<string>("SDK");
		string static5 = androidJavaClass.GetStatic<string>("HARDWARE");
		string static6 = androidJavaClass.GetStatic<string>("DEVICE");
		string static7 = androidJavaClass.GetStatic<string>("HOST");
		string static8 = androidJavaClass.GetStatic<string>("CPU_ABI");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("HARDWARE|" + static5 + ",");
		stringBuilder.Append("DEVICE|" + static6 + ",");
		stringBuilder.Append("HOST|" + static7 + ",");
		stringBuilder.Append("MODEL|" + @static + ",");
		stringBuilder.Append("MANUFACTURER|" + static2 + ",");
		stringBuilder.Append("PRODUCT|" + static3 + ",");
		stringBuilder.Append("SDK|" + static4 + ",");
		stringBuilder.Append("ABI|" + static8);
		return stringBuilder.ToString();
	}

	public static string GetArch()
	{
		string result = string.Empty;
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.System");
			result = androidJavaClass.CallStatic<string>("getProperty", new object[]
			{
				"os.arch"
			});
		}
		catch (Exception ex)
		{
		}
		return result;
	}

	public static string CPU_ABI()
	{
		try
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils");
			return androidJavaClass.CallStatic<string>("GetCPU_ABI", new object[0]);
		}
		catch (Exception ex)
		{
		}
		return string.Empty;
	}

	public static bool isVmCpu()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
		string text = androidJavaClass.GetStatic<string>("HARDWARE").ToLower();
		string text2 = androidJavaClass.GetStatic<string>("SERIAL").ToLower();
		string text3 = CheckSFAndWorker.getCpuInfo().ToLower();
		if (string.IsNullOrEmpty(text3))
		{
			return false;
		}
		string arch = CheckSFAndWorker.GetArch();
		MUDebug.Log<string>(new string[]
		{
			"arch=" + arch
		});
		string text4 = CheckSFAndWorker.CPU_ABI();
		if (string.IsNullOrEmpty(text4))
		{
			text4 = androidJavaClass.GetStatic<string>("CPU_ABI");
		}
		MUDebug.Log<string>(new string[]
		{
			"abi=" + text4
		});
		if ((arch.Contains("i686") || arch.Contains("i386") || arch.Contains("i486") || arch.Contains("i586") || text4.Contains("x86")) && text3.IndexOf("atom") < 0)
		{
			return true;
		}
		if (text.Contains("amd") || text.Contains("google_sdk") || text.Contains("sdk") || text.Contains("sdk_x86") || text.Contains("vbox86"))
		{
			MUDebug.Log<string>(new string[]
			{
				"HARDWARE error=" + text
			});
			return true;
		}
		if (text2.Contains("00000000") || string.IsNullOrEmpty(text2) || text2.Equals("nox") || text2.Equals("unknown") || text2.Equals("null"))
		{
			MUDebug.Log<string>(new string[]
			{
				"SERIAL error=" + text2
			});
			return true;
		}
		return false;
	}

	private float startTime;

	public static CheckSFAndWorker _SingleInstance;

	public static string _unsafeApp = string.Empty;

	public static Dictionary<string, int> _appMap;

	private string mSubject = string.Empty;

	private string _serlisturl = string.Empty;

	private string _verifyurl = string.Empty;
}
