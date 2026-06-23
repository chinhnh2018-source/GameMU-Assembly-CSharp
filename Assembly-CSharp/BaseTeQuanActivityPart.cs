using System;
using Server.Data;

public interface BaseTeQuanActivityPart
{
	int ID { get; set; }

	void RefreshPart(SpecPriorityActInfo inf);
}
