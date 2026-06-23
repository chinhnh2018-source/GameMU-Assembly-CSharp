using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class CangBaoMiJingRoleHeadPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitRoleHead();
		this.m_Transform = base.transform;
		NGUITools.SetActive(this.m_SelectBG, false);
	}

	public override void Update()
	{
		base.Update();
		if (this.m_bMove)
		{
			this.m_HasMoveTime += this.GetTimeScale();
			if (1f > this.m_HasMoveTime)
			{
				if (this.m_HasMoveTime < 0.33f)
				{
					this.m_Speed_Time -= 0.035f;
				}
				else if (this.m_HasMoveTime > 0.66f)
				{
					this.m_Speed_Time += 0.035f;
				}
				this.m_Transform.localPosition = Vector3.Lerp(this.m_OldPos, this.m_NextPos, this.m_HasMoveTime);
			}
			else
			{
				this.m_Transform.localPosition = this.m_NextPos;
				this.m_bMove = false;
				this.m_HasMoveTime = 0f;
				this.m_RoleOnFloorID = this.m_NextFloorID;
				this.m_Speed_Time = 0.75f;
				this.Handler(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		}
	}

	private float GetTimeScale()
	{
		return Time.deltaTime / this.m_Speed_Time;
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
	}

	private void InitRoleHead()
	{
		this.m_RoleHeadImage.URL = StringUtil.substitute("{0}{1}{2}_0.png", new object[]
		{
			this.HeadPath,
			Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation),
			0
		});
	}

	public void RefreshHeadPos(Vector3 pos, bool bFristSet = true, int nextFloorID = 0)
	{
		this.m_NextFloorID = nextFloorID;
		if (bFristSet)
		{
			this.m_Transform.localPosition = pos;
		}
		else
		{
			this.m_NextPos = pos;
		}
		this.m_bMove = !bFristSet;
		this.m_OldPos = this.m_Transform.localPosition;
		if (!bFristSet)
		{
		}
	}

	public int RoleOnFloorID
	{
		get
		{
			return this.m_RoleOnFloorID;
		}
		set
		{
			this.m_RoleOnFloorID = value;
			if (30 < this.m_RoleOnFloorID)
			{
				this.m_RoleOnFloorID = 30;
			}
			else if (0 > this.m_RoleOnFloorID)
			{
				this.m_RoleOnFloorID = 0;
			}
		}
	}

	public ShowNetImage m_RoleHeadImage;

	public GameObject m_SelectBG;

	private string HeadPath = "NetImages/CangBaoFace/";

	private Transform m_Transform;

	private Vector3 m_NextPos = Vector3.zero;

	private Vector3 m_OldPos = Vector3.zero;

	private float m_Speed_Time = 0.75f;

	private float m_HasMoveTime;

	private bool m_bMove;

	private int m_RoleOnFloorID;

	private int m_NextFloorID;

	public DPSelectedItemEventHandler Handler;
}
