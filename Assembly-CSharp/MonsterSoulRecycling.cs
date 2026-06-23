using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class MonsterSoulRecycling : UserControl
{
	private void InitTextInPrefabs()
	{
		this.recyclingBtn.Text = Global.GetLang("立即回收");
		this.cancelBtn.Text = Global.GetLang("取消");
		this.recyclingProgressTxt.Text = Global.GetLang("今日已回收：");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.progressBar.ProgessText = "0/100";
		this.progressBar.BodyHeight = 12.0;
		this.progressBar.BodyWidth = 266.0;
		this.progressBar.Percent = 0.0;
		this.panelPos = this.bagPanel.transform.localPosition;
		this.panelClip = this.bagPanel.clipRange;
		this.recyclingBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			int num = this.TotalAvailableGuardPoints();
			if (num > this.maxRecyclingPoints)
			{
				DPSelectedItemEventHandler handler = delegate(object sender, DPSelectedItemEventArgs args)
				{
					if (args.ID == 0)
					{
						this.RecycylingSoulItems();
					}
				};
				Super.ShowMessageBoxEx(Global.GetLang("达到最大限制提示"), Global.GetLang("当前回收值超过今日最大回收限制,超出部分不会获得,确认要回收?"), handler, new string[]
				{
					Global.GetLang("确定"),
					Global.GetLang("取消")
				}, false);
			}
			else
			{
				this.RecycylingSoulItems();
			}
		};
		this.cancelBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
		this.dragPanel.onDragFinished = delegate()
		{
			float num = Mathf.Round(this.bagPanel.clipRange.y / 78f) * 78f;
			SpringPanel.Begin(this.bagPanel.gameObject, new Vector3(-8f, -num, 0f), 10f);
		};
	}

	protected override void OnDestroy()
	{
		if (this.soulGoodsDataList != null)
		{
			this.soulGoodsDataList.Clear();
			this.soulGoodsDataList = null;
		}
		if (this.selectedGoodsDataDict != null)
		{
			this.selectedGoodsDataDict.Clear();
			this.selectedGoodsDataDict = null;
		}
		this.list_soul = null;
	}

	private void RecycylingSoulItems()
	{
		string recyclingItems = this.GetRecyclingItems();
		if (recyclingItems != null)
		{
			this.RecyclingGuardPoint(recyclingItems);
		}
	}

	private string GetRecyclingItems()
	{
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Keys.Count <= 0)
		{
			return null;
		}
		string text = string.Empty;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (int num in this.selectedGoodsDataDict.Keys)
		{
			SoulRecyclingAttribute soulRecyclingAttribute = this.selectedGoodsDataDict[num];
			int id = soulRecyclingAttribute.id;
			if (!dictionary.ContainsKey(id))
			{
				dictionary.Add(id, 1);
			}
			else
			{
				Dictionary<int, int> dictionary3;
				Dictionary<int, int> dictionary2 = dictionary3 = dictionary;
				int num3;
				int num2 = num3 = id;
				num3 = dictionary3[num3];
				dictionary2[num2] = num3 + 1;
			}
		}
		foreach (int num4 in dictionary.Keys)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				num4.ToString(),
				",",
				dictionary[num4],
				":"
			});
		}
		if (text.Length >= 1)
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text;
	}

	private void RefreshGuardPointCompleteProgress(int recycledPoints, int maxPoints)
	{
		this.recycledPoints = recycledPoints;
		this.maxRecyclingPoints = maxPoints;
		this.RefreshProgressBar(recycledPoints, maxPoints);
	}

	private void RefreshProgressBar(int cur, int next)
	{
		string text = (!this.isChecked) ? "ffffff" : "00ff00";
		this.progressBar.ProgessText = Global.GetColorStringForNGUIText(new object[]
		{
			text,
			string.Format("{0}/{1}", cur, next)
		});
		this.progressBar.PercentByStopTween = (double)cur / (double)next;
	}

	private int TotalAvailableGuardPoints()
	{
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Keys.Count <= 0)
		{
			return this.recycledPoints;
		}
		int num = this.recycledPoints;
		foreach (int num2 in this.selectedGoodsDataDict.Keys)
		{
			SoulRecyclingAttribute soulRecyclingAttribute = this.selectedGoodsDataDict[num2];
			num += soulRecyclingAttribute.recyclingPoint;
		}
		return num;
	}

	private SoulRecyclingAttribute GetMonsterSoulItemAttributeByGoodsID(int goodsID)
	{
		if (this.list_soul == null || this.list_soul.Count <= 0)
		{
			return null;
		}
		foreach (SoulRecyclingItem soulRecyclingItem in this.list_soul)
		{
			if (soulRecyclingItem.goodsID == goodsID)
			{
				return new SoulRecyclingAttribute
				{
					id = soulRecyclingItem.id,
					recyclingPoint = soulRecyclingItem.recyclingPoint
				};
			}
		}
		return null;
	}

	public void InitBag(List<SoulRecyclingItem> soulList)
	{
		if (this.bagOrient == BagOrientTypes.Vertical && !this.isPage)
		{
			this.background.localScale = new Vector3(390f, 1560f, 0f);
			this.background.transform.localPosition = new Vector3(-39f, -741f, 0f);
			this.dragPanel.dragEffect = 2;
			this.dragPanel.momentumAmount = 35f;
			this.dragPanel.scale = Vector3.up;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal && this.isPage)
		{
			this.background.localScale = new Vector3(0f, 1560f, 0f);
			this.background.transform.localPosition = new Vector3(-39f, -741f, 0f);
			this.dragPanel.dragEffect = 0;
			this.dragPanel.momentumAmount = 0f;
			this.dragPanel.scale = Vector3.right;
		}
		this.goodsBox.RowCount = 20;
		this.goodsBox.ColCount = 5;
		this.goodsBox.InitBox();
		this.RefreshMonsterSoulList(soulList);
		this.GetGuardPointsInfo();
	}

	public void RefreshMonsterSoulList(List<SoulRecyclingItem> soulList)
	{
		if (soulList == null || soulList.Count <= 0)
		{
			this.goodsBox.ClearAllGoodsIcon();
			return;
		}
		soulList.Sort(new Comparison<SoulRecyclingItem>(this.SortGoodsDataList));
		if (this.soulGoodsDataList == null)
		{
			this.soulGoodsDataList = new List<GoodsData>();
		}
		this.list_soul = soulList;
		int num = 0;
		this.soulGoodsDataList.Clear();
		for (int i = 0; i < soulList.Count; i++)
		{
			SoulRecyclingItem soulRecyclingItem = soulList[i];
			if (soulRecyclingItem != null && soulRecyclingItem.goodsID > 0)
			{
				GoodsData goodsDataByID = Global.GetGoodsDataByID(soulRecyclingItem.goodsID);
				if (goodsDataByID != null)
				{
					for (int j = 0; j < soulRecyclingItem.goodsCount; j++)
					{
						num++;
						GoodsData goodsData = Global.CloneGoodsData(goodsDataByID, false);
						goodsData.Id = num;
						goodsData.GCount = 1;
						this.soulGoodsDataList.Add(goodsData);
					}
					if (this.soulGoodsDataList.Count >= 100)
					{
						break;
					}
				}
			}
		}
		base.StartCoroutine<bool>(this.ShowPage(this.currentSelectedPage));
	}

	private IEnumerator ShowPage(int pageIndex)
	{
		if (this.soulGoodsDataList == null || this.soulGoodsDataList.Count <= 0)
		{
			yield return null;
		}
		this.goodsBox.ClearAllGoodsIcon();
		int gridCount = Mathf.Min(this.soulGoodsDataList.Count, 100);
		for (int i = 0; i < gridCount; i++)
		{
			if (i < this.soulGoodsDataList.Count)
			{
				GoodsData gd = this.soulGoodsDataList[i];
				GGoodIcon icon = this.AddIcon(gd);
				this.goodsBox.SetGoodsIcon(this.Getindex(i), icon);
				icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			}
			yield return null;
		}
		yield return null;
		yield break;
	}

	private GGoodIcon AddIcon(GoodsData goodsData)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 78.0;
			ggoodIcon.Height = 78.0;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = -1;
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			ggoodIcon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
			};
			Super.InitGoodsGIcon(ggoodIcon, goodsData, true, IconTextTypes.Qianghua);
			return ggoodIcon;
		}
		return null;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon icon = evt.target.SafeGetComponent<GGoodIcon>();
		this.SelectIcon(icon);
	}

	private void SelectIcon(GGoodIcon icon)
	{
		if (icon != null)
		{
			GoodsData goodsData = icon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			int id = goodsData.Id;
			bool state = !this.selectedGoodsDataDict.ContainsKey(id);
			this.SelectIconExe(state, icon);
		}
	}

	private IEnumerator SelectAllAvailableIcon()
	{
		if (null == this.goodsBox)
		{
			yield return null;
		}
		ObservableCollection items = this.goodsBox.listBox.ItemsSource;
		for (int i = 0; i < items.Count; i++)
		{
			GGoodIcon icon = U3DUtils.AS<GGoodIcon>(items[i]);
			if (null != icon)
			{
				GoodsData gd = icon.ItemObject as GoodsData;
				int key = gd.Id;
				SoulRecyclingAttribute recyclingAttr = this.GetMonsterSoulItemAttributeByGoodsID(gd.GoodsID);
				int totalPoints = this.TotalAvailableGuardPoints();
				if (totalPoints >= this.maxRecyclingPoints)
				{
					break;
				}
				if (!this.selectedGoodsDataDict.ContainsKey(key))
				{
					this.selectedGoodsDataDict.Add(key, recyclingAttr);
				}
				icon.BackSpriteName2 = "iconState_selected";
				this.isChecked = true;
				this.AfterSelectedIcon();
			}
			yield return null;
		}
		yield return null;
		yield break;
	}

	private void SelectIconExe(bool state, GGoodIcon icon)
	{
		if (null == icon || this.selectedGoodsDataDict == null)
		{
			return;
		}
		GoodsData goodsData = icon.ItemObject as GoodsData;
		if (goodsData == null)
		{
			return;
		}
		int id = goodsData.Id;
		if (!state)
		{
			this.selectedGoodsDataDict.Remove(id);
			icon.BackSpriteName2 = string.Empty;
			icon.BackgroundSprite2.gameObject.SetActive(false);
			if (this.recycledPoints == this.TotalAvailableGuardPoints())
			{
				this.isChecked = false;
			}
		}
		else
		{
			if (this.recycledPoints >= this.maxRecyclingPoints)
			{
				Super.HintMainText(Global.GetLang("已达到今日回收上限"), 10, 3);
				return;
			}
			SoulRecyclingAttribute monsterSoulItemAttributeByGoodsID = this.GetMonsterSoulItemAttributeByGoodsID(goodsData.GoodsID);
			int num = this.TotalAvailableGuardPoints();
			if (num > this.maxRecyclingPoints)
			{
				Super.HintMainText(Global.GetLang("已达到今日回收上限"), 10, 3);
				return;
			}
			this.selectedGoodsDataDict.Add(id, monsterSoulItemAttributeByGoodsID);
			icon.BackSpriteName2 = "iconState_selected";
			this.isChecked = true;
		}
		this.AfterSelectedIcon();
	}

	private void AfterSelectedIcon()
	{
		int cur = this.TotalAvailableGuardPoints();
		this.RefreshProgressBar(cur, this.maxRecyclingPoints);
	}

	private void ClearSelectedIcons()
	{
		if (this.selectedGoodsDataDict != null)
		{
			this.selectedGoodsDataDict.Clear();
		}
	}

	private int SortGoodsDataList(SoulRecyclingItem x, SoulRecyclingItem y)
	{
		return x.recyclingPoint - y.recyclingPoint;
	}

	private int GetGoodsIconIndex(GoodsData goodsData)
	{
		return this.Getindex(goodsData.BagIndex);
	}

	private int Getindex(int bagIndex)
	{
		int result = -1;
		if (this.bagOrient == BagOrientTypes.Vertical && !this.isPage)
		{
			result = bagIndex;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal && this.isPage)
		{
			int num = 5;
			int num2 = 20;
			int num3 = 0;
			this.goodsBox.listBox.maxPerLine = num3;
			int num4 = bagIndex / num / num2;
			int num5 = bagIndex % (num * num2);
			int num6 = num5 % num;
			int num7 = num5 / num % num2;
			result = num6 + num7 * num3 + num4 * num;
		}
		return result;
	}

	public void SetGuardPointInfo(int status, int recycledPoints, int maxPoints)
	{
		Super.HideNetWaiting();
		if (status != 0)
		{
			if (status == 1)
			{
				Super.HintMainText(Global.GetLang("需要完成3转1级的【废柴壁垒】才可以开启守护雕像"), 10, 3);
			}
		}
		else
		{
			this.isChecked = false;
			this.RefreshGuardPointCompleteProgress(recycledPoints, maxPoints);
			if (this.recycledPoints < this.maxRecyclingPoints)
			{
				base.StartCoroutine<bool>(this.SelectAllAvailableIcon());
			}
		}
	}

	public void SetRecyclingResult(int status, int recycledPoints, int maxPoints)
	{
		string textMsg = string.Empty;
		switch (status)
		{
		case 0:
			this.isChecked = false;
			this.RefreshGuardPointCompleteProgress(recycledPoints, maxPoints);
			this.ClearSelectedIcons();
			break;
		case 1:
			textMsg = Global.GetLang("需要完成3转1级的【废柴壁垒】才可以开启守护雕像");
			break;
		default:
			if (status == 11)
			{
				textMsg = Global.GetLang("请稍后再试");
			}
			break;
		case 3:
			textMsg = Global.GetLang("已达到今日回收上限");
			break;
		case 5:
			textMsg = Global.GetLang("材料不足");
			break;
		}
		if (status != 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
		}
	}

	private void RecyclingGuardPoint(string items)
	{
		GameInstance.Game.RecyclingGuardPoint(items);
	}

	private void GetGuardPointsInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.GetGuardPoint();
	}

	private const int maxGridCount = 100;

	private const int rowsInPage = 20;

	private const int columnsInPage = 5;

	private const int columns = 0;

	private const int bagSizeAPage = 100;

	private const int aGridSize = 78;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton recyclingBtn;

	public GButton cancelBtn;

	public GImgProgressBar progressBar;

	public GGoodsBox goodsBox;

	public TextBlock recyclingProgressTxt;

	public UIPanel bagPanel;

	public UIDraggablePanel dragPanel;

	private Vector3 panelPos = Vector3.zero;

	private Vector4 panelClip = Vector4.zero;

	public Transform background;

	private BagOrientTypes bagOrient = BagOrientTypes.Vertical;

	private bool isPage;

	private int currentSelectedPage;

	private int recycledPoints;

	private int maxRecyclingPoints = 100;

	private Dictionary<int, SoulRecyclingAttribute> selectedGoodsDataDict = new Dictionary<int, SoulRecyclingAttribute>();

	private List<SoulRecyclingItem> list_soul;

	private List<GoodsData> soulGoodsDataList;

	private bool isChecked;
}
