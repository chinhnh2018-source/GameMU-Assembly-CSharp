using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;
using UnityEngine;

public class RoleAttributeDetailPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Remain.textColor = 15917757U;
		this._Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			Object.Destroy(base.gameObject);
		};
		this._Close_JiaDian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_JiaDian.SetActive(false);
		};
		this._Close_TuiJian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_TuiJian.SetActive(false);
		};
		this._Close_XiDian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this._ModalBak.gameObject.SetActive(false);
			this._Part_XiDian.SetActive(false);
		};
		this._TuiJianJiaDian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowRecommendPointPart();
		};
		this._XiDian.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			this.ShowClearPointPart();
		};
		this._Submit_JIaDian.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit_JiaDian);
		this._Submit_TuiJian.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit_TuiJian);
		this._Submit_XiDian.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.OnSubmit_XiDian);
		this._Bak.URL = Global.GetGameResImageString("roleJiadian_bak.png");
		this._Bak_JiaDian.URL = Global.GetGameResImageString("roleToJiadian_bak.png");
		this._Bak_XiDian.URL = Global.GetGameResImageString("roleToJiadian_bak.png");
		this._Bak_TuiJian.URL = Global.GetGameResImageString("roleToJiadian_bak.png");
		this._Input_JiaDian.TextChanged = new EventHandler(this.Input_JiaDian_EventHandler);
		this._Input_JiaDian.text = string.Empty;
		this._Max_JiaDian.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this._Input_JiaDian.text = RoleAttributeDetailPart.nRemainPoint.ToString();
		};
		if (null != RoleAttributeDetailPart.Instance)
		{
			Object.Destroy(RoleAttributeDetailPart.Instance.gameObject);
		}
		RoleAttributeDetailPart.Instance = this;
		this.InitPartData();
	}

	protected override void OnDestroy()
	{
		RoleAttributeDetailPart.Instance = null;
		base.OnDestroy();
	}

	private void OnPlayEnd(UITweener tw)
	{
		UIButtonTween component = tw.eventReceiver.GetComponent<UIButtonTween>();
		if (null != component)
		{
			component.callWhenFinished = null;
			component.eventReceiver = null;
		}
	}

	private void SetTab(int index = -1)
	{
		if (index >= 0)
		{
			this._SelectedTab = index;
		}
		for (int i = 0; i < this.Items.Length; i++)
		{
			this.Items[i].ToggleState = (this._SelectedTab == i);
		}
	}

	public void InitPartData()
	{
		RoleAttributeDetailPart.Occupation = Global.CalcOriginalOccupationID(Global.Data.roleData.Occupation);
		if (this.Items == null)
		{
			this.Items = new RoleAttributeDetailPartItem[4];
		}
		RoleAttributeDetailPart.CalcPoint();
		this._Remain.text = string.Format(Global.GetLang("剩余点数: {0}"), RoleAttributeDetailPart.nRemainPoint);
		for (int i = 0; i < 4; i++)
		{
			int tag = i;
			if (this.Items[i] == null)
			{
				this.Items[i] = U3DUtils.NEW<RoleAttributeDetailPartItem>();
				U3DUtils.AddChild(this._Table.gameObject, this.Items[i].gameObject, false);
				this.Items[i]._Attribute.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
				{
					this.SetTab(tag);
				};
				this.Items[i]._Submit.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
				{
					this.ShowAddPointPart(tag);
				};
			}
			this.Items[i]._Attribute.Text = string.Format("{0}({1})", Global.GetUnitPropIndexeName(i), Global.GetCurrentRoleProp(1, i));
			this.Items[i].LabelText.text = this.EncodingExtPropFrom(i);
		}
		this.SetTab(-1);
		if (HintQueueIcon.HintAddPoint && RoleAttributeDetailPart.nRemainPoint >= 500)
		{
			this._TuiJianJiaDian.SetSpriteAnimationVisible(true);
		}
		else
		{
			this._TuiJianJiaDian.SetSpriteAnimationVisible(false);
			HintQueueIcon.HintAddPoint = false;
		}
	}

	private string EncodingExtPropFrom(int unitPropIndex)
	{
		string text = string.Empty;
		double currentRoleProp = Global.GetCurrentRoleProp(1, unitPropIndex);
		switch (unitPropIndex)
		{
		case 0:
			switch (RoleAttributeDetailPart.Occupation)
			{
			case 0:
			case 2:
				text = string.Format("{0} : {1}~{2}\r\n\r\n{3} : {4}~{5}\r\n", new object[]
				{
					Global.GetLang("物理攻击"),
					(int)Global.GetCurrentRoleProp(2, 7),
					(int)Global.GetCurrentRoleProp(2, 8),
					Global.GetLang("物理防御"),
					(int)Global.GetCurrentRoleProp(2, 3),
					(int)Global.GetCurrentRoleProp(2, 4)
				});
				break;
			case 1:
				text = string.Format("{0} : {1}~{2}\r\n\r\n{3} : {4}\r\n", new object[]
				{
					Global.GetLang("物理防御"),
					(int)Global.GetCurrentRoleProp(2, 3),
					(int)Global.GetCurrentRoleProp(2, 4),
					Global.GetLang("魔法增幅"),
					(int)Global.GetCurrentRoleProp(2, 33)
				});
				break;
			}
			break;
		case 1:
			switch (RoleAttributeDetailPart.Occupation)
			{
			case 0:
			case 2:
				text = string.Format("{0} : {1}~{2}\r\n\r\n{3} : {4}\r\n", new object[]
				{
					Global.GetLang("魔法防御"),
					(int)Global.GetCurrentRoleProp(2, 5),
					(int)Global.GetCurrentRoleProp(2, 6),
					Global.GetLang("物理增幅"),
					(int)Global.GetCurrentRoleProp(2, 31)
				});
				break;
			case 1:
				text = string.Format("{0} : {1}~{2}\r\n\r\n{3} : {4}~{5}\r\n", new object[]
				{
					Global.GetLang("魔法攻击"),
					(int)Global.GetCurrentRoleProp(2, 9),
					(int)Global.GetCurrentRoleProp(2, 10),
					Global.GetLang("魔法防御"),
					(int)Global.GetCurrentRoleProp(2, 5),
					(int)Global.GetCurrentRoleProp(2, 6)
				});
				break;
			}
			break;
		case 2:
			text = string.Format("{0} : {1}\r\n{2} : {3}\r\n{4} : {5}\r\n", new object[]
			{
				Global.GetLang("攻击速度"),
				(int)Global.GetCurrentRoleProp(2, 1),
				Global.GetLang("命        中"),
				(int)Global.GetCurrentRoleProp(2, 18),
				Global.GetLang("闪        避"),
				(int)Global.GetCurrentRoleProp(2, 19)
			});
			break;
		case 3:
			text = string.Format("{0} : {1}\r\n\r\n{2} : {3}\r\n", new object[]
			{
				Global.GetLang("生命上限"),
				(int)Global.GetCurrentRoleProp(2, 13),
				Global.GetLang("魔法上限"),
				(int)Global.GetCurrentRoleProp(2, 15)
			});
			break;
		}
		return "{F2E2BD}" + text + "{-}";
	}

	private void ShowAddPointPart(int type)
	{
		RoleAttributeDetailPart.CalcPoint();
		this._Title.Text = Global.GetLang("加点");
		this._ModalBak.gameObject.SetActive(true);
		this._SelectedTab = type;
		this._Part_JiaDian.SetActive(true);
		this._Avalid_JiaDian.Text = RoleAttributeDetailPart.nRemainPoint.ToString();
		this._Input_JiaDian.text = string.Empty;
		this._Name_JiaDian.text = Global.GetUnitPropIndexeName(type);
	}

	public static int CalcPoint()
	{
		RoleAttributeDetailPart.nTotalPoint = (int)Global.GetCurrentRoleProp(0, 0);
		RoleAttributeDetailPart.nStrengthPoint = (int)Global.GetCurrentRoleProp(1, 0);
		RoleAttributeDetailPart.nIntelligencePoint = (int)Global.GetCurrentRoleProp(1, 1);
		RoleAttributeDetailPart.nDexterityPoint = (int)Global.GetCurrentRoleProp(1, 2);
		RoleAttributeDetailPart.nConstitutionPoint = (int)Global.GetCurrentRoleProp(1, 3);
		RoleAttributeDetailPart.nUsedPoint = RoleAttributeDetailPart.nStrengthPoint + RoleAttributeDetailPart.nIntelligencePoint + RoleAttributeDetailPart.nDexterityPoint + RoleAttributeDetailPart.nConstitutionPoint;
		RoleAttributeDetailPart.nRemainPoint = RoleAttributeDetailPart.nTotalPoint - RoleAttributeDetailPart.nUsedPoint;
		return RoleAttributeDetailPart.nRemainPoint;
	}

	public static void AutoAddPoint(int avalidPoint)
	{
		RoleAttributeDetailPart.EncodingRecommendPointString(avalidPoint);
		GameInstance.Game.SpriteRecommendPoint(RoleAttributeDetailPart.addStrengthPoint, RoleAttributeDetailPart.addIntelligencePoint, RoleAttributeDetailPart.addDexterityPoint, RoleAttributeDetailPart.addConstitutionPoint);
	}

	private static string EncodingRecommendPointString(int avalidPoint)
	{
		string empty = string.Empty;
		RoleAttributeDetailPart.addStrengthPoint = 0;
		RoleAttributeDetailPart.addIntelligencePoint = 0;
		RoleAttributeDetailPart.addDexterityPoint = 0;
		RoleAttributeDetailPart.addConstitutionPoint = 0;
		float num = 0f;
		float num2 = 0f;
		float num3 = 2f;
		float num4 = 1f;
		RoleAttributeDetailPart.Occupation = Global.CalcChangeOccupationID(Global.Data.roleData.Occupation);
		switch (RoleAttributeDetailPart.Occupation)
		{
		case 0:
			num = 2f;
			break;
		case 1:
			num2 = 2f;
			break;
		case 2:
			num = 2f;
			break;
		}
		int num5;
		for (;;)
		{
			num5 = RoleAttributeDetailPart.nRemainPoint;
			if (num > 0f)
			{
				num5 += RoleAttributeDetailPart.nStrengthPoint;
			}
			if (num2 > 0f)
			{
				num5 += RoleAttributeDetailPart.nIntelligencePoint;
			}
			if (num3 > 0f)
			{
				num5 += RoleAttributeDetailPart.nDexterityPoint;
			}
			if (num4 > 0f)
			{
				num5 += RoleAttributeDetailPart.nConstitutionPoint;
			}
			if (num4 > 0f && (float)num5 * num4 / (num4 + num + num2 + num3) <= (float)RoleAttributeDetailPart.nConstitutionPoint)
			{
				num4 = 0f;
			}
			else if (num > 0f && (float)num5 * num / (num4 + num + num2 + num3) <= (float)RoleAttributeDetailPart.nStrengthPoint)
			{
				num = 0f;
			}
			else if (num2 > 0f && (float)num5 * num2 / (num4 + num + num2 + num3) <= (float)RoleAttributeDetailPart.nIntelligencePoint)
			{
				num2 = 0f;
			}
			else
			{
				if (num3 <= 0f || (float)num5 * num3 / (num4 + num + num2 + num3) > (float)RoleAttributeDetailPart.nDexterityPoint)
				{
					break;
				}
				num3 = 0f;
			}
		}
		if (num > 0f)
		{
			RoleAttributeDetailPart.addStrengthPoint = (int)((float)num5 * num / (num4 + num + num2 + num3) - (float)RoleAttributeDetailPart.nStrengthPoint);
		}
		if (num2 > 0f)
		{
			RoleAttributeDetailPart.addIntelligencePoint = (int)((float)num5 * num2 / (num4 + num + num2 + num3) - (float)RoleAttributeDetailPart.nIntelligencePoint);
		}
		if (num3 > 0f)
		{
			RoleAttributeDetailPart.addDexterityPoint = (int)((float)num5 * num3 / (num4 + num + num2 + num3) - (float)RoleAttributeDetailPart.nDexterityPoint);
		}
		if (num4 > 0f)
		{
			RoleAttributeDetailPart.addConstitutionPoint = (int)((float)num5 * num4 / (num4 + num + num2 + num3) - (float)RoleAttributeDetailPart.nConstitutionPoint);
		}
		int num6 = RoleAttributeDetailPart.nRemainPoint - (RoleAttributeDetailPart.addStrengthPoint + RoleAttributeDetailPart.addIntelligencePoint + RoleAttributeDetailPart.addDexterityPoint + RoleAttributeDetailPart.addConstitutionPoint);
		if (num4 > 0f)
		{
			RoleAttributeDetailPart.addConstitutionPoint += num6;
		}
		else if (num > 0f)
		{
			RoleAttributeDetailPart.addStrengthPoint += num6;
		}
		else if (num2 > 0f)
		{
			RoleAttributeDetailPart.addIntelligencePoint += num6;
		}
		else if (num3 > 0f)
		{
			RoleAttributeDetailPart.addDexterityPoint += num6;
		}
		return string.Format("{0}: {1}({{ffffff}}+{2}{{-}})\r\n{3}: {4}({{ffffff}}+{5}{{-}})\r\n{6}: {7}({{ffffff}}+{8}{{-}})\r\n{9}: {10}({{ffffff}}+{11}{{-}})\r\n", new object[]
		{
			Global.GetLang("力量"),
			RoleAttributeDetailPart.nStrengthPoint,
			RoleAttributeDetailPart.addStrengthPoint,
			Global.GetLang("智力"),
			RoleAttributeDetailPart.nIntelligencePoint,
			RoleAttributeDetailPart.addIntelligencePoint,
			Global.GetLang("敏捷"),
			RoleAttributeDetailPart.nDexterityPoint,
			RoleAttributeDetailPart.addDexterityPoint,
			Global.GetLang("体力"),
			RoleAttributeDetailPart.nConstitutionPoint,
			RoleAttributeDetailPart.addConstitutionPoint
		});
	}

	private void ShowRecommendPointPart()
	{
		this._Title.Text = Global.GetLang("推荐加点");
		RoleAttributeDetailPart.CalcPoint();
		this._ModalBak.gameObject.SetActive(true);
		this._Part_TuiJian.SetActive(true);
		this._Scheme_TuiJian.text = RoleAttributeDetailPart.EncodingRecommendPointString(0);
	}

	private void Input_JiaDian_EventHandler(object sender, EventArgs e)
	{
		string text = this._Input_JiaDian.text;
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text.get_Chars(i);
			if (char.IsDigit(c))
			{
				text2 += c;
			}
		}
		this._Input_JiaDian.text = text2;
	}

	public void OnCanClearPointResult(int nValue)
	{
		int num = nValue;
		this._Title.Text = Global.GetLang("洗点");
		this._ModalBak.gameObject.SetActive(true);
		long systemParamIntByName = ConfigSystemParam.GetSystemParamIntByName("MianFeiXiDian");
		if ((long)Global.Data.roleData.ChangeLifeCount > systemParamIntByName)
		{
			this.nFeeXidian = num * 10;
			this._Fee_XiJian.text = this.nFeeXidian.ToString();
		}
		else
		{
			this.nFeeXidian = 0;
			this._Fee_XiJian.text = string.Format("{0}", this.nFeeXidian);
			this._Fee_Desc.text = string.Format(Global.GetLang("{0}转前免费"), systemParamIntByName);
		}
		this._Avalid_XiDian.text = num.ToString();
		this._Part_XiDian.SetActive(true);
	}

	private void ShowClearPointPart()
	{
		GameInstance.Game.SpriteQueryCleanPropAddPointInfoCmd();
	}

	public void NotifyAttribute2()
	{
		this.InitPartData();
	}

	public void OnSubmit_JiaDian(object sender, MouseEvent e)
	{
		int num = this._Input_JiaDian.text.SafeToInt32(0);
		if (num > 0 && num <= RoleAttributeDetailPart.nRemainPoint)
		{
			GameInstance.Game.SpriteAddPoint(this._SelectedTab, num);
			this._Close_JiaDian.MouseLeftButtonUp(sender, e);
		}
		else
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("请输入要分配的点数!"), new object[0]), 0, -1, -1, 0);
		}
	}

	public void OnSubmit_XiDian(object sender, MouseEvent e)
	{
		int num = Global.Data.roleData.UserMoney + Global.Data.roleData.Gold;
		if (num >= this.nFeeXidian)
		{
			GameInstance.Game.SpriteClearPoint();
			this._Close_XiDian.MouseLeftButtonUp(sender, e);
		}
		else
		{
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedZuanshi, null, string.Empty, string.Empty);
		}
	}

	public void OnSubmit_TuiJian(object sender, MouseEvent e)
	{
		if (RoleAttributeDetailPart.nRemainPoint > 0)
		{
			GameInstance.Game.SpriteRecommendPoint(RoleAttributeDetailPart.addStrengthPoint, RoleAttributeDetailPart.addIntelligencePoint, RoleAttributeDetailPart.addDexterityPoint, RoleAttributeDetailPart.addConstitutionPoint);
		}
		this._Close_TuiJian.MouseLeftButtonUp(sender, e);
	}

	public void NotifyResult(int ret, int type = 0)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (ret == -1)
		{
			GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, StringUtil.substitute(Global.GetLang("剩余属性点数不足!"), new object[0]), 0, -1, -1, 0);
		}
		else
		{
			this.InitPartData();
		}
	}

	public static RoleAttributeDetailPart Instance;

	public GButton _Close;

	public TextBlock _Title;

	public ShowNetImage _Bak;

	public GButton _XiDian;

	public GButton _TuiJianJiaDian;

	public TextBlock _Remain;

	public UITable _Table;

	private RoleAttributeDetailPartItem[] Items;

	public int _SelectedTab;

	public UISprite _ModalBak;

	public GameObject _Part_JiaDian;

	public ShowNetImage _Bak_JiaDian;

	public GButton _Close_JiaDian;

	public GButton _Submit_JIaDian;

	public GButton _Max_JiaDian;

	public TextBlock _Name_JiaDian;

	public TextBlock _Total_JiaDian;

	public TextBlock _Avalid_JiaDian;

	public TextBox _Input_JiaDian;

	public TextBlock _Avalid_Point;

	public GameObject _Part_XiDian;

	public ShowNetImage _Bak_XiDian;

	public GButton _Close_XiDian;

	public GButton _Submit_XiDian;

	public TextBlock _Avalid_XiDian;

	public TextBlock _Fee_XiJian;

	public TextBlock _Fee_Desc;

	public GameObject _Part_TuiJian;

	public ShowNetImage _Bak_TuiJian;

	public GButton _Close_TuiJian;

	public GButton _Submit_TuiJian;

	public TextBlock _Scheme_TuiJian;

	private static int Occupation;

	private static int nTotalPoint;

	private static int nRemainPoint;

	private static int nUsedPoint;

	private static int nStrengthPoint;

	private static int nIntelligencePoint;

	private static int nDexterityPoint;

	private static int nConstitutionPoint;

	private static int addStrengthPoint;

	private static int addIntelligencePoint;

	private static int addDexterityPoint;

	private static int addConstitutionPoint;

	private int nFeeXidian;
}
