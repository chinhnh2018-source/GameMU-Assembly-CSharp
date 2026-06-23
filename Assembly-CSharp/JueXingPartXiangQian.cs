using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class JueXingPartXiangQian : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnAttack.Label.text = Global.GetLang("选择");
		this.BtnDef.Label.text = Global.GetLang("选择");
		this.lblAttackTitle.text = Global.GetLang("攻击觉醒");
		this.lblAttactShuXing.text = Global.GetLang("防御觉醒");
		this.lblDefTitle.text = Global.GetLang("防御觉醒");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnAttack.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectAttackTaoZhuang();
		};
		this.BtnDef.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnSelectDefTaoZhuang();
		};
	}

	public void ReRefersh()
	{
		this.SetJueXingContent(JueXingData.GetEquipTaoZhuangId(JueXingTaoZhuangType.AttackType), JueXingData.GetEquipTaoZhuangId(JueXingTaoZhuangType.DefenseType));
	}

	public void SetJueXingContent(int attackTaoZhuangId, int defTaoZhuangId)
	{
		if (attackTaoZhuangId <= 0)
		{
			this.m_attackTaoZhuang = null;
			this.attackTaoZhuang.SetNoTaoZhuang();
			this.BtnAttack.Label.text = Global.GetLang("选择");
		}
		else
		{
			this.m_attackTaoZhuang = JueXingData.GetAwakenSuitDetailById(attackTaoZhuangId);
			this.attackTaoZhuang.SetJueXingShis(this.m_attackTaoZhuang);
			this.BtnAttack.Label.text = Global.GetLang("更换");
		}
		if (defTaoZhuangId <= 0)
		{
			this.m_defTaoZhuang = null;
			this.defTaoZhuang.SetNoTaoZhuang();
			this.BtnDef.Label.text = Global.GetLang("选择");
		}
		else
		{
			this.m_defTaoZhuang = JueXingData.GetAwakenSuitDetailById(defTaoZhuangId);
			this.defTaoZhuang.SetJueXingShis(this.m_defTaoZhuang);
			this.BtnDef.Label.text = Global.GetLang("更换");
		}
		this.RefershTeXiao();
		this.SetShuXingInfo();
	}

	public void RefershTeXiao()
	{
		for (int i = 0; i < this.attackTeXiao.childCount; i++)
		{
			Object.Destroy(this.attackTeXiao.GetChild(i).gameObject);
		}
		for (int j = 0; j < this.defTeXiao.childCount; j++)
		{
			Object.Destroy(this.defTeXiao.GetChild(j).gameObject);
		}
		if (this.m_attackTaoZhuang != null)
		{
			this.AddTeXiao(this.attackTeXiao.gameObject, this.m_attackTaoZhuang);
		}
		if (this.m_defTaoZhuang != null)
		{
			this.AddTeXiao(this.defTeXiao.gameObject, this.m_defTaoZhuang);
		}
	}

	private void AddTeXiao(GameObject parent, MUAwakenSuitDetail taozhuang)
	{
		DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(taozhuang.SpecialEffect.SafeToInt32(0));
		if (decorationVOByCode != null)
		{
			string resName = decorationVOByCode.ResName;
			string bundleID = MuAssetManager.GetBundleID("Decoration", resName);
			GameObject emptyLoader = U3DUtils.GetEmptyLoader(resName, bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			emptyLoader.transform.localPosition = Vector3.zero;
			emptyLoader.transform.localScale = Vector3.one;
			emptyLoader.GetComponent<AssetbundleLoader>().LoadOK = new AssetbundleLoaderComplete(this.RoleDecLoaderCompleteOK);
			emptyLoader.GetComponent<AssetbundleLoader>().AutoDestroySelf = true;
			U3DUtils.AddChild(parent, emptyLoader, true);
		}
	}

	private void RoleDecLoaderCompleteOK(AssetbundleLoader loader, GameObject go)
	{
		if (go != null)
		{
			Transform transform = loader.transform;
			LookAtCameraYRotationOnly component = go.GetComponent<LookAtCameraYRotationOnly>();
			if (component != null)
			{
				Object.Destroy(component);
			}
			go.transform.SetParent(transform);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
		}
	}

	private void SetShuXingInfo()
	{
		if (this.m_attackTaoZhuang == null)
		{
			this.lblAttackName.gameObject.SetActive(false);
			this.lblAttactShuXing.gameObject.SetActive(false);
			this.defContainer.localPosition = new Vector3(0f, -36f, 0f);
		}
		else
		{
			this.lblAttackName.gameObject.SetActive(true);
			this.lblAttactShuXing.gameObject.SetActive(true);
			this.lblAttackName.text = string.Concat(new object[]
			{
				Global.GetLang(this.m_attackTaoZhuang.Name),
				"(",
				JueXingData.GetJiHuoJueXingShiNum(this.m_attackTaoZhuang),
				"/",
				this.m_attackTaoZhuang.AwakenIDs.Count,
				")"
			});
			this.lblAttactShuXing.text = this.GetTaoZhuangShuXing(this.m_attackTaoZhuang);
			this.defContainer.localPosition = new Vector3(0f, (float)(-36.0 - this.lblAttactShuXing.ActualHeight - 50.0), 0f);
		}
		if (this.m_defTaoZhuang == null)
		{
			this.lblDefName.gameObject.SetActive(false);
			this.lblDefShuXing.gameObject.SetActive(false);
		}
		else
		{
			this.lblDefName.gameObject.SetActive(true);
			this.lblDefShuXing.gameObject.SetActive(true);
			this.lblDefName.text = string.Concat(new object[]
			{
				Global.GetLang(this.m_defTaoZhuang.Name),
				"(",
				JueXingData.GetJiHuoJueXingShiNum(this.m_defTaoZhuang),
				"/",
				this.m_defTaoZhuang.AwakenIDs.Count,
				")"
			});
			this.lblDefShuXing.text = this.GetTaoZhuangShuXing(this.m_defTaoZhuang);
		}
	}

	private void OnSelectAttackTaoZhuang()
	{
		if (this.HasXiangQian(JueXingTaoZhuangType.AttackType))
		{
			this.OpenTaoZhuangSelectWindow(JueXingTaoZhuangType.AttackType);
		}
		else
		{
			string lang = Global.GetLang("请前往激活界面激活觉醒石。觉醒石套装有一档套装属性被激活，就可以进行镶嵌");
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s2, DPSelectedItemEventArgs e2)
			{
			}, buttons);
		}
	}

	private void OnSelectDefTaoZhuang()
	{
		if (this.HasXiangQian(JueXingTaoZhuangType.DefenseType))
		{
			this.OpenTaoZhuangSelectWindow(JueXingTaoZhuangType.DefenseType);
		}
		else
		{
			string lang = Global.GetLang("请前往激活界面激活觉醒石。觉醒石套装有一档套装属性被激活，就可以进行镶嵌");
			string[] buttons = new string[]
			{
				Global.GetLang("确定")
			};
			Super.ShowMessageBoxEx(Global.GetLang("提示"), lang, delegate(object s2, DPSelectedItemEventArgs e2)
			{
			}, buttons);
		}
	}

	private bool HasXiangQian(JueXingTaoZhuangType type)
	{
		List<MUAwakenSuitDetail> taoZhuangsByType = JueXingData.GetTaoZhuangsByType(type);
		for (int i = 0; i < taoZhuangsByType.Count; i++)
		{
			if (taoZhuangsByType[i].TaoZhuangProps1Num <= JueXingData.GetJiHuoJueXingShiNum(taoZhuangsByType[i]))
			{
				return true;
			}
		}
		return false;
	}

	private string GetTaoZhuangShuXing(MUAwakenSuitDetail taoZhuang)
	{
		TaoZhuangDesSettingInfo settingInfo = new TaoZhuangDesSettingInfo();
		return JueXingData.GetTaoZhuangShuXingDes(taoZhuang, settingInfo);
	}

	private void OpenTaoZhuangSelectWindow(JueXingTaoZhuangType type)
	{
		if (this.m_taoZhuangSelectWindow == null)
		{
			this.m_taoZhuangSelectWindow = U3DUtils.NEW<GChildWindow>();
			this.m_taoZhuangSelectWindow.IsShowModal = true;
			this.m_taoZhuangSelectWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_taoZhuangSelectWindow, Global.GetLang("套装选择"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_taoZhuangSelectWindow);
		}
		if (this.m_taoZhuangSelectPart == null)
		{
			this.m_taoZhuangSelectPart = U3DUtils.NEW<JueXingPartSelect>();
			this.m_taoZhuangSelectPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseTaoZhuangSelectWindow();
			};
		}
		this.m_taoZhuangSelectWindow.SetContent(this.m_taoZhuangSelectWindow.BodyPresenter, this.m_taoZhuangSelectPart, 0.0, 0.0, true);
		this.m_taoZhuangSelectPart.InitTaoZhuangs(type);
	}

	private void CloseTaoZhuangSelectWindow()
	{
		if (null != this.m_taoZhuangSelectPart)
		{
			this.m_taoZhuangSelectPart.transform.parent = null;
			Object.Destroy(this.m_taoZhuangSelectPart.gameObject);
			this.m_taoZhuangSelectPart = null;
		}
		if (null != this.m_taoZhuangSelectWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_taoZhuangSelectWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_taoZhuangSelectWindow, true);
			this.m_taoZhuangSelectWindow = null;
		}
	}

	private void OnEnable()
	{
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<int, int>("CMD_SPR_JUEXING_TAOCHANGE", new Action<int, int>(this.ServerSelectZhuang));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<int, int>("CMD_SPR_JUEXING_TAOCHANGE", new Action<int, int>(this.ServerSelectZhuang));
	}

	private void ServerSelectZhuang(int type, int taoZhuangId)
	{
		if (taoZhuangId == 0)
		{
			Super.HintMainText(Global.GetLang("卸下成功"), 10, 3);
		}
		else
		{
			Super.HintMainText(Global.GetLang("装备成功"), 10, 3);
		}
		this.SetJueXingContent(JueXingData.GetEquipTaoZhuangId(JueXingTaoZhuangType.AttackType), JueXingData.GetEquipTaoZhuangId(JueXingTaoZhuangType.DefenseType));
		if (this.m_taoZhuangSelectPart != null)
		{
			this.m_taoZhuangSelectPart.SetEquipTaoZhuang(taoZhuangId);
		}
	}

	private const int TitleHeight = 36;

	private const string ShuXingNameColor = "cea46c";

	private const string ShuXingValuecolor = "f7f7de";

	public JueXingTaoZhuangItem attackTaoZhuang;

	public JueXingTaoZhuangItem defTaoZhuang;

	public GButton BtnAttack;

	public GButton BtnDef;

	public UILabel lblAttackTitle;

	public UILabel lblAttackName;

	public TextBlock lblAttactShuXing;

	public UILabel lblDefTitle;

	public UILabel lblDefName;

	public TextBlock lblDefShuXing;

	public Transform defContainer;

	public Transform attackTeXiao;

	public Transform defTeXiao;

	private MUAwakenSuitDetail m_attackTaoZhuang;

	private MUAwakenSuitDetail m_defTaoZhuang;

	protected GChildWindow m_taoZhuangSelectWindow;

	protected JueXingPartSelect m_taoZhuangSelectPart;
}
