using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CangBaoMiJingHelpPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitHandler();
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void InitHandler()
	{
		this.m_Mask.alpha = 0.6f;
		UIEventListener.Get(this.m_Mask.gameObject).onClick = delegate(GameObject e)
		{
			this.Closehandler(e, new DPSelectedItemEventArgs());
		};
		this.m_Closebtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			this.Closehandler(e, new DPSelectedItemEventArgs());
		};
	}

	private void InitPrefabText()
	{
		this.m_Label1.pivot = 3;
		this.m_label2.pivot = 0;
		this.m_Label3.pivot = 3;
		this.m_label4.pivot = 0;
		this.m_Label5.pivot = 3;
		this.m_label6.pivot = 3;
		this.m_label2.lineWidth = 465;
		this.m_label4.lineWidth = 465;
		this.m_label2.transform.localScale = new Vector3(16f, 16f, 1f);
		this.m_Label1.transform.localPosition = new Vector3(-230f, 180f, this.m_Label1.transform.localPosition.z);
		this.m_label2.transform.localPosition = new Vector3(-230f, 165f, this.m_label2.transform.localPosition.z);
		this.m_Label3.transform.localPosition = new Vector3(-230f, -45f, this.m_Label3.transform.localPosition.z);
		this.m_label4.transform.localPosition = new Vector3(-230f, -55f, this.m_label4.transform.localPosition.z);
		this.m_Label5.transform.localPosition = new Vector3(-230f, -130f, this.m_Label5.transform.localPosition.z);
		this.m_label6.transform.localPosition = new Vector3(-230f, -170f, this.m_label6.transform.localPosition.z);
	}

	public void RefreshContent(string str1, string str2, string str3, string str4, string str5, string str6)
	{
		this.m_Label1.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			str1
		});
		this.m_label2.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dec69c",
			str2
		});
		this.m_Label3.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			str3
		});
		this.m_label4.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dec69c",
			str4
		});
		this.m_Label5.text = Global.GetColorStringForNGUIText(new object[]
		{
			"fac60d",
			str5
		});
		this.m_label6.text = Global.GetColorStringForNGUIText(new object[]
		{
			"dec69c",
			str6
		});
	}

	public UISprite m_Mask;

	public UILabel m_Label1;

	public UILabel m_label2;

	public UILabel m_Label3;

	public UILabel m_label4;

	public UILabel m_Label5;

	public UILabel m_label6;

	public GButton m_Closebtn;

	public DPSelectedItemEventHandler Closehandler;
}
