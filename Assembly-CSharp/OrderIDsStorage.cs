using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class OrderIDsStorage
{
	public static void Save()
	{
		if (OrderIDsStorage.OrderIdsDict != null)
		{
			if (OrderIDsStorage.OrderIdsDict.Count > 0)
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(DictionarySerializable<string, OrderIDsStorage.VerifyInfo>));
					xmlSerializer.Serialize(stringWriter, OrderIDsStorage.OrderIdsDict);
					PlayerPrefs.SetString("MU_OrderIDs", stringWriter.ToString());
					PlayerPrefs.Save();
				}
			}
			else if (PlayerPrefs.HasKey("MU_OrderIDs"))
			{
				PlayerPrefs.DeleteKey("MU_OrderIDs");
				PlayerPrefs.Save();
			}
		}
	}

	public static void Load()
	{
		if (PlayerPrefs.HasKey("MU_OrderIDs"))
		{
			string @string = PlayerPrefs.GetString("MU_OrderIDs");
			if (@string != null && @string.Length > 0)
			{
				using (StringReader stringReader = new StringReader(@string))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(DictionarySerializable<string, OrderIDsStorage.VerifyInfo>));
					OrderIDsStorage.OrderIdsDict = (DictionarySerializable<string, OrderIDsStorage.VerifyInfo>)xmlSerializer.Deserialize(stringReader);
				}
			}
		}
		else
		{
			OrderIDsStorage.OrderIdsDict = new DictionarySerializable<string, OrderIDsStorage.VerifyInfo>();
		}
	}

	public static void Add(string OrderId, OrderIDsStorage.VerifyInfo RecipientData)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		if (OrderIDsStorage.OrderIdsDict.ContainsKey(OrderId))
		{
			OrderIDsStorage.OrderIdsDict.Remove(OrderId);
		}
		OrderIDsStorage.OrderIdsDict.Add(OrderId, RecipientData);
	}

	public static void Del(string OrderId)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		if (OrderIDsStorage.OrderIdsDict.ContainsKey(OrderId))
		{
			OrderIDsStorage.OrderIdsDict.Remove(OrderId);
		}
	}

	public static bool Has(string key)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		return OrderIDsStorage.OrderIdsDict.ContainsKey(key);
	}

	public static void Clear()
	{
		if (PlayerPrefs.HasKey("MU_OrderIDs"))
		{
			PlayerPrefs.DeleteKey("MU_OrderIDs");
			PlayerPrefs.Save();
		}
		if (OrderIDsStorage.OrderIdsDict != null)
		{
			OrderIDsStorage.OrderIdsDict.Clear();
		}
	}

	public static void CheckOrders(Action<string, OrderIDsStorage.VerifyInfo> handler)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		if (handler != null)
		{
			foreach (KeyValuePair<string, OrderIDsStorage.VerifyInfo> keyValuePair in OrderIDsStorage.OrderIdsDict)
			{
				handler.Invoke(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	public static int OrdersCount()
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		return OrderIDsStorage.OrderIdsDict.Count;
	}

	public static bool HasOrders()
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		return OrderIDsStorage.OrderIdsDict.Count > 0;
	}

	public static OrderIDsStorage.VerifyInfo GetOrderInfo(string orderId)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		OrderIDsStorage.VerifyInfo result = null;
		if (OrderIDsStorage.OrderIdsDict.Count > 0 && OrderIDsStorage.OrderIdsDict.ContainsKey(orderId))
		{
			result = OrderIDsStorage.OrderIdsDict.GetValue(orderId);
		}
		return result;
	}

	public static void GetOrderInfo(out string orderId, out OrderIDsStorage.VerifyInfo verifyData, int verifyState = -1)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		orderId = string.Empty;
		verifyData = null;
		if (OrderIDsStorage.OrderIdsDict.Count > 0)
		{
			foreach (KeyValuePair<string, OrderIDsStorage.VerifyInfo> keyValuePair in OrderIDsStorage.OrderIdsDict)
			{
				if (keyValuePair.Value.verifyState == verifyState)
				{
					orderId = keyValuePair.Key;
					verifyData = keyValuePair.Value;
					break;
				}
			}
		}
	}

	public static void GetOrderInfoWithEmptyData(out string orderId, out OrderIDsStorage.VerifyInfo verifyData)
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		orderId = string.Empty;
		verifyData = null;
		if (OrderIDsStorage.OrderIdsDict.Count > 0)
		{
			foreach (KeyValuePair<string, OrderIDsStorage.VerifyInfo> keyValuePair in OrderIDsStorage.OrderIdsDict)
			{
				string text = Convert.ToBase64String(keyValuePair.Value.verifyData);
				if (text == "NULLDATA")
				{
					orderId = keyValuePair.Key;
					verifyData = keyValuePair.Value;
					break;
				}
			}
		}
	}

	public static List<string> GetValidOrderInfo()
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		List<string> list = new List<string>();
		if (OrderIDsStorage.OrderIdsDict.Count > 0)
		{
			foreach (KeyValuePair<string, OrderIDsStorage.VerifyInfo> keyValuePair in OrderIDsStorage.OrderIdsDict)
			{
				string text = Convert.ToBase64String(keyValuePair.Value.verifyData);
				if (text != "NULLDATA")
				{
					list.Add(keyValuePair.Key);
				}
			}
		}
		return list;
	}

	public static Dictionary<string, OrderIDsStorage.VerifyInfo> GetAllOrderInfo()
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		return OrderIDsStorage.OrderIdsDict;
	}

	public static void ClearOrderInfo()
	{
		if (OrderIDsStorage.OrderIdsDict == null)
		{
			OrderIDsStorage.Load();
		}
		OrderIDsStorage.OrderIdsDict.Clear();
	}

	private const string KEY_ORDERID = "MU_OrderIDs";

	private static DictionarySerializable<string, OrderIDsStorage.VerifyInfo> OrderIdsDict;

	public class VerifyInfo
	{
		public byte[] verifyData;

		public int money;

		public string verNum = string.Empty;

		public string TransID = string.Empty;

		public string countryCode = string.Empty;

		public string currencyCode = string.Empty;

		public int verifyState = -1;
	}
}
