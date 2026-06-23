using System;

public class LiDaiChengZhuItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public string RoleName
	{
		get
		{
			return this.itemRoleName.text;
		}
		set
		{
			this.itemRoleName.text = value;
		}
	}

	public string BangHuiName
	{
		get
		{
			return this.itemBangHuiName.text;
		}
		set
		{
			this.itemBangHuiName.text = value;
		}
	}

	public string BeiMoBaiCiShu
	{
		get
		{
			return this.itemBeiMoBaiCiShu.text;
		}
		set
		{
			this.itemBeiMoBaiCiShu.text = value;
		}
	}

	public string GetChengZhuTime
	{
		get
		{
			return this.itemGetChengZhuTime.text;
		}
		set
		{
			this.itemGetChengZhuTime.text = value;
		}
	}

	public UILabel itemRoleName;

	public UILabel itemBangHuiName;

	public UILabel itemBeiMoBaiCiShu;

	public UILabel itemGetChengZhuTime;
}
