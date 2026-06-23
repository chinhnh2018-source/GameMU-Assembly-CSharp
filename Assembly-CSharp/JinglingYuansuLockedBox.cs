using System;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class JinglingYuansuLockedBox : UserControl
{
	public void Init(int index)
	{
		this.index = index;
		this.bak = U3DUtils.FindGameObjectByName(base.gameObject, "Bak").GetComponent<UISprite>();
		this.equipSpriteSL = U3DUtils.FindGameObjectByName(base.gameObject, "SpriteSL");
		this.lockStateTrs = U3DUtils.FindGameObjectByName(base.gameObject, "SpriteSL_Lock");
		this.unlockStateTrs = U3DUtils.FindGameObjectByName(base.gameObject, "SpriteSL_Unlock");
		if (this.equipSpriteSL)
		{
			this.equipSpriteSL.SetActive(true);
		}
		if (this.lockStateTrs)
		{
			this.lockStateTrs.SetActive(true);
		}
		if (this.unlockStateTrs)
		{
			this.unlockStateTrs.SetActive(false);
		}
		if (this.unlockLbl)
		{
			this.unlockLbl.text = Global.GetLang("可购买");
		}
		string systemParamByName = ConfigSystemParam.GetSystemParamByName("ElementsHeartSlots", true);
		if (!string.IsNullOrEmpty(systemParamByName))
		{
			string[] array = systemParamByName.Split(new char[]
			{
				',',
				'|'
			});
			if (array.Length >= 4)
			{
				this.defaultMoney = ConvertExt.SafeConvertToInt32(array[1]);
				if (index == 9)
				{
					this.defaultMoney = ConvertExt.SafeConvertToInt32(array[3]);
				}
			}
		}
		if (this.unlockMoneyLbl)
		{
			this.unlockMoneyLbl.text = this.defaultMoney.ToString();
		}
		if (this.bak)
		{
			UIEventListener.Get(this.bak.gameObject).onClick = new UIEventListener.VoidDelegate(this.onClick);
		}
	}

	private void onClick(GameObject go)
	{
		if (this.isOpend)
		{
			return;
		}
		if (this.lockStateTrs && this.lockStateTrs.activeSelf)
		{
			Super.HintMainText(Global.GetLang("需先购买9号栏位，才能购买10号栏位！"), 10, 3);
			return;
		}
		GChildWindow messageBoxWindow = U3DUtils.NEW<GChildWindow>();
		Super.InitChildWindow(messageBoxWindow, "MessageBoxExWindow");
		PlayZone.GlobalPlayZone.Children.Add(messageBoxWindow);
		JinglingYuansuLockedBoxMessageBox messageBoxtPart = U3DUtils.NEW<JinglingYuansuLockedBoxMessageBox>();
		messageBoxWindow.SetContent(messageBoxWindow.BodyPresenter, messageBoxtPart, 9.0, 0.0, true);
		messageBoxWindow.ModalType = ChildWindowModalType.Translucent;
		messageBoxtPart.InitPartData(Global.GetLang("提示"), Global.GetLang("激活此元素之心栏位需要耗费"), this.defaultMoney, 0f, new string[]
		{
			Global.GetLang("确定"),
			Global.GetLang("取消")
		});
		messageBoxtPart.ButtonClick = delegate(object s, EventArgs e)
		{
			int myMessageBoxPartReturn = messageBoxtPart.MyMessageBoxPartReturn;
			messageBoxWindow.NotifyClose(myMessageBoxPartReturn);
			Super.CloseChildWindow(PlayZone.GlobalPlayZone.Children, messageBoxWindow);
			if (myMessageBoxPartReturn == 0)
			{
				if (this.defaultMoney <= Global.Data.roleData.UserMoney)
				{
					GameInstance.Game.SpriteQueryOpenYuanSuBox(this.index + 1);
				}
				else
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
				}
			}
		};
	}

	public bool isOpend
	{
		get
		{
			return this._isOpend;
		}
		set
		{
			this._isOpend = value;
			if (value)
			{
				this.SetState(0);
			}
			else
			{
				this.SetState(1);
			}
		}
	}

	public void UpdateBox89(JinglingYuansuLockedBox b8, JinglingYuansuLockedBox b9)
	{
		if (b8.isOpend)
		{
			b8.SetState(0);
			if (b9.isOpend)
			{
				b9.SetState(0);
			}
			else
			{
				b9.SetState(1);
			}
		}
		else
		{
			b8.SetState(1);
			b9.SetState(2);
		}
	}

	public void SetState(int istate)
	{
		if (istate == 0)
		{
			if (this.lockStateTrs)
			{
				this.lockStateTrs.SetActive(false);
			}
			if (this.unlockStateTrs)
			{
				this.unlockStateTrs.SetActive(false);
			}
			if (this.bak && this.bak.gameObject.GetComponent<BoxCollider>())
			{
				this.bak.gameObject.GetComponent<BoxCollider>().enabled = false;
			}
		}
		else if (istate == 1)
		{
			if (this.lockStateTrs)
			{
				this.lockStateTrs.SetActive(false);
			}
			if (this.unlockStateTrs)
			{
				this.unlockStateTrs.SetActive(true);
			}
			if (this.bak && this.bak.gameObject.GetComponent<BoxCollider>())
			{
				this.bak.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
		}
		else if (istate == 2)
		{
			if (this.lockStateTrs)
			{
				this.lockStateTrs.SetActive(true);
			}
			if (this.unlockStateTrs)
			{
				this.unlockStateTrs.SetActive(false);
			}
			if (this.bak && this.bak.gameObject.GetComponent<BoxCollider>())
			{
				this.bak.gameObject.GetComponent<BoxCollider>().enabled = true;
			}
		}
	}

	public int index;

	public UISprite bak;

	public GameObject equipSpriteSL;

	public GameObject lockStateTrs;

	public GameObject unlockStateTrs;

	public UILabel unlockLbl;

	public UILabel unlockMoneyLbl;

	private int defaultMoney = 100;

	private bool _isOpend;
}
