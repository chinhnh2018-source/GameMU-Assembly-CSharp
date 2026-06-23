using System;
using UnityEngine;

public class CollectionObject
{
	public CollectionObject(int index, GameObject obj)
	{
		this.index = index;
		this.obj = obj;
	}

	public int index;

	public GameObject obj;
}
