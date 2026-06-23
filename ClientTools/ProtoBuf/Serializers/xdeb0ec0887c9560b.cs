using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xdeb0ec0887c9560b : x2b98a1e927f9de72
	{
		static xdeb0ec0887c9560b()
		{
			x8cd29e50453cbd62.x9f2c0dc847992f03 = false;
		}

		internal static bool xe0bc7b4a174cc0ae(WireType xa5694e1c82a939b4)
		{
			switch (xa5694e1c82a939b4)
			{
			case WireType.Variant:
			case WireType.Fixed64:
				break;
			default:
				while (xa5694e1c82a939b4 == WireType.Fixed32)
				{
					if (255 != 0)
					{
						return true;
					}
				}
				if (xa5694e1c82a939b4 != WireType.SignedVariant)
				{
					return false;
				}
				break;
			}
			return true;
		}

		private bool x2e7a2ea5da15ce85
		{
			get
			{
				return (this.xdfde339da46db651 & 1) != 0;
			}
		}

		private bool xa420f074d7e0f708
		{
			get
			{
				return (this.xdfde339da46db651 & 2) != 0;
			}
		}

		private bool x1b38645f6a8db043
		{
			get
			{
				return (this.xdfde339da46db651 & 4) != 0;
			}
		}

		private bool x2984e6683711bc23
		{
			get
			{
				return (this.xdfde339da46db651 & 32) != 0;
			}
		}

		private bool x12f7520ba66abc7a
		{
			get
			{
				return (this.xdfde339da46db651 & 8) != 0;
			}
		}

		public xdeb0ec0887c9560b(TypeModel model, Type declaredType, Type concreteType, x66ec8c25e4c7547d tail, int fieldNumber, bool writePacked, WireType packedWireType, bool returnList, bool overwriteList, bool supportNull) : base(tail)
		{
			for (;;)
			{
				IL_218:
				if (returnList)
				{
					this.xdfde339da46db651 |= 8;
				}
				while (overwriteList)
				{
					this.xdfde339da46db651 |= 16;
					bool flag2;
					bool flag = ((flag2 ? 1U : 0U) | 2147483648U) == 0U;
					if (!flag && (flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) <= 4294967295U)
					{
						IL_175:
						while (supportNull)
						{
							this.xdfde339da46db651 |= 32;
							flag = ((flag2 ? 1U : 0U) + (supportNull ? 1U : 0U) < 0U);
							if (!flag)
							{
								for (;;)
								{
									IL_162:
									if (writePacked)
									{
										goto IL_13D;
									}
									if (packedWireType != WireType.None)
									{
										goto IL_13D;
									}
									IL_142:
									if (xdeb0ec0887c9560b.xe0bc7b4a174cc0ae(packedWireType))
									{
										break;
									}
									if (writePacked)
									{
										goto IL_14D;
									}
									if (2 != 0)
									{
										goto Block_7;
									}
									continue;
									IL_13D:
									if (fieldNumber > 0)
									{
										goto IL_142;
									}
									goto IL_168;
								}
								IL_12D:
								this.xade3b695478596d6 = fieldNumber;
								if (false)
								{
									goto IL_4E;
								}
								if (writePacked)
								{
									this.xdfde339da46db651 |= 4;
								}
								this.xb9c62ffb3e96c0c8 = packedWireType;
								if (declaredType != null)
								{
									while (!declaredType.IsArray)
									{
										this.xf9d569c75ba50e9e = declaredType;
										if (2147483647 != 0)
										{
											this.x4460d386925de0c5 = concreteType;
											this.x502d59bacbd3e16e = TypeModel.xf6961b25e800f372(model, declaredType, tail.x00c54479c3a7c440, out flag2);
											if (flag2)
											{
												goto IL_4E;
											}
											if (((returnList ? 1U : 0U) & 0U) != 0U)
											{
												goto IL_31;
											}
											goto IL_0D;
										}
									}
									goto Block_5;
								}
								goto IL_B4;
								IL_0D:
								if (this.x502d59bacbd3e16e == null)
								{
									goto IL_31;
								}
								if (!true)
								{
									goto IL_218;
								}
								return;
								IL_4E:
								this.xdfde339da46db651 |= 1;
								if (declaredType.FullName.StartsWith("System.Data.Linq.EntitySet`1[["))
								{
									this.xdfde339da46db651 |= 2;
									goto IL_0D;
								}
								goto IL_0D;
								Block_7:
								packedWireType = WireType.None;
								goto IL_12D;
							}
						}
						goto IL_162;
					}
				}
				goto IL_175;
			}
			IL_31:
			throw new InvalidOperationException("Unable to resolve a suitable Add method for " + declaredType.FullName);
			IL_B4:
			throw new ArgumentNullException("declaredType");
			Block_5:
			throw new ArgumentException("Cannot treat arrays as lists", "declaredType");
			IL_14D:
			throw new InvalidOperationException("Only simple data-types can use packed encoding");
			IL_168:
			throw new ArgumentOutOfRangeException("fieldNumber");
		}

		public override Type x00c54479c3a7c440
		{
			get
			{
				return this.xf9d569c75ba50e9e;
			}
		}

		public override bool x95726b5912481139
		{
			get
			{
				return this.x4a8d88e6d727acb6;
			}
		}

		public override bool xf33087968b28143c
		{
			get
			{
				return this.x12f7520ba66abc7a;
			}
		}

		private bool x4a8d88e6d727acb6
		{
			get
			{
				return (this.xdfde339da46db651 & 16) == 0;
			}
		}

		private MethodInfo x38a9fbd8934b9364(TypeModel xad70a5849826ecef, out MethodInfo x64e645e4a13c5284, out MethodInfo x3bd62873fafa6252)
		{
			Type type = null;
			MethodInfo methodInfo;
			Type returnType;
			for (;;)
			{
				Type x00c54479c3a7c = this.x00c54479c3a7c440;
				methodInfo = x479f2661aae93792.x40c628a821a074f8(x00c54479c3a7c, "GetEnumerator", null);
				Type x00c54479c3a7c2 = this.x97cebb6cd312a01b.x00c54479c3a7c440;
				if (methodInfo != null)
				{
					goto IL_1A7;
				}
				IL_D3:
				Type type2 = xad70a5849826ecef.MapType(typeof(IEnumerable<>), false);
				if (type2 != null)
				{
					type2 = type2.MakeGenericType(new Type[]
					{
						x00c54479c3a7c2
					});
					type = type2;
				}
				if (type == null)
				{
					break;
				}
				if (!type.IsAssignableFrom(x00c54479c3a7c))
				{
					break;
				}
				methodInfo = x479f2661aae93792.x40c628a821a074f8(type, "GetEnumerator");
				if (false)
				{
					goto IL_1A5;
				}
				returnType = methodInfo.ReturnType;
				x64e645e4a13c5284 = x479f2661aae93792.x40c628a821a074f8(xad70a5849826ecef.MapType(xdeb0ec0887c9560b.xfb8faf6291b61c37), "MoveNext");
				if (false)
				{
					goto IL_1A5;
				}
				if (4 != 0)
				{
					if (false)
					{
						continue;
					}
					goto IL_216;
				}
				IL_11D:
				MethodInfo methodInfo2;
				x3bd62873fafa6252 = (methodInfo2 = (methodInfo = null));
				x64e645e4a13c5284 = methodInfo2;
				if (-1 != 0)
				{
					goto IL_D3;
				}
				goto IL_1A7;
				IL_110:
				goto IL_11D;
				IL_15C:
				if (x64e645e4a13c5284 == null)
				{
					goto IL_11D;
				}
				if (4 == 0)
				{
					if (!false)
					{
						goto IL_153;
					}
					if (false)
					{
						break;
					}
				}
				else
				{
					if (x64e645e4a13c5284.ReturnType != xad70a5849826ecef.MapType(typeof(bool)))
					{
						goto IL_11D;
					}
					goto IL_153;
				}
				IL_112:
				if (x3bd62873fafa6252.ReturnType != x00c54479c3a7c2)
				{
					goto IL_11D;
				}
				return methodInfo;
				IL_153:
				if (x3bd62873fafa6252 == null)
				{
					goto IL_110;
				}
				goto IL_112;
				IL_1A5:
				goto IL_15C;
				IL_1A7:
				returnType = methodInfo.ReturnType;
				x64e645e4a13c5284 = x479f2661aae93792.x40c628a821a074f8(returnType, "MoveNext", null);
				PropertyInfo propertyInfo = x479f2661aae93792.x4ff37084a5d7d57f(returnType, "Current");
				x3bd62873fafa6252 = ((propertyInfo == null) ? null : x479f2661aae93792.xbfdf1fd45a6330fd(propertyInfo, false));
				if (false)
				{
					if (!false)
					{
						goto IL_176;
					}
				}
				else
				{
					if (x64e645e4a13c5284 == null)
					{
						goto IL_176;
					}
					goto IL_1A5;
				}
				IL_18B:
				x64e645e4a13c5284 = x479f2661aae93792.x40c628a821a074f8(xad70a5849826ecef.MapType(xdeb0ec0887c9560b.xfb8faf6291b61c37), "MoveNext", null);
				goto IL_15C;
				IL_176:
				if (!xad70a5849826ecef.MapType(xdeb0ec0887c9560b.xfb8faf6291b61c37).IsAssignableFrom(returnType))
				{
					goto IL_15C;
				}
				goto IL_18B;
			}
			IL_47:
			type = xad70a5849826ecef.MapType(xdeb0ec0887c9560b.xf62a09b3f0408316);
			if (true)
			{
				methodInfo = x479f2661aae93792.x40c628a821a074f8(type, "GetEnumerator");
				returnType = methodInfo.ReturnType;
				x64e645e4a13c5284 = x479f2661aae93792.x40c628a821a074f8(returnType, "MoveNext");
				if (255 != 0)
				{
					x3bd62873fafa6252 = x479f2661aae93792.xbfdf1fd45a6330fd(x479f2661aae93792.x4ff37084a5d7d57f(returnType, "Current"), false);
				}
				return methodInfo;
			}
			IL_5A:
			x3bd62873fafa6252 = x479f2661aae93792.xbfdf1fd45a6330fd(x479f2661aae93792.x4ff37084a5d7d57f(returnType, "Current"), false);
			return methodInfo;
			goto IL_47;
			IL_216:
			goto IL_5A;
		}

		public override void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			bool x1b38645f6a8db = this.x1b38645f6a8db043;
			for (;;)
			{
				bool flag;
				SubItemToken token;
				if (x1b38645f6a8db)
				{
					do
					{
						ProtoWriter.WriteFieldHeader(this.xade3b695478596d6, WireType.String, x6b8e154b42d5c1e3);
					}
					while ((flag ? 1U : 0U) + (x1b38645f6a8db ? 1U : 0U) < 0U);
					token = ProtoWriter.StartSubItem(xbcea506a33cf9111, x6b8e154b42d5c1e3);
					goto IL_10B;
				}
				token = default(SubItemToken);
				bool flag2 = (flag ? 1U : 0U) > uint.MaxValue;
				if (flag2)
				{
					continue;
				}
				if (((flag ? 1U : 0U) & 0U) == 0U)
				{
					goto IL_54;
				}
				IL_0C:
				ProtoWriter.EndSubItem(token, x6b8e154b42d5c1e3);
				if (2147483647 != 0)
				{
					flag2 = ((flag ? 1U : 0U) > uint.MaxValue);
					if (!flag2)
					{
						break;
					}
					flag2 = ((x1b38645f6a8db ? 1U : 0U) - (x1b38645f6a8db ? 1U : 0U) < 0U);
					if (flag2)
					{
						goto IL_54;
					}
					continue;
				}
				IL_15:
				if (!x1b38645f6a8db)
				{
					break;
				}
				goto IL_0C;
				IL_54:
				flag = !this.x2984e6683711bc23;
				IEnumerator enumerator = ((IEnumerable)xbcea506a33cf9111).GetEnumerator();
				try
				{
					for (;;)
					{
						if (!enumerator.MoveNext())
						{
							flag2 = (((x1b38645f6a8db ? 1U : 0U) | 2147483648U) == 0U);
							if (flag2)
							{
								break;
							}
							if (!false)
							{
								break;
							}
						}
						object obj = enumerator.Current;
						if (flag && obj == null)
						{
							goto Block_10;
						}
						this.x97cebb6cd312a01b.x6210059f049f0d48(obj, x6b8e154b42d5c1e3);
					}
					goto IL_B4;
					Block_10:
					throw new NullReferenceException();
					IL_B4:
					goto IL_15;
				}
				finally
				{
					IDisposable disposable = enumerator as IDisposable;
					if (((flag ? 1U : 0U) & 0U) != 0U || disposable != null)
					{
						disposable.Dispose();
					}
				}
				IL_10B:
				ProtoWriter.SetPackedField(this.xade3b695478596d6, x6b8e154b42d5c1e3);
				goto IL_54;
			}
		}

		public override object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			int fieldNumber = x337e217cb3ba0627.FieldNumber;
			bool flag2;
			bool flag = (uint)fieldNumber - (flag2 ? 1U : 0U) < 0U;
			if (!flag)
			{
				goto IL_2F7;
			}
			IL_69:
			while (!flag2)
			{
				if (!false)
				{
					goto IL_72;
				}
				if (((uint)fieldNumber & 0U) != 0U)
				{
					if (((flag2 ? 1U : 0U) & 0U) != 0U)
					{
						goto IL_AD;
					}
					if (false)
					{
						goto IL_314;
					}
				}
				else if (4 != 0)
				{
					goto IL_7B;
				}
			}
			goto IL_111;
			IL_27:
			object obj;
			if (obj == xbcea506a33cf9111)
			{
				goto IL_340;
			}
			return xbcea506a33cf9111;
			IL_72:
			IL_7B:
			object[] array = new object[1];
			IL_AD:
			if (3 == 0)
			{
				goto IL_161;
			}
			do
			{
				array[0] = this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627);
				if (false)
				{
					goto IL_1C7;
				}
				this.x502d59bacbd3e16e.Invoke(xbcea506a33cf9111, array);
			}
			while (x337e217cb3ba0627.TryReadFieldHeader(fieldNumber));
			goto IL_27;
			IL_1C7:
			goto IL_19B;
			IL_111:
			IList list = (IList)xbcea506a33cf9111;
			IL_146:
			flag = (((uint)fieldNumber | uint.MaxValue) == 0U);
			if (!flag)
			{
				do
				{
					list.Add(this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627));
				}
				while (x337e217cb3ba0627.TryReadFieldHeader(fieldNumber));
				goto IL_27;
			}
			IL_161:
			goto IL_340;
			IL_18F:
			SubItemToken token;
			ProtoReader.EndSubItem(token, x337e217cb3ba0627);
			goto IL_27;
			IL_19B:
			object[] array2 = new object[1];
			while (ProtoReader.HasSubValue(this.xb9c62ffb3e96c0c8, x337e217cb3ba0627))
			{
				array2[0] = this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627);
				this.x502d59bacbd3e16e.Invoke(xbcea506a33cf9111, array2);
			}
			if (((uint)fieldNumber | 4U) != 0U)
			{
				goto IL_18F;
			}
			goto IL_111;
			IL_2F7:
			if ((flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) < 0U)
			{
				goto IL_72;
			}
			obj = xbcea506a33cf9111;
			IL_314:
			flag = ((flag2 ? 1U : 0U) < 0U);
			if (flag)
			{
				goto IL_329;
			}
			goto IL_292;
			IL_265:
			if (this.x2e7a2ea5da15ce85)
			{
				goto IL_2B9;
			}
			IL_281:
			bool flag3 = false;
			IL_282:
			flag2 = flag3;
			flag = ((uint)fieldNumber < 0U);
			if (flag)
			{
				flag = ((uint)fieldNumber - (uint)fieldNumber < 0U);
				if (flag)
				{
					goto IL_221;
				}
			}
			else
			{
				if (this.xb9c62ffb3e96c0c8 == WireType.None)
				{
					goto IL_69;
				}
				flag = ((flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) < 0U);
				if (flag)
				{
					goto IL_146;
				}
			}
			if (x337e217cb3ba0627.WireType == WireType.String)
			{
				goto IL_221;
			}
			if (((flag2 ? 1U : 0U) | 1U) != 0U)
			{
				goto IL_69;
			}
			IL_1C9:
			if (!flag2)
			{
				goto IL_19B;
			}
			IL_1FD:
			IList list2 = (IList)xbcea506a33cf9111;
			while (ProtoReader.HasSubValue(this.xb9c62ffb3e96c0c8, x337e217cb3ba0627))
			{
				list2.Add(this.x97cebb6cd312a01b.x06b0e25aa6ad68a9(null, x337e217cb3ba0627));
				if (false)
				{
					goto IL_292;
				}
				flag = (((uint)fieldNumber | 255U) == 0U);
				if (flag)
				{
					IL_2DF:
					flag = ((uint)fieldNumber + (flag2 ? 1U : 0U) < 0U);
					if (flag)
					{
						goto IL_2F7;
					}
					goto IL_18F;
				}
			}
			goto IL_2DF;
			IL_221:
			token = ProtoReader.StartSubItem(x337e217cb3ba0627);
			if (2147483647 == 0)
			{
				if ((flag2 ? 1U : 0U) >= 0U)
				{
					goto IL_281;
				}
				goto IL_265;
			}
			else
			{
				flag = ((uint)fieldNumber - (flag2 ? 1U : 0U) < 0U);
				if (flag)
				{
					goto IL_1FD;
				}
				goto IL_1C9;
			}
			IL_292:
			if (xbcea506a33cf9111 != null)
			{
				goto IL_265;
			}
			goto IL_329;
			IL_2B9:
			flag3 = !this.xa420f074d7e0f708;
			goto IL_282;
			IL_329:
			xbcea506a33cf9111 = Activator.CreateInstance(this.x4460d386925de0c5);
			if ((uint)fieldNumber - (uint)fieldNumber < 0U)
			{
				goto IL_292;
			}
			if (4 == 0)
			{
				goto IL_2B9;
			}
			goto IL_265;
			IL_340:
			return null;
		}

		private const byte xcc3b2e08f4b202ae = 1;

		private const byte x091e991f185fae06 = 2;

		private const byte x0cb185874a68ca46 = 4;

		private const byte x5d4c313083f5419a = 8;

		private const byte x007cff453cb0ea17 = 16;

		private const byte x75e63ef75c60cbc1 = 32;

		private readonly byte xdfde339da46db651;

		private readonly Type xf9d569c75ba50e9e;

		private readonly Type x4460d386925de0c5;

		private readonly MethodInfo x502d59bacbd3e16e;

		private readonly int xade3b695478596d6;

		private readonly WireType xb9c62ffb3e96c0c8;

		private static readonly Type xfb8faf6291b61c37 = typeof(IEnumerator);

		private static readonly Type xf62a09b3f0408316 = typeof(IEnumerable);
	}
}
