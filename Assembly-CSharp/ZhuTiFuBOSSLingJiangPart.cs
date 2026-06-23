using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class ZhuTiFuBOSSLingJiangPart : UserControl
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
		this.BtnOk.Text = Global.GetLang("确定");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.ItemCollection = this.ItemList.ItemsSource;
		this.BtnOk.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
	}

	public void SetData(string[] str)
	{
		if (str[0].SafeToInt32(0) == 1)
		{
			this.InitPartData(true, str[2].SafeToInt32(0), str[3].SafeToInt32(0), str[1], str[4].SafeToInt32(0), 0L);
		}
		else
		{
			this.InitPartData(false, str[2].SafeToInt32(0), str[3].SafeToInt32(0), str[1], str[4].SafeToInt32(0), 0L);
		}
	}

	public void InitPartData(bool success, int exp, int money, string goodsString, int paiming = -1, long startTick = 0L)
	{
		if (paiming <= 0)
		{
			this.TxtPiMing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				Global.GetLang("您的伤害未进入排名")
			});
		}
		else
		{
			this.TxtPiMing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				string.Format(Global.GetLang("恭喜您的队伍在活动中伤害排名第{0}"), paiming)
			});
		}
		if (startTick == 0L)
		{
			this.StartTicks = Global.GetCorrectLocalTime();
		}
		else
		{
			this.StartTicks = startTick;
		}
		if (this.StartTicks > Global.GetCorrectLocalTime())
		{
			this.DelayShow = true;
			this.ShowMainPart(false);
		}
		else
		{
			this.DelayShow = false;
			this.ShowMainPart(true);
		}
		this.TimeLimit = 20L;
		this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit);
		base.StartCoroutine<bool>(this.ShowAnim(success));
		string text = string.Format(Global.GetLang("获得经验：{0}        "), exp);
		if (money > 0)
		{
			text += string.Format(Global.GetLang("        金币：{0}"), money);
		}
		this.TxtName.Text = text;
		this.ItemCollection.Clear();
		UIHelper.AddAwardGoods(this.ItemCollection, goodsString);
		Transform transform = this.ItemList.transform;
		transform.localPosition = this.PosX(transform.localPosition, (float)(Mathf.Max(this.ItemList.Count() - 1, 0) * -39));
	}

	private void OnEnable()
	{
		base.StartCoroutine<bool>(this.TickProc());
	}

	protected IEnumerator TickProc()
	{
		for (;;)
		{
			long second = (Global.GetCorrectLocalTime() - this.StartTicks) / 1000L;
			if (second > 0L)
			{
				if (this.DelayShow)
				{
					this.ShowMainPart(true);
				}
				if (this.TimeLimit > second)
				{
					this.TxtTime.text = string.Format(Global.GetLang("{0}秒"), this.TimeLimit - second);
				}
				else
				{
					this.OnSubmit(null, null);
				}
			}
			yield return new WaitForSeconds(this.TickInterval);
		}
		yield break;
	}

	public void ShowMainPart(bool show)
	{
		if (show)
		{
			if (this.DelayShow)
			{
				this.DelayShow = false;
				this.StartTicks = Global.GetCorrectLocalTime();
			}
			base.gameObject.SetActive(true);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private IEnumerator ShowAnim(bool success)
	{
		yield return new WaitForSeconds(0.5f);
		this.Anim[0].gameObject.SetActive(false);
		this.Anim[1].gameObject.SetActive(false);
		if (success)
		{
			this.Anim[0].gameObject.SetActive(true);
		}
		else
		{
			this.Anim[1].gameObject.SetActive(true);
		}
		yield break;
	}

	private Vector3 PosX(Vector3 v, float x)
	{
		v.x = x;
		return v;
	}

	private void AddXmlThemeActivityZhuanShengReward(string XmlList)
	{
		this.m_Dic.Clear();
		XElement gameResXml = Global.GetGameResXml("Config/ThemeActivityZhuanShengReward");
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ThemeActivityZhuanShengReward");
		for (int i = 0; i < xelementList.Count; i++)
		{
			ZhuTiFuBOSSLingJiangPart.ThemeActivityZhuanShengReward themeActivityZhuanShengReward = new ZhuTiFuBOSSLingJiangPart.ThemeActivityZhuanShengReward();
			themeActivityZhuanShengReward.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			themeActivityZhuanShengReward.MapCode = Global.GetXElementAttributeInt(xelementList[i], "MapCode");
			themeActivityZhuanShengReward.MinRank = Global.GetXElementAttributeInt(xelementList[i], "MinRank");
			themeActivityZhuanShengReward.MaxRank = Global.GetXElementAttributeInt(xelementList[i], "MaxRank");
			themeActivityZhuanShengReward.WinRewardItem = Global.GetXElementAttributeStr(xelementList[i], "WinRewardItem");
			themeActivityZhuanShengReward.WinRewardExp = Global.GetXElementAttributeInt(xelementList[i], "WinRewardExp");
			themeActivityZhuanShengReward.WinRewardMoney = Global.GetXElementAttributeInt(xelementList[i], "WinRewardMoney");
			themeActivityZhuanShengReward.LoseRewardItem = Global.GetXElementAttributeStr(xelementList[i], "LoseRewardItem");
			themeActivityZhuanShengReward.LoseRewardExp = Global.GetXElementAttributeInt(xelementList[i], "LoseRewardExp");
			themeActivityZhuanShengReward.LoseRewardMoney = Global.GetXElementAttributeInt(xelementList[i], "LoseRewardMoney");
			if (!this.m_Dic.ContainsKey(themeActivityZhuanShengReward.ID))
			{
				this.m_Dic.Add(themeActivityZhuanShengReward.ID, themeActivityZhuanShengReward);
			}
		}
	}

	private void OnSubmit(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(null, null);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ListBox ItemList;

	public GButton BtnOk;

	public TextBlock TxtTime;

	public TextBlock TxtName;

	public TextBlock TxtPiMing;

	public Animator[] Anim;

	private float TickInterval = 0.5f;

	private long StartTicks;

	private long TimeLimit = 20L;

	private bool DelayShow;

	private ObservableCollection _ItemCollection;

	private Dictionary<int, ZhuTiFuBOSSLingJiangPart.ThemeActivityZhuanShengReward> m_Dic = new Dictionary<int, ZhuTiFuBOSSLingJiangPart.ThemeActivityZhuanShengReward>();

	public class ThemeActivityZhuanShengReward
	{
		public int ID;

		public int MapCode;

		public int MinRank;

		public int MaxRank;

		public int MaxLevel;

		public string WinRewardItem;

		public int WinRewardExp;

		public int WinRewardMoney;

		public string LoseRewardItem;

		public int LoseRewardExp;

		public int LoseRewardMoney;
	}
}
