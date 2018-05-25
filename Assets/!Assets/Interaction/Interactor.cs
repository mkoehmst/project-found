namespace ProjectFound.Environment
{

	using System.Collections.Generic;

	using UnityEngine;

	using ProjectFound.Environment.Handlers;

	public abstract class Interactor : Interactee
	{
		private MEC.CoroutineHandle m_oneShotHandle;
		//public Interactee OneShotInteractee { get; private set; }

		private MEC.CoroutineHandle m_oneShotReleaseHandle;
		//public Interactee OneShotReleaseInteractee { get; private set; }

		private MEC.CoroutineHandle m_windowHandle;
		//public Interactee WindowInteractee { get; private set; }

		private MEC.CoroutineHandle m_windowReleaseHandle;
		//public Interactee WindowReleaseInteractee { get; private set; }

		private MEC.CoroutineHandle m_holdingHandle;
		//public Interactee HoldingInteractee { get; private set; }

		private MEC.CoroutineHandle m_holdingReleaseHandle;
		//public Interactee HoldingReleaseInteractee { get; private set; }

		private MEC.CoroutineHandle m_focusHandle;
		//public Interactee FocusInteractee { get; private set; }

		private MEC.CoroutineHandle m_focusReleaseHandle;
		//public Interactee FocusReleaseInteractee { get; private set; }

		private MEC.CoroutineHandle m_usageHandle;

		private MEC.CoroutineHandle m_abortHandle;

		protected bool m_isBusy = false;
		public bool IsBusy
		{
			get { return m_isBusy; }
			set { m_isBusy = value; }
		}

		protected Interactee m_targetInteractee;
		public Interactee TargetInteractee { get { return m_targetInteractee; } }
		public LayerID TargetLayerID { get { return m_targetInteractee.LayerID; } }
		public bool HasTarget { get { return m_targetInteractee != null; } }

		public void RunOneShotChain( Interactee ie )
		{ 
			HandlerChain chain = ie.OneShotChain;

			if ( chain != null )
			{ 
				RunChain( ie, chain, ref m_oneShotHandle, HandlerChain.ChainType.Handler ); 
			}
		}

		public void RunOneShotReleaseChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_oneShotReleaseHandle, HandlerChain.ChainType.Handler ); 
		}

		public void RunWindowChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_windowHandle, HandlerChain.ChainType.Window ); 
		}

		public void RunWindowReleaseChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_windowReleaseHandle, HandlerChain.ChainType.WindowRelease ); 
		}

		public void RunHoldingChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_holdingHandle, HandlerChain.ChainType.Handler ); 
		}

		public void RunHoldingReleaseChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_holdingReleaseHandle, 
				HandlerChain.ChainType.HandlerRelease ); 
		}

		public void RunFocusChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_focusHandle, HandlerChain.ChainType.Handler ); 
		}

		public void RunFocusReleaseChain( Interactee ie, HandlerChain chain )
		{ 
			RunChain( ie, chain, ref m_focusReleaseHandle, HandlerChain.ChainType.HandlerRelease ); 
		}

		public void RunUsageChain( Interactee ie, HandlerChain chain )
		{
			RunChain( ie, chain, ref m_usageHandle, HandlerChain.ChainType.Handler );
		}

		public void KillOneShotChain()
		{
			if ( m_oneShotHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillOneShotChain()" );
				KillChain( ref m_oneShotHandle );
			}
		}

		public void KillOneShotReleaseChain( )
		{
			if ( m_oneShotReleaseHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillOneShotReleaseChain()" );
				KillChain( ref m_oneShotReleaseHandle );
			}
		}

		public void KillWindowChain()
		{
			if ( m_windowHandle.IsValid ) 
			{ 
				//Debug.Log( "Interactor.KillWindowChain()" );
				KillChain( ref m_windowHandle );
			}
		}

		public void KillWindowReleaseChain( )
		{
			if ( m_windowReleaseHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillWindowReleaseChain()" );
				KillChain( ref m_windowReleaseHandle );
			}
		}

		public void KillHoldingChain()
		{
			if ( m_holdingHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillHoldigChain()" );
				KillChain( ref m_holdingHandle );
			}
		}

		public void KillHoldingReleaseChain( )
		{
			if ( m_holdingReleaseHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillHoldingReleaseChain()" );
				KillChain( ref m_holdingReleaseHandle );
			}
		}

		public void KillFocusChain()
		{
			if ( m_focusHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillFocusChain()" );
				KillChain( ref m_focusHandle );
			}
		}

		public void KillFocusReleaseChain( )
		{
			if ( m_focusReleaseHandle.IsValid )
			{ 
				//Debug.Log( "Interactor.KillFocusReleaseChain()" );
				KillChain( ref m_focusReleaseHandle );
			}
		}

		public void KillUsageChain( )
		{
			if ( m_usageHandle.IsValid )
			{
				KillChain( ref m_usageHandle );
			}
		}

		public void AbortOneShotChain( Interactee ie, HandlerChain chain )
		{
			if ( m_oneShotHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortOneShotChain()" );
				KillChain( ref m_oneShotHandle );
				RunChain( ie, chain, ref m_abortHandle, HandlerChain.ChainType.HandlerAbort );
			}
		}

		public void AbortWindowChain( Interactee ie, HandlerChain chain )
		{
			if ( m_windowHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortWindowChain()" );
				KillChain( ref m_windowHandle );
				RunChain( ie, chain, ref m_abortHandle, HandlerChain.ChainType.WindowAbort );
			}
		}

		public void AbortHoldingChain( Interactee ie, HandlerChain chain )
		{
			if ( m_holdingHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortHoldingChain()" );
				KillChain( ref m_holdingHandle );
				RunChain( ie, chain, ref m_abortHandle, HandlerChain.ChainType.HandlerAbort );
			}
		}

		public void AbortFocusChain( Interactee ie, HandlerChain chain )
		{
			if ( m_focusHandle.IsValid )
			{
				//Debug.Log( "Interactor.AbortFocusChain" );
				KillChain( ref m_focusHandle );
				RunChain( ie, chain, ref m_abortHandle, HandlerChain.ChainType.HandlerAbort );
			}
		}

		public void AbortUsageChain( Interactee ie, HandlerChain chain )
		{
			if ( m_usageHandle.IsValid )
			{
				KillChain( ref m_usageHandle );
				RunChain( ie, chain, ref m_abortHandle, HandlerChain.ChainType.HandlerAbort );
			}
		}

		private void RunChain( Interactee ie, HandlerChain chain, ref MEC.CoroutineHandle handle,
			HandlerChain.ChainType chainType )
		{
			//if ( ie.IsReceptive == true )
			//{
				m_targetInteractee = ie;

				if ( chain == null )
				{
					Debug.LogError( "Chain is null!" );
					return;
				}

				switch ( chainType )
				{
					case HandlerChain.ChainType.Window:
						//Debug.Log( "Interactor.RunChain() Window " + chain );
						handle = 
							MEC.Timing.RunCoroutine( chain.RunWindowChain( ie, this ) );
						break;
					case HandlerChain.ChainType.WindowRelease:
					//	Debug.Log( "Interactor.RunChain() WindowRelease " + chain );
						handle = 
							MEC.Timing.RunCoroutine( chain.RunWindowReleaseChain( ie, this ) );
						break;
					case HandlerChain.ChainType.WindowAbort:
					//	Debug.Log( "Interactor.RunChain() WindowAbort " + chain );
						handle = 
							MEC.Timing.RunCoroutine( chain.RunWindowAbortChain( ie, this ) );
						break;
					case HandlerChain.ChainType.Handler:
						//Debug.Log( "Interactor.RunChain() Handler " + chain );
						handle = 
							MEC.Timing.RunCoroutine( chain.RunHandlerChain( ie, this ) );
						break;
					case HandlerChain.ChainType.HandlerRelease:
						//Debug.Log( "Interactor.RunChain() HandlerRelease " + chain );
						handle = 
							MEC.Timing.RunCoroutine( chain.RunHandlerReleaseChain( ie, this ) );
						break;
					case HandlerChain.ChainType.HandlerAbort:
						//Debug.Log( "Interactor.RunChain() AbortHandler " + chain );
						handle = 
							MEC.Timing.RunCoroutine( chain.RunHandlerAbortChain( ie, this ) );
						break;
				}
			//}
		}

		private void KillChain( ref MEC.CoroutineHandle handle )
		{
			if ( handle.IsRunning )
			{
				MEC.Timing.KillCoroutines( handle );
			}

			handle = MEC.CoroutineHandle.RawHandle;
		}
	}

}