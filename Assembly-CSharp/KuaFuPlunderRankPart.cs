using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class KuaFuPlunderRankPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.TopBtnTabClassClick(null, new DPSelectedItemEventArgs
		{
			ID = this.mKuaFuLueDuoRankType
		});
	}

	private void InitPrefabText()
	{
		try
		{
			this.mLiftInfLabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("上周掠夺冠军")
			});
			this.mMyPlunderValue.text = string.Empty;
			this.mNoDataLabel.text = Global.GetLang("暂无数据");
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
			this.mBgImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/MainPartBgImage.png";
			this.mBgImage.ImageDownloaded = delegate(object g)
			{
				this.mBgImage.transform.localScale = new Vector3((float)this.mBgImage.ItsSizeWidth, (float)this.mBgImage.ItsSizeHeight, 0f);
			};
			this.mLiftImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/QiZi.png";
			this.mLiftImage.ImageDownloaded = delegate(object g)
			{
				this.mLiftImage.transform.localScale = new Vector3((float)this.mLiftImage.ItsSizeWidth, (float)this.mLiftImage.ItsSizeHeight, 0f);
			};
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
			KuaFuPlunderRankPart.TopBtnTabClass topBtnTabClass = new KuaFuPlunderRankPart.TopBtnTabClass(this.mTopBtnSp[0], this.mTopBtnlabel[0], 4);
			topBtnTabClass.RefreshInf(Global.GetLang("个人击杀"));
			topBtnTabClass.Hander = new DPSelectedItemEventHandler(this.TopBtnTabClassClick);
			topBtnTabClass.BSelect = false;
			this.mlistTopBtnTabClass.Add(topBtnTabClass);
			KuaFuPlunderRankPart.TopBtnTabClass topBtnTabClass2 = new KuaFuPlunderRankPart.TopBtnTabClass(this.mTopBtnSp[1], this.mTopBtnlabel[1], 2);
			topBtnTabClass2.RefreshInf(Global.GetLang("战盟掠夺"));
			topBtnTabClass2.Hander = new DPSelectedItemEventHandler(this.TopBtnTabClassClick);
			topBtnTabClass2.BSelect = false;
			this.mlistTopBtnTabClass.Add(topBtnTabClass2);
			KuaFuPlunderRankPart.TopBtnTabClass topBtnTabClass3 = new KuaFuPlunderRankPart.TopBtnTabClass(this.mTopBtnSp[2], this.mTopBtnlabel[2], 0);
			topBtnTabClass3.RefreshInf(Global.GetLang("服务器征服"));
			topBtnTabClass3.Hander = new DPSelectedItemEventHandler(this.TopBtnTabClassClick);
			topBtnTabClass3.BSelect = true;
			this.mlistTopBtnTabClass.Add(topBtnTabClass3);
			this.mOBCView = this.mViewListBox.ItemsSource;
			this.mCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
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
		}
		catch (Exception ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + ex.Message + "</color>"
			});
		}
	}

	private void RefreshTitles(int index)
	{
		if (index == 0)
		{
			this.mViewTitleLabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排行")
			});
			this.mViewTitleLabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("角色名")
			});
			this.mViewTitleLabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("击杀个数")
			});
			this.mTitleSp.spriteName = "ZongJiShaPaiHang";
			this.mIconImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/RoleKill.png";
		}
		else if (index == 1)
		{
			this.mViewTitleLabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排行")
			});
			this.mViewTitleLabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("战盟")
			});
			this.mViewTitleLabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("掠夺资源")
			});
			this.mTitleSp.spriteName = "ZongLueDuoPaiHang";
			this.mIconImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/LueDuo.png";
		}
		else
		{
			this.mViewTitleLabel[0].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("排行")
			});
			this.mViewTitleLabel[1].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("服务器名")
			});
			this.mViewTitleLabel[2].text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("征服服务器个数")
			});
			this.mIconImage.URL = "NetImages/GameRes/Images/KuaFuPlunderImage/ZhengFu.png";
			this.mTitleSp.spriteName = "ZongZengFuPaiHang";
		}
		Vector3 localPosition = this.mIconImage.transform.localPosition;
		localPosition.x = 28f;
		this.mIconImage.transform.localPosition = localPosition;
		this.mIconImage.ImageDownloaded = delegate(object g)
		{
			this.mIconImage.transform.localScale = new Vector3((float)this.mIconImage.ItsSizeWidth, (float)this.mIconImage.ItsSizeHeight, 0f);
		};
		this.mTitleSp.transform.localScale = new Vector3(154f, 34f, 0f);
	}

	private void RefreshTitles(KuaFuLueDuoRankType type)
	{
		if (type == null)
		{
			this.RefreshTitles(2);
		}
		else if (type == 2)
		{
			this.RefreshTitles(1);
		}
		else if (type == 4)
		{
			this.RefreshTitles(0);
		}
	}

	private void TopBtnTabClassClick(object sender, DPSelectedItemEventArgs args)
	{
		KuaFuLueDuoRankType id = args.ID;
		byte b = 0;
		while ((int)b < this.mlistTopBtnTabClass.Count)
		{
			if (id == this.mlistTopBtnTabClass[(int)b].RankType)
			{
				this.mlistTopBtnTabClass[(int)b].BSelect = true;
			}
			else
			{
				this.mlistTopBtnTabClass[(int)b].BSelect = false;
			}
			b += 1;
		}
		if (sender != null && this.mKuaFuLueDuoRankType == id)
		{
			return;
		}
		this.mKuaFuLueDuoRankType = id;
		long age = -1L;
		if (this.mKuaFuLueDuoRankType == 4)
		{
			if (this.mKuaFuLueDuoRankListCmdDataRoleKill != null)
			{
				age = this.mKuaFuLueDuoRankListCmdDataRoleKill.Age;
			}
		}
		else if (this.mKuaFuLueDuoRankType == null)
		{
			if (this.mKuaFuLueDuoRankListCmdDataServer != null)
			{
				age = this.mKuaFuLueDuoRankListCmdDataServer.Age;
			}
		}
		else if (this.mKuaFuLueDuoRankType == 2 && this.mKuaFuLueDuoRankListCmdDataBH != null)
		{
			age = this.mKuaFuLueDuoRankListCmdDataBH.Age;
		}
		GameInstance.Game.SendGetKuFuPlubdeRankData(age, id);
		this.RefreshTitles(id);
		Super.ShowNetWaiting(null);
	}

	private bool GetKuaFuPlunderRankItem(int Index, out KuaFuPlunderRankItem item)
	{
		item = null;
		GameObject at = this.mOBCView.GetAt(Index);
		if (null != at)
		{
			item = at.GetComponent<KuaFuPlunderRankItem>();
			if (null == at)
			{
				Object.Destroy(at);
			}
		}
		if (null == item)
		{
			item = U3DUtils.NEW<KuaFuPlunderRankItem>();
			this.mOBCView.Add(item);
			item.gameObject.SetActive(true);
			item.DragPanel = this.mViewDragPanel;
			return true;
		}
		item.gameObject.SetActive(true);
		return false;
	}

	private IEnumerator RefeashView(List<KuaFuLueDuoRankInfo> rankList)
	{
		byte num = 0;
		int i = 0;
		int max = (rankList.Count < 20) ? rankList.Count : 20;
		while (i < max)
		{
			KuaFuPlunderRankItem item = null;
			if (this.GetKuaFuPlunderRankItem(i, out item))
			{
				num += 1;
				if (num > 3)
				{
					num = 0;
					yield return null;
				}
			}
			num = 0;
			if (null != item)
			{
				string Name = string.Empty;
				if (this.mKuaFuLueDuoRankType == 2)
				{
					Name = rankList[i].Param1;
				}
				else if (this.mKuaFuLueDuoRankType == 4)
				{
					Name = rankList[i].Param1;
				}
				else
				{
					ZtBuffServerInfo serverInf = null;
					if (Global.GetNowServerIsZhuTiFu(rankList[i].Key, out serverInf))
					{
						Name = serverInf.strServerName;
					}
					else
					{
						Name = "S." + rankList[i].Key.ToString();
					}
				}
				item.SetInf(i + 1, Name, rankList[i].Value);
			}
			i++;
		}
		if (rankList.Count > this.mOBCView.Count)
		{
			for (int j = rankList.Count; j < this.mOBCView.Count; j++)
			{
				GameObject obj = this.mOBCView.GetAt(j);
				if (null != obj)
				{
					obj.SetActive(false);
				}
			}
		}
		this.mViewListBox.repositionNow = true;
		SpringPanel.Begin(this.mViewDragPanel.gameObject, new Vector3(-240f, -244f, -1f), 10f);
		yield break;
	}

	private void HideViewChilds()
	{
		for (int i = 0; i < this.mOBCView.Count; i++)
		{
			GameObject at = this.mOBCView.GetAt(i);
			if (null != at)
			{
				at.SetActive(false);
			}
		}
	}

	public void NoticeGetRankDataCallback(KuaFuLueDuoRankListCmdData data)
	{
		KuaFuLueDuoRankListCmdData kuaFuLueDuoRankListCmdData = data;
		if (data != null)
		{
			if (this.mKuaFuLueDuoRankType == 4)
			{
				if (this.mKuaFuLueDuoRankListCmdDataRoleKill != null)
				{
					if (this.mKuaFuLueDuoRankListCmdDataRoleKill.Age != data.Age)
					{
						this.mKuaFuLueDuoRankListCmdDataRoleKill = data;
					}
				}
				else
				{
					this.mKuaFuLueDuoRankListCmdDataRoleKill = data;
				}
				kuaFuLueDuoRankListCmdData = this.mKuaFuLueDuoRankListCmdDataRoleKill;
			}
			else if (this.mKuaFuLueDuoRankType == null)
			{
				if (this.mKuaFuLueDuoRankListCmdDataServer != null)
				{
					if (this.mKuaFuLueDuoRankListCmdDataServer.Age != data.Age)
					{
						this.mKuaFuLueDuoRankListCmdDataServer = data;
					}
				}
				else
				{
					this.mKuaFuLueDuoRankListCmdDataServer = data;
				}
				kuaFuLueDuoRankListCmdData = this.mKuaFuLueDuoRankListCmdDataServer;
			}
			else if (this.mKuaFuLueDuoRankType == 2)
			{
				if (this.mKuaFuLueDuoRankListCmdDataBH != null)
				{
					if (this.mKuaFuLueDuoRankListCmdDataBH.Age != data.Age)
					{
						this.mKuaFuLueDuoRankListCmdDataBH = data;
					}
				}
				else
				{
					this.mKuaFuLueDuoRankListCmdDataBH = data;
				}
				kuaFuLueDuoRankListCmdData = this.mKuaFuLueDuoRankListCmdDataBH;
			}
			this.HideViewChilds();
			if (kuaFuLueDuoRankListCmdData.RankType == this.mKuaFuLueDuoRankType)
			{
				if (kuaFuLueDuoRankListCmdData.ListRankList != null && 0 < kuaFuLueDuoRankListCmdData.ListRankList.Count)
				{
					base.StartCoroutine<bool>(this.RefeashView(kuaFuLueDuoRankListCmdData.ListRankList));
					this.mNoDataLabel.text = string.Empty;
				}
				else
				{
					this.mNoDataLabel.text = Global.GetLang("暂无数据");
				}
			}
		}
		else
		{
			this.HideViewChilds();
		}
		if (kuaFuLueDuoRankListCmdData != null)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			if (this.mKuaFuLueDuoRankType == 2)
			{
				text = Global.GetLang("掠夺资源：");
				this.mLiftInfLabel[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("上周掠夺冠军")
				});
			}
			else if (this.mKuaFuLueDuoRankType == 4)
			{
				text = Global.GetLang("击杀人数：");
				this.mLiftInfLabel[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("上周击杀冠军")
				});
			}
			else
			{
				text = Global.GetLang("征服服务器个数：");
				this.mLiftInfLabel[0].text = Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("征服冠军")
				});
			}
			this.RefreshTitles(this.mKuaFuLueDuoRankType);
			if (kuaFuLueDuoRankListCmdData.RankType == this.mKuaFuLueDuoRankType)
			{
				int num = -1;
				if (kuaFuLueDuoRankListCmdData.LastData != null)
				{
					if (this.mKuaFuLueDuoRankType == 2)
					{
						num = kuaFuLueDuoRankListCmdData.LastData.Value;
						this.mLiftInfLabel[1].text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							kuaFuLueDuoRankListCmdData.LastData.Param1
						});
					}
					else if (this.mKuaFuLueDuoRankType == 4)
					{
						num = kuaFuLueDuoRankListCmdData.LastData.Value;
						this.mLiftInfLabel[1].text = Global.GetColorStringForNGUIText(new object[]
						{
							"dac7ae",
							kuaFuLueDuoRankListCmdData.LastData.Param1
						});
					}
					else
					{
						num = kuaFuLueDuoRankListCmdData.LastData.Value;
						ZtBuffServerInfo ztBuffServerInfo = null;
						if (Global.GetNowServerIsZhuTiFu(kuaFuLueDuoRankListCmdData.LastData.Key, out ztBuffServerInfo))
						{
							this.mLiftInfLabel[1].text = Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								ztBuffServerInfo.strServerName
							});
						}
						else
						{
							this.mLiftInfLabel[1].text = Global.GetColorStringForNGUIText(new object[]
							{
								"dac7ae",
								"S." + kuaFuLueDuoRankListCmdData.LastData.Key
							});
						}
					}
				}
				else
				{
					this.mLiftInfLabel[1].text = Global.GetLang("暂无数据");
				}
				this.mLiftInfLabel[2].text = Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					text,
					"fdf7dd",
					(num > 0) ? num.ToString() : "0"
				});
				if (kuaFuLueDuoRankListCmdData.SelfData != null)
				{
					int num2 = -1;
					if (kuaFuLueDuoRankListCmdData.ListRankList != null)
					{
						if (this.mKuaFuLueDuoRankType == 2)
						{
							num2 = kuaFuLueDuoRankListCmdData.ListRankList.FindIndex((KuaFuLueDuoRankInfo e) => e.Key == Global.Data.roleData.Faction);
						}
						else if (this.mKuaFuLueDuoRankType == 4)
						{
							num2 = kuaFuLueDuoRankListCmdData.ListRankList.FindIndex((KuaFuLueDuoRankInfo e) => e.Key == Global.Data.roleData.RoleID);
						}
						else if (this.mKuaFuLueDuoStateData != null)
						{
							num2 = kuaFuLueDuoRankListCmdData.ListRankList.FindIndex((KuaFuLueDuoRankInfo e) => e.Key == this.mKuaFuLueDuoStateData.ServerID);
						}
					}
					if (0 > num2)
					{
						this.mMyRankLabel.text = Global.GetLang("未上榜");
					}
					else
					{
						num2++;
						if (20 < num2)
						{
							this.mMyRankLabel.text = Global.GetLang("未上榜");
						}
						else
						{
							this.mMyRankLabel.text = num2.ToString();
						}
					}
					this.mMyPlunderValue.text = Global.GetColorStringForNGUIText(new object[]
					{
						"e3b36c",
						text,
						"fdf7dd",
						kuaFuLueDuoRankListCmdData.SelfData.Value
					});
				}
				else
				{
					this.mMyRankLabel.text = Global.GetLang("暂无数据");
					this.mMyPlunderValue.text = string.Empty;
				}
			}
			else
			{
				this.mLiftInfLabel[1].text = Global.GetLang("暂无数据");
				this.mMyRankLabel.text = Global.GetLang("暂无数据");
				text2 = Global.GetLang("未上榜");
			}
		}
		else
		{
			this.mLiftInfLabel[1].text = Global.GetLang("暂无数据");
			this.mLiftInfLabel[2].text = string.Empty;
			this.mMyRankLabel.text = Global.GetLang("暂无数据");
			this.mMyPlunderValue.text = string.Empty;
		}
	}

	public KuaFuLueDuoStateData KuaFuLueDuoStateData
	{
		set
		{
			this.mKuaFuLueDuoStateData = value;
			if (this.mKuaFuLueDuoStateData != null)
			{
				if (this.mKuaFuLueDuoRankType == 4)
				{
					this.NoticeGetRankDataCallback(this.mKuaFuLueDuoRankListCmdDataRoleKill);
				}
				else if (this.mKuaFuLueDuoRankType == null)
				{
					this.NoticeGetRankDataCallback(this.mKuaFuLueDuoRankListCmdDataServer);
				}
				else if (this.mKuaFuLueDuoRankType == 2)
				{
					this.NoticeGetRankDataCallback(this.mKuaFuLueDuoRankListCmdDataBH);
				}
			}
		}
	}

	[SerializeField]
	private GButton mCloseBtn;

	[SerializeField]
	private UISprite[] mTopBtnSp;

	[SerializeField]
	private UILabel[] mTopBtnlabel;

	[SerializeField]
	private ShowNetImage mBgImage;

	[SerializeField]
	private UILabel[] mLiftInfLabel;

	[SerializeField]
	private ShowNetImage mIconImage;

	[SerializeField]
	private UILabel[] mViewTitleLabel;

	[SerializeField]
	private UIDraggablePanel mViewDragPanel;

	[SerializeField]
	private ListBox mViewListBox;

	[SerializeField]
	private UILabel mMyRankLabel;

	[SerializeField]
	private UILabel mMyPlunderValue;

	[SerializeField]
	private ShowNetImage mLiftImage;

	[SerializeField]
	private UILabel mNoDataLabel;

	[SerializeField]
	private UISprite mTitleSp;

	private ObservableCollection mOBCView;

	private List<KuaFuPlunderRankPart.TopBtnTabClass> mlistTopBtnTabClass = new List<KuaFuPlunderRankPart.TopBtnTabClass>();

	private KuaFuLueDuoRankListCmdData mKuaFuLueDuoRankListCmdDataRoleKill;

	private KuaFuLueDuoRankListCmdData mKuaFuLueDuoRankListCmdDataBH;

	private KuaFuLueDuoRankListCmdData mKuaFuLueDuoRankListCmdDataServer;

	private KuaFuLueDuoRankType mKuaFuLueDuoRankType = 2;

	private KuaFuLueDuoStateData mKuaFuLueDuoStateData;

	public DPSelectedItemEventHandler Hander;

	private class TopBtnTabClass
	{
		public TopBtnTabClass(UISprite bg, UILabel Label, KuaFuLueDuoRankType id)
		{
			KuaFuPlunderRankPart.TopBtnTabClass <>f__this = this;
			this.mBGSp = bg;
			this.mLabel = Label;
			this.mRankType = id;
			BoxCollider boxCollider = bg.GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				boxCollider = bg.gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.center = new Vector3(0f, 0.5f, 0f);
			UIEventListener.Get(bg.gameObject).onClick = delegate(GameObject g)
			{
				if (<>f__this.Hander != null)
				{
					<>f__this.Hander(bg, new DPSelectedItemEventArgs
					{
						ID = <>f__this.mRankType
					});
				}
			};
		}

		public KuaFuLueDuoRankType RankType
		{
			get
			{
				return this.mRankType;
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

		private KuaFuLueDuoRankType mRankType;

		private string mBtnStr = string.Empty;

		private bool mBSelect;
	}
}
