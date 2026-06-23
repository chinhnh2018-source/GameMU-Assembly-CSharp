using System;
using System.Collections.Generic;

namespace HTMLEngine.Core
{
	internal class DeviceChunkLine : PoolableObject
	{
		internal override void OnAcquire()
		{
			this.Y = 0;
			this.MaxWidth = 0;
			this.Width = 0;
			this.Height = 0;
		}

		internal override void OnRelease()
		{
			this.Clear(true);
		}

		public void Clear(bool releaseItems = true)
		{
			if (releaseItems)
			{
				foreach (DeviceChunk deviceChunk in this.list)
				{
					deviceChunk.Dispose();
				}
			}
			this.list.Clear();
		}

		public List<DeviceChunk> Chunks
		{
			get
			{
				return this.list;
			}
		}

		public int AvailWidth
		{
			get
			{
				DeviceChunk deviceChunk = (this.list.Count <= 0) ? null : this.list[this.list.Count - 1];
				int num = (deviceChunk != null) ? (deviceChunk.Rect.Right + deviceChunk.Font.WhiteSize) : 0;
				return this.MaxWidth - num;
			}
		}

		public int GetRemainWidth()
		{
			DeviceChunk deviceChunk = (this.list.Count <= 0) ? null : this.list[this.list.Count - 1];
			int num = (deviceChunk != null) ? (deviceChunk.Rect.Right + deviceChunk.Font.WhiteSize) : 0;
			return this.MaxWidth - num;
		}

		public bool AddChunk(DeviceChunk chunk, bool prevIsWord)
		{
			DeviceChunk deviceChunk = (this.list.Count <= 0) ? null : this.list[this.list.Count - 1];
			int num = (deviceChunk != null) ? (deviceChunk.Rect.Right + deviceChunk.Font.WhiteSize) : 0;
			if (num + chunk.Rect.Width > this.MaxWidth + 15)
			{
				return false;
			}
			if (deviceChunk != null && prevIsWord)
			{
				deviceChunk.ExtraSpace = deviceChunk.Font.WhiteSize;
				this.Width += deviceChunk.ExtraSpace;
			}
			chunk.Rect.X = num;
			chunk.Rect.Y = this.Y;
			chunk.ExtraSpace = 0;
			this.Width += chunk.Rect.Width;
			if (chunk.Rect.Height > this.Height)
			{
				this.Height = chunk.Rect.Height;
			}
			this.list.Add(chunk);
			return true;
		}

		public void HorzAlign(TextAlign align)
		{
			if (align == TextAlign.Justify && (!this.IsFull || this.list.Count < 2 || this.MaxWidth - this.Width <= 0))
			{
				align = TextAlign.Left;
			}
			switch (align)
			{
			case TextAlign.Left:
			{
				int num = 0;
				for (int i = 0; i < this.list.Count; i++)
				{
					DeviceChunk deviceChunk = this.list[i];
					deviceChunk.Rect.X = num;
					num += deviceChunk.TotalWidth;
				}
				break;
			}
			case TextAlign.Right:
			{
				int num = this.MaxWidth - this.Width;
				for (int j = 0; j < this.list.Count; j++)
				{
					DeviceChunk deviceChunk2 = this.list[j];
					deviceChunk2.Rect.X = num;
					num += deviceChunk2.TotalWidth;
				}
				break;
			}
			case TextAlign.Center:
			{
				int num = (this.MaxWidth - this.Width) / 2;
				for (int k = 0; k < this.list.Count; k++)
				{
					DeviceChunk deviceChunk3 = this.list[k];
					deviceChunk3.Rect.X = num;
					num += deviceChunk3.TotalWidth;
				}
				break;
			}
			case TextAlign.Justify:
			{
				float num2 = (float)(this.MaxWidth - this.Width) / (float)(this.list.Count - 1);
				float num3 = 0f;
				for (int l = 0; l < this.list.Count; l++)
				{
					DeviceChunk deviceChunk4 = this.list[l];
					deviceChunk4.Rect.X = (int)num3;
					num3 += (float)deviceChunk4.TotalWidth;
					num3 += num2;
				}
				break;
			}
			}
		}

		public void VertAlign(VertAlign align)
		{
			switch (align)
			{
			case HTMLEngine.Core.VertAlign.Top:
				for (int i = 0; i < this.list.Count; i++)
				{
					DeviceChunk deviceChunk = this.list[i];
					deviceChunk.Rect.Y = this.Y;
				}
				break;
			case HTMLEngine.Core.VertAlign.Middle:
				for (int j = 0; j < this.list.Count; j++)
				{
					DeviceChunk deviceChunk2 = this.list[j];
					deviceChunk2.Rect.Y = this.Y + this.Height / 2 - deviceChunk2.Rect.Height / 2;
				}
				break;
			case HTMLEngine.Core.VertAlign.Bottom:
				for (int k = 0; k < this.list.Count; k++)
				{
					DeviceChunk deviceChunk3 = this.list[k];
					deviceChunk3.Rect.Y = this.Y + this.Height - deviceChunk3.Rect.Height;
				}
				break;
			}
		}

		public override string ToString()
		{
			return string.Format("Chunks:{0}", this.list.Count);
		}

		public bool IsFull;

		public int Y;

		public int MaxWidth;

		public int Width;

		public int Height;

		private readonly List<DeviceChunk> list = new List<DeviceChunk>();
	}
}
