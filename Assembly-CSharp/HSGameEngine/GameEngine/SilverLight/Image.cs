using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class Image : TTMonoBehaviour
	{
		protected virtual void Awake()
		{
			this.mTrans = base.transform;
		}

		public int State
		{
			set
			{
				if (!(null != this.CachedTexture) || this._State_Mats == null || this._State_Mats.Count > value)
				{
				}
			}
		}

		public double Width
		{
			get
			{
				return (double)this.mTrans.localScale.x;
			}
			set
			{
				this.mTrans.localScale = new Vector3((float)value, this.mTrans.localScale.y, this.mTrans.localScale.z);
			}
		}

		public double Height
		{
			get
			{
				return (double)this.mTrans.localScale.y;
			}
			set
			{
				this.mTrans.localScale = new Vector3(this.mTrans.localScale.x, (float)value, this.mTrans.localScale.z);
			}
		}

		public ImageBrush Source
		{
			get
			{
				return new ImageBrush(new BitmapData(0.0, 0.0, true, uint.MaxValue)
				{
					TextureData = this.Texture.mainTexture
				});
			}
			set
			{
				if (null != this.Texture && value != null)
				{
					this.Texture.mainTexture = value.ImageSource.TextureData;
				}
			}
		}

		public int Stretch { get; set; }

		public void Destroy()
		{
		}

		private void OnDestroy()
		{
			this.Texture = null;
		}

		public bool IsHitTestVisible { get; set; }

		public Rectangle scrollRect { get; set; }

		public bool Visibility { get; set; }

		public bool mouseEnabled { get; set; }

		public string Name { get; set; }

		public string HorizontalAlignment { get; set; }

		public string VerticalAlignment { get; set; }

		public double BodyWidth { get; set; }

		public double BodyHeight { get; set; }

		public bool addEventListener(string action, MouseEventHandler handler)
		{
			return true;
		}

		public bool removeEventListener(string action, MouseEventHandler handler)
		{
			return true;
		}

		public List<Material> _State_Mats;

		public UITexture Texture;

		private Transform mTrans;

		private Texture CachedTexture;
	}
}
