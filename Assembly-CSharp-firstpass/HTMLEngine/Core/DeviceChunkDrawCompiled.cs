using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HTMLEngine.Core
{
	internal class DeviceChunkDrawCompiled : DeviceChunk
	{
		public void Parse(IEnumerator<HtmlChunk> source, int width, string id = null, HtFont font = null, [Optional] HtColor color, TextAlign align = TextAlign.Left, VertAlign valign = VertAlign.Bottom)
		{
			this.compiled.Compile(source, width, id, font, color, align, valign);
			this.offsetApplied = false;
		}

		internal override void OnAcquire()
		{
			this.offsetApplied = false;
			this.compiled = HtEngine.GetCompiler();
			base.OnAcquire();
		}

		internal override void OnRelease()
		{
			this.compiled.Dispose();
			this.compiled = null;
			base.OnRelease();
		}

		public override void Draw(float deltaTime, string linkText, object userData)
		{
			if (!this.offsetApplied)
			{
				this.compiled.Offset(this.Rect.X, this.Rect.Y);
				this.offsetApplied = true;
			}
			this.compiled.Draw(deltaTime, userData);
		}

		public override void MeasureSize()
		{
			this.Rect.Width = this.compiled.CompiledWidth;
			this.Rect.Height = this.compiled.CompiledHeight;
		}

		public HtCompiler compiled;

		private bool offsetApplied;
	}
}
