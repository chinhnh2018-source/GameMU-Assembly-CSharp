using System;
using UnityEngine;

namespace HSGameEngine.GameFramework.Logic
{
	public interface ISystemHelpPart
	{
		Bounds GetBounds(int id);

		void PreWizardExec();

		object DoAction(int id, int param);

		int GetPartID();
	}
}
