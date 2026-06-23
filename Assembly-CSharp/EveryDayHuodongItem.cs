using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class EveryDayHuodongItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.jiangliListBoxOBC = this.mJiangliListBox.ItemsSource;
		this.qianggouListBoxOBC = this.mQianggouGood.ItemsSource;
		this.btnConfim.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_EveryDayActivityXML.GoalType == 1)
			{
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), this.m_EveryDayActivityXML.Price.Split(new char[]
					{
						'|'
					})[0])
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						this.DPSelectedItem(this, new DPSelectedItemEventArgs
						{
							ID = 1,
							Flag = this.ActID,
							Type = this.m_ChongZhiID,
							IDType = this.m_ZhiGouID
						});
					}
					return true;
				};
				return;
			}
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				Flag = this.ActID,
				Type = this.m_ChongZhiID,
				IDType = this.m_ZhiGouID
			});
		};
		if (this.m_XiaoHao && this.m_XiaoHao.transform.GetChild(4).name.Equals("zuanshi"))
		{
			this.m_XiaoHao.transform.GetChild(4).gameObject.SetActive(false);
		}
	}

	public int ChongZhiJiFen
	{
		get
		{
			return this.m_ChongZhiJiFen;
		}
		set
		{
			this.m_ChongZhiJiFen = value;
			if (this.m_ChongZhiJiMax > this.m_ChongZhiJiFen)
			{
				this.m_XiaoHaoNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					string.Format("{0}", Global.GetLang(this.m_ChongZhiJiMax.ToString()))
				});
			}
			else
			{
				this.m_XiaoHaoNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					string.Format("{0}", Global.GetLang(this.m_ChongZhiJiMax.ToString()))
				});
			}
			this.mShengyuNum.text = string.Format(Global.GetLang("充值      {0}可获得{1}每日积分"), this.strArr[0], this.strArr[1]) + string.Format(Global.GetLang("          现有每日积分：{0}"), (this.m_ChongZhiJiFen < 0) ? 0 : this.m_ChongZhiJiFen);
		}
	}

	public void SetListBox(int inCount = 5)
	{
		this.mJiangliListBox.transform.localPosition = new Vector3(this.mJiangliListBox.transform.localPosition.x + (float)((5 - inCount) * 50), this.mJiangliListBox.transform.localPosition.y, -1f);
		this.mQianggouGood.transform.localPosition = new Vector3(this.mQianggouGood.transform.localPosition.x + (float)((5 - inCount) * 50), this.mQianggouGood.transform.localPosition.y, -1f);
	}

	public void InitItemData(UIType MUIType, ZhuanXiangType mZhuanXiangType, EveryDayActInfo mSpecActInfo, EveryDayActivity mEveryDayActivityXML, string Price = "")
	{
		this.m_EveryDayActivityXML = mEveryDayActivityXML;
		this.mTitle.text = mEveryDayActivityXML.Name;
		this.ActID = mSpecActInfo.ActID;
		this.m_Price = Price;
		if (null != this.m_BackImg.gameObject.GetComponent<UITexture>())
		{
			this.m_BackImg.URL = string.Format("NetImages/GameRes/Images/Images/zhuanxiang{0}.jpg", 1);
		}
		if (null != this.m_PeiTuImg.gameObject.GetComponent<UITexture>())
		{
			this.m_PeiTuImg.URL = string.Format("NetImages/GameRes/Images/EveryDayHuodong/PeiTuType{0}.png", mEveryDayActivityXML.GoalType);
		}
		switch (MUIType)
		{
		case UIType.Qianggou:
			NGUITools.SetActive(this.mOtherPnl.gameObject, false);
			NGUITools.SetActive(this.mQianggouPnl.gameObject, true);
			this.btnConfim.Text = Global.GetLang("购 买");
			break;
		case UIType.Other:
			NGUITools.SetActive(this.mOtherPnl.gameObject, true);
			NGUITools.SetActive(this.mQianggouPnl.gameObject, false);
			this.btnConfim.Text = Global.GetLang("领 取");
			break;
		case UIType.ChongZhi:
			NGUITools.SetActive(this.mOtherPnl.gameObject, true);
			NGUITools.SetActive(this.mTiaojianLable.gameObject, false);
			NGUITools.SetActive(this.mQianggouPnl.gameObject, false);
			NGUITools.SetActive(this.m_XiaoHao, true);
			this.btnConfim.Text = Global.GetLang("兑 换");
			break;
		case UIType.ZhiGou:
			NGUITools.SetActive(this.mOtherPnl.gameObject, false);
			NGUITools.SetActive(this.mQianggouPnl.gameObject, true);
			NGUITools.SetActive(this.m_Old.gameObject, false);
			NGUITools.SetActive(this.m_New.gameObject, false);
			NGUITools.SetActive(this.m_ZhiGouPni.gameObject, true);
			this.btnConfim.Text = Global.GetLang("购 买");
			break;
		}
		this.MUIType = MUIType;
		this.SetLingquState(mSpecActInfo);
		switch (mZhuanXiangType)
		{
		case ZhuanXiangType.XianshiQianggou:
			this.mShengyuNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("个人限购：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format(Global.GetLang("{0}"), (mSpecActInfo.LeftPurNum < 0) ? 0 : mSpecActInfo.LeftPurNum)
			});
			if (mEveryDayActivityXML.Price.Split(new char[]
			{
				'|'
			}).Length == 2)
			{
				this.mNewPriceLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					string.Format("{0}", mEveryDayActivityXML.Price.Split(new char[]
					{
						'|'
					})[0])
				});
			}
			break;
		case ZhuanXiangType.ChongzhiDuihuan:
			this.strArr = ConfigSystemParam.GetSystemParamStringArrayByName("SpecialChongZhiDuiHuan", ':');
			if (this.strArr.Length == 2)
			{
				this.mTitle.text = mEveryDayActivityXML.Name;
			}
			this.mShengyuNum.text = string.Format(Global.GetLang("充值      {0}可获得{1}每日积分"), this.strArr[0], this.strArr[1]) + string.Format(Global.GetLang("          现有每日积分：{0}"), (mSpecActInfo.ShowNum < 0) ? 0 : mSpecActInfo.ShowNum);
			if (mEveryDayActivityXML.Price.Split(new char[]
			{
				'|'
			}).Length == 1)
			{
				this.m_ChongZhiJiMax = int.Parse(mEveryDayActivityXML.Price.Split(new char[]
				{
					'|'
				})[0]);
				this.m_XiaoHaoText1.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}", Global.GetLang("消耗："))
				});
				this.m_XiaoHaoText2.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format("{0}", Global.GetLang("每日积分"))
				});
				this.m_XianGouYiCi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					string.Format(Global.GetLang("个人限兑{0}次"), (mSpecActInfo.LeftPurNum < 0) ? 0 : mSpecActInfo.LeftPurNum)
				});
				if (int.Parse(mEveryDayActivityXML.Price.Split(new char[]
				{
					'|'
				})[0]) > mSpecActInfo.ShowNum)
				{
					this.m_XiaoHaoNum.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ff0000",
						string.Format("{0}", Global.GetLang(mEveryDayActivityXML.Price.Split(new char[]
						{
							'|'
						})[0]))
					});
				}
				else
				{
					this.m_XiaoHaoNum.text = Global.GetColorStringForNGUIText(new object[]
					{
						"fffffe",
						string.Format("{0}", Global.GetLang(mEveryDayActivityXML.Price.Split(new char[]
						{
							'|'
						})[0]))
					});
				}
			}
			break;
		case ZhuanXiangType.LeijiXiaofei:
		{
			NGUITools.SetActive(this.m_LeiJiXiaoFeizuanshi.gameObject, true);
			int num = mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_Str_Awardterm + string.Format(Global.GetLang("累计消费达到      {0}（{1} / {2}）"), num, (mSpecActInfo.ShowNum < num) ? mSpecActInfo.ShowNum : num, num)
			});
			this.XiaoFeiTiaojian = num;
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		}
		case ZhuanXiangType.ZhijieLingqu:
			NGUITools.SetActive(this.mTiaojianLable.gameObject, false);
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.ZhuanshengLevel:
			if (mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			}).Length == 2)
			{
				this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Str_Awardterm + string.Format(Global.GetLang("转生等级达到{0}转{1}级"), mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[0], mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[1])
				});
			}
			else
			{
				this.mTiaojianLable.text = Global.GetLang("暂无条件！");
			}
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.ChibangLevel:
			if (mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			}).Length == 2)
			{
				this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Str_Awardterm + string.Format(Global.GetLang("翅膀等级达到{0}阶{1}级"), mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[0], mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[1])
				});
			}
			else
			{
				this.mTiaojianLable.text = Global.GetLang("暂无条件！");
			}
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.VIPlevel:
		{
			int num2 = mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_Str_Awardterm + string.Format(Global.GetLang("VIP等级达到{0}级（{1} / {2}）"), num2, (Global.GetVIPLeve() < num2) ? Global.GetVIPLeve() : num2, num2)
			});
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		}
		case ZhuanXiangType.ChengjiuLevel:
		{
			int num3 = mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_Str_Awardterm + string.Format(Global.GetLang("成就等级达到{0}（{1} / {2}）"), this.GetChengjiuName(num3), (mSpecActInfo.ShowNum < num3) ? mSpecActInfo.ShowNum : num3, num3)
			});
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		}
		case ZhuanXiangType.JunxianLevel:
		{
			int num4 = mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			})[0].SafeToInt32(0);
			this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_Str_Awardterm + string.Format(Global.GetLang("军衔等级达到{0}（{1} / {2}）"), this.GetJunxianName(num4), (mSpecActInfo.ShowNum < num4) ? mSpecActInfo.ShowNum : num4, num4)
			});
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		}
		case ZhuanXiangType.MeilinLevel:
			if (mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			}).Length == 2)
			{
				this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Str_Awardterm + string.Format(Global.GetLang("梅林之书等级达到{0}阶{1}级"), mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[0], mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[1])
				});
			}
			else
			{
				this.mTiaojianLable.text = Global.GetLang("暂无条件！");
			}
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.ShengwuLevel:
			this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_Str_Awardterm + string.Format(Global.GetLang("圣物总等级达到{0}级"), mEveryDayActivityXML.GoalNum.Split(new char[]
				{
					','
				})[0])
			});
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.HunjieLevel:
			if (mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			}).Length == 2)
			{
				this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Str_Awardterm + string.Format(Global.GetLang("婚戒等级达到{0}阶{1}级"), mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[0], mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[1])
				});
			}
			else
			{
				this.mTiaojianLable.text = Global.GetLang("暂无条件！");
			}
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.ShouhushenLevel:
			if (mEveryDayActivityXML.GoalNum.Split(new char[]
			{
				','
			}).Length == 2)
			{
				this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					this.m_Str_Awardterm + string.Format(Global.GetLang("守护神等级达到{0}阶{1}级"), mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[0], mEveryDayActivityXML.GoalNum.Split(new char[]
					{
						','
					})[1])
				});
			}
			else
			{
				this.mTiaojianLable.text = Global.GetLang("暂无条件！");
			}
			NGUITools.SetActive(this.mShengyuNum.gameObject, false);
			break;
		case ZhuanXiangType.ChaoJiZhiGou:
			this.mShengyuNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("个人限购：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format(Global.GetLang("{0}"), (mSpecActInfo.LeftPurNum < 0) ? 0 : mSpecActInfo.LeftPurNum)
			});
			if (mEveryDayActivityXML.Price.Split(new char[]
			{
				'|'
			}).Length == 3)
			{
				this.m_ChongZhiID = int.Parse(mEveryDayActivityXML.Price.Split(new char[]
				{
					'|'
				})[1]);
				this.m_ZhiGouID = int.Parse(mEveryDayActivityXML.Price.Split(new char[]
				{
					'|'
				})[2]);
				this.m_NewPriceZhiGou.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang(string.Format("{0}", Price))
				});
			}
			break;
		}
	}

	private void SetLingquState(EveryDayActInfo mSpecActInfo)
	{
		this.MLingquState = (LingquState)mSpecActInfo.State;
	}

	public UIType MUIType
	{
		get
		{
			return this.mUIType;
		}
		set
		{
			this.mUIType = value;
			switch (value)
			{
			case UIType.Qianggou:
				NGUITools.SetActive(this.mOtherPnl.gameObject, false);
				NGUITools.SetActive(this.mQianggouPnl.gameObject, true);
				this.btnConfim.Text = Global.GetLang("购 买");
				break;
			case UIType.Other:
				NGUITools.SetActive(this.mOtherPnl.gameObject, true);
				NGUITools.SetActive(this.mQianggouPnl.gameObject, false);
				this.btnConfim.Text = Global.GetLang("领 取");
				break;
			case UIType.ChongZhi:
				NGUITools.SetActive(this.mOtherPnl.gameObject, true);
				NGUITools.SetActive(this.mTiaojianLable.gameObject, false);
				NGUITools.SetActive(this.mQianggouPnl.gameObject, false);
				NGUITools.SetActive(this.m_XiaoHao, true);
				this.btnConfim.Text = Global.GetLang("兑 换");
				break;
			case UIType.ZhiGou:
				NGUITools.SetActive(this.mOtherPnl.gameObject, false);
				NGUITools.SetActive(this.mQianggouPnl.gameObject, true);
				this.btnConfim.Text = Global.GetLang("购 买");
				break;
			}
		}
	}

	public LingquState MLingquState
	{
		get
		{
			return this.mLingquState;
		}
		set
		{
			this.mLingquState = value;
			switch (value + 1)
			{
			case LingquState.CanGain:
				this.btnConfim.isEnabled = false;
				NGUITools.SetActive(this.mYilingqu.gameObject, false);
				break;
			case LingquState.Gained:
				this.btnConfim.isEnabled = true;
				NGUITools.SetActive(this.mYilingqu.gameObject, false);
				break;
			case (LingquState)2:
				this.btnConfim.isEnabled = false;
				if (this.mUIType == UIType.Other)
				{
					NGUITools.SetActive(this.mYilingqu.gameObject, true);
					NGUITools.SetActive(this.btnConfim.gameObject, false);
				}
				else if (this.mUIType == UIType.Qianggou || this.mUIType == UIType.ZhiGou)
				{
					NGUITools.SetActive(this.m_YiGouMai.gameObject, true);
					NGUITools.SetActive(this.btnConfim.gameObject, false);
				}
				break;
			}
		}
	}

	public void SetShengyuNum(int mActID, int leftPurNum, int ShowNum1, int ShowNum2)
	{
		if (this.m_EveryDayActivityXML.GoalType == 1 && leftPurNum <= 0)
		{
			this.MLingquState = LingquState.Gained;
		}
		else if (this.m_EveryDayActivityXML.GoalType == 2 && leftPurNum <= 0)
		{
			this.MLingquState = LingquState.Gained;
		}
		else if (this.m_EveryDayActivityXML.GoalType == 14 && leftPurNum <= 0)
		{
			this.MLingquState = LingquState.Gained;
		}
		else if (this.m_EveryDayActivityXML.GoalType != 1 && this.m_EveryDayActivityXML.GoalType != 14 && this.m_EveryDayActivityXML.GoalType != 2)
		{
			this.MLingquState = LingquState.Gained;
		}
		ZhuanXiangType goalType = (ZhuanXiangType)this.m_EveryDayActivityXML.GoalType;
		switch (goalType)
		{
		case ZhuanXiangType.XianshiQianggou:
			this.mShengyuNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("个人限购：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fffffe",
				string.Format(Global.GetLang("{0}"), (leftPurNum < 0) ? 0 : leftPurNum)
			});
			break;
		case ZhuanXiangType.ChongzhiDuihuan:
			this.m_XianGouYiCi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format(Global.GetLang("个人限兑{0}次"), (leftPurNum < 0) ? 0 : leftPurNum)
			});
			this.mShengyuNum.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				string.Format(Global.GetLang("充值      {0}可获得{1}每日积分          现有每日积分：{2}"), this.strArr[0], this.strArr[1], (ShowNum1 < 0) ? 0 : ShowNum1)
			});
			break;
		case ZhuanXiangType.LeijiXiaofei:
			this.mTiaojianLable.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				this.m_Str_Awardterm + string.Format(Global.GetLang("累计消费达到      {0}（{1} / {2}）"), this.XiaoFeiTiaojian, (ShowNum1 < this.XiaoFeiTiaojian) ? ShowNum1 : this.XiaoFeiTiaojian, this.XiaoFeiTiaojian)
			});
			break;
		default:
			if (goalType == ZhuanXiangType.ChaoJiZhiGou)
			{
				this.mShengyuNum.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("个人限购：")
				}) + Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					string.Format(Global.GetLang("{0}"), (leftPurNum < 0) ? 0 : leftPurNum)
				});
			}
			break;
		}
	}

	private string GetChengjiuName(int chengjiuLevel)
	{
		string result = string.Empty;
		switch (chengjiuLevel)
		{
		case 1:
			result = Global.GetLang("守护者");
			break;
		case 2:
			result = Global.GetLang("先锋者");
			break;
		case 3:
			result = Global.GetLang("无畏者");
			break;
		case 4:
			result = Global.GetLang("讨伐者");
			break;
		case 5:
			result = Global.GetLang("不败者");
			break;
		case 6:
			result = Global.GetLang("至高者");
			break;
		case 7:
			result = Global.GetLang("屠戮者");
			break;
		case 8:
			result = Global.GetLang("终结者");
			break;
		case 9:
			result = Global.GetLang("毁灭者");
			break;
		case 10:
			result = Global.GetLang("征服者");
			break;
		case 11:
			result = Global.GetLang("统治者");
			break;
		case 12:
			result = Global.GetLang("救世主");
			break;
		default:
			result = Global.GetLang("暂无成就");
			break;
		}
		return result;
	}

	private string GetJunxianName(int chengjiuLevel)
	{
		string result = string.Empty;
		switch (chengjiuLevel)
		{
		case 1:
			result = Global.GetLang("列兵");
			break;
		case 2:
			result = Global.GetLang("下士");
			break;
		case 3:
			result = Global.GetLang("中士");
			break;
		case 4:
			result = Global.GetLang("军士");
			break;
		case 5:
			result = Global.GetLang("骑士");
			break;
		case 6:
			result = Global.GetLang("中尉");
			break;
		case 7:
			result = Global.GetLang("少校");
			break;
		case 8:
			result = Global.GetLang("中将");
			break;
		case 9:
			result = Global.GetLang("司令");
			break;
		case 10:
			result = Global.GetLang("统帅");
			break;
		case 11:
			result = Global.GetLang("督军");
			break;
		case 12:
			result = Global.GetLang("元帅");
			break;
		default:
			result = Global.GetLang("暂无军衔");
			break;
		}
		return result;
	}

	public TextBlock mTitle;

	public TextBlock mTime;

	public TextBlock mShengyuNum;

	public GButton btnConfim;

	public UISprite mYilingqu;

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage m_BackImg;

	public ShowNetImage m_PeiTuImg;

	public UISprite m_LeiJiXiaoFeizuanshi;

	public UISprite m_YiGouMai;

	public GameObject m_XiaoHao;

	public UILabel m_XiaoHaoText1;

	public UILabel m_XiaoHaoText2;

	public UILabel m_XiaoHaoNum;

	public UILabel m_XianGouYiCi;

	public string[] strArr = new string[]
	{
		string.Empty,
		string.Empty
	};

	public GameObject mQianggouPnl;

	public ListBox mQianggouGood;

	public TextBlock mNewPriceLabel;

	public GameObject m_Old;

	public GameObject m_New;

	public UILabel m_OldPriceZhiGou;

	public UILabel m_NewPriceZhiGou;

	public GameObject m_ZhiGouPni;

	public GameObject m_HongXian;

	public GameObject mOtherPnl;

	public TextBlock mTiaojianLable;

	public ListBox mJiangliListBox;

	public ObservableCollection jiangliListBoxOBC;

	public ObservableCollection qianggouListBoxOBC;

	private string m_Price = string.Empty;

	private int m_ChongZhiID;

	private int m_ZhiGouID;

	public int m_ChongZhiJiFen;

	public int m_ChongZhiJiMax;

	private int ActID;

	private int XiaoFeiTiaojian;

	private string m_Str_Awardterm = Global.GetLang("领取条件：");

	private EveryDayActivity m_EveryDayActivityXML = default(EveryDayActivity);

	private UIType mUIType;

	private LingquState mLingquState;
}
