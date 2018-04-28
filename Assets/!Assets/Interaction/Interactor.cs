using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectFound.Environment {

	using ProjectFound.Environment.Handlers;
	using ProjectFound.Environment.Props;

	public abstract class Interactor : Interactee
	{
		protected bool m_isBusy = false;
		public bool IsBusy { get { return m_isBusy; } set { m_isBusy = value; } }

		protected Interactee m_targetInteractee;
		public Interactee TargetInteractee { get { return m_targetInteractee; } }
		public LayerID TargetLayerID { get { return m_targetInteractee.LayerID; } }
		public bool HasTarget { get { return m_targetInteractee != null; } }

		public Dictionary<InteracteeHandler, bool> HandlerExecutionDictionary { get; }
			= new Dictionary<InteracteeHandler, bool>( );

		public SelectionSpec SelectionSpec { get; } = new SelectionSpec( );
		public UsageSpec UsageSpec { get; } = new UsageSpec( );

		public void NullifySelection( )
		{
			m_targetInteractee = null;
		}

		public bool CheckExecutionState( InteracteeHandler handler )
		{
			if ( HandlerExecutionDictionary.ContainsKey( handler ) )
			{
				return HandlerExecutionDictionary[handler];
			}
			else
			{
				return false;
			}
		}
	}


}
