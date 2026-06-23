using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class SpiritTrackItemAttrPart : UserControl
{
	private new string Name
	{
		set
		{
			this.Title.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang(value)
			});
		}
	}

	private string Attr1
	{
		set
		{
			this.AttrLab1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(value)
			});
		}
	}

	private string Attr2
	{
		set
		{
			this.AttrLab2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(value)
			});
		}
	}

	private string AttrAdd1
	{
		set
		{
			this.AttrAddLab1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(value)
			});
		}
	}

	private string AttrAdd2
	{
		set
		{
			this.AttrAddLab2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(value)
			});
		}
	}

	private string MiaoShuTe
	{
		set
		{
			this.Miaoshu.text = Global.GetColorStringForNGUIText(new object[]
			{
				this.Color,
				Global.GetLang(value)
			});
		}
	}

	private string QianZhiTe
	{
		set
		{
			this.QianZhi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang(value)
			});
		}
	}

	private bool ShowJianTou
	{
		set
		{
			this.JianTou1.gameObject.SetActive(value);
			this.JianTou2.gameObject.SetActive(value);
		}
	}

	private bool ShowManJi
	{
		set
		{
			this.ManJi.gameObject.SetActive(value);
		}
	}

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

	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
		}
	}

	public string Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
		}
	}

	public void JiHuoState(string name, string attr1, string attr2, string miaoshu, bool canJiHuo)
	{
		this.AttrAdd1 = string.Empty;
		this.AttrAdd2 = string.Empty;
		this.QianZhiTe = string.Empty;
		this.ShowJianTou = false;
		this.ShowManJi = false;
		this.BtnShengJi.gameObject.SetActive(true);
		this.BtnShengJi.Label.text = Global.GetLang("激活");
		if (canJiHuo)
		{
			this.BtnShengJi.isEnabled = true;
		}
		else
		{
			this.Color = "FF0000";
			this.BtnShengJi.isEnabled = false;
		}
		this.Name = name;
		this.Attr1 = attr1;
		this.Attr2 = attr2;
		this.MiaoShuTe = miaoshu;
	}

	public void ShengJiState(string name, string attr1, string attr2, string attradd1, string attradd2, string miaoshu, bool canjihuo)
	{
		this.QianZhiTe = string.Empty;
		this.ShowJianTou = true;
		this.ShowManJi = false;
		this.BtnShengJi.gameObject.SetActive(true);
		this.BtnShengJi.Label.text = Global.GetLang("升级");
		if (canjihuo)
		{
			this.BtnShengJi.isEnabled = true;
		}
		else
		{
			this.Color = "FF0000";
			this.BtnShengJi.isEnabled = false;
		}
		this.Name = name;
		this.Attr1 = attr1;
		this.Attr2 = attr2;
		this.AttrAdd1 = attradd1;
		this.AttrAdd2 = attradd2;
		this.MiaoShuTe = miaoshu;
	}

	public void ManJiState(string name, string attr1, string attr2)
	{
		this.AttrAdd1 = string.Empty;
		this.AttrAdd2 = string.Empty;
		this.QianZhiTe = string.Empty;
		this.MiaoShuTe = string.Empty;
		this.ShowJianTou = false;
		this.ShowManJi = true;
		this.BtnShengJi.gameObject.SetActive(false);
		this.Name = name;
		this.Attr1 = attr1;
		this.Attr2 = attr2;
	}

	public void JiHuoQianZhiState(string name, string attr1, string attr2, string qianzhi)
	{
		this.AttrAdd1 = string.Empty;
		this.AttrAdd2 = string.Empty;
		this.MiaoShuTe = string.Empty;
		this.ShowJianTou = false;
		this.ShowManJi = false;
		this.BtnShengJi.gameObject.SetActive(false);
		this.Name = name;
		this.Attr1 = attr1;
		this.Attr2 = attr2;
		this.QianZhiTe = qianzhi;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
		this.BtnShengJi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.BtnShengJi.Label.text.Equals("激活"))
			{
				this.DPSelected(this, new DPSelectedItemEventArgs
				{
					ID = 1,
					Index = this.index
				});
			}
			else if (this.BtnShengJi.Label.text.Equals("升级"))
			{
				this.DPSelected(this, new DPSelectedItemEventArgs
				{
					ID = 2,
					Index = this.index
				});
			}
			Super.ShowNetWaiting(null);
			GameInstance.Game.ShenJiShengJi(this.ID);
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
	}

	public UILabel Title;

	public UILabel AttrLab1;

	public UILabel AttrLab2;

	public UILabel AttrAddLab1;

	public UILabel AttrAddLab2;

	public UILabel Miaoshu;

	public UILabel QianZhi;

	public UISprite JianTou1;

	public UISprite JianTou2;

	public UISprite ManJi;

	public GButton BtnClose;

	public GButton BtnShengJi;

	public DPSelectedItemEventHandler DPSelectedClose;

	public DPSelectedItemEventHandler DPSelected;

	private int id;

	private int index;

	private string color = "17e43e";
}
