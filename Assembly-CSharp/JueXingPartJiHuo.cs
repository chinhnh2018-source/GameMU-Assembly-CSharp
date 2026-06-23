using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;
using XMLCreater;

public class JueXingPartJiHuo : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnJiHuo.Label.text = Global.GetLang("激活");
		this.BtnGet.Label.text = Global.GetLang("获取");
		this.lblJiHuoCost.text = Global.GetLang("激活消耗");
		this.lblAlreadyJiHuo.text = Global.GetLang("已激活");
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("AwakenCondition", ',');
		int num = systemParamIntArrayByName[0];
		int excellenceNum = systemParamIntArrayByName[1];
		GoodsQuality equipQualityByExcellNum = JueXingData.GetEquipQualityByExcellNum(excellenceNum);
		string enchanceText = Global.GetEnchanceText(equipQualityByExcellNum);
		this.lblEffect.text = string.Format(Global.GetLang("属性生效条件:装备{0}阶{1}"), num, enchanceText);
		this.lblEffect.pivot = 5;
		this.lblEffect.transform.localPosition = new Vector3(-80f, this.lblEffect.transform.localPosition.y, this.lblEffect.transform.localPosition.z);
		this.lblAlreadyJiHuo.pivot = 5;
		this.lblAlreadyJiHuo.transform.localPosition = new Vector3(55f, -290f, -1f);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.attackTaoZhuang.OnSelectJueXingShi = new Action<JueXingShiItem>(this.OnSelectJueXingShi);
		this.defTaoZhuang.OnSelectJueXingShi = new Action<JueXingShiItem>(this.OnSelectJueXingShi);
		this.taoZhuangMenu.OnSelectTaoZhuang = new Action<MUAwakenSuitDetail>(this.OnSelectTaoZhuang);
		this.attackTaoZhuang.gameObject.SetActive(false);
		this.defTaoZhuang.gameObject.SetActive(false);
		this.BtnJiHuo.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_selectTaoZhuang == null)
			{
				return;
			}
			this.SendJuHuoInfo(this.m_selectTaoZhuang.ID, this.selectJueXingShi.JueXingShi.ID);
			Super.ShowNetWaiting(null);
		};
		this.BtnGet.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.m_selectTaoZhuang == null)
			{
				return;
			}
			if (this.selectJueXingShi == null)
			{
				return;
			}
			Super.ShowGuideWindow(BaodianGuidePart.GuideType.NeedJueXing, null, string.Empty, string.Empty);
		};
		UIEventListener.Get(this.iconCost.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTip);
	}

	private new void Start()
	{
		this.InitLeftTaoZhuangMeuns();
	}

	private void OnClickTip(GameObject go)
	{
		MUAwakenActivationDetail jueXingShi = this.m_jueXingShiSelect.JueXingShi;
		if (jueXingShi == null)
		{
			return;
		}
		int materialId = jueXingShi.Material.MaterialId;
		GoodVO goodsXmlNodeByID = ConfigGoods.GetGoodsXmlNodeByID(materialId);
		if (goodsXmlNodeByID == null)
		{
			return;
		}
		this.OpenTipWindow(goodsXmlNodeByID);
	}

	private void OnSelectJueXingShi(JueXingShiItem jueXingShiItem)
	{
		this.m_jueXingShiSelect = jueXingShiItem;
		MUAwakenActivationDetail jueXingShi = jueXingShiItem.JueXingShi;
		this.selectJueXingShi.positionType = jueXingShiItem.positionType;
		this.selectJueXingShi.SetJueXingShiId(jueXingShi.ID, jueXingShiItem.TaoZhuangId);
		bool flag = JueXingData.IsSelfJiHuo(jueXingShi.ID, this.selectJueXingShi.TaoZhuangId);
		if (flag)
		{
			this.goCost.SetActive(false);
			this.lblAlreadyJiHuo.gameObject.SetActive(true);
		}
		else
		{
			this.goCost.SetActive(true);
			int materialNum = JueXingData.GetMaterialNum(jueXingShi.Material.MaterialId);
			int num = jueXingShi.Material.Num;
			bool flag2 = materialNum >= num;
			string text = materialNum + "/" + num;
			this.lblCostNum.text = text;
			if (flag2)
			{
				this.lblCostNum.color = Color.white;
				this.BtnJiHuo.gameObject.SetActive(true);
				this.BtnGet.gameObject.SetActive(false);
			}
			else
			{
				this.lblCostNum.color = Color.red;
				this.BtnJiHuo.gameObject.SetActive(false);
				this.BtnGet.gameObject.SetActive(true);
			}
			this.iconCost.URL = JueXingData.GetItemURL(jueXingShi.Material.MaterialId);
			this.lblAlreadyJiHuo.gameObject.SetActive(false);
		}
		GoodsData equipInfo = JueXingData.GetEquipInfo(JueXingData.GetSelfJueXingEquips(), jueXingShiItem.positionType);
		bool flag3 = JueXingData.IsCanEffect(equipInfo);
		if (flag3)
		{
			this.lblEffect.gameObject.SetActive(false);
		}
		else
		{
			this.lblEffect.gameObject.SetActive(true);
		}
		this.lblSelectDes.text = this.GetJueXingShiDes(jueXingShi, flag, flag3);
	}

	public void InitInfos()
	{
		this.InitTaoZhuangInfos(JueXingData.GetTaoZhuangsByType(JueXingTaoZhuangType.AttackType), JueXingData.GetTaoZhuangsByType(JueXingTaoZhuangType.DefenseType));
	}

	public void InitTaoZhuangInfos(List<MUAwakenSuitDetail> attackTaoZhuangs, List<MUAwakenSuitDetail> defTaoZhuangs)
	{
		this.m_attackTaoZhuangs = attackTaoZhuangs;
		this.m_defTaoZhuangs = defTaoZhuangs;
	}

	public void InitLeftTaoZhuangMeuns()
	{
		this.taoZhuangMenu.InitTaoZhuangInfos(this.m_attackTaoZhuangs, this.m_defTaoZhuangs);
	}

	private void OnSelectTaoZhuang(MUAwakenSuitDetail taoZhuangIfno)
	{
		if (taoZhuangIfno.Type == 1)
		{
			this.OnSelectAttackTaoZhuang(taoZhuangIfno);
		}
		else if (taoZhuangIfno.Type == 2)
		{
			this.OnSelectDefTaoZhuang(taoZhuangIfno);
		}
		this.AddTeXiao(this.texiaoContainer, taoZhuangIfno);
	}

	private void OnSelectAttackTaoZhuang(MUAwakenSuitDetail attackTaoZhuangIfno)
	{
		if (this.m_selectTaoZhuang == attackTaoZhuangIfno)
		{
			return;
		}
		this.m_selectTaoZhuang = attackTaoZhuangIfno;
		this.attackTaoZhuang.gameObject.SetActive(true);
		this.defTaoZhuang.gameObject.SetActive(false);
		this.attackTaoZhuang.SetJueXingShis(attackTaoZhuangIfno);
		this.SetTaoZhuangDes(attackTaoZhuangIfno);
	}

	private void OnSelectDefTaoZhuang(MUAwakenSuitDetail defTaoZhuangIfno)
	{
		if (this.m_selectTaoZhuang == defTaoZhuangIfno)
		{
			return;
		}
		this.m_selectTaoZhuang = defTaoZhuangIfno;
		this.attackTaoZhuang.gameObject.SetActive(false);
		this.defTaoZhuang.gameObject.SetActive(true);
		this.defTaoZhuang.SetJueXingShis(defTaoZhuangIfno);
		this.SetTaoZhuangDes(defTaoZhuangIfno);
	}

	private void SetTaoZhuangDes(MUAwakenSuitDetail taoZhuangIfno)
	{
		TaoZhuangDesSettingInfo settingInfo = new TaoZhuangDesSettingInfo();
		this.lblTaoZhuangDes1.text = JueXingData.GetTaoZhuangShuXingDes(taoZhuangIfno, settingInfo);
	}

	private string GetJueXingShiDes(MUAwakenActivationDetail jueXingShi, bool beJiHuo, bool beEffect)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string lang = Global.GetLang(jueXingShi.Name);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"f7f7de",
			lang
		}));
		List<MUPropInfo> baseProps = jueXingShi.BaseProps;
		Dictionary<string, string> shuXingString = JueXingData.GetShuXingString(baseProps, 1f);
		Dictionary<string, string>.Enumerator enumerator = shuXingString.GetEnumerator();
		while (enumerator.MoveNext())
		{
			object[] array = new object[2];
			array[0] = "cea46c";
			int num = 1;
			KeyValuePair<string, string> keyValuePair = enumerator.Current;
			array[num] = Global.GetLang(keyValuePair.Key + " : ");
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(array);
			object[] array2 = new object[2];
			array2[0] = "f7f7de";
			int num2 = 1;
			KeyValuePair<string, string> keyValuePair2 = enumerator.Current;
			array2[num2] = keyValuePair2.Value;
			string colorStringForNGUIText2 = Global.GetColorStringForNGUIText(array2);
			stringBuilder.AppendFormat("\n\r{0}{1}", colorStringForNGUIText, colorStringForNGUIText2);
		}
		return stringBuilder.ToString();
	}

	private void AddTeXiao(GameObject parent, MUAwakenSuitDetail taozhuang)
	{
		DecorationVO decorationVOByCode = ConfigDecoration.GetDecorationVOByCode(taozhuang.SpecialEffect.SafeToInt32(0));
		if (decorationVOByCode != null)
		{
			if (this.m_TeXiao != null)
			{
				Object.Destroy(this.m_TeXiao);
			}
			string resName = decorationVOByCode.ResName;
			string bundleID = MuAssetManager.GetBundleID("Decoration", resName);
			GameObject emptyLoader = U3DUtils.GetEmptyLoader(resName, bundleID, false, null, null, -1, null, -1, 1f, true, false, null);
			emptyLoader.transform.localPosition = Vector3.zero;
			emptyLoader.transform.localScale = Vector3.one;
			emptyLoader.GetComponent<AssetbundleLoader>().LoadOK = new AssetbundleLoaderComplete(this.RoleDecLoaderCompleteOK);
			emptyLoader.GetComponent<AssetbundleLoader>().AutoDestroySelf = true;
			this.m_TeXiao = emptyLoader;
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

	private void OpenTipWindow(GoodVO goodVO)
	{
		if (this.m_TipWindow == null)
		{
			this.m_TipWindow = U3DUtils.NEW<GChildWindow>();
			this.m_TipWindow.IsShowModal = true;
			this.m_TipWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_TipWindow, Global.GetLang("属性"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_TipWindow);
		}
		if (this.m_TipPart == null)
		{
			this.m_TipPart = U3DUtils.NEW<JueXingPartGoodTip>();
			this.m_TipPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseTipWindow();
			};
		}
		this.m_TipWindow.SetContent(this.m_TipWindow.BodyPresenter, this.m_TipPart, 0.0, 0.0, true);
		this.m_TipPart.InitGoods(goodVO);
	}

	private void CloseTipWindow()
	{
		if (null != this.m_TipPart)
		{
			this.m_TipPart.transform.parent = null;
			Object.Destroy(this.m_TipPart.gameObject);
			this.m_TipPart = null;
		}
		if (null != this.m_TipWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_TipWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_TipWindow, true);
			this.m_TipWindow = null;
		}
	}

	private void SendJuHuoInfo(int taoZhuangId, int jueXingShiId)
	{
		GameInstance.Game.JueXingShiJiHuo(taoZhuangId, jueXingShiId);
	}

	public void JueXingShiServerJiHuo(int taozhuangId, int jueXingShiId)
	{
		Super.HintMainText(Global.GetLang("激活成功"), 10, 3);
		this.taoZhuangMenu.RefershContent();
		if (jueXingShiId == this.m_jueXingShiSelect.JueXingShi.ID)
		{
			this.m_jueXingShiSelect.Refersh();
			this.OnSelectJueXingShi(this.m_jueXingShiSelect);
			this.SetTaoZhuangDes(this.m_selectTaoZhuang);
		}
	}

	private const string JiHuoColor = "ffffff";

	private const string NotJiHuoColor = "b4b4b4";

	private const string ShuXingNameColor = "cea46c";

	private const string ShuXingValuecolor = "f7f7de";

	private const string ShuXingNotEffectColor = "786F6F";

	public JueXingJiHuoMenu taoZhuangMenu;

	public JueXingTaoZhuangItem attackTaoZhuang;

	public JueXingTaoZhuangItem defTaoZhuang;

	public TextBlock lblTaoZhuangDes1;

	public JueXingShiItem selectJueXingShi;

	public TextBlock lblSelectDes;

	public UILabel lblEffect;

	public GameObject goCost;

	public GButton BtnJiHuo;

	public GButton BtnGet;

	public UILabel lblJiHuoCost;

	public UILabel lblCostNum;

	public ShowNetImage iconCost;

	public UILabel lblAlreadyJiHuo;

	public GameObject texiaoContainer;

	private MUAwakenSuitDetail m_selectTaoZhuang;

	private JueXingShiItem m_jueXingShiSelect;

	private List<MUAwakenSuitDetail> m_attackTaoZhuangs;

	private List<MUAwakenSuitDetail> m_defTaoZhuangs;

	private GameObject m_TeXiao;

	protected GChildWindow m_TipWindow;

	protected JueXingPartGoodTip m_TipPart;
}
