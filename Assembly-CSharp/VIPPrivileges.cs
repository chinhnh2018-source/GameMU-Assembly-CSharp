using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class VIPPrivileges : UserControl
{
	private void InitTextInPrefabs()
	{
		this.privilege_Description.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffe485",
			Global.GetLang("领取VIP用户认证奖励")
		});
		this.rewards_Description.Text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffe485",
			Global.GetLang("VIP用户认证奖励")
		});
		this.QRCode_Description.Text = Global.GetLang("VIP-QQ客服群");
		this.QRCode_Join_Description.Text = Global.GetLang("扫描或点击二维码申请加入");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.itemCollection = this.rewardsList.ItemsSource;
		this.ReadVIPPrivilegeConfig();
		UIEventListener.Get(this.QRCode).onClick = delegate(GameObject s)
		{
			Application.OpenURL(this.qrcodeURL);
		};
	}

	public string GoodsIDs
	{
		get
		{
			return this._goodsIDs;
		}
		set
		{
			this._goodsIDs = value;
			this.LoadGoodsList(this.GoodsIDs);
		}
	}

	private void ReadVIPPrivilegeConfig()
	{
		this.ReadMinDeposit();
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("VIPKeFuIntro", true);
		this.privilege_Description.Text = systemParamByName;
		string systemParamByName2 = ConfigSystemParam.GetSystemParamByName("VIPKeFuAward", true);
		this.qrcodeURL = ConfigSystemParam.GetSystemParamByName("VIPKeFuUrl", true);
		if (!string.IsNullOrEmpty(systemParamByName2))
		{
			this.LoadGoodsList(systemParamByName2);
		}
	}

	private void ReadMinDeposit()
	{
		string name = string.Empty;
		switch (Global.DevicePlatform())
		{
		case AppPlatform.Default:
		case AppPlatform.Android:
			name = "VIPKeFu_Android";
			break;
		case AppPlatform.IOS:
			name = "VIPKeFu_APP";
			break;
		case AppPlatform.IOS_Jailbreak:
			name = "VIPKeFu_YueYu";
			break;
		}
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName(name, ',');
		if (systemParamStringArrayByName != null && systemParamStringArrayByName.Length >= 2)
		{
			this.minDeposit.Text = systemParamStringArrayByName[1];
		}
	}

	private void LoadGoodsList(string goodsIDs)
	{
		this.itemCollection.Clear();
		if (!string.IsNullOrEmpty(goodsIDs))
		{
			string[] array = goodsIDs.Split(new char[]
			{
				'@'
			});
			if (array.Length == 1)
			{
				this.LoadOtherRewardGoodsList(goodsIDs, false);
			}
			else
			{
				this.LoadOtherRewardGoodsList(array[0], false);
				this.LoadOtherRewardGoodsList(array[1], true);
			}
		}
	}

	private void LoadOtherRewardGoodsList(string goodsStr, bool isOccupation = false)
	{
		string text = StringUtil.trim(goodsStr);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new char[]
		{
			'|'
		});
		if (array.Length <= 0)
		{
			return;
		}
		foreach (string goodsStr2 in array)
		{
			GGoodIcon ggoodIcon = VIPPrivileges.LoadRewardItemGoodsIcon(goodsStr2, isOccupation, false, true);
			if (!(null == ggoodIcon))
			{
				this.itemCollection.Add(ggoodIcon);
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
				ggoodIcon.addEventListener("click", new MouseEventHandler(VIPPrivileges.MouseLeftButtonUp));
			}
		}
		this.itemCollection.DelayUpdate();
	}

	public static GGoodIcon LoadRewardItemGoodsIcon(string goodsStr, bool isOccupation = false, bool autoListen = true, bool activeBackground = true)
	{
		if (string.IsNullOrEmpty(goodsStr))
		{
			return null;
		}
		int num = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		string[] array = goodsStr.Split(new char[]
		{
			','
		});
		if (array.Length != 7)
		{
			return null;
		}
		if (isOccupation)
		{
			int nGoodsID = Convert.ToInt32(array[0]);
			if (!VIPPrivileges.IsGoodsToOccupation(nGoodsID))
			{
				return null;
			}
		}
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array[0]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[6]), Convert.ToInt32(array[5]), Convert.ToInt32(array[2]), Convert.ToInt32(array[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon = VIPPrivileges.CreateGoodsIcon(dummyGoodsDataMu, false, true);
		if (autoListen && null != ggoodIcon)
		{
			ggoodIcon.addEventListener("click", new MouseEventHandler(VIPPrivileges.MouseLeftButtonUp));
		}
		return ggoodIcon;
	}

	private static bool IsGoodsToOccupation(int nGoodsID)
	{
		if (0 >= nGoodsID)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nGoodsID);
		int mainOccupation = goodsXmlNodeByID.MainOccupation;
		return mainOccupation == -1 || (mainOccupation == Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) && (mainOccupation != 3 || (Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && goodsXmlNodeByID.Strength > goodsXmlNodeByID.Intelligence) || (Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && goodsXmlNodeByID.Intelligence > goodsXmlNodeByID.Strength)));
	}

	private GGoodIcon AddGoodsIcon(GoodsData goodData, bool grayShow = false)
	{
		GGoodIcon ggoodIcon = VIPPrivileges.CreateGoodsIcon(goodData, grayShow, true);
		if (null != ggoodIcon)
		{
			this.itemCollection.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(VIPPrivileges.MouseLeftButtonUp));
		}
		return ggoodIcon;
	}

	public static GGoodIcon CreateGoodsIcon(GoodsData goodData, bool grayShow = false, bool activeBackground = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodData.GoodsID);
		if (goodsXmlNodeByID == null)
		{
			return null;
		}
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
		string text = "bagGrid4_bak";
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BackSpriteName0 = ((!activeBackground) ? null : text);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
		ggoodIcon.ItemCode = goodData.GoodsID;
		ggoodIcon.ItemObject = goodData;
		ggoodIcon.BoxTypes = -1;
		if (!grayShow)
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
		}
		else
		{
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
		}
		bool canUse = Global.CanUseGoods(goodData.GoodsID, false, true);
		Super.InitGoodsGIcon(ggoodIcon, goodData, canUse, IconTextTypes.Qianghua);
		return ggoodIcon;
	}

	private static void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	private const string fontColor_rewards = "e3b36c";

	private const string fontColor_signPrivilege = "ffe485";

	public ListBox rewardsList;

	public TextBlock minDeposit;

	public TextBlock privilege_Description;

	public TextBlock rewards_Description;

	public TextBlock QRCode_Description;

	public TextBlock QRCode_Join_Description;

	public GameObject QRCode;

	private ObservableCollection itemCollection;

	private string qrcodeURL = string.Empty;

	private string _goodsIDs = string.Empty;
}
