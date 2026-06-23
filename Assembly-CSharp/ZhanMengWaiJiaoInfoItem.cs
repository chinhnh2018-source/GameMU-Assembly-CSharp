using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class ZhanMengWaiJiaoInfoItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	private void SetContent(string content, string time)
	{
		this.txtContent.text = content;
		this.txtTime.text = time;
	}

	public void SetValue(AllyLogData data)
	{
		if (data == null)
		{
			return;
		}
		ZtBuffServerInfo ztBuffServerInfo = null;
		if (Global.GetNowServerIsZhuTiFu(data.UnionZoneID, out ztBuffServerInfo))
		{
			int logState = data.LogState;
			if (logState != 20)
			{
				if (logState != 21)
				{
					if (logState != 41)
					{
						if (logState != 42)
						{
							if (logState == 1)
							{
								this.SetContent(string.Concat(new string[]
								{
									Global.GetLang("{dac7ae}你向{-}"),
									"{18bf35}",
									Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.UnionName, 0),
									"{-}",
									Global.GetLang("{dac7ae}发送了结盟申请{-}")
								}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
							}
						}
						else
						{
							this.SetContent("{18bf35}" + Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.UnionName, 0) + "{-}" + Global.GetLang("{ff0000}解除了{-}{dac7ae}与你的结盟关系{-}"), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
						}
					}
					else
					{
						this.SetContent(string.Concat(new string[]
						{
							Global.GetLang("{dac7ae}你{-}{ff0000}解除了{-}{dac7ae}与{-}"),
							"{18bf35}",
							Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.UnionName, 0),
							"{-}",
							Global.GetLang("{dac7ae}的结盟关系{-}")
						}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
					}
				}
				else
				{
					this.SetContent(string.Concat(new string[]
					{
						"{18bf35}",
						Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.UnionName, 0),
						"{-}",
						Global.GetLang("{fac60d}同意了{-}"),
						Global.GetLang("{dac7ae}你的结盟申请，与你结为盟友{-}")
					}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
				}
			}
			else
			{
				this.SetContent(string.Concat(new string[]
				{
					"{18bf35}",
					Global.FormatRoleNameZhuTiFu(ztBuffServerInfo.strServerName, data.UnionName, 0),
					"{-}",
					Global.GetLang("{ff0000}拒绝了{-}"),
					Global.GetLang("{dac7ae}你的结盟申请，结盟失败{-}")
				}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
			}
		}
		else
		{
			int logState = data.LogState;
			if (logState != 20)
			{
				if (logState != 21)
				{
					if (logState != 41)
					{
						if (logState != 42)
						{
							if (logState == 1)
							{
								this.SetContent(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", new object[]
								{
									Global.GetLang("{dac7ae}你向{-}"),
									"{18bf35}[",
									"S",
									data.UnionZoneID,
									".",
									data.UnionName,
									"]{-}",
									Global.GetLang("{dac7ae}发送了结盟申请{-}")
								}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
							}
						}
						else
						{
							this.SetContent(string.Format("{0}{1}{2}{3}{4}{5}{6}", new object[]
							{
								"{18bf35}[",
								"S",
								data.UnionZoneID,
								".",
								data.UnionName,
								"]{-}",
								Global.GetLang("{ff0000}解除了{-}{dac7ae}与你的结盟关系{-}")
							}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
						}
					}
					else
					{
						this.SetContent(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", new object[]
						{
							Global.GetLang("{dac7ae}你{-}{ff0000}解除了{-}{dac7ae}与{-}"),
							"{18bf35}[",
							"S",
							data.UnionZoneID,
							".",
							data.UnionName,
							"]{-}",
							Global.GetLang("{dac7ae}的结盟关系{-}")
						}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
					}
				}
				else
				{
					this.SetContent(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", new object[]
					{
						"{18bf35}[",
						"S",
						data.UnionZoneID,
						".",
						data.UnionName,
						"]{-}",
						Global.GetLang("{fac60d}同意了{-}"),
						Global.GetLang("{dac7ae}你的结盟申请，与你结为盟友{-}")
					}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
				}
			}
			else
			{
				this.SetContent(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}", new object[]
				{
					"{18bf35}[",
					"S",
					data.UnionZoneID,
					".",
					data.UnionName,
					"]{-}",
					Global.GetLang("{ff0000}拒绝了{-}"),
					Global.GetLang("{dac7ae}你的结盟申请，结盟失败{-}")
				}), data.LogTime.ToString("yyyy-MM-dd HH:mm:ss"));
			}
		}
	}

	public TextBlock txtContent;

	public TextBlock txtTime;

	public int tempData;
}
