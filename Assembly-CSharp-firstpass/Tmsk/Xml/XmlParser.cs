using System;
using System.Collections.Generic;
using System.Text;

namespace Tmsk.Xml
{
	internal class XmlParser
	{
		private static int SkipSpace(string content, int start, bool withRN = false)
		{
			int i = start;
			int length = content.Length;
			while (i < length)
			{
				char c = content.get_Chars(i);
				if (c != ' ' && c != '\t')
				{
					if (!withRN || (c != '\r' && c != '\n'))
					{
						break;
					}
				}
				i++;
			}
			return i - start;
		}

		private static string ReplaceSpecMark(string str, List<int> marks)
		{
			if (str == null || marks == null || marks.Count <= 0)
			{
				return str;
			}
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = 0; i < marks.Count; i++)
			{
				int num2 = marks[i];
				if (str.get_Chars(num2) == '&')
				{
					int num3 = -1;
					for (int j = 1; j < 6; j++)
					{
						if (str.get_Chars(num2 + j) == ';')
						{
							num3 = j;
							break;
						}
					}
					if (num3 != -1)
					{
						if (num2 > num)
						{
							stringBuilder.Append(str, num, num2 - num);
						}
						string text = str.Substring(num2, num3 + 1);
						if (text == "&amp;")
						{
							stringBuilder.Append('&');
							flag = true;
						}
						else if (text == "&lt;")
						{
							stringBuilder.Append('<');
							flag = true;
						}
						else if (text == "&gt;")
						{
							stringBuilder.Append('>');
							flag = true;
						}
						else if (text == "&quot;")
						{
							stringBuilder.Append('"');
							flag = true;
						}
						else if (text == "&#xD;")
						{
							stringBuilder.Append('\r');
							flag = true;
						}
						else if (text == "&#xA;")
						{
							stringBuilder.Append('\n');
							flag = true;
						}
						else if (text == "&#x9;")
						{
							stringBuilder.Append('\t');
							flag = true;
						}
						else
						{
							stringBuilder.Append(text);
						}
						num = num2 + num3 + 1;
					}
				}
			}
			if (flag)
			{
				if (num + 1 < str.Length)
				{
					stringBuilder.Append(str, num, str.Length - num);
				}
				return stringBuilder.ToString();
			}
			return str;
		}

		private static string ReplaceSpecMark(string str)
		{
			if (str == null)
			{
				return str;
			}
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			for (int i = str.IndexOf('&', num); i >= 0; i = str.IndexOf('&', num))
			{
				stringBuilder.Append(str, num, i - num);
				num = i;
				int num2 = str.IndexOf(';', i);
				if (num2 <= i)
				{
					break;
				}
				string text = str.Substring(i, num2 - i + 1);
				if (text == "&amp;")
				{
					stringBuilder.Append('&');
					flag = true;
				}
				else if (text == "&lt;")
				{
					stringBuilder.Append('<');
					flag = true;
				}
				else if (text == "&gt;")
				{
					stringBuilder.Append('>');
					flag = true;
				}
				else if (text == "&quot;")
				{
					stringBuilder.Append('"');
					flag = true;
				}
				else if (text == "&#xD;")
				{
					stringBuilder.Append('\r');
					flag = true;
				}
				else if (text == "&#xA;")
				{
					stringBuilder.Append('\n');
					flag = true;
				}
				else if (text == "&#x9;")
				{
					stringBuilder.Append('\t');
					flag = true;
				}
				else
				{
					try
					{
						if (text.StartsWith("&#x"))
						{
							int num3 = int.Parse(text.Substring(3, text.Length - 3 - 1), 512);
							stringBuilder.Append((char)num3);
							flag = true;
						}
						else if (text.StartsWith("&#"))
						{
							int num4 = int.Parse(text.Substring(3, text.Length - 3 - 1));
							stringBuilder.Append((char)num4);
							flag = true;
						}
						else
						{
							stringBuilder.Append(text);
						}
					}
					catch
					{
						stringBuilder.Append(text);
					}
				}
				num = num2 + 1;
			}
			if (flag)
			{
				if (num + 1 < str.Length)
				{
					stringBuilder.Append(str, num, str.Length - num);
				}
				return stringBuilder.ToString();
			}
			return str;
		}

		private static string ParseAttName(XParserContext context, char[] content, int start, int total, ref int offset)
		{
			string result = string.Empty;
			int num = -1;
			int i = start;
			bool flag = true;
			while (flag)
			{
				char c = content[i];
				char c2 = c;
				switch (c2)
				{
				case '\t':
				case '\n':
				case '\r':
					break;
				default:
					if (c2 != ' ')
					{
						flag = false;
						continue;
					}
					break;
				}
				i++;
			}
			while (i < total)
			{
				char c3 = content[i];
				char c2 = c3;
				if (c2 == '=' || c2 == '>' || c2 == '\t' || c2 == ' ')
				{
					break;
				}
				if (c2 != '/')
				{
					if (num == -1)
					{
						num = i;
					}
				}
				else
				{
					char c4 = (i + 1 >= total) ? '~' : content[i + 1];
					if (c4 == '>')
					{
						break;
					}
				}
				i++;
			}
			int num2 = i - num;
			if (num > -1 && num2 >= 1)
			{
				string text = context.AddString(content, num, num2);
				if (text != null)
				{
					result = text;
				}
				else
				{
					result = new string(content, num, num2);
				}
			}
			offset = i - start;
			return result;
		}

		private static string ParseAttValue(XParserContext context, char[] content, int start, int total, ref int offset)
		{
			string text = string.Empty;
			int num = -1;
			int i = start;
			bool flag = false;
			bool flag2 = false;
			while (i < total)
			{
				char c = content[i];
				if (c == '&' && flag && !flag2)
				{
					for (int j = 1; j < 9; j++)
					{
						if (i + j < total && content[i + j] == ';')
						{
							flag2 = true;
						}
					}
				}
				if (c == '"' || c == '\'')
				{
					if (flag)
					{
						int num2 = i - num;
						if (num > -1 && num2 >= 1)
						{
							if (num2 < 3)
							{
								string text2 = context.AddString(content, num, num2);
								if (text2 != null)
								{
									text = text2;
									break;
								}
							}
							text = new string(content, num, num2);
						}
						break;
					}
					flag = true;
				}
				else if (flag)
				{
					if (num == -1)
					{
						num = i;
					}
				}
				else
				{
					char c2 = c;
					switch (c2)
					{
					case '<':
					case '>':
						throw new Exception(string.Format("parse error [>,/>], pos=>{0}", i));
					default:
						if (c2 == '/')
						{
							char c3 = (i + 1 >= total) ? '~' : content[i + 1];
							if (c3 == '>')
							{
								throw new Exception(string.Format("parse error [>,/>], pos=>{0}", i));
							}
						}
						break;
					}
				}
				i++;
			}
			offset = i - start;
			if (flag2)
			{
				text = XmlParser.ReplaceSpecMark(text);
			}
			return text;
		}

		private static int ParseAttribute(XParserContext context, XElement xmlElement, char[] content, int start)
		{
			int num = content.Length;
			int i;
			for (i = start; i < num; i++)
			{
				char c = content[i];
				char c2 = '~';
				if (i + 1 < num)
				{
					c2 = content[i + 1];
				}
				if (c == '>' || (c == '/' && c2 == '>'))
				{
					break;
				}
				int num2 = 0;
				string text = XmlParser.ParseAttName(context, content, i, num, ref num2);
				i += num2;
				if (text.Equals(string.Empty))
				{
					break;
				}
				bool flag = true;
				while (flag)
				{
					char c3 = content[i];
					char c4 = c3;
					switch (c4)
					{
					case '\t':
					case '\n':
					case '\r':
						break;
					default:
						if (c4 != ' ')
						{
							flag = false;
							continue;
						}
						break;
					}
					i++;
				}
				if (i >= num)
				{
					break;
				}
				if (content[i] != '=')
				{
					throw new Exception(string.Format("need =, but is {0}, pos=>{1}", content[i], i));
				}
				i++;
				num2 = 0;
				string value = XmlParser.ParseAttValue(context, content, i, num, ref num2);
				i += num2;
				xmlElement.AppendAttribute(text, value);
			}
			return i - start;
		}

		private static XElement ParseElement(XParserContext context, char[] content, int start, int total, ref int offset, bool noRet = false)
		{
			XElement xelement = null;
			if (!noRet)
			{
				xelement = new XElement();
			}
			string text = string.Empty;
			int num = -1;
			int i;
			for (i = start; i < total; i++)
			{
				char c = content[i];
				char c2 = '~';
				if (i + 1 < total)
				{
					c2 = content[i + 1];
				}
				if (c == '>' || (c == '/' && c2 == '>'))
				{
					if (text.Equals(string.Empty))
					{
						int num2 = i - num;
						if (num != -1 && num2 >= 1)
						{
							string text2 = context.AddString(content, num, num2);
							if (text2 != null)
							{
								text = text2;
							}
							else
							{
								text = new string(content, num, num2);
							}
						}
					}
					break;
				}
				if (c == ' ' || c == '\t')
				{
					int num3 = i - num;
					if (num != -1 && num3 >= 1)
					{
						string text3 = context.AddString(content, num, num3);
						if (text3 != null)
						{
							text = text3;
						}
						else
						{
							text = new string(content, num, num3);
						}
					}
					xelement.Name = text;
					i += XmlParser.ParseAttribute(context, xelement, content, i);
					i--;
				}
				else if (num == -1)
				{
					num = i;
				}
			}
			if (xelement != null)
			{
				xelement.Name = text;
			}
			offset = i - start;
			return xelement;
		}

		private static int SkipMeta(char[] content, int start, int total)
		{
			int i;
			for (i = start; i < total; i++)
			{
				char c = content[i];
				char c2 = '~';
				if (i + 1 < total)
				{
					c2 = content[i + 1];
				}
				if (c == '?' && c2 == '>')
				{
					i += 2;
					break;
				}
			}
			return i - start;
		}

		private static int SkipComment(char[] content, int start, int total)
		{
			int i = start;
			char c = content[i];
			char c2 = '~';
			if (i + 1 < total)
			{
				c2 = content[i + 1];
			}
			if (c == '-' && c2 == '-')
			{
				for (i += 2; i < total; i++)
				{
					char c3 = content[i];
					char c4 = '~';
					char c5 = '~';
					if (i + 1 < total)
					{
						c4 = content[i + 1];
					}
					if (i + 2 < total)
					{
						c5 = content[i + 2];
					}
					if (c3 == '-' && c4 == '-' && c5 == '>')
					{
						i += 3;
						break;
					}
				}
			}
			else
			{
				string text = string.Empty;
				if (i + 7 < total)
				{
					text = new string(content, i, 7);
				}
				if (text == "[CDATA[")
				{
					for (i += 7; i < total; i++)
					{
						char c6 = content[i];
						char c7 = '~';
						char c8 = '~';
						if (i + 1 < total)
						{
							c7 = content[i + 1];
						}
						if (i + 1 < total)
						{
							c8 = content[i + 2];
						}
						if (c6 == ']' && c7 == ']' && c8 == '>')
						{
							i += 3;
							break;
						}
					}
				}
			}
			return i - start;
		}

		public static XElement Parse(string strContent)
		{
			XElement xelement = new XElement("root");
			XElement xelement2 = xelement;
			XParserContext context = new XParserContext();
			List<XElement> list = new List<XElement>();
			char[] array = strContent.ToCharArray();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				char c = array[i];
				char c2 = '~';
				if (i + 1 < num)
				{
					c2 = array[i + 1];
				}
				if (c == '<' && c2 == '?')
				{
					i += 2;
					i += XmlParser.SkipMeta(array, i, num);
					i--;
				}
				else if (c == '<' && c2 == '!')
				{
					i += 2;
					i += XmlParser.SkipComment(array, i, num);
					i--;
				}
				else if (c == '<' && c2 == '/')
				{
					int num2 = 0;
					i++;
					XmlParser.ParseElement(context, array, i, num, ref num2, true);
					i += num2;
					XElement xelement3 = list[list.Count - 1];
					list.RemoveAt(list.Count - 1);
					xelement2 = xelement3;
				}
				else if (c == '<')
				{
					int num3 = 0;
					i++;
					XElement xelement4 = XmlParser.ParseElement(context, array, i, num, ref num3, false);
					i += num3;
					if (i >= num)
					{
						break;
					}
					char c3 = array[i];
					char c4 = (i + 1 >= num) ? '~' : array[i + 1];
					if (c3 == '/' && c4 == '>')
					{
						i++;
						if (xelement2 != null)
						{
							xelement2.Add(xelement4);
						}
					}
					else if (c3 == '>')
					{
						if (xelement == null)
						{
							xelement = xelement4;
						}
						if (xelement2 != null)
						{
							xelement2.Add(xelement4);
						}
						else
						{
							xelement2 = xelement4;
						}
						list.Add(xelement2);
						xelement2 = xelement4;
					}
				}
			}
			if (xelement.Children.Count == 1)
			{
				return (XElement)xelement.Children[0];
			}
			return xelement;
		}

		private const char LT = '<';

		private const char GT = '>';

		private const char SPACE = ' ';

		private const char QUOTE = '"';

		private const char QUOTE2 = '\'';

		private const char SLASH = '/';

		private const char QMARK = '?';

		private const char EQUALS = '=';

		private const char EXCLAMATION = '!';

		private const char DASH = '-';

		private const char SQR = ']';

		private const char AMP = '&';
	}
}
