using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using UnityEngine;

public class SaveGetMoneyPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.staticTxt[0].text = Global.GetLang("数量:");
		this.staticTxt[1].text = Global.GetLang("金币");
		this.staticTxt[2].text = Global.GetLang("绑金");
		this.saveBtn.Text = Global.GetLang("存入");
		this.getBtn.Text = Global.GetLang("取出");
		this.bindMoney.transform.localPosition = new Vector3(20f, -54f, 0f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.normalMoney.Check = true;
		this.bindMoney.Check = false;
		this.type = 0;
		this.normalMoney.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.type = 0;
			this.bindMoney.Check = false;
		};
		this.bindMoney.CheckChanged = delegate(object s, BaseEventArgs e)
		{
			this.type = 1;
			this.normalMoney.Check = false;
		};
		UIEventListener.Get(this.inputNum.gameObject).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(null, this.money, 0, -100);
		};
		this.saveBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = Convert.ToInt32(this.money.text);
			if (this.type == 0)
			{
				GameInstance.Game.StoreSaveMoney(num);
			}
			else if (this.type == 1)
			{
				GameInstance.Game.StoreSaveBindMoney(num);
			}
		};
		this.getBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = Convert.ToInt32(this.money.text);
			if (this.type == 0)
			{
				GameInstance.Game.StoreSaveMoney(num * -1);
			}
			else if (this.type == 1)
			{
				GameInstance.Game.StoreSaveBindMoney(num * -1);
			}
		};
	}

	public GButton saveBtn;

	public GButton getBtn;

	public GButton close;

	public Transform inputNum;

	public UILabel money;

	public GCheckBox normalMoney;

	public GCheckBox bindMoney;

	private int type;

	public TextBlock[] staticTxt;
}
