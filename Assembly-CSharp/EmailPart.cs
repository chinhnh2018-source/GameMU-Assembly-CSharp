using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class EmailPart : UserControl
{
	public ObservableCollection ItemCollection
	{
		get
		{
			return this._ItemCollection;
		}
		set
		{
			this._ItemCollection = value;
		}
	}

	public ObservableCollection ItemCollection2
	{
		get
		{
			return this._ItemCollection2;
		}
		set
		{
			this._ItemCollection2 = value;
		}
	}

	private void InitTextInPrefabs()
	{
		this.ConstTests[0].pivot = 3;
		this.ConstTests[1].pivot = 5;
		this.ConstTests[2].pivot = 5;
		this.ConstTests[0].transform.localPosition = new Vector3(-315f, 165f, -1f);
		this.ConstTests[1].transform.localPosition = new Vector3(84f, 182f, -1f);
		this.ConstTests[2].transform.localPosition = new Vector3(84f, 149f, -1f);
		this.m_LblZhuTi.transform.localPosition = new Vector3(114f, 158f, -1f);
		this.m_LblFaJianRen.transform.localPosition = new Vector3(114f, 191f, -1f);
		this.m_LblShiJian.pivot = 3;
		this.m_LblShiJian.transform.localPosition = new Vector3(444f, 191f, -1f);
		this.m_BtnGet.Text = Global.GetLang("提取");
		this.m_BtnDelete.Text = Global.GetLang("删除");
		this.m_BtnDeleteAll.Text = Global.GetLang("批量删除");
		this.m_BtnGetAll.Text = Global.GetLang("批量提取");
		this.m_ChkAllEmail.Text = Global.GetLang("全选");
		if (this.ConstTests != null && this.ConstTests.Length == 3)
		{
			this.ConstTests[0].text = Global.GetLang("信件保留15天，逾期将自动删除");
			this.ConstTests[1].text = Global.GetLang("发件人：");
			this.ConstTests[2].text = Global.GetLang("主    题：");
		}
		this.m_LblZhuTi.text = Global.GetLang("无");
		this.m_LblFaJianRen.text = Global.GetLang("无");
		this.m_LblJinBi.pivot = 3;
		this.m_LblZunShi.pivot = 3;
		this.m_LblJinBi.transform.localPosition = new Vector3(70f, -167f, -0.5f);
		this.m_LblZunShi.transform.localPosition = new Vector3(285f, -167f, -0.5f);
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.m_nSelectedMailItemIndex = -1;
		this.InitPartData();
		if (null != this.m_BtnClose)
		{
			this.m_BtnClose.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			};
		}
		if (null != this.m_BtnGet)
		{
			this.m_BtnGet.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (null != this.SelectedListItem && this.SelectedListItem.mailData != null)
				{
					GameInstance.Game.SpriteFetchMailGoods(Global.Data.RoleID, this.SelectedListItem.mailData.MailID);
				}
			};
		}
		if (null != this.m_BtnDelete)
		{
			this.m_BtnDelete.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				if (null != this.SelectedListItem && null != this.SelectedListItem && this.SelectedListItem.mailData != null)
				{
					this.DeleteSelectOne();
				}
			};
		}
		if (null != this.m_ChkAllEmail)
		{
			this.m_ChkAllEmail.CheckChanged = delegate(object sender, BaseEventArgs e)
			{
				this.SetCheckAllEmailState();
			};
		}
		if (null != this.m_BtnDeleteAll)
		{
			this.m_BtnDeleteAll.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
			{
				this.m_strDeleteMaillID = this.CheckDeleteAllMall();
			};
		}
		if (null != this.m_BtnGetAll)
		{
			this.m_BtnGetAll.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				this.GetAllMailGoods();
			};
		}
		this.listBox.SelectionChanged = new MouseLeftButtonUpEventHandler(this.listBox_SelectionChanged);
		this.goodList.SelectionChanged = new MouseLeftButtonUpEventHandler(this.goodList_SelectionChanged);
		this.ItemCollection = this.listBox.ItemsSource;
		this.ItemCollection2 = this.goodList.ItemsSource;
	}

	private void DeleteSelectOne()
	{
		if (this.SelectedListItem.mailData.Hasfetchattachment == 0)
		{
			this.m_strDeleteMaillID = Convert.ToString(this.SelectedListItem.EmailID);
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("邮件中附件未提取，是否删除？"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
		}
		else
		{
			GameInstance.Game.SpriteDeleteUserMail(Global.Data.RoleID, Convert.ToString(this.SelectedListItem.mailData.MailID));
		}
	}

	private void ShowText(string str)
	{
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, string.Format(Global.GetLang(str), new object[0]), 0, -1, -1, 0);
	}

	public void GetAllMailGoods()
	{
		if (this.ItemCollection != null)
		{
			string text = string.Empty;
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				GameObject at = this.ItemCollection.GetAt(i);
				EmailListItem component = at.gameObject.GetComponent<EmailListItem>();
				if (null != component && component.mailData.Hasfetchattachment == 0 && component.m_ChkSelectEmail.isChecked)
				{
					if (string.Empty == text)
					{
						text = Convert.ToString(component.mailData.MailID);
					}
					else
					{
						text = text + "," + Convert.ToString(component.mailData.MailID);
					}
				}
			}
			GameInstance.Game.GetAllMallGoods(Global.Data.RoleID, text);
		}
	}

	public string CheckDeleteAllMall()
	{
		bool flag = false;
		string text = string.Empty;
		if (this.ItemCollection != null)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				GameObject at = this.ItemCollection.GetAt(i);
				EmailListItem component = at.gameObject.GetComponent<EmailListItem>();
				if (null != component)
				{
					if (component.mailData.Hasfetchattachment == 0)
					{
						if (component.m_ChkSelectEmail.isChecked)
						{
							flag = true;
							if (string.Empty == text)
							{
								text = Convert.ToString(component.mailData.MailID);
							}
							else
							{
								text = text + "," + Convert.ToString(component.mailData.MailID);
							}
						}
					}
					else if (component.m_ChkSelectEmail.isChecked)
					{
						if (string.Empty == text)
						{
							text = Convert.ToString(component.mailData.MailID);
						}
						else
						{
							text = text + "," + Convert.ToString(component.mailData.MailID);
						}
					}
				}
			}
		}
		if (flag)
		{
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("勾选的邮件中有附件，是否删除？"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
			return text;
		}
		bool flag2 = false;
		if (this.ItemCollection != null)
		{
			for (int j = 0; j < this.ItemCollection.Count; j++)
			{
				GameObject at2 = this.ItemCollection.GetAt(j);
				EmailListItem component2 = at2.gameObject.GetComponent<EmailListItem>();
				if (null != component2 && component2.m_ChkSelectEmail.isChecked)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (!flag2)
		{
			string[] buttons2 = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("未勾选的邮件"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons2);
		}
		if (!flag && flag2 && string.Empty != text)
		{
			GameInstance.Game.SpriteDeleteUserMail(Global.Data.RoleID, text);
		}
		return string.Empty;
	}

	public void DPSelectItemHandler(object sender, DPSelectedItemEventArgs args)
	{
		if (args.ID == 0)
		{
			if (string.Empty != this.m_strDeleteMaillID)
			{
				GameInstance.Game.SpriteDeleteUserMail(Global.Data.RoleID, this.m_strDeleteMaillID);
			}
			this.m_strDeleteMaillID = string.Empty;
		}
		else if (args.ID == 1)
		{
			MUDebug.Log<string>(new string[]
			{
				"取消"
			});
		}
	}

	private void SetCheckAllEmailState()
	{
		bool isChecked = false;
		if (null != this.m_ChkAllEmail)
		{
			isChecked = this.m_ChkAllEmail.isChecked;
		}
		if (this.ItemCollection != null)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				GameObject at = this.ItemCollection.GetAt(i);
				if (null != at)
				{
					EmailListItem component = at.GetComponent<EmailListItem>();
					if (null != component)
					{
						component.m_ChkSelectEmail.isChecked = isChecked;
					}
				}
			}
		}
	}

	private void DeleteMailResetUI()
	{
		if (0 < this.ItemCollection2.Count)
		{
			this.ItemCollection2.Clear();
		}
		this.m_LblFaJianRen.text = Global.GetLang("无");
		this.m_LblShiJian.text = string.Empty;
		this.m_LblNeiRong.text = string.Empty;
		this.m_LblZhuTi.text = Global.GetLang("无");
		this.m_LblJinBi.text = "0";
		this.m_LblZunShi.text = "0";
		if (null != this.SelectedListItem)
		{
			this.m_nSelectedMailItemIndex = -1;
			this.ItemCollection.Remove(this.SelectedListItem.gameObject);
			this.ItemCollection.DelayUpdate();
			if (null != this.m_MailListPanel)
			{
				this.m_MailListPanel.gameObject.SetActive(false);
				this.m_MailListPanel.gameObject.SetActive(true);
			}
			if (null != this.m_ScrollBar)
			{
				this.m_ScrollBar.scrollValue = 0f;
			}
			this.SelectedListItem = null;
		}
		this.RefrshAllItem(this.listBox.transform);
	}

	private void DeleteMaillByID(string strMallID)
	{
		if (string.Empty == strMallID)
		{
			return;
		}
		if (this.ItemCollection != null)
		{
			for (int i = 0; i < this.ItemCollection.Count; i++)
			{
				GameObject at = this.ItemCollection.GetAt(i);
				if (null != at)
				{
					EmailListItem component = at.gameObject.GetComponent<EmailListItem>();
					if (component.EmailID == Convert.ToInt32(strMallID))
					{
						if (null != this.SelectedListItem && component.EmailID == this.SelectedListItem.EmailID)
						{
							this.DeleteMailResetUI();
						}
						if (null != component)
						{
							this.ItemCollection.RemoveNoUpdate(component.gameObject);
							this.ItemCollection.DelayUpdate();
						}
						break;
					}
				}
			}
		}
		this.RefrshAllItem(this.listBox.transform);
	}

	private void RefrshAllItem(Transform tr)
	{
		UIWidget[] componentsInChildren = tr.GetComponentsInChildren<UIWidget>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			componentsInChildren[j].enabled = true;
		}
	}

	private void SplitMaillID(string strID)
	{
		if (string.Empty != strID)
		{
			string[] array = strID.Split(new char[]
			{
				','
			});
			foreach (string strMallID in array)
			{
				this.DeleteMaillByID(strMallID);
			}
		}
	}

	private GGoodIcon GetGoodsItemIcon(MailGoodsData mailGoodsData)
	{
		GoodsData dummyGoodsDataMu = Global.GetDummyGoodsDataMu(mailGoodsData.GoodsID, mailGoodsData.Forge_level, mailGoodsData.AppendPropLev, mailGoodsData.ExcellenceInfo, mailGoodsData.Lucky, mailGoodsData.Binding, mailGoodsData.GCount, 0, null, "1900-01-01 12:00:00", "1900-01-01 12:00:00");
		GGoodIcon ggoodIcon;
		if (dummyGoodsDataMu != null)
		{
			GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(dummyGoodsDataMu.GoodsID);
			int categoriy = goodsXmlNodeByID.Categoriy;
			string goodsImageURLFromIconCode = Super.GetGoodsImageURLFromIconCode(Super.GetIconCode(goodsXmlNodeByID), string.Empty);
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.GoodsID = dummyGoodsDataMu.GoodsID;
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.ItemCategory = categoriy;
			ggoodIcon.ItemObject = dummyGoodsDataMu;
			ggoodIcon.isAutoSize = true;
			ggoodIcon.BodyURL = new ImageURL(StringUtil.substitute("NetImages/GameRes/{0}", new object[]
			{
				goodsImageURLFromIconCode
			}), false, 0);
			ggoodIcon.Tip = Global.GetGoodsNameByID(dummyGoodsDataMu.GoodsID, false);
			Super.InitGoodsGIcon(ggoodIcon, dummyGoodsDataMu, true, IconTextTypes.Qianghua);
		}
		else
		{
			ggoodIcon = U3DUtils.NEW<GGoodIcon>();
			ggoodIcon.Width = 64.0;
			ggoodIcon.Height = 64.0;
			ggoodIcon.BackSpriteName0 = "bagGrid_bak";
		}
		ggoodIcon.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.ShowGoodsTip(s);
		};
		UIButtonOffset componentInChildren = ggoodIcon.GetComponentInChildren<UIButtonOffset>();
		if (null != componentInChildren)
		{
			componentInChildren.enabled = false;
		}
		return ggoodIcon;
	}

	private void ShowGoodsTip(object icon)
	{
		GGoodIcon ggoodIcon = icon as GGoodIcon;
		GoodsData goodData = ggoodIcon.ItemObject as GoodsData;
		GTipServiceEx.SelfBagOnly = false;
		GTipServiceEx.ShowTip(ggoodIcon, TipTypes.GoodsText, GoodsOwnerTypes.SysGifts, goodData);
	}

	private void ShowWuPinList(List<MailGoodsData> GoodsList = null)
	{
		if (GoodsList == null)
		{
			return;
		}
		if (0 >= GoodsList.Count)
		{
			return;
		}
		this.ItemCollection2.Clear();
		for (int i = 0; i < 6; i++)
		{
			GGoodIcon ggoodIcon = null;
			if (GoodsList.Count > i)
			{
				ggoodIcon = this.GetGoodsItemIcon(GoodsList[i]);
			}
			if (null != ggoodIcon)
			{
				this.ItemCollection2.AddNoUpdate(ggoodIcon);
			}
		}
		this.ItemCollection2.DelayUpdate();
	}

	private void ShowMailList(List<MailData> MailDataList = null)
	{
		if (0 >= MailDataList.Count)
		{
			return;
		}
		this.ItemCollection.Clear();
		for (int i = 0; i < MailDataList.Count; i++)
		{
			EmailListItem emailListItem = U3DUtils.NEW<EmailListItem>();
			emailListItem.mailData = MailDataList[i];
			emailListItem.EmailID = MailDataList[i].MailID;
			emailListItem.m_EmailBiaoTi.text = MailDataList[i].Subject;
			emailListItem.m_HotSpriteBak.gameObject.SetActive(false);
			emailListItem.m_LblEmailRecvTime.text = MailDataList[i].SendTime;
			if (MailDataList[i].IsRead == 0)
			{
				emailListItem.m_SpriteStateIsRead.gameObject.SetActive(false);
				emailListItem.m_SpriteStateNoRead.gameObject.SetActive(true);
			}
			if (MailDataList[i].IsRead == 1)
			{
				emailListItem.m_SpriteStateIsRead.gameObject.SetActive(true);
				emailListItem.m_SpriteStateNoRead.gameObject.SetActive(false);
			}
			this.ItemCollection.AddNoUpdate(emailListItem);
		}
		this.ItemCollection.DelayUpdate();
	}

	private void goodList_SelectionChanged(object sender, object e)
	{
	}

	private void listBox_SelectionChanged(object sender, object e)
	{
		EmailListItem emailListItem = U3DUtils.AS<EmailListItem>(this.listBox.SelectedItem);
		if (this.SelectedListItem == emailListItem)
		{
			return;
		}
		if (null != this.goodList)
		{
			this.ItemCollection2.Clear();
			this.ItemCollection2.DelayUpdate();
		}
		if (null != emailListItem)
		{
			emailListItem.m_HotSpriteBak.gameObject.SetActive(true);
			if (null != this.SelectedListItem)
			{
				this.SelectedListItem.m_HotSpriteBak.gameObject.SetActive(false);
				this.SelectedListItem.m_SpriteStateIsRead.gameObject.SetActive(true);
				this.SelectedListItem.m_SpriteStateNoRead.gameObject.SetActive(false);
			}
			this.SelectedListItem = emailListItem;
			this.m_nSelectedMailItemIndex = this.listBox.SelectedIndex;
			this.GetEmailDetails();
		}
	}

	private void GetEmailDetails()
	{
		if (null != this.SelectedListItem)
		{
			if (0 < this.SelectedListItem.EmailID)
			{
				GameInstance.Game.SpriteGetUserMailData(Global.Data.roleData.RoleID, this.SelectedListItem.EmailID);
			}
			else
			{
				this.ShowEmailDetails(this.SelectedListItem.mailData);
			}
		}
	}

	private void ShowEmailDetails(MailData MailData)
	{
		if (MailData != null)
		{
			this.m_LblFaJianRen.text = MailData.SenderRName;
			this.m_LblShiJian.text = MailData.SendTime;
			this.m_LblJinBi.text = Convert.ToString(MailData.Yinliang);
			this.m_LblNeiRong.text = MailData.Content;
			UIDraggablePanel component = this.m_LblNeiRong.transform.parent.GetComponent<UIDraggablePanel>();
			if (component != null)
			{
				component.ResetPosition();
				component.RestrictWithinBounds(true);
			}
			this.m_LblZunShi.text = Convert.ToString(MailData.YuanBao);
			this.m_LblZhuTi.text = MailData.Subject;
			this.SelectedListItem.From = MailData.SenderRName;
			this.ShowWuPinList(MailData.GoodsList);
		}
	}

	public void InitPartData()
	{
		GameInstance.Game.SpriteGetUserMailList(Global.Data.roleData.RoleID);
	}

	public void OnGetMailSendCodeCompleted(int roleID, string code)
	{
	}

	public void ONGetUserMailListCompleted(List<MailData> ls)
	{
		if (ls == null)
		{
			return;
		}
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (ls == null || ls.Count <= 0)
		{
			return;
		}
		List<MailData> list = new List<MailData>();
		for (int i = 0; i < ls.Count; i++)
		{
			MailData mailData = ls[i];
			list.Add(new MailData
			{
				MailID = mailData.MailID,
				Subject = mailData.Subject,
				SendTime = mailData.SendTime,
				MailType = mailData.MailType,
				IsRead = mailData.IsRead,
				GoodsList = mailData.GoodsList,
				Hasfetchattachment = mailData.Hasfetchattachment
			});
		}
		this.RefreshData(list);
	}

	public void OnGetMailDetailCompleted(MailData obj)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (obj == null)
		{
			return;
		}
		MailData mailData = new MailData();
		mailData.MailID = obj.MailID;
		mailData.Subject = obj.Subject;
		mailData.SenderRName = obj.SenderRName;
		mailData.Content = obj.Content;
		mailData.Tongqian = obj.Tongqian;
		mailData.Yinliang = obj.Yinliang;
		mailData.YuanBao = obj.YuanBao;
		mailData.Hasfetchattachment = obj.Hasfetchattachment;
		if (obj.Hasfetchattachment == 0)
		{
			mailData.GoodsList = obj.GoodsList;
		}
		else if (obj.Hasfetchattachment == 1)
		{
			mailData.GoodsList = null;
			mailData.Yinliang = 0;
			mailData.YuanBao = 0;
		}
		this.NotifyEmailDetailData(mailData);
	}

	public void OnGetAllMailAttachmentGoodsAndMoneyCompleted(int result, int roleID, int mailID)
	{
		if (result > 0)
		{
			if (this.ItemCollection != null)
			{
				for (int i = 0; i < this.ItemCollection.Count; i++)
				{
					GameObject at = this.ItemCollection.GetAt(i);
					EmailListItem component = at.gameObject.GetComponent<EmailListItem>();
					if (null != component)
					{
						if (mailID == component.mailData.MailID)
						{
							component.mailData.Hasfetchattachment = 1;
						}
						if (this.SelectedListItem == component && null != this.goodList)
						{
							this.ItemCollection2.Clear();
							this.ItemCollection2.DelayUpdate();
						}
					}
				}
			}
			this.ShowRetMsg(result);
		}
		else
		{
			this.ShowRetMsg(result);
		}
	}

	public void OnGetMailAttachmentGoodsAndMoneyCompleted(int result, int roleID, int mailID)
	{
		if (this.LoadingWin != null)
		{
		}
		if (result > 0)
		{
			this.SelectedListItem.mailData.Hasfetchattachment = 1;
			this.m_strDeleteMaillID = Convert.ToString(this.SelectedListItem.mailData.MailID);
			string[] buttons = new string[]
			{
				Global.GetLang("确定"),
				Global.GetLang("取消")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), Global.GetLang("附件已提取，是否删除？"), new DPSelectedItemEventHandler(this.DPSelectItemHandler), buttons);
			if (null != this.goodList)
			{
				this.ItemCollection2.Clear();
				this.ItemCollection2.DelayUpdate();
			}
			this.ShowRetMsg(result);
		}
		else
		{
			this.ShowRetMsg(result);
		}
	}

	private void ShowRetMsg(int result)
	{
		if (result > 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取成功"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -99)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("系统暂时停止邮件附件提取功能，你的附件物品开启提取功能后才可以提取"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -110)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取失败，邮件不存在"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -115)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取失败，这个邮件不是你的"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -120)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取失败，邮件没有任何附件"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -121)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取失败，邮件附件已经被提取过"), new object[0]), 0, -1, -1, 0);
		}
		else if (result == -125)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取失败，背包没有足够的空位置"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件附件提取失败,错误码{0}"), new object[]
			{
				result
			}), 0, -1, -1, 0);
		}
	}

	public void OnRecvLastMail(int roleID, int mailID)
	{
		GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("收到新邮件"), new object[0]), 0, -1, -1, 0);
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = 1,
				IDType = 0
			});
		}
	}

	public void OnDeleteMailCompleted(int result, int roleID, string mailIDs)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (result > 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("删除邮件成功"), new object[0]), 0, -1, -1, 0);
			this.SplitMaillID(mailIDs);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("删除邮件失败"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void OnMailSendCompleted(int roleID, int mailID)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (mailID >= 0)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送成功"), new object[0]), 0, -1, -1, 0);
			GameInstance.Game.SpriteGetMailSendCode(Global.Data.roleData.RoleID);
		}
		else if (mailID == -109)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，接收角色不存在"), new object[0]), 0, -1, -1, 0);
		}
		else if (mailID == -90)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，等级大于{0}才能发送邮件"), new object[]
			{
				ConfigSystemParam.GetSystemParamIntByName("MinLevelForMailSend")
			}), 0, -1, -1, 0);
		}
		else if (mailID == -100)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，验证码不对"), new object[0]), 0, -1, -1, 0);
		}
		else if (mailID == -101)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，标题或内容过长"), new object[0]), 0, -1, -1, 0);
		}
		else if (mailID == -107)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，主题和内容不能为空"), new object[0]), 0, -1, -1, 0);
		}
		else if (mailID == -108)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，不能给自己发送邮件"), new object[0]), 0, -1, -1, 0);
		}
		else if (mailID == -120)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
		}
		else if (mailID == -130)
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
		else if (mailID == -135)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，绑定物品不让发送"), new object[0]), 0, -1, -1, 0);
		}
		else if (mailID == -140)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败，背包里面没有要发送的物品"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("邮件发送失败,错误码{0}"), new object[]
			{
				mailID
			}), 0, -1, -1, 0);
		}
	}

	public void RefreshData(List<MailData> MailDataList)
	{
		if (this.LoadingWin != null)
		{
			this.Container.Children.Remove(this.LoadingWin, true);
			this.LoadingWin.Destroy();
			this.LoadingWin = null;
		}
		if (MailDataList == null || MailDataList.Count <= 0)
		{
			return;
		}
		this.ShowMailList(MailDataList);
	}

	public void NotifyEmailDetailData(MailData MailData)
	{
		if (MailData != null)
		{
			this.ShowEmailDetails(MailData);
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	private List<GChildWindow> ChildWindowList = new List<GChildWindow>();

	public Canvas emailNeiRongBak;

	private LoadingWindow LoadingWin;

	public ListBox listBox = new ListBox();

	public ListBox goodList = new ListBox();

	private GTextBlockOutLine Page = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	public GScrollView ScrollViewer1 = new GScrollView(0, 0, 0);

	public Canvas Wrapper = new Canvas();

	public GButton m_BtnClose;

	public GButton m_BtnGet;

	public GButton m_BtnDelete;

	public GCheckBox m_ChkAllEmail;

	public GButton m_BtnDeleteAll;

	public GButton m_BtnGetAll;

	public UILabel m_LblFaJianRen;

	public UILabel m_LblShiJian;

	public UILabel m_LblNeiRong;

	public UILabel m_LblZhuTi;

	public UILabel m_LblJinBi;

	public UILabel m_LblZunShi;

	public UIScrollBar m_ScrollBar;

	public UIPanel m_MailListPanel;

	public int m_nSelectedMailItemIndex;

	public string m_strDeleteMaillID = string.Empty;

	public UILabel[] ConstTests;

	private Dictionary<int, List<MailData>> TabItemsDict = new Dictionary<int, List<MailData>>();

	private Dictionary<int, GoodsData> EmailFuJianGoodsDict = new Dictionary<int, GoodsData>();

	private GTextBlockOutLine TotalPage = new GTextBlockOutLine(string.Empty, -1, -1, -1, -1, -1);

	private EmailListItem SelectedListItem;

	private ObservableCollection _ItemCollection;

	private ObservableCollection _ItemCollection2;
}
