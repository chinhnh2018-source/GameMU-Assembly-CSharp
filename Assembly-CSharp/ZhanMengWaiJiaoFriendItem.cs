using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Data;

public class ZhanMengWaiJiaoFriendItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.btnCancelMengYue.Text = Global.GetLang("解除结盟");
		this.btnCancelRequest.Text = Global.GetLang("取消请求");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
		this.btnCancelMengYue.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = -1,
					IDType = this.zhanMengId,
					Title = this.zhanMengName
				});
			}
		};
		this.btnCancelRequest.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.DPSelectedItem != null)
			{
				this.DPSelectedItem(this, new DPSelectedItemEventArgs
				{
					ID = 0,
					IDType = this.zhanMengId
				});
			}
		};
	}

	public void SetValue(AllyData data)
	{
		if (data == null)
		{
			return;
		}
		this.zhanMengId = data.UnionID;
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(data.UnionZoneID, out ztBuffServerInfo))
		{
			this.zhanMengName = ztBuffServerInfo.strServerName;
		}
		else
		{
			this.zhanMengName = string.Format("S{0}.{1}", data.UnionZoneID, data.UnionName);
		}
		if (data.LogState == 12)
		{
			this.txtStatus.Text = ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("已结盟"), "17e43e");
			this.txtZhanMengName.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"99ccff",
				Global.FormatRoleNameZoneid(data.UnionZoneID, data.UnionName, 1, 1)
			});
			this.txtLevel.Text = ZhanMengWaiJiaoPart.GetFontColorContentForChinese(string.Format("{0}{1}", "Lv", data.UnionLevel), "ffffff");
			this.txtLeaderName.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"18bf35",
				Global.FormatRoleNameZoneid(data.UnionZoneID, data.LeaderName, 1, 1)
			});
			this.btnCancelMengYue.gameObject.SetActive(true);
			this.btnCancelRequest.gameObject.SetActive(false);
		}
		else if (data.LogState == 1)
		{
			this.txtStatus.Text = ZhanMengWaiJiaoPart.GetFontColorContentForChinese(Global.GetLang("请求中"), "ff0000");
			this.txtZhanMengName.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.FormatRoleNameZoneid(data.UnionZoneID, data.UnionName, 1, 1)
			});
			this.txtLevel.Text = ZhanMengWaiJiaoPart.GetFontColorContentForChinese(string.Format("{0}{1}", "Lv", data.UnionLevel), "808081");
			this.txtLeaderName.Text = Global.GetColorStringForNGUIText(new object[]
			{
				"808081",
				Global.FormatRoleNameZoneid(data.UnionZoneID, data.LeaderName, 1, 1)
			});
			this.btnCancelMengYue.gameObject.SetActive(false);
			this.btnCancelRequest.gameObject.SetActive(true);
		}
	}

	public TextBlock txtZhanMengName;

	public TextBlock txtLevel;

	public TextBlock txtLeaderName;

	public TextBlock txtStatus;

	public GButton btnCancelMengYue;

	public GButton btnCancelRequest;

	public DPSelectedItemEventHandler DPSelectedItem;

	private int zhanMengId;

	private string zhanMengName = string.Empty;
}
