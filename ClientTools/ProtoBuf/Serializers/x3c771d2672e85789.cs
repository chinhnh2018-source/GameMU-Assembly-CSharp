using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuf.Serializers
{
	internal class x3c771d2672e85789
	{
		public static object xc0d16742a0a2ab91(string x11e2b209aab110d3)
		{
			x3c771d2672e85789.x7df73acf28d072a1 = x11e2b209aab110d3;
			int num;
			bool flag2;
			for (;;)
			{
				bool flag = (uint)num - (uint)num < 0U;
				if (flag || x11e2b209aab110d3 != null)
				{
					goto IL_A7;
				}
				flag = ((flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue);
				if (flag)
				{
					goto Block_3;
				}
				flag = ((flag2 ? 1U : 0U) + (uint)num > uint.MaxValue);
				if (!flag)
				{
					goto IL_B2;
				}
			}
			IL_34:
			x3c771d2672e85789.xf52c112c1b49bf7b = num;
			object result;
			return result;
			Block_3:
			if ((uint)num + (uint)num >= 0U)
			{
				goto IL_34;
			}
			IL_56:
			if (flag2)
			{
				x3c771d2672e85789.xf52c112c1b49bf7b = -1;
				return result;
			}
			if (3 != 0)
			{
				goto IL_34;
			}
			goto IL_34;
			IL_A7:
			char[] x11e2b209aab110d4 = x11e2b209aab110d3.ToCharArray();
			num = 0;
			flag2 = true;
			result = x3c771d2672e85789.xe464bdcf3db42586(x11e2b209aab110d4, ref num, ref flag2);
			goto IL_56;
			IL_B2:
			return null;
		}

		public static string x7603f80ae89ed7d1(object x11e2b209aab110d3)
		{
			StringBuilder stringBuilder = new StringBuilder(2000);
			bool flag;
			if ((flag ? 1U : 0U) - (flag ? 1U : 0U) >= 0U)
			{
				flag = x3c771d2672e85789.x18017ffcd5c7e2ae(x11e2b209aab110d3, stringBuilder);
				if (!flag)
				{
					return null;
				}
			}
			return stringBuilder.ToString();
		}

		public static bool x28884227c6ce27dd()
		{
			return x3c771d2672e85789.xf52c112c1b49bf7b == -1;
		}

		public static int x54411b2a6aec4bdf()
		{
			return x3c771d2672e85789.xf52c112c1b49bf7b;
		}

		public static string x4797ac62a173c6c6()
		{
			int num;
			if (x3c771d2672e85789.xf52c112c1b49bf7b == -1)
			{
				bool flag = (uint)num < 0U;
				if (!flag)
				{
					goto IL_89;
				}
			}
			int num2 = x3c771d2672e85789.xf52c112c1b49bf7b - 5;
			do
			{
				num = x3c771d2672e85789.xf52c112c1b49bf7b + 15;
				bool flag = (uint)num + (uint)num < 0U;
				if (flag || num2 < 0)
				{
					goto IL_71;
				}
			}
			while (false);
			IL_1F:
			if (num < x3c771d2672e85789.x7df73acf28d072a1.Length)
			{
				if (((uint)num & 0U) != 0U)
				{
					goto IL_89;
				}
			}
			else
			{
				num = x3c771d2672e85789.x7df73acf28d072a1.Length - 1;
			}
			return x3c771d2672e85789.x7df73acf28d072a1.Substring(num2, num - num2 + 1);
			goto IL_1F;
			IL_71:
			num2 = 0;
			goto IL_1F;
			IL_89:
			return "";
		}

		protected static Hashtable x30f085f1ccc05ade(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00)
		{
			Hashtable hashtable = new Hashtable();
			bool flag3;
			bool flag2;
			for (;;)
			{
				IL_17E:
				x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
				bool flag = false;
				for (;;)
				{
					if (!flag)
					{
						goto IL_18D;
					}
					if (255 == 0)
					{
						goto IL_113;
					}
					flag2 = (((flag3 ? 1U : 0U) | uint.MaxValue) == 0U);
					if (!flag2)
					{
						return hashtable;
					}
					if (((flag3 ? 1U : 0U) | 4294967295U) == 0U)
					{
						goto IL_12D;
					}
					if (false)
					{
						goto IL_18D;
					}
					IL_11:
					string text;
					object value;
					hashtable[text] = value;
					continue;
					IL_12D:
					x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
					continue;
					IL_113:
					int num;
					if (num == 6)
					{
						goto IL_12D;
					}
					IL_C7:
					while (num != 2)
					{
						do
						{
							text = x3c771d2672e85789.x95db5d235451d5fa(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
							if (text == null)
							{
								goto IL_9D;
							}
							num = x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
							if ((flag3 ? 1U : 0U) - (flag ? 1U : 0U) > 4294967295U)
							{
								goto IL_C7;
							}
						}
						while (false);
						if (8 == 0)
						{
							return hashtable;
						}
						flag2 = ((flag3 ? 1U : 0U) - (uint)num < 0U);
						if (flag2)
						{
							flag2 = ((uint)num + (uint)num > uint.MaxValue);
							if (flag2)
							{
								goto Block_12;
							}
							goto IL_153;
						}
						else
						{
							if (2 == 0)
							{
								goto IL_17E;
							}
							if (num != 5)
							{
								goto Block_6;
							}
							flag3 = true;
							if ((uint)num <= 4294967295U)
							{
								value = x3c771d2672e85789.xe464bdcf3db42586(x11e2b209aab110d3, ref xc0c4c459c6ccbd00, ref flag3);
							}
							if (flag3)
							{
								goto IL_11;
							}
							goto IL_69;
						}
					}
					x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
					flag2 = (((uint)num | uint.MaxValue) == 0U);
					if (!flag2)
					{
						goto IL_172;
					}
					IL_153:
					if (2 == 0)
					{
						goto Block_15;
					}
					if (num != 0)
					{
						goto IL_113;
					}
					goto IL_139;
					IL_18D:
					num = x3c771d2672e85789.x92b1f39635dc2e69(x11e2b209aab110d3, xc0c4c459c6ccbd00);
					goto IL_153;
				}
			}
			return hashtable;
			IL_69:
			return null;
			Block_6:
			return null;
			IL_9D:
			return null;
			Block_12:
			IL_139:
			return null;
			Block_15:
			flag2 = (((flag3 ? 1U : 0U) | 4294967294U) == 0U);
			if (!flag2)
			{
				goto IL_139;
			}
			IL_172:
			return hashtable;
		}

		protected static ArrayList x9e224c41c42ca8ee(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00)
		{
			ArrayList arrayList = new ArrayList();
			if (15 == 0)
			{
				goto IL_4F;
			}
			x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			bool flag = false;
			bool flag3;
			bool flag2 = (flag3 ? 1U : 0U) + (flag ? 1U : 0U) > uint.MaxValue;
			if (flag2)
			{
				goto IL_ED;
			}
			IL_1E:
			if (flag)
			{
				return arrayList;
			}
			int num = x3c771d2672e85789.x92b1f39635dc2e69(x11e2b209aab110d3, xc0c4c459c6ccbd00);
			if (num == 0)
			{
				return null;
			}
			if (num == 6)
			{
				x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
				flag2 = ((flag3 ? 1U : 0U) + (flag3 ? 1U : 0U) < 0U);
				if (flag2)
				{
					goto IL_ED;
				}
				goto IL_8A;
			}
			IL_35:
			if (num == 4)
			{
				goto IL_7C;
			}
			flag3 = true;
			if (((flag ? 1U : 0U) & 0U) == 0U)
			{
				object value = x3c771d2672e85789.xe464bdcf3db42586(x11e2b209aab110d3, ref xc0c4c459c6ccbd00, ref flag3);
				if (flag3)
				{
					arrayList.Add(value);
					goto IL_1E;
				}
				return null;
			}
			IL_4F:
			if ((flag3 ? 1U : 0U) + (uint)num > 4294967295U)
			{
				goto IL_8F;
			}
			if ((flag ? 1U : 0U) > 4294967295U)
			{
				goto IL_8A;
			}
			if (!false)
			{
				goto IL_35;
			}
			IL_7C:
			x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			return arrayList;
			IL_8A:
			IL_8F:
			goto IL_1E;
			IL_ED:
			goto IL_4F;
		}

		protected static object xe464bdcf3db42586(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00, ref bool xd938fd32778a1c95)
		{
			int num = x3c771d2672e85789.x92b1f39635dc2e69(x11e2b209aab110d3, xc0c4c459c6ccbd00);
			for (;;)
			{
				switch (num)
				{
				case 1:
					goto IL_37;
				case 3:
					goto IL_3F;
				case 7:
					goto IL_A4;
				case 8:
					goto IL_AC;
				case 9:
					goto IL_47;
				case 10:
					goto IL_5F;
				case 11:
					goto IL_1B;
				}
				xd938fd32778a1c95 = false;
				if (3 == 0)
				{
					goto IL_5F;
				}
				if (!false)
				{
					goto IL_BE;
				}
			}
			IL_1B:
			x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			return null;
			IL_37:
			return x3c771d2672e85789.x30f085f1ccc05ade(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			IL_3F:
			return x3c771d2672e85789.x9e224c41c42ca8ee(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			IL_47:
			x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			return bool.Parse("TRUE");
			IL_5F:
			x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			return bool.Parse("FALSE");
			IL_A4:
			return x3c771d2672e85789.x95db5d235451d5fa(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			IL_AC:
			return x3c771d2672e85789.xe903d4e242ac5b50(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			IL_BE:
			return null;
		}

		protected static string x95db5d235451d5fa(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00)
		{
			string text = "";
			int num;
			int num2;
			bool flag = (uint)num + (uint)num2 > uint.MaxValue;
			bool flag2;
			if (!flag)
			{
				int num3;
				if ((uint)num2 - (uint)num3 >= 0U)
				{
					goto IL_30B;
				}
				goto IL_1C9;
				IL_29:
				if (flag2)
				{
					goto IL_21;
				}
				if (xc0c4c459c6ccbd00 != x11e2b209aab110d3.Length)
				{
					goto IL_2E8;
				}
				goto IL_21;
				IL_7C:
				xc0c4c459c6ccbd00 += 4;
				IL_172:
				goto IL_29;
				IL_1C9:
				char c;
				if (c == '/')
				{
					text += '/';
					goto IL_29;
				}
				int num4;
				if ((flag2 ? 1U : 0U) <= 4294967295U)
				{
					if (c != 'b')
					{
						if (4 != 0)
						{
							goto IL_19D;
						}
						IL_177:
						if (c == 'n')
						{
							text += '\n';
							flag = ((uint)num4 + (uint)num3 < 0U);
							if (!flag)
							{
								goto IL_172;
							}
						}
						flag = ((uint)num4 + (uint)num4 < 0U);
						if (!flag)
						{
							flag = ((uint)num2 + (uint)num4 > uint.MaxValue);
							if (!flag)
							{
								if (c != 'r')
								{
									if ((flag2 ? 1U : 0U) > 4294967295U)
									{
										goto IL_1C4;
									}
									flag = ((uint)num3 - (uint)num4 > uint.MaxValue);
									if (!flag)
									{
										goto IL_369;
									}
									flag = ((uint)num > uint.MaxValue);
									if (flag)
									{
										goto IL_172;
									}
									goto IL_30B;
								}
							}
							text += '\r';
							goto IL_29;
						}
						IL_369:
						flag = (((flag2 ? 1U : 0U) & 0U) == 0U);
						if (flag)
						{
							if (c == 't')
							{
								text += '\t';
								goto IL_29;
							}
							if (c != 'u')
							{
								goto IL_29;
							}
							num2 = x11e2b209aab110d3.Length - xc0c4c459c6ccbd00;
							if (num2 < 4)
							{
								goto IL_21;
							}
							flag = ((uint)num3 < 0U);
							if (flag)
							{
								goto IL_30B;
							}
							char[] array = new char[4];
							Array.Copy(x11e2b209aab110d3, xc0c4c459c6ccbd00, array, 0, 4);
							text = text + "&#x" + new string(array) + ";";
							goto IL_7C;
						}
						IL_19D:
						if (c != 'f')
						{
							goto IL_177;
						}
						text += '\f';
					}
					else
					{
						text += '\b';
					}
					IL_1C4:
					goto IL_29;
				}
				goto IL_2F6;
				IL_2E8:
				num = xc0c4c459c6ccbd00++;
				c = x11e2b209aab110d3[num];
				IL_2F6:
				while (c != '"')
				{
					if (c != '\\')
					{
						text += c;
						goto IL_29;
					}
					flag = (((uint)num4 | 2147483648U) == 0U);
					if (!flag && xc0c4c459c6ccbd00 == x11e2b209aab110d3.Length)
					{
						goto IL_21;
					}
					num4 = xc0c4c459c6ccbd00++;
					c = x11e2b209aab110d3[num4];
					flag = ((uint)num2 + (uint)num4 < 0U);
					if (flag)
					{
						flag = ((uint)num4 - (uint)num3 > uint.MaxValue);
						if (!flag)
						{
							goto IL_2E8;
						}
					}
					else
					{
						flag = ((uint)num3 + (uint)num > uint.MaxValue);
						if (flag || c == '"')
						{
							text += '"';
							goto IL_29;
						}
						if (c != '\\')
						{
							goto IL_1C9;
						}
						text += '\\';
						flag = (((uint)num4 & 0U) == 0U);
						if (!flag)
						{
							goto IL_7C;
						}
						if ((flag2 ? 1U : 0U) - (uint)num4 <= 4294967295U)
						{
							goto IL_29;
						}
						goto IL_172;
					}
				}
				flag2 = true;
				goto IL_21;
				IL_30B:
				x3c771d2672e85789.x6503e8821f6023fa(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
				num3 = xc0c4c459c6ccbd00++;
				c = x11e2b209aab110d3[num3];
				flag2 = false;
				goto IL_29;
			}
			IL_21:
			if (!flag2)
			{
				return null;
			}
			return text;
		}

		protected static double xe903d4e242ac5b50(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00)
		{
			x3c771d2672e85789.x6503e8821f6023fa(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			int num = x3c771d2672e85789.xdb23184c8634c3aa(x11e2b209aab110d3, xc0c4c459c6ccbd00);
			int num2;
			do
			{
				num2 = num - xc0c4c459c6ccbd00 + 1;
			}
			while ((uint)num2 + (uint)num2 < 0U);
			char[] array = new char[num2];
			Array.Copy(x11e2b209aab110d3, xc0c4c459c6ccbd00, array, 0, num2);
			xc0c4c459c6ccbd00 = num + 1;
			return double.Parse(new string(array));
		}

		protected static int xdb23184c8634c3aa(char[] x11e2b209aab110d3, int xc0c4c459c6ccbd00)
		{
			int num = xc0c4c459c6ccbd00;
			while (num < x11e2b209aab110d3.Length || (uint)num > 4294967295U)
			{
				if ("0123456789+-.eE".IndexOf(x11e2b209aab110d3[num]) == -1)
				{
					IL_38:
					return num - 1;
				}
				num++;
			}
			goto IL_38;
		}

		protected static void x6503e8821f6023fa(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00)
		{
			while (xc0c4c459c6ccbd00 < x11e2b209aab110d3.Length)
			{
				if (" \t\n\r".IndexOf(x11e2b209aab110d3[xc0c4c459c6ccbd00]) == -1)
				{
					return;
				}
				xc0c4c459c6ccbd00++;
			}
		}

		protected static int x92b1f39635dc2e69(char[] x11e2b209aab110d3, int xc0c4c459c6ccbd00)
		{
			int num = xc0c4c459c6ccbd00;
			return x3c771d2672e85789.xab05c62a2a183f47(x11e2b209aab110d3, ref num);
		}

		protected static int xab05c62a2a183f47(char[] x11e2b209aab110d3, ref int xc0c4c459c6ccbd00)
		{
			x3c771d2672e85789.x6503e8821f6023fa(x11e2b209aab110d3, ref xc0c4c459c6ccbd00);
			if (xc0c4c459c6ccbd00 != x11e2b209aab110d3.Length)
			{
				char c = x11e2b209aab110d3[xc0c4c459c6ccbd00];
				int num;
				if (((uint)num | 255U) != 0U)
				{
					for (;;)
					{
						xc0c4c459c6ccbd00++;
						char c2 = c;
						switch (c2)
						{
						case '"':
							return 7;
						case '#':
						case '$':
						case '%':
						case '&':
						case '\'':
						case '(':
						case ')':
						case '*':
						case '+':
						case '.':
						case '/':
							break;
						case ',':
							return 6;
						case '-':
						case '0':
						case '1':
						case '2':
						case '3':
						case '4':
						case '5':
						case '6':
						case '7':
						case '8':
						case '9':
							return 8;
						case ':':
							return 5;
						default:
							if (false)
							{
								goto Block_32;
							}
							switch (c2)
							{
							case '[':
								return 3;
							case '\\':
								break;
							case ']':
								return 4;
							default:
								switch (c2)
								{
								case '{':
									return 1;
								case '}':
									return 2;
								}
								break;
							}
							break;
						}
						xc0c4c459c6ccbd00--;
						if (-1 == 0)
						{
							goto IL_1F0;
						}
						if (false)
						{
							goto IL_1D6;
						}
						if (4 != 0)
						{
							goto IL_1F0;
						}
						goto IL_41;
						IL_1F7:
						if (num < 5)
						{
							goto IL_101;
						}
						if (8 != 0)
						{
							goto IL_181;
						}
						continue;
						IL_A7:
						if (15 != 0)
						{
							goto IL_41;
						}
						goto IL_1F7;
						IL_1F0:
						num = x11e2b209aab110d3.Length - xc0c4c459c6ccbd00;
						goto IL_1F7;
						IL_101:
						bool flag;
						if (num >= 4)
						{
							while (x11e2b209aab110d3[xc0c4c459c6ccbd00] == 't' && x11e2b209aab110d3[xc0c4c459c6ccbd00 + 1] == 'r')
							{
								if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 2] != 'u')
								{
									if (false)
									{
										continue;
									}
								}
								else
								{
									if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 3] != 'e')
									{
										break;
									}
									xc0c4c459c6ccbd00 += 4;
									if (!false)
									{
										return 9;
									}
								}
								if (false)
								{
									goto IL_FE;
								}
								if (255 == 0)
								{
									goto IL_115;
								}
								flag = (((uint)num | 3U) == 0U);
								if (flag)
								{
									goto IL_17C;
								}
								goto IL_A7;
							}
							goto IL_41;
						}
						goto IL_41;
						IL_115:
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 4] == 'e')
						{
							goto IL_1E8;
						}
						flag = ((uint)num + (uint)num > uint.MaxValue);
						if (!flag)
						{
							goto IL_101;
						}
						if (2147483647 != 0)
						{
							goto IL_101;
						}
						goto IL_181;
						IL_FE:
						if (!false)
						{
							goto IL_101;
						}
						goto IL_115;
						IL_41:
						if (num < 4)
						{
							goto Block_3;
						}
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00] != 'n')
						{
							return 0;
						}
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 1] != 'u')
						{
							if (!false)
							{
								goto Block_2;
							}
						}
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 2] != 'l')
						{
							break;
						}
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 3] != 'l' || (uint)num - (uint)num > 4294967295U)
						{
							return 0;
						}
						flag = (((uint)num | 1U) == 0U);
						if (flag)
						{
							goto IL_A7;
						}
						goto IL_B6;
						IL_17C:
						goto IL_101;
						IL_181:
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00] != 'f')
						{
							goto IL_17C;
						}
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 1] != 'a')
						{
							if (((uint)num & 0U) == 0U)
							{
								goto IL_101;
							}
							flag = ((uint)num + (uint)num < 0U);
							if (flag)
							{
								goto IL_115;
							}
							if (3 == 0)
							{
								return 11;
							}
							if (!false)
							{
								goto IL_16D;
							}
							goto IL_37;
						}
						IL_1D6:
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 2] != 'l')
						{
							goto IL_101;
						}
						IL_16D:
						if (x11e2b209aab110d3[xc0c4c459c6ccbd00 + 3] != 's')
						{
							goto IL_FE;
						}
						goto IL_115;
					}
					if ((uint)num > 4294967295U)
					{
						return 6;
					}
					IL_37:
					Block_2:
					Block_3:
					return 0;
					IL_B6:
					goto IL_4C;
					IL_1E8:
					xc0c4c459c6ccbd00 += 5;
					return 10;
					Block_32:
					return 4;
				}
				IL_4C:
				xc0c4c459c6ccbd00 += 4;
				return 11;
			}
			return 0;
		}

		protected static bool xca1f06ca5a1f26ac(object x50a87f60bf5d15ae, StringBuilder xd07ce4b74c5774a7)
		{
			if (x50a87f60bf5d15ae is Hashtable)
			{
				return x3c771d2672e85789.x988d678ef128ed4f((Hashtable)x50a87f60bf5d15ae, xd07ce4b74c5774a7);
			}
			while (!(x50a87f60bf5d15ae is ArrayList))
			{
				if (!false)
				{
					return false;
				}
			}
			return x3c771d2672e85789.x327a6c5b340675b5((ArrayList)x50a87f60bf5d15ae, xd07ce4b74c5774a7);
		}

		protected static bool x988d678ef128ed4f(Hashtable xa5c6726892f9f63d, StringBuilder xd07ce4b74c5774a7)
		{
			xd07ce4b74c5774a7.Append("{");
			bool flag2;
			bool flag = ((flag2 ? 1U : 0U) | 2147483647U) == 0U;
			if (flag)
			{
				goto IL_6A;
			}
			goto IL_B6;
			IL_29:
			IDictionaryEnumerator enumerator;
			string x0c0ec4acfbf729d;
			object value;
			if (!enumerator.MoveNext())
			{
				xd07ce4b74c5774a7.Append("}");
				if (!false)
				{
					if (2147483647 == 0)
					{
						goto IL_B6;
					}
					return true;
				}
			}
			else
			{
				x0c0ec4acfbf729d = enumerator.Key.ToString();
				value = enumerator.Value;
				do
				{
					flag = ((flag2 ? 1U : 0U) - (flag2 ? 1U : 0U) > uint.MaxValue);
					if (!flag && flag2)
					{
						break;
					}
					xd07ce4b74c5774a7.Append(", ");
				}
				while (false);
			}
			IL_40:
			x3c771d2672e85789.x8acd328265e1a735(x0c0ec4acfbf729d, xd07ce4b74c5774a7);
			xd07ce4b74c5774a7.Append(":");
			if (false)
			{
				goto IL_BD;
			}
			if (!x3c771d2672e85789.x18017ffcd5c7e2ae(value, xd07ce4b74c5774a7))
			{
				return false;
			}
			flag2 = false;
			goto IL_29;
			IL_6A:
			goto IL_40;
			IL_B6:
			enumerator = xa5c6726892f9f63d.GetEnumerator();
			IL_BD:
			flag2 = true;
			goto IL_29;
		}

		protected static bool x7174baaa60fba55a(Dictionary<string, string> x273c212ea6c4689b, StringBuilder xd07ce4b74c5774a7)
		{
			xd07ce4b74c5774a7.Append("{");
			bool flag = true;
			using (Dictionary<string, string>.Enumerator enumerator = x273c212ea6c4689b.GetEnumerator())
			{
				for (;;)
				{
					if (!enumerator.MoveNext())
					{
						if (15 != 0)
						{
							break;
						}
					}
					else
					{
						KeyValuePair<string, string> keyValuePair = enumerator.Current;
						if (!flag)
						{
							xd07ce4b74c5774a7.Append(", ");
						}
						x3c771d2672e85789.x8acd328265e1a735(keyValuePair.Key, xd07ce4b74c5774a7);
						xd07ce4b74c5774a7.Append(":");
						x3c771d2672e85789.x8acd328265e1a735(keyValuePair.Value, xd07ce4b74c5774a7);
					}
					flag = false;
				}
			}
			xd07ce4b74c5774a7.Append("}");
			return true;
		}

		protected static bool x327a6c5b340675b5(ArrayList xfce3f26cf248085d, StringBuilder xd07ce4b74c5774a7)
		{
			xd07ce4b74c5774a7.Append("[");
			bool flag = true;
			int num = 0;
			for (;;)
			{
				object xbcea506a33cf;
				if (num < xfce3f26cf248085d.Count)
				{
					xbcea506a33cf = xfce3f26cf248085d[num];
					goto IL_9C;
				}
				bool flag2 = (flag ? 1U : 0U) + (uint)num > uint.MaxValue;
				if (flag2)
				{
					goto IL_77;
				}
				goto IL_95;
				continue;
				IL_4D:
				if (!x3c771d2672e85789.x18017ffcd5c7e2ae(xbcea506a33cf, xd07ce4b74c5774a7))
				{
					break;
				}
				flag = false;
				num++;
				flag2 = ((flag ? 1U : 0U) > uint.MaxValue);
				if (!flag2)
				{
					continue;
				}
				IL_6E:
				goto IL_4D;
				IL_77:
				if (255 != 0)
				{
					flag2 = ((uint)num > uint.MaxValue);
					if (flag2)
					{
						continue;
					}
					goto IL_6E;
				}
				IL_70:
				if (flag)
				{
					if (false)
					{
						break;
					}
					goto IL_4D;
				}
				else
				{
					xd07ce4b74c5774a7.Append(", ");
					if (((flag ? 1U : 0U) | 3U) != 0U)
					{
						goto IL_77;
					}
				}
				IL_9C:
				goto IL_70;
			}
			return false;
			IL_95:
			xd07ce4b74c5774a7.Append("]");
			return true;
		}

		protected static bool x18017ffcd5c7e2ae(object xbcea506a33cf9111, StringBuilder xd07ce4b74c5774a7)
		{
			if (xbcea506a33cf9111 == null)
			{
				xd07ce4b74c5774a7.Append("null");
			}
			else if (!xbcea506a33cf9111.GetType().IsArray)
			{
				if (!(xbcea506a33cf9111 is string))
				{
					if (false)
					{
						return true;
					}
				}
				else
				{
					x3c771d2672e85789.x8acd328265e1a735((string)xbcea506a33cf9111, xd07ce4b74c5774a7);
					if (4 != 0)
					{
						return true;
					}
				}
				if (!(xbcea506a33cf9111 is char))
				{
					if (xbcea506a33cf9111 is Hashtable)
					{
						x3c771d2672e85789.x988d678ef128ed4f((Hashtable)xbcea506a33cf9111, xd07ce4b74c5774a7);
					}
					else if (!(xbcea506a33cf9111 is Dictionary<string, string>))
					{
						if (xbcea506a33cf9111 is ArrayList)
						{
							x3c771d2672e85789.x327a6c5b340675b5((ArrayList)xbcea506a33cf9111, xd07ce4b74c5774a7);
						}
						else
						{
							for (;;)
							{
								if (!(xbcea506a33cf9111 is bool))
								{
									goto IL_30;
								}
								if (!(bool)xbcea506a33cf9111)
								{
									goto IL_30;
								}
								xd07ce4b74c5774a7.Append("true");
								if (8 != 0)
								{
									goto IL_50;
								}
								IL_7C:
								if (!true)
								{
									continue;
								}
								goto IL_85;
								IL_30:
								if (!(xbcea506a33cf9111 is bool))
								{
									break;
								}
								if (!(bool)xbcea506a33cf9111)
								{
									xd07ce4b74c5774a7.Append("false");
									goto IL_7C;
								}
								break;
							}
							IL_1C:
							if (!xbcea506a33cf9111.GetType().IsPrimitive)
							{
								return false;
							}
							x3c771d2672e85789.x27357a604dab004b(Convert.ToDouble(xbcea506a33cf9111), xd07ce4b74c5774a7);
							return true;
							goto IL_1C;
							IL_50:
							IL_85:;
						}
					}
					else
					{
						x3c771d2672e85789.x7174baaa60fba55a((Dictionary<string, string>)xbcea506a33cf9111, xd07ce4b74c5774a7);
					}
				}
				else
				{
					x3c771d2672e85789.x8acd328265e1a735(Convert.ToString((char)xbcea506a33cf9111), xd07ce4b74c5774a7);
				}
			}
			else
			{
				x3c771d2672e85789.x327a6c5b340675b5(new ArrayList((ICollection)xbcea506a33cf9111), xd07ce4b74c5774a7);
			}
			return true;
		}

		protected static void x8acd328265e1a735(string x0c0ec4acfbf729d1, StringBuilder xd07ce4b74c5774a7)
		{
			xd07ce4b74c5774a7.Append("\"");
			char[] array = x0c0ec4acfbf729d1.ToCharArray();
			int num;
			if (!false)
			{
				num = 0;
				goto IL_38;
			}
			goto IL_A1;
			IL_34:
			num++;
			IL_38:
			int num2;
			char c;
			if (num >= array.Length)
			{
				if ((uint)num2 - (uint)num >= 0U)
				{
					xd07ce4b74c5774a7.Append("\"");
					return;
				}
			}
			else
			{
				c = array[num];
				if (c == '"')
				{
					xd07ce4b74c5774a7.Append("\\\"");
					goto IL_34;
				}
			}
			if (c == '\\')
			{
				xd07ce4b74c5774a7.Append("\\\\");
				goto IL_34;
			}
			bool flag = (uint)num + (uint)num > uint.MaxValue;
			if (!flag)
			{
				IL_164:
				while (c != '\b')
				{
					while (!false && c != '\f')
					{
						flag = ((uint)num - (uint)num < 0U);
						if (!flag)
						{
							if ((uint)num2 < 0U)
							{
								goto IL_8D;
							}
							if (false)
							{
								flag = ((uint)num - (uint)num < 0U);
								if (flag)
								{
									continue;
								}
								goto IL_164;
							}
						}
						flag = ((uint)num2 > uint.MaxValue);
						if (flag)
						{
							goto IL_108;
						}
						IL_E8:
						if (c != '\n')
						{
							goto IL_A1;
						}
						IL_108:
						xd07ce4b74c5774a7.Append("\\n");
						if (3 == 0)
						{
							goto IL_E8;
						}
						goto IL_34;
					}
					xd07ce4b74c5774a7.Append("\\f");
					goto IL_34;
				}
				xd07ce4b74c5774a7.Append("\\b");
				goto IL_34;
			}
			if (!false)
			{
				goto IL_BF;
			}
			flag = ((uint)num2 < 0U);
			if (flag)
			{
				goto IL_1EB;
			}
			IL_6D:
			if (c == '\t')
			{
				goto IL_8F;
			}
			num2 = Convert.ToInt32(c);
			if (num2 >= 32 && num2 <= 126)
			{
				xd07ce4b74c5774a7.Append(c);
				goto IL_BA;
			}
			xd07ce4b74c5774a7.Append("\\u" + Convert.ToString(num2, 16).PadLeft(4, '0'));
			goto IL_34;
			IL_8D:
			goto IL_6D;
			IL_8F:
			xd07ce4b74c5774a7.Append("\\t");
			goto IL_34;
			IL_A1:
			if (c == '\r')
			{
				goto IL_BF;
			}
			if (((uint)num2 & 0U) == 0U)
			{
				goto IL_8D;
			}
			IL_BA:
			goto IL_1EB;
			IL_BF:
			xd07ce4b74c5774a7.Append("\\r");
			if ((uint)num + (uint)num2 >= 0U)
			{
				goto IL_34;
			}
			goto IL_8F;
			IL_1EB:
			flag = ((uint)num2 < 0U);
			if (!flag)
			{
				goto IL_34;
			}
		}

		protected static void x27357a604dab004b(double x78b0a0bc28459535, StringBuilder xd07ce4b74c5774a7)
		{
			xd07ce4b74c5774a7.Append(Convert.ToString(x78b0a0bc28459535));
		}

		private const int x104b4448c6929fbe = 0;

		private const int xdf52104353ab3519 = 1;

		private const int x75bb5767c5ad7d0e = 2;

		private const int x64737ca07754510f = 3;

		private const int x94707e45b98184ac = 4;

		private const int x80c8359022c75c01 = 5;

		private const int xc4888b2bc85b431c = 6;

		private const int x44de952adb034cd7 = 7;

		private const int x261777e1fe4c031e = 8;

		private const int xb1aa669f2c4f41fa = 9;

		private const int x85e06f5d5671f175 = 10;

		private const int xf66bd4ea39af51a1 = 11;

		private const int xcd37ee7186ba98a4 = 2000;

		protected static int xf52c112c1b49bf7b = -1;

		protected static string x7df73acf28d072a1 = "";
	}
}
