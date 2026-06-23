using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class ShenShiPartTuiJian : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("推荐");
		this.lblFuWenWord.text = Global.GetLang("推荐符文");
		this.lblShenShiWord.text = Global.GetLang("推荐神识");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		UIEventListener.Get(this.bgObj).onClick = new UIEventListener.VoidDelegate(this.OnCloseTip);
		this.InitContent();
	}

	protected void InitContent()
	{
		int occupation = Global.Data.roleData.Occupation;
		MUFuWenCommend fuWenCommend = ShenShiPartTuiJian.GetFuWenCommend(occupation);
		if (fuWenCommend == null)
		{
			return;
		}
		int num = 0;
		while (num < fuWenCommend.FuWenID.Count && num < 3)
		{
			ShenShiTuiJianItem shenShiTuiJianItem = U3DUtils.NEW<ShenShiTuiJianItem>();
			shenShiTuiJianItem.OnClickTuiJianItem = new Action<ShenShiTuiJianItem>(this.OnClickItem);
			shenShiTuiJianItem.transform.SetParent(this.FuWenTable.gameObject.transform);
			shenShiTuiJianItem.transform.localPosition = Vector3.zero;
			shenShiTuiJianItem.transform.localScale = Vector3.one;
			shenShiTuiJianItem.InitInfo(1, fuWenCommend.FuWenID[num]);
			num++;
		}
		int num2 = 0;
		while (num2 < fuWenCommend.ShenShiID.Count && num2 < 3)
		{
			ShenShiTuiJianItem shenShiTuiJianItem2 = U3DUtils.NEW<ShenShiTuiJianItem>();
			shenShiTuiJianItem2.OnClickTuiJianItem = new Action<ShenShiTuiJianItem>(this.OnClickItem);
			shenShiTuiJianItem2.transform.SetParent(this.ShenShiTable.gameObject.transform);
			shenShiTuiJianItem2.transform.localPosition = Vector3.zero;
			shenShiTuiJianItem2.transform.localScale = Vector3.one;
			shenShiTuiJianItem2.InitInfo(2, fuWenCommend.ShenShiID[num2]);
			num2++;
		}
		this.FuWenTable.Reposition();
		this.ShenShiTable.Reposition();
	}

	protected void OnClickItem(ShenShiTuiJianItem item)
	{
		this.lblTipName.text = item.Name;
		this.lblDes.text = item.Des;
		this.objTip.SetActive(true);
	}

	private void OnCloseTip(GameObject go)
	{
		this.objTip.SetActive(false);
	}

	public static MUFuWenCommend GetFuWenCommend(int occupationID)
	{
		if (ShenShiPartTuiJian.m_allMUAllFuWenCommend == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/FuWenCommend.xml");
			if (gameResXml == null)
			{
				return null;
			}
			ShenShiPartTuiJian.m_allMUAllFuWenCommend = new MUAllFuWenCommend(gameResXml);
		}
		return ShenShiPartTuiJian.m_allMUAllFuWenCommend.GetFuWenCommendByOccupationID(occupationID);
	}

	public static void ClearXMLData()
	{
		ShenShiPartTuiJian.m_allMUAllFuWenCommend = null;
	}

	public UILabel lblTitle;

	public UILabel lblFuWenWord;

	public UILabel lblShenShiWord;

	public UIGrid FuWenTable;

	public UIGrid ShenShiTable;

	public GButton BtnClose;

	public GameObject objTip;

	public UILabel lblTipName;

	public TextBlock lblDes;

	public GameObject bgObj;

	public DPSelectedItemEventHandler CloseHandler;

	public static MUAllFuWenCommend m_allMUAllFuWenCommend;
}
