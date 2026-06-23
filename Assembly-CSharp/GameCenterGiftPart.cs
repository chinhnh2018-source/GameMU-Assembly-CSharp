using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class GameCenterGiftPart : UserControl
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
		this.InitAwardGoods();
	}

	private void InitTextInPrefabs()
	{
		this.mBtnAward.Text = Global.GetLang("领取");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseWindow();
		};
		this.mBtnAward.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendGameCenterAwardMsg(0);
		};
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
	}

	private void InitAwardGoods()
	{
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("AliGifts", '|');
		if (systemParamStringArrayByName == null || systemParamStringArrayByName.Length == 0)
		{
			return;
		}
		for (int i = 0; i < systemParamStringArrayByName.Length; i++)
		{
			GGoodIcon icon = Global.LoadRewardItemGoodsIcon(systemParamStringArrayByName[i], false, true, true);
			this.ItemCollection.Add(icon);
			UIPanel component = icon.gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			icon.MouseLeftButtonUp = null;
			icon.addEventListener("click", delegate(MouseEvent e)
			{
				GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
				if (null == ggoodIcon)
				{
					return;
				}
				GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
				if (goodsData == null)
				{
					return;
				}
				GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			});
		}
	}

	public void RespondAward(int ret, int param)
	{
		if (ret >= 0)
		{
			Global.IsOpenGameFromGameCenter = false;
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
		}
		this.CloseWindow();
	}

	private void CloseWindow()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, new DPSelectedItemEventArgs
			{
				IDType = 0
			});
		}
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public GButton mBtnAward;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;
}
