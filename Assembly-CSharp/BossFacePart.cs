using System;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class BossFacePart : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(this.imgBuff.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickBuff);
	}

	private void OnClickBuff(GameObject go)
	{
		GTipServiceEx.ShowTip(this.imgBuff.transform.position, TipTypes.BufferTip, this.buffId);
	}

	public void SetLifeInfo(double totalLife, int lifeDepth, int level)
	{
		this.TotalLife = (long)totalLife;
		this.LifeDepth = (long)lifeDepth;
		this.FullLife = this.TotalLife / this.LifeDepth;
		this.UpdateBuff(this.buffId);
	}

	public void UpdateBuff(int buffCode)
	{
		this.buffId = buffCode;
		if (this.buffId <= 0)
		{
			this.imgBuff.gameObject.SetActive(false);
			return;
		}
		if (IConfigbase<ConfigMoYuDuoBao>.Instance.GetBossInfo() != null && this.MonsterID != IConfigbase<ConfigMoYuDuoBao>.Instance.GetBossInfo().MonsterId)
		{
			this.imgBuff.gameObject.SetActive(false);
		}
		else
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(buffCode);
			if (goodsXmlNodeByID == null)
			{
				this.imgBuff.gameObject.SetActive(false);
			}
			else
			{
				this.imgBuff.gameObject.SetActive(true);
				this.imgBuff.URL = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
				{
					goodsXmlNodeByID.IconCode
				});
			}
		}
	}

	public void UpdateLife(double newLife)
	{
		if ((double)this.NewLife == newLife)
		{
			return;
		}
		this.NewLife = (long)newLife;
		long num = this.NewLife / this.FullLife;
		this.NewLifev = this.NewLife % this.FullLife;
		if (this.NewLifev == 0L && num > 0L)
		{
			this.NewLifev = this.FullLife;
			num -= 1L;
		}
		long num2 = this.CurrentLife / this.FullLife;
		this.CurrentLifev = this.CurrentLife % this.FullLife;
		if (this.CurrentLifev == 0L && num2 > 0L)
		{
			this.CurrentLifev = this.FullLife;
			num2 -= 1L;
		}
		if (num != num2)
		{
			if (this.CurrentLife < this.NewLife)
			{
				this.CurrentLifev = 0L;
			}
			else
			{
				this.CurrentLifev = this.FullLife;
			}
		}
		if (num != this.CurrentDepth)
		{
			this.CurrentDepth = num;
			long num3 = num % 5L;
			if (num3 >= 0L && num3 <= 4L)
			{
				switch ((int)num3)
				{
				case 0:
					if (num == 0L)
					{
						this.HPBar0.foreground.GetComponent<UISprite>().spriteName = "bantou";
					}
					else
					{
						this.HPBar0.foreground.GetComponent<UISprite>().spriteName = "zi";
					}
					this.HPBar2.foreground.GetComponent<UISprite>().spriteName = "hong";
					break;
				case 1:
					this.HPBar0.foreground.GetComponent<UISprite>().spriteName = "hong";
					this.HPBar2.foreground.GetComponent<UISprite>().spriteName = "huang";
					break;
				case 2:
					this.HPBar0.foreground.GetComponent<UISprite>().spriteName = "huang";
					this.HPBar2.foreground.GetComponent<UISprite>().spriteName = "lv";
					break;
				case 3:
					this.HPBar0.foreground.GetComponent<UISprite>().spriteName = "lv";
					this.HPBar2.foreground.GetComponent<UISprite>().spriteName = "lan";
					break;
				case 4:
					this.HPBar0.foreground.GetComponent<UISprite>().spriteName = "lan";
					this.HPBar2.foreground.GetComponent<UISprite>().spriteName = "zi";
					break;
				}
			}
		}
		if (this.CurrentDepth == 0L)
		{
			this.HPBar0.Percent = 0.0;
			this.CurrentDepth += 1L;
		}
		else
		{
			this.HPBar0.Percent = 1.0;
		}
		this._Depth.Text = string.Format("x{0}", this.CurrentDepth);
		if (this.CurrentLifev < this.NewLifev)
		{
			this.HPBar1.Percent = (double)((float)this.NewLifev / (float)this.FullLife);
			this.HPBar2.Percent = this.HPBar1.Percent;
		}
		else
		{
			this.HPBar1.Percent = (double)((float)this.CurrentLifev / (float)this.FullLife);
			this.HPBar2.Percent = (double)((float)this.NewLifev / (float)this.FullLife);
		}
		this.deltav = (float)Math.Abs(this.NewLife - this.CurrentLife) / (float)this.FullLife;
		this.CurrentLife = this.NewLife;
		this.UpdateTime = Time.fixedTime;
	}

	public new void Update()
	{
		if (this.NewLifev != this.CurrentLifev)
		{
			float num = this.deltav * Time.deltaTime;
			if (this.CurrentLifev < this.NewLifev)
			{
				if (this.HPBar2.Percent <= this.HPBar1.Percent + (double)num)
				{
					this.HPBar2.Percent += (double)num;
				}
				else
				{
					this.HPBar2.Percent = this.HPBar1.Percent;
					this.CurrentLifev = this.NewLifev;
				}
			}
			else if (this.HPBar1.Percent >= this.HPBar2.Percent + (double)num)
			{
				this.HPBar1.Percent -= (double)num;
			}
			else
			{
				this.HPBar1.Percent = this.HPBar2.Percent;
				this.CurrentLifev = this.NewLifev;
			}
		}
	}

	public string VSName
	{
		get
		{
			return this.MonsterName.Text;
		}
		set
		{
			this.MonsterName.Text = value;
			int num = (int)(this.MonsterName.Label.relativeSize.x * this.MonsterName.Label.transform.localScale.x) + 15;
			this.MonsterLevel.transform.localPosition = this.MonsterName.Label.transform.localPosition + new Vector3((float)num, 0f, 0f);
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

	public int MonsterID { get; set; }

	public int RoleID { get; set; }

	public string VLevel
	{
		set
		{
			this.MonsterLevel.Text = "LV" + value;
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

	public UISprite _Bak;

	public UISprite _Front;

	public GImgProgressBar HPBar0;

	public GImgProgressBar HPBar1;

	public GImgProgressBar HPBar2;

	public TextBlock MonsterName;

	public TextBlock MonsterLevel;

	public TextBlock _Depth;

	public ShowNetImage imgBuff;

	private long TotalLife = 2147483647L;

	private long LifeDepth = 1L;

	private long FullLife = 2147483647L;

	private long CurrentDepth;

	private long CurrentLife;

	private long NewLife;

	private long CurrentLifev;

	private long NewLifev;

	private float UpdateTime;

	private int buffId = -1;

	private float deltav = 0.02f;

	private int _SpriteType;

	private string _LifeTip = string.Empty;

	private object _ItemObject;
}
