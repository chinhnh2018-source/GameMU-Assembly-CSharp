using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChangeOccupationPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.LeftLab.pivot = 4;
		this.RightLab.pivot = 4;
		this.XiaoHaoLab.pivot = 4;
		this.LeftLab.transform.localPosition = new Vector3(-138f, -54f, -1f);
		this.RightLab.transform.localPosition = new Vector3(138f, -54f, -1f);
		this.XiaoHaoLab.transform.localPosition = new Vector3(0f, -101f, -1f);
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("开启转职")
		});
		this.LeftLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("安全区可切换职业")
		});
		this.RightLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("除装备、技能外,其余系统共享")
		});
		this.XiaoHaoLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("副职业创建后不可删除")
		});
		this.BtnSure.Label.text = Global.GetLang("立即开启");
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("PurchaseOccupation", ',');
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("PurchaseOccupationNum", true);
		int num = 0;
		if (Global.Data.roleData.OccupationList != null && int.Parse(systemParamByName) == systemParamIntArrayByName.Length && Global.Data.roleData.OccupationList.Count - 1 <= int.Parse(systemParamByName))
		{
			num = systemParamIntArrayByName[Global.Data.roleData.OccupationList.Count - 1];
		}
		this.ZuanShiNumLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fdf7dd",
			num
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Handler(this, new DPSelectedItemEventArgs());
		};
		this.BtnSure.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.Handler(this, new DPSelectedItemEventArgs());
			Super.ShowCreateFuRolePanelControl(Super.MainWindowRoot, 0);
		};
	}

	public DPSelectedItemEventHandler Handler;

	public GButton BtnClose;

	public GButton BtnSure;

	public UILabel Title;

	public UILabel LeftLab;

	public UILabel RightLab;

	public UILabel XiaoHaoLab;

	public UILabel ZuanShiNumLab;

	public UILabel FuZhiYeLab;
}
