using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class Modal3DShow : MonoBehaviour
{
	public bool Show
	{
		set
		{
			if (null != this._Target)
			{
				this._Target.SetActive(value);
			}
		}
	}

	public bool IsTarget(int targetID)
	{
		return !(null == this._Target) && targetID == this.MonsterID;
	}

	public void Add(GameObject go, bool takeOldTransform)
	{
		this.Clear();
		U3DUtils.AddChild(base.gameObject, go, takeOldTransform);
		this._Target = go;
	}

	public void Clear()
	{
		if (null != this._Target)
		{
			for (int i = 0; i < this.ChildGameObjectList.Count; i++)
			{
				GameObject gameObject = this.ChildGameObjectList[i];
				if (null != gameObject)
				{
					Object.Destroy(gameObject);
				}
			}
			this.ChildGameObjectList.Clear();
			Object.DestroyImmediate(this._Target);
			this._Target = null;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (null != this._Target && this.CanRotate)
		{
			this._Target.transform.Rotate((delta.x <= 0f) ? Vector3.up : Vector3.down, Mathf.Abs(delta.x));
		}
	}

	public static void EndAnimation(object sender, EventArgs args)
	{
		AnimationManager animationManager = sender as AnimationManager;
		if (null != animationManager)
		{
			animationManager.ChangeAnimation("Stand", 2, false, null, 0f);
		}
	}

	public static bool AddChildToList(GameObject parent, GameObject child)
	{
		Modal3DShow modal3DShow = NGUITools.FindInParents<Modal3DShow>(parent);
		if (null == modal3DShow)
		{
			return false;
		}
		modal3DShow.ChildGameObjectList.Add(child);
		return true;
	}

	public void OnClick()
	{
		if (this.ClickCallBack != null)
		{
			this.ClickCallBack(this, new DPSelectedItemEventArgs
			{
				ID = (int)(UICamera.lastTouchPosition.x / Global.Data.ScreenScaleX),
				IDType = (int)(UICamera.lastTouchPosition.y / Global.Data.ScreenScaleY)
			});
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public GameObject _Target;

	public int MonsterID;

	public bool CanRotate = true;

	public List<GameObject> ChildGameObjectList = new List<GameObject>();

	public DPSelectedItemEventHandler ClickCallBack;

	public DPSelectedItemEventHandler LoadCompleteCallBack;
}
