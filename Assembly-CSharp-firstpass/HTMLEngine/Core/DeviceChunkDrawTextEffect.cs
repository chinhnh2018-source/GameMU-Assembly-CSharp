using System;

namespace HTMLEngine.Core
{
	internal class DeviceChunkDrawTextEffect : DeviceChunkDrawText
	{
		public override void Draw(float deltaTime, string linkText, object userData)
		{
			bool flag = this.Text.Length == 1 && this.Text.get_Chars(0) <= ' ';
			DrawTextEffect effect = this.Effect;
			if (effect != DrawTextEffect.Shadow)
			{
				if (effect == DrawTextEffect.Outline)
				{
					if (!flag)
					{
						this.Font.Draw(null, this.Rect.Offset(this.EffectAmount, 0), this.EffectColor, this.Text, true, this.Effect, this.EffectColor, this.EffectAmount, null, userData);
						this.Font.Draw(null, this.Rect.Offset(-this.EffectAmount, 0), this.EffectColor, this.Text, true, this.Effect, this.EffectColor, this.EffectAmount, null, userData);
						this.Font.Draw(null, this.Rect.Offset(0, this.EffectAmount), this.EffectColor, this.Text, true, this.Effect, this.EffectColor, this.EffectAmount, null, userData);
						this.Font.Draw(null, this.Rect.Offset(0, -this.EffectAmount), this.EffectColor, this.Text, true, this.Effect, this.EffectColor, this.EffectAmount, null, userData);
					}
				}
			}
			else if (!flag)
			{
				this.Font.Draw(null, this.Rect.Offset(this.EffectAmount, this.EffectAmount), this.EffectColor, this.Text, true, this.Effect, this.EffectColor, this.EffectAmount, null, userData);
			}
			HtDevice device = HtEngine.Device;
			if ((this.Deco & DrawTextDeco.Underline) != DrawTextDeco.None)
			{
				device.FillRect(new HtRect(this.Rect.X, this.Rect.Bottom - 2, (!this.DecoStop) ? base.TotalWidth : this.Rect.Width, 1), this.Color, userData);
			}
			if ((this.Deco & DrawTextDeco.Strike) != DrawTextDeco.None)
			{
				device.FillRect(new HtRect(this.Rect.X, this.Rect.Bottom - this.Rect.Height / 2 - 1, (!this.DecoStop) ? base.TotalWidth : this.Rect.Width, 1), this.Color, userData);
			}
			this.Font.Draw(this.Id, this.Rect, this.Color, this.Text, false, this.Effect, this.EffectColor, this.EffectAmount, linkText, userData);
		}

		public DrawTextEffect Effect;

		public HtColor EffectColor;

		public int EffectAmount;
	}
}
