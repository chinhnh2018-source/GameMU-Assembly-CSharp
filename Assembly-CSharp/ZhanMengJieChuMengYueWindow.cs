using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ZhanMengJieChuMengYueWindow : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnOK.Text = Global.GetLang("解除盟约");
		this.btnCancel.Text = Global.GetLang("取消");
		this.txtTitle.text = Global.GetLang("解除盟约");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DestroySelf();
		};
		this.btnOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, null);
			}
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DestroySelf();
		};
	}

	public void SetContent(string zhanMengName)
	{
		string text = string.Format("{0}{1}{2}", ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("确定要与"), "dac7ae"), ZhanMengWaiJiaoPart.GetFontColorContentForChinese(string.Format("{0}{1}{2}", Global.GetLang("【"), zhanMengName, Global.GetLang("】")), "17e43e"), ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("解除结盟关系吗？"), "dac7ae"));
		this.txtContent.text = text;
	}

	public void DestroySelf()
	{
		base.gameObject.SetActive(false);
	}

	public UILabel txtTitle;

	public UILabel txtContent;

	public GButton btnClose;

	public GButton btnOK;

	public GButton btnCancel;

	public DPSelectedItemEventHandler DPSelectedItem;
}
