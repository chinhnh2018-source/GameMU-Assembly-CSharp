using System;
using UnityEngine;

public class FxResLoaderData
{
	public Transform parent;

	public Vector3 localPosition = Vector3.zero;

	public Quaternion localRotation = Quaternion.identity;

	public string assetName;

	public string soundName;

	public int objID;

	public int magicID;

	public bool isLimited;

	public int skillID;

	public bool autoSetEmptyParent;

	public int HangPos;

	public int layer;
}
