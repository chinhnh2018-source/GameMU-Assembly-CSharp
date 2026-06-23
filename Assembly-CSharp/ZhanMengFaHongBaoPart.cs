using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ZhanMengFaHongBaoPart : UserControl
{
	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.InitEvent();
		this.InitValue();
	}

	private void OnEnable()
	{
		this.InitTextInPrefabs();
		this.InitValue();
	}

	private void InitTextInPrefabs()
	{
		this.mCheckBoxNormal.Text = Global.GetLang("普通红包");
		this.mCheckBoxShouQi.Text = Global.GetLang("手气红包");
		string text = string.Format("{0}{1}{2}", Global.GetLang("红包发出"), ConfigSystemParam.GetSystemParamByName("RedPacketsTime", true), Global.GetLang("小时后过期"));
		this.mLblTips.Text = text;
		this.mLblHongBaoNumBegin.Text = Global.GetLang("红包个数：");
		this.mLblSingleMoney.Text = Global.GetLang("单个金额：");
		this.mLblSendSumMoney.Text = Global.GetLang("总金额：");
		this.mBtnSend.Text = Global.GetLang("发红包");
		this.mInputLeaveMessage.Text = Global.GetLang(ConfigSystemParam.GetSystemParamByName("RedPacketsMessage", true));
		this.mLblSendSumMoney.Pivot = 5;
		this.mLblSendSumMoney.X = 30.0;
	}

	private void InitEvent()
	{
		this.InitInputHongBaoNumEvent();
		this.InitInputDiamondNumEvent();
		if (null != this.mCheckBoxNormal)
		{
			this.mCheckBoxNormal.CheckChanged = delegate(object s, BaseEventArgs e)
			{
				if (!this.mRecordNormalCheckStatus)
				{
					this.mRecordNormalCheckStatus = true;
					this.mRecordShouQiCheckStatus = false;
					this.mHongBaoType = ZhanMengFaHongBaoPart.EHongBaoType.Normal;
					this.ChangeHongBaoType(this.mHongBaoType);
				}
			};
		}
		if (null != this.mCheckBoxShouQi)
		{
			this.mCheckBoxShouQi.CheckChanged = delegate(object s, BaseEventArgs e)
			{
				if (!this.mRecordShouQiCheckStatus)
				{
					this.mRecordShouQiCheckStatus = true;
					this.mRecordNormalCheckStatus = false;
					this.mHongBaoType = ZhanMengFaHongBaoPart.EHongBaoType.ShouQi;
					this.ChangeHongBaoType(this.mHongBaoType);
				}
			};
		}
		if (this.mBtnSend != null)
		{
			this.mBtnSend.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (!this.isClick)
				{
					return;
				}
				this.isClick = false;
				FaHongBaoData data = this.GetFaHongBaoData();
				if (data == null)
				{
					return;
				}
				if (data.diamondNum < this.minDiamondNum_cfg)
				{
					string msg = string.Format("{0}{1}{2}", Global.GetLang("红包总额不能小于"), this.minDiamondNum_cfg, Global.GetLang("钻，发红包失败"));
					Super.HintMainText(msg, 10, 3);
					return;
				}
				if (data.diamondNum > this.maxDiamondNum_cfg)
				{
					string msg2 = string.Format("{0}{1}{2}", Global.GetLang("红包总额已超"), this.maxDiamondNum_cfg, Global.GetLang("钻，发红包失败"));
					Super.HintMainText(msg2, 10, 3);
					return;
				}
				if (data.message.Length > 20)
				{
					Super.HintMainText(Global.GetLang("留言不能超过20个字符"), 10, 3);
					return;
				}
				if (data.diamondNum > Global.Data.roleData.UserMoney)
				{
					Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
					return;
				}
				GChildWindow messageBoxWindow = Super.ShowMessageBox(Super.MainWindowRoot, 1, Global.GetLang("提示"), Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					string.Format(Global.GetLang("需要消耗{0}钻石，确定吗？"), data.diamondNum)
				}), -1, -1, -1, -1, 0.7, default(Vector3), null, null);
				messageBoxWindow.ChildWindowClose = delegate(object s1, EventArgs e1)
				{
					int messageBoxReturn = messageBoxWindow.MessageBoxReturn;
					Super.CloseMessageBox(Super.MainWindowRoot, messageBoxWindow);
					if (messageBoxReturn == 0)
					{
						GameInstance.Game.SendFaHongBaoRequest(data);
					}
					return true;
				};
			};
		}
	}

	private new void Update()
	{
		if (!this.isClick)
		{
			this.CDTime -= Time.deltaTime;
			if (this.CDTime <= 0f)
			{
				this.CDTime = 1f;
				this.isClick = true;
			}
		}
	}

	private void InitInputHongBaoNumEvent()
	{
		if (this.mInputHongBaoNum != null)
		{
			UIEventListener.Get(this.mInputHongBaoNumClick).onClick = delegate(GameObject s)
			{
				PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.HongBaoNumDPS, this.mInputHongBaoNum.Label, 0, -100);
			};
		}
		this.HongBaoNumDPS = delegate(object s, DPSelectedItemEventArgs e)
		{
			int num = e.ID;
			if (num <= 0)
			{
				Super.HintMainText(Global.GetLang("红包数量不能为0"), 10, 3);
				return;
			}
			if (num <= this.minHongBaoNum_cfg)
			{
				num = this.minHongBaoNum_cfg;
			}
			int mCurrentZhanMengMemberNum = this.maxHongBaoNum_cfg;
			if (Global.Data != null)
			{
				mCurrentZhanMengMemberNum = Global.Data.mCurrentZhanMengMemberNum;
			}
			if (num > mCurrentZhanMengMemberNum)
			{
				num = mCurrentZhanMengMemberNum;
				string msg = string.Format("{0}{1}{2}{3}{4}{5}", new object[]
				{
					Global.GetLang("当前战盟【"),
					num,
					Global.GetLang("】个成员"),
					Global.GetLang("，红包个数最多【"),
					num,
					Global.GetLang("】个")
				});
				Super.HintMainText(msg, 10, 3);
			}
			this.mHongBaoNum = num;
			this.CalculateSumDiamondByHongBaoNum(num, this.mHongBaoType);
		};
	}

	private void InitInputDiamondNumEvent()
	{
		if (this.mInputSingleMoney != null)
		{
			UIEventListener.Get(this.mInputSingleMoneyClick).onClick = delegate(GameObject s)
			{
				if (!this.IsInputCorrectFormat(this.mInputHongBaoNum.Text, 1))
				{
					Super.HintMainText(Global.GetLang("请输入红包数量"), 10, 3);
					return;
				}
				PlayZone.GlobalPlayZone.OpenNumberKeyboardPart(this.DaimondNumDPS, this.mInputSingleMoney.Label, 0, -100);
			};
		}
		this.DaimondNumDPS = delegate(object s, DPSelectedItemEventArgs e)
		{
			int id = e.ID;
			if (id <= 0)
			{
				Super.HintMainText(Global.GetLang("红包金额不能为0"), 10, 3);
				return;
			}
			if (id >= this.maxDiamondNum_cfg)
			{
				id = this.maxDiamondNum_cfg;
			}
			if (this.mHongBaoType == ZhanMengFaHongBaoPart.EHongBaoType.ShouQi && this.mHongBaoNum > id)
			{
				Super.HintMainText(Global.GetLang("钻石数量必须大于红包数量"), 10, 3);
				this.SetSumDiamondToDefault();
				this.mInputSingleMoney.Text = "0";
				return;
			}
			this.CalculateSumDiamondByDiamondNum(id, this.mHongBaoType);
		};
	}

	private string GetSumDiamondFontColor(int sumDiamond)
	{
		bool flag = sumDiamond <= Global.Data.roleData.UserMoney;
		return (!flag) ? "ff0000" : "f0f0f0";
	}

	private void InitValue()
	{
		this.mCheckBoxNormal.Check = false;
		this.mCheckBoxShouQi.Check = true;
		this.RefreshDiamondNum();
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("RedPacketsLimit", ',');
		if (systemParamIntArrayByName.Length >= 4)
		{
			this.minHongBaoNum_cfg = systemParamIntArrayByName[0];
			this.maxHongBaoNum_cfg = systemParamIntArrayByName[1];
			this.minDiamondNum_cfg = systemParamIntArrayByName[2];
			this.maxDiamondNum_cfg = systemParamIntArrayByName[3];
		}
		this.mInputHongBaoNum.Text = "0";
		this.mInputSingleMoney.Text = "0";
		this.mLblSendSumMoneyValue.Text = "0";
		this.mHongBaoType = ZhanMengFaHongBaoPart.EHongBaoType.ShouQi;
		this.ChangeHongBaoType(this.mHongBaoType);
		this.mRecordShouQiCheckStatus = true;
		this.mRecordNormalCheckStatus = false;
	}

	public void RefreshDiamondNum()
	{
		this.mLblCurrentSumDiamonds.Text = Global.Data.roleData.UserMoney.ToString();
	}

	private void ChangeHongBaoType(ZhanMengFaHongBaoPart.EHongBaoType type)
	{
		if (type == ZhanMengFaHongBaoPart.EHongBaoType.Normal)
		{
			int valueByLabelText = this.GetValueByLabelText(this.mInputHongBaoNum.Text);
			int valueByLabelText2 = this.GetValueByLabelText(this.mInputSingleMoney.Text);
			this.mLblSingleMoney.Text = Global.GetLang("单个金额：");
			int num = (valueByLabelText2 > 0) ? (valueByLabelText2 / valueByLabelText) : 0;
			this.mInputSingleMoney.Text = num.ToString();
			this.DiamondSumNum = (valueByLabelText * num).ToString();
			this.mLblSendSumMoneyValue.Text = Global.GetColorStringForNGUIText(new object[]
			{
				this.GetSumDiamondFontColor(valueByLabelText * num),
				valueByLabelText * num
			});
		}
		else if (type == ZhanMengFaHongBaoPart.EHongBaoType.ShouQi)
		{
			int valueByLabelText3 = this.GetValueByLabelText(this.mInputHongBaoNum.Text);
			int valueByLabelText4 = this.GetValueByLabelText(this.mInputSingleMoney.Text);
			this.mLblSingleMoney.Text = Global.GetLang("红包总额：");
			int num2 = valueByLabelText3 * valueByLabelText4;
			this.DiamondSumNum = num2.ToString();
			this.mInputSingleMoney.Text = this.DiamondSumNum;
			this.mLblSendSumMoneyValue.Text = Global.GetColorStringForNGUIText(new object[]
			{
				this.GetSumDiamondFontColor(num2),
				num2
			});
		}
	}

	private void SetSumDiamondToDefault()
	{
		this.DiamondSumNum = "0";
		this.mLblSendSumMoneyValue.Text = "0";
	}

	private void CalculateSumDiamondByHongBaoNum(int hongBaoNum, ZhanMengFaHongBaoPart.EHongBaoType type)
	{
		this.mInputHongBaoNum.Text = hongBaoNum.ToString();
		int num = ConvertExt.SafeConvertToInt32(this.mInputSingleMoney.Text);
		int num2 = 0;
		if (type == ZhanMengFaHongBaoPart.EHongBaoType.Normal)
		{
			num2 = hongBaoNum * num;
		}
		else if (type == ZhanMengFaHongBaoPart.EHongBaoType.ShouQi)
		{
			num2 = num;
		}
		this.DiamondSumNum = num2.ToString();
		this.mLblSendSumMoneyValue.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.GetSumDiamondFontColor(num2),
			num2
		});
	}

	private void CalculateSumDiamondByDiamondNum(int diamondNum, ZhanMengFaHongBaoPart.EHongBaoType type)
	{
		this.mInputSingleMoney.Text = diamondNum.ToString();
		int num = ConvertExt.SafeConvertToInt32(this.mInputHongBaoNum.Text);
		int num2 = 0;
		if (type == ZhanMengFaHongBaoPart.EHongBaoType.Normal)
		{
			num2 = num * diamondNum;
		}
		else if (type == ZhanMengFaHongBaoPart.EHongBaoType.ShouQi)
		{
			num2 = diamondNum;
		}
		this.DiamondSumNum = num2.ToString();
		this.mLblSendSumMoneyValue.Text = Global.GetColorStringForNGUIText(new object[]
		{
			this.GetSumDiamondFontColor(num2),
			num2
		});
	}

	private FaHongBaoData GetFaHongBaoData()
	{
		FaHongBaoData faHongBaoData = new FaHongBaoData();
		faHongBaoData.type = (int)this.mHongBaoType;
		faHongBaoData.count = this.GetValueByLabelText(this.mInputHongBaoNum.Text);
		faHongBaoData.diamondNum = this.GetValueByLabelText(this.DiamondSumNum);
		faHongBaoData.message = this.mInputLeaveMessage.Text;
		if (faHongBaoData.count <= 0)
		{
			Super.HintMainText(Global.GetLang("红包数量不能为0"), 10, 3);
			this.SetSumDiamondToDefault();
			return new FaHongBaoData();
		}
		if (faHongBaoData.diamondNum <= 0)
		{
			Super.HintMainText(Global.GetLang("钻石数量不能为0"), 10, 3);
			this.SetSumDiamondToDefault();
			return new FaHongBaoData();
		}
		if (!string.IsNullOrEmpty(faHongBaoData.message))
		{
			faHongBaoData.message = Global.ReplaceFilterFileds(faHongBaoData.message);
			faHongBaoData.message = this.MessageSpaceCharReplace(faHongBaoData.message);
		}
		return faHongBaoData;
	}

	private int GetValueByLabelText(string result)
	{
		return (!string.IsNullOrEmpty(result)) ? ConvertExt.SafeConvertToInt32(result) : 0;
	}

	private void SetEnableSendBtn(GButton btn, bool isShow)
	{
		btn.GetComponent<BoxCollider>().enabled = isShow;
		btn.GetComponentInChildren<UISprite>().color = ((!isShow) ? Color.gray : Color.white);
	}

	private bool IsInputCorrectFormat(string content, int type)
	{
		bool result = false;
		if (string.IsNullOrEmpty(content))
		{
			return result;
		}
		if (type == 1)
		{
			return ConvertExt.SafeConvertToInt32(content) > 0;
		}
		if (type == 2)
		{
			return content.Length > 0;
		}
		return result;
	}

	public void NotifyFaHongBaoResult(int result)
	{
		if (result >= 0)
		{
			Super.HintMainText(Global.GetLang("红包已发出"), 10, 3);
		}
		else if (result == -41)
		{
			string msg = string.Format("{0}{1}{2}", Global.GetLang("每小时最多发放【"), ConvertExt.SafeConvertToInt32(ConfigSystemParam.GetSystemParamByName("RedPacketsNumMax", true)), Global.GetLang("】个红包，请1小时后再发"));
			Super.HintMainText(msg, 10, 3);
		}
		else
		{
			string errMsg = StdErrorCode.GetErrMsg(result, true, true);
			Super.HintMainText(Global.GetLang(errMsg), 10, 3);
		}
		this.RefreshDiamondNum();
	}

	protected override void OnDestroy()
	{
		this.HongBaoNumDPS = null;
		this.DaimondNumDPS = null;
		this.mLblCurrentSumDiamonds = null;
		this.mLblTips = null;
		this.mLblHongBaoNumBegin = null;
		this.mInputHongBaoNum = null;
		this.mLblHongBaoNumEnd = null;
		this.mLblSingleMoney = null;
		this.mInputSingleMoney = null;
		this.mInputLeaveMessage = null;
		this.mLblSendSumMoney = null;
		this.mLblSendSumMoneyValue = null;
		this.mCheckBoxNormal = null;
		this.mCheckBoxShouQi = null;
		this.mBtnSend = null;
	}

	private string MessageSpaceCharReplace(string message)
	{
		char[] array = message.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Equals(' '))
			{
				array[i] = '_';
			}
		}
		message = new string(array);
		return message;
	}

	public DPSelectedItemEventHandler HongBaoNumDPS;

	public DPSelectedItemEventHandler DaimondNumDPS;

	public TextBlock mLblCurrentSumDiamonds;

	public TextBlock mLblTips;

	public TextBlock mLblHongBaoNumBegin;

	public TextBlock mInputHongBaoNum;

	public GameObject mInputHongBaoNumClick;

	public TextBlock mLblHongBaoNumEnd;

	public TextBlock mLblSingleMoney;

	public TextBlock mInputSingleMoney;

	public GameObject mInputSingleMoneyClick;

	public TextBox mInputLeaveMessage;

	public TextBlock mLblSendSumMoney;

	public TextBlock mLblSendSumMoneyValue;

	public GCheckBox mCheckBoxNormal;

	public GCheckBox mCheckBoxShouQi;

	public GButton mBtnSend;

	private int mHongBaoNum;

	private ZhanMengFaHongBaoPart.EHongBaoType mHongBaoType;

	private string DiamondSumNum;

	private int minHongBaoNum_cfg;

	private int maxHongBaoNum_cfg;

	private int minDiamondNum_cfg;

	private int maxDiamondNum_cfg;

	private float CDTime = 1f;

	private bool isClick = true;

	private bool mRecordNormalCheckStatus;

	private bool mRecordShouQiCheckStatus;

	private enum EHongBaoType
	{
		Normal,
		ShouQi
	}
}
