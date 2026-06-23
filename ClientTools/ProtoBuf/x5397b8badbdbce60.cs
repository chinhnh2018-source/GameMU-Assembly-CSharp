using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf.Meta;

namespace ProtoBuf
{
	internal static class x5397b8badbdbce60
	{
		internal static IEnumerable<TValue> x31e559cfa050dec9<TValue>(IExtensible x6ed4ed9ed59eb694, int xffe521cc76054baf, DataFormat x5786461d089b10a0, bool xadcd70907fa33ee6, bool x5b299e2216f95995)
		{
			x5397b8badbdbce60.<GetExtendedValues>d__0<TValue> <GetExtendedValues>d__ = new x5397b8badbdbce60.<GetExtendedValues>d__0<TValue>(-2);
			if ((x5b299e2216f95995 ? 1U : 0U) - (xadcd70907fa33ee6 ? 1U : 0U) >= 0U)
			{
			}
			<GetExtendedValues>d__.<>3__instance = x6ed4ed9ed59eb694;
			<GetExtendedValues>d__.<>3__tag = xffe521cc76054baf;
			<GetExtendedValues>d__.<>3__format = x5786461d089b10a0;
			<GetExtendedValues>d__.<>3__singleton = xadcd70907fa33ee6;
			<GetExtendedValues>d__.<>3__allowDefinedTag = x5b299e2216f95995;
			return <GetExtendedValues>d__;
		}

		internal static IEnumerable x31e559cfa050dec9(TypeModel xad70a5849826ecef, Type x43163d22e8cd5a71, IExtensible x6ed4ed9ed59eb694, int xffe521cc76054baf, DataFormat x5786461d089b10a0, bool xadcd70907fa33ee6, bool x5b299e2216f95995)
		{
			x5397b8badbdbce60.<GetExtendedValues>d__7 <GetExtendedValues>d__ = new x5397b8badbdbce60.<GetExtendedValues>d__7(-2);
			<GetExtendedValues>d__.<>3__model = xad70a5849826ecef;
			<GetExtendedValues>d__.<>3__type = x43163d22e8cd5a71;
			<GetExtendedValues>d__.<>3__instance = x6ed4ed9ed59eb694;
			<GetExtendedValues>d__.<>3__tag = xffe521cc76054baf;
			bool flag = (x5b299e2216f95995 ? 1U : 0U) < 0U;
			if (!flag && ((x5b299e2216f95995 ? 1U : 0U) | 2147483647U) != 0U)
			{
				<GetExtendedValues>d__.<>3__format = x5786461d089b10a0;
				<GetExtendedValues>d__.<>3__singleton = xadcd70907fa33ee6;
			}
			return <GetExtendedValues>d__;
		}

		internal static void x1e83ff1d25ed5b48(TypeModel xad70a5849826ecef, IExtensible x6ed4ed9ed59eb694, int xffe521cc76054baf, DataFormat x5786461d089b10a0, object xbcea506a33cf9111)
		{
			if (x6ed4ed9ed59eb694 != null)
			{
				IExtension extensionObject;
				if (xbcea506a33cf9111 == null)
				{
					if (!false)
					{
						throw new ArgumentNullException("value");
					}
				}
				else
				{
					extensionObject = x6ed4ed9ed59eb694.GetExtensionObject(true);
				}
				if (extensionObject == null)
				{
					throw new InvalidOperationException("No extension object available; appended data would be lost.");
				}
				bool commit = false;
				Stream stream = extensionObject.BeginAppend();
				try
				{
					using (ProtoWriter protoWriter = new ProtoWriter(stream, xad70a5849826ecef, null))
					{
						xad70a5849826ecef.x07feef0c759efbcc(protoWriter, null, x5786461d089b10a0, xffe521cc76054baf, xbcea506a33cf9111, false);
						protoWriter.Close();
					}
					commit = true;
					return;
				}
				finally
				{
					extensionObject.EndAppend(stream, commit);
				}
			}
			throw new ArgumentNullException("instance");
		}

		public static void x8c9e88d48fba4ed7<TSource, TValue>(TypeModel xad70a5849826ecef, TSource x6ed4ed9ed59eb694, int xffe521cc76054baf, DataFormat x5786461d089b10a0, TValue xbcea506a33cf9111) where TSource : class, IExtensible
		{
			x5397b8badbdbce60.x1e83ff1d25ed5b48(xad70a5849826ecef, x6ed4ed9ed59eb694, xffe521cc76054baf, x5786461d089b10a0, xbcea506a33cf9111);
		}
	}
}
