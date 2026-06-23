using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class TeQuanActivityZhiGouPart : UserControl, BaseTeQuanActivityPart
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitShop()
	{
		if (Vector4.zero == this.Panel)
		{
			UIPanel component = this._ShopDragPanel.GetComponent<UIPanel>();
			int num = 54;
			int num2 = -23;
			float num3 = component.clipRange.z / 2f - (component.clipRange.x + (float)num);
			float num4 = component.clipRange.w / 2f - (component.clipRange.y + (float)num2);
			float num5 = component.clipRange.z / 2f + (component.clipRange.x + (float)num);
			float num6 = component.clipRange.w / 2f + (component.clipRange.y + (float)num2);
			Vector3 vector = Global.UICamera.WorldToViewportPoint(base.transform.TransformPoint(-num3, -num4, 0f));
			Vector3 vector2 = Global.UICamera.WorldToViewportPoint(base.transform.TransformPoint(num5, num6, 0f));
			this.Panel = new Vector4(vector.x, vector.y, vector2.x, vector2.y);
		}
		BetterList<TeQuanZhiGouVO> zhiGouVOItems = IConfigbase<ConfigTeQuan>.Instance.GetZhiGouVOItems(this.mID);
		if (zhiGouVOItems != null && 0 < zhiGouVOItems.size)
		{
			for (int i = 0; i < zhiGouVOItems.size; i++)
			{
				TeQuanZhiGouVO teQuanZhiGouVO = zhiGouVOItems[i];
				if (teQuanZhiGouVO != null)
				{
					TeQuanActivityZhiGouItem teQuanActivityZhiGouItem = U3DUtils.NEW<TeQuanActivityZhiGouItem>();
					teQuanActivityZhiGouItem.SetData(teQuanZhiGouVO);
					teQuanActivityZhiGouItem.transform.SetParent(this._ShopViewRoot.transform, false);
					this.mShopItems.Add(teQuanActivityZhiGouItem);
					teQuanActivityZhiGouItem.transform.localPosition = new Vector3((float)(-290 + i * 250), 0f, 0f);
					teQuanActivityZhiGouItem.DragPanel = this._ShopDragPanel;
					teQuanActivityZhiGouItem.Hander = new DPSelectedItemEventHandler(this.ItemCallBack);
					teQuanActivityZhiGouItem.SetTeXiao(teQuanZhiGouVO.ZhiGouPinZhi == 4, this.Panel);
				}
			}
			SpringPanel springPanel = this._ShopDragPanel.gameObject.GetComponent<SpringPanel>();
			if (springPanel != null)
			{
				springPanel.enabled = true;
			}
			else
			{
				springPanel = this._ShopDragPanel.gameObject.AddComponent<SpringPanel>();
				springPanel.target = new Vector3(51f, 0f, 0f);
				springPanel.strength = 13f;
				springPanel.enabled = true;
			}
		}
	}

	private void ClearRoot()
	{
		for (int i = this.mShopItems.size - 1; i >= 0; i--)
		{
			if (null != this.mShopItems[i])
			{
				Object.Destroy(this.mShopItems[i].gameObject);
			}
			this.mShopItems.RemoveAt(i);
		}
	}

	private void ItemCallBack(object sender, DPSelectedItemEventArgs args)
	{
		if (!this.PartOpen)
		{
			Super.HintMainText(Global.GetLang("当前活动暂未激活"), 10, 3);
			return;
		}
		TeQuanZhiGouVO vo = IConfigbase<ConfigTeQuan>.Instance.GetZhiGouVOItemByID(args.ID);
		if (vo != null)
		{
			MUDebug.Log<string>(new string[]
			{
				string.Concat(new object[]
				{
					"ChongZhiID = ",
					vo.ChongZhiID,
					" ZhiGouID ",
					vo.ZhiGouID
				})
			});
			string rechargeItemConf = Global.GetRechargeItemCfgTypeByPlatform();
			XElement gameResXml = Global.GetGameResXml("Config/MU_ChongZhi.xml");
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "Type");
			XElement xelement = xelementList.Find((XElement e) => Global.GetXElementAttributeStr(e, "TypeID") == rechargeItemConf);
			int num = -1;
			string productId = string.Empty;
			if (xelement != null)
			{
				List<XElement> xelementList2 = Global.GetXElementList(xelement, "ChongZhi");
				XElement xelement2 = xelementList2.Find((XElement e) => Global.GetXElementAttributeStr(e, "ID") == vo.ChongZhiID.ToString());
				if (xelement2 != null)
				{
					num = Global.GetXElementAttributeInt(xelement2, "RMB");
					productId = Global.GetXElementAttributeStr(xelement2, "ID");
					productId = Global.GetXElementAttributeStr(xelement2, "productIdAn");
				}
			}
			if (num != -1)
			{
				this.ChongZhi(num, productId, vo.ZhiGouID);
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"数据有误"
				});
			}
		}
	}

	private void ChongZhi(int money, string productId = "", int zhiZhouId = 0)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南测试用特权直购充值：productId=",
				productId,
				"; money=",
				money
			})
		});
		MUDebug.Log<string>(new string[]
		{
			"YN_Android特权直购里的充值"
		});
		PlatSDKMgr.Pay(8, "1", zhiZhouId);
	}

	private void InitPrefabText()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void InitHandler()
	{
		try
		{
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	public void RefreshPart(BetterList<SpecPriorityActInfo> infList)
	{
		for (int i = 0; i < infList.size; i++)
		{
			if (infList[i] != null)
			{
				TeQuanActivityZhiGouItem teQuanActivityZhiGouItem = null;
				for (int j = 0; j < this.mShopItems.size; j++)
				{
					if (null != this.mShopItems[i] && infList[i].ActID == this.mShopItems[i].ID)
					{
						teQuanActivityZhiGouItem = this.mShopItems[i];
						break;
					}
				}
				if (null != teQuanActivityZhiGouItem)
				{
					teQuanActivityZhiGouItem.BuyNumber = infList[i].LeftPurNum;
				}
			}
		}
	}

	public void RefreshPart(SpecPriorityActInfo inf)
	{
	}

	public bool PartOpen { get; set; }

	public int ID
	{
		get
		{
			return this.mID;
		}
		set
		{
			this.mID = value;
			this.ClearRoot();
			this.InitShop();
		}
	}

	[SerializeField]
	private UIDraggablePanel _ShopDragPanel;

	[SerializeField]
	private GameObject _ShopViewRoot;

	private Vector4 Panel = Vector4.zero;

	private BetterList<TeQuanActivityZhiGouItem> mShopItems = new BetterList<TeQuanActivityZhiGouItem>();

	private int mID = -1;
}
