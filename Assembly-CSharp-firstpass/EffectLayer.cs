using System;
using System.Collections.Generic;
using UnityEngine;
using Xft;

public class EffectLayer : MonoBehaviour
{
	public Plane CollisionPlane
	{
		get
		{
			return this.mCollisionPlane;
		}
	}

	public void FindMyCamera()
	{
		int num = 1 << base.gameObject.layer;
		Camera[] array = Object.FindSceneObjectsOfType(typeof(Camera)) as Camera[];
		int i = 0;
		int num2 = array.Length;
		while (i < num2)
		{
			Camera camera = array[i];
			if ((camera.cullingMask & num) != 0)
			{
				this.MyCamera = camera;
				return;
			}
			i++;
		}
	}

	protected void InitCollision()
	{
		if (!this.UseCollisionDetection)
		{
			return;
		}
		this.mCollisionPlane = new Plane(this.PlaneDir.normalized, base.transform.position + this.PlaneOffset);
		if (this.CollisionType == COLLITION_TYPE.CollisionLayer || this.CollisionType == COLLITION_TYPE.Plane)
		{
			return;
		}
		if (this.CollisionGoal == null)
		{
			Debug.LogWarning("please set the collision goal!");
		}
	}

	protected List<Affector> InitAffectors(EffectNode node)
	{
		List<Affector> list = new List<Affector>();
		if (this.UVAffectorEnable)
		{
			UVAnimation uvanimation = new UVAnimation();
			if (this.UVType == 1)
			{
				float num = this.OriUVDimensions.x / (float)this.Cols;
				float num2 = Mathf.Abs(this.OriUVDimensions.y / (float)this.Rows);
				Vector2 cellSize;
				cellSize..ctor(num, num2);
				uvanimation.BuildUVAnim(this.OriTopLeftUV, cellSize, this.Cols, this.Rows, this.Cols * this.Rows);
			}
			this.UVDimension = uvanimation.UVDimensions[0];
			this.UVTopLeft = uvanimation.frames[0];
			if (uvanimation.frames.Length != 1)
			{
				uvanimation.loopCycles = this.LoopCircles;
				Affector affector = new UVAffector(uvanimation, this.UVTime, node, this.RandomStartFrame);
				list.Add(affector);
			}
		}
		else
		{
			this.UVDimension = this.OriUVDimensions;
			this.UVTopLeft = this.OriTopLeftUV;
		}
		if (this.RotAffectorEnable && this.RotateType != RSTYPE.NONE)
		{
			Affector affector2;
			if (this.RotateType == RSTYPE.NONE)
			{
				affector2 = new RotateAffector(this.DeltaRot, node);
			}
			else
			{
				affector2 = new RotateAffector(this.RotateType, node);
			}
			list.Add(affector2);
		}
		if (this.ScaleAffectorEnable && this.ScaleType != RSTYPE.NONE)
		{
			Affector affector3;
			if (this.ScaleType == RSTYPE.NONE)
			{
				affector3 = new ScaleAffector(this.DeltaScaleX, this.DeltaScaleY, node);
			}
			else
			{
				affector3 = new ScaleAffector(this.ScaleType, node);
			}
			list.Add(affector3);
		}
		if (this.ColorAffectorEnable)
		{
			ColorAffector colorAffector = new ColorAffector(this, node);
			list.Add(colorAffector);
		}
		if (this.JetAffectorEnable)
		{
			Affector affector4 = new JetAffector(this.JetMag, this.JetMagType, this.JetCurve, node);
			list.Add(affector4);
		}
		if (this.VortexAffectorEnable)
		{
			Affector affector5 = new VortexAffector(this.VortexObj, this.VortexMagType, this.VortexMag, this.VortexCurve, this.VortexDirection, this.VortexInheritRotation, node);
			list.Add(affector5);
		}
		if (this.UVRotAffectorEnable)
		{
			float rotXSpeed = this.UVRotXSpeed;
			float rotYSpeed = this.UVRotYSpeed;
			if (this.RandomUVRotateSpeed)
			{
				rotXSpeed = Random.Range(this.UVRotXSpeed, this.UVRotXSpeedMax);
				rotYSpeed = Random.Range(this.UVRotYSpeed, this.UVRotYSpeedMax);
			}
			Affector affector6 = new UVRotAffector(rotXSpeed, rotYSpeed, node);
			list.Add(affector6);
		}
		if (this.GravityAffectorEnable)
		{
			Affector affector7 = new GravityAffector(this.GravityObject, this.GravityAftType, this.GravityMagType, this.IsGravityAccelerate, this.GravityDirection, this.GravityMag, this.GravityCurve, node);
			list.Add(affector7);
			if (this.GravityAftType == GAFTTYPE.Spherical && this.GravityObject == null)
			{
				Debug.LogWarning("Gravity Object is missing, automatically set to effect layer self:" + base.gameObject.name);
				this.GravityObject = base.transform;
			}
		}
		if (this.AirAffectorEnable)
		{
			Affector affector8 = new AirFieldAffector(this.AirObject, this.AirDirection, this.AirMagType, this.AirMagnitude, this.AirMagCurve, this.AirAttenuation, this.AirUseMaxDistance, this.AirMaxDistance, this.AirEnableSpread, this.AirSpread, this.AirInheritVelocity, this.AirInheritRotation, node);
			list.Add(affector8);
		}
		if (this.BombAffectorEnable)
		{
			Affector affector9 = new BombAffector(this.BombObject, this.BombType, this.BombMagType, this.BombDecayType, this.BombMagnitude, this.BombMagCurve, this.BombDecay, this.BombAxis, node);
			list.Add(affector9);
		}
		if (this.TurbulenceAffectorEnable)
		{
			Affector affector10 = new TurbulenceFieldAffector(this.TurbulenceObject, this.TurbulenceMagType, this.TurbulenceMagnitude, this.TurbulenceMagCurve, this.TurbulenceAttenuation, this.TurbulenceUseMaxDistance, this.TurbulenceMaxDistance, node);
			list.Add(affector10);
		}
		if (this.DragAffectorEnable)
		{
			Affector affector11 = new DragAffector(this.DragObj, this.DragUseDir, this.DragDir, this.DragMag, this.DragUseMaxDist, this.DragMaxDist, this.DragAtten, node);
			list.Add(affector11);
		}
		return list;
	}

	public void SetClient(Transform client)
	{
		this.ClientTransform = client;
		for (int i = 0; i < this.MaxENodes; i++)
		{
			EffectNode effectNode = this.ActiveENodes[i];
			if (effectNode == null)
			{
				effectNode = this.AvailableENodes[i];
			}
			effectNode.ClientTrans = client;
		}
	}

	protected void Init()
	{
		this.InitCollision();
		this.Owner = base.transform.parent.gameObject.GetComponent<XffectComponent>();
		if (this.Owner == null)
		{
			Debug.LogError("you must set EffectLayer to be XffectComponent's child.");
		}
		if (this.ClientTransform == null)
		{
			Debug.LogWarning("effect layer: " + base.gameObject.name + " haven't assign a client transform, automaticly set to itself.");
			this.ClientTransform = base.transform;
		}
		this.AvailableENodes = new EffectNode[this.MaxENodes];
		this.ActiveENodes = new EffectNode[this.MaxENodes];
		for (int i = 0; i < this.MaxENodes; i++)
		{
			EffectNode effectNode = new EffectNode(i, this.ClientTransform, this.SyncClient, this);
			List<Affector> affectorList = this.InitAffectors(effectNode);
			effectNode.SetAffectorList(affectorList);
			if (this.RenderType == 0)
			{
				effectNode.SetType(this.SpriteWidth, this.SpriteHeight, (STYPE)this.SpriteType, (ORIPOINT)this.OriPoint, this.SpriteUVStretch, 60f);
			}
			else if (this.RenderType == 1)
			{
				float width = this.RibbonWidth;
				float len = this.RibbonLen;
				if (this.UseRandomRibbon)
				{
					width = Random.Range(this.RibbonWidthMin, this.RibbonWidthMax);
					len = Random.Range(this.RibbonLenMin, this.RibbonLenMax);
				}
				effectNode.SetType(this.FaceToObject, this.FaceObject, width, this.MaxRibbonElements, len, this.ClientTransform.position + this.EmitPoint, this.StretchType, 60f);
			}
			else if (this.RenderType == 2)
			{
				effectNode.SetType(this.ConeSize, this.ConeSegment, this.ConeAngle, base.transform.rotation * this.OriVelocityAxis, 0, 60f, this.UseConeAngleChange, this.ConeDeltaAngle);
			}
			else if (this.RenderType == 3)
			{
				Vector3 dir = Vector3.zero;
				if (this.OriVelocityAxis == Vector3.zero)
				{
					this.OriVelocityAxis = Vector3.up;
				}
				dir = base.transform.rotation * this.OriVelocityAxis;
				effectNode.SetType(this.CMesh, dir, 60f);
			}
			this.AvailableENodes[i] = effectNode;
		}
		this.AvailableNodeCount = this.MaxENodes;
		this.emitter = new Emitter(this);
		this.mStopped = false;
	}

	public VertexPool GetVertexPool()
	{
		return this.Vertexpool;
	}

	public int GetActiveNodeCount()
	{
		return this.ActiveENodes.Length;
	}

	public void RemoveActiveNode(EffectNode node)
	{
		if (this.AvailableNodeCount == this.MaxENodes)
		{
			Debug.LogError("out index!");
		}
		if (this.ActiveENodes[node.Index] == null)
		{
			return;
		}
		this.ActiveENodes[node.Index] = null;
		this.AvailableENodes[node.Index] = node;
		this.AvailableNodeCount++;
	}

	public void AddActiveNode(EffectNode node)
	{
		if (this.AvailableNodeCount == 0)
		{
			Debug.LogError("out index!");
		}
		if (this.AvailableENodes[node.Index] == null)
		{
			return;
		}
		this.ActiveENodes[node.Index] = node;
		this.AvailableENodes[node.Index] = null;
		this.AvailableNodeCount--;
	}

	protected void AddNodes(int num)
	{
		int num2 = 0;
		for (int i = 0; i < this.MaxENodes; i++)
		{
			if (num2 == num)
			{
				break;
			}
			EffectNode effectNode = this.AvailableENodes[i];
			if (effectNode != null)
			{
				this.AddActiveNode(effectNode);
				num2++;
				if (this.UseSubEmitters && !string.IsNullOrEmpty(this.BirthSubEmitter))
				{
					XffectComponent effect = this.SpawnCache.GetEffect(this.BirthSubEmitter);
					if (effect == null)
					{
						return;
					}
					effectNode.SubEmitter = effect;
					effect.Active();
				}
				this.emitter.SetEmitPosition(effectNode);
				float life;
				if (this.IsNodeLifeLoop)
				{
					life = -1f;
				}
				else
				{
					life = Random.Range(this.NodeLifeMin, this.NodeLifeMax);
				}
				Vector3 emitRotation = this.emitter.GetEmitRotation(effectNode);
				float speed = this.OriSpeed;
				if (this.IsRandomSpeed)
				{
					speed = Random.Range(this.SpeedMin, this.SpeedMax);
				}
				effectNode.Init(emitRotation.normalized, speed, life, Random.Range(this.OriRotationMin, this.OriRotationMax), Random.Range(this.OriScaleXMin, this.OriScaleXMax), Random.Range(this.OriScaleYMin, this.OriScaleYMax), this.Color1, this.UVTopLeft, this.UVDimension);
			}
		}
	}

	public void ResetCamera(Camera cam)
	{
		if (this.ActiveENodes == null)
		{
			return;
		}
		for (int i = 0; i < this.MaxENodes; i++)
		{
			EffectNode effectNode = this.ActiveENodes[i];
			if (effectNode != null)
			{
				effectNode.ResetCamera(cam);
			}
			else
			{
				effectNode = this.AvailableENodes[i];
				effectNode.ResetCamera(cam);
			}
		}
	}

	public void Reset()
	{
		if (this.ActiveENodes == null)
		{
			return;
		}
		for (int i = 0; i < this.MaxENodes; i++)
		{
			EffectNode effectNode = this.ActiveENodes[i];
			if (effectNode != null)
			{
				effectNode.Reset();
				this.RemoveActiveNode(effectNode);
			}
		}
		this.emitter.Reset();
		this.mStopped = false;
	}

	public void FixedUpdateCustom(float deltaTime)
	{
		if (this.MyCamera == null)
		{
			this.FindMyCamera();
			this.ResetCamera(this.MyCamera);
		}
		int nodes = this.emitter.GetNodes(deltaTime);
		this.AddNodes(nodes);
		for (int i = 0; i < this.MaxENodes; i++)
		{
			EffectNode effectNode = this.ActiveENodes[i];
			if (effectNode != null)
			{
				effectNode.Update(deltaTime);
			}
		}
	}

	public void StartCustom()
	{
		this.FindMyCamera();
		this.Init();
		this.LastClientPos = this.ClientTransform.position;
	}

	private void OnDrawGizmos()
	{
		if (this.ClientTransform == null)
		{
			return;
		}
		Gizmos.color = this.DebugColor;
		float num;
		if (this.RenderType == 0)
		{
			num = (this.SpriteWidth + this.SpriteHeight) / 6f;
		}
		else if (this.RenderType == 1)
		{
			num = this.RibbonWidth / 3f;
		}
		else
		{
			num = (this.ConeSize.x + this.ConeSize.y) / 6f;
		}
		num = Mathf.Clamp(num, 0f, 1f);
		if (this.EmitType == 0 || this.EmitType == 3)
		{
			Gizmos.DrawWireSphere(this.ClientTransform.position + this.EmitPoint, num);
		}
		if (this.EmitType == 1)
		{
			Gizmos.DrawWireCube(this.ClientTransform.position + this.EmitPoint, this.BoxSize);
		}
		else if (this.EmitType == 4)
		{
			Vector3 vector = this.ClientTransform.position + this.EmitPoint + this.ClientTransform.forward * this.LineLengthLeft;
			Vector3 vector2 = this.ClientTransform.position + this.EmitPoint + this.ClientTransform.forward * this.LineLengthRight;
			Gizmos.DrawLine(vector, vector2);
		}
		else if (this.EmitType == 5)
		{
		}
		if (this.OriVelocityAxis != Vector3.zero)
		{
			Gizmos.DrawLine(this.ClientTransform.position + this.EmitPoint, this.ClientTransform.position + this.EmitPoint + this.ClientTransform.rotation * this.OriVelocityAxis * num * 15f);
		}
		if (this.UseCollisionDetection && this.CollisionType == COLLITION_TYPE.Plane)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(this.ClientTransform.position + this.PlaneOffset, new Vector3(num * 50f, 0f, num * 50f));
			Gizmos.color = Color.white;
		}
	}

	public bool EmitOver(float deltaTime)
	{
		if (this.ActiveENodes == null)
		{
			return false;
		}
		if (this.AvailableNodeCount == this.MaxENodes)
		{
			if (this.EmitWay == EEmitWay.ByRate)
			{
				if (this.emitter.EmitLoop == 0f)
				{
					return true;
				}
			}
			else if (this.EmitWay == EEmitWay.ByCurve)
			{
				if (this.emitter.CurveEmitDone)
				{
					return true;
				}
			}
			else if (this.EmitWay == EEmitWay.ByDistance && this.mStopped)
			{
				return true;
			}
		}
		return false;
	}

	public void StopEmit()
	{
		this.mStopped = true;
		if (this.IsNodeLifeLoop)
		{
			for (int i = 0; i < this.MaxENodes; i++)
			{
				EffectNode effectNode = this.ActiveENodes[i];
				if (effectNode != null)
				{
					effectNode.Stop();
				}
			}
		}
		this.emitter.StopEmit();
	}

	public void SetCollisionGoalPos(Transform pos)
	{
		if (!this.UseCollisionDetection)
		{
			Debug.LogWarning(base.gameObject.name + "is not set to collision detect mode, please check it");
			return;
		}
		this.CollisionGoal = pos;
	}

	public void SetArractionAffectorGoal(Transform goal)
	{
		if (!this.GravityAffectorEnable || this.GravityAftType == GAFTTYPE.Planar)
		{
			Debug.LogWarning(base.gameObject.name + "has no attraction affector, please check it");
			return;
		}
		foreach (EffectNode effectNode in this.AvailableENodes)
		{
			List<Affector> affectorList = effectNode.GetAffectorList();
			foreach (Affector affector in affectorList)
			{
				if (affector.Type == AFFECTORTYPE.GravityAffector)
				{
					GravityAffector gravityAffector = (GravityAffector)affector;
					gravityAffector.SetAttraction(goal);
				}
			}
		}
	}

	public void SetScale(Vector2 scale)
	{
		for (int i = 0; i < this.MaxENodes; i++)
		{
			EffectNode effectNode = this.ActiveENodes[i];
			if (effectNode == null)
			{
				effectNode = this.AvailableENodes[i];
			}
			effectNode.Scale = scale;
		}
	}

	public EffectNode EmitByPos(Vector3 pos)
	{
		int num = 0;
		EffectNode result = null;
		for (int i = 0; i < this.MaxENodes; i++)
		{
			if (num == 1)
			{
				break;
			}
			EffectNode effectNode = this.AvailableENodes[i];
			if (effectNode != null)
			{
				this.AddActiveNode(effectNode);
				num++;
				effectNode.SetLocalPosition(pos);
				float life;
				if (this.IsNodeLifeLoop)
				{
					life = -1f;
				}
				else
				{
					life = Random.Range(this.NodeLifeMin, this.NodeLifeMax);
				}
				effectNode.Init(this.emitter.GetEmitRotation(effectNode).normalized, this.OriSpeed, life, Random.Range(this.OriRotationMin, this.OriRotationMax), Random.Range(this.OriScaleXMin, this.OriScaleXMax), Random.Range(this.OriScaleYMin, this.OriScaleYMax), this.Color1, this.UVTopLeft, this.UVDimension);
				result = effectNode;
			}
		}
		return result;
	}

	public int VersionIdentifier1 = 222;

	public VertexPool Vertexpool;

	public Transform ClientTransform;

	public bool SyncClient;

	public Material Material;

	public int RenderType;

	public float StartTime;

	public float MaxFps = 60f;

	public Color DebugColor = Color.white;

	public int Depth;

	public int SpriteType;

	public int OriPoint;

	public float SpriteWidth = 1f;

	public float SpriteHeight = 1f;

	public int SpriteUVStretch;

	public bool RandomOriScale;

	public bool RandomOriRot;

	public int OriRotationMin;

	public int OriRotationMax;

	public bool RotAffectorEnable;

	public RSTYPE RotateType;

	public float DeltaRot;

	public AnimationCurve RotateCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 360f)
	});

	public WRAP_TYPE RotateCurveWrap;

	public float RotateCurveTime = 1f;

	public float RotateCurveMaxValue = 1f;

	public AnimationCurve RotateCurve01 = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public float RotateSpeedMin;

	public float RotateSpeedMax;

	public float OriScaleXMin = 1f;

	public float OriScaleXMax = 1f;

	public float OriScaleYMin = 1f;

	public float OriScaleYMax = 1f;

	public bool ScaleAffectorEnable;

	public RSTYPE ScaleType;

	public float DeltaScaleX;

	public float DeltaScaleY;

	public AnimationCurve ScaleXCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 5f)
	});

	public AnimationCurve ScaleYCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 5f)
	});

	public WRAP_TYPE ScaleWrapMode;

	public float ScaleCurveTime = 1f;

	public float MaxScaleCalue = 1f;

	public AnimationCurve ScaleXCurveNew = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public AnimationCurve ScaleYCurveNew = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public bool UseSameScaleCurve;

	public float DeltaScaleXMax;

	public float DeltaScaleYMax;

	public bool ColorAffectorEnable;

	public int ColorAffectType;

	public float ColorGradualTimeLength = 1f;

	public COLOR_GRADUAL_TYPE ColorGradualType;

	public Color Color1 = Color.white;

	public Color Color2;

	public Color Color3;

	public Color Color4;

	public Color Color5;

	public COLOR_CHANGE_TYPE ColorChangeType;

	public ColorParameter ColorParam;

	public AnimationCurve ColorGradualCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 1f)
	});

	public float RibbonWidth = 1f;

	public int MaxRibbonElements = 8;

	public float RibbonLen = 15f;

	public float TailDistance;

	public int StretchType;

	public bool FaceToObject;

	public Transform FaceObject;

	public bool UseRandomRibbon;

	public float RibbonWidthMin = 1f;

	public float RibbonWidthMax = 1f;

	public float RibbonLenMin = 15f;

	public float RibbonLenMax = 15f;

	public Vector2 ConeSize = Vector2.one;

	public float ConeAngle;

	public int ConeSegment = 4;

	public bool UseConeAngleChange;

	public AnimationCurve ConeDeltaAngle = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 60f)
	});

	public Mesh CMesh;

	public int EmitType;

	public Vector3 BoxSize;

	public Vector3 EmitPoint;

	public float Radius;

	public bool UseRandomCircle;

	public float CircleRadiusMin = 1f;

	public float CircleRadiusMax = 10f;

	public Vector3 CircleDir = Vector3.up;

	public bool EmitUniform;

	public float LineLengthLeft = -1f;

	public float LineLengthRight = 1f;

	public int MaxENodes = 1;

	public bool IsNodeLifeLoop = true;

	public float NodeLifeMin = 1f;

	public float NodeLifeMax = 1f;

	public EEmitWay EmitWay;

	public AnimationCurve EmitterCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(1f, 10f)
	});

	public float DiffDistance = 0.1f;

	public Mesh EmitMesh;

	public int EmitMeshType;

	public bool IsBurstEmit;

	public float ChanceToEmit = 100f;

	public float EmitDuration = 100f;

	public float EmitRate = 20f;

	public int EmitLoop = -1;

	public float EmitDelay;

	public DIRECTION_TYPE DirType;

	public Transform DirCenter;

	public Vector3 OriVelocityAxis;

	public int AngleAroundAxis;

	public bool UseRandomDirAngle;

	public int AngleAroundAxisMax;

	public float OriSpeed;

	public bool AlwaysSyncRotation;

	public bool IsRandomSpeed;

	public float SpeedMin;

	public float SpeedMax;

	public bool JetAffectorEnable;

	public MAGTYPE JetMagType;

	public float JetMag;

	public AnimationCurve JetCurve;

	public bool VortexAffectorEnable;

	public MAGTYPE VortexMagType;

	public float VortexMag = 1f;

	public AnimationCurve VortexCurve;

	public Vector3 VortexDirection = Vector3.up;

	public bool VortexInheritRotation = true;

	public Transform VortexObj;

	public bool IsFixedCircle;

	public bool IsRandomVortexDir;

	public bool IsVortexAccelerate;

	public float VortexAttenuation;

	public bool UseVortexMaxDistance;

	public float VortexMaxDistance;

	public bool UVRotAffectorEnable;

	public bool RandomUVRotateSpeed;

	public float UVRotXSpeed;

	public float UVRotYSpeed;

	public float UVRotXSpeedMax;

	public float UVRotYSpeedMax;

	public bool GravityAffectorEnable;

	public GAFTTYPE GravityAftType;

	public MAGTYPE GravityMagType;

	public float GravityMag;

	public AnimationCurve GravityCurve;

	public Vector3 GravityDirection = Vector3.up;

	public Transform GravityObject;

	public bool IsGravityAccelerate = true;

	public bool AirAffectorEnable;

	public Transform AirObject;

	public MAGTYPE AirMagType;

	public float AirMagnitude;

	public AnimationCurve AirMagCurve;

	public Vector3 AirDirection;

	public float AirAttenuation;

	public bool AirUseMaxDistance;

	public float AirMaxDistance;

	public bool AirEnableSpread;

	public float AirSpread;

	public float AirInheritVelocity;

	public bool AirInheritRotation;

	public bool BombAffectorEnable;

	public Transform BombObject;

	public BOMBTYPE BombType = BOMBTYPE.Spherical;

	public BOMBDECAYTYPE BombDecayType;

	public MAGTYPE BombMagType;

	public float BombMagnitude;

	public AnimationCurve BombMagCurve;

	public Vector3 BombAxis;

	public float BombDecay;

	public bool TurbulenceAffectorEnable;

	public Transform TurbulenceObject;

	public MAGTYPE TurbulenceMagType;

	public float TurbulenceMagnitude;

	public AnimationCurve TurbulenceMagCurve;

	public float TurbulenceAttenuation;

	public bool TurbulenceUseMaxDistance;

	public float TurbulenceMaxDistance;

	public bool DragAffectorEnable;

	public Transform DragObj;

	public bool DragUseDir;

	public Vector3 DragDir = Vector3.up;

	public float DragMag = 10f;

	public bool DragUseMaxDist;

	public float DragMaxDist = 50f;

	public float DragAtten;

	public bool UVAffectorEnable;

	public int UVType;

	public Vector2 OriTopLeftUV = Vector2.zero;

	public Vector2 OriUVDimensions = Vector2.one;

	protected Vector2 UVTopLeft;

	protected Vector2 UVDimension;

	public int Cols = 1;

	public int Rows = 1;

	public int LoopCircles = -1;

	public float UVTime = 1f;

	public string EanPath = "none";

	public int EanIndex;

	public bool RandomStartFrame;

	public bool UseCollisionDetection;

	public float ParticleRadius = 1f;

	public COLLITION_TYPE CollisionType;

	public bool CollisionAutoDestroy = true;

	public Transform EventReceiver;

	public string EventHandleFunctionName = " ";

	public Transform CollisionGoal;

	public float ColliisionPosRange;

	public LayerMask CollisionLayer;

	public float CollisionOffset;

	public Vector3 PlaneDir = Vector3.up;

	public Vector3 PlaneOffset = Vector3.zero;

	protected Plane mCollisionPlane;

	public bool UseSubEmitters;

	public XffectCache SpawnCache;

	public string BirthSubEmitter;

	public string CollisionSubEmitter;

	public string DeathSubEmitter;

	public Emitter emitter;

	public EffectNode[] AvailableENodes;

	public EffectNode[] ActiveENodes;

	public int AvailableNodeCount;

	public Vector3 LastClientPos;

	public Camera MyCamera;

	public XffectComponent Owner;

	public bool mStopped;
}
