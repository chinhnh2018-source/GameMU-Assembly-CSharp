using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class RecyclePart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.mLabTitle.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("回收类型")
			});
			this.mLabRole.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("人物装备回收")
			});
			this.mLabJingLing.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("精灵道具回收")
			});
			this.mLabHorse.text = Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				Global.GetLang("坐骑道具回收")
			});
			this.mLabRole.pivot = 3;
			this.mLabRole.transform.localPosition = new Vector3(-45f, -4.5f, -1.2f);
			this.mLabJingLing.pivot = 3;
			this.mLabJingLing.transform.localPosition = new Vector3(-45f, -4.5f, -1.2f);
			this.mLabHorse.pivot = 3;
			this.mLabHorse.transform.localPosition = new Vector3(-45f, -4.5f, -1.2f);
			this.mLabRole.lineWidth = 155;
			this.mLabJingLing.lineWidth = 155;
			this.mLabHorse.lineWidth = 155;
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
			this.mBtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(e, new DPSelectedItemEventArgs
					{
						IDType = 0,
						ID = -1
					});
				}
			};
			RecyclePart.Itemshander itemshander = new RecyclePart.Itemshander(this.Item1);
			itemshander.Hander = new DPSelectedItemEventHandler(this.ItemSelect);
			itemshander.select = false;
			this.mItemsList.Add(itemshander);
			RecyclePart.Itemshander itemshander2 = new RecyclePart.Itemshander(this.Item2);
			itemshander2.Hander = new DPSelectedItemEventHandler(this.ItemSelect);
			itemshander2.select = false;
			this.mItemsList.Add(itemshander2);
			if (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.Horese))
			{
				RecyclePart.Itemshander itemshander3 = new RecyclePart.Itemshander(this.Item3);
				itemshander3.Hander = new DPSelectedItemEventHandler(this.ItemSelect);
				itemshander3.select = false;
				this.mItemsList.Add(itemshander3);
				this.BakTran.localScale = new Vector3(this.BakTran.localScale.x, 314f, this.BakTran.localScale.y);
			}
			else
			{
				this.BakTran.localScale = new Vector3(this.BakTran.localScale.x, 230f, this.BakTran.localScale.y);
				this.Item3.SetActive(false);
			}
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void ItemSelect(object sender, DPSelectedItemEventArgs args)
	{
		if (this.Hander != null)
		{
			this.Hander(sender, args);
		}
	}

	internal void ShowHelpAnim(int p1, int p2)
	{
		if (p1 == 704 && null != this.Item1)
		{
			SystemHelpPart.SetMask(this.Item1.transform, default(Vector4));
		}
	}

	internal void InitData()
	{
		if (0 < SystemHelpMgr.ActiveHelpID)
		{
			SystemHelpMgr.OnAction(UIObjIDs.RecyclePart, HelpStateEvents.Actived, -1);
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GameObject Item1;

	[SerializeField]
	private GameObject Item2;

	[SerializeField]
	private GameObject Item3;

	[SerializeField]
	private GButton mBtnClose;

	[SerializeField]
	private UILabel mLabTitle;

	[SerializeField]
	private UILabel mLabRole;

	[SerializeField]
	private UILabel mLabJingLing;

	[SerializeField]
	private UILabel mLabHorse;

	[SerializeField]
	private Transform BakTran;

	private List<RecyclePart.Itemshander> mItemsList = new List<RecyclePart.Itemshander>();

	private class Itemshander
	{
		public Itemshander(GameObject ItemObj)
		{
			this.mRootObj = ItemObj;
			try
			{
				UIEventListener.Get(this.mRootObj).onClick = delegate(GameObject g)
				{
					if (this.Hander != null)
					{
						int id = 0;
						if (this.mRootObj.name == "RenWuHuiShou")
						{
							id = 0;
						}
						else if (this.mRootObj.name == "JingLingHuiShou")
						{
							id = 1;
						}
						else if (this.mRootObj.name == "ZuoQiHuiShou")
						{
							id = 2;
						}
						this.Hander(this.mRootObj, new DPSelectedItemEventArgs
						{
							ID = id,
							IDType = 0
						});
					}
				};
				ShowNetImage component = this.mRootObj.transform.GetChild(0).GetComponent<ShowNetImage>();
				component.URL = "NetImages/GameRes/Images/Bag/" + this.mRootObj.name + ".jpg";
				this.mSelectEffest = this.mRootObj.transform.GetChild(1).gameObject;
			}
			catch (Exception ex)
			{
				MUDebug.Log<string>(new string[]
				{
					ex.Message
				});
			}
		}

		public bool select
		{
			set
			{
				this.bSelect = value;
				this.mSelectEffest.SetActive(this.bSelect);
			}
		}

		private GameObject mRootObj;

		public DPSelectedItemEventHandler Hander;

		private GameObject mSelectEffest;

		private bool bSelect;
	}
}
