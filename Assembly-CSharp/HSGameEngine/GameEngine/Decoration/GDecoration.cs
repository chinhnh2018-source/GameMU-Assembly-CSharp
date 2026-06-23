using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.BaseObject;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Render;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

namespace HSGameEngine.GameEngine.Decoration
{
	public class GDecoration : GBaseObject, IObject
	{
		public GDecoration(string resName)
		{
			this.ResName = resName;
			GDecoration.TotalObjectCount++;
			GDecoration.ShowObjectCount++;
			this.RenderType = RenderTypes.Decoration;
			this.Name = ObjectsManager.GetAutoObjectName(base.GetType().ToString());
		}

		public GameObject tarTrans
		{
			get
			{
				return this._tarTrans;
			}
			set
			{
				this._tarTrans = value;
			}
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
				if (null == value)
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
				Vector3 groundPosition = U3DUtils.GetGroundPosition2((float)this._cx / 100f, (float)this._cy / 100f, 51f);
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

		public GDecorationTypes DecorationType { get; set; }

		public string ResName { get; set; }

		public long LifeTicks { get; set; }

		public long AlphaTicks { get; set; }

		public string SoundFileName { get; set; }

		public void Start()
		{
			if (this._Started)
			{
				return;
			}
			this._Started = true;
			this._InitStatus = true;
			RenderManager.AddObject(this);
			if (!string.IsNullOrEmpty(this.ResName))
			{
				if (this.Layer < 0)
				{
					this.Layer = LayerMask.NameToLayer("Decoration");
				}
				if (this._Position3D != Vector3.zero)
				{
					this._the3DGameObject = U3DUtils.LoadDecoration(this.Name, this.ResName, this._Position3D.x, this._Position3D.y, this._Position3D.z, this.OwnerName, this.TriggerType, new AssetbundleLoaderComplete(this.LoadOK), this.Layer, this.ForceSyncLoad, this.IsCache);
					this._Trans = this._the3DGameObject.transform;
				}
				else
				{
					this._the3DGameObject = U3DUtils.LoadDecoration(this.Name, this.ResName, (float)this._cx / 100f, 0f, (float)this._cy / 100f, this.OwnerName, this.TriggerType, new AssetbundleLoaderComplete(this.LoadOK), this.Layer, this.ForceSyncLoad, this.IsCache);
					this._Trans = this._the3DGameObject.transform;
				}
				if (!string.IsNullOrEmpty(this.SoundFileName))
				{
					this._NetAudioSource = this._the3DGameObject.AddComponent<NetAudioSource>();
					this.PlaySound(string.Format("Audio/Deco/{0}", this.SoundFileName));
				}
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
			GDecoration.ShowObjectCount++;
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
			GDecoration.ShowObjectCount--;
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
			if (this.isDestory)
			{
				return;
			}
			if (GSprite.ActiveMagicDecorations.ContainsKey(this.ResName))
			{
				Dictionary<string, int> activeMagicDecorations;
				Dictionary<string, int> dictionary = activeMagicDecorations = GSprite.ActiveMagicDecorations;
				string resName;
				string text = resName = this.ResName;
				int num = activeMagicDecorations[resName];
				dictionary[text] = num - 1;
				GSprite.TotalMagicDecorations--;
			}
			this.isDestory = true;
			RenderManager.RemoveObject(this);
			GDecoration.TotalObjectCount--;
			if (null != this._the3DGameObject && this._the3DGameObject.activeSelf)
			{
				GDecoration.ShowObjectCount--;
			}
			if (null != this._the3DGameObject)
			{
				Object.Destroy(this._the3DGameObject);
				this._the3DGameObject = null;
				this._Trans = null;
			}
			this.StopSound();
			if (this.DecorationDestroyNotify != null)
			{
				this.DecorationDestroyNotify(this, EventArgs.Empty);
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

		private bool JugeLifeTicks()
		{
			if (this.DecorationType == GDecorationTypes.Loop && this.LifeTicks > 0L)
			{
				if (Global.GetCorrectLocalTime() >= this.LifeTicks)
				{
					Global.RemoveObject(this, true);
					return true;
				}
				if (this.AlphaTicks > 0L)
				{
					long num = this.LifeTicks - Global.GetCorrectLocalTime();
					if ((double)num / (double)this.AlphaTicks < 1.0)
					{
					}
				}
			}
			return false;
		}

		public void OnFrameRender()
		{
			if (!this._Started)
			{
				return;
			}
			this.JugeLifeTicks();
		}

		public Vector3 Position3D
		{
			get
			{
				return this._Position3D;
			}
			set
			{
				this._Position3D = value;
				if (null != this._Trans)
				{
					this._Trans.localPosition = new Vector3(this._Position3D.x / 100f, this._Position3D.y / 100f, this._Position3D.z / 100f);
				}
			}
		}

		public bool Pause { get; set; }

		public double BodyWidth { get; set; }

		public double BodyHeight { get; set; }

		public void EffectRootNotify(object sender, EventArgs e)
		{
			Global.RemoveObject(this, true);
		}

		public void LoadOK(AssetbundleLoader loader, GameObject go)
		{
			this._Started = true;
			if (null == go)
			{
				return;
			}
			EffectManager component = go.GetComponent<EffectManager>();
			if (component)
			{
				component.OnEffectDestroy = delegate()
				{
					Global.RemoveObject(this, true);
					Object.Destroy(this._the3DGameObject);
				};
			}
			FxPlayController component2 = go.GetComponent<FxPlayController>();
			if (component2)
			{
				FxPlayController fxPlayController = component2;
				fxPlayController.onEnd = (EndEventHandler)Delegate.Combine(fxPlayController.onEnd, delegate(object ev, GameObject g)
				{
					this.Destroy();
				});
			}
			if (GSprite.ActiveMagicDecorations.ContainsKey(this.ResName))
			{
				Dictionary<string, int> activeMagicDecorations;
				Dictionary<string, int> dictionary = activeMagicDecorations = GSprite.ActiveMagicDecorations;
				string resName;
				string text = resName = this.ResName;
				int num = activeMagicDecorations[resName];
				dictionary[text] = num + 1;
				GSprite.TotalMagicDecorations++;
			}
			if (this.ResName == "PLHXZ_wuqi.unity3d")
			{
				if (null == this.WuQi_To_PiLiHuiXuanZhan_Obj)
				{
					return;
				}
				MeshFilter componentInChildren = this.WuQi_To_PiLiHuiXuanZhan_Obj.GetComponentInChildren<MeshFilter>();
				if (null == componentInChildren)
				{
					return;
				}
				Mesh sharedMesh = componentInChildren.sharedMesh;
				MeshRenderer componentInChildren2 = this.WuQi_To_PiLiHuiXuanZhan_Obj.GetComponentInChildren<MeshRenderer>();
				if (null == componentInChildren2)
				{
					return;
				}
				GameObject gameObject = U3DUtils.FindGameObjectByName(this.The3DGameObject, "WuQi_01");
				if (null != gameObject)
				{
					MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
					MeshRenderer[] componentsInChildren2 = gameObject.GetComponentsInChildren<MeshRenderer>();
					if (componentsInChildren != null && componentsInChildren2 != null)
					{
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							componentsInChildren[i].mesh = sharedMesh;
						}
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							componentsInChildren2[j].materials = componentInChildren2.sharedMaterials;
						}
					}
				}
				Object.Destroy(this.WuQi_To_PiLiHuiXuanZhan_Obj);
				this.WuQi_To_PiLiHuiXuanZhan_Obj = null;
			}
			else if (this.ResName == "FNS_chuchang_effect.unity3d")
			{
				GameObject gameObject2 = U3DUtils.FindGameObjectByName(this.The3DGameObject, "fenghuangchuchang");
				if (gameObject2 != null)
				{
					gameObject2.AddComponent<LoadRoleShaderAgain>();
				}
			}
			if (this.DecorationLoadCompleteNotify != null)
			{
				this.DecorationLoadCompleteNotify(go, EventArgs.Empty);
			}
			ParticleSize[] componentsInChildren3 = loader.transform.GetComponentsInChildren<ParticleSize>();
			if (componentsInChildren3 != null && componentsInChildren3.Length > 0)
			{
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					componentsInChildren3[k].SetParticleSize();
				}
			}
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

		public void AddFly(Vector3 toPosition, float time)
		{
			if (null == this.The3DGameObject)
			{
				return;
			}
			FlyToPosition flyToPosition = this.The3DGameObject.AddComponent<FlyToPosition>();
			flyToPosition.FlyOverNotify = new FlyOverNotifyHandler(this.FlyOverNotify);
			flyToPosition.ToPosition = toPosition;
			flyToPosition.FlyTime = time;
		}

		public void FlyOverNotify(object sender, EventArgs e)
		{
		}

		public GDecoration Clone()
		{
			if (null == this.The3DGameObject)
			{
				return null;
			}
			return new GDecoration(this.ResName)
			{
				OwnerName = this.OwnerName,
				_the3DGameObject = Object.Instantiate<GameObject>(this.The3DGameObject),
				SpriteType = this.SpriteType,
				OwnerName = this.OwnerName,
				_Trans = this._Trans,
				LifeTicks = this.LifeTicks
			};
		}

		public static int TotalObjectCount;

		public static int ShowObjectCount;

		private Transform _Trans;

		private GameObject _tarTrans;

		private bool _InitStatus;

		private GameObject _the3DGameObject;

		private int _cx;

		private int _cy;

		public string OwnerName;

		public int TriggerType = -1;

		public int Layer = -1;

		public int HangPos;

		public bool ForceSyncLoad = true;

		public bool IsCache;

		public bool ToGround;

		private bool _Started;

		private bool isDestory;

		private Vector3 _Position3D = Vector3.zero;

		public GameObject WuQi_To_PiLiHuiXuanZhan_Obj;

		public DecorationDestroyNotifyHandler DecorationLoadCompleteNotify;

		private NetAudioSource _NetAudioSource;

		public DecorationDestroyNotifyHandler DecorationDestroyNotify;
	}
}
