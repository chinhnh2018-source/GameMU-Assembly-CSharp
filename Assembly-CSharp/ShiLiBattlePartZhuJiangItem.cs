using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Server.Tools;
using XMLCreater;

public class ShiLiBattlePartZhuJiangItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void SetZhuJiangInfo(CompBattleZhuJiangInfo info)
	{
		string url = StringUtil.substitute("NetImages/Face/{0}{1}_0.png", new object[]
		{
			info.Occupation,
			info.RoleSex
		});
		this.iconHead.URL = url;
		MUCompLevel compLevelByCompIDAndLevel = ShiLiData.GetCompLevelByCompIDAndLevel(ShiLiData.GetSelfCompType(), info.CompZhiWu);
		if (compLevelByCompIDAndLevel == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"职务信息错误"
			});
			return;
		}
		if (compLevelByCompIDAndLevel.Level == 0)
		{
			return;
		}
		this.lblName.text = compLevelByCompIDAndLevel.Name + "   " + info.Name;
		this.lblBuff.text = Global.GetGoodsEquipPropsStringForBufferTips(compLevelByCompIDAndLevel.CraftBuffID);
	}

	public UILabel lblName;

	public UILabel lblBuff;

	public ShowNetImage iconHead;
}
