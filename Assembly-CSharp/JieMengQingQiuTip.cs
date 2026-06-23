using System;
using HSGameEngine.GameEngine.Logic;

public class JieMengQingQiuTip : UserControl
{
	private void InitTextInPrefabs()
	{
		string text = string.Format("{0}{1}{2}", Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			Global.GetLang("结盟战盟在主线地图、跨服地图中选择")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("【战盟模式】")
		}), Global.GetColorStringForNGUIText(new object[]
		{
			"ffffff",
			Global.GetLang("时，不会相互攻击")
		}));
		this.txtContent.Text = text;
		this.txtContent.MaxWidth = 575.0;
		this.txtContent.Pivot = 3;
		this.txtContent.X = -340.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public void SetMengYouCount(int count, int sumCount)
	{
		if (count > sumCount)
		{
			count = sumCount;
		}
		string chineseText = string.Format("{0}{1}{2}{3}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("盟友：")
			}),
			count,
			"/",
			sumCount
		});
		this.txtMengYouCount.Text = Global.GetLang(chineseText);
	}

	public TextBlock txtContent;

	public TextBlock txtMengYouCount;
}
