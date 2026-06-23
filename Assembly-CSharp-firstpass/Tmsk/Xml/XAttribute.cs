using System;
using System.Collections.Generic;
using System.Text;

namespace Tmsk.Xml
{
	public class XAttribute
	{
		public XAttribute()
		{
		}

		public XAttribute(string name, string value)
		{
			this.name = name;
			this.value = value;
		}

		public XAttribute(XAttribute other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			this.name = other.name;
			this.value = other.value;
		}

		public static IEnumerable<XAttribute> EmptySequence
		{
			get
			{
				return XAttribute.empty_array;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Value
		{
			get
			{
				return this.value;
			}
		}

		public XAttribute NextAttribute
		{
			get
			{
				return this.next;
			}
			internal set
			{
				this.next = value;
			}
		}

		public XAttribute PreviousAttribute
		{
			get
			{
				return this.previous;
			}
			internal set
			{
				this.previous = value;
			}
		}

		public void Remove()
		{
			if (this.next != null)
			{
				this.next.previous = this.previous;
			}
			if (this.previous != null)
			{
				this.previous.next = this.next;
			}
			this.next = null;
			this.previous = null;
		}

		public void GetString(StringBuilder sb)
		{
			sb.Append(" ");
			sb.Append(this.name.ToString());
			sb.Append("=\"");
			int num = 0;
			int i = this.value.IndexOfAny(XAttribute.escapeChars, num);
			if (i < 0)
			{
				sb.Append(this.value);
				sb.Append("\"");
				return;
			}
			while (i >= 0)
			{
				sb.Append(this.value, num, i - num);
				char c = this.value.get_Chars(i);
				switch (c)
				{
				case '\t':
					sb.Append("&#x9;");
					break;
				case '\n':
					sb.Append("&#xA;");
					break;
				default:
					switch (c)
					{
					case '<':
						sb.Append("&lt;");
						break;
					default:
						if (c != '"')
						{
							if (c == '&')
							{
								sb.Append("&amp;");
							}
						}
						else
						{
							sb.Append("&quot;");
						}
						break;
					case '>':
						sb.Append("&gt;");
						break;
					}
					break;
				case '\r':
					sb.Append("&#xD;");
					break;
				}
				num = i + 1;
				i = this.value.IndexOfAny(XAttribute.escapeChars, num);
			}
			if (i < 0)
			{
				if (num > 0)
				{
					sb.Append(this.value, num, this.value.Length - num);
				}
				else
				{
					sb.Append(this.value);
				}
				sb.Append("\"");
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.GetString(stringBuilder);
			return stringBuilder.ToString();
		}

		public static explicit operator bool(XAttribute att)
		{
			if (att == null || att.value == null)
			{
				throw new ArgumentNullException("attribute");
			}
			string text = att.value;
			text = text.Trim(XAttribute.whitespaceChars);
			string text2 = text;
			if (text2 != null)
			{
				if (XAttribute.<>f__switch$map8 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
					dictionary.Add("1", 0);
					dictionary.Add("true", 1);
					dictionary.Add("0", 2);
					dictionary.Add("false", 3);
					XAttribute.<>f__switch$map8 = dictionary;
				}
				int num;
				if (XAttribute.<>f__switch$map8.TryGetValue(text2, ref num))
				{
					switch (num)
					{
					case 0:
						return true;
					case 1:
						return true;
					case 2:
						return false;
					case 3:
						return false;
					}
				}
			}
			throw new FormatException(text + " is not a valid boolean value");
		}

		public static explicit operator string(XAttribute attribute)
		{
			if (attribute == null)
			{
				return null;
			}
			return attribute.value;
		}

		private static readonly XAttribute[] empty_array = new XAttribute[0];

		private static readonly char[] escapeChars = new char[]
		{
			'<',
			'>',
			'&',
			'"',
			'\r',
			'\n',
			'\t'
		};

		private static readonly char[] whitespaceChars = new char[]
		{
			' ',
			'\r',
			'\n',
			'\t'
		};

		public XAttribute next;

		public XAttribute previous;

		public string name = string.Empty;

		public string value = string.Empty;
	}
}
