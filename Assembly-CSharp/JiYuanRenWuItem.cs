using System;
using HSGameEngine.GameEngine.Logic;

public class JiYuanRenWuItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.m_LabJiangLi.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			Global.GetLang("奖励")
		});
		this.m_ObservableCollection = this.m_List.ItemsSource;
	}

	public string SetTitle
	{
		set
		{
			this.m_LabTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang(value)
			});
		}
	}

	public string SetContent
	{
		set
		{
			this.m_LabContent.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang(value)
			});
		}
	}

	public int SetJinDu
	{
		set
		{
			this.m_LabJinDu.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("第") + this.JiYuanKey.ToString() + Global.GetLang("纪元进度")
			}) + Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				"+" + value + "%"
			});
		}
	}

	public UILabel m_LabTitle;

	public UILabel m_LabContent;

	public ListBox m_List;

	public UILabel m_LabJiangLi;

	public UILabel m_LabJinDu;

	public UISprite m_SpYiWanCheng;

	public ObservableCollection m_ObservableCollection;

	public int Id = -1;

	public int JiYuanKey;
}
