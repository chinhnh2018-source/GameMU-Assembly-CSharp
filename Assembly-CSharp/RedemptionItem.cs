using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class RedemptionItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.redeemBtn.Text = Global.GetLang("立即兑换");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.redeemBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.redeemBtn.isEnabled)
			{
				return;
			}
			GameInstance.Game.SpriteFetchActivityAward(64, this.redemptionID);
		};
	}

	public int redemptionID
	{
		get
		{
			return this._redemptionID;
		}
		set
		{
			this._redemptionID = value;
		}
	}

	public int points
	{
		get
		{
			return this._points;
		}
		set
		{
			this._points = value;
			this.pointsLabel.Text = string.Format("{0}{1}", this._points, Global.GetLang("点数"));
		}
	}

	public int leftRedeemTimes
	{
		get
		{
			return this._leftRedeemTimes;
		}
		set
		{
			this._leftRedeemTimes = value;
			this.leftRedeemTimesLabel.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"00fe30",
				string.Format(Global.GetLang("剩余次数:{0}"), this._leftRedeemTimes)
			});
			if (this._leftRedeemTimes <= 0)
			{
				this.leftRedeemTimesLabel.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"00fe30",
					Global.GetLang("剩余次数:0")
				});
				this.redeemBtn.isEnabled = false;
			}
		}
	}

	public string goodsIDs
	{
		get
		{
			return this._goodsIDs;
		}
		set
		{
			this._goodsIDs = value;
			this.LoadGoodsList(this._goodsIDs, false);
		}
	}

	private void LoadGoodsList(string goodsStr, bool isOcc = false)
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
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				','
			});
			if (array2.Length == 7)
			{
				if (!isOcc || !RedemptionItem.IsRoleOccupation(array2[0], roleOcc))
				{
					GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(Convert.ToInt32(array2[0]), Convert.ToInt32(array2[3]), Convert.ToInt32(array2[4]), Convert.ToInt32(array2[6]), Convert.ToInt32(array2[5]), Convert.ToInt32(array2[2]), Convert.ToInt32(array2[1]), 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
					this.AddGoodsIcon(dummyGoodsDataMu, false);
				}
			}
		}
	}

	public static bool IsRoleOccupation(string goodStr, int roleOcc)
	{
		if (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && ConfigSystemParam.GetSystemParamStringArrayByName("LiMJSDaTianShi", ',').IndexOf(goodStr) != -1)
		{
			return false;
		}
		if (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && ConfigSystemParam.GetSystemParamStringArrayByName("ZhiMJSDaTianShi", ',').IndexOf(goodStr) != -1)
		{
			return false;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(Convert.ToInt32(goodStr));
		return goodsXmlNodeByID == null || goodsXmlNodeByID.MainOccupation != roleOcc || (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Magic_Sword && goodsXmlNodeByID.Intelligence < goodsXmlNodeByID.Strength) || (roleOcc == 3 && Global.GetMJSTypeByAttr() == MJSSkillType.Strength_Sword && goodsXmlNodeByID.Intelligence > goodsXmlNodeByID.Strength);
	}

	private void AddGoodsIcon(GoodsData gd, bool grayShow = false)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = gd.GoodsID;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.BoxTypes = -1;
			if (!grayShow)
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			}
			else
			{
				ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, true, 0);
			}
			bool canUse = Global.CanUseGoods(gd.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, gd, canUse, IconTextTypes.Qianghua);
			ObservableCollection itemsSource = this.goodList.ItemsSource;
			itemsSource.Add(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			MUJierihuodongPart.SetGoodsIconBoxCollider(ggoodIcon);
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
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

	public ListBox goodList;

	public GButton redeemBtn;

	public TextBlock pointsLabel;

	public TextBlock leftRedeemTimesLabel;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int _redemptionID;

	private int _points;

	private int _leftRedeemTimes;

	private string _goodsIDs = string.Empty;
}
