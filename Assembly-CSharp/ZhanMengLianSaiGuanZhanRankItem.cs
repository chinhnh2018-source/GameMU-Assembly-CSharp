using System;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class ZhanMengLianSaiGuanZhanRankItem : UserControl
{
	protected override void InitializeComponent()
	{
		NGUITools.SetActive(this.mRankObj, false);
		NGUITools.SetActive(this.mJingCaiObj, false);
		this.InitChineseName();
	}

	public void InitRankData(BangHuiMatchRankInfo data, int index)
	{
		if (data == null)
		{
			return;
		}
		NGUITools.SetActive(this.mRankObj, true);
		int num = index + 1;
		if (num <= 3)
		{
			NGUITools.SetActive(this.mLblRankID.gameObject, false);
			NGUITools.SetActive(this.mImgRankID.gameObject, true);
			this.mImgRankID.spriteName = num.ToString();
		}
		else
		{
			NGUITools.SetActive(this.mLblRankID.gameObject, true);
			NGUITools.SetActive(this.mImgRankID.gameObject, false);
			this.mLblRankID.Text = num.ToString();
		}
		this.mLblZhanMengName.Text = data.Param1;
		this.mLblVictoryTimes.Text = data.Value + Global.GetLang("胜");
	}

	public void InitJingCaiData(BangHuiMatchGuessInfo data, int index)
	{
		if (data == null)
		{
			return;
		}
		NGUITools.SetActive(this.mJingCaiObj, true);
		try
		{
			this.mLblRound.Text = this.GetChineseNumber(data.round - 1);
			this.mLblContent.Text = this.GetString(new object[]
			{
				Global.GetLang("猜中场次："),
				data.right,
				Global.GetLang("场")
			});
			this.mLblValue.Text = data.jifen.ToString();
		}
		catch (Exception ex)
		{
			MUDebug.LogError<Exception>(new Exception[]
			{
				ex
			});
		}
	}

	public string GetString(params object[] args)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < args.Length; i++)
		{
			stringBuilder.Append(args[i]);
		}
		return stringBuilder.ToString();
	}

	private string GetChineseNumber(int num)
	{
		if (num < 0)
		{
			return string.Empty;
		}
		if (this.roundIndex != null && num <= this.roundIndex.Length - 1)
		{
			return this.roundIndex[num];
		}
		return string.Empty;
	}

	private void InitChineseName()
	{
		this.roundIndex = new string[]
		{
			Global.GetLang("第一轮"),
			Global.GetLang("第二轮"),
			Global.GetLang("第三轮"),
			Global.GetLang("第四轮"),
			Global.GetLang("第五轮"),
			Global.GetLang("第六轮"),
			Global.GetLang("第七轮"),
			Global.GetLang("第八轮"),
			Global.GetLang("第九轮"),
			Global.GetLang("第十轮"),
			Global.GetLang("第十一轮"),
			Global.GetLang("第十二轮"),
			Global.GetLang("第十三轮"),
			Global.GetLang("第十四轮"),
			Global.GetLang("第十五轮"),
			Global.GetLang("第十六轮"),
			Global.GetLang("第十七轮"),
			Global.GetLang("第十八轮"),
			Global.GetLang("第十九轮"),
			Global.GetLang("第二十轮")
		};
	}

	protected override void OnDestroy()
	{
		this.roundIndex = null;
		this.mRankObj = null;
		this.mImgRankID = null;
		this.mLblRankID = null;
		this.mLblZhanMengName = null;
		this.mLblVictoryTimes = null;
		this.mJingCaiObj = null;
		this.mLblRound = null;
		this.mLblContent = null;
		this.mLblValue = null;
	}

	public GameObject mRankObj;

	public UISprite mImgRankID;

	public TextBlock mLblRankID;

	public TextBlock mLblZhanMengName;

	public TextBlock mLblVictoryTimes;

	public GameObject mJingCaiObj;

	public TextBlock mLblRound;

	public TextBlock mLblContent;

	public TextBlock mLblValue;

	private string[] roundIndex;
}
