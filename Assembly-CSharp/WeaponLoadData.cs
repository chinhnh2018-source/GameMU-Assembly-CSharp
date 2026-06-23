using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using Server.Data;
using UnityEngine;

public class WeaponLoadData
{
	public GameObject parent;

	public List<GoodsData> weaponList = new List<GoodsData>();

	public List<string> hangPointList = new List<string>();

	public int occupation;

	public int roleSex;

	public OnLoadChildResComplete EffectCallBack;

	public GameObject WeapEffect;

	public CacheType cacheType;

	public float Scale = 1f;

	public int ToLayer = -1;

	public bool ReplaceChildLayer;

	public bool HideGameEffect;
}
