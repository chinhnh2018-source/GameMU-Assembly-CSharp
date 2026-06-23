using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class FamilyBossPart : UserControl
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

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ItemCollection = this.listBoss.ItemsSource;
		this.btnChallenge.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.EnterFuben);
		this.listBoss.SelectionChanged = new MouseLeftButtonUpEventHandler(this.SelectedItem);
		this.InitQuanminReward();
		this.InitTongguanReward();
		this.InitBossItem();
		this.InitTime();
		this.btnChallenge.isEnabled = false;
		GameInstance.Game.GetFamilyBossInfoCmd();
		this.listboxPanel.onDragFinished = delegate()
		{
			float num = Mathf.Round(this.panel.clipRange.x / 230f) * 230f;
			SpringPanel.Begin(this.panel.gameObject, new Vector3(-num + 68f, 0f, 0f), 10f);
		};
		this.listBoss.SelectedIndex = 0;
	}

	private void InitTextInPrefabs()
	{
		this.btnChallenge.Text = Global.GetLang("立即挑战");
	}

	private void InitQuanminReward()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("ZhanMengCodeAward", '|');
		int num = -1;
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			string[] array = systemParamStringArrayByName[i].Split(new char[]
			{
				','
			});
			if (array.Length == 2 && int.TryParse(array[0], ref num) && !this.DicReward.ContainsKey(num))
			{
				this.DicReward.Add(num, array[1]);
			}
		}
	}

	private void InitTongguanReward()
	{
		XElement gameResXml = Global.GetGameResXml("Config/FuBenMap.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		foreach (XElement xelement in xelementList)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "CopyID");
			if (this.DicReward.ContainsKey(xelementAttributeInt) && !this.DicTongGuan.ContainsKey(xelementAttributeInt))
			{
				FamilyBossPart.TongGuanReward tongGuanReward = new FamilyBossPart.TongGuanReward();
				tongGuanReward.Experienceaward = Global.GetXElementAttributeInt(xelement, "Experienceaward");
				tongGuanReward.Moneyaward = Global.GetXElementAttributeInt(xelement, "Moneyaward");
				tongGuanReward.ZhanGongaward = Global.GetXElementAttributeInt(xelement, "ZhanGongaward");
				this.DicTongGuan.Add(xelementAttributeInt, tongGuanReward);
			}
		}
	}

	private void InitBossItem()
	{
		this.ItemCollection.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/FuBen.Xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "Copy");
		for (int i = 0; i < xelementList.Count; i++)
		{
			XElement xelement = xelementList[i];
			if (xelement != null)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (this.DicReward.ContainsKey(xelementAttributeInt))
				{
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "UpCopyID");
					int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "DownCopyID");
					int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "MapCode");
					int xelementAttributeInt5 = Global.GetXElementAttributeInt(xelement, "WeekEnterNumber");
					string bakUrl = Global.GetXElementAttributeStr(xelement, "Preview2").ToString();
					int xelementAttributeInt6 = Global.GetXElementAttributeInt(xelement, "BossID");
					FamilyBosslistItem familyBosslistItem = U3DUtils.NEW<FamilyBosslistItem>();
					familyBosslistItem.Id = xelementAttributeInt;
					familyBosslistItem.UpCopyID = xelementAttributeInt2;
					familyBosslistItem.DownCopyID = xelementAttributeInt3;
					familyBosslistItem.MapCode = xelementAttributeInt4;
					familyBosslistItem.WeekEnterNumber = xelementAttributeInt5;
					familyBosslistItem.SetBakUrl(bakUrl);
					familyBosslistItem.FamilybossState = FamilyBossState.NotOpen;
					familyBosslistItem.FamilyBossAwardState = FamilyBossAwardState.Gained;
					familyBosslistItem.SelectState = false;
					if (this.DicReward.ContainsKey(xelementAttributeInt))
					{
						familyBosslistItem.Reward = this.DicReward[xelementAttributeInt];
					}
					this.ItemCollection.AddNoUpdate(familyBosslistItem);
					this.listBossItem.Add(familyBosslistItem);
					this.DicBossItem.Add(xelementAttributeInt, familyBosslistItem);
					this.DicIDtoBossid.Add(xelementAttributeInt, xelementAttributeInt6);
					UIPanel component = familyBosslistItem.transform.GetComponent<UIPanel>();
					if (component != null)
					{
						Object.Destroy(component);
					}
				}
			}
		}
	}

	private void SelectedItem(object sender, MouseEvent e)
	{
		FamilyBosslistItem familyBosslistItem = U3DUtils.AS<FamilyBosslistItem>(this.listBoss.SelectedItem);
		if (null == familyBosslistItem)
		{
			return;
		}
		if (!familyBosslistItem.IsEnable)
		{
			return;
		}
		if (this.listItem != null && this.listItem != familyBosslistItem)
		{
			this.listItem.SelectState = false;
		}
		if (familyBosslistItem == this.listItem)
		{
			return;
		}
		this.listItem = familyBosslistItem;
		this.listItem.SelectState = true;
		this.bossID = this.listItem.Id;
		if (this.listItem.FamilybossState == FamilyBossState.Passed)
		{
			this.btnChallenge.isEnabled = false;
		}
		else
		{
			this.btnChallenge.isEnabled = true;
		}
	}

	private void InitTime()
	{
		this.nextRefresh = default(DateTime);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		int dayOfWeek = correctDateTime.DayOfWeek;
		this.nextRefresh = this.nextDay(correctDateTime, dayOfWeek);
		base.InvokeRepeating("TickProc", 0f, 1f);
	}

	private DateTime nextDay(DateTime nowTime, int week)
	{
		DateTime result;
		result..ctor(nowTime.Year, nowTime.Month, nowTime.Day, 0, 0, 0);
		if (week == 0)
		{
			result = result.Add(new TimeSpan(0, 23, 59, 59));
		}
		else
		{
			result = result.Add(new TimeSpan(8 - week - 1, 23, 59, 59));
		}
		return result;
	}

	protected void TickProc()
	{
		long correctLocalTime = Global.GetCorrectLocalTime();
		long num = this.nextRefresh.Ticks / 10000L;
		if (num > correctLocalTime)
		{
			int num2 = (int)((num - correctLocalTime) / 1000L);
			if (num2 <= 0)
			{
				this.labDaojishi.text = Global.GetLang(string.Empty);
				base.CancelInvoke("TickProc");
				this.InitializeComponent();
			}
			else
			{
				this.labDaojishi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"9d8667",
					Global.GetLang("刷新倒计时:") + Global.GetTimeStrBySecEx((double)num2, true, -1)
				});
			}
		}
		else
		{
			this.labDaojishi.text = Global.GetLang(string.Empty);
			base.CancelInvoke("TickProc");
			this.InitializeComponent();
		}
	}

	private void EnterFuben(object sender, MouseEvent e)
	{
		GameInstance.Game.SpriteEnterFuBen(this.bossID);
	}

	public void GetFubenInfo(int roleID, int fubenID, int flag, string names)
	{
		int count = this.listBossItem.Count;
		int num = -1;
		if (fubenID == 0)
		{
			num = count;
			foreach (int num2 in this.DicBossItem.Keys)
			{
				FamilyBosslistItem familyBosslistItem = this.DicBossItem[num2];
				familyBosslistItem.FamilybossState = FamilyBossState.Passed;
			}
		}
		else
		{
			num = fubenID - 40000 + 1;
			if (this.DicBossItem.ContainsKey(fubenID))
			{
				foreach (int num3 in this.DicBossItem.Keys)
				{
					FamilyBosslistItem familyBosslistItem = this.DicBossItem[num3];
					if (num3 < fubenID)
					{
						familyBosslistItem.FamilybossState = FamilyBossState.Passed;
					}
					if (num3 == fubenID)
					{
						familyBosslistItem.FamilybossState = FamilyBossState.IsOpen;
						break;
					}
				}
			}
		}
		string[] array = names.Split(new char[]
		{
			','
		});
		for (int i = 1; i <= num; i++)
		{
			FamilyBosslistItem familyBosslistItem = this.listBossItem[i - 1];
			int intSomeBit = Global.GetIntSomeBit(flag, i * 2 - 1);
			if (familyBosslistItem.FamilybossState == FamilyBossState.Passed)
			{
				if (i <= array.Length)
				{
					familyBosslistItem.StrName = array[i - 1];
				}
				if (intSomeBit == 1)
				{
					familyBosslistItem.FamilyBossAwardState = FamilyBossAwardState.Gained;
				}
				else if (intSomeBit == 0)
				{
					familyBosslistItem.FamilyBossAwardState = FamilyBossAwardState.CanGain;
				}
			}
			else
			{
				familyBosslistItem.FamilyBossAwardState = FamilyBossAwardState.Gained;
			}
		}
	}

	public void GetAwardState(int roleID, int result, int zhangong = 0, int fubenID = 0)
	{
		if (result == 0)
		{
			if (this.DicBossItem.ContainsKey(fubenID))
			{
				FamilyBosslistItem familyBosslistItem = this.DicBossItem[fubenID];
				familyBosslistItem.FamilyBossAwardState = FamilyBossAwardState.Gained;
			}
		}
		else
		{
			if (result == -1)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("副本信息错误"), new object[0]), 0, -1, -1, 0);
			}
			if (result == -2)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("未知错误"), new object[0]), 0, -1, -1, 0);
			}
			if (result == -3)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("领取条件不足"), new object[0]), 0, -1, -1, 0);
			}
			if (result == -4)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("没有奖励设置"), new object[0]), 0, -1, -1, 0);
			}
			if (result == -6)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您已领取奖励"), new object[0]), 0, -1, -1, 0);
			}
		}
	}

	private const int AGridSize = 230;

	private const string ZhanMeng = "ZhanMengCodeAward";

	public TextBlock labDaojishi;

	public GButton btnChallenge;

	public ListBox listBoss;

	public UIPanel panel;

	public UIDraggablePanel listboxPanel;

	private DateTime nextRefresh;

	private Dictionary<int, string> DicReward = new Dictionary<int, string>();

	private Dictionary<int, FamilyBossPart.TongGuanReward> DicTongGuan = new Dictionary<int, FamilyBossPart.TongGuanReward>();

	private Dictionary<int, int> DicIDtoBossid = new Dictionary<int, int>();

	private List<FamilyBosslistItem> listBossItem = new List<FamilyBosslistItem>();

	private Dictionary<int, FamilyBosslistItem> DicBossItem = new Dictionary<int, FamilyBosslistItem>();

	private int bossID;

	private ObservableCollection _ItemCollection;

	private FamilyBosslistItem listItem;

	public class TongGuanReward
	{
		public int Moneyaward;

		public int Experienceaward;

		public int ZhanGongaward;
	}
}
