using System;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.BaseObject;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Render;
using UnityEngine;

namespace HSGameEngine.GameEngine.Teleport
{
	public class GTeleport : GBaseObject, IObject
	{
		public GTeleport(string resName)
		{
			this.ResName = resName;
			GTeleport.TotalObjectCount++;
			GTeleport.ShowObjectCount++;
			this.RenderType = RenderTypes.Teleport;
			this.Name = ObjectsManager.GetAutoObjectName(base.GetType().ToString());
		}

		public void LoadOK(AssetbundleLoader loader, GameObject go)
		{
			if (null == go || loader == null)
			{
				return;
			}
			if (Global.IsInKuaFuHuoDongWangZhe())
			{
				if (!KuaFuWangZhePart.wangzheData.IsTeleportEnable((int)this.Key))
				{
					this.The3DGameObject.SetActive(false);
				}
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

		public string ResName { get; set; }

		public string Tip { get; set; }

		public byte Key { get; set; }

		public int To { get; set; }

		public double ToX { get; set; }

		public double ToY { get; set; }

		public double ToDirection { get; set; }

		public double Radius { get; set; }

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
				Vector3 groundPosition = U3DUtils.GetGroundPosition2((float)this._cx / 100f, (float)this._cy / 100f, 50f);
				this._the3DGameObject = U3DUtils.LoadTeleport(this.Tip, this.Name, this.ResName, groundPosition.x, groundPosition.y, groundPosition.z, new AssetbundleLoaderComplete(this.LoadOK), delegate(AssetbundleLoader e, GameObject s)
				{
					if (this.LoadLuolanFazhenTeleportsIndex++ < 10)
					{
						Object.Destroy(this._the3DGameObject);
						this._Started = false;
						this.Start();
					}
				});
				this._Trans = this._the3DGameObject.transform;
				this._the3DGameObject.name = this.Name;
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
			if (Global.IsInKuaFuHuoDongWangZhe() && !KuaFuWangZhePart.wangzheData.IsTeleportEnable((int)this.Key))
			{
				return false;
			}
			GTeleport.ShowObjectCount++;
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
			if (Global.IsInKuaFuHuoDongWangZhe() && KuaFuWangZhePart.wangzheData.IsTeleportEnable((int)this.Key))
			{
				return false;
			}
			GTeleport.ShowObjectCount--;
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
			GTeleport.TotalObjectCount--;
			if (null != this._the3DGameObject && this._the3DGameObject.activeSelf)
			{
				GTeleport.ShowObjectCount--;
			}
			if (null != this._the3DGameObject)
			{
				Object.Destroy(this._the3DGameObject);
				this._the3DGameObject = null;
				this._Trans = null;
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
		}

		public static int TotalObjectCount;

		public static int ShowObjectCount;

		private Transform _Trans;

		private bool _InitStatus;

		private GameObject _the3DGameObject;

		private int _cx;

		private int _cy;

		private bool _Started;

		private int LoadLuolanFazhenTeleportsIndex;
	}
}
