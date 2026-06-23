using System;
using HSGameEngine.GameEngine.Logic;

public class ZhanMengLianSaiGuanZhanRolelistItem : UserControl
{
	public int ID { get; set; }

	public bool Selected
	{
		set
		{
			NGUITools.SetActive(this.mSelectedBg.gameObject, value);
		}
	}

	public void InitData(GuanZhanRoleMiniData data)
	{
		this.ID = data.RoleID;
		this.mLblName.Text = data.Name;
		this.mLblOccupation.Text = this.GetZhiYe(data.Occupation);
		this.mLblLevel.Text = this.GetLevel(data.ChangeLevel, data.Level);
		this.mLblZhiWu.Text = this.GetZhiWu(data.BHZhiWu);
		if (Global.IsInLangHunLingYuScene())
		{
			this.mLblJiSha.Text = string.Empty;
		}
		else
		{
			this.mLblJiSha.Text = data.Param1.ToString();
		}
	}

	private string GetZhiYe(int Occupation)
	{
		return Global.GetOccupationStr(Occupation);
	}

	private string GetLevel(int zhuanSheng, int level)
	{
		return string.Format(Global.GetLang("{0}转{1}级"), zhuanSheng, level);
	}

	private string GetZhiWu(int flag)
	{
		return Global.GetBHZhiWu(flag);
	}

	protected override void OnDestroy()
	{
	}

	public TextBlock mLblName;

	public TextBlock mLblOccupation;

	public TextBlock mLblLevel;

	public TextBlock mLblZhiWu;

	public TextBlock mLblJiSha;

	public UISprite mBg;

	public UISprite mSelectedBg;
}
