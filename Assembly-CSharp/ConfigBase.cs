using System;

public interface ConfigBase
{
	ClearType XmlClearType { get; set; }

	void ClearXMLData(byte clearType);

	void DisposeInstance();
}
