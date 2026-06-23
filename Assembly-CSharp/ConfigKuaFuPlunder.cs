using System;

public class ConfigKuaFuPlunder : IConfigbase<ConfigKuaFuPlunder>, ConfigBase
{
	public ConfigKuaFuPlunder()
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

	public CrusadeWarXml GetCrusadeWarXmlInstance()
	{
		if (this.mCrusadeWarXml == null)
		{
			this.mCrusadeWarXml = new CrusadeWarXml();
		}
		this.mCrusadeWarXml.RecommendCount++;
		return this.mCrusadeWarXml;
	}

	public int GetCrusadeWarXmlRecommendCount()
	{
		if (this.mCrusadeWarXml != null)
		{
			return this.mCrusadeWarXml.RecommendCount;
		}
		return 0;
	}

	public void DisposeCrusadeWarXml()
	{
		if (this.mCrusadeWarXml != null)
		{
			this.mCrusadeWarXml.RecommendCount--;
			if (0 >= this.mCrusadeWarXml.RecommendCount)
			{
				this.mCrusadeWarXml.ClearData();
				this.mCrusadeWarXml = null;
			}
		}
	}

	public CrusadeWorldXML GetCrusadeWorldXMLInstance()
	{
		if (this.mCrusadeWorldXML == null)
		{
			this.mCrusadeWorldXML = new CrusadeWorldXML();
		}
		this.mCrusadeWorldXML.RecommendCount++;
		return this.mCrusadeWorldXML;
	}

	public int GetCrusadeWorldRecommendCount()
	{
		if (this.mCrusadeWorldXML != null)
		{
			return this.mCrusadeWorldXML.RecommendCount;
		}
		return 0;
	}

	public void DisposadeCrusadeWorldXml()
	{
		if (this.mCrusadeWorldXML != null)
		{
			this.mCrusadeWorldXML.RecommendCount--;
			if (0 >= this.mCrusadeWorldXML.RecommendCount)
			{
				this.mCrusadeWorldXML.ClearData();
				this.mCrusadeWorldXML = null;
			}
		}
	}

	public CrusadeStoreXml GetCrusadeStoreXmlInstance()
	{
		if (this.mCrusadeStoreXml == null)
		{
			this.mCrusadeStoreXml = new CrusadeStoreXml();
		}
		this.mCrusadeStoreXml.RecommendCount++;
		return this.mCrusadeStoreXml;
	}

	public int GetCrusadeStoreXmlRecommendCount()
	{
		if (this.mCrusadeStoreXml != null)
		{
			return this.mCrusadeStoreXml.RecommendCount;
		}
		return 0;
	}

	public void DisposadeCrusadeStoreXml()
	{
		if (this.mCrusadeStoreXml != null)
		{
			this.mCrusadeStoreXml.RecommendCount--;
			if (0 >= this.mCrusadeStoreXml.RecommendCount)
			{
				this.mCrusadeStoreXml.ClearData();
				this.mCrusadeStoreXml = null;
			}
		}
	}

	public void ClearData()
	{
		if (this.mCrusadeWarXml != null)
		{
			this.mCrusadeWarXml.ClearData();
		}
		if (this.mCrusadeWorldXML != null)
		{
			this.mCrusadeWorldXML.ClearData();
		}
		if (this.mCrusadeStoreXml != null)
		{
			this.mCrusadeStoreXml.ClearData();
		}
	}

	public ClearType XmlClearType { get; set; }

	private CrusadeWarXml mCrusadeWarXml;

	private CrusadeWorldXML mCrusadeWorldXML;

	private CrusadeStoreXml mCrusadeStoreXml;
}
