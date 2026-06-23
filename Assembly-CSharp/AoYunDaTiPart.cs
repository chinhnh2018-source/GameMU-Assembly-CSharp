using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class AoYunDaTiPart : UserControl
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
		this.HuodongShuoMing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("活动说明")
		});
		this.HuodongShiJian.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动时间")
		});
		this.GuiZeShuoMing.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("规则说明")
		});
		this.GuiZeShuoMingLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("答对每道题可获得经验、金币奖励，根据使用时间获得积分，活动结束时根据排名获得额外奖励。")
		});
		this.HuodongDaoJu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("活动道具")
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
		this.PaiHang.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("上期排行")
		});
		this.Name.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("玩家姓名")
		});
		this.JiFen.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("答题积分")
		});
		this.NoShuJu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("目前暂无任何排名信息")
		});
		this.NoShuJu.gameObject.SetActive(false);
		this.BtnTianShiGouMai.Label.text = Global.GetLang("购买");
		this.BtnEMogouMai.Label.text = Global.GetLang("购买");
		this.BtnDaTi.Label.text = Global.GetLang("开始答题");
		this.BG.URL = "NetImages/GameRes/Images/Plate/AoYunDiTu.png.qj";
		this.TianShi.URL = "NetImages/GameRes/Images/Plate/TianShi.png.qj";
		this.EMo.URL = "NetImages/GameRes/Images/Plate/EMo.png.qj";
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("QuestionItem", '|');
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			this.danjia[i] = int.Parse(array[0]);
			this.shangxian[i] = int.Parse(array[1]);
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.ListItem.ItemsSource;
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSHandler(this, new DPSelectedItemEventArgs());
		};
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
		this.BtnDaTi.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenDaTiWindow();
		};
		this.BtnAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OpenAwardWindow();
		};
		GameInstance.Game.SendAoYunMainData();
		Super.ShowNetWaiting(null);
	}

	public void AoYunDaTiBuyGoodsDataManager(int goodsType, int num)
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

	public void AoYunDaTiMainDataManager(AoyunDatiMainData mainData)
	{
		if (mainData == null)
		{
			return;
		}
		this.ItemCollection.Clear();
		if (mainData.AoyunPaiHangRoleDataArray == null || mainData.AoyunPaiHangRoleDataArray.Count == 0)
		{
			this.NoShuJu.gameObject.SetActive(true);
		}
		if (mainData.AoyunPaiHangRoleDataArray != null && mainData.AoyunPaiHangRoleDataArray.Count > 0)
		{
			this.NoShuJu.gameObject.SetActive(false);
			for (int i = 0; i < mainData.AoyunPaiHangRoleDataArray.Count; i++)
			{
				AoYunDaTiPartItem aoYunDaTiPartItem = U3DUtils.NEW<AoYunDaTiPartItem>();
				aoYunDaTiPartItem.SetPaiMing = mainData.AoyunPaiHangRoleDataArray[i].RoleRank;
				aoYunDaTiPartItem.SetName = mainData.AoyunPaiHangRoleDataArray[i].RoleName;
				aoYunDaTiPartItem.SetJiFen = mainData.AoyunPaiHangRoleDataArray[i].RolePoint;
				this.ItemCollection.AddNoUpdate(aoYunDaTiPartItem);
				UIPanel component = aoYunDaTiPartItem.GetComponent<UIPanel>();
				if (component)
				{
					Object.Destroy(component);
				}
			}
		}
		if (mainData.SelfRank == -1)
		{
			this.MyPaiMing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				Global.GetLang("无")
			});
		}
		else
		{
			this.MyPaiMing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				mainData.SelfRank.ToString()
			});
		}
		this.XianYouTianShi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("现有："),
			"dac7ae",
			mainData.TianShiNum
		});
		this.XianYouEMo.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("现有："),
			"dac7ae",
			mainData.EMoNum
		});
		string text = (mainData.StartTime.Minute >= 10) ? mainData.StartTime.Minute.ToString() : string.Format("0{0}", mainData.StartTime.Minute);
		string text2 = (mainData.EndTime.Minute >= 10) ? mainData.EndTime.Minute.ToString() : string.Format("0{0}", mainData.EndTime.Minute);
		string text3 = string.Format("{0}:{1}:00-{2}:{3}:00", new object[]
		{
			mainData.StartTime.Hour,
			text,
			mainData.EndTime.Hour,
			text2
		});
		this.huodongShiJianLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			text3
		});
		this.tianshishengyuNum = mainData.TianShiNum;
		this.emoshengyuNum = mainData.EMoNum;
		if (mainData.IsHaveAward)
		{
			GameInstance.Game.SendAoYunGetAwardData();
			Super.ShowNetWaiting(null);
		}
		long ticks = mainData.StartTime.Ticks;
		DateTime maxValue = DateTime.MaxValue;
		if (ticks == maxValue.Ticks)
		{
			this.BtnDaTi.gameObject.SetActive(false);
			this.HuodongTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("活动已结束")
			});
			this.huodongShiJianLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				Global.GetLang("已结束")
			});
			return;
		}
		if (Global.GetCorrectDateTime().Ticks < mainData.StartTime.Ticks)
		{
			long lTimeToStart = mainData.StartTime.Ticks - Global.GetCorrectDateTime().Ticks;
			this.InitTime(lTimeToStart);
		}
		else if (Global.GetCorrectDateTime().Ticks > mainData.StartTime.Ticks && Global.GetCorrectDateTime().Ticks < mainData.EndTime.Ticks)
		{
			this.BtnDaTi.gameObject.SetActive(true);
			this.HuodongTime.text = string.Empty;
		}
		else
		{
			long lTimeToStart2 = mainData.NextStartTime.Ticks - Global.GetCorrectDateTime().Ticks;
			this.InitTime(lTimeToStart2);
		}
	}

	private void InitTime(long lTimeToStart)
	{
		if (lTimeToStart == 0L)
		{
			this.BtnDaTi.gameObject.SetActive(true);
			return;
		}
		this.BtnDaTi.gameObject.SetActive(false);
		this.countdowntimes = lTimeToStart / 10000000L;
		this.StartUITimer();
	}

	protected void StartUITimer()
	{
		this.UITimer = new DispatcherTimer("AoYunDaTiPart_Timer");
		this.UITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.UITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
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
		if (this.countdowntimes > 0L)
		{
			this.HuodongTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				string.Format(Global.GetLang("距活动开启：{0}"), AoYunDaTiPart.GetTimeStrBySecEx((double)this.countdowntimes))
			});
			this.countdowntimes -= 1L;
		}
		else
		{
			this.HuodongTime.text = string.Empty;
			this.BtnDaTi.gameObject.SetActive(true);
			this.StopTimer();
		}
	}

	public override void Destroy()
	{
		this.StopTimer();
	}

	private static string GetTimeStrBySecEx(double sec)
	{
		int num = 86400;
		int num2 = 3600;
		int num3 = 60;
		int[] array3;
		string[] array4;
		if (sec > (double)num)
		{
			int[] array = new int[]
			{
				(int)(sec / (double)num),
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3)
			};
			string[] array2 = new string[]
			{
				Global.GetLang("天"),
				Global.GetLang("小时"),
				Global.GetLang("分")
			};
			array3 = array;
			array4 = array2;
		}
		else
		{
			int[] array5 = new int[]
			{
				(int)(sec / (double)num),
				(int)(sec % (double)num / (double)num2),
				(int)(sec % (double)num % (double)num2 / (double)num3),
				(int)(sec % (double)num % (double)num2 % (double)num3)
			};
			string[] array6 = new string[]
			{
				Global.GetLang("天"),
				Global.GetLang("小时"),
				Global.GetLang("分"),
				Global.GetLang("秒")
			};
			array3 = array5;
			array4 = array6;
		}
		List<int> list = Enumerable.ToList<int>(array3);
		List<string> list2 = Enumerable.ToList<string>(array4);
		while (list.Count > 0 && list[0] == 0)
		{
			list.RemoveAt(0);
			list2.RemoveAt(0);
		}
		string text = string.Empty;
		for (int i = 0; i < list.Count; i++)
		{
			text += list[i].ToString();
			text += list2[i];
		}
		return text;
	}

	public static Dictionary<int, AoYunAward> GetQuestionAwardDic()
	{
		if (AoYunDaTiPart.QuestionAwardDic.Count > 0)
		{
			return AoYunDaTiPart.QuestionAwardDic;
		}
		XElement gameResXml = Global.GetGameResXml("Config/QuestionAward.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "QuestionAward");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			AoYunAward aoYunAward = new AoYunAward();
			aoYunAward.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			aoYunAward.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			aoYunAward.GoodsOne = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
			if (!AoYunDaTiPart.QuestionAwardDic.ContainsKey(aoYunAward.ID))
			{
				AoYunDaTiPart.QuestionAwardDic.Add(aoYunAward.ID, aoYunAward);
			}
			i++;
		}
		return AoYunDaTiPart.QuestionAwardDic;
	}

	public static void ClearXMLData()
	{
		if (AoYunDaTiPart.QuestionAwardDic.Count > 0)
		{
			AoYunDaTiPart.QuestionAwardDic.Clear();
		}
	}

	private void OpenAwardWindow()
	{
		if (this.AwardWindow == null)
		{
			this.AwardWindow = U3DUtils.NEW<GChildWindow>();
			this.AwardWindow.IsShowModal = true;
			this.AwardWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.AwardWindow, Global.GetLang("奖励界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.AwardWindow);
		}
		if (this.AwardPart == null)
		{
			this.AwardPart = U3DUtils.NEW<AoYunDaTiPartAward>();
			this.AwardPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseAwardWindow();
			};
		}
		this.AwardWindow.SetContent(this.AwardWindow.BodyPresenter, this.AwardPart, 0.0, 0.0, true);
	}

	private void CloseAwardWindow()
	{
		if (null != this.AwardPart)
		{
			this.AwardPart.transform.parent = null;
			Object.Destroy(this.AwardPart.gameObject);
			this.AwardPart = null;
		}
		if (null != this.AwardWindow)
		{
			Super.CloseChildWindow(base.Children, this.AwardWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.AwardWindow, true);
			this.AwardWindow = null;
		}
	}

	private void OpenDaTiWindow()
	{
		if (this.DaTiWindow == null)
		{
			this.DaTiWindow = U3DUtils.NEW<GChildWindow>();
			this.DaTiWindow.IsShowModal = true;
			this.DaTiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.DaTiWindow, Global.GetLang("答题界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.DaTiWindow);
		}
		if (this.DaTiPart == null)
		{
			this.DaTiPart = U3DUtils.NEW<AoYunDaTiPart_DaTi>();
			this.DaTiPart.MainDataManager(this.tianshishengyuNum, this.emoshengyuNum);
			this.DaTiPart.DPSHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseDaTiWindow();
			};
		}
		this.DaTiWindow.SetContent(this.DaTiWindow.BodyPresenter, this.DaTiPart, 0.0, 0.0, true);
	}

	public void CloseDaTiWindow()
	{
		if (null != this.DaTiPart)
		{
			this.DaTiPart.transform.parent = null;
			Object.Destroy(this.DaTiPart.gameObject);
			this.DaTiPart = null;
		}
		if (null != this.DaTiWindow)
		{
			Super.CloseChildWindow(base.Children, this.DaTiWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.DaTiWindow, true);
			this.DaTiWindow = null;
		}
	}

	public DPSelectedItemEventHandler DPSHandler;

	public GButton BtnClose;

	public GButton BtnTianShiGouMai;

	public GButton BtnEMogouMai;

	public GButton BtnDaTi;

	public GButton BtnAward;

	public UILabel HuodongShuoMing;

	public UILabel HuodongShiJian;

	public UILabel huodongShiJianLab;

	public UILabel GuiZeShuoMing;

	public UILabel GuiZeShuoMingLab;

	public UILabel HuodongDaoJu;

	public UILabel YongtuTianShi;

	public UILabel XianYouTianShi;

	public UILabel YongtuEMo;

	public UILabel XianYouEMo;

	public UILabel PaiHang;

	public new UILabel Name;

	public UILabel JiFen;

	public UILabel MyPaiMing;

	public UILabel HuodongTime;

	public UILabel NoShuJu;

	public ListBox ListItem;

	public ShowNetImage BG;

	public ShowNetImage TianShi;

	public ShowNetImage EMo;

	private int[] danjia = new int[2];

	private int[] shangxian = new int[2];

	private ObservableCollection _ItemCollection;

	private long countdowntimes;

	private int tianshishengyuNum;

	private int emoshengyuNum;

	private DispatcherTimer UITimer;

	private static Dictionary<int, AoYunAward> QuestionAwardDic = new Dictionary<int, AoYunAward>();

	public GChildWindow AwardWindow;

	public AoYunDaTiPartAward AwardPart;

	public GChildWindow DaTiWindow;

	public AoYunDaTiPart_DaTi DaTiPart;
}
