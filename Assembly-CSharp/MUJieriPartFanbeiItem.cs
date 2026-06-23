using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MUJieriPartFanbeiItem : UserControl
{
	public int TypeID
	{
		get
		{
			return this.typeID;
		}
		set
		{
			this.typeID = value;
		}
	}

	public string LabName
	{
		get
		{
			return this._labName;
		}
		set
		{
			this._labName = value;
			this.labName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				this.LabName
			});
		}
	}

	public string LabTime
	{
		get
		{
			return this._labTime;
		}
		set
		{
			this._labTime = value;
			this.labTime.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ffffff",
				this.LabTime
			});
		}
	}

	public int Link
	{
		get
		{
			return this._link;
		}
		set
		{
			this._link = value;
		}
	}

	public int Effective
	{
		get
		{
			return this.effective;
		}
		set
		{
			this.effective = value;
			if (this.Effective == 0)
			{
				base.gameObject.SetActive(false);
			}
			if (this.Effective == 1)
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnGO.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.CloseJieRihuodongWindow();
			PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = this.Link,
				Flag = this.TypeID
			});
		};
	}

	private void InitTextInPrefabs()
	{
		this.btnGO.Text = Global.GetLang("立即前往");
		this.labName.Pivot = 3;
		this.labName.X = -320.0;
		this.labTime.X = 160.0;
		this.sprX2.transform.localPosition = new Vector3(45f, 3f, 0f);
	}

	public TextBlock labName;

	public TextBlock labTime;

	public GButton btnGO;

	public DPSelectedItemEventHandler DPSelectedItem;

	public UISprite sprX2;

	private int typeID;

	private string _labName = string.Empty;

	private string _labTime = string.Empty;

	private int _link;

	private int effective;
}
