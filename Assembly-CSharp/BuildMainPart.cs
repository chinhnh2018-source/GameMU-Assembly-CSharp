using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class BuildMainPart : UserControl
{
	public int PetShowSignGoodsID
	{
		set
		{
			if (value == 0)
			{
				this.mPetShowSign = 0;
			}
			else
			{
				this.mPetShowSign = 1;
			}
			this.mPetShowFixPosGoodsID = value;
			this.SetPetShowSign(this.mPetShowSign);
		}
	}

	private void PetShowFixPos()
	{
		if (0 < this.mPetShowFixPosGoodsID)
		{
			bool flag = true;
			bool flag2 = true;
			this.PetShowDragPanel.IsOnLiftOrRight(ref flag, ref flag2);
			if (flag2 && NGUIMath.CalculateRelativeWidgetBounds(this.PetShowDragPanel.transform).size.x > this.PetShowDragPanel.panel.clipRange.z - this.PetShowListBox.cellWidth)
			{
				float num = 0f;
				float num2 = 0f;
				for (int i = 0; i < this.mOBCPetShow.Count; i++)
				{
					GameObject at = this.mOBCPetShow.GetAt(i);
					if (num > at.transform.localPosition.x)
					{
						num = at.transform.localPosition.x;
					}
					if (num2 < at.transform.localPosition.x)
					{
						num2 = at.transform.localPosition.x;
					}
				}
				SpringPanel.Begin(this.PetShowDragPanel.gameObject, new Vector3(this.PetShowDragPanel.transform.localPosition.x - (num2 + 84f - this.PetShowDragPanel.panel.clipRange.z), this.PetShowDragPanel.transform.localPosition.y, this.PetShowDragPanel.transform.localPosition.z), (float)this.PetShowListBox.transform.childCount);
			}
			this.mPetShowFixPosGoodsID = 0;
		}
	}

	private void SetPetShowSign(byte bShow)
	{
		this.mPetShowSign = bShow;
		BoxCollider component = this.PetShowDirectionSign.GetComponent<BoxCollider>();
		Vector3 center = component.center;
		this.PetShowInfBg.alpha = 0.5f;
		float num = 0.15f;
		if (this.mPetShowSign == 0)
		{
			this.DirectionSignDirectionValue = 1;
			base.StartCoroutine<bool>(this.WaitForSecond(delegate(object e, DPSelectedItemEventArgs s)
			{
				this.PetShowDragPanel.transform.localPosition = new Vector3(this.PetShowDragPanel.transform.localPosition.x, -90f, this.PetShowDragPanel.transform.localPosition.z);
				this.EnabledBoxCollider(true);
			}, num));
			base.StartCoroutine<bool>(this.WaitForSecond(delegate(object e, DPSelectedItemEventArgs s)
			{
				this.PetShowDirectionSign_[0].enabled = false;
				this.PetShowDirectionSign_[1].enabled = false;
			}, 0f));
			TweenPosition.Begin(this.PetShowRoot.gameObject, num, new Vector3(this.PetShowRoot.transform.localPosition.x, -90f, 0f));
			TweenPosition.Begin(this.PetShowInfRoot, num, new Vector3(0f, -32f, -202f));
			center.x = Mathf.Abs(center.x);
		}
		else
		{
			this.DirectionSignDirectionValue = 0;
			base.StartCoroutine<bool>(this.WaitForSecond(delegate(object e, DPSelectedItemEventArgs s)
			{
				this.PetShowDragPanel.transform.localPosition = new Vector3(this.PetShowDragPanel.transform.localPosition.x, 0f, this.PetShowDragPanel.transform.localPosition.z);
				this.EnabledBoxCollider(false);
			}, 0f));
			base.StartCoroutine<bool>(this.WaitForSecond(delegate(object e, DPSelectedItemEventArgs s)
			{
				this.PetShowDirectionSign_[0].enabled = true;
				this.PetShowDirectionSign_[1].enabled = true;
			}, num));
			TweenPosition.Begin(this.PetShowRoot.gameObject, num, new Vector3(this.PetShowRoot.transform.localPosition.x, 0f, 0f));
			TweenPosition.Begin(this.PetShowInfRoot, num, new Vector3(0f, 74f, -202f));
			center.x = -Mathf.Abs(center.x);
		}
		component.center = center;
	}

	private void EnabledBoxCollider(bool enabled)
	{
		if (this.buildItem != null)
		{
			byte b = 0;
			while ((int)b < this.buildItem.Length)
			{
				if (null != this.buildItem[(int)b])
				{
					this.buildItem[(int)b].EnabledBuildBtn = enabled;
				}
				b += 1;
			}
		}
	}

	private IEnumerator WaitForSecond(DPSelectedItemEventHandler hander, float time)
	{
		if (0f < time)
		{
			yield return new WaitForSeconds(time);
		}
		if (hander != null)
		{
			hander(null, null);
		}
		yield break;
	}

	private IEnumerator WaitForOfFrame()
	{
		yield return new WaitForEndOfFrame();
		this.PetShowDragPanel.onDragFinished.Invoke();
		yield break;
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public void InitBuildMain()
	{
		this.InitPrefabstext();
		this.InitHander();
		this.InitTextTure();
		base.StartCoroutine<bool>(this.InitBuild());
		this.ChangeLevelAwardBtnGanTanHaoState(false);
		base.StartCoroutine<bool>(this.RefreshRolePetShow(Global.GetRolePaiPets(true)));
	}

	private void InitHander()
	{
		this.mOBCPetShow = this.PetShowListBox.ItemsSource;
		this.PetShowDragPanel.onDragIng = delegate()
		{
			float x = this.PetShowDragPanel.currentMomentum.x;
			if (x != 0f)
			{
				this.IsDragIngView = 1;
			}
			this.mPetShowBtnWaiteBtnTime = 1f;
		};
		this.PetShowDragPanel.onDragFinished = delegate()
		{
			this.IsDragIngView = 0;
			bool flag = false;
			bool flag2 = false;
			this.PetShowDragPanel.IsOnLiftOrRight(ref flag, ref flag2);
			if (flag && flag2)
			{
				NGUITools.SetActive(this.PetShowDirectionSign_[0], false);
				NGUITools.SetActive(this.PetShowDirectionSign_[1], false);
			}
			else
			{
				NGUITools.SetActive(this.PetShowDirectionSign_[1], flag);
				NGUITools.SetActive(this.PetShowDirectionSign_[0], flag2);
			}
			this.mPetShowBtnWaiteBtnTime = 1f;
			this.StartCoroutine<bool>(this.WaitForSecond(delegate(object e, DPSelectedItemEventArgs s)
			{
				this.mPetShowBtnWaiteBtnTime = 0f;
			}, this.mPetShowBtnWaiteBtnTime));
		};
		string[] HelpStr = new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("要塞开发")
			}),
			string.Concat(new string[]
			{
				Global.GetLang("1、开发过程为离线进行"),
				Environment.NewLine,
				Global.GetLang("2、建筑开发需要消耗"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("能量核心")
				}),
				Global.GetLang("，建筑可同时"),
				Environment.NewLine,
				Global.GetLang("开发"),
				Environment.NewLine,
				Global.GetLang("3、开发完成后，可领取"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("资源奖励")
				}),
				Global.GetLang("和"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("建筑经验")
				}),
				Environment.NewLine,
				Global.GetLang("4、建筑升级后，产出提高"),
				Environment.NewLine,
				Global.GetLang("5、能量核心可在世界战役中获得"),
				Environment.NewLine,
				Global.GetColorStringForNGUIText(new object[]
				{
					"fac60d",
					Global.GetLang("总等级奖励")
				}),
				Environment.NewLine,
				Global.GetLang("1、建筑总等级达到要求，可领取"),
				Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					Global.GetLang("总等级奖励")
				})
			})
		};
		this.m_HelpOpenBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			Global.ShowHelpPart(HelpStr[0], Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				HelpStr[1]
			}), true, -15f);
		};
		this._LevelAwardBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowLevelAwardWindow(this.transform.parent);
		};
		this._CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			PlayZone.GlobalPlayZone.CloseBuildMain();
		};
		UIEventListener.Get(this.PetShowDirectionSign.gameObject).onClick = delegate(GameObject g)
		{
			SpringPanel component = this.PetShowDragPanel.transform.GetComponent<SpringPanel>();
			if (null != component && component.enabled)
			{
				return;
			}
			if (this.mPetShowBtnWaiteBtnTime != 0f)
			{
				return;
			}
			this.mPetShowBtnWaiteBtnTime = 0.5f;
			this.StartCoroutine<bool>(this.WaitForSecond(delegate(object e, DPSelectedItemEventArgs s)
			{
				this.mPetShowBtnWaiteBtnTime = 0f;
			}, this.mPetShowBtnWaiteBtnTime));
			if (this.mPetShowSign == 0)
			{
				this.SetPetShowSign(1);
			}
			else
			{
				this.SetPetShowSign(0);
			}
		};
		this._BuildRoot.GetComponent<UIDraggablePanel>().onMomentumIng = delegate(Vector3 offset)
		{
		};
		this._BuildRoot.GetComponent<UIDraggablePanel>().onDragIng = delegate()
		{
		};
		this._BuildRoot.GetComponent<UIDraggablePanel>().onDragFinished = delegate()
		{
			if (null != this._BuildBak[0].Texture.mainTexture)
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.BgRoot, this.BgRoot);
				float num = Mathf.Abs(this._BuildRoot.localPosition.x);
				float num2 = (float)(this._BuildBak[0].ItsSizeWidth / 2) - bounds.size.x / 2f;
				if (num > num2 && num2 >= 0f)
				{
					bool flag = true;
					bool flag2 = true;
					this._BuildRoot.GetComponent<UIDraggablePanel>().IsOnLiftOrRight(ref flag, ref flag2);
					if (flag && !flag2)
					{
						SpringPanel.Begin(this._BuildRoot.gameObject, new Vector3(-170f, 0f, 0f), 8f);
					}
					else if (flag2 && !flag)
					{
						SpringPanel.Begin(this._BuildRoot.gameObject, new Vector3(170f, 0f, 0f), 8f);
					}
					else if (0f < this._BuildRoot.localPosition.x)
					{
						SpringPanel.Begin(this._BuildRoot.gameObject, new Vector3(170f, 0f, 0f), 8f);
					}
					else
					{
						SpringPanel.Begin(this._BuildRoot.gameObject, new Vector3(-170f, 0f, 0f), 8f);
					}
				}
			}
		};
	}

	private IEnumerator RefreshRolePetShow(List<GoodsData> lst)
	{
		if (lst != null)
		{
			int count = lst.Count;
			int MaxPetCount = (int)ConfigSystemParam.GetSystemParamIntByName("ManorPetMax");
			if (0 > MaxPetCount)
			{
				MaxPetCount = 15;
			}
			this.PetShowInfLabel.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("派驻精灵") + Global.GetLang("（") + Global.GetColorStringForNGUIText(new object[]
				{
					(lst.Count >= MaxPetCount) ? "ff0000" : "17e43e",
					count.ToString() + "/" + MaxPetCount.ToString()
				}) + Global.GetLang("）")
			});
			if (count < this.mOBCPetShow.Count)
			{
				count = this.mOBCPetShow.Count;
			}
			for (int i = 0; i < count; i++)
			{
				if (i < lst.Count)
				{
					GoodsData da = lst[i];
					if (da != null)
					{
						GGoodIcon icon = this.GetGoodsIcon(i, da);
						Vector3 pos = icon.transform.localPosition;
						pos.z = -5f;
						icon.transform.localPosition = pos;
					}
					else
					{
						this.mOBCPetShow.RemoveAt(i);
					}
				}
				else
				{
					this.mOBCPetShow.RemoveAt(i);
				}
				if (i < 10)
				{
					if (i > 0 && i % 4 == 0)
					{
						yield return null;
					}
				}
				else
				{
					yield return null;
				}
			}
			base.StartCoroutine<bool>(this.WaitForOfFrame());
			this.PetShowFixPos();
		}
		yield break;
	}

	private GGoodIcon GetGoodsIcon(int index, GoodsData gd)
	{
		GameObject at = this.mOBCPetShow.GetAt(index);
		byte b = 0;
		GGoodIcon ggoodIcon;
		if (null == at)
		{
			ggoodIcon = this.initGood(gd, true);
			b = 1;
		}
		else
		{
			ggoodIcon = at.GetComponent<GGoodIcon>();
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = gd;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, gd, Global.CanUseGoods(gd.GoodsID, false, true), IconTextTypes.Qianghua);
			if (Global.GetZhuoyueAttributeCount(gd) == 0)
			{
				ggoodIcon.TeXiao.gameObject.SetActive(false);
			}
		}
		if (gd.Site == 10001)
		{
			ggoodIcon.ContentText.Label.supportEncoding = true;
			ggoodIcon.ContentText.Label.pivot = 4;
			ggoodIcon.ContentText.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				Global.GetLang("任务中")
			});
			ggoodIcon.ContentText.transform.localScale = Vector3.one * 16f;
			Vector3 localPosition = ggoodIcon.ContentText.transform.localPosition;
			localPosition.y = -15f;
			localPosition.x = 0f;
			ggoodIcon.ContentText.transform.localPosition = localPosition;
			ggoodIcon.NoUseSpriteVisible = true;
			ggoodIcon.NoUseSprite.spriteName = "iconState_nouse3";
			ggoodIcon.NoUseSprite.alpha = 0.65f;
			ggoodIcon.NoUseSprite.transform.localPosition = new Vector3(ggoodIcon.NoUseSprite.transform.localPosition.x, ggoodIcon.NoUseSprite.transform.localPosition.y, -0.1f);
		}
		else
		{
			ggoodIcon.ContentText.Text = string.Empty;
			ggoodIcon.NoUseSpriteVisible = false;
		}
		UIDragPanelContents uidragPanelContents = ggoodIcon.GetComponent<UIDragPanelContents>();
		if (null == uidragPanelContents)
		{
			uidragPanelContents = ggoodIcon.gameObject.AddComponent<UIDragPanelContents>();
		}
		uidragPanelContents.draggablePanel = this.PetShowDragPanel;
		if (b == 1)
		{
			this.mOBCPetShow.AddNoUpdate(ggoodIcon);
		}
		UIPanel component = ggoodIcon.GetComponent<UIPanel>();
		if (null != component)
		{
			Object.Destroy(component);
		}
		ggoodIcon.DPSelectedItem = delegate(object e, DPSelectedItemEventArgs s)
		{
			if (s.IDType == 17)
			{
				GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, 0, gd.GCount, gd.BagIndex, string.Empty);
			}
			else if (s.IDType == 15)
			{
				if (Global.Data.equipPet != null && 4 <= Global.Data.equipPet.Count)
				{
					Super.HintMainText(Global.GetLang("精灵栏位已满"), 10, 3);
				}
				else
				{
					GameInstance.Game.SpriteModGoods(3, gd.Id, gd.GoodsID, gd.Using, 5000, gd.GCount, gd.BagIndex, string.Empty);
				}
			}
		};
		return ggoodIcon;
	}

	private GGoodIcon initGood(GoodsData data, bool BHaveTips = true)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(data.GoodsID);
		GGoodIcon ggoodIcon = null;
		if (goodsXmlNodeByID != null)
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.SecondText.transform.localScale = new Vector3(12f, 12f, 1f);
			ggoodIcon.Width = 50.0;
			ggoodIcon.Height = 50.0;
			ggoodIcon.ItemObject = data;
			ggoodIcon.ItemCode = goodsXmlNodeByID.ID;
			ggoodIcon.TipType = 1;
			ggoodIcon.ItemCategory = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon.BodyURL = new ImageURL(string.Format("NetImages/GameRes/{0}", goodsImageURLFromIconCode), false, 0);
			NGUITools.SetActive(ggoodIcon.BackgroundSprite0, true);
			ggoodIcon.BackgroundSprite0.spriteName = "bagGrid4_bak";
			Super.InitGoodsGIcon(ggoodIcon, data, Global.CanUseGoods(goodsXmlNodeByID.ID, false, true), IconTextTypes.Qianghua);
			if (BHaveTips)
			{
				ggoodIcon.MouseLeftButtonUp = delegate(object e, MouseEvent s)
				{
					this.ShowGoodsTip(e);
				};
			}
		}
		return ggoodIcon;
	}

	public void ShowGoodsTip(object icon)
	{
		if (this.IsDragIngView == 1)
		{
			return;
		}
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.jingLingYaoSai, goodData);
	}

	private void InitPrefabstext()
	{
	}

	private void InitTextTure()
	{
		this._BuildBak[0].URL = "NetImages/GameRes/Images/Build/BUildMain/lingdi_bak.jpg";
		this._BuildBak[0].ImageDownloaded = delegate(object o)
		{
			if (null != this._BuildBak[0].Texture && null != this._BuildBak[0].Texture.mainTexture)
			{
				this._BuildBak[0].transform.localScale = new Vector3((float)this._BuildBak[0].ItsSizeWidth, (float)this._BuildBak[0].ItsSizeHeight, 1f);
			}
			BoxCollider boxCollider = this._BuildBak[0].transform.GetComponent<BoxCollider>();
			if (null == boxCollider)
			{
				boxCollider = this._BuildBak[0].gameObject.AddComponent<BoxCollider>();
			}
			boxCollider.size = Vector3.one;
		};
	}

	private void BgDragHander(GameObject go, Vector2 delta)
	{
		Vector3 relative;
		relative..ctor(delta.x, 0f, 0f);
		if (20f < Mathf.Abs(relative.x))
		{
			relative.x = relative.x / Mathf.Abs(relative.x) * 20f;
		}
		this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + new Vector3(relative.x, 0f, 0f) * 0.35f, 0.67f);
		this.MoveRelative(relative);
	}

	private void LateUpdate()
	{
	}

	public void MoveAbsolute(Vector3 absolute)
	{
		Vector3 vector = this._BuildBak[0].transform.InverseTransformPoint(absolute);
		Vector3 vector2 = this._BuildBak[0].transform.InverseTransformPoint(Vector3.zero);
		this.MoveRelative(vector - vector2);
	}

	public void MoveRelative(Vector3 relative)
	{
		byte b;
		if (0f < relative.x)
		{
			b = 0;
		}
		else
		{
			b = 1;
		}
		relative = NGUITools.Round(relative);
		if (this.mDirection == 2)
		{
			this.mDirection = b;
		}
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.BgRoot, this.BgRoot);
		float num = Mathf.Abs(this._BuildBak[0].transform.localPosition.x);
		float num2 = this._BuildBak[0].transform.localScale.x / 2f - bounds.size.x / 2f;
		if (Mathf.Abs(relative.x) > num2 - num && b == this.mDirection)
		{
			relative.x = relative.x / Mathf.Abs(relative.x) * (num2 - num);
		}
		if (num < num2 || b != this.mDirection)
		{
			this._BuildRoot.localPosition += relative;
			this.mDirection = b;
		}
	}

	private IEnumerator InitBuild()
	{
		if (this.mConfigBuildXml == null)
		{
			this.mConfigBuildXml = new ConfigBuildXml();
		}
		List<BuildVO> argsList = this.mConfigBuildXml.GetBuildVOList();
		Super.ShowNetWaiting(null);
		this.buildItem = new BuildItem[argsList.Count];
		byte i = 0;
		while ((int)i < argsList.Count)
		{
			if (!BuildMainPart.ldName.ContainsKey(argsList[(int)i].ID))
			{
				BuildMainPart.ldName.Add(argsList[(int)i].ID, argsList[(int)i].Name);
			}
			if (null != this.buildItem[(int)i])
			{
				Object.Destroy(this.buildItem[(int)i].gameObject);
				this.buildItem[(int)i] = null;
			}
			this.buildItem[(int)i] = U3DUtils.NEW<BuildItem>();
			this.buildItem[(int)i].transform.SetParent(this._BuildRoot, false);
			this.buildItem[(int)i].ID = argsList[(int)i].ID;
			this.buildItem[(int)i].buildState = BuildState.KongXian;
			this.buildItem[(int)i].MaxLev = argsList[(int)i].MaxLevel;
			this.buildItem[(int)i].Hander = new DPSelectedItemEventHandler(this.BuildClick);
			this.buildItem[(int)i].MPC = this._BuildRoot.GetComponent<UIDraggablePanel>();
			yield return null;
			i += 1;
		}
		Super.HideNetWaiting();
		Super.ShowNetWaiting(null);
		GameInstance.Game.BuildGetLingDiInfo();
		GameInstance.Game.SendGetBuildLevelAwardState();
		yield break;
	}

	private void BuildClick(object sender, DPSelectedItemEventArgs args)
	{
		if (args != null)
		{
			int id = args.ID;
			if (args.Type == 1)
			{
				switch (id)
				{
				case 5:
					if (Global.Data.CurrentCopyTeamData != null)
					{
						Global.ZuDuiFuBenTeam(delegate(object s2, DPSelectedItemEventArgs e2)
						{
							if (e2.ID == 0)
							{
								GameInstance.Game.SpriteCopyTeam(TeamCmds.Quit, 0L, 0, 0, 0);
								if (Global.GetMapSceneUIClass() != SceneUIClasses.Normal)
								{
									GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("进入世界战役将会离开当前场景，是否确认前往"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
									messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
									{
										int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
										Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
										if (messageBoxReturn == 0)
										{
											JingLingMap.inst.OpenWorldBattle(JingLingMap.JingLingMapType.MyHome);
										}
										return true;
									};
								}
								else
								{
									JingLingMap.inst.OpenWorldBattle(JingLingMap.JingLingMapType.MyHome);
								}
							}
						}, -1);
					}
					else if (Global.GetMapSceneUIClass() != SceneUIClasses.Normal)
					{
						GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetLang("进入世界战役将会离开当前场景，是否确认前往"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
							Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								JingLingMap.inst.OpenWorldBattle(JingLingMap.JingLingMapType.MyHome);
							}
							return true;
						};
					}
					else
					{
						JingLingMap.inst.OpenWorldBattle(JingLingMap.JingLingMapType.MyHome);
					}
					break;
				case 6:
					JingLingMapEvent.ProcessEvent(EmJingMapEvent.FromJingLingMap_ShenJi);
					break;
				case 7:
					if (this.buildItem[BuildMainPart.BuildIDToArrayId(id)].buildState == BuildState.KongXian)
					{
						Super.HintMainText(Global.GetLang("请先在世界战役里创建要塞！"), 10, 3);
					}
					else
					{
						JingLingMapEvent.ProcessEvent(EmJingMapEvent.FromJingLingMap_Prison);
					}
					break;
				}
			}
			else if (args.Type == 11)
			{
				Vector2 vector;
				vector..ctor(float.Parse(args.Title), 0f);
			}
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public void ChangeLevelAwardBtnGanTanHaoState(bool bShow = true)
	{
		if (bShow != this._GanTanHao.activeSelf)
		{
			NGUITools.SetActive(this._GanTanHao, bShow);
		}
	}

	public void UpdateBuildData(List<BuildData> dataList)
	{
		if (dataList == null)
		{
			return;
		}
		byte b = 0;
		while ((int)b < dataList.Count)
		{
			if ((int)b < this.buildItem.Length)
			{
				this.buildItem[BuildMainPart.BuildIDToArrayId(dataList[(int)b].BuildID)].Lev = dataList[(int)b].Lev;
				this.buildItem[BuildMainPart.BuildIDToArrayId(dataList[(int)b].BuildID)].EXP = dataList[(int)b].Exp;
				this.buildItem[BuildMainPart.BuildIDToArrayId(dataList[(int)b].BuildID)].DevelopTime = dataList[(int)b].DevelopTime;
			}
			b += 1;
		}
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendGetBuildState();
	}

	public void UpdataBuildData_Task(int BuildId, int task1, int task2, int task3)
	{
	}

	public void UpdateBuildTeamData(List<BuildTeamData> datalist)
	{
		if (datalist != null && 0 < datalist.Count)
		{
			byte b = 0;
			while ((int)b < datalist.Count)
			{
				if (datalist[(int)b] != null && datalist[(int)b].HaveTask)
				{
					this.buildItem[BuildMainPart.BuildIDToArrayId(datalist[(int)b].buildid)].Build_TaskId = datalist[(int)b].taskId;
				}
				b += 1;
			}
		}
	}

	public void UpDateGetBuildAwardsuccessful(int buildId, int BuildLev, int EXP)
	{
		this.buildItem[BuildMainPart.BuildIDToArrayId(buildId)].buildState = BuildState.KongXian;
		this.buildItem[BuildMainPart.BuildIDToArrayId(buildId)].EXP += EXP;
		this.buildItem[BuildMainPart.BuildIDToArrayId(buildId)].Lev = BuildLev;
		GameInstance.Game.SendGetTeamState();
	}

	public void UpdateOneBuildData(BuildData data)
	{
		int num = BuildMainPart.BuildIDToArrayId(data.BuildID);
		this.buildItem[num].EXP = data.Exp;
		this.buildItem[num].ID = data.BuildID;
		this.buildItem[num].Lev = data.Lev;
		this.buildItem[num].DevelopTime = data.DevelopTime;
		if (this.buildItem[num].buildState == BuildState.KongXian)
		{
			this.buildItem[num].Build_TaskId = 0;
		}
	}

	public void UpdateBuild_ZhiXing(int buildId, int taskId, int TeamId)
	{
		XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildTask.xml");
		if (gameResXml != null)
		{
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildTask");
			foreach (XElement xelement in xelementList)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ID");
				if (taskId == xelementAttributeInt)
				{
					int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "Time");
					break;
				}
			}
		}
		this.buildItem[BuildMainPart.BuildIDToArrayId(buildId)].Build_TaskId = taskId;
		this.buildItem[BuildMainPart.BuildIDToArrayId(buildId)].buildState = BuildState.GongZuo;
		this.buildItem[BuildMainPart.BuildIDToArrayId(buildId)].DevelopTime = this.DateTimeToMyString(Global.GetCorrectDateTime());
		this.ClickedBuild = buildId;
	}

	public void UpdateBuild_QuickFinishtask(int BuildId, int TeamId, int BuildLev, int EXP)
	{
		this.buildItem[BuildMainPart.BuildIDToArrayId(BuildId)].buildState = BuildState.KongXian;
		this.buildItem[BuildMainPart.BuildIDToArrayId(BuildId)].Build_TaskId = 0;
		this.buildItem[BuildMainPart.BuildIDToArrayId(BuildId)].Lev = BuildLev;
		this.buildItem[BuildMainPart.BuildIDToArrayId(BuildId)].EXP = EXP;
	}

	public void UpdateBuildState(List<int> BuildId, List<int> buildState)
	{
		if (BuildId.Count == buildState.Count)
		{
			byte b = 0;
			while ((int)b < BuildId.Count)
			{
				if ((int)b < this.buildItem.Length)
				{
					this.buildItem[BuildMainPart.BuildIDToArrayId(BuildId[(int)b])].buildState = (BuildState)buildState[(int)b];
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						string.Concat(new object[]
						{
							"<color=red>",
							this.buildItem.Length,
							"::",
							BuildId.Count,
							"</color>"
						})
					});
				}
				b += 1;
			}
		}
		Super.ShowNetWaiting(null);
		GameInstance.Game.SendGetTeamState();
	}

	public static int BuildIDToArrayId(int buildId)
	{
		return buildId - 1;
	}

	private string DateTimeToMyString(DateTime DTime)
	{
		string empty = string.Empty;
		return string.Concat(new object[]
		{
			DTime.Year.ToString(),
			"-",
			DTime.Month.ToString(),
			"-",
			DTime.Day,
			" ",
			DTime.Hour.ToString(),
			":",
			DTime.Minute.ToString(),
			":",
			DTime.Second.ToString()
		});
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BuildMainPart.ldName.Clear();
		byte b = 0;
		while ((int)b < this.buildItem.Length)
		{
			this.buildItem[(int)b] = null;
			b += 1;
		}
	}

	private void OnEnable()
	{
		if (this.InitByOnEnabel == 1)
		{
			this.InitByOnEnabel += 1;
		}
		if (this.InitByOnEnabel == 2)
		{
			this.InitBuildMain();
		}
	}

	public static Dictionary<int, string> LDName
	{
		get
		{
			return BuildMainPart.ldName;
		}
	}

	public static string[] GetLdHintTitileName()
	{
		return new string[]
		{
			"shengWang",
			"moJing",
			"xingHun",
			"chengJiu",
			"chengJiu",
			"chengJiu"
		};
	}

	public byte DirectionSignDirectionValue
	{
		set
		{
			if (null != this.PetShowDirectionSign)
			{
				if (value == 0)
				{
					this.PetShowDirectionSign.pivot = 7;
					this.PetShowDirectionSign.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
					this.PetShowDirectionSign.transform.localPosition = new Vector3(0f, -21f, -0.1f);
				}
				else
				{
					this.PetShowDirectionSign.pivot = 1;
					this.PetShowDirectionSign.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
					this.PetShowDirectionSign.transform.localPosition = new Vector3(0f, 38f, -0.1f);
				}
			}
		}
	}

	public void ShowLevelAwardWindow(Transform parent)
	{
		if (null != this.buildLevelAwardWindow)
		{
			this.buildLevelAwardWindow.Visibility = true;
		}
		else
		{
			this.buildLevelAwardWindow = U3DUtils.NEW<GChildWindow>();
			this.buildLevelAwardWindow.transform.localPosition = new Vector3(0f, 0f, -300f);
			this.buildLevelAwardWindow.transform.SetParent(parent, false);
			this.buildLevelAwardWindow.IsShowModal = true;
		}
		this.buildLevelAward = U3DUtils.NEW<BuildLevelAwardPart>();
		this.buildLevelAward.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.buildLevelAward.transform.SetParent(this.buildLevelAwardWindow.Body.transform, false);
		this.buildLevelAward._BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseLevelAwardwindow();
		};
	}

	private void CloseLevelAwardwindow()
	{
		if (null != this.buildLevelAward)
		{
			GameObject gameObject = this.buildLevelAward.gameObject.transform.parent.parent.gameObject;
			Object.Destroy(this.buildLevelAward);
			if (null != gameObject)
			{
				Object.Destroy(gameObject);
			}
			this.buildLevelAward = null;
			this.buildLevelAwardWindow = null;
		}
	}

	public void UpDateAwardState(List<int> datalist)
	{
		if (null != this.buildLevelAward)
		{
			this.buildLevelAward.UpdataAwardState(datalist, this.GetAllBuildLev());
		}
		else
		{
			XElement gameResXml = Global.GetGameResXml("Config/Manor/BuildLevelAward.xml");
			if (gameResXml != null)
			{
				List<XElement> xelementList = Global.GetXElementList(gameResXml, "BuildLevelAward");
				if (0 >= datalist.Count)
				{
					int xelementAttributeInt = Global.GetXElementAttributeInt(xelementList[0], "AllLevel");
					if (xelementAttributeInt <= this.GetAllBuildLev())
					{
						NGUITools.SetActive(this._GanTanHao, true);
					}
					else
					{
						NGUITools.SetActive(this._GanTanHao, false);
					}
				}
				else
				{
					for (int i = 0; i < xelementList.Count; i++)
					{
						if (datalist[datalist.Count - 1] + 1 == Global.GetXElementAttributeInt(xelementList[i], "ID"))
						{
							int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelementList[i], "AllLevel");
							if (xelementAttributeInt2 <= this.GetAllBuildLev())
							{
								NGUITools.SetActive(this._GanTanHao, true);
							}
							else
							{
								NGUITools.SetActive(this._GanTanHao, false);
							}
							break;
						}
					}
				}
			}
		}
	}

	public void RefreshPaiZhuData(List<GoodsData> list)
	{
		base.StartCoroutine<bool>(this.RefreshRolePetShow(list));
	}

	public void UpDateGetAwardState(int AwardId)
	{
		this.buildLevelAward.UpDateGetAwardState(AwardId);
		this.CheckLeveAwardState();
	}

	private void CheckLeveAwardState()
	{
		this.ChangeLevelAwardBtnGanTanHaoState(this.buildLevelAward.HaveAwardCanGet());
	}

	private int GetAllBuildLev()
	{
		int num = 0;
		for (int i = 0; i < this.buildItem.Length; i++)
		{
			if (null != this.buildItem[i])
			{
				if (4 >= this.buildItem[i].ID)
				{
					num += this.buildItem[i].Lev;
				}
			}
		}
		return num;
	}

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private GButton _LevelAwardBtn;

	[SerializeField]
	private GameObject _GanTanHao;

	[SerializeField]
	private ShowNetImage[] _BuildBak = new ShowNetImage[2];

	[SerializeField]
	private Transform _BuildRoot;

	[SerializeField]
	private GButton m_HelpOpenBtn;

	[SerializeField]
	private Transform PetShowRoot;

	[SerializeField]
	private GameObject PetShowInfRoot;

	[SerializeField]
	private UIDraggablePanel PetShowDragPanel;

	[SerializeField]
	private ListBox PetShowListBox;

	[SerializeField]
	private UISprite PetShowDirectionSign;

	[SerializeField]
	private UILabel PetShowInfLabel;

	[SerializeField]
	private UISprite PetShowInfBg;

	[SerializeField]
	private UISprite[] PetShowDirectionSign_;

	[SerializeField]
	private Transform BgRoot;

	private static Dictionary<int, string> ldName = new Dictionary<int, string>();

	private string[] BtnStr = new string[]
	{
		Global.GetLang("领取奖励"),
		Global.GetLang("开启"),
		Global.GetLang("一键完成")
	};

	protected BuildLevelAwardPart buildLevelAward;

	protected GChildWindow buildLevelAwardWindow;

	private BuildItem[] buildItem;

	private int ClickedBuild;

	private ObservableCollection mOBCPetShow;

	private byte mPetShowSign;

	private int mPetShowFixPosGoodsID;

	private ConfigBuildXml mConfigBuildXml;

	public byte InitByOnEnabel;

	private float mPetShowBtnWaiteBtnTime;

	private byte IsDragIngView;

	private byte mDirection = 2;

	private Vector3 mMomentum = Vector3.zero;
}
