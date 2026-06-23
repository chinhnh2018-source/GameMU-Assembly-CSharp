using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenQiSystemPart : UserControl
{
	protected override void InitializeComponent()
	{
		GameInstance.Game.SendGetShenQiDataRequest();
		this.InitTextInPrefabs();
		this.InitButton();
		this.InitTextBlock();
	}

	private void InitTextInPrefabs()
	{
		this.m_ShenQiBtn.Text = Global.GetLang("神器");
		this.m_ShenXiangBtn.Text = Global.GetLang("神像");
	}

	private void InitButton()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.m_Tabs.TabClick += delegate(GameObject s, int index)
		{
			if (index == this.m_CurrentTabIndex)
			{
				return;
			}
			this.ShowContent(index);
		};
	}

	private void Start1()
	{
		this.m_Tabs.TabIndex = 1;
		this.m_CurrentTabIndex = 1;
	}

	public void InitValue(ShenQiData shenQiData)
	{
		if (shenQiData == null)
		{
			return;
		}
		this.m_CurrentShenQiID = shenQiData.ShenQiID;
		float num = (float)(shenQiData.LifeAdd + shenQiData.DefenseAdd + shenQiData.ToughnessAdd + shenQiData.AttackAdd);
		ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(this.m_CurrentShenQiID);
		float num2 = (float)(shenQiDataByID.LifeV + shenQiDataByID.AddDefense + shenQiDataByID.Toughness + shenQiDataByID.AddAttack);
		this.isManJi = (num2 == num);
		Global.Data.ShenLiJingHuaCount = shenQiData.ShenLiJingHuaLeft;
		this.m_JingHua.Text = Global.Data.ShenLiJingHuaCount.ToString();
		ShenQiManager.InitCurrentShenQiDict(shenQiData, this.isManJi);
		this.Start1();
		this.isFirst = false;
		if (null != this.m_ShenQiContentPart)
		{
			Object.Destroy(this.m_ShenQiContentPart);
			this.m_ShenQiContentPart = null;
		}
		if (null == this.m_ShenQiContentPart)
		{
			this.m_ShenQiContentPart = U3DUtils.NEW<ShenQiContentPart>();
			U3DUtils.AddChild(this.m_Parent, this.m_ShenQiContentPart.gameObject, true);
			this.m_ShenQiContentPart.CloseCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				if (this.CloseHandler != null)
				{
					this.CloseHandler(null, new DPSelectedItemEventArgs
					{
						IDType = 0
					});
				}
			};
			this.m_ShenQiContentPart.RefreshJingHuaCallback = delegate(object s, DPSelectedItemEventArgs e)
			{
				Global.Data.ShenLiJingHuaCount = e.ID;
				this.m_JingHua.Text = Global.Data.ShenLiJingHuaCount.ToString();
				this.InitTextBlock();
			};
		}
		this.m_ShenQiContentPart.InitValue(this.m_CurrentShenQiID, this.isManJi);
	}

	public void InitTextBlock()
	{
		this.m_Diamond.Text = Global.Data.roleData.UserMoney.ToString();
		this.m_JinBi.Text = Global.GetRoleOwnNumByMoneyType(8).ToString();
	}

	private void ShowContent(int index)
	{
		this.m_CurrentTabIndex = index;
		if (index != 1)
		{
			if (index == 2)
			{
				if (null == this.m_ShenXiangContentPart)
				{
					this.m_ShenXiangContentPart = U3DUtils.NEW<ShenXiangContentPart>();
					U3DUtils.AddChild(this.m_Parent, this.m_ShenXiangContentPart.gameObject, true);
					this.m_ShenXiangContentPart.Visibility = false;
				}
			}
		}
		else if (!this.isFirst)
		{
			GameInstance.Game.SendGetShenQiDataRequest();
		}
		this.ShowCurrentContent(index);
	}

	private void ShowCurrentContent(int index)
	{
		if (null != this.m_ShenQiContentPart)
		{
			Object.Destroy(this.m_ShenQiContentPart.gameObject);
			this.m_ShenQiContentPart = null;
		}
		if (null != this.m_ShenXiangContentPart)
		{
			bool flag = index == 2;
			this.m_ShenXiangContentPart.Visibility = flag;
			if (flag)
			{
				this.m_ShenXiangContentPart.InitValue();
				this.m_ShenXiangContentPart.ResetPanel();
			}
		}
	}

	protected override void OnDestroy()
	{
		ShenQiManager.Clear();
		ShenXiangManager.Clear();
		ShenQiPropertyManager.Clear();
		this.CloseHandler = null;
		this.m_CloseBtn = null;
		this.m_ShenQiBtn = null;
		this.m_ShenXiangBtn = null;
		this.m_Diamond = null;
		this.m_JinBi = null;
		this.m_JingHua = null;
		this.m_Tabs = null;
		this.m_CurrentTabIndex = 0;
		this.m_Parent = null;
		this.m_ShenQiContentPart = null;
		this.m_ShenXiangContentPart = null;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton m_CloseBtn;

	public GButton m_ShenQiBtn;

	public GButton m_ShenXiangBtn;

	public TextBlock m_Diamond;

	public TextBlock m_JinBi;

	public TextBlock m_JingHua;

	public UITab m_Tabs;

	public int m_CurrentTabIndex;

	public GameObject m_Parent;

	[HideInInspector]
	public ShenQiContentPart m_ShenQiContentPart;

	[HideInInspector]
	public ShenXiangContentPart m_ShenXiangContentPart;

	private bool isFirst = true;

	private int m_CurrentShenQiID;

	private bool isManJi;

	private enum TabIndex
	{
		ShenQi = 1,
		ShenXiang
	}
}
