using System;

namespace HTMLEngine.Core
{
	internal class HtmlChunkCollection : PList<HtmlChunk>
	{
		internal override void OnRelease()
		{
			this.Clear(true);
			base.OnRelease();
		}

		public void Clear(bool releaseItems = true)
		{
			if (releaseItems)
			{
				foreach (HtmlChunk htmlChunk in this.list)
				{
					htmlChunk.Dispose();
				}
			}
			this.list.Clear();
		}

		public void Read(Reader reader)
		{
			this.Clear(true);
			while (!reader.IsEof)
			{
				reader.SkipWhitespace();
				if (reader.IsOnChar('<', false))
				{
					HtmlChunkTag htmlChunkTag = OP<HtmlChunkTag>.Acquire();
					if (!htmlChunkTag.ReadTag(reader))
					{
						htmlChunkTag.Dispose();
					}
					else
					{
						base.Add(htmlChunkTag);
					}
				}
				else
				{
					HtmlChunkWord htmlChunkWord = OP<HtmlChunkWord>.Acquire();
					if (!htmlChunkWord.ReadWord(reader))
					{
						htmlChunkWord.Dispose();
						return;
					}
					base.Add(htmlChunkWord);
				}
			}
		}
	}
}
