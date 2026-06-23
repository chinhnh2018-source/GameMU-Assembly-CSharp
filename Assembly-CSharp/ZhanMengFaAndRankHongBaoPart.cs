using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengFaAndRankHongBaoPart : UserControl
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
	}

	private void InitTextInPrefabs()
	{
		if (this.staticText.Length > 1 && Context.IsHaiwai)
		{
			this.staticText[0].text = Global.GetLang("发红包");
			this.staticText[1].text = Global.GetLang("红包榜");
			this.staticText[2].text = Global.GetLang("我抢的红包");
			this.staticText[3].text = Global.GetLang("我发出的红包");
			this.staticText[4].text = Global.GetLang("红包总览");
		}
		try
		{
			this.mBtnFaHongBaoRank.Text = Global.GetLang("发红包榜");
			this.mBtnQiangHongBaoRank.Text = Global.GetLang("抢红包榜");
			this.PaiHang.text = Global.GetLang("排行");
			this.WanJia.text = Global.GetLang("玩家");
			this.mBtnFaHongBaoRank.Label.pivot = 3;
			this.mBtnFaHongBaoRank.Label.transform.localPosition = new Vector3(16f, this.mBtnFaHongBaoRank.Label.transform.localPosition.y, this.mBtnFaHongBaoRank.Label.transform.localPosition.z);
			this.mBtnQiangHongBaoRank.Label.pivot = 3;
			this.mBtnQiangHongBaoRank.Label.transform.localPosition = new Vector3(16f, this.mBtnQiangHongBaoRank.Label.transform.localPosition.y, this.mBtnQiangHongBaoRank.Label.transform.localPosition.z);
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.Message
			});
		}
	}

	private void InitEvent()
	{
		if (this.mTab != null)
		{
			this.mTab.TabClick += delegate(GameObject s, int index)
			{
				this.ShowTabType((ZhanMengFaAndRankHongBaoPart.EHongBaoTabType)index);
			};
		}
		if (this.mBtnFaHongBaoRank != null)
		{
			this.mBtnFaHongBaoRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.mPanel != null)
				{
					this.mPanel.transform.localPosition = this.origionalPosition;
					this.mPanel.clipRange = this.origionalClipRange;
				}
				this.SetBtnSelectStatus(this.mBtnFaHongBaoRank);
				GameInstance.Game.SendHongBaoRankRequest(1);
			};
		}
		if (this.mBtnQiangHongBaoRank != null)
		{
			this.mBtnQiangHongBaoRank.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.mPanel != null)
				{
					this.mPanel.transform.localPosition = this.origionalPosition;
					this.mPanel.clipRange = this.origionalClipRange;
				}
				this.SetBtnSelectStatus(this.mBtnQiangHongBaoRank);
				GameInstance.Game.SendHongBaoRankRequest(0);
			};
		}
	}

	private void InitValue()
	{
		this.ItemCollection = this.mListBox.ItemsSource;
	}

	private new void Start()
	{
		this.origionalClipRange = this.mPanel.clipRange;
		this.mTab.TabIndex = 1;
	}

	private void ShowTabType(ZhanMengFaAndRankHongBaoPart.EHongBaoTabType tabType)
	{
		if (tabType == ZhanMengFaAndRankHongBaoPart.EHongBaoTabType.FaHongBao)
		{
			NGUITools.SetActive(this.mZMFaHongBao.gameObject, true);
			NGUITools.SetActive(this.mRankObj, false);
		}
		else if (tabType == ZhanMengFaAndRankHongBaoPart.EHongBaoTabType.HongBaoRank)
		{
			this.mBtnFaHongBaoRank.Label.color = NGUIMath.HexToColorEx(14337966U);
			this.mBtnQiangHongBaoRank.Label.color = NGUIMath.HexToColorEx(14337966U);
			NGUITools.SetActive(this.mZMFaHongBao.gameObject, false);
			NGUITools.SetActive(this.mRankObj, true);
			GameInstance.Game.SendHongBaoRankRequest(1);
			this.SetBtnSelectStatus(this.mBtnFaHongBaoRank);
		}
	}

	private void SetBtnSelectStatus(GButton btn)
	{
		if (this.tempBtn != null)
		{
			if (this.tempBtn == btn)
			{
				btn.Label.color = NGUIMath.HexToColorEx(14922604U);
				return;
			}
			btn.Pressed = true;
			btn.Label.color = NGUIMath.HexToColorEx(14922604U);
			this.tempBtn.Pressed = false;
			this.tempBtn.Label.color = NGUIMath.HexToColorEx(14337966U);
			this.tempBtn = btn;
		}
		else
		{
			btn.Label.color = NGUIMath.HexToColorEx(14922604U);
			btn.Pressed = true;
			this.tempBtn = btn;
		}
	}

	public void InitUIDataByServerData()
	{
	}

	public void InitHongBaoRankDataByServerData(HongBaoRankData data, bool isRefresh)
	{
		if (data == null)
		{
			Super.HintMainText(Global.GetLang("暂无数据"), 10, 3);
			return;
		}
		if (this.mPanel != null)
		{
			this.mPanel.transform.localPosition = this.origionalPosition;
			this.mPanel.clipRange = this.origionalClipRange;
		}
		if (isRefresh)
		{
			this.ClearRankList();
		}
		List<HongBaoRankItemData> items = data.items;
		if (items == null || items.Count <= 0)
		{
			this.ClearRankList();
			return;
		}
		if (!isRefresh)
		{
			return;
		}
		this.ClearRankList();
		if (items == null || items.Count <= 0)
		{
			return;
		}
		base.StopCoroutine("LoadItems");
		base.StartCoroutine<bool>(this.LoadItems(items));
	}

	private IEnumerator LoadItems(List<HongBaoRankItemData> dataLists)
	{
		for (int i = 0; i < dataLists.Count; i++)
		{
			if (i % 5 == 0)
			{
				yield return null;
			}
			HongBaoRankItemData itemData = dataLists[i];
			ZhanMengHongBaoRankItem item = U3DUtils.NEW<ZhanMengHongBaoRankItem>();
			int id = i + 1;
			if (id <= 3)
			{
				item.SetSpriteActivity(true);
				item.SpriteRankID = itemData.rankID;
			}
			else
			{
				item.SetSpriteActivity(false);
				item.LblRankID = itemData.rankID;
			}
			item.Name = itemData.roleName;
			UIPanel temppanel = item.transform.GetComponent<UIPanel>();
			if (temppanel != null)
			{
				Object.Destroy(temppanel);
			}
			this.ItemCollection.AddNoUpdate(item);
		}
		yield break;
	}

	private void ClearRankList()
	{
		this.ItemCollection.Clear();
	}

	protected override void OnDestroy()
	{
		base.StopCoroutine("LoadItems");
		this.mTab = null;
		this.mListBox = null;
		this.mZMFaHongBao = null;
		this._ItemCollection = null;
	}

	private const int kRankType_QiangHongBao = 0;

	private const int kRankType_FaHongBao = 1;

	public UILabel[] staticText;

	public UITab mTab;

	public ListBox mListBox;

	public ZhanMengFaHongBaoPart mZMFaHongBao;

	private ObservableCollection _ItemCollection;

	public GameObject mRankObj;

	public GButton mBtnFaHongBaoRank;

	public GButton mBtnQiangHongBaoRank;

	public UILabel PaiHang;

	public UILabel WanJia;

	private Vector3 origionalPosition = Vector3.zero;

	private Vector4 origionalClipRange = Vector4.zero;

	public UIPanel mPanel;

	private GButton tempBtn;

	private enum EHongBaoTabType
	{
		FaHongBao = 1,
		HongBaoRank
	}
}
