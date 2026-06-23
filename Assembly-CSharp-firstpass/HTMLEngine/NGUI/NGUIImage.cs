using System;
using HTMLEngine.Core;
using UnityEngine;

namespace HTMLEngine.NGUI
{
	public class NGUIImage : HtImage
	{
		public NGUIImage(string source, int fps)
		{
			if ("#time".Equals(source, 3))
			{
				this.isTime = true;
				this.timeFont = HtEngine.Device.LoadFont("code", 16, false, false);
			}
			else
			{
				this.isAnim = (fps >= 0);
				this.FPS = fps;
				this.spriteName = source;
				this.uiAtlas = (Resources.Load("Prefabs/Atlas/ChatIconAtlas", typeof(UIAtlas)) as UIAtlas);
				if (this.uiAtlas == null)
				{
					Debug.LogError("Could not load html image atlas from Prefabs/Atlas/ChatIconAtlas");
				}
			}
		}

		public override int Width
		{
			get
			{
				if (this.isTime)
				{
					return 120;
				}
				if (this.uiAtlas == null)
				{
					return 1;
				}
				UIAtlas.Sprite sprite = null;
				if (this.isAnim)
				{
					int i = 0;
					int count = this.uiAtlas.spriteList.Count;
					while (i < count)
					{
						UIAtlas.Sprite sprite2 = this.uiAtlas.spriteList[i];
						if (string.IsNullOrEmpty(this.spriteName) || sprite2.name.StartsWith(this.spriteName))
						{
							sprite = sprite2;
							break;
						}
						i++;
					}
				}
				else
				{
					sprite = this.uiAtlas.GetSprite(this.spriteName);
				}
				return (sprite == null) ? 1 : ((int)sprite.outer.width);
			}
		}

		public override int Height
		{
			get
			{
				if (this.isTime)
				{
					return 20;
				}
				if (this.uiAtlas == null)
				{
					return 1;
				}
				UIAtlas.Sprite sprite = null;
				if (this.isAnim)
				{
					int i = 0;
					int count = this.uiAtlas.spriteList.Count;
					while (i < count)
					{
						UIAtlas.Sprite sprite2 = this.uiAtlas.spriteList[i];
						if (string.IsNullOrEmpty(this.spriteName) || sprite2.name.StartsWith(this.spriteName))
						{
							sprite = sprite2;
							break;
						}
						i++;
					}
				}
				else
				{
					sprite = this.uiAtlas.GetSprite(this.spriteName);
				}
				return (sprite == null) ? 1 : ((int)sprite.outer.height);
			}
		}

		public override void Draw(string id, HtRect rect, HtColor color, string linkText, object userData)
		{
			if (this.isTime)
			{
				DateTime now = DateTime.Now;
				this.timeFont.Draw("time", rect, color, string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", new object[]
				{
					now.Hour,
					now.Minute,
					now.Second,
					now.Millisecond
				}), false, DrawTextEffect.None, HtColor.white, 0, linkText, userData);
			}
			else if (this.uiAtlas != null)
			{
				Transform transform = userData as Transform;
				if (transform != null)
				{
					GameObject gameObject = new GameObject((!string.IsNullOrEmpty(id)) ? id : "image", new Type[]
					{
						typeof(UISprite)
					});
					gameObject.layer = transform.gameObject.layer;
					gameObject.transform.parent = transform;
					gameObject.transform.localPosition = new Vector3((float)(rect.X + rect.Width / 2), (float)(-(float)rect.Y - rect.Height / 2), -1f);
					gameObject.transform.localScale = new Vector3((float)rect.Width, (float)rect.Height, 1f);
					UISprite component = gameObject.GetComponent<UISprite>();
					component.pivot = UIWidget.Pivot.Center;
					component.atlas = this.uiAtlas;
					component.color = new Color32(color.R, color.G, color.B, color.A);
					if (this.isAnim)
					{
						UISpriteAnimation uispriteAnimation = gameObject.AddComponent<UISpriteAnimation>();
						uispriteAnimation.framesPerSecond = this.FPS;
						uispriteAnimation.namePrefix = this.spriteName;
					}
					else
					{
						component.spriteName = this.spriteName;
						component.MakePixelPerfect();
						if (gameObject.transform.localScale.y == 0f)
						{
							gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 1f, 1f);
						}
					}
					if (!string.IsNullOrEmpty(linkText))
					{
						BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
						boxCollider.isTrigger = true;
						boxCollider.center = new Vector3(0f, 0f, -0.25f);
						boxCollider.size = new Vector3(1f, 1f, 1f);
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
		}

		public void OnRelease()
		{
			if (this.uiAtlas != null && this.uiAtlas.gameObject != null)
			{
				this.uiAtlas = null;
				Resources.UnloadUnusedAssets();
			}
		}

		private readonly bool isTime;

		private readonly HtFont timeFont;

		public UIAtlas uiAtlas;

		public readonly string spriteName;

		public readonly bool isAnim;

		public readonly int FPS;
	}
}
