using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class MonsterNPCLoaderData
{
	public GameObject parent;

	public string resName;

	public GSpriteTypes spriteType;

	public float scale = 1f;

	public int leftWeaponID = -1;

	public int leftShaderID = -1;

	public int rightWeaponID = -1;

	public int rightShaderID = -1;

	public int layer;

	public bool hideEffect;

	public OnLoadMonsterNPCComplete completeFinishCallback;

	public OnInstantiateNewPoolObject monsterInstantiateCallback;

	public int TagEx;

	public int MonsterID = -1;

	public bool ReplaceChildLayer;
}
