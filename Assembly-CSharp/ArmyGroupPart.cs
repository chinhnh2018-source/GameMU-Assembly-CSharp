using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ArmyGroupPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitData();
	}

	private void InitData()
	{
		Super.ShowNetWaiting(null);
		this.mTishiCount = 0;
		GameInstance.Game.SendGetRoleArmyGroupData(Global.Data.RoleID);
	}

	private void InitPrefabText()
	{
		this._ArmyGroupInf[0].Text = Global.GetColorStringForNGUIText(new object[]
		{
			"ffcc19",
			Global.GetLang("军团信息")
		});
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LegionProsperityCost", ',');
		if (systemParamIntArrayByName != null && systemParamIntArrayByName.Length == 4)
		{
			this.BowmInf.Text = string.Concat(new string[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("每天0点扣除")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fffffe",
					systemParamIntArrayByName[1].ToString()
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("繁荣度，繁荣度降为")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					systemParamIntArrayByName[3].ToString()
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					Global.GetLang("军团自动解散")
				})
			});
		}
	}

	private void InitTexture()
	{
		if (null != this._BgTexture)
		{
			this._BgTexture.URL = "NetImages/GameRes/Images/ArmyGroup/ArmyGroupBak0.jpg";
		}
	}

	private void InitHandler()
	{
		if (null != this._CloseBtn)
		{
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						Index = 0
					});
				}
			};
		}
		byte b = 0;
		while ((int)b < this._EventBtns.Length)
		{
			this.m_btnHanderLst.Add(new ArmyGroupPart.BtnHander(this._EventBtns[(int)b], (int)b, new DPSelectedItemEventHandler(this.EventBtnsHander), false, false));
			b += 1;
		}
		NGUITools.SetActive(this.GongGaoObj, false);
		this._GongGaoBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.ShowGongGao();
		};
	}

	private void ShowGongGao()
	{
		this.mArmyGroupGongGao = null;
		if (this.m_JunTuanData == null || string.IsNullOrEmpty(this.m_JunTuanData.Bulletin))
		{
			if (Global.Data.roleData.JunTuanZhiWu == 1)
			{
				this.mArmyGroupGongGao = new ArmyGroupPart.ArmyGroupGongGao(this.GongGaoObj, this.GongGaoTextBox, this.GongGaoCloseBtn, this.m_JunTuanData.Bulletin, this.GongGaoTitle, this.mLastChangeGongGaoSTime);
				this.mArmyGroupGongGao.GongGaoHander = delegate(object e, DPSelectedItemEventArgs s)
				{
					if (s != null)
					{
						this.mLastChangeGongGaoSTime = s.MyBufferData.StartTime;
					}
				};
			}
			else
			{
				Super.HintMainText(Global.GetLang("暂无公告"), 10, 3);
			}
		}
		else
		{
			this.mArmyGroupGongGao = new ArmyGroupPart.ArmyGroupGongGao(this.GongGaoObj, this.GongGaoTextBox, this.GongGaoCloseBtn, this.m_JunTuanData.Bulletin, this.GongGaoTitle, this.mLastChangeGongGaoSTime);
			this.mArmyGroupGongGao.GongGaoHander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (s != null)
				{
					this.mLastChangeGongGaoSTime = s.MyBufferData.StartTime;
				}
			};
		}
	}

	private void EventBtnsHander(object sender, DPSelectedItemEventArgs e)
	{
		if (e != null)
		{
			this.ShowWind((byte)e.MyID);
		}
	}

	private void ShowWind(byte type)
	{
		if (type == 0)
		{
			PlayZone.GlobalPlayZone.ShowArmyGroupRenwuPart(1, 0, 0, 0);
		}
		else if (type == 1)
		{
			PlayZone.GlobalPlayZone.ShowArmyGroupRenwuPart(0, this.m_JunTuanData.WeekRank, this.m_JunTuanData.WeekPoint, 0);
		}
		else if (type == 2)
		{
			PlayZone.GlobalPlayZone.OpenArmyTeamActivityWindow();
		}
		else if (type == 3)
		{
			this.ShowArmyGroupEvrntHall();
		}
		else if (type == 4)
		{
			if (this.Hander != null)
			{
				this.Hander(null, new DPSelectedItemEventArgs
				{
					Index = 13
				});
			}
		}
		else if (type == 5 && this.Hander != null)
		{
			this.Hander(null, new DPSelectedItemEventArgs
			{
				Index = 11
			});
		}
	}

	private void ShowArmyGroupEvrntHall()
	{
		if (null == this.m_ArmyGroupEvrntHallWind)
		{
			this.m_ArmyGroupEvrntHallWind = U3DUtils.NEW<GChildWindow>();
			this.m_ArmyGroupEvrntHallWind.Modal = true;
			this.m_ArmyGroupEvrntHallWind.ModalType = ChildWindowModalType.Translucent;
		}
		this.m_ArmyGroupEvrntHallWind.Visibility = true;
		base.Add(this.m_ArmyGroupEvrntHallWind);
		Super.InitChildWindow(this.m_ArmyGroupEvrntHallWind, "ArmyGroupEvrntHall");
		if (null != this.m_ArmyGroupEvrntHall)
		{
			Object.Destroy(this.m_ArmyGroupEvrntHall);
		}
		this.m_ArmyGroupEvrntHall = U3DUtils.NEW<ArmyGroupEvrntHall>();
		this.m_ArmyGroupEvrntHallWind.Body.Add(this.m_ArmyGroupEvrntHall);
		this.m_ArmyGroupEvrntHall.Hander = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.ID == 0)
			{
				this.CloseArmyGroupEventHall();
			}
		};
	}

	private void CloseArmyGroupEventHall()
	{
		if (null != this.m_ArmyGroupEvrntHallWind)
		{
			Object.Destroy(this.m_ArmyGroupEvrntHallWind.gameObject);
			Super.CloseChildWindow(this, this.m_ArmyGroupEvrntHallWind);
			if (null != this.m_ArmyGroupEvrntHall)
			{
				Object.Destroy(this.m_ArmyGroupEvrntHall.gameObject);
			}
			this.m_ArmyGroupEvrntHall = null;
			this.m_ArmyGroupEvrntHallWind = null;
		}
	}

	private void CloseArmyGroupShenQingLieBiaoPart()
	{
	}

	private void RefreshUI(JunTuanData data)
	{
		if (data != null)
		{
			this._ArmyGroupInf[0].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("军团名称：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fa6c0d",
				string.Format("{0}", data.JunTuanName)
			});
			this._ArmyGroupInf[1].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("军团团长：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format("{0}", Global.FormatRoleNameZoneid(data.LeaderZoneId, data.LeaderName, 1, 1))
			});
			this._ArmyGroupInf[2].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟个数：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format("{0}", data.BangHuiNum)
			});
			this._ArmyGroupInf[3].Text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("军团繁荣：")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"fdf7dd",
				string.Format("{0}", data.Point)
			});
			for (byte b = 0; b < 4; b += 1)
			{
				this._ArmyGroupInf[(int)b].transform.localScale = Vector3.one * 18f;
			}
			DateTime correctDateTime = Global.GetCorrectDateTime();
			if (correctDateTime.DayOfWeek == null && 4 >= data.WeekRank && correctDateTime.Hour == 20 && 30 > correctDateTime.Minute)
			{
				this.m_btnHanderLst[2].ShowGanTanHao = true;
			}
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupTaskList();
			this.RefreshShenQingGanTanHao(-1);
			byte b2;
			this.mTishiCount = (b2 = this.mTishiCount) + 1;
			if (b2 == 0)
			{
				int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("LegionProsperityCost", ',');
				if (systemParamIntArrayByName != null && 2 < systemParamIntArrayByName.Length && data.Point <= systemParamIntArrayByName[2])
				{
					string lang = Global.GetLang("当前军团繁荣度不足，军团繁荣度低于0将被解散");
					string[] buttons = new string[]
					{
						Global.GetLang("确定")
					};
					Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
					{
					}, buttons);
				}
			}
		}
	}

	public void RefreshShenQingGanTanHao(int ret = -1)
	{
		if (0 < this.mShenQingGanTanHaoActiveCount)
		{
			this.m_btnHanderLst[0].ShowGanTanHao = false;
			return;
		}
		if (0 <= ret)
		{
			this.m_btnHanderLst[0].ShowGanTanHao = false;
		}
		else if (this.m_JunTuanData != null && 0 < this.m_JunTuanData.RequestCount && ArmyGroupPart.GetZhiWu(Global.Data.roleData.JunTuanZhiWu) == 1 && 4 > this.m_JunTuanData.BangHuiNum)
		{
			this.m_btnHanderLst[0].ShowGanTanHao = true;
			this.mShenQingGanTanHaoActiveCount += 1;
		}
	}

	public void RefreshRenWuBtn(List<JunTuanTaskData> data)
	{
		if (data != null && 0 < data.Count)
		{
			for (int i = 0; i < data.Count; i++)
			{
				if (data[i].TaskState == 0L)
				{
					this.m_btnHanderLst[1].ShowGanTanHao = true;
					break;
				}
				if (data[i].TaskState == 1L && data[i].HasGet == 0)
				{
					this.m_btnHanderLst[1].ShowGanTanHao = true;
					break;
				}
			}
		}
	}

	public void ShowArmyGroupShenQingLieBiaoPart()
	{
		if (this.Hander != null)
		{
			this.Hander(null, new DPSelectedItemEventArgs
			{
				Index = 12
			});
		}
	}

	public static int GetZhiWu(int ZhiWu)
	{
		if (ZhiWu == 0)
		{
			return 4;
		}
		return ZhiWu;
	}

	public void RefreshArmyTaskData(List<JunTuanMiniData> data)
	{
	}

	public void RefreshGongGao(int ret)
	{
		if (ret == 0)
		{
			GameInstance.Game.SendGetRoleArmyGroupData(Global.Data.RoleID);
		}
		else
		{
			ArmyGroupPart.ErrorLog(ret);
		}
	}

	public void RefreshChangeJunTuanLeaerCallBack(int ret)
	{
		if (ret == 0)
		{
			this.InitData();
		}
		else
		{
			ArmyGroupPart.ErrorLog(ret);
		}
	}

	public static void CheckListBoxChild(int ActiveCount, ObservableCollection mOBC)
	{
		if (mOBC != null)
		{
			for (int i = 0; i < mOBC.Count; i++)
			{
				if (ActiveCount <= i)
				{
					NGUITools.SetActive(mOBC.GetAt(i), false);
				}
				else
				{
					NGUITools.SetActive(mOBC.GetAt(i), true);
				}
			}
		}
	}

	public static void ErrorLog(int ret)
	{
		if (0 > ret)
		{
			string errMsg = StdErrorCode.GetErrMsg(ret, false, false);
			Super.HintMainText(Global.GetLang(errMsg), 10, 3);
		}
		else
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendGetArmyGroupJingYingDataLst();
		}
	}

	public static void SuccessMesg(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			ArmyGroupPart.MesData = str;
		}
	}

	public void RefreshArmyGroupData(JunTuanData data)
	{
		Super.HideNetWaiting();
		this.m_JunTuanData = data;
		this.RefreshUI(this.m_JunTuanData);
	}

	public void RefreshTaskData(List<JunTuanTaskData> lst)
	{
		if (lst == null || 0 < lst.Count)
		{
		}
	}

	public void RefreshTaskData(int ret)
	{
		if (ret == 0)
		{
			GameInstance.Game.SendGetArmyGroupTaskList();
		}
		else
		{
			ArmyGroupPart.ErrorLog(ret);
		}
	}

	public void RefreshBHData(List<JunTuanBangHuiData> data)
	{
		Super.HideNetWaiting();
		this.m_JunTuanBangHuiDataLst = data;
		this.RefreshUI(this.m_JunTuanData);
	}

	public void RefreshEventPartUI(List<JunTuanEventLog> data)
	{
		if (null != this.m_ArmyGroupEvrntHall)
		{
			this.m_ArmyGroupEvrntHall.RefreshEventUI(data);
		}
	}

	public ShowNetImage _BgTexture;

	public GButton _CloseBtn;

	public TextBlock[] _ArmyGroupInf;

	public GameObject[] _EventBtns;

	public GButton _GongGaoBtn;

	public TextBox GongGaoTextBox;

	public GameObject GongGaoObj;

	public GButton GongGaoCloseBtn;

	public UILabel GongGaoTitle;

	public TextBlock BowmInf;

	private ArmyGroupEvrntHall m_ArmyGroupEvrntHall;

	private GChildWindow m_ArmyGroupEvrntHallWind;

	private List<ArmyGroupPart.BtnHander> m_btnHanderLst = new List<ArmyGroupPart.BtnHander>();

	private ArmyGroupPart.ArmyGroupGongGao mArmyGroupGongGao;

	private List<JunTuanBangHuiData> m_JunTuanBangHuiDataLst;

	private JunTuanData m_JunTuanData;

	private byte mTishiCount;

	private long mLastChangeGongGaoSTime;

	private byte mShenQingGanTanHaoActiveCount;

	private static string MesData = string.Empty;

	public DPSelectedItemEventHandler Hander;

	private class BtnHander
	{
		public BtnHander(GameObject btn, int Tag, DPSelectedItemEventHandler Hander, bool iconIsActive = false, bool showGanTanHao = false)
		{
			string[] array = new string[]
			{
				Global.GetLang("军团联盟"),
				Global.GetLang("任务营"),
				Global.GetLang("活动大厅"),
				Global.GetLang("事件大厅"),
				Global.GetLang("世界军团"),
				Global.GetLang("精英殿")
			};
			btn.transform.FindChild("Label").GetComponent<UILabel>().text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				array[Tag]
			});
			this.GanTanHaoObj = btn.transform.FindChild("GanTanHao").gameObject;
			this.BtnSP = btn.transform.FindChild("SP").GetComponent<UISprite>();
			this.Icon = btn.transform.FindChild("Icon").GetComponent<UISprite>();
			UIEventListener.Get(btn).onClick = delegate(GameObject e)
			{
				if (Hander != null)
				{
					Hander(e, new DPSelectedItemEventArgs
					{
						MyID = Tag
					});
				}
			};
			this.ShowGanTanHao = showGanTanHao;
		}

		public bool IconIsActive
		{
			get
			{
				return null != this.Icon && this.Icon.spriteName.Contains("1");
			}
			set
			{
				string text = this.Icon.spriteName;
				if (value)
				{
					if (!this.IconIsActive)
					{
						text = text.Replace('0', '1');
					}
				}
				else if (this.IconIsActive)
				{
					text = text.Replace('1', '0');
				}
				this.Icon.spriteName = text;
			}
		}

		public bool ShowGanTanHao
		{
			get
			{
				return NGUITools.GetActive(this.GanTanHaoObj);
			}
			set
			{
				NGUITools.SetActive(this.GanTanHaoObj, value);
			}
		}

		public GButton btn;

		public GameObject GanTanHaoObj;

		public UISprite Icon;

		public UISprite BtnSP;
	}

	public class ArmyGroupGongGao
	{
		public ArmyGroupGongGao(GameObject obj, TextBox textBox, GButton CloseBtn, string GongGaoText, UILabel titletext, long lastChangGongGaoTime)
		{
			this.mThisObj = obj;
			this.mTextBox = textBox;
			this.LastChangGongGaoTime = lastChangGongGaoTime;
			if (string.IsNullOrEmpty(GongGaoText))
			{
				this.mTextBox.label.text = Global.GetLang("点击修改公告");
			}
			else
			{
				this.mTextBox.label.text = GongGaoText;
			}
			this.GongGaoContent = GongGaoText;
			this.mCloseBtn = CloseBtn;
			titletext.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("军团公告")
			});
			NGUITools.SetActive(this.mThisObj, true);
			this.InitMiniGongGao();
		}

		private void InitMiniGongGao()
		{
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mTextBox.label.text != this.GongGaoContent && this.mTextBox.label.text != Global.GetLang("点击修改公告"))
				{
					if (this.mTextBox.text.Length > 100)
					{
						Super.HintMainText(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"), 10, 3);
						this.mTextBox.selected = true;
						return;
					}
					if (this.GongGaoHander != null)
					{
						BufferData bufferData = new BufferData();
						bufferData.StartTime = (long)U3DUtils.GetTimer();
						this.GongGaoHander(null, new DPSelectedItemEventArgs
						{
							MyBufferData = bufferData
						});
					}
					GameInstance.Game.SendSetArmyGroupJionBullEtin(this.mTextBox.Text);
				}
				NGUITools.SetActive(this.mThisObj, false);
			};
			this.mTextBox.LostFocus = new EventHandler(this.TextChanged);
			this.mTextBox.GotFocus = new EventHandler(this.TextGotFouce);
		}

		private void TextGotFouce(object sender, EventArgs e)
		{
			ArmyGroupLegionsVO roleArmyGroupLimitsVO = ConfigArmyGroupLegions.GetRoleArmyGroupLimitsVO(ArmyGroupPart.GetZhiWu(Global.Data.roleData.JunTuanZhiWu));
			if (roleArmyGroupLimitsVO != null)
			{
				if (0 >= roleArmyGroupLimitsVO.BulletinCD)
				{
					Super.HintMainText(string.Format(Global.GetLang("非军团长无法修改军团公告"), new object[0]), 10, 3);
					this.mTextBox.NotFocus();
				}
			}
			else
			{
				Super.HintMainText(string.Format(Global.GetLang("非军团长无法修改军团公告"), new object[0]), 10, 3);
				this.mTextBox.NotFocus();
			}
		}

		private void TextChanged(object sender, EventArgs e)
		{
			ArmyGroupLegionsVO roleArmyGroupLimitsVO = ConfigArmyGroupLegions.GetRoleArmyGroupLimitsVO(ArmyGroupPart.GetZhiWu(Global.Data.roleData.JunTuanZhiWu));
			if (roleArmyGroupLimitsVO != null)
			{
				if (0 >= roleArmyGroupLimitsVO.BulletinCD)
				{
					Super.HintMainText(string.Format(Global.GetLang("非军团长无法修改军团公告"), new object[0]), 10, 3);
					this.mTextBox.Text = this.GongGaoContent;
				}
				else
				{
					if (this.mTextBox.text.Length > 100)
					{
						Super.HintMainText(Global.GetLang("您输入的战盟公告超过了100汉字，请重新输入！"), 10, 3);
						this.mTextBox.selected = true;
						return;
					}
					if (Global.IncludeReplaceFilterFileds(this.mTextBox.Text))
					{
						Super.HintMainText(Global.GetLang("抱歉,您的修改的公告当中含有敏感词汇，请重新输入!"), 10, 3);
						this.mTextBox.Text = this.GongGaoContent;
					}
				}
			}
			else
			{
				Super.HintMainText(string.Format(Global.GetLang("非军团长无法修改军团公告"), new object[0]), 10, 3);
				this.mTextBox.Text = this.GongGaoContent;
			}
			int bulletinCD = roleArmyGroupLimitsVO.BulletinCD;
			int num = 0;
			if (this.LastChangGongGaoTime != 0L)
			{
				num = bulletinCD - (int)(((long)U3DUtils.GetTimer() - this.LastChangGongGaoTime) / 1000L);
			}
			if (num > 0)
			{
				Super.HintMainText(StringUtil.substitute(Global.GetLang("{0}秒之后才能修改军团公告"), new object[]
				{
					num
				}), 10, 3);
				this.mTextBox.Text = this.GongGaoContent;
			}
		}

		private TextBox mTextBox;

		private GButton mCloseBtn;

		private GameObject mThisObj;

		public DPSelectedItemEventHandler GongGaoHander;

		private string GongGaoContent;

		private long LastChangGongGaoTime;
	}
}
