using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class ShiLiPartConfigWindow : UserControl
{
	private void InitTextInPrefabs()
	{
		try
		{
			this.btnOk.Text = Global.GetLang("确定");
			this.lblTitle.text = Global.GetLang("势力转换");
			this.lblWord.text = Global.GetLang("消耗钻石 :");
			this.lblWord.pivot = 3;
			this.lblNum.pivot = 3;
			this.lblWord.transform.localPosition = new Vector3(120f, 0f, -1f);
			this.lblNum.transform.localPosition = new Vector3(205f, 0f, -1f);
			this.SprBackgroundDiamond.transform.localPosition = new Vector3(190f, 2f, -1f);
		}
		catch
		{
			MUDebug.Log<string>(new string[]
			{
				"越南东南亚英文报空：" + base.GetType().Name
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnOk.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OnConfig != null)
			{
				this.OnConfig.Invoke();
			}
			base.gameObject.SetActive(false);
		};
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.OnClose != null)
			{
				this.OnClose.Invoke();
			}
			base.gameObject.SetActive(false);
		};
	}

	public void InitContent(string message, int costNum)
	{
		this.lblContent.text = message;
		this.lblNum.text = costNum.ToString();
	}

	public UILabel lblTitle;

	public UILabel lblContent;

	public UILabel lblNum;

	public UILabel lblWord;

	public GButton btnOk;

	public GButton btnClose;

	public Action OnConfig;

	public Action OnClose;

	public UISprite SprBackgroundDiamond;
}
