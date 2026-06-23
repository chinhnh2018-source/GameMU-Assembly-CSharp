using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class BoCaiStoreItem : UserControl
{
	public GoodsData Gd { get; set; }

	public DuiHuanShangChengVO XmlVo { get; set; }

	public int Number { get; set; }

	public void SetData(GoodsData gd, BoCaiTypeEnum type, DuiHuanShangChengVO xmlVo)
	{
		this.Gd = gd;
		this.XmlVo = xmlVo;
		this.labTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang(this.XmlVo.Name)
		});
		this.labNumber.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			this.XmlVo.DaiBiJiaGe
		});
		if (this.m_EquipIcon != null)
		{
			Object.Destroy(this.m_EquipIcon.gameObject);
			this.m_EquipIcon = null;
		}
		this.m_EquipIcon = Global.CreateGoodsIcon(gd, false, true);
		this.m_EquipIcon.transform.SetParent(this.gameGdParent.transform, false);
		if (this.m_EquipIcon.GetComponent<UIPanel>() != null)
		{
			Object.Destroy(this.m_EquipIcon.GetComponent<UIPanel>());
		}
		if (this.m_EquipIcon.GetComponent<BoxCollider>() != null)
		{
			Object.Destroy(this.m_EquipIcon.GetComponent<BoxCollider>());
		}
	}

	public UILabel labTitle;

	public GameObject gameGdParent;

	public UISprite spHuoBi;

	public UILabel labNumber;

	private int number;

	private GGoodIcon m_EquipIcon;
}
