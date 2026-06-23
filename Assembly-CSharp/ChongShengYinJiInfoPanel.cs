using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChongShengYinJiInfoPanel : UserControl
{
	private int CurrentLevelTitle
	{
		set
		{
			if (value == -1)
			{
				this.mLblCurrentTitle.Text = Global.GetLang("[当前等级：MAX]");
			}
			else
			{
				this.mLblCurrentTitle.Text = Global.GetString(new object[]
				{
					Global.GetLang("[当前等级：Lv"),
					value,
					"]"
				});
			}
		}
	}

	private int NextLevelTitle
	{
		set
		{
			if (value == -1)
			{
				this.CurrentLevelTitle = -1;
				NGUITools.SetActive(this.mLblNextTitle.gameObject, false);
				NGUITools.SetActive(this.mSprtNextBg.gameObject, false);
				NGUITools.SetActive(this.mLblNextInfo.gameObject, false);
			}
			else
			{
				this.mLblNextTitle.Text = Global.GetString(new object[]
				{
					Global.GetLang("[下一等级：Lv"),
					value,
					"]"
				});
			}
		}
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
	}

	private void InitTextInPrefabs()
	{
		this.mBtnConfirm.Text = Global.GetLang("确  定");
		this.mlblTitle.Text = Global.GetLang("印记属性");
	}

	private void InitEvent()
	{
		this.mBtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			base.gameObject.SetActive(false);
		};
		this.mBtnConfirm.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (!this.mYinJiData.IsFullLevel)
			{
				if (this.Count <= 0)
				{
					Super.HintMainText(Global.GetLang("请先选择需要增加的印记点数"), 10, 3);
					return;
				}
				GameInstance.Game.SendYinJiLevelUp(this.mYinJiData.ID, this.mYinJiData.Type, this.Count);
			}
			else
			{
				Super.HintMainText(Global.GetLang("已达最大级数"), 10, 3);
			}
		};
		this.mBtnAdd.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.mIsFullLevel)
			{
				Super.HintMainText(Global.GetLang("已达最大等级"), 10, 3);
				return;
			}
			if (this.LeftYinJiCount <= 0L)
			{
				Super.HintMainText(Global.GetLang("印记点不足"), 10, 3);
				return;
			}
			this.mChildID += this.NumberKeyboard;
			this.NumberKeyboard = 0;
			if ((long)this.Count >= this.LeftYinJiCount)
			{
				Super.HintMainText(Global.GetLang("印记点不足"), 10, 3);
				return;
			}
			this.Count++;
			this.mChildID++;
			if (this.IsMaxZiYinJi(this.mChildID))
			{
				this.Count--;
				this.mChildID--;
				Super.HintMainText(Global.GetLang("已达最大等级"), 10, 3);
				return;
			}
			this.mLblNextInfo.Text = this.GetNextYinJiInfo(this.mChildID, out this.mIsFullLevel);
		};
		this.mBtnSub.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.Count <= 0)
			{
				return;
			}
			this.Count--;
			this.mChildID += this.NumberKeyboard;
			this.NumberKeyboard = 0;
			if (this.Count <= 0)
			{
				this.mLblNextInfo.Text = this.GetNextYinJiInfo(this.mChildID, out this.mIsFullLevel);
				this.mChildID--;
				return;
			}
			this.mChildID--;
			this.mLblNextInfo.Text = this.GetNextYinJiInfo(this.mChildID, out this.mIsFullLevel);
		};
		UIEventListener.Get(this.mInputObj).onClick = delegate(GameObject s)
		{
			PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(new DPSelectedItemEventHandler(this.NumberKeyboardCallBack), this.mLblYinJiCount.Label, 0, -100);
		};
		UIEventListener.Get(this.mBak.gameObject).onClick = delegate(GameObject s)
		{
			base.gameObject.SetActive(false);
		};
	}

	private void NumberKeyboardCallBack(object sender, DPSelectedItemEventArgs args)
	{
		int num = args.ID;
		int ziMaxID = this.GetZiMaxID(this.mChildType);
		this.mChildID = this.mCurrentChildID;
		if (this.mChildID + num > ziMaxID)
		{
			num = ziMaxID - this.mChildID;
		}
		this.NumberKeyboard = num;
		this.Count = num;
		string nextYinJiInfo = this.GetNextYinJiInfo(this.mChildID + num, out this.mIsFullLevel);
		if (this.mIsFullLevel)
		{
			Super.HintMainText(Global.GetLang("已超出最大等级"), 10, 3);
		}
		else
		{
			this.mLblNextInfo.Text = nextYinJiInfo;
		}
	}

	public void InitValue(YinJiData yinji, long yinjiCount)
	{
		this.Count = 0;
		this.NumberKeyboard = 0;
		this.mIsFullLevel = false;
		this.LeftYinJiCount = yinjiCount;
		this.mYinJiData = yinji;
		this.mLblTips.transform.localPosition = new Vector3(this.mLblTips.transform.localPosition.x, -22f, this.mLblTips.transform.localPosition.z);
		this.mLblNextInfo.transform.localPosition = new Vector3(this.mLblNextInfo.transform.localPosition.x, -48f, this.mLblNextInfo.transform.localPosition.z);
		if (this.mYinJiData.IsMainYinJi)
		{
			this.mBak.transform.localScale = this.ZhuYinJiScale;
			this.AddObj.SetActive(false);
			if (this.mYinJiData.IsFullLevel)
			{
				this.CurrentLevelTitle = -1;
			}
			else
			{
				this.CurrentLevelTitle = yinji.Level;
			}
			NGUITools.SetActive(this.mLblTips.gameObject, !this.mYinJiData.IsFullLevel);
			this.mLblCurrentInfo.Text = ChongShengYinJiData.GetShuXingStr(ChongShengYinJiData.GetZhuYinJiCfgByIdAndType(this.mYinJiData.ID, this.mYinJiData.Type).ShuXing, this.colorStr);
			ZhuYinJiCfgData zhuYinJiCfgByIdAndType = ChongShengYinJiData.GetZhuYinJiCfgByIdAndType(this.mYinJiData.ID + 1, this.mYinJiData.Type);
			if (zhuYinJiCfgByIdAndType != null)
			{
				NGUITools.SetActive(this.mLblNextTitle.gameObject, true);
				NGUITools.SetActive(this.mSprtNextBg.gameObject, true);
				NGUITools.SetActive(this.mLblNextInfo.gameObject, true);
				this.NextLevelTitle = zhuYinJiCfgByIdAndType.Level;
				this.mLblNextInfo.Text = ChongShengYinJiData.GetShuXingStr(zhuYinJiCfgByIdAndType.ShuXing, this.colorStr);
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(Global.GetLang("(所有子印记等级达到"));
				stringBuilder.Append(zhuYinJiCfgByIdAndType.NeedLevel);
				stringBuilder.Append(Global.GetLang("级)"));
				this.mLblTips.Text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					stringBuilder
				});
			}
			else
			{
				this.NextLevelTitle = -1;
			}
		}
		else
		{
			this.mBak.transform.localScale = this.ZiYinJiScale;
			this.mCurrentChildID = this.mYinJiData.ID;
			this.mChildID = this.mYinJiData.ID;
			this.mChildType = this.mYinJiData.Type;
			this.mLblTips.Text = string.Empty;
			this.mLblNextInfo.transform.localPosition = new Vector3(this.mLblNextInfo.transform.localPosition.x, -22f, this.mLblNextInfo.transform.localPosition.z);
			this.CurrentLevelTitle = yinji.Level;
			this.AddObj.SetActive(true);
			this.mLblCurrentInfo.Text = ChongShengYinJiData.GetShuXingStr(ChongShengYinJiData.GetZiYinJiCfgByIdAndType(this.mYinJiData.ID, this.mYinJiData.Type).ShuXing, this.colorStr);
			ZiYinJiCfgData ziYinJiCfgByIdAndType = ChongShengYinJiData.GetZiYinJiCfgByIdAndType(this.mYinJiData.ID + 1, this.mYinJiData.Type);
			if (ziYinJiCfgByIdAndType != null)
			{
				NGUITools.SetActive(this.mLblNextTitle.gameObject, true);
				NGUITools.SetActive(this.mSprtNextBg.gameObject, true);
				NGUITools.SetActive(this.mLblNextInfo.gameObject, true);
				this.NextLevelTitle = ziYinJiCfgByIdAndType.Level;
				this.mLblNextInfo.Text = ChongShengYinJiData.GetShuXingStr(ziYinJiCfgByIdAndType.ShuXing, this.colorStr);
			}
			else
			{
				this.NextLevelTitle = -1;
			}
		}
	}

	public new int Count
	{
		get
		{
			return this.mCount;
		}
		set
		{
			this.mCount = value;
			if (this.mCount <= 0)
			{
				this.mCount = 0;
			}
			this.mLblYinJiCount.Text = this.mCount.ToString();
		}
	}

	private bool IsMaxZiYinJi(int id)
	{
		ZiYinJiCfgData ziYinJiCfgByIdAndType = ChongShengYinJiData.GetZiYinJiCfgByIdAndType(id, this.mChildType);
		return ziYinJiCfgByIdAndType == null;
	}

	private string GetNextYinJiInfo(int id, out bool isFullLevel)
	{
		isFullLevel = false;
		ZiYinJiCfgData ziYinJiCfgByIdAndType = ChongShengYinJiData.GetZiYinJiCfgByIdAndType(id, this.mChildType);
		if (ziYinJiCfgByIdAndType != null)
		{
			isFullLevel = false;
			this.NextLevelTitle = ziYinJiCfgByIdAndType.Level;
			return ChongShengYinJiData.GetShuXingStr(ziYinJiCfgByIdAndType.ShuXing, this.colorStr);
		}
		isFullLevel = true;
		return Global.GetLang("无");
	}

	private int GetZiMaxLevel(int type)
	{
		Dictionary<int, ZiYinJiCfgData> dictZiYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZiYinJiCfgData();
		Dictionary<int, ZiYinJiCfgData>.Enumerator enumerator = dictZiYinJiCfgData.GetEnumerator();
		int num = 0;
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, ZiYinJiCfgData> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.Type == type)
			{
				num++;
			}
		}
		return num - 1;
	}

	private int GetZiMaxID(int type)
	{
		Dictionary<int, ZiYinJiCfgData> dictZiYinJiCfgData = ChongShengYinJiConfigManager.GetInstance().GetDictZiYinJiCfgData();
		Dictionary<int, ZiYinJiCfgData>.Enumerator enumerator = dictZiYinJiCfgData.GetEnumerator();
		List<ZiYinJiCfgData> list = new List<ZiYinJiCfgData>();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, ZiYinJiCfgData> keyValuePair = enumerator.Current;
			if (keyValuePair.Value.Type == type)
			{
				List<ZiYinJiCfgData> list2 = list;
				KeyValuePair<int, ZiYinJiCfgData> keyValuePair2 = enumerator.Current;
				list2.Add(keyValuePair2.Value);
			}
		}
		list.Sort((ZiYinJiCfgData x, ZiYinJiCfgData y) => x.ID - y.ID);
		return list[list.Count - 1].ID;
	}

	public Action<int> OnClickHandler;

	public GButton mBtnClose;

	public GButton mBtnAdd;

	public TextBlock mLblYinJiCount;

	public GButton mBtnSub;

	public GButton mBtnConfirm;

	public TextBlock mlblTitle;

	public TextBlock mLblCurrentTitle;

	public TextBlock mLblCurrentInfo;

	public TextBlock mLblTips;

	public TextBlock mLblNextTitle;

	public TextBlock mLblNextInfo;

	public UISprite mBak;

	public GameObject mInputObj;

	public GameObject AddObj;

	private YinJiData mYinJiData;

	private int mCount;

	private long LeftYinJiCount;

	private int mChildID;

	private int mCurrentChildID;

	private int mChildType;

	private bool mIsFullLevel;

	private Vector3 ZhuYinJiScale = new Vector3(374f, 348f, 1f);

	private Vector3 ZiYinJiScale = new Vector3(374f, 398f, 1f);

	public UISprite mSprtNextBg;

	private int NumberKeyboard;

	private string colorStr = "dac7ae";
}
