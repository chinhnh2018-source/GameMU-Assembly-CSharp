using System;

namespace HTMLEngine.Core
{
	internal class DeviceChunkDrawText : DeviceChunk
	{
		public override void Draw(float deltaTime, string linkText, object userData)
		{
			HtDevice device = HtEngine.Device;
			if ((this.Deco & DrawTextDeco.Underline) != DrawTextDeco.None)
			{
				device.FillRect(new HtRect(this.Rect.X, this.Rect.Bottom - 2, (!this.DecoStop) ? base.TotalWidth : this.Rect.Width, 1), this.Color, userData);
			}
			if ((this.Deco & DrawTextDeco.Strike) != DrawTextDeco.None)
			{
				device.FillRect(new HtRect(this.Rect.X, this.Rect.Bottom - this.Rect.Height / 2 - 1, (!this.DecoStop) ? base.TotalWidth : this.Rect.Width, 1), this.Color, userData);
			}
			this.Font.Draw(this.Id, this.Rect, this.Color, this.Text, false, DrawTextEffect.None, HtColor.white, 0, linkText, userData);
		}

		public override void MeasureSize()
		{
			this.Rect.Width = this.Font.Measure(this.Text).Width;
			this.Rect.Height = this.Font.LineSpacing;
		}

		public override string ToString()
		{
			return this.Text ?? "(null)";
		}

		public DrawTextDeco Deco;

		public bool DecoStop;

		public HtColor Color;

		public string Text;

		public string Id;

		public bool PrevIsWord;
	}
}
