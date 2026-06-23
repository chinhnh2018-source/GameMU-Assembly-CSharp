using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class LianzhiPart : UserControl
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
		this.BtnSubmit.Text = Global.GetLang("转换");
		this.ConstHintText.Text = Global.GetLang("小提示：VIP等级越高，每日炼制次数越多");
		this.BtnSubmit.gameObject.transform.localPosition = new Vector3(290f, -175f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		GameInstance.Game.GetJieriFanbeiInfo();
		this.CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
		};
		this.BtnSubmit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.StartSubmit();
		};
		this.BtnSubmitAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
	}

	public void InitPartData(int rewardType = -1)
	{
		this.ItemCollection = this.List.ItemsSource;
		this.List.SelectionChanged = new MouseLeftButtonUpEventHandler(this.ListMouseLeftButtonUp);
		this.InitFanbei();
		this.selectItem(0);
		this.GetLianzhiCount();
	}

	private void InitFanbei()
	{
		this.obj_bothAward.gameObject.SetActive(false);
		this.rewardFanbei = U3DUtils.NEW<FanbeiPrefab>();
		this.rewardFanbei.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/RewartDouble.png";
		this.obj_reward.Add(this.rewardFanbei);
		this.obj_reward.gameObject.SetActive(false);
		this.timesFanbei = U3DUtils.NEW<FanbeiPrefab>();
		this.timesFanbei.tetUrl.URL = "NetImages/GameRes/Images/JieriFanbei/TimesDouble.png";
		this.obj_times.Add(this.timesFanbei);
		this.obj_times.gameObject.SetActive(false);
	}

	public void LoadList(int rewardType = -1)
	{
		if (Global.isFanbei(6) && Global.isFanbei(9))
		{
			this.obj_reward.gameObject.SetActive(false);
			this.obj_times.gameObject.SetActive(false);
			this.obj_bothAward.gameObject.SetActive(true);
		}
		else if (Global.isFanbei(209) && Global.isFanbei(210))
		{
			this.obj_reward.gameObject.SetActive(false);
			this.obj_times.gameObject.SetActive(false);
			this.obj_bothAward.gameObject.SetActive(true);
		}
		else
		{
			this.obj_bothAward.gameObject.SetActive(false);
			if (Global.isFanbei(6) || Global.isFanbei(209))
			{
				this.obj_reward.gameObject.SetActive(false);
				this.obj_times.gameObject.SetActive(true);
			}
			if (Global.isFanbei(9) || Global.isFanbei(210))
			{
				this.obj_reward.gameObject.SetActive(true);
				this.obj_times.gameObject.SetActive(false);
			}
		}
		this.ItemCollection.Clear();
		LianzhiItem lianzhiItem = U3DUtils.NEW<LianzhiItem>();
		lianzhiItem.Type = LianzhiTypes.Diamond;
		lianzhiItem.SelectStat = false;
		this.ItemCollection.AddNoUpdate(lianzhiItem);
		lianzhiItem = U3DUtils.NEW<LianzhiItem>();
		lianzhiItem.Type = LianzhiTypes.BindDiamond;
		lianzhiItem.SelectStat = false;
		this.ItemCollection.AddNoUpdate(lianzhiItem);
		lianzhiItem = U3DUtils.NEW<LianzhiItem>();
		lianzhiItem.Type = LianzhiTypes.Gold;
		lianzhiItem.SelectStat = false;
		this.ItemCollection.AddNoUpdate(lianzhiItem);
		this.ItemCollection.DelayUpdate();
	}

	private void ListMouseLeftButtonUp(object sender, MouseEvent e)
	{
		LianzhiItem lianzhiItem = U3DUtils.AS<LianzhiItem>(this.List.SelectedItem);
		if (null == lianzhiItem)
		{
			return;
		}
		if (this.SelectedItem != null && this.SelectedItem != lianzhiItem)
		{
			this.SelectedItem.SelectStat = false;
		}
		if (this.SelectedItem == lianzhiItem)
		{
			return;
		}
		this.SelectedItem = lianzhiItem;
		this.SelectedItem.SelectStat = true;
		this.nIndex = this.List.SelectedIndex;
	}

	private void showFanbei(int index)
	{
		if (index == 0)
		{
			this.obj_reward.gameObject.SetActive(false);
		}
	}

	private void selectItem(int index)
	{
		this.List.SelectedIndex = index;
		LianzhiItem lianzhiItem = U3DUtils.AS<LianzhiItem>(this.List.SelectedItem);
		if (null == lianzhiItem)
		{
			return;
		}
		lianzhiItem.SelectStat = true;
		this.SelectedItem = lianzhiItem;
	}

	private void RefreshList(int type, int count, List<int> resultList)
	{
		if (type > -1 && count > 0)
		{
			if (null != this.SelectedItem)
			{
				this.SelectedItem.LianzhiNum = this.SelectedItem.LianzhiNum - 1;
			}
		}
		else
		{
			if (resultList == null || resultList.Count != 3)
			{
				return;
			}
			for (int i = 0; i < this.ItemCollection.Length; i++)
			{
				LianzhiItem lianzhiItem = U3DUtils.AS<LianzhiItem>(this.ItemCollection[i]);
				lianzhiItem.LianzhiNum -= resultList[(int)lianzhiItem.Type];
			}
		}
	}

	private void GetLianzhiCount()
	{
		this.ShowModalDialog();
		GameInstance.Game.SpriteQueryLianzhiNumCmd();
	}

	public void NotifyLianzhiCount(int result, List<int> resultList)
	{
		this.CloseModalDialog();
		if (result <= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("炼制时发生错误:{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
		else if (result == 1)
		{
			this.RefreshList(-1, 0, resultList);
		}
	}

	private void StartSubmit()
	{
		if (null == this.SelectedItem)
		{
			return;
		}
		if (this.SelectedItem.LianzhiNum <= 0)
		{
			string text = string.Empty;
			if (Global.GetVIPLeve() <= 0)
			{
				text = Global.GetLang(",成为vip可以提高转换次数");
			}
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("今日可用次数为0") + text, new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.SelectedItem.Type == LianzhiTypes.Diamond && Global.Data.roleData.UserMoney < this.SelectedItem.LianzhiFromMoneyValue)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("钻石不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.SelectedItem.Type == LianzhiTypes.BindDiamond && Global.Data.roleData.Gold < this.SelectedItem.LianzhiFromMoneyValue)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("绑钻不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		if (this.SelectedItem.Type == LianzhiTypes.Gold && Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < this.SelectedItem.LianzhiFromMoneyValue)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币不足"), new object[0]), 0, -1, -1, 0);
			return;
		}
		this.ShowModalDialog();
		GameInstance.Game.SpriteExeLianzhiCmd((int)this.SelectedItem.Type, 1);
	}

	public void NotifyLianzhiResult(int result, int type, int count)
	{
		this.CloseModalDialog();
		if (result <= 0)
		{
			if (result == -9)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("金币不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -10)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("钻石不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -17)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("绑钻不足"), new object[0]), 0, -1, -1, 0);
			}
			else if (result == -16)
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("无剩余次数"), new object[0]), 0, -1, -1, 0);
			}
			else
			{
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("炼制时发生错误:{0}"), new object[]
				{
					result
				}), 0, -1, -1, 0);
			}
			return;
		}
		if (result == 1)
		{
			this.RefreshList(type, count, null);
			if (null != this.SelectedItem && this.SelectedItem.AwardsData != null)
			{
				int num = 0;
				if (Global.isFanbei(9))
				{
					num = 2;
				}
				if (Global.isFanbei(210))
				{
					double num2 = 0.0;
					if (double.TryParse(Global.JieriFanbeiInfo[210].ExtArg1, ref num2))
					{
						num += (int)num2;
					}
				}
				if (num == 0)
				{
					num = 1;
				}
				long num3 = this.SelectedItem.AwardsData.Experienceaward * (long)num;
				if (this.SelectedItem.Type == LianzhiTypes.Diamond || this.SelectedItem.Type == LianzhiTypes.Gold)
				{
					GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("您获得了：经验 + {0}"), new object[]
					{
						num3
					}), 0, -1, -1, 0);
				}
			}
		}
	}

	public void ShowModalDialog()
	{
		Super.ShowNetWaiting(string.Empty);
	}

	public void CloseModalDialog()
	{
		Super.HideNetWaiting();
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton CloseBtn;

	public ListBox List;

	public GButton BtnSubmit;

	public GButton BtnSubmitAll;

	public TextBlock ConstHintText;

	public SpriteSL obj_reward;

	public SpriteSL obj_times;

	public SpriteSL obj_bothAward;

	private FanbeiPrefab rewardFanbei;

	private FanbeiPrefab timesFanbei;

	private int nIndex;

	private ObservableCollection _ItemCollection;

	private LianzhiItem SelectedItem;
}
