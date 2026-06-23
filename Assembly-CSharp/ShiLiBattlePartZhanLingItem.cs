using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartZhanLingItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblName.pivot = 3;
		this.lblName.transform.localPosition = new Vector3(-218f, 25f, this.lblName.transform.localPosition.z);
		this.lblZhanBi.pivot = 5;
		this.lblZhanBi.transform.localPosition = new Vector3(25f, 25f, this.lblZhanBi.transform.localPosition.z);
		this.lblOwner.pivot = 5;
		this.lblOwner.transform.localPosition = new Vector3(210f, 25f, this.lblOwner.transform.localPosition.z);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void SetJuDianInfo(MUForceStronghold juDianInfo, ShiLiType type)
	{
		this.lblName.text = juDianInfo.Name;
		this.lblZhanBi.text = (int)(juDianInfo.Rate * 100f) + "%";
		if (type == ShiLiType.None)
		{
			this.lblOwner.text = Global.GetLang("暂未被占领");
		}
		else
		{
			string shiLiNameByType = ShiLiData.GetShiLiNameByType(type);
			this.lblOwner.text = shiLiNameByType;
		}
	}

	public UILabel lblName;

	public UILabel lblZhanBi;

	public UILabel lblOwner;
}
