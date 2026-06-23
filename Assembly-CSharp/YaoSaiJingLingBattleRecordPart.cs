using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class YaoSaiJingLingBattleRecordPart : UserControl
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
		this.mLblMingCi.Text = Global.GetLang("名字");
		this.mLblName.Text = Global.GetLang("玩家名称");
		this.mLblInvite.Text = Global.GetLang("关系");
		this.mLblDamage.Text = Global.GetLang("伤害量");
		this.mLblTitle.text = Global.GetLang("战斗记录");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
	}

	private void InitValue()
	{
	}

	public void RefreshItemDataByServerData(YaoSaiBossFightLogInfo data)
	{
		if (data == null)
		{
			return;
		}
		base.StopCoroutine("LoadItems");
		base.StartCoroutine<bool>(this.LoadItems(data));
	}

	private IEnumerator LoadItems(YaoSaiBossFightLogInfo data)
	{
		if (data == null)
		{
			yield break;
		}
		List<YaoSaiBossFightLog> bossFightLogList = data.BossFightLogList;
		if (bossFightLogList == null || bossFightLogList.Count <= 0)
		{
			yield break;
		}
		this.ItemCollection.Clear();
		for (int i = 0; i < bossFightLogList.Count; i++)
		{
			YaoSaiJingLingBattleRecordItem item = U3DUtils.NEW<YaoSaiJingLingBattleRecordItem>();
			YaoSaiBossFightLog recordItemData = bossFightLogList[i];
			item.InitRecordDataByServerData(recordItemData, i + 1);
			UIPanel temppanel = item.transform.GetComponent<UIPanel>();
			if (temppanel != null)
			{
				Object.Destroy(temppanel);
			}
			this.ItemCollection.Add(item);
			yield return null;
		}
		yield break;
	}

	protected override void OnDestroy()
	{
		base.StopCoroutine("LoadItems");
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton mBtnClose;

	public TextBlock mLblTitle;

	public TextBlock mLblMingCi;

	public TextBlock mLblName;

	public TextBlock mLblInvite;

	public TextBlock mLblDamage;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;
}
