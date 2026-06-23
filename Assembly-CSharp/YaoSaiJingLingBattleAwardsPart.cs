using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YaoSaiJingLingBattleAwardsPart : UserControl
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
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.ItemCollection = this.mListBox.ItemsSource;
	}

	private void InitTextInPrefabs()
	{
		this.mBtnConfirm.Text = Global.GetLang("确定");
		this.mTipsContent = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("协助领奖次数已达上限，今日协助战斗不再获得奖励")
		});
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Close();
		};
		this.mBtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Close();
		};
	}

	private void Close()
	{
		switch (this.mAwardType)
		{
		case YaoSaiJingLingBattleAwardsPart.EAwardType.BossDeadPreview:
			this.CloseThisWindow();
			break;
		case YaoSaiJingLingBattleAwardsPart.EAwardType.Battle:
			this.StopCountDown();
			this.CloseThisWindow();
			if (this.BossOwnerId != Global.Data.RoleID)
			{
				GameInstance.Game.SendYaoSaiMainBossMainInfoRequest(this.BossOwnerId);
				JingLingMap.inst.mapmini.RequestRelation();
				this.CloseBossInfoWindow();
			}
			break;
		case YaoSaiJingLingBattleAwardsPart.EAwardType.BossDead:
			GameInstance.Game.SendYaoSaiMainBossMainInfoRequest(Global.Data.roleData.RoleID);
			this.StopCountDown();
			this.CloseThisWindow();
			this.CloseBossInfoWindow();
			break;
		case YaoSaiJingLingBattleAwardsPart.EAwardType.TaskAwark:
			GameInstance.Game.SendGetJingLingYaiSaiData(Global.Data.roleData.RoleID);
			this.StopCountDown();
			this.CloseThisWindow();
			break;
		}
	}

	private void CloseThisWindow()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, new DPSelectedItemEventArgs
			{
				IDType = 0
			});
		}
	}

	private void CloseBossInfoWindow()
	{
		PlayZone.GlobalPlayZone.CloseYaoSaiBossInfoPart();
	}

	private void InitValue()
	{
	}

	public void ShowWindow(int type, string[] awards, int ownerId = 0, int damage = 0, float lifePercent = 0f, bool hasAward = true, int bossId = 0, string[] extraAwards = null)
	{
		this.BossOwnerId = ownerId;
		this.mAwardType = (YaoSaiJingLingBattleAwardsPart.EAwardType)type;
		this.bossLeftHP = lifePercent;
		this.BossId = bossId;
		switch (this.mAwardType)
		{
		case YaoSaiJingLingBattleAwardsPart.EAwardType.BossDeadPreview:
			this.mLblTitle.Text = Global.GetLang("讨伐奖励预览");
			this.Describe = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("BOSS被成功击杀或者倒计时结束, 讨伐BOSS的玩家可以获得讨伐奖励")
			});
			NGUITools.SetActive(this.mDescribeObj, true);
			this.mDescribeObj.transform.localPosition = Vector3.zero;
			this.mContentObj.transform.localPosition = new Vector3(0f, -4f, 0f);
			NGUITools.SetActive(this.mCountDownObj, false);
			this.mBtnConfirm.gameObject.transform.localPosition = new Vector3(0f, -100f, 0f);
			break;
		case YaoSaiJingLingBattleAwardsPart.EAwardType.Battle:
		{
			this.mLblTitle.Text = Global.GetLang("战斗奖励");
			string chineseText = string.Format("{0}{1}     {2}{3}", new object[]
			{
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("造成伤害：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					damage
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("BOSS剩余血量：")
				}),
				Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					lifePercent.ToString("p")
				})
			});
			this.Describe = Global.GetLang(chineseText);
			NGUITools.SetActive(this.mDescribeObj, true);
			NGUITools.SetActive(this.mCountDownObj, true);
			this.mContentObj.transform.localPosition = new Vector3(0f, 28f, 0f);
			this.StartCountDown(this.coutDown_sec);
			break;
		}
		case YaoSaiJingLingBattleAwardsPart.EAwardType.BossDead:
			this.mLblTitle.Text = Global.GetLang("讨伐奖励");
			NGUITools.SetActive(this.mDescribeObj, false);
			NGUITools.SetActive(this.mCountDownObj, true);
			this.mContentObj.transform.localPosition = new Vector3(0f, 28f, 0f);
			this.StartCountDown(this.coutDown_sec);
			break;
		case YaoSaiJingLingBattleAwardsPart.EAwardType.TaskAwark:
			this.mLblTitle.Text = Global.GetLang("奖励");
			NGUITools.SetActive(this.mDescribeObj, false);
			NGUITools.SetActive(this.mCountDownObj, true);
			this.mContentObj.transform.localPosition = new Vector3(0f, 28f, 0f);
			this.coutDown_sec = 8;
			this.StartCountDown(this.coutDown_sec);
			break;
		}
		this.DisplayAwards(awards, type, hasAward, extraAwards);
		this.LoadEffect(type);
	}

	private void LoadEffect(int type)
	{
		int num = 0;
		if (this.mListBox != null)
		{
			num = this.mListBox.gameObject.transform.childCount;
		}
		for (int i = 0; i < num; i++)
		{
			switch (type)
			{
			case 2:
				this.LoadEffect(new Vector3((float)(i * 90), 0f, -10f));
				break;
			case 3:
				this.LoadEffect(new Vector3((float)(i * 90), 0f, -10f));
				break;
			}
		}
	}

	private void DisplayAwards(string[] awards, int type, bool hasAward, string[] extraGoods = null)
	{
		if (hasAward)
		{
			NGUITools.SetActive(this.mTips, false);
			NGUITools.SetActive(this.mContentObj, true);
		}
		else
		{
			NGUITools.SetActive(this.mTips, true);
			NGUITools.SetActive(this.mContentObj, false);
			this.mLblTips.Text = this.mTipsContent;
		}
		if (awards.Length <= 0 && !hasAward)
		{
			return;
		}
		this.ItemCollection.Clear();
		this.LoadGoodsList(awards, type);
		if (type == 3)
		{
			if (this.bossLeftHP > 0f)
			{
				return;
			}
			this.LoadGoodsList(extraGoods, type);
		}
	}

	private void LoadGoodsList(string[] goods, int type)
	{
		if (goods == null || goods.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < goods.Length; i++)
		{
			string[] array = goods[i].Split(new char[]
			{
				','
			});
			if (array.Length == 7)
			{
				YaoSaiJingLingBattleAwardsPart.EAwardType eawardType = this.mAwardType;
				if (eawardType != YaoSaiJingLingBattleAwardsPart.EAwardType.None)
				{
				}
				this.m_ShowFanBei.gameObject.SetActive(false);
				if (type == 2 || type == 1 || type == 3)
				{
					if (Global.isFanbei(14))
					{
						this.m_ShowFanBei.gameObject.SetActive(true);
					}
					else
					{
						this.m_ShowFanBei.gameObject.SetActive(false);
					}
					if (Global.isFanbei(14))
					{
						this.AddGoodsIcon(type, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]) * 2, Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
					}
					else
					{
						this.AddGoodsIcon(type, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
					}
				}
				else
				{
					this.AddGoodsIcon(type, Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), Convert.ToInt32(array[2]), Convert.ToInt32(array[3]), Convert.ToInt32(array[4]), Convert.ToInt32(array[5]), Convert.ToInt32(array[6]));
				}
			}
		}
	}

	private void AddGoodsIcon(int type, int goodsID, int gcount, int binding, int forgeLevel, int zhuijiaLevel = 0, int lucky = 0, int zhuoyueIndex = 0)
	{
		int num = gcount;
		if (type == 3)
		{
			float num2 = 1f - this.bossLeftHP;
			if (num2 <= 0.1f)
			{
				num2 = 0.1f;
			}
			float num3 = (float)gcount * num2;
			num = (int)num3;
			if (num == 0)
			{
				return;
			}
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsID);
		if (goodsXmlNodeByID != null)
		{
			GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(goodsID, forgeLevel, zhuijiaLevel, zhuoyueIndex, lucky, binding, num, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			GGoodIcon ggoodIcon;
			if (dummyGoodsDataMu != null)
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
				ggoodIcon.Width = 64.0;
				ggoodIcon.Height = 64.0;
				ggoodIcon.ItemCategory = categoriy;
				ggoodIcon.ItemObject = dummyGoodsDataMu;
				ggoodIcon.isAutoSize = true;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
				ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
				{
					goodsImageURLFromIconCode
				}), false, 0);
				ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
				bool canUse = Global.CanUseGoods(dummyGoodsDataMu.GoodsID, false, true);
				Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, canUse, IconTextTypes.Qianghua);
			}
			else
			{
				ggoodIcon = U3DUtils.NEW<GGoodIcon>();
				ggoodIcon.Width = 64.0;
				ggoodIcon.Height = 64.0;
				ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
			}
			this.ItemCollection.Add(ggoodIcon);
			ggoodIcon.SecondText.transform.localPosition = new Vector3(ggoodIcon.SecondText.transform.localPosition.x, ggoodIcon.SecondText.transform.localPosition.y, -3f);
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
			ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		}
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
	}

	private void LoadEffect(Vector3 localPosition)
	{
		GameObject effect = this.GetEffect("yaosai_dianliang");
		U3DUtils.AddChild(this.mListBox.gameObject, effect, false);
		effect.transform.localPosition = localPosition;
	}

	private GameObject GetEffect(string effectName)
	{
		string text = string.Format("{0}{1}", "UITeXiao/Perfabs/yaosai/", effectName);
		return Object.Instantiate(Resources.Load(text)) as GameObject;
	}

	private string Describe
	{
		set
		{
			this.mLblDescribe.Text = value;
		}
	}

	private void StartCountDown(int second)
	{
		this.StopCountDown();
		base.StartCoroutine<bool>(this.DaoJiShi(second, delegate
		{
			switch (this.mAwardType)
			{
			case YaoSaiJingLingBattleAwardsPart.EAwardType.Battle:
				this.CloseThisWindow();
				if (this.BossOwnerId != Global.Data.RoleID)
				{
					GameInstance.Game.SendYaoSaiMainBossMainInfoRequest(this.BossOwnerId);
					JingLingMap.inst.mapmini.RequestRelation();
					this.CloseBossInfoWindow();
				}
				break;
			case YaoSaiJingLingBattleAwardsPart.EAwardType.BossDead:
				GameInstance.Game.SendYaoSaiMainBossMainInfoRequest(Global.Data.roleData.RoleID);
				this.CloseThisWindow();
				this.CloseBossInfoWindow();
				break;
			case YaoSaiJingLingBattleAwardsPart.EAwardType.TaskAwark:
				this.Close();
				break;
			}
		}));
	}

	private void StopCountDown()
	{
		base.StopCoroutine("DaoJiShi");
	}

	private IEnumerator DaoJiShi(int time, Action act = null)
	{
		string describe = Global.GetLang("秒后关闭");
		for (int i = time; i > 0; i--)
		{
			this.mLblCoutDown.Text = string.Format("{0}{1}", i, describe);
			yield return new WaitForSeconds(1f);
		}
		if (act != null)
		{
			act.Invoke();
		}
		yield break;
	}

	protected override void OnDestroy()
	{
		this.StopCountDown();
		this.CloseHandler = null;
		this.mBtnClose = null;
		this.mLblTitle = null;
		this.mListBox = null;
		this.mBtnConfirm = null;
		this.mDescribeObj = null;
		this.mLblDescribe = null;
		this.mContentObj = null;
		this.mCountDownObj = null;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblTitle;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public GButton mBtnConfirm;

	public GameObject mDescribeObj;

	public TextBlock mLblDescribe;

	public GameObject mContentObj;

	public GameObject mCountDownObj;

	public TextBlock mLblCoutDown;

	public GameObject mTips;

	public TextBlock mLblTips;

	private string mTipsContent;

	public ShowNetImage m_ShowFanBei;

	private YaoSaiJingLingBattleAwardsPart.EAwardType mAwardType;

	private int BossOwnerId;

	private int coutDown_sec = 10;

	private float bossLeftHP;

	private int BossId;

	private enum EAwardType
	{
		None,
		BossDeadPreview,
		Battle,
		BossDead,
		TaskAwark
	}
}
