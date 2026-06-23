using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Sprite;
using Server.Data;
using UnityEngine;

public class HorseController
{
	public HorseController(GSprite HoserContriller)
	{
		this.gSprite = HoserContriller;
	}

	public Animator HorseAnimator
	{
		get
		{
			if (null == this._Animator && null != this._TheHorseGameobject)
			{
				this._Animator = this._TheHorseGameobject.GetComponent<Animator>();
			}
			return this._Animator;
		}
	}

	public int HorseID
	{
		get
		{
			if (this._HorseLoaderData != null)
			{
				return this._HorseLoaderData.GoodsID;
			}
			return 0;
		}
	}

	public GameObject TheHorseGameobject
	{
		get
		{
			return this._TheHorseGameobject;
		}
		set
		{
			this._TheHorseGameobject = value;
			if (!(null != this._TheHorseGameobject))
			{
				this.mTheHorseMountPoint = null;
			}
		}
	}

	public Transform TheHorseMountPoint
	{
		get
		{
			if (null == this.mTheHorseMountPoint)
			{
				GameObject gameObject = U3DUtils.FindGameObjectByName(this.TheHorseGameobject, "zuoqi");
				if (null != gameObject)
				{
					this.mTheHorseMountPoint = gameObject.transform;
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"坐骑的挂点没有找到"
					});
				}
			}
			return this.mTheHorseMountPoint;
		}
	}

	public Transform HorseTrans
	{
		get
		{
			if (null != this._TheHorseGameobject)
			{
				return this._TheHorseGameobject.transform;
			}
			return null;
		}
	}

	public bool OnHorse { get; set; }

	public void UnLoadMount(byte DestoryMount)
	{
		if (this._HorseResLoader != null)
		{
			this._HorseResLoader.Stop();
			this._HorseResLoader = null;
		}
		this.ReleaseHorse(DestoryMount);
	}

	public void LoadMount(HorseMountCallBask horseMountCallBask = null)
	{
		if (this.gSprite == null)
		{
			if (horseMountCallBask != null)
			{
				horseMountCallBask(null);
			}
			return;
		}
		this.mUnloadTime = 10f;
		this.mRoleMoveTimes = 0f;
		if (this.gSprite.SpriteType == GSpriteTypes.Leader || this.gSprite.SpriteType == GSpriteTypes.Other)
		{
			if (ConfigSettings.GetSettingHorseCanUseByMapCode(Global.Data.roleData.MapCode))
			{
				GoodsData goodsData = null;
				if (0 < this.gSprite.RoleID)
				{
					goodsData = Global.GetRoleFightHorseData(this.gSprite.RoleID);
				}
				if (goodsData != null)
				{
					GoodsData goodsData2 = null;
					if (this.gSprite.SpriteType == GSpriteTypes.Leader)
					{
						goodsData2 = Global.GetRoleHorseFashionList(0).Find((GoodsData e) => 1 == e.Using);
					}
					else if (Global.Data.OtherRoles.ContainsKey(this.gSprite.RoleID))
					{
						List<GoodsData> goodsDataList = Global.Data.OtherRoles[this.gSprite.RoleID].GoodsDataList;
						if (goodsDataList != null)
						{
							goodsData2 = goodsDataList.Find((GoodsData e) => Global.GetCategoriyByGoodsID(e.GoodsID) == 27 && 1 == e.Using);
						}
					}
					GoodVO goodsXmlNodeByID;
					if (goodsData2 != null)
					{
						goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData2.GoodsID);
					}
					else
					{
						goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
					}
					if (goodsXmlNodeByID != null)
					{
						byte b = 0;
						if (this._HorseLoaderData != null && this._HorseLoaderData.GoodsID == goodsXmlNodeByID.ID && this.ActiveHorse())
						{
							b = 1;
						}
						if (b == 0)
						{
							if (this._HorseResLoader != null)
							{
								this._HorseResLoader.Stop();
								this._HorseResLoader = null;
							}
							if (this._HorseLoaderData == null)
							{
								this._HorseLoaderData = new HorseLoaderData();
							}
							this._HorseLoaderData.GoodsID = goodsXmlNodeByID.ID;
							this._HorseLoaderData.HorseLevel = goodsData.Forge_level + 1;
							this._HorseLoaderData.parent = null;
							this._HorseLoaderData.resName = Global.GetGoods3DResNameByID(goodsXmlNodeByID.ID, -1);
							this._HorseLoaderData.ReplaceChildLayer = true;
							this._HorseLoaderData.ToLayer = LayerMask.NameToLayer(this.gSprite.DefaultLayerName);
							this._HorseLoaderData.Hander = horseMountCallBask;
							this._HorseResLoader = new HorseResLoader(this._HorseLoaderData, new OnHorserLoaderComplete(this.HorseLoaderComplete));
						}
						else if (horseMountCallBask != null)
						{
							horseMountCallBask(this._TheHorseGameobject);
						}
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"XML  坐骑加载出错"
						});
						if (horseMountCallBask != null)
						{
							horseMountCallBask(null);
						}
					}
				}
				else if (horseMountCallBask != null)
				{
					horseMountCallBask(null);
				}
			}
			else if (horseMountCallBask != null)
			{
				horseMountCallBask(null);
			}
		}
	}

	private bool ActiveHorse()
	{
		if (null != this._TheHorseGameobject)
		{
			MonoBehaviour[] componentsInChildren = this.TheHorseGameobject.GetComponentsInChildren<MonoBehaviour>(true);
			Renderer[] componentsInChildren2 = this.TheHorseGameobject.GetComponentsInChildren<Renderer>(true);
			ParticleSystem[] componentsInChildren3 = this.TheHorseGameobject.GetComponentsInChildren<ParticleSystem>(true);
			if (componentsInChildren != null)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (null != componentsInChildren[i])
					{
						componentsInChildren[i].enabled = true;
					}
				}
			}
			if (componentsInChildren2 != null)
			{
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					if (null != componentsInChildren2[j])
					{
						componentsInChildren2[j].enabled = true;
					}
				}
			}
			if (componentsInChildren3 != null)
			{
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					if (null != componentsInChildren3[k])
					{
						componentsInChildren3[k].Play(true);
					}
				}
			}
			return true;
		}
		return false;
	}

	private void ReleaseHorse(byte DestoryMount)
	{
		if (null != this.TheHorseGameobject)
		{
			if (DestoryMount == 0)
			{
				try
				{
					this.TheHorseGameobject.transform.localPosition = new Vector3(-2000f, -2000f, 0f);
					MonoBehaviour[] componentsInChildren = this.TheHorseGameobject.GetComponentsInChildren<MonoBehaviour>(true);
					Renderer[] componentsInChildren2 = this.TheHorseGameobject.GetComponentsInChildren<Renderer>(true);
					ParticleSystem[] componentsInChildren3 = this.TheHorseGameobject.GetComponentsInChildren<ParticleSystem>(true);
					if (componentsInChildren != null)
					{
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							if (null != componentsInChildren[i])
							{
								componentsInChildren[i].enabled = false;
							}
						}
					}
					if (componentsInChildren2 != null)
					{
						for (int j = 0; j < componentsInChildren2.Length; j++)
						{
							if (null != componentsInChildren2[j])
							{
								componentsInChildren2[j].enabled = false;
							}
						}
					}
					if (componentsInChildren3 != null)
					{
						for (int k = 0; k < componentsInChildren3.Length; k++)
						{
							if (null != componentsInChildren3[k])
							{
								componentsInChildren3[k].Stop(true);
							}
						}
					}
				}
				catch (Exception ex)
				{
					MUDebug.LogError<string>(new string[]
					{
						ex.Message
					});
				}
			}
			else
			{
				this.Destorty();
			}
		}
	}

	private void HorseLoaderComplete(HorseLoaderData data, GameObject go)
	{
		if (null == go)
		{
			if (data.Hander != null)
			{
				data.Hander(null);
			}
			return;
		}
		try
		{
			if (this.gSprite.SpriteType != GSpriteTypes.Leader)
			{
				if (Global.Data.SysSetting.HideGameEffect)
				{
					U3DUtils.ReplaceAlphaMaterials(go);
					this.mEffectArrayControl = go.GetComponent<EffectArrayControl>();
					if (null != this.mEffectArrayControl)
					{
						this.mEffectArrayControl.ShowEffect(false);
						this.mEffectArrayControl.ShowtParticle(false, false);
					}
					else
					{
						MUDebug.Log<string>(new string[]
						{
							"<color=yellow>  EffectArrayControl  == null</color>"
						});
					}
				}
				go.name = "RoleHorse_" + this.gSprite.RoleID.ToString();
			}
			else
			{
				go.name = "LeaderHorse_" + this.gSprite.RoleID.ToString();
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogError<string>(new string[]
			{
				ex.Message
			});
		}
		try
		{
			this.TheHorseGameobject = go;
			U3DUtils.ReplaceLayerInChildren(this._TheHorseGameobject, data.ToLayer, null);
			this._Animator = go.GetComponent<Animator>();
		}
		catch (Exception ex2)
		{
			MUDebug.LogError<string>(new string[]
			{
				ex2.Message
			});
		}
		if (data.Hander != null)
		{
			data.Hander(go);
		}
	}

	public void RefreshHorseLevelEffect()
	{
	}

	public void SetPos(Vector3 pos)
	{
		if (null != this.TheHorseGameobject)
		{
			this.TheHorseGameobject.transform.position = pos;
		}
	}

	public void SetRotation(Quaternion Qua)
	{
		if (null != this.TheHorseGameobject)
		{
			this.TheHorseGameobject.transform.localRotation = Qua;
		}
	}

	internal void OnReameRender()
	{
		if (this.gSprite != null)
		{
			GSpriteTypes spriteType = this.gSprite.SpriteType;
			GActions action = this.gSprite.Action;
			if (!this.OnHorse)
			{
				if (action == GActions.Walk || action == GActions.Run)
				{
					if (spriteType == GSpriteTypes.Leader && !ShenHunData.IsInBianShenState() && ConfigSettings.GetSettingHorseCanUseByMapCode(Global.Data.roleData.MapCode) && Global.RoleHaveFightHorse(this.gSprite.RoleID))
					{
						this.mRoleMoveTimes += Time.deltaTime;
						if (2f <= this.mRoleMoveTimes && !Global.IsInHorseRequestRideCD())
						{
							Global.LastRequestRideHorseTick = Global.GetCorrectLocalTime();
							this.mRoleMoveTimes = 0f;
							GameInstance.Game.SendZuoQiRide();
						}
					}
				}
				else
				{
					if (spriteType == GSpriteTypes.Leader)
					{
						this.mRoleMoveTimes = 0f;
					}
					if (null != this._TheHorseGameobject)
					{
						this.mUnloadTime -= Time.deltaTime;
						if (0f >= this.mUnloadTime)
						{
							this.Destorty();
						}
					}
				}
			}
		}
	}

	internal void Destorty()
	{
		if (null != this._TheHorseGameobject)
		{
			Object.Destroy(this._TheHorseGameobject);
			this._TheHorseGameobject = null;
		}
	}

	private const float HorseUnloadTime = 10f;

	private const float HorseLoadTime = 2f;

	private GSprite gSprite;

	private Animator _Animator;

	private EffectArrayControl mEffectArrayControl;

	private GameObject _TheHorseGameobject;

	private Transform mTheHorseMountPoint;

	private HorseLoaderData _HorseLoaderData;

	private HorseResLoader _HorseResLoader;

	private float mUnloadTime;

	protected float mRoleMoveTimes;
}
