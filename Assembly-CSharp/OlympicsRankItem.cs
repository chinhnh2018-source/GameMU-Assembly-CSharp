using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class OlympicsRankItem : UserControl
{
	public void SetValue(KFRankData data)
	{
		NGUITools.SetActive(this.rankNum.gameObject, false);
		NGUITools.SetActive(this.rankImg.gameObject, false);
		if (data.Rank <= 3)
		{
			NGUITools.SetActive(this.rankNum.gameObject, false);
			NGUITools.SetActive(this.rankImg.gameObject, true);
		}
		else
		{
			NGUITools.SetActive(this.rankNum.gameObject, true);
			NGUITools.SetActive(this.rankImg.gameObject, false);
		}
		if (data.Rank == 1)
		{
			this.rankImg.spriteName = "1";
		}
		else if (data.Rank == 2)
		{
			this.rankImg.spriteName = "2";
		}
		else if (data.Rank == 3)
		{
			this.rankImg.spriteName = "3";
		}
		else
		{
			this.rankNum.Text = data.Rank.ToString();
		}
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(data.ZoneID, out ztBuffServerInfo))
		{
			this.name.Text = Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.RoleName, 0);
		}
		else
		{
			this.name.Text = string.Format("S{0}.{1}", data.ZoneID, data.RoleName);
		}
		this.score.Text = data.Grade.ToString();
	}

	public UISprite rankImg;

	public TextBlock rankNum;

	public TextBlock name;

	public TextBlock score;
}
