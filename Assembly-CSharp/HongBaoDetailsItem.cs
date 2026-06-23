using System;

public class HongBaoDetailsItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void NotifyInitDataByServerData(SingleHongBaoRankInfo data)
	{
		if (data == null)
		{
			return;
		}
		NGUITools.SetActive(this.mZuiJia.gameObject, data.zuiJia != 0);
		this.mName.Text = data.roleName;
		this.mNum.Text = data.diamondNum.ToString();
	}

	protected override void OnDestroy()
	{
		this.mZuiJia = null;
		this.mName = null;
		this.mNum = null;
	}

	public UISprite mZuiJia;

	public TextBlock mName;

	public TextBlock mNum;
}
