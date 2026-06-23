using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class MenuIconBoxContainer : UserControl
{
	protected override void InitializeComponent()
	{
		this.BodyTopStartPos = new Vector3(0f, 224f, 0f);
		this.BodyTopEndPos = new Vector3(0f, 0f, 0f);
		this.BodyTop.transform.localPosition = this.BodyTopStartPos;
		UIEventListener.Get(this.MenuIconRole.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowHelpAnim(0, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 0
			});
			SystemHelpMgr.OnAction(UIObjIDs.GameRenWu, HelpStateEvents.Clicked, -1);
		};
		UIEventListener.Get(this.MenuIconBag.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowHelpAnim(1, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 1
			});
			SystemHelpMgr.OnAction(UIObjIDs.GameBag, HelpStateEvents.Clicked, -1);
		};
		UIEventListener.Get(this.MenuIconLianlu.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowHelpAnim(2, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 2
			});
			SystemHelpMgr.OnAction(UIObjIDs.GameLianLu, HelpStateEvents.Clicked, -1);
		};
		UIEventListener.Get(this.MenuIconHecheng.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowHelpAnim(3, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 3
			});
			SystemHelpMgr.OnAction(UIObjIDs.GameHeCheng, HelpStateEvents.Clicked, -1);
		};
		UIEventListener.Get(this.MenuIconShejiao.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowHelpAnim(4, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 4
			});
		};
		UIEventListener.Get(this.MenuIconFamily.gameObject).onClick = delegate(GameObject s)
		{
			this.ShowHelpAnim(5, 0);
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 7
			});
		};
		UIEventListener.Get(this.MenuIconTop.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 5
			});
		};
		UIEventListener.Get(this.MenuIconTask.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 6
			});
		};
		UIEventListener.Get(this.MenuIconShanCheng.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 8
			});
		};
		UIEventListener.Get(this.MenuIconTujian.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 9
			});
		};
		UIEventListener.Get(this.MenuIconChengJiu.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 10
			});
		};
		UIEventListener.Get(this.MenuIconJingLing.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 11
			});
		};
		UIEventListener.Get(this.MenuIconMarry.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 12
			});
		};
		UIEventListener.Get(this.MenuIconTianFu.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 13
			});
		};
		UIEventListener.Get(this.MenuIconShengBei.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 14
			});
		};
		UIEventListener.Get(this.MenuMeiLanShu.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 16
			});
		};
		UIEventListener.Get(this.MenuFluorescentDiamond.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 15
			});
		};
		UIEventListener.Get(this.MenuLingDi.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 18
			});
		};
		UIEventListener.Get(this.MenuIconCangBaoMiJing.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 19
			});
		};
		UIEventListener.Get(this.MenuIconKaLuoPai.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 20
			});
		};
		UIEventListener.Get(this.MenuIconFashionForge.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 21
			});
		};
		UIEventListener.Get(this.MenuIconLoversWish.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 22
			});
		};
		UIEventListener.Get(this.MenuIconShiPin.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 23
			});
		};
		UIEventListener.Get(this.MenuIconShenQiSystem.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 24
			});
		};
		UIEventListener.Get(this.MenuIconArmyGroup.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 25
			});
		};
		UIEventListener.Get(this.MenuIconHuiJi.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 26
			});
		};
		UIEventListener.Get(this.MenuIconAlchemy.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 27
			});
		};
		UIEventListener.Get(this.MenuIconShenShi.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 28
			});
		};
		UIEventListener.Get(this.MenuIconJueXing.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 29
			});
		};
		UIEventListener.Get(this.MenuIconZuoQi.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 30
			});
		};
		UIEventListener.Get(this.MenuIconShiLi.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 31
			});
		};
		UIEventListener.Get(this.MenuIconShenShengHuDuan.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 32
			});
		};
		UIEventListener.Get(this.MenuIconShenHun.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 33
			});
		};
		UIEventListener.Get(this.MenuIconRebirth.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 34
			});
		};
		UIEventListener.Get(this.MenuIconZhanDui.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = 35
			});
		};
		ActivityTipManager.RegActivityTipItem(15005, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(9000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(9001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(3037, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(15000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(16000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(16001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(15050, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(15011, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(15012, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(18003, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(32000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(31000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(19000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(7500, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(21000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.RegActivityTipItem(21001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		this.BodyTop.Visibility = false;
	}

	private void OnActivityStateChanged(int type, ActivityTipItem args)
	{
		if (type == 9000)
		{
			this.TipIconChengJiu.SetActive(args.IsActive);
		}
		else if (type == 9001)
		{
			this.TipIconTop.SetActive(args.IsActive);
		}
		else if (type == 3037)
		{
			this.TipIconFuMo.SetActive(args.IsActive);
		}
		else if (type == 15000)
		{
			this.TipIconFamily.SetActive(args.IsActive);
		}
		else if (type == 16000)
		{
			if (this.TipIconJingLing)
			{
				this.TipIconJingLing.SetActive(args.IsActive);
			}
		}
		else if (type == 16001)
		{
			if (this.TipIconJingLing)
			{
				this.TipIconJingLing.SetActive(args.IsActive);
			}
		}
		else if (type == 15050)
		{
			if (null != this.TipIconLingDi)
			{
				this.TipIconLingDi.SetActive(args.IsActive);
			}
		}
		else if (type == 15005)
		{
			if (null != this.TipArnyGroup)
			{
				this.TipArnyGroup.SetActive(args.IsActive);
			}
		}
		else if (type == 15011)
		{
			if (null != this.TipIconPKLovers)
			{
				this.TipIconPKLovers.SetActive(args.IsActive);
			}
		}
		else if (type == 15012)
		{
			if (null != this.TipIconLoversWish)
			{
				this.TipIconLoversWish.SetActive(args.IsActive);
			}
		}
		else if (type == 18003)
		{
			if (null != this.TipJueXing)
			{
				this.TipJueXing.SetActive(args.IsActive);
			}
		}
		else if (type == 32000)
		{
			if (null != this.TipRole)
			{
				this.TipRole.SetActive(args.IsActive);
			}
		}
		else if (type == 7500)
		{
			if (null != this.TipPaiHang)
			{
				this.TipPaiHang.SetActive(args.IsActive);
			}
		}
		else if (type == 31000)
		{
			if (null != this.TipLianLu && (GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuiJia) || GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ZhuangBeiPeiYang) || GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.LianLu)))
			{
				this.TipLianLu.SetActive(args.IsActive);
			}
		}
		else if (type == 19000)
		{
			if (null != this.TipTuJian)
			{
				this.TipTuJian.SetActive(args.IsActive);
			}
		}
		else if ((type == 21000 || type == 21001) && GongnengYugaoMgr.IsGongNengOpened(GongNengIDs.ChongSheng))
		{
			this.TipIconRebirth.SetActive(true);
		}
	}

	public void CheckStaticMenuTipsIconState()
	{
		this.CheckShengWuStat();
		this.CheckFluorescentDiamondIconState();
	}

	public void CheckShengWuStat()
	{
		bool active = Global.InitTiShi();
		this.TipIconShengWu.SetActive(active);
	}

	public void CheckFluorescentDiamondIconState()
	{
		bool active = Global.IsUpgradableDiamond();
		this.TipIconFluorescentDiamond.SetActive(active);
	}

	protected override void OnDestroy()
	{
		ActivityTipManager.UnRegActivityTipItem(9000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(9001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(3037, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(15000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(16000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(16001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(15050, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(15011, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(15012, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(15005, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(18003, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(32000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(31000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(19000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(21000, new ActivityTipEventHandler(this.OnActivityStateChanged));
		ActivityTipManager.UnRegActivityTipItem(21001, new ActivityTipEventHandler(this.OnActivityStateChanged));
		base.OnDestroy();
	}

	public void PrepareAddIconAnim(int order)
	{
		for (int i = 1; i <= this.IconOpenArr.Length - 1; i++)
		{
			this.IconOpenArr[i] = GongnengYugaoMgr.IsIconOpened(this.IconOrder[i]);
		}
		int num = -1;
		for (int j = 0; j < this.IconOpenArr.Length; j++)
		{
			if (this.IconOrder[j] == order)
			{
				num = j;
			}
		}
		if (0 < num && !this.Icons[num].gameObject.activeSelf)
		{
			this.AddedOrder = order;
			this.BodyIconAddAnimStart = Time.time;
			this.BodyIconAddAnimEnd = this.BodyIconAddAnimStart + 0.75f;
		}
	}

	public void RefreshIconByOrder(int order, bool bShow)
	{
		int num = this.IconOrder.IndexOf(order);
		if (num < this.Icons.Length)
		{
			this.Icons[num].gameObject.SetActive(bShow);
		}
		this.ReSetIconPos();
	}

	public void ReSetIconPos()
	{
		for (int i = 1; i < this.IconOpenArr.Length; i++)
		{
			this.IconOpenArr[i] = GongnengYugaoMgr.IsIconOpened(this.IconOrder[i]);
		}
		int num = 0;
		for (int j = 0; j < this.IconOpenArr.Length; j++)
		{
			if (this.IconOpenArr[j])
			{
				num++;
			}
		}
		int num2 = 10;
		int num3 = 10;
		int num4 = 10;
		int num5 = 10;
		for (int k = this.IconOpenArr.Length - 1; k >= 1; k--)
		{
			bool flag = this.IconOpenArr[k];
			this.Icons[k].gameObject.SetActive(flag);
			if (num > 32)
			{
				if (!flag)
				{
					if (k > 32)
					{
						this.Icons[k].transform.localPosition = this.BodyIconFourSize + this.BodyIconIncSize * (float)(num5 - 1);
					}
					else
					{
						this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)(num4 - 1);
					}
				}
				else if (k > 32)
				{
					num5--;
					this.Icons[k].transform.localPosition = this.BodyIconFourSize + this.BodyIconIncSize * (float)num5;
					num--;
				}
				else
				{
					num4--;
					this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)num4;
					num--;
				}
			}
			else if (num > 21)
			{
				if (!flag)
				{
					this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)(num4 - 1);
				}
				else
				{
					num4--;
					this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)num4;
					num--;
				}
			}
			else if (k > 10)
			{
				if (!flag)
				{
					this.Icons[k].transform.localPosition = this.BodyIconVecFistSize + this.BodyIconIncSize * (float)(num3 - 1);
					if (num >= 21)
					{
						if (k > 21)
						{
							this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)(num4 - 1);
						}
						else
						{
							this.Icons[k].transform.localPosition = this.BodyIconVecFistSize + this.BodyIconIncSize * (float)num3;
						}
					}
				}
				else
				{
					num3--;
					this.Icons[k].transform.localPosition = this.BodyIconVecFistSize + this.BodyIconIncSize * (float)num3;
					if (0 > num3)
					{
						Global.m_MenuIconBoxYinCan = true;
						if (!Global.m_MenuIconBoxFlag)
						{
							PlayZone.GlobalPlayZone.SetUIMenuIconBoxFlag(false);
						}
					}
				}
			}
			else if (!flag)
			{
				this.Icons[k].transform.localPosition = this.BodyIconFirstPos + this.BodyIconIncSize * (float)(num2 - 1);
			}
			else
			{
				num2--;
				this.Icons[k].transform.localPosition = this.BodyIconFirstPos + this.BodyIconIncSize * (float)num2;
			}
		}
	}

	private void LateUpdate()
	{
		if (this.BodyIconAddAnimStart > 0f && this.BodyIconAddAnimEnd > 0f)
		{
			if (Time.time > this.BodyIconAddAnimEnd || Time.time < this.BodyIconAddAnimStart)
			{
				this.BodyIconAddAnimStart = 0f;
				this.BodyIconAddAnimEnd = 0f;
				this.ReSetIconPos();
				return;
			}
			int num = 0;
			for (int i = 0; i < this.IconOpenArr.Length; i++)
			{
				if (this.IconOpenArr[i])
				{
					num++;
				}
			}
			int num2 = 10;
			int num3 = 10;
			int num4 = 10;
			int num5 = 10;
			int num6 = this.IconOpenArr.Length - 1;
			int num7 = -1;
			int num8 = num;
			for (int j = this.IconOpenArr.Length - 1; j >= 1; j--)
			{
				bool flag = this.IconOpenArr[j];
				if (flag)
				{
					if (num8 <= 21)
					{
						if (this.AddedOrder <= this.IconOrder[j])
						{
							num7 = 2;
							break;
						}
						num7 = 3;
						break;
					}
					else
					{
						num8--;
					}
				}
			}
			for (int k = this.IconOpenArr.Length - 1; k >= 1; k--)
			{
				bool flag2 = this.IconOpenArr[k];
				if (flag2)
				{
					num6--;
				}
				if (num > 32)
				{
					if (!flag2)
					{
						if (k > 32)
						{
							this.Icons[k].transform.localPosition = this.BodyIconFourSize + this.BodyIconIncSize * (float)(num5 - 1);
						}
						else
						{
							this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)(num4 - 1);
						}
					}
					else if (this.IconOrder[k] >= this.AddedOrder)
					{
						if (k > 32)
						{
							num5--;
							this.Icons[k].transform.localPosition = this.BodyIconFourSize + this.BodyIconIncSize * (float)num5;
							num--;
						}
						else
						{
							num4--;
							this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)num4;
							num--;
						}
					}
					else
					{
						num5--;
						Vector3 vector = Vector3.Slerp(this.BodyIconIncSize, Vector3.zero, (Time.time - this.BodyIconAddAnimStart) / (this.BodyIconAddAnimEnd - this.BodyIconAddAnimStart));
						vector.x = (float)((int)vector.x);
						vector.y = (float)((int)vector.y);
						vector.z = (float)((int)vector.z);
						vector = this.BodyIconFourSize + this.BodyIconIncSize * (float)num5 + vector;
						this.Icons[k].transform.localPosition = vector;
						num--;
					}
				}
				else if (num > 21)
				{
					if (!flag2)
					{
						this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)(num4 - 1);
					}
					else if (this.IconOrder[k] >= this.AddedOrder)
					{
						num4--;
						this.Icons[k].transform.localPosition = this.BodyIconThreeSize + this.BodyIconIncSize * (float)num4;
						num--;
					}
					else
					{
						num4--;
						Vector3 vector = Vector3.Slerp(this.BodyIconIncSize, Vector3.zero, (Time.time - this.BodyIconAddAnimStart) / (this.BodyIconAddAnimEnd - this.BodyIconAddAnimStart));
						vector.x = (float)((int)vector.x);
						vector.y = (float)((int)vector.y);
						vector.z = (float)((int)vector.z);
						vector = this.BodyIconThreeSize + this.BodyIconIncSize * (float)num4 + vector;
						this.Icons[k].transform.localPosition = vector;
						num--;
					}
				}
				else if (k > 10)
				{
					if (!flag2)
					{
						this.Icons[k].transform.localPosition = this.BodyIconVecFistSize + this.BodyIconIncSize * (float)(num3 - 1);
					}
					else if (this.IconOrder[k] >= this.AddedOrder)
					{
						num3--;
						if (num3 == 0)
						{
							Global.m_MenuIconBoxYinCan = true;
						}
						this.Icons[k].transform.localPosition = this.BodyIconVecFistSize + this.BodyIconIncSize * (float)num3;
					}
					else if (num7 == 2)
					{
						num3--;
						Vector3 vector = Vector3.Slerp(this.BodyIconIncSize, Vector3.zero, (Time.time - this.BodyIconAddAnimStart) / (this.BodyIconAddAnimEnd - this.BodyIconAddAnimStart));
						vector.x = (float)((int)vector.x);
						vector.y = (float)((int)vector.y);
						vector.z = (float)((int)vector.z);
						vector = this.BodyIconVecFistSize + this.BodyIconIncSize * (float)num3 + vector;
						this.Icons[k].transform.localPosition = vector;
					}
					if (0 > num3)
					{
						Global.m_MenuIconBoxYinCan = true;
						if (!Global.m_MenuIconBoxFlag)
						{
							PlayZone.GlobalPlayZone.SetUIMenuIconBoxFlag(false);
						}
					}
				}
				else if (this.AddedOrder <= 10 && k <= 10)
				{
					if (!flag2)
					{
						this.Icons[k].transform.localPosition = this.BodyIconFirstPos + this.BodyIconIncSize * (float)(num2 - 1);
					}
					else if (this.IconOrder[k] >= this.AddedOrder)
					{
						num2--;
						this.Icons[k].transform.localPosition = this.BodyIconFirstPos + this.BodyIconIncSize * (float)num2;
					}
					else
					{
						num2--;
						Vector3 vector = Vector3.Slerp(this.BodyIconIncSize, Vector3.zero, (Time.time - this.BodyIconAddAnimStart) / (this.BodyIconAddAnimEnd - this.BodyIconAddAnimStart));
						vector.x = (float)((int)vector.x);
						vector.y = (float)((int)vector.y);
						vector.z = (float)((int)vector.z);
						vector = this.BodyIconFirstPos + this.BodyIconIncSize * (float)num2 + vector;
						this.Icons[k].transform.localPosition = vector;
					}
				}
			}
		}
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			if (id == 0)
			{
				SystemHelpPart.SetMask(this.MenuIconRole, default(Vector4));
			}
			else if (id == 1)
			{
				SystemHelpPart.SetMask(this.MenuIconBag, default(Vector4));
			}
			else if (id == 2)
			{
				SystemHelpPart.SetMask(this.MenuIconLianlu, default(Vector4));
			}
			else if (id == 3)
			{
				SystemHelpPart.SetMask(this.MenuIconHecheng, default(Vector4));
			}
			else if (id == 4)
			{
				SystemHelpPart.SetMask(this.MenuIconShejiao, default(Vector4));
			}
			else if (id == 5)
			{
				SystemHelpPart.SetMask(this.MenuIconFamily, default(Vector4));
			}
			else if (id == 6)
			{
				SystemHelpPart.SetMask(this.MenuIconTop, default(Vector4));
			}
			else if (id == 7)
			{
				SystemHelpPart.SetMask(this.MenuIconTask, default(Vector4));
			}
			else if (id == 8)
			{
				SystemHelpPart.SetMask(this.MenuIconTujian, default(Vector4));
			}
			else if (id == 11)
			{
				SystemHelpPart.SetMask(this.MenuIconJingLing, default(Vector4));
			}
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	public bool BodyVisible
	{
		get
		{
			return this._BodyVisible;
		}
		set
		{
			if (this.isStarted)
			{
				return;
			}
			this._BodyVisible = value;
			this.BodyTop.Visibility = true;
			this.isStarted = true;
			if (value)
			{
				this.tp = TweenPosition.Begin(this.BodyTop.gameObject, 0.75f, this.BodyTopStartPos, this.BodyTopEndPos);
				this.tp.method = 2;
			}
			else
			{
				this.tp = TweenPosition.Begin(this.BodyTop.gameObject, 0.75f, this.BodyTopEndPos, this.BodyTopStartPos);
				this.tp.method = 3;
			}
			if (null != this.tp)
			{
				this.tp.onFinished = new UITweener.OnFinished(this.onFinished);
			}
		}
	}

	private void onFinished(UITweener tween)
	{
		if (this.isStarted)
		{
			this.isStarted = false;
			this.BodyTop.Visibility = this._BodyVisible;
			if (!this._BodyVisible)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 1,
					ID = 0
				});
			}
			else
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					IDType = 1,
					ID = 1
				});
			}
		}
	}

	public void PlayAnimation(int idx)
	{
		switch (idx)
		{
		case 6:
		{
			Animation component = this.MenuIconFamily.transform.parent.parent.GetComponent<Animation>();
			if (component != null)
			{
				component.Play();
			}
			break;
		}
		}
	}

	public void StopAnimation(int idx)
	{
		switch (idx)
		{
		case 6:
		{
			Animation component = this.MenuIconFamily.transform.parent.parent.GetComponent<Animation>();
			if (component != null)
			{
				component.Stop();
			}
			this.MenuIconFamily.transform.parent.parent.localScale = new Vector3(1f, 1f, 1f);
			break;
		}
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public SpriteSL BodyTop;

	public Transform Top;

	public SpriteSL BodyLeft;

	public UIButton MenuIconRole;

	public UIButton MenuIconBag;

	public UIButton MenuIconLianlu;

	public UIButton MenuIconHecheng;

	public UIButton MenuIconShejiao;

	public UIButton MenuIconFamily;

	public UIButton MenuIconShanCheng;

	public UIButton MenuIconTujian;

	public UIButton MenuIconTop;

	public UIButton MenuIconTask;

	public UIButton MenuIconChengJiu;

	public UIButton MenuIconJingLing;

	public UIButton MenuIconMarry;

	public UIButton MenuIconTianFu;

	public UIButton MenuIconShengBei;

	public UIButton MenuMeiLanShu;

	public UIButton MenuFluorescentDiamond;

	public UIButton MenuLingDi;

	public UIButton MenuIconKaLuoPai;

	public UIButton MenuIconCangBaoMiJing;

	public UIButton MenuIconFashionForge;

	public UIButton MenuIconLoversWish;

	public UIButton MenuIconShiPin;

	public UIButton MenuIconArmyGroup;

	public UIButton MenuIconShenQiSystem;

	public UIButton MenuIconHuiJi;

	public UIButton MenuIconAlchemy;

	public UIButton MenuIconShenShi;

	public UIButton MenuIconJueXing;

	public UIButton MenuIconZuoQi;

	public UIButton MenuIconShiLi;

	public UIButton MenuIconShenHun;

	public UIButton MenuIconShenShengHuDuan;

	public UIButton MenuIconRebirth;

	public UIButton MenuIconZhanDui;

	public GameObject TipIconChengJiu;

	public GameObject TipIconTop;

	public GameObject TipIconFuMo;

	public GameObject TipIconFamily;

	public GameObject TipIconJingLing;

	public GameObject TipIconShengWu;

	public GameObject TipIconFluorescentDiamond;

	public GameObject TipIconLingDi;

	public GameObject TipIconPKLovers;

	public GameObject TipIconLoversWish;

	public GameObject TipArnyGroup;

	public GameObject TipJueXing;

	public GameObject TipRole;

	public GameObject TipLianLu;

	public GameObject TipTuJian;

	public GameObject TipPaiHang;

	public GameObject TipIconRebirth;

	public Transform[] Icons;

	private Vector3 BodyTopStartPos;

	private Vector3 BodyTopEndPos;

	public bool isStarted;

	private TweenPosition tp;

	private int AddedOrder;

	private float BodyIconAddAnimStart;

	private float BodyIconAddAnimEnd;

	private bool[] IconOpenArr = new bool[34];

	public int[] IconOrder = new int[1];

	private Vector3 BodyIconFirstPos = new Vector3(0f, 0f, 0f);

	private Vector3 BodyIconIncSize = new Vector3(64f, 0f, 0f);

	private Vector3 BodyIconVecFistSize = new Vector3(64f, -64f, 0f);

	private Vector3 BodyIconThreeSize = new Vector3(64f, -128f, 0f);

	private Vector3 BodyIconFourSize = new Vector3(64f, -192f, 0f);

	private bool _BodyVisible;
}
