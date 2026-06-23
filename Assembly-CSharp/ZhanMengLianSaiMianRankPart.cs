using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiMianRankPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		if (null != this.mZhanMengLianSaiMainRankItem)
		{
			this.mZhanMengLianSaiMainRankItem.gameObject.SetActive(false);
			if (null != this.mZhanMengLianSaiMainRankItem.GetComponent<BoxCollider>())
			{
				Object.Destroy(this.mZhanMengLianSaiMainRankItem.GetComponent<BoxCollider>());
			}
		}
	}

	private string[] GetTitleStr(BangHuiMatchRankType type)
	{
		string[] array = new string[]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};
		switch (type)
		{
		case 8:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排名")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟名")
			});
			array[2] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("首领")
			});
			array[3] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("胜利场次")
			});
			break;
		case 9:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排名")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟名")
			});
			array[2] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("首领")
			});
			array[3] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排行积分")
			});
			break;
		case 10:
		case 11:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排名")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("玩家名")
			});
			array[2] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟名")
			});
			array[3] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("MVP次数")
			});
			break;
		case 12:
		case 13:
			array[0] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排名")
			});
			array[1] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("玩家名")
			});
			array[2] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟名")
			});
			array[3] = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("击杀人数")
			});
			break;
		}
		return array;
	}

	private string GetValueMOnad(BangHuiMatchRankType type, int value)
	{
		string result = value.ToString();
		switch (type)
		{
		case 12:
		case 13:
			if (9999 < value)
			{
				try
				{
					double num = (double)value / 10000.0;
					if (0 < num.ToString("f1").Split(new char[]
					{
						'.'
					})[1].SafeToInt32(0))
					{
						result = num.ToString("f1") + "w";
					}
					else
					{
						result = num.ToString("f0") + "w";
					}
				}
				catch (Exception ex)
				{
				}
			}
			break;
		}
		return result;
	}

	private void RefreshMatchRankTyp()
	{
		int num = 0;
		int num2 = 0;
		BangHuiMatchRankType bangHuiMatchRankType = this.mBangHuiMatchRankType;
		try
		{
			for (int i = 0; i < this.mBtnTopHanders.Length; i++)
			{
				if (this.mBtnTopHanders[i].BSelect)
				{
					num = this.mBtnTopHanders[i].ID;
					break;
				}
			}
		}
		catch (Exception ex)
		{
		}
		try
		{
			for (int j = 0; j < this.mBtnSelectHanders.Length; j++)
			{
				if (this.mBtnSelectHanders[j].BSelect)
				{
					num2 = this.mBtnSelectHanders[j].ID;
					break;
				}
			}
		}
		catch (Exception ex2)
		{
		}
		if (num2 == 2)
		{
			if (num == 1)
			{
				this.mBangHuiMatchRankType = 9;
			}
			else if (num == 2)
			{
				this.mBangHuiMatchRankType = 11;
			}
			else
			{
				this.mBangHuiMatchRankType = 13;
			}
			this.mSpTitle.spriteName = "NewLianSaiTitle01";
			this.mSpTitle.transform.localScale = new Vector3(120f, 27f, 0f);
		}
		else
		{
			if (num == 1)
			{
				this.mBangHuiMatchRankType = 8;
			}
			else if (num == 2)
			{
				this.mBangHuiMatchRankType = 10;
			}
			else
			{
				this.mBangHuiMatchRankType = 12;
			}
			this.mSpTitle.spriteName = "SuperLianSaiTitle01";
			this.mSpTitle.transform.localScale = new Vector3(143f, 28f, 0f);
		}
		if (bangHuiMatchRankType != this.mBangHuiMatchRankType && this.Hander != null)
		{
			this.Hander(this, new DPSelectedItemEventArgs
			{
				ID = this.mBangHuiMatchRankType,
				Type = 1
			});
		}
		string[] titleStr = this.GetTitleStr(this.mBangHuiMatchRankType);
		this.mLabelRankTipTitle1.text = titleStr[0];
		this.mLabelRankTipTitle2.text = titleStr[1];
		this.mLabelRankTipTitle3.text = titleStr[2];
		this.mLabelRankTipTitle4.text = titleStr[3];
	}

	private void InitPrefabText()
	{
		try
		{
			this.mTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("赛季排名")
			});
		}
		catch (Exception ex)
		{
		}
	}

	private void InitTexture()
	{
		try
		{
		}
		catch (Exception ex)
		{
		}
	}

	private void InitHandler()
	{
		try
		{
			this.mBtnSelectHanders = new ZhanMengLianSaiMianRankPart.BtnSelectHander[2];
			this.mBtnSelectHanders[0] = new ZhanMengLianSaiMianRankPart.BtnSelectHander(this.mBowmSelectBtnsObjBtn1, 1);
			this.mBtnSelectHanders[0].RefreshInf(Global.GetLang("黄金联赛"));
			this.mBtnSelectHanders[0].Hander = new DPSelectedItemEventHandler(this.SelectBtnsClick);
			this.mBtnSelectHanders[1] = new ZhanMengLianSaiMianRankPart.BtnSelectHander(this.mBowmSelectBtnsObjBtn2, 2);
			this.mBtnSelectHanders[0].BSelect = true;
			this.mBtnSelectHanders[1].RefreshInf(Global.GetLang("新星赛"));
			this.mBtnSelectHanders[1].Hander = new DPSelectedItemEventHandler(this.SelectBtnsClick);
			this.mBtnSelectHanders[1].BSelect = false;
			this.mBtnTopHanders = new ZhanMengLianSaiMianRankPart.BtnTopHander[3];
			this.mBtnTopHanders[0] = new ZhanMengLianSaiMianRankPart.BtnTopHander(this.mObjBtn1, 1);
			this.mBtnTopHanders[0].RefreshInf(Global.GetLang("战盟排行"));
			this.mBtnTopHanders[0].Hander = new DPSelectedItemEventHandler(this.BtnTopBtnsClick);
			this.mBtnTopHanders[0].BSelect = true;
			this.mBtnTopHanders[1] = new ZhanMengLianSaiMianRankPart.BtnTopHander(this.mObjBtn2, 2);
			this.mBtnTopHanders[1].RefreshInf(Global.GetLang("MVP排行"));
			this.mBtnTopHanders[1].Hander = new DPSelectedItemEventHandler(this.BtnTopBtnsClick);
			this.mBtnTopHanders[1].BSelect = false;
			this.mBtnTopHanders[2] = new ZhanMengLianSaiMianRankPart.BtnTopHander(this.mObjBtn3, 3);
			this.mBtnTopHanders[2].RefreshInf(Global.GetLang("击杀排行"));
			this.mBtnTopHanders[2].Hander = new DPSelectedItemEventHandler(this.BtnTopBtnsClick);
			this.mBtnTopHanders[2].BSelect = false;
			this.mNoDataLabel.text = string.Empty;
			this.mBangHuiMatchRankType = 8;
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this.mObservableCollection = this.mListBoxListBox.ItemsSource;
			this.mListBoxListBox.hideInactive = true;
			this.RefreshMatchRankTyp();
		}
		catch (Exception ex)
		{
		}
	}

	private void BtnTopBtnsClick(object sender, DPSelectedItemEventArgs args)
	{
		try
		{
			for (int i = 0; i < this.mBtnTopHanders.Length; i++)
			{
				if (args.ID == this.mBtnTopHanders[i].ID)
				{
					this.mBtnTopHanders[i].BSelect = true;
				}
				else
				{
					this.mBtnTopHanders[i].BSelect = false;
				}
			}
		}
		catch (Exception ex)
		{
		}
		this.RefreshMatchRankTyp();
	}

	private void SelectBtnsClick(object sender, DPSelectedItemEventArgs args)
	{
		try
		{
			for (int i = 0; i < this.mBtnSelectHanders.Length; i++)
			{
				if (args.ID == this.mBtnSelectHanders[i].ID)
				{
					this.mBtnSelectHanders[i].BSelect = true;
				}
				else
				{
					this.mBtnSelectHanders[i].BSelect = false;
				}
			}
		}
		catch (Exception ex)
		{
		}
		this.RefreshMatchRankTyp();
	}

	public void ShowData(BangHuiMatchType type)
	{
		this.InitHandler();
		Super.ShowNetWaiting(null);
		if (type == 1)
		{
			this.mBtnSelectHanders[0].BSelect = true;
			this.mBtnSelectHanders[1].BSelect = false;
			GameInstance.Game.GetZhanMengLianSaiRankInfo(8);
		}
		else if (type == 2)
		{
			this.mBtnSelectHanders[1].BSelect = true;
			this.mBtnSelectHanders[0].BSelect = false;
			GameInstance.Game.GetZhanMengLianSaiRankInfo(9);
		}
		this.RefreshMatchRankTyp();
	}

	public void NoticeRankDataCallBack(List<BangHuiMatchRankInfo> data)
	{
		this.mZhanMengLianSaiMainRankItem.gameObject.SetActive(false);
		if (data != null && 0 < data.Count)
		{
			this.mNoDataLabel.gameObject.SetActive(false);
			int num = -1;
			byte b = 0;
			string empty = string.Empty;
			for (int i = 0; i < this.mObservableCollection.Count; i++)
			{
				GameObject at = this.mObservableCollection.GetAt(i);
				if (null != at)
				{
					at.SetActive(false);
				}
			}
			int j = 0;
			int count = data.Count;
			while (j < count)
			{
				if (Global.Data.roleData.RoleID == data[j].Key || Global.Data.roleData.Faction == data[j].Key)
				{
					if (j == count - 1)
					{
						if (this.mBangHuiMatchRankType != 8)
						{
							b = 1;
						}
						empty = string.Empty;
					}
					else
					{
						num = j;
					}
				}
				ZhanMengLianSaiMainRankItem zhanMengLianSaiMainRankItem;
				if (b != 1)
				{
					zhanMengLianSaiMainRankItem = this.GetZhanMengLianSaiMainRankItem(j);
				}
				else
				{
					zhanMengLianSaiMainRankItem = this.mZhanMengLianSaiMainRankItem;
				}
				zhanMengLianSaiMainRankItem.gameObject.SetActive(true);
				if (b == 1)
				{
					if (num != -1 && 0 <= num)
					{
						zhanMengLianSaiMainRankItem.SetInf((num + 1).ToString(), Global.GetLang("我的排名"), data[num].Param2, this.GetValueMOnad(this.mBangHuiMatchRankType, data[num].Value));
					}
					else
					{
						zhanMengLianSaiMainRankItem.SetInf(Global.GetLang("未上榜"), Global.GetLang("我的排名"), empty, this.GetValueMOnad(this.mBangHuiMatchRankType, data[j].Value));
					}
				}
				else
				{
					zhanMengLianSaiMainRankItem.SetInf((j + 1).ToString(), data[j].Param1, data[j].Param2, this.GetValueMOnad(this.mBangHuiMatchRankType, data[j].Value));
				}
				j++;
			}
			this.mListBoxListBox.repositionNow = true;
		}
		else
		{
			this.mNoDataLabel.text = Global.GetLang("暂无数据");
			this.mNoDataLabel.gameObject.SetActive(true);
			if (0 < this.mObservableCollection.Count)
			{
				for (int k = 0; k < this.mObservableCollection.Count; k++)
				{
					GameObject at2 = this.mObservableCollection.GetAt(k);
					if (null != at2)
					{
						at2.SetActive(false);
					}
				}
			}
		}
		if (this.mZhanMengLianSaiMainRankItem.gameObject.activeSelf)
		{
			Vector4 clipRange = this.mDraggablePanel.panel.clipRange;
			clipRange.w = 224f;
			if (this.mDraggablePanel.panel.clipRange.w == 264f)
			{
				this.mDraggablePanel.transform.localPosition = new Vector3(-3f, this.mDraggablePanel.transform.localPosition.y + 20f, -0.5f);
			}
			this.mDraggablePanel.panel.clipRange = clipRange;
		}
		else
		{
			Vector4 clipRange2 = this.mDraggablePanel.panel.clipRange;
			clipRange2.w = 264f;
			if (this.mDraggablePanel.panel.clipRange.w != 264f)
			{
				this.mDraggablePanel.transform.localPosition = new Vector3(-3f, this.mDraggablePanel.transform.localPosition.y - 20f, -0.5f);
			}
			this.mDraggablePanel.panel.clipRange = clipRange2;
		}
		SpringPanel.Begin(this.mDraggablePanel.gameObject, new Vector3(-3f, -52f, 0f), 10f);
	}

	private ZhanMengLianSaiMainRankItem GetZhanMengLianSaiMainRankItem(int idnex)
	{
		ZhanMengLianSaiMainRankItem zhanMengLianSaiMainRankItem = null;
		GameObject at = this.mObservableCollection.GetAt(idnex);
		if (null != at)
		{
			zhanMengLianSaiMainRankItem = at.GetComponent<ZhanMengLianSaiMainRankItem>();
			if (null == zhanMengLianSaiMainRankItem)
			{
				Object.Destroy(at);
			}
		}
		if (null == zhanMengLianSaiMainRankItem)
		{
			zhanMengLianSaiMainRankItem = U3DUtils.NEW<ZhanMengLianSaiMainRankItem>();
			this.mObservableCollection.AddNoUpdate(zhanMengLianSaiMainRankItem);
			zhanMengLianSaiMainRankItem.DragPanel = this.mDraggablePanel;
		}
		return zhanMengLianSaiMainRankItem;
	}

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private GameObject mObjBtn3;

	[SerializeField]
	private GameObject mObjBtn1;

	[SerializeField]
	private GameObject mObjBtn2;

	[SerializeField]
	private GameObject mBowmSelectBtnsObjBtn1;

	[SerializeField]
	private GameObject mBowmSelectBtnsObjBtn2;

	[SerializeField]
	private UILabel mLabelRankTipTitle1;

	[SerializeField]
	private UILabel mLabelRankTipTitle2;

	[SerializeField]
	private UILabel mLabelRankTipTitle3;

	[SerializeField]
	private UILabel mLabelRankTipTitle4;

	[SerializeField]
	private UIDraggablePanel mDraggablePanel;

	[SerializeField]
	private ListBox mListBoxListBox;

	[SerializeField]
	private UISprite mSpTitle;

	[SerializeField]
	private UILabel mNoDataLabel;

	[SerializeField]
	private UILabel mTitleLabel;

	[SerializeField]
	private ZhanMengLianSaiMainRankItem mZhanMengLianSaiMainRankItem;

	private ZhanMengLianSaiMianRankPart.BtnSelectHander[] mBtnSelectHanders;

	private ZhanMengLianSaiMianRankPart.BtnTopHander[] mBtnTopHanders;

	private BangHuiMatchRankType mBangHuiMatchRankType = 8;

	private ObservableCollection mObservableCollection;

	public DPSelectedItemEventHandler Hander;

	private class BtnSelectHander
	{
		public BtnSelectHander(GameObject root, int id)
		{
			try
			{
				this.mBGSp = root.transform.FindChild("sp").GetComponent<UISprite>();
				this.mLabel = root.transform.FindChild("label").GetComponent<UILabel>();
				this.mID = id;
				BoxCollider boxCollider = this.mBGSp.GetComponent<BoxCollider>();
				if (null == boxCollider)
				{
					boxCollider = this.mBGSp.gameObject.AddComponent<BoxCollider>();
				}
				boxCollider.size = new Vector3(4.1f, 1f, 0f);
				boxCollider.center = new Vector3(boxCollider.size.x / 2.4f, 0f, 0f);
				UIEventListener.Get(this.mBGSp.gameObject).onClick = delegate(GameObject g)
				{
					if (this.Hander != null)
					{
						this.Hander(this.mBGSp, new DPSelectedItemEventArgs
						{
							ID = this.mID
						});
					}
				};
			}
			catch (Exception ex)
			{
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		public void RefreshInf(string LabelStr)
		{
			this.mLabel.text = LabelStr;
			this.mBtnStr = LabelStr;
			this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.mBtnStr
			});
		}

		public bool BSelect
		{
			get
			{
				return this.mBSelect;
			}
			set
			{
				this.mBSelect = value;
				if (this.mBSelect)
				{
					this.mBGSp.spriteName = "SelectPointBtn0";
					this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						this.mBtnStr
					});
				}
				else
				{
					this.mBGSp.spriteName = "SelectPointBtn1";
					this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						this.mBtnStr
					});
				}
			}
		}

		public DPSelectedItemEventHandler Hander;

		private UISprite mBGSp;

		private UILabel mLabel;

		private int mID;

		private string mBtnStr = string.Empty;

		private bool mBSelect;
	}

	private class BtnTopHander
	{
		public BtnTopHander(GameObject root, int id)
		{
			try
			{
				this.mBGSp = root.transform.FindChild("sp").GetComponent<UISprite>();
				this.mLabel = root.transform.FindChild("label").GetComponent<UILabel>();
				this.mID = id;
				BoxCollider boxCollider = this.mBGSp.GetComponent<BoxCollider>();
				if (null == boxCollider)
				{
					boxCollider = this.mBGSp.gameObject.AddComponent<BoxCollider>();
				}
				boxCollider.center = new Vector3(0f, 0.5f, 0f);
				UIEventListener.Get(this.mBGSp.gameObject).onClick = delegate(GameObject g)
				{
					if (this.Hander != null)
					{
						this.Hander(g, new DPSelectedItemEventArgs
						{
							ID = this.mID
						});
					}
				};
			}
			catch (Exception ex)
			{
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		public void RefreshInf(string LabelStr)
		{
			this.mLabel.text = LabelStr;
			this.mBtnStr = LabelStr;
			this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				this.mBtnStr
			});
		}

		public bool BSelect
		{
			get
			{
				return this.mBSelect;
			}
			set
			{
				this.mBSelect = value;
				if (this.mBSelect)
				{
					this.mBGSp.spriteName = "TopTabBtn_hover";
					Vector3 localPosition = this.mBGSp.transform.localPosition;
					localPosition.y = -26f;
					this.mBGSp.transform.localPosition = localPosition;
					this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"f0f0f0",
						this.mBtnStr
					});
				}
				else
				{
					this.mBGSp.spriteName = "TopTabBtn_normal";
					Vector3 localPosition2 = this.mBGSp.transform.localPosition;
					localPosition2.y = -30f;
					this.mBGSp.transform.localPosition = localPosition2;
					this.mLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"dac7ae",
						this.mBtnStr
					});
				}
			}
		}

		public DPSelectedItemEventHandler Hander;

		private UISprite mBGSp;

		private UILabel mLabel;

		private int mID;

		private string mBtnStr = string.Empty;

		private bool mBSelect;
	}
}
