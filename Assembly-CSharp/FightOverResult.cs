using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class FightOverResult : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.BtnClose.Label.text = Global.GetLang("离开");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.CountDown();
		this.InitTextInPrefabs();
		this.ItemCollection = this.ListAward.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isYaoSaiJianYu)
			{
				GameInstance.Game.SpriteGetJingJiJunxianLeaveCmd();
			}
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void ZhengDuoAwardDataManage(ZhengDuoAwardData data)
	{
		if (data == null)
		{
			return;
		}
		if (data.State == 1)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		if (data.Second == 0)
		{
			this.Lab1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("未击杀")
			});
		}
		else
		{
			this.Lab1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("用时:{0}"), Global.GetTimeStrBySecEx((double)data.Second, true, -1))
			});
		}
		this.Lab2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("经验："),
			"fdf7dd",
			data.Exp
		});
		this.Lab4.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("绑金："),
			"fdf7dd",
			data.Money
		});
		if (data.GoodsList != null && data.GoodsList.Count > 0)
		{
			for (int i = 0; i < data.GoodsList.Count; i++)
			{
				this.addGoodsIcon(data.GoodsList[i], false);
			}
		}
	}

	public void ZhengDuoAwardDataManage(KarenBattleAwardsData data)
	{
		if (data == null)
		{
			return;
		}
		if (data.Success == 1)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		this.Lab2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("经验："),
			"fdf7dd",
			data.Exp
		});
		this.Lab4.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("绑金："),
			"fdf7dd",
			data.BindJinBi
		});
		if (data.AwardsItemDataList != null && data.AwardsItemDataList.Count > 0)
		{
			for (int i = 0; i < data.AwardsItemDataList.Count; i++)
			{
				this.addGoodsIcon(data.AwardsItemDataList[i], false);
			}
		}
	}

	public void YaoSaiJianYuDataManage(int type, int win, string name)
	{
		this.isYaoSaiJianYu = true;
		if (win == 1)
		{
			this.AnimWin.gameObject.SetActive(true);
			this.AnimLose.gameObject.SetActive(false);
		}
		else
		{
			this.AnimWin.gameObject.SetActive(false);
			this.AnimLose.gameObject.SetActive(true);
		}
		if (type == 0)
		{
			if (win == 1)
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("你向{0}发起征服，恭喜你成功了！"), name)
				});
			}
			else
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("你向{0}发起征服，很可惜失败了！"), name)
				});
			}
		}
		else if (type == -1)
		{
			if (win == 1)
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("你向主人发起反抗，恭喜你成功了！")
				});
			}
			else
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					Global.GetLang("你向主人发起反抗，很可惜失败了！")
				});
			}
		}
		else if (type == 1)
		{
			if (win == 1)
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("你向{0}发起抢夺，恭喜你成功了！"), name)
				});
			}
			else
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("你向{0}发起抢夺，很可惜失败了！"), name)
				});
			}
		}
		else if (type == 2)
		{
			if (win == 1)
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("你仗义解救{0}，恭喜你成功了！"), name)
				});
			}
			else
			{
				this.Lab5.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("你仗义解救{0}，很可惜失败了！"), name)
				});
			}
		}
	}

	private void CountDown()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	private IEnumerator TickProc()
	{
		for (;;)
		{
			if (this.Time)
			{
				if (this.count < 1)
				{
					base.StopCoroutine("TickProc");
					PlayZone.GlobalPlayZone.ClosefightOverResultWindow();
				}
				this.Time.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format(Global.GetLang("{0}秒后关闭"), this.count)
				});
				this.count--;
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	private void addGoodsIcon(GoodsData gd, bool grayShow = false)
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
			this.ItemCollection.AddNoUpdate(ggoodIcon);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
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

	public DPSelectedItemEventHandler CloseHandler;

	public Animator AnimWin;

	public Animator AnimLose;

	public UILabel Lab1;

	public UILabel Lab2;

	public UILabel Lab4;

	public UILabel Time;

	public UILabel Lab5;

	public GButton BtnClose;

	public ListBox ListAward;

	private int count = 5;

	private bool isYaoSaiJianYu;

	private ObservableCollection _ItemCollection;
}
