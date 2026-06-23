using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class JingLingMapHomeFace : JingLingMapObjFace
{
	private JingLingMapHomeFace()
	{
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void Awake()
	{
		base.Awake();
		this.modelObjectName = "jinglingmap_home";
	}

	private new void Start()
	{
		base.Start();
		if (this.title)
		{
			this.title.text = Global.GetLang(string.Empty);
		}
		if (this.titlebak)
		{
			this.titlebak.gameObject.SetActive(true);
		}
		if (this.Bak)
		{
			this.Bak.gameObject.SetActive(true);
		}
		if (this.btn)
		{
			this.btn.gameObject.SetActive(false);
		}
		if (this.tipicon)
		{
			this.tipicon.gameObject.SetActive(false);
		}
		if (this.bar)
		{
			this.bar.gameObject.SetActive(false);
		}
		if (this.cdlabel)
		{
			this.cdlabel.gameObject.SetActive(false);
		}
		if (this.redlabel)
		{
			this.redlabel.gameObject.SetActive(false);
		}
		this.title.Label.color = Color.white;
		JingLingMap.inst.homeface = this;
		this.ResetState();
		this.UpdateUI();
		this.UpdateGameObject();
		base.ResetRootPos();
	}

	protected override void OnBakClick(object sender, MouseEvent e)
	{
		this.ClickFunction();
	}

	protected override void OnClick(object sender, MouseEvent e)
	{
		this.ClickFunction();
	}

	private void ClickFunction()
	{
		PlayZone.GlobalPlayZone.ShowYaoSaiXinXiPartWindow();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (JingLingMap.inst.mapmini == null)
		{
			return;
		}
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.NuLiSearch)
		{
			if (Global.Data.yaosaiData != null)
			{
				this.title.text = Global.Data.yaosaiData.Mine.Name;
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome)
		{
			this.title.text = Global.Data.roleData.RoleName;
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome)
		{
			if (JingLingMap.inst.mapmini.curSelectFriendItem != null)
			{
				this.title.text = JingLingMap.inst.mapmini.curSelectFriendItem.relationData.RoleName;
			}
			else
			{
				this.title.text = string.Empty;
			}
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome)
		{
			if (Global.Data.yaosaiData != null)
			{
				if (Global.Data.yaosaiData.state == 1)
				{
					this.eState = JingLingMapHomeFace.EState.BeControling;
				}
				else
				{
					this.eState = JingLingMapHomeFace.EState.Freeing;
				}
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome)
		{
			if (Global.Data.yaosaiData != null)
			{
				if (Global.Data.yaosaiData.state == 1)
				{
					this.eState = JingLingMapHomeFace.EState.BeControling;
				}
				else
				{
					this.eState = JingLingMapHomeFace.EState.Freeing;
				}
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.NuLiSearch && Global.Data.yaosaiData != null)
		{
			if (Global.Data.yaosaiData.state == 1)
			{
				this.eState = JingLingMapHomeFace.EState.BeControling;
			}
			else
			{
				this.eState = JingLingMapHomeFace.EState.Freeing;
			}
		}
	}

	public void UpdateGameObject()
	{
		int npcID;
		if (this.eState == JingLingMapHomeFace.EState.Freeing)
		{
			npcID = 84000;
			this.preModelObjectName = this.curModelObjectName;
			this.curModelObjectName = this.modelObjectName + this.eState.ToString();
		}
		else
		{
			npcID = 84001;
			this.preModelObjectName = this.curModelObjectName;
			this.curModelObjectName = this.modelObjectName + this.eState.ToString();
		}
		NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcID);
		if (npcvobyID != null)
		{
			if (this.curModelObjectName != this.preModelObjectName && !string.IsNullOrEmpty(this.preModelObjectName))
			{
				GameObject gameObject = GameObject.Find(this.preModelObjectName);
				if (gameObject)
				{
					Object.Destroy(gameObject);
				}
				if (this.modelObject != null)
				{
					ObjectsManager.Remove(this.modelObject);
					this.modelObject = null;
				}
				this.preModelObjectName = string.Empty;
			}
			if (!GameObject.Find(this.curModelObjectName) && this.modelObject == null)
			{
				if (this.modelObject != null)
				{
					ObjectsManager.Remove(this.modelObject);
					this.modelObject = null;
				}
				float posX = 100f * JingLingMapObjectData.homeData.pos.x;
				float posY = 100f * JingLingMapObjectData.homeData.pos.z;
				this.modelObject = JingLingMap.inst.ForceCreateFadeNPC(npcID, posX, posY, GSpriteTypes.NPC, npcvobyID.ResName, this.curModelObjectName, 3);
			}
		}
		base.RemoveModel(true);
	}

	private JingLingMapHomeFace.EState eState;

	public enum EState
	{
		Freeing,
		BeControling
	}
}
