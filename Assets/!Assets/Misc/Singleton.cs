namespace ProjectFound.Misc 
{

	using UnityEngine;

	public class Singleton<_T> : MonoBehaviour
		where _T : MonoBehaviour
	{
		private static _T m_instance;

		public static _T Instance
		{
			get
			{
				if ( m_instance == null )
				{
					m_instance = FindObjectOfType<_T>( );

					// If still null, create the instance
				}

				return m_instance;
			}
		}
	}

}
