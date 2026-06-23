using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class YaoSaiPaiZhuJingLingPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	private int mBossId { get; set; }

	public int mJingLingKey { get; set; }

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
		this.ItemCollection = this.mListBox.ItemsSource;
		this.mListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.lbTopPlayer_SelectionChanged);
	}

	private void InitTextInPrefabs()
	{
		this.BtnChangeText = Global.GetLang("上阵");
		this.mLblTips.Text = Global.GetLang("背包中的精灵派驻到精灵要塞后, 才可以在要塞BOSS战斗中上阵");
		this.mLblLevel.Text = string.Empty;
		this.mLblDamageValue.Pivot = 5;
		this.mLblDamageValue.X = 80.0;
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseHandler != null)
			{
				this.CloseHandler(null, new DPSelectedItemEventArgs
				{
					IDType = 0
				});
			}
		};
		this.mBtnChange.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (null == this.mCurrentYaoSaiPaiZhuJingLingItem)
			{
				return;
			}
			if (this.mCurrentYaoSaiPaiZhuJingLingItem.IsShangZhen)
			{
				this.mCurrentYaoSaiPaiZhuJingLingItem.CheXiao();
				this.mCurrentJingLingDBId = 0;
				this.ReplaceDBIdInJingLingZhenRongDict(this.mCurrentYaoSaiPaiZhuJingLingItem.mJingLingData.Id, true);
				if (this.SelectJingLingHandler != null)
				{
					this.SelectJingLingHandler(null, new DPSelectedItemEventArgs
					{
						Title = this.GetJingLingStrID(),
						Flag = 0
					});
				}
				this.BtnChangeText = Global.GetLang("上阵");
			}
			else
			{
				if (null != this.mLastYaoSaiPaiZhuJingLingItem)
				{
					this.mLastYaoSaiPaiZhuJingLingItem.CheXiao();
				}
				if (null != this.mCacheYaoSaiPaiZhuJingLingItem)
				{
					this.mCacheYaoSaiPaiZhuJingLingItem.CheXiao();
					this.mCacheYaoSaiPaiZhuJingLingItem.IsSelected = false;
					this.mCacheYaoSaiPaiZhuJingLingItem = null;
				}
				if (this.lastClickItems.Count > 0)
				{
					for (int i = 0; i < this.lastClickItems.Count; i++)
					{
						if (!this.lastClickItems[i].IsShangZhen)
						{
							this.lastClickItems[i].CheXiao();
						}
					}
					this.lastClickItems.Clear();
				}
				this.mCurrentYaoSaiPaiZhuJingLingItem.ShangZhen();
				this.mCurrentJingLingDBId = this.mCurrentYaoSaiPaiZhuJingLingItem.mJingLingData.Id;
				this.SaveJingLingDBIdToDict(this.mJingLingKey, this.mCurrentJingLingDBId);
				if (this.SelectJingLingHandler != null)
				{
					this.SelectJingLingHandler(null, new DPSelectedItemEventArgs
					{
						Title = this.GetJingLingStrID(),
						Flag = 1
					});
				}
				this.BtnChangeText = Global.GetLang("撤销");
			}
		};
	}

	private void InitValue()
	{
		this.JingLingIconById = 0;
		this.jingLingZhenRongdict.Add(0, 0);
		this.jingLingZhenRongdict.Add(1, 0);
		this.jingLingZhenRongdict.Add(2, 0);
	}

	public void InitJingLingList(int DBId, int bossId, string jingLingZhenRongDBIdStr, int index)
	{
		this.mJingLingKey = index;
		this.mBossId = bossId;
		this.IsShowJingLingContent = false;
		List<GoodsData> list = Global.GetRolePaiPets(false);
		if (list == null || list.Count <= 0)
		{
			NGUITools.SetActive(this.mBtnChange.gameObject, false);
			NGUITools.SetActive(this.mLblTips.gameObject, true);
			return;
		}
		NGUITools.SetActive(this.mBtnChange.gameObject, false);
		NGUITools.SetActive(this.mLblTips.gameObject, false);
		this.ItemCollection.Clear();
		if (string.IsNullOrEmpty(jingLingZhenRongDBIdStr))
		{
			return;
		}
		string[] array = jingLingZhenRongDBIdStr.Split(new char[]
		{
			'|'
		});
		this.ReSaveJingLingZhenRongToDict(jingLingZhenRongDBIdStr);
		List<GoodsData> list2 = new List<GoodsData>();
		List<GoodsData> list3 = new List<GoodsData>();
		List<GoodsData> list4 = new List<GoodsData>();
		for (int i = 0; i < list.Count; i++)
		{
			GoodsData goodsData = list[i];
			if (goodsData.Site == 10001)
			{
				list2.Add(goodsData);
			}
			else
			{
				list3.Add(goodsData);
			}
		}
		if (list3.Count > 0)
		{
			this.CustomSort(list3);
			list4.AddRange(list3);
		}
		if (list2.Count > 0)
		{
			this.CustomSort(list2);
			list4.AddRange(list2);
		}
		if (list4.Count > 0)
		{
			list = list4;
		}
		else
		{
			this.CustomSort(list);
		}
		for (int j = 0; j < list.Count; j++)
		{
			YaoSaiPaiZhuJingLingItem yaoSaiPaiZhuJingLingItem = U3DUtils.NEW<YaoSaiPaiZhuJingLingItem>();
			yaoSaiPaiZhuJingLingItem.InitItemData(list[j]);
			for (int k = 0; k < array.Length; k++)
			{
				if (list[j].Id == ConvertExt.SafeConvertToInt32(array[k]) && list[j].Site != 10001)
				{
					yaoSaiPaiZhuJingLingItem.ShangZhen();
				}
			}
			yaoSaiPaiZhuJingLingItem.JingLingDataCallBack = delegate(object s, DPSelectedItemEventArgs e)
			{
				YaoSaiPaiZhuJingLingItem item = s as YaoSaiPaiZhuJingLingItem;
				this.ShowJingLingDetails(item);
			};
			UIPanel component = yaoSaiPaiZhuJingLingItem.transform.GetComponent<UIPanel>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			this.ItemCollection.Add(yaoSaiPaiZhuJingLingItem);
		}
		this.mScrollBar.scrollValue = 0f;
	}

	private void CustomSort(List<GoodsData> list)
	{
		list.Sort(delegate(GoodsData d1, GoodsData d2)
		{
			if ((d1.Forge_level + 1).CompareTo(d2.Forge_level + 1) == 1)
			{
				return -1;
			}
			if ((d1.Forge_level + 1).CompareTo(d2.Forge_level + 1) != 0)
			{
				return 1;
			}
			if (Global.GetZhuoyueAttributeCount(d1).CompareTo(Global.GetZhuoyueAttributeCount(d2)) == 1)
			{
				return -1;
			}
			if (Global.GetZhuoyueAttributeCount(d1).CompareTo(Global.GetZhuoyueAttributeCount(d2)) == 0)
			{
				return 0;
			}
			if (Global.GetZhuoyueAttributeCount(d1).CompareTo(Global.GetZhuoyueAttributeCount(d2)) == -1)
			{
				return 1;
			}
			if (d1.Id.CompareTo(d2.Id) == 1)
			{
				return -1;
			}
			return 0;
		});
	}

	private void lbTopPlayer_SelectionChanged(object sender, object e)
	{
		ListBox listBox = sender as ListBox;
		if (null != listBox)
		{
			if (null != listBox.LastSelectedItem)
			{
				YaoSaiPaiZhuJingLingItem component = listBox.LastSelectedItem.GetComponent<YaoSaiPaiZhuJingLingItem>();
				this.mLastYaoSaiPaiZhuJingLingItem = component;
				component.IsSelected = false;
				this.lastClickItems.Add(component);
			}
			GameObject itemByIndex = listBox.GetItemByIndex(listBox.SelectedIndex);
			if (null != itemByIndex)
			{
				YaoSaiPaiZhuJingLingItem component2 = itemByIndex.GetComponent<YaoSaiPaiZhuJingLingItem>();
				this.mCurrentYaoSaiPaiZhuJingLingItem = component2;
				int site = component2.mJingLingData.Site;
				if (site == 10001)
				{
					Super.HintMainText(Global.GetLang("正在任务中的精灵不能上阵"), 10, 3);
					this.ClearRightDescribe();
					return;
				}
				component2.IsSelected = true;
				this.ShowJingLingDetails(component2);
			}
		}
	}

	private void ShowJingLingDetails(YaoSaiPaiZhuJingLingItem item)
	{
		if (item == null)
		{
			return;
		}
		GoodsData mJingLingData = item.mJingLingData;
		if (mJingLingData == null)
		{
			return;
		}
		if (null != this.mCacheYaoSaiPaiZhuJingLingItem)
		{
			this.mCacheYaoSaiPaiZhuJingLingItem.IsSelected = false;
		}
		NGUITools.SetActive(this.mBtnChange.gameObject, true);
		if (item.IsShangZhen)
		{
			this.BtnChangeText = Global.GetLang("撤销");
		}
		else
		{
			this.BtnChangeText = Global.GetLang("上阵");
		}
		if (item.mIsRenWuIng)
		{
			this.BtnChangeText = Global.GetLang("任务中");
			this.SetBtnStatus(this.mBtnChange, false);
		}
		else
		{
			this.SetBtnStatus(this.mBtnChange, true);
		}
		this.mCurrentJingLingDBId = mJingLingData.Id;
		this.IsShowJingLingContent = true;
		this.JingLingLevel = mJingLingData.Forge_level + 1;
		this.JingLingIconById = mJingLingData.Id;
		this.mLblName.Text = ConfigGoods.GetGoodsXmlNodeByID(mJingLingData.GoodsID).Title;
		string chineseText = string.Format("{0}{1}", Global.GetLang("伤害增加："), this.GetAverageDamage(this.mCurrentJingLingDBId));
		this.mLblDamageValue.Text = Global.GetLang(chineseText);
		string chineseText2 = string.Format("{0}{1}", Global.GetLang("卓越属性条数: "), Global.GetZhuoyueAttributeCount(mJingLingData));
		this.mLblZhuoYueValue.Text = Global.GetLang(chineseText2);
	}

	private int GetAverageDamage(int id)
	{
		PetBossXMLData dataByID = PetBossXMLConfigManager.GetDataByID(this.mBossId);
		if (dataByID == null)
		{
			return 0;
		}
		GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData x) => x.Id == id);
		int num = 1 + (1 + goodsData.Forge_level) / dataByID.PetLevelStep;
		string[] array = dataByID.PetLevelStepNum.Split(new char[]
		{
			','
		});
		int num2 = num * ConvertExt.SafeConvertToInt32(array[0]);
		int num3 = num * ConvertExt.SafeConvertToInt32(array[1]);
		string[] array2 = dataByID.ExcellentStepNum.Split(new char[]
		{
			','
		});
		int num4 = 1 + Global.GetZhuoyueAttributeCount(goodsData) / dataByID.ExcellentStep;
		int num5 = num4 * ConvertExt.SafeConvertToInt32(array2[0]);
		int num6 = num4 * ConvertExt.SafeConvertToInt32(array2[1]);
		float num7 = (float)(num2 + num5);
		float num8 = (float)(num3 + num6);
		return Mathf.CeilToInt((num7 + num8) / 2f);
	}

	private string BtnChangeText
	{
		set
		{
			this.mBtnChange.Text = Global.GetLang(value);
		}
	}

	private bool IsShowJingLingContent
	{
		set
		{
			NGUITools.SetActive(this.mJingLingContent, value);
		}
	}

	private int JingLingIconById
	{
		set
		{
			if (value == 0)
			{
				this.mImgIcon.URL = "NetImages/GameRes/Images/YaoSaiBossTexture/defalut_jinglingKuang.png";
			}
			else
			{
				if (this.mJingLingParent != null)
				{
					int childCount = this.mJingLingParent.childCount;
					if (childCount > 0)
					{
						for (int i = 0; i < childCount; i++)
						{
							Object.Destroy(this.mJingLingParent.GetChild(i).gameObject);
						}
					}
				}
				this.AddJingLingIcon(value);
			}
		}
	}

	private string JingLingName
	{
		set
		{
			this.mLblName.Text = Global.GetLang(value);
		}
	}

	private int JingLingLevel { get; set; }

	private int JingLingZhuoYueValue
	{
		set
		{
			this.mLblZhuoYueValue.Text = value.ToString();
		}
	}

	public void AddJingLingIcon(int DBID)
	{
		GoodsData goodsData = Global.GetRolePaiPets(false).Find((GoodsData s) => s.Id == DBID);
		if (goodsData == null)
		{
			return;
		}
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(goodsData.GoodsID);
		if (goodsXmlNodeByID != null)
		{
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), "NetImages/GameRes/");
			string backSpriteName = "bagGrid4_bak";
			GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BackSpriteName0 = backSpriteName;
			ggoodIcon.BodyURL = new ImageURL(goodsImageURLFromIconCode, false, 0);
			ggoodIcon.TipType = 1;
			ggoodIcon.Tip = StringUtil.substitute("{0},{1},{2},{3}", new object[]
			{
				goodsData.GoodsID,
				0,
				-1,
				-1
			});
			ggoodIcon.ItemCode = goodsData.GoodsID;
			ggoodIcon.ItemObject = goodsData;
			ggoodIcon.BoxTypes = 5;
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextColor = 16777215U;
			ggoodIcon.DisableTextColor = 8421504U;
			ggoodIcon.STextVisibility = false;
			ggoodIcon.petLevel.text = "Lv" + this.JingLingLevel.ToString();
			ggoodIcon.petLevel.transform.localPosition = new Vector3(ggoodIcon.petLevel.transform.localPosition.x, ggoodIcon.petLevel.transform.localPosition.y, -40f);
			ggoodIcon.SecondText.gameObject.SetActive(false);
			ggoodIcon.GoodImg.transform.localPosition = new Vector3(0f, 0f, -1.5f);
			ggoodIcon.BindingSprite.transform.localPosition = new Vector3(-24f, -24f, -4f);
			bool canUse = Global.CanUseGoods(goodsData.GoodsID, false, true);
			Super.InitGoodsGIcon(ggoodIcon, goodsData, canUse, IconTextTypes.Qianghua);
			ggoodIcon.TextShadowColor = 4278190080U;
			ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
			ggoodIcon.TextVerticalAlignment = global::Layout.Top;
			ggoodIcon.gameObject.transform.parent = this.mJingLingParent;
			ggoodIcon.gameObject.transform.localPosition = this.mImgIcon.transform.localPosition;
			ggoodIcon.gameObject.transform.localScale = Vector3.one;
			ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
			UIPanel component = ggoodIcon.GetComponent<UIPanel>();
			if (component)
			{
				Object.Destroy(component);
			}
		}
	}

	private void SetBtnStatus(GButton btn, bool isActivity)
	{
		if (null == btn)
		{
			return;
		}
		btn.GetComponent<BoxCollider>().enabled = isActivity;
		btn.GetComponentInChildren<UISprite>().color = ((!isActivity) ? Color.gray : Color.white);
	}

	private void SaveJingLingDBIdToDict(int key, int value)
	{
		if (this.jingLingZhenRongdict.ContainsKey(this.mJingLingKey))
		{
			int lastSelectDBId = this.jingLingZhenRongdict[this.mJingLingKey];
			this.CheXiaoHasShangZhenJingLing(lastSelectDBId);
			this.jingLingZhenRongdict[this.mJingLingKey] = this.mCurrentJingLingDBId;
		}
		else
		{
			this.jingLingZhenRongdict.Add(this.mJingLingKey, this.mCurrentJingLingDBId);
		}
	}

	private void CheXiaoHasShangZhenJingLing(int lastSelectDBId)
	{
		if (lastSelectDBId > 0 && this.mListBox.transform.childCount > 0)
		{
			for (int i = 0; i < this.mListBox.transform.childCount; i++)
			{
				YaoSaiPaiZhuJingLingItem component = this.mListBox.transform.GetChild(i).GetComponent<YaoSaiPaiZhuJingLingItem>();
				if (component != null && component.mJingLingData.Id == lastSelectDBId)
				{
					component.CheXiao();
				}
			}
		}
	}

	private void ReSaveJingLingZhenRongToDict(string jingLingZhenRongstr)
	{
		if (string.IsNullOrEmpty(jingLingZhenRongstr))
		{
			return;
		}
		string[] array = jingLingZhenRongstr.Split(new char[]
		{
			'|'
		});
		for (int i = 0; i < array.Length; i++)
		{
			int num = ConvertExt.SafeConvertToInt32(array[i]);
			if (this.jingLingZhenRongdict.ContainsKey(i))
			{
				this.jingLingZhenRongdict[i] = num;
			}
			else
			{
				this.jingLingZhenRongdict.Add(i, num);
			}
		}
	}

	private void ReplaceDBIdInJingLingZhenRongDict(int jingLingDBId, bool isReplace)
	{
		if (!isReplace)
		{
			return;
		}
		int num = -1;
		foreach (KeyValuePair<int, int> keyValuePair in this.jingLingZhenRongdict)
		{
			if (keyValuePair.Value == jingLingDBId)
			{
				Dictionary<int, int>.Enumerator enumerator;
				KeyValuePair<int, int> keyValuePair2 = enumerator.Current;
				num = keyValuePair2.Key;
				break;
			}
		}
		if (isReplace && num >= 0)
		{
			this.jingLingZhenRongdict[num] = 0;
		}
	}

	private string GetJingLingStrID()
	{
		if (this.jingLingZhenRongdict == null || this.jingLingZhenRongdict.Count <= 0)
		{
			return "0|0|0";
		}
		StringBuilder stringBuilder = new StringBuilder();
		Dictionary<int, int>.Enumerator enumerator = this.jingLingZhenRongdict.GetEnumerator();
		while (enumerator.MoveNext())
		{
			StringBuilder stringBuilder2 = stringBuilder;
			KeyValuePair<int, int> keyValuePair = enumerator.Current;
			stringBuilder2.Append(keyValuePair.Value);
			stringBuilder.Append("|");
		}
		if (stringBuilder.Length > 0)
		{
			return stringBuilder.ToString().TrimEnd(new char[]
			{
				'|'
			});
		}
		return stringBuilder.ToString();
	}

	private void ClearRightDescribe()
	{
		if (this.mJingLingParent != null)
		{
			int childCount = this.mJingLingParent.childCount;
			if (childCount > 0)
			{
				for (int i = 0; i < childCount; i++)
				{
					Object.Destroy(this.mJingLingParent.GetChild(i).gameObject);
				}
			}
		}
		this.IsShowJingLingContent = false;
		NGUITools.SetActive(this.mBtnChange.gameObject, false);
		this.JingLingIconById = 0;
	}

	protected override void OnDestroy()
	{
	}

	public DPSelectedItemEventHandler CloseHandler;

	public DPSelectedItemEventHandler SelectJingLingHandler;

	public GButton mBtnClose;

	public ListBox mListBox;

	private ObservableCollection _ItemCollection;

	public TextBlock mLblName;

	public TextBlock mLblLevel;

	public ShowNetImage mImgIcon;

	public TextBlock mLblZhuoYueValue;

	public TextBlock mLblDamageValue;

	public UISprite mImgArrowUp;

	public GButton mBtnChange;

	public UIScrollBar mScrollBar;

	public Transform mJingLingParent;

	public GameObject mJingLingContent;

	private int mCurrentJingLingDBId;

	private YaoSaiPaiZhuJingLingItem mCurrentYaoSaiPaiZhuJingLingItem;

	private YaoSaiPaiZhuJingLingItem mLastYaoSaiPaiZhuJingLingItem;

	private YaoSaiPaiZhuJingLingItem mCacheYaoSaiPaiZhuJingLingItem;

	public TextBlock mLblTips;

	private Dictionary<int, int> jingLingZhenRongdict = new Dictionary<int, int>();

	private List<YaoSaiPaiZhuJingLingItem> lastClickItems = new List<YaoSaiPaiZhuJingLingItem>();
}
