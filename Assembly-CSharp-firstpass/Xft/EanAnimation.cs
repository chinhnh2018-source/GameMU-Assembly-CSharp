using System;
using System.IO;

namespace Xft
{
	public class EanAnimation
	{
		public void Load(BinaryReader br, FileStream fs)
		{
			this.Offset = br.ReadInt32();
			this.Offset += 16;
			this.FrameCount = br.ReadInt32();
			this.MipWidth = br.ReadInt32();
			this.MipHeight = br.ReadInt32();
			this.StartX = br.ReadInt32();
			this.StartY = br.ReadInt32();
			this.TileCount = br.ReadUInt16();
			this.TotalCount = br.ReadUInt16();
			this.CellWidth = br.ReadUInt16();
			this.CellHeight = br.ReadUInt16();
			this.Frames = new EanFrame[(int)this.TotalCount];
			long position = fs.Position;
			fs.Seek((long)this.Offset, 0);
			for (int i = 0; i < (int)this.TotalCount; i++)
			{
				this.Frames[i].X = br.ReadUInt16();
				this.Frames[i].Y = br.ReadUInt16();
				this.Frames[i].Width = br.ReadUInt16();
				this.Frames[i].Height = br.ReadUInt16();
			}
			fs.Seek(position, 0);
		}

		public int Offset;

		public int FrameCount;

		public int MipWidth;

		public int MipHeight;

		public int StartX;

		public int StartY;

		public ushort TileCount;

		public ushort TotalCount;

		public ushort CellWidth;

		public ushort CellHeight;

		public EanFrame[] Frames;
	}
}
