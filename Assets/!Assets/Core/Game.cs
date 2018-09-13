namespace ProjectFound.Core
{


	//[ExcludeFromSerialization(ExclusionOption.ExcludeGameObject)]
	public class Game : UnityEngine.MonoBehaviour
	{
		private GameContext _gameContext;
		[System.NonSerialized] private bool _doesNeedReloading;

		void Awake( )
		{
			_doesNeedReloading = false;

			if ( Autelia.Serialization.Serializer.IsLoading ) return;

			_gameContext = new GameContext( );
		}

		void Start( )
		{
			if ( Autelia.Serialization.Serializer.IsLoading ) return;

			_gameContext.Initialize( );
			_gameContext.Setup( );

			AssignStatics( );
		}

		void Update( )
		{
			if ( ReloadCheck( ) ) return ;

			_gameContext.InputMaster.TrackingLoop( );

			//if ( !_gameContext.UIMaster.IsCursorOverUI( ) )
			//{ 
				_gameContext.RaycastMaster.Loop( );
			//}
			//else
			//{
			//	int j = 22;
			//}

			_gameContext.InputMaster.MappingLoop( );
			_gameContext.UIMaster.Loop( );
		}

		void LateUpdate( )
		{
			if ( ReloadCheck( ) ) return ;
		}

		void FixedUpdate( )
		{
			if ( ReloadCheck( ) ) return ;
		}

		void OnDisable( )
		{
			//m_gameContext.Cleanup( );
		}

		private bool ReloadCheck( )
		{
			if ( Autelia.Serialization.Serializer.IsLoading )
			{
				_doesNeedReloading = true;

				return true;
			}

			if ( _doesNeedReloading == true )
			{
				_doesNeedReloading = false;
				ResetMasters( );
				AssignStatics( );
			}

			return false;
		}

		private void ResetMasters( )
		{
			_gameContext.RaycastMaster.Clear( );
		}
		private void AssignStatics( )
		{
			ContextHandler.AssignContext( _gameContext );
			MEC.CoroutineHandle.AssignRawHandle( );
		}
	}


}