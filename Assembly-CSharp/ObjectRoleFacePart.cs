using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ObjectRoleFacePart : UserControl
{
	public bool IsSelectedReborn
	{
		get
		{
			return this.m_isSelectedReborn;
		}
	}

	private void InitTextInPrefabs()
	{
		this.jiaoyiBtn.Text = Global.GetLang("交 易");
		this.siliaoBtn.Text = Global.GetLang("私 聊");
		this.zuduiBtn.Text = Global.GetLang("邀请组队");
		this.chakanBtn.Text = Global.GetLang("查看装备");
		this.chakanRebornBtn.Text = Global.GetLang("查看重生");
		this.tianFuBtn.Text = Global.GetLang("查看天赋");
		this.marryMeBtn.Text = Global.GetLang("向TA求婚");
		this.wishBtn.Text = Global.GetLang("送TA祝福");
		this.OtherHorseBtn.Text = Global.GetLang("查看坐骑");
		this.mBtnKauFuTeamCompete.Text = Global.GetLang("邀请战队");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.tw.gameObject.SetActive(false);
		UIEventListener.Get(this.RolePhotoIcon.gameObject).onClick = delegate(GameObject s)
		{
			if ((Global.IsKuaFuHuoDongMapSceneUIClass(Global.GetMapSceneUIClass()) || Global.GetMapSceneUIClass() == SceneUIClasses.LangHunLingYu || Global.GetMapSceneUIClass() == SceneUIClasses.JingJiChang) && !SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 8
				});
			}
			if (this.ObjectFaceType == GSpriteTypes.Other)
			{
				if (Global.IsChongShengOpen())
				{
					this.RePosition();
				}
				this.tw.gameObject.SetActive(true);
				if (null != this.tw)
				{
					if (this.isTeam)
					{
						this.zuduiBtn.Label.text = Global.GetLang("申请组队");
					}
					else
					{
						this.zuduiBtn.Label.text = Global.GetLang("邀请组队");
					}
					if (this.istw)
					{
						this.RolePhotoIcon.GetComponent<BoxCollider>().size = new Vector3(150f, 65f, 0f);
					}
					else
					{
						if (Global.Data.CurrentTeamData == null)
						{
						}
						this.RolePhotoIcon.GetComponent<BoxCollider>().size = new Vector3(2000f, 2000f, -104f);
						this.tw.from = new Vector3(0f, 350f);
						this.tw.to = new Vector3(0f, -3f);
					}
					this.istw = !this.istw;
					this.tw.Play(this.istw);
				}
			}
			else if (this.ObjectFaceType == GSpriteTypes.FakeRole)
			{
				this.tw.gameObject.SetActive(false);
				this.DPSelectedItemPart(100);
			}
		};
		this.OtherHorseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (SceneUIClasses.RebornMap.IsTheScene())
			{
				Super.HintMainText(Global.GetLang("当前不能进行此操作"), 10, 3);
				return;
			}
			GoodsData goodsData = null;
			RoleData roleData = null;
			if (Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData))
			{
				if (roleData.MountEquipList != null)
				{
					goodsData = roleData.MountEquipList.Find((GoodsData f) => f.Using == 1 && 340 == Global.GetCategoriyByGoodsID(f.GoodsID));
				}
				if (goodsData == null)
				{
					Super.HintMainText(Global.GetLang("目标角色暂时没有出战坐骑"), 10, 3);
				}
				else
				{
					goodsData = goodsData.Clone();
					goodsData.Strong = 1;
					if (roleData.ZuoQiMainData != null)
					{
						goodsData.Strong = roleData.ZuoQiMainData.MountLevel + 1;
					}
					GGoodIcon newGoodIcon = Global.GetNewGoodIcon();
					newGoodIcon.Width = 80.0;
					newGoodIcon.Height = 80.0;
					newGoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
					{
						Global.GetGoodsIconCodeByID(goodsData.GoodsID)
					}), false, 0);
					newGoodIcon.TipType = 1;
					newGoodIcon.ItemCategory = Global.GetCategoriyByGoodsID(goodsData.GoodsID);
					newGoodIcon.ItemCode = goodsData.GoodsID;
					newGoodIcon.ItemObject = goodsData;
					newGoodIcon.BoxTypes = 14;
					newGoodIcon.gameObject.AddComponent<UIDragPanelContents>();
					GTipServiceEx.ShowTip(newGoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.OtherRole, goodsData);
				}
			}
			else
			{
				Super.HintMainText(Global.GetLang("目标角色已离线或不在同一服务器"), 10, 3);
			}
		};
		this.chakanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_isSelectedReborn = false;
			this.DPSelectedItemPart(0);
		};
		this.chakanRebornBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.m_isSelectedReborn = true;
			this.DPSelectedItemPart(0);
		};
		this.siliaoBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItemPart(1);
		};
		this.jiaoyiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItemPart(2);
		};
		this.zuduiBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isTeam)
			{
				this.DPSelectedItemPart(3);
			}
			else
			{
				this.DPSelectedItemPart(4);
			}
		};
		this.marryMeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItemPart(5);
		};
		this.wishBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItemPart(6);
		};
		this.tianFuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItemPart(7);
		};
		this.mBtnKauFuTeamCompete.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItemPart(9);
		};
		this.RePosition();
	}

	protected void RePosition()
	{
		RoleData roleData = null;
		Global.Data.OtherRoles.TryGetValue(this.RoleID, ref roleData);
		float num = this.chakanBtn.transform.localPosition.y;
		if (this.chakanBtn.gameObject.activeSelf)
		{
			num -= 30f;
		}
		if (roleData != null && roleData.RebornCount > 0)
		{
			this.chakanRebornBtn.gameObject.SetActive(true);
			this.chakanRebornBtn.transform.localPosition = new Vector3(this.chakanRebornBtn.transform.localPosition.x, num, this.chakanRebornBtn.transform.localPosition.z);
			num -= 30f;
		}
		else
		{
			this.chakanRebornBtn.gameObject.SetActive(false);
		}
		if (this.tianFuBtn.gameObject.activeSelf)
		{
			this.tianFuBtn.transform.localPosition = new Vector3(this.tianFuBtn.transform.localPosition.x, num, this.tianFuBtn.transform.localPosition.z);
			num -= 30f;
		}
		if (this.OtherHorseBtn.gameObject.activeSelf)
		{
			this.OtherHorseBtn.transform.localPosition = new Vector3(this.OtherHorseBtn.transform.localPosition.x, num, this.OtherHorseBtn.transform.localPosition.z);
			num -= 30f;
		}
		if (this.siliaoBtn.gameObject.activeSelf)
		{
			this.siliaoBtn.transform.localPosition = new Vector3(this.siliaoBtn.transform.localPosition.x, num, this.siliaoBtn.transform.localPosition.z);
			num -= 30f;
		}
		if (this.zuduiBtn.gameObject.activeSelf)
		{
			this.zuduiBtn.transform.localPosition = new Vector3(this.zuduiBtn.transform.localPosition.x, num, this.zuduiBtn.transform.localPosition.z);
			num -= 30f;
		}
		if (this.jiaoyiBtn.gameObject.activeSelf)
		{
			this.jiaoyiBtn.transform.localPosition = new Vector3(this.jiaoyiBtn.transform.localPosition.x, num, this.jiaoyiBtn.transform.localPosition.z);
			num -= 30f;
		}
		if (!ConfigVersionSystemOpen.IsVersionSystemOpen(100108) || !GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.KuaFuTeamCompete))
		{
			this.mBtnKauFuTeamCompete.gameObject.SetActive(false);
		}
		else
		{
			this.mBtnKauFuTeamCompete.gameObject.SetActive(true);
			if (this.mBtnKauFuTeamCompete.gameObject.activeSelf)
			{
				this.mBtnKauFuTeamCompete.transform.localPosition = new Vector3(this.mBtnKauFuTeamCompete.transform.localPosition.x, num, this.mBtnKauFuTeamCompete.transform.localPosition.z);
				num -= 30f;
			}
		}
		if (this.marryMeBtn.gameObject.activeSelf)
		{
			this.marryMeBtn.transform.localPosition = new Vector3(this.marryMeBtn.transform.localPosition.x, num, this.marryMeBtn.transform.localPosition.z);
			num -= 30f;
		}
		if (this.wishBtn.gameObject.activeSelf)
		{
			this.wishBtn.transform.localPosition = new Vector3(this.wishBtn.transform.localPosition.x, num, this.wishBtn.transform.localPosition.z);
			num -= 30f;
		}
	}

	private void DPSelectedItemPart(int index)
	{
		this.RolePhotoIcon.GetComponent<BoxCollider>().size = new Vector3(150f, 65f, 0f);
		this.istw = !this.istw;
		this.tw.Play(this.istw);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = index
			});
		}
	}

	public bool BodyVisible
	{
		get
		{
			return this.Body.Visibility;
		}
		set
		{
			this.Body.Visibility = value;
		}
	}

	public int RoleID
	{
		get
		{
			return this._RoleID;
		}
		set
		{
			this._RoleID = value;
			GSprite gsprite = Global.FindSpriteByID(this._RoleID);
			if (gsprite != null && (gsprite.SpriteType == GSpriteTypes.Other || gsprite.SpriteType == GSpriteTypes.FakeRole))
			{
				if (0L >= gsprite.VArmorMax)
				{
					this.RefreshLeaderArmor(0f);
				}
				else
				{
					this.RefreshLeaderArmor(Mathf.Clamp01((float)gsprite.VArmor / (float)gsprite.VArmorMax));
				}
			}
		}
	}

	public string VSName
	{
		get
		{
			return this._VSName;
		}
		set
		{
			this._VSName = value;
		}
	}

	public string ShowName
	{
		get
		{
			return this.RoleName.Text;
		}
		set
		{
			this.RoleName.Text = value;
			int num = (int)(this.RoleName.Label.relativeSize.x * this.RoleName.Label.transform.localScale.x) + 15;
			this.RoleLevel.transform.localPosition = this.RoleName.Label.transform.localPosition + new Vector3((float)num, 0f, 0f);
		}
	}

	public int SpriteType
	{
		get
		{
			return this._SpriteType;
		}
		set
		{
			this._SpriteType = value;
		}
	}

	public string RolePhotoUrl
	{
		set
		{
			this.RolePhoto.URL = value;
		}
	}

	public string VLevel
	{
		set
		{
			this.RoleLevel.Text = value;
		}
	}

	public double LifePercent
	{
		set
		{
			this.HPBar.Percent = value;
		}
	}

	public double MagicPercent
	{
		set
		{
			this.MPBar.Percent = value;
		}
	}

	public string LifeText
	{
		set
		{
			this.HPBar.ProgessText = value;
		}
	}

	public string LifeTip
	{
		get
		{
			return this._LifeTip;
		}
		set
		{
			this._LifeTip = value;
		}
	}

	public string MagicText
	{
		set
		{
			this.MPBar.ProgessText = value;
		}
	}

	public string MagicTip
	{
		get
		{
			return this._MagicTip;
		}
		set
		{
			this._MagicTip = value;
		}
	}

	public double BackWidth
	{
		get
		{
			return this.Container.Width;
		}
	}

	public object ItemObject
	{
		get
		{
			return this._ItemObject;
		}
		set
		{
			this._ItemObject = value;
		}
	}

	public Canvas RootCanvas
	{
		get
		{
			return this.Container;
		}
	}

	public bool TeamMode
	{
		get
		{
			return this._TeamMode;
		}
		set
		{
			this._TeamMode = value;
		}
	}

	public string StallName
	{
		get
		{
			return this._StallName;
		}
		set
		{
			this._StallName = value;
		}
	}

	internal void RefreshLeaderArmor(float value)
	{
		value = Mathf.Clamp01(value);
		this._RoleArmorProgressBarSP.fillAmount = 0.5f * value + 0.5f;
	}

	public const int OtherObjBtnsYH = 30;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UIButton RolePhotoIcon;

	public ShowNetImage RolePhoto;

	public TweenPosition tw;

	public TextBlock RoleLevel;

	public TextBlock RoleName;

	public SpriteSL Body;

	public GImgProgressBar HPBar;

	public GImgProgressBar MPBar;

	public GSpriteTypes ObjectFaceType;

	private bool istw;

	public GButton chakanBtn;

	public GButton chakanRebornBtn;

	public GButton tianFuBtn;

	public GButton siliaoBtn;

	public GButton jiaoyiBtn;

	public GButton zuduiBtn;

	public GButton marryMeBtn;

	public GButton wishBtn;

	public GButton OtherHorseBtn;

	public GButton mBtnKauFuTeamCompete;

	public UISprite _RoleArmorProgressBarSP;

	public bool isTeam;

	private bool m_isSelectedReborn;

	private int _RoleID;

	private string _VSName;

	private int _SpriteType;

	private string _LifeTip = string.Empty;

	private string _MagicTip = string.Empty;

	private object _ItemObject;

	private bool _TeamMode;

	private string _StallName = string.Empty;
}
