using System;

public class ShiLiPartShiLiInfoItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void Init(ShiLiType type, int currentValue, int maxValue)
	{
		float fillAmount = 0f;
		if (maxValue > 0)
		{
			fillAmount = (float)currentValue * 1f / (float)maxValue;
		}
		this.imgValue.fillAmount = fillAmount;
		if (currentValue > 100000)
		{
			this.lblNum.text = ((float)currentValue / 10000f).ToString("f1") + "w";
		}
		else
		{
			this.lblNum.text = currentValue.ToString(string.Empty);
		}
		if (type == (ShiLiType)ShiLiData.GetSelfCompData().kfCompData.CompType)
		{
			this.imgName.spriteName = this.selfImgName;
		}
	}

	public UISprite imgName;

	public UILabel lblNum;

	public UISprite imgValue;

	public string selfImgName = string.Empty;

	private ShiLiType m_type;
}
