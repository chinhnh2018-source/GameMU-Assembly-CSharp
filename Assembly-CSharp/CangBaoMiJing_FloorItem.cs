using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CangBaoMiJing_FloorItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitGameObjectActive();
		this.InitHandler();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void InitPrefabText()
	{
	}

	private void InitGameObjectActive()
	{
		if (null != this.m_BeginSign)
		{
			NGUITools.SetActive(this.m_BeginSign, false);
		}
		if (null != this.m_Collider)
		{
			NGUITools.SetActive(this.m_Collider, true);
		}
		if (null != this.m_SelectRoot)
		{
			NGUITools.SetActive(this.m_SelectRoot, false);
		}
		if (null != this.m_SpNumber)
		{
			NGUITools.SetActive(this.m_SpNumber, false);
		}
		GameObject gameObject = this.LoadRollTeXiaoObj("UITeXiao/Perfabs/shaizi/mizang_kuang");
		this.m_SelectRoot.transform.localScale = Vector3.one;
		gameObject.transform.SetParent(this.m_SelectRoot, false);
		gameObject.transform.localPosition = Vector3.zero;
	}

	private GameObject LoadRollTeXiaoObj(string Path)
	{
		Object @object = Resources.Load(Path);
		if (null != @object)
		{
			GameObject gameObject = SpawnManager.Instantiate(@object) as GameObject;
			gameObject.transform.SetParent(base.transform, false);
			U3DUtils.ResetLayer(gameObject, "MUUI");
			return gameObject;
		}
		return null;
	}

	private void InitHandler()
	{
		UIEventListener.Get(this.m_Collider).onClick = delegate(GameObject s)
		{
			this.Btnhandler(this, new DPSelectedItemEventArgs
			{
				ID = this.m_FloorID,
				MyID = this.m_MyTag
			});
		};
	}

	public void SetParent(Transform parent, bool worldPositionStays = false)
	{
		if (null != parent)
		{
			base.transform.SetParent(parent, worldPositionStays);
		}
	}

	public void SetPos(Vector3 pos)
	{
		base.transform.localPosition = pos;
		this.m_FloorPos = base.transform.localPosition;
	}

	public void RefreshFlooar()
	{
		if (string.IsNullOrEmpty(this._CangbaoData.Pic))
		{
			NGUITools.SetActive(this.m_AwardSign, false);
		}
		else
		{
			if (!this.m_AwardSign.gameObject.activeSelf)
			{
				NGUITools.SetActive(this.m_AwardSign, true);
			}
			string[] array = this._CangbaoData.Pic.Split(new char[]
			{
				','
			});
			if ("1" == array[0].ToString())
			{
				this.m_AwardSign.spriteName = array[1];
				this.m_FloorSP.spriteName = "MapGeZi";
				NGUITools.SetActive(this.m_AwardSign, true);
			}
			else if ("0" == array[0].ToString())
			{
				this.m_FloorSP.spriteName = array[1];
				NGUITools.SetActive(this.m_AwardSign, false);
			}
			GameObject gameObject = null;
			if (array[1] == "mijing_00")
			{
				gameObject = this.LoadRollTeXiaoObj("UITeXiao/Perfabs/shaizi/mizang_fazhen");
				Transform child = gameObject.transform.GetChild(0);
				if (null != child)
				{
					Vector3 localPosition = child.localPosition;
					localPosition.z = -9f;
					child.localPosition = localPosition;
				}
			}
			else if (array[1] == "mijing_04")
			{
				gameObject = this.LoadRollTeXiaoObj("UITeXiao/Perfabs/shaizi/mizang_baoxiang_pt");
			}
			else if (array[1] == "mijing_05")
			{
				gameObject = this.LoadRollTeXiaoObj("UITeXiao/Perfabs/shaizi/mizang_baoxiang_gj");
			}
			if (null != gameObject)
			{
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.SetParent(this.m_AwardTexiaoRoot, false);
				NGUITools.SetActive(this.m_AwardSign, false);
			}
		}
	}

	public int ID
	{
		get
		{
			return this.m_FloorID;
		}
		set
		{
			this.m_FloorID = value;
		}
	}

	public bool SelectState
	{
		get
		{
			return this.m_FloorState;
		}
		set
		{
			this.m_FloorState = value;
			if (this.m_FloorState != this.m_SelectRoot.gameObject.activeSelf)
			{
				NGUITools.SetActive(this.m_SelectRoot, this.m_FloorState);
			}
			if (this.m_FloorState)
			{
				UIPanel component = base.GetComponent<UIPanel>();
				if (null == component)
				{
					base.gameObject.AddComponent<UIPanel>();
				}
			}
			else if (this.m_FloorID != 0)
			{
				UIPanel component2 = base.GetComponent<UIPanel>();
				if (null != component2)
				{
					NGUITools.Destroy(component2);
				}
			}
		}
	}

	public int SelectDepth
	{
		set
		{
			if (value != (int)this.m_SelectRoot.transform.localPosition.z)
			{
				Vector3 localPosition = this.m_SelectRoot.transform.localPosition;
				localPosition.z = (float)value;
				this.m_SelectRoot.transform.localPosition = localPosition;
			}
		}
	}

	public CangBaoMiJing_FloorItem.Event_CangBao EventFloor
	{
		get
		{
			return this.m_FloorEvent;
		}
		set
		{
			this.m_FloorEvent = value;
		}
	}

	public bool RoleIsOn
	{
		get
		{
			return this.m_RoleOnFloorId == this.m_FloorID;
		}
	}

	public int RoleOnFloorId
	{
		get
		{
			return this.m_RoleOnFloorId;
		}
		set
		{
			this.m_RoleOnFloorId = value;
		}
	}

	public Vector3 FloorPos
	{
		get
		{
			return this.m_FloorPos;
		}
	}

	public int MyTag
	{
		get
		{
			return this.m_MyTag;
		}
		set
		{
			this.m_MyTag = value;
			if (this.m_MyTag == 0)
			{
				UIPanel component = base.transform.GetComponent<UIPanel>();
				if (null == component)
				{
					base.gameObject.AddComponent<UIPanel>();
				}
			}
		}
	}

	public GameObject m_Collider;

	public UISprite m_BeginSign;

	public UISprite m_SpNumber;

	public Transform m_SelectRoot;

	public UISprite m_AwardSign;

	public UISprite m_FloorSP;

	public DPSelectedItemEventHandler Btnhandler;

	public Transform m_AwardTexiaoRoot;

	private int m_FloorID;

	private bool m_FloorState;

	private CangBaoMiJing_FloorItem.Event_CangBao m_FloorEvent = CangBaoMiJing_FloorItem.Event_CangBao.EventNull;

	private int m_RoleOnFloorId;

	private Vector3 m_FloorPos = Vector3.zero;

	private int m_MyTag;

	public Data_CangBao_Map _CangbaoData = new Data_CangBao_Map();

	public enum Event_CangBao
	{
		EventNull = -1,
		Award,
		BigDiamond,
		Box,
		EndBox,
		Exchange,
		Fighting,
		SmailDiamond
	}
}
