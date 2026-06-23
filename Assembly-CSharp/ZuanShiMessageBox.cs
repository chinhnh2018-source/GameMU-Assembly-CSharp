using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class ZuanShiMessageBox : UserControl
{
	public int BoxType
	{
		set
		{
			this.mBoxType = value;
			if (value == 0)
			{
				this.CheckBox.gameObject.SetActive(false);
				this.CancelBtn.gameObject.SetActive(false);
				this.OkBtn.gameObject.SetActive(true);
				this.OkBtn.gameObject.transform.localPosition = this.CancelBtn.transform.localPosition;
			}
			else if (value == 1 || value == 11)
			{
				this.CheckBox.gameObject.SetActive(false);
				this.OkBtn.gameObject.SetActive(true);
				this.CancelBtn.gameObject.SetActive(true);
				Transform transform = base.transform.FindChild("bak");
				if (null != transform)
				{
					transform.localScale = new Vector3(transform.localScale.x, 200f, transform.localScale.z);
				}
				Transform transform2 = base.transform.FindChild("Btns");
				if (null != transform2)
				{
					transform2.localPosition = new Vector3(transform2.localPosition.x, 10f, transform2.localPosition.z);
				}
			}
			else if (value == 2)
			{
				this.CheckBox.gameObject.SetActive(true);
				this.OkBtn.gameObject.SetActive(true);
				this.CancelBtn.gameObject.SetActive(true);
			}
		}
	}

	public string HintTitle
	{
		set
		{
			if (null != this.HintTitle_Label)
			{
				this.HintTitle_Label.text = value;
			}
		}
	}

	public string HintText
	{
		set
		{
			if (null != this.HintText_Label)
			{
				if (this.mBoxType == 11)
				{
					this.HintText_Label.Label.pivot = 3;
				}
				this.HintText_Label.text = value;
			}
		}
	}

	public int MyMessageBoxPartReturn
	{
		get
		{
			return this._MyMessageBoxPartReturn;
		}
	}

	public bool ZhuanShiPosMove
	{
		set
		{
			if (value)
			{
				int num = this.HintText_Label.FontSize * this.HintText_Label.text.Length;
				Vector3 localPosition = this.HintText_Label.transform.localPosition;
				localPosition.x += (float)num * 0.2f + 10f;
				localPosition.z = -3f;
				localPosition.y += 5f;
				this.m_ZhuanShiSp.transform.localPosition = localPosition;
			}
		}
	}

	public UIWidget.Pivot Pivot
	{
		set
		{
			try
			{
				this.HintText_Label.Pivot = value;
				this.HintText_Label.MaxWidth = 0.0;
			}
			catch (Exception ex)
			{
			}
		}
	}

	public float ZhuanShiIconTransX
	{
		set
		{
			if (value != 0f)
			{
				try
				{
					if (value == -130f)
					{
						this.m_ZhuanShiSp.transform.localPosition = new Vector3(-40f, 37f, -3f);
					}
					else
					{
						this.m_ZhuanShiSp.transform.localPosition += new Vector3(value, 0f, 0f);
					}
				}
				catch (Exception ex)
				{
					MUDebug.Log<string>(new string[]
					{
						ex.Message
					});
				}
			}
		}
	}

	public int zuanShi { get; set; }

	public string DaiBi
	{
		set
		{
			IConfigbase<ConfigDaiBiShiYong>.Instance.RefreshSp(this.m_ZhuanShiSp, value, this.zuanShi, string.Empty);
		}
	}

	private void InitTextInPrefabs()
	{
		this.OkBtn.Text = Global.GetLang("确定");
		this.CancelBtn.Text = Global.GetLang("取消");
		this.CheckBox.Text = Global.GetLang("不再提示");
		this.HintText_Label.text = Global.GetLang("{e3b36c}本次操作需要花费钻石        10 ,  确认要执行该操作吗？{-}");
		this.m_ZhuanShiSp.transform.localPosition = new Vector3(-108f, 9f, -3f);
		this.CheckBox._Lable.transform.localPosition = new Vector3(26f, 0f, 0f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.OkBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OkBtn_MouseLeftButtonUp);
		this.CancelBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
		this.CloseBtn.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.CancelBtn_MouseLeftButtonUp);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	private void OkBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = 0;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		Object.Destroy(base.gameObject);
	}

	private void CancelBtn_MouseLeftButtonUp(object sender, MouseEvent e)
	{
		if (this.ButtonClick != null)
		{
			this._MyMessageBoxPartReturn = 1;
			this.ButtonClick.Invoke(this, EventArgs.Empty);
		}
		Object.Destroy(base.gameObject);
	}

	public TextBlock HintText_Label;

	public UILabel HintTitle_Label;

	public GButton OkBtn;

	public GButton CancelBtn;

	public GButton CloseBtn;

	public GCheckBox CheckBox;

	public UISprite m_ZhuanShiSp;

	public EventHandler ButtonClick;

	private int mBoxType;

	private int _MyMessageBoxPartReturn = -1;
}
