using System;

namespace HTMLEngine.Core
{
	internal class HtmlChunkWord : HtmlChunk
	{
		public bool ReadWord(Reader reader)
		{
			reader.AutoSkipWhitespace = false;
			this.Text = reader.ReadToWhitespaceOrChar('<');
			if (!string.IsNullOrEmpty(this.Text))
			{
				this.Text = this.Text.Replace("&nbsp;", " ").Replace("&gt;", ">").Replace("&lt;", "<");
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Format("WORD:" + this.Text, new object[0]);
		}

		public string Text;
	}
}
