using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TianFuSystemPart : UserControl
{
	protected override void InitializeComponent()
	{
		this._ShengYuDianShuBtn.Label.text = string.Empty;
		base.InitializeComponent();
		this._ExtraBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ExtraListense();
		};
		this._XiDianBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.XiDianListense();
		};
		this._ShengYuDianShuBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShengYuDianShuListense();
		};
		this._Close.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				GameInstance.Game.isOtherFaceId = false;
				Global.Data.otherPlayerTalentData = null;
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
		this._TiamFuUI._TianFuSYDS.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == -1)
			{
				this.setShengYuDianShu();
			}
		};
	}

	private new void Start()
	{
		if (Global.Data.roleData.MyTalentData == null)
		{
			return;
		}
		if (!Global.Data.roleData.MyTalentData.IsOpen)
		{
			if (GameInstance.Game.isOtherFaceId)
			{
				this.ResetTianFuSkillLocalPosition(-80f);
				this.IsShowButton(false);
			}
			else
			{
				this.ResetTianFuSkillLocalPosition(-50f);
				GameInstance.Game.TianFuGetInfo();
				this.IsShowButton(true);
			}
		}
		else if (GameInstance.Game.isOtherFaceId)
		{
			this.ResetTianFuSkillLocalPosition(-80f);
			this.IsShowButton(false);
		}
		else
		{
			this.ResetTianFuSkillLocalPosition(-50f);
			this.Init();
			this.IsShowButton(true);
		}
	}

	private void ResetTianFuSkillLocalPosition(float x)
	{
		if (null != this._TiamFuUI._TianFuSkillPro)
		{
			this._TiamFuUI._TianFuSkillPro.transform.localPosition = new Vector3(x, -28f, 0f);
		}
	}

	private void IsShowButton(bool isShow)
	{
		NGUITools.SetActive(this._XiDianBtn.gameObject, isShow);
		NGUITools.SetActive(this._ExtraBtn.gameObject, isShow);
		NGUITools.SetActive(this._ShengYuDianShuBtn.gameObject, isShow);
	}

	public void Init()
	{
		this.setShengYuDianShu();
		this._TiamFuUI._TianFuSkillPro.initGoodList();
	}

	public void setShengYuDianShu()
	{
		this._ShengYuDianShuBtn.Label.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			string.Format(Global.GetLang("剩余点数:{0}"), TianFuSystemPart.getShengYuDianShu())
		});
	}

	public static int getShengYuDianShu()
	{
		int num = 0;
		if (Global.Data.roleData.MyTalentData.CountList != null)
		{
			num = Global.Data.roleData.MyTalentData.CountList[1] + Global.Data.roleData.MyTalentData.CountList[2] + Global.Data.roleData.MyTalentData.CountList[3];
		}
		return Global.Data.roleData.MyTalentData.TotalCount - num;
	}

	public static int getUsePoint()
	{
		return Global.Data.roleData.MyTalentData.TotalCount - TianFuSystemPart.getShengYuDianShu();
	}

	private void ExtraListense()
	{
		if (this._TiamFuUI != null)
		{
			this._TiamFuUI.OpenEWSX();
		}
	}

	private void XiDianListense()
	{
		if (this._TiamFuUI != null)
		{
			this._TiamFuUI.OpenXiDian();
		}
	}

	private void ShengYuDianShuListense()
	{
		if (this._TiamFuUI != null)
		{
			this._TiamFuUI.OpenSYDS();
		}
	}

	public GButton _XiDianBtn;

	public GButton _ExtraBtn;

	public GButton _ShengYuDianShuBtn;

	public TianFuUIManager _TiamFuUI;

	public GButton _Close;

	public DPSelectedItemEventHandler DPSelectedItem;
}
