using System;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenQiSingleProperty : UserControl
{
	protected override void InitializeComponent()
	{
		this.m_CloseBtn.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			if (this.CloseCallback != null)
			{
				this.CloseCallback(null, new DPSelectedItemEventArgs
				{
					ID = 0
				});
			}
		};
	}

	public void Show(int type, string titleName, int count, string content)
	{
		if (type == 1)
		{
			this.m_ValueList.transform.parent.localPosition = new Vector3(0f, -53.2f, 0f);
			this.ShowShenQi(titleName, count, content);
		}
		else if (type == 2)
		{
			this.m_ValueList.transform.parent.localPosition = new Vector3(-26f, -53.2f, 0f);
			this.ShowShenXiang(titleName, count, content);
		}
	}

	private void ShowShenXiang(string titleName, int count, string content)
	{
		this.m_Title.Text = Global.GetLang(titleName);
		StringBuilder stringBuilder = new StringBuilder();
		if (content != null)
		{
			string[] array = content.Split(new char[]
			{
				'|'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				float num = float.Parse(array2[1]);
				string extPropIndexesDescriptionByWord = ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], false);
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					extPropIndexesDescriptionByWord
				}));
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang(" ： ")
				}));
				string text = string.Format("{0:0%}", num);
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"dac7ae",
					"+" + text
				}));
				stringBuilder.Append('\n');
			}
		}
		this.m_ValueList.Text = stringBuilder.ToString();
	}

	private void ShowShenQi(string titleName, int count, string content)
	{
		ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(count);
		this.m_Title.Text = Global.GetLang(shenQiDataByID.Name);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("生  命  值 ：")
		}));
		stringBuilder.Append("    ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.LifeV
		}));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("攻  击  力 ：")
		}));
		stringBuilder.Append("    ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.AddAttack
		}));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("防  御  力 ：")
		}));
		stringBuilder.Append("    ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.AddDefense
		}));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("韧        性 ：")
		}));
		stringBuilder.Append("    ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.Toughness
		}));
		this.m_ValueList.Text = stringBuilder.ToString();
	}

	protected override void OnDestroy()
	{
		this.CloseCallback = null;
		this.m_CloseBtn = null;
		this.m_Title = null;
		this.m_split = null;
		this.m_ValueList = null;
	}

	public DPSelectedItemEventHandler CloseCallback;

	public GButton m_CloseBtn;

	public TextBlock m_Title;

	public UISprite m_split;

	public TextBlock m_ValueList;
}
