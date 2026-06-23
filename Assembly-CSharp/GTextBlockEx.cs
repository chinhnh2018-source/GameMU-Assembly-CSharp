using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class GTextBlockEx : GTextBlockOutLine
{
	public GTextBlockEx(string _text = "", int fontcolor = -1, int bgcolor = -1, int border = -1, int fontsize = -1, int shadow = 0) : base(string.Empty, -1, -1, -1, -1, -1)
	{
	}

	private void uiAddEvents()
	{
	}

	private void onLink(MouseEvent e)
	{
	}

	private void onMouseLeave(MouseEvent e)
	{
	}

	private void onMouseMove(MouseEvent e)
	{
	}

	public bool BlackOutLine
	{
		get
		{
			return this._BlackOutLine;
		}
		set
		{
			this._BlackOutLine = value;
		}
	}

	public string NoRenderText
	{
		get
		{
			return base.Text;
		}
		set
		{
			base.Text = value;
			this.SpecialTextDict.Clear();
			this.IndexArray.Clear();
		}
	}

	public void SetSpecialText(string text, SolidColorBrush brush, bool underLine, object tag = null, bool rendText = true)
	{
	}

	protected override void OnClick(string key)
	{
	}

	public string GetFormatedTextContent(string value, int offset, int roleID, bool isChat = false)
	{
		string text = value;
		if (offset > 0)
		{
		}
		int num = text.IndexOf("<$goods$>");
		if (num != -1)
		{
			text = text.Substring(0, num);
		}
		if (!isChat)
		{
			base.Text = text;
		}
		if (num != -1)
		{
			string goods = value.Substring(num + "<$goods$>".Length);
			return this.GetFormatedGoodsText(goods, text, offset, roleID);
		}
		return value;
	}

	public string GetFormatedStr(string value, string color)
	{
		return string.Format("{0}{1}{2}", "{" + color + "}", value, "{-}");
	}

	public string GetFormatedRolename(string value, int roleID)
	{
		GTextData gtextData = new GTextData();
		gtextData.startIndex = 0;
		gtextData.endIndex = 0;
		gtextData.RoleID = roleID;
		gtextData.type = 0;
		gtextData.key = string.Format("{0}_{1}{2}", gtextData.type, gtextData.startIndex, gtextData.endIndex);
		if (this.SpecialTextDict == null)
		{
			this.SpecialTextDict = new Dictionary<string, GTextData>();
		}
		if (!this.SpecialTextDict.ContainsKey(gtextData.key))
		{
			this.SpecialTextDict.Add(gtextData.key, gtextData);
		}
		return string.Format("{0}{1}{2}{3}{4}", new object[]
		{
			"｛RoleID = " + roleID + "｝",
			"{ffffff}",
			value,
			"{-}",
			"｛-｝"
		});
	}

	private string GetFormatedGoodsText(string goods, string msg, int offset, int roleID)
	{
		string[] array = goods.Split(StringMark.CHAT_SUB_MARK1);
		if (array.Length > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (string text in array)
			{
				string[] array3 = text.Split(StringMark.CHAT_SUB_MARK2);
				GTextData gtextData = new GTextData();
				gtextData.startIndex = Convert.ToInt32(array3[0]) + offset;
				gtextData.endIndex = Convert.ToInt32(array3[1]) + offset;
				gtextData.goodsId = Convert.ToInt32(array3[2]);
				gtextData.RoleID = roleID;
				gtextData.type = 2;
				gtextData.Quality = Convert.ToInt32(array3[3]);
				gtextData.key = string.Format("{0}_{1}{2}", gtextData.type, gtextData.startIndex, gtextData.endIndex);
				if (gtextData.endIndex <= base.Text.Length && gtextData.startIndex >= 0)
				{
					string text2 = base.Text.Substring(gtextData.startIndex, gtextData.endIndex - gtextData.startIndex);
					string[] array4 = text2.Split(new char[]
					{
						'+'
					});
					int num2 = ConfigGoods.FindGoodsIDByName(array4[0]);
					if (num2 >= 0)
					{
						gtextData.color = Global.ParseStringColor(Global.GetGoodsColorString(num2)).Color;
						if (!this.SpecialTextDict.ContainsKey(gtextData.key))
						{
							this.SpecialTextDict.Add(gtextData.key, gtextData);
						}
						this.FormatGoodsStrAndPreStr(stringBuilder, msg, gtextData, ref num);
					}
				}
			}
			return stringBuilder.ToString();
		}
		return msg;
	}

	private void FormatGoodsStrAndPreStr(StringBuilder strFinal, string strMsg, GTextData data, ref int index)
	{
		string text = strMsg.Substring(index, data.startIndex - index - 1);
		strFinal.Append(text);
		text = strMsg.Substring(data.startIndex, data.endIndex);
		strFinal.Append(string.Format("{0}{1}{2}{3}{4}", new object[]
		{
			"｛" + data.key + "｝",
			"{" + data.color.ToString() + "}",
			text,
			"{-}",
			"｛-｝"
		}));
		index = data.endIndex + 1;
	}

	public void RenderText()
	{
	}

	private bool _BlackOutLine;

	private List<GTextData> IndexArray = new List<GTextData>();

	private Dictionary<string, GTextData> SpecialTextDict = new Dictionary<string, GTextData>();
}
