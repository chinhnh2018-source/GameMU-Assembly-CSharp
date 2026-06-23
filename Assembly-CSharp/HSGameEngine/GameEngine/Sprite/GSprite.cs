using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.BaseObject;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.Render;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

namespace HSGameEngine.GameEngine.Sprite
{
	public class GSprite : GBaseObject, IObject
	{
		public GSprite()
		{
			GSprite.TotalObjectCount++;
			GSprite.ShowObjectCount++;
			this.RenderType = RenderTypes.Sprite;
			this.Name = ObjectsManager.GetAutoObjectName(base.GetType().ToString());
			this.GuidCurrentId++;
			GSprite.GuidSpriteId = this.GuidCurrentId;
			if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other)
			{
				this.HorseController = new HorseController(this);
			}
		}

		public event CoordinateEventHandler CoordinateChanged;

		public event SpriteChangeActionEventHandler SpriteChangeAction;

		public event EventHandler SpriteDead;

		public AnimationManager AnimationMgr
		{
			get
			{
				return this._AnimationMgr;
			}
		}

		public Transform Trans
		{
			get
			{
				return this._Trans;
			}
			set
			{
				this._Trans = value;
				if (null != this._Trans)
				{
					this.RefreshRoleInfoDisplayObjTrans();
				}
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

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

		public HorseController HorseController
		{
			get
			{
				return this._HorseController;
			}
			set
			{
				this._HorseController = value;
			}
		}

		private Transform JiaoYinFashionParent
		{
			set
			{
				if (null != this.mJiaoYinFashion)
				{
					this.mJiaoYinFashion.transform.SetParent(value, false);
					this.mJiaoYinFashion.transform.localPosition = Vector3.zero;
				}
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
				if (null == this.Trans)
				{
					return null;
				}
				return this.Trans.parent;
			}
			set
			{
				if (null == this.Trans)
				{
					return;
				}
				U3DUtils.AddChild(value.gameObject, this.Trans.gameObject, true);
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
				if (this.IsFixedPosition)
				{
					this.cx = this.FixedPosition.X;
					this.cy = this.FixedPosition.Y;
				}
				else
				{
					this.cx = value.X;
					this.cy = value.Y;
				}
				this.ChangeCoordinateProperty();
			}
		}

		private void ChangeCoordinateProperty()
		{
			if (null != this.Trans)
			{
				Vector3 vector;
				vector..ctor((float)this._cx / 100f, this.Trans.localPosition.y, (float)this._cy / 100f);
				this.Move3D(vector - this.Trans.localPosition, vector);
			}
			if (this.CoordinateChanged != null)
			{
				this.CoordinateChanged(this);
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
				this.ChangeCoordinateProperty();
			}
		}

		public int CenterX { get; set; }

		public int CenterY { get; set; }

		public RenderTypes RenderType { get; set; }

		public GSpriteTypes SpriteType { get; set; }

		public Animator HorseAnimator
		{
			get
			{
				if (this.HorseController != null)
				{
					return this.HorseController.HorseAnimator;
				}
				return null;
			}
		}

		public bool OnHorseEX
		{
			get
			{
				return this.HorseController != null && this.HorseController.OnHorse;
			}
			protected set
			{
				if (this.HorseController != null && value != this.HorseController.OnHorse)
				{
					this.HorseController.OnHorse = value;
				}
			}
		}

		public MonsterTypes MonsterType { get; set; }

		public FakeRoleTypes FakeRoleType { get; set; }

		public bool IsCloneRole { get; set; }

		public GActions LastAction
		{
			get
			{
				return this._LastAction;
			}
		}

		public Animator AnimatorComponent
		{
			get
			{
				return this.mAnimatorComponent;
			}
		}

		public MonsterStateController AnimationController
		{
			get
			{
				return this.mAnimationController;
			}
		}

		public bool HasClip(string name)
		{
			return this.mAnimationClips != null && this.mAnimationClips.ContainsKey(name);
		}

		public float GetClipLength(string name)
		{
			if (this.mAnimationClips == null || this.mAnimatorComponent == null)
			{
				return 1f;
			}
			float num = this.mAnimatorComponent.GetFloat(name + "Time");
			if (num <= 0f)
			{
				num = 1f;
			}
			return num;
		}

		private void InitAnimationClips()
		{
			if (!this.mAnimatorComponent)
			{
				return;
			}
			this.mAnimationClips = new Dictionary<string, AnimationClip>();
			if (this.mAnimatorComponent.runtimeAnimatorController != null)
			{
				AnimationClip[] animationClips = this.mAnimatorComponent.runtimeAnimatorController.animationClips;
				for (int i = 0; i < animationClips.Length; i++)
				{
					if (animationClips[i])
					{
						this.mAnimationClips[animationClips[i].name] = animationClips[i];
						this.AnimatorComponent.SetBool(animationClips[i].name, false);
					}
				}
			}
		}

		public NpcStateController NpcStateMgr
		{
			get
			{
				return this._NpcStateMgr;
			}
		}

		public GActions Action
		{
			get
			{
				return this._Action;
			}
			set
			{
				if (this.AnimationController != null && this.SpriteType == GSpriteTypes.Monster)
				{
					if (this.SpriteType == GSpriteTypes.Monster && !this.IsBossType && value == GActions.Injured)
					{
						if (this._Action == GActions.Death)
						{
							return;
						}
						GActions action = this._Action;
						this._LastAction = action;
						if (action == GActions.Death)
						{
							return;
						}
					}
					else if (this._Action != value)
					{
						if (!this.LoadReal3DResOK || (!this.LoadReal3DWeaponOk && (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole || this.SpriteType == GSpriteTypes.Other)))
						{
							return;
						}
						if (this._Action == GActions.Death)
						{
							return;
						}
						if (value == GActions.Injured && this.MagicID != -1)
						{
							return;
						}
						if (this._Action == GActions.Magic && (value == GActions.Walk || value == GActions.Run))
						{
							return;
						}
						GActions action2 = this._Action;
						this._LastAction = action2;
						if (action2 == GActions.Death)
						{
							return;
						}
						this._LastActionTicks = Global.GetCorrectLocalTime();
					}
					this.FreeStoppedSound();
					if (this._Action != GActions.Magic)
					{
						this.IsShowAllWeapon(true);
					}
					this._Action = value;
					if (value == GActions.Stand)
					{
						this.AnimationController.SetState(SpriteAnimState.Stand);
					}
					else if (value == GActions.Walk)
					{
						this.AnimationController.SetState(SpriteAnimState.Walk);
					}
					else if (value == GActions.Run)
					{
						this.AnimationController.SetState(SpriteAnimState.Run);
					}
					else if (value == GActions.Attack)
					{
						this.AnimationController.SetState(SpriteAnimState.Attack);
					}
					else if (value == GActions.Death)
					{
						this.AnimationController.SetState(SpriteAnimState.Death);
					}
					else if (value == GActions.Injured)
					{
						this.AnimationController.SetState(SpriteAnimState.Injured);
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"Unhandled State " + value
						});
						this.AnimationController.SetState(SpriteAnimState.Stand);
					}
					return;
				}
				if (this.SpriteType == GSpriteTypes.Monster && !this.IsBossType && value == GActions.Injured)
				{
					if (this._Action == GActions.Death)
					{
						return;
					}
					GActions action3 = this._Action;
					this._LastAction = action3;
					if (action3 == GActions.Death)
					{
						return;
					}
					this._Action = value;
					this.ChangeAction(action3);
				}
				else if (this._Action != value)
				{
					if (!this.LoadReal3DResOK || (!this.LoadReal3DWeaponOk && (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole || this.SpriteType == GSpriteTypes.Other)))
					{
						return;
					}
					if (this._Action == GActions.Death)
					{
						return;
					}
					if (value == GActions.Injured && this.MagicID != -1)
					{
						return;
					}
					if (this._Action == GActions.Magic && (value == GActions.Walk || value == GActions.Run))
					{
						return;
					}
					GActions action4 = this._Action;
					this._LastAction = action4;
					if (action4 == GActions.Death)
					{
						return;
					}
					this._Action = value;
					this._LastActionTicks = Global.GetCorrectLocalTime();
					this.ChangeAction(action4);
				}
				if (this.Action == GActions.Death)
				{
					this.RoleUnLoadMount(0);
				}
			}
		}

		public bool IsBossType
		{
			get
			{
				return this.MonsterType == MonsterTypes.Boss || this.MonsterType == MonsterTypes.JingJiChangRobot || this.MonsterType == MonsterTypes.Rarity;
			}
		}

		public void ChangeHorseAction(GActions action)
		{
			if (null != this._AnimationMgr)
			{
				this._AnimationMgr.ChangeHorseAction(this._Action);
			}
			WrapMode wrapMode = 2;
			string actionName = Global.GetActionName(this, this.Action, this.IsInSafeRegion, out wrapMode);
			if (!string.IsNullOrEmpty(actionName) && actionName.StartsWith("R_") && null != this._AnimationMgr)
			{
				if (this._Action == GActions.Stand)
				{
					if (this._AnimationMgr.AnimationName != actionName)
					{
						this._AnimationMgr.DelayChangeAnimation(actionName, wrapMode, 0.5f);
					}
				}
				else if (this._AnimationMgr.AnimationName != actionName)
				{
					this._AnimationMgr.ChangeAnimation(actionName, wrapMode, false, null, 0f);
				}
			}
		}

		public void ChangeAction(GActions oldAction)
		{
			if (this.SpriteType != GSpriteTypes.NPC)
			{
				this.FreeStoppedSound();
			}
			if (this._Action == GActions.Wenhao || this._Action == GActions.Genwolai || this._Action == GActions.Guzhang || this._Action == GActions.Huanhu || this._Action == GActions.Jushang || this._Action == GActions.Xingli || this._Action == GActions.Chongfeng || this._Action == GActions.Mobai || this._Action == GActions.Tiaoxin || this._Action == GActions.Zuoxia || this._Action == GActions.Shuijiao || this._Action == GActions.Collect)
			{
				this.IsShowAllWeapon(false);
			}
			else if (this._Action != GActions.Magic)
			{
				this.IsShowAllWeapon(true);
			}
			if (null != this._AnimationMgr)
			{
				if (this.Action != GActions.Magic || this.CurrentMagic < 0)
				{
					WrapMode wrapMode = 2;
					string animationName = Global.GetActionName(this, this.Action, this.IsInSafeRegion, out wrapMode);
					if (this.SpriteType == GSpriteTypes.Monster && this.Action == GActions.Attack)
					{
						animationName = Global.GetMonsterActionName(this.CurrentMagic, out wrapMode);
					}
					bool onceLoop = false;
					if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other)
					{
						if (this.OnHorseEX)
						{
							this.DoActionOnHorse(oldAction);
						}
						else if ((this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole) && oldAction == GActions.Attack && this._Action == GActions.Stand && this.AttackNum < 2)
						{
							this._AnimationMgr.DelayChangeAnimation(animationName, wrapMode, 0f);
						}
						else if (this.SpriteType != GSpriteTypes.NPC)
						{
							this._AnimationMgr.ChangeAnimation(animationName, wrapMode, onceLoop, this, 0f);
						}
					}
					else if ((this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole) && oldAction == GActions.Attack && this._Action == GActions.Stand && this.AttackNum < 2)
					{
						this._AnimationMgr.DelayChangeAnimation(animationName, wrapMode, 0f);
					}
					else if (this.SpriteType != GSpriteTypes.NPC)
					{
						this._AnimationMgr.ChangeAnimation(animationName, wrapMode, onceLoop, this, 0f);
					}
				}
				else
				{
					WrapMode wrapMode2 = 1;
					string text = ConfigMagicInfos.GetSkillActionName(this.CurrentMagic);
					if (ConfigMagicInfos.IsHorseSkill(this.CurrentMagic))
					{
						int num = Global.CalcOriginalOccupationID(this.Occupation);
						if (Global.CalcOriginalOccupationID(this.Occupation) == 0)
						{
							text = "smzg";
						}
						else if (Global.CalcOriginalOccupationID(this.Occupation) == 1)
						{
							text = "shzh";
						}
						else if (Global.CalcOriginalOccupationID(this.Occupation) == 2)
						{
							text = "zl";
						}
						else if (Global.CalcOriginalOccupationID(this.Occupation) == 3)
						{
							text = "xjzf";
						}
						else if (Global.CalcOriginalOccupationID(this.Occupation) == 4)
						{
							text = string.Empty;
						}
						else if (Global.CalcOriginalOccupationID(this.Occupation) == 5)
						{
							text = "shfs";
						}
					}
					else if (text.CompareTo("dcj") == 0)
					{
						text = "Ndcj";
						if (this.WeaponState == WeaponStates.G)
						{
							text = "Gdcj";
						}
					}
					else if (text.CompareTo("wcj") == 0)
					{
						text = "Nwcj";
						if (this.WeaponState == WeaponStates.G)
						{
							text = "Gwcj";
						}
					}
					else if (text.CompareTo("zl") == 0)
					{
						text = "Nzl";
						if (this.WeaponState == WeaponStates.G)
						{
							text = "Gzl";
						}
					}
					else if (text.CompareTo("ctj") == 0)
					{
						text = "Nctj";
						if (this.WeaponState == WeaponStates.G)
						{
							text = "Gctj";
						}
					}
					else if (text.Contains("bfj"))
					{
						if (this.WeaponState == WeaponStates.G)
						{
							text = "G" + text;
						}
						else
						{
							text = "N" + text;
						}
					}
					if (this.OnHorseEX)
					{
						this.DoActionOnHorse(oldAction);
					}
					else
					{
						this._AnimationMgr.ChangeAnimation(text, wrapMode2, false, null, 0f);
					}
				}
			}
			if (this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole)
			{
				if (this._Action == GActions.Run || this._Action == GActions.Walk || this._Action == GActions.Stand)
				{
					this.ChangeHorseAction(this._Action);
				}
				else if (this.OnHorseEX)
				{
				}
			}
			if (this._Action == GActions.Run)
			{
				this.ChangeWingsAction(GActions.Run);
			}
			else
			{
				this.ChangeWingsAction(GActions.Stand);
			}
			if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other)
			{
			}
			if ((this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other) && this.InWater && !this.IsFlying)
			{
				if (this._Action == GActions.Run || this._Action == GActions.Walk)
				{
					this.ChangeWeaponsToBack(true);
				}
				else
				{
					this.ChangeWeaponsToBack(false);
				}
			}
		}

		private void DoActionOnHorse(GActions oldAction)
		{
			if ("R_Stand" == this._AnimationMgr.AnimationName || "R_Walk" == this._AnimationMgr.AnimationName || "R_Run" == this._AnimationMgr.AnimationName)
			{
				if (this._Action == GActions.Attack)
				{
					this.PlayingAnimation(null, null);
				}
			}
			else if (this.Action != GActions.Magic || this.CurrentMagic < 0)
			{
				WrapMode wrapMode = 2;
				string animationName = Global.GetActionName(this, this.Action, this.IsInSafeRegion, out wrapMode);
				if (this.SpriteType == GSpriteTypes.Monster && this.Action == GActions.Attack)
				{
					animationName = Global.GetMonsterActionName(this.CurrentMagic, out wrapMode);
				}
				bool onceLoop = false;
				if ((this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole) && oldAction == GActions.Attack && this._Action == GActions.Stand && this.AttackNum < 2)
				{
					this._AnimationMgr.DelayChangeAnimation(animationName, wrapMode, 0f);
				}
				else if (this.SpriteType != GSpriteTypes.NPC)
				{
					this._AnimationMgr.ChangeAnimation(animationName, wrapMode, onceLoop, this, 0f);
				}
			}
			else
			{
				WrapMode wrapMode2 = 1;
				string text = ConfigMagicInfos.GetSkillActionName(this.CurrentMagic);
				if (ConfigMagicInfos.IsHorseSkill(this.CurrentMagic))
				{
					int num = Global.CalcOriginalOccupationID(this.Occupation);
					if (Global.CalcOriginalOccupationID(this.Occupation) == 0)
					{
						text = "smzg";
					}
					else if (Global.CalcOriginalOccupationID(this.Occupation) == 1)
					{
						text = "shzh";
					}
					else if (Global.CalcOriginalOccupationID(this.Occupation) == 2)
					{
						text = "zl";
					}
					else if (Global.CalcOriginalOccupationID(this.Occupation) == 3)
					{
						text = "xjzf";
					}
					else if (Global.CalcOriginalOccupationID(this.Occupation) == 4)
					{
						text = string.Empty;
					}
					else if (Global.CalcOriginalOccupationID(this.Occupation) == 5)
					{
						text = "shfs";
					}
				}
				else if (text.CompareTo("dcj") == 0)
				{
					text = "Ndcj";
					if (this.WeaponState == WeaponStates.G)
					{
						text = "Gdcj";
					}
				}
				else if (text.CompareTo("wcj") == 0)
				{
					text = "Nwcj";
					if (this.WeaponState == WeaponStates.G)
					{
						text = "Gwcj";
					}
				}
				else if (text.CompareTo("zl") == 0)
				{
					text = "Nzl";
					if (this.WeaponState == WeaponStates.G)
					{
						text = "Gzl";
					}
				}
				else if (text.CompareTo("ctj") == 0)
				{
					text = "Nctj";
					if (this.WeaponState == WeaponStates.G)
					{
						text = "Gctj";
					}
				}
				else if (text.Contains("bfj"))
				{
					if (this.WeaponState == WeaponStates.G)
					{
						text = "G" + text;
					}
					else
					{
						text = "N" + text;
					}
				}
				this._AnimationMgr.ChangeAnimation(text, wrapMode2, false, null, 0f);
			}
		}

		public int GetDirection()
		{
			return this._Direction;
		}

		public int Direction
		{
			get
			{
				return this._Direction;
			}
			set
			{
				if (this.IsFixedDirection)
				{
					this._Direction = this.FixedDirection;
					this.FixedRotation = Global.GetQuaternionByDir(this.Direction);
				}
				else
				{
					this._Direction = Math.Min(value, 7);
					this._Direction = Math.Max(value, 0);
				}
				this.ChangeDirection();
			}
		}

		private void ChangeDirection()
		{
			Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
			int yangle = Global.GetYAngle(quaternionByDir);
			int yangle2 = Global.GetYAngle(this._Rotation);
			if (yangle / 45 != yangle2 / 45)
			{
				this._Rotation = quaternionByDir;
			}
		}

		public Quaternion Rotation
		{
			get
			{
				return this._Rotation;
			}
			set
			{
				if (this.IsFixedDirection)
				{
					this._Rotation = this.FixedRotation;
				}
				else
				{
					this._Rotation = value;
				}
			}
		}

		public void DecMovingSpeedMills(int delta)
		{
			this.MovingSpeedMills -= delta;
			if (this.MovingSpeedMills <= 0)
			{
				this.MovingSpeedMills = 0;
				this._MovingSpeed = 1.0;
			}
		}

		public void DecDongJieAndFaint(int delta)
		{
			RoleData roleData = null;
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				roleData = Global.Data.roleData;
			}
			else
			{
				Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData);
			}
			if (roleData != null && roleData.DongJieStart > 0L)
			{
				roleData.DongJieMills -= delta;
				if (roleData.DongJieMills <= 0)
				{
					roleData.DongJieStart = 0L;
					roleData.DongJieMills = 0;
				}
			}
		}

		public double MoveSpeed
		{
			get
			{
				return this._MovingSpeed;
			}
			set
			{
				this._MovingSpeed = value;
			}
		}

		public long VArmorMax { get; set; }

		public long VArmor { get; set; }

		public int RoleID { get; set; }

		public int RoleSex { get; set; }

		public int FactionID { get; set; }

		public string VFaction { get; set; }

		public GPKModes PKMode { get; set; }

		public bool IsOpposition { get; set; }

		public string VOtherName
		{
			get
			{
				Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
				return this._VOtherName;
			}
			set
			{
				this._VOtherName = value;
				if (null != this._RoleInfoDisplay)
				{
					this._RoleInfoDisplay.OtherNameText = this._VOtherName;
				}
				Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
			}
		}

		public string VSName
		{
			get
			{
				return this._VSName;
			}
			set
			{
				this._VSName = value;
			}
		}

		public string ShowName
		{
			get
			{
				return this._ShowName;
			}
			set
			{
				this._ShowName = value;
				if (null != this._RoleInfoDisplay)
				{
					this._RoleInfoDisplay.RoleNameText = this._ShowName;
				}
			}
		}

		public int Occupation { get; set; }

		public int BattleWhichSide { get; set; }

		public int ExtensionID
		{
			get
			{
				return this._ExtensionID;
			}
			set
			{
				this._ExtensionID = value;
				int fixedDirection;
				Point fixedPosition;
				Global.IsFixedDirectionMonster(this._ExtensionID, out fixedDirection, out fixedPosition, out this.isFixed);
				if (!this.isFixed)
				{
					Global.IsFixedDirectionMonsterInWangZheZhanChang(this._ExtensionID, this.cx, this.cy, out fixedDirection, out fixedPosition, out this.isFixed);
				}
				if (!this.isFixed && (this._ExtensionID == 9610000 || this._ExtensionID == 9610001))
				{
					Vector2 zhanMengLianSaiBiSaiQiZiFixedPosition = Global.GetZhanMengLianSaiBiSaiQiZiFixedPosition(this.cx, this.cy);
					fixedPosition = new Point((int)zhanMengLianSaiBiSaiQiZiFixedPosition.x, (int)zhanMengLianSaiBiSaiQiZiFixedPosition.y);
					fixedDirection = 6;
					this.isFixed = true;
				}
				if (this.isFixed && (Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuWangZhe || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong || Global.GetMapSceneUIClass() == SceneUIClasses.ZhanMengLianSaiBiSai || Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuPlunderBattle))
				{
					this.IsFixedDirection = true;
					this.IsFixedPosition = true;
					this.FixedDirection = fixedDirection;
					this.FixedPosition = fixedPosition;
					this.Direction = this.FixedDirection;
				}
				else if (this._ExtensionID > 600000 && this._ExtensionID < 700000)
				{
					this.IsFixedDirection = true;
					this.IsFixedPosition = true;
					int num = this._ExtensionID % 100;
					if (num == 7 || num == 8)
					{
						if (num == 8)
						{
							this.FixedDirection = 6;
						}
						else if (Global.IsBloodCastleLingGuan(this.ExtensionID))
						{
							this.FixedDirection = 0;
						}
						XElement gameResXml = Global.GetGameResXml("Config/FreshPlayerCopySceneInfo.xml");
						XElement xelement = Global.GetXElement(gameResXml, "FreshPlayerCopySceneInfo", "MapCode", "6090");
						int[] xelementAttributeIntArray = Global.GetXElementAttributeIntArray(xelement, "DiaoXiangPos", "*");
						if (xelementAttributeIntArray.Length == 2)
						{
							this._cx = xelementAttributeIntArray[0];
							this._cy = xelementAttributeIntArray[1];
							this.FixedPosition = new Point(this._cx, this._cy);
						}
					}
					else if (num == 6)
					{
						this.FixedDirection = 6;
						XElement gameResXml2 = Global.GetGameResXml("Config/FreshPlayerCopySceneInfo.xml");
						XElement xelement2 = Global.GetXElement(gameResXml2, "FreshPlayerCopySceneInfo", "MapCode", "6090");
						int[] xelementAttributeIntArray2 = Global.GetXElementAttributeIntArray(xelement2, "GatePos", "*");
						if (xelementAttributeIntArray2.Length == 2)
						{
							this._cx = xelementAttributeIntArray2[0];
							this._cy = xelementAttributeIntArray2[1];
							this.FixedPosition = new Point(this._cx, this._cy);
						}
					}
					else
					{
						this.IsFixedPosition = false;
					}
					this.Direction = this.FixedDirection;
				}
				else if (this.MonsterType == MonsterTypes.BloodCastleGateAndCrystal)
				{
					if (Global.GetMapSceneUIClass() == SceneUIClasses.LuoLanChengZhan || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu)
					{
						this.IsFixedDirection = true;
						this.FixedDirection = 4;
						this.Direction = this.FixedDirection;
					}
				}
				else if (this._ExtensionID >= 9000004 && this._ExtensionID <= 9000007)
				{
					this.IsFixedDirection = true;
					this.IsFixedPosition = true;
					this.FixedDirection = 2;
					XElement gameResXml3 = Global.GetGameResXml("Config/HolyGrail.xml");
					XElement xelement3 = Global.GetXElement(gameResXml3, "HolyGrail", "MonsterID", this._ExtensionID.ToString());
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelement3, "PosX");
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement3, "PosY");
					this._cx = xelementAttributeInt;
					this._cy = xelementAttributeInt2;
					this.FixedPosition = new Point(this._cx, this._cy);
					this.Direction = this.FixedDirection;
				}
				else if (Global.IsLangHunYaoSai(this._ExtensionID))
				{
					this.IsFixedDirection = true;
					this.FixedDirection = 4;
					this.Direction = this.FixedDirection;
				}
				else if (Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle && this._ExtensionID >= 9810000 && this._ExtensionID <= 9810044)
				{
					Vector2 compBattleQiZiFixedPosition = Global.GetCompBattleQiZiFixedPosition(this.cx, this.cy);
					fixedPosition = new Point((int)compBattleQiZiFixedPosition.x, (int)compBattleQiZiFixedPosition.y);
					fixedDirection = 6;
					this.isFixed = true;
					this.IsFixedDirection = true;
					this.IsFixedPosition = true;
					this.FixedDirection = fixedDirection;
					this.FixedPosition = fixedPosition;
					this.Direction = this.FixedDirection;
				}
				else
				{
					this.IsFixedDirection = false;
					this.IsFixedPosition = false;
				}
			}
		}

		public int VPK { get; set; }

		public double VLife { get; set; }

		public double VLifeMax { get; set; }

		public int VLevel
		{
			get
			{
				return this._VLevel;
			}
			set
			{
				this._VLevel = value;
			}
		}

		public GSkillMoveType SkillMoveType
		{
			get
			{
				return this._skillMoveType;
			}
			set
			{
				this._skillMoveType = value;
			}
		}

		public NPCTaskStates NPCTaskState { get; set; }

		public long CurrentMagicTicks
		{
			get
			{
				return this._CurrentMagicTicks;
			}
		}

		public int CurrentMagic
		{
			get
			{
				return this._CurrentMagic;
			}
			set
			{
				this._CurrentMagic = value;
				this._CurrentMagicTicks = TimeManager.GetCorrectLocalTime();
			}
		}

		public string LockObject { get; set; }

		public string TargetSrpite { get; set; }

		public Point EnemyTarget { get; set; }

		public bool IsMagicMove { get; set; }

		public bool IsDeath { get; set; }

		public int MagicID { get; set; }

		public ExtActionTypes ExtAction { get; set; }

		public double SpriteMoveSpeed { get; set; }

		public Point Destination { get; set; }

		public string PathString { get; set; }

		public bool IsAutoFindRoad { get; set; }

		public int EquipmentBody { get; set; }

		public int EquipmentWeapon { get; set; }

		public int EquipmentShouHu { get; set; }

		public int EquipmentWing { get; set; }

		public string VTalk { get; set; }

		public bool MaskVisibility { get; set; }

		public SolidColorBrush MaskBrush { get; set; }

		public bool ShadowVisible { get; set; }

		public SolidColorBrush SNameBrush
		{
			get
			{
				return this._SNameBrush;
			}
			set
			{
				this._SNameBrush = value;
				if ((this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.FakeRole) && null != this._RoleInfoDisplay)
				{
					this._RoleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this._SNameBrush.Color);
					Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
				}
			}
		}

		public bool FlagsVisible { get; set; }

		public bool InSameTeam(int roleId)
		{
			bool result = false;
			if (Global.Data.CurrentTeamData != null)
			{
				TeamMemberData teamMemberData = Global.Data.CurrentTeamData.TeamRoles.Find((TeamMemberData a) => roleId == a.RoleID);
				result = (teamMemberData != null);
			}
			return result;
		}

		public int FlagsType
		{
			get
			{
				return this._flagsType;
			}
			set
			{
				this._flagsType = value;
				if (this._RoleInfoDisplay != null)
				{
					string teamSpriteName = string.Empty;
					if (this.InSameTeam(this.RoleID))
					{
						if (value == 1)
						{
							teamSpriteName = "team1";
						}
						else if (value == 2)
						{
							teamSpriteName = "team2";
						}
					}
					this._RoleInfoDisplay.TeamSpriteName = teamSpriteName;
				}
				if (this.roleInfoDisplayObj != null)
				{
					RoleInfoDisplay component = this.roleInfoDisplayObj.GetComponent<RoleInfoDisplay>();
					if (component != null)
					{
						string teamSpriteName2 = string.Empty;
						if (this.InSameTeam(this.RoleID))
						{
							if (this.FlagsType == 1)
							{
								teamSpriteName2 = "team1";
							}
							else if (this.FlagsType == 2)
							{
								teamSpriteName2 = "team2";
							}
						}
						component.TeamSpriteName = teamSpriteName2;
					}
				}
			}
		}

		public string ChengjiuFlag
		{
			set
			{
				if (this._RoleInfoDisplay != null)
				{
					this._RoleInfoDisplay.ChengjiuSpriteFlag = value;
					return;
				}
			}
		}

		public string JunxianFlag
		{
			set
			{
				if (this._RoleInfoDisplay != null)
				{
					this._RoleInfoDisplay.JunxianSpriteName = value;
					return;
				}
			}
		}

		public string LuolanChengZhanFlag
		{
			set
			{
				if (this._RoleInfoDisplay != null)
				{
					this._RoleInfoDisplay.LuoLanSpriteName = value;
					return;
				}
			}
		}

		public string ZhongShenZhiShenFlag
		{
			set
			{
				if (this._RoleInfoDisplay != null)
				{
					this._RoleInfoDisplay.ZhongShenSpriteName = value;
					return;
				}
			}
		}

		public string TitleName
		{
			set
			{
				if (null != this._RoleInfoDisplay)
				{
					this._RoleInfoDisplay.TitleName = value;
					return;
				}
			}
		}

		public bool SNameVisibile { get; set; }

		public bool HSpbarVisibile { get; set; }

		public double TransparentVisible { get; set; }

		public double LifeWidth
		{
			set
			{
				if (this.SpriteType != GSpriteTypes.Leader && this.SpriteType != GSpriteTypes.Other && this.SpriteType != GSpriteTypes.FakeRole)
				{
					if (this.SpriteType == GSpriteTypes.Monster && null != this._MonsterInfoDisplay)
					{
						this._MonsterInfoDisplay.MonsterNameVisible = (this.VLife > 0.0 && this.VLife < this.VLifeMax);
					}
				}
			}
		}

		public double LifeTotalWidth { get; set; }

		public void Start()
		{
			if (this._Started)
			{
				return;
			}
			this._Started = true;
			this._InitStatus = true;
			if (Global.IsInJingLingYaoSai())
			{
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					JingLingMap.inst.ResetCamera();
					return;
				}
				if (this.SpriteType == GSpriteTypes.Other)
				{
					return;
				}
				if (!this.CreateByClient)
				{
					return;
				}
			}
			else if (Global.IsInKuaFuPlunderMainMap())
			{
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					return;
				}
				if (this.SpriteType == GSpriteTypes.Other)
				{
					return;
				}
				if (!this.CreateByClient)
				{
					return;
				}
			}
			if (this.SpriteType == GSpriteTypes.Monster)
			{
				this.mAnimationController = new MonsterStateController(this);
			}
			this.Load3DRes();
			RenderManager.AddObject(this);
		}

		public bool ShowObject()
		{
			if (null == this._the3DGameObject)
			{
				return false;
			}
			if (null != this._the3DGameObject && this._the3DGameObject.activeSelf && !this.isDefault3DRes)
			{
				return false;
			}
			if (!Global.IsShowOtherRole)
			{
				return false;
			}
			if (this.SpriteType == GSpriteTypes.FakeRole && !Global.IsShowFakeRole)
			{
				return false;
			}
			if (this.isDefault3DRes)
			{
				if (this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.FakeRole)
				{
					this.ComposeGameObject();
				}
			}
			else
			{
				this.UpdateRoleLabel(true);
				GSprite.ShowObjectCount++;
				this._the3DGameObject.SetActive(true);
				this._Visiblity = true;
				if (null != this._AnimationMgr && this._AnimationMgr.gSprite == null)
				{
					this._AnimationMgr.gSprite = this;
				}
				if (this.shouHuChongController != null && this.shouHuChongController.gameObject != null)
				{
					this.shouHuChongController.gameObject.SetActive(true);
				}
				Global.Data.GameRadarMap.AddRolePoint(this.RoleID);
				if (Global.Data.roleData.RoleID == this.RoleID)
				{
					if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MountIsRide) == 1)
					{
						this.RoleLoadMount();
					}
				}
				else if (Global.Data.OtherRoles.ContainsKey(this.RoleID) && Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams != null && 52 < Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams.Count && Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams[52] == 1)
				{
					this.RoleLoadMount();
				}
				GActions action = this._Action;
				this._Action = GActions.MaxAction;
				this.Action = action;
				try
				{
					Global.GetStrenthenDecoration1(this, this.RoleID);
					Global.Data.GameScene.AddSpriteAllJueXingTeXiao(this, this.RoleID);
					Global.Data.GameScene.AddChongShengYinJiEffect(this, this.RoleID);
					this.LoadPKloversBuffer(this, this.RoleID);
					this.LoadLingDiBufferDes(this, this.RoleID);
					if (this._AnimationMgr != null)
					{
						if (this._AnimationMgr.PrepareAnimation == null)
						{
							this._AnimationMgr.PrepareAnimation = new AnimationChangeEventHandler(this.PrepareAnimation);
						}
						if (this._AnimationMgr.PlayingAnimation == null)
						{
							this._AnimationMgr.PlayingAnimation = new AnimationChangeEventHandler(this.PlayingAnimation);
						}
						if (this._AnimationMgr.EndAnimation == null)
						{
							this._AnimationMgr.EndAnimation = new AnimationChangeEventHandler(this.EndAnimation);
						}
						if (this._AnimationMgr.gSprite == null)
						{
							this._AnimationMgr.gSprite = this;
						}
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
			}
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
				if (!this._Visiblity && null == this.roleInfoDisplayObj)
				{
					this.UpdateRoleLabel(false);
				}
				return false;
			}
			this.UpdateRoleLabel(false);
			this.ChildrenObjectsDict.Clear();
			Global.Data.GameScene.RemoveAllJueXingTeXiao(this, this.RoleID);
			Global.Data.GameScene.RemoveChongShengYinJiEffect(this, this.RoleID);
			GSprite.ShowObjectCount--;
			this._the3DGameObject.SetActive(false);
			this._Visiblity = false;
			if (this.shouHuChongController != null && this.shouHuChongController.gameObject != null)
			{
				this.shouHuChongController.gameObject.SetActive(false);
			}
			Global.Data.GameRadarMap.RemoveRolePoint(this.RoleID);
			this.RoleUnLoadMount(0);
			return true;
		}

		public bool ShowOthersQiangHuaEffect()
		{
			if (this.SpriteType != GSpriteTypes.Other)
			{
				return false;
			}
			if (this.ChildrenObjectsDict != null && !this.ChildrenObjectsDict.ContainsKey(this.qiangHua20EffectName1) && !this.ChildrenObjectsDict.ContainsKey(this.qiangHua20EffectName2))
			{
				Global.GetStrenthenDecoration1(this, this.RoleID);
				this.SetParentAgain(this.Trans);
			}
			if (this.shouHuChongController != null)
			{
				if (this.shouHuChongController.GameJueXing != null)
				{
					if (!this.shouHuChongController.GameJueXing.activeSelf && ConfigSystemParam.GetSystemParamIntByName("IsOpenJinglingwaixianEffect") == 1L)
					{
						this.shouHuChongController.GameJueXing.SetActive(true);
					}
				}
				else if (Global.Data.OtherRoles.ContainsKey(this.RoleID))
				{
					RoleData roleData = Global.Data.OtherRoles[this.RoleID];
					if (roleData != null && roleData.JingLingYuanSuJueXingData != null && (!Global.Data.SysSetting.HideGameEffect || ConfigSystemParam.GetSystemParamIntByName("IsOpenJinglingwaixianEffect") == 0L))
					{
						bool flag = true;
						JingLingYuanSuShuXingVO jingLingYuanSuShuXinKeyType = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXinKeyType(roleData.JingLingYuanSuJueXingData.ActiveIDs, roleData.JingLingYuanSuJueXingData.ActiveType, out flag);
						int level = jingLingYuanSuShuXinKeyType.Level;
						int minLevel = jingLingYuanSuShuXinKeyType.MinLevel;
						if (flag && minLevel / 4 > 0)
						{
							string text = string.Empty;
							int code = jingLingYuanSuShuXinKeyType.JingLingSpecial.SafeToInt32(0);
							DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(code);
							if (decorationVOByCode != null)
							{
								text = decorationVOByCode.ResName;
							}
							if (!string.IsNullOrEmpty(text))
							{
								string bundleID = MuAssetManager.GetBundleID("Decoration", text);
								GameObject emptyLoader = U3DUtils.GetEmptyLoader("Decoration_JueXing", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
								if (this.shouHuChongController.Categoriy == ItemCategories.ChongWu)
								{
									emptyLoader.transform.localPosition = Vector3.zero;
								}
								Vector3 localScale;
								localScale..ctor(0f, 0f, 0f);
								if (this.shouHuChongController.transform.localScale.x > 0f)
								{
									localScale.x = 1f / this.shouHuChongController.transform.localScale.x;
								}
								if (this.shouHuChongController.transform.localScale.y > 0f)
								{
									localScale.y = 1f / this.shouHuChongController.transform.localScale.y;
								}
								if (this.shouHuChongController.transform.localScale.z > 0f)
								{
									localScale.z = 1f / this.shouHuChongController.transform.localScale.z;
								}
								emptyLoader.transform.localScale = localScale;
								emptyLoader.GetComponent<AssetbundleLoader>().AutoDestroySelf = false;
								U3DUtils.AddChild(this.shouHuChongController.gameObject, emptyLoader, true);
								this.shouHuChongController.GameJueXing = emptyLoader;
							}
						}
					}
				}
			}
			return false;
		}

		public bool HideOthersQiangHuaEffect()
		{
			if (this.SpriteType != GSpriteTypes.Other)
			{
				return false;
			}
			if (this.ChildrenObjectsDict != null)
			{
				if (this.ChildrenObjectsDict.ContainsKey(this.qiangHua20EffectName1))
				{
					IObject @object = this.ChildrenObjectsDict[this.qiangHua20EffectName1];
					if (@object != null)
					{
						this.Remove(@object);
					}
				}
				if (this.ChildrenObjectsDict.ContainsKey(this.qiangHua20EffectName2))
				{
					IObject object2 = this.ChildrenObjectsDict[this.qiangHua20EffectName2];
					if (object2 != null)
					{
						this.Remove(object2);
					}
				}
			}
			if (this.shouHuChongController != null && this.shouHuChongController.GameJueXing != null && this.shouHuChongController.GameJueXing.activeSelf && this.SpriteType == GSpriteTypes.Other && ConfigSystemParam.GetSystemParamIntByName("IsOpenJinglingwaixianEffect") == 1L)
			{
				this.shouHuChongController.GameJueXing.SetActive(false);
			}
			return false;
		}

		public bool ShowChiBang()
		{
			if (this.SpriteType != GSpriteTypes.Other)
			{
				return false;
			}
			if (null == this._the3DGameObject)
			{
				return false;
			}
			if (this.wingsResLoader != null && this.wingsResLoader.GetWingsLoadData != null)
			{
				if (this.chiBangParentObj == null)
				{
					this.chiBangParentObj = U3DUtils.FindGameObjectByName(this.wingsResLoader.GetWingsLoadData.parent, this.wingsResLoader.GetWingsLoadData.hangPointName);
				}
				if (null != this.chiBangParentObj && !this.chiBangParentObj.activeSelf)
				{
					this.chiBangParentObj.SetActive(true);
				}
			}
			else
			{
				if (!this._the3DGameObject.transform.name.Contains(this.RoleID.ToString()))
				{
					return false;
				}
				RoleLoaderData roleLoaderData = new RoleLoaderData();
				this.GetRoleLoaderGoodsDataList(ref roleLoaderData.GoodsDataList, ref this.weaponGoodsDataListChiBang, ref roleLoaderData.wingData);
				if (roleLoaderData.wingData == null)
				{
					return false;
				}
				roleLoaderData.parent = this._the3DGameObject;
				roleLoaderData.ForceSyncLoad = (GSpriteTypes.Leader != this.SpriteType);
				roleLoaderData.HideGameEffect = (this.SpriteType == GSpriteTypes.Other && Global.Data.SysSetting.HideGameEffect);
				roleLoaderData.Occupation = this.Occupation;
				RoleData roleData = null;
				if (this.RoleID == Global.Data.RoleID)
				{
					roleData = Global.Data.roleData;
				}
				else
				{
					Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData);
				}
				if (roleData != null)
				{
					roleLoaderData.SubOccupation = roleData.SubOccupation;
				}
				else
				{
					roleLoaderData.SubOccupation = 0;
				}
				this.Wings = null;
				this.WingsAnimation = null;
				string hangPointName = null;
				try
				{
					GoodsData goodsData = null;
					List<GoodsData> goodsDataList = roleLoaderData.GoodsDataList;
					if (this.SpriteType == GSpriteTypes.Other && roleLoaderData.wingData != null)
					{
						RoleData roleData2 = null;
						if (Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData2))
						{
							int num = 26;
							if (num < roleData2.RoleCommonUseIntPamams.Count)
							{
								int num2 = roleData2.RoleCommonUseIntPamams[num];
								if (num2 > 0)
								{
									if (goodsData == null)
									{
										int @using = roleLoaderData.wingData.Using;
										roleLoaderData.wingData.Using = 1;
										if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out hangPointName))
										{
											Global.ParseWingsGoodsDataInfo(roleLoaderData.wingData, out goodsData, out hangPointName, roleLoaderData.Occupation);
										}
										roleLoaderData.wingData.Using = @using;
									}
									if (goodsData != null)
									{
										goodsData.GoodsID = Global.GetFashionGoodsID(num2);
										goodsData.Using = 1;
									}
								}
								else
								{
									if (roleData2.MyWingData == null || (roleData2.MyWingData != null && roleData2.MyWingData.Using == 0))
									{
										return false;
									}
									if (goodsData == null)
									{
										int using2 = roleLoaderData.wingData.Using;
										roleLoaderData.wingData.Using = 1;
										if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out hangPointName))
										{
											Global.ParseWingsGoodsDataInfo(roleLoaderData.wingData, out goodsData, out hangPointName, roleLoaderData.Occupation);
										}
										roleLoaderData.wingData.Using = using2;
									}
								}
							}
							else if (goodsData == null)
							{
								int using3 = roleLoaderData.wingData.Using;
								roleLoaderData.wingData.Using = 1;
								if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out hangPointName))
								{
									Global.ParseWingsGoodsDataInfo(roleLoaderData.wingData, out goodsData, out hangPointName, roleLoaderData.Occupation);
								}
								roleLoaderData.wingData.Using = using3;
							}
						}
					}
					if (goodsData != null)
					{
						WingsLoadData wingsLoadData = new WingsLoadData();
						wingsLoadData.parent = this._the3DGameObject;
						if (!this._the3DGameObject.transform.name.Contains(this.RoleID.ToString()))
						{
							return false;
						}
						wingsLoadData.HideGameEffect = (this.SpriteType == GSpriteTypes.Other && Global.Data.SysSetting.HideGameEffect);
						wingsLoadData.data = goodsData;
						wingsLoadData.hangPointName = hangPointName;
						this.wingsResLoader = new WingsResLoader(wingsLoadData, new OnWingsLoadComplete(this.WingsLoaderComplete));
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogError<string>(new string[]
					{
						"加载翅膀数据出错"
					});
					MUDebug.LogException(ex);
				}
				return false;
			}
			return false;
		}

		public bool HideChiBang()
		{
			if (this.SpriteType != GSpriteTypes.Other)
			{
				return false;
			}
			if (null == this._the3DGameObject)
			{
				return false;
			}
			if (this.wingsResLoader != null && this.wingsResLoader.GetWingsLoadData != null)
			{
				if (this.chiBangParentObj2 == null)
				{
					this.chiBangParentObj2 = U3DUtils.FindGameObjectByName(this.wingsResLoader.GetWingsLoadData.parent, this.wingsResLoader.GetWingsLoadData.hangPointName);
				}
				if (null != this.chiBangParentObj2 && this.chiBangParentObj2.activeSelf)
				{
					this.chiBangParentObj2.SetActive(false);
				}
			}
			return false;
		}

		public void UpdateRoleLabel(bool isShow = false)
		{
			RoleData roleData;
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				roleData = Global.Data.roleData;
			}
			else if (this.SpriteType == GSpriteTypes.Other)
			{
				roleData = Global.FindRoleDataByID(this.RoleID);
			}
			else
			{
				roleData = null;
			}
			if (this.roleInfoDisplayObj == null && !isShow)
			{
				this.roleInfoDisplayObj = new GameObject(this.Name);
				this.roleInfoDisplayObj.transform.localPosition = this._the3DGameObject.transform.localPosition;
				this.roleInfoDisplayObj.transform.localRotation = this._the3DGameObject.transform.localRotation;
				this.roleInfoDisplayObj.transform.localScale = this._the3DGameObject.transform.localScale;
				RoleInfoDisplay roleInfoDisplay = this.roleInfoDisplayObj.AddComponent<RoleInfoDisplay>();
				roleInfoDisplay.Target = this.Trans;
				roleInfoDisplay.RoleNameText = this.ShowName;
				roleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this._SNameBrush.Color);
				Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
				roleInfoDisplay.BanghuiNameText = this.VFaction;
				roleInfoDisplay.OtherNameText = this.VOtherName;
				roleInfoDisplay.PKKingSpriteName = ((roleData == null) ? null : Global.GetPKKingSpriteName(roleData));
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					roleInfoDisplay.RolePlatform = ((roleData != null) ? roleData.PTID : -1);
				}
				string teamSpriteName = string.Empty;
				if (this.InSameTeam(this.RoleID))
				{
					if (this.FlagsType == 1)
					{
						teamSpriteName = "team1";
					}
					else if (this.FlagsType == 2)
					{
						teamSpriteName = "team2";
					}
				}
				roleInfoDisplay.TeamSpriteName = teamSpriteName;
			}
			else
			{
				if (null != this.roleInfoDisplayObj)
				{
					Object.Destroy(this.roleInfoDisplayObj);
					this.roleInfoDisplayObj = null;
				}
				if (null != this._RoleInfoDisplay)
				{
					this._RoleInfoDisplay.RoleNameText = this.ShowName;
					this._RoleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this._SNameBrush.Color);
					Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
					this._RoleInfoDisplay.BanghuiNameText = this.VFaction;
					this._RoleInfoDisplay.OtherNameText = this.VOtherName;
					this._RoleInfoDisplay.PKKingSpriteName = ((roleData == null) ? null : Global.GetPKKingSpriteName(roleData));
					if (roleData != null)
					{
						this._RoleInfoDisplay.roledataBufferDataList = roleData.BufferDataList;
						this._RoleInfoDisplay.roledataBufferFashionID = roleData.RoleCommonUseIntPamams;
					}
					string teamSpriteName2 = string.Empty;
					if (this.InSameTeam(this.RoleID))
					{
						if (this.FlagsType == 1)
						{
							teamSpriteName2 = "team1";
						}
						else if (this.FlagsType == 2)
						{
							teamSpriteName2 = "team2";
						}
					}
					this._RoleInfoDisplay.TeamSpriteName = teamSpriteName2;
					int num;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						num = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TitleID);
					}
					else
					{
						num = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.TitleID);
					}
					if (num <= 0)
					{
						this._RoleInfoDisplay.TitleName = null;
					}
					if (num > 0)
					{
						string text = "NetImages/GameRes/Images/ChengHao/";
						string text2 = ".png";
						int fashionGoodsID = Global.GetFashionGoodsID(num);
						if (fashionGoodsID == 0)
						{
							this._RoleInfoDisplay.TitleName = null;
						}
						else
						{
							string titleName = string.Concat(new object[]
							{
								text,
								"title_",
								fashionGoodsID,
								text2
							});
							this._RoleInfoDisplay.TitleName = titleName;
						}
						this._RoleInfoDisplay.LuoLanSpriteName = "none";
						this._RoleInfoDisplay.ZhongShenSpriteName = "none";
					}
					string luoLanSpriteName = string.Empty;
					string zhongShenSpriteName = string.Empty;
					RoleData roleData2;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						roleData2 = Global.Data.roleData;
					}
					else
					{
						roleData2 = Global.Data.OtherRoles.GetValue(this.RoleID);
					}
					if (roleData2 != null && Global.IsBufferExist(111, roleData2))
					{
						zhongShenSpriteName = "zhongshenZhiShen";
						this._RoleInfoDisplay.ZhongShenSpriteName = zhongShenSpriteName;
					}
					if (roleData2 != null && Global.IsBufferExist(103, roleData2))
					{
						if (Global.IsBangHuiLeader(roleData2, roleData2.Faction))
						{
							luoLanSpriteName = "langhunChengzhu";
						}
						else
						{
							luoLanSpriteName = "langhunGuizu";
						}
						this._RoleInfoDisplay.LuoLanSpriteName = luoLanSpriteName;
					}
					else if (Global.Data.roleData != null && Global.Data.roleData.BangHuiLingDiItemsDict != null && Global.Data.roleData.BangHuiLingDiItemsDict[7] != null)
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = Global.Data.roleData.BangHuiLingDiItemsDict[7];
						if (roleData2 != null && bangHuiLingDiItemData.BHID == roleData2.Faction && roleData2.Faction > 0)
						{
							if (Global.IsBangHuiLeader(roleData2, roleData2.Faction))
							{
								luoLanSpriteName = "luolanChengzhu";
							}
							else
							{
								luoLanSpriteName = "luolanGuizhu";
							}
						}
						this._RoleInfoDisplay.LuoLanSpriteName = luoLanSpriteName;
					}
				}
				this.roleInfoDisplayObj = null;
			}
		}

		protected void RefreshRolePetTrans()
		{
			if (null != this.shouHuChongController)
			{
				PetFollow component = this.shouHuChongController.GetComponent<PetFollow>();
				if (null != component)
				{
					if (this.OnHorseEX)
					{
						if (this.HorseController != null && null != this.HorseController.HorseTrans)
						{
							component.target = this.HorseController.HorseTrans;
						}
						if (this.shouHuChongController.Categoriy == ItemCategories.ChongWu)
						{
							component.offsetX = -1f;
							component.offsetY = 0f;
							component.offsetZ = 0f;
						}
						else if (this.shouHuChongController.Categoriy == ItemCategories.ShouHuChong)
						{
							component.offsetX = 0f;
							component.offsetY = 2f;
							component.offsetZ = -0.5f;
						}
					}
					else
					{
						if (null != this.The3DGameObject)
						{
							component.target = this.The3DGameObject.transform;
						}
						if (this.shouHuChongController.Categoriy == ItemCategories.ChongWu)
						{
							component.offsetX = 0f;
							component.offsetY = 0f;
							component.offsetZ = 0f;
						}
						else if (this.shouHuChongController.Categoriy == ItemCategories.ShouHuChong)
						{
							component.offsetX = 0f;
							component.offsetY = 2f;
							component.offsetZ = -0.5f;
						}
					}
				}
			}
		}

		private void RefreshRoleInfoDisplayObjTrans()
		{
			GameObject gameObject = this.roleInfoDisplayObj;
			RoleInfoDisplay roleInfoDisplay = null;
			if (null == gameObject)
			{
				if (null != this.The3DGameObject)
				{
					roleInfoDisplay = this.The3DGameObject.GetComponent<RoleInfoDisplay>();
				}
			}
			else
			{
				roleInfoDisplay = gameObject.GetComponent<RoleInfoDisplay>();
			}
			if (null != roleInfoDisplay)
			{
				if (null != this.Trans)
				{
					roleInfoDisplay.Target = this.Trans;
				}
				if (this.OnHorseEX)
				{
					float[] horseDecoH = Global.GetHorseDecoH(this.HorseController.HorseID);
					roleInfoDisplay.ChangeUIFollowTargetHorseHeght((horseDecoH.Length != 3) ? -0.11f : horseDecoH[2]);
				}
				else
				{
					roleInfoDisplay.ChangeUIFollowTargetHorseHeght(0.02f);
				}
			}
		}

		public bool CurrentObjectState
		{
			get
			{
				return !(null == this._the3DGameObject) && this._the3DGameObject.activeSelf;
			}
		}

		private void ReleaseResLoader()
		{
			if (this.roleResLoader != null)
			{
				this.roleResLoader.Stop();
				this.roleResLoader = null;
			}
			if (this.wingsResLoader != null)
			{
				this.wingsResLoader.Stop();
				this.wingsResLoader = null;
			}
			if (this.shouHuChongResLoader != null)
			{
				this.shouHuChongResLoader.Stop();
				this.shouHuChongResLoader = null;
			}
			if (this.weaponResLoader != null)
			{
				this.weaponResLoader.Stop();
				this.weaponResLoader = null;
			}
			if (this.monsterNPCResLoader != null)
			{
				this.monsterNPCResLoader.Stop();
				this.monsterNPCResLoader = null;
			}
			if (this.junqiResLoader != null)
			{
				this.junqiResLoader.Stop();
				this.junqiResLoader = null;
			}
			this.RoleUnLoadMount(1);
		}

		public void Destroy()
		{
			RenderManager.RemoveObject(this);
			this.ReleaseResLoader();
			if (this.roleInfoDisplayObj != null)
			{
				Object.Destroy(this.roleInfoDisplayObj);
			}
			if (this.SpriteType == GSpriteTypes.Monster)
			{
				if (this.LoadReal3DResOK)
				{
					MonsterCachingManager.ReleaseCachingItem(this.ExtensionID);
				}
				ConfigMonsters.RemoveMonsterPlaySound(this.ExtensionID);
				if (this.mAnimationController != null)
				{
					this.mAnimationController.ExitState();
					this.mAnimationController = null;
				}
			}
			this.DestroyShouHuChong();
			this.ClearAllNoParentChildrenObjects();
			if (this.HorseController != null)
			{
				this.HorseController.Destorty();
			}
			if (null != this._the3DGameObject)
			{
				Object.Destroy(this._the3DGameObject);
				this._the3DGameObject = null;
				this._AnimationMgr = null;
				this.Trans = null;
			}
			this.WeaponList = null;
			this.NotSafeEmptyNamesList = null;
			this.SafeEmptyNamesList = null;
			this.Wings = null;
			this.WingsAnimator = null;
			this.FreeStoppedSound();
			this.GDecorationEmblem = null;
		}

		public IObject FindName(string name)
		{
			if (this.ChildrenObjectsDict.ContainsKey(name))
			{
				return this.ChildrenObjectsDict[name];
			}
			return null;
		}

		public void Add(IObject obj)
		{
			if (!(obj is GDecoration))
			{
				Global.AssertException(false, string.Format(Global.GetLang("GSprite.Add 函数不能添加非GDecoration对象!"), new object[0]));
				return;
			}
			if (this.ChildrenObjectsDict.ContainsKey(obj.Name))
			{
				Global.AssertException(false, string.Format(Global.GetLang("GSprite.Add 函数执行重复的对象添加!"), new object[0]));
				return;
			}
			this.ChildrenObjectsDict[obj.Name] = obj;
			obj.Parent = this.Trans;
			obj.Tag = this;
		}

		public void Add(IObject obj, string emptyObjectName)
		{
			if (!(obj is GDecoration))
			{
				Global.AssertException(false, string.Format(Global.GetLang("GSprite.Add 函数不能添加非GDecoration对象!"), new object[0]));
				return;
			}
			if (this.ChildrenObjectsDict.ContainsKey(obj.Name))
			{
				Global.AssertException(false, string.Format(Global.GetLang("GSprite.Add 函数执行重复的对象添加!"), new object[0]));
				return;
			}
			this.ChildrenObjectsDict[obj.Name] = obj;
			if (null != this._the3DGameObject)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(this._the3DGameObject, emptyObjectName);
				if (null == gameObject)
				{
					obj.Destroy();
					obj = null;
					Global.AssertException(false, string.Format(Global.GetLang("GSprite.Add 函数执行添加到命名的空对象时，空对象不存在!"), new object[0]));
					return;
				}
				obj.Parent = gameObject.transform;
			}
			obj.Tag = this;
		}

		public void Remove(IObject obj)
		{
			if (!this.ChildrenObjectsDict.ContainsKey(obj.Name))
			{
				return;
			}
			this.ChildrenObjectsDict.Remove(obj.Name);
			obj.Destroy();
			obj = null;
		}

		public void Remove(string fuzzyName)
		{
			if (string.IsNullOrEmpty(fuzzyName))
			{
				return;
			}
			string text = null;
			foreach (KeyValuePair<string, IObject> keyValuePair in this.ChildrenObjectsDict)
			{
				string key = keyValuePair.Key;
				if (this.ChildrenObjectsDict[key].The3DGameObject != null && this.ChildrenObjectsDict[key].The3DGameObject.transform.childCount > 0 && this.ChildrenObjectsDict[key].The3DGameObject.transform.GetChild(0).name.Contains(fuzzyName))
				{
					text = key;
				}
				if (key.Contains(fuzzyName))
				{
					text = key;
					break;
				}
			}
			if (text != null && this.ChildrenObjectsDict.ContainsKey(text))
			{
				this.ChildrenObjectsDict[text].Destroy();
				this.ChildrenObjectsDict.Remove(text);
			}
		}

		private void SetParentAgain(Transform parent)
		{
			foreach (KeyValuePair<string, IObject> keyValuePair in this.ChildrenObjectsDict)
			{
				IObject value = keyValuePair.Value;
				value.Parent = parent;
				value.Tag = this;
				if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other)
				{
					IObject obj = value;
					Dictionary<string, IObject>.Enumerator enumerator;
					KeyValuePair<string, IObject> keyValuePair2 = enumerator.Current;
					this.ClickDeco(obj, keyValuePair2.Key);
				}
			}
		}

		private void ClickDeco(IObject obj, string Key)
		{
			if (obj != null)
			{
				if ("DelayDecoration_15155" == Key)
				{
					return;
				}
				float[] horseDecoH = Global.GetHorseDecoH(this.HorseController.HorseID);
				if ("DelayDecoration_14000" == Key)
				{
					if (Global.Data.SysSetting.HideGameEffect)
					{
						return;
					}
					if (obj is GDecoration)
					{
						GDecoration gdecoration = obj as GDecoration;
						if (gdecoration != null && 0 < this.HorseController.HorseID && gdecoration.The3DGameObject != null)
						{
							Vector3 localPosition = gdecoration.The3DGameObject.transform.localPosition;
							localPosition.y = 0f;
							if (this.OnHorseEX && horseDecoH != null && 0 < horseDecoH.Length)
							{
								localPosition.y = horseDecoH[0];
							}
							gdecoration.The3DGameObject.transform.localPosition = localPosition;
						}
					}
				}
				else if (obj is GDecoration)
				{
					GDecoration gdecoration2 = obj as GDecoration;
					if (gdecoration2 != null)
					{
						if (gdecoration2.Equals(this.mGDecorationEmblem))
						{
							this.mGDecorationEmblem.Parent = this.Trans;
							if (null != this.mGDecorationEmblem.The3DGameObject)
							{
								Vector3 localPosition2 = this.mGDecorationEmblem.The3DGameObject.transform.localPosition;
								localPosition2.y = 0f;
								if (this.OnHorseEX && horseDecoH != null && 1 < horseDecoH.Length)
								{
									localPosition2.y = horseDecoH[1];
								}
								this.mGDecorationEmblem.The3DGameObject.transform.localPosition = localPosition2;
							}
						}
						else if (gdecoration2.HangPos == 1 && null != gdecoration2.The3DGameObject)
						{
							Vector3 localPosition3 = gdecoration2.The3DGameObject.transform.localPosition;
							localPosition3.y = 0f;
							if (!gdecoration2.ToGround && this.OnHorseEX)
							{
								localPosition3.y = 0.8f;
							}
							gdecoration2.The3DGameObject.transform.localPosition = localPosition3;
						}
						if (gdecoration2.The3DGameObject != null && gdecoration2.The3DGameObject.transform.name.Contains("_ChongShengYinJiRootEffect"))
						{
							float[] horseYinJiDecoH = Global.GetHorseYinJiDecoH(this.HorseController.HorseID);
							Transform transform = gdecoration2.The3DGameObject.transform.FindChild("YinJi_effect(Clone)");
							if (this.OnHorseEX)
							{
								if (horseYinJiDecoH.Length >= 3)
								{
									gdecoration2.The3DGameObject.transform.localPosition = new Vector3(horseYinJiDecoH[0], horseYinJiDecoH[1], horseYinJiDecoH[2]);
								}
								else
								{
									gdecoration2.The3DGameObject.transform.localPosition = new Vector3(0.12f, 1.83f, 0.12f);
								}
							}
							else
							{
								gdecoration2.The3DGameObject.transform.localPosition = new Vector3(0f, 1f, 0f);
							}
						}
					}
				}
			}
		}

		private void ClearAllNoParentChildrenObjects()
		{
			foreach (KeyValuePair<string, IObject> keyValuePair in this.ChildrenObjectsDict)
			{
				keyValuePair.Value.Destroy();
			}
			this.ChildrenObjectsDict.Clear();
		}

		public void OnFrameRender()
		{
			if (!Global.IsInJingLingMap())
			{
				if (null != this.Trans && this._Rotation != this.Trans.rotation)
				{
					this.Trans.rotation = Quaternion.Slerp(this.Trans.rotation, this._Rotation, Time.deltaTime * 15f);
				}
			}
			if (this.AnimationController != null)
			{
				this.AnimationController.Update();
			}
			if (this.NpcStateMgr != null)
			{
				this.NpcStateMgr.Update();
			}
			int num = (int)TmskTime.DeltaMills();
			if (this.MovingSpeedMills > 0)
			{
				this.DecMovingSpeedMills(num);
			}
			this.DecDongJieAndFaint(num);
			if (this.SpriteType == GSpriteTypes.Monster && this.VLife <= 0.0 && Global.Data.roleData.MapCode / 1000 == 6)
			{
				this.ForceToQuickFall();
			}
			if (this.SkillMoveType == GSkillMoveType.Rotate && this.RotateSkillTypeTimes > 0)
			{
				this.RotateSkillTypeTimes -= num;
				if (this.RotateSkillTypeTimes <= 0)
				{
					this.SkillMoveType = GSkillMoveType.None;
				}
			}
			if (this.ProcessDead())
			{
				return;
			}
			this.ProcessIdleAction();
			this.ProcessMonsterRunAction();
			if (Time.frameCount % 10 == 0)
			{
				this.ForceToGround();
				this.RandomPlaySpriteSound(false);
				this.PlaySpriteSoundSlotTime();
			}
			Global.SetNearRoleShow(this);
			if ((this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other) && this._HorseController != null)
			{
				this._HorseController.OnReameRender();
			}
		}

		public GSprite Pet
		{
			get
			{
				return this._Pet;
			}
			set
			{
				this._Pet = value;
			}
		}

		public GSprite BiaoChe
		{
			get
			{
				return this._BiaoChe;
			}
			set
			{
				this._BiaoChe = value;
			}
		}

		public long LastInjuredTicks { get; set; }

		public long GetElapsedLastAttackTicks()
		{
			return TimeManager.GetCorrectLocalTime() - this.LastAttackTicks;
		}

		public long LastActionTicks
		{
			get
			{
				return this._LastActionTicks;
			}
		}

		public bool CanAttack()
		{
			return true;
		}

		public bool CanMagic()
		{
			return true;
		}

		public bool CanMove()
		{
			return true;
		}

		public bool CanMoveMap()
		{
			return true;
		}

		public void ExternalDeath()
		{
			if (this.Action != GActions.Death)
			{
				this.Action = GActions.Death;
			}
			this.ProcessDead();
		}

		public int OldGridX
		{
			get
			{
				return this._OldGridX;
			}
			set
			{
				this._OldGridX = value;
			}
		}

		public int OldGridY
		{
			get
			{
				return this._OldGridY;
			}
			set
			{
				this._OldGridY = value;
			}
		}

		public int[] EfficaciousSection { get; set; }

		public Point ShadowOffset { get; set; }

		public int SpriteWidth { get; set; }

		public int SpriteHeight { get; set; }

		public string StallName { get; set; }

		public bool SkillWordVisible { get; set; }

		public string SkillWordImageURL { get; set; }

		public bool LittleVIPVisible { get; set; }

		public string LittleVIPImageURL { get; set; }

		public bool LingDiWordVisible { get; set; }

		public string LingDiWordImageURL { get; set; }

		public bool JingMaiWordVisible { get; set; }

		public string JingMaiWordImageURL { get; set; }

		public bool BattleNameVisible { get; set; }

		public string BattleNameImageURL { get; set; }

		public bool HeroIndexNameVisible { get; set; }

		public string HeroIndexNameImageURL { get; set; }

		public bool ChuanQiJingMaiVisible { get; set; }

		public string ChuanQiJingMaiImageURL { get; set; }

		public bool ChanQiWuXueVisible { get; set; }

		public string ChanQiWuXueImageURL { get; set; }

		public bool HuangChengNameVisible { get; set; }

		public string HuangChengNameImageURL { get; set; }

		public bool JieriWordVisible { get; set; }

		public string JieriWordImageURL { get; set; }

		public void CheckAttackAction()
		{
			if (this.SpriteType != GSpriteTypes.Monster)
			{
				return;
			}
			if (this.Action == GActions.PreAttack)
			{
				WrapMode wrapMode = 1;
				this.Action = GActions.Attack;
				string actionName = Global.GetActionName(this, this.Action, this.IsInSafeRegion, out wrapMode);
				this._AnimationMgr.ChangeAnimation(actionName, wrapMode, false, this, 0.3f);
			}
		}

		public GameObject Find3DObject(string name)
		{
			if (null == this._the3DGameObject)
			{
				return null;
			}
			return U3DUtils.FindGameObjectByName(this._the3DGameObject, name);
		}

		private Vector3 GetGroundPos(float x, float z, float y = 0f)
		{
			return Global.GetGroundPos(this._LeaderInfo, this.SpriteType, x, z, y);
		}

		private void ForceToGround()
		{
			if (this.SpriteType != GSpriteTypes.NPC)
			{
				return;
			}
			if (null == this.Trans)
			{
				return;
			}
			if (this.tempX == this.Trans.position.x && this.tempY == this.Trans.position.y)
			{
				return;
			}
			Vector3 groundPos = this.GetGroundPos(this.Trans.position.x, this.Trans.position.z, 50f);
			if (groundPos != Vector3.zero)
			{
				this.Trans.localPosition = Vector3.Slerp(this.Trans.localPosition, groundPos, 0.05f);
				this.tempX = this.Trans.localPosition.x;
				this.tempY = this.Trans.localPosition.y;
			}
		}

		private void ForceToQuickFall()
		{
			if (null == this.Trans)
			{
				return;
			}
			if (!Global.OnObstruction(this.Coordinate, Global.CurrentMapData))
			{
				return;
			}
			this.Trans.localPosition = new Vector3(this.Trans.localPosition.x, this.Trans.localPosition.y - Time.deltaTime * 5f, this.Trans.localPosition.z);
		}

		private void Move3D(Vector3 offset, Vector3 newPos)
		{
			if (null == this.Trans)
			{
				return;
			}
			this.Trans.localPosition += offset;
			if (this.SpriteType != GSpriteTypes.Leader)
			{
				float num = 0.2f;
				if (this.SpriteType == GSpriteTypes.Monster)
				{
					num = 0.5f;
				}
				if (Mathf.Abs(this.Trans.localPosition.x - this.tempX) < num && Mathf.Abs(this.Trans.localPosition.z - this.tempY) < num)
				{
					return;
				}
			}
			Vector3 groundPos = this.GetGroundPos(this.Trans.position.x, this.Trans.position.z, 50f);
			if (groundPos != Vector3.zero)
			{
				if (this.ExtensionID == 9610000 || this.ExtensionID == 9610001)
				{
					this.Trans.localPosition = new Vector3(this.Trans.localPosition.x, groundPos.y + 0.65f, this.Trans.localPosition.z);
				}
				else
				{
					this.Trans.localPosition = new Vector3(this.Trans.localPosition.x, groundPos.y, this.Trans.localPosition.z);
				}
				this.tempY = this.Trans.localPosition.z;
				this.tempX = this.Trans.localPosition.x;
			}
		}

		private bool LoadReal3DResOK
		{
			get
			{
				return this.mLoadReal3DResOK;
			}
			set
			{
				this.mLoadReal3DResOK = value;
				if (this.mLoadReal3DResOK)
				{
					this.GDecorationEmblem = this.GDecorationEmblem;
					this.GDecorationEmblemShan = this.GDecorationEmblemShan;
				}
			}
		}

		public bool Load3DResOK
		{
			get
			{
				return this.mLoadReal3DResOK;
			}
		}

		public bool IsDefault3DRes
		{
			get
			{
				return this.isDefault3DRes;
			}
		}

		private void Load3DRes()
		{
			if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.FakeRole)
			{
				if (this.SpriteType == GSpriteTypes.Other && this.IsInSafeRegion)
				{
					this.LoadDefaultOtherRole();
				}
				else
				{
					this.ComposeGameObject();
				}
			}
			else if (this.SpriteType == GSpriteTypes.Monster)
			{
				this.LoadMonsterRoot();
				this.LoadMonsterBody();
			}
			else if (this.SpriteType == GSpriteTypes.NPC)
			{
				this.ComposeNPC();
			}
			else if (this.SpriteType != GSpriteTypes.Pet)
			{
				if (this.SpriteType == GSpriteTypes.JunQi)
				{
					this.ComposeJunQi();
				}
				else if (this.SpriteType == GSpriteTypes.BiaoChe)
				{
				}
			}
		}

		private void LoadDefaultOtherRole()
		{
			GameObject gameObject = U3DUtils.LoadDefaultObject();
			if (gameObject != null)
			{
				GSprite.ShowObjectCount--;
				Object.Destroy(this._the3DGameObject);
				this._the3DGameObject = null;
				this._the3DGameObject = gameObject;
				this.Trans = this._the3DGameObject.transform;
				this._the3DGameObject.AddComponent<SkinnedMeshRenderer>();
				U3DUtils.AddBoxCollider(gameObject, new Vector3(0f, 1.2f, 0f), new Vector3(0.5f, 2.4f, 0.5f), true);
				OwnerTypeManager ownerTypeManager = gameObject.AddComponent<OwnerTypeManager>();
				ownerTypeManager.OwnerObject = this;
				Rigidbody rigidbody = this._the3DGameObject.AddComponent<Rigidbody>();
				rigidbody.isKinematic = true;
				Vector3 groundPos = this.GetGroundPos((float)this._cx / 100f, (float)this._cy / 100f, 50f);
				this.Trans.localPosition = new Vector3((float)this._cx / 100f, groundPos.y, (float)this._cy / 100f);
				RoleInfoDisplay roleInfoDisplay = gameObject.AddComponent<RoleInfoDisplay>();
				roleInfoDisplay.Target = gameObject.transform;
				roleInfoDisplay.RoleNameText = this.ShowName;
				roleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this._SNameBrush.Color);
				Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
				roleInfoDisplay.BanghuiNameText = this.VFaction;
				roleInfoDisplay.OtherNameText = this.VOtherName;
				roleInfoDisplay.PKKingSpriteName = this.PKKingSpriteName;
				if (Global.Data.roleData.Faction == this.FactionID)
				{
					roleInfoDisplay.BanghuiNameColor = ColorSL.ParseArgb(ColorSL.FromArgb(255, 0, 255, 0));
				}
				else
				{
					roleInfoDisplay.BanghuiNameColor = ColorSL.ParseArgb(ColorSL.FromArgb(255, 153, 204, 255));
				}
				string teamSpriteName = string.Empty;
				if (this.InSameTeam(this.RoleID))
				{
					if (this.FlagsType == 1)
					{
						teamSpriteName = "team1";
					}
					else if (this.FlagsType == 2)
					{
						teamSpriteName = "team2";
					}
				}
				roleInfoDisplay.TeamSpriteName = teamSpriteName;
				int roleCommonUseParamsValueForOtherRole = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.ChengJiuLevel);
				if (roleCommonUseParamsValueForOtherRole > 0)
				{
					roleInfoDisplay.ChengjiuSpriteFlag = string.Format("chengjiu{0}", roleCommonUseParamsValueForOtherRole);
				}
				int roleCommonUseParamsValueForOtherRole2 = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.ShengWangLevel);
				if (roleCommonUseParamsValueForOtherRole2 > 0)
				{
					roleInfoDisplay.JunxianSpriteName = StringUtil.substitute("junxian{0}", new object[]
					{
						roleCommonUseParamsValueForOtherRole2
					});
				}
				RoleData roleData = null;
				if (Global.Data.OtherRoles.ContainsKey(this.RoleID))
				{
					roleData = Global.Data.OtherRoles.GetValue(this.RoleID);
					roleInfoDisplay.roledataBufferDataList = roleData.BufferDataList;
					roleInfoDisplay.roledataBufferFashionID = roleData.RoleCommonUseIntPamams;
				}
				if (roleData != null)
				{
					int roleCommonUseParamsValueForOtherRole3 = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.TitleID);
					if (roleCommonUseParamsValueForOtherRole3 <= 0)
					{
						roleInfoDisplay.TitleName = null;
					}
					else
					{
						string text = "NetImages/GameRes/Images/ChengHao/";
						string text2 = ".png";
						int fashionGoodsID = Global.GetFashionGoodsID(roleCommonUseParamsValueForOtherRole3);
						if (fashionGoodsID == 0)
						{
							roleInfoDisplay.TitleName = null;
						}
						else
						{
							string titleName = string.Concat(new object[]
							{
								text,
								"title_",
								fashionGoodsID,
								text2
							});
							roleInfoDisplay.TitleName = titleName;
						}
						roleInfoDisplay.LuoLanSpriteName = "none";
						roleInfoDisplay.ZhongShenSpriteName = "none";
					}
				}
				string luoLanSpriteName = string.Empty;
				string zhongShenSpriteName = string.Empty;
				if (roleData != null && Global.IsBufferExist(111, roleData))
				{
					zhongShenSpriteName = "zhongshenZhiShen";
					roleInfoDisplay.ZhongShenSpriteName = zhongShenSpriteName;
				}
				if (roleData != null && Global.IsBufferExist(103, roleData))
				{
					if (Global.IsBangHuiLeader(roleData, roleData.Faction))
					{
						luoLanSpriteName = "langhunChengzhu";
					}
					else
					{
						luoLanSpriteName = "langhunGuizu";
					}
					roleInfoDisplay.LuoLanSpriteName = luoLanSpriteName;
				}
				else if (Global.Data.roleData != null && Global.Data.roleData.BangHuiLingDiItemsDict != null && Global.Data.roleData.BangHuiLingDiItemsDict[7] != null)
				{
					BangHuiLingDiItemData bangHuiLingDiItemData = Global.Data.roleData.BangHuiLingDiItemsDict[7];
					if (roleData != null && roleData.Faction != 0 && bangHuiLingDiItemData.BHID == roleData.Faction)
					{
						if (Global.IsBangHuiLeader(roleData, roleData.Faction))
						{
							luoLanSpriteName = "luolanChengzhu";
						}
						else
						{
							luoLanSpriteName = "luolanGuizhu";
						}
					}
					roleInfoDisplay.LuoLanSpriteName = luoLanSpriteName;
				}
				this._RoleInfoDisplay = roleInfoDisplay;
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
				{
					this.JunxianFlag = "none";
					this._RoleInfoDisplay.PKKingSpriteName = "none";
					this.ChengjiuFlag = "none";
					this.LuolanChengZhanFlag = "none";
				}
				this.HeadDispText = gameObject.AddComponent<HeadDisplayText>();
				this.HeadDispText.Target = gameObject.transform;
				gameObject.name = this.Name;
				this.IsFlying = false;
				U3DUtils.ReplaceLayerInChildren(gameObject, LayerMask.NameToLayer(this._DefaultLayerName), null);
				this.MeshSize = U3DUtils.GetMeshSize(gameObject.transform);
				Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
				this.Trans.rotation = quaternionByDir;
				this.SetParentAgain(this.Trans);
				this.isDefault3DRes = true;
			}
		}

		private bool GetRoleLoaderGoodsDataList(ref List<GoodsData> GoodsDataList, ref List<GoodsData> weaponGoodsDataList, ref WingData wingData)
		{
			GoodsData[] array = null;
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				try
				{
					if (Global.IsBattleMap() && Global.Data.roleData.BattleWhichSide >= 1)
					{
						GoodsDataList = Global.GetBattleEquipGoodsDataList(this.BattleWhichSide, this.Occupation);
						weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(this.BattleWhichSide, this.Occupation);
						GoodsDataList.AddRange(weaponGoodsDataList);
					}
					else if (Global.GetMapSceneUIClass() == SceneUIClasses.PKKing)
					{
						GoodsDataList = Global.GetPKKingEquipGoodsDataList(1, this.Occupation);
						weaponGoodsDataList = Global.GetPKKingWeaponGoodsDataList(1, this.Occupation);
						GoodsDataList.AddRange(weaponGoodsDataList);
					}
					else if (Global.CanGiveFakeEquips)
					{
						GoodsDataList = Global.GetTopLevelEquipGoodsDataList(this.Occupation, 5, 13);
						weaponGoodsDataList = Global.GetTopLevelWeaponGoodsData(this.Occupation, 5, 13);
						GoodsDataList.AddRange(weaponGoodsDataList);
					}
					else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
					{
						GoodsDataList = Global.GetBattleEquipGoodsDataList(2, this.Occupation);
						weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(2, this.Occupation);
						GoodsDataList.AddRange(weaponGoodsDataList);
					}
					else if (Global.InZhanMengLianSaiScene() && !Global.IsCompetitionGuanKan)
					{
						GoodsDataList = Global.GetBattleEquipGoodsDataList(Global.Data.roleData.BattleWhichSide, Global.Data.roleData.Occupation);
						weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(Global.Data.roleData.BattleWhichSide, Global.Data.roleData.Occupation);
						GoodsDataList.AddRange(weaponGoodsDataList);
					}
					else if (Global.IsInKuaFuPlunderBattleMap())
					{
						if (Global.Data.roleData.BattleWhichSide == 1)
						{
							GoodsDataList = Global.GetPlunderBattleEquipGoodsDataList(1, this.Occupation);
							weaponGoodsDataList = Global.GetPlunderBattleWeaponGoodsDataList(1, this.Occupation);
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						else
						{
							GoodsDataList = Global.GetPlunderBattleEquipGoodsDataList(2, this.Occupation);
							weaponGoodsDataList = Global.GetPlunderBattleWeaponGoodsDataList(2, this.Occupation);
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
					}
					else if (Global.IsFashioned(out array))
					{
						this.DealWithUserFashion(Global.Data.roleData, array, out GoodsDataList, out weaponGoodsDataList);
						if (0 < weaponGoodsDataList.Count)
						{
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						wingData = Global.Data.roleData.MyWingData;
					}
					else if (SceneUIClasses.RebornMap.IsTheScene())
					{
						if (Global.Data != null && Global.Data.roleData != null)
						{
							this.GetRoleReborithUsingGoods(Global.Data.roleData.RebornGoodsDataList, Global.CheckRoleOcc(Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation), out GoodsDataList, out weaponGoodsDataList);
						}
						if (GoodsDataList != null && weaponGoodsDataList != null)
						{
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						wingData = Global.Data.roleData.MyWingData;
					}
					else
					{
						if (Global.Data.roleData.RebornShowEquip == 1)
						{
							this.GetRoleReborithUsingGoods(Global.Data.roleData.RebornGoodsDataList, Global.CheckRoleOcc(Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation), out GoodsDataList, out weaponGoodsDataList);
							if (GoodsDataList != null)
							{
								if (weaponGoodsDataList != null)
								{
									GoodsDataList.AddRange(weaponGoodsDataList);
								}
							}
							else if (weaponGoodsDataList != null)
							{
								GoodsDataList.AddRange(weaponGoodsDataList);
								GoodsDataList = new List<GoodsData>();
							}
						}
						else
						{
							GoodsDataList = Global.Data.roleData.GoodsDataList;
						}
						wingData = Global.Data.roleData.MyWingData;
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
			}
			else if (this.SpriteType == GSpriteTypes.Other)
			{
				try
				{
					RoleData roleData = null;
					if (Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData))
					{
						if (Global.IsBattleMap() && Global.Data.roleData.BattleWhichSide >= 1)
						{
							GoodsDataList = Global.GetBattleEquipGoodsDataList(roleData.BattleWhichSide, roleData.Occupation);
							weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(roleData.BattleWhichSide, roleData.Occupation);
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						else if (Global.GetMapSceneUIClass() == SceneUIClasses.PKKing)
						{
							GoodsDataList = Global.GetPKKingEquipGoodsDataList(1, this.Occupation);
							weaponGoodsDataList = Global.GetPKKingWeaponGoodsDataList(1, this.Occupation);
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						else if (Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunDong || Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
						{
							if (Global.Data.roleData.BattleWhichSide == roleData.BattleWhichSide)
							{
								GoodsDataList = Global.GetBattleEquipGoodsDataList(2, this.Occupation);
								weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(2, this.Occupation);
							}
							else
							{
								GoodsDataList = Global.GetBattleEquipGoodsDataList(1, this.Occupation);
								weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(1, this.Occupation);
							}
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						else if (Global.InZhanMengLianSaiScene())
						{
							GoodsDataList = Global.GetBattleEquipGoodsDataList(roleData.BattleWhichSide, roleData.Occupation);
							weaponGoodsDataList = Global.GetOtherBattleWeaponGoodsDataListInZhanMengLianSai(roleData);
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						else if (Global.IsInKuaFuPlunderBattleMap())
						{
							if (roleData.BattleWhichSide == 1)
							{
								GoodsDataList = Global.GetPlunderBattleEquipGoodsDataList(1, roleData.Occupation);
								weaponGoodsDataList = Global.GetPlunderBattleWeaponGoodsDataList(1, roleData.Occupation);
								GoodsDataList.AddRange(weaponGoodsDataList);
							}
							else if (Global.Data.roleData.BattleWhichSide == roleData.BattleWhichSide)
							{
								GoodsDataList = Global.GetPlunderBattleEquipGoodsDataList(2, roleData.Occupation);
								weaponGoodsDataList = Global.GetPlunderBattleWeaponGoodsDataList(2, roleData.Occupation);
								GoodsDataList.AddRange(weaponGoodsDataList);
							}
							else
							{
								GoodsDataList = Global.GetPlunderBattleEquipGoodsDataList(3, roleData.Occupation);
								weaponGoodsDataList = Global.GetPlunderBattleWeaponGoodsDataList(3, roleData.Occupation);
								GoodsDataList.AddRange(weaponGoodsDataList);
							}
						}
						else if (Global.IsOtherFashioned(roleData, out array))
						{
							this.DealWithUserFashion(roleData, array, out GoodsDataList, out weaponGoodsDataList);
							if (0 < weaponGoodsDataList.Count)
							{
								GoodsDataList.AddRange(weaponGoodsDataList);
							}
							List<GoodsData> list = null;
							this.DealWithPet(roleData, out list);
							if (list != null && 0 < list.Count)
							{
								for (int i = 0; i < list.Count; i++)
								{
									if (!GoodsDataList.Contains(list[i]))
									{
										GoodsDataList.Add(list[i]);
									}
								}
							}
							wingData = roleData.MyWingData;
						}
						else if (SceneUIClasses.RebornMap.IsTheScene())
						{
							this.GetRoleReborithUsingGoods(roleData.RebornGoodsDataList, Global.CheckRoleOcc(roleData.Occupation, roleData.SubOccupation), out GoodsDataList, out weaponGoodsDataList);
							if (GoodsDataList == null)
							{
								GoodsDataList = new List<GoodsData>();
							}
							if (weaponGoodsDataList != null)
							{
								GoodsDataList.AddRange(weaponGoodsDataList);
							}
							List<GoodsData> list2 = null;
							this.DealWithPet(roleData, out list2);
							if (list2 != null)
							{
								GoodsDataList.AddRange(list2);
							}
							wingData = roleData.MyWingData;
						}
						else
						{
							if (roleData.RebornShowEquip == 1)
							{
								this.GetRoleReborithUsingGoods(roleData.RebornGoodsDataList, Global.CheckRoleOcc(roleData.Occupation, roleData.SubOccupation), out GoodsDataList, out weaponGoodsDataList);
								if (GoodsDataList == null)
								{
									GoodsDataList = new List<GoodsData>();
								}
								if (weaponGoodsDataList != null)
								{
									GoodsDataList.AddRange(weaponGoodsDataList);
								}
								List<GoodsData> list3 = null;
								this.DealWithPet(roleData, out list3);
								if (list3 != null)
								{
									GoodsDataList.AddRange(list3);
								}
							}
							else
							{
								GoodsDataList = roleData.GoodsDataList;
							}
							wingData = roleData.MyWingData;
						}
					}
				}
				catch (Exception ex2)
				{
					MUDebug.LogException(ex2);
				}
			}
			else
			{
				try
				{
					FakeRoleData fakeRoleData = null;
					if (Global.Data.FakeRoles.TryGetValue(this.RoleID, ref fakeRoleData))
					{
						if (Global.IsBattleMap() && Global.Data.roleData.BattleWhichSide >= 1)
						{
							GoodsDataList = Global.GetBattleEquipGoodsDataList(fakeRoleData.MyRoleDataMini.BattleWhichSide, fakeRoleData.MyRoleDataMini.Occupation);
							weaponGoodsDataList = Global.GetBattleWeaponGoodsDataList(fakeRoleData.MyRoleDataMini.BattleWhichSide, fakeRoleData.MyRoleDataMini.Occupation);
							GoodsDataList.AddRange(weaponGoodsDataList);
						}
						else if (Global.IsFakeRoleFashioned(fakeRoleData.MyRoleDataMini, out array))
						{
							byte b = 0;
							while ((int)b < array.Length)
							{
								if (array[(int)b] != null)
								{
									if (Global.GetCategoriyByGoodsID(array[(int)b].GoodsID) == 24)
									{
										GoodsDataList = Global.GetFashionEquipGoodsDataList(array[(int)b].GoodsID, this.Occupation, array[(int)b].Forge_level);
									}
									else
									{
										if (GoodsDataList == null)
										{
											GoodsDataList = new List<GoodsData>();
										}
										GoodsDataList.Add(array[(int)b]);
									}
								}
								b += 1;
							}
							if (GoodsDataList == null || (GoodsDataList != null && 0 >= GoodsDataList.Count))
							{
								GoodsDataList = fakeRoleData.MyRoleDataMini.GoodsDataList;
							}
							weaponGoodsDataList = Global.GetFakeRoleBattleWeaponGoodsDataList(fakeRoleData.MyRoleDataMini);
							if (GoodsDataList == null)
							{
								GoodsDataList = new List<GoodsData>();
							}
							GoodsDataList.AddRange(weaponGoodsDataList);
							wingData = fakeRoleData.MyRoleDataMini.MyWingData;
							if (array[0] == null && fakeRoleData.MyRoleDataMini.GoodsDataList != null)
							{
								for (int j = 0; j < fakeRoleData.MyRoleDataMini.GoodsDataList.Count; j++)
								{
									GoodsData goodsData = fakeRoleData.MyRoleDataMini.GoodsDataList[j];
									if (goodsData.Using != 0)
									{
										int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
										if ((categoriyByGoodsID < 11 && categoriyByGoodsID != 8) || categoriyByGoodsID == 22)
										{
											if (GoodsDataList == null)
											{
												GoodsDataList = new List<GoodsData>();
											}
											if (!GoodsDataList.Contains(goodsData))
											{
												GoodsDataList.Add(goodsData);
											}
										}
									}
								}
							}
						}
						else
						{
							GoodsDataList = fakeRoleData.MyRoleDataMini.GoodsDataList;
							wingData = fakeRoleData.MyRoleDataMini.MyWingData;
						}
					}
				}
				catch (Exception ex3)
				{
					MUDebug.LogException(ex3);
				}
			}
			if (array != null && 0 < array.Length)
			{
				for (int k = 0; k < array.Length; k++)
				{
					if (array[k] != null && array[k].Using == 1 && Global.GetCategoriyByGoodsID(array[k].GoodsID) == 25)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void DealWithPet(RoleData roleData, out List<GoodsData> PetList)
		{
			PetList = new List<GoodsData>();
			if (roleData.GoodsDataList != null)
			{
				for (int i = 0; i < roleData.GoodsDataList.Count; i++)
				{
					GoodsData goodsData = roleData.GoodsDataList[i];
					int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					if ((categoriyByGoodsID == 10 || categoriyByGoodsID == 9) && !PetList.Contains(goodsData))
					{
						PetList.Add(goodsData);
					}
				}
			}
		}

		private void DealWithUserFashion(RoleData roleData, GoodsData[] FashionGoodsDataList, out List<GoodsData> Equit, out List<GoodsData> Weapon)
		{
			Equit = new List<GoodsData>();
			Weapon = new List<GoodsData>();
			if (roleData == null || FashionGoodsDataList == null)
			{
				return;
			}
			List<GoodsData> list = new List<GoodsData>();
			List<GoodsData> list2 = new List<GoodsData>();
			byte mapShowEquipType = Global.GetMapShowEquipType(roleData);
			if (mapShowEquipType != 0)
			{
				this.GetRoleReborithUsingGoods(roleData.RebornGoodsDataList, Global.CheckRoleOcc(roleData.Occupation, roleData.SubOccupation), out list, out list2);
			}
			if (mapShowEquipType == 0 || mapShowEquipType == 2)
			{
				if (FashionGoodsDataList[0] != null)
				{
					if (Global.GetCategoriyByGoodsID(FashionGoodsDataList[0].GoodsID) == 24)
					{
						Equit = Global.GetFashionEquipGoodsDataList(FashionGoodsDataList[0].GoodsID, this.Occupation, FashionGoodsDataList[0].Forge_level);
					}
				}
				else if (mapShowEquipType == 2)
				{
					if (0 < list.Count)
					{
						Equit.AddRange(list);
					}
				}
				else if (roleData.GoodsDataList != null)
				{
					for (int i = 0; i < roleData.GoodsDataList.Count; i++)
					{
						GoodsData goodsData = roleData.GoodsDataList[i];
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
						if (((categoriyByGoodsID < 11 && categoriyByGoodsID != 8) || categoriyByGoodsID == 22) && !Equit.Contains(goodsData))
						{
							Equit.Add(goodsData);
						}
					}
				}
				if (FashionGoodsDataList[2] != null)
				{
					Equit.Add(FashionGoodsDataList[2]);
				}
				else if (mapShowEquipType == 2)
				{
					if (0 < list2.Count)
					{
						Weapon.AddRange(list2);
					}
				}
				else
				{
					Weapon = this.GetBattleWeaponGoodsDataList(roleData.GoodsDataList);
				}
			}
			else if (mapShowEquipType == 3)
			{
				if (FashionGoodsDataList[0] != null)
				{
					if (Global.GetCategoriyByGoodsID(FashionGoodsDataList[0].GoodsID) == 24)
					{
						Equit = Global.GetFashionEquipGoodsDataList(FashionGoodsDataList[0].GoodsID, this.Occupation, FashionGoodsDataList[0].Forge_level);
					}
				}
				else if (0 < list.Count)
				{
					Equit.AddRange(list);
				}
				if (FashionGoodsDataList[2] != null)
				{
					Equit.Add(FashionGoodsDataList[2]);
				}
				else if (0 < list2.Count)
				{
					Weapon.AddRange(list2);
				}
			}
			else
			{
				if (0 < list.Count)
				{
					Equit.AddRange(list);
				}
				if (0 < list2.Count)
				{
					Weapon.AddRange(list2);
				}
			}
			if (FashionGoodsDataList[1] != null)
			{
				Equit.Add(FashionGoodsDataList[1]);
			}
			if (FashionGoodsDataList[3] != null)
			{
				Equit.Add(FashionGoodsDataList[3]);
			}
		}

		public List<GoodsData> GetBattleWeaponGoodsDataList(List<GoodsData> List)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (List != null)
			{
				for (int i = 0; i < List.Count; i++)
				{
					GoodsData goodsData = List[i];
					if (goodsData != null && goodsData.Using == 1)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
						if (categoriyByGoodsID >= 11 && categoriyByGoodsID <= 21)
						{
							list.Add(Global.GetFakeEquipGoodsData(goodsData.GoodsID, goodsData.Forge_level, goodsData.BagIndex));
						}
					}
				}
			}
			return list;
		}

		private void GetRoleReborithUsingGoods(List<GoodsData> lst, int Occ, out List<GoodsData> Equit, out List<GoodsData> Weapon)
		{
			Equit = new List<GoodsData>();
			Weapon = new List<GoodsData>();
			if (lst != null)
			{
				for (int i = 0; i < lst.Count; i++)
				{
					if (lst[i] != null && lst[i].Using == 1)
					{
						int categoriyByGoodsID = Global.GetCategoriyByGoodsID(lst[i].GoodsID);
						if (30 <= categoriyByGoodsID && 36 >= categoriyByGoodsID)
						{
							Equit.Add(lst[i]);
						}
						else if (categoriyByGoodsID == 37 || categoriyByGoodsID == 38)
						{
							GoodVO rebornEquipsByGoodsIDAndOccForShengWuAndShengQi = IConfigbase<ConfigReborn>.Instance.GetRebornEquipsByGoodsIDAndOccForShengWuAndShengQi(lst[i].GoodsID, Occ);
							if (rebornEquipsByGoodsIDAndOccForShengWuAndShengQi != null)
							{
								GoodsData goodsData = lst[i].Clone(rebornEquipsByGoodsIDAndOccForShengWuAndShengQi.ID);
								goodsData.Forge_level = 1;
								Weapon.Add(goodsData);
							}
						}
					}
				}
			}
		}

		public TransformationState CurrentTraState
		{
			get
			{
				return this.mCurrentTraState;
			}
			set
			{
				this.mCurrentTraState = value;
			}
		}

		private void ComposeGameObject()
		{
			this.ReCalcWeaponState();
			string skeletonNameByOccupation = Global.GetSkeletonNameByOccupation(this.Occupation);
			GameObject gameObject = U3DUtils.LoadSkeletonByName(skeletonNameByOccupation, false);
			if (null != gameObject)
			{
				if (this.isDefault3DRes)
				{
					this.isDefault3DRes = false;
					Object.Destroy(this._the3DGameObject);
					this._the3DGameObject = null;
					this.Trans = null;
					this._RoleInfoDisplay = null;
					this.HeadDispText = null;
					GSprite.ShowObjectCount++;
				}
				this._the3DGameObject = gameObject;
				this.Trans = this._the3DGameObject.transform;
				this._RB = this._the3DGameObject.AddComponent<Rigidbody>();
				this._RB.isKinematic = true;
				RoleLoaderData roleLoaderData = new RoleLoaderData();
				roleLoaderData.parent = gameObject;
				roleLoaderData.ForceSyncLoad = (GSpriteTypes.Leader != this.SpriteType);
				roleLoaderData.HideGameEffect = (this.SpriteType == GSpriteTypes.Other && Global.Data.SysSetting.HideGameEffect);
				RoleData roleData = null;
				if (this.RoleID == Global.Data.RoleID)
				{
					roleData = Global.Data.roleData;
				}
				else
				{
					Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData);
				}
				if (roleData != null)
				{
					roleLoaderData.SubOccupation = roleData.SubOccupation;
				}
				else
				{
					roleLoaderData.SubOccupation = 0;
				}
				roleLoaderData.GuidCurrentId = (this.GuidCurrentId = GSprite.GuidSpriteId++);
				List<GoodsData> list = null;
				if (Global.IsBufferExist(121, roleData))
				{
					roleLoaderData.GoodsDataList = new List<GoodsData>();
					list = ShenHunData.GetBianShenWeapon(roleData);
					roleLoaderData.GoodsDataList.AddRange(list);
					this.mCurrentTraState = TransformationState.BianShen;
					roleLoaderData.ChangeBodyID = ShenHunData.GetBianModel(roleData);
					roleLoaderData.IsChangeBody = true;
				}
				else
				{
					this.mCurrentTraState = TransformationState.None;
					roleLoaderData.ChangeBodyID = 0;
					roleLoaderData.IsChangeBody = false;
					this.GetRoleLoaderGoodsDataList(ref roleLoaderData.GoodsDataList, ref list, ref roleLoaderData.wingData);
					roleLoaderData.weaponGoodsDataList = list;
				}
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					float num = (float)this._cx / 100f;
					float y = 50f;
					float num2 = (float)this._cy / 100f;
					Vector3 groundPos = this.GetGroundPos(num, num2, y);
					this.Trans.localPosition = new Vector3(num, groundPos.y, num2);
					U3DUtils.AddPlayerController(this._the3DGameObject);
					this._LeaderInfo = this._the3DGameObject.GetComponent<LeaderInfo>();
					this._LeaderInfo.TriggerByCancel = Global.Data.TriggerByCancel;
					if (!ZoneLoader.DisableSliceTerrain)
					{
						ZoneLoader singleton = ZoneLoader.singleton;
						singleton.m_PlayerTransform = gameObject.transform;
					}
					if (null != Global.AudioListener43D)
					{
						this._AudioListener = gameObject.AddComponent<AudioListener>();
						Global.AudioListener43D.enabled = false;
					}
				}
				roleLoaderData.SkeletonName = skeletonNameByOccupation;
				roleLoaderData.DefaultPartNames = Global.GetNakePartsList(this.Occupation);
				roleLoaderData.Occupation = this.Occupation;
				roleLoaderData.VSName = this.VSName;
				if (SceneUIClasses.RebornMap.IsTheScene() || (!SceneUIClasses.RebornMap.IsTheScene() && roleData != null && roleData.RebornShowEquip == 1))
				{
					roleLoaderData.LoadRebitrhEquit = 1;
				}
				else
				{
					roleLoaderData.LoadRebitrhEquit = 0;
				}
				if (this.roleResLoader != null)
				{
					this.roleResLoader.Stop();
				}
				this.roleResLoader = new RoleResLoader(roleLoaderData, new OnLoadRoleComplete(this.RoleLoaderComplete));
				U3DUtils.ReplaceLayerInChildren(gameObject, LayerMask.NameToLayer(this._DefaultLayerName), null);
				this.MeshSize = U3DUtils.GetMeshSize(gameObject.transform);
				Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
				this.Trans.rotation = quaternionByDir;
			}
		}

		public void RoleLoaderComplete(RoleLoaderData loader, GameObject go)
		{
			this.roleResLoader = null;
			if (loader.GuidCurrentId != this.GuidCurrentId)
			{
				Object.Destroy(go);
				go = null;
				return;
			}
			this.HandleSpriteResComplete(go);
			Object.Destroy(this._the3DGameObject);
			this._the3DGameObject = null;
			this._the3DGameObject = go;
			try
			{
				Global.GetStrenthenDecoration1(this, this.RoleID);
				Global.Data.GameScene.AddSpriteAllJueXingTeXiao(this, this.RoleID);
				Global.Data.GameScene.AddChongShengYinJiEffect(this, this.RoleID);
				this.LoadPKloversBuffer(this, this.RoleID);
				this.LoadLingDiBufferDes(this, this.RoleID);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
			U3DUtils.AddBoxCollider(this._the3DGameObject, new Vector3(0f, 1.2f, 0f), new Vector3(0.5f, 2.4f, 0.5f), true);
			OwnerTypeManager ownerTypeManager = this._the3DGameObject.AddComponent<OwnerTypeManager>();
			ownerTypeManager.OwnerObject = this;
			this.Trans = this._the3DGameObject.transform;
			if (this._RB != null)
			{
				this._RB = this._the3DGameObject.AddComponent<Rigidbody>();
				this._RB.isKinematic = true;
			}
			this._NetAudioSource = go.AddComponent<NetAudioSource>();
			this._AnimationMgr = this._the3DGameObject.AddComponent<AnimationManager>();
			if (this._AnimationMgr != null)
			{
				this._AnimationMgr.PrepareAnimation = new AnimationChangeEventHandler(this.PrepareAnimation);
				this._AnimationMgr.PlayingAnimation = new AnimationChangeEventHandler(this.PlayingAnimation);
				this._AnimationMgr.EndAnimation = new AnimationChangeEventHandler(this.EndAnimation);
				this._AnimationMgr.gSprite = this;
			}
			if (this.Trans != null)
			{
				Vector3 groundPos = this.GetGroundPos((float)this._cx / 100f, (float)this._cy / 100f, 50f);
				this.Trans.localPosition = new Vector3((float)this._cx / 100f, groundPos.y, (float)this._cy / 100f);
			}
			string bundleID = MuAssetManager.GetBundleID("Decoration", "yingzi");
			GameObject emptyLoader = U3DUtils.GetEmptyLoader("yingzi", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			if (emptyLoader != null)
			{
				emptyLoader.transform.localPosition = new Vector3(0f, 0.05f, 0f);
				AssetbundleLoader component = emptyLoader.GetComponent<AssetbundleLoader>();
				if (component)
				{
					component.AutoDestroySelf = true;
					component.KeepOwnRenderQueue = true;
				}
				U3DUtils.AddChild(this._the3DGameObject, emptyLoader, true);
			}
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				U3DUtils.AddPlayerController(this._the3DGameObject);
				this._LeaderInfo = this._the3DGameObject.GetComponent<LeaderInfo>();
				if (this._LeaderInfo != null)
				{
					this._LeaderInfo.TriggerByCancel = Global.Data.TriggerByCancel;
				}
				if (!ZoneLoader.DisableSliceTerrain)
				{
					ZoneLoader singleton = ZoneLoader.singleton;
					singleton.m_PlayerTransform = go.transform;
				}
				if (null != Global.AudioListener43D)
				{
					this._AudioListener = go.AddComponent<AudioListener>();
					Global.AudioListener43D.enabled = false;
				}
			}
			RoleInfoDisplay roleInfoDisplay = go.AddComponent<RoleInfoDisplay>();
			try
			{
				if (roleInfoDisplay != null)
				{
					roleInfoDisplay.Target = go.transform;
					roleInfoDisplay.RoleNameText = this.ShowName;
					roleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this._SNameBrush.Color);
					Global.AddRoleNameColor(this.RoleID, ColorSL.ParseArgb(this._SNameBrush.Color));
					roleInfoDisplay.BanghuiNameText = this.VFaction;
					roleInfoDisplay.OtherNameText = this.VOtherName;
					roleInfoDisplay.PKKingSpriteName = this.PKKingSpriteName;
					this._RoleInfoDisplay = roleInfoDisplay;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						roleInfoDisplay.BanghuiNameColor = ColorSL.ParseArgb(ColorSL.FromArgb(255, 153, 204, 255));
					}
					else if (Global.Data.roleData.Faction == this.FactionID)
					{
						roleInfoDisplay.BanghuiNameColor = ColorSL.ParseArgb(ColorSL.FromArgb(255, 0, 255, 0));
					}
					else
					{
						roleInfoDisplay.BanghuiNameColor = ColorSL.ParseArgb(ColorSL.FromArgb(255, 153, 204, 255));
					}
					string teamSpriteName = string.Empty;
					if (this.InSameTeam(this.RoleID))
					{
						if (this.FlagsType == 1)
						{
							teamSpriteName = "team1";
						}
						else if (this.FlagsType == 2)
						{
							teamSpriteName = "team2";
						}
					}
					roleInfoDisplay.TeamSpriteName = teamSpriteName;
					int num;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						num = Global.GetChengJiuLevel(0);
					}
					else
					{
						num = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.ChengJiuLevel);
					}
					if (num > 0)
					{
						roleInfoDisplay.ChengjiuSpriteFlag = string.Format("chengjiu{0}", num);
					}
					int num2;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						num2 = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.ShengWangLevel);
					}
					else
					{
						num2 = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.ShengWangLevel);
					}
					if (num2 > 0)
					{
						roleInfoDisplay.JunxianSpriteName = StringUtil.substitute("junxian{0}", new object[]
						{
							num2
						});
					}
					RoleData roleData = null;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						roleData = Global.Data.roleData;
					}
					else if (Global.Data.OtherRoles.ContainsKey(this.RoleID))
					{
						roleData = Global.Data.OtherRoles[this.RoleID];
					}
					if (roleData != null)
					{
						roleInfoDisplay.roledataBufferDataList = roleData.BufferDataList;
						roleInfoDisplay.roledataBufferFashionID = roleData.RoleCommonUseIntPamams;
					}
					if (Global.IsBufferExist(121, roleData))
					{
						roleInfoDisplay.ChangBianShenHeight(ShenHunData.GetNameDisplayHeight(roleData));
					}
					else
					{
						roleInfoDisplay.ChangBianShenHeight(0f);
					}
					int num3;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						num3 = Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.TitleID);
					}
					else
					{
						num3 = Global.GetRoleCommonUseParamsValueForOtherRole(this.RoleID, RoleCommonUseIntParamsIndexs.TitleID);
					}
					if (num3 <= 0)
					{
						roleInfoDisplay.TitleName = null;
					}
					if (num3 > 0)
					{
						string text = "NetImages/GameRes/Images/ChengHao/";
						string text2 = ".png";
						int fashionGoodsID = Global.GetFashionGoodsID(num3);
						if (fashionGoodsID == 0)
						{
							roleInfoDisplay.TitleName = null;
						}
						else
						{
							string titleName = string.Concat(new object[]
							{
								text,
								"title_",
								fashionGoodsID,
								text2
							});
							roleInfoDisplay.TitleName = titleName;
						}
						roleInfoDisplay.LuoLanSpriteName = "none";
						roleInfoDisplay.ZhongShenSpriteName = "none";
					}
					string luoLanSpriteName = string.Empty;
					string zhongShenSpriteName = string.Empty;
					if (roleData != null && Global.IsBufferExist(111, roleData))
					{
						zhongShenSpriteName = "zhongshenZhiShen";
						roleInfoDisplay.ZhongShenSpriteName = zhongShenSpriteName;
					}
					if (roleData != null && Global.IsBufferExist(103, roleData))
					{
						if (Global.IsBangHuiLeader(roleData, roleData.Faction))
						{
							luoLanSpriteName = "langhunChengzhu";
						}
						else
						{
							luoLanSpriteName = "langhunGuizu";
						}
						roleInfoDisplay.LuoLanSpriteName = luoLanSpriteName;
					}
					else if (Global.Data.roleData.BangHuiLingDiItemsDict != null && Global.Data.roleData.BangHuiLingDiItemsDict[7] != null)
					{
						BangHuiLingDiItemData bangHuiLingDiItemData = Global.Data.roleData.BangHuiLingDiItemsDict[7];
						if (roleData != null && roleData.Faction != 0 && bangHuiLingDiItemData.BHID == roleData.Faction)
						{
							if (Global.IsBangHuiLeader(roleData, roleData.Faction))
							{
								luoLanSpriteName = "luolanChengzhu";
							}
							else
							{
								luoLanSpriteName = "luolanGuizhu";
							}
						}
						roleInfoDisplay.LuoLanSpriteName = luoLanSpriteName;
					}
					if (roleData != null && (Global.IsInShiLiZhengBaMap() || Global.IsInShiLiZhengBaBattleMap() || Global.IsCompMiDongMap()))
					{
						roleInfoDisplay.CompType = roleData.CompType;
						if (roleData.CompType == Global.Data.roleData.CompType)
						{
							this.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
						}
						else
						{
							this.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
						}
						roleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this.SNameBrush.Color);
					}
					if (roleData != null && Global.IsInMoYuDuoBao())
					{
						if (roleData.BattleWhichSide == Global.Data.roleData.BattleWhichSide)
						{
							this.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
						}
						else
						{
							this.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
						}
						roleInfoDisplay.RoleNameColor = ColorSL.ParseArgb(this.SNameBrush.Color);
					}
					else if (SceneUIClasses.RebornMap.IsTheScene())
					{
						roleInfoDisplay.RolePlatform = roleData.PTID;
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>进入重生地图 ：平台ＩＤ\u3000＝\u3000" + roleData.PTID + "</color>"
						});
						if (roleData.PTID == Global.Data.roleData.PTID)
						{
							this.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 153, 204, 255));
						}
						else
						{
							this.SNameBrush = new SolidColorBrush(ColorSL.FromArgb(255, 255, 0, 0));
						}
					}
					else
					{
						roleInfoDisplay.RolePlatform = -1;
						roleInfoDisplay.CompType = -1;
					}
				}
				if (Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()))
				{
					this.JunxianFlag = "none";
					this._RoleInfoDisplay.PKKingSpriteName = "none";
					this.LuolanChengZhanFlag = "none";
				}
			}
			catch (Exception ex2)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载称号出错"
				});
				MUDebug.LogException(ex2);
			}
			this.HeadDispText = go.AddComponent<HeadDisplayText>();
			if (this.HeadDispText != null)
			{
				this.HeadDispText.Target = go.transform;
			}
			go.name = this.Name;
			this.IsFlying = false;
			this.Wings = null;
			this.WingsAnimation = null;
			string text3 = null;
			try
			{
				GoodsData goodsData = null;
				List<GoodsData> goodsDataList = loader.GoodsDataList;
				if (Global.Data != null && Global.Data.roleData != null && Global.CanGiveFakeEquips && Global.Data.roleData.IsFlashPlayer >= 1)
				{
					goodsData = Global.GetTopLevelWingsHuGoodsData(loader.Occupation);
					Global.ParseWingsGoodsDataInfo(out text3);
				}
				else if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
				{
					Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
				}
				if (this.SpriteType == GSpriteTypes.Leader && loader.wingData != null)
				{
					int num4 = 26;
					if (Global.Data != null && Global.Data.roleData != null && num4 < Global.Data.roleData.RoleCommonUseIntPamams.Count)
					{
						int num5 = Global.Data.roleData.RoleCommonUseIntPamams[num4];
						if (num5 > 0)
						{
							if (goodsData == null)
							{
								int @using = loader.wingData.Using;
								loader.wingData.Using = 1;
								if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
								{
									Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
								}
								loader.wingData.Using = @using;
							}
							if (goodsData != null)
							{
								goodsData.GoodsID = Global.GetFashionGoodsID(num5);
								goodsData.Using = 1;
							}
						}
					}
				}
				else if (this.SpriteType == GSpriteTypes.Other && loader.wingData != null)
				{
					if (this.SpriteType != GSpriteTypes.Other || Global.Data == null || !Global.Data.SysSetting.HideChiBang)
					{
						RoleData roleData2 = null;
						if (Global.Data != null && Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData2))
						{
							int num6 = 26;
							if (num6 < roleData2.RoleCommonUseIntPamams.Count)
							{
								int num7 = roleData2.RoleCommonUseIntPamams[num6];
								if (num7 > 0)
								{
									if (goodsData == null)
									{
										int using2 = loader.wingData.Using;
										loader.wingData.Using = 1;
										if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
										{
											Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
										}
										loader.wingData.Using = using2;
									}
									if (goodsData != null)
									{
										goodsData.GoodsID = Global.GetFashionGoodsID(num7);
										goodsData.Using = 1;
									}
								}
							}
							else if (goodsData == null)
							{
								int using3 = loader.wingData.Using;
								loader.wingData.Using = 1;
								if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
								{
									Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
								}
								loader.wingData.Using = using3;
							}
						}
					}
				}
				else if (this.SpriteType == GSpriteTypes.FakeRole && loader.wingData != null)
				{
					FakeRoleData fakeRoleData = null;
					if (roleInfoDisplay != null && Global.Data != null && Global.Data.FakeRoles.TryGetValue(this.RoleID, ref fakeRoleData))
					{
						if (fakeRoleData.FakeRoleType == 0)
						{
							roleInfoDisplay.fakeRoleBuffFashionID = 39;
						}
						else if (fakeRoleData.FakeRoleType == 4)
						{
							roleInfoDisplay.fakeRoleBuffFashionID = 10013;
						}
						else if (fakeRoleData.FakeRoleType == 7)
						{
							roleInfoDisplay.fakeRoleBuffFashionID = 111;
						}
						else if (fakeRoleData.FakeRoleType == 8)
						{
							roleInfoDisplay.fakeRoleBuffFashionID = 10010;
						}
						else if (fakeRoleData.FakeRoleType == 9)
						{
							roleInfoDisplay.fakeRoleBuffFashionID = 10008;
						}
						else if (fakeRoleData.FakeRoleType == 10)
						{
							roleInfoDisplay.fakeRoleBuffFashionID = 10001;
						}
						if (fakeRoleData.FakeRoleType == 5 && !Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
						{
							Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
						}
						if (fakeRoleData.FakeRoleType == 6 && !Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
						{
							Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
						}
						roleInfoDisplay.roledataBufferFashionID = fakeRoleData.MyRoleDataMini.RoleCommonUseIntPamams;
						int num8 = 26;
						if (num8 < fakeRoleData.MyRoleDataMini.RoleCommonUseIntPamams.Count)
						{
							int num9;
							if (fakeRoleData.FakeRoleType == 3)
							{
								num9 = 1;
								roleInfoDisplay.fakeRoleBuffFashionID = 10012;
								roleInfoDisplay.TitleName = null;
							}
							else if (fakeRoleData.FakeRoleType == 11 || fakeRoleData.FakeRoleType == 12 || fakeRoleData.FakeRoleType == 13)
							{
								num9 = fakeRoleData.MyRoleDataMini.RoleCommonUseIntPamams[num8];
								switch (fakeRoleData.FakeRoleType)
								{
								case 11:
									roleInfoDisplay.fakeRoleBuffFashionID = 9003;
									break;
								case 12:
									roleInfoDisplay.fakeRoleBuffFashionID = 9006;
									break;
								case 13:
									roleInfoDisplay.fakeRoleBuffFashionID = 9009;
									break;
								}
								roleInfoDisplay.TitleName = null;
							}
							else if (fakeRoleData.FakeRoleType == 10)
							{
								num9 = 337;
								roleInfoDisplay.fakeRoleBuffFashionID = 10001;
								roleInfoDisplay.TitleName = null;
							}
							else
							{
								num9 = fakeRoleData.MyRoleDataMini.RoleCommonUseIntPamams[num8];
							}
							if (num9 > 0)
							{
								if (goodsData == null)
								{
									int using4 = loader.wingData.Using;
									loader.wingData.Using = 1;
									if (!Global.CheckWingFashionData(goodsDataList, out goodsData, out text3))
									{
										Global.ParseWingsGoodsDataInfo(loader.wingData, out goodsData, out text3, loader.Occupation);
									}
									loader.wingData.Using = using4;
								}
								if (goodsData != null)
								{
									goodsData.GoodsID = Global.GetFashionGoodsID(num9);
									goodsData.Using = 1;
								}
							}
						}
					}
				}
				if (goodsData != null)
				{
					if (this.SpriteType != GSpriteTypes.Other || !Global.Data.SysSetting.HideChiBang)
					{
						this.wingsResLoader = new WingsResLoader(new WingsLoadData
						{
							parent = go,
							HideGameEffect = (this.SpriteType == GSpriteTypes.Other && Global.Data.SysSetting.HideGameEffect),
							data = goodsData,
							hangPointName = text3
						}, new OnWingsLoadComplete(this.WingsLoaderComplete));
					}
				}
			}
			catch (Exception ex3)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载翅膀数据出错"
				});
				MUDebug.LogException(ex3);
			}
			try
			{
				this.DestroyShouHuChong();
				ShouHuChongLoadData shouHuChongLoadData = new ShouHuChongLoadData();
				shouHuChongLoadData.parent = go;
				if (shouHuChongLoadData != null)
				{
					shouHuChongLoadData.Occupation = loader.Occupation;
					shouHuChongLoadData.HideGameEffect = (this.SpriteType == GSpriteTypes.Other && Global.Data.SysSetting.HideGameEffect);
					GoodsData goodsData2 = null;
					List<GoodsData> goodsDataList2;
					if (this.SpriteType == GSpriteTypes.Leader)
					{
						goodsDataList2 = Global.Data.equipPet;
					}
					else
					{
						goodsDataList2 = loader.GoodsDataList;
					}
					text3 = null;
					if (Global.CanGiveFakeEquips && Global.Data.roleData != null && Global.Data.roleData.IsFlashPlayer >= 1)
					{
						goodsData2 = Global.GetTopLevelShouHuGoodsData(loader.Occupation);
						Global.ParseShouHuChongGoodsDataInfo(goodsData2, out goodsData2, out text3);
					}
					else
					{
						Global.ParseShouHuChongGoodsDataInfo(goodsDataList2, out goodsData2, out text3);
					}
					shouHuChongLoadData.data = goodsData2;
					shouHuChongLoadData.EmptyName = text3;
					if (goodsData2 != null)
					{
						shouHuChongLoadData.Categoriy = (ItemCategories)Global.GetCategoriyByGoodsID(goodsData2.GoodsID);
					}
					shouHuChongLoadData.SpecialGameObjectsComplete = new AssetbundleLoaderComplete(this.ShouHuChongLoaderSpecialGameObjectsComplete);
					this.shouHuChongResLoader = new ShouHuChongResLoader(shouHuChongLoadData, new OnShouHuChongLoadComplete(this.ShouHuChongLoaderComplete));
				}
			}
			catch (Exception ex4)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载守护宠物出错"
				});
				MUDebug.LogException(ex4);
			}
			try
			{
				List<GoodsData> goodsDataList3 = loader.GoodsDataList;
				List<GoodsData> list = new List<GoodsData>();
				List<string> list2 = new List<string>();
				List<string> list3 = new List<string>();
				int occupation = this.Occupation;
				int subOcc = 0;
				byte b = 0;
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					if (Global.Data.roleData != null)
					{
						subOcc = Global.Data.roleData.SubOccupation;
						b = (byte)Global.Data.roleData.RebornShowEquip;
					}
				}
				else if (this.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData3 = null;
					if (Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData3))
					{
						subOcc = roleData3.SubOccupation;
						b = (byte)roleData3.RebornShowEquip;
					}
				}
				else if (this.SpriteType == GSpriteTypes.FakeRole)
				{
					FakeRoleData fakeRoleData2 = null;
					if (Global.Data.FakeRoles.TryGetValue(this.RoleID, ref fakeRoleData2))
					{
						subOcc = fakeRoleData2.MyRoleDataMini.SubOccupation;
					}
				}
				if (loader.IsChangeBody)
				{
					if (goodsDataList3.Count > 0)
					{
						list2.Add("youshou");
						list3.Add("youshou");
						list.AddRange(goodsDataList3);
					}
				}
				else
				{
					if (SceneUIClasses.RebornMap.IsTheScene())
					{
						Global.CheckBagIndex(goodsDataList3, Global.CheckRoleOcc(occupation, subOcc));
					}
					else if (b == 1)
					{
						Global.CheckBagIndex(goodsDataList3, Global.CheckRoleOcc(occupation, subOcc));
					}
					Global.ParseWeaponGoodsDataInfo(goodsDataList3, list, list2, list3, Global.CheckRoleOcc(occupation, subOcc));
				}
				this.WeaponState = Global.CalcWeaponState(list, null, this.Occupation);
				this.SafeEmptyNamesList = list3;
				this.NotSafeEmptyNamesList = list2;
				WeaponLoadData weaponLoadData = new WeaponLoadData();
				weaponLoadData.parent = go;
				if (this.SpriteType == GSpriteTypes.Other)
				{
					bool flag = ConfigSystemParam.GetSystemParamIntByName("AutoHideWeaponEffect") == 1L;
					if (flag)
					{
						weaponLoadData.HideGameEffect = true;
					}
					else
					{
						weaponLoadData.HideGameEffect = Global.Data.SysSetting.HideGameEffect;
					}
				}
				else
				{
					weaponLoadData.HideGameEffect = false;
				}
				weaponLoadData.occupation = ((b != 1) ? this.Occupation : Global.CheckRoleOcc(this.Occupation, subOcc));
				weaponLoadData.weaponList = list;
				weaponLoadData.hangPointList = ((!this.IsInSafeRegion) ? list2 : list3);
				this.weaponResLoader = new WeaponResLoader(weaponLoadData, new OnWeaponLoadComplete(this.WeaponLoaderComplete));
			}
			catch (Exception ex5)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载武器出错"
				});
				MUDebug.LogException(ex5);
			}
			try
			{
				byte b2 = 0;
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					b2 = 1;
				}
				else if (!Global.Data.SysSetting.HideGameEffect)
				{
					b2 = 1;
				}
				if (b2 == 1 && loader.GoodsDataList != null && 0 < loader.GoodsDataList.Count)
				{
					GoodsData goodsData3 = null;
					for (int i = 0; i < loader.GoodsDataList.Count; i++)
					{
						GoodsData goodsData4 = loader.GoodsDataList[i];
						if (goodsData4 != null && Global.GetCategoriyByGoodsID(goodsData4.GoodsID) == 26 && 0 < goodsData4.Using)
						{
							goodsData3 = goodsData4;
							break;
						}
					}
					if (goodsData3 != null)
					{
						FashionLevelupVO fashionLevelupVO = ConfigFashion.Get(ItemCategories.ShiZhuang_JiaoYin, goodsData3.GoodsID, goodsData3.Forge_level);
						if (fashionLevelupVO != null)
						{
							GDecoration decoration = Global.GetDecoration(fashionLevelupVO.Scene, GDecorationTypes.Loop, new Point(0, 0), true, null, -1, -1, true, false);
							this.Add(decoration);
						}
					}
				}
			}
			catch (Exception ex6)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载时装脚印出错"
				});
				MUDebug.LogException(ex6);
			}
			U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer(this._DefaultLayerName), null);
			this.MeshSize = U3DUtils.GetMeshSize(go.transform);
			Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
			if (this.Trans != null)
			{
				this.Trans.rotation = quaternionByDir;
			}
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				go.AddComponent<TransparentCharacter>();
			}
			this.LoadReal3DResOK = true;
			if (!this._Visiblity)
			{
				this.HideObject();
			}
			if (this.SpriteType != GSpriteTypes.Leader || !Global.CanGuanZhan())
			{
				if (Global.Data.roleData != null && Global.Data.roleData.RoleID == this.RoleID)
				{
					if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MountIsRide) == 1)
					{
						this.RoleLoadMount();
					}
				}
				else if (!Global.Data.SysSetting.HideOtherRoles && Global.Data.OtherRoles.ContainsKey(this.RoleID) && Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams != null && 52 < Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams.Count && Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams[52] == 1)
				{
					this.RoleLoadMount();
				}
			}
			if (this.SpriteType == GSpriteTypes.FakeRole && (this.FakeRoleType == FakeRoleTypes.CoupleWishMan || this.FakeRoleType == FakeRoleTypes.CoupleWishWife))
			{
				if (this.Occupation == 0)
				{
					this._Action = GActions.ZS_Orz;
				}
				else if (this.Occupation == 1)
				{
					this._Action = GActions.FS_Orz;
				}
				else if (this.Occupation == 2)
				{
					this._Action = GActions.GS_Orz;
				}
				else if (this.Occupation == 3)
				{
					this._Action = GActions.MJ_Orz;
				}
			}
			else
			{
				this._Action = GActions.Stand;
			}
			this.ChangeAction(this._Action);
			this.SetParentAgain(this.Trans);
			if (Global.IsInJingLingMap())
			{
				JingLingMap.inst.RoleLoaderComplete(this, loader, go);
			}
			if (this.SpriteType == GSpriteTypes.Leader && Global.CanGuanZhan())
			{
				Transform transform = go.transform;
				if (transform != null)
				{
					int childCount = transform.childCount;
					if (childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
					{
						for (int j = 0; j < childCount; j++)
						{
							transform.GetChild(j).gameObject.SetActive(false);
						}
						SkinnedMeshRenderer component2 = transform.GetComponent<SkinnedMeshRenderer>();
						if (component2)
						{
							component2.enabled = false;
						}
						BoxCollider component3 = transform.GetComponent<BoxCollider>();
						if (component3)
						{
							component3.enabled = false;
						}
					}
				}
			}
			if (Global.IsInShiLiZhengBaBattleMap())
			{
				RoleData roleData4 = null;
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					roleData4 = Global.Data.roleData;
				}
				else if (Global.Data.OtherRoles.ContainsKey(this.RoleID))
				{
					roleData4 = Global.Data.OtherRoles[this.RoleID];
				}
				if (roleData4 != null)
				{
					if (Global.IsBufferExist(121, roleData4))
					{
						go.transform.localScale = new Vector3(1f, 1f, 1f);
					}
					else
					{
						MUCompLevel compLevelByCompIDAndLevel = ShiLiData.GetCompLevelByCompIDAndLevel(roleData4.CompType, (int)roleData4.CompZhiWu);
						if (compLevelByCompIDAndLevel == null)
						{
							go.transform.localScale = new Vector3(1f, 1f, 1f);
						}
						else
						{
							go.transform.localScale = new Vector3(compLevelByCompIDAndLevel.ModEnlarge, compLevelByCompIDAndLevel.ModEnlarge, compLevelByCompIDAndLevel.ModEnlarge);
						}
					}
				}
			}
			if (Global.IsInDaTaoSha() && DaTaoShaDataManager.BianShenGuanZhanCallBack != null && this.SpriteType == GSpriteTypes.Other)
			{
				RoleData roleData5 = null;
				if (Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData5))
				{
					DaTaoShaDataManager.BianShenGuanZhanCallBack.Invoke(this.RoleID);
				}
			}
		}

		public void RoleLoadMount()
		{
			if (this._Action == GActions.Walk || this._Action == GActions.Run || this._Action == GActions.Stand)
			{
				if (this.OnHorseEX)
				{
					this.RoleUnLoadMount(0);
				}
				if (this._Visiblity)
				{
					this.HorseController.LoadMount(new HorseMountCallBask(this.RoleLoadMountComplete));
				}
			}
		}

		public void RoleUnLoadMount(byte DestoryMount = 0)
		{
			if (this.OnHorseEX)
			{
				try
				{
					Vector3 rolepos = Vector3.one * -2000f;
					if (null != this.Trans)
					{
						rolepos = this.Trans.position;
					}
					this.ChangeRoleParentRoot(false, rolepos);
					this.HorseController.UnLoadMount(DestoryMount);
					this.RefreshRolePetTrans();
					this.ChangeRoleBoxColliderSize(false);
					if (this.RoleDisHorse == 1)
					{
						this.ChangeAction(this._Action);
						this.RoleDisHorse = 0;
					}
					this.ChangeWeaponsToBack(this.IsInSafeRegion);
				}
				catch (Exception ex)
				{
					MUDebug.LogError<string>(new string[]
					{
						ex.Message
					});
				}
			}
		}

		private void RoleLoadMountComplete(GameObject go)
		{
			if (null != go)
			{
				if (null == this.The3DGameObject)
				{
					this.HorseController.UnLoadMount(1);
				}
				else
				{
					try
					{
						this.HorseController.RefreshHorseLevelEffect();
						this.ChangeRoleParentRoot(true, this.The3DGameObject.transform.position);
						this.RoleDisHorse = 1;
						this.RefreshRolePetTrans();
						this.ChangeRoleBoxColliderSize(true);
						this.ChangeAction(this._Action);
						this.ChangeHorseAction(this._Action);
						this.ChangeWeaponsToBack(true);
					}
					catch (Exception ex)
					{
						MUDebug.LogError<string>(new string[]
						{
							ex.Message
						});
					}
				}
			}
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				if (Global.GetRoleCommonUseParamsValue(RoleCommonUseIntParamsIndexs.MountIsRide) == 0)
				{
					this.RoleUnLoadMount(0);
				}
			}
			else if (Global.Data.OtherRoles.ContainsKey(this.RoleID) && Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams != null && 52 < Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams.Count && Global.Data.OtherRoles[this.RoleID].RoleCommonUseIntPamams[52] == 0)
			{
				this.RoleUnLoadMount(0);
			}
		}

		private void ChangeRoleParentRoot(bool bHaveHorse, Vector3 Rolepos)
		{
			if (null == this.The3DGameObject)
			{
				return;
			}
			this.OnHorseEX = bHaveHorse;
			Rigidbody rigidbody = null;
			if (bHaveHorse)
			{
				this._HorseController.SetPos(Rolepos);
				this._HorseController.SetRotation(this.Trans.localRotation);
				U3DUtils.AddChild(this._HorseController.TheHorseMountPoint.gameObject, this.The3DGameObject, true);
				this.The3DGameObject.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				this.The3DGameObject.transform.localPosition = Vector3.zero;
				this.Trans = this._HorseController.TheHorseGameobject.transform;
				Transform transform = this.The3DGameObject.transform.FindChild("yingzi");
				if (null != transform)
				{
					transform.SetParent(this._HorseController.HorseTrans, false);
					transform.localPosition = Vector3.zero;
				}
				SkinnedMeshRenderer componentInChildren = this._HorseController.HorseTrans.GetComponentInChildren<SkinnedMeshRenderer>();
				if (null != componentInChildren)
				{
					rigidbody = componentInChildren.GetComponent<Rigidbody>();
					if (null == rigidbody)
					{
						componentInChildren.gameObject.AddComponent<Rigidbody>();
						componentInChildren.name = this._HorseController.HorseTrans.name;
					}
				}
			}
			else
			{
				this.The3DGameObject.transform.SetParent(null, false);
				this.Trans = this.The3DGameObject.transform;
				this.Trans.position = Rolepos;
				Transform transform2 = this._HorseController.TheHorseMountPoint.FindChild("yingzi");
				if (null != transform2)
				{
					transform2.SetParent(this.The3DGameObject.transform, false);
					transform2.localPosition = Vector3.zero;
				}
				rigidbody = this.The3DGameObject.GetComponent<Rigidbody>();
				if (null == rigidbody)
				{
					this.The3DGameObject.AddComponent<Rigidbody>();
				}
			}
			if (null != rigidbody)
			{
				this._RB = rigidbody;
				this._RB.isKinematic = true;
			}
			Vector3 groundPos = this.GetGroundPos(this.Trans.position.x, this.Trans.position.z, 50f);
			if (groundPos != Vector3.zero)
			{
				this.Trans.localPosition = Vector3.Slerp(this.Trans.localPosition, groundPos, 0.05f);
			}
			this.JiaoYinFashionParent = this.Trans;
			if (this.RoleID == Global.Data.RoleID)
			{
				CameraController component = this.The3DGameObject.GetComponent<CameraController>();
				if (null != component)
				{
					if (bHaveHorse)
					{
						component.TagterTrans = this._HorseController.TheHorseGameobject.transform;
						if (null != Global.Data.GameRadarMap)
						{
							Global.Data.GameRadarMap.PlayerTransTarget = this._HorseController.TheHorseGameobject.transform;
						}
					}
					else
					{
						component.TagterTrans = null;
						if (null != Global.Data.GameRadarMap)
						{
							Global.Data.GameRadarMap.PlayerTransTarget = null;
						}
					}
				}
			}
			this.SetParentAgain(this.Trans);
		}

		private void ChangeRoleBoxColliderSize(bool bHaveHorse)
		{
			if (null == this.The3DGameObject)
			{
				return;
			}
			BoxCollider boxCollider = this.The3DGameObject.GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				boxCollider = this.The3DGameObject.AddComponent<BoxCollider>();
			}
			Vector3 size = boxCollider.size;
			Vector3 center = boxCollider.center;
			if (bHaveHorse)
			{
				size.y = 2.8f;
				center.y = -0.07f;
			}
			else
			{
				size.y = 2.4f;
				center.y = 1.2f;
			}
			boxCollider.center = center;
			boxCollider.size = size;
		}

		public void WingsLoaderComplete(WingsLoadData loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			if (null == go)
			{
				return;
			}
			if (null == loader.parent)
			{
				MUDebug.LogError<string>(new string[]
				{
					"loader.parent = NULL =====加载翅膀的父类为空===== "
				});
				Object.Destroy(go);
				return;
			}
			GameObject gameObject = U3DUtils.FindGameObjectByName(loader.parent, loader.hangPointName);
			if (null == gameObject)
			{
				Object.Destroy(go);
				return;
			}
			U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer(this._DefaultLayerName), null);
			U3DUtils.AddChild(gameObject, go, true);
			this.IsFlying = true;
			this.Wings = go;
			this.WingsAnimator = this.Wings.GetComponent<Animator>();
			if (this.WingsAnimator == null)
			{
				this.WingsAnimation = this.Wings.GetComponent<Animation>();
			}
			this.ChangeAction(GActions.Stand);
		}

		public void ShouHuChongLoaderComplete(ShouHuChongLoadData loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			this.DestroyShouHuChong();
			if (this.The3DGameObject == null)
			{
				Object.Destroy(go);
				return;
			}
			this.shouHuChongController = go.AddComponent<ShouHuChongController>();
			this.shouHuChongController.LoaderURL = loader.LoaderURL;
			this.shouHuChongController.Categoriy = loader.Categoriy;
			U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer(this._DefaultLayerName), null);
			this.shouHuChongController.InitController(go, this.The3DGameObject.transform);
			string bundleID = MuAssetManager.GetBundleID("Decoration", "yingzi");
			GameObject emptyLoader = U3DUtils.GetEmptyLoader("yingzi", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			if (loader.Categoriy == ItemCategories.ChongWu)
			{
				emptyLoader.transform.localPosition = new Vector3(0f, 0.05f, 0f);
			}
			else if (loader.Categoriy == ItemCategories.ShouHuChong)
			{
				emptyLoader.transform.localPosition = new Vector3(0f, -1.95f, 0f);
			}
			AssetbundleLoader component = emptyLoader.GetComponent<AssetbundleLoader>();
			if (component)
			{
				component.AutoDestroySelf = true;
				component.KeepOwnRenderQueue = true;
			}
			U3DUtils.AddChild(go, emptyLoader, true);
			RoleData roleData = null;
			if (this.RoleID == Global.Data.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else
			{
				Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData);
			}
			if (roleData != null && roleData.JingLingYuanSuJueXingData != null && (!Global.Data.SysSetting.HideGameEffect || this.SpriteType != GSpriteTypes.Other || ConfigSystemParam.GetSystemParamIntByName("IsOpenJinglingwaixianEffect") == 0L))
			{
				bool flag = true;
				JingLingYuanSuShuXingVO jingLingYuanSuShuXinKeyType = ConfigYuanSuJueXing.instance.GetJingLingYuanSuShuXinKeyType(roleData.JingLingYuanSuJueXingData.ActiveIDs, roleData.JingLingYuanSuJueXingData.ActiveType, out flag);
				int level = jingLingYuanSuShuXinKeyType.Level;
				int minLevel = jingLingYuanSuShuXinKeyType.MinLevel;
				if (flag && minLevel / 4 > 0)
				{
					string text = string.Empty;
					int code = jingLingYuanSuShuXinKeyType.JingLingSpecial.SafeToInt32(0);
					DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(code);
					if (decorationVOByCode != null)
					{
						text = decorationVOByCode.ResName;
					}
					if (!string.IsNullOrEmpty(text))
					{
						string bundleID2 = MuAssetManager.GetBundleID("Decoration", text);
						GameObject emptyLoader2 = U3DUtils.GetEmptyLoader("Decoration_JueXing", bundleID2, false, null, null, -1, null, -1, 1f, true, false, null);
						if (loader.Categoriy == ItemCategories.ChongWu)
						{
							emptyLoader2.transform.localPosition = Vector3.zero;
						}
						Vector3 localScale;
						localScale..ctor(0f, 0f, 0f);
						if (go.transform.localScale.x > 0f)
						{
							localScale.x = 1f / go.transform.localScale.x;
						}
						if (go.transform.localScale.y > 0f)
						{
							localScale.y = 1f / go.transform.localScale.y;
						}
						if (go.transform.localScale.z > 0f)
						{
							localScale.z = 1f / go.transform.localScale.z;
						}
						emptyLoader2.transform.localScale = localScale;
						emptyLoader2.GetComponent<AssetbundleLoader>().AutoDestroySelf = false;
						U3DUtils.AddChild(go, emptyLoader2, true);
						this.shouHuChongController.GameJueXing = emptyLoader2;
					}
				}
			}
			PetFollow component2 = go.GetComponent<PetFollow>();
			if (component2 != null)
			{
				if (loader.Categoriy == ItemCategories.ChongWu)
				{
					component2.offsetX = 0f;
					component2.offsetY = 0f;
					component2.offsetZ = 0f;
					go.transform.localPosition = component2.target.localPosition + component2.target.forward * -0.5f + component2.target.up * 0f + component2.target.right * 0.5f;
				}
				else if (loader.Categoriy == ItemCategories.ShouHuChong)
				{
					component2.offsetX = 0f;
					component2.offsetY = 2f;
					component2.offsetZ = -0.5f;
					go.transform.localPosition = component2.target.localPosition + component2.target.forward * component2.offsetX + component2.target.up * component2.offsetY + component2.target.right * component2.offsetZ;
				}
				component2.ActionRange = 1f;
				component2.stopRange = 1f;
				component2.PetItemEvent = delegate(object s, PetEventArgs e)
				{
					if (e.StepType == 1)
					{
						if (this.shouHuChongController != null && this.shouHuChongController.Action != GPetActions.Walk)
						{
							this.shouHuChongController.Action = GPetActions.Walk;
						}
					}
					else if (e.StepType == 2 && this.shouHuChongController != null && this.shouHuChongController.Action == GPetActions.Walk)
					{
						this.shouHuChongController.Action = GPetActions.Stand;
					}
				};
			}
			go.AddComponent<LoadRoleShaderAgain>();
			UIModelDestroyEffect component3 = go.GetComponent<UIModelDestroyEffect>();
			if (component3 != null)
			{
				component3.DestroyEffect();
			}
			if (this.SpriteType == GSpriteTypes.Leader && Global.CanGuanZhan())
			{
				Transform transform = go.transform;
				if (transform.childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
				{
					for (int i = 0; i < transform.childCount; i++)
					{
						transform.GetChild(i).gameObject.SetActive(false);
					}
				}
			}
			if (this._Visiblity)
			{
				this.RefreshRolePetTrans();
			}
			else
			{
				this.shouHuChongController.gameObject.SetActive(false);
			}
		}

		public void ShouHuChongLoaderSpecialGameObjectsComplete(AssetbundleLoader loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			go.AddComponent<LoadRoleShaderAgain>();
		}

		public void WeaponLoaderComplete(WeaponLoadData loader, List<GameObject> gameObjectList)
		{
			if (gameObjectList == null || gameObjectList.Count <= 0)
			{
				this.LoadReal3DWeaponOk = true;
				return;
			}
			for (int i = 0; i < gameObjectList.Count; i++)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(loader.parent, loader.hangPointList[i]);
				if (null == gameObject)
				{
					Object.Destroy(gameObjectList[i]);
				}
				else
				{
					U3DUtils.ReplaceLayerInChildren(gameObjectList[i], LayerMask.NameToLayer(this._DefaultLayerName), null);
					U3DUtils.AddChild(gameObject, gameObjectList[i], true);
				}
			}
			this.WeaponList = gameObjectList;
			this.WeaponState = Global.CalcWeaponState(loader.weaponList, this.WeaponList, this.Occupation);
			if (this.SafeEmptyNamesList != null && this.NotSafeEmptyNamesList != null)
			{
				this.ChangeWeaponsPosition(this.IsInSafeRegion);
			}
			this.LoadReal3DWeaponOk = true;
		}

		private void LoadMonsterRoot()
		{
			this._the3DGameObject = new GameObject();
			GameObject the3DGameObject = this._the3DGameObject;
			OwnerTypeManager ownerTypeManager = this._the3DGameObject.AddComponent<OwnerTypeManager>();
			ownerTypeManager.OwnerObject = this;
			string bundleID = MuAssetManager.GetBundleID("Decoration", "yingzi");
			GameObject emptyLoader = U3DUtils.GetEmptyLoader("yingzi", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			emptyLoader.transform.localPosition = new Vector3(0f, 0.05f, 0f);
			AssetbundleLoader component = emptyLoader.GetComponent<AssetbundleLoader>();
			if (component)
			{
				component.AutoDestroySelf = true;
				component.KeepOwnRenderQueue = true;
			}
			U3DUtils.AddChild(this._the3DGameObject, emptyLoader, true);
			MonsterInfoDisplay monsterInfoDisplay = the3DGameObject.AddComponent<MonsterInfoDisplay>();
			monsterInfoDisplay.MosterID = this.RoleID;
			monsterInfoDisplay.Target = the3DGameObject.transform;
			monsterInfoDisplay.MonsterNameText = this.VSName;
			monsterInfoDisplay.MonsterNameVisible = (this.VLife < this.VLifeMax);
			this._MonsterInfoDisplay = monsterInfoDisplay;
			this.HeadDispText = the3DGameObject.AddComponent<HeadDisplayText>();
			this.HeadDispText.Target = the3DGameObject.transform;
			this.Trans = this._the3DGameObject.transform;
			this._RB = this._the3DGameObject.AddComponent<Rigidbody>();
			this._RB.isKinematic = true;
			U3DUtils.AddBoxCollider(this._the3DGameObject, new Vector3(0f, 1.2f, 0f), new Vector3(0.75f, 2.4f, 0.75f), true);
			Vector3 groundPos = this.GetGroundPos((float)this._cx / 100f, (float)this._cy / 100f, 50f);
			this.Trans.localPosition = new Vector3((float)this._cx / 100f, groundPos.y, (float)this._cy / 100f);
			the3DGameObject.name = this.Name;
			this._NetAudioSource = the3DGameObject.AddComponent<NetAudioSource>();
			Animation component2 = the3DGameObject.GetComponent<Animation>();
			if (component2 != null)
			{
				this.DisplayAnimationComponent();
				U3DUtils.ModifyAnimationSpeed(the3DGameObject, DataObject.Instance.GetMonsterSpeed(this.ExtensionID));
				this._AnimationMgr = this._the3DGameObject.AddComponent<AnimationManager>();
				this._AnimationMgr.PrepareAnimation = new AnimationChangeEventHandler(this.PrepareAnimation);
				this._AnimationMgr.PlayingAnimation = new AnimationChangeEventHandler(this.PlayingAnimation);
				this._AnimationMgr.EndAnimation = new AnimationChangeEventHandler(this.EndAnimation);
			}
			U3DUtils.ReplaceLayerInChildren(the3DGameObject, LayerMask.NameToLayer(this._DefaultLayerName), null);
			this.MeshSize = U3DUtils.GetMeshSize(the3DGameObject.transform);
			Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
			this.Trans.rotation = quaternionByDir;
		}

		private void LoadMonsterBody()
		{
			string resName;
			if (Global.Data.roleData.BattleWhichSide == this.BattleWhichSide && Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
			{
				resName = "WarFlag_2.unity3d";
			}
			else
			{
				resName = ConfigMonsters.GetMonster3DResNameByID(this.ExtensionID);
			}
			MonsterNPCLoaderData monsterNPCLoaderData = new MonsterNPCLoaderData();
			monsterNPCLoaderData.resName = resName;
			monsterNPCLoaderData.spriteType = GSpriteTypes.Monster;
			monsterNPCLoaderData.MonsterID = this.ExtensionID;
			Global.HandleHandWeapon(monsterNPCLoaderData, this.ExtensionID, this.SpriteType);
			this.monsterNPCResLoader = new MonsterNPCResLoader(monsterNPCLoaderData, new OnLoadMonsterNPCComplete(this.MonsterLoaderComplete));
		}

		public void MonsterLoaderComplete(MonsterNPCLoaderData loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			this.HandleSpriteResComplete(go);
			Vector3 localPosition = (!(this.Trans != null)) ? Vector3.zero : this.Trans.localPosition;
			Object.Destroy(this._the3DGameObject);
			this._the3DGameObject = null;
			this._the3DGameObject = go;
			if (this.AnimationController != null)
			{
				this.mAnimatorComponent = go.GetComponent<Animator>();
				if (this.mAnimatorComponent != null)
				{
					this.InitAnimationClips();
					this.AnimationController.SetState(SpriteAnimState.Stand);
					this.mAnimatorComponent.speed = DataObject.Instance.GetMonsterSpeed(this.ExtensionID);
				}
				else
				{
					this.mAnimationController = null;
				}
			}
			if (this.AnimationController == null)
			{
				this._AnimationMgr = this._the3DGameObject.AddComponent<AnimationManager>();
				this._AnimationMgr.PrepareAnimation = new AnimationChangeEventHandler(this.PrepareAnimation);
				this._AnimationMgr.PlayingAnimation = new AnimationChangeEventHandler(this.PlayingAnimation);
				this._AnimationMgr.EndAnimation = new AnimationChangeEventHandler(this.EndAnimation);
				U3DUtils.ModifyAnimationSpeed(go, DataObject.Instance.GetMonsterSpeed(this.ExtensionID));
			}
			string bundleID = MuAssetManager.GetBundleID("Decoration", "yingzi");
			GameObject emptyLoader = U3DUtils.GetEmptyLoader("yingzi", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			emptyLoader.transform.localPosition = new Vector3(0f, 0.05f, 0f);
			AssetbundleLoader component = emptyLoader.GetComponent<AssetbundleLoader>();
			if (component)
			{
				component.AutoDestroySelf = true;
				component.KeepOwnRenderQueue = true;
			}
			U3DUtils.AddChild(this._the3DGameObject, emptyLoader, true);
			OwnerTypeManager ownerTypeManager = this._the3DGameObject.AddComponent<OwnerTypeManager>();
			ownerTypeManager.OwnerObject = this;
			this.Trans = this._the3DGameObject.transform;
			this._RB = this._the3DGameObject.AddComponent<Rigidbody>();
			this._RB.isKinematic = true;
			U3DUtils.AddBoxCollider(this._the3DGameObject, new Vector3(0f, 1.2f, 0f), new Vector3(0.75f, 2.4f, 0.75f), true);
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.ExtensionID);
			if (monsterXmlNodeByID != null && monsterXmlNodeByID.IsCollide)
			{
				U3DUtils.AddBoxCollider(this._the3DGameObject, monsterXmlNodeByID.CollideCenter, monsterXmlNodeByID.CollideSize, true);
			}
			this.Trans.localPosition = localPosition;
			int monsterID = loader.MonsterID;
			if (monsterID > 600000 && monsterID < 700000)
			{
				if (monsterID >= 609005 && monsterID <= 609008)
				{
					if (monsterID == 609005)
					{
						string bundleID2 = MuAssetManager.GetBundleID("Decoration", "TDJT");
						GameObject emptyLoader2 = U3DUtils.GetEmptyLoader("jiantou", bundleID2, false, null, null, -1, null, -1, 1f, true, false, null);
						emptyLoader2.transform.localPosition = new Vector3(0f, -0.4f, 0f);
						emptyLoader2.GetComponent<AssetbundleLoader>().AutoDestroySelf = true;
						U3DUtils.AddChild(this._the3DGameObject, emptyLoader2, true);
					}
					else if (monsterID == 609006 || monsterID == 609007)
					{
						string bundleID3 = MuAssetManager.GetBundleID("Decoration", "TDJT");
						GameObject emptyLoader3 = U3DUtils.GetEmptyLoader("jiantou", bundleID3, false, null, null, -1, null, -1, 1f, true, false, null);
						emptyLoader3.transform.localPosition = new Vector3(0f, -1f, 0.2f);
						emptyLoader3.GetComponent<AssetbundleLoader>().AutoDestroySelf = true;
						BoxCollider component2 = this._the3DGameObject.GetComponent<BoxCollider>();
						if (loader.MonsterID == 609006)
						{
							component2.center = new Vector3(-0.5f, 1.2f, 0f);
							component2.size = new Vector3(2f, 2.4f, 0.75f);
						}
						else
						{
							component2.center = new Vector3(0f, 0.6f, 0f);
							component2.size = new Vector3(2.4f, 1.1f, 1f);
						}
						U3DUtils.AddChild(this._the3DGameObject, emptyLoader3, true);
					}
				}
				if (monsterID % 100 == 8)
				{
					go.AddComponent<LoadRoleShaderAgain>();
				}
			}
			if (monsterID == 8100000)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("MoRiShenPanEffect", ',');
				if (Global.MoRiShenPanOnTimeKillCount > 0)
				{
					int code = systemParamIntArrayByName[Global.MoRiShenPanOnTimeKillCount - 1];
					string bundleID4 = MuAssetManager.GetBundleID("Decoration", ConfigDecoration.GetDecorationVOByCode(code).ResName);
					GameObject emptyLoader4 = U3DUtils.GetEmptyLoader("BossTeXiao", bundleID4, false, null, null, -1, null, -1, 1f, true, false, null);
					emptyLoader4.transform.localPosition = new Vector3(0f, 0f, 0f);
					emptyLoader4.GetComponent<AssetbundleLoader>().AutoDestroySelf = false;
					U3DUtils.AddChild(this._the3DGameObject, emptyLoader4, true);
				}
			}
			Global.AddSpecialGameObjects4Monster(this._the3DGameObject, this.ExtensionID, -1, 1f);
			Global.ReplaceMaterials4Monster(go, this.ExtensionID);
			MonsterInfoDisplay monsterInfoDisplay = go.AddComponent<MonsterInfoDisplay>();
			monsterInfoDisplay.MosterID = this.RoleID;
			monsterInfoDisplay.Target = go.transform;
			monsterInfoDisplay.MonsterNameText = this.VSName;
			monsterInfoDisplay.MonsterNameVisible = (this.VLife < this.VLifeMax);
			this._MonsterInfoDisplay = monsterInfoDisplay;
			if (Global.IsInShiLiZhengBaMap() && this.MonsterType == MonsterTypes.CaiJiByTime)
			{
				MonsterCaiJiInfoDisplay monsterCaiJiInfoDisplay = go.AddComponent<MonsterCaiJiInfoDisplay>();
				monsterCaiJiInfoDisplay.MosterID = this.RoleID;
				monsterCaiJiInfoDisplay.Target = go.transform;
			}
			this.HeadDispText = go.AddComponent<HeadDisplayText>();
			this.HeadDispText.Target = go.transform;
			go.name = this.Name;
			this._NetAudioSource = go.AddComponent<NetAudioSource>();
			LoadRoleShaderAgain loadRoleShaderAgain = go.AddComponent<LoadRoleShaderAgain>();
			float num = (float)ConfigMonsters.GetMonster3DResScaleByID(this.ExtensionID);
			if (num != 1f && num != 0f)
			{
				go.transform.localScale = new Vector3(num, num, num);
			}
			U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer(this._DefaultLayerName), null);
			this.MeshSize = U3DUtils.GetMeshSize(go.transform);
			Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
			this.Trans.rotation = quaternionByDir;
			string text;
			if (Global.Data.roleData.BattleWhichSide == this.BattleWhichSide && Global.GetMapSceneUIClass() == SceneUIClasses.AKaLunXi)
			{
				text = "WarFlag_2.unity3d";
			}
			else
			{
				text = ConfigMonsters.GetMonster3DResNameByID(this.ExtensionID);
			}
			if (text.IndexOf("Monster_048") >= 0)
			{
				this._the3DGameObject.AddComponent<LoadRoleShaderAgain>();
			}
			if (Global.IsWangZheZhanChangQi(this.ExtensionID))
			{
				int num2;
				Point point;
				Global.IsFixedDirectionMonsterInWangZheZhanChang(this._ExtensionID, this.cx, this.cy, out num2, out point, out this.isFixed);
				if (this.isFixed)
				{
					this.Trans.localPosition = new Vector3((float)point.X / 100f, this.Trans.localPosition.y + 1.1f, (float)point.Y / 100f);
					this.Trans = null;
				}
			}
			MonsterCachingManager.AddCachingItem(this.ExtensionID, go, this.IsCloneRole);
			ConfigMonsters.AddMonsterPlaySound(this.ExtensionID, this);
			this.SetParentAgain(this.Trans);
			this.DisplayAnimationComponent();
			this.LoadReal3DResOK = true;
			this.LoadReal3DWeaponOk = true;
			if (Global.IsInJingLingMap())
			{
				JingLingMap.inst.MonsterLoaderComplete(this, loader, go);
			}
		}

		private void DisplayAnimationComponent()
		{
			Animation component = this._the3DGameObject.GetComponent<Animation>();
			if (component != null && !component.enabled)
			{
				component.enabled = true;
			}
			Animator component2 = this._the3DGameObject.GetComponent<Animator>();
			if (component2 != null && !component2.enabled)
			{
				component2.enabled = true;
			}
		}

		private void ComposeNPC()
		{
			MonsterNPCLoaderData monsterNPCLoaderData = new MonsterNPCLoaderData();
			monsterNPCLoaderData.resName = ConfigNPCs.GetNPC3DResNameByID(this.ExtensionID);
			monsterNPCLoaderData.spriteType = GSpriteTypes.NPC;
			Global.HandleHandWeapon(monsterNPCLoaderData, this.ExtensionID, this.SpriteType);
			this.monsterNPCResLoader = new MonsterNPCResLoader(monsterNPCLoaderData, new OnLoadMonsterNPCComplete(this.NpcLoaderComplete));
		}

		public void NpcLoaderComplete(MonsterNPCLoaderData loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			this.HandleSpriteResComplete(go);
			this._the3DGameObject = go;
			string bundleID = MuAssetManager.GetBundleID("Decoration", "yingzi");
			GameObject emptyLoader = U3DUtils.GetEmptyLoader("yingzi", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			emptyLoader.transform.localPosition = new Vector3(0f, 0.05f, 0f);
			AssetbundleLoader component = emptyLoader.GetComponent<AssetbundleLoader>();
			if (component)
			{
				component.AutoDestroySelf = true;
				component.KeepOwnRenderQueue = true;
			}
			U3DUtils.AddChild(this._the3DGameObject, emptyLoader, true);
			OwnerTypeManager ownerTypeManager = this._the3DGameObject.AddComponent<OwnerTypeManager>();
			ownerTypeManager.OwnerObject = this;
			this.Trans = this._the3DGameObject.transform;
			this._RB = this._the3DGameObject.AddComponent<Rigidbody>();
			this._RB.isKinematic = true;
			U3DUtils.AddBoxCollider(this._the3DGameObject, new Vector3(0f, 1.2f, 0f), new Vector3(0.75f, 2.4f, 0.75f), true);
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(this.ExtensionID);
			if (npcvobyID != null && npcvobyID.IsCollide)
			{
				U3DUtils.AddBoxCollider(this._the3DGameObject, npcvobyID.CollideCenter, npcvobyID.CollideSize, true);
			}
			Vector3 groundPos = this.GetGroundPos((float)this._cx / 100f, (float)this._cy / 100f, 50f);
			this.Trans.localPosition = new Vector3((float)this._cx / 100f, groundPos.y, (float)this._cy / 100f);
			Global.AddSpecialGameObjects4NPC(this._the3DGameObject, this.ExtensionID);
			if (!Global.IsInJingLingMap() && !Global.IsInKuaFuPlunderMainMap())
			{
				Global.ReplaceMaterials4NPC(go, this.ExtensionID);
			}
			NPCInfoDisplay npcinfoDisplay = go.AddComponent<NPCInfoDisplay>();
			npcinfoDisplay.Target = go.transform;
			npcinfoDisplay.NPCNameText = this.VSName;
			this.HeadDispText = go.AddComponent<HeadDisplayText>();
			this.HeadDispText.Target = go.transform;
			go.name = this.Name;
			if (Global.GetMapSceneUIClass() == SceneUIClasses.MoYu && Global.DicThemeActivityNpc.ContainsKey(this.ExtensionID))
			{
				int monstersID = Global.DicThemeActivityNpc[this.ExtensionID].MonstersID;
				string[] array = Global.GetZhuTiFuBossTime(Global.DicThemeActivityNpc[this.ExtensionID].MapId, monstersID).Split(new char[]
				{
					':'
				});
				int num = array[0].SafeToInt32(0) * 3600 + array[1].SafeToInt32(0) * 60;
				int num2 = Global.GetCorrectDateTime().Hour * 3600 + Global.GetCorrectDateTime().Minute * 60 + Global.GetCorrectDateTime().Second;
				if (this.SpriteType == GSpriteTypes.NPC && this.The3DGameObject != null && this.The3DGameObject.GetComponent<NPCInfoDisplay>() != null)
				{
					if (num >= num2)
					{
						this.The3DGameObject.GetComponent<NPCInfoDisplay>().Time = num - num2;
					}
					else if (num < num2)
					{
						this.The3DGameObject.GetComponent<NPCInfoDisplay>().Time = num + 86400 - num2;
					}
				}
			}
			this._NetAudioSource = go.AddComponent<NetAudioSource>();
			this.mAnimatorComponent = go.GetComponent<Animator>();
			if (this.mAnimatorComponent != null)
			{
				if (this._NpcStateMgr == null)
				{
					this._NpcStateMgr = new NpcStateController(this);
				}
				this.mAnimatorComponent.speed = DataObject.Instance.GetMonsterSpeed(this.ExtensionID);
			}
			if (this._NpcStateMgr == null)
			{
				this._AnimationMgr = this._the3DGameObject.AddComponent<AnimationManager>();
				this._AnimationMgr.PrepareAnimation = new AnimationChangeEventHandler(this.PrepareAnimation);
				this._AnimationMgr.PlayingAnimation = new AnimationChangeEventHandler(this.PlayingAnimation);
				this._AnimationMgr.EndAnimation = new AnimationChangeEventHandler(this.EndAnimation);
				U3DUtils.ModifyAnimationSpeed(go, DataObject.Instance.GetMonsterSpeed(this.ExtensionID));
			}
			float num3 = (float)ConfigNPCs.GetNPC3DResScaleByID(this.ExtensionID);
			if (num3 != 1f && num3 != 0f)
			{
				go.transform.localScale = new Vector3(num3, num3, num3);
			}
			U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer(this._DefaultLayerName), null);
			this.MeshSize = U3DUtils.GetMeshSize(go.transform);
			Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
			this.Trans.rotation = quaternionByDir;
			this.LoadReal3DResOK = true;
			this.LoadReal3DWeaponOk = true;
			if (this.SpriteType == GSpriteTypes.NPC && !string.IsNullOrEmpty(this.PlaySoundURL))
			{
				this._NpcSoundTicksSlot = (long)ConfigNPCs.GetNPCSoundIntervalByID(this.ExtensionID) * 1000L;
				this._NpcPlayingMusicFile = this.PlaySoundURL;
				this._LastPlaySpriteSoundTicks = Global.GetCorrectLocalTime();
				this.PlaySpriteSound(this.PlaySoundURL, false);
			}
			this.SetParentAgain(this.Trans);
			Global.HandleNpcWingsAndPet(go, this.ExtensionID, new OnWingsLoadComplete(this.WingsLoaderComplete), new OnShouHuChongLoadComplete(this.ShouHuChongLoaderComplete));
			if (this.ExtensionID >= 91000 && this.ExtensionID <= 91003)
			{
				int num4 = Global.SetShuijingEffect(this.ExtensionID % 100 + 1);
				int num5 = this.ExtensionID % 100 * 3 + 13000;
				string bundleID2 = MuAssetManager.GetBundleID("Decoration", ConfigDecoration.GetDecorationVOByCode((num4 != 0) ? (num4 - 1 + num5) : num5).ResName);
				GameObject emptyLoader2 = U3DUtils.GetEmptyLoader(ConfigDecoration.GetDecorationVOByCode((num4 != 0) ? (num4 - 1 + num5) : num5).ResName, bundleID2, false, null, null, -1, null, -1, 1f, true, false, null);
				emptyLoader2.transform.localPosition = new Vector3(0f, 0f, 0f);
				emptyLoader2.GetComponent<AssetbundleLoader>().AutoDestroySelf = false;
				Global.shuijingtexiao = emptyLoader2;
				U3DUtils.AddChild(this._the3DGameObject, emptyLoader2, true);
			}
			if (Global.IsWangZheZhanChangQiZuo(this.ExtensionID))
			{
				npcinfoDisplay.gameObject.SetActive(false);
			}
			go.AddComponent<LoadRoleShaderAgain>();
			if (this.NpcStateMgr != null)
			{
				this.NpcStateMgr.SetState(NPCAnimState.Stand);
			}
			if (Global.IsInJingLingMap())
			{
				JingLingMap.inst.NpcLoaderComplete(this, loader, go);
			}
			if (Global.IsInKuaFuPlunderMainMap())
			{
				KuaFuPlunderMap.GetInstance().NpcLoaderComplete(this, loader, go);
			}
		}

		private void ComposeJunQi()
		{
			GameObject gameObject = new GameObject("TempJunQiLoader");
			if (null != gameObject)
			{
				XElement xelement = Global.GetXElement(Global.GetGameResXml("Config/JunQi.Xml"), "Item", "ID", this.ExtensionID.ToString());
				if (this.junqiResLoader != null)
				{
					this.junqiResLoader.Stop();
				}
				this.junqiResLoader = U3DUtils.LoadJunQi(gameObject, Global.GetXElementAttributeStr(xelement, "ResName"), new OnWingsLingYuLoadComplete(this.JunQiLoaderComplete));
			}
		}

		private void JunQiLoaderComplete(WingsLingYuLoadData loader, GameObject go)
		{
			if (null == go)
			{
				return;
			}
			this._the3DGameObject = go;
			string bundleID = MuAssetManager.GetBundleID("Decoration", "yingzi");
			GameObject emptyLoader = U3DUtils.GetEmptyLoader("yingzi", bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			emptyLoader.transform.localPosition = new Vector3(0f, 0.05f, 0f);
			AssetbundleLoader component = emptyLoader.GetComponent<AssetbundleLoader>();
			if (component)
			{
				component.AutoDestroySelf = true;
				component.KeepOwnRenderQueue = true;
			}
			U3DUtils.AddChild(this._the3DGameObject, emptyLoader, true);
			OwnerTypeManager ownerTypeManager = this._the3DGameObject.AddComponent<OwnerTypeManager>();
			ownerTypeManager.OwnerObject = this;
			this.Trans = this._the3DGameObject.transform;
			this._RB = this._the3DGameObject.AddComponent<Rigidbody>();
			this._RB.isKinematic = true;
			U3DUtils.AddBoxCollider(this._the3DGameObject, new Vector3(0f, 1.2f, 0f), new Vector3(0.75f, 2.4f, 0.75f), true);
			XElement xelement = Global.GetXElement(Global.GetGameResXml("Config/JunQi.Xml"), "Item", "ID", this.ExtensionID.ToString());
			string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "Collide");
			if (!string.IsNullOrEmpty(xelementAttributeStr))
			{
				string[] array = xelementAttributeStr.Split(new char[]
				{
					';'
				});
				if (array.Length == 2)
				{
					string[] array2 = array[0].Split(new char[]
					{
						','
					});
					Vector3 center;
					center..ctor(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
					array2 = array[1].Split(new char[]
					{
						','
					});
					Vector3 size;
					size..ctor(Convert.ToSingle(array2[0]), Convert.ToSingle(array2[1]), Convert.ToSingle(array2[2]));
					U3DUtils.AddBoxCollider(this._the3DGameObject, center, size, true);
				}
			}
			Vector3 groundPos = this.GetGroundPos((float)this._cx / 100f, (float)this._cy / 100f, 50f);
			this.Trans.localPosition = new Vector3((float)this._cx / 100f, groundPos.y, (float)this._cy / 100f);
			Global.AddSpecialGameObjects4NPC(this._the3DGameObject, this.ExtensionID);
			Global.ReplaceMaterials4NPC(go, this.ExtensionID);
			JunQiInfoDisplay junQiInfoDisplay = go.AddComponent<JunQiInfoDisplay>();
			junQiInfoDisplay.Target = go.transform;
			junQiInfoDisplay.JunQiNameText = this.VSName;
			this.HeadDispText = go.AddComponent<HeadDisplayText>();
			this.HeadDispText.Target = go.transform;
			go.name = this.Name;
			this._NetAudioSource = go.AddComponent<NetAudioSource>();
			U3DUtils.ModifyAnimationSpeed(go, DataObject.Instance.GetMonsterSpeed(this.ExtensionID));
			this._AnimationMgr = this._the3DGameObject.AddComponent<AnimationManager>();
			this._AnimationMgr.PrepareAnimation = new AnimationChangeEventHandler(this.PrepareAnimation);
			this._AnimationMgr.PlayingAnimation = new AnimationChangeEventHandler(this.PlayingAnimation);
			this._AnimationMgr.EndAnimation = new AnimationChangeEventHandler(this.EndAnimation);
			U3DUtils.ReplaceLayerInChildren(go, LayerMask.NameToLayer(this._DefaultLayerName), null);
			this.MeshSize = U3DUtils.GetMeshSize(go.transform);
			Quaternion quaternionByDir = Global.GetQuaternionByDir(this.Direction);
			this.Trans.rotation = quaternionByDir;
			this.LoadReal3DResOK = true;
			this.LoadReal3DWeaponOk = true;
			this.SetParentAgain(this.Trans);
			go.AddComponent<LoadRoleShaderAgain>();
		}

		public void AddHeadText(object obj, Color c, float stayDuration, float offsetX = 0f, float fontSize = 0f, HUDTextCustom.TextType textType = HUDTextCustom.TextType.Normal)
		{
			if (!this._Visiblity)
			{
				return;
			}
			if (null == this.HeadDispText)
			{
				return;
			}
			this.HeadDispText.Add(obj, c, stayDuration, offsetX, fontSize, textType);
		}

		public void ShowAllWeapon()
		{
			if (this.WeaponList == null || this.WeaponList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this.WeaponList.Count; i++)
			{
				if (!(null == this.WeaponList[i]))
				{
					if (!this.WeaponList[i].activeSelf)
					{
						this.WeaponList[i].SetActive(true);
					}
				}
			}
		}

		public void IsShowAllWeapon(bool isShow)
		{
			if (this.WeaponList == null || this.WeaponList.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < this.WeaponList.Count; i++)
			{
				if (!(null == this.WeaponList[i]))
				{
					this.WeaponList[i].SetActive(isShow);
				}
			}
		}

		public bool HasWeapons()
		{
			if (this.WeaponList == null || this.WeaponList.Count <= 0)
			{
				return false;
			}
			for (int i = 0; i < this.WeaponList.Count; i++)
			{
				if (!(null == this.WeaponList[i]))
				{
					return true;
				}
			}
			return false;
		}

		public void ChangeWeaponsPosition(bool inSafe)
		{
			this.IsInSafeRegion = inSafe;
			this.ChangeWeaponsToBack(this.IsInSafeRegion);
		}

		public void ChangeWeaponsToBack(bool IsToBack)
		{
			if (this.WeaponList == null)
			{
				return;
			}
			if (this.WeaponList == null || this.WeaponList.Count <= 0)
			{
				return;
			}
			if (this.OnHorseEX)
			{
				IsToBack = true;
			}
			List<string> list;
			if (IsToBack)
			{
				list = this.SafeEmptyNamesList;
			}
			else
			{
				list = this.NotSafeEmptyNamesList;
			}
			for (int i = 0; i < this.WeaponList.Count; i++)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(this.The3DGameObject, list[i]);
				if (!(null == gameObject))
				{
					U3DUtils.AddChild(gameObject, this.WeaponList[i], true);
				}
			}
		}

		public void ParseWeaponGoodsLst(List<GoodsData> goodsDataList, out List<GoodsData> weaponGoodsDataList, int Occupation)
		{
			weaponGoodsDataList = new List<GoodsData>();
			if (goodsDataList == null || goodsDataList.Count <= 0)
			{
				return;
			}
			byte b = 0;
			for (int i = goodsDataList.Count - 1; i >= 0; i--)
			{
				int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsDataList[i].GoodsID);
				if (categoriyByGoodsID == 25 && 0 < goodsDataList[i].Using)
				{
					WuQiShiZhuangMoXingVO wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion = ConfigFashion.GetWuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion(goodsDataList[i].GoodsID, Occupation);
					if (wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion != null)
					{
						GoodsData goodsData = goodsDataList[i].Clone();
						GoodsData goodsData2 = goodsDataList[i].Clone();
						goodsData.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Left;
						goodsData.Id = -1;
						goodsData2.GoodsID = wuQiShiZhuangMoXingVOByGoodsIDAndOOccupayion.Right;
						goodsData2.Id = -1;
						goodsData2.BagIndex = 1;
						if (0 < goodsData.GoodsID)
						{
							weaponGoodsDataList.Add(goodsData);
							b += 1;
						}
						if (0 < goodsData2.GoodsID)
						{
							weaponGoodsDataList.Add(goodsData2);
							b += 1;
						}
						break;
					}
				}
			}
			if (0 >= b)
			{
				for (int j = 0; j < goodsDataList.Count; j++)
				{
					if (goodsDataList[j].Using > 0)
					{
						GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsDataList[j].GoodsID);
						if (goodsXmlNodeByID != null)
						{
							int categoriy = goodsXmlNodeByID.Categoriy;
							if (categoriy == 37 || categoriy == 38)
							{
								GoodVO rebornEquipsByGoodsIDAndOccForShengWuAndShengQi = IConfigbase<ConfigReborn>.Instance.GetRebornEquipsByGoodsIDAndOccForShengWuAndShengQi(goodsDataList[j].GoodsID, Occupation);
								if (rebornEquipsByGoodsIDAndOccForShengWuAndShengQi != null)
								{
									GoodsData goodsData3 = goodsDataList[j].Clone(rebornEquipsByGoodsIDAndOccForShengWuAndShengQi.ID);
									goodsData3.Forge_level = 1;
									weaponGoodsDataList.Add(goodsData3);
								}
							}
							else if (categoriy >= 11 && categoriy < 25)
							{
								int handType = goodsXmlNodeByID.HandType;
								if (handType > -1)
								{
									int actionType = goodsXmlNodeByID.ActionType;
									if (actionType > 0)
									{
										weaponGoodsDataList.Add(goodsDataList[j]);
									}
								}
							}
						}
					}
				}
			}
		}

		public bool ReCalcWeaponState()
		{
			try
			{
				List<GoodsData> goodsDataList = null;
				List<GoodsData> list = null;
				WingData wingData = null;
				bool roleLoaderGoodsDataList = this.GetRoleLoaderGoodsDataList(ref goodsDataList, ref list, ref wingData);
				List<GoodsData> weaponGoodsDataList = new List<GoodsData>();
				int occupation = this.Occupation;
				int subOcc = 0;
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					subOcc = Global.Data.roleData.SubOccupation;
				}
				else if (this.SpriteType == GSpriteTypes.Other)
				{
					RoleData roleData = null;
					if (Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData))
					{
						subOcc = roleData.SubOccupation;
					}
				}
				else if (this.SpriteType == GSpriteTypes.FakeRole)
				{
					FakeRoleData fakeRoleData = null;
					if (Global.Data.FakeRoles.TryGetValue(this.RoleID, ref fakeRoleData))
					{
						subOcc = fakeRoleData.MyRoleDataMini.SubOccupation;
					}
				}
				this.ParseWeaponGoodsLst(goodsDataList, out weaponGoodsDataList, Global.CheckRoleOcc(occupation, subOcc));
				this.WeaponState = Global.CalcWeaponState(weaponGoodsDataList, this.WeaponList, this.Occupation);
				if (roleLoaderGoodsDataList && this.SpriteType == GSpriteTypes.Leader)
				{
					if (Global.Data != null && Global.Data.roleData != null)
					{
						if (Global.GetMapShowEquipType(Global.Data.roleData) == 0)
						{
							foreach (KeyValuePair<int, GoodsData> keyValuePair in Global.GetUsingGoodsDataList())
							{
								int categoriyByGoodsID = Global.GetCategoriyByGoodsID(keyValuePair.Value.GoodsID);
								if (11 <= categoriyByGoodsID && 21 > categoriyByGoodsID && categoriyByGoodsID != 18 && categoriyByGoodsID != 20)
								{
									return true;
								}
							}
						}
						else if (Global.Data.roleData.RebornGoodsDataList != null)
						{
							for (int i = 0; i < Global.Data.roleData.RebornGoodsDataList.Count; i++)
							{
								if (Global.Data.roleData.RebornGoodsDataList[i] != null && Global.Data.roleData.RebornGoodsDataList[i].Using == 1 && (Global.GetCategoriyByGoodsID(Global.Data.roleData.RebornGoodsDataList[i].GoodsID) == 38 || Global.GetCategoriyByGoodsID(Global.Data.roleData.RebornGoodsDataList[i].GoodsID) == 37))
								{
									return true;
								}
							}
						}
					}
					return false;
				}
			}
			catch (Exception ex)
			{
			}
			return true;
		}

		public string PlaySoundURL
		{
			get
			{
				return this._PlaySoundURL;
			}
			set
			{
				this._PlaySoundURL = value;
			}
		}

		public string[] PlayNpcTalkSoundURLs
		{
			get
			{
				return this._PlayNpcTalkSoundURLs;
			}
			set
			{
				this._PlayNpcTalkSoundURLs = value;
			}
		}

		public void RandomPlaySpriteSound(bool isFirstPlay = false)
		{
			if (this.SpriteType != GSpriteTypes.Monster)
			{
				return;
			}
			if (this._Action != GActions.IdleStand && this._Action != GActions.Stand)
			{
				return;
			}
			if (string.IsNullOrEmpty(this._PlaySoundURL))
			{
				return;
			}
			if (!isFirstPlay)
			{
				long correctLocalTime = Global.GetCorrectLocalTime();
				if (this._LastPlaySpriteSoundTicks <= 0L)
				{
					this._LastPlaySpriteSoundTicks = correctLocalTime + (long)(Global.GetRandomNumber(0, 10) * 1000);
					return;
				}
				if (correctLocalTime - this._LastPlaySpriteSoundTicks < 0L)
				{
					return;
				}
				this._LastPlaySpriteSoundTicks = correctLocalTime + (long)(Global.GetRandomNumber(10, 20) * 1000);
			}
			if ((Global.GetRandomNumber(0, 20) == 0 || isFirstPlay) && null != this._NetAudioSource && !this._NetAudioSource.IsPlaying())
			{
				string url = StringUtil.substitute("Audio/Monster/{0}", new object[]
				{
					this._PlaySoundURL
				});
				if (Global.Data.SysSetting.CloseGameAudio)
				{
					this._NetAudioSource.StopPlay();
				}
				else
				{
					this._NetAudioSource.PlayAudio(url, false, false);
				}
			}
		}

		private void PlaySpriteSoundSlotTime()
		{
			if (this.SpriteType != GSpriteTypes.NPC)
			{
				return;
			}
			if (this._NpcSoundTicksSlot < 0L)
			{
				return;
			}
			if (string.IsNullOrEmpty(this._NpcPlayingMusicFile))
			{
				return;
			}
			long correctLocalTime = Global.GetCorrectLocalTime();
			if (correctLocalTime - this._LastPlaySpriteSoundTicks < 0L)
			{
				return;
			}
			this._LastPlaySpriteSoundTicks = correctLocalTime + this._NpcSoundTicksSlot;
			if (this._NetAudioSource == null)
			{
			}
			if (null != this._NetAudioSource && !this._NetAudioSource.IsPlaying())
			{
				if (Global.Data.SysSetting.CloseGameAudio)
				{
					this._NetAudioSource.StopPlay();
				}
				else
				{
					this._NetAudioSource.PlayAudio(this._NpcPlayingMusicFile, false, false);
				}
			}
		}

		public void PlayNpcTalkSound()
		{
			if (this._PlayNpcTalkSoundURLs == null || this._PlayNpcTalkSoundURLs.Length <= 0)
			{
				return;
			}
			int randomNumber = Global.GetRandomNumber(0, this._PlayNpcTalkSoundURLs.Length);
			string playingMusicFile = this._PlayNpcTalkSoundURLs[randomNumber];
			this.PlaySpriteSound(playingMusicFile, false);
		}

		public void PlaySpriteSound(string playingMusicFile, bool loop = false)
		{
			if (null == this._the3DGameObject)
			{
				return;
			}
			if (string.IsNullOrEmpty(playingMusicFile) && null != this._NetAudioSource)
			{
				this._NetAudioSource.StopPlay();
			}
			if (null == this._NetAudioSource)
			{
				this._NetAudioSource = this._the3DGameObject.GetComponent<NetAudioSource>();
				if (null == this._NetAudioSource)
				{
					return;
				}
			}
			if (Global.Data.SysSetting.CloseGameAudio)
			{
				this._NetAudioSource.StopPlay();
			}
			else
			{
				this._NetAudioSource.StopPlay();
				this._NetAudioSource.PlayAudio(playingMusicFile, loop, false);
			}
		}

		public void StopSpriteSound()
		{
			if (null == this._NetAudioSource)
			{
				return;
			}
			this._NetAudioSource.StopPlay();
		}

		private void FreeStoppedSound()
		{
			if (null == this._NetAudioSource)
			{
				return;
			}
			if (this._NetAudioSource.IsPlaying())
			{
				return;
			}
			this._NetAudioSource.StopPlay();
			this._NetAudioSource = null;
		}

		public bool InWater
		{
			get
			{
				SettingMapVO settingMapVOByCode = ConfigSettings.GetSettingMapVOByCode(Global.Data.roleData.MapCode);
				return settingMapVOByCode != null && ((this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.FakeRole) && settingMapVOByCode.MoveType == 1) && !this.IsInSafeRegion;
			}
		}

		public void ChangeWingsActionByAnimator(GActions action)
		{
			if (null == this.WingsAnimator || !this.WingsAnimator.isInitialized)
			{
				return;
			}
			if (action == GActions.Stand)
			{
				this.WingsAnimator.SetBool("Stand", true);
				this.WingsAnimator.SetBool("Run", false);
			}
			else if (action == GActions.Run)
			{
				this.WingsAnimator.SetBool("Run", true);
				this.WingsAnimator.SetBool("Stand", false);
			}
		}

		public void ChangeWingsAction(GActions action)
		{
			if (this.WingsAnimator != null)
			{
				this.ChangeWingsActionByAnimator(action);
			}
			else
			{
				this.ChangeWingsActionByAnimation(action);
			}
		}

		public void ChangeWingsActionByAnimation(GActions action)
		{
			if (null == this.WingsAnimation)
			{
				return;
			}
			if (action == GActions.Stand)
			{
				this.WingsAnimation.CrossFade("Stand");
			}
			else if (action == GActions.Run)
			{
				this.WingsAnimation.CrossFade("Run");
			}
		}

		public void ChangePetAction(GActions action)
		{
			if (null == this.shouHuChongController)
			{
				return;
			}
			if (action == GActions.Run || action == GActions.Walk)
			{
				this.shouHuChongController.Action = GPetActions.Walk;
			}
			else
			{
				this.shouHuChongController.Action = GPetActions.Stand;
			}
		}

		public void DestroyShouHuChong()
		{
			if (this.shouHuChongController != null)
			{
				this.shouHuChongController.Dispose();
				Object.Destroy(this.shouHuChongController);
				this.shouHuChongController = null;
			}
		}

		public Vector3 GetOrigMeshSize()
		{
			return this.MeshSize;
		}

		public string DefaultLayerName
		{
			get
			{
				return this._DefaultLayerName;
			}
			set
			{
				this._DefaultLayerName = value;
			}
		}

		public void ModifyLayers(string layerName, string[] ignoreList = null)
		{
			this._DefaultLayerName = layerName;
			if (null != this._the3DGameObject)
			{
				U3DUtils.ReplaceLayerInChildren(this._the3DGameObject, LayerMask.NameToLayer(this._DefaultLayerName), ignoreList);
			}
		}

		private bool ProcessDead()
		{
			if (!Global.IsInKuaFuTeamCompete())
			{
				this.isDeathInKuaFuTeamCompete = true;
			}
			if (this.IsDeath)
			{
				int num = Global.GetMyTimer() - this.deathDelay;
				if (this.SpriteType != GSpriteTypes.Leader && this.SpriteType != GSpriteTypes.Other && this.SpriteType != GSpriteTypes.FakeRole)
				{
					int num2 = 3000;
					if (num >= num2)
					{
						if (this.SpriteDead != null)
						{
							this.VLife = 0.0;
							this.SpriteDead.Invoke(this, EventArgs.Empty);
						}
						Global.RemoveObject(this, true);
					}
					if (this.SpriteType == GSpriteTypes.Monster)
					{
						GScene.CheckMonsterForZhaoHuanShou(this.RoleID, 0);
					}
				}
				else
				{
					int num2 = 1000;
					if (this.deathDelay != 0)
					{
						if (num >= num2)
						{
							this.deathDelay = 0;
							if (this.SpriteDead != null)
							{
								this.VLife = 0.0;
								this.SpriteDead.Invoke(this, EventArgs.Empty);
							}
						}
					}
					else if (Global.Data.roleData != null && Global.Data.roleData.LifeV <= 0 && null == PlayZone.GlobalPlayZone.m_FuhuoPanle)
					{
						if (Global.IsInKuaFuTeamCompete())
						{
							if (this.isDeathInKuaFuTeamCompete)
							{
								this.isDeathInKuaFuTeamCompete = false;
								if (this.SpriteDead != null)
								{
									this.VLife = 0.0;
									this.SpriteDead.Invoke(this, EventArgs.Empty);
								}
								this.HideDeadBody();
								if (PlayZone.GlobalPlayZone.GameTaskBoxMini != null && Global.IsInKuaFuTeamCompete())
								{
									PlayZone.GlobalPlayZone.GameTaskBoxMini.IsShowLblTeamCompeteGuanZhan = true;
								}
							}
						}
						else
						{
							this.deathDelay = Global.GetMyTimer() + 1000;
						}
					}
				}
				return true;
			}
			if (this.SpriteType == GSpriteTypes.Leader && this._Action == GActions.Death && Global.Data.roleData != null && Global.Data.roleData.LifeV <= 0)
			{
				this.IsDeath = true;
				if (Global.IsInKuaFuTeamCompete())
				{
					if (this.isDeathInKuaFuTeamCompete)
					{
						this.deathDelay = 0;
					}
				}
				else
				{
					this.deathDelay = Global.GetMyTimer();
				}
			}
			return false;
		}

		private void HideDeadBody()
		{
			GSprite gsprite = Global.FindSpriteByID(Global.Data.roleData.RoleID);
			Transform transform = gsprite.The3DGameObject.transform;
			if (transform != null)
			{
				int childCount = transform.childCount;
				if (childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
				{
					for (int i = 0; i < childCount; i++)
					{
						transform.GetChild(i).gameObject.SetActive(false);
					}
					SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
					if (component)
					{
						component.enabled = false;
					}
					BoxCollider component2 = transform.GetComponent<BoxCollider>();
					if (component2)
					{
						component2.enabled = false;
					}
				}
			}
		}

		public int DirectGetAttackNum
		{
			get
			{
				int attackNum = this.AttackNum;
				int attackNum2 = this.GetAttackNum(3);
				this.AttackNum = attackNum;
				return attackNum2;
			}
		}

		public int GetAttackNum(int action)
		{
			if (action != 3)
			{
				return this.AttackNum;
			}
			long num = 0L;
			if (this.SpriteType != GSpriteTypes.Leader)
			{
				num += 300L;
			}
			bool flag = (this.SpriteType == GSpriteTypes.Leader) ? (this.GetElapsedLastAttackTicks() < 600L) : (this.GetElapsedLastAttackTicks() < 600L + num);
			if (this.SpriteType != GSpriteTypes.Leader)
			{
				this.LastAttackTicks = TimeManager.GetCorrectLocalTime();
			}
			if (flag)
			{
				this.AttackNum++;
				if (this.AttackNum > 5)
				{
					this.AttackNum = 1;
				}
				return this.AttackNum;
			}
			this.AttackNum = 1;
			return this.AttackNum;
		}

		private void HandleSpriteResComplete(GameObject go)
		{
			if (this.SpriteType == GSpriteTypes.Monster)
			{
				if (Global.IsBloodCastleChengMen(this.ExtensionID))
				{
					GameObject gameObject = U3DUtils.FindGameObjectByName(null, "/Sprites/Monster_chenbaomen");
					if (null != gameObject)
					{
						gameObject.SetActive(false);
					}
				}
				else if (Global.IsBloodCastleLingGuan(this.ExtensionID))
				{
					GameObject gameObject2 = U3DUtils.FindGameObjectByName(null, "/Sprites/19_xscb_common_shuijinguancai_001");
					if (null != gameObject2)
					{
						gameObject2.SetActive(false);
					}
				}
			}
		}

		public int YAngle
		{
			get
			{
				return Global.GetYAngle(this.Rotation);
			}
			set
			{
				if (value >= 0)
				{
					this.Rotation = Global.GetRotaionByAngle(value);
				}
			}
		}

		private void ProcessIdleAction()
		{
			this.TotalIdleTime += Time.deltaTime;
			if (this.TotalIdleTime >= 5f)
			{
				this.TotalIdleTime = 0f;
				if (null != this._AnimationMgr && Global.GetRandomNumber(0, 2) == 0 && (this.SpriteType == GSpriteTypes.NPC || this.SpriteType == GSpriteTypes.Monster))
				{
					if (this.SpriteType == GSpriteTypes.NPC)
					{
						this._AnimationMgr.CrossFadeTime = 0f;
					}
					if (this._AnimationMgr != null && this._AnimationMgr.GetComponent<Animation>() != null && this._AnimationMgr.GetComponent<Animation>()["Relax"] != null && this.Action == GActions.Stand)
					{
						this.Action = GActions.IdleStand;
					}
				}
			}
		}

		private void ProcessMonsterRunAction()
		{
			if (this.SpriteType == GSpriteTypes.Monster && this.Trans != null)
			{
				if (this.monsterTempVec == Vector3.zero)
				{
					this.monsterTempVec = this.Trans.localPosition;
				}
				if (Global.FindStoryboard(this.Name) == null && this.monsterTempVec == this.Trans.localPosition)
				{
					if (this.Action == GActions.Run || this.Action == GActions.Walk)
					{
						this.TotalRunTime += Time.deltaTime;
						if (this.TotalRunTime >= 0.5f)
						{
							this.TotalRunTime = 0f;
							this.Action = GActions.Stand;
						}
					}
				}
				else
				{
					this.Action = GActions.Walk;
					if (this.Trans != null)
					{
						this.monsterTempVec = this.Trans.localPosition;
					}
				}
			}
		}

		private void LoadPKloversBuffer(GSprite sprite, int roleID)
		{
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			if (Global.IsBufferExist(2080010, roleData))
			{
				int bufferDecID = Global.GetBufferDecID(2080010);
				this.AddBufferDeco(sprite, bufferDecID);
			}
			if (Global.IsBufferExist(2080011, roleData))
			{
				int bufferDecID2 = Global.GetBufferDecID(2080011);
				this.AddBufferDeco(sprite, bufferDecID2);
			}
		}

		private void AddBufferDeco(GSprite sprite, int DecorationCode)
		{
			string name = string.Format("BufferDecoration_{0}", DecorationCode);
			GDecoration gdecoration = sprite.Root.FindName(name) as GDecoration;
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			gdecoration = (ObjectsManager.FindSprite(name) as GDecoration);
			if (gdecoration != null)
			{
				Global.RemoveObject(gdecoration, true);
			}
			Point pos = new Point(sprite.CenterX, sprite.CenterY);
			gdecoration = Global.GetDecoration(DecorationCode, GDecorationTypes.Loop, pos, false, null, -1, -1, true, false);
			if (gdecoration != null)
			{
				gdecoration.Name = name;
				if (gdecoration.HangPos == 1)
				{
					sprite.Root.Children.Add(gdecoration);
				}
				else
				{
					gdecoration.cx = sprite.cx;
					gdecoration.cy = sprite.cy;
					ObjectsManager.Add(gdecoration);
				}
			}
		}

		private void LoadLingDiBufferDes(GSprite sprite, int roleID)
		{
			RoleData roleData = null;
			if (roleID == Global.Data.roleData.RoleID)
			{
				roleData = Global.Data.roleData;
			}
			else if (!Global.Data.OtherRoles.TryGetValue(roleID, ref roleData))
			{
				return;
			}
			if (roleData == null)
			{
				return;
			}
			if (Global.IsBufferExist(2080002, roleData))
			{
				int bufferDecID = Global.GetBufferDecID(2080002);
				this.AddBufferDeco(sprite, bufferDecID);
			}
		}

		public GDecoration GDecorationEmblem
		{
			get
			{
				return this.mGDecorationEmblem;
			}
			set
			{
				if (this.mGDecorationEmblem != value && this.mGDecorationEmblem != null)
				{
					Global.RemoveObject(this.mGDecorationEmblem, true);
				}
				if (value != null)
				{
					this.mGDecorationEmblem = value;
					if (null != this.mGDecorationEmblem.The3DGameObject)
					{
						if (this.Load3DResOK)
						{
							RoleData roleData;
							if (this.SpriteType == GSpriteTypes.Leader)
							{
								roleData = Global.Data.roleData;
							}
							else
							{
								roleData = Global.Data.OtherRoles.GetValue(this.RoleID);
							}
							if (Global.IsBufferExist(121, roleData))
							{
								float huiJiScale = ShenHunData.GetHuiJiScale(roleData);
								this.mGDecorationEmblem.The3DGameObject.transform.localScale = new Vector3(huiJiScale, huiJiScale, huiJiScale);
							}
							else
							{
								this.mGDecorationEmblem.The3DGameObject.transform.localScale = Vector3.one;
							}
							this.Add(this.mGDecorationEmblem);
							this.mGDecorationEmblem.Tag = this;
						}
					}
					else
					{
						this.mGDecorationEmblem.DecorationLoadCompleteNotify = delegate(object e, EventArgs s)
						{
							this.GDecorationEmblem = this.mGDecorationEmblem;
						};
					}
				}
			}
		}

		public GDecoration GDecorationEmblemShan
		{
			get
			{
				return this.mGDecorationEmblemShan;
			}
			set
			{
				if (this.mGDecorationEmblemShan != value && this.mGDecorationEmblemShan != null)
				{
					Global.RemoveObject(this.mGDecorationEmblemShan, true);
				}
				if (value != null)
				{
					this.mGDecorationEmblemShan = value;
					if (null != this.mGDecorationEmblemShan.The3DGameObject)
					{
						if (this.Load3DResOK)
						{
							this.Add(this.mGDecorationEmblemShan);
							this.mGDecorationEmblemShan.Tag = this;
						}
					}
					else
					{
						this.mGDecorationEmblemShan.DecorationLoadCompleteNotify = delegate(object e, EventArgs s)
						{
							this.GDecorationEmblemShan = this.mGDecorationEmblemShan;
						};
					}
				}
			}
		}

		public void PlayMonsterAttackSound()
		{
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.ExtensionID);
			string attackSound = monsterXmlNodeByID.AttackSound;
			if (!string.IsNullOrEmpty(attackSound))
			{
				string playingMusicFile = StringUtil.substitute("Audio/Monster/{0}", new object[]
				{
					attackSound
				});
				this.PlaySpriteSound(playingMusicFile, false);
			}
		}

		public void PlayMonsterHitSound()
		{
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.ExtensionID);
			string hitSound = monsterXmlNodeByID.HitSound;
			if (!string.IsNullOrEmpty(hitSound))
			{
				string playingMusicFile = StringUtil.substitute("Audio/Monster/{0}", new object[]
				{
					hitSound
				});
				this.PlaySpriteSound(playingMusicFile, false);
			}
		}

		private void PrepareAnimation(object sender, EventArgs args)
		{
			MUDebug.Log<string>(new string[]
			{
				"开始播放 = " + this._Action
			});
			GActions action = this._Action;
			switch (action)
			{
			case GActions.Attack:
				if (this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole)
				{
					this.ReleaseAttackEffect();
				}
				else if (this.SpriteType == GSpriteTypes.Monster)
				{
					this.DoMagicDecoration();
					MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(this.ExtensionID);
					string attackSound = monsterXmlNodeByID.AttackSound;
					if (!string.IsNullOrEmpty(attackSound))
					{
						string playingMusicFile = StringUtil.substitute("Audio/Monster/{0}", new object[]
						{
							attackSound
						});
						this.PlaySpriteSound(playingMusicFile, false);
					}
				}
				break;
			case GActions.Injured:
				if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.FakeRole)
				{
					string text = "injured_nan.mp3";
					int num = Global.CalcOriginalOccupationID(this.Occupation);
					if (num == 2 || num == 5)
					{
						text = "injured_nv.mp3";
					}
					string playingMusicFile2 = StringUtil.substitute("Audio/Action/{0}", new object[]
					{
						text
					});
					this.PlaySpriteSound(playingMusicFile2, false);
				}
				else if (this.SpriteType == GSpriteTypes.Monster)
				{
					MonsterVO monsterXmlNodeByID2 = ConfigMonsters.GetMonsterXmlNodeByID(this.ExtensionID);
					string hitSound = monsterXmlNodeByID2.HitSound;
					if (!string.IsNullOrEmpty(hitSound))
					{
						string playingMusicFile3 = StringUtil.substitute("Audio/Monster/{0}", new object[]
						{
							hitSound
						});
						this.PlaySpriteSound(playingMusicFile3, false);
					}
				}
				break;
			default:
				if (action == GActions.Death)
				{
					if (this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.Other || this.SpriteType == GSpriteTypes.FakeRole)
					{
						string text2 = "death_nan.mp3";
						int num2 = Global.CalcOriginalOccupationID(this.Occupation);
						if (num2 == 2 || num2 == 5)
						{
							text2 = "death_nv.mp3";
						}
						string playingMusicFile4 = StringUtil.substitute("Audio/Action/{0}", new object[]
						{
							text2
						});
						this.PlaySpriteSound(playingMusicFile4, false);
					}
					else if (this.SpriteType == GSpriteTypes.Monster)
					{
						MonsterVO monsterXmlNodeByID3 = ConfigMonsters.GetMonsterXmlNodeByID(this.ExtensionID);
						if (monsterXmlNodeByID3 != null && monsterXmlNodeByID3.DieAnimation > 0)
						{
							GDecoration decoration = Global.GetDecoration(monsterXmlNodeByID3.DieAnimation, GDecorationTypes.AutoRemove, new Point(0, 0), false, null, -1, -1, true, false);
							if (decoration != null)
							{
								return;
							}
						}
						string text3 = string.Empty;
						if (monsterXmlNodeByID3 != null)
						{
							text3 = monsterXmlNodeByID3.DieSound;
						}
						if (!string.IsNullOrEmpty(text3))
						{
							string playingMusicFile5 = StringUtil.substitute("Audio/Monster/{0}", new object[]
							{
								text3
							});
							this.PlaySpriteSound(playingMusicFile5, false);
						}
						if (Global.IsBloodCastleChengMen(this.ExtensionID))
						{
							int code = 79;
							GDecoration decoration2 = Global.GetDecoration(code, GDecorationTypes.Loop, this.Coordinate, true, null, -1, -1, true, false);
						}
					}
				}
				break;
			case GActions.Magic:
				this.DoMagicDecoration();
				break;
			}
		}

		private void ReleaseAttackEffect()
		{
			this.CurrentMagic = this.GetLianZhaoMagicCode();
			int lianZhaoDecorationCode = this.GetLianZhaoDecorationCode();
			if (lianZhaoDecorationCode >= 0)
			{
				int num = Global.CalcOriginalOccupationID(this.Occupation);
				GDecoration decoration = Global.GetDecoration(lianZhaoDecorationCode, GDecorationTypes.AutoRemove, new Point(0, 0), false, this.Name, (num != 0) ? this.CurrentMagic : lianZhaoDecorationCode, -1, true, false);
				if (null == decoration.The3DGameObject)
				{
					Debug.LogError(lianZhaoDecorationCode + "这个特效没有哇");
					return;
				}
				decoration.The3DGameObject.transform.localPosition = this.Trans.localPosition;
				GSprite gsprite = (this.LockObject == null) ? null : (ObjectsManager.FindSprite(this.LockObject) as GSprite);
				if (gsprite != null && null != gsprite.Trans)
				{
					Vector3 localPosition = gsprite.Trans.localPosition;
					decoration.The3DGameObject.transform.localRotation = Quaternion.LookRotation(localPosition - this.Trans.localPosition, Vector3.up);
				}
				else
				{
					decoration.The3DGameObject.transform.localRotation = this.Rotation;
				}
				ObjectsManager.Add(decoration);
			}
		}

		public void DoMagicDecoration()
		{
			if (this.CurrentMagic >= 0)
			{
				MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(this.CurrentMagic);
				if (maigcInfoVOByCode != null)
				{
					int num = Global.GetMagicCode(maigcInfoVOByCode, "MagicCode", this);
					bool isCache = false;
					if ((this.SpriteType == GSpriteTypes.Leader || this.SpriteType == GSpriteTypes.FakeRole || this.SpriteType == GSpriteTypes.Other) && this.Action == GActions.Magic && this.CurrentMagic != 104 && this.CurrentMagic != 205 && this.CurrentMagic != 304)
					{
						isCache = true;
					}
					if (num >= 0)
					{
						DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(num);
						string resName = decorationVOByCode.ResName;
						int num2 = 0;
						if (!GSprite.ActiveMagicDecorations.TryGetValue(resName, ref num2))
						{
							GSprite.ActiveMagicDecorations.Add(resName, 0);
						}
						if (this.SpriteType != GSpriteTypes.Leader && (num2 > 0 || GSprite.TotalMagicDecorations > 8))
						{
							num = -1;
						}
					}
					if (num >= 0)
					{
						GDecoration decoration = Global.GetDecoration(num, GDecorationTypes.AutoRemove, new Point(0, 0), false, this.Name, this.CurrentMagic, -1, num != 48 && 17 != num, isCache);
						if (decoration.HangPos == 1)
						{
							if (num == 48 && this.WeaponList != null && this.WeaponList.Count > 0)
							{
								for (int i = 0; i < this.WeaponList.Count; i++)
								{
									if (!(null == this.WeaponList[i]))
									{
										if (this.WeaponList[i].transform.parent.name == "youshou")
										{
											decoration.WuQi_To_PiLiHuiXuanZhan_Obj = (SpawnManager.Instantiate(this.WeaponList[i]) as GameObject);
											this.WeaponList[i].SetActive(false);
											break;
										}
									}
								}
							}
							this.Add(decoration);
						}
						else
						{
							if (decoration.The3DGameObject == null)
							{
								return;
							}
							decoration.The3DGameObject.transform.localPosition = this.Trans.localPosition;
							decoration.The3DGameObject.transform.localRotation = this.Rotation;
							ObjectsManager.Add(decoration);
						}
					}
					num = Global.GetMagicCode(maigcInfoVOByCode, "FlyDecoration", this);
					if (num >= 0)
					{
						DecorationVO decorationVOByCode2 = ConfigDecoration.GetDecorationVOByCode(num);
						string resName2 = decorationVOByCode2.ResName;
						int num3 = 0;
						if (!GSprite.ActiveMagicDecorations.TryGetValue(resName2, ref num3))
						{
							GSprite.ActiveMagicDecorations.Add(resName2, 0);
						}
						if (this.SpriteType != GSpriteTypes.Leader && (num3 > 0 || GSprite.TotalMagicDecorations > 8))
						{
							num = -1;
						}
					}
					if (num >= 0)
					{
						GDecoration decoration2 = Global.GetDecoration(num, GDecorationTypes.AutoRemove, new Point(0, 0), false, this.Name, this.CurrentMagic, -1, true, isCache);
						if (decoration2.HangPos == 1)
						{
							this.Add(decoration2);
						}
						else
						{
							decoration2.The3DGameObject.transform.localPosition = this.Trans.localPosition;
							GSprite gsprite = (this.LockObject == null) ? null : (ObjectsManager.FindSprite(this.LockObject) as GSprite);
							if (gsprite != null && null != gsprite.The3DGameObject)
							{
								Vector3 localPosition = gsprite.The3DGameObject.transform.localPosition;
								decoration2.The3DGameObject.transform.localRotation = Quaternion.LookRotation(localPosition - this.Trans.localPosition, Vector3.up);
							}
							else
							{
								decoration2.The3DGameObject.transform.localRotation = this.Rotation;
							}
							ObjectsManager.Add(decoration2);
						}
					}
				}
				if (maigcInfoVOByCode != null)
				{
					string playingMusicFile = string.Empty;
					if (this.HasWeapons())
					{
						if (!string.IsNullOrEmpty(maigcInfoVOByCode.MusicWeapon))
						{
							playingMusicFile = StringUtil.substitute("Audio/Skill/{0}", new object[]
							{
								maigcInfoVOByCode.MusicWeapon
							});
						}
					}
					else if (!string.IsNullOrEmpty(maigcInfoVOByCode.MusicNoWeapon))
					{
						playingMusicFile = StringUtil.substitute("Audio/Skill/{0}", new object[]
						{
							maigcInfoVOByCode.MusicNoWeapon
						});
					}
					this.PlaySpriteSound(playingMusicFile, false);
				}
				if (maigcInfoVOByCode != null)
				{
					this.RotateSkillTypeTimes = maigcInfoVOByCode.PubCDTime;
					this.SkillMoveType = (GSkillMoveType)maigcInfoVOByCode.MoveType;
				}
			}
		}

		private void PlayingAnimation(object sender, EventArgs args)
		{
			GActions action = this._Action;
			switch (action)
			{
			case GActions.Attack:
				if (this.Name == "Leader")
				{
					LeaderInfo.InAction = true;
					this.CurrentMagic = this.GetLianZhaoMagicCode();
					if (this.LockObject == null)
					{
						Global.DoInjure(this, null, this.EnemyTarget, this.CurrentMagic);
					}
					else
					{
						GSprite gsprite = ObjectsManager.FindSprite(this.LockObject) as GSprite;
						Point targetPos = this.EnemyTarget;
						if (gsprite != null)
						{
							targetPos = gsprite.Coordinate;
						}
						Global.DoInjure(this, gsprite, targetPos, this.CurrentMagic);
					}
					MagicInfoVO maigcInfoVOByCode = ConfigMagicInfos.GetMaigcInfoVOByCode(this.CurrentMagic);
					if (maigcInfoVOByCode != null && maigcInfoVOByCode != null)
					{
						string playingMusicFile = string.Empty;
						if (this.HasWeapons())
						{
							if (!string.IsNullOrEmpty(maigcInfoVOByCode.MusicWeapon))
							{
								playingMusicFile = StringUtil.substitute("Audio/Skill/{0}", new object[]
								{
									maigcInfoVOByCode.MusicWeapon
								});
							}
						}
						else if (!string.IsNullOrEmpty(maigcInfoVOByCode.MusicNoWeapon))
						{
							playingMusicFile = StringUtil.substitute("Audio/Skill/{0}", new object[]
							{
								maigcInfoVOByCode.MusicNoWeapon
							});
						}
						this.PlaySpriteSound(playingMusicFile, false);
					}
				}
				break;
			case GActions.Injured:
				break;
			default:
				if (action != GActions.Death)
				{
				}
				break;
			case GActions.Magic:
				if (this.CurrentMagic >= 0)
				{
					if (this.Name == "Leader")
					{
						LeaderInfo.InAction = true;
						GSprite gsprite2 = null;
						if (this.LockObject != null)
						{
							gsprite2 = (ObjectsManager.FindSprite(this.LockObject) as GSprite);
						}
						Point targetPos2 = this.EnemyTarget;
						if (gsprite2 != null)
						{
							targetPos2 = gsprite2.Coordinate;
						}
						Global.DoInjure(this, gsprite2, targetPos2, this.CurrentMagic);
					}
					if (this.SpriteType != GSpriteTypes.Monster)
					{
						this.CurrentMagic = -1;
					}
				}
				break;
			}
		}

		private void EndAnimation(object sender, EventArgs args)
		{
			switch (this._Action)
			{
			case GActions.Attack:
				this.LastAttackTicks = TimeManager.GetCorrectLocalTime();
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					LeaderInfo.InAction = false;
				}
				if (this.SpriteChangeAction != null)
				{
					this.SpriteChangeAction(this, new SpriteChangeActionEventArgs
					{
						Action = 3
					});
				}
				return;
			case GActions.Injured:
				this.Action = GActions.Stand;
				return;
			case GActions.Magic:
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					LeaderInfo.InAction = false;
					this.SkillMoveType = GSkillMoveType.None;
				}
				if (this.SpriteChangeAction != null)
				{
					this.SpriteChangeAction(this, new SpriteChangeActionEventArgs
					{
						Action = 6
					});
				}
				return;
			case GActions.Death:
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					LeaderInfo.InAction = false;
				}
				if (!this.IsDeath)
				{
					this.IsDeath = true;
					this.deathDelay = Global.GetMyTimer();
				}
				return;
			case GActions.PreAttack:
				if (this.SpriteChangeAction != null)
				{
					this.SpriteChangeAction(this, new SpriteChangeActionEventArgs
					{
						Action = 24
					});
				}
				return;
			case GActions.IdleStand:
				this.Action = GActions.Stand;
				if (this.SpriteType == GSpriteTypes.Leader)
				{
					LeaderInfo.InAction = false;
				}
				return;
			case GActions.Collect:
			case GActions.Wenhao:
			case GActions.Genwolai:
			case GActions.Guzhang:
			case GActions.Huanhu:
			case GActions.Jushang:
			case GActions.Xingli:
			case GActions.Chongfeng:
			case GActions.Mobai:
			case GActions.Tiaoxin:
			case GActions.Zuoxia:
			case GActions.Shuijiao:
			case GActions.ZS_Orz:
			case GActions.GS_Orz:
			case GActions.FS_Orz:
			case GActions.MJ_Orz:
				if (this.SpriteChangeAction != null)
				{
					this.SpriteChangeAction(this, new SpriteChangeActionEventArgs
					{
						Action = (int)this._Action
					});
				}
				return;
			}
			if (this.SpriteType == GSpriteTypes.Leader)
			{
				LeaderInfo.InAction = false;
			}
		}

		private int GetLianZhaoDecorationCode()
		{
			if (Global.CalcOriginalOccupationID(this.Occupation) == 0)
			{
				if (this.WeaponState == WeaponStates.K)
				{
					return 1300 + this.AttackNum - 1;
				}
				if (this.WeaponState == WeaponStates.D)
				{
					return 1200 + this.AttackNum - 1;
				}
				if (this.WeaponState == WeaponStates.S)
				{
					return 1400 + this.AttackNum - 1;
				}
				if (this.WeaponState == WeaponStates.C)
				{
					return 1100 + this.AttackNum - 1;
				}
				if (this.WeaponState == WeaponStates.SD)
				{
					return 1500 + this.AttackNum - 1;
				}
			}
			else if (Global.CalcOriginalOccupationID(this.Occupation) == 1)
			{
				if (this.WeaponState == WeaponStates.K)
				{
					return 1700 + this.AttackNum - 1;
				}
				if (this.WeaponState == WeaponStates.D)
				{
					return 1600 + this.AttackNum - 1;
				}
				if (this.WeaponState == WeaponStates.S)
				{
					return 1800 + this.AttackNum - 1;
				}
				if (this.WeaponState != WeaponStates.C)
				{
					if (this.WeaponState == WeaponStates.SD)
					{
					}
				}
			}
			else if (Global.CalcOriginalOccupationID(this.Occupation) == 2)
			{
				if (this.WeaponState == WeaponStates.K)
				{
					return 1900 + this.AttackNum - 1;
				}
				if (this.WeaponState != WeaponStates.D)
				{
					if (this.WeaponState != WeaponStates.S)
					{
						if (this.WeaponState != WeaponStates.C)
						{
							if (this.WeaponState == WeaponStates.G)
							{
								return 2000 + this.AttackNum - 1;
							}
							if (this.WeaponState == WeaponStates.N)
							{
								return 2100 + this.AttackNum - 1;
							}
							if (this.WeaponState == WeaponStates.SD)
							{
							}
						}
					}
				}
			}
			else if (Global.CalcOriginalOccupationID(this.Occupation) == 5)
			{
				if (this.WeaponState == WeaponStates.K)
				{
					return 3101 + this.AttackNum - 1;
				}
				return 3106 + this.AttackNum - 1;
			}
			else
			{
				RoleData rd = null;
				if (this.SpriteType != GSpriteTypes.Leader && Global.Data.OtherRoles.ContainsKey(this.RoleID))
				{
					rd = Global.Data.OtherRoles[this.RoleID];
				}
				if (((this.SpriteType != GSpriteTypes.Leader) ? Global.GetMJSType(rd) : Global.GetMJSType()) == MJSSkillType.Strength_Sword)
				{
					if (this.WeaponState == WeaponStates.K)
					{
						return 2200 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.D)
					{
						return 2400 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.S)
					{
						return 2500 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.C)
					{
						return 2300 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.SD)
					{
						return 2600 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.MJ)
					{
						return 7000 + this.AttackNum - 1;
					}
				}
				else
				{
					if (this.WeaponState == WeaponStates.K)
					{
						return 2700 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.D)
					{
						return 2800 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.S)
					{
						return 2900 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.SD)
					{
						return 7200 + this.AttackNum - 1;
					}
					if (this.WeaponState == WeaponStates.MJ)
					{
						return 7100 + this.AttackNum - 1;
					}
				}
			}
			return -1;
		}

		private int GetLianZhaoMagicCode()
		{
			if (Global.CalcOriginalOccupationID(this.Occupation) == 0)
			{
				if (this.AttackNum == 1)
				{
					return 100;
				}
				if (this.AttackNum == 2)
				{
					return 187;
				}
				if (this.AttackNum == 3)
				{
					return 188;
				}
				if (this.AttackNum == 4)
				{
					return 189;
				}
				if (this.AttackNum == 5)
				{
					return 190;
				}
			}
			else if (Global.CalcOriginalOccupationID(this.Occupation) == 1)
			{
				if (this.AttackNum == 1)
				{
					return 200;
				}
				if (this.AttackNum == 2)
				{
					return 287;
				}
				if (this.AttackNum == 3)
				{
					return 288;
				}
				if (this.AttackNum == 4)
				{
					return 289;
				}
				if (this.AttackNum == 5)
				{
					return 290;
				}
			}
			else if (Global.CalcOriginalOccupationID(this.Occupation) == 2)
			{
				if (this.AttackNum == 1)
				{
					return 300;
				}
				if (this.AttackNum == 2)
				{
					return 388;
				}
				if (this.AttackNum == 3)
				{
					return 389;
				}
				if (this.AttackNum == 4)
				{
					return 390;
				}
				if (this.AttackNum == 5)
				{
					return 391;
				}
			}
			else if (Global.CalcOriginalOccupationID(this.Occupation) == 5)
			{
				if (this.AttackNum == 1)
				{
					return 11000;
				}
				if (this.AttackNum == 2)
				{
					return 11088;
				}
				if (this.AttackNum == 3)
				{
					return 11089;
				}
				if (this.AttackNum == 4)
				{
					return 11090;
				}
				if (this.AttackNum == 5)
				{
					return 11091;
				}
			}
			else
			{
				List<GoodsData> lst = null;
				if (this.SpriteType != GSpriteTypes.Leader && Global.Data.OtherRoles.ContainsKey(this.RoleID))
				{
					lst = Global.Data.OtherRoles[this.RoleID].GoodsDataList;
				}
				if (((this.SpriteType != GSpriteTypes.Leader) ? Global.GetMJSType(lst, this.Occupation, 0) : Global.GetMJSType()) == MJSSkillType.Strength_Sword)
				{
					if (this.AttackNum == 1)
					{
						return 10000;
					}
					if (this.AttackNum == 2)
					{
						return 10088;
					}
					if (this.AttackNum == 3)
					{
						return 10089;
					}
					if (this.AttackNum == 4)
					{
						return 10090;
					}
					if (this.AttackNum == 5)
					{
						return 10091;
					}
				}
				else
				{
					if (this.AttackNum == 1)
					{
						return 10100;
					}
					if (this.AttackNum == 2)
					{
						return 10188;
					}
					if (this.AttackNum == 3)
					{
						return 10189;
					}
					if (this.AttackNum == 4)
					{
						return 10190;
					}
					if (this.AttackNum == 5)
					{
						return 10191;
					}
				}
			}
			return -1;
		}

		public static int TotalObjectCount;

		public static int ShowObjectCount;

		public static int GuidSpriteId;

		private AnimationManager _AnimationMgr;

		private Transform _Trans;

		private Rigidbody _RB;

		public LeaderInfo _LeaderInfo;

		private string _name;

		private bool _InitStatus;

		private GameObject _the3DGameObject;

		private HorseController _HorseController;

		private GameObject mJiaoYinFashion;

		private int _cx;

		private int _cy;

		private GActions _LastAction;

		private Dictionary<string, AnimationClip> mAnimationClips;

		private Animator mAnimatorComponent;

		private MonsterStateController mAnimationController;

		private NpcStateController _NpcStateMgr;

		private GActions _Action;

		private int _Direction;

		private Quaternion _Rotation = Quaternion.identity;

		public long MovingSpeedStartTicks;

		public int MovingSpeedMills;

		private double _MovingSpeed = 1.0;

		private string _VOtherName = string.Empty;

		public string PKKingSpriteName;

		public string LangHunSpriteName;

		private int GuidCurrentId;

		private string _VSName;

		private string _ShowName;

		private bool IsFixedPosition;

		private bool IsFixedDirection;

		private Point FixedPosition;

		private int FixedDirection;

		private Quaternion FixedRotation;

		private int _ExtensionID = -1;

		private bool isFixed;

		private int _VLevel;

		private GSkillMoveType _skillMoveType;

		private long _CurrentMagicTicks;

		public int _CurrentMagic = -1;

		public int deathDelay;

		private SolidColorBrush _SNameBrush;

		private int _flagsType;

		public bool CreateByClient;

		private bool _Started;

		private bool _Visiblity = true;

		private string qiangHua20EffectName1 = "DelayDecoration_15155";

		private string qiangHua20EffectName2 = "DelayDecoration_14000";

		private GameObject chiBangParentObj;

		private List<GoodsData> weaponGoodsDataListChiBang = new List<GoodsData>();

		private GameObject chiBangParentObj2;

		private GameObject roleInfoDisplayObj;

		private RoleResLoader roleResLoader;

		private WingsResLoader wingsResLoader;

		private ShouHuChongResLoader shouHuChongResLoader;

		private WeaponResLoader weaponResLoader;

		private MonsterNPCResLoader monsterNPCResLoader;

		private WingsLingyuResLoader junqiResLoader;

		private Dictionary<string, IObject> ChildrenObjectsDict = new Dictionary<string, IObject>();

		private int RotateSkillTypeTimes;

		private GSprite _Pet;

		private GSprite _BiaoChe;

		private long LastAttackTicks;

		private long _LastActionTicks = Global.GetCorrectLocalTime();

		private int _OldGridX = -1;

		private int _OldGridY = -1;

		private float tempX;

		private float tempY;

		private bool LoadReal3DWeaponOk;

		private bool mLoadReal3DResOK;

		private bool isDefault3DRes;

		private TransformationState mCurrentTraState;

		private byte RoleDisHorse;

		private HeadDisplayText HeadDispText;

		private List<GameObject> WeaponList;

		public List<string> NotSafeEmptyNamesList;

		public List<string> SafeEmptyNamesList;

		private NetAudioSource _NetAudioSource;

		private AudioListener _AudioListener;

		private string _PlaySoundURL;

		private string[] _PlayNpcTalkSoundURLs;

		private long _LastPlaySpriteSoundTicks;

		private long _NpcSoundTicksSlot = -1L;

		private string _NpcPlayingMusicFile = string.Empty;

		public WeaponStates WeaponState;

		public bool IsFlying;

		public bool OnHorse;

		public bool IsInSafeRegion;

		public GameObject Wings;

		public Animator WingsAnimator;

		public Animation WingsAnimation;

		public ShouHuChongController shouHuChongController;

		private Vector3 MeshSize = Vector3.zero;

		private string _DefaultLayerName = "Sprites";

		private bool isDeathInKuaFuTeamCompete = true;

		private int AttackNum = 1;

		private RoleInfoDisplay _RoleInfoDisplay;

		private MonsterInfoDisplay _MonsterInfoDisplay;

		public StallNamePart m_StallNamePart;

		public Vector2 LastJoyPosition = Vector2.zero;

		public int WaitingDirection;

		private float TotalIdleTime;

		public float TotalRunTime;

		private Vector3 monsterTempVec = Vector3.zero;

		private GDecoration mGDecorationEmblem;

		private GDecoration mGDecorationEmblemShan;

		public static Dictionary<string, int> ActiveMagicDecorations = new Dictionary<string, int>();

		public static int TotalMagicDecorations = 0;
	}
}
