using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class SystemHongBaoTipIcon : UserControl
{
	private void OnEnable()
	{
		this.InitValue();
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
	}

	private void InitEvent()
	{
		UIEventListener.Get(this.mPanel).onClick = delegate(GameObject s)
		{
			this.SendShowHongBaoRequest();
		};
	}

	public void SendShowHongBaoRequest()
	{
		if (this.GetHongBaoCount() > 0)
		{
			SystemHongBaoTipIcon.TypeAndId hongBaoTypeAndId = this.GetHongBaoTypeAndId();
			int type = hongBaoTypeAndId.type;
			int id = hongBaoTypeAndId.id;
			if (id > 0)
			{
				GameInstance.Game.SendShowHongBaoRequest(type, id, 1);
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang("红包都被抢光了"), 10, 3);
		}
	}

	private void InitValue()
	{
		this.RedPacketsAutomaticRecordMax = ConfigSystemParam.GetSystemParamByName("RedPacketsAutomaticRecordMax", true);
		if (!string.IsNullOrEmpty(this.RedPacketsAutomaticRecordMax))
		{
			this.maxCount_cfg = ConvertExt.SafeConvertToInt32(this.RedPacketsAutomaticRecordMax);
		}
	}

	public void NotifyInitUIDataByServerData()
	{
		int hongBaoCount = this.GetHongBaoCount();
		if (hongBaoCount > 0)
		{
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameHintQueueIcon != null)
			{
				PlayZone.GlobalPlayZone.GameHintQueueIcon.HideIconHint();
			}
			if (hongBaoCount >= this.maxCount_cfg)
			{
				this.mNum.Text = this.maxCount_cfg.ToString();
			}
			else
			{
				this.mNum.Text = hongBaoCount.ToString();
			}
		}
		else
		{
			if (PlayZone.GlobalPlayZone != null && PlayZone.GlobalPlayZone.GameHintQueueIcon != null)
			{
				PlayZone.GlobalPlayZone.GameHintQueueIcon.ShowIconHint();
			}
			this.mNum.Text = "0";
			base.gameObject.SetActive(false);
		}
	}

	public int GetHongBaoCount()
	{
		if (Global.Data == null || Global.Data.mSystemHongBaoTipsDataDict == null)
		{
			return 0;
		}
		Dictionary<int, List<HongBaoTipData>> mSystemHongBaoTipsDataDict = Global.Data.mSystemHongBaoTipsDataDict;
		if (mSystemHongBaoTipsDataDict == null || mSystemHongBaoTipsDataDict.Count <= 0)
		{
			return 0;
		}
		Dictionary<int, List<HongBaoTipData>>.Enumerator enumerator = mSystemHongBaoTipsDataDict.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			if (Global.GetMapSceneUIClass() == SceneUIClasses.KuaFuMap || Global.GetMapSceneUIClass() == SceneUIClasses.Comp || Global.GetMapSceneUIClass() == SceneUIClasses.CompBattle)
			{
				KeyValuePair<int, List<HongBaoTipData>> keyValuePair = enumerator.Current;
				if (keyValuePair.Key == 102)
				{
					continue;
				}
			}
			KeyValuePair<int, List<HongBaoTipData>> keyValuePair2 = enumerator.Current;
			List<HongBaoTipData> value = keyValuePair2.Value;
			if (value != null && value.Count > 0)
			{
				num += value.Count;
			}
		}
		return num;
	}

	private SystemHongBaoTipIcon.TypeAndId GetHongBaoTypeAndId()
	{
		if (Global.Data == null || Global.Data.mSystemHongBaoTipsDataDict == null)
		{
			return new SystemHongBaoTipIcon.TypeAndId
			{
				type = 0,
				id = 0
			};
		}
		Dictionary<int, List<HongBaoTipData>> mSystemHongBaoTipsDataDict = Global.Data.mSystemHongBaoTipsDataDict;
		if (mSystemHongBaoTipsDataDict == null || mSystemHongBaoTipsDataDict.Count <= 0)
		{
			return new SystemHongBaoTipIcon.TypeAndId
			{
				type = 0,
				id = 0
			};
		}
		Dictionary<int, List<HongBaoTipData>>.Enumerator enumerator = mSystemHongBaoTipsDataDict.GetEnumerator();
		int type = 0;
		int id = 0;
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, List<HongBaoTipData>> keyValuePair = enumerator.Current;
			type = keyValuePair.Key;
			KeyValuePair<int, List<HongBaoTipData>> keyValuePair2 = enumerator.Current;
			List<HongBaoTipData> value = keyValuePair2.Value;
			if (value != null && value.Count > 0)
			{
				id = value[0].hongBaoID;
				break;
			}
		}
		return new SystemHongBaoTipIcon.TypeAndId
		{
			type = type,
			id = id
		};
	}

	protected override void OnDestroy()
	{
	}

	public TextBlock mNum;

	public GameObject mPanel;

	private string RedPacketsAutomaticRecordMax = string.Empty;

	private int maxCount_cfg;

	private struct TypeAndId
	{
		public int type;

		public int id;
	}
}
