using System;
using HSGameEngine.GameEngine.Logic;

public class JiYuanPaiHangJiangLiItem : UserControl
{
	public int SetMingCi
	{
		set
		{
			if (value > 0 && value <= 5)
			{
				this.m_MingCi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("第") + value.ToString() + Global.GetLang("名奖励")
				});
			}
			else
			{
				this.m_MingCi.text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("6名后奖励")
				});
			}
		}
	}

	public int SetJiYuan
	{
		set
		{
			this.m_JiYuan.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("至少完成第") + value.ToString() + Global.GetLang("纪元")
			});
		}
	}

	public UILabel m_MingCi;

	public UILabel m_JiYuan;

	public ListBox m_List;

	public ObservableCollection m_ObservableCollection_ListBox_JiangLi;
}
