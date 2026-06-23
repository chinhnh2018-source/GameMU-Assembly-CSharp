using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiBattleParAllZhiWu : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("全部职务");
		this.lblZhiWu.text = Global.GetLang("势力职务");
		this.lblName.text = Global.GetLang("角色名称");
		this.lblBuff.text = Global.GetLang("势力BUFF");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void InitInfo(CompZhiWuData zhiWuData)
	{
		int num = 0;
		if (zhiWuData == null || zhiWuData.CompRoleData == null)
		{
			return;
		}
		if (zhiWuData.CompRoleData != null && zhiWuData.CompRoleData.Count > 0)
		{
			for (int i = 0; i < zhiWuData.CompRoleData.Count; i++)
			{
				if (zhiWuData.CompRoleData[i] != null)
				{
					num++;
					ShiLiBattlePartZhiWuItem shiLiBattlePartZhiWuItem = U3DUtils.NEW<ShiLiBattlePartZhiWuItem>();
					shiLiBattlePartZhiWuItem.gameObject.name = "item" + i;
					shiLiBattlePartZhiWuItem.transform.SetParent(this.grid.transform);
					shiLiBattlePartZhiWuItem.transform.localScale = Vector3.one;
					shiLiBattlePartZhiWuItem.transform.localPosition = Vector3.zero;
					zhiWuData.CompRoleData[i].ZhiWu = i + 1;
					shiLiBattlePartZhiWuItem.SetContent(zhiWuData.CompRoleData[i]);
				}
			}
		}
		if (num < 7)
		{
			for (int j = num; j < 7; j++)
			{
				ShiLiBattlePartZhiWuItem shiLiBattlePartZhiWuItem2 = U3DUtils.NEW<ShiLiBattlePartZhiWuItem>();
				shiLiBattlePartZhiWuItem2.gameObject.name = "item" + j;
				shiLiBattlePartZhiWuItem2.transform.SetParent(this.grid.transform);
				shiLiBattlePartZhiWuItem2.transform.localScale = Vector3.one;
				shiLiBattlePartZhiWuItem2.transform.localPosition = Vector3.zero;
				shiLiBattlePartZhiWuItem2.SetNULL();
			}
		}
		this.grid.Reposition();
	}

	private const int OnceNum = 7;

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblZhiWu;

	public UILabel lblName;

	public UILabel lblBuff;

	public GButton btnClose;

	public UIGrid grid;
}
