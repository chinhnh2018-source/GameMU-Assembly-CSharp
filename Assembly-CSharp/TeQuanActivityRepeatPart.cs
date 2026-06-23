using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class TeQuanActivityRepeatPart : UserControl, BaseTeQuanActivityPart
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	public override void Update()
	{
		base.Update();
	}

	private void InitActivityItems()
	{
		byte b = 0;
		while ((int)b < this._ActivityStateItems.Length)
		{
			TeQuanActivityRepeatPart.ItemState itemState = new TeQuanActivityRepeatPart.ItemState(this._ActivityStateItems[(int)b], (int)(b + 1));
			itemState.ShowTeXiao = false;
			itemState.OnClick = new DPSelectedItemEventHandler(this.ActivityOnCkick);
			this.mItemsStateList.Add(itemState);
			b += 1;
		}
	}

	private void ActivityOnCkick(object sender, DPSelectedItemEventArgs args)
	{
		TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(args.ID);
		if (teQuanJiHuoVOByID != null)
		{
			int num = teQuanJiHuoVOByID.ID % 4;
			if (num == 1)
			{
				this._ActivityTipsRoot.transform.localPosition = new Vector3(-245f, 0f, -10f);
			}
			else if (num == 2)
			{
				this._ActivityTipsRoot.transform.localPosition = new Vector3(-50f, 65f, -10f);
			}
			else if (num == 3)
			{
				this._ActivityTipsRoot.transform.localPosition = new Vector3(145f, 65f, -10f);
			}
			else
			{
				this._ActivityTipsRoot.transform.localPosition = new Vector3(260f, 0f, -10f);
			}
			this._ActivityTipsRoot.SetActive(true);
			this._TipsLabel.text = this.ParseStrNewLine(teQuanJiHuoVOByID.Tips);
			this._TipsUICollider.updataNow = true;
		}
	}

	private string ParseStrNewLine(string str)
	{
		if (!string.IsNullOrEmpty(str))
		{
			return str.Replace("&#xA", "\n\r");
		}
		return str;
	}

	private void InitPrefabText()
	{
		try
		{
			this._ReChargeBtn.Text = Global.GetLang("充值");
			this._JuanZengUseZhuanShi.Text = Global.GetLang("钻石捐赠");
			this._JuanZengUseGoods.Text = Global.GetLang("道具捐赠");
			this._JuanZengFanKuiMiaoShulabel1.text = string.Empty;
			this._JuanZengFanKuiMiaoShulabel2.text = string.Empty;
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
			UIEventListener.Get(this._ActivityTipsBakBoxMask).onClick = delegate(GameObject g)
			{
				this._ActivityTipsRoot.SetActive(false);
			};
			this._ReChargeBtn.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (0f < Global.GetBtnCD(this._ReChargeBtn.GetInstanceID()))
				{
					return;
				}
				Global.AddBtnCD(this._ReChargeBtn.GetInstanceID(), 1f);
				if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 1)
				{
					PlayZone.GlobalPlayZone.ShowChongZhiWindow();
					if (this.Hander != null)
					{
						this.Hander(null, null);
					}
				}
				else if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 2)
				{
					PlayZone.GlobalPlayZone.ShowChongZhiWindow();
					if (this.Hander != null)
					{
						this.Hander(null, null);
					}
				}
				else if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 3)
				{
					LinkedList<string> linkedList = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
					if (linkedList.Contains("JieRi"))
					{
						PlayZone.GlobalPlayZone.OpenJieRihuodongWindow();
						if (this.Hander != null)
						{
							this.Hander(null, null);
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("直购活动暂未开启"), 10, 3);
					}
				}
				else if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 4)
				{
					LinkedList<string> linkedList2 = HuoDongCommonFlag.TopIconTreeActivityActivedLst();
					if (linkedList2.Contains("JieRi"))
					{
						PlayZone.GlobalPlayZone.OpenJieRihuodongWindow();
						if (this.Hander != null)
						{
							this.Hander(null, null);
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("直购活动暂未开启"), 10, 3);
					}
				}
				else if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 7 || this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 8)
				{
					PlayZone.GlobalPlayZone.ShowChongZhiWindow();
					if (this.Hander != null)
					{
						this.Hander(null, null);
					}
				}
				else if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 9 || this.mTeQuanTiaoJianVO.TiaoJianLeiXing == 10)
				{
					PlayZone.GlobalPlayZone.ProcessGuideRequest(new DPSelectedItemEventArgs
					{
						ID = 303
					});
					if (this.Hander != null)
					{
						this.Hander(null, null);
					}
				}
			};
			this._JuanZengUseGoods.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mSpecPriorityActInfo != null && 0 < this.mTeQuanTiaoJianVO.MeiRiShangXian)
				{
					if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing % 2 == 0)
					{
						if (this.mTeQuanTiaoJianVO.MeiRiShangXian <= this.DonateNumKF)
						{
							Super.HintMainText(Global.GetLang("今日捐赠次数已达上限"), 10, 3);
							return;
						}
					}
					else if (this.mTeQuanTiaoJianVO.MeiRiShangXian <= this.DonateNum)
					{
						Super.HintMainText(Global.GetLang("今日捐赠次数已达上限"), 10, 3);
						return;
					}
				}
				string[] array = this.mTeQuanTiaoJianVO._GuDingLeiXing.Split(new char[]
				{
					'|'
				});
				int goodsId = array[0].Split(new char[]
				{
					','
				})[0].SafeToInt32(0);
				int num = array[0].Split(new char[]
				{
					','
				})[1].SafeToInt32(0);
				int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(goodsId);
				if (num > roleGoodsNumberCountByGoodsID)
				{
					Super.HintMainText(Global.GetLang("所需物品不足"), 10, 3);
				}
				else
				{
					Super.ShowNetWaiting(null);
					GameInstance.Game.SendTeQuanActiviteRoleJuanZeng(this.mTeQuanTiaoJianVO.ID, 0);
				}
			};
			this._JuanZengUseZhuanShi.MouseLeftButtonUp = delegate(object e, MouseEvent s)
			{
				if (this.mTeQuanTiaoJianVO != null && 0 < this.mTeQuanTiaoJianVO.MeiRiShangXian)
				{
					if (this.mTeQuanTiaoJianVO.TiaoJianLeiXing % 2 == 0)
					{
						if (this.mTeQuanTiaoJianVO.MeiRiShangXian <= this.DonateNumKF)
						{
							Super.HintMainText(Global.GetLang("今日捐赠次数已达上限"), 10, 3);
							return;
						}
					}
					else if (this.mTeQuanTiaoJianVO.MeiRiShangXian <= this.DonateNum)
					{
						Super.HintMainText(Global.GetLang("今日捐赠次数已达上限"), 10, 3);
						return;
					}
				}
				string[] array = this.mTeQuanTiaoJianVO._GuDingLeiXing.Split(new char[]
				{
					'|'
				});
				if (Global.Data.roleData.UserMoney >= array[1].SafeToInt32(0))
				{
					if (Global.GetZuanShi(ZuanShiPartClass.ZuiQiShengYin))
					{
						if (this.messageBoxWindow != null)
						{
							Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
						}
						this.messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 2, Global.GetLang("提示"), Global.GetLang("花费") + array[1] + Global.GetLang("钻石捐赠一次"), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
						if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
						{
							this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked = Global.ZuanShiIsCheck;
						}
						this.messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
						{
							int messageBoxReturn = this.messageBoxWindow.MessageBoxReturn;
							if (this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>() != null)
							{
								Global.SetZuanShi(ZuanShiPartClass.ZuiQiShengYin, !this.messageBoxWindow.GetComponentInChildren<MyMessageBoxPart>().CheckBox.isChecked);
							}
							Super.CloseMessageBox(Super.MainWindowRoot, this.messageBoxWindow);
							if (messageBoxReturn == 0)
							{
								Super.ShowNetWaiting(null);
								GameInstance.Game.SendTeQuanActiviteRoleJuanZeng(this.mTeQuanTiaoJianVO.ID, 1);
							}
							return true;
						};
					}
					else
					{
						Super.ShowNetWaiting(null);
						GameInstance.Game.SendTeQuanActiviteRoleJuanZeng(this.mTeQuanTiaoJianVO.ID, 1);
					}
				}
				else
				{
					Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
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

	private void RefreshPrence(float[] percent)
	{
		byte b = 0;
		while ((int)b < percent.Length)
		{
			percent[(int)b] = Mathf.Clamp01(percent[(int)b]);
			b += 1;
		}
		int num = 0;
		byte b2 = 0;
		while ((int)b2 < percent.Length)
		{
			if (1f > percent[(int)b2])
			{
				num = (int)b2;
				break;
			}
			if ((int)b2 == percent.Length - 1)
			{
				num = (int)b2;
			}
			b2 += 1;
		}
		if (num == 0)
		{
			float num2 = percent[0];
			if (0.5 > (double)percent[0])
			{
				num2 = 0.25f + 0.65f * percent[0];
			}
			else if (0.5 < (double)percent[0])
			{
				num2 = 0.65f * percent[0];
			}
			this._ProgressSps[0].fillAmount = percent[0];
			byte b3 = 1;
			while ((int)b3 < this._ProgressSps.Length)
			{
				this._ProgressSps[(int)b3].fillAmount = 0f;
				b3 += 1;
			}
		}
		else if (num == 1)
		{
			this._ProgressSps[0].fillAmount = 1f;
			this._ProgressSps[1].fillAmount = Mathf.Clamp01(percent[num] * 2f);
			this._ProgressSps[2].fillAmount = Mathf.Clamp01(percent[num] * 2f - 1f);
			byte b4 = 3;
			while ((int)b4 < this._ProgressSps.Length)
			{
				this._ProgressSps[(int)b4].fillAmount = 0f;
				b4 += 1;
			}
		}
		else if (num == 2)
		{
			this._ProgressSps[3].fillAmount = Mathf.Clamp01(percent[num] * 2f);
			this._ProgressSps[4].fillAmount = Mathf.Clamp01(percent[num] * 2f - 1f);
			byte b5 = 0;
			while ((int)b5 < this._ProgressSps.Length)
			{
				if (b5 < 3)
				{
					this._ProgressSps[(int)b5].fillAmount = 1f;
				}
				else if (b5 > 4)
				{
					this._ProgressSps[(int)b5].fillAmount = 0f;
				}
				b5 += 1;
			}
		}
		else
		{
			this._ProgressSps[5].fillAmount = Mathf.Clamp01(percent[num] * 2f);
			this._ProgressSps[6].fillAmount = Mathf.Clamp01(percent[num] * 2f - 1f);
			byte b6 = 0;
			while ((int)b6 < this._ProgressSps.Length - 2)
			{
				this._ProgressSps[(int)b6].fillAmount = 1f;
				b6 += 1;
			}
		}
	}

	private void RefreshPrence(float percent)
	{
		if (0f >= percent)
		{
			percent = 0.035f;
		}
		if (1f <= percent)
		{
			percent = 0.999f;
		}
		int num = 0;
		float[] array = new float[2];
		if (this._SubsectionEX[0] <= percent && this._SubsectionEX[1] >= percent)
		{
			array[0] = this._SubsectionEX[0];
			array[1] = this._SubsectionEX[1];
			num = 0;
		}
		if (this._SubsectionEX[1] < percent && this._SubsectionEX[2] >= percent)
		{
			array[0] = this._SubsectionEX[1];
			array[1] = this._SubsectionEX[2];
			num = 1;
		}
		if (this._SubsectionEX[2] < percent && this._SubsectionEX[3] >= percent)
		{
			array[0] = this._SubsectionEX[2];
			array[1] = this._SubsectionEX[3];
			num = 2;
		}
		if (this._SubsectionEX[3] < percent && this._SubsectionEX[4] >= percent)
		{
			array[0] = this._SubsectionEX[3];
			array[1] = this._SubsectionEX[4];
			num = 3;
		}
		if (this._SubsectionEX[4] < percent && this._SubsectionEX[5] >= percent)
		{
			array[0] = this._SubsectionEX[4];
			array[1] = this._SubsectionEX[5];
			num = 4;
		}
		if (this._SubsectionEX[5] < percent && this._SubsectionEX[6] >= percent)
		{
			array[0] = this._SubsectionEX[5];
			array[1] = this._SubsectionEX[6];
			num = 5;
		}
		if (this._SubsectionEX[5] < percent && this._SubsectionEX[7] >= percent)
		{
			array[0] = this._SubsectionEX[6];
			array[1] = this._SubsectionEX[7];
			num = 6;
		}
		byte b = 0;
		while ((int)b < this._ProgressSps.Length)
		{
			if ((int)b < num)
			{
				this._ProgressSps[(int)b].fillAmount = 1f;
			}
			else if ((int)b == num)
			{
				float num2 = percent - array[0];
				float fillAmount = num2 / (array[1] - array[0]);
				this._ProgressSps[(int)b].fillAmount = fillAmount;
			}
			else
			{
				this._ProgressSps[(int)b].fillAmount = 0f;
			}
			b += 1;
		}
	}

	public void RefreshPart(TeQuanTiaoJianVO vo, int Num = -1)
	{
		this.mTeQuanTiaoJianVO = vo;
		if (vo != null)
		{
			if (0 >= this.mItemsStateList.size)
			{
				this.InitActivityItems();
			}
			byte b = 0;
			while ((int)b < this.mItemsStateList.size)
			{
				this.mItemsStateList[(int)b].ID = vo.JiHuoIDs[(int)b];
				b += 1;
			}
			int num = 0;
			for (int i = 0; i < this.mTeQuanTiaoJianVO.JiHuoIDs.size; i++)
			{
				TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(this.mTeQuanTiaoJianVO.JiHuoIDs[i]);
				if (num < teQuanJiHuoVOByID.CanShu)
				{
					num = teQuanJiHuoVOByID.CanShu;
				}
				if (i < this.mItemsStateList.size)
				{
					this.mItemsStateList[i].ID = teQuanJiHuoVOByID.ID;
				}
			}
			this._ActivityProgressLabel.text = vo.JieMianTips;
			if (!string.IsNullOrEmpty(this._ActivityProgressLabel.text))
			{
				Vector3 localScale = this._ActivityProgressBgSp.transform.localScale;
				localScale.y = (this._ActivityProgressLabel.relativeSize * this._ActivityProgressLabel.transform.localScale.y).y;
				this._ActivityProgressBgSp.transform.localScale = localScale;
			}
			this._JuanZengFanKuiMiaoShulabel2.text = string.Empty;
			if (vo.TiaoJianLeiXing == 5 || vo.TiaoJianLeiXing == 6)
			{
				this._ChongZhiRoot.SetActive(false);
				this._JuanZengRoot.SetActive(true);
				this._JuanZengFanKuiMiaoShulabel2.text = vo.JieMianTips;
				this.RefreshJuanZeng(vo);
			}
			else
			{
				this._ChongZhiRoot.SetActive(true);
				this._JuanZengRoot.SetActive(false);
			}
			if (vo.TiaoJianLeiXing == 1)
			{
				this._ReChargeBtn.Text = Global.GetLang("充值");
				this._CenterZiTi1.spriteName = "quanminchongzhi";
				this._CenterZiTi2.spriteName = "biaoti_benfu";
			}
			else if (vo.TiaoJianLeiXing == 2)
			{
				this._ReChargeBtn.Text = Global.GetLang("充值");
				this._CenterZiTi1.spriteName = "quanminchongzhi";
				this._CenterZiTi2.spriteName = "biaoti_kuafu";
			}
			else if (vo.TiaoJianLeiXing == 3)
			{
				this._ReChargeBtn.Text = Global.GetLang("购买");
				this._CenterZiTi1.spriteName = "quanminxiangtequan";
				this._CenterZiTi2.spriteName = "biaoti_benfu";
			}
			else if (vo.TiaoJianLeiXing == 4)
			{
				this._ReChargeBtn.Text = Global.GetLang("购买");
				this._CenterZiTi1.spriteName = "quanminxiangtequan";
				this._CenterZiTi2.spriteName = "biaoti_kuafu";
			}
			else if (vo.TiaoJianLeiXing == 5)
			{
				this._JuanZengFanKuiMiaoShulabel1.text = Global.GetLang("捐赠道具可获得活跃值同时可获得以下其中一项奖励");
				this._JuanZengFanKuiMiaoShuSp.spriteName = "biaoti_benfu";
			}
			else if (vo.TiaoJianLeiXing == 6)
			{
				this._JuanZengFanKuiMiaoShulabel1.text = Global.GetLang("捐赠道具可获得活跃值同时可获得以下其中一项奖励");
				this._JuanZengFanKuiMiaoShuSp.spriteName = "biaoti_kuafu";
			}
			else if (vo.TiaoJianLeiXing == 7)
			{
				this._ReChargeBtn.Text = Global.GetLang("充值");
				this._CenterZiTi1.spriteName = "quanminleichong";
				this._CenterZiTi2.spriteName = "biaoti_benfu";
			}
			else if (vo.TiaoJianLeiXing == 8)
			{
				this._ReChargeBtn.Text = Global.GetLang("充值");
				this._CenterZiTi1.spriteName = "quanminleichong";
				this._CenterZiTi2.spriteName = "biaoti_kuafu";
			}
			else if (vo.TiaoJianLeiXing == 9)
			{
				this._ReChargeBtn.Text = Global.GetLang("消费");
				this._CenterZiTi1.spriteName = "quanminleixiao";
				this._CenterZiTi2.spriteName = "biaoti_benfu";
			}
			else if (vo.TiaoJianLeiXing == 10)
			{
				this._ReChargeBtn.Text = Global.GetLang("消费");
				this._CenterZiTi1.spriteName = "quanminleixiao";
				this._CenterZiTi2.spriteName = "biaoti_kuafu";
			}
			string text = Global.GetLang("活动时间") + "\n";
			if (this.mTeQuanTiaoJianVO != null)
			{
				text += Global.GetColorStringForNGUIText(new object[]
				{
					"17e43e",
					string.Concat(new string[]
					{
						this.mTeQuanTiaoJianVO.KaiQiShiJian,
						Global.GetLang("至"),
						"\n",
						this.mTeQuanTiaoJianVO.JieShuShiJian,
						"\n"
					})
				});
			}
			if (Num != -1)
			{
				text = text + Global.GetLang("当前已达成：") + Num;
			}
			this._ActivityTimeInfLabel.text = text;
		}
	}

	private void RefreshJuanZeng(TeQuanTiaoJianVO vo)
	{
		if (vo != null)
		{
			string[] array = vo._GuDingLeiXing.Split(new char[]
			{
				'|'
			});
			int num = array[0].Split(new char[]
			{
				','
			})[1].SafeToInt32(0);
			int roleGoodsNumberCountByGoodsID = Global.GetRoleGoodsNumberCountByGoodsID(array[0].Split(new char[]
			{
				','
			})[0].SafeToInt32(0));
			GoodsData emptyGoodsData = Global.GetEmptyGoodsData(array[0].Split(new char[]
			{
				','
			})[0].SafeToInt32(0), 0, 0, 0, num, 0, 0, 0, 0);
			this.mJuanZengGGoodIcon = this.RefreshGoodIcon(this.mJuanZengGGoodIcon, emptyGoodsData);
			this.mJuanZengGGoodIcon.transform.SetParent(this._JuanZengGoodsRoot.transform, false);
			this.mJuanZengGGoodIcon.TeXiao.transform.localPosition = Vector3.zero;
			this.mJuanZengGGoodIcon.SecondText.Label.supportEncoding = true;
			this.mJuanZengGGoodIcon.SecondText.text = ((num <= roleGoodsNumberCountByGoodsID) ? (roleGoodsNumberCountByGoodsID + "/" + num) : Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				roleGoodsNumberCountByGoodsID + "/" + num
			}));
			this._JuanZengZuanShiNumLabel.text = "X" + array[1];
			this._JuanZengGoodsNumLabel.text = "X" + array[0].Split(new char[]
			{
				','
			})[1];
			Dictionary<int, GGoodIcon>.Enumerator enumerator = this.mJuanZengFanKuiGoodsIcons.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Object @object = null;
				KeyValuePair<int, GGoodIcon> keyValuePair = enumerator.Current;
				if (@object != keyValuePair.Value)
				{
					KeyValuePair<int, GGoodIcon> keyValuePair2 = enumerator.Current;
					keyValuePair2.Value.gameObject.SetActive(false);
				}
			}
			string[] array2 = vo.JiangLiFanKui.Split(new char[]
			{
				'|'
			});
			Transform[] array3 = new Transform[array2.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				GoodsData goodsDataByStr = Global.GetGoodsDataByStr(array2[i], 1);
				if (goodsDataByStr != null)
				{
					if (this.mJuanZengFanKuiGoodsIcons.ContainsKey(goodsDataByStr.GoodsID))
					{
						this.mJuanZengFanKuiGoodsIcons[goodsDataByStr.GoodsID] = this.RefreshGoodIcon(this.mJuanZengFanKuiGoodsIcons[goodsDataByStr.GoodsID], goodsDataByStr);
					}
					else
					{
						this.mJuanZengFanKuiGoodsIcons[goodsDataByStr.GoodsID] = this.RefreshGoodIcon(null, goodsDataByStr);
					}
					this.mJuanZengFanKuiGoodsIcons[goodsDataByStr.GoodsID].GoodImg.transform.localPosition = new Vector3(0f, 0f, -0.9f);
					this.mJuanZengFanKuiGoodsIcons[goodsDataByStr.GoodsID].transform.SetParent(this._JuanZengFanKuiGoodsRoot.transform, false);
					array3[i] = this.mJuanZengFanKuiGoodsIcons[goodsDataByStr.GoodsID].transform;
				}
			}
			this.UpdataTransPos(array3);
		}
	}

	private void UpdataTransPos(Transform[] trans)
	{
		if (trans != null)
		{
			int num = trans.Length;
			if (0 < trans.Length)
			{
				if (num != 1)
				{
					int num2 = 0;
					if (num % 2 != 0)
					{
						num2 = -45;
					}
					for (int i = 0; i < num; i++)
					{
						Transform transform = trans[i];
						if (null != transform)
						{
							if (i % 2 == 0)
							{
								transform.transform.localPosition = new Vector3((float)(45 + i * 45 + num2), 0f, 0f);
							}
							else
							{
								transform.transform.localPosition = new Vector3((float)(0 - i * 45 + num2), 0f, 0f);
							}
						}
					}
				}
			}
		}
	}

	private GGoodIcon RefreshGoodIcon(GGoodIcon icon, GoodsData gd)
	{
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(gd.GoodsID);
		string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
		if (null == icon)
		{
			icon = U3DUtils.NEW<GGoodIcon>();
		}
		icon.Width = 78.0;
		icon.Height = 78.0;
		icon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
		{
			goodsImageURLFromIconCode
		}), false, 0);
		icon.TipType = 1;
		icon.ItemCode = gd.GoodsID;
		icon.ItemObject = gd;
		icon.BoxTypes = 0;
		icon.TextSize = 16;
		icon.TextShadowColor = 4278190080U;
		icon.Tag = gd.ExcellenceInfo;
		icon.SecondText.Text = gd.GCount.ToString();
		icon.BackSpriteName0 = "bagGrid4_bak";
		Super.InitGoodsGIcon(icon, gd, true, IconTextTypes.Qianghua);
		icon.SecondText.transform.localPosition = new Vector3(32f, -24f, -2.5f);
		icon.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.MouseLeftButtonUp);
		if (!icon.gameObject.activeSelf)
		{
			icon.gameObject.SetActive(true);
		}
		return icon;
	}

	private void MouseLeftButtonUp(object sender, MouseEvent e)
	{
		GGoodIcon ggoodIcon = sender as GGoodIcon;
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

	public void RefreshPartPrence(int num)
	{
		if (this.mTeQuanTiaoJianVO != null)
		{
			int num2 = 0;
			float[] array = new float[4];
			for (int i = 0; i < this.mTeQuanTiaoJianVO.JiHuoIDs.size; i++)
			{
				TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(this.mTeQuanTiaoJianVO.JiHuoIDs[i]);
				if (num2 < teQuanJiHuoVOByID.CanShu)
				{
					num2 = teQuanJiHuoVOByID.CanShu;
				}
				if (num >= teQuanJiHuoVOByID.CanShu)
				{
					array[i] = 1f;
					if (i < this.mItemsStateList.size)
					{
						this.mItemsStateList[i].ShowTeXiao = true;
					}
				}
				else
				{
					array[i] = (float)num / (float)teQuanJiHuoVOByID.CanShu;
					if (i < this.mItemsStateList.size)
					{
						this.mItemsStateList[i].ShowTeXiao = false;
					}
				}
			}
			this.RefreshPrence(array);
			this.RefreshPart(this.mTeQuanTiaoJianVO, num);
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"TeQuanTiaoJianVO  == null "
			});
		}
	}

	public void RefreshPart(SpecPriorityActInfo inf)
	{
		if (inf == null)
		{
			return;
		}
		this.mSpecPriorityActInfo = inf;
		this._ActivityProgressLabel.text = string.Empty;
		this.mThisTimeID = 1;
		string text = Global.GetLang("活动时间") + "\n";
		if (this.mTeQuanTiaoJianVO == null)
		{
			this.mTeQuanTiaoJianVO = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanTiaoJianVOByID(inf.ActID);
		}
		if (this.mTeQuanTiaoJianVO != null)
		{
			text += Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Concat(new string[]
				{
					this.mTeQuanTiaoJianVO.KaiQiShiJian,
					"\n",
					Global.GetLang("至"),
					this.mTeQuanTiaoJianVO.JieShuShiJian,
					"\n"
				})
			});
		}
		text = text + Global.GetLang("当前已完成：") + inf.ShowNum;
		this._ActivityTimeInfLabel.text = text;
	}

	public bool PartOpen { get; set; }

	public int ID { get; set; }

	public DPSelectedItemEventHandler Hander;

	[SerializeField]
	private UILabel _ActivityTimeInfLabel;

	[SerializeField]
	private GameObject[] _ActivityStateItems;

	[SerializeField]
	private UILabel _ActivityProgressLabel;

	[SerializeField]
	private UISprite _ActivityProgressBgSp;

	[SerializeField]
	private UISprite[] _ProgressSps;

	[SerializeField]
	private float _Progress;

	[SerializeField]
	private float[] _Subsection;

	[SerializeField]
	private float[] _SubsectionEX;

	[SerializeField]
	private GButton _ReChargeBtn;

	[SerializeField]
	private GameObject _ActivityTipsRoot;

	[SerializeField]
	private GameObject _ActivityTipsBakBoxMask;

	[SerializeField]
	private UICollider _TipsUICollider;

	[SerializeField]
	private UILabel _TipsLabel;

	[SerializeField]
	private UISprite _CenterZiTi1;

	[SerializeField]
	private UISprite _CenterZiTi2;

	[SerializeField]
	private GameObject _ChongZhiRoot;

	[SerializeField]
	private GameObject _JuanZengRoot;

	[SerializeField]
	private GameObject _JuanZengGoodsRoot;

	[SerializeField]
	private GameObject _JuanZengFanKuiGoodsRoot;

	[SerializeField]
	private UILabel _JuanZengGoodsMiaoShuLabel;

	[SerializeField]
	private UILabel _JuanZengFanKuiMiaoShulabel1;

	[SerializeField]
	private UILabel _JuanZengFanKuiMiaoShulabel2;

	[SerializeField]
	private UISprite _JuanZengFanKuiMiaoShuSp;

	[SerializeField]
	private GButton _JuanZengUseZhuanShi;

	[SerializeField]
	private GButton _JuanZengUseGoods;

	[SerializeField]
	private UILabel _JuanZengZuanShiNumLabel;

	[SerializeField]
	private UILabel _JuanZengGoodsNumLabel;

	private int mThisTimeID = -1;

	private float mProgress;

	private TeQuanTiaoJianVO mTeQuanTiaoJianVO;

	private SpecPriorityActInfo mSpecPriorityActInfo;

	private BetterList<TeQuanActivityRepeatPart.ItemState> mItemsStateList = new BetterList<TeQuanActivityRepeatPart.ItemState>();

	private Dictionary<int, GGoodIcon> mJuanZengFanKuiGoodsIcons = new Dictionary<int, GGoodIcon>();

	private GChildWindow messageBoxWindow;

	public int DonateNumKF;

	public int DonateNum;

	private float time;

	private int Temp;

	private GGoodIcon mJuanZengGGoodIcon;

	public class ItemState
	{
		public ItemState(GameObject root, int id)
		{
			TeQuanActivityRepeatPart.ItemState <>f__this = this;
			this.mRoot = root;
			if (null != root)
			{
				UIEventListener.Get(root).onClick = delegate(GameObject s)
				{
					if (<>f__this.OnClick != null)
					{
						<>f__this.OnClick(root, new DPSelectedItemEventArgs
						{
							ID = <>f__this.mID
						});
					}
				};
				this._TeXiaoRoot = this.mRoot.transform.FindChild("teXiaoRoot").gameObject;
				this._TeXiaoXing = this._TeXiaoRoot.transform.FindChild("chongzhi_tequan_xing").gameObject;
				this._Icon = this.mRoot.transform.FindChild("Icon").GetComponent<ShowNetImage>();
				this._Value = this.mRoot.transform.FindChild("value").GetComponent<UILabel>();
				this.ShowXing = false;
				this.Value = string.Empty;
			}
			this.ID = id;
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
				TeQuanJiHuoVO teQuanJiHuoVOByID = IConfigbase<ConfigTeQuan>.Instance.GetTeQuanJiHuoVOByID(this.mID);
				if (teQuanJiHuoVOByID != null)
				{
					if (null != this._Icon)
					{
						this._Icon.URL = "NetImages/GameRes/Images/TeQuanActivity/" + teQuanJiHuoVOByID.HuoDongTuBiao + ".png";
					}
					this.Value = teQuanJiHuoVOByID.CanShu.ToString();
				}
			}
		}

		public bool ShowTeXiao
		{
			set
			{
				if (null != this._TeXiaoRoot)
				{
					this._TeXiaoRoot.SetActive(value);
				}
				if (value)
				{
					this._Icon.ToGrayBitmap = false;
				}
				else
				{
					this._Icon.ToGrayBitmap = true;
				}
			}
		}

		public bool ShowXing
		{
			set
			{
				if (null != this._TeXiaoXing)
				{
					this._TeXiaoXing.SetActive(value);
				}
			}
		}

		public string Value
		{
			set
			{
				if (null != this._Value)
				{
					this._Value.text = value;
				}
			}
		}

		public DPSelectedItemEventHandler OnClick;

		private GameObject mRoot;

		private GameObject _TeXiaoRoot;

		private GameObject _TeXiaoXing;

		private ShowNetImage _Icon;

		private UILabel _Value;

		private int mID;

		public int LeiIXng;
	}
}
