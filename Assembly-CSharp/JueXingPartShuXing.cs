using System;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using XMLCreater;

public class JueXingPartShuXing : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("累计加成属性");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		float enlargeRate = 1f;
		MUAwakenLevelDetail levelsByOrderAndstar = JueXingData.GetLevelsByOrderAndstar(JueXingData.GetJieShu(), JueXingData.GetXingShu());
		if (levelsByOrderAndstar != null)
		{
			enlargeRate = (100f + levelsByOrderAndstar.EnlargeRate) / 100f;
		}
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		List<MUAwakenActivationDetail> selfTaoZhuangJiHuoList = JueXingData.GetSelfTaoZhuangJiHuoList();
		for (int i = 0; i < selfTaoZhuangJiHuoList.Count; i++)
		{
			MUAwakenActivationDetail muawakenActivationDetail = selfTaoZhuangJiHuoList[i];
			if (muawakenActivationDetail == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"不存在激活石Id为：" + selfTaoZhuangJiHuoList[i] + Global.GetLang(" 的激活石信息")
				});
			}
			else
			{
				for (int j = 0; j < muawakenActivationDetail.BaseProps.Count; j++)
				{
					string propName = muawakenActivationDetail.BaseProps[j].PropName;
					float propNum = muawakenActivationDetail.BaseProps[j].PropNum;
					if (!dictionary.ContainsKey(propName))
					{
						dictionary[propName] = propNum;
					}
					else
					{
						dictionary[propName] += propNum;
					}
				}
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		List<MUPropInfo> list = new List<MUPropInfo>();
		foreach (KeyValuePair<string, float> keyValuePair in dictionary)
		{
			string key = keyValuePair.Key;
			Dictionary<string, float>.Enumerator enumerator;
			KeyValuePair<string, float> keyValuePair2 = enumerator.Current;
			MUPropInfo mupropInfo = new MUPropInfo(key, keyValuePair2.Value);
			list.Add(mupropInfo);
		}
		Dictionary<string, string> shuXingString = JueXingData.GetShuXingString(list, enlargeRate);
		Dictionary<string, string>.Enumerator enumerator2 = shuXingString.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			StringBuilder stringBuilder3 = stringBuilder;
			string text = "{0}\n\r";
			KeyValuePair<string, string> keyValuePair3 = enumerator2.Current;
			stringBuilder3.AppendFormat(text, keyValuePair3.Key);
			StringBuilder stringBuilder4 = stringBuilder2;
			string text2 = ":  {0}\n\r";
			KeyValuePair<string, string> keyValuePair4 = enumerator2.Current;
			stringBuilder4.AppendFormat(text2, keyValuePair4.Value);
		}
		this.lblName.text = stringBuilder.ToString();
		this.lblValue.text = stringBuilder2.ToString();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnClose;

	public UILabel lblTitle;

	public TextBlock lblName;

	public TextBlock lblValue;
}
