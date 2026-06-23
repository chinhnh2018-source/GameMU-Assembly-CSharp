using System;
using System.ComponentModel;
using System.Reflection;

public static class EnumUtils
{
	public static string GetDescription(this Enum value, bool nameInstend = true)
	{
		Type type = value.GetType();
		string name = Enum.GetName(type, value);
		if (name == null)
		{
			return string.Empty;
		}
		FieldInfo field = type.GetField(name);
		DescriptionAttribute descriptionAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
		if (descriptionAttribute == null && nameInstend)
		{
			return name;
		}
		return (descriptionAttribute != null) ? descriptionAttribute.Description : string.Empty;
	}
}
