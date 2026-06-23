using System;
using System.Reflection;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers
{
	internal sealed class xe558091e7a042d4f : x66ec8c25e4c7547d, x9da713b4847e9e6e
	{
		public bool xf5cdd81490e1c274(TypeModel.CallbackType xee5efdfe033701c1)
		{
			if (this.xbc2aaa430ff675df != null)
			{
				goto IL_91;
			}
			IL_67:
			int i;
			bool flag;
			for (i = 0; i < this.x55531fe975acd4f6.Length; i++)
			{
				if (this.x55531fe975acd4f6[i].x00c54479c3a7c440 == this.x34eba590a5ec9a74)
				{
					flag = (((uint)i | 1U) == 0U);
					if (flag)
					{
						return false;
					}
				}
				else if (((x9da713b4847e9e6e)this.x55531fe975acd4f6[i]).xf5cdd81490e1c274(xee5efdfe033701c1))
				{
					return true;
				}
			}
			flag = ((uint)i + (uint)i > uint.MaxValue);
			if (!flag)
			{
				return false;
			}
			IL_91:
			if (this.xbc2aaa430ff675df.get_xe6d4b1b411ed94b5(xee5efdfe033701c1) != null)
			{
				return true;
			}
			flag = (((uint)i & 0U) == 0U);
			if (flag)
			{
				goto IL_67;
			}
			return false;
		}

		public Type x00c54479c3a7c440
		{
			get
			{
				return this.x34eba590a5ec9a74;
			}
		}

		public xe558091e7a042d4f(TypeModel model, Type forType, int[] fieldNumbers, x66ec8c25e4c7547d[] serializers, MethodInfo[] baseCtorCallbacks, bool isRootType, bool useConstructor, CallbackSet callbacks, Type constructType, MethodInfo factory)
		{
			int num;
			for (;;)
			{
				IL_3B8:
				x479f2661aae93792.xee9aac96ed24c7f9(fieldNumbers, serializers);
				for (;;)
				{
					bool flag = false;
					num = 1;
					for (;;)
					{
						for (;;)
						{
							IL_2AB:
							bool flag2;
							if (num >= fieldNumbers.Length)
							{
								this.x34eba590a5ec9a74 = forType;
								this.x64b16fcabdb0518e = factory;
								flag2 = ((uint)num > uint.MaxValue);
								if (flag2)
								{
									goto IL_34C;
								}
								if ((flag ? 1U : 0U) - (useConstructor ? 1U : 0U) > 4294967295U)
								{
									goto IL_8E;
								}
								if (constructType != null)
								{
									goto IL_253;
								}
								constructType = forType;
								IL_229:
								this.x718bc396727ef5e4 = constructType;
								this.x55531fe975acd4f6 = serializers;
								this.xed57745ced0c37a4 = fieldNumbers;
								this.xbc2aaa430ff675df = callbacks;
								this.x9f191e0c449fb8d1 = isRootType;
								this.xdcf89e90b059245d = useConstructor;
								if ((isRootType ? 1U : 0U) - (flag ? 1U : 0U) > 4294967295U)
								{
									goto IL_10A;
								}
								if (baseCtorCallbacks != null)
								{
									if (baseCtorCallbacks.Length != 0)
									{
										flag2 = ((flag ? 1U : 0U) + (uint)num > uint.MaxValue);
										if (flag2)
										{
											goto IL_253;
										}
									}
									else
									{
										baseCtorCallbacks = null;
									}
								}
								this.xb7037fcd8ee99709 = baseCtorCallbacks;
								if (false || x479f2661aae93792.xe5e08d1dc9f521de(forType) != null)
								{
									goto IL_1C2;
								}
								if (!model.MapType(xe558091e7a042d4f.x5f4400404e3d6700).IsAssignableFrom(forType))
								{
									goto IL_C5;
								}
								if (forType.IsValueType)
								{
									goto IL_163;
								}
								flag2 = ((uint)num + (useConstructor ? 1U : 0U) < 0U);
								if (flag2)
								{
									goto IL_1AB;
								}
								goto IL_115;
								IL_253:
								if (forType.IsAssignableFrom(constructType))
								{
									goto IL_229;
								}
								goto IL_266;
							}
							IL_37F:
							while (fieldNumbers[num] != fieldNumbers[num - 1])
							{
								for (;;)
								{
									if (!flag)
									{
										goto IL_31F;
									}
									flag2 = (((isRootType ? 1U : 0U) & 0U) == 0U);
									if (!flag2)
									{
										goto IL_160;
									}
									if (8 == 0)
									{
										goto IL_31F;
									}
									flag2 = (((flag ? 1U : 0U) & 0U) == 0U);
									if (!flag2)
									{
										break;
									}
									IL_312:
									num++;
									if (-2 == 0)
									{
										continue;
									}
									goto IL_331;
									IL_31F:
									if (serializers[num].x00c54479c3a7c440 == forType)
									{
										goto IL_312;
									}
									flag = true;
									flag2 = ((useConstructor ? 1U : 0U) > uint.MaxValue);
									if (flag2)
									{
										goto Block_27;
									}
									goto IL_312;
								}
								continue;
								IL_331:
								flag2 = (((useConstructor ? 1U : 0U) | 255U) == 0U);
								if (flag2)
								{
									goto IL_34C;
								}
								goto IL_2AB;
							}
							goto IL_353;
							IL_12A:
							if ((uint)num + (flag ? 1U : 0U) < 0U)
							{
								goto IL_37F;
							}
							if ((flag ? 1U : 0U) - (isRootType ? 1U : 0U) >= 0U)
							{
								goto IL_160;
							}
							break;
							IL_34C:
							flag2 = (((isRootType ? 1U : 0U) | uint.MaxValue) == 0U);
							if (flag2)
							{
								goto IL_3EB;
							}
							goto IL_12A;
							IL_10A:
							if (!useConstructor)
							{
								return;
							}
							if (false)
							{
								goto IL_3EB;
							}
							while (((useConstructor ? 1U : 0U) & 0U) == 0U && this.x1f170e14e19c30aa)
							{
								if (((uint)num & 0U) == 0U)
								{
									return;
								}
								flag2 = ((isRootType ? 1U : 0U) < 0U);
								if (!flag2)
								{
									goto IL_3B8;
								}
								flag2 = ((flag ? 1U : 0U) - (flag ? 1U : 0U) > uint.MaxValue);
								if (!flag2)
								{
									goto IL_12A;
								}
							}
							goto IL_8E;
							IL_C5:
							this.x1f170e14e19c30aa = (!constructType.IsAbstract && x479f2661aae93792.xdaa1a96eb962bff0(constructType, x479f2661aae93792.xf6f6ea67665595b8, true) != null);
							if (constructType == forType)
							{
								return;
							}
							if ((useConstructor ? 1U : 0U) - (useConstructor ? 1U : 0U) <= 4294967295U)
							{
								goto IL_10A;
							}
							IL_126:
							goto IL_C5;
							IL_119:
							if (!flag)
							{
								this.x5e0d6f22b8494d3d = true;
								goto IL_126;
							}
							goto IL_1B0;
							IL_115:
							if (isRootType)
							{
								goto IL_119;
							}
							goto IL_163;
							IL_160:
							if (false)
							{
								goto IL_163;
							}
							goto IL_115;
							IL_1AB:
							goto IL_119;
							IL_3EB:
							if ((flag ? 1U : 0U) + (isRootType ? 1U : 0U) >= 0U)
							{
								goto Block_1;
							}
							goto IL_1AB;
						}
					}
					Block_27:
					if ((flag ? 1U : 0U) >= 0U)
					{
						goto Block_28;
					}
				}
			}
			Block_1:
			return;
			IL_8E:
			throw new ArgumentException("The supplied default implementation cannot be created: " + constructType.FullName, "constructType");
			IL_163:
			throw new NotSupportedException("IExtensible is not supported in structs or classes with inheritance");
			IL_1B0:
			goto IL_163;
			IL_1C2:
			throw new ArgumentException("Cannot create a TypeSerializer for nullable types", "forType");
			IL_266:
			throw new InvalidOperationException(forType.FullName + " cannot be assigned from " + constructType.FullName);
			IL_353:
			throw new InvalidOperationException("Duplicate field-number detected; " + fieldNumbers[num].ToString() + " on: " + forType.FullName);
			Block_28:
			goto IL_353;
		}

		private bool x8ab1b6f2b7722712
		{
			get
			{
				return (this.x34eba590a5ec9a74.IsClass || this.x34eba590a5ec9a74.IsInterface) && !this.x34eba590a5ec9a74.IsSealed;
			}
		}

		public void x4e4178637618473f(object xbcea506a33cf9111, TypeModel.CallbackType xee5efdfe033701c1, SerializationContext x0f7b23d1c393aed9)
		{
			if (this.xbc2aaa430ff675df == null)
			{
				goto IL_2D;
			}
			if (-2147483648 != 0)
			{
				this.x07d1191dbcdc9881(this.xbc2aaa430ff675df.get_xe6d4b1b411ed94b5(xee5efdfe033701c1), xbcea506a33cf9111, x0f7b23d1c393aed9);
				goto IL_2D;
			}
			return;
			IL_2D:
			x9da713b4847e9e6e x9da713b4847e9e6e = (x9da713b4847e9e6e)this.x1031720644474f9f(xbcea506a33cf9111);
			if (x9da713b4847e9e6e != null)
			{
				x9da713b4847e9e6e.x4e4178637618473f(xbcea506a33cf9111, xee5efdfe033701c1, x0f7b23d1c393aed9);
			}
		}

		private x66ec8c25e4c7547d x1031720644474f9f(object xbcea506a33cf9111)
		{
			if (!this.x8ab1b6f2b7722712)
			{
				return null;
			}
			Type type = xbcea506a33cf9111.GetType();
			if (type != this.x34eba590a5ec9a74)
			{
				int num = 0;
				if ((uint)num + (uint)num < 0U)
				{
					goto IL_A8;
				}
				IL_38:
				if (num < this.x55531fe975acd4f6.Length)
				{
					goto IL_A8;
				}
				if (type == this.x718bc396727ef5e4)
				{
					if (((uint)num & 0U) == 0U)
					{
						return null;
					}
				}
				else
				{
					TypeModel.ThrowUnexpectedSubtype(this.x34eba590a5ec9a74, type);
					bool flag = (uint)num - (uint)num > uint.MaxValue;
					if (!flag)
					{
						flag = ((uint)num - (uint)num < 0U);
						if (!flag)
						{
							return null;
						}
						if (false)
						{
							goto IL_A4;
						}
					}
				}
				IL_63:
				x66ec8c25e4c7547d x66ec8c25e4c7547d;
				if (x66ec8c25e4c7547d.x00c54479c3a7c440 != this.x34eba590a5ec9a74)
				{
					if (x479f2661aae93792.xd3703a5e339a1b56(x66ec8c25e4c7547d.x00c54479c3a7c440, type))
					{
						return x66ec8c25e4c7547d;
					}
				}
				IL_81:
				num++;
				goto IL_38;
				IL_A4:
				goto IL_81;
				IL_A8:
				x66ec8c25e4c7547d = this.x55531fe975acd4f6[num];
				goto IL_63;
			}
			return null;
		}

		public void x6210059f049f0d48(object xbcea506a33cf9111, ProtoWriter x6b8e154b42d5c1e3)
		{
			if (this.x9f191e0c449fb8d1)
			{
				this.x4e4178637618473f(xbcea506a33cf9111, TypeModel.CallbackType.BeforeSerialize, x6b8e154b42d5c1e3.Context);
			}
			for (;;)
			{
				x66ec8c25e4c7547d x66ec8c25e4c7547d = this.x1031720644474f9f(xbcea506a33cf9111);
				int i;
				if (((uint)i & 0U) != 0U || x66ec8c25e4c7547d != null)
				{
					x66ec8c25e4c7547d.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
				}
				for (;;)
				{
					i = 0;
					if (false)
					{
						break;
					}
					while (i < this.x55531fe975acd4f6.Length)
					{
						x66ec8c25e4c7547d x66ec8c25e4c7547d2 = this.x55531fe975acd4f6[i];
						if (x66ec8c25e4c7547d2.x00c54479c3a7c440 == this.x34eba590a5ec9a74)
						{
							x66ec8c25e4c7547d2.x6210059f049f0d48(xbcea506a33cf9111, x6b8e154b42d5c1e3);
						}
						i++;
					}
					if (this.x5e0d6f22b8494d3d)
					{
						ProtoWriter.AppendExtensionData((IExtensible)xbcea506a33cf9111, x6b8e154b42d5c1e3);
					}
					if (!this.x9f191e0c449fb8d1)
					{
						if (-2147483648 != 0)
						{
							return;
						}
					}
					else
					{
						this.x4e4178637618473f(xbcea506a33cf9111, TypeModel.CallbackType.AfterSerialize, x6b8e154b42d5c1e3.Context);
						if (!false)
						{
							return;
						}
					}
				}
			}
		}

		public object x06b0e25aa6ad68a9(object xbcea506a33cf9111, ProtoReader x337e217cb3ba0627)
		{
			if (this.x9f191e0c449fb8d1 && xbcea506a33cf9111 != null)
			{
				this.x4e4178637618473f(xbcea506a33cf9111, TypeModel.CallbackType.BeforeDeserialize, x337e217cb3ba0627.Context);
			}
			int num = 0;
			int num2 = 0;
			for (;;)
			{
				int num3;
				bool flag;
				bool flag2;
				int i;
				x66ec8c25e4c7547d x66ec8c25e4c7547d;
				if ((num3 = x337e217cb3ba0627.ReadFieldHeader()) <= 0)
				{
					if ((uint)num - (flag ? 1U : 0U) >= 0U)
					{
						flag2 = (((flag ? 1U : 0U) | 4U) == 0U);
						if (!flag2)
						{
							break;
						}
						if ((flag ? 1U : 0U) >= 0U)
						{
							goto Block_7;
						}
						flag2 = ((flag ? 1U : 0U) - (flag ? 1U : 0U) < 0U);
						if (flag2)
						{
							goto IL_1AE;
						}
						goto IL_17C;
					}
				}
				else
				{
					flag = false;
					if (num3 < num)
					{
						num2 = (num = 0);
						if ((uint)num3 - (flag ? 1U : 0U) < 0U)
						{
							continue;
						}
					}
					i = num2;
					if ((uint)num3 + (uint)i > 4294967295U)
					{
						goto IL_31;
					}
					if ((flag ? 1U : 0U) - (uint)num <= 4294967295U)
					{
						while (i < this.xed57745ced0c37a4.Length)
						{
							if (this.xed57745ced0c37a4[i] != num3)
							{
								i++;
							}
							else
							{
								x66ec8c25e4c7547d = this.x55531fe975acd4f6[i];
								if (xbcea506a33cf9111 == null)
								{
									goto IL_17C;
								}
								goto IL_167;
							}
						}
						goto IL_E8;
					}
					goto IL_14C;
				}
				continue;
				IL_E8:
				if (flag)
				{
					continue;
				}
				if (xbcea506a33cf9111 == null)
				{
					flag2 = (((uint)i | 3U) == 0U);
					if (flag2)
					{
						break;
					}
					if ((uint)num > 4294967295U)
					{
						continue;
					}
					xbcea506a33cf9111 = this.x8506c44a27b96f94(x337e217cb3ba0627);
				}
				if (!this.x5e0d6f22b8494d3d)
				{
					x337e217cb3ba0627.SkipField();
					continue;
				}
				x337e217cb3ba0627.AppendExtensionData((IExtensible)xbcea506a33cf9111);
				continue;
				IL_111:
				goto IL_E8;
				IL_14C:
				num = num3;
				flag = true;
				goto IL_111;
				IL_149:
				num2 = i;
				goto IL_14C;
				IL_23C:
				if (15 == 0)
				{
					goto IL_149;
				}
				x66ec8c25e4c7547d.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
				flag2 = (((flag ? 1U : 0U) & 0U) == 0U);
				if (flag2)
				{
					goto IL_149;
				}
				goto IL_111;
				IL_167:
				if (!x66ec8c25e4c7547d.xf33087968b28143c)
				{
					goto IL_23C;
				}
				IL_15A:
				xbcea506a33cf9111 = x66ec8c25e4c7547d.x06b0e25aa6ad68a9(xbcea506a33cf9111, x337e217cb3ba0627);
				goto IL_149;
				IL_236:
				if (false)
				{
					goto IL_23C;
				}
				if (false)
				{
					goto IL_15A;
				}
				goto IL_167;
				IL_1AE:
				xbcea506a33cf9111 = this.x8506c44a27b96f94(x337e217cb3ba0627);
				if (255 == 0)
				{
					goto IL_236;
				}
				goto IL_167;
				IL_17C:
				if (x66ec8c25e4c7547d.x00c54479c3a7c440 != this.x34eba590a5ec9a74)
				{
					goto IL_236;
				}
				goto IL_1AE;
			}
			IL_10:
			if (xbcea506a33cf9111 == null)
			{
				goto IL_31;
			}
			IL_13:
			if (this.x9f191e0c449fb8d1)
			{
				this.x4e4178637618473f(xbcea506a33cf9111, TypeModel.CallbackType.AfterDeserialize, x337e217cb3ba0627.Context);
			}
			return xbcea506a33cf9111;
			IL_31:
			xbcea506a33cf9111 = this.x8506c44a27b96f94(x337e217cb3ba0627);
			goto IL_13;
			Block_7:
			goto IL_31;
			goto IL_10;
		}

		private object x07d1191dbcdc9881(MethodInfo x1306445c04667cc7, object xa59bff7708de3a18, SerializationContext x0f7b23d1c393aed9)
		{
			object result = null;
			bool flag;
			if (4 != 0)
			{
				if (x1306445c04667cc7 == null)
				{
					return result;
				}
				flag = false;
				int num;
				if ((flag ? 1U : 0U) + (uint)num <= 4294967295U)
				{
					ParameterInfo[] parameters = x1306445c04667cc7.GetParameters();
					num = parameters.Length;
					switch (num)
					{
					case 0:
						result = x1306445c04667cc7.Invoke(xa59bff7708de3a18, null);
						flag = true;
						break;
					case 1:
					{
						Type parameterType = parameters[0].ParameterType;
						bool flag2 = (flag ? 1U : 0U) > uint.MaxValue;
						if (!flag2 && parameterType != typeof(SerializationContext))
						{
							flag2 = ((uint)num + (uint)num < 0U);
							if (flag2)
							{
								goto IL_4A;
							}
						}
						else
						{
							result = x1306445c04667cc7.Invoke(xa59bff7708de3a18, new object[]
							{
								x0f7b23d1c393aed9
							});
							flag = true;
						}
						break;
					}
					}
				}
			}
			IL_19:
			if (!flag)
			{
				goto IL_4A;
			}
			if (255 != 0)
			{
				return result;
			}
			goto IL_19;
			IL_4A:
			throw CallbackSet.x1922373fb3058bb0(x1306445c04667cc7);
		}

		private object x8506c44a27b96f94(ProtoReader x337e217cb3ba0627)
		{
			if (this.x64b16fcabdb0518e != null)
			{
				goto IL_12D;
			}
			goto IL_101;
			IL_CA:
			object obj = BclHelpers.GetUninitializedObject(this.x718bc396727ef5e4);
			IL_D6:
			ProtoReader.NoteObject(obj, x337e217cb3ba0627);
			int i;
			if (this.xb7037fcd8ee99709 == null)
			{
				if (((uint)i | 1U) != 0U)
				{
					goto IL_53;
				}
			}
			else
			{
				i = 0;
			}
			IL_64:
			while (i >= this.xb7037fcd8ee99709.Length)
			{
				if ((uint)i + (uint)i <= 4294967295U)
				{
					goto IL_53;
				}
			}
			this.x07d1191dbcdc9881(this.xb7037fcd8ee99709[i], obj, x337e217cb3ba0627.Context);
			goto IL_60;
			IL_53:
			if (this.xbc2aaa430ff675df != null)
			{
				this.x07d1191dbcdc9881(this.xbc2aaa430ff675df.BeforeDeserialize, obj, x337e217cb3ba0627.Context);
				if (false)
				{
					goto IL_60;
				}
				if (-2 == 0)
				{
					if (((uint)i & 0U) == 0U)
					{
						goto IL_12D;
					}
					bool flag = (uint)i + (uint)i < 0U;
					if (flag)
					{
						goto IL_CA;
					}
					goto IL_101;
				}
			}
			return obj;
			IL_60:
			i++;
			goto IL_64;
			IL_101:
			if (this.xdcf89e90b059245d)
			{
				if (!this.x1f170e14e19c30aa)
				{
					TypeModel.ThrowCannotCreateInstance(this.x718bc396727ef5e4);
				}
				obj = Activator.CreateInstance(this.x718bc396727ef5e4, true);
				goto IL_D6;
			}
			goto IL_CA;
			IL_12D:
			obj = this.x07d1191dbcdc9881(this.x64b16fcabdb0518e, null, x337e217cb3ba0627.Context);
			goto IL_D6;
		}

		bool x66ec8c25e4c7547d.x84790f641eeacdd2
		{
			get
			{
				return true;
			}
		}

		bool x66ec8c25e4c7547d.x60089ae1c8e627b2
		{
			get
			{
				return false;
			}
		}

		private readonly Type x34eba590a5ec9a74;

		private readonly Type x718bc396727ef5e4;

		private readonly x66ec8c25e4c7547d[] x55531fe975acd4f6;

		private readonly int[] xed57745ced0c37a4;

		private readonly bool x9f191e0c449fb8d1;

		private readonly bool xdcf89e90b059245d;

		private readonly bool x5e0d6f22b8494d3d;

		private readonly bool x1f170e14e19c30aa;

		private readonly CallbackSet xbc2aaa430ff675df;

		private readonly MethodInfo[] xb7037fcd8ee99709;

		private readonly MethodInfo x64b16fcabdb0518e;

		private static readonly Type x5f4400404e3d6700 = typeof(IExtensible);
	}
}
