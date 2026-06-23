using System;

namespace HTMLEngine.Core
{
	internal class DeviceChunkDrawImage : DeviceChunk
	{
		public override void Draw(float deltaTime, string linkText, object userData)
		{
			this.Image.Draw(this.Id, this.Rect, this.Color, linkText, userData);
		}

		public override void MeasureSize()
		{
			this.Rect.Width = this.Image.Width;
			this.Rect.Height = this.Image.Height;
		}

		public HtImage Image;

		public HtColor Color = HtColor.white;

		public string Id;
	}
}
