using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ZaDanPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.MyItemCollection = this.myList.ItemsSource;
		this.iconDic.Add(0, this.icon1);
		this.iconDic.Add(1, this.icon2);
		this.iconDic.Add(2, this.icon3);
		this.iconDic.Add(3, this.icon4);
		this.iconDic.Add(4, this.icon5);
		this.iconDic.Add(5, this.icon6);
		this.iconDic.Add(6, this.icon7);
		this.iconDic.Add(7, this.icon8);
		this.iconDic.Add(8, this.icon9);
		this.iconDic.Add(9, this.icon10);
		this.iconDic.Add(10, this.icon11);
		this.iconDic.Add(11, this.icon12);
		this.closeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
			SystemHelpMgr.OnAction(UIObjIDs.QiFuPart, HelpStateEvents.Inactived, 1);
		};
		this.qifuBtn1.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.Data.IsDoingZaJinDan = true;
			GameInstance.Game.SpriteZaJinDan(1);
			SystemHelpMgr.OnAction(UIObjIDs.QiFuPartBtn01, HelpStateEvents.Clicked, 1);
		};
		this.qifuBtn10.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.Data.IsDoingZaJinDan = true;
			GameInstance.Game.SpriteZaJinDan(10);
		};
		this.qifuBtn50.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Global.Data.IsDoingZaJinDan = true;
			GameInstance.Game.SpriteZaJinDan(50);
		};
		this.duiHuanBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.changKuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 2
				});
			}
		};
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
		this.PauseAllEffect(true);
	}

	public void InitPartSize(int width, int height)
	{
	}

	public void InitPartData()
	{
		if (!this.FirstInitPartData)
		{
			return;
		}
		this.FirstInitPartData = false;
		this.InitGoodsGicon();
		this.LastQueryHistoryTicks = U3DUtils.GetTimer();
		this.QueryHistory();
		if (Global.Data.JinDanGoodsDataList == null)
		{
			GameInstance.Game.SpriteGetJinDanGoodsDataList(2000);
		}
		SystemHelpMgr.OnAction(UIObjIDs.QiFuPart, HelpStateEvents.Actived, 1);
	}

	public void GetNewData()
	{
		if (U3DUtils.GetTimer() - this.LastQueryHistoryTicks >= 5000)
		{
			this.LastQueryHistoryTicks = U3DUtils.GetTimer();
			this.QueryHistory();
		}
		this.RefreshMoney();
		GameInstance.Game.SpriteGetZaJinDanJiFen();
	}

	private void InitGoodsGicon()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("BaoKuJiangLi", '|');
		if (systemParamStringArrayByName.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length > 0)
			{
				int goodsID = Convert.ToInt32(array[0]);
				int forgeLevel = Convert.ToInt32(array[2]);
				int zhuijiaLevel = Convert.ToInt32(array[3]);
				int zhuoyueIndex = Convert.ToInt32(array[5]);
				int lucky = Convert.ToInt32(array[4]);
				GGoodIcon icon = this.iconDic[i];
				GoodsData gd = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				icon.Width = 78.0;
				icon.Height = 78.0;
				icon.BodyURL = new ImageURL(Global.GetGoodsIconString(gd.GoodsID), false, 0);
				icon.TipType = 1;
				icon.ItemCode = gd.GoodsID;
				icon.ItemObject = gd;
				icon.BoxTypes = 0;
				icon.TextSize = 16;
				icon.TextShadowColor = 4278190080U;
				icon.Tag = gd.ExcellenceInfo;
				Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
				icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
				{
					GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, gd);
				};
				if (Convert.ToInt32(array[1]) == -1 || string.Empty == array[1])
				{
				}
			}
		}
	}

	public void UpdateGoodsCountText(int count)
	{
		this.goodsCountText.Text = count.ToString();
	}

	public void SetGoodsCountText()
	{
		if (Global.Data.JinDanGoodsDataList != null)
		{
			this.goodsCountText.Text = Global.Data.JinDanGoodsDataList.Count.ToString();
		}
		else
		{
			this.goodsCountText.Text = string.Empty;
		}
	}

	private void QueryHistory()
	{
		GameInstance.Game.SpriteQueryZaJinDanHistoryList(true);
	}

	public void OnZaDanCompleted(int ret)
	{
		this.RefreshMoney();
		if (ret > 0)
		{
			this.QueryHistory();
			int num = U3DUtils.GetTimer() - this.startTime;
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 2
				});
			}
		}
		else if (ret == -100)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("当宝物仓库满后，直接提示宝物仓库已满，建议您取出部分仓库内的物品"), 0, -1, -1, 0);
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("宝物仓库必须有{0}个空格"), new object[]
			{
				this.ZaJinDanTimesType
			}), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("砸金蛋失败！"), 0, -1, -1, 0);
		}
	}

	public void OnSelfZaDanHistoryListQueryCompleted(List<ZaJinDanHistory> ls)
	{
		if (ls == null)
		{
			return;
		}
		this.MyItemCollection.Clear();
		for (int i = 0; i < ls.Count; i++)
		{
			ZaJinDanHistory zaJinDanHistory = ls[i];
			if (zaJinDanHistory != null)
			{
				RecordItem recordItem = U3DUtils.NEW<RecordItem>();
				recordItem.BodyWidth = 180.0;
				recordItem.BodyHeight = 20.0;
				recordItem.Text = this.GetGainString(zaJinDanHistory, true);
				this.MyItemCollection.AddNoUpdate(recordItem);
			}
		}
		this.MyItemCollection.DelayUpdate();
	}

	public void OnZaDanHistoryListQueryCompleted(List<ZaJinDanHistory> ls)
	{
		if (ls == null)
		{
			return;
		}
		this.jiluAllText.Text = string.Empty;
		string text = string.Empty;
		for (int i = 0; i < ls.Count; i++)
		{
			ZaJinDanHistory zaJinDanHistory = ls[i];
			if (zaJinDanHistory != null)
			{
				text += StringUtil.substitute("{0}\n", new object[]
				{
					this.GetGainString(zaJinDanHistory, false)
				});
			}
		}
		this.jiluAllText.Text = text;
	}

	public string GetGainString(ZaJinDanHistory obj, bool isSelf = true)
	{
		string text = string.Empty;
		if (obj.GainGoodsId > 0)
		{
			text += Global.GetGoodsNameByID(obj.GainGoodsId, false);
			string[] array = obj.GoodPorp.Split(new char[]
			{
				'|'
			});
			if (array != null)
			{
				if (array.Length != 4)
				{
					return text;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int lucky = Convert.ToInt32(array[2]);
				int num3 = Convert.ToInt32(array[3]);
				if (num3 != 0)
				{
					text = string.Format(Global.GetLang("卓越的{0}"), text);
				}
				else if (num != 0)
				{
					text += string.Format(Global.GetLang("强化+{0}"), num);
				}
				else if (num2 != 0)
				{
					text += string.Format(Global.GetLang("追{0}"), num2);
				}
				GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(obj.GainGoodsId, num, num2, num3, lucky, 0, 0, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
				text = StringUtil.substitute(Global.GetLang("{{{0}}}【{1}】{{-}}"), new object[]
				{
					Global.GetStrColorByGoodsData(dummyGoodsDataMu),
					text
				});
			}
		}
		if (obj.GainGold > 0)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += StringUtil.substitute(Global.GetLang("{0}绑定钻石"), new object[]
			{
				obj.GainGold
			});
		}
		if (obj.GainYinLiang > 0)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += StringUtil.substitute(Global.GetLang("{0}金币"), new object[]
			{
				obj.GainYinLiang
			});
		}
		if (obj.GainExp > 0)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += StringUtil.substitute(Global.GetLang("{{f9f702}}经验{0}{{-}}"), new object[]
			{
				obj.GainExp
			});
		}
		if (isSelf)
		{
			return StringUtil.substitute(Global.GetLang("您获得{0}"), new object[]
			{
				text
			});
		}
		return StringUtil.substitute(Global.GetLang("{{00ff00}}{0}{{-}}祈福获得{1}"), new object[]
		{
			obj.RoleName,
			text
		});
	}

	public void PauseAllEffect(bool pause)
	{
		for (int i = 0; i < this.DecoArr.Count; i++)
		{
			if (this.DecoArr[i] != null)
			{
				this.DecoArr[i].Pause = pause;
			}
		}
	}

	private void select(EventArgs e)
	{
	}

	public void OnGetXinyundianCompleted(int lucky, int bits)
	{
		if (lucky < 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("获取幸运点失败"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			this.JifenText.Text = lucky.ToString();
		}
	}

	private void RefreshMoney()
	{
		this.zhuangshiText.Text = StringUtil.substitute("{0}", new object[]
		{
			Global.Data.roleData.UserMoney
		});
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.qifuBtn1, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.changKuBtn, default(Vector4));
			}
			else if (id == 10000)
			{
				SystemHelpPart.SetMask(this.closeBtn, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	private ListBox myList = new ListBox();

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<GDecoration> DecoArr = new List<GDecoration>();

	private int startTime;

	private int ZaJinDanTimesType = 1;

	private ObservableCollection MyItemCollection;

	public GGoodIcon icon1;

	public GGoodIcon icon2;

	public GGoodIcon icon3;

	public GGoodIcon icon4;

	public GGoodIcon icon5;

	public GGoodIcon icon6;

	public GGoodIcon icon7;

	public GGoodIcon icon8;

	public GGoodIcon icon9;

	public GGoodIcon icon10;

	public GGoodIcon icon11;

	public GGoodIcon icon12;

	public GButton qifuBtn1;

	public GButton qifuBtn10;

	public GButton qifuBtn50;

	public GButton duiHuanBtn;

	public GButton changKuBtn;

	public GButton chongZhiBtn;

	public GButton closeBtn;

	public TextBlock zhuangshiText;

	public TextBlock JifenText;

	public TextBlock jiluAllText;

	public TextBlock goodsCountText;

	public Dictionary<int, GGoodIcon> iconDic = new Dictionary<int, GGoodIcon>();

	public UISpriteAnimation TexiaoAnimation;

	private GImgProgressBar XingyunBar = new GImgProgressBar();

	private int LastQueryHistoryTicks;

	private bool FirstInitPartData = true;
}
