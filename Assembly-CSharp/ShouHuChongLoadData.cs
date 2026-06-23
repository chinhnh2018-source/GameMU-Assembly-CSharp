using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class ShouHuChongLoadData
{
	public string LoaderURL
	{
		get
		{
			string result = string.Empty;
			if (this.data == null || this.data.Using <= 0)
			{
				return result;
			}
			string goods3DResNameByID = Global.GetGoods3DResNameByID(this.data.GoodsID, -1);
			if (!string.IsNullOrEmpty(goods3DResNameByID))
			{
				result = Global.WebPath(string.Format("Equip/{0}", goods3DResNameByID));
			}
			return result;
		}
	}

	public GameObject parent;

	public GoodsData data;

	public ItemCategories Categoriy = ItemCategories.ChongWu;

	public AssetbundleLoaderComplete SpecialGameObjectsComplete;

	public int ToLayer = -1;

	public bool ReplaceChildLayer;

	public float Scale = 1f;

	public bool HideGameEffect;

	public string EmptyName;

	public int Occupation;
}
