using System;

public class ConfigShenShengHuJia : IConfigbase<ConfigShenShengHuJia>, ConfigBase
{
	public ConfigShenShengHuJia()
	{
		this.XmlClearType = ClearType.ClearOnChangeScene;
		ConfigManager.AddConfig(this);
	}

	public void DisposeInstance()
	{
		base.IDisposeInstance();
	}

	public void ClearXMLData(byte clearType)
	{
		this.ClearData();
	}

	public ShenshenghudunXing GetShenshenghudunXingInstance()
	{
		if (this.mShenshenghudunXing == null)
		{
			this.mShenshenghudunXing = new ShenshenghudunXing();
		}
		this.mShenshenghudunXing.AddRecommendCount();
		return this.mShenshenghudunXing;
	}

	public void SubXingRecommendCount()
	{
		if (this.mShenshenghudunXing != null && 0 >= this.mShenshenghudunXing.SubRecommendCount())
		{
			this.mShenshenghudunXing = null;
		}
	}

	public ShenshenghudunJie GetShenshenghudunJieInstance()
	{
		if (this.mShenshenghudunJie == null)
		{
			this.mShenshenghudunJie = new ShenshenghudunJie();
		}
		this.mShenshenghudunJie.AddRecommendCount();
		return this.mShenshenghudunJie;
	}

	public void SubJieRecommendCount()
	{
		if (this.mShenshenghudunJie != null && 0 >= this.mShenshenghudunJie.SubRecommendCount())
		{
			this.mShenshenghudunJie = null;
		}
	}

	public void ClearData()
	{
		if (this.mShenshenghudunJie != null)
		{
			this.mShenshenghudunJie.ClearXMLData();
		}
		if (this.mShenshenghudunXing != null)
		{
			this.mShenshenghudunXing.ClearXMLData();
		}
	}

	public ClearType XmlClearType { get; set; }

	private ShenshenghudunXing mShenshenghudunXing;

	private ShenshenghudunJie mShenshenghudunJie;
}
