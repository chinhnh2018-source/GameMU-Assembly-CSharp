using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CompeteCityPartRule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.haixuan.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("海选阶段：")
		});
		this.taotai.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("淘汰阶段：")
		});
		this.haixuanLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0} \r\n{1}", string.Format(Global.GetLang("1、活动时间：每周三 {0}-{1}"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime), Global.GetLang("2、需要盟主报名，报名后开始计时，用时最短\r\n的16个战盟晋级下一天的淘汰赛"))
		});
		this.taotaiLab.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			string.Format("{0} \r\n{1}\r\n{2} \r\n{3}\r\n{4}", new object[]
			{
				string.Format(Global.GetLang("1、活动时间：每周四、五 {0}-{1}(每天两\r\n场）"), CompeteCityPart.ConstStartTime, CompeteCityPart.ConstEndTime),
				Global.GetLang("2、淘汰阶段不需报名，活动开始即可进入"),
				Global.GetLang("3，根据预选赛排名两两一组，副本中双方可以互\r\n相PK，最终对BOSS伤害多的一方胜利"),
				Global.GetLang("4、对BOSS最后一击的玩家可获得额外奖励"),
				Global.GetLang("5、对BOSS未造成伤害没有奖励")
			})
		});
		this.haixuan.pivot = 3;
		this.haixuanLab.pivot = 0;
		this.haixuanLab.lineWidth = 425;
		this.taotaiLab.lineWidth = 425;
		this.taotai.pivot = 3;
		this.taotaiLab.pivot = 0;
		this.haixuanLab.transform.localScale = new Vector3(16f, 16f, 1f);
		this.taotaiLab.transform.localScale = new Vector3(16f, 16f, 1f);
		this.haixuan.transform.localPosition = new Vector3(-213f, 145f, -1f);
		this.haixuanLab.transform.localPosition = new Vector3(-213f, 132f, -1f);
		this.taotai.transform.localPosition = new Vector3(-213f, 12f, -1f);
		this.taotaiLab.transform.localPosition = new Vector3(-213f, -3f, -1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandle(this, new DPSelectedItemEventArgs());
		};
	}

	public DPSelectedItemEventHandler CloseHandle;

	public GButton Close;

	public UILabel haixuan;

	public UILabel haixuanLab;

	public UILabel taotai;

	public UILabel taotaiLab;
}
