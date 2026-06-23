using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhongShenZhengBaPart_Rule : UserControl
{
	private void InitTextInPrefabs()
	{
		this.TitleGuize.transform.localScale = new Vector3(18f, 18f, 1f);
		this.TitleJaingli.transform.localScale = new Vector3(18f, 18f, 1f);
		this.TitleJingcai.transform.localScale = new Vector3(18f, 18f, 1f);
		this.Guize.transform.localScale = new Vector3(16f, 16f, 1f);
		this.Jiangli.transform.localScale = new Vector3(16f, 16f, 1f);
		this.Jiangcai.transform.localScale = new Vector3(16f, 16f, 1f);
		this.Title.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("规则")
		});
		this.TitleGuize.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("比赛规则")
		});
		this.TitleJaingli.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("比赛奖励")
		});
		this.TitleJingcai.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("比赛竞彩")
		});
		this.Guize.text = string.Concat(new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("1、每月"),
				"17e43e",
				Global.GetLang("10日至17日"),
				"dac7ae",
				Global.GetLang("进行的下午"),
				"17e43e",
				Global.GetLang("15:30至16:30"),
				"dac7ae",
				Global.GetLang("进行比赛。")
			}),
			"\r\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("2、只有上月跨服天梯赛最终结算"),
				"17e43e",
				Global.GetLang("排名前100名"),
				"dac7ae",
				Global.GetLang("有资格参加比赛。")
			}),
			"\r\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("3、第一至三个比赛日为"),
				"17e43e",
				Global.GetLang("争先赛"),
				"dac7ae",
				Global.GetLang("，系统随机匹配玩家进行1V1战斗，根据每日晋级名额，"),
				"17e43e",
				Global.GetLang("先达到3胜"),
				"dac7ae",
				Global.GetLang("的玩家晋级。")
			}),
			"\r\n",
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("4、第四至七个比赛日为"),
				"17e43e",
				Global.GetLang("淘汰赛"),
				"dac7ae",
				Global.GetLang("，根据分组抽签进行1V1战斗，"),
				"17e43e",
				Global.GetLang("获得3胜"),
				"dac7ae",
				Global.GetLang("的玩家晋级。")
			})
		});
		this.Jiangli.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、全部比赛结束后，根据成绩"),
			"17e43e",
			Global.GetLang("获得奖励")
		}) + "\r\n" + Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("2、冠军还可获得尊贵称号“"),
			"17e43e",
			Global.GetLang("众神之王"),
			"dac7ae",
			Global.GetLang("”，直到下次众神争霸赛开始。")
		});
		this.Jiangcai.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("1、每轮"),
			"17e43e",
			Global.GetLang("比赛开始前"),
			"dac7ae",
			Global.GetLang("，所有玩家可对比赛进行竞猜")
		}) + "\r\n" + Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("2、竞猜正确获得"),
			"17e43e",
			Global.GetLang("争霸点数"),
			"dac7ae",
			Global.GetLang("，可在争霸商店中兑换珍贵道具。")
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton Close;

	public UILabel Title;

	public UILabel TitleGuize;

	public UILabel Guize;

	public UILabel TitleJaingli;

	public UILabel Jiangli;

	public UILabel TitleJingcai;

	public UILabel Jiangcai;
}
