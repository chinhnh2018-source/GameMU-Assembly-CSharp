using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class FaQiJieMengTip : UserControl
{
	private void InitTextInPrefabs()
	{
		string text = string.Format("{0}{1}{2}", ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("结盟战盟在主线地图、跨服地图中选择"), "ffffff"), ZhanMengWaiJiaoPart.GetFontColorContentForChinese("【战盟模式】", "fac60d"), ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("时，不会相互攻击"), "ffffff"));
		this.txtContent.text = text;
		this.btnFaQiJieMeng.Text = Global.GetLang("发起结盟");
		this.txtContent.MaxWidth = 565.0;
		this.txtContent.Pivot = 3;
		this.txtContent.X = -480.0;
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.faQiJieMeng = this.faQiJieMengWindow.GetComponent<FaQiJieMengWindow>();
		this.btnFaQiJieMeng.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.curMengYouCount > this.sumMengYouCount)
			{
				string msg = string.Format("{0}{1}{2}", Global.GetLang("最多只能跟"), this.sumMengYouCount, Global.GetLang("个战盟结盟"));
				Super.HintMainText(msg, 10, 3);
				return;
			}
			this.PopFaQiJieMengWindow();
		};
	}

	private void PopFaQiJieMengWindow()
	{
		this.faQiJieMengWindow.SetActive(true);
		this.faQiJieMeng.InitInputBox();
	}

	public void SetMengYouCount(int tmpCurCount, int sumCount)
	{
		this.curMengYouCount = tmpCurCount;
		this.sumMengYouCount = sumCount;
		this.ShowMengYouCount(this.curMengYouCount, this.sumMengYouCount);
	}

	private void ShowMengYouCount(int count, int sumCount)
	{
		if (count > sumCount)
		{
			count = sumCount;
		}
		string text = string.Format("{0}{1}{2}{3}", new object[]
		{
			ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("盟友："), "fac60d"),
			count,
			"/",
			sumCount
		});
		this.txtMengYouCount.Text = text;
	}

	public TextBlock txtContent;

	public TextBlock txtMengYouCount;

	public GButton btnFaQiJieMeng;

	public GameObject faQiJieMengWindow;

	public FaQiJieMengWindow faQiJieMeng;

	private int curMengYouCount;

	private int sumMengYouCount;
}
