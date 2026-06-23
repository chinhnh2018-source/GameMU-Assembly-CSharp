using System;
using UnityEngine;

public class JingLingMapObj : MonoBehaviour
{
	private void Start()
	{
		base.gameObject.name = typeof(JingLingMapEvent).ToString();
		base.transform.position = JingLingMapObjectData.ObjectDataList[this.index].pos;
		JingLingMapObjFaceDisplay jingLingMapObjFaceDisplay = new GameObject
		{
			name = typeof(JingLingMapEvent).ToString() + this.index
		}.AddComponent<JingLingMapObjFaceDisplay>();
		jingLingMapObjFaceDisplay.Target = base.transform;
		jingLingMapObjFaceDisplay.nObjectIndex = this.index;
		base.gameObject.SetActive(false);
	}

	public int index;

	public enum EmBuildType
	{
		Home,
		Boss,
		Task,
		Max
	}
}
