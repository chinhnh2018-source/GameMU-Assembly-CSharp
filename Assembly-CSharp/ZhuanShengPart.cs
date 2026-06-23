using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Decoration;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class ZhuanShengPart : UserControl
{
	public int Level
	{
		set
		{
			if (Global.Data != null && Global.Data.roleData != null)
			{
				this._Level_Value.text = ColorCode.EncodingText1((long)Global.Data.roleData.Level, (long)value, "fd010c");
			}
		}
	}

	public long Money
	{
		set
		{
			string text = value.ToString();
			if (Global.Data != null && Global.Data.roleData != null)
			{
				this._Money_Value.text = ColorCode.EncodingText1((long)(Global.Data.roleData.YinLiang + Global.Data.roleData.Money1), value, "fd010c");
			}
		}
	}

	public long MoJing
	{
		set
		{
			string text = value.ToString();
			if (Global.Data != null && Global.Data.roleData != null)
			{
				this._MoJing_Value.text = ColorCode.EncodingText1((long)Global.GetRoleOwnNumByMoneyType(13), value, "fd010c");
			}
		}
	}

	private void InitTextInPrefabs()
	{
		this._Submit.Text = Global.GetLang("转生");
		this._SuccessOK.Text = Global.GetLang("确定");
		this._ZhuanShengCount0.Pivot = 5;
		this._ZhuanShengCount0.X = 140.0;
		this._ZhuanShengCount1.Pivot = 3;
		this._ZhuanShengCount1.X = 290.0;
		this.zhuanShengPrompting._Lable.text = Global.GetLang("不再提示转生");
	}

	public override void Destroy()
	{
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
			this.roleResLoader = null;
		}
		base.Destroy();
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this._Close.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnClose);
		this._Submit.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit);
		this._Attribute_List.ItemsSource.AddNoUpdate(this._Attack = U3DUtils.NEW<CAttribute>("CAttribute"));
		this._Attribute_List.ItemsSource.AddNoUpdate(this._PDefence = U3DUtils.NEW<CAttribute>("CAttribute"));
		this._Attribute_List.ItemsSource.AddNoUpdate(this._MDefence = U3DUtils.NEW<CAttribute>("CAttribute"));
		this._Attribute_List.ItemsSource.AddNoUpdate(this._Hit = U3DUtils.NEW<CAttribute>("CAttribute"));
		this._Attribute_List.ItemsSource.AddNoUpdate(this._Dodge = U3DUtils.NEW<CAttribute>("CAttribute"));
		this._Attribute_List.ItemsSource.AddNoUpdate(this._MaxLife = U3DUtils.NEW<CAttribute>("CAttribute"));
		this._Attribute_List.ItemsSource.DelayUpdate();
		long num;
		if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 0)
		{
			this._Attack.Name = ExtPropIndexes.ExtPropIndexChineseNames[7];
			CAttribute attack = this._Attack;
			num = 0L;
			this._Attack.Value = num;
			attack.MaxValue = num;
		}
		else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 1)
		{
			this._Attack.Name = ExtPropIndexes.ExtPropIndexChineseNames[9];
			CAttribute attack2 = this._Attack;
			num = 0L;
			this._Attack.Value = num;
			attack2.MaxValue = num;
		}
		else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 2)
		{
			this._Attack.Name = ExtPropIndexes.ExtPropIndexChineseNames[7];
			CAttribute attack3 = this._Attack;
			num = 0L;
			this._Attack.Value = num;
			attack3.MaxValue = num;
		}
		else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
		{
			this._Attack.Name = ExtPropIndexes.ExtPropIndexChineseNames[45];
			CAttribute attack4 = this._Attack;
			num = 0L;
			this._Attack.Value = num;
			attack4.MaxValue = num;
		}
		else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 5)
		{
			this._Attack.Name = ExtPropIndexes.ExtPropIndexChineseNames[9];
			CAttribute attack5 = this._Attack;
			num = 0L;
			this._Attack.Value = num;
			attack5.MaxValue = num;
		}
		this._PDefence.Name = ExtPropIndexes.ExtPropIndexChineseNames[3];
		CAttribute pdefence = this._PDefence;
		num = 0L;
		this._PDefence.Value = num;
		pdefence.MaxValue = num;
		this._MDefence.Name = ExtPropIndexes.ExtPropIndexChineseNames[5];
		CAttribute mdefence = this._MDefence;
		num = 0L;
		this._MDefence.Value = num;
		mdefence.MaxValue = num;
		this._Hit.Name = ExtPropIndexes.ExtPropIndexChineseNames[18];
		this._Hit.Value = 0L;
		this._Dodge.Name = ExtPropIndexes.ExtPropIndexChineseNames[19];
		this._Dodge.Value = 0L;
		this._MaxLife.Name = ExtPropIndexes.ExtPropIndexChineseNames[13];
		this._MaxLife.Value = 0L;
		this.AddLevelPoint();
		bool visibility = this.Refresh(false);
		this._HelpAnim[0].Visibility = visibility;
		this._Submit.Text = Global.GetLang("转生");
		this._Title.Text = Global.GetLang("转生");
		this._SuccessOK.Text = Global.GetLang("确定");
		this._SuccessOK.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._WinAnimation.Play("Group_close");
			UIHelper.DelayInvoke(0.6f, delegate(object s1, EventArgs e1)
			{
				this._SuccessWin.SetActive(false);
				this.modal.gameObject.SetActive(true);
				this.Refresh(false);
				this._Close.isEnabled = true;
			});
		};
		this.modal.Clear();
		int fashionID = Global.Data.roleData.RoleCommonUseIntPamams[26];
		int fashionGoodsID = Global.GetFashionGoodsID(fashionID);
		List<GoodsData> list = new List<GoodsData>();
		Dictionary<int, GoodsData> usingGoodsDataList = Global.GetUsingGoodsDataList();
		if (usingGoodsDataList != null)
		{
			foreach (KeyValuePair<int, GoodsData> keyValuePair in usingGoodsDataList)
			{
				GoodsData goodsData = Global.CloneGoodsData(keyValuePair.Value, true);
				list.Add(goodsData);
			}
		}
		if (this.roleResLoader != null)
		{
			this.roleResLoader.Stop();
		}
		this.roleResLoader = UIHelper.LoadRoleRes(this.modal, Global.Data.roleData.SettingBitFlags, Global.Data.roleData.Occupation, Global.Data.roleData.SubOccupation, Global.Data.RoleName, list, Global.Data.equipPet, Global.Data.roleData.MyWingData, 1f, fashionGoodsID, null, false);
		UIHelper.SetModalPosZ(this.modal.transform);
	}

	private void AddLevelPoint()
	{
		XElement gameResXml = Global.GetGameResXml("Config/Roles/ZhuanShengAddPoint.xml");
		if (gameResXml == null)
		{
			return;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "ShuXing");
		if (Global.Data.roleData.ChangeLifeCount + 1 >= xelementList.Count)
		{
			this._Level_Value.text = Global.GetLang("您已达到最大转生级别!!!");
			return;
		}
		XElement xelement = xelementList[Global.Data.roleData.ChangeLifeCount];
		if (xelement != null)
		{
			int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "JiaDian");
			return;
		}
	}

	private XElement InitConfigs(int changeLifeIDNew)
	{
		if (changeLifeIDNew != this.InitZhuanShengLevel)
		{
			Dictionary<int, AttributeValueEx> dictionary = new Dictionary<int, AttributeValueEx>();
			Dictionary<int, AttributeValueEx> dictionary2 = new Dictionary<int, AttributeValueEx>();
			this.InitXmlItem = null;
			XElement gameResXml = Global.GetGameResXml(Global.GetZhuanShengXmlFilePath());
			if (gameResXml == null)
			{
				return null;
			}
			List<XElement> xelementList = Global.GetXElementList(gameResXml, "ZhuanSheng");
			foreach (XElement xelement in xelementList)
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "ChangeLifeID");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "AwardShuXing");
				if (xelementAttributeInt < changeLifeIDNew)
				{
					Global.ParseXmlAttributeValueExItemDict(dictionary, xelementAttributeStr, '|');
				}
				else if (xelementAttributeInt == changeLifeIDNew)
				{
					Global.ParseXmlAttributeValueExItemDict(dictionary2, xelementAttributeStr, '|');
					this.InitXmlItem = xelement;
					this.InitZhuanShengLevel = changeLifeIDNew;
					break;
				}
			}
			AttributeValueEx attributeValueEx;
			if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 0)
			{
				if (dictionary.TryGetValue(7, ref attributeValueEx))
				{
					this._Attack.Value = (long)attributeValueEx.AttributeValue0;
					this._Attack.MaxValue = (long)attributeValueEx.AttributeValue1;
				}
				if (dictionary2.TryGetValue(7, ref attributeValueEx))
				{
					this._Attack.DeltaValue = (long)attributeValueEx.AttributeValue1;
				}
			}
			else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 1)
			{
				if (dictionary.TryGetValue(9, ref attributeValueEx))
				{
					this._Attack.Value = (long)attributeValueEx.AttributeValue0;
					this._Attack.MaxValue = (long)attributeValueEx.AttributeValue1;
				}
				if (dictionary2.TryGetValue(9, ref attributeValueEx))
				{
					this._Attack.DeltaValue = (long)attributeValueEx.AttributeValue1;
				}
			}
			else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 2)
			{
				if (dictionary.TryGetValue(7, ref attributeValueEx))
				{
					this._Attack.Value = (long)attributeValueEx.AttributeValue0;
					this._Attack.MaxValue = (long)attributeValueEx.AttributeValue1;
				}
				if (dictionary2.TryGetValue(7, ref attributeValueEx))
				{
					this._Attack.DeltaValue = (long)attributeValueEx.AttributeValue1;
				}
			}
			else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 3)
			{
				if (dictionary.TryGetValue(45, ref attributeValueEx))
				{
					this._Attack.Value = (long)attributeValueEx.AttributeValue0;
					this._Attack.MaxValue = (long)attributeValueEx.AttributeValue1;
				}
				if (dictionary2.TryGetValue(45, ref attributeValueEx))
				{
					this._Attack.DeltaValue = (long)attributeValueEx.AttributeValue1;
				}
			}
			else if (Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation) == 5)
			{
				if (dictionary.TryGetValue(9, ref attributeValueEx))
				{
					this._Attack.Value = (long)attributeValueEx.AttributeValue0;
					this._Attack.MaxValue = (long)attributeValueEx.AttributeValue1;
				}
				if (dictionary2.TryGetValue(9, ref attributeValueEx))
				{
					this._Attack.DeltaValue = (long)attributeValueEx.AttributeValue1;
				}
			}
			if (dictionary.TryGetValue(3, ref attributeValueEx))
			{
				this._PDefence.Value = (long)attributeValueEx.AttributeValue0;
				this._PDefence.MaxValue = (long)attributeValueEx.AttributeValue1;
			}
			if (dictionary2.TryGetValue(3, ref attributeValueEx))
			{
				this._PDefence.DeltaValue = (long)attributeValueEx.AttributeValue1;
			}
			if (dictionary.TryGetValue(5, ref attributeValueEx))
			{
				this._MDefence.Value = (long)attributeValueEx.AttributeValue0;
				this._MDefence.MaxValue = (long)attributeValueEx.AttributeValue1;
			}
			if (dictionary2.TryGetValue(5, ref attributeValueEx))
			{
				this._MDefence.DeltaValue = (long)attributeValueEx.AttributeValue1;
			}
			if (dictionary.TryGetValue(18, ref attributeValueEx))
			{
				this._Hit.Value = (long)attributeValueEx.AttributeValue0;
			}
			if (dictionary2.TryGetValue(18, ref attributeValueEx))
			{
				this._Hit.DeltaValue = (long)attributeValueEx.AttributeValue0;
			}
			if (dictionary.TryGetValue(19, ref attributeValueEx))
			{
				this._Dodge.Value = (long)attributeValueEx.AttributeValue0;
			}
			if (dictionary2.TryGetValue(19, ref attributeValueEx))
			{
				this._Dodge.DeltaValue = (long)attributeValueEx.AttributeValue0;
			}
			if (dictionary.TryGetValue(13, ref attributeValueEx))
			{
				this._MaxLife.Value = (long)attributeValueEx.AttributeValue0;
			}
			if (dictionary2.TryGetValue(13, ref attributeValueEx))
			{
				this._MaxLife.DeltaValue = (long)attributeValueEx.AttributeValue0;
			}
		}
		return this.InitXmlItem;
	}

	public bool Refresh(bool showNotify = false)
	{
		bool result = true;
		if (Global.Data == null || Global.Data.roleData == null)
		{
			return false;
		}
		int changeLifeCount = Global.Data.roleData.ChangeLifeCount;
		int num = changeLifeCount + 1;
		int occupation = Global.Data.roleData.Occupation;
		this._ZhuanShengCount0.Text = string.Format(Global.GetLang("{0}"), changeLifeCount);
		this._ZhuanShengCount1.Text = string.Format(Global.GetLang("{0}"), num);
		XElement xelement = this.InitConfigs(num);
		if (xelement == null)
		{
			if (ConfigSystemParam.GetSystemParamIntByName("ChangeLifeMaxValue") < (long)num)
			{
				this.ShowErrorMsg(Global.GetLang("当前转生已达服务器上限"));
			}
			return false;
		}
		if (ConfigSystemParam.GetSystemParamIntByName("ChangeLifeMaxValue") < (long)num)
		{
			this.ShowErrorMsg(Global.GetLang("当前转生已达服务器上限"));
			return false;
		}
		this._ZhuanShengCount1.Text = string.Format(Global.GetLang("{0}"), num);
		this._ZhuanShengCount2.Text = string.Format(Global.GetLang("{0}"), num);
		int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "Level");
		int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "NeedJinBi");
		int xelementAttributeInt3 = Global.GetXElementAttributeInt(xelement, "NeedMoJing");
		string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "NeedGoods");
		int xelementAttributeInt4 = Global.GetXElementAttributeInt(xelement, "AwardShuXing");
		string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "AwardGoods");
		this.Level = xelementAttributeInt;
		this.Money = (long)xelementAttributeInt2;
		this.MoJing = (long)xelementAttributeInt3;
		if (Global.Data.roleData.Level < xelementAttributeInt)
		{
			result = false;
			if (showNotify)
			{
				this.ShowErrorMsg(string.Format(Global.GetLang("角色等级不够{0}, 暂时无法转生"), xelementAttributeInt));
				showNotify = false;
			}
		}
		if (xelementAttributeInt3 > 0 && Global.GetRoleOwnNumByMoneyType(13) < xelementAttributeInt3)
		{
			result = false;
			if (showNotify)
			{
				this.ShowErrorMsg(string.Format(Global.GetLang("魔晶不够{0}, 暂时无法转生"), xelementAttributeInt3));
				showNotify = false;
			}
		}
		if (xelementAttributeInt2 > 0 && Global.Data.roleData.YinLiang + Global.Data.roleData.Money1 < xelementAttributeInt2)
		{
			result = false;
			if (showNotify)
			{
				Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
				showNotify = false;
			}
		}
		if (xelementAttributeStr.Length > 3)
		{
			this._Need_ItemList.ItemsSource.Clear();
			List<GoodsData> list = UIHelper.ParseRewardGoodsList(xelementAttributeStr, 0, int.MaxValue);
			foreach (GoodsData goodsData in list)
			{
				int goodsID = goodsData.GoodsID;
				int gcount = goodsData.GCount;
				string goodsNameByID = Global.GetGoodsNameByID(goodsID, false);
				int totalGoodsCountByID = Global.GetTotalGoodsCountByID(goodsID);
				GGoodIcon ggoodIcon = UIHelper.AddGoodsIcon(this._Need_ItemList.ItemsSource, goodsData, null, false, "bagGrid4_bak");
				ggoodIcon.isAutoSize = true;
				ggoodIcon.BodyURL = new ImageURL(Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(goodsID)), false, 0);
				ggoodIcon.Text = string.Format("{0}/{1}", totalGoodsCountByID, gcount);
				ggoodIcon.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon.TextVerticalAlignment = global::Layout.Bottom;
				if (gcount > 0 && totalGoodsCountByID < gcount)
				{
					result = false;
					if (showNotify)
					{
						this.ShowErrorMsg(string.Format(Global.GetLang("{0}不足{1}个,暂时无法转职"), goodsNameByID, gcount));
						showNotify = false;
					}
				}
			}
			this._Need_ItemList.ItemsSource.DelayUpdate();
		}
		else
		{
			this._NeedItem.SetActive(false);
		}
		if (xelementAttributeStr2.Length > 3)
		{
			this._JiangLi_ItemList.ItemsSource.Clear();
			List<GoodsData> list2 = UIHelper.ParseRewardGoodsList(xelementAttributeStr2, 0, int.MaxValue);
			foreach (GoodsData goodsData2 in list2)
			{
				int goodsID2 = goodsData2.GoodsID;
				int gcount2 = goodsData2.GCount;
				GGoodIcon ggoodIcon2 = UIHelper.AddGoodsIcon(this._JiangLi_ItemList.ItemsSource, goodsData2, null, false, "bagGrid4_bak");
				ggoodIcon2.isAutoSize = true;
				ggoodIcon2.BodyURL = new ImageURL(Global.GetGoodsIconString(Global.GetGoodsIconCodeByID(goodsID2)), false, 0);
				ggoodIcon2.Text = gcount2.ToString();
				ggoodIcon2.TextHorizontalAlignment = global::Layout.Right;
				ggoodIcon2.TextVerticalAlignment = global::Layout.Bottom;
			}
			this._JiangLi_ItemList.ItemsSource.DelayUpdate();
		}
		return result;
	}

	private void OnClose(object sender, MouseEvent e)
	{
		if (this.DPSelectedItem != null)
		{
			if (this.zhuanShengPrompting.Check)
			{
				this.DPSelectedItem(this.zhuanShengPrompting, new DPSelectedItemEventArgs
				{
					ID = 1
				});
			}
			else
			{
				this.DPSelectedItem(this.zhuanShengPrompting, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnSubmit(object sender, MouseEvent e)
	{
		bool flag = this.Refresh(true);
		this._HelpAnim[0].Visibility = flag;
		if (flag)
		{
			GameInstance.Game.SpriteExecuteZhuanSheng();
		}
		SystemHelpMgr.OnAction(UIObjIDs.ZhuanShengSubmit, HelpStateEvents.Clicked, -1);
	}

	private void ShowSuccessDeco()
	{
		this._Close.isEnabled = false;
		GDecoration decoration = Global.GetDecoration(3000, GDecorationTypes.Loop, default(Point), false, null, -1, -1, true, false);
		U3DUtils.AddChild(this.modal.gameObject, decoration.The3DGameObject, true);
		decoration.The3DGameObject.transform.localPosition = Vector3.back * 100f;
		decoration.The3DGameObject.transform.localScale = Vector3.one;
		decoration.DecorationLoadCompleteNotify = delegate(object s, EventArgs e)
		{
			U3DUtils.ReplaceLayerInChildren(s as GameObject, this.modal.gameObject.layer, null);
		};
		decoration.DecorationDestroyNotify = delegate(object s, EventArgs e)
		{
			this._WinAnimation.Play("Group_open");
			UIHelper.DelayInvoke(0.6f, delegate(object s1, EventArgs e1)
			{
				this.modal.gameObject.SetActive(false);
				this._SuccessWin.SetActive(true);
			});
		};
	}

	public void NotifyResult(int ret, int roleID, int newChangeLiftID)
	{
		switch (ret + 4)
		{
		case 0:
			this.ShowErrorMsg(string.Format(Global.GetLang("所需物品数量不足"), new object[0]));
			break;
		case 1:
			this.ShowErrorMsg(string.Format(Global.GetLang("没有所需物品"), new object[0]));
			break;
		case 2:
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJinbi, null, string.Empty, string.Empty);
			break;
		case 3:
			this.ShowErrorMsg(string.Format(Global.GetLang("角色等级不够"), new object[0]));
			break;
		case 5:
			this.ShowSuccessDeco();
			if (null != PlayZone.GlobalPlayZone.GameHintQueueIcon)
			{
				PlayZone.GlobalPlayZone.GameHintQueueIcon.ProcessHintQueue(true);
				int num = RoleAttributePart.CalcPoint();
				if (num > 0 && Global.Data.roleData.AutoAssignPropertyPoint != 0)
				{
					RoleAttributePart.AutoAddPoint(num);
				}
			}
			break;
		}
	}

	private void ShowErrorMsg(string msg)
	{
		Super.HintMainText(msg, 10, 3);
	}

	public void ShowHelpAnim(int id, int state = 0)
	{
		if (state > 0)
		{
			SystemHelpPart.SetMask(this._Submit, default(Vector4));
		}
		else
		{
			SystemHelpPart.HideMask();
		}
	}

	protected void OnDisable()
	{
		this.ShowHelpAnim(0, 0);
		SystemHelpMgr.OnAction(UIObjIDs.ZhuanShengPart, HelpStateEvents.Inactived, -1);
	}

	public TextBlock _Title;

	public TextBlock _ZhuanShengCount0;

	public TextBlock _ZhuanShengCount1;

	public ListBox _Need_ItemList;

	public ListBox _JiangLi_ItemList;

	public ListBox _Attribute_List;

	public Modal3DShow modal;

	public GCheckBox zhuanShengPrompting;

	public GameObject _SuccessWin;

	public GButton _SuccessOK;

	public TextBlock _ZhuanShengCount2;

	public Animator _WinAnimation;

	private CAttribute _Attack;

	private CAttribute _PDefence;

	private CAttribute _MDefence;

	private CAttribute _Hit;

	private CAttribute _Dodge;

	private CAttribute _MaxLife;

	public GTextBlockOutLine _Level_Value;

	public GTextBlockOutLine _Money_Value;

	public TextBlock _MoJing_Value;

	public TextBlock _JiangLi_ShuXing;

	public TextBlock _JiangLi_ChengZhang;

	public GButton _Submit;

	public GButton _Close;

	public ShowNetImage _Background;

	public DPSelectedItemEventHandler DPSelectedItem;

	public GameObject _NeedItem;

	public CAnimation[] _HelpAnim;

	private RoleResLoader roleResLoader;

	private int InitZhuanShengLevel = -1;

	private XElement InitXmlItem;
}
