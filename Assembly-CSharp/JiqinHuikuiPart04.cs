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

public class JiqinHuikuiPart04 : UserControl
{
	public void InitPartSize(int width, int height)
	{
		this.Width = (double)width;
		this.Height = (double)height;
		this.Container.Width = (double)width;
		this.Container.Height = (double)height;
		this.Container.Children.Add(this.txtShengyuTime);
		Canvas.SetLeft(this.txtShengyuTime, 115);
		Canvas.SetTop(this.txtShengyuTime, 8);
		this.txtShengyuTime.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtChongzhi);
		Canvas.SetLeft(this.txtChongzhi, 115);
		Canvas.SetTop(this.txtChongzhi, 32);
		this.txtChongzhi.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtShengyuChoujiangNum);
		Canvas.SetLeft(this.txtShengyuChoujiangNum, 355);
		Canvas.SetTop(this.txtShengyuChoujiangNum, 32);
		this.txtShengyuChoujiangNum.TextColor = new SolidColorBrush(16777215U);
		this.Container.Children.Add(this.txtChongzhi2);
		Canvas.SetLeft(this.txtChongzhi2, 112);
		Canvas.SetTop(this.txtChongzhi2, 78);
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
		Canvas.SetLeft(gicon, 165);
		Canvas.SetTop(gicon, 185);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!base.EnableIcon)
			{
				return;
			}
			this.LoadingWin = U3DUtils.NEW<LoadingWindow>();
			this.Container.Children.Add(this.LoadingWin);
			GameInstance.Game.SpriteXingYunChouJiang();
		};
		this.BtnArr[0] = gicon;
		gicon = U3DUtils.NEW<GIcon>();
		gicon.Width = 88.0;
		gicon.Height = 31.0;
		gicon.BodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_normal.png"));
		gicon.NewSource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_hover.png"));
		gicon.DisableBodySource = new ImageBrush(Global.GetGameResImage("Images/Plate/lingqu_nouse.png"));
		gicon.Text = Global.GetLang("宝物仓库");
		gicon.TextColor = new SolidColorBrush(16777080U);
		Canvas.SetLeft(gicon, 325);
		Canvas.SetTop(gicon, 71);
		this.Container.Children.Add(gicon);
		gicon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 12,
					IDType = 0
				});
			}
		};
	}

	public void InitPartData()
	{
		if (this.state != null)
		{
			this.txtChongzhi.Text = this.state.ChongzhiYuanbaoNum.ToString();
			this.txtShengyuChoujiangNum.Text = this.state.ShengyuChoujiangNum.ToString();
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

	public void InitGoodsGicon()
	{
		this.txtChongzhi2.Text = ConfigSystemParam.GetSystemParamByName("ChongzhiNumByXinyunDazhuanpan", true);
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("XinyunDazhuanpan", '|');
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
				GIcon gicon = this.GetGicon(i, Convert.ToInt32(array[0]));
				Canvas.SetLeft(gicon, this.GoodsGIconXYArr[i, 0]);
				Canvas.SetTop(gicon, this.GoodsGIconXYArr[i, 1]);
				this.Container.Children.Add(gicon);
				if (Convert.ToInt32(array[1]) != -1 && !(string.Empty == array[1]))
				{
					GDecoration decoration = Global.GetDecoration(Convert.ToInt32(array[1]), GDecorationTypes.Loop, new Point(0, 0), false, null, -1, -1, true, false);
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
	}

	public void OnGetDataCompleted(JiQingHuiKuiData result)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result != null)
		{
			this.state = new StateObject04();
			this.state.ChongzhiYuanbaoNum = result.XingYunChouJiangYuanBao;
			this.state.ShengyuChoujiangNum = result.XingYunChouJiangCount;
			this.InitPartData();
		}
	}

	public void OnLingquCompleted(int result, JiQingHuiKuiData resdata)
	{
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
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("宝物仓库剩余空格不足1格，无法进行抽奖"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -100)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("当前抽奖次数为0，无法进行抽奖，请充值后再进行抽奖"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取奖励失败，失败原因{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, Global.GetLang("恭喜你，成功领取了奖品"), 0, -1, -1, 0);
			this.state = new StateObject04();
			this.state.ChongzhiYuanbaoNum = resdata.XingYunChouJiangYuanBao;
			this.state.ShengyuChoujiangNum = resdata.XingYunChouJiangCount;
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

	public DPSelectedItemEventHandler DPSelectedItem;

	private GTextBlockOutLine txtShengyuTime = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtChongzhi = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtShengyuChoujiangNum = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private GTextBlockOutLine txtChongzhi2 = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private DateTime HuodongEndTime;

	private int[,] GoodsGIconXYArr = new int[,]
	{
		{
			23,
			106
		},
		{
			79,
			106
		},
		{
			135,
			106
		},
		{
			191,
			106
		},
		{
			247,
			106
		},
		{
			303,
			106
		},
		{
			359,
			106
		},
		{
			359,
			157
		},
		{
			359,
			208
		},
		{
			359,
			259
		},
		{
			303,
			259
		},
		{
			247,
			259
		},
		{
			191,
			259
		},
		{
			135,
			259
		},
		{
			79,
			259
		},
		{
			23,
			259
		},
		{
			23,
			208
		},
		{
			23,
			157
		},
		{
			99,
			175
		},
		{
			267,
			175
		}
	};

	private List<GDecoration> DecoArr = new List<GDecoration>();

	private LoadingWindow LoadingWin;

	private StateObject04 state;

	private List<GIcon> BtnArr = new List<GIcon>();
}
