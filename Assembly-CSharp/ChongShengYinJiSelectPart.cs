using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongShengYinJiSelectPart : UserControl
{
	public string ImgDetails
	{
		set
		{
			this.mImgDeatils.URL = value;
		}
	}

	public string LblDetails
	{
		set
		{
			this.mLblDetails.Text = value;
		}
	}

	private int mHasResetCount { get; set; }

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.ResetCfgNum = ConfigSystemParam.GetSystemParamIntArrayByName("ChongShengYinJiChongZhi", ',');
		this.describes = ConfigSystemParam.GetSystemParamStringArrayByName("ChongShengYinJiMiaoShu", '|');
		this.IsShowSelectPanel = false;
		this.mDictID.Add(this.Left, default(ChongShengYinJiSelectPart.SelectData));
		this.mDictID.Add(this.Right, default(ChongShengYinJiSelectPart.SelectData));
	}

	private void InitTextInPrefabs()
	{
		this.mBtnSelectAndResetSingle.Text = Global.GetLang("选择");
		this.mLblTitle.Text = Global.GetLang("印记介绍");
	}

	private void InitEvent()
	{
		this.mBtnClickSelectPanel.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBtnSelectOrGiveUp);
		this.mBtnConfirmAndResetAll.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnBtnConfirmAndResetAll);
		this.mBtnCloseSelectPanel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.IsShowSelectPanel = false;
		};
		UIEventListener.Get(this.mTextureLeft.gameObject).onClick = delegate(GameObject s)
		{
			if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.ResetAll)
			{
				return;
			}
			ChongShengYinJiSelectPart.SelectData selectData = this.mDictID[this.Left];
			if (selectData.ID > 0)
			{
				this.mCurrentYinJiID = selectData.ID;
				this.OpenSelectPanel();
			}
		};
		UIEventListener.Get(this.mTextureRight.gameObject).onClick = delegate(GameObject s)
		{
			if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.ResetAll)
			{
				return;
			}
			ChongShengYinJiSelectPart.SelectData selectData = this.mDictID[this.Right];
			if (selectData.ID > 0)
			{
				this.mCurrentYinJiID = selectData.ID;
				this.OpenSelectPanel();
			}
		};
		UIEventListener.Get(this.mSprtSelectBg.gameObject).onClick = delegate(GameObject s)
		{
			this.IsShowSelectPanel = false;
		};
	}

	public void InitValue(RebornStampData data)
	{
		string yinJiIds = string.Empty;
		if (data != null && data.StampInfo != null && data.StampInfo.Count > 0)
		{
			this.mHasResetCount = data.ResetNum;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(data.StampInfo[0]);
			stringBuilder.Append(',');
			stringBuilder.Append(data.StampInfo[8]);
			yinJiIds = stringBuilder.ToString();
		}
		this.InitItems();
		this.InitSelectYinJi(yinJiIds);
	}

	private void InitItems()
	{
		for (int i = 0; i < this.MaxShowYinJi; i++)
		{
			ChongShengYinJiSelectPartItem chongShengYinJiSelectPartItem = U3DUtils.NEW<ChongShengYinJiSelectPartItem>();
			chongShengYinJiSelectPartItem.transform.SetParent(this.ItemsRoot[i]);
			chongShengYinJiSelectPartItem.transform.localPosition = Vector3.zero;
			chongShengYinJiSelectPartItem.transform.localScale = Vector3.one;
			chongShengYinJiSelectPartItem.InitValue(i + 1);
			chongShengYinJiSelectPartItem.CancelSelect = true;
			chongShengYinJiSelectPartItem.ClickHandler = new DPSelectedItemEventHandler(this.OnClickHandler);
			this.ItemsCache.Add(chongShengYinJiSelectPartItem);
		}
	}

	private void InitSelectYinJi(string yinJiIds)
	{
		if (!string.IsNullOrEmpty(yinJiIds))
		{
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.ResetAll;
		}
		else
		{
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.None;
		}
		if (string.IsNullOrEmpty(yinJiIds))
		{
			return;
		}
		this.SetAllSelectedEffect(yinJiIds);
	}

	private void SetSelectYinJi()
	{
	}

	private void OnBtnSelectOrGiveUp(object sender, MouseEvent e)
	{
		if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.Select)
		{
			if (this.IsFull)
			{
				Super.HintMainText(Global.GetLang("印记已满"), 10, 3);
				return;
			}
			this.mCacheID.Add(this.mCurrentYinJiID);
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.Reset;
			this.SetDict(true, this.GetSelectData(this.mCurrentYinJiID));
			this.SetSingleSelectedEffect();
		}
		else if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.Reset)
		{
			this.mCacheID.Remove(this.mCurrentYinJiID);
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.Select;
			this.SetDict(false, this.GetSelectData(this.mCurrentYinJiID));
			this.CancelSingleSelectedEffect();
		}
		else if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.None)
		{
			Super.HintMainText(Global.GetLang("请选择印记"), 10, 3);
		}
		this.IsShowSelectPanel = false;
	}

	private void OnBtnConfirmAndResetAll(object sender, MouseEvent e)
	{
		if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.ResetAll)
		{
			int num = this.ResetCfgNum[(this.mHasResetCount < this.ResetCfgNum.Length) ? this.mHasResetCount : (this.ResetCfgNum.Length - 1)];
			string message = string.Format(Global.GetLang("是否确定消耗{0}钻石，重置印记以及印记点数吗？"), num);
			GChildWindow messageBoxWindow = Super.ShowMessageBoxByPosition(Super.MainWindowRoot, 1, Global.GetLang("提示"), message, new Vector3(-150f, 17f, -0.01f), new Vector3(-73f, -55f, -0.01f), new Vector3(76.5f, -55f, -0.01f), Global.GetLang("确定"), Global.GetLang("取消"), 316, default(Vector3), null);
			messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
			{
				int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
				Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
				if (messageBoxReturn == 0)
				{
					GameInstance.Game.SendResetYinJi();
				}
				return true;
			};
		}
		else if (this.IsFull)
		{
			GameInstance.Game.SendSetYinJi(this.mCacheID[0], this.mCacheID[1]);
		}
		else
		{
			Super.HintMainText(Global.GetLang("需选择2个印记"), 10, 3);
		}
	}

	private void OnClickHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (this.mCurrentSelect == ChongShengYinJiSelectPart.SelectType.ResetAll)
		{
			return;
		}
		this.mCurrentYinJiID = args.ID;
		this.OpenSelectPanel();
	}

	public string ImgLeft
	{
		get
		{
			return this.imgLeft;
		}
		set
		{
			this.imgLeft = value;
			this.mTextureLeft.URL = this.imgLeft;
		}
	}

	public string ImgRight
	{
		get
		{
			return this.imgRight;
		}
		set
		{
			this.imgRight = value;
			this.mTextureRight.URL = this.imgRight;
		}
	}

	private string TexturePath(string name)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("NetImages/GameRes/Images/ChongShengYinJi/");
		stringBuilder.Append(name);
		return stringBuilder.ToString();
	}

	private string LblContentSelectPanel
	{
		set
		{
			this.mLblContent.Text = this.GetContent(value);
		}
	}

	private string GetContent(string sss)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = sss.Split(new char[]
		{
			'@'
		});
		if (array.Length >= 2)
		{
			stringBuilder.Append(array[0]);
			stringBuilder.Append(Environment.NewLine);
			stringBuilder.Append(array[1]);
		}
		return stringBuilder.ToString();
	}

	private bool IsShowSelectPanel
	{
		set
		{
			NGUITools.SetActive(this.mSelectPanelObj, value);
		}
	}

	public void RespondResetAll(int result)
	{
		if (result == 1)
		{
			Global.Data.roleData.RebornYinJi = null;
			ChongShengYinJiData.ResetChongShengYinJiEffect();
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.None;
			this.CancelAllSelectedEffect();
			this.mDictID[this.Left] = default(ChongShengYinJiSelectPart.SelectData);
			this.mDictID[this.Right] = default(ChongShengYinJiSelectPart.SelectData);
			this.mCacheID.Clear();
			if (this.mDictID.Count > 0)
			{
				Dictionary<int, ChongShengYinJiSelectPart.SelectData>.Enumerator enumerator = this.mDictID.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					num++;
					if (num == 1)
					{
						KeyValuePair<int, ChongShengYinJiSelectPart.SelectData> keyValuePair = enumerator.Current;
						this.ImgLeft = keyValuePair.Value.Path;
					}
					else if (num == 2)
					{
						KeyValuePair<int, ChongShengYinJiSelectPart.SelectData> keyValuePair2 = enumerator.Current;
						this.ImgRight = keyValuePair2.Value.Path;
					}
				}
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(ChongShengYinJiData.GetErrorMsg(result)), 10, 3);
		}
	}

	private bool IsFull
	{
		get
		{
			return this.mCacheID.Count >= this.MaxSelect;
		}
	}

	private ChongShengYinJiSelectPart.SelectType mCurrentSelect
	{
		get
		{
			return this.currentSelect;
		}
		set
		{
			this.currentSelect = value;
			switch (this.currentSelect)
			{
			case ChongShengYinJiSelectPart.SelectType.None:
				this.mBtnClickSelectPanel.Text = Global.GetLang("放入");
				this.mBtnConfirmAndResetAll.Text = Global.GetLang("确认");
				break;
			case ChongShengYinJiSelectPart.SelectType.Select:
				this.mBtnClickSelectPanel.Text = Global.GetLang("放入");
				break;
			case ChongShengYinJiSelectPart.SelectType.Reset:
				this.mBtnClickSelectPanel.Text = Global.GetLang("卸下");
				break;
			case ChongShengYinJiSelectPart.SelectType.ResetAll:
				NGUITools.SetActive(this.mBtnSelectAndResetSingle.gameObject, false);
				this.mBtnConfirmAndResetAll.Text = Global.GetLang("重置所有");
				break;
			}
		}
	}

	private void SetAllSelectedEffect(string yinJiIds)
	{
		string[] array = yinJiIds.Split(new char[]
		{
			','
		});
		int num = Global.SafeConvertToInt32(array[0]);
		int num2 = Global.SafeConvertToInt32(array[1]);
		this.mCacheID.Add(num);
		this.mCacheID.Add(num2);
		this.SetDict(true, this.GetSelectData(num));
		this.SetDict(true, this.GetSelectData(num2));
		for (int i = 0; i < this.ItemsCache.Count; i++)
		{
			if (this.ItemsCache[i].IDType == num || this.ItemsCache[i].IDType == num2)
			{
				this.ItemsCache[i].Selected = true;
			}
		}
	}

	private void CancelAllSelectedEffect()
	{
		for (int i = 0; i < this.ItemsCache.Count; i++)
		{
			this.ItemsCache[i].CancelSelect = true;
		}
	}

	private void SetSingleSelectedEffect()
	{
		for (int i = 0; i < this.ItemsCache.Count; i++)
		{
			if (this.ItemsCache[i].IDType == this.mCurrentYinJiID)
			{
				this.ItemsCache[i].Selected = true;
				break;
			}
		}
	}

	private void CancelSingleSelectedEffect()
	{
		for (int i = 0; i < this.ItemsCache.Count; i++)
		{
			if (this.ItemsCache[i].IDType == this.mCurrentYinJiID)
			{
				this.ItemsCache[i].CancelSelect = true;
				break;
			}
		}
	}

	private ChongShengYinJiSelectPart.SelectData GetSelectData(int id)
	{
		if (id <= 0)
		{
			return default(ChongShengYinJiSelectPart.SelectData);
		}
		return new ChongShengYinJiSelectPart.SelectData
		{
			ID = id,
			Path = this.TexturePath(this.GetYinJiName(id))
		};
	}

	private void SetDict(bool isAdd, ChongShengYinJiSelectPart.SelectData data)
	{
		if (isAdd)
		{
			ChongShengYinJiSelectPart.SelectData selectData = this.mDictID[this.Left];
			ChongShengYinJiSelectPart.SelectData selectData2 = this.mDictID[this.Right];
			if (selectData.ID <= 0 || string.IsNullOrEmpty(selectData.Path))
			{
				this.mDictID[this.Left] = data;
			}
			else if (selectData2.ID <= 0 || string.IsNullOrEmpty(selectData2.Path))
			{
				this.mDictID[this.Right] = data;
			}
		}
		else
		{
			ChongShengYinJiSelectPart.SelectData selectData3 = this.mDictID[this.Left];
			ChongShengYinJiSelectPart.SelectData selectData4 = this.mDictID[this.Right];
			if (selectData3.ID > 0 || selectData4.ID > 0)
			{
				if (selectData3.ID == data.ID)
				{
					ChongShengYinJiSelectPart.SelectData selectData5 = default(ChongShengYinJiSelectPart.SelectData);
					this.mDictID[this.Left] = selectData5;
				}
				else if (selectData4.ID == data.ID)
				{
					ChongShengYinJiSelectPart.SelectData selectData6 = default(ChongShengYinJiSelectPart.SelectData);
					this.mDictID[this.Right] = selectData6;
				}
			}
			else
			{
				ChongShengYinJiSelectPart.SelectData selectData7 = default(ChongShengYinJiSelectPart.SelectData);
				this.mDictID[this.Left] = selectData7;
				ChongShengYinJiSelectPart.SelectData selectData8 = default(ChongShengYinJiSelectPart.SelectData);
				this.mDictID[this.Right] = selectData8;
			}
		}
		if (this.mDictID.Count > 0)
		{
			Dictionary<int, ChongShengYinJiSelectPart.SelectData>.Enumerator enumerator = this.mDictID.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				num++;
				if (num == 1)
				{
					KeyValuePair<int, ChongShengYinJiSelectPart.SelectData> keyValuePair = enumerator.Current;
					this.ImgLeft = keyValuePair.Value.Path;
				}
				else if (num == 2)
				{
					KeyValuePair<int, ChongShengYinJiSelectPart.SelectData> keyValuePair2 = enumerator.Current;
					this.ImgRight = keyValuePair2.Value.Path;
				}
			}
		}
	}

	private void OpenSelectPanel()
	{
		if (this.mCacheID.Contains(this.mCurrentYinJiID))
		{
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.Reset;
		}
		else
		{
			this.mCurrentSelect = ChongShengYinJiSelectPart.SelectType.Select;
		}
		this.IsShowSelectPanel = true;
		this.LblContentSelectPanel = this.GetYinJiDes(this.mCurrentYinJiID);
	}

	private string GetYinJiDes(int index)
	{
		string result = string.Empty;
		if (this.describes.Length >= 5)
		{
			result = this.describes[index - 1];
		}
		return result;
	}

	private string GetYinJiName(int index)
	{
		return index.ToString() + ".png.qj";
	}

	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
	}

	protected override void OnDestroy()
	{
		this.mDictID.Clear();
		this.ItemsCache.Clear();
		this.mCacheID.Clear();
		base.OnDestroy();
	}

	public DPSelectedItemEventHandler ChangeHandler;

	public TextBlock mLblTitle;

	public Transform[] ItemsRoot;

	public GButton mBtnSelectAndResetSingle;

	public GButton mBtnConfirmAndResetAll;

	public ShowNetImage mImgDeatils;

	public TextBlock mLblCount;

	public TextBlock mLblDetails;

	public ShowNetImage mTextureLeft;

	public ShowNetImage mTextureRight;

	private List<ChongShengYinJiSelectPartItem> ItemsCache = new List<ChongShengYinJiSelectPartItem>();

	private ChongShengYinJiSelectPart.SelectType currentSelect = ChongShengYinJiSelectPart.SelectType.Select;

	private int MaxShowYinJi = 5;

	private int MaxSelect = 2;

	private int[] ResetCfgNum;

	private List<int> mCacheID = new List<int>();

	private Dictionary<int, ChongShengYinJiSelectPart.SelectData> mDictID = new Dictionary<int, ChongShengYinJiSelectPart.SelectData>();

	private int Left;

	private int Right = 1;

	public UISprite mSprtSelectBg;

	public GameObject mSelectPanelObj;

	public GButton mBtnCloseSelectPanel;

	public GButton mBtnClickSelectPanel;

	public TextBlock mLblContent;

	private string[] ZhuYinJiSpriteNames = new string[]
	{
		"ShenShengYinJi",
		"AnYingYinJi",
		"ZiRanYinJi",
		"HunDunYinJi",
		"MengYanYinJi"
	};

	private string[] describes;

	private int mCurrentYinJiID = -1;

	private string imgLeft = string.Empty;

	private string imgRight = string.Empty;

	private enum SelectType
	{
		None,
		Select,
		Reset,
		ResetAll
	}

	[StructLayout(0, Size = 1)]
	private struct SelectData
	{
		public int ID { get; set; }

		public string Path { get; set; }
	}
}
