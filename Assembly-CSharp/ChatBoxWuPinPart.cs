using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class ChatBoxWuPinPart : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_GTab.TabBtns[0].Text = Global.GetLang("身上");
		this.m_GTab.TabBtns[1].Text = Global.GetLang("背包");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.UIDragPl.GetComponent<UIDraggablePanel>().onDragIng = new UIDraggablePanel.OnDragIng(this.onDragFinished);
		if (null != this.lstBeiBao)
		{
			this.BeiBaoItems = this.lstBeiBao.ItemsSource;
			this.lstBeiBao.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lstBeiBao_SelectionChanged);
		}
		if (null != this.lstZhuangbei)
		{
			this.lstZhuangbei.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lstZhuangbei_SelectionChanged);
			this.ZhuangBeiItems = this.lstZhuangbei.ItemsSource;
		}
		this.InitBeiBaoList();
		this.InitZhuangBeiList();
		this.m_GTab.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
		{
			if (e.ID == 0)
			{
				this.PageText.Text = "1/1";
			}
			else if (e.ID == 1)
			{
				this.PageText.Text = StringUtil.substitute("{0}/{1}", new object[]
				{
					this.currentSelectedPage + 1,
					this.maxPageCount
				});
			}
			return false;
		};
		UIEventListener.Get(this.Bak.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	private void lstZhuangbei_SelectionChanged(object sender, object e)
	{
		GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.lstZhuangbei.SelectedItem);
		if (null != ggoodIcon && ggoodIcon.Tip != null && this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				Title = ggoodIcon.Tip
			});
		}
	}

	private void lstBeiBao_SelectionChanged(object sender, object e)
	{
		GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(this.lstBeiBao.SelectedItem);
		if (null != ggoodIcon && ggoodIcon.Tip != null && this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				Title = ggoodIcon.Tip
			});
		}
	}

	private void InitBeiBaoList()
	{
		this.BeiBaoItems.Clear();
		this.lstBeiBaoWuPin = new List<GoodsData>();
		this.lstBeiBaoWuPin = this.GetBeiBaoWuPin();
		this.showPage(this.currentSelectedPage);
	}

	private GGoodIcon GetGoodsItemIcon(GoodsData goodsData, bool isDrag, bool autoBackground = true)
	{
		GGoodIcon ggoodIcon;
		if (goodsData != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			string textColoeByGoodsIDHTML = ChatBoxWuPinPart.GetTextColoeByGoodsIDHTML(goodsData);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = ((!autoBackground) ? "none" : "bagGrid_bak");
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = textColoeByGoodsIDHTML;
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			if (isDrag)
			{
				ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			}
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private List<GoodsData> GetBeiBaoWuPin()
	{
		List<GoodsData> list = new List<GoodsData>();
		if (list == null || Global.Data.roleData.GoodsDataList == null)
		{
			return list;
		}
		for (int i = 0; i < Global.Data.roleData.GoodsDataList.Count; i++)
		{
			if (Global.Data.roleData.GoodsDataList[i].Using == 0)
			{
				list.Add(Global.Data.roleData.GoodsDataList[i]);
			}
		}
		return list;
	}

	private List<GoodsData> GetUseingGoods()
	{
		Global.GetUsingGoodsDataList();
		List<GoodsData> list = new List<GoodsData>();
		if (Super.GData.RoleUsingGoodsDataList != null)
		{
			Dictionary<int, GoodsData> roleUsingGoodsDataList = Super.GData.RoleUsingGoodsDataList;
			foreach (GoodsData goodsData in roleUsingGoodsDataList.Values)
			{
				if (goodsData != null)
				{
					if (Global.GetCategoriyByGoodsID(goodsData.GoodsID) != 23)
					{
						list.Add(goodsData);
					}
				}
			}
			return list;
		}
		return null;
	}

	private void InitZhuangBeiList()
	{
		if (null != this.lstZhuangbei)
		{
			this.ZhuangBeiItems.Clear();
			List<GoodsData> useingGoods = this.GetUseingGoods();
			int num = 20;
			int num2 = Math.Max(useingGoods.Count, num);
			for (int i = 0; i < num2; i++)
			{
				GoodsData goodsData = (i + 1 <= useingGoods.Count) ? useingGoods[i] : null;
				this.ZhuangBeiItems.AddNoUpdate(this.GetGoodsItemIcon(goodsData, num2 > num, true));
			}
			this.ZhuangBeiItems.DelayUpdate();
		}
	}

	private string GetGoodsIconImageURL(int nGoodsID)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(nGoodsID);
		if (goodsXmlNodeByID != null)
		{
			int categoriy = goodsXmlNodeByID.Categoriy;
			string empty = string.Empty;
			return Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		}
		return string.Empty;
	}

	private string GetTextColoeByGoodsID(GoodsData goodsData)
	{
		if (goodsData == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (zhuoyueAttributeCount > 0)
		{
			text2 = "{00FF00}";
		}
		else if (goodsData.Lucky > 0)
		{
			text2 = "{0099FF}";
		}
		else if (goodsData.Forge_level == 3 || goodsData.Forge_level == 4)
		{
			text2 = "{FF9900}";
		}
		else if (goodsData.Forge_level == 5 || goodsData.Forge_level == 6)
		{
			text2 = "{0099FF}";
		}
		else if (goodsData.Forge_level >= 7)
		{
			text2 = "{FFFF00}";
		}
		else
		{
			text2 = "{ffffff}";
		}
		string goodsNameByID = Global.GetGoodsNameByID(goodsData.GoodsID, false);
		text = string.Format("{0}[{1}]{2}", text2, goodsNameByID, "{-}");
		string text3 = string.Format("goodid={0}", goodsData.GoodsID);
		string text4 = string.Format("id={0}", goodsData.Id);
		return string.Concat(new string[]
		{
			Global.GetLang("｛"),
			text3,
			"_",
			text4,
			Global.GetLang("｝"),
			text,
			Global.GetLang("｛-｝")
		});
	}

	public static string GetTextColoeByGoodsIDHTML(GoodsData goodsData)
	{
		if (goodsData == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		string text2 = "{FFFFFF}";
		int zhuoyueAttributeCount = Global.GetZhuoyueAttributeCount(goodsData);
		if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 10 || Global.GetGoodsCatetoriy(goodsData.GoodsID) == 9)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
			if (zhuoyueAttributeCount != 0 || 1 < goodsXmlNodeByID.SuitID)
			{
				text2 = "{ff08ff}";
			}
			else
			{
				text2 = "{0099ff}";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) >= 0 && Global.GetGoodsCatetoriy(goodsData.GoodsID) <= 22 && zhuoyueAttributeCount > 0)
		{
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (Global.IsRebornEquip(Global.GetGoodsCatetoriy(goodsData.GoodsID)) && zhuoyueAttributeCount > 0)
		{
			if (zhuoyueAttributeCount > 0 && zhuoyueAttributeCount <= 2)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount >= 3 && zhuoyueAttributeCount <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (zhuoyueAttributeCount >= 5 && zhuoyueAttributeCount <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) >= 40 && Global.GetGoodsCatetoriy(goodsData.GoodsID) <= 45 && zhuoyueAttributeCount > 0)
		{
			int zhuoyueAttributeCount2 = Global.GetZhuoyueAttributeCount(goodsData);
			if (zhuoyueAttributeCount2 > 0 && zhuoyueAttributeCount2 <= 2)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (zhuoyueAttributeCount2 >= 3 && zhuoyueAttributeCount2 <= 4)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (zhuoyueAttributeCount2 >= 5 && zhuoyueAttributeCount2 <= 6)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (Global.GetGoodsCatetoriy(goodsData.GoodsID) == 340)
		{
			int horseQuality = Super.GetHorseQuality(goodsData);
			if (horseQuality == 1)
			{
				text2 = "{00ff00}";
				text += UIHelper.ZuoyueTitleNames[0];
			}
			else if (horseQuality == 2)
			{
				text += UIHelper.ZuoyueTitleNames[1];
				text2 = "{0099ff}";
			}
			else if (horseQuality >= 3)
			{
				text += UIHelper.ZuoyueTitleNames[2];
				text2 = "{ff08ff}";
			}
		}
		else if (goodsData.Lucky > 0)
		{
			text2 = "{0099FF}";
		}
		string text3 = text + Global.GetGoodsNameByID(goodsData.GoodsID, false);
		string text4 = string.Format("{0}[{1}]{2}", text2, text3, "{-}");
		return string.Concat(new object[]
		{
			Global.GetLang("｛"),
			goodsData.GoodsID,
			"_",
			goodsData.Id,
			"_",
			Global.Data.roleData.RoleID,
			Global.GetLang("｝"),
			text4,
			Global.GetLang("｛-｝")
		});
	}

	private void startAnim(int dir)
	{
		if (dir == 0)
		{
			return;
		}
		this.isTweenIng = true;
		this.m_Dir = dir;
		this.m_Tween = TweenPosition.Begin(this.Body, 0.15f, Vector3.zero, new Vector3(this.fWidth * (float)this.m_Dir, 0f, 0f));
		this.m_Tween.method = 3;
		this.m_Tween.onFinished = new UITweener.OnFinished(this.animFinish);
	}

	private void animFinish(UITweener tweener)
	{
		if (this.m_Dir == 1)
		{
			this.prePage();
		}
		else
		{
			this.nextPage();
		}
		float num = this.fWidth * (float)(-(float)this.m_Dir);
		this.UIDragPl.transform.localPosition = new Vector3(0f, this.UIDragPl.transform.localPosition.y, this.UIDragPl.transform.localPosition.z);
		UIPanel component = this.UIDragPl.GetComponent<UIPanel>();
		component.clipRange = new Vector4(70f, component.clipRange.y, component.clipRange.z, component.clipRange.w);
		this.m_Tween = TweenPosition.Begin(this.Body, 0.25f, new Vector3(num, 0f, 0f), Vector3.zero);
		this.m_Tween.method = 3;
		this.isTweenIng = false;
	}

	private void onDragFinished()
	{
		if (Math.Abs(this.UIDragPl.transform.localPosition.x) > 30f)
		{
			if (this.isTweenIng)
			{
				return;
			}
			if (this.UIDragPl.transform.localPosition.x > 0f && this.currentSelectedPage > 0)
			{
				this.startAnim(1);
			}
			else if (this.UIDragPl.transform.localPosition.x < 0f && this.currentSelectedPage < this.maxPageCount - 1)
			{
				this.startAnim(-1);
			}
		}
	}

	private void nextPage()
	{
		if (this.currentSelectedPage < this.maxPageCount - 1)
		{
			this.currentSelectedPage++;
			this.showPage(this.currentSelectedPage);
		}
	}

	private void prePage()
	{
		if (this.currentSelectedPage > 0)
		{
			this.currentSelectedPage--;
			this.showPage(this.currentSelectedPage);
		}
	}

	private void showPage(int pageIndex)
	{
		if (this.lstBeiBaoWuPin == null)
		{
			return;
		}
		this.BeiBaoItems.Clear();
		this.maxPageCount = (this.lstBeiBaoWuPin.Count - 1) / this.pageNum + 1;
		this.PageText.Text = StringUtil.substitute("{0}/{1}", new object[]
		{
			pageIndex + 1,
			this.maxPageCount
		});
		int num = pageIndex * this.pageNum;
		int num2 = num;
		while (num2 < this.lstBeiBaoWuPin.Count && num2 < num + this.pageNum)
		{
			if (this.lstBeiBaoWuPin[num2] != null)
			{
				this.BeiBaoItems.AddNoUpdate(this.GetGoodsItemIcon(this.lstBeiBaoWuPin[num2], true, false));
			}
			num2++;
		}
		this.BeiBaoItems.DelayUpdate();
	}

	public ListBox lstZhuangbei = new ListBox();

	public ListBox lstBeiBao = new ListBox();

	public GTab m_GTab;

	public GameObject Body;

	public ShowNetImage Bak;

	public DPSelectedItemEventHandler DPSelectedItem;

	private ObservableCollection BeiBaoItems;

	private ObservableCollection ZhuangBeiItems;

	public SpringPanel UIDragPl;

	private List<GoodsData> lstBeiBaoWuPin;

	public TextBlock PageText;

	private bool isTweenIng;

	private int m_Dir;

	private TweenPosition m_Tween;

	private float fWidth = 800f;

	private int currentSelectedPage;

	private int maxPageCount;

	private int pageNum = 20;
}
