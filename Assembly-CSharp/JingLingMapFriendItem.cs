using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class JingLingMapFriendItem : UserControl
{
	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.nuyiBtn.Label.text = Global.GetLang(string.Empty);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.nuyiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		this.bossBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
	}

	public void UpdateUI()
	{
		this.nameLbl.text = this.relationData.RoleName;
		this.lvLbl.text = string.Concat(new string[]
		{
			"LV",
			this.relationData.Lv.ToString(),
			"[",
			this.relationData.ChangeLife.ToString(),
			Global.GetLang("转]")
		});
		if (this.relationData.YaoSaiBossState == 2)
		{
			this.bossSpr.spriteName = "guai";
			this.bossSpr.gameObject.SetActive(true);
		}
		else if (this.relationData.YaoSaiBossState == 3)
		{
			this.bossSpr.spriteName = "guaigray";
			this.bossSpr.gameObject.SetActive(true);
		}
		else
		{
			this.bossSpr.gameObject.SetActive(false);
		}
		if (this.relationData.YaoSaiJianYuState == 2)
		{
			this.nuyiSpr.spriteName = "nuyi";
			this.nuyiSpr.gameObject.SetActive(true);
		}
		else
		{
			this.nuyiSpr.gameObject.SetActive(false);
		}
	}

	public void OnItemClick()
	{
		if (this.selectSpr.gameObject.activeSelf)
		{
			return;
		}
		this.selectSpr.gameObject.SetActive(true);
		JingLingMap.inst.RequestChangeMap(this.relationData.RoleID);
	}

	internal void OnBossDataChange(YaoSaiBossData bossdata)
	{
		if (bossdata == null)
		{
			return;
		}
		this.relationData.bossdata = bossdata;
		if (this.relationData.friendData != null)
		{
			if (bossdata.LifeV <= 0.0)
			{
				this.relationData.friendData.YaoSaiBossState = 1;
			}
			else
			{
				DateTime deadTime = bossdata.DeadTime;
				if ((deadTime - Global.GetCorrectDateTime()).Ticks > 0L)
				{
					if (this.relationData.friendData.YaoSaiBossState <= 1)
					{
						this.relationData.friendData.YaoSaiBossState = 2;
					}
				}
				else
				{
					this.relationData.friendData.YaoSaiBossState = 1;
				}
			}
		}
		if (this.relationData.banghuiData != null)
		{
			if (bossdata.LifeV <= 0.0)
			{
				this.relationData.banghuiData.YaoSaiBossState = 1;
			}
			else
			{
				DateTime deadTime2 = bossdata.DeadTime;
				if ((deadTime2 - Global.GetCorrectDateTime()).Ticks > 0L)
				{
					if (this.relationData.banghuiData.YaoSaiBossState <= 1)
					{
						this.relationData.banghuiData.YaoSaiBossState = 2;
					}
				}
				else
				{
					this.relationData.banghuiData.YaoSaiBossState = 1;
				}
			}
		}
		this.UpdateUI();
	}

	public static JingLingMapFriendItem getItemByRoleID(int roleid)
	{
		if (JingLingMap.inst.mapmini != null)
		{
			for (int i = 0; i < JingLingMap.inst.mapmini.ItemCollection.Count; i++)
			{
				JingLingMapFriendItem jingLingMapFriendItem = U3DUtils.AS<JingLingMapFriendItem>(JingLingMap.inst.mapmini.ItemCollection[i]);
				if (jingLingMapFriendItem.relationData.RoleID == roleid)
				{
					return jingLingMapFriendItem;
				}
			}
		}
		return null;
	}

	public DPSelectedItemEventHandler DPSelectItem;

	public GButton nuyiBtn;

	public GButton bossBtn;

	public UISprite selectSpr;

	public UISprite nuyiSpr;

	public UISprite bossSpr;

	public UILabel nameLbl;

	public UILabel lvLbl;

	public JingLingRelationData relationData = new JingLingRelationData();

	private int id;
}
