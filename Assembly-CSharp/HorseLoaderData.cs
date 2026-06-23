using System;
using System.Collections.Generic;
using UnityEngine;

public class HorseLoaderData
{
	public GameObject parent;

	public List<string> hangPointList = new List<string>();

	public string resName;

	public int occupation;

	public int roleSex;

	public int GoodsID = -1;

	public int HorseLevel;

	public Quaternion Quaternion;

	public Vector3 TransScale = Vector3.one;

	public float Scale = 1f;

	public int ToLayer = -1;

	public bool ReplaceChildLayer;

	public bool HideGameEffect;

	public HorseMountCallBask Hander;
}
