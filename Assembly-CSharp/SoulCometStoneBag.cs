using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class SoulCometStoneBag : UserControl
{
	private void InitTextInPrefabs()
	{
		this.backBtn.Text = Global.GetLang("返回");
		this.resortBtn.Text = Global.GetLang("整理");
	}

	public ImageBrush BodyBackground
	{
		set
		{
			this.Container.Background = value;
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.springPanel.GetComponent<UIDraggablePanel>().onDragFinished = new UIDraggablePanel.OnDragFinished(this.onDragFinished);
		this.backBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ResetBag();
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1200
			});
		};
		this.resortBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.SortSoulCometStoneBagRequest();
		};
		this.goodsBox.RowCount = 20;
		this.goodsBox.ColCount = 5;
		this.goodsBox.InitBox();
	}

	public override void Destroy()
	{
		if (this.selectedGoodsDataDict != null)
		{
			this.selectedGoodsDataDict.Clear();
			this.selectedGoodsDataDict = null;
		}
	}

	public void ResetBag()
	{
		this.CancelSelectIcon();
		this.currentGoodIcon = null;
		this.currentSelectedGoodsData = null;
	}

	public SoulCometStoneBagTypes soulCometStoneBagTypes
	{
		get
		{
			return this._soulCometStoneBagTypes;
		}
		set
		{
			if (value == SoulCometStoneBagTypes.Strengthening_Bag)
			{
				if (this._soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Body)
				{
					this._soulCometStoneBagTypes = SoulCometStoneBagTypes.Strengthening_Bag__FromBody;
				}
				else if (this._soulCometStoneBagTypes == SoulCometStoneBagTypes.Gathering)
				{
					this._soulCometStoneBagTypes = SoulCometStoneBagTypes.Strengthening_Bag__FromGathering;
				}
			}
			else
			{
				this._soulCometStoneBagTypes = value;
			}
			if ((this._soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromBody || this._soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromGathering || this._soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag || this._soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Body) && this.selectedGoodsDataDict == null)
			{
				this.selectedGoodsDataDict = new Dictionary<int, GoodsData>();
			}
		}
	}

	public void InitBag()
	{
		List<GoodsData> bagSoulCometStoneList = Global.GetBagSoulCometStoneList();
		if (bagSoulCometStoneList == null || bagSoulCometStoneList.Count <= 0)
		{
			base.StartCoroutine<bool>(this.ClearBag());
			return;
		}
		base.StartCoroutine<bool>(this.RefreshGoodsList(bagSoulCometStoneList));
	}

	private void onDragFinished()
	{
		int num = 5;
		int num2 = 312;
		if (Math.Abs(Math.Abs(this.springPanel.transform.localPosition.y) - (float)(num2 * this.currentSelectedPage)) > 30f)
		{
			if (this.springPanel.transform.localPosition.y <= (float)(num2 * this.currentSelectedPage))
			{
				this.currentSelectedPage--;
				if (this.currentSelectedPage <= 0)
				{
					this.currentSelectedPage = 0;
				}
			}
			else
			{
				this.currentSelectedPage++;
				if (this.currentSelectedPage >= num)
				{
					this.currentSelectedPage = num - 1;
				}
			}
		}
		this.springPanel.target.y = (float)(num2 * this.currentSelectedPage);
		this.springPanel.enabled = true;
	}

	private IEnumerator RefreshGoodsList(List<GoodsData> list_goods)
	{
		yield return base.StartCoroutine<bool>(this.ClearBag());
		if (list_goods == null || list_goods.Count <= 0)
		{
			yield break;
		}
		int counter = 0;
		for (int i = 0; i < list_goods.Count; i++)
		{
			GoodsData gd = list_goods[i];
			GGoodIcon icon = this.AddIcon(gd, null);
			this.goodsBox.SetGoodsIcon(this.Getindex(gd.BagIndex), icon);
			icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			counter++;
			if (counter % 5 == 0)
			{
				yield return null;
			}
		}
		this.RevertSelectState();
		yield break;
	}

	private IEnumerator ClearBag()
	{
		int counter = 0;
		for (int i = 0; i < 100; i++)
		{
			GGoodIcon icon = this.AddEmptyIcon();
			this.goodsBox.SetGoodsIcon(this.Getindex(i), icon);
			counter++;
			if (counter % 100 == 0)
			{
				yield return null;
			}
		}
		yield break;
	}

	public void ReplaceGoodsIcon(GoodsData gd)
	{
		if (gd == null)
		{
			return;
		}
		GoodsData bagSoulCometStoneGoodsDataByDBID = Global.GetBagSoulCometStoneGoodsDataByDBID(gd.Id);
		GGoodIcon icon;
		if (bagSoulCometStoneGoodsDataByDBID != null)
		{
			icon = this.AddIcon(gd, null);
		}
		else
		{
			icon = this.AddEmptyIcon();
		}
		this.ReplaceGoodsIconAtIndex(gd.BagIndex, icon);
	}

	private void ReplaceGoodsIconAtIndex(int bagIndex, GGoodIcon icon)
	{
		if (null != this.goodsBox)
		{
			int num = this.Getindex(bagIndex);
			if (null != icon)
			{
				this.goodsBox.SetGoodsIcon(num, icon);
				icon.addEventListener("click", new MouseEventHandler(this.IconMouseLeftButtonUp));
			}
		}
	}

	private GGoodIcon AddIcon(GoodsData goodsData, MouseLeftButtonUpEventHandler handler = null)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			GGoodIcon icon = U3DUtils.NEW<GGoodIcon>();
			icon.Width = 78.0;
			icon.Height = 78.0;
			icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
			{
				goodsXmlNodeByID.IconCode
			}), false, 0);
			icon.TipType = 1;
			icon.ItemCategory = goodsXmlNodeByID.Categoriy;
			icon.ItemCode = goodsData.GoodsID;
			icon.ItemObject = goodsData;
			icon.BoxTypes = -1;
			icon.TextColor = 15793920U;
			int num = 0;
			int soulCometStoneLevel = Global.GetSoulCometStoneLevel(goodsData, out num);
			icon.ContentText.Text = "Lv" + soulCometStoneLevel;
			icon.gameObject.AddComponent<UIDragPanelContents>();
			icon.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs ev)
			{
				if (ev.IDType == 3)
				{
					if (null != this.currentGoodIcon)
					{
						int dbid = -1;
						if (this.currentSelectedGoodsData != null)
						{
							dbid = this.currentSelectedGoodsData.Id;
						}
						this.SelectIcon(icon, dbid);
					}
					this.StrengtheningSoulCometStone(this.currentSelectedGoodsData, SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_Reload);
				}
			};
			Super.InitGoodsGIcon(icon, goodsData, true, IconTextTypes.Qianghua);
			return icon;
		}
		return null;
	}

	private GGoodIcon AddEmptyIcon()
	{
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		return ggoodIcon;
	}

	private void IconMouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null == ggoodIcon)
		{
			return;
		}
		if (this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromBody || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromGathering || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Body)
		{
			int dbid = -1;
			if (this.currentSelectedGoodsData != null)
			{
				dbid = this.currentSelectedGoodsData.Id;
			}
			this.SelectIcon(ggoodIcon, dbid);
		}
		else
		{
			GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
			this.currentSelectedGoodsData = goodData;
			this.currentGoodIcon = ggoodIcon;
			GTipServiceEx.ShowTip(ggoodIcon, TipTypes.SoulCometStoneBagTip, GoodsOwnerTypes.SoulCometStoneBag, goodData);
		}
	}

	private int Getindex(int bagIndex)
	{
		int result = -1;
		if (this.bagOrient == BagOrientTypes.Vertical)
		{
			result = bagIndex;
		}
		else if (this.bagOrient == BagOrientTypes.Horizontal)
		{
			int num = 5;
			int num2 = 4;
			int num3 = 25;
			this.goodsBox.listBox.maxPerLine = num3;
			int num4 = bagIndex / num / num2;
			int num5 = bagIndex % (num * num2);
			int num6 = num5 % num;
			int num7 = num5 / num % num2;
			result = num6 + num7 * num3 + num4 * num;
		}
		return result;
	}

	private void SelectIcon(GGoodIcon icon, int dbid = -1)
	{
		if (icon != null)
		{
			GoodsData goodsData = icon.ItemObject as GoodsData;
			if (goodsData == null)
			{
				return;
			}
			int id = goodsData.Id;
			if (dbid > 0 && dbid == id)
			{
				icon.BackSpriteName2 = "iconState_qianghua";
				return;
			}
			bool state = false;
			if (this.selectedGoodsDataDict.ContainsKey(id))
			{
				state = false;
				this.SelectIconExe(state, icon);
			}
			else
			{
				GoodsQuality goodsQuality = (GoodsQuality)Super.GetGoodsQuality(goodsData.GoodsID);
				if (goodsQuality >= GoodsQuality.Purple && Super.MessageBoxIsHint[8] == 0)
				{
					GChildWindow messageBoxWindow = Super.ShowMessageBox(Global.GetLang("提示"), Global.GetLang("您选择的魂石为紫色道具，确定要吞噬？"), 2, null, MessBoxIsHintTypes.SoulCometStoneEventHint);
					messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
					{
						int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
						Super.CloseMessageBox(this.Container, messageBoxWindow);
						if (messageBoxReturn == 0)
						{
							state = true;
							this.SelectIconExe(state, icon);
						}
						return true;
					};
				}
				else
				{
					state = true;
					this.SelectIconExe(state, icon);
				}
			}
		}
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
		}
		else
		{
			this.selectedGoodsDataDict.Add(id, goodsData);
			icon.BackSpriteName2 = "iconState_selected";
		}
		this.AfterSelectedIcon();
	}

	public void SelectIconByColor(GoodsQuality goodColor, bool state, int dbid = -1)
	{
		ObservableCollection itemsSource = this.goodsBox.listBox.ItemsSource;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(itemsSource[i]);
			if (null != ggoodIcon)
			{
				GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
				if (goodsData != null)
				{
					int id = goodsData.Id;
					if (dbid > 0 && dbid == id)
					{
						ggoodIcon.BackSpriteName2 = "iconState_qianghua";
					}
					else
					{
						GoodsQuality goodsQuality = (GoodsQuality)Super.GetGoodsQuality(goodsData.GoodsID);
						if (goodsQuality == goodColor)
						{
							if (state)
							{
								ggoodIcon.BackSpriteName2 = "iconState_selected";
								if (!this.selectedGoodsDataDict.ContainsKey(id))
								{
									this.selectedGoodsDataDict.Add(id, goodsData);
								}
							}
							else
							{
								ggoodIcon.BackSpriteName2 = string.Empty;
								ggoodIcon.BackgroundSprite2.gameObject.SetActive(false);
								if (this.selectedGoodsDataDict.ContainsKey(id))
								{
									this.selectedGoodsDataDict.Remove(id);
								}
							}
						}
					}
				}
			}
		}
		this.AfterSelectedIcon();
	}

	public void CancelSelectIcon()
	{
		if (null != this.currentGoodIcon)
		{
			this.currentGoodIcon.BackSpriteName2 = string.Empty;
			this.currentGoodIcon.BackgroundSprite2.gameObject.SetActive(false);
		}
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Count <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.goodsBox.listBox.ItemsSource;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(itemsSource[i]);
			if (null != ggoodIcon)
			{
				ggoodIcon.BackSpriteName2 = string.Empty;
				ggoodIcon.BackgroundSprite2.gameObject.SetActive(false);
			}
		}
		this.selectedGoodsDataDict.Clear();
		this.AfterSelectedIcon();
	}

	private void AfterSelectedIcon()
	{
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = 1100,
			Data = this.selectedGoodsDataDict
		});
	}

	public void RemoveSelectIcon()
	{
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Count <= 0)
		{
			return;
		}
		foreach (GoodsData gd in this.selectedGoodsDataDict.Values)
		{
			this.ReplaceGoodsIcon(gd);
		}
		this.selectedGoodsDataDict.Clear();
	}

	public GoodsData SelectGoodsIconInStrengthening(int dbid)
	{
		GoodsData bagSoulCometStoneGoodsDataByDBID = Global.GetBagSoulCometStoneGoodsDataByDBID(dbid);
		if (bagSoulCometStoneGoodsDataByDBID == null)
		{
			return null;
		}
		int num = this.Getindex(bagSoulCometStoneGoodsDataByDBID.BagIndex);
		GGoodIcon goodsIcon = this.goodsBox.GetGoodsIcon(num);
		this.currentGoodIcon = goodsIcon;
		this.currentSelectedGoodsData = bagSoulCometStoneGoodsDataByDBID;
		this.SelectIcon(goodsIcon, dbid);
		return bagSoulCometStoneGoodsDataByDBID;
	}

	public void SelectGoodsIconsInDictionary(int dbid = -1)
	{
		if (this.selectedGoodsDataDict == null || this.selectedGoodsDataDict.Count <= 0)
		{
			return;
		}
		ObservableCollection itemsSource = this.goodsBox.listBox.ItemsSource;
		for (int i = 0; i < itemsSource.Count; i++)
		{
			GGoodIcon ggoodIcon = U3DUtils.AS<GGoodIcon>(itemsSource[i]);
			if (null != ggoodIcon)
			{
				GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
				if (goodsData != null)
				{
					int id = goodsData.Id;
					if (dbid > 0 && dbid == id)
					{
						ggoodIcon.BackSpriteName2 = "iconState_qianghua";
					}
					else if (this.selectedGoodsDataDict.ContainsKey(id))
					{
						ggoodIcon.BackSpriteName2 = "iconState_selected";
						this.selectedGoodsDataDict[id] = goodsData;
					}
					else
					{
						ggoodIcon.BackSpriteName2 = string.Empty;
						ggoodIcon.BackgroundSprite2.gameObject.SetActive(false);
					}
				}
			}
		}
	}

	private void RevertSelectState()
	{
		if (this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromBody || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromGathering || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Body)
		{
			this.SelectGoodsIconsInDictionary((this.currentSelectedGoodsData == null) ? -1 : this.currentSelectedGoodsData.Id);
		}
		if ((this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromBody || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag__FromGathering || this.soulCometStoneBagTypes == SoulCometStoneBagTypes.Strengthening_Bag) && this.currentSelectedGoodsData != null)
		{
			GoodsData goodsData = this.SelectGoodsIconInStrengthening(this.currentSelectedGoodsData.Id);
			if (goodsData != null)
			{
				this.StrengtheningSoulCometStone(this.currentSelectedGoodsData, SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_DataOnly);
			}
		}
	}

	private void StrengtheningSoulCometStone(GoodsData gd, SoulCometStoneStrengtheningDataChangeType dataChangeType = SoulCometStoneStrengtheningDataChangeType.StrengtheningDataChangeType_Reload)
	{
		if (gd == null)
		{
			return;
		}
		GoodsData goodsData = Global.CloneGoodsData(gd, false);
		goodsData.Id = gd.Id;
		this.DPSelectedItem(this, new DPSelectedItemEventArgs
		{
			ID = 1000,
			Data = goodsData,
			Type = 1,
			IDType = (int)dataChangeType
		});
	}

	public void SetBagSortResult(List<GoodsData> list_goods)
	{
		Global.SetBagSoulCometStoneList(list_goods);
		base.StartCoroutine<bool>(this.RefreshGoodsList(list_goods));
	}

	private void SortSoulCometStoneBagRequest()
	{
		GameInstance.Game.SortSoulCometStoneBag();
	}

	private const int maxGridCount = 100;

	private const int rowsInPage = 4;

	private const int columnsInPage = 5;

	private const int aGridSize = 78;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton backBtn;

	public GButton resortBtn;

	public GGoodsBox goodsBox;

	public SpringPanel springPanel;

	private BagOrientTypes bagOrient = BagOrientTypes.Vertical;

	public UIPanel bagPanel;

	public UIDraggablePanel dragPanel;

	private int currentSelectedPage;

	private Dictionary<int, GoodsData> selectedGoodsDataDict;

	private GGoodIcon currentGoodIcon;

	private GoodsData currentSelectedGoodsData;

	private SoulCometStoneBagTypes _soulCometStoneBagTypes = SoulCometStoneBagTypes.Gathering;
}
