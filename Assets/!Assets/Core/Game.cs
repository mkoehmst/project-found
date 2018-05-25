namespace ProjectFound.Core 
{

	public class Game : Misc.Singleton<Game>
	{
		private GameContext m_gameContext;

		void Start( )
		{
			m_gameContext = new GameContext( );

			m_gameContext.Setup( );
		}

		void Update( )
		{
			m_gameContext.Loop( );
		}
	}

}
