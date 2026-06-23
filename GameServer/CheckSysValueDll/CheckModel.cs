using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;
using Microsoft.CSharp.RuntimeBinder;

namespace CheckSysValueDll
{
	public class CheckModel
	{
		private static List<SeachData> CopySeachList(List<SeachData> data)
		{
			List<SeachData> list = new List<SeachData>();
			list.AddRange(data);
			return list;
		}

		private static bool IsIEnumerable(object model)
		{
			return model is IEnumerable<object>;
		}

		private static bool IsList(object model)
		{
			return model.GetType().Name.Equals(typeof(List<object>).Name);
		}

		private static bool IsArray(object model)
		{
			return model.GetType().Name.IndexOf("[]") > -1;
		}

		private static bool IsDict(object model)
		{
			return typeof(Dictionary<object, object>).Name.Equals(model.GetType().Name);
		}

		public static string Data2Json(object model)
		{
			string result = "err";
			try
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(model.GetType());
				using (MemoryStream memoryStream = new MemoryStream())
				{
					dataContractJsonSerializer.WriteObject(memoryStream, model);
					return Encoding.UTF8.GetString(memoryStream.ToArray());
				}
			}
			catch
			{
			}
			finally
			{
				result = CheckModel.Data2Json1(model);
			}
			return result;
		}

		private static string Data2Json1(object model)
		{
			string result = "err";
			try
			{
				JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
				return javaScriptSerializer.Serialize(model);
			}
			catch (Exception ex)
			{
				result = ex.ToString();
			}
			return result;
		}

		public static CheckValueResult GetValue(GetValueModel model, Assembly assembly, bool isFirst = false)
		{
			CheckValueResult checkValueResult = new CheckValueResult();
			checkValueResult.Info = "";
			try
			{
				object @object = RelationMapModel.GetObject(assembly, model.TypeName, model.SeachName, ref checkValueResult);
				if (!string.IsNullOrEmpty(checkValueResult.Info))
				{
					return checkValueResult;
				}
				CheckModel._getValueData(@object, model.SeachDataList, string.Format("{0}->[{1}]->", model.TypeName, model.SeachName), ref checkValueResult, true);
			}
			catch (Exception ex)
			{
				checkValueResult.Info = ex.ToString();
			}
			return checkValueResult;
		}

		private static bool _comparer(string d1, string d2, SeachValueType type)
		{
			bool result;
			if (SeachValueType.Less == type)
			{
				result = (Convert.ToDouble(d1) > Convert.ToDouble(d2));
			}
			else if (SeachValueType.Greater == type)
			{
				result = (Convert.ToDouble(d1) < Convert.ToDouble(d2));
			}
			else if (SeachValueType.Equal == type)
			{
				result = d1.Equals(d2);
			}
			else
			{
				result = (SeachValueType.NoEqual == type && !d1.Equals(d2));
			}
			return result;
		}

		private static bool _canAdd(object data, string[] files, ref CheckValueResult resultData)
		{
			bool result;
			if (null == data)
			{
				result = false;
			}
			else if (null == files)
			{
				result = true;
			}
			else
			{
				Type type = data.GetType();
				foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					foreach (string text in files)
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						string text2 = array[1];
						int num = Convert.ToInt32(array[2]);
						string text3 = array[0];
						if (string.IsNullOrEmpty(array[0]))
						{
							string d;
							if (typeof(short) == type || typeof(int) == type || typeof(long) == type || typeof(double) == type)
							{
								d = Convert.ToDouble(data).ToString();
							}
							else if (typeof(bool) == type)
							{
								d = Convert.ToInt32(data).ToString();
							}
							else
							{
								if (!(typeof(string) == type))
								{
									resultData.Info = string.Concat(new object[]
									{
										"筛选条件 不对",
										text3,
										text2,
										num
									});
									return false;
								}
								d = data.ToString();
							}
							return CheckModel._comparer(text2, d, (SeachValueType)num);
						}
						if (fieldInfo.Name.Equals(text3))
						{
							object obj = fieldInfo.GetValue(data);
							if (typeof(bool) == obj.GetType())
							{
								obj = Convert.ToInt32(obj);
							}
							if (CheckModel._comparer(text2, obj.ToString(), (SeachValueType)num))
							{
								return true;
							}
						}
					}
				}
				result = false;
			}
			return result;
		}

		private static void _checkList(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			FieldInfo field = objData.GetType().GetField("_items", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null == field)
			{
				resultData.Info = _strResultKey + " _items type.GetField = null";
			}
			else
			{
				object value = field.GetValue(objData);
				field = objData.GetType().GetField("_size", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (null == field)
				{
					resultData.Info = _strResultKey + " _size type.GetField = null";
				}
				else
				{
					int num = (int)field.GetValue(objData);
					if (num >= 1)
					{
						for (int i = 0; i < num; i++)
						{
							if (CheckModel.<_checkList>o__SiteContainer0.<>p__Site1 == null)
							{
								CheckModel.<_checkList>o__SiteContainer0.<>p__Site1 = CallSite<Func<CallSite, object, int, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetIndex(CSharpBinderFlags.None, typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
								}));
							}
							object obj = CheckModel.<_checkList>o__SiteContainer0.<>p__Site1.Target(CheckModel.<_checkList>o__SiteContainer0.<>p__Site1, value, i);
							if (CheckModel._canAdd(obj, files, ref resultData))
							{
								if (SeachList.Count == 0)
								{
									resultData.AddData(obj, _strResultKey);
								}
								else
								{
									CheckModel._getValueData(obj, SeachList, _strResultKey, ref resultData, false);
								}
							}
						}
					}
				}
			}
		}

		private static void _checkDict(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			FieldInfo field = objData.GetType().GetField("count", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null == field)
			{
				resultData.Info = _strResultKey + " count type.GetField = null";
			}
			else
			{
				int num = (int)field.GetValue(objData);
				if (num >= 1)
				{
					if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4 == null)
					{
						CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4 = CallSite<Func<CallSite, object, IEnumerable>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(IEnumerable), typeof(CheckModel)));
					}
					foreach (object obj in CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Site4, objData))
					{
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5 = CallSite<Func<CallSite, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof(string), typeof(CheckModel)));
						}
						Func<CallSite, object, string> target = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5.Target;
						CallSite <>p__Site = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site5;
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6 = CallSite<Func<CallSite, Type, string, string, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "Format", null, typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, Type, string, string, object, object> target2 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6.Target;
						CallSite <>p__Site2 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site6;
						Type typeFromHandle = typeof(string);
						string arg = "{0}[key={1}]";
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Key", typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						string text = target(<>p__Site, target2(<>p__Site2, typeFromHandle, arg, _strResultKey, CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Site7, obj)));
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8 == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
							}));
						}
						Func<CallSite, object, bool> target3 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8.Target;
						CallSite <>p__Site3 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site8;
						if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea == null)
						{
							CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea = CallSite<CheckModel.<_checkDict>o__SiteContainer3.<>q__SiteDelegate9>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "_canAdd", null, typeof(CheckModel), new CSharpArgumentInfo[]
							{
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
								CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsRef, null)
							}));
						}
						if (target3(<>p__Site3, CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitea, typeof(CheckModel), obj, files, ref resultData)))
						{
							if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb == null)
							{
								CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, bool> target4 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb.Target;
							CallSite <>p__Siteb = CheckModel.<_checkDict>o__SiteContainer3.<>p__Siteb;
							if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec == null)
							{
								CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, null),
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							Func<CallSite, object, object, object> target5 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec.Target;
							CallSite <>p__Sitec = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitec;
							object arg2 = null;
							if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited == null)
							{
								CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(CheckModel), new CSharpArgumentInfo[]
								{
									CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
								}));
							}
							if (!target4(<>p__Siteb, target5(<>p__Sitec, arg2, CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Sited, obj))))
							{
								if (SeachList.Count == 0)
								{
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee = CallSite<Action<CallSite, CheckValueResult, object, string>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "AddData", null, typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
										}));
									}
									Action<CallSite, CheckValueResult, object, string> target6 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee.Target;
									CallSite <>p__Sitee = CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitee;
									CheckValueResult arg3 = resultData;
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									target6(<>p__Sitee, arg3, CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Sitef, obj), text);
								}
								else
								{
									List<object> list = new List<object>();
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10 == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10 = CallSite<Action<CallSite, List<object>, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", null, typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									Action<CallSite, List<object>, object> target7 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10.Target;
									CallSite <>p__Site4 = CheckModel.<_checkDict>o__SiteContainer3.<>p__Site10;
									List<object> arg4 = list;
									if (CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11 == null)
									{
										CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "Value", typeof(CheckModel), new CSharpArgumentInfo[]
										{
											CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
										}));
									}
									target7(<>p__Site4, arg4, CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11.Target(CheckModel.<_checkDict>o__SiteContainer3.<>p__Site11, obj));
									List<object> objData2 = list;
									List<SeachData> list2 = CheckModel.CopySeachList(SeachList);
									list2.Insert(0, new SeachData());
									CheckModel._getValueData(objData2, list2, text, ref resultData, false);
								}
							}
						}
					}
				}
			}
		}

		private static void _checkEnumerable(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			foreach (object obj in (objData as IEnumerable<object>))
			{
				if (CheckModel._canAdd(obj, files, ref resultData))
				{
					if (SeachList.Count == 0)
					{
						resultData.AddData(obj, _strResultKey);
					}
					else
					{
						CheckModel._getValueData(obj, SeachList, _strResultKey, ref resultData, false);
					}
				}
			}
		}

		private static void _checkArray(object objData, ref CheckValueResult resultData, string[] files, string _strResultKey, List<SeachData> SeachList)
		{
			resultData.AddData(CheckModel.Data2Json(objData), _strResultKey);
		}

		private static void _getValueData(object objData, List<SeachData> SeachList, string _strResultKey, ref CheckValueResult resultData, bool isFirst = false)
		{
			if (null == objData)
			{
				resultData.AddData(null, _strResultKey);
			}
			else if (SeachList == null || SeachList.Count < 1)
			{
				resultData.AddData(objData, _strResultKey);
			}
			else
			{
				Type type = objData.GetType();
				string text = "";
				List<SeachData> list = CheckModel.CopySeachList(SeachList);
				SeachData seachData = list[0];
				list.RemoveAt(0);
				object obj;
				if (!string.IsNullOrEmpty(seachData.AttName))
				{
					FieldInfo field = type.GetField(seachData.AttName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (null == field)
					{
						resultData.Info = seachData.AttName + "type.GetField = null";
						return;
					}
					obj = field.GetValue(objData);
					if (null == obj)
					{
						resultData.AddData(null, _strResultKey);
						return;
					}
					Type type2 = obj.GetType();
				}
				else
				{
					obj = objData;
				}
				string[] files = null;
				if (!string.IsNullOrEmpty(seachData.SeachVal))
				{
					files = seachData.SeachVal.Split(new char[]
					{
						'|'
					});
				}
				if (CheckModel.IsList(obj))
				{
					if (!string.IsNullOrEmpty(seachData.AttName))
					{
						text = string.Format("{0}查找[{1}][list]->", _strResultKey, seachData.AttName);
					}
					else
					{
						text = _strResultKey + "[list]->";
					}
					if (!string.IsNullOrEmpty(seachData.SeachVal))
					{
						text = string.Format("{0}筛选[{1}]->", text, seachData.SeachVal);
					}
					CheckModel._checkList(obj, ref resultData, files, text, CheckModel.CopySeachList(list));
				}
				else if (CheckModel.IsDict(obj))
				{
					if (!string.IsNullOrEmpty(seachData.AttName))
					{
						text = string.Format("{0}查找[{1}][dict]->", _strResultKey, seachData.AttName);
					}
					else
					{
						text = _strResultKey + "[dict]->";
					}
					if (!string.IsNullOrEmpty(seachData.SeachVal))
					{
						text = string.Format("{0}筛选[{1}]->", text, seachData.SeachVal);
					}
					CheckModel._checkDict(obj, ref resultData, files, text, CheckModel.CopySeachList(list));
				}
				else if (CheckModel.IsIEnumerable(obj))
				{
					if (!string.IsNullOrEmpty(seachData.AttName))
					{
						text = string.Format("{0}查找[{1}][Enumerable]->", _strResultKey, seachData.AttName);
					}
					else
					{
						text = _strResultKey + "[Enumerable]->";
					}
					if (!string.IsNullOrEmpty(seachData.SeachVal))
					{
						text = string.Format("{0}筛选[{1}]->", text, seachData.SeachVal);
					}
					CheckModel._checkEnumerable(obj, ref resultData, files, text, CheckModel.CopySeachList(list));
				}
				else if (CheckModel.IsArray(obj))
				{
					if (!string.IsNullOrEmpty(seachData.AttName))
					{
						text = string.Format("{0}查找[{1}][Array]->", _strResultKey, seachData.AttName);
					}
					else
					{
						text = _strResultKey + "[Array]->";
					}
					if (!string.IsNullOrEmpty(seachData.SeachVal))
					{
						text = string.Format("{0}筛选[{1}]->", text, seachData.SeachVal);
					}
					CheckModel._checkArray(obj, ref resultData, files, text, CheckModel.CopySeachList(list));
				}
				else if (string.IsNullOrEmpty(seachData.AttName) && isFirst)
				{
					resultData.AddData(objData, text);
				}
				else if (CheckModel._canAdd(obj, files, ref resultData))
				{
					text = _strResultKey;
					if (!string.IsNullOrEmpty(seachData.AttName))
					{
						text = string.Format("{0}查找[{1}]->", _strResultKey, seachData.AttName);
					}
					if (!string.IsNullOrEmpty(seachData.SeachVal))
					{
						text = string.Format("{0}筛选[{1}]->", text, seachData.SeachVal);
					}
					if (list.Count == 0)
					{
						resultData.AddData(obj, text);
					}
					else
					{
						CheckModel._getValueData(obj, CheckModel.CopySeachList(list), text, ref resultData, false);
					}
				}
			}
		}

		[CompilerGenerated]
		private static class <_checkList>o__SiteContainer0
		{
			public static CallSite<Func<CallSite, object, int, object>> <>p__Site1;
		}

		[CompilerGenerated]
		private static class <_checkDict>o__SiteContainer3
		{
			public static CallSite<Func<CallSite, object, IEnumerable>> <>p__Site4;

			public static CallSite<Func<CallSite, object, string>> <>p__Site5;

			public static CallSite<Func<CallSite, Type, string, string, object, object>> <>p__Site6;

			public static CallSite<Func<CallSite, object, object>> <>p__Site7;

			public static CallSite<Func<CallSite, object, bool>> <>p__Site8;

			public static CallSite<CheckModel.<_checkDict>o__SiteContainer3.<>q__SiteDelegate9> <>p__Sitea;

			public static CallSite<Func<CallSite, object, bool>> <>p__Siteb;

			public static CallSite<Func<CallSite, object, object, object>> <>p__Sitec;

			public static CallSite<Func<CallSite, object, object>> <>p__Sited;

			public static CallSite<Action<CallSite, CheckValueResult, object, string>> <>p__Sitee;

			public static CallSite<Func<CallSite, object, object>> <>p__Sitef;

			public static CallSite<Action<CallSite, List<object>, object>> <>p__Site10;

			public static CallSite<Func<CallSite, object, object>> <>p__Site11;

			public delegate object <>q__SiteDelegate9(CallSite param0, Type param1, dynamic param2, string[] param3, ref CheckValueResult param4);
		}
	}
}
