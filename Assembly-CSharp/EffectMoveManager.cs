using System;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Scene;
using UnityEngine;

public class EffectMoveManager : MonoBehaviour
{
	private void Awake()
	{
	}

	public void SetTarget(GameObject obj)
	{
		this.TargetObj = obj;
	}

	private void Update()
	{
		if (this.TargetObj != null)
		{
			try
			{
				this.endPos = this.TargetObj.transform.position + new Vector3(0f, 1.2f, 0f);
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
				this.TargetObj = null;
				if (this.ObjMgr != null)
				{
					GScene.Remove(this.ObjMgr);
					this.ObjMgr.Destroy();
				}
				return;
			}
			this.theTime += Time.deltaTime;
			if (this.theTime >= 2f)
			{
				base.transform.position = this.endPos;
			}
			else
			{
				this.detalVal += 0.4f;
				this.lerpVal = Time.deltaTime * this.detalVal;
			}
			base.transform.position = Vector3.Lerp(base.transform.position, this.endPos, this.lerpVal);
			if (base.transform.position == this.endPos)
			{
				this.TargetObj = null;
				if (this.ObjMgr != null)
				{
					GScene.Remove(this.ObjMgr);
					this.ObjMgr.Destroy();
				}
			}
			return;
		}
		this.TargetObj = null;
		if (this.ObjMgr != null)
		{
			GScene.Remove(this.ObjMgr);
			this.ObjMgr.Destroy();
		}
	}

	private GameObject TargetObj;

	public GDecoration ObjMgr;

	private float detalVal;

	private Vector3 endPos = default(Vector3);

	private float theTime;

	private float lerpVal;
}
