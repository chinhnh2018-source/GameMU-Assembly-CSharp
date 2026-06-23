using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameEngine.Sprite;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DaTaoShaGuanZhan : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.BtnConfirm.Label.text = Global.GetLang("确定");
	}

	private void InitEvent()
	{
		this.BtnSelect.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
		};
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
		};
		this.BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseUI();
			GameInstance.Game.SendGuanZhanTrackOtherPlayer(this.selectID);
			this.ClearItem();
		};
	}

	private void InitValue()
	{
		for (int i = 0; i < 4; i++)
		{
			DaTaoShaGuanZhanItem daTaoShaGuanZhanItem = U3DUtils.NEW<DaTaoShaGuanZhanItem>();
			NGUITools.AddChild2(this.parent.gameObject, daTaoShaGuanZhanItem);
			this.items.Add(daTaoShaGuanZhanItem);
			daTaoShaGuanZhanItem.transform.localPosition = new Vector3(0f, (float)(74 - i * 36), 0f);
			daTaoShaGuanZhanItem.gameObject.SetActive(false);
		}
	}

	public void InitValueByData(List<GuanZhanRoleMiniData> datas)
	{
		for (int i = 0; i < datas.Count; i++)
		{
			DaTaoShaGuanZhanItem item = this.items[i];
			item.transform.gameObject.SetActive(true);
			item.InitValue(datas[i]);
			item.ClickHandler = delegate(int s)
			{
				if (this.lastItem != null)
				{
					this.lastItem.IsSelect = false;
				}
				this.selectID = s;
				item.IsSelect = true;
				this.lastItem = item;
			};
		}
	}

	private void ClearItem()
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			this.items[i].transform.gameObject.SetActive(false);
		}
	}

	public void RespondGuanZhanData(GuanZhanData data)
	{
		if (data == null)
		{
			return;
		}
		Dictionary<int, List<GuanZhanRoleMiniData>> roleMiniDataDict = data.RoleMiniDataDict;
		Dictionary<int, List<GuanZhanRoleMiniData>>.Enumerator enumerator = roleMiniDataDict.GetEnumerator();
		List<GuanZhanRoleMiniData> list = null;
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, List<GuanZhanRoleMiniData>> keyValuePair = enumerator.Current;
			list = keyValuePair.Value;
		}
		if (list != null && list.Count > 0)
		{
			this.HideDeadBody();
			this.InitValueByData(list);
		}
	}

	private void HideDeadBody()
	{
		if (PlayZone.GlobalPlayZone != null)
		{
			PlayZone.GlobalPlayZone.CloseRoleLowLifeWindow();
		}
		GSprite gsprite = Global.FindSpriteByID(Global.Data.roleData.RoleID);
		Transform transform = null;
		if (gsprite != null)
		{
			transform = gsprite.The3DGameObject.transform;
		}
		if (transform != null)
		{
			int childCount = transform.childCount;
			if (childCount > 0 && transform.GetChild(0).gameObject.activeSelf)
			{
				for (int i = 0; i < childCount; i++)
				{
					transform.GetChild(i).gameObject.SetActive(false);
				}
				SkinnedMeshRenderer component = transform.GetComponent<SkinnedMeshRenderer>();
				if (component)
				{
					component.enabled = false;
				}
				BoxCollider component2 = transform.GetComponent<BoxCollider>();
				if (component2)
				{
					component2.enabled = false;
				}
			}
		}
	}

	private void CloseUI()
	{
		if (this.CloseHandler != null)
		{
			this.CloseHandler(null, null);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler ClickHandler;

	public GButton BtnSelect;

	public GButton BtnClose;

	public GButton BtnConfirm;

	public new Transform parent;

	private List<DaTaoShaGuanZhanItem> items = new List<DaTaoShaGuanZhanItem>();

	private int selectID;

	private DaTaoShaGuanZhanItem lastItem;
}
