using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

[ExecuteInEditMode]
public class KuaFuPlunderMapObj
{
	public KuaFuPlunderMapObj()
	{
		this.ModeHaveLoad = false;
	}

	public int ModeDir
	{
		get
		{
			return this.mModeDir;
		}
	}

	public byte RoundType
	{
		get
		{
			return this.mRoundType;
		}
	}

	public int NPCID
	{
		get
		{
			return this.GetNpcId(this.mModeType);
		}
	}

	public byte ModeType
	{
		get
		{
			return this.mModeType;
		}
	}

	public int ServerID
	{
		get
		{
			return this.mServerID;
		}
		set
		{
			this.mServerID = value;
		}
	}

	public Vector3 CamePos
	{
		set
		{
			this.mCamePos = value;
			this.RefreshPos(false);
			if (this.modelObject != null && null != this.modelObject.The3DGameObject && null != this.kuaFuPlunderMapObjFace && null == this.kuaFuPlunderMapObjFace.Target)
			{
				this.kuaFuPlunderMapObjFace.Target = this.modelObject.The3DGameObject.transform;
			}
		}
	}

	public int Round
	{
		get
		{
			return this.mRound;
		}
		set
		{
			this.mRound = value;
		}
	}

	public void RefreshPos(bool force = false)
	{
		if (force && this.modelObject != null)
		{
			this.modelObject.Destroy();
			this.modelObject = null;
		}
		if (this.mbShow && this.modelObject == null)
		{
			float num = Vector2.Distance(new Vector2(this.mCamePos.x + Global.MainCamera.fieldOfView * Mathf.Tan(Global.MainCamera.transform.localRotation.x), this.mCamePos.z + Global.MainCamera.fieldOfView * Mathf.Tan(Global.MainCamera.transform.localRotation.z)), new Vector2(Mathf.Abs(this.mModeX / 100f), Mathf.Abs(this.mModeY / 100f)));
			byte b = 0;
			if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && this.mServerID == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID)
			{
				b = 1;
			}
			if (b == 1)
			{
				if (!this.ModeHaveLoad)
				{
					this.LoadMode();
				}
			}
			else if (9.09f >= num)
			{
				this.LoadMode();
			}
		}
	}

	private void LoadMode()
	{
		this.ModeHaveLoad = true;
		if (this.modelObject != null)
		{
			this.modelObject.Destroy();
			this.modelObject = null;
		}
		int npcId = this.GetNpcId(this.mModeType);
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcId);
		if (npcvobyID != null)
		{
			Vector3 vector;
			vector..ctor(this.mModeX, this.mModeY, (float)this.mModeDir);
			this.kuaFuPlunderMapObjFace = U3DUtils.NEW<KuaFuPlunderMapObjFace>();
			this.kuaFuPlunderMapObjFace.transform.SetParent(HUDTextRoot.go.transform, false);
			this.kuaFuPlunderMapObjFace.transform.localPosition = new Vector3(10000f, 10000f, 0f);
			this.kuaFuPlunderMapObjFace.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
				{
					ID = 1510,
					MyID = this.ServerID
				});
			};
			this.kuaFuPlunderMapObjFace.RefreshServerInf(this.mServerID.ToString(), (int)this.mRoundType);
			this.modelObject = KuaFuPlunderMap.GetInstance().CreateBuildNPC(npcId, vector.x, vector.y, GSpriteTypes.NPC, npcvobyID.ResName, this.mServerID.ToString(), (int)vector.z);
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"<color=yellow>ERROR ServerID = ",
					this.ServerID,
					"ModeType =",
					this.mModeType,
					"</color>"
				})
			});
		}
		this.RrefreshGray();
	}

	public void RerfreshMode(float x, float y, int Dir, bool bLoad, int type)
	{
		this.mModeX = x;
		this.mModeY = y;
		this.mModeDir = Dir;
		this.mModeType = (byte)type;
		if (bLoad)
		{
			this.LoadMode();
		}
	}

	public void RerfreshMode(float x, float y, int Dir, int type)
	{
		this.mModeX = x;
		this.mModeY = y;
		this.mModeDir = Dir;
		bool flag = false;
		if ((int)this.mModeType != type)
		{
			flag = true;
		}
		this.mModeType = (byte)type;
		if (flag)
		{
			this.RefreshPos(true);
		}
	}

	public void RefreshFaceTag(GameObject tag)
	{
		if (null != this.kuaFuPlunderMapObjFace)
		{
			this.kuaFuPlunderMapObjFace.Target = tag.transform;
		}
	}

	public void RefreshInf(string name, int type)
	{
		this.mRoundType = (byte)type;
		if (null != this.kuaFuPlunderMapObjFace)
		{
			this.kuaFuPlunderMapObjFace.RefreshServerInf(name, type);
		}
	}

	private int GetNpcId(byte Index)
	{
		if (this.IsSelfServer())
		{
			return 86000;
		}
		if (Index == 0)
		{
			return 86004;
		}
		if (Index == 1)
		{
			return 86003;
		}
		if (Index == 2)
		{
			return 86002;
		}
		if (Index == 3)
		{
			return 86001;
		}
		return 86000;
	}

	private bool IsSelfServer()
	{
		return KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && this.mServerID == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID;
	}

	private void RrefreshGray()
	{
		bool flag = false;
		if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo != null)
		{
			KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaStateDataByID = KuaFuPlunderMap.GetInstance().KuaFuLueDuoMainInfo.GetKuaFuLueDuoServerJingJiaStateDataByID(this.mServerID);
			if (kuaFuLueDuoServerJingJiaStateDataByID != null && (kuaFuLueDuoServerJingJiaStateDataByID.State == 2 || kuaFuLueDuoServerJingJiaStateDataByID.State == 3) && !this.IsSelfServer())
			{
				flag = true;
			}
		}
		if (this.modelObject != null && null != this.modelObject.The3DGameObject)
		{
			Renderer component = this.modelObject.The3DGameObject.GetComponent<Renderer>();
			if (null != component && null != component.sharedMaterial)
			{
				if (flag)
				{
					component.sharedMaterial.EnableKeyword("_GRAY");
				}
				else
				{
					component.sharedMaterial.DisableKeyword("_GRAY");
				}
			}
		}
	}

	public void ShowInf(bool bShow)
	{
		this.mbShow = bShow;
		if (KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData != null && this.mServerID == KuaFuPlunderMap.GetInstance().KuaFuLueDuoStateData.ServerID)
		{
			this.mbShow = true;
		}
		if (this.modelObject != null && null != this.modelObject.The3DGameObject)
		{
			this.modelObject.The3DGameObject.SetActive(this.mbShow);
		}
		if (null != this.kuaFuPlunderMapObjFace)
		{
			this.kuaFuPlunderMapObjFace.gameObject.SetActive(this.mbShow);
		}
		if (this.mbShow)
		{
			this.RefreshPos(false);
		}
		this.RrefreshGray();
	}

	private const float Radius = 9.09f;

	private const float ModeScale = 100f;

	public GSprite modelObject;

	public KuaFuPlunderMapObjFace kuaFuPlunderMapObjFace;

	public Transform FaceRoot;

	private Vector3 mCamePos = Vector3.one;

	private float mModeX;

	private float mModeY;

	private int mModeDir;

	private int mServerID;

	private byte mModeType;

	private byte mRoundType;

	private int mRound;

	private bool ModeHaveLoad;

	private bool mbShow = true;
}
