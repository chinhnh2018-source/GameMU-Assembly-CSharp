using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using Tmsk.Contract.KuaFuData;
using UnityEngine;
using XMLCreater;

public class ShiLiPartZhiWuPlayer : MonoBehaviour
{
	private void Awake()
	{
		this.lblNoPlayer.text = Global.GetLang("暂时空缺");
		this.lblPlayerName.text = string.Empty;
		this.lblNoPlayer.gameObject.SetActive(false);
	}

	public void SetCompAndLevel(int comp, int level)
	{
		this.m_compType = comp;
		this.m_zhiWuLevel = level;
	}

	public void InitData(KFCompRoleData role)
	{
		if (role == null)
		{
			this.lblNoPlayer.gameObject.SetActive(true);
			this.lblPlayerName.text = string.Empty;
		}
		else
		{
			this.lblNoPlayer.gameObject.SetActive(false);
			if (role.RoleData4Selector != null)
			{
				this.AddModel(role.RoleData4Selector, this.modelPlayer1);
				this.lblPlayerName.text = role.RoleData4Selector.RoleName;
				MUCompLevel compLevelByCompIDAndLevel = ShiLiData.GetCompLevelByCompIDAndLevel(this.m_compType, this.m_zhiWuLevel);
				if (compLevelByCompIDAndLevel == null)
				{
					this.imgTitle.URL = string.Empty;
					MUDebug.LogError<string>(new string[]
					{
						string.Concat(new object[]
						{
							"未找到职务配置 compType = ",
							this.m_compType,
							"  level = ",
							this.m_zhiWuLevel
						})
					});
				}
				else
				{
					this.imgTitle.URL = ShiLiData.GetSpecialTitleURL(compLevelByCompIDAndLevel);
				}
			}
		}
	}

	private void AddModel(RoleData4Selector rd, Modal3DShow modal)
	{
		if (rd != null)
		{
			int fashionGoodsID = Global.GetFashionGoodsID(rd.FashionWingsID);
			if (rd != null && modal != null)
			{
				this.roleResLoader = UIHelper.LoadRoleRes(modal, rd.SettingBitFlags, rd.Occupation, rd.SubOccupation, rd.RoleName, rd.GoodsDataList, null, rd.MyWingData, 1f, fashionGoodsID, null, false);
			}
		}
		else
		{
			MUDebug.LogError<string>(new string[]
			{
				"RoleData4Selector 数据错误"
			});
		}
	}

	private void OnDestroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
	}

	public UILabel lblNoPlayer;

	public UILabel lblPlayerName;

	public Modal3DShow modelPlayer1;

	public ShowNetImage imgTitle;

	private int m_compType;

	private int m_zhiWuLevel;

	private RoleResLoader roleResLoader;
}
