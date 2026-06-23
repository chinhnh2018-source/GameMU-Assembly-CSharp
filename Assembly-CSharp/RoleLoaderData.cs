using System;
using System.Collections.Generic;
using Server.Data;
using UnityEngine;

public class RoleLoaderData
{
	public GameObject parent;

	public string SkeletonName;

	public List<GoodsData> GoodsDataList;

	public List<GoodsData> weaponGoodsDataList;

	public List<GoodsData> PetDataList;

	public string[] DefaultPartNames;

	public int Occupation;

	public int SubOccupation;

	public string VSName = string.Empty;

	public bool IsChangeBody;

	public int ChangeBodyID;

	public bool ReplaceChildLayer;

	public int ToLayer = -1;

	public float Scale = 1f;

	public bool ForceSyncLoad = true;

	public bool HideGameEffect;

	public int GuidCurrentId;

	public WingData wingData;

	public string RoleName = string.Empty;

	public int FashionWingGoodsID;

	public byte LoadRebitrhEquit;
}
