using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ConfigSystemParam
{
	public static string GetSystemParamByName(string name, bool isCache = true)
	{
		string text = string.Empty;
		ConfigSystemParam._CacheStr.TryGetValue(name, ref text);
		if (text != null)
		{
			return text;
		}
		XElement gameResXml = Global.GetGameResXml("Config/SystemParams.Xml");
		if (gameResXml == null)
		{
			return null;
		}
		XElement xelement = Global.GetXElement(gameResXml, "Param", "Name", name);
		if (xelement == null)
		{
			return null;
		}
		text = Global.GetXElementAttributeStr(xelement, "Value");
		if (isCache)
		{
			ConfigSystemParam._CacheStr[name] = text;
		}
		return text;
	}

	public static long GetSystemParamIntByName(string name)
	{
		object obj = null;
		if (ConfigSystemParam._CachingSystemParams.TryGetValue(name, ref obj) && obj != null)
		{
			return (long)obj;
		}
		string systemParamByName = ConfigSystemParam.GetSystemParamByName(name, false);
		if (string.IsNullOrEmpty(systemParamByName))
		{
			return -1L;
		}
		try
		{
			long num = ConvertExt.SafeToInt64(systemParamByName);
			ConfigSystemParam._CachingSystemParams.Add(name, num);
			return num;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为整型时异常, {0}=>{1}"), name, systemParamByName)));
			MUDebug.LogException(ex);
		}
		return -1L;
	}

	public static double GetSystemParamDoubleByName(string name)
	{
		object obj = null;
		if (ConfigSystemParam._CachingSystemParams.TryGetValue(name, ref obj) && obj != null)
		{
			return (double)obj;
		}
		string systemParamByName = ConfigSystemParam.GetSystemParamByName(name, false);
		if (string.IsNullOrEmpty(systemParamByName))
		{
			return 0.0;
		}
		try
		{
			double num = ConvertExt.SafeConvertToDouble(systemParamByName);
			ConfigSystemParam._CachingSystemParams.Add(name, num);
			return num;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为浮点型时异常, {0}=>{1}"), name, systemParamByName)));
			MUDebug.LogException(ex);
		}
		return 0.0;
	}

	public static int[] GetSystemParamIntArrayByName(string name, char splitChar = ',')
	{
		int[] array = null;
		if (ConfigSystemParam._CachingIntParams.TryGetValue(name, ref array) && array != null)
		{
			return array;
		}
		string text = string.Empty;
		try
		{
			int[] array2 = ConfigSystemParam.IntArrayEmpty;
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array3 = text.Split(new char[]
				{
					splitChar
				});
				array2 = new int[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					int num = Global.SafeConvertToInt32(array3[i]);
					array2[i] = num;
				}
				ConfigSystemParam._CachingIntParams.Add(name, array2);
			}
			return array2;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为整型时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return null;
	}

	public static float[] GetSystemParamFloatArrayByName(string name, char splitChar = ',')
	{
		float[] array = null;
		if (ConfigSystemParam._CachingFloatParams.TryGetValue(name, ref array) && array != null)
		{
			return array;
		}
		string text = string.Empty;
		try
		{
			float[] array2 = ConfigSystemParam.FloatArrayEmpty;
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array3 = text.Split(new char[]
				{
					splitChar
				});
				array2 = new float[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					float num = ConvertExt.SafeConvertToFloat(array3[i], 0f);
					array2[i] = num;
				}
				ConfigSystemParam._CachingFloatParams.Add(name, array2);
			}
			return array2;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为整型时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return null;
	}

	public static double[] GetSystemParamDoubleArrayByName(string name)
	{
		double[] array = null;
		if (ConfigSystemParam._CachingDoubleParams.TryGetValue(name, ref array) && array != null)
		{
			return array;
		}
		string text = null;
		try
		{
			double[] array2 = ConfigSystemParam.DoubleArrayEmpty;
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array3 = text.Split(new char[]
				{
					','
				});
				array2 = new double[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					double num = ConvertExt.SafeConvertToDouble(array3[i]);
					array2[i] = num;
				}
				ConfigSystemParam._CachingDoubleParams.Add(name, array2);
			}
			return array2;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为整型时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return null;
	}

	public static string[] GetSystemParamStringArrayByName(string name, char splitChar = ',')
	{
		string[] array = null;
		if (ConfigSystemParam._CachingStringParams.TryGetValue(name, ref array) && array != null)
		{
			return array;
		}
		string text = string.Empty;
		try
		{
			string[] array2 = ConfigSystemParam.StringArrayEmpty;
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array3 = text.Split(new char[]
				{
					splitChar
				});
				array2 = new string[array3.Length];
				for (int i = 0; i < array3.Length; i++)
				{
					string text2 = array3[i];
					array2[i] = text2;
				}
				ConfigSystemParam._CachingStringParams.Add(name, array2);
			}
			return array2;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为整型时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return null;
	}

	public static HashSet<int> GetSystemParamIntSetByName(string name, char splitChar1 = ',')
	{
		object obj = null;
		if (ConfigSystemParam._CachingSystemParams.TryGetValue(name, ref obj) && obj != null)
		{
			return (HashSet<int>)obj;
		}
		HashSet<int> hashSet = new HashSet<int>();
		string text = string.Empty;
		try
		{
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array = text.Split(new char[]
				{
					splitChar1
				});
				for (int i = 0; i < array.Length; i++)
				{
					hashSet.Add(Global.SafeConvertToInt32(array[i]));
				}
				ConfigSystemParam._CachingSystemParams.Add(name, hashSet);
			}
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为HashSet<int>时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return hashSet;
	}

	public static Dictionary<int, int[]> GetSystemParamIntDictByName(string name, char splitChar1 = '|', char splitChar2 = ',')
	{
		object obj = null;
		if (ConfigSystemParam._CachingSystemParams.TryGetValue(name, ref obj) && obj != null)
		{
			return (Dictionary<int, int[]>)obj;
		}
		Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
		string text = string.Empty;
		try
		{
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array = text.Split(new char[]
				{
					splitChar1
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						splitChar2
					});
					int[] array3 = new int[array2.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						int num = Global.SafeConvertToInt32(array2[j]);
						array3[j] = num;
					}
					dictionary.Add(dictionary.Count, array3);
				}
				ConfigSystemParam._CachingSystemParams.Add(name, dictionary);
			}
			return dictionary;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为Dictionary<int, int[]>时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return dictionary;
	}

	public static Dictionary<int, double[]> GetSystemParamIntDoubleDictByName(string name, char splitChar1 = '|', char splitChar2 = ',')
	{
		object obj = null;
		if (ConfigSystemParam._CachingSystemParams.TryGetValue(name, ref obj) && obj != null)
		{
			return (Dictionary<int, double[]>)obj;
		}
		Dictionary<int, double[]> dictionary = new Dictionary<int, double[]>();
		string text = string.Empty;
		try
		{
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string[] array = text.Split(new char[]
				{
					splitChar1
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						splitChar2
					});
					double[] array3 = new double[array2.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						double num = ConvertExt.SafeConvertToDouble(array2[j]);
						array3[j] = num;
					}
					dictionary.Add((int)array3[0], array3);
				}
				ConfigSystemParam._CachingSystemParams.Add(name, dictionary);
			}
			return dictionary;
		}
		catch (Exception ex)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为Dictionary<int, int[]>时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex);
		}
		return dictionary;
	}

	public static Dictionary<int, int[]> GetSystemParamIntDict1ByName(string name, char splitChar1 = '|', char splitChar2 = ',')
	{
		object obj = null;
		if (ConfigSystemParam._CachingSystemParams.TryGetValue(name, ref obj) && obj != null)
		{
			return (Dictionary<int, int[]>)obj;
		}
		Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
		string text = string.Empty;
		try
		{
			text = ConfigSystemParam.GetSystemParamByName(name, false);
			if (string.IsNullOrEmpty(text))
			{
				return dictionary;
			}
			text = text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					splitChar1
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						splitChar2
					});
					int num = Global.SafeConvertToInt32(array2[0]);
					int[] array3 = new int[array2.Length];
					for (int j = 0; j < array2.Length; j++)
					{
						int num2 = Global.SafeConvertToInt32(array2[j]);
						array3[j] = num2;
					}
					try
					{
						dictionary.Add(num, array3);
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
					}
				}
				ConfigSystemParam._CachingSystemParams.Add(name, dictionary);
			}
			return dictionary;
		}
		catch (Exception ex2)
		{
			GError.AddErrMsg(new Exception(string.Format(Global.GetLang("将系统配置参数转为Dictionary<int, int[]>时异常, {0}=>{1}"), name, text)));
			MUDebug.LogException(ex2);
		}
		return dictionary;
	}

	public static void ClearData()
	{
		ConfigSystemParam._CachingSystemParams.Clear();
		ConfigSystemParam._CachingIntParams.Clear();
		ConfigSystemParam._CachingFloatParams.Clear();
		ConfigSystemParam._CachingDoubleParams.Clear();
		ConfigSystemParam._CachingStringParams.Clear();
		ConfigSystemParam._CacheStr.Clear();
	}

	private static Dictionary<string, object> _CachingSystemParams = new Dictionary<string, object>();

	private static Dictionary<string, int[]> _CachingIntParams = new Dictionary<string, int[]>();

	private static Dictionary<string, float[]> _CachingFloatParams = new Dictionary<string, float[]>();

	private static Dictionary<string, double[]> _CachingDoubleParams = new Dictionary<string, double[]>();

	private static Dictionary<string, string[]> _CachingStringParams = new Dictionary<string, string[]>();

	private static Dictionary<string, string> _CacheStr = new Dictionary<string, string>();

	private static float[] FloatArrayEmpty = new float[0];

	private static string[] StringArrayEmpty = new string[0];

	public static int[] IntArrayEmpty = new int[0];

	public static double[] DoubleArrayEmpty = new double[0];
}
