using System;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf;

namespace CheckSysValueDll
{
	[ProtoContract]
	public class CheckValueResult
	{
		public CheckValueResult()
		{
			this.Info = "";
			this.ResultDict = new Dictionary<string, List<CheckValueResultItem>>();
		}

		public void AddData(object obj, string StrSeach)
		{
			CheckValueResultItem checkValueResultItem = new CheckValueResultItem();
			if (null == obj)
			{
				checkValueResultItem.TypeName = "null";
				checkValueResultItem.StrValue = "null";
			}
			else
			{
				checkValueResultItem.TypeName = obj.GetType().Name;
				checkValueResultItem.StrValue = obj;
			}
			CheckValueResult.Data2String(ref checkValueResultItem);
			if (this.ResultDict.ContainsKey(StrSeach))
			{
				this.ResultDict[StrSeach].Add(checkValueResultItem);
			}
			else
			{
				this.ResultDict.Add(StrSeach, new List<CheckValueResultItem>
				{
					checkValueResultItem
				});
			}
		}

		private static void Data2String(ref CheckValueResultItem model)
		{
			model.Childs = new List<string>();
			FieldInfo[] fields = model.StrValue.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields.Length < 1)
			{
				model.StrValue = CheckModel.Data2Json(model.StrValue);
			}
			else
			{
				foreach (FieldInfo fieldInfo in fields)
				{
					object value = fieldInfo.GetValue(model.StrValue);
					model.Childs.Add(string.Format("{0},{1}", fieldInfo.Name, value));
				}
				model.StrValue = model.StrValue.ToString();
			}
		}

		[ProtoMember(1)]
		public Dictionary<string, List<CheckValueResultItem>> ResultDict;

		[ProtoMember(2)]
		public string Info;
	}
}
