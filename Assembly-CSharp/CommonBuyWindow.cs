using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CommonBuyWindow : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblName.text = Global.GetLang("提示");
		this.lblLevelWord.text = Global.GetLang("使用等级: ");
		this.lblLevel.text = Global.GetLang("20级");
		this.lblLimitWord.text = Global.GetLang("适用职业: ");
		this.lblBtnOK.text = Global.GetLang("购买");
		this.lblLimit.text = Global.GetLang("通用");
		this.lblType.text = Global.GetLang("使用等级: ");
		this.lblDes.text = Global.GetLang("使用后可随机获得一个1-6天赋属性的精灵");
		this.lblNumName.text = Global.GetLang("数量:");
		this.lblMax.text = Global.GetLang("最大");
		this.lblPriceWord.text = Global.GetLang("总价: ");
		this.lblPrice.text = Global.GetLang("200");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OnClose != null)
			{
				this.OnClose.Invoke();
			}
		};
		UIEventListener.Get(this.btnJian.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnJianNum);
		UIEventListener.Get(this.btnJia.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnJiaNum);
		UIEventListener.Get(this.btnMax).onClick = new UIEventListener.VoidDelegate(this.OnMaxNum);
		UIEventListener.Get(this.btnBuy).onClick = new UIEventListener.VoidDelegate(this.OnBuyClick);
		UIEventListener.Get(this.btnInput).onClick = new UIEventListener.VoidDelegate(this.OnInputClick);
	}

	private void OnInputClick(GameObject go)
	{
		PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(delegate(object es, DPSelectedItemEventArgs s2)
		{
			this.OnNumFinish(s2.ID);
		}, null, 0, -100);
	}

	private void OnNumFinish(int num)
	{
		if (num < 0)
		{
			num = 1;
		}
		if (num > this.m_maxNum)
		{
			num = this.m_maxNum;
		}
		this.inputNum.text = num.ToString();
		this.ResetNum(num);
	}

	private void OnBuyClick(GameObject go)
	{
		int num = this.inputNum.text.SafeToInt32(0);
		if (num > 0 && num <= this.m_maxNum)
		{
			if (this.OnBuyItem != null)
			{
				int num2 = num;
				this.OnBuyItem.Invoke(this.m_id, this.m_good, num2);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("数量错误"), 10, 3);
		}
	}

	private void OnMaxNum(GameObject go)
	{
		this.ResetNum(this.m_maxNum);
	}

	private void OnJiaNum(GameObject go)
	{
		int num = this.inputNum.text.SafeToInt32(0);
		if (num < this.m_maxNum)
		{
			this.ResetNum(num + 1);
		}
	}

	private void OnJianNum(GameObject go)
	{
		int num = this.inputNum.text.SafeToInt32(0);
		if (num > 1)
		{
			this.ResetNum(num - 1);
		}
	}

	private void ResetNum(int num)
	{
		int num2 = num * this.m_eachPrice;
		this.inputNum.text = num.ToString();
		this.lblPrice.text = num2.ToString();
		if (num > 1)
		{
			this.btnJian.isEnabled = true;
		}
		else
		{
			this.btnJian.isEnabled = false;
		}
		if (num < this.m_maxNum)
		{
			this.btnJia.isEnabled = true;
		}
		else
		{
			this.btnJia.isEnabled = false;
		}
	}

	public void InitWindow(int id, GoodVO goodVO, int price, int maxNum)
	{
		this.m_id = id;
		this.m_good = goodVO;
		this.m_eachPrice = price;
		this.m_maxNum = maxNum;
		this.lblName.text = goodVO.Title;
		this.LoadGoodsIcon(goodVO.ID);
		int toZhuanSheng = goodVO.ToZhuanSheng;
		int num = Global.GMax(goodVO.ToLevel, 1);
		string text = string.Empty;
		if (toZhuanSheng > 0)
		{
			text = string.Format(Global.GetLang("{0}转{1}级"), toZhuanSheng, num);
		}
		else
		{
			text = string.Format(Global.GetLang("{0}级"), num);
		}
		this.lblLevel.text = text;
		if (goodVO.ToOccupation >= 0)
		{
			string occupationStrByGoods = Global.GetOccupationStrByGoods(goodVO.ToOccupation);
			this.lblLimit.text = Global.GetLang(occupationStrByGoods);
		}
		else
		{
			this.lblLimit.text = Global.GetLang("通用");
		}
		int categoriy = goodVO.Categoriy;
		this.lblType.text = Global.GetGoodsType(categoriy);
		this.lblDes.text = goodVO.Description;
		this.ResetNum(1);
	}

	private void LoadGoodsIcon(int goodsId)
	{
		GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIconByGoodsID(goodsId, true);
		ggoodIcon.transform.SetParent(this.itemContainer.transform);
		ggoodIcon.transform.localPosition = new Vector3(0f, 0f, -1f);
	}

	public Action<int, GoodVO, int> OnBuyItem;

	public Action OnClose;

	public UILabel lblName;

	public UILabel lblLevelWord;

	public UILabel lblLevel;

	public UILabel lblLimitWord;

	public UILabel lblBtnOK;

	public UILabel lblLimit;

	public UILabel lblType;

	public UILabel lblDes;

	public UILabel lblNumName;

	public UILabel lblMax;

	public UILabel lblPriceWord;

	public UILabel lblPrice;

	public UISprite imgIcon;

	public GButton btnClose;

	public UIInput inputNum;

	public UIImageButton btnJian;

	public UIImageButton btnJia;

	public GameObject btnMax;

	public GameObject btnBuy;

	public GameObject btnInput;

	public GameObject itemContainer;

	private int m_eachPrice;

	private int m_maxNum;

	private int m_id;

	private GoodVO m_good;
}
