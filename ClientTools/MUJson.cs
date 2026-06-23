using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class MUJson
{
	public static object jsonDecode(string json)
	{
		MUJson.lastDecode = json;
		while (json != null)
		{
			char[] json2 = json.ToCharArray();
			bool flag;
			if ((flag ? 1U : 0U) - (flag ? 1U : 0U) > 4294967295U)
			{
				IL_8B:
				return null;
			}
			int num = 0;
			flag = true;
			object result = MUJson.parseValue(json2, ref num, ref flag);
			if ((flag ? 1U : 0U) - (uint)num <= 4294967295U)
			{
				if (flag)
				{
					if (((uint)num & 0U) != 0U)
					{
						continue;
					}
					MUJson.lastErrorIndex = -1;
				}
				else
				{
					MUJson.lastErrorIndex = num;
				}
				return result;
			}
		}
		goto IL_8B;
	}

	public static string jsonEncode(object json)
	{
		StringBuilder stringBuilder = new StringBuilder(2000);
		if (!MUJson.serializeValue(json, stringBuilder))
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	public static bool lastDecodeSuccessful()
	{
		return MUJson.lastErrorIndex == -1;
	}

	public static int getLastErrorIndex()
	{
		return MUJson.lastErrorIndex;
	}

	public static string getLastErrorSnippet()
	{
		int num;
		bool flag;
		int num2;
		if (MUJson.lastErrorIndex == -1)
		{
			flag = ((uint)num < 0U);
			if (!flag)
			{
				goto IL_97;
			}
		}
		else
		{
			do
			{
				num = MUJson.lastErrorIndex - 5;
				num2 = MUJson.lastErrorIndex + 15;
				if (num < 0)
				{
					num = 0;
				}
				if (num2 < MUJson.lastDecode.Length)
				{
					goto IL_A2;
				}
				flag = ((uint)num > uint.MaxValue);
			}
			while (flag);
		}
		do
		{
			num2 = MUJson.lastDecode.Length - 1;
		}
		while ((uint)num2 - (uint)num < 0U);
		flag = ((uint)num2 < 0U);
		if (!flag)
		{
			goto IL_A2;
		}
		IL_97:
		return "";
		IL_A2:
		return MUJson.lastDecode.Substring(num, num2 - num + 1);
	}

	protected static Hashtable parseObject(char[] json, ref int index)
	{
		Hashtable hashtable = new Hashtable();
		bool flag;
		if (((flag ? 1U : 0U) & 0U) == 0U)
		{
			goto IL_14D;
		}
		IL_1D:
		bool flag2;
		int num;
		if (!flag2)
		{
			num = MUJson.lookAhead(json, index);
			goto IL_146;
		}
		if (((uint)num | 2147483647U) != 0U)
		{
			return hashtable;
		}
		IL_BE:
		string text;
		if (text != null)
		{
			num = MUJson.nextToken(json, ref index);
			if (num != 5)
			{
				return null;
			}
			for (;;)
			{
				flag = true;
				object value = MUJson.parseValue(json, ref index, ref flag);
				if (!flag)
				{
					break;
				}
				hashtable[text] = value;
				if (!false)
				{
					goto IL_67;
				}
			}
			return null;
			IL_67:
			goto IL_175;
		}
		IL_C1:
		return null;
		IL_10C:
		if (num != 6)
		{
			for (;;)
			{
				while (num != 2)
				{
					if (((flag ? 1U : 0U) & 0U) == 0U)
					{
						goto IL_A1;
					}
					bool flag3 = ((flag2 ? 1U : 0U) | 4U) == 0U;
					if (!flag3)
					{
						if (false)
						{
							goto Block_8;
						}
						if (false)
						{
							goto IL_175;
						}
						if (((flag2 ? 1U : 0U) | 3U) == 0U)
						{
							goto IL_C1;
						}
						if ((uint)num >= 0U)
						{
							goto IL_A1;
						}
					}
				}
				goto IL_FA;
			}
			IL_A1:
			text = MUJson.parseString(json, ref index);
			if ((flag ? 1U : 0U) <= 4294967295U)
			{
				goto IL_BE;
			}
			goto IL_142;
			IL_FA:
			MUJson.nextToken(json, ref index);
			return hashtable;
			Block_8:
			goto IL_14D;
		}
		IL_11B:
		MUJson.nextToken(json, ref index);
		if ((flag ? 1U : 0U) - (flag2 ? 1U : 0U) <= 4294967295U)
		{
			goto IL_1D;
		}
		if (2147483647 != 0)
		{
			goto IL_10C;
		}
		goto IL_146;
		IL_142:
		return null;
		IL_146:
		if (num != 0)
		{
			goto IL_10C;
		}
		goto IL_142;
		IL_14D:
		MUJson.nextToken(json, ref index);
		if (false)
		{
			goto IL_11B;
		}
		flag2 = false;
		IL_175:
		goto IL_1D;
	}

	protected static ArrayList parseArray(char[] json, ref int index)
	{
		ArrayList arrayList = new ArrayList();
		MUJson.nextToken(json, ref index);
		bool flag = false;
		bool flag2;
		int num;
		if ((flag2 ? 1U : 0U) + (uint)num <= 4294967295U)
		{
			if (((uint)num & 0U) != 0U)
			{
				goto IL_B2;
			}
			IL_19:
			while (!flag)
			{
				num = MUJson.lookAhead(json, index);
				if (num == 0)
				{
					return null;
				}
				if (num == 6)
				{
					goto IL_B2;
				}
				if (num == 4)
				{
					MUJson.nextToken(json, ref index);
					break;
				}
				do
				{
					flag2 = true;
				}
				while ((flag ? 1U : 0U) > 4294967295U);
				object value = MUJson.parseValue(json, ref index, ref flag2);
				if (!flag2)
				{
					return null;
				}
				arrayList.Add(value);
			}
			return arrayList;
			IL_B2:
			MUJson.nextToken(json, ref index);
			goto IL_19;
		}
		return arrayList;
	}

	protected static object parseValue(char[] json, ref int index, ref bool success)
	{
		int num = MUJson.lookAhead(json, index);
		while ((uint)num + (uint)num >= 0U)
		{
			switch (num)
			{
			case 1:
				return MUJson.parseObject(json, ref index);
			case 3:
				return MUJson.parseArray(json, ref index);
			case 7:
				return MUJson.parseString(json, ref index);
			case 8:
				return MUJson.parseNumber(json, ref index);
			case 9:
				MUJson.nextToken(json, ref index);
				return bool.Parse("TRUE");
			case 10:
				MUJson.nextToken(json, ref index);
				return bool.Parse("FALSE");
			case 11:
				goto IL_23;
			}
			success = false;
			bool flag = ((uint)num | 2147483648U) == 0U;
			if (!flag)
			{
				return null;
			}
		}
		IL_23:
		MUJson.nextToken(json, ref index);
		return null;
	}

	protected static string parseString(char[] json, ref int index)
	{
		string text = "";
		if (2147483647 == 0)
		{
			goto IL_11E;
		}
		goto IL_2FA;
		IL_37:
		bool flag;
		int num;
		char c;
		int num2;
		bool flag2;
		while (!flag)
		{
			if (index == json.Length)
			{
				break;
			}
			num = index++;
			c = json[num];
			if (c == '"')
			{
				flag = true;
				break;
			}
			int num3;
			while (c == '\\')
			{
				if (index == json.Length)
				{
					goto IL_26;
				}
				if (15 != 0)
				{
				}
				num2 = index++;
				c = json[num2];
				if ((uint)num + (uint)num2 <= 4294967295U)
				{
					if (((uint)num | 4294967295U) == 0U)
					{
						goto IL_125;
					}
					if (c == '"')
					{
						text += '"';
						goto IL_37;
					}
					if (c == '\\')
					{
						text += '\\';
						goto IL_37;
					}
					if (c == '/')
					{
						text += '/';
						goto IL_37;
					}
					if (c == 'b')
					{
						text += '\b';
						goto IL_37;
					}
					if (c == 'f')
					{
						goto IL_1CB;
					}
					if (((uint)num2 & 0U) == 0U && c != 'n')
					{
						goto IL_125;
					}
					text += '\n';
					flag2 = (((uint)num3 & 0U) == 0U);
					if (flag2)
					{
						goto IL_1EB;
					}
					goto IL_112;
				}
			}
			if (2 == 0)
			{
				return text;
			}
			text += c;
			flag2 = ((uint)num3 - (flag ? 1U : 0U) < 0U);
			if (flag2)
			{
				goto IL_15D;
			}
		}
		goto IL_26;
		IL_1F:
		return null;
		IL_26:
		if (flag)
		{
			return text;
		}
		goto IL_1F;
		IL_112:
		char[] array;
		if (c != 't')
		{
			if (c != 'u')
			{
				goto IL_37;
			}
			int num3 = json.Length - index;
			if (num3 < 4)
			{
				goto IL_26;
			}
			array = new char[4];
			Array.Copy(json, index, array, 0, 4);
			goto IL_13D;
		}
		else
		{
			text += '\t';
			if (false)
			{
				goto IL_13D;
			}
			if ((uint)num >= 0U)
			{
				goto IL_37;
			}
		}
		IL_84:
		text = text + "&#x" + new string(array) + ";";
		index += 4;
		goto IL_37;
		IL_13D:
		if ((uint)num2 - (flag ? 1U : 0U) > 4294967295U)
		{
			goto IL_1CB;
		}
		int num4;
		flag2 = ((uint)num4 + (flag ? 1U : 0U) < 0U);
		if (flag2)
		{
			goto IL_15D;
		}
		goto IL_84;
		IL_11E:
		if (15 != 0)
		{
			goto IL_112;
		}
		IL_125:
		if (c != 'r')
		{
			goto IL_112;
		}
		text += '\r';
		goto IL_37;
		IL_15D:
		if (-2 != 0)
		{
			goto IL_1F;
		}
		goto IL_2FA;
		IL_1CB:
		text += '\f';
		flag2 = ((flag ? 1U : 0U) > uint.MaxValue);
		if (!flag2)
		{
			goto IL_37;
		}
		IL_1EB:
		flag2 = ((uint)num + (uint)num4 < 0U);
		if (flag2)
		{
			goto IL_11E;
		}
		goto IL_37;
		IL_2FA:
		MUJson.eatWhitespace(json, ref index);
		num4 = index++;
		c = json[num4];
		flag = false;
		goto IL_37;
	}

	protected static double parseNumber(char[] json, ref int index)
	{
		MUJson.eatWhitespace(json, ref index);
		int lastIndexOfNumber;
		int num;
		if (-2147483648 != 0)
		{
			lastIndexOfNumber = MUJson.getLastIndexOfNumber(json, index);
			num = lastIndexOfNumber - index + 1;
		}
		char[] array = new char[num];
		Array.Copy(json, index, array, 0, num);
		index = lastIndexOfNumber + 1;
		return double.Parse(new string(array));
	}

	protected static int getLastIndexOfNumber(char[] json, int index)
	{
		int i = index;
		while (i < json.Length)
		{
			if ("0123456789+-.eE".IndexOf(json[i]) == -1)
			{
				if ((uint)i <= 4294967295U)
				{
					IL_3A:
					return i - 1;
				}
			}
			else
			{
				i++;
			}
		}
		goto IL_3A;
	}

	protected static void eatWhitespace(char[] json, ref int index)
	{
		while (index < json.Length)
		{
			if (" \t\n\r".IndexOf(json[index]) == -1)
			{
				return;
			}
			index++;
		}
	}

	protected static int lookAhead(char[] json, int index)
	{
		int num = index;
		return MUJson.nextToken(json, ref num);
	}

	protected static int nextToken(char[] json, ref int index)
	{
		MUJson.eatWhitespace(json, ref index);
		while (index != json.Length)
		{
			char c = json[index];
			if (255 != 0)
			{
				index++;
				int num;
				if ((uint)num + (uint)num >= 0U)
				{
					char c2 = c;
					bool flag = ((uint)num & 0U) == 0U;
					if (flag)
					{
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
							IL_1EB:
							index--;
							num = json.Length - index;
							if (num >= 5)
							{
								if (json[index] != 'f')
								{
									if ((uint)num >= 0U)
									{
										if (!false)
										{
											goto IL_117;
										}
									}
									else
									{
										if ((uint)num - (uint)num <= 4294967295U)
										{
											goto IL_13F;
										}
										break;
									}
								}
								else
								{
									if (json[index + 1] == 'a')
									{
										while ((uint)num + (uint)num >= 0U)
										{
											if (json[index + 2] != 'l')
											{
												goto IL_117;
											}
											if (json[index + 3] != 's')
											{
												if (!false)
												{
													goto IL_117;
												}
											}
											else
											{
												if (json[index + 4] == 'e')
												{
													goto IL_1C2;
												}
												goto IL_117;
											}
										}
										goto IL_1E;
									}
									goto IL_117;
								}
								IL_1C2:
								index += 5;
								return 10;
							}
							goto IL_117;
							IL_2F:
							if (json[index + 2] != 'l')
							{
								return 0;
							}
							flag = ((uint)num - (uint)num < 0U);
							if (flag)
							{
								goto IL_51;
							}
							if (false)
							{
								return 8;
							}
							if (json[index + 3] == 'l')
							{
								index += 4;
								return 11;
							}
							flag = ((uint)num - (uint)num > uint.MaxValue);
							if (!flag)
							{
								return 0;
							}
							if (-1 == 0)
							{
								break;
							}
							continue;
							IL_1E:
							if (json[index + 1] == 'u')
							{
								goto IL_2F;
							}
							return 0;
							IL_51:
							if (-2 == 0)
							{
								goto IL_1E;
							}
							goto IL_2F;
							IL_7A:
							if (num < 4)
							{
								return 0;
							}
							if (false)
							{
								return 0;
							}
							if (json[index] != 'n')
							{
								return 0;
							}
							if (((uint)num & 0U) != 0U)
							{
								goto IL_117;
							}
							if (2 == 0)
							{
								goto IL_51;
							}
							goto IL_1E;
							IL_CA:
							if (json[index + 2] != 'u')
							{
								goto IL_7A;
							}
							if (json[index + 3] == 'e')
							{
								index += 4;
								return 9;
							}
							goto IL_7A;
							IL_AE:
							if (((uint)num | 255U) != 0U)
							{
								goto IL_7A;
							}
							goto IL_CA;
							IL_117:
							if (num < 4)
							{
								goto IL_7A;
							}
							if (json[index] == 't')
							{
								if (json[index + 1] != 'r')
								{
									goto IL_7A;
								}
								flag = ((uint)num > uint.MaxValue);
								if (!flag)
								{
									goto IL_CA;
								}
							}
							IL_13F:
							goto IL_AE;
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
								continue;
							}
							break;
						}
						switch (c2)
						{
						case '[':
							return 3;
						case '\\':
							goto IL_1EB;
						case ']':
							return 4;
						default:
							switch (c2)
							{
							case '{':
								return 1;
							case '|':
								goto IL_1EB;
							case '}':
								return 2;
							default:
								goto IL_1EB;
							}
							break;
						}
						return 8;
					}
					return 10;
				}
				return 1;
			}
			return 0;
		}
		return 0;
	}

	protected static bool serializeObjectOrArray(object objectOrArray, StringBuilder builder)
	{
		if (objectOrArray is Hashtable)
		{
			return MUJson.serializeObject((Hashtable)objectOrArray, builder);
		}
		return objectOrArray is ArrayList && MUJson.serializeArray((ArrayList)objectOrArray, builder);
	}

	protected static bool serializeObject(Hashtable anObject, StringBuilder builder)
	{
		builder.Append("{");
		if (!false)
		{
			goto IL_D4;
		}
		IL_12:
		object value;
		if (!MUJson.serializeValue(value, builder))
		{
			return false;
		}
		IL_1B:
		bool flag = false;
		IL_1D:
		IDictionaryEnumerator enumerator;
		if (!enumerator.MoveNext())
		{
			builder.Append("}");
			return true;
		}
		string aString = enumerator.Key.ToString();
		value = enumerator.Value;
		bool flag2 = ((flag ? 1U : 0U) & 0U) == 0U;
		if (flag2)
		{
			flag2 = ((flag ? 1U : 0U) + (flag ? 1U : 0U) < 0U);
			if (flag2 || !flag)
			{
				builder.Append(", ");
			}
			MUJson.serializeString(aString, builder);
		}
		builder.Append(":");
		flag2 = ((flag ? 1U : 0U) < 0U);
		if (!flag2)
		{
			goto IL_12;
		}
		flag2 = ((flag ? 1U : 0U) < 0U);
		if (!flag2)
		{
			goto IL_12;
		}
		IL_D4:
		if (((flag ? 1U : 0U) | 2U) != 0U)
		{
			enumerator = anObject.GetEnumerator();
			flag = true;
			goto IL_1D;
		}
		goto IL_1B;
	}

	protected static bool serializeDictionary(Dictionary<string, string> dict, StringBuilder builder)
	{
		builder.Append("{");
		bool flag = true;
		foreach (KeyValuePair<string, string> keyValuePair in dict)
		{
			if (!flag)
			{
				builder.Append(", ");
			}
			MUJson.serializeString(keyValuePair.Key, builder);
			if ((flag ? 1U : 0U) - (flag ? 1U : 0U) <= 4294967295U)
			{
				builder.Append(":");
				MUJson.serializeString(keyValuePair.Value, builder);
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected static bool serializeArray(ArrayList anArray, StringBuilder builder)
	{
		builder.Append("[");
		bool flag2;
		bool flag = (flag2 ? 1U : 0U) + (flag2 ? 1U : 0U) < 0U;
		int num;
		if (!flag)
		{
			if (false)
			{
				goto IL_48;
			}
			flag2 = true;
			do
			{
				num = 0;
			}
			while ((flag2 ? 1U : 0U) - (uint)num > 4294967295U);
		}
		IL_27:
		if (num >= anArray.Count)
		{
			builder.Append("]");
			return true;
		}
		object value = anArray[num];
		while (!flag2)
		{
			builder.Append(", ");
			flag = ((uint)num - (uint)num < 0U);
			if (flag)
			{
				flag = (((flag2 ? 1U : 0U) | 4294967294U) == 0U);
				if (flag)
				{
					goto IL_E5;
				}
			}
			else if (((flag2 ? 1U : 0U) & 0U) != 0U)
			{
				continue;
			}
			if (4 != 0)
			{
				break;
			}
		}
		IL_48:
		if (!MUJson.serializeValue(value, builder))
		{
			return false;
		}
		flag2 = false;
		num++;
		IL_E5:
		goto IL_27;
	}

	protected static bool serializeValue(object value, StringBuilder builder)
	{
		if (value != null)
		{
			if (value.GetType().IsArray)
			{
				MUJson.serializeArray(new ArrayList((ICollection)value), builder);
				return true;
			}
			if (value is string)
			{
				MUJson.serializeString((string)value, builder);
				if (-1 != 0)
				{
					return true;
				}
			}
			if (3 != 0)
			{
				if (value is char)
				{
					if (!false)
					{
						MUJson.serializeString(Convert.ToString((char)value), builder);
						return true;
					}
				}
				else if (value is Hashtable)
				{
					MUJson.serializeObject((Hashtable)value, builder);
					return true;
				}
			}
			if (!false)
			{
				if (value is Dictionary<string, string>)
				{
					MUJson.serializeDictionary((Dictionary<string, string>)value, builder);
					return true;
				}
				if (!(value is ArrayList))
				{
					if (8 != 0)
					{
						if (value is bool)
						{
							if ((bool)value)
							{
								builder.Append("true");
								goto IL_7B;
							}
						}
						if ((!(value is bool) && 255 != 0) || (bool)value)
						{
							if (!value.GetType().IsPrimitive)
							{
								return false;
							}
							MUJson.serializeNumber(Convert.ToDouble(value), builder);
							if (!false)
							{
							}
						}
						else
						{
							builder.Append("false");
						}
					}
					IL_7B:
					return true;
				}
				MUJson.serializeArray((ArrayList)value, builder);
				return true;
			}
		}
		builder.Append("null");
		return true;
	}

	protected static void serializeString(string aString, StringBuilder builder)
	{
		builder.Append("\"");
		char[] array = aString.ToCharArray();
		int num;
		if (3 != 0)
		{
			num = 0;
			goto IL_23;
		}
		IL_1F:
		num++;
		IL_23:
		if (num >= array.Length)
		{
			builder.Append("\"");
		}
		else
		{
			char c = array[num];
			if (c == '"')
			{
				builder.Append("\\\"");
				goto IL_1BE;
			}
			if (c == '\\')
			{
				builder.Append("\\\\");
				goto IL_1F;
			}
			if (c == '\b')
			{
				builder.Append("\\b");
				if (!false)
				{
					goto IL_180;
				}
			}
			if (c != '\f')
			{
				if (c != '\n')
				{
					if (((uint)num | 3U) != 0U)
					{
						if (c != '\r')
						{
							int num2;
							if (c != '\t')
							{
								bool flag = (uint)num2 + (uint)num > uint.MaxValue;
								if (flag)
								{
									return;
								}
								num2 = Convert.ToInt32(c);
								if (num2 >= 32)
								{
									goto IL_74;
								}
							}
							else
							{
								builder.Append("\\t");
								if ((uint)num + (uint)num >= 0U)
								{
									if ((uint)num + (uint)num >= 0U)
									{
										goto IL_1F;
									}
									goto IL_74;
								}
								else
								{
									if (((uint)num | 255U) == 0U)
									{
										goto IL_10B;
									}
									if ((uint)num + (uint)num2 <= 4294967295U)
									{
										goto IL_FF;
									}
									goto IL_180;
								}
							}
							IL_2E:
							builder.Append("\\u" + Convert.ToString(num2, 16).PadLeft(4, '0'));
							goto IL_1F;
							IL_74:
							if (num2 > 126)
							{
								goto IL_2E;
							}
							builder.Append(c);
							goto IL_59;
						}
						IL_FF:
						builder.Append("\\r");
						IL_10B:;
					}
					IL_59:;
				}
				else
				{
					builder.Append("\\n");
				}
			}
			else
			{
				builder.Append("\\f");
			}
			IL_180:
			goto IL_1F;
		}
		return;
		IL_1BE:
		goto IL_1F;
	}

	protected static void serializeNumber(double number, StringBuilder builder)
	{
		builder.Append(Convert.ToString(number));
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

	protected static int lastErrorIndex = -1;

	protected static string lastDecode = "";
}
