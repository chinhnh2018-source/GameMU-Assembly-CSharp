using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Network.Tools;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class HuoDongPartLiPinDuiHuan : UserControl
{
	private void InitTextInPrefabs()
	{
		this.m_BtnCommit.Text = Global.GetLang("兑换");
		this.m_LblKeyWord.text = Global.GetLang("请在此输入激活码");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		if (null != this.m_BtnCommit)
		{
			this.m_BtnCommit.MouseLeftButtonUp = delegate(object s, MouseEvent e)
			{
				if (this.CanCheck)
				{
					this.CanCheck = false;
					base.StartCoroutine<bool>(this.ChangeValue());
					if (this.m_LblKeyWord.text.Trim() != string.Empty)
					{
						bool flag = ConfigSystemParam.GetSystemParamIntByName("ActiveCodeIsOpen") == 0L;
						if (flag)
						{
							GameInstance.Game.SpriteGetSongLiGift(this.m_LblKeyWord.text.Trim());
						}
						else
						{
							this.SendPlamtMsg();
						}
					}
					else
					{
						Super.HintMainText(Global.GetLang("请输入礼品码！"), 10, 3);
					}
				}
			};
		}
		this.m_BtnPaste.gameObject.SetActive(false);
	}

	private IEnumerator ChangeValue()
	{
		yield return new WaitForSeconds(0.85f);
		this.CanCheck = true;
		yield break;
	}

	private void ActiveCodeCallBack(WWW data)
	{
		if (0 < this.m_HttpPostDataList.Count)
		{
			this.m_HttpPostDataList.RemoveAt(0);
		}
		if (data != null && string.IsNullOrEmpty(data.error))
		{
			int num = Global.SafeConvertToInt32(data.text);
			if (num < 0)
			{
				int num2 = num;
				switch (num2 + 22)
				{
				case 0:
					Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
					goto IL_235;
				case 1:
					Super.HintMainText(Global.GetLang("礼包码请求超时"), 10, 3);
					goto IL_235;
				case 2:
					Super.HintMainText(Global.GetLang("表示主题不匹配"), 10, 3);
					goto IL_235;
				case 4:
					Super.HintMainText(Global.GetLang("当前区服不可使用该激活码"), 10, 3);
					goto IL_235;
				case 5:
					Super.HintMainText(Global.GetLang("激活码已过期或不可使用"), 10, 3);
					goto IL_235;
				case 6:
					Super.HintMainText(Global.GetLang("激活码使用失败，请确认使用渠道"), 10, 3);
					goto IL_235;
				case 7:
					Super.HintMainText(Global.GetLang("激活码使用失败，请确认使用平台"), 10, 3);
					goto IL_235;
				case 8:
					Super.HintMainText(Global.GetLang("该礼包码已经被使用"), 10, 3);
					goto IL_235;
				case 10:
					Super.HintMainText(Global.GetLang("礼品码已经超过了最大使用次数限制，请输入其他的礼品码!"), 10, 3);
					goto IL_235;
				case 11:
					Super.HintMainText(Global.GetLang("礼品码不存在，请输入有效的礼品码!"), 10, 3);
					goto IL_235;
				case 12:
					Super.HintMainText(Global.GetLang("礼品码错误，请输入当前区的礼品码!"), 10, 3);
					goto IL_235;
				case 17:
					Super.HintMainText(Global.GetLang("礼包码不可用"), 10, 3);
					goto IL_235;
				case 18:
					Super.HintMainText(Global.GetLang("其他人正在校验"), 10, 3);
					goto IL_235;
				case 19:
					Super.HintMainText(Global.GetLang("服务器错误"), 10, 3);
					goto IL_235;
				case 20:
					Super.HintMainText(Global.GetLang("功能未开启"), 10, 3);
					goto IL_235;
				case 21:
					Super.HintMainText(Global.GetLang("领取活动礼物时出错"), 10, 3);
					goto IL_235;
				}
				Super.HintMainText(Global.GetLang("领取活动礼物时出错"), 10, 3);
				IL_235:;
			}
			else if (num == 0)
			{
				Super.HintMainText(Global.GetLang("激活成功，奖励已通过邮件发放"), 10, 3);
			}
			else if (num == 2)
			{
				Super.HintMainText(Global.GetLang("背包空己满，奖励已通过邮件发放"), 10, 3);
			}
		}
		else if (data != null)
		{
		}
	}

	private void SendPlamtMsg()
	{
		ClientVerifyGift codeData = new ClientVerifyGift();
		codeData.codeno = this.m_LblKeyWord.text.Trim();
		codeData.zoneid = Global.Data.roleData.ZoneID;
		codeData.rid = Global.Data.roleData.RoleID;
		codeData.uid = Global.Data.UserID;
		codeData.appid = 1;
		codeData.lTime = TimeManager.GetCorrectLocalTime();
		codeData.channel = AndroidSDKPlugin.PlatName;
		if (PlatSDKMgr.PlatName == "YYB")
		{
			codeData.ptid = 10;
		}
		else
		{
			codeData.ptid = 9;
		}
		codeData.token = MD5Helper.get_md5_string(string.Concat(new object[]
		{
			codeData.ptid,
			string.Empty,
			codeData.lTime,
			codeData.codeno
		}));
		codeData.themeid = Global.Data.ServerData.LastServer.nFirstLevelServerID;
		string text = ConfigSystemParam.GetSystemParamByName("HttpTask", true);
		if (string.IsNullOrEmpty(text))
		{
			text = "http://119.29.10.118/Validate.aspx";
		}
		if (this.m_HttpPostDataList.Find((HttpPostData e) => e._ClientVerifyGift.codeno == codeData.codeno) == null)
		{
			HttpPostData httpPostData = new HttpPostData();
			httpPostData._ClientVerifyGift = codeData;
			httpPostData.HaveSend = false;
			httpPostData.HttpPost = text;
			httpPostData.LifeTime = 0f;
			this.m_HttpPostDataList.Add(httpPostData);
		}
		else
		{
			Super.HintMainText(Global.GetLang("此礼品码已在兑换的路上，请耐心等待"), 10, 3);
		}
	}

	public override void Update()
	{
		base.Update();
		if (0 < this.m_HttpPostDataList.Count && this.m_HttpPostDataList[0] != null)
		{
			HttpPostData httpPostData = this.m_HttpPostDataList[0];
			if (!httpPostData.HaveSend)
			{
				SimpleHttpTask.HttpPost(httpPostData.HttpPost, null, DataHelper.ObjectToBytes<ClientVerifyGift>(httpPostData._ClientVerifyGift), new SimpleHttpTask.HttpCallback(this.ActiveCodeCallBack), 10f);
				httpPostData.HaveSend = true;
			}
			else if (httpPostData.LifeTime <= this.m_WaitingTime)
			{
				httpPostData.LifeTime += Time.deltaTime;
			}
			else
			{
				this.m_HttpPostDataList.RemoveAt(0);
			}
		}
	}

	public GButton m_BtnPaste;

	public GButton m_BtnCommit;

	public UILabel m_LblKeyWord;

	private bool CanCheck = true;

	private List<HttpPostData> m_HttpPostDataList = new List<HttpPostData>();

	private float m_WaitingTime = 1f;

	private enum PingTaiId
	{
		YueYuIOS = 7,
		IOS,
		Android,
		YingYongBao,
		TaiWan = 23
	}
}
