using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class AdendaItemPropertyHelperController : MonoBehaviour
{
	private void Awake()
	{
		this.attributeField.text = string.Empty;
		this.additionField.text = string.Empty;
		this.progressPercent.text = string.Empty;
		this.upArrow.gameObject.SetActive(false);
	}

	public void SetAdditionProperty(string max, int addition, int flag, int percent, string str)
	{
		if (addition <= 0)
		{
			this.additionField.text = string.Empty;
		}
		else
		{
			this.additionField.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(addition.ToString(), new object[0])
			});
		}
		if (flag == 0)
		{
			this.upArrow.spriteName = this.up;
		}
		else if (flag == -1)
		{
			this.upArrow.gameObject.SetActive(false);
		}
		else
		{
			this.upArrow.spriteName = this.down;
		}
		if (percent < 100)
		{
			this.upArrow.gameObject.SetActive(true);
			this.attributeField.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e8cda5",
				string.Format(str, max)
			});
			this.progressPercent.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(Global.GetLang("(达成:{0}%)"), percent)
			});
			return;
		}
		this.attributeField.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e8cda5",
			string.Format(str, max)
		});
		this.progressPercent.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ef09ef",
			string.Format(Global.GetLang("(达成:{0}%)"), percent)
		});
		this.upArrow.gameObject.SetActive(false);
		this.additionField.text = string.Empty;
	}

	public UILabel attributeField;

	public UILabel additionField;

	public UISprite upArrow;

	public UILabel progressPercent;

	private string up = "up_arrow";

	private string down = "up_arrow";
}
