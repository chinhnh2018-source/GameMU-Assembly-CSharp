using System;
using System.Text.RegularExpressions;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;
using UnityEngine;

public class MyReferee : RecallGoodsEx
{
	public static MyReferee.ClientEReturnState clientEReturnState
	{
		get
		{
			return MyReferee.curSt;
		}
		set
		{
			MyReferee.preSt = MyReferee.curSt;
			MyReferee.curSt = value;
			MyReferee.bClientUpdate = true;
		}
	}

	protected override void InitTextInPrefabs()
	{
		base.InitTextInPrefabs();
		this.submitRefereeBtn.Text = Global.GetLang("确定");
		this.yanzhengDefault_Sunmit.Text = Global.GetLang("确定");
		this.yanzhengFail_Sunmit.Text = Global.GetLang("确定");
		this.refereeInputField.text = Global.GetLang("推荐人的ID号码");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.SetActivityContent();
		this.yanzhengDefalt.SetActive(false);
		this.yanzhengIng.SetActive(false);
		this.yanzhengFail.SetActive(false);
		this.yanzhengTuiJianRen.SetActive(false);
		this.inputTextBox.GotFocus = new EventHandler(this.GotFocus);
		this.defaultPosX_refereeInputField = this.refereeInputField.cachedTransform.localPosition.x;
		if (null != this.submitRefereeBtn)
		{
			this.submitRefereeBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (null != this.refereeInputField)
				{
					string text = this.refereeInputField.text;
					if (!string.IsNullOrEmpty(text) && !text.Equals(Global.GetLang("推荐人的ID号码")) && this.IsValid(text))
					{
						ServerBufferZhaoHui.Instance.SendCheckTuiJianRen(text.Trim());
					}
					else
					{
						Super.HintMainText(Global.GetLang("当前服务器中未找到对应ID的角色，请验证后再试"), 10, 3);
					}
				}
			};
		}
		if (null != this.yanzhengDefault_Sunmit)
		{
			this.yanzhengDefault_Sunmit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				ServerBufferZhaoHui.Instance.SendCheckTuiJianRen("0");
			};
		}
		if (null != this.yanzhengFail_Sunmit)
		{
			this.yanzhengFail_Sunmit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.Default;
			};
		}
		if (null != this.activityTime)
		{
			this.activityTime.text = base.StringActivityStartTimeYYYYMMDD_HHMMSS() + " - " + base.StringActivityEndTimeYYYYMMDD_HHMMSS();
		}
	}

	private void GotFocus(object sender, EventArgs e)
	{
		if (this.inputTextBox.text.Contains(Global.GetLang("验证失败")) || this.inputTextBox.text.Contains(Global.GetLang("验证中, 请稍候")) || this.inputTextBox.text.Contains(Global.GetLang("只能被同平台用户召回")))
		{
			this.inputTextBox.text = string.Empty;
			Vector3 localPosition = this.refereeInputField.cachedTransform.localPosition;
			localPosition.x = this.defaultPosX_refereeInputField;
			this.refereeInputField.cachedTransform.localPosition = localPosition;
			this.refereeInputField.text = this.inputTextBox.caratChar;
		}
	}

	private bool IsValid(string id)
	{
		string text = "^[0-9A-Za-z#]*$";
		Regex regex = new Regex(text);
		return regex.IsMatch(id) && id.Contains("#");
	}

	protected override void _Timer_Tick()
	{
		base._Timer_Tick();
		if (ServerBufferZhaoHui.Instance.userData == null)
		{
			return;
		}
		if (MyReferee.curSt == MyReferee.ClientEReturnState.Default)
		{
			if (MyReferee.bClientUpdate)
			{
				this.yanzhengDefalt.SetActive(false);
				this.yanzhengIng.SetActive(false);
				this.yanzhengFail.SetActive(false);
				this.yanzhengTuiJianRen.SetActive(false);
			}
			if (!this.yanzhengDefalt.activeSelf)
			{
				this.yanzhengDefalt.SetActive(true);
			}
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.WaitCheck)
		{
			if (MyReferee.bClientUpdate)
			{
				this.yanzhengDefalt.SetActive(false);
				this.yanzhengIng.SetActive(false);
				this.yanzhengFail.SetActive(false);
				this.yanzhengTuiJianRen.SetActive(false);
			}
			if (!this.yanzhengIng.activeSelf)
			{
				this.yanzhengIng.SetActive(true);
			}
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.CheckFail)
		{
			if (MyReferee.preSt == MyReferee.ClientEReturnState.Default || MyReferee.preSt == MyReferee.ClientEReturnState.WaitCheck)
			{
				if (MyReferee.bClientUpdate)
				{
					this.yanzhengDefalt.SetActive(false);
					this.yanzhengIng.SetActive(false);
					this.yanzhengFail.SetActive(false);
					this.yanzhengTuiJianRen.SetActive(false);
				}
				this.yanzhengFail.SetActive(true);
			}
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.SignFail)
		{
			if (MyReferee.preSt == MyReferee.ClientEReturnState.Default || MyReferee.preSt == MyReferee.ClientEReturnState.WaitSign)
			{
				if (MyReferee.bClientUpdate)
				{
					this.yanzhengDefalt.SetActive(false);
					this.yanzhengIng.SetActive(false);
					this.yanzhengFail.SetActive(false);
					this.yanzhengTuiJianRen.SetActive(false);
				}
				if (!this.yanzhengTuiJianRen.activeSelf)
				{
					this.yanzhengTuiJianRen.SetActive(true);
				}
				this.SetRefereeInfo(Global.GetLang("验证中, 请稍候"));
			}
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.Check)
		{
			if (MyReferee.bClientUpdate)
			{
				this.yanzhengDefalt.SetActive(false);
				this.yanzhengIng.SetActive(false);
				this.yanzhengFail.SetActive(false);
				this.yanzhengTuiJianRen.SetActive(false);
			}
			if (!this.yanzhengTuiJianRen.activeSelf)
			{
				this.yanzhengTuiJianRen.SetActive(true);
			}
			if (MyReferee.preSt == MyReferee.ClientEReturnState.LaoTaoReturnType1Fail)
			{
				this.RetYanZhengCodeToPrepareToCheck();
			}
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.WaitSign)
		{
			if (MyReferee.bClientUpdate)
			{
				this.yanzhengDefalt.SetActive(false);
				this.yanzhengIng.SetActive(false);
				this.yanzhengFail.SetActive(false);
				this.yanzhengTuiJianRen.SetActive(false);
			}
			if (MyReferee.preSt == MyReferee.ClientEReturnState.Default || MyReferee.preSt == MyReferee.ClientEReturnState.WaitCheck)
			{
			}
			if (!this.yanzhengTuiJianRen.activeSelf)
			{
				this.yanzhengTuiJianRen.SetActive(true);
			}
			this.SetRefereeInfo(Global.GetLang("验证中, 请稍候"));
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.CheckAndSign)
		{
			if (MyReferee.bClientUpdate)
			{
				this.yanzhengDefalt.SetActive(false);
				this.yanzhengIng.SetActive(false);
				this.yanzhengFail.SetActive(false);
				this.yanzhengTuiJianRen.SetActive(false);
			}
			if (MyReferee.preSt == MyReferee.ClientEReturnState.Default || MyReferee.preSt == MyReferee.ClientEReturnState.WaitCheck)
			{
			}
			if (!this.yanzhengTuiJianRen.activeSelf)
			{
				this.yanzhengTuiJianRen.SetActive(true);
			}
			this.SetRefereeInfo(ServerBufferZhaoHui.Instance.userData.RecallCode);
		}
		else if (MyReferee.curSt == MyReferee.ClientEReturnState.LaoTaoReturnType0Fail)
		{
			MyReferee.clientEReturnState = MyReferee.ClientEReturnState.CheckFail;
		}
		else if (MyReferee.curSt != MyReferee.ClientEReturnState.LaoTaoReturnType0OK)
		{
			if (MyReferee.curSt == MyReferee.ClientEReturnState.LaoTaoReturnType1Fail)
			{
				string lang = Global.GetLang("失败");
				this.inputTextBox.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("验证失败")
				});
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.Check;
			}
			else if (MyReferee.curSt == MyReferee.ClientEReturnState.LaoTaoReturnType1OK)
			{
			}
		}
		EReturnState clientReturnState = ServerBufferZhaoHui.Instance.ClientReturnState;
		if (clientReturnState != null)
		{
			if (clientReturnState != 1)
			{
				if (clientReturnState != 2 && clientReturnState != 3)
				{
					if (clientReturnState == 4)
					{
					}
				}
			}
		}
		this.lastSt = MyReferee.curSt;
		MyReferee.bClientUpdate = false;
	}

	public void SendToLaoTaoT(int type = 0)
	{
		if (type == 0)
		{
			VerifyPlayerRecallAccount.PostRecallInfo("0", type);
			MyReferee.clientEReturnState = MyReferee.ClientEReturnState.WaitCheck;
		}
		else
		{
			string text = this.refereeInputField.text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				int num = text.IndexOf('#');
				if (num > -1)
				{
					VerifyPlayerRecallAccount.PostRecallInfo(text.Substring(num + 1, text.Length - num - 1), type);
				}
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.WaitSign;
			}
		}
	}

	public void GetRefereeRequest()
	{
		if (string.IsNullOrEmpty(Global.Data.UserID))
		{
			return;
		}
		GameInstance.Game.GetMyReferee(Global.Data.UserID);
	}

	public void InitMyReferee()
	{
		if (ServerBufferZhaoHui.Instance.userData != null)
		{
			EReturnState serverReturnState = ServerBufferZhaoHui.Instance.ServerReturnState;
			if (serverReturnState == null)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.Default;
			}
			else if (serverReturnState == 1)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.WaitCheck;
			}
			else if (serverReturnState == 2)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.Check;
			}
			else if (serverReturnState == 3)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.WaitSign;
			}
			else if (serverReturnState == 4)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.CheckAndSign;
			}
			else if (serverReturnState == -51)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.CheckFail;
			}
			else if (serverReturnState == -50)
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.CheckFail;
			}
			else
			{
				MyReferee.clientEReturnState = MyReferee.ClientEReturnState.Default;
			}
		}
	}

	public void GetMyReferee(int status, string userID)
	{
		Super.HideNetWaiting();
		switch (status + 2)
		{
		case 0:
			Super.HintMainText(Global.GetLang("活动已结束"), 10, 3);
			break;
		default:
			if (status == -13)
			{
				this.inputTextBox.text = Global.GetColorStringForNGUIText(new object[]
				{
					"ff0000",
					Global.GetLang("只能被同平台用户召回")
				});
			}
			break;
		case 2:
			break;
		case 3:
			this.SetRefereeInfo(userID);
			break;
		case 4:
			this.SetRefereeInfo(Global.GetLang("验证中, 请稍候"));
			break;
		case 5:
			this.inputTextBox.text = Global.GetColorStringForNGUIText(new object[]
			{
				"ff0000",
				Global.GetLang("验证失败")
			});
			break;
		}
	}

	private void GetMyRefereeRequest()
	{
	}

	private void RetYanZhengCodeToPrepareToCheck()
	{
		if (null != this.refereeInfo)
		{
			this.refereeInfo.transform.parent.gameObject.SetActive(true);
			this.refereeInfo.text = string.Empty;
		}
		if (null != this.refereeInputField)
		{
			this.refereeInputField.transform.parent.gameObject.SetActive(true);
		}
		if (null != this.submitRefereeBtn)
		{
			this.submitRefereeBtn.gameObject.SetActive(true);
		}
	}

	private void SetRefereeInfo(string referee)
	{
		if (null != this.refereeInfo)
		{
			this.refereeInfo.transform.parent.gameObject.SetActive(true);
			this.refereeInfo.text = referee;
		}
		if (null != this.refereeInputField)
		{
			this.refereeInputField.transform.parent.gameObject.SetActive(false);
		}
		if (null != this.submitRefereeBtn)
		{
			this.submitRefereeBtn.gameObject.SetActive(false);
		}
	}

	private void SetActivityContent()
	{
		if (null != this.activityContent)
		{
			this.activityContent.text = this.HintMessage_ActivityCondition();
		}
	}

	private string HintMessage_ActivityCondition()
	{
		string xmlName = "Config/PlayerRecall/HuoDongZhaoHui.xml";
		XElement isolateResXml = Global.GetIsolateResXml(xmlName);
		if (isolateResXml == null)
		{
			return null;
		}
		string empty = string.Empty;
		XElement xelement = Global.GetXElement(isolateResXml, "HuoDongZhaoHui", "ID", "1");
		if (xelement == null)
		{
			return null;
		}
		string text = PlayerRecall.TimeStringToDateStrMD(Global.GetXElementAttributeStr(xelement, "NotLoggedInBegin"));
		string text2 = PlayerRecall.TimeStringToDateStrMD(Global.GetXElementAttributeStr(xelement, "NotLoggedInFinish"));
		string text3 = "0";
		string text4 = "0";
		string[] array = Global.GetXElementAttributeStr(xelement, "Level").Split(new char[]
		{
			','
		});
		if (array != null && array.Length > 1)
		{
			text3 = array[0];
			text4 = array[1];
		}
		return string.Format(Global.GetLang("{0}至{1}内未登录，{2}转{3}级以上"), new object[]
		{
			text,
			text2,
			text3,
			text4
		});
	}

	private static MyReferee.ClientEReturnState preSt;

	private static MyReferee.ClientEReturnState curSt;

	private static bool bClientUpdate = true;

	public GButton submitRefereeBtn;

	public TextBox inputTextBox;

	public UILabel refereeInputField;

	private float defaultPosX_refereeInputField;

	public UILabel refereeInfo;

	public UILabel activityTime;

	public UILabel daojishiLabel;

	public GameObject yanzhengDefalt;

	public GameObject yanzhengIng;

	public GameObject yanzhengFail;

	public GameObject yanzhengTuiJianRen;

	public GButton yanzhengDefault_Sunmit;

	public GButton yanzhengFail_Sunmit;

	public UILabel activityContent;

	private MyReferee.ClientEReturnState lastSt;

	public enum ClientEReturnState
	{
		Default,
		WaitCheck,
		Check,
		WaitSign,
		CheckAndSign,
		CheckFail,
		SignFail,
		LaoTaoReturnType0OK,
		LaoTaoReturnType0Fail,
		LaoTaoReturnType1OK,
		LaoTaoReturnType1Fail
	}
}
