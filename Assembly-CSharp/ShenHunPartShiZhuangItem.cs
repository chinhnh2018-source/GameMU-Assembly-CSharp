using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class ShenHunPartShiZhuangItem : UserControl
{
	public GoodsData GoodsData
	{
		get
		{
			return this.m_goodsData;
		}
		set
		{
			this.m_goodsData = value;
		}
	}

	public bool BeOwn
	{
		get
		{
			return this.m_goodsData != null;
		}
	}

	public MUTransfigurationFashion Fashion
	{
		get
		{
			return this.m_fashion;
		}
	}

	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public bool BSelect
	{
		get
		{
			return this.m_beSelect;
		}
		set
		{
			this.m_beSelect = value;
			NGUITools.SetActive(this.selecetObj, this.m_beSelect);
		}
	}

	public bool BeEquip
	{
		get
		{
			return this.m_beEquip;
		}
		set
		{
			this.m_beEquip = value;
			NGUITools.SetActive(this.equipObj, this.m_beEquip);
		}
	}

	public void IntShiZhuang(MUTransfigurationFashion fashion, GoodsData goodsData)
	{
		this.m_fashion = fashion;
		GGoodIcon ggoodIcon = Global.LoadRewardItemGoodsIconByGoodsID(this.m_fashion.GoodsID, false);
		ggoodIcon.transform.SetParent(this._GoodsRoot);
		ggoodIcon.transform.localScale = new Vector3(1f, 1f, 1f);
		ggoodIcon.transform.localPosition = Vector3.zero;
		this.m_goodsData = goodsData;
		ggoodIcon.EnableIcon = this.BeOwn;
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		this.selecetObj.SetActive(false);
	}

	private void MouseLeftButtonUp(MouseEvent e)
	{
		if (this.OnSelectShiZhuang != null)
		{
			this.OnSelectShiZhuang.Invoke(this);
		}
	}

	public Action<ShenHunPartShiZhuangItem> OnSelectShiZhuang;

	public GameObject selecetObj;

	public GameObject equipObj;

	public Transform _GoodsRoot;

	private GoodsData m_goodsData;

	private bool m_beSelect;

	private bool m_beEquip;

	private MUTransfigurationFashion m_fashion;
}
