using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName=("Project Found/Game State Datum"))]
public class GameStateDatum : ScriptableObject
{
	static public NodeCanvas.Framework.IBlackboard _blackboard;
	static public NodeCanvas.StateMachines.FSMOwner _owner;

	protected const string m_tag = "State";

	public string m_blackboardName;

	void OnEnable( )
	{

	}

	public void SetValue<_T>( _T valueToSet )
	{
		if ( _blackboard == null )
		{
			GameObject obj = GameObject.FindGameObjectWithTag( m_tag );

			_blackboard = obj.GetComponent<NodeCanvas.Framework.IBlackboard>( );
			_owner = obj.GetComponent<NodeCanvas.StateMachines.FSMOwner>( );
		}

		_owner.SendEvent<_T>( m_blackboardName + "Event", valueToSet );

	}

	public void Toggle( )
	{
		var variable = CreateIfNeeded<bool>( );

		if ( variable.varType != typeof( bool ) )
		{
			return ;
		}

		bool currentValue = (bool)variable.value;
		variable.value = !currentValue;
	}

	protected NodeCanvas.Framework.Variable CreateIfNeeded<_T>( )
	{
		if ( _blackboard.variables.ContainsKey( m_blackboardName ) == false )
		{
			return _blackboard.AddVariable( m_blackboardName, typeof( _T ) );
		}
		else
		{
			return _blackboard.variables[m_blackboardName];
		}
	}
}
