using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ArmyTeQuanConfirmWindow : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitBtnCallBack();
	}

	private void InitTextInPrefabs()
	{
		this.m_Title.Text = Global.GetLang("提 示");
		this.m_Label1.Text = Global.GetLang("确定花费9999");
		this.m_Label2.Text = Global.GetLang("立即部署该弩塔？");
		this.m_BtnConfirm.Text = Global.GetLang("确定");
		this.m_BtnCancel.Text = Global.GetLang("取消");
	}

	private void InitBtnCallBack()
	{
		this.m_BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OptionCallBack != null)
			{
				this.OptionCallBack(null, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this.m_BtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OptionCallBack != null)
			{
				this.OptionCallBack(null, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
		};
		this.m_BtnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OptionCallBack != null)
			{
				this.OptionCallBack(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void InitValue(int diamondCount)
	{
		this.m_Label1.Text = string.Format("{0}{1}", Global.GetLang("确定花费 "), diamondCount);
	}

	protected override void OnDestroy()
	{
		this.OptionCallBack = null;
		this.m_BtnClose = null;
		this.m_BtnConfirm = null;
		this.m_BtnCancel = null;
		this.m_Title = null;
		this.m_Label1 = null;
		this.m_Label2 = null;
	}

	public DPSelectedItemEventHandler OptionCallBack;

	public GButton m_BtnClose;

	public GButton m_BtnConfirm;

	public GButton m_BtnCancel;

	public TextBlock m_Title;

	public TextBlock m_Label1;

	public TextBlock m_Label2;
}
