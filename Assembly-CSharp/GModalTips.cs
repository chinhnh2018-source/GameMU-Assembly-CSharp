using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GModalTips : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	public override void OnActive(bool active)
	{
		base.OnActive(active);
		if (!active && 0 < this.mHorseResLoaderList.Count)
		{
			for (int i = 0; i < this.mHorseResLoaderList.Count; i++)
			{
				if (this.mHorseResLoaderList[i] != null)
				{
					this.mHorseResLoaderList[i].Stop();
				}
			}
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (0 < this.mHorseResLoaderList.Count)
		{
			for (int i = 0; i < this.mHorseResLoaderList.Count; i++)
			{
				if (this.mHorseResLoaderList[i] != null)
				{
					this.mHorseResLoaderList[i].Stop();
				}
			}
		}
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
			this._BakImage.URL = "NetImages/GameRes/Images/RidePet/TipsDi.png";
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
			this._IsHaveObj.SetActive(false);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void RefreshTips(GoodsOwnerTypes goodsOwner, GoodsData goodsData)
	{
		this.mHorseResLoaderList.Clear();
		bool flag = false;
		if (goodsOwner == GoodsOwnerTypes.SelfBag)
		{
			flag = true;
		}
		else
		{
			if (Global.Data.roleData.GoodsDataList != null && Global.Data.roleData.GoodsDataList.Find((GoodsData e) => goodsData.GoodsID == e.GoodsID) != null)
			{
				flag = true;
			}
			if (!flag)
			{
				if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 340)
				{
					if (Global.Data.roleData.MountEquipList != null && Global.Data.roleData.MountEquipList.Find((GoodsData e) => goodsData.GoodsID == e.GoodsID) != null)
					{
						flag = true;
					}
					if (!flag && Global.Data.roleData.MountStoreList != null && Global.Data.roleData.MountStoreList.Find((GoodsData e) => goodsData.GoodsID == e.GoodsID) != null)
					{
						flag = true;
					}
				}
				else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 10 || Global.GetGoodsCatetoriy(goodsData.GoodsID) == 9)
				{
					if (Global.Data.equipPet != null && Global.Data.equipPet.Find((GoodsData e) => goodsData.GoodsID == e.GoodsID) != null)
					{
						flag = true;
					}
					if (!flag && Global.Data.PaiZhuPetList != null && Global.Data.PaiZhuPetList.Find((GoodsData e) => goodsData.GoodsID == e.GoodsID) != null)
					{
						flag = true;
					}
					if (!flag && Global.Data.PetsDataList != null && Global.Data.PetsDataList.Find((PetData e) => goodsData.GoodsID == e.PetID) != null)
					{
						flag = true;
					}
				}
			}
		}
		this._IsHaveObj.SetActive(flag);
		if (goodsData != null)
		{
			if (this._Modal.ChildGameObjectList != null && 0 < this._Modal.ChildGameObjectList.Count)
			{
				this._Modal.Clear();
			}
			HorseResLoader horseResLoader = UIHelper.LoadHorseRes(this._Modal, goodsData.GoodsID, goodsData.Forge_level + 1, Quaternion.Euler(new Vector3(0f, 125f, 0f)), new Vector3(110f, 110f, 110f), delegate(GameObject g)
			{
				if (this._Modal.ChildGameObjectList != null && 1 < this._Modal.ChildGameObjectList.Count)
				{
					for (int i = this._Modal.ChildGameObjectList.Count - 1; i > 0; i--)
					{
						if (null != this._Modal.ChildGameObjectList[i])
						{
							Object.Destroy(this._Modal.ChildGameObjectList[i]);
							this._Modal.ChildGameObjectList.RemoveAt(this._Modal.ChildGameObjectList.Count - 1);
						}
					}
					this._Modal._Target = this._Modal.ChildGameObjectList[0];
				}
			});
			this.mHorseResLoaderList.Add(horseResLoader);
		}
		this._Title.text = string.Empty;
		int categoriyByGoodsID = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
		if (goodsData.Forge_level >= 0)
		{
			this._Title.text = (goodsData.Forge_level + 1).ToString() + Global.GetLang("阶");
		}
		if (categoriyByGoodsID == 340 && (goodsOwner == GoodsOwnerTypes.LookOther || goodsOwner == GoodsOwnerTypes.OtherRole || goodsOwner == GoodsOwnerTypes.OtherRole2))
		{
			UILabel title = this._Title;
			title.text = title.text + goodsData.Strong.ToString() + Global.GetLang("级");
		}
	}

	[SerializeField]
	private UILabel _Title;

	[SerializeField]
	private Modal3DShow _Modal;

	[SerializeField]
	private GameObject _IsHaveObj;

	[SerializeField]
	private ShowNetImage _BakImage;

	private List<HorseResLoader> mHorseResLoaderList = new List<HorseResLoader>();
}
