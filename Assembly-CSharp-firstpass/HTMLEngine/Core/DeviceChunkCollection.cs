using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HTMLEngine.Core
{
	internal class DeviceChunkCollection : PoolableObject
	{
		internal override void OnAcquire()
		{
		}

		internal override void OnRelease()
		{
			this.Clear(true);
		}

		public List<DeviceChunkLine> Lines
		{
			get
			{
				return this.list;
			}
		}

		public void Clear(bool releaseItems = true)
		{
			if (releaseItems)
			{
				foreach (DeviceChunkLine deviceChunkLine in this.list)
				{
					deviceChunkLine.Dispose();
				}
			}
			this.Links.Clear();
			this.list.Clear();
			this.fontStack.Clear();
			this.colorStack.Clear();
			this.alignStack.Clear();
			this.valignStack.Clear();
		}

		private DeviceChunkDrawText AcquireDeviceChunkDrawText(string id, string text, HtFont font, HtColor color, DrawTextDeco deco, bool decoStop, bool prevIsWord)
		{
			DeviceChunkDrawText deviceChunkDrawText = OP<DeviceChunkDrawText>.Acquire();
			deviceChunkDrawText.Id = id;
			deviceChunkDrawText.Text = text;
			deviceChunkDrawText.Font = font;
			deviceChunkDrawText.Color = color;
			deviceChunkDrawText.Deco = deco;
			deviceChunkDrawText.DecoStop = decoStop;
			deviceChunkDrawText.PrevIsWord = prevIsWord;
			deviceChunkDrawText.MeasureSize();
			return deviceChunkDrawText;
		}

		private DeviceChunkDrawTextEffect AcquireDeviceChunkDrawTextEffect(string id, string text, HtFont font, HtColor color, DrawTextDeco deco, bool decoStop, DrawTextEffect effect, int effectAmount, HtColor effectColor, bool prevIsWord)
		{
			DeviceChunkDrawTextEffect deviceChunkDrawTextEffect = OP<DeviceChunkDrawTextEffect>.Acquire();
			deviceChunkDrawTextEffect.Id = id;
			deviceChunkDrawTextEffect.Text = text;
			deviceChunkDrawTextEffect.Font = font;
			deviceChunkDrawTextEffect.Color = color;
			deviceChunkDrawTextEffect.Deco = deco;
			deviceChunkDrawTextEffect.DecoStop = decoStop;
			deviceChunkDrawTextEffect.Effect = effect;
			deviceChunkDrawTextEffect.EffectAmount = effectAmount;
			deviceChunkDrawTextEffect.EffectColor = effectColor;
			deviceChunkDrawTextEffect.PrevIsWord = prevIsWord;
			deviceChunkDrawTextEffect.MeasureSize();
			return deviceChunkDrawTextEffect;
		}

		public void Parse(IEnumerator<HtmlChunk> htmlChunks, int viewportWidth, string id = null, HtFont font = null, [Optional] HtColor color, TextAlign align = TextAlign.Left, VertAlign valign = VertAlign.Bottom)
		{
			this.Clear(true);
			HtFont htFont = HtEngine.Device.LoadFont(HtEngine.DefaultFontFace, HtEngine.DefaultFontSize, false, false);
			font = ((font != null) ? font : htFont);
			color = ((color.R != 0 || color.G != 0 || color.B != 0 || color.A != 0) ? color : HtEngine.DefaultColor);
			DrawTextDeco drawTextDeco = DrawTextDeco.None;
			DrawTextEffect drawTextEffect = DrawTextEffect.None;
			HtColor effectColor = HtEngine.DefaultColor;
			int effectAmount = 1;
			string text = null;
			bool prevIsWord = false;
			DeviceChunkLine deviceChunkLine = null;
			DeviceChunkDrawText deviceChunkDrawText = null;
			while (htmlChunks.MoveNext())
			{
				HtmlChunk htmlChunk = htmlChunks.Current;
				HtmlChunkWord htmlChunkWord = htmlChunk as HtmlChunkWord;
				if (htmlChunkWord != null)
				{
					if (deviceChunkLine == null)
					{
						deviceChunkLine = this.NewLine(null, viewportWidth, align, valign);
					}
					if (drawTextEffect == DrawTextEffect.None)
					{
						deviceChunkDrawText = this.AcquireDeviceChunkDrawText(id, htmlChunkWord.Text, font, color, drawTextDeco, deviceChunkDrawText != null && deviceChunkDrawText.Deco != drawTextDeco, prevIsWord);
					}
					else
					{
						deviceChunkDrawText = this.AcquireDeviceChunkDrawTextEffect(id, htmlChunkWord.Text, font, color, drawTextDeco, deviceChunkDrawText != null && deviceChunkDrawText.Deco != drawTextDeco, drawTextEffect, effectAmount, effectColor, prevIsWord);
					}
					if (text != null && !this.Links.ContainsKey(deviceChunkDrawText))
					{
						this.Links.Add(deviceChunkDrawText, text);
					}
					if (!deviceChunkLine.AddChunk(deviceChunkDrawText, prevIsWord))
					{
						prevIsWord = true;
						string text2 = deviceChunkDrawText.Text;
						deviceChunkDrawText.Dispose();
						deviceChunkDrawText = null;
						bool decoStop = deviceChunkDrawText != null && deviceChunkDrawText.Deco != drawTextDeco;
						string text3 = null;
						int i;
						for (i = 0; i < text2.Length; i++)
						{
							char c = text2.get_Chars(i);
							if (c == ' ')
							{
								text3 = text2.Substring(0, i);
								break;
							}
						}
						if (text3 != null)
						{
							DeviceChunkDrawText deviceChunkDrawText2;
							if (drawTextEffect == DrawTextEffect.None)
							{
								deviceChunkDrawText2 = this.AcquireDeviceChunkDrawText(id, text3, font, color, drawTextDeco, decoStop, prevIsWord);
							}
							else
							{
								deviceChunkDrawText2 = this.AcquireDeviceChunkDrawTextEffect(id, text3, font, color, drawTextDeco, decoStop, drawTextEffect, effectAmount, effectColor, prevIsWord);
							}
							if (deviceChunkLine.AddChunk(deviceChunkDrawText2, prevIsWord))
							{
								if (text != null && !this.Links.ContainsKey(deviceChunkDrawText2))
								{
									this.Links.Add(deviceChunkDrawText2, text);
								}
								text2 = text2.Substring(i);
								decoStop = false;
							}
						}
						i = 0;
						int num = deviceChunkLine.GetRemainWidth();
						bool flag = false;
						string text4 = null;
						while (i < text2.Length)
						{
							char c2 = text2.get_Chars(i);
							if (c2 == '[')
							{
								text4 = null;
								flag = true;
							}
							else if (c2 == ']')
							{
								flag = false;
								text4 += c2;
							}
							if (!flag)
							{
								num -= font.Measure(c2.ToString()).Width;
							}
							else
							{
								text4 += c2;
							}
							if (num < 0)
							{
								string text5 = text2.Substring(0, i);
								DeviceChunkDrawText deviceChunkDrawText3;
								if (drawTextEffect == DrawTextEffect.None)
								{
									deviceChunkDrawText3 = this.AcquireDeviceChunkDrawText(id, text5, font, color, drawTextDeco, decoStop, prevIsWord);
								}
								else
								{
									deviceChunkDrawText3 = this.AcquireDeviceChunkDrawTextEffect(id, text5, font, color, drawTextDeco, decoStop, drawTextEffect, effectAmount, effectColor, prevIsWord);
								}
								deviceChunkLine.AddChunk(deviceChunkDrawText3, prevIsWord);
								if (text != null && !this.Links.ContainsKey(deviceChunkDrawText3))
								{
									this.Links.Add(deviceChunkDrawText3, text);
								}
								text2 = text2.Substring(i);
								if (text4 != null && text4 != "[-]")
								{
									text2 = text4 + text2;
									text4 = null;
								}
								break;
							}
							i++;
						}
						i = 0;
						num = viewportWidth;
						while (i < text2.Length)
						{
							char c3 = text2.get_Chars(i);
							if (c3 == '[')
							{
								text4 = null;
								flag = true;
							}
							else if (c3 == ']')
							{
								flag = false;
								text4 += c3;
							}
							if (!flag)
							{
								num -= font.Measure(c3.ToString()).Width;
							}
							else
							{
								text4 += c3;
							}
							if (num < 0)
							{
								string text6 = text2.Substring(0, i);
								DeviceChunkDrawText deviceChunkDrawText4;
								if (drawTextEffect == DrawTextEffect.None)
								{
									deviceChunkDrawText4 = this.AcquireDeviceChunkDrawText(id, text6, font, color, drawTextDeco, decoStop, prevIsWord);
								}
								else
								{
									deviceChunkDrawText4 = this.AcquireDeviceChunkDrawTextEffect(id, text6, font, color, drawTextDeco, decoStop, drawTextEffect, effectAmount, effectColor, prevIsWord);
								}
								deviceChunkLine = this.NewLine(deviceChunkLine, viewportWidth, align, valign);
								deviceChunkLine.AddChunk(deviceChunkDrawText4, prevIsWord);
								if (text != null && !this.Links.ContainsKey(deviceChunkDrawText4))
								{
									this.Links.Add(deviceChunkDrawText4, text);
								}
								text2 = text2.Substring(i);
								if (text4 != null && text4 != "[-]")
								{
									text2 = text4 + text2;
									text4 = null;
								}
								i = 0;
								num = viewportWidth;
							}
							else
							{
								i++;
							}
						}
						if (!string.IsNullOrEmpty(text2))
						{
							if (drawTextEffect == DrawTextEffect.None)
							{
								deviceChunkDrawText = this.AcquireDeviceChunkDrawText(id, text2, font, color, drawTextDeco, decoStop, prevIsWord);
							}
							else
							{
								deviceChunkDrawText = this.AcquireDeviceChunkDrawTextEffect(id, text2, font, color, drawTextDeco, decoStop, drawTextEffect, effectAmount, effectColor, prevIsWord);
							}
							deviceChunkLine = this.NewLine(deviceChunkLine, viewportWidth, align, valign);
							deviceChunkLine.AddChunk(deviceChunkDrawText, prevIsWord);
							if (text != null && !this.Links.ContainsKey(deviceChunkDrawText))
							{
								this.Links.Add(deviceChunkDrawText, text);
							}
						}
					}
					prevIsWord = true;
				}
				else
				{
					prevIsWord = false;
				}
				HtmlChunkTag htmlChunkTag = htmlChunk as HtmlChunkTag;
				if (htmlChunkTag != null)
				{
					string tag = htmlChunkTag.Tag;
					if (tag != null)
					{
						if (DeviceChunkCollection.<>f__switch$map2 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
							dictionary.Add("spin", 0);
							dictionary.Add("effect", 1);
							dictionary.Add("u", 2);
							dictionary.Add("s", 3);
							dictionary.Add("strike", 3);
							dictionary.Add("code", 4);
							dictionary.Add("b", 5);
							dictionary.Add("i", 6);
							dictionary.Add("a", 7);
							dictionary.Add("font", 8);
							dictionary.Add("br", 9);
							dictionary.Add("img", 10);
							dictionary.Add("p", 11);
							DeviceChunkCollection.<>f__switch$map2 = dictionary;
						}
						int num2;
						if (DeviceChunkCollection.<>f__switch$map2.TryGetValue(tag, ref num2))
						{
							switch (num2)
							{
							case 0:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										id = null;
										this.FinishLine(deviceChunkLine, align, valign);
										return;
									}
									id = htmlChunkTag.GetAttr("id");
									DeviceChunkCollection.ExctractAligns(htmlChunkTag, ref align, ref valign);
									DeviceChunkDrawCompiled deviceChunkDrawCompiled = OP<DeviceChunkDrawCompiled>.Acquire();
									deviceChunkDrawCompiled.Font = font;
									string text7 = htmlChunkTag.GetAttr("width") ?? "0";
									int num3 = 0;
									if (!int.TryParse(text7, ref num3))
									{
										num3 = 0;
									}
									if (num3 == 0)
									{
										num3 = ((deviceChunkLine != null) ? (deviceChunkLine.AvailWidth - font.WhiteSize) : viewportWidth);
									}
									if (num3 > 0)
									{
										if (num3 > viewportWidth)
										{
											num3 = viewportWidth;
										}
										deviceChunkDrawCompiled.Parse(htmlChunks, num3, id, font, color, align, valign);
										deviceChunkDrawCompiled.MeasureSize();
										if (deviceChunkLine == null)
										{
											deviceChunkLine = this.NewLine(null, viewportWidth, align, valign);
										}
										if (!deviceChunkLine.AddChunk(deviceChunkDrawCompiled, prevIsWord))
										{
											deviceChunkLine.IsFull = true;
											deviceChunkLine = this.NewLine(deviceChunkLine, viewportWidth, align, valign);
											if (!deviceChunkLine.AddChunk(deviceChunkDrawCompiled, prevIsWord))
											{
												HtEngine.Log(HtLogLevel.Error, "Could not fit spin into line. Word is too big: {0}", new object[]
												{
													deviceChunkDrawText
												});
												deviceChunkDrawCompiled.Dispose();
											}
										}
									}
									else
									{
										HtEngine.Log(HtLogLevel.Warning, "spin width is not given", new object[0]);
									}
								}
								continue;
							case 1:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										drawTextEffect = DrawTextEffect.None;
									}
									else
									{
										string text8 = htmlChunkTag.GetAttr("name") ?? "outline";
										string text9 = text8;
										if (text9 != null)
										{
											if (DeviceChunkCollection.<>f__switch$map1 == null)
											{
												Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
												dictionary.Add("shadow", 0);
												dictionary.Add("outline", 1);
												DeviceChunkCollection.<>f__switch$map1 = dictionary;
											}
											int num4;
											if (DeviceChunkCollection.<>f__switch$map1.TryGetValue(text9, ref num4))
											{
												if (num4 != 0)
												{
													if (num4 == 1)
													{
														drawTextEffect = DrawTextEffect.Outline;
														effectAmount = 1;
														effectColor = HtColor.RGBA(byte.MaxValue, byte.MaxValue, byte.MaxValue, 80);
													}
												}
												else
												{
													drawTextEffect = DrawTextEffect.Shadow;
													effectAmount = 1;
													effectColor = HtColor.RGBA(0, 0, 0, 80);
												}
											}
										}
										string attr = htmlChunkTag.GetAttr("amount");
										if (attr != null && !int.TryParse(attr, ref effectAmount))
										{
											HtEngine.Log(HtLogLevel.Error, "Invalid numeric value: " + attr, new object[0]);
										}
										string attr2 = htmlChunkTag.GetAttr("color");
										if (attr2 != null)
										{
											effectColor = HtColor.Parse(attr2);
										}
									}
								}
								continue;
							case 2:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										drawTextDeco &= ~DrawTextDeco.Underline;
									}
									else
									{
										drawTextDeco |= DrawTextDeco.Underline;
									}
								}
								continue;
							case 3:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										drawTextDeco &= ~DrawTextDeco.Strike;
									}
									else
									{
										drawTextDeco |= DrawTextDeco.Strike;
									}
								}
								continue;
							case 4:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										font = ((this.fontStack.Count <= 0) ? htFont : this.fontStack.Pop());
									}
									else
									{
										this.fontStack.Push(font);
										int size = font.Size;
										bool bold = font.Bold;
										bool italic = font.Italic;
										font = HtEngine.Device.LoadFont("code", size, bold, italic);
									}
								}
								continue;
							case 5:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										font = ((this.fontStack.Count <= 0) ? htFont : this.fontStack.Pop());
									}
									else
									{
										this.fontStack.Push(font);
										string face = font.Face;
										int size2 = font.Size;
										bool italic2 = font.Italic;
										font = HtEngine.Device.LoadFont(face, size2, true, italic2);
									}
								}
								continue;
							case 6:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										font = ((this.fontStack.Count <= 0) ? htFont : this.fontStack.Pop());
									}
									else
									{
										this.fontStack.Push(font);
										string face2 = font.Face;
										int size3 = font.Size;
										bool bold2 = font.Bold;
										font = HtEngine.Device.LoadFont(face2, size3, bold2, true);
									}
								}
								continue;
							case 7:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										id = null;
										if (this.colorStack.Count > 0)
										{
											color = this.colorStack.Pop();
										}
										text = null;
									}
									else
									{
										id = htmlChunkTag.GetAttr("id");
										text = htmlChunkTag.GetAttr("href");
										this.colorStack.Push(color);
										color = HtEngine.DefaultLinkColor;
									}
								}
								continue;
							case 8:
								if (!htmlChunkTag.IsSingle)
								{
									if (htmlChunkTag.IsClosing)
									{
										font = ((this.fontStack.Count <= 0) ? htFont : this.fontStack.Pop());
										color = ((this.colorStack.Count <= 0) ? HtEngine.DefaultColor : this.colorStack.Pop());
									}
									else
									{
										this.fontStack.Push(font);
										this.colorStack.Push(color);
										string face3 = htmlChunkTag.GetAttr("face") ?? font.Face;
										string attr3 = htmlChunkTag.GetAttr("size");
										int size4;
										if (attr3 == null || !int.TryParse(attr3, ref size4))
										{
											size4 = font.Size;
										}
										bool bold3 = font.Bold;
										bool italic3 = font.Italic;
										font = HtEngine.Device.LoadFont(face3, size4, bold3, italic3);
										color = HtColor.Parse(htmlChunkTag.GetAttr("color"), color);
									}
								}
								continue;
							case 9:
								deviceChunkLine = this.NewLine(deviceChunkLine, viewportWidth, align, valign);
								deviceChunkLine.Height = font.LineSpacing;
								continue;
							case 10:
								if (!htmlChunkTag.IsClosing)
								{
									string attr4 = htmlChunkTag.GetAttr("src");
									string attr5 = htmlChunkTag.GetAttr("width");
									string attr6 = htmlChunkTag.GetAttr("height");
									string attr7 = htmlChunkTag.GetAttr("fps");
									string attr8 = htmlChunkTag.GetAttr("id");
									int num5;
									if (attr5 == null || !int.TryParse(attr5, ref num5))
									{
										num5 = -1;
									}
									int num6;
									if (attr6 == null || !int.TryParse(attr6, ref num6))
									{
										num6 = -1;
									}
									int fps;
									if (attr7 == null || !int.TryParse(attr7, ref fps))
									{
										fps = -1;
									}
									HtImage htImage = HtEngine.Device.LoadImage(attr4, fps);
									if (num5 < 0)
									{
										num5 = htImage.Width;
									}
									if (num6 < 0)
									{
										num6 = htImage.Height;
									}
									DeviceChunkDrawImage deviceChunkDrawImage = OP<DeviceChunkDrawImage>.Acquire();
									if (deviceChunkLine == null)
									{
										deviceChunkLine = this.NewLine(null, viewportWidth, align, valign);
									}
									deviceChunkDrawImage.Image = htImage;
									deviceChunkDrawImage.Rect.Width = num5;
									deviceChunkDrawImage.Rect.Height = num6;
									deviceChunkDrawImage.Font = font;
									deviceChunkDrawImage.Id = attr8;
									if (text != null && !this.Links.ContainsKey(deviceChunkDrawImage))
									{
										this.Links.Add(deviceChunkDrawImage, text);
									}
									if (!deviceChunkLine.AddChunk(deviceChunkDrawImage, prevIsWord))
									{
										deviceChunkLine.IsFull = true;
										deviceChunkLine = this.NewLine(deviceChunkLine, viewportWidth, align, valign);
										if (!deviceChunkLine.AddChunk(deviceChunkDrawImage, prevIsWord))
										{
											HtEngine.Log(HtLogLevel.Error, "Could not fit image into line. Image is too big: {0}", new object[]
											{
												deviceChunkDrawImage
											});
											deviceChunkDrawImage.Dispose();
										}
									}
								}
								continue;
							case 11:
								if (htmlChunkTag.IsClosing)
								{
									id = null;
								}
								else
								{
									id = htmlChunkTag.GetAttr("id");
									deviceChunkLine = this.NewLine(deviceChunkLine, viewportWidth, align, valign);
									DeviceChunkCollection.ExctractAligns(htmlChunkTag, ref align, ref valign);
								}
								continue;
							}
						}
					}
					HtEngine.Log(HtLogLevel.Error, "Unsupported html tag {0}", new object[]
					{
						htmlChunkTag
					});
				}
			}
			this.FinishLine(deviceChunkLine, align, valign);
		}

		private static void ExctractAligns(HtmlChunkTag tag, ref TextAlign align, ref VertAlign valign)
		{
			string attr = tag.GetAttr("ALIGN");
			int num;
			if (attr != null)
			{
				string text = attr.ToUpperInvariant();
				if (text != null)
				{
					if (DeviceChunkCollection.<>f__switch$map3 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
						dictionary.Add("CENTER", 0);
						dictionary.Add("JUSTIFY", 1);
						dictionary.Add("RIGHT", 2);
						dictionary.Add("LEFT", 3);
						DeviceChunkCollection.<>f__switch$map3 = dictionary;
					}
					if (DeviceChunkCollection.<>f__switch$map3.TryGetValue(text, ref num))
					{
						switch (num)
						{
						case 0:
							align = TextAlign.Center;
							goto IL_D0;
						case 1:
							align = TextAlign.Justify;
							goto IL_D0;
						case 2:
							align = TextAlign.Right;
							goto IL_D0;
						case 3:
							align = TextAlign.Left;
							goto IL_D0;
						}
					}
				}
				HtEngine.Log(HtLogLevel.Warning, "Invalid attribute align: '{0}'", new object[]
				{
					attr
				});
				align = TextAlign.Left;
				IL_D0:;
			}
			attr = tag.GetAttr("VALIGN");
			if (attr != null)
			{
				string text = attr.ToUpperInvariant();
				if (text != null)
				{
					if (DeviceChunkCollection.<>f__switch$map4 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
						dictionary.Add("MIDDLE", 0);
						dictionary.Add("TOP", 1);
						dictionary.Add("BOTTOM", 2);
						DeviceChunkCollection.<>f__switch$map4 = dictionary;
					}
					if (DeviceChunkCollection.<>f__switch$map4.TryGetValue(text, ref num))
					{
						switch (num)
						{
						case 0:
							valign = VertAlign.Middle;
							goto IL_18D;
						case 1:
							valign = VertAlign.Top;
							goto IL_18D;
						case 2:
							valign = VertAlign.Bottom;
							goto IL_18D;
						}
					}
				}
				HtEngine.Log(HtLogLevel.Warning, "Invalid attribute valign: '{0}'", new object[]
				{
					attr
				});
				valign = VertAlign.Bottom;
				IL_18D:;
			}
		}

		internal DeviceChunkLine NewLine(DeviceChunkLine prevLine, int viewPortWidth, TextAlign prevAlign, VertAlign prevVAlign)
		{
			int y = 0;
			if (prevLine != null)
			{
				this.FinishLine(prevLine, prevAlign, prevVAlign);
				y = prevLine.Y + prevLine.Height;
			}
			DeviceChunkLine deviceChunkLine = OP<DeviceChunkLine>.Acquire();
			deviceChunkLine.MaxWidth = viewPortWidth;
			deviceChunkLine.Y = y;
			this.list.Add(deviceChunkLine);
			return deviceChunkLine;
		}

		internal void FinishLine(DeviceChunkLine line, TextAlign align, VertAlign valign)
		{
			if (line != null)
			{
				line.HorzAlign(align);
				line.VertAlign(valign);
			}
		}

		private readonly List<DeviceChunkLine> list = new List<DeviceChunkLine>();

		internal readonly Stack<HtFont> fontStack = new Stack<HtFont>();

		internal readonly Stack<HtColor> colorStack = new Stack<HtColor>();

		private readonly Stack<TextAlign> alignStack = new Stack<TextAlign>();

		private readonly Stack<VertAlign> valignStack = new Stack<VertAlign>();

		public Dictionary<DeviceChunk, string> Links = new Dictionary<DeviceChunk, string>();
	}
}
