using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.BaseObject;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Render;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

namespace HSGameEngine.GameEngine.GoodsPack
{
	public class GGoodsPack : GBaseObject, IObject
	{
		public GGoodsPack()
		{
			GGoodsPack.TotalObjectCount++;
			GGoodsPack.ShowObjectCount++;
			this.RenderType = RenderTypes.GoodsPack;
			this.Name = ObjectsManager.GetAutoObjectName(base.GetType().ToString());
		}

		public string Name { get; set; }

		public bool InitStatus
		{
			get
			{
				return this._InitStatus;
			}
		}

		public object Tag { get; set; }

		public GameObject The3DGameObject
		{
			get
			{
				return this._the3DGameObject;
			}
		}

		public IObject Root
		{
			get
			{
				return this;
			}
		}

		public IObject Children
		{
			get
			{
				return this;
			}
		}

		public Transform Parent
		{
			get
			{
				if (null == this._Trans)
				{
					return null;
				}
				return this._Trans.parent;
			}
			set
			{
				if (null == this._Trans)
				{
					return;
				}
				U3DUtils.AddChild(value.gameObject, this._Trans.gameObject, true);
			}
		}

		public Point OrigCoordinate { get; set; }

		public Point Coordinate
		{
			get
			{
				return new Point(this.cx, this.cy);
			}
			set
			{
				this.cx = value.X;
				this.cy = value.Y;
				this.ApplyXYPos();
			}
		}

		public int cx
		{
			get
			{
				return this._cx;
			}
			set
			{
				this._cx = value;
			}
		}

		public int cy
		{
			get
			{
				return this._cy;
			}
			set
			{
				this._cy = value;
				this.ApplyXYPos();
			}
		}

		private void ApplyXYPos()
		{
			if (null != this._Trans)
			{
				Vector3 groundPosition = U3DUtils.GetGroundPosition2((float)this._cx / 100f, (float)this._cy / 100f, 50f);
				this._Trans.localPosition = new Vector3((float)this._cx / 100f, groundPosition.y, (float)this._cy / 100f);
			}
		}

		public int CenterX { get; set; }

		public int CenterY { get; set; }

		public RenderTypes RenderType { get; set; }

		public GSpriteTypes SpriteType { get; set; }

		public MonsterTypes MonsterType { get; set; }

		public FakeRoleTypes FakeRoleType { get; set; }

		public GActions Action { get; set; }

		public int Direction { get; set; }

		public Quaternion Rotation { get; set; }

		public double MoveSpeed { get; set; }

		public string Tip { get; set; }

		public string OwnerText { get; set; }

		public int AutoID { get; set; }

		public int GoodsPackID { get; set; }

		public int OwnerRoleID { get; set; }

		public long ProduceTicks { get; set; }

		public bool TextVisible { get; set; }

		public long TeamID { get; set; }

		public List<int> TeamRoleIDs { get; set; }

		public int GoodsID { get; set; }

		public int GoodsNum { get; set; }

		public string GoodsImageURL { get; set; }

		public int[] EfficaciousSection { get; set; }

		public int Lucky { get; set; }

		public int ExcellenceInfo { get; set; }

		public int AppendPropLev { get; set; }

		public int ForgeLevel { get; set; }

		public void Start()
		{
			if (this._Started)
			{
				return;
			}
			this._Started = true;
			this._InitStatus = true;
			RenderManager.AddObject(this);
			string fallGoods3DResNameByID = Global.GetFallGoods3DResNameByID(this.GoodsID);
			if (!string.IsNullOrEmpty(fallGoods3DResNameByID))
			{
				if (Global.IsEquip(this.GoodsID))
				{
					this.GoodsImageURL = MuAssetManager.GetBundleID("Equip", Global.GetFallGoods3DResNameByID(this.GoodsID));
				}
				else
				{
					this.GoodsImageURL = MuAssetManager.GetBundleID("Goods3d", Global.GetFallGoods3DResNameByID(this.GoodsID));
				}
			}
			if (!string.IsNullOrEmpty(this.GoodsImageURL))
			{
				Vector3 groundPos = Global.GetGroundPos(null, GSpriteTypes.None, (float)this._cx / 100f, (float)this._cy / 100f, 50f);
				this._the3DGameObject = U3DUtils.LoadGoodsPack(this.Name, this.GoodsImageURL, (float)this._cx / 100f, groundPos.y, (float)this._cy / 100f, new AssetbundleLoaderComplete(this.LoadOK));
				this._Trans = this._the3DGameObject.transform;
				string goodsFallSoundByID = Global.GetGoodsFallSoundByID(this.GoodsID);
				if (!string.IsNullOrEmpty(goodsFallSoundByID))
				{
					this._NetAudioSource = this._the3DGameObject.AddComponent<NetAudioSource>();
					this.PlaySound(string.Format("Audio/Goods/{0}", goodsFallSoundByID));
				}
				this.GetSound = Global.GetGoodsGetSoundByID(this.GoodsID);
			}
		}

		public bool ShowObject()
		{
			if (null == this._the3DGameObject)
			{
				return false;
			}
			if (null != this._the3DGameObject && this._the3DGameObject.activeSelf)
			{
				return false;
			}
			GGoodsPack.ShowObjectCount++;
			this._the3DGameObject.SetActive(true);
			return true;
		}

		public bool HideObject()
		{
			if (null == this._the3DGameObject)
			{
				return false;
			}
			if (!this._the3DGameObject.activeSelf)
			{
				return false;
			}
			GGoodsPack.ShowObjectCount--;
			this._the3DGameObject.SetActive(false);
			return true;
		}

		public bool CurrentObjectState
		{
			get
			{
				return !(null == this._the3DGameObject) && this._the3DGameObject.activeSelf;
			}
		}

		public void Destroy()
		{
			RenderManager.RemoveObject(this);
			GGoodsPack.TotalObjectCount--;
			if (null != this._the3DGameObject && this._the3DGameObject.activeSelf)
			{
				GGoodsPack.ShowObjectCount--;
			}
			if (null != this._the3DGameObject)
			{
				Object.Destroy(this._the3DGameObject);
				this._the3DGameObject = null;
				this._Trans = null;
			}
			this.StopSound();
			if (this.GoodsPackDeco != null)
			{
				Global.RemoveObject(this.GoodsPackDeco, true);
				this.GoodsPackDeco = null;
			}
		}

		public IObject FindName(string name)
		{
			return null;
		}

		public void Add(IObject obj)
		{
			Global.AssertException(false, string.Format(Global.GetLang("{0}.Add 函数不能被执行!"), base.GetType().ToString()));
		}

		public void OnFrameRender()
		{
			if (!this._Started)
			{
				return;
			}
			if (this.ProduceTicks > 0L && Global.GetCorrectLocalTime() - this.ProduceTicks >= (long)(Global.Data.GoodsDestroytimeTick * 1000))
			{
				Global.RemoveObject(this, true);
				return;
			}
		}

		public void PickUpGoodsPackByAnimation(GSprite sprite)
		{
			if (null == this.The3DGameObject)
			{
				return;
			}
			if (this.AlreadyPickUpGoodsPack)
			{
				return;
			}
			if (this.categoriy == 501 && GGoodsPack.SubYinLiang != 0)
			{
				Global.ShowTextForExpOrGold(ColorSL.ParseArgb(16422400U), Global.GetLang("金币") + "+" + GGoodsPack.SubYinLiang, 1000f, 0f, 40f, 280f, -150f);
				GGoodsPack.SubYinLiang = 0;
			}
			if (!string.IsNullOrEmpty(this.GetSound))
			{
				this._NetAudioSource = this._the3DGameObject.GetComponent<NetAudioSource>();
				if (this._NetAudioSource != null)
				{
					this.PlaySound(string.Format("Audio/Goods/{0}", this.GetSound));
				}
			}
			this.AlreadyPickUpGoodsPack = true;
			if (this.GoodsPackDeco != null)
			{
				Global.RemoveObject(this.GoodsPackDeco, true);
				this.GoodsPackDeco = null;
			}
			DestroyGoodsPack destroyGoodsPack = this.The3DGameObject.AddComponent<DestroyGoodsPack>();
			destroyGoodsPack._NotifyDestroyGoods = new NotifyDestroyGoodsHandler(this.NotifyDestroyGoods);
			destroyGoodsPack.SpritePos = sprite.The3DGameObject.transform.localPosition;
			destroyGoodsPack.TranslationTime = 3f;
			destroyGoodsPack.StartMove(this.GoodsID);
		}

		private void NotifyDestroyGoods()
		{
			Global.RemoveObject(this, true);
		}

		public void PlaySound(string playingMusicFile)
		{
			if (string.IsNullOrEmpty(playingMusicFile) && null != this._NetAudioSource)
			{
				this._NetAudioSource.StopPlay();
			}
			if (null == this._NetAudioSource)
			{
				return;
			}
			if (Global.Data.SysSetting.CloseGameAudio)
			{
				this._NetAudioSource.StopPlay();
			}
			else
			{
				this._NetAudioSource.PlayAudio(playingMusicFile, false, false);
			}
		}

		private void StopSound()
		{
			if (null == this._NetAudioSource)
			{
				return;
			}
			this._NetAudioSource.StopPlay();
			this._NetAudioSource = null;
		}

		public void LoadOK(AssetbundleLoader loader, GameObject go)
		{
			if (null == go || this.The3DGameObject == null)
			{
				return;
			}
			bool flag = true;
			Transform transform = go.transform;
			Vector3 localPosition = (!flag) ? Vector3.zero : transform.localPosition;
			Quaternion localRotation = (!flag) ? Quaternion.identity : transform.localRotation;
			Vector3 localScale = (!flag) ? Vector3.one : transform.localScale;
			transform.parent = this._Trans;
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.localScale = localScale;
			go.layer = this.The3DGameObject.layer;
			OwnerTypeManager ownerTypeManager = transform.gameObject.AddComponent<OwnerTypeManager>();
			ownerTypeManager.OwnerObject = this;
			if (Global.IsEquip(this.GoodsID))
			{
				Global.AddSpecialGameObjects4Goods(go, this.GoodsID, this.The3DGameObject.layer, 1f, new AssetbundleLoaderComplete(GGoodsPack.SpecialGameObjects4GoodsLoadOK));
				int goodsShaderIDsByID = Global.GetGoodsShaderIDsByID(this.GoodsID, this.ForgeLevel);
				if (goodsShaderIDsByID > 0)
				{
					U3DUtils.ReplaceMaterials(go, goodsShaderIDsByID, false);
				}
				go.AddComponent<LoadRoleShaderAgain>();
			}
			Vector3 center;
			center..ctor(0f, 1f, 0f);
			Vector3 size;
			size..ctor(0.85f, 2f, 0.85f);
			this.categoriy = Global.GetCategoriyByGoodsID(this.GoodsID);
			if (this.categoriy == 0)
			{
				if (go.GetComponent<Renderer>())
				{
					this._Trans.localPosition += new Vector3(0f, 0.3f, 0f);
				}
				else
				{
					this._Trans.localPosition -= new Vector3(0f, 1.42f, 0f);
				}
			}
			else if (this.categoriy == 1)
			{
				this._Trans.localPosition -= new Vector3(0f, 0.6f, 0f);
			}
			else if (this.categoriy == 2)
			{
				this._Trans.localPosition -= new Vector3(1f, 0.26f, 0f);
				this._Trans.localRotation = Quaternion.Euler(0f, 0f, 290f);
				size..ctor(1f, 1f, 1f);
			}
			else if (this.categoriy == 3)
			{
				this._Trans.localPosition -= new Vector3(0f, 0.52f, 0f);
			}
			else if (this.categoriy == 18)
			{
				this._Trans.localPosition += new Vector3(0f, 0.1f, 0f);
				this._Trans.localRotation = Quaternion.Euler(50f, 60f, 355f);
			}
			else if (this.categoriy == 4)
			{
			}
			U3DUtils.AddBoxCollider(transform.gameObject, center, size, true);
			U3DUtils.LoadRoleShaderAgain(go);
		}

		public static void SpecialGameObjects4GoodsLoadOK(AssetbundleLoader loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			go.AddComponent<LoadRoleShaderAgain>();
		}

		public static int TotalObjectCount;

		public static int ShowObjectCount;

		public static int SubYinLiang;

		private Transform _Trans;

		private bool _InitStatus;

		private GameObject _the3DGameObject;

		private int _cx;

		private int _cy;

		private bool _Started;

		private string GetSound;

		private bool AlreadyPickUpGoodsPack;

		private int categoriy;

		public GDecoration GoodsPackDeco;

		private NetAudioSource _NetAudioSource;
	}
}
