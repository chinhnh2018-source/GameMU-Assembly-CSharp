using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZuduiFubtnItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblNeedLevelStatic.text = Global.GetLang("需要等级:");
		this.lblFinishNumStatic.text = Global.GetLang("完成次数:");
		this.m_lblZhanLiStatic.text = Global.GetLang("推荐战力:");
		this.lblNeedLevel.transform.localPosition = new Vector3(-90f, 68f, -1f);
		this.lblFinishNum.transform.localPosition = new Vector3(-79f, 43f, -1f);
		this.lblFinishNumLimit.transform.localPosition = new Vector3(-79f, 43f, -1f);
		this.m_lblZhanLi.transform.localPosition = new Vector3(-40f, 29f, -1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public bool ShowFanbei
	{
		get
		{
			return this.showFanbei;
		}
		set
		{
			this.showFanbei = value;
			if (this.ShowFanbei)
			{
				FanbeiPrefab fanbeiPrefab = U3DUtils.NEW<FanbeiPrefab>();
				fanbeiPrefab.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/RewartDouble.png";
				this.obj.Add(fanbeiPrefab);
				this.obj.gameObject.SetActive(this.ShowFanbei);
			}
		}
	}

	public bool CanSelect
	{
		get
		{
			return this.canSelect;
		}
		set
		{
			this.canSelect = value;
		}
	}

	public int FubenID
	{
		get
		{
			return this._fubenID;
		}
		set
		{
			this._fubenID = value;
			if (this._fubenID == 4000)
			{
				this.btnHelp.gameObject.SetActive(false);
			}
		}
	}

	public string NeedLevel
	{
		get
		{
			return this.lblNeedLevel.text;
		}
		set
		{
			this.lblNeedLevel.text = value;
		}
	}

	public string FinishNum
	{
		get
		{
			return this.lblFinishNum.text;
		}
		set
		{
			this.lblFinishNum.text = value;
		}
	}

	public string FinishNumLimit
	{
		get
		{
			return this.lblFinishNumLimit.text;
		}
		set
		{
			this.lblFinishNumLimit.text = value;
		}
	}

	public string RewardExp
	{
		get
		{
			return this.lblRewardExp.text;
		}
		set
		{
			this.lblRewardExp.text = value;
			string[] goods = ("8002," + value + ",0,0,0,0,0").Split(new char[]
			{
				','
			});
			this.initGood(goods, 0);
		}
	}

	public string RewardMoney
	{
		get
		{
			return this.lblRewardMoney.text;
		}
		set
		{
			this.lblRewardMoney.text = value;
			string[] goods = ("8014," + value + ",0,0,0,0,0").Split(new char[]
			{
				','
			});
			this.initGood(goods, 1);
		}
	}

	public int RewardFenMo
	{
		get
		{
			return this.rewardFenMo;
		}
		set
		{
			this.rewardFenMo = value;
			if (value != -1)
			{
				string[] goods = ("8018," + value.ToString() + ",0,0,0,0,0").Split(new char[]
				{
					','
				});
				this.initGood(goods, 2);
				this._goodsIndex = 3;
			}
			else
			{
				this._goodsIndex = 2;
			}
		}
	}

	public int YingGuangaward
	{
		get
		{
			return this.yingGuangaward;
		}
		set
		{
			this.yingGuangaward = value;
			if (value != -1)
			{
				string[] goods = ("8018," + value.ToString() + ",0,0,0,0,0").Split(new char[]
				{
					','
				});
				this.initGood(goods, 2);
				this._goodsIndex = 4;
			}
			else
			{
				this._goodsIndex = 3;
			}
		}
	}

	public int LangHunaward
	{
		get
		{
			return this.langHunaward;
		}
		set
		{
			this.langHunaward = value;
			if (value != -1)
			{
				string[] goods = ("8030," + value.ToString() + ",0,0,0,0,0").Split(new char[]
				{
					','
				});
				this.initGood(goods, 2);
				this._goodsIndex = 4;
			}
			else
			{
				this._goodsIndex = 3;
			}
		}
	}

	public string RewardGoods
	{
		get
		{
			return this._rewardGoodIDs;
		}
		set
		{
			this._rewardGoodIDs = value;
			string text = value + "|";
			string[] array = text.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length > 0)
				{
					string[] goods = array[i].Split(new char[]
					{
						','
					});
					this.initGood(goods, i + this._goodsIndex);
				}
			}
		}
	}

	public void Setbak(string bakName)
	{
		this.bak.spriteName = bakName;
	}

	public void SetBakUrl(string name)
	{
		this.bakUrl.URL = this.GetImageUrlString(name);
	}

	private string GetImageUrlString(string name)
	{
		return string.Format("NetImages/GameRes/Images/Preview/{0}.png", name);
	}

	public void initGood(string[] goods, int idx)
	{
		int num = Convert.ToInt32(goods[0]);
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(num);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			GoodsData goodsData = new GoodsData();
			goodsData.GoodsID = num;
			goodsData.GCount = Convert.ToInt32(goods[1]);
			goodsData.Binding = Convert.ToInt32(goods[2]);
			goodsData.Forge_level = Convert.ToInt32(goods[3]);
			goodsData.AppendPropLev = Convert.ToInt32(goods[4]);
			goodsData.Lucky = Convert.ToInt32(goods[5]);
			goodsData.ExcellenceInfo = Convert.ToInt32(goods[6]);
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.ItemCode = num;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			ggoodIcon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon2 = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon2)
				{
					return;
				}
				GoodsData goodsData2 = ggoodIcon2.ItemObject as GoodsData;
				if (goodsData2 == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(ggoodIcon2, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData2);
			});
			BoxCollider component = ggoodIcon.transform.GetComponent<BoxCollider>();
			Object.Destroy(component);
			UIPanel component2 = ggoodIcon.GetComponent<UIPanel>();
			if (component2)
			{
				Object.Destroy(component2);
			}
			ggoodIcon.transform.localPosition = new Vector3(158f - 76f * (float)idx, -68f, 0f);
			U3DUtils.AddChild(base.gameObject, ggoodIcon.gameObject, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, Global.CanUseGoods(num, false, true), IconTextTypes.Qianghua);
		}
	}

	public ShowNetImage bakUrl;

	public UILabel lblNeedLevel;

	public UILabel lblNeedLevelStatic;

	public UILabel lblFinishNum;

	public UILabel lblFinishNumStatic;

	public UILabel lblFinishNumLimit;

	public UILabel lblRewardExp;

	public UILabel lblRewardMoney;

	public UIButton btnHelp;

	public UILabel m_lblZhanLiStatic;

	public UILabel m_lblZhanLi;

	public UISprite bak;

	public SpriteSL obj;

	private int _fubenID;

	private string _rewardGoodIDs = string.Empty;

	private int _goodsIndex = 2;

	private bool showFanbei;

	public int Type;

	private bool canSelect = true;

	private int rewardFenMo = -1;

	private int yingGuangaward = -1;

	private int langHunaward = -1;
}
