using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;
using XMLCreater;

public class JueXingPartMoHua : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitleShuXing.text = Global.GetLang("基础属性");
		this.lblTitleJinJie.text = Global.GetLang("进阶消耗");
		this.lblTitleShengXing.text = Global.GetLang("升星消耗");
		this.btnJinJie.Text = Global.GetLang("进阶");
		this.btnShengXing.Text = Global.GetLang("升星");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.InitClickEvent();
		List<MUAwakenLevelDetail> awakenLevels = JueXingData.GetAwakenLevelInfos().AwakenLevels;
		if (awakenLevels != null && awakenLevels.Count > 0)
		{
			this.MaxJieNum = awakenLevels[awakenLevels.Count - 1].Order;
		}
		this.RefreshData();
		this.RefreshModel();
	}

	private void RefreshModel()
	{
		if (this.m_nowMoHuaLevel != null)
		{
			this.Load3DModel(this.m_nowMoHuaLevel.ModID);
		}
		else
		{
			this.Load3DModel(this.m_nextMoHuaLevel.ModID);
		}
	}

	private void Load3DModel(int modelId)
	{
		Modal3DShow gameModel = this.modelContainer.GetComponentInChildren<Modal3DShow>();
		if (gameModel != null)
		{
			gameModel.Clear();
			Object.DestroyObject(gameModel.gameObject);
		}
		Modal3DShow modal3DShow = U3DUtils.NEW<Modal3DShow>();
		U3DUtils.AddChild(this.modelContainer, modal3DShow.gameObject, false);
		ResourceResLoader resourceResLoader = UIHelper.LoadModelResource(modal3DShow, modelId, 1f, delegate(object e, DPSelectedItemEventArgs s)
		{
			gameModel = this.modelContainer.GetComponentInChildren<Modal3DShow>();
			if (gameModel != null)
			{
				SkinnedMeshRenderer[] componentsInChildren = gameModel.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
				if (componentsInChildren == null || componentsInChildren.Length <= 0)
				{
					return;
				}
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if ("Artist/PlayerCharacter".Equals(componentsInChildren[i].sharedMaterial.shader.name))
					{
						componentsInChildren[i].sharedMaterial.shader = Shader.Find("Artist/PlayerCharacterForUI");
					}
				}
			}
		});
	}

	public void SetJieShuAndXingShu(int jieShu, int xingShu)
	{
		this.SetNowAndNextInfos(jieShu, xingShu);
		this.SetXingXingNum(xingShu);
		this.SetJieShu(jieShu);
		this.InitShuXingInfos();
		if (xingShu == this.MaxXingNum && jieShu == this.MaxJieNum)
		{
			this.SetShowType(3);
			return;
		}
		if (xingShu == this.MaxXingNum)
		{
			this.SetShowType(1);
			return;
		}
		this.SetShowType(2);
	}

	public void RefreshData()
	{
		this.SetJieShuAndXingShu(JueXingData.GetJieShu(), JueXingData.GetXingShu());
	}

	private void InitShuXingInfos()
	{
		float num = 0f;
		float num2 = 0f;
		if (this.m_nowMoHuaLevel == null)
		{
			num = 0f;
			num2 = this.m_nextMoHuaLevel.EnlargeRate;
		}
		else if (this.m_nextMoHuaLevel == null)
		{
			num = this.m_nowMoHuaLevel.EnlargeRate;
			num2 = 0f;
		}
		else if (this.m_nextMoHuaLevel != null && this.m_nowMoHuaLevel != null)
		{
			num = this.m_nowMoHuaLevel.EnlargeRate;
			num2 = this.m_nextMoHuaLevel.EnlargeRate - this.m_nowMoHuaLevel.EnlargeRate;
		}
		this.lblShuXingBase.text = string.Format(Global.GetLang("觉醒石总属性:{0}%"), num);
		this.lblShuXingAdd.pivot = 2;
		this.lblShuXingAdd.transform.localPosition = new Vector3(92f, 101f, 0f);
		this.lblShuXingAdd.text = num2 + "%";
		List<MUAwakenLevelDetail> advancedEffectLevels = JueXingData.GetAdvancedEffectLevels();
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < advancedEffectLevels.Count; i++)
		{
			string text = "786F6F";
			if (this.m_nowMoHuaLevel != null && this.m_nowMoHuaLevel.Order >= advancedEffectLevels[i].Order)
			{
				text = "f7f7de";
			}
			string text2 = string.Format(Global.GetLang("{0}阶"), advancedEffectLevels[i].Order);
			string text3 = string.Empty;
			if (advancedEffectLevels[i].AdvancedEffects[0].BePercent)
			{
				text3 = (advancedEffectLevels[i].AdvancedEffects[0].PropNum * 100f).ToString("f0") + "%";
			}
			else
			{
				text3 = advancedEffectLevels[i].AdvancedEffects[0].PropNum.ToString();
			}
			string text4 = Global.GetLang(advancedEffectLevels[i].AdvancedEffects[0].ChinesePropName) + " + " + text3;
			string colorStringForNGUIText = Global.GetColorStringForNGUIText(new object[]
			{
				text,
				text2 + text4
			});
			if (i == advancedEffectLevels.Count - 1)
			{
				stringBuilder.AppendFormat("{0}", colorStringForNGUIText);
			}
			else
			{
				stringBuilder.AppendFormat("{0}\n\r", colorStringForNGUIText);
			}
		}
		this.lblShuXingJinJie.text = stringBuilder.ToString();
	}

	private void SetNowAndNextInfos(int jieShu, int xingShu)
	{
		this.m_nowMoHuaLevel = JueXingData.GetLevelsByOrderAndstar(jieShu, xingShu);
		if (this.m_nowMoHuaLevel == null)
		{
			this.m_nextMoHuaLevel = JueXingData.GetLevelsByOrderAndstar(1, 1);
		}
		else if (xingShu == this.MaxXingNum && jieShu == this.MaxJieNum)
		{
			this.m_nextMoHuaLevel = null;
		}
		else if (xingShu == this.MaxXingNum)
		{
			this.m_nextMoHuaLevel = JueXingData.GetLevelsByOrderAndstar(jieShu + 1, 0);
		}
		else
		{
			this.m_nextMoHuaLevel = JueXingData.GetLevelsByOrderAndstar(jieShu, xingShu + 1);
		}
	}

	public void SetXingXingNum(int starNum)
	{
		for (int i = 0; i < starNum; i++)
		{
			this.Stars[i].spriteName = "xingxing1";
		}
		for (int j = starNum; j < this.MaxXingNum; j++)
		{
			this.Stars[j].spriteName = "xingxing2";
		}
	}

	public void SetJieShu(int jieShu)
	{
		this.lblJieShu.text = jieShu.ToString();
	}

	public void SetShowType(int index)
	{
		if (index == 2 || index == 1)
		{
			this.JinJieContainer.SetActive(false);
			this.ShengXingContainer.SetActive(true);
			this.EndContainer.SetActive(false);
			GButton gbutton;
			if (index == 1)
			{
				this.lblTitleShengXing.text = Global.GetLang("进阶消耗");
				this.btnJinJie.gameObject.SetActive(true);
				this.btnShengXing.gameObject.SetActive(false);
				gbutton = this.btnJinJie;
			}
			else
			{
				this.lblTitleShengXing.text = Global.GetLang("升星消耗");
				this.btnJinJie.gameObject.SetActive(false);
				this.btnShengXing.gameObject.SetActive(true);
				gbutton = this.btnShengXing;
			}
			this.lblShengXingNum.text = this.m_nextMoHuaLevel.Awakenment.ToString();
			if (JueXingData.GetJueXingZhiChenNum() < (long)this.m_nextMoHuaLevel.Awakenment)
			{
				this.lblShengXingNum.color = Color.red;
				gbutton.isEnabled = false;
			}
			else
			{
				this.lblShengXingNum.color = Color.white;
				gbutton.isEnabled = true;
			}
		}
		else
		{
			this.JinJieContainer.SetActive(false);
			this.ShengXingContainer.SetActive(false);
			this.EndContainer.SetActive(true);
		}
	}

	private void InitClickEvent()
	{
		this.btnHuiShou.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnClickHuiShou();
		};
		this.btnShuXing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnClickShuXing();
		};
		this.btnJinJie.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnClickJinJie();
		};
		this.btnShengXing.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.OnClickShengXing();
		};
	}

	private void OnClickHuiShou()
	{
		this.OpenHuiShouWindow();
	}

	private void OnClickShuXing()
	{
		this.OpenShuXingWindow();
	}

	private void OnClickJinJie()
	{
		if (this.m_beCanClickAgain)
		{
			this.SendMoHuaInfo();
			this.m_beCanClickAgain = false;
			base.Invoke("ResetClickEventTime", 0.5f);
		}
		else
		{
			Super.HintMainText(Global.GetLang("您的点击频率太快"), 10, 3);
		}
	}

	private void OnClickShengXing()
	{
		if (this.m_beCanClickAgain)
		{
			this.SendMoHuaInfo();
			this.m_beCanClickAgain = false;
			base.Invoke("ResetClickEventTime", 0.5f);
		}
		else
		{
			Super.HintMainText(Global.GetLang("您的点击频率太快"), 10, 3);
		}
	}

	private void ResetClickEventTime()
	{
		this.m_beCanClickAgain = true;
	}

	private void OpenShuXingWindow()
	{
		if (this.m_shuXingWindow == null)
		{
			this.m_shuXingWindow = U3DUtils.NEW<GChildWindow>();
			this.m_shuXingWindow.IsShowModal = true;
			this.m_shuXingWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_shuXingWindow, Global.GetLang("总属性"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_shuXingWindow);
		}
		if (this.m_shuXingPart == null)
		{
			this.m_shuXingPart = U3DUtils.NEW<JueXingPartShuXing>();
			this.m_shuXingPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseShuXingWindow();
			};
		}
		this.m_shuXingWindow.SetContent(this.m_shuXingWindow.BodyPresenter, this.m_shuXingPart, 0.0, 0.0, true);
	}

	private void CloseShuXingWindow()
	{
		if (null != this.m_shuXingPart)
		{
			this.m_shuXingPart.transform.parent = null;
			Object.Destroy(this.m_shuXingPart.gameObject);
			this.m_shuXingPart = null;
		}
		if (null != this.m_shuXingWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_shuXingWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_shuXingWindow, true);
			this.m_shuXingWindow = null;
		}
	}

	private void OpenHuiShouWindow()
	{
		if (this.m_huiShouWindow == null)
		{
			this.m_huiShouWindow = U3DUtils.NEW<GChildWindow>();
			this.m_huiShouWindow.IsShowModal = true;
			this.m_huiShouWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(this.m_huiShouWindow, Global.GetLang("回收"));
			Super.GData.GlobalPlayZone.Children.Add(this.m_huiShouWindow);
		}
		if (this.m_huiShouPart == null)
		{
			this.m_huiShouPart = U3DUtils.NEW<JueXingPartHuiShou>();
			this.m_huiShouPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				this.CloseHuiShouWindow();
			};
		}
		this.m_huiShouWindow.SetContent(this.m_huiShouWindow.BodyPresenter, this.m_huiShouPart, 0.0, 0.0, true);
	}

	private void CloseHuiShouWindow()
	{
		if (null != this.m_huiShouPart)
		{
			this.m_huiShouPart.transform.parent = null;
			Object.Destroy(this.m_huiShouPart.gameObject);
			this.m_huiShouPart = null;
		}
		if (null != this.m_huiShouWindow)
		{
			Super.CloseChildWindow(base.Children, this.m_huiShouWindow);
			Super.GData.GlobalPlayZone.Children.Remove(this.m_huiShouWindow, true);
			this.m_huiShouWindow = null;
		}
	}

	private void OnEnable()
	{
		this.m_beCanClickAgain = true;
		this.AddEventLinster();
	}

	private void OnDisable()
	{
		this.RemoveEventLinster();
	}

	public void AddEventLinster()
	{
		MUEventManager.AddEventListener<int, int>("CMD_SPR_JUEXING_MOHUA", new Action<int, int>(this.ServerMoHua));
	}

	public void RemoveEventLinster()
	{
		MUEventManager.RemoveEventListener<int, int>("CMD_SPR_JUEXING_MOHUA", new Action<int, int>(this.ServerMoHua));
	}

	private void SendMoHuaInfo()
	{
		Super.ShowNetWaiting(null);
		GameInstance.Game.JueXingMoHua();
	}

	private void ServerMoHua(int jieShu, int xingShu)
	{
		base.StartCoroutine<bool>(this.ShowSuccess(jieShu, xingShu));
	}

	private IEnumerator ShowSuccess(int jieShu, int xingShu)
	{
		if (xingShu == 0)
		{
			this.jieTeXiao.SetActive(true);
			yield return new WaitForSeconds(1f);
			this.SetJieShuAndXingShu(JueXingData.GetJieShu(), JueXingData.GetXingShu());
			this.RefreshModel();
			yield return new WaitForSeconds(1f);
			this.jieTeXiao.SetActive(false);
		}
		else if (xingShu > 0)
		{
			this.SetJieShuAndXingShu(JueXingData.GetJieShu(), JueXingData.GetXingShu());
			GameObject newTeXiao = NGUITools.AddChild(this.xingTeXiao.transform.parent.gameObject, this.xingTeXiao);
			newTeXiao.transform.localPosition = this.xingTeXiao.transform.localPosition;
			Transform trans = this.Stars[xingShu - 1].transform;
			Vector3 pos = newTeXiao.transform.localPosition;
			pos.x = trans.localPosition.x;
			newTeXiao.transform.localPosition = pos;
			newTeXiao.SetActive(true);
			yield return new WaitForSeconds(1f);
			Object.Destroy(newTeXiao);
		}
		yield break;
	}

	private const string JiHuoColor = "f7f7de";

	private const string NotJiHuoColor = "786F6F";

	public UILabel lblJieShu;

	public GButton btnShuXing;

	public GButton btnHuiShou;

	public List<UISprite> Stars;

	public UILabel lblTitleShuXing;

	public UILabel lblShuXingBase;

	public UILabel lblShuXingAdd;

	public TextBlock lblShuXingJinJie;

	public GameObject JinJieContainer;

	public UILabel lblTitleJinJie;

	public UILabel lblJinJieNum;

	public GButton btnJinJie;

	public ShowNetImage imgCostIcon;

	public GameObject ShengXingContainer;

	public UILabel lblTitleShengXing;

	public UILabel lblShengXingNum;

	public GButton btnShengXing;

	public GameObject jieTeXiao;

	public GameObject xingTeXiao;

	public GameObject modelContainer;

	public GameObject EndContainer;

	private int MaxXingNum = 10;

	private int MaxJieNum = 10;

	private bool m_beCanClickAgain = true;

	private MUAwakenLevelDetail m_nowMoHuaLevel;

	private MUAwakenLevelDetail m_nextMoHuaLevel;

	protected GChildWindow m_shuXingWindow;

	protected JueXingPartShuXing m_shuXingPart;

	protected GChildWindow m_huiShouWindow;

	protected JueXingPartHuiShou m_huiShouPart;
}
