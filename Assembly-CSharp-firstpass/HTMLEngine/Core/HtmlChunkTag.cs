using System;
using System.Collections.Generic;

namespace HTMLEngine.Core
{
	internal class HtmlChunkTag : HtmlChunk
	{
		internal override void OnRelease()
		{
			this.Attrs.Clear();
			base.OnRelease();
		}

		public string GetAttr(string attrName)
		{
			string text;
			return (!this.Attrs.TryGetValue(attrName, ref text)) ? null : text;
		}

		public bool ReadTag(Reader reader)
		{
			reader.AutoSkipWhitespace = true;
			reader.SkipWhitespace();
			if (!reader.IsOnChar('<', false))
			{
				return false;
			}
			reader.Skip(1);
			this.IsClosing = false;
			if (reader.CurrChar == '/')
			{
				this.IsClosing = true;
				reader.Skip(1);
			}
			this.Tag = reader.ReadToStopChar(HtmlChunkTag.TAG_NAME_STOP_CHARS, false);
			while (reader.IsOnLetter())
			{
				string text = reader.ReadToStopChar(HtmlChunkTag.ATTR_NAME_STOP_CHARS, false);
				reader.ReadToStopChar('=', false);
				reader.Skip(1);
				string text2 = (!reader.IsOnQuote()) ? reader.ReadToStopChar(HtmlChunkTag.ATTR_VALUE_STOP_CHARS, false) : reader.ReadQuotedString();
				this.Attrs[text] = text2;
				reader.SkipWhitespace();
			}
			string tag = this.Tag;
			if (tag != null)
			{
				if (HtmlChunkTag.<>f__switch$map5 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
					dictionary.Add("br", 0);
					dictionary.Add("hr", 0);
					dictionary.Add("img", 0);
					dictionary.Add("meta", 0);
					HtmlChunkTag.<>f__switch$map5 = dictionary;
				}
				int num;
				if (HtmlChunkTag.<>f__switch$map5.TryGetValue(tag, ref num))
				{
					if (num == 0)
					{
						this.IsSingle = true;
						goto IL_151;
					}
				}
			}
			this.IsSingle = (reader.CurrChar == '/');
			IL_151:
			reader.ReadToStopChar('>', false);
			if (reader.CurrChar != '>')
			{
				return false;
			}
			reader.Skip(1);
			return this.IsTagSupported();
		}

		private bool IsTagSupported()
		{
			string tag = this.Tag;
			if (tag != null)
			{
				if (HtmlChunkTag.<>f__switch$map6 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
					dictionary.Add("a", 0);
					dictionary.Add("img", 0);
					dictionary.Add("p", 0);
					dictionary.Add("spin", 0);
					dictionary.Add("br", 0);
					dictionary.Add("font", 0);
					dictionary.Add("code", 0);
					dictionary.Add("b", 0);
					dictionary.Add("i", 0);
					dictionary.Add("u", 0);
					dictionary.Add("s", 0);
					dictionary.Add("strike", 0);
					dictionary.Add("effect", 0);
					HtmlChunkTag.<>f__switch$map6 = dictionary;
				}
				int num;
				if (HtmlChunkTag.<>f__switch$map6.TryGetValue(tag, ref num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			HtEngine.Log(HtLogLevel.Warning, "Ignoring unsupported tag: " + this.Tag, new object[0]);
			return false;
		}

		public override string ToString()
		{
			return string.Format("<{0}>", (!this.IsClosing) ? this.Tag : ("/" + this.Tag));
		}

		private static readonly char[] TAG_NAME_STOP_CHARS = new char[]
		{
			' ',
			'/',
			'>'
		};

		private static readonly char[] ATTR_NAME_STOP_CHARS = new char[]
		{
			' ',
			'='
		};

		private static readonly char[] ATTR_VALUE_STOP_CHARS = new char[]
		{
			' ',
			'/',
			'>'
		};

		private readonly Dictionary<string, string> Attrs = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		public bool IsClosing;

		public bool IsSingle;

		public string Tag;
	}
}
