using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class ShiLiMiDongChoseFightPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendShiLiMiDongGetBaseData();
	}

	private void InitPrefabText()
	{
		try
		{
			this._FightBtn.Text = Global.GetLang("进攻");
			this._GuardBtn.Text = Global.GetLang("防守");
			this._battlegroundInfTitleLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"d6b15a",
				Global.GetLang("战场信息")
			});
			this.battlegOwnInfLabel.text = string.Empty;
			this.ProgressLabel.text = string.Empty;
			this.ResultNumLabel.text = string.Empty;
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
			this._BtnClose.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 0
					});
				}
			};
			this._FightBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._FightBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._FightBtn.GetInstanceID(), 0.2f);
				CompMineWarVO compMineWarVOByID = IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineWarVOByID(this.mChoseFightItems[this.mSelectIndex].ID + 1);
				if (compMineWarVOByID != null)
				{
					GameInstance.Game.SendShiLiMiDongGoToBattleGround(compMineWarVOByID.ID);
					if (this.Hander != null)
					{
						this.Hander(this, new DPSelectedItemEventArgs
						{
							Type = 2,
							ID = 1,
							Index = this.mSelectIndex
						});
					}
				}
				else if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 2,
						ID = 1,
						Index = this.mSelectIndex
					});
				}
			};
			this._GuardBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._GuardBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._GuardBtn.GetInstanceID(), 1f);
				if (this.Hander != null)
				{
					this.Hander(this, new DPSelectedItemEventArgs
					{
						Type = 2,
						ID = 2,
						Index = this.mSelectIndex
					});
				}
			};
			List<int> list = new List<int>();
			int num = ShiLiData.GetSelfCompType() - 1;
			list.Add(num);
			for (byte b = 0; b < 3; b += 1)
			{
				if ((int)b != num)
				{
					list.Add((int)b);
				}
			}
			byte b2 = 0;
			while ((int)b2 < this._Items.Length)
			{
				ShiLiMiDongChoseFightPart.ItemHander itemHander = new ShiLiMiDongChoseFightPart.ItemHander(this._Items[(int)b2], list[(int)b2]);
				this.mChoseFightItems.Add(itemHander);
				itemHander.Hander = new DPSelectedItemEventHandler(this.ItemClickHander);
				b2 += 1;
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

	private void ItemClickHander(object sender, DPSelectedItemEventArgs args)
	{
		byte b = 0;
		while ((int)b < this.mChoseFightItems.Count)
		{
			ShiLiMiDongChoseFightPart.ItemHander itemHander = this.mChoseFightItems[(int)b];
			if (itemHander.ID == args.ID)
			{
				itemHander.BSelect = true;
				this.mSelectIndex = (int)b;
			}
			else
			{
				itemHander.BSelect = false;
			}
			b += 1;
		}
		this.RefreshBattlegroundInf();
	}

	private void RefreshBattlegroundInf()
	{
		if (this.mCompMineBaseDataList != null)
		{
			if (this.mSelectIndex < this.mCompMineBaseDataList.Count)
			{
				CompMineBaseData compMineBaseData = this.mCompMineBaseDataList[this.mChoseFightItems[this.mSelectIndex].ID];
				CompMineWarVO compMineWarVOByID = IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineWarVOByID(this.mChoseFightItems[this.mSelectIndex].ID + 1);
				if (compMineWarVOByID != null)
				{
					this.battlegOwnInfLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ddd2bd",
						Global.GetLang("归属势力：") + compMineWarVOByID.Name
					});
				}
				else
				{
					this.battlegOwnInfLabel.text = Global.GetColorStringForNGUIText(new object[]
					{
						"ddd2bd",
						Global.GetLang("归属势力：") + string.Empty
					});
				}
				this.ProgressLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ddd2bd",
					string.Concat(new object[]
					{
						Global.GetLang("矿车进度："),
						compMineBaseData.MineTruckProcess,
						"/",
						IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineTruckCount()
					})
				});
				this.ResultNumLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ddd2bd",
					Global.GetLang("安全到达：") + compMineBaseData.SafeArrived
				});
			}
			else
			{
				this.battlegOwnInfLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ddd2bd",
					Global.GetLang("归属势力：") + string.Empty
				});
				this.ProgressLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ddd2bd",
					Global.GetLang("矿车进度：") + "0/" + IConfigbase<ConfigShiLiMiDong>.Instance.GetCompMineTruckCount()
				});
				this.ResultNumLabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ddd2bd",
					Global.GetLang("安全到达：") + "0"
				});
			}
		}
	}

	internal void NoticeGetBaseDataCallBack(List<CompMineBaseData> data)
	{
		this.mCompMineBaseDataList = data;
		int id = 0;
		byte b = 0;
		while ((int)b < this.mChoseFightItems.Count)
		{
			if (ShiLiData.GetSelfCompType() - 1 == this.mChoseFightItems[(int)b].ID)
			{
				id = this.mChoseFightItems[(int)b].ID;
				break;
			}
			b += 1;
		}
		this.ItemClickHander(null, new DPSelectedItemEventArgs
		{
			ID = id
		});
	}

	public const string COLORD6B15A = "d6b15a";

	public const string COLORDDD2BD = "ddd2bd";

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton _BtnClose;

	[SerializeField]
	private GameObject[] _Items;

	[SerializeField]
	private UILabel _battlegroundInfTitleLabel;

	[SerializeField]
	private UILabel battlegOwnInfLabel;

	[SerializeField]
	private UILabel ProgressLabel;

	[SerializeField]
	private UILabel ResultNumLabel;

	[SerializeField]
	private GButton _FightBtn;

	[SerializeField]
	private GButton _GuardBtn;

	private List<ShiLiMiDongChoseFightPart.ItemHander> mChoseFightItems = new List<ShiLiMiDongChoseFightPart.ItemHander>();

	private int mSelectIndex;

	private List<CompMineBaseData> mCompMineBaseDataList;

	public class ItemHander
	{
		public ItemHander(GameObject root, int id)
		{
			ShiLiMiDongChoseFightPart.ItemHander <>f__this = this;
			this.mID = id;
			this.imageBak = root.transform.FindChild("Bg").GetComponent<ShowNetImage>();
			this.imageBg = root.transform.FindChild("Image").GetComponent<ShowNetImage>();
			this.Title = root.transform.FindChild("Label").GetComponent<UILabel>();
			this.BSelect = false;
			UIEventListener.Get(root).onClick = delegate(GameObject e)
			{
				if (0f < Global.GetBtnCD(root.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(root.GetInstanceID(), 0.5f);
				if (<>f__this.Hander != null)
				{
					<>f__this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = <>f__this.mID
					});
				}
			};
			this.Title.text = string.Empty;
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		public bool BSelect
		{
			set
			{
				this.mBSelect = value;
				if (!this.mBSelect)
				{
					this.imageBg.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/battlegroundBack" + this.mID + ".png";
					this.imageBg.ImageDownloaded = delegate(object e)
					{
						this.imageBg.transform.localScale = new Vector3((float)this.imageBg.ItsSizeWidth, (float)this.imageBg.ItsSizeHeight, 0f);
					};
				}
				else
				{
					this.imageBg.URL = "NetImages/GameRes/Images/ShiLiMiDongImage/battleground" + this.mID + ".png";
					this.imageBg.ImageDownloaded = delegate(object e)
					{
						this.imageBg.transform.localScale = new Vector3((float)this.imageBg.ItsSizeWidth, (float)this.imageBg.ItsSizeHeight, 0f);
					};
				}
			}
		}

		private int mID;

		private ShowNetImage imageBak;

		private ShowNetImage imageBg;

		private UILabel Title;

		public DPSelectedItemEventHandler Hander;

		private bool mBSelect;
	}
}
