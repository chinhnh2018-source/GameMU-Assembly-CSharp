using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class AoYunDaTiPart_DaTi : UserControl
{
	private void InitTextInPrefabs()
	{
		this.DaTiTime.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("答题时间")
		});
		this.JiFen.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("积分：")
		});
		this.YongtuTianShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("用途："),
			"dac7ae",
			Global.GetLang("去掉两个错误答案")
		});
		this.YongtuEMo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("用途："),
			"dac7ae",
			Global.GetLang("本题活动积分翻倍")
		});
		this.BtnTianShiShiYong.Label.text = Global.GetLang("使用");
		this.BtnEMoShiYong.Label.text = Global.GetLang("使用");
		this.BtnSure.Label.text = Global.GetLang("确定");
		this.BtnSure.gameObject.SetActive(false);
		this.BtnLingQuAward.gameObject.SetActive(false);
		this.BtnLingQuAward.Label.text = Global.GetLang("领取奖励");
		this.ShuangBeiJiFen.gameObject.SetActive(false);
		this.BG.URL = "NetImages/GameRes/Images/Plate/AoYunDiTu.png.qj";
		this.TianShi.URL = "NetImages/GameRes/Images/Plate/TianShi.png.qj";
		this.EMo.URL = "NetImages/GameRes/Images/Plate/EMo.png.qj";
		for (int i = 0; i < this.TiHao.Length; i++)
		{
			this.TiHao[i].gameObject.SetActive(false);
			this.TiHaoNum[i].text = Global.GetColorStringForNGUIText(new object[]
			{
				"4c3e27",
				i + 1
			});
		}
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("QuestionItem", '|');
		for (int j = 0; j < systemParamStringArrayByName.Length; j++)
		{
			string[] array = systemParamStringArrayByName[j].Split(new char[]
			{
				','
			});
			this.shangxian[j] = int.Parse(array[1]);
		}
	}

	private void GetInitTime()
	{
		XElement gameResXml = Global.GetGameResXml("Config/QuestionBank.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "QuestionBank");
		this.datiTime = Global.GetXElementAttributeInt(xelementList[0], "ExamTime");
		this.waitTime = Global.GetXElementAttributeInt(xelementList[0], "WaitTime");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.GetInitTime();
		this.InitCheck();
		this.BtnTianShiGouMai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.tianshishengyuNum >= this.shangxian[0])
			{
				Super.HintMainText(Global.GetLang("已达上限"), 10, 3);
				return;
			}
			PlayZone.GlobalPlayZone.OpenBuyWindow(true, this.tianshishengyuNum);
		};
		this.BtnEMogouMai.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.emoshengyuNum >= this.shangxian[1])
			{
				Super.HintMainText(Global.GetLang("已达上限"), 10, 3);
				return;
			}
			PlayZone.GlobalPlayZone.OpenBuyWindow(false, this.emoshengyuNum);
		};
		this.BtnTianShiShiYong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendAoYunUseGoodsData(0);
			Super.ShowNetWaiting(null);
		};
		this.BtnEMoShiYong.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendAoYunUseGoodsData(1);
			Super.ShowNetWaiting(null);
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSHandler(this, new DPSelectedItemEventArgs());
			GameInstance.Game.SendAoYunMainData();
		};
		this.BtnSure.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.answerId == -2)
			{
				Super.HintMainText(Global.GetLang("请先选择答案!"), 10, 3);
				return;
			}
			GameInstance.Game.SendAoYunAnswerQuestionData(this.answerId);
			this.BtnSure.isEnabled = false;
			for (int i = 0; i < this.Answers.Length; i++)
			{
				this.Answers[i].GetComponent<BoxCollider>().enabled = false;
			}
			this.BtnTianShiShiYong.isEnabled = false;
			this.BtnEMoShiYong.isEnabled = false;
		};
		this.BtnLingQuAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.isGet)
			{
				GameInstance.Game.SendAoYunGetAwardData();
				Super.ShowNetWaiting(null);
			}
			else
			{
				this.DPSHandler(this, new DPSelectedItemEventArgs());
				GameInstance.Game.SendAoYunMainData();
			}
		};
		GameInstance.Game.SendAoYunGetQuestionData();
		Super.ShowNetWaiting(null);
	}

	private void SelectRadioMoney(int index)
	{
		for (int i = 0; i < this.Answers.Length; i++)
		{
			if (i == index)
			{
				this.Answers[i].Check = true;
				this.answerId = index;
				if (!this.isWaitingTime)
				{
					this.BtnSure.gameObject.SetActive(true);
					this.BtnSure.isEnabled = true;
					this.BtnSure.Label.text = Global.GetLang("确定");
				}
			}
			else
			{
				this.Answers[i].Check = false;
			}
		}
	}

	private void InitCheck()
	{
		if (null != this.Answers[0])
		{
			this.Answers[0].CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SelectRadioMoney(0);
			};
		}
		if (null != this.Answers[1])
		{
			this.Answers[1].CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SelectRadioMoney(1);
			};
		}
		if (null != this.Answers[2])
		{
			this.Answers[2].CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SelectRadioMoney(2);
			};
		}
		if (null != this.Answers[3])
		{
			this.Answers[3].CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SelectRadioMoney(3);
			};
		}
	}

	public void ServerKeLingQuAward(int res)
	{
		this.BtnSure.gameObject.SetActive(false);
		this.BtnLingQuAward.gameObject.SetActive(true);
		this.BtnLingQuAward.isEnabled = true;
		if (res == 1)
		{
			this.isGet = true;
		}
		else
		{
			this.isGet = false;
			this.BtnLingQuAward.Label.text = Global.GetLang("关闭");
		}
	}

	public void ServerDaTiResultDataManager(AoyunQuestionAward data)
	{
		while (this.currentnumber > 10)
		{
			this.currentnumber -= 10;
		}
		if (data.Result == -1)
		{
			this.TiHao[this.currentnumber - 1].spriteName = "dacuobiaoshi";
		}
		else
		{
			this.TiHao[this.currentnumber - 1].spriteName = "daduibiaoshi";
		}
		this.JieGuo.gameObject.SetActive(true);
		if (data.Result == -1)
		{
			this.JieGuo.spriteName = "huidacuowu";
		}
		else
		{
			this.JieGuo.spriteName = "huidazhengque";
		}
		string color = "17e43e";
		this.SetTextColor(data.RightAnswer, color);
		this.ServerBuyGoodsDataManager(0, data.TianShiCount);
		this.ServerBuyGoodsDataManager(1, data.EMoCount);
		this.JiFenLab.text = data.RolePoint.ToString();
	}

	public void ServerBuyGoodsDataManager(int goodsType, int num)
	{
		if (goodsType == 0)
		{
			this.XianYouTianShi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("现有："),
				"dac7ae",
				num
			});
			this.tianshishengyuNum = num;
		}
		else if (goodsType == 1)
		{
			this.XianYouEMo.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("现有："),
				"dac7ae",
				num
			});
			this.emoshengyuNum = num;
		}
	}

	public void ServerUseGoodsDataManager(int wr1, int wr2, int type, int num)
	{
		if (wr1 != -1)
		{
			this.SetTextColor(wr1, "313131");
		}
		if (wr2 != -1)
		{
			this.SetTextColor(wr2, "313131");
		}
		if (type == 0)
		{
			this.XianYouTianShi.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("现有："),
				"dac7ae",
				num
			});
			this.tianshishengyuNum = num;
			this.BtnTianShiShiYong.isEnabled = false;
		}
		else if (type == 1)
		{
			this.XianYouEMo.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("现有："),
				"dac7ae",
				num
			});
			this.emoshengyuNum = num;
			this.BtnEMoShiYong.isEnabled = false;
			this.ShuangBeiJiFen.gameObject.SetActive(true);
		}
	}

	private void SetTextColor(int wr, string color = "313131")
	{
		if (wr == 0)
		{
			string text = Super.ClearStringColor(this.Daan1.text);
			this.Daan1.text = Global.GetColorStringForNGUIText(new object[]
			{
				color,
				text
			});
			this.Answers[0].GetComponent<BoxCollider>().enabled = false;
		}
		else if (wr == 1)
		{
			string text2 = Super.ClearStringColor(this.Daan2.text);
			this.Daan2.text = Global.GetColorStringForNGUIText(new object[]
			{
				color,
				text2
			});
			this.Answers[1].GetComponent<BoxCollider>().enabled = false;
		}
		else if (wr == 2)
		{
			string text3 = Super.ClearStringColor(this.Daan3.text);
			this.Daan3.text = Global.GetColorStringForNGUIText(new object[]
			{
				color,
				text3
			});
			this.Answers[2].GetComponent<BoxCollider>().enabled = false;
		}
		else if (wr == 3)
		{
			string text4 = Super.ClearStringColor(this.Daan4.text);
			this.Daan4.text = Global.GetColorStringForNGUIText(new object[]
			{
				color,
				text4
			});
			this.Answers[3].GetComponent<BoxCollider>().enabled = false;
		}
	}

	public void ServerDataManager(QuestionItemData data)
	{
		if (data == null)
		{
			return;
		}
		this.StopTimer();
		this.InitTime();
		this.BtnSure.gameObject.SetActive(false);
		this.JieGuo.gameObject.SetActive(false);
		this.ShuangBeiJiFen.gameObject.SetActive(false);
		for (int i = 0; i < this.Answers.Length; i++)
		{
			this.Answers[i].GetComponent<BoxCollider>().enabled = true;
		}
		this.BtnTianShiShiYong.isEnabled = !data.UseTianShi;
		this.BtnEMoShiYong.isEnabled = !data.UseEMo;
		this.SelectRadioMoney(data.RoleAnswer);
		if (data.RoleAnswer != -1)
		{
			this.BtnTianShiShiYong.isEnabled = false;
			this.BtnEMoShiYong.isEnabled = false;
			this.BtnSure.gameObject.SetActive(true);
			this.BtnSure.isEnabled = false;
			for (int j = 0; j < this.Answers.Length; j++)
			{
				this.Answers[j].GetComponent<BoxCollider>().enabled = false;
			}
		}
		this.isWaitingTime = false;
		this.TiNum.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("第{0}题"), data.QuestionId)
		});
		this.TimuNeiRong.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			data.Question
		});
		this.Daan1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.AnswerContentArray[0]
		});
		this.Daan2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.AnswerContentArray[1]
		});
		this.Daan3.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.AnswerContentArray[2]
		});
		this.Daan4.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			data.AnswerContentArray[3]
		});
		if (data.UseTianShi)
		{
			GameInstance.Game.SendAoYunUseGoodsData(0);
		}
		if (data.UseEMo)
		{
			GameInstance.Game.SendAoYunUseGoodsData(1);
		}
		if (data.EndTime.Ticks < Global.GetCorrectDateTime().Ticks)
		{
			this.countdowntimes = 0;
		}
		else
		{
			this.countdowntimes = (int)((data.EndTime.Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L);
		}
		this.currentEndTime = data.EndTime.Ticks;
		int k = data.QuestionId;
		int num = 0;
		this.currentnumber = data.QuestionId;
		while (k > 10)
		{
			k -= 10;
			num++;
		}
		for (int l = 0; l < this.TiHaoNum.Length; l++)
		{
			this.TiHaoNum[l].text = Global.GetColorStringForNGUIText(new object[]
			{
				"4c3e27",
				l + 1 + 10 * num
			});
		}
		if (data.QuestionId > 10)
		{
			for (int m = 0; m < this.TiHaoNum.Length; m++)
			{
				this.TiHao[m].gameObject.SetActive(false);
			}
		}
		this.DatiJinDuTiao.transform.localScale = new Vector3((float)(38 * (k - 1)), 4f, 1f);
		this.CurrentTiHight.transform.localPosition = new Vector3((float)(38 * (k - 1)) + 95.8f, 117f, -0.3f);
		this.TiHaoNum[k - 1].text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			Super.ClearStringColor(this.TiHaoNum[k - 1].text)
		});
		for (int n = 0; n < k; n++)
		{
			this.TiHao[n].gameObject.SetActive(true);
			if (data.QuestionState[n + 10 * num])
			{
				this.TiHao[n].spriteName = "daduibiaoshi";
			}
			else
			{
				this.TiHao[n].spriteName = "dacuobiaoshi";
			}
		}
		this.JiFenLab.text = data.RolePoint.ToString();
	}

	public void MainDataManager(int tianshi, int emo)
	{
		this.XianYouTianShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("现有："),
			"dac7ae",
			tianshi
		});
		this.XianYouEMo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("现有："),
			"dac7ae",
			emo
		});
		this.tianshishengyuNum = tianshi;
		this.emoshengyuNum = emo;
	}

	private void InitTime()
	{
		this.TimeJinDuTiao.GetComponent<UISprite>().spriteName = "luyinjindutiao_green";
		this.TimeJinDuTiao.transform.localScale = new Vector3(0f, 16f, 1f);
		this.StartUITimer();
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("AoYunDatiPart_Timer");
		this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.UITimer.Start();
	}

	protected void StartUITimer2()
	{
		this.UITimer = new DispatcherTimer("AoYunDatiPart_Timer2");
		this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick2);
		this.UITimer.Start();
	}

	private void StopTimer()
	{
		if (this.UITimer != null)
		{
			this.UITimer.Tick = null;
			this.UITimer.Stop();
			this.UITimer = null;
		}
	}

	protected void UITimer_Tick(object sender, object e)
	{
		if (this.countdowntimes > 0)
		{
			if (this.countdowntimes > 10)
			{
				this.CountTime.text = string.Format(Global.GetLang("00:{0}"), this.countdowntimes);
			}
			else
			{
				this.CountTime.text = Global.GetColorStringForNGUIText(new object[]
				{
					"FF0000",
					string.Format(Global.GetLang("00:{0}"), this.countdowntimes)
				});
				this.TimeJinDuTiao.GetComponent<UISprite>().spriteName = "luyinjindutiao_red";
			}
			this.countdowntimes--;
		}
		else
		{
			this.CountTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				string.Format(Global.GetLang("00:00"), new object[0])
			});
			this.BtnSure.gameObject.SetActive(true);
			this.BtnSure.isEnabled = false;
			this.BtnTianShiShiYong.isEnabled = false;
			this.BtnEMoShiYong.isEnabled = false;
			this.StopTimer();
			this.StartUITimer2();
			this.isWaitingTime = true;
			this.lasttimes = this.waitTime - (int)(Global.GetCorrectDateTime().Ticks - this.currentEndTime) / 10000000 - 1;
		}
		this.TimeJinDuTiao.transform.localScale = new Vector3((26.68f * (float)this.countdowntimes >= 0f) ? (26.68f * (float)this.countdowntimes) : 0f, 16f, 1f);
		float num = 268f - 25.52f * (float)(this.datiTime - this.countdowntimes);
		this.TimeShaLou.transform.localPosition = new Vector3(num, 184f, -0.2f);
		float num2 = 283f - 25.52f * (float)(this.datiTime - this.countdowntimes);
		this.CountTime.transform.localPosition = new Vector3(num2, 161f, -0.2f);
	}

	protected void UITimer_Tick2(object sender, object e)
	{
		if (this.lasttimes > 0)
		{
			this.BtnSure.Label.text = string.Format(Global.GetLang("{0}s"), this.lasttimes);
			this.lasttimes--;
		}
		else
		{
			this.BtnSure.Label.text = Global.GetLang("0s");
			this.StopTimer();
		}
	}

	public override void Destroy()
	{
		this.StopTimer();
	}

	public DPSelectedItemEventHandler DPSHandler;

	public UILabel DaTiTime;

	public UILabel CountTime;

	public UILabel JiFen;

	public UILabel JiFenLab;

	public UILabel TiNum;

	public UILabel TimuNeiRong;

	public UILabel Daan1;

	public UILabel Daan2;

	public UILabel Daan3;

	public UILabel Daan4;

	public UILabel YongtuTianShi;

	public UILabel XianYouTianShi;

	public UILabel YongtuEMo;

	public UILabel XianYouEMo;

	public UILabel[] TiHaoNum;

	public GButton BtnTianShiGouMai;

	public GButton BtnEMogouMai;

	public GButton BtnTianShiShiYong;

	public GButton BtnEMoShiYong;

	public GButton BtnClose;

	public GButton BtnSure;

	public GButton BtnLingQuAward;

	public GCheckBox[] Answers;

	public UISprite TimeJinDuTiao;

	public UISprite TimeShaLou;

	public UISprite DatiJinDuTiao;

	public UISprite CurrentTiHight;

	public UISprite JieGuo;

	public UISprite[] TiHao = new UISprite[10];

	public UISprite ShuangBeiJiFen;

	public ShowNetImage BG;

	public ShowNetImage TianShi;

	public ShowNetImage EMo;

	private int countdowntimes = 25;

	private int lasttimes = 4;

	private int tianshishengyuNum;

	private int emoshengyuNum;

	private int answerId = -2;

	private bool isGet;

	private int datiTime = 25;

	private int waitTime = 5;

	private long currentEndTime;

	private int currentnumber;

	private int[] shangxian = new int[2];

	private bool isWaitingTime;

	private DispatcherTimer UITimer;
}
