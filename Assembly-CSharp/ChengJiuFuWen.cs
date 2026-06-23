using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChengJiuFuWen : UserControl
{
	protected override void InitializeComponent()
	{
		try
		{
			this.BtnTiSheng.Label.text = Global.GetLang("提升");
			this.BtnTiSheng.Label.transform.localPosition = new Vector3(0f, 0f, -0.1f);
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"东南亚英文，ChengJiuFuWen.cs报空!"
			});
		}
		this.ExtraBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ExtraUI();
		};
		this.BtnTiSheng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.nowTime = Global.GetCorrectLocalTime();
			if (this.nowTime - this.lastTime < 100L)
			{
				Super.HintMainText(Global.GetLang("请不要过于频繁操作！"), 10, 3);
				this.lastTime = this.nowTime;
				return;
			}
			this.lastTime = this.nowTime;
			if (Super.MessageBoxIsHint[5] == 0)
			{
				DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
				{
					if (args.ID == 0)
					{
						this.ClickTiShengPro();
					}
					else
					{
						Super.MessageBoxIsHint[5] = 0;
					}
				};
				if (!this.cjPro.stoneLabel.text.Equals(Global.GetLang("免费")))
				{
					Super.ZuanShiShowMessageBox(Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						string.Format(Global.GetLang("本次操作需要花费钻石        {0} ,  确认要执行该操作吗？"), this.cjPro.stoneLabel.text)
					}), 2, handler, MessBoxIsHintTypes.ChengJiuHuaFeiZuanShiNeedHint, -15f, "ChengJieFuWen", Global.Data.ChengjiuFuWen.Diamond);
				}
				else
				{
					this.ClickTiShengPro();
				}
			}
			else
			{
				this.ClickTiShengPro();
			}
		};
		this.fwim.DPSelectedItem = new DPSelectedItemEventHandler(this.ClickFuWen);
		this.cjPro.Init();
		this.fwim.init();
	}

	private void initAnimationObject(GameObject parent)
	{
		if (this.animation != null)
		{
			this.animation.transform.parent = parent.transform;
			this.animation.transform.localPosition = new Vector3(-40f, 43f, -1f);
			this.animation.transform.localScale = Vector3.one;
		}
		if (this.animation == null)
		{
			GameObject original = Resources.Load("UITeXiao/ChengJiuJieMian/FuWenJieSuo/FuWenJieSuo_effect") as GameObject;
			this.animation = (SpawnManager.Instantiate(original) as GameObject);
			this.animation.transform.parent = parent.transform;
			this.animation.transform.localPosition = new Vector3(-40f, 43f, -1f);
			this.animation.transform.localScale = Vector3.one;
			if (!this.animation.activeSelf)
			{
				this.animation.SetActive(true);
			}
		}
		else if (!this.animation.activeSelf)
		{
			this.animation.SetActive(true);
		}
		else
		{
			this.animation.SetActive(false);
			this.animation.SetActive(true);
		}
	}

	public void StartUI(int type = 0)
	{
		if (Global.Data.ChengjiuFuWen.UpResultType == 3)
		{
			this.FuWenType = Global.Data.ChengjiuFuWen.RuneID - 1;
		}
		else
		{
			this.FuWenType = Global.Data.ChengjiuFuWen.RuneID;
		}
		this.InitUI(type);
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "ChengJieFuWen", Global.Data.ChengjiuFuWen.Diamond, string.Empty);
	}

	private void InitUI(int type)
	{
		if (Global.Data.ChengjiuFuWen.UpResultType == 2 && type == 0)
		{
			this.lockIs = true;
			this.initAnimationObject(this.fwim.getLocalPosition(this.FuWenType));
			this.fwim.showKaiQiFuWenLine();
			this.refreshData(this.FuWenType - 1);
			base.StopCoroutine("AddGoodListIcon");
			base.StartCoroutine<bool>(this.AddGoodListIcon());
		}
		else
		{
			this.fwim.showKaiQiFuWenLine();
			this.fwim.initUI();
			this.refreshData(this.FuWenType);
		}
	}

	public IEnumerator AddGoodListIcon()
	{
		yield return new WaitForSeconds(2.2f);
		this.lockIs = false;
		this.animation.SetActive(false);
		this.cjPro.setStarVisible(false, -1);
		this.fwim.initUI();
		this.refreshData(this.FuWenType);
		yield break;
	}

	public void ClickFuWen(object sender, DPSelectedItemEventArgs args)
	{
		if (this.FuWenType == args.ID)
		{
			return;
		}
		if (!this.fwim.isFuWen(args.ID))
		{
			return;
		}
		this.cjPro.setStarVisible(false, -1);
		this.fwim.setBtnChoise(this.FuWenType, args.ID);
		this.FuWenType = args.ID;
		this.refreshData(this.FuWenType);
	}

	public void refreshData(int id = 1)
	{
		if (Global.Data.ChengjiuFuWen.RuneID > id)
		{
			this.BtnTiSheng.isEnabled = false;
		}
		else
		{
			this.BtnTiSheng.isEnabled = true;
		}
		this.cjPro.refreshUI(id, 0);
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "ChengJieFuWen", Global.Data.ChengjiuFuWen.Diamond, string.Empty);
	}

	public void initData()
	{
		GameInstance.Game.FuWenChengJiuInfo();
	}

	private void ExtraUI()
	{
		if (!this.cjr.gameObject.activeSelf)
		{
			this.cjr.gameObject.SetActive(true);
			this.cjr.setFuWenExtarProUI();
			return;
		}
	}

	private void ClickTiShengPro()
	{
		if (Global.Data.ChengjiuFuWen != null)
		{
			if ((long)Global.Data.ChengjiuFuWen.Achievement > Global.Data.ChengJiuData.ChengJiuPoints)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedChengJiu, null, string.Empty, string.Empty);
				return;
			}
			if (Global.Data.ChengjiuFuWen.Diamond > Global.Data.roleData.UserMoney)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
			GameInstance.Game.FuWenChengJiuTiSheng(this.FuWenType);
		}
	}

	public void UpdatePro()
	{
		if (this.lockIs)
		{
			return;
		}
		this.cjPro.refreshUI(this.FuWenType, 1);
		this.fwim.UpdateUI(this.FuWenType);
		if (Global.Data.ChengjiuFuWen.BurstType == 1)
		{
			this.cjPro.CreatePre(1);
		}
		else if (Global.Data.ChengjiuFuWen.BurstType == 2)
		{
			this.cjPro.CreatePre(2);
		}
		else if (Global.Data.ChengjiuFuWen.BurstType == 0)
		{
			this.cjPro.CreatePre(0);
		}
		IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.listDaiBi[0], "ChengJieFuWen", Global.Data.ChengjiuFuWen.Diamond, string.Empty);
	}

	public void ErrorInfo(int index)
	{
		if (index == -1)
		{
			Super.HintMainText(Global.GetLang("此系统暂时未开放！"), 10, 3);
		}
		else
		{
			if (index == -2)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedChengJiu, null, string.Empty, string.Empty);
				return;
			}
			if (index == -3)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				return;
			}
			if (index == -4)
			{
				Super.HintMainText(Global.GetLang("全部开启!"), 10, 3);
			}
			else if (index == 0)
			{
				Super.HintMainText(Global.GetLang("提升失败！"), 10, 3);
			}
		}
	}

	public ChengJiuProShuaXin cjPro;

	public FuWenItemManager fwim;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton ExtraBtn;

	public GButton BtnTiSheng;

	public ChengJiuExtraPro cjr;

	public List<UISprite> listDaiBi = new List<UISprite>();

	private int FuWenType = 1;

	private GameObject animation;

	private bool lockIs;

	private long nowTime;

	private long lastTime;

	private enum AchievementRuneResultType
	{
		End = 3,
		Next = 2,
		Success = 1,
		Efail = 0,
		EnoOpen = -1,
		EnoAchievement = -2,
		EnoDiamond = -3,
		EOver = -4
	}
}
