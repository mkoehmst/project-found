using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;

using ProjectFound.Environment.Characters;

public class CheckTargetHealth : ConditionTask
{
	[BlackboardOnly]
	public BBParameter<Combatant> target;
	public CompareMethod checkType = CompareMethod.EqualTo;
	public BBParameter<float> valueB;

	[SliderField(0,0.1f)]
	public float differenceThreshold = 0.05f;

	protected override string info
	{
		get
		{
			return target + ".Health" + OperationTools.GetCompareString( checkType ) + valueB;
		}
	}

	protected override bool OnCheck( )
	{
		float health = target.value.Health;

		return OperationTools.Compare( health, (float)valueB.value, checkType, differenceThreshold );
	}
}
