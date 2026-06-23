using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class ShiLiPartDiDui : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("敌对势力");
		this.lblDiDui1.text = Global.GetLang("选择【自由同盟】为敌对势力,在迷踪岛对该势力角色伤害提高50%");
		this.lblDiDui2.text = Global.GetLang("选择【自由同盟】为敌对势力,在迷踪岛对该势力角色伤害提高50%");
		this.lblDes.text = Global.GetLang("修改敌对势力会在次日0时生效");
		this.btnXiuGai.Text = Global.GetLang("修  改");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(this.goShiLi1).onClick = new UIEventListener.VoidDelegate(this.OnShiLi1Click);
		UIEventListener.Get(this.goShiLi2).onClick = new UIEventListener.VoidDelegate(this.OnShiLi2Click);
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnXiuGai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnConfigXiuGai();
		};
		this.btnXiuGai.gameObject.SetActive(false);
		this.InitData();
	}

	private void InitData()
	{
		switch (ShiLiData.GetSelfCompData().kfCompData.CompType)
		{
		case 1:
			this.m_type1 = ShiLiType.ZiYouTongMeng;
			this.m_type2 = ShiLiType.ZhiMengXieHui;
			break;
		case 2:
			this.m_type1 = ShiLiType.ShenShengJiaoTuan;
			this.m_type2 = ShiLiType.ZhiMengXieHui;
			break;
		case 3:
			this.m_type1 = ShiLiType.ShenShengJiaoTuan;
			this.m_type2 = ShiLiType.ZiYouTongMeng;
			break;
		}
		string compName = ShiLiData.GetMUCompById((int)this.m_type1).CompName;
		string compName2 = ShiLiData.GetMUCompById((int)this.m_type2).CompName;
		this.lblDiDui1.text = string.Format(Global.GetLang("选择【{0}】为敌对势力,在迷踪岛对该势力角色伤害提高{1}%"), compName, ShiLiData.GetCompEnemyHurtNum());
		this.lblDiDui2.text = string.Format(Global.GetLang("选择【{0}】为敌对势力,在迷踪岛对该势力角色伤害提高{1}%"), compName2, ShiLiData.GetCompEnemyHurtNum());
		this.imgShiLibg1.spriteName = this.GetSpriteByShiLiType(this.m_type1);
		this.imgShiLibg2.spriteName = this.GetSpriteByShiLiType(this.m_type2);
		if (this.m_type1 == (ShiLiType)ShiLiData.GetSelfCompData().kfCompData.EnemyCompType)
		{
			this.imgState1.gameObject.SetActive(true);
			this.imgState1.spriteName = "ShiLi_DQDD";
		}
		else if (this.m_type1 == (ShiLiType)ShiLiData.GetSelfCompData().kfCompData.EnemyCompTypeSet)
		{
			this.imgState1.gameObject.SetActive(true);
			this.imgState1.spriteName = "ShiLi_CRSX";
		}
		else
		{
			this.imgState1.gameObject.SetActive(false);
		}
		if (this.m_type2 == (ShiLiType)ShiLiData.GetSelfCompData().kfCompData.EnemyCompType)
		{
			this.imgState2.gameObject.SetActive(true);
			this.imgState2.spriteName = "ShiLi_DQDD";
		}
		else if (this.m_type2 == (ShiLiType)ShiLiData.GetSelfCompData().kfCompData.EnemyCompTypeSet)
		{
			this.imgState2.gameObject.SetActive(true);
			this.imgState2.spriteName = "ShiLi_CRSX";
		}
		else
		{
			this.imgState2.gameObject.SetActive(false);
		}
		this.m_canSetEnemy = false;
		MUCompLevel compLevelByID = ShiLiData.GetCompLevelByID((int)GameInstance.Game.CurrentSession.roleData.CompZhiWu);
		if (compLevelByID != null && compLevelByID.Enemy == 1)
		{
			this.m_canSetEnemy = true;
		}
		if (this.m_canSetEnemy)
		{
			this.btnXiuGai.gameObject.SetActive(true);
			this.imgSelect.gameObject.SetActive(true);
		}
		else
		{
			this.btnXiuGai.gameObject.SetActive(false);
			this.imgSelect.gameObject.SetActive(false);
		}
		if (this.m_selectType == ShiLiType.None)
		{
			this.OnShiLi1Click(this.goShiLi1);
		}
	}

	private void OnShiLi2Click(GameObject go)
	{
		if (!this.m_canSetEnemy)
		{
			return;
		}
		this.imgSelect.transform.localPosition = go.transform.localPosition;
		this.m_selectType = this.m_type2;
	}

	private void OnShiLi1Click(GameObject go)
	{
		if (!this.m_canSetEnemy)
		{
			return;
		}
		this.imgSelect.transform.localPosition = go.transform.localPosition;
		this.m_selectType = this.m_type1;
	}

	public string GetSpriteByShiLiType(ShiLiType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case ShiLiType.ShenShengJiaoTuan:
			result = "ShiLi_ShenShengJT";
			break;
		case ShiLiType.ZiYouTongMeng:
			result = "ShiLi_ZiYouTongMeng";
			break;
		case ShiLiType.ZhiMengXieHui:
			result = "ShiLi__ZhiMengXieHui";
			break;
		}
		return result;
	}

	private void OnConfigXiuGai()
	{
		this.SendSetEnemy();
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<int>("CMD_SPR_COMP_SET_ENEMY", new Action<int>(this.ServerSetEnemyReslut));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<int>("CMD_SPR_COMP_SET_ENEMY", new Action<int>(this.ServerSetEnemyReslut));
	}

	private void SendSetEnemy()
	{
		GameInstance.Game.ShiLiSetEnemy((int)this.m_selectType);
	}

	private void ServerSetEnemyReslut(int compType)
	{
		Super.HintMainText(Global.GetLang("设置成功"), 10, 3);
		this.InitData();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblDiDui1;

	public UILabel lblDiDui2;

	public UILabel lblDes;

	public UISprite imgShiLibg1;

	public UISprite imgState1;

	public UISprite imgShiLibg2;

	public UISprite imgState2;

	public UISprite imgSelect;

	public UISprite imgShiLiIcon1;

	public UISprite imgShiLiIcon2;

	public GButton btnClose;

	public GButton btnXiuGai;

	public GameObject goShiLi1;

	public GameObject goShiLi2;

	private ShiLiType m_type1;

	private ShiLiType m_type2;

	private ShiLiType m_selectType;

	private bool m_canSetEnemy;
}
