using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xft;

[AddComponentMenu("Xffect/XffectComponent"), ExecuteInEditMode]
public class XffectComponent : MonoBehaviour
{
	private void Awake()
	{
		if (!this.Initialized)
		{
			this.Initialize();
		}
	}

	public void Initialize()
	{
		if (this.EflList.Count > 0)
		{
			return;
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			EffectLayer effectLayer = (EffectLayer)transform.GetComponent(typeof(EffectLayer));
			if (!(effectLayer == null))
			{
				if (effectLayer.Material == null)
				{
					Debug.LogWarning("effect layer: " + effectLayer.gameObject.name + " has no material, please assign a material first!");
				}
				else
				{
					Material material = effectLayer.Material;
					material.renderQueue = material.shader.renderQueue;
					material.renderQueue += effectLayer.Depth;
					this.EflList.Add(effectLayer);
					Transform transform2 = base.transform.Find("xftmesh " + material.name);
					if (transform2 != null)
					{
						MeshFilter meshFilter = (MeshFilter)transform2.GetComponent(typeof(MeshFilter));
						MeshRenderer meshRenderer = (MeshRenderer)transform2.GetComponent(typeof(MeshRenderer));
						meshFilter.sharedMesh = new Mesh();
						meshFilter.sharedMesh.Clear();
						this.MatDic[material.name] = new VertexPool(meshFilter.sharedMesh, material);
						if (!this.MeshList.Contains(transform2.gameObject))
						{
							this.MeshList.Add(transform2.gameObject);
						}
					}
					if (!this.MatDic.ContainsKey(material.name))
					{
						GameObject gameObject = new GameObject("xftmesh " + material.name);
						gameObject.layer = base.gameObject.layer;
						this.MeshList.Add(gameObject);
						gameObject.AddComponent<MeshFilter>();
						gameObject.AddComponent<MeshRenderer>();
						XffectComponent.SetActive(gameObject, XffectComponent.IsActive(base.gameObject));
						MeshFilter meshFilter = (MeshFilter)gameObject.GetComponent(typeof(MeshFilter));
						MeshRenderer meshRenderer = (MeshRenderer)gameObject.GetComponent(typeof(MeshRenderer));
						meshRenderer.castShadows = false;
						meshRenderer.receiveShadows = false;
						meshRenderer.GetComponent<Renderer>().sharedMaterial = material;
						meshFilter.sharedMesh = new Mesh();
						this.MatDic[material.name] = new VertexPool(meshFilter.sharedMesh, material);
					}
				}
			}
		}
		foreach (object obj2 in this.MeshList)
		{
			GameObject gameObject2 = (GameObject)obj2;
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.position = Vector3.zero;
			gameObject2.transform.rotation = Quaternion.identity;
			Vector3 zero = Vector3.zero;
			zero.x = 1f / gameObject2.transform.parent.lossyScale.x;
			zero.y = 1f / gameObject2.transform.parent.lossyScale.y;
			zero.z = 1f / gameObject2.transform.parent.lossyScale.z;
			gameObject2.transform.localScale = zero * this.Scale;
		}
		foreach (EffectLayer effectLayer2 in this.EflList)
		{
			effectLayer2.Vertexpool = this.MatDic[effectLayer2.Material.name];
		}
		base.transform.localScale = Vector3.one;
		foreach (EffectLayer effectLayer3 in this.EflList)
		{
			effectLayer3.StartCustom();
		}
		this.EventList.Clear();
		foreach (object obj3 in base.transform)
		{
			Transform transform3 = (Transform)obj3;
			XftEventComponent component = transform3.GetComponent<XftEventComponent>();
			if (!(component == null))
			{
				this.EventList.Add(component);
				component.Initialize();
			}
		}
		this.Initialized = true;
	}

	private void Start()
	{
		this.LastTime = (double)Time.realtimeSinceStartup;
	}

	public void Update()
	{
		this.CurTime = (double)Time.realtimeSinceStartup;
		float num = (float)(this.CurTime - this.LastTime);
		if (num > 0.05f)
		{
			num = 0.0333f;
		}
		if (!this.IgnoreTimeScale)
		{
			num *= Time.timeScale;
		}
		this.ElapsedTime += num;
		for (int i = 0; i < this.EflList.Count; i++)
		{
			if (this.EflList[i] == null)
			{
				return;
			}
			EffectLayer effectLayer = this.EflList[i];
			if (this.ElapsedTime > effectLayer.StartTime && XffectComponent.IsActive(effectLayer.gameObject))
			{
				effectLayer.FixedUpdateCustom(num);
			}
		}
		for (int j = 0; j < this.EventList.Count; j++)
		{
			XftEventComponent xftEventComponent = this.EventList[j];
			if (XffectComponent.IsActive(xftEventComponent.gameObject))
			{
				xftEventComponent.UpdateCustom(num);
			}
		}
		this.LastTime = this.CurTime;
	}

	public void ResetEditorEvents()
	{
		foreach (XftEventComponent xftEventComponent in this.EventList)
		{
			xftEventComponent.ResetCustom();
		}
	}

	private void DoFinish()
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.Reset();
		}
		this.DeActive();
		this.ElapsedTime = 0f;
	}

	private void LateUpdate()
	{
		foreach (object obj in this.MeshList)
		{
			GameObject gameObject = (GameObject)obj;
			if (gameObject != null)
			{
				gameObject.transform.position = Vector3.zero;
				gameObject.transform.rotation = Quaternion.identity;
			}
		}
		foreach (KeyValuePair<string, VertexPool> keyValuePair in this.MatDic)
		{
			keyValuePair.Value.LateUpdate();
		}
		if (this.ElapsedTime > this.LifeTime && this.LifeTime >= 0f)
		{
			this.DoFinish();
		}
		else if (this.LifeTime < 0f && this.EflList.Count > 0)
		{
			float deltaTime = (float)(this.CurTime - this.LastTime);
			bool flag = true;
			foreach (EffectLayer effectLayer in this.EflList)
			{
				if (!effectLayer.EmitOver(deltaTime))
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.DoFinish();
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
	}

	public void ResetCamera(Camera cam)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.ResetCamera(cam);
		}
	}

	public void SetClient(Transform client)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.SetClient(client);
		}
	}

	public void SetDirectionAxis(Vector3 axis)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.OriVelocityAxis = axis;
		}
	}

	public void SetEmitPosition(Vector3 pos)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.EmitPoint = pos;
		}
	}

	public void SetCollisionGoalPos(Transform pos)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			if (effectLayer.UseCollisionDetection)
			{
				effectLayer.SetCollisionGoalPos(pos);
			}
		}
	}

	public void SetCollisionGoalPos(Transform pos, string eflName)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			if (effectLayer.gameObject.name == eflName && effectLayer.UseCollisionDetection)
			{
				effectLayer.SetCollisionGoalPos(pos);
			}
		}
	}

	public void SetArractionAffectorGoal(Transform goal)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			if (effectLayer.GravityAffectorEnable)
			{
				effectLayer.SetArractionAffectorGoal(goal);
			}
		}
	}

	public void SetArractionAffectorGoal(Transform goal, string eflName)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			if (effectLayer.gameObject.name == eflName && effectLayer.GravityAffectorEnable)
			{
				effectLayer.SetArractionAffectorGoal(goal);
			}
		}
	}

	public void SetScale(Vector2 scale, string eflName)
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			if (effectLayer.gameObject.name == eflName)
			{
				effectLayer.SetScale(scale);
			}
		}
	}

	public EffectNode EmitByPos(Vector3 pos)
	{
		if (this.EflList.Count > 1)
		{
			Debug.LogWarning("EmitByPos only support one effect layer!");
		}
		EffectLayer effectLayer = this.EflList[0];
		return effectLayer.EmitByPos(pos);
	}

	public void Reset()
	{
		this.ElapsedTime = 0f;
		this.Start();
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.Reset();
		}
		foreach (XftEventComponent xftEventComponent in this.EventList)
		{
			xftEventComponent.ResetCustom();
		}
	}

	public void StopEmit()
	{
		foreach (EffectLayer effectLayer in this.EflList)
		{
			effectLayer.StopEmit();
		}
	}

	public void DeActive()
	{
		foreach (XftEventComponent xftEventComponent in this.EventList)
		{
			xftEventComponent.ResetCustom();
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			XffectComponent.SetActive(transform.gameObject, false);
		}
		XffectComponent.SetActive(base.gameObject, false);
	}

	public void ActiveNoInterrupt()
	{
		if (XffectComponent.IsActive(base.gameObject))
		{
			return;
		}
		this.Active();
	}

	public void Active()
	{
		if (!this.Initialized)
		{
			this.Initialize();
		}
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			XffectComponent.SetActive(transform.gameObject, true);
		}
		XffectComponent.SetActive(base.gameObject, true);
		this.Reset();
	}

	public static bool IsActive(GameObject obj)
	{
		return obj.active;
	}

	public static void SetActive(GameObject obj, bool flag)
	{
		obj.active = flag;
	}

	private Dictionary<string, VertexPool> MatDic = new Dictionary<string, VertexPool>();

	private List<EffectLayer> EflList = new List<EffectLayer>();

	private List<XftEventComponent> EventList = new List<XftEventComponent>();

	public float LifeTime = -1f;

	public bool IgnoreTimeScale;

	protected float ElapsedTime;

	protected bool Initialized;

	protected ArrayList MeshList = new ArrayList();

	protected double LastTime;

	protected double CurTime;

	public bool EditView;

	public float Scale = 1f;
}
