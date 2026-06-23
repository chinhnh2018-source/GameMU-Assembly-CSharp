using System;
using System.IO;

namespace Xft
{
	internal class EanFile
	{
		public void Load(BinaryReader br, FileStream fs)
		{
			this.Header = br.ReadInt32();
			this.Version = br.ReadInt32();
			this.Reserved = br.ReadInt32();
			this.AnimCount = br.ReadInt32();
			this.Anims = new EanAnimation[this.AnimCount];
			for (int i = 0; i < this.AnimCount; i++)
			{
				this.Anims[i] = new EanAnimation();
				this.Anims[i].Load(br, fs);
			}
		}

		public int Header;

		public int Version;

		public int Reserved;

		public int AnimCount;

		public EanAnimation[] Anims;
	}
}
