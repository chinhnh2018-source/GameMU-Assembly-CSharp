using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HTMLEngine.Core;

namespace HTMLEngine
{
	public class HtCompiler : PoolableObject
	{
		internal override void OnAcquire()
		{
			this.d = OP<DeviceChunkCollection>.Acquire();
		}

		internal override void OnRelease()
		{
			this.d.Dispose();
			this.d = null;
		}

		public int CompiledWidth { get; private set; }

		public int CompiledHeight { get; private set; }

		public string GetLink(int x, int y)
		{
			if (this.d != null)
			{
				foreach (KeyValuePair<DeviceChunk, string> keyValuePair in this.d.Links)
				{
					if (keyValuePair.Key.Contains(x, y))
					{
						return keyValuePair.Value;
					}
				}
			}
			return null;
		}

		public void Compile(string source, int width)
		{
			this.reader.SetSource(source);
			using (HtmlChunkCollection htmlChunkCollection = OP<HtmlChunkCollection>.Acquire())
			{
				htmlChunkCollection.Read(this.reader);
				this.Compile(htmlChunkCollection.GetEnumerator(), width, null, null, default(HtColor), TextAlign.Left, VertAlign.Bottom);
			}
		}

		internal void Compile(IEnumerator<HtmlChunk> source, int width, string id = null, HtFont font = null, [Optional] HtColor color, TextAlign align = TextAlign.Left, VertAlign valign = VertAlign.Bottom)
		{
			this.d.Clear(true);
			this.CompiledWidth = width;
			this.d.Parse(source, width, id, font, color, align, valign);
			this.MergeSameTextChunks();
			this.UpdateHeight();
		}

		private void UpdateHeight()
		{
			if (this.d.Lines.Count > 0)
			{
				DeviceChunkLine deviceChunkLine = this.d.Lines[this.d.Lines.Count - 1];
				this.CompiledHeight = deviceChunkLine.Y + deviceChunkLine.Height;
			}
			else
			{
				this.CompiledHeight = 0;
			}
		}

		private void MergeSameTextChunks()
		{
			if (this.d != null)
			{
				for (int i = 0; i < this.d.Lines.Count; i++)
				{
					DeviceChunkLine deviceChunkLine = this.d.Lines[i];
					DeviceChunk deviceChunk = null;
					int j = 0;
					while (j < deviceChunkLine.Chunks.Count)
					{
						DeviceChunk deviceChunk2 = deviceChunkLine.Chunks[j];
						if (deviceChunk == null)
						{
							deviceChunk = deviceChunk2;
							j++;
						}
						else
						{
							string text;
							this.d.Links.TryGetValue(deviceChunk, ref text);
							string text2;
							this.d.Links.TryGetValue(deviceChunk2, ref text2);
							if (string.Equals(text, text2))
							{
								DeviceChunkDrawTextEffect deviceChunkDrawTextEffect = deviceChunk as DeviceChunkDrawTextEffect;
								DeviceChunkDrawTextEffect deviceChunkDrawTextEffect2 = deviceChunk2 as DeviceChunkDrawTextEffect;
								if (deviceChunkDrawTextEffect != null && deviceChunkDrawTextEffect2 != null)
								{
									if (deviceChunkDrawTextEffect.Font.Equals(deviceChunkDrawTextEffect2.Font) && deviceChunkDrawTextEffect.Deco == deviceChunkDrawTextEffect2.Deco && deviceChunkDrawTextEffect.Color.R == deviceChunkDrawTextEffect2.Color.R && deviceChunkDrawTextEffect.Color.G == deviceChunkDrawTextEffect2.Color.G && deviceChunkDrawTextEffect.Color.B == deviceChunkDrawTextEffect2.Color.B && deviceChunkDrawTextEffect.Color.A == deviceChunkDrawTextEffect2.Color.A && (deviceChunkDrawTextEffect.DecoStop || (!deviceChunkDrawTextEffect.DecoStop && !deviceChunkDrawTextEffect2.DecoStop)) && deviceChunkDrawTextEffect.Effect == deviceChunkDrawTextEffect2.Effect && deviceChunkDrawTextEffect.EffectColor.R == deviceChunkDrawTextEffect2.EffectColor.R && deviceChunkDrawTextEffect.EffectColor.G == deviceChunkDrawTextEffect2.EffectColor.G && deviceChunkDrawTextEffect.EffectColor.B == deviceChunkDrawTextEffect2.EffectColor.B && deviceChunkDrawTextEffect.EffectColor.A == deviceChunkDrawTextEffect2.EffectColor.A && deviceChunkDrawTextEffect.EffectAmount == deviceChunkDrawTextEffect2.EffectAmount)
									{
										if (deviceChunkDrawTextEffect2.PrevIsWord)
										{
											deviceChunkDrawTextEffect.Text = deviceChunkDrawTextEffect + " " + deviceChunkDrawTextEffect2.Text;
											DeviceChunkDrawTextEffect deviceChunkDrawTextEffect3 = deviceChunkDrawTextEffect;
											deviceChunkDrawTextEffect3.Rect.Width = deviceChunkDrawTextEffect3.Rect.Width + (deviceChunkDrawTextEffect.Font.WhiteSize + deviceChunkDrawTextEffect2.Rect.Width);
										}
										else
										{
											deviceChunkDrawTextEffect.Text = deviceChunkDrawTextEffect + deviceChunkDrawTextEffect2.Text;
											DeviceChunkDrawTextEffect deviceChunkDrawTextEffect4 = deviceChunkDrawTextEffect;
											deviceChunkDrawTextEffect4.Rect.Width = deviceChunkDrawTextEffect4.Rect.Width + deviceChunkDrawTextEffect2.Rect.Width;
										}
										deviceChunkLine.Chunks.RemoveAt(j);
										deviceChunkDrawTextEffect2.Dispose();
										continue;
									}
								}
								else if (deviceChunkDrawTextEffect == null && deviceChunkDrawTextEffect2 == null)
								{
									DeviceChunkDrawText deviceChunkDrawText = deviceChunk as DeviceChunkDrawText;
									DeviceChunkDrawText deviceChunkDrawText2 = deviceChunk2 as DeviceChunkDrawText;
									if (deviceChunkDrawText != null && deviceChunkDrawText2 != null && deviceChunkDrawText.Font.Equals(deviceChunkDrawText2.Font) && deviceChunkDrawText.Deco == deviceChunkDrawText2.Deco && deviceChunkDrawText.Color.R == deviceChunkDrawText2.Color.R && deviceChunkDrawText.Color.G == deviceChunkDrawText2.Color.G && deviceChunkDrawText.Color.B == deviceChunkDrawText2.Color.B && deviceChunkDrawText.Color.A == deviceChunkDrawText2.Color.A && (deviceChunkDrawText.DecoStop || (!deviceChunkDrawText.DecoStop && !deviceChunkDrawText2.DecoStop)))
									{
										if (deviceChunkDrawText2.PrevIsWord)
										{
											deviceChunkDrawText.Text = deviceChunkDrawText + " " + deviceChunkDrawText2.Text;
											DeviceChunkDrawText deviceChunkDrawText3 = deviceChunkDrawText;
											deviceChunkDrawText3.Rect.Width = deviceChunkDrawText3.Rect.Width + (deviceChunkDrawText.Font.WhiteSize + deviceChunkDrawText2.Rect.Width);
										}
										else
										{
											deviceChunkDrawText.Text = deviceChunkDrawText + deviceChunkDrawText2.Text;
											DeviceChunkDrawText deviceChunkDrawText4 = deviceChunkDrawText;
											deviceChunkDrawText4.Rect.Width = deviceChunkDrawText4.Rect.Width + deviceChunkDrawText2.Rect.Width;
										}
										deviceChunkLine.Chunks.RemoveAt(j);
										deviceChunkDrawText2.Dispose();
										continue;
									}
								}
							}
							deviceChunk = deviceChunk2;
							j++;
						}
					}
				}
			}
		}

		public void Draw(float deltaTime, object userData = null)
		{
			if (this.d != null)
			{
				for (int i = 0; i < this.d.Lines.Count; i++)
				{
					DeviceChunkLine deviceChunkLine = this.d.Lines[i];
					for (int j = 0; j < deviceChunkLine.Chunks.Count; j++)
					{
						DeviceChunk deviceChunk = deviceChunkLine.Chunks[j];
						string linkText;
						if (this.d.Links.TryGetValue(deviceChunk, ref linkText))
						{
							deviceChunk.Draw(deltaTime, linkText, userData);
						}
						else
						{
							deviceChunk.Draw(deltaTime, null, userData);
						}
					}
				}
			}
		}

		public void Offset(int dx, int dy)
		{
			if (this.d != null)
			{
				for (int i = 0; i < this.d.Lines.Count; i++)
				{
					DeviceChunkLine deviceChunkLine = this.d.Lines[i];
					for (int j = 0; j < deviceChunkLine.Chunks.Count; j++)
					{
						DeviceChunk deviceChunk = deviceChunkLine.Chunks[j];
						DeviceChunk deviceChunk2 = deviceChunk;
						deviceChunk2.Rect.X = deviceChunk2.Rect.X + dx;
						DeviceChunk deviceChunk3 = deviceChunk;
						deviceChunk3.Rect.Y = deviceChunk3.Rect.Y + dy;
					}
				}
			}
		}

		public int GetLineCount()
		{
			return this.d.Lines.Count;
		}

		private readonly Reader reader = new Reader();

		private DeviceChunkCollection d;
	}
}
