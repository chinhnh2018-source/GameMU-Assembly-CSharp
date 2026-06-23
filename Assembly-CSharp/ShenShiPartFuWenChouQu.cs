using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenShiPartFuWenChouQu : UserControl
{
	public DateTime nextTime
	{
		get
		{
			return this.nexttime;
		}
		set
		{
			this.nexttime = value;
			if (this.nexttime.Ticks - Global.GetCorrectDateTime().Ticks <= 0L)
			{
				this.oneTimeLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("免费抽取"), new object[0])
				});
				this.oneTimeXiaoHao.text = Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Format(Global.GetLang("免费"), new object[0])
				});
			}
		}
	}

	private long countdowntimes
	{
		get
		{
			return (this.nextTime.Ticks - Global.GetCorrectDateTime().Ticks) / 10000000L;
		}
		set
		{
			this.counttime = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.values = ConfigSystemParam.GetSystemParamIntArrayByName("FuWenPay", ',');
		string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("十连抽必得一个3级以上的符文")
		});
		if (Global.isFanbei(105))
		{
			colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				"9d8667",
				Global.GetLang(ConfigSystemParam.GetSystemParamByName("JieRiFuWenIntro", true))
			});
		}
		this.tenTimeLab.text = colorStringForNGUIText;
		this.oneTimeXiaoHao.text = this.values[0].ToString();
		this.TenTimeXiaoHao.text = this.values[1].ToString();
		this.BtnOneTime.Label.text = Global.GetLang("抽一次");
		this.BtnTenTime.Label.text = Global.GetLang("抽十次");
		this.BG.URL = "NetImages/GameRes/Images/shenshiTexture/ronglian.jpg.qj";
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "FuWenChouQu", this.values[0], "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[1], "FuWenChouQu", this.values[1], "xingyunzhixing");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnOneTime.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.countdowntimes < 0L)
			{
				GameInstance.Game.GetFuWenChouQu(1);
				return;
			}
			if (this.values[0] > Global.GetRoleOwnNumByMoneyType(163) && this.OneTimeType == ShenShiPartFuWenChouQu.FuWenTimeType.Diamond && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("FuWenChouQu", this.values[0], true))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = this.values[0] - Global.GetRoleOwnNumByMoneyType(163);
				string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
					}
					return true;
				};
				return;
			}
			if (this.OneTimeType != ShenShiPartFuWenChouQu.FuWenTimeType.Good && this.OneTimeType != ShenShiPartFuWenChouQu.FuWenTimeType.Diamond)
			{
				Global.SendEvent("1500", Global.GetLang("免费抽取次数"));
			}
			else
			{
				if (this.OneTimeType == ShenShiPartFuWenChouQu.FuWenTimeType.Diamond && Global.GetZuanShi(ZuanShiPartClass.ShenShiChouQu))
				{
					string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "FuWenChouQu", this.values[0]);
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.values[0], text)
					}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
					if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
					}
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							Global.SetZuanShi(ZuanShiPartClass.ShenShiChouQu, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
						}
						Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							Global.SendEvent("1501", Global.GetLang("单次抽取次数"));
							Super.ShowNetWaiting(null);
							GameInstance.Game.GetFuWenChouQu(1);
						}
						return true;
					};
					return;
				}
				Global.SendEvent("1501", Global.GetLang("单次抽取次数"));
			}
			Super.ShowNetWaiting(null);
			GameInstance.Game.GetFuWenChouQu(1);
		};
		this.BtnTenTime.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.values[1] > Global.GetRoleOwnNumByMoneyType(163) && this.OneTimeType == ShenShiPartFuWenChouQu.FuWenTimeType.Diamond && !IConfigbase<ConfigDaiBiShiYong>.Instance.SendHuoBiNumber("FuWenChouQu", this.values[1], true))
			{
				IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi = this.values[1] - Global.GetRoleOwnNumByMoneyType(163);
				string lang = Global.GetLang("花费{0}钻石，（用于祈福、精灵、坐骑、荧石、符文、转盘）");
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(lang, IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendBoCaiDaiBi(IConfigbase<ConfigDaiBiShiYong>.Instance.buyZuanShi, 2);
					}
					return true;
				};
				return;
			}
			if (Global.GetZuanShi(ZuanShiPartClass.ShenShiChouQu))
			{
				string text = IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshString(Global.GetLang("幸运之星"), "FuWenChouQu", this.values[1]);
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}{1}，确定吗？"), this.values[1], text)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
				{
					messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
				}
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					if (messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
					{
						Global.SetZuanShi(ZuanShiPartClass.ShenShiChouQu, !messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
					}
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						Super.ShowNetWaiting(null);
						Global.SendEvent("1502", Global.GetLang("10次抽取次数"));
						GameInstance.Game.GetFuWenChouQu(2);
					}
					return true;
				};
				return;
			}
			Super.ShowNetWaiting(null);
			Global.SendEvent("1502", Global.GetLang("10次抽取次数"));
			GameInstance.Game.GetFuWenChouQu(2);
		};
	}

	public void OnQiFuCompleted(int qiFuType, string goodsStr)
	{
		if (this.ShenShiPartChouQuGifts == null)
		{
			this.OpenShenShiPartChouQuGiftsWindow();
		}
		if (qiFuType == 1)
		{
			this.ShenShiPartChouQuGifts.RefreshUIUnit(true);
		}
		else
		{
			this.ShenShiPartChouQuGifts.RefreshUIUnit(false);
		}
		this.ShenShiPartChouQuGifts.RefreshAddGoodIcons(goodsStr);
	}

	public void StartUITimer()
	{
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "FuWenChouQu", this.values[0], "xingyunzhixing");
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[1], "FuWenChouQu", this.values[1], "xingyunzhixing");
		this.oneTimeXiaoHao.text = this.values[0].ToString();
		this.StopTimer();
		this.UITimer = new DispatcherTimer("ShenShiPartFuWenChouQu_Timer");
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
			this.oneTimeLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"FF0000",
				string.Format(Global.GetLang("距下次免费 {0}"), ShenShiPartFuWenChouQu.GetTimeStrBySecEx((double)this.countdowntimes))
			});
			this.countdowntimes -= 1L;
		}
		else
		{
			this.oneTimeLab.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("免费抽取"), new object[0])
			});
			this.oneTimeXiaoHao.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("免费"), new object[0])
			});
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
				(int)(sec / (double)num2),
				(int)(sec % (double)num2 / (double)num3),
				(int)(sec % (double)num2 % (double)num3)
			};
			string[] array2 = new string[]
			{
				Global.GetLang(":"),
				Global.GetLang(":"),
				Global.GetLang(string.Empty)
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
				Global.GetLang(":"),
				Global.GetLang(":"),
				Global.GetLang(":"),
				Global.GetLang(string.Empty)
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
			text += ((list[i] >= 10) ? list[i].ToString() : ("0" + list[i].ToString()));
			text += list2[i];
		}
		return text;
	}

	private void OpenShenShiPartChouQuGiftsWindow()
	{
		if (this.ShenShiPartChouQuGiftsWindow == null)
		{
			this.ShenShiPartChouQuGiftsWindow = U3DUtils.NEW<GChildWindow>();
			this.ShenShiPartChouQuGiftsWindow.IsShowModal = true;
			this.ShenShiPartChouQuGiftsWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.ShenShiPartChouQuGiftsWindow, Global.GetLang("抽取符文界面"));
			Super.GData.GlobalPlayZone.Children.Add(this.ShenShiPartChouQuGiftsWindow);
		}
		if (this.ShenShiPartChouQuGifts == null)
		{
			this.ShenShiPartChouQuGifts = U3DUtils.NEW<ShenShiPartFuWenChouQuGifts>();
			this.ShenShiPartChouQuGifts.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShenShiPartChouQuGiftsWindow();
			};
		}
		this.ShenShiPartChouQuGiftsWindow.SetContent(this.ShenShiPartChouQuGiftsWindow.BodyPresenter, this.ShenShiPartChouQuGifts, 0.0, 0.0, true);
	}

	private void CloseShenShiPartChouQuGiftsWindow()
	{
		if (null != this.ShenShiPartChouQuGifts)
		{
			this.ShenShiPartChouQuGifts.transform.parent = null;
			Object.Destroy(this.ShenShiPartChouQuGifts.gameObject);
			this.ShenShiPartChouQuGifts = null;
		}
		if (null != this.ShenShiPartChouQuGiftsWindow)
		{
			Super.CloseChildWindow(base.Children, this.ShenShiPartChouQuGiftsWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.ShenShiPartChouQuGiftsWindow, true);
			this.ShenShiPartChouQuGiftsWindow = null;
		}
	}

	public GButton BtnOneTime;

	public GButton BtnTenTime;

	public UILabel oneTimeLab;

	public UILabel tenTimeLab;

	public UILabel oneTimeXiaoHao;

	public UILabel TenTimeXiaoHao;

	public ShowNetImage BG;

	private DateTime nexttime;

	private long counttime;

	private ShenShiPartFuWenChouQu.FuWenTimeType OneTimeType = ShenShiPartFuWenChouQu.FuWenTimeType.Diamond;

	public List<UISprite> listDaiBi = new List<UISprite>();

	private int[] values;

	private DispatcherTimer UITimer;

	protected GChildWindow ShenShiPartChouQuGiftsWindow;

	protected ShenShiPartFuWenChouQuGifts ShenShiPartChouQuGifts;

	public enum FuWenTimeType
	{
		FirstChouQu = 1,
		Free,
		Good,
		Diamond
	}
}
