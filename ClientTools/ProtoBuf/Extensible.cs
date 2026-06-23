using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	public abstract class Extensible : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return this.GetExtensionObject(createIfMissing);
		}

		protected virtual IExtension GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.x8123467383ef8c98, createIfMissing);
		}

		public static IExtension GetExtensionObject(ref IExtension extensionObject, bool createIfMissing)
		{
			if (createIfMissing)
			{
				bool flag = (createIfMissing ? 1U : 0U) - (createIfMissing ? 1U : 0U) < 0U;
				if (flag || extensionObject == null)
				{
					extensionObject = new BufferExtension();
				}
			}
			return extensionObject;
		}

		public static void AppendValue<TValue>(IExtensible instance, int tag, TValue value)
		{
			Extensible.AppendValue<TValue>(instance, tag, DataFormat.Default, value);
		}

		public static void AppendValue<TValue>(IExtensible instance, int tag, DataFormat format, TValue value)
		{
			x5397b8badbdbce60.x1e83ff1d25ed5b48(RuntimeTypeModel.Default, instance, tag, format, value);
		}

		public static TValue GetValue<TValue>(IExtensible instance, int tag)
		{
			return Extensible.GetValue<TValue>(instance, tag, DataFormat.Default);
		}

		public static TValue GetValue<TValue>(IExtensible instance, int tag, DataFormat format)
		{
			TValue result;
			Extensible.TryGetValue<TValue>(instance, tag, format, out result);
			return result;
		}

		public static bool TryGetValue<TValue>(IExtensible instance, int tag, out TValue value)
		{
			return Extensible.TryGetValue<TValue>(instance, tag, DataFormat.Default, out value);
		}

		public static bool TryGetValue<TValue>(IExtensible instance, int tag, DataFormat format, out TValue value)
		{
			return Extensible.TryGetValue<TValue>(instance, tag, format, false, out value);
		}

		public static bool TryGetValue<TValue>(IExtensible instance, int tag, DataFormat format, bool allowDefinedTag, out TValue value)
		{
			value = default(TValue);
			bool result = false;
			foreach (TValue tvalue in x5397b8badbdbce60.x31e559cfa050dec9<TValue>(instance, tag, format, true, allowDefinedTag))
			{
				value = tvalue;
				result = true;
			}
			return result;
		}

		public static IEnumerable<TValue> GetValues<TValue>(IExtensible instance, int tag)
		{
			return x5397b8badbdbce60.x31e559cfa050dec9<TValue>(instance, tag, DataFormat.Default, false, false);
		}

		public static IEnumerable<TValue> GetValues<TValue>(IExtensible instance, int tag, DataFormat format)
		{
			return x5397b8badbdbce60.x31e559cfa050dec9<TValue>(instance, tag, format, false, false);
		}

		public static bool TryGetValue(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format, bool allowDefinedTag, out object value)
		{
			value = null;
			bool result = false;
			foreach (object obj in x5397b8badbdbce60.x31e559cfa050dec9(model, type, instance, tag, format, true, allowDefinedTag))
			{
				value = obj;
				result = true;
			}
			return result;
		}

		public static IEnumerable GetValues(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format)
		{
			return x5397b8badbdbce60.x31e559cfa050dec9(model, type, instance, tag, format, false, false);
		}

		public static void AppendValue(TypeModel model, IExtensible instance, int tag, DataFormat format, object value)
		{
			x5397b8badbdbce60.x1e83ff1d25ed5b48(model, instance, tag, format, value);
		}

		private IExtension x8123467383ef8c98;
	}
}
