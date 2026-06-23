using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class RebirthBossPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		this.InitBoss();
		this.InitLines();
		this.CloseJiangLiYuLan();
		this.ClosegetAward();
		if (null != this.mSelectActivityBossItem)
		{
			Super.ShowNetWaiting(null);
			GameInstance.Game.SendRoleGetRebirthBossdata(this.mSelectActivityBossItem.MapCode);
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>mSelectActivityBossItem = null </color>"
			});
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.mLoader != null)
		{
			this.mLoader.Stop();
		}
		this.StopTimer();
	}

	public GGoodIcon AddGoodIcon(GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		GGoodIcon ggoodIcon = U3DUtils.NEW<GGoodIcon>();
		ggoodIcon.Width = 78.0;
		ggoodIcon.Height = 78.0;
		ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		ggoodIcon.TipType = 1;
		ggoodIcon.ItemCode = gd.GoodsID;
		ggoodIcon.ItemObject = gd;
		ggoodIcon.BoxTypes = 0;
		ggoodIcon.TextSize = 16;
		ggoodIcon.TextShadowColor = 4278190080U;
		ggoodIcon.Tag = gd.ExcellenceInfo;
		ggoodIcon.SecondText.Text = gd.GCount.ToString();
		ggoodIcon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(ggoodIcon, gd, true, IconTextTypes.Qianghua);
		ggoodIcon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2.5f);
		ggoodIcon.addEventListener("click", new MouseEventHandler(this.MouseLeftButtonUp));
		if (Global.GetZhuoyueAttributeCount(gd) >= 5)
		{
			ggoodIcon.TeXiao.gameObject.SetActive(true);
		}
		return ggoodIcon;
	}

	private void MouseLeftButtonUp(MouseEvent evt)
	{
		GGoodIcon ggoodIcon = evt.target.SafeGetComponent<GGoodIcon>();
		if (null != ggoodIcon && ggoodIcon.ItemObject != null)
		{
			GoodsData goodsData = ggoodIcon.ItemObject as GoodsData;
			if (goodsData != null)
			{
				GTipServiceEx.SelfBagOnly = false;
				GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodsData);
			}
		}
	}

	private void ShowJiangLiYuLan(int monsterID)
	{
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		this._AwardYuLanRoot.SetActive(true);
		List<RebornBossAwardVO> awardsByListMonsterID = IConfigbase<ConfigRebirth>.Instance.GetAwardsByListMonsterID(monsterID, false);
		if (awardsByListMonsterID != null)
		{
			this._AwardYULanGoodsRoot.gameObject.SetActive(false);
			this._AwardYULanGoodsTitle.gameObject.SetActive(false);
			for (int i = 0; i < awardsByListMonsterID.Count; i++)
			{
				Transform transform = Object.Instantiate<GameObject>(this._AwardYULanGoodsRoot.gameObject).transform;
				transform.gameObject.SetActive(true);
				transform.SetParent(this._AwardYULanGoodsRoot.parent, false);
				UILabel uilabel = NGUITools.AddWidget<UILabel>(this._AwardYULanGoodsTitle.transform.parent.gameObject);
				uilabel.Margin = this._AwardYULanGoodsTitle.Margin;
				uilabel.font = this._AwardYULanGoodsTitle.font;
				uilabel.transform.localScale = this._AwardYULanGoodsTitle.transform.localScale;
				uilabel.color = this._AwardYULanGoodsTitle.color;
				List<GoodsData> list = new List<GoodsData>();
				if (!string.IsNullOrEmpty(awardsByListMonsterID[i].GoodsOne))
				{
					string[] array = awardsByListMonsterID[i].GoodsOne.Split(new char[]
					{
						'|'
					});
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array[j], 0);
							if (goodsDataByStr != null)
							{
								list.Add(goodsDataByStr);
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(awardsByListMonsterID[i].GoodsTwo))
				{
					string[] array2 = awardsByListMonsterID[i].GoodsTwo.Split(new char[]
					{
						'|'
					});
					if (array2 != null)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							GoodsData goodsDataByStr2 = Global.GetGoodsDataByStr(array2[k], 0);
							if (!MUJieripartChongzhiKingItem.IsTongGuo(goodsDataByStr2.GoodsID.ToString(), roleOcc) && goodsDataByStr2 != null)
							{
								list.Add(goodsDataByStr2);
							}
						}
					}
				}
				for (int l = 0; l < list.Count; l++)
				{
					GGoodIcon icon = this.AddGoodIcon(list[l]);
					icon.transform.SetParent(transform, false);
					icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
						if (null == ggoodIcon)
						{
							return;
						}
						GoodsData goodsData = icon.ItemObject as GoodsData;
						if (goodsData == null)
						{
							return;
						}
						GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
					};
				}
				this.UpdataTransPos(transform);
				uilabel.text = Global.GetColorStringForNGUIText(new object[]
				{
					"cca06b",
					awardsByListMonsterID[i].Name
				});
				transform.transform.localPosition = new Vector3(0f, (float)(140 + -130 * i), 0f);
				uilabel.transform.localPosition = new Vector3(0f, (float)(200 + -130 * i), 0f);
				this.mAwardGoodsRootAndLabels.Add(uilabel.gameObject);
				this.mAwardGoodsRootAndLabels.Add(transform.gameObject);
			}
		}
		UIDraggablePanel uidraggablePanel = NGUITools.FindInParents<UIDraggablePanel>(this._AwardYULanUICollider.gameObject);
		SpringPanel.Begin(uidraggablePanel.gameObject, new Vector3(0f, 0f, 0f), 10f);
		base.StartCoroutine<bool>(this.ResetBoxCollider(this._AwardYULanUICollider));
	}

	private void DestoryAllChild(Transform trans)
	{
		if (null != trans && 0 < trans.childCount)
		{
			for (int i = trans.childCount - 1; i >= 0; i--)
			{
				Transform child = trans.GetChild(i);
				if (null != child)
				{
					Object.Destroy(child.gameObject);
				}
			}
		}
	}

	private void UpdataTransPos(Transform trans)
	{
		if (null != trans && 0 < trans.childCount)
		{
			if (trans.childCount != 1)
			{
				int num = 0;
				int childCount = trans.childCount;
				if (childCount % 2 != 0)
				{
					num = -45;
				}
				for (int i = 0; i < childCount; i++)
				{
					Transform child = trans.GetChild(i);
					if (i % 2 == 0)
					{
						child.transform.localPosition = new Vector3((float)(45 + i * 45 + num), 0f, 0f);
					}
					else
					{
						child.transform.localPosition = new Vector3((float)(0 - i * 45 + num), 0f, 0f);
					}
				}
			}
		}
	}

	private void ClosegetAward()
	{
		this._GetAwardRoot.SetActive(false);
		this.DestoryAllChild(this._GetAwardGoodsRoot);
		this.DestoryAllChild(this._GetLastAttAwardGoodsRoot);
	}

	private void CloseJiangLiYuLan()
	{
		this._AwardYuLanRoot.SetActive(false);
		for (int i = 0; i < this.mAwardGoodsRootAndLabels.size; i++)
		{
			if (null != this.mAwardGoodsRootAndLabels[i])
			{
				Object.Destroy(this.mAwardGoodsRootAndLabels[i]);
			}
		}
	}

	private void ShowGetAward(int Rood, int BossID, int rank, int BossKill = 0)
	{
		byte b = 0;
		MonsterVO monsterXmlNodeByID = ConfigMonsters.GetMonsterXmlNodeByID(BossID);
		if (monsterXmlNodeByID != null)
		{
			this._GetAwardMyRankLabel.text = string.Concat(new object[]
			{
				monsterXmlNodeByID.SName,
				" ",
				Global.GetLang("我的排名："),
				rank
			});
		}
		else
		{
			this._GetAwardMyRankLabel.text = Global.GetLang("我的排名：") + rank;
		}
		int roleOcc = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		this._GetAwardRoot.SetActive(true);
		List<RebornBossAwardVO> awardsByListMonsterID = IConfigbase<ConfigRebirth>.Instance.GetAwardsByListMonsterID(BossID, true);
		if (awardsByListMonsterID != null && 0 < awardsByListMonsterID.Count)
		{
			if (0 >= rank && awardsByListMonsterID[awardsByListMonsterID.Count - 1].EndNum != -1)
			{
				this._GetAwardMyRankLabel.text = string.Empty;
			}
			else if (0 >= rank && awardsByListMonsterID[awardsByListMonsterID.Count - 1].EndNum == -1)
			{
				this._GetAwardMyRankLabel.text = Global.GetLang("未上榜");
			}
			RebornBossAwardVO rebornBossAwardVO = null;
			for (int i = 0; i < awardsByListMonsterID.Count; i++)
			{
				if (awardsByListMonsterID[i].BeginNum <= rank && awardsByListMonsterID[i].EndNum >= rank)
				{
					rebornBossAwardVO = awardsByListMonsterID[i];
					break;
				}
			}
			if (rebornBossAwardVO == null && -1 < rank && awardsByListMonsterID[awardsByListMonsterID.Count - 1].EndNum == -1)
			{
				rebornBossAwardVO = awardsByListMonsterID[awardsByListMonsterID.Count - 1];
			}
			if (rebornBossAwardVO != null)
			{
				List<GoodsData> list = new List<GoodsData>();
				if (!string.IsNullOrEmpty(rebornBossAwardVO.GoodsOne))
				{
					string[] array = rebornBossAwardVO.GoodsOne.Split(new char[]
					{
						'|'
					});
					if (array != null)
					{
						for (int j = 0; j < array.Length; j++)
						{
							GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array[j], 0);
							if (goodsDataByStr != null)
							{
								list.Add(goodsDataByStr);
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(rebornBossAwardVO.GoodsTwo))
				{
					string[] array2 = rebornBossAwardVO.GoodsTwo.Split(new char[]
					{
						'|'
					});
					if (array2 != null)
					{
						for (int k = 0; k < array2.Length; k++)
						{
							GoodsData goodsDataByStr2 = Global.GetGoodsDataByStr(array2[k], 0);
							if (!MUJieripartChongzhiKingItem.IsTongGuo(goodsDataByStr2.GoodsID.ToString(), roleOcc) && goodsDataByStr2 != null)
							{
								list.Add(goodsDataByStr2);
							}
						}
					}
				}
				for (int l = 0; l < list.Count; l++)
				{
					GGoodIcon icon = this.AddGoodIcon(list[l]);
					icon.transform.SetParent(this._GetAwardGoodsRoot, false);
					icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
						if (null == ggoodIcon)
						{
							return;
						}
						GoodsData goodsData = icon.ItemObject as GoodsData;
						if (goodsData == null)
						{
							return;
						}
						GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
					};
				}
				this.UpdataTransPos(this._GetAwardGoodsRoot);
				b = 1;
			}
			else if (BossKill == 0)
			{
				Super.HintMainText(Global.GetLang("当前排名：") + rank + Global.GetLang("暂无奖励可领"), 10, 3);
				this.ClosegetAward();
			}
		}
		if (BossKill == 1)
		{
			this._GetLastAttRoot.gameObject.SetActive(true);
			RebornBossAwardVO rebornBossAwardVOLastAttAward = IConfigbase<ConfigRebirth>.Instance.GetRebornBossAwardVOLastAttAward(BossID);
			if (rebornBossAwardVOLastAttAward != null)
			{
				this._GetAwardLastAttLabel.text = rebornBossAwardVOLastAttAward.Name;
				List<GoodsData> list2 = new List<GoodsData>();
				if (!string.IsNullOrEmpty(rebornBossAwardVOLastAttAward.GoodsOne))
				{
					string[] array3 = rebornBossAwardVOLastAttAward.GoodsOne.Split(new char[]
					{
						'|'
					});
					if (array3 != null)
					{
						for (int m = 0; m < array3.Length; m++)
						{
							GoodsData goodsDataByStr3 = Global.GetGoodsDataByStr(array3[m], 0);
							if (goodsDataByStr3 != null)
							{
								list2.Add(goodsDataByStr3);
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(rebornBossAwardVOLastAttAward.GoodsTwo))
				{
					string[] array4 = rebornBossAwardVOLastAttAward.GoodsTwo.Split(new char[]
					{
						'|'
					});
					if (array4 != null)
					{
						for (int n = 0; n < array4.Length; n++)
						{
							GoodsData goodsDataByStr4 = Global.GetGoodsDataByStr(array4[n], 0);
							if (!MUJieripartChongzhiKingItem.IsTongGuo(goodsDataByStr4.GoodsID.ToString(), roleOcc) && goodsDataByStr4 != null)
							{
								list2.Add(goodsDataByStr4);
							}
						}
					}
				}
				for (int num = 0; num < list2.Count; num++)
				{
					GGoodIcon icon = this.AddGoodIcon(list2[num]);
					icon.transform.SetParent(this._GetLastAttAwardGoodsRoot, false);
					icon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
					{
						GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
						if (null == ggoodIcon)
						{
							return;
						}
						GoodsData goodsData = icon.ItemObject as GoodsData;
						if (goodsData == null)
						{
							return;
						}
						GTipServiceEx.ShowTip(icon, TipTypes.GoodsText, GoodsOwnerTypes.None, goodsData);
					};
				}
				this.UpdataTransPos(this._GetLastAttAwardGoodsRoot);
				if (b == 1)
				{
					b = 2;
				}
				else
				{
					b = 3;
				}
			}
		}
		else
		{
			this._GetLastAttRoot.gameObject.SetActive(false);
		}
		if (b == 1)
		{
			this._GetRankRoot.localPosition = new Vector3(0f, -10f, 0f);
			this._GetAwardBak.localScale = new Vector3(482f, 250f, 1f);
			this._GetAwardBtn.transform.localPosition = new Vector3(0f, -95f, 0f);
			this._GetAwardRoot.transform.localPosition = new Vector3(0f, 0f, -400f);
		}
		else if (b == 2)
		{
			this._GetLastAttRoot.localPosition = new Vector3(0f, -16f, 0f);
			this._GetRankRoot.localPosition = new Vector3(0f, -150f, 0f);
			this._GetAwardBak.localScale = new Vector3(482f, 400f, 1f);
			this._GetAwardBtn.transform.localPosition = new Vector3(0f, -252f, 0f);
			this._GetAwardRoot.transform.localPosition = new Vector3(0f, 75f, -400f);
		}
		else if (b == 3)
		{
			this._GetLastAttRoot.localPosition = new Vector3(0f, -10f, 0f);
			this._GetAwardBak.localScale = new Vector3(482f, 250f, 1f);
			this._GetAwardBtn.transform.localPosition = new Vector3(0f, -95f, 0f);
			this._GetAwardRoot.transform.localPosition = new Vector3(0f, 0f, -400f);
		}
		base.StartCoroutine<bool>(this.ResetBoxCollider(this._GetAwardUICollider));
		base.StartCoroutine<bool>(this.ResetBoxCollider(this._GetAwardUIColliderLastAtt));
	}

	private IEnumerator ResetBoxCollider(UICollider _UICollider)
	{
		_UICollider.updataNow = true;
		yield return null;
		BoxCollider[] boxs = _UICollider.GetComponentsInChildren<BoxCollider>();
		if (boxs != null)
		{
			for (int i = 0; i < boxs.Length; i++)
			{
				if (boxs != null && !boxs[i].gameObject.Equals(_UICollider.gameObject))
				{
					yield return null;
					Vector3 center = boxs[i].center;
					center.z = _UICollider.box.center.z - 0.1f;
					boxs[i].center = center;
				}
			}
		}
		Vector3 Size = _UICollider.box.size;
		Size.x *= 2f;
		_UICollider.box.size = Size;
		yield break;
	}

	private void StopTimer()
	{
		if (this.mUITimer != null)
		{
			this.mUITimer.Tick = null;
			this.mUITimer.Stop();
			this.mUITimer.Dispose();
			this.mUITimer = null;
		}
	}

	private void StartUITimer()
	{
		this.mUITimer = new DispatcherTimer("RebirthBossPart");
		this.mUITimer.Interval = TimeSpan.FromSeconds(1.0);
		this.mUITimer.Tick = new DispatcherTimerEventHandler(this.UITimer_Tick);
		this.mUITimer.Start();
	}

	private void UITimer_Tick(object sender, EventArgs args)
	{
		if (DateTime.MinValue != this.mNextrefreshTime)
		{
			if (this.mNextrefreshTime > Global.GetCorrectDateTime())
			{
				TimeSpan timeSpan;
				timeSpan..ctor(this.mNextrefreshTime.Ticks - Global.GetCorrectDateTime().Ticks);
				this._BossinflabelTime.text = Global.GetLang("距离刷新：") + Global.GetTimeStrBySecFilterZero((int)timeSpan.TotalSeconds, true, 2);
			}
			else
			{
				this._BossinflabelTime.text = Global.GetLang("距离刷新：已刷新");
			}
		}
	}

	private void InitLines()
	{
		if (null != this._RoadLineItem)
		{
			for (byte b = 0; b < 5; b += 1)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this._RoadLineItem);
				if (null != gameObject)
				{
					gameObject.name = ((int)(b + 1)).ToString();
					gameObject.transform.SetParent(this._RoadlinesRoot, false);
					gameObject.transform.localPosition = new Vector3((float)(0 + 60 * b), 0f, 0f);
					this.mLineItems[(int)b] = new RebirthBossPart.LineItems(gameObject);
					this.mLineItems[(int)b].BtnClick = new RebirthBossPart.BtnHanderDelegate(this.RoadLineItemClick);
				}
			}
		}
	}

	private void RoadLineItemClick(int btnID)
	{
		this.mSelectLineIndex = (byte)(btnID - 1);
		this.RefreshLineInf();
		this.RefreshBossInf();
		int num = 0;
		this.RefreshBossActive(out num);
	}

	private void RefreshLineInf()
	{
		for (int i = 0; i < this.mLineItems.Length; i++)
		{
			RebirthBossPart.LineItems lineItems = this.mLineItems[i];
			if (this.mBossData != null && i < this.mBossData.Count && 0 < this.mBossData[i].AwardExtensionID)
			{
				if ((int)(this.mSelectLineIndex + 1) == lineItems.ID)
				{
					lineItems.Select = true;
					lineItems.SelectEX = true;
				}
				else
				{
					lineItems.Select = false;
					lineItems.SelectEX = true;
				}
			}
			else if ((int)(this.mSelectLineIndex + 1) == lineItems.ID)
			{
				lineItems.Select = true;
				lineItems.SelectEX = false;
			}
			else
			{
				lineItems.Select = false;
				lineItems.SelectEX = false;
			}
		}
	}

	private void InitBoss()
	{
		BetterList<RebornBossVO> rebirthBossItems = IConfigbase<ConfigRebirth>.Instance.GetRebirthBossItems();
		if (rebirthBossItems != null && 0 < rebirthBossItems.size)
		{
			GameObject gameObject = null;
			for (int i = 0; i < rebirthBossItems.size; i++)
			{
				RebornBossVO rebornBossVO = rebirthBossItems[i];
				RebirthBossItem rebirthBossItem = U3DUtils.NEW<RebirthBossItem>();
				rebirthBossItem.MapCode = rebornBossVO.MapID;
				rebirthBossItem.MapName = ConfigSettings.GetMapNameByCode(rebornBossVO.MapID, false);
				rebirthBossItem.Scale = (float)rebornBossVO.Scale;
				rebirthBossItem.ZhanLi = rebornBossVO.ZhanLi.ToString();
				rebirthBossItem.BossID = rebornBossVO.MonstersID;
				this.mOBC.AddNoUpdate(rebirthBossItem);
				rebirthBossItem.Init(new MouseLeftButtonUpEventHandler(this.BossHanderCallBack));
				if (SceneUIClasses.RebornMap.IsTheScene())
				{
					if (i == 0)
					{
						gameObject = rebirthBossItem.gameObject;
					}
				}
				else if (i + 1 == KuaFuLoginManager.GetKuaFuSeverLineNumber())
				{
					gameObject = rebirthBossItem.gameObject;
				}
			}
			this._BossListBox.repositionNow = true;
			if (null == gameObject)
			{
				gameObject = this.mOBC.GetAt(0);
			}
			if (null != gameObject)
			{
				this.BossHanderCallBack(null, new MouseEvent("mouseUp", null)
				{
					target = gameObject.GetComponentInChildren<GGoodIcon>().gameObject
				});
			}
			SpringPanel.Begin(this._BossList.gameObject, new Vector3(0f, -7f, 0f), 10f);
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>" + Global.GetLang("RebornBossVO 读取有误 ") + "</color>"
			});
		}
	}

	private void BossHanderCallBack(object sender, MouseEvent e)
	{
		if (null != this._BossListBox.GetComponent<SpringPanel>() && this._BossListBox.GetComponent<SpringPanel>().enabled)
		{
			return;
		}
		if (e != null)
		{
			GGoodIcon ggoodIcon = e.target.SafeGetComponent<GGoodIcon>();
			if (null != ggoodIcon)
			{
				RebirthBossItem rebirthBossItem = NGUITools.FindInParents<RebirthBossItem>(ggoodIcon.gameObject);
				if (null != rebirthBossItem)
				{
					this.mSelectActivityBossItem = rebirthBossItem;
					this.RefreshModal(rebirthBossItem.BossID, rebirthBossItem.Scale);
					this.RefreshBossInf();
					for (int i = 0; i < this.mOBC.Count; i++)
					{
						GameObject at = this.mOBC.GetAt(i);
						if (null != at)
						{
							RebirthBossItem component = at.GetComponent<RebirthBossItem>();
							if (null != component)
							{
								if (component.BossID == rebirthBossItem.BossID)
								{
									component.Select = true;
								}
								else
								{
									component.Select = false;
								}
							}
						}
					}
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"<color=yellow> BossCallBack   从父节点获取数据失败</color>"
					});
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow> BossCallBack  获取 GGoodIcon 失败 </color>"
				});
			}
		}
	}

	private RebirthBossItem FindBossItem(int BossID)
	{
		for (int i = 0; i < this.mOBC.Count; i++)
		{
			GameObject at = this.mOBC.GetAt(i);
			if (null != at)
			{
				RebirthBossItem component = at.GetComponent<RebirthBossItem>();
				if (null != component && component.BossID == BossID)
				{
					return component;
				}
			}
		}
		return null;
	}

	private void RefreshBossInf()
	{
		if (null == this.mSelectActivityBossItem)
		{
			GameObject at = this.mOBC.GetAt(0);
			if (null != at)
			{
				this.mSelectActivityBossItem = at.GetComponent<RebirthBossItem>();
			}
		}
		if (null == this.mSelectActivityBossItem)
		{
			return;
		}
		byte b = 0;
		if (this.mBossData != null)
		{
			if ((int)this.mSelectLineIndex < this.mBossData.Count)
			{
				if (this.mBossData[(int)this.mSelectLineIndex] != null && this.mBossData[(int)this.mSelectLineIndex].ExtensionID == this.mSelectActivityBossItem.BossID)
				{
					if (string.IsNullOrEmpty(this.mBossData[(int)this.mSelectLineIndex].NextTime))
					{
						this._BossinflabelTime.text = Global.GetLang("下次刷新：已刷新");
					}
					else
					{
						DateTime.TryParse(this.mBossData[(int)this.mSelectLineIndex].NextTime, ref this.mNextrefreshTime);
						this.UITimer_Tick(null, null);
					}
					b = 1;
				}
				if (0 < this.mBossData[(int)this.mSelectLineIndex].AwardExtensionID || this.mBossData[(int)this.mSelectLineIndex].BossKill == 1)
				{
					this._AwardBtn.Label.text = Global.GetLang("领取奖励");
				}
				else
				{
					this._AwardBtn.Label.text = Global.GetLang("奖励预览");
				}
			}
		}
		else
		{
			this._AwardBtn.Label.text = Global.GetLang("奖励预览");
		}
		if (b == 0)
		{
			this._BossinflabelTime.text = Global.GetLang("下次刷新：未知");
		}
		this._BossinflabelZhanLi.text = Global.GetLang("推荐战力：") + this.mSelectActivityBossItem.ZhanLi;
		this._BossinflabelName.text = ConfigMonsters.GetMonsterXmlNodeByID(this.mSelectActivityBossItem.BossID).SName;
		this._MapName.text = ConfigSettings.GetMapNameByCode(this.mSelectActivityBossItem.MapCode, true);
	}

	private void RefreshBossActive(out int SelectIndex)
	{
		SelectIndex = 0;
		RebirthBossItem rebirthBossItem = null;
		if (this.mBossData != null && (int)this.mSelectLineIndex < this.mBossData.Count)
		{
			int extensionID = this.mBossData[(int)this.mSelectLineIndex].ExtensionID;
			bool flag = false;
			for (int i = 0; i < this.mOBC.Count; i++)
			{
				GameObject at = this.mOBC.GetAt(i);
				if (null != at)
				{
					RebirthBossItem rebirthBossItem2 = at.SafeGetComponent<RebirthBossItem>();
					if (null != rebirthBossItem2)
					{
						if (rebirthBossItem2.BossID == extensionID)
						{
							flag = true;
							rebirthBossItem = rebirthBossItem2;
							SelectIndex = i;
						}
						rebirthBossItem2.IsLock = !flag;
					}
				}
			}
		}
		if (null == rebirthBossItem)
		{
			GameObject at2 = this.mOBC.GetAt(0);
			if (null != at2)
			{
				rebirthBossItem = at2.SafeGetComponent<RebirthBossItem>();
			}
		}
		if (null != rebirthBossItem)
		{
			this.BossHanderCallBack(null, new MouseEvent("mouseUp", null)
			{
				target = rebirthBossItem.GetComponentInChildren<GGoodIcon>().gameObject
			});
		}
	}

	private void RefreshModal(int modalID, float sacle)
	{
		if (null != this._BossShow)
		{
			this._BossShow.Clear();
			if (this.mLoader != null)
			{
				this.mLoader.Stop();
				this.mLoader = null;
			}
			this.mLoader = UIHelper.LoadMonsterRes(this._BossShow, modalID, sacle);
		}
	}

	private void InitPrefabText()
	{
		this._GetAwardTitle.text = Global.GetLang("领取奖励");
		this._GetAwardBtn.Text = Global.GetLang("确定");
		this._GoFightBtn.Text = Global.GetLang("立即前往");
		try
		{
			this._RoadLinesLabel.text = Global.GetLang("线路");
			this._Title.text = Global.GetLang("奖励预览");
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
			this.mOBC = this._BossListBox.ItemsSource;
			this._CloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = 0
					});
				}
			};
			this._AwardBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._AwardBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._AwardBtn.GetInstanceID(), 1f);
				if (this.mBossData != null && (int)this.mSelectLineIndex < this.mBossData.Count && (0 < this.mBossData[(int)this.mSelectLineIndex].AwardExtensionID || this.mBossData[(int)this.mSelectLineIndex].BossKill == 1))
				{
					this.ShowGetAward((int)this.mSelectLineIndex, this.mBossData[(int)this.mSelectLineIndex].AwardExtensionID, this.mBossData[(int)this.mSelectLineIndex].RankNum, this.mBossData[(int)this.mSelectLineIndex].BossKill);
				}
				else
				{
					this.ShowJiangLiYuLan(this.mSelectActivityBossItem.BossID);
				}
			};
			this._GoFightBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._AwardBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._AwardBtn.GetInstanceID(), 1f);
				if (this.mBossData != null && this.mBossData.Count > (int)this.mSelectLineIndex && 0 < this.mBossData[(int)this.mSelectLineIndex].ExtensionID)
				{
					if (SceneUIClasses.RebornMap.IsTheScene() && (int)(this.mSelectLineIndex + 1) == KuaFuLoginManager.GetKuaFuSeverLineNumber())
					{
						RebornBossVO rebornBossVOByMonsterID = IConfigbase<ConfigRebirth>.Instance.GetRebornBossVOByMonsterID(this.mSelectActivityBossItem.BossID);
						if (rebornBossVOByMonsterID != null)
						{
							Global.Data.GameScene.AutoFindRoad(rebornBossVOByMonsterID.MapID, rebornBossVOByMonsterID.monsetRefreshPos, 0, ExtActionTypes.EXTACTION_NONE);
						}
					}
					else
					{
						GameInstance.Game.EnterKuaFuMap(this.mSelectActivityBossItem.MapCode, (int)(this.mSelectLineIndex + 1), this.mSelectActivityBossItem.BossID, 0);
						RebornBossVO rebornBossVOByMonsterID2 = IConfigbase<ConfigRebirth>.Instance.GetRebornBossVOByMonsterID(this.mSelectActivityBossItem.BossID);
						if (rebornBossVOByMonsterID2 != null)
						{
							Global.Data.GameScene.AutoFindRoad(rebornBossVOByMonsterID2.MapID, rebornBossVOByMonsterID2.monsetRefreshPos, 0, ExtActionTypes.EXTACTION_NONE);
						}
					}
					if (this.Hander != null)
					{
						this.Hander(null, new DPSelectedItemEventArgs
						{
							Type = 3,
							ID = this.mSelectActivityBossItem.BossID,
							IDType = (int)(this.mSelectLineIndex + 1)
						});
						return;
					}
				}
				Super.HintMainText(Global.GetLang("此线路不通！"), 10, 3);
			};
			this._AwardYULanCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.CloseJiangLiYuLan();
			};
			this._GetAwardCloseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				this.ClosegetAward();
			};
			this._GetAwardBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._AwardBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._AwardBtn.GetInstanceID(), 1f);
				GameInstance.Game.SendRoleGetRebirthAward((int)(this.mSelectLineIndex + 1), this.mSelectActivityBossItem.MapCode, this.mBossData[(int)this.mSelectLineIndex].AwardExtensionID);
				this.ClosegetAward();
			};
			this._HelpBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (0f < Global.GetBtnCD(this._AwardBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._AwardBtn.GetInstanceID(), 1f);
				this.OpenHelpWindow(string.Empty);
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

	private void OpenHelpWindow(string path)
	{
		if (this.m_helpWindow == null)
		{
			this.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			this.m_helpWindow.IsShowModal = true;
			this.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_helpWindow, Global.GetLang("NewCommonHelpWindow"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_helpWindow);
		}
		if (this.m_helpPart == null)
		{
			this.m_helpPart = U3DUtils.NEW<NewCommonHelpWindow>();
			this.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHelpWindow();
			};
		}
		this.m_helpWindow.SetContent(this.m_helpWindow.BodyPresenter, this.m_helpPart, 0.0, 0.0, true);
		XElement gameResXml = Global.GetGameResXml("GameRes/Config/RebornIntroBoss.xml");
		if (gameResXml == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				string.Format(Global.GetLang("加载{0}出现错误"), path)
			});
		}
		ChangeableRulePart.RuleXml ruleXml = new ChangeableRulePart.RuleXml(gameResXml);
		this.m_helpPart.SetHelpInfo(ruleXml.list, false);
	}

	private void CloseHelpWindow()
	{
		if (null != this.m_helpPart)
		{
			this.m_helpPart.transform.parent = null;
			Object.Destroy(this.m_helpPart.gameObject);
			this.m_helpPart = null;
		}
		if (null != this.m_helpWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_helpWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_helpWindow, true);
			this.m_helpWindow = null;
		}
	}

	internal void NoticeGetAwardCallBack(int ret, int lineid)
	{
		if (0 <= ret)
		{
			if (this.mBossData != null && lineid - 1 < this.mBossData.Count)
			{
				this.mBossData[lineid - 1].AwardExtensionID = 0;
				this.mBossData[lineid - 1].BossKill = 0;
			}
			Super.HintMainText(Global.GetLang("领取成功"), 10, 3);
			this.ClosegetAward();
			this.RefreshLineInf();
			this.RefreshBossInf();
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(ret, false, false)), 10, 3);
		}
	}

	internal void NocticGetDataCallBack(List<RebornBossData> data)
	{
		if (data == null)
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=yellow>重生Boss从服务器拿到的数据为空 data  为空 </color>"
			});
		}
		this.mBossData = data;
		int num = 0;
		this.RefreshBossActive(out num);
		if (500 < num * 110)
		{
			SpringPanel.Begin(this._BossList.gameObject, new Vector3((float)(500 - num * 110), -7f, 0f), 10f);
		}
		if (this.mBossData != null)
		{
			int btnID = 1;
			byte b = 0;
			while ((int)b < this.mLineItems.Length)
			{
				if (this.mLineItems[(int)b].ID == KuaFuLoginManager.GetKuaFuSeverLineNumber())
				{
					btnID = this.mLineItems[(int)b].ID;
					break;
				}
				b += 1;
			}
			this.RoadLineItemClick(btnID);
		}
		else
		{
			this.RoadLineItemClick(1);
		}
	}

	private const int RoadLineSpace = 60;

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private ShowNetImage _BakImage;

	[SerializeField]
	private GButton _CloseBtn;

	[SerializeField]
	private UIDraggablePanel _BossList;

	[SerializeField]
	private ListBox _BossListBox;

	[SerializeField]
	private Modal3DShow _BossShow;

	[SerializeField]
	private GameObject _RoadLineItem;

	[SerializeField]
	private Transform _RoadlinesRoot;

	[SerializeField]
	private UILabel _RoadLinesLabel;

	[SerializeField]
	private UILabel _BossinflabelName;

	[SerializeField]
	private UILabel _BossinflabelTime;

	[SerializeField]
	private UILabel _BossinflabelZhanLi;

	[SerializeField]
	private GButton _AwardBtn;

	[SerializeField]
	private GButton _GoFightBtn;

	[SerializeField]
	private UILabel _MapName;

	[SerializeField]
	private GameObject _AwardYuLanRoot;

	[SerializeField]
	private UICollider _AwardYULanUICollider;

	[SerializeField]
	private Transform _AwardYULanGoodsRoot;

	[SerializeField]
	private UILabel _AwardYULanGoodsTitle;

	[SerializeField]
	private GButton _AwardYULanCloseBtn;

	[SerializeField]
	private UILabel _Title;

	[SerializeField]
	private GameObject _GetAwardRoot;

	[SerializeField]
	private UICollider _GetAwardUICollider;

	[SerializeField]
	private UICollider _GetAwardUIColliderLastAtt;

	[SerializeField]
	private Transform _GetAwardGoodsRoot;

	[SerializeField]
	private GButton _GetAwardCloseBtn;

	[SerializeField]
	private GButton _GetAwardBtn;

	[SerializeField]
	private UILabel _GetAwardMyRankLabel;

	[SerializeField]
	private Transform _GetLastAttAwardGoodsRoot;

	[SerializeField]
	private UILabel _GetAwardLastAttLabel;

	[SerializeField]
	private Transform _GetLastAttRoot;

	[SerializeField]
	private Transform _GetRankRoot;

	[SerializeField]
	private GButton _HelpBtn;

	[SerializeField]
	private Transform _GetAwardBak;

	[SerializeField]
	private UILabel _GetAwardTitle;

	private MonsterNPCResLoader mLoader;

	private RebirthBossItem mSelectActivityBossItem;

	private RebirthBossPart.LineItems[] mLineItems = new RebirthBossPart.LineItems[5];

	private byte mSelectLineIndex;

	private List<RebornBossData> mBossData;

	private ObservableCollection mOBC;

	private DispatcherTimer mUITimer;

	private DateTime mNextrefreshTime = DateTime.MinValue;

	private BetterList<GameObject> mAwardGoodsRootAndLabels = new BetterList<GameObject>();

	protected GChildWindow m_helpWindow;

	protected NewCommonHelpWindow m_helpPart;

	private class LineItems
	{
		public LineItems(GameObject obj)
		{
			RebirthBossPart.LineItems <>f__this = this;
			if (!NGUITools.GetActive(obj))
			{
				obj.SetActive(true);
			}
			this.mID = obj.name.SafeToInt32(0);
			UIEventListener.Get(obj).onClick = delegate(GameObject g)
			{
				if (0f < Global.GetBtnCD(obj.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(obj.GetInstanceID(), 0.5f);
				if (<>f__this.BtnClick != null)
				{
					<>f__this.BtnClick(<>f__this.mID);
				}
			};
			this._BgSp = obj.transform.FindChild("BG").GetComponent<UISprite>();
			this._BaNum = obj.transform.FindChild("Num").GetComponent<UISprite>();
			this._SelectBGEX = obj.transform.FindChild("SelectBg/chongsheng_boss_kuang").gameObject;
			this._SelectBg = obj.transform.FindChild("SelectBg/SelectBg (1)").gameObject;
			this._BaNum.spriteName = "xianlu_" + this.mID.ToString();
			UIEventListener.Get(obj).onPress = delegate(GameObject g, bool s)
			{
				if (s)
				{
					<>f__this.OnHover(s);
				}
				if (!s && !<>f__this.mSelect)
				{
					<>f__this.OnHover(false);
				}
			};
			this.ThisObj = obj;
			if (this.mID == 1)
			{
				this.Select = true;
			}
			else
			{
				this.Select = false;
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
		}

		private void OnHover(bool s)
		{
			if (null != this._BgSp)
			{
				this._BgSp.spriteName = ((!s) ? "lianlu_bg_2" : "lianlu_bg_1");
			}
		}

		public bool Select
		{
			get
			{
				return this.mSelect;
			}
			set
			{
				this.mSelect = value;
				if (this.mSelect)
				{
					string spriteName = this._BaNum.spriteName;
					if (spriteName.EndsWith("_l"))
					{
						this._BaNum.spriteName = spriteName;
					}
					else
					{
						this._BaNum.spriteName = spriteName + "_l";
					}
				}
				else
				{
					string spriteName2 = this._BaNum.spriteName;
					if (spriteName2.EndsWith("_l"))
					{
						this._BaNum.spriteName = spriteName2.Remove(spriteName2.Length - 2, 2);
					}
					else
					{
						this._BaNum.spriteName = spriteName2;
					}
				}
				this._SelectBg.gameObject.SetActive(this.mSelect);
				this.OnHover(this.mSelect);
			}
		}

		public bool SelectEX
		{
			set
			{
				this.mHaveAward = value;
				this._SelectBGEX.SetActive(value);
			}
		}

		public RebirthBossPart.BtnHanderDelegate BtnClick;

		private int mID;

		private UISprite _BgSp;

		private UISprite _BaNum;

		private GameObject _SelectBg;

		private GameObject _SelectBGEX;

		private GameObject ThisObj;

		private bool mSelect;

		private bool mHaveAward;
	}

	private delegate void BtnHanderDelegate(int btnID);
}
