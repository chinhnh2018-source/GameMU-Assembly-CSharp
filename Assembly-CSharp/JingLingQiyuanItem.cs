using System;
using UnityEngine;

public class JingLingQiyuanItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public void RefreshIcon()
	{
	}

	public void SetText(string str, bool IsTongGuo)
	{
		if (!string.IsNullOrEmpty(str))
		{
			string[] array = str.Split(new char[]
			{
				';'
			});
			if (array != null)
			{
				int num = array.Length;
				int i = 0;
				int num2 = this.ContentZuhe.Length;
				while (i < num2)
				{
					if (array[i] != string.Empty)
					{
						this.ContentZuhe[i].text = array[i];
						if (!IsTongGuo)
						{
							this.ContentZuhe[i].textColor = (uint)NGUIMath.ColorToIntEx(Color.gray);
						}
					}
					else
					{
						this.BgArr[i].gameObject.SetActive(false);
					}
					i++;
				}
			}
		}
		else
		{
			for (int j = 0; j < 3; j++)
			{
				this.BgArr[j].gameObject.SetActive(false);
			}
		}
	}

	public Transform[] ZuHeItemArr;

	public GGoodIcon[] ImageArr;

	public Transform[] BgArr;

	public TextBlock[] ContentZuhe;
}
