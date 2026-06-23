using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class zhanmenghuodong : UserControl
{
	private void InitTextInPrefabs()
	{
		this.louLanChengZhan.Text = Global.GetLang("罗兰城战");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				Type = -10
			});
		};
		if (null == this.luolanPart)
		{
			this.luolanPart = U3DUtils.NEW<luolan_part>();
			this.luolanPart.transform.parent = this.childPanel;
			this.luolanPart.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.luolanPart.transform.localScale = new Vector3(1f, 1f, 1f);
			this.luolanPart.chengZhanShenQing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = 1
				});
			};
			this.luolanPart.yanHui.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					Type = 2
				});
			};
			this.luolanPart.chaKanGuiZe.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (null == this.luoLanRolePart)
				{
					this.luoLanRolePart = U3DUtils.NEW<LuoLanRolePart>();
					this.luoLanRolePart.transform.parent = this.childPanel;
					this.luoLanRolePart.transform.localPosition = new Vector3(0f, 0f, 0f);
					this.luoLanRolePart.transform.localScale = new Vector3(1f, 1f, 1f);
					if (null != this.luolanPart)
					{
						this.luolanPart.gameObject.SetActive(false);
					}
					this.luoLanRolePart.DPSelectedItem = delegate(object ss, DPSelectedItemEventArgs ee)
					{
						Object.Destroy(this.luoLanRolePart.gameObject);
						this.luoLanRolePart = null;
						this.luolanPart.gameObject.SetActive(true);
					};
				}
			};
		}
	}

	public GButton louLanChengZhan;

	public GButton close;

	public Transform childPanel;

	public DPSelectedItemEventHandler DPSelectedItem;

	public shenqingchengzhan shenQingPart;

	private luolan_part luolanPart;

	private LuoLanRolePart luoLanRolePart;
}
