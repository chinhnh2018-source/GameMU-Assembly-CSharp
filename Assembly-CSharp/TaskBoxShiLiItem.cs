using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class TaskBoxShiLiItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblZhuJiangWord.text = Global.GetLang("势力主将 X ");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void SetInfo(ShiLiType type, float zhanBi, int zhuJiangNum)
	{
		string shiLiNameByType = ShiLiData.GetShiLiNameByType(type);
		this.lblName.text = shiLiNameByType + "  :";
		this.lblZhanJuNum.text = zhuJiangNum.ToString();
		this.lblZhanJu.text = Global.GetLang("占据") + Mathf.Round(zhanBi * 100f) + "%";
	}

	public UILabel lblName;

	public UILabel lblZhanJu;

	public UILabel lblZhuJiangWord;

	public UILabel lblZhanJuNum;
}
