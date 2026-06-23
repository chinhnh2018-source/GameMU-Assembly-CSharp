using System;

public class StrengtheningBaseLevelAttribute
{
	public StrengtheningBaseLevelAttribute(bool isNumber, double value)
	{
		this.isNumber = isNumber;
		this.value = value;
	}

	public bool isNumber;

	public double value;
}
