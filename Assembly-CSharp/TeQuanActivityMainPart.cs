using System;
using System.Collections;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

public class TeQuanActivityMainPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
		IConfigbase<ConfigTeQuan>.Instance.GetDataFormSever(delegate(object e, DPSelectedItemEventArgs s)
		{
			Super.ShowNetWaiting(null);
			TCPGameServerCmds.CMD_SPR_SPEPRIORITY_ACTIVITY_QUERY.SendDataUseRoleID();
		});
	}

	public override void Update()
	{
		base.Update();
		if (this.Refresh)
		{
			this.Refresh = false;
			this.mRefreshCount++;
			if (this.mSpecPriorityActivityData != null && this.mSpecPriorityActivityData.ConditionDict != null)
			{
				List<int> list = new List<int>();
				Dictionary<int, int>.Enumerator enumerator = this.mSpecPriorityActivityData.ConditionDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					List<int> list2 = list;
					KeyValuePair<int, int> keyValuePair = enumerator.Current;
					list2.Add(keyValuePair.Key);
				}
				for (int i = 0; i < list.Count; i++)
				{
					TeQuanJiHuoVO teQuanJiHuoVO = null;
					if (i < this.mRefreshCount)
					{
						teQuanJiHuoVO = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(list[i]);
					}
					if (teQuanJiHuoVO != null)
					{
						this.mSpecPriorityActivityData.ConditionDict[list[i]] = teQuanJiHuoVO.CanShu;
					}
					else if (i < this.Percent.Length)
					{
						this.mSpecPriorityActivityData.ConditionDict[list[i]] = this.Percent[i];
					}
				}
			}
			this.NoticeGetMainDataCallBack(this.mSpecPriorityActivityData);
		}
	}

	private void InitActivityTypeBtns()
	{
		this.btnHaveInit = true;
		List<int> list = new List<int>();
		if (this.mSpecPriorityActivityData != null && this.mSpecPriorityActivityData.ConditionDict != null)
		{
			Dictionary<int, int>.Enumerator enumerator = this.mSpecPriorityActivityData.ConditionDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConfigTeQuan instance = IConfigbase<ConfigTeQuan>.Instance;
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				TeQuanTiaoJianVO teQuanTiaoJianVOByID = instance.GetTeQuanTiaoJianVOByID(keyValuePair.Key);
				if (teQuanTiaoJianVOByID != null)
				{
					TeQuanTiaoJianVO teQuanTiaoJianVO = teQuanTiaoJianVOByID;
					if (teQuanTiaoJianVO != null)
					{
						if (0 < teQuanTiaoJianVO.JiHuoIDs.size)
						{
							for (int i = 0; i < teQuanTiaoJianVO.JiHuoIDs.size; i++)
							{
								list.Add(teQuanTiaoJianVO.JiHuoIDs[i]);
							}
						}
						GameObject gameObject = Object.Instantiate<GameObject>(this._ActivityTypeBtn);
						TeQuanActivityMainPart.ActivityBtnHander activityBtnHander = new TeQuanActivityMainPart.ActivityBtnHander(gameObject, teQuanTiaoJianVO.ID, 0);
						activityBtnHander.BtnText = teQuanTiaoJianVO.HuoDongNiCheng;
						if (teQuanTiaoJianVO.TiaoJianLeiXing % 2 == 0)
						{
							activityBtnHander.SeverType = 1;
						}
						else
						{
							activityBtnHander.SeverType = 0;
						}
						activityBtnHander.Hander = new DPSelectedItemEventHandler(this.BtnSHander);
						this.mBtnObc.AddNoUpdate(gameObject);
						this.mActivityBtnHanderList0.Add(activityBtnHander);
					}
				}
			}
		}
		this.InitActivityTypeBtns1(list);
		for (int j = 0; j < this.mActivityBtnHanderList0.size; j++)
		{
			this.mActivityBtnHanderList0[j].Index = j;
		}
		for (int k = 0; k < this.mActivityBtnHanderList1.size; k++)
		{
			this.mActivityBtnHanderList1[k].Index = k;
		}
		SpringPosition.Begin(this._ActivityTypeBtnsListBox.gameObject, new Vector3(0f, 172f, 0f), 10f);
		this.DestoryPanel();
		this._ActivityTypeBtnsListBox.repositionNow = true;
		this._ActivityTypeBtnsListBox.SelectedIndex = 0;
	}

	private void BtnSHander(object sender, DPSelectedItemEventArgs args)
	{
		if (args.Type == 1)
		{
			byte b = 0;
			while ((int)b < this.mActivityBtnHanderList0.size)
			{
				this.mActivityBtnHanderList0[(int)b].BSelect = this.mActivityBtnHanderList0[(int)b].BSelect;
				b += 1;
			}
			byte b2 = 0;
			while ((int)b2 < this.mActivityBtnHanderList1.size)
			{
				this.mActivityBtnHanderList1[(int)b2].BSelect = this.mActivityBtnHanderList1[(int)b2].BSelect;
				b2 += 1;
			}
		}
	}

	private void DestoryPanel()
	{
		for (int i = 0; i < this.mActivityBtnHanderList0.size; i++)
		{
			this.mActivityBtnHanderList0[i].DestoryPanel();
		}
		for (int j = 0; j < this.mActivityBtnHanderList1.size; j++)
		{
			this.mActivityBtnHanderList1[j].DestoryPanel();
		}
	}

	private void InitActivityTypeBtns1(List<int> BtnIDList)
	{
		BetterList<TeQuanJiHuoVO> betterList = new BetterList<TeQuanJiHuoVO>();
		for (int i = 0; i < BtnIDList.Count; i++)
		{
			TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(BtnIDList[i]);
			if (teQuanJiHuoVOByID != null && teQuanJiHuoVOByID.UIAnNiu == 1)
			{
				betterList.Add(teQuanJiHuoVOByID);
			}
		}
		if (this.mActivityBtnHanderList1.size < betterList.size)
		{
			for (int j = this.mActivityBtnHanderList1.size; j < betterList.size; j++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this._ActivityTypeBtn);
				TeQuanActivityMainPart.ActivityBtnHander activityBtnHander = new TeQuanActivityMainPart.ActivityBtnHander(gameObject, j, 1);
				activityBtnHander.Hander = new DPSelectedItemEventHandler(this.BtnSHander);
				this.mBtnObc.AddNoUpdate(gameObject);
				this.mActivityBtnHanderList1.Add(activityBtnHander);
			}
		}
		for (int k = 0; k < betterList.size; k++)
		{
			TeQuanJiHuoVO teQuanJiHuoVO = betterList[k];
			if (teQuanJiHuoVO != null)
			{
				TeQuanActivityMainPart.ActivityBtnHander activityBtnHander2 = this.mActivityBtnHanderList1[k];
				activityBtnHander2.BtnText = teQuanJiHuoVO.HuoDongName;
				activityBtnHander2.MyType = 1;
				activityBtnHander2.SeverType = 3;
				activityBtnHander2.ID = teQuanJiHuoVO.ID;
				activityBtnHander2.BtnIsLock = true;
			}
		}
	}

	private void InitPrefabText()
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
			this.mBtnObc = this._ActivityTypeBtnsListBox.ItemsSource;
			this._CLoseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this._ActivityTypeBtnsListBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.BtnTypeSelectHave);
			this._TeQuanActivityRepeatPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (e == null && s == null && this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						ID = 0,
						Type = 0
					});
				}
			};
			this._TeQuanActiviteBuffPart.Hander = delegate(object e, DPSelectedItemEventArgs s)
			{
				if (e == null && this.Hander != null)
				{
					this.Hander(null, new DPSelectedItemEventArgs
					{
						Type = s.Type
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

	private void BtnTypeSelectHave(object sender, MouseEvent e)
	{
		GameObject selectedItem = this._ActivityTypeBtnsListBox.SelectedItem;
		if (null == selectedItem)
		{
			return;
		}
		string[] array = selectedItem.name.Split(new char[]
		{
			'_'
		});
		int num = array[0].SafeToInt32(0);
		int num2 = array[1].SafeToInt32(0);
		if (num == 0)
		{
			byte b = 0;
			while ((int)b < this.mActivityBtnHanderList0.size)
			{
				if (num2 == this.mActivityBtnHanderList0[(int)b].ID)
				{
					this.mActivityBtnHanderList0[(int)b].BSelect = true;
				}
				else
				{
					this.mActivityBtnHanderList0[(int)b].BSelect = false;
				}
				b += 1;
			}
			byte b2 = 0;
			while ((int)b2 < this.mActivityBtnHanderList1.size)
			{
				this.mActivityBtnHanderList1[(int)b2].BSelect = false;
				b2 += 1;
			}
			this._TeQuanActivityRepeatPart.Visibility = true;
			this._TeQuanActivityShopPart.Visibility = false;
			this._TeQuanActivityEventuallyAwardPart.Visibility = false;
			this._TeQuanActivityZhiGouPart.Visibility = false;
			this._TeQuanActiviteBuffPart.Visibility = false;
			this._TeQuanActivityRepeatPart.ID = num2;
			this._TeQuanActivityRepeatPart.RefreshPart(IConfigbase<ConfigTeQuan>.Instance.GetTeQuanTiaoJianVOByID(num2), -1);
			this._TeQuanActivityRepeatPart.RefreshPart(this.GetSpecActInfoBtyID(num2));
			if (this.mSpecPriorityActivityData.ConditionDict.ContainsKey(num2))
			{
				this._TeQuanActivityRepeatPart.RefreshPartPrence(this.mSpecPriorityActivityData.ConditionDict[num2]);
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"<color=yellow> 不包含：Key " + num2 + "</color>"
				});
			}
		}
		else
		{
			byte b3 = 0;
			byte b4 = 0;
			while ((int)b4 < this.mActivityBtnHanderList1.size)
			{
				if (num2 == this.mActivityBtnHanderList1[(int)b4].ID && this.mActivityBtnHanderList1[(int)b4].BtnIsLock)
				{
					b3 = 1;
					break;
				}
				b4 += 1;
			}
			if (b3 == 1)
			{
				Super.HintMainText(Global.GetLang("活动暂未激活"), 10, 3);
				return;
			}
			byte b5 = 0;
			while ((int)b5 < this.mActivityBtnHanderList1.size)
			{
				if (num2 == this.mActivityBtnHanderList1[(int)b5].ID)
				{
					this.mActivityBtnHanderList1[(int)b5].BSelect = true;
				}
				else
				{
					this.mActivityBtnHanderList1[(int)b5].BSelect = false;
				}
				b5 += 1;
			}
			byte b6 = 0;
			while ((int)b6 < this.mActivityBtnHanderList0.size)
			{
				this.mActivityBtnHanderList0[(int)b6].BSelect = false;
				b6 += 1;
			}
			TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(num2);
			switch (teQuanJiHuoVOByID.tips.SafeToInt32(0))
			{
			case 1:
			case 9:
			case 10:
				this._TeQuanActivityRepeatPart.ID = num2;
				this._TeQuanActivityRepeatPart.Visibility = true;
				this._TeQuanActivityShopPart.Visibility = false;
				this._TeQuanActivityEventuallyAwardPart.Visibility = false;
				this._TeQuanActivityZhiGouPart.Visibility = false;
				this._TeQuanActiviteBuffPart.Visibility = false;
				break;
			case 2:
				this._TeQuanActivityEventuallyAwardPart.ID = num2;
				this._TeQuanActivityShopPart.Visibility = false;
				this._TeQuanActivityRepeatPart.Visibility = false;
				this._TeQuanActivityEventuallyAwardPart.Visibility = true;
				this._TeQuanActivityZhiGouPart.Visibility = false;
				this._TeQuanActiviteBuffPart.Visibility = false;
				break;
			case 3:
				this._TeQuanActivityZhiGouPart.ID = num2;
				this._TeQuanActivityShopPart.Visibility = false;
				this._TeQuanActivityRepeatPart.Visibility = false;
				this._TeQuanActivityEventuallyAwardPart.Visibility = false;
				this._TeQuanActivityZhiGouPart.Visibility = true;
				this._TeQuanActiviteBuffPart.Visibility = false;
				break;
			case 4:
				this._TeQuanActivityShopPart.ID = num2;
				this._TeQuanActivityShopPart.Visibility = true;
				this._TeQuanActivityRepeatPart.Visibility = false;
				this._TeQuanActivityEventuallyAwardPart.Visibility = false;
				this._TeQuanActivityZhiGouPart.Visibility = false;
				this._TeQuanActiviteBuffPart.Visibility = false;
				break;
			case 5:
				this._TeQuanActiviteBuffPart.ID = num2;
				this._TeQuanActivityShopPart.Visibility = false;
				this._TeQuanActivityRepeatPart.Visibility = false;
				this._TeQuanActivityEventuallyAwardPart.Visibility = false;
				this._TeQuanActivityZhiGouPart.Visibility = false;
				this._TeQuanActiviteBuffPart.Visibility = true;
				break;
			}
			this.RefreshPart();
		}
	}

	private BetterList<SpecPriorityActInfo> GetSpecActInfoListBtyID(int ID)
	{
		BetterList<SpecPriorityActInfo> betterList = new BetterList<SpecPriorityActInfo>();
		if (this.mSpecPriorityActivityData != null && this.mSpecPriorityActivityData.SpecActInfoList != null)
		{
			for (int i = 0; i < this.mSpecPriorityActivityData.SpecActInfoList.Count; i++)
			{
				if (ID == this.mSpecPriorityActivityData.SpecActInfoList[i].TeQuanID)
				{
					betterList.Add(this.mSpecPriorityActivityData.SpecActInfoList[i]);
				}
			}
		}
		return betterList;
	}

	private SpecPriorityActInfo GetSpecActInfoBtyID(int ID)
	{
		if (this.mSpecPriorityActivityData != null && this.mSpecPriorityActivityData.SpecActInfoList != null)
		{
			for (int i = 0; i < this.mSpecPriorityActivityData.SpecActInfoList.Count; i++)
			{
				if (ID == this.mSpecPriorityActivityData.SpecActInfoList[i].TeQuanID)
				{
					return this.mSpecPriorityActivityData.SpecActInfoList[i];
				}
			}
		}
		return null;
	}

	private SpecPriorityActInfo GetSpecActInfoBtyID(int TeQuanID, int Act)
	{
		if (this.mSpecPriorityActivityData != null && this.mSpecPriorityActivityData.SpecActInfoList != null)
		{
			for (int i = 0; i < this.mSpecPriorityActivityData.SpecActInfoList.Count; i++)
			{
				if (TeQuanID == this.mSpecPriorityActivityData.SpecActInfoList[i].TeQuanID && Act == this.mSpecPriorityActivityData.SpecActInfoList[i].ActID)
				{
					return this.mSpecPriorityActivityData.SpecActInfoList[i];
				}
			}
		}
		return null;
	}

	private TeQuanActivityMainPart.ActivityBtnHander GetItem0(int ID)
	{
		byte b = 0;
		while ((int)b < this.mActivityBtnHanderList0.size)
		{
			if (ID == this.mActivityBtnHanderList0[(int)b].ID)
			{
				return this.mActivityBtnHanderList0[(int)b];
			}
			b += 1;
		}
		return null;
	}

	private TeQuanActivityMainPart.ActivityBtnHander GetItem1(int ID)
	{
		byte b = 0;
		while ((int)b < this.mActivityBtnHanderList1.size)
		{
			if (ID == this.mActivityBtnHanderList1[(int)b].ID)
			{
				return this.mActivityBtnHanderList1[(int)b];
			}
			b += 1;
		}
		return null;
	}

	private void RefreshBtns()
	{
		if (this.mSpecPriorityActivityData != null)
		{
			List<int> list = new List<int>();
			Dictionary<int, int>.Enumerator enumerator = this.mSpecPriorityActivityData.ConditionDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				List<int> list2 = list;
				KeyValuePair<int, int> keyValuePair = enumerator.Current;
				list2.Add(keyValuePair.Key);
			}
			for (int i = 0; i < list.Count; i++)
			{
				TeQuanTiaoJianVO teQuanTiaoJianVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanTiaoJianVOByID(list[i]);
				if (teQuanTiaoJianVOByID != null)
				{
					TeQuanActivityMainPart.ActivityBtnHander item = this.GetItem0(teQuanTiaoJianVOByID.ID);
					if (teQuanTiaoJianVOByID.TiaoJianLeiXing % 2 == 0)
					{
						item.SeverType = 1;
					}
					else
					{
						item.SeverType = 0;
					}
					for (int j = 0; j < teQuanTiaoJianVOByID.JiHuoIDs.size; j++)
					{
						TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(teQuanTiaoJianVOByID.JiHuoIDs[j]);
						if (teQuanJiHuoVOByID != null)
						{
							TeQuanActivityMainPart.ActivityBtnHander item2 = this.GetItem1(teQuanJiHuoVOByID.ID);
							if (item2 != null)
							{
								item2.SeverType = 2;
								item2.ID = teQuanJiHuoVOByID.ID;
								SpecPriorityActInfo specActInfoBtyID = this.GetSpecActInfoBtyID(teQuanTiaoJianVOByID.JiHuoIDs[j]);
								if (specActInfoBtyID != null)
								{
									item2.BtnIsLock = false;
								}
								else
								{
									item2.BtnIsLock = true;
								}
								MUDebug.Log<string>(new string[]
								{
									teQuanJiHuoVOByID.HuoDongName + " ::  BtnIsLock = " + item2.BtnIsLock
								});
							}
							else
							{
								MUDebug.Log<string>(new string[]
								{
									string.Concat(new object[]
									{
										teQuanJiHuoVOByID.HuoDongName,
										" ::  id  = ",
										teQuanJiHuoVOByID.ID,
										" 没有找到的按钮"
									})
								});
							}
						}
					}
				}
			}
			BetterList<TeQuanActivityMainPart.ActivityBtnHander> betterList = new BetterList<TeQuanActivityMainPart.ActivityBtnHander>();
			for (int k = 0; k < this.mActivityBtnHanderList1.size; k++)
			{
				if (!this.mActivityBtnHanderList1[k].BtnIsLock)
				{
					betterList.Add(this.mActivityBtnHanderList1[k]);
				}
			}
			for (int l = 0; l < this.mActivityBtnHanderList1.size; l++)
			{
				if (!betterList.Contains(this.mActivityBtnHanderList1[l]))
				{
					betterList.Add(this.mActivityBtnHanderList1[l]);
				}
			}
			this.mActivityBtnHanderList1.Clear();
			for (int m = 0; m < betterList.size; m++)
			{
				this.mActivityBtnHanderList1.Add(betterList[m]);
			}
			base.StartCoroutine<bool>(this.ResetPos());
		}
	}

	private bool GetBtnSelectBtnIsLook()
	{
		byte b = 0;
		while ((int)b < this.mActivityBtnHanderList0.size)
		{
			if (this.mActivityBtnHanderList0[(int)b].BSelect)
			{
				return !this.mActivityBtnHanderList0[(int)b].BtnIsLock;
			}
			b += 1;
		}
		byte b2 = 0;
		while ((int)b2 < this.mActivityBtnHanderList1.size)
		{
			if (this.mActivityBtnHanderList1[(int)b2].BSelect)
			{
				return !this.mActivityBtnHanderList1[(int)b2].BtnIsLock;
			}
			b2 += 1;
		}
		return false;
	}

	private void RefreshPart()
	{
		if (null != this._TeQuanActivityEventuallyAwardPart && this._TeQuanActivityEventuallyAwardPart.IsActive)
		{
			this._TeQuanActivityEventuallyAwardPart.RefreshPart(this.GetSpecActInfoBtyID(this._TeQuanActivityEventuallyAwardPart.ID));
			this._TeQuanActivityEventuallyAwardPart.PartOpen = this.GetBtnSelectBtnIsLook();
		}
		if (null != this._TeQuanActivityShopPart && this._TeQuanActivityShopPart.IsActive)
		{
			this._TeQuanActivityShopPart.InitShop(this._TeQuanActivityShopPart.ID);
			this._TeQuanActivityShopPart.RefreshPart(this.GetSpecActInfoListBtyID(this._TeQuanActivityShopPart.ID));
			this._TeQuanActivityShopPart.PartOpen = this.GetBtnSelectBtnIsLook();
		}
		if (null != this._TeQuanActivityRepeatPart && this._TeQuanActivityRepeatPart.IsActive)
		{
			if (this.mSpecPriorityActivityData != null && this.mSpecPriorityActivityData.ConditionDict != null && this.mSpecPriorityActivityData.ConditionDict.ContainsKey(this._TeQuanActivityRepeatPart.ID))
			{
				this._TeQuanActivityRepeatPart.RefreshPartPrence(this.mSpecPriorityActivityData.ConditionDict[this._TeQuanActivityRepeatPart.ID]);
			}
			this._TeQuanActivityRepeatPart.RefreshPart(this.GetSpecActInfoBtyID(this._TeQuanActivityRepeatPart.ID));
			this._TeQuanActivityRepeatPart.PartOpen = this.GetBtnSelectBtnIsLook();
			if (this.mSpecPriorityActivityData != null)
			{
				this._TeQuanActivityRepeatPart.DonateNum = this.mSpecPriorityActivityData.DonateNum;
				this._TeQuanActivityRepeatPart.DonateNumKF = this.mSpecPriorityActivityData.DonateNumKF;
			}
		}
		if (null != this._TeQuanActivityZhiGouPart && this._TeQuanActivityZhiGouPart.IsActive)
		{
			this._TeQuanActivityZhiGouPart.RefreshPart(this.GetSpecActInfoListBtyID(this._TeQuanActivityZhiGouPart.ID));
			this._TeQuanActivityZhiGouPart.PartOpen = this.GetBtnSelectBtnIsLook();
		}
		if (null != this._TeQuanActiviteBuffPart && this._TeQuanActiviteBuffPart.IsActive)
		{
			this._TeQuanActiviteBuffPart.RefreshPart(this.GetSpecActInfoBtyID(this._TeQuanActiviteBuffPart.ID));
		}
	}

	private IEnumerator ResetPos()
	{
		yield return new WaitForEndOfFrame();
		int x = 0;
		int y = 0;
		BetterList<TeQuanActivityMainPart.ActivityBtnHander> temp = new BetterList<TeQuanActivityMainPart.ActivityBtnHander>();
		int i = 0;
		int imax = this.mActivityBtnHanderList0.size;
		while (i < imax)
		{
			temp.Add(this.mActivityBtnHanderList0[i]);
			i++;
		}
		int j = 0;
		int imax2 = this.mActivityBtnHanderList1.size;
		while (j < imax2)
		{
			temp.Add(this.mActivityBtnHanderList1[j]);
			j++;
		}
		int k = 0;
		int imax3 = temp.size;
		while (k < imax3)
		{
			Transform t = temp[k].RootTreans;
			if (NGUITools.GetActive(t.gameObject))
			{
				float depth = t.localPosition.z;
				t.localPosition = new Vector3(this._ActivityTypeBtnsListBox.cellWidth * (float)y, -this._ActivityTypeBtnsListBox.cellHeight * (float)x, depth);
				if (++x >= this._ActivityTypeBtnsListBox.maxPerLine && this._ActivityTypeBtnsListBox.maxPerLine > 0)
				{
					x = 0;
					y++;
				}
			}
			k++;
		}
		yield break;
	}

	public void NoticeTeQuanBuyOrGetAwardCallBack(string[] p)
	{
		if (p[0].SafeToInt32(0) == 0)
		{
			SpecPriorityActInfo specActInfoBtyID = this.GetSpecActInfoBtyID(p[2].SafeToInt32(0), p[3].SafeToInt32(0));
			if (specActInfoBtyID != null)
			{
				specActInfoBtyID.LeftPurNum = p[4].SafeToInt32(0);
				if (specActInfoBtyID.TeQuanID % 4 == 2)
				{
					specActInfoBtyID.State = 1;
				}
			}
			this.RefreshPart();
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(p[0].SafeToInt32(0), false, false)), 10, 3);
		}
	}

	internal void NoticeTeQuanJuanZengCallBack(string[] p)
	{
		if ("0" == p[0])
		{
			if (this.mSpecPriorityActivityData != null)
			{
				if (this.mSpecPriorityActivityData.ConditionDict.ContainsKey(p[1].SafeToInt32(0)))
				{
					Dictionary<int, int> conditionDict;
					Dictionary<int, int> dictionary = conditionDict = this.mSpecPriorityActivityData.ConditionDict;
					int num2;
					int num = num2 = p[1].SafeToInt32(0);
					num2 = conditionDict[num2];
					dictionary[num] = num2 + 1;
				}
				if (p[1].SafeToInt32(0) % 2 == 0)
				{
					this.mSpecPriorityActivityData.DonateNumKF++;
				}
				else
				{
					this.mSpecPriorityActivityData.DonateNum++;
				}
				this.RefreshPart();
			}
		}
		else
		{
			Super.HintMainText(Global.GetLang(StdErrorCode.GetErrMsg(p[0].SafeToInt32(0), false, false)), 10, 3);
		}
	}

	public void NoticeGetMainDataCallBack(SpecPriorityActivityData data)
	{
		if (data != null)
		{
			this.mSpecPriorityActivityData = data;
			if (!this.btnHaveInit)
			{
				this.InitActivityTypeBtns();
			}
			this.RefreshBtns();
			this.RefreshPart();
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"<color=red>Data==null</color>"
			});
		}
	}

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private GButton _CLoseBtn;

	[SerializeField]
	private GameObject _ActivityTypeBtn;

	[SerializeField]
	private UIDraggablePanel _ActivityTypeBtnsDragPanel;

	[SerializeField]
	private ListBox _ActivityTypeBtnsListBox;

	[SerializeField]
	private TeQuanActivityRepeatPart _TeQuanActivityRepeatPart;

	[SerializeField]
	private TeQuanActivityShopPart _TeQuanActivityShopPart;

	[SerializeField]
	private TeQuanActivityEventuallyAwardPart _TeQuanActivityEventuallyAwardPart;

	[SerializeField]
	private TeQuanActivityZhiGouPart _TeQuanActivityZhiGouPart;

	[SerializeField]
	private TeQuanActiviteBuffPart _TeQuanActiviteBuffPart;

	[SerializeField]
	private int[] Percent;

	[SerializeField]
	private bool Refresh;

	private BetterList<TeQuanActivityMainPart.ActivityBtnHander> mActivityBtnHanderList0 = new BetterList<TeQuanActivityMainPart.ActivityBtnHander>();

	private BetterList<TeQuanActivityMainPart.ActivityBtnHander> mActivityBtnHanderList1 = new BetterList<TeQuanActivityMainPart.ActivityBtnHander>();

	private SpecPriorityActivityData mSpecPriorityActivityData;

	private ObservableCollection mBtnObc;

	private bool btnHaveInit;

	private int mRefreshCount;

	private class ActivityBtnHander
	{
		public ActivityBtnHander(GameObject Root, int Id, int myType)
		{
			Root.SetActive(true);
			this.mID = Id;
			this.mRoot = Root;
			this.mMyType = myType;
			Root.name = this.mMyType.ToString() + "_" + this.mID;
			UIEventListener.Get(this.mRoot).onPress = delegate(GameObject g, bool s)
			{
				if (s)
				{
					this.BakSp.spriteName = "zuoceanniu_2";
					this.ValueLabel.color = NGUIMath.HexToColorEx(16777215U);
				}
				else
				{
					if (this.mBtnIsLock)
					{
						this.BakSp.spriteName = "zuoceanniu_3";
					}
					else
					{
						this.BakSp.spriteName = "zuoceanniu_1";
					}
					this.ValueLabel.color = NGUIMath.HexToColorEx(14337966U);
					if (this.Hander != null)
					{
						this.Hander(g, new DPSelectedItemEventArgs
						{
							Type = 1,
							ID = this.mID,
							MyID = this.mMyType,
							Index = this.mIndex
						});
					}
				}
			};
			UIEventListener.Get(this.mRoot).onClick = delegate(GameObject g)
			{
				this.BakSp.spriteName = "zuoceanniu_2";
				this.ValueLabel.color = NGUIMath.HexToColorEx(16777215U);
				if (this.Hander != null)
				{
					this.Hander(g, new DPSelectedItemEventArgs
					{
						Type = 0,
						ID = this.mID,
						Index = this.mIndex,
						MyID = this.mMyType
					});
				}
			};
			this.ValueLabel = Root.transform.FindChild("ValueLabel").GetComponent<UILabel>();
			this.BakSp = Root.transform.FindChild("BakSp").GetComponent<UISprite>();
			this._SeverType = Root.transform.FindChild("SeverType").GetComponent<UISprite>();
			this.Suo = Root.transform.FindChild("Suo").gameObject;
			this.LianZi = Root.transform.FindChild("LianZi").gameObject;
			this.BtnIsLock = false;
			TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(this.mID);
		}

		public int Index
		{
			get
			{
				return this.mIndex;
			}
			set
			{
				this.mIndex = value;
			}
		}

		public int MyType
		{
			get
			{
				return this.mMyType;
			}
			set
			{
				this.mMyType = value;
			}
		}

		public int ID
		{
			get
			{
				return this.mID;
			}
			set
			{
				this.mID = value;
				this.mRoot.name = this.mMyType.ToString() + "_" + this.mID;
			}
		}

		public void SetTipShow(TeQuanJiHuoVO jihuoVO, GameObject GanTanhao)
		{
			if (jihuoVO.tips.ToString() == "2")
			{
				GanTanhao.SetActive(true);
			}
			else
			{
				GanTanhao.SetActive(false);
			}
		}

		public void DestoryPanel()
		{
			UIPanel component = this.mRoot.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}

		public Transform RootTreans
		{
			get
			{
				return this.mRoot.transform;
			}
		}

		public byte SeverType
		{
			set
			{
				if (value == 0 || value == 1)
				{
					this._SeverType.enabled = true;
					this._SeverType.spriteName = ((value != 0) ? "kuafujiaobiao" : "benfujiaobiao");
				}
				else
				{
					this._SeverType.enabled = false;
				}
			}
		}

		public string BtnText
		{
			get
			{
				return this.mBtnText;
			}
			set
			{
				this.mBtnText = value;
				this.ValueLabel.text = this.mBtnText;
			}
		}

		public bool BtnIsLock
		{
			get
			{
				return this.mBtnIsLock;
			}
			set
			{
				this.mBtnIsLock = value;
				if (this.mBtnIsLock)
				{
					this.BakSp.spriteName = "zuoceanniu_3";
					this.ValueLabel.color = NGUIMath.HexToColorEx(8421505U);
				}
				else
				{
					this.BakSp.spriteName = "zuoceanniu_2";
					this.ValueLabel.color = NGUIMath.HexToColorEx(16777215U);
				}
				this.Suo.SetActive(value);
				this.BSelect = this.mBSelect;
			}
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
				if (value)
				{
					this.BakSp.spriteName = "zuoceanniu_2";
					this.ValueLabel.color = NGUIMath.HexToColorEx(16777215U);
				}
				else
				{
					if (this.mBtnIsLock)
					{
						this.BakSp.spriteName = "zuoceanniu_3";
					}
					else
					{
						this.BakSp.spriteName = "zuoceanniu_1";
					}
					this.ValueLabel.color = NGUIMath.HexToColorEx(14337966U);
				}
			}
		}

		public DPSelectedItemEventHandler Hander;

		private int mID;

		private int mMyType;

		private int mIndex;

		private GameObject mRoot;

		private UILabel ValueLabel;

		private UISprite BakSp;

		private UISprite _SeverType;

		private GameObject Suo;

		private GameObject LianZi;

		private string mBtnText = string.Empty;

		private bool mBtnIsLock;

		private bool mBSelect;
	}
}
