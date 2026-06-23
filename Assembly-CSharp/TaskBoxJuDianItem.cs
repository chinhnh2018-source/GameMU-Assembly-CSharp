using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;
using XMLCreater;

public class TaskBoxJuDianItem : UserControl
{
	public MUForceStronghold JuDianInfo
	{
		get
		{
			return this.m_juDianInfo;
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

	public int GetJuDianId()
	{
		return this.m_juDianInfo.ID;
	}

	public void SetJuDianInfo(MUForceStronghold juDianInfo, ShiLiType type)
	{
		this.m_juDianInfo = juDianInfo;
		this.lblName.text = juDianInfo.Name;
		this.lblZhanBi.text = Mathf.RoundToInt(juDianInfo.Rate * 100f) + "%";
		this.SetShiLi(type);
	}

	public void SetShiLi(ShiLiType type)
	{
		string text = Global.GetLang("暂无势力");
		if (type != ShiLiType.None)
		{
			text = ShiLiData.GetShiLiNameByType(type);
		}
		this.lblShiLi.text = Global.GetColorStringForNGUIText(new object[]
		{
			this.ShiLiColor[(int)type],
			text
		});
	}

	public UILabel lblName;

	public UILabel lblZhanBi;

	public UILabel lblShiLi;

	private string[] ShiLiColor = new string[]
	{
		"FFFFFF",
		"B73838",
		"3681FF",
		"FAC60D"
	};

	private MUForceStronghold m_juDianInfo;
}
