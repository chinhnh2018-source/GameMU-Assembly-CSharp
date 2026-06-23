using System;
using HSGameEngine.GameEngine.Logic;
using Tmsk.Xml;

public class ShengYinTaoZhuangVO
{
	public ShengYinTaoZhuangVO()
	{
	}

	public ShengYinTaoZhuangVO(XElement xe)
	{
		this.ID = xe.GetAttributeInt("ID", -1);
		this.Name = xe.GetAttributeStr("Name");
		this.LeiXing = xe.GetAttributeInt("LeiXing", -1);
		this.TaoZhuangShuXingTwo = xe.GetAttributeStr("TaoZhuangShuXingTwo");
		this.TaoZhuangShuXingFour = xe.GetAttributeStr("TaoZhuangShuXingFour");
		this.TaoZhuangShuXingSix = xe.GetAttributeStr("TaoZhuangShuXingSix");
	}

	public int ID { get; set; }

	public string Name { get; set; }

	public int LeiXing { get; set; }

	public string TaoZhuangShuXingTwo { get; set; }

	public string TaoZhuangStr2
	{
		get
		{
			if (string.IsNullOrEmpty(this.mTaoZhuangStr2) && !string.IsNullOrEmpty(this.TaoZhuangShuXingTwo))
			{
				string[] array = this.TaoZhuangShuXingTwo.Split(new char[]
				{
					'|'
				});
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length == 2)
						{
							if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
							{
								string text = this.mTaoZhuangStr2;
								this.mTaoZhuangStr2 = string.Concat(new string[]
								{
									text,
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], true),
									Global.GetLang("："),
									(double.Parse(array2[1]) * 100.0).ToString("f1"),
									"%"
								});
							}
							else
							{
								this.mTaoZhuangStr2 = this.mTaoZhuangStr2 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], true) + Global.GetLang("：") + array2[1];
							}
							this.mTaoZhuangStr2 += "\n";
						}
					}
				}
			}
			return this.mTaoZhuangStr2;
		}
	}

	public string TaoZhuangShuXingFour { get; set; }

	public string TaoZhuangStr4
	{
		get
		{
			if (string.IsNullOrEmpty(this.mTaoZhuangStr4) && !string.IsNullOrEmpty(this.TaoZhuangShuXingFour))
			{
				string[] array = this.TaoZhuangShuXingFour.Split(new char[]
				{
					'|'
				});
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length == 2)
						{
							if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
							{
								string text = this.mTaoZhuangStr4;
								this.mTaoZhuangStr4 = string.Concat(new string[]
								{
									text,
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], true),
									Global.GetLang("："),
									(double.Parse(array2[1]) * 100.0).ToString("f1"),
									"%"
								});
							}
							else
							{
								this.mTaoZhuangStr4 = this.mTaoZhuangStr4 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], true) + Global.GetLang("：") + array2[1];
							}
							this.mTaoZhuangStr4 += "\n";
						}
					}
				}
			}
			return this.mTaoZhuangStr4;
		}
	}

	public string TaoZhuangShuXingSix { get; set; }

	public string TaoZhuangStr6
	{
		get
		{
			if (string.IsNullOrEmpty(this.mTaoZhuangStr6) && !string.IsNullOrEmpty(this.TaoZhuangShuXingSix))
			{
				string[] array = this.TaoZhuangShuXingSix.Split(new char[]
				{
					'|'
				});
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (array2.Length == 2)
						{
							if (ConfigExtPropIndexes.GetPercentByWord(array2[0]))
							{
								string text = this.mTaoZhuangStr6;
								this.mTaoZhuangStr6 = string.Concat(new string[]
								{
									text,
									ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], true),
									Global.GetLang("："),
									(double.Parse(array2[1]) * 100.0).ToString("f1"),
									"%"
								});
							}
							else
							{
								this.mTaoZhuangStr6 = this.mTaoZhuangStr6 + ConfigExtPropIndexes.GetExtPropIndexesDescriptionByWord(array2[0], true) + Global.GetLang("：") + array2[1];
							}
							this.mTaoZhuangStr6 += "\n";
						}
					}
				}
			}
			return this.mTaoZhuangStr6;
		}
	}

	private string mTaoZhuangStr2 = string.Empty;

	private string mTaoZhuangStr4 = string.Empty;

	private string mTaoZhuangStr6 = string.Empty;
}
