namespace ProjectFound.Environment.Handlers
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public abstract class HandlerChain : ScriptableObject
	{
		protected List<InteracteeHandler> m_asyncHandlers = new List<InteracteeHandler>( );

		public abstract IEnumerator ExecuteChain( Interactee ie, Interactor ir );

		protected virtual void BeginChain( Interactor ir )
		{
			ir.IsBusy = true;
		}

		protected virtual bool EndChain( Interactor ir )
		{
			for ( int i = 0; i < m_asyncHandlers.Count; ++i )
			{
				InteracteeHandler handler = m_asyncHandlers[i];

				if ( ir.HandlerExecutionDictionary.ContainsKey( handler )
					&& ir.HandlerExecutionDictionary[handler] == true )
				{
					return false;
				}
			}

			ir.IsBusy = false;
			return true;
		}
	}

}