using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

public class LeaderInfo : MonoBehaviour
{
	public bool LeaderIsOnHorse
	{
		get
		{
			return this.mLeader != null && this.mLeader.OnHorseEX;
		}
	}

	private Transform Trans
	{
		get
		{
			if (this.mLeader == null)
			{
				this.mLeader = ObjectsManager.FindSprite(Global.Data.RoleID);
			}
			if (this.mLeader != null)
			{
				if (this.mLeader.OnHorseEX)
				{
					this._Trans = this.mLeader.HorseController.HorseTrans;
				}
				else
				{
					this._Trans = base.transform;
				}
			}
			if (null == this._Trans)
			{
				this._Trans = base.transform;
			}
			return this._Trans;
		}
	}

	public bool TriggerByCancel
	{
		get
		{
			return this._TriggerByCancel;
		}
		set
		{
			this._TriggerByCancel = value;
		}
	}

	private void Start()
	{
		this.mLeader = ObjectsManager.FindSprite(Global.Data.RoleID);
		this._Trans = base.transform;
	}

	private void Update()
	{
		LeaderInfo.LeaderPos = this.Trans.position;
	}

	public static Vector3 LeaderPos = Vector3.zero;

	public static bool InAction = false;

	private Transform _Trans;

	private bool _TriggerByCancel;

	private GSprite mLeader;
}
