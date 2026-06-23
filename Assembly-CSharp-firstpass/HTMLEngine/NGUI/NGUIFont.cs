using System;
using HTMLEngine.Core;
using UnityEngine;

namespace HTMLEngine.NGUI
{
	public class NGUIFont : HtFont
	{
		public NGUIFont(string face, int size, bool bold, bool italic) : base(face, size, bold, italic)
		{
			size = 20;
			string text;
			if (UILabel._useDefaultFont)
			{
				text = "DroidSansFallback";
			}
			else
			{
				text = "myfont";
			}
			UIFont uifont = Resources.Load("Prefabs/Fonts/" + text, typeof(UIFont)) as UIFont;
			if (uifont == null)
			{
				Debug.LogError("Could not load font: " + text);
				return;
			}
			this.uiFont = Object.Instantiate<UIFont>(uifont);
			Object.DontDestroyOnLoad(this.uiFont);
			GameObject gameObject = GameObject.Find("/cachedHtmlFonts");
			if (gameObject == null)
			{
				gameObject = new GameObject("cachedHtmlFonts");
			}
			Object.DontDestroyOnLoad(gameObject);
			this.uiFont.transform.parent = gameObject.transform;
			this.uiFont.name = text;
			int num = 0;
			this.whiteSize = (int)(this.uiFont.CalculatePrintedSize(" .", true, UIFont.SymbolStyle.None, ref num, default(Vector2)).x * (float)size);
			this.whiteSize -= (int)(this.uiFont.CalculatePrintedSize(".", true, UIFont.SymbolStyle.None, ref num, default(Vector2)).x * (float)size);
		}

		public override int LineSpacing
		{
			get
			{
				return 20 + this.uiFont.verticalSpacing + 4;
			}
		}

		public override int WhiteSize
		{
			get
			{
				return this.whiteSize;
			}
		}

		public override HtSize Measure(string text)
		{
			int num = 0;
			Vector2 vector = this.uiFont.CalculatePrintedSize(text, true, UIFont.SymbolStyle.None, ref num, default(Vector2)) * 20f;
			return new HtSize((int)vector.x, (int)vector.y);
		}

		public override void Draw(string id, HtRect rect, HtColor color, string text, bool isEffect, DrawTextEffect effect, HtColor effectColor, int effectAmount, string linkText, object userData)
		{
			if (isEffect)
			{
				return;
			}
			Transform transform = userData as Transform;
			if (transform != null)
			{
				GameObject gameObject = new GameObject((!string.IsNullOrEmpty(id)) ? id : "label", new Type[]
				{
					typeof(UILabel)
				});
				gameObject.layer = transform.gameObject.layer;
				gameObject.transform.parent = transform;
				gameObject.transform.localPosition = new Vector3((float)(rect.X + rect.Width / 2), (float)(-(float)rect.Y - rect.Height / 2), 0f);
				gameObject.transform.localScale = new Vector3(20f, 20f, 1f);
				UILabel component = gameObject.GetComponent<UILabel>();
				component.pivot = UIWidget.Pivot.Center;
				component.supportEncoding = true;
				component.font = this.uiFont;
				component.text = text;
				component.color = new Color32(color.R, color.G, color.B, color.A);
				if (effect != DrawTextEffect.Shadow)
				{
					if (effect == DrawTextEffect.Outline)
					{
						component.effectStyle = UILabel.Effect.Outline;
					}
				}
				else
				{
					component.effectStyle = UILabel.Effect.Shadow;
				}
				component.effectColor = new Color32(effectColor.R, effectColor.G, effectColor.B, effectColor.A);
				component.effectDistance = new Vector2((float)effectAmount, (float)effectAmount);
				if (!string.IsNullOrEmpty(linkText))
				{
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					boxCollider.isTrigger = true;
					boxCollider.center = new Vector3(0f, 0f, -0.25f);
					boxCollider.size = new Vector3(component.relativeSize.x, 1f, 1f);
					NGUILinkText nguilinkText = gameObject.AddComponent<NGUILinkText>();
					nguilinkText.linkText = linkText;
					UIButtonColor uibuttonColor = gameObject.AddComponent<UIButtonColor>();
					uibuttonColor.tweenTarget = gameObject;
					uibuttonColor.hover = new Color32(HtEngine.LinkHoverColor.R, HtEngine.LinkHoverColor.G, HtEngine.LinkHoverColor.B, HtEngine.LinkHoverColor.A);
					uibuttonColor.pressed = new Color(component.color.r * HtEngine.LinkPressedFactor, component.color.g * HtEngine.LinkPressedFactor, component.color.b * HtEngine.LinkPressedFactor, component.color.a);
					uibuttonColor.duration = 0f;
					UIButtonMessage uibuttonMessage = gameObject.AddComponent<UIButtonMessage>();
					uibuttonMessage.target = transform.gameObject;
					uibuttonMessage.functionName = HtEngine.LinkFunctionName;
				}
			}
			else
			{
				HtEngine.Log(HtLogLevel.Error, "Can't draw without root.", new object[0]);
			}
		}

		public void OnRelease()
		{
			if (this.uiFont != null && this.uiFont)
			{
				Object.Destroy(this.uiFont.gameObject);
				this.uiFont = null;
			}
		}

		private const int DefaultSize = 20;

		public UIFont uiFont;

		private readonly int whiteSize;
	}
}
