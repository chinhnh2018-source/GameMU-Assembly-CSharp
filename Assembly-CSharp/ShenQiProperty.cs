using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShenQiProperty : UserControl
{
	protected override void InitializeComponent()
	{
		this.Obj_UpArrows.transform.localPosition = new Vector3(45f, 0f, 0f);
		this.m_UpValueList.Pivot = 0;
		this.m_UpValueList.X = 130.0;
		this.m_ValueList.Pivot = 0;
		this.m_ValueList.X = -180.0;
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

	public void IsShow(ShenQiPropertyType type, string titleName, int count = 0, string content = null)
	{
		switch (type)
		{
		case ShenQiPropertyType.ShenQi:
			this.ShenQiPropertys(titleName, count, content);
			break;
		case ShenQiPropertyType.ShenXiang:
			this.ShenXiangProperty(titleName, count, content);
			break;
		case ShenQiPropertyType.ShenXiangSum:
			this.ShenXiangSumProperty(titleName, count, content);
			break;
		case ShenQiPropertyType.RenXing:
			this.RenXingProperty(titleName, count, content);
			break;
		}
	}

	private void ShenQiPropertys(string titleName, int count, string content)
	{
		this.IsShowArrow(0);
		this.m_RenXingTitle.gameObject.SetActive(false);
		this.m_UpValueList.gameObject.SetActive(false);
		ShenQiXMLData shenQiDataByID = ShenQiManager.GetShenQiDataByID(count);
		this.m_Title.Text = Global.GetLang(shenQiDataByID.Name);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("生  命  值     ：")
		}));
		stringBuilder.Append("        ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.LifeV
		}));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("攻  击  力     ：")
		}));
		stringBuilder.Append("        ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.AddAttack
		}));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("防  御  力     ：")
		}));
		stringBuilder.Append("        ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.AddDefense
		}));
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"e3b36c",
			Global.GetLang("韧        性     ：")
		}));
		stringBuilder.Append("        ");
		stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
		{
			"dac7ae",
			shenQiDataByID.Toughness
		}));
		this.m_ValueList.Text = stringBuilder.ToString();
	}

	private void ShenXiangProperty(string titleName, int count, string content)
	{
		this.IsShowArrow(0);
		this.m_RenXingTitle.gameObject.SetActive(false);
		this.m_UpValueList.gameObject.SetActive(false);
		this.m_Title.Text = Global.GetLang(titleName);
		this.ShowContent(content);
	}

	private void ShenXiangSumProperty(string titleName, int count, string content)
	{
		this.IsShowArrow(0);
		this.m_RenXingTitle.gameObject.SetActive(false);
		this.m_UpValueList.gameObject.SetActive(false);
		this.m_Title.Text = Global.GetLang(titleName);
		this.m_ValueList.Text = string.Format(Global.GetLang("{0}"), content);
	}

	private void RenXingProperty(string titleName, int count, string content)
	{
		this.IsShowArrow(10);
		this.m_RenXingTitle.gameObject.SetActive(true);
		this.m_UpValueList.gameObject.SetActive(true);
		this.m_Title.Text = Global.GetLang(titleName);
		List<ShenQiPropertyData> shenQiPropertyDataList = ShenQiPropertyManager.GetShenQiPropertyDataList();
		int count2 = shenQiPropertyDataList.Count;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		for (int i = count2 - 1; i >= 0; i--)
		{
			if (count >= shenQiPropertyDataList[i].Toughness)
			{
				num3++;
				num = i;
				num2 = num + 1;
				break;
			}
		}
		if (num3 <= 0)
		{
			num2 = 0;
		}
		if (num2 >= shenQiPropertyDataList.Count)
		{
			num2--;
		}
		int toughness = shenQiPropertyDataList[num2].Toughness;
		this.m_RenXingTitle.Text = string.Format("{0}{1}{2}{3}{4}", new object[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Global.GetLang("当前韧性效果：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				count
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				"/"
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				toughness
			}),
			"\n"
		});
		if (count >= toughness)
		{
			flag = true;
		}
		string[] array = new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵抗幸运一击：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵抗卓越一击：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵抗双倍一击：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵抗野蛮一击：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵抗冷血一击：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵抗无情一击：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵  抗  冰  冻：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵  抗  麻  痹：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵  抗  减  速：")
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"e3b36c",
				Global.GetLang("抵  抗  重  击：")
			})
		};
		ShenQiPropertyData shenQiPropertyDataByID = ShenQiPropertyManager.GetShenQiPropertyDataByID((num3 > 0) ? (num + 1) : 1);
		ShenQiPropertyData shenQiPropertyDataByID2 = ShenQiPropertyManager.GetShenQiPropertyDataByID((num2 + 1 < count2) ? (num2 + 1) : count2);
		int num4 = 0;
		int num5 = 0;
		int num6 = (num3 > 0) ? (num + 1) : 0;
		string[] array2 = new string[]
		{
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeLucky : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeFatalAttack : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeDoubleAttack : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeSavagePercent : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeColdPercent : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeRuthlessPercent : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeFrozenPercent : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DePalsyPercent : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeSpeedDownPercent : 0f)
			}),
			Global.GetColorStringForNGUIText(new object[]
			{
				"dac7ae",
				string.Format("{0}{1:0%}", "+", (num6 != num4) ? shenQiPropertyDataByID.DeBlowPercent : 0f)
			})
		};
		string[] array3 = new string[]
		{
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeLucky - shenQiPropertyDataByID.DeLucky) : shenQiPropertyDataByID2.DeLucky),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeFatalAttack - shenQiPropertyDataByID.DeFatalAttack) : shenQiPropertyDataByID2.DeFatalAttack),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeDoubleAttack - shenQiPropertyDataByID.DeDoubleAttack) : shenQiPropertyDataByID2.DeDoubleAttack),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeSavagePercent - shenQiPropertyDataByID.DeSavagePercent) : shenQiPropertyDataByID2.DeSavagePercent),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeColdPercent - shenQiPropertyDataByID.DeColdPercent) : shenQiPropertyDataByID2.DeColdPercent),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeRuthlessPercent - shenQiPropertyDataByID.DeRuthlessPercent) : shenQiPropertyDataByID2.DeRuthlessPercent),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeFrozenPercent - shenQiPropertyDataByID.DeFrozenPercent) : shenQiPropertyDataByID2.DeFrozenPercent),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DePalsyPercent - shenQiPropertyDataByID.DePalsyPercent) : shenQiPropertyDataByID2.DePalsyPercent),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeSpeedDownPercent - shenQiPropertyDataByID.DeSpeedDownPercent) : shenQiPropertyDataByID2.DeSpeedDownPercent),
			string.Format("{0}{1:0%}", "+", (num6 != num5) ? (shenQiPropertyDataByID2.DeBlowPercent - shenQiPropertyDataByID.DeBlowPercent) : shenQiPropertyDataByID2.DeBlowPercent)
		};
		StringBuilder stringBuilder = new StringBuilder();
		for (int j = 0; j < 10; j++)
		{
			stringBuilder.Append(array[j]);
			stringBuilder.Append(array2[j]);
			stringBuilder.Append("\n");
		}
		this.m_ValueList.Text = stringBuilder.ToString();
		if (!flag)
		{
			if (!this.m_UpValueList.gameObject.activeSelf)
			{
				this.m_UpValueList.gameObject.SetActive(true);
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int k = 0; k < 10; k++)
			{
				stringBuilder2.Append(array3[k]);
				stringBuilder2.Append("\n");
			}
			this.m_UpValueList.Text = stringBuilder2.ToString();
		}
		else
		{
			this.m_UpValueList.gameObject.SetActive(false);
			for (int l = 0; l < this.m_UpArrowList.Length; l++)
			{
				this.m_UpArrowList[l].gameObject.SetActive(false);
			}
		}
	}

	private void ShowContent(string content)
	{
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
					string.Format(Global.GetLang("{0}"), extPropIndexesDescriptionByWord)
				}));
				stringBuilder.Append(Global.GetColorStringForNGUIText(new object[]
				{
					"e3b36c",
					Global.GetLang("  ：      ")
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

	private void IsShowArrow(int count = 0)
	{
		if (count <= 0)
		{
			for (int i = 0; i < this.m_UpArrowList.Length; i++)
			{
				this.m_UpArrowList[i].gameObject.SetActive(false);
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				this.m_UpArrowList[j].gameObject.SetActive(true);
			}
		}
	}

	protected override void OnDestroy()
	{
		this.m_CloseBtn = null;
		this.CloseCallback = null;
		this.m_Title = null;
		this.m_split = null;
		this.m_RenXingTitle = null;
		this.m_ValueList = null;
		this.m_UpArrowList = null;
		this.m_UpValueList = null;
		ShenQiPropertyManager.Clear();
	}

	public DPSelectedItemEventHandler CloseCallback;

	public GButton m_CloseBtn;

	public TextBlock m_Title;

	public UISprite m_split;

	public TextBlock m_RenXingTitle;

	public TextBlock m_ValueList;

	public UISprite[] m_UpArrowList;

	public TextBlock m_UpValueList;

	public GameObject Obj_UpArrows;

	[StructLayout(0, Size = 1)]
	private struct RenXingData
	{
		public int ID { get; set; }

		public int Value { get; set; }
	}
}
