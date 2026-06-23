using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class JingLingMapObjFaceDisplay : MonoBehaviour
{
	private void Start()
	{
		this.ShowDisplayInfo();
	}

	private void OnBecameVisible()
	{
		this.ShowDisplayInfo();
	}

	private void OnBecameInvisible()
	{
		this.HideDisplayInfo();
	}

	private void OnDestroy()
	{
		this.HideDisplayInfo();
	}

	private void ShowDisplayInfo()
	{
		if (null != this.NGUIChildObject)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		JingLingMapObj.EmBuildType buildType = JingLingMapObjectData.ObjectDataList[this.nObjectIndex].buildType;
		if (buildType == JingLingMapObj.EmBuildType.Home)
		{
			JingLingMapHomeFace jingLingMapHomeFace = U3DUtils.NEW<JingLingMapHomeFace>();
			jingLingMapHomeFace.nObjectIndex = this.nObjectIndex;
			jingLingMapHomeFace.Target = this.Target;
			this.NGUIChildObject = NGUITools.AddChild2(HUDTextRoot.go, jingLingMapHomeFace.gameObject);
		}
		else if (buildType == JingLingMapObj.EmBuildType.Boss)
		{
			JingLingMapBossFace jingLingMapBossFace = U3DUtils.NEW<JingLingMapBossFace>();
			jingLingMapBossFace.nObjectIndex = this.nObjectIndex;
			jingLingMapBossFace.Target = this.Target;
			this.NGUIChildObject = NGUITools.AddChild2(HUDTextRoot.go, jingLingMapBossFace.gameObject);
		}
		else
		{
			if (buildType != JingLingMapObj.EmBuildType.Task)
			{
				return;
			}
			JingLingMapTaskFace jingLingMapTaskFace = U3DUtils.NEW<JingLingMapTaskFace>();
			jingLingMapTaskFace.nObjectIndex = this.nObjectIndex;
			jingLingMapTaskFace.Target = this.Target;
			this.NGUIChildObject = NGUITools.AddChild2(HUDTextRoot.go, jingLingMapTaskFace.gameObject);
		}
		if (this.NGUIChildObject != null)
		{
			this.NGUIChildObject.AddComponent<UIFollowTarget>().target = this.Target;
		}
	}

	private void HideDisplayInfo()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		Object.Destroy(this.NGUIChildObject);
		this.NGUIChildObject = null;
	}

	public Transform Target;

	public string TeleNameText;

	public int nObjectIndex;

	private GameObject NGUIChildObject;
}
