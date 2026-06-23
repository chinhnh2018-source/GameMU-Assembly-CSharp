using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JingLingMapTaskFace : JingLingMapObjFace
{
	public byte ClickGetAwark
	{
		get
		{
			if (this.eState == JingLingMapTaskFace.EState.FinishFailing || this.eState == JingLingMapTaskFace.EState.FinishSuccessing || this.eState == JingLingMapTaskFace.EState.TimeEndRenWuing)
			{
				return this.mClickGetAwark;
			}
			return 0;
		}
		set
		{
			this.mClickGetAwark = value;
		}
	}

	public string[] GetAwardStrAray()
	{
		return this.ConfigPetMissionXml.GetAwardStrAray(this.YaoSaiMissionData.State == 1);
	}

	public JingLingMapTaskFace.EState TaskState
	{
		get
		{
			return this.eState;
		}
	}

	public YaoSaiMissionData YaoSaiMissionData
	{
		get
		{
			if (JingLingMap.inst.curMissionDataLst.Count > 0)
			{
				return JingLingMap.inst.curMissionDataLst[this.TypeIndex];
			}
			return null;
		}
	}

	public int TypeIndex
	{
		get
		{
			return JingLingMapObjectData.ObjectDataList[this.nObjectIndex].typeIndex;
		}
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void Awake()
	{
		base.Awake();
		this.modelObjectName = "jinglingmap_task_";
	}

	private new void Start()
	{
		base.Start();
		this.tipicon.gameObject.SetActive(false);
		this.title.gameObject.SetActive(false);
		this.titlebak.gameObject.SetActive(false);
		this.btn.gameObject.SetActive(false);
		this.bar.gameObject.SetActive(false);
		this.cdlabel.gameObject.SetActive(false);
		this.redlabel.gameObject.SetActive(false);
		this.iconlistRoot.gameObject.SetActive(false);
		this.succIcon.gameObject.SetActive(false);
		this.failIcon.gameObject.SetActive(false);
		this.title.Label.color = Color.white;
		this.emEmBuildType = JingLingMapObj.EmBuildType.Task;
		JingLingMap.inst.taskfaces.Add(this.TypeIndex, this);
		this.ResetState();
		this.UpdateUI();
		base.ResetRootPos();
	}

	private string TimeToString(int second)
	{
		int num = second % 60;
		int num2 = (second - num) / 3600;
		int number = (second - num) / 60 - num2 * 60;
		return string.Concat(new string[]
		{
			this.GetTwoOrderNumber(num2),
			":",
			this.GetTwoOrderNumber(number),
			":",
			this.GetTwoOrderNumber(num)
		});
	}

	private string GetTwoOrderNumber(int number)
	{
		if (10 > number)
		{
			return "0" + number.ToString();
		}
		return number.ToString();
	}

	protected override void OnBakClick(object sender, MouseEvent e)
	{
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome && this.emEmBuildType == JingLingMapObj.EmBuildType.Task)
		{
			PlayZone.GlobalPlayZone.ShowForTressTaskPart(this.YaoSaiMissionData);
		}
	}

	protected override void OnClick(object sender, MouseEvent e)
	{
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome && this.emEmBuildType == JingLingMapObj.EmBuildType.Task && (this.eState == JingLingMapTaskFace.EState.FinishSuccessing || this.eState == JingLingMapTaskFace.EState.FinishFailing))
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendJingLingYaiSaiToFight(this.YaoSaiMissionData.SiteID);
			this.mClickGetAwark = 1;
		}
	}

	private void HideAllUI()
	{
		this.title.gameObject.SetActive(false);
		this.titlebak.gameObject.SetActive(false);
		this.btn.gameObject.SetActive(false);
		this.bar.gameObject.SetActive(false);
		this.cdlabel.gameObject.SetActive(false);
		this.redlabel.gameObject.SetActive(false);
		this.iconlistRoot.gameObject.SetActive(false);
		this.succIcon.gameObject.SetActive(false);
		this.failIcon.gameObject.SetActive(false);
		this.tipicon.gameObject.SetActive(false);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (JingLingMap.inst.mapmini == null)
		{
			return;
		}
		this.UpdateGameObject();
		if (this.eState == JingLingMapTaskFace.EState.NoServerData)
		{
			this.title.gameObject.SetActive(false);
			this.titlebak.gameObject.SetActive(false);
			this.btn.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			this.redlabel.gameObject.SetActive(false);
			this.iconlistRoot.gameObject.SetActive(false);
			this.succIcon.gameObject.SetActive(false);
			this.failIcon.gameObject.SetActive(false);
			this.tipicon.gameObject.SetActive(false);
		}
		else if (this.eState == JingLingMapTaskFace.EState.JieRenWuing)
		{
			this.title.gameObject.SetActive(true);
			this.titlebak.gameObject.SetActive(true);
			this.btn.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			this.redlabel.gameObject.SetActive(false);
			this.iconlistRoot.gameObject.SetActive(false);
			this.succIcon.gameObject.SetActive(false);
			this.failIcon.gameObject.SetActive(false);
			this.tipicon.gameObject.SetActive(true);
			this.title.Text = this.ConfigPetMissionXml.GetPetMissionVO().TitleColorString;
			if (!JingLingMap.IsInHome())
			{
				this.HideAllUI();
			}
		}
		else if (this.eState == JingLingMapTaskFace.EState.TimeKeepRenWuing)
		{
			if (JingLingMap.IsInHome())
			{
				this.title.gameObject.SetActive(true);
				this.titlebak.gameObject.SetActive(true);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(true);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				if (!JingLingMap.IsInHome())
				{
					this.title.gameObject.SetActive(false);
					this.titlebak.gameObject.SetActive(false);
					this.cdlabel.gameObject.SetActive(true);
				}
				if (this.ConfigPetMissionXml != null)
				{
					this.title.text = this.ConfigPetMissionXml.GetPetMissionVO().TitleColorString;
				}
				this._UpdateCD();
				this._UpdateIconList();
			}
			else
			{
				this.HideAllUI();
			}
		}
		else if (this.eState == JingLingMapTaskFace.EState.TimeEndRenWuing || this.eState == JingLingMapTaskFace.EState.FinishSuccessing || this.eState == JingLingMapTaskFace.EState.FinishFailing)
		{
			if (JingLingMap.IsInHome())
			{
				this.title.gameObject.SetActive(true);
				this.titlebak.gameObject.SetActive(true);
				this.btn.gameObject.SetActive(true);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(true);
				this.redlabel.gameObject.SetActive(false);
				this.iconlistRoot.gameObject.SetActive(false);
				this.succIcon.gameObject.SetActive(false);
				this.failIcon.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				if (this.ConfigPetMissionXml != null)
				{
					this.title.text = this.ConfigPetMissionXml.GetPetMissionVO().TitleColorString;
				}
				this._UpdateCD();
				this.btn.Text = Global.GetLang("获得奖励");
				this.btn.normalSprite = "tongyongBtn2_2";
				this.btn.pressedSprite = "tongyongBtn2_2";
				this.btn.hoverSprite = "tongyongBtn2_2";
				this.btn.disabledSprite = "tongyongBtn2_2";
				if (!JingLingMap.IsInHome())
				{
					this.btn.gameObject.SetActive(false);
				}
				if (this.eState == JingLingMapTaskFace.EState.FinishFailing)
				{
					this.failIcon.gameObject.SetActive(true);
				}
				else if (this.eState == JingLingMapTaskFace.EState.FinishSuccessing)
				{
					this.succIcon.gameObject.SetActive(true);
				}
			}
			else
			{
				this.HideAllUI();
			}
		}
		if (this.btn.gameObject.activeSelf)
		{
			this.title.gameObject.SetActive(false);
			this.titlebak.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			if (this.redlabel)
			{
				this.redlabel.gameObject.SetActive(false);
			}
		}
		else if (this.cdlabel.gameObject.activeSelf)
		{
			this.title.gameObject.SetActive(false);
			this.titlebak.gameObject.SetActive(false);
			this.btn.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			if (this.redlabel)
			{
				this.redlabel.gameObject.SetActive(false);
			}
		}
		else if (this.title.gameObject.activeSelf)
		{
			this.btn.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			if (this.redlabel)
			{
				this.redlabel.gameObject.SetActive(false);
			}
		}
	}

	private void _UpdateCD()
	{
		if (this.eState == JingLingMapTaskFace.EState.TimeKeepRenWuing)
		{
			if (this.ConfigPetMissionXml != null)
			{
				float time = this.ConfigPetMissionXml.GetPetMissionVO().Time;
				this.cdlabel.text = "00:00:00";
				long num = Global.GetCorrectDateTime().Ticks - this.YaoSaiMissionData.StartTime.Ticks;
				if (0L <= num)
				{
					TimeSpan timeSpan;
					timeSpan..ctor(num);
					if (timeSpan.TotalSeconds < (double)this.ConfigPetMissionXml.GetPetMissionVO().Time)
					{
						this.cdlabel.text = this.TimeToString((int)((double)this.ConfigPetMissionXml.GetPetMissionVO().Time - timeSpan.TotalSeconds));
					}
					else
					{
						this.mSendGetJingLingYaiSaiDataCD += 1;
						GameInstance.Game.SendGetJingLingYaiSaiData(Global.Data.roleData.RoleID);
						this.eState = JingLingMapTaskFace.EState.TimeEndRenWuing;
					}
				}
			}
			else
			{
				this.cdlabel.gameObject.SetActive(false);
			}
		}
		else
		{
			if (this.eState == JingLingMapTaskFace.EState.TimeEndRenWuing && this.YaoSaiMissionData.State == 3 && this.mSendGetJingLingYaiSaiDataCD < 255)
			{
				if (this.mSendGetJingLingYaiSaiDataCD % 10 == 0)
				{
					this.mSendGetJingLingYaiSaiDataCD = 0;
					GameInstance.Game.SendGetJingLingYaiSaiData(Global.Data.roleData.RoleID);
				}
				this.mSendGetJingLingYaiSaiDataCD += 1;
			}
			if (!NGUITools.GetActive(this.cdlabel.gameObject))
			{
				this.cdlabel.gameObject.SetActive(false);
			}
		}
		if (!JingLingMap.IsInHome())
		{
			this.cdlabel.gameObject.SetActive(false);
		}
	}

	private void _UpdateIconList()
	{
		this.iconlistRoot.SetActive(false);
	}

	public override void UITimer_Tick(object sender, object e)
	{
		this._UpdateCD();
		this.UpdateGameObject();
	}

	public override void ResetState()
	{
		base.ResetState();
		this.eState = JingLingMapTaskFace.EState.NoServerData;
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome || JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome)
		{
			if (JingLingMap.inst.curMissionDataLst == null)
			{
				return;
			}
			YaoSaiMissionData yaoSaiMissionData = this.YaoSaiMissionData;
			if (yaoSaiMissionData == null)
			{
				return;
			}
			if (yaoSaiMissionData.State == 3)
			{
				if (yaoSaiMissionData.StartTime != DateTime.MinValue && yaoSaiMissionData.StartTime != DateTime.MaxValue)
				{
					if (this.ConfigPetMissionXml.GetPetMissionVO() != null)
					{
						float time = this.ConfigPetMissionXml.GetPetMissionVO().Time;
						long num = Global.GetCorrectDateTime().Ticks - yaoSaiMissionData.StartTime.Ticks;
						if (0L <= num)
						{
							TimeSpan timeSpan;
							timeSpan..ctor(num);
							if ((double)time <= timeSpan.TotalSeconds)
							{
								this.eState = JingLingMapTaskFace.EState.TimeEndRenWuing;
							}
							else
							{
								this.eState = JingLingMapTaskFace.EState.TimeKeepRenWuing;
							}
						}
						else
						{
							this.eState = JingLingMapTaskFace.EState.TimeEndRenWuing;
						}
					}
				}
			}
			else if (yaoSaiMissionData.State == 0)
			{
				this.eState = JingLingMapTaskFace.EState.JieRenWuing;
			}
			else if (yaoSaiMissionData.State == 1)
			{
				this.eState = JingLingMapTaskFace.EState.FinishSuccessing;
			}
			else if (yaoSaiMissionData.State == 2)
			{
				this.eState = JingLingMapTaskFace.EState.FinishFailing;
			}
			else
			{
				this.eState = JingLingMapTaskFace.EState.TimeKeepRenWuing;
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.NuLiSearch)
		{
		}
	}

	public void UpdateGameObject()
	{
		bool flag = false;
		if ((JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome || JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome) && this.YaoSaiMissionData != null && this.ConfigPetMissionXml.GetPetMissionVO() != null)
		{
			int npcId = this.ConfigPetMissionXml.GetPetMissionVO().NpcId;
			int quality = this.ConfigPetMissionXml.GetPetMissionVO().Quality;
			NPCInfoVO npcvobyID = ConfigNPCs.GetNPCVOByID(npcId);
			this.preModelObjectName = this.curModelObjectName;
			this.curModelObjectName = string.Concat(new object[]
			{
				this.modelObjectName,
				npcId.ToString(),
				"_",
				this.ConfigPetMissionXml.GetPetMissionVO().ID.ToString(),
				"_",
				this.ConfigPetMissionXml.GetPetMissionVO().Type
			});
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
			flag = true;
			if (flag)
			{
				if (!GameObject.Find(this.curModelObjectName) && this.modelObject == null)
				{
					if (npcvobyID != null)
					{
						if (this.modelObject != null)
						{
							ObjectsManager.Remove(this.modelObject);
							this.modelObject = null;
						}
						float posX = 100f * JingLingPos.taskpos(this.ConfigPetMissionXml.GetPetMissionVO().Type, 0).x;
						float posY = 100f * JingLingPos.taskpos(this.ConfigPetMissionXml.GetPetMissionVO().Type, 0).z;
						this.modelObject = JingLingMap.inst.ForceCreateFadeNPC(npcId, posX, posY, GSpriteTypes.NPC, npcvobyID.ResName, this.curModelObjectName, 3);
						this.Target.localPosition = JingLingPos.taskpos(this.ConfigPetMissionXml.GetPetMissionVO().Type, 0);
					}
				}
			}
		}
		base.RemoveModel(flag);
	}

	private ConfigPetMissionXml ConfigPetMissionXml
	{
		get
		{
			YaoSaiMissionData yaoSaiMissionData = this.YaoSaiMissionData;
			if (yaoSaiMissionData == null)
			{
				return null;
			}
			if (this.mConfigPetMissionXml == null || yaoSaiMissionData.MissionID != this.mConfigPetMissionXml.GetPetMissionVO().ID)
			{
				this.mConfigPetMissionXml = new ConfigPetMissionXml(yaoSaiMissionData.MissionID);
				return this.mConfigPetMissionXml;
			}
			return this.mConfigPetMissionXml;
		}
	}

	private JingLingMapTaskFace.EState eState;

	public UISprite succIcon;

	public UISprite failIcon;

	public UISprite[] iconlistbak = new UISprite[3];

	public ShowNetImage[] iconlist = new ShowNetImage[3];

	public GameObject iconlistRoot;

	private byte mClickGetAwark;

	private byte mSendGetJingLingYaiSaiDataCD;

	private ConfigPetMissionXml mConfigPetMissionXml;

	public enum EState
	{
		NoServerData,
		JieRenWuing,
		TimeKeepRenWuing,
		TimeEndRenWuing,
		FinishSuccessing,
		FinishFailing
	}
}
