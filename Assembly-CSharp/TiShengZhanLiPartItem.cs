using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class TiShengZhanLiPartItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnGoto.Text = Global.GetLang("立即前往");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		if (null != this.m_BtnGoto)
		{
			this.m_BtnGoto.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.DPSelectedItem != null)
				{
					this.DPSelectedItem(this, new DPSelectedItemEventArgs
					{
						ID = this.LinkID
					});
				}
			};
		}
	}

	public void RefreshUI(float percent, int standardValue, string name, int linkId, string imageName, int itemType)
	{
		this.LinkID = linkId;
		this.m_TextName.text = name;
		this.Bak.URL = "NetImages/GameRes/Images/BianQiang/" + imageName + ".png";
		this.progressBar.Percent = (double)percent;
		this.m_JinDuText.text = string.Format(Global.GetLang("提升度:{0}%"), (int)(percent * 100f));
	}

	public GButton m_BtnGoto;

	public GTextBlockOutLine m_TextName;

	public ShowNetImage Bak;

	public GImgProgressBar progressBar;

	public UILabel m_JinDuText;

	public int LinkID = -1;

	public DPSelectedItemEventHandler DPSelectedItem;
}
