using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class TerritoryFightPartZhanLingFuLi : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BG.URL = "NetImages/GameRes/Images/Plate/zhongshen/tongyongdikuang1.png.qj";
		this.ZhaoHuanShouWeiImageBG.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/dikuang.png.qj";
		this.KaiQiShuangBeiImageBG.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/dikuang.png.qj";
		this.CaiJiJiaSuImageBG.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/dikuang.png.qj";
		this.TuanZhangDiaoXiangImageBG.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/dikuang.png.qj";
		this.ZhaoHuanShouWeiImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/shouwei.png.qj";
		this.KaiQiShuangBeiImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/shuangbei.png.qj";
		this.CaiJiJiaSuImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/caiji.png.qj";
		this.TuanZhangDiaoXiangImage.URL = "NetImages/GameRes/Images/Plate/ArmayActivityBG/tuanzhang.png.qj";
		this.ZhaoHuanShouWeiName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("召唤守卫")
		});
		this.ZhaoHuanShouWeiMiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("召唤领地守卫,守护军团专属采集区域")
		});
		this.KaiQiShuangBeiName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("开启双倍")
		});
		this.KaiQiShuangBeiMiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("自主开启领地采集双倍时间,我的领地我做主！")
		});
		this.CaiJiJiaSuName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("采集加速")
		});
		this.CaiJiJiaSuMiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("领地内采集加速,先声夺人快人一步！")
		});
		this.TuanZhangDiaoXiangName.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("团长雕像")
		});
		this.TuanZhangDiaoXiangMiaoShu.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			Global.GetLang("膜拜军团长英姿，我的团长我的团！")
		});
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedClose(this, new DPSelectedItemEventArgs());
		};
	}

	public GButton BtnClose;

	public UISprite title;

	public DPSelectedItemEventHandler DPSelectedClose;

	public ShowNetImage BG;

	public ShowNetImage ZhaoHuanShouWeiImageBG;

	public ShowNetImage KaiQiShuangBeiImageBG;

	public ShowNetImage CaiJiJiaSuImageBG;

	public ShowNetImage TuanZhangDiaoXiangImageBG;

	public ShowNetImage ZhaoHuanShouWeiImage;

	public UILabel ZhaoHuanShouWeiName;

	public UILabel ZhaoHuanShouWeiMiaoShu;

	public ShowNetImage KaiQiShuangBeiImage;

	public UILabel KaiQiShuangBeiName;

	public UILabel KaiQiShuangBeiMiaoShu;

	public ShowNetImage CaiJiJiaSuImage;

	public UILabel CaiJiJiaSuName;

	public UILabel CaiJiJiaSuMiaoShu;

	public ShowNetImage TuanZhangDiaoXiangImage;

	public UILabel TuanZhangDiaoXiangName;

	public UILabel TuanZhangDiaoXiangMiaoShu;
}
