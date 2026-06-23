using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Contract.KuaFuData;
using UnityEngine;
using XMLCreater;

public class ShiLiBattlePartZhiWuItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblBuff1.transform.localScale = new Vector3(15f, 15f, 1f);
		this.lblBuff2.transform.localScale = new Vector3(15f, 15f, 1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void SetNULL()
	{
		this.lblZhiWu.text = string.Empty;
		this.lblName.text = string.Empty;
		this.lblBuff1.text = string.Empty;
		this.lblBuff2.text = string.Empty;
	}

	public void SetContent(KFCompRoleData roleData)
	{
		if (roleData == null)
		{
			this.SetNULL();
			return;
		}
		if (roleData.RoleData4Selector == null)
		{
			this.SetNULL();
			return;
		}
		int compType = roleData.CompType;
		int zhiWu = roleData.ZhiWu;
		MUCompLevel compLevelByCompIDAndLevel = ShiLiData.GetCompLevelByCompIDAndLevel(compType, zhiWu);
		if (compLevelByCompIDAndLevel == null)
		{
			this.SetNULL();
			MUDebug.LogError<string>(new string[]
			{
				string.Concat(new object[]
				{
					"职务信息错误: 势力id:",
					compType,
					Global.GetLang("  职务id:"),
					zhiWu
				})
			});
			return;
		}
		this.lblName.text = Global.FormatRoleNameZoneid(roleData.RoleData4Selector.ZoneId, roleData.RoleData4Selector.RoleName, 1, 0);
		this.lblZhiWu.text = compLevelByCompIDAndLevel.Name;
		this.lblBuff1.text = Global.GetLang("全员") + ShiLiBattlePartZhiWuItem.GetGoodsEquipPropsStringForBufferTips(compLevelByCompIDAndLevel.CraftBuffID);
		this.lblBuff2.text = Global.GetLang("自身") + ShiLiBattlePartZhiWuItem.GetGoodsEquipPropsStringForBufferTips(compLevelByCompIDAndLevel.CraftSelfBuffID);
	}

	public static string GetGoodsEquipPropsStringForBufferTips(int goodsID)
	{
		double[] goodsEquipPropsDoubleList = Global.GetGoodsEquipPropsDoubleList(goodsID);
		string text = string.Empty;
		for (int i = 0; i < goodsEquipPropsDoubleList.Length; i++)
		{
			if (goodsEquipPropsDoubleList[i] > 0.0)
			{
				string text2 = ExtPropIndexes.ExtPropIndexChineseNames[i];
				string text3 = goodsEquipPropsDoubleList[i].ToString();
				if (ExtPropIndexes.ExtPropIndexPercents[i] > 0)
				{
					text3 = string.Format("+{0}%", (int)(goodsEquipPropsDoubleList[i] * 100.0));
				}
				else if (i >= 2 && i <= 10)
				{
					text3 = string.Format("{0}-{1}", text3, (int)goodsEquipPropsDoubleList[i + 1]);
					i++;
				}
				if (text.Length > 0)
				{
					text += "  ";
				}
				text += string.Format("{0}: {1}", text2, Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					text3
				}));
			}
		}
		return text;
	}

	public UILabel lblZhiWu;

	public UILabel lblName;

	public UILabel lblBuff1;

	public UILabel lblBuff2;
}
