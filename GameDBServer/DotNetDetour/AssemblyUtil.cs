using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetDetour
{
	public static class AssemblyUtil
	{
		public static T CreateInstance<T>(string type)
		{
			return AssemblyUtil.CreateInstance<T>(type, new object[0]);
		}

		public static T CreateInstance<T>(string type, object[] parameters)
		{
			T t = default(T);
			Type type2 = Type.GetType(type, false, true);
			T result;
			if (type2 == null)
			{
				result = default(T);
			}
			else
			{
				object obj = Activator.CreateInstance(type2, parameters);
				t = (T)((object)obj);
				result = t;
			}
			return result;
		}

		public static T CreateInstance<T>(string assembleName, string type)
		{
			Type type2 = null;
			T t = default(T);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (string.Equals(assembly.FullName, assembleName, StringComparison.CurrentCultureIgnoreCase))
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type3 in types)
					{
						if (string.Equals(type3.ToString(), type, StringComparison.CurrentCultureIgnoreCase))
						{
							type2 = type3;
							break;
						}
					}
					break;
				}
			}
			T result;
			if (type2 == null)
			{
				result = default(T);
			}
			else
			{
				object obj = Activator.CreateInstance(type2, new object[0]);
				t = (T)((object)obj);
				result = t;
			}
			return result;
		}

		public static T CreateInstance<T>(string assembleName, string type, object[] parameters)
		{
			Type type2 = null;
			T t = default(T);
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (string.Equals(assembly.FullName, assembleName, StringComparison.CurrentCultureIgnoreCase))
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type3 in types)
					{
						if (string.Equals(type3.ToString(), type, StringComparison.CurrentCultureIgnoreCase))
						{
							type2 = type3;
							break;
						}
					}
					break;
				}
			}
			T result;
			if (type2 == null)
			{
				result = default(T);
			}
			else
			{
				object obj = Activator.CreateInstance(type2, parameters);
				t = (T)((object)obj);
				result = t;
			}
			return result;
		}

		public static IEnumerable<Type> GetImplementTypes<TBaseType>(this Assembly assembly)
		{
			return from t in assembly.GetExportedTypes()
			where t.IsSubclassOf(typeof(TBaseType)) && t.IsClass && !t.IsAbstract
			select t;
		}

		public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly) where TBaseInterface : class
		{
			return assembly.GetImplementedObjectsByInterface(typeof(TBaseInterface));
		}

		public static IEnumerable<TBaseInterface> GetImplementedObjectsByInterface<TBaseInterface>(this Assembly assembly, Type targetType) where TBaseInterface : class
		{
			Type[] exportedTypes = assembly.GetExportedTypes();
			List<TBaseInterface> list = new List<TBaseInterface>();
			foreach (Type type in exportedTypes)
			{
				if (!type.IsAbstract)
				{
					if (targetType.IsAssignableFrom(type))
					{
						list.Add((TBaseInterface)((object)Activator.CreateInstance(type)));
					}
				}
			}
			return list;
		}

		public static T BinaryClone<T>(this T target)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			T result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				binaryFormatter.Serialize(memoryStream, target);
				memoryStream.Position = 0L;
				result = (T)((object)binaryFormatter.Deserialize(memoryStream));
			}
			return result;
		}

		public static T CopyPropertiesTo<T>(this T source, T target)
		{
			PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
			Dictionary<string, PropertyInfo> dictionary = properties.ToDictionary((PropertyInfo p) => p.Name);
			PropertyInfo[] properties2 = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
			int i = 0;
			while (i < properties2.Length)
			{
				PropertyInfo propertyInfo = properties2[i];
				PropertyInfo propertyInfo2;
				if (dictionary.TryGetValue(propertyInfo.Name, out propertyInfo2))
				{
					if (!(propertyInfo2.PropertyType != propertyInfo.PropertyType))
					{
						if (propertyInfo2.PropertyType.IsSerializable)
						{
							propertyInfo.SetValue(target, propertyInfo2.GetValue(source, AssemblyUtil.m_EmptyObjectArray), AssemblyUtil.m_EmptyObjectArray);
						}
					}
				}
				IL_C0:
				i++;
				continue;
				goto IL_C0;
			}
			return target;
		}

		public static IEnumerable<Assembly> GetAssembliesFromString(string assemblyDef)
		{
			return AssemblyUtil.GetAssembliesFromStrings(assemblyDef.Split(new char[]
			{
				',',
				';'
			}, StringSplitOptions.RemoveEmptyEntries));
		}

		public static IEnumerable<Assembly> GetAssembliesFromStrings(string[] assemblies)
		{
			List<Assembly> list = new List<Assembly>(assemblies.Length);
			foreach (string assemblyString in assemblies)
			{
				list.Add(Assembly.Load(assemblyString));
			}
			return list;
		}

		public static string GetAssembleVer(string filePath)
		{
			FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(filePath);
			return string.Format(" {0}.{1}.{2}.{3}", new object[]
			{
				versionInfo.ProductMajorPart,
				versionInfo.ProductMinorPart,
				versionInfo.ProductBuildPart,
				versionInfo.ProductPrivatePart
			});
		}

		private static object[] m_EmptyObjectArray = new object[0];
	}
}
