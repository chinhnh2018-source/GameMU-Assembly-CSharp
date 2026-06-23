using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class GEquipTipsResult : UserControl
{
	public void RenderTips(Dictionary<int, int> resultDict)
	{
		this.SetText(resultDict);
		this.SetPropsPanel();
	}

	private void SetText(Dictionary<int, int> resultDict)
	{
		if (resultDict == null)
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = "+";
		string text5 = string.Empty;
		bool isRight = true;
		foreach (int num in resultDict.Keys)
		{
			text5 = string.Empty;
			if (num != 0 && num != GTipServiceEx.HandKey)
			{
				if (num != 3 && num != 5 && num != 7 && num != 9)
				{
					if (resultDict[num] != 0)
					{
						if (resultDict[num] > 0)
						{
							text3 = "3aab1f";
							text4 = "+";
						}
						else if (resultDict[num] < 0)
						{
							text3 = "fd010c";
							text4 = string.Empty;
						}
						if (num == 177)
						{
							int flag = resultDict[num];
							text2 = this.GetXingyunAttributeStr(flag);
							if (text2 != string.Empty)
							{
								text += Global.GetColorStringForNGUIText(new object[]
								{
									text3,
									text2
								});
							}
						}
						else
						{
							if (num == 178 || num == 179)
							{
								if (num == 178)
								{
									text3 = "fd010c";
									isRight = false;
								}
								if (num == 179)
								{
									text3 = "3aab1f";
									isRight = true;
								}
								int flag = resultDict[num];
								text2 = this.GetZhuoyueAttributeStr(flag, isRight);
								if (text2 != string.Empty)
								{
									text += Global.GetColorStringForNGUIText(new object[]
									{
										text3,
										text2
									});
								}
								continue;
							}
							text5 = ExtPropIndexes.ExtPropIndexChineseNames[num];
						}
						if (!string.IsNullOrEmpty(text5))
						{
							if (ExtPropIndexes.ExtPropIndexPercents[num] == 0)
							{
								text2 = string.Format("{0}: {1}{2}\n", text5, text4, resultDict[num]);
							}
							else
							{
								text2 = string.Format("{0}: {1}{2}%\n", text5, text4, resultDict[num]);
							}
							text += Global.GetColorStringForNGUIText(new object[]
							{
								text3,
								text2
							});
						}
					}
				}
			}
		}
		this.txtResult.Text = ((!(text != string.Empty)) ? Global.GetLang("属性相同") : text);
		int num2 = -1;
		resultDict.TryGetValue(GTipServiceEx.HandKey, ref num2);
		text2 = string.Empty;
		if (num2 == 1)
		{
			text2 = Global.GetLang("[右手]");
		}
		else if (num2 == 0)
		{
			text2 = Global.GetLang("[左手]");
		}
		this.txtTitle.Text = string.Format(Global.GetLang("佩戴后属性{0}"), text2);
	}

	private void SetPropsPanel()
	{
		this.Bak.transform.localScale = new Vector3(this.Bak.transform.localScale.x, (float)(this.txtResult.ActualHeight + 80.0), 1f);
	}

	public string GetZhuoyuePropStr(int flag, bool isRight)
	{
		string result = string.Empty;
		string text = string.Empty;
		string text2 = ZhuoyuePropIndexes.ZhuoyuePropIndexChineseNames[flag];
		if (flag == 1 || flag == 2)
		{
			text = ((Global.Data.roleData.Level + Global.Data.roleData.ChangeLifeCount * 100) / ZhuoyuePropIndexes.ZhuoyuePropIndexValues[flag]).ToString();
		}
		else
		{
			text = ZhuoyuePropIndexes.ZhuoyuePropIndexValues[flag].ToString();
		}
		if (ZhuoyuePropIndexes.ZhuoyuePropIndexPercents[flag] == 1)
		{
			result = string.Format("{0}: {2}{1}%", text2, text, (!isRight) ? "-" : "+");
		}
		else
		{
			result = string.Format("{0}: {2}{1}", text2, text, (!isRight) ? "-" : "+");
		}
		return result;
	}

	private string GetZhuoyueAttributeStr(int flag, bool isRight = true)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (flag <= 0)
		{
			if (flag >= 0)
			{
				return string.Empty;
			}
		}
		flag = Math.Abs(flag);
		int num = 24;
		for (int i = 0; i < num; i++)
		{
			if (Global.GetIntSomeBit(flag, i) == 1)
			{
				text2 = this.GetZhuoyuePropStr(i, isRight);
				if (!string.IsNullOrEmpty(text2))
				{
					text += text2;
					text += "\n";
				}
			}
		}
		return text;
	}

	private string GetXingyunAttributeStr(int flag)
	{
		string text = string.Empty;
		if (flag == 0)
		{
			return text;
		}
		if (flag > 0)
		{
			text += Global.GetLang("幸运一击概率: +5%");
		}
		else
		{
			text += Global.GetLang("幸运一击概率: -5%");
		}
		return text + "\n";
	}

	public UISprite Bak;

	public TextBlock txtTitle;

	public TextBlock txtResult;
}
