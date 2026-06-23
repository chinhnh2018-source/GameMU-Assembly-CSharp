using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class JingLingMapBossFace : JingLingMapObjFace
{
	private new void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void Awake()
	{
		base.Awake();
		this.modelObjectName = "jinglingmap_boss";
	}

	private new void Start()
	{
		base.Start();
		this.title.text = Global.GetLang("召唤");
		this.titlebak.gameObject.SetActive(true);
		this.Bak.gameObject.SetActive(true);
		this.btn.gameObject.SetActive(false);
		this.tipicon.gameObject.SetActive(false);
		this.bar.gameObject.SetActive(false);
		this.cdlabel.gameObject.SetActive(false);
		this.redlabel.gameObject.SetActive(false);
		this.title.Label.color = Color.white;
		JingLingMap.inst.bossface = this;
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

	public void ClickFunction()
	{
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome)
		{
			if (this.eState == JingLingMapBossFace.EState.ZhaoHuaning)
			{
				int zhaoHuanBossIDss = JingLingMap.inst.CurYaoSaiBossMainData.ZhaoHuanBossIDss;
				JingLingMapObjFace.pz.OpenYaoSaiCallBossPartWindow(zhaoHuanBossIDss, JingLingMap.inst.CurYaoSaiBossMainData.TaoFaCount, JingLingMap.inst.CurYaoSaiBossMainData.HaveZhaoHuanCount);
			}
			else if (this.eState == JingLingMapBossFace.EState.TaoFaing)
			{
				GameInstance.Game.SendFightBossInfoRequest(JingLingMap.inst.CurYaoSaiBossMainData.OtherID);
			}
			else if (this.eState == JingLingMapBossFace.EState.LingJianging || this.eState == JingLingMapBossFace.EState.TimeoutLingJianging)
			{
				GameInstance.Game.SendFightBossInfoRequest(Global.Data.RoleID);
			}
			else if (this.eState != JingLingMapBossFace.EState.NoTimes)
			{
				if (this.eState == JingLingMapBossFace.EState.NoServerData)
				{
				}
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome && this.eState == JingLingMapBossFace.EState.TaoFaing)
		{
			GameInstance.Game.SendFightBossInfoRequest(JingLingMap.inst.CurYaoSaiBossMainData.OtherID);
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (JingLingMap.inst.mapmini == null)
		{
			return;
		}
		this.UpdateGameObject();
		if (this.eState == JingLingMapBossFace.EState.NoServerData)
		{
			this.title.gameObject.SetActive(false);
			this.titlebak.gameObject.SetActive(false);
			this.btn.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			this.redlabel.gameObject.SetActive(false);
			this.tipicon.gameObject.SetActive(false);
			JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
		}
		else if (this.eState == JingLingMapBossFace.EState.ZhaoHuaning)
		{
			if (JingLingMap.IsInHome())
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(true);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				this.btn.Text = Global.GetLang("召唤");
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
			}
			else
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
			}
		}
		else if (this.eState == JingLingMapBossFace.EState.TaoFaing)
		{
			if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome || JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome)
			{
				this.title.gameObject.SetActive(true);
				this.titlebak.gameObject.SetActive(true);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(true);
				this.cdlabel.gameObject.SetActive(true);
				this.redlabel.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
				PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID);
				MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID);
				if (JingLingMap.IsInHome())
				{
					int num = 3;
					int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossFight", ',');
					if (systemParamIntArrayByName.Length > 0)
					{
						num = systemParamIntArrayByName[0];
					}
					int fightCount = JingLingMap.inst.CurYaoSaiBossMainData.FightCount;
					if (fightCount < num)
					{
						this.tipicon.gameObject.SetActive(true);
					}
					else
					{
						this.tipicon.gameObject.SetActive(false);
					}
				}
				else
				{
					JingLingMapFriendItem itemByRoleID = JingLingMapFriendItem.getItemByRoleID(JingLingMap.inst.curRoleID);
					if (itemByRoleID != null)
					{
						if (itemByRoleID.relationData.YaoSaiBossState == 2)
						{
							this.tipicon.gameObject.SetActive(true);
						}
						else
						{
							this.tipicon.gameObject.SetActive(false);
						}
					}
				}
				float num2 = (float)JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.LifeV;
				float num3 = (float)monsterXmlNodeByID.MaxLife;
				this.title.text = monsterXmlNodeByID.SName;
				this.cdlabel.text = "00:00:00";
				if (num3 > 0f)
				{
					int num4 = (int)(100f * (num2 / num3));
					if (num4 <= 0 && num2 > 0f)
					{
						num4 = 1;
					}
					this.bar.ProgessText = string.Format("{0}%", num4);
					this.bar.Percent = (double)(num2 / num3);
				}
				DateTime deadTime = JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.DeadTime;
				TimeSpan timeSpan = deadTime - Global.GetCorrectDateTime();
				if (timeSpan.Ticks > 0L)
				{
					this.cdlabel.text = string.Format("{0}:{1}:{2}", timeSpan.Hours.ToString("00"), timeSpan.Minutes.ToString("00"), timeSpan.Seconds.ToString("00"));
				}
			}
			else
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
			}
		}
		else if (this.eState == JingLingMapBossFace.EState.LingJianging || this.eState == JingLingMapBossFace.EState.TimeoutLingJianging)
		{
			if (JingLingMap.IsInHome())
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(true);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
				this.btn.Text = Global.GetLang("领奖");
			}
			else
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
			}
		}
		else if (this.eState == JingLingMapBossFace.EState.NoTimes)
		{
			if (JingLingMap.IsInHome())
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(true);
				this.tipicon.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
				this.redlabel.text = Global.GetLang("今日讨伐次数已满");
			}
			else
			{
				this.title.gameObject.SetActive(false);
				this.titlebak.gameObject.SetActive(false);
				this.btn.gameObject.SetActive(false);
				this.bar.gameObject.SetActive(false);
				this.cdlabel.gameObject.SetActive(false);
				this.redlabel.gameObject.SetActive(false);
				this.tipicon.gameObject.SetActive(false);
				JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
			}
		}
		else
		{
			this.title.gameObject.SetActive(false);
			this.titlebak.gameObject.SetActive(false);
			this.btn.gameObject.SetActive(false);
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			this.redlabel.gameObject.SetActive(false);
			this.tipicon.gameObject.SetActive(false);
			JingLingMap.inst.mapmini.refreshBossBtn.gameObject.SetActive(false);
		}
		if (this.btn.gameObject.activeSelf)
		{
			this.bar.gameObject.SetActive(false);
			this.cdlabel.gameObject.SetActive(false);
			if (this.redlabel)
			{
				this.redlabel.gameObject.SetActive(false);
			}
		}
		else if (this.cdlabel.gameObject.activeSelf)
		{
			this.btn.gameObject.SetActive(false);
			if (this.redlabel)
			{
				this.redlabel.gameObject.SetActive(false);
			}
		}
		else if (this.title.gameObject.activeSelf && this.redlabel)
		{
			this.redlabel.gameObject.SetActive(false);
		}
	}

	public override void ResetState()
	{
		this.eState = JingLingMapBossFace.EState.NoServerData;
		if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.MyHome)
		{
			if (Global.Data == null || JingLingMap.inst.CurYaoSaiBossMainData == null)
			{
				return;
			}
			int num = 0;
			int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossCall", ',');
			if (systemParamIntArrayByName.Length >= 3)
			{
				num = systemParamIntArrayByName[2];
			}
			if (JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo == null || JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID <= 0)
			{
				this.eState = JingLingMapBossFace.EState.ZhaoHuaning;
				if (JingLingMap.inst.CurYaoSaiBossMainData.TaoFaCount >= num)
				{
					this.eState = JingLingMapBossFace.EState.NoTimes;
				}
			}
			else
			{
				this.eState = JingLingMapBossFace.EState.TaoFaing;
				if (JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID > 0 && JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.LifeV <= 0.0)
				{
					this.eState = JingLingMapBossFace.EState.LingJianging;
				}
				else if (JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID > 0 && JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.DeadTime <= Global.GetCorrectDateTime())
				{
					this.eState = JingLingMapBossFace.EState.TimeoutLingJianging;
				}
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.FriendHome)
		{
			if (Global.Data == null || JingLingMap.inst.CurYaoSaiBossMainData == null)
			{
				return;
			}
			int[] systemParamIntArrayByName2 = ConfigSystemParam.GetSystemParamIntArrayByName("ManorBossCall", ',');
			if (systemParamIntArrayByName2.Length >= 3)
			{
				int num2 = systemParamIntArrayByName2[2];
			}
			if (JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo == null || JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID <= 0)
			{
				this.eState = JingLingMapBossFace.EState.ZhaoHuaning;
				if (JingLingMap.inst.CurYaoSaiBossMainData.FightCount >= 1)
				{
					this.eState = JingLingMapBossFace.EState.NoTimes;
				}
			}
			else
			{
				this.eState = JingLingMapBossFace.EState.TaoFaing;
				if (JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID > 0 && JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.LifeV <= 0.0)
				{
					this.eState = JingLingMapBossFace.EState.LingJianging;
				}
				else if (JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.BossID > 0 && JingLingMap.inst.CurYaoSaiBossMainData.BossMiniInfo.DeadTime <= Global.GetCorrectDateTime())
				{
					this.eState = JingLingMapBossFace.EState.TimeoutLingJianging;
				}
			}
		}
		else if (JingLingMap.inst.showType == JingLingMap.JingLingMapType.NuLiSearch)
		{
		}
	}

	public override void UITimer_Tick(object sender, object e)
	{
		this.ResetState();
		this.UpdateUI();
	}

	public void UpdateGameObject()
	{
		bool flag = false;
		if (JingLingMap.IsInHome())
		{
		}
		if (this.eState == JingLingMapBossFace.EState.TaoFaing)
		{
			YaoSaiBossMainData curYaoSaiBossMainData = JingLingMap.inst.CurYaoSaiBossMainData;
			if (curYaoSaiBossMainData.BossMiniInfo != null && curYaoSaiBossMainData.BossMiniInfo.BossID > 0)
			{
				if (curYaoSaiBossMainData.OtherID == Global.Data.RoleID)
				{
					if (curYaoSaiBossMainData.BossMiniInfo != null && curYaoSaiBossMainData.BossMiniInfo.BossID > 0)
					{
						flag = true;
					}
				}
				else if (curYaoSaiBossMainData.BossMiniInfo != null && curYaoSaiBossMainData.BossMiniInfo.BossID > 0 && curYaoSaiBossMainData.BossMiniInfo.LifeV > 0.0)
				{
					flag = true;
				}
			}
			PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(curYaoSaiBossMainData.BossMiniInfo.BossID);
			MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(dataByID.MonsterID);
			this.preModelObjectName = this.curModelObjectName;
			this.curModelObjectName = string.Concat(new object[]
			{
				this.modelObjectName,
				this.nObjectIndex.ToString(),
				"_",
				dataByID.MonsterID
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
			if (flag)
			{
				if (!GameObject.Find(this.curModelObjectName) && this.modelObject == null)
				{
					if (monsterXmlNodeByID != null)
					{
						if (this.modelObject != null)
						{
							ObjectsManager.Remove(this.modelObject);
							this.modelObject = null;
						}
						float posX = 100f * JingLingMapObjectData.bossData.pos.x;
						float posY = 100f * JingLingMapObjectData.bossData.pos.z;
						this.modelObject = JingLingMap.inst.ForceCreateFadeNPC(monsterXmlNodeByID.ID, posX, posY, GSpriteTypes.Monster, monsterXmlNodeByID.ResName, this.curModelObjectName, 5);
					}
				}
			}
		}
		else if (this.eState == JingLingMapBossFace.EState.LingJianging)
		{
			if (JingLingMap.IsInHome())
			{
				if (this.eState == JingLingMapBossFace.EState.LingJianging)
				{
					flag = false;
				}
			}
		}
		base.RemoveModel(flag);
		if (this.eState != JingLingMapBossFace.EState.ZhaoHuaning)
		{
			if (this.eState != JingLingMapBossFace.EState.TaoFaing)
			{
				if (this.eState != JingLingMapBossFace.EState.LingJianging && this.eState != JingLingMapBossFace.EState.TimeoutLingJianging)
				{
					if (this.eState == JingLingMapBossFace.EState.NoTimes)
					{
					}
				}
			}
		}
	}

	private JingLingMapBossFace.EState eState;

	private enum EState
	{
		NoServerData,
		ZhaoHuaning,
		TaoFaing,
		LingJianging,
		TimeoutLingJianging,
		NoTimes
	}
}
