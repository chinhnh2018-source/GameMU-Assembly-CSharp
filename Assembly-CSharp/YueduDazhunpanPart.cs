using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class YueduDazhunpanPart : UserControl
{
	public YueduDazhunpanPart()
	{
		this.GoodsGIconXYArr = new int[,]
		{
			{
				33,
				115
			},
			{
				81,
				115
			},
			{
				129,
				115
			},
			{
				177,
				115
			},
			{
				225,
				115
			},
			{
				273,
				115
			},
			{
				321,
				115
			},
			{
				321,
				168
			},
			{
				321,
				221
			},
			{
				321,
				274
			},
			{
				273,
				274
			},
			{
				225,
				274
			},
			{
				177,
				274
			},
			{
				129,
				274
			},
			{
				81,
				274
			},
			{
				33,
				274
			},
			{
				33,
				221
			},
			{
				33,
				168
			},
			{
				89,
				186
			},
			{
				249,
				186
			}
		};
		this.MyItemCollection = this.myList.ItemsSource;
		this.AllItemCollection = this.allList.ItemsSource;
	}

	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.txtShengyuTime);
		Canvas.SetLeft(this.txtShengyuTime, 122);
		Canvas.SetTop(this.txtShengyuTime, 28);
		this.txtShengyuTime.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtChongzhi);
		Canvas.SetLeft(this.txtChongzhi, 122);
		Canvas.SetTop(this.txtChongzhi, 51);
		this.txtChongzhi.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtShengyuChoujiangNum);
		Canvas.SetLeft(this.txtShengyuChoujiangNum, 320);
		Canvas.SetTop(this.txtShengyuChoujiangNum, 51);
		this.txtShengyuChoujiangNum.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtChongzhi2);
		Canvas.SetLeft(this.txtChongzhi2, 138);
		Canvas.SetTop(this.txtChongzhi2, 92);
		this.txtChongzhi2.TextColor = new SolidColorBrush(65535U);
		this.txtChongzhi2.fontBold = true;
		GIcon gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("抽奖");
		gicon.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(gicon, 152);
		Canvas.SetTop(gicon, 196);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			this.ShowModalDialog();
			GameInstance.Game.SpriteExecuteYueDuChouJiang();
		};
		this.BtnArr[0] = gicon;
		ImageBrush bodySource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_normal.png"), 81.0, 21.0, 3.0, 2.0));
		ImageBrush newSource = new ImageBrush(Super.ConvertBitmapDataByGrid9(Global.GetLoginResImage("Images/Plate/btn21_hover.png"), 81.0, 21.0, 3.0, 2.0));
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 82.0;
		gicon.Height = 25.0;
		gicon.BodySource = bodySource;
		gicon.NewSource = newSource;
		gicon.Text = Global.GetLang("金蛋仓库");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 1
				});
			}
		};
		Canvas.SetLeft(gicon, 396);
		Canvas.SetTop(gicon, 14);
		this.Container.Children.Add(gicon);
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 82.0;
		gicon.Height = 25.0;
		gicon.BodySource = bodySource;
		gicon.NewSource = newSource;
		gicon.Text = Global.GetLang("充  值");
		gicon.TextColor = new SolidColorBrush(ColorSL.FromArgb(255, 160, 255, 255));
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			Super.OpenChongZhiHtmlWindow();
		};
		Canvas.SetLeft(gicon, 500);
		Canvas.SetTop(gicon, 14);
		this.Container.Children.Add(gicon);
		this.Container.Children.Add(this.myList);
		this.myList.Width = 205.0;
		this.myList.Height = 112.0;
		this.myList.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.myList, 383);
		Canvas.SetTop(this.myList, 65);
		this.myList.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
		this.Container.Children.Add(this.allList);
		this.allList.Width = 205.0;
		this.allList.Height = 112.0;
		this.allList.Background = new SolidColorBrush(16777215U);
		Canvas.SetLeft(this.allList, 383);
		Canvas.SetTop(this.allList, 211);
		this.allList.VerticalScrollBarVisibility = global::ScrollBarVisibility.Auto;
	}

	public void InitPartData()
	{
		if (this.state != null)
		{
			this.txtChongzhi.Text = string.Empty + this.state.ChongzhiYuanbaoNum;
			this.txtShengyuChoujiangNum.Text = string.Empty + this.state.ShengyuChoujiangNum;
		}
		DateTime correctDateTime = Global.GetCorrectDateTime();
		double num = (double)Math.Max(0L, (this.HuodongEndTime.Ticks - correctDateTime.Ticks) / 10000000L);
		if (num <= 0.0)
		{
			this.txtShengyuTime.Text = Global.GetLang("活动已结束");
			this.txtShengyuTime.TextColor = new SolidColorBrush(16725815U);
		}
		else
		{
			this.txtShengyuTime.Text = Global.GetTimeStrBySecEx(num, true, -1);
			this.txtShengyuTime.TextColor = new SolidColorBrush(65280U);
		}
		if (this.state.ShengyuChoujiangNum > 0)
		{
			this.SetBtnState(0, true);
		}
		else
		{
			this.SetBtnState(0, false);
		}
	}

	public void GetNewData()
	{
		if (U3DUtils.GetTimer() - this.LastQueryHistoryTicks >= 5000)
		{
			this.LastQueryHistoryTicks = U3DUtils.GetTimer();
			this.QueryHistory();
		}
	}

	public void CleanUpChildWindows()
	{
		Super.CleanUpAllChildWindows(this.Container);
		this.PauseAllEffect(true);
	}

	public void InitGoodsGicon()
	{
		this.txtChongzhi2.Text = ConfigSystemParam.GetSystemParamByName("ChongzhiNumByYueduDazhuanpan", true);
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("YueduDazhuanpan", '|');
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
				GIcon gicon = this.GetGicon(i, int.Parse(array[0]));
				Canvas.SetLeft(gicon, this.GoodsGIconXYArr[i, 0]);
				Canvas.SetTop(gicon, this.GoodsGIconXYArr[i, 1]);
				this.Container.Children.Add(gicon);
				if (int.Parse(array[1]) != -1 && !(string.Empty == array[1]))
				{
					GDecoration decoration = Global.GetDecoration(int.Parse(array[1]), GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
					decoration.Coordinate = new Point(this.GoodsGIconXYArr[i, 0] + 68, this.GoodsGIconXYArr[i, 1] + 20);
					Canvas.SetZIndex(decoration, 1.0);
					this.DecoArr.Add(decoration);
				}
			}
		}
	}

	private GIcon GetGicon(int index, int iGoodsid)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(iGoodsid);
		if (goodsXmlNodeByID != null)
		{
			GIcon gicon = U3DUtils.NEW<GIcon>();
			gicon.Width = 32.0;
			gicon.Height = 32.0;
			gicon.TipType = 1;
			gicon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				iGoodsid,
				0,
				-1,
				-1
			});
			gicon.ItemCode = iGoodsid;
			gicon.ItemCategory = goodsXmlNodeByID.Categoriy;
			if (this.GoodsGIconXYArr.Length - index <= 2)
			{
				gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCodeEx(goodsXmlNodeByID.IconCode), false, 2);
			}
			else
			{
				gicon.BodyURL = new ImageURL(Super.GetGoodsImageURLFromIconCode(goodsXmlNodeByID.IconCode, string.Empty), false, 0);
			}
			return gicon;
		}
		return null;
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

	public void GetData()
	{
		GameInstance.Game.SpriteQueryYueDuChouJiangInfo();
		this.GetNewData();
	}

	public void OnGetDataCompleted(int roleID, int nCount, int nYuanBao)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		this.state = new YueduDazhunpanPart.localParamTrans();
		this.state.ChongzhiYuanbaoNum = nYuanBao;
		this.state.ShengyuChoujiangNum = nCount;
		this.InitPartData();
	}

	public void OnLingquCompleted(int result, int roleID, int nCount, int nYuanBao)
	{
		this.CloseModalDialog();
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result < 0)
		{
			if (result == -20)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("宝物仓库已满，建议您取出部分仓库内的物品"), 0, -1, -1, 0);
			}
			if (result == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("当前抽奖次数为0，无法进行抽奖，请充值后再进行抽奖"), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, Global.GetLang("抽奖失败！"), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("领取奖品成功"), 0, -1, -1, 0);
			this.QueryHistory();
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = 2
				});
			}
			this.state = new YueduDazhunpanPart.localParamTrans();
			this.state.ChongzhiYuanbaoNum = nYuanBao;
			this.state.ShengyuChoujiangNum = nCount;
			this.InitPartData();
		}
	}

	private void SetBtnState(int id, bool flag)
	{
		if (id >= 0 && null != this.BtnArr[id])
		{
			this.BtnArr[id].Visibility = true;
			if (this.state != null)
			{
				this.BtnArr[id].Text = "抽奖";
				this.BtnArr[id].EnableIcon = flag;
			}
		}
	}

	private void QueryHistory()
	{
		GameInstance.Game.SpriteQueryYueDuChouJiangHistory(true);
		GameInstance.Game.SpriteQueryYueDuChouJiangHistory(false);
	}

	public void OnSelfZaDanHistoryListQueryCompleted(List<YueDuChouJiangData> ls)
	{
		if (ls == null)
		{
			return;
		}
		this.MyItemCollection.Clear();
		for (int i = 0; i < ls.Count; i++)
		{
			YueDuChouJiangData yueDuChouJiangData = ls[i];
			if (yueDuChouJiangData != null)
			{
				RecordItem recordItem = U3DUtils.NEW<RecordItem>();
				recordItem.BodyWidth = 180.0;
				recordItem.BodyHeight = 20.0;
				recordItem.Text = this.GetGainString(yueDuChouJiangData, true);
				this.MyItemCollection.AddNoUpdate(recordItem);
			}
		}
		this.MyItemCollection.DelayUpdate();
	}

	public void OnZaDanHistoryListQueryCompleted(List<YueDuChouJiangData> ls)
	{
		this.AllItemCollection.Clear();
		for (int i = 0; i < ls.Count; i++)
		{
			YueDuChouJiangData yueDuChouJiangData = ls[i];
			if (yueDuChouJiangData != null)
			{
				RecordItem recordItem = U3DUtils.NEW<RecordItem>();
				recordItem.BodyWidth = 180.0;
				recordItem.BodyHeight = 20.0;
				recordItem.Text = this.GetGainString(yueDuChouJiangData, false);
				this.AllItemCollection.AddNoUpdate(recordItem);
			}
		}
		this.AllItemCollection.DelayUpdate();
	}

	public string GetGainString(YueDuChouJiangData obj, bool isSelf = true)
	{
		string text = string.Empty;
		if (obj.GainGoodsId > 0)
		{
			text += StringUtil.substitute(Global.GetLang("｛color={0} uline=false tag= text={1}｝"), new object[]
			{
				Global.GetGoodsColorString(obj.GainGoodsId),
				Global.GetGoodsNameByID(obj.GainGoodsId, false)
			});
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
			text += StringUtil.substitute(Global.GetLang("{0}经验"), new object[]
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
		return StringUtil.substitute(Global.GetLang("{0}获得{1}"), new object[]
		{
			obj.RoleName,
			text
		});
	}

	public void ShowModalDialog()
	{
		this.PlaceHolder = new Canvas();
		this.PlaceHolder.Background = new SolidColorBrush(4286611584U);
		this.PlaceHolder.Opacity = 0.01;
		this.PlaceHolder.Width = this.Width;
		this.PlaceHolder.Height = this.Height;
		Canvas.SetZIndex(this.PlaceHolder, 9000.0);
		this.Container.Children.Add(this.PlaceHolder);
	}

	public void CloseModalDialog()
	{
		if (null != this.PlaceHolder)
		{
			this.PlaceHolder.Visibility = false;
			this.Container.Children.Remove(this.PlaceHolder, true);
			this.PlaceHolder = null;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine txtShengyuTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtChongzhi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtShengyuChoujiangNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtChongzhi2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private ListBox myList = new ListBox();

	private ListBox allList = new ListBox();

	private ObservableCollection MyItemCollection;

	private ObservableCollection AllItemCollection;

	private int LastQueryHistoryTicks;

	private DateTime HuodongEndTime;

	private int[,] GoodsGIconXYArr;

	private List<GDecoration> DecoArr = new List<GDecoration>();

	private LoadingWindow LoadingWin;

	private YueduDazhunpanPart.localParamTrans state;

	private GIcon[] BtnArr = new GIcon[0];

	private Canvas PlaceHolder;

	private class localParamTrans
	{
		public int ChongzhiYuanbaoNum;

		public int ShengyuChoujiangNum;
	}
}
