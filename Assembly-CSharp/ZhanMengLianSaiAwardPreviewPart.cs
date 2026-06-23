using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiAwardPreviewPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("赛季奖励")
			});
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0
					});
				}
			};
			this.mObservableCollectionNew = this.mListBoxNewAward.ItemsSource;
			this.mObservableCollectionSuper = this.mListBoxSuperAward.ItemsSource;
		}
		catch (Exception ex)
		{
		}
	}

	private IEnumerator InitNewAwardView()
	{
		List<int> lst = this.mConfigZhanMengLianSaiLeagueNewAward.GetAllAwardID();
		if (0 < lst.Count)
		{
			int i = 0;
			while (i < lst.Count)
			{
				List<GoodsData> NewAwardDataList = this.mConfigZhanMengLianSaiLeagueNewAward.GetLianSaiAwardVOByID(lst[i]).Goods;
				if (0 < NewAwardDataList.Count)
				{
					ZhanMengLianSaiAwarditem item = U3DUtils.NEW<ZhanMengLianSaiAwarditem>();
					item.DraggablePanel = this.mDragPanelNewAwardView_;
					item.RefreahInf(NewAwardDataList, this.mConfigZhanMengLianSaiLeagueNewAward.GetLianSaiAwardVOByID(lst[i]).Name);
					this.mObservableCollectionNew.AddNoUpdate(item.gameObject);
				}
				i++;
				if (i % 3 == 0)
				{
					yield return null;
				}
			}
			this.mListBoxNewAward.repositionNow = true;
			this.mDragPanelNewAwardView_.ResetPosition();
		}
		yield break;
	}

	private IEnumerator InitSuperAwardView()
	{
		List<int> lst = this.mConfigZhanMengLianSaiLeagueSuperAward.GetAllAwardID();
		if (0 < lst.Count)
		{
			int i = 0;
			while (i < lst.Count)
			{
				List<GoodsData> AwardDataList = this.mConfigZhanMengLianSaiLeagueSuperAward.GetLianSaiLeagueAwardVOByID(lst[i]).Goods;
				if (0 < AwardDataList.Count)
				{
					ZhanMengLianSaiAwarditem item = U3DUtils.NEW<ZhanMengLianSaiAwarditem>();
					item.DraggablePanel = this.mDragPanelSuperAwardView_;
					item.RefreahInf(AwardDataList, this.mConfigZhanMengLianSaiLeagueSuperAward.GetLianSaiLeagueAwardVOByID(lst[i]).Name);
					this.mObservableCollectionSuper.AddNoUpdate(item.gameObject);
				}
				i++;
				if (i % 3 == 0)
				{
					yield return null;
				}
			}
			this.mListBoxSuperAward.repositionNow = true;
			this.mDragPanelSuperAwardView_.ResetPosition();
		}
		yield break;
	}

	public void RefreshView(ConfigZhanMengLianSaiLeagueNewAward NewAwardxml, ConfigZhanMengLianSaiLeagueSuperAward SuperAwardxml)
	{
		this.mConfigZhanMengLianSaiLeagueNewAward = NewAwardxml;
		this.mConfigZhanMengLianSaiLeagueSuperAward = SuperAwardxml;
		if (this.mConfigZhanMengLianSaiLeagueNewAward != null)
		{
			base.StartCoroutine<bool>(this.InitNewAwardView());
		}
		if (this.mConfigZhanMengLianSaiLeagueSuperAward != null)
		{
			base.StartCoroutine<bool>(this.InitSuperAwardView());
		}
	}

	[SerializeField]
	private UILabel mTitleLabel;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UIDraggablePanel mDragPanelNewAwardView_;

	[SerializeField]
	private ListBox mListBoxNewAward;

	[SerializeField]
	private UIDraggablePanel mDragPanelSuperAwardView_;

	[SerializeField]
	private ListBox mListBoxSuperAward;

	private ConfigZhanMengLianSaiLeagueNewAward mConfigZhanMengLianSaiLeagueNewAward;

	private ConfigZhanMengLianSaiLeagueSuperAward mConfigZhanMengLianSaiLeagueSuperAward;

	private ObservableCollection mObservableCollectionNew;

	private ObservableCollection mObservableCollectionSuper;

	public DPSelectedItemEventHandler Hander;
}
