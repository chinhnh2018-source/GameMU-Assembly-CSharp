using System;
using System.Collections.Generic;
using UnityEngine;

namespace HTMLEngine.NGUI
{
	public class NGUIDevice : HtDevice
	{
		public override HtFont LoadFont(string face, int size, bool bold, bool italic)
		{
			string text = string.Format("{0}{1}{2}{3}", new object[]
			{
				face,
				size,
				(!bold) ? string.Empty : "b",
				(!italic) ? string.Empty : "i"
			});
			NGUIFont nguifont;
			if (this.fonts.TryGetValue(text, ref nguifont) && nguifont.uiFont != null)
			{
				return nguifont;
			}
			nguifont = new NGUIFont(face, size, bold, italic);
			this.fonts[text] = nguifont;
			return nguifont;
		}

		public override HtImage LoadImage(string src, int fps)
		{
			NGUIImage nguiimage;
			if (this.images.TryGetValue(src, ref nguiimage))
			{
				return nguiimage;
			}
			nguiimage = new NGUIImage(src, fps);
			this.images[src] = nguiimage;
			return nguiimage;
		}

		public override void FillRect(HtRect rect, HtColor color, object userData)
		{
			Transform transform = userData as Transform;
			if (transform != null)
			{
				GameObject gameObject = new GameObject("fill", new Type[]
				{
					typeof(UISlicedSprite)
				});
				gameObject.layer = transform.gameObject.layer;
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = new Vector3((float)(rect.X + rect.Width / 2), (float)(-(float)rect.Y - rect.Height / 2 - 2), -1f);
				gameObject.transform.localScale = new Vector3((float)rect.Width, (float)rect.Height, 1f);
				UISlicedSprite component = gameObject.GetComponent<UISlicedSprite>();
				component.pivot = UIWidget.Pivot.Center;
				component.atlas = (Resources.Load("atlases/white", typeof(UIAtlas)) as UIAtlas);
				component.spriteName = "white";
				component.color = new Color32(color.R, color.G, color.B, color.A);
				component.MakePixelPerfect();
				if (gameObject.transform.localScale.y == 0f)
				{
					gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 1f, 1f);
				}
			}
			else
			{
				HtEngine.Log(HtLogLevel.Error, "Can't draw without root.", new object[0]);
			}
		}

		public override void OnRelease()
		{
			foreach (KeyValuePair<string, NGUIFont> keyValuePair in this.fonts)
			{
				keyValuePair.Value.OnRelease();
			}
			this.fonts.Clear();
			foreach (KeyValuePair<string, NGUIImage> keyValuePair2 in this.images)
			{
				keyValuePair2.Value.OnRelease();
			}
			this.images.Clear();
		}

		public void ReleaseImageAndAtalas()
		{
			foreach (KeyValuePair<string, NGUIImage> keyValuePair in this.images)
			{
				keyValuePair.Value.OnRelease();
			}
			this.images.Clear();
		}

		private readonly Dictionary<string, NGUIFont> fonts = new Dictionary<string, NGUIFont>();

		private readonly Dictionary<string, NGUIImage> images = new Dictionary<string, NGUIImage>();

		private static Texture2D whiteTex;
	}
}
