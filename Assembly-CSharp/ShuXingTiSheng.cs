using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShuXingTiSheng : MonoBehaviour
{
	private void Awake()
	{
		this.Max.text = string.Empty;
		this.AddValue.text = string.Empty;
		this.WCD.text = string.Empty;
		this.JianTou.gameObject.SetActive(false);
	}

	public void setAddPro(string max, int addValue, int jiant, int wcd, string str)
	{
		if (addValue <= 0)
		{
			this.AddValue.text = string.Empty;
		}
		else
		{
			this.AddValue.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(addValue.ToString(), new object[0])
			});
		}
		if (jiant == 0)
		{
			this.JianTou.spriteName = this.up;
		}
		else if (jiant == -1)
		{
			this.JianTou.gameObject.SetActive(false);
		}
		else
		{
			this.JianTou.spriteName = this.down;
		}
		if (wcd < 100)
		{
			this.JianTou.gameObject.SetActive(true);
			this.Max.text = Global.GetColorStringForNGUIText(new object[]
			{
				"e8cda5",
				string.Format(str, max)
			});
			this.WCD.text = Global.GetColorStringForNGUIText(new object[]
			{
				"17e43e",
				string.Format(this.strWcd, wcd)
			});
			return;
		}
		this.Max.text = Global.GetColorStringForNGUIText(new object[]
		{
			"e8cda5",
			string.Format(str, max)
		});
		this.WCD.text = Global.GetColorStringForNGUIText(new object[]
		{
			"ef09ef",
			string.Format(this.strWcd, wcd)
		});
		this.JianTou.gameObject.SetActive(false);
		this.AddValue.text = string.Empty;
	}

	public UILabel Max;

	public UILabel AddValue;

	public UISprite JianTou;

	public UILabel WCD;

	private string strWcd = Global.GetLang("(达成:{0}%)");

	private string up = "jiantou";

	private string down = "jiantou";
}
